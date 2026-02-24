using System;
using System.Collections.Generic;
using System.Linq;

namespace EE.Doklad.Models.Climate
{
    /// <summary>
    /// Провайдър на климатични данни от EPW файл (8760 часови записа за цяла година).
    /// Данните се подават в конструктора като масив - няма external file dependencies.
    /// </summary>
    public class EpwHourlyClimateProvider : IClimateDataProvider
    {
        private readonly ClimatePoint[] _hourlyData;
        private readonly Dictionary<DateTime, int> _dateTimeIndex;
        
        public ClimateDataSource Source => ClimateDataSource.ASHRAE_EPW;
        
        public string OriginalFileName { get; }
        public string? City { get; }
        public double? Latitude { get; }
        public double? Longitude { get; }
        public int FixedYearUsed { get; }

        /// <summary>
        /// Конструктор - приема 8760 часови записа за цялата година.
        /// </summary>
        /// <param name="hourlyData">Масив с точно 8760 климатични точки (365 дни × 24 часа)</param>
        /// <param name="originalFileName">Име на оригиналния EPW файл (само за информация)</param>
        /// <param name="city">Град (от EPW header)</param>
        /// <param name="latitude">Географска ширина</param>
        /// <param name="longitude">Географска дължина</param>
        /// <param name="fixedYearUsed">Фиксирана година използвана за DateTime (референтна, напр. 2024)</param>
        public EpwHourlyClimateProvider(
            ClimatePoint[] hourlyData,
            string originalFileName,
            string? city = null,
            double? latitude = null,
            double? longitude = null,
            int fixedYearUsed = 2024)
        {
            if (hourlyData == null || hourlyData.Length != 8760)
                throw new ArgumentException("EPW данни трябва да съдържат точно 8760 часови записа (365 дни × 24 часа).", nameof(hourlyData));

            if (string.IsNullOrWhiteSpace(originalFileName))
                throw new ArgumentNullException(nameof(originalFileName));

            _hourlyData = hourlyData;
            OriginalFileName = originalFileName;
            City = city;
            Latitude = latitude;
            Longitude = longitude;
            FixedYearUsed = fixedYearUsed;

            // Построяваме индекс за бърз достъп по DateTime
            _dateTimeIndex = new Dictionary<DateTime, int>(8760);
            for (int i = 0; i < _hourlyData.Length; i++)
            {
                var point = _hourlyData[i];
                var key = new DateTime(fixedYearUsed, point.Month, point.LocalTime.Day, point.Hour, 0, 0, DateTimeKind.Unspecified);
                _dateTimeIndex[key] = i;
            }

            // Валидация: проверяваме че покрива цялата година
            ValidateCoverage();
        }

        private void ValidateCoverage()
        {
            var expectedStart = new DateTime(FixedYearUsed, 1, 1, 0, 0, 0);
            var expectedEnd = new DateTime(FixedYearUsed, 12, 31, 23, 0, 0);

            var actualStart = _hourlyData.Min(p => p.LocalTime);
            var actualEnd = _hourlyData.Max(p => p.LocalTime);

            if (actualStart.Month != 1 || actualStart.Day != 1 || actualStart.Hour != 0)
                throw new InvalidOperationException($"EPW данни не започват от 01.01 00:00 (начало: {actualStart}).");

            if (actualEnd.Month != 12 || actualEnd.Day != 31 || actualEnd.Hour != 23)
                throw new InvalidOperationException($"EPW данни не завършват на 31.12 23:00 (край: {actualEnd}).");
        }

        public ClimatePoint GetPoint(DateTime localDateTime)
        {
            // Нормализираме към fixed year
            var key = new DateTime(
                FixedYearUsed,
                localDateTime.Month,
                localDateTime.Day,
                localDateTime.Hour,
                0, 0, DateTimeKind.Unspecified);

            if (_dateTimeIndex.TryGetValue(key, out var index))
            {
                var point = _hourlyData[index];
                // Връщаме point с актуалната дата (не fixed year)
                return new ClimatePoint(localDateTime, point.DryBulbC, point.RH);
            }

            // Ако липсва точен match (не би трябвало), търсим най-близката точка
            return FindNearestPoint(key, localDateTime);
        }

        private ClimatePoint FindNearestPoint(DateTime key, DateTime originalDateTime)
        {
            // Намираме най-близката точка по разлика в часове
            int targetDayOfYear = key.DayOfYear;
            int targetHour = key.Hour;

            var nearest = _hourlyData
                .Select((p, idx) => new { Point = p, Index = idx })
                .OrderBy(x =>
                {
                    int dayDiff = Math.Abs(x.Point.LocalTime.DayOfYear - targetDayOfYear);
                    int hourDiff = Math.Abs(x.Point.Hour - targetHour);
                    return dayDiff * 24 + hourDiff;
                })
                .First();

            // Връщаме с оригиналната дата
            return new ClimatePoint(originalDateTime, nearest.Point.DryBulbC, nearest.Point.RH);
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

        /// <summary>
        /// Връща всички 8760 точки в хронологичен ред.
        /// </summary>
        public IReadOnlyList<ClimatePoint> GetAllPoints() => _hourlyData;
    }
}
