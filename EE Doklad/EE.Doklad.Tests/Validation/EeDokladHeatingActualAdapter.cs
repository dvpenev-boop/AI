using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EE.Doklad.Models;
using EE.Doklad.Sections.Section11Heating.Models;
using EE.Doklad.Sections.Section11Heating.Services;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EeDokladHeatingActualAdapter
    {
        public EecalcActualSnapshot CalculateActual(
            EecalcValidationFixture fixture,
            IReadOnlyList<EecalcMonthlyDaysOracleRow> monthlyDays)
        {
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(monthlyDays);

            var hve = fixture.HeatedVolume * fixture.Infiltration * 0.34;
            var heatingMonths = monthlyDays.Select(row => row.Month - 1).ToArray();
            var monthlyIndoorTemps = new double[12];
            var seasonalHours = new int[12];
            foreach (var row in monthlyDays)
            {
                var projectHours = row.WorkDays * Duration(fixture.WorkdaySchedule)
                    + row.Saturdays * Duration(fixture.SaturdaySchedule)
                    + row.Sundays * Duration(fixture.SundaySchedule);
                var nonProjectHours = row.WorkDays * (24.0 - Duration(fixture.WorkdaySchedule))
                    + row.Saturdays * (24.0 - Duration(fixture.SaturdaySchedule))
                    + row.Sundays * (24.0 - Duration(fixture.SundaySchedule))
                    + row.Holidays * 24.0;
                var totalHours = projectHours + nonProjectHours;

                seasonalHours[row.Month - 1] = (int)Math.Round(totalHours);
                monthlyIndoorTemps[row.Month - 1] = totalHours > 0.0
                    ? (fixture.ProjectTemperature * projectHours + fixture.NonProjectTemperature * nonProjectHours) / totalHours
                    : fixture.ProjectTemperature;
            }

            var service = new HeatingCalculationService();
            var result = service.Calculate(
                HeatingCalculationMethod.AuerSoftware,
                Array.Empty<WallData>(),
                Array.Empty<WindowData>(),
                Array.Empty<RoofData>(),
                htr: 0.0,
                hve: hve,
                cm: 0.0,
                thetaI: fixture.ProjectTemperature,
                area: fixture.HeatedArea,
                climateZone: fixture.ClimateZoneId > 0 ? fixture.ClimateZoneId : 9,
                heatingMonths: heatingMonths,
                monthlyIndoorTemps: monthlyIndoorTemps,
                seasonalHoursByMonth: seasonalHours,
                getQint: _ => 0.0);

            return new EecalcActualSnapshot
            {
                FixtureName = fixture.Id,
                Scenario = fixture.Scenario,
                Source = "EE.Doklad HeatingCalculationService",
                Months = result.Monthly.Select(row =>
                {
                    var monthNumber = row.MonthIndex + 1;
                    var days = monthlyDays.Single(expected => expected.Month == monthNumber);
                    return new EecalcMonthlySnapshotRow
                    {
                        Month = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(monthNumber),
                        WorkDays = days.WorkDays,
                        Saturdays = days.Saturdays,
                        Sundays = days.Sundays,
                        Holidays = days.Holidays,
                        TotalDays = days.TotalDays,
                        Weeks = days.Weeks,
                        Hve = result.Annual.Hve,
                        Htr = null,
                        Qtr = null,
                        Qve = row.Qht,
                        Qgn = null,
                        Gamma = null,
                        Ni = null,
                        Qnd = row.QH,
                        QndPerArea = fixture.HeatedArea > 0.0 ? row.QH / fixture.HeatedArea : 0.0
                    };
                }).ToList()
            };
        }

        private static int Duration(EecalcDailySchedule schedule)
        {
            return schedule.EndHour - schedule.StartHour;
        }
    }
}
