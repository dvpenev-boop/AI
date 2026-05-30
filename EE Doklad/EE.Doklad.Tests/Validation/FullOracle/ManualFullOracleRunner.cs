using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using EE.Doklad.Services.EecalcClimate;

namespace EE.Doklad.Tests.Validation.FullOracle
{
    public sealed class ManualEecalcInput
    {
        // --- General Project Data ---
        public int ClimateZoneId { get; init; }
        public double HeatedArea { get; init; }
        public double HeatedVolume { get; init; }
        public double HeatCapacity { get; init; }
        public double MetabolicHeat { get; init; }
        public double LatentMetabolicHeat { get; init; }
        public int HeatingStartDay { get; init; }
        public int HeatingStartMonth { get; init; }
        public int HeatingEndDay { get; init; }
        public int HeatingEndMonth { get; init; }
        public double Infiltration { get; init; }
        public double ProjectTemperature { get; init; }
        public double NonProjectTemperature { get; init; }

        // --- Building Envelope ---
        public List<ManualWall> Walls { get; init; } = new();
        public List<ManualWindow> Windows { get; init; } = new();
        public List<ManualRoof> Roofs { get; init; } = new();

        // --- System Efficiencies ---
        public double HeatingGeneratorEfficiency { get; init; } = 100.0;
        public double HeatingDistributionEfficiency { get; init; } = 100.0;

        // --- System Toggles ---
        public bool HasCooling { get; init; }
        public bool HasMechanicalVentilation { get; init; }
        public bool IsBgvUsed { get; init; }
        public bool HasLighting { get; init; }
        public bool HasDevices { get; init; }
    }

    public sealed class ManualWall
    {
        public double Area { get; init; }
        public double U { get; init; }
        public double Epsilon { get; init; } = 0.9; // Standard emissivity
        public double Rsi { get; init; } = 0.13;
        public double Rse { get; init; } = 0.04;
    }

    public sealed class ManualWindow
    {
        public double Area { get; init; }
        public double U { get; init; }
        public double G { get; init; } = 0.75; // Standard solar energy transmittance
        public double Fsh { get; init; } = 1.0; // No shading
    }

    public sealed class ManualRoof
    {
        public double Area { get; init; }
        public double U { get; init; }
        public double Epsilon { get; init; } = 0.9;
        public double Rsi { get; init; } = 0.10;
        public double Rse { get; init; } = 0.04;
    }

    public sealed class ManualHeatingBreakdown
    {
        public double Qtr { get; init; }

        public double Qve { get; init; }

        public double Qht { get; init; }

        public double Qgn { get; init; }

        public double Gamma { get; init; }

        public double Ni { get; init; }

        public double FinalQnd { get; init; }
    }

    public sealed class ManualEecalcResult
    {
        public ManualEecalcInput Input { get; init; } = new();

        public double HeatingNeededEnergy { get; init; }

        public double HeatingNetEnergy { get; init; }

        public double PrimaryEnergyTotal { get; init; }

        public double FuelEnergyTotal { get; init; }

        public double CO2Total { get; init; }

        public double? EnergyClassPointer { get; init; }

        public ManualHeatingBreakdown HeatingBreakdown { get; init; } = new();

        public string ToReadableText()
        {
            var builder = new StringBuilder();
            builder.AppendLine("EECalc Manual Calculation Result");
            builder.AppendLine("Input:");
            builder.AppendLine($"  Area: {Format(Input.HeatedArea)}");
            builder.AppendLine($"  Volume: {Format(Input.HeatedVolume)}");
            builder.AppendLine($"  Climate zone: {Input.ClimateZoneId.ToString(CultureInfo.InvariantCulture)}");
            builder.AppendLine();
            builder.AppendLine("Heating:");
            builder.AppendLine($"  Qtr: {Format(HeatingBreakdown.Qtr)}");
            builder.AppendLine($"  Qve: {Format(HeatingBreakdown.Qve)}");
            builder.AppendLine($"  Qht: {Format(HeatingBreakdown.Qht)}");
            builder.AppendLine($"  Qgn: {Format(HeatingBreakdown.Qgn)}");
            builder.AppendLine($"  Gamma: {Format(HeatingBreakdown.Gamma)}");
            builder.AppendLine($"  Ni: {Format(HeatingBreakdown.Ni)}");
            builder.AppendLine($"  Final Qnd: {Format(HeatingBreakdown.FinalQnd)}");
            builder.AppendLine();
            builder.AppendLine("Final:");
            builder.AppendLine($"  Needed heating: {Format(HeatingNeededEnergy)}");
            builder.AppendLine($"  Primary total: {Format(PrimaryEnergyTotal)}");
            builder.AppendLine($"  Fuel total: {Format(FuelEnergyTotal)}");
            builder.AppendLine($"  CO2 total: {Format(CO2Total)}");
            if (EnergyClassPointer.HasValue)
            {
                builder.AppendLine($"  Energy class pointer: {Format(EnergyClassPointer.Value)}");
            }

            return builder.ToString();
        }

        private static string Format(double value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }
    }

    public static class ManualFullOracleRunner
    {
        public static ManualEecalcResult Calculate(ManualEecalcInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var fixture = CreateFixture(input);
            var fullInput = CreateFullOracleInput(input);
            var fullResult = new EECalcFullOracle().Run(new EECalcOracleContext(fixture, input: fullInput));
            return CreateManualResult(input, fullResult);
        }

        private static EecalcEnvelopeFixture CreateFixture(ManualEecalcInput input)
        {
            var climate = new LegacyEecalcXmlClimateDataProvider(ClimateProviderMode.LegacyEECalcStrict);
            var calculation = new EecalcValidationFixture
            {
                Id = "manual_full_oracle",
                Scenario = "Manual",
                ClimateZoneId = input.ClimateZoneId,
                FirstMonth = input.HeatingStartMonth,
                LastMonth = input.HeatingEndMonth,
                FirstDay = input.HeatingStartDay,
                LastDay = input.HeatingEndDay,
                HeatedArea = input.HeatedArea,
                HeatedVolume = input.HeatedVolume,
                Infiltration = input.Infiltration,
                HeatCapacity = input.HeatCapacity,
                MetabolicHeat = input.MetabolicHeat,
                LatentMetabolicHeat = input.LatentMetabolicHeat,
                ProjectTemperature = input.ProjectTemperature,
                NonProjectTemperature = input.NonProjectTemperature,
                ProjectHumidity = 50.0,
                FlowTemperature = 0.0,
                FlowRelativeHumidity = 50.0,
                VentilationDebit = 0.0,
                WorkdaySchedule = FullDay(),
                SaturdaySchedule = FullDay(),
                SundaySchedule = FullDay(),
                OccupantsWorkdaySchedule = FullDay(),
                OccupantsSaturdaySchedule = FullDay(),
                OccupantsSundaySchedule = FullDay(),
                VentilationWorkdaySchedule = FullDay(),
                VentilationSaturdaySchedule = FullDay(),
                VentilationSundaySchedule = FullDay(),
                NightVentilationWorkdaySchedule = EmptyDay(),
                NightVentilationSaturdaySchedule = EmptyDay(),
                NightVentilationSundaySchedule = EmptyDay(),
                HolidaysByMonth = Enumerable.Range(1, 12).ToDictionary(month => month, _ => 0),
                AverageOutdoorTemperatureByMonth = ClimateAverages(climate, input.ClimateZoneId),
                SolarRadiationByMonth = SolarRadiation(climate, input.ClimateZoneId),
                HourlyWeatherByMonth = HourlyWeather(climate, input.ClimateZoneId)
            };

            return new EecalcEnvelopeFixture
            {
                Id = calculation.Id,
                Calculation = calculation
            };
        }

        private static EECalcFullOracleInput CreateFullOracleInput(ManualEecalcInput input)
        {
            return new EECalcFullOracleInput
            {
                Ventilation = input.HasMechanicalVentilation
                    ? new EECalcVentilationInput()
                    : new EECalcVentilationInput { Debit = 0.0 },
                DhwBgv = input.IsBgvUsed ? new EECalcDhwBgvInput() : new EECalcDhwBgvInput(),
                LightingDevices = new EECalcLightingDevicesInput(),
                Aggregation = new EECalcAggregationInput
                {
                    HasHeating = true,
                    HasCooling = input.HasCooling,
                    IsBgvUsed = input.IsBgvUsed,
                    FansAndPumps = 0.0,
                    Other = 0.0
                }
            };
        }

        private static ManualEecalcResult CreateManualResult(ManualEecalcInput input, EECalcOracleResult fullResult)
        {
            var heatingRows = fullResult.DebugRows
                .Where(row => row.Module == EECalcOracleModule.R5_HeatingBalance.ToString())
                .ToArray();
            var breakdown = new ManualHeatingBreakdown
            {
                Qtr = Sum(heatingRows, "Qtr"),
                Qve = Sum(heatingRows, "Qve"),
                Qht = Sum(heatingRows, "Qht"),
                Qgn = Sum(heatingRows, "Qgn"),
                Gamma = Average(heatingRows, "Gamma"),
                Ni = Average(heatingRows, "Ni"),
                FinalQnd = Sum(heatingRows, "FinalQnd")
            };

            return new ManualEecalcResult
            {
                Input = input,
                HeatingNeededEnergy = Value(fullResult, "NeededEnergyTable.Heating.Actual"),
                HeatingNetEnergy = Value(fullResult, "NetEnergyTable.Heating.Actual"),
                PrimaryEnergyTotal = Value(fullResult, "PrimaryEnergyTable.Total.Actual"),
                FuelEnergyTotal = Value(fullResult, "FuelEnergyTable.Total.Actual"),
                CO2Total = Value(fullResult, "EmissionNeededEnergyTable.Total.Actual"),
                EnergyClassPointer = TryValue(fullResult, "EnergyClassScale.PoiterValue"),
                HeatingBreakdown = breakdown
            };
        }

        private static EecalcDailySchedule FullDay()
        {
            return new EecalcDailySchedule { StartHour = 0, EndHour = 24 };
        }

        private static EecalcDailySchedule EmptyDay()
        {
            return new EecalcDailySchedule { StartHour = 0, EndHour = 0 };
        }

        private static Dictionary<int, double> ClimateAverages(IClimateDataProvider climate, int zoneId)
        {
            return Enumerable.Range(1, 12).ToDictionary(
                month => month,
                month => climate.GetMonthlyAvgTemp(zoneId, ToMonth(month)));
        }

        private static Dictionary<int, EecalcSolarRadiationFixture> SolarRadiation(IClimateDataProvider climate, int zoneId)
        {
            return Enumerable.Range(1, 12).ToDictionary(
                month => month,
                month =>
                {
                    var solar = climate.GetSolarRadiation(zoneId, ToMonth(month));
                    return new EecalcSolarRadiationFixture
                    {
                        N = solar.N,
                        E = solar.E,
                        S = solar.S,
                        W = solar.W,
                        H = solar.H
                    };
                });
        }

        private static Dictionary<int, IReadOnlyList<EecalcHourlyWeatherFixture>> HourlyWeather(IClimateDataProvider climate, int zoneId)
        {
            return Enumerable.Range(1, 12).ToDictionary(
                month => month,
                month => (IReadOnlyList<EecalcHourlyWeatherFixture>)climate.GetHourlyClimateData(zoneId, ToMonth(month))
                    .Select(hour => new EecalcHourlyWeatherFixture
                    {
                        Temperature = hour.Temperature,
                        Humidity = hour.Humidity
                    })
                    .ToList());
        }

        private static Month ToMonth(int month)
        {
            return (Month)(month - 1);
        }

        private static double Value(EECalcOracleResult result, string key)
        {
            return result.FullValues.TryGetValue(key, out var value) ? value : 0.0;
        }

        private static double? TryValue(EECalcOracleResult result, string key)
        {
            return result.FullValues.TryGetValue(key, out var value) ? value : null;
        }

        private static double Sum(IEnumerable<EECalcDebugRow> rows, string key)
        {
            return rows.Sum(row => Field(row, key));
        }

        private static double Average(IReadOnlyCollection<EECalcDebugRow> rows, string key)
        {
            return rows.Count == 0 ? 0.0 : rows.Average(row => Field(row, key));
        }

        private static double Field(EECalcDebugRow row, string key)
        {
            return row.Fields.TryGetValue(key, out var value)
                && double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
                    ? parsed
                    : 0.0;
        }
    }
}
