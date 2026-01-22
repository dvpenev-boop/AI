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

    /// <summary>
    /// Input model for Unheated Basement floor type
    /// Под към неотопляем сутерен с изчисление на контакт със земята
    /// </summary>
    public partial class FloorUnheatedBasementInput : ObservableObject
    {
        // === Геометрия ===
        
        /// <summary>
        /// Площ на пода между отопляемото и сутерена (m²)
        /// </summary>
        [ObservableProperty]
        private double area;

        /// <summary>
        /// Периметър на сутерена (m)
        /// </summary>
        [ObservableProperty]
        private double perimeter;

        /// <summary>
        /// Дълбочина на сутеренните стени под терена (m)
        /// </summary>
        [ObservableProperty]
        private double depthBelowGround;

        /// <summary>
        /// Височина на сутеренните стени над терена (m)
        /// </summary>
        [ObservableProperty]
        private double heightAboveGround;

        /// <summary>
        /// Обем на неотопляемия сутерен (m³)
        /// </summary>
        [ObservableProperty]
        private double volume;

        // === Параметри за земята ===
        
        /// <summary>
        /// Топлопроводност на земята λg (W/m·K)
        /// </summary>
        [ObservableProperty]
        private double lambdaGround = 2.0;

        /// <summary>
        /// Пълна дебелина на стените на нивото на терена d_we (m)
        /// </summary>
        [ObservableProperty]
        private double wallThicknessAtGrade;

        // === Вентилация ===
        
        /// <summary>
        /// Кратност на въздухообмена n (1/h)
        /// По подразбиране 0.3 ако липсва
        /// </summary>
        [ObservableProperty]
        private double airChangeRate = 0.3;

        // === Константи (могат да се презаписват при нужда) ===
        
        /// <summary>
        /// Плътност на въздуха ρ (kg/m³)
        /// </summary>
        [ObservableProperty]
        private double airDensity = 1.2;

        /// <summary>
        /// Специфичен топлинен капацитет на въздуха cp (Wh/(kg·K))
        /// </summary>
        [ObservableProperty]
        private double airSpecificHeat = 0.28;

        // === Топлинни съпротивления ===
        
        /// <summary>
        /// Rsi за под между отопляемото и сутерена
        /// </summary>
        [ObservableProperty]
        private double rsiFloorToBasement = 0.17;

        /// <summary>
        /// Rse за под между отопляемото и сутерена
        /// </summary>
        [ObservableProperty]
        private double rseFloorToBasement = 0.17;

        /// <summary>
        /// Rsi за подова плоча на сутерена към земя
        /// </summary>
        [ObservableProperty]
        private double rsiBasementFloor = 0.17;

        /// <summary>
        /// Rse за подова плоча на сутерена към земя
        /// </summary>
        [ObservableProperty]
        private double rseBasementFloor = 0.00;

        /// <summary>
        /// Rsi за сутеренни стени към земя
        /// </summary>
        [ObservableProperty]
        private double rsiBasementWall = 0.13;

        /// <summary>
        /// Rse за сутеренни стени към земя
        /// </summary>
        [ObservableProperty]
        private double rseBasementWall = 0.00;

        /// <summary>
        /// Rsi за сутеренни стени над терена
        /// </summary>
        [ObservableProperty]
        private double rsiWallAboveGrade = 0.13;

        /// <summary>
        /// Rse за сутеренни стени над терена
        /// </summary>
        [ObservableProperty]
        private double rseWallAboveGrade = 0.04;

        // === Слоеве на конструкции ===
        
        /// <summary>
        /// Слоеве на пода между отопляемото и сутерена
        /// </summary>
        public ObservableCollection<FloorLayer> FloorToBasementLayers { get; } = new ObservableCollection<FloorLayer>();

        /// <summary>
        /// Слоеве на подовата плоча на сутерена към земя
        /// </summary>
        public ObservableCollection<FloorLayer> BasementFloorLayers { get; } = new ObservableCollection<FloorLayer>();

        /// <summary>
        /// Слоеве на сутеренните стени към земя
        /// </summary>
        public ObservableCollection<FloorLayer> BasementWallLayers { get; } = new ObservableCollection<FloorLayer>();

        /// <summary>
        /// Слоеве на сутеренните стени над терена
        /// </summary>
        public ObservableCollection<FloorLayer> WallAboveGradeLayers { get; } = new ObservableCollection<FloorLayer>();
    }
}
