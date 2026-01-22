using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Input model for External Air floor type
    /// </summary>
    public partial class FloorExternalAirInput : ObservableObject
    {
        [ObservableProperty]
        private double area;

        [ObservableProperty]
        private double ti = 20.0;

        [ObservableProperty]
        private double te = -15.0;

        [ObservableProperty]
        private double rsi = 0.17;

        [ObservableProperty]
        private double rse = 0.04;

    public ObservableCollection<FloorLayer> Layers { get; } = new ObservableCollection<FloorLayer>();
    }

    /// <summary>
    /// Input model for Ground floor type
    /// </summary>
    public partial class FloorGroundInput : ObservableObject
    {
        [ObservableProperty]
        private double area;

        [ObservableProperty]
        private double perimeter;

        [ObservableProperty]
        private double lambdaGround = 2.0;

        [ObservableProperty]
        private GroundInsulationType insulationType = GroundInsulationType.None;

        [ObservableProperty]
        private double insulationWidth;

        [ObservableProperty]
        private double insulationDepth;


        [ObservableProperty]
        private double rsi = 0.17;

    public ObservableCollection<FloorLayer> Layers { get; } = new ObservableCollection<FloorLayer>();

    // Нови полета за разширена логика (съответствие с FloorGroundDetail)
    [ObservableProperty]
    private double wallThickness;

    [ObservableProperty]
    private double insulationThickness;

    [ObservableProperty]
    private double insulationLambda = 0.04; // λ of peripheral insulation (W/mK)

    }

    /// <summary>
    /// Input model for Unheated Space floor type
    /// </summary>
    public partial class FloorUnheatedSpaceInput : ObservableObject
    {
        [ObservableProperty]
        private double area;

        [ObservableProperty]
        private double perimeter;

        [ObservableProperty]
        private double height;

        [ObservableProperty]
        private double ti = 20.0;

        [ObservableProperty]
        private double te = -15.0;

        [ObservableProperty]
        private VentilationMode ventilationMode = VentilationMode.None;

        [ObservableProperty]
        private double airChangeRate; // n

        [ObservableProperty]
        private double volumeFlowRate;

        [ObservableProperty]
        private double windSpeed;

        [ObservableProperty]
        private double rsi = 0.17;

        [ObservableProperty]
        private double rse = 0.04;

        [ObservableProperty]
        private double n; // Air change rate (same as airChangeRate, but for backward compat)

    public ObservableCollection<FloorLayer> Layers { get; } = new ObservableCollection<FloorLayer>();
    }

    /// <summary>
    /// Input model for Heated Basement floor type
    /// </summary>
    public partial class FloorHeatedBasementInput : ObservableObject
    {
        [ObservableProperty]
        private double floorArea;

        [ObservableProperty]
        private double areaFloor; // Alias for floorArea

        [ObservableProperty]
        private double ti = 20.0;

        [ObservableProperty]
        private double tb = 15.0;

        [ObservableProperty]
        private double te = -15.0;

        [ObservableProperty]
        private double basementDepth;

        [ObservableProperty]
        private double z; // Alias for basementDepth

        [ObservableProperty]
        private double wallAreaToGround;

        [ObservableProperty]
        private double rsiFloor = 0.17;

        [ObservableProperty]
        private double rseFloor = 0.04;

        [ObservableProperty]
        private double rsiWall = 0.13;

    public ObservableCollection<FloorLayer> FloorLayers { get; } = new ObservableCollection<FloorLayer>();
    public ObservableCollection<FloorLayer> WallLayers { get; } = new ObservableCollection<FloorLayer>();
    }
}
