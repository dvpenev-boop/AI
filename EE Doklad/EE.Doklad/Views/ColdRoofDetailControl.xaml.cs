using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;
using EE.Doklad.Sections.ThermalBridges;
using EE.Doklad.ViewModels;

namespace EE.Doklad.Views
{
    public partial class ColdRoofDetailControl : UserControl
    {
        private WallThermalBridgeItem? _selectedTbItem;

        public ColdRoofDetailControl()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        // ── DataContext tracking for Ur/Area → recalc ──────────────────

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is RoofType oldRoof)
            {
                oldRoof.PropertyChanged -= RoofType_PropertyChanged;
                if (oldRoof.ColdDetail != null)
                    oldRoof.ColdDetail.PropertyChanged -= ColdDetail_PropertyChanged;
            }
            if (e.NewValue is RoofType newRoof)
            {
                newRoof.PropertyChanged += RoofType_PropertyChanged;
                if (newRoof.ColdDetail != null)
                    newRoof.ColdDetail.PropertyChanged += ColdDetail_PropertyChanged;
                // Initial recalc on load
                ThermalBridgeCalculator.Recalculate(newRoof);
            }
        }

        private void RoofType_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RoofType.Area) && DataContext is RoofType roof)
                ThermalBridgeCalculator.Recalculate(roof);
        }

        private void ColdDetail_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ColdRoofDetail.Ur) && DataContext is RoofType roof)
                ThermalBridgeCalculator.Recalculate(roof);
        }

        private void AddU1Layer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoofType roofType && roofType.ColdDetail != null)
            {
                var vm = FindRoofSectionViewModel();
                var materialOptions = vm?.MaterialOptions.ToList() as IReadOnlyList<MaterialOption>;
                var layer = new RoofLayer { MaterialOptions = materialOptions };
                roofType.ColdDetail.U1.Layers.Add(layer);
            }
        }

        private void RemoveU1Layer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoofType roofType && roofType.ColdDetail != null && roofType.ColdDetail.U1.Layers.Any())
            {
                roofType.ColdDetail.U1.Layers.RemoveAt(roofType.ColdDetail.U1.Layers.Count - 1);
            }
        }

        private void AddU2Layer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoofType roofType && roofType.ColdDetail != null)
            {
                var vm = FindRoofSectionViewModel();
                var materialOptions = vm?.MaterialOptions.ToList() as IReadOnlyList<MaterialOption>;
                var layer = new RoofLayer { MaterialOptions = materialOptions };
                roofType.ColdDetail.U2.Layers.Add(layer);
            }
        }

        private void RemoveU2Layer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoofType roofType && roofType.ColdDetail != null && roofType.ColdDetail.U2.Layers.Any())
            {
                roofType.ColdDetail.U2.Layers.RemoveAt(roofType.ColdDetail.U2.Layers.Count - 1);
            }
        }

        private void AddUwLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoofType roofType && roofType.ColdDetail != null)
            {
                var vm = FindRoofSectionViewModel();
                var materialOptions = vm?.MaterialOptions.ToList() as IReadOnlyList<MaterialOption>;
                var layer = new RoofLayer { MaterialOptions = materialOptions };
                roofType.ColdDetail.Uw.Layers.Add(layer);
            }
        }

        private void RemoveUwLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoofType roofType && roofType.ColdDetail != null && roofType.ColdDetail.Uw.Layers.Any())
            {
                roofType.ColdDetail.Uw.Layers.RemoveAt(roofType.ColdDetail.Uw.Layers.Count - 1);
            }
        }

        // Removed handlers that updated a shared MaterialSearchText/filter. Using per-layer MaterialOptions + TextSearch.

        private RoofSectionViewModel? FindRoofSectionViewModel()
        {
            var parent = FindParentView();
            return parent?.DataContext as RoofSectionViewModel;
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoofType roofType && roofType.ColdDetail != null)
            {
                roofType.ColdDetail.CalculateAll();
            }
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

        // ──────────────────────────────────────────────────────────────────
        //  Thermal Bridges handlers (reuses same logic as External Walls)
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
