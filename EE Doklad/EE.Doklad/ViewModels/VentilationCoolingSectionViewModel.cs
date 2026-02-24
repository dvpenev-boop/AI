using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Win32;
using EE.Doklad.Models;
using EE.Doklad.Models.Climate;
using EE.Doklad.Services;
using EE.Doklad.Services.Climate;
using EE.Doklad.Services.Psychrometrics;
using EE.Doklad.Services.Schedule;
using EE.Doklad.Services.VentCooling;

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
        private readonly Report? _report;
        private readonly ClimateService _climateService;
        private ClimateZoneData? _climateData;
    // Legacy monthly calculator removed; Engine V2 provides outputs.
        private VentCoolingOutputV2 _outputV2 = new VentCoolingOutputV2 { IsValid = false, ErrorMessage = "Не е изчислено." };
        // Нетен принос към охлаждането – изчислен по новата формула (BaseFactor × ΔT, f_on от графиците).
        private VentCoolingContributionResult _contribResult = new VentCoolingContributionResult { IsValid = false, ErrorMessage = "Не е изчислено." };
        // Принос от нощна вентилация с НЕобработен въздух (sensible-only free cooling).
        private NightVentResult _nightVentResult = NightVentResult.Fail("Не е изчислено.");
        // Паралелен sensible резултат — използва се само за debug сравнение когато ThermalMass е активен.
        private NightVentResult _nightVentSensibleResult = NightVentResult.Fail("Не е изчислено.");
        private bool _showDebug;
    private string _debugText = string.Empty;
    // Optional override prefix that replaces the displayed DebugText when non-null.
    // We use this to prefix the existing Engine V2 debug with Cooling (Section 12)
    // sourced values (design temperature, "температура с повишение" and RH) so
    // the UI shows the correct zone setpoint coming from Section 12.
    private string? _debugTextOverride = null;
        private bool _isAdjustingShares = false;

        private readonly VentCoolingEngineV2 _engineV2 = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Expose ObjectData for XAML binding (e.g., to CoolingSchedules).
        /// </summary>
        public ObjectDataSectionData? ObjectData => _objectData;

        public VentilationCoolingSectionViewModel(
            VentilationSectionData data,
            ObjectDataSectionData? objectData,
            CoolingSectionData? coolingData,
            Report? report = null)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _objectData = objectData;
            _coolingData = coolingData;
            _report = report;
            // legacy calculator removed; Engine V2 will be used instead
            _climateService = new ClimateService(new JsonClimateRepository());

            if (_objectData != null)
            {
                _objectData.PropertyChanged += OnObjectDataPropertyChanged;
                UpdateClimateZone();
                
                // Subscribe to nested CoolingSchedules changes
                if (_objectData.CoolingSchedules != null)
                {
                    SubscribeToCoolingSchedules(_objectData.CoolingSchedules);
                }
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
            get => _debugTextOverride ?? _debugText;
            private set
            {
                if (_debugText != value)
                {
                    _debugText = value;
                    // When the engine updates the raw debug text we rebuild the override
                    // so that the displayed DebugText prefixed with Section 12 info is kept
                    // in sync.
                    UpdateDebugOverride();
                    OnPropertyChanged(nameof(DebugText));
                }
            }
        }

        /// <summary>
        /// CSV съдържание с всички почасови debug стойности (всички месеци × 24 часа).
        /// Предназначено за copy-paste в Excel или за "Save as CSV" бутон.
        /// </summary>
        public string HourlyDebugCsv => _outputV2.IsValid ? _outputV2.BuildHourlyDebugCsv() : string.Empty;

        /// <summary>
        /// Команда: запис на <see cref="HourlyDebugCsv"/> в CSV файл (SaveFileDialog).
        /// </summary>
        public ICommand ExportHourlyDebugCsvCommand => new RelayCommand(_ =>
        {
            try
            {
                var dlg = new SaveFileDialog
                {
                    Title            = "Запис на почасов debug CSV",
                    Filter           = "CSV файлове (*.csv)|*.csv|Всички файлове (*.*)|*.*",
                    DefaultExt       = "csv",
                    FileName         = "Section14_HourlyDebug.csv",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                };
                if (dlg.ShowDialog() == true)
                {
                    System.IO.File.WriteAllText(dlg.FileName, HourlyDebugCsv, System.Text.Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Грешка при запис: {ex.Message}", "Грешка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        });

        // Build or refresh the DebugText override which prefixes the engine's debug
        // output with the Cooling section (Section 12) design temperature, the
        // "температура с повишение" and the RH (informational only).
        private void UpdateDebugOverride()
        {
            try
            {
                if (_coolingData != null)
                {
                    var prefix = new StringBuilder();
                    prefix.AppendLine($"T_zone (from Section 12 - Охлаждане) : {_coolingData.DesignTemperature:F2} °C");
                    prefix.AppendLine($"Температура с повишение : {_coolingData.ReductionTemperature:F2} °C  (informational)");
                    prefix.AppendLine($"RH (Cooling section) : {_coolingData.RelativeHumidity:F1} %  (informational)");
                    prefix.AppendLine();

                    _debugTextOverride = prefix.ToString() + _debugText;
                }
                else
                {
                    // No Cooling section available — clear the override so the existing
                    // debug (which may use Section 14 values) is shown unchanged.
                    _debugTextOverride = null;
                }

                // Notify UI that DebugText may have changed due to override update.
                OnPropertyChanged(nameof(DebugText));
            }
            catch
            {
                // Never throw from a debug helper — fall back to showing the raw text.
                _debugTextOverride = null;
            }
        }

        public double OperatingHoursPerWeek
        {
            get
            {
                // Show the nominal weekly hours derived from the building schedules (before subtracting days-off)
                if (_objectData != null)
                {
                    double workdayHours = ParseHours(_objectData.VentilationCoolingWorkdaysHours);
                    double saturdayHours = ParseHours(_objectData.VentilationCoolingSaturdayHours);
                    double sundayHours = ParseHours(_objectData.VentilationCoolingSundayHours);
                    // Nominal week: 5 workdays + 1 saturday + 1 sunday
                    return workdayHours * 5.0 + saturdayHours + sundayHours;
                }

                // Fallback: no object data — return 0 (legacy calculator removed)
                return 0.0;
            }
        }

        private static double ParseHours(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0.0;
            if (double.TryParse(s.Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var v)) return v;
            if (double.TryParse(s.Trim(), out v)) return v;
            return 0.0;
        }

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

    public double WorkingDaysInSeason => _outputV2.TotalWorkingDays;

    public double WorkingHoursInSeason => _outputV2.TotalWorkingHours;

        /// <summary>
        /// Указва дали охладителният период е включен (контролира gating на секцията)
        /// </summary>
        public bool IsCoolingSeasonEnabled => _objectData?.CoolingSeasonEnabled ?? false;

        public string CoolingSeasonWarning => !IsCoolingSeasonEnabled
            ? "Не е избран охладителен период."
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

    // Outputs (mapped to Engine V2)
    public double SensibleCooling_kWh => _outputV2.TotalCoolNet_kWh;
    public double SensibleCooling_kWh_m2 => _outputV2.TotalCoolNet_kWhm2;
    public double SensibleHeating_kWh => _outputV2.TotalHeatNet_kWh;
    public double SensibleHeating_kWh_m2 => _outputV2.TotalHeatNet_kWhm2;
    public double Latent_kWh => _outputV2.TotalDryNet_kWh;
    public double Latent_kWh_m2 => _outputV2.TotalDryNet_kWhm2;
    public double NetCoolingContribution_kWh => _contribResult.Net_kWh;
    public double NetCoolingContribution_kWh_m2 => _contribResult.Net_kWhm2;

    // ── Допълнителни полета от новото изчисление на нетния принос ────────────
    /// <summary>Сценарий с проектна температура [kWh/m²].</summary>
    public double NetContrib_ScenarioDesign_kWhm2 => _contribResult.ScenarioDesign_kWhm2;
    /// <summary>Сценарий с повишена температура [kWh/m²].</summary>
    public double NetContrib_ScenarioRaised_kWhm2 => _contribResult.ScenarioRaised_kWhm2;
    /// <summary>Минимален сценарий [kWh/m²].</summary>
    public double NetContrib_Min_kWhm2 => _contribResult.Min_kWhm2;
    /// <summary>Максимален сценарий [kWh/m²].</summary>
    public double NetContrib_Max_kWhm2 => _contribResult.Max_kWhm2;
    /// <summary>Коефициент на застъпване на графиците f_on [0..1].</summary>
    public double NetContrib_Fon => _contribResult.F_on;
    /// <summary>Base factor ρCp·q·H/1000 [kWh/m²·K].</summary>
    public double NetContrib_BaseFactor => _contribResult.BaseFactor;

    /// <summary>
    /// Общ брой работни часове за сезона – UI поле „Основни параметри" (Секция 14).
    /// Използва се директно в изчислението на нетния принос.
    /// </summary>
    public double TotalWorkHoursSeason
    {
        get => _data.TotalWorkHoursSeason;
        set
        {
            var clamped = Math.Max(0.0, value);
            if (Math.Abs(_data.TotalWorkHoursSeason - clamped) > 0.001)
            {
                _data.TotalWorkHoursSeason = clamped;
                OnPropertyChanged(nameof(TotalWorkHoursSeason));
                Recalculate();
            }
        }
    }

    // ── Нощна вентилация (free cooling, sensible-only) ────────────────────────

    /// <summary>
    /// Специфичен дебит за нощна вентилация [m³/h·m²].
    /// VdotNight = NightVentSpecAirflow × CooledArea_m2.
    /// </summary>
    public double NightVentSpecAirflow
    {
        get => _data.NightVentSpecAirflow;
        set
        {
            var clamped = Math.Max(0.0, value);
            if (Math.Abs(_data.NightVentSpecAirflow - clamped) > 0.0001)
            {
                _data.NightVentSpecAirflow = clamped;
                OnPropertyChanged(nameof(NightVentSpecAirflow));
                Recalculate();
            }
        }
    }

    /// <summary>
    /// Режим на изчисление на нощната вентилация:
    /// false (default) → sensible-only (backward-compatible).
    /// true            → 1R1C термична маса (PHPP-подобен, c_eff от Секция 12).
    /// </summary>
    public bool NightVentUseThermalMass
    {
        get => _data.NightVentUseThermalMass;
        set
        {
            if (_data.NightVentUseThermalMass != value)
            {
                _data.NightVentUseThermalMass = value;
                OnPropertyChanged(nameof(NightVentUseThermalMass));
                Recalculate();
            }
        }
    }

    /// <summary>Принос от нощна вентилация с НЕобработен въздух [kWh/m²].</summary>
    public double NightVentContribution_kWh_m2 => _nightVentResult.SpecificKWhPerM2;

    /// <summary>Принос от нощна вентилация с НЕобработен въздух [kWh].</summary>
    public double NightVentContribution_kWh => _nightVentResult.TotalKWh;

    /// <summary>true ако изчислението на нощната вентилация е успешно.</summary>
    public bool NightVentIsValid => _nightVentResult.IsValid;

    /// <summary>Debug текст специфично за нощна вентилация.</summary>
    public string NightVentDebugText => BuildNightVentDebugText(_nightVentResult, _nightVentSensibleResult, _data.NightVentUseThermalMass);

    public double FinalEnergySource1_kWh => _outputV2.FinalEnergyEI1_kWhm2 * CooledArea_m2;
    public double FinalEnergySource2_kWh => _outputV2.FinalEnergyEI2_kWhm2 * CooledArea_m2;
    public double FinalEnergySource1_kWh_m2 => _outputV2.FinalEnergyEI1_kWhm2;
    public double FinalEnergySource2_kWh_m2 => _outputV2.FinalEnergyEI2_kWhm2;
    // Backwards-compatible bindings used by the shared view XAML
    public double FinalEnergySource1_kWh_per_m2 => FinalEnergySource1_kWh_m2;
    public double FinalEnergySource2_kWh_per_m2 => FinalEnergySource2_kWh_m2;
    public double TotalFinalEnergy_kWh => _outputV2.TotalFinalEnergy_kWh;
    public double SpecificFinalEnergy_kWh_m2 => _outputV2.TotalFinalEnergy_kWhm2;

        // ── Engine V2 results (нова методика 7257_1 §3.14) ───────────────────────

        /// <summary>Ефективност на рекуперация η_r [%] (0-100). Входен параметър.</summary>
        public double RecuperationEfficiency
        {
            get => _data.FirstStageRecuperationEfficiency;
            set
            {
                var clamped = Math.Clamp(value, 0.0, 100.0);
                if (Math.Abs(_data.FirstStageRecuperationEfficiency - clamped) > 0.0001)
                {
                    _data.FirstStageRecuperationEfficiency = clamped;
                    OnPropertyChanged(nameof(RecuperationEfficiency));
                    Recalculate();
                }
            }
        }

        // Нетни енергии [kWh/m²]
        public double V2_CoolNet_kWhm2     => _outputV2.TotalCoolNet_kWhm2;
        public double V2_HeatNet_kWhm2     => _outputV2.TotalHeatNet_kWhm2;
        public double V2_DryNet_kWhm2      => _outputV2.TotalDryNet_kWhm2;
        public double V2_VentContrib_kWhm2 => _outputV2.TotalVentContrib_kWhm2;
        public double V2_TotalNet_kWhm2    => _outputV2.TotalNetEnergy_kWhm2;

        // Потребна доставена енергия [kWh/m²]
        public double V2_FinalEI1_kWhm2    => _outputV2.FinalEnergyEI1_kWhm2;
        public double V2_FinalEI2_kWhm2    => _outputV2.FinalEnergyEI2_kWhm2;
        public double V2_TotalFinal_kWhm2  => _outputV2.TotalFinalEnergy_kWhm2;

        // Потребна доставена енергия [kWh] = kWh/m² × охлаждаема площ
        public double V2_FinalEI1_kWh      => V2_FinalEI1_kWhm2 * CooledArea_m2;
        public double V2_FinalEI2_kWh      => V2_FinalEI2_kWhm2 * CooledArea_m2;

        // Абсолютни стойности [kWh]
        public double V2_CoolNet_kWh       => _outputV2.TotalCoolNet_kWh;
        public double V2_HeatNet_kWh       => _outputV2.TotalHeatNet_kWh;
        public double V2_DryNet_kWh        => _outputV2.TotalDryNet_kWh;
        public double V2_TotalFinal_kWh    => _outputV2.TotalFinalEnergy_kWh;

        public double V2_WorkingDays       => _outputV2.TotalWorkingDays;
        public double V2_WorkingHours      => _outputV2.TotalWorkingHours;

        public string V2_ErrorMessage      => !_outputV2.IsValid ? _outputV2.ErrorMessage ?? string.Empty : string.Empty;
        public bool   V2_IsValid           => _outputV2.IsValid;

        public double CooledArea_m2
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_objectData?.CooledArea))
                {
                    // Accept both comma and dot as decimal separators (user input may use either).
                    var normalized = _objectData.CooledArea.Replace(',', '.').Trim();
                    if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double area))
                        return area;
                }

                return 0.0;
            }
        }

    public string ErrorMessage => V2_ErrorMessage;

        private void Recalculate()
        {
            UpdateClimateZone();

            // legacy calculation removed
            _outputV2 = RunEngineV2();

            // Нетен принос към охлаждането (нова формула – замества старата психрометрична)
            _contribResult = RunContribCalculator();

            // Принос от нощна вентилация с НЕобработен въздух (sensible-only)
            _nightVentSensibleResult = RunNightVentSensible();
            // Основен резултат: sensible или thermal-mass в зависимост от чекбокс
            _nightVentResult = _data.NightVentUseThermalMass
                ? RunNightVentThermalMass()
                : _nightVentSensibleResult;

            DebugText = BuildDebugText(_outputV2);

            OnPropertyChanged(nameof(OperatingHoursPerWeek));
            OnPropertyChanged(nameof(CooledArea_m2));
            OnPropertyChanged(nameof(ClimateZoneDisplay));
            OnPropertyChanged(nameof(CoolingSeasonDisplay));
            OnPropertyChanged(nameof(WorkingDaysInSeason));
            OnPropertyChanged(nameof(WorkingHoursInSeason));
            OnPropertyChanged(nameof(WorkDaysSeason));
            OnPropertyChanged(nameof(WorkHoursSeason));
            OnPropertyChanged(nameof(CoolingSeasonWarning));

            OnPropertyChanged(nameof(SensibleCooling_kWh));
            OnPropertyChanged(nameof(SensibleCooling_kWh_m2));
            OnPropertyChanged(nameof(SensibleHeating_kWh));
            OnPropertyChanged(nameof(SensibleHeating_kWh_m2));
            OnPropertyChanged(nameof(Latent_kWh));
            OnPropertyChanged(nameof(Latent_kWh_m2));
            OnPropertyChanged(nameof(NetCoolingContribution_kWh));
            OnPropertyChanged(nameof(NetCoolingContribution_kWh_m2));
            OnPropertyChanged(nameof(NetContrib_ScenarioDesign_kWhm2));
            OnPropertyChanged(nameof(NetContrib_ScenarioRaised_kWhm2));
            OnPropertyChanged(nameof(NetContrib_Min_kWhm2));
            OnPropertyChanged(nameof(NetContrib_Max_kWhm2));
            OnPropertyChanged(nameof(NetContrib_Fon));
            OnPropertyChanged(nameof(NetContrib_BaseFactor));
            OnPropertyChanged(nameof(TotalWorkHoursSeason));

            OnPropertyChanged(nameof(FinalEnergySource1_kWh));
            OnPropertyChanged(nameof(FinalEnergySource2_kWh));
            OnPropertyChanged(nameof(FinalEnergySource1_kWh_m2));
            OnPropertyChanged(nameof(FinalEnergySource2_kWh_m2));
            OnPropertyChanged(nameof(TotalFinalEnergy_kWh));
            OnPropertyChanged(nameof(SpecificFinalEnergy_kWh_m2));
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(EnergySource1Carrier));
            OnPropertyChanged(nameof(EnergySource2Carrier));

            // V2 properties
            OnPropertyChanged(nameof(V2_CoolNet_kWhm2));
            OnPropertyChanged(nameof(V2_HeatNet_kWhm2));
            OnPropertyChanged(nameof(V2_DryNet_kWhm2));
            OnPropertyChanged(nameof(V2_VentContrib_kWhm2));
            OnPropertyChanged(nameof(V2_TotalNet_kWhm2));
            OnPropertyChanged(nameof(V2_FinalEI1_kWhm2));
            OnPropertyChanged(nameof(V2_FinalEI2_kWhm2));
            OnPropertyChanged(nameof(V2_FinalEI1_kWh));
            OnPropertyChanged(nameof(V2_FinalEI2_kWh));
            OnPropertyChanged(nameof(V2_TotalFinal_kWhm2));
            OnPropertyChanged(nameof(V2_CoolNet_kWh));
            OnPropertyChanged(nameof(V2_HeatNet_kWh));
            OnPropertyChanged(nameof(V2_DryNet_kWh));
            OnPropertyChanged(nameof(V2_TotalFinal_kWh));
            OnPropertyChanged(nameof(V2_WorkingDays));
            OnPropertyChanged(nameof(V2_WorkingHours));
            OnPropertyChanged(nameof(V2_ErrorMessage));
            OnPropertyChanged(nameof(V2_IsValid));
            OnPropertyChanged(nameof(HourlyDebugCsv));

            // Нощна вентилация
            OnPropertyChanged(nameof(NightVentContribution_kWh));
            OnPropertyChanged(nameof(NightVentContribution_kWh_m2));
            OnPropertyChanged(nameof(NightVentIsValid));
            OnPropertyChanged(nameof(NightVentDebugText));
            OnPropertyChanged(nameof(NightVentUseThermalMass));
        }

        /// <summary>
        /// Изгражда VentCoolingInputV2 и изпълнява Engine V2.
        /// Използва BgAvgClimateProvider при BG данни.
        /// </summary>
        private VentCoolingOutputV2 RunEngineV2()
        {
            static VentCoolingOutputV2 Fail(string msg) =>
                new VentCoolingOutputV2 { IsValid = false, ErrorMessage = msg };

            if (_objectData == null)  return Fail("Липсват данни за обекта (секция 5).");
            if (_climateData == null) return Fail("Липсват климатични данни. Изберете климатична зона.");
            if (!_objectData.CoolingSeasonEnabled) return Fail("Охладителният сезон не е активиран.");

            // ── Season ───────────────────────────────────────────────────────────
            if (!_objectData.CoolingSeasonStartMonth.HasValue || !_objectData.CoolingSeasonStartDay.HasValue ||
                !_objectData.CoolingSeasonEndMonth.HasValue   || !_objectData.CoolingSeasonEndDay.HasValue)
                return Fail("Не са въведени дати на охладителния сезон.");

            int yearRef = 2024; // use 2024 for calendar calculations to match legacy behavior
            int sm = _objectData.CoolingSeasonStartMonth.Value, sd = _objectData.CoolingSeasonStartDay.Value;
            int em = _objectData.CoolingSeasonEndMonth.Value,   ed = _objectData.CoolingSeasonEndDay.Value;
            var seasonStart = new DateTime(yearRef, sm, Math.Min(sd, DateTime.DaysInMonth(yearRef, sm)));
            var seasonEnd   = new DateTime(yearRef, em, Math.Min(ed, DateTime.DaysInMonth(yearRef, em)));
            if (seasonEnd < seasonStart) seasonEnd = seasonEnd.AddYears(1);

            // ── Area + airflow ────────────────────────────────────────────────────
            if (!double.TryParse(_objectData.CooledArea, NumberStyles.Float, CultureInfo.InvariantCulture, out double area) || area <= 0)
                return Fail($"Невалидна охлаждаема площ: '{_objectData.CooledArea}'. Трябва да е > 0.");
            if (_data.CoolingSupplyAirflow <= 0)
                return Fail("Дебитът на приточен въздух трябва да е > 0.");

            // ── Schedule (from CoolingSchedules model) ────────────────────────────
            var ventSched = _objectData.CoolingSchedules?.VentilationCoolingSchedule;
            var coolSched = _objectData.CoolingSchedules?.CoolingSchedule;

            WeeklyScheduleConfig ventConfig;
            WeeklyScheduleConfig? coolConfig = null;

            if (ventSched != null && (ventSched.Workdays.GetHours() > 0 || ventSched.Saturday.GetHours() > 0 || ventSched.Sunday.GetHours() > 0))
            {
                // Derive StartHour/EndHour from TimeSpan schedule
                ventConfig = BuildWeeklyConfig(ventSched);
                if (coolSched != null && (coolSched.Workdays.GetHours() > 0 || coolSched.Saturday.GetHours() > 0 || coolSched.Sunday.GetHours() > 0))
                    coolConfig = BuildWeeklyConfig(coolSched);
            }
            else
            {
                // Fallback to legacy string-based schedule hours from objectData
                double wdH  = ParseHours(_objectData.VentilationCoolingWorkdaysHours);
                double satH = ParseHours(_objectData.VentilationCoolingSaturdayHours);
                double sunH = ParseHours(_objectData.VentilationCoolingSundayHours);
                if (wdH <= 0 && satH <= 0 && sunH <= 0)
                    return Fail("Не са въведени работни часове за вентилация охлаждане (нито в График C, нито в legacy полетата).");

                int runH = (int)Math.Round(wdH > 0 ? wdH : (satH > 0 ? satH : sunH));
                runH = Math.Max(1, Math.Min(24, runH));
                ventConfig = new WeeklyScheduleConfig
                {
                    TimeRange      = new DailyTimeRange { StartHour = 0, EndHour = runH - 1 },
                    WorkdaysActive = wdH  > 0,
                    SaturdayActive = satH > 0,
                    SundayActive   = sunH > 0,
                };
            }

            if (!ventConfig.IsValid)
                return Fail($"Невалиден график за вентилация: StartHour={ventConfig.TimeRange.StartHour} EndHour={ventConfig.TimeRange.EndHour}.");

            // ── Days-off ──────────────────────────────────────────────────────────
            var daysOff = new int[12];
            for (int m = 1; m <= 12; m++)
                daysOff[m - 1] = GetOffDaysForMonth(m);

            // ── EI1/EI2 ──────────────────────────────────────────────────────────
            var ei1 = new EnergySourceConfigV2
            {
                Share_Pct       = _data.EnergySource1.Share,
                TotalEfficiency = Math.Max(0.001, _data.EnergySource1.TotalEfficiency),
                Label           = "ЕИ1",
            };
            EnergySourceConfigV2? ei2 = null;
            if (_data.UseSecondEnergySource && _data.EnergySource2 != null)
            {
                ei2 = new EnergySourceConfigV2
                {
                    Share_Pct       = _data.EnergySource2.Share,
                    TotalEfficiency = Math.Max(0.001, _data.EnergySource2.TotalEfficiency),
                    Label           = "ЕИ2",
                };
            }

            // ── Barometric pressure ───────────────────────────────────────────────
            // BG: from climate zone defaults. ASHRAE: per-hour from EPW (provider carries it).
            double bPa = _climateData.GetEffectiveBarometricPressure();

            // ── Recuperation ──────────────────────────────────────────────────────
            double eta_r = Math.Clamp(_data.FirstStageRecuperationEfficiency / 100.0, 0.0, 1.0);

            var input = new VentCoolingInputV2
            {
                AirflowSpec_m3hm2       = _data.CoolingSupplyAirflow,   // [m³/h·m²] – потребителят въвежда специфичен дебит
                CooledArea_m2           = area,
                SupplyTemperature_C     = _data.CoolingSupplyTemperature,
                SupplyRH_Pct            = _data.CoolingRelativeHumidity > 0 ? _data.CoolingRelativeHumidity : 60.0,
                BarometricPressure_Pa   = bPa,
                RecuperationEfficiency  = eta_r,
                // Prefer the Cooling section (Section 12) design temperature when present.
                // This ensures the engine uses the project temperature from Section 12
                // rather than a local Section 14 field.
                ExtractAirTemperature_C = _coolingData != null
                    ? _coolingData.DesignTemperature
                    : (_data.CoolingIndoorTemperature > 0 ? _data.CoolingIndoorTemperature : (double?)null),
                VentSchedule            = ventConfig,
                CoolSchedule            = coolConfig,
                SeasonStart             = seasonStart,
                SeasonEnd               = seasonEnd,
                DaysOffPerMonth         = daysOff,
                EnergySource1           = ei1,
                EnergySource2           = ei2,
            };

            // ── Climate provider + engine dispatch ────────────────────────────────
            // BG: BgAvgClimateProvider (24 typical-day points), isBgAvgMode=true.
            // ASHRAE: EpwClimateProvider (8760 real hours), isBgAvgMode=false.
            bool isAshrae = _data.CoolingClimateDatabase == ClimateDatabase.ASHRAE
                         && _report?.EmbeddedEpwData != null;

            try
            {
                if (isAshrae)
                {
                    var epwProvider = _report!.EmbeddedEpwData!.ToEngineClimateProvider();
                    // For EPW, use provider's average barometric pressure as fallback
                    // (individual hourly values are in each ClimateHourPoint.B_Pa).
                    // Re-create input with EPW average pressure.
                    var epwInput = new VentCoolingInputV2
                    {
                        AirflowSpec_m3hm2       = input.AirflowSpec_m3hm2,
                        CooledArea_m2           = input.CooledArea_m2,
                        SupplyTemperature_C     = input.SupplyTemperature_C,
                        SupplyRH_Pct            = input.SupplyRH_Pct,
                        BarometricPressure_Pa   = epwProvider.BarometricPressure_Pa,
                        RecuperationEfficiency  = input.RecuperationEfficiency,
                        ExtractAirTemperature_C = input.ExtractAirTemperature_C,
                        ExtractAirRH_Pct        = input.ExtractAirRH_Pct,
                        VentSchedule            = input.VentSchedule,
                        CoolSchedule            = input.CoolSchedule,
                        SeasonStart             = input.SeasonStart,
                        SeasonEnd               = input.SeasonEnd,
                        DaysOffPerMonth         = input.DaysOffPerMonth,
                        OfficialHolidays        = input.OfficialHolidays,
                        EnergySource1           = input.EnergySource1,
                        EnergySource2           = input.EnergySource2,
                    };
                    return _engineV2.Calculate(epwInput, epwProvider.GetHourlyData, isBgAvgMode: false, yearRef: yearRef);
                }
                else
                {
                    var bgProvider = new BgAvgClimateProvider(_climateData);
                    return _engineV2.Calculate(input, bgProvider.GetHourlyData, isBgAvgMode: true, yearRef: yearRef);
                }
            }
            catch (Exception ex)
            {
                return Fail($"Грешка при изчислението: {ex.Message}");
            }
        }

        // ── Нощна вентилация (sensible-only free cooling) ─────────────────────────

        /// <summary>
        /// Изгражда <see cref="NightVentInput"/> и изпълнява
        /// <see cref="NightVentilationCalculator.Calculate"/> за текущата секция.
        ///
        /// Входни данни:
        ///   • CooledArea_m2           – от ObjectData.CooledArea
        ///   • NightVentSpecAirflow    – _data.NightVentSpecAirflow  [m³/h·m²]
        ///   • IndoorCoolingSetpoint   – _coolingData.DesignTemperature (Ti от Секция 12)
        ///   • CoolingSeasonMonths     – от ObjectData.CoolingSeasonStart/End
        ///   • ClimateProfiles         – от _climateData (BgAvgClimateProvider)
        ///   • Schedule (Graph D)      – OutdoorAirVentSchedule от CoolingSchedules
        /// </summary>
        private NightVentResult RunNightVentSensible() => RunNightVentCalculatorCore(thermalMass: false);

        private NightVentResult RunNightVentThermalMass() => RunNightVentCalculatorCore(thermalMass: true);

        /// <summary>
        /// Shared core: builds NightVentInput and calls the requested calculator.
        /// thermalMass=false → NightVentilationCalculator.Calculate (sensible-only).
        /// thermalMass=true  → NightVentilationCalculator.CalculateThermalMass (1R1C).
        /// </summary>
        private NightVentResult RunNightVentCalculatorCore(bool thermalMass)
        {
            if (_objectData == null)   return NightVentResult.Fail("Липсват данни за обекта (секция 5).");
            if (_climateData == null)  return NightVentResult.Fail("Липсват климатични данни. Изберете климатична зона.");
            if (!_objectData.CoolingSeasonEnabled) return NightVentResult.Fail("Охладителният сезон не е активиран.");

            // ── Площ ────────────────────────────────────────────────────────────
            if (!double.TryParse(_objectData.CooledArea, NumberStyles.Float, CultureInfo.InvariantCulture, out double area) || area <= 0)
                return NightVentResult.Fail($"Невалидна охлаждаема площ: '{_objectData.CooledArea}'.");

            // ── Ti: вътрешен setpoint (от Секция 12, ако е налична) ──────────────
            double ti = _coolingData != null ? _coolingData.DesignTemperature : 25.0;

            // ── Охладителен сезон: използваме както месеците, така и (ако са налични)
            //     конкретните дни за по-прецизна разбивка (поддържа частични месеци
            //     като напр. 15.08–15.09). Ако Days не са въведени, падаме обратно
            //     на агрегация по цели месеци (legacy поведение).
            int startMonth = _objectData.CoolingSeasonStartMonth ?? 0;
            int endMonth   = _objectData.CoolingSeasonEndMonth   ?? 0;
            if (startMonth == 0 || endMonth == 0)
                return NightVentResult.Fail("Не са въведени дати на охладителния сезон.");

            var seasonMonths = new List<int>();
            if (startMonth <= endMonth)
            {
                for (int m = startMonth; m <= endMonth; m++) seasonMonths.Add(m);
            }
            else
            {
                // Wrap-around (e.g. Oct–Mar)
                for (int m = startMonth; m <= 12; m++) seasonMonths.Add(m);
                for (int m = 1; m <= endMonth; m++)   seasonMonths.Add(m);
            }

            if (seasonMonths.Count == 0)
                return NightVentResult.Fail("Охладителният сезон не съдържа нито един месец.");

            // Start/end DAYS are mandatory — if missing, refuse to calculate (avoid whole-month fallback).
            if (!_objectData.CoolingSeasonStartDay.HasValue || !_objectData.CoolingSeasonEndDay.HasValue)
                return NightVentResult.Fail("Не са въведени начален/краен ден на охладителния сезон (Секция 5).");

            // Compute exact counts of weekdays/saturdays/sundays per month inside the
            // exact season interval [StartDay.StartMonth … EndDay.EndMonth].
            IReadOnlyDictionary<int, (int Weekdays, int Saturdays, int Sundays)>? dayTypeCountsPerMonth = null;
            int yearRef = 2024; // match other calendar calculations
            {
                int sd = _objectData.CoolingSeasonStartDay.Value;
                int ed = _objectData.CoolingSeasonEndDay.Value;
                // Clamp days to valid range for their month
                sd = Math.Max(1, Math.Min(sd, DateTime.DaysInMonth(yearRef, startMonth)));
                ed = Math.Max(1, Math.Min(ed, DateTime.DaysInMonth(yearRef, endMonth)));

                var seasonStart = new DateTime(yearRef, startMonth, sd);
                var seasonEnd = new DateTime(yearRef, endMonth, ed);
                if (seasonEnd < seasonStart) seasonEnd = seasonEnd.AddYears(1);

                var counts = new Dictionary<int, (int Weekdays, int Saturdays, int Sundays)>();
                // initialize months in season with zero
                foreach (var m in seasonMonths) counts[m] = (0, 0, 0);

                for (var d = seasonStart.Date; d <= seasonEnd.Date; d = d.AddDays(1))
                {
                    int m = d.Month; // month in 1..12
                    if (!counts.ContainsKey(m))
                    {
                        // In case season spans into next year and includes months not in seasonMonths
                        counts[m] = (0, 0, 0);
                    }

                    var tuple = counts[m];
                    switch (d.DayOfWeek)
                    {
                        case DayOfWeek.Saturday:
                            tuple.Saturdays++;
                            break;
                        case DayOfWeek.Sunday:
                            tuple.Sundays++;
                            break;
                        default:
                            tuple.Weekdays++;
                            break;
                    }
                    counts[m] = tuple;
                }

                dayTypeCountsPerMonth = counts;
            }

            // ── Климатични профили (само температура, без RH) ────────────────────
            var provider  = new BgAvgClimateProvider(_climateData);
            var profiles  = new Dictionary<int, ClimateHourlyProfile>();

            foreach (int m in seasonMonths)
            {
                try
                {
                    var pts = provider.GetHourlyData(m);
                    var temps = new double[24];
                    foreach (var pt in pts)
                    {
                        if (pt.Hour >= 0 && pt.Hour < 24)
                            temps[pt.Hour] = pt.T_out_C;
                    }
                    profiles[m] = new ClimateHourlyProfile(temps);
                }
                catch
                {
                    return NightVentResult.Fail($"Грешка при зареждане на климатичен профил за месец {m}.");
                }
            }

            // ── График D – „График за вентилиране с външен въздух" ──────────────
            var schedD = _objectData.CoolingSchedules?.OutdoorAirVentSchedule;
            bool[] wdActive, satActive, sunActive;

            if (schedD != null)
            {
                wdActive  = NightVentilationCalculator.BuildActiveHoursFromTimeRange(
                    schedD.Workdays.StartTime, schedD.Workdays.EndTime);
                satActive = NightVentilationCalculator.BuildActiveHoursFromTimeRange(
                    schedD.Saturday.StartTime, schedD.Saturday.EndTime);
                sunActive = NightVentilationCalculator.BuildActiveHoursFromTimeRange(
                    schedD.Sunday.StartTime,   schedD.Sunday.EndTime);
            }
            else
            {
                // График D не е попълнен → нощна вентилация = 0
                wdActive  = new bool[24];
                satActive = new bool[24];
                sunActive = new bool[24];
            }

            var nightSched = new NightVentSchedule(wdActive, satActive, sunActive);

            // ── c_eff за thermal-mass режим ──────────────────────────────────────
            double cEffWhPerM2K = (thermalMass && _coolingData != null)
                ? _coolingData.SpecificHeatCapacityWhPerM2K
                : 0.0;

            // ── Входен DTO ──────────────────────────────────────────────────────
            var nightInput = new NightVentInput(
                AreaM2:                      area,
                SpecAirflowM3phM2:           _data.NightVentSpecAirflow,
                IndoorCoolingSetpointC:      ti,
                CoolingSeasonMonths:         seasonMonths,
                ClimateProfiles:             profiles,
                Schedule:                    nightSched,
                DayTypeCountsPerMonth:       dayTypeCountsPerMonth,
                SpecificHeatCapacityWhPerM2K: cEffWhPerM2K
            );

            try
            {
                return thermalMass
                    ? NightVentilationCalculator.CalculateThermalMass(nightInput, collectDebug: true)
                    : NightVentilationCalculator.Calculate(nightInput, collectDebug: true);
            }
            catch (Exception ex)
            {
                return NightVentResult.Fail($"Грешка при изчислението: {ex.Message}");
            }
        }

        /// <summary>
        /// Изгражда debug текст за нощната вентилация.
        /// Показва активния резултат и (ако ThermalMass е активен) и sensible за сравнение.
        /// </summary>
        private string BuildNightVentDebugText(NightVentResult r, NightVentResult sensibleR, bool useThermalMass)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("════════════════════════════════════════════════════════");
            sb.AppendLine("  ПРИНОС ОТ НОЩНА ВЕНТИЛАЦИЯ С НЕобработен въздух");
            if (useThermalMass)
                sb.AppendLine("  Режим: С ТОПЛИНЕН КАПАЦИТЕТ (1R1C динамичен модел)");
            else
                sb.AppendLine("  Режим: Sensible-only (ρ·ca = 0.34 Wh/m³K)");
            sb.AppendLine("════════════════════════════════════════════════════════");

            // Входни параметри
            sb.AppendLine($"  Климатична база : DefaultParams_climateZones_hourly_flat_1to9.json");
            sb.AppendLine($"  Климатична зона : {ClimateZoneDisplay}");
            if (_objectData != null &&
                _objectData.CoolingSeasonStartDay.HasValue && _objectData.CoolingSeasonStartMonth.HasValue &&
                _objectData.CoolingSeasonEndDay.HasValue   && _objectData.CoolingSeasonEndMonth.HasValue)
            {
                sb.AppendLine($"  Охладителен сезон (Сек.5)   = " +
                              $"{_objectData.CoolingSeasonStartDay:00}.{_objectData.CoolingSeasonStartMonth:00}" +
                              $" – {_objectData.CoolingSeasonEndDay:00}.{_objectData.CoolingSeasonEndMonth:00}");
            }
            double ti = _coolingData?.DesignTemperature ?? 25.0;
            sb.AppendLine($"  Ti (вътр. setpoint, Сек.12)  = {ti:F2} °C");
            sb.AppendLine($"  VdotSpecNight                = {_data.NightVentSpecAirflow:F4} m³/h·m²");

            if (useThermalMass && _coolingData != null)
            {
                double cEff   = _coolingData.SpecificHeatCapacityWhPerM2K;
                double area2  = CooledArea_m2;
                double vdot2  = _data.NightVentSpecAirflow * area2;
                double hnv    = 0.34 * vdot2;
                double cEffWK = cEff * area2;
                sb.AppendLine($"  cEff (Wh/m²K, Сек.12)        = {cEff:F2}");
                sb.AppendLine($"  A                             = {area2:F2} m²");
                sb.AppendLine($"  Hnv = 0.34 × VdotNight        = {hnv:F2} W/K");
                sb.AppendLine($"  Ceff_WhPerK = cEff × A        = {cEffWK:F2} Wh/K");
                if (cEffWK > 0)
                {
                    double _a = hnv * 3600.0 / (cEffWK * 3600.0);
                    sb.AppendLine($"  a = Hnv×3600 / Ceff_J         = {_a:F6}");
                }
            }

            if (!r.IsValid)
            {
                sb.AppendLine($"  ГРЕШКА: {r.ErrorMessage}");
                sb.AppendLine("════════════════════════════════════════════════════════");
                return sb.ToString();
            }

            double vdotNight = r.DebugDetails?.VdotNight_m3ph ?? (_data.NightVentSpecAirflow * CooledArea_m2);
            sb.AppendLine($"  VdotNight (A×spec)           = {vdotNight:F2} m³/h  (A={CooledArea_m2:F1} m²)");
            sb.AppendLine();

            if (r.DebugDetails?.PerMonth != null)
            {
                foreach (var md in r.DebugDetails.PerMonth)
                {
                    string monthName = System.Globalization.CultureInfo.InvariantCulture
                        .DateTimeFormat.GetMonthName(md.Month);

                    sb.AppendLine($"  ── Месец {md.Month,2} ({monthName}) ──────────────────────────");
                    sb.AppendLine($"  Дни: Weekdays={md.Weekdays} Saturdays={md.Saturdays} Sundays={md.Sundays}");
                    sb.AppendLine();

                    if (useThermalMass)
                    {
                        sb.AppendLine($"  {"Час",4} {"Te°C",7} {"ActWd",5} {"TiSt_Wd",9} {"TiEn_Wd",9} {"E_Wd kWh",10} {"ActSat",6} {"TiSt_St",9} {"E_Sat kWh",10} {"ActSun",6} {"TiSt_Su",9} {"E_Sun kWh",10}");
                        foreach (var h in md.Hours)
                        {
                            if (h.ActiveWeekday || h.ActiveSaturday || h.ActiveSunday)
                            {
                                sb.AppendLine(
                                    $"  {h.Hour,4} {h.Te,7:F1} {(h.ActiveWeekday  ? "Y" : "-"),5}" +
                                    $" {h.TiStartWeekday,9:F2} {h.TiEndWeekday,9:F2} {h.EHourWeekday_kWh,10:F6}" +
                                    $" {(h.ActiveSaturday ? "Y" : "-"),6} {h.TiStartSaturday,9:F2} {h.EHourSaturday_kWh,10:F6}" +
                                    $" {(h.ActiveSunday   ? "Y" : "-"),6} {h.TiStartSunday,9:F2} {h.EHourSunday_kWh,10:F6}");
                            }
                        }
                    }
                    else
                    {
                        sb.AppendLine($"  {"Час",4} {"Te°C",7} {"ΔT",6} {"ActWd",5} {"ActSat",6} {"ActSun",6} {"E_Wd kWh",10} {"E_Sat kWh",10} {"E_Sun kWh",10}");
                        foreach (var h in md.Hours)
                        {
                            if (h.DT > 0 || h.ActiveWeekday || h.ActiveSaturday || h.ActiveSunday)
                            {
                                sb.AppendLine($"  {h.Hour,4} {h.Te,7:F1} {h.DT,6:F2} {(h.ActiveWeekday  ? "Y" : "-"),5} {(h.ActiveSaturday ? "Y" : "-"),6} {(h.ActiveSunday   ? "Y" : "-"),6}" +
                                              $" {h.EHourWeekday_kWh,10:F6} {h.EHourSaturday_kWh,10:F6} {h.EHourSunday_kWh,10:F6}");
                            }
                        }
                    }

                    sb.AppendLine();
                    sb.AppendLine($"  ΣWd={md.SumWeekday_kWh:F4}  ΣSat={md.SumSaturday_kWh:F4}  ΣSun={md.SumSunday_kWh:F4}");
                    sb.AppendLine($"  monthEnergy = {md.SumWeekday_kWh:F4}×{md.Weekdays} + {md.SumSaturday_kWh:F4}×{md.Saturdays} + {md.SumSunday_kWh:F4}×{md.Sundays}");
                    sb.AppendLine($"             = {md.MonthEnergy_kWh:F3} kWh");
                    sb.AppendLine();
                }
            }

            sb.AppendLine($"  seasonEnergy              = {r.TotalKWh:F3} kWh");
            sb.AppendLine($"  seasonSpecific            = {r.SpecificKWhPerM2:F4} kWh/m²");

            // Если ThermalMass, показваме и sensible за сравнение
            if (useThermalMass && sensibleR.IsValid)
            {
                sb.AppendLine();
                sb.AppendLine($"  ── Sensible-only (за сравнение) ───────────────────────");
                sb.AppendLine($"  seasonEnergy (sensible)   = {sensibleR.TotalKWh:F3} kWh");
                sb.AppendLine($"  seasonSpecific (sensible) = {sensibleR.SpecificKWhPerM2:F4} kWh/m²");
                if (r.TotalKWh > 0 && sensibleR.TotalKWh > 0)
                    sb.AppendLine($"  ThermalMass / Sensible    = {r.TotalKWh / sensibleR.TotalKWh:F3}×");
            }

            sb.AppendLine("════════════════════════════════════════════════════════");
            return sb.ToString();
        }

        /// <summary>
        /// Изгражда VentCoolingContributionInput и изпълнява новата формула за
        /// „Принос към охлаждането от вентилация (нетен)".
        ///
        /// Входни данни:
        ///   • Airflow_m3ph_per_m2   – _data.CoolingSupplyAirflow    (Специфичен дебит)
        ///   • SupplyAirTemp_C       – _data.CoolingSupplyTemperature (Темп. на подавания въздух)
        ///   • TotalWorkHoursSeason  – _data.TotalWorkHoursSeason     (готово поле от UI – НЕ преизчислява)
        ///   • RoomTemp_Design_C     – _coolingData.DesignTemperature (Проектна температура, Секция 12)
        ///   • RoomTemp_Raised_C     – _coolingData.ReductionTemperature (Температура с повишение, Секция 12)
        ///   • CoolingArea_m2        – от ObjectData.CooledArea
        ///   • CoolingSchedule       – График B (от CoolingSchedules)
        ///   • VentCoolingSchedule   – График C (от CoolingSchedules)
        /// </summary>
        private VentCoolingContributionResult RunContribCalculator()
        {
            static VentCoolingContributionResult Fail(string msg) =>
                new VentCoolingContributionResult { IsValid = false, ErrorMessage = msg };

            if (_objectData == null)  return Fail("Липсват данни за обекта (секция 5).");
            if (_coolingData == null) return Fail("Липсват данни за охлаждане (секция 12).");

            // Площ
            if (!double.TryParse(_objectData.CooledArea, NumberStyles.Float, CultureInfo.InvariantCulture, out double area) || area < 0.0)
                area = 0.0;

            var contribInput = new VentCoolingContributionInput
            {
                Airflow_m3ph_per_m2  = _data.CoolingSupplyAirflow,
                SupplyAirTemp_C      = _data.CoolingSupplyTemperature,
                // WorkHoursSeason е вече изчисленото readonly property от секция 14 UI
                // ("Общ брой работни часове за сезона") – НЕ се взима от _data.TotalWorkHoursSeason (=0)
                TotalWorkHoursSeason = WorkHoursSeason,
                RoomTemp_Design_C    = _coolingData.DesignTemperature,
                RoomTemp_Raised_C    = _coolingData.ReductionTemperature,
                CoolingArea_m2       = area,
                CoolingSchedule      = _objectData.CoolingSchedules?.CoolingSchedule,
                VentCoolingSchedule  = _objectData.CoolingSchedules?.VentilationCoolingSchedule,
            };

            return VentCoolingContributionCalculator.Calculate(contribInput);
        }

        /// <summary>
        /// Конвертира WeeklySchedule (TimeSpan) → WeeklyScheduleConfig (int hours).
        /// </summary>
        private static WeeklyScheduleConfig BuildWeeklyConfig(WeeklySchedule sched)
        {
            // Determine dominant time range (workdays preferred)
            var range = sched.Workdays.GetHours() > 0 ? sched.Workdays
                      : sched.Saturday.GetHours() > 0 ? sched.Saturday
                      : sched.Sunday;

            // StartHour: integral part of StartTime
            int startH = (int)range.StartTime.TotalHours;
            startH = Math.Clamp(startH, 0, 23);

            // RunHours: duration of occupancy (rounded to nearest integer, min 1)
            int runH = Math.Max(1, (int)Math.Round(range.GetHours()));

            // EndHour (inclusive): last active hour of the day
            int endH = Math.Min(23, startH + runH - 1);

            return new WeeklyScheduleConfig
            {
                TimeRange      = new DailyTimeRange { StartHour = startH, EndHour = endH },
                WorkdaysActive = sched.Workdays.GetHours() > 0,
                SaturdayActive = sched.Saturday.GetHours() > 0,
                SundayActive   = sched.Sunday.GetHours()   > 0,
            };
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

        /// <summary>
        /// Subscribe to nested CoolingSchedules property changes
        /// </summary>
        private void SubscribeToCoolingSchedules(CoolingSchedulesModel schedules)
        {
            schedules.PropertyChanged += OnCoolingSchedulesChanged;

            // Helper: subscribe to all three time-range objects inside a WeeklySchedule
            static void SubscribeWeekly(WeeklySchedule? ws, PropertyChangedEventHandler schedHandler, PropertyChangedEventHandler rangeHandler)
            {
                if (ws == null) return;
                ws.PropertyChanged += schedHandler;
                if (ws.Workdays  != null) ws.Workdays.PropertyChanged  += rangeHandler;
                if (ws.Saturday  != null) ws.Saturday.PropertyChanged  += rangeHandler;
                if (ws.Sunday    != null) ws.Sunday.PropertyChanged    += rangeHandler;
            }

            // C) График за вентилация охлаждане  (VentSchedule for engine)
            SubscribeWeekly(schedules.VentilationCoolingSchedule, OnScheduleChanged, OnTimeRangeChanged);

            // B) График за охлаждане  (CoolSchedule for engine – overlap fraction)
            SubscribeWeekly(schedules.CoolingSchedule, OnScheduleChanged, OnTimeRangeChanged);
        }

        private void OnCoolingSchedulesChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HoursPerWeek));
            OnPropertyChanged(nameof(WorkDaysSeason));
            OnPropertyChanged(nameof(WorkHoursSeason));
            Recalculate();
        }

        private void OnScheduleChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HoursPerWeek));
            OnPropertyChanged(nameof(WorkDaysSeason));
            OnPropertyChanged(nameof(WorkHoursSeason));
            Recalculate();
        }

        private void OnTimeRangeChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HoursPerWeek));
            OnPropertyChanged(nameof(WorkDaysSeason));
            OnPropertyChanged(nameof(WorkHoursSeason));
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
                e.PropertyName == nameof(ObjectDataSectionData.CoolingSchedules) ||
                e.PropertyName?.StartsWith("DaysOff", StringComparison.OrdinalIgnoreCase) == true)
            {
                if (e.PropertyName == nameof(ObjectDataSectionData.CoolingSeasonEnabled))
                {
                    OnPropertyChanged(nameof(IsCoolingSeasonEnabled));
                    OnPropertyChanged(nameof(CoolingSeasonWarning));
                }

                // Update computed properties for new methodology
                if (e.PropertyName == nameof(ObjectDataSectionData.CoolingSchedules) ||
                    e.PropertyName?.StartsWith("DaysOff", StringComparison.OrdinalIgnoreCase) == true ||
                    e.PropertyName == nameof(ObjectDataSectionData.CoolingSeasonStartMonth) ||
                    e.PropertyName == nameof(ObjectDataSectionData.CoolingSeasonEndMonth))
                {
                    OnPropertyChanged(nameof(HoursPerWeek));
                    OnPropertyChanged(nameof(WorkDaysSeason));
                    OnPropertyChanged(nameof(WorkHoursSeason));
                }

                Recalculate();
            }
        }

        private void OnCoolingDataPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CoolingSectionData.DesignTemperature) ||
                e.PropertyName == nameof(CoolingSectionData.ReductionTemperature) ||
                e.PropertyName == nameof(CoolingSectionData.SpecificHeatCapacityWhPerM2K))
            {
                Recalculate();
            }
        }

        private string BuildDebugText(VentCoolingOutputV2 v2)
        {
            // Show zone setpoint (used as extract/indoor temperature) and humidity info
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("  Zone setpoint used for calculations:");
            if (_data.CoolingIndoorTemperature > 0.0)
                sb.AppendLine($"    T_zone (CoolingIndoorTemperature) : {_data.CoolingIndoorTemperature,6:F1} °C");
            else
                sb.AppendLine("    T_zone (CoolingIndoorTemperature) : (not set) -> engine will use fallback behavior");

            // There is no separate 'zone RH' field; show the supply RH and note what the engine will use for extract RH
            if (_data.CoolingRelativeHumidity > 0.0)
                sb.AppendLine($"    RH_supply (CoolingRelativeHumidity) : {_data.CoolingRelativeHumidity,6:F1} %");
            else
                sb.AppendLine("    RH_supply (CoolingRelativeHumidity) : (not set)");

            sb.AppendLine("    Note: engine uses ExtractAirTemperature if set (T_zone) and ExtractAirRH if provided; otherwise RH_extract defaults to 50%.");
            sb.AppendLine();

            // Then append the detailed Engine V2 debug block
            AppendV2Debug(sb, v2);

            // Append new contribution calculation debug
            AppendContribDebug(sb, _contribResult, WorkHoursSeason, _data.CoolingSupplyAirflow, _data.CoolingSupplyTemperature,
                _coolingData?.DesignTemperature, _coolingData?.ReductionTemperature);

            // Append night ventilation debug
            sb.Append(BuildNightVentDebugText(_nightVentResult, _nightVentSensibleResult, _data.NightVentUseThermalMass));

            return sb.ToString();
        }

        private static void AppendContribDebug(StringBuilder sb, VentCoolingContributionResult r,
            double ventSeasonHours, double airflow, double supplyTemp,
            double? tRoomDesign = null, double? tRoomRaised = null)
        {
            sb.AppendLine();
            sb.AppendLine("════════════════════════════════════════════════════════");
            sb.AppendLine("  ПРИНОС КЪМ ОХЛАЖДАНЕТО ОТ ВЕНТИЛАЦИЯ (нов метод)");
            sb.AppendLine("════════════════════════════════════════════════════════");

            if (!r.IsValid)
            {
                sb.AppendLine($"  ГРЕШКА: {r.ErrorMessage}");
                return;
            }

            // Входни параметри (за верификация)
            sb.AppendLine($"  Входни параметри:");
            sb.AppendLine($"    q (дебит)           = {airflow,10:F4}  m³/h·m²");
            sb.AppendLine($"    T_supply            = {supplyTemp,10:F2}  °C");
            sb.AppendLine($"    H_season            = {ventSeasonHours,10:F2}  h  (WorkHoursSeason, 2026)");
            sb.AppendLine($"    T_room_design       = {(tRoomDesign.HasValue ? $"{tRoomDesign.Value,6:F2} °C  (Секция 12 – Проектна температура)" : "?  (липсва Секция 12)")}");
            sb.AppendLine($"    T_room_raised       = {(tRoomRaised.HasValue ? $"{tRoomRaised.Value,6:F2} °C  (Секция 12 – Температура с повишение)" : "?  (липсва Секция 12)")}");
            sb.AppendLine();
            sb.AppendLine($"  Формула: BaseFactor = 0.34 × q × H / 1000");
            sb.AppendLine($"           = 0.34 × {airflow:F4} × {ventSeasonHours:F2} / 1000 = {r.BaseFactor:F6}");
            if (tRoomDesign.HasValue)
                sb.AppendLine($"  ScenarioDesign = {r.BaseFactor:F6} × ({tRoomDesign.Value:F2} - {supplyTemp:F2}) = {r.ScenarioDesign_kWhm2:F3}");
            if (tRoomRaised.HasValue)
                sb.AppendLine($"  ScenarioRaised = {r.BaseFactor:F6} × ({tRoomRaised.Value:F2} - {supplyTemp:F2}) = {r.ScenarioRaised_kWhm2:F3}");
            sb.AppendLine();
            sb.AppendLine($"  ScenarioDesign      = {r.ScenarioDesign_kWhm2,10:F3}  kWh/m²");
            sb.AppendLine($"  ScenarioRaised      = {r.ScenarioRaised_kWhm2,10:F3}  kWh/m²");
            sb.AppendLine($"  Min / Max           = {r.Min_kWhm2,10:F3} / {r.Max_kWhm2:F3}  kWh/m²");
            sb.AppendLine($"  f_on                = {r.F_on,10:F4}");
            sb.AppendLine($"  Net = {r.F_on:F4}×{r.ScenarioDesign_kWhm2:F3} + {(1-r.F_on):F4}×{r.ScenarioRaised_kWhm2:F3}");
            sb.AppendLine($"  Net (нетен принос)  = {r.Net_kWhm2,10:F3}  kWh/m²");
            sb.AppendLine($"  Net (абсолютно)     = {r.Net_kWh,10:F2}  kWh");

            if (r.Warnings.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("  Предупреждения:");
                foreach (var w in r.Warnings)
                    sb.AppendLine($"    ⚠ {w}");
            }

            sb.AppendLine("════════════════════════════════════════════════════════");
        }

        private static void AppendV2Debug(StringBuilder sb, VentCoolingOutputV2 v2)
        {
            sb.AppendLine();
            sb.AppendLine("════════════════════════════════════════════════════════");
            sb.AppendLine("  ENGINE V2  (Наредба 7257_1 §3.14, психрометрия)");
            sb.AppendLine("════════════════════════════════════════════════════════");

            if (!v2.IsValid)
            {
                sb.AppendLine($"  ГРЕШКА: {v2.ErrorMessage}");
                return;
            }

            // ── Input snapshot ────────────────────────────────────────────────────
            if (!string.IsNullOrEmpty(v2.DebugInputSummary))
            {
                sb.AppendLine("  Входни параметри:");
                foreach (var line in v2.DebugInputSummary.Split('\n'))
                    if (!string.IsNullOrWhiteSpace(line))
                        sb.AppendLine($"    {line.Trim()}");
                sb.AppendLine();
            }

            sb.AppendLine($"  Работни дни сезон : {v2.TotalWorkingDays:F1}  |  Работни часове: {v2.TotalWorkingHours:F1}");
            sb.AppendLine();
            sb.AppendLine("  Резултати [kWh/m²]:");
            sb.AppendLine($"    Охлаждане нетна        : {v2.TotalCoolNet_kWhm2,8:F3}");
            sb.AppendLine($"    Загряване              : {v2.TotalHeatNet_kWhm2,8:F3}");
            sb.AppendLine($"    Изсушаване (латентна)  : {v2.TotalDryNet_kWhm2,8:F3}");
            sb.AppendLine($"    Принос вент. охлаждане : {v2.TotalVentContrib_kWhm2,8:F3}");
            sb.AppendLine($"    Обща нетна             : {v2.TotalNetEnergy_kWhm2,8:F3}");
            sb.AppendLine();
            sb.AppendLine("  Потребна доставена [kWh/m²]:");
            sb.AppendLine($"    ЕИ 1                   : {v2.FinalEnergyEI1_kWhm2,8:F3}");
            sb.AppendLine($"    ЕИ 2                   : {v2.FinalEnergyEI2_kWhm2,8:F3}");
            sb.AppendLine($"    Обща                   : {v2.TotalFinalEnergy_kWhm2,8:F3}");
            sb.AppendLine();

            if (v2.MonthlyResults.Count > 0)
            {
                sb.AppendLine("  Месечна разбивка:");
                sb.AppendLine($"  {"Месец",-12} {"Дни",5} {"Часове",7} {"h_out",7} {"h_sup",7} {"Охл.",8} {"Заг.",8} {"Изс.",8} {"Принос",8}");
                foreach (var mr in v2.MonthlyResults)
                {
                    sb.AppendLine($"  {mr.MonthName,-12} {mr.WorkingDays,5:F1} {mr.WorkingHours,7:F1}" +
                                  $" {mr.Avg_h_out_kJkg,7:F1} {mr.Avg_h_sup_kJkg,7:F1}" +
                                  $" {mr.E_cool_net_kWhm2,8:F3} {mr.E_heat_net_kWhm2,8:F3}" +
                                  $" {mr.E_dry_net_kWhm2,8:F3} {mr.E_vent_contrib_net_kWhm2,8:F3}");
                }

                // ── Hourly debug table (first month that has rows) ─────────────────
                var firstWithRows = v2.MonthlyResults.FirstOrDefault(mr => mr.HourlyDebugRows.Count > 0);
                if (firstWithRows != null)
                {
                    sb.AppendLine();
                    sb.AppendLine($"  Debug таблица – {firstWithRows.MonthName} (типичен ден):");
                    sb.AppendLine($"  {"Час",4} {"Run",4} {"T_out",7} {"RH_out",7} {"x_out",8} {"rho_da",8} {"h_out",7} {"x_sup",8} {"h_sup",7} {"rhoh_out",10} {"rhoh_sup",10} {"Δrhoh",8} {"Охл.",10} {"Заг.",10} {"Изс.",10} {"Охл.мес",10} {"Заг.мес",10} {"Изс.мес",10} {"Дни",5}");
                    foreach (var r in firstWithRows.HourlyDebugRows)
                    {
                        sb.AppendLine($"  {r.Hour,4} {r.Run,4} {r.T_out_C,7:F1} {r.RH_out_Pct,7:F1}" +
                                      $" {r.x_out,8:F5} {r.rho_da_out,8:F5} {r.h_out,7:F2}" +
                                      $" {r.x_sup,8:F5} {r.h_sup,7:F2}" +
                                      $" {r.rhoh_out,10:F5} {r.rhoh_sup,10:F5} {r.delta_h,8:F3}" +
                                      $" {r.E_cool_hour,10:F6} {r.E_heat_hour,10:F6} {r.E_dry_hour,10:F6}" +
                                      $" {r.E_cool_month_kWhm2,10:F6} {r.E_heat_month_kWhm2,10:F6} {r.E_dry_month_kWhm2,10:F6} {r.Workdays,5}");
                    }
                }
            }

            if (v2.Warnings.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("  Предупреждения:");
                foreach (var w in v2.Warnings)
                    sb.AppendLine($"    ⚠ {w}");
            }

            sb.AppendLine("════════════════════════════════════════════════════════");
        }

        // ========== NEW METHODOLOGY: COOLING PARAMETERS + k_m LOGIC ==========

        /// <summary>
        /// Избор на климатична база данни за охлаждане.
        /// </summary>
        public ClimateDatabase CoolingClimateDatabase
        {
            get => _data.CoolingClimateDatabase;
            set
            {
                if (_data.CoolingClimateDatabase != value)
                {
                    _data.CoolingClimateDatabase = value;
                    OnPropertyChanged(nameof(CoolingClimateDatabase));
                    OnPropertyChanged(nameof(EpwFileDisplayName));
                    OnPropertyChanged(nameof(EpwFileDisplayColor));
                    OnPropertyChanged(nameof(EpwLocationInfo));
                    // Re-calculate when switching between BG and ASHRAE
                    Recalculate();
                }
            }
        }

        /// <summary>
        /// Име на EPW файл за показване в UI.
        /// </summary>
        public string EpwFileDisplayName
        {
            get
            {
                if (CoolingClimateDatabase != ClimateDatabase.ASHRAE)
                    return string.Empty;

                if (_report?.EmbeddedEpwData != null)
                    return _report.EmbeddedEpwData.OriginalFileName;

                return "(не е избран EPW файл)";
            }
        }

        /// <summary>
        /// Цвят на текста за EPW файл (червен ако липсва, нормален ако има).
        /// </summary>
        public string EpwFileDisplayColor
        {
            get
            {
                if (CoolingClimateDatabase != ClimateDatabase.ASHRAE)
                    return "#000000";

                return _report?.EmbeddedEpwData != null ? "#000000" : "#CC0000";
            }
        }

        /// <summary>
        /// Информация за местоположението от EPW файл.
        /// </summary>
        public string? EpwLocationInfo
        {
            get
            {
                if (CoolingClimateDatabase != ClimateDatabase.ASHRAE)
                    return null;

                return _report?.EmbeddedEpwData?.GetDisplayName();
            }
        }

        /// <summary>
        /// Command за зареждане на EPW файл.
        /// </summary>
        public ICommand LoadEpwFileCommand => new RelayCommand(_ => LoadEpwFile());

        private void LoadEpwFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "EPW файлове (*.epw)|*.epw|Всички файлове (*.*)|*.*",
                Title = "Изберете EPW файл"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var parser = new EpwParser();
                    var result = parser.ParseFile(dialog.FileName);

                    if (!result.Success)
                    {
                        System.Windows.MessageBox.Show(
                            $"Грешка при парсване на EPW файл:\n\n{result.ErrorMessage}",
                            "Грешка",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Error);
                        return;
                    }

                    // Вграждаме данните в Report
                    if (_report != null)
                    {
                        _report.EmbeddedEpwData = result.ToEmbeddedData();
                        _report.ModifiedDate = DateTime.Now;
                    }

                    // Обновяваме UI
                    OnPropertyChanged(nameof(EpwFileDisplayName));
                    OnPropertyChanged(nameof(EpwFileDisplayColor));
                    OnPropertyChanged(nameof(EpwLocationInfo));

                    System.Windows.MessageBox.Show(
                        $"EPW файлът е зареден успешно!\n\n{result.GetDisplayName()}\n\n8760 часови записа",
                        "Успех",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);

                    // Re-calculate with new EPW data
                    Recalculate();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(
                        $"Неочаквана грешка:\n\n{ex.Message}",
                        "Грешка",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Работни часове на седмица (readonly) - изчислено от графици за охлаждане.
        /// </summary>
        public double HoursPerWeek
        {
            get
            {
                if (_objectData?.CoolingSchedules == null) return 0.0;
                return _objectData.CoolingSchedules.VentilationCoolingSchedule?.GetHoursPerWeek() ?? 0.0;
            }
        }

        /// <summary>
        /// Дебит на приточен въздух [m³/h] за охлаждане.
        /// </summary>
        public double CoolingSupplyAirflow
        {
            get => _data.CoolingSupplyAirflow;
            set
            {
                var clamped = Math.Max(0.0, value);
                if (Math.Abs(_data.CoolingSupplyAirflow - clamped) > 0.0001)
                {
                    _data.CoolingSupplyAirflow = clamped;
                    OnPropertyChanged(nameof(CoolingSupplyAirflow));
                    Recalculate();
                }
            }
        }

        /// <summary>
        /// Дебит на отточен въздух [m³/h] за охлаждане.
        /// </summary>
        public double CoolingExhaustAirflow
        {
            get => _data.CoolingExhaustAirflow;
            set
            {
                var clamped = Math.Max(0.0, value);
                if (Math.Abs(_data.CoolingExhaustAirflow - clamped) > 0.0001)
                {
                    _data.CoolingExhaustAirflow = clamped;
                    OnPropertyChanged(nameof(CoolingExhaustAirflow));
                    // exhaust airflow не влияе пряко на contrib формулата
                }
            }
        }

        /// <summary>
        /// Температура на подавания въздух при охлаждане [°C].
        /// </summary>
        public double CoolingSupplyTemperature
        {
            get => _data.CoolingSupplyTemperature;
            set
            {
                if (Math.Abs(_data.CoolingSupplyTemperature - value) > 0.0001)
                {
                    _data.CoolingSupplyTemperature = value;
                    OnPropertyChanged(nameof(CoolingSupplyTemperature));
                    Recalculate();
                }
            }
        }

        /// <summary>
        /// Температура на вътрешния въздух при охлаждане [°C].
        /// </summary>
        public double CoolingIndoorTemperature
        {
            get => _data.CoolingIndoorTemperature;
            set
            {
                if (Math.Abs(_data.CoolingIndoorTemperature - value) > 0.0001)
                {
                    _data.CoolingIndoorTemperature = value;
                    OnPropertyChanged(nameof(CoolingIndoorTemperature));
                    // TODO: Recalculate
                }
            }
        }

        /// <summary>
        /// Относителна влажност на подавания въздух при охлаждане [%] (0-100).
        /// </summary>
        public double CoolingRelativeHumidity
        {
            get => _data.CoolingRelativeHumidity;
            set
            {
                var clamped = Math.Clamp(value, 0.0, 100.0);
                if (Math.Abs(_data.CoolingRelativeHumidity - clamped) > 0.0001)
                {
                    _data.CoolingRelativeHumidity = clamped;
                    OnPropertyChanged(nameof(CoolingRelativeHumidity));
                    // TODO: Recalculate
                }
            }
        }

        /// <summary>
        /// Работни дни за сезона — взима се директно от engine output (TotalWorkingDays),
        /// защото engine-ът брои точно дните в [SeasonStart..SeasonEnd] с правилните дати,
        /// докато старото CalculateSeasonalWorkDays() броеше цели месеци (без начален/краен ден).
        /// </summary>
        public double WorkDaysSeason
        {
            get
            {
                if (_objectData == null || !_objectData.CoolingSeasonEnabled) return 0.0;
                // Engine output-ът вече е изчислен с точните дати — ползваме го директно.
                if (_outputV2 != null && _outputV2.TotalWorkingDays > 0)
                    return _outputV2.TotalWorkingDays;
                return 0.0;
            }
        }

        /// <summary>
        /// Работни часове за сезона — взима се директно от engine output (TotalWorkingHours),
        /// защото engine-ът брои точно активните часове в [SeasonStart..SeasonEnd],
        /// докато старото CalculateSeasonalWorkHours() броеше цели месеци (без начален/краен ден).
        /// </summary>
        public double WorkHoursSeason
        {
            get
            {
                if (_objectData == null || !_objectData.CoolingSeasonEnabled) return 0.0;
                // Engine output-ът вече е изчислен с точните дати — ползваме го директно.
                if (_outputV2 != null && _outputV2.TotalWorkingHours > 0)
                    return _outputV2.TotalWorkingHours;
                return 0.0;
            }
        }

        /// <summary>
        /// Изчислява общ брой работни дни за сезона с k_m корекция.
        /// </summary>
        private double CalculateSeasonalWorkDays(double hoursWorkday, double hoursSaturday, double hoursSunday)
        {
            if (_objectData == null || !_objectData.CoolingSeasonEnabled) return 0.0;

            int startMonth = _objectData.CoolingSeasonStartMonth ?? 1;
            int endMonth = _objectData.CoolingSeasonEndMonth ?? 12;
                int year = 2024; // consistent with Section 13 (BgVentilationCalculator uses hardcoded 2024)

            double totalDays = 0.0;

            for (int m = 1; m <= 12; m++)
            {
                if (!IsMonthInCoolingSeason(m, startMonth, endMonth)) continue;

                var (workDays_m, satCount_m, sunCount_m) = CalendarService.GetCalendarCounts(year, m);
                int offDays_m = GetOffDaysForMonth(m);

                // Effective work days per day type (if hours are 0:00-0:00, no occupancy)
                double effectiveWorkdays = (hoursWorkday > 0) ? workDays_m : 0.0;
                double effectiveSaturdays = (hoursSaturday > 0) ? satCount_m : 0.0;
                double effectiveSundays = (hoursSunday > 0) ? sunCount_m : 0.0;

                // k_m coefficient: only applies to effective workdays (Mon-Fri with occupancy)
                double workDays_m_total = effectiveWorkdays + effectiveSaturdays + effectiveSundays;
                double offDays_m_effective = Math.Min(offDays_m, effectiveWorkdays); // OffDays apply ONLY to Mon-Fri
                double k_m = (effectiveWorkdays > 0) ? Math.Max(0.0, (effectiveWorkdays - offDays_m_effective) / effectiveWorkdays) : 1.0;

                // Corrected days: scale only Mon-Fri by k_m, Sat/Sun remain unchanged
                double correctedDays = (k_m * effectiveWorkdays) + effectiveSaturdays + effectiveSundays;
                totalDays += correctedDays;
            }

            return totalDays;
        }

        /// <summary>
        /// Изчислява общ брой работни часове за сезона с k_m корекция.
        /// </summary>
        private double CalculateSeasonalWorkHours(double hoursWorkday, double hoursSaturday, double hoursSunday)
        {
            if (_objectData == null || !_objectData.CoolingSeasonEnabled) return 0.0;

            int startMonth = _objectData.CoolingSeasonStartMonth ?? 1;
            int endMonth = _objectData.CoolingSeasonEndMonth ?? 12;
                int year = 2024; // consistent with Section 13 (BgVentilationCalculator uses hardcoded 2024)

            double totalHours = 0.0;

            for (int m = 1; m <= 12; m++)
            {
                if (!IsMonthInCoolingSeason(m, startMonth, endMonth)) continue;

                var (workDays_m, satCount_m, sunCount_m) = CalendarService.GetCalendarCounts(year, m);
                int offDays_m = GetOffDaysForMonth(m);

                // Effective work days per day type (if hours are 0:00-0:00, no occupancy)
                double effectiveWorkdays = (hoursWorkday > 0) ? workDays_m : 0.0;
                double effectiveSaturdays = (hoursSaturday > 0) ? satCount_m : 0.0;
                double effectiveSundays = (hoursSunday > 0) ? sunCount_m : 0.0;

                // k_m coefficient: only applies to Mon-Fri workdays
                double offDays_m_effective = Math.Min(offDays_m, effectiveWorkdays); // OffDays apply ONLY to Mon-Fri
                double k_m = (effectiveWorkdays > 0) ? Math.Max(0.0, (effectiveWorkdays - offDays_m_effective) / effectiveWorkdays) : 1.0;

                // Corrected hours: scale only Mon-Fri hours by k_m, Sat/Sun remain unchanged
                double hoursThisMonth = (k_m * effectiveWorkdays * hoursWorkday) + (effectiveSaturdays * hoursSaturday) + (effectiveSundays * hoursSunday);
                totalHours += hoursThisMonth;
            }

            return totalHours;
        }

        /// <summary>
        /// Връща OffDays за конкретен месец от ObjectData.
        /// </summary>
        private int GetOffDaysForMonth(int month)
        {
            if (_objectData == null) return 0;

            string? offDaysStr = month switch
            {
                1 => _objectData.DaysOffJanuary,
                2 => _objectData.DaysOffFebruary,
                3 => _objectData.DaysOffMarch,
                4 => _objectData.DaysOffApril,
                5 => _objectData.DaysOffMay,
                6 => _objectData.DaysOffJune,
                7 => _objectData.DaysOffJuly,
                8 => _objectData.DaysOffAugust,
                9 => _objectData.DaysOffSeptember,
                10 => _objectData.DaysOffOctober,
                11 => _objectData.DaysOffNovember,
                12 => _objectData.DaysOffDecember,
                _ => null
            };

            if (string.IsNullOrWhiteSpace(offDaysStr)) return 0;
            if (int.TryParse(offDaysStr, out int result)) return Math.Max(0, result);
            return 0;
        }

        /// <summary>
        /// Проверка дали месец е в охладителния сезон.
        /// </summary>
        private bool IsMonthInCoolingSeason(int month, int startMonth, int endMonth)
        {
            if (startMonth <= endMonth)
            {
                return month >= startMonth && month <= endMonth;
            }
            else
            {
                // Wraparound (например октомври-март)
                return month >= startMonth || month <= endMonth;
            }
        }

        // ========== END OF NEW METHODOLOGY ==========

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
