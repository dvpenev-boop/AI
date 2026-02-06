using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Diagnostics;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Данни за раздел №5 - "Данни за обекта"
    /// </summary>
    public partial class ObjectDataSectionData : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Данни за обекта";

        [ObservableProperty]
        private string? _description;

        [ObservableProperty]
        private string? _buildingName;

        [ObservableProperty]
        private string? _address;

        [ObservableProperty]
        private string? _buildingType;

        /// <summary>
        /// Тип сграда (код) - нов подход с enum
        /// </summary>
        [ObservableProperty]
        private BuildingTypeCode? _buildingTypeCode;

        [ObservableProperty]
        private string? _ownership;

        [ObservableProperty]
        private string? _yearOfConstruction;

        [ObservableProperty]
        private string? _numberOfOccupants;

    [ObservableProperty]
    private string? _occupancySchedule;

    [ObservableProperty]
    private string? _heatingSchedule;
        
        // График на обитаване - отделни колони (работни дни / събота / неделя)
        [ObservableProperty]
        private string? _occupancyWorkdaysHours;

        [ObservableProperty]
        private string? _occupancySaturdayHours;

        [ObservableProperty]
        private string? _occupancySundayHours;

        // График на отопление - отделни колони (работни дни / събота / неделя)
        [ObservableProperty]
        private string? _heatingWorkdaysHours;

        [ObservableProperty]
        private string? _heatingSaturdayHours;

        [ObservableProperty]
        private string? _heatingSundayHours;

    // График на охлаждане - отделни колони (работни дни / събота / неделя)
    [ObservableProperty]
    private string? _coolingWorkdaysHours;

    [ObservableProperty]
    private string? _coolingSaturdayHours;

    [ObservableProperty]
    private string? _coolingSundayHours;

    // Охладителен сезон - само месец и ден (imaginary period, no year)
    [ObservableProperty]
    private int? _coolingSeasonStartDay;

    [ObservableProperty]
    private int? _coolingSeasonStartMonth;

    [ObservableProperty]
    private int? _coolingSeasonEndDay;

    [ObservableProperty]
    private int? _coolingSeasonEndMonth;

    // График на вентилация - отделни колони (работни дни / събота / неделя)
    [ObservableProperty]
    private string? _ventilationWorkdaysHours;

    [ObservableProperty]
    private string? _ventilationSaturdayHours;

    [ObservableProperty]
    private string? _ventilationSundayHours;

    // График Вентилация Охлаждане - отделни колони (работни дни / събота / неделя)
    [ObservableProperty]
    private string? _ventilationCoolingWorkdaysHours;

    [ObservableProperty]
    private string? _ventilationCoolingSaturdayHours;

    [ObservableProperty]
    private string? _ventilationCoolingSundayHours;

        [ObservableProperty]
        private int _climateZone = 1; // Климатична зона (1-9)

        // ========== ПЛОЩИ И ОБЕМИ ==========

        /// <summary>
        /// Застроена площ [m²]
        /// </summary>
        [ObservableProperty]
        private string? _builtUpArea;

        /// <summary>
        /// Разгъната застроена площ (РЗП) [m²]
        /// </summary>
        [ObservableProperty]
        private string? _totalFloorArea;

        /// <summary>
        /// Отопляема площ [m²]
        /// </summary>
        [ObservableProperty]
        private string? _heatedArea;

        /// <summary>
        /// Нетен отопляем обем [m³]
        /// </summary>
        [ObservableProperty]
        private string? _netHeatedVolume;

        /// <summary>
        /// Брутен отопляем обем [m³]
        /// </summary>
        [ObservableProperty]
        private string? _grossHeatedVolume;

        /// <summary>
        /// Охлаждаема площ [m²]
        /// </summary>
        [ObservableProperty]
        private string? _cooledArea;

        /// <summary>
        /// Нетен охлаждаем обем [m³]
        /// </summary>
        [ObservableProperty]
        private string? _netCooledVolume;

        /// <summary>
        /// Брутен охлаждаем обем [m³]
        /// </summary>
        [ObservableProperty]
        private string? _grossCooledVolume;

        // ========== МЕСЕЧНИ ДНИ ПОЧИВНИ (за всеки месец) ==========
        [ObservableProperty]
        private string? _daysOffJanuary;

        [ObservableProperty]
        private string? _daysOffFebruary;

        [ObservableProperty]
        private string? _daysOffMarch;

        [ObservableProperty]
        private string? _daysOffApril;

        [ObservableProperty]
        private string? _daysOffMay;

        [ObservableProperty]
        private string? _daysOffJune;

        [ObservableProperty]
        private string? _daysOffJuly;

        [ObservableProperty]
        private string? _daysOffAugust;

        [ObservableProperty]
        private string? _daysOffSeptember;

        [ObservableProperty]
        private string? _daysOffOctober;

        [ObservableProperty]
        private string? _daysOffNovember;

        [ObservableProperty]
        private string? _daysOffDecember;

        /// <summary>
        /// Сума на всички дни почивни за годината (int)
        /// </summary>
        public int MonthlyDaysOffSum
        {
            get
            {
                int SumParse(string? s)
                {
                    if (string.IsNullOrWhiteSpace(s)) return 0;
                    if (int.TryParse(s.Trim(), out int v)) return v;
                    return 0;
                }

                // compute sum
                var sum = SumParse(DaysOffJanuary)
                       + SumParse(DaysOffFebruary)
                       + SumParse(DaysOffMarch)
                       + SumParse(DaysOffApril)
                       + SumParse(DaysOffMay)
                       + SumParse(DaysOffJune)
                       + SumParse(DaysOffJuly)
                       + SumParse(DaysOffAugust)
                       + SumParse(DaysOffSeptember)
                       + SumParse(DaysOffOctober)
                       + SumParse(DaysOffNovember)
                       + SumParse(DaysOffDecember);

                // Log for debugging when getter is invoked
                try
                {
                    Debug.WriteLine($"[Debug] MonthlyDaysOffSum getter -> {sum}");
                    try { System.Console.WriteLine($"[Debug] MonthlyDaysOffSum getter -> {sum}"); } catch { }
                }
                catch { }

                return sum;
            }
        }

        // Notify that MonthlyDaysOffSum changed when any month changes
        partial void OnDaysOffJanuaryChanged(string? value)
        {
            ClampAndNotify(ref _daysOffJanuary, value, 31, nameof(DaysOffJanuary));
        }

        partial void OnDaysOffFebruaryChanged(string? value)
        {
            ClampAndNotify(ref _daysOffFebruary, value, 28, nameof(DaysOffFebruary));
        }

        partial void OnDaysOffMarchChanged(string? value)
        {
            ClampAndNotify(ref _daysOffMarch, value, 31, nameof(DaysOffMarch));
        }

        partial void OnDaysOffAprilChanged(string? value)
        {
            ClampAndNotify(ref _daysOffApril, value, 30, nameof(DaysOffApril));
        }

        partial void OnDaysOffMayChanged(string? value)
        {
            ClampAndNotify(ref _daysOffMay, value, 31, nameof(DaysOffMay));
        }

        partial void OnDaysOffJuneChanged(string? value)
        {
            ClampAndNotify(ref _daysOffJune, value, 30, nameof(DaysOffJune));
        }

        partial void OnDaysOffJulyChanged(string? value)
        {
            ClampAndNotify(ref _daysOffJuly, value, 31, nameof(DaysOffJuly));
        }

        partial void OnDaysOffAugustChanged(string? value)
        {
            ClampAndNotify(ref _daysOffAugust, value, 31, nameof(DaysOffAugust));
        }

        partial void OnDaysOffSeptemberChanged(string? value)
        {
            ClampAndNotify(ref _daysOffSeptember, value, 30, nameof(DaysOffSeptember));
        }

        partial void OnDaysOffOctoberChanged(string? value)
        {
            ClampAndNotify(ref _daysOffOctober, value, 31, nameof(DaysOffOctober));
        }

        partial void OnDaysOffNovemberChanged(string? value)
        {
            ClampAndNotify(ref _daysOffNovember, value, 30, nameof(DaysOffNovember));
        }

        partial void OnDaysOffDecemberChanged(string? value)
        {
            ClampAndNotify(ref _daysOffDecember, value, 31, nameof(DaysOffDecember));
        }

        private void ClampAndNotify(ref string? field, string? incoming, int max, string propertyName)
        {
            // Parse incoming, clamp to [0,max], store cleaned string (empty if zero/null)
            if (string.IsNullOrWhiteSpace(incoming))
            {
                field = null;
                OnPropertyChanged(propertyName);
                OnPropertyChanged(nameof(MonthlyDaysOffSum));
                return;
            }

            if (int.TryParse(incoming.Trim(), out int v))
            {
                if (v < 0) v = 0;
                if (v > max) v = max;
                var s = v == 0 ? null : v.ToString();
                if (field != s)
                {
                    field = s;
                    OnPropertyChanged(propertyName);
                    OnPropertyChanged(nameof(MonthlyDaysOffSum));
                }
            }
            else
            {
                // Non-numeric: clear
                field = null;
                OnPropertyChanged(propertyName);
                OnPropertyChanged(nameof(MonthlyDaysOffSum));
            }
        }

        // Notify that HeatingSeasonInfo changed when ClimateZone changes
        partial void OnClimateZoneChanged(int value)
        {
            OnPropertyChanged(nameof(HeatingSeasonInfo));
        }

        // Notify that CoolingSeasonInfo and CoolingDaysCount change when components change
        partial void OnCoolingSeasonStartDayChanged(int? value)
        {
            ClampCoolingDay(ref _coolingSeasonStartDay, _coolingSeasonStartMonth);
            OnPropertyChanged(nameof(CoolingSeasonInfo));
            OnPropertyChanged(nameof(CoolingDaysCount));
        }

        partial void OnCoolingSeasonStartMonthChanged(int? value)
        {
            // clamp start day to valid range for month
            ClampCoolingDay(ref _coolingSeasonStartDay, value);
            OnPropertyChanged(nameof(CoolingSeasonInfo));
            OnPropertyChanged(nameof(CoolingDaysCount));
        }

        partial void OnCoolingSeasonEndDayChanged(int? value)
        {
            ClampCoolingDay(ref _coolingSeasonEndDay, _coolingSeasonEndMonth);
            OnPropertyChanged(nameof(CoolingSeasonInfo));
            OnPropertyChanged(nameof(CoolingDaysCount));
        }

        partial void OnCoolingSeasonEndMonthChanged(int? value)
        {
            ClampCoolingDay(ref _coolingSeasonEndDay, value);
            OnPropertyChanged(nameof(CoolingSeasonInfo));
            OnPropertyChanged(nameof(CoolingDaysCount));
        }

        private void ClampCoolingDay(ref int? dayField, int? month)
        {
            if (!dayField.HasValue || !month.HasValue)
                return;
            int m = month.Value;
            if (m < 1 || m > 12)
            {
                dayField = null;
                return;
            }
            int max = MonthLengths[m - 1];
            if (dayField.Value < 1) dayField = 1;
            if (dayField.Value > max) dayField = max;
        }

        // Синхронизация между BuildingType (старо поле) и BuildingTypeCode (ново поле)
        partial void OnBuildingTypeCodeChanged(BuildingTypeCode? value)
        {
            // Когато се промени кодът, обновяваме и старото текстово поле
            if (value.HasValue)
            {
                var info = BuildingTypeInfo.GetByCode(value.Value);
                if (info != null)
                {
                    _buildingType = info.DisplayName;
                    OnPropertyChanged(nameof(BuildingType));
                }
            }
        }

        /// <summary>
        /// Изчислява и връща display name на типа сграда
        /// </summary>
        public string? BuildingTypeDisplayName
        {
            get
            {
                if (BuildingTypeCode.HasValue)
                {
                    var info = BuildingTypeInfo.GetByCode(BuildingTypeCode.Value);
                    return info?.DisplayName;
                }
                return BuildingType; // fallback към стария текст
            }
        }

        /// <summary>
        /// Опит за миграция от стар BuildingType към BuildingTypeCode
        /// Извиква се при десериализация на стари файлове
        /// </summary>
        public void MigrateBuildingType()
        {
            // Ако вече имаме код, не правим нищо
            if (BuildingTypeCode.HasValue)
                return;

            // Опит за mapване от стария текст
            if (!string.IsNullOrWhiteSpace(BuildingType))
            {
                var mapped = BuildingTypeInfo.TryMapFromString(BuildingType);
                if (mapped.HasValue)
                {
                    BuildingTypeCode = mapped.Value;
                }
            }
        }

        public string HeatingSeasonInfo => ClimateZone switch
        {
            1 => "Начало: 21 октомври; Край: 20 април",
            2 => "Начало: 21 октомври; Край: 25 април",
            3 => "Начало: 23 октомври; Край: 15 април",
            4 => "Начало: 16 октомври; Край: 23 април",
            5 => "Начало: 25 октомври; Край: 19 април",
            6 => "Начало: 24 октомври; Край: 6 април",
            7 => "Начало: 15 октомври; Край: 23 април",
            8 => "Начало: 28 октомври; Край: 6 април",
            9 => "Начало: 28 октомври; Край: 5 април",
            _ => string.Empty
        };

        // month lengths for non-leap year and Bulgarian month names
        private static readonly int[] MonthLengths = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private static readonly string[] MonthNames = { "януари", "февруари", "март", "април", "май", "юни", "юли", "август", "септември", "октомври", "ноември", "декември" };

        /// <summary>
        /// Display string for the cooling season as entered by the user (day + month, no year)
        /// </summary>
        public string CoolingSeasonInfo
        {
            get
            {
                string Format(int d, int m) => $"{d} {MonthNames[m - 1]}";

                if (CoolingSeasonStartDay.HasValue && CoolingSeasonStartMonth.HasValue && CoolingSeasonEndDay.HasValue && CoolingSeasonEndMonth.HasValue)
                {
                    return $"Начало: {Format(CoolingSeasonStartDay.Value, CoolingSeasonStartMonth.Value)}; Край: {Format(CoolingSeasonEndDay.Value, CoolingSeasonEndMonth.Value)}";
                }
                if (CoolingSeasonStartDay.HasValue && CoolingSeasonStartMonth.HasValue)
                    return $"Начало: {Format(CoolingSeasonStartDay.Value, CoolingSeasonStartMonth.Value)}";
                if (CoolingSeasonEndDay.HasValue && CoolingSeasonEndMonth.HasValue)
                    return $"Край: {Format(CoolingSeasonEndDay.Value, CoolingSeasonEndMonth.Value)}";
                return "-";
            }
        }

        /// <summary>
        /// Total number of days in the cooling season (assumes non-leap year). Returns 0 if start or end not set.
        /// Inclusive of both start and end days.
        /// </summary>
        public int CoolingDaysCount
        {
            get
            {
                if (!CoolingSeasonStartDay.HasValue || !CoolingSeasonStartMonth.HasValue || !CoolingSeasonEndDay.HasValue || !CoolingSeasonEndMonth.HasValue)
                    return 0;

                int start = DayOfYear(CoolingSeasonStartMonth.Value, CoolingSeasonStartDay.Value);
                int end = DayOfYear(CoolingSeasonEndMonth.Value, CoolingSeasonEndDay.Value);
                if (start <= end)
                    return end - start + 1;
                // wrap around year
                return (365 - start + 1) + end;
            }
        }

        private static int DayOfYear(int month, int day)
        {
            int sum = 0;
            for (int i = 0; i < month - 1; i++) sum += MonthLengths[i];
            return sum + day;
        }
    }
}
