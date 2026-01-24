using System;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Models;

namespace EE.Doklad.Services
{
    /// <summary>
    /// Калкулатор за засенчване по БДС EN ISO 52016-1
    /// Изчислява месечни коефициенти F_sh,dir за прозорци
    /// </summary>
    public static class ShadingCalculator
    {
        #region Месечни константи

        /// <summary>
        /// Слънчева деклинация δ_m (градуси) за всеки месец
        /// </summary>
        private static readonly double[] SolarDeclinationByMonth = new double[12]
        {
            -20.8, // Jan
            -13.3, // Feb
            -2.4,  // Mar
            9.5,   // Apr
            18.8,  // May
            23.1,  // Jun
            21.1,  // Jul
            13.3,  // Aug
            2.0,   // Sep
            -9.8,  // Oct
            -19.1, // Nov
            -23.1  // Dec
        };

        /// <summary>
        /// Имена на месеците (за визуализация)
        /// </summary>
        private static readonly string[] MonthNames = new string[12]
        {
            "Януари", "Февруари", "Март", "Април", "Май", "Юни",
            "Юли", "Август", "Септември", "Октомври", "Ноември", "Декември"
        };

        #endregion

        #region Корелационни коефициенти

        /// <summary>
        /// Коефициенти за навеси (Overhang) - период лято (Jun-Sep), северно полукълбо
        /// </summary>
        private static readonly Dictionary<OrientationBucket, OverhangCoefficients> OverhangCoeffs = new()
        {
            { OrientationBucket.South,     new OverhangCoefficients(-3.023, 0.045, 1.285, -0.006) },
            { OrientationBucket.SouthEast, new OverhangCoefficients(-1.255, 0.015, 0.905, -0.008) },
            { OrientationBucket.East,      new OverhangCoefficients(-0.684, 0.005, 0.610, -0.004) },
            { OrientationBucket.NorthEast, new OverhangCoefficients(-0.654, 0.006, 0.616, -0.006) },
            { OrientationBucket.North,     new OverhangCoefficients(-0.726, 0.007, 0.616, -0.007) }
        };

        /// <summary>
        /// Коефициенти за ребра (Fins) - период лято (Jun-Sep), северно полукълбо
        /// </summary>
        private static readonly Dictionary<OrientationBucket, FinCoefficients> FinCoeffs = new()
        {
            { OrientationBucket.South,     new FinCoefficients(-1.175, 0.012, 0.860, -0.008) },
            { OrientationBucket.SouthEast, new FinCoefficients(-0.799, 0.009, 0.684, -0.006) },
            { OrientationBucket.East,      new FinCoefficients(0.118, -0.014, 0.005, 0.010) },
            { OrientationBucket.NorthEast, new FinCoefficients(0.155, -0.041, -0.680, 0.009) },
            { OrientationBucket.North,     new FinCoefficients(0.275, -0.133, 0.641, 0.039) }
        };

        #endregion

        #region Публични методи

        /// <summary>
        /// Изчислява месечни коефициенти F_sh,dir[m] за прозорец
        /// </summary>
        /// <param name="wk">Ширина на прозореца (m)</param>
        /// <param name="hk">Височина на прозореца (m)</param>
        /// <param name="orientation">Фасада/ориентация</param>
        /// <param name="shadings">Списък с обекти за засенчване</param>
        /// <param name="latitude">Географска ширина (градуси)</param>
        /// <param name="northHemisphere">Северно полукълбо</param>
        /// <returns>Масив от 12 коефициента (Jan..Dec), всеки в [0..1]</returns>
        public static double[] CalculateFshDirMonthly(
            double wk,
            double hk,
            Orientation orientation,
            IEnumerable<ShadingObject> shadings,
            double latitude = 42.7,
            bool northHemisphere = true)
        {
            if (wk <= 0 || hk <= 0)
                return Enumerable.Repeat(1.0, 12).ToArray();

            var shadingsList = shadings?.ToList() ?? new List<ShadingObject>();
            if (shadingsList.Count == 0)
                return Enumerable.Repeat(1.0, 12).ToArray();

            var result = new double[12];
            var orientBucket = MapOrientationToBucket(orientation);

            for (int m = 0; m < 12; m++)
            {
                double deltaM = SolarDeclinationByMonth[m];

                // 1. Изчисли засенчване от навеси (overhangs)
                double hOverhang = CalculateOverhangShadowHeight(
                    shadingsList.Where(s => s.Type == ShadingType.Overhang),
                    hk, orientBucket, latitude, deltaM);

                // 2. Изчисли засенчване от странични ребра
                double wFinRight = CalculateFinShadowWidth(
                    shadingsList.Where(s => s.Type == ShadingType.RightFin),
                    wk, orientBucket, latitude, deltaM);

                double wFinLeft = CalculateFinShadowWidth(
                    shadingsList.Where(s => s.Type == ShadingType.LeftFin),
                    wk, orientBucket, latitude, deltaM);

                // 3. Засенчване от препятствия (за бъдещо разширение)
                double hObstacle = 0.0; // TODO: Implement obstacle logic

                // 4. Изчисли осветени размери
                double hSun = Math.Max(0, hk - (hObstacle + hOverhang));
                double wSun = Math.Max(0, wk - (wFinRight + wFinLeft));

                // 5. Коефициент на намаление
                double fshDir = (hSun * wSun) / (hk * wk);
                result[m] = Math.Clamp(fshDir, 0.0, 1.0);
            }

            return result;
        }

        /// <summary>
        /// Изчислява детайлни месечни резултати (за UI preview)
        /// </summary>
        public static List<MonthlyShadingResult> CalculateDetailedMonthly(
            double wk,
            double hk,
            Orientation orientation,
            IEnumerable<ShadingObject> shadings,
            double latitude = 42.7,
            bool northHemisphere = true)
        {
            if (wk <= 0 || hk <= 0)
                return Enumerable.Range(0, 12).Select(m => new MonthlyShadingResult
                {
                    Month = m + 1,
                    MonthName = MonthNames[m],
                    FshDir = 1.0
                }).ToList();

            var shadingsList = shadings?.ToList() ?? new List<ShadingObject>();
            var results = new List<MonthlyShadingResult>();
            var orientBucket = MapOrientationToBucket(orientation);

            for (int m = 0; m < 12; m++)
            {
                double deltaM = SolarDeclinationByMonth[m];

                double hOverhang = CalculateOverhangShadowHeight(
                    shadingsList.Where(s => s.Type == ShadingType.Overhang),
                    hk, orientBucket, latitude, deltaM);

                double wFinRight = CalculateFinShadowWidth(
                    shadingsList.Where(s => s.Type == ShadingType.RightFin),
                    wk, orientBucket, latitude, deltaM);

                double wFinLeft = CalculateFinShadowWidth(
                    shadingsList.Where(s => s.Type == ShadingType.LeftFin),
                    wk, orientBucket, latitude, deltaM);

                double hObstacle = 0.0;

                double hSun = Math.Max(0, hk - (hObstacle + hOverhang));
                double wSun = Math.Max(0, wk - (wFinRight + wFinLeft));
                double fshDir = (hSun * wSun) / (hk * wk);

                results.Add(new MonthlyShadingResult
                {
                    Month = m + 1,
                    MonthName = MonthNames[m],
                    HOverhang = hOverhang,
                    WFinLeft = wFinLeft,
                    WFinRight = wFinRight,
                    HObstacle = hObstacle,
                    HSun = hSun,
                    WSun = wSun,
                    FshDir = Math.Clamp(fshDir, 0.0, 1.0)
                });
            }

            return results;
        }

        #endregion

        #region Вътрешни методи за изчисления

        /// <summary>
        /// Изчислява височина на засенчване от навеси за един месец
        /// </summary>
        private static double CalculateOverhangShadowHeight(
            IEnumerable<ShadingObject> overhangs,
            double hk,
            OrientationBucket orientBucket,
            double latitude,
            double deltaM)
        {
            var overhangsList = overhangs.ToList();
            if (overhangsList.Count == 0)
                return 0.0;

            var coeffs = OverhangCoeffs[orientBucket];
            double maxShadow = 0.0;

            foreach (var ovh in overhangsList)
            {
                double d = ovh.Depth;
                double l = ovh.Distance;

                if (d <= 0 || hk <= 0)
                    continue;

                double p1 = d / hk;
                double p2 = l / hk;

                // Корелационна формула (1) от методиката
                // h_ovh = (1 - Hk) * { 1 + [(A1 + B1*c_south*(φ_w - δ_m)) * P1 + 
                //                          (A2 + B2*c_south*(φ_w - δ_m)) * P2] }
                // За простота: c_south = -1 (южно полукълбо коригира знака)
                // TODO: Имплементирай пълната формула при нужда
                
                // Временна приближена формула:
                double latDiff = latitude - deltaM;
                double term1 = (coeffs.A1 + coeffs.B1 * (-1.0) * latDiff) * p1;
                double term2 = (coeffs.A2 + coeffs.B2 * (-1.0) * latDiff) * p2;
                double hOvhNorm = 1.0 + term1 + term2;
                double hOvh = (1 - hk) * hOvhNorm; // Това е опростена версия
                
                // По-реалистична връзка (нормализирана):
                // h_ovh ≈ H_k * (коефициент базиран на геометрия)
                // За най-простата версия:
                hOvh = hk * Math.Max(0, Math.Min(1.0, 0.3 * (d / hk) + 0.1 * (l / hk)));

                maxShadow = Math.Max(maxShadow, hOvh);
            }

            return Math.Min(maxShadow, hk);
        }

        /// <summary>
        /// Изчислява ширина на засенчване от ребра за един месец
        /// </summary>
        private static double CalculateFinShadowWidth(
            IEnumerable<ShadingObject> fins,
            double wk,
            OrientationBucket orientBucket,
            double latitude,
            double deltaM)
        {
            var finsList = fins.ToList();
            if (finsList.Count == 0)
                return 0.0;

            var coeffs = FinCoeffs[orientBucket];
            double maxShadow = 0.0;

            foreach (var fin in finsList)
            {
                double d = fin.Depth;
                double l = fin.Distance;

                if (d <= 0 || wk <= 0)
                    continue;

                double p1 = d / wk;
                double p2 = l / wk;

                // Корелационна формула (3) от методиката (аналогична на overhangs)
                // Временна приближена формула:
                double latDiff = latitude - deltaM;
                double term1 = (coeffs.A1 + coeffs.B1 * (-1.0) * latDiff) * p1;
                double term2 = (coeffs.A2 + coeffs.B2 * (-1.0) * latDiff) * p2;
                
                // Опростена връзка:
                double wFin = wk * Math.Max(0, Math.Min(1.0, 0.3 * (d / wk) + 0.1 * (l / wk)));

                maxShadow = Math.Max(maxShadow, wFin);
            }

            return Math.Min(maxShadow, wk);
        }

        #endregion

        #region Помощни методи

        /// <summary>
        /// Мапва ориентация към bucket (закръгляване към 45°)
        /// </summary>
        private static OrientationBucket MapOrientationToBucket(Orientation orientation)
        {
            return orientation switch
            {
                Orientation.South => OrientationBucket.South,
                Orientation.SouthEast => OrientationBucket.SouthEast,
                Orientation.SouthWest => OrientationBucket.SouthEast, // ЮЗ е близо до ЮИ
                Orientation.East => OrientationBucket.East,
                Orientation.West => OrientationBucket.East, // Запад е симетрично на изток
                Orientation.NorthEast => OrientationBucket.NorthEast,
                Orientation.NorthWest => OrientationBucket.NorthEast,
                Orientation.North => OrientationBucket.North,
                _ => OrientationBucket.South
            };
        }

        #endregion

        #region Вътрешни структури

        private enum OrientationBucket
        {
            South,
            SouthEast, // Включва ЮИ и ЮЗ
            East,      // Включва И и З
            NorthEast, // Включва СИ и СЗ
            North
        }

        private record OverhangCoefficients(double A1, double B1, double A2, double B2);
        private record FinCoefficients(double A1, double B1, double A2, double B2);

        #endregion
    }
}
