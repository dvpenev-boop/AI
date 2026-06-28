using System;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Services.EecalcClimate;

namespace EE.Doklad.Tests.Validation.FullOracle
{
    public sealed class EECalcVentilationOracle
    {
        private const double AirHeatCapacityFactor = 0.34;
        private const double LatitudeRadians = 0.7382742735936013;

        private readonly LegacyEecalcXmlClimateDataProvider climate =
            new(ClimateProviderMode.LegacyEECalcStrict);
        private readonly EecalcMonthlyDaysOracle monthlyDaysOracle = new();

        public EECalcVentilationOracleResult Calculate(EecalcValidationFixture fixture, EECalcVentilationInput input)
        {
            ArgumentNullException.ThrowIfNull(fixture);
            ArgumentNullException.ThrowIfNull(input);

            var monthlyDays = monthlyDaysOracle.Calculate(fixture);
            var rows = monthlyDays.Select(month => CalculateMonth(fixture, input, month)).ToList();
            var heatingTotal = rows.Sum(row => row.MonthlyHeat + row.ThermoPumpEnergy);
            var heatingInputs = rows.Sum(row => row.HeatingInputs);
            var coolingTotal = rows.Sum(row => row.PowCooling);
            var coolingHeating = rows.Sum(row => row.PowHeating);
            var withering = rows.Sum(row => row.WitheringEnergy);
            var coolingInputs = rows.Sum(row => row.CoolingInputs);

            var sourceHeating1 = 0.0;
            var sourceHeating2 = 0.0;
            if (input.SecondRecEfficiency > 100.0)
            {
                sourceHeating1 = rows.Sum(row => row.ThermoPumpEnergy);
                sourceHeating2 = heatingTotal - sourceHeating1;
            }
            else
            {
                sourceHeating1 = heatingTotal * input.Part1 / 100.0;
                sourceHeating2 = heatingTotal * input.Part2 / 100.0;
            }

            sourceHeating1 = EECalcMath.DivideByEfficiency(sourceHeating1, EECalcMath.EfficiencyProduct(input.HeatingEfficiency1));
            sourceHeating2 = EECalcMath.DivideByEfficiency(sourceHeating2, EECalcMath.EfficiencyProduct(input.HeatingEfficiency2));

            var sourceCooling1 = EECalcMath.DivideByEfficiency(
                coolingTotal * input.Part1 / 100.0,
                EECalcMath.EfficiencyProduct(input.CoolingEfficiency1));
            var sourceCooling2 = EECalcMath.DivideByEfficiency(
                coolingTotal * input.Part2 / 100.0,
                EECalcMath.EfficiencyProduct(input.CoolingEfficiency2));

            return new EECalcVentilationOracleResult
            {
                Rows = rows,
                ResultEnergyForHeating = heatingTotal + coolingHeating,
                ResultEnergyForCooling = coolingTotal,
                ResultEnergyForWithering = withering,
                HeatingNeededEnergy = sourceHeating1 + sourceHeating2,
                CoolingNeededEnergy = sourceCooling1 + sourceCooling2,
                ResultSourceEnergy = sourceHeating1 + sourceCooling1,
                ResultSourceEnergy2 = sourceHeating2 + sourceCooling2,
                ResultNeededEnergy = sourceHeating1 + sourceHeating2 + sourceCooling1 + sourceCooling2,
                ResulVentilationInputs = heatingInputs + coolingInputs
            };
        }

        private EECalcVentilationMonthRow CalculateMonth(
            EecalcValidationFixture fixture,
            EECalcVentilationInput input,
            EecalcMonthlyDaysOracleRow month)
        {
            var avgTemp = climate.GetMonthlyAvgTemp(fixture.ClimateZoneId, ToMonth(month.Month));
            var hourly = climate.GetHourlyClimateData(fixture.ClimateZoneId, ToMonth(month.Month));
            var humidity = hourly.Average(hour => hour.Humidity);
            var monthHours = MonthHours(input, month);
            var averageVentHeatTemp = AverageVentHeatTemp(fixture, input, month);
            var num = averageVentHeatTemp - input.FirstRecEfficiency / 100.0 * (averageVentHeatTemp - avgTemp);
            var num2 = avgTemp + input.FirstRecEfficiency / 100.0 * (averageVentHeatTemp - avgTemp);
            var num3 = num2;
            var thermoPumpEnergy = 0.0;
            var monthlyHeat = 0.0;

            if (input.SecondRecEfficiency > 0.0)
            {
                if (input.HeatingAirDifference >= 3.0 && input.HeatingAirDifference <= 8.0)
                {
                    var h1 = CalcEntalpia(num, humidity, climate.GetPb(fixture.ClimateZoneId));
                    var h2 = CalcEntalpia(input.MinimumEndTemperature, humidity, climate.GetPb(fixture.ClimateZoneId));
                    var q = input.Debit * 1.2 * (h1 - h2) * monthHours / 3600.0;
                    thermoPumpEnergy = q / (1.0 - 100.0 / input.SecondRecEfficiency);
                    var airLift = input.Debit == 0.0 || monthHours == 0.0
                        ? 0.0
                        : thermoPumpEnergy * 1000.0 / (input.Debit * AirHeatCapacityFactor * monthHours);

                    if (airLift >= input.HeatingAirDifference)
                    {
                        thermoPumpEnergy = input.Debit * AirHeatCapacityFactor * input.HeatingAirDifference * monthHours / 1000.0;
                    }

                    if (airLift < input.FlowTemperature - num2)
                    {
                        num3 = input.FlowTemperature - num2 - airLift;
                        monthlyHeat = input.Debit * AirHeatCapacityFactor * (input.FlowTemperature - num3) * monthHours / 1000.0;
                    }
                    else
                    {
                        thermoPumpEnergy = input.Debit * AirHeatCapacityFactor * (input.FlowTemperature - num2) * monthHours / 1000.0;
                        monthlyHeat = 0.0;
                    }
                }
            }
            else
            {
                monthlyHeat = input.Debit * AirHeatCapacityFactor * (input.FlowTemperature - num3) * monthHours / 1000.0;
            }

            var heatingInputs = input.Debit * AirHeatCapacityFactor
                * (input.FlowTemperature - fixture.ProjectTemperature) * monthHours / 1000.0;
            var cooling = CalculateCoolingMonth(fixture, input, month, hourly);

            return new EECalcVentilationMonthRow
            {
                Month = month.Month,
                MonthHours = monthHours,
                AverageVentHeatTemp = averageVentHeatTemp,
                FirstRecoveryTemp = num,
                PostRecoveryTemp = num2,
                ThermoPumpEnergy = EECalcMath.CleanFinite(thermoPumpEnergy),
                MonthlyHeat = EECalcMath.CleanFinite(monthlyHeat),
                HeatingInputs = EECalcMath.CleanFinite(heatingInputs),
                PowHeating = cooling.PowHeating,
                PowCooling = cooling.PowCooling,
                WitheringEnergy = cooling.WitheringEnergy,
                CoolingInputs = cooling.CoolingInputs
            };
        }

        private CoolingVentMonth CalculateCoolingMonth(
            EecalcValidationFixture fixture,
            EECalcVentilationInput input,
            EecalcMonthlyDaysOracleRow month,
            IReadOnlyList<HourlyClimateData> hourly)
        {
            var shifted = ShiftedHours(hourly);
            var powHeating = 0.0;
            var powCooling = 0.0;
            var withering = 0.0;
            var coolingInputs = 0.0;

            AccumulateDay(input.WorkdaySchedule, fixture.WorkdaySchedule, month.WorkDays, false, fixture, input, shifted, ref powHeating, ref powCooling, ref withering, ref coolingInputs);
            AccumulateDay(input.SaturdaySchedule, fixture.SaturdaySchedule, month.Saturdays, true, fixture, input, shifted, ref powHeating, ref powCooling, ref withering, ref coolingInputs);
            AccumulateDay(input.SundaySchedule, fixture.SundaySchedule, month.Sundays, true, fixture, input, shifted, ref powHeating, ref powCooling, ref withering, ref coolingInputs);

            return new CoolingVentMonth(powHeating, powCooling, withering, coolingInputs);
        }

        private static void AccumulateDay(
            EecalcDailySchedule ventilationSchedule,
            EecalcDailySchedule coolingSchedule,
            int dayCount,
            bool inclusiveCoolingScheduleEnd,
            EecalcValidationFixture fixture,
            EECalcVentilationInput input,
            IReadOnlyList<HourlyClimateData> shifted,
            ref double powHeating,
            ref double powCooling,
            ref double withering,
            ref double coolingInputs)
        {
            if (dayCount <= 0)
            {
                return;
            }

            for (var hour = ventilationSchedule.StartHour; hour < ventilationSchedule.EndHour && hour < 24; hour++)
            {
                var weather = shifted[hour];
                var outdoor = CalcRo(weather.Temperature, weather.Humidity) * CalcEntalpiaCooling(weather.Temperature, weather.Humidity);
                var flow = CalcRo(input.FlowTemperature, input.FlowRelativeHumidity) * CalcEntalpiaCooling(input.FlowTemperature, input.FlowRelativeHumidity);
                var delta = input.Debit * (outdoor - flow);
                if (delta < 0.0)
                {
                    powHeating += Math.Abs(delta) / 3600.0 * dayCount;
                }
                else
                {
                    powCooling += delta / 3600.0 * dayCount;
                }

                var witheringDelta = input.Debit
                    * (CalcRoW(weather.Temperature) * CalcWitheringEntalpy(weather.Temperature, weather.Humidity)
                       - CalcRoW(input.FlowTemperature) * CalcWitheringEntalpy(input.FlowTemperature, input.FlowRelativeHumidity));
                withering += witheringDelta / 3600.0 * dayCount;

                var selectedTemp = IsCoolingProjectHour(hour, coolingSchedule, inclusiveCoolingScheduleEnd)
                    ? fixture.ProjectTemperature
                    : fixture.NonProjectTemperature;
                coolingInputs += input.Debit * AirHeatCapacityFactor * (selectedTemp - input.FlowTemperature) / 1000.0 * dayCount;
            }
        }

        private static bool IsCoolingProjectHour(
            int hour,
            EecalcDailySchedule schedule,
            bool inclusiveEnd)
        {
            return hour >= schedule.StartHour
                && (inclusiveEnd ? hour <= schedule.EndHour : hour < schedule.EndHour);
        }

        private static double MonthHours(EECalcVentilationInput input, EecalcMonthlyDaysOracleRow month)
        {
            var hours = month.WorkDays * Duration(input.WorkdaySchedule);
            if (month.Sundays > 0)
            {
                hours += month.Sundays * Duration(input.SundaySchedule);
            }

            if (month.Saturdays > 0)
            {
                hours += month.Saturdays * Duration(input.SaturdaySchedule);
            }

            return hours;
        }

        private static double AverageVentHeatTemp(
            EecalcValidationFixture fixture,
            EECalcVentilationInput input,
            EecalcMonthlyDaysOracleRow month)
        {
            var project = 0.0;
            var nonProject = 0.0;
            AccumulateVentTemp(input.WorkdaySchedule, month.WorkDays, fixture, ref project, ref nonProject);
            AccumulateVentTemp(input.SaturdaySchedule, month.Saturdays, fixture, ref project, ref nonProject);
            AccumulateVentTemp(input.SundaySchedule, month.Sundays, fixture, ref project, ref nonProject);
            var total = project + nonProject;
            return total == 0.0
                ? fixture.ProjectTemperature
                : (project * fixture.ProjectTemperature + nonProject * fixture.NonProjectTemperature) / total;
        }

        private static void AccumulateVentTemp(
            EecalcDailySchedule schedule,
            int dayCount,
            EecalcValidationFixture fixture,
            ref double project,
            ref double nonProject)
        {
            for (var hour = schedule.StartHour; hour < schedule.EndHour && hour < 24; hour++)
            {
                if (fixture.WorkdaySchedule.StartHour <= hour && fixture.WorkdaySchedule.EndHour > hour)
                {
                    project += dayCount;
                }
                else
                {
                    nonProject += dayCount;
                }
            }
        }

        private static IReadOnlyList<HourlyClimateData> ShiftedHours(IReadOnlyList<HourlyClimateData> hourly)
        {
            var result = new List<HourlyClimateData> { hourly[Math.Min(23, hourly.Count - 1)] };
            result.AddRange(hourly);
            return result;
        }

        private static int Duration(EecalcDailySchedule schedule)
        {
            return schedule.EndHour - schedule.StartHour;
        }

        private static double CalcEntalpia(double temp, double humidity, double pb)
        {
            var kelvin = 273.15 + temp;
            var saturationPressure = Math.Exp(77.345 + 0.0057 * kelvin - 7235.0 / kelvin) / Math.Pow(kelvin, 8.2);
            var partialPressure = humidity * saturationPressure / 100.0;
            var x = 0.62198 * partialPressure / (pb - partialPressure);
            return 1.006 * temp + x * (1.805 * temp + 2501.0);
        }

        private static double CalcEntalpiaCooling(double temp, double humidity)
        {
            var x = CalcAirX(temp, humidity);
            return 1.006 * temp + x * (2501.0 + 1.805 * temp);
        }

        private static double CalcWitheringEntalpy(double temp, double humidity)
        {
            var x = CalcAirX(temp, humidity);
            return x * (2501.0 + 1.805 * temp);
        }

        private static double CalcAirX(double temp, double humidity)
        {
            var kelvin = 273.15 + temp;
            var saturationPressure = Math.Exp(77.345 + 0.0057 * kelvin - 7235.0 / kelvin) / Math.Pow(kelvin, 8.2);
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

        private static Month ToMonth(int oneBasedMonth)
        {
            return (Month)(oneBasedMonth - 1);
        }

        private sealed record CoolingVentMonth(double PowHeating, double PowCooling, double WitheringEnergy, double CoolingInputs);
    }

    public sealed class EECalcDhwBgvOracle
    {
        private const double LatitudeRadians = 0.7382742735936013;

        private readonly EecalcMonthlyDaysOracle monthlyDaysOracle = new();
        private readonly LegacyEecalcXmlClimateDataProvider climate =
            new(ClimateProviderMode.LegacyEECalcStrict);
        private readonly LegacyEecalcXmlSunEnergyDataProvider sun =
            new();

        public EECalcDhwBgvOracleResult Calculate(EecalcValidationFixture fixture, EECalcDhwBgvInput input)
        {
            var months = monthlyDaysOracle.Calculate(fixture);
            var totalArea = fixture.HeatedArea;
            var mixedWater = input.Consumption * totalArea / 1000.0;
            var resulNetEnergy = 1.161 * input.TempDifference * 0.98 * input.Consumption / 1000.0;
            var resultEnergyForHeating = Math.Max(0.0, resulNetEnergy - input.SunEnergy);
            var source1 = EECalcMath.DivideByEfficiency(
                resultEnergyForHeating * input.Part1 / 100.0,
                EECalcMath.EfficiencyProduct(input.Efficiency1));
            var source2 = EECalcMath.DivideByEfficiency(
                resultEnergyForHeating * input.Part2 / 100.0,
                EECalcMath.EfficiencyProduct(input.Efficiency2));
            var pumpEnergy = input.HotWaterPumpPower * input.HotWaterPumpWorkSchedule
                * months.Sum(month => month.Weeks) / 1000.0;
            var solarRows = CalculateSolarRows(fixture, input, months);
            var bgvSunEnergy = Math.Round(solarRows.Sum(row => row.UsedSunEnergy), 1);
            var bgvPumpsTotal = Math.Round(solarRows.Sum(row => row.BGVPumps) * totalArea / 1000.0, 1);
            var totalUsedSunEnergy = totalArea == 0.0 ? 0.0 : solarRows.Sum(row => row.UsedSunEnergy) / totalArea;

            return new EECalcDhwBgvOracleResult
            {
                MixedWater = mixedWater,
                ResulNetEnergy = resulNetEnergy,
                ResultEnergyForHeating = resultEnergyForHeating,
                ResultSourceEnergy = source1,
                ResultSourceEnergy2 = source2,
                ResultNeededEnergy = source1 + source2,
                HeatEfficiencyGenerating = WeightedGeneratorEfficiency(source1, source2, input),
                HotWaterPumpsNeededEnergy = pumpEnergy,
                SolarRows = solarRows,
                BGVSunEnergy = bgvSunEnergy,
                BGVPumpsTotal = bgvPumpsTotal,
                TotalUsedSunEnergy = EECalcMath.CleanFinite(totalUsedSunEnergy)
            };
        }

        private IReadOnlyList<EECalcSolarDhwMonthRow> CalculateSolarRows(
            EecalcValidationFixture fixture,
            EECalcDhwBgvInput input,
            IReadOnlyList<EecalcMonthlyDaysOracleRow> months)
        {
            var rows = new List<EECalcSolarDhwMonthRow>();
            foreach (var month in months.Where(month => IsActiveSolarMonth(input, month.Month)))
            {
                var collectorsArea = input.AbsorbingSurface * input.CollectorsCount;
                var qHotWater = input.SolarWaterUsage
                    * (input.SolarHotWaterTemperature - input.SolarColdWaterTemperature)
                    * 1.163 / 1000.0
                    * (input.SolarDaysInWeek * month.Weeks);
                var qHotWaterTotal = input.SolarWaterUsage
                    * (input.SolarHotWaterTemperature - input.SolarColdWaterTemperature)
                    * 1.163 / 1000.0
                    * month.TotalDays;
                var radiation = sun.GetMonthlyRadiation(fixture.ClimateZoneId, ToMonth(month.Month));
                var cloudiness = sun.GetMonthlyCloudiness(fixture.ClimateZoneId, ToMonth(month.Month));
                var ht = ProjectionCoefficient(input, month.Month, cloudiness) * radiation;
                var x = ParameterX(fixture, input, month, collectorsArea, qHotWaterTotal);
                var y = ParameterY(input, month, collectorsArea, ht, qHotWaterTotal);
                var correctedX = CorrectedX(input, collectorsArea, x);
                var f = 1.029 * y - 0.065 * correctedX - 0.245 * Math.Pow(y, 2.0)
                    + 0.0018 * Math.Pow(correctedX, 2.0) + 0.0215 * Math.Pow(y, 3.0);
                var qSunWater = month.TotalDays == 0 ? 0.0 : f * qHotWaterTotal / month.TotalDays * (input.SolarDaysInWeek * month.Weeks);
                var fm = qHotWater == 0.0 ? 0.0 : Math.Min(qSunWater / qHotWater * 100.0, 100.0);
                var used = qHotWater * fm / 100.0
                    * (input.SerpentineEfficiencyIsUsed ? input.SerpentineEfficiency : 100.0) / 100.0;

                rows.Add(new EECalcSolarDhwMonthRow
                {
                    Month = month.Month,
                    CollectorsArea = collectorsArea,
                    Ht = ht,
                    X = x,
                    Y = y,
                    CorrectedX = correctedX,
                    F = f,
                    QhotWater = qHotWater,
                    QsunWater = qSunWater,
                    Fm = fm,
                    UsedSunEnergy = EECalcMath.CleanFinite(used),
                    BGVPumps = input.SolarDaysInWeek * month.Weeks * 8.0 * input.PumpsVolume
                });
            }

            return rows;
        }

        private double ParameterX(
            EecalcValidationFixture fixture,
            EECalcDhwBgvInput input,
            EecalcMonthlyDaysOracleRow month,
            double collectorsArea,
            double needed)
        {
            if (needed == 0.0)
            {
                return 0.0;
            }

            var avgTemp = climate.GetMonthlyAvgTemp(fixture.ClimateZoneId, ToMonth(month.Month));
            var deltaT = 100.0 - avgTemp;
            var seconds = month.TotalDays * 24.0 * 60.0 * 60.0;
            var convertedDemand = needed * 1000.0 / 1.163 * 4187.0;
            return EECalcMath.CleanFinite(
                input.FR * ToaEffect(input, collectorsArea) * deltaT * seconds * (collectorsArea / convertedDemand));
        }

        private static double ParameterY(EECalcDhwBgvInput input, EecalcMonthlyDaysOracleRow month, double collectorsArea, double ht, double needed)
        {
            if (needed == 0.0)
            {
                return 0.0;
            }

            var coverFactor = input.TrasparentCoverings == 1 ? 0.95 : 0.93;
            if (input.TrasparentCoverings == 2 && month.Month is 6 or 7 or 8)
            {
                coverFactor = 0.9;
            }

            return EECalcMath.CleanFinite(
                input.FRta * ToaEffect(input, collectorsArea) * coverFactor * ht * month.TotalDays * (collectorsArea / needed));
        }

        private static double ToaEffect(EECalcDhwBgvInput input, double collectorsArea)
        {
            if (input.Scheme1Selected || input.Scheme2Selected)
            {
                return 1.0;
            }

            var collectorCapacity = input.CollectorDebit * input.SpecialHeatCapacity;
            var mtoaEfficiency = input.MTOAEfficiency / 100.0;
            var mtoaCapacity = input.MTOADebit * input.MTOASpecialHeatCapacity;
            var minCapacity = Math.Min(collectorCapacity, mtoaCapacity);
            if (collectorCapacity == 0.0 || mtoaEfficiency == 0.0 || minCapacity == 0.0)
            {
                return 0.0;
            }

            return EECalcMath.CleanFinite(Math.Pow(
                1.0 + collectorsArea * input.FR / collectorCapacity
                * (collectorCapacity / (mtoaEfficiency * minCapacity) - 1.0),
                -1.0));
        }

        private static double CorrectedX(EECalcDhwBgvInput input, double collectorsArea, double x)
        {
            if (collectorsArea == 0.0)
            {
                return x;
            }

            var ratio = input.AcumulatorVolume / collectorsArea;
            return ratio > 37.5 && ratio < 300.0
                ? Math.Pow(ratio / 75.0, -0.25) * x
                : x;
        }

        private static double ProjectionCoefficient(EECalcDhwBgvInput input, int month, double cloudiness)
        {
            var diffuse = 1.39 - 4.03 * cloudiness + 5.53 * Math.Pow(cloudiness, 2.0) - 3.11 * Math.Pow(cloudiness, 3.0);
            var rb = MonthlyHorizontalRadiation(input.Pitch, month);
            var pitchRadians = input.Pitch * Math.PI / 180.0;
            return EECalcMath.CleanFinite(
                (1.0 - diffuse) * rb
                + diffuse * ((1.0 + Math.Cos(pitchRadians)) / 2.0)
                + input.ImpactEnvironment * ((1.0 - Math.Cos(pitchRadians)) / 2.0));
        }

        private static int RepresentativeDay(int month)
        {
            return month switch
            {
                1 => 21,
                2 => 52,
                3 => 80,
                4 => 111,
                5 => 141,
                6 => 172,
                7 => 202,
                8 => 233,
                9 => 264,
                10 => 294,
                11 => 325,
                12 => 355,
                _ => 21
            };
        }

        private static double MonthlyHorizontalRadiation(double pitch, int month)
        {
            var latitudeTilt = 42.3 - pitch;
            var declination = SunDeclination(month);
            var sunset = SunsetHour(month);
            var tiltedSunset = Math.Min(
                sunset,
                Math.Acos(-Math.Tan(latitudeTilt * Math.PI / 180.0) * Math.Tan(declination * Math.PI / 180.0)) * 180.0 / Math.PI);
            var numerator =
                Math.Cos(latitudeTilt * Math.PI / 180.0)
                * Math.Cos(declination * Math.PI / 180.0)
                * Math.Sin(tiltedSunset * Math.PI / 180.0)
                + Math.PI / 180.0
                * tiltedSunset
                * Math.Sin(latitudeTilt * Math.PI / 180.0)
                * Math.Sin(declination * Math.PI / 180.0);
            var denominator =
                Math.Cos(LatitudeRadians)
                * Math.Cos(declination * Math.PI / 180.0)
                * Math.Sin(sunset * Math.PI / 180.0)
                + Math.PI / 180.0
                * sunset
                * Math.Sin(LatitudeRadians)
                * Math.Sin(declination * Math.PI / 180.0);

            return denominator == 0.0 ? 0.0 : numerator / denominator;
        }

        private static double SunDeclination(int month)
        {
            return 23.45 * Math.Sin(Math.PI * 2.0 * (284.0 + RepresentativeDay(month)) / 365.0);
        }

        private static double SunsetHour(int month)
        {
            var declination = SunDeclination(month);
            return Math.Acos(-Math.Tan(LatitudeRadians) * Math.Tan(declination * Math.PI / 180.0)) * 180.0 / Math.PI;
        }

        private static bool IsActiveSolarMonth(EECalcDhwBgvInput input, int month)
        {
            if (input.SolarStartMonth <= input.SolarEndMonth)
            {
                return month >= input.SolarStartMonth && month <= input.SolarEndMonth;
            }

            return month >= input.SolarStartMonth || month <= input.SolarEndMonth;
        }

        private static double WeightedGeneratorEfficiency(double source1, double source2, EECalcDhwBgvInput input)
        {
            var total = source1 + source2;
            return EECalcMath.CleanFinite(total == 0.0
                ? 0.0
                : (source1 * input.Efficiency1.GeneratorHeatEfficiency + source2 * input.Efficiency2.GeneratorHeatEfficiency) / total);
        }

        private static Month ToMonth(int oneBasedMonth)
        {
            return (Month)(oneBasedMonth - 1);
        }
    }

    public sealed class EECalcLightingDevicesOracle
    {
        private readonly EecalcMonthlyDaysOracle monthlyDaysOracle = new();

        public EECalcLightingDevicesOracleResult Calculate(
            EecalcValidationFixture fixture,
            EECalcLightingDevicesInput input,
            IReadOnlyList<EecalcMonthlyHeatingOracleRow>? heatingRows)
        {
            var heatingMonths = monthlyDaysOracle.Calculate(fixture);
            var generalMonths = new EecalcMonthlyDaysOracle().Calculate(new EecalcValidationFixture
            {
                Id = fixture.Id,
                ClimateZoneId = fixture.ClimateZoneId,
                FirstMonth = 1,
                LastMonth = 12,
                FirstDay = 1,
                LastDay = 31,
                HolidaysByMonth = fixture.HolidaysByMonth
            });

            var lights = CalculateGroup("Lights", input.Lights, heatingMonths, generalMonths, heatingRows, includeThermal: true);
            var balanced = CalculateGroup("BalancedDevices", input.BalancedDevices, heatingMonths, generalMonths, heatingRows, includeThermal: true);
            var nonBalanced = CalculateGroup("NonBalancedDevices", input.NonBalancedDevices, heatingMonths, generalMonths, heatingRows, includeThermal: false);
            var hotWaterPumps = CalculateGroup("HotWaterPumps", input.HotWaterPumps, heatingMonths, generalMonths, heatingRows, includeThermal: false);

            return new EECalcLightingDevicesOracleResult
            {
                GroupRows = lights.GroupRows.Concat(balanced.GroupRows).Concat(nonBalanced.GroupRows).Concat(hotWaterPumps.GroupRows).ToList(),
                LightsGeneralNeededEnergy = lights.GeneralEnergy,
                BalancedDevicesGeneralNeededEnergy = balanced.GeneralEnergy,
                NonBalancedDevicesGeneralNeededEnergy = nonBalanced.GeneralEnergy,
                HotWaterPumpsGeneralNeededEnergy = hotWaterPumps.GeneralEnergy,
                ResulLightInputs = lights.HeatingInput,
                ResulAppliancesInputs = balanced.HeatingInput,
                CoolingQintContribution = lights.CoolingEnergy + balanced.CoolingEnergy
            };
        }

        private static GroupCalculation CalculateGroup(
            string group,
            EECalcEquipmentInput input,
            IReadOnlyList<EecalcMonthlyDaysOracleRow> heatingMonths,
            IReadOnlyList<EecalcMonthlyDaysOracleRow> generalMonths,
            IReadOnlyList<EecalcMonthlyHeatingOracleRow>? heatingRows,
            bool includeThermal)
        {
            var rows = new List<EECalcLightingDevicesRow>();
            var heatingEnergy = PeriodEnergy(input, Period.Heating, heatingMonths, rows, group);
            var coolingEnergy = PeriodEnergy(input, Period.Cooling, heatingMonths, rows, group);
            var generalEnergy = PeriodEnergy(input, Period.General, generalMonths, rows, group);
            var heatingInput = 0.0;

            if (includeThermal && heatingRows != null)
            {
                heatingInput = heatingRows.Sum(row =>
                {
                    var month = heatingMonths.FirstOrDefault(item => item.Month == row.Month);
                    return month == null ? 0.0 : MonthEnergy(input, Period.Heating, month, out _) * row.Ni;
                });
            }

            return new GroupCalculation(rows, generalEnergy, heatingInput, includeThermal ? coolingEnergy : 0.0);
        }

        private static double PeriodEnergy(
            EECalcEquipmentInput input,
            Period period,
            IReadOnlyList<EecalcMonthlyDaysOracleRow> months,
            List<EECalcLightingDevicesRow> rows,
            string group)
        {
            if (!input.ByMonths)
            {
                var power = Power(input, period);
                var schedule = WorkSchedule(input, period);
                var energy = power * schedule * months.Sum(month => month.Weeks) / 1000.0;
                rows.Add(new EECalcLightingDevicesRow
                {
                    Group = group,
                    Period = period.ToString(),
                    ByMonths = false,
                    DevicesNeededEnergy = energy,
                    FuelInputEnum = EECalcFuel.Fuel1,
                    FuelReportBucket = EECalcLegacyAggregation.MapFuelReportBucket(EECalcFuel.Fuel1)
                });
                return energy;
            }

            var total = 0.0;
            foreach (var month in months)
            {
                var monthEnergy = MonthEnergy(input, period, month, out var weekRegime);
                total += monthEnergy;
                rows.Add(new EECalcLightingDevicesRow
                {
                    Group = group,
                    Period = period.ToString(),
                    ByMonths = true,
                    Month = month.Month,
                    Weeks = month.Weeks,
                    WeekRegime = weekRegime,
                    DevicesNeededEnergy = monthEnergy,
                    FuelInputEnum = EECalcFuel.Fuel1,
                    FuelReportBucket = EECalcLegacyAggregation.MapFuelReportBucket(EECalcFuel.Fuel1)
                });
            }

            return total;
        }

        private static double MonthEnergy(
            EECalcEquipmentInput input,
            Period period,
            EecalcMonthlyDaysOracleRow month,
            out double weekRegime)
        {
            if (!input.ByMonths || !input.MonthlySchedules.TryGetValue(month.Month, out var schedule))
            {
                weekRegime = WorkSchedule(input, period);
                return Power(input, period) * WorkSchedule(input, period) * month.Weeks / 1000.0;
            }

            weekRegime = schedule.WorkDays * 5.0 + schedule.Saturdays + schedule.Sundays;
            var avgPower = weekRegime == 0.0
                ? 0.0
                : (schedule.WorkDays * schedule.WorkDaysUsedEnergy * 5.0
                   + schedule.Saturdays * schedule.SaturdaysUsedEnergy
                   + schedule.Sundays * schedule.SundaysUsedEnergy) / weekRegime;
            return EECalcMath.CleanFinite(avgPower * weekRegime * month.Weeks / 1000.0);
        }

        private static double Power(EECalcEquipmentInput input, Period period)
        {
            return period switch
            {
                Period.Heating => input.HeatingPower,
                Period.Cooling => input.CoolingPower,
                _ => input.GeneralPower
            };
        }

        private static double WorkSchedule(EECalcEquipmentInput input, Period period)
        {
            return period switch
            {
                Period.Heating => input.HeatingWorkSchedule,
                Period.Cooling => input.CoolingWorkSchedule,
                _ => input.GeneralWorkSchedule
            };
        }

        private sealed record GroupCalculation(
            IReadOnlyList<EECalcLightingDevicesRow> GroupRows,
            double GeneralEnergy,
            double HeatingInput,
            double CoolingEnergy);

        private enum Period
        {
            Heating,
            Cooling,
            General
        }
    }

    public sealed class EECalcCoolingFansAndPumpsOracle
    {
        private readonly EecalcMonthlyDaysOracle monthlyDaysOracle = new();

        public EECalcCoolingFansAndPumpsResult Calculate(
            EecalcValidationFixture coolingFixture,
            EECalcCoolingFansAndPumpsInput input,
            EECalcVentilationInput coolingVentilationInput)
        {
            var months = monthlyDaysOracle.Calculate(coolingFixture);
            var coolingWeeks = months.Sum(month => month.Weeks);
            var weeklyCoolingVentilationHours = WeeklyHours(
                coolingVentilationInput.WorkdaySchedule,
                coolingVentilationInput.SaturdaySchedule,
                coolingVentilationInput.SundaySchedule);
            var weeklyCoolingSeasonHours = WeeklyHours(
                coolingFixture.WorkdaySchedule,
                coolingFixture.SaturdaySchedule,
                coolingFixture.SundaySchedule);
            var neededEnergy = EECalcMath.CleanFinite(
                (input.VentilatorsCool
                 + input.PumpVentilationCool
                 + input.VentilatorsOutdoorAirCool)
                * weeklyCoolingVentilationHours
                * coolingWeeks
                / 1000.0
                / (input.EnergyManagement / 100.0)
                + input.CoolingPump
                * 24.0
                * 7.0
                * coolingWeeks
                / 1000.0
                / (input.EnergyManagement / 100.0));
            var otherNeededEnergy = EECalcMath.CleanFinite(
                input.OtherCoolingVentilation
                * weeklyCoolingVentilationHours
                * coolingWeeks
                / 1000.0
                + input.OtherCooling
                * weeklyCoolingSeasonHours
                * coolingWeeks
                / 1000.0);

            return new EECalcCoolingFansAndPumpsResult
            {
                CoolingWeeks = coolingWeeks,
                WeeklyCoolingVentilationHours = weeklyCoolingVentilationHours,
                WeeklyCoolingSeasonHours = weeklyCoolingSeasonHours,
                NeededEnergy = neededEnergy,
                OtherNeededEnergy = otherNeededEnergy
            };
        }

        private static double Duration(EecalcDailySchedule schedule)
        {
            return schedule.EndHour > schedule.StartHour
                ? schedule.EndHour - schedule.StartHour
                : 0.0;
        }

        private static double WeeklyHours(
            EecalcDailySchedule workday,
            EecalcDailySchedule saturday,
            EecalcDailySchedule sunday)
        {
            return 5.0 * Duration(workday) + Duration(saturday) + Duration(sunday);
        }
    }

    public sealed class EECalcHeatingFansAndPumpsOracle
    {
        private readonly EecalcMonthlyDaysOracle monthlyDaysOracle = new();

        public EECalcHeatingFansAndPumpsResult Calculate(
            EecalcValidationFixture heatingFixture,
            EECalcHeatingFansAndPumpsInput input,
            EECalcVentilationInput heatingVentilationInput)
        {
            var months = monthlyDaysOracle.Calculate(heatingFixture);
            var heatingWeeks = months.Sum(month => month.Weeks);
            var weeklyVentilationHours = WeeklyHours(
                heatingVentilationInput.WorkdaySchedule,
                heatingVentilationInput.SaturdaySchedule,
                heatingVentilationInput.SundaySchedule);
            var weeklyHeatingSeasonHours = WeeklyHours(
                heatingFixture.WorkdaySchedule,
                heatingFixture.SaturdaySchedule,
                heatingFixture.SundaySchedule);
            var efficiency = input.EnergyManagement / 100.0;
            var neededEnergy = EECalcMath.CleanFinite(
                (input.VentilatorsHeat + input.PumpVentilationHeat)
                * weeklyVentilationHours
                * heatingWeeks
                / 1000.0
                / efficiency
                + input.HeatingPump
                * 24.0
                * 7.0
                * heatingWeeks
                / 1000.0
                / efficiency);
            var otherNeededEnergy = EECalcMath.CleanFinite(
                input.OtherHeatingVentilation
                * weeklyVentilationHours
                * heatingWeeks
                / 1000.0
                + input.OtherHeating
                * weeklyHeatingSeasonHours
                * heatingWeeks
                / 1000.0);

            return new EECalcHeatingFansAndPumpsResult
            {
                HeatingWeeks = heatingWeeks,
                WeeklyVentilationHours = weeklyVentilationHours,
                WeeklyHeatingSeasonHours = weeklyHeatingSeasonHours,
                NeededEnergy = neededEnergy,
                OtherNeededEnergy = otherNeededEnergy
            };
        }

        private static double Duration(EecalcDailySchedule schedule)
        {
            return schedule.EndHour > schedule.StartHour
                ? schedule.EndHour - schedule.StartHour
                : 0.0;
        }

        private static double WeeklyHours(
            EecalcDailySchedule workday,
            EecalcDailySchedule saturday,
            EecalcDailySchedule sunday)
        {
            return 5.0 * Duration(workday) + Duration(saturday) + Duration(sunday);
        }
    }

    public sealed class EECalcVentilationOracleResult
    {
        public IReadOnlyList<EECalcVentilationMonthRow> Rows { get; init; } = Array.Empty<EECalcVentilationMonthRow>();
        public double ResultEnergyForHeating { get; init; }
        public double ResultEnergyForCooling { get; init; }
        public double ResultEnergyForWithering { get; init; }
        public double HeatingNeededEnergy { get; init; }
        public double CoolingNeededEnergy { get; init; }
        public double ResultSourceEnergy { get; init; }
        public double ResultSourceEnergy2 { get; init; }
        public double ResultNeededEnergy { get; init; }
        public double ResulVentilationInputs { get; init; }
    }

    public sealed class EECalcVentilationMonthRow
    {
        public int Month { get; init; }
        public double MonthHours { get; init; }
        public double AverageVentHeatTemp { get; init; }
        public double FirstRecoveryTemp { get; init; }
        public double PostRecoveryTemp { get; init; }
        public double ThermoPumpEnergy { get; init; }
        public double MonthlyHeat { get; init; }
        public double HeatingInputs { get; init; }
        public double PowHeating { get; init; }
        public double PowCooling { get; init; }
        public double WitheringEnergy { get; init; }
        public double CoolingInputs { get; init; }
    }

    public sealed class EECalcDhwBgvOracleResult
    {
        public double MixedWater { get; init; }
        public double ResulNetEnergy { get; init; }
        public double ResultEnergyForHeating { get; init; }
        public double ResultSourceEnergy { get; init; }
        public double ResultSourceEnergy2 { get; init; }
        public double ResultNeededEnergy { get; init; }
        public double HeatEfficiencyGenerating { get; init; }
        public double HotWaterPumpsNeededEnergy { get; init; }
        public IReadOnlyList<EECalcSolarDhwMonthRow> SolarRows { get; init; } = Array.Empty<EECalcSolarDhwMonthRow>();
        public double BGVSunEnergy { get; init; }
        public double BGVPumpsTotal { get; init; }
        public double TotalUsedSunEnergy { get; init; }
    }

    public sealed class EECalcSolarDhwMonthRow
    {
        public int Month { get; init; }
        public double CollectorsArea { get; init; }
        public double Ht { get; init; }
        public double X { get; init; }
        public double Y { get; init; }
        public double CorrectedX { get; init; }
        public double F { get; init; }
        public double QhotWater { get; init; }
        public double QsunWater { get; init; }
        public double Fm { get; init; }
        public double UsedSunEnergy { get; init; }
        public double BGVPumps { get; init; }
    }

    public sealed class EECalcLightingDevicesOracleResult
    {
        public IReadOnlyList<EECalcLightingDevicesRow> GroupRows { get; init; } = Array.Empty<EECalcLightingDevicesRow>();
        public double LightsGeneralNeededEnergy { get; init; }
        public double BalancedDevicesGeneralNeededEnergy { get; init; }
        public double NonBalancedDevicesGeneralNeededEnergy { get; init; }
        public double HotWaterPumpsGeneralNeededEnergy { get; init; }
        public double ResulLightInputs { get; init; }
        public double ResulAppliancesInputs { get; init; }
        public double CoolingQintContribution { get; init; }
    }

    public sealed class EECalcLightingDevicesRow
    {
        public string Group { get; init; } = string.Empty;
        public string Period { get; init; } = string.Empty;
        public bool ByMonths { get; init; }
        public int Month { get; init; }
        public double Weeks { get; init; }
        public double WeekRegime { get; init; }
        public double DevicesNeededEnergy { get; init; }
        public EECalcFuel FuelInputEnum { get; init; }
        public EECalcFuel FuelReportBucket { get; init; }
    }
}
