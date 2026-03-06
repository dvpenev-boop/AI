using EE.Doklad.Sections.Section24SolarGains.Models;

namespace EE.Doklad.Sections.Section24SolarGains.Validation
{
    /// <summary>
    /// Резултат от валидация на един елемент.
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; } = [];
        public List<string> Warnings { get; } = [];
    }

    /// <summary>
    /// Валидира входните данни за Секция 24.
    /// Проверява физически диапазони на параметрите.
    /// </summary>
    public static class Section24Validator
    {
        // ================================================================== //
        //  ПРОЗОРЦИ
        // ================================================================== //

        /// <summary>
        /// Валидира прозрачен елемент.
        /// </summary>
        public static ValidationResult ValidateWindow(WindowElement win)
        {
            var result = new ValidationResult();
            string prefix = $"Прозорец '{win.Id}':";

            // Площ
            if (win.A_wi <= 0)
                result.Errors.Add($"{prefix} A_wi трябва да е > 0 m².");
            else if (win.A_wi > 1000)
                result.Warnings.Add($"{prefix} A_wi = {win.A_wi:F2} m² – необичайно голяма стойност.");

            // Дял рамка
            if (win.F_fr < 0 || win.F_fr >= 1.0)
                result.Errors.Add($"{prefix} F_fr трябва да е в [0, 1).");

            // U-стойност
            if (win.U_c <= 0)
                result.Errors.Add($"{prefix} U_c трябва да е > 0 W/(m²·K).");
            else if (win.U_c > 10)
                result.Warnings.Add($"{prefix} U_c = {win.U_c:F2} – необичайно висока стойност.");

            // R_se
            if (win.R_se <= 0)
                result.Errors.Add($"{prefix} R_se трябва да е > 0 m²·K/W.");
            else if (win.R_se > 1.0)
                result.Warnings.Add($"{prefix} R_se = {win.R_se:F4} – необичайно висока стойност.");

            // F_sky
            if (win.F_sky < 0 || win.F_sky > 1.0)
                result.Errors.Add($"{prefix} F_sky трябва да е в [0, 1].");

            // ε
            if (win.Epsilon < 0 || win.Epsilon > 1.0)
                result.Errors.Add($"{prefix} ε трябва да е в [0, 1].");

            // θ_ss
            if (win.ThetaSs < -50 || win.ThetaSs > 80)
                result.Warnings.Add($"{prefix} θ_ss = {win.ThetaSs}°C – необичайна стойност.");

            // Масиви
            ValidateMonthlyArray(win.H_sol,    $"{prefix} H_sol",     result, allowNegative: false);
            ValidateMonthlyArray(win.F_sh_obst,$"{prefix} F_sh_obst", result, allowNegative: false, max: 1.0);
            ValidateMonthlyArray(win.G_gl,     $"{prefix} g_gl",      result, allowNegative: false, max: 1.0);

            return result;
        }

        // ================================================================== //
        //  НЕПРОЗРАЧНИ
        // ================================================================== //

        /// <summary>
        /// Валидира непрозрачен елемент.
        /// </summary>
        public static ValidationResult ValidateOpaque(OpaqueElement op)
        {
            var result = new ValidationResult();
            string prefix = $"Непрозрачен '{op.Id}':";

            if (op.A_c <= 0)
                result.Errors.Add($"{prefix} A_c трябва да е > 0 m².");

            if (op.Alpha_sol < 0 || op.Alpha_sol > 1.0)
                result.Errors.Add($"{prefix} α_sol трябва да е в [0, 1]. Таблица 1: 0.3/0.6/0.9.");

            if (op.U_c <= 0)
                result.Errors.Add($"{prefix} U_c трябва да е > 0 W/(m²·K).");

            if (op.R_se <= 0)
                result.Errors.Add($"{prefix} R_se трябва да е > 0 m²·K/W.");

            if (op.F_sky < 0 || op.F_sky > 1.0)
                result.Errors.Add($"{prefix} F_sky трябва да е в [0, 1].");

            if (op.Epsilon < 0 || op.Epsilon > 1.0)
                result.Errors.Add($"{prefix} ε трябва да е в [0, 1].");

            if (op.ThetaSs < -50 || op.ThetaSs > 80)
                result.Warnings.Add($"{prefix} θ_ss = {op.ThetaSs}°C – необичайна стойност.");

            ValidateMonthlyArray(op.H_sol,    $"{prefix} H_sol",     result, allowNegative: false);
            ValidateMonthlyArray(op.F_sh_obst,$"{prefix} F_sh_obst", result, allowNegative: false, max: 1.0);

            return result;
        }

        // ================================================================== //
        //  ОБЩИ МЕСЕЧНИ ДАННИ
        // ================================================================== //

        /// <summary>
        /// Валидира масива от общи месечни данни (12 реда).
        /// </summary>
        public static ValidationResult ValidateMonthlyData(MonthlyGeneralData[] monthly)
        {
            var result = new ValidationResult();

            if (monthly == null || monthly.Length != 12)
            {
                result.Errors.Add("MonthlyData трябва да съдържа точно 12 реда.");
                return result;
            }

            for (int i = 0; i < 12; i++)
            {
                if (monthly[i].DeltaT_m < 0)
                    result.Errors.Add($"Месец {i + 1}: Δt_m не може да е отрицателно.");
                if (monthly[i].DeltaTheta_sky_m < 0)
                    result.Errors.Add($"Месец {i + 1}: Δθ_sky_m не може да е отрицателно.");
            }

            return result;
        }

        // ================================================================== //
        //  ПЪЛНА ВАЛИДАЦИЯ
        // ================================================================== //

        /// <summary>
        /// Валидира целия набор от входни данни.
        /// </summary>
        public static ValidationResult ValidateAll(Models.Section24SolarGainsData data)
        {
            var combined = new ValidationResult();

            foreach (var err in ValidateMonthlyData(data.MonthlyData).Errors)
                combined.Errors.Add(err);

            foreach (var w in data.Windows)
            {
                var r = ValidateWindow(w);
                combined.Errors.AddRange(r.Errors);
                combined.Warnings.AddRange(r.Warnings);
            }

            foreach (var op in data.OpaqueElements)
            {
                var r = ValidateOpaque(op);
                combined.Errors.AddRange(r.Errors);
                combined.Warnings.AddRange(r.Warnings);
            }

            if (data.Windows.Count == 0 && data.OpaqueElements.Count == 0)
                combined.Warnings.Add("Няма зададени елементи. Добавете поне един прозорец или непрозрачен елемент.");

            return combined;
        }

        // ================================================================== //
        //  ПОМОЩНИ
        // ================================================================== //

        private static void ValidateMonthlyArray(
            double[] arr, string name, ValidationResult result,
            bool allowNegative = true, double max = double.MaxValue)
        {
            if (arr == null || arr.Length != 12)
            {
                result.Errors.Add($"{name}: масивът трябва да съдържа точно 12 стойности.");
                return;
            }

            for (int i = 0; i < 12; i++)
            {
                if (!allowNegative && arr[i] < 0)
                    result.Errors.Add($"{name}[{i + 1}] = {arr[i]:F4} – не може да е отрицателно.");
                if (arr[i] > max)
                    result.Errors.Add($"{name}[{i + 1}] = {arr[i]:F4} – надвишава максималната допустима стойност {max}.");
            }
        }
    }
}
