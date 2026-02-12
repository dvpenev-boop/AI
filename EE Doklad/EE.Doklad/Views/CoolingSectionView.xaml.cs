using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using EE.Doklad.Models;

namespace EE.Doklad.Views
{
    /// <summary>
    /// Interaction logic for CoolingSectionView.xaml
    /// </summary>
    public partial class CoolingSectionView : UserControl
    {
        public CoolingSectionView()
        {
            InitializeComponent();
            Loaded += CoolingSectionView_Loaded;
        }

        private void CoolingSectionView_Loaded(object sender, RoutedEventArgs e)
        {
            // Prepare grouped collection view for energy carriers and assign to comboboxes used in this view.
            var grouped = CollectionViewSource.GetDefaultView(EnergyCarrierInfo.All);
            grouped.GroupDescriptions.Clear();
            grouped.GroupDescriptions.Add(new PropertyGroupDescription("Category"));

            // Assign grouped view to any ComboBox that is still using the raw All list
            LoadEnergyCarrierComboBoxes(this, grouped);
        }

        private void LoadEnergyCarrierComboBoxes(DependencyObject parent, System.ComponentModel.ICollectionView groupedData)
        {
            int childrenCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);

                if (child is ComboBox combo)
                {
                    // If the ComboBox is using the raw static list, replace with the grouped view so GroupStyle headers render
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
