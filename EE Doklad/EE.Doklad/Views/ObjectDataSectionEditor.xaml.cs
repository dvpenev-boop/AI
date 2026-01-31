using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using EE.Doklad.Models;

namespace EE.Doklad.Views
{
    /// <summary>
    /// Interaction logic for ObjectDataSectionEditor.xaml
    /// </summary>
    public partial class ObjectDataSectionEditor : UserControl
    {
        public ObjectDataSectionEditor()
        {
            InitializeComponent();
            Loaded += ObjectDataSectionEditor_Loaded;
        }

        private void ObjectDataSectionEditor_Loaded(object sender, RoutedEventArgs e)
        {
            // Намираме ComboBox за типа сграда и зареждаме ItemsSource с групирани данни
            var buildingTypeCombo = FindName("BuildingTypeCombo") as ComboBox;
            if (buildingTypeCombo == null)
            {
                // Ако няма име, търсим по Grid.Column
                buildingTypeCombo = FindBuildingTypeComboBox(this);
            }

            if (buildingTypeCombo != null)
            {
                var groupedData = CollectionViewSource.GetDefaultView(BuildingTypeInfo.All);
                groupedData.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
                buildingTypeCombo.ItemsSource = groupedData;
            }
        }

        private ComboBox? FindBuildingTypeComboBox(DependencyObject parent)
        {
            int childrenCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is ComboBox combo && combo.DisplayMemberPath == "DisplayName")
                {
                    return combo;
                }

                var result = FindBuildingTypeComboBox(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private static readonly Regex _digitsRegex = new Regex("^[0-9]+$", RegexOptions.Compiled);

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only digits
            e.Handled = !_digitsRegex.IsMatch(e.Text);
        }

        private void NumberOnly_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                var text = e.DataObject.GetData(DataFormats.Text) as string ?? string.Empty;
                if (!_digitsRegex.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
