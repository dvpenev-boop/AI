using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.Views
{
    public partial class AddWindowFullDialog : Window
    {
        private WindowBatch _batch;
        private List<ShadingOption> _allShadingOptions = new();
        private Dictionary<string, List<ShadingOption>> _shadingByCategory = new();
        private List<ObstacleProfile> _obstacleProfiles = new();
        public WindowBatch Result => _batch;

        public AddWindowFullDialog(WindowBatch? existingBatch = null)
        {
            InitializeComponent();
            _batch = existingBatch ?? new WindowBatch();
            LoadShadingOptions();
            LoadObstacleProfiles();
            InitializeAllSections();
            UpdatePreview();
        }

        private void LoadShadingOptions()
        {
            _allShadingOptions = WindowCalculator.GetShadingOptions();
            _shadingByCategory = WindowCalculator.GetShadingOptionsByCategory();
        }

        private void LoadObstacleProfiles()
        {
            _obstacleProfiles = WindowCalculator.GetObstacleProfiles();
        }

        private void InitializeAllSections()
        {
            // Kind
            KindComboBox.ItemsSource = Enum.GetValues(typeof(WindowKind)).Cast<WindowKind>().Select(k => new { Value = k, Label = GetEnumDescription(k) }).ToList();
            KindComboBox.DisplayMemberPath = "Label";
            KindComboBox.SelectedValuePath = "Value";
            KindComboBox.SelectedValue = _batch.Kind;
            KindComboBox.SelectionChanged += AnyInputChanged;

            // Orientation
            OrientationComboBox.ItemsSource = Enum.GetValues(typeof(EE.Doklad.Models.Orientation)).Cast<EE.Doklad.Models.Orientation>().Select(o => new { Value = o, Label = GetEnumDescription(o) }).ToList();
            OrientationComboBox.DisplayMemberPath = "Label";
            OrientationComboBox.SelectedValuePath = "Value";
            OrientationComboBox.SelectedValue = _batch.Orientation;
            OrientationComboBox.SelectionChanged += AnyInputChanged;

            // Count
            CountTextBox.Text = _batch.Count.ToString();
            CountTextBox.TextChanged += AnyInputChanged;

            // Geometry
            WidthTextBox.TextChanged += AnyInputChanged;
            HeightTextBox.TextChanged += AnyInputChanged;

            // U/g/OpticalType
            UValueTextBox.Text = _batch.UValue > 0 ? _batch.UValue.ToString("F2") : "1.40";
            UValueTextBox.TextChanged += AnyInputChanged;
            GNTextBox.Text = _batch.GN > 0 ? _batch.GN.ToString("F3") : "0.750";
            GNTextBox.TextChanged += AnyInputChanged;
            OpticalTypeComboBox.ItemsSource = Enum.GetValues(typeof(OpticalType));
            OpticalTypeComboBox.SelectedItem = _batch.OpticalType;
            OpticalTypeComboBox.SelectionChanged += AnyInputChanged;

            // FrameFraction
            FrameFractionTextBox.Text = (_batch.FrameFraction * 100).ToString("F1");
            FrameFractionTextBox.TextChanged += AnyInputChanged;

            // Shading
            ShadingNoneRadio.Checked += AnyInputChanged;
            ShadingInternalRadio.Checked += AnyInputChanged;
            ShadingExternalRadio.Checked += AnyInputChanged;
            ShadingCategoryComboBox.ItemsSource = _shadingByCategory.Keys.ToList();
            if (ShadingCategoryComboBox.Items.Count > 0)
            {
                ShadingCategoryComboBox.SelectedIndex = 0;
                var first = ShadingCategoryComboBox.SelectedItem as string;
                if (first != null && _shadingByCategory.ContainsKey(first))
                    ShadingOptionsDataGrid.ItemsSource = _shadingByCategory[first];
            }
            ShadingCategoryComboBox.SelectionChanged += ShadingCategoryComboBox_SelectionChanged;
            ShadingOptionsDataGrid.SelectionChanged += AnyInputChanged;

            // Obstacle
            ObstacleProfileComboBox.ItemsSource = _obstacleProfiles;
            ObstacleProfileComboBox.DisplayMemberPath = "Name";
            ObstacleProfileComboBox.SelectedIndex = 0;
            ObstacleProfileComboBox.SelectionChanged += AnyInputChanged;

            // Buttons
            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += (s, e) => { DialogResult = false; Close(); };
            ResetButton.Click += (s, e) => { InitializeAllSections(); UpdatePreview(); };
        }

        private void AnyInputChanged(object? sender, EventArgs e)
        {
            UpdatePreview();
            ValidateAll();
        }

        private void ShadingCategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ShadingCategoryComboBox.SelectedItem is string cat && _shadingByCategory.ContainsKey(cat))
                ShadingOptionsDataGrid.ItemsSource = _shadingByCategory[cat];
        }

        private void UpdatePreview()
        {
            // Изчисления за preview (може да се изнесе във ViewModel/helper)
            double width = TryParseDouble(WidthTextBox.Text, out var w) ? w / 100.0 : 0;
            double height = TryParseDouble(HeightTextBox.Text, out var h) ? h / 100.0 : 0;
            double areaGross = width * height;
            CalculatedAreaTextBlock.Text = areaGross > 0 ? $"Площ: {areaGross:F3} m² (от {w:F0}×{h:F0} см)" : "Площ: —";
            double ffr = TryParseDouble(FrameFractionTextBox.Text, out var f) ? f / 100.0 : 0;
            double areaGlass = areaGross * (1 - ffr);
            double u = TryParseDouble(UValueTextBox.Text, out var uval) ? uval : 0;
            double gn = TryParseDouble(GNTextBox.Text, out var gnval) ? gnval : 0;
            var opticalType = OpticalTypeComboBox.SelectedItem is OpticalType ot ? ot : OpticalType.Clear;
            double g_base = opticalType == OpticalType.Clear ? 0.90 * gn : 0.75 * gn + 0.25 * gn;
            double shading = 1.0;
            if (ShadingInternalRadio.IsChecked == true && ShadingOptionsDataGrid.SelectedItem is ShadingOption so1) shading = so1.FShadeInt;
            if (ShadingExternalRadio.IsChecked == true && ShadingOptionsDataGrid.SelectedItem is ShadingOption so2) shading = so2.FShadeExt;
            double g_eff = g_base * shading;
            int count = TryParseInt(CountTextBox.Text, out var c) ? c : 1;
            double ua = u * areaGross;
            double totalGross = count * areaGross;
            double totalGlass = count * areaGlass;
            double totalUA = count * ua;
            double totalGA = count * g_eff * areaGlass;
            // Set preview fields
            PreviewAreaGross.Text = areaGross.ToString("F3");
            PreviewAreaGlass.Text = areaGlass.ToString("F3");
            PreviewGBase.Text = g_base.ToString("F3");
            PreviewShadingReduction.Text = shading.ToString("F3");
            PreviewGEff.Text = g_eff.ToString("F3");
            PreviewUA.Text = ua.ToString("F3");
            PreviewTotalGrossArea.Text = totalGross.ToString("F3");
            PreviewTotalGlassArea.Text = totalGlass.ToString("F3");
            PreviewTotalUA.Text = totalUA.ToString("F3");
            PreviewTotalGA.Text = totalGA.ToString("F3");
        }

        private void ValidateAll()
        {
            // Минимални проверки и inline грешки
            bool valid = true;
            CountError.Text = (!TryParseInt(CountTextBox.Text, out var c) || c < 1) ? "Броят трябва да е >= 1" : "";
            valid &= string.IsNullOrEmpty(CountError.Text);
            GeometryError.Text = (!TryParseDouble(WidthTextBox.Text, out var w) || w <= 0 || !TryParseDouble(HeightTextBox.Text, out var h) || h <= 0) ? "Ширина и височина > 0" : "";
            valid &= string.IsNullOrEmpty(GeometryError.Text);
            UError.Text = (!TryParseDouble(UValueTextBox.Text, out var u) || u < 0.1 || u > 10) ? "U трябва да е 0.1..10" : "";
            valid &= string.IsNullOrEmpty(UError.Text);
            GNError.Text = (!TryParseDouble(GNTextBox.Text, out var gn) || gn < 0 || gn > 1) ? "g_n трябва да е 0..1" : "";
            valid &= string.IsNullOrEmpty(GNError.Text);
            FrameFractionError.Text = (!TryParseDouble(FrameFractionTextBox.Text, out var ffr) || ffr < 0 || ffr >= 50) ? "F_fr 0..50%" : "";
            valid &= string.IsNullOrEmpty(FrameFractionError.Text);
            OpticalTypeError.Text = OpticalTypeComboBox.SelectedItem == null ? "Изберете тип" : "";
            valid &= string.IsNullOrEmpty(OpticalTypeError.Text);
            SaveButton.IsEnabled = valid;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Основни данни
            _batch.Kind = (WindowKind)KindComboBox.SelectedValue;
            _batch.Orientation = (EE.Doklad.Models.Orientation)OrientationComboBox.SelectedValue;
            _batch.Count = TryParseInt(CountTextBox.Text, out var count) ? count : 1;

            // 2. Геометрия
            TryParseDouble(WidthTextBox.Text, out double widthCm);
            TryParseDouble(HeightTextBox.Text, out double heightCm);
            _batch.Width = widthCm / 100.0;
            _batch.Height = heightCm / 100.0;
            _batch.AreaGross = _batch.Width * _batch.Height;

            // 3. Топлотехнически и оптични данни
            TryParseDouble(UValueTextBox.Text, out double u);
            TryParseDouble(GNTextBox.Text, out double g);
            _batch.UValue = u;
            _batch.GN = g;
            _batch.OpticalType = OpticalTypeComboBox.SelectedItem is OpticalType ot ? ot : OpticalType.Clear;

            // 4. Рамка
            TryParseDouble(FrameFractionTextBox.Text, out double ffr);
            _batch.FrameFraction = ffr / 100.0;

            // 5. Слънцезащита
            if (ShadingNoneRadio.IsChecked == true)
            {
                _batch.ShadingTypeId = null;
                _batch.ShadingReductionFactor = 1.0;
            }
            else if (ShadingOptionsDataGrid.SelectedItem is ShadingOption option)
            {
                _batch.ShadingTypeId = option.Id;
                _batch.ShadingReductionFactor = ShadingInternalRadio.IsChecked == true
                    ? option.FShadeInt
                    : option.FShadeExt;
            }

            // 6. Препятствия
            if (ObstacleProfileComboBox.SelectedItem is ObstacleProfile profile)
            {
                _batch.ObstacleProfileId = profile.Id;
                _batch.MonthlyObstacleFactors = (double[])profile.MonthlyFactors.Clone();
            }

            // Генериране на TypeName (SaveAllData)
            if (string.IsNullOrEmpty(_batch.TypeName))
            {
                if (_batch.Width > 0 && _batch.Height > 0)
                {
                    _batch.TypeName = $"{_batch.Width:F2}×{_batch.Height:F2} {GetKindLabel(_batch.Kind)}";
                }
                else
                {
                    _batch.TypeName = $"A={_batch.AreaGross:F2}m² {GetKindLabel(_batch.Kind)}";
                }
            }

            DialogResult = true;
            Close();
        }

        private string GetKindLabel(WindowKind kind)
        {
            return kind == WindowKind.Window ? "Прозорец" : "Врата";
        }

        private static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi != null && Attribute.GetCustomAttribute(fi, typeof(System.ComponentModel.DescriptionAttribute)) is System.ComponentModel.DescriptionAttribute attr)
                return attr.Description;
            return value.ToString();
        }

        private static bool TryParseDouble(string? text, out double result)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;
            // Try current culture first (accepts comma in many locales), then invariant (dot)
            if (double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out result)) return true;
            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out result)) return true;
            // Fallback: replace comma with dot and try invariant
            var alt = text.Replace(',', '.');
            return double.TryParse(alt, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        }

        private static bool TryParseInt(string? text, out int result)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.CurrentCulture, out result)) return true;
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out result)) return true;
            return int.TryParse(text.Replace(',', '.'), NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
        }
    }
}
