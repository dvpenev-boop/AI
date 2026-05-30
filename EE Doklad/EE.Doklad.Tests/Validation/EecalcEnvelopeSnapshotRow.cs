namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcEnvelopeSnapshotRow
    {
        public string Fixture { get; init; } = string.Empty;

        public string Month { get; init; } = string.Empty;

        public double AvgTemp { get; init; }

        public double AvgInnerHeatTemp { get; init; }

        public double Hd { get; init; }

        public double Hg { get; init; }

        public double HuWalls { get; init; }

        public double HuCeilings { get; init; }

        public double HuFloors { get; init; }

        public double Hu { get; init; }

        public double Htr { get; init; }

        public double DegreeHours { get; init; }

        public double Qtr { get; init; }
    }
}
