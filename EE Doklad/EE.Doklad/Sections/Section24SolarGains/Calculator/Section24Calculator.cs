using EE.Doklad.Sections.Section24SolarGains.Models;
using EE.Doklad.Sections.Section24SolarGains.Results;

namespace EE.Doklad.Sections.Section24SolarGains.Calculator
{
    /// <summary>
    /// Изчислител на топлинни печалби от слънчево греене – Секция 24.
    /// <para>
    /// Имплементирани формули:
    /// <list type="bullet">
    ///   <item>(3.36) Q_sol_total = Σ Q_sol_window + Σ Q_sol_opaque</item>
    ///   <item>(3.37) Q_sol_window = g_gl * A_gl * (1–F_fr) * F_sh_obst * H_sol – Q_sky</item>
    ///   <item>(3.38) Q_sol_opaque = α_sol * R_se * U_c * A_c * F_sh_obst * H_sol – Q_sky</item>
    ///   <item>(3.39) Q_sky = 0.001 * F_sky * R_se * U_c * A_k * h_lr * Δθ_sky * Δt</item>
    ///   <item>h_lr = 4·ε·σ·(θ_ss + 273)³</item>
    /// </list>
    /// </para>
    /// <para>
    /// НЕ се имплементира логиката по т.3.7.4 – входните параметри (g_gl, g_gl,n, F_w, щори, …)
    /// се задават директно от потребителя.
    /// </para>
    /// </summary>
    public static class Section24Calculator
    {
        /// <summary>
        /// Константа на Стефан–Болцман σ = 5.670374419×10⁻⁸ W/(m²·K⁴).
        /// </summary>
        public const double StefanBoltzmann = 5.670374419e-8;

        // ================================================================== //
        //  ГЛАВЕН МЕТОД
        // ================================================================== //

        /// <summary>
        /// Изчислява пълните резултати за Секция 24.
        /// </summary>
        /// <param name="data">Входни данни (прозорци, непрозрачни, месечни параметри).</param>
        /// <returns>Структуриран обект с всички междинни и крайни резултати.</returns>
        public static Section24Results Calculate(Section24SolarGainsData data)
        {
            ArgumentNullException.ThrowIfNull(data);

            var windowResults = data.Windows
                .Select(w => CalculateWindow(w, data.MonthlyData))
                .ToArray();

            var opaqueResults = data.OpaqueElements
                .Select(o => CalculateOpaque(o, data.MonthlyData))
                .ToArray();

            var monthlyTotals = BuildMonthlyTotals(data.Windows, data.OpaqueElements, data.MonthlyData);

            return new Section24Results
            {
                WindowResults  = windowResults,
                OpaqueResults  = opaqueResults,
                MonthlyTotals  = monthlyTotals
            };
        }

        // ================================================================== //
        //  ПРОЗОРЦИ
        // ================================================================== //

        /// <summary>
        /// Изчислява слънчевите топлинни печалби за един прозрачен елемент (формула 3.37).
        /// </summary>
        public static WindowElementResult CalculateWindow(
            WindowElement win,
            MonthlyGeneralData[] monthly)
        {
            ArgumentNullException.ThrowIfNull(win);
            ArgumentNullException.ThrowIfNull(monthly);

            // h_lr е постоянен за елемента (зависи само от θ_ss и ε)
            double h_lr = ComputeH_lr(win.Epsilon, win.ThetaSs);

            // A_gl = A_wi * (1 – F_fr)  [формула (3.40)]
            double a_gl = win.A_wi * (1.0 - win.F_fr);

            var monthResults = new WindowMonthlyResult[12];

            for (int m = 0; m < 12; m++)
            {
                double g_gl     = SafeGet(win.G_gl,     m);
                double fShObst  = SafeGet(win.F_sh_obst, m);
                double h_sol    = SafeGet(win.H_sol,     m);
                double deltaT   = monthly[m].DeltaT_m;
                double deltaSky = monthly[m].DeltaTheta_sky_m;

                // SolarFactor = g_gl * A_gl * F_sh_obst * H_sol
                double solarFactor = g_gl * a_gl * fShObst * h_sol;

                // Q_sky (3.39): 0.001 * F_sky * R_se * U_c * A_wi * h_lr * Δθ_sky * Δt
                double q_sky = ComputeQ_sky(
                    win.F_sky, win.R_se, win.U_c, win.A_wi,
                    h_lr, deltaSky, deltaT);

                // Q_sol_window (3.37): SolarFactor – Q_sky
                double q_sol = solarFactor - q_sky;

                monthResults[m] = new WindowMonthlyResult
                {
                    MonthIndex        = m,
                    MonthName         = monthly[m].MonthName,
                    DeltaT_m          = deltaT,
                    DeltaTheta_sky_m  = deltaSky,
                    A_gl              = a_gl,
                    G_gl              = g_gl,
                    F_sh_obst         = fShObst,
                    H_sol             = h_sol,
                    SolarFactor       = solarFactor,
                    H_lr              = h_lr,
                    Q_sky             = q_sky,
                    Q_sol_window      = q_sol
                };
            }

            return new WindowElementResult
            {
                ElementId      = win.Id,
                MonthlyResults = monthResults
            };
        }

        // ================================================================== //
        //  НЕПРОЗРАЧНИ ЕЛЕМЕНТИ
        // ================================================================== //

        /// <summary>
        /// Изчислява слънчевите топлинни печалби за един непрозрачен елемент (формула 3.38).
        /// </summary>
        public static OpaqueElementResult CalculateOpaque(
            OpaqueElement op,
            MonthlyGeneralData[] monthly)
        {
            ArgumentNullException.ThrowIfNull(op);
            ArgumentNullException.ThrowIfNull(monthly);

            double h_lr = ComputeH_lr(op.Epsilon, op.ThetaSs);

            var monthResults = new OpaqueMonthlyResult[12];

            for (int m = 0; m < 12; m++)
            {
                double fShObst  = SafeGet(op.F_sh_obst, m);
                double h_sol    = SafeGet(op.H_sol,     m);
                double deltaT   = monthly[m].DeltaT_m;
                double deltaSky = monthly[m].DeltaTheta_sky_m;

                // SolarFactorOpaque = α_sol * R_se * U_c * A_c * F_sh_obst * H_sol
                double solarFactorOpaque = op.Alpha_sol * op.R_se * op.U_c
                                           * op.A_c * fShObst * h_sol;

                // Q_sky (3.39)
                double q_sky = ComputeQ_sky(
                    op.F_sky, op.R_se, op.U_c, op.A_c,
                    h_lr, deltaSky, deltaT);

                // Q_sol_opaque (3.38)
                double q_sol = solarFactorOpaque - q_sky;

                monthResults[m] = new OpaqueMonthlyResult
                {
                    MonthIndex        = m,
                    MonthName         = monthly[m].MonthName,
                    DeltaT_m          = deltaT,
                    DeltaTheta_sky_m  = deltaSky,
                    Alpha_sol         = op.Alpha_sol,
                    R_se              = op.R_se,
                    U_c               = op.U_c,
                    A_c               = op.A_c,
                    F_sh_obst         = fShObst,
                    H_sol             = h_sol,
                    SolarFactorOpaque = solarFactorOpaque,
                    H_lr              = h_lr,
                    Q_sky             = q_sky,
                    Q_sol_opaque      = q_sol
                };
            }

            return new OpaqueElementResult
            {
                ElementId      = op.Id,
                MonthlyResults = monthResults
            };
        }

        // ================================================================== //
        //  ПОМОЩНИ МЕТОДИ
        // ================================================================== //

        /// <summary>
        /// Изчислява коефициента на топлопренасяне при дълговълново излъчване:
        /// h_lr = 4·ε·σ·(θ_ss + 273)³  [W/(m²·K)].
        /// </summary>
        /// <param name="epsilon">Степен на чернота ε [-].</param>
        /// <param name="thetaSs">Средна температура на повърхността θ_ss [°C].</param>
        public static double ComputeH_lr(double epsilon, double thetaSs)
        {
            double tAbs = thetaSs + 273.0;
            return 4.0 * epsilon * StefanBoltzmann * tAbs * tAbs * tAbs;
        }

        /// <summary>
        /// Изчислява топлинния поток към небето за един елемент за един месец:
        /// Q_sky = 0.001·F_sky·R_se·U_c·A·h_lr·Δθ_sky·Δt  [kWh].
        /// Формула (3.39).
        /// </summary>
        /// <param name="fSky">Коефициент на видимост между елемент и небе F_sky [-].</param>
        /// <param name="rSe">Топлинно съпр. на外 повърхност R_se [m²·K/W].</param>
        /// <param name="uC">U-стойност U_c [W/(m²·K)].</param>
        /// <param name="area">Площ A [m²].</param>
        /// <param name="hLr">Коефициент h_lr [W/(m²·K)].</param>
        /// <param name="deltaThetaSky">Δθ_sky_m [K].</param>
        /// <param name="deltaT">Продължителност на месеца Δt_m [h].</param>
        public static double ComputeQ_sky(
            double fSky, double rSe, double uC, double area,
            double hLr, double deltaThetaSky, double deltaT)
        {
            return 0.001 * fSky * rSe * uC * area * hLr * deltaThetaSky * deltaT;
        }

        // ------------------------------------------------------------------ //

        private static MonthlyTotalResult[] BuildMonthlyTotals(
            IEnumerable<WindowElement> windows,
            IEnumerable<OpaqueElement> opaqueElements,
            MonthlyGeneralData[] monthly)
        {
            var totals = new MonthlyTotalResult[12];

            for (int m = 0; m < 12; m++)
            {
                double deltaTHeat = Math.Max(0, monthly[m].HeatingDays) * 24.0;
                double deltaTCool = Math.Max(0, monthly[m].CoolingDays) * 24.0;
                double deltaTUnion = monthly[m].DeltaT_m;
                double deltaSky = monthly[m].DeltaTheta_sky_m;

                double sumWindows = 0.0;
                double sumOpaque = 0.0;
                double sumQsky = 0.0;
                double qHeating = 0.0;
                double qCooling = 0.0;

                foreach (var win in windows)
                {
                    double a_gl = win.A_wi * (1.0 - win.F_fr);
                    double fSh = SafeGet(win.F_sh_obst, m);
                    double hSol = SafeGet(win.H_sol, m);

                    SplitBySeasonHours(hSol, deltaTUnion, deltaTHeat, deltaTCool, out double hSolHeat, out double hSolCool);

                    double gHeat = ResolveModeG(win.G_gl_heat, win.G_gl, m);
                    double gCool = ResolveModeG(win.G_gl_cool, win.G_gl, m);
                    double hLr = ComputeH_lr(win.Epsilon, win.ThetaSs);

                    double qSkyHeat = ComputeQ_sky(win.F_sky, win.R_se, win.U_c, win.A_wi, hLr, deltaSky, deltaTHeat);
                    double qSkyCool = ComputeQ_sky(win.F_sky, win.R_se, win.U_c, win.A_wi, hLr, deltaSky, deltaTCool);

                    double qWinHeat = gHeat * a_gl * fSh * hSolHeat - qSkyHeat;
                    double qWinCool = gCool * a_gl * fSh * hSolCool - qSkyCool;

                    qHeating += qWinHeat;
                    qCooling += qWinCool;
                    sumWindows += qWinHeat + qWinCool;
                    sumQsky += qSkyHeat + qSkyCool;
                }

                foreach (var op in opaqueElements)
                {
                    double fSh = SafeGet(op.F_sh_obst, m);
                    double hSol = SafeGet(op.H_sol, m);
                    SplitBySeasonHours(hSol, deltaTUnion, deltaTHeat, deltaTCool, out double hSolHeat, out double hSolCool);

                    double hLr = ComputeH_lr(op.Epsilon, op.ThetaSs);
                    double qSkyHeat = ComputeQ_sky(op.F_sky, op.R_se, op.U_c, op.A_c, hLr, deltaSky, deltaTHeat);
                    double qSkyCool = ComputeQ_sky(op.F_sky, op.R_se, op.U_c, op.A_c, hLr, deltaSky, deltaTCool);

                    double qOpHeat = op.Alpha_sol * op.R_se * op.U_c * op.A_c * fSh * hSolHeat - qSkyHeat;
                    double qOpCool = op.Alpha_sol * op.R_se * op.U_c * op.A_c * fSh * hSolCool - qSkyCool;

                    qHeating += qOpHeat;
                    qCooling += qOpCool;
                    sumOpaque += qOpHeat + qOpCool;
                    sumQsky += qSkyHeat + qSkyCool;
                }

                double qSolTotal = qHeating + qCooling;

                totals[m] = new MonthlyTotalResult
                {
                    MonthIndex       = m,
                    MonthName        = monthly[m].MonthName,
                    SumQ_sol_windows = sumWindows,
                    SumQ_sol_opaque  = sumOpaque,
                    SumQ_sky         = sumQsky,
                    Q_sol_total      = qSolTotal,   // (3.36)
                    Q_sol_heating    = qHeating,
                    Q_sol_cooling    = qCooling
                };
            }

            return totals;
        }

        private static double SafeGet(double[] arr, int index)
            => (arr != null && index < arr.Length) ? arr[index] : 0.0;

        private static void SplitBySeasonHours(
            double total,
            double deltaTUnion,
            double deltaTHeat,
            double deltaTCool,
            out double heating,
            out double cooling)
        {
            double denominator = deltaTHeat + deltaTCool;
            if (denominator <= 0)
            {
                heating = 0.0;
                cooling = 0.0;
                return;
            }

            double source = deltaTUnion > 0 ? total : 0.0;
            heating = source * (deltaTHeat / denominator);
            cooling = source * (deltaTCool / denominator);
        }

        private static double ResolveModeG(double modeG, double[] gMonthly, int monthIndex)
            => modeG > 0 ? modeG : SafeGet(gMonthly, monthIndex);
    }
}
