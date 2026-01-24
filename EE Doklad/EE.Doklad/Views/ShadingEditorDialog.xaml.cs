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

            // Режим
            if (_config.EditMode == ShadingEditMode.Custom)
            {
                CustomModeRadio.IsChecked = true;
            }
            else
            {
                SimpleModeRadio.IsChecked = true;
            }

            UpdateModeVisibility();
        }

        private void LoadExistingConfig()
        {
            if (_config.Shadings.Count == 0)
                return;

            // Прост режим: зареди checkbox-ове и полета
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

            // Custom режим: зареди DataGrid
            CustomShadingsDataGrid.ItemsSource = _config.Shadings;
        }

        #region Режим (Simple/Custom)

        private void EditModeRadio_Checked(object sender, RoutedEventArgs e)
        {
            UpdateModeVisibility();
        }

        private void UpdateModeVisibility()
        {
            if (SimpleModeRadio == null || CustomModeRadio == null)
                return;

            bool isSimple = SimpleModeRadio.IsChecked == true;
            SimpleModeBorder.Visibility = isSimple ? Visibility.Visible : Visibility.Collapsed;
            CustomModeBorder.Visibility = isSimple ? Visibility.Collapsed : Visibility.Visible;

            _config.EditMode = isSimple ? ShadingEditMode.Simple : ShadingEditMode.Custom;
        }

        #endregion

        #region Прост режим

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

        #region Custom режим

        private void AddShadingButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddShadingObjectDialog();
            if (dialog.ShowDialog() == true)
            {
                _config.Shadings.Add(dialog.Result);
                CustomShadingsDataGrid.Items.Refresh();
                RecalculateResults();
            }
        }

        private void EditShadingButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomShadingsDataGrid.SelectedItem is ShadingObject selected)
            {
                var dialog = new AddShadingObjectDialog(selected);
                if (dialog.ShowDialog() == true)
                {
                    int index = _config.Shadings.IndexOf(selected);
                    _config.Shadings[index] = dialog.Result;
                    CustomShadingsDataGrid.Items.Refresh();
                    RecalculateResults();
                }
            }
        }

        private void DeleteShadingButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomShadingsDataGrid.SelectedItem is ShadingObject selected)
            {
                _config.Shadings.Remove(selected);
                CustomShadingsDataGrid.Items.Refresh();
                RecalculateResults();
            }
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

    /// <summary>
    /// Малък диалог за добавяне/редакция на ShadingObject (за custom режим)
    /// </summary>
    public class AddShadingObjectDialog : Window
    {
        private ComboBox _typeComboBox;
        private TextBox _depthTextBox;
        private TextBox _distanceTextBox;
        private TextBox _nameTextBox;
        private ShadingObject? _existing;

        public ShadingObject Result { get; private set; } = null!;

        public AddShadingObjectDialog(ShadingObject? existing = null)
        {
            _existing = existing;
            Title = existing == null ? "Добавяне на обект" : "Редакция на обект";
            Width = 400;
            Height = 280;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;

            var grid = new Grid { Margin = new Thickness(20) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Type
            var typeLbl = new TextBlock { Text = "Тип:", Margin = new Thickness(0, 0, 0, 5) };
            _typeComboBox = new ComboBox { Margin = new Thickness(0, 0, 0, 15) };
            _typeComboBox.ItemsSource = Enum.GetValues(typeof(ShadingType));
            _typeComboBox.SelectedIndex = 0;
            Grid.SetRow(typeLbl, 0);
            Grid.SetRow(_typeComboBox, 0);
            typeLbl.Margin = new Thickness(0, 0, 0, 5);
            _typeComboBox.Margin = new Thickness(0, 20, 0, 15);

            // Depth
            var depthLbl = new TextBlock { Text = "D - Дълбочина (m):", Margin = new Thickness(0, 0, 0, 5) };
            _depthTextBox = new TextBox { Margin = new Thickness(0, 0, 0, 15) };
            Grid.SetRow(depthLbl, 1);
            Grid.SetRow(_depthTextBox, 1);
            depthLbl.Margin = new Thickness(0, 0, 0, 5);
            _depthTextBox.Margin = new Thickness(0, 20, 0, 15);

            // Distance
            var distLbl = new TextBlock { Text = "L - Разстояние (m):", Margin = new Thickness(0, 0, 0, 5) };
            _distanceTextBox = new TextBox { Margin = new Thickness(0, 0, 0, 15) };
            Grid.SetRow(distLbl, 2);
            Grid.SetRow(_distanceTextBox, 2);
            distLbl.Margin = new Thickness(0, 0, 0, 5);
            _distanceTextBox.Margin = new Thickness(0, 20, 0, 15);

            // Name
            var nameLbl = new TextBlock { Text = "Име (опционално):", Margin = new Thickness(0, 0, 0, 5) };
            _nameTextBox = new TextBox { Margin = new Thickness(0, 0, 0, 15) };
            Grid.SetRow(nameLbl, 3);
            Grid.SetRow(_nameTextBox, 3);
            nameLbl.Margin = new Thickness(0, 0, 0, 5);
            _nameTextBox.Margin = new Thickness(0, 20, 0, 15);

            // Buttons
            var btnPanel = new StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            var okBtn = new Button { Content = "OK", Width = 80, Margin = new Thickness(0, 0, 10, 0) };
            var cancelBtn = new Button { Content = "Отказ", Width = 80 };
            okBtn.Click += OkButton_Click;
            cancelBtn.Click += (s, e) => { DialogResult = false; Close(); };
            btnPanel.Children.Add(okBtn);
            btnPanel.Children.Add(cancelBtn);
            Grid.SetRow(btnPanel, 5);

            grid.Children.Add(typeLbl);
            grid.Children.Add(_typeComboBox);
            grid.Children.Add(depthLbl);
            grid.Children.Add(_depthTextBox);
            grid.Children.Add(distLbl);
            grid.Children.Add(_distanceTextBox);
            grid.Children.Add(nameLbl);
            grid.Children.Add(_nameTextBox);
            grid.Children.Add(btnPanel);

            Content = grid;

            // Load existing
            if (_existing != null)
            {
                _typeComboBox.SelectedItem = _existing.Type;
                _depthTextBox.Text = _existing.Depth.ToString("F2", CultureInfo.InvariantCulture);
                _distanceTextBox.Text = _existing.Distance.ToString("F2", CultureInfo.InvariantCulture);
                _nameTextBox.Text = _existing.Name;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(_depthTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double d) || d < 0)
            {
                MessageBox.Show("Въведете валидна стойност за D (>= 0)", "Грешка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(_distanceTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double l) || l < 0)
            {
                MessageBox.Show("Въведете валидна стойност за L (>= 0)", "Грешка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Result = new ShadingObject
            {
                Id = _existing?.Id ?? Guid.NewGuid().ToString(),
                Type = (ShadingType)_typeComboBox.SelectedItem,
                Depth = d,
                Distance = l,
                Name = _nameTextBox.Text
            };

            DialogResult = true;
            Close();
        }
    }
}
