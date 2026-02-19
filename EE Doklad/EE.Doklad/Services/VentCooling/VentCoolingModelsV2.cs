using System;
using System.Collections.Generic;
using EE.Doklad.Services.Schedule;

namespace EE.Doklad.Services.VentCooling
{
    // ── Input DTO ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Параметри за Секция 14 – Вентилация/Охлаждане (входен DTO за Engine V2).
    /// </summary>
    public sealed class VentCoolingInputV2
    {
        // ── Дебит ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Специфичен дебит на приточния въздух [m³/h/m²] (q_spec).
        /// Умножава се по A_zone, за да се получи q_total [m³/h].
        /// Всички енергийни резултати са в kWh/m² (разделени на A_zone).
        /// </summary>
        public double AirflowSpec_m3hm2 { get; init; }

        /// <summary>Охлаждаема площ A_zone [m²].</summary>
        public double CooledArea_m2 { get; init; }

        // ── Подаван въздух ────────────────────────────────────────────────────────

        /// <summary>Температура на подавания въздух T_sup [°C].</summary>
        public double SupplyTemperature_C { get; init; }

        /// <summary>Относителна влажност на подавания въздух RH_sup [%].</summary>
        public double SupplyRH_Pct { get; init; }

        // ── Рекуперация ───────────────────────────────────────────────────────────

        /// <summary>
        /// Ефективност на рекуперация η_r [0..1].
        /// Редуцира ефективното h_out: h_out_eff = h_out – η_r·(h_out – h_extract).
        /// Ако η_r = 0 → без рекуперация; ако η_r > 0 и h_extract = null → h_extract ≈ h_sup.
        /// </summary>
        public double RecuperationEfficiency { get; init; }

        /// <summary>
        /// Температура на изхвърления въздух (extract air) [°C] за рекуперация.
        /// Ако null, се приема = SupplyTemperature_C (консервативна апроксимация без данни за extract).
        /// </summary>
        public double? ExtractAirTemperature_C { get; init; }

        /// <summary>
        /// RH на изхвърления въздух [%] за рекуперация.
        /// Ако null, се приема = 50%.
        /// </summary>
        public double? ExtractAirRH_Pct { get; init; }

        // ── Барометрично налягане ─────────────────────────────────────────────────

        /// <summary>
        /// Барометрично налягане B [Pa].
        /// За BG_avg данни – от ClimateZonePressureDefaults.
        /// За EPW – от почасовите данни (или средно за месеца).
        /// </summary>
        public double BarometricPressure_Pa { get; init; }

        // ── График вентилация ─────────────────────────────────────────────────────

        /// <summary>График за вентилация охлаждане (Секция 14).</summary>
        public WeeklyScheduleConfig VentSchedule { get; init; } = new();

        /// <summary>График за охлаждане (Секция 12) – за изчисляване на застъпването f_on.</summary>
        public WeeklyScheduleConfig? CoolSchedule { get; init; }

        // ── Охладителен сезон ─────────────────────────────────────────────────────

        public DateTime SeasonStart { get; init; }
        public DateTime SeasonEnd   { get; init; }

        // ── Почивни дни (Секция 5) ────────────────────────────────────────────────

        /// <summary>Масив 12 елемента: брой почивни дни по месеци.</summary>
        public int[] DaysOffPerMonth { get; init; } = new int[12];

        /// <summary>Допълнителни официални празници (optional).</summary>
        public IReadOnlyList<DateTime>? OfficialHolidays { get; init; }

        // ── ЕИ1 / ЕИ2 ────────────────────────────────────────────────────────────

        public EnergySourceConfigV2 EnergySource1 { get; init; } = new();
        public EnergySourceConfigV2? EnergySource2 { get; init; }
    }

    /// <summary>Конфигурация на енергиен източник за секция 14.</summary>
    public sealed class EnergySourceConfigV2
    {
        /// <summary>Дял от нетната енергия [%] (0..100).</summary>
        public double Share_Pct { get; init; } = 100.0;

        /// <summary>
        /// Обща ефективност (комбинирана): η_total = η_d · η_a · η_g (или COP за ТП).
        /// Трябва да е > 0. Типична стойност за ел. фрижидерна машина: COP 2.5–4.
        /// </summary>
        public double TotalEfficiency { get; init; } = 1.0;

        public string Label { get; init; } = string.Empty;
    }

    // ── Climate hourly point ──────────────────────────────────────────────────────

    /// <summary>
    /// Почасова климатична точка (входни данни от climate provider).
    /// За BG_avg: 24 часа за "типичен ден" на месеца.
    /// За EPW: реален час от годината.
    /// </summary>
    public sealed class ClimateHourPoint
    {
        public int Hour { get; init; }        // 0..23
        public double T_out_C { get; init; }
        public double RH_out_Pct { get; init; }
        public double B_Pa { get; init; }
    }

    // ── Output DTOs ───────────────────────────────────────────────────────────────

    /// <summary>Резултати от Секция 14 Engine V2 – месечни агрегати.</summary>
    public sealed class VentCoolingMonthlyResultV2
    {
        public int MonthNumber { get; init; }
        public string MonthName { get; init; } = string.Empty;

        // Schedule
        public double WorkingDays { get; init; }
        public double WorkingHours { get; init; }
        public int    HolidaysSubtracted { get; init; }

        // Нетни енергии [kWh/m²] (per unit floor area)
        /// <summary>Охлаждащо натоварване (h_out &gt; h_sup): нетна охлаждаща енергия [kWh/m²]</summary>
        public double E_cool_net_kWhm2 { get; init; }

        /// <summary>Топлинно натоварване (h_out &lt; h_sup): нетна отоплителна енергия [kWh/m²]</summary>
        public double E_heat_net_kWhm2 { get; init; }

        /// <summary>Латентна (изсушаваща) компонента при x_out &gt; x_sup [kWh/m²]</summary>
        public double E_dry_net_kWhm2 { get; init; }

        /// <summary>Принос към охлаждането (само в часове ∩ cool schedule) [kWh/m²]</summary>
        public double E_vent_contrib_net_kWhm2 { get; init; }

        // Debug psychrometrics (месечни средни)
        public double Avg_h_out_kJkg { get; init; }
        public double Avg_h_sup_kJkg { get; init; }
        public double Avg_x_out_kgkg { get; init; }
        public double Avg_x_sup_kgkg { get; init; }
        public double Avg_rho_out_kgm3 { get; init; }
    }

    /// <summary>Финален изход от Engine V2 (цял охладителен сезон).</summary>
    public sealed class VentCoolingOutputV2
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }

        // ── Сезонни суми [kWh/m²] ────────────────────────────────────────────────
        public double TotalCoolNet_kWhm2 { get; set; }
        public double TotalHeatNet_kWhm2 { get; set; }
        public double TotalDryNet_kWhm2  { get; set; }
        public double TotalVentContrib_kWhm2 { get; set; }

        // ── Потребна енергия ──────────────────────────────────────────────────────
        /// <summary>Обща нетна (без КПД) [kWh/m²]</summary>
        public double TotalNetEnergy_kWhm2 { get; set; }

        /// <summary>Потребна доставена ЕИ1 [kWh/m²] = нетна · дял / η</summary>
        public double FinalEnergyEI1_kWhm2 { get; set; }

        /// <summary>Потребна доставена ЕИ2 [kWh/m²]</summary>
        public double FinalEnergyEI2_kWhm2 { get; set; }

        /// <summary>Обща потребна доставена енергия [kWh/m²]</summary>
        public double TotalFinalEnergy_kWhm2 { get; set; }

        // ── Абсолютни стойности [kWh] ─────────────────────────────────────────────
        public double TotalCoolNet_kWh { get; set; }
        public double TotalHeatNet_kWh { get; set; }
        public double TotalDryNet_kWh  { get; set; }
        public double TotalFinalEnergy_kWh { get; set; }

        // ── График ────────────────────────────────────────────────────────────────
        public double TotalWorkingDays { get; set; }
        public double TotalWorkingHours { get; set; }

        public List<VentCoolingMonthlyResultV2> MonthlyResults { get; set; } = new();

        // ── Warnings ─────────────────────────────────────────────────────────
        public List<string> Warnings { get; set; } = new();

        // ── Debug snapshot of inputs (for UI display) ─────────────────────────
        public string DebugInputSummary { get; set; } = string.Empty;
    }
}
