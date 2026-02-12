using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.ViewModels
{
    /// <summary>
    /// ViewModel for Section 14 - Ventilation Cooling.
    /// </summary>
    public class VentilationCoolingSectionViewModel : INotifyPropertyChanged
    {
        private readonly VentilationSectionData _data;
        private readonly ObjectDataSectionData? _objectData;
        private readonly CoolingSectionData? _coolingData;
        private readonly ClimateService _climateService;
        private ClimateZoneData? _climateData;
        private readonly VentCoolingCalculatorMonthly _calculator;

        private VentilationCoolingCalculationOutput? _calculationOutput;
        private bool _showDebug;
        private string _debugText = string.Empty;
        private bool _isAdjustingShares = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public VentilationCoolingSectionViewModel(
            VentilationSectionData data,
            ObjectDataSectionData? objectData,
            CoolingSectionData? coolingData)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _objectData = objectData;
            _coolingData = coolingData;
            _calculator = new VentCoolingCalculatorMonthly();
            _climateService = new ClimateService(new JsonClimateRepository());

            if (_objectData != null)
            {
                _objectData.PropertyChanged += OnObjectDataPropertyChanged;
                UpdateClimateZone();
            }

            _data.PropertyChanged += OnDataPropertyChanged;
            if (_coolingData != null)
            {
                _coolingData.PropertyChanged += OnCoolingDataPropertyChanged;
            }

            Recalculate();
        }

        public System.Collections.Generic.IReadOnlyList<VentilationCoolingModeOption> CalculationModeOptions { get; } =
            new[]
            {
                new VentilationCoolingModeOption
                {
                    Mode = VentilationCoolingCalculationMode.MechanicalRecirculation3112,
                    DisplayName = "3.11.2 — Механична вентилация с рециркулация"
                },
                new VentilationCoolingModeOption
                {
                    Mode = VentilationCoolingCalculationMode.FreshAirProcessed3113,
                    DisplayName = "3.11.3 — Пресен въздух, обработен извън зоната"
                }
            };

        public bool ShowDebug
        {
            get => _showDebug;
            set
            {
                if (_showDebug != value)
                {
                    _showDebug = value;
                    OnPropertyChanged(nameof(ShowDebug));
                }
            }
        }

        public string DebugText
        {
            get => _debugText;
            private set
            {
                if (_debugText != value)
                {
                    _debugText = value;
                    OnPropertyChanged(nameof(DebugText));
                }
            }
        }

        public double OperatingHoursPerWeek => _calculationOutput?.Result.OperatingHoursPerWeek ?? 0;

        public double AirflowRatePerM2
        {
            get => _data.AirflowRatePerM2;
            set
            {
                if (Math.Abs(_data.AirflowRatePerM2 - value) > 0.0001)
                {
                    _data.AirflowRatePerM2 = Math.Max(0.0, value);
                    OnPropertyChanged(nameof(AirflowRatePerM2));
                    Recalculate();
                }
            }
        }

        public double SupplyTemperature
        {
            get => _data.SupplyTemperature;
            set
            {
                if (Math.Abs(_data.SupplyTemperature - value) > 0.0001)
                {
                    _data.SupplyTemperature = value;
                    OnPropertyChanged(nameof(SupplyTemperature));
                    Recalculate();
                }
            }
        }

        public double RelativeHumidity
        {
            get => _data.RelativeHumidity;
            set
            {
                var clamped = Math.Clamp(value, 0, 100);
                if (Math.Abs(_data.RelativeHumidity - clamped) > 0.0001)
                {
                    _data.RelativeHumidity = clamped;
                    OnPropertyChanged(nameof(RelativeHumidity));
                    Recalculate();
                }
            }
        }

        public VentilationCoolingCalculationMode CalculationMode
        {
            get => _data.CoolingCalculationMode;
            set
            {
                if (_data.CoolingCalculationMode != value)
                {
                    _data.CoolingCalculationMode = value;
                    OnPropertyChanged(nameof(CalculationMode));
                    OnPropertyChanged(nameof(IsRecirculationMode));
                    Recalculate();
                }
            }
        }

        public bool IsRecirculationMode => CalculationMode == VentilationCoolingCalculationMode.MechanicalRecirculation3112;

        public double RecirculationPercent
        {
            get => _data.RecirculationPercent;
            set
            {
                var clamped = Math.Clamp(value, 0, 100);
                if (Math.Abs(_data.RecirculationPercent - clamped) > 0.0001)
                {
                    _data.RecirculationPercent = clamped;
                    OnPropertyChanged(nameof(RecirculationPercent));
                    Recalculate();
                }
            }
        }

        public string ClimateZoneDisplay => _climateData != null ? $"{_objectData?.ClimateZone} - {_climateData.Name}" : "(не е избрана)";

        public string CoolingSeasonDisplay
        {
            get
            {
                if (_objectData == null || !_objectData.CoolingSeasonEnabled)
                {
                    return "(неактивен)";
                }

                if (!_objectData.CoolingSeasonStartDay.HasValue || !_objectData.CoolingSeasonStartMonth.HasValue ||
                    !_objectData.CoolingSeasonEndDay.HasValue || !_objectData.CoolingSeasonEndMonth.HasValue)
                {
                    return "(липсва)";
                }

                return $"{_objectData.CoolingSeasonStartDay:00}.{_objectData.CoolingSeasonStartMonth:00} - {_objectData.CoolingSeasonEndDay:00}.{_objectData.CoolingSeasonEndMonth:00}";
            }
        }

        public double WorkingDaysInSeason => _calculationOutput?.Result.TotalWorkingDays ?? 0.0;

        public double WorkingHoursInSeason => _calculationOutput?.Result.TotalWorkingHours ?? 0.0;

        public string CoolingSeasonWarning => (_calculationOutput != null && !_calculationOutput.Result.CoolingSeasonEnabled)
            ? "Охладителният сезон не е активен."
            : string.Empty;

        // Energy source 1/2 bindings (same as section 13)
        public double EnergySource1Share
        {
            get => _data.EnergySource1.Share;
            set
            {
                var clamped = Math.Clamp(value, 0, 100);
                if (Math.Abs(_data.EnergySource1.Share - clamped) > 0.0001)
                {
                    _data.EnergySource1.Share = clamped;
                    OnPropertyChanged(nameof(EnergySource1Share));

                    if (UseSecondEnergySource && _data.EnergySource2 != null && !_isAdjustingShares)
                    {
                        try
                        {
                            _isAdjustingShares = true;
                            _data.EnergySource2.Share = Math.Clamp(100.0 - clamped, 0.0, 100.0);
                            OnPropertyChanged(nameof(EnergySource2Share));
                        }
                        finally
                        {
                            _isAdjustingShares = false;
                        }
                    }

                    Recalculate();
                }
            }
        }

        public double EnergySource1EmissionEfficiency
        {
            get => _data.EnergySource1.EmissionEfficiency;
            set
            {
                var clamped = Math.Clamp(value, 0, 100);
                if (Math.Abs(_data.EnergySource1.EmissionEfficiency - clamped) > 0.0001)
                {
                    _data.EnergySource1.EmissionEfficiency = clamped;
                    OnPropertyChanged(nameof(EnergySource1EmissionEfficiency));
                    Recalculate();
                }
            }
        }

        public double EnergySource1DistributionEfficiency
        {
            get => _data.EnergySource1.DistributionEfficiency;
            set
            {
                var clamped = Math.Clamp(value, 0, 100);
                if (Math.Abs(_data.EnergySource1.DistributionEfficiency - clamped) > 0.0001)
                {
                    _data.EnergySource1.DistributionEfficiency = clamped;
                    OnPropertyChanged(nameof(EnergySource1DistributionEfficiency));
                    Recalculate();
                }
            }
        }

        public double EnergySource1AutomaticControl
        {
            get => _data.EnergySource1.AutomaticControl;
            set
            {
                var clamped = Math.Clamp(value, 0, 100);
                if (Math.Abs(_data.EnergySource1.AutomaticControl - clamped) > 0.0001)
                {
                    _data.EnergySource1.AutomaticControl = clamped;
                    OnPropertyChanged(nameof(EnergySource1AutomaticControl));
                    Recalculate();
                }
            }
        }

        public double EnergySource1EnergyManagement
        {
            get => _data.EnergySource1.EnergyManagement;
            set
            {
                var clamped = Math.Clamp(value, 0, 100);
                if (Math.Abs(_data.EnergySource1.EnergyManagement - clamped) > 0.0001)
                {
                    _data.EnergySource1.EnergyManagement = clamped;
                    OnPropertyChanged(nameof(EnergySource1EnergyManagement));
                    Recalculate();
                }
            }
        }

        public double EnergySource1GenerationEfficiency
        {
            get => _data.EnergySource1.GenerationEfficiency;
            set
            {
                var clamped = Math.Max(value, 0);
                if (Math.Abs(_data.EnergySource1.GenerationEfficiency - clamped) > 0.0001)
                {
                    _data.EnergySource1.GenerationEfficiency = clamped;
                    OnPropertyChanged(nameof(EnergySource1GenerationEfficiency));
                    Recalculate();
                }
            }
        }

        public Models.EnergyCarrierCode? EnergySource1Carrier
        {
            get => _data.EnergySource1.EnergyCarrier;
            set
            {
                if (_data.EnergySource1.EnergyCarrier != value)
                {
                    _data.EnergySource1.EnergyCarrier = value;
                    OnPropertyChanged(nameof(EnergySource1Carrier));
                    Recalculate();
                }
            }
        }

        public bool UseSecondEnergySource
        {
            get => _data.UseSecondEnergySource;
            set
            {
                if (_data.UseSecondEnergySource != value)
                {
                    _data.UseSecondEnergySource = value;
                    if (value && _data.EnergySource2 == null)
                    {
                        _data.EnergySource2 = new VentilationEnergySource
                        {
                            Type = EnergySourceType.Electricity,
                            Share = 0
                        };
                    }
                    OnPropertyChanged(nameof(UseSecondEnergySource));
                    Recalculate();
                }
            }
        }

        public double EnergySource2Share
        {
            get => _data.EnergySource2?.Share ?? 0;
            set
            {
                if (_data.EnergySource2 == null) return;
                var clamped = Math.Clamp(value, 0, 100);
                if (Math.Abs(_data.EnergySource2.Share - clamped) > 0.0001)
                {
                    _data.EnergySource2.Share = clamped;
                    OnPropertyChanged(nameof(EnergySource2Share));

                    if (UseSecondEnergySource && !_isAdjustingShares)
                    {
                        try
                        {
                            _isAdjustingShares = true;
                            _data.EnergySource1.Share = Math.Clamp(100.0 - clamped, 0.0, 100.0);
                            OnPropertyChanged(nameof(EnergySource1Share));
                        }
                        finally
                        {
                            _isAdjustingShares = false;
                        }
                    }

                    Recalculate();
                }
            }
        }

        public double EnergySource2EmissionEfficiency
        {
            get => _data.EnergySource2?.EmissionEfficiency ?? 100;
            set
            {
                if (_data.EnergySource2 == null) return;
                var clamped = Math.Clamp(value, 0, 100);
                if (Math.Abs(_data.EnergySource2.EmissionEfficiency - clamped) > 0.0001)
                {
                    _data.EnergySource2.EmissionEfficiency = clamped;
                    OnPropertyChanged(nameof(EnergySource2EmissionEfficiency));
                    Recalculate();
                }
            }
        }

        public double EnergySource2DistributionEfficiency
        {
            get => _data.EnergySource2?.DistributionEfficiency ?? 100;
            set
            {
                if (_data.EnergySource2 == null) return;
                var clamped = Math.Clamp(value, 0, 100);
                if (Math.Abs(_data.EnergySource2.DistributionEfficiency - clamped) > 0.0001)
                {
                    _data.EnergySource2.DistributionEfficiency = clamped;
                    OnPropertyChanged(nameof(EnergySource2DistributionEfficiency));
                    Recalculate();
                }
            }
        }

        public double EnergySource2AutomaticControl
        {
            get => _data.EnergySource2?.AutomaticControl ?? 100;
            set
            {
                if (_data.EnergySource2 == null) return;
                var clamped = Math.Clamp(value, 0, 100);
                if (Math.Abs(_data.EnergySource2.AutomaticControl - clamped) > 0.0001)
                {
                    _data.EnergySource2.AutomaticControl = clamped;
                    OnPropertyChanged(nameof(EnergySource2AutomaticControl));
                    Recalculate();
                }
            }
        }

        public double EnergySource2EnergyManagement
        {
            get => _data.EnergySource2?.EnergyManagement ?? 100;
            set
            {
                if (_data.EnergySource2 == null) return;
                var clamped = Math.Clamp(value, 0, 100);
                if (Math.Abs(_data.EnergySource2.EnergyManagement - clamped) > 0.0001)
                {
                    _data.EnergySource2.EnergyManagement = clamped;
                    OnPropertyChanged(nameof(EnergySource2EnergyManagement));
                    Recalculate();
                }
            }
        }

        public double EnergySource2GenerationEfficiency
        {
            get => _data.EnergySource2?.GenerationEfficiency ?? 100;
            set
            {
                if (_data.EnergySource2 == null) return;
                var clamped = Math.Max(value, 0);
                if (Math.Abs(_data.EnergySource2.GenerationEfficiency - clamped) > 0.0001)
                {
                    _data.EnergySource2.GenerationEfficiency = clamped;
                    OnPropertyChanged(nameof(EnergySource2GenerationEfficiency));
                    Recalculate();
                }
            }
        }

        public Models.EnergyCarrierCode? EnergySource2Carrier
        {
            get => _data.EnergySource2?.EnergyCarrier;
            set
            {
                if (_data.EnergySource2 == null) return;
                if (_data.EnergySource2.EnergyCarrier != value)
                {
                    _data.EnergySource2.EnergyCarrier = value;
                    OnPropertyChanged(nameof(EnergySource2Carrier));
                    Recalculate();
                }
            }
        }

        // Outputs
        public double SensibleCooling_kWh => _calculationOutput?.Result.SensibleCoolingEnergy_kWh ?? 0.0;
        public double SensibleCooling_kWh_m2 => _calculationOutput?.Result.SensibleCoolingEnergy_kWh_m2 ?? 0.0;
        public double SensibleHeating_kWh => _calculationOutput?.Result.SensibleHeatingEnergy_kWh ?? 0.0;
        public double SensibleHeating_kWh_m2 => _calculationOutput?.Result.SensibleHeatingEnergy_kWh_m2 ?? 0.0;
        public double Latent_kWh => _calculationOutput?.Result.LatentEnergy_kWh ?? 0.0;
        public double Latent_kWh_m2 => _calculationOutput?.Result.LatentEnergy_kWh_m2 ?? 0.0;
        public double NetCoolingContribution_kWh => _calculationOutput?.Result.NetCoolingContribution_kWh ?? 0.0;
        public double NetCoolingContribution_kWh_m2 => _calculationOutput?.Result.NetCoolingContribution_kWh_m2 ?? 0.0;

        public double FinalEnergySource1_kWh => _calculationOutput?.Result.FinalEnergySource1_kWh ?? 0.0;
        public double FinalEnergySource2_kWh => _calculationOutput?.Result.FinalEnergySource2_kWh ?? 0.0;
        public double FinalEnergySource1_kWh_m2 => CooledArea_m2 > 0 ? FinalEnergySource1_kWh / CooledArea_m2 : 0.0;
        public double FinalEnergySource2_kWh_m2 => CooledArea_m2 > 0 ? FinalEnergySource2_kWh / CooledArea_m2 : 0.0;
        public double TotalFinalEnergy_kWh => _calculationOutput?.Result.TotalFinalEnergy_kWh ?? 0.0;
        public double SpecificFinalEnergy_kWh_m2 => _calculationOutput?.Result.SpecificFinalEnergy_kWh_m2 ?? 0.0;

        public double CooledArea_m2
        {
            get
            {
                if (_objectData?.CooledArea != null &&
                    double.TryParse(_objectData.CooledArea, NumberStyles.Float, CultureInfo.InvariantCulture, out double area))
                {
                    return area;
                }

                return 0.0;
            }
        }

        public string ErrorMessage => _calculationOutput?.Result.IsValid == false ? _calculationOutput.Result.ErrorMessage ?? string.Empty : string.Empty;

        private void Recalculate()
        {
            UpdateClimateZone();

            _calculationOutput = _calculator.Calculate(_data, _objectData, _climateData, _coolingData);
            DebugText = _calculationOutput != null ? BuildDebugText(_calculationOutput) : string.Empty;

            OnPropertyChanged(nameof(OperatingHoursPerWeek));
            OnPropertyChanged(nameof(CooledArea_m2));
            OnPropertyChanged(nameof(ClimateZoneDisplay));
            OnPropertyChanged(nameof(CoolingSeasonDisplay));
            OnPropertyChanged(nameof(WorkingDaysInSeason));
            OnPropertyChanged(nameof(WorkingHoursInSeason));
            OnPropertyChanged(nameof(CoolingSeasonWarning));

            OnPropertyChanged(nameof(SensibleCooling_kWh));
            OnPropertyChanged(nameof(SensibleCooling_kWh_m2));
            OnPropertyChanged(nameof(SensibleHeating_kWh));
            OnPropertyChanged(nameof(SensibleHeating_kWh_m2));
            OnPropertyChanged(nameof(Latent_kWh));
            OnPropertyChanged(nameof(Latent_kWh_m2));
            OnPropertyChanged(nameof(NetCoolingContribution_kWh));
            OnPropertyChanged(nameof(NetCoolingContribution_kWh_m2));

            OnPropertyChanged(nameof(FinalEnergySource1_kWh));
            OnPropertyChanged(nameof(FinalEnergySource2_kWh));
            OnPropertyChanged(nameof(FinalEnergySource1_kWh_m2));
            OnPropertyChanged(nameof(FinalEnergySource2_kWh_m2));
            OnPropertyChanged(nameof(TotalFinalEnergy_kWh));
            OnPropertyChanged(nameof(SpecificFinalEnergy_kWh_m2));
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(EnergySource1Carrier));
            OnPropertyChanged(nameof(EnergySource2Carrier));
        }

        private void UpdateClimateZone()
        {
            if (_objectData == null)
            {
                _climateData = null;
                return;
            }

            if (_climateService.TryGetZone(_objectData.ClimateZone, out var zone))
            {
                _climateData = zone;
            }
        }

        private void OnDataPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Recalculate();
        }

        private void OnObjectDataPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ObjectDataSectionData.ClimateZone) ||
                e.PropertyName == nameof(ObjectDataSectionData.CoolingSeasonEnabled) ||
                e.PropertyName == nameof(ObjectDataSectionData.CoolingSeasonStartDay) ||
                e.PropertyName == nameof(ObjectDataSectionData.CoolingSeasonStartMonth) ||
                e.PropertyName == nameof(ObjectDataSectionData.CoolingSeasonEndDay) ||
                e.PropertyName == nameof(ObjectDataSectionData.CoolingSeasonEndMonth) ||
                e.PropertyName == nameof(ObjectDataSectionData.VentilationCoolingWorkdaysHours) ||
                e.PropertyName == nameof(ObjectDataSectionData.VentilationCoolingSaturdayHours) ||
                e.PropertyName == nameof(ObjectDataSectionData.VentilationCoolingSundayHours) ||
                e.PropertyName == nameof(ObjectDataSectionData.CooledArea) ||
                e.PropertyName?.StartsWith("DaysOff", StringComparison.OrdinalIgnoreCase) == true)
            {
                Recalculate();
            }
        }

        private void OnCoolingDataPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CoolingSectionData.DesignTemperature))
            {
                Recalculate();
            }
        }

        private string BuildDebugText(VentilationCoolingCalculationOutput output)
        {
            var sb = new StringBuilder();
            var result = output.Result;
            var debug = output.Debug;

            sb.AppendLine("--- Debug: секция 14 - Вентилация (охлаждане) ---");
            sb.AppendLine($"Климатична зона: {debug.ClimateZoneName}");
            sb.AppendLine($"Сезон активен: {debug.SeasonEnabled}");
            if (debug.SeasonStart.HasValue && debug.SeasonEnd.HasValue)
            {
                sb.AppendLine($"SeasonStart: {debug.SeasonStart:dd.MM.yyyy}, SeasonEnd: {debug.SeasonEnd:dd.MM.yyyy}");
            }

            sb.AppendLine($"A_cool = {debug.AreaCooled_m2:F2} m2");
            sb.AppendLine($"qv_spec = {debug.AirflowRatePerM2:F3} m3/h·m2");
            sb.AppendLine($"qv = {debug.AirflowTotal_m3h:F2} m3/h");
            sb.AppendLine($"m_dot = {debug.MassFlow_kg_h:F2} kg/h");
            sb.AppendLine($"Tsup = {debug.SupplyTemperature_C:F1} °C, RHsup = {debug.SupplyRH_percent:F1}%");
            sb.AppendLine($"Mode = {debug.Mode}, Recirc% = {debug.RecirculationPercent:F1}%");
            sb.AppendLine($"T_in = {debug.T_in_C:F1} °C, RH_in = {debug.RH_in_percent:F1}% (assumed: {debug.RH_in_assumed})");
            sb.AppendLine($"Schedule h/day: workday={debug.WorkdayHours:F1}, sat={debug.SaturdayHours:F1}, sun={debug.SundayHours:F1}");
            sb.AppendLine($"{debug.HolidaysSourceNote}");
            sb.AppendLine($"Total workdays: {debug.TotalWorkdays:F1}, Total hours: {debug.TotalHours:F1}");
            sb.AppendLine(" ");
            sb.AppendLine("Month | days_in_season | rest | holidays | workdays | t_m[h] | Te | RH | h_e | h_sup | Δh | e_sens_cool | e_sens_heat | e_tot | e_lat");

            foreach (var m in debug.Months)
            {
                var rh = m.HasRH ? m.RH_m_percent?.ToString("F1", CultureInfo.InvariantCulture) : "n/a";
                sb.AppendLine($"{m.MonthName.PadRight(7)} | {m.DaysInSeason,5} | {m.RestDays,4} | {m.Holidays,4} | {m.WorkingDays,7:0.0} | {m.WorkingHours_h,6:0.0} | {m.Te_m_C,5:0.0} | {rh,5} | {m.h_e_kJkg,6:0.0} | {m.h_sup_kJkg,6:0.0} | {m.DeltaH_kJkg,5:0.0} | {m.SensibleCooling_kWh,10:0.00} | {m.SensibleHeating_kWh,10:0.00} | {m.TotalCooling_kWh,7:0.00} | {m.Latent_kWh,7:0.00}");
            }

            sb.AppendLine(" ");
            sb.AppendLine($"e_sens_cool = {debug.SensibleCooling_kWh_m2:F2} kWh/m2");
            sb.AppendLine($"e_sens_heat = {debug.SensibleHeating_kWh_m2:F2} kWh/m2");
            sb.AppendLine($"e_lat = {debug.Latent_kWh_m2:F2} kWh/m2");
            sb.AppendLine($"Contribution (net) = e_sens_cool + e_lat = {debug.NetCoolingContribution_kWh_m2:F2} kWh/m2");
            sb.AppendLine($"NetEnergyTotal = e_sens_cool + e_sens_heat + e_lat = {debug.NetEnergyTotal_kWh_m2:F2} kWh/m2");
            sb.AppendLine($"EI1 efficiency = {debug.CombinedEfficiency1:F4}, Need_EI1 = {debug.NeedEnergy1_kWh:F2} kWh");
            sb.AppendLine($"EI2 efficiency = {debug.CombinedEfficiency2:F4}, Need_EI2 = {debug.NeedEnergy2_kWh:F2} kWh");
            sb.AppendLine("--- Край на debug ---");

            return sb.ToString();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
