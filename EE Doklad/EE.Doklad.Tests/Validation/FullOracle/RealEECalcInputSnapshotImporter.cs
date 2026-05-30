using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using EE.Doklad.Services.EecalcClimate;

namespace EE.Doklad.Tests.Validation.FullOracle
{
    public sealed class RealEECalcInputSnapshot
    {
        public EecalcEnvelopeFixture Fixture { get; init; } = new();

        public EECalcFullOracleInput Input { get; init; } = new();
    }

    public sealed class RealEECalcInputSnapshotImporter
    {
        public RealEECalcInputSnapshot Load(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            if (!File.Exists(path))
            {
                throw Missing(path);
            }

            using var document = JsonDocument.Parse(File.ReadAllText(path));
            var root = document.RootElement;
            var fixtureId = RequiredString(root, "fixtureId", "fixtureId");
            var mode = RequiredString(root, "mode", "mode");
            if (!string.Equals(mode, EECalcOracleMode.LegacyEECalcStrict.ToString(), StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Missing fixture input: mode");
            }

            var general = RequiredObject(root, "general", "general");
            var season = RequiredObject(root, "season", "season");
            var envelope = RequiredObject(root, "envelope", "envelope");
            var holidaysByMonth = RequiredMonthIntMap(RequiredObject(root, "holidaysByMonth", "holidaysByMonth"), "holidaysByMonth");
            var flags = RequiredObject(root, "flags", "flags");
            var heating = RequiredObject(root, "heating", "heating");
            var cooling = RequiredObject(root, "cooling", "cooling");
            var ventilation = RequiredObject(root, "ventilation", "ventilation");
            var dhwBgv = RequiredObject(root, "dhwBgv", "dhwBgv");
            var schedules = RequiredObject(root, "schedules", "schedules");
            var lightingDevices = RequiredObject(root, "lightingDevices", "lightingDevices");

            RequireGeneralInputs(general);
            RequireSeasonInputs(season);
            RequireEnvelopeInputs(envelope);

            var hasHeating = RequiredBool(flags, "hasHeating", "flags.hasHeating");
            var hasCooling = RequiredBool(flags, "hasCooling", "flags.hasCooling");
            var isBgvUsed = RequiredBool(flags, "isBgvUsed", "flags.isBgvUsed");
            var hasMechanicalVentilation = RequiredBool(flags, "hasMechanicalVentilation", "flags.hasMechanicalVentilation");
            var hasLighting = RequiredBool(flags, "hasLighting", "flags.hasLighting");
            var hasDevices = RequiredBool(flags, "hasDevices", "flags.hasDevices");
            RequiredBool(cooling, "enabled", "cooling.enabled");
            RequiredBool(ventilation, "enabled", "ventilation.enabled");
            RequiredBool(dhwBgv, "enabled", "dhwBgv.enabled");
            RequiredBool(lightingDevices, "lightsEnabled", "lightingDevices.lightsEnabled");
            RequiredBool(lightingDevices, "balancedDevicesEnabled", "lightingDevices.balancedDevicesEnabled");
            RequiredBool(lightingDevices, "nonBalancedDevicesEnabled", "lightingDevices.nonBalancedDevicesEnabled");
            var hasHotWaterPumps = RequiredBool(lightingDevices, "hotWaterPumpsEnabled", "lightingDevices.hotWaterPumpsEnabled");

            if (hasHeating)
            {
                RequireHeatingInputs(heating);
            }
            if (hasCooling)
            {
                RequireCoolingInputs(cooling);
            }
            if (hasMechanicalVentilation)
            {
                RequireVentilationInputs(ventilation);
            }
            if (isBgvUsed)
            {
                RequireDhwBgvInputs(dhwBgv);
            }

            var climateZoneId = RequiredInt(general, "climateZoneId", "general.climateZoneId");
            var climate = new LegacyEecalcXmlClimateDataProvider(ClimateProviderMode.LegacyEECalcStrict);
            var calculation = new EecalcValidationFixture
            {
                Id = fixtureId,
                Scenario = "Actual",
                ClimateZoneId = climateZoneId,
                FirstMonth = RequiredInt(season, "heatingStartMonth", "season.heatingStartMonth"),
                LastMonth = RequiredInt(season, "heatingEndMonth", "season.heatingEndMonth"),
                FirstDay = RequiredInt(season, "heatingStartDay", "season.heatingStartDay"),
                LastDay = RequiredInt(season, "heatingEndDay", "season.heatingEndDay"),
                HeatedArea = RequiredDouble(general, "heatedArea", "general.heatedArea"),
                HeatedVolume = RequiredDouble(general, "conditionedVolume", "general.conditionedVolume"),
                Infiltration = RequiredDouble(heating, "infiltration", "heating.infiltration"),
                HeatCapacity = RequiredDouble(general, "heatCapacityWhPerM2K", "general.heatCapacityWhPerM2K"),
                MetabolicHeat = RequiredDouble(general, "metabolicHeatWPerM2", "general.metabolicHeatWPerM2"),
                LatentMetabolicHeat = RequiredDouble(general, "latentMetabolicHeatWPerM2", "general.latentMetabolicHeatWPerM2"),
                ProjectTemperature = RequiredDouble(heating, "projectTemperature", "heating.projectTemperature"),
                NonProjectTemperature = RequiredDouble(heating, "nonProjectTemperature", "heating.nonProjectTemperature"),
                ProjectHumidity = hasCooling
                    ? RequiredDouble(RequiredObject(root, "cooling", "cooling"), "relativeHumidity", "cooling.relativeHumidity")
                    : 50.0,
                FlowTemperature = hasMechanicalVentilation
                    ? RequiredDouble(RequiredObject(root, "ventilation", "ventilation"), "flowTemperature", "ventilation.flowTemperature")
                    : 0.0,
                FlowRelativeHumidity = hasMechanicalVentilation
                    ? RequiredDouble(RequiredObject(root, "ventilation", "ventilation"), "flowRelativeHumidity", "ventilation.flowRelativeHumidity")
                    : 0.0,
                VentilationDebit = hasMechanicalVentilation
                    ? RequiredDouble(RequiredObject(root, "ventilation", "ventilation"), "debit", "ventilation.debit")
                    : 0.0,
                LightsCoolingPower = hasLighting
                    ? RequiredEquipment(RequiredObject(root, "lightingDevices", "lightingDevices"), "lights", "lightingDevices.lights").CoolingPower
                    : 0.0,
                BalancedDevicesCoolingPower = hasDevices
                    ? RequiredEquipment(RequiredObject(root, "lightingDevices", "lightingDevices"), "balancedDevices", "lightingDevices.balancedDevices").CoolingPower
                    : 0.0,
                LightsCoolingWorkSchedule = hasLighting
                    ? RequiredEquipment(RequiredObject(root, "lightingDevices", "lightingDevices"), "lights", "lightingDevices.lights").CoolingWorkSchedule
                    : 0.0,
                BalancedDevicesCoolingWorkSchedule = hasDevices
                    ? RequiredEquipment(RequiredObject(root, "lightingDevices", "lightingDevices"), "balancedDevices", "lightingDevices.balancedDevices").CoolingWorkSchedule
                    : 0.0,
                WorkdaySchedule = RequiredSchedule(schedules, "workday", "schedules.workday"),
                SaturdaySchedule = RequiredSchedule(schedules, "saturday", "schedules.saturday"),
                SundaySchedule = RequiredSchedule(schedules, "sunday", "schedules.sunday"),
                OccupantsWorkdaySchedule = RequiredSchedule(schedules, "occupantsWorkday", "schedules.occupantsWorkday"),
                OccupantsSaturdaySchedule = RequiredSchedule(schedules, "occupantsSaturday", "schedules.occupantsSaturday"),
                OccupantsSundaySchedule = RequiredSchedule(schedules, "occupantsSunday", "schedules.occupantsSunday"),
                VentilationWorkdaySchedule = RequiredSchedule(schedules, "ventilationWorkday", "schedules.ventilationWorkday"),
                VentilationSaturdaySchedule = RequiredSchedule(schedules, "ventilationSaturday", "schedules.ventilationSaturday"),
                VentilationSundaySchedule = RequiredSchedule(schedules, "ventilationSunday", "schedules.ventilationSunday"),
                NightVentilationWorkdaySchedule = RequiredSchedule(schedules, "nightVentilationWorkday", "schedules.nightVentilationWorkday"),
                NightVentilationSaturdaySchedule = RequiredSchedule(schedules, "nightVentilationSaturday", "schedules.nightVentilationSaturday"),
                NightVentilationSundaySchedule = RequiredSchedule(schedules, "nightVentilationSunday", "schedules.nightVentilationSunday"),
                HolidaysByMonth = holidaysByMonth,
                AverageOutdoorTemperatureByMonth = ClimateAverages(climate, climateZoneId),
                SolarRadiationByMonth = SolarRadiation(climate, climateZoneId),
                HourlyWeatherByMonth = HourlyWeather(climate, climateZoneId)
            };

            var fixture = new EecalcEnvelopeFixture
            {
                Id = fixtureId,
                Calculation = calculation,
                SouthWalls =
                {
                    AccumulateOuterA = RequiredDouble(envelope, "wallsAreaActual", "envelope.wallsAreaActual"),
                    AccumulateWindowA = RequiredDouble(envelope, "windowsAreaActual", "envelope.windowsAreaActual")
                },
                Roof =
                {
                    AccumulateNonTransparentA = RequiredDouble(envelope, "roofAreaActual", "envelope.roofAreaActual")
                },
                Floor =
                {
                    AccumulateFloorA = RequiredDouble(envelope, "floorAreaActual", "envelope.floorAreaActual")
                }
            };

            var input = new EECalcFullOracleInput
            {
                Ventilation = hasMechanicalVentilation
                    ? LoadVentilation(root)
                    : new EECalcVentilationInput(),
                DhwBgv = isBgvUsed
                    ? LoadDhwBgv(root)
                    : new EECalcDhwBgvInput(),
                LightingDevices = LoadLightingDevices(root, hasLighting, hasDevices, hasHotWaterPumps),
                Aggregation = LoadAggregation(root, hasHeating, hasCooling, isBgvUsed)
            };

            return new RealEECalcInputSnapshot
            {
                Fixture = fixture,
                Input = input
            };
        }

        private static EECalcVentilationInput LoadVentilation(JsonElement root)
        {
            var ventilation = RequiredObject(root, "ventilation", "ventilation");
            return new EECalcVentilationInput
            {
                Debit = RequiredDouble(ventilation, "debit", "ventilation.debit"),
                FlowTemperature = RequiredDouble(ventilation, "flowTemperature", "ventilation.flowTemperature"),
                FlowRelativeHumidity = RequiredDouble(ventilation, "flowRelativeHumidity", "ventilation.flowRelativeHumidity"),
                FirstRecEfficiency = RequiredDouble(ventilation, "firstRecEfficiency", "ventilation.firstRecEfficiency"),
                SecondRecEfficiency = RequiredDouble(ventilation, "secondRecEfficiency", "ventilation.secondRecEfficiency"),
                HeatingAirDifference = RequiredDouble(ventilation, "heatingAirDifference", "ventilation.heatingAirDifference")
            };
        }

        private static EECalcDhwBgvInput LoadDhwBgv(JsonElement root)
        {
            var dhw = RequiredObject(root, "dhwBgv", "dhwBgv");
            return new EECalcDhwBgvInput
            {
                Consumption = RequiredDouble(dhw, "consumption", "dhwBgv.consumption"),
                TempDifference = RequiredDouble(dhw, "tempDifference", "dhwBgv.tempDifference"),
                SunEnergy = RequiredDouble(dhw, "sunEnergy", "dhwBgv.sunEnergy"),
                Part1 = RequiredDouble(dhw, "part1", "dhwBgv.part1"),
                Part2 = RequiredDouble(dhw, "part2", "dhwBgv.part2"),
                Efficiency1 = RequiredDhwEfficiency(dhw, "efficiency1", "dhwBgv.efficiency1"),
                Efficiency2 = RequiredDhwEfficiency(dhw, "efficiency2", "dhwBgv.efficiency2")
            };
        }

        private static EECalcLightingDevicesInput LoadLightingDevices(
            JsonElement root,
            bool hasLighting,
            bool hasDevices,
            bool hasHotWaterPumps)
        {
            var lighting = RequiredObject(root, "lightingDevices", "lightingDevices");
            return new EECalcLightingDevicesInput
            {
                Lights = hasLighting
                    ? RequiredEquipment(lighting, "lights", "lightingDevices.lights")
                    : new EECalcEquipmentInput(),
                BalancedDevices = hasDevices
                    ? RequiredEquipment(lighting, "balancedDevices", "lightingDevices.balancedDevices")
                    : new EECalcEquipmentInput(),
                NonBalancedDevices = hasDevices
                    ? RequiredEquipment(lighting, "nonBalancedDevices", "lightingDevices.nonBalancedDevices")
                    : new EECalcEquipmentInput(),
                HotWaterPumps = hasHotWaterPumps
                    ? RequiredEquipment(lighting, "hotWaterPumps", "lightingDevices.hotWaterPumps")
                    : new EECalcEquipmentInput()
            };
        }

        private static EECalcAggregationInput LoadAggregation(
            JsonElement root,
            bool hasHeating,
            bool hasCooling,
            bool isBgvUsed)
        {
            var aggregation = RequiredObject(root, "aggregation", "aggregation");
            return new EECalcAggregationInput
            {
                HasHeating = hasHeating,
                HasCooling = hasCooling,
                IsBgvUsed = isBgvUsed,
                FansAndPumps = RequiredDouble(aggregation, "fansAndPumps", "aggregation.fansAndPumps"),
                Other = RequiredDouble(aggregation, "other", "aggregation.other")
            };
        }

        private static void RequireHeatingInputs(JsonElement heating)
        {
            RequiredDouble(heating, "infiltration", "heating.infiltration");
            RequiredDouble(heating, "projectTemperature", "heating.projectTemperature");
            RequiredDouble(heating, "nonProjectTemperature", "heating.nonProjectTemperature");
            RequiredString(heating, "fuel1", "heating.fuel1");
            RequiredString(heating, "fuel2", "heating.fuel2");
            RequiredDouble(heating, "part1", "heating.part1");
            RequiredDouble(heating, "part2", "heating.part2");
            RequiredDouble(heating, "efficiency1", "heating.efficiency1");
            RequiredDouble(heating, "efficiency2", "heating.efficiency2");
        }

        private static void RequireCoolingInputs(JsonElement cooling)
        {
            RequiredDouble(cooling, "projectTemperature", "cooling.projectTemperature");
            RequiredDouble(cooling, "nonProjectTemperature", "cooling.nonProjectTemperature");
            RequiredDouble(cooling, "relativeHumidity", "cooling.relativeHumidity");
        }

        private static void RequireVentilationInputs(JsonElement ventilation)
        {
            RequiredDouble(ventilation, "debit", "ventilation.debit");
            RequiredDouble(ventilation, "flowTemperature", "ventilation.flowTemperature");
            RequiredDouble(ventilation, "flowRelativeHumidity", "ventilation.flowRelativeHumidity");
            RequiredDouble(ventilation, "firstRecEfficiency", "ventilation.firstRecEfficiency");
            RequiredDouble(ventilation, "secondRecEfficiency", "ventilation.secondRecEfficiency");
            RequiredDouble(ventilation, "heatingAirDifference", "ventilation.heatingAirDifference");
        }

        private static void RequireDhwBgvInputs(JsonElement dhwBgv)
        {
            RequiredDouble(dhwBgv, "consumption", "dhwBgv.consumption");
            RequiredDouble(dhwBgv, "tempDifference", "dhwBgv.tempDifference");
            RequiredDouble(dhwBgv, "sunEnergy", "dhwBgv.sunEnergy");
            RequiredDouble(dhwBgv, "part1", "dhwBgv.part1");
            RequiredDouble(dhwBgv, "part2", "dhwBgv.part2");
            RequiredDouble(dhwBgv, "efficiency1", "dhwBgv.efficiency1");
            RequiredDouble(dhwBgv, "efficiency2", "dhwBgv.efficiency2");
        }

        private static void RequireGeneralInputs(JsonElement general)
        {
            RequiredInt(general, "climateZoneId", "general.climateZoneId");
            RequiredDouble(general, "conditionedArea", "general.conditionedArea");
            RequiredDouble(general, "heatedArea", "general.heatedArea");
            RequiredDouble(general, "otherArea", "general.otherArea");
            RequiredDouble(general, "conditionedVolume", "general.conditionedVolume");
            RequiredDouble(general, "otherVolume", "general.otherVolume");
            RequiredDouble(general, "heatCapacityWhPerM2K", "general.heatCapacityWhPerM2K");
            RequiredDouble(general, "metabolicHeatWPerM2", "general.metabolicHeatWPerM2");
            RequiredDouble(general, "latentMetabolicHeatWPerM2", "general.latentMetabolicHeatWPerM2");
        }

        private static void RequireSeasonInputs(JsonElement season)
        {
            RequiredInt(season, "heatingStartDay", "season.heatingStartDay");
            RequiredInt(season, "heatingStartMonth", "season.heatingStartMonth");
            RequiredInt(season, "heatingEndDay", "season.heatingEndDay");
            RequiredInt(season, "heatingEndMonth", "season.heatingEndMonth");
        }

        private static void RequireEnvelopeInputs(JsonElement envelope)
        {
            RequiredDouble(envelope, "wallsAreaActual", "envelope.wallsAreaActual");
            RequiredDouble(envelope, "wallsAreaEsm", "envelope.wallsAreaEsm");
            RequiredDouble(envelope, "windowsAreaActual", "envelope.windowsAreaActual");
            RequiredDouble(envelope, "windowsAreaEsm", "envelope.windowsAreaEsm");
            RequiredDouble(envelope, "roofAreaActual", "envelope.roofAreaActual");
            RequiredDouble(envelope, "roofAreaEsm", "envelope.roofAreaEsm");
            RequiredDouble(envelope, "floorAreaActual", "envelope.floorAreaActual");
            RequiredDouble(envelope, "floorAreaEsm", "envelope.floorAreaEsm");
        }

        private static EECalcEquipmentInput RequiredEquipment(JsonElement parent, string name, string path)
        {
            var equipment = RequiredObject(parent, name, path);
            return new EECalcEquipmentInput
            {
                HeatingPower = RequiredDouble(equipment, "heatingPower", path + ".heatingPower"),
                HeatingWorkSchedule = RequiredDouble(equipment, "heatingWorkSchedule", path + ".heatingWorkSchedule"),
                CoolingPower = RequiredDouble(equipment, "coolingPower", path + ".coolingPower"),
                CoolingWorkSchedule = RequiredDouble(equipment, "coolingWorkSchedule", path + ".coolingWorkSchedule"),
                GeneralPower = RequiredDouble(equipment, "generalPower", path + ".generalPower"),
                GeneralWorkSchedule = RequiredDouble(equipment, "generalWorkSchedule", path + ".generalWorkSchedule")
            };
        }

        private static EECalcDhwEfficiencyChain RequiredDhwEfficiency(JsonElement parent, string name, string path)
        {
            var value = RequiredDouble(parent, name, path);
            return new EECalcDhwEfficiencyChain
            {
                SupplyNetEfficiency = value,
                Automatic = 100.0,
                EnergyManagement = 100.0,
                GeneratorHeatEfficiency = 100.0
            };
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

        private static EecalcDailySchedule RequiredSchedule(JsonElement parent, string name, string path)
        {
            var schedule = RequiredObject(parent, name, path);
            return new EecalcDailySchedule
            {
                StartHour = RequiredInt(schedule, "startHour", path + ".startHour"),
                EndHour = RequiredInt(schedule, "endHour", path + ".endHour")
            };
        }

        private static Dictionary<int, int> RequiredMonthIntMap(JsonElement parent, string path)
        {
            var result = new Dictionary<int, int>();
            for (var month = 1; month <= 12; month++)
            {
                result[month] = RequiredInt(parent, month.ToString(CultureInfo.InvariantCulture), path + "." + month.ToString(CultureInfo.InvariantCulture));
            }

            return result;
        }

        private static JsonElement RequiredObject(JsonElement parent, string name, string path)
        {
            var element = RequiredProperty(parent, name, path);
            return element.ValueKind == JsonValueKind.Object ? element : throw Missing(path);
        }

        private static string RequiredString(JsonElement parent, string name, string path)
        {
            var element = RequiredProperty(parent, name, path);
            return element.ValueKind == JsonValueKind.String ? element.GetString() ?? throw Missing(path) : throw Missing(path);
        }

        private static bool RequiredBool(JsonElement parent, string name, string path)
        {
            var element = RequiredProperty(parent, name, path);
            return element.ValueKind switch
            {
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => throw Missing(path)
            };
        }

        private static int RequiredInt(JsonElement parent, string name, string path)
        {
            var element = RequiredProperty(parent, name, path);
            return element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out var value) ? value : throw Missing(path);
        }

        private static double RequiredDouble(JsonElement parent, string name, string path)
        {
            var element = RequiredProperty(parent, name, path);
            return element.ValueKind == JsonValueKind.Number ? element.GetDouble() : throw Missing(path);
        }

        private static JsonElement RequiredProperty(JsonElement parent, string name, string path)
        {
            return parent.TryGetProperty(name, out var element) && element.ValueKind != JsonValueKind.Null
                ? element
                : throw Missing(path);
        }

        private static InvalidOperationException Missing(string path)
        {
            return new InvalidOperationException("Missing fixture input: " + path);
        }
    }
}
