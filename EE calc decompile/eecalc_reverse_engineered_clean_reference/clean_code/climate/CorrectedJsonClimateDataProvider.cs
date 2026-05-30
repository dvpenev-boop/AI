using System;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.Services.EecalcClimate
{
    public sealed class CorrectedJsonClimateDataProvider : IClimateDataProvider
    {
        private readonly Dictionary<int, ClimateZoneData> _zonesById;

        public CorrectedJsonClimateDataProvider()
            : this(new JsonClimateRepository())
        {
        }

        public CorrectedJsonClimateDataProvider(IClimateRepository repository)
        {
            ArgumentNullException.ThrowIfNull(repository);
            _zonesById = repository.LoadSeed().Zones.ToDictionary(zone => zone.Id);
        }

        public double GetMonthlyAvgTemp(int zoneId, Month month)
        {
            return GetZone(zoneId).Monthly.AvgMonthlyTempC[ToMonthIndex(month)];
        }

        public SolarRadiationData GetSolarRadiation(int zoneId, Month month)
        {
            var zone = GetZone(zoneId);
            var index = ToMonthIndex(month);
            return new SolarRadiationData(
                GetSolar(zone, "N", index),
                GetSolar(zone, "E", index),
                GetSolar(zone, "S", index),
                GetSolar(zone, "W", index),
                GetSolar(zone, "H", index));
        }

        public IReadOnlyList<HourlyClimateData> GetHourlyClimateData(int zoneId, Month month)
        {
            // Temporary limitation: climate_zones.json does not contain an hourly
            // temperature/humidity profile. CurrentOrdinance therefore exposes a
            // 24-hour monthly fallback until an authoritative hourly source exists.
            var zone = GetZone(zoneId);
            var monthIndex = ToMonthIndex(month);
            var temperature = zone.Monthly.AvgMonthlyTempC[monthIndex];
            var humidity = GetMonthlyHumidityFallback(zone, monthIndex);

            return Enumerable
                .Range(0, 24)
                .Select(hour => new HourlyClimateData(hour, temperature, humidity))
                .ToArray();
        }

        public double GetPb(int zoneId)
        {
            return GetZone(zoneId).GetEffectiveBarometricPressure();
        }

        private ClimateZoneData GetZone(int zoneId)
        {
            if (!_zonesById.TryGetValue(zoneId, out var zone))
            {
                throw new ArgumentOutOfRangeException(nameof(zoneId), zoneId, "No climate_zones.json zone with this Id.");
            }

            return zone;
        }

        private static double GetSolar(ClimateZoneData zone, string orientation, int monthIndex)
        {
            if (!zone.Monthly.AvgFullSolarVerticalWm2.TryGetValue(orientation, out var values))
            {
                throw new InvalidOperationException($"Zone {zone.Id} is missing solar orientation '{orientation}'.");
            }

            return values[monthIndex];
        }

        private static double GetMonthlyHumidityFallback(ClimateZoneData zone, int monthIndex)
        {
            if (monthIndex >= 4 && monthIndex <= 8)
            {
                var rhIndex = monthIndex - 4;
                var values = zone.Monthly.AvgMonthlyRelHumidityPercentMayToSep;
                if (values.Length > rhIndex && values[rhIndex] > 0)
                {
                    return values[rhIndex];
                }
            }

            return 50.0;
        }

        private static int ToMonthIndex(Month month)
        {
            var value = (int)month;
            if (value < 0 || value > 11)
            {
                throw new ArgumentOutOfRangeException(nameof(month), month, "Month must be January..December.");
            }

            return value;
        }
    }
}
