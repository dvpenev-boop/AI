using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Данни за раздел №14 - "Топла вода за битови нужди (БГВ)"
    /// </summary>
    public partial class HotWaterSectionData : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Топла вода за битови нужди (БГВ)";

        [ObservableProperty]
        private string? _description = "Попълнете данните за секция: 14. Топла вода за битови нужди (БГВ)";

        // ========== EDITABLE FIELDS ==========

        /// <summary>
        /// Разход на вода (смесена) на човек [l/d човек]
        /// </summary>
        [ObservableProperty]
        private double _waterConsumptionPerPerson = 10.0;

        /// <summary>
        /// Температурна разлика [°C]
        /// </summary>
        [ObservableProperty]
        private double _temperatureDifference = 30.0;

        /// <summary>
        /// Ефективност на разпределителната мрежа [%]
        /// </summary>
        [ObservableProperty]
        private double _distributionNetworkEfficiency = 97.0;

        /// <summary>
        /// Автоматизирано управление [%]
        /// </summary>
        [ObservableProperty]
        private double _automatedControl = 97.0;

        /// <summary>
        /// Енергиен мениджмънт [%]
        /// </summary>
        [ObservableProperty]
        private double _energyManagement = 96.0;

    // Guard to avoid re-entrant setter calls when clamping percentage properties
    private bool _suppressEfficiencyClamp = false;

        /// <summary>
        /// КПД на топлоснабдяване [%]
        /// </summary>
        [ObservableProperty]
        private double _heatSupplyEfficiency = 100.0;

        // ── Регенерируеми загуби към зоната (Секция 23 → Q_spec;int;WA) ─────

        /// <summary>
        /// Режим за изчисляване на регенерируемите загуби:
        ///   Manual      – ръчно въведена kWh/год стойност
        ///   Automatic   – автоматично по методиката (тръбни сегменти)
        ///   PercentShare – % дял от общите загуби
        /// </summary>
        [ObservableProperty]
        private DhwLossMode _recoverableLossMode = DhwLossMode.Manual;

        /// <summary>
        /// Годишни регенерируеми загуби от ВиК системата към отопляваната зона [kWh/год].
        /// Директна стойност (ако е зададена).
        /// Използва се в Секция 23 като компонент Q_spec;int;WA.
        /// </summary>
        [ObservableProperty]
        private double _recoverableHeatToZone_kWh = 0.0;

        /// <summary>
        /// Дял на регенерируемите загуби [%] от общите системни загуби.
        /// Алтернативен начин за задаване. Ако RecoverableHeatToZone_kWh == 0
        /// и RecoverableFraction_pct > 0, стойността се изчислява като:
        ///   RecoverableHeatToZone_kWh = TotalSystemLosses_kWh * RecoverableFraction_pct / 100
        /// </summary>
        [ObservableProperty]
        private double _recoverableFraction_pct = 0.0;

        /// <summary>
        /// Ефективна стойност на регенерируемите загуби [kWh/год] –
        /// използва директната стойност или изчисляването от %.
        /// </summary>
        public double EffectiveRecoverableHeat_kWh
        {
            get
            {
                if (RecoverableHeatToZone_kWh > 0.0)
                    return RecoverableHeatToZone_kWh;

                if (RecoverableFraction_pct > 0.0)
                {
                    // Приближение на системните загуби от Energy_kWh_per_y
                    double systemLosses = Energy_kWh_per_y * (1.0 - HeatSupplyEfficiency / 100.0);
                    return systemLosses * RecoverableFraction_pct / 100.0;
                }

                return 0.0;
            }
        }

        partial void OnRecoverableHeatToZone_kWhChanged(double value)
        {
            OnPropertyChanged(nameof(EffectiveRecoverableHeat_kWh));
        }

        partial void OnRecoverableFraction_pctChanged(double value)
        {
            OnPropertyChanged(nameof(EffectiveRecoverableHeat_kWh));
        }

        // ========== READ-ONLY FIELDS (от Section 5) ==========

        private int _numberOfPeople;
        private double _heatedArea_m2;
        private int _holidaysPerYear;
    private int _daysPerWeek = 7; // default 7 (can be overridden by provider)

        /// <summary>
        /// Брой хора (от секция 5)
        /// </summary>
        public int NumberOfPeople
        {
            get => _numberOfPeople;
            private set
            {
                if (SetProperty(ref _numberOfPeople, value))
                {
                    OnPropertyChanged(nameof(TotalConsumption_l_per_m2a));
                    OnPropertyChanged(nameof(Energy_kWh_per_y));
                }
            }
        }

        /// <summary>
        /// Отопляема площ [m²] (от секция 5)
        /// </summary>
        public double HeatedArea_m2
        {
            get => _heatedArea_m2;
            private set
            {
                if (SetProperty(ref _heatedArea_m2, value))
                {
                    OnPropertyChanged(nameof(TotalConsumption_l_per_m2a));
                    OnPropertyChanged(nameof(Energy_kWh_per_y));
                }
            }
        }

        /// <summary>
        /// Брой дни [d/y] - изчислява се по формулата от секции 15/16/17
        /// WorkingDaysPerYear = (365 / 7.0) * 7 - HolidaysPerYear
        /// За БГВ винаги използваме 7 дни в седмицата (24/7 консумация)
        /// </summary>
        public double WorkingDaysPerYear
        {
            get
            {
                double daysPerWeek = _daysPerWeek;
                double workingDaysWithoutHolidays = (365.0 / 7.0) * daysPerWeek;
                double result = workingDaysWithoutHolidays - _holidaysPerYear;
                return result >= 0 ? result : 0.0;
            }
        }

        // ========== COMPUTED FIELDS ==========

        /// <summary>
        /// Обща консумация [l/m²a]
        /// Формула: WaterConsumptionPerPerson * NumberOfPeople * WorkingDaysPerYear / HeatedArea_m2
        /// </summary>
        public double TotalConsumption_l_per_m2a
        {
            get
            {
                if (HeatedArea_m2 <= 0)
                    return 0.0;

                return (WaterConsumptionPerPerson * NumberOfPeople * WorkingDaysPerYear) / HeatedArea_m2;
            }
        }

        /// <summary>
        /// Енергия [kWh/y]
        /// Формула: WaterConsumptionPerPerson * NumberOfPeople * WorkingDaysPerYear * TemperatureDifference * 0.00116
        ///          / ((DistributionNetworkEfficiency * AutomatedControl * EnergyManagement * HeatSupplyEfficiency) / 100000000)
        /// </summary>
        public double Energy_kWh_per_y
        {
            get
            {
                // Проверка за деление на нула
                double efficiencyProduct = (DistributionNetworkEfficiency * AutomatedControl * EnergyManagement * HeatSupplyEfficiency);
                if (efficiencyProduct <= 0)
                    return 0.0;

                double numerator = WaterConsumptionPerPerson * NumberOfPeople * WorkingDaysPerYear * TemperatureDifference * 0.00116;
                double denominator = efficiencyProduct / 100000000.0; // Нормализация на 4 процента (100^4 = 100000000)

                return numerator / denominator;
            }
        }

        // ========== PARTIAL METHODS FOR PROPERTY CHANGES ==========

        partial void OnWaterConsumptionPerPersonChanged(double value)
        {
            OnPropertyChanged(nameof(TotalConsumption_l_per_m2a));
            OnPropertyChanged(nameof(Energy_kWh_per_y));
        }

        partial void OnTemperatureDifferenceChanged(double value)
        {
            OnPropertyChanged(nameof(Energy_kWh_per_y));
        }

        partial void OnDistributionNetworkEfficiencyChanged(double value)
        {
            if (_suppressEfficiencyClamp) { OnPropertyChanged(nameof(Energy_kWh_per_y)); return; }

            // Clamp to [0,100]
            var clamped = value;
            if (double.IsNaN(clamped) || double.IsInfinity(clamped)) clamped = 0.0;
            if (clamped < 0.0) clamped = 0.0;
            if (clamped > 100.0) clamped = 100.0;

            if (clamped != value)
            {
                try
                {
                    _suppressEfficiencyClamp = true;
                    DistributionNetworkEfficiency = clamped;
                }
                finally { _suppressEfficiencyClamp = false; }
                return;
            }

            OnPropertyChanged(nameof(Energy_kWh_per_y));
        }

        partial void OnAutomatedControlChanged(double value)
        {
            if (_suppressEfficiencyClamp) { OnPropertyChanged(nameof(Energy_kWh_per_y)); return; }

            var clamped = value;
            if (double.IsNaN(clamped) || double.IsInfinity(clamped)) clamped = 0.0;
            if (clamped < 0.0) clamped = 0.0;
            if (clamped > 100.0) clamped = 100.0;

            if (clamped != value)
            {
                try
                {
                    _suppressEfficiencyClamp = true;
                    AutomatedControl = clamped;
                }
                finally { _suppressEfficiencyClamp = false; }
                return;
            }

            OnPropertyChanged(nameof(Energy_kWh_per_y));
        }

        partial void OnEnergyManagementChanged(double value)
        {
            if (_suppressEfficiencyClamp) { OnPropertyChanged(nameof(Energy_kWh_per_y)); return; }

            var clamped = value;
            if (double.IsNaN(clamped) || double.IsInfinity(clamped)) clamped = 0.0;
            if (clamped < 0.0) clamped = 0.0;
            if (clamped > 100.0) clamped = 100.0;

            if (clamped != value)
            {
                try
                {
                    _suppressEfficiencyClamp = true;
                    EnergyManagement = clamped;
                }
                finally { _suppressEfficiencyClamp = false; }
                return;
            }

            OnPropertyChanged(nameof(Energy_kWh_per_y));
        }

        partial void OnHeatSupplyEfficiencyChanged(double value)
        {
            OnPropertyChanged(nameof(Energy_kWh_per_y));
        }

        // ========== PUBLIC METHODS FOR DATA SYNCHRONIZATION ==========

        /// <summary>
        /// Задава броя хора от секция 5 "Данни за обекта"
        /// </summary>
        public void SetNumberOfPeople(int people)
        {
            NumberOfPeople = people;
        }

        /// <summary>
        /// Задава отопляемата площ от секция 5 "Данни за обекта"
        /// </summary>
        public void SetHeatedArea(double area_m2)
        {
            HeatedArea_m2 = area_m2;
        }

        /// <summary>
        /// Задава броя почивни дни от секция 5 "Данни за обекта"
        /// </summary>
        public void SetHolidaysPerYear(int holidays)
        {
            if (_holidaysPerYear != holidays)
            {
                _holidaysPerYear = holidays;
                OnPropertyChanged(nameof(WorkingDaysPerYear));
                OnPropertyChanged(nameof(TotalConsumption_l_per_m2a));
                OnPropertyChanged(nameof(Energy_kWh_per_y));
            }
        }

        /// <summary>
        /// Задава дни в седмицата (5/6/7) от външен provider
        /// </summary>
        public void SetDaysPerWeek(int days)
        {
            if (days < 1 || days > 7) return;
            if (_daysPerWeek != days)
            {
                _daysPerWeek = days;
                OnPropertyChanged(nameof(WorkingDaysPerYear));
                OnPropertyChanged(nameof(TotalConsumption_l_per_m2a));
                OnPropertyChanged(nameof(Energy_kWh_per_y));
            }
        }
    }
}
