using System;
using EE.Doklad.Models;

namespace EE.Doklad.Services
{
    /// <summary>
    /// Service that computes heating hours per month based on ObjectData schedules, holidays and climate heating season.
    /// </summary>
    public static class HeatingScheduleService
    {
        /// <summary>
        /// Compute heating hours per month [h] using ObjectData heating schedule and climate heating-season.
        /// If schedule fields are missing/empty -> returns zeros (caller may fallback).
        /// Holidays (monthly days off) reduce heating-season days using the same rule as other modules.
        /// </summary>
        public static double[] ComputeHeatingHoursPerMonth(ObjectDataSectionData? objectData, ClimateZoneData? climateData, int yearRef = 2024)
        {
            var result = new double[12];

            if (objectData == null || climateData == null)
            {
                return result;
            }

            // Parse schedule hours (ч./ден)
            double workdayHours = ParseDoubleOrZero(objectData.HeatingWorkdaysHours);
            double saturdayHours = ParseDoubleOrZero(objectData.HeatingSaturdayHours);
            double sundayHours = ParseDoubleOrZero(objectData.HeatingSundayHours);

            // If no schedule defined, return zeros (caller will fallback to design temperature)
            if (workdayHours <= 0 && saturdayHours <= 0 && sundayHours <= 0)
            {
                return result;
            }

            // Build monthly days-off array
            int[] monthlyDaysOff = new int[12];
            int ParseMonth(string? s) { if (string.IsNullOrWhiteSpace(s)) return 0; if (int.TryParse(s.Trim(), out var v)) return Math.Max(0, v); return 0; }
            monthlyDaysOff[0] = ParseMonth(objectData.DaysOffJanuary);
            monthlyDaysOff[1] = ParseMonth(objectData.DaysOffFebruary);
            monthlyDaysOff[2] = ParseMonth(objectData.DaysOffMarch);
            monthlyDaysOff[3] = ParseMonth(objectData.DaysOffApril);
            monthlyDaysOff[4] = ParseMonth(objectData.DaysOffMay);
            monthlyDaysOff[5] = ParseMonth(objectData.DaysOffJune);
            monthlyDaysOff[6] = ParseMonth(objectData.DaysOffJuly);
            monthlyDaysOff[7] = ParseMonth(objectData.DaysOffAugust);
            monthlyDaysOff[8] = ParseMonth(objectData.DaysOffSeptember);
            monthlyDaysOff[9] = ParseMonth(objectData.DaysOffOctober);
            monthlyDaysOff[10] = ParseMonth(objectData.DaysOffNovember);
            monthlyDaysOff[11] = ParseMonth(objectData.DaysOffDecember);

            for (int m = 0; m < 12; m++)
            {
                int monthNumber = m + 1;
                int daysInMonth = DateTime.DaysInMonth(yearRef, monthNumber);

                // Count actual weekdays/saturdays/sundays that fall into the heating season
                int workdayCount = 0;
                int saturdayCount = 0;
                int sundayCount = 0;
                int heatingSeasonDays = 0;

                // Prepare heating season range if available
                DateTime? seasonStart = null;
                DateTime? seasonEnd = null;
                if (climateData?.HeatingSeason != null && !string.IsNullOrWhiteSpace(climateData.HeatingSeason.Start) && !string.IsNullOrWhiteSpace(climateData.HeatingSeason.End))
                {
                    if (TryParseMonthDay(climateData.HeatingSeason.Start, out int sm, out int sd) && TryParseMonthDay(climateData.HeatingSeason.End, out int em, out int ed))
                    {
                        seasonStart = new DateTime(yearRef, sm, Math.Min(sd, DateTime.DaysInMonth(yearRef, sm)));
                        seasonEnd = new DateTime(yearRef, em, Math.Min(ed, DateTime.DaysInMonth(yearRef, em)));
                        if (seasonEnd < seasonStart) seasonEnd = seasonEnd.Value.AddYears(1);
                    }
                }

                for (int d = 1; d <= daysInMonth; d++)
                {
                    var dt = new DateTime(yearRef, monthNumber, d);
                    bool inSeason = true;
                    if (seasonStart.HasValue && seasonEnd.HasValue)
                    {
                        inSeason = IsDateInRange(dt, seasonStart.Value, seasonEnd.Value) || IsDateInRange(dt.AddYears(1), seasonStart.Value, seasonEnd.Value);
                    }
                    if (!inSeason) continue;

                    heatingSeasonDays++;
                    switch (dt.DayOfWeek)
                    {
                        case DayOfWeek.Saturday: saturdayCount++; break;
                        case DayOfWeek.Sunday: sundayCount++; break;
                        default: workdayCount++; break;
                    }
                }

                // Initial base hours = counts * schedule
                double baseHours = workdayCount * workdayHours + saturdayCount * saturdayHours + sundayCount * sundayHours;

                // Subtract holidays that fall within heating-season portion
                int holidays = Math.Max(0, monthlyDaysOff[m]);
                double reductionHours = 0.0;
                if (holidays > 0 && heatingSeasonDays > 0)
                {
                    // Neutral approach: reduce by holidays * averageDailyHours (consistent with previous behavior that reduced days)
                    double avgDailyHours = baseHours / (double)heatingSeasonDays;
                    reductionHours = Math.Min(baseHours, holidays * avgDailyHours);
                }

                double heatingHours = Math.Max(0.0, baseHours - reductionHours);

                // Clamp to month total hours
                double monthTotalHours = daysInMonth * 24.0;
                if (heatingHours > monthTotalHours) heatingHours = monthTotalHours;

                result[m] = heatingHours;
            }

            return result;
        }

        private static double ParseDoubleOrZero(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0.0;
            if (double.TryParse(s.Trim(), out var v)) return v;
            return 0.0;
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

        private static bool IsDateInRange(DateTime dt, DateTime start, DateTime end)
        {
            return dt >= start && dt <= end;
        }

        /// <summary>
        /// Compute cooling hours per month from object data cooling schedule.
        /// If no cooling schedule values provided -> returns zeros.
        /// Does not use heating season.
        /// </summary>
        public static double[] ComputeCoolingHoursPerMonth(ObjectDataSectionData? objectData, int yearRef = 2024)
        {
            var result = new double[12];
            if (objectData == null) return result;

            double workdayHours = ParseDoubleOrZero(objectData.CoolingWorkdaysHours);
            double saturdayHours = ParseDoubleOrZero(objectData.CoolingSaturdayHours);
            double sundayHours = ParseDoubleOrZero(objectData.CoolingSundayHours);

            if (workdayHours <= 0 && saturdayHours <= 0 && sundayHours <= 0)
            {
                return result;
            }

            for (int m = 0; m < 12; m++)
            {
                int monthNumber = m + 1;
                int daysInMonth = DateTime.DaysInMonth(yearRef, monthNumber);
                int workdayCount = 0, saturdayCount = 0, sundayCount = 0;
                for (int d = 1; d <= daysInMonth; d++)
                {
                    var dt = new DateTime(yearRef, monthNumber, d);
                    switch (dt.DayOfWeek)
                    {
                        case DayOfWeek.Saturday: saturdayCount++; break;
                        case DayOfWeek.Sunday: sundayCount++; break;
                        default: workdayCount++; break;
                    }
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
                if (holidays > 0 && daysInMonth > 0)
                {
                    double avgDaily = baseHours / (double)daysInMonth;
                    reduction = Math.Min(baseHours, holidays * avgDaily);
                }

                result[m] = Math.Max(0.0, baseHours - reduction);
            }

            return result;
        }
    }
}
