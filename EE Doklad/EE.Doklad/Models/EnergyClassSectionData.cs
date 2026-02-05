using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using EE.Doklad.Services;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Ред в таблицата с прагове за енергиен клас
    /// </summary>
    public partial class EnergyClassThresholdRow : ObservableObject
    {
        [ObservableProperty]
        private string _class = string.Empty;

        [ObservableProperty]
        private double? _minValue;

        [ObservableProperty]
        private double? _maxValue;

        [ObservableProperty]
        private string _ruleText = string.Empty;

        [ObservableProperty]
        private string _colorHex = "#CCCCCC";

        /// <summary>
        /// Дали този ред е избраният клас (за маркер)
        /// </summary>
        [ObservableProperty]
        private bool _isSelectedClass;

        /// <summary>
        /// Текст на маркера (празен ако не е избран)
        /// </summary>
        [ObservableProperty]
        private string _markerText = string.Empty;

        public string MinValueDisplay => MinValue?.ToString("F0") ?? "—";
        public string MaxValueDisplay => MaxValue?.ToString("F0") ?? "—";
    }

    /// <summary>
    /// Данни за секция "Клас на енергопотребление" (display-only)
    /// </summary>
    public partial class EnergyClassSectionData : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Клас на енергопотребление";

        [ObservableProperty]
        private string? _description;

        /// <summary>
        /// Тип на сградата от секция 5 (readonly - set externally by ViewModel)
        /// </summary>
        [ObservableProperty]
        private BuildingTypeCode? _buildingType;

        /// <summary>
    /// EP - Годишна специфична енергия от секция 'Резултати сграда' [kWh/m²] (readonly - set externally)
        /// </summary>
        [ObservableProperty]
        private double? _energyPerformance;

        /// <summary>
        /// Редове в таблицата с прагове (A..G)
        /// </summary>
        public ObservableCollection<EnergyClassThresholdRow> ThresholdRows { get; } = new();

        /// <summary>
        /// Автоматично изчислен енергиен клас (A-G)
        /// </summary>
        public EnergyClass? CalculatedClass
        {
            get
            {
                if (!BuildingType.HasValue || !EnergyPerformance.HasValue)
                    return null;

                return EnergyClassCalculator.CalculateClass(BuildingType.Value, EnergyPerformance.Value);
            }
        }

        /// <summary>
        /// Описание на класа с диапазон
        /// </summary>
        public string ClassDescription
        {
            get
            {
                if (!BuildingType.HasValue || CalculatedClass == null)
                    return "—";

                return EnergyClassCalculator.GetClassDescription(BuildingType.Value, CalculatedClass.Value);
            }
        }

        /// <summary>
        /// Кратък display на класа (само буквата)
        /// </summary>
        public string ClassDisplay => CalculatedClass?.ToString() ?? "—";

        /// <summary>
        /// Цвят на класа според скалата A-G
        /// </summary>
        public string ClassColor
        {
            get
            {
                if (!CalculatedClass.HasValue)
                    return "#CCCCCC"; // Сиво за неизвестен

                return CalculatedClass.Value switch
                {
                    EnergyClass.A => "#00A651", // Клас A - RGB: 0, 166, 81
                    EnergyClass.B => "#50B848", // Клас B - RGB: 80, 184, 72
                    EnergyClass.C => "#BFD730", // Клас C - RGB: 191, 215, 48
                    EnergyClass.D => "#FFF200", // Клас D - RGB: 255, 242, 0
                    EnergyClass.E => "#FDB913", // Клас E - RGB: 253, 185, 19
                    EnergyClass.F => "#F37021", // Клас F - RGB: 248, 112, 33
                    EnergyClass.G => "#ED1C24", // Клас G - RGB: 239, 28, 36
                    _ => "#CCCCCC"
                };
            }
        }

        /// <summary>
        /// Display name на типа сграда
        /// </summary>
        public string BuildingTypeDisplay
        {
            get
            {
                if (!BuildingType.HasValue)
                    return "Не е избран";

                var info = BuildingTypeInfo.GetByCode(BuildingType.Value);
                return info?.DisplayName ?? "Неизвестен";
            }
        }

        /// <summary>
        /// EP display стойност закръглена до цяло число (за маркера)
        /// </summary>
        public int? MarkerValueRounded
        {
            get
            {
                if (!EnergyPerformance.HasValue)
                    return null;
                
                return (int)Math.Round(EnergyPerformance.Value, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// EP display като текст
        /// </summary>
        public string EnergyPerformanceDisplay
        {
            get
            {
                if (!EnergyPerformance.HasValue)
                    return "—";
                
                return EnergyPerformance.Value.ToString("F2");
            }
        }

        /// <summary>
        /// Дали данните са налични за изчисление
        /// </summary>
        public bool IsDataAvailable => BuildingType.HasValue && EnergyPerformance.HasValue;

        /// <summary>
        /// Съобщение при липса на данни
        /// </summary>
        public string DataUnavailableMessage
        {
            get
            {
                if (!BuildingType.HasValue && !EnergyPerformance.HasValue)
                    return "Изберете тип сграда в секция 5 и въведете/изчислете данните в секция 'Резултати сграда'.";
                
                if (!BuildingType.HasValue)
                    return "Изберете тип сграда в секция 5 'Данни за обекта'.";
                
                if (!EnergyPerformance.HasValue)
                    return "Въведете/изчислете данните в секция 'Резултати сграда'.";
                
                return string.Empty;
            }
        }

        /// <summary>
        /// Обновява таблицата с прагове на база текущия тип сграда
        /// </summary>
        public void RefreshThresholds()
        {
            ThresholdRows.Clear();

            if (!BuildingType.HasValue)
                return;

            var thresholds = EnergyClassCalculator.GetThresholds(BuildingType.Value);
            if (thresholds == null)
                return;

            var rows = BuildThresholdRows(thresholds);
            foreach (var row in rows)
            {
                ThresholdRows.Add(row);
            }
        }

        /// <summary>
        /// Генерира 7 реда (A..G) с прагове.
        /// Също задава IsSelectedClass и MarkerText за текущия клас.
        /// </summary>
        private List<EnergyClassThresholdRow> BuildThresholdRows(EnergyClassThresholds thresholds)
        {
            var colorMap = new Dictionary<string, string>
            {
                ["A"] = "#00A651", // Клас A - RGB: 0, 166, 81
                ["B"] = "#50B848", // Клас B - RGB: 80, 184, 72
                ["C"] = "#BFD730", // Клас C - RGB: 191, 215, 48
                ["D"] = "#FFF200", // Клас D - RGB: 255, 242, 0
                ["E"] = "#FDB913", // Клас E - RGB: 253, 185, 19
                ["F"] = "#F37021", // Клас F - RGB: 248, 112, 33
                ["G"] = "#ED1C24"  // Клас G - RGB: 239, 28, 36
            };

            // Изчисли текущия клас и маркера
            var computedClass = CalculatedClass;
            var markerValue = MarkerValueRounded;

            var rows = new List<EnergyClassThresholdRow>
            {
                new EnergyClassThresholdRow
                {
                    Class = "A",
                    MinValue = null,
                    MaxValue = thresholds.A,
                    RuleText = $"EP < {thresholds.A}",
                    ColorHex = colorMap["A"]
                },
                new EnergyClassThresholdRow
                {
                    Class = "B",
                    MinValue = thresholds.A,
                    MaxValue = thresholds.B,
                    RuleText = $"{thresholds.A} ≤ EP < {thresholds.B}",
                    ColorHex = colorMap["B"]
                },
                new EnergyClassThresholdRow
                {
                    Class = "C",
                    MinValue = thresholds.B,
                    MaxValue = thresholds.C,
                    RuleText = $"{thresholds.B} ≤ EP < {thresholds.C}",
                    ColorHex = colorMap["C"]
                },
                new EnergyClassThresholdRow
                {
                    Class = "D",
                    MinValue = thresholds.C,
                    MaxValue = thresholds.D,
                    RuleText = $"{thresholds.C} ≤ EP < {thresholds.D}",
                    ColorHex = colorMap["D"]
                },
                new EnergyClassThresholdRow
                {
                    Class = "E",
                    MinValue = thresholds.D,
                    MaxValue = thresholds.E,
                    RuleText = $"{thresholds.D} ≤ EP < {thresholds.E}",
                    ColorHex = colorMap["E"]
                },
                new EnergyClassThresholdRow
                {
                    Class = "F",
                    MinValue = thresholds.E,
                    MaxValue = thresholds.F,
                    RuleText = $"{thresholds.E} ≤ EP < {thresholds.F}",
                    ColorHex = colorMap["F"]
                },
                new EnergyClassThresholdRow
                {
                    Class = "G",
                    MinValue = thresholds.F,
                    MaxValue = null,
                    RuleText = $"EP ≥ {thresholds.F}",
                    ColorHex = colorMap["G"]
                }
            };

            // Задай IsSelectedClass и MarkerText за правилния ред
            foreach (var row in rows)
            {
                bool isSelected = (computedClass.HasValue && row.Class == computedClass.Value.ToString());
                row.IsSelectedClass = isSelected;
                row.MarkerText = (isSelected && markerValue.HasValue) ? markerValue.Value.ToString() : string.Empty;
            }

            return rows;
        }

        /// <summary>
        /// Изчислява Y позицията на маркера в скалата (0.0 - 1.0 normalized)
        /// </summary>
        public double? GetNormalizedMarkerPosition()
        {
            if (!BuildingType.HasValue || !EnergyPerformance.HasValue || !CalculatedClass.HasValue)
                return null;

            var thresholds = EnergyClassCalculator.GetThresholds(BuildingType.Value);
            if (thresholds == null)
                return null;

            var ep = EnergyPerformance.Value;
            var classEnum = CalculatedClass.Value;
            int bandIndex = (int)classEnum; // A=0, B=1, ..., G=6

            double bandStart = bandIndex / 7.0;
            double bandHeight = 1.0 / 7.0;

            // Интерполация вътре в лентата
            double t = 0.5; // по подразбиране - център на лентата

            switch (classEnum)
            {
                case EnergyClass.A:
                    // Отворен край отгоре, нормализираме от 0 до A
                    if (thresholds.A > 0)
                        t = Math.Clamp(ep / thresholds.A, 0.0, 1.0);
                    break;

                case EnergyClass.B:
                    if (thresholds.B > thresholds.A)
                        t = Math.Clamp((ep - thresholds.A) / (thresholds.B - thresholds.A), 0.0, 1.0);
                    break;

                case EnergyClass.C:
                    if (thresholds.C > thresholds.B)
                        t = Math.Clamp((ep - thresholds.B) / (thresholds.C - thresholds.B), 0.0, 1.0);
                    break;

                case EnergyClass.D:
                    if (thresholds.D > thresholds.C)
                        t = Math.Clamp((ep - thresholds.C) / (thresholds.D - thresholds.C), 0.0, 1.0);
                    break;

                case EnergyClass.E:
                    if (thresholds.E > thresholds.D)
                        t = Math.Clamp((ep - thresholds.D) / (thresholds.E - thresholds.D), 0.0, 1.0);
                    break;

                case EnergyClass.F:
                    if (thresholds.F > thresholds.E)
                        t = Math.Clamp((ep - thresholds.E) / (thresholds.F - thresholds.E), 0.0, 1.0);
                    break;

                case EnergyClass.G:
                    // Отворен край отдолу, нормализираме пропорционално
                    // Давам малка допълнителна скала над F
                    var range = thresholds.F * 0.5; // 50% допълнително над F
                    if (range > 0)
                        t = Math.Clamp((ep - thresholds.F) / range, 0.0, 1.0);
                    break;
            }

            return bandStart + t * bandHeight;
        }

        // Notify calculated properties when dependencies change
        partial void OnBuildingTypeChanged(BuildingTypeCode? value)
        {
            RefreshThresholds();
            OnPropertyChanged(nameof(CalculatedClass));
            OnPropertyChanged(nameof(ClassDescription));
            OnPropertyChanged(nameof(ClassDisplay));
            OnPropertyChanged(nameof(ClassColor));
            OnPropertyChanged(nameof(BuildingTypeDisplay));
            OnPropertyChanged(nameof(IsDataAvailable));
            OnPropertyChanged(nameof(DataUnavailableMessage));
            OnPropertyChanged(nameof(GetNormalizedMarkerPosition));
        }

        partial void OnEnergyPerformanceChanged(double? value)
        {
            RefreshThresholds();
            OnPropertyChanged(nameof(CalculatedClass));
            OnPropertyChanged(nameof(ClassDescription));
            OnPropertyChanged(nameof(ClassDisplay));
            OnPropertyChanged(nameof(ClassColor));
            OnPropertyChanged(nameof(MarkerValueRounded));
            OnPropertyChanged(nameof(EnergyPerformanceDisplay));
            OnPropertyChanged(nameof(IsDataAvailable));
            OnPropertyChanged(nameof(DataUnavailableMessage));
            OnPropertyChanged(nameof(GetNormalizedMarkerPosition));
        }
    }
}
