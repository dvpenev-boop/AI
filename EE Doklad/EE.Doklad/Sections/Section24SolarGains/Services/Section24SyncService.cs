using System;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Models;
using EE.Doklad.Sections.Section24SolarGains.Models;
using EE.Doklad.Services;

namespace EE.Doklad.Sections.Section24SolarGains.Services
{
    /// <summary>
    /// Synchronizes Section 24 input rows from sections 6 (walls), 7 (roof), and 9 (windows/doors).
    /// Section 8 (floor) is intentionally excluded.
    /// </summary>
    public sealed class Section24SyncService
    {
        private const double ThetaSsDefault = 10.0;
        private const double HceDefault = 20.0; // W/(m2.K)
        private const double Sigma = 5.670374419e-8; // Stefan-Boltzmann constant
        private static readonly int[] MonthLengths = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        private readonly ClimateService _climateService;

        public Section24SyncService()
        {
            _climateService = new ClimateService(new JsonClimateRepository());
        }

        public void SyncFromReport(Report report, Section24SolarGainsData target)
        {
            ArgumentNullException.ThrowIfNull(report);
            ArgumentNullException.ThrowIfNull(target);

            int climateZone = ResolveClimateZone(report);
            var zoneData = _climateService.GetZone(climateZone);

            var monthlyProfile = BuildMonthlyProfileFromObjectData(report, zoneData);
            ApplyMonthlyProfileToTarget(target, monthlyProfile);

            target.Windows.Clear();
            target.OpaqueElements.Clear();

            AddWindowsFromSection9(report, target, zoneData, monthlyProfile.Hours, monthlyProfile.HeatingDays, monthlyProfile.CoolingDays);
            AddWallsFromSection6(report, target, zoneData, monthlyProfile.Hours);
            AddRoofsFromSection7(report, target, zoneData, monthlyProfile.Hours);
        }

        private sealed class MonthlyProfile
        {
            public double[] Hours { get; } = new double[12];
            public int[] HeatingDays { get; } = new int[12];
            public int[] CoolingDays { get; } = new int[12];
        }

        private static void ApplyMonthlyProfileToTarget(Section24SolarGainsData target, MonthlyProfile profile)
        {
            if (target.MonthlyData == null || target.MonthlyData.Length != 12)
                return;

            for (int i = 0; i < 12; i++)
            {
                target.MonthlyData[i].DeltaT_m = SafeAt(profile.Hours, i);
                target.MonthlyData[i].HeatingDays = profile.HeatingDays[i];
                target.MonthlyData[i].CoolingDays = profile.CoolingDays[i];
            }
        }

        private static MonthlyProfile BuildMonthlyProfileFromObjectData(Report report, ClimateZoneData zoneData)
        {
            var objectData = report.Sections
                .FirstOrDefault(s => s.Type == SectionType.ObjectData)
                ?.ObjectDataSectionData;

            // Active days in a non-leap reference year (365), indexed 0..364.
            var heatingDaysMask = new bool[365];
            var coolingDaysMask = new bool[365];

            if (objectData != null && objectData.HeatingSeasonEnabled)
            {
                if (TryParseMonthDay(zoneData.HeatingSeason.Start, out int hStartMonth, out int hStartDay) &&
                    TryParseMonthDay(zoneData.HeatingSeason.End, out int hEndMonth, out int hEndDay))
                {
                    MarkInclusivePeriod(heatingDaysMask, hStartMonth, hStartDay, hEndMonth, hEndDay);
                }
            }

            if (objectData != null && objectData.CoolingSeasonEnabled &&
                objectData.CoolingSeasonStartMonth.HasValue && objectData.CoolingSeasonStartDay.HasValue &&
                objectData.CoolingSeasonEndMonth.HasValue && objectData.CoolingSeasonEndDay.HasValue)
            {
                MarkInclusivePeriod(
                    coolingDaysMask,
                    objectData.CoolingSeasonStartMonth.Value,
                    objectData.CoolingSeasonStartDay.Value,
                    objectData.CoolingSeasonEndMonth.Value,
                    objectData.CoolingSeasonEndDay.Value);
            }

            var profile = new MonthlyProfile();
            int dayOfYear = 0;
            for (int m = 0; m < 12; m++)
            {
                int daysInMonth = MonthLengths[m];
                int heatInMonth = 0;
                int coolInMonth = 0;
                int unionInMonth = 0;
                for (int d = 0; d < daysInMonth; d++)
                {
                    bool h = heatingDaysMask[dayOfYear + d];
                    bool c = coolingDaysMask[dayOfYear + d];
                    if (h) heatInMonth++;
                    if (c) coolInMonth++;
                    if (h || c) unionInMonth++;
                }
                profile.HeatingDays[m] = heatInMonth;
                profile.CoolingDays[m] = coolInMonth;
                profile.Hours[m] = unionInMonth * 24.0;
                dayOfYear += daysInMonth;
            }

            return profile;
        }

        private static bool TryParseMonthDay(string? mmDashDd, out int month, out int day)
        {
            month = 0;
            day = 0;
            if (string.IsNullOrWhiteSpace(mmDashDd))
                return false;

            var parts = mmDashDd.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length != 2)
                return false;

            if (!int.TryParse(parts[0], out month) || !int.TryParse(parts[1], out day))
                return false;

            if (month < 1 || month > 12)
                return false;

            int maxDay = MonthLengths[month - 1];
            if (day < 1 || day > maxDay)
                return false;

            return true;
        }

        private static int DayOfYear(int month, int day)
        {
            int sum = 0;
            for (int i = 0; i < month - 1; i++)
                sum += MonthLengths[i];
            return sum + day; // 1-based
        }

        private static void MarkInclusivePeriod(bool[] activeDays, int startMonth, int startDay, int endMonth, int endDay)
        {
            if (activeDays == null || activeDays.Length != 365)
                return;

            if (startMonth < 1 || startMonth > 12 || endMonth < 1 || endMonth > 12)
                return;

            int start = DayOfYear(startMonth, Math.Clamp(startDay, 1, MonthLengths[startMonth - 1]));
            int end = DayOfYear(endMonth, Math.Clamp(endDay, 1, MonthLengths[endMonth - 1]));

            if (start <= end)
            {
                for (int d = start; d <= end; d++)
                    activeDays[d - 1] = true;
            }
            else
            {
                // Period across year boundary (e.g. Oct -> Apr)
                for (int d = start; d <= 365; d++)
                    activeDays[d - 1] = true;
                for (int d = 1; d <= end; d++)
                    activeDays[d - 1] = true;
            }
        }

        private static int ResolveClimateZone(Report report)
        {
            var objectData = report.Sections
                .FirstOrDefault(s => s.Type == SectionType.ObjectData)
                ?.ObjectDataSectionData;
            if (objectData == null)
                return 1;

            return Math.Clamp(objectData.ClimateZone, 1, 9);
        }

        private static double[] Interpolate(double[] a, double[] b)
        {
            var r = new double[12];
            for (int i = 0; i < 12; i++)
                r[i] = (SafeAt(a, i) + SafeAt(b, i)) * 0.5;
            return r;
        }

        private static double[] GetMonthlySolarByOrientation(ClimateZoneData zoneData, Orientation orientation, double[] monthlyHours)
        {
            var src = zoneData.Monthly.AvgFullSolarVerticalWm2;
            double[] n = src.TryGetValue("N", out var nArr) ? nArr : new double[12];
            double[] e = src.TryGetValue("E", out var eArr) ? eArr : new double[12];
            double[] w = src.TryGetValue("W", out var wArr) ? wArr : new double[12];
            double[] s = src.TryGetValue("S", out var sArr) ? sArr : new double[12];

            double[] wm2 = orientation switch
            {
                Orientation.North => n,
                Orientation.East => e,
                Orientation.West => w,
                Orientation.South => s,
                Orientation.NorthEast => Interpolate(n, e),
                Orientation.NorthWest => Interpolate(n, w),
                Orientation.SouthEast => Interpolate(s, e),
                Orientation.SouthWest => Interpolate(s, w),
                _ => s
            };

            return ToKwhPerM2(wm2, monthlyHours);
        }

        private static double[] GetMonthlySolarHorizontal(ClimateZoneData zoneData, double[] monthlyHours)
        {
            var src = zoneData.Monthly.AvgFullSolarVerticalWm2;
            double[] h = src.TryGetValue("H", out var hArr) ? hArr : new double[12];
            return ToKwhPerM2(h, monthlyHours);
        }

        private static double[] ToKwhPerM2(double[] irradianceWm2, double[] monthlyHours)
        {
            var result = new double[12];
            for (int i = 0; i < 12; i++)
                result[i] = SafeAt(irradianceWm2, i) * SafeAt(monthlyHours, i) / 1000.0;
            return result;
        }

        private static double SafeAt(double[] values, int index)
            => values != null && index >= 0 && index < values.Length ? values[index] : 0.0;

        private static double ComputeHre(double epsilon, double thetaSsC)
        {
            double tAbs = thetaSsC + 273.0;
            return 4.0 * epsilon * Sigma * tAbs * tAbs * tAbs;
        }

        private static double ComputeRse(double epsilon, double thetaSsC)
        {
            double hRe = ComputeHre(epsilon, thetaSsC);
            double denominator = HceDefault + hRe;
            return denominator > 0 ? 1.0 / denominator : 0.0;
        }

        private static double[] CopyOrDefault(double[]? source, double defaultValue)
        {
            if (source == null || source.Length != 12)
                return Enumerable.Repeat(defaultValue, 12).ToArray();
            return (double[])source.Clone();
        }

        private static double ResolveRoofU(RoofType roofType)
        {
            if (roofType.Mode == RoofMode.Warm)
                return roofType.WarmDetail?.Uw ?? 0.0;

            if (roofType.Mode == RoofMode.Cold)
                return roofType.ColdDetail?.Ur
                    ?? roofType.ColdDetail?.UwCalculated
                    ?? 0.0;

            return 0.0;
        }

        private static void AddWindowsFromSection9(
            Report report,
            Section24SolarGainsData target,
            ClimateZoneData zoneData,
            double[] monthlyHours,
            int[] heatingDays,
            int[] coolingDays)
        {
            var section9 = report.Sections.FirstOrDefault(s => s.Type == SectionType.Windows)?.WindowsSectionData;
            if (section9 == null)
                return;

            int index = 1;
            foreach (var batch in section9.WindowBatches)
            {
                if (batch.Count <= 0 || batch.AreaGross <= 0)
                    continue;

                double epsilon = batch.GlassEmissivity > 0 ? batch.GlassEmissivity : 0.84;
                double rSe = ComputeRse(epsilon, ThetaSsDefault);
                double[] hSol = GetMonthlySolarByOrientation(zoneData, batch.Orientation, monthlyHours);
                double[] fSh = CopyOrDefault(batch.FshDirMonthly, 1.0);
                double gHeat = ResolveWindowGEffNoObst(batch, isHeatingMode: true);
                double gCool = ResolveWindowGEffNoObst(batch, isHeatingMode: false);
                double[] gGl = BuildMonthlyModeWeightedG(gHeat, gCool, heatingDays, coolingDays);

                target.Windows.Add(new WindowElement
                {
                    Id = $"W{index++}-{batch.Orientation}",
                    A_wi = batch.Count * batch.AreaGross,
                    F_fr = batch.FrameFraction,
                    U_c = batch.UValue,
                    R_se = rSe,
                    F_sky = 0.5,
                    Epsilon = epsilon,
                    ThetaSs = ThetaSsDefault,
                    H_sol = hSol,
                    F_sh_obst = fSh,
                    G_gl = gGl,
                    G_gl_heat = gHeat,
                    G_gl_cool = gCool
                });
            }
        }

        private static double ResolveWindowGEffNoObst(WindowBatch batch, bool isHeatingMode)
        {
            double baseValue = isHeatingMode ? batch.GEffBaseHeat : batch.GEffBaseCool;
            bool hasShade = isHeatingMode ? batch.ShadingModeHeat > 0 : batch.ShadingModeCool > 0;
            double shadeFactor = isHeatingMode ? batch.ShadingReductionFactorHeat : batch.ShadingReductionFactorCool;
            return baseValue * (hasShade ? shadeFactor : 1.0);
        }

        private static double[] BuildMonthlyModeWeightedG(double gHeat, double gCool, int[] heatingDays, int[] coolingDays)
        {
            var result = new double[12];
            for (int m = 0; m < 12; m++)
            {
                double h = heatingDays != null && m < heatingDays.Length ? Math.Max(0, heatingDays[m]) : 0.0;
                double c = coolingDays != null && m < coolingDays.Length ? Math.Max(0, coolingDays[m]) : 0.0;
                double total = h + c;
                if (total <= 0)
                {
                    result[m] = gHeat;
                    continue;
                }

                result[m] = (gHeat * h + gCool * c) / total;
            }
            return result;
        }

        private static void AddWallsFromSection6(
            Report report,
            Section24SolarGainsData target,
            ClimateZoneData zoneData,
            double[] monthlyHours)
        {
            var section6 = report.Sections.FirstOrDefault(s => s.Type == SectionType.ExternalWalls)?.ExternalWallsSectionData;
            if (section6 == null)
                return;

            foreach (var wall in section6.WallTypes)
            {
                var areas = new List<(string Label, Orientation Orientation, double Area, double Alpha, double Epsilon)>
                {
                    ("N", Orientation.North, wall.FacadeNorth, wall.SurfaceProperties.AlphaDefault, wall.SurfaceProperties.EpsilonDefault),
                    ("NE", Orientation.NorthEast, wall.FacadeNorthEast, wall.SurfaceProperties.GetAlpha(WallOrientation.NE), wall.SurfaceProperties.GetEpsilon(WallOrientation.NE)),
                    ("E", Orientation.East, wall.FacadeEast, wall.SurfaceProperties.GetAlpha(WallOrientation.E), wall.SurfaceProperties.GetEpsilon(WallOrientation.E)),
                    ("SE", Orientation.SouthEast, wall.FacadeSouthEast, wall.SurfaceProperties.GetAlpha(WallOrientation.SE), wall.SurfaceProperties.GetEpsilon(WallOrientation.SE)),
                    ("S", Orientation.South, wall.FacadeSouth, wall.SurfaceProperties.GetAlpha(WallOrientation.S), wall.SurfaceProperties.GetEpsilon(WallOrientation.S)),
                    ("SW", Orientation.SouthWest, wall.FacadeSouthWest, wall.SurfaceProperties.GetAlpha(WallOrientation.SW), wall.SurfaceProperties.GetEpsilon(WallOrientation.SW)),
                    ("W", Orientation.West, wall.FacadeWest, wall.SurfaceProperties.GetAlpha(WallOrientation.W), wall.SurfaceProperties.GetEpsilon(WallOrientation.W)),
                    ("NW", Orientation.NorthWest, wall.FacadeNorthWest, wall.SurfaceProperties.GetAlpha(WallOrientation.NW), wall.SurfaceProperties.GetEpsilon(WallOrientation.NW))
                };

                foreach (var a in areas.Where(x => x.Area > 0))
                {
                    double rSe = ComputeRse(a.Epsilon, ThetaSsDefault);
                    target.OpaqueElements.Add(new OpaqueElement
                    {
                        Id = $"OP-W{wall.Index}-{a.Label}",
                        A_c = a.Area,
                        Alpha_sol = a.Alpha,
                        U_c = wall.Uw,
                        R_se = rSe,
                        F_sky = 0.5,
                        Epsilon = a.Epsilon,
                        ThetaSs = ThetaSsDefault,
                        H_sol = GetMonthlySolarByOrientation(zoneData, a.Orientation, monthlyHours),
                        F_sh_obst = Enumerable.Repeat(1.0, 12).ToArray()
                    });
                }
            }
        }

        private static void AddRoofsFromSection7(
            Report report,
            Section24SolarGainsData target,
            ClimateZoneData zoneData,
            double[] monthlyHours)
        {
            var section7 = report.Sections.FirstOrDefault(s => s.Type == SectionType.Roof)?.RoofSectionData;
            if (section7 == null)
                return;

            foreach (var roof in section7.RoofTypes)
            {
                if (roof.Area <= 0)
                    continue;

                double u = ResolveRoofU(roof);
                if (u <= 0)
                    continue;

                double epsilon = roof.SurfaceProperties.EpsilonDefault;
                double rSe = ComputeRse(epsilon, ThetaSsDefault);

                target.OpaqueElements.Add(new OpaqueElement
                {
                    Id = $"OP-R{roof.Number}",
                    A_c = roof.Area,
                    Alpha_sol = roof.SurfaceProperties.AlphaDefault,
                    U_c = u,
                    R_se = rSe,
                    F_sky = 1.0,
                    Epsilon = epsilon,
                    ThetaSs = ThetaSsDefault,
                    H_sol = GetMonthlySolarHorizontal(zoneData, monthlyHours),
                    F_sh_obst = Enumerable.Repeat(1.0, 12).ToArray()
                });
            }
        }
    }
}
