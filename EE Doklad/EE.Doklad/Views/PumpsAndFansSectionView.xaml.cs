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
    }
}
