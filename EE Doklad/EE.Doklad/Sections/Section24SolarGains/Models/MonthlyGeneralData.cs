using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EE.Doklad.Sections.Section24SolarGains.Models
{
    /// <summary>
    /// Общи месечни данни – приложими за всички елементи.
    /// </summary>
    public class MonthlyGeneralData : INotifyPropertyChanged
    {
        private static readonly string[] _monthNames =
        {
            "Яну", "Фев", "Мар", "Апр", "Май", "Юни",
            "Юли", "Авг", "Сеп", "Окт", "Ное", "Дек"
        };

        private int _monthIndex;
        private double _deltaTm;
        private double _deltaThetaSkyM;
        private int _heatingDays;
        private int _coolingDays;

        // ------------------------------------------------------------------ //

        /// <summary>Индекс на месеца 0–11.</summary>
        public int MonthIndex
        {
            get => _monthIndex;
            set { _monthIndex = value; OnPropertyChanged(); OnPropertyChanged(nameof(MonthName)); }
        }

        /// <summary>Кратко наименование на месеца (Яну … Дек).</summary>
        public string MonthName => _monthIndex >= 0 && _monthIndex < 12
            ? _monthNames[_monthIndex] : "?";

        /// <summary>
        /// Продължителност на месеца Δt_m, h.
        /// Януари = 744, Февруари = 672, …
        /// </summary>
        public double DeltaT_m
        {
            get => _deltaTm;
            set { _deltaTm = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Осреднена разлика между привидната температура на небето и температурата на въздуха
        /// Δθ_sky_m = 11 K (по подразбиране).
        /// </summary>
        public double DeltaTheta_sky_m
        {
            get => _deltaThetaSkyM;
            set { _deltaThetaSkyM = value; OnPropertyChanged(); }
        }

        /// <summary>Брой активни дни от отоплителен период за месеца.</summary>
        public int HeatingDays
        {
            get => _heatingDays;
            set { _heatingDays = value; OnPropertyChanged(); }
        }

        /// <summary>Брой активни дни от охладителен период за месеца.</summary>
        public int CoolingDays
        {
            get => _coolingDays;
            set { _coolingDays = value; OnPropertyChanged(); }
        }

        // ------------------------------------------------------------------ //

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
