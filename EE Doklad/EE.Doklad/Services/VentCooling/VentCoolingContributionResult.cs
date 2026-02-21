using System.Collections.Generic;

namespace EE.Doklad.Services.VentCooling
{
    // ─────────────────────────────────────────────────────────────────────────────
    // VentCoolingContributionResult
    //
    // DTO с резултатите от изчислението на
    // „Принос към охлаждането от вентилация (нетен)" по новата логика
    // (формула с BaseFactor, ScenarioDesign, ScenarioRaised, f_on).
    // ─────────────────────────────────────────────────────────────────────────────

    public sealed class VentCoolingContributionResult
    {
        // ── Входни агрегати ───────────────────────────────────────────────────────

        /// <summary>ρ·Cp·q·H_season / 1000  [kWh/m²·K]</summary>
        public double BaseFactor { get; set; }

        // ── Сценарии [kWh/m²] ────────────────────────────────────────────────────

        /// <summary>BaseFactor × (T_room_design − T_supply)  [kWh/m²]</summary>
        public double ScenarioDesign_kWhm2 { get; set; }

        /// <summary>BaseFactor × (T_room_raised − T_supply)  [kWh/m²]</summary>
        public double ScenarioRaised_kWhm2 { get; set; }

        /// <summary>min(ScenarioDesign, ScenarioRaised)  [kWh/m²]</summary>
        public double Min_kWhm2 { get; set; }

        /// <summary>max(ScenarioDesign, ScenarioRaised)  [kWh/m²]</summary>
        public double Max_kWhm2 { get; set; }

        // ── Графикова компонента ──────────────────────────────────────────────────

        /// <summary>
        /// Коефициент на застъпване OverlapHoursWeek / VentHoursWeek, clamp [0,1].
        /// </summary>
        public double F_on { get; set; }

        // ── Нетен резултат ────────────────────────────────────────────────────────

        /// <summary>
        /// f_on·ScenarioDesign + (1−f_on)·ScenarioRaised  [kWh/m²]
        /// UI поле „Принос към охлаждането от вентилация (нетен)" – Разход на m²
        /// Може да е отрицателно (не се клипва).
        /// </summary>
        public double Net_kWhm2 { get; set; }

        /// <summary>
        /// Net_kWhm2 × CoolingArea_m2  [kWh]
        /// UI поле „Потребна енергия"
        /// </summary>
        public double Net_kWh { get; set; }

        // ── Диагностика ───────────────────────────────────────────────────────────

        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string> Warnings { get; set; } = new();
    }
}
