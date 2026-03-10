using System;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Models;

namespace EE.Doklad.Services
{
    /// <summary>
    /// Калкулатор за месечни изчисления на неклиматизирани зони (ztu)
    /// </summary>
    public class UnconditionedZonesCalculator
    {
        private static readonly string[] MonthNames = new[]
        {
            "Януари", "Февруари", "Март", "Април", "Май", "Юни",
            "Юли", "Август", "Септември", "Октомври", "Ноември", "Декември"
        };

        /// <summary>
        /// Изчислява месечните параметри за неклиматизирана зона
        /// </summary>
        /// <param name="zone">Неклиматизираната зона</param>
        /// <param name="climateData">Климатични данни</param>
        /// <param name="indoorTempC">Вътрешна изчислителна температура на отопляемата зона (°C)</param>
        /// <returns>Списък с месечни резултати</returns>
        public ZtuMonthlyResults Calculate(
            ZtuZone zone,
            ClimateZoneData climateData,
            double indoorTempC = 20.0)
        {
            var results = new ZtuMonthlyResults
            {
                ZoneName = zone.Name,
                ZoneType = zone.Type
            };

            for (int month = 0; month < 12; month++)
            {
                var monthly = CalculateMonth(
                    zone,
                    month,
                    climateData.Monthly.AvgMonthlyTempC[month],
                    indoorTempC);

                results.Months.Add(monthly);
            }

            return results;
        }

        /// <summary>
        /// New overload: seasonal temperatures per month. thetaIntSummer is used for months May..Sep (5..9),
        /// winter months use thetaIntWinterCalc[m] unless isWinterOverride==true in which case winterOverrideValue is used.
        /// </summary>
        public ZtuMonthlyResults CalculateWithSeasonalTemps(
            ZtuZone zone,
            ClimateZoneData climateData,
            double thetaIntSummer = 25.0,
            double[]? thetaIntCoolingCalc = null,
            double[]? thetaIntWinterCalc = null,
            bool isWinterOverride = false,
            double? winterOverrideValue = null)
        {
            var results = new ZtuMonthlyResults
            {
                ZoneName = zone.Name,
                ZoneType = zone.Type
            };

            for (int month = 0; month < 12; month++)
            {
                double thetaUsed;
                int monthNumber = month + 1;
                if (monthNumber >= 5 && monthNumber <= 9)
                {
                    // Cooling months: use calculated cooling indoor temperature if available.
                    if (thetaIntCoolingCalc != null && thetaIntCoolingCalc.Length == 12)
                    {
                        thetaUsed = thetaIntCoolingCalc[month];
                    }
                    else
                    {
                        thetaUsed = thetaIntSummer;
                    }
                }
                else
                {
                    if (isWinterOverride && winterOverrideValue.HasValue)
                    {
                        thetaUsed = winterOverrideValue.Value;
                    }
                    else if (thetaIntWinterCalc != null && thetaIntWinterCalc.Length == 12)
                    {
                        thetaUsed = thetaIntWinterCalc[month];
                    }
                    else
                    {
                        // fallback
                        thetaUsed = winterOverrideValue ?? 20.0;
                    }
                }

                var monthly = CalculateMonth(
                    zone,
                    month,
                    climateData.Monthly.AvgMonthlyTempC[month],
                    thetaUsed);

                results.Months.Add(monthly);
            }

            return results;
        }

        private ZtuMonthlyResult CalculateMonth(
            ZtuZone zone,
            int monthIndex,
            double outdoorTempC,
            double thetaIntUsed)
        {
            var result = new ZtuMonthlyResult
            {
                MonthNumber = monthIndex + 1,
                MonthName = MonthNames[monthIndex],
                OutdoorTempC = outdoorTempC,
                IndoorTempC = thetaIntUsed,
                ThetaIntUsed_C = thetaIntUsed
            };

            // Стъпка 1: Hztu,e,m = Σ(Uk,m * Ak) за елементите към външна среда
            double hztuE = 0.0;
            foreach (var element in zone.ElementsToExternal)
            {
                hztuE += element.UValue * element.Area;
            }
            result.HztuE_WK = hztuE;

            // Стъпка 2: Hztc-ztu,m = Σ(Uk,m * Ak) за разделящите елементи
            double hztcZtu = 0.0;
            foreach (var element in zone.ElementsToBoundary)
            {
                hztcZtu += element.UValue * element.Area;
            }
            result.HztcZtu_WK = hztcZtu;

            // Стъпка 3: Hztu,tot,m = Hztu,e,m + Hztc-ztu,m
            double hztuTot = hztuE + hztcZtu;
            result.HztuTot_WK = hztuTot;

            // Стъпка 4: bztu,m = Hztu,e,m / Hztu,tot,m
            // Guard: ако Hztu,tot == 0 → bztu = 0
            double bztu = 0.0;
            if (hztuTot > 1e-6)
            {
                bztu = hztuE / hztuTot;
                // Ограничаваме в диапазона [0..1]
                bztu = Math.Max(0.0, Math.Min(1.0, bztu));
            }
            result.Bztu = bztu;

            // Стъпка 5: θztu,m = θe,a,m + bztu,m * (θint_used - θe,a,m)
            double tempZtu = outdoorTempC + bztu * (thetaIntUsed - outdoorTempC);
            result.TempZtu_C = tempZtu;

            return result;
        }

        /// <summary>
        /// Изчислява влиянието на ztu върху топлопреминаването на отопляемата зона
        /// </summary>
        /// <param name="zone">Неклиматизираната зона</param>
        /// <param name="monthlyResults">Резултати от Calculate()</param>
        /// <returns>Месечни стойности на Hel за всеки разделящ елемент</returns>
        public List<ZtuElementInfluence> CalculateInfluenceOnHtr(
            ZtuZone zone,
            ZtuMonthlyResults monthlyResults)
        {
            var influences = new List<ZtuElementInfluence>();

            for (int month = 0; month < 12; month++)
            {
                var monthly = monthlyResults.Months[month];
                double bztu = monthly.Bztu;

                foreach (var element in zone.ElementsToBoundary)
                {
                    double hel = 0.0;
                    double factor = 0.0;

                    // For External ztu: factor = bztu; for Internal: factor = 1 - bztu
                    if (zone.Type == ZtuType.External)
                    {
                        factor = bztu;
                        hel = factor * element.UValue * element.Area;
                    }
                    else // Internal
                    {
                        factor = 1.0 - bztu;
                        hel = factor * element.UValue * element.Area;
                    }

                    influences.Add(new ZtuElementInfluence
                    {
                        MonthNumber = month + 1,
                        MonthName = MonthNames[month],
                        ElementName = element.Name,
                        UValue = element.UValue,
                        Area = element.Area,
                        Bztu = bztu,
                        BztuFactor = factor,
                        Hel_WK = hel
                    });
                }
            }

            return influences;
        }

        /// <summary>
        /// Compute Qtr (kWh) per month for separating elements given existing monthlyResults and object/heat data
        /// </summary>
        public ZtuQtrResults CalculateQtrResults(
            ZtuZone zone,
            ZtuMonthlyResults monthlyResults,
            ObjectDataSectionData? objectData,
            HeatingSectionData? heatingData,
            CoolingSectionData? coolingData,
            UnconditionedZoneSectionData? unconditionedSectionData,
            ClimateZoneData climateData,
            int yearRef = 2024)
        {
            var qtrResults = new ZtuQtrResults();

            // Sum UA for separating elements is available per month in monthlyResults.Months[m].HztcZtu_WK
            // Compute heating hours via HeatingScheduleService
            var heatingHours = EE.Doklad.Services.HeatingScheduleService.ComputeHeatingHoursPerMonth(objectData, climateData, yearRef);
            var coolingHours = EE.Doklad.Services.HeatingScheduleService.ComputeCoolingHoursPerMonth(objectData, yearRef);

            // Compute thetaIntCalcHeating (effective indoor temperature for heating) from heating module/object data
            var thetaIntCalcHeating = ScheduleHelper.ComputeThetaIntCalcH(objectData, heatingData, climateData, yearRef);
            var thetaIntCalcCooling = ScheduleHelper.ComputeThetaIntCalcC(objectData, coolingData, yearRef);

            for (int m = 0; m < 12; m++)
            {
                var monthly = monthlyResults.Months[m];
                double sumUA_sep = monthly.HztcZtu_WK; // W/K

                // Adjacent temperature follows the calculated monthly ztu temperature.
                double thetaAdjUsed_heating = monthly.TempZtu_C;
                double thetaAdjUsed_cooling = monthly.TempZtu_C;

                double heatingHours_m = heatingHours != null && heatingHours.Length == 12 ? heatingHours[m] : 0.0;
                double coolingHours_m = coolingHours != null && coolingHours.Length == 12 ? coolingHours[m] : 0.0;

                // TEMPORARY FILTER: only consider cooling hours for May..Sep (months 5..9, zero-based 4..8)
                // Remove month filter when full cooling season/calendar is implemented.
                // TODO: Remove month filter when full cooling season/calendar is implemented.
                if (m < 4 || m > 8)
                {
                    coolingHours_m = 0.0;
                }

                // theta for heated indoor (from heating module) or fallback 20°C
                double thetaHeatedHeat = 20.0;
                if (thetaIntCalcHeating != null && thetaIntCalcHeating.Length == 12)
                    thetaHeatedHeat = thetaIntCalcHeating[m];

                // Qtr heating
                double deltaT_heat = thetaHeatedHeat - thetaAdjUsed_heating;
                double q_heat_kWh = 0.0;
                if (sumUA_sep > 0 && heatingHours_m > 0)
                {
                    q_heat_kWh = sumUA_sep * deltaT_heat * heatingHours_m / 1000.0;
                }

                // Qtr cooling: use calculated indoor cooling temperature from schedule/season.
                double thetaHeatedCool = coolingData?.DesignTemperature ?? 25.0;
                if (thetaIntCalcCooling != null && thetaIntCalcCooling.Length == 12)
                    thetaHeatedCool = thetaIntCalcCooling[m];
                double deltaT_cool = thetaAdjUsed_cooling - thetaHeatedCool;
                double q_cool_kWh = 0.0;
                if (sumUA_sep > 0 && coolingHours_m > 0)
                {
                    q_cool_kWh = sumUA_sep * deltaT_cool * coolingHours_m / 1000.0;
                }

                qtrResults.Months.Add(new ZtuQtrMonthResult
                {
                    MonthNumber = m + 1,
                    MonthName = monthly.MonthName,
                    OutdoorTempC = monthly.OutdoorTempC,
                    // keep explicit heat/cool adjacent temperatures so UI and calculations can show both
                    ThetaAdjHeat_C = thetaAdjUsed_heating,
                    ThetaAdjCool_C = thetaAdjUsed_cooling,
                    SumUA_Separating_WK = sumUA_sep,
                    HeatingHours_h = heatingHours_m,
                    CoolingHours_h = coolingHours_m,
                    Qtr_heat_kWh = q_heat_kWh,
                    Qtr_cool_kWh = q_cool_kWh
                });
            }

            return qtrResults;
        }
    }

    /// <summary>
    /// Резултати за всички месеци на една зона
    /// </summary>
    public class ZtuMonthlyResults
    {
        public string ZoneName { get; set; } = string.Empty;
        public ZtuType ZoneType { get; set; }
        public List<ZtuMonthlyResult> Months { get; } = new();
    }

    /// <summary>
    /// Резултати за един месец на една зона
    /// </summary>
    public class ZtuMonthlyResult
    {
        public int MonthNumber { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public double OutdoorTempC { get; set; }
        public double IndoorTempC { get; set; }
        /// <summary>
        /// The internal temperature used for this month (°C) — either summer fixed or winter calculated/overridden
        /// </summary>
        public double ThetaIntUsed_C { get; set; }

        /// <summary>
        /// Топлопреминаване на ztu към външна среда (W/K)
        /// </summary>
        public double HztuE_WK { get; set; }

        /// <summary>
        /// Топлопреминаване между отопляема зона и ztu (W/K)
        /// </summary>
        public double HztcZtu_WK { get; set; }

        /// <summary>
        /// Общо топлопреминаване на ztu (W/K)
        /// </summary>
        public double HztuTot_WK { get; set; }

        /// <summary>
        /// Редукционен фактор [0..1]
        /// </summary>
        public double Bztu { get; set; }

        /// <summary>
        /// Температура в неклиматизираната зона (°C)
        /// </summary>
        public double TempZtu_C { get; set; }
    }

    /// <summary>
    /// Qtr results (heating / cooling through separating elements) for one month
    /// </summary>
    public class ZtuQtrMonthResult
    {
        public int MonthNumber { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public double OutdoorTempC { get; set; }
    /// <summary>
    /// Temperature of the unconditioned space used for heating (adjacent)
    /// </summary>
    public double ThetaAdjHeat_C { get; set; }

    /// <summary>
    /// Temperature of the unconditioned space used for cooling (adjacent)
    /// </summary>
    public double ThetaAdjCool_C { get; set; }
        /// <summary>
        /// ΣUA for separating elements (Hztc-ztu) (W/K)
        /// </summary>
        public double SumUA_Separating_WK { get; set; }
        /// <summary>
        /// Effective heating hours (h) for this month based on schedule + holidays
        /// </summary>
        public double HeatingHours_h { get; set; }
        /// <summary>
        /// Cooling hours (temporary: May..Sep full month hours)
        /// </summary>
        public double CoolingHours_h { get; set; }
        /// <summary>
        /// Q through separating elements that affects heating (kWh)
        /// </summary>
        public double Qtr_heat_kWh { get; set; }
        /// <summary>
        /// Q through separating elements that affects cooling (kWh)
        /// </summary>
        public double Qtr_cool_kWh { get; set; }
    }

    public class ZtuQtrResults
    {
        public List<ZtuQtrMonthResult> Months { get; } = new();
        public double Annual_Qtr_heat_kWh => Months.Sum(m => m.Qtr_heat_kWh);
        public double Annual_Qtr_cool_kWh => Months.Sum(m => m.Qtr_cool_kWh);
    }


    /// <summary>
    /// Влияние на един елемент на ztu върху Htr за един месец
    /// </summary>
    public class ZtuElementInfluence
    {
        public int MonthNumber { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public string ElementName { get; set; } = string.Empty;
        public double UValue { get; set; }
        public double Area { get; set; }
        public double Bztu { get; set; }

        /// <summary>
        /// Редуцирано топлопреминаване (W/K)
        /// </summary>
        public double Hel_WK { get; set; }

        // ==== Presentation-friendly aliases used by the view XAML (backwards compatible names) ==== 
        public double U => UValue;
        public double Hel => Hel_WK;

        /// <summary>
        /// Factor applied to U*A for this element (bztu for external, 1-bztu for internal)
        /// </summary>
        public double BztuFactor { get; set; }
    }
}
