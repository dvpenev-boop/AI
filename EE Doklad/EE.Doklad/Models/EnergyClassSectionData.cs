using CommunityToolkit.Mvvm.ComponentModel;
using EE.Doklad.Services;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Данни за секция "Клас на енергопотребление"
    /// </summary>
    public partial class EnergyClassSectionData : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Клас на енергопотребление";

        [ObservableProperty]
        private string? _description;

        /// <summary>
        /// Тип на сградата (за определяне на праговете)
        /// </summary>
        [ObservableProperty]
        private BuildingTypeCode? _buildingType;

        /// <summary>
        /// EP - Годишна специфична енергия [kWh/m²]
        /// </summary>
        [ObservableProperty]
        private double? _energyPerformance;

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
        /// Информация за праговете на избрания тип сграда
        /// </summary>
        public string ThresholdsInfo
        {
            get
            {
                if (!BuildingType.HasValue)
                    return string.Empty;

                var thresholds = EnergyClassCalculator.GetThresholds(BuildingType.Value);
                if (thresholds == null)
                    return string.Empty;

                return $"A: EP < {thresholds.A} kWh/m²\n" +
                       $"B: {thresholds.A} ≤ EP < {thresholds.B} kWh/m²\n" +
                       $"C: {thresholds.B} ≤ EP < {thresholds.C} kWh/m²\n" +
                       $"D: {thresholds.C} ≤ EP < {thresholds.D} kWh/m²\n" +
                       $"E: {thresholds.D} ≤ EP < {thresholds.E} kWh/m²\n" +
                       $"F: {thresholds.E} ≤ EP < {thresholds.F} kWh/m²\n" +
                       $"G: EP ≥ {thresholds.F} kWh/m²";
            }
        }

        // Notify calculated properties when dependencies change
        partial void OnBuildingTypeChanged(BuildingTypeCode? value)
        {
            OnPropertyChanged(nameof(CalculatedClass));
            OnPropertyChanged(nameof(ClassDescription));
            OnPropertyChanged(nameof(ClassDisplay));
            OnPropertyChanged(nameof(BuildingTypeDisplay));
            OnPropertyChanged(nameof(ThresholdsInfo));
        }

        partial void OnEnergyPerformanceChanged(double? value)
        {
            OnPropertyChanged(nameof(CalculatedClass));
            OnPropertyChanged(nameof(ClassDescription));
            OnPropertyChanged(nameof(ClassDisplay));
        }
    }
}
