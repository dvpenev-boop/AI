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
        // shading
        private ShadingConfig? _shadingConfigLocal;
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
            // Initialize width/height (user inputs are in cm)
            WidthTextBox.Text = _batch.Width > 0 ? (_batch.Width * 100.0).ToString("F0") : string.Empty;
            HeightTextBox.Text = _batch.Height > 0 ? (_batch.Height * 100.0).ToString("F0") : string.Empty;
            WidthTextBox.TextChanged += AnyInputChanged;
            HeightTextBox.TextChanged += AnyInputChanged;

            // U/g/OpticalType
            UValueTextBox.Text = _batch.UValue > 0 ? _batch.UValue.ToString("F2") : "1.40";
            UValueTextBox.TextChanged += AnyInputChanged;
            // GN is computed from glazing tables (read-only)
            GNTextBox.Text = _batch.GN > 0 ? _batch.GN.ToString("F3") : string.Empty;
            OpticalTypeComboBox.ItemsSource = Enum.GetValues(typeof(OpticalType));
            OpticalTypeComboBox.SelectedItem = _batch.OpticalType;
            OpticalTypeComboBox.SelectionChanged += AnyInputChanged;

            // Glazing type (Table 3) and g_diff input
            GlazingTypeComboBox.ItemsSource = Enum.GetValues(typeof(GlazingType)).Cast<GlazingType>().Select(g => new { Value = g, Label = GetEnumDescription(g) }).ToList();
            GlazingTypeComboBox.DisplayMemberPath = "Label";
            GlazingTypeComboBox.SelectedValuePath = "Value";
            GlazingTypeComboBox.SelectedValue = _batch.GlazingType;
            GlazingTypeComboBox.SelectionChanged += (s, e) =>
            {
                if (GlazingTypeComboBox.SelectedValue is GlazingType gt)
                {
                    GlazingGAltTextBlock.Text = WindowCalculator.GetGlazingGAlt(gt).ToString("F3");
                }
                AnyInputChanged(s, e);
            };

            GlazingGAltTextBlock.Text = WindowCalculator.GetGlazingGAlt(_batch.GlazingType).ToString("F3");
            GlazingGDiffTextBox.Text = _batch.GlazingGDif > 0 ? _batch.GlazingGDif.ToString("F3") : string.Empty;
            GlazingGDiffTextBox.TextChanged += AnyInputChanged;

            // FrameFraction
            FrameFractionTextBox.Text = (_batch.FrameFraction * 100).ToString("F1");
            FrameFractionTextBox.TextChanged += AnyInputChanged;

            // Shading - use ComboBox for mode (0=None,1=Internal,2=External)
            ShadingModeComboBox.SelectionChanged += (s, e) => AnyInputChanged(s, e);
            // Default shading mode: none if no shading type saved, otherwise internal
            ShadingModeComboBox.SelectedIndex = string.IsNullOrEmpty(_batch.ShadingTypeId) ? 0 : 1;
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
            // Replace old obstacle dropdown with simplified shading controls
            // Wiring for edit/clear buttons and initial summary
            EditShadingButton.Click += EditShadingButton_Click;
            ClearShadingButton.Click += (s, e) => { ClearShading(); UpdatePreview(); };
            // initialize from existing batch shading config (if present)
            if (_batch.ShadingConfig != null)
            {
                _shadingConfigLocal = _batch.ShadingConfig;
                WithShadingRadio.IsChecked = true;
            }
            else
            {
                _shadingConfigLocal = null;
                NoShadingRadio.IsChecked = true;
            }
            UpdateShadingSummaryUI();

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

        private void EditShadingButton_Click(object? sender, RoutedEventArgs e)
        {
            // Open the modal shading editor using current window geometry and orientation.
            // Prefer values from the Width/Height text boxes (user input in cm), fallback to _batch values (m).
            double wk = _batch.Width;
            double hk = _batch.Height;
            if (TryParseDouble(WidthTextBox?.Text, out double wCm) && wCm > 0)
            {
                wk = wCm / 100.0;
            }
            if (TryParseDouble(HeightTextBox?.Text, out double hCm) && hCm > 0)
            {
                hk = hCm / 100.0;
            }

            if (wk <= 0 || hk <= 0)
            {
                MessageBox.Show("Моля, първо въведете валидна ширина и височина.", "Грешка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new ShadingEditorDialog(wk, hk, _batch.Orientation, _shadingConfigLocal);
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
            {
                _shadingConfigLocal = dialog.Result;
                // Update batch previewed shading immediately
                _batch.ShadingConfig = _shadingConfigLocal;
                if (_shadingConfigLocal?.FshDirMonthly != null && _shadingConfigLocal.FshDirMonthly.Length == 12)
                    _batch.FshDirMonthly = (double[])_shadingConfigLocal.FshDirMonthly.Clone();
                else
                    _batch.FshDirMonthly = Enumerable.Repeat(1.0, 12).ToArray();

                // Set UI to show shading
                // (radio buttons and summary updated)
                if (WithShadingRadio != null) WithShadingRadio.IsChecked = true;
                UpdateShadingSummaryUI();
                UpdatePreview();
            }
        }

        private void ClearShading()
        {
            _shadingConfigLocal = null;
            _batch.ShadingConfig = null;
            _batch.FshDirMonthly = Enumerable.Repeat(1.0, 12).ToArray();
            NoShadingRadio.IsChecked = true;
            UpdateShadingSummaryUI();
        }

        private void UpdateShadingSummaryUI()
        {
            if (ShadingSummaryTextBlock == null)
                return;

            if (_shadingConfigLocal == null)
            {
                ShadingSummaryTextBlock.Text = "Няма засенчване";
                return;
            }

            // Show basic summary: number of objects and min/avg/max of monthly F_sh,dir if available
            var arr = _shadingConfigLocal.FshDirMonthly ?? Enumerable.Repeat(1.0, 12).ToArray();
            if (arr.Length == 12)
            {
                double min = arr.Min();
                double avg = arr.Average();
                double max = arr.Max();
                ShadingSummaryTextBlock.Text = $"Обекти: {_shadingConfigLocal.Shadings.Count}, F_sh,dir (min/avg/max) = {min:F3}/{avg:F3}/{max:F3}";
            }
            else
            {
                ShadingSummaryTextBlock.Text = $"Обекти: {_shadingConfigLocal.Shadings.Count}";
            }
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

            // Glazing and g calculations (Variant B)
            var selectedGlazing = GlazingTypeComboBox.SelectedValue is GlazingType gtv ? gtv : _batch.GlazingType;
            double gAlt = WindowCalculator.GetGlazingGAlt(selectedGlazing);
            double gDif = TryParseDouble(GlazingGDiffTextBox.Text, out var gdifVal) && gdifVal > 0 ? gdifVal : (_batch.GlazingGDif > 0 ? _batch.GlazingGDif : gAlt);

            const double a_gl = 0.75; // table 3 default for vertical
            const double Fw = 0.90; // correction factor from table 3

            double g_no_shade_base = a_gl * gAlt + (1 - a_gl) * gDif; // formula 3.42 base
            double g_n_computed = Fw * g_no_shade_base; // apply correction 3.41

            // shading (use τ from selected shading option if any)
            double tau = 1.0;
            if (ShadingModeComboBox.SelectedIndex > 0 && ShadingOptionsDataGrid.SelectedItem is ShadingOption sop)
            {
                tau = sop.TransmittanceTau > 0 ? sop.TransmittanceTau : 1.0;
            }

            double g_with_shade_base = a_gl * (gAlt * tau) + (1 - a_gl) * (gDif * tau);
            double g_eff = Fw * g_with_shade_base;
            double shadingFactor = g_no_shade_base > 1e-9 ? (g_with_shade_base / g_no_shade_base) : 1.0;

            // set computed GN (read-only textbox)
            GNTextBox.Text = g_n_computed.ToString("F3");
            int count = TryParseInt(CountTextBox.Text, out var c) ? c : 1;
            double ua = u * areaGross;
            double totalGross = count * areaGross;
            double totalGlass = count * areaGlass;
            double totalUA = count * ua;
            double totalGA = count * g_eff * areaGlass;
            // Set preview fields
            PreviewAreaGross.Text = areaGross.ToString("F3");
            PreviewAreaGlass.Text = areaGlass.ToString("F3");
            PreviewGBase.Text = g_no_shade_base.ToString("F3");
            PreviewShadingReduction.Text = shadingFactor.ToString("F3");
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
            // GN is computed from glazing tables (do not read user input)
            _batch.UValue = u;
            // save glazing selections
            _batch.GlazingType = GlazingTypeComboBox.SelectedValue is GlazingType gtv ? gtv : _batch.GlazingType;
            _batch.GlazingGDif = TryParseDouble(GlazingGDiffTextBox.Text, out var gd) ? gd : _batch.GlazingGDif;

            // compute GN and shading factor again for save
            double gAlt_save = WindowCalculator.GetGlazingGAlt(_batch.GlazingType);
            double gDif_save = _batch.GlazingGDif > 0 ? _batch.GlazingGDif : gAlt_save;
            const double a_gl = 0.75;
            const double Fw = 0.90;
            double g_no_shade_base_save = a_gl * gAlt_save + (1 - a_gl) * gDif_save;
            double g_n_save = Fw * g_no_shade_base_save;
            _batch.GN = g_n_save;
            _batch.OpticalType = OpticalTypeComboBox.SelectedItem is OpticalType ot ? ot : OpticalType.Clear;

            // 4. Рамка
            TryParseDouble(FrameFractionTextBox.Text, out double ffr);
            _batch.FrameFraction = ffr / 100.0;

            // 5. Слънцезащита (ComboBox mode)
            // Compute shading reduction factor based on τ
            if (ShadingModeComboBox.SelectedIndex == 0)
            {
                _batch.ShadingTypeId = null;
                _batch.ShadingReductionFactor = 1.0;
            }
            else if (ShadingOptionsDataGrid.SelectedItem is ShadingOption option)
            {
                _batch.ShadingTypeId = option.Id;
                double tau_save = option.TransmittanceTau > 0 ? option.TransmittanceTau : 1.0;
                double g_with_shade_base_save = a_gl * (gAlt_save * tau_save) + (1 - a_gl) * (gDif_save * tau_save);
                double shadingFactor_save = g_no_shade_base_save > 1e-9 ? (g_with_shade_base_save / g_no_shade_base_save) : 1.0;
                _batch.ShadingReductionFactor = shadingFactor_save;
            }

            // 6. Препятствия (засенчване) - използваме ShadingConfig и месечни F_sh,dir
            if (_shadingConfigLocal != null)
            {
                _batch.ShadingConfig = _shadingConfigLocal;
                // copy monthly F_sh,dir into batch (ensure length 12)
                if (_shadingConfigLocal.FshDirMonthly != null && _shadingConfigLocal.FshDirMonthly.Length == 12)
                    _batch.FshDirMonthly = (double[])_shadingConfigLocal.FshDirMonthly.Clone();
                else
                    _batch.FshDirMonthly = Enumerable.Repeat(1.0, 12).ToArray();
            }
            else
            {
                // No shading: default 1.0 for all months
                _batch.ShadingConfig = null;
                _batch.FshDirMonthly = Enumerable.Repeat(1.0, 12).ToArray();
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
