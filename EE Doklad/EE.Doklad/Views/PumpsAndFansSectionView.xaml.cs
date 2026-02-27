using System.Windows.Controls;
using EE.Doklad.Models;
using EE.Doklad.ViewModels;

namespace EE.Doklad.Views
{
    /// <summary>
    /// Interaction logic for PumpsAndFansSectionView.xaml
    /// </summary>
    public partial class PumpsAndFansSectionView : UserControl
    {
        public PumpsAndFansSectionView()
        {
            InitializeComponent();
        }

        private void AddHeatingPump_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                viewModel.Data.HeatingPumpRows.Add(new PumpFanHeatingRow { DeviceType = "Помпи отопление" });
            }
        }

        private void RemoveHeatingPump_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                var list = viewModel.Data.HeatingPumpRows;
                if (list.Count > 0)
                    list.RemoveAt(list.Count - 1);
            }
        }

        private void AddHeatingFan_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                viewModel.Data.HeatingFanRows.Add(new PumpFanHeatingRow { DeviceType = "Вентилатори" });
            }
        }

        private void RemoveHeatingFan_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                var list = viewModel.Data.HeatingFanRows;
                if (list.Count > 0)
                    list.RemoveAt(list.Count - 1);
            }
        }

        private void AddCoolingPump_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                viewModel.Data.CoolingPumpRows.Add(new PumpFanCoolingRow { DeviceType = "Помпи охлаждане" });
            }
        }

        private void RemoveCoolingPump_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                var list = viewModel.Data.CoolingPumpRows;
                if (list.Count > 0)
                    list.RemoveAt(list.Count - 1);
            }
        }

        private void AddCoolingFan_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                viewModel.Data.CoolingFanRows.Add(new PumpFanCoolingRow { DeviceType = "Вентилатори (вентилация)" });
            }
        }

        private void RemoveCoolingFan_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                var list = viewModel.Data.CoolingFanRows;
                if (list.Count > 0)
                    list.RemoveAt(list.Count - 1);
            }
        }
    }
}
