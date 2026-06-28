using System;
using System.Collections.Generic;
using System.Linq;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcComponentSavingsOracle<TInput>
    {
        public EecalcComponentSavingsResult Calculate(
            string technology,
            TInput baseLine,
            TInput esm,
            Func<TInput, double> energyCalculator,
            IReadOnlyList<EecalcComponentMeasure<TInput>> measures)
        {
            ArgumentNullException.ThrowIfNull(baseLine);
            ArgumentNullException.ThrowIfNull(esm);
            ArgumentNullException.ThrowIfNull(energyCalculator);
            ArgumentNullException.ThrowIfNull(measures);

            var baseLineEnergy = energyCalculator(baseLine);
            var esmEnergy = energyCalculator(esm);
            var totalSaving = baseLineEnergy - esmEnergy;
            var changed = measures
                .Where(measure => Math.Abs(measure.OldValue - measure.NewValue) >= 0.0000001)
                .ToList();

            if (changed.Count == 0)
            {
                return new EecalcComponentSavingsResult
                {
                    Technology = technology,
                    BaseLineEnergy = baseLineEnergy,
                    EsmEnergy = esmEnergy,
                    TotalSaving = totalSaving,
                    Items = new List<EecalcComponentSavingItem>()
                };
            }

            var virtualItems = changed
                .Select(measure =>
                {
                    var virtualInput = measure.ApplyEsm(baseLine, esm);
                    var virtualEnergy = energyCalculator(virtualInput);
                    return new EecalcComponentSavingItem
                    {
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
                    return new EecalcComponentSavingItem
                    {
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

            return new EecalcComponentSavingsResult
            {
                Technology = technology,
                BaseLineEnergy = baseLineEnergy,
                EsmEnergy = esmEnergy,
                TotalSaving = totalSaving,
                Items = items
            };
        }

        private static List<EecalcComponentSavingItem> ApplyNegativeSavingsCorrection(
            double totalSaving,
            List<EecalcComponentSavingItem> items)
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
                    return new EecalcComponentSavingItem
                    {
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
    }

    public sealed class EecalcComponentMeasure<TInput>
    {
        public EecalcComponentMeasure(
            string tag,
            string row,
            double oldValue,
            double newValue,
            Func<TInput, TInput, TInput> applyEsm)
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

        public Func<TInput, TInput, TInput> ApplyEsm { get; }
    }
}
