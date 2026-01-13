using System.Windows;

namespace EE.Doklad.Views
{
    public partial class AddSectionDialog : Window
    {
        public string SectionName { get; private set; } = string.Empty;
        public string FullSectionTitle { get; private set; } = string.Empty;

        public AddSectionDialog(int nextSectionNumber)
        {
            InitializeComponent();
            
            SectionNumberTextBox.Text = nextSectionNumber.ToString();
            SectionNameTextBox.Focus();
            
            UpdatePreview();
        }

        private void SectionNameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            var number = SectionNumberTextBox.Text;
            var name = SectionNameTextBox.Text.Trim();
            
            if (!string.IsNullOrWhiteSpace(name))
            {
                FullSectionTitle = $"{number}. {name}";
                PreviewTextBlock.Text = FullSectionTitle;
                OkButton.IsEnabled = true;
            }
            else
            {
                PreviewTextBlock.Text = $"{number}. (въведете име)";
                OkButton.IsEnabled = false;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            SectionName = SectionNameTextBox.Text.Trim();
            
            if (string.IsNullOrWhiteSpace(SectionName))
            {
                MessageBox.Show("Моля, въведете име на секцията!", 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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
