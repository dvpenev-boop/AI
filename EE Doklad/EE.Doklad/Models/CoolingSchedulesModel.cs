using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Представлява диапазон от време (начало-край) за един тип ден
    /// </summary>
    public partial class WeeklyTimeRange : ObservableObject
    {
        [ObservableProperty]
        private TimeSpan _startTime = TimeSpan.Zero;

        [ObservableProperty]
        private TimeSpan _endTime = TimeSpan.Zero;

        /// <summary>
        /// Изчислява часовете обитаване за този тип ден.
        /// Ако Start==End => 0 часа (няма обитаване).
        /// Ако End > Start => End - Start.
        /// Ако End &lt; Start => overnight => (24 - Start) + End.
        /// </summary>
        public double GetHours()
        {
            if (StartTime == EndTime)
                return 0.0;

            if (EndTime > StartTime)
                return (EndTime - StartTime).TotalHours;

            // overnight: (24 - start) + end
            return (TimeSpan.FromHours(24) - StartTime + EndTime).TotalHours;
        }
    }

    /// <summary>
    /// График за един вид обитаване/охлаждане/вентилация (работни дни, събота, неделя)
    /// </summary>
    public partial class WeeklySchedule : ObservableObject
    {
        [ObservableProperty]
        private WeeklyTimeRange _workdays = new();

        [ObservableProperty]
        private WeeklyTimeRange _saturday = new();

        [ObservableProperty]
        private WeeklyTimeRange _sunday = new();

        /// <summary>
        /// Изчислява часовете за дадения тип ден
        /// </summary>
        /// <param name="dayType">"workdays", "saturday", "sunday"</param>
        public double GetHours(string dayType)
        {
            return dayType.ToLowerInvariant() switch
            {
                "workdays" => Workdays.GetHours(),
                "saturday" => Saturday.GetHours(),
                "sunday" => Sunday.GetHours(),
                _ => 0.0
            };
        }

        /// <summary>
        /// Изчислява часове/седмица: 5*Workdays + Saturday + Sunday
        /// </summary>
        public double GetHoursPerWeek()
        {
            return Workdays.GetHours() * 5 + Saturday.GetHours() + Sunday.GetHours();
        }
    }

    /// <summary>
    /// Съдържа всички 4 графика за охладителния период:
    /// - График за обитаване (охладителен период)
    /// - График за охлаждане
    /// - График за вентилация охлаждане
    /// - График за вентилиране с външен въздух
    /// </summary>
    public partial class CoolingSchedulesModel : ObservableObject
    {
        /// <summary>
        /// A) График за обитаване – охладителен период
        /// </summary>
        [ObservableProperty]
        private WeeklySchedule _occupancyCoolingSchedule = new();

        /// <summary>
        /// B) График за охлаждане
        /// </summary>
        [ObservableProperty]
        private WeeklySchedule _coolingSchedule = new();

        /// <summary>
        /// C) График за вентилация охлаждане
        /// </summary>
        [ObservableProperty]
        private WeeklySchedule _ventilationCoolingSchedule = new();

        /// <summary>
        /// D) График за вентилиране с външен въздух
        /// </summary>
        [ObservableProperty]
        private WeeklySchedule _outdoorAirVentSchedule = new();
    }

    /// <summary>
    /// График за отопление (секция 5).
    /// Start==End==Zero = не е зададено (0 h).
    /// Start=00:00, End=24:00 = 24 h/ден (TimeSpan(24,0,0).TotalHours == 24.0).
    /// Същата логика като CoolingSchedulesModel.
    /// </summary>
    public partial class HeatingSchedulesModel : ObservableObject
    {
        /// <summary>А) График за обитаване – отоплителен период</summary>
        [ObservableProperty]
        private WeeklySchedule _occupancyHeatingSchedule = new();

        /// <summary>Б) График за отопление</summary>
        [ObservableProperty]
        private WeeklySchedule _heatingSchedule = new();

        /// <summary>Р’) Р“СЂР°С„РёРє Р·Р° РІРµРЅС‚РёР»Р°С†РёСЏ РѕС‚РѕРїР»РµРЅРёРµ</summary>
        [ObservableProperty]
        private WeeklySchedule _ventilationHeatingSchedule = new();
    }
}
