using System;
using System.Collections.Generic;
using EE.Doklad.Tests.Validation;

namespace EE.Doklad.Models
{
    public sealed class ClimateSeedData
    {
        public IReadOnlyList<ClimateZoneData> Zones { get; init; } = Array.Empty<ClimateZoneData>();
    }

    public sealed class ClimateZoneData
    {
        public int Id { get; init; }

        public ClimateMonthlyData Monthly { get; init; } = new();

        public double BarometricPressure { get; init; }

        public double GetEffectiveBarometricPressure()
        {
            return BarometricPressure;
        }
    }

    public sealed class ClimateMonthlyData
    {
        public double[] AvgMonthlyTempC { get; init; } = new double[12];

        public IReadOnlyDictionary<string, double[]> AvgFullSolarVerticalWm2 { get; init; } =
            new Dictionary<string, double[]>();

        public double[] AvgMonthlyRelHumidityPercentMayToSep { get; init; } = new double[5];
    }
}

namespace EE.Doklad.Services
{
    using EE.Doklad.Models;

    public interface IClimateRepository
    {
        ClimateSeedData LoadSeed();
    }

    public sealed class JsonClimateRepository : IClimateRepository
    {
        public ClimateSeedData LoadSeed()
        {
            throw new NotSupportedException("Json climate seed loading is not included in this validation harness.");
        }
    }
}

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcExpectedSnapshot
    {
        public string FixtureName { get; init; } = string.Empty;

        public string Scenario { get; init; } = string.Empty;

        public string Source { get; init; } = string.Empty;

        public IReadOnlyList<EecalcMonthlySnapshotRow> Months { get; init; } =
            Array.Empty<EecalcMonthlySnapshotRow>();
    }
}
