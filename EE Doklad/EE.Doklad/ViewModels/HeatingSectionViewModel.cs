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

        public event PropertyChangedEventHandler? PropertyChanged;

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
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.HeatingEfficiency = clamped;
                    OnPropertyChanged(nameof(HeatingEfficiency));
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
