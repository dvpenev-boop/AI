using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;

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
                input.Layers.Add(new FloorLayer());
            }
        }

        private void RemoveLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedSpaceInput input && input.Layers.Count > 0)
            {
                input.Layers.RemoveAt(input.Layers.Count - 1);
            }
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
