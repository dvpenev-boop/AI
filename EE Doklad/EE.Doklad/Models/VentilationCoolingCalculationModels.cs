using System;
using System.Collections.Generic;

namespace EE.Doklad.Models
{
    public enum VentilationCoolingCalculationMode
    {
        /// <summary>
        /// 3.11.2 — Механична вентилация с рециркулация.
        /// </summary>
        MechanicalRecirculation3112,

        /// <summary>
        /// 3.11.3 — Пресен въздух, обработен извън зоната (без рециркулация).
        /// </summary>
        FreshAirProcessed3113
    }

    public sealed class VentilationCoolingModeOption
    {
        public VentilationCoolingCalculationMode Mode { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }

    public sealed class VentilationCoolingMonthlyResult
    {
        public int MonthNumber { get; set; }
        public string MonthName { get; set; } = string.Empty;

        public double OutdoorTemperature_C { get; set; }
        public double? OutdoorRelativeHumidityPercent { get; set; }
        public bool HasHumidityData { get; set; }

        public double WorkingHours_h { get; set; }
        public double WorkingDays { get; set; }
        public double WorkingDaysWeekday { get; set; }
        public double WorkingDaysSaturday { get; set; }
        public double WorkingDaysSunday { get; set; }

        public double SensibleCoolingEnergy_kWh { get; set; }
        public double SensibleHeatingEnergy_kWh { get; set; }
        public double TotalCoolingEnergy_kWh { get; set; }
        public double LatentEnergy_kWh { get; set; }
    }

    public sealed class VentilationCoolingCalculationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }

        public bool CoolingSeasonEnabled { get; set; }
        public DateTime? SeasonStart { get; set; }
        public DateTime? SeasonEnd { get; set; }

        public int ClimateZoneId { get; set; }
        public string ClimateZoneName { get; set; } = string.Empty;

        public double CooledArea_m2 { get; set; }
        public double AirflowRatePerM2 { get; set; }
        public double SupplyTemperature_C { get; set; }
        public double SupplyRelativeHumidityPercent { get; set; }
        public double OperatingHoursPerWeek { get; set; }

        public double TotalWorkingDays { get; set; }
        public double TotalWorkingHours { get; set; }

        public List<VentilationCoolingMonthlyResult> MonthlyResults { get; set; } = new();

        public double SensibleCoolingEnergy_kWh { get; set; }
        public double SensibleCoolingEnergy_kWh_m2 { get; set; }
        public double SensibleHeatingEnergy_kWh { get; set; }
        public double SensibleHeatingEnergy_kWh_m2 { get; set; }
        public double LatentEnergy_kWh { get; set; }
        public double LatentEnergy_kWh_m2 { get; set; }
        public double NetCoolingContribution_kWh { get; set; }
        public double NetCoolingContribution_kWh_m2 { get; set; }

        public double NetEnergyTotal_kWh { get; set; }

        public double FinalEnergySource1_kWh { get; set; }
        public double FinalEnergySource2_kWh { get; set; }
        public double TotalFinalEnergy_kWh { get; set; }
        public double SpecificFinalEnergy_kWh_m2 { get; set; }
    }

    public sealed class VentilationCoolingMonthlyDebug
    {
        public int MonthNumber { get; set; }
        public string MonthName { get; set; } = string.Empty;

        public int DaysInSeason { get; set; }
        public int RestDays { get; set; }
        public int Holidays { get; set; }
        public double WorkingDays { get; set; }

        public double WorkdaysWeekday { get; set; }
        public double WorkdaysSaturday { get; set; }
        public double WorkdaysSunday { get; set; }

        public double WorkingHours_h { get; set; }

        public double Te_m_C { get; set; }
        public double? RH_m_percent { get; set; }
        public bool HasRH { get; set; }

        public double h_e_kJkg { get; set; }
        public double h_sup_kJkg { get; set; }
        public double DeltaH_kJkg { get; set; }

    public double x_e_kgkg { get; set; }
    public double x_sup_kgkg { get; set; }

    public double Q_sens_kWh { get; set; }
    public double Q_lat_kWh { get; set; }
    public double Q_total_kWh { get; set; }

        public double SensibleCooling_kWh { get; set; }
        public double SensibleHeating_kWh { get; set; }
        public double TotalCooling_kWh { get; set; }
        public double Latent_kWh { get; set; }

        public double qv_fresh_m3h { get; set; }
        public double qv_rec_m3h { get; set; }
        public double h_in_kJkg { get; set; }
        public double h_mix_kJkg { get; set; }
        public double T_mix_C { get; set; }
        // Psychrometric debug (supply / outdoor)
        public double p_ws_sup_Pa { get; set; }
        public double p_w_sup_Pa { get; set; }
        public double W_sup_kgkg { get; set; }

        public double p_ws_out_Pa { get; set; }
        public double p_w_out_Pa { get; set; }
        public double W_out_kgkg { get; set; }
    }

    public sealed class VentilationCoolingDebugInfo
    {
        public bool SeasonEnabled { get; set; }
        public DateTime? SeasonStart { get; set; }
        public DateTime? SeasonEnd { get; set; }
        public string ClimateZoneName { get; set; } = string.Empty;

        public string HolidaysSourceNote { get; set; } = string.Empty;

        public double WorkdayHours { get; set; }
        public double SaturdayHours { get; set; }
        public double SundayHours { get; set; }

        public double TotalWorkdays { get; set; }
        public double TotalHours { get; set; }

        public double AreaCooled_m2 { get; set; }
        public double AirflowRatePerM2 { get; set; }
        public double AirflowTotal_m3h { get; set; }
        public double MassFlow_kg_h { get; set; }
        public double SupplyTemperature_C { get; set; }
        public double SupplyRH_percent { get; set; }

        public VentilationCoolingCalculationMode Mode { get; set; }
        public double RecirculationPercent { get; set; }
        public double? T_in_C { get; set; }
        public double? RH_in_percent { get; set; }
        public bool RH_in_assumed { get; set; }

        public double SensibleCooling_kWh_m2 { get; set; }
        public double SensibleHeating_kWh_m2 { get; set; }
        public double Latent_kWh_m2 { get; set; }
        public double NetCoolingContribution_kWh_m2 { get; set; }
        public double NetEnergyTotal_kWh_m2 { get; set; }

        public double CombinedEfficiency1 { get; set; }
        public double CombinedEfficiency2 { get; set; }
        public double NeedEnergy1_kWh { get; set; }
        public double NeedEnergy2_kWh { get; set; }

        public List<VentilationCoolingMonthlyDebug> Months { get; set; } = new();
        public System.Collections.Generic.List<string> Warnings { get; set; } = new();
    }

    public sealed class VentilationCoolingCalculationOutput
    {
        public VentilationCoolingCalculationResult Result { get; set; } = new();
        public VentilationCoolingDebugInfo Debug { get; set; } = new();
    }
}
