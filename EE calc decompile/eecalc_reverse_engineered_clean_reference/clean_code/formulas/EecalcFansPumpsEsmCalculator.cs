using System;
using System.Collections.Generic;
using EE.Doklad.Tests.Validation.FullOracle;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcFansPumpsEsmCalculator
    {
        private readonly EecalcComponentSavingsOracle<EECalcHeatingFansAndPumpsInput> heatingSavingsOracle = new();
        private readonly EecalcComponentSavingsOracle<EECalcCoolingFansAndPumpsInput> coolingSavingsOracle = new();

        public EecalcComponentSavingsResult CalculateHeatingSavings(
            EecalcValidationFixture fixture,
            EECalcVentilationInput ventilationInput,
            EECalcHeatingFansAndPumpsInput baseLine,
            EECalcHeatingFansAndPumpsInput esm)
        {
            return heatingSavingsOracle.Calculate(
                "Fans and pumps - heating",
                baseLine,
                esm,
                input =>
                {
                    var result = new EECalcHeatingFansAndPumpsOracle().Calculate(fixture, input, ventilationInput);
                    return result.NeededEnergy + result.OtherNeededEnergy;
                },
                BuildHeatingMeasures(baseLine, esm));
        }

        public EecalcComponentSavingsResult CalculateCoolingSavings(
            EecalcValidationFixture fixture,
            EECalcVentilationInput ventilationInput,
            EECalcCoolingFansAndPumpsInput baseLine,
            EECalcCoolingFansAndPumpsInput esm)
        {
            return coolingSavingsOracle.Calculate(
                "Fans and pumps - cooling",
                baseLine,
                esm,
                input =>
                {
                    var result = new EECalcCoolingFansAndPumpsOracle().Calculate(fixture, input, ventilationInput);
                    return result.NeededEnergy + result.OtherNeededEnergy;
                },
                BuildCoolingMeasures(baseLine, esm));
        }

        private static IReadOnlyList<EecalcComponentMeasure<EECalcHeatingFansAndPumpsInput>> BuildHeatingMeasures(
            EECalcHeatingFansAndPumpsInput baseLine,
            EECalcHeatingFansAndPumpsInput esm)
        {
            return new List<EecalcComponentMeasure<EECalcHeatingFansAndPumpsInput>>
            {
                new("VentilatorsHeat", "Ventilators", baseLine.VentilatorsHeat, esm.VentilatorsHeat, (current, target) => Clone(current, ventilatorsHeat: target.VentilatorsHeat)),
                new("PumpVentilationHeat", "Ventilation pumps", baseLine.PumpVentilationHeat, esm.PumpVentilationHeat, (current, target) => Clone(current, pumpVentilationHeat: target.PumpVentilationHeat)),
                new("HeatingPump", "Heating pumps", baseLine.HeatingPump, esm.HeatingPump, (current, target) => Clone(current, heatingPump: target.HeatingPump)),
                new("EnergyManagement", "Energy management", baseLine.EnergyManagement, esm.EnergyManagement, (current, target) => Clone(current, energyManagement: target.EnergyManagement)),
                new("OtherHeatingVentilation", "Other ventilation", baseLine.OtherHeatingVentilation, esm.OtherHeatingVentilation, (current, target) => Clone(current, otherHeatingVentilation: target.OtherHeatingVentilation)),
                new("OtherHeating", "Other heating", baseLine.OtherHeating, esm.OtherHeating, (current, target) => Clone(current, otherHeating: target.OtherHeating))
            };
        }

        private static IReadOnlyList<EecalcComponentMeasure<EECalcCoolingFansAndPumpsInput>> BuildCoolingMeasures(
            EECalcCoolingFansAndPumpsInput baseLine,
            EECalcCoolingFansAndPumpsInput esm)
        {
            return new List<EecalcComponentMeasure<EECalcCoolingFansAndPumpsInput>>
            {
                new("VentilatorsCool", "Ventilators", baseLine.VentilatorsCool, esm.VentilatorsCool, (current, target) => Clone(current, ventilatorsCool: target.VentilatorsCool)),
                new("VentilatorsOutdoorAirCool", "Outdoor air ventilators", baseLine.VentilatorsOutdoorAirCool, esm.VentilatorsOutdoorAirCool, (current, target) => Clone(current, ventilatorsOutdoorAirCool: target.VentilatorsOutdoorAirCool)),
                new("PumpVentilationCool", "Ventilation pumps", baseLine.PumpVentilationCool, esm.PumpVentilationCool, (current, target) => Clone(current, pumpVentilationCool: target.PumpVentilationCool)),
                new("CoolingPump", "Cooling pumps", baseLine.CoolingPump, esm.CoolingPump, (current, target) => Clone(current, coolingPump: target.CoolingPump)),
                new("EnergyManagement", "Energy management", baseLine.EnergyManagement, esm.EnergyManagement, (current, target) => Clone(current, energyManagement: target.EnergyManagement)),
                new("OtherCoolingVentilation", "Other ventilation", baseLine.OtherCoolingVentilation, esm.OtherCoolingVentilation, (current, target) => Clone(current, otherCoolingVentilation: target.OtherCoolingVentilation)),
                new("OtherCooling", "Other cooling", baseLine.OtherCooling, esm.OtherCooling, (current, target) => Clone(current, otherCooling: target.OtherCooling))
            };
        }

        public static EECalcHeatingFansAndPumpsInput Clone(
            EECalcHeatingFansAndPumpsInput source,
            double? ventilatorsHeat = null,
            double? pumpVentilationHeat = null,
            double? heatingPump = null,
            double? energyManagement = null,
            double? otherHeatingVentilation = null,
            double? otherHeating = null)
        {
            return new EECalcHeatingFansAndPumpsInput
            {
                VentilatorsHeat = ventilatorsHeat ?? source.VentilatorsHeat,
                PumpVentilationHeat = pumpVentilationHeat ?? source.PumpVentilationHeat,
                HeatingPump = heatingPump ?? source.HeatingPump,
                EnergyManagement = energyManagement ?? source.EnergyManagement,
                OtherHeatingVentilation = otherHeatingVentilation ?? source.OtherHeatingVentilation,
                OtherHeating = otherHeating ?? source.OtherHeating
            };
        }

        public static EECalcCoolingFansAndPumpsInput Clone(
            EECalcCoolingFansAndPumpsInput source,
            double? ventilatorsCool = null,
            double? ventilatorsOutdoorAirCool = null,
            double? pumpVentilationCool = null,
            double? coolingPump = null,
            double? energyManagement = null,
            double? otherCoolingVentilation = null,
            double? otherCooling = null)
        {
            return new EECalcCoolingFansAndPumpsInput
            {
                VentilatorsCool = ventilatorsCool ?? source.VentilatorsCool,
                VentilatorsOutdoorAirCool = ventilatorsOutdoorAirCool ?? source.VentilatorsOutdoorAirCool,
                PumpVentilationCool = pumpVentilationCool ?? source.PumpVentilationCool,
                CoolingPump = coolingPump ?? source.CoolingPump,
                EnergyManagement = energyManagement ?? source.EnergyManagement,
                OtherCoolingVentilation = otherCoolingVentilation ?? source.OtherCoolingVentilation,
                OtherCooling = otherCooling ?? source.OtherCooling
            };
        }
    }
}
