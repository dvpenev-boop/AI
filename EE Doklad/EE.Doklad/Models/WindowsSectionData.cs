using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Данни за секция 9 "Прозорци и врати"
    /// </summary>
    public partial class WindowsSectionData : ObservableObject
    {
        [ObservableProperty]
        private string description = "Въведете прозорци и врати като партиди по фасада. Обобщената таблица групира автоматично.";

        /// <summary>
        /// Всички партиди прозорци/врати (източник на истина)
        /// </summary>
        public ObservableCollection<WindowBatch> WindowBatches { get; } = new ObservableCollection<WindowBatch>();
    }

    /// <summary>
    /// Партида прозорци/врати (източник на истина)
    /// </summary>
    public partial class WindowBatch : ObservableObject
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [ObservableProperty]
        private WindowKind kind = WindowKind.Window;

        [ObservableProperty]
        private Orientation orientation = Orientation.South;

        [ObservableProperty]
        private int count = 1;

        [ObservableProperty]
        private double width; // m

        [ObservableProperty]
        private double height; // m

        [ObservableProperty]
        private double areaGross; // m² (или изчислено от Width x Height)

        [ObservableProperty]
        private double uValue; // W/m²K

        [ObservableProperty]
        private double gN; // g perpendicular (0..1)

        [ObservableProperty]
        private OpticalType opticalType = OpticalType.Clear;

        

        [ObservableProperty]
        private GlazingType glazingType = GlazingType.Double;

        [ObservableProperty]
        private double glazingGDif; // g_gl,dif,wi - дифузна пропускливост (по сертификат ISO 15099)
        [ObservableProperty]
        private double frameFraction = 0.15; // F_fr (0..0.5)

        [ObservableProperty]
        private string? shadingTypeId; // nullable

        [ObservableProperty]
        private double shadingReductionFactor = 1.0; // множител за g_eff

        [ObservableProperty]
        private string? obstacleProfileId; // nullable (deprecated - вижте ShadingConfig)

        [ObservableProperty]
        private double[]? monthlyObstacleFactors; // 12 месечни коефициента F_sh,obst[m] (deprecated)

        /// <summary>
        /// Конфигурация на засенчването (null = без засенчване)
        /// </summary>
        [ObservableProperty]
        private ShadingConfig? shadingConfig;

        /// <summary>
        /// Месечни коефициенти на намаление на прякото засенчване F_sh,dir[m] (Jan..Dec)
        /// Ако няма засенчване, всички са 1.0
        /// </summary>
        [ObservableProperty]
        private double[] fshDirMonthly = new double[12] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        /// <summary>
        /// Месечни стойности на ефективната пропускливост g_eff[m] (Jan..Dec)
        /// Изчислява се като: g_eff[m] = g_base * F_sh,gl * F_sh,dir[m]
        /// </summary>
        [ObservableProperty]
        private double[] gEffMonthly = new double[12];

        /// <summary>
        /// Дали има активно засенчване
        /// </summary>
        public bool HasShading => ShadingConfig != null && ShadingConfig.Shadings.Count > 0;

        /// <summary>
        /// Име на типа прозорец (за групиране и визуализация)
        /// </summary>
        [ObservableProperty]
        private string typeName = string.Empty;

        // Derived properties

        /// <summary>
        /// Площ на стъклото: A_gl = A_gross * (1 - F_fr)
        /// </summary>
        public double AreaGlass => AreaGross * (1 - FrameFraction);

        /// <summary>
        /// Базова ефективна пропускливост (преди shading)
        /// </summary>
        public double GEffBase
        {
            get
            {
                if (OpticalType == OpticalType.Clear && string.IsNullOrEmpty(ShadingTypeId))
                {
                    // 3.41: g_eff = 0.90 * g_n  (без щора, Clear стъкло)
                    return 0.90 * GN;
                }
                else
                {
                    // 3.42: g_eff_base = g_n  (с щора или не-Clear стъкло)
                    // ShadingReductionFactor (FShadeInt/FShadeExt) се прилага отделно
                    return GN;
                }
            }
        }

        /// <summary>
        /// Ефективна пропускливост след shading: g_eff = g_eff_base * ShadingReductionFactor
        /// Без щора: g_eff = 0.90 * g_n * 1.0
        /// С щора:   g_eff = g_n * FShadeInt/FShadeExt
        /// </summary>
        public double GEff => GEffBase * ShadingReductionFactor;

        partial void OnAreaGrossChanged(double value)
        {
            OnPropertyChanged(nameof(AreaGlass));
            OnPropertyChanged(nameof(GEffBase));
            OnPropertyChanged(nameof(GEff));
        }

        partial void OnFrameFractionChanged(double value)
        {
            OnPropertyChanged(nameof(AreaGlass));
        }

        partial void OnGNChanged(double value)
        {
            OnPropertyChanged(nameof(GEffBase));
            OnPropertyChanged(nameof(GEff));
        }

        partial void OnOpticalTypeChanged(OpticalType value)
        {
            OnPropertyChanged(nameof(GEffBase));
            OnPropertyChanged(nameof(GEff));
        }

        partial void OnShadingReductionFactorChanged(double value)
        {
            OnPropertyChanged(nameof(GEff));
        }

        partial void OnShadingTypeIdChanged(string? value)
        {
            OnPropertyChanged(nameof(GEffBase));
            OnPropertyChanged(nameof(GEff));
        }
    }

    /// <summary>
    /// Обобщен ред за визуализация (група по фасада и тип)
    /// </summary>
    public class WindowSummaryRow
    {
        public Orientation Orientation { get; set; }
        public string TypeSignature { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public int TotalCount { get; set; }
        public double ATotalGross { get; set; } // m²
        public double ATotalGlass { get; set; } // m²
        public double UAvg { get; set; } // W/m²K
        public double GAvg { get; set; } // безразмерен

        /// <summary>
        /// Партидите, които формират тази група
        /// </summary>
        public List<WindowBatch> Batches { get; set; } = new List<WindowBatch>();
    }

    /// <summary>
    /// Вид прозорец/врата
    /// </summary>
    public enum WindowKind
    {
        [Description("Прозорец")]
        Window,
        [Description("Врата")]
        Door
    }

    /// <summary>
    /// Ориентация (фасада)
    /// </summary>
    public enum Orientation
    {
        [Description("И")]
        East,
        [Description("СИ")]
        NorthEast,
        [Description("С")]
        North,
        [Description("СЗ")]
        NorthWest,
        [Description("З")]
        West,
        [Description("ЮЗ")]
        SouthWest,
        [Description("Ю")]
        South,
        [Description("ЮИ")]
        SouthEast
    }

    /// <summary>
    /// Оптичен тип
    /// </summary>
    public enum OpticalType
    {
        [Description("Прозрачно (Clear)")]
        Clear,
        [Description("Дифузно (Diffusing)")]
        Diffusing
    }

    /// <summary>
    /// Вид остъкляване (Таблица 3)
    /// </summary>
    public enum GlazingType
    {
        [Description("Единично стъкло")]
        Single,
        [Description("Двоен стъклопакет")]
        Double,
        [Description("Двоен селективен нискоемис.")]
        DoubleSelective,
        [Description("Троен стъклопакет")]
        Triple,
        [Description("Троен селективен")]
        TripleSelective,
        [Description("Друг / неуточнен")]
        Other
    }

    /// <summary>
    /// Данни за слънцезащита (Таблица 4)
    /// </summary>
    public class ShadingOption
    {
        public string Id { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public double AbsorptionAlpha { get; set; }
        public double TransmittanceTau { get; set; }
        public double FShadeInt { get; set; }
        public double FShadeExt { get; set; }

        public string DisplayName => $"{CategoryName} (τ={TransmittanceTau:F2})";
    }

    /// <summary>
    /// Профил на препятствие
    /// </summary>
    public class ObstacleProfile
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double[] MonthlyFactors { get; set; } = new double[12]; // F_sh,obst[m]
    }

    /// <summary>
    /// Тип на слънцезащита
    /// </summary>
    public enum ShadingLocation
    {
        [Description("Без")]
        None,
        [Description("Вътрешна")]
        Internal,
        [Description("Външна")]
        External
    }
}
