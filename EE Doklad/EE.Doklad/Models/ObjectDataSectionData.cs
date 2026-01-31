using CommunityToolkit.Mvvm.ComponentModel;
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
    }
}
