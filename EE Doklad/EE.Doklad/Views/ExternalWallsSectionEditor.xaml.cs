using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using EE.Doklad.Models;
using EE.Doklad.Sections.ThermalBridges;
using EE.Doklad.Services;
using Microsoft.Win32;

namespace EE.Doklad.Views
{
    public partial class ExternalWallsSectionEditor : UserControl
    {
        private const int MaxWallTypes = 8;
        private readonly MaterialsService _materialsService;

        // Tracks the wall type whose popup is currently open
        private ExternalWallType? _popupWallType;

        // Dictionary to track selected thermal bridge items per Settings object
        private readonly Dictionary<WallThermalBridgeSettings, WallThermalBridgeItem?> _selectedTbItems = new();

        public ObservableCollection<MaterialOption> MaterialOptions { get; } = new();

        public ExternalWallsSectionEditor()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;

            // Initialize materials service
            _materialsService = new MaterialsService(new JsonMaterialsRepository());
            LoadMaterialOptions();
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

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Ensure column visibility is correct for the current data
            AttachWallTypesHandlers(e.OldValue as ExternalWallsSectionData, e.NewValue as ExternalWallsSectionData);
            UpdateFacadeColumnsVisibility();
            InjectMaterialOptionsIntoLayers();
        }

        private void AttachWallTypesHandlers(ExternalWallsSectionData? oldData, ExternalWallsSectionData? newData)
        {
            if (oldData != null)
            {
                oldData.WallTypes.CollectionChanged -= WallTypes_CollectionChanged;
                foreach (var wt in oldData.WallTypes)
                {
                    wt.PropertyChanged -= WallType_PropertyChanged;
                }
            }

            if (newData != null)
            {
                newData.WallTypes.CollectionChanged += WallTypes_CollectionChanged;
                foreach (var wt in newData.WallTypes)
                {
                    wt.PropertyChanged += WallType_PropertyChanged;
                    // Initial recalc so Hel/Htb/Htotal are populated on load
                    ThermalBridgeCalculator.Recalculate(wt);
                }
            }
        }

        private void WallTypes_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ExternalWallType wt in e.OldItems)
                {
                    wt.PropertyChanged -= WallType_PropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (ExternalWallType wt in e.NewItems)
                {
                    wt.PropertyChanged += WallType_PropertyChanged;
                    // Initial recalc for newly added wall
                    ThermalBridgeCalculator.Recalculate(wt);
                }
            }

            UpdateFacadeColumnsVisibility();
        }

        private void WallType_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not ExternalWallType wall) return;

            // Recalculate column visibility if any facade area changed
            if (e.PropertyName is nameof(ExternalWallType.FacadeEast)
                or nameof(ExternalWallType.FacadeNorth)
                or nameof(ExternalWallType.FacadeWest)
                or nameof(ExternalWallType.FacadeSouth)
                or nameof(ExternalWallType.FacadeNorthEast)
                or nameof(ExternalWallType.FacadeNorthWest)
                or nameof(ExternalWallType.FacadeSouthEast)
                or nameof(ExternalWallType.FacadeSouthWest))
            {
                UpdateFacadeColumnsVisibility();
                // Area changed → Hel must be recalculated
                ThermalBridgeCalculator.Recalculate(wall);
            }

            // U changed (layer edit, Rsi/Rse change) → Hel must be recalculated
            if (e.PropertyName is nameof(ExternalWallType.Uw))
            {
                ThermalBridgeCalculator.Recalculate(wall);
            }
        }

        private void InjectMaterialOptionsIntoLayers()
        {
            if (DataContext is not ExternalWallsSectionData data)
                return;

            var optionsList = MaterialOptions.ToList() as IReadOnlyList<MaterialOption>;

            foreach (var wallType in data.WallTypes)
            {
                foreach (var layer in wallType.Layers)
                {
                    layer.MaterialOptions = optionsList;
                }
            }
        }

        private void AddWallType_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not ExternalWallsSectionData data)
            {
                return;
            }

            if (data.WallTypes.Count >= MaxWallTypes)
            {
                MessageBox.Show("Максимум 8 типа външни стени.", "Ограничение",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var wallType = new ExternalWallType
            {
                Index = data.WallTypes.Count + 1,
                Name = $"Тип стена {data.WallTypes.Count + 1}"
            };
            var layer = new ExternalWallLayer
            {
                MaterialOptions = MaterialOptions.ToList()
            };
            wallType.Layers.Add(layer);
            data.WallTypes.Add(wallType);
            UpdateIndexes(data);
            UpdateFacadeColumnsVisibility();
        }

        private void RemoveWallType_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not ExternalWallsSectionData data)
            {
                return;
            }

            if (sender is Button button && button.Tag is ExternalWallType tagged)
            {
                data.WallTypes.Remove(tagged);
                UpdateIndexes(data);
                UpdateFacadeColumnsVisibility();
                return;
            }

            if (data.WallTypes.Any())
            {
                data.WallTypes.RemoveAt(data.WallTypes.Count - 1);
                UpdateIndexes(data);
                UpdateFacadeColumnsVisibility();
            }
        }

        private void AddLayer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ExternalWallType wallType)
            {
                var layer = new ExternalWallLayer
                {
                    MaterialOptions = MaterialOptions.ToList()
                };
                wallType.Layers.Add(layer);
            }
        }

        private void RemoveLayer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ExternalWallType wallType && wallType.Layers.Any())
            {
                wallType.Layers.RemoveAt(wallType.Layers.Count - 1);
            }
        }

        private void UploadScheme_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not ExternalWallType wallType)
            {
                return;
            }

            var filePath = SelectImageFile();
            if (filePath == null)
            {
                return;
            }

            var newAttachment = new AttachmentData();
            LoadImage(newAttachment, filePath);
            wallType.SchemeAttachment = newAttachment;
        }

        private void RemoveScheme_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not ExternalWallType wallType)
            {
                return;
            }

            if (wallType.SchemeAttachment == null)
            {
                return;
            }

            wallType.SchemeAttachment = new AttachmentData();
        }

        private void FacadeOptions_Changed(object sender, RoutedEventArgs e)
        {
            UpdateFacadeColumnsVisibility();
        }

        private void HideEmptyColumns_Changed(object sender, RoutedEventArgs e)
        {
            UpdateFacadeColumnsVisibility();
        }

        private void UpdateFacadeColumnsVisibility()
        {
            // If grid columns not yet created, skip
            if (SummaryGrid == null)
                return;

            // Determine which groups are requested
            var showCardinal = (FindName("ShowCardinalCheckBox") as CheckBox)?.IsChecked == true;
            var showRotated = (FindName("ShowRotatedCheckBox") as CheckBox)?.IsChecked == true;
            var hideEmpty = (FindName("HideEmptyColumnsCheckBox") as CheckBox)?.IsChecked == true;

            // Default behavior: if neither group selected, show both groups (show all directions)
            if (!showCardinal && !showRotated)
            {
                showCardinal = true;
                showRotated = true;
            }

            // Collect totals from data
            double totalEast = 0, totalNorth = 0, totalWest = 0, totalSouth = 0;
            double totalNE = 0, totalNW = 0, totalSE = 0, totalSW = 0;
            if (DataContext is ExternalWallsSectionData data)
            {
                foreach (var wt in data.WallTypes)
                {
                    totalEast += wt.FacadeEast;
                    totalNorth += wt.FacadeNorth;
                    totalWest += wt.FacadeWest;
                    totalSouth += wt.FacadeSouth;
                    totalNE += wt.FacadeNorthEast;
                    totalNW += wt.FacadeNorthWest;
                    totalSE += wt.FacadeSouthEast;
                    totalSW += wt.FacadeSouthWest;
                }
            }

            // Decide visibility per column (true = visible)
            bool visNorth = showCardinal ? true : showRotated ? false : false;
            bool visEast = showCardinal ? true : showRotated ? false : false;
            bool visWest = showCardinal ? true : showRotated ? false : false;
            bool visSouth = showCardinal ? true : showRotated ? false : false;

            bool visNE = showRotated ? true : false;
            bool visNW = showRotated ? true : false;
            bool visSE = showRotated ? true : false;
            bool visSW = showRotated ? true : false;

            // If both groups are checked, show all
            if (showCardinal && showRotated)
            {
                visNorth = visEast = visWest = visSouth = visNE = visNW = visSE = visSW = true;
            }

            // If hideEmpty is set, collapse those with zero totals
            if (hideEmpty)
            {
                if (totalNorth == 0) visNorth = false;
                if (totalEast == 0) visEast = false;
                if (totalWest == 0) visWest = false;
                if (totalSouth == 0) visSouth = false;
                if (totalNE == 0) visNE = false;
                if (totalNW == 0) visNW = false;
                if (totalSE == 0) visSE = false;
                if (totalSW == 0) visSW = false;
            }

            // Apply visibility
            NorthColumn.Visibility = visNorth ? Visibility.Visible : Visibility.Collapsed;
            NorthEastColumn.Visibility = visNE ? Visibility.Visible : Visibility.Collapsed;
            EastColumn.Visibility = visEast ? Visibility.Visible : Visibility.Collapsed;
            SouthEastColumn.Visibility = visSE ? Visibility.Visible : Visibility.Collapsed;
            SouthColumn.Visibility = visSouth ? Visibility.Visible : Visibility.Collapsed;
            SouthWestColumn.Visibility = visSW ? Visibility.Visible : Visibility.Collapsed;
            WestColumn.Visibility = visWest ? Visibility.Visible : Visibility.Collapsed;
            NorthWestColumn.Visibility = visNW ? Visibility.Visible : Visibility.Collapsed;

            // Re-order display indexes to follow requested order: С, СИ, И, ЮИ, ЮГ, ЮЗ, З, СЗ
            var desiredOrder = new List<DataGridColumn>
            {
                NorthColumn,       // С
                NorthEastColumn,   // СИ
                EastColumn,        // И
                SouthEastColumn,   // ЮИ
                SouthColumn,       // Ю
                SouthWestColumn,   // ЮЗ
                WestColumn,        // З
                NorthWestColumn    // СЗ
            };

            // Base index is number of fixed columns before the facade direction columns
            // (№, Тип стена, A (m²), U (W/m²K), α, ε) == 6
            // After facade columns: Режим ТМ, Htb, Hel, Htotal, Премахни — do not include them here
            int baseIndex = 6;
            // Compute visible and collapsed lists to assign contiguous valid DisplayIndex values
            var visibleCols = desiredOrder.Where(c => c.Visibility == Visibility.Visible).ToList();
            var collapsedCols = desiredOrder.Where(c => c.Visibility != Visibility.Visible).ToList();

            int displayPos = baseIndex;
            // Assign display indexes for visible columns sequentially
            foreach (var col in visibleCols)
            {
                col.DisplayIndex = displayPos++;
            }

            // Assign remaining display indexes to collapsed columns so indices stay within range
            foreach (var col in collapsedCols)
            {
                col.DisplayIndex = displayPos++;
            }
        }

        // Removed global search/filter handlers to avoid refreshing a shared ICollectionView.

        private static void UpdateIndexes(ExternalWallsSectionData data)
        {
            for (int i = 0; i < data.WallTypes.Count; i++)
            {
                data.WallTypes[i].Index = i + 1;
            }
        }

        private static string? SelectImageFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Изображения|*.png;*.jpg;*.jpeg",
                Title = "Изберете изображение"
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        private static void LoadImage(AttachmentData attachment, string filePath)
        {
            try
            {
                attachment.FileName = Path.GetFileName(filePath);
                attachment.Bytes = File.ReadAllBytes(filePath);
                var ext = Path.GetExtension(filePath).ToLowerInvariant();
                attachment.ContentType = ext switch
                {
                    ".png" => "image/png",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    _ => "application/octet-stream"
                };
                attachment.SourcePageCount = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Грешка при зареждане: {ex.Message}",
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ──────────────────────────────────────────────────────────────────
        //  α / ε column cell click → orientation breakdown popup
        // ──────────────────────────────────────────────────────────────────

        private static readonly (WallOrientation Orientation, string Label,
            Func<ExternalWallType, double> GetArea)[] _orientationMeta =
        {
            (WallOrientation.NE, "СИ", w => w.FacadeNorthEast),
            (WallOrientation.E,  "И",  w => w.FacadeEast),
            (WallOrientation.SE, "ЮИ", w => w.FacadeSouthEast),
            (WallOrientation.S,  "Ю",  w => w.FacadeSouth),
            (WallOrientation.SW, "ЮЗ", w => w.FacadeSouthWest),
            (WallOrientation.W,  "З",  w => w.FacadeWest),
            (WallOrientation.NW, "СЗ", w => w.FacadeNorthWest),
        };

        private void AlphaEpsilonCell_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.DataContext is not ExternalWallType wallType) return;

            _popupWallType = wallType;
            RefreshOrientationPopup(wallType);
            OrientationPopup.IsOpen = true;
        }

        private void PopupEditValues_Click(object sender, RoutedEventArgs e)
        {
            OrientationPopup.IsOpen = false;
            if (_popupWallType == null) return;

            // Expand the surface-params panel for that wall type so user can edit
            _popupWallType.SurfaceProperties.IsExpanded = true;

            // Scroll to and focus the detail card – find the ItemsControl container
            if (WallTypesItemsControl.ItemContainerGenerator
                    .ContainerFromItem(_popupWallType) is FrameworkElement container)
            {
                container.BringIntoView();
            }
        }

        private void PopupCopyDefault_Click(object sender, RoutedEventArgs e)
        {
            if (_popupWallType == null) return;

            var sp = _popupWallType.SurfaceProperties;

            var result = MessageBox.Show(
                $"Заменяне на всички ориентационни стойности с:\n\n" +
                $"  α = {sp.AlphaDefault:0.00}\n" +
                $"  ε = {sp.EpsilonDefault:0.00}\n\n" +
                "Сигурни ли сте?",
                "Копиране на default стойности",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                // User said No – keep popup open with current values unchanged
                return;
            }

            foreach (WallOrientation o in Enum.GetValues(typeof(WallOrientation)))
            {
                if (!sp.Overrides.ContainsKey(o))
                    sp.Overrides[o] = new SurfaceProps();
                sp.Overrides[o].Alpha   = sp.AlphaDefault;
                sp.Overrides[o].Epsilon = sp.EpsilonDefault;
            }

            // Refresh popup rows to show the updated values
            RefreshOrientationPopup(_popupWallType);
        }

        /// <summary>Rebuilds the popup table rows and title for the given wall type.</summary>
        private void RefreshOrientationPopup(ExternalWallType wallType)
        {
            var rows = _orientationMeta.Select(m => new OrientationRow
            {
                Label   = m.Label,
                Alpha   = wallType.SurfaceProperties.GetAlpha(m.Orientation),
                Epsilon = wallType.SurfaceProperties.GetEpsilon(m.Orientation),
                Area    = m.GetArea(wallType)
            }).ToList();

            OrientationBreakdownGrid.ItemsSource = rows;

            PopupTitle.Text = wallType.SurfaceProperties.UseOrientationOverride
                ? "Повърхностни параметри по ориентации  ▸ ориент. режим"
                : "Повърхностни параметри по ориентации  ▸ еднакви стойности";
        }

        private void AlphaEpsilonCell_ClickForWallType(ExternalWallType wallType)
        {
            _popupWallType = wallType;
            RefreshOrientationPopup(wallType);
            OrientationPopup.IsOpen = true;
        }

        // ──────────────────────────────────────────────────────────────────
        //  Thermal Bridges – ComboBox mode switch
        // ──────────────────────────────────────────────────────────────────

        private void TbMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ComboBox cb) return;
            if (cb.SelectedItem is not ComboBoxItem item) return;

            // Resolve the WallThermalBridgeSettings from Tag or DataContext
            WallThermalBridgeSettings? settings = null;

            if (cb.Tag is WallThermalBridgeSettings tbTag)
                settings = tbTag;
            else if (cb.DataContext is WallThermalBridgeSettings tbDc)
                settings = tbDc;

            if (settings == null) return;

            settings.Mode = item.Tag?.ToString() switch
            {
                "GlobalPercentage" => ThermalBridgeMode.GlobalPercentage,
                "Manual"           => ThermalBridgeMode.Manual,
                _                  => ThermalBridgeMode.None
            };

            RecalcThermalBridgesForSettings(settings);
        }

        private void TbGlobalPercent_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBox tb) return;
            if (tb.DataContext is not WallThermalBridgeSettings settings) return;
            RecalcThermalBridgesForSettings(settings);
        }

        // ──────────────────────────────────────────────────────────────────
        //  Thermal Bridges – Manual CRUD
        // ──────────────────────────────────────────────────────────────────

        private void TbGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not DataGrid grid) return;
            if (grid.DataContext is not WallThermalBridgeSettings settings) return;
            
            _selectedTbItems[settings] = grid.SelectedItem as WallThermalBridgeItem;
        }

        private void TbAdd_Click(object sender, RoutedEventArgs e)
        {
            var settings = GetSettingsFromButton(sender);
            if (settings == null) return;

            var dlg = new WallThermalBridgeItemDialog { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() == true && dlg.Result != null)
            {
                settings.Items.Add(dlg.Result);
                RecalcThermalBridgesForSettings(settings);
            }
        }

        private void TbEdit_Click(object sender, RoutedEventArgs e)
        {
            var settings = GetSettingsFromButton(sender);
            if (settings == null) return;

            // Get selected item from dictionary
            if (!_selectedTbItems.TryGetValue(settings, out var selected) || selected == null)
            {
                MessageBox.Show("Моля изберете термомост от таблицата.", "Информация", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dlg = new WallThermalBridgeItemDialog(selected) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() == true)
            {
                RecalcThermalBridgesForSettings(settings);
            }
        }

        private void TbDelete_Click(object sender, RoutedEventArgs e)
        {
            var settings = GetSettingsFromButton(sender);
            if (settings == null) return;

            // Get selected item from dictionary
            if (!_selectedTbItems.TryGetValue(settings, out var selected) || selected == null)
            {
                MessageBox.Show("Моля изберете термомост от таблицата.", "Информация", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            settings.Items.Remove(selected);
            _selectedTbItems[settings] = null;
            RecalcThermalBridgesForSettings(settings);
        }

        private static WallThermalBridgeSettings? GetSettingsFromButton(object sender)
        {
            if (sender is not Button btn) return null;
            if (btn.Tag is WallThermalBridgeSettings s) return s;
            if (btn.DataContext is WallThermalBridgeSettings d) return d;
            return null;
        }

        private void RecalcThermalBridgesForSettings(WallThermalBridgeSettings settings)
        {
            if (DataContext is not ExternalWallsSectionData data) return;
            var wall = data.WallTypes.FirstOrDefault(w => w.ThermalBridges == settings);
            if (wall != null)
                ThermalBridgeCalculator.Recalculate(wall);
        }

        // ──────────────────────────────────────────────────────────────────
        //  Thermal Bridges – Summary cell popup (аналогично на α/ε)
        // ──────────────────────────────────────────────────────────────────

        private ExternalWallType? _tbPopupWallType;

        private void ThermalBridgeModeCell_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.DataContext is not ExternalWallType wallType) return;

            _tbPopupWallType = wallType;
            RefreshThermalBridgesPopup(wallType);
            ThermalBridgesPopup.IsOpen = true;
        }

        private void RefreshThermalBridgesPopup(ExternalWallType wallType)
        {
            var tb = wallType.ThermalBridges;

            TbPopupTitle.Text = $"Топлинни мостове  ▸ {wallType.Name}";

            TbPopupMode.Text = tb.Mode switch
            {
                ThermalBridgeMode.GlobalPercentage => "Глобална стойност",
                ThermalBridgeMode.Manual           => "Детайлно въвеждане",
                _                                  => "Няма"
            };

            TbPopupPercentRow.Visibility = tb.Mode == ThermalBridgeMode.GlobalPercentage
                ? Visibility.Visible : Visibility.Collapsed;
            TbPopupPercent.Text = $"{tb.GlobalPercent:0.0} %";

            TbPopupCountRow.Visibility = tb.Mode == ThermalBridgeMode.Manual
                ? Visibility.Visible : Visibility.Collapsed;
            TbPopupCount.Text = tb.Items.Count.ToString();

            TbPopupHel.Text    = tb.Hel.ToString("0.000");
            TbPopupHtb.Text    = tb.Htb.ToString("0.000");
            TbPopupHtotal.Text = tb.Htotal.ToString("0.000");
        }

        private void TbPopupNavigate_Click(object sender, RoutedEventArgs e)
        {
            ThermalBridgesPopup.IsOpen = false;
            if (_tbPopupWallType == null) return;

            // Expand the thermal bridges panel for that wall type
            _tbPopupWallType.ThermalBridges.IsExpanded = true;

            // Scroll to and focus the detail card
            if (WallTypesItemsControl.ItemContainerGenerator
                    .ContainerFromItem(_tbPopupWallType) is FrameworkElement container)
            {
                container.BringIntoView();
            }
        }
    }   // end class ExternalWallsSectionEditor

    // ──────────────────────────────────────────────────────────────────
    //  Helper DTO for the orientation breakdown DataGrid
    // ──────────────────────────────────────────────────────────────────
    internal sealed class OrientationRow
    {
        public string Label   { get; set; } = string.Empty;
        public double Alpha   { get; set; }
        public double Epsilon { get; set; }
        public double Area    { get; set; }
    }
}
