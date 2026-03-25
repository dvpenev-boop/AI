using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.ViewModels
{
    public class ZtuZoneOverviewRow
    {
        public ZtuZoneOverviewRow(ZtuZone zone)
        {
            Zone = zone;
        }

        public ZtuZone Zone { get; }
        public string ZoneName { get; set; } = string.Empty;
        public string ZoneType { get; set; } = string.Empty;
        public double AreaBoundary_m2 { get; set; }
        public double UextAvg_Wm2K { get; set; }
        public double UintAvg_Wm2K { get; set; }
        public double HztuE_WK { get; set; }
        public double HztcZtu_WK { get; set; }
        public double Bztu { get; set; }
        public double ThetaZtuWinterAvg_C { get; set; }
        public double ThetaZtuSummerAvg_C { get; set; }
        public double Hel_WK { get; set; }
    }

    public partial class UnconditionedZonesSectionViewModel : ObservableObject
    {
        private readonly UnconditionedZoneSectionData _data;
        private readonly MaterialsService _materialsService;
        private readonly UnconditionedZonesCalculator _calculator;
        private readonly ObjectDataSectionData? _objectData;
        private readonly HeatingSectionData? _heatingData;
        private readonly CoolingSectionData? _coolingData;

        [ObservableProperty]
        private ZtuZone? _selectedZone;

        [ObservableProperty]
        private ZtuMonthlyResults? _calculationResults;

        [ObservableProperty]
        private System.Collections.Generic.List<ZtuElementInfluence>? _elementInfluences;

        [ObservableProperty]
        private ZtuQtrResults? _qtrResults;

        [ObservableProperty]
        private bool _isDebugMonthlyEnabled;

        [ObservableProperty]
        private ObservableCollection<ZtuZoneOverviewRow> _zoneOverviewRows = new();

        [ObservableProperty]
        private ZtuZoneOverviewRow? _selectedOverviewRow;

        [ObservableProperty]
        private double _indoorTemperatureC = 20.0;

        public ObservableCollection<MaterialOption> MaterialOptions { get; } = new();

        public UnconditionedZonesSectionViewModel(
            UnconditionedZoneSectionData data,
            ObjectDataSectionData? objectData = null,
            HeatingSectionData? heatingData = null,
            CoolingSectionData? coolingData = null)
        {
            _data = data;
            _materialsService = new MaterialsService(new JsonMaterialsRepository());
            _calculator = new UnconditionedZonesCalculator();
            _objectData = objectData;
            _heatingData = heatingData;
            _coolingData = coolingData;

            LoadMaterialOptions();

            _data.Zones.CollectionChanged += Zones_CollectionChanged;
            foreach (var zone in _data.Zones)
            {
                AttachZone(zone);
            }

            RefreshZoneOverview();
            if (_data.Zones.Count > 0)
            {
                SelectedZone = _data.Zones[0];
            }
        }

        private void Zones_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ZtuZone zone in e.OldItems)
                {
                    DetachZone(zone);
                }
            }

            if (e.NewItems != null)
            {
                foreach (ZtuZone zone in e.NewItems)
                {
                    AttachZone(zone);
                }
            }

            RefreshZoneOverview();
            CalculateCommand.NotifyCanExecuteChanged();
        }

        private void AttachZone(ZtuZone zone)
        {
            zone.PropertyChanged += Zone_PropertyChanged;
            zone.ElementsToExternal.CollectionChanged += Elements_CollectionChanged;
            zone.ElementsToBoundary.CollectionChanged += Elements_CollectionChanged;

            InjectMaterialOptionsIntoElements(zone);
            foreach (var element in zone.ElementsToExternal)
            {
                AttachElement(element);
            }
            foreach (var element in zone.ElementsToBoundary)
            {
                AttachElement(element);
            }
        }

        private void DetachZone(ZtuZone zone)
        {
            zone.PropertyChanged -= Zone_PropertyChanged;
            zone.ElementsToExternal.CollectionChanged -= Elements_CollectionChanged;
            zone.ElementsToBoundary.CollectionChanged -= Elements_CollectionChanged;

            foreach (var element in zone.ElementsToExternal)
            {
                DetachElement(element);
            }
            foreach (var element in zone.ElementsToBoundary)
            {
                DetachElement(element);
            }
        }

        private void Zone_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ZtuZone.Name)
                || e.PropertyName == nameof(ZtuZone.Type)
                || e.PropertyName == nameof(ZtuZone.ManualUnconditionedTempWinterC)
                || e.PropertyName == nameof(ZtuZone.ManualUnconditionedTempSummerC))
            {
                RefreshZoneOverview();
                CalculateCommand.NotifyCanExecuteChanged();
                RecalculateDebugIfEnabled();
            }
        }

        private void Elements_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ZtuElement element in e.NewItems)
                {
                    InjectMaterialOptionsIntoElement(element);
                    AttachElement(element);
                }
            }

            if (e.OldItems != null)
            {
                foreach (ZtuElement element in e.OldItems)
                {
                    DetachElement(element);
                }
            }

            RefreshZoneOverview();
            CalculateCommand.NotifyCanExecuteChanged();
            RecalculateDebugIfEnabled();
        }

        private void AttachElement(ZtuElement element)
        {
            element.PropertyChanged += Element_PropertyChanged;
            element.Layers.CollectionChanged += Layers_CollectionChanged;
            foreach (var layer in element.Layers)
            {
                layer.PropertyChanged += Layer_PropertyChanged;
            }
        }

        private void DetachElement(ZtuElement element)
        {
            element.PropertyChanged -= Element_PropertyChanged;
            element.Layers.CollectionChanged -= Layers_CollectionChanged;
            foreach (var layer in element.Layers)
            {
                layer.PropertyChanged -= Layer_PropertyChanged;
            }
        }

        private void Element_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not ZtuElement element)
                return;

            if (e.PropertyName == nameof(ZtuElement.Kind))
            {
                RecalculateUValue(element);
            }

            if (e.PropertyName == nameof(ZtuElement.Area)
                || e.PropertyName == nameof(ZtuElement.UValue)
                || e.PropertyName == nameof(ZtuElement.Kind))
            {
                RefreshZoneOverview();
                RecalculateDebugIfEnabled();
            }
        }

        private void Layers_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ZtuLayer layer in e.NewItems)
                {
                    layer.PropertyChanged += Layer_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (ZtuLayer layer in e.OldItems)
                {
                    layer.PropertyChanged -= Layer_PropertyChanged;
                }
            }

            if (sender is ObservableCollection<ZtuLayer> layers)
            {
                var element = FindElementByLayers(layers);
                if (element != null)
                {
                    RecalculateUValue(element);
                }
            }

            RefreshZoneOverview();
            RecalculateDebugIfEnabled();
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
            if (sender is ZtuLayer layer &&
                (e.PropertyName == nameof(ZtuLayer.Thickness) || e.PropertyName == nameof(ZtuLayer.Lambda)))
            {
                var element = FindElementByLayer(layer);
                if (element != null)
                {
                    RecalculateUValue(element);
                    RefreshZoneOverview();
                    RecalculateDebugIfEnabled();
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

        private void RecalculateUValue(ZtuElement element)
        {
            if (element.Layers.Count == 0)
            {
                element.UValue = 0.0;
                return;
            }

            double rsi = element.Kind switch
            {
                ElementKind.Wall => 0.13,
                ElementKind.Roof => 0.10,
                ElementKind.Floor => 0.17,
                _ => 0.13
            };

            double rse = element.IsToExternalEnvironment ? 0.04 : rsi;
            double sumR = element.Layers.Sum(layer => layer.R);
            double rTotal = rsi + sumR + rse;
            element.UValue = rTotal > 0 ? 1.0 / rTotal : 0.0;
        }

        private void RefreshZoneOverview()
        {
            var oldSelection = SelectedZone;
            ZoneOverviewRows.Clear();

            var climateService = new ClimateService(new JsonClimateRepository());
            int climateZoneId = _objectData?.ClimateZone ?? 3;
            var climateData = climateService.GetZone(climateZoneId);

            var heatingBreakdown = climateData != null
                ? HeatingScheduleService.ComputeBreakdown(
                    _objectData?.CalculationMethod ?? HeatingCalculationMethod.Rd0220_3,
                    _objectData,
                    climateData)
                : Array.Empty<HeatingScheduleService.HeatingHoursBreakdown>();
            double[] thetaIntWinterCalc = climateData != null
                ? ScheduleHelper.ComputeThetaIntCalcH(_heatingData, heatingBreakdown)
                : Enumerable.Repeat(20.0, 12).ToArray();
            double[] thetaIntCoolingCalc = ScheduleHelper.ComputeThetaIntCalcC(_objectData, _coolingData);

            foreach (var zone in _data.Zones)
            {
                double areaBoundary = zone.ElementsToBoundary.Sum(x => x.Area);
                double areaExternal = zone.ElementsToExternal.Sum(x => x.Area);

                double hztuE = zone.Type == ZtuType.External
                    ? zone.ElementsToExternal.Sum(x => x.UValue * x.Area)
                    : 0.0;

                double hztc = zone.ElementsToBoundary.Sum(x => x.UValue * x.Area);
                double htot = hztuE + hztc;
                double bztu = htot > 1e-6 ? Math.Clamp(hztuE / htot, 0.0, 1.0) : 0.0;

                double uExtAvg = areaExternal > 1e-6
                    ? zone.ElementsToExternal.Sum(x => x.UValue * x.Area) / areaExternal
                    : 0.0;

                double uIntAvg = areaBoundary > 1e-6
                    ? zone.ElementsToBoundary.Sum(x => x.UValue * x.Area) / areaBoundary
                    : 0.0;

                var thetaZtuWinterMonths = new System.Collections.Generic.List<double>();
                var thetaZtuSummerMonths = new System.Collections.Generic.List<double>();

                for (int m = 0; m < 12; m++)
                {
                    int monthNumber = m + 1;
                    bool isSummer = monthNumber >= 5 && monthNumber <= 9;
                    double thetaZtuMonth;

                    if (zone.Type == ZtuType.Internal)
                    {
                        thetaZtuMonth = isSummer
                            ? zone.ManualUnconditionedTempSummerC
                            : zone.ManualUnconditionedTempWinterC;
                    }
                    else
                    {
                        double thetaExt = climateData?.Monthly.AvgMonthlyTempC[m] ?? 0.0;
                        double thetaInt;
                        if (isSummer)
                        {
                            thetaInt = thetaIntCoolingCalc != null && thetaIntCoolingCalc.Length == 12
                                ? thetaIntCoolingCalc[m]
                                : _data.ThetaIntSummer;
                        }
                        else
                        {
                            thetaInt = _data.IsWinterTempOverride && _data.ThetaIntWinterOverride.HasValue
                                ? _data.ThetaIntWinterOverride.Value
                                : thetaIntWinterCalc[m];
                        }

                        thetaZtuMonth = thetaExt + bztu * (thetaInt - thetaExt);
                    }

                    if (isSummer)
                        thetaZtuSummerMonths.Add(thetaZtuMonth);
                    else
                        thetaZtuWinterMonths.Add(thetaZtuMonth);
                }

                double thetaZtuWinterAvg = thetaZtuWinterMonths.Count > 0 ? thetaZtuWinterMonths.Average() : 0.0;
                double thetaZtuSummerAvg = thetaZtuSummerMonths.Count > 0 ? thetaZtuSummerMonths.Average() : 0.0;

                double helFactor = zone.Type == ZtuType.External ? bztu : (1.0 - bztu);
                double hel = helFactor * hztc;

                ZoneOverviewRows.Add(new ZtuZoneOverviewRow(zone)
                {
                    ZoneName = zone.Name,
                    ZoneType = zone.Type == ZtuType.External ? "External" : "Internal",
                    AreaBoundary_m2 = areaBoundary,
                    UextAvg_Wm2K = uExtAvg,
                    UintAvg_Wm2K = uIntAvg,
                    HztuE_WK = hztuE,
                    HztcZtu_WK = hztc,
                    Bztu = bztu,
                    ThetaZtuWinterAvg_C = thetaZtuWinterAvg,
                    ThetaZtuSummerAvg_C = thetaZtuSummerAvg,
                    Hel_WK = hel
                });
            }

            if (oldSelection != null)
            {
                SelectedOverviewRow = ZoneOverviewRows.FirstOrDefault(x => x.Zone == oldSelection);
            }
            if (SelectedOverviewRow == null && ZoneOverviewRows.Count > 0)
            {
                SelectedOverviewRow = ZoneOverviewRows[0];
            }
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
            RefreshZoneOverview();
        }

        [RelayCommand]
        private void DeleteZone()
        {
            if (SelectedZone == null) return;

            var index = _data.Zones.IndexOf(SelectedZone);
            _data.Zones.Remove(SelectedZone);

            if (_data.Zones.Count > 0)
            {
                SelectedZone = index < _data.Zones.Count ? _data.Zones[index] : _data.Zones[_data.Zones.Count - 1];
            }
            else
            {
                SelectedZone = null;
            }

            RefreshZoneOverview();
        }

        [RelayCommand]
        private void AddElementToExternal()
        {
            if (SelectedZone == null || SelectedZone.Type != ZtuType.External) return;

            var newElement = new ZtuElement
            {
                Name = $"Елемент {SelectedZone.ElementsToExternal.Count + 1}",
                Kind = ElementKind.Wall,
                Area = 0.0,
                IsToExternalEnvironment = true
            };

            SelectedZone.ElementsToExternal.Add(newElement);
            RefreshZoneOverview();
        }

        [RelayCommand]
        private void AddElementToBoundary()
        {
            if (SelectedZone == null) return;

            var newElement = new ZtuElement
            {
                Name = $"Разделящ елемент {SelectedZone.ElementsToBoundary.Count + 1}",
                Kind = ElementKind.Wall,
                Area = 0.0,
                IsToExternalEnvironment = false
            };

            SelectedZone.ElementsToBoundary.Add(newElement);
            RefreshZoneOverview();
        }

        [RelayCommand]
        private void DeleteElementToExternal(ZtuElement element)
        {
            if (SelectedZone == null || element == null) return;
            SelectedZone.ElementsToExternal.Remove(element);
            RefreshZoneOverview();
        }

        [RelayCommand]
        private void DeleteElementToBoundary(ZtuElement element)
        {
            if (SelectedZone == null || element == null) return;
            SelectedZone.ElementsToBoundary.Remove(element);
            RefreshZoneOverview();
        }

        [RelayCommand]
        private void AddLayer(ZtuElement element)
        {
            if (element == null) return;

            var newLayer = new ZtuLayer
            {
                MaterialName = "Избери материал",
                Thickness = 100.0,
                Lambda = 1.0,
                MaterialOptions = MaterialOptions.ToList()
            };

            newLayer.PropertyChanged += Layer_PropertyChanged;
            element.Layers.Add(newLayer);
            RecalculateUValue(element);
            RefreshZoneOverview();
        }

        [RelayCommand]
        private void DeleteLayer((ZtuElement element, ZtuLayer layer) parameters)
        {
            if (parameters.element == null || parameters.layer == null) return;

            parameters.element.Layers.Remove(parameters.layer);
            RecalculateUValue(parameters.element);
            RefreshZoneOverview();
        }

        [RelayCommand(CanExecute = nameof(CanCalculate))]
        private void Calculate()
        {
            if (SelectedZone == null) return;

            var climateService = new ClimateService(new JsonClimateRepository());
            int climateZoneId = _objectData?.ClimateZone ?? 3;
            var climateData = climateService.GetZone(climateZoneId);
            if (climateData == null)
            {
                System.Diagnostics.Debug.WriteLine($"Cannot load climate data for zone {climateZoneId}");
                return;
            }

            var heatingBreakdown = HeatingScheduleService.ComputeBreakdown(
                _objectData?.CalculationMethod ?? HeatingCalculationMethod.Rd0220_3,
                _objectData,
                climateData);
            double[] thetaIntWinterCalc = ScheduleHelper.ComputeThetaIntCalcH(_heatingData, heatingBreakdown);
            double[] thetaIntCoolingCalc = ScheduleHelper.ComputeThetaIntCalcC(_objectData, _coolingData);

            CalculationResults = _calculator.CalculateWithSeasonalTemps(
                SelectedZone,
                climateData,
                thetaIntSummer: _data.ThetaIntSummer,
                thetaIntCoolingCalc: thetaIntCoolingCalc,
                thetaIntWinterCalc: thetaIntWinterCalc,
                isWinterOverride: _data.IsWinterTempOverride,
                winterOverrideValue: _data.ThetaIntWinterOverride);

            if (CalculationResults != null)
            {
                ElementInfluences = _calculator.CalculateInfluenceOnHtr(SelectedZone, CalculationResults);
                QtrResults = _calculator.CalculateQtrResults(
                    SelectedZone,
                    CalculationResults,
                    _objectData,
                    _heatingData,
                    _coolingData,
                    _data,
                    climateData);
            }

            RefreshZoneOverview();
        }

        private void ClearDebugResults()
        {
            CalculationResults = null;
            QtrResults = null;
            ElementInfluences = null;
        }

        private void RecalculateDebugIfEnabled()
        {
            if (!IsDebugMonthlyEnabled)
            {
                ClearDebugResults();
                return;
            }

            if (CalculateCommand.CanExecute(null))
            {
                Calculate();
            }
            else
            {
                ClearDebugResults();
            }
        }

        private bool CanCalculate()
        {
            return SelectedZone != null
                && (SelectedZone.ElementsToExternal.Any() || SelectedZone.ElementsToBoundary.Any());
        }

        partial void OnSelectedZoneChanged(ZtuZone? value)
        {
            if (value != null)
            {
                SelectedOverviewRow = ZoneOverviewRows.FirstOrDefault(x => x.Zone == value);
            }

            CalculateCommand.NotifyCanExecuteChanged();
            RecalculateDebugIfEnabled();
        }

        partial void OnSelectedOverviewRowChanged(ZtuZoneOverviewRow? value)
        {
            if (value != null && SelectedZone != value.Zone)
            {
                SelectedZone = value.Zone;
            }
        }

        partial void OnIndoorTemperatureCChanged(double value)
        {
            RecalculateDebugIfEnabled();
        }

        partial void OnIsDebugMonthlyEnabledChanged(bool value)
        {
            RecalculateDebugIfEnabled();
        }

        public UnconditionedZoneSectionData Data => _data;
    }
}
