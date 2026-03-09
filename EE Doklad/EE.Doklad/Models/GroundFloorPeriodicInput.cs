namespace EE.Doklad.Models
{
    /// <summary>
    /// Входни данни за изчисление на периодични коефициенти.
    /// Приема резултати от съществуващите стратегии + климатични данни.
    /// </summary>
    public class GroundFloorPeriodicInput
    {
        /// <summary>
        /// Тип подова конструкция.
        /// </summary>
        public FloorType FloorType { get; set; }

        /// <summary>
        /// Площ A (m²).
        /// </summary>
        public double Area { get; set; }

        /// <summary>
        /// Изложен периметър P (m).
        /// </summary>
        public double ExposedPerimeter { get; set; }

        /// <summary>
        /// Топлопроводност на земята lambda_g (W/(m·K)).
        /// </summary>
        public double LambdaGround { get; set; }

        /// <summary>
        /// Еквивалентна дебелина df от съществуващата стратегия (m).
        /// </summary>
        public double df { get; set; }

        /// <summary>
        /// U от съществуващата стратегия (W/(m²·K)).
        /// </summary>
        public double Ufg { get; set; }

        /// <summary>
        /// Hg стационарен от съществуващата стратегия (W/K).
        /// </summary>
        public double Hg_steady { get; set; }

        /// <summary>
        /// Тип изолация за под към земя.
        /// </summary>
        public GroundInsulationType InsulationType { get; set; }

        /// <summary>
        /// Еквивалентна дебелина за земята под crawl space dg (m).
        /// </summary>
        public double dg { get; set; }

        /// <summary>
        /// U на пода към crawl space Uf_sus (W/(m²·K)).
        /// </summary>
        public double Uf_sus { get; set; }

        /// <summary>
        /// Вентилационен еквивалентен U Ux (W/(m²·K)).
        /// </summary>
        public double Ux { get; set; }

        /// <summary>
        /// Дълбочина на сутерена z (m).
        /// </summary>
        public double BasementDepth { get; set; }

        /// <summary>
        /// Еквивалентна дебелина на стените dw_b (m).
        /// </summary>
        public double dw_b { get; set; }

        /// <summary>
        /// Месечни външни температури (12 стойности, °C), индекс 0=януари.
        /// </summary>
        public double[] MonthlyExteriorTemperature { get; set; } = new double[12];

        /// <summary>
        /// Годишна средна външна температура theta_e_bar (°C).
        /// </summary>
        public double AnnualMeanExteriorTemperature { get; set; }

        /// <summary>
        /// Амплитуда на външната температура theta_e_hat = (max-min)/2 (K).
        /// </summary>
        public double ExteriorTemperatureAmplitude { get; set; }

        /// <summary>
        /// Най-студен месец tau (1-базиран, обикновено 1).
        /// </summary>
        public int ColdestMonth { get; set; }

        /// <summary>
        /// Обемна топлоемност на почвата rho_c (J/(m³·K)).
        /// Категория 1 Clay/Silt: 3.0e6.
        /// Категория 2 Sand/Gravel: 2.0e6 (default).
        /// Категория 3 Homogeneous Rock: 2.0e6.
        /// </summary>
        public double RhoC { get; set; } = 2.0e6;
    }
}
