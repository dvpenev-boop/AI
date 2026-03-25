using System;
using EE.Doklad.Models;

namespace EE.Doklad.Services
{
    /// <summary>
    /// Service that computes heating hours per month based on ObjectData schedules, holidays and climate heating season.
    /// </summary>
    public static class HeatingScheduleService
    {
        public readonly record struct HeatingHoursBreakdown(double FullHours, double SetbackHours);

        /// <summary>
        /// Compute heating hours per month [h] using ObjectData heating schedule and climate heating-season.
        /// If schedule fields are missing/empty -> returns zeros (caller may fallback).
        /// Holidays (monthly days off) reduce heating-season days using the same rule as other modules.
        /// </summary>
        public static double[] ComputeHeatingHoursPerMonth(ObjectDataSectionData? objectData, ClimateZoneData? climateData, int yearRef = global::EE.Doklad.CalendarDefaults.ReferenceYear)
        {
            return ComputeBreakdown(
                    objectData?.CalculationMethod ?? HeatingCalculationMethod.Rd0220_3,
                    objectData,
                    climateData)
                .Select(x => x.FullHours)
                .ToArray();
        }

        public static double[] ComputeHeatingSetbackHoursPerMonth(ObjectDataSectionData? objectData, ClimateZoneData? climateData, int yearRef = global::EE.Doklad.CalendarDefaults.ReferenceYear)
        {
            return ComputeBreakdown(
                    objectData?.CalculationMethod ?? HeatingCalculationMethod.Rd0220_3,
                    objectData,
                    climateData)
                .Select(x => x.SetbackHours)
                .ToArray();
        }

        public static HeatingHoursBreakdown[] ComputeBreakdown(
            HeatingCalculationMethod method,
            ObjectDataSectionData? objectData,
            ClimateZoneData? climateData)
        {
            return method switch
            {
                HeatingCalculationMethod.AuerSoftware => ComputeHeatingHoursBreakdownPerMonth_Auer(objectData, climateData),
                _ => ComputeHeatingHoursBreakdownPerMonth_RD(objectData, climateData)
            };
        }

        /// <summary>
        /// Стойности >= 23.98 h (напр. 23:59) се нормализират към 24.0
        /// за да тригерират isFullTime247 guard-а.
        /// При вход 00:00–24:00 GetHours() връща точно 24.0 — нормализацията не е нужна,
        /// но е оставена като safety net за стари данни с 23:59.
        /// </summary>
        private static double NormalizeHeatingHours(double h) =>
            h >= 23.98 ? 24.0 : h;

        /// <summary>
        /// Извлича ч/ден за всеки тип ден.
        /// Приоритет: HeatingSchedules (нов модел) → legacy string полета (fallback).
        /// </summary>
        private static (double workday, double saturday, double sunday) ResolveHeatingHoursPerDayType(
            ObjectDataSectionData objectData)
        {
            var sched = objectData.HeatingSchedules?.HeatingSchedule;
            if (sched != null)
            {
                double wd = NormalizeHeatingHours(sched.Workdays.GetHours());
                double sat = NormalizeHeatingHours(sched.Saturday.GetHours());
                double sun = NormalizeHeatingHours(sched.Sunday.GetHours());
                if (wd > 0 || sat > 0 || sun > 0)
                    return (ClampHours(wd), ClampHours(sat), ClampHours(sun));
            }

            return
            (
                ClampHours(ParseDoubleOrZero(objectData.HeatingWorkdaysHours)),
                ClampHours(ParseDoubleOrZero(objectData.HeatingSaturdayHours)),
                ClampHours(ParseDoubleOrZero(objectData.HeatingSundayHours))
            );
        }

        public static HeatingHoursBreakdown[] ComputeHeatingHoursBreakdownPerMonth(ObjectDataSectionData? objectData, ClimateZoneData? climateData, int yearRef = global::EE.Doklad.CalendarDefaults.ReferenceYear)
        {
            var result = new HeatingHoursBreakdown[12];
            if (objectData == null || climateData == null)
            {
                return result;
            }

            var (workdayHours, saturdayHours, sundayHours) = ResolveHeatingHoursPerDayType(objectData);

            if (workdayHours <= 0 && saturdayHours <= 0 && sundayHours <= 0)
            {
                return result;
            }

            int[] monthlyDaysOff = ParseMonthlyDaysOff(objectData);
            bool isFullTime247 = workdayHours >= 24.0 && saturdayHours >= 24.0 && sundayHours >= 24.0;

            for (int month = 0; month < 12; month++)
            {
                int seasonDays = ScheduleHelper.GetHeatingSeasonDaysInMonth(yearRef, month + 1, climateData);
                if (seasonDays <= 0)
                {
                    result[month] = new HeatingHoursBreakdown(0.0, 0.0);
                    continue;
                }

                if (isFullTime247)
                {
                    result[month] = new HeatingHoursBreakdown(seasonDays * 24.0, 0.0);
                    continue;
                }

                double totalWeeks = seasonDays / 7.0;
                double rawWorkDays = totalWeeks * 5.0;
                double saturdayDays = totalWeeks;
                double sundayDays = totalWeeks;
                double holidayDays = Math.Min(Math.Max(0, monthlyDaysOff[month]), rawWorkDays);
                double workDays = Math.Max(0.0, rawWorkDays - holidayDays);

                double fullHours =
                    workDays * workdayHours +
                    saturdayDays * saturdayHours +
                    sundayDays * sundayHours;

                double setbackHours =
                    workDays * Math.Max(0.0, 24.0 - workdayHours) +
                    saturdayDays * Math.Max(0.0, 24.0 - saturdayHours) +
                    sundayDays * Math.Max(0.0, 24.0 - sundayHours) +
                    holidayDays * 24.0;

                double monthTotalHours = seasonDays * 24.0;
                fullHours = Math.Clamp(fullHours, 0.0, monthTotalHours);
                setbackHours = Math.Clamp(setbackHours, 0.0, monthTotalHours - fullHours);

                result[month] = new HeatingHoursBreakdown(fullHours, setbackHours);
            }

            return result;
        }

        public static HeatingHoursBreakdown[] ComputeHeatingHoursBreakdownPerMonth_Auer(
            ObjectDataSectionData? objectData,
            ClimateZoneData? climateData)
        {
            const int referenceYear = 2006;
            var result = new HeatingHoursBreakdown[12];
            if (objectData == null || climateData == null)
            {
                return result;
            }

            var (workdayHours, saturdayHours, sundayHours) = ResolveHeatingHoursPerDayType(objectData);
            if (workdayHours <= 0 && saturdayHours <= 0 && sundayHours <= 0)
            {
                return result;
            }

            int[] monthlyDaysOff = ParseMonthlyDaysOff(objectData);
            bool isFullTime247 = workdayHours >= 24.0 && saturdayHours >= 24.0 && sundayHours >= 24.0;

            for (int month = 0; month < 12; month++)
            {
                int monthNumber = month + 1;
                int daysInMonth = DateTime.DaysInMonth(referenceYear, monthNumber);
                var (startDay, endDay) = GetHeatingSeasonDayRange(referenceYear, monthNumber, climateData);
                if (endDay < startDay)
                {
                    result[month] = new HeatingHoursBreakdown(0.0, 0.0);
                    continue;
                }

                int seasonDays = endDay - startDay + 1;
                if (seasonDays <= 0)
                {
                    result[month] = new HeatingHoursBreakdown(0.0, 0.0);
                    continue;
                }

                if (isFullTime247)
                {
                    result[month] = new HeatingHoursBreakdown(seasonDays * 24.0, 0.0);
                    continue;
                }

                int workDays;
                int saturdays;
                int sundays;

                if (startDay == 1 && endDay == daysInMonth)
                {
                    (workDays, saturdays, sundays) = CountDayTypesInRange(referenceYear, monthNumber, startDay, endDay);
                }
                else if (startDay > 1 && endDay == daysInMonth)
                {
                    int remaining = daysInMonth - startDay + 1;
                    if (startDay > 21)
                    {
                        workDays = 0;
                        saturdays = 0;
                        sundays = 0;
                    }
                    else if (startDay > 14)
                    {
                        saturdays = 1;
                        sundays = 1;
                        workDays = remaining - 2;
                    }
                    else if (startDay > 7)
                    {
                        saturdays = 2;
                        sundays = 2;
                        workDays = remaining - 4;
                    }
                    else
                    {
                        (workDays, saturdays, sundays) = CountDayTypesInRange(referenceYear, monthNumber, startDay, endDay);
                    }
                }
                else if (startDay == 1 && endDay < daysInMonth)
                {
                    int remaining = endDay;
                    if (endDay < 7)
                    {
                        workDays = 0;
                        saturdays = 0;
                        sundays = 0;
                    }
                    else if (endDay < 14)
                    {
                        saturdays = 1;
                        sundays = 1;
                        workDays = remaining - 2;
                    }
                    else if (endDay < 21)
                    {
                        saturdays = 2;
                        sundays = 2;
                        workDays = remaining - 4;
                    }
                    else
                    {
                        (workDays, saturdays, sundays) = CountDayTypesInRange(referenceYear, monthNumber, startDay, endDay);
                    }
                }
                else
                {
                    (workDays, saturdays, sundays) = CountDayTypesInRange(referenceYear, monthNumber, startDay, endDay);
                }

                workDays = Math.Max(0, workDays);
                result[month] = BuildBreakdown(
                    workDays,
                    saturdays,
                    sundays,
                    monthlyDaysOff[month],
                    workdayHours,
                    saturdayHours,
                    sundayHours);
            }

            return result;
        }

        public static HeatingHoursBreakdown[] ComputeHeatingHoursBreakdownPerMonth_RD(
            ObjectDataSectionData? objectData,
            ClimateZoneData? climateData)
        {
            const int referenceYear = global::EE.Doklad.CalendarDefaults.ReferenceYear;
            var result = new HeatingHoursBreakdown[12];
            if (objectData == null || climateData == null)
            {
                return result;
            }

            var (workdayHours, saturdayHours, sundayHours) = ResolveHeatingHoursPerDayType(objectData);
            if (workdayHours <= 0 && saturdayHours <= 0 && sundayHours <= 0)
            {
                return result;
            }

            int[] monthlyDaysOff = ParseMonthlyDaysOff(objectData);
            bool isFullTime247 = workdayHours >= 24.0 && saturdayHours >= 24.0 && sundayHours >= 24.0;

            for (int month = 0; month < 12; month++)
            {
                int monthNumber = month + 1;
                var (startDay, endDay) = GetHeatingSeasonDayRange(referenceYear, monthNumber, climateData);
                if (endDay < startDay)
                {
                    result[month] = new HeatingHoursBreakdown(0.0, 0.0);
                    continue;
                }

                int seasonDays = endDay - startDay + 1;
                if (seasonDays <= 0)
                {
                    result[month] = new HeatingHoursBreakdown(0.0, 0.0);
                    continue;
                }

                if (isFullTime247)
                {
                    result[month] = new HeatingHoursBreakdown(seasonDays * 24.0, 0.0);
                    continue;
                }

                var (workDays, saturdays, sundays) = CountDayTypesInRange(referenceYear, monthNumber, startDay, endDay);
                result[month] = BuildBreakdown(
                    workDays,
                    saturdays,
                    sundays,
                    monthlyDaysOff[month],
                    workdayHours,
                    saturdayHours,
                    sundayHours);
            }

            return result;
        }

        private static double ParseDoubleOrZero(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0.0;
            if (double.TryParse(s.Trim(), out var v)) return v;
            return 0.0;
        }

        private static double ClampHours(double hours)
        {
            return Math.Clamp(hours, 0.0, 24.0);
        }

        private static int[] ParseMonthlyDaysOff(ObjectDataSectionData objectData)
        {
            int ParseMonth(string? s) { if (string.IsNullOrWhiteSpace(s)) return 0; if (int.TryParse(s.Trim(), out var v)) return Math.Max(0, v); return 0; }
            return
            [
                ParseMonth(objectData.DaysOffJanuary),
                ParseMonth(objectData.DaysOffFebruary),
                ParseMonth(objectData.DaysOffMarch),
                ParseMonth(objectData.DaysOffApril),
                ParseMonth(objectData.DaysOffMay),
                ParseMonth(objectData.DaysOffJune),
                ParseMonth(objectData.DaysOffJuly),
                ParseMonth(objectData.DaysOffAugust),
                ParseMonth(objectData.DaysOffSeptember),
                ParseMonth(objectData.DaysOffOctober),
                ParseMonth(objectData.DaysOffNovember),
                ParseMonth(objectData.DaysOffDecember)
            ];
        }

        private static bool TryParseMonthDay(string s, out int month, out int day)
        {
            month = 1; day = 1;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var parts = s.Split('-');
            if (parts.Length != 2) return false;
            if (!int.TryParse(parts[0], out month)) return false;
            if (!int.TryParse(parts[1], out day)) return false;
            return true;
        }

        private static int GetEffectiveHeatingYear(int yearRef, int monthNumber, ClimateZoneData? climateData)
        {
            if (climateData?.HeatingSeason == null ||
                string.IsNullOrWhiteSpace(climateData.HeatingSeason.Start) ||
                string.IsNullOrWhiteSpace(climateData.HeatingSeason.End) ||
                !TryParseMonthDay(climateData.HeatingSeason.Start, out int startM, out int startD) ||
                !TryParseMonthDay(climateData.HeatingSeason.End, out int endM, out int endD))
            {
                return yearRef;
            }

            bool wrapsYear = endM < startM || (endM == startM && endD < startD);
            return wrapsYear && monthNumber <= endM ? yearRef + 1 : yearRef;
        }

        private static (int StartDay, int EndDay) GetHeatingSeasonDayRange(int yearRef, int monthNumber, ClimateZoneData? climateData)
        {
            int effectiveYear = GetEffectiveHeatingYear(yearRef, monthNumber, climateData);
            int daysInMonth = DateTime.DaysInMonth(effectiveYear, monthNumber);

            if (climateData?.HeatingSeason == null ||
                string.IsNullOrWhiteSpace(climateData.HeatingSeason.Start) ||
                string.IsNullOrWhiteSpace(climateData.HeatingSeason.End) ||
                !TryParseMonthDay(climateData.HeatingSeason.Start, out int startM, out int startD) ||
                !TryParseMonthDay(climateData.HeatingSeason.End, out int endM, out int endD))
            {
                return (1, daysInMonth);
            }

            int startMonthDays = DateTime.DaysInMonth(yearRef, startM);
            int endMonthDays = DateTime.DaysInMonth(GetEffectiveHeatingYear(yearRef, endM, climateData), endM);
            startD = Math.Min(startD, startMonthDays);
            endD = Math.Min(endD, endMonthDays);

            bool wrapsYear = endM < startM || (endM == startM && endD < startD);
            bool monthInSeason = wrapsYear
                ? monthNumber >= startM || monthNumber <= endM
                : monthNumber >= startM && monthNumber <= endM;

            if (!monthInSeason)
            {
                return (1, 0);
            }

            if (startM == endM && !wrapsYear)
            {
                return (Math.Min(startD, daysInMonth), Math.Min(endD, daysInMonth));
            }

            if (monthNumber == startM)
            {
                return (Math.Min(startD, daysInMonth), daysInMonth);
            }

            if (monthNumber == endM)
            {
                return (1, Math.Min(endD, daysInMonth));
            }

            return (1, daysInMonth);
        }

        private static (int WorkDays, int Saturdays, int Sundays) CountDayTypesInRange(
            int year,
            int month,
            int startDay,
            int endDay)
        {
            int workDays = 0;
            int saturdays = 0;
            int sundays = 0;

            for (int day = startDay; day <= endDay; day++)
            {
                switch (new DateTime(year, month, day).DayOfWeek)
                {
                    case DayOfWeek.Saturday:
                        saturdays++;
                        break;
                    case DayOfWeek.Sunday:
                        sundays++;
                        break;
                    default:
                        workDays++;
                        break;
                }
            }

            return (workDays, saturdays, sundays);
        }

        private static HeatingHoursBreakdown BuildBreakdown(
            int workDays,
            int saturdays,
            int sundays,
            int monthlyDaysOff,
            double workdayHours,
            double saturdayHours,
            double sundayHours)
        {
            int holidayDays = Math.Min(Math.Max(0, monthlyDaysOff), Math.Max(0, workDays));
            workDays = Math.Max(0, workDays - holidayDays);

            double fullHours =
                workDays * workdayHours +
                saturdays * saturdayHours +
                sundays * sundayHours;

            double setbackHours =
                workDays * Math.Max(0.0, 24.0 - workdayHours) +
                saturdays * Math.Max(0.0, 24.0 - saturdayHours) +
                sundays * Math.Max(0.0, 24.0 - sundayHours) +
                holidayDays * 24.0;

            return new HeatingHoursBreakdown(fullHours, setbackHours);
        }

        private static bool IsDateInRange(DateTime dt, DateTime start, DateTime end)
        {
            return dt >= start && dt <= end;
        }

        /// <summary>
        /// Compute cooling hours per month from object data cooling schedule.
        /// If no cooling schedule values provided -> returns zeros.
        /// Does not use heating season.
        /// </summary>
        public static double[] ComputeCoolingHoursPerMonth(ObjectDataSectionData? objectData, int yearRef = global::EE.Doklad.CalendarDefaults.ReferenceYear)
        {
            var result = new double[12];
            if (objectData == null) return result;

            // Prefer the new time-range schedule model (Section 5, Graph B).
            double workdayHours = objectData.CoolingSchedules?.CoolingSchedule?.Workdays?.GetHours() ?? 0.0;
            double saturdayHours = objectData.CoolingSchedules?.CoolingSchedule?.Saturday?.GetHours() ?? 0.0;
            double sundayHours = objectData.CoolingSchedules?.CoolingSchedule?.Sunday?.GetHours() ?? 0.0;

            // Backward compatibility: legacy numeric fields.
            if (workdayHours <= 0 && saturdayHours <= 0 && sundayHours <= 0)
            {
                workdayHours = ParseDoubleOrZero(objectData.CoolingWorkdaysHours);
                saturdayHours = ParseDoubleOrZero(objectData.CoolingSaturdayHours);
                sundayHours = ParseDoubleOrZero(objectData.CoolingSundayHours);
            }

            if (workdayHours <= 0 && saturdayHours <= 0 && sundayHours <= 0)
            {
                return result;
            }

            for (int m = 0; m < 12; m++)
            {
                int monthNumber = m + 1;
                int daysInMonth = DateTime.DaysInMonth(yearRef, monthNumber);
                int workdayCount = 0, saturdayCount = 0, sundayCount = 0;
                int seasonDays = 0;
                for (int d = 1; d <= daysInMonth; d++)
                {
                    var dt = new DateTime(yearRef, monthNumber, d);
                    if (!IsDateInCoolingSeason(dt, objectData, yearRef))
                        continue;

                    seasonDays++;
                    switch (dt.DayOfWeek)
                    {
                        case DayOfWeek.Saturday: saturdayCount++; break;
                        case DayOfWeek.Sunday: sundayCount++; break;
                        default: workdayCount++; break;
                    }
                }

                if (seasonDays == 0)
                {
                    result[m] = 0.0;
                    continue;
                }

                double baseHours = workdayCount * workdayHours + saturdayCount * saturdayHours + sundayCount * sundayHours;

                // Subtract holidays (treat as neutral: reduce by avg daily hours weighted)
                int holidays = 0;
                int ParseMonth(string? s) { if (string.IsNullOrWhiteSpace(s)) return 0; if (int.TryParse(s.Trim(), out var v)) return Math.Max(0, v); return 0; }
                switch (m)
                {
                    case 0: holidays = ParseMonth(objectData.DaysOffJanuary); break;
                    case 1: holidays = ParseMonth(objectData.DaysOffFebruary); break;
                    case 2: holidays = ParseMonth(objectData.DaysOffMarch); break;
                    case 3: holidays = ParseMonth(objectData.DaysOffApril); break;
                    case 4: holidays = ParseMonth(objectData.DaysOffMay); break;
                    case 5: holidays = ParseMonth(objectData.DaysOffJune); break;
                    case 6: holidays = ParseMonth(objectData.DaysOffJuly); break;
                    case 7: holidays = ParseMonth(objectData.DaysOffAugust); break;
                    case 8: holidays = ParseMonth(objectData.DaysOffSeptember); break;
                    case 9: holidays = ParseMonth(objectData.DaysOffOctober); break;
                    case 10: holidays = ParseMonth(objectData.DaysOffNovember); break;
                    case 11: holidays = ParseMonth(objectData.DaysOffDecember); break;
                }

                double reduction = 0.0;
                if (holidays > 0 && seasonDays > 0)
                {
                    double avgDaily = baseHours / (double)seasonDays;
                    reduction = Math.Min(baseHours, holidays * avgDaily);
                }

                result[m] = Math.Max(0.0, baseHours - reduction);
            }

            return result;
        }

        private static bool IsDateInCoolingSeason(DateTime dt, ObjectDataSectionData objectData, int yearRef)
        {
            if (!objectData.CoolingSeasonStartMonth.HasValue || !objectData.CoolingSeasonStartDay.HasValue ||
                !objectData.CoolingSeasonEndMonth.HasValue || !objectData.CoolingSeasonEndDay.HasValue)
            {
                return true;
            }

            int sm = objectData.CoolingSeasonStartMonth.Value;
            int sd = objectData.CoolingSeasonStartDay.Value;
            int em = objectData.CoolingSeasonEndMonth.Value;
            int ed = objectData.CoolingSeasonEndDay.Value;

            var seasonStart = new DateTime(yearRef, sm, Math.Min(sd, DateTime.DaysInMonth(yearRef, sm)));
            var seasonEnd = new DateTime(yearRef, em, Math.Min(ed, DateTime.DaysInMonth(yearRef, em)));
            if (seasonEnd < seasonStart)
                seasonEnd = seasonEnd.AddYears(1);

            return IsDateInRange(dt, seasonStart, seasonEnd) || IsDateInRange(dt.AddYears(1), seasonStart, seasonEnd);
        }
    }
}
