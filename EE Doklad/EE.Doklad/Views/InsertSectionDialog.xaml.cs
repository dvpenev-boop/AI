using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace EE.Doklad.Views
{
    public partial class InsertSectionDialog : Window
    {
        public int SectionNumber { get; private set; }
        public string SectionName { get; private set; } = string.Empty;
        public string FullSectionTitle { get; private set; } = string.Empty;

        private readonly int _minNumber;
        private readonly int _maxNumber;

        public InsertSectionDialog(int currentSectionCount)
        {
            InitializeComponent();

            _minNumber = 1;
            _maxNumber = currentSectionCount + 1;

            // Показваме валидния диапазон
            RangeHintTextBlock.Text = $"(валидни стойности: {_minNumber} - {_maxNumber})";

            // Задаваме по подразбиране следващия номер
            SectionNumberTextBox.Text = _maxNumber.ToString();
            SectionNameTextBox.Focus();

            UpdatePreview();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Позволява само цифри
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SectionNumberTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void SectionNameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            var numberText = SectionNumberTextBox.Text.Trim();
            var name = SectionNameTextBox.Text.Trim();

            // Валидация на номера
            bool isValidNumber = int.TryParse(numberText, out int number) && 
                                 number >= _minNumber && 
                                 number <= _maxNumber;

            // Валидация на заглавието
            bool isValidName = !string.IsNullOrWhiteSpace(name);

            if (isValidNumber && isValidName)
            {
                FullSectionTitle = $"{number}. {name}";
                PreviewTextBlock.Text = FullSectionTitle;
                PreviewTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                OkButton.IsEnabled = true;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(numberText))
                {
                    PreviewTextBlock.Text = "(въведете номер и заглавие)";
                }
                else if (!isValidNumber)
                {
                    PreviewTextBlock.Text = $"⚠ Номерът трябва да е между {_minNumber} и {_maxNumber}";
                }
                else if (!isValidName)
                {
                    PreviewTextBlock.Text = $"{numberText}. (въведете заглавие)";
                }
                
                PreviewTextBlock.Foreground = System.Windows.Media.Brushes.Gray;
                OkButton.IsEnabled = false;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            var numberText = SectionNumberTextBox.Text.Trim();
            var name = SectionNameTextBox.Text.Trim();

            // Финална валидация
            if (!int.TryParse(numberText, out int number))
            {
                MessageBox.Show("Моля, въведете валиден номер!", 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (number < _minNumber || number > _maxNumber)
            {
                MessageBox.Show($"Номерът трябва да е между {_minNumber} и {_maxNumber}!", 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Моля, въведете заглавие на секцията!", 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SectionNumber = number;
            SectionName = name;
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
