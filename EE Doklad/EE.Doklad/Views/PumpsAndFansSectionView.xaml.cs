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

        private void AddHeatingRow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                viewModel.Data.HeatingRows.Add(new PumpFanHeatingRow());
            }
        }

        private void RemoveHeatingRow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                if (viewModel.Data.HeatingRows.Count > 0)
                {
                    viewModel.Data.HeatingRows.RemoveAt(viewModel.Data.HeatingRows.Count - 1);
                }
            }
        }

        private void AddHeatingPump_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                viewModel.Data.HeatingRows.Add(new PumpFanHeatingRow { DeviceType = "Помпи отопление" });
            }
        }

        private void RemoveHeatingPump_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                var list = viewModel.Data.HeatingRows;
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var item = list[i];
                    if (!(item.DeviceType ?? string.Empty).Contains("Вентил"))
                    {
                        list.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void AddHeatingFan_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                viewModel.Data.HeatingRows.Add(new PumpFanHeatingRow { DeviceType = "Вентилатори" });
            }
        }

        private void RemoveHeatingFan_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                var list = viewModel.Data.HeatingRows;
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var item = list[i];
                    if ((item.DeviceType ?? string.Empty).Contains("Вентил"))
                    {
                        list.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void AddCoolingRow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                viewModel.Data.CoolingRows.Add(new PumpFanCoolingRow());
            }
        }

        private void RemoveCoolingRow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                if (viewModel.Data.CoolingRows.Count > 0)
                {
                    viewModel.Data.CoolingRows.RemoveAt(viewModel.Data.CoolingRows.Count - 1);
                }
            }
        }

        private void AddCoolingPump_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                viewModel.Data.CoolingRows.Add(new PumpFanCoolingRow { DeviceType = "Помпи охлаждане" });
            }
        }

        private void RemoveCoolingPump_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                var list = viewModel.Data.CoolingRows;
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var item = list[i];
                    if (!(item.DeviceType ?? string.Empty).Contains("Вентил"))
                    {
                        list.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void AddCoolingFan_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                viewModel.Data.CoolingRows.Add(new PumpFanCoolingRow { DeviceType = "Вентилатори (вентилация)" });
            }
        }

        private void RemoveCoolingFan_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PumpsAndFansSectionViewModel viewModel)
            {
                var list = viewModel.Data.CoolingRows;
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var item = list[i];
                    if ((item.DeviceType ?? string.Empty).Contains("Вентил"))
                    {
                        list.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
}
