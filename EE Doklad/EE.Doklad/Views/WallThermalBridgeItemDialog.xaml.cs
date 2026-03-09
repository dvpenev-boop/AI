using System;
using System.Globalization;
using System.Windows;
using EE.Doklad.Models;

namespace EE.Doklad.Views
{
    /// <summary>
    /// Диалог за добавяне / редактиране на един топлинен мост.
    /// </summary>
    public partial class WallThermalBridgeItemDialog : Window
    {
        /// <summary>Резултатният обект след успешно запазване.</summary>
        public WallThermalBridgeItem? Result { get; private set; }

        private readonly WallThermalBridgeItem? _editing;

        public WallThermalBridgeItemDialog(WallThermalBridgeItem? existing = null, bool showFacades = true)
        {
            InitializeComponent();
            _editing = existing;

            if (!showFacades)
            {
                FacadesLabel.Visibility  = Visibility.Collapsed;
                FacadesPanel.Visibility  = Visibility.Collapsed;
            }

            if (existing != null)
            {
                TypeBox.Text        = existing.Type;
                LengthBox.Text      = existing.Length.ToString("0.000", CultureInfo.InvariantCulture);
                PsiBox.Text         = existing.Psi.ToString("0.000", CultureInfo.InvariantCulture);
                ChiBox.Text         = existing.Chi.ToString("0.000", CultureInfo.InvariantCulture);
                CbNorth.IsChecked     = existing.FacadeNorth;
                CbNorthEast.IsChecked = existing.FacadeNorthEast;
                CbEast.IsChecked      = existing.FacadeEast;
                CbSouthEast.IsChecked = existing.FacadeSouthEast;
                CbSouth.IsChecked     = existing.FacadeSouth;
                CbSouthWest.IsChecked = existing.FacadeSouthWest;
                CbWest.IsChecked      = existing.FacadeWest;
                CbNorthWest.IsChecked = existing.FacadeNorthWest;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // L и Ψ са задължителни; χ е незадължителен (default 0)
            if (!TryParseDouble(LengthBox.Text, out double len))
            {
                MessageBox.Show("Моля въведете валидно число за L (дължина).",
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Warning);
                LengthBox.Focus();
                return;
            }
            if (!TryParseDouble(PsiBox.Text, out double psi))
            {
                MessageBox.Show("Моля въведете валидно число за Ψ.",
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Warning);
                PsiBox.Focus();
                return;
            }
            // χ е опционален – празно поле = 0
            if (!TryParseDouble(ChiBox.Text, out double chi))
                chi = 0.0;

            var item = _editing ?? new WallThermalBridgeItem
            {
                Id = Guid.NewGuid().ToString()
            };

            item.Type          = string.IsNullOrWhiteSpace(TypeBox.Text) ? "Топлинен мост" : TypeBox.Text.Trim();
            item.Length        = len;
            item.Psi           = psi;
            item.Chi           = chi;
            item.FacadeNorth     = CbNorth.IsChecked == true;
            item.FacadeNorthEast = CbNorthEast.IsChecked == true;
            item.FacadeEast      = CbEast.IsChecked == true;
            item.FacadeSouthEast = CbSouthEast.IsChecked == true;
            item.FacadeSouth     = CbSouth.IsChecked == true;
            item.FacadeSouthWest = CbSouthWest.IsChecked == true;
            item.FacadeWest      = CbWest.IsChecked == true;
            item.FacadeNorthWest = CbNorthWest.IsChecked == true;

            Result = item;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Разпознава double независимо от текущата culture (приема и '.' и ',' като десетичен разделител).
        /// Връща false само ако низът е празен или не съдържа валидно число.
        /// </summary>
        private static bool TryParseDouble(string text, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            // Нормализираме: заменяме запетая с точка и премахваме интервали
            string normalized = text.Trim().Replace(" ", "").Replace(',', '.');

            // Опит с InvariantCulture (разделител .)
            if (double.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                return true;

            // Запасен опит с текущата culture на системата
            if (double.TryParse(text.Trim(), NumberStyles.Any, CultureInfo.CurrentCulture, out value))
                return true;

            return false;
        }
    }
}
