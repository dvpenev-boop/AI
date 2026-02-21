using System;
using EE.Doklad.Models;

namespace EE.Doklad.Services.Schedule
{
    // ─────────────────────────────────────────────────────────────────────────────
    // OverlapCalculator
    //
    // Изчислява продължителност и застъпване между два дневни графика, изразени
    // като WeeklyTimeRange (TimeSpan Start / End).
    //
    // Конвенции:
    //   • Start == End  → 0 часа (графикът е изключен). [консистентно с WeeklyTimeRange.GetHours()]
    //   • End > Start   → нормален диапазон.
    //   • End < Start   → overnight wrap-around, напр. 22:00–06:00.
    //   • Вътрешно работим с [0, 24) интервали в минути, за да поддържаме wrap-around
    //     чрез разбиване на сегменти.
    // ─────────────────────────────────────────────────────────────────────────────

    public static class OverlapCalculator
    {
        // ── Public API ────────────────────────────────────────────────────────────

        /// <summary>
        /// Продължителност на <paramref name="range"/> в часове.
        /// Делегира към <see cref="WeeklyTimeRange.GetHours()"/>, за да се запази консистентност.
        /// </summary>
        public static double GetDurationHours(WeeklyTimeRange range)
        {
            if (range is null) return 0.0;
            return range.GetHours();
        }

        /// <summary>
        /// Застъпване в часове между два дневни диапазона.
        /// Поддържа wrap-around (overnight) графици.
        /// Start == End → 0 часа (изключено).
        /// </summary>
        public static double GetOverlapHours(WeeklyTimeRange a, WeeklyTimeRange b)
        {
            if (a is null || b is null) return 0.0;

            // Получаваме сегменти за всеки диапазон
            var segA = ToMinuteSegments(a);
            var segB = ToMinuteSegments(b);

            double totalMinutes = 0.0;
            foreach (var sa in segA)
                foreach (var sb in segB)
                    totalMinutes += IntervalOverlapMinutes(sa.start, sa.end, sb.start, sb.end);

            return totalMinutes / 60.0;
        }

        /// <summary>
        /// Седмично застъпване [ч] = WdOverlap*5 + SatOverlap + SunOverlap.
        /// </summary>
        public static double GetWeeklyOverlapHours(WeeklySchedule vent, WeeklySchedule cool)
        {
            if (vent is null || cool is null) return 0.0;
            return GetOverlapHours(vent.Workdays, cool.Workdays) * 5.0
                 + GetOverlapHours(vent.Saturday, cool.Saturday)
                 + GetOverlapHours(vent.Sunday,   cool.Sunday);
        }

        /// <summary>
        /// Седмична продължителност на вентилационния график [ч] = WdH*5 + SatH + SunH.
        /// </summary>
        public static double GetWeeklyVentHours(WeeklySchedule vent)
        {
            if (vent is null) return 0.0;
            return GetDurationHours(vent.Workdays) * 5.0
                 + GetDurationHours(vent.Saturday)
                 + GetDurationHours(vent.Sunday);
        }

        /// <summary>
        /// Коефициент на застъпване f_on = WeeklyOverlap / WeeklyVent (clamp 0..1).
        /// При нулев WeeklyVent → f_on = 0 (с предупреждение в <paramref name="warning"/>).
        /// </summary>
        public static double ComputeFon(WeeklySchedule vent, WeeklySchedule cool, out string? warning)
        {
            warning = null;
            double ventHours = GetWeeklyVentHours(vent);
            if (ventHours <= 0.0)
            {
                warning = "VentHoursWeek == 0 → f_on = 0 (вентилационният график е изключен).";
                return 0.0;
            }
            double overlapHours = GetWeeklyOverlapHours(vent, cool);
            return Math.Clamp(overlapHours / ventHours, 0.0, 1.0);
        }

        // ── Internal helpers ──────────────────────────────────────────────────────

        /// <summary>
        /// Разбива WeeklyTimeRange на списък от сегменти [startMin, endMin) в [0, 1440).
        /// Start == End → празен списък (нула часа).
        /// Overnight (End &lt; Start) → два сегмента: [Start,1440) и [0,End).
        /// </summary>
        private static (int start, int end)[] ToMinuteSegments(WeeklyTimeRange r)
        {
            int startMin = (int)Math.Round(r.StartTime.TotalMinutes);
            int endMin   = (int)Math.Round(r.EndTime.TotalMinutes);

            // Нормализиране в [0,1440)
            startMin = ((startMin % 1440) + 1440) % 1440;
            endMin   = ((endMin   % 1440) + 1440) % 1440;

            if (startMin == endMin)
                return Array.Empty<(int, int)>();   // изключено

            if (endMin > startMin)
                return new[] { (startMin, endMin) };

            // Overnight wrap-around: [startMin, 1440) + [0, endMin)
            return new[] { (startMin, 1440), (0, endMin) };
        }

        /// <summary>
        /// Припокриване в минути между два полуотворени интервала [s1,e1) и [s2,e2).
        /// </summary>
        private static int IntervalOverlapMinutes(int s1, int e1, int s2, int e2)
        {
            int os = Math.Max(s1, s2);
            int oe = Math.Min(e1, e2);
            return oe > os ? oe - os : 0;
        }
    }
}
