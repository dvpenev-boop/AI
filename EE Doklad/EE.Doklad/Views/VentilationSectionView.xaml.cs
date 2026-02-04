using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

using EE.Doklad.Models;

namespace EE.Doklad.Views
{
    /// <summary>
    /// Interaction logic for VentilationSectionView.xaml
    /// Секция 12 - Вентилация
    /// </summary>
    public partial class VentilationSectionView : UserControl
    {
        public VentilationSectionView()
        {
            InitializeComponent();
            Loaded += VentilationSectionView_Loaded;
        }

        private void VentilationSectionView_Loaded(object sender, RoutedEventArgs e)
        {
            // Prepare grouped collection view for energy carriers and assign to both comboboxes
            var grouped = CollectionViewSource.GetDefaultView(EnergyCarrierInfo.All);
            grouped.GroupDescriptions.Clear();
            grouped.GroupDescriptions.Add(new PropertyGroupDescription("Category"));

            // Find ComboBoxes named EnergyCarrierCombo and set ItemsSource
            LoadEnergyCarrierComboBoxes(this, grouped);
        }

    private void LoadEnergyCarrierComboBoxes(DependencyObject parent, System.ComponentModel.ICollectionView groupedData)
        {
            int childrenCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);

                if (child is ComboBox combo && combo.Name != null && combo.Name.StartsWith("EnergyCarrierCombo"))
                {
                    if (combo.ItemsSource == null || combo.ItemsSource == (object)EnergyCarrierInfo.All)
                    {
                        combo.ItemsSource = groupedData;
                    }
                }

                // recurse
                LoadEnergyCarrierComboBoxes(child, groupedData);
            }
        }
    }
}
