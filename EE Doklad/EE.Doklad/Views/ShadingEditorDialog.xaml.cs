using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;
using EE.Doklad.Services;
using ModelOrientation = EE.Doklad.Models.Orientation;

namespace EE.Doklad.Views
{
    /// <summary>
    /// Модален диалог за редакция на засенчване
    /// </summary>
    public partial class ShadingEditorDialog : Window
    {
        private readonly double _wk; // Ширина на прозореца (m)
        private readonly double _hk; // Височина на прозореца (m)
        private readonly ModelOrientation _orientation;
        private ShadingConfig _config;
        private List<MonthlyShadingResult> _currentResults = new();

        public ShadingConfig Result => _config;

        public ShadingEditorDialog(double wk, double hk, ModelOrientation orientation, ShadingConfig? existingConfig = null)
        {
            InitializeComponent();

            _wk = wk;
            _hk = hk;
            _orientation = orientation;
            _config = existingConfig ?? new ShadingConfig();

            InitializeUI();
            LoadExistingConfig();
            RecalculateResults();
        }

        private void InitializeUI()
        {
            // Header info
            WindowInfoTextBlock.Text = $"Прозорец: W={_wk:F2} m, H={_hk:F2} m, Фасада={GetOrientationDescription(_orientation)}";
        }

        private void LoadExistingConfig()
        {
            if (_config.Shadings.Count == 0)
                return;

            // Зареди checkbox-ове и полета
            var overhang = _config.Shadings.FirstOrDefault(s => s.Type == ShadingType.Overhang);
            if (overhang != null)
            {
                OverhangCheckBox.IsChecked = true;
                OverhangDepthTextBox.Text = overhang.Depth.ToString("F2", CultureInfo.InvariantCulture);
                OverhangDistanceTextBox.Text = overhang.Distance.ToString("F2", CultureInfo.InvariantCulture);
            }

            var leftFin = _config.Shadings.FirstOrDefault(s => s.Type == ShadingType.LeftFin);
            if (leftFin != null)
            {
                LeftFinCheckBox.IsChecked = true;
                LeftFinDepthTextBox.Text = leftFin.Depth.ToString("F2", CultureInfo.InvariantCulture);
                LeftFinDistanceTextBox.Text = leftFin.Distance.ToString("F2", CultureInfo.InvariantCulture);
            }

            var rightFin = _config.Shadings.FirstOrDefault(s => s.Type == ShadingType.RightFin);
            if (rightFin != null)
            {
                RightFinCheckBox.IsChecked = true;
                RightFinDepthTextBox.Text = rightFin.Depth.ToString("F2", CultureInfo.InvariantCulture);
                RightFinDistanceTextBox.Text = rightFin.Distance.ToString("F2", CultureInfo.InvariantCulture);
            }
        }

        #region Управление на елементи

        private void SimpleModeCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            OverhangPanel.Visibility = OverhangCheckBox.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            LeftFinPanel.Visibility = LeftFinCheckBox.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            RightFinPanel.Visibility = RightFinCheckBox.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;

            RecalculateFromSimpleMode();
        }

        private void SimpleMode_ValueChanged(object sender, TextChangedEventArgs e)
        {
            RecalculateFromSimpleMode();
        }

        private void RecalculateFromSimpleMode()
        {
            if (_config == null)
                return;

            // Изчисти текущите обекти
            _config.Shadings.Clear();

            // Overhang
            if (OverhangCheckBox.IsChecked == true)
            {
                if (TryParseDouble(OverhangDepthTextBox.Text, out double d) &&
                    TryParseDouble(OverhangDistanceTextBox.Text, out double l))
                {
                    _config.Shadings.Add(new ShadingObject
                    {
                        Type = ShadingType.Overhang,
                        Depth = d,
                        Distance = l,
                        Name = "Навес"
                    });
                }
            }

            // Left Fin
            if (LeftFinCheckBox.IsChecked == true)
            {
                if (TryParseDouble(LeftFinDepthTextBox.Text, out double d) &&
                    TryParseDouble(LeftFinDistanceTextBox.Text, out double l))
                {
                    _config.Shadings.Add(new ShadingObject
                    {
                        Type = ShadingType.LeftFin,
                        Depth = d,
                        Distance = l,
                        Name = "Ляво ребро"
                    });
                }
            }

            // Right Fin
            if (RightFinCheckBox.IsChecked == true)
            {
                if (TryParseDouble(RightFinDepthTextBox.Text, out double d) &&
                    TryParseDouble(RightFinDistanceTextBox.Text, out double l))
                {
                    _config.Shadings.Add(new ShadingObject
                    {
                        Type = ShadingType.RightFin,
                        Depth = d,
                        Distance = l,
                        Name = "Дясно ребро"
                    });
                }
            }

            RecalculateResults();
        }

        private void SetbackPresetButton_Click(object sender, RoutedEventArgs e)
        {
            // Preset: Отстъп (комбинация)
            // Примерни стойности: D=0.3m, L=0.2m за всички
            OverhangCheckBox.IsChecked = true;
            OverhangDepthTextBox.Text = "0.30";
            OverhangDistanceTextBox.Text = "0.20";

            LeftFinCheckBox.IsChecked = true;
            LeftFinDepthTextBox.Text = "0.30";
            LeftFinDistanceTextBox.Text = "0.20";

            RightFinCheckBox.IsChecked = true;
            RightFinDepthTextBox.Text = "0.30";
            RightFinDistanceTextBox.Text = "0.20";

            RecalculateFromSimpleMode();
        }

        #endregion

        #region Изчисления и резултати

        private void RecalculateResults()
        {
            if (_config == null)
                return;

            // Изчисли детайлни месечни резултати
            _currentResults = ShadingCalculator.CalculateDetailedMonthly(
                _wk, _hk, _orientation, _config.Shadings, _config.Latitude, _config.NorthHemisphere);

            // Актуализирай FshDirMonthly в конфига
            _config.FshDirMonthly = _currentResults.Select(r => r.FshDir).ToArray();

            // Покажи в таблицата
            ResultsDataGrid.ItemsSource = _currentResults;

            // Обнови сумарни показатели
            if (_currentResults.Count > 0)
            {
                double min = _currentResults.Min(r => r.FshDir);
                double avg = _currentResults.Average(r => r.FshDir);
                double max = _currentResults.Max(r => r.FshDir);

                MinFshTextBlock.Text = $"Min: {min:F3}";
                AvgFshTextBlock.Text = $"Avg: {avg:F3}";
                MaxFshTextBlock.Text = $"Max: {max:F3}";
            }
        }

        #endregion

        #region Бутони

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            // Запази конфигурацията
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #endregion

        #region Helpers

        private bool TryParseDouble(string text, out double value)
        {
            return double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value) && value >= 0;
        }

        private string GetOrientationDescription(ModelOrientation orientation)
        {
            return orientation switch
            {
                ModelOrientation.East => "И",
                ModelOrientation.NorthEast => "СИ",
                ModelOrientation.North => "С",
                ModelOrientation.NorthWest => "СЗ",
                ModelOrientation.West => "З",
                ModelOrientation.SouthWest => "ЮЗ",
                ModelOrientation.South => "Ю",
                ModelOrientation.SouthEast => "ЮИ",
                _ => "—"
            };
        }

        #endregion
    }
}
