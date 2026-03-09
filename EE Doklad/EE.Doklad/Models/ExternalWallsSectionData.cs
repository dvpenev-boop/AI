using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    // Типове за секция "Външни стени".
    /// <summary>
    /// Данни за раздел "Външни стени"
    /// </summary>
    public partial class ExternalWallsSectionData : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Външни стени";

        [ObservableProperty]
        private string? _description;

    [ObservableProperty]
    private bool _showFacadeDistribution = true;

        public ObservableCollection<ExternalWallType> WallTypes { get; } = new();
    }

    /// <summary>
    /// Тип външна стена
    /// </summary>
    public partial class ExternalWallType : ObservableObject
    {
        private const double DefaultRsi = 0.13;
        private const double DefaultRse = 0.04;

        [ObservableProperty]
        private int _index;

        [ObservableProperty]
        private string _name = "Тип стена";

        public double Area
        {
            get
            {
                return FacadeEast + FacadeNorth + FacadeWest + FacadeSouth + FacadeNorthEast + FacadeNorthWest + FacadeSouthWest + FacadeSouthEast;
            }
        }

    [ObservableProperty]
    private double _facadeEast;

    [ObservableProperty]
    private double _facadeNorth;

    [ObservableProperty]
    private double _facadeWest;

    [ObservableProperty]
    private double _facadeSouth;

    [ObservableProperty]
    private double _facadeNorthEast;

    [ObservableProperty]
    private double _facadeNorthWest;

    [ObservableProperty]
    private double _facadeSouthWest;

    [ObservableProperty]
    private double _facadeSouthEast;
        partial void OnFacadeEastChanged(double value)
        {
            OnPropertyChanged(nameof(Area));
            NotifySurfaceEffProperties();
        }
        partial void OnFacadeNorthChanged(double value)
        {
            OnPropertyChanged(nameof(Area));
            NotifySurfaceEffProperties();
        }
        partial void OnFacadeWestChanged(double value)
        {
            OnPropertyChanged(nameof(Area));
            NotifySurfaceEffProperties();
        }
        partial void OnFacadeSouthChanged(double value)
        {
            OnPropertyChanged(nameof(Area));
            NotifySurfaceEffProperties();
        }
        partial void OnFacadeNorthEastChanged(double value)
        {
            OnPropertyChanged(nameof(Area));
            NotifySurfaceEffProperties();
        }
        partial void OnFacadeNorthWestChanged(double value)
        {
            OnPropertyChanged(nameof(Area));
            NotifySurfaceEffProperties();
        }
        partial void OnFacadeSouthWestChanged(double value)
        {
            OnPropertyChanged(nameof(Area));
            NotifySurfaceEffProperties();
        }
        partial void OnFacadeSouthEastChanged(double value)
        {
            OnPropertyChanged(nameof(Area));
            NotifySurfaceEffProperties();
        }

        [ObservableProperty]
        private double _rsi = DefaultRsi;

        [ObservableProperty]
        private double _rse = DefaultRse;

        [ObservableProperty]
        private AttachmentData? _schemeAttachment = new();

        /// <summary>
        /// Surface optical/thermal properties (α_sol, ε).
        /// Initialized with safe defaults so older projects without this data still work.
        /// </summary>
        [ObservableProperty]
        private WallSurfaceProperties _surfaceProperties = new();

        /// <summary>
        /// Настройки за топлинни мостове за този тип стена.
        /// </summary>
        [ObservableProperty]
        private WallThermalBridgeSettings _thermalBridges = new();

        partial void OnSurfacePropertiesChanged(WallSurfaceProperties? oldValue, WallSurfaceProperties newValue)
        {
            if (oldValue != null)
                oldValue.PropertyChanged -= SurfaceProperties_PropertyChanged;
            if (newValue != null)
                newValue.PropertyChanged += SurfaceProperties_PropertyChanged;
            NotifySurfaceEffProperties();
        }

        private void SurfaceProperties_PropertyChanged(object? sender, PropertyChangedEventArgs e)
            => NotifySurfaceEffProperties();

        private void NotifySurfaceEffProperties()
        {
            OnPropertyChanged(nameof(AlphaEff));
            OnPropertyChanged(nameof(EpsilonEff));
        }

        // ── Orientation → area mapping ──────────────────────────────────────
        private static readonly (WallOrientation Orientation, Func<ExternalWallType, double> GetArea)[] _orientationAreas =
        {
            (WallOrientation.NE, w => w.FacadeNorthEast),
            (WallOrientation.E,  w => w.FacadeEast),
            (WallOrientation.SE, w => w.FacadeSouthEast),
            (WallOrientation.S,  w => w.FacadeSouth),
            (WallOrientation.SW, w => w.FacadeSouthWest),
            (WallOrientation.W,  w => w.FacadeWest),
            (WallOrientation.NW, w => w.FacadeNorthWest),
        };

        /// <summary>
        /// Effective solar absorptance α_eff.
        /// Weighted average over orientations when overrides are active; fallback to AlphaDefault.
        /// </summary>
        public double AlphaEff
        {
            get
            {
                var sp = SurfaceProperties;
                if (!sp.UseOrientationOverride) return sp.AlphaDefault;
                double sumA = 0, sumAa = 0;
                foreach (var (o, getArea) in _orientationAreas)
                {
                    double a = getArea(this);
                    sumA  += a;
                    sumAa += a * sp.GetAlpha(o);
                }
                return sumA > 0 ? sumAa / sumA : sp.AlphaDefault;
            }
        }

        /// <summary>
        /// Effective thermal emissivity ε_eff.
        /// Weighted average over orientations when overrides are active; fallback to EpsilonDefault.
        /// </summary>
        public double EpsilonEff
        {
            get
            {
                var sp = SurfaceProperties;
                if (!sp.UseOrientationOverride) return sp.EpsilonDefault;
                double sumA = 0, sumAe = 0;
                foreach (var (o, getArea) in _orientationAreas)
                {
                    double a = getArea(this);
                    sumA  += a;
                    sumAe += a * sp.GetEpsilon(o);
                }
                return sumA > 0 ? sumAe / sumA : sp.EpsilonDefault;
            }
        }

        public ObservableCollection<ExternalWallLayer> Layers { get; } = new();

        public double Rw => Layers.Sum(layer => layer.R);

        public double Rtotal => Rw + Rsi + Rse;

        public double Uw => Rtotal > 0 ? 1.0 / Rtotal : 0;

        public ExternalWallType()
        {
            Layers.CollectionChanged += Layers_CollectionChanged;
            _surfaceProperties.PropertyChanged += SurfaceProperties_PropertyChanged;
        }

        private void Layers_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ExternalWallLayer layer in e.OldItems)
                {
                    layer.PropertyChanged -= Layer_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (ExternalWallLayer layer in e.NewItems)
                {
                    layer.PropertyChanged += Layer_PropertyChanged;
                }
            }

            NotifyCalculatedProperties();
        }

        private void Layer_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExternalWallLayer.Material))
            {
                return;
            }

            NotifyCalculatedProperties();
        }

        partial void OnRsiChanged(double value) => NotifyCalculatedProperties();

        partial void OnRseChanged(double value) => NotifyCalculatedProperties();

        private void NotifyCalculatedProperties()
        {
            OnPropertyChanged(nameof(Rw));
            OnPropertyChanged(nameof(Rtotal));
            OnPropertyChanged(nameof(Uw));
        }
    }

    /// <summary>
    /// Слой на външна стена
    /// </summary>
    public partial class ExternalWallLayer : ObservableObject
    {
        [ObservableProperty]
        private string _material = string.Empty;

        [ObservableProperty]
        private string? _selectedMaterialId;

        [ObservableProperty]
        private double _thickness;

        [ObservableProperty]
        private double _lambda;

        public double R => Lambda > 0 ? Thickness / Lambda : 0;

        partial void OnSelectedMaterialIdChanged(string? value)
        {
            // Auto-fill Lambda when material is selected
            if (!string.IsNullOrEmpty(value) && MaterialOptions != null)
            {
                var option = MaterialOptions.FirstOrDefault(o => o.Id == value);
                if (option != null)
                {
                    Lambda = option.LambdaWmk;
                    Material = option.NameBg;
                }
            }
        }

        partial void OnThicknessChanged(double value) => OnPropertyChanged(nameof(R));

        partial void OnLambdaChanged(double value) => OnPropertyChanged(nameof(R));

        // Reference to material options (set from ViewModel)
        public IReadOnlyList<MaterialOption>? MaterialOptions { get; set; }
    }

    // ──────────────────────────────────────────────────────────
    //  Surface parameters (α_sol / ε) – new optional extension
    // ──────────────────────────────────────────────────────────

    /// <summary>
    /// Cardinal + intercardinal orientations used for per-orientation surface overrides.
    /// </summary>
    public enum WallOrientation
    {
        NE,  // СИ
        E,   // И
        SE,  // ЮИ
        S,   // Ю
        SW,  // ЮЗ
        W,   // З
        NW   // СЗ
    }

    /// <summary>
    /// Per-orientation solar absorptance and thermal emissivity.
    /// </summary>
    public partial class SurfaceProps : ObservableObject
    {
        [ObservableProperty]
        private double _alpha = 0.6;

        [ObservableProperty]
        private double _epsilon = 0.9;
    }

    /// <summary>
    /// Surface optical/thermal properties attached to a wall type.
    /// If <see cref="UseOrientationOverride"/> is <c>false</c>, use
    /// <see cref="AlphaDefault"/> / <see cref="EpsilonDefault"/> for all orientations.
    /// </summary>
    public partial class WallSurfaceProperties : ObservableObject
    {
        [ObservableProperty]
        private double _alphaDefault = 0.6;

        [ObservableProperty]
        private double _epsilonDefault = 0.9;

        /// <summary>
        /// When <c>true</c> the per-orientation values in <see cref="Overrides"/> are used.
        /// </summary>
        [ObservableProperty]
        private bool _useOrientationOverride;

        /// <summary>Controls whether the surface params expander is open in the UI (not persisted).</summary>
        [ObservableProperty]
        private bool _isExpanded;

        /// <summary>Per-orientation overrides (only relevant when <see cref="UseOrientationOverride"/> is true).</summary>
        public Dictionary<WallOrientation, SurfaceProps> Overrides { get; set; } = BuildDefaultOverrides();

        private static Dictionary<WallOrientation, SurfaceProps> BuildDefaultOverrides()
        {
            var dict = new Dictionary<WallOrientation, SurfaceProps>();
            foreach (WallOrientation o in System.Enum.GetValues(typeof(WallOrientation)))
            {
                dict[o] = new SurfaceProps();
            }
            return dict;
        }

        /// <summary>
        /// Returns the effective alpha for a given orientation (respects override flag).
        /// </summary>
        public double GetAlpha(WallOrientation orientation)
            => UseOrientationOverride && Overrides.TryGetValue(orientation, out var p) ? p.Alpha : AlphaDefault;

        /// <summary>
        /// Returns the effective epsilon for a given orientation (respects override flag).
        /// </summary>
        public double GetEpsilon(WallOrientation orientation)
            => UseOrientationOverride && Overrides.TryGetValue(orientation, out var p) ? p.Epsilon : EpsilonDefault;
    }

    // ──────────────────────────────────────────────────────────────────
    //  Thermal bridges (топлинни мостове) – Секция „Външни стени"
    // ──────────────────────────────────────────────────────────────────

    /// <summary>Режим за изчисление на топлинни мостове за един тип стена.</summary>
    public enum ThermalBridgeMode
    {
        None,               // Няма – Htb = 0
        GlobalPercentage,   // Глобална стойност (%)
        Manual              // Ръчно въвеждане на отделни топлинни мостове
    }

    /// <summary>
    /// Един топлинен мост (linear ψ-bridge + point χ-bridge).
    /// </summary>
    public partial class WallThermalBridgeItem : ObservableObject
    {
        [ObservableProperty] private string _id   = System.Guid.NewGuid().ToString();
        [ObservableProperty] private string _wallId = string.Empty;

        /// <summary>Описателен тип / наименование.</summary>
        [ObservableProperty] private string _type = "Топлинен мост";

        /// <summary>Дължина на линейния мост [m].</summary>
        [ObservableProperty] private double _length;

        /// <summary>Линеен коефициент на топлопреминаване ψ [W/(m·K)].</summary>
        [ObservableProperty] private double _psi;

        /// <summary>Точков коефициент χ [W/K].</summary>
        [ObservableProperty] private double _chi;

        // ── Фасади (може повече от една да е отметната) ──
        [ObservableProperty] private bool _facadeNorth;
        [ObservableProperty] private bool _facadeNorthEast;
        [ObservableProperty] private bool _facadeEast;
        [ObservableProperty] private bool _facadeSouthEast;
        [ObservableProperty] private bool _facadeSouth;
        [ObservableProperty] private bool _facadeSouthWest;
        [ObservableProperty] private bool _facadeWest;
        [ObservableProperty] private bool _facadeNorthWest;

        /// <summary>
        /// Брой чекнати фасади. Използва се като множител при изчислението на Htb.
        /// Ако нито една фасада не е чекната, приемаме 1 (мостът важи веднъж).
        /// </summary>
        public int FacadeCount =>
            System.Math.Max(1,
                (FacadeNorth     ? 1 : 0) +
                (FacadeNorthEast ? 1 : 0) +
                (FacadeEast      ? 1 : 0) +
                (FacadeSouthEast ? 1 : 0) +
                (FacadeSouth     ? 1 : 0) +
                (FacadeSouthWest ? 1 : 0) +
                (FacadeWest      ? 1 : 0) +
                (FacadeNorthWest ? 1 : 0));

        /// <summary>L × ψ [W/K] — за един екземпляр на моста.</summary>
        public double LinearLoss => Length * Psi;

        /// <summary>
        /// (L × ψ + χ) × FacadeCount [W/K] — пълна загуба с отчитане на фасадите.
        /// Използва се при „Външни стени".
        /// </summary>
        public double TotalLoss => (LinearLoss + Chi) * FacadeCount;

        /// <summary>
        /// L × ψ + χ [W/K] — загуба БЕЗ множител по фасади.
        /// Използва се при „Покрив" (няма фасади).
        /// </summary>
        public double TotalLossNoFacade => LinearLoss + Chi;

        partial void OnLengthChanged(double value)
        {
            OnPropertyChanged(nameof(LinearLoss));
            OnPropertyChanged(nameof(TotalLoss));
            OnPropertyChanged(nameof(TotalLossNoFacade));
        }
        partial void OnPsiChanged(double value)
        {
            OnPropertyChanged(nameof(LinearLoss));
            OnPropertyChanged(nameof(TotalLoss));
            OnPropertyChanged(nameof(TotalLossNoFacade));
        }
        partial void OnChiChanged(double value)
        {
            OnPropertyChanged(nameof(TotalLoss));
            OnPropertyChanged(nameof(TotalLossNoFacade));
        }

        // Notify FacadeCount и TotalLoss при промяна на всяка фасада
        partial void OnFacadeNorthChanged(bool value)     { OnPropertyChanged(nameof(FacadeCount)); OnPropertyChanged(nameof(TotalLoss)); }
        partial void OnFacadeNorthEastChanged(bool value) { OnPropertyChanged(nameof(FacadeCount)); OnPropertyChanged(nameof(TotalLoss)); }
        partial void OnFacadeEastChanged(bool value)      { OnPropertyChanged(nameof(FacadeCount)); OnPropertyChanged(nameof(TotalLoss)); }
        partial void OnFacadeSouthEastChanged(bool value) { OnPropertyChanged(nameof(FacadeCount)); OnPropertyChanged(nameof(TotalLoss)); }
        partial void OnFacadeSouthChanged(bool value)     { OnPropertyChanged(nameof(FacadeCount)); OnPropertyChanged(nameof(TotalLoss)); }
        partial void OnFacadeSouthWestChanged(bool value) { OnPropertyChanged(nameof(FacadeCount)); OnPropertyChanged(nameof(TotalLoss)); }
        partial void OnFacadeWestChanged(bool value)      { OnPropertyChanged(nameof(FacadeCount)); OnPropertyChanged(nameof(TotalLoss)); }
        partial void OnFacadeNorthWestChanged(bool value) { OnPropertyChanged(nameof(FacadeCount)); OnPropertyChanged(nameof(TotalLoss)); }
    }

    /// <summary>
    /// Настройки за топлинни мостове, прикачени към един тип стена.
    /// </summary>
    public partial class WallThermalBridgeSettings : ObservableObject
    {
        [ObservableProperty] private ThermalBridgeMode _mode = ThermalBridgeMode.None;

        /// <summary>Процент за режим GlobalPercentage (0–10 %).</summary>
        [ObservableProperty] private double _globalPercent = 5.0;

        /// <summary>Дали Expander-ът е разгърнат в UI (не се персистира).</summary>
        [ObservableProperty] private bool _isExpanded;

        /// <summary>Топлинни мостове (само за Manual режим).</summary>
        public ObservableCollection<WallThermalBridgeItem> Items { get; } = new();

        // ── Computed (попълват се от ThermalBridgeCalculator) ──

        /// <summary>Hel = U × A  [W/K].</summary>
        [ObservableProperty] private double _hel;

        /// <summary>Htb [W/K] – принос от термомостовете.</summary>
        [ObservableProperty] private double _htb;

        /// <summary>Htotal = Hel + Htb  [W/K].</summary>
        [ObservableProperty] private double _htotal;

        /// <summary>Брой термомостове (удобно за popup summary).</summary>
        public int BridgeCount => Items.Count;

        partial void OnModeChanged(ThermalBridgeMode value)
        {
            // Нотифицирай UI за промяна на computed свойствата при смяна на режим
            OnPropertyChanged(nameof(BridgeCount));
        }
    }
}
