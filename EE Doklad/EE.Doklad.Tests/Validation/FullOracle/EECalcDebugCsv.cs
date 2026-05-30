using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace EE.Doklad.Tests.Validation.FullOracle
{
    public static class EECalcDebugCsv
    {
        private static readonly object WriteLock = new();

        public static readonly string[] RequiredColumns =
        {
            "FixtureId",
            "Mode",
            "Variant",
            "ZoneIndex",
            "Month",
            "Module",
            "Source"
        };

        public static void Write(string path, IEnumerable<EECalcDebugRow> rows)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            ArgumentNullException.ThrowIfNull(rows);

            var materialized = rows.ToList();
            var columns = RequiredColumns
                .Concat(materialized.SelectMany(row => row.Fields.Keys).Distinct(StringComparer.Ordinal))
                .ToList();
            var lines = new List<string> { string.Join(",", columns.Select(Escape)) };

            foreach (var row in materialized)
            {
                lines.Add(string.Join(",", columns.Select(column => Escape(GetValue(row, column)))));
            }

            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            WriteAllLinesWithRetry(path, lines);
        }

        public static IReadOnlyList<Dictionary<string, string>> Read(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            using var reader = new StreamReader(path, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            var headerLine = reader.ReadLine();
            if (headerLine == null)
            {
                return Array.Empty<Dictionary<string, string>>();
            }

            var columns = ParseLine(headerLine);
            var rows = new List<Dictionary<string, string>>();
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var values = ParseLine(line);
                var row = new Dictionary<string, string>(StringComparer.Ordinal);
                for (var i = 0; i < columns.Count; i++)
                {
                    row[columns[i]] = i < values.Count ? values[i] : string.Empty;
                }

                rows.Add(row);
            }

            return rows;
        }

        private static string GetValue(EECalcDebugRow row, string column)
        {
            return column switch
            {
                "FixtureId" => row.FixtureId,
                "Mode" => row.Mode,
                "Variant" => row.Variant,
                "ZoneIndex" => row.ZoneIndex.ToString(CultureInfo.InvariantCulture),
                "Month" => row.Month.ToString(CultureInfo.InvariantCulture),
                "Module" => row.Module,
                "Source" => row.Source,
                _ => row.Fields.TryGetValue(column, out var value) ? value : string.Empty
            };
        }

        private static string Escape(string value)
        {
            if (!value.Contains(',') && !value.Contains('"') && !value.Contains('\n') && !value.Contains('\r'))
            {
                return value;
            }

            return "\"" + value.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";
        }

        private static void WriteAllLinesWithRetry(string path, IReadOnlyList<string> lines)
        {
            lock (WriteLock)
            {
                const int maxAttempts = 5;
                for (var attempt = 1; attempt < maxAttempts; attempt++)
                {
                    try
                    {
                        File.WriteAllLines(path, lines, Encoding.UTF8);
                        return;
                    }
                    catch (IOException) when (attempt < maxAttempts)
                    {
                        Thread.Sleep(50 * attempt);
                    }
                }

                File.WriteAllLines(path, lines, Encoding.UTF8);
            }
        }

        private static IReadOnlyList<string> ParseLine(string line)
        {
            var values = new List<string>();
            var current = new StringBuilder();
            var inQuotes = false;

            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (inQuotes)
                {
                    if (c == '"' && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else if (c == '"')
                    {
                        inQuotes = false;
                    }
                    else
                    {
                        current.Append(c);
                    }
                }
                else if (c == ',')
                {
                    values.Add(current.ToString());
                    current.Clear();
                }
                else if (c == '"')
                {
                    inQuotes = true;
                }
                else
                {
                    current.Append(c);
                }
            }

            values.Add(current.ToString());
            return values;
        }
    }
}
