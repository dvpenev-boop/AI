using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using EE.Doklad.Models;

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
        private bool _isAdjustingShares = false; // guard to avoid recursive share updates

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

        public HeatingSectionViewModel(HeatingSectionData data, ObjectDataSectionData? objectData = null)
        {
            _data = data;
            _objectData = objectData;

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

            // Първоначално изчисление
            RecalculateOccupantHeat();
        }

        private void ObjectData_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ObjectDataSectionData.NumberOfOccupants))
            {
                OnPropertyChanged(nameof(NumberOfOccupants));
                RecalculateOccupantHeat();
            }
            else if (e.PropertyName == nameof(ObjectDataSectionData.HeatedArea))
            {
                OnPropertyChanged(nameof(HeatedArea));
                OnPropertyChanged(nameof(TotalOccupantHeatPerArea));
                OnPropertyChanged(nameof(TotalOccupantHeatPerAreaDisplay));
                OnPropertyChanged(nameof(TotalLatentHeatPerArea));
                OnPropertyChanged(nameof(TotalLatentHeatPerAreaDisplay));
            }
            else if (e.PropertyName == nameof(ObjectDataSectionData.HeatingSeasonEnabled))
            {
                OnPropertyChanged(nameof(IsHeatingSeasonEnabled));
                OnPropertyChanged(nameof(HeatingSeasonWarning));
            }
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
}
