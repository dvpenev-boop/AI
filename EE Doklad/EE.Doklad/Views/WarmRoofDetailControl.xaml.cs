using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;
using EE.Doklad.ViewModels;
using Microsoft.Win32;

namespace EE.Doklad.Views
{
    public partial class WarmRoofDetailControl : UserControl
    {
        public WarmRoofDetailControl()
        {
            InitializeComponent();
        }

        private void AddLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoofType roofType && roofType.WarmDetail != null)
            {
                var vm = FindRoofSectionViewModel();
                var materialOptions = vm?.MaterialOptions.ToList() as IReadOnlyList<MaterialOption>;
                var layer = new RoofLayer { MaterialOptions = materialOptions };
                roofType.WarmDetail.Layers.Add(layer);
            }
        }

        private void RemoveLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoofType roofType && roofType.WarmDetail != null && roofType.WarmDetail.Layers.Any())
            {
                roofType.WarmDetail.Layers.RemoveAt(roofType.WarmDetail.Layers.Count - 1);
            }
        }

        // Using WPF built-in TextSearch; removed code that updated a shared MaterialSearchText/filter.

        private RoofSectionViewModel? FindRoofSectionViewModel()
        {
            var parent = FindParentView();
            return parent?.DataContext as RoofSectionViewModel;
        }

        private void UploadScheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not RoofType roofType)
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
            if (DataContext is not RoofType roofType)
            {
                return;
            }

            roofType.SchemeAttachment = new AttachmentData();
        }

        private void RemoveDetail_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoofType roofType)
            {
                // Find the parent view and call its removal logic
                var parent = FindParentView();
                if (parent != null && parent.DataContext is ViewModels.RoofSectionViewModel vm)
                {
                    vm.RemoveRoofTypeCommand.Execute(roofType);
                }
            }
        }

        private RoofSectionView? FindParentView()
        {
            DependencyObject current = this;
            while (current != null)
            {
                if (current is RoofSectionView view)
                {
                    return view;
                }
                current = System.Windows.Media.VisualTreeHelper.GetParent(current);
            }
            return null;
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
