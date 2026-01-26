using System.Windows;
using EE.Doklad.Models;
using EE.Doklad.Services;
using EE.Doklad.ViewModels;

namespace EE.Doklad.Views
{
    public partial class MaterialsEditWindow : Window
    {
        private readonly MaterialsEditViewModel _vm;

        public MaterialsEditWindow(MaterialsService service, BuildingMaterialUser material)
        {
            InitializeComponent();
            _vm = new MaterialsEditViewModel(service, material);
            DataContext = _vm;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (!_vm.Validate(out var error))
            {
                MessageBox.Show(error ?? "Грешка", "Валидиране", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _vm.Save();
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
