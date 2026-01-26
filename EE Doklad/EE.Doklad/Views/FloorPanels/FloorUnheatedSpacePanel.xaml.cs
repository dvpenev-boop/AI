using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;
using EE.Doklad.ViewModels;

namespace EE.Doklad.Views.FloorPanels
{
    public partial class FloorUnheatedSpacePanel : UserControl
    {
        public FloorUnheatedSpacePanel()
        {
            InitializeComponent();
        }

        private void AddLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedSpaceInput input)
            {
                var vm = FindFloorSectionViewModel();
                var materialOptions = vm?.MaterialOptions.ToList() as IReadOnlyList<MaterialOption>;
                var layer = new FloorLayer { MaterialOptions = materialOptions };
                input.Layers.Add(layer);
            }
        }

        private void RemoveLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedSpaceInput input && input.Layers.Count > 0)
            {
                input.Layers.RemoveAt(input.Layers.Count - 1);
            }
        }

        // Removed shared MaterialSearchText usage; using per-layer MaterialOptions + TextSearch.

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

        public FloorUnheatedSpaceInput GetInput()
        {
            return DataContext as FloorUnheatedSpaceInput ?? new FloorUnheatedSpaceInput();
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

        private void UploadUnheatedSpaceScheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedSpaceDetail detail)
            {
                var filePath = SelectImageFile();
                if (filePath == null)
                    return;
                var newAttachment = new EE.Doklad.Models.AttachmentData();
                LoadImage(newAttachment, filePath);
                detail.UnheatedSpaceSchemeAttachment = newAttachment;
            }
        }

        private void RemoveUnheatedSpaceScheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedSpaceDetail detail)
            {
                detail.UnheatedSpaceSchemeAttachment = new EE.Doklad.Models.AttachmentData();
            }
        }
    }
}
