using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using EE.Doklad.Models;
using EE.Doklad.ViewModels;

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
                var viewModel = FindFloorSectionViewModel();
                var newLayer = new FloorLayer { Material = "Бетон", Thickness = 0.2, Lambda = 1.7 };
                if (viewModel != null)
                {
                    newLayer.MaterialOptions = viewModel.MaterialOptions;
                }
                detail.FloorLayers.Add(newLayer);
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
                var viewModel = FindFloorSectionViewModel();
                var newLayer = new FloorLayer { Material = "Бетон", Thickness = 0.25, Lambda = 1.7 };
                if (viewModel != null)
                {
                    newLayer.MaterialOptions = viewModel.MaterialOptions;
                }
                detail.WallLayers.Add(newLayer);
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

        private void MaterialComboBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (sender is ComboBox comboBox && !string.IsNullOrEmpty(comboBox.Text))
            {
                var viewModel = FindFloorSectionViewModel();
                if (viewModel != null)
                {
                    viewModel.MaterialSearchText = comboBox.Text;
                    if (viewModel.MaterialOptionsView != null)
                    {
                        viewModel.MaterialOptionsView.Refresh();
                        comboBox.IsDropDownOpen = true;
                    }
                }
            }
        }

        private void MaterialComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Clear filter when ComboBox loses focus to restore all items
            var vm = FindFloorSectionViewModel();
            if (vm != null)
            {
                vm.MaterialSearchText = string.Empty;
            }
        }

        private FloorSectionViewModel? FindFloorSectionViewModel()
        {
            var parent = FindParentView(this);
            return parent?.DataContext as FloorSectionViewModel;
        }

        private FloorSectionView? FindParentView(DependencyObject child)
        {
            var parent = System.Windows.Media.VisualTreeHelper.GetParent(child);
            while (parent != null)
            {
                if (parent is FloorSectionView view)
                    return view;
                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            }
            return null;
        }
    }
}
