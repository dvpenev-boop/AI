using System;
using System.IO;
using System.Linq;
using Xunit;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcR1R2ValidationTests
    {
        [Fact]
        public void MonthlyDays_28OctTo5Apr_HolidaysZero_MatchesEecalc()
        {
            var fixture = EecalcValidationFixtures.CreateZone9MinimalHeating2260626();
            var monthlyDays = new EecalcMonthlyDaysOracle().Calculate(fixture);

            Assert.Collection(monthlyDays,
                row => AssertMonth(row, "October", 31, 4, 0, 0, 0, 4.0 / 7.0),
                row => AssertMonth(row, "November", 30, 22, 4, 4, 0, 30.0 / 7.0),
                row => AssertMonth(row, "December", 31, 21, 5, 5, 0, 31.0 / 7.0),
                row => AssertMonth(row, "January", 31, 22, 4, 5, 0, 31.0 / 7.0),
                row => AssertMonth(row, "February", 28, 20, 4, 4, 0, 28.0 / 7.0),
                row => AssertMonth(row, "March", 31, 23, 4, 4, 0, 31.0 / 7.0),
                row => AssertMonth(row, "April", 30, 5, 0, 0, 0, 5.0 / 7.0));
        }

        [Fact]
        public void Qve_Zone9Minimal_Hve_Is425()
        {
            var fixture = EecalcValidationFixtures.CreateZone9MinimalHeating2260626();
            var hve = new EecalcQveOracle().CalculateHve(fixture);

            Assert.Equal(425.0, hve, precision: 6);
        }

        [Fact]
        public void Qve_Zone9Minimal_QndPerArea_Is22_60626()
        {
            var fixture = EecalcValidationFixtures.CreateZone9MinimalHeating2260626();
            var monthlyDays = new EecalcMonthlyDaysOracle().Calculate(fixture);
            var qveOracle = new EecalcQveOracle();
            var balanceRows = qveOracle.Calculate(fixture, monthlyDays);

            var qndPerArea = balanceRows.Sum(row => row.NetEnergyRaw) / fixture.HeatedArea;

            Assert.Equal(22.60626, qndPerArea, precision: 5);
        }

        [Fact]
        public void DebugArtifacts_Zone9Minimal_AreGenerated()
        {
            var fixture = EecalcValidationFixtures.CreateZone9MinimalHeating2260626();
            var monthlyDaysOracle = new EecalcMonthlyDaysOracle();
            var qveOracle = new EecalcQveOracle();
            var monthlyDays = monthlyDaysOracle.Calculate(fixture);
            var balanceRows = qveOracle.Calculate(fixture, monthlyDays);
            var expected = qveOracle.CreateExpectedSnapshot(fixture, monthlyDays, balanceRows);
            var actual = new EeDokladHeatingActualAdapter().CalculateActual(fixture, monthlyDays);
            var comparison = new EecalcValidationReporter().Compare(expected, actual, tolerance: 0.000001);
            var artifactDirectory = GetArtifactDirectory(fixture.Id);
            var writer = new EecalcValidationDebugWriter();

            writer.WriteMonthlyDays(Path.Combine(artifactDirectory, "debug_monthly_days.csv"), fixture, monthlyDays);
            writer.WriteHeatingMonthlyBalance(Path.Combine(artifactDirectory, "debug_heating_monthly_balance.csv"), fixture, balanceRows);
            writer.WriteDiffReport(Path.Combine(artifactDirectory, "diff_report.md"), comparison);

            Assert.True(File.Exists(Path.Combine(artifactDirectory, "debug_monthly_days.csv")));
            Assert.True(File.Exists(Path.Combine(artifactDirectory, "debug_heating_monthly_balance.csv")));
            Assert.True(File.Exists(Path.Combine(artifactDirectory, "diff_report.md")));
        }

        private static void AssertMonth(
            EecalcMonthlyDaysOracleRow row,
            string month,
            int totalDays,
            int workDays,
            int saturdays,
            int sundays,
            int holidays,
            double weeks)
        {
            Assert.Equal(month, row.MonthName);
            Assert.Equal(totalDays, row.TotalDays);
            Assert.Equal(workDays, row.WorkDays);
            Assert.Equal(saturdays, row.Saturdays);
            Assert.Equal(sundays, row.Sundays);
            Assert.Equal(holidays, row.Holidays);
            Assert.Equal(weeks, row.Weeks, precision: 12);
        }

        private static string GetArtifactDirectory(string fixtureId)
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null && !File.Exists(Path.Combine(directory.FullName, "EE.Doklad.Tests.csproj")))
            {
                directory = directory.Parent;
            }

            var projectDirectory = directory?.FullName ?? AppContext.BaseDirectory;
            return Path.Combine(projectDirectory, "Validation", "Artifacts", fixtureId);
        }
    }
}
