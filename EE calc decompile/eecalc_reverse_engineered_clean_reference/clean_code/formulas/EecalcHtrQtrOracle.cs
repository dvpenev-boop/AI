using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcHtrQtrOracle
    {
        public double CalculateParameterHgCurrent(EecalcEnvelopeFixture fixture)
        {
            ArgumentNullException.ThrowIfNull(fixture);

            return fixture.Floor.AccumulateFloorA * fixture.Floor.AccumulateFloorU;
        }

        public double CalculateItemsWalls(EecalcWallDirectionFixture walls)
        {
            ArgumentNullException.ThrowIfNull(walls);

            return SumProduct(walls.OuterA, walls.OuterU, 6)
                + Sum(walls.OuterSumL, 6)
                + Sum(walls.OuterSumX, 6);
        }

        public double SumAllDirectionsWallsCurrent(EecalcEnvelopeFixture fixture)
        {
            ArgumentNullException.ThrowIfNull(fixture);

            return WallDirections(fixture).Sum(CalculateItemsWalls);
        }

        public double SumAllDirectionWindowsCurrent(EecalcEnvelopeFixture fixture)
        {
            ArgumentNullException.ThrowIfNull(fixture);

            return WallDirections(fixture).Sum(wall => wall.AccumulateWindowU * wall.AccumulateWindowA);
        }

        public double SumNonTrasparentRoof(EecalcRoofFixture roof)
        {
            ArgumentNullException.ThrowIfNull(roof);

            return SumProduct(roof.NonTransparentA, roof.NonTransparentU, 9)
                + Sum(roof.NonTransparentSumL, 9)
                + Sum(roof.NonTransparentSumX, 9);
        }

        public double SumTrasparentRoof(EecalcRoofFixture roof)
        {
            ArgumentNullException.ThrowIfNull(roof);

            return SumProduct(roof.TransparentA, roof.TransparentU, 9);
        }

        public double CalculateParameterHdCurrent(EecalcEnvelopeFixture fixture)
        {
            ArgumentNullException.ThrowIfNull(fixture);

            return SumAllDirectionsWallsCurrent(fixture)
                + SumAllDirectionWindowsCurrent(fixture)
                + SumNonTrasparentRoof(fixture.Roof)
                + SumTrasparentRoof(fixture.Roof);
        }

        public double CalcWallDirectionParameterHu1(
            EecalcWallDirectionFixture wall,
            double averageMontlyTemp,
            double averageInnerHeatTemp)
        {
            ArgumentNullException.ThrowIfNull(wall);

            var denominator = averageInnerHeatTemp - averageMontlyTemp;
            return wall.InnerA[0] * wall.InnerU[0] * (averageInnerHeatTemp - wall.InnerW[0]) / denominator
                + wall.InnerA[1] * wall.InnerU[1] * (averageInnerHeatTemp - wall.InnerW[1]) / denominator
                + wall.InnerA[2] * wall.InnerU[2] * (averageInnerHeatTemp - wall.InnerW[2]) / denominator
                + wall.InnerA[3] * wall.InnerU[3] * (averageInnerHeatTemp - wall.InnerW[3]) / denominator
                + wall.InnerA[4] * wall.InnerA[4] * (averageInnerHeatTemp - wall.InnerW[4]) / denominator
                + wall.InnerA[5] * wall.InnerU[5] * (averageInnerHeatTemp - wall.InnerW[5]) / denominator;
        }

        public double SumWallDirecrionsHu1(
            EecalcEnvelopeFixture fixture,
            double averageMontlyTemp,
            double averageInnerHeatTemp)
        {
            ArgumentNullException.ThrowIfNull(fixture);

            return 8.0 * CalcWallDirectionParameterHu1(fixture.NorthWalls, averageMontlyTemp, averageInnerHeatTemp);
        }

        public double CalcCeilingsParameterHu2(
            EecalcRoofFixture roof,
            double averageMontlyTemp,
            double averageInnerHeatTemp)
        {
            ArgumentNullException.ThrowIfNull(roof);

            var denominator = averageInnerHeatTemp - averageMontlyTemp;
            return roof.CeilingA[0] * roof.CeilingU[0] * (averageInnerHeatTemp - roof.CeilingW[0]) / denominator
                + roof.CeilingA[1] * roof.CeilingU[1] * (averageInnerHeatTemp - roof.CeilingW[1]) / denominator
                + roof.CeilingA[2] * roof.CeilingU[2] * (averageInnerHeatTemp - roof.CeilingW[2]) / denominator
                + roof.CeilingA[3] * roof.CeilingU[3] * (averageInnerHeatTemp - roof.CeilingW[3]) / denominator
                + roof.CeilingA[4] * roof.CeilingA[4] * (averageInnerHeatTemp - roof.CeilingW[4]) / denominator
                + roof.CeilingA[5] * roof.CeilingU[5] * (averageInnerHeatTemp - roof.CeilingW[5]) / denominator;
        }

        public double CalcFloorsParameterHu3(
            EecalcFloorFixture floor,
            double averageMontlyTemp,
            double averageInnerHeatTemp)
        {
            ArgumentNullException.ThrowIfNull(floor);

            var denominator = averageInnerHeatTemp - averageMontlyTemp;
            return floor.OtherFloorA[0] * floor.OtherFloorU[0] * (averageInnerHeatTemp - floor.OtherFloorW[0]) / denominator
                + floor.OtherFloorA[1] * floor.OtherFloorU[1] * (averageInnerHeatTemp - floor.OtherFloorW[1]) / denominator
                + floor.OtherFloorA[2] * floor.OtherFloorU[2] * (averageInnerHeatTemp - floor.OtherFloorW[2]) / denominator
                + floor.OtherFloorA[3] * floor.OtherFloorU[3] * (averageInnerHeatTemp - floor.OtherFloorW[3]) / denominator
                + floor.OtherFloorA[4] * floor.OtherFloorU[4] * (averageInnerHeatTemp - floor.OtherFloorW[4]) / denominator
                + floor.OtherFloorA[5] * floor.OtherFloorU[5] * (averageInnerHeatTemp - floor.OtherFloorW[5]) / denominator;
        }

        public double CalculateAverageHeatTempCurrent(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(month);

            var projectHours = CalculateProjectHours(fixture, month);
            var nonProjectHours = CalculateNonProjectHours(fixture, month);

            return (projectHours * fixture.ProjectTemperature + nonProjectHours * fixture.NonProjectTemperature)
                / (projectHours + nonProjectHours);
        }

        public double CalculateParameterHtr(
            EecalcEnvelopeFixture fixture,
            double averageMontlyTemp,
            double averageInnerHeatTemp,
            out double hd,
            out double hg,
            out double huWalls,
            out double huCeilings,
            out double huFloors)
        {
            ArgumentNullException.ThrowIfNull(fixture);

            huWalls = SumWallDirecrionsHu1(fixture, averageMontlyTemp, averageInnerHeatTemp);
            huCeilings = CalcCeilingsParameterHu2(fixture.Roof, averageMontlyTemp, averageInnerHeatTemp);
            huFloors = CalcFloorsParameterHu3(fixture.Floor, averageMontlyTemp, averageInnerHeatTemp);
            hd = CalculateParameterHdCurrent(fixture);
            hg = CalculateParameterHgCurrent(fixture);

            return hd + hg + huWalls + huCeilings + huFloors;
        }

        public double CalculateParameterQtr(
            EecalcEnvelopeFixture fixture,
            EecalcMonthlyDaysOracleRow month,
            out EecalcEnvelopeSnapshotRow snapshot)
        {
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(month);

            var calculation = fixture.Calculation;
            var avgTemp = calculation.AverageOutdoorTemperatureByMonth.TryGetValue(month.Month, out var temperature)
                ? temperature
                : 0.0;
            var avgInnerHeatTemp = CalculateAverageHeatTempCurrent(calculation, month);
            var htr = CalculateParameterHtr(
                fixture,
                avgTemp,
                avgInnerHeatTemp,
                out var hd,
                out var hg,
                out var huWalls,
                out var huCeilings,
                out var huFloors);
            var degreeHours = CalculateDegreeHours(calculation, month, avgTemp);
            var qtr = htr * degreeHours / 1000.0;

            snapshot = new EecalcEnvelopeSnapshotRow
            {
                Fixture = fixture.Id,
                Month = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month.Month),
                AvgTemp = avgTemp,
                AvgInnerHeatTemp = avgInnerHeatTemp,
                Hd = hd,
                Hg = hg,
                HuWalls = huWalls,
                HuCeilings = huCeilings,
                HuFloors = huFloors,
                Hu = huWalls + huCeilings + huFloors,
                Htr = htr,
                DegreeHours = degreeHours,
                Qtr = qtr
            };

            return qtr;
        }

        public static double CalculateDegreeHours(
            EecalcValidationFixture fixture,
            EecalcMonthlyDaysOracleRow month,
            double avgTemp)
        {
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(month);

            var projectHours = CalculateProjectHours(fixture, month);
            var nonProjectHours = CalculateNonProjectHours(fixture, month);

            return (fixture.ProjectTemperature - avgTemp) * projectHours
                + (fixture.NonProjectTemperature - avgTemp) * nonProjectHours;
        }

        private static IReadOnlyList<EecalcWallDirectionFixture> WallDirections(EecalcEnvelopeFixture fixture)
        {
            return new[]
            {
                fixture.NorthWalls,
                fixture.NorthEastWalls,
                fixture.EastWalls,
                fixture.SouthEastWalls,
                fixture.SouthWalls,
                fixture.SouthWestWalls,
                fixture.WestWalls,
                fixture.NorthWestWalls
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

        private static double SumProduct(IReadOnlyList<double> left, IReadOnlyList<double> right, int count)
        {
            var total = 0.0;
            for (var i = 0; i < count; i++)
            {
                total += left[i] * right[i];
            }

            return total;
        }

        private static double Sum(IReadOnlyList<double> values, int count)
        {
            var total = 0.0;
            for (var i = 0; i < count; i++)
            {
                total += values[i];
            }

            return total;
        }
    }
}
