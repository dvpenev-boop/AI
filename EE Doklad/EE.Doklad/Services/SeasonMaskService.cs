using System;

namespace EE.Doklad.Services
{
    /// <summary>
    /// Параметри на един сезон за SeasonMaskService.
    /// </summary>
    public sealed class SeasonParams
    {
        /// <summary>Начален месец (1–12)</summary>
        public int StartMonth { get; set; }
        /// <summary>Начален ден</summary>
        public int StartDay   { get; set; } = 1;
        /// <summary>Краен месец (1–12)</summary>
        public int EndMonth   { get; set; }
        /// <summary>Краен ден (включително)</summary>
        public int EndDay     { get; set; }
        /// <summary>Часове активност на ден (средно)</summary>
        public double HoursPerDay { get; set; } = 24.0;
        /// <summary>Референтна година за изчисленията</summary>
        public int YearRef        { get; set; } = 2024;
        public bool IncludeStartDay { get; set; } = true;
        public bool IncludeEndDay { get; set; } = true;
    }

    /// <summary>
    /// Резултат от SeasonMaskService: разпределение на часовете по месеци.
    /// Индекс 0 = Януари ... 11 = Декември.
    /// </summary>
    public sealed class SeasonMaskResult
    {
        /// <summary>Часове [h] в месеца активни за дадения сезон (12 елемента).</summary>
        public double[] Hours { get; } = new double[12];
        /// <summary>Дни в месеца активни за сезона (12 елемента).</summary>
        public double[] Days  { get; } = new double[12];
        /// <summary>Общо часове за целия сезон.</summary>
        public double TotalHours => SumArray(Hours);
        /// <summary>Общо дни за целия сезон.</summary>
        public double TotalDays  => SumArray(Days);

        private static double SumArray(double[] arr)
        {
            double s = 0;
            foreach (double v in arr) s += v;
            return s;
        }
    }

    /// <summary>
    /// SeasonMaskService – изчислява за всеки месец колко часове/дни
    /// попадат в даден сезон.
    ///
    /// Поддържа:
    ///   • сезони в рамките на едната календарна година  (напр. 1.06 – 30.09)
    ///   • сезони, пресичащи Нова Година               (напр. 21.10 – 20.04)
    ///   • частични начален и краен месец
    ///
    /// Алгоритъм:
    ///   За всеки месец m (1..12) се определя пресечението на [1-ви ден, последен ден]
    ///   с интервала [SeasonStart, SeasonEnd]. Часовете = дни × HoursPerDay.
    /// </summary>
    public static class SeasonMaskService
    {
        /// <summary>
        /// Пресмята масивите Hours[12] и Days[12] за дадения сезон.
        /// </summary>
        public static SeasonMaskResult Compute(SeasonParams p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));

            var result = new SeasonMaskResult();

            // Нормализираме началото и края на сезона като DateTime
            // За сезон, пресичащ НГ (EndMonth < StartMonth):
            //   seasonStart = YearRef-1 / StartMonth / StartDay
            //   seasonEnd   = YearRef   / EndMonth   / EndDay
            // Итерираме месеци на YearRef (Януари..Декември YearRef).
            // Но за пресичащия сезон месеците от StartMonth..12 на YearRef-1
            // трябва да бъдат в YearRef (просто ги отчитаме за YearRef чрез wrap).
            // По-прост подход: при wrap итерираме YearRef и YearRef+1,
            // след това сгъваме месеците обратно към 1..12.

            bool wraps = p.EndMonth < p.StartMonth
                      || (p.EndMonth == p.StartMonth && p.EndDay < p.StartDay);

            if (!wraps)
            {
                // Сезонът е изцяло в YearRef
                var seasonStart = new DateTime(p.YearRef, p.StartMonth, p.StartDay);
                var seasonEnd   = new DateTime(p.YearRef, p.EndMonth,
                    Math.Min(p.EndDay, DateTime.DaysInMonth(p.YearRef, p.EndMonth)));

                for (int m = 1; m <= 12; m++)
                {
                    double days = IntersectDays(m, p.YearRef, seasonStart, seasonEnd);
                    if (!p.IncludeStartDay && m == p.StartMonth && days > 0)
                        days = Math.Max(0.0, days - 1.0);
                    if (!p.IncludeEndDay && m == p.EndMonth && days > 0)
                        days = Math.Max(0.0, days - 1.0);
                    result.Days[m - 1]  = days;
                    result.Hours[m - 1] = days * p.HoursPerDay;
                }
            }
            else
            {
                // Сезонът пресича НГ.
                // Разбиваме на две половини:
                //   Part A: StartMonth/StartDay .. 31.12 на YearRef-1
                //   Part B: 01.01 .. EndMonth/EndDay на YearRef
                // Итерираме месеци 1..12 на YearRef:
                //   • Месеците от StartMonth..12 = Part A (отнасят се за предходната година)
                //   • Месеците от 1..EndMonth    = Part B
                // Ключова забележка: зимен сезон 21.10 – 20.04 → в доклада касае
                //   месеци Окт, Ноем, Дек (начало) + Яну, Фев, Мар, Апр (край).
                // Трактуваме ги всички в YearRef (стандарт за EPB).

                var seasonStartA = new DateTime(p.YearRef, p.StartMonth, p.StartDay);
                var seasonEndA   = new DateTime(p.YearRef, 12, 31);

                int nextYear = p.YearRef + 1;
                var seasonStartB = new DateTime(nextYear, 1, 1);
                var seasonEndB   = new DateTime(nextYear, p.EndMonth,
                    Math.Min(p.EndDay, DateTime.DaysInMonth(nextYear, p.EndMonth)));

                for (int m = 1; m <= 12; m++)
                {
                    double daysA = m >= p.StartMonth
                        ? IntersectDays(m, p.YearRef, seasonStartA, seasonEndA)
                        : 0.0;
                    double daysB = m <= p.EndMonth
                        ? IntersectDays(m, nextYear, seasonStartB, seasonEndB)
                        : 0.0;
                    double days  = daysA + daysB;
                    if (!p.IncludeStartDay && m == p.StartMonth && days > 0)
                        days = Math.Max(0.0, days - 1.0);
                    if (!p.IncludeEndDay && m == p.EndMonth && days > 0)
                        days = Math.Max(0.0, days - 1.0);
                    result.Days[m - 1]  = days;
                    result.Hours[m - 1] = days * p.HoursPerDay;
                }
            }

            return result;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Изчислява колко дни от месец m на годината year попадат в
        // интервала [seasonStart, seasonEnd] (включително краищата).
        // ─────────────────────────────────────────────────────────────────────
        private static double IntersectDays(int month, int year, DateTime seasonStart, DateTime seasonEnd)
        {
            var monthStart = new DateTime(year, month, 1);
            var monthEnd   = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            // Пресечение
            DateTime intStart = seasonStart > monthStart ? seasonStart : monthStart;
            DateTime intEnd   = seasonEnd   < monthEnd   ? seasonEnd   : monthEnd;

            if (intEnd < intStart) return 0.0;

            // +1 защото и двата края са включени
            return (intEnd - intStart).TotalDays + 1.0;
        }
    }
}
