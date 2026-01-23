using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;

namespace EE.Doklad.Views.FloorPanels
{
    public partial class FloorHeatedBasementPanel : UserControl
    {
        public FloorHeatedBasementPanel()
        {
            InitializeComponent();
        }

        private void AddFloorLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorHeatedBasementDetail detail)
            {
                detail.FloorLayers.Add(new FloorLayer { Material = "Бетон", Thickness = 0.2, Lambda = 1.7 });
            }
        }

        private void RemoveFloorLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorHeatedBasementDetail detail && detail.FloorLayers.Count > 0)
            {
                detail.FloorLayers.RemoveAt(detail.FloorLayers.Count - 1);
            }
        }

        private void AddWallLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorHeatedBasementDetail detail)
            {
                detail.WallLayers.Add(new FloorLayer { Material = "Бетон", Thickness = 0.25, Lambda = 1.7 });
            }
        }

        private void RemoveWallLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorHeatedBasementDetail detail && detail.WallLayers.Count > 0)
            {
                detail.WallLayers.RemoveAt(detail.WallLayers.Count - 1);
            }
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

        private void UploadHeatedBasementFloorScheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorHeatedBasementDetail detail)
            {
                var filePath = SelectImageFile();
                if (filePath == null)
                    return;
                var newAttachment = new EE.Doklad.Models.AttachmentData();
                LoadImage(newAttachment, filePath);
                detail.HeatedBasementFloorSchemeAttachment = newAttachment;
            }
        }

        private void RemoveHeatedBasementFloorScheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorHeatedBasementDetail detail)
            {
                detail.HeatedBasementFloorSchemeAttachment = new EE.Doklad.Models.AttachmentData();
            }
        }

        private void UploadHeatedBasementWallScheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorHeatedBasementDetail detail)
            {
                var filePath = SelectImageFile();
                if (filePath == null)
                    return;
                var newAttachment = new EE.Doklad.Models.AttachmentData();
                LoadImage(newAttachment, filePath);
                detail.HeatedBasementWallSchemeAttachment = newAttachment;
            }
        }

        private void RemoveHeatedBasementWallScheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorHeatedBasementDetail detail)
            {
                detail.HeatedBasementWallSchemeAttachment = new EE.Doklad.Models.AttachmentData();
            }
        }
    }
}
