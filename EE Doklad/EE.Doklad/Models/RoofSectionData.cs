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

        private void LayerTable_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RoofLayerTable.Uw))
            {
                InvalidateCalculation();
            }
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

        partial void OnA1Changed(double value) => InvalidateCalculation();

        partial void OnA2Changed(double value) => InvalidateCalculation();

        partial void OnAwChanged(double value) => InvalidateCalculation();

        partial void OnSpaceTypeChanged(ColdRoofSpaceType value) => InvalidateCalculation();

        partial void OnNChanged(double value) => InvalidateCalculation();

        partial void OnVChanged(double value) => InvalidateCalculation();

        partial void OnTiChanged(double value) => InvalidateCalculation();

        partial void OnTeChanged(double value) => InvalidateCalculation();

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
