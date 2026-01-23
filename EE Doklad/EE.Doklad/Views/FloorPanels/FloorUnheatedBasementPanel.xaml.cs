using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;

namespace EE.Doklad.Views.FloorPanels
{
    public partial class FloorUnheatedBasementPanel : UserControl
    {
        public FloorUnheatedBasementPanel()
        {
            InitializeComponent();
        }

        private void AddFloorToBasementLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail)
            {
                detail.FloorToBasementLayers.Add(new FloorLayer { Material = "Нов слой", Thickness = 0.1, Lambda = 1.0 });
            }
        }

        private void RemoveFloorToBasementLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail && detail.FloorToBasementLayers.Count > 0)
            {
                detail.FloorToBasementLayers.RemoveAt(detail.FloorToBasementLayers.Count - 1);
            }
        }

        private void AddBasementFloorLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail)
            {
                detail.BasementFloorLayers.Add(new FloorLayer { Material = "Нов слой", Thickness = 0.1, Lambda = 1.0 });
            }
        }

        private void RemoveBasementFloorLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail && detail.BasementFloorLayers.Count > 0)
            {
                detail.BasementFloorLayers.RemoveAt(detail.BasementFloorLayers.Count - 1);
            }
        }

        private void AddBasementWallLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail)
            {
                detail.BasementWallLayers.Add(new FloorLayer { Material = "Нов слой", Thickness = 0.1, Lambda = 1.0 });
            }
        }

        private void RemoveBasementWallLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail && detail.BasementWallLayers.Count > 0)
            {
                detail.BasementWallLayers.RemoveAt(detail.BasementWallLayers.Count - 1);
            }
        }

        private void AddWallAboveGradeLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail)
            {
                detail.WallAboveGradeLayers.Add(new FloorLayer { Material = "Нов слой", Thickness = 0.1, Lambda = 1.0 });
            }
        }

        private void RemoveWallAboveGradeLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail && detail.WallAboveGradeLayers.Count > 0)
            {
                detail.WallAboveGradeLayers.RemoveAt(detail.WallAboveGradeLayers.Count - 1);
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

        private void UploadFloorToBasementScheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail)
            {
                var filePath = SelectImageFile();
                if (filePath == null)
                    return;
                var newAttachment = new EE.Doklad.Models.AttachmentData();
                LoadImage(newAttachment, filePath);
                detail.FloorToBasementSchemeAttachment = newAttachment;
            }
        }

        private void RemoveFloorToBasementScheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail)
            {
                detail.FloorToBasementSchemeAttachment = new EE.Doklad.Models.AttachmentData();
            }
        }

        private void UploadBasementFloorScheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail)
            {
                var filePath = SelectImageFile();
                if (filePath == null)
                    return;
                var newAttachment = new EE.Doklad.Models.AttachmentData();
                LoadImage(newAttachment, filePath);
                detail.BasementFloorSchemeAttachment = newAttachment;
            }
        }

        private void RemoveBasementFloorScheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail)
            {
                detail.BasementFloorSchemeAttachment = new EE.Doklad.Models.AttachmentData();
            }
        }

        private void UploadBasementWallScheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail)
            {
                var filePath = SelectImageFile();
                if (filePath == null)
                    return;
                var newAttachment = new EE.Doklad.Models.AttachmentData();
                LoadImage(newAttachment, filePath);
                detail.BasementWallSchemeAttachment = newAttachment;
            }
        }

        private void RemoveBasementWallScheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail)
            {
                detail.BasementWallSchemeAttachment = new EE.Doklad.Models.AttachmentData();
            }
        }

        private void UploadWallAboveGradeScheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail)
            {
                var filePath = SelectImageFile();
                if (filePath == null)
                    return;
                var newAttachment = new EE.Doklad.Models.AttachmentData();
                LoadImage(newAttachment, filePath);
                detail.WallAboveGradeSchemeAttachment = newAttachment;
            }
        }

        private void RemoveWallAboveGradeScheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail)
            {
                detail.WallAboveGradeSchemeAttachment = new EE.Doklad.Models.AttachmentData();
            }
        }
    }
}
