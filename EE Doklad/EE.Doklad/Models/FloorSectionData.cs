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
    public ObservableCollection<FloorLayer> Layers { get; } = new ObservableCollection<FloorLayer>();
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

        [ObservableProperty]
        private FloorUnheatedBasementDetail? unheatedBasementDetail;

        /// <summary>
        /// Уникален идентификатор на групата за композитни подове
        /// </summary>
        [ObservableProperty]
        private string? groupId;

        /// <summary>
        /// Дали този под е част от композитна група (напр. отопляем сутерен = под + стена)
        /// </summary>
        [ObservableProperty]
        private bool isComposite;

        /// <summary>
        /// Тип на компонента в композитната група: "Floor" или "Wall"
        /// </summary>
        [ObservableProperty]
        private string? compositeType;

        public string TypeLabel => FloorType switch
        {
            FloorType.ExternalAir => "Под към външен въздух",
            FloorType.Ground => "Под към земя",
            FloorType.UnheatedBasement => "Под към неотопляем сутерен",
            FloorType.HeatedBasement when CompositeType == "Floor" => "Под над отопляем сутерен – Под към земя",
            FloorType.HeatedBasement when CompositeType == "Wall" => "Под над отопляем сутерен – Стена към земя",
            FloorType.HeatedBasement => "Под над отопляем сутерен",
            _ => "Неизвестен"
        };

        public string UDisplay => UValue > 0 ? $"{UValue:F3}" : "—";
        public string ADisplay => Area > 0 ? $"{Area:F2}" : "—";

        // Partial method to notify display changes when Area changes
        partial void OnAreaChanged(double value)
        {
            OnPropertyChanged(nameof(ADisplay));
        }

        partial void OnCompositeTypeChanged(string? value)
        {
            OnPropertyChanged(nameof(TypeLabel));
        }

        public void NotifyDisplayPropertiesChanged()
        {
            // Only notify display properties, not Area itself to avoid triggering recursive recalculation
            OnPropertyChanged(nameof(UDisplay));
            OnPropertyChanged(nameof(ADisplay));
            OnPropertyChanged(nameof(TypeLabel));
        }
    }

    // Detail class for External Air floor type
    public partial class FloorExternalAirDetail : ObservableObject
    {
        // Area property for auto-calculation
        [ObservableProperty] 
        private double area;

        // Константи за топлинни съпротивления
        public double Rsi => 0.17;
        public double Rse => 0.04;

        // Многослойна конструкция
    public ObservableCollection<FloorLayer> Layers { get; } = new ObservableCollection<FloorLayer>();

        // За изображение (схема)
        private AttachmentData? _schemeAttachment = new();
        public AttachmentData? SchemeAttachment
        {
            get => _schemeAttachment;
            set { _schemeAttachment = value; OnPropertyChanged(); }
        }
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
    public ObservableCollection<FloorLayer> Layers { get; } = new ObservableCollection<FloorLayer>();
    }

    // Detail class for Heated Basement floor type
    public partial class FloorHeatedBasementDetail : ObservableObject
    {
        // === Геометрия ===
        /// <summary>
        /// Площ на подовата плоча на сутерена (m²)
        /// </summary>
        [ObservableProperty]
        private double area;

        /// <summary>
        /// Периметър на сутерена (m)
        /// </summary>
        [ObservableProperty]
        private double perimeter;

        /// <summary>
        /// Дълбочина на сутерена под терена z (m)
        /// </summary>
        [ObservableProperty]
        private double depth;

        /// <summary>
        /// Пълна дебелина на стените на нивото на терена d_we (m)
        /// </summary>
        [ObservableProperty]
        private double wallThicknessAtGrade;

        // === Параметри за земята ===
        /// <summary>
        /// Топлопроводност на земята λg (W/m·K)
        /// </summary>
        [ObservableProperty]
        private double lambdaGround = 2.0;

        /// <summary>
        /// Линеен топлинен мост ψ_wf (W/mK), по подразбиране 0
        /// </summary>
        [ObservableProperty]
        private double psiWallFloor = 0.0;

        // === Топлинни съпротивления ===
        /// <summary>
        /// Rsi за подова плоча на сутерена (m²K/W)
        /// </summary>
        [ObservableProperty]
        private double rsiFloor = 0.17;

        /// <summary>
        /// Rse за подова плоча на сутерена (m²K/W)
        /// </summary>
        [ObservableProperty]
        private double rseFloor = 0.04;

        /// <summary>
        /// Rsi за сутеренни стени към земя (m²K/W)
        /// </summary>
        [ObservableProperty]
        private double rsiWall = 0.13;

        /// <summary>
        /// Rse за сутеренни стени към земя (m²K/W)
        /// </summary>
        [ObservableProperty]
        private double rseWall = 0.00;

        // === Диагностични резултати ===
        [ObservableProperty]
        private double resultUfgb; // U на подова плоча към земя

        [ObservableProperty]
        private double resultUwgb; // U на стени към земя

        [ObservableProperty]
        private double resultHg; // Стационарен коефициент

        [ObservableProperty]
        private double resultB; // Характеристичен размер

        [ObservableProperty]
        private double resultDf; // Еквивалентна дебелина на пода

        [ObservableProperty]
        private double resultDwb; // Еквивалентна дебелина на стените

        [ObservableProperty]
        private double resultAwalls; // Площ на стените към земя

        // === Слоеве на конструкции ===
        /// <summary>
        /// Слоеве на подовата плоча на сутерена към земя
        /// </summary>
        public ObservableCollection<FloorLayer> FloorLayers { get; } = new ObservableCollection<FloorLayer>();

        /// <summary>
        /// Слоеве на сутеренните стени към земя
        /// </summary>
        public ObservableCollection<FloorLayer> WallLayers { get; } = new ObservableCollection<FloorLayer>();
    }

    // Detail class for Unheated Basement floor type
    public partial class FloorUnheatedBasementDetail : ObservableObject
    {
        // === Геометрия ===
        [ObservableProperty] private double area;
        [ObservableProperty] private double perimeter;
        [ObservableProperty] private double depthBelowGround;
        [ObservableProperty] private double heightAboveGround;
        [ObservableProperty] private double volume;

        // === Параметри за земята ===
        [ObservableProperty] private double lambdaGround = 2.0;
        [ObservableProperty] private double wallThicknessAtGrade;

        // === Вентилация ===
        [ObservableProperty] private double airChangeRate = 0.3;
        [ObservableProperty] private double airDensity = 1.2;
        [ObservableProperty] private double airSpecificHeat = 0.28;

        // === Топлинни съпротивления ===
        [ObservableProperty] private double rsiFloorToBasement = 0.17;
        [ObservableProperty] private double rseFloorToBasement = 0.17;
        [ObservableProperty] private double rsiBasementFloor = 0.17;
        [ObservableProperty] private double rseBasementFloor = 0.00;
        [ObservableProperty] private double rsiBasementWall = 0.13;
        [ObservableProperty] private double rseBasementWall = 0.00;
        [ObservableProperty] private double rsiWallAboveGrade = 0.13;
        [ObservableProperty] private double rseWallAboveGrade = 0.04;

        // === Диагностични изходи ===
        [ObservableProperty] private double resultUfSus;
        [ObservableProperty] private double resultUfGb;
        [ObservableProperty] private double resultUwGb;
        [ObservableProperty] private double resultUw;
        [ObservableProperty] private double resultHveB;
        [ObservableProperty] private double resultB;
        [ObservableProperty] private double resultDf;
        [ObservableProperty] private double resultDwb;
        [ObservableProperty] private double resultS;
        [ObservableProperty] private double resultUub;

        // === Слоеве на конструкции ===
        public ObservableCollection<FloorLayer> FloorToBasementLayers { get; } = new ObservableCollection<FloorLayer>();
        public ObservableCollection<FloorLayer> BasementFloorLayers { get; } = new ObservableCollection<FloorLayer>();
        public ObservableCollection<FloorLayer> BasementWallLayers { get; } = new ObservableCollection<FloorLayer>();
        public ObservableCollection<FloorLayer> WallAboveGradeLayers { get; } = new ObservableCollection<FloorLayer>();
    }
}
