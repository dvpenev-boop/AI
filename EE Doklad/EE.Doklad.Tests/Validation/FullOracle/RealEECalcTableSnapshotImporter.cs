using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace EE.Doklad.Tests.Validation.FullOracle
{
    public sealed class RealEECalcTableSnapshotImporter
    {
        public IReadOnlyList<OracleCsvRecord> Load(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            using var document = JsonDocument.Parse(File.ReadAllText(path));
            var root = document.RootElement;
            var fixtureId = root.TryGetProperty("fixtureId", out var fixtureElement)
                ? fixtureElement.GetString() ?? string.Empty
                : string.Empty;
            var variant = root.TryGetProperty("variant", out var variantElement)
                ? variantElement.GetString() ?? "Actual"
                : "Actual";
            var records = new List<OracleCsvRecord>();

            if (!root.TryGetProperty("finalTables", out var finalTables))
            {
                return records;
            }

            foreach (var table in finalTables.EnumerateObject())
            {
                Flatten(table.Name, table.Value, fixtureId, variant, records);
            }

            return records;
        }

        private static void Flatten(
            string path,
            JsonElement element,
            string fixtureId,
            string variant,
            List<OracleCsvRecord> records)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var child in element.EnumerateObject())
                {
                    Flatten(path + "." + child.Name, child.Value, fixtureId, variant, records);
                }

                return;
            }

            var rawValue = element.ValueKind == JsonValueKind.Number
                ? element.GetDouble().ToString("G17", CultureInfo.InvariantCulture)
                : element.ToString();

            records.Add(new OracleCsvRecord
            {
                SourceName = "RealEECalcTableSnapshot",
                FixtureId = fixtureId,
                Module = "FullResult",
                FormulaField = path,
                Variant = variant,
                ZoneIndex = 0,
                Month = 0,
                RawValue = rawValue,
                Value = double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
                    ? value
                    : null
            });
        }
    }
}
