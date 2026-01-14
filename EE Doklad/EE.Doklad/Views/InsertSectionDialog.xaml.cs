using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace EE.Doklad.Views
{
    public partial class InsertSectionDialog : Window
    {
        public int SectionNumber { get; private set; }
        public string SectionName { get; private set; } = string.Empty;
        public bool CopyStructure { get; private set; }

        public InsertSectionDialog(int suggestedNumber, int maxNumber)
        {
            InitializeComponent();
            
            SectionNumberTextBox.Text = suggestedNumber.ToString();
            SectionNumberTextBox.Tag = maxNumber; // Запазваме максималния номер
            
            // Фокусираме полето за име
            SectionNameTextBox.Focus();

            // Инициална валидация (OK е изключен докато няма валидни данни)
            ValidateInput();
        }

        private void SectionNumberTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateInput();
        }

        private void SectionNameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateInput();
        }

        private void ValidateInput()
        {
            bool isValid = false;

            if (int.TryParse(SectionNumberTextBox.Text, out int number))
            {
                int maxNumber = SectionNumberTextBox.Tag is int tagValue ? tagValue : 0;
                bool isNumberValid = number >= 1 && number <= maxNumber + 1;
                bool isNameValid = !string.IsNullOrWhiteSpace(SectionNameTextBox.Text);

                isValid = isNumberValid && isNameValid;
            }

            OkButton.IsEnabled = isValid;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(SectionNumberTextBox.Text, out int number))
            {
                SectionNumber = number;
                SectionName = SectionNameTextBox.Text.Trim();
                DialogResult = true;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
