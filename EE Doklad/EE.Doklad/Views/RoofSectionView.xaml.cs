using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;
using EE.Doklad.Sections.ThermalBridges;
using EE.Doklad.ViewModels;

namespace EE.Doklad.Views
{
    public partial class RoofSectionView : UserControl
    {
        // Tracks the roof type whose popup is currently open
        private RoofType? _tbPopupRoofType;

        public RoofSectionView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        // ── DataContext tracking ─────────────────────────────────────────

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is RoofSectionViewModel oldVm)
            {
                oldVm.RoofTypes.CollectionChanged -= RoofTypes_CollectionChanged;
                foreach (var rt in oldVm.RoofTypes)
                    rt.PropertyChanged -= RoofType_PropertyChanged;
            }
            if (e.NewValue is RoofSectionViewModel newVm)
            {
                newVm.RoofTypes.CollectionChanged += RoofTypes_CollectionChanged;
                foreach (var rt in newVm.RoofTypes)
                {
                    rt.PropertyChanged += RoofType_PropertyChanged;
                    ThermalBridgeCalculator.Recalculate(rt);
                }
            }
        }

        private void RoofTypes_CollectionChanged(object? sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (RoofType rt in e.OldItems)
                    rt.PropertyChanged -= RoofType_PropertyChanged;

            if (e.NewItems != null)
                foreach (RoofType rt in e.NewItems)
                {
                    rt.PropertyChanged += RoofType_PropertyChanged;
                    ThermalBridgeCalculator.Recalculate(rt);
                }
        }

        private void RoofType_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not RoofType rt) return;
            if (e.PropertyName is nameof(RoofType.Area)
                or nameof(RoofType.UValue)
                or nameof(RoofType.UDisplay))
            {
                ThermalBridgeCalculator.Recalculate(rt);
            }
        }

        // ── Add roof button ──────────────────────────────────────────────

        private void AddRoof_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as RoofSectionViewModel;
            if (viewModel == null) return;

            var dialog = new RoofTypeSelectionDialog();
            if (dialog.ShowDialog() == true)
            {
                if (!dialog.SelectedMode.HasValue) return;
                
                if (!viewModel.TryAddRoof(dialog.SelectedMode.Value, out var error))
                {
                    MessageBox.Show(error, "Грешка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        // ── Thermal Bridges – Summary cell popup ─────────────────────────

        private void ThermalBridgeModeCell_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not System.Windows.Controls.Button btn) return;
            if (btn.DataContext is not RoofType roofType) return;

            _tbPopupRoofType = roofType;
            RefreshThermalBridgesPopup(roofType);
            ThermalBridgesPopup.IsOpen = true;
        }

        private void RefreshThermalBridgesPopup(RoofType roofType)
        {
            var tb = roofType.ThermalBridges;

            TbPopupTitle.Text = $"Топлинни мостове  ▸ {roofType.Name}";

            TbPopupMode.Text = tb.Mode switch
            {
                ThermalBridgeMode.GlobalPercentage => "Глобална стойност",
                ThermalBridgeMode.Manual           => "Ръчно въвеждане",
                _                                  => "Няма"
            };

            TbPopupPercentRow.Visibility = tb.Mode == ThermalBridgeMode.GlobalPercentage
                ? Visibility.Visible : Visibility.Collapsed;
            TbPopupPercent.Text = $"{tb.GlobalPercent:0.0} %";

            TbPopupCountRow.Visibility = tb.Mode == ThermalBridgeMode.Manual
                ? Visibility.Visible : Visibility.Collapsed;
            TbPopupCount.Text = tb.Items.Count.ToString();

            TbPopupHel.Text    = tb.Hel.ToString("0.000");
            TbPopupHtb.Text    = tb.Htb.ToString("0.000");
            TbPopupHtotal.Text = tb.Htotal.ToString("0.000");
        }

        private void TbPopupNavigate_Click(object sender, RoutedEventArgs e)
        {
            ThermalBridgesPopup.IsOpen = false;
            if (_tbPopupRoofType == null) return;

            // Expand the thermal bridges panel for that roof type
            _tbPopupRoofType.ThermalBridges.IsExpanded = true;

            // Scroll to the detail card inside the WarmRoofs or ColdRoofs ItemsControl
            var container = FindRoofContainer(_tbPopupRoofType);
            container?.BringIntoView();
        }

        private System.Windows.FrameworkElement? FindRoofContainer(RoofType roofType)
        {
            // Try warm roofs container
            var warmContainer = WarmRoofsContainer?.ItemContainerGenerator
                .ContainerFromItem(roofType) as System.Windows.FrameworkElement;
            if (warmContainer != null) return warmContainer;

            // Try cold roofs container
            var coldContainer = ColdRoofsContainer?.ItemContainerGenerator
                .ContainerFromItem(roofType) as System.Windows.FrameworkElement;
            return coldContainer;
        }
    }
}
