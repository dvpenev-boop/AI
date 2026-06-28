using System;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Tests.Validation.FullOracle;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcEnvelopeEsmCalculator
    {
        private readonly EecalcEnvelopeSavingsOracle savingsOracle = new();
        private readonly EecalcMonthlyHeatingOracle heatingOracle = new();

        public EecalcEnvelopeSavingsResult CalculateSavings(
            EecalcEnvelopeFixture source,
            EecalcEnvelopeUValues baseLineUValues,
            EecalcEnvelopeUValues esmUValues,
            EECalcVentilationInput ventilationInput,
            EECalcLightingDevicesInput lightingInput)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(baseLineUValues);
            ArgumentNullException.ThrowIfNull(esmUValues);
            ArgumentNullException.ThrowIfNull(ventilationInput);
            ArgumentNullException.ThrowIfNull(lightingInput);

            var baseLine = CreateEnvelopeState(source, baseLineUValues);
            var esm = CreateEnvelopeState(baseLine, esmUValues);

            return savingsOracle.Calculate(
                baseLine,
                esm,
                fixture => CalculateNetHeatingEnergyAfterInputs(fixture, ventilationInput, lightingInput));
        }

        public double CalculateNetHeatingEnergyAfterInputs(
            EecalcEnvelopeFixture fixture,
            EECalcVentilationInput ventilationInput,
            EECalcLightingDevicesInput lightingInput)
        {
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(ventilationInput);
            ArgumentNullException.ThrowIfNull(lightingInput);

            var heatingRows = heatingOracle.Calculate(fixture);
            var ventilation = new EECalcVentilationOracle().Calculate(fixture.Calculation, ventilationInput);
            var lighting = new EECalcLightingDevicesOracle().Calculate(fixture.Calculation, lightingInput, heatingRows);

            return heatingRows.Sum(row => row.FinalQnd)
                - ventilation.Rows.Sum(row => row.HeatingInputs)
                - lighting.ResulLightInputs
                - lighting.ResulAppliancesInputs;
        }

        public static EecalcEnvelopeFixture CreateEnvelopeState(
            EecalcEnvelopeFixture source,
            EecalcEnvelopeUValues uValues)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(uValues);

            var state = EecalcEnvelopeSavingsOracle.Clone(source);
            ApplyOuterWallU(state, uValues.OuterWallsU);
            ApplyWindowU(state, uValues.WindowsU);
            ApplyRoofU(state, uValues.NonTransparentRoofU);
            ApplyFloorU(state, uValues.FloorU);
            return state;
        }

        private static void ApplyOuterWallU(EecalcEnvelopeFixture fixture, double uValue)
        {
            foreach (var wall in WallDirections(fixture))
            {
                if (wall.AccumulateOuterA <= 0.0)
                {
                    continue;
                }

                wall.AccumulateOuterU = uValue;
                for (var index = 0; index < wall.OuterU.Length; index++)
                {
                    if (wall.OuterA[index] > 0.0)
                    {
                        wall.OuterU[index] = uValue;
                    }
                }
            }
        }

        private static void ApplyWindowU(EecalcEnvelopeFixture fixture, double uValue)
        {
            foreach (var wall in WallDirections(fixture))
            {
                if (wall.AccumulateWindowA > 0.0)
                {
                    wall.AccumulateWindowU = uValue;
                }
            }
        }

        private static void ApplyRoofU(EecalcEnvelopeFixture fixture, double uValue)
        {
            fixture.Roof.AccumulateNonTransparentU = uValue;
            for (var index = 0; index < fixture.Roof.NonTransparentU.Length; index++)
            {
                if (fixture.Roof.NonTransparentA[index] > 0.0)
                {
                    fixture.Roof.NonTransparentU[index] = uValue;
                }
            }
        }

        private static void ApplyFloorU(EecalcEnvelopeFixture fixture, double uValue)
        {
            fixture.Floor.AccumulateFloorU = uValue;
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
    }
}
