using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using EE.Doklad.Sections.Section11Heating.Models;
using Xunit;

namespace EE.Doklad.Tests.Validation
{
    public sealed class Zone7ForensicValidationTests
    {
        private const string FixtureId = "zone7_minimal_heating_from_climate_json";
        private const int ZoneId = 7;

        [Fact]
        public void Zone7_FromClimateJson_MonthlyDays_Report()
        {
            var context = GenerateReports();

            Assert.Equal(7, context.Zone.Id);
            Assert.Equal("10-15", context.Zone.HeatingSeasonStart);
            Assert.Equal("04-23", context.Zone.HeatingSeasonEnd);
            Assert.True(File.Exists(Path.Combine(context.RepositoryRoot, "test-results", "validation", "debug_zone7_monthly_days.csv")));
        }

        [Fact]
        public void Zone7_FromClimateJson_Qve_Report()
        {
            var context = GenerateReports();

            Assert.Equal(425.0, context.HveOracle, precision: 6);
            Assert.True(File.Exists(Path.Combine(context.RepositoryRoot, "test-results", "validation", "debug_zone7_monthly_balance.csv")));
        }

        [Fact]
        public void Zone7_FromClimateJson_CompareAgainstEeDoklad_Report()
        {
            var context = GenerateReports();

            Assert.Equal(425.0, context.HveOracle, precision: 6);
            Assert.True(File.Exists(Path.Combine(context.RepositoryRoot, "test-results", "validation", "debug_zone7_climate.csv")));
            Assert.True(File.Exists(Path.Combine(context.RepositoryRoot, "analysis", "zone7_forensic_report.md")));
        }

        private static Zone7ForensicContext GenerateReports()
        {
            var root = FindRepositoryRoot();
            var zone = LoadZone7(Path.Combine(root, "EE.Doklad", "Data", "climate_zones.json"));
            var fixture = CreateFixture(zone);
            var monthlyDays = new EecalcMonthlyDaysOracle().Calculate(fixture);
            var qveOracle = new EecalcQveOracle();
            var oracleBalance = qveOracle.Calculate(fixture, monthlyDays);
            var actual = new EeDokladHeatingActualAdapter().CalculateActual(fixture, monthlyDays);
            var rows = BuildRows(fixture, zone, monthlyDays, oracleBalance, actual);

            WriteFixture(root, fixture, zone);
            WriteClimateCsv(Path.Combine(root, "test-results", "validation", "debug_zone7_climate.csv"), rows);
            WriteMonthlyDaysCsv(Path.Combine(root, "test-results", "validation", "debug_zone7_monthly_days.csv"), rows);
            WriteMonthlyBalanceCsv(Path.Combine(root, "test-results", "validation", "debug_zone7_monthly_balance.csv"), rows);
            WriteMarkdownReport(Path.Combine(root, "analysis", "zone7_forensic_report.md"), fixture, zone, rows);

            return new Zone7ForensicContext(root, zone, qveOracle.CalculateHve(fixture), rows);
        }

        private static EecalcValidationFixture CreateFixture(Zone7ClimateJsonZone zone)
        {
            var (firstMonth, firstDay) = ParseMonthDay(zone.HeatingSeasonStart);
            var (lastMonth, lastDay) = ParseMonthDay(zone.HeatingSeasonEnd);

            return new EecalcValidationFixture
            {
                Id = FixtureId,
                Scenario = "Actual",
                ClimateZoneId = ZoneId,
                FirstMonth = firstMonth,
                FirstDay = firstDay,
                LastMonth = lastMonth,
                LastDay = lastDay,
                HeatedArea = 1000.0,
                HeatedVolume = 2500.0,
                Infiltration = 0.5,
                ProjectTemperature = 21.0,
                NonProjectTemperature = 17.0,
                WorkdaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 20 },
                SaturdaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 12 },
                SundaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 0 },
                HolidaysByMonth = Enumerable.Range(1, 12).ToDictionary(month => month, _ => 0),
                AverageOutdoorTemperatureByMonth = zone.AverageMonthlyTemperature
                    .Select((temperature, index) => new { Month = index + 1, Temperature = temperature })
                    .ToDictionary(item => item.Month, item => item.Temperature)
            };
        }

        private static IReadOnlyList<Zone7ForensicRow> BuildRows(
            EecalcValidationFixture fixture,
            Zone7ClimateJsonZone zone,
            IReadOnlyList<EecalcMonthlyDaysOracleRow> monthlyDays,
            IReadOnlyList<EecalcHeatingMonthlyBalanceRow> oracleBalance,
            EecalcActualSnapshot actual)
        {
            var actualByMonth = actual.Months.ToDictionary(row => row.Month, StringComparer.OrdinalIgnoreCase);
            var balanceByMonth = oracleBalance.ToDictionary(row => row.MonthName, StringComparer.OrdinalIgnoreCase);

            return monthlyDays.Select(days =>
            {
                var balance = balanceByMonth[days.MonthName];
                var actualMonth = actualByMonth[days.MonthName];
                var teClimateJson = zone.AverageMonthlyTemperature[days.Month - 1];
                var teProduction = ClimateDatabase.GetTe(ZoneId, days.Month - 1);
                var projectHours = ProjectHours(fixture, days);
                var nonProjectHours = NonProjectHours(fixture, days);

                return new Zone7ForensicRow
                {
                    Month = days.MonthName,
                    TeClimateJson = teClimateJson,
                    TeEeDokladProduction = teProduction,
                    DeltaTe = teProduction - teClimateJson,
                    WorkDaysOracle = days.WorkDays,
                    WorkDaysEeDoklad = actualMonth.WorkDays,
                    SaturdaysOracle = days.Saturdays,
                    SaturdaysEeDoklad = actualMonth.Saturdays,
                    SundaysOracle = days.Sundays,
                    SundaysEeDoklad = actualMonth.Sundays,
                    Holidays = days.Holidays,
                    ProjectHoursOracle = projectHours,
                    ProjectHoursEeDoklad = projectHours,
                    NonProjectHoursOracle = nonProjectHours,
                    NonProjectHoursEeDoklad = nonProjectHours,
                    HveOracle = balance.Hve,
                    HveEeDoklad = actualMonth.Hve,
                    QveOracle = balance.Qve,
                    QveEeDoklad = actualMonth.Qve,
                    DeltaQve = actualMonth.Qve - balance.Qve,
                    QndPerAreaOracle = balance.NetEnergyPerArea,
                    QndPerAreaEeDoklad = actualMonth.QndPerArea,
                    DeltaQndPerArea = actualMonth.QndPerArea - balance.NetEnergyPerArea
                };
            }).ToList();
        }

        private static Zone7ClimateJsonZone LoadZone7(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("climate_zones.json cannot be loaded.", path);
            }

            using var document = JsonDocument.Parse(File.ReadAllText(path));
            if (!document.RootElement.TryGetProperty("Zones", out var zones))
            {
                throw new InvalidOperationException("climate_zones.json does not contain Zones.");
            }

            foreach (var zone in zones.EnumerateArray())
            {
                if (!zone.TryGetProperty("Id", out var id) || id.GetInt32() != ZoneId)
                {
                    continue;
                }

                var heatingSeason = zone.GetProperty("HeatingSeason");
                var start = heatingSeason.GetProperty("Start").GetString() ?? throw new InvalidOperationException("Zone 7 HeatingSeason.Start is missing.");
                var end = heatingSeason.GetProperty("End").GetString() ?? throw new InvalidOperationException("Zone 7 HeatingSeason.End is missing.");
                _ = ParseMonthDay(start);
                _ = ParseMonthDay(end);

                var temperatures = zone.GetProperty("Monthly").GetProperty("AvgMonthlyTempC")
                    .EnumerateArray()
                    .Select(item => item.GetDouble())
                    .ToArray();

                if (temperatures.Length != 12)
                {
                    throw new InvalidOperationException("Zone 7 AvgMonthlyTempC must contain 12 values.");
                }

                return new Zone7ClimateJsonZone(
                    id.GetInt32(),
                    zone.GetProperty("Name").GetString() ?? string.Empty,
                    start,
                    end,
                    temperatures);
            }

            throw new InvalidOperationException("Zone 7 cannot be found in climate_zones.json.");
        }

        private static (int Month, int Day) ParseMonthDay(string value)
        {
            var parts = value.Split('-', StringSplitOptions.TrimEntries);
            if (parts.Length != 2
                || !int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var month)
                || !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var day)
                || month < 1
                || month > 12
                || day < 1
                || day > 31)
            {
                throw new InvalidOperationException($"Heating season value cannot be parsed: {value}");
            }

            return (month, day);
        }

        private static double ProjectHours(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            return month.WorkDays * Duration(fixture.WorkdaySchedule)
                + month.Saturdays * Duration(fixture.SaturdaySchedule)
                + month.Sundays * Duration(fixture.SundaySchedule);
        }

        private static double NonProjectHours(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
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

        private static void WriteFixture(string root, EecalcValidationFixture fixture, Zone7ClimateJsonZone zone)
        {
            var path = Path.Combine(root, "tests", "validation", "fixtures", "zone7_minimal_heating_from_climate_json.json");
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            var json = $$"""
{
  "id": "{{fixture.Id}}",
  "scenario": "{{fixture.Scenario}}",
  "climateSource": "EE.Doklad/Data/climate_zones.json",
  "zoneId": 7,
  "zoneName": "{{zone.Name}}",
  "heatingSeason": {
    "start": "{{zone.HeatingSeasonStart}}",
    "end": "{{zone.HeatingSeasonEnd}}"
  },
  "heatedArea": 1000.0,
  "heatedVolume": 2500.0,
  "infiltration": 0.5,
  "expectedHve": 425.0,
  "projectTemperature": 21.0,
  "nonProjectTemperature": 17.0,
  "schedule": {
    "workday": { "startHour": 0, "endHour": 20 },
    "saturday": { "startHour": 0, "endHour": 12 },
    "sunday": { "startHour": 0, "endHour": 0 }
  },
  "holidays": 0
}
""";
            File.WriteAllText(path, json);
        }

        private static void WriteClimateCsv(string path, IReadOnlyList<Zone7ForensicRow> rows)
        {
            WriteLines(path, new[] { "month,te_climate_json,te_eedoklad_production,delta_te" }
                .Concat(rows.Select(row => string.Join(",",
                    row.Month,
                    Format(row.TeClimateJson),
                    Format(row.TeEeDokladProduction),
                    Format(row.DeltaTe)))));
        }

        private static void WriteMonthlyDaysCsv(string path, IReadOnlyList<Zone7ForensicRow> rows)
        {
            WriteLines(path, new[]
            {
                "month,workdays_oracle,workdays_eedoklad,saturdays_oracle,saturdays_eedoklad,sundays_oracle,sundays_eedoklad,holidays,project_hours_oracle,project_hours_eedoklad,nonproject_hours_oracle,nonproject_hours_eedoklad"
            }.Concat(rows.Select(row => string.Join(",",
                row.Month,
                row.WorkDaysOracle,
                row.WorkDaysEeDoklad,
                row.SaturdaysOracle,
                row.SaturdaysEeDoklad,
                row.SundaysOracle,
                row.SundaysEeDoklad,
                row.Holidays,
                Format(row.ProjectHoursOracle),
                Format(row.ProjectHoursEeDoklad),
                Format(row.NonProjectHoursOracle),
                Format(row.NonProjectHoursEeDoklad)))));
        }

        private static void WriteMonthlyBalanceCsv(string path, IReadOnlyList<Zone7ForensicRow> rows)
        {
            WriteLines(path, new[]
            {
                "month,te_climate_json,te_eedoklad_production,delta_te,workdays_oracle,workdays_eedoklad,saturdays_oracle,saturdays_eedoklad,sundays_oracle,sundays_eedoklad,holidays,project_hours_oracle,project_hours_eedoklad,nonproject_hours_oracle,nonproject_hours_eedoklad,hve_oracle,hve_eedoklad,qve_oracle,qve_eedoklad,delta_qve,qnd_per_area_oracle,qnd_per_area_eedoklad,delta_qnd_per_area"
            }.Concat(rows.Select(row => string.Join(",",
                row.Month,
                Format(row.TeClimateJson),
                Format(row.TeEeDokladProduction),
                Format(row.DeltaTe),
                row.WorkDaysOracle,
                row.WorkDaysEeDoklad,
                row.SaturdaysOracle,
                row.SaturdaysEeDoklad,
                row.SundaysOracle,
                row.SundaysEeDoklad,
                row.Holidays,
                Format(row.ProjectHoursOracle),
                Format(row.ProjectHoursEeDoklad),
                Format(row.NonProjectHoursOracle),
                Format(row.NonProjectHoursEeDoklad),
                Format(row.HveOracle),
                Format(row.HveEeDoklad),
                Format(row.QveOracle),
                Format(row.QveEeDoklad),
                Format(row.DeltaQve),
                Format(row.QndPerAreaOracle),
                Format(row.QndPerAreaEeDoklad),
                Format(row.DeltaQndPerArea)))));
        }

        private static void WriteMarkdownReport(
            string path,
            EecalcValidationFixture fixture,
            Zone7ClimateJsonZone zone,
            IReadOnlyList<Zone7ForensicRow> rows)
        {
            var totalOracle = rows.Sum(row => row.QveOracle) / fixture.HeatedArea;
            var totalEeDoklad = rows.Sum(row => row.QveEeDoklad ?? 0.0) / fixture.HeatedArea;
            var suspectedCause = rows.Any(row => Math.Abs(row.DeltaTe) > 0.000001)
                ? "climate data"
                : rows.Any(row => row.WorkDaysOracle != row.WorkDaysEeDoklad || row.SaturdaysOracle != row.SaturdaysEeDoklad || row.SundaysOracle != row.SundaysEeDoklad)
                    ? "MonthlyDays"
                    : rows.Any(row => Math.Abs(row.ProjectHoursOracle - row.ProjectHoursEeDoklad) > 0.000001 || Math.Abs(row.NonProjectHoursOracle - row.NonProjectHoursEeDoklad) > 0.000001)
                        ? "hours/schedule"
                        : rows.Any(row => Math.Abs(row.HveOracle - (row.HveEeDoklad ?? 0.0)) > 0.000001)
                            ? "Hve inputs"
                            : rows.Any(row => Math.Abs(row.DeltaQve ?? 0.0) > 0.000001)
                                ? "Qve formula"
                                : "none detected";

            var builder = new StringBuilder();
            builder.AppendLine("# Zone 7 forensic validation");
            builder.AppendLine();
            builder.AppendLine($"Fixture: `{fixture.Id}`");
            builder.AppendLine($"Climate source: `EE.Doklad/Data/climate_zones.json`");
            builder.AppendLine($"Zone: `{zone.Id}` `{zone.Name}`");
            builder.AppendLine($"Heating season: `{zone.HeatingSeasonStart}` -> `{zone.HeatingSeasonEnd}`");
            builder.AppendLine($"Oracle total QndPerArea: `{Format(totalOracle)}` kWh/m2");
            builder.AppendLine($"EE.Doklad total QndPerArea: `{Format(totalEeDoklad)}` kWh/m2");
            builder.AppendLine($"Top suspected cause: `{suspectedCause}`");
            builder.AppendLine();
            builder.AppendLine("| Month | Te_ClimateJson | Te_EeDokladProduction | DeltaTe | WorkDays_Oracle | WorkDays_EeDoklad | Saturdays_Oracle | Saturdays_EeDoklad | Sundays_Oracle | Sundays_EeDoklad | Holidays | ProjectHours_Oracle | ProjectHours_EeDoklad | NonProjectHours_Oracle | NonProjectHours_EeDoklad | Hve_Oracle | Hve_EeDoklad | Qve_Oracle | Qve_EeDoklad | DeltaQve | QndPerArea_Oracle | QndPerArea_EeDoklad | DeltaQndPerArea |");
            builder.AppendLine("|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|");
            foreach (var row in rows)
            {
                builder.AppendLine($"| {row.Month} | {Format(row.TeClimateJson)} | {Format(row.TeEeDokladProduction)} | {Format(row.DeltaTe)} | {row.WorkDaysOracle} | {row.WorkDaysEeDoklad} | {row.SaturdaysOracle} | {row.SaturdaysEeDoklad} | {row.SundaysOracle} | {row.SundaysEeDoklad} | {row.Holidays} | {Format(row.ProjectHoursOracle)} | {Format(row.ProjectHoursEeDoklad)} | {Format(row.NonProjectHoursOracle)} | {Format(row.NonProjectHoursEeDoklad)} | {Format(row.HveOracle)} | {Format(row.HveEeDoklad)} | {Format(row.QveOracle)} | {Format(row.QveEeDoklad)} | {Format(row.DeltaQve)} | {Format(row.QndPerAreaOracle)} | {Format(row.QndPerAreaEeDoklad)} | {Format(row.DeltaQndPerArea)} |");
            }

            WriteLines(path, builder.ToString().Split(Environment.NewLine));
        }

        private static void WriteLines(string path, IEnumerable<string> lines)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllLines(path, lines);
        }

        private static string FindRepositoryRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null && !File.Exists(Path.Combine(directory.FullName, "EE Doklad.sln")))
            {
                directory = directory.Parent;
            }

            return directory?.FullName ?? Directory.GetCurrentDirectory();
        }

        private static string Format(double? value)
        {
            return value.HasValue ? Format(value.Value) : string.Empty;
        }

        private static string Format(double value)
        {
            return value.ToString("G17", CultureInfo.InvariantCulture);
        }

        private sealed record Zone7ForensicContext(
            string RepositoryRoot,
            Zone7ClimateJsonZone Zone,
            double HveOracle,
            IReadOnlyList<Zone7ForensicRow> Rows);

        private sealed record Zone7ClimateJsonZone(
            int Id,
            string Name,
            string HeatingSeasonStart,
            string HeatingSeasonEnd,
            double[] AverageMonthlyTemperature);

        private sealed class Zone7ForensicRow
        {
            public string Month { get; init; } = string.Empty;
            public double TeClimateJson { get; init; }
            public double TeEeDokladProduction { get; init; }
            public double DeltaTe { get; init; }
            public int WorkDaysOracle { get; init; }
            public int? WorkDaysEeDoklad { get; init; }
            public int SaturdaysOracle { get; init; }
            public int? SaturdaysEeDoklad { get; init; }
            public int SundaysOracle { get; init; }
            public int? SundaysEeDoklad { get; init; }
            public int Holidays { get; init; }
            public double ProjectHoursOracle { get; init; }
            public double ProjectHoursEeDoklad { get; init; }
            public double NonProjectHoursOracle { get; init; }
            public double NonProjectHoursEeDoklad { get; init; }
            public double HveOracle { get; init; }
            public double? HveEeDoklad { get; init; }
            public double QveOracle { get; init; }
            public double? QveEeDoklad { get; init; }
            public double? DeltaQve { get; init; }
            public double QndPerAreaOracle { get; init; }
            public double? QndPerAreaEeDoklad { get; init; }
            public double? DeltaQndPerArea { get; init; }
        }
    }
}
