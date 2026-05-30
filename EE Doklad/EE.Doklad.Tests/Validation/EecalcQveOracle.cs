using System;
using System.Collections.Generic;
using System.Linq;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcQveOracle
    {
        public double CalculateHve(EecalcValidationFixture fixture)
        {
            ArgumentNullException.ThrowIfNull(fixture);

            return fixture.HeatedVolume * fixture.Infiltration * 0.34;
        }

        public IReadOnlyList<EecalcHeatingMonthlyBalanceRow> Calculate(
            EecalcValidationFixture fixture,
            IReadOnlyList<EecalcMonthlyDaysOracleRow> monthlyDays)
        {
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(monthlyDays);

            var hve = CalculateHve(fixture);
            return monthlyDays.Select(month =>
            {
                var avgOutdoorTemp = fixture.AverageOutdoorTemperatureByMonth.TryGetValue(month.Month, out var value)
                    ? value
                    : 0.0;
                var projectHours = CalculateProjectHours(fixture, month);
                var nonProjectHours = CalculateNonProjectHours(fixture, month);
                var deltaProject = (fixture.ProjectTemperature - avgOutdoorTemp) * projectHours;
                var deltaNonProject = (fixture.NonProjectTemperature - avgOutdoorTemp) * nonProjectHours;
                var qve = hve * (deltaProject + deltaNonProject) / 1000.0;

                return new EecalcHeatingMonthlyBalanceRow
                {
                    Month = month.Month,
                    MonthName = month.MonthName,
                    AverageOutdoorTemperature = avgOutdoorTemp,
                    ProjectTemperature = fixture.ProjectTemperature,
                    NonProjectTemperature = fixture.NonProjectTemperature,
                    ProjectHours = projectHours,
                    NonProjectHours = nonProjectHours,
                    DeltaProject = deltaProject,
                    DeltaNonProject = deltaNonProject,
                    Hve = hve,
                    Qve = qve,
                    Htr = null,
                    Qtr = null,
                    Qgn = null,
                    Gamma = null,
                    Ni = null,
                    Qht = qve,
                    NetEnergyRaw = qve,
                    NetEnergyPerArea = fixture.HeatedArea > 0.0 ? qve / fixture.HeatedArea : 0.0
                };
            }).ToList();
        }

        public EecalcExpectedSnapshot CreateExpectedSnapshot(
            EecalcValidationFixture fixture,
            IReadOnlyList<EecalcMonthlyDaysOracleRow> monthlyDays,
            IReadOnlyList<EecalcHeatingMonthlyBalanceRow> balanceRows)
        {
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(monthlyDays);
            ArgumentNullException.ThrowIfNull(balanceRows);

            var balanceByMonth = balanceRows.ToDictionary(row => row.Month);
            return new EecalcExpectedSnapshot
            {
                FixtureName = fixture.Id,
                Scenario = fixture.Scenario,
                Source = "EECalc oracle: InputDataCalc.CalculateMonthlyDays + HeatingAndCoolingResultCalc.CalculateParameterQve",
                Months = monthlyDays.Select(month =>
                {
                    var balance = balanceByMonth[month.Month];
                    return new EecalcMonthlySnapshotRow
                    {
                        Month = month.MonthName,
                        WorkDays = month.WorkDays,
                        Saturdays = month.Saturdays,
                        Sundays = month.Sundays,
                        Holidays = month.Holidays,
                        TotalDays = month.TotalDays,
                        Weeks = month.Weeks,
                        Hve = balance.Hve,
                        Htr = balance.Htr,
                        Qtr = balance.Qtr,
                        Qve = balance.Qve,
                        Qgn = balance.Qgn,
                        Gamma = balance.Gamma,
                        Ni = balance.Ni,
                        Qnd = balance.NetEnergyRaw,
                        QndPerArea = balance.NetEnergyPerArea
                    };
                }).ToList()
            };
        }

        private static double CalculateProjectHours(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            return month.WorkDays * Duration(fixture.WorkdaySchedule)
                + month.Saturdays * Duration(fixture.SaturdaySchedule)
                + month.Sundays * Duration(fixture.SundaySchedule);
        }

        private static double CalculateNonProjectHours(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            return month.WorkDays * (24.0 - Duration(fixture.WorkdaySchedule))
                + month.Saturdays * (24.0 - Duration(fixture.SaturdaySchedule))
                + month.Sundays * (24.0 - Duration(fixture.SundaySchedule))
                + month.Holidays * 24.0;
        }

        private static int Duration(EecalcDailySchedule schedule)
        {
            return schedule.EndHour - schedule.StartHour;
        }
    }

    public sealed class EecalcHeatingMonthlyBalanceRow
    {
        public int Month { get; init; }

        public string MonthName { get; init; } = string.Empty;

        public double AverageOutdoorTemperature { get; init; }

        public double ProjectTemperature { get; init; }

        public double NonProjectTemperature { get; init; }

        public double ProjectHours { get; init; }

        public double NonProjectHours { get; init; }

        public double DeltaProject { get; init; }

        public double DeltaNonProject { get; init; }

        public double Hve { get; init; }

        public double Qve { get; init; }

        public double? Htr { get; init; }

        public double? Qtr { get; init; }

        public double? Qgn { get; init; }

        public double? Gamma { get; init; }

        public double? Ni { get; init; }

        public double Qht { get; init; }

        public double NetEnergyRaw { get; init; }

        public double NetEnergyPerArea { get; init; }
    }
}
