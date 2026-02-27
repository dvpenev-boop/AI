using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;

namespace EE.Doklad.Views
{
    /// <summary>
    /// Interaction logic for HotWaterSectionEditor.xaml
    /// </summary>
    public partial class HotWaterSectionEditor : UserControl
    {
        public HotWaterSectionEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Отваря прозореца „Методика (БГВ)" при клик върху бутона.
        /// Взима текущите данни от DataContext (HotWaterSectionData)
        /// и след потвърждаване записва резултата обратно.
        /// </summary>
        private void BtnMethodology_Click(object sender, RoutedEventArgs e)
        {
            // Получаваме моделните данни от DataContext
            var data = DataContext as HotWaterSectionData;

            double workingDays = data?.WorkingDaysPerYear ?? 251.0;
            // Температурата на БГВ: TemperatureDifference + 10°C (студена вода)
            // или директно 55°C по подразбиране
            double hotWaterTemp = data != null
                ? data.TemperatureDifference + 10.0   // ΔT + θ_cold (10°C assumed)
                : 55.0;

            var initialMode = data?.RecoverableLossMode ?? DhwLossMode.Manual;

            var window = new DhwLossMethodologyWindow(
                workingDaysPerYear: workingDays,
                hotWaterTemp:       hotWaterTemp,
                initialMode:        initialMode,
                onConfirm: (qRblYear, fRblPct) =>
                {
                    if (data is null) return;

                    // Записваме резултата обратно в модела
                    data.RecoverableHeatToZone_kWh = System.Math.Round(qRblYear, 2);
                    data.RecoverableFraction_pct    = System.Math.Round(fRblPct, 2);
                    data.RecoverableLossMode        = DhwLossMode.Automatic;
                });

            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
        }
    }
}
