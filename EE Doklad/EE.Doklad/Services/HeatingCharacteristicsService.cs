using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EE.Doklad.Models;

namespace EE.Doklad.Services
{
    public sealed class HeatingCharacteristicsSnapshot
    {
        public EnvelopeMetric Walls { get; init; } = EnvelopeMetric.Empty;
        public EnvelopeMetric Roof { get; init; } = EnvelopeMetric.Empty;
        public EnvelopeMetric Floor { get; init; } = EnvelopeMetric.Empty;
        public EnvelopeMetric Windows { get; init; } = EnvelopeMetric.Empty;
        public double ThermalBridgesH_WK { get; init; }
        public double ZtuH_WK { get; init; }
        public double InfiltrationRate_AirChangesPerHour { get; init; }
        public double BuildingVolume_m3 { get; init; }
        public double HeatedArea_m2 { get; init; }
        public double DesignIndoorTemp_C { get; init; }
        public double EffectiveIndoorTemp_C { get; init; }
        public double DesignOutdoorTemp_C { get; init; }
        public double DegreeDays_Kd { get; init; }
        public double HeatingOperatingHours_h { get; init; }
        public double HeatingSeasonHours_h { get; init; }
        public double[] MonthlyOutdoorTemps_C { get; init; } = new double[12];
        public double[] MonthlyFullHours_h { get; init; } = new double[12];
        public double[] MonthlySetbackHours_h { get; init; } = new double[12];
        public double SetbackIndoorTemp_C { get; init; }
    }

    public sealed class EnvelopeMetric
    {
        public static EnvelopeMetric Empty { get; } = new();

        public double U { get; init; }
        public double Area { get; init; }
        public double H { get; init; }
        public double Htb { get; init; }
    }

    public static class HeatingCharacteristicsService
    {
        public static HeatingCharacteristicsSnapshot Build(
            Report? report,
            ObjectDataSectionData? objectData,
            HeatingSectionData? heatingData,
            ClimateZoneData? climateData,
            IReadOnlyList<HeatingScheduleService.HeatingHoursBreakdown>? breakdown)
        {
            var monthlyFullHours = (breakdown ?? Array.Empty<HeatingScheduleService.HeatingHoursBreakdown>())
                .Select(x => x.FullHours)
                .Concat(Enumerable.Repeat(0.0, 12))
                .Take(12)
                .ToArray();
            var monthlySetbackHours = (breakdown ?? Array.Empty<HeatingScheduleService.HeatingHoursBreakdown>())
                .Select(x => x.SetbackHours)
                .Concat(Enumerable.Repeat(0.0, 12))
                .Take(12)
                .ToArray();
            var heatingOperatingHours = monthlyFullHours.Sum();
            var heatingSeasonHoursByMonth = ComputeHeatingSeasonHoursPerMonth(breakdown);
            var heatingSeasonHours = heatingSeasonHoursByMonth.Sum();
            var effectiveIndoorTemp = ComputeAnnualEffectiveIndoorTemperature(heatingData, breakdown, heatingSeasonHoursByMonth);
            var wallsMetric = BuildWallsMetric(report?.Sections?.FirstOrDefault(s => s.Type == SectionType.ExternalWalls)?.ExternalWallsSectionData);
            var roofMetric = BuildRoofMetric(report?.Sections?.FirstOrDefault(s => s.Type == SectionType.Roof)?.RoofSectionData);
            var windowsMetric = BuildWindowsMetric(report?.Sections?.FirstOrDefault(s => s.Type == SectionType.Windows)?.WindowsSectionData);

            return new HeatingCharacteristicsSnapshot
            {
                Walls = wallsMetric,
                Roof = roofMetric,
                Floor = BuildFloorMetric(report?.Sections?.FirstOrDefault(s => s.Type == SectionType.Floor)?.FloorSectionData),
                Windows = windowsMetric,
                ThermalBridgesH_WK =
                    wallsMetric.Htb +
                    roofMetric.Htb +
                    windowsMetric.Htb,
                ZtuH_WK = BuildZtuMetric(
                    report?.Sections?.FirstOrDefault(s => s.Type == SectionType.UnconditionedZones)?.UnconditionedZoneSectionData,
                    objectData,
                    heatingData,
                    report?.Sections?.FirstOrDefault(s => s.CoolingSectionData != null)?.CoolingSectionData,
                    breakdown),
                InfiltrationRate_AirChangesPerHour = heatingData?.Infiltration ?? 0.0,
                BuildingVolume_m3 = ParseDoubleOrZero(objectData?.GrossHeatedVolume) > 0
                    ? ParseDoubleOrZero(objectData?.GrossHeatedVolume)
                    : ParseDoubleOrZero(objectData?.NetHeatedVolume),
                HeatedArea_m2 = ParseDoubleOrZero(objectData?.HeatedArea),
                DesignIndoorTemp_C = heatingData?.DesignTemperature ?? 20.0,
                SetbackIndoorTemp_C = heatingData?.ReductionTemperature ?? 16.0,
                EffectiveIndoorTemp_C = effectiveIndoorTemp,
                DesignOutdoorTemp_C = climateData?.DesignOutdoorTempC ?? 0.0,
                DegreeDays_Kd = climateData?.DegreeDays19C ?? 0.0,
                HeatingOperatingHours_h = heatingOperatingHours,
                HeatingSeasonHours_h = heatingSeasonHours,
                MonthlyOutdoorTemps_C = climateData?.Monthly?.AvgMonthlyTempC?.Length == 12
                    ? climateData.Monthly.AvgMonthlyTempC.ToArray()
                    : new double[12],
                MonthlyFullHours_h = monthlyFullHours,
                MonthlySetbackHours_h = monthlySetbackHours
            };
        }

        private static EnvelopeMetric BuildWallsMetric(ExternalWallsSectionData? data)
        {
            if (data == null)
            {
                return EnvelopeMetric.Empty;
            }

            var wallTypes = data.WallTypes.ToList();
            double area = wallTypes.Sum(w => Math.Max(0.0, w.Area));
            double ua = wallTypes.Sum(w => Math.Max(0.0, w.Area) * Math.Max(0.0, w.Uw));
            double htb = wallTypes.Sum(w => Math.Max(0.0, w.ThermalBridges?.Htb ?? 0.0));
            double h = wallTypes.Sum(w =>
            {
                double hel = Math.Max(0.0, w.Area) * Math.Max(0.0, w.Uw);
                double stored = Math.Max(0.0, w.ThermalBridges?.Htotal ?? 0.0);
                return stored > 0 ? stored : hel;
            });

            return new EnvelopeMetric
            {
                Area = area,
                U = area > 0 ? ua / area : 0.0,
                H = h,
                Htb = htb
            };
        }

        private static EnvelopeMetric BuildRoofMetric(RoofSectionData? data)
        {
            if (data == null)
            {
                return EnvelopeMetric.Empty;
            }

            var roofTypes = data.RoofTypes.Where(r => r.IsConfigured).ToList();
            double area = roofTypes.Sum(r => Math.Max(0.0, r.Area));
            double ua = roofTypes.Sum(r => Math.Max(0.0, r.Area) * Math.Max(0.0, r.UValue));
            double htb = roofTypes.Sum(r => Math.Max(0.0, r.ThermalBridges?.Htb ?? 0.0));
            double h = roofTypes.Sum(r =>
            {
                double hel = Math.Max(0.0, r.Area) * Math.Max(0.0, r.UValue);
                double stored = Math.Max(0.0, r.ThermalBridges?.Htotal ?? 0.0);
                return stored > 0 ? stored : hel;
            });

            return new EnvelopeMetric
            {
                Area = area,
                U = area > 0 ? ua / area : 0.0,
                H = h,
                Htb = htb
            };
        }

        private static EnvelopeMetric BuildFloorMetric(FloorSectionData? data)
        {
            if (data == null)
            {
                return EnvelopeMetric.Empty;
            }

            var items = data.FloorItems.ToList();
            double area = items.Sum(i => Math.Max(0.0, i.Area));
            double h = items.Sum(i =>
            {
                if (i.PeriodicHcAdj > 0)
                {
                    return i.PeriodicHcAdj;
                }

                return Math.Max(0.0, i.UValue) * Math.Max(0.0, i.Area);
            });

            return new EnvelopeMetric
            {
                Area = area,
                U = area > 0 ? h / area : 0.0,
                H = h
            };
        }

        private static EnvelopeMetric BuildWindowsMetric(WindowsSectionData? data)
        {
            if (data == null)
            {
                return EnvelopeMetric.Empty;
            }

            var rows = WindowCalculator.BuildSystemLossSummary(data.WindowBatches);
            double area = rows.Sum(r => Math.Max(0.0, r.TotalArea));
            double ua = rows.Sum(r => Math.Max(0.0, r.TotalArea) * Math.Max(0.0, r.AverageUw));

            return new EnvelopeMetric
            {
                Area = area,
                U = area > 0 ? ua / area : 0.0,
                H = rows.Sum(r => Math.Max(0.0, r.Htotal)),
                Htb = rows.Sum(r => Math.Max(0.0, r.Htb))
            };
        }

        private static double BuildZtuMetric(
            UnconditionedZoneSectionData? data,
            ObjectDataSectionData? objectData,
            HeatingSectionData? heatingData,
            CoolingSectionData? coolingData,
            IReadOnlyList<HeatingScheduleService.HeatingHoursBreakdown>? breakdown)
        {
            if (data == null)
            {
                return 0.0;
            }

            double[] thetaIntWinterCalc = ScheduleHelper.ComputeThetaIntCalcH(heatingData, breakdown);
            double[] thetaIntCoolingCalc = ScheduleHelper.ComputeThetaIntCalcC(objectData, coolingData);

            double sumHel = 0.0;
            foreach (var zone in data.Zones)
            {
                double hztuE = zone.Type == ZtuType.External
                    ? zone.ElementsToExternal.Sum(x => Math.Max(0.0, x.UValue) * Math.Max(0.0, x.Area))
                    : 0.0;

                double hztc = zone.ElementsToBoundary.Sum(x => Math.Max(0.0, x.UValue) * Math.Max(0.0, x.Area));
                double htot = hztuE + hztc;
                double bztu = htot > 1e-6 ? Math.Clamp(hztuE / htot, 0.0, 1.0) : 0.0;

                _ = thetaIntWinterCalc;
                _ = thetaIntCoolingCalc;

                double helFactor = zone.Type == ZtuType.External ? bztu : (1.0 - bztu);
                sumHel += helFactor * hztc;
            }

            return sumHel;
        }

        private static double[] ComputeHeatingSeasonHoursPerMonth(
            IReadOnlyList<HeatingScheduleService.HeatingHoursBreakdown>? breakdown)
        {
            return (breakdown ?? Array.Empty<HeatingScheduleService.HeatingHoursBreakdown>())
                .Select(x => x.FullHours + x.SetbackHours)
                .Concat(Enumerable.Repeat(0.0, 12))
                .Take(12)
                .ToArray();
        }

        private static double ComputeAnnualEffectiveIndoorTemperature(
            HeatingSectionData? heatingData,
            IReadOnlyList<HeatingScheduleService.HeatingHoursBreakdown>? breakdown,
            double[] heatingSeasonHoursByMonth)
        {
            double designTemp = heatingData?.DesignTemperature ?? 20.0;
            var monthlyEffectiveTemps = ScheduleHelper.ComputeThetaIntCalcH(heatingData, breakdown);
            double totalSeasonHours = heatingSeasonHoursByMonth.Sum();
            if (totalSeasonHours <= 0.0)
            {
                return designTemp;
            }

            double weightedTheta = 0.0;
            for (int month = 0; month < 12; month++)
            {
                weightedTheta += monthlyEffectiveTemps[month] * heatingSeasonHoursByMonth[month];
            }

            return weightedTheta / totalSeasonHours;
        }

        private static double ParseDoubleOrZero(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0.0;
            }

            var normalized = value.Replace(',', '.');
            return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double result)
                ? result
                : 0.0;
        }
    }
}
