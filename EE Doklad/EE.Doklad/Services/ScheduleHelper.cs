using System;
using EE.Doklad.Models;

namespace EE.Doklad.Services
{
    /// <summary>
    /// Helper for heating-season days and simple effective indoor temperature computation
    /// (used by Unconditioned Zones and other sections).
    /// </summary>
    public static class ScheduleHelper
    {
        // Copy of logic used in ventilation to compute heating-season days per month.
        public static int GetHeatingSeasonDaysInMonth(int yearRef, int monthNumber, ClimateZoneData? climateData)
        {
            if (climateData?.HeatingSeason == null || string.IsNullOrWhiteSpace(climateData.HeatingSeason.Start) || string.IsNullOrWhiteSpace(climateData.HeatingSeason.End))
            {
                return DateTime.DaysInMonth(yearRef, monthNumber);
            }

            if (!TryParseMonthDay(climateData.HeatingSeason.Start, out int startM, out int startD) ||
                !TryParseMonthDay(climateData.HeatingSeason.End, out int endM, out int endD))
            {
                return DateTime.DaysInMonth(yearRef, monthNumber);
            }

            bool wrapsYear = endM < startM || (endM == startM && endD < startD);
            int effectiveYear = wrapsYear && monthNumber <= endM
                ? yearRef + 1
                : yearRef;
            int daysInMonth = DateTime.DaysInMonth(effectiveYear, monthNumber);
            int startMonthDays = DateTime.DaysInMonth(yearRef, startM);
            int endMonthDays = DateTime.DaysInMonth(wrapsYear ? yearRef + 1 : yearRef, endM);

            startD = Math.Min(startD, startMonthDays);
            endD = Math.Min(endD, endMonthDays);

            bool monthInSeason = wrapsYear
                ? monthNumber >= startM || monthNumber <= endM
                : monthNumber >= startM && monthNumber <= endM;

            if (!monthInSeason)
            {
                return 0;
            }

            if (startM == endM && !wrapsYear)
            {
                return Math.Max(0, endD - startD);
            }

            if (monthNumber == startM)
            {
                return Math.Max(0, daysInMonth - startD);
            }

            if (monthNumber == endM)
            {
                return Math.Max(0, Math.Min(endD, daysInMonth));
            }

            return daysInMonth;
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
        /// Compute effective indoor cooling temperature per month (theta_int for cooling regime).
        /// Uses cooling schedule from section 5, holidays and cooling season (start/end day+month).
        /// During active cooling hours temperature == design; during other in-season hours == elevated/reduction temp.
        /// </summary>
        public static double[] ComputeThetaIntCalcC(
            ObjectDataSectionData? objectData,
            CoolingSectionData? coolingData,
            int yearRef = global::EE.Doklad.CalendarDefaults.ReferenceYear)
        {
            var result = new double[12];

            double designTemp = coolingData?.DesignTemperature ?? 25.0;
            // For cooling, "reduction" means relaxed (typically higher) setpoint.
            double elevatedTemp = coolingData != null
                ? Math.Max(coolingData.ReductionTemperature, designTemp)
                : designTemp + 2.0;

            // Prefer new Section 5 cooling schedule (Graph B time ranges).
            double workdayHours = objectData?.CoolingSchedules?.CoolingSchedule?.Workdays?.GetHours() ?? 0.0;
            double saturdayHours = objectData?.CoolingSchedules?.CoolingSchedule?.Saturday?.GetHours() ?? 0.0;
            double sundayHours = objectData?.CoolingSchedules?.CoolingSchedule?.Sunday?.GetHours() ?? 0.0;

            // Backward compatibility with legacy numeric fields.
            if (workdayHours <= 0 && saturdayHours <= 0 && sundayHours <= 0)
            {
                workdayHours = ParseDoubleOrZero(objectData?.CoolingWorkdaysHours);
                saturdayHours = ParseDoubleOrZero(objectData?.CoolingSaturdayHours);
                sundayHours = ParseDoubleOrZero(objectData?.CoolingSundayHours);
            }
            bool hasSchedule = workdayHours > 0 || saturdayHours > 0 || sundayHours > 0;

            int[] monthlyDaysOff = new int[12];
            if (objectData != null)
            {
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
            }

            for (int m = 0; m < 12; m++)
            {
                int monthNumber = m + 1;
                int daysInMonth = DateTime.DaysInMonth(yearRef, monthNumber);

                int workdayCount = 0;
                int saturdayCount = 0;
                int sundayCount = 0;
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

                if (!hasSchedule || seasonDays == 0)
                {
                    result[m] = designTemp;
                    continue;
                }

                double coolingHours = workdayCount * workdayHours + saturdayCount * saturdayHours + sundayCount * sundayHours;
                int holidays = Math.Max(0, monthlyDaysOff[m]);
                if (holidays > 0 && seasonDays > 0)
                {
                    double avgDaily = coolingHours / seasonDays;
                    coolingHours = Math.Max(0.0, coolingHours - Math.Min(coolingHours, holidays * avgDaily));
                }

                double totalSeasonHours = seasonDays * 24.0;
                result[m] = GetEffectiveHeatingIndoorTemp(designTemp, elevatedTemp, coolingHours, totalSeasonHours);
            }

            return result;
        }

        /// <summary>
        /// Compute effective indoor heating temperature per month (θint for heating regime) given object data and heating data.
        /// Simple model: during 'heating hours' temperature == designTemperature; during other heating-season hours == reductionTemperature.
        /// If heating schedule is missing, fallback to heatingData.DesignTemperature (or 20°C if null).
        /// Holidays (monthlyDaysOff) reduce heating-season days similarly to other modules.
        /// </summary>
        public static double[] ComputeThetaIntCalcH(
            ObjectDataSectionData? objectData,
            HeatingSectionData? heatingData,
            ClimateZoneData? climateData,
            int yearRef = global::EE.Doklad.CalendarDefaults.ReferenceYear)
        {
            var result = new double[12];

            // Fallbacks
            double fallbackDesign = heatingData != null ? heatingData.DesignTemperature : 20.0;

            var monthlyBreakdown = HeatingScheduleService.ComputeHeatingHoursBreakdownPerMonth(objectData, climateData, yearRef);
            bool hasSchedule = monthlyBreakdown.Any(x => x.FullHours > 0.0 || x.SetbackHours > 0.0);

            for (int m = 0; m < 12; m++)
            {
                int daysInHeatingSeason = GetHeatingSeasonDaysInMonth(yearRef, m + 1, climateData);

                if (!hasSchedule || daysInHeatingSeason == 0)
                {
                    // insufficient data -> fallback to design temperature
                    result[m] = fallbackDesign;
                    continue;
                }

                double heatedHours = monthlyBreakdown[m].FullHours;
                double totalHours = monthlyBreakdown[m].FullHours + monthlyBreakdown[m].SetbackHours;

                // If for some reason totalHours is zero, fallback
                if (totalHours <= 0.0)
                {
                    result[m] = fallbackDesign;
                    continue;
                }

                double designTemp = heatingData != null ? heatingData.DesignTemperature : fallbackDesign;
                double reductionTemp = heatingData != null ? heatingData.ReductionTemperature : Math.Max(0.0, fallbackDesign - 4.0);

                // Use the small utility to compute effective indoor temperature for the month
                result[m] = GetEffectiveHeatingIndoorTemp(designTemp, reductionTemp, heatedHours, totalHours);
            }

            return result;
        }

        /// <summary>
        /// Compute simple effective indoor heating temperature for a month.
        /// f = clamp(heatingHours / hoursInMonth, 0..1)
        /// theta_eff = f * designTemp + (1-f) * reductionTemp
        /// This makes behavior explicit and testable.
        /// </summary>
        public static double GetEffectiveHeatingIndoorTemp(double designTemp, double reductionTemp, double heatingHours, double hoursInMonth)
        {
            if (hoursInMonth <= 0.0) return designTemp;
            double f = heatingHours / hoursInMonth;
            if (double.IsNaN(f) || double.IsInfinity(f)) f = 0.0;
            f = Math.Max(0.0, Math.Min(1.0, f));
            return f * designTemp + (1.0 - f) * reductionTemp;
        }

        private static double ParseDoubleOrZero(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0.0;
            if (double.TryParse(s.Trim(), out var v)) return v;
            return 0.0;
        }

        private static bool IsDateInCoolingSeason(DateTime dt, ObjectDataSectionData? objectData, int yearRef)
        {
            if (objectData == null ||
                !objectData.CoolingSeasonStartMonth.HasValue || !objectData.CoolingSeasonStartDay.HasValue ||
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
