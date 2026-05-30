using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Xunit;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcMonthlyCoolingOracleTests
    {
        [Fact]
        public void CompleteMonthlyCoolingOracle_InternalConsistency()
        {
            var fixture = CreateFixture();
            var oracle = new EecalcMonthlyCoolingOracle();

            var result = oracle.Calculate(fixture, ventilationInputs: 1.25);
            WriteDebugCsv(result.Rows);

            Assert.Equal(new[] { 6, 7, 8, 9 }, result.Rows.Select(row => row.Month).ToArray());

            foreach (var row in result.Rows)
            {
                Assert.Equal(row.Qsol + row.Qint + row.Qoccupants, row.Qgain, precision: 10);
                Assert.Equal(row.QtrCooling + row.Qinf, row.Qloss, precision: 10);
                Assert.Equal(ExpectedEta(row.Gamma, row.Ac, out var expectedBranch), row.Eta, precision: 10);
                Assert.Equal(expectedBranch, row.EtaBranch);
                Assert.Equal(
                    row.Qgain - row.Eta * row.Qloss + row.QLatentOccupants + row.QLatentInf + row.QLatentVent,
                    row.QcoolRaw,
                    precision: 10);
                Assert.Equal(row.QcoolRaw + row.QfreeCooling + row.QveCooling, row.QcoolWithInputs, precision: 10);
            }

            Assert.Equal(result.Rows.Sum(row => row.QcoolRaw) / fixture.Calculation.HeatedArea, result.ResultNoInputsNetEnergy, precision: 10);
            Assert.Equal(result.Rows.Sum(row => row.QfreeCooling), result.ResultCoolingInputs, precision: 10);
            Assert.Equal(result.ResultNoInputsNetEnergy - result.ResultCoolingInputs - result.ResultVentilationInputs, result.ResultNetEnergy, precision: 10);
            Assert.True(File.Exists(GetDebugCsvPath()));
        }

        [Theory]
        [InlineData(0.5, 2.0, "positive_negative_power")]
        [InlineData(1.005, 2.0, "near_one")]
        [InlineData(-0.1, 2.0, "negative_gamma")]
        [InlineData(0.0, 2.0, "fallback_zero")]
        [InlineData(0.99, 2.0, "fallback_zero")]
        [InlineData(1.01, 2.0, "fallback_zero")]
        public void CompleteMonthlyCoolingOracle_EtaBranchCorrectness(double gamma, double ac, string expectedBranch)
        {
            var oracle = new EecalcMonthlyCoolingOracle();

            var eta = oracle.Eta(ac, loses: 1.0, gainings: gamma, out var actualGamma, out var branch);

            Assert.Equal(gamma, actualGamma, precision: 12);
            Assert.Equal(expectedBranch, branch);
            Assert.Equal(ExpectedEta(gamma, ac, out _), eta, precision: 12);
        }

        [Fact]
        public void CompleteMonthlyCoolingOracle_PreservesTransmissionQuirks()
        {
            var fixture = CreateFixture();
            var month = new EecalcMonthlyDaysOracle().Calculate(fixture.Calculation).First();
            var oracle = new EecalcMonthlyCoolingOracle();

            var row = oracle.CalculateMonth(fixture, month);
            var avgOutdoor = fixture.Calculation.AverageOutdoorTemperatureByMonth[month.Month];
            var projectHours = month.WorkDays * 10.0 + month.Saturdays * 8.0 + month.Sundays * 6.0;
            var nonProjectHours = month.WorkDays * 14.0 + month.Saturdays * 16.0 + month.Sundays * 18.0 + month.Holidays * 24.0;
            var avgInner = (projectHours * 26.0 + nonProjectHours * 30.0) / (projectHours + nonProjectHours);
            var denominator = avgInner - avgOutdoor;
            var expectedWallLayer5 = 8.0 * fixture.NorthWalls.InnerA[4] * fixture.NorthWalls.InnerA[4]
                * (avgInner - fixture.NorthWalls.InnerCoolingS[4]) / denominator;
            var expectedCeilingLayer5 = fixture.Roof.CeilingA[4] * fixture.Roof.CeilingA[4]
                * (avgInner - fixture.Roof.CeilingCoolingS[4]) / denominator;
            var expectedFloorLayer6 = fixture.Floor.OtherFloorA[5] * fixture.Floor.OtherFloorU[5]
                * (avgInner - fixture.Floor.OtherFloorCoolingS[3]) / denominator;

            Assert.True(row.HuWalls >= expectedWallLayer5);
            Assert.True(row.HuCeilings >= expectedCeilingLayer5);
            Assert.True(row.HuFloors >= expectedFloorLayer6);
        }

        private static EecalcEnvelopeFixture CreateFixture()
        {
            var calculation = new EecalcValidationFixture
            {
                Id = "complete_monthly_cooling_oracle",
                Scenario = "Actual",
                ClimateZoneId = 7,
                FirstMonth = 6,
                LastMonth = 9,
                FirstDay = 3,
                LastDay = 21,
                HeatedArea = 1000.0,
                HeatedVolume = 2600.0,
                Infiltration = 0.45,
                HeatCapacity = 55.0,
                MetabolicHeat = 3.2,
                LatentMetabolicHeat = 1.4,
                ProjectTemperature = 26.0,
                NonProjectTemperature = 30.0,
                ProjectHumidity = 50.0,
                FlowTemperature = 17.0,
                FlowRelativeHumidity = 65.0,
                VentilationDebit = 900.0,
                LightsCoolingPower = 8.0,
                BalancedDevicesCoolingPower = 11.0,
                LightsCoolingWorkSchedule = 52.0,
                BalancedDevicesCoolingWorkSchedule = 48.0,
                WorkdaySchedule = new EecalcDailySchedule { StartHour = 8, EndHour = 18 },
                SaturdaySchedule = new EecalcDailySchedule { StartHour = 9, EndHour = 17 },
                SundaySchedule = new EecalcDailySchedule { StartHour = 10, EndHour = 16 },
                OccupantsWorkdaySchedule = new EecalcDailySchedule { StartHour = 8, EndHour = 18 },
                OccupantsSaturdaySchedule = new EecalcDailySchedule { StartHour = 9, EndHour = 15 },
                OccupantsSundaySchedule = new EecalcDailySchedule { StartHour = 10, EndHour = 14 },
                VentilationWorkdaySchedule = new EecalcDailySchedule { StartHour = 7, EndHour = 19 },
                VentilationSaturdaySchedule = new EecalcDailySchedule { StartHour = 8, EndHour = 16 },
                VentilationSundaySchedule = new EecalcDailySchedule { StartHour = 9, EndHour = 15 },
                NightVentilationWorkdaySchedule = new EecalcDailySchedule { StartHour = 22, EndHour = 5 },
                NightVentilationSaturdaySchedule = new EecalcDailySchedule { StartHour = 23, EndHour = 6 },
                NightVentilationSundaySchedule = new EecalcDailySchedule { StartHour = 21, EndHour = 4 },
                HolidaysByMonth = new Dictionary<int, int>
                {
                    [6] = 1,
                    [7] = 0,
                    [8] = 1,
                    [9] = 0
                },
                AverageOutdoorTemperatureByMonth = new Dictionary<int, double>
                {
                    [6] = 22.5,
                    [7] = 25.2,
                    [8] = 24.6,
                    [9] = 21.4
                },
                SolarRadiationByMonth = new Dictionary<int, EecalcSolarRadiationFixture>
                {
                    [6] = new() { N = 65.0, E = 130.0, S = 165.0, W = 125.0, H = 190.0 },
                    [7] = new() { N = 70.0, E = 140.0, S = 175.0, W = 135.0, H = 205.0 },
                    [8] = new() { N = 62.0, E = 125.0, S = 160.0, W = 128.0, H = 188.0 },
                    [9] = new() { N = 48.0, E = 105.0, S = 142.0, W = 100.0, H = 155.0 }
                },
                HourlyWeatherByMonth = new Dictionary<int, IReadOnlyList<EecalcHourlyWeatherFixture>>
                {
                    [6] = BuildWeather(21.0, 58.0),
                    [7] = BuildWeather(24.0, 62.0),
                    [8] = BuildWeather(23.0, 60.0),
                    [9] = BuildWeather(20.0, 57.0)
                }
            };

            var fixture = new EecalcEnvelopeFixture
            {
                Id = calculation.Id,
                Calculation = calculation
            };

            fixture.NorthWalls.OuterA[0] = 120.0;
            fixture.NorthWalls.OuterU[0] = 0.34;
            fixture.NorthWalls.AccumulateOuterA = 120.0;
            fixture.NorthWalls.AccumulateOuterU = 0.34;
            fixture.NorthWalls.AccumulateOuterAlfa = 0.62;
            fixture.NorthWalls.AccumulateOuterE = 0.9;
            fixture.NorthWalls.AccumulateWindowA = 24.0;
            fixture.NorthWalls.AccumulateWindowU = 1.35;
            fixture.NorthWalls.AccumulateWindowG = 0.54;
            fixture.NorthWalls.AccumulateWindowE = 0.84;
            fixture.NorthWalls.InnerA[0] = 42.0;
            fixture.NorthWalls.InnerU[0] = 0.45;
            fixture.NorthWalls.InnerCoolingS[0] = 28.0;
            fixture.NorthWalls.InnerA[4] = 3.0;
            fixture.NorthWalls.InnerU[4] = 99.0;
            fixture.NorthWalls.InnerCoolingS[4] = 27.0;

            fixture.SouthWalls.OuterA[0] = 155.0;
            fixture.SouthWalls.OuterU[0] = 0.29;
            fixture.SouthWalls.AccumulateOuterA = 155.0;
            fixture.SouthWalls.AccumulateOuterU = 0.29;
            fixture.SouthWalls.AccumulateOuterAlfa = 0.6;
            fixture.SouthWalls.AccumulateOuterE = 0.9;
            fixture.SouthWalls.AccumulateWindowA = 38.0;
            fixture.SouthWalls.AccumulateWindowU = 1.25;
            fixture.SouthWalls.AccumulateWindowG = 0.57;
            fixture.SouthWalls.AccumulateWindowE = 0.84;

            fixture.Roof.NonTransparentA[0] = 1000.0;
            fixture.Roof.NonTransparentU[0] = 0.23;
            fixture.Roof.AccumulateNonTransparentA = 1000.0;
            fixture.Roof.AccumulateNonTransparentU = 0.23;
            fixture.Roof.AccumulateNonTransparentAlfa = 0.58;
            fixture.Roof.AccumulateNonTransparentE = 0.9;
            fixture.Roof.TransparentA[8] = 16.0;
            fixture.Roof.TransparentU[8] = 1.18;
            fixture.Roof.TransparentG[8] = 0.5;
            fixture.Roof.TransparentE[8] = 0.84;
            fixture.Roof.CeilingA[0] = 75.0;
            fixture.Roof.CeilingU[0] = 0.38;
            fixture.Roof.CeilingCoolingS[0] = 29.0;
            fixture.Roof.CeilingA[4] = 4.0;
            fixture.Roof.CeilingU[4] = 88.0;
            fixture.Roof.CeilingCoolingS[4] = 29.0;

            fixture.Floor.AccumulateFloorA = 1000.0;
            fixture.Floor.AccumulateFloorU = 0.37;
            fixture.Floor.OtherFloorA[0] = 60.0;
            fixture.Floor.OtherFloorU[0] = 0.41;
            fixture.Floor.OtherFloorCoolingS[0] = 28.0;
            fixture.Floor.OtherFloorA[3] = 8.0;
            fixture.Floor.OtherFloorU[3] = 0.5;
            fixture.Floor.OtherFloorCoolingS[3] = 27.0;
            fixture.Floor.OtherFloorA[5] = 10.0;
            fixture.Floor.OtherFloorU[5] = 0.4;
            fixture.Floor.OtherFloorCoolingS[5] = 99.0;

            return fixture;
        }

        private static IReadOnlyList<EecalcHourlyWeatherFixture> BuildWeather(double baseTemperature, double baseHumidity)
        {
            return Enumerable.Range(0, 24)
                .Select(hour => new EecalcHourlyWeatherFixture
                {
                    Temperature = baseTemperature + Math.Sin((hour - 6) * Math.PI / 12.0) * 5.0,
                    Humidity = baseHumidity + Math.Cos(hour * Math.PI / 12.0) * 8.0
                })
                .ToList();
        }

        private static double ExpectedEta(double gamma, double ac, out string branch)
        {
            if (gamma > 0.0 && Math.Abs(gamma - 1.0) > 0.01)
            {
                branch = "positive_negative_power";
                return (1.0 - Math.Pow(gamma, 0.0 - ac)) / (1.0 - Math.Pow(gamma, 0.0 - (ac + 1.0)));
            }

            if (Math.Abs(gamma - 1.0) < 0.01)
            {
                branch = "near_one";
                return ac / (ac + 1.0);
            }

            if (gamma < 0.0)
            {
                branch = "negative_gamma";
                return 1.0;
            }

            branch = "fallback_zero";
            return 0.0;
        }

        private static void WriteDebugCsv(IReadOnlyList<EecalcMonthlyCoolingOracleRow> rows)
        {
            var path = GetDebugCsvPath();
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            var lines = new List<string>
            {
                "Month,Qsol,Qint,Qoccupants,Qgain,QtrCooling,Qinf,Qloss,Ac,Eta,QLatentOccupants,QLatentInf,QLatentVent,QcoolRaw,QfreeCooling,QveCooling,QcoolWithInputs"
            };
            lines.AddRange(rows.Select(row => row.ToDebugCsvRow()));

            File.WriteAllLines(path, lines);
        }

        private static string GetDebugCsvPath()
        {
            return Path.Combine(FindRepositoryRoot(), "test-results", "validation", "debug_cooling_full_pipeline.csv");
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
    }
}
