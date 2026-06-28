using System;
using System.Collections.Generic;

namespace EE.Doklad.Tests.Validation.FullOracle
{
    public sealed class EECalcFullOracleInput
    {
        public EECalcVentilationInput Ventilation { get; init; } = new();

        public EECalcDhwBgvInput DhwBgv { get; init; } = new();

        public EECalcLightingDevicesInput LightingDevices { get; init; } = new();

        public EECalcAggregationInput Aggregation { get; init; } = new();
    }

    public sealed class EECalcVentilationInput
    {
        public double Debit { get; init; }

        public double FlowTemperature { get; init; }

        public double FlowRelativeHumidity { get; init; } = 0.0;

        public double ProjectHumidity { get; init; } = 50.0;

        public double FirstRecEfficiency { get; init; }

        public double SecondRecEfficiency { get; init; }

        public double HeatingAirDifference { get; init; } = 5.0;

        public double MinimumEndTemperature { get; init; } = 5.0;

        public double Part1 { get; init; } = 100.0;

        public double Part2 { get; init; }

        public EECalcFuel Fuel1 { get; init; } = EECalcFuel.Fuel1;

        public EECalcFuel Fuel2 { get; init; } = EECalcFuel.Fuel2;

        public EECalcEfficiencyChain HeatingEfficiency1 { get; init; } = EECalcEfficiencyChain.OneHundred;

        public EECalcEfficiencyChain HeatingEfficiency2 { get; init; } = EECalcEfficiencyChain.OneHundred;

        public EECalcEfficiencyChain CoolingEfficiency1 { get; init; } = EECalcEfficiencyChain.OneHundred;

        public EECalcEfficiencyChain CoolingEfficiency2 { get; init; } = EECalcEfficiencyChain.OneHundred;

        public EecalcDailySchedule WorkdaySchedule { get; init; } = new() { StartHour = 0, EndHour = 24 };

        public EecalcDailySchedule SaturdaySchedule { get; init; } = new() { StartHour = 0, EndHour = 24 };

        public EecalcDailySchedule SundaySchedule { get; init; } = new() { StartHour = 0, EndHour = 24 };
    }

    public sealed class EECalcDhwBgvInput
    {
        public double Consumption { get; init; }

        public double TempDifference { get; init; }

        public double SunEnergy { get; init; }

        public double Part1 { get; init; } = 100.0;

        public double Part2 { get; init; }

        public EECalcFuel Fuel1 { get; init; } = EECalcFuel.Fuel1;

        public EECalcFuel Fuel2 { get; init; } = EECalcFuel.Fuel2;

        public EECalcDhwEfficiencyChain Efficiency1 { get; init; } = EECalcDhwEfficiencyChain.OneHundred;

        public EECalcDhwEfficiencyChain Efficiency2 { get; init; } = EECalcDhwEfficiencyChain.OneHundred;

        public double HotWaterPumpPower { get; init; }

        public double HotWaterPumpWorkSchedule { get; init; }

        public int SolarStartMonth { get; init; } = 1;

        public int SolarEndMonth { get; init; } = 12;

        public double SolarWaterUsage { get; init; }

        public double SolarHotWaterTemperature { get; init; } = 55.0;

        public double SolarColdWaterTemperature { get; init; } = 10.0;

        public double SolarDaysInWeek { get; init; } = 7.0;

        public double AbsorbingSurface { get; init; }

        public double CollectorsCount { get; init; }

        public double FR { get; init; } = 0.75;

        public double FRta { get; init; } = 0.75;

        public double AcumulatorVolume { get; init; } = 150.0;

        public int TrasparentCoverings { get; init; } = 1;

        public double Pitch { get; init; } = 45.0;

        public double ImpactEnvironment { get; init; } = 8.0;

        public bool Scheme1Selected { get; init; } = true;

        public bool Scheme2Selected { get; init; }

        public double CollectorDebit { get; init; }

        public double SpecialHeatCapacity { get; init; } = 4187.0;

        public double MTOAEfficiency { get; init; }

        public double MTOADebit { get; init; }

        public double MTOASpecialHeatCapacity { get; init; } = 4187.0;

        public bool SerpentineEfficiencyIsUsed { get; init; }

        public double SerpentineEfficiency { get; init; } = 100.0;

        public double PumpsVolume { get; init; }
    }

    public sealed class EECalcLightingDevicesInput
    {
        public EECalcEquipmentInput Lights { get; init; } = new();

        public EECalcEquipmentInput BalancedDevices { get; init; } = new();

        public EECalcEquipmentInput NonBalancedDevices { get; init; } = new();

        public EECalcEquipmentInput HotWaterPumps { get; init; } = new();
    }

    public sealed class EECalcEquipmentInput
    {
        public double HeatingPower { get; init; }

        public double HeatingWorkSchedule { get; init; }

        public double CoolingPower { get; init; }

        public double CoolingWorkSchedule { get; init; }

        public double GeneralPower { get; init; }

        public double GeneralWorkSchedule { get; init; }

        public bool ByMonths { get; init; }

        public IReadOnlyDictionary<int, EECalcMonthlyEquipmentSchedule> MonthlySchedules { get; init; } =
            new Dictionary<int, EECalcMonthlyEquipmentSchedule>();
    }

    public sealed class EECalcMonthlyEquipmentSchedule
    {
        public double WorkDays { get; init; }

        public double Saturdays { get; init; }

        public double Sundays { get; init; }

        public double WorkDaysUsedEnergy { get; init; }

        public double SaturdaysUsedEnergy { get; init; }

        public double SundaysUsedEnergy { get; init; }
    }

    public sealed class EECalcAggregationInput
    {
        public bool HasHeating { get; init; } = true;

        public bool HasCooling { get; init; } = true;

        public bool IsBgvUsed { get; init; } = true;

        public double FansAndPumps { get; init; }

        public double Other { get; init; }
    }

    public sealed class EECalcCoolingFansAndPumpsInput
    {
        public double VentilatorsCool { get; init; }

        public double VentilatorsOutdoorAirCool { get; init; }

        public double PumpVentilationCool { get; init; }

        public double CoolingPump { get; init; }

        public double EnergyManagement { get; init; } = 100.0;

        public double OtherCoolingVentilation { get; init; }

        public double OtherCooling { get; init; }
    }

    public sealed class EECalcHeatingFansAndPumpsInput
    {
        public double VentilatorsHeat { get; init; }

        public double PumpVentilationHeat { get; init; }

        public double HeatingPump { get; init; }

        public double EnergyManagement { get; init; } = 100.0;

        public double OtherHeatingVentilation { get; init; }

        public double OtherHeating { get; init; }
    }

    public sealed class EECalcHeatingFansAndPumpsResult
    {
        public double HeatingWeeks { get; init; }

        public double WeeklyVentilationHours { get; init; }

        public double WeeklyHeatingSeasonHours { get; init; }

        public double NeededEnergy { get; init; }

        public double OtherNeededEnergy { get; init; }
    }

    public sealed class EECalcCoolingFansAndPumpsResult
    {
        public double CoolingWeeks { get; init; }

        public double WeeklyCoolingVentilationHours { get; init; }

        public double WeeklyCoolingSeasonHours { get; init; }

        public double NeededEnergy { get; init; }

        public double OtherNeededEnergy { get; init; }
    }

    public sealed class EECalcEfficiencyChain
    {
        public static EECalcEfficiencyChain OneHundred { get; } = new();

        public double TransmitTempEfficiency { get; init; } = 100.0;

        public double SupplyNetEfficiency { get; init; } = 100.0;

        public double Automatic { get; init; } = 100.0;

        public double EnergyManagement { get; init; } = 100.0;

        public double GeneratorEfficiency { get; init; } = 100.0;
    }

    public sealed class EECalcDhwEfficiencyChain
    {
        public static EECalcDhwEfficiencyChain OneHundred { get; } = new();

        public double SupplyNetEfficiency { get; init; } = 100.0;

        public double Automatic { get; init; } = 100.0;

        public double EnergyManagement { get; init; } = 100.0;

        public double GeneratorHeatEfficiency { get; init; } = 100.0;
    }

    public sealed class EECalcFullOracleState
    {
        public double HeatingNeeded { get; set; }

        public double CoolingNeeded { get; set; }

        public double HeatingVentilationNeeded { get; set; }

        public double CoolingVentilationNeeded { get; set; }

        public double BgvNeeded { get; set; }

        public double BgvPumpsNeeded { get; set; }

        public double LightsNeeded { get; set; }

        public double HeatAffectingDevicesNeeded { get; set; }

        public double NonHeatAffectingDevicesNeeded { get; set; }

        public double FansAndPumpsNeeded { get; set; }

        public double OtherNeeded { get; set; }

        public double BgvSolarPumpsTotal { get; set; }

        public Dictionary<EECalcFuel, double> FuelEnergy { get; } = new();

        public Dictionary<EECalcFuel, double> PrimaryFuelEnergy { get; } = new();

        public Dictionary<EECalcFuel, double> EmissionSupplyEnergy { get; } = new();
    }

    internal static class EECalcMath
    {
        public static double CleanFinite(double value)
        {
            return double.IsNaN(value) || double.IsInfinity(value) ? 0.0 : value;
        }

        public static double EfficiencyProduct(EECalcEfficiencyChain chain)
        {
            return chain.TransmitTempEfficiency / 100.0
                * chain.SupplyNetEfficiency / 100.0
                * chain.Automatic / 100.0
                * chain.EnergyManagement / 100.0
                * chain.GeneratorEfficiency / 100.0;
        }

        public static double EfficiencyProduct(EECalcDhwEfficiencyChain chain)
        {
            return chain.SupplyNetEfficiency / 100.0
                * chain.Automatic / 100.0
                * chain.EnergyManagement / 100.0
                * chain.GeneratorHeatEfficiency / 100.0;
        }

        public static double DivideByEfficiency(double value, double efficiencyProduct)
        {
            return CleanFinite(efficiencyProduct == 0.0 ? 0.0 : value / efficiencyProduct);
        }

        public static double PrimaryCoefficient(EECalcFuel fuel)
        {
            return fuel switch
            {
                EECalcFuel.Fuel1 => 3.0,
                EECalcFuel.Fuel2 => 1.1,
                EECalcFuel.Fuel3 => 1.1,
                EECalcFuel.Fuel4 => 1.2,
                EECalcFuel.Fuel5 => 1.2,
                EECalcFuel.Fuel6 => 1.05,
                EECalcFuel.Fuel7 => 1.25,
                EECalcFuel.Fuel8 => 1.1,
                EECalcFuel.Fuel9 => 1.3,
                EECalcFuel.Fuel10 => 1.1,
                EECalcFuel.Fuel11 => 1.2,
                _ => 0.0
            };
        }

        public static double Co2Coefficient(EECalcFuel fuel)
        {
            return fuel switch
            {
                EECalcFuel.Fuel1 => 819.0,
                EECalcFuel.Fuel2 => 202.0,
                EECalcFuel.Fuel3 => 227.0,
                EECalcFuel.Fuel4 => 341.0,
                EECalcFuel.Fuel5 => 364.0,
                EECalcFuel.Fuel6 => 43.0,
                EECalcFuel.Fuel7 => 351.0,
                EECalcFuel.Fuel8 => 267.0,
                EECalcFuel.Fuel9 => 290.0,
                EECalcFuel.Fuel10 => 279.0,
                EECalcFuel.Fuel11 => 354.0,
                _ => 0.0
            };
        }
    }
}
