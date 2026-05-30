namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcEnvelopeFixture
    {
        public string Id { get; init; } = string.Empty;

        public EecalcValidationFixture Calculation { get; init; } = new();

        public EecalcWallDirectionFixture NorthWalls { get; init; } = new();

        public EecalcWallDirectionFixture NorthEastWalls { get; init; } = new();

        public EecalcWallDirectionFixture EastWalls { get; init; } = new();

        public EecalcWallDirectionFixture SouthEastWalls { get; init; } = new();

        public EecalcWallDirectionFixture SouthWalls { get; init; } = new();

        public EecalcWallDirectionFixture SouthWestWalls { get; init; } = new();

        public EecalcWallDirectionFixture WestWalls { get; init; } = new();

        public EecalcWallDirectionFixture NorthWestWalls { get; init; } = new();

        public EecalcRoofFixture Roof { get; init; } = new();

        public EecalcFloorFixture Floor { get; init; } = new();
    }

    public sealed class EecalcWallDirectionFixture
    {
        public double[] OuterA { get; init; } = new double[6];

        public double[] OuterU { get; init; } = new double[6];

        public double[] OuterSumL { get; init; } = new double[6];

        public double[] OuterSumX { get; init; } = new double[6];

        public double AccumulateWindowA { get; set; }

        public double AccumulateWindowU { get; set; }

        public double AccumulateWindowG { get; set; }

        public double AccumulateWindowE { get; set; }

        public double AccumulateOuterA { get; set; }

        public double AccumulateOuterU { get; set; }

        public double AccumulateOuterAlfa { get; set; }

        public double AccumulateOuterE { get; set; }

        public double[] InnerA { get; init; } = new double[6];

        public double[] InnerU { get; init; } = new double[6];

        public double[] InnerW { get; init; } = new double[6];

        public double[] InnerCoolingS { get; init; } = new double[6];
    }

    public sealed class EecalcRoofFixture
    {
        public double[] NonTransparentA { get; init; } = new double[9];

        public double[] NonTransparentU { get; init; } = new double[9];

        public double[] NonTransparentSumL { get; init; } = new double[9];

        public double[] NonTransparentSumX { get; init; } = new double[9];

        public double[] TransparentA { get; init; } = new double[9];

        public double[] TransparentU { get; init; } = new double[9];

        public double[] TransparentG { get; init; } = new double[9];

        public double[] TransparentE { get; init; } = new double[9];

        public double AccumulateNonTransparentA { get; set; }

        public double AccumulateNonTransparentU { get; set; }

        public double AccumulateNonTransparentAlfa { get; set; }

        public double AccumulateNonTransparentE { get; set; }

        public double[] CeilingA { get; init; } = new double[6];

        public double[] CeilingU { get; init; } = new double[6];

        public double[] CeilingW { get; init; } = new double[6];

        public double[] CeilingCoolingS { get; init; } = new double[6];
    }

    public sealed class EecalcFloorFixture
    {
        public double AccumulateFloorA { get; set; }

        public double AccumulateFloorU { get; set; }

        public double[] OtherFloorA { get; init; } = new double[6];

        public double[] OtherFloorU { get; init; } = new double[6];

        public double[] OtherFloorW { get; init; } = new double[6];

        public double[] OtherFloorCoolingS { get; init; } = new double[6];
    }
}
