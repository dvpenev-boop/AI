using System;
using System.Linq;
using EE.Doklad.Tests.Validation.FullOracle;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcCoolingEsmCalculator
    {
        private readonly EecalcEnvelopeSavingsOracle savingsOracle = new();
        private readonly EecalcMonthlyCoolingOracle coolingOracle = new();

        public EecalcEnvelopeSavingsResult CalculateSavings(
            EecalcEnvelopeFixture source,
            EecalcEnvelopeUValues baseLineUValues,
            EecalcEnvelopeUValues esmUValues,
            EECalcVentilationInput coolingVentilationInput)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(baseLineUValues);
            ArgumentNullException.ThrowIfNull(esmUValues);
            ArgumentNullException.ThrowIfNull(coolingVentilationInput);

            var baseLine = EecalcEnvelopeEsmCalculator.CreateEnvelopeState(source, baseLineUValues);
            var esm = EecalcEnvelopeEsmCalculator.CreateEnvelopeState(baseLine, esmUValues);

            return savingsOracle.Calculate(
                baseLine,
                esm,
                fixture => CalculateNetCoolingEnergy(fixture, coolingVentilationInput));
        }

        public EecalcEnvelopeSavingsResult CalculateNeededSavings(
            EecalcEnvelopeFixture source,
            EecalcEnvelopeUValues baseLineUValues,
            EecalcEnvelopeUValues esmUValues,
            EECalcVentilationInput coolingVentilationInput,
            EecalcValidationFixture? esmCalculation = null)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(baseLineUValues);
            ArgumentNullException.ThrowIfNull(esmUValues);
            ArgumentNullException.ThrowIfNull(coolingVentilationInput);

            var baseLine = EecalcEnvelopeEsmCalculator.CreateEnvelopeState(source, baseLineUValues);
            var esm = EecalcEnvelopeEsmCalculator.CreateEnvelopeState(baseLine, esmUValues);
            if (esmCalculation is not null)
            {
                esm = WithCalculation(esm, esmCalculation);
            }

            return savingsOracle.Calculate(
                baseLine,
                esm,
                fixture => CalculateNeededCoolingEnergy(fixture, coolingVentilationInput));
        }

        public double CalculateNetCoolingEnergy(
            EecalcEnvelopeFixture fixture,
            EECalcVentilationInput coolingVentilationInput)
        {
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(coolingVentilationInput);

            var ventilation = new EECalcVentilationOracle().Calculate(fixture.Calculation, coolingVentilationInput);
            var ventilationInputs = ventilation.Rows.Sum(row => row.CoolingInputs);
            return coolingOracle.Calculate(fixture, ventilationInputs).ResultNetEnergy;
        }

        public double CalculateNeededCoolingEnergy(
            EecalcEnvelopeFixture fixture,
            EECalcVentilationInput coolingVentilationInput)
        {
            var netEnergy = CalculateNetCoolingEnergy(fixture, coolingVentilationInput);
            return EECalcMath.DivideByEfficiency(
                netEnergy,
                EECalcMath.EfficiencyProduct(coolingVentilationInput.CoolingEfficiency1));
        }

        private static EecalcEnvelopeFixture WithCalculation(
            EecalcEnvelopeFixture source,
            EecalcValidationFixture calculation)
        {
            return new EecalcEnvelopeFixture
            {
                Id = source.Id,
                Calculation = calculation,
                NorthWalls = source.NorthWalls,
                NorthEastWalls = source.NorthEastWalls,
                EastWalls = source.EastWalls,
                SouthEastWalls = source.SouthEastWalls,
                SouthWalls = source.SouthWalls,
                SouthWestWalls = source.SouthWestWalls,
                WestWalls = source.WestWalls,
                NorthWestWalls = source.NorthWestWalls,
                Roof = source.Roof,
                Floor = source.Floor
            };
        }
    }
}
