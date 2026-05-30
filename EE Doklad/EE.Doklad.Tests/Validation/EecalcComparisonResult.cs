using System.Collections.Generic;
using System.Linq;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcComparisonResult
    {
        public string FixtureName { get; init; } = string.Empty;

        public string Scenario { get; init; } = string.Empty;

        public IList<EecalcMetricMismatch> Mismatches { get; init; } = new List<EecalcMetricMismatch>();

        public bool Passed => !Mismatches.Any();
    }

    public sealed class EecalcMetricMismatch
    {
        public string Metric { get; init; } = string.Empty;

        public string Month { get; init; } = string.Empty;

        public double? Expected { get; init; }

        public double? Actual { get; init; }

        public double? AbsoluteDifference { get; init; }

        public double Tolerance { get; init; }
    }
}
