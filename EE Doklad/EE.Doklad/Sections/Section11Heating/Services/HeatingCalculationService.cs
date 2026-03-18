using System;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Models;
using EE.Doklad.Sections.Section11Heating.Models;
using ClimateDb = EE.Doklad.Sections.Section11Heating.Models.ClimateDatabase;

namespace EE.Doklad.Sections.Section11Heating.Services;

public class HeatingCalculationService
{
    private readonly Dictionary<HeatingCalculationMethod, ISolarStrategy> _strategies = new()
    {
        [HeatingCalculationMethod.AuerSoftware] = new AuerSolarStrategy(),
        [HeatingCalculationMethod.Rd0220_3] = new Rd0220SolarStrategy(),
        [HeatingCalculationMethod.Ashrae8760] = new AshraeStrategy(),
    };

    public (List<HeatingMonthlyResult> Monthly, HeatingAnnualResult Annual) Calculate(
        HeatingCalculationMethod method,
        IEnumerable<WallData> walls,
        IEnumerable<WindowData> windows,
        IEnumerable<RoofData> roofs,
        double htr,
        double hve,
        double cm,
        double thetaI,
        double area,
        int climateZone,
        IReadOnlyList<int> heatingMonths,
        Func<int, double> getQint)
    {
        var strategy = _strategies[method];
        double hTotal = htr + hve;
        double tau = hTotal > 0.0 ? cm / hTotal : 0.0;
        double aH = HeatingConstants.AH0 + tau / HeatingConstants.TauH0;

        var monthly = new List<HeatingMonthlyResult>();

        foreach (int monthIndex in heatingMonths.Distinct().OrderBy(m => m))
        {
            double te = ClimateDb.GetTe(climateZone, monthIndex);
            int dt = ClimateDb.DtHours[monthIndex];
            double qht = Math.Max(0.0, hTotal * Math.Max(0.0, thetaI - te) * dt / 1000.0);

            double qsol;
            try
            {
                qsol = strategy.CalcQsol(walls, windows, roofs, climateZone, monthIndex);
            }
            catch (NotImplementedException ex)
            {
                return (monthly, new HeatingAnnualResult
                {
                    IsValid = false,
                    ErrorMessage = ex.Message,
                    Htr = htr,
                    Hve = hve,
                    Htotal = hTotal,
                    Cm = cm,
                    Tau = tau,
                    AH = aH
                });
            }

            double qint = getQint(monthIndex);
            double qgn = qsol + qint;
            double gamma = qht > 1e-9 ? qgn / qht : 0.0;
            double eta = CalcEta(gamma, aH);
            double qh = Math.Max(0.0, qht - eta * qgn);

            monthly.Add(new HeatingMonthlyResult
            {
                MonthIndex = monthIndex,
                MonthName = ClimateDb.MonthNames[monthIndex],
                Te = te,
                Qht = qht,
                Qgn = qgn,
                Qsol = qsol,
                Qint = qint,
                Gamma = gamma,
                Eta = eta,
                QH = qh,
                IsHeating = thetaI > te
            });
        }

        return (monthly, new HeatingAnnualResult
        {
            IsValid = true,
            Htr = htr,
            Hve = hve,
            Htotal = hTotal,
            Cm = cm,
            Tau = tau,
            AH = aH,
            QH_total_kWh = monthly.Sum(r => r.QH),
            QH_per_m2 = area > 0.0 ? monthly.Sum(r => r.QH) / area : 0.0,
            Qht_total = monthly.Sum(r => r.Qht),
            Qgn_total = monthly.Sum(r => r.Qgn),
            Qsol_total = monthly.Sum(r => r.Qsol),
            Qint_total = monthly.Sum(r => r.Qint)
        });
    }

    private static double CalcEta(double gamma, double aH)
    {
        if (gamma <= 0.0)
        {
            return 1.0;
        }

        if (Math.Abs(gamma - 1.0) < 1e-6)
        {
            return aH / (aH + 1.0);
        }

        double gammaPowA = Math.Pow(gamma, aH);
        double gammaPowA1 = Math.Pow(gamma, aH + 1.0);
        double denominator = 1.0 - gammaPowA1;
        if (Math.Abs(denominator) < 1e-9)
        {
            return 0.0;
        }

        double eta = (1.0 - gammaPowA) / denominator;
        return double.IsFinite(eta) ? Math.Clamp(eta, 0.0, 1.0) : 0.0;
    }
}

public class HeatingAnnualResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public double Htr { get; set; }
    public double Hve { get; set; }
    public double Htotal { get; set; }
    public double Cm { get; set; }
    public double Tau { get; set; }
    public double AH { get; set; }
    public double QH_total_kWh { get; set; }
    public double QH_per_m2 { get; set; }
    public double Qht_total { get; set; }
    public double Qgn_total { get; set; }
    public double Qsol_total { get; set; }
    public double Qint_total { get; set; }
}
