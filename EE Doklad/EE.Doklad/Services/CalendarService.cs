using System;

namespace EE.Doklad.Services
{
    /// <summary>
    /// Услуга за календарни изчисления: работни дни, съботи, недели в даден месец.
    /// </summary>
    public static class CalendarService
    {
        /// <summary>
        /// Връща броя работни дни (понеделник-петък), съботи и недели в даден месец.
        /// </summary>
        /// <param name="year">Годината (напр. 2026)</param>
        /// <param name="month">Месецът (1 = януари, ..., 12 = декември)</param>
        /// <returns>Tuple: (WorkDays, SaturdayCount, SundayCount)</returns>
        public static (int WorkDays, int SaturdayCount, int SundayCount) GetCalendarCounts(int year, int month)
        {
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(nameof(month), "Месецът трябва да е между 1 и 12.");

            int daysInMonth = DateTime.DaysInMonth(year, month);
            int workDays = 0;
            int saturdayCount = 0;
            int sundayCount = 0;

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day);
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                    case DayOfWeek.Tuesday:
                    case DayOfWeek.Wednesday:
                    case DayOfWeek.Thursday:
                    case DayOfWeek.Friday:
                        workDays++;
                        break;
                    case DayOfWeek.Saturday:
                        saturdayCount++;
                        break;
                    case DayOfWeek.Sunday:
                        sundayCount++;
                        break;
                }
            }

            return (workDays, saturdayCount, sundayCount);
        }
    }
}
