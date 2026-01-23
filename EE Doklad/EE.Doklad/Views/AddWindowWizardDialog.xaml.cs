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

            LoadShadingOptions();
            LoadObstacleProfiles();

            InitializeStep1();
            UpdateStepVisibility();
            UpdateNavigationButtons();
        }

        private void LoadShadingOptions()
        {
            _allShadingOptions = WindowCalculator.GetShadingOptions();
            _shadingByCategory = WindowCalculator.GetShadingOptionsByCategory();
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
            if (GeometryWidthHeightRadio == null || WidthTextBox == null || HeightTextBox == null || 
                GeometryAreaRadio == null || AreaGrossTextBox == null) return;
            
            if (_batch.Width > 0 && _batch.Height > 0)
            {
                GeometryWidthHeightRadio.IsChecked = true;
                WidthTextBox.Text = _batch.Width.ToString("F2");
                HeightTextBox.Text = _batch.Height.ToString("F2");
            }
            else if (_batch.AreaGross > 0)
            {
                GeometryAreaRadio.IsChecked = true;
                AreaGrossTextBox.Text = _batch.AreaGross.ToString("F2");
            }
            else
            {
                GeometryWidthHeightRadio.IsChecked = true;
            }

            UpdateGeometryFields();
        }

        private void GeometryRadio_Checked(object sender, RoutedEventArgs e)
        {
            UpdateGeometryFields();
        }

        private void UpdateGeometryFields()
        {
            if (GeometryWidthHeightRadio == null || WidthTextBox == null || HeightTextBox == null || AreaGrossTextBox == null) return;

            bool useWidthHeight = GeometryWidthHeightRadio.IsChecked == true;
            WidthTextBox.IsEnabled = useWidthHeight;
            HeightTextBox.IsEnabled = useWidthHeight;
            AreaGrossTextBox.IsEnabled = !useWidthHeight;

            UpdateCalculatedArea();
        }

        private void GeometryTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateCalculatedArea();
        }

        private void UpdateCalculatedArea()
        {
            if (CalculatedAreaTextBlock == null || GeometryWidthHeightRadio == null || 
                WidthTextBox == null || HeightTextBox == null) return;

            if (GeometryWidthHeightRadio.IsChecked == true)
            {
                if (double.TryParse(WidthTextBox.Text, out double wCm) &&
                    double.TryParse(HeightTextBox.Text, out double hCm) &&
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
            else
            {
                CalculatedAreaTextBlock.Text = "";
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
            if (double.TryParse(FrameFractionTextBox.Text, out double ffrPct) &&
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
            if (GeometryWidthHeightRadio?.IsChecked == true)
            {
                if (double.TryParse(WidthTextBox?.Text, out double w) &&
                    double.TryParse(HeightTextBox?.Text, out double h))
                {
                    return w * h;
                }
            }
            else if (double.TryParse(AreaGrossTextBox?.Text, out double a))
            {
                return a;
            }
            return 0;
        }

        #endregion

        #region Step 5: Слънцезащита

        private void InitializeStep5()
        {
            if (ShadingNoneRadio == null) return; // Safety check
            
            ShadingNoneRadio.IsChecked = string.IsNullOrEmpty(_batch.ShadingTypeId);
            ShadingInternalRadio.IsChecked = !string.IsNullOrEmpty(_batch.ShadingTypeId);

            // Populate category combo
            ShadingCategoryComboBox.ItemsSource = _shadingByCategory.Keys;
            if (_shadingByCategory.Keys.Count > 0)
            {
                ShadingCategoryComboBox.SelectedIndex = 0;
            }

            UpdateShadingUI();
        }

        private void ShadingRadio_Checked(object sender, RoutedEventArgs e)
        {
            UpdateShadingUI();
        }

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
            if (ShadingNoneRadio == null || ShadingCategoryComboBox == null || ShadingOptionsDataGrid == null) return;

            bool hasShading = ShadingInternalRadio.IsChecked == true || ShadingExternalRadio.IsChecked == true;
            ShadingCategoryComboBox.IsEnabled = hasShading;
            ShadingOptionsDataGrid.IsEnabled = hasShading;

            UpdateShadingPreview();
        }

        private void UpdateShadingPreview()
        {
            if (ShadingPreviewTextBlock == null || ShadingNoneRadio == null) return;

            if (ShadingNoneRadio.IsChecked == true)
            {
                ShadingPreviewTextBlock.Text = "Избран коефициент: 1.00 (без щора)";
                return;
            }

            if (ShadingOptionsDataGrid?.SelectedItem is ShadingOption option && 
                (ShadingInternalRadio != null && ShadingExternalRadio != null))
            {
                double factor = ShadingInternalRadio.IsChecked == true ? option.FShadeInt : option.FShadeExt;
                string location = ShadingInternalRadio.IsChecked == true ? "вътрешна" : "външна";
                ShadingPreviewTextBlock.Text = $"Избран коефициент: {factor:F2} ({location})";
            }
            else
            {
                ShadingPreviewTextBlock.Text = "—";
            }
        }

        #endregion

        #region Step 6: Препятствия

        private void InitializeStep6()
        {
            // Initialize obstacle profiles
            if (ObstacleProfileComboBox.ItemsSource == null)
            {
                ObstacleProfileComboBox.ItemsSource = _obstacleProfiles;
                ObstacleProfileComboBox.DisplayMemberPath = "Name";
                ObstacleProfileComboBox.SelectedIndex = 0;
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
                    if (GeometryWidthHeightRadio?.IsChecked == true)
                    {
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
                    }
                    else
                    {
                        if (!TryParseDouble(AreaGrossTextBox?.Text, out double a) || a <= 0)
                        {
                            MessageBox.Show("Площ трябва да е положително число.", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }
                        return true;
                    }
                    
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
                    if (GeometryWidthHeightRadio.IsChecked == true)
                    {
                        // Конвертиране от см в м
                        TryParseDouble(WidthTextBox.Text, out double widthCm);
                        TryParseDouble(HeightTextBox.Text, out double heightCm);
                        _batch.Width = widthCm / 100.0;  // см → м
                        _batch.Height = heightCm / 100.0; // см → м
                        _batch.AreaGross = _batch.Width * _batch.Height;
                    }
                    else
                    {
                        TryParseDouble(AreaGrossTextBox.Text, out double area);
                        _batch.AreaGross = area;
                        _batch.Width = 0;
                        _batch.Height = 0;
                    }
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
                    break;
                case 6:
                    if (ObstacleProfileComboBox.SelectedItem is ObstacleProfile profile)
                    {
                        _batch.ObstacleProfileId = profile.Id;
                        _batch.MonthlyObstacleFactors = (double[])profile.MonthlyFactors.Clone();
                    }
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
            
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        }

        #endregion
    }
}
