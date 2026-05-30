using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Xunit;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcMonthlyHeatingOracleTests
    {
        [Fact]
        public void CompleteMonthlyHeatingOracle_InternalConsistency()
        {
            var fixture = CreateFixture();
            var oracle = new EecalcMonthlyHeatingOracle();

            var rows = oracle.Calculate(fixture);
            WriteDebugCsv(rows);

            Assert.Equal(new[] { 10, 11, 12, 1, 2, 3, 4 }, rows.Select(row => row.Month).ToArray());

            foreach (var row in rows)
            {
                Assert.Equal(row.Qtr + row.Qve, row.Qht, precision: 10);
                Assert.Equal((row.Qgn + row.MetabolicHeat) / row.Qht, row.Gamma, precision: 10);
                Assert.Equal(1.0 + row.Tau / 15.0, row.AH, precision: 10);
                Assert.Equal(row.Qht - row.Ni * row.Qgn, row.RawQnd, precision: 10);
                Assert.Equal(row.RawQnd / fixture.Calculation.HeatedArea - row.Ni * row.MetabolicHeatPerArea, row.FinalQnd, precision: 10);
                Assert.Equal(ExpectedNi(row.Gamma, row.AH, out var expectedBranch), row.Ni, precision: 10);
                Assert.Equal(expectedBranch, row.NiBranch);
            }

            Assert.True(File.Exists(GetDebugCsvPath()));
        }

        [Theory]
        [InlineData(0.5, 2.0, "positive_power")]
        [InlineData(-0.1, 2.0, "negative_gamma")]
        [InlineData(1.005, 2.0, "near_one")]
        [InlineData(0.0, 2.0, "fallback_zero")]
        [InlineData(0.99, 2.0, "fallback_zero")]
        [InlineData(1.01, 2.0, "fallback_zero")]
        public void CompleteMonthlyHeatingOracle_NiBranchCorrectness(double gamma, double aH, string expectedBranch)
        {
            var oracle = new EecalcMonthlyHeatingOracle();

            var ni = oracle.CalculateNi(gamma, aH, out var branch);

            Assert.Equal(expectedBranch, branch);
            Assert.Equal(ExpectedNi(gamma, aH, out _), ni, precision: 12);
        }

        private static EecalcEnvelopeFixture CreateFixture()
        {
            var calculation = new EecalcValidationFixture
            {
                Id = "complete_monthly_heating_oracle",
                Scenario = "Actual",
                ClimateZoneId = 7,
                FirstMonth = 10,
                LastMonth = 4,
                FirstDay = 15,
                LastDay = 23,
                HeatedArea = 1000.0,
                HeatedVolume = 2500.0,
                Infiltration = 0.5,
                HeatCapacity = 50.0,
                MetabolicHeat = 3.0,
                ProjectTemperature = 21.0,
                NonProjectTemperature = 17.0,
                WorkdaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 20 },
                SaturdaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 12 },
                SundaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 0 },
                HolidaysByMonth = new Dictionary<int, int>
                {
                    [10] = 0,
                    [11] = 0,
                    [12] = 0,
                    [1] = 0,
                    [2] = 0,
                    [3] = 0,
                    [4] = 0
                },
                AverageOutdoorTemperatureByMonth = new Dictionary<int, double>
                {
                    [10] = 11.2,
                    [11] = 5.1,
                    [12] = 0.4,
                    [1] = -0.4,
                    [2] = 0.2,
                    [3] = 4.6,
                    [4] = 10.4
                },
                SolarRadiationByMonth = new Dictionary<int, EecalcSolarRadiationFixture>
                {
                    [10] = new() { N = 20.0, E = 42.0, S = 70.0, W = 40.0, H = 55.0 },
                    [11] = new() { N = 12.0, E = 25.0, S = 45.0, W = 24.0, H = 32.0 },
                    [12] = new() { N = 8.0, E = 18.0, S = 35.0, W = 17.0, H = 24.0 },
                    [1] = new() { N = 10.0, E = 22.0, S = 42.0, W = 21.0, H = 30.0 },
                    [2] = new() { N = 16.0, E = 35.0, S = 60.0, W = 33.0, H = 45.0 },
                    [3] = new() { N = 26.0, E = 55.0, S = 85.0, W = 52.0, H = 70.0 },
                    [4] = new() { N = 36.0, E = 70.0, S = 95.0, W = 68.0, H = 85.0 }
                }
            };

            var fixture = new EecalcEnvelopeFixture
            {
                Id = calculation.Id,
                Calculation = calculation
            };

            fixture.NorthWalls.OuterA[0] = 120.0;
            fixture.NorthWalls.OuterU[0] = 0.35;
            fixture.NorthWalls.AccumulateOuterA = 120.0;
            fixture.NorthWalls.AccumulateOuterU = 0.35;
            fixture.NorthWalls.AccumulateOuterAlfa = 0.6;
            fixture.NorthWalls.AccumulateOuterE = 0.9;
            fixture.NorthWalls.AccumulateWindowA = 20.0;
            fixture.NorthWalls.AccumulateWindowU = 1.4;
            fixture.NorthWalls.AccumulateWindowG = 0.55;
            fixture.NorthWalls.AccumulateWindowE = 0.84;
            fixture.NorthWalls.InnerA[0] = 50.0;
            fixture.NorthWalls.InnerU[0] = 0.45;
            fixture.NorthWalls.InnerW[0] = 12.0;

            fixture.SouthWalls.OuterA[0] = 150.0;
            fixture.SouthWalls.OuterU[0] = 0.3;
            fixture.SouthWalls.AccumulateOuterA = 150.0;
            fixture.SouthWalls.AccumulateOuterU = 0.3;
            fixture.SouthWalls.AccumulateOuterAlfa = 0.6;
            fixture.SouthWalls.AccumulateOuterE = 0.9;
            fixture.SouthWalls.AccumulateWindowA = 35.0;
            fixture.SouthWalls.AccumulateWindowU = 1.3;
            fixture.SouthWalls.AccumulateWindowG = 0.58;
            fixture.SouthWalls.AccumulateWindowE = 0.84;

            fixture.Roof.NonTransparentA[0] = 1000.0;
            fixture.Roof.NonTransparentU[0] = 0.25;
            fixture.Roof.AccumulateNonTransparentA = 1000.0;
            fixture.Roof.AccumulateNonTransparentU = 0.25;
            fixture.Roof.AccumulateNonTransparentAlfa = 0.6;
            fixture.Roof.AccumulateNonTransparentE = 0.9;
            fixture.Roof.TransparentA[8] = 12.0;
            fixture.Roof.TransparentU[8] = 1.2;
            fixture.Roof.TransparentG[8] = 0.52;
            fixture.Roof.TransparentE[8] = 0.84;
            fixture.Roof.CeilingA[0] = 80.0;
            fixture.Roof.CeilingU[0] = 0.4;
            fixture.Roof.CeilingW[0] = 13.0;

            fixture.Floor.AccumulateFloorA = 1000.0;
            fixture.Floor.AccumulateFloorU = 0.4;
            fixture.Floor.OtherFloorA[0] = 50.0;
            fixture.Floor.OtherFloorU[0] = 0.45;
            fixture.Floor.OtherFloorW[0] = 12.0;

            return fixture;
        }

        private static double ExpectedNi(double gamma, double aH, out string branch)
        {
            if (gamma > 0.0 && Math.Abs(gamma - 1.0) > 0.01)
            {
                branch = "positive_power";
                return (1.0 - Math.Pow(gamma, aH)) / (1.0 - Math.Pow(gamma, aH + 1.0));
            }

            if (gamma < 0.0)
            {
                branch = "negative_gamma";
                return 1.0;
            }

            if (Math.Abs(gamma - 1.0) < 0.01)
            {
                branch = "near_one";
                return aH / (aH + 1.0);
            }

            branch = "fallback_zero";
            return 0.0;
        }

        private static void WriteDebugCsv(IReadOnlyList<EecalcMonthlyHeatingOracleRow> rows)
        {
            var path = GetDebugCsvPath();
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            var lines = new List<string>
            {
                "Month,Qtr,Qve,Qht,Qgn,MetabolicHeat,Gamma,aH,Ni,RawQnd,FinalQnd"
            };
            lines.AddRange(rows.Select(row => string.Join(",",
                row.MonthName,
                Format(row.Qtr),
                Format(row.Qve),
                Format(row.Qht),
                Format(row.Qgn),
                Format(row.MetabolicHeat),
                Format(row.Gamma),
                Format(row.AH),
                Format(row.Ni),
                Format(row.RawQnd),
                Format(row.FinalQnd))));

            File.WriteAllLines(path, lines);
        }

        private static string GetDebugCsvPath()
        {
            return Path.Combine(FindRepositoryRoot(), "test-results", "validation", "debug_heating_full_pipeline.csv");
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

        private static string Format(double value)
        {
            return value.ToString("G17", CultureInfo.InvariantCulture);
        }
    }
}
