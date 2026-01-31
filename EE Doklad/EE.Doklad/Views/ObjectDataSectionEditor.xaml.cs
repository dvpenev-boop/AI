using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
