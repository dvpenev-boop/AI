namespace EE.Doklad.Sections.Section24SolarGains.Results
{
    /// <summary>
    /// Пълни резултати за един прозорец за един месец (формула 3.37 + 3.39).
    /// Съдържа всички междинни стойности за проверка.
    /// </summary>
    public class WindowMonthlyResult
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

        // ---------- Площи ----------

        /// <summary>
        /// Остъклена площ: A_gl = A_wi * (1 – F_fr), m².
        /// Формула (3.40).
        /// </summary>
        public double A_gl { get; init; }

        // ---------- Слънчеви параметри ----------

        /// <summary>Средномесечна ефективна сумарна пропускливост g_gl [-].</summary>
        public double G_gl { get; init; }

        /// <summary>Коефициент на засенчване F_sh_obst [-].</summary>
        public double F_sh_obst { get; init; }

        /// <summary>Слънчево облъчване H_sol [kWh/m²].</summary>
        public double H_sol { get; init; }

        /// <summary>
        /// Произведение: SolarFactor = g_gl * A_gl * F_sh_obst * H_sol, kWh.
        /// Частта от формула (3.37) преди изваждане на Q_sky.
        /// </summary>
        public double SolarFactor { get; init; }

        // ---------- Дълговълново излъчване ----------

        /// <summary>
        /// Коефициент на топлопренасяне при външно дълговълново излъчване:
        /// h_lr = 4·ε·σ·(θ_ss + 273)³, W/(m²·K).
        /// </summary>
        public double H_lr { get; init; }

        /// <summary>
        /// Топлинен поток към небето:
        /// Q_sky = 0.001 * F_sky * R_se * U_c * A_wi * h_lr * Δθ_sky * Δt, kWh.
        /// Формула (3.39).
        /// </summary>
        public double Q_sky { get; init; }

        // ---------- Краен резултат ----------

        /// <summary>
        /// Слънчеви топлинни печалби на прозореца за месеца:
        /// Q_sol_window = g_gl * A_gl * F_sh_obst * H_sol – Q_sky, kWh.
        /// Формула (3.37).
        /// </summary>
        public double Q_sol_window { get; init; }
    }

    /// <summary>
    /// Обобщени резултати за един прозорец (всички 12 месеца).
    /// </summary>
    public class WindowElementResult
    {
        /// <summary>Идентификатор на прозореца.</summary>
        public string ElementId { get; init; } = string.Empty;

        /// <summary>Резултати за всеки от 12-те месеца.</summary>
        public WindowMonthlyResult[] MonthlyResults { get; init; } = new WindowMonthlyResult[12];

        /// <summary>Годишна сума Q_sol_window [kWh].</summary>
        public double AnnualQ_sol_window => MonthlyResults.Sum(r => r.Q_sol_window);

        /// <summary>Годишна Q_sky [kWh].</summary>
        public double AnnualQ_sky => MonthlyResults.Sum(r => r.Q_sky);
    }
}
