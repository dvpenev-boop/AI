using System.Windows.Controls;
using EE.Doklad.ViewModels;
using EE.Doklad.Models;

namespace EE.Doklad.Views
{
    /// <summary>
    /// Interaction logic for WindowsSectionView.xaml
    /// </summary>
    public partial class WindowsSectionView : UserControl
    {
        public WindowsSectionView()
        {
            InitializeComponent();
        }

        private void ApplySystemThermalBridge_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is not WindowsSectionViewModel vm) return;
            if (sender is not Button button) return;
            if (button.DataContext is not WindowSystemLossSummaryRow row) return;

            vm.ApplyThermalBridgeToSystemRow(row);
        }

        private void OpenSystemDetails_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is not WindowsSectionViewModel vm) return;
            if (sender is not Button button) return;
            if (button.DataContext is not WindowSystemLossSummaryRow row) return;

            vm.OpenSystemDetails(row);
        }
    }
}
