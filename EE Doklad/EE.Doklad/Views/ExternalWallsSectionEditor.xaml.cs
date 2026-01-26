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
            UpdateFacadeColumnsVisibility();
            InjectMaterialOptionsIntoLayers();
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
                return;
            }

            if (data.WallTypes.Any())
            {
                data.WallTypes.RemoveAt(data.WallTypes.Count - 1);
                UpdateIndexes(data);
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

        private void FacadeToggle_Changed(object sender, RoutedEventArgs e)
        {
            UpdateFacadeColumnsVisibility();
        }

        private void UpdateFacadeColumnsVisibility()
        {
            if (FacadeToggle == null)
            {
                return;
            }

            var show = FacadeToggle.IsChecked == true;
            EastColumn.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            NorthColumn.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            WestColumn.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            SouthColumn.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
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
