using System;
using System.Collections.Generic;
using System.Linq;

namespace EE.Doklad.Services.Schedule
{
    /// <summary>
    /// Описва дневен график: начален и краен час (включително, 0..23).
    /// EndHour >= StartHour (overnight графици не се поддържат по нормативно изискване на 7257_1).
    /// RunHoursPerDay = EndHour - StartHour + 1
    /// </summary>
    public sealed class DailyTimeRange
    {
        /// <summary>Начален час (включително), 0..23</summary>
        public int StartHour { get; init; }

        /// <summary>Краен час (включително), 0..23. Трябва StartHour &lt;= EndHour.</summary>
        public int EndHour { get; init; }

        public bool IsValid => StartHour >= 0 && EndHour <= 23 && StartHour <= EndHour;

        /// <summary>Брой работни часа на ден (включително и двата края).</summary>
        public int RunHoursPerDay => IsValid ? EndHour - StartHour + 1 : 0;

        /// <summary>Стандартен начин за представяне "10..19 (10 h)".</summary>
        public override string ToString() => IsValid ? $"{StartHour}..{EndHour} ({RunHoursPerDay} h)" : "(невалиден)";
    }

    /// <summary>
    /// Дефиниция на седмичен график: активни типове дни + дневен диапазон от часове.
    /// </summary>
    public sealed class WeeklyScheduleConfig
    {
        public DailyTimeRange TimeRange { get; init; } = new DailyTimeRange();

        /// <summary>Работни дни (Пн..Пт) са активни?</summary>
        public bool WorkdaysActive { get; init; } = true;

        /// <summary>Събота е активна?</summary>
        public bool SaturdayActive { get; init; } = false;

        /// <summary>Неделя е активна?</summary>
        public bool SundayActive { get; init; } = false;

        /// <summary>Дали даден ден от седмицата е активен.</summary>
        public bool IsActiveDayOfWeek(DayOfWeek dow) => dow switch
        {
            DayOfWeek.Saturday => SaturdayActive,
            DayOfWeek.Sunday   => SundayActive,
            _                  => WorkdaysActive   // Mon..Fri
        };

        public bool IsValid => TimeRange.IsValid;
    }

    /// <summary>
    /// Резултат за един месец: работни дни / часове (след приспадане на почивки).
    /// </summary>
    public sealed class MonthlyScheduleResult
    {
        public int MonthNumber { get; init; }

        /// <summary>Дни в сезона (преди приспадане на почивни).</summary>
        public int DaysInSeason { get; init; }

        /// <summary>Дни извън почивни дни (приспаднати от DaysInSeason пропорционално по тип).</summary>
        public double WorkingDays { get; init; }

        /// <summary>Работни дни Пн-Пт (след приспадане).</summary>
        public double WorkingDaysWeekday { get; init; }

        /// <summary>Работни дни Събота (след приспадане).</summary>
        public double WorkingDaysSaturday { get; init; }

        /// <summary>Работни дни Неделя (след приспадане).</summary>
        public double WorkingDaysSunday { get; init; }

        /// <summary>Общо работни часове в месеца (= WorkingDays * RunHoursPerDay).</summary>
        public double WorkingHours { get; init; }

        public int HolidaysSubtracted { get; init; }
    }

    /// <summary>
    /// Изчислява работни дни, часове и ваканции по охладителния сезон.
    ///
    /// Правила:
    /// 1. Работни дни за месец m = (дни в сезона ∩ активен тип ден) – почивни дни за месеца.
    /// 2. Почивните дни (DaysOff[m]) се приспадат пропорционално спрямо делника на дадения тип ден
    ///    (т.е. ако уикендът не е активен, почивните дни се приспадат само от работните).
    /// 3. Официални празници: ако попадат на неактивен ден → нямат ефект.
    ///    Имплементирано чрез DaysOff масива (Секция 5 дни почивни).
    /// 4. Overlap (застъпване) между два графика: [s1..e1] ∩ [s2..e2] при включителни часове.
    /// </summary>
    public static class WorkdayScheduleCalculator
    {
        /// <summary>
        /// Изчислява месечните работни дни/часове за целия сезон.
        /// </summary>
        /// <param name="schedule">Седмичен график с диапазон часове и активни дни.</param>
        /// <param name="seasonStart">Начало на охладителния сезон (включително).</param>
        /// <param name="seasonEnd">Край на охладителния сезон (включително).</param>
        /// <param name="daysOff">Масив 12 елемента – брой почивни дни по месеци (Секция 5).</param>
        /// <param name="officialHolidays">Допълнителен списък с дати на официални празници (optional).
        ///   Само тези, попадащи на активен ден от графика, се приспадат.</param>
        /// <param name="yearRef">Референтна година за изграждане на Calendar.</param>
        public static List<MonthlyScheduleResult> ComputeMonthly(
            WeeklyScheduleConfig schedule,
            DateTime seasonStart,
            DateTime seasonEnd,
            int[] daysOff,                          // length 12
            IReadOnlyList<DateTime>? officialHolidays,
            int yearRef = 2024)
        {
            if (schedule == null) throw new ArgumentNullException(nameof(schedule));
            if (daysOff == null || daysOff.Length != 12) throw new ArgumentException("daysOff трябва да е с дължина 12.", nameof(daysOff));

            var results = new List<MonthlyScheduleResult>(12);

            for (int m = 1; m <= 12; m++)
            {
                int daysInMonth = DateTime.DaysInMonth(yearRef, m);
                int weekday = 0, saturday = 0, sunday = 0;

                // Count season-days by day type (active days only)
                for (int d = 1; d <= daysInMonth; d++)
                {
                    var dt = new DateTime(yearRef, m, d);
                    if (!IsInSeasonRange(dt, seasonStart, seasonEnd, yearRef))
                        continue;

                    if (!schedule.IsActiveDayOfWeek(dt.DayOfWeek))
                        continue;

                    switch (dt.DayOfWeek)
                    {
                        case DayOfWeek.Saturday: saturday++; break;
                        case DayOfWeek.Sunday:   sunday++;   break;
                        default:                 weekday++;  break;
                    }
                }

                int candidateActiveDays = weekday + saturday + sunday;

                // Official holidays that fall on an active day of this month & season
                int officialHolidaysOnActiveDays = 0;
                if (officialHolidays != null)
                {
                    foreach (var h in officialHolidays)
                    {
                        if (h.Month != m) continue;
                        var dt = new DateTime(yearRef, h.Month, h.Day);
                        if (!IsInSeasonRange(dt, seasonStart, seasonEnd, yearRef)) continue;
                        if (schedule.IsActiveDayOfWeek(dt.DayOfWeek))
                            officialHolidaysOnActiveDays++;
                    }
                }

                // Additional days-off (Секция 5) – applied proportionally across active day types
                int monthDaysOff = Math.Max(0, daysOff[m - 1]);

                // Total days to subtract (capped to candidate active days)
                int totalSubtract = Math.Min(candidateActiveDays, officialHolidaysOnActiveDays + monthDaysOff);

                double workingDays;
                double workDaysWeekday, workDaysSat, workDaysSun;

                if (candidateActiveDays <= 0)
                {
                    workingDays = workDaysWeekday = workDaysSat = workDaysSun = 0.0;
                }
                else
                {
                    workingDays = Math.Max(0.0, candidateActiveDays - totalSubtract);

                    // Distribute the remaining work-days proportionally across types
                    double ratio = workingDays / candidateActiveDays;
                    workDaysWeekday = weekday * ratio;
                    workDaysSat     = saturday * ratio;
                    workDaysSun     = sunday * ratio;
                }

                double workingHours = workingDays * schedule.TimeRange.RunHoursPerDay;

                results.Add(new MonthlyScheduleResult
                {
                    MonthNumber          = m,
                    DaysInSeason         = candidateActiveDays,
                    WorkingDays          = workingDays,
                    WorkingDaysWeekday   = workDaysWeekday,
                    WorkingDaysSaturday  = workDaysSat,
                    WorkingDaysSunday    = workDaysSun,
                    WorkingHours         = workingHours,
                    HolidaysSubtracted   = totalSubtract,
                });
            }

            return results;
        }

        /// <summary>
        /// Изчислява дните в сезона за дадения месец (само от активните дни по графика).
        /// </summary>
        public static int CountActiveDaysInMonth(
            WeeklyScheduleConfig schedule,
            DateTime seasonStart,
            DateTime seasonEnd,
            int month,
            int yearRef = 2024)
        {
            int daysInMonth = DateTime.DaysInMonth(yearRef, month);
            int count = 0;
            for (int d = 1; d <= daysInMonth; d++)
            {
                var dt = new DateTime(yearRef, month, d);
                if (IsInSeasonRange(dt, seasonStart, seasonEnd, yearRef) && schedule.IsActiveDayOfWeek(dt.DayOfWeek))
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Изчислява броя на застъпващите се часове между два дневни графика (включителни краища).
        /// Двата диапазона са [start1..end1] и [start2..end2] (часове 0..23).
        /// Резултатът е броят часове, включени и в двата диапазона.
        /// Пример: Overlap(10,19, 8,17) = 8  (часове 10..17)
        /// </summary>
        public static int OverlapHours(int start1, int end1, int start2, int end2)
        {
            if (start1 > end1 || start2 > end2) return 0;
            int overlapStart = Math.Max(start1, start2);
            int overlapEnd   = Math.Min(end1,   end2);
            return overlapEnd >= overlapStart ? overlapEnd - overlapStart + 1 : 0;
        }

        /// <summary>
        /// Коефициент на застъпване f_on = OverlapHours / RunHoursPerDay (vent schedule).
        /// Ако RunHoursPerDay == 0, резултатът е 0.
        /// </summary>
        public static double OverlapFraction(DailyTimeRange ventSchedule, DailyTimeRange coolSchedule)
        {
            if (ventSchedule == null || coolSchedule == null) return 0.0;
            int vent = ventSchedule.RunHoursPerDay;
            if (vent == 0) return 0.0;
            int overlap = OverlapHours(ventSchedule.StartHour, ventSchedule.EndHour,
                                       coolSchedule.StartHour, coolSchedule.EndHour);
            return (double)overlap / vent;
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Проверява дали дата е в сезонния диапазон (включително краищата).
        /// Поддържа "wrap-around" сезони (напр. ноември → април).
        /// </summary>
        private static bool IsInSeasonRange(DateTime dt, DateTime seasonStart, DateTime seasonEnd, int yearRef)
        {
            // Primary check with yearRef dates
            if (dt >= seasonStart && dt <= seasonEnd) return true;
            // If season wraps to next year, check shifted by +1 year
            if (seasonEnd > seasonStart) return false; // non-wrapping, already checked
            // Wrap: seasonEnd.Year may be yearRef+1
            if (dt.AddYears(1) >= seasonStart && dt.AddYears(1) <= seasonEnd) return true;
            return false;
        }
    }
}
