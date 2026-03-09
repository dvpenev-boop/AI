using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;
using EE.Doklad.Sections.ThermalBridges;
using EE.Doklad.ViewModels;
using Microsoft.Win32;

namespace EE.Doklad.Views
{
    public partial class WarmRoofDetailControl : UserControl
    {
        private WallThermalBridgeItem? _selectedTbItem;

        public WarmRoofDetailControl()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        // ── DataContext tracking for Uw/Area → recalc ──────────────────

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is RoofType oldRoof)
            {
                oldRoof.PropertyChanged -= RoofType_PropertyChanged;
                if (oldRoof.WarmDetail != null)
                    oldRoof.WarmDetail.PropertyChanged -= WarmDetail_PropertyChanged;
            }
            if (e.NewValue is RoofType newRoof)
            {
                newRoof.PropertyChanged += RoofType_PropertyChanged;
                if (newRoof.WarmDetail != null)
                    newRoof.WarmDetail.PropertyChanged += WarmDetail_PropertyChanged;
                // Initial recalc on load
                ThermalBridgeCalculator.Recalculate(newRoof);
            }
        }

        private void RoofType_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RoofType.Area) && DataContext is RoofType roof)
                ThermalBridgeCalculator.Recalculate(roof);
        }

        private void WarmDetail_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WarmRoofDetail.Uw) && DataContext is RoofType roof)
                ThermalBridgeCalculator.Recalculate(roof);
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

        // ──────────────────────────────────────────────────────────────────
        //  Thermal Bridges handlers (shared logic with External Walls)
        // ──────────────────────────────────────────────────────────────────

        private void TbMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ComboBox cb) return;
            if (cb.SelectedItem is not ComboBoxItem item) return;
            if (DataContext is not RoofType roof) return;

            roof.ThermalBridges.Mode = item.Tag?.ToString() switch
            {
                "GlobalPercentage" => ThermalBridgeMode.GlobalPercentage,
                "Manual"           => ThermalBridgeMode.Manual,
                _                  => ThermalBridgeMode.None
            };
            ThermalBridgeCalculator.Recalculate(roof);
        }

        private void TbGlobalPercent_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoofType roof)
                ThermalBridgeCalculator.Recalculate(roof);
        }

        private void TbGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid grid)
                _selectedTbItem = grid.SelectedItem as WallThermalBridgeItem;
        }

        private void TbAdd_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not RoofType roof) return;
            var dlg = new WallThermalBridgeItemDialog(showFacades: false) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() == true && dlg.Result != null)
            {
                roof.ThermalBridges.Items.Add(dlg.Result);
                ThermalBridgeCalculator.Recalculate(roof);
            }
        }

        private void TbEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not RoofType roof) return;
            if (_selectedTbItem == null)
            {
                MessageBox.Show("Моля изберете термомост от таблицата.", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var dlg = new WallThermalBridgeItemDialog(_selectedTbItem, showFacades: false) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() == true)
                ThermalBridgeCalculator.Recalculate(roof);
        }

        private void TbDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not RoofType roof) return;
            if (_selectedTbItem == null)
            {
                MessageBox.Show("Моля изберете термомост от таблицата.", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            roof.ThermalBridges.Items.Remove(_selectedTbItem);
            _selectedTbItem = null;
            ThermalBridgeCalculator.Recalculate(roof);
        }
    }
}
