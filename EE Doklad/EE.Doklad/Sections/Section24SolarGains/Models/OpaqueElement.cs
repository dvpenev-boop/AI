using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EE.Doklad.Sections.Section24SolarGains.Models
{
    /// <summary>
    /// Непрозрачен елемент на обвивката – входни данни за изчисление по формула (3.38).
    /// </summary>
    public class OpaqueElement : INotifyPropertyChanged
    {
        private string _id = string.Empty;
        private double _aC;
        private double _alphaSol;
        private double _uC;
        private double _rSe;
        private double _fSky;
        private double _epsilon;
        private double _thetaSs = 10.0;
        private double[] _hSol    = new double[12];
        private double[] _fShObst = new double[12];

        // ------------------------------------------------------------------ //

        /// <summary>Идентификатор на елемента (напр. "OP1", "Южна стена").</summary>
        public string Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Площ на елемента/проекцията A_c, m².
        /// При издадени компоненти се използва площта на проекцията.
        /// </summary>
        public double A_c
        {
            get => _aC;
            set { _aC = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Безразмерен коефициент на поглъщане на слънчева енергия α_sol [-].
        /// Светъл цвят=0.3, среден=0.6, тъмен=0.9 (Таблица 1).
        /// </summary>
        public double Alpha_sol
        {
            get => _alphaSol;
            set { _alphaSol = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Коефициент на топлопреминаване U_c, W/(m²·K).
        /// </summary>
        public double U_c
        {
            get => _uC;
            set { _uC = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Топлинно съпротивление на външна повърхност R_se = 1/(h_ce + h_re), m²·K/W.
        /// Типична стойност: 0.13 m²·K/W.
        /// </summary>
        public double R_se
        {
            get => _rSe;
            set { _rSe = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Коефициент на видимост между елемента и небето F_sky [-].
        /// Хоризонтален покрив = 1.0; Вертикална стена = 0.5.
        /// </summary>
        public double F_sky
        {
            get => _fSky;
            set { _fSky = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Степен на чернота на повърхността ε [-].
        /// </summary>
        public double Epsilon
        {
            get => _epsilon;
            set { _epsilon = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Средна аритметична стойност на температурата на повърхността θ_ss, °C.
        /// При липса на данни се приема 10 °C.
        /// </summary>
        public double ThetaSs
        {
            get => _thetaSs;
            set { _thetaSs = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Месечно слънчево облъчване H_sol [kWh/m²], 12 стойности.
        /// </summary>
        public double[] H_sol
        {
            get => _hSol;
            set { _hSol = value ?? new double[12]; OnPropertyChanged(); }
        }

        /// <summary>
        /// Месечен коефициент на намаляване на засенчването F_sh_obst [-], 12 стойности.
        /// </summary>
        public double[] F_sh_obst
        {
            get => _fShObst;
            set { _fShObst = value ?? new double[12]; OnPropertyChanged(); }
        }

        // ------------------------------------------------------------------ //

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
