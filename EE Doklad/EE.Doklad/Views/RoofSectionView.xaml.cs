using System.Windows;
using System.Windows.Controls;
using EE.Doklad.ViewModels;
using EE.Doklad.Models;

namespace EE.Doklad.Views
{
    public partial class RoofSectionView : UserControl
    {
        public RoofSectionView()
        {
            InitializeComponent();
        }

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
    }
}
