using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Xunit;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcR3HtrQtrValidationTests
    {
        [Fact]
        public void R3_Hg_MinimalFloor()
        {
            var fixture = CreateMinimalEnvelopeFixture();
            var oracle = new EecalcHtrQtrOracle();

            var hg = oracle.CalculateParameterHgCurrent(fixture);

            Assert.Equal(50.0, hg, precision: 6);
        }

        [Fact]
        public void R3_Hd_MinimalNorthWall()
        {
            var fixture = CreateMinimalEnvelopeFixture();
            var oracle = new EecalcHtrQtrOracle();

            var hd = oracle.CalculateParameterHdCurrent(fixture);

            Assert.Equal(4.0, hd, precision: 6);
        }

        [Fact]
        public void R3_Hu_KD004_NorthWallRepeatedEightTimes()
        {
            var fixture = CreateMinimalEnvelopeFixture();
            var oracle = new EecalcHtrQtrOracle();

            var singleNorthHu = oracle.CalcWallDirectionParameterHu1(
                fixture.NorthWalls,
                averageMontlyTemp: 0.0,
                averageInnerHeatTemp: 20.0);
            var huWalls = oracle.SumWallDirecrionsHu1(
                fixture,
                averageMontlyTemp: 0.0,
                averageInnerHeatTemp: 20.0);

            Assert.Equal(2.5, singleNorthHu, precision: 6);
            Assert.Equal(8.0 * singleNorthHu, huWalls, precision: 6);
            Assert.Equal(20.0, huWalls, precision: 6);
        }

        [Fact]
        public void R3_Htr_MinimalEnvelope()
        {
            var fixture = CreateMinimalEnvelopeFixture();
            var month = CreateJanuaryWorkday();
            var oracle = new EecalcHtrQtrOracle();
            var avgTemp = fixture.Calculation.AverageOutdoorTemperatureByMonth[1];
            var avgInner = oracle.CalculateAverageHeatTempCurrent(fixture.Calculation, month);

            var htr = oracle.CalculateParameterHtr(
                fixture,
                avgTemp,
                avgInner,
                out var hd,
                out var hg,
                out var huWalls,
                out var huCeilings,
                out var huFloors);

            Assert.Equal(4.0, hd, precision: 6);
            Assert.Equal(50.0, hg, precision: 6);
            Assert.Equal(21.379310344827584, huWalls, precision: 12);
            Assert.Equal(0.0, huCeilings, precision: 6);
            Assert.Equal(0.0, huFloors, precision: 6);
            Assert.Equal(75.37931034482759, htr, precision: 12);
        }

        [Fact]
        public void R3_Qtr_MinimalEnvelope()
        {
            var fixture = CreateMinimalEnvelopeFixture();
            var month = CreateJanuaryWorkday();
            var oracle = new EecalcHtrQtrOracle();

            var qtr = oracle.CalculateParameterQtr(fixture, month, out var snapshot);
            WriteDebugCsv(snapshot);

            Assert.Equal(snapshot.Htr * snapshot.DegreeHours / 1000.0, qtr, precision: 12);
            Assert.Equal(464.0, snapshot.DegreeHours, precision: 6);
            Assert.Equal(34.976, qtr, precision: 6);
            Assert.True(File.Exists(GetDebugCsvPath()));
        }

        private static EecalcEnvelopeFixture CreateMinimalEnvelopeFixture()
        {
            var calculation = new EecalcValidationFixture
            {
                Id = "r3_minimal_envelope",
                Scenario = "Actual",
                FirstMonth = 1,
                LastMonth = 1,
                FirstDay = 1,
                LastDay = 1,
                HeatedArea = 100.0,
                HeatedVolume = 0.0,
                Infiltration = 0.0,
                ProjectTemperature = 21.0,
                NonProjectTemperature = 17.0,
                WorkdaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 20 },
                SaturdaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 12 },
                SundaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 0 },
                HolidaysByMonth = new Dictionary<int, int> { [1] = 0 },
                AverageOutdoorTemperatureByMonth = new Dictionary<int, double> { [1] = 1.0 }
            };

            var fixture = new EecalcEnvelopeFixture
            {
                Id = "r3_minimal_envelope",
                Calculation = calculation
            };

            fixture.NorthWalls.OuterA[0] = 10.0;
            fixture.NorthWalls.OuterU[0] = 0.4;
            fixture.NorthWalls.InnerA[0] = 10.0;
            fixture.NorthWalls.InnerU[0] = 0.5;
            fixture.NorthWalls.InnerW[0] = 10.0;
            fixture.Floor.AccumulateFloorA = 100.0;
            fixture.Floor.AccumulateFloorU = 0.5;

            return fixture;
        }

        private static EecalcMonthlyDaysOracleRow CreateJanuaryWorkday()
        {
            return new EecalcMonthlyDaysOracleRow
            {
                Month = 1,
                MonthName = "January",
                TotalDays = 31,
                WorkDays = 1,
                Saturdays = 0,
                Sundays = 0,
                Holidays = 0,
                Weeks = 1.0 / 7.0
            };
        }

        private static void WriteDebugCsv(EecalcEnvelopeSnapshotRow row)
        {
            var path = GetDebugCsvPath();
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllLines(path, new[]
            {
                "fixture,month,avgTemp,avgInnerHeatTemp,hd,hg,huWalls,huCeilings,huFloors,hu,htr,degreeHours,qtr",
                string.Join(",",
                    row.Fixture,
                    row.Month,
                    Format(row.AvgTemp),
                    Format(row.AvgInnerHeatTemp),
                    Format(row.Hd),
                    Format(row.Hg),
                    Format(row.HuWalls),
                    Format(row.HuCeilings),
                    Format(row.HuFloors),
                    Format(row.Hu),
                    Format(row.Htr),
                    Format(row.DegreeHours),
                    Format(row.Qtr))
            });
        }

        private static string GetDebugCsvPath()
        {
            return Path.Combine(FindRepositoryRoot(), "test-results", "validation", "debug_r3_htr_qtr.csv");
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
