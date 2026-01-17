using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    public partial class RoofSectionData : ObservableObject
    {
        [ObservableProperty]
        private string _description = string.Empty;

        public ObservableCollection<RoofType> RoofTypes { get; } = new();
        
        // Collections for warm and cold roofs for UI display
        public ObservableCollection<RoofType> WarmRoofs { get; } = new();
        public ObservableCollection<RoofType> ColdRoofs { get; } = new();
    }

    public partial class RoofType : ObservableObject
    {
        [ObservableProperty]
        private int _number;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private RoofMode _mode = RoofMode.Unselected;

        [ObservableProperty]
        private double _area;

        [ObservableProperty]
        private bool _isSeed = true;

        [ObservableProperty]
        private bool _hasPrompted;

    [ObservableProperty]
    private AttachmentData? _schemeAttachment = new();

        [ObservableProperty]
        private WarmRoofDetail? _warmDetail;

        [ObservableProperty]
        private ColdRoofDetail? _coldDetail;

        public bool IsConfigured => Mode != RoofMode.Unselected;

        public string ModeLabel => Mode switch
        {
            RoofMode.Warm => "Топъл",
            RoofMode.Cold => "Студен",
            _ => "НЕИЗБРАН ТИП"
        };

        public string UDisplay => Mode switch
        {
            RoofMode.Warm => WarmDetail == null ? "—" : WarmDetail.Uw.ToString("0.000"),
            RoofMode.Cold => ColdDetail?.IsCalculated == true && ColdDetail.Ur is { } value
                ? value.ToString("0.000")
                : "—",
            _ => "—"
        };

        partial void OnModeChanged(RoofMode value)
        {
            OnPropertyChanged(nameof(IsConfigured));
            OnPropertyChanged(nameof(ModeLabel));
            OnPropertyChanged(nameof(UDisplay));
        }

        partial void OnWarmDetailChanged(WarmRoofDetail? value)
        {
            if (value != null)
            {
                value.PropertyChanged += Detail_PropertyChanged;
            }
            OnPropertyChanged(nameof(UDisplay));
        }

        partial void OnColdDetailChanged(ColdRoofDetail? value)
        {
            if (value != null)
            {
                value.PropertyChanged += Detail_PropertyChanged;
            }
            OnPropertyChanged(nameof(UDisplay));
        }

        private void Detail_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WarmRoofDetail.Uw) ||
                e.PropertyName == nameof(ColdRoofDetail.Ur) ||
                e.PropertyName == nameof(ColdRoofDetail.IsCalculated))
            {
                OnPropertyChanged(nameof(UDisplay));
            }
        }
    }

    public enum RoofMode
    {
        Unselected,
        Warm,
        Cold
    }

    public partial class WarmRoofDetail : ObservableObject
    {
        public ObservableCollection<RoofLayer> Layers { get; } = new();

        [ObservableProperty]
        private double _rsi = 0.13;

        [ObservableProperty]
        private double _rse = 0.04;

        public double Rw => Layers.Sum(layer => layer.R);

        public double Rtotal => Rw + Rsi + Rse;

        public double Uw => Rtotal > 0 ? 1.0 / Rtotal : 0;

        public WarmRoofDetail()
        {
            Layers.CollectionChanged += Layers_CollectionChanged;
        }

        private void Layers_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (RoofLayer layer in e.OldItems)
                {
                    layer.PropertyChanged -= Layer_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (RoofLayer layer in e.NewItems)
                {
                    layer.PropertyChanged += Layer_PropertyChanged;
                }
            }

            NotifyCalculatedProperties();
        }

        private void Layer_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RoofLayer.Material))
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

    public partial class ColdRoofDetail : ObservableObject
    {
        /// <summary>
        /// Кинематичен вискозитет на въздуха (ν), изчислен по методиката (Sutherland's formula)
        /// </summary>
        public double? KinematicViscosity
        {
            get
            {
                // Температура в K
                if (ThetaU == null) return null;
                double T = ThetaU.Value + 273.15;
                // Sutherland's formula for air (μ in kg/(m·s))
                double mu = 1.458e-6 * Math.Pow(T, 1.5) / (T + 110.4); // динамичен вискозитет
                double rho = 101325 / (287.058 * T); // плътност на въздуха (p=101325 Pa, R=287.058 J/kgK)
                return mu / rho; // кинематичен вискозитет ν = μ/ρ
            }
        }

        /// <summary>
        /// Топлопроводност на въздуха (λ), изчислена по методиката (примерна формула)
        /// </summary>
        public double? LambdaAir
        {
            get
            {
                if (ThetaU == null) return null;
                double T = ThetaU.Value + 273.15;
                // Примерна формула за λ на въздуха (W/mK)
                return 2.334e-3 + 7.322e-5 * T; // λ = 0.002334 + 0.00007322*T
            }
        }

        /// <summary>
        /// Критерий на Прандтъл (Pr), изчислен по методиката
        /// </summary>
        public double? Prandtl
        {
            get
            {
                if (ThetaU == null) return null;
                double T = ThetaU.Value + 273.15;
                // Cp = 1005 J/kgK (прибл.), μ и λ както по-горе
                double mu = 1.458e-6 * Math.Pow(T, 1.5) / (T + 110.4);
                double lambda = 2.334e-3 + 7.322e-5 * T;
                double Cp = 1005.0;
                double Pr = (mu * Cp) / lambda;
                return Pr;
            }
        }

        /// <summary>
        /// Коефициент на обемно разширение β
        /// </summary>
        public double? Beta
        {
            get
            {
                if (ThetaU == null) return null;
                double T = ThetaU.Value + 273.15;
                return 1.0 / T;
            }
        }

        /// <summary>
        /// Критерий на Грасхоф (Gr)
        /// </summary>
        public double? Grashof
        {
            get
            {
                if (ThetaU == null || ThetaSe1 == null || ThetaSi2 == null || Deltavc == null || KinematicViscosity == null || Beta == null) return null;
                double g = 9.81;
                double delta = Deltavc.Value;
                double dT = ThetaSe1.Value - ThetaSi2.Value;
                double v = KinematicViscosity.Value;
                double beta = Beta.Value;
                return (g * beta * Math.Pow(delta, 3) * dT) / (v * v);
            }
        }

        /// <summary>
        /// Gr.Pr произведение
        /// </summary>
        public double? GrPr
        {
            get
            {
                if (Grashof == null || Prandtl == null) return null;
                return Grashof.Value * Prandtl.Value;
            }
        }

        /// <summary>
        /// Корекционен коефициент εk
        /// </summary>
        public double? EpsilonK
        {
            get
            {
                if (GrPr == null) return null;
                double grpr = GrPr.Value;
                if (grpr < 1e3) return 1.0;
                if (grpr < 1e6) return 0.105 * Math.Pow(grpr, 0.25);
                if (grpr < 1e10) return 0.4 * Math.Pow(grpr, 0.25);
                return null;
            }
        }

        /// <summary>
        /// Еквивалентен коефициент на топлопроводност на въздушния слой λекв
        /// </summary>
        public double? LambdaEk
        {
            get
            {
                if (LambdaAir == null || EpsilonK == null) return null;
                return LambdaAir.Value * EpsilonK.Value;
            }
    }
        /// <summary>
        /// Температура на повърхността, граничеща с въздушния слой в подкровното пространство (откъм сградата)
        /// </summary>
        public double? ThetaSe1
        {
            get
            {
                if (ThetaU == null || (U1?.Uw ?? 0) <= 0) return null;
                return ThetaU + 0.1 * (U1?.Uw ?? 0) * (Ti - ThetaU.Value);
            }
        }

        /// <summary>
        /// Температура на повърхността, граничеща с въздушния слой в подкровното пространство (откъм външния въздух)
        /// </summary>
        public double? ThetaSi2
        {
            get
            {
                if (ThetaU == null || (U2?.Uw ?? 0) <= 0) return null;
                return ThetaU - 0.17 * (U2?.Uw ?? 0) * (ThetaU.Value - Te);
            }
        }
        // Позволява ръчно въвеждане на Te
        [ObservableProperty]
        private bool _manualTeInput = false;
        // 5.1 Geometry
        [ObservableProperty]
        private double _vp;

        [ObservableProperty]
        private double _ap;

    public double? Deltavc => (Ap > 0) ? Vp / Ap : null;
        // 5.2 Areas
        [ObservableProperty]
        private double _a1;

        [ObservableProperty]
        private double _a2;

        [ObservableProperty]
        private double _aw;
        // 5.3 Ventilation
        [ObservableProperty]
        private ColdRoofSpaceType _spaceType = ColdRoofSpaceType.Sealed;

        [ObservableProperty]
        private double _n = 0.1;

        [ObservableProperty]
        private double _v;
        // 5.4 Temperatures
        [ObservableProperty]
        private double _ti;

        [ObservableProperty]
        private double _te;
        // 5.5 Constructions
        public RoofLayerTable U1 { get; set; } = new();
        public RoofLayerTable U2 { get; set; } = new();
        public RoofLayerTable Uw { get; set; } = new();

        [ObservableProperty]
        private double? _ur;

        [ObservableProperty]
        private bool _isCalculated;

        public ColdRoofDetail()
        {
            U1.PropertyChanged += LayerTable_PropertyChanged;
            U2.PropertyChanged += LayerTable_PropertyChanged;
            Uw.PropertyChanged += LayerTable_PropertyChanged;
        }

        /// <summary>
        /// Температура на въздуха в подкровното пространство (Θu)
        /// </summary>
        public double? ThetaU
        {
            get
            {
                // Проверка за валидност на данните
                if ((U1?.Uw ?? 0) <= 0 && (U2?.Uw ?? 0) <= 0 && (Uw?.Uw ?? 0) <= 0) return null;
                double a1 = A1;
                double a2 = A2;
                double aw = Aw;
                double u1 = U1?.Uw ?? 0;
                double u2 = U2?.Uw ?? 0;
                double uw = Uw?.Uw ?? 0;
                double n = N;
                double v = V;
                double ti = Ti;
                double te = Te;
                double numerator = ti * u1 * a1 + te * u2 * a2 + te * uw * aw + te * 0.33 * n * v;
                double denominator = u1 * a1 + u2 * a2 + uw * aw + 0.33 * n * v;
                return denominator != 0 ? numerator / denominator : (double?)null;
            }
        }

        private void LayerTable_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RoofLayerTable.Uw))
            {
                InvalidateCalculation();
            }
            // При всяка промяна на слоевете, преизчисляваме ThetaU
            OnPropertyChanged(nameof(ThetaU));
            OnPropertyChanged(nameof(ThetaSe1));
            OnPropertyChanged(nameof(ThetaSi2));
        }

        partial void OnVpChanged(double value)
        {
            OnPropertyChanged(nameof(Deltavc));
            InvalidateCalculation();
            OnPropertyChanged(nameof(ThetaU));
            OnPropertyChanged(nameof(ThetaSe1));
            OnPropertyChanged(nameof(ThetaSi2));
        }

        partial void OnApChanged(double value)
        {
            OnPropertyChanged(nameof(Deltavc));
            InvalidateCalculation();
            OnPropertyChanged(nameof(ThetaU));
            OnPropertyChanged(nameof(ThetaSe1));
            OnPropertyChanged(nameof(ThetaSi2));
        }


    partial void OnA1Changed(double value) { InvalidateCalculation(); OnPropertyChanged(nameof(ThetaU)); OnPropertyChanged(nameof(ThetaSe1)); OnPropertyChanged(nameof(ThetaSi2)); }
    partial void OnA2Changed(double value) { InvalidateCalculation(); OnPropertyChanged(nameof(ThetaU)); OnPropertyChanged(nameof(ThetaSe1)); OnPropertyChanged(nameof(ThetaSi2)); }
    partial void OnAwChanged(double value) { InvalidateCalculation(); OnPropertyChanged(nameof(ThetaU)); OnPropertyChanged(nameof(ThetaSe1)); OnPropertyChanged(nameof(ThetaSi2)); }

        partial void OnSpaceTypeChanged(ColdRoofSpaceType value)
        {
            // Автоматично задаване на n според избрания тип
            N = value == ColdRoofSpaceType.Sealed ? 0.1 : 0.3;
            InvalidateCalculation();
            OnPropertyChanged(nameof(ThetaU));
            OnPropertyChanged(nameof(ThetaSe1));
            OnPropertyChanged(nameof(ThetaSi2));
        }


    partial void OnNChanged(double value) { InvalidateCalculation(); OnPropertyChanged(nameof(ThetaU)); OnPropertyChanged(nameof(ThetaSe1)); OnPropertyChanged(nameof(ThetaSi2)); }
    partial void OnVChanged(double value) { InvalidateCalculation(); OnPropertyChanged(nameof(ThetaU)); OnPropertyChanged(nameof(ThetaSe1)); OnPropertyChanged(nameof(ThetaSi2)); }
    partial void OnTiChanged(double value) { InvalidateCalculation(); OnPropertyChanged(nameof(ThetaU)); OnPropertyChanged(nameof(ThetaSe1)); OnPropertyChanged(nameof(ThetaSi2)); }
    partial void OnTeChanged(double value) { InvalidateCalculation(); OnPropertyChanged(nameof(ThetaU)); OnPropertyChanged(nameof(ThetaSe1)); OnPropertyChanged(nameof(ThetaSi2)); }

        public void CalculateUr()
        {
            var uValues = new[] { U1.Uw, U2.Uw, Uw.Uw }.Where(value => value > 0).ToList();
            Ur = uValues.Count > 0 ? uValues.Average() : null;
            IsCalculated = uValues.Count > 0;
            OnPropertyChanged(nameof(Ur));
        }

        private void InvalidateCalculation()
        {
            IsCalculated = false;
            Ur = null;
            OnPropertyChanged(nameof(Ur));
        }
    }

    public enum ColdRoofSpaceType
    {
        Sealed, // уплътнено
        Unsealed // неуплътнено
    }

    public partial class RoofLayerTable : ObservableObject
    {
        public ObservableCollection<RoofLayer> Layers { get; } = new();

        [ObservableProperty]
        private double _rsi = 0.13;

        [ObservableProperty]
        private double _rse = 0.04;

        [ObservableProperty]
        private bool _rsiEditable = true;

        [ObservableProperty]
        private bool _rseEditable = true;

        public double Rw => Layers.Sum(layer => layer.R);

        public double Rtotal => Rw + Rsi + Rse;

        public double Uw => Rtotal > 0 ? 1.0 / Rtotal : 0;

        public RoofLayerTable()
        {
            Layers.CollectionChanged += Layers_CollectionChanged;
        }

        private void Layers_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (RoofLayer layer in e.OldItems)
                {
                    layer.PropertyChanged -= Layer_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (RoofLayer layer in e.NewItems)
                {
                    layer.PropertyChanged += Layer_PropertyChanged;
                }
            }

            NotifyCalculatedProperties();
        }

        private void Layer_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RoofLayer.Material))
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

    public partial class RoofLayer : ObservableObject
    {
        [ObservableProperty]
        private string _material = string.Empty;

        [ObservableProperty]
        private double _thickness;

        [ObservableProperty]
        private double _lambda;

        public double R => Lambda > 0 ? Thickness / Lambda : 0;

        partial void OnThicknessChanged(double value) => OnPropertyChanged(nameof(R));

        partial void OnLambdaChanged(double value) => OnPropertyChanged(nameof(R));
    }
}
