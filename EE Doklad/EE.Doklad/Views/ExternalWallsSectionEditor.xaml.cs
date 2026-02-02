using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using EE.Doklad.Models;
using EE.Doklad.Services;
using Microsoft.Win32;

namespace EE.Doklad.Views
{
    public partial class ExternalWallsSectionEditor : UserControl
    {
        private const int MaxWallTypes = 8;
        private readonly MaterialsService _materialsService;

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
                }
            }

            UpdateFacadeColumnsVisibility();
        }

        private void WallType_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
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

            // Base index is number of columns before the facade columns (№, Тип стена, A (m²), U (W/m²K)) == 4
            int baseIndex = 4;
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
    }
}
