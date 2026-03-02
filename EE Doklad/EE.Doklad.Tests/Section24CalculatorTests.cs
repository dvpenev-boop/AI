using System;
using System.Linq;
using EE.Doklad.Sections.Section24SolarGains.Calculator;
using EE.Doklad.Sections.Section24SolarGains.Models;
using EE.Doklad.Sections.Section24SolarGains.Validation;
using Xunit;

namespace EE.Doklad.Tests
{
    /// <summary>
    /// Модулни тестове за Секция 24 – топлинни печалби от слънчево греене.
    /// Покриват формули (3.37), (3.38), (3.39), h_lr и (3.36).
    /// </summary>
    public class Section24CalculatorTests
    {
        // ------------------------------------------------------------------ //
        //  Помощни методи за генериране на тестови данни
        // ------------------------------------------------------------------ //

        private static MonthlyGeneralData[] MakeMonthly(double deltaT = 744.0, double deltaSky = 11.0)
            => Enumerable.Range(0, 12)
               .Select(i => new MonthlyGeneralData { MonthIndex = i, DeltaT_m = deltaT, DeltaTheta_sky_m = deltaSky })
               .ToArray();

        private static WindowElement MakeWindow(
            double a_wi = 4.0, double f_fr = 0.20, double u_c = 1.3,
            double r_se = 0.13, double f_sky = 0.5, double eps = 0.9, double theta_ss = 10.0,
            double g_gl = 0.6, double h_sol = 100.0, double f_sh = 1.0)
            => new()
            {
                Id      = "W1",
                A_wi    = a_wi,
                F_fr    = f_fr,
                U_c     = u_c,
                R_se    = r_se,
                F_sky   = f_sky,
                Epsilon = eps,
                ThetaSs = theta_ss,
                G_gl    = Enumerable.Repeat(g_gl,  12).ToArray(),
                H_sol   = Enumerable.Repeat(h_sol, 12).ToArray(),
                F_sh_obst = Enumerable.Repeat(f_sh, 12).ToArray()
            };

        private static OpaqueElement MakeOpaque(
            double a_c = 20.0, double alpha_sol = 0.6, double u_c = 0.25,
            double r_se = 0.13, double f_sky = 0.5, double eps = 0.9, double theta_ss = 10.0,
            double h_sol = 80.0, double f_sh = 1.0)
            => new()
            {
                Id        = "OP1",
                A_c       = a_c,
                Alpha_sol = alpha_sol,
                U_c       = u_c,
                R_se      = r_se,
                F_sky     = f_sky,
                Epsilon   = eps,
                ThetaSs   = theta_ss,
                H_sol     = Enumerable.Repeat(h_sol, 12).ToArray(),
                F_sh_obst = Enumerable.Repeat(f_sh,  12).ToArray()
            };

        // ================================================================== //
        //  1. ComputeH_lr – формулата h_lr = 4·ε·σ·(θ_ss + 273)³
        // ================================================================== //

        [Fact]
        public void H_lr_CalculatesCorrectly_ForTypicalValues()
        {
            // eps=0.9, θ_ss=10°C → T = 283 K
            double h_lr = Section24Calculator.ComputeH_lr(0.9, 10.0);
            double expected = 4.0 * 0.9 * Section24Calculator.StefanBoltzmann * Math.Pow(283, 3);

            Assert.Equal(expected, h_lr, precision: 8);
        }

        [Fact]
        public void H_lr_ZeroEpsilon_ReturnsZero()
        {
            double h_lr = Section24Calculator.ComputeH_lr(0.0, 10.0);
            Assert.Equal(0.0, h_lr);
        }

        // ================================================================== //
        //  2. ComputeQ_sky – формула (3.39)
        // ================================================================== //

        [Fact]
        public void Q_sky_CalculatesCorrectly_KnownValues()
        {
            // 0.001 * 0.5 * 0.13 * 1.3 * 4.0 * h_lr(0.9,10) * 11 * 744
            double h_lr = Section24Calculator.ComputeH_lr(0.9, 10.0);
            double q_sky = Section24Calculator.ComputeQ_sky(0.5, 0.13, 1.3, 4.0, h_lr, 11.0, 744.0);
            double expected = 0.001 * 0.5 * 0.13 * 1.3 * 4.0 * h_lr * 11.0 * 744.0;

            Assert.Equal(expected, q_sky, precision: 10);
        }

        [Fact]
        public void Q_sky_ZeroFSky_ReturnsZero()
        {
            double h_lr = Section24Calculator.ComputeH_lr(0.9, 10.0);
            double q_sky = Section24Calculator.ComputeQ_sky(0.0, 0.13, 1.3, 4.0, h_lr, 11.0, 744.0);
            Assert.Equal(0.0, q_sky);
        }

        // ================================================================== //
        //  3. CalculateWindow – формула (3.37)
        // ================================================================== //

        [Fact]
        public void Window_QsolWindow_EqualsFormula()
        {
            // Прозорец с H_sol=100 kWh/m², g_gl=0.6, F_fr=0.20, A_wi=4 m², F_sh=1, Δt=744h
            var win     = MakeWindow();
            var monthly = MakeMonthly();

            var result  = Section24Calculator.CalculateWindow(win, monthly);

            // За месец 0 (всички константни)
            double a_gl      = 4.0 * (1 - 0.20);     // = 3.2
            double h_lr_val  = Section24Calculator.ComputeH_lr(0.9, 10.0);
            double q_sky_val = Section24Calculator.ComputeQ_sky(0.5, 0.13, 1.3, 4.0, h_lr_val, 11.0, 744.0);
            double solar     = 0.6 * a_gl * 1.0 * 100.0;
            double expected  = solar - q_sky_val;

            Assert.Equal(expected, result.MonthlyResults[0].Q_sol_window, precision: 8);
        }

        [Fact]
        public void Window_SolarFactor_MatchesIntermediate()
        {
            var win     = MakeWindow(g_gl: 0.5, a_wi: 2.0, f_fr: 0.10, h_sol: 90.0, f_sh: 0.8);
            var monthly = MakeMonthly();

            var result = Section24Calculator.CalculateWindow(win, monthly);

            double a_gl          = 2.0 * (1 - 0.10);  // 1.8
            double expectedFactor = 0.5 * a_gl * 0.8 * 90.0;

            Assert.Equal(expectedFactor, result.MonthlyResults[5].SolarFactor, precision: 8);
        }

        [Fact]
        public void Window_AllMonthsHaveSameResult_WhenInputsAreConstant()
        {
            var win     = MakeWindow();
            var monthly = MakeMonthly();
            var result  = Section24Calculator.CalculateWindow(win, monthly);

            double first = result.MonthlyResults[0].Q_sol_window;
            for (int m = 1; m < 12; m++)
                Assert.Equal(first, result.MonthlyResults[m].Q_sol_window, precision: 8);
        }

        [Fact]
        public void Window_AnnualSum_EqualsSumOf12Months()
        {
            var win     = MakeWindow();
            var monthly = MakeMonthly(deltaT: 730.0);
            var result  = Section24Calculator.CalculateWindow(win, monthly);

            double manualSum = result.MonthlyResults.Sum(r => r.Q_sol_window);
            Assert.Equal(manualSum, result.AnnualQ_sol_window, precision: 8);
        }

        // ================================================================== //
        //  4. CalculateOpaque – формула (3.38)
        // ================================================================== //

        [Fact]
        public void Opaque_QsolOpaque_EqualsFormula()
        {
            var op      = MakeOpaque();
            var monthly = MakeMonthly();

            var result = Section24Calculator.CalculateOpaque(op, monthly);

            double h_lr_val  = Section24Calculator.ComputeH_lr(0.9, 10.0);
            double q_sky_val = Section24Calculator.ComputeQ_sky(0.5, 0.13, 0.25, 20.0, h_lr_val, 11.0, 744.0);
            double solarOp   = 0.6 * 0.13 * 0.25 * 20.0 * 1.0 * 80.0;
            double expected  = solarOp - q_sky_val;

            Assert.Equal(expected, result.MonthlyResults[0].Q_sol_opaque, precision: 8);
        }

        [Fact]
        public void Opaque_SolarFactorOpaque_MatchesIntermediateProperty()
        {
            var op     = MakeOpaque(h_sol: 120.0, f_sh: 0.9, alpha_sol: 0.3, r_se: 0.13, u_c: 0.3, a_c: 15.0);
            var monthly = MakeMonthly();
            var result = Section24Calculator.CalculateOpaque(op, monthly);

            double expected = 0.3 * 0.13 * 0.3 * 15.0 * 0.9 * 120.0;
            Assert.Equal(expected, result.MonthlyResults[3].SolarFactorOpaque, precision: 8);
        }

        // ================================================================== //
        //  5. Calculate (main entry) – формула (3.36)
        // ================================================================== //

        [Fact]
        public void Calculate_TotalEqualsWindowsPlusOpaque()
        {
            var data = new Section24SolarGainsData();
            data.Windows.Add(MakeWindow());
            data.OpaqueElements.Add(MakeOpaque());

            var results = Section24Calculator.Calculate(data);

            for (int m = 0; m < 12; m++)
            {
                double expectedTotal = results.WindowResults[0].MonthlyResults[m].Q_sol_window
                                     + results.OpaqueResults[0].MonthlyResults[m].Q_sol_opaque;
                Assert.Equal(expectedTotal, results.MonthlyTotals[m].Q_sol_total, precision: 8);
            }
        }

        [Fact]
        public void Calculate_NoElements_AllZeroTotals()
        {
            var data = new Section24SolarGainsData();
            var results = Section24Calculator.Calculate(data);

            Assert.All(results.MonthlyTotals, mt => Assert.Equal(0.0, mt.Q_sol_total));
        }

        [Fact]
        public void Calculate_MultipleWindows_SumCorrect()
        {
            var data = new Section24SolarGainsData();
            data.Windows.Add(MakeWindow(h_sol: 80.0));
            data.Windows.Add(MakeWindow(h_sol: 50.0));

            var results = Section24Calculator.Calculate(data);

            for (int m = 0; m < 12; m++)
            {
                double sum = results.WindowResults.Sum(w => w.MonthlyResults[m].Q_sol_window);
                Assert.Equal(sum, results.MonthlyTotals[m].SumQ_sol_windows, precision: 8);
            }
        }

        [Fact]
        public void Calculate_AnnualTotal_EqualsSumOfMonthlyTotals()
        {
            var data = new Section24SolarGainsData();
            data.Windows.Add(MakeWindow());
            data.OpaqueElements.Add(MakeOpaque());

            var results = Section24Calculator.Calculate(data);

            double manualAnnual = results.MonthlyTotals.Sum(t => t.Q_sol_total);
            Assert.Equal(manualAnnual, results.AnnualQ_sol_total, precision: 8);
        }

        // ================================================================== //
        //  6. SampleSolarGainsData – проверка, че примерните данни се изчисляват
        // ================================================================== //

        [Fact]
        public void SampleData_CalculatesWithoutErrors()
        {
            var sample  = EE.Doklad.Sections.Section24SolarGains.SampleSolarGainsData.Create();
            var results = Section24Calculator.Calculate(sample);

            Assert.Equal(12, results.MonthlyTotals.Length);
            Assert.True(results.AnnualQ_sol_total > 0, "Очаква се положителни общи годишни печалби.");
        }

        // ================================================================== //
        //  7. Validation
        // ================================================================== //

        [Fact]
        public void Validator_Window_InvalidArea_ReturnsError()
        {
            var win = MakeWindow(a_wi: -1.0);
            var r   = Section24Validator.ValidateWindow(win);
            Assert.False(r.IsValid);
            Assert.Contains(r.Errors, e => e.Contains("A_wi"));
        }

        [Fact]
        public void Validator_Window_InvalidFfr_ReturnsError()
        {
            var win = MakeWindow(f_fr: 1.5);
            var r   = Section24Validator.ValidateWindow(win);
            Assert.False(r.IsValid);
            Assert.Contains(r.Errors, e => e.Contains("F_fr"));
        }

        [Fact]
        public void Validator_Opaque_InvalidAlphaSol_ReturnsError()
        {
            var op = MakeOpaque(alpha_sol: 1.5);
            var r  = Section24Validator.ValidateOpaque(op);
            Assert.False(r.IsValid);
            Assert.Contains(r.Errors, e => e.Contains("α_sol"));
        }

        [Fact]
        public void Validator_ValidData_IsValid()
        {
            var win = MakeWindow();
            Assert.True(Section24Validator.ValidateWindow(win).IsValid);

            var op = MakeOpaque();
            Assert.True(Section24Validator.ValidateOpaque(op).IsValid);
        }

        [Fact]
        public void Validator_EmptyData_HasWarning()
        {
            var data = new Section24SolarGainsData();
            var r    = Section24Validator.ValidateAll(data);
            Assert.True(r.Warnings.Count > 0);
        }
    }
}
