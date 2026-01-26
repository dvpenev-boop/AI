using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;
using EE.Doklad.ViewModels;

namespace EE.Doklad.Views.FloorPanels
{
    public partial class FloorGroundPanel : UserControl
    {
        public FloorGroundPanel()
        {
            InitializeComponent();
        }

        private void AddLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorGroundDetail detail)
            {
                var vm = FindFloorSectionViewModel();
                var materialOptions = vm?.MaterialOptions.ToList() as IReadOnlyList<MaterialOption>;
                var layer = new FloorLayer { MaterialOptions = materialOptions };
                detail.Layers.Add(layer);
            }
        }

        private void RemoveLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorGroundDetail detail && detail.Layers.Count > 0)
            {
                detail.Layers.RemoveAt(detail.Layers.Count - 1);
            }
        }

        // Removed handlers that updated a shared MaterialSearchText/filter.

        private FloorSectionViewModel? FindFloorSectionViewModel()
        {
            var parent = FindParentView();
            return parent?.DataContext as FloorSectionViewModel;
        }

        private FloorSectionView? FindParentView()
        {
            DependencyObject current = this;
            while (current != null)
            {
                if (current is FloorSectionView view)
                {
                    return view;
                }
                current = System.Windows.Media.VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        public FloorGroundInput GetInput()
        {
            return DataContext as FloorGroundInput ?? new FloorGroundInput(); // Not used, kept for compatibility
        }

        public static string? SelectImageFile()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Изображения|*.png;*.jpg;*.jpeg",
                Title = "Изберете изображение"
            };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public static void LoadImage(EE.Doklad.Models.AttachmentData attachment, string filePath)
        {
            try
            {
                attachment.AttachmentFileName = System.IO.Path.GetFileName(filePath);
                attachment.Data = System.IO.File.ReadAllBytes(filePath);
                var ext = System.IO.Path.GetExtension(filePath).ToLowerInvariant();
                attachment.MimeType = ext switch
                {
                    ".png" => "image/png",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    _ => "application/octet-stream"
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Грешка при зареждане: {ex.Message}", "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UploadGroundScheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorGroundDetail detail)
            {
                var filePath = SelectImageFile();
                if (filePath == null)
                    return;
                var newAttachment = new EE.Doklad.Models.AttachmentData();
                LoadImage(newAttachment, filePath);
                detail.GroundSchemeAttachment = newAttachment;
            }
        }

        private void RemoveGroundScheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorGroundDetail detail)
            {
                detail.GroundSchemeAttachment = new EE.Doklad.Models.AttachmentData();
            }
        }
    }
}
