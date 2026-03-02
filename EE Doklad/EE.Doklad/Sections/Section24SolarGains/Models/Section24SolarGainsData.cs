using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EE.Doklad.Sections.Section24SolarGains.Models
{
    /// <summary>
    /// Входни данни за Секция 24 – топлинни печалби от слънчево греене.
    /// Съдържа списъци с елементи и масив от общи месечни данни.
    /// </summary>
    public class Section24SolarGainsData : INotifyPropertyChanged
    {
        private string _description = "24. Топлинни печалби от слънчево греене";

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        /// <summary>Прозрачни елементи (прозорци).</summary>
        public ObservableCollection<WindowElement> Windows { get; } = [];

        /// <summary>Непрозрачни елементи на обвивката.</summary>
        public ObservableCollection<OpaqueElement> OpaqueElements { get; } = [];

        /// <summary>
        /// Общи месечни параметри: Δt_m и Δθ_sky_m – масив от 12 реда.
        /// </summary>
        public MonthlyGeneralData[] MonthlyData { get; } = CreateDefaultMonthlyData();

        // ------------------------------------------------------------------ //

        private static MonthlyGeneralData[] CreateDefaultMonthlyData()
        {
            // Стандартни продължителности на месеците в часове
            double[] deltaTm = { 744, 672, 744, 720, 744, 720, 744, 744, 720, 744, 720, 744 };

            return Enumerable.Range(0, 12).Select(i => new MonthlyGeneralData
            {
                MonthIndex = i,
                DeltaT_m = deltaTm[i],
                DeltaTheta_sky_m = 11.0  // стойност по подразбиране (3.39)
            }).ToArray();
        }

        // ------------------------------------------------------------------ //

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
