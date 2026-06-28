namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcMonthlySnapshotRow
    {
        public string Month { get; init; } = string.Empty;

        public int? WorkDays { get; init; }

        public int? Saturdays { get; init; }

        public int? Sundays { get; init; }

        public int? Holidays { get; init; }

        public int? TotalDays { get; init; }

        public double? Weeks { get; init; }

        public double? Hve { get; init; }

        public double? Htr { get; init; }

        public double? Qtr { get; init; }

        public double? Qve { get; init; }

        public double? Qgn { get; init; }

        public double? Gamma { get; init; }

        public double? Ni { get; init; }

        public double? Qnd { get; init; }

        public double? QndPerArea { get; init; }
    }
}
