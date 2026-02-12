using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Данни за раздел №12 - "Вентилация"
    /// Съгласно Наредба RD-02-20-3
    /// </summary>
    public partial class VentilationSectionData : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Вентилация";

        [ObservableProperty]
        private string? _description = "Попълнете данните за секция: 12. Вентилация";

        // ========== РЪЧНИ ВХОДОВЕ ==========

        /// <summary>
        /// Работен режим [h/week]
        /// </summary>
        [ObservableProperty]
        private double _operatingHoursPerWeek = 0.0;

        /// <summary>
        /// Дебит [m³/h] на m²
        /// Въздушен дебит на единица отопляема площ
        /// </summary>
        [ObservableProperty]
        private double _airflowRatePerM2 = 0.0;

        /// <summary>
        /// Температура на подаване [°C]
        /// Входна температура на приточния въздух (след рекуперация)
        /// </summary>
        [ObservableProperty]
        private double _supplyTemperature = 0.0;

    /// <summary>
    /// Флаг дали температурата на подаване е въведена ръчно от потребителя
    /// Ако е true - използваме тази стойност; ако е false - калкулаторът може да изчисли Tsup
    /// </summary>
    [ObservableProperty]
    private bool _supplyTemperatureIsUserDefined = false;

        /// <summary>
        /// Относителна влажност на подавания въздух [%] (0-100)
        /// </summary>
        [ObservableProperty]
        private double _relativeHumidity = 0.0;

    /// <summary>
    /// Режим на изчисление за вентилация-охлаждане (3.11.2 или 3.11.3)
    /// </summary>
    [ObservableProperty]
    private VentilationCoolingCalculationMode _coolingCalculationMode = VentilationCoolingCalculationMode.FreshAirProcessed3113;

    /// <summary>
    /// Дял рециркулация [%] (0-100)
    /// </summary>
    [ObservableProperty]
    private double _recirculationPercent = 0.0;

        /// <summary>
        /// Ефективност на първа степен на рекуперация [%] (0-100)
        /// Термична ефективност на топлообменника за рекуперация
        /// </summary>
        [ObservableProperty]
        private double _firstStageRecuperationEfficiency = 0.0;

        /// <summary>
        /// Ефективност на втора степен на рекуперация [%] (0-100)
        /// Допълнителна ефективност на втората степен на загряване
        /// </summary>
        [ObservableProperty]
        private double _secondStageRecuperationEfficiency = 0.0;

        /// <summary>
        /// Макс. разлика на температура при загряване на въздуха
        /// във втора степен (от 4 до 8) [°C]
        /// </summary>
        [ObservableProperty]
        private double _maxTemperatureDifferenceSecondStage = 4.0;

        /// <summary>
        /// Мин. крайна температура на отработения въздух (от 3 до 5) [°C]
        /// </summary>
        [ObservableProperty]
        private double _minExhaustAirTemperature = 3.0;

        // ========== ЕНЕРГИЕН ИЗТОЧНИК ==========

        /// <summary>
        /// Енергиен източник 1 (ЕИ1)
        /// </summary>
        [ObservableProperty]
        private VentilationEnergySource _energySource1 = new();

        /// <summary>
        /// Енергиен източник 2 (ЕИ2) - опционален
        /// </summary>
        [ObservableProperty]
        private VentilationEnergySource? _energySource2 = null;

        /// <summary>
        /// Използва ли се втори енергиен източник
        /// </summary>
        [ObservableProperty]
        private bool _useSecondEnergySource = false;

        // ========== ОБЩИ ПАРАМЕТРИ ==========

        /// <summary>
        /// Обща ефективност на генериране на топлина [%] (0-100+)
        /// Може да бъде над 100% за термопомпи
        /// Default = 100%
        /// </summary>
        [ObservableProperty]
        private double _overallHeatGenerationEfficiency = 100.0;

        /// <summary>
        /// Принос към отоплението [kWh/m²]
        /// Изчислена стойност
        /// </summary>
        [ObservableProperty]
        private double _heatingContribution = 0.0;

        /// <summary>
        /// Обща потребна енергия [kWh/m²]
        /// Финална изчислена стойност
        /// </summary>
        [ObservableProperty]
        private double _totalRequiredEnergy = 0.0;

        // ========== READ-ONLY FIELDS (от други секции) ==========

        private double _heatedArea_m2;
        private double _indoorTemperature_C = 20.0;

        /// <summary>
        /// Отопляема площ [m²] (от секция 5)
        /// </summary>
        public double HeatedArea_m2
        {
            get => _heatedArea_m2;
            set
            {
                if (SetProperty(ref _heatedArea_m2, value))
                {
                    OnPropertyChanged(nameof(HeatedArea_m2));
                }
            }
        }

        /// <summary>
        /// Вътрешна референтна температура [°C] (от секция 10 - Отопление)
        /// </summary>
        public double IndoorTemperature_C
        {
            get => _indoorTemperature_C;
            set
            {
                if (SetProperty(ref _indoorTemperature_C, value))
                {
                    OnPropertyChanged(nameof(IndoorTemperature_C));
                }
            }
        }

        /// <summary>
        /// Инициализира източниците на енергия с дефолтни стойности
        /// </summary>
        public VentilationSectionData()
        {
            EnergySource1 = new VentilationEnergySource
            {
                Type = EnergySourceType.Electricity,
                Share = 100.0
            };
        }
    }
}
