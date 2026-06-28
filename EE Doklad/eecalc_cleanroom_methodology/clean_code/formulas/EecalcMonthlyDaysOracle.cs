using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcMonthlyDaysOracle
    {
        private const int ReferenceYear = 2006;

        public IReadOnlyList<EecalcMonthlyDaysOracleRow> Calculate(EecalcValidationFixture fixture)
        {
            ArgumentNullException.ThrowIfNull(fixture);

            var period = CalculatePeriod(fixture.FirstMonth, fixture.LastMonth);
            return CalculateMonthlyDays(period, fixture.FirstDay, fixture.LastDay, fixture.HolidaysByMonth);
        }

        private static IReadOnlyList<int> CalculatePeriod(int firstMonth, int lastMonth)
        {
            if (firstMonth == lastMonth)
            {
                return new[] { firstMonth };
            }

            var period = new List<int>();
            if (firstMonth < lastMonth && lastMonth <= 12)
            {
                for (var month = firstMonth; month <= lastMonth; month++)
                {
                    period.Add(month);
                }

                return period;
            }

            for (var month = firstMonth; month <= 12; month++)
            {
                period.Add(month);
            }

            for (var month = 1; month <= lastMonth; month++)
            {
                period.Add(month);
            }

            return period;
        }

        private static IReadOnlyList<EecalcMonthlyDaysOracleRow> CalculateMonthlyDays(
            IReadOnlyList<int> period,
            int firstDay,
            int lastDay,
            IReadOnlyDictionary<int, int> holidaysByMonth)
        {
            var rows = new List<EecalcMonthlyDaysOracleRow>();

            foreach (var month in period)
            {
                var daysInMonth = DateTime.DaysInMonth(ReferenceYear, month);
                var holidays = holidaysByMonth.TryGetValue(month, out var value) ? value : 0;
                var isFirst = month == period.First();
                var isLast = month == period.Last();

                if (isFirst)
                {
                    if (period.Count == 1)
                    {
                        var singleMonthRow = CreateEmptyRow(month, daysInMonth, holidays, (daysInMonth - holidays) / 7);
                        CountDays(singleMonthRow, month, firstDay, lastDay);
                        singleMonthRow.WorkDays = Math.Max(singleMonthRow.WorkDays - holidays, 0);
                        rows.Add(singleMonthRow);
                        break;
                    }

                    var weeks = GetWeeksInMonth(firstDay, lastDay, daysInMonth, holidays, isFirstMonth: true);
                    if (firstDay == daysInMonth)
                    {
                        weeks = GetWeeksInMonth(firstDay, lastDay, daysInMonth + 1, holidays, isFirstMonth: true);
                    }

                    if (firstDay > 21)
                    {
                        var remainingDays = daysInMonth - firstDay + 1;
                        rows.Add(CreateRow(month, daysInMonth, holidays, weeks, Math.Max(remainingDays - holidays, 0), 0, 0));
                        continue;
                    }

                    if (firstDay > 14)
                    {
                        var remainingDays = daysInMonth - firstDay + 1 - 2;
                        rows.Add(CreateRow(month, daysInMonth, holidays, weeks, Math.Max(remainingDays - holidays, 0), 1, 1));
                        continue;
                    }

                    if (firstDay > 7)
                    {
                        var remainingDays = daysInMonth - firstDay + 1 - 4;
                        rows.Add(CreateRow(month, daysInMonth, holidays, weeks, Math.Max(remainingDays - holidays, 0), 2, 2));
                        continue;
                    }

                    var row = CreateEmptyRow(month, daysInMonth, holidays, weeks);
                    CountDays(row, month, firstDay, daysInMonth);
                    row.WorkDays = Math.Max(row.WorkDays - holidays, 0);
                    rows.Add(row);
                    continue;
                }

                if (isLast)
                {
                    var correctedLastDay = Math.Min(lastDay, daysInMonth);
                    var weeks = GetWeeksInMonth(firstDay, correctedLastDay, daysInMonth, holidays, isFirstMonth: false);

                    if (correctedLastDay < 7)
                    {
                        rows.Add(CreateRow(month, daysInMonth, holidays, weeks, Math.Max(correctedLastDay - holidays, 0), 0, 0));
                        continue;
                    }

                    if (correctedLastDay < 14)
                    {
                        var workDays = correctedLastDay - 2;
                        rows.Add(CreateRow(month, daysInMonth, holidays, weeks, Math.Max(workDays - holidays, 0), 1, 1));
                        continue;
                    }

                    if (correctedLastDay < 21)
                    {
                        var workDays = correctedLastDay - 4;
                        rows.Add(CreateRow(month, daysInMonth, holidays, weeks, Math.Max(workDays - holidays, 0), 2, 2));
                        continue;
                    }

                    var row = CreateEmptyRow(month, daysInMonth, holidays, weeks);
                    CountDays(row, month, 1, correctedLastDay);
                    row.WorkDays = Math.Max(row.WorkDays - holidays, 0);
                    rows.Add(row);
                    continue;
                }

                var fullMonthWeeks = (daysInMonth - holidays) / 7.0;
                var fullMonth = CreateEmptyRow(month, daysInMonth, holidays, fullMonthWeeks);
                CountDays(fullMonth, month, 1, daysInMonth);
                fullMonth.WorkDays = Math.Max(fullMonth.WorkDays - holidays, 0);
                rows.Add(fullMonth);
            }

            return rows;
        }

        private static EecalcMonthlyDaysOracleRow CreateEmptyRow(int month, int totalDays, int holidays, double weeks)
        {
            return CreateRow(month, totalDays, holidays, weeks, 0, 0, 0);
        }

        private static EecalcMonthlyDaysOracleRow CreateRow(
            int month,
            int totalDays,
            int holidays,
            double weeks,
            int workDays,
            int saturdays,
            int sundays)
        {
            return new EecalcMonthlyDaysOracleRow
            {
                Month = month,
                MonthName = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month),
                TotalDays = totalDays,
                WorkDays = workDays,
                Saturdays = saturdays,
                Sundays = sundays,
                Holidays = holidays,
                Weeks = weeks
            };
        }

        private static void CountDays(EecalcMonthlyDaysOracleRow row, int month, int firstDay, int lastDay)
        {
            for (var day = firstDay; day <= lastDay; day++)
            {
                var date = new DateTime(ReferenceYear, month, day);
                if (date.DayOfWeek == DayOfWeek.Saturday)
                {
                    row.Saturdays++;
                }
                else if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    row.Sundays++;
                }
                else
                {
                    row.WorkDays++;
                }
            }
        }

        private static double GetWeeksInMonth(double startingDay, double endDay, double daysInMonth, double holidays, bool isFirstMonth)
        {
            if (isFirstMonth)
            {
                return daysInMonth - startingDay + 1.0 > holidays
                    ? (daysInMonth - startingDay + 1.0 - holidays) / 7.0
                    : 0.0;
            }

            return endDay > holidays ? (endDay - holidays) / 7.0 : 0.0;
        }
    }

    public sealed class EecalcMonthlyDaysOracleRow
    {
        public int Month { get; init; }

        public string MonthName { get; init; } = string.Empty;

        public int TotalDays { get; init; }

        public int WorkDays { get; set; }

        public int Saturdays { get; set; }

        public int Sundays { get; set; }

        public int Holidays { get; init; }

        public double Weeks { get; init; }

        public int ActiveDays => WorkDays + Saturdays + Sundays + Holidays;
    }
}
