using System.Collections.Generic;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcExpectedSnapshot
    {
        public string FixtureName { get; init; } = string.Empty;

        public string Scenario { get; init; } = "Actual";

        public string Source { get; init; } = "EECalc";

        public IList<EecalcMonthlySnapshotRow> Months { get; init; } = new List<EecalcMonthlySnapshotRow>();
    }
}
