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
            int daysInMonth = DateTime.DaysInMonth(yearRef, monthNumber);

            if (climateData?.HeatingSeason == null || string.IsNullOrWhiteSpace(climateData.HeatingSeason.Start) || string.IsNullOrWhiteSpace(climateData.HeatingSeason.End))
            {
                return daysInMonth;
            }

            if (!TryParseMonthDay(climateData.HeatingSeason.Start, out int startM, out int startD) ||
                !TryParseMonthDay(climateData.HeatingSeason.End, out int endM, out int endD))
            {
                return daysInMonth;
            }

            DateTime startDate = new DateTime(yearRef, startM, Math.Min(startD, DateTime.DaysInMonth(yearRef, startM)));
            DateTime endDate = new DateTime(yearRef, endM, Math.Min(endD, DateTime.DaysInMonth(yearRef, endM)));
            if (endDate < startDate)
            {
                endDate = endDate.AddYears(1);
            }

            int count = 0;
            for (int d = 1; d <= daysInMonth; d++)
            {
                DateTime dt = new DateTime(yearRef, monthNumber, d);
                if (IsDateInRange(dt, startDate, endDate) || IsDateInRange(dt.AddYears(1), startDate, endDate))
                {
                    count++;
                }
            }

            return count;
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
        /// Compute effective indoor heating temperature per month (θint for heating regime) given object data and heating data.
        /// Simple model: during 'heating hours' temperature == designTemperature; during other heating-season hours == reductionTemperature.
        /// If heating schedule is missing, fallback to heatingData.DesignTemperature (or 20°C if null).
        /// Holidays (monthlyDaysOff) reduce heating-season days similarly to other modules.
        /// </summary>
        public static double[] ComputeThetaIntCalcH(
            ObjectDataSectionData? objectData,
            HeatingSectionData? heatingData,
            ClimateZoneData? climateData,
            int yearRef = 2024)
        {
            var result = new double[12];

            // Fallbacks
            double fallbackDesign = heatingData != null ? heatingData.DesignTemperature : 20.0;

            // Parse heating schedule hours (ч./ден) - if missing we consider insufficient data
            bool hasSchedule = false;
            double workdayHours = ParseDoubleOrZero(objectData?.HeatingWorkdaysHours);
            double saturdayHours = ParseDoubleOrZero(objectData?.HeatingSaturdayHours);
            double sundayHours = ParseDoubleOrZero(objectData?.HeatingSundayHours);
            double hoursPerWeek = workdayHours * 5.0 + saturdayHours + sundayHours;
            double hoursPerDayEquivalent = hoursPerWeek / 7.0;
            if (workdayHours > 0 || saturdayHours > 0 || sundayHours > 0)
                hasSchedule = true;

            // Build monthly days-off array
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
                // Determine heating-season days in month
                int daysInHeatingSeason = GetHeatingSeasonDaysInMonth(yearRef, m + 1, climateData);

                // Subtract monthly days-off that fall in heating-season portion (same rule as other modules)
                if (monthlyDaysOff != null)
                {
                    int holidays = Math.Max(0, monthlyDaysOff[m]);
                    if (holidays > 0 && daysInHeatingSeason > 0)
                    {
                        daysInHeatingSeason = Math.Max(0, daysInHeatingSeason - Math.Min(holidays, daysInHeatingSeason));
                    }
                }

                if (!hasSchedule || daysInHeatingSeason == 0)
                {
                    // insufficient data -> fallback to design temperature
                    result[m] = fallbackDesign;
                    continue;
                }

                // heated hours in month
                double heatedHours = hoursPerDayEquivalent * daysInHeatingSeason;
                double totalHours = daysInHeatingSeason * 24.0;

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
    }
}
