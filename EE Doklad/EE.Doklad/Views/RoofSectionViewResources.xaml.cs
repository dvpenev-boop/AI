using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;
using Microsoft.Win32;

namespace EE.Doklad.Views
{
    public partial class RoofSectionViewResources : ResourceDictionary
    {
        public RoofSectionViewResources()
        {
            InitializeComponent();
        }

        private void AddWarmLayer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is WarmRoofDetail detail)
            {
                detail.Layers.Add(new RoofLayer());
            }
        }

        private void RemoveWarmLayer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is WarmRoofDetail detail && detail.Layers.Any())
            {
                detail.Layers.RemoveAt(detail.Layers.Count - 1);
            }
        }

        private void AddColdLayer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is RoofLayerTable table)
            {
                table.Layers.Add(new RoofLayer());
            }
        }

        private void RemoveColdLayer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is RoofLayerTable table && table.Layers.Any())
            {
                table.Layers.RemoveAt(table.Layers.Count - 1);
            }
        }

        private void CalculateColdRoof_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is RoofType roofType && roofType.ColdDetail != null)
            {
                roofType.ColdDetail.CalculateUr();
            }
        }

        private void UploadScheme_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not RoofType roofType)
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
            roofType.SchemeAttachment = newAttachment;
        }

        private void RemoveScheme_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not RoofType roofType)
            {
                return;
            }

            roofType.SchemeAttachment = new AttachmentData();
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
            catch (IOException ex)
            {
                MessageBox.Show($"Грешка при зареждане: {ex.Message}",
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
