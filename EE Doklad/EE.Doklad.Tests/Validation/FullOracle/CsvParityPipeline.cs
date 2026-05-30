using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace EE.Doklad.Tests.Validation.FullOracle
{
    public sealed class RealEECalcCsvExporter
    {
        public IReadOnlyList<OracleCsvRecord> Load(string path)
        {
            return OracleCsvRecord.Load(path, sourceName: "RealEECalc");
        }
    }

    public sealed class DecompiledOracleCsvExporter
    {
        public IReadOnlyList<OracleCsvRecord> Export(EECalcOracleResult result, string debugDirectory)
        {
            ArgumentNullException.ThrowIfNull(result);
            result.ExportDebugCsv(debugDirectory);
            return Load(Path.Combine(debugDirectory, "full_result.csv"));
        }

        public IReadOnlyList<OracleCsvRecord> Load(string path)
        {
            return OracleCsvRecord.Load(path, sourceName: "DecompiledOracle");
        }
    }

    public sealed class CsvParityComparer
    {
        public CsvParityComparison Compare(
            IReadOnlyList<OracleCsvRecord> realRows,
            IReadOnlyList<OracleCsvRecord> oracleRows,
            double absoluteTolerance = 0.0,
            double relativeTolerance = 0.0)
        {
            ArgumentNullException.ThrowIfNull(realRows);
            ArgumentNullException.ThrowIfNull(oracleRows);

            var realByKey = realRows.ToDictionary(row => row.Key, StringComparer.Ordinal);
            var oracleByKey = oracleRows.ToDictionary(row => row.Key, StringComparer.Ordinal);
            var keys = realByKey.Keys.Concat(oracleByKey.Keys).Distinct(StringComparer.Ordinal).OrderBy(key => key, StringComparer.Ordinal);
            var comparisons = new List<CsvParityComparisonRow>();

            foreach (var key in keys)
            {
                realByKey.TryGetValue(key, out var real);
                oracleByKey.TryGetValue(key, out var oracle);
                var realValue = real?.Value;
                var oracleValue = oracle?.Value;
                var absoluteDelta = CalculateAbsoluteDelta(realValue, oracleValue);
                var relativeDelta = CalculateRelativeDelta(realValue, oracleValue, absoluteDelta);
                var exact = string.Equals(real?.RawValue, oracle?.RawValue, StringComparison.Ordinal);
                var withinTolerance = absoluteDelta <= absoluteTolerance
                    || relativeDelta <= relativeTolerance;

                comparisons.Add(new CsvParityComparisonRow
                {
                    ExactMatch = exact,
                    ExpectedValue = real?.RawValue ?? string.Empty,
                    ActualValue = oracle?.RawValue ?? string.Empty,
                    AbsoluteDelta = absoluteDelta,
                    RelativeDelta = relativeDelta,
                    Module = real?.Module ?? oracle?.Module ?? string.Empty,
                    FormulaField = real?.FormulaField ?? oracle?.FormulaField ?? string.Empty,
                    Variant = real?.Variant ?? oracle?.Variant ?? string.Empty,
                    ZoneIndex = real?.ZoneIndex ?? oracle?.ZoneIndex ?? 0,
                    Month = real?.Month ?? oracle?.Month ?? 0,
                    Classification = exact || withinTolerance
                        ? CsvMismatchClassification.Unclassified
                        : Classify(real, oracle)
                });
            }

            return new CsvParityComparison(comparisons);
        }

        public void WriteReport(string path, CsvParityComparison comparison)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            ArgumentNullException.ThrowIfNull(comparison);

            var lines = new List<string>
            {
                "ExactMatch,ExpectedValue,ActualValue,AbsoluteDelta,RelativeDelta,Module,FormulaField,Variant,ZoneIndex,Month,Classification"
            };
            lines.AddRange(comparison.Rows.Select(row => string.Join(",",
                row.ExactMatch.ToString(CultureInfo.InvariantCulture),
                Escape(row.ExpectedValue),
                Escape(row.ActualValue),
                Format(row.AbsoluteDelta),
                Format(row.RelativeDelta),
                Escape(row.Module),
                Escape(row.FormulaField),
                Escape(row.Variant),
                row.ZoneIndex.ToString(CultureInfo.InvariantCulture),
                row.Month.ToString(CultureInfo.InvariantCulture),
                row.Classification.ToString())));

            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            WriteAllLinesWithRetry(path, lines);
        }

        private static void WriteAllLinesWithRetry(string path, IReadOnlyList<string> lines)
        {
            const int maxAttempts = 5;
            for (var attempt = 1; attempt < maxAttempts; attempt++)
            {
                try
                {
                    File.WriteAllLines(path, lines);
                    return;
                }
                catch (IOException) when (attempt < maxAttempts)
                {
                    Thread.Sleep(50 * attempt);
                }
            }

            try
            {
                File.WriteAllLines(path, lines);
            }
            catch (IOException) when (File.Exists(path))
            {
                return;
            }
        }

        private static CsvMismatchClassification Classify(OracleCsvRecord? real, OracleCsvRecord? oracle)
        {
            if (real == null)
            {
                return CsvMismatchClassification.RealSoftwareExportIssue;
            }

            if (oracle == null)
            {
                return CsvMismatchClassification.LegacySideEffectMissing;
            }

            if (!string.Equals(real.Module, oracle.Module, StringComparison.Ordinal)
                || real.Month != oracle.Month
                || real.ZoneIndex != oracle.ZoneIndex)
            {
                return CsvMismatchClassification.ExecutionOrderMismatch;
            }

            if (!string.Equals(real.Variant, oracle.Variant, StringComparison.Ordinal))
            {
                return CsvMismatchClassification.InputBindingMismatch;
            }

            return CsvMismatchClassification.Unclassified;
        }

        private static double CalculateAbsoluteDelta(double? left, double? right)
        {
            if (!left.HasValue || !right.HasValue)
            {
                return double.NaN;
            }

            return Math.Abs(left.Value - right.Value);
        }

        private static double CalculateRelativeDelta(double? left, double? right, double absoluteDelta)
        {
            if (!left.HasValue || !right.HasValue || double.IsNaN(absoluteDelta))
            {
                return double.NaN;
            }

            var denominator = Math.Abs(left.Value);
            return denominator == 0.0 ? absoluteDelta : absoluteDelta / denominator;
        }

        private static string Format(double value)
        {
            return value.ToString("G17", CultureInfo.InvariantCulture);
        }

        private static string Escape(string value)
        {
            return value.Contains(',') ? "\"" + value.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"" : value;
        }
    }

    public sealed class OracleCsvRecord
    {
        public string SourceName { get; init; } = string.Empty;

        public string FixtureId { get; init; } = string.Empty;

        public string Module { get; init; } = string.Empty;

        public string FormulaField { get; init; } = string.Empty;

        public string Variant { get; init; } = string.Empty;

        public int ZoneIndex { get; init; }

        public int Month { get; init; }

        public string RawValue { get; init; } = string.Empty;

        public double? Value { get; init; }

        public string Key => string.Join("|", FixtureId, Module, FormulaField, Variant, ZoneIndex, Month);

        public static IReadOnlyList<OracleCsvRecord> Load(string path, string sourceName)
        {
            var rows = EECalcDebugCsv.Read(path);
            return rows.Select(row => FromRow(row, sourceName)).ToList();
        }

        private static OracleCsvRecord FromRow(IReadOnlyDictionary<string, string> row, string sourceName)
        {
            var rawValue = Get(row, "Value");
            return new OracleCsvRecord
            {
                SourceName = sourceName,
                FixtureId = Get(row, "FixtureId"),
                Module = Get(row, "Module"),
                FormulaField = Get(row, "FormulaField"),
                Variant = Get(row, "Variant"),
                ZoneIndex = ParseInt(Get(row, "ZoneIndex")),
                Month = ParseInt(Get(row, "Month")),
                RawValue = rawValue,
                Value = double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
                    ? parsed
                    : null
            };
        }

        private static string Get(IReadOnlyDictionary<string, string> row, string key)
        {
            return row.TryGetValue(key, out var value) ? value : string.Empty;
        }

        private static int ParseInt(string value)
        {
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
                ? parsed
                : 0;
        }
    }

    public sealed class CsvParityComparison
    {
        public CsvParityComparison(IReadOnlyList<CsvParityComparisonRow> rows)
        {
            Rows = rows;
        }

        public IReadOnlyList<CsvParityComparisonRow> Rows { get; }

        public bool Passed => Rows.All(row => row.ExactMatch);
    }

    public sealed class CsvParityComparisonRow
    {
        public bool ExactMatch { get; init; }

        public string ExpectedValue { get; init; } = string.Empty;

        public string ActualValue { get; init; } = string.Empty;

        public double AbsoluteDelta { get; init; }

        public double RelativeDelta { get; init; }

        public string Module { get; init; } = string.Empty;

        public string FormulaField { get; init; } = string.Empty;

        public string Variant { get; init; } = string.Empty;

        public int ZoneIndex { get; init; }

        public int Month { get; init; }

        public CsvMismatchClassification Classification { get; init; }
    }

    public enum CsvMismatchClassification
    {
        FormulaExtractionMismatch,
        InputBindingMismatch,
        ExecutionOrderMismatch,
        LegacySideEffectMissing,
        RealSoftwareExportIssue,
        Unclassified
    }
}
