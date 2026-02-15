using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace EE.Doklad.Models.Climate
{
    /// <summary>
    /// Провайдър на климатични данни от BG типични дни (12 месеца × 24 часа).
    /// Зарежда данни от DefaultParams_climateZones_hourly_flat_1to9.json.
    /// </summary>
    public class BgTypicalDayClimateProvider : IClimateDataProvider
    {
        private readonly int _zoneNumber;
        private readonly ClimatePoint[,] _typicalDays; // [month 1-12, hour 0-23]
        private readonly int _fixedYear = 2021; // Non-leap year за консистентност

        public ClimateDataSource Source => ClimateDataSource.BG;

        public int ZoneNumber => _zoneNumber;
        public string ZoneName { get; private set; }

        /// <summary>
        /// Конструктор - зарежда данни за дадена климатична зона.
        /// </summary>
        public BgTypicalDayClimateProvider(int zoneNumber)
        {
            if (zoneNumber < 1 || zoneNumber > 9)
                throw new ArgumentOutOfRangeException(nameof(zoneNumber), "Климатична зона трябва да е между 1 и 9.");

            _zoneNumber = zoneNumber;
            _typicalDays = new ClimatePoint[13, 24]; // Index 0 unused, 1-12 for months
            ZoneName = $"Зона {zoneNumber}";

            LoadTypicalDaysFromJson();
        }

        private void LoadTypicalDaysFromJson()
        {
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DefaultParams_climateZones_hourly_flat_1to9.json");
            
            if (!File.Exists(jsonPath))
                throw new FileNotFoundException($"Липсва файл с климатични данни: {jsonPath}");

            var json = File.ReadAllText(jsonPath);
            var allRecords = JsonConvert.DeserializeObject<List<BgClimateHourlyRecord>>(json);

            if (allRecords == null || allRecords.Count == 0)
                throw new InvalidOperationException("Празен JSON файл с климатични данни.");

            // Филтрираме само записите за нашата зона
            var zoneRecords = allRecords.Where(r => r.zone_number == _zoneNumber).ToList();

            if (zoneRecords.Count == 0)
                throw new InvalidOperationException($"Липсват данни за зона {_zoneNumber}.");

            // Попълваме типичните дни
            foreach (var record in zoneRecords)
            {
                if (record.month < 1 || record.month > 12)
                    continue;
                if (record.hour < 0 || record.hour > 23)
                    continue;

                var localTime = new DateTime(_fixedYear, record.month, 15, record.hour, 0, 0, DateTimeKind.Unspecified);
                
                _typicalDays[record.month, record.hour] = new ClimatePoint(
                    localTime,
                    record.temp_C,
                    record.rh_percent
                );

                // Запазваме името на зоната от първия запис
                if (string.IsNullOrEmpty(ZoneName) || ZoneName == $"Зона {_zoneNumber}")
                    ZoneName = record.zone_title ?? $"Зона {_zoneNumber}";
            }

            // Валидация: проверяваме че има данни за всички месеци и часове
            for (int m = 1; m <= 12; m++)
            {
                for (int h = 0; h < 24; h++)
                {
                    if (_typicalDays[m, h].LocalTime == default)
                        throw new InvalidOperationException($"Липсват данни за зона {_zoneNumber}, месец {m}, час {h}.");
                }
            }
        }

        public ClimatePoint GetPoint(DateTime localDateTime)
        {
            int month = localDateTime.Month;
            int hour = localDateTime.Hour;

            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(nameof(localDateTime), $"Месец {month} е извън диапазона 1-12.");

            if (hour < 0 || hour > 23)
                throw new ArgumentOutOfRangeException(nameof(localDateTime), $"Час {hour} е извън диапазона 0-23.");

            // Връщаме типичния ден за този месец/час, но с актуалната дата
            var typical = _typicalDays[month, hour];
            return new ClimatePoint(localDateTime, typical.DryBulbC, typical.RH);
        }

        public IEnumerable<ClimatePoint> GetRange(DateTime from, DateTime to)
        {
            var current = from;
            while (current < to)
            {
                yield return GetPoint(current);
                current = current.AddHours(1);
            }
        }

        // DTO за десериализация от JSON
        private class BgClimateHourlyRecord
        {
            public int zone_number { get; set; }
            public string? zone_title { get; set; }
            public int month { get; set; }
            public int hour { get; set; }
            public double temp_C { get; set; }
            public double rh_percent { get; set; }
        }
    }
}
