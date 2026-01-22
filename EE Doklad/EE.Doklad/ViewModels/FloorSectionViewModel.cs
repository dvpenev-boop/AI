using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using EE.Doklad.Models;
using System.Diagnostics;

namespace EE.Doklad.ViewModels
{
    public class FloorSectionViewModel : INotifyPropertyChanged
    {
        private readonly FloorSectionData _data;

        public event PropertyChangedEventHandler? PropertyChanged;

        public System.Collections.ObjectModel.ObservableCollection<FloorItem> FloorItems => _data.FloorItems;

        public string Description
        {
            get => _data.Description;
            set
            {
                if (_data.Description != value)
                {
                    _data.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public ICommand RemoveFloorItemCommand { get; }

        private FloorItem? _selectedFloorItem;
        public FloorItem? SelectedFloorItem
        {
            get => _selectedFloorItem;
            set
            {
                Debug.WriteLine($"[FloorSectionViewModel] SelectedFloorItem changing from {_selectedFloorItem?.Name} to {value?.Name}");
                if (_selectedFloorItem != value)
                {
                    _selectedFloorItem = value;
                    OnPropertyChanged(nameof(SelectedFloorItem));
                    Debug.WriteLine($"[FloorSectionViewModel] SelectedFloorItem changed successfully");
                }
            }
        }

        public FloorSectionViewModel()
            : this(new FloorSectionData())
        {
            Debug.WriteLine("[FloorSectionViewModel] Constructor called (default)");
        }

        public FloorSectionViewModel(FloorSectionData data)
        {
            Debug.WriteLine("[FloorSectionViewModel] Constructor called with data");
            _data = data;
            RemoveFloorItemCommand = new RelayCommand(param => RemoveFloorItem(param as FloorItem));
            
            // Subscribe to collection changes
            _data.FloorItems.CollectionChanged += (s, e) =>
            {
                Debug.WriteLine($"[FloorSectionViewModel] FloorItems.CollectionChanged: Action={e.Action}, NewItemsCount={e.NewItems?.Count ?? 0}");
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (item is FloorItem floorItem)
                        {
                            if (floorItem.FloorType == Models.FloorType.Ground && floorItem.GroundDetail != null)
                                SubscribeGroundDetail(floorItem);
                            if (floorItem.FloorType == Models.FloorType.ExternalAir && floorItem.ExternalAirDetail != null)
                                SubscribeExternalAirDetail(floorItem);
                        }
                    }
                }
            };

            // Subscribe for already existing items
            foreach (var item in _data.FloorItems)
            {
                if (item is FloorItem floorItem)
                {
                    if (floorItem.FloorType == Models.FloorType.Ground && floorItem.GroundDetail != null)
                        SubscribeGroundDetail(floorItem);
                    if (floorItem.FloorType == Models.FloorType.ExternalAir && floorItem.ExternalAirDetail != null)
                        SubscribeExternalAirDetail(floorItem);
                }
            }
            Debug.WriteLine("[FloorSectionViewModel] Constructor completed");
        }

        // ---
        // Добавено: ExternalAir subscribe и преизчисление
        private void SubscribeExternalAirDetail(FloorItem item)
        {
            var detail = item.ExternalAirDetail;
            if (detail == null) return;
            detail.Layers.CollectionChanged += (s, e) => {
                // Subscribe към PropertyChanged за нови слоеве
                if (e.NewItems != null)
                {
                    foreach (FloorLayer layer in e.NewItems)
                        layer.PropertyChanged += (s2, e2) => RecalculateExternalAir(item);
                }
                RecalculateExternalAir(item);
            };
            foreach (var layer in detail.Layers)
                layer.PropertyChanged += (s, e) => RecalculateExternalAir(item);
            // Subscribe към промяна на Area (ако има такова свойство)
            detail.PropertyChanged += (s, e) => {
                if (e.PropertyName == "Area")
                    RecalculateExternalAir(item);
            };
        }

        private void RecalculateExternalAir(FloorItem item)
        {
            try
            {
                if (item.ExternalAirDetail == null) return;
                var detail = item.ExternalAirDetail;
                System.Diagnostics.Debug.WriteLine($"[RecalculateExternalAir] Area={item.Area}, Layers={detail.Layers.Count}");
                var input = new Models.FloorExternalAirInput
                {
                    Area = item.Area,
                    Rsi = detail.Rsi,
                    Rse = detail.Rse
                };
                // Копирай слоевете
                if (detail.Layers != null)
                {
                    foreach (var l in detail.Layers)
                    {
                        var layer = new Models.FloorLayer { Material = l.Material, Thickness = l.Thickness, Lambda = l.Lambda };
                        input.Layers.Add(layer);
                        layer.PropertyChanged += (s, e) => RecalculateExternalAir(item);
                    }
                }

                var calc = new Services.FloorCalculator();
                var result = calc.Calculate(Models.FloorType.ExternalAir, input);
                if (result.IsValid)
                {
                    item.UValue = result.U;
                    item.Area = input.Area;
                    item.NotifyDisplayPropertiesChanged();
                }
                else
                {
                    item.UValue = 0;
                    item.NotifyDisplayPropertiesChanged();
                }
                OnPropertyChanged(nameof(FloorItems));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RecalculateExternalAir] Exception: {ex}");
            }
        }
            private void SubscribeGroundDetail(FloorItem item)
            {
                var detail = item.GroundDetail;
                if (detail == null) return;
                detail.PropertyChanged += (s, e) => RecalculateGround(item);
                detail.Layers.CollectionChanged += (s, e) => {
                    // При добавяне/премахване на слой – subscribe към PropertyChanged и преизчисли
                    if (e.NewItems != null)
                    {
                        foreach (FloorLayer layer in e.NewItems)
                            layer.PropertyChanged += (s2, e2) => RecalculateGround(item);
                    }
                    RecalculateGround(item);
                };
                foreach (var layer in detail.Layers)
                    layer.PropertyChanged += (s, e) => RecalculateGround(item);
            }

            private void RecalculateGround(FloorItem item)
            {
                if (item.GroundDetail == null) return;
                var detail = item.GroundDetail;
                var input = new Models.FloorGroundInput
                {
                    Area = detail.Area,
                    Perimeter = detail.Perimeter,
                    LambdaGround = detail.LambdaGround,
                    InsulationType = detail.InsulationType,
                    InsulationWidth = detail.InsulationWidth,
                    InsulationDepth = detail.InsulationDepth,
                    WallThickness = detail.WallThickness,
                    InsulationThickness = detail.InsulationThickness,
                    InsulationLambda = detail.InsulationLambda
                };
                // Копирай слоевете
                if (detail.Layers != null)
                {
                    foreach (var l in detail.Layers)
                    {
                        var layer = new FloorLayer { Material = l.Material, Thickness = l.Thickness, Lambda = l.Lambda };
                        input.Layers.Add(layer);
                        layer.PropertyChanged += (s, e) => RecalculateGround(item);
                    }
                }

                var calc = new Services.FloorCalculator();
                var result = calc.Calculate(Models.FloorType.Ground, input);
                if (result.IsValid)
                {
                    item.UValue = result.U;
                    item.Area = input.Area;
                    item.NotifyDisplayPropertiesChanged();
                    detail.DebugInfo = string.Join("\n", result.Assumptions);
                    detail.Error = string.Empty;

                    // Set result properties for UI
                    detail.ResultRn = GetAssumptionValue(result.Assumptions, "Rn = ");
                    detail.ResultRPrime = GetAssumptionValue(result.Assumptions, "R' = ");
                    detail.ResultDPrime = GetAssumptionValue(result.Assumptions, "d' = ");
                    detail.ResultArg1 = GetAssumptionValue(result.Assumptions, "arg1 = ");
                    detail.ResultArg2 = GetAssumptionValue(result.Assumptions, "arg2 = ");
                    detail.ResultPsiGed = GetAssumptionValue(result.Assumptions, "Psi_g_ed = ");
                    detail.ResultU0 = GetAssumptionValue(result.Assumptions, "U0 = ");
                    detail.ResultU = GetAssumptionValue(result.Assumptions, "U = ");
                    detail.ResultB = GetAssumptionValue(result.Assumptions, "B = ");
                    detail.ResultRf = GetAssumptionValue(result.Assumptions, "Rf = ");
                    detail.ResultDf = GetAssumptionValue(result.Assumptions, "df = ");
                }
                else
                {
                    item.UValue = 0;
                    detail.DebugInfo = string.Empty;
                    detail.Error = result.ErrorMessage;
                }
                OnPropertyChanged(nameof(FloorItems));
            }

        private double GetAssumptionValue(System.Collections.Generic.List<string> assumptions, string prefix)
        {
            var line = assumptions.FirstOrDefault(a => a.StartsWith(prefix));
            if (line != null)
            {
                var valueStr = line.Substring(prefix.Length).Trim();
                if (double.TryParse(valueStr.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var val))
                    return val;
            }
            return double.NaN;
        }
        // (Премахнато дублиране на RecalculateGround)

        public bool TryAddFloor(FloorType floorType, out string? error)
        {
            Debug.WriteLine($"[FloorSectionViewModel] TryAddFloor START with type: {floorType}");
            error = null;

            try
            {
                Debug.WriteLine($"[FloorSectionViewModel] Creating new FloorItem...");
                var floorItem = new FloorItem
                {
                    Number = FloorItems.Count + 1,
                    Name = $"Под тип {FloorItems.Count + 1}",
                    FloorType = floorType,
                    Area = 0,
                    UValue = 0
                };

                Debug.WriteLine($"[FloorSectionViewModel] FloorItem created: Number={floorItem.Number}, Name={floorItem.Name}, Type={floorItem.FloorType}");

                // Initialize specific details based on floor type
                Debug.WriteLine($"[FloorSectionViewModel] Initializing detail for type: {floorType}");
                switch (floorType)
                {
                    case FloorType.ExternalAir:
                        Debug.WriteLine("[FloorSectionViewModel] Creating ExternalAir detail");
                        floorItem.ExternalAirDetail = new FloorExternalAirDetail();
                        if (!floorItem.ExternalAirDetail.Layers.Any())
                        {
                            Debug.WriteLine("[FloorSectionViewModel] Adding default layer to ExternalAir");
                            floorItem.ExternalAirDetail.Layers.Add(new FloorLayer());
                        }
                        SubscribeExternalAirDetail(floorItem);
                        RecalculateExternalAir(floorItem);
                        break;

                    case FloorType.Ground:
                        Debug.WriteLine("[FloorSectionViewModel] Creating Ground detail");
                        floorItem.GroundDetail = new FloorGroundDetail();
                        if (!floorItem.GroundDetail.Layers.Any())
                        {
                            Debug.WriteLine("[FloorSectionViewModel] Adding default layer to Ground");
                            floorItem.GroundDetail.Layers.Add(new FloorLayer());
                        }
                        break;

                    case FloorType.UnheatedSpace:
                        Debug.WriteLine("[FloorSectionViewModel] Creating UnheatedSpace detail");
                        floorItem.UnheatedSpaceDetail = new FloorUnheatedSpaceDetail();
                        if (!floorItem.UnheatedSpaceDetail.Layers.Any())
                        {
                            Debug.WriteLine("[FloorSectionViewModel] Adding default layer to UnheatedSpace");
                            floorItem.UnheatedSpaceDetail.Layers.Add(new FloorLayer());
                        }
                        break;

                    case FloorType.HeatedBasement:
                        Debug.WriteLine("[FloorSectionViewModel] Creating HeatedBasement detail");
                        floorItem.HeatedBasementDetail = new FloorHeatedBasementDetail();
                        if (!floorItem.HeatedBasementDetail.FloorLayers.Any())
                        {
                            Debug.WriteLine("[FloorSectionViewModel] Adding default floor layer to HeatedBasement");
                            floorItem.HeatedBasementDetail.FloorLayers.Add(new FloorLayer());
                        }
                        if (!floorItem.HeatedBasementDetail.WallLayers.Any())
                        {
                            Debug.WriteLine("[FloorSectionViewModel] Adding default wall layer to HeatedBasement");
                            floorItem.HeatedBasementDetail.WallLayers.Add(new FloorLayer());
                        }
                        break;

                    default:
                        Debug.WriteLine($"[FloorSectionViewModel] ERROR: Unknown floor type: {floorType}");
                        error = $"Непознат тип под: {floorType}";
                        return false;
                }

                Debug.WriteLine($"[FloorSectionViewModel] Detail initialized successfully");
                
                Debug.WriteLine($"[FloorSectionViewModel] Setting SelectedFloorItem BEFORE adding to collection");
                SelectedFloorItem = floorItem;
                
                Debug.WriteLine($"[FloorSectionViewModel] Adding FloorItem to collection... Current count: {FloorItems.Count}");
                FloorItems.Add(floorItem);
                Debug.WriteLine($"[FloorSectionViewModel] FloorItem added successfully. New count: {FloorItems.Count}");
                
                Debug.WriteLine($"[FloorSectionViewModel] Raising PropertyChanged for FloorItems");
                OnPropertyChanged(nameof(FloorItems));
                
                Debug.WriteLine("[FloorSectionViewModel] TryAddFloor completed successfully");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FloorSectionViewModel] EXCEPTION in TryAddFloor: {ex.GetType().Name}: {ex.Message}");
                Debug.WriteLine($"[FloorSectionViewModel] Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"[FloorSectionViewModel] Inner exception: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                    Debug.WriteLine($"[FloorSectionViewModel] Inner stack trace: {ex.InnerException.StackTrace}");
                }
                error = $"Грешка при добавяне на под: {ex.Message}";
                return false;
            }
        }

        public void RemoveFloorItem(FloorItem? floorItem)
        {
            Debug.WriteLine($"[FloorSectionViewModel] RemoveFloorItem called for: {floorItem?.Name}");
            if (floorItem == null)
            {
                Debug.WriteLine("[FloorSectionViewModel] RemoveFloorItem: floorItem is null, returning");
                return;
            }

            var removedIndex = FloorItems.IndexOf(floorItem);
            FloorItems.Remove(floorItem);
            UpdateIndexes();
            SelectedFloorItem = GetNextSelection(removedIndex);
            OnPropertyChanged(nameof(FloorItems));
            Debug.WriteLine($"[FloorSectionViewModel] FloorItem removed. Total items: {FloorItems.Count}");
        }

        private FloorItem? GetNextSelection(int removedIndex)
        {
            if (!FloorItems.Any())
            {
                return null;
            }

            if (removedIndex < FloorItems.Count && removedIndex >= 0)
            {
                return FloorItems[removedIndex];
            }

            return FloorItems.LastOrDefault();
        }

        private void UpdateIndexes()
        {
            for (int i = 0; i < FloorItems.Count; i++)
            {
                FloorItems[i].Number = i + 1;
                if (string.IsNullOrWhiteSpace(FloorItems[i].Name))
                {
                    FloorItems[i].Name = $"Под тип {i + 1}";
                }
            }
        }

        protected void OnPropertyChanged(string name)
        {
            Debug.WriteLine($"[FloorSectionViewModel] OnPropertyChanged: {name}");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
