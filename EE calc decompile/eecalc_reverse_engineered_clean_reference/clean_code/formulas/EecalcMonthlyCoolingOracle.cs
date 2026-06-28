using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcMonthlyCoolingOracle
    {
        private const double AirHeatCapacityFactor = 0.34;
        private const double LatentFactor = 0.6947222222222222;

        private readonly EecalcMonthlyDaysOracle monthlyDaysOracle = new();

        public EecalcMonthlyCoolingOracleResult Calculate(
            EecalcEnvelopeFixture fixture,
            double ventilationInputs = 0.0)
        {
            ArgumentNullException.ThrowIfNull(fixture);

            var months = MonthlyDays(fixture.Calculation);
            var rows = months.Select(month => CalculateMonth(fixture, month)).ToList();
            var resultNoInputs = rows.Sum(row => row.QcoolRaw) / fixture.Calculation.HeatedArea;
            var resultCoolingInputs = rows.Sum(row => row.QfreeCooling);
            var resultNetEnergy = resultNoInputs - resultCoolingInputs - ventilationInputs;

            return new EecalcMonthlyCoolingOracleResult
            {
                Rows = rows,
                ResultNoInputsNetEnergy = resultNoInputs,
                ResultCoolingInputs = resultCoolingInputs,
                ResultVentilationInputs = ventilationInputs,
                ResultNetEnergy = resultNetEnergy
            };
        }

        public IReadOnlyList<EecalcMonthlyDaysOracleRow> MonthlyDays(EecalcValidationFixture fixture)
        {
            return monthlyDaysOracle.Calculate(fixture);
        }

        public EecalcMonthlyCoolingOracleRow CalculateMonth(
            EecalcEnvelopeFixture fixture,
            EecalcMonthlyDaysOracleRow month)
        {
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(month);

            var calculation = fixture.Calculation;
            var qsol = Qsol(fixture, month);
            var qint = Qint(calculation, month);
            var qoccupants = Qoccupants(calculation, month);
            var qgain = qsol + qint + qoccupants;
            var htrCooling = HtrCooling(fixture, month, out var hd, out var hg, out var huWalls, out var huCeilings, out var huFloors);
            var qtrCooling = QtrCooling(fixture, month, htrCooling);
            var hinf = Hinf(calculation);
            var qinf = Qinf(calculation, month, hinf);
            var qloss = qtrCooling + qinf;
            var ac = Ac(calculation, htrCooling, hinf);
            var eta = Eta(ac, qloss, qgain, out var gamma, out var etaBranch);
            var qLatentOccupants = QLatentOccupants(calculation, month);
            var qLatentInf = QLatentInf(calculation, month);
            var qLatentVent = QLatentVent(calculation, month);
            var qcoolRaw = qgain - eta * qloss + qLatentOccupants + qLatentInf + qLatentVent;
            var qfreeCooling = QfreeCooling(calculation, month);
            var qveCooling = QveCooling(calculation, month);
            var qcoolWithInputs = qcoolRaw + qfreeCooling + qveCooling;

            return new EecalcMonthlyCoolingOracleRow
            {
                Month = month.Month,
                MonthName = month.MonthName,
                WorkDays = month.WorkDays,
                Saturdays = month.Saturdays,
                Sundays = month.Sundays,
                Holidays = month.Holidays,
                Qsol = qsol,
                Qint = qint,
                Qoccupants = qoccupants,
                Qgain = qgain,
                Hd = hd,
                Hg = hg,
                HuWalls = huWalls,
                HuCeilings = huCeilings,
                HuFloors = huFloors,
                Hu = huWalls + huCeilings + huFloors,
                HtrCooling = htrCooling,
                QtrCooling = qtrCooling,
                Hinf = hinf,
                Qinf = qinf,
                Qloss = qloss,
                Ac = ac,
                Gamma = gamma,
                Eta = eta,
                EtaBranch = etaBranch,
                QLatentOccupants = qLatentOccupants,
                QLatentInf = qLatentInf,
                QLatentVent = qLatentVent,
                QcoolRaw = qcoolRaw,
                QfreeCooling = qfreeCooling,
                QveCooling = qveCooling,
                QcoolWithInputs = qcoolWithInputs
            };
        }

        public double Qsol(EecalcEnvelopeFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(month);

            var solar = GetSolarRadiation(fixture.Calculation, month.Month);
            var transparent = CalculateTrasparentFsol(fixture, solar);
            var nonTransparent = CalculateNonTrasparentFsol(fixture, solar);
            var hours = CoolingProjectHours(fixture.Calculation, month) + CoolingNonProjectHours(fixture.Calculation, month);

            return (transparent + nonTransparent) * hours / 1000.0;
        }

        public double Qint(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(month);

            var lights = fixture.LightsCoolingPower * (fixture.LightsCoolingWorkSchedule * month.Weeks) / 1000.0;
            var devices = fixture.BalancedDevicesCoolingPower * (fixture.BalancedDevicesCoolingWorkSchedule * month.Weeks) / 1000.0;

            return (lights + devices) * fixture.HeatedArea;
        }

        public double Qoccupants(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            return fixture.MetabolicHeat * OccupantHours(fixture, month) / 1000.0 * fixture.HeatedArea;
        }

        public double Qgain(EecalcEnvelopeFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            return Qsol(fixture, month) + Qint(fixture.Calculation, month) + Qoccupants(fixture.Calculation, month);
        }

        public double HtrCooling(
            EecalcEnvelopeFixture fixture,
            EecalcMonthlyDaysOracleRow month,
            out double hd,
            out double hg,
            out double huWalls,
            out double huCeilings,
            out double huFloors)
        {
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(month);

            var avgOutdoor = GetAverageOutdoorTemperature(fixture.Calculation, month.Month);
            var avgInnerCool = AverageCoolingTemp(fixture.Calculation, month);
            huWalls = SumWallDirecrionsHu1Cooling(fixture, avgOutdoor, avgInnerCool);
            huCeilings = CalcCeilingsParameterHu2Cooling(fixture.Roof, avgOutdoor, avgInnerCool);
            huFloors = CalcFloorsParameterHu3Cooling(fixture.Floor, avgOutdoor, avgInnerCool);
            hd = CalculateParameterHdCurrent(fixture);
            hg = CalculateParameterHgCurrent(fixture);

            return hd + hg + huWalls + huCeilings + huFloors;
        }

        public double HtrCooling(EecalcEnvelopeFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            return HtrCooling(fixture, month, out _, out _, out _, out _, out _);
        }

        public double QtrCooling(EecalcEnvelopeFixture fixture, EecalcMonthlyDaysOracleRow month, double htrCooling)
        {
            var degreeHours = CoolingDegreeHours(fixture.Calculation, month);
            return htrCooling * degreeHours / 1000.0;
        }

        public double Hinf(EecalcValidationFixture fixture)
        {
            return fixture.HeatedVolume * fixture.Infiltration * AirHeatCapacityFactor;
        }

        public double Qinf(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month, double hinf)
        {
            return hinf * CoolingDegreeHours(fixture, month) / 1000.0;
        }

        public double Ac(EecalcValidationFixture fixture, double htrCooling, double hinf)
        {
            var tau = fixture.HeatedArea * fixture.HeatCapacity / (htrCooling + hinf);
            return 1.0 + tau / 15.0;
        }

        public double Eta(double ac, double loses, double gainings, out double gamma, out string branch)
        {
            gamma = gainings / loses;
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

        public double QLatentOccupants(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            return fixture.LatentMetabolicHeat * OccupantHours(fixture, month) / 1000.0 * fixture.HeatedArea;
        }

        public double QLatentInf(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            var weather = GetHourlyWeather(fixture, month.Month);
            var work = SumLatentInfDay(
                weather,
                fixture.OccupantsWorkdaySchedule,
                fixture.ProjectTemperature,
                fixture.NonProjectTemperature,
                fixture.ProjectHumidity) * month.WorkDays;
            var saturday = SumLatentInfDay(
                weather,
                fixture.OccupantsSaturdaySchedule,
                fixture.ProjectTemperature,
                fixture.NonProjectTemperature,
                fixture.ProjectHumidity) * month.Saturdays;
            var sunday = SumLatentInfDay(
                weather,
                fixture.OccupantsSundaySchedule,
                fixture.ProjectTemperature,
                fixture.NonProjectTemperature,
                fixture.ProjectHumidity) * month.Sundays;
            var result = fixture.HeatedVolume * fixture.Infiltration * (1.0 / fixture.HeatedArea) * (work + saturday + sunday) * LatentFactor;

            return double.IsNaN(result) || double.IsInfinity(result) ? 0.0 : result;
        }

        public double QLatentVent(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            var weather = GetHourlyWeather(fixture, month.Month);
            var work = SumLatentVentDay(weather, fixture.VentilationWorkdaySchedule, fixture, doubleDebitAfterEnd: false)
                * month.WorkDays;
            work = CleanFinite(work);
            var saturday = SumLatentVentDay(weather, fixture.VentilationSaturdaySchedule, fixture, doubleDebitAfterEnd: true)
                * month.Saturdays;
            saturday = CleanFinite(saturday);
            var sunday = SumLatentVentDay(weather, fixture.VentilationSundaySchedule, fixture, doubleDebitAfterEnd: false)
                * month.Sundays;
            sunday = CleanFinite(sunday);
            var holiday = SumLatentVentHoliday(fixture) * month.Holidays;
            holiday = CleanFinite(holiday);

            return work + saturday + sunday + holiday;
        }

        public double QfreeCooling(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            var weather = GetHourlyWeather(fixture, month.Month);
            var hfree = fixture.VentilationDebit * AirHeatCapacityFactor;
            var work = SumNightFreeCooling(weather, fixture.NightVentilationWorkdaySchedule, hfree, fixture.ProjectTemperature)
                * month.WorkDays;
            var saturday = SumNightFreeCooling(weather, fixture.NightVentilationSaturdaySchedule, hfree, fixture.ProjectTemperature)
                * month.Saturdays;
            var sunday = SumNightFreeCooling(weather, fixture.NightVentilationSundaySchedule, hfree, fixture.ProjectTemperature)
                * month.Sundays;

            // KD-C006: EECalc uses the Sunday night-ventilation schedule for holidays.
            var holiday = SumNightFreeCooling(weather, fixture.NightVentilationSundaySchedule, hfree, fixture.NonProjectTemperature)
                * month.Holidays;

            return work + saturday + sunday + holiday;
        }

        public double QveCooling(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            var hve = fixture.VentilationDebit * AirHeatCapacityFactor;
            var work = SumQveDay(
                hve,
                fixture.FlowTemperature,
                fixture.ProjectTemperature,
                fixture.NonProjectTemperature,
                fixture.VentilationWorkdaySchedule,
                fixture.OccupantsWorkdaySchedule) * month.WorkDays;
            var saturday = SumQveDay(
                hve,
                fixture.FlowTemperature,
                fixture.ProjectTemperature,
                fixture.NonProjectTemperature,
                fixture.VentilationSaturdaySchedule,
                fixture.OccupantsSaturdaySchedule) * month.Saturdays;
            var sunday = SumQveDay(
                hve,
                fixture.FlowTemperature,
                fixture.ProjectTemperature,
                fixture.NonProjectTemperature,
                fixture.VentilationSundaySchedule,
                fixture.OccupantsSundaySchedule) * month.Sundays;
            var holidays = Enumerable.Range(0, 24)
                .Sum(_ => hve * (fixture.NonProjectTemperature - fixture.FlowTemperature) / 1000.0)
                * month.Holidays;

            return work + saturday + sunday + holidays;
        }

        private static double SumLatentInfDay(
            IReadOnlyList<EecalcHourlyWeatherFixture> weather,
            EecalcDailySchedule occupants,
            double projectTemperature,
            double nonProjectTemperature,
            double projectHumidity)
        {
            var nonProject = 0.0;
            var project = 0.0;

            for (var hour = 0; hour < occupants.StartHour; hour++)
            {
                var outdoor = WeatherAt(weather, hour);
                nonProject += CalcRo(outdoor.Temperature, outdoor.Humidity) * CalcAirX(outdoor.Temperature, outdoor.Humidity)
                    - CalcRo(nonProjectTemperature, projectHumidity) * CalcAirX(nonProjectTemperature, projectHumidity);
            }

            for (var hour = occupants.StartHour; hour < occupants.EndHour; hour++)
            {
                var outdoor = WeatherAt(weather, hour);
                project += CalcRo(outdoor.Temperature, outdoor.Humidity) * CalcAirX(outdoor.Temperature, outdoor.Humidity)
                    - CalcRo(projectTemperature, projectHumidity) * CalcAirX(projectTemperature, projectHumidity);
            }

            for (var hour = occupants.EndHour; hour < 24; hour++)
            {
                var outdoor = WeatherAt(weather, hour);
                nonProject += CalcRo(outdoor.Temperature, outdoor.Humidity) * CalcAirX(outdoor.Temperature, outdoor.Humidity)
                    - CalcRo(nonProjectTemperature, projectHumidity) * CalcAirX(nonProjectTemperature, projectHumidity);
            }

            return nonProject + project;
        }

        private static double SumLatentVentDay(
            IReadOnlyList<EecalcHourlyWeatherFixture> weather,
            EecalcDailySchedule ventilation,
            EecalcValidationFixture fixture,
            bool doubleDebitAfterEnd)
        {
            var active = 0.0;
            var inactive = 0.0;

            for (var hour = 0; hour < ventilation.StartHour; hour++)
            {
                inactive += fixture.VentilationDebit * LatentVentDelta(fixture, ShiftedWeatherAt(weather, hour)) * LatentFactor;
            }

            for (var hour = ventilation.StartHour; hour < ventilation.EndHour; hour++)
            {
                active += fixture.VentilationDebit * LatentVentDelta(fixture, ShiftedWeatherAt(weather, hour)) * LatentFactor;
            }

            for (var hour = ventilation.EndHour; hour < 24; hour++)
            {
                var multiplier = doubleDebitAfterEnd
                    ? fixture.VentilationDebit * fixture.VentilationDebit
                    : fixture.VentilationDebit;
                inactive += multiplier * LatentVentDelta(fixture, ShiftedWeatherAt(weather, hour)) * LatentFactor;
            }

            return active + inactive;
        }

        private static double SumLatentVentHoliday(EecalcValidationFixture fixture)
        {
            var supply = CalcRoW(fixture.FlowTemperature) * CalcAirX(fixture.FlowTemperature, fixture.FlowRelativeHumidity);
            var inside = CalcRoW(fixture.NonProjectTemperature) * CalcAirX(fixture.NonProjectTemperature, fixture.ProjectHumidity);
            return Enumerable.Range(0, 24)
                .Sum(_ => fixture.VentilationDebit * (supply - inside) * LatentFactor);
        }

        private static double LatentVentDelta(EecalcValidationFixture fixture, EecalcHourlyWeatherFixture outdoor)
        {
            return CalcRoW(fixture.FlowTemperature) * CalcAirX(fixture.FlowTemperature, fixture.FlowRelativeHumidity)
                - CalcRoW(outdoor.Temperature) * CalcAirX(outdoor.Temperature, outdoor.Humidity);
        }

        private static double SumNightFreeCooling(
            IReadOnlyList<EecalcHourlyWeatherFixture> weather,
            EecalcDailySchedule schedule,
            double hfree,
            double indoorTemperature)
        {
            return GetNightWorkingHours(schedule.StartHour, schedule.EndHour)
                .Sum(hour => hfree * (indoorTemperature - WeatherAt(weather, hour).Temperature) / 1000.0);
        }

        private static double SumQveDay(
            double hve,
            double flowTemperature,
            double projectTemperature,
            double nonProjectTemperature,
            EecalcDailySchedule ventilation,
            EecalcDailySchedule occupants)
        {
            var active = 0.0;
            var inactive = 0.0;

            for (var hour = 0; hour < ventilation.StartHour; hour++)
            {
                var indoor = hour < occupants.StartHour ? nonProjectTemperature : projectTemperature;
                inactive += hve * (indoor - flowTemperature) / 1000.0;
            }

            for (var hour = ventilation.StartHour; hour < ventilation.EndHour; hour++)
            {
                var indoor = hour >= occupants.StartHour && hour < occupants.EndHour
                    ? projectTemperature
                    : nonProjectTemperature;
                active += hve * (indoor - flowTemperature) / 1000.0;
            }

            for (var hour = ventilation.EndHour; hour < 24; hour++)
            {
                var indoor = hour < occupants.EndHour ? projectTemperature : nonProjectTemperature;
                inactive += hve * (indoor - flowTemperature) / 1000.0;
            }

            return active + inactive;
        }

        private static double AverageCoolingTemp(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            var projectHours = CoolingProjectHours(fixture, month);
            var nonProjectHours = CoolingNonProjectHours(fixture, month);
            return (projectHours * fixture.ProjectTemperature + nonProjectHours * fixture.NonProjectTemperature)
                / (projectHours + nonProjectHours);
        }

        private static double CoolingDegreeHours(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            var avgOutdoor = GetAverageOutdoorTemperature(fixture, month.Month);
            return (fixture.ProjectTemperature - avgOutdoor) * CoolingProjectHours(fixture, month)
                + (fixture.NonProjectTemperature - avgOutdoor) * CoolingNonProjectHours(fixture, month);
        }

        private static double CoolingProjectHours(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            return month.WorkDays * Duration(fixture.WorkdaySchedule)
                + month.Saturdays * Duration(fixture.SaturdaySchedule)
                + month.Sundays * Duration(fixture.SundaySchedule);
        }

        private static double CoolingNonProjectHours(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            return month.WorkDays * (24.0 - Duration(fixture.WorkdaySchedule))
                + month.Saturdays * (24.0 - Duration(fixture.SaturdaySchedule))
                + month.Sundays * (24.0 - Duration(fixture.SundaySchedule))
                + month.Holidays * 24.0;
        }

        private static double OccupantHours(EecalcValidationFixture fixture, EecalcMonthlyDaysOracleRow month)
        {
            return month.WorkDays * Duration(fixture.OccupantsWorkdaySchedule)
                + month.Saturdays * Duration(fixture.OccupantsSaturdaySchedule)
                + month.Sundays * Duration(fixture.OccupantsSundaySchedule);
        }

        private static int Duration(EecalcDailySchedule schedule)
        {
            return schedule.EndHour - schedule.StartHour;
        }

        private static IEnumerable<int> GetNightWorkingHours(int start, int end)
        {
            if (start == end)
            {
                return Array.Empty<int>();
            }

            var hours = new List<int>();
            if (start > end)
            {
                for (var hour = 0; hour < end; hour++)
                {
                    hours.Add(hour);
                }

                for (var hour = start; hour < 24; hour++)
                {
                    hours.Add(hour);
                }
            }
            else
            {
                for (var hour = start; hour < end; hour++)
                {
                    hours.Add(hour);
                }
            }

            return hours;
        }

        private static double CalculateParameterHdCurrent(EecalcEnvelopeFixture fixture)
        {
            return WallDirections(fixture).Sum(CalculateItemsWalls)
                + WallDirections(fixture).Sum(wall => wall.AccumulateWindowU * wall.AccumulateWindowA)
                + SumProduct(fixture.Roof.NonTransparentA, fixture.Roof.NonTransparentU, 9)
                + Sum(fixture.Roof.NonTransparentSumL, 9)
                + Sum(fixture.Roof.NonTransparentSumX, 9)
                + SumProduct(fixture.Roof.TransparentA, fixture.Roof.TransparentU, 9);
        }

        private static double CalculateItemsWalls(EecalcWallDirectionFixture walls)
        {
            return SumProduct(walls.OuterA, walls.OuterU, 6)
                + Sum(walls.OuterSumL, 6)
                + Sum(walls.OuterSumX, 6);
        }

        private static double CalculateParameterHgCurrent(EecalcEnvelopeFixture fixture)
        {
            return fixture.Floor.AccumulateFloorA * fixture.Floor.AccumulateFloorU;
        }

        private static double SumWallDirecrionsHu1Cooling(
            EecalcEnvelopeFixture fixture,
            double averageMontlyTemp,
            double averageInnerCoolTemp)
        {
            // KD-C001: EECalc uses NorthWalls.Current eight times.
            return 8.0 * CalcWallDirectionParameterHu1Cooling(fixture.NorthWalls, averageMontlyTemp, averageInnerCoolTemp);
        }

        private static double CalcWallDirectionParameterHu1Cooling(
            EecalcWallDirectionFixture wall,
            double averageMontlyTemp,
            double averageInnerCoolTemp)
        {
            var denominator = averageInnerCoolTemp - averageMontlyTemp;
            if (object.Equals(denominator, 0.0))
            {
                return 0.0;
            }

            return wall.InnerA[0] * wall.InnerU[0] * (averageInnerCoolTemp - wall.InnerCoolingS[0]) / denominator
                + wall.InnerA[1] * wall.InnerU[1] * (averageInnerCoolTemp - wall.InnerCoolingS[1]) / denominator
                + wall.InnerA[2] * wall.InnerU[2] * (averageInnerCoolTemp - wall.InnerCoolingS[2]) / denominator
                + wall.InnerA[3] * wall.InnerU[3] * (averageInnerCoolTemp - wall.InnerCoolingS[3]) / denominator
                // KD-C002: IneerA5 is used as both area and U value.
                + wall.InnerA[4] * wall.InnerA[4] * (averageInnerCoolTemp - wall.InnerCoolingS[4]) / denominator
                + wall.InnerA[5] * wall.InnerU[5] * (averageInnerCoolTemp - wall.InnerCoolingS[5]) / denominator;
        }

        private static double CalcCeilingsParameterHu2Cooling(
            EecalcRoofFixture roof,
            double averageMontlyTemp,
            double averageInnerCoolTemp)
        {
            var denominator = averageInnerCoolTemp - averageMontlyTemp;
            if (object.Equals(denominator, 0.0))
            {
                return 0.0;
            }

            return roof.CeilingA[0] * roof.CeilingU[0] * (averageInnerCoolTemp - roof.CeilingCoolingS[0]) / denominator
                + roof.CeilingA[1] * roof.CeilingU[1] * (averageInnerCoolTemp - roof.CeilingCoolingS[1]) / denominator
                + roof.CeilingA[2] * roof.CeilingU[2] * (averageInnerCoolTemp - roof.CeilingCoolingS[2]) / denominator
                + roof.CeilingA[3] * roof.CeilingU[3] * (averageInnerCoolTemp - roof.CeilingCoolingS[3]) / denominator
                // KD-C003: CeilingA5 is used as both area and U value.
                + roof.CeilingA[4] * roof.CeilingA[4] * (averageInnerCoolTemp - roof.CeilingCoolingS[4]) / denominator
                + roof.CeilingA[5] * roof.CeilingU[5] * (averageInnerCoolTemp - roof.CeilingCoolingS[5]) / denominator;
        }

        private static double CalcFloorsParameterHu3Cooling(
            EecalcFloorFixture floor,
            double averageMontlyTemp,
            double averageInnerCoolTemp)
        {
            var denominator = averageInnerCoolTemp - averageMontlyTemp;
            if (object.Equals(denominator, 0.0))
            {
                return 0.0;
            }

            return floor.OtherFloorA[0] * floor.OtherFloorU[0] * (averageInnerCoolTemp - floor.OtherFloorCoolingS[0]) / denominator
                + floor.OtherFloorA[1] * floor.OtherFloorU[1] * (averageInnerCoolTemp - floor.OtherFloorCoolingS[1]) / denominator
                + floor.OtherFloorA[2] * floor.OtherFloorU[2] * (averageInnerCoolTemp - floor.OtherFloorCoolingS[2]) / denominator
                + floor.OtherFloorA[3] * floor.OtherFloorU[3] * (averageInnerCoolTemp - floor.OtherFloorCoolingS[3]) / denominator
                + floor.OtherFloorA[4] * floor.OtherFloorU[4] * (averageInnerCoolTemp - floor.OtherFloorCoolingS[4]) / denominator
                // KD-C004: layer 6 uses OtherFloorS4.
                + floor.OtherFloorA[5] * floor.OtherFloorU[5] * (averageInnerCoolTemp - floor.OtherFloorCoolingS[3]) / denominator;
        }

        private static double CalculateTransparentFsol(
            double windowA,
            double windowG,
            double windowE,
            double sunShiningIntensity,
            bool horizontal = false)
        {
            var radiativeCoeff = 4m * (decimal)windowE * 0.0000000567m * (decimal)Math.Pow(283.0, 3.0);
            var loss = 0.04 * windowG * windowA * 11.0 * (double)radiativeCoeff;
            var directionFactor = horizontal ? 1.0 : 0.5;

            return windowA * windowG * sunShiningIntensity - directionFactor * loss;
        }

        private static double CalculateTrasparentFsol(EecalcEnvelopeFixture fixture, EecalcSolarRadiationFixture solar)
        {
            var total = CalculateWallTransparentFsol(fixture.NorthWalls, solar.N);
            total += CalculateWallTransparentFsol(fixture.NorthEastWalls, (solar.N + solar.E) / 2.0);
            total += CalculateWallTransparentFsol(fixture.EastWalls, solar.E);
            total += CalculateWallTransparentFsol(fixture.SouthEastWalls, (solar.S + solar.E) / 2.0);
            total += CalculateWallTransparentFsol(fixture.SouthWalls, solar.S);
            total += CalculateWallTransparentFsol(fixture.SouthWestWalls, (solar.S + solar.W) / 2.0);
            total += CalculateWallTransparentFsol(fixture.WestWalls, solar.W);
            total += CalculateWallTransparentFsol(fixture.NorthWestWalls, (solar.N + solar.W) / 2.0);

            total += CalculateTransparentFsol(fixture.Roof.TransparentA[0], fixture.Roof.TransparentG[0], fixture.Roof.TransparentE[0], solar.N);
            total += CalculateTransparentFsol(fixture.Roof.TransparentA[1], fixture.Roof.TransparentG[1], fixture.Roof.TransparentE[1], (solar.N + solar.E) / 2.0);
            total += CalculateTransparentFsol(fixture.Roof.TransparentA[2], fixture.Roof.TransparentG[2], fixture.Roof.TransparentE[2], solar.E);
            total += CalculateTransparentFsol(fixture.Roof.TransparentA[3], fixture.Roof.TransparentG[3], fixture.Roof.TransparentE[3], (solar.S + solar.E) / 2.0);
            total += CalculateTransparentFsol(fixture.Roof.TransparentA[4], fixture.Roof.TransparentG[4], fixture.Roof.TransparentE[4], solar.S);
            total += CalculateTransparentFsol(fixture.Roof.TransparentA[5], fixture.Roof.TransparentG[5], fixture.Roof.TransparentE[5], (solar.S + solar.W) / 2.0);
            total += CalculateTransparentFsol(fixture.Roof.TransparentA[6], fixture.Roof.TransparentG[6], fixture.Roof.TransparentE[6], solar.W);
            total += CalculateTransparentFsol(fixture.Roof.TransparentA[7], fixture.Roof.TransparentG[7], fixture.Roof.TransparentE[7], (solar.N + solar.W) / 2.0);
            total += CalculateTransparentFsol(fixture.Roof.TransparentA[8], fixture.Roof.TransparentG[8], fixture.Roof.TransparentE[8], solar.H, horizontal: true);

            return total;
        }

        private static double CalculateNonTransparentFsol(
            double outerWallAlfa,
            double outerWallU,
            double outerWallEpsi,
            double outerWallArea,
            double sunShiningIntensity,
            bool horizontal = false)
        {
            var absorbed = outerWallAlfa * 0.04 * outerWallU * outerWallArea;
            var radiativeCoeff = 4m * (decimal)outerWallEpsi * 0.0000000567m * (decimal)Math.Pow(283.0, 3.0);
            var loss = 0.04 * outerWallU * outerWallArea * 11.0 * (double)radiativeCoeff;
            var directionFactor = horizontal ? 1.0 : 0.5;

            return absorbed * sunShiningIntensity - directionFactor * loss;
        }

        private static double CalculateNonTrasparentFsol(EecalcEnvelopeFixture fixture, EecalcSolarRadiationFixture solar)
        {
            var total = CalculateWallNonTransparentFsol(fixture.NorthWalls, solar.N);
            total += CalculateWallNonTransparentFsol(fixture.NorthEastWalls, (solar.N + solar.E) / 2.0);
            total += CalculateWallNonTransparentFsol(fixture.EastWalls, solar.E);
            total += CalculateWallNonTransparentFsol(fixture.SouthEastWalls, (solar.S + solar.E) / 2.0);
            total += CalculateWallNonTransparentFsol(fixture.SouthWalls, solar.S);
            total += CalculateWallNonTransparentFsol(fixture.SouthWestWalls, (solar.S + solar.W) / 2.0);
            total += CalculateWallNonTransparentFsol(fixture.WestWalls, solar.W);
            total += CalculateWallNonTransparentFsol(fixture.NorthWestWalls, (solar.N + solar.W) / 2.0);

            total += CalculateNonTransparentFsol(
                fixture.Roof.AccumulateNonTransparentAlfa,
                fixture.Roof.AccumulateNonTransparentU,
                fixture.Roof.AccumulateNonTransparentE,
                fixture.Roof.AccumulateNonTransparentA,
                solar.H,
                horizontal: true);

            return total;
        }

        private static double CalculateWallTransparentFsol(EecalcWallDirectionFixture wall, double radiation)
        {
            return CalculateTransparentFsol(
                wall.AccumulateWindowA,
                wall.AccumulateWindowG,
                wall.AccumulateWindowE,
                radiation);
        }

        private static double CalculateWallNonTransparentFsol(EecalcWallDirectionFixture wall, double radiation)
        {
            return CalculateNonTransparentFsol(
                wall.AccumulateOuterAlfa,
                wall.AccumulateOuterU,
                wall.AccumulateOuterE,
                wall.AccumulateOuterA,
                radiation);
        }

        private static double CalcAirX(double temp, double humidity)
        {
            var kelvin = 273.15 + temp;
            var saturationPressure = Math.Pow(2.718281828459, 77.345 + 0.0057 * kelvin - 7235.0 / kelvin)
                / Math.Pow(kelvin, 8.2);
            var vapourPressure = humidity * saturationPressure / 100.0;
            return 0.62198 * (vapourPressure / (101325.0 - vapourPressure));
        }

        private static double CalcRoW(double temp)
        {
            var kelvin = temp + 273.15;
            return 101325.0 / (286.9 * kelvin);
        }

        private static double CalcRo(double temp, double humidity)
        {
            var airX = CalcAirX(temp, humidity);
            return CalcRoW(temp) * (1.0 + airX) / (1.0 + 1.609 * airX);
        }

        private static EecalcHourlyWeatherFixture WeatherAt(IReadOnlyList<EecalcHourlyWeatherFixture> weather, int hour)
        {
            var index = hour < weather.Count ? hour : 0;
            return weather[index];
        }

        private static EecalcHourlyWeatherFixture ShiftedWeatherAt(IReadOnlyList<EecalcHourlyWeatherFixture> weather, int hour)
        {
            if (weather.Count == 0)
            {
                return new EecalcHourlyWeatherFixture();
            }

            if (hour == 0)
            {
                return weather[Math.Min(23, weather.Count - 1)];
            }

            var index = hour - 1 < weather.Count ? hour - 1 : 0;
            return weather[index];
        }

        private static IReadOnlyList<EecalcHourlyWeatherFixture> GetHourlyWeather(EecalcValidationFixture fixture, int month)
        {
            if (fixture.HourlyWeatherByMonth.TryGetValue(month, out var value) && value.Count > 0)
            {
                return value;
            }

            var average = GetAverageOutdoorTemperature(fixture, month);
            return Enumerable.Range(0, 24)
                .Select(_ => new EecalcHourlyWeatherFixture { Temperature = average, Humidity = 50.0 })
                .ToList();
        }

        private static double GetAverageOutdoorTemperature(EecalcValidationFixture fixture, int month)
        {
            return fixture.AverageOutdoorTemperatureByMonth.TryGetValue(month, out var value) ? value : 0.0;
        }

        private static EecalcSolarRadiationFixture GetSolarRadiation(EecalcValidationFixture fixture, int month)
        {
            return fixture.SolarRadiationByMonth.TryGetValue(month, out var value)
                ? value
                : new EecalcSolarRadiationFixture();
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

        private static double CleanFinite(double value)
        {
            return double.IsNaN(value) || double.IsInfinity(value) ? 0.0 : value;
        }
    }

    public sealed class EecalcMonthlyCoolingOracleResult
    {
        public IReadOnlyList<EecalcMonthlyCoolingOracleRow> Rows { get; init; } =
            Array.Empty<EecalcMonthlyCoolingOracleRow>();

        public double ResultNoInputsNetEnergy { get; init; }

        public double ResultCoolingInputs { get; init; }

        public double ResultVentilationInputs { get; init; }

        public double ResultNetEnergy { get; init; }
    }

    public sealed class EecalcMonthlyCoolingOracleRow
    {
        public int Month { get; init; }

        public string MonthName { get; init; } = string.Empty;

        public int WorkDays { get; init; }

        public int Saturdays { get; init; }

        public int Sundays { get; init; }

        public int Holidays { get; init; }

        public double Qsol { get; init; }

        public double Qint { get; init; }

        public double Qoccupants { get; init; }

        public double Qgain { get; init; }

        public double Hd { get; init; }

        public double Hg { get; init; }

        public double HuWalls { get; init; }

        public double HuCeilings { get; init; }

        public double HuFloors { get; init; }

        public double Hu { get; init; }

        public double HtrCooling { get; init; }

        public double QtrCooling { get; init; }

        public double Hinf { get; init; }

        public double Qinf { get; init; }

        public double Qloss { get; init; }

        public double Ac { get; init; }

        public double Gamma { get; init; }

        public double Eta { get; init; }

        public string EtaBranch { get; init; } = string.Empty;

        public double QLatentOccupants { get; init; }

        public double QLatentInf { get; init; }

        public double QLatentVent { get; init; }

        public double QcoolRaw { get; init; }

        public double QfreeCooling { get; init; }

        public double QveCooling { get; init; }

        public double QcoolWithInputs { get; init; }

        public string ToDebugCsvRow()
        {
            return string.Join(",",
                MonthName,
                Format(Qsol),
                Format(Qint),
                Format(Qoccupants),
                Format(Qgain),
                Format(QtrCooling),
                Format(Qinf),
                Format(Qloss),
                Format(Ac),
                Format(Eta),
                Format(QLatentOccupants),
                Format(QLatentInf),
                Format(QLatentVent),
                Format(QcoolRaw),
                Format(QfreeCooling),
                Format(QveCooling),
                Format(QcoolWithInputs));
        }

        private static string Format(double value)
        {
            return value.ToString("G17", CultureInfo.InvariantCulture);
        }
    }
}
