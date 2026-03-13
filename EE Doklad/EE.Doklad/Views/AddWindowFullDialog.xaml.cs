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
        private List<WindowProfileSystemOption> _profileSystemOptions = new();
        private List<WindowThermalBridgeOption> _thermalBridgeOptions = new();
        // shading
        private ShadingConfig? _shadingConfigLocal;
        // Climate zone for seasonal calculations (1-9), null = use defaults
        private int? _climateZone;
        // Season enabled flags
        private bool _heatingSeasonEnabled = true;
        private bool _coolingSeasonEnabled = true;
        // Cooling season month range from Section 5 (1-based, null = not configured)
        private int? _coolingStartMonth;
        private int? _coolingEndMonth;
        // Whether this dialog was opened for a new (not existing) batch
        private bool _isNewBatch;
        private static string? s_lastProfileSystemId;
        private static double? s_lastManualMountingDepthMm;
        private static double? s_lastManualVisibleHeightMm;

        public WindowBatch Result => _batch;

        public AddWindowFullDialog(WindowBatch? existingBatch = null, int? climateZone = null, 
                                    bool heatingEnabled = true, bool coolingEnabled = true,
                                    int? coolingStartMonth = null, int? coolingEndMonth = null)
        {
            InitializeComponent();
            _isNewBatch = existingBatch == null;
            _batch = existingBatch ?? new WindowBatch();
            _climateZone = climateZone;
            _heatingSeasonEnabled = heatingEnabled;
            _coolingSeasonEnabled = coolingEnabled;
            _coolingStartMonth = coolingStartMonth;
            _coolingEndMonth = coolingEndMonth;
            LoadShadingOptions();
            LoadObstacleProfiles();
            LoadProfileSystems();
            LoadThermalBridgeOptions();
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

        private void LoadProfileSystems()
        {
            _profileSystemOptions = WindowCalculator.GetProfileSystemOptions();
        }

        private void LoadThermalBridgeOptions()
        {
            _thermalBridgeOptions = WindowCalculator.GetThermalBridgeOptions();
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

            InitializeProfileSystemControls();
            InitializeThermalBridgeControls();

            // Geometry
            // Initialize width/height (user inputs are in cm)
            WidthTextBox.Text = _batch.Width > 0 ? (_batch.Width * 100.0).ToString("F0") : string.Empty;
            HeightTextBox.Text = _batch.Height > 0 ? (_batch.Height * 100.0).ToString("F0") : string.Empty;
            WidthTextBox.TextChanged += AnyInputChanged;
            HeightTextBox.TextChanged += AnyInputChanged;

            // U/g/OpticalType
            UValueTextBox.Text = _batch.UValue > 0 ? _batch.UValue.ToString("F2") : "1.40";
            UValueTextBox.TextChanged += AnyInputChanged;
            DetailedUwModeCheckBox.IsChecked = _batch.UseDetailedUwMode;
            DetailedUwModeCheckBox.Checked += AnyInputChanged;
            DetailedUwModeCheckBox.Unchecked += AnyInputChanged;
            ProfileUFrameTextBox.Text = _batch.ProfileUFrame > 0 ? _batch.ProfileUFrame.ToString("F2") : string.Empty;
            ProfileUGlassTextBox.Text = _batch.ProfileUGlass > 0 ? _batch.ProfileUGlass.ToString("F2") : string.Empty;
            ProfileUFrameTextBox.TextChanged += AnyInputChanged;
            ProfileUGlassTextBox.TextChanged += AnyInputChanged;
            // GN is computed from glazing tables (read-only)
            GNTextBox.Text = _batch.GN > 0 ? _batch.GN.ToString("F3") : string.Empty;
            OpticalTypeComboBox.ItemsSource = Enum.GetValues(typeof(OpticalType));
            OpticalTypeComboBox.SelectedItem = _batch.OpticalType;
            OpticalTypeComboBox.SelectionChanged += AnyInputChanged;

            // Glazing emissivity preset (Тип стъкло) and ε field
            var emissivityPresets = Enum.GetValues(typeof(GlazingEmissivityPreset))
                .Cast<GlazingEmissivityPreset>()
                .Select(p => new { Value = p, Label = GetEnumDescription(p) })
                .ToList();
            GlazingEmissivityPresetComboBox.ItemsSource = emissivityPresets;
            GlazingEmissivityPresetComboBox.DisplayMemberPath = "Label";
            GlazingEmissivityPresetComboBox.SelectedValuePath = "Value";
            GlazingEmissivityPresetComboBox.SelectedValue = _batch.GlazingEmissivityPreset;
            // For a brand-new batch the stored GlassEmissivity matches the model field default (0.84).
            // If the stored value equals the old model default AND we are in a new batch (no area set),
            // show the preset value (0.05 for LowEHighInsulation) so the UI reflects the selected preset.
            double displayEmissivity = _isNewBatch
                ? GlazingEmissivityHelper.GetEmissivity(_batch.GlazingEmissivityPreset)
                : (_batch.GlassEmissivity > 0 ? _batch.GlassEmissivity : 0.84);
            GlassEmissivityTextBox.Text = displayEmissivity.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

            // When a preset is selected → auto-populate the emissivity field
            GlazingEmissivityPresetComboBox.SelectionChanged += (s, e) =>
            {
                if (GlazingEmissivityPresetComboBox.SelectedValue is GlazingEmissivityPreset preset)
                {
                    double eps = GlazingEmissivityHelper.GetEmissivity(preset);
                    GlassEmissivityTextBox.Text = eps.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
                }
            };
            // Manual override is allowed – the TextBox is not read-only

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
            UpdateWindowSpecificUi();

            // ── Слънцезащита – Отоплителен сезон ──────────────────────────────
            InitShadingControls(
                ShadingModeHeatComboBox,
                ShadingCategoryHeatComboBox,
                ShadingOptionsHeatDataGrid,
                savedMode: _batch.ShadingModeHeat,
                savedTypeId: _batch.ShadingTypeIdHeat);

            // ── Слънцезащита – Охладителен сезон ──────────────────────────────
            InitShadingControls(
                ShadingModeCoolComboBox,
                ShadingCategoryCoolComboBox,
                ShadingOptionsCoolDataGrid,
                savedMode: _batch.ShadingModeCool,
                savedTypeId: _batch.ShadingTypeIdCool);

            // Obstacle (section 6) - ComboBox replaces radio buttons
            ObstacleModeComboBox.SelectionChanged += (s, e) =>
            {
                bool hasShading = ObstacleModeComboBox.SelectedIndex == 1;
                EditShadingButton.IsEnabled = hasShading;
                ClearShadingButton.IsEnabled = hasShading;
                UpdateShadingSummaryUI();
                UpdatePreview();
            };
            EditShadingButton.Click += EditShadingButton_Click;
            ClearShadingButton.Click += (s, e) => { ClearShading(); UpdatePreview(); };
            // initialize from existing batch shading config (if present)
            if (_batch.ShadingConfig != null)
            {
                _shadingConfigLocal = _batch.ShadingConfig;
                ObstacleModeComboBox.SelectedIndex = 1;
                EditShadingButton.IsEnabled = true;
                ClearShadingButton.IsEnabled = true;
            }
            else
            {
                _shadingConfigLocal = null;
                ObstacleModeComboBox.SelectedIndex = 0;
                EditShadingButton.IsEnabled = false;
                ClearShadingButton.IsEnabled = false;
            }
            UpdateShadingSummaryUI();

            // Buttons
            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += (s, e) => { DialogResult = false; Close(); };
            ResetButton.Click += (s, e) => { InitializeAllSections(); UpdatePreview(); };
        }

        private void InitializeProfileSystemControls()
        {
            ProfileSystemComboBox.ItemsSource = _profileSystemOptions;
            ProfileSystemComboBox.DisplayMemberPath = nameof(WindowProfileSystemOption.DisplayLabel);
            ProfileSystemComboBox.SelectedValuePath = nameof(WindowProfileSystemOption.Id);

            string? selectedProfileId = _batch.ProfileSystemId;
            if (string.IsNullOrWhiteSpace(selectedProfileId) && _isNewBatch)
                selectedProfileId = s_lastProfileSystemId;
            if (string.IsNullOrWhiteSpace(selectedProfileId))
                selectedProfileId = _profileSystemOptions.FirstOrDefault()?.Id;

            ProfileSystemComboBox.SelectedValue = selectedProfileId;
            ProfileSystemComboBox.SelectionChanged += AnyInputChanged;

            if (_batch.ProfileMountingDepthMm.HasValue)
                ManualMountingDepthTextBox.Text = _batch.ProfileMountingDepthMm.Value.ToString("F0");
            else if (_isNewBatch && s_lastManualMountingDepthMm.HasValue)
                ManualMountingDepthTextBox.Text = s_lastManualMountingDepthMm.Value.ToString("F0");

            if (_batch.ProfileVisibleHeightMm.HasValue)
                ManualVisibleHeightTextBox.Text = _batch.ProfileVisibleHeightMm.Value.ToString("F0");
            else if (_isNewBatch && s_lastManualVisibleHeightMm.HasValue)
                ManualVisibleHeightTextBox.Text = s_lastManualVisibleHeightMm.Value.ToString("F0");

            ManualMountingDepthTextBox.TextChanged += AnyInputChanged;
            ManualVisibleHeightTextBox.TextChanged += AnyInputChanged;
        }

        private void InitializeThermalBridgeControls()
        {
            ThermalBridgeComboBox.ItemsSource = _thermalBridgeOptions;
            ThermalBridgeComboBox.DisplayMemberPath = nameof(WindowThermalBridgeOption.DisplayLabel);
            ThermalBridgeComboBox.SelectedValuePath = nameof(WindowThermalBridgeOption.Id);
            ThermalBridgeComboBox.SelectionChanged += AnyInputChanged;

            ThermalBridgeYesRadioButton.Checked += AnyInputChanged;
            ThermalBridgeNoRadioButton.Checked += AnyInputChanged;

            ThermalBridgeYesRadioButton.IsChecked = _batch.HasThermalBridge;
            ThermalBridgeNoRadioButton.IsChecked = !_batch.HasThermalBridge;

            if (!string.IsNullOrWhiteSpace(_batch.ThermalBridgeTypeId))
            {
                ThermalBridgeComboBox.SelectedValue = _batch.ThermalBridgeTypeId;
            }
            else if (_thermalBridgeOptions.Count > 0)
            {
                ThermalBridgeComboBox.SelectedIndex = 0;
            }

            UpdateThermalBridgeUi();
        }

        private void UpdateThermalBridgeUi()
        {
            bool hasThermalBridge = ThermalBridgeYesRadioButton.IsChecked == true;
            ThermalBridgeComboBox.Visibility = hasThermalBridge ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateWindowSpecificUi()
        {
            bool isWindow = KindComboBox.SelectedValue is WindowKind kind && kind == WindowKind.Window;
            bool detailedMode = isWindow && DetailedUwModeCheckBox.IsChecked == true;

            ProfileSystemLabel.Visibility = isWindow ? Visibility.Visible : Visibility.Collapsed;
            ProfileSystemPanel.Visibility = isWindow ? Visibility.Visible : Visibility.Collapsed;
            ProfileSystemHintTextBlock.Visibility = isWindow ? Visibility.Visible : Visibility.Collapsed;
            DetailedUwModeCheckBox.Visibility = isWindow ? Visibility.Visible : Visibility.Collapsed;
            DetailedUwPanel.Visibility = detailedMode ? Visibility.Visible : Visibility.Collapsed;
            DetailedUwHintTextBlock.Visibility = detailedMode ? Visibility.Visible : Visibility.Collapsed;
            FrameFractionAutoHintTextBlock.Visibility = detailedMode ? Visibility.Visible : Visibility.Collapsed;

            UValueTextBox.IsReadOnly = detailedMode;
            UValueTextBox.Background = detailedMode ? System.Windows.Media.Brushes.WhiteSmoke : System.Windows.Media.Brushes.White;
            FrameFractionTextBox.IsReadOnly = detailedMode;
            FrameFractionTextBox.Background = detailedMode ? System.Windows.Media.Brushes.WhiteSmoke : System.Windows.Media.Brushes.White;

            bool requiresManualProfile = isWindow
                && ProfileSystemComboBox.SelectedItem is WindowProfileSystemOption opt
                && opt.RequiresManualInput;
            ManualMountingDepthTextBox.Visibility = requiresManualProfile ? Visibility.Visible : Visibility.Collapsed;
            ManualVisibleHeightTextBox.Visibility = requiresManualProfile ? Visibility.Visible : Visibility.Collapsed;

            if (isWindow)
            {
                ProfileSystemHintTextBlock.Text = requiresManualProfile
                    ? "За 'Друго' въведете монтажна дълбочина и видима височина в mm."
                    : "Видимата височина на системата се използва за автоматично изчисление на F_fr.";
            }

            if (detailedMode)
            {
                DetailedUwHintTextBlock.Text = "Uw се изчислява автоматично от F_fr, Ufr и Ugl.";
                FrameFractionAutoHintTextBlock.Text = "F_fr се изчислява автоматично от размерите и профилната система.";
            }
            else
            {
                DetailedUwHintTextBlock.Text = string.Empty;
                FrameFractionAutoHintTextBlock.Text = string.Empty;
            }
        }

        private WindowProfileSystemOption? GetSelectedProfileSystem()
        {
            return ProfileSystemComboBox.SelectedItem as WindowProfileSystemOption;
        }

        private double? GetSelectedVisibleHeightMm()
        {
            var option = GetSelectedProfileSystem();
            if (option == null)
                return null;

            if (option.RequiresManualInput)
                return TryParseDouble(ManualVisibleHeightTextBox.Text, out var manualVisible) && manualVisible > 0 ? manualVisible : null;

            return option.VisibleHeightMm;
        }

        private double? GetSelectedMountingDepthMm()
        {
            var option = GetSelectedProfileSystem();
            if (option == null)
                return null;

            if (option.RequiresManualInput)
                return TryParseDouble(ManualMountingDepthTextBox.Text, out var manualDepth) && manualDepth > 0 ? manualDepth : null;

            return option.MountingDepthMm;
        }

        private string GetSelectedProfileSystemLabel()
        {
            var option = GetSelectedProfileSystem();
            if (option == null)
                return string.Empty;

            if (!option.RequiresManualInput)
                return option.DisplayLabel;

            string depthLabel = GetSelectedMountingDepthMm()?.ToString("F0", CultureInfo.InvariantCulture) ?? "?";
            return $"Друго - {depthLabel} mm";
        }

        private void AnyInputChanged(object? sender, EventArgs e)
        {
            UpdateWindowSpecificUi();
            UpdateThermalBridgeUi();
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

            // Use current selection from the OrientationComboBox if available (reflect live UI selection)
            var selectedOrient = _batch.Orientation;
            if (OrientationComboBox?.SelectedValue is EE.Doklad.Models.Orientation oSel)
                selectedOrient = oSel;
            var dialog = new ShadingEditorDialog(wk, hk, selectedOrient, _shadingConfigLocal);
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
                if (ObstacleModeComboBox != null) ObstacleModeComboBox.SelectedIndex = 1;
                EditShadingButton.IsEnabled = true;
                ClearShadingButton.IsEnabled = true;
                UpdateShadingSummaryUI();
                UpdatePreview();
            }
        }

        private void ClearShading()
        {
            _shadingConfigLocal = null;
            _batch.ShadingConfig = null;
            _batch.FshDirMonthly = Enumerable.Repeat(1.0, 12).ToArray();
            if (ObstacleModeComboBox != null) ObstacleModeComboBox.SelectedIndex = 0;
            EditShadingButton.IsEnabled = false;
            ClearShadingButton.IsEnabled = false;
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

        // ── Helper: initialise one shading row (Heat or Cool) ─────────────────
        private void InitShadingControls(
            ComboBox modeCombo, ComboBox categoryCombo, DataGrid optionsGrid,
            int savedMode, string? savedTypeId)
        {
            // Populate category list
            categoryCombo.ItemsSource = _shadingByCategory.Keys.ToList();

            // Wire category → grid population
            categoryCombo.SelectionChanged += (s, e) =>
            {
                if (categoryCombo.SelectedItem is string cat && _shadingByCategory.ContainsKey(cat))
                {
                    optionsGrid.ItemsSource = _shadingByCategory[cat];
                    if (optionsGrid.Items.Count > 0)
                        optionsGrid.SelectedIndex = 0;
                }
            };
            optionsGrid.SelectionChanged += AnyInputChanged;

            // Select category and row from saved state
            if (!string.IsNullOrEmpty(savedTypeId))
            {
                foreach (var kv in _shadingByCategory)
                {
                    var match = kv.Value.FirstOrDefault(o => o.Id == savedTypeId);
                    if (match != null)
                    {
                        categoryCombo.SelectedItem = kv.Key;
                        optionsGrid.ItemsSource = kv.Value;
                        optionsGrid.SelectedItem = match;
                        break;
                    }
                }
            }
            else if (categoryCombo.Items.Count > 0)
            {
                categoryCombo.SelectedIndex = 0;
                if (categoryCombo.SelectedItem is string firstCat && _shadingByCategory.ContainsKey(firstCat))
                {
                    optionsGrid.ItemsSource = _shadingByCategory[firstCat];
                    if (optionsGrid.Items.Count > 0)
                        optionsGrid.SelectedIndex = 0;
                }
            }

            // Wire mode ComboBox
            modeCombo.SelectionChanged += (s, e) =>
            {
                bool hasShading = modeCombo.SelectedIndex > 0;
                categoryCombo.IsEnabled = hasShading;
                optionsGrid.IsEnabled = hasShading;
                if (hasShading && optionsGrid.SelectedItem == null && optionsGrid.Items.Count > 0)
                    optionsGrid.SelectedIndex = 0;
                AnyInputChanged(s, e);
            };

            // Restore saved mode (0/1/2); derive mode from savedTypeId if needed
            int mode = savedMode;
            if (mode == 0 && !string.IsNullOrEmpty(savedTypeId)) mode = 1; // legacy fallback
            modeCombo.SelectedIndex = Math.Clamp(mode, 0, 2);
            categoryCombo.IsEnabled = mode > 0;
            optionsGrid.IsEnabled = mode > 0;
        }

        private void ShadingCategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Legacy stub – individual wiring is now done inside InitShadingControls
        }

        private void UpdatePreview()
        {
            // Изчисления за preview (може да се изнесе във ViewModel/helper)
            double width = TryParseDouble(WidthTextBox.Text, out var w) ? w / 100.0 : 0;
            double height = TryParseDouble(HeightTextBox.Text, out var h) ? h / 100.0 : 0;
            double areaGross = width * height;
            CalculatedAreaTextBlock.Text = areaGross > 0 ? $"Площ: {areaGross:F3} m² (от {w:F0}×{h:F0} см)" : "Площ: —";
            bool isWindow = KindComboBox.SelectedValue is WindowKind kindValue && kindValue == WindowKind.Window;
            bool detailedMode = isWindow && DetailedUwModeCheckBox.IsChecked == true;
            double ffr = TryParseDouble(FrameFractionTextBox.Text, out var f) ? f / 100.0 : 0;

            if (detailedMode)
            {
                double? visibleHeightMm = GetSelectedVisibleHeightMm();
                if (visibleHeightMm.HasValue)
                {
                    ffr = WindowCalculator.CalculateFrameFractionFromProfile(width, height, visibleHeightMm.Value);
                    FrameFractionTextBox.Text = (ffr * 100.0).ToString("F1", CultureInfo.InvariantCulture);
                }

                if (TryParseDouble(ProfileUFrameTextBox.Text, out var uFrame) &&
                    TryParseDouble(ProfileUGlassTextBox.Text, out var uGlass))
                {
                    double autoUw = WindowCalculator.CalculateUwFromDetailedInputs(ffr, uFrame, uGlass);
                    UValueTextBox.Text = autoUw.ToString("F2", CultureInfo.InvariantCulture);
                }
            }

            // Специален случай: Врата + F_fr=100% → плътна врата, без остъкляване
            bool isDoor = KindComboBox.SelectedValue is WindowKind kv && kv == WindowKind.Door;
            bool isSolidDoor = isDoor && ffr >= 1.0;

            double areaGlass = areaGross * (1 - ffr);
            double u = TryParseDouble(UValueTextBox.Text, out var uval) ? uval : 0;

            // Glazing and g calculations (Variant B)
            var selectedGlazing = GlazingTypeComboBox.SelectedValue is GlazingType gtv ? gtv : _batch.GlazingType;
            double gAlt = WindowCalculator.GetGlazingGAlt(selectedGlazing);
            double gDif = TryParseDouble(GlazingGDiffTextBox.Text, out var gdifVal) && gdifVal > 0 ? gdifVal : (_batch.GlazingGDif > 0 ? _batch.GlazingGDif : gAlt);

            const double a_gl = 0.75; // table 3 default for vertical
            const double Fw = 0.90; // correction factor from table 3

            double g_no_shade_base;
            double g_n_computed;
            double shadingFactorHeat;
            double shadingFactorCool;
            double g_eff;

            if (isSolidDoor)
            {
                // Плътна врата: няма остъкляване
                g_no_shade_base = 0.0;
                g_n_computed = 0.0;
                shadingFactorHeat = 1.0;
                shadingFactorCool = 1.0;
                g_eff = 0.0;
            }
            else
            {
                g_no_shade_base = a_gl * gAlt + (1 - a_gl) * gDif; // formula 3.42 base
                g_n_computed = Fw * g_no_shade_base; // 3.41: g_n = 0.90 * base

                shadingFactorHeat = GetShadingFactor(ShadingModeHeatComboBox, ShadingOptionsHeatDataGrid);
                shadingFactorCool = GetShadingFactor(ShadingModeCoolComboBox, ShadingOptionsCoolDataGrid);

                // Preview uses the heating factor for the combined g_eff display row
                g_eff = g_n_computed * shadingFactorHeat;
            }

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
            PreviewShadingReduction.Text = shadingFactorHeat.ToString("F3");
            PreviewGEff.Text = g_eff.ToString("F3");
            PreviewUA.Text = ua.ToString("F3");
            PreviewTotalGrossArea.Text = totalGross.ToString("F3");
            PreviewTotalGlassArea.Text = totalGlass.ToString("F3");
            PreviewTotalUA.Text = totalUA.ToString("F3");
            PreviewTotalGA.Text = totalGA.ToString("F3");

            // If we have a shading config loaded, recalculate its monthly F_sh,dir
            if (_shadingConfigLocal != null && _shadingConfigLocal.Shadings.Count > 0 && width > 0 && height > 0)
            {
                // Determine selected orientation from UI (prefer live selection)
                var selOrient = _batch.Orientation;
                if (OrientationComboBox?.SelectedValue is EE.Doklad.Models.Orientation oSel)
                    selOrient = oSel;

                var detailed = ShadingCalculator.CalculateDetailedMonthly(width, height, selOrient, _shadingConfigLocal.Shadings, _shadingConfigLocal.Latitude, _shadingConfigLocal.NorthHemisphere);
                if (detailed != null && detailed.Count > 0)
                    _shadingConfigLocal.FshDirMonthly = detailed.Select(d => d.FshDir).ToArray();
                else
                    _shadingConfigLocal.FshDirMonthly = Enumerable.Repeat(1.0, 12).ToArray();

                // Also update _batch.FshDirMonthly so seasonal results use the recalculated values in preview
                _batch.FshDirMonthly = (double[])_shadingConfigLocal.FshDirMonthly.Clone();

                // Refresh summary UI (shows min/avg/max)
                UpdateShadingSummaryUI();
            }

            // Compute per-mode GEffBase for preview — each depends ONLY on its own shading selection.
            // This mirrors the GEffBaseHeat / GEffBaseCool properties in the model.
            var optSel = OpticalTypeComboBox.SelectedItem is OpticalType otSel ? otSel : _batch.OpticalType;
            bool hasShadingHeat = ShadingModeHeatComboBox.SelectedIndex > 0;
            bool hasShadingCool = ShadingModeCoolComboBox.SelectedIndex > 0;
            double geffBaseHeatPreview = WindowCalculator.CalculateGEffBase(g_n_computed, optSel, hasShadingHeat);
            double geffBaseCoolPreview = WindowCalculator.CalculateGEffBase(g_n_computed, optSel, hasShadingCool);

            // The top "g_base" field in the preview panel shows the heating base (most common reference)
            PreviewGBase.Text = geffBaseHeatPreview.ToString("F3");
            UpdateSeasonalResults(geffBaseHeatPreview, geffBaseCoolPreview, shadingFactorHeat, shadingFactorCool);
        }

        /// <summary>Reads the shading reduction factor from a mode/grid pair.</summary>
        private double GetShadingFactor(ComboBox modeCombo, DataGrid optionsGrid)
        {
            if (modeCombo.SelectedIndex <= 0) return 1.0;
            ShadingOption? sop = optionsGrid.SelectedItem as ShadingOption
                ?? optionsGrid.Items.OfType<ShadingOption>().FirstOrDefault();
            if (sop == null) return 1.0;
            // 1 = Вътрешна → FShadeInt, 2 = Външна → FShadeExt
            return modeCombo.SelectedIndex == 1 ? sop.FShadeInt : sop.FShadeExt;
        }

    private void UpdateSeasonalResults(double gBaseHeat, double gBaseCool, double shadingFactorHeat, double shadingFactorCool)
        {
            // Ако нито един режим не е активен, скриваме целия GroupBox
            if (!_heatingSeasonEnabled && !_coolingSeasonEnabled)
            {
                SeasonalResultsGroupBox.Visibility = Visibility.Collapsed;
                return;
            }
            
            SeasonalResultsGroupBox.Visibility = Visibility.Visible;

            // ── Месечни g_eff (всички 12 месеца, всеки сезон с отделен shading factor) ──
            double[] gEffHeatMonthly = new double[12];
            double[] gEffCoolMonthly = new double[12];
            for (int m = 0; m < 12; m++)
            {
                double F_sh_dir_m = _batch.FshDirMonthly[m];
                // standard values using GEffBase (consistent with SaveButton_Click and matrix)
                gEffHeatMonthly[m] = gBaseHeat * shadingFactorHeat * F_sh_dir_m;
                gEffCoolMonthly[m] = gBaseCool * shadingFactorCool * F_sh_dir_m;
            }

            // ── Отоплителен сезон – месеците идват от климатична зона (Секция 5) ──
            WindowCalculator.TryGetHeatingMonths(_climateZone, _heatingSeasonEnabled, out List<int> heatingMonths);

            if (_heatingSeasonEnabled && heatingMonths.Count > 0)
            {
                double heatingAvg = heatingMonths.Average(m => gEffHeatMonthly[m]);
                HeatingSeasonRow.Visibility = Visibility.Visible;
                // Display the first and last month names from the resolved list
                HeatingMonthsText.Text = $"{GetMonthName(heatingMonths[0])}-{GetMonthName(heatingMonths[^1])} ({heatingMonths.Count} месеца)";
                HeatingGEffAvg.Text = heatingAvg.ToString("F3");
                int zone = Math.Clamp(_climateZone ?? 1, 1, 9);
                HeatingNote.Text = $"Средно за отоплителен период (Климатична зона {zone})";
            }
            else
            {
                if (_heatingSeasonEnabled)
                {
                    // Enabled but no months defined -> show 0
                    HeatingSeasonRow.Visibility = Visibility.Visible;
                    HeatingMonthsText.Text = "0 месеца";
                    HeatingGEffAvg.Text = "0,000";
                    HeatingNote.Text = "Няма зададен отоплителен период";
                }
                else
                {
                    HeatingSeasonRow.Visibility = Visibility.Collapsed;
                }
            }

            // ── Охладителен сезон – месеците идват САМО от Секция 5 (без complement) ──
            WindowCalculator.TryGetCoolingMonths(_coolingStartMonth, _coolingEndMonth, _coolingSeasonEnabled, out List<int> coolingMonths);

            if (_coolingSeasonEnabled && coolingMonths.Count > 0)
            {
                double coolingAvg = coolingMonths.Average(m => gEffCoolMonthly[m]);
                CoolingSeasonRow.Visibility = Visibility.Visible;
                CoolingMonthsText.Text = $"{GetMonthName(coolingMonths[0])}-{GetMonthName(coolingMonths[^1])} ({coolingMonths.Count} месеца)";
                CoolingGEffAvg.Text = coolingAvg.ToString("F3");
                CoolingNote.Text = "Средно за охладителен период";
            }
            else
            {
                if (_coolingSeasonEnabled)
                {
                    // Enabled but no months defined -> show 0
                    CoolingSeasonRow.Visibility = Visibility.Visible;
                    CoolingMonthsText.Text = "0 месеца";
                    CoolingGEffAvg.Text = "0,000";
                    CoolingNote.Text = "Охладителният период не е зададен в Секция 5";
                }
                else
                {
                    CoolingSeasonRow.Visibility = Visibility.Collapsed;
                }
            }
        }

        private string GetMonthName(int monthIndex)
        {
            string[] names = { "Яну", "Фев", "Мар", "Апр", "Май", "Юни", "Юли", "Авг", "Сеп", "Окт", "Ное", "Дек" };
            return names[Math.Clamp(monthIndex, 0, 11)];
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

            // F_fr: Прозорец → 0..50%, Врата → 0..100%
            bool isDoor = KindComboBox.SelectedValue is WindowKind k && k == WindowKind.Door;
            bool isWindow = !isDoor;
            bool detailedMode = isWindow && DetailedUwModeCheckBox.IsChecked == true;
            double maxFfr = isDoor ? 100.0 : 50.0;
            if (!TryParseDouble(FrameFractionTextBox.Text, out var ffr) || ffr < 0 || ffr > maxFfr)
                FrameFractionError.Text = isDoor ? "F_fr 0..100% (Врата)" : "F_fr 0..50% (Прозорец)";
            else
                FrameFractionError.Text = "";
            valid &= string.IsNullOrEmpty(FrameFractionError.Text);

            if (detailedMode)
            {
                if (!TryParseDouble(ProfileUFrameTextBox.Text, out var uFrame) || uFrame <= 0)
                {
                    UError.Text = "Ufr трябва да е > 0";
                    valid = false;
                }

                if (!TryParseDouble(ProfileUGlassTextBox.Text, out var uGlass) || uGlass <= 0)
                {
                    UError.Text = "Ugl трябва да е > 0";
                    valid = false;
                }

                if (!GetSelectedVisibleHeightMm().HasValue)
                {
                    FrameFractionError.Text = "Липсва видима височина на профила";
                    valid = false;
                }
            }

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
            _batch.UValue = u;
            _batch.UseDetailedUwMode = _batch.Kind == WindowKind.Window && DetailedUwModeCheckBox.IsChecked == true;
            _batch.ProfileSystemId = _batch.Kind == WindowKind.Window ? ProfileSystemComboBox.SelectedValue as string : null;
            _batch.ProfileSystemLabel = _batch.Kind == WindowKind.Window ? GetSelectedProfileSystemLabel() : string.Empty;
            _batch.ProfileMountingDepthMm = _batch.Kind == WindowKind.Window ? GetSelectedMountingDepthMm() : null;
            _batch.ProfileVisibleHeightMm = _batch.Kind == WindowKind.Window ? GetSelectedVisibleHeightMm() : null;
            _batch.ProfileUFrame = TryParseDouble(ProfileUFrameTextBox.Text, out var uFrameSave) ? uFrameSave : 0.0;
            _batch.ProfileUGlass = TryParseDouble(ProfileUGlassTextBox.Text, out var uGlassSave) ? uGlassSave : 0.0;
            _batch.HasThermalBridge = ThermalBridgeYesRadioButton.IsChecked == true;
            if (_batch.HasThermalBridge && ThermalBridgeComboBox.SelectedItem is WindowThermalBridgeOption thermalBridge)
            {
                _batch.ThermalBridgeTypeId = thermalBridge.Id;
                _batch.ThermalBridgeTypeLabel = thermalBridge.InstallationType;
                _batch.ThermalBridgePsi = thermalBridge.Psi;
            }
            else
            {
                _batch.ThermalBridgeTypeId = null;
                _batch.ThermalBridgeTypeLabel = string.Empty;
                _batch.ThermalBridgePsi = 0.0;
            }
            _batch.GlazingType = GlazingTypeComboBox.SelectedValue is GlazingType gtv ? gtv : _batch.GlazingType;
            _batch.GlazingGDif = TryParseDouble(GlazingGDiffTextBox.Text, out var gd) ? gd : _batch.GlazingGDif;

            // 4. Рамка
            TryParseDouble(FrameFractionTextBox.Text, out double ffr);
            if (_batch.UseDetailedUwMode && _batch.ProfileVisibleHeightMm.HasValue)
            {
                ffr = WindowCalculator.CalculateFrameFractionFromProfile(_batch.Width, _batch.Height, _batch.ProfileVisibleHeightMm.Value) * 100.0;
                _batch.UValue = WindowCalculator.CalculateUwFromDetailedInputs(ffr / 100.0, _batch.ProfileUFrame, _batch.ProfileUGlass);
            }
            _batch.FrameFraction = ffr / 100.0;

            // Специален случай: Врата + F_fr=100% → плътна врата
            bool isSolidDoor = _batch.Kind == WindowKind.Door && _batch.FrameFraction >= 1.0;

            if (isSolidDoor)
            {
                // Плътна врата: без остъкляване – g=0 за всички режими
                _batch.GN = 0.0;
                _batch.OpticalType = OpticalType.Clear;
                _batch.ShadingTypeId = null;
                _batch.ShadingReductionFactor = 1.0;
                _batch.ShadingConfig = null;
                _batch.FshDirMonthly = Enumerable.Repeat(1.0, 12).ToArray();
                _batch.GEffMonthly = new double[12]; // all 0
                _batch.GEffHeat = 0.0;
                _batch.GEffCool = 0.0;
                // Still save emissivity for solid doors (informative)
                if (GlazingEmissivityPresetComboBox.SelectedValue is GlazingEmissivityPreset epDoor)
                    _batch.GlazingEmissivityPreset = epDoor;
                _batch.GlassEmissivity = TryParseDouble(GlassEmissivityTextBox.Text, out double epsDoor)
                    ? Math.Clamp(epsDoor, 0.0, 1.0)
                    : GlazingEmissivityHelper.GetEmissivity(_batch.GlazingEmissivityPreset);
            }
            else
            {
                // Обичайно изчисление (прозорец или врата с частично остъкляване)
                double gAlt_save = WindowCalculator.GetGlazingGAlt(_batch.GlazingType);
                double gDif_save = _batch.GlazingGDif > 0 ? _batch.GlazingGDif : gAlt_save;
                const double a_gl = 0.75;
                const double Fw = 0.90;
                double g_no_shade_base_save = a_gl * gAlt_save + (1 - a_gl) * gDif_save;
                double g_n_save = Fw * g_no_shade_base_save;
                _batch.GN = g_n_save;
                _batch.OpticalType = OpticalTypeComboBox.SelectedItem is OpticalType ot ? ot : OpticalType.Clear;

                // Тип стъкло / емисивитет ε (информативно – не влияе на U или g изчисленията)
                if (GlazingEmissivityPresetComboBox.SelectedValue is GlazingEmissivityPreset ep)
                    _batch.GlazingEmissivityPreset = ep;
                _batch.GlassEmissivity = TryParseDouble(GlassEmissivityTextBox.Text, out double eps)
                    ? Math.Clamp(eps, 0.0, 1.0)
                    : GlazingEmissivityHelper.GetEmissivity(_batch.GlazingEmissivityPreset);

                // 5. Слънцезащита – запазваме отделно за отопление и охлаждане
                double shadingFactorHeat_save = GetShadingFactor(ShadingModeHeatComboBox, ShadingOptionsHeatDataGrid);
                double shadingFactorCool_save = GetShadingFactor(ShadingModeCoolComboBox, ShadingOptionsCoolDataGrid);

                _batch.ShadingModeHeat = ShadingModeHeatComboBox.SelectedIndex;
                _batch.ShadingModeCool = ShadingModeCoolComboBox.SelectedIndex;
                _batch.ShadingReductionFactorHeat = shadingFactorHeat_save;
                _batch.ShadingReductionFactorCool = shadingFactorCool_save;

                // Persist selected shading type IDs
                if (ShadingModeHeatComboBox.SelectedIndex > 0)
                {
                    ShadingOption? optH = ShadingOptionsHeatDataGrid.SelectedItem as ShadingOption
                        ?? ShadingOptionsHeatDataGrid.Items.OfType<ShadingOption>().FirstOrDefault();
                    _batch.ShadingTypeIdHeat = optH?.Id;
                }
                else _batch.ShadingTypeIdHeat = null;

                if (ShadingModeCoolComboBox.SelectedIndex > 0)
                {
                    ShadingOption? optC = ShadingOptionsCoolDataGrid.SelectedItem as ShadingOption
                        ?? ShadingOptionsCoolDataGrid.Items.OfType<ShadingOption>().FirstOrDefault();
                    _batch.ShadingTypeIdCool = optC?.Id;
                }
                else _batch.ShadingTypeIdCool = null;

                // Legacy single-shading fields (ShadingTypeId / ShadingReductionFactor) are used
                // ONLY to determine the GEffBase formula branch (3.41 vs 3.42) in the model.
                // IMPORTANT: they must reflect ONLY the HEATING shading selection.
                // Cooling-only shading must NOT change ShadingTypeId, otherwise GEffBase changes
                // and the heating g_eff is silently modified.
                _batch.ShadingTypeId = !string.IsNullOrEmpty(_batch.ShadingTypeIdHeat)
                    ? _batch.ShadingTypeIdHeat
                    : null;

                _batch.ShadingReductionFactor = ShadingModeHeatComboBox.SelectedIndex > 0
                    ? shadingFactorHeat_save
                    : 1.0;

                // 6. Препятствия (засенчване)
                if (_shadingConfigLocal != null)
                {
                    _batch.ShadingConfig = _shadingConfigLocal;
                    if (_shadingConfigLocal.FshDirMonthly != null && _shadingConfigLocal.FshDirMonthly.Length == 12)
                        _batch.FshDirMonthly = (double[])_shadingConfigLocal.FshDirMonthly.Clone();
                    else
                        _batch.FshDirMonthly = Enumerable.Repeat(1.0, 12).ToArray();
                }
                else
                {
                    _batch.ShadingConfig = null;
                    _batch.FshDirMonthly = Enumerable.Repeat(1.0, 12).ToArray();
                }

                // 7. Месечни g_eff стойности – използваме GEffBaseHeat (за heating reference, legacy поле)
                //    Всеки режим ползва собствен per-mode GEffBase за пълна независимост.
                double gEffBaseHeat_save = _batch.GEffBaseHeat;
                double gEffBaseCool_save = _batch.GEffBaseCool;
                _batch.GEffMonthly = new double[12];
                for (int m = 0; m < 12; m++)
                {
                    double F_sh_dir_m = _batch.FshDirMonthly[m];
                    _batch.GEffMonthly[m] = gEffBaseHeat_save * shadingFactorHeat_save * F_sh_dir_m;
                }

                // 8. Сезонни g_eff стойности (Heat / Cool) – всеки с отделен shading factor
                //    и собствен per-mode GEffBase → промяна на единия режим НЕ влияе на другия.
                WindowCalculator.TryGetHeatingMonths(_climateZone, _heatingSeasonEnabled, out List<int> heatingMonths);
                WindowCalculator.TryGetCoolingMonths(_coolingStartMonth, _coolingEndMonth, _coolingSeasonEnabled, out List<int> coolingMonths);

                _batch.GEffHeat = heatingMonths.Count > 0
                    ? heatingMonths.Average(m => gEffBaseHeat_save * shadingFactorHeat_save * _batch.FshDirMonthly[m])
                    : 0.0;

                _batch.GEffCool = coolingMonths.Count > 0
                    ? coolingMonths.Average(m => gEffBaseCool_save * shadingFactorCool_save * _batch.FshDirMonthly[m])
                    : 0.0;
            }

            // Генериране на TypeName
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

            if (_batch.Kind == WindowKind.Window)
            {
                s_lastProfileSystemId = _batch.ProfileSystemId;
                s_lastManualMountingDepthMm = _batch.ProfileMountingDepthMm;
                s_lastManualVisibleHeightMm = _batch.ProfileVisibleHeightMm;
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
