using System;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Tests.Validation.FullOracle;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcLightingDevicesEsmCalculator
    {
        private readonly EecalcComponentSavingsOracle<EECalcLightingDevicesInput> savingsOracle = new();

        public EecalcComponentSavingsResult CalculateLightingSavings(
            EecalcValidationFixture fixture,
            EECalcLightingDevicesInput baseLine,
            EECalcLightingDevicesInput esm)
        {
            return CalculateGroupSavings(
                "Lighting",
                fixture,
                baseLine,
                esm,
                input => input.Lights,
                (input, equipment) => Clone(input, lights: equipment),
                result => result.LightsGeneralNeededEnergy);
        }

        public EecalcComponentSavingsResult CalculateBalancedDevicesSavings(
            EecalcValidationFixture fixture,
            EECalcLightingDevicesInput baseLine,
            EECalcLightingDevicesInput esm)
        {
            return CalculateGroupSavings(
                "Devices affecting heat balance",
                fixture,
                baseLine,
                esm,
                input => input.BalancedDevices,
                (input, equipment) => Clone(input, balancedDevices: equipment),
                result => result.BalancedDevicesGeneralNeededEnergy);
        }

        public EecalcComponentSavingsResult CalculateNonBalancedDevicesSavings(
            EecalcValidationFixture fixture,
            EECalcLightingDevicesInput baseLine,
            EECalcLightingDevicesInput esm)
        {
            return CalculateGroupSavings(
                "Devices not affecting heat balance",
                fixture,
                baseLine,
                esm,
                input => input.NonBalancedDevices,
                (input, equipment) => Clone(input, nonBalancedDevices: equipment),
                result => result.NonBalancedDevicesGeneralNeededEnergy);
        }

        public EecalcComponentSavingsResult CalculateHotWaterPumpsSavings(
            EecalcValidationFixture fixture,
            EECalcLightingDevicesInput baseLine,
            EECalcLightingDevicesInput esm)
        {
            return CalculateGroupSavings(
                "Hot water pumps",
                fixture,
                baseLine,
                esm,
                input => input.HotWaterPumps,
                (input, equipment) => Clone(input, hotWaterPumps: equipment),
                result => result.HotWaterPumpsGeneralNeededEnergy);
        }

        private EecalcComponentSavingsResult CalculateGroupSavings(
            string technology,
            EecalcValidationFixture fixture,
            EECalcLightingDevicesInput baseLine,
            EECalcLightingDevicesInput esm,
            Func<EECalcLightingDevicesInput, EECalcEquipmentInput> select,
            Func<EECalcLightingDevicesInput, EECalcEquipmentInput, EECalcLightingDevicesInput> apply,
            Func<EECalcLightingDevicesOracleResult, double> energySelector)
        {
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(baseLine);
            ArgumentNullException.ThrowIfNull(esm);

            var baseLineEquipment = select(baseLine);
            var esmEquipment = select(esm);
            return savingsOracle.Calculate(
                technology,
                baseLine,
                esm,
                input => energySelector(new EECalcLightingDevicesOracle().Calculate(fixture, input, heatingRows: null)),
                BuildMeasures(baseLineEquipment, esmEquipment, select, apply));
        }

        private static IReadOnlyList<EecalcComponentMeasure<EECalcLightingDevicesInput>> BuildMeasures(
            EECalcEquipmentInput baseLine,
            EECalcEquipmentInput esm,
            Func<EECalcLightingDevicesInput, EECalcEquipmentInput> select,
            Func<EECalcLightingDevicesInput, EECalcEquipmentInput, EECalcLightingDevicesInput> apply)
        {
            return new List<EecalcComponentMeasure<EECalcLightingDevicesInput>>
            {
                new(
                    "HeatingPower",
                    "Heating power",
                    baseLine.HeatingPower,
                    esm.HeatingPower,
                    (current, target) => apply(current, Clone(select(current), heatingPower: select(target).HeatingPower))),
                new(
                    "HeatingWorkSchedule",
                    "Heating work schedule",
                    baseLine.HeatingWorkSchedule,
                    esm.HeatingWorkSchedule,
                    (current, target) => apply(current, Clone(select(current), heatingWorkSchedule: select(target).HeatingWorkSchedule))),
                new(
                    "CoolingPower",
                    "Cooling power",
                    baseLine.CoolingPower,
                    esm.CoolingPower,
                    (current, target) => apply(current, Clone(select(current), coolingPower: select(target).CoolingPower))),
                new(
                    "CoolingWorkSchedule",
                    "Cooling work schedule",
                    baseLine.CoolingWorkSchedule,
                    esm.CoolingWorkSchedule,
                    (current, target) => apply(current, Clone(select(current), coolingWorkSchedule: select(target).CoolingWorkSchedule))),
                new(
                    "GeneralPower",
                    "General power",
                    baseLine.GeneralPower,
                    esm.GeneralPower,
                    (current, target) => apply(current, Clone(select(current), generalPower: select(target).GeneralPower))),
                new(
                    "GeneralWorkSchedule",
                    "General work schedule",
                    baseLine.GeneralWorkSchedule,
                    esm.GeneralWorkSchedule,
                    (current, target) => apply(current, Clone(select(current), generalWorkSchedule: select(target).GeneralWorkSchedule)))
            };
        }

        public static EECalcLightingDevicesInput Clone(
            EECalcLightingDevicesInput source,
            EECalcEquipmentInput? lights = null,
            EECalcEquipmentInput? balancedDevices = null,
            EECalcEquipmentInput? nonBalancedDevices = null,
            EECalcEquipmentInput? hotWaterPumps = null)
        {
            return new EECalcLightingDevicesInput
            {
                Lights = lights ?? Clone(source.Lights),
                BalancedDevices = balancedDevices ?? Clone(source.BalancedDevices),
                NonBalancedDevices = nonBalancedDevices ?? Clone(source.NonBalancedDevices),
                HotWaterPumps = hotWaterPumps ?? Clone(source.HotWaterPumps)
            };
        }

        public static EECalcEquipmentInput Clone(
            EECalcEquipmentInput source,
            double? heatingPower = null,
            double? heatingWorkSchedule = null,
            double? coolingPower = null,
            double? coolingWorkSchedule = null,
            double? generalPower = null,
            double? generalWorkSchedule = null)
        {
            return new EECalcEquipmentInput
            {
                HeatingPower = heatingPower ?? source.HeatingPower,
                HeatingWorkSchedule = heatingWorkSchedule ?? source.HeatingWorkSchedule,
                CoolingPower = coolingPower ?? source.CoolingPower,
                CoolingWorkSchedule = coolingWorkSchedule ?? source.CoolingWorkSchedule,
                GeneralPower = generalPower ?? source.GeneralPower,
                GeneralWorkSchedule = generalWorkSchedule ?? source.GeneralWorkSchedule,
                ByMonths = source.ByMonths,
                MonthlySchedules = source.MonthlySchedules.ToDictionary(pair => pair.Key, pair => pair.Value)
            };
        }
    }
}
