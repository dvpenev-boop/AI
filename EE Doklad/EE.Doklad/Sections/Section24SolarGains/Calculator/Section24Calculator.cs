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

            var monthlyAccumulators = CreateMonthlyAccumulators();

            var windowResults = data.Windows
                .Select(w => CalculateWindow(w, data.MonthlyData, monthlyAccumulators))
                .ToArray();

            var opaqueResults = data.OpaqueElements
                .Select(o => CalculateOpaque(o, data.MonthlyData, monthlyAccumulators))
                .ToArray();

            var monthlyTotals = BuildMonthlyTotals(monthlyAccumulators, data.MonthlyData);

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
            => CalculateWindow(win, monthly, accumulators: null);

        private static WindowElementResult CalculateWindow(
            WindowElement win,
            MonthlyGeneralData[] monthly,
            MonthlyAccumulator[]? accumulators)
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

                if (accumulators != null)
                    AccumulateWindowMonthlyTotals(accumulators[m], win, monthly[m], m, a_gl, h_lr, fShObst, h_sol, deltaSky);
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
            => CalculateOpaque(op, monthly, accumulators: null);

        private static OpaqueElementResult CalculateOpaque(
            OpaqueElement op,
            MonthlyGeneralData[] monthly,
            MonthlyAccumulator[]? accumulators)
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

                if (accumulators != null)
                    AccumulateOpaqueMonthlyTotals(accumulators[m], op, monthly[m], h_lr, fShObst, h_sol, deltaSky);
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

        private static MonthlyAccumulator[] CreateMonthlyAccumulators()
            => Enumerable.Range(0, 12).Select(_ => new MonthlyAccumulator()).ToArray();

        private static MonthlyTotalResult[] BuildMonthlyTotals(
            MonthlyAccumulator[] accumulators,
            MonthlyGeneralData[] monthly)
        {
            var totals = new MonthlyTotalResult[12];

            for (int m = 0; m < 12; m++)
            {
                var acc = accumulators[m];
                double qSolTotal = acc.QHeating + acc.QCooling;

                totals[m] = new MonthlyTotalResult
                {
                    MonthIndex       = m,
                    MonthName        = monthly[m].MonthName,
                    SumQ_sol_windows = acc.SumWindows,
                    SumQ_sol_opaque  = acc.SumOpaque,
                    SumQ_sky         = acc.SumQSky,
                    Q_sol_total      = qSolTotal,   // (3.36)
                    Q_sol_heating    = acc.QHeating,
                    Q_sol_cooling    = acc.QCooling
                };
            }

            return totals;
        }

        private static void AccumulateWindowMonthlyTotals(
            MonthlyAccumulator acc,
            WindowElement win,
            MonthlyGeneralData month,
            int monthIndex,
            double a_gl,
            double hLr,
            double fSh,
            double hSol,
            double deltaSky)
        {
            double deltaTHeat = Math.Max(0, month.HeatingDays) * 24.0;
            double deltaTCool = Math.Max(0, month.CoolingDays) * 24.0;
            double deltaTUnion = month.DeltaT_m;

            SplitBySeasonHours(hSol, deltaTUnion, deltaTHeat, deltaTCool, out double hSolHeat, out double hSolCool);

            double gHeat = ResolveModeG(win.G_gl_heat, win.G_gl, monthIndex);
            double gCool = ResolveModeG(win.G_gl_cool, win.G_gl, monthIndex);

            double qSkyHeat = ComputeQ_sky(win.F_sky, win.R_se, win.U_c, win.A_wi, hLr, deltaSky, deltaTHeat);
            double qSkyCool = ComputeQ_sky(win.F_sky, win.R_se, win.U_c, win.A_wi, hLr, deltaSky, deltaTCool);

            double qWinHeat = gHeat * a_gl * fSh * hSolHeat - qSkyHeat;
            double qWinCool = gCool * a_gl * fSh * hSolCool - qSkyCool;

            acc.QHeating += qWinHeat;
            acc.QCooling += qWinCool;
            acc.SumWindows += qWinHeat + qWinCool;
            acc.SumQSky += qSkyHeat + qSkyCool;
        }

        private static void AccumulateOpaqueMonthlyTotals(
            MonthlyAccumulator acc,
            OpaqueElement op,
            MonthlyGeneralData month,
            double hLr,
            double fSh,
            double hSol,
            double deltaSky)
        {
            double deltaTHeat = Math.Max(0, month.HeatingDays) * 24.0;
            double deltaTCool = Math.Max(0, month.CoolingDays) * 24.0;
            double deltaTUnion = month.DeltaT_m;

            SplitBySeasonHours(hSol, deltaTUnion, deltaTHeat, deltaTCool, out double hSolHeat, out double hSolCool);

            double qSkyHeat = ComputeQ_sky(op.F_sky, op.R_se, op.U_c, op.A_c, hLr, deltaSky, deltaTHeat);
            double qSkyCool = ComputeQ_sky(op.F_sky, op.R_se, op.U_c, op.A_c, hLr, deltaSky, deltaTCool);

            double qOpHeat = op.Alpha_sol * op.R_se * op.U_c * op.A_c * fSh * hSolHeat - qSkyHeat;
            double qOpCool = op.Alpha_sol * op.R_se * op.U_c * op.A_c * fSh * hSolCool - qSkyCool;

            acc.QHeating += qOpHeat;
            acc.QCooling += qOpCool;
            acc.SumOpaque += qOpHeat + qOpCool;
            acc.SumQSky += qSkyHeat + qSkyCool;
        }

        private sealed class MonthlyAccumulator
        {
            public double SumWindows;
            public double SumOpaque;
            public double SumQSky;
            public double QHeating;
            public double QCooling;
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
