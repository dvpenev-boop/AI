using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcMonthlyHeatingOracle
    {
        private readonly EecalcMonthlyDaysOracle monthlyDaysOracle = new();
        private readonly EecalcHtrQtrOracle htrQtrOracle = new();

        public IReadOnlyList<EecalcMonthlyHeatingOracleRow> Calculate(EecalcEnvelopeFixture fixture)
        {
            ArgumentNullException.ThrowIfNull(fixture);

            var monthlyDays = monthlyDaysOracle.Calculate(fixture.Calculation);
            return monthlyDays.Select(month => CalculateMonth(fixture, month)).ToList();
        }

        public EecalcMonthlyHeatingOracleRow CalculateMonth(
            EecalcEnvelopeFixture fixture,
            EecalcMonthlyDaysOracleRow month)
        {
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(month);

            var calculation = fixture.Calculation;
            var avgTemp = GetAverageOutdoorTemperature(calculation, month.Month);
            var avgInnerHeatTemp = htrQtrOracle.CalculateAverageHeatTempCurrent(calculation, month);
            var hve = CalculateHve(calculation);
            var htr = htrQtrOracle.CalculateParameterHtr(
                fixture,
                avgTemp,
                avgInnerHeatTemp,
                out var hd,
                out var hg,
                out var huWalls,
                out var huCeilings,
                out var huFloors);
            var degreeHours = EecalcHtrQtrOracle.CalculateDegreeHours(calculation, month, avgTemp);
            var qtr = htr * degreeHours / 1000.0;
            var qve = hve * degreeHours / 1000.0;
            var qht = qtr + qve;
            var solar = GetSolarRadiation(calculation, month.Month);
            var transparentFsol = CalculateTrasparentFsol(fixture, solar);
            var nonTransparentFsol = CalculateNonTrasparentFsol(fixture, solar);
            var projectHours = CalculateProjectHours(calculation, month);
            var nonProjectHours = CalculateNonProjectHours(calculation, month);
            var totalHours = projectHours + nonProjectHours;
            var qgnRaw = (nonTransparentFsol + transparentFsol) * totalHours;
            var qgn = qgnRaw / 1000.0;
            var occupantHours = CalculateOccupantHours(calculation, month);
            var metabolicHeatPerArea = calculation.MetabolicHeat * occupantHours / 1000.0;
            var metabolicHeat = metabolicHeatPerArea * calculation.HeatedArea;
            var gamma = (qgn + metabolicHeat) / qht;
            var tau = calculation.HeatedArea * calculation.HeatCapacity / (htr + hve);
            var aH = 1.0 + tau / 15.0;
            var ni = CalculateNi(gamma, aH, out var niBranch);
            var rawQnd = qht - ni * qgn;
            var finalQnd = rawQnd / calculation.HeatedArea - ni * metabolicHeatPerArea;

            return new EecalcMonthlyHeatingOracleRow
            {
                Month = month.Month,
                MonthName = month.MonthName,
                WorkDays = month.WorkDays,
                Saturdays = month.Saturdays,
                Sundays = month.Sundays,
                Holidays = month.Holidays,
                AverageOutdoorTemperature = avgTemp,
                AverageInnerHeatTemp = avgInnerHeatTemp,
                ProjectHours = projectHours,
                NonProjectHours = nonProjectHours,
                TotalHours = totalHours,
                Hve = hve,
                Hd = hd,
                Hg = hg,
                HuWalls = huWalls,
                HuCeilings = huCeilings,
                HuFloors = huFloors,
                Hu = huWalls + huCeilings + huFloors,
                Htr = htr,
                DegreeHours = degreeHours,
                Qtr = qtr,
                Qve = qve,
                Qht = qht,
                TransparentFsol = transparentFsol,
                NonTransparentFsol = nonTransparentFsol,
                QgnRaw = qgnRaw,
                Qgn = qgn,
                OccupantHours = occupantHours,
                MetabolicHeatPerArea = metabolicHeatPerArea,
                MetabolicHeat = metabolicHeat,
                Tau = tau,
                AH = aH,
                Gamma = gamma,
                Ni = ni,
                NiBranch = niBranch,
                RawQnd = rawQnd,
                FinalQnd = finalQnd
            };
        }

        public double CalculateHve(EecalcValidationFixture fixture)
        {
            ArgumentNullException.ThrowIfNull(fixture);

            return fixture.HeatedVolume * fixture.Infiltration * 0.34;
        }

        public double CalculateTransparentFsol(
            double windowA,
            double windowG,
            double windowE,
            double sunShiningIntensity,
            bool horizontal = false)
        {
            var radiativeCoeff = 4m * (decimal)windowE * 0.0000000567m * (decimal)Math.Pow(283.0, 3.0);
            var loss = 0.04 * windowG * windowA * 11.0 * (double)radiativeCoeff;
            var directionFactor = horizontal ? 1.0 : 0.5;

            return windowA * windowG * sunShiningIntensity - directionFactor * loss;
        }

        public double CalculateTrasparentFsol(EecalcEnvelopeFixture fixture, EecalcSolarRadiationFixture solar)
        {
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(solar);

            var total = CalculateWallTransparentFsol(fixture.NorthWalls, solar.N);
            total += CalculateWallTransparentFsol(fixture.NorthEastWalls, (solar.N + solar.E) / 2.0);
            total += CalculateWallTransparentFsol(fixture.EastWalls, solar.E);
            total += CalculateWallTransparentFsol(fixture.SouthEastWalls, (solar.S + solar.E) / 2.0);
            total += CalculateWallTransparentFsol(fixture.SouthWalls, solar.S);
            total += CalculateWallTransparentFsol(fixture.SouthWestWalls, (solar.S + solar.W) / 2.0);
            total += CalculateWallTransparentFsol(fixture.WestWalls, solar.W);
            total += CalculateWallTransparentFsol(fixture.NorthWestWalls, (solar.N + solar.W) / 2.0);

            total += CalculateTransparentFsol(fixture.Roof.TransparentA[0], fixture.Roof.TransparentG[0], fixture.Roof.TransparentE[0], solar.N);
            total += CalculateTransparentFsol(fixture.Roof.TransparentA[1], fixture.Roof.TransparentG[1], fixture.Roof.TransparentE[1], (solar.N + solar.E) / 2.0);
            total += CalculateTransparentFsol(fixture.Roof.TransparentA[2], fixture.Roof.TransparentG[2], fixture.Roof.TransparentE[2], solar.E);
            total += CalculateTransparentFsol(fixture.Roof.TransparentA[3], fixture.Roof.TransparentG[3], fixture.Roof.TransparentE[3], (solar.S + solar.E) / 2.0);
            total += CalculateTransparentFsol(fixture.Roof.TransparentA[4], fixture.Roof.TransparentG[4], fixture.Roof.TransparentE[4], solar.S);
            total += CalculateTransparentFsol(fixture.Roof.TransparentA[5], fixture.Roof.TransparentG[5], fixture.Roof.TransparentE[5], (solar.S + solar.W) / 2.0);
            total += CalculateTransparentFsol(fixture.Roof.TransparentA[6], fixture.Roof.TransparentG[6], fixture.Roof.TransparentE[6], solar.W);
            total += CalculateTransparentFsol(fixture.Roof.TransparentA[7], fixture.Roof.TransparentG[7], fixture.Roof.TransparentE[7], (solar.N + solar.W) / 2.0);
            total += CalculateTransparentFsol(fixture.Roof.TransparentA[8], fixture.Roof.TransparentG[8], fixture.Roof.TransparentE[8], solar.H, horizontal: true);

            return total;
        }

        public double CalculateNonTransparentFsol(
            double outerWallAlfa,
            double outerWallU,
            double outerWallEpsi,
            double outerWallArea,
            double sunShiningIntensity,
            bool horizontal = false)
        {
            var absorbed = outerWallAlfa * 0.04 * outerWallU * outerWallArea;
            var radiativeCoeff = 4m * (decimal)outerWallEpsi * 0.0000000567m * (decimal)Math.Pow(283.0, 3.0);
            var loss = 0.04 * outerWallU * outerWallArea * 11.0 * (double)radiativeCoeff;
            var directionFactor = horizontal ? 1.0 : 0.5;

            return absorbed * sunShiningIntensity - directionFactor * loss;
        }

        public double CalculateNonTrasparentFsol(EecalcEnvelopeFixture fixture, EecalcSolarRadiationFixture solar)
        {
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(solar);

            var total = CalculateWallNonTransparentFsol(fixture.NorthWalls, solar.N);
            total += CalculateWallNonTransparentFsol(fixture.NorthEastWalls, (solar.N + solar.E) / 2.0);
            total += CalculateWallNonTransparentFsol(fixture.EastWalls, solar.E);
            total += CalculateWallNonTransparentFsol(fixture.SouthEastWalls, (solar.S + solar.E) / 2.0);
            total += CalculateWallNonTransparentFsol(fixture.SouthWalls, solar.S);
            total += CalculateWallNonTransparentFsol(fixture.SouthWestWalls, (solar.S + solar.W) / 2.0);
            total += CalculateWallNonTransparentFsol(fixture.WestWalls, solar.W);
            total += CalculateWallNonTransparentFsol(fixture.NorthWestWalls, (solar.N + solar.W) / 2.0);

            total += CalculateNonTransparentFsol(
                fixture.Roof.AccumulateNonTransparentAlfa,
                fixture.Roof.AccumulateNonTransparentU,
                fixture.Roof.AccumulateNonTransparentE,
                fixture.Roof.AccumulateNonTransparentA,
                solar.H,
                horizontal: true);

            return total;
        }

        public double CalculateNi(double gamma, double aH, out string branch)
        {
            if (gamma > 0.0 && Math.Abs(gamma - 1.0) > 0.01)
            {
                branch = "positive_power";
                return (1.0 - Math.Pow(gamma, aH)) / (1.0 - Math.Pow(gamma, aH + 1.0));
            }

            if (gamma < 0.0)
            {
                branch = "negative_gamma";
                return 1.0;
            }

            if (Math.Abs(gamma - 1.0) < 0.01)
            {
                branch = "near_one";
                return aH / (aH + 1.0);
            }

            branch = "fallback_zero";
            return 0.0;
        }

        private double CalculateWallTransparentFsol(EecalcWallDirectionFixture wall, double radiation)
        {
            return CalculateTransparentFsol(
                wall.AccumulateWindowA,
                wall.AccumulateWindowG,
                wall.AccumulateWindowE,
                radiation);
        }

        private double CalculateWallNonTransparentFsol(EecalcWallDirectionFixture wall, double radiation)
        {
            return CalculateNonTransparentFsol(
                wall.AccumulateOuterAlfa,
                wall.AccumulateOuterU,
                wall.AccumulateOuterE,
                wall.AccumulateOuterA,
                radiation);
        }

        private static double CalculateProjectHours(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            return month.WorkDays * Duration(fixture.WorkdaySchedule)
                + month.Saturdays * Duration(fixture.SaturdaySchedule)
                + month.Sundays * Duration(fixture.SundaySchedule);
        }

        private static double CalculateOccupantHours(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            return month.WorkDays * Duration(fixture.OccupantsWorkdaySchedule)
                + month.Saturdays * Duration(fixture.OccupantsSaturdaySchedule)
                + month.Sundays * Duration(fixture.OccupantsSundaySchedule);
        }

        private static double CalculateNonProjectHours(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            return month.WorkDays * (24.0 - Duration(fixture.WorkdaySchedule))
                + month.Saturdays * (24.0 - Duration(fixture.SaturdaySchedule))
                + month.Sundays * (24.0 - Duration(fixture.SundaySchedule))
                + month.Holidays * 24.0;
        }

        private static int Duration(EecalcDailySchedule schedule)
        {
            return schedule.EndHour - schedule.StartHour;
        }

        private static double GetAverageOutdoorTemperature(EecalcValidationFixture fixture, int month)
        {
            return fixture.AverageOutdoorTemperatureByMonth.TryGetValue(month, out var value) ? value : 0.0;
        }

        private static EecalcSolarRadiationFixture GetSolarRadiation(EecalcValidationFixture fixture, int month)
        {
            return fixture.SolarRadiationByMonth.TryGetValue(month, out var value)
                ? value
                : new EecalcSolarRadiationFixture();
        }
    }

    public sealed class EecalcMonthlyHeatingOracleRow
    {
        public int Month { get; init; }

        public string MonthName { get; init; } = string.Empty;

        public int WorkDays { get; init; }

        public int Saturdays { get; init; }

        public int Sundays { get; init; }

        public int Holidays { get; init; }

        public double AverageOutdoorTemperature { get; init; }

        public double AverageInnerHeatTemp { get; init; }

        public double ProjectHours { get; init; }

        public double NonProjectHours { get; init; }

        public double TotalHours { get; init; }

        public double Hve { get; init; }

        public double Hd { get; init; }

        public double Hg { get; init; }

        public double HuWalls { get; init; }

        public double HuCeilings { get; init; }

        public double HuFloors { get; init; }

        public double Hu { get; init; }

        public double Htr { get; init; }

        public double DegreeHours { get; init; }

        public double Qtr { get; init; }

        public double Qve { get; init; }

        public double Qht { get; init; }

        public double TransparentFsol { get; init; }

        public double NonTransparentFsol { get; init; }

        public double QgnRaw { get; init; }

        public double Qgn { get; init; }

        public double OccupantHours { get; init; }

        public double MetabolicHeatPerArea { get; init; }

        public double MetabolicHeat { get; init; }

        public double Tau { get; init; }

        public double AH { get; init; }

        public double Gamma { get; init; }

        public double Ni { get; init; }

        public string NiBranch { get; init; } = string.Empty;

        public double RawQnd { get; init; }

        public double FinalQnd { get; init; }
    }
}
