namespace EE.Doklad.Models
{
    /// <summary>
    /// Периодични коефициенти на топлопренасяне за подови конструкции
    /// в контакт със земята по ISO 13370.
    /// </summary>
    public class GroundFloorPeriodicResult
    {
        /// <summary>
        /// Стационарен коефициент на топлопренасяне W/K.
        /// За slab-on-ground: Hg = A*Ufg + P*Psi_wf.
        /// За heated basement: Hg = A*Ufg_b + z*P*Uwg_b + P*Psi_wf.
        /// За unheated basement: Hg = A*Uub.
        /// </summary>
        public double Hg { get; set; }

        /// <summary>
        /// Елементарен принос W/K = A * Ufg.
        /// Без периметровия топлинен мост.
        /// </summary>
        public double Hel { get; set; }

        /// <summary>
        /// Вътрешен периодичен коефициент W/K - ISO 13370 Eq.(44).
        /// Свързан с колебанията на вътрешната температура.
        /// При постоянна вътрешна температура влиянието е нулево.
        /// </summary>
        public double Hpi { get; set; }

        /// <summary>
        /// Външен периодичен коефициент W/K - ISO 13370 Eq.(45).
        /// Свързан с колебанията на външната температура.
        /// Отчита термичната инерция на земята - забавен с Beta месеца.
        /// </summary>
        public double Hpe { get; set; }

        /// <summary>
        /// Дълбочина на периодично проникване m - ISO 13370 Eq.(43).
        /// delta = sqrt(3.15e7 * lambda_g / (pi * rho_c)).
        /// </summary>
        public double Delta { get; set; }

        /// <summary>
        /// Фазово изоставане в месеци - ISO 13370 Table 4.
        /// 0 = suspended floor.
        /// 1 = slab-on-ground без/с хоризонтална изолация, heated/unheated basement.
        /// 2 = slab-on-ground с вертикална или външна изолация.
        /// </summary>
        public int Beta { get; set; }

        /// <summary>
        /// Месечни еквивалентни коефициенти на топлопренасяне W/K.
        /// Индекс 0 = януари, 11 = декември.
        /// Използване: Q_m = Hmonthly[m] * (theta_int - theta_e_m) * hours_m.
        /// Вече включва корекция за термична инерция на земята.
        /// </summary>
        public double[] Hmonthly { get; set; } = new double[12];

        /// <summary>
        /// Характеристичен размер m.
        /// </summary>
        public double B { get; set; }

        /// <summary>
        /// Еквивалентна дебелина m.
        /// </summary>
        public double df { get; set; }

        /// <summary>
        /// U-стойност на пода W/(m²·K).
        /// </summary>
        public double Ufg { get; set; }
    }
}
