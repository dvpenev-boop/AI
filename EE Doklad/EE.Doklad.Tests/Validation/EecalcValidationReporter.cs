using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcValidationReporter
    {
        public EecalcComparisonResult Compare(
            EecalcExpectedSnapshot expected,
            EecalcActualSnapshot actual,
            double tolerance = 0.000001)
        {
            ArgumentNullException.ThrowIfNull(expected);
            ArgumentNullException.ThrowIfNull(actual);

            var mismatches = new List<EecalcMetricMismatch>();
            var actualByMonth = actual.Months.ToDictionary(m => m.Month, StringComparer.OrdinalIgnoreCase);

            foreach (var expectedMonth in expected.Months)
            {
                if (!actualByMonth.TryGetValue(expectedMonth.Month, out var actualMonth))
                {
                    mismatches.Add(new EecalcMetricMismatch
                    {
                        Month = expectedMonth.Month,
                        Metric = "Month",
                        Expected = 1.0,
                        Actual = null,
                        Tolerance = tolerance
                    });
                    continue;
                }

                CompareMetric(mismatches, expectedMonth.Month, "WorkDays", expectedMonth.WorkDays, actualMonth.WorkDays, tolerance);
                CompareMetric(mismatches, expectedMonth.Month, "Saturdays", expectedMonth.Saturdays, actualMonth.Saturdays, tolerance);
                CompareMetric(mismatches, expectedMonth.Month, "Sundays", expectedMonth.Sundays, actualMonth.Sundays, tolerance);
                CompareMetric(mismatches, expectedMonth.Month, "Holidays", expectedMonth.Holidays, actualMonth.Holidays, tolerance);
                CompareMetric(mismatches, expectedMonth.Month, "TotalDays", expectedMonth.TotalDays, actualMonth.TotalDays, tolerance);
                CompareMetric(mismatches, expectedMonth.Month, "Weeks", expectedMonth.Weeks, actualMonth.Weeks, tolerance);
                CompareMetric(mismatches, expectedMonth.Month, "Hve", expectedMonth.Hve, actualMonth.Hve, tolerance);
                CompareMetric(mismatches, expectedMonth.Month, "Htr", expectedMonth.Htr, actualMonth.Htr, tolerance);
                CompareMetric(mismatches, expectedMonth.Month, "Qtr", expectedMonth.Qtr, actualMonth.Qtr, tolerance);
                CompareMetric(mismatches, expectedMonth.Month, "Qve", expectedMonth.Qve, actualMonth.Qve, tolerance);
                CompareMetric(mismatches, expectedMonth.Month, "Qgn", expectedMonth.Qgn, actualMonth.Qgn, tolerance);
                CompareMetric(mismatches, expectedMonth.Month, "Gamma", expectedMonth.Gamma, actualMonth.Gamma, tolerance);
                CompareMetric(mismatches, expectedMonth.Month, "Ni", expectedMonth.Ni, actualMonth.Ni, tolerance);
                CompareMetric(mismatches, expectedMonth.Month, "Qnd", expectedMonth.Qnd, actualMonth.Qnd, tolerance);
                CompareMetric(mismatches, expectedMonth.Month, "QndPerArea", expectedMonth.QndPerArea, actualMonth.QndPerArea, tolerance);
            }

            return new EecalcComparisonResult
            {
                FixtureName = expected.FixtureName,
                Scenario = expected.Scenario,
                Mismatches = mismatches
            };
        }

        public string Format(EecalcComparisonResult result)
        {
            ArgumentNullException.ThrowIfNull(result);

            if (result.Passed)
            {
                return $"{result.FixtureName} / {result.Scenario}: passed";
            }

            var builder = new StringBuilder();
            builder.AppendLine($"{result.FixtureName} / {result.Scenario}: {result.Mismatches.Count} mismatch(es)");

            foreach (var mismatch in result.Mismatches)
            {
                builder.AppendLine(
                    $"{mismatch.Month} {mismatch.Metric}: expected={mismatch.Expected}, actual={mismatch.Actual}, diff={mismatch.AbsoluteDifference}, tolerance={mismatch.Tolerance}");
            }

            return builder.ToString();
        }

        private static void CompareMetric(
            ICollection<EecalcMetricMismatch> mismatches,
            string month,
            string metric,
            int? expected,
            int? actual,
            double tolerance)
        {
            CompareMetric(
                mismatches,
                month,
                metric,
                expected.HasValue ? expected.Value : (double?)null,
                actual.HasValue ? actual.Value : (double?)null,
                tolerance);
        }

        private static void CompareMetric(
            ICollection<EecalcMetricMismatch> mismatches,
            string month,
            string metric,
            double? expected,
            double? actual,
            double tolerance)
        {
            if (!expected.HasValue && !actual.HasValue)
            {
                return;
            }

            var absoluteDifference = expected.HasValue && actual.HasValue
                ? Math.Abs(expected.Value - actual.Value)
                : (double?)null;

            if (absoluteDifference.HasValue && absoluteDifference.Value <= tolerance)
            {
                return;
            }

            mismatches.Add(new EecalcMetricMismatch
            {
                Month = month,
                Metric = metric,
                Expected = expected,
                Actual = actual,
                AbsoluteDifference = absoluteDifference,
                Tolerance = tolerance
            });
        }
    }
}
