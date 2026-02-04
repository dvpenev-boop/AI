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

        private ZtuMonthlyResult CalculateMonth(
            ZtuZone zone,
            int monthIndex,
            double outdoorTempC,
            double indoorTempC)
        {
            var result = new ZtuMonthlyResult
            {
                MonthNumber = monthIndex + 1,
                MonthName = MonthNames[monthIndex],
                OutdoorTempC = outdoorTempC,
                IndoorTempC = indoorTempC
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

            // Стъпка 5: θztu,m = θe,a,m + bztu,m * (θint - θe,a,m)
            double tempZtu = outdoorTempC + bztu * (indoorTempC - outdoorTempC);
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

                    // За ztue (External): Hel,k,m = bztu,m * Uk,m * Ak
                    // За ztui (Internal): Hel,k,m = (1 - bztu,m) * Uk,m * Ak
                    if (zone.Type == ZtuType.External)
                    {
                        hel = bztu * element.UValue * element.Area;
                    }
                    else // Internal
                    {
                        hel = (1.0 - bztu) * element.UValue * element.Area;
                    }

                    influences.Add(new ZtuElementInfluence
                    {
                        MonthNumber = month + 1,
                        MonthName = MonthNames[month],
                        ElementName = element.Name,
                        UValue = element.UValue,
                        Area = element.Area,
                        Bztu = bztu,
                        Hel_WK = hel
                    });
                }
            }

            return influences;
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
    }
}
