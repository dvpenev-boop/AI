using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    public partial class FloorSectionData : ObservableObject
    {
        [ObservableProperty]
        private string description = string.Empty;

        public ObservableCollection<FloorItem> FloorItems { get; } = new ObservableCollection<FloorItem>();
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
        [ObservableProperty]
        private double area;
        [ObservableProperty]
        private string error = string.Empty;
        [ObservableProperty]
        private double rsi = 0.17;
        [ObservableProperty]
        private double perimeter;

        [ObservableProperty]
        private double lambdaGround = 2.0; // Default value for soil

        [ObservableProperty]
        private GroundInsulationType insulationType = GroundInsulationType.None;

        [ObservableProperty]
        private double insulationWidth; // For edge insulation

        [ObservableProperty]
        private double insulationDepth; // For edge insulation

        [ObservableProperty]
        private double df; // Characteristic dimension

        // Нови полета за разширена логика
        [ObservableProperty]
        private double insulationThickness; // dn

        [ObservableProperty]
        private double insulationResistance; // Rn

        [ObservableProperty]
        private double wallThickness; // dw,e

        [ObservableProperty]
        private bool isDfAuto = true;

        [ObservableProperty]
        private bool isAltMethod;

        [ObservableProperty]
        private string debugInfo = string.Empty;



        public ObservableCollection<RoofLayer> Layers { get; } = new ObservableCollection<RoofLayer>();
    }

    // Detail class for Unheated Space floor type
    public partial class FloorUnheatedSpaceDetail : ObservableObject
    {
        [ObservableProperty]
        private double ti = 20.0;



        [ObservableProperty]
        private double height; // Height above ground

        [ObservableProperty]
        private double perimeter;

        [ObservableProperty]
        private VentilationMode ventilationMode = VentilationMode.None;

        [ObservableProperty]
        private double airChangeRate; // n (1/h)

        [ObservableProperty]
        private double volumeFlowRate; // V_dot (m³/h)

        [ObservableProperty]
        private double windSpeed; // at 10m height

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
