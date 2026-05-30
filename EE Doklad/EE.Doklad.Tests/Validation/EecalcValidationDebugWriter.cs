using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcValidationDebugWriter
    {
        public void WriteMonthlyDays(
            string path,
            EecalcValidationFixture fixture,
            IReadOnlyList<EecalcMonthlyDaysOracleRow> rows)
        {
            ArgumentNullException.ThrowIfNull(path);
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(rows);

            EnsureDirectory(path);
            var lines = new List<string>
            {
                "fixture,scenario,month,total_days,work_days,saturdays,sundays,holidays,weeks"
            };

            lines.AddRange(rows.Select(row => string.Join(",",
                fixture.Id,
                fixture.Scenario,
                row.MonthName,
                row.TotalDays.ToString(CultureInfo.InvariantCulture),
                row.WorkDays.ToString(CultureInfo.InvariantCulture),
                row.Saturdays.ToString(CultureInfo.InvariantCulture),
                row.Sundays.ToString(CultureInfo.InvariantCulture),
                row.Holidays.ToString(CultureInfo.InvariantCulture),
                row.Weeks.ToString("G17", CultureInfo.InvariantCulture))));

            File.WriteAllLines(path, lines);
        }

        public void WriteHeatingMonthlyBalance(
            string path,
            EecalcValidationFixture fixture,
            IReadOnlyList<EecalcHeatingMonthlyBalanceRow> rows)
        {
            ArgumentNullException.ThrowIfNull(path);
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(rows);

            EnsureDirectory(path);
            var lines = new List<string>
            {
                "fixture,scenario,month,avg_outdoor_temp,project_temp,non_project_temp,project_hours,non_project_hours,delta_project,delta_non_project,hve,qve,hd,hg,hu,htr,qtr,fsol_transparent,fsol_nontransparent,qgn_raw,qgn,latent_heat_per_month,gamma,ah,ni,qht,net_energy_raw,net_energy_per_area"
            };

            lines.AddRange(rows.Select(row => string.Join(",",
                fixture.Id,
                fixture.Scenario,
                row.MonthName,
                Format(row.AverageOutdoorTemperature),
                Format(row.ProjectTemperature),
                Format(row.NonProjectTemperature),
                Format(row.ProjectHours),
                Format(row.NonProjectHours),
                Format(row.DeltaProject),
                Format(row.DeltaNonProject),
                Format(row.Hve),
                Format(row.Qve),
                "0",
                "0",
                "0",
                Format(row.Htr),
                Format(row.Qtr),
                "",
                "",
                "",
                Format(row.Qgn),
                "",
                Format(row.Gamma),
                "",
                Format(row.Ni),
                Format(row.Qht),
                Format(row.NetEnergyRaw),
                Format(row.NetEnergyPerArea))));

            File.WriteAllLines(path, lines);
        }

        public void WriteDiffReport(string path, EecalcComparisonResult comparison)
        {
            ArgumentNullException.ThrowIfNull(path);
            ArgumentNullException.ThrowIfNull(comparison);

            EnsureDirectory(path);
            var lines = new List<string>
            {
                "# EECalc vs EE.Doklad diff report",
                string.Empty,
                $"fixture={comparison.FixtureName}",
                $"scenario={comparison.Scenario}",
                $"passed={comparison.Passed.ToString(CultureInfo.InvariantCulture).ToLowerInvariant()}",
                string.Empty
            };

            if (comparison.Passed)
            {
                lines.Add("No mismatches.");
            }
            else
            {
                lines.Add("month,metric,expected,actual,abs_diff,tolerance");
                lines.AddRange(comparison.Mismatches.Select(mismatch => string.Join(",",
                    mismatch.Month,
                    mismatch.Metric,
                    Format(mismatch.Expected),
                    Format(mismatch.Actual),
                    Format(mismatch.AbsoluteDifference),
                    Format(mismatch.Tolerance))));
            }

            File.WriteAllLines(path, lines);
        }

        private static void EnsureDirectory(string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static string Format(double? value)
        {
            return value.HasValue ? Format(value.Value) : string.Empty;
        }

        private static string Format(double value)
        {
            return value.ToString("G17", CultureInfo.InvariantCulture);
        }
    }
}
