using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using EE.Doklad.Models;

namespace EE.Doklad.Models
{
    public partial class FloorSectionData : ObservableObject
    {
        [ObservableProperty]
        private string description = string.Empty;

        public ObservableCollection<FloorItem> FloorItems { get; } = new ObservableCollection<FloorItem>();
    }

    // Detail class for Unheated Space floor type
    public partial class FloorUnheatedSpaceDetail : ObservableObject
    {
        [ObservableProperty] private double ti = 20.0;
        [ObservableProperty] private double height;
        [ObservableProperty] private double perimeter;
        [ObservableProperty] private VentilationMode ventilationMode = VentilationMode.None;
        [ObservableProperty] private double airChangeRate;
        [ObservableProperty] private double volumeFlowRate;
        [ObservableProperty] private double windSpeed;
        public ObservableCollection<RoofLayer> Layers { get; } = new ObservableCollection<RoofLayer>();
    }

    public partial class FloorItem : ObservableObject
    {
        [ObservableProperty]
        private int number;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private FloorType floorType;

        [ObservableProperty]
        private double area;

        [ObservableProperty]
        private double uValue;

        // Detail properties for each floor type
        [ObservableProperty]
        private FloorExternalAirDetail? externalAirDetail;

        [ObservableProperty]
        private FloorGroundDetail? groundDetail;

        [ObservableProperty]
        private FloorUnheatedSpaceDetail? unheatedSpaceDetail;

    [ObservableProperty]
    private FloorHeatedBasementDetail? heatedBasementDetail;

        public string TypeLabel => FloorType switch
        {
            FloorType.ExternalAir => "Под към външен въздух",
            FloorType.Ground => "Под към земя",
            FloorType.UnheatedSpace => "Под към неотопляемо помещение",
            FloorType.HeatedBasement => "Под над отопляем сутерен",
            _ => "Неизвестен"
        };

        public string UDisplay => UValue > 0 ? $"{UValue:F3}" : "—";
        public string ADisplay => Area > 0 ? $"{Area:F2}" : "—";

        public void NotifyDisplayPropertiesChanged()
        {
            OnPropertyChanged(nameof(UDisplay));
            OnPropertyChanged(nameof(ADisplay));
        }
    }

    // Detail class for External Air floor type
    public partial class FloorExternalAirDetail : ObservableObject
    {
        [ObservableProperty]
        private double ti = 20.0;

        [ObservableProperty]
        private double te = -15.0;

        public ObservableCollection<RoofLayer> Layers { get; } = new ObservableCollection<RoofLayer>();
    }

    // Detail class for Ground floor type
    public partial class FloorGroundDetail : ObservableObject
    {
    [ObservableProperty] private double area;
    [ObservableProperty] private double perimeter;
    [ObservableProperty] private double lambdaGround;
    [ObservableProperty] private GroundInsulationType insulationType = GroundInsulationType.None;
    [ObservableProperty] private double insulationWidth;
    [ObservableProperty] private double insulationDepth;
    [ObservableProperty] private double wallThickness;
    [ObservableProperty] private double insulationThickness;
    [ObservableProperty] private double insulationLambda;
    [ObservableProperty] private string debugInfo = string.Empty;
    [ObservableProperty] private string error = string.Empty;
    [ObservableProperty] private double resultB;
    [ObservableProperty] private double resultRf;
    [ObservableProperty] private double resultDf;
    [ObservableProperty] private double resultRn;
    [ObservableProperty] private double resultRPrime;
    [ObservableProperty] private double resultDPrime;
    [ObservableProperty] private double resultArg1;
    [ObservableProperty] private double resultArg2;
    [ObservableProperty] private double resultPsiGed;
    [ObservableProperty] private double resultU0;
    [ObservableProperty] private double resultU;
    public ObservableCollection<RoofLayer> Layers { get; } = new ObservableCollection<RoofLayer>();
    }

    // Detail class for Heated Basement floor type
    public partial class FloorHeatedBasementDetail : ObservableObject
    {
        [ObservableProperty]
        private double ti = 20.0; // Main heated space

        [ObservableProperty]
        private double tb = 15.0; // Basement temperature

        [ObservableProperty]
        private double te = -15.0; // External/ground temperature

        [ObservableProperty]
        private double floorArea;

        [ObservableProperty]
        private double basementDepth; // z

        [ObservableProperty]
        private double wallAreaToGround;

        public ObservableCollection<RoofLayer> FloorLayers { get; } = new ObservableCollection<RoofLayer>();
        public ObservableCollection<RoofLayer> WallLayers { get; } = new ObservableCollection<RoofLayer>();
    }
}
