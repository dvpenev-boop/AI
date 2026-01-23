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
                if (value != null)
                {
                    // If the selected row is a composite child (e.g. the "Wall" part of a HeatedBasement),
                    // redirect selection to the composite owner (the "Floor" part) so the UI shows
                    // exactly one detail container for the composite element.
                    if (value.IsComposite && value.CompositeType == "Wall" && !string.IsNullOrEmpty(value.GroupId))
                    {
                        var owner = FloorItems.FirstOrDefault(fi => fi.GroupId == value.GroupId && fi.CompositeType == "Floor");
                        if (owner != null)
                        {
                            Debug.WriteLine($"[FloorSectionViewModel] Selection redirected from child '{value.Name}' to owner '{owner.Name}'");
                            value = owner;
                        }
                    }
                }

                if (_selectedFloorItem != value)
                {
                    _selectedFloorItem = value;
                    OnPropertyChanged(nameof(SelectedFloorItem));
                    Debug.WriteLine($"[FloorSectionViewModel] SelectedFloorItem changed successfully to {_selectedFloorItem?.Name}");
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
                System.Diagnostics.Debug.WriteLine($"[RecalculateExternalAir] Area={detail.Area}, Layers={detail.Layers.Count}");
                var input = new Models.FloorExternalAirInput
                {
                    Area = detail.Area,
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
                    item.Area = detail.Area;
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

                    case FloorType.UnheatedBasement:
                        Debug.WriteLine("[FloorSectionViewModel] Creating UnheatedBasement detail");
                        floorItem.UnheatedBasementDetail = new FloorUnheatedBasementDetail();
                        // Инициализация на слоевете - добавяме по 1 слой във всяка колекция
                        if (!floorItem.UnheatedBasementDetail.FloorToBasementLayers.Any())
                        {
                            Debug.WriteLine("[FloorSectionViewModel] Adding default layer to FloorToBasementLayers");
                            floorItem.UnheatedBasementDetail.FloorToBasementLayers.Add(new FloorLayer { Material = "Бетон", Thickness = 0.2, Lambda = 1.7 });
                        }
                        if (!floorItem.UnheatedBasementDetail.BasementFloorLayers.Any())
                        {
                            Debug.WriteLine("[FloorSectionViewModel] Adding default layer to BasementFloorLayers");
                            floorItem.UnheatedBasementDetail.BasementFloorLayers.Add(new FloorLayer { Material = "Бетон", Thickness = 0.2, Lambda = 1.7 });
                        }
                        if (!floorItem.UnheatedBasementDetail.BasementWallLayers.Any())
                        {
                            Debug.WriteLine("[FloorSectionViewModel] Adding default layer to BasementWallLayers");
                            floorItem.UnheatedBasementDetail.BasementWallLayers.Add(new FloorLayer { Material = "Бетон", Thickness = 0.25, Lambda = 1.7 });
                        }
                        if (!floorItem.UnheatedBasementDetail.WallAboveGradeLayers.Any())
                        {
                            Debug.WriteLine("[FloorSectionViewModel] Adding default layer to WallAboveGradeLayers");
                            floorItem.UnheatedBasementDetail.WallAboveGradeLayers.Add(new FloorLayer { Material = "Бетон", Thickness = 0.25, Lambda = 1.7 });
                        }
                        SubscribeUnheatedBasementDetail(floorItem);
                        RecalculateUnheatedBasement(floorItem);
                        break;

                    case FloorType.HeatedBasement:
                        Debug.WriteLine("[FloorSectionViewModel] Creating HeatedBasement detail");
                        floorItem.HeatedBasementDetail = new FloorHeatedBasementDetail();
                        if (!floorItem.HeatedBasementDetail.FloorLayers.Any())
                        {
                            Debug.WriteLine("[FloorSectionViewModel] Adding default floor layer to HeatedBasement");
                            floorItem.HeatedBasementDetail.FloorLayers.Add(new FloorLayer { Material = "Бетон", Thickness = 0.2, Lambda = 1.7 });
                        }
                        if (!floorItem.HeatedBasementDetail.WallLayers.Any())
                        {
                            Debug.WriteLine("[FloorSectionViewModel] Adding default wall layer to HeatedBasement");
                            floorItem.HeatedBasementDetail.WallLayers.Add(new FloorLayer { Material = "Бетон", Thickness = 0.25, Lambda = 1.7 });
                        }

                        // КРИТИЧНО: Създаваме композитна група (2 реда)
                        var groupId = Guid.NewGuid().ToString();
                        
                        // РЕД 1: Под към земя
                        floorItem.GroupId = groupId;
                        floorItem.IsComposite = true;
                        floorItem.CompositeType = "Floor";
                        floorItem.Name = "Под над отопляем сутерен";
                        
                        // Subscribe за промени и преизчисляване
                        SubscribeHeatedBasementDetail(floorItem);
                        RecalculateHeatedBasement(floorItem);

                        Debug.WriteLine($"[FloorSectionViewModel] Adding Floor component to collection");
                        SelectedFloorItem = floorItem;
                        FloorItems.Insert(0, floorItem);

                        // РЕД 2: Стена към земя (създаваме нов FloorItem с референция към същия detail)
                        var wallItem = new FloorItem
                        {
                            Number = FloorItems.Count + 1,
                            Name = "Под над отопляем сутерен",
                            FloorType = FloorType.HeatedBasement,
                            GroupId = groupId,
                            IsComposite = true,
                            CompositeType = "Wall",
                            HeatedBasementDetail = floorItem.HeatedBasementDetail,
                            Area = 0, // Ще се изчисли от z * P
                            UValue = 0
                        };

                        Debug.WriteLine($"[FloorSectionViewModel] Adding Wall component to collection");
                        // Insert wallItem just below the main floorItem for composite
                        FloorItems.Insert(1, wallItem);

                        Debug.WriteLine($"[FloorSectionViewModel] Raising PropertyChanged for FloorItems");
                        OnPropertyChanged(nameof(FloorItems));
                        
                        Debug.WriteLine("[FloorSectionViewModel] TryAddFloor completed successfully (HeatedBasement with 2 rows)");
                        return true;

                    default:
                        Debug.WriteLine($"[FloorSectionViewModel] ERROR: Unknown floor type: {floorType}");
                        error = $"Непознат тип под: {floorType}";
                        return false;
                }

                Debug.WriteLine($"[FloorSectionViewModel] Detail initialized successfully");
                
                Debug.WriteLine($"[FloorSectionViewModel] Setting SelectedFloorItem BEFORE adding to collection");
                SelectedFloorItem = floorItem;
                
                Debug.WriteLine($"[FloorSectionViewModel] Adding FloorItem to collection... Current count: {FloorItems.Count}");
                FloorItems.Insert(0, floorItem);
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

            // КРИТИЧНО: Ако това е композитен под (Heated Basement), трием и двата реда
            if (floorItem.IsComposite && !string.IsNullOrEmpty(floorItem.GroupId))
            {
                Debug.WriteLine($"[FloorSectionViewModel] Removing composite floor group: {floorItem.GroupId}");
                
                // Намираме всички редове от същата група
                var groupItems = FloorItems.Where(item => item.GroupId == floorItem.GroupId).ToList();
                
                Debug.WriteLine($"[FloorSectionViewModel] Found {groupItems.Count} items in composite group");
                
                // Премахваме всички от групата
                foreach (var item in groupItems)
                {
                    FloorItems.Remove(item);
                }
                
                var removedIndex = 0; // За простота, ще вземем първия
                UpdateIndexes();
                SelectedFloorItem = GetNextSelection(removedIndex);
                OnPropertyChanged(nameof(FloorItems));
                Debug.WriteLine($"[FloorSectionViewModel] Composite group removed. Total items: {FloorItems.Count}");
                return;
            }

            // За обикновени (некомпозитни) подове
            var removedIndex2 = FloorItems.IndexOf(floorItem);
            FloorItems.Remove(floorItem);
            UpdateIndexes();
            SelectedFloorItem = GetNextSelection(removedIndex2);
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

        private void SubscribeUnheatedBasementDetail(FloorItem item)
        {
            var detail = item.UnheatedBasementDetail;
            if (detail == null) return;

            // Subscribe към промени на основните свойства
            detail.PropertyChanged += (s, e) => RecalculateUnheatedBasement(item);

            // Subscribe към промени във всяка колекция от слоеве
            var collections = new[]
            {
                detail.FloorToBasementLayers,
                detail.BasementFloorLayers,
                detail.BasementWallLayers,
                detail.WallAboveGradeLayers
            };

            foreach (var collection in collections)
            {
                collection.CollectionChanged += (s, e) =>
                {
                    if (e.NewItems != null)
                    {
                        foreach (FloorLayer layer in e.NewItems)
                            layer.PropertyChanged += (s2, e2) => RecalculateUnheatedBasement(item);
                    }
                    RecalculateUnheatedBasement(item);
                };

                foreach (var layer in collection)
                    layer.PropertyChanged += (s, e) => RecalculateUnheatedBasement(item);
            }
        }

        private void RecalculateUnheatedBasement(FloorItem item)
        {
            try
            {
                if (item.UnheatedBasementDetail == null) return;
                var detail = item.UnheatedBasementDetail;

                System.Diagnostics.Debug.WriteLine($"[RecalculateUnheatedBasement] Area={detail.Area}, Perimeter={detail.Perimeter}");

                var input = new Models.FloorUnheatedBasementInput
                {
                    Area = detail.Area,
                    Perimeter = detail.Perimeter,
                    DepthBelowGround = detail.DepthBelowGround,
                    HeightAboveGround = detail.HeightAboveGround,
                    Volume = detail.Volume,
                    LambdaGround = detail.LambdaGround,
                    WallThicknessAtGrade = detail.WallThicknessAtGrade,
                    AirChangeRate = detail.AirChangeRate,
                    AirDensity = detail.AirDensity,
                    AirSpecificHeat = detail.AirSpecificHeat,
                    RsiFloorToBasement = detail.RsiFloorToBasement,
                    RseFloorToBasement = detail.RseFloorToBasement,
                    RsiBasementFloor = detail.RsiBasementFloor,
                    RseBasementFloor = detail.RseBasementFloor,
                    RsiBasementWall = detail.RsiBasementWall,
                    RseBasementWall = detail.RseBasementWall,
                    RsiWallAboveGrade = detail.RsiWallAboveGrade,
                    RseWallAboveGrade = detail.RseWallAboveGrade,
                    // Нови полета за прозорци и врати
                    WindowArea = detail.WindowArea,
                    DoorArea = detail.DoorArea,
                    WindowUValue = detail.WindowUValue,
                    DoorUValue = detail.DoorUValue
                };

                // Копирай слоевете от всички 4 конструкции
                CopyLayers(detail.FloorToBasementLayers, input.FloorToBasementLayers);
                CopyLayers(detail.BasementFloorLayers, input.BasementFloorLayers);
                CopyLayers(detail.BasementWallLayers, input.BasementWallLayers);
                CopyLayers(detail.WallAboveGradeLayers, input.WallAboveGradeLayers);

                var calc = new Services.FloorCalculator();
                var result = calc.Calculate(Models.FloorType.UnheatedBasement, input);

                if (result.IsValid)
                {
                    item.UValue = result.U;
                    item.Area = detail.Area;

                    // Попълни диагностичните резултати
                    detail.ResultUub = result.U;
                    foreach (var component in result.Components)
                    {
                        switch (component.Name)
                        {
                            case "Uf_sus": detail.ResultUfSus = component.Value; break;
                            case "Uf_gb": detail.ResultUfGb = component.Value; break;
                            case "Uw_gb": detail.ResultUwGb = component.Value; break;
                            case "Uw": detail.ResultUw = component.Value; break;
                            case "Hve_b": detail.ResultHveB = component.Value; break;
                            case "B": detail.ResultB = component.Value; break;
                            case "df": detail.ResultDf = component.Value; break;
                            case "dwb": detail.ResultDwb = component.Value; break;
                            case "S": detail.ResultS = component.Value; break;
                        }
                    }

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
                System.Diagnostics.Debug.WriteLine($"[RecalculateUnheatedBasement] Exception: {ex}");
            }
        }

        private void CopyLayers(System.Collections.ObjectModel.ObservableCollection<FloorLayer> source, System.Collections.ObjectModel.ObservableCollection<FloorLayer> target)
        {
            target.Clear();
            foreach (var l in source)
            {
                target.Add(new FloorLayer { Material = l.Material, Thickness = l.Thickness, Lambda = l.Lambda });
            }
        }

        private void SubscribeHeatedBasementDetail(FloorItem item)
        {
            var detail = item.HeatedBasementDetail;
            if (detail == null) return;

            // Subscribe към промени на основните свойства
            detail.PropertyChanged += (s, e) => RecalculateHeatedBasement(item);

            // Subscribe към промени в колекциите от слоеве
            var collections = new[]
            {
                detail.FloorLayers,
                detail.WallLayers
            };

            foreach (var collection in collections)
            {
                collection.CollectionChanged += (s, e) =>
                {
                    if (e.NewItems != null)
                    {
                        foreach (FloorLayer layer in e.NewItems)
                            layer.PropertyChanged += (s2, e2) => RecalculateHeatedBasement(item);
                    }
                    RecalculateHeatedBasement(item);
                };

                foreach (var layer in collection)
                    layer.PropertyChanged += (s, e) => RecalculateHeatedBasement(item);
            }
        }

        private void RecalculateHeatedBasement(FloorItem item)
        {
            try
            {
                if (item.HeatedBasementDetail == null) return;
                var detail = item.HeatedBasementDetail;

                System.Diagnostics.Debug.WriteLine($"[RecalculateHeatedBasement] Area={detail.Area}, Perimeter={detail.Perimeter}, Depth={detail.Depth}");

                var input = new Models.FloorHeatedBasementInput
                {
                    Area = detail.Area,
                    Perimeter = detail.Perimeter,
                    Depth = detail.Depth,
                    WallThicknessAtGrade = detail.WallThicknessAtGrade,
                    LambdaGround = detail.LambdaGround,
                    PsiWallFloor = detail.PsiWallFloor,
                    RsiFloor = detail.RsiFloor,
                    RseFloor = detail.RseFloor,
                    RsiWall = detail.RsiWall,
                    RseWall = detail.RseWall
                };

                // Копирай слоевете
                CopyLayers(detail.FloorLayers, input.FloorLayers);
                CopyLayers(detail.WallLayers, input.WallLayers);

                var calc = new Services.FloorCalculator();
                var result = calc.Calculate(Models.FloorType.HeatedBasement, input);

                if (result.IsValid)
                {
                    // Извличаме резултатите от components
                    double U_f_g_b = GetComponentValue(result.Components, "Uf;g;b");
                    double U_w_g_b = GetComponentValue(result.Components, "Uw;g;b");
                    double H_g = GetComponentValue(result.Components, "Hg");
                    double B = GetComponentValue(result.Components, "B");
                    double d_f = GetComponentValue(result.Components, "df");
                    double d_w_b = GetComponentValue(result.Components, "dwb");
                    double A_walls = GetComponentValue(result.Components, "Awalls");

                    // Съхраняваме диагностичните резултати
                    detail.ResultUfgb = U_f_g_b;
                    detail.ResultUwgb = U_w_g_b;
                    detail.ResultHg = H_g;
                    detail.ResultB = B;
                    detail.ResultDf = d_f;
                    detail.ResultDwb = d_w_b;
                    detail.ResultAwalls = A_walls;

                    // КРИТИЧНО: Обновяваме ДВАТА реда в таблицата
                    if (!string.IsNullOrEmpty(item.GroupId))
                    {
                        var groupItems = FloorItems.Where(fi => fi.GroupId == item.GroupId).ToList();
                        
                        foreach (var groupItem in groupItems)
                        {
                            if (groupItem.CompositeType == "Floor")
                            {
                                // Ред 1: Под към земя
                                groupItem.UValue = U_f_g_b;
                                groupItem.Area = detail.Area;
                                groupItem.NotifyDisplayPropertiesChanged();
                            }
                            else if (groupItem.CompositeType == "Wall")
                            {
                                // Ред 2: Стена към земя
                                groupItem.UValue = U_w_g_b;
                                groupItem.Area = A_walls; // z * P
                                groupItem.NotifyDisplayPropertiesChanged();
                            }
                        }
                    }
                }
                else
                {
                    // При грешка, нулираме U на всички редове от групата
                    if (!string.IsNullOrEmpty(item.GroupId))
                    {
                        var groupItems = FloorItems.Where(fi => fi.GroupId == item.GroupId).ToList();
                        foreach (var groupItem in groupItems)
                        {
                            groupItem.UValue = 0;
                            groupItem.NotifyDisplayPropertiesChanged();
                        }
                    }
                }

                OnPropertyChanged(nameof(FloorItems));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RecalculateHeatedBasement] Exception: {ex}");
            }
        }

        private double GetComponentValue(System.Collections.Generic.List<FloorCalcComponent> components, string name)
        {
            var component = components.FirstOrDefault(c => c.Name == name);
            return component?.Value ?? 0.0;
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
