using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using EE.Doklad.Models;

namespace EE.Doklad.Views
{
    /// <summary>
    /// Interaction logic for ResultsSectionEditor.xaml
    /// </summary>
    public partial class ResultsSectionEditor : UserControl
    {
        public ResultsSectionEditor()
        {
            InitializeComponent();
            Loaded += ResultsSectionEditor_Loaded;
        }

        private void ResultsSectionEditor_Loaded(object sender, RoutedEventArgs e)
        {
            // Зареждаме енергийните носители за всички ComboBox-ове
            LoadEnergyCarrierComboBoxes();
        }

        private void LoadEnergyCarrierComboBoxes()
        {
            // Подготвяме групираните данни веднъж
            var groupedData = CollectionViewSource.GetDefaultView(EnergyCarrierInfo.All);
            groupedData.GroupDescriptions.Clear();
            groupedData.GroupDescriptions.Add(new PropertyGroupDescription("Category"));

            // Намираме всички ComboBox-ове чрез визуалното дърво
            FindAndLoadComboBoxes(this, groupedData);
        }

        private void FindAndLoadComboBoxes(DependencyObject parent, ICollectionView groupedData)
        {
            int childrenCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                
                if (child is ComboBox combo && combo.Name == "EnergyCarrierCombo")
                {
                    // Зареждаме ItemsSource само ако още не е зареден
                    if (combo.ItemsSource == null)
                    {
                        combo.ItemsSource = groupedData;
                    }
                }

                // Рекурсивно обхождаме дървото
                FindAndLoadComboBoxes(child, groupedData);
            }
        }
    }
}
