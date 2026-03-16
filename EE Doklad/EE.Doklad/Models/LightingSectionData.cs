using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Данни за раздел №15 - "Осветление"
    /// </summary>
    public partial class LightingSectionData : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Осветление";

        [ObservableProperty]
        private string? _description;

        /// <summary>
        /// Линия items (редове в таблицата)
        /// </summary>
        public ObservableCollection<LightingLineItem> LineItems { get; } = new();

        /// <summary>
        /// Обща мощност [kW] = Σ PowerTotal_kW от всички редове
        /// </summary>
        public double TotalPower_kW
        {
            get
            {
                if (LineItems == null || LineItems.Count == 0)
                    return 0.0;
                return LineItems.Sum(item => item.PowerTotal_kW);
            }
        }

        /// <summary>
        /// Средно работен режим [h/day]
        /// </summary>
        public double AverageHoursPerDay
        {
            get
            {
                if (LineItems == null || LineItems.Count == 0)
                    return 0.0;
                return LineItems.Average(item => item.HoursPerDay);
            }
        }

        /// <summary>
        /// Средно дни седмично [days/week]
        /// </summary>
        public double AverageDaysPerWeek
        {
            get
            {
                if (LineItems == null || LineItems.Count == 0)
                    return 0.0;
                return LineItems.Average(item => item.DaysPerWeek);
            }
        }

        public double RoundedWorkRegimeHoursPerWeek
        {
            get
            {
                var weeklyHours = AverageHoursPerDay * AverageDaysPerWeek;
                if (weeklyHours <= 0) return 0.0;
                return Math.Round(weeklyHours);
            }
        }

        /// <summary>
        /// Обща консумирана енергия [kWh/y] = Σ AnnualEnergy_kWh от всички редове
        /// </summary>
        public double TotalAnnualEnergy_kWh
        {
            get
            {
                if (LineItems == null || LineItems.Count == 0)
                    return 0.0;
                return LineItems.Sum(item => item.AnnualEnergy_kWh);
            }
        }

        /// <summary>
        /// Едновременна мощност [W/m²]
        /// Формула:
        /// SimultaneousPower_W_per_m2 = (TotalAnnualEnergy_kWh * 1000.0) / (((365.0 - HolidaysPerYear) / 7.0) * RoundedWorkRegime * HeatedArea_m2)
        /// където RoundedWorkRegime = round(AverageHoursPerDay * AverageDaysPerWeek)
        /// </summary>
        public double SimultaneousPower_W_per_m2
        {
            get
            {
                return CalculateSimultaneousPower_W_per_m2(
                    TotalAnnualEnergy_kWh,
                    _holidaysPerYear,
                    _heatedArea_m2,
                    AverageHoursPerDay,
                    AverageDaysPerWeek
                );
            }
        }

        /// <summary>
        /// Едновременна мощност [W] = SimultaneousPower_W_per_m2 * HeatedArea_m2
        /// </summary>
        public double SimultaneousPower_W
        {
            get
            {
                if (_heatedArea_m2 <= 0) return 0.0;
                return SimultaneousPower_W_per_m2 * _heatedArea_m2;
            }
        }

        // ========== Външни данни от секция 5 "Данни за обекта" ==========
        private int _holidaysPerYear;
        private double _heatedArea_m2;
        private int _occupancyWorkingDaysPerYear;

        /// <summary>
        /// Задава HolidaysPerYear от Секция 5 "Данни за обекта"
        /// </summary>
        public void SetHolidaysPerYear(int holidays)
        {
            // Debug logging
            System.Diagnostics.Debug.WriteLine($"[LightingSectionData] SetHolidaysPerYear called with: {holidays}");
            
            if (_holidaysPerYear != holidays)
            {
                _holidaysPerYear = holidays;
                
                // Актуализираме всички съществуващи редове
                foreach (var item in LineItems)
                {
                    item.SetHolidaysPerYear(_holidaysPerYear);
                }
                
                OnPropertyChanged(nameof(SimultaneousPower_W_per_m2));
                OnPropertyChanged(nameof(SimultaneousPower_W));
            }
        }

        /// <summary>
        /// Задава HeatedArea_m2 от Секция 5 "Данни за обекта"
        /// </summary>
        public void SetHeatedArea(double area)
        {
            if (Math.Abs(_heatedArea_m2 - area) > 0.001)
            {
                _heatedArea_m2 = area;
                OnPropertyChanged(nameof(SimultaneousPower_W_per_m2));
                OnPropertyChanged(nameof(SimultaneousPower_W));

                // Уведомяваме всеки ред за промяна в отопляемата площ (ако имат калкулации)
                foreach (var item in LineItems)
                {
                    item.SetHolidaysPerYear(_holidaysPerYear);
                }
            }
        }

        public void SetOccupancyWorkingDaysPerYear(int workingDaysPerYear)
        {
            if (_occupancyWorkingDaysPerYear == workingDaysPerYear)
            {
                return;
            }

            _occupancyWorkingDaysPerYear = workingDaysPerYear;

            foreach (var item in LineItems)
            {
                item.SetOccupancyWorkingDaysPerYear(_occupancyWorkingDaysPerYear);
            }
        }

        public LightingSectionData()
        {
            LineItems.CollectionChanged += LineItems_CollectionChanged;
        }

        private void LineItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (LightingLineItem item in e.OldItems)
                {
                    item.PropertyChanged -= LineItem_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (LightingLineItem item in e.NewItems)
                {
                    item.PropertyChanged += LineItem_PropertyChanged;
                    // Set external params
                    item.SetHolidaysPerYear(_holidaysPerYear);
                    item.SetOccupancyWorkingDaysPerYear(_occupancyWorkingDaysPerYear);
                }
            }

            NotifyCalculatedProperties();
        }

        private void LineItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Ако се промени някое от изчислимите свойства на ред, уведомяваме общите суми
            if (e.PropertyName == nameof(LightingLineItem.PowerTotal_kW) ||
                e.PropertyName == nameof(LightingLineItem.AnnualEnergy_kWh))
            {
                NotifyCalculatedProperties();
            }
        }

        private void NotifyCalculatedProperties()
        {
            OnPropertyChanged(nameof(TotalPower_kW));
            OnPropertyChanged(nameof(AverageHoursPerDay));
            OnPropertyChanged(nameof(AverageDaysPerWeek));
            OnPropertyChanged(nameof(RoundedWorkRegimeHoursPerWeek));
            OnPropertyChanged(nameof(TotalAnnualEnergy_kWh));
            OnPropertyChanged(nameof(SimultaneousPower_W_per_m2));
            OnPropertyChanged(nameof(SimultaneousPower_W));
        }

        /// <summary>
        /// Изчисление на едновременна мощност [W/m²] по зададената формула
        /// </summary>
        private static double CalculateSimultaneousPower_W_per_m2(
            double totalAnnualEnergy_kWh,
            int holidaysPerYear,
            double heatedArea_m2,
            double defaultHoursPerDay,
            double defaultDaysPerWeek)
        {
            // Валидации
            if (totalAnnualEnergy_kWh <= 0) return 0.0;
            if (heatedArea_m2 <= 0) return 0.0;
            if (defaultHoursPerDay <= 0) return 0.0;
            if (defaultDaysPerWeek <= 0) return 0.0;
            if (holidaysPerYear < 0) holidaysPerYear = 0;
            if (holidaysPerYear > 365) holidaysPerYear = 365;

            // RoundedWorkRegime = round(defaultHoursPerDay * defaultDaysPerWeek)
            var roundedWorkRegime = Math.Round(defaultHoursPerDay * defaultDaysPerWeek);
            if (roundedWorkRegime <= 0) return 0.0;

            // Знаменател = ((365 - holidaysPerYear) / 7.0) * roundedWorkRegime * heatedArea_m2
            var denominator = ((365.0 - holidaysPerYear) / 7.0) * roundedWorkRegime * heatedArea_m2;
            if (denominator <= 0) return 0.0;

            // SimultaneousPower_W_per_m2 = (totalAnnualEnergy_kWh * 1000.0) / denominator
            return (totalAnnualEnergy_kWh * 1000.0) / denominator;
        }
    }

    /// <summary>
    /// Ред в таблицата "Описание на осветителната инсталация на сградата"
    /// </summary>
    public partial class LightingLineItem : ObservableObject
    {
        [ObservableProperty]
        private int _index;

        /// <summary>
        /// Име на избрания осветител от ComboBox
        /// </summary>
        [ObservableProperty]
        private string? _selectedLightingComponentName;

        /// <summary>
        /// Мощност [W] - автоматично попълнена от избрания компонент
        /// </summary>
        [ObservableProperty]
        private double _powerW;

        /// <summary>
        /// Количество [бр.]
        /// </summary>
        [ObservableProperty]
        private int _quantity = 1;

        /// <summary>
        /// Мощност общо [kW] = (PowerW * Quantity) / 1000
        /// </summary>
        public double PowerTotal_kW
        {
            get
            {
                return (PowerW * Quantity) / 1000.0;
            }
        }

        /// <summary>
        /// Работен режим [h/day]
        /// </summary>
        [ObservableProperty]
        private double _hoursPerDay = 5.0;

        /// <summary>
        /// Дни седмично [days/week]
        /// </summary>
        [ObservableProperty]
        private double _daysPerWeek = 5.0;

        /// <summary>
        /// Работен режим [days/y] = (365 / 7.0) * DaysPerWeek - HolidaysPerYear
        /// Предполага че HolidaysPerYear съдържа САМО официални празници (без уикенди)
        /// Уикендите се изчисляват автоматично чрез DaysPerWeek (5 дни = понеделник-петък)
        /// </summary>
        public double WorkingDaysPerYear
        {
            get
            {
                if (_occupancyWorkingDaysPerYear.HasValue)
                    return _occupancyWorkingDaysPerYear.Value >= 0 ? _occupancyWorkingDaysPerYear.Value : 0.0;

                if (DaysPerWeek < 0 || DaysPerWeek > 7) return 0.0;
                
                // Работни дни в годината (автоматично изключва уикендите)
                double workingDaysWithoutHolidays = (365.0 / 7.0) * DaysPerWeek;
                // За DaysPerWeek=5: (365/7) × 5 = 52.14 × 5 = 260.71 дни
                
                // Изваждаме официалните празници
                double result = workingDaysWithoutHolidays - _holidaysPerYear;
                // За HolidaysPerYear=21: 260.71 - 21 = 239.71 дни
                
                return result >= 0 ? result : 0.0;
            }
        }

        /// <summary>
        /// Ke (0..1) - коефициент
        /// </summary>
        [ObservableProperty]
        private double _ke = 0.6;

        /// <summary>
        /// Консумирана енергия [kWh/y] = PowerTotal_kW * HoursPerDay * WorkingDaysPerYear * Ke
        /// </summary>
        public double AnnualEnergy_kWh
        {
            get
            {
                return PowerTotal_kW * HoursPerDay * WorkingDaysPerYear * Ke;
            }
        }

        private int _holidaysPerYear;
        private int? _occupancyWorkingDaysPerYear;

        /// <summary>
        /// Задава HolidaysPerYear от секция "Данни за обекта"
        /// </summary>
        public void SetHolidaysPerYear(int holidays)
        {
            // Debug logging
            System.Diagnostics.Debug.WriteLine($"[LightingLineItem #{Index}] SetHolidaysPerYear called with: {holidays}");
            
            if (_holidaysPerYear != holidays)
            {
                _holidaysPerYear = holidays;
                System.Diagnostics.Debug.WriteLine($"[LightingLineItem #{Index}] _holidaysPerYear updated to: {_holidaysPerYear}, WorkingDaysPerYear will be: {WorkingDaysPerYear}");
                OnPropertyChanged(nameof(WorkingDaysPerYear));
                OnPropertyChanged(nameof(AnnualEnergy_kWh));
            }
        }

        public void SetOccupancyWorkingDaysPerYear(int workingDaysPerYear)
        {
            if (_occupancyWorkingDaysPerYear == workingDaysPerYear)
            {
                return;
            }

            _occupancyWorkingDaysPerYear = workingDaysPerYear;
            OnPropertyChanged(nameof(WorkingDaysPerYear));
            OnPropertyChanged(nameof(AnnualEnergy_kWh));
        }

        partial void OnSelectedLightingComponentNameChanged(string? value)
        {
            // Не правим нищо тук - попълването на PowerW се случва в code-behind
        }

        partial void OnPowerWChanged(double value)
        {
            OnPropertyChanged(nameof(PowerTotal_kW));
            OnPropertyChanged(nameof(AnnualEnergy_kWh));
        }

        partial void OnQuantityChanged(int value)
        {
            OnPropertyChanged(nameof(PowerTotal_kW));
            OnPropertyChanged(nameof(AnnualEnergy_kWh));
        }

        partial void OnHoursPerDayChanged(double value)
        {
            double clampedValue = Math.Clamp(value, 0.0, 24.0);
            if (Math.Abs(clampedValue - value) > 0.0001)
            {
                HoursPerDay = clampedValue;
                return;
            }

            OnPropertyChanged(nameof(AnnualEnergy_kWh));
        }

        partial void OnDaysPerWeekChanged(double value)
        {
            double clampedValue = Math.Clamp(value, 0.0, 7.0);
            if (Math.Abs(clampedValue - value) > 0.0001)
            {
                DaysPerWeek = clampedValue;
                return;
            }

            OnPropertyChanged(nameof(WorkingDaysPerYear));
            OnPropertyChanged(nameof(AnnualEnergy_kWh));
        }

        partial void OnKeChanged(double value)
        {
            OnPropertyChanged(nameof(AnnualEnergy_kWh));
        }
    }
}
