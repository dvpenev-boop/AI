using System.Collections.Generic;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcEnvelopeSavingsResult
    {
        public double BaseLineEnergy { get; init; }

        public double EsmEnergy { get; init; }

        public double TotalSaving { get; init; }

        public IReadOnlyList<EecalcEnvelopeSavingItem> Items { get; init; } =
            new List<EecalcEnvelopeSavingItem>();
    }

    public sealed class EecalcEnvelopeUValues
    {
        public double OuterWallsU { get; init; }

        public double WindowsU { get; init; }

        public double NonTransparentRoofU { get; init; }

        public double FloorU { get; init; }
    }

    public sealed class EecalcEnvelopeSavingItem
    {
        public string Tag { get; init; } = string.Empty;

        public string Row { get; init; } = string.Empty;

        public double OldValue { get; init; }

        public double NewValue { get; init; }

        public double VirtualEnergy { get; init; }

        public double VirtualSaving { get; init; }

        public double VirtualEnergyNMinusOne { get; init; }

        public double VirtualSavingNMinusOne { get; init; }

        public double Part { get; init; }

        public double ActualSaving { get; init; }

        public double Percent => Part * 100.0;
    }

    public enum EecalcVentilationEsmMode
    {
        Heating,
        Cooling
    }

    public sealed class EecalcVentilationSavingsResult
    {
        public EecalcVentilationEsmMode Mode { get; init; }

        public double BaseLineEnergy { get; init; }

        public double EsmEnergy { get; init; }

        public double TotalSaving { get; init; }

        public IReadOnlyList<EecalcVentilationSavingItem> Items { get; init; } =
            new List<EecalcVentilationSavingItem>();
    }

    public sealed class EecalcVentilationSavingItem
    {
        public string Technology { get; init; } = string.Empty;

        public string Tag { get; init; } = string.Empty;

        public string Row { get; init; } = string.Empty;

        public double OldValue { get; init; }

        public double NewValue { get; init; }

        public double VirtualEnergy { get; init; }

        public double VirtualSaving { get; init; }

        public double Part { get; init; }

        public double ActualSaving { get; init; }

        public double Percent => Part * 100.0;
    }

    public sealed class EecalcComponentSavingsResult
    {
        public string Technology { get; init; } = string.Empty;

        public double BaseLineEnergy { get; init; }

        public double EsmEnergy { get; init; }

        public double TotalSaving { get; init; }

        public IReadOnlyList<EecalcComponentSavingItem> Items { get; init; } =
            new List<EecalcComponentSavingItem>();
    }

    public sealed class EecalcComponentSavingItem
    {
        public string Tag { get; init; } = string.Empty;

        public string Row { get; init; } = string.Empty;

        public double OldValue { get; init; }

        public double NewValue { get; init; }

        public double VirtualEnergy { get; init; }

        public double VirtualSaving { get; init; }

        public double Part { get; init; }

        public double ActualSaving { get; init; }

        public double Percent => Part * 100.0;
    }
}
