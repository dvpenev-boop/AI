using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EE.Doklad.Models;

namespace EE.Doklad.Views.Controls
{
    /// <summary>
    /// Surface-parameters editor for a roof type (α_sol / ε).
    /// DataContext must be set to a <see cref="WallSurfaceProperties"/> instance.
    /// No orientation overrides – only default (uniform) values are used.
    /// </summary>
    public partial class RoofSurfaceParamsEditor : UserControl
    {
        public RoofSurfaceParamsEditor()
        {
            InitializeComponent();
        }

        // ------------------------------------------------------------------ //
        //  Alpha input
        // ------------------------------------------------------------------ //

        private void AlphaDefaultBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is not WallSurfaceProperties props) return;
            if (TryParseAlphaEpsilon(AlphaDefaultBox.Text, out double v))
            {
                props.AlphaDefault = v;
                AlphaValidationMsg.Text = string.Empty;
                AlphaDefaultBox.BorderBrush = System.Windows.Media.Brushes.Gray;
            }
            else
            {
                AlphaValidationMsg.Text = "Стойност 0 < α ≤ 1";
                AlphaDefaultBox.BorderBrush = System.Windows.Media.Brushes.Red;
                AlphaDefaultBox.Text = props.AlphaDefault.ToString("0.00", CultureInfo.InvariantCulture);
            }
        }

        // ------------------------------------------------------------------ //
        //  Epsilon input
        // ------------------------------------------------------------------ //

        private void EpsilonDefaultBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is not WallSurfaceProperties props) return;
            if (TryParseAlphaEpsilon(EpsilonDefaultBox.Text, out double v))
            {
                props.EpsilonDefault = v;
                EpsilonValidationMsg.Text = string.Empty;
                EpsilonDefaultBox.BorderBrush = System.Windows.Media.Brushes.Gray;
            }
            else
            {
                EpsilonValidationMsg.Text = "Стойност 0 < ε ≤ 1";
                EpsilonDefaultBox.BorderBrush = System.Windows.Media.Brushes.Red;
                EpsilonDefaultBox.Text = props.EpsilonDefault.ToString("0.00", CultureInfo.InvariantCulture);
            }
        }

        // ------------------------------------------------------------------ //
        //  Typical values dropdown
        // ------------------------------------------------------------------ //

        private void TypicalValuesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not WallSurfaceProperties props) return;
            if (TypicalValuesCombo.SelectedItem is not ComboBoxItem item) return;
            if (!item.IsEnabled) return;   // skip placeholder

            var tag = item.Tag as string ?? string.Empty;
            var parts = tag.Split('|');
            if (parts.Length != 2) return;

            if (!double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double alpha)) return;
            if (!double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double epsilon)) return;

            props.AlphaDefault = alpha;
            props.EpsilonDefault = epsilon;

            // Clear validation messages after a preset is applied
            AlphaValidationMsg.Text = string.Empty;
            EpsilonValidationMsg.Text = string.Empty;
            AlphaDefaultBox.BorderBrush = System.Windows.Media.Brushes.Gray;
            EpsilonDefaultBox.BorderBrush = System.Windows.Media.Brushes.Gray;

            // Deselect so the same preset can be re-applied
            TypicalValuesCombo.SelectedIndex = -1;
        }

        // ------------------------------------------------------------------ //
        //  Help popups
        // ------------------------------------------------------------------ //

        private void AlphaHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Коефициент на слънчево поглъщане (α_sol)\n\n" +
                "Типични стойности:\n" +
                "  Светла повърхност                0.3\n" +
                "  Среден цвят                      0.6\n" +
                "  Тъмна повърхност                 0.9\n\n" +
                "Граници: 0 < α ≤ 1\n\n" +
                "Типични покривни материали:\n" +
                "  Керамични/бетонни керемиди       α=0.60\n" +
                "  Битумни керемиди/мембрана        α=0.85\n" +
                "  Полимерна мембрана (PVC/TPO)     α=0.50\n" +
                "  Боядисан метален покрив          α=0.60\n" +
                "  Поцинкован стоманен покрив       α=0.65\n" +
                "  Полиран алуминиев покрив         α=0.30\n\n" +
                "Източници:\n" +
                "  Наредба № Е-РД-04-2\n" +
                "  Приложение 1 – Таблица 1\n" +
                "  (Методология за енергийна ефективност – България)",
                "Помощ: Коефициент α_sol",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EpsilonHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Топлинна излъчваемост (ε)\n\n" +
                "Типични стойности:\n" +
                "  Мазилка / бетон / тухла          0.90\n" +
                "  Керемиди (керамични/бетонни)     0.90\n" +
                "  Боядисан метал                   0.85\n" +
                "  Поцинкована стомана              0.30\n" +
                "  Полиран алуминий                 0.05\n\n" +
                "Обичайна стойност за покриви: 0.90\n\n" +
                "Граници: 0 < ε ≤ 1\n\n" +
                "Източници:\n" +
                "  EN ISO 6946\n" +
                "  ISO 52016-1\n" +
                "  ASHRAE Handbook – Fundamentals\n" +
                "  US DOE Cool Roof Database",
                "Помощ: Топлинна излъчваемост ε",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ------------------------------------------------------------------ //
        //  Helpers
        // ------------------------------------------------------------------ //

        /// <summary>Validates that a string represents a double in (0, 1].</summary>
        private static bool TryParseAlphaEpsilon(string? text, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;

            // Accept comma or dot as decimal separator
            var normalized = text.Trim().Replace(',', '.');
            if (!double.TryParse(normalized, NumberStyles.Any,
                                  CultureInfo.InvariantCulture, out value)) return false;

            return value > 0 && value <= 1.0;
        }

        /// <summary>Blocks non-numeric characters in TextBox input (allows digits, . and ,).</summary>
        private void NumericInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"[\d\.,]");
        }
    }
}
