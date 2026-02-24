using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EE.Doklad.Models.Climate;

namespace EE.Doklad.Services.Climate
{
    /// <summary>
    /// Парсър за EPW (EnergyPlus Weather) файлове.
    /// Стандарт: 8 header lines + 8760 data records (365 дни × 24 часа).
    ///
    /// Ключови правила:
    ///   1. Строга валидация: точно 8760 data реда (regex: ^\d{4},\d{1,2},\d{1,2},).
    ///   2. Hour нормализация: EPW Hour 1..24 → 0..23 (Hour=1 = интервал 00:00–01:00).
    ///   3. Barometric pressure: колона 9 (0-based) = Station Pressure [Pa].
    ///      - Невалидни (празно, 999999, ≤0, parse fail) → null → imputation.
    ///      - Imputation: prev-wins nearest-neighbor.
    ///      - Поне 1 валидна стойност е задължителна.
    /// </summary>
    public class EpwParser
    {
        private const int ExpectedDataRecords = 8760; // 365 дни × 24 часа
        private const int FixedYear = 2024;           // Референтна година за целия софтуер

        // Regex за идентифициране на data ред: YYYY,M(M),D(D),…
        private static readonly Regex DataLineRegex = new Regex(
            @"^\d{4},\d{1,2},\d{1,2},",
            RegexOptions.Compiled);

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
                FixedYearUsed = FixedYear
            };

            if (lines == null || lines.Length < 9)
            {
                result.ErrorMessage = "Невалиден EPW файл: твърде малко редове (минимум 9: 8 header + 1 data).";
                return result;
            }

            // ── Парсваме header (първите 8 реда) ──────────────────────────────────
            ParseHeader(lines, result);

            // ── Строга валидация: преброй data редовете с regex ────────────────────
            // Идентифицираме data lines по pattern YYYY,M,D, (не по позиция в файла).
            var dataLines = lines
                .Where(l => DataLineRegex.IsMatch(l))
                .ToArray();

            if (dataLines.Length != ExpectedDataRecords)
            {
                string detail;
                if (dataLines.Length == 8784)
                    detail = "Високосна година (8784 записа) не се поддържа.";
                else if (dataLines.Length > ExpectedDataRecords)
                    detail = "Възможни дублирани месеци/периоди.";
                else
                    detail = "Орязан файл.";

                result.ErrorMessage =
                    $"EPW must contain exactly {ExpectedDataRecords} hourly records. " +
                    $"Found: {dataLines.Length}. {detail}";
                return result;
            }

            // ── Парсване на data записите ─────────────────────────────────────────
            var hourlyData = new List<ClimatePoint>(ExpectedDataRecords);
            var rawPressures = new double?[ExpectedDataRecords]; // за imputation

            for (int i = 0; i < dataLines.Length; i++)
            {
                var parseLineResult = ParseDataLine(dataLines[i], i + 1, out double? rawPressure);

                if (!parseLineResult.Success)
                {
                    result.ErrorMessage = $"Грешка на data ред {i + 1}: {parseLineResult.ErrorMessage}";
                    return result;
                }

                hourlyData.Add(parseLineResult.Point);
                rawPressures[i] = rawPressure;
            }

            // ── Валидация: поне 1 валидна pressure стойност ────────────────────────
            if (!rawPressures.Any(p => p.HasValue))
            {
                result.ErrorMessage =
                    "EPW file does not contain any valid hourly barometric pressure values. Upload rejected.";
                return result;
            }

            // ── Imputation на липсващи pressure стойности ──────────────────────────
            ImputePressures(rawPressures);

            // Записваме imputed стойности обратно в точките
            for (int i = 0; i < hourlyData.Count; i++)
            {
                var pt = hourlyData[i];
                hourlyData[i] = new ClimatePoint(
                    pt.LocalTime,
                    pt.DryBulbC,
                    pt.RH,
                    rawPressures[i]!.Value);
            }

            // ── Валидация за покритие на цялата година ─────────────────────────────
            if (!ValidateYearCoverage(hourlyData, out var validationError))
            {
                result.ErrorMessage = validationError;
                return result;
            }

            // ── Sanity check: няма B_Pa ≤ 0 след imputation ──────────────────────
            for (int i = 0; i < hourlyData.Count; i++)
            {
                if (hourlyData[i].BarometricPressurePa <= 0)
                {
                    result.ErrorMessage =
                        $"Вътрешна грешка: запис {i + 1} има налягане ≤ 0 след imputation ({hourlyData[i].BarometricPressurePa:F0} Pa).";
                    return result;
                }
            }

            result.HourlyData = hourlyData.ToArray();
            result.Success = true;
            return result;
        }

        // ── Header ────────────────────────────────────────────────────────────────

        private void ParseHeader(string[] lines, EpwParseResult result)
        {
            try
            {
                // Line 1: LOCATION,city,state,country,source,WMO,latitude,longitude,timezone,elevation
                var locationParts = lines[0].Split(',');
                if (locationParts.Length >= 10 &&
                    locationParts[0].Trim().Equals("LOCATION", StringComparison.OrdinalIgnoreCase))
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

                // COMMENTS 1 (ред, започващ с COMMENTS 1) — пазим като текст, НЕ влияе на логиката.
                var comments1Line = lines.FirstOrDefault(l =>
                    l.TrimStart().StartsWith("COMMENTS 1", StringComparison.OrdinalIgnoreCase));
                if (comments1Line != null)
                    result.Comments1 = comments1Line;
            }
            catch
            {
                // Header parse failure is non-critical – continue.
            }
        }

        // ── Data line parsing ─────────────────────────────────────────────────────

        /// <summary>
        /// Парсва един data ред и връща ClimatePoint + raw pressure (nullable за imputation).
        /// </summary>
        private (bool Success, ClimatePoint Point, string? ErrorMessage) ParseDataLine(
            string line, int lineNumber, out double? rawPressure)
        {
            rawPressure = null;
            var parts = line.Split(',');

            // EPW data format (35+ columns):
            // 0=Year, 1=Month, 2=Day, 3=Hour, 4=Minute,
            // 5=DataSource, 6=DryBulb, 7=DewPoint, 8=RelHum, 9=AtmosPressure, ...
            if (parts.Length < 10)
                return (false, default, $"Недостатъчно колони ({parts.Length} < 10).");

            try
            {
                // Month, Day, Hour (raw)
                if (!int.TryParse(parts[1].Trim(), out var month) || month < 1 || month > 12)
                    return (false, default, $"Невалиден месец: {parts[1]}");

                if (!int.TryParse(parts[2].Trim(), out var day) || day < 1 || day > 31)
                    return (false, default, $"Невалиден ден: {parts[2]}");

                if (!int.TryParse(parts[3].Trim(), out var hourRaw) || hourRaw < 1 || hourRaw > 24)
                    return (false, default, $"Невалиден час: {parts[3]}");

                int minuteRaw = 0;
                if (parts.Length > 4)
                    int.TryParse(parts[4].Trim(), out minuteRaw);

                // ── КРИТИЧНО: EPW Hour нормализация ──────────────────────────────
                // EPW Hour 1..24, Hour=1 = интервал 00:00–01:00
                // hourIndex = hourRaw - 1  → 0..23
                int hourIndex = hourRaw - 1;

                // ── DryBulb temperature (°C) – колона 6 ─────────────────────────
                if (!double.TryParse(parts[6].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var dryBulbC))
                    return (false, default, $"Невалидна температура: {parts[6]}");

                // ── Relative Humidity (%) – колона 8 ─────────────────────────────
                double rh;
                if (!double.TryParse(parts[8].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out rh))
                {
                    // Ако RH липсва, опитваме да я изчислим от DewPoint
                    if (double.TryParse(parts[7].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var dewPointC))
                        rh = CalculateRhFromDewPoint(dryBulbC, dewPointC);
                    else
                        rh = 50.0;
                }

                // ── Barometric Pressure (Pa) – колона 9 (0-based) ────────────────
                //   Правила:
                //   - празно / parse fail / == 999999 / <= 0 → null (ще се impute)
                //   - иначе: rawPressure = parsedValue
                //   НЕ присвоявай 0 като крайна стойност.
                string pressureStr = parts[9].Trim();
                if (!string.IsNullOrEmpty(pressureStr) &&
                    double.TryParse(pressureStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var pParsed))
                {
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (pParsed != 999999.0 && pParsed > 0)
                        rawPressure = pParsed;
                    // else → remains null → will be imputed
                }

                var localTime = new DateTime(
                    FixedYear,
                    month,
                    day,
                    hourIndex,
                    minuteRaw,
                    0,
                    DateTimeKind.Unspecified);

                // Създаваме точка с временна pressure=0 (ще се презапише след imputation)
                var point = new ClimatePoint(localTime, dryBulbC, rh, rawPressure ?? 0.0);

                return (true, point, null);
            }
            catch (Exception ex)
            {
                return (false, default, $"Изключение: {ex.Message}");
            }
        }

        // ── Barometric Pressure Imputation ────────────────────────────────────────

        /// <summary>
        /// Nearest-neighbor imputation, prev-wins:
        ///   За всеки час i с pressure == null:
        ///   1) Търси назад (j = i-1, i-2, …) → ако намери → използвай.
        ///   2) Ако няма назад → търси напред (k = i+1, i+2, …).
        ///   Гарантирано: поне 1 валидна стойност съществува (валидирано преди извикването).
        ///
        /// Детерминистично правило: при равна дистанция prev wins.
        /// Няма future-leak проблем – файлът е статичен.
        /// </summary>
        private static void ImputePressures(double?[] pressures)
        {
            int n = pressures.Length;
            for (int i = 0; i < n; i++)
            {
                if (pressures[i].HasValue) continue;

                // Търси назад
                double? found = null;
                for (int j = i - 1; j >= 0; j--)
                {
                    if (pressures[j].HasValue)
                    {
                        found = pressures[j]!.Value;
                        break;
                    }
                }

                if (found == null)
                {
                    // Търси напред
                    for (int k = i + 1; k < n; k++)
                    {
                        if (pressures[k].HasValue)
                        {
                            found = pressures[k]!.Value;
                            break;
                        }
                    }
                }

                pressures[i] = found!.Value;
            }
        }

        // ── RH from DewPoint ──────────────────────────────────────────────────────

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

        // ── Year coverage validation ──────────────────────────────────────────────

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

            // Месечни бройки: 672..744 (28*24=672, 31*24=744)
            for (int m = 1; m <= 12; m++)
            {
                int count = hourlyData.Count(p => p.Month == m);
                if (count < 672 || count > 744)
                {
                    errorMessage = $"Месец {m} има {count} записа (очаквано 672..744).";
                    return false;
                }
            }

            return true;
        }
    }
}
