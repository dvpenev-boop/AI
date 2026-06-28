using System;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Tests.Validation.FullOracle;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcVentilationEsmCalculator
    {
        private readonly EECalcVentilationOracle ventilationOracle = new();

        public EecalcVentilationSavingsResult CalculateHeatingSavings(
            EecalcValidationFixture fixture,
            EECalcVentilationInput baseLineInput,
            EECalcVentilationInput esmInput)
        {
            return CalculateSavings(
                EecalcVentilationEsmMode.Heating,
                fixture,
                baseLineInput,
                esmInput,
                result => result.HeatingNeededEnergy);
        }

        public EecalcVentilationSavingsResult CalculateCoolingSavings(
            EecalcValidationFixture fixture,
            EECalcVentilationInput baseLineInput,
            EECalcVentilationInput esmInput)
        {
            return CalculateSavings(
                EecalcVentilationEsmMode.Cooling,
                fixture,
                baseLineInput,
                esmInput,
                result => result.CoolingNeededEnergy);
        }

        private EecalcVentilationSavingsResult CalculateSavings(
            EecalcVentilationEsmMode mode,
            EecalcValidationFixture fixture,
            EECalcVentilationInput baseLineInput,
            EECalcVentilationInput esmInput,
            Func<EECalcVentilationOracleResult, double> energySelector)
        {
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(baseLineInput);
            ArgumentNullException.ThrowIfNull(esmInput);
            ArgumentNullException.ThrowIfNull(energySelector);

            var baseLineEnergy = Energy(fixture, baseLineInput, energySelector);
            var esmEnergy = Energy(fixture, esmInput, energySelector);
            var totalSaving = baseLineEnergy - esmEnergy;
            var measures = BuildChangedMeasures(mode, baseLineInput, esmInput);

            if (measures.Count == 0)
            {
                return new EecalcVentilationSavingsResult
                {
                    Mode = mode,
                    BaseLineEnergy = baseLineEnergy,
                    EsmEnergy = esmEnergy,
                    TotalSaving = totalSaving,
                    Items = new List<EecalcVentilationSavingItem>()
                };
            }

            var virtualItems = measures
                .Select(measure =>
                {
                    var virtualInput = measure.ApplyEsm(baseLineInput, esmInput);
                    var virtualEnergy = Energy(fixture, virtualInput, energySelector);
                    return new EecalcVentilationSavingItem
                    {
                        Technology = Technology(mode),
                        Tag = measure.Tag,
                        Row = measure.Row,
                        OldValue = measure.OldValue,
                        NewValue = measure.NewValue,
                        VirtualEnergy = virtualEnergy,
                        VirtualSaving = baseLineEnergy - virtualEnergy
                    };
                })
                .ToList();

            var virtualSavingTotal = virtualItems.Sum(item => item.VirtualSaving);
            var items = virtualItems
                .Select(item =>
                {
                    var part = virtualSavingTotal == 0.0 ? 0.0 : item.VirtualSaving / virtualSavingTotal;
                    return new EecalcVentilationSavingItem
                    {
                        Technology = item.Technology,
                        Tag = item.Tag,
                        Row = item.Row,
                        OldValue = item.OldValue,
                        NewValue = item.NewValue,
                        VirtualEnergy = item.VirtualEnergy,
                        VirtualSaving = item.VirtualSaving,
                        Part = part,
                        ActualSaving = totalSaving * part
                    };
                })
                .ToList();
            items = ApplyNegativeSavingsCorrection(totalSaving, items);

            return new EecalcVentilationSavingsResult
            {
                Mode = mode,
                BaseLineEnergy = baseLineEnergy,
                EsmEnergy = esmEnergy,
                TotalSaving = totalSaving,
                Items = items
            };
        }

        private static List<EecalcVentilationSavingItem> ApplyNegativeSavingsCorrection(
            double totalSaving,
            List<EecalcVentilationSavingItem> items)
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
                    return new EecalcVentilationSavingItem
                    {
                        Technology = item.Technology,
                        Tag = item.Tag,
                        Row = item.Row,
                        OldValue = item.OldValue,
                        NewValue = item.NewValue,
                        VirtualEnergy = item.VirtualEnergy,
                        VirtualSaving = item.VirtualSaving,
                        Part = part,
                        ActualSaving = correctedPositiveTotal * part
                    };
                })
                .ToList();
        }

        private double Energy(
            EecalcValidationFixture fixture,
            EECalcVentilationInput input,
            Func<EECalcVentilationOracleResult, double> energySelector)
        {
            return energySelector(ventilationOracle.Calculate(fixture, input));
        }

        private static List<VentilationMeasure> BuildChangedMeasures(
            EecalcVentilationEsmMode mode,
            EECalcVentilationInput baseLine,
            EECalcVentilationInput esm)
        {
            var measures = new List<VentilationMeasure>();
            AddScalar(measures, "Debit", "Debit", baseLine.Debit, esm.Debit, (current, target) => Clone(current, debit: target.Debit));
            AddScalar(measures, "FlowTemperature", "Flow temperature", baseLine.FlowTemperature, esm.FlowTemperature, (current, target) => Clone(current, flowTemperature: target.FlowTemperature));
            AddScalar(measures, "RelativeHumidity", "Flow relative humidity", baseLine.FlowRelativeHumidity, esm.FlowRelativeHumidity, (current, target) => Clone(current, flowRelativeHumidity: target.FlowRelativeHumidity));
            AddScalar(measures, "ProjectHumidity", "Project humidity", baseLine.ProjectHumidity, esm.ProjectHumidity, (current, target) => Clone(current, projectHumidity: target.ProjectHumidity));
            AddScalar(measures, "FirstRecEfficiency", "First recovery efficiency", baseLine.FirstRecEfficiency, esm.FirstRecEfficiency, (current, target) => Clone(current, firstRecEfficiency: target.FirstRecEfficiency));
            AddScalar(measures, "SecondRecEfficiency", "Second recovery efficiency", baseLine.SecondRecEfficiency, esm.SecondRecEfficiency, (current, target) => Clone(current, secondRecEfficiency: target.SecondRecEfficiency));
            AddScalar(measures, "HeatingAirDifference", "Heating air difference", baseLine.HeatingAirDifference, esm.HeatingAirDifference, (current, target) => Clone(current, heatingAirDifference: target.HeatingAirDifference));
            AddScalar(measures, "MinimumEndTemperature", "Minimum end temperature", baseLine.MinimumEndTemperature, esm.MinimumEndTemperature, (current, target) => Clone(current, minimumEndTemperature: target.MinimumEndTemperature));
            AddScalar(measures, "Part1", "Energy source 1 share", baseLine.Part1, esm.Part1, (current, target) => Clone(current, part1: target.Part1));
            AddScalar(measures, "Part2", "Energy source 2 share", baseLine.Part2, esm.Part2, (current, target) => Clone(current, part2: target.Part2));

            if (!SameScheduleSet(baseLine, esm))
            {
                measures.Add(new VentilationMeasure(
                    "WorkingSchedule",
                    "Working schedule",
                    WeeklyHours(baseLine),
                    WeeklyHours(esm),
                    (current, target) => Clone(
                        current,
                        workdaySchedule: target.WorkdaySchedule,
                        saturdaySchedule: target.SaturdaySchedule,
                        sundaySchedule: target.SundaySchedule)));
            }

            AddEfficiencyMeasures(measures, mode, baseLine, esm);
            return measures;
        }

        private static void AddEfficiencyMeasures(
            ICollection<VentilationMeasure> measures,
            EecalcVentilationEsmMode mode,
            EECalcVentilationInput baseLine,
            EECalcVentilationInput esm)
        {
            if (mode == EecalcVentilationEsmMode.Heating)
            {
                AddChain(measures, string.Empty, baseLine.HeatingEfficiency1, esm.HeatingEfficiency1, (current, chain) => Clone(current, heatingEfficiency1: chain));
                AddChain(measures, "2", baseLine.HeatingEfficiency2, esm.HeatingEfficiency2, (current, chain) => Clone(current, heatingEfficiency2: chain));
                return;
            }

            AddChain(measures, string.Empty, baseLine.CoolingEfficiency1, esm.CoolingEfficiency1, (current, chain) => Clone(current, coolingEfficiency1: chain), coldGenerator: true);
            AddChain(measures, "2", baseLine.CoolingEfficiency2, esm.CoolingEfficiency2, (current, chain) => Clone(current, coolingEfficiency2: chain), coldGenerator: true);
        }

        private static void AddChain(
            ICollection<VentilationMeasure> measures,
            string suffix,
            EECalcEfficiencyChain baseLine,
            EECalcEfficiencyChain esm,
            Func<EECalcVentilationInput, EECalcEfficiencyChain, EECalcVentilationInput> apply,
            bool coldGenerator = false)
        {
            AddScalar(
                measures,
                "TransmitTempEfficiency" + suffix,
                "Transmit temperature efficiency " + DisplaySuffix(suffix),
                baseLine.TransmitTempEfficiency,
                esm.TransmitTempEfficiency,
                (current, target) => apply(current, Clone(
                    SelectChain(current, suffix, coldGenerator),
                    transmitTempEfficiency: SelectChain(target, suffix, coldGenerator).TransmitTempEfficiency)));
            AddScalar(
                measures,
                "SupplyNetEfficiency" + suffix,
                "Supply net efficiency " + DisplaySuffix(suffix),
                baseLine.SupplyNetEfficiency,
                esm.SupplyNetEfficiency,
                (current, target) => apply(current, Clone(
                    SelectChain(current, suffix, coldGenerator),
                    supplyNetEfficiency: SelectChain(target, suffix, coldGenerator).SupplyNetEfficiency)));
            AddScalar(
                measures,
                "Automatic" + suffix,
                "Automatic control " + DisplaySuffix(suffix),
                baseLine.Automatic,
                esm.Automatic,
                (current, target) => apply(current, Clone(
                    SelectChain(current, suffix, coldGenerator),
                    automatic: SelectChain(target, suffix, coldGenerator).Automatic)));
            AddScalar(
                measures,
                "EnergyManagement" + suffix,
                "Energy management " + DisplaySuffix(suffix),
                baseLine.EnergyManagement,
                esm.EnergyManagement,
                (current, target) => apply(current, Clone(
                    SelectChain(current, suffix, coldGenerator),
                    energyManagement: SelectChain(target, suffix, coldGenerator).EnergyManagement)));
            AddScalar(
                measures,
                (coldGenerator ? "GeneratorColdEfficiency" : "GeneratorHeatEfficiency") + (suffix == string.Empty ? "1" : suffix),
                "Generator efficiency " + DisplaySuffix(suffix),
                baseLine.GeneratorEfficiency,
                esm.GeneratorEfficiency,
                (current, target) => apply(current, Clone(
                    SelectChain(current, suffix, coldGenerator),
                    generatorEfficiency: SelectChain(target, suffix, coldGenerator).GeneratorEfficiency)));
        }

        private static EECalcEfficiencyChain SelectChain(
            EECalcVentilationInput input,
            string suffix,
            bool coldGenerator)
        {
            if (coldGenerator)
            {
                return suffix == "2" ? input.CoolingEfficiency2 : input.CoolingEfficiency1;
            }

            return suffix == "2" ? input.HeatingEfficiency2 : input.HeatingEfficiency1;
        }

        private static void AddScalar(
            ICollection<VentilationMeasure> measures,
            string tag,
            string row,
            double oldValue,
            double newValue,
            Func<EECalcVentilationInput, EECalcVentilationInput, EECalcVentilationInput> applyEsm)
        {
            if (Math.Abs(oldValue - newValue) < 0.0000001)
            {
                return;
            }

            measures.Add(new VentilationMeasure(tag, row.Trim(), oldValue, newValue, applyEsm));
        }

        private static string DisplaySuffix(string suffix)
        {
            return suffix == string.Empty ? "1" : suffix;
        }

        private static bool SameScheduleSet(EECalcVentilationInput left, EECalcVentilationInput right)
        {
            return SameSchedule(left.WorkdaySchedule, right.WorkdaySchedule)
                && SameSchedule(left.SaturdaySchedule, right.SaturdaySchedule)
                && SameSchedule(left.SundaySchedule, right.SundaySchedule);
        }

        private static bool SameSchedule(EecalcDailySchedule left, EecalcDailySchedule right)
        {
            return left.StartHour == right.StartHour && left.EndHour == right.EndHour;
        }

        private static double WeeklyHours(EECalcVentilationInput input)
        {
            return 5.0 * Duration(input.WorkdaySchedule)
                + Duration(input.SaturdaySchedule)
                + Duration(input.SundaySchedule);
        }

        private static double Duration(EecalcDailySchedule schedule)
        {
            return schedule.EndHour > schedule.StartHour ? schedule.EndHour - schedule.StartHour : 0.0;
        }

        public static EECalcVentilationInput Clone(
            EECalcVentilationInput source,
            double? debit = null,
            double? flowTemperature = null,
            double? flowRelativeHumidity = null,
            double? projectHumidity = null,
            double? firstRecEfficiency = null,
            double? secondRecEfficiency = null,
            double? heatingAirDifference = null,
            double? minimumEndTemperature = null,
            double? part1 = null,
            double? part2 = null,
            EECalcEfficiencyChain? heatingEfficiency1 = null,
            EECalcEfficiencyChain? heatingEfficiency2 = null,
            EECalcEfficiencyChain? coolingEfficiency1 = null,
            EECalcEfficiencyChain? coolingEfficiency2 = null,
            EecalcDailySchedule? workdaySchedule = null,
            EecalcDailySchedule? saturdaySchedule = null,
            EecalcDailySchedule? sundaySchedule = null)
        {
            return new EECalcVentilationInput
            {
                Debit = debit ?? source.Debit,
                FlowTemperature = flowTemperature ?? source.FlowTemperature,
                FlowRelativeHumidity = flowRelativeHumidity ?? source.FlowRelativeHumidity,
                ProjectHumidity = projectHumidity ?? source.ProjectHumidity,
                FirstRecEfficiency = firstRecEfficiency ?? source.FirstRecEfficiency,
                SecondRecEfficiency = secondRecEfficiency ?? source.SecondRecEfficiency,
                HeatingAirDifference = heatingAirDifference ?? source.HeatingAirDifference,
                MinimumEndTemperature = minimumEndTemperature ?? source.MinimumEndTemperature,
                Part1 = part1 ?? source.Part1,
                Part2 = part2 ?? source.Part2,
                Fuel1 = source.Fuel1,
                Fuel2 = source.Fuel2,
                HeatingEfficiency1 = heatingEfficiency1 ?? Clone(source.HeatingEfficiency1),
                HeatingEfficiency2 = heatingEfficiency2 ?? Clone(source.HeatingEfficiency2),
                CoolingEfficiency1 = coolingEfficiency1 ?? Clone(source.CoolingEfficiency1),
                CoolingEfficiency2 = coolingEfficiency2 ?? Clone(source.CoolingEfficiency2),
                WorkdaySchedule = Clone(workdaySchedule ?? source.WorkdaySchedule),
                SaturdaySchedule = Clone(saturdaySchedule ?? source.SaturdaySchedule),
                SundaySchedule = Clone(sundaySchedule ?? source.SundaySchedule)
            };
        }

        private static EECalcEfficiencyChain Clone(
            EECalcEfficiencyChain source,
            double? transmitTempEfficiency = null,
            double? supplyNetEfficiency = null,
            double? automatic = null,
            double? energyManagement = null,
            double? generatorEfficiency = null)
        {
            return new EECalcEfficiencyChain
            {
                TransmitTempEfficiency = transmitTempEfficiency ?? source.TransmitTempEfficiency,
                SupplyNetEfficiency = supplyNetEfficiency ?? source.SupplyNetEfficiency,
                Automatic = automatic ?? source.Automatic,
                EnergyManagement = energyManagement ?? source.EnergyManagement,
                GeneratorEfficiency = generatorEfficiency ?? source.GeneratorEfficiency
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

        private static string Technology(EecalcVentilationEsmMode mode)
        {
            return mode == EecalcVentilationEsmMode.Heating
                ? "Вентилация - Отопление"
                : "Вентилация - Охлаждане";
        }

        private sealed class VentilationMeasure
        {
            public VentilationMeasure(
                string tag,
                string row,
                double oldValue,
                double newValue,
                Func<EECalcVentilationInput, EECalcVentilationInput, EECalcVentilationInput> applyEsm)
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

            public Func<EECalcVentilationInput, EECalcVentilationInput, EECalcVentilationInput> ApplyEsm { get; }
        }
    }
}
