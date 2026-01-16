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
        private bool _showFacadeDistribution;

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
        }
        partial void OnFacadeNorthChanged(double value)
        {
            OnPropertyChanged(nameof(Area));
        }
        partial void OnFacadeWestChanged(double value)
        {
            OnPropertyChanged(nameof(Area));
        }
        partial void OnFacadeSouthChanged(double value)
        {
            OnPropertyChanged(nameof(Area));
        }
        partial void OnFacadeNorthEastChanged(double value)
        {
            OnPropertyChanged(nameof(Area));
        }
        partial void OnFacadeNorthWestChanged(double value)
        {
            OnPropertyChanged(nameof(Area));
        }
        partial void OnFacadeSouthWestChanged(double value)
        {
            OnPropertyChanged(nameof(Area));
        }
        partial void OnFacadeSouthEastChanged(double value)
        {
            OnPropertyChanged(nameof(Area));
        }

        [ObservableProperty]
        private double _rsi = DefaultRsi;

        [ObservableProperty]
        private double _rse = DefaultRse;

        [ObservableProperty]
        private AttachmentData? _schemeAttachment = new();

        public ObservableCollection<ExternalWallLayer> Layers { get; } = new();

        public double Rw => Layers.Sum(layer => layer.R);

        public double Rtotal => Rw + Rsi + Rse;

        public double Uw => Rtotal > 0 ? 1.0 / Rtotal : 0;

        public ExternalWallType()
        {
            Layers.CollectionChanged += Layers_CollectionChanged;
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
        private double _thickness;

        [ObservableProperty]
        private double _lambda;

        public double R => Lambda > 0 ? Thickness / Lambda : 0;

        partial void OnThicknessChanged(double value) => OnPropertyChanged(nameof(R));

        partial void OnLambdaChanged(double value) => OnPropertyChanged(nameof(R));
    }
}
