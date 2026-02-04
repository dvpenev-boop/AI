using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.ViewModels
{
    public partial class UnconditionedZonesSectionViewModel : ObservableObject
    {
        private readonly UnconditionedZoneSectionData _data;
        private readonly MaterialsService _materialsService;
        private readonly UnconditionedZonesCalculator _calculator;

        [ObservableProperty]
        private ZtuZone? _selectedZone;

        [ObservableProperty]
        private ZtuMonthlyResults? _calculationResults;

        [ObservableProperty]
        private System.Collections.Generic.List<ZtuElementInfluence>? _elementInfluences;

        [ObservableProperty]
        private double _indoorTemperatureC = 20.0;

        public ObservableCollection<MaterialOption> MaterialOptions { get; } = new();

        public UnconditionedZonesSectionViewModel(UnconditionedZoneSectionData data)
        {
            _data = data;
            _materialsService = new MaterialsService(new JsonMaterialsRepository());
            _calculator = new UnconditionedZonesCalculator();
            LoadMaterialOptions();

            // Attach handlers to zones collection
            _data.Zones.CollectionChanged += Zones_CollectionChanged;
        }

        private void Zones_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // When zones are added or removed, attach/detach property change handlers
            if (e.OldItems != null)
            {
                foreach (ZtuZone zone in e.OldItems)
                {
                    zone.ElementsToExternal.CollectionChanged -= Elements_CollectionChanged;
                    zone.ElementsToBoundary.CollectionChanged -= Elements_CollectionChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (ZtuZone zone in e.NewItems)
                {
                    zone.ElementsToExternal.CollectionChanged += Elements_CollectionChanged;
                    zone.ElementsToBoundary.CollectionChanged += Elements_CollectionChanged;
                    InjectMaterialOptionsIntoElements(zone);
                }
            }
        }

        private void Elements_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ZtuElement element in e.NewItems)
                {
                    InjectMaterialOptionsIntoElement(element);
                    element.Layers.CollectionChanged += Layers_CollectionChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (ZtuElement element in e.OldItems)
                {
                    element.Layers.CollectionChanged -= Layers_CollectionChanged;
                }
            }
        }

        private void Layers_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // When layers change, recalculate U-value for the parent element
            if (sender is ObservableCollection<ZtuLayer> layers)
            {
                // Find parent element
                var element = FindElementByLayers(layers);
                if (element != null)
                {
                    RecalculateUValue(element);
                }
            }
        }

        private ZtuElement? FindElementByLayers(ObservableCollection<ZtuLayer> layers)
        {
            foreach (var zone in _data.Zones)
            {
                var element = zone.ElementsToExternal.FirstOrDefault(e => e.Layers == layers);
                if (element != null) return element;

                element = zone.ElementsToBoundary.FirstOrDefault(e => e.Layers == layers);
                if (element != null) return element;
            }
            return null;
        }

        private void LoadMaterialOptions()
        {
            MaterialOptions.Clear();
            var options = _materialsService.GetMaterialOptionsFlattened();
            foreach (var option in options)
            {
                MaterialOptions.Add(option);
            }
        }

        private void InjectMaterialOptionsIntoElements(ZtuZone zone)
        {
            foreach (var element in zone.ElementsToExternal)
            {
                InjectMaterialOptionsIntoElement(element);
            }

            foreach (var element in zone.ElementsToBoundary)
            {
                InjectMaterialOptionsIntoElement(element);
            }
        }

        private void InjectMaterialOptionsIntoElement(ZtuElement element)
        {
            var options = MaterialOptions.ToList() as System.Collections.Generic.IReadOnlyList<MaterialOption>;
            
            foreach (var layer in element.Layers)
            {
                layer.MaterialOptions = options;
                layer.PropertyChanged += Layer_PropertyChanged;
            }
        }

        private void Layer_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is ZtuLayer layer && (e.PropertyName == nameof(ZtuLayer.Thickness) || e.PropertyName == nameof(ZtuLayer.Lambda)))
            {
                // Find parent element and recalculate U
                var element = FindElementByLayer(layer);
                if (element != null)
                {
                    RecalculateUValue(element);
                }
            }
        }

        private ZtuElement? FindElementByLayer(ZtuLayer layer)
        {
            foreach (var zone in _data.Zones)
            {
                var element = zone.ElementsToExternal.FirstOrDefault(e => e.Layers.Contains(layer));
                if (element != null) return element;

                element = zone.ElementsToBoundary.FirstOrDefault(e => e.Layers.Contains(layer));
                if (element != null) return element;
            }
            return null;
        }

        /// <summary>
        /// Изчисляване на U-value за елемент според EN ISO 6946
        /// </summary>
        private void RecalculateUValue(ZtuElement element)
        {
            if (element.Layers.Count == 0)
            {
                element.UValue = 0.0;
                return;
            }

            // Rsi според вида детайл
            double rsi = element.Kind switch
            {
                ElementKind.Wall => 0.13,   // Вертикална стена
                ElementKind.Roof => 0.10,   // Покрив/таван (топлина нагоре)
                ElementKind.Floor => 0.17,  // Под (топлина надолу)
                _ => 0.13
            };

            // За boundary към ZTU: Rsi от двете страни
            // За boundary към external: Rsi + Rse
            double rse = element.IsToExternalEnvironment ? 0.04 : rsi;

            // Сума на термичните съпротивления на слоевете
            double sumR = element.Layers.Sum(layer => layer.R);

            // Обща термична съпротивление
            double rTotal = rsi + sumR + rse;

            // U = 1 / R_total
            element.UValue = rTotal > 0 ? 1.0 / rTotal : 0.0;
        }

        [RelayCommand]
        private void AddZone()
        {
            var newZone = new ZtuZone
            {
                Name = $"Зона {_data.Zones.Count + 1}",
                Type = ZtuType.External,
                Notes = string.Empty
            };

            _data.Zones.Add(newZone);
            SelectedZone = newZone;
        }

        [RelayCommand]
        private void DeleteZone()
        {
            if (SelectedZone == null) return;

            var index = _data.Zones.IndexOf(SelectedZone);
            _data.Zones.Remove(SelectedZone);

            // Select next zone or previous
            if (_data.Zones.Count > 0)
            {
                SelectedZone = index < _data.Zones.Count ? _data.Zones[index] : _data.Zones[_data.Zones.Count - 1];
            }
            else
            {
                SelectedZone = null;
            }
        }

        [RelayCommand]
        private void AddElementToExternal()
        {
            if (SelectedZone == null) return;

            // Тук ще се отваря dialog за добавяне на елемент
            // За момента добавяме placeholder
            var newElement = new ZtuElement
            {
                Name = $"Елемент {SelectedZone.ElementsToExternal.Count + 1}",
                Kind = ElementKind.Wall,
                Area = 0.0,
                IsToExternalEnvironment = true
            };

            SelectedZone.ElementsToExternal.Add(newElement);
        }

        [RelayCommand]
        private void AddElementToBoundary()
        {
            if (SelectedZone == null) return;

            // Тук ще се отваря dialog за добавяне на елемент
            // За момента добавяме placeholder
            var newElement = new ZtuElement
            {
                Name = $"Разделящ елемент {SelectedZone.ElementsToBoundary.Count + 1}",
                Kind = ElementKind.Wall,
                Area = 0.0,
                IsToExternalEnvironment = false
            };

            SelectedZone.ElementsToBoundary.Add(newElement);
        }

        [RelayCommand]
        private void DeleteElementToExternal(ZtuElement element)
        {
            if (SelectedZone == null || element == null) return;
            SelectedZone.ElementsToExternal.Remove(element);
        }

        [RelayCommand]
        private void DeleteElementToBoundary(ZtuElement element)
        {
            if (SelectedZone == null || element == null) return;
            SelectedZone.ElementsToBoundary.Remove(element);
        }

        [RelayCommand]
        private void AddLayer(ZtuElement element)
        {
            if (element == null) return;

            var newLayer = new ZtuLayer
            {
                MaterialName = "Избери материал",
                Thickness = 100.0, // mm
                Lambda = 1.0,      // W/(m·K)
                MaterialOptions = MaterialOptions.ToList()
            };

            newLayer.PropertyChanged += Layer_PropertyChanged;
            element.Layers.Add(newLayer);
            RecalculateUValue(element);
        }

        [RelayCommand]
        private void DeleteLayer((ZtuElement element, ZtuLayer layer) parameters)
        {
            if (parameters.element == null || parameters.layer == null) return;
            
            parameters.element.Layers.Remove(parameters.layer);
            RecalculateUValue(parameters.element);
        }

        /// <summary>
        /// Изчислява месечните параметри на избраната зона
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanCalculate))]
        private void Calculate()
        {
            if (SelectedZone == null) return;

            // За демо целите използваме климатична зона 3 (София)
            // В реална имплементация се вземат от MainViewModel/Document
            var climateService = new ClimateService(new JsonClimateRepository());
            var climateData = climateService.GetZone(3);

            if (climateData == null)
            {
                System.Diagnostics.Debug.WriteLine("Cannot load climate data for zone 3");
                return;
            }

            CalculationResults = _calculator.Calculate(
                SelectedZone,
                climateData,
                IndoorTemperatureC);

            // Изчисляваме влиянието на елементите върху Htr
            if (CalculationResults != null)
            {
                ElementInfluences = _calculator.CalculateInfluenceOnHtr(
                    SelectedZone,
                    CalculationResults);
            }
        }

        private bool CanCalculate()
        {
            return SelectedZone != null
                && (SelectedZone.ElementsToExternal.Any() || SelectedZone.ElementsToBoundary.Any());
        }

        partial void OnSelectedZoneChanged(ZtuZone? value)
        {
            // Обновяваме CanExecute на Calculate когато се смени зоната
            CalculateCommand.NotifyCanExecuteChanged();
        }

        partial void OnIndoorTemperatureCChanged(double value)
        {
            // При промяна на температурата автоматично изчисляваме наново
            if (SelectedZone != null && CalculateCommand.CanExecute(null))
            {
                Calculate();
            }
        }

        public UnconditionedZoneSectionData Data => _data;
    }
}
