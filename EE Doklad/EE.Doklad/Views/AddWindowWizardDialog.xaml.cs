using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Wizard dialog за добавяне/редакция на прозорец/врата
    /// </summary>
    public partial class AddWindowWizardDialog : Window
    {
        private WindowBatch _batch;
        private bool _isEditMode;
        private int _currentStep = 1;
        private const int TotalSteps = 6;

        // Shading options
        private List<ShadingOption> _allShadingOptions = new();
        private Dictionary<string, List<ShadingOption>> _shadingByCategory = new();

        // Obstacle profiles
        private List<ObstacleProfile> _obstacleProfiles = new();

        public WindowBatch Result => _batch;

        public AddWindowWizardDialog(WindowBatch? existingBatch = null)
        {
            InitializeComponent();

            _isEditMode = existingBatch != null;
            _batch = existingBatch ?? new WindowBatch();

            LoadObstacleProfiles();

            InitializeStep1();
            UpdateStepVisibility();
            UpdateNavigationButtons();
        }

        private void LoadObstacleProfiles()
        {
            _obstacleProfiles = WindowCalculator.GetObstacleProfiles();
            // Don't set ItemsSource here - will be done in InitializeStep6
        }

        #region Step 1: Основни данни

        private void InitializeStep1()
        {

            // Kind (с Description)
            KindComboBox.ItemsSource = Enum.GetValues(typeof(WindowKind))
                .Cast<WindowKind>()
                .Select(k => new { Value = k, Label = GetEnumDescription(k) })
                .ToList();
            KindComboBox.DisplayMemberPath = "Label";
            KindComboBox.SelectedValuePath = "Value";
            KindComboBox.SelectedValue = _batch.Kind;

            // Orientation (с Description)
            OrientationComboBox.ItemsSource = Enum.GetValues(typeof(ModelOrientation))
                .Cast<ModelOrientation>()
                .Select(o => new { Value = o, Label = GetEnumDescription(o) })
                .ToList();
            OrientationComboBox.DisplayMemberPath = "Label";
            OrientationComboBox.SelectedValuePath = "Value";
            OrientationComboBox.SelectedValue = _batch.Orientation;

            // Count
            CountTextBox.Text = _batch.Count.ToString();
        }

        #endregion

        #region Step 2: Геометрия

        private void InitializeStep2()
        {
            if (WidthTextBox == null || HeightTextBox == null) return;

            if (_batch.Width > 0 && _batch.Height > 0)
            {
                WidthTextBox.Text = (_batch.Width * 100.0).ToString("F0");
                HeightTextBox.Text = (_batch.Height * 100.0).ToString("F0");
            }

            UpdateCalculatedArea();
        }

        private void UpdateCalculatedArea()
        {
            if (CalculatedAreaTextBlock == null || WidthTextBox == null || HeightTextBox == null) return;

            if (TryParseDouble(WidthTextBox.Text, out double wCm) &&
                TryParseDouble(HeightTextBox.Text, out double hCm) &&
                wCm > 0 && hCm > 0)
            {
                // Конвертиране от см в м²
                double wM = wCm / 100.0;
                double hM = hCm / 100.0;
                double area = wM * hM;
                CalculatedAreaTextBlock.Text = $"Площ: {area:F3} m² (от {wCm:F0}×{hCm:F0} см)";
            }
            else
            {
                CalculatedAreaTextBlock.Text = "Площ: —";
            }
        }

        #endregion

        #region Step 3: Топлотехнически/оптични данни

        private void InitializeStep3()
        {
            if (CatalogRadio == null || UValueTextBox == null || GNTextBox == null || OpticalTypeComboBox == null) return;
            
            CatalogRadio.IsChecked = true;
            UValueTextBox.Text = _batch.UValue > 0 ? _batch.UValue.ToString("F2") : "1.40";
            GNTextBox.Text = _batch.GN > 0 ? _batch.GN.ToString("F3") : "0.750";

            OpticalTypeComboBox.ItemsSource = Enum.GetValues(typeof(OpticalType));
            OpticalTypeComboBox.SelectedItem = _batch.OpticalType;
        }

        #endregion

        #region Step 4: Рамка

        private void InitializeStep4()
        {
            if (FrameFractionTextBox == null) return;
            
            FrameFractionTextBox.Text = (_batch.FrameFraction * 100).ToString("F1");
            UpdateFrameCalculation();
        }

        private void FrameFractionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFrameCalculation();
        }

        private void UpdateFrameCalculation()
        {
            if (FrameAreaGlassTextBlock == null) return;

            double aGross = GetCurrentAreaGross();
            if (TryParseDouble(FrameFractionTextBox.Text, out double ffrPct) &&
                ffrPct >= 0 && ffrPct < 50 && aGross > 0)
            {
                double ffr = ffrPct / 100.0;
                double aGlass = WindowCalculator.CalculateAreaGlass(aGross, ffr);
                FrameAreaGlassTextBlock.Text = $"A_стъкло = {aGlass:F3} m²";
            }
            else
            {
                FrameAreaGlassTextBlock.Text = "—";
            }
        }

        private double GetCurrentAreaGross()
        {
            if (TryParseDouble(WidthTextBox?.Text, out double wCm) &&
                TryParseDouble(HeightTextBox?.Text, out double hCm) &&
                wCm > 0 && hCm > 0)
            {
                // width/height are entered in cm in the wizard; return area in m²
                double wM = wCm / 100.0;
                double hM = hCm / 100.0;
                return wM * hM;
            }
            return 0;
        }

        #endregion

        #region Step 5: Слънцезащита

        private void InitializeStep5()
        {
            if (ShadingModeComboBox == null) return; // Safety check

            // Default: none if no saved shading type, otherwise internal
            ShadingModeComboBox.SelectedIndex = string.IsNullOrEmpty(_batch.ShadingTypeId) ? 0 : 1;
            ShadingModeComboBox.SelectionChanged += (s, e) => UpdateShadingUI();

            // Populate category combo
            ShadingCategoryComboBox.ItemsSource = _shadingByCategory.Keys;
            if (_shadingByCategory.Keys.Count > 0)
            {
                ShadingCategoryComboBox.SelectedIndex = 0;
            }

            UpdateShadingUI();
        }

        // kept for compatibility (not used now)
        private void ShadingRadio_Checked(object sender, RoutedEventArgs e) { }

        private void ShadingCategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ShadingCategoryComboBox.SelectedItem is string category)
            {
                ShadingOptionsDataGrid.ItemsSource = _shadingByCategory[category];
                if (_shadingByCategory[category].Count > 0)
                {
                    ShadingOptionsDataGrid.SelectedIndex = 0;
                }
            }
        }

        private void ShadingOptionsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateShadingPreview();
        }

        private void UpdateShadingUI()
        {
            if (ShadingModeComboBox == null || ShadingCategoryComboBox == null || ShadingOptionsDataGrid == null) return;

            bool hasShading = ShadingModeComboBox.SelectedIndex == 1 || ShadingModeComboBox.SelectedIndex == 2;
            ShadingCategoryComboBox.IsEnabled = hasShading;
            ShadingOptionsDataGrid.IsEnabled = hasShading;

            UpdateShadingPreview();
        }

        private void UpdateShadingPreview()
        {
            if (ShadingPreviewTextBlock == null || ShadingModeComboBox == null) return;

            if (ShadingModeComboBox.SelectedIndex == 0)
            {
                ShadingPreviewTextBlock.Text = "Избран коефициент: 1.00 (без щора)";
                return;
            }

            if (ShadingOptionsDataGrid?.SelectedItem is ShadingOption option)
            {
                double factor = ShadingModeComboBox.SelectedIndex == 1 ? option.FShadeInt : option.FShadeExt;
                string location = ShadingModeComboBox.SelectedIndex == 1 ? "вътрешна" : "външна";
                ShadingPreviewTextBlock.Text = $"Избран коефициент: {factor:F2} ({location})";
            }
            else
            {
                ShadingPreviewTextBlock.Text = "—";
            }
        }

        #endregion

        #region Step 6: Засенчване

        private void InitializeStep6()
        {
            LoadStep6_Shading();
        }

        private void LoadStep6_Shading()
        {
            if (_batch == null) return;

            // Проверяваме дали има засенчване
            if (_batch.HasShading)
            {
                WithShadingRadio.IsChecked = true;
            }
            else
            {
                NoShadingRadio.IsChecked = true;
            }

            UpdateShadingSummaryUI();
        }

        private void NoShadingRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (_batch == null) return;
            
            // Изчистваме засенчването
            _batch.ShadingConfig = null;
            _batch.FshDirMonthly = new double[12] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            
            UpdateShadingSummaryUI();
        }

        private void WithShadingRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (_batch == null) return;
            
            // Отваряме диалога за засенчване
            OpenShadingEditor();
        }

        private void EditShadingButton_Click(object sender, RoutedEventArgs e)
        {
            OpenShadingEditor();
        }

        private void ClearShadingButton_Click(object sender, RoutedEventArgs e)
        {
            if (_batch == null) return;
            
            // Премахваме засенчването
            _batch.ShadingConfig = null;
            _batch.FshDirMonthly = new double[12] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            
            // Връщаме радио бутона към "Без засенчване"
            NoShadingRadio.IsChecked = true;
            
            UpdateShadingSummaryUI();
        }

        private void OpenShadingEditor()
        {
            if (_batch == null) return;
            // Вземи текущите размери: предпочитай стойностите от текстовите полета (в см), иначе използвай _batch (в m)
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
                MessageBox.Show("Моля, първо въведете валидна ширина и височина в Стъпка 2.", "Грешка", MessageBoxButton.OK, MessageBoxImage.Warning);
                NoShadingRadio.IsChecked = true;
                return;
            }

            var dialog = new ShadingEditorDialog(wk, hk, _batch.Orientation, _batch.ShadingConfig)
            {
                Owner = this
            };

            if (dialog.ShowDialog() == true)
            {
                // Запази конфигурацията от диалога
                _batch.ShadingConfig = dialog.Result;
                _batch.FshDirMonthly = dialog.Result.FshDirMonthly;
                UpdateShadingSummaryUI();
            }
            else
            {
                // При Cancel, ако няма конфигурация, връщаме към "Без засенчване"
                if (_batch.ShadingConfig == null || _batch.ShadingConfig.Shadings.Count == 0)
                {
                    NoShadingRadio.IsChecked = true;
                }
            }
        }

        private void UpdateShadingSummaryUI()
        {
            if (_batch == null) return;

            bool hasShading = _batch.HasShading;

            // Показваме/скриваме summary панела
            if (ShadingSummaryPanel != null)
            {
                ShadingSummaryPanel.Visibility = hasShading ? Visibility.Visible : Visibility.Collapsed;
            }

            if (hasShading && _batch.FshDirMonthly != null)
            {
                double min = _batch.FshDirMonthly.Min();
                double avg = _batch.FshDirMonthly.Average();

                if (FshMinText != null)
                    FshMinText.Text = min.ToString("F3");

                if (FshAvgText != null)
                    FshAvgText.Text = avg.ToString("F3");
            }
        }

        #endregion

        #region Navigation

        private void UpdateStepVisibility()
        {
            Step1Panel.Visibility = _currentStep == 1 ? Visibility.Visible : Visibility.Collapsed;
            Step2Panel.Visibility = _currentStep == 2 ? Visibility.Visible : Visibility.Collapsed;
            Step3Panel.Visibility = _currentStep == 3 ? Visibility.Visible : Visibility.Collapsed;
            Step4Panel.Visibility = _currentStep == 4 ? Visibility.Visible : Visibility.Collapsed;
            Step5Panel.Visibility = _currentStep == 5 ? Visibility.Visible : Visibility.Collapsed;
            Step6Panel.Visibility = _currentStep == 6 ? Visibility.Visible : Visibility.Collapsed;

            StepIndicatorTextBlock.Text = $"Стъпка {_currentStep} от {TotalSteps}";

            // Initialize current step
            switch (_currentStep)
            {
                case 2: InitializeStep2(); break;
                case 3: InitializeStep3(); break;
                case 4: InitializeStep4(); break;
                case 5: InitializeStep5(); break;
                case 6: InitializeStep6(); break;
            }
        }

        private void UpdateNavigationButtons()
        {
            BackButton.IsEnabled = _currentStep > 1;
            NextButton.Content = _currentStep < TotalSteps ? "Напред >" : "Завърши";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentStep > 1)
            {
                _currentStep--;
                UpdateStepVisibility();
                UpdateNavigationButtons();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateCurrentStep())
            {
                return; // Validation message already shown in ValidateCurrentStep
            }

            if (_currentStep < TotalSteps)
            {
                SaveCurrentStep();
                _currentStep++;
                UpdateStepVisibility();
                UpdateNavigationButtons();
            }
            else
            {
                // Finish
                SaveCurrentStep();
                SaveAllData();
                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateCurrentStep()
        {
            switch (_currentStep)
            {
                case 1:
                    if (!int.TryParse(CountTextBox?.Text, out int count) || count < 1)
                    {
                        MessageBox.Show("Брой трябва да е поне 1.", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    return true;
                    
                case 2:
                    if (!TryParseDouble(WidthTextBox?.Text, out double w) || w <= 0)
                    {
                        MessageBox.Show("Ширина трябва да е положително число.", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    if (!TryParseDouble(HeightTextBox?.Text, out double h) || h <= 0)
                    {
                        MessageBox.Show("Височина трябва да е положително число.", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    return true;
                    
                case 3:
                    if (!TryParseDouble(UValueTextBox?.Text, out double u) || u <= 0 || u > 10)
                    {
                        MessageBox.Show("U-стойност трябва да е между 0 и 10 W/m²K.", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    if (!TryParseDouble(GNTextBox?.Text, out double g) || g < 0 || g > 1)
                    {
                        MessageBox.Show("g_n трябва да е между 0 и 1.", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    if (OpticalTypeComboBox?.SelectedItem == null)
                    {
                        MessageBox.Show("Моля, изберете оптичен тип.", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    return true;
                    
                case 4:
                    if (!TryParseDouble(FrameFractionTextBox?.Text, out double ffr) || ffr < 0)
                    {
                        MessageBox.Show("F_fr трябва да е положително число.", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    
                    // Ограничение 10-20% само за прозорци, врати могат до 100%
                    if (_batch.Kind == WindowKind.Window && (ffr < 10 || ffr > 20))
                    {
                        MessageBox.Show("F_fr за прозорци трябва да е между 10% и 20%.", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    
                    if (ffr > 100)
                    {
                        MessageBox.Show("F_fr не може да надвишава 100%.", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    
                    return true;
                    
                default:
                    return true;
            }
        }

        private void SaveCurrentStep()
        {
            switch (_currentStep)
            {
                case 1:
                    _batch.Kind = (WindowKind)KindComboBox.SelectedValue;
                    _batch.Orientation = (ModelOrientation)OrientationComboBox.SelectedValue;
                    _batch.Count = int.Parse(CountTextBox.Text);
                    break;
                case 2:
                        // Конвертиране от см в м (винаги ширина×височина)
                        TryParseDouble(WidthTextBox.Text, out double widthCm2);
                        TryParseDouble(HeightTextBox.Text, out double heightCm2);
                        _batch.Width = widthCm2 / 100.0;  // см → м
                        _batch.Height = heightCm2 / 100.0; // см → м
                        _batch.AreaGross = _batch.Width * _batch.Height;
                    break;
                case 3:
                    TryParseDouble(UValueTextBox.Text, out double u);
                    TryParseDouble(GNTextBox.Text, out double g);
                    _batch.UValue = u;
                    _batch.GN = g;
                    _batch.OpticalType = (OpticalType)OpticalTypeComboBox.SelectedItem;
                    break;
                case 4:
                    TryParseDouble(FrameFractionTextBox.Text, out double ffr);
                    _batch.FrameFraction = ffr / 100.0;
                    break;
                case 5:
                    if (ShadingModeComboBox.SelectedIndex == 0)
                    {
                        _batch.ShadingTypeId = null;
                        _batch.ShadingReductionFactor = 1.0;
                    }
                    else if (ShadingOptionsDataGrid.SelectedItem is ShadingOption option)
                    {
                        _batch.ShadingTypeId = option.Id;
                        _batch.ShadingReductionFactor = ShadingModeComboBox.SelectedIndex == 1
                            ? option.FShadeInt
                            : option.FShadeExt;
                    }
                    break;
                case 6:
                    // Засенчването се записва директно в OpenShadingDialog, няма нужда от допълнителен код тук
                    break;
            }

        }

        private static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi != null && Attribute.GetCustomAttribute(fi, typeof(System.ComponentModel.DescriptionAttribute)) is System.ComponentModel.DescriptionAttribute attr)
                return attr.Description;
            return value.ToString();
        
        }

        private void SaveAllData()
        {
            // Generate TypeName if empty
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
        }

        private string GetKindLabel(WindowKind kind)
        {
            return kind == WindowKind.Window ? "Прозорец" : "Врата";
        }

        private bool TryParseDouble(string? text, out double result)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(text))
                return false;
            // Try current culture first (accepts comma in many locales), then invariant (dot)
            if (double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out result)) return true;
            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out result)) return true;
            // Fallback: replace comma with dot and try invariant
            var alt = text.Replace(',', '.');
            return double.TryParse(alt, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        }

        #endregion
    }
}
