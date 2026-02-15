using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using EE.Doklad.Models.Climate;

namespace EE.Doklad.Services.Climate
{
    /// <summary>
    /// Парсър за EPW (EnergyPlus Weather) файлове.
    /// Стандарт: 8 header lines + 8760 data records (365 дни × 24 часа).
    /// </summary>
    public class EpwParser
    {
        private const int ExpectedDataRecords = 8760; // 365 дни × 24 часа
        private const int FixedYearNonLeap = 2021; // Non-leap year за консистентност

        /// <summary>
        /// Парсва EPW файл от път.
        /// </summary>
        public EpwParseResult ParseFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new EpwParseResult
                {
                    Success = false,
                    ErrorMessage = $"Файлът не съществува: {filePath}"
                };
            }

            try
            {
                var lines = File.ReadAllLines(filePath);
                return ParseLines(lines, Path.GetFileName(filePath));
            }
            catch (Exception ex)
            {
                return new EpwParseResult
                {
                    Success = false,
                    ErrorMessage = $"Грешка при четене на файл: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Парсва EPW съдържание от масив от редове.
        /// </summary>
        public EpwParseResult ParseLines(string[] lines, string originalFileName)
        {
            var result = new EpwParseResult
            {
                OriginalFileName = originalFileName,
                FixedYearUsed = FixedYearNonLeap
            };

            if (lines == null || lines.Length < 9)
            {
                result.ErrorMessage = "Невалиден EPW файл: твърде малко редове (минимум 9: 8 header + 1 data).";
                return result;
            }

            // Парсваме header (първите 8 реда)
            ParseHeader(lines, result);

            // Парсваме data records (от ред 9 нататък)
            var dataLines = lines.Skip(8).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

            if (dataLines.Length != ExpectedDataRecords)
            {
                result.ErrorMessage = $"Невалиден брой data records: очаквани {ExpectedDataRecords}, намерени {dataLines.Length}.";
                return result;
            }

            var hourlyData = new List<ClimatePoint>(ExpectedDataRecords);

            for (int i = 0; i < dataLines.Length; i++)
            {
                var parseLineResult = ParseDataLine(dataLines[i], i + 1);
                
                if (!parseLineResult.Success)
                {
                    result.ErrorMessage = $"Грешка на ред {i + 9}: {parseLineResult.ErrorMessage}";
                    return result;
                }

                hourlyData.Add(parseLineResult.Point);
            }

            // Валидация: проверяваме че покрива цялата година
            if (!ValidateYearCoverage(hourlyData, out var validationError))
            {
                result.ErrorMessage = validationError;
                return result;
            }

            result.HourlyData = hourlyData.ToArray();
            result.Success = true;
            return result;
        }

        private void ParseHeader(string[] lines, EpwParseResult result)
        {
            try
            {
                // Line 1: LOCATION,city,state,country,source,WMO,latitude,longitude,timezone,elevation
                var locationParts = lines[0].Split(',');
                if (locationParts.Length >= 10 && locationParts[0].Trim().Equals("LOCATION", StringComparison.OrdinalIgnoreCase))
                {
                    result.City = locationParts[1].Trim();
                    result.StateProvince = locationParts[2].Trim();
                    result.Country = locationParts[3].Trim();

                    if (double.TryParse(locationParts[6].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var lat))
                        result.Latitude = lat;

                    if (double.TryParse(locationParts[7].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
                        result.Longitude = lon;

                    if (double.TryParse(locationParts[8].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var tz))
                        result.TimeZone = tz;

                    if (double.TryParse(locationParts[9].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var elev))
                        result.Elevation = elev;
                }
            }
            catch
            {
                // Ако header парсването се провали, продължаваме - не е критично
            }
        }

        private (bool Success, ClimatePoint Point, string? ErrorMessage) ParseDataLine(string line, int lineNumber)
        {
            var parts = line.Split(',');

            // EPW data format (35+ columns):
            // Year,Month,Day,Hour,Minute,DataSource,DryBulb,DewPoint,RelHum,AtmosPressure,...
            // Индекси: 0=Year, 1=Month, 2=Day, 3=Hour, 4=Minute, 6=DryBulb, 7=DewPoint, 8=RelHum

            if (parts.Length < 9)
            {
                return (false, default, $"Недостатъчно колони ({parts.Length} < 9).");
            }

            try
            {
                // Month, Day, Hour
                if (!int.TryParse(parts[1].Trim(), out var month) || month < 1 || month > 12)
                    return (false, default, $"Невалиден месец: {parts[1]}");

                if (!int.TryParse(parts[2].Trim(), out var day) || day < 1 || day > 31)
                    return (false, default, $"Невалиден ден: {parts[2]}");

                if (!int.TryParse(parts[3].Trim(), out var hour) || hour < 1 || hour > 24)
                    return (false, default, $"Невалиден час: {parts[3]}");

                // EPW използва час 1-24, където час N представлява периода от N-1:00 до N:00
                // Час 1 = 00:00-01:00, Час 24 = 23:00-00:00 (следващ ден)
                // За нашите цели използваме началото на часа: час 1 → 0, час 2 → 1, ..., час 24 → 23
                int hourNormalized = hour - 1; // 1-24 → 0-23

                // DryBulb temperature (°C)
                if (!double.TryParse(parts[6].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var dryBulbC))
                    return (false, default, $"Невалидна температура: {parts[6]}");

                // Relative Humidity (%)
                double rh = 50.0; // Default fallback
                if (!double.TryParse(parts[8].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out rh))
                {
                    // Ако RH липсва, опитваме да я изчислим от DewPoint
                    if (double.TryParse(parts[7].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var dewPointC))
                    {
                        rh = CalculateRhFromDewPoint(dryBulbC, dewPointC);
                    }
                    else
                    {
                        // Ако и двете липсват, използваме default 50%
                        rh = 50.0;
                    }
                }

                var localTime = new DateTime(FixedYearNonLeap, month, day, hourNormalized, 0, 0, DateTimeKind.Unspecified);
                var point = new ClimatePoint(localTime, dryBulbC, rh);

                return (true, point, null);
            }
            catch (Exception ex)
            {
                return (false, default, $"Изключение: {ex.Message}");
            }
        }

        /// <summary>
        /// Опростена формула за изчисляване на RH от DryBulb и DewPoint (Magnus-Tetens).
        /// </summary>
        private double CalculateRhFromDewPoint(double dryBulbC, double dewPointC)
        {
            const double a = 17.27;
            const double b = 237.7;

            double alpha = (a * dryBulbC) / (b + dryBulbC) + Math.Log(1.0);
            double beta = (a * dewPointC) / (b + dewPointC) + Math.Log(1.0);

            double rh = 100.0 * Math.Exp(beta - alpha);
            return Math.Clamp(rh, 0.0, 100.0);
        }

        private bool ValidateYearCoverage(List<ClimatePoint> hourlyData, out string? errorMessage)
        {
            errorMessage = null;

            if (hourlyData.Count != ExpectedDataRecords)
            {
                errorMessage = $"Невалиден брой записи: {hourlyData.Count} вместо {ExpectedDataRecords}.";
                return false;
            }

            // Проверяваме че започва от 01.01 00:00
            var first = hourlyData[0];
            if (first.Month != 1 || first.LocalTime.Day != 1 || first.Hour != 0)
            {
                errorMessage = $"Първият запис не е 01.01 00:00 (намерен: {first.LocalTime:MM.dd HH:00}).";
                return false;
            }

            // Проверяваме че завършва на 31.12 23:00
            var last = hourlyData[ExpectedDataRecords - 1];
            if (last.Month != 12 || last.LocalTime.Day != 31 || last.Hour != 23)
            {
                errorMessage = $"Последният запис не е 31.12 23:00 (намерен: {last.LocalTime:MM.dd HH:00}).";
                return false;
            }

            // Проверяваме че записите са в хронологичен ред
            for (int i = 1; i < hourlyData.Count; i++)
            {
                if (hourlyData[i].LocalTime <= hourlyData[i - 1].LocalTime)
                {
                    errorMessage = $"Записите не са в хронологичен ред на позиция {i + 1}.";
                    return false;
                }
            }

            return true;
        }
    }
}
