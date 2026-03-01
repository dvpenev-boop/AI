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

        #region Sampling-based helpers (solar geometry + shadow sampling)

        // Convert degrees to radians
        private static double Deg2Rad(double d) => d * Math.PI / 180.0;
        private static double Rad2Deg(double r) => r * 180.0 / Math.PI;

        // Compute sun altitude (alpha) and azimuth (A) for given latitude (deg), declination (deg) and hour angle omega (rad)
        // Returns: altitude in radians, azimuth in degrees from North clockwise (0=N, 90=E, 180=S, 270=W)
        // Hour angle omega: negative = morning (East, before solar noon), positive = afternoon (West)
        private static (double altitudeRad, double azimuthDeg) SunPosition(double latitudeDeg, double declinationDeg, double omegaRad)
        {
            double phi   = Deg2Rad(latitudeDeg);
            double delta = Deg2Rad(declinationDeg);

            // Solar altitude (elevation above horizon)
            // sin α = sin φ · sin δ + cos φ · cos δ · cos ω
            double sinAlpha = Math.Sin(phi) * Math.Sin(delta)
                            + Math.Cos(phi) * Math.Cos(delta) * Math.Cos(omegaRad);
            double alpha = Math.Asin(Math.Clamp(sinAlpha, -1.0, 1.0));

            // Solar azimuth – formula gives angle from South, positive toward West:
            //   Az_fromSouth = atan2( sin(ω),  cos(ω)·sin(φ) − tan(δ)·cos(φ) )
            // Convert to from-North clockwise: Az_fromNorth = (Az_fromSouth + 180) mod 360
            double az_s = Rad2Deg(Math.Atan2(
                Math.Sin(omegaRad),
                Math.Cos(omegaRad) * Math.Sin(phi) - Math.Tan(delta) * Math.Cos(phi)));
            double azDeg = (az_s + 180.0 + 360.0) % 360.0;

            return (alpha, azDeg);
        }

        // Compute sunrise/sunset hour angle (rad) for given latitude and declination
        private static double SunriseHourAngle(double latitudeDeg, double declinationDeg)
        {
            double phi = Deg2Rad(latitudeDeg);
            double delta = Deg2Rad(declinationDeg);
            // cos omega0 = -tan phi * tan delta
            double cosw = -Math.Tan(phi) * Math.Tan(delta);
            if (cosw <= -1.0) return Math.PI; // polar day
            if (cosw >= 1.0) return 0.0; // polar night
            return Math.Acos(Math.Clamp(cosw, -1.0, 1.0));
        }

        // Compute per-month sample array of F_sh values by sampling sun positions during daytime
        private static double[] ComputeMonthlySamplesFsh(double wk, double hk, Orientation orientation, List<ShadingObject> shadings, double latitude, double declinationDeg)
        {
            var samples = ComputeMonthlyDetailedSamples(wk, hk, orientation, shadings, latitude, declinationDeg);
            if (samples.Count == 0) return Array.Empty<double>();
            return samples.Select(s => s.FshDir).ToArray();
        }

        // Per-sample structure
        private class SampleResult
        {
            public double HOverhang;
            public double WFinLeft;
            public double WFinRight;
            public double HObstacle;
            public double HSun;
            public double WSun;
            public double FshDir;
        }

        // Compute per-sample detailed shading results for a month by sampling hour angles
        private static List<SampleResult> ComputeMonthlyDetailedSamples(double wk, double hk, Orientation orientation, List<ShadingObject> shadings, double latitude, double declinationDeg)
        {
            var results = new List<SampleResult>();

            // sunrise/sunset hour angle
            double omega0 = SunriseHourAngle(latitude, declinationDeg);
            if (omega0 <= 0) return results; // no sun

            // sample N points across day (including sunrise/sunset). Use 25 samples (hourly-ish)
            int N = 25;
            for (int i = 0; i < N; i++)
            {
                double frac = (double)i / (N - 1);
                double omega = -omega0 + frac * (2.0 * omega0); // radians

                var (alphaRad, azimuthDeg) = SunPosition(latitude, declinationDeg, omega);
                if (alphaRad <= 0) continue; // sun below horizon

                // facade azimuth (deg from North clockwise)
                double facadeAz = OrientationToAzimuth(orientation);

                // Signed azimuth difference of sun relative to facade normal
                // gamma_wf > 0  → sun to the right of facade normal
                // gamma_wf < 0  → sun to the left of facade normal
                double gamma_wf = NormalizeAngleDeg(azimuthDeg - facadeAz); // –180..+180

                // Sun must be on the lit side of the facade (|gamma_wf| < 90°)
                bool sunFacesFacade = Math.Abs(gamma_wf) < 90.0;

                // ── Overhang (Навес) ───────────────────────────────────────────────────
                // Геометрия по Гл. 5 (7257_1): във вертикалния профилен ъгъл
                // φ_v спрямо нормалата на фасадата:
                //   tan(φ_v) = tan(α) / |cos(γ_wf)|
                // Търсената сянка върху фасадата се измерва надолу по височина.
                // При трасировка от точка на прозореца към слънцето пресичането с
                // долния ръб на навеса е в рамките на d, когато:
                //   x = h * cot(α) * cos(γ_wf) ≤ D
                // оттук максималната засенчена височина е:
                //   h_shadow,max = D * tan(α) / |cos(γ_wf)|
                // и реалното засенчване на прозореца: h_ov = max(0, h_shadow,max − L)
                // → по-високите слънчеви ъгли (лято) дават по-голямо засенчване.
                double maxHov = 0.0;
                if (sunFacesFacade)
                {
                    double cosGamma = Math.Abs(Math.Cos(Deg2Rad(gamma_wf)));
                    double tanAlpha = Math.Tan(alphaRad);
                    if (tanAlpha > 1e-9 && cosGamma > 1e-6)
                    {
                        foreach (var ov in shadings.Where(s => s.Type == ShadingType.Overhang))
                        {
                            double D = ov.Depth;
                            double L = ov.Distance;
                            if (D <= 0) continue;
                            // Максимална сянка върху фасадата по височина
                            double hShadow = D * tanAlpha / cosGamma;
                            // L е вертикалният просвет между навеса и горния ръб на прозореца
                            double hOnWindow = Math.Max(0.0, hShadow - L);
                            maxHov = Math.Max(maxHov, Math.Min(hOnWindow, hk));
                        }
                    }
                }

                // ── Lateral fins (Странични ребра) ───────────────────────────────────
                // The left fin shades from the left side of the window; it is active when the
                // sun is to the LEFT of the facade normal (gamma_wf < 0).
                // The right fin shades from the right side; active when gamma_wf > 0.
                //
                // Horizontal profile angle β_h (планов ъгъл) ≈ |γ_wf| (Гл. 5)
                // Трасировка в план: x = D при y_shadow = D · tan|γ_wf|.
                // Ширина на засенчване върху фасадата:
                //   w_shadow = D · tan|γ_wf| − L   ≥ 0
                //
                // Both fins can be active simultaneously for different halves of the window.
                double maxWfinL = 0.0;
                double maxWfinR = 0.0;
                if (sunFacesFacade)
                {
                    double absGammaRad = Math.Abs(Deg2Rad(gamma_wf));
                    // Only project fin shadow when sun is not directly normal (avoid ÷0)
                    if (absGammaRad > Deg2Rad(1.0))
                    {
                        double tanAbsGamma = Math.Tan(absGammaRad);

                        // Left fin → active when sun is to the LEFT (gamma_wf < 0)
                        if (gamma_wf < 0)
                        {
                            foreach (var fin in shadings.Where(s => s.Type == ShadingType.LeftFin))
                            {
                                double D = fin.Depth;
                                double L = fin.Distance;
                                if (D <= 0) continue;
                                double wShadow = D * tanAbsGamma;
                                double wOnWindow = Math.Max(0.0, wShadow - L);
                                maxWfinL = Math.Max(maxWfinL, Math.Min(wOnWindow, wk));
                            }
                        }

                        // Right fin → active when sun is to the RIGHT (gamma_wf > 0)
                        if (gamma_wf > 0)
                        {
                            foreach (var fin in shadings.Where(s => s.Type == ShadingType.RightFin))
                            {
                                double D = fin.Depth;
                                double L = fin.Distance;
                                if (D <= 0) continue;
                                double wShadow = D * tanAbsGamma;
                                double wOnWindow = Math.Max(0.0, wShadow - L);
                                maxWfinR = Math.Max(maxWfinR, Math.Min(wOnWindow, wk));
                            }
                        }
                    }
                }

                // obstacles
                double maxHobst = 0.0;
                foreach (var ob in shadings.Where(s => s.Type == ShadingType.Obstacle))
                {
                    double H = ob.Depth; // interpret as obstacle top height above window top (m)
                    double dist = ob.Distance; // horizontal distance from facade to obstacle (m)
                    if (H <= 0 || dist <= 0) continue;
                    // obstacle elevation angle
                    double thetaObs = Math.Atan2(H, dist);
                    // if obstacle elevation angle > sun altitude, there will be shadow
                    if (thetaObs > alphaRad)
                    {
                        // simple proportional shadow: fraction of window height blocked ~ (thetaObs - alpha)/thetaObs
                        double fracObs = Math.Clamp((thetaObs - alphaRad) / thetaObs, 0.0, 1.0);
                        double hObs = fracObs * hk;
                        maxHobst = Math.Max(maxHobst, Math.Min(hObs, hk));
                    }
                }

                double hSun = Math.Max(0.0, hk - (maxHobst + maxHov));
                double wSun = Math.Max(0.0, wk - (maxWfinL + maxWfinR));
                double fsh = (hSun * wSun) / (hk * wk);

                results.Add(new SampleResult
                {
                    HOverhang = maxHov,
                    WFinLeft = maxWfinL,
                    WFinRight = maxWfinR,
                    HObstacle = maxHobst,
                    HSun = hSun,
                    WSun = wSun,
                    FshDir = Math.Clamp(fsh, 0.0, 1.0)
                });
            }

            return results;
        }

        // Map Orientation enum to facade azimuth (deg from North clockwise)
        private static double OrientationToAzimuth(Orientation o)
        {
            return o switch
            {
                Orientation.North => 0.0,
                Orientation.NorthEast => 45.0,
                Orientation.East => 90.0,
                Orientation.SouthEast => 135.0,
                Orientation.South => 180.0,
                Orientation.SouthWest => 225.0,
                Orientation.West => 270.0,
                Orientation.NorthWest => 315.0,
                _ => 180.0
            };
        }

        private static double NormalizeAngleDeg(double a)
        {
            double v = a % 360.0;
            if (v < -180.0) v += 360.0;
            if (v > 180.0) v -= 360.0;
            return v;
        }

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
            // If geometry invalid -> no shading effect
            if (wk <= 0 || hk <= 0)
                return Enumerable.Repeat(1.0, 12).ToArray();

            var shadingsList = shadings?.ToList() ?? new List<ShadingObject>();
            // If no shading objects provided, return 1.0 for all months
            if (shadingsList.Count == 0)
                return Enumerable.Repeat(1.0, 12).ToArray();

            var result = new double[12];
            // We'll compute monthly F_sh,dir by sampling the sun across the daytime
            for (int m = 0; m < 12; m++)
            {
                double deltaDeg = SolarDeclinationByMonth[m];
                double[] monthlySamples = ComputeMonthlySamplesFsh(wk, hk, orientation, shadingsList, latitude, deltaDeg);
                // average across samples (samples array may be empty if sun never up -> treat as no shading)
                if (monthlySamples.Length == 0)
                    result[m] = 1.0;
                else
                    result[m] = Math.Clamp(monthlySamples.Average(), 0.0, 1.0);
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
            var results = new List<MonthlyShadingResult>();
            if (wk <= 0 || hk <= 0)
            {
                for (int m = 0; m < 12; m++)
                {
                    results.Add(new MonthlyShadingResult { Month = m + 1, MonthName = MonthNames[m], FshDir = 1.0 });
                }
                return results;
            }

            var shadingsList = shadings?.ToList() ?? new List<ShadingObject>();
            for (int m = 0; m < 12; m++)
            {
                double deltaDeg = SolarDeclinationByMonth[m];
                // Compute per-sample values and then aggregate (mean)
                var samples = ComputeMonthlyDetailedSamples(wk, hk, orientation, shadingsList, latitude, deltaDeg);

                if (samples.Count == 0)
                {
                    results.Add(new MonthlyShadingResult { Month = m + 1, MonthName = MonthNames[m], FshDir = 1.0 });
                    continue;
                }

                // average over samples
                double avgHOver = samples.Average(s => s.HOverhang);
                double avgWFinL = samples.Average(s => s.WFinLeft);
                double avgWFinR = samples.Average(s => s.WFinRight);
                double avgHObs = samples.Average(s => s.HObstacle);
                double avgHSun = samples.Average(s => s.HSun);
                double avgWSun = samples.Average(s => s.WSun);
                double avgFsh = samples.Average(s => s.FshDir);

                results.Add(new MonthlyShadingResult
                {
                    Month = m + 1,
                    MonthName = MonthNames[m],
                    HOverhang = avgHOver,
                    WFinLeft = avgWFinL,
                    WFinRight = avgWFinR,
                    HObstacle = avgHObs,
                    HSun = avgHSun,
                    WSun = avgWSun,
                    FshDir = Math.Clamp(avgFsh, 0.0, 1.0)
                });
            }

            return results;
        }

        #endregion
    }
}
