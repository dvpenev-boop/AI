using System;

namespace EE.Doklad.Services.Psychrometrics
{
    /// <summary>
    /// Детерминирана, unit-testable имплементация на психрометричните зависимости
    /// по Наредба 7257_1 §3.14 (формули 3.98 – 3.106).
    ///
    /// Константи (§3.14, фиксирани):
    ///   c_pa  = 1.006 kJ/(kg·K)   — специфичен топлинен капацитет на сух въздух
    ///   c_pw  = 1.805 kJ/(kg·K)   — специфичен топлинен капацитет на водни пари (при P=const)
    ///   h_we  = 2501  kJ/kg       — специфична топлина на изпарение при 0 °C
    ///   R_a   = 286.9 J/(kg·K)    — газова константа на сух въздух
    ///   R_w/R_a = 1.609            ⇒  0.62198 = 1/1.609
    /// </summary>
    public sealed class PsychrometricsService : IPsychrometrics
    {
        // ── Константи ────────────────────────────────────────────────────────────
        public const double C_PA_kJkgK  = 1.006;    // kJ/(kg_da·K)
        public const double C_PW_kJkgK  = 1.805;    // kJ/(kg_w·K)
        public const double H_WE_kJkg   = 2501.0;   // kJ/kg_w
        public const double R_A_JkgK    = 286.9;    // J/(kg_da·K)
        public const double RW_RA_ratio  = 1.609;    // R_w / R_a
        public const double EPS          = 1.0 / RW_RA_ratio; // ≈ 0.62198 kg_da/kg_w

        // Singleton convenience instance (stateless service).
        public static readonly PsychrometricsService Default = new();

        // ── IPsychrometrics ───────────────────────────────────────────────────────

        /// <inheritdoc/>
        public double SaturationPressure_Pa(double tempC)
        {
            // Формула (3.98): p_ws = exp(77.3450 + 0.0057·T – 7235/T) / T^8.2
            // T = t + 273.15 [K]
            // exp(числител) разделено на T^8.2  (не е деление вътре в показателя)
            double T = tempC + 273.15;
            if (T <= 0.0) throw new ArgumentOutOfRangeException(nameof(tempC), "Температурата в Келвин трябва да е > 0.");
            return Math.Exp(77.3450 + 0.0057 * T - 7235.0 / T) / Math.Pow(T, 8.2);
        }

        /// <inheritdoc/>
        public AirState Compute(double tempC, double rhPercent, double bPa)
        {
            if (bPa <= 0.0) throw new ArgumentOutOfRangeException(nameof(bPa), "Барометричното налягане трябва да е > 0 Pa.");
            double rh = Math.Clamp(rhPercent, 0.0, 100.0) / 100.0;

            // (3.98) Налягане на насищане
            double p_ws = SaturationPressure_Pa(tempC);

            // (3.99) Парциално налягане на водните пари
            double p_w = rh * p_ws;

            // Guard: p_w < B (физическо ограничение)
            if (p_w >= bPa)
                p_w = bPa * 0.9999; // cap – предотвратява деление на нула

            // (3.100) Влагосъдържание
            double x = EPS * p_w / (bPa - p_w);

            // (3.103) Енталпия на водните пари
            double h_w = C_PW_kJkgK * tempC + H_WE_kJkg;

            // (3.104) Специфична енталпия на влажния въздух
            //   h = c_pa·T + x·(c_pw·T + h_we)  kJ/kg_da
            double h = C_PA_kJkgK * tempC + x * (C_PW_kJkgK * tempC + H_WE_kJkg);

            // (3.105) Плътност на сухия въздух
            //   ρ_da = B / (R_a · T)   — пълното барометрично налягане в числителя (не B – p_w)
            double T_K = tempC + 273.15;
            double rho_da = bPa / (R_A_JkgK * T_K);   // kg_da/m³

            // (3.106) Плътност на влажния въздух
            //   ρ = ρ_da · (1 + x) / (1 + 1.609·x)
            double rho = rho_da * (1.0 + x) / (1.0 + RW_RA_ratio * x);

            return new AirState
            {
                T_C        = tempC,
                RH_Pct     = rhPercent,
                B_Pa       = bPa,
                p_ws_Pa    = p_ws,
                p_w_Pa     = p_w,
                x_kgkg     = x,
                h_kJkg     = h,
                rho_da_kgm3 = rho_da,
                rho_kgm3   = rho,
                h_w_kJkg   = h_w,
            };
        }
    }
}
