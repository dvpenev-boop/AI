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

        /// <summary>
        /// Surface optical/thermal properties (α_sol, ε) for this roof type.
        /// UseOrientationOverride is always false for roofs (no orientation logic).
        /// Initialized with defaults (α=0.6, ε=0.9) for backward compatibility.
        /// </summary>
        [ObservableProperty]
        private WallSurfaceProperties _surfaceProperties = new() { UseOrientationOverride = false };

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
        // --- U1/U2 за изчисление на θse1 и θsi2 (фиксирани съпротивления) ---
        public double? U1ForTheta
        {
            get
            {
                // За таванска плоча (U1) при θse1: Rsi = 0.10, Rse = 0.17
                double sumLayers = U1.Layers.Sum(l => l.R);
                double denominator = 0.10 + sumLayers + 0.10;
                return denominator > 0 ? 1.0 / denominator : null;
            }
        }

        public double? U2ForTheta
        {
            get
            {
                // За покривна плоча (U2) при θsi2: Rsi = 0.04, Rse = 0.10
                double sumLayers = U2.Layers.Sum(l => l.R);
                double denominator = 0.04 + sumLayers + 0.10;
                return denominator > 0 ? 1.0 / denominator : null;
            }
        }

        // --- Реални U1/U2 с изчислени съпротивления (за Ur и доклада) ---
        public double? U1Actual
        {
            get
            {
                double sumLayers = U1.Layers.Sum(l => l.R);
                double rse1 = Rse1Rsi2 ?? 0.0;
                double denominator = 0.10 + sumLayers + rse1;
                return denominator > 0 ? 1.0 / denominator : null;
            }
        }

        public double? U2Actual
        {
            get
            {
                double sumLayers = U2.Layers.Sum(l => l.R);
                double rsi2 = Rse1Rsi2 ?? 0.0;
                double denominator = rsi2 + sumLayers + 0.04;
                return denominator > 0 ? 1.0 / denominator : null;
            }
        }

        // --- Помощни методи за изчисление на θse1 и θsi2 ---
        public double? ThetaSe1Fixed
        {
            get
            {
                if (ThetaU == null || U1ForTheta == null) return null;
                return ThetaU + 0.1 * U1ForTheta.Value * (Ti - ThetaU.Value);
            }
        }

        public double? ThetaSi2Fixed
        {
            get
            {
                if (ThetaU == null || U2ForTheta == null) return null;
                return ThetaU - 0.17 * U2ForTheta.Value * (ThetaU.Value - Te);
            }
        }

        // Оставяме старите ThetaSe1/ThetaSi2 за съвместимост, но ги насочваме към новите фиксирани варианти
        public double? ThetaSe1 => ThetaSe1Fixed;
        public double? ThetaSi2 => ThetaSi2Fixed;
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

        /// <summary>
        /// Изчислена стойност на Rse1 = Rsi2 (за визуализация)
        /// </summary>
        public double? Rse1Rsi2 => CalculateRse1Rsi2();

        /// <summary>
        /// Изчислена стойност на U1 (за визуализация)
        /// </summary>
        public double? U1Calculated => CalculateU1();

        /// <summary>
        /// Изчислена стойност на U2 (за визуализация)
        /// </summary>
        public double? U2Calculated => CalculateU2();

        /// <summary>
        /// Изчислена стойност на Uw (за визуализация)
        /// </summary>
        public double? UwCalculated => CalculateUw();

        public ColdRoofDetail()
        {
            U1.PropertyChanged += LayerTable_PropertyChanged;
            U2.PropertyChanged += LayerTable_PropertyChanged;
            Uw.PropertyChanged += LayerTable_PropertyChanged;
            
            // Настройка на началните стойности за U1, U2, Uw
            U1.Rsi = 0.10;
            U1.RsiEditable = false;
            U1.Rse = 0.0; // Ще се изчисли динамично
            U1.RseEditable = false;
            
            U2.Rsi = 0.0; // Ще се изчисли динамично
            U2.RsiEditable = false;
            U2.Rse = 0.04;
            U2.RseEditable = false;
            
            Uw.Rsi = 0.13;
            Uw.RsiEditable = false;
            Uw.Rse = 0.04;
            Uw.RseEditable = false;
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

        /// <summary>
        /// Актуализира Rse1 и Rsi2 стойностите в таблиците U1 и U2
        /// </summary>
        private void UpdateResistanceValues()
        {
            double? rse1rsi2 = CalculateRse1Rsi2();
            if (rse1rsi2.HasValue && rse1rsi2.Value > 0)
            {
                U1.Rse = rse1rsi2.Value;
                U2.Rsi = rse1rsi2.Value;
            }
            else
            {
                U1.Rse = 0.0;
                U2.Rsi = 0.0;
            }
            // Уведомяванията се правят от CalculateAll()
        }

        private void LayerTable_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RoofLayerTable.Uw))
            {
                InvalidateCalculation();
            }
            // Обнови U1ForTheta и U2ForTheta при промяна на слоевете
            OnPropertyChanged(nameof(U1ForTheta));
            OnPropertyChanged(nameof(U2ForTheta));
        }

        partial void OnVpChanged(double value)
        {
            OnPropertyChanged(nameof(Deltavc));
            InvalidateCalculation();
        }

        partial void OnApChanged(double value)
        {
            OnPropertyChanged(nameof(Deltavc));
            InvalidateCalculation();
        }


    partial void OnA1Changed(double value) { InvalidateCalculation(); }
    partial void OnA2Changed(double value) { InvalidateCalculation(); }
    partial void OnAwChanged(double value) { InvalidateCalculation(); }

        partial void OnSpaceTypeChanged(ColdRoofSpaceType value)
        {
            // Автоматично задаване на n според избрания тип
            N = value == ColdRoofSpaceType.Sealed ? 0.1 : 0.3;
            InvalidateCalculation();
        }


    partial void OnNChanged(double value) { InvalidateCalculation(); }
    partial void OnVChanged(double value) { InvalidateCalculation(); }
    partial void OnTiChanged(double value) { InvalidateCalculation(); }
    partial void OnTeChanged(double value) { InvalidateCalculation(); }

        /// <summary>
        /// Извършва всички изчисления и актуализира всички изчислени стойности.
        /// Този метод се извиква при натискане на бутона "Изчисли".
        /// </summary>
        public void CalculateAll()
        {
            // Актуализирай съпротивленията
            UpdateResistanceValues();

            // Уведоми за промяна на всички изчислени свойства
            OnPropertyChanged(nameof(ThetaU));
            OnPropertyChanged(nameof(ThetaSe1));
            OnPropertyChanged(nameof(ThetaSi2));
            OnPropertyChanged(nameof(KinematicViscosity));
            OnPropertyChanged(nameof(LambdaAir));
            OnPropertyChanged(nameof(Prandtl));
            OnPropertyChanged(nameof(Beta));
            OnPropertyChanged(nameof(Grashof));
            OnPropertyChanged(nameof(GrPr));
            OnPropertyChanged(nameof(EpsilonK));
            OnPropertyChanged(nameof(LambdaEk));
            OnPropertyChanged(nameof(Rse1Rsi2));
            OnPropertyChanged(nameof(U1Calculated));
            OnPropertyChanged(nameof(U2Calculated));
            OnPropertyChanged(nameof(UwCalculated));

            // Изчисли Ur
            CalculateUr();
        }

        /// <summary>
        /// Изчисляване на Rse1 = Rsi2 по формула: δвс / (2 * λекв)
        /// </summary>
        public double? CalculateRse1Rsi2()
        {
            if (Deltavc == null || LambdaEk == null || LambdaEk <= 0) return null;
            return Deltavc.Value / (2.0 * LambdaEk.Value);
        }

        /// <summary>
        /// Изчисляване на U1 (реален) по формула: 1 / (0.10 + Σ(δj/λj) + Rse1)
        /// </summary>
        public double? CalculateU1() => U1Actual;

        /// <summary>
        /// Изчисляване на U2 (реален) по формула: 1 / (Rsi2 + Σ(δj/λj) + 0.04)
        /// </summary>
        public double? CalculateU2() => U2Actual;

        /// <summary>
        /// Изчисляване на Uw по формула: 1 / (Σ(δj/λj) + 0.13 + 0.04)
        /// </summary>
        public double? CalculateUw()
        {
            // Σ(δj/λj) за всички слоеве в Uw
            double sumLayers = Uw.Layers.Sum(layer => layer.R);
            
            double denominator = sumLayers + 0.13 + 0.04;
            return denominator > 0 ? 1.0 / denominator : null;
        }

        /// <summary>
        /// Изчисляване на Ur по формула: 1 / (1/U1 + A1/(A2*U2 + Aw*Uw + 0.33*n*V))
        /// </summary>
        public void CalculateUr()
        {
            double? u1 = CalculateU1();
            double? u2 = CalculateU2();
            double? uw = CalculateUw();

            if (u1 == null || u2 == null || uw == null || 
                u1 <= 0 || u2 <= 0 || uw <= 0 || 
                A1 <= 0 || A2 <= 0 || Aw <= 0)
            {
                InvalidateCalculation();
                return;
            }

            // Изчисление на знаменателя: A2*U2 + Aw*Uw + 0.33*n*V
            double innerDenominator = (A2 * u2.Value) + (Aw * uw.Value) + (0.33 * N * V);
            
            if (innerDenominator <= 0)
            {
                InvalidateCalculation();
                return;
            }

            // Изчисление на: 1/U1 + A1/(A2*U2 + Aw*Uw + 0.33*n*V)
            double sum = (1.0 / u1.Value) + (A1 / innerDenominator);
            
            if (sum <= 0)
            {
                InvalidateCalculation();
                return;
            }

            // Изчисление на Ur
            Ur = 1.0 / sum;
            IsCalculated = true;
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
}
