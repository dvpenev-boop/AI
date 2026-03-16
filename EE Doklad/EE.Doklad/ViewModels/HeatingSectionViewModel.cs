using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using EE.Doklad.Models;
using EE.Doklad.Sections.Section24SolarGains.Models;
using EE.Doklad.Services;

namespace EE.Doklad.ViewModels
{
    /// <summary>
    /// ViewModel за секция "10. Отопление"
    /// Управлява ръчните входове, изчислява автоматично топлината от обитатели
    /// с линейна интерполация на базата на таблица
    /// </summary>
    public class HeatingSectionViewModel : INotifyPropertyChanged
    {
        private readonly HeatingSectionData _data;
        private readonly ObjectDataSectionData? _objectData;
        private readonly Report? _report;
        private readonly ExternalWallsSectionData? _wallsData;
        private readonly RoofSectionData? _roofData;
        private readonly FloorSectionData? _floorData;
        private readonly WindowsSectionData? _windowsData;
        private readonly UnconditionedZoneSectionData? _ztuData;
        private readonly VentilationSectionData? _ventilationHeatingData;
        private readonly InternalGainsDebugInput? _section23Data;
        private readonly Section24SolarGainsData? _section24Data;
        private bool _isAdjustingShares = false; // guard to avoid recursive share updates
        private double _heatingInstallationHours;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Указва дали отоплителният сезон е включен (контролира gating на секцията)
        /// </summary>
        public bool IsHeatingSeasonEnabled => _objectData?.HeatingSeasonEnabled ?? true;

        /// <summary>
        /// Информационен текст, когато отоплителният сезон не е избран
        /// </summary>
        public string HeatingSeasonWarning => IsHeatingSeasonEnabled ? string.Empty : "Не е избран отоплителен сезон.";

        // ========== PROPERTIES ==========

        public string Description
        {
            get => _data.Description ?? string.Empty;
            set
            {
                if (_data.Description != value)
                {
                    _data.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        // ========== РЪЧНИ ВХОДОВЕ ==========

        private string _infiltrationText;
        public string InfiltrationText
        {
            get => _infiltrationText;
            set
            {
                if (_infiltrationText != value)
                {
                    _infiltrationText = value;
                    OnPropertyChanged(nameof(InfiltrationText));
                    ValidateAndSetInfiltration(value);
                }
            }
        }

        private string _designTemperatureText;
        public string DesignTemperatureText
        {
            get => _designTemperatureText;
            set
            {
                if (_designTemperatureText != value)
                {
                    _designTemperatureText = value;
                    OnPropertyChanged(nameof(DesignTemperatureText));
                    ValidateAndSetDesignTemperature(value);
                }
            }
        }

        private string _reductionTemperatureText;
        public string ReductionTemperatureText
        {
            get => _reductionTemperatureText;
            set
            {
                if (_reductionTemperatureText != value)
                {
                    _reductionTemperatureText = value;
                    OnPropertyChanged(nameof(ReductionTemperatureText));
                    ValidateAndSetReductionTemperature(value);
                }
            }
        }

        public double EmissionEfficiency
        {
            get => _data.EmissionEfficiency;
            set
            {
                if (_data.EmissionEfficiency != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EmissionEfficiency = clamped;
                    OnPropertyChanged(nameof(EmissionEfficiency));
                }
            }
        }

        public double DistributionEfficiency
        {
            get => _data.DistributionEfficiency;
            set
            {
                if (_data.DistributionEfficiency != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.DistributionEfficiency = clamped;
                    OnPropertyChanged(nameof(DistributionEfficiency));
                }
            }
        }

        public double AutomaticControl
        {
            get => _data.AutomaticControl;
            set
            {
                if (_data.AutomaticControl != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.AutomaticControl = clamped;
                    OnPropertyChanged(nameof(AutomaticControl));
                }
            }
        }

        public double EnergyManagement
        {
            get => _data.EnergyManagement;
            set
            {
                if (_data.EnergyManagement != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergyManagement = clamped;
                    OnPropertyChanged(nameof(EnergyManagement));
                }
            }
        }

        public double HeatingEfficiency
        {
            get => _data.HeatingEfficiency;
            set
            {
                if (_data.HeatingEfficiency != value)
                {
                    // Allow heating efficiency values greater than 100 (some heat sources may be represented >100%),
                    // but do not allow negative values.
                    var bounded = Math.Max(value, 0);
                    _data.HeatingEfficiency = bounded;
                    OnPropertyChanged(nameof(HeatingEfficiency));
                }
            }
        }

        // ========== ENERGY SOURCE 1 PROPERTIES (for heating) ==========

        public double EnergySource1Share
        {
            get => _data.EnergySource1.Share;
            set
            {
                if (_data.EnergySource1.Share != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource1.Share = clamped;
                    OnPropertyChanged(nameof(EnergySource1Share));

                    // If second source is used, auto-adjust the other share so total is 100%
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

                    NotifyGrossHeatingEnergyChanged();
                }
            }
        }

        public double EnergySource1EmissionEfficiency
        {
            get => _data.EnergySource1.EmissionEfficiency;
            set
            {
                if (_data.EnergySource1.EmissionEfficiency != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource1.EmissionEfficiency = clamped;
                    OnPropertyChanged(nameof(EnergySource1EmissionEfficiency));
                    NotifyGrossHeatingEnergyChanged();
                }
            }
        }

        public double EnergySource1DistributionEfficiency
        {
            get => _data.EnergySource1.DistributionEfficiency;
            set
            {
                if (_data.EnergySource1.DistributionEfficiency != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource1.DistributionEfficiency = clamped;
                    OnPropertyChanged(nameof(EnergySource1DistributionEfficiency));
                    NotifyGrossHeatingEnergyChanged();
                }
            }
        }

        public double EnergySource1AutomaticControl
        {
            get => _data.EnergySource1.AutomaticControl;
            set
            {
                if (_data.EnergySource1.AutomaticControl != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource1.AutomaticControl = clamped;
                    OnPropertyChanged(nameof(EnergySource1AutomaticControl));
                    NotifyGrossHeatingEnergyChanged();
                }
            }
        }

        public double EnergySource1EnergyManagement
        {
            get => _data.EnergySource1.EnergyManagement;
            set
            {
                if (_data.EnergySource1.EnergyManagement != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource1.EnergyManagement = clamped;
                    OnPropertyChanged(nameof(EnergySource1EnergyManagement));
                    NotifyGrossHeatingEnergyChanged();
                }
            }
        }

        public double EnergySource1GenerationEfficiency
        {
            get => _data.EnergySource1.GenerationEfficiency;
            set
            {
                if (_data.EnergySource1.GenerationEfficiency != value)
                {
                    var clamped = Math.Max(value, 0); // Can be > 100 for heat pumps
                    _data.EnergySource1.GenerationEfficiency = clamped;
                    OnPropertyChanged(nameof(EnergySource1GenerationEfficiency));
                    NotifyGrossHeatingEnergyChanged();
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
                }
            }
        }

        // ========== ENERGY SOURCE 2 PROPERTIES (optional) ==========

        public bool UseSecondEnergySource
        {
            get => _data.UseSecondEnergySource;
            set
            {
                if (_data.UseSecondEnergySource != value)
                {
                    _data.UseSecondEnergySource = value;
                    // If disabling second source, force shares to 100/0
                    if (!value)
                    {
                        if (_data.EnergySource1 != null)
                        {
                            _data.EnergySource1.Share = 100.0;
                            OnPropertyChanged(nameof(EnergySource1Share));
                        }
                        if (_data.EnergySource2 != null)
                        {
                            _data.EnergySource2.Share = 0.0;
                            OnPropertyChanged(nameof(EnergySource2Share));
                        }
                    }
                    else
                    {
                        // Ensure energySource2 exists
                        if (_data.EnergySource2 == null)
                        {
                            _data.EnergySource2 = new VentilationEnergySource
                            {
                                Type = EnergySourceType.Electricity,
                                Share = 0.0
                            };
                        }
                    }

                    OnPropertyChanged(nameof(UseSecondEnergySource));
                    NotifyGrossHeatingEnergyChanged();
                }
            }
        }

        public double EnergySource2Share
        {
            get => _data.EnergySource2?.Share ?? 0;
            set
            {
                if (_data.EnergySource2 != null && _data.EnergySource2.Share != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource2.Share = clamped;
                    OnPropertyChanged(nameof(EnergySource2Share));

                    // Auto-adjust source1 if needed
                    if (UseSecondEnergySource && _data.EnergySource1 != null && !_isAdjustingShares)
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

                    NotifyGrossHeatingEnergyChanged();
                }
            }
        }

        public double EnergySource2EmissionEfficiency
        {
            get => _data.EnergySource2?.EmissionEfficiency ?? 100;
            set
            {
                if (_data.EnergySource2 != null && _data.EnergySource2.EmissionEfficiency != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource2.EmissionEfficiency = clamped;
                    OnPropertyChanged(nameof(EnergySource2EmissionEfficiency));
                    NotifyGrossHeatingEnergyChanged();
                }
            }
        }

        public double EnergySource2DistributionEfficiency
        {
            get => _data.EnergySource2?.DistributionEfficiency ?? 100;
            set
            {
                if (_data.EnergySource2 != null && _data.EnergySource2.DistributionEfficiency != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource2.DistributionEfficiency = clamped;
                    OnPropertyChanged(nameof(EnergySource2DistributionEfficiency));
                    NotifyGrossHeatingEnergyChanged();
                }
            }
        }

        public double EnergySource2AutomaticControl
        {
            get => _data.EnergySource2?.AutomaticControl ?? 100;
            set
            {
                if (_data.EnergySource2 != null && _data.EnergySource2.AutomaticControl != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource2.AutomaticControl = clamped;
                    OnPropertyChanged(nameof(EnergySource2AutomaticControl));
                    NotifyGrossHeatingEnergyChanged();
                }
            }
        }

        public double EnergySource2EnergyManagement
        {
            get => _data.EnergySource2?.EnergyManagement ?? 100;
            set
            {
                if (_data.EnergySource2 != null && _data.EnergySource2.EnergyManagement != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource2.EnergyManagement = clamped;
                    OnPropertyChanged(nameof(EnergySource2EnergyManagement));
                    NotifyGrossHeatingEnergyChanged();
                }
            }
        }

        public double EnergySource2GenerationEfficiency
        {
            get => _data.EnergySource2?.GenerationEfficiency ?? 100;
            set
            {
                if (_data.EnergySource2 != null && _data.EnergySource2.GenerationEfficiency != value)
                {
                    var clamped = Math.Max(value, 0);
                    _data.EnergySource2.GenerationEfficiency = clamped;
                    OnPropertyChanged(nameof(EnergySource2GenerationEfficiency));
                    NotifyGrossHeatingEnergyChanged();
                }
            }
        }

        public Models.EnergyCarrierCode? EnergySource2Carrier
        {
            get => _data.EnergySource2?.EnergyCarrier;
            set
            {
                if (_data.EnergySource2 != null && _data.EnergySource2.EnergyCarrier != value)
                {
                    _data.EnergySource2.EnergyCarrier = value;
                    OnPropertyChanged(nameof(EnergySource2Carrier));
                }
            }
        }

        // ========== ОБИТАТЕЛИ ==========

        /// <summary>
        /// Списък с налични активности за dropdown
        /// </summary>
        public List<ActivityLevelOption> ActivityLevelOptions { get; }

        public ActivityLevel SelectedActivityLevel
        {
            get => _data.SelectedActivityLevel;
            set
            {
                if (_data.SelectedActivityLevel != value)
                {
                    _data.SelectedActivityLevel = value;
                    OnPropertyChanged(nameof(SelectedActivityLevel));
                    RecalculateOccupantHeat();
                }
            }
        }

        /// <summary>
        /// Брой обитатели (read-only, от секция 5)
        /// </summary>
        public int NumberOfOccupants
        {
            get
            {
                if (_objectData?.NumberOfOccupants != null &&
                    int.TryParse(_objectData.NumberOfOccupants, NumberStyles.Integer, CultureInfo.InvariantCulture, out int count))
                {
                    return count;
                }
                return 0;
            }
        }

        /// <summary>
        /// Температура в помещението (read-only, равна на проектна температура)
        /// </summary>
        public string RoomTemperatureDisplay => $"{_data.DesignTemperature:F2} °C";

        public ObservableCollection<HeatingCharacteristicRow> ThermalCharacteristicsRows { get; } = new();
        public string HeatingInstallationHoursDisplay => $"{_heatingInstallationHours:F0}";
        public double NetHeatingEnergyNoGainsPerArea { get; private set; }
        public string NetHeatingEnergyNoGainsPerAreaDisplay => NetHeatingEnergyNoGainsPerArea.ToString("F2", CultureInfo.InvariantCulture);
        public double VentilationGainPerArea { get; private set; } = 0.0;
        public double LightingGainPerArea { get; private set; } = 0.0;
        public double AppliancesGainPerArea { get; private set; } = 0.0;
        public double NetHeatingEnergyAfterGains =>
            Math.Max(0.0, NetHeatingEnergyNoGainsPerArea
                - VentilationGainPerArea
                - LightingGainPerArea
                - AppliancesGainPerArea);
        public string VentilationGainPerAreaDisplay => VentilationGainPerArea.ToString("F2", CultureInfo.InvariantCulture);
        public string LightingGainPerAreaDisplay => LightingGainPerArea.ToString("F2", CultureInfo.InvariantCulture);
        public string AppliancesGainPerAreaDisplay => AppliancesGainPerArea.ToString("F2", CultureInfo.InvariantCulture);
        public string NetHeatingEnergyAfterGainsDisplay => NetHeatingEnergyAfterGains.ToString("F2", CultureInfo.InvariantCulture);
        public double VentilationGainKwh => VentilationGainPerArea * HeatedArea;
        public double LightingGainKwh => LightingGainPerArea * HeatedArea;
        public double AppliancesGainKwh => AppliancesGainPerArea * HeatedArea;
        public double NetHeatingEnergyAfterGainsKwh => NetHeatingEnergyAfterGains * HeatedArea;
        public string VentilationGainKwhDisplay => VentilationGainKwh.ToString("F2", CultureInfo.InvariantCulture);
        public string LightingGainKwhDisplay => LightingGainKwh.ToString("F2", CultureInfo.InvariantCulture);
        public string AppliancesGainKwhDisplay => AppliancesGainKwh.ToString("F2", CultureInfo.InvariantCulture);
        public string NetHeatingEnergyAfterGainsKwhDisplay => NetHeatingEnergyAfterGainsKwh.ToString("F2", CultureInfo.InvariantCulture);
        public double? EnergySource1RequiredEnergyPerArea => CalculateRequiredEnergyForSource(
            NetHeatingEnergyAfterGains,
            EnergySource1Share,
            EnergySource1EmissionEfficiency,
            EnergySource1DistributionEfficiency,
            EnergySource1AutomaticControl,
            EnergySource1EnergyManagement,
            EnergySource1GenerationEfficiency);
        public string EnergySource1RequiredEnergyPerAreaDisplay => FormatNullableDouble(EnergySource1RequiredEnergyPerArea);
        public double? EnergySource2RequiredEnergyPerArea => UseSecondEnergySource
            ? CalculateRequiredEnergyForSource(
                NetHeatingEnergyAfterGains,
                EnergySource2Share,
                EnergySource2EmissionEfficiency,
                EnergySource2DistributionEfficiency,
                EnergySource2AutomaticControl,
                EnergySource2EnergyManagement,
                EnergySource2GenerationEfficiency)
            : 0.0;
        public string EnergySource2RequiredEnergyPerAreaDisplay => FormatNullableDouble(EnergySource2RequiredEnergyPerArea);
        public double TotalGrossHeatingEnergyPerArea =>
            (EnergySource1RequiredEnergyPerArea ?? 0.0) +
            (EnergySource2RequiredEnergyPerArea ?? 0.0);
        public string TotalGrossHeatingEnergyPerAreaDisplay => TotalGrossHeatingEnergyPerArea.ToString("F2", CultureInfo.InvariantCulture);
        public double? OverallHeatGenerationEfficiencyPercent =>
            TotalGrossHeatingEnergyPerArea > 0.0
                ? NetHeatingEnergyAfterGains / TotalGrossHeatingEnergyPerArea * 100.0
                : null;
        public string OverallHeatGenerationEfficiencyDisplay => FormatNullableDouble(OverallHeatGenerationEfficiencyPercent);
        public bool HasEnergySourceShareWarning => Math.Abs(GetTotalEnergySourceShare() - 100.0) > 0.01;
        public string EnergySourceShareWarning => "Сборът на дяловете трябва да е 100%";

        // ========== ИЗЧИСЛЕНИ СТОЙНОСТИ ==========

        private double _sensibleHeatPerPerson;
        public double SensibleHeatPerPerson
        {
            get => _sensibleHeatPerPerson;
            private set
            {
                if (Math.Abs(_sensibleHeatPerPerson - value) > 0.001)
                {
                    _sensibleHeatPerPerson = value;
                    OnPropertyChanged(nameof(SensibleHeatPerPerson));
                    OnPropertyChanged(nameof(SensibleHeatPerPersonDisplay));
                }
            }
        }

        public string SensibleHeatPerPersonDisplay => $"{SensibleHeatPerPerson:F2} W";

        private double _latentHeatPerPerson;
        public double LatentHeatPerPerson
        {
            get => _latentHeatPerPerson;
            private set
            {
                if (Math.Abs(_latentHeatPerPerson - value) > 0.001)
                {
                    _latentHeatPerPerson = value;
                    OnPropertyChanged(nameof(LatentHeatPerPerson));
                    OnPropertyChanged(nameof(LatentHeatPerPersonDisplay));
                }
            }
        }

        public string LatentHeatPerPersonDisplay => $"{LatentHeatPerPerson:F2} W";

        private double _totalOccupantHeat;
        public double TotalOccupantHeat
        {
            get => _totalOccupantHeat;
            private set
            {
                if (Math.Abs(_totalOccupantHeat - value) > 0.001)
                {
                    _totalOccupantHeat = value;
                    OnPropertyChanged(nameof(TotalOccupantHeat));
                    OnPropertyChanged(nameof(TotalOccupantHeatDisplay));
                }
            }
        }

        public string TotalOccupantHeatDisplay => $"{TotalOccupantHeat:F2} W";

        private double _totalLatentHeat;
        public double TotalLatentHeat
        {
            get => _totalLatentHeat;
            private set
            {
                if (Math.Abs(_totalLatentHeat - value) > 0.001)
                {
                    _totalLatentHeat = value;
                    OnPropertyChanged(nameof(TotalLatentHeat));
                    OnPropertyChanged(nameof(TotalLatentHeatDisplay));
                }
            }
        }

        public string TotalLatentHeatDisplay => $"{TotalLatentHeat:F2} W";

        // ========== W/m² ИЗЧИСЛЕНИЯ ==========

        /// <summary>
        /// Отопляема площ от Секция 5
        /// </summary>
        public double HeatedArea
        {
            get
            {
                if (_objectData?.HeatedArea != null &&
                    double.TryParse(_objectData.HeatedArea.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double area))
                {
                    return area > 0 ? area : 0;
                }
                return 0;
            }
        }

        /// <summary>
        /// Топлина от обитатели [W/m²]
        /// </summary>
        public double TotalOccupantHeatPerArea => HeatedArea > 0 ? TotalOccupantHeat / HeatedArea : 0;

        public string TotalOccupantHeatPerAreaDisplay => HeatedArea > 0 
            ? $"{TotalOccupantHeatPerArea:F2} W/m²" 
            : "– (няма площ)";

        /// <summary>
        /// Латентна топлина от обитатели [W/m²]
        /// </summary>
        public double TotalLatentHeatPerArea => HeatedArea > 0 ? TotalLatentHeat / HeatedArea : 0;

        public string TotalLatentHeatPerAreaDisplay => HeatedArea > 0 
            ? $"{TotalLatentHeatPerArea:F2} W/m²" 
            : "– (няма площ)";

        // ========== VALIDATION ERRORS ==========

        private string? _infiltrationError;
        public string? InfiltrationError
        {
            get => _infiltrationError;
            private set
            {
                if (_infiltrationError != value)
                {
                    _infiltrationError = value;
                    OnPropertyChanged(nameof(InfiltrationError));
                    OnPropertyChanged(nameof(HasInfiltrationError));
                }
            }
        }

        public bool HasInfiltrationError => !string.IsNullOrEmpty(InfiltrationError);

        private string? _designTemperatureError;
        public string? DesignTemperatureError
        {
            get => _designTemperatureError;
            private set
            {
                if (_designTemperatureError != value)
                {
                    _designTemperatureError = value;
                    OnPropertyChanged(nameof(DesignTemperatureError));
                    OnPropertyChanged(nameof(HasDesignTemperatureError));
                }
            }
        }

        public bool HasDesignTemperatureError => !string.IsNullOrEmpty(DesignTemperatureError);

        private string? _reductionTemperatureError;
        public string? ReductionTemperatureError
        {
            get => _reductionTemperatureError;
            private set
            {
                if (_reductionTemperatureError != value)
                {
                    _reductionTemperatureError = value;
                    OnPropertyChanged(nameof(ReductionTemperatureError));
                    OnPropertyChanged(nameof(HasReductionTemperatureError));
                }
            }
        }

        public bool HasReductionTemperatureError => !string.IsNullOrEmpty(ReductionTemperatureError);

        // ========== CONSTRUCTOR ==========

        public HeatingSectionViewModel(HeatingSectionData data, ObjectDataSectionData? objectData = null, Report? report = null)
        {
            _data = data;
            _objectData = objectData;
            _report = report;
            _wallsData = _report?.Sections?.FirstOrDefault(s => s.Type == SectionType.ExternalWalls)?.ExternalWallsSectionData;
            _roofData = _report?.Sections?.FirstOrDefault(s => s.Type == SectionType.Roof)?.RoofSectionData;
            _floorData = _report?.Sections?.FirstOrDefault(s => s.Type == SectionType.Floor)?.FloorSectionData;
            _windowsData = _report?.Sections?.FirstOrDefault(s => s.Type == SectionType.Windows)?.WindowsSectionData;
            _ztuData = _report?.Sections?.FirstOrDefault(s => s.Type == SectionType.UnconditionedZones)?.UnconditionedZoneSectionData;
            _section23Data = _report?.Sections?.FirstOrDefault(s => s.Type == SectionType.InternalGainsDebug)?.InternalGainsDebugInput;
            _section24Data = _report?.Sections?.FirstOrDefault(s => s.Type == SectionType.SolarGains)?.SolarGainsData;
            _ventilationHeatingData = _report?.Sections?.FirstOrDefault(s =>
                s.Type == SectionType.Ventilation &&
                s.VentilationSectionData != null &&
                (s.Title?.Contains("Вентилация Отопление", StringComparison.OrdinalIgnoreCase) ?? false))
                ?.VentilationSectionData;

            // Инициализация на activity options
            ActivityLevelOptions = ActivityDataService.GetAllActivities()
                .Select(a => new ActivityLevelOption
                {
                    Level = a.ActivityLevel,
                    DisplayName = a.DisplayName
                })
                .ToList();

            // Инициализация на текстови полета
            _infiltrationText = _data.Infiltration.ToString("F2", CultureInfo.InvariantCulture);
            _designTemperatureText = _data.DesignTemperature.ToString("F2", CultureInfo.InvariantCulture);
            _reductionTemperatureText = _data.ReductionTemperature.ToString("F2", CultureInfo.InvariantCulture);

            // Subscribe към промени в ObjectData за брой обитатели
            if (_objectData != null)
            {
                _objectData.PropertyChanged += ObjectData_PropertyChanged;
            }

            if (_ventilationHeatingData != null)
            {
                _ventilationHeatingData.PropertyChanged += VentilationHeatingData_PropertyChanged;
            }

            // Първоначално изчисление
            RecalculateOccupantHeat();
            AttachSectionListeners();
            RefreshThermalCharacteristics();
        }

        private void ObjectData_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ObjectDataSectionData.NumberOfOccupants))
            {
                OnPropertyChanged(nameof(NumberOfOccupants));
                RecalculateOccupantHeat();
                RefreshThermalCharacteristics();
            }
            else if (e.PropertyName == nameof(ObjectDataSectionData.HeatedArea))
            {
                OnPropertyChanged(nameof(HeatedArea));
                OnPropertyChanged(nameof(TotalOccupantHeatPerArea));
                OnPropertyChanged(nameof(TotalOccupantHeatPerAreaDisplay));
                OnPropertyChanged(nameof(TotalLatentHeatPerArea));
                OnPropertyChanged(nameof(TotalLatentHeatPerAreaDisplay));
                NotifyGrossHeatingEnergyChanged();
            }
            else if (e.PropertyName == nameof(ObjectDataSectionData.HeatingSeasonEnabled))
            {
                OnPropertyChanged(nameof(IsHeatingSeasonEnabled));
                OnPropertyChanged(nameof(HeatingSeasonWarning));
            }

            RefreshThermalCharacteristics();
        }

        private void VentilationHeatingData_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RefreshThermalCharacteristics();
        }

        // ========== VALIDATION & PARSING ==========

        private void ValidateAndSetInfiltration(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                InfiltrationError = "Полето е задължително";
                return;
            }

            // Normalize: replace comma with dot
            var normalized = text.Replace(',', '.');

            if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                if (value < 0)
                {
                    InfiltrationError = "Стойността трябва да е >= 0";
                    return;
                }

                // Round to 2 decimal places
                value = Math.Round(value, 2);
                _data.Infiltration = value;
                InfiltrationError = null;

                // Update text to normalized format
                if (_infiltrationText != value.ToString("F2", CultureInfo.InvariantCulture))
                {
                    _infiltrationText = value.ToString("F2", CultureInfo.InvariantCulture);
                    OnPropertyChanged(nameof(InfiltrationText));
                }
                RefreshThermalCharacteristics();
            }
            else
            {
                InfiltrationError = "Невалидно число";
            }
        }

        private void ValidateAndSetDesignTemperature(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                DesignTemperatureError = "Полето е задължително";
                return;
            }

            var normalized = text.Replace(',', '.');

            if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                value = Math.Round(value, 2);
                _data.DesignTemperature = value;
                DesignTemperatureError = null;

                if (_designTemperatureText != value.ToString("F2", CultureInfo.InvariantCulture))
                {
                    _designTemperatureText = value.ToString("F2", CultureInfo.InvariantCulture);
                    OnPropertyChanged(nameof(DesignTemperatureText));
                }

                // Преизчисли топлината (температурата влияе на интерполацията)
                OnPropertyChanged(nameof(RoomTemperatureDisplay));
                RecalculateOccupantHeat();
                RefreshThermalCharacteristics();
            }
            else
            {
                DesignTemperatureError = "Невалидно число";
            }
        }

        private void ValidateAndSetReductionTemperature(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                ReductionTemperatureError = "Полето е задължително";
                return;
            }

            var normalized = text.Replace(',', '.');

            if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                value = Math.Round(value, 2);
                _data.ReductionTemperature = value;
                ReductionTemperatureError = null;

                if (_reductionTemperatureText != value.ToString("F2", CultureInfo.InvariantCulture))
                {
                    _reductionTemperatureText = value.ToString("F2", CultureInfo.InvariantCulture);
                    OnPropertyChanged(nameof(ReductionTemperatureText));
                }
                RefreshThermalCharacteristics();
            }
            else
            {
                ReductionTemperatureError = "Невалидно число";
            }
        }

        // ========== CALCULATION ==========

        /// <summary>
        /// Преизчислява топлината от обитатели с линейна интерполация
        /// </summary>
        private void RecalculateOccupantHeat()
        {
            var temperature = _data.DesignTemperature;
            var activityLevel = _data.SelectedActivityLevel;
            var occupantCount = NumberOfOccupants;

            // Изчисли sensible и latent heat per person чрез интерполация
            var (sensible, latent) = ActivityDataService.CalculateHeatForTemperature(activityLevel, temperature);

            SensibleHeatPerPerson = sensible;
            LatentHeatPerPerson = latent;

            // Изчисли total heat
            TotalOccupantHeat = sensible * occupantCount;
            TotalLatentHeat = latent * occupantCount;

            // Актуализирай W/m² полета
            OnPropertyChanged(nameof(HeatedArea));
            OnPropertyChanged(nameof(TotalOccupantHeatPerArea));
            OnPropertyChanged(nameof(TotalOccupantHeatPerAreaDisplay));
            OnPropertyChanged(nameof(TotalLatentHeatPerArea));
            OnPropertyChanged(nameof(TotalLatentHeatPerAreaDisplay));
        }

        private void AttachSectionListeners()
        {
            AttachWallsListeners();
            AttachRoofListeners();
            AttachFloorListeners();
            AttachWindowsListeners();
            AttachZtuListeners();
            AttachSection23Listeners();
            AttachSection24Listeners();
        }

        private void AttachSection23Listeners()
        {
            if (_section23Data == null)
            {
                return;
            }

            _section23Data.HeatingMonths.CollectionChanged += Section23HeatingMonths_CollectionChanged;
        }

        private void AttachSection24Listeners()
        {
            if (_section24Data == null)
            {
                return;
            }

            _section24Data.MonthlyResults.CollectionChanged += Section24MonthlyResults_CollectionChanged;
        }

        private void Section23HeatingMonths_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshSectionGainContributions();
            NotifyGrossHeatingEnergyChanged();
        }

        private void Section24MonthlyResults_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshSectionGainContributions();
            NotifyGrossHeatingEnergyChanged();
        }

        private void AttachWallsListeners()
        {
            if (_wallsData == null)
            {
                return;
            }

            _wallsData.WallTypes.CollectionChanged += Walls_CollectionChanged;
            foreach (var wall in _wallsData.WallTypes)
            {
                AttachWall(wall);
            }
        }

        private void Walls_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ExternalWallType wall in e.OldItems)
                {
                    wall.PropertyChanged -= SourceData_PropertyChanged;
                    if (wall.ThermalBridges != null)
                    {
                        wall.ThermalBridges.PropertyChanged -= SourceData_PropertyChanged;
                    }
                }
            }

            if (e.NewItems != null)
            {
                foreach (ExternalWallType wall in e.NewItems)
                {
                    AttachWall(wall);
                }
            }

            RefreshThermalCharacteristics();
        }

        private void AttachWall(ExternalWallType wall)
        {
            wall.PropertyChanged += SourceData_PropertyChanged;
            if (wall.ThermalBridges != null)
            {
                wall.ThermalBridges.PropertyChanged += SourceData_PropertyChanged;
            }
        }

        private void AttachRoofListeners()
        {
            if (_roofData == null)
            {
                return;
            }

            _roofData.RoofTypes.CollectionChanged += Roof_CollectionChanged;
            foreach (var roof in _roofData.RoofTypes)
            {
                AttachRoof(roof);
            }
        }

        private void Roof_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (RoofType roof in e.OldItems)
                {
                    roof.PropertyChanged -= SourceData_PropertyChanged;
                    if (roof.ThermalBridges != null)
                    {
                        roof.ThermalBridges.PropertyChanged -= SourceData_PropertyChanged;
                    }
                }
            }

            if (e.NewItems != null)
            {
                foreach (RoofType roof in e.NewItems)
                {
                    AttachRoof(roof);
                }
            }

            RefreshThermalCharacteristics();
        }

        private void AttachRoof(RoofType roof)
        {
            roof.PropertyChanged += SourceData_PropertyChanged;
            if (roof.ThermalBridges != null)
            {
                roof.ThermalBridges.PropertyChanged += SourceData_PropertyChanged;
            }
        }

        private void AttachFloorListeners()
        {
            if (_floorData == null)
            {
                return;
            }

            _floorData.FloorItems.CollectionChanged += Floor_CollectionChanged;
            foreach (var item in _floorData.FloorItems)
            {
                item.PropertyChanged += SourceData_PropertyChanged;
            }
        }

        private void Floor_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (FloorItem item in e.OldItems)
                {
                    item.PropertyChanged -= SourceData_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (FloorItem item in e.NewItems)
                {
                    item.PropertyChanged += SourceData_PropertyChanged;
                }
            }

            RefreshThermalCharacteristics();
        }

        private void AttachWindowsListeners()
        {
            if (_windowsData == null)
            {
                return;
            }

            _windowsData.WindowBatches.CollectionChanged += Windows_CollectionChanged;
            foreach (var batch in _windowsData.WindowBatches)
            {
                batch.PropertyChanged += SourceData_PropertyChanged;
            }
        }

        private void Windows_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (WindowBatch batch in e.OldItems)
                {
                    batch.PropertyChanged -= SourceData_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (WindowBatch batch in e.NewItems)
                {
                    batch.PropertyChanged += SourceData_PropertyChanged;
                }
            }

            RefreshThermalCharacteristics();
        }

        private void AttachZtuListeners()
        {
            if (_ztuData == null)
            {
                return;
            }

            _ztuData.Zones.CollectionChanged += ZtuZones_CollectionChanged;
            foreach (var zone in _ztuData.Zones)
            {
                AttachZone(zone);
            }
        }

        private void ZtuZones_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ZtuZone zone in e.OldItems)
                {
                    DetachZone(zone);
                }
            }

            if (e.NewItems != null)
            {
                foreach (ZtuZone zone in e.NewItems)
                {
                    AttachZone(zone);
                }
            }

            RefreshThermalCharacteristics();
        }

        private void AttachZone(ZtuZone zone)
        {
            zone.PropertyChanged += SourceData_PropertyChanged;
            zone.ElementsToExternal.CollectionChanged += ZtuElements_CollectionChanged;
            zone.ElementsToBoundary.CollectionChanged += ZtuElements_CollectionChanged;

            foreach (var element in zone.ElementsToExternal)
            {
                AttachElement(element);
            }

            foreach (var element in zone.ElementsToBoundary)
            {
                AttachElement(element);
            }
        }

        private void DetachZone(ZtuZone zone)
        {
            zone.PropertyChanged -= SourceData_PropertyChanged;
            zone.ElementsToExternal.CollectionChanged -= ZtuElements_CollectionChanged;
            zone.ElementsToBoundary.CollectionChanged -= ZtuElements_CollectionChanged;

            foreach (var element in zone.ElementsToExternal)
            {
                DetachElement(element);
            }

            foreach (var element in zone.ElementsToBoundary)
            {
                DetachElement(element);
            }
        }

        private void ZtuElements_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ZtuElement element in e.OldItems)
                {
                    DetachElement(element);
                }
            }

            if (e.NewItems != null)
            {
                foreach (ZtuElement element in e.NewItems)
                {
                    AttachElement(element);
                }
            }

            RefreshThermalCharacteristics();
        }

        private void AttachElement(ZtuElement element)
        {
            element.PropertyChanged += SourceData_PropertyChanged;
            element.Layers.CollectionChanged += ZtuLayers_CollectionChanged;
            foreach (var layer in element.Layers)
            {
                layer.PropertyChanged += SourceData_PropertyChanged;
            }
        }

        private void DetachElement(ZtuElement element)
        {
            element.PropertyChanged -= SourceData_PropertyChanged;
            element.Layers.CollectionChanged -= ZtuLayers_CollectionChanged;
            foreach (var layer in element.Layers)
            {
                layer.PropertyChanged -= SourceData_PropertyChanged;
            }
        }

        private void ZtuLayers_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ZtuLayer layer in e.OldItems)
                {
                    layer.PropertyChanged -= SourceData_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (ZtuLayer layer in e.NewItems)
                {
                    layer.PropertyChanged += SourceData_PropertyChanged;
                }
            }

            RefreshThermalCharacteristics();
        }

        private void SourceData_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RefreshThermalCharacteristics();
        }

        private void RefreshThermalCharacteristics()
        {
            var snapshot = HeatingCharacteristicsService.Build(_report, _objectData, _data);
            VentilationGainPerArea = CalculateVentilationHeatingContributionPerArea();
            RefreshSectionGainContributions();

            double hTrTotal = snapshot.Walls.H + snapshot.Roof.H + snapshot.Floor.H + snapshot.Windows.H + snapshot.ThermalBridgesH_WK + snapshot.ZtuH_WK;
            double hInf = 0.34 * snapshot.InfiltrationRate_AirChangesPerHour * snapshot.BuildingVolume_m3;
            double hVeTotal = hInf;
            double hTotal = hTrTotal + hVeTotal;
            double annualEquivalentHours = CalculateAnnualEquivalentHours(snapshot);
            double deltaT = 1.0;
            double energyHours = annualEquivalentHours;
            _heatingInstallationHours = snapshot.HeatingOperatingHours_h;
            NetHeatingEnergyNoGainsPerArea = snapshot.HeatedArea_m2 > 0.0
                ? (hTotal * deltaT * energyHours / 1000.0) / snapshot.HeatedArea_m2
                : 0.0;
            OnPropertyChanged(nameof(HeatingInstallationHoursDisplay));
            NotifyGrossHeatingEnergyChanged();

            ThermalCharacteristicsRows.Clear();
            ThermalCharacteristicsRows.Add(CreateMetricRow("Външни стени", snapshot.Walls.U, snapshot.Walls.Area, snapshot.Walls.H, snapshot.HeatedArea_m2, deltaT, energyHours));
            ThermalCharacteristicsRows.Add(CreateMetricRow("Покрив", snapshot.Roof.U, snapshot.Roof.Area, snapshot.Roof.H, snapshot.HeatedArea_m2, deltaT, energyHours));
            ThermalCharacteristicsRows.Add(CreateMetricRow("Под", snapshot.Floor.U, snapshot.Floor.Area, snapshot.Floor.H, snapshot.HeatedArea_m2, deltaT, energyHours));
            ThermalCharacteristicsRows.Add(CreateMetricRow("Прозорци и врати", snapshot.Windows.U, snapshot.Windows.Area, snapshot.Windows.H, snapshot.HeatedArea_m2, deltaT, energyHours));
            ThermalCharacteristicsRows.Add(CreateMetricRow("Топлинни мостове", null, null, snapshot.ThermalBridgesH_WK, snapshot.HeatedArea_m2, deltaT, energyHours));
            ThermalCharacteristicsRows.Add(CreateMetricRow("ZTU (неотопляеми)", null, null, snapshot.ZtuH_WK, snapshot.HeatedArea_m2, deltaT, energyHours));
            ThermalCharacteristicsRows.Add(HeatingCharacteristicRow.Separator());
            ThermalCharacteristicsRows.Add(CreateMetricRow("H_tr ОБЩО", null, null, hTrTotal, snapshot.HeatedArea_m2, deltaT, energyHours, isTotal: true));
            ThermalCharacteristicsRows.Add(CreateMetricRow("Инфилтрация", snapshot.InfiltrationRate_AirChangesPerHour, snapshot.BuildingVolume_m3, hInf, snapshot.HeatedArea_m2, deltaT, energyHours, uUnit: "1/h", aUnit: "m³"));
            ThermalCharacteristicsRows.Add(HeatingCharacteristicRow.Separator());
            ThermalCharacteristicsRows.Add(CreateMetricRow("H_ve ОБЩО", null, null, hVeTotal, snapshot.HeatedArea_m2, deltaT, energyHours, isTotal: true));
            ThermalCharacteristicsRows.Add(CreateMetricRow("H_TOTAL", null, null, hTotal, snapshot.HeatedArea_m2, deltaT, energyHours, isTotal: true, isHighlighted: true));
        }

        private void RefreshSectionGainContributions()
        {
            double area = HeatedArea;
            if (_section23Data == null || area <= 0.0)
            {
                LightingGainPerArea = 0.0;
                AppliancesGainPerArea = 0.0;
                return;
            }

            double lightingGainKwh = _section23Data.HeatingMonths.Sum(m => m.L_kWh);
            double appliancesGainKwh = _section23Data.HeatingMonths.Sum(m => m.A_kWh);

            LightingGainPerArea = lightingGainKwh / area;
            AppliancesGainPerArea = appliancesGainKwh / area;
        }

        private double CalculateVentilationHeatingContributionPerArea()
        {
            if (_ventilationHeatingData == null || _objectData == null)
            {
                return 0.0;
            }

            var climateService = new ClimateService(new JsonClimateRepository());
            if (!climateService.TryGetZone(_objectData.ClimateZone, out var climateData) || climateData == null)
            {
                return 0.0;
            }

            _ventilationHeatingData.HeatedArea_m2 = HeatedArea;

            int[] monthlyDaysOff =
            {
                ParseMonthlyDaysOff(_objectData.DaysOffJanuary),
                ParseMonthlyDaysOff(_objectData.DaysOffFebruary),
                ParseMonthlyDaysOff(_objectData.DaysOffMarch),
                ParseMonthlyDaysOff(_objectData.DaysOffApril),
                ParseMonthlyDaysOff(_objectData.DaysOffMay),
                ParseMonthlyDaysOff(_objectData.DaysOffJune),
                ParseMonthlyDaysOff(_objectData.DaysOffJuly),
                ParseMonthlyDaysOff(_objectData.DaysOffAugust),
                ParseMonthlyDaysOff(_objectData.DaysOffSeptember),
                ParseMonthlyDaysOff(_objectData.DaysOffOctober),
                ParseMonthlyDaysOff(_objectData.DaysOffNovember),
                ParseMonthlyDaysOff(_objectData.DaysOffDecember)
            };

            var calculator = new BgVentilationCalculator();
            var result = calculator.Calculate(_ventilationHeatingData, climateData, monthlyDaysOff);

            return result?.VentilationHeatingNetContribution_kWh_m2a ?? 0.0;
        }

        private static int ParseMonthlyDaysOff(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }

            return int.TryParse(value.Trim(), out int parsed)
                ? Math.Max(0, parsed)
                : 0;
        }

        private static HeatingCharacteristicRow CreateMetricRow(
            string parameter,
            double? u,
            double? area,
            double h,
            double heatedArea,
            double deltaT,
            double operatingHours,
            bool isTotal = false,
            bool isHighlighted = false,
            string uUnit = "W/m²K",
            string aUnit = "m²")
        {
            double kwh = h * deltaT * operatingHours / 1000.0;
            return new HeatingCharacteristicRow
            {
                Parameter = parameter,
                UDisplay = u.HasValue ? u.Value.ToString("F3", CultureInfo.InvariantCulture) : "-",
                ADisplay = area.HasValue ? area.Value.ToString("F2", CultureInfo.InvariantCulture) : "-",
                HDisplay = h.ToString("F2", CultureInfo.InvariantCulture),
                KwhPerAreaDisplay = heatedArea > 0 ? (kwh / heatedArea).ToString("F2", CultureInfo.InvariantCulture) : "0.00",
                KwhDisplay = kwh.ToString("F2", CultureInfo.InvariantCulture),
                IsTotal = isTotal,
                IsHighlighted = isHighlighted,
                UToolTip = u.HasValue ? uUnit : null,
                AToolTip = area.HasValue ? aUnit : null
            };
        }

        private static double CalculateAnnualEquivalentHours(HeatingCharacteristicsSnapshot snapshot)
        {
            double annualEquivalentHours = 0.0;
            int monthCount = new[]
            {
                snapshot.MonthlyOutdoorTemps_C.Length,
                snapshot.MonthlyOperatingHours_h.Length,
                snapshot.MonthlySetbackHours_h.Length
            }.Min();

            for (int month = 0; month < monthCount; month++)
            {
                double te = snapshot.MonthlyOutdoorTemps_C[month];
                double fullDelta = Math.Max(0.0, snapshot.DesignIndoorTemp_C - te);
                double setbackDelta = Math.Max(0.0, snapshot.SetbackIndoorTemp_C - te);
                double fullHours = Math.Max(0.0, snapshot.MonthlyOperatingHours_h[month]);
                double setbackHours = Math.Max(0.0, snapshot.MonthlySetbackHours_h[month]);

                annualEquivalentHours += fullDelta * fullHours + setbackDelta * setbackHours;
            }

            return annualEquivalentHours;
        }

        private void NotifyGrossHeatingEnergyChanged()
        {
            OnPropertyChanged(nameof(NetHeatingEnergyNoGainsPerArea));
            OnPropertyChanged(nameof(NetHeatingEnergyNoGainsPerAreaDisplay));
            OnPropertyChanged(nameof(NetHeatingEnergyAfterGains));
            OnPropertyChanged(nameof(NetHeatingEnergyAfterGainsDisplay));
            OnPropertyChanged(nameof(NetHeatingEnergyAfterGainsKwh));
            OnPropertyChanged(nameof(NetHeatingEnergyAfterGainsKwhDisplay));
            OnPropertyChanged(nameof(VentilationGainPerAreaDisplay));
            OnPropertyChanged(nameof(VentilationGainKwhDisplay));
            OnPropertyChanged(nameof(LightingGainPerAreaDisplay));
            OnPropertyChanged(nameof(LightingGainKwhDisplay));
            OnPropertyChanged(nameof(AppliancesGainPerAreaDisplay));
            OnPropertyChanged(nameof(AppliancesGainKwhDisplay));
            OnPropertyChanged(nameof(EnergySource1RequiredEnergyPerArea));
            OnPropertyChanged(nameof(EnergySource1RequiredEnergyPerAreaDisplay));
            OnPropertyChanged(nameof(EnergySource2RequiredEnergyPerArea));
            OnPropertyChanged(nameof(EnergySource2RequiredEnergyPerAreaDisplay));
            OnPropertyChanged(nameof(TotalGrossHeatingEnergyPerArea));
            OnPropertyChanged(nameof(TotalGrossHeatingEnergyPerAreaDisplay));
            OnPropertyChanged(nameof(OverallHeatGenerationEfficiencyPercent));
            OnPropertyChanged(nameof(OverallHeatGenerationEfficiencyDisplay));
            OnPropertyChanged(nameof(HasEnergySourceShareWarning));
        }

        private double GetTotalEnergySourceShare()
        {
            return UseSecondEnergySource
                ? EnergySource1Share + EnergySource2Share
                : EnergySource1Share;
        }

        private static double? CalculateRequiredEnergyForSource(
            double netEnergyPerArea,
            double sharePercent,
            double emissionEfficiency,
            double distributionEfficiency,
            double automaticControl,
            double energyManagement,
            double generationEfficiency)
        {
            if (netEnergyPerArea <= 0.0 || sharePercent <= 0.0)
            {
                return 0.0;
            }

            double etaSys =
                (emissionEfficiency / 100.0) *
                (distributionEfficiency / 100.0) *
                (automaticControl / 100.0) *
                (energyManagement / 100.0) *
                (generationEfficiency / 100.0);

            if (etaSys <= 0.0)
            {
                return null;
            }

            return netEnergyPerArea * (sharePercent / 100.0) / etaSys;
        }

        private static string FormatNullableDouble(double? value)
        {
            return value.HasValue
                ? value.Value.ToString("F2", CultureInfo.InvariantCulture)
                : "-";
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Опция за dropdown на активности
    /// </summary>
    public class ActivityLevelOption
    {
        public ActivityLevel Level { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }

    public class HeatingCharacteristicRow
    {
        public string Parameter { get; set; } = string.Empty;
        public string UDisplay { get; set; } = "-";
        public string ADisplay { get; set; } = "-";
        public string HDisplay { get; set; } = "0.00";
        public string KwhPerAreaDisplay { get; set; } = "0.00";
        public string KwhDisplay { get; set; } = "0.00";
        public bool IsTotal { get; set; }
        public bool IsHighlighted { get; set; }
        public bool IsSeparator { get; set; }
        public string? UToolTip { get; set; }
        public string? AToolTip { get; set; }

        public static HeatingCharacteristicRow Separator()
        {
            return new HeatingCharacteristicRow
            {
                IsSeparator = true,
                Parameter = string.Empty,
                UDisplay = string.Empty,
                ADisplay = string.Empty,
                HDisplay = string.Empty,
                KwhPerAreaDisplay = string.Empty,
                KwhDisplay = string.Empty
            };
        }
    }
}
