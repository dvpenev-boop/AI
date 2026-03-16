using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Models.Climate;

namespace EE.Doklad.Services.Climate
{
    /// <summary>
    /// Резултат от парсване на EPW файл.
    /// </summary>
    public class EpwParseResult
    {
        /// <summary>
        /// Успешно ли е парсването?
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Съобщение за грешка (ако !Success).
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Име на оригиналния файл.
        /// </summary>
        public string? OriginalFileName { get; set; }

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
        /// COMMENTS 1 ред от EPW header (типични месеци и пр.). Само текст, НЕ влияе на логиката.
        /// </summary>
        public string? Comments1 { get; set; }

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
        /// Фиксирана година използвана за DateTime (референтна, напр. 2026).
        /// </summary>
        public int FixedYearUsed { get; set; }

        /// <summary>
        /// Масив от 8760 климатични точки (365 дни × 24 часа).
        /// </summary>
        public ClimatePoint[]? HourlyData { get; set; }

        /// <summary>
        /// Кратък описателен текст за UI (напр. "Sofia, Bulgaria (42.65°N, 23.38°E)").
        /// </summary>
        public string GetDisplayName()
        {
            if (!Success || string.IsNullOrEmpty(City))
                return "Непознато местоположение";

            var parts = new List<string> { City };
            
            if (!string.IsNullOrEmpty(Country))
                parts.Add(Country);

            if (Latitude.HasValue && Longitude.HasValue)
            {
                var latDir = Latitude.Value >= 0 ? "N" : "S";
                var lonDir = Longitude.Value >= 0 ? "E" : "W";
                parts.Add($"({Math.Abs(Latitude.Value):F2}°{latDir}, {Math.Abs(Longitude.Value):F2}°{lonDir})");
            }

            return string.Join(", ", parts);
        }

        /// <summary>
        /// Конвертира към EpwEmbeddedData за вграждане в Report.
        /// </summary>
        public EpwEmbeddedData ToEmbeddedData()
        {
            if (!Success || HourlyData == null)
                throw new InvalidOperationException("Не може да се конвертира неуспешен parse резултат.");

            return new EpwEmbeddedData
            {
                OriginalFileName = OriginalFileName ?? "unknown.epw",
                City = City,
                StateProvince = StateProvince,
                Country = Country,
                Latitude = Latitude,
                Longitude = Longitude,
                TimeZone = TimeZone,
                Elevation = Elevation,
                FixedYearUsed = FixedYearUsed,
                HourlyData = HourlyData.Select(p => new ClimatePointDto(p)).ToList()
            };
        }
    }
}
