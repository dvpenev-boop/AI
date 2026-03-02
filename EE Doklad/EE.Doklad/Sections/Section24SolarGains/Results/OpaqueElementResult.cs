namespace EE.Doklad.Sections.Section24SolarGains.Results
{
    /// <summary>
    /// Пълни резултати за един непрозрачен елемент за един месец (формула 3.38 + 3.39).
    /// Съдържа всички междинни стойности за проверка.
    /// </summary>
    public class OpaqueMonthlyResult
    {
        /// <summary>Индекс на месеца 0–11.</summary>
        public int MonthIndex { get; init; }

        /// <summary>Кратко наименование на месеца.</summary>
        public string MonthName { get; init; } = string.Empty;

        // ---------- Входни за месеца ----------

        /// <summary>Продължителност Δt_m, h.</summary>
        public double DeltaT_m { get; init; }

        /// <summary>Δθ_sky_m, K.</summary>
        public double DeltaTheta_sky_m { get; init; }

        // ---------- Параметри на елемента ----------

        /// <summary>Коефициент на поглъщане α_sol [-].</summary>
        public double Alpha_sol { get; init; }

        /// <summary>Топлинно съпротивление на外 повърхност R_se [m²·K/W].</summary>
        public double R_se { get; init; }

        /// <summary>Коефициент на топлопреминаване U_c [W/(m²·K)].</summary>
        public double U_c { get; init; }

        /// <summary>Площ A_c [m²].</summary>
        public double A_c { get; init; }

        /// <summary>Коефициент на засенчване F_sh_obst [-].</summary>
        public double F_sh_obst { get; init; }

        /// <summary>Слънчево облъчване H_sol [kWh/m²].</summary>
        public double H_sol { get; init; }

        /// <summary>
        /// Произведение: SolarFactorOpaque = α_sol * R_se * U_c * A_c * F_sh_obst * H_sol, kWh.
        /// Частта от формула (3.38) преди изваждане на Q_sky.
        /// </summary>
        public double SolarFactorOpaque { get; init; }

        // ---------- Дълговълново излъчване ----------

        /// <summary>
        /// h_lr = 4·ε·σ·(θ_ss + 273)³, W/(m²·K).
        /// </summary>
        public double H_lr { get; init; }

        /// <summary>
        /// Q_sky = 0.001 * F_sky * R_se * U_c * A_c * h_lr * Δθ_sky * Δt, kWh.
        /// Формула (3.39).
        /// </summary>
        public double Q_sky { get; init; }

        // ---------- Краен резултат ----------

        /// <summary>
        /// Q_sol_opaque = α_sol * R_se * U_c * A_c * F_sh_obst * H_sol – Q_sky, kWh.
        /// Формула (3.38).
        /// </summary>
        public double Q_sol_opaque { get; init; }
    }

    /// <summary>
    /// Обобщени резултати за един непрозрачен елемент (всички 12 месеца).
    /// </summary>
    public class OpaqueElementResult
    {
        /// <summary>Идентификатор на елемента.</summary>
        public string ElementId { get; init; } = string.Empty;

        /// <summary>Резултати за всеки от 12-те месеца.</summary>
        public OpaqueMonthlyResult[] MonthlyResults { get; init; } = new OpaqueMonthlyResult[12];

        /// <summary>Годишна сума Q_sol_opaque [kWh].</summary>
        public double AnnualQ_sol_opaque => MonthlyResults.Sum(r => r.Q_sol_opaque);

        /// <summary>Годишна Q_sky [kWh].</summary>
        public double AnnualQ_sky => MonthlyResults.Sum(r => r.Q_sky);
    }
}
