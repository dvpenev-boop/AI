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
    /// Expandable surface-parameters editor for a single <see cref="WallSurfaceProperties"/>.
    /// DataContext must be set to a <see cref="WallSurfaceProperties"/> instance.
    /// </summary>
    public partial class WallSurfaceParamsEditor : UserControl
    {
        // ------------------------------------------------------------------ //
        //  Construction / data-context
        // ------------------------------------------------------------------ //

        public WallSurfaceParamsEditor()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is WallSurfaceProperties props)
            {
                RefreshOrientationBoxes(props);
            }
        }

        // ------------------------------------------------------------------ //
        //  Uniform-mode input events
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
        //  Typical-values dropdowns
        // ------------------------------------------------------------------ //

        private void TypicalValuesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not WallSurfaceProperties props) return;
            ApplyTypicalSelection(TypicalValuesCombo, props, applyToAll: false);
        }

        private void TypicalValuesComboOrientation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not WallSurfaceProperties props) return;
            ApplyTypicalSelection(TypicalValuesComboOrientation, props, applyToAll: true);
        }

        private static void ApplyTypicalSelection(ComboBox combo, WallSurfaceProperties props, bool applyToAll)
        {
            if (combo.SelectedItem is not ComboBoxItem item) return;
            if (item.IsEnabled == false) return;   // skip placeholder

            var tag = item.Tag as string ?? string.Empty;
            var parts = tag.Split('|');
            if (parts.Length != 2) return;

            if (!double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double alpha)) return;
            if (!double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double epsilon)) return;

            if (applyToAll)
            {
                foreach (WallOrientation o in Enum.GetValues(typeof(WallOrientation)))
                {
                    if (!props.Overrides.ContainsKey(o))
                        props.Overrides[o] = new SurfaceProps();
                    props.Overrides[o].Alpha = alpha;
                    props.Overrides[o].Epsilon = epsilon;
                }
            }
            else
            {
                props.AlphaDefault = alpha;
                props.EpsilonDefault = epsilon;
            }

            // Deselect so it can be re-applied
            combo.SelectedIndex = -1;
        }

        // ------------------------------------------------------------------ //
        //  Per-orientation TextBox events
        // ------------------------------------------------------------------ //

        private void OrientationBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is not WallSurfaceProperties props) return;
            if (sender is not TextBox tb) return;

            var tag = tb.Tag as string ?? string.Empty;
            var parts = tag.Split('|');
            if (parts.Length != 2) return;

            var fieldName = parts[0];   // "Alpha" or "Epsilon"
            var orientStr = parts[1];   // "NE", "E", etc.

            if (!Enum.TryParse<WallOrientation>(orientStr, out var orientation)) return;
            if (!props.Overrides.ContainsKey(orientation))
                props.Overrides[orientation] = new SurfaceProps();

            var surfaceProps = props.Overrides[orientation];

            if (TryParseAlphaEpsilon(tb.Text, out double v))
            {
                tb.BorderBrush = System.Windows.Media.Brushes.Gray;
                if (fieldName == "Alpha")
                    surfaceProps.Alpha = v;
                else
                    surfaceProps.Epsilon = v;
            }
            else
            {
                tb.BorderBrush = System.Windows.Media.Brushes.Red;
                // Revert to stored value
                var storedVal = fieldName == "Alpha" ? surfaceProps.Alpha : surfaceProps.Epsilon;
                tb.Text = storedVal.ToString("0.00", CultureInfo.InvariantCulture);
            }
        }

        private void CopyFirstValue_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not WallSurfaceProperties props) return;

            // Read current values from NE boxes (first orientation column)
            bool alphaOk = TryParseAlphaEpsilon(AlphaNEBox.Text, out double alpha);
            bool epsOk   = TryParseAlphaEpsilon(EpsilonNEBox.Text, out double epsilon);

            foreach (WallOrientation o in Enum.GetValues(typeof(WallOrientation)))
            {
                if (!props.Overrides.ContainsKey(o))
                    props.Overrides[o] = new SurfaceProps();
                if (alphaOk)   props.Overrides[o].Alpha   = alpha;
                if (epsOk)     props.Overrides[o].Epsilon = epsilon;
            }

            RefreshOrientationBoxes(props);
        }

        // ------------------------------------------------------------------ //
        //  Override mode toggle
        // ------------------------------------------------------------------ //

        private void OrientationOverride_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is WallSurfaceProperties props)
            {
                // If switching to per-orientation mode, seed all overrides from the defaults
                if (props.UseOrientationOverride)
                {
                    foreach (WallOrientation o in Enum.GetValues(typeof(WallOrientation)))
                    {
                        if (!props.Overrides.ContainsKey(o))
                            props.Overrides[o] = new SurfaceProps();
                        props.Overrides[o].Alpha   = props.AlphaDefault;
                        props.Overrides[o].Epsilon = props.EpsilonDefault;
                    }
                }
                RefreshOrientationBoxes(props);
            }
        }

        // ------------------------------------------------------------------ //
        //  Help popups (click opens a MessageBox with full help content)
        // ------------------------------------------------------------------ //

        private void AlphaHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Коефициент на слънчево поглъщане (α_sol)\n\n" +
                "Типични стойности:\n" +
                "  Светла повърхност            0.3\n" +
                "  Среден цвят                  0.6\n" +
                "  Тъмна повърхност             0.9\n\n" +
                "Граници: 0 < α ≤ 1\n\n" +
                "Източник:\n" +
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
                "  Мазилка / бетон / тухла      0.90\n" +
                "  Керемиди                     0.90\n" +
                "  Боядисан метал               0.85\n" +
                "  Полиран алуминий             0.05\n\n" +
                "Обичайна стойност за фасади: 0.90\n\n" +
                "Граници: 0 < ε ≤ 1\n\n" +
                "Източници:\n" +
                "  EN ISO 6946\n" +
                "  ISO 52016\n" +
                "  ASHRAE Handbook – Fundamentals",
                "Помощ: Топлинна излъчваемост ε",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ------------------------------------------------------------------ //
        //  Helpers
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Refreshes the per-orientation TextBox values from the model.
        /// </summary>
        private void RefreshOrientationBoxes(WallSurfaceProperties props)
        {
            SetOrientationBox(AlphaNEBox,   props, WallOrientation.NE, "Alpha");
            SetOrientationBox(AlphaEBox,    props, WallOrientation.E,  "Alpha");
            SetOrientationBox(AlphaSEBox,   props, WallOrientation.SE, "Alpha");
            SetOrientationBox(AlphaSBox,    props, WallOrientation.S,  "Alpha");
            SetOrientationBox(AlphaSWBox,   props, WallOrientation.SW, "Alpha");
            SetOrientationBox(AlphaWBox,    props, WallOrientation.W,  "Alpha");
            SetOrientationBox(AlphaNWBox,   props, WallOrientation.NW, "Alpha");

            SetOrientationBox(EpsilonNEBox, props, WallOrientation.NE, "Epsilon");
            SetOrientationBox(EpsilonEBox,  props, WallOrientation.E,  "Epsilon");
            SetOrientationBox(EpsilonSEBox, props, WallOrientation.SE, "Epsilon");
            SetOrientationBox(EpsilonSBox,  props, WallOrientation.S,  "Epsilon");
            SetOrientationBox(EpsilonSWBox, props, WallOrientation.SW, "Epsilon");
            SetOrientationBox(EpsilonWBox,  props, WallOrientation.W,  "Epsilon");
            SetOrientationBox(EpsilonNWBox, props, WallOrientation.NW, "Epsilon");
        }

        private static void SetOrientationBox(TextBox tb, WallSurfaceProperties props,
                                               WallOrientation orientation, string field)
        {
            if (!props.Overrides.TryGetValue(orientation, out var sp))
            {
                sp = new SurfaceProps { Alpha = props.AlphaDefault, Epsilon = props.EpsilonDefault };
                props.Overrides[orientation] = sp;
            }
            var val = field == "Alpha" ? sp.Alpha : sp.Epsilon;
            tb.Text = val.ToString("0.00", CultureInfo.InvariantCulture);
            tb.BorderBrush = System.Windows.Media.Brushes.Gray;
        }

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
