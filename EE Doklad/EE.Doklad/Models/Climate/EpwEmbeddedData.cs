using System.Collections.Generic;
using System.Linq;

namespace EE.Doklad.Models.Climate
{
    /// <summary>
    /// Вградени EPW климатични данни за конкретен доклад.
    /// Сериализира се заедно с доклада - няма external file dependencies.
    /// </summary>
    public class EpwEmbeddedData
    {
        /// <summary>
        /// Име на оригиналния EPW файл (само за информация).
        /// </summary>
        public string OriginalFileName { get; set; } = string.Empty;

        /// <summary>
        /// Град (от EPW header).
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Държава/регион (от EPW header).
        /// </summary>
        public string? StateProvince { get; set; }

        /// <summary>
        /// Държава (от EPW header).
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Географска ширина (degrees).
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Географска дължина (degrees).
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Часова зона (UTC offset).
        /// </summary>
        public double? TimeZone { get; set; }

        /// <summary>
        /// Надморска височина (meters).
        /// </summary>
        public double? Elevation { get; set; }

        /// <summary>
        /// Фиксирана година използвана за DateTime (non-leap, напр. 2021).
        /// </summary>
        public int FixedYearUsed { get; set; } = 2021;

        /// <summary>
        /// 8760 часови записа (365 дни × 24 часа).
        /// Сериализира се като List за JSON compatibility.
        /// </summary>
        public List<ClimatePointDto> HourlyData { get; set; } = new();

        /// <summary>
        /// Кратък описателен текст за UI.
        /// </summary>
        public string GetDisplayName()
        {
            if (string.IsNullOrEmpty(City))
                return "Непознато местоположение";

            var parts = new List<string> { City };

            if (!string.IsNullOrEmpty(Country))
                parts.Add(Country);

            if (Latitude.HasValue && Longitude.HasValue)
            {
                var latDir = Latitude.Value >= 0 ? "N" : "S";
                var lonDir = Longitude.Value >= 0 ? "E" : "W";
                parts.Add($"({System.Math.Abs(Latitude.Value):F2}°{latDir}, {System.Math.Abs(Longitude.Value):F2}°{lonDir})");
            }

            return string.Join(", ", parts);
        }

        /// <summary>
        /// Конвертира към EpwHourlyClimateProvider за използване в изчисления.
        /// </summary>
        public EpwHourlyClimateProvider ToClimateProvider()
        {
            var points = HourlyData.Select(dto => dto.ToClimatePoint(FixedYearUsed)).ToArray();

            return new EpwHourlyClimateProvider(
                points,
                OriginalFileName,
                City,
                Latitude,
                Longitude,
                FixedYearUsed
            );
        }
    }
}
