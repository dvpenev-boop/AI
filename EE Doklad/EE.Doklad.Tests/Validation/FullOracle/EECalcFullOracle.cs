using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using EE.Doklad.Services.EecalcClimate;

namespace EE.Doklad.Tests.Validation.FullOracle
{
    public sealed class EECalcFullOracle
    {
        private static readonly IReadOnlyList<EECalcOracleModule> DefaultModules =
            Enum.GetValues<EECalcOracleModule>();

        private readonly EecalcMonthlyDaysOracle monthlyDaysOracle = new();
        private readonly EecalcMonthlyHeatingOracle heatingOracle = new();
        private readonly EecalcMonthlyCoolingOracle coolingOracle = new();
        private readonly EECalcVentilationOracle ventilationOracle = new();
        private readonly EECalcDhwBgvOracle dhwBgvOracle = new();
        private readonly EECalcLightingDevicesOracle lightingDevicesOracle = new();

        public EECalcOracleResult Run(EECalcOracleContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            if (context.Mode != EECalcOracleMode.LegacyEECalcStrict)
            {
                throw new InvalidOperationException("EECalcFullOracle is strict-parity only. Corrected/current modes are intentionally unavailable.");
            }

            var modules = context.Modules.Count == 0 ? DefaultModules : context.Modules;
            var result = new EECalcOracleResult(context.Fixture.Id, context.Mode, context.Variant);
            var state = new EECalcFullOracleState();
            IReadOnlyList<EecalcMonthlyDaysOracleRow>? monthlyDays = null;
            IReadOnlyList<EecalcMonthlyHeatingOracleRow>? heatingRows = null;
            EecalcMonthlyCoolingOracleResult? coolingResult = null;
            EECalcVentilationOracleResult? ventilationResult = null;
            EECalcDhwBgvOracleResult? dhwBgvResult = null;
            EECalcLightingDevicesOracleResult? lightingDevicesResult = null;

            foreach (var module in modules)
            {
                switch (module)
                {
                    case EECalcOracleModule.R1_R2_CalendarAndDegreeHours:
                        monthlyDays = RunR1R2(context, result);
                        break;
                    case EECalcOracleModule.R3_Transmission:
                        monthlyDays ??= monthlyDaysOracle.Calculate(context.Fixture.Calculation);
                        RunR3(context, result, monthlyDays);
                        break;
                    case EECalcOracleModule.R4_HeatingGains:
                        monthlyDays ??= monthlyDaysOracle.Calculate(context.Fixture.Calculation);
                        RunR4(context, result, monthlyDays);
                        break;
                    case EECalcOracleModule.R5_HeatingBalance:
                        heatingRows = RunR5(context, result, state);
                        break;
                    case EECalcOracleModule.R6_Cooling:
                        coolingResult = RunR6(context, result, state);
                        break;
                    case EECalcOracleModule.R7_Ventilation:
                        ventilationResult = RunR7(context, result, state);
                        break;
                    case EECalcOracleModule.R8_DhwBgv:
                        dhwBgvResult = RunR8(context, result, state);
                        break;
                    case EECalcOracleModule.R9_LightingDevices:
                        lightingDevicesResult = RunR9(context, result, state, heatingRows);
                        break;
                    case EECalcOracleModule.R10_AggregationPrimaryCo2Class:
                        RunR10(context, result, state, heatingRows, coolingResult, ventilationResult, dhwBgvResult, lightingDevicesResult);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(context), module, "Unknown oracle module.");
                }
            }

            return result;
        }

        private IReadOnlyList<EecalcMonthlyDaysOracleRow> RunR1R2(
            EECalcOracleContext context,
            EECalcOracleResult result)
        {
            var fixture = context.Fixture.Calculation;
            var strictClimate = new LegacyEecalcXmlClimateDataProvider(ClimateProviderMode.LegacyEECalcStrict);
            var rows = monthlyDaysOracle.Calculate(fixture);

            foreach (var row in rows)
            {
                var avgTemp = strictClimate.GetMonthlyAvgTemp(fixture.ClimateZoneId, ToMonth(row.Month));
                result.Add(DebugRow(context, row.Month, EECalcOracleModule.R1_R2_CalendarAndDegreeHours, "EecalcMonthlyDaysOracle")
                    .With("TotalDays", row.TotalDays)
                    .With("WorkDays", row.WorkDays)
                    .With("Saturdays", row.Saturdays)
                    .With("Sundays", row.Sundays)
                    .With("Holidays", row.Holidays)
                    .With("Weeks", row.Weeks)
                    .With("AvgTempFromDefaultParamsXml", avgTemp));
            }

            AddStrictDataPreservationRows(context, result, strictClimate);
            return rows;
        }

        private void RunR3(
            EECalcOracleContext context,
            EECalcOracleResult result,
            IReadOnlyList<EecalcMonthlyDaysOracleRow> monthlyDays)
        {
            var htrOracle = new EecalcHtrQtrOracle();
            foreach (var month in monthlyDays)
            {
                htrOracle.CalculateParameterQtr(context.Fixture, month, out var snapshot);
                result.Add(DebugRow(context, month.Month, EECalcOracleModule.R3_Transmission, "EecalcHtrQtrOracle")
                    .With("AvgTemp", snapshot.AvgTemp)
                    .With("AvgInnerHeatTemp", snapshot.AvgInnerHeatTemp)
                    .With("Hd", snapshot.Hd)
                    .With("Hg", snapshot.Hg)
                    .With("HuWalls", snapshot.HuWalls)
                    .With("HuCeilings", snapshot.HuCeilings)
                    .With("HuFloors", snapshot.HuFloors)
                    .With("Hu", snapshot.Hu)
                    .With("Htr", snapshot.Htr)
                    .With("DegreeHours", snapshot.DegreeHours)
                    .With("Qtr", snapshot.Qtr));
            }
        }

        private void RunR4(
            EECalcOracleContext context,
            EECalcOracleResult result,
            IReadOnlyList<EecalcMonthlyDaysOracleRow> monthlyDays)
        {
            foreach (var month in monthlyDays)
            {
                var row = heatingOracle.CalculateMonth(context.Fixture, month);
                result.Add(DebugRow(context, month.Month, EECalcOracleModule.R4_HeatingGains, "EecalcMonthlyHeatingOracle")
                    .With("TransparentFsol", row.TransparentFsol)
                    .With("NonTransparentFsol", row.NonTransparentFsol)
                    .With("QgnRaw", row.QgnRaw)
                    .With("Qgn", row.Qgn)
                    .With("OccupantHours", row.OccupantHours)
                    .With("MetabolicHeatPerArea", row.MetabolicHeatPerArea)
                    .With("MetabolicHeat", row.MetabolicHeat));
            }
        }

        private IReadOnlyList<EecalcMonthlyHeatingOracleRow> RunR5(
            EECalcOracleContext context,
            EECalcOracleResult result,
            EECalcFullOracleState state)
        {
            var rows = heatingOracle.Calculate(context.Fixture);
            foreach (var row in rows)
            {
                result.Add(DebugRow(context, row.Month, EECalcOracleModule.R5_HeatingBalance, "EecalcMonthlyHeatingOracle")
                    .With("Qtr", row.Qtr)
                    .With("Qve", row.Qve)
                    .With("Qht", row.Qht)
                    .With("Qgn", row.Qgn)
                    .With("Tau", row.Tau)
                    .With("AH", row.AH)
                    .With("Gamma", row.Gamma)
                    .With("Ni", row.Ni)
                    .With("NiBranch", row.NiBranch)
                    .With("RawQnd", row.RawQnd)
                    .With("FinalQnd", row.FinalQnd));
            }

            state.HeatingNeeded = rows.Sum(row => row.FinalQnd);
            result.SetFullValue("HeatingFinalQndPerArea", state.HeatingNeeded);
            return rows;
        }

        private EecalcMonthlyCoolingOracleResult RunR6(
            EECalcOracleContext context,
            EECalcOracleResult result,
            EECalcFullOracleState state)
        {
            var cooling = coolingOracle.Calculate(context.Fixture);
            foreach (var row in cooling.Rows)
            {
                result.Add(DebugRow(context, row.Month, EECalcOracleModule.R6_Cooling, "EecalcMonthlyCoolingOracle")
                    .With("Qsol", row.Qsol)
                    .With("Qint", row.Qint)
                    .With("Qoccupants", row.Qoccupants)
                    .With("Qgain", row.Qgain)
                    .With("QtrCooling", row.QtrCooling)
                    .With("Qinf", row.Qinf)
                    .With("Qloss", row.Qloss)
                    .With("Ac", row.Ac)
                    .With("Gamma", row.Gamma)
                    .With("Eta", row.Eta)
                    .With("EtaBranch", row.EtaBranch)
                    .With("QLatentOccupants", row.QLatentOccupants)
                    .With("QLatentInf", row.QLatentInf)
                    .With("QLatentVent", row.QLatentVent)
                    .With("QcoolRaw", row.QcoolRaw)
                    .With("QfreeCooling", row.QfreeCooling)
                    .With("QveCooling", row.QveCooling)
                    .With("QcoolWithInputs", row.QcoolWithInputs));
            }

            state.CoolingNeeded = cooling.ResultNetEnergy;
            result.SetFullValue("CoolingNoInputsNetEnergy", cooling.ResultNoInputsNetEnergy);
            result.SetFullValue("CoolingInputs", cooling.ResultCoolingInputs);
            result.SetFullValue("CoolingVentilationInputs", cooling.ResultVentilationInputs);
            result.SetFullValue("CoolingNetEnergy", cooling.ResultNetEnergy);
            return cooling;
        }

        private EECalcVentilationOracleResult RunR7(
            EECalcOracleContext context,
            EECalcOracleResult result,
            EECalcFullOracleState state)
        {
            var ventilation = ventilationOracle.Calculate(context.Fixture.Calculation, context.Input.Ventilation);
            foreach (var row in ventilation.Rows)
            {
                result.Add(DebugRow(context, row.Month, EECalcOracleModule.R7_Ventilation, "EECalcVentilationOracle")
                    .With("MonthHours", row.MonthHours)
                    .With("AverageVentHeatTemp", row.AverageVentHeatTemp)
                    .With("FirstRecoveryTemp", row.FirstRecoveryTemp)
                    .With("PostRecoveryTemp", row.PostRecoveryTemp)
                    .With("ThermoPumpEnergy", row.ThermoPumpEnergy)
                    .With("MonthlyHeat", row.MonthlyHeat)
                    .With("HeatingInputs", row.HeatingInputs)
                    .With("PowHeating", row.PowHeating)
                    .With("PowCooling", row.PowCooling)
                    .With("WitheringEnergy", row.WitheringEnergy)
                    .With("CoolingInputs", row.CoolingInputs)
                    .With("ConfirmedKdBehaviors", "KD-V001;KD-V002;KD-V003;KD-V004;KD-V005;KD-V006;KD-V008;KD-V009;KD-V010;KD-V011;KD-V012;KD-V013;KD-V014;KD-V015"));
            }

            state.HeatingVentilationNeeded = ventilation.ResultSourceEnergy;
            state.CoolingVentilationNeeded = ventilation.ResultSourceEnergy2;
            AddFull(result, "R7_Ventilation.ResultEnergyForHeating", ventilation.ResultEnergyForHeating);
            AddFull(result, "R7_Ventilation.ResultEnergyForCooling", ventilation.ResultEnergyForCooling);
            AddFull(result, "R7_Ventilation.ResultEnergyForWithering", ventilation.ResultEnergyForWithering);
            AddFull(result, "R7_Ventilation.ResultSourceEnergy", ventilation.ResultSourceEnergy);
            AddFull(result, "R7_Ventilation.ResultSourceEnergy2", ventilation.ResultSourceEnergy2);
            AddFull(result, "R7_Ventilation.ResultNeededEnergy", ventilation.ResultNeededEnergy);
            AddFull(result, "R7_Ventilation.ResulVentilationInputs", ventilation.ResulVentilationInputs);
            return ventilation;
        }

        private EECalcDhwBgvOracleResult RunR8(
            EECalcOracleContext context,
            EECalcOracleResult result,
            EECalcFullOracleState state)
        {
            var dhw = dhwBgvOracle.Calculate(context.Fixture.Calculation, context.Input.DhwBgv);
            result.Add(DebugRow(context, 0, EECalcOracleModule.R8_DhwBgv, "EECalcDhwBgvOracle")
                .With("MixedWater", dhw.MixedWater)
                .With("ResulNetEnergy", dhw.ResulNetEnergy)
                .With("ResultEnergyForHeating", dhw.ResultEnergyForHeating)
                .With("ResultSourceEnergy", dhw.ResultSourceEnergy)
                .With("ResultSourceEnergy2", dhw.ResultSourceEnergy2)
                .With("ResultNeededEnergy", dhw.ResultNeededEnergy)
                .With("HeatEfficiencyGenerating", dhw.HeatEfficiencyGenerating)
                .With("HotWaterPumpsNeededEnergy", dhw.HotWaterPumpsNeededEnergy)
                .With("BGVSunEnergy", dhw.BGVSunEnergy)
                .With("BGVPumpsTotal", dhw.BGVPumpsTotal)
                .With("TotalUsedSunEnergy", dhw.TotalUsedSunEnergy)
                .With("LegacyNotes", "first-zone BGV; BGVPumpsTotal; ordinary DHW omits TransmitTempEfficiency; MixedWater/ResulNetEnergy difference"));

            foreach (var row in dhw.SolarRows)
            {
                result.Add(DebugRow(context, row.Month, EECalcOracleModule.R8_DhwBgv, "SunEnergyResTable")
                    .With("CollectorsArea", row.CollectorsArea)
                    .With("Ht", row.Ht)
                    .With("X", row.X)
                    .With("Y", row.Y)
                    .With("CorrectedX", row.CorrectedX)
                    .With("F", row.F)
                    .With("QhotWater", row.QhotWater)
                    .With("QsunWater", row.QsunWater)
                    .With("Fm", row.Fm)
                    .With("UsedSunEnergy", row.UsedSunEnergy)
                    .With("BGVPumps", row.BGVPumps));
            }

            state.BgvNeeded = dhw.ResultNeededEnergy;
            state.BgvPumpsNeeded = dhw.HotWaterPumpsNeededEnergy + dhw.BGVPumpsTotal;
            state.BgvSolarPumpsTotal = dhw.BGVPumpsTotal;
            AddFull(result, "R8_DhwBgv.ResultNeededEnergy", dhw.ResultNeededEnergy);
            AddFull(result, "R8_DhwBgv.BGVPumpsTotal", dhw.BGVPumpsTotal);
            AddFull(result, "R8_DhwBgv.SunEnergyResTable.BGVSunEnergy", dhw.BGVSunEnergy);
            return dhw;
        }

        private EECalcLightingDevicesOracleResult RunR9(
            EECalcOracleContext context,
            EECalcOracleResult result,
            EECalcFullOracleState state,
            IReadOnlyList<EecalcMonthlyHeatingOracleRow>? heatingRows)
        {
            heatingRows ??= heatingOracle.Calculate(context.Fixture);
            var lighting = lightingDevicesOracle.Calculate(context.Fixture.Calculation, context.Input.LightingDevices, heatingRows);
            foreach (var row in lighting.GroupRows)
            {
                result.Add(DebugRow(context, row.Month, EECalcOracleModule.R9_LightingDevices, "EECalcLightingDevicesOracle")
                    .With("Group", row.Group)
                    .With("Period", row.Period)
                    .With("ByMonths", row.ByMonths)
                    .With("Weeks", row.Weeks)
                    .With("WeekRegime", row.WeekRegime)
                    .With("DevicesNeededEnergy", row.DevicesNeededEnergy)
                    .With("FuelInputEnum", row.FuelInputEnum)
                    .With("FuelReportBucket", row.FuelReportBucket)
                    .With("LegacyNotes", "NonBalancedDevices excluded from thermal balance; CalcWeekPower weekRegime side effect preserved"));
            }

            state.LightsNeeded = lighting.LightsGeneralNeededEnergy;
            state.HeatAffectingDevicesNeeded = lighting.BalancedDevicesGeneralNeededEnergy;
            state.NonHeatAffectingDevicesNeeded = lighting.NonBalancedDevicesGeneralNeededEnergy;
            state.BgvPumpsNeeded += lighting.HotWaterPumpsGeneralNeededEnergy;
            AddFull(result, "R9_LightingDevices.Lights.General.DevicesNeededEnergy", lighting.LightsGeneralNeededEnergy);
            AddFull(result, "R9_LightingDevices.BalancedDevices.General.DevicesNeededEnergy", lighting.BalancedDevicesGeneralNeededEnergy);
            AddFull(result, "R9_LightingDevices.NonBalancedDevices.General.DevicesNeededEnergy", lighting.NonBalancedDevicesGeneralNeededEnergy);
            AddFull(result, "R9_LightingDevices.HotWaterPumps.General.DevicesNeededEnergy", lighting.HotWaterPumpsGeneralNeededEnergy);
            AddFull(result, "R9_LightingDevices.ResulLightInputs", lighting.ResulLightInputs);
            AddFull(result, "R9_LightingDevices.ResulAppliancesInputs", lighting.ResulAppliancesInputs);
            AddFull(result, "R9_LightingDevices.CoolingQintContribution", lighting.CoolingQintContribution);
            return lighting;
        }

        private static void RunR10(
            EECalcOracleContext context,
            EECalcOracleResult result,
            EECalcFullOracleState state,
            IReadOnlyList<EecalcMonthlyHeatingOracleRow>? heatingRows,
            EecalcMonthlyCoolingOracleResult? coolingResult,
            EECalcVentilationOracleResult? ventilationResult,
            EECalcDhwBgvOracleResult? dhwBgvResult,
            EECalcLightingDevicesOracleResult? lightingDevicesResult)
        {
            state.HeatingNeeded = state.HeatingNeeded != 0.0 ? state.HeatingNeeded : heatingRows?.Sum(row => row.FinalQnd) ?? 0.0;
            state.CoolingNeeded = state.CoolingNeeded != 0.0 ? state.CoolingNeeded : coolingResult?.ResultNetEnergy ?? 0.0;
            state.HeatingVentilationNeeded = state.HeatingVentilationNeeded != 0.0 ? state.HeatingVentilationNeeded : ventilationResult?.ResultSourceEnergy ?? 0.0;
            state.CoolingVentilationNeeded = state.CoolingVentilationNeeded != 0.0 ? state.CoolingVentilationNeeded : ventilationResult?.ResultSourceEnergy2 ?? 0.0;
            state.BgvNeeded = state.BgvNeeded != 0.0 ? state.BgvNeeded : dhwBgvResult?.ResultNeededEnergy ?? 0.0;
            state.BgvPumpsNeeded = state.BgvPumpsNeeded != 0.0 ? state.BgvPumpsNeeded : dhwBgvResult?.HotWaterPumpsNeededEnergy + dhwBgvResult?.BGVPumpsTotal ?? 0.0;
            state.LightsNeeded = state.LightsNeeded != 0.0 ? state.LightsNeeded : lightingDevicesResult?.LightsGeneralNeededEnergy ?? 0.0;
            state.HeatAffectingDevicesNeeded = state.HeatAffectingDevicesNeeded != 0.0 ? state.HeatAffectingDevicesNeeded : lightingDevicesResult?.BalancedDevicesGeneralNeededEnergy ?? 0.0;
            state.NonHeatAffectingDevicesNeeded = state.NonHeatAffectingDevicesNeeded != 0.0 ? state.NonHeatAffectingDevicesNeeded : lightingDevicesResult?.NonBalancedDevicesGeneralNeededEnergy ?? 0.0;
            state.FansAndPumpsNeeded = context.Input.Aggregation.FansAndPumps;
            state.OtherNeeded = context.Input.Aggregation.Other;
            ApplyLegacyAggregationGates(context.Input.Aggregation, state);

            var neededTotal = state.HeatingNeeded + state.CoolingNeeded + state.HeatingVentilationNeeded
                + state.CoolingVentilationNeeded + state.BgvNeeded + state.BgvPumpsNeeded
                + state.FansAndPumpsNeeded + state.LightsNeeded + state.HeatAffectingDevicesNeeded
                + state.NonHeatAffectingDevicesNeeded + state.OtherNeeded;

            AddFinalTableValues(result, "NeededEnergyTable", state, neededTotal);
            AddFinalTableValues(result, "NetEnergyTable", state, neededTotal);
            AddFinalTableValues(result, "NoInputsNetEnergyTable", state, neededTotal);

            var primaryTotal = AddPrimaryRows(context, result, state);
            var co2Total = AddEmissionRows(context, result, state);

            var fuelValues = new Dictionary<EECalcFuel, double>
            {
                [EECalcFuel.Fuel1] = state.CoolingNeeded + state.BgvPumpsNeeded + state.LightsNeeded
                    + state.HeatAffectingDevicesNeeded + state.NonHeatAffectingDevicesNeeded
                    + state.FansAndPumpsNeeded + state.OtherNeeded,
                [EECalcFuel.Fuel2] = state.HeatingNeeded + state.HeatingVentilationNeeded + state.BgvNeeded
            };

            var totalFuel = EECalcLegacyAggregation.CalculateTotalFuelWithDuplicateFuel1(fuelValues);
            result.SetFullValue("TotalFuel_KD_A001_DuplicateFuel1", totalFuel);

            foreach (var fuel in Enum.GetValues<EECalcFuel>())
            {
                var bucket = EECalcLegacyAggregation.MapFuelReportBucket(fuel);
                result.Add(DebugRow(context, 0, EECalcOracleModule.R10_AggregationPrimaryCo2Class, "EECalcLegacyAggregation")
                    .With("FuelInputEnum", fuel.ToString())
                    .With("FuelReportBucket", bucket.ToString())
                    .With("FuelQuantityBeforeBucket", fuelValues.TryGetValue(fuel, out var value) ? value : 0.0)
                    .With("PrimaryFuelBucket", bucket.ToString())
                    .With("EmissionSupplyBucket", bucket.ToString())
                    .With("TotalFuelIncludesDuplicateFuel1", true));
            }

            AddFull(result, "FuelEnergyTable.Total.ActualArea", totalFuel);
            AddFull(result, "PrimaryEnergyFuelTable.Total.ActualArea", primaryTotal);
            AddFull(result, "EmissionEnergySupplyTable.Total.ActualArea", co2Total);
            AddFull(result, "VEI.Total", 0.0);
            AddFull(result, "EnergyClassScale.PoiterValue", (int)primaryTotal);
            AddFull(result, "EnergyClassScale.PoiterValueBaseLine", (int)primaryTotal);
            result.Add(DebugRow(context, 0, EECalcOracleModule.R10_AggregationPrimaryCo2Class, "EECalcFullOracle")
                .With("FormulaField", "EnergyClassScaleUnavailable")
                .With("Reason", "Ref1/Ref2 primary totals are not calculated yet."));
        }

        private static void ApplyLegacyAggregationGates(
            EECalcAggregationInput aggregation,
            EECalcFullOracleState state)
        {
            if (!aggregation.HasHeating)
            {
                state.HeatingNeeded = 0.0;
                state.HeatingVentilationNeeded = 0.0;
            }

            if (!aggregation.HasCooling)
            {
                state.CoolingNeeded = 0.0;
                state.CoolingVentilationNeeded = 0.0;
            }

            if (!aggregation.IsBgvUsed)
            {
                state.BgvNeeded = 0.0;
                state.BgvPumpsNeeded = 0.0;
            }
        }

        private static void AddFinalTableValues(
            EECalcOracleResult result,
            string table,
            EECalcFullOracleState state,
            double total)
        {
            AddFull(result, $"{table}.Heating.Actual", state.HeatingNeeded);
            AddFull(result, $"{table}.Cooling.Actual", state.CoolingNeeded);
            AddFull(result, $"{table}.HeatingVentilation.Actual", state.HeatingVentilationNeeded);
            AddFull(result, $"{table}.CoolingVentilation.Actual", state.CoolingVentilationNeeded);
            AddFull(result, $"{table}.BGV.Actual", state.BgvNeeded);
            AddFull(result, $"{table}.BGVPumps.Actual", state.BgvPumpsNeeded);
            AddFull(result, $"{table}.FansAndPumps.Actual", state.FansAndPumpsNeeded);
            AddFull(result, $"{table}.Lights.Actual", state.LightsNeeded);
            AddFull(result, $"{table}.HeatAffectingDevices.Actual", state.HeatAffectingDevicesNeeded);
            AddFull(result, $"{table}.NonHeatAffectingDevices.Actual", state.NonHeatAffectingDevicesNeeded);
            AddFull(result, $"{table}.Other.Actual", state.OtherNeeded);
            AddFull(result, $"{table}.Total.Actual", total);
        }

        private static double AddPrimaryRows(EECalcOracleContext context, EECalcOracleResult result, EECalcFullOracleState state)
        {
            var direct = EECalcMath.PrimaryCoefficient(EECalcFuel.Fuel1);
            var primaryHeating = state.HeatingNeeded * EECalcMath.PrimaryCoefficient(EECalcFuel.Fuel2);
            var primaryCooling = state.CoolingNeeded * direct;
            var primaryVent = (state.HeatingVentilationNeeded + state.CoolingVentilationNeeded) * EECalcMath.PrimaryCoefficient(EECalcFuel.Fuel2);
            var primaryBgv = state.BgvNeeded * EECalcMath.PrimaryCoefficient(context.Input.DhwBgv.Fuel1);
            var primaryBgvPumps = state.BgvPumpsNeeded * direct;
            var primaryLights = state.LightsNeeded * direct;
            var primaryBalanced = state.HeatAffectingDevicesNeeded * direct;
            var primaryNonBalanced = state.NonHeatAffectingDevicesNeeded * direct;
            var primaryOther = (state.FansAndPumpsNeeded + state.OtherNeeded) * direct;
            var total = primaryHeating + primaryCooling + primaryVent + primaryBgv + primaryBgvPumps
                + primaryLights + primaryBalanced + primaryNonBalanced + primaryOther;

            AddFull(result, "PrimaryEnergyTable.Heating.Actual", primaryHeating);
            AddFull(result, "PrimaryEnergyTable.Cooling.Actual", primaryCooling);
            AddFull(result, "PrimaryEnergyTable.Ventilation.Actual", primaryVent);
            AddFull(result, "PrimaryEnergyTable.BGV.Actual", primaryBgv);
            AddFull(result, "PrimaryEnergyTable.BGVPumps.Actual", primaryBgvPumps);
            AddFull(result, "PrimaryEnergyTable.Lights.Actual", primaryLights);
            AddFull(result, "PrimaryEnergyTable.HeatAffectingDevices.Actual", primaryBalanced);
            AddFull(result, "PrimaryEnergyTable.NonHeatAffectingDevices.Actual", primaryNonBalanced);
            AddFull(result, "PrimaryEnergyTable.Other.Actual", primaryOther);
            AddFull(result, "PrimaryEnergyTable.Total.Actual", total);
            return total;
        }

        private static double AddEmissionRows(EECalcOracleContext context, EECalcOracleResult result, EECalcFullOracleState state)
        {
            var direct = EECalcMath.Co2Coefficient(EECalcFuel.Fuel1) / 1000000.0;
            var heating = state.HeatingNeeded * EECalcMath.Co2Coefficient(EECalcFuel.Fuel2) / 1000000.0;
            var cooling = state.CoolingNeeded * direct;
            var ventilation = (state.HeatingVentilationNeeded + state.CoolingVentilationNeeded)
                * EECalcMath.Co2Coefficient(EECalcFuel.Fuel2) / 1000000.0;
            var bgv = state.BgvNeeded * EECalcMath.Co2Coefficient(context.Input.DhwBgv.Fuel1) / 1000000.0;
            var bgvPumps = state.BgvPumpsNeeded * direct;
            var lights = state.LightsNeeded * direct;
            var balanced = state.HeatAffectingDevicesNeeded * direct;
            var nonBalanced = state.NonHeatAffectingDevicesNeeded * direct;
            var other = (state.FansAndPumpsNeeded + state.OtherNeeded) * direct;
            var total = heating + cooling + ventilation + bgv + bgvPumps + lights + balanced + nonBalanced + other;

            AddFull(result, "EmissionNeededEnergyTable.Heating.Actual", heating);
            AddFull(result, "EmissionNeededEnergyTable.Cooling.Actual", cooling);
            AddFull(result, "EmissionNeededEnergyTable.Ventilation.Actual", ventilation);
            AddFull(result, "EmissionNeededEnergyTable.BGV.Actual", bgv);
            AddFull(result, "EmissionNeededEnergyTable.BGVPumps.Actual", bgvPumps);
            AddFull(result, "EmissionNeededEnergyTable.Lights.Actual", lights);
            AddFull(result, "EmissionNeededEnergyTable.HeatAffectingDevices.Actual", balanced);
            AddFull(result, "EmissionNeededEnergyTable.NonHeatAffectingDevices.Actual", nonBalanced);
            AddFull(result, "EmissionNeededEnergyTable.Other.Actual", other);
            AddFull(result, "EmissionNeededEnergyTable.Total.Actual", total);
            return total;
        }

        private static void AddFull(EECalcOracleResult result, string field, double value)
        {
            result.SetFullValue(field, value);
        }

        private static void AddStrictDataPreservationRows(
            EECalcOracleContext context,
            EECalcOracleResult result,
            LegacyEecalcXmlClimateDataProvider strictClimate)
        {
            foreach (var zone in new[] { 1, 2, 3 })
            {
                result.Add(new EECalcDebugRow(
                        context.Fixture.Id,
                        context.Mode.ToString(),
                        context.Variant,
                        zone,
                        1,
                        EECalcOracleModule.R1_R2_CalendarAndDegreeHours.ToString(),
                        "DefaultParams.xml")
                    .With("ConfirmedDefect", "KD-DATA-001")
                    .With("AvgTemp", strictClimate.GetMonthlyAvgTemp(zone, Month.January)));
            }
        }

        private static EECalcDebugRow DebugRow(
            EECalcOracleContext context,
            int month,
            EECalcOracleModule module,
            string source)
        {
            return new EECalcDebugRow(
                context.Fixture.Id,
                context.Mode.ToString(),
                context.Variant,
                context.Fixture.Calculation.ClimateZoneId,
                month,
                module.ToString(),
                source);
        }

        private static Month ToMonth(int oneBasedMonth)
        {
            return (Month)(oneBasedMonth - 1);
        }
    }

    public sealed class EECalcOracleContext
    {
        public EECalcOracleContext(
            EecalcEnvelopeFixture fixture,
            EECalcOracleMode mode = EECalcOracleMode.LegacyEECalcStrict,
            string variant = "Actual",
            IReadOnlyList<EECalcOracleModule>? modules = null,
            EECalcFullOracleInput? input = null)
        {
            Fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            Mode = mode;
            Variant = variant;
            Modules = modules ?? Array.Empty<EECalcOracleModule>();
            Input = input ?? new EECalcFullOracleInput();
        }

        public EecalcEnvelopeFixture Fixture { get; }

        public EECalcOracleMode Mode { get; }

        public string Variant { get; }

        public IReadOnlyList<EECalcOracleModule> Modules { get; }

        public EECalcFullOracleInput Input { get; }
    }

    public enum EECalcOracleMode
    {
        LegacyEECalcStrict
    }

    public enum EECalcOracleModule
    {
        R1_R2_CalendarAndDegreeHours,
        R3_Transmission,
        R4_HeatingGains,
        R5_HeatingBalance,
        R6_Cooling,
        R7_Ventilation,
        R8_DhwBgv,
        R9_LightingDevices,
        R10_AggregationPrimaryCo2Class
    }

    public sealed class EECalcOracleResult
    {
        private readonly List<EECalcDebugRow> debugRows = new();
        private readonly Dictionary<string, double> fullValues = new(StringComparer.Ordinal);

        public EECalcOracleResult(string fixtureId, EECalcOracleMode mode, string variant)
        {
            FixtureId = fixtureId;
            Mode = mode;
            Variant = variant;
        }

        public string FixtureId { get; }

        public EECalcOracleMode Mode { get; }

        public string Variant { get; }

        public IReadOnlyList<EECalcDebugRow> DebugRows => debugRows;

        public IReadOnlyDictionary<string, double> FullValues => fullValues;

        public void Add(EECalcDebugRow row)
        {
            debugRows.Add(row);
        }

        public void SetFullValue(string field, double value)
        {
            fullValues[field] = value;
        }

        public void ExportDebugCsv(string debugDirectory)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(debugDirectory);
            Directory.CreateDirectory(debugDirectory);

            WriteModule(debugDirectory, EECalcOracleModule.R1_R2_CalendarAndDegreeHours, "r1_r2_calendar_degreehours.csv");
            WriteModule(debugDirectory, EECalcOracleModule.R3_Transmission, "r3_transmission.csv");
            WriteModule(debugDirectory, EECalcOracleModule.R4_HeatingGains, "r4_heating_gains.csv");
            WriteModule(debugDirectory, EECalcOracleModule.R5_HeatingBalance, "r5_heating_balance.csv");
            WriteModule(debugDirectory, EECalcOracleModule.R6_Cooling, "r6_cooling.csv");
            WriteModule(debugDirectory, EECalcOracleModule.R7_Ventilation, "r7_ventilation.csv");
            WriteModule(debugDirectory, EECalcOracleModule.R8_DhwBgv, "r8_dhw_bgv.csv");
            WriteModule(debugDirectory, EECalcOracleModule.R9_LightingDevices, "r9_lighting_devices.csv");
            WriteModule(debugDirectory, EECalcOracleModule.R10_AggregationPrimaryCo2Class, "r10_aggregation.csv");
            WriteFullResult(Path.Combine(debugDirectory, "full_result.csv"));
        }

        private void WriteModule(string debugDirectory, EECalcOracleModule module, string fileName)
        {
            var moduleName = module.ToString();
            EECalcDebugCsv.Write(
                Path.Combine(debugDirectory, fileName),
                debugRows.Where(row => row.Module == moduleName));
        }

        private void WriteFullResult(string path)
        {
            var rows = fullValues.Select(pair =>
                new EECalcDebugRow(FixtureId, Mode.ToString(), Variant, 0, 0, "FullResult", "EECalcFullOracle")
                    .With("FormulaField", pair.Key)
                    .With("Value", pair.Value));

            EECalcDebugCsv.Write(path, rows);
        }
    }

    public sealed class EECalcDebugRow
    {
        private readonly Dictionary<string, string> fields = new(StringComparer.Ordinal);

        public EECalcDebugRow(
            string fixtureId,
            string mode,
            string variant,
            int zoneIndex,
            int month,
            string module,
            string source)
        {
            FixtureId = fixtureId;
            Mode = mode;
            Variant = variant;
            ZoneIndex = zoneIndex;
            Month = month;
            Module = module;
            Source = source;
        }

        public string FixtureId { get; }

        public string Mode { get; }

        public string Variant { get; }

        public int ZoneIndex { get; }

        public int Month { get; }

        public string Module { get; }

        public string Source { get; }

        public IReadOnlyDictionary<string, string> Fields => fields;

        public EECalcDebugRow With(string key, object? value)
        {
            fields[key] = Format(value);
            return this;
        }

        private static string Format(object? value)
        {
            return value switch
            {
                null => string.Empty,
                double doubleValue => doubleValue.ToString("G17", CultureInfo.InvariantCulture),
                float floatValue => floatValue.ToString("G9", CultureInfo.InvariantCulture),
                IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                _ => value.ToString() ?? string.Empty
            };
        }
    }
}
