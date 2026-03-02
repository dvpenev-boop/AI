using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EE.Doklad.Sections.Section24SolarGains.Models
{
    /// <summary>
    /// Прозрачен елемент (прозорец) – входни данни за изчисление по формула (3.37).
    /// </summary>
    public class WindowElement : INotifyPropertyChanged
    {
        private string _id = string.Empty;
        private double _aWi;
        private double _fFr;
        private double _uC;
        private double _rSe;
        private double _fSky;
        private double _epsilon;
        private double _thetaSs = 10.0;
        private double[] _hSol  = new double[12];
        private double[] _fShObst = new double[12];
        private double[] _gGl    = new double[12];

        // ------------------------------------------------------------------ //

        /// <summary>Идентификатор на елемента (напр. "W1", "Прозорец Юг").</summary>
        public string Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Площ на прозорния елемент wi (проекция), m².
        /// Включва рамка + стъкло.
        /// </summary>
        public double A_wi
        {
            get => _aWi;
            set { _aWi = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Дял на площта на рамката: F_fr = площ_рамка / A_wi  [-].
        /// Типична стойност: 0.20 – 0.35.
        /// </summary>
        public double F_fr
        {
            get => _fFr;
            set { _fFr = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Коефициент на топлопреминаване на прозореца, W/(m²·K).
        /// Използва се за изчисление на Q_sky.
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
        /// Хоризонтален незасенчен покрив = 1.0; Вертикална стена = 0.5.
        /// </summary>
        public double F_sky
        {
            get => _fSky;
            set { _fSky = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Степен на чернота на повърхността ε [-].
        /// Обикновено 0.9 за стъкло.
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
        /// Месечно слънчево облъчване на всяка площ на елемента H_sol [kWh/m²], 12 стойности.
        /// Зависи от ориентация (β, γ) и климатичен район.
        /// </summary>
        public double[] H_sol
        {
            get => _hSol;
            set { _hSol = value ?? new double[12]; OnPropertyChanged(); }
        }

        /// <summary>
        /// Месечен безразмерен коефициент на намаляване на засенчването F_sh_obst [-], 12 стойности.
        /// </summary>
        public double[] F_sh_obst
        {
            get => _fShObst;
            set { _fShObst = value ?? new double[12]; OnPropertyChanged(); }
        }

        /// <summary>
        /// Безразмерна средномесечна ефективна сумарна пропускливост за слънчева енергия
        /// g_gl [-], 12 стойности. Вж. формули (3.41)/(3.42) за определяне.
        /// </summary>
        public double[] G_gl
        {
            get => _gGl;
            set { _gGl = value ?? new double[12]; OnPropertyChanged(); }
        }

        // ------------------------------------------------------------------ //

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
