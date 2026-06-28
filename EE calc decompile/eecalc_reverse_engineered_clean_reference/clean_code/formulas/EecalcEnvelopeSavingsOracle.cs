using System;
using System.Collections.Generic;
using System.Linq;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcEnvelopeSavingsOracle
    {
        public EecalcEnvelopeSavingsResult Calculate(
            EecalcEnvelopeFixture baseLine,
            EecalcEnvelopeFixture esm,
            Func<EecalcEnvelopeFixture, double> energyCalculator)
        {
            ArgumentNullException.ThrowIfNull(baseLine);
            ArgumentNullException.ThrowIfNull(esm);
            ArgumentNullException.ThrowIfNull(energyCalculator);

            var baseLineEnergy = energyCalculator(baseLine);
            var esmEnergy = energyCalculator(esm);
            var totalSaving = baseLineEnergy - esmEnergy;
            var candidates = BuildChangedEnvelopeMeasures(baseLine, esm);

            if (candidates.Count == 0)
            {
                return new EecalcEnvelopeSavingsResult
                {
                    BaseLineEnergy = baseLineEnergy,
                    EsmEnergy = esmEnergy,
                    TotalSaving = totalSaving,
                    Items = new List<EecalcEnvelopeSavingItem>()
                };
            }

            var virtualItems = candidates
                .Select(candidate =>
                {
                    var baseLineVirtualFixture = Clone(baseLine);
                    candidate.ApplyEsm(baseLineVirtualFixture, esm);
                    var baseLineVirtualEnergy = energyCalculator(baseLineVirtualFixture);

                    var esmVirtualFixture = Clone(esm);
                    candidate.ApplyEsm(esmVirtualFixture, baseLine);
                    var esmVirtualEnergy = energyCalculator(esmVirtualFixture);

                    return new EecalcEnvelopeSavingItem
                    {
                        Tag = candidate.Tag,
                        Row = candidate.Row,
                        OldValue = candidate.OldValue,
                        NewValue = candidate.NewValue,
                        VirtualEnergy = baseLineVirtualEnergy,
                        VirtualSaving = baseLineEnergy - baseLineVirtualEnergy,
                        VirtualEnergyNMinusOne = esmVirtualEnergy,
                        VirtualSavingNMinusOne = esmVirtualEnergy - esmEnergy
                    };
                })
                .ToList();

            var virtualSavingTotal = virtualItems.Sum(item => item.VirtualSaving);
            var totalSavingRatio = virtualSavingTotal == 0.0 ? 0.0 : totalSaving / virtualSavingTotal;
            var unscaledItems = virtualItems
                .Select(item =>
                {
                    var part = virtualSavingTotal == 0.0 ? 0.0 : item.VirtualSaving / virtualSavingTotal;
                    var actualSaving = totalSaving == 0.0
                        ? 0.0
                        : totalSaving * (item.VirtualSaving / totalSaving * totalSavingRatio
                            + item.VirtualSavingNMinusOne / totalSaving) / 2.0;

                    return new EecalcEnvelopeSavingItem
                    {
                        Tag = item.Tag,
                        Row = item.Row,
                        OldValue = item.OldValue,
                        NewValue = item.NewValue,
                        VirtualEnergy = item.VirtualEnergy,
                        VirtualSaving = item.VirtualSaving,
                        VirtualEnergyNMinusOne = item.VirtualEnergyNMinusOne,
                        VirtualSavingNMinusOne = item.VirtualSavingNMinusOne,
                        Part = part,
                        ActualSaving = actualSaving
                    };
                })
                .ToList();
            var unscaledActualSavingTotal = unscaledItems.Sum(item => item.ActualSaving);
            var actualSavingScale = unscaledActualSavingTotal == 0.0 ? 0.0 : totalSaving / unscaledActualSavingTotal;
            var items = unscaledItems
                .Select(item => new EecalcEnvelopeSavingItem
                {
                    Tag = item.Tag,
                    Row = item.Row,
                    OldValue = item.OldValue,
                    NewValue = item.NewValue,
                    VirtualEnergy = item.VirtualEnergy,
                    VirtualSaving = item.VirtualSaving,
                    VirtualEnergyNMinusOne = item.VirtualEnergyNMinusOne,
                    VirtualSavingNMinusOne = item.VirtualSavingNMinusOne,
                    Part = totalSaving == 0.0 ? 0.0 : item.ActualSaving * actualSavingScale / totalSaving,
                    ActualSaving = item.ActualSaving * actualSavingScale
                })
                .ToList();
            items = ApplyNegativeSavingsCorrection(totalSaving, items);

            return new EecalcEnvelopeSavingsResult
            {
                BaseLineEnergy = baseLineEnergy,
                EsmEnergy = esmEnergy,
                TotalSaving = totalSaving,
                Items = items
            };
        }

        private static List<EecalcEnvelopeSavingItem> ApplyNegativeSavingsCorrection(
            double totalSaving,
            List<EecalcEnvelopeSavingItem> items)
        {
            if (!items.Any(item => item.ActualSaving > 0.0) || !items.Any(item => item.ActualSaving < 0.0))
            {
                return items;
            }

            var positiveTotal = items.Where(item => item.ActualSaving > 0.0).Sum(item => item.ActualSaving);
            var negativeAbsTotal = items.Where(item => item.ActualSaving < 0.0).Sum(item => Math.Abs(item.ActualSaving));
            var correctedPositiveTotal = totalSaving + negativeAbsTotal;
            return items
                .Select(item =>
                {
                    if (item.ActualSaving <= 0.0)
                    {
                        return item;
                    }

                    var part = positiveTotal == 0.0 ? 0.0 : item.ActualSaving / positiveTotal;
                    return new EecalcEnvelopeSavingItem
                    {
                        Tag = item.Tag,
                        Row = item.Row,
                        OldValue = item.OldValue,
                        NewValue = item.NewValue,
                        VirtualEnergy = item.VirtualEnergy,
                        VirtualSaving = item.VirtualSaving,
                        VirtualEnergyNMinusOne = item.VirtualEnergyNMinusOne,
                        VirtualSavingNMinusOne = item.VirtualSavingNMinusOne,
                        Part = part,
                        ActualSaving = correctedPositiveTotal * part
                    };
                })
                .ToList();
        }

        public static EecalcEnvelopeFixture Clone(EecalcEnvelopeFixture source)
        {
            ArgumentNullException.ThrowIfNull(source);

            return new EecalcEnvelopeFixture
            {
                Id = source.Id,
                Calculation = Clone(source.Calculation),
                NorthWalls = Clone(source.NorthWalls),
                NorthEastWalls = Clone(source.NorthEastWalls),
                EastWalls = Clone(source.EastWalls),
                SouthEastWalls = Clone(source.SouthEastWalls),
                SouthWalls = Clone(source.SouthWalls),
                SouthWestWalls = Clone(source.SouthWestWalls),
                WestWalls = Clone(source.WestWalls),
                NorthWestWalls = Clone(source.NorthWestWalls),
                Roof = Clone(source.Roof),
                Floor = Clone(source.Floor)
            };
        }

        private static List<EnvelopeMeasure> BuildChangedEnvelopeMeasures(
            EecalcEnvelopeFixture baseLine,
            EecalcEnvelopeFixture esm)
        {
            var measures = new List<EnvelopeMeasure>();
            AddIfChanged(
                measures,
                "UouterWalls",
                "U outer walls",
                WeightedWallOuterU(baseLine),
                WeightedWallOuterU(esm),
                ApplyOuterWallsEsm);
            AddIfChanged(
                measures,
                "Uwindows",
                "U windows",
                WeightedWindowU(baseLine),
                WeightedWindowU(esm),
                ApplyWindowsEsm);
            AddIfChanged(
                measures,
                "Unontransparent",
                "U non-transparent roof",
                baseLine.Roof.AccumulateNonTransparentU,
                esm.Roof.AccumulateNonTransparentU,
                ApplyNonTransparentRoofEsm);
            AddIfChanged(
                measures,
                "Ufloor",
                "U floor",
                baseLine.Floor.AccumulateFloorU,
                esm.Floor.AccumulateFloorU,
                ApplyFloorEsm);

            return measures;
        }

        private static void AddIfChanged(
            ICollection<EnvelopeMeasure> measures,
            string tag,
            string row,
            double oldValue,
            double newValue,
            Action<EecalcEnvelopeFixture, EecalcEnvelopeFixture> applyEsm)
        {
            if (Math.Abs(oldValue - newValue) < 0.0000001)
            {
                return;
            }

            measures.Add(new EnvelopeMeasure(tag, row, oldValue, newValue, applyEsm));
        }

        private static double WeightedWallOuterU(EecalcEnvelopeFixture fixture)
        {
            var walls = WallDirections(fixture);
            var area = walls.Sum(wall => wall.AccumulateOuterA);
            return area == 0.0
                ? 0.0
                : walls.Sum(wall => wall.AccumulateOuterA * wall.AccumulateOuterU) / area;
        }

        private static double WeightedWindowU(EecalcEnvelopeFixture fixture)
        {
            var walls = WallDirections(fixture);
            var area = walls.Sum(wall => wall.AccumulateWindowA);
            return area == 0.0
                ? 0.0
                : walls.Sum(wall => wall.AccumulateWindowA * wall.AccumulateWindowU) / area;
        }

        private static IReadOnlyList<EecalcWallDirectionFixture> WallDirections(EecalcEnvelopeFixture fixture)
        {
            return new[]
            {
                fixture.NorthWalls,
                fixture.NorthEastWalls,
                fixture.EastWalls,
                fixture.SouthEastWalls,
                fixture.SouthWalls,
                fixture.SouthWestWalls,
                fixture.WestWalls,
                fixture.NorthWestWalls
            };
        }

        private static void ApplyOuterWallsEsm(EecalcEnvelopeFixture target, EecalcEnvelopeFixture esm)
        {
            var targetWalls = WallDirections(target);
            var esmWalls = WallDirections(esm);
            for (var index = 0; index < targetWalls.Count; index++)
            {
                targetWalls[index].AccumulateOuterU = esmWalls[index].AccumulateOuterU;
                CopyArray(esmWalls[index].OuterU, targetWalls[index].OuterU);
            }
        }

        private static void ApplyWindowsEsm(EecalcEnvelopeFixture target, EecalcEnvelopeFixture esm)
        {
            var targetWalls = WallDirections(target);
            var esmWalls = WallDirections(esm);
            for (var index = 0; index < targetWalls.Count; index++)
            {
                targetWalls[index].AccumulateWindowU = esmWalls[index].AccumulateWindowU;
            }
        }

        private static void ApplyNonTransparentRoofEsm(EecalcEnvelopeFixture target, EecalcEnvelopeFixture esm)
        {
            target.Roof.AccumulateNonTransparentU = esm.Roof.AccumulateNonTransparentU;
            CopyArray(esm.Roof.NonTransparentU, target.Roof.NonTransparentU);
        }

        private static void ApplyFloorEsm(EecalcEnvelopeFixture target, EecalcEnvelopeFixture esm)
        {
            target.Floor.AccumulateFloorU = esm.Floor.AccumulateFloorU;
        }

        private static EecalcValidationFixture Clone(EecalcValidationFixture source)
        {
            return new EecalcValidationFixture
            {
                Id = source.Id,
                Scenario = source.Scenario,
                ClimateZoneId = source.ClimateZoneId,
                FirstMonth = source.FirstMonth,
                LastMonth = source.LastMonth,
                FirstDay = source.FirstDay,
                LastDay = source.LastDay,
                HeatedArea = source.HeatedArea,
                HeatedVolume = source.HeatedVolume,
                Infiltration = source.Infiltration,
                HeatCapacity = source.HeatCapacity,
                MetabolicHeat = source.MetabolicHeat,
                LatentMetabolicHeat = source.LatentMetabolicHeat,
                ProjectTemperature = source.ProjectTemperature,
                NonProjectTemperature = source.NonProjectTemperature,
                ProjectHumidity = source.ProjectHumidity,
                FlowTemperature = source.FlowTemperature,
                FlowRelativeHumidity = source.FlowRelativeHumidity,
                VentilationDebit = source.VentilationDebit,
                LightsCoolingPower = source.LightsCoolingPower,
                BalancedDevicesCoolingPower = source.BalancedDevicesCoolingPower,
                LightsCoolingWorkSchedule = source.LightsCoolingWorkSchedule,
                BalancedDevicesCoolingWorkSchedule = source.BalancedDevicesCoolingWorkSchedule,
                WorkdaySchedule = Clone(source.WorkdaySchedule),
                SaturdaySchedule = Clone(source.SaturdaySchedule),
                SundaySchedule = Clone(source.SundaySchedule),
                OccupantsWorkdaySchedule = Clone(source.OccupantsWorkdaySchedule),
                OccupantsSaturdaySchedule = Clone(source.OccupantsSaturdaySchedule),
                OccupantsSundaySchedule = Clone(source.OccupantsSundaySchedule),
                VentilationWorkdaySchedule = Clone(source.VentilationWorkdaySchedule),
                VentilationSaturdaySchedule = Clone(source.VentilationSaturdaySchedule),
                VentilationSundaySchedule = Clone(source.VentilationSundaySchedule),
                NightVentilationWorkdaySchedule = Clone(source.NightVentilationWorkdaySchedule),
                NightVentilationSaturdaySchedule = Clone(source.NightVentilationSaturdaySchedule),
                NightVentilationSundaySchedule = Clone(source.NightVentilationSundaySchedule),
                HolidaysByMonth = new Dictionary<int, int>(source.HolidaysByMonth),
                AverageOutdoorTemperatureByMonth = new Dictionary<int, double>(source.AverageOutdoorTemperatureByMonth),
                SolarRadiationByMonth = CloneSolar(source.SolarRadiationByMonth),
                HourlyWeatherByMonth = CloneHourly(source.HourlyWeatherByMonth)
            };
        }

        private static EecalcDailySchedule Clone(EecalcDailySchedule source)
        {
            return new EecalcDailySchedule
            {
                StartHour = source.StartHour,
                EndHour = source.EndHour
            };
        }

        private static EecalcWallDirectionFixture Clone(EecalcWallDirectionFixture source)
        {
            return new EecalcWallDirectionFixture
            {
                OuterA = Copy(source.OuterA),
                OuterU = Copy(source.OuterU),
                OuterSumL = Copy(source.OuterSumL),
                OuterSumX = Copy(source.OuterSumX),
                AccumulateWindowA = source.AccumulateWindowA,
                AccumulateWindowU = source.AccumulateWindowU,
                AccumulateWindowG = source.AccumulateWindowG,
                AccumulateWindowE = source.AccumulateWindowE,
                AccumulateOuterA = source.AccumulateOuterA,
                AccumulateOuterU = source.AccumulateOuterU,
                AccumulateOuterAlfa = source.AccumulateOuterAlfa,
                AccumulateOuterE = source.AccumulateOuterE,
                InnerA = Copy(source.InnerA),
                InnerU = Copy(source.InnerU),
                InnerW = Copy(source.InnerW),
                InnerCoolingS = Copy(source.InnerCoolingS)
            };
        }

        private static EecalcRoofFixture Clone(EecalcRoofFixture source)
        {
            return new EecalcRoofFixture
            {
                NonTransparentA = Copy(source.NonTransparentA),
                NonTransparentU = Copy(source.NonTransparentU),
                NonTransparentSumL = Copy(source.NonTransparentSumL),
                NonTransparentSumX = Copy(source.NonTransparentSumX),
                TransparentA = Copy(source.TransparentA),
                TransparentU = Copy(source.TransparentU),
                TransparentG = Copy(source.TransparentG),
                TransparentE = Copy(source.TransparentE),
                AccumulateNonTransparentA = source.AccumulateNonTransparentA,
                AccumulateNonTransparentU = source.AccumulateNonTransparentU,
                AccumulateNonTransparentAlfa = source.AccumulateNonTransparentAlfa,
                AccumulateNonTransparentE = source.AccumulateNonTransparentE,
                CeilingA = Copy(source.CeilingA),
                CeilingU = Copy(source.CeilingU),
                CeilingW = Copy(source.CeilingW),
                CeilingCoolingS = Copy(source.CeilingCoolingS)
            };
        }

        private static EecalcFloorFixture Clone(EecalcFloorFixture source)
        {
            return new EecalcFloorFixture
            {
                AccumulateFloorA = source.AccumulateFloorA,
                AccumulateFloorU = source.AccumulateFloorU,
                OtherFloorA = Copy(source.OtherFloorA),
                OtherFloorU = Copy(source.OtherFloorU),
                OtherFloorW = Copy(source.OtherFloorW),
                OtherFloorCoolingS = Copy(source.OtherFloorCoolingS)
            };
        }

        private static Dictionary<int, EecalcSolarRadiationFixture> CloneSolar(
            IReadOnlyDictionary<int, EecalcSolarRadiationFixture> source)
        {
            return source.ToDictionary(
                pair => pair.Key,
                pair => new EecalcSolarRadiationFixture
                {
                    N = pair.Value.N,
                    E = pair.Value.E,
                    S = pair.Value.S,
                    W = pair.Value.W,
                    H = pair.Value.H
                });
        }

        private static Dictionary<int, IReadOnlyList<EecalcHourlyWeatherFixture>> CloneHourly(
            IReadOnlyDictionary<int, IReadOnlyList<EecalcHourlyWeatherFixture>> source)
        {
            return source.ToDictionary(
                pair => pair.Key,
                pair => (IReadOnlyList<EecalcHourlyWeatherFixture>)pair.Value
                    .Select(hour => new EecalcHourlyWeatherFixture
                    {
                        Temperature = hour.Temperature,
                        Humidity = hour.Humidity
                    })
                    .ToList());
        }

        private static double[] Copy(double[] source)
        {
            var copy = new double[source.Length];
            Array.Copy(source, copy, source.Length);
            return copy;
        }

        private static void CopyArray(double[] source, double[] target)
        {
            var length = Math.Min(source.Length, target.Length);
            Array.Copy(source, target, length);
        }

        private sealed class EnvelopeMeasure
        {
            public EnvelopeMeasure(
                string tag,
                string row,
                double oldValue,
                double newValue,
                Action<EecalcEnvelopeFixture, EecalcEnvelopeFixture> applyEsm)
            {
                Tag = tag;
                Row = row;
                OldValue = oldValue;
                NewValue = newValue;
                ApplyEsm = applyEsm;
            }

            public string Tag { get; }

            public string Row { get; }

            public double OldValue { get; }

            public double NewValue { get; }

            public Action<EecalcEnvelopeFixture, EecalcEnvelopeFixture> ApplyEsm { get; }
        }
    }
}
