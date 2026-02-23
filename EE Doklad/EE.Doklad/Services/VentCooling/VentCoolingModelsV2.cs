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

    // ── Debug DTOs ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Един ред от debug таблицата (формула 3.93 стъпка по стъпка).
    /// Съдържа всички междинни психрометрични стойности за 1 час.
    /// Съответства 1:1 на Excel колоните от референцията.
    /// </summary>
    public sealed class VentCoolingHourlyDebugRow
    {
        public int    Hour        { get; init; }   // 0..23
        public int    Run         { get; init; }   // 0=неактивен, 1=активен по график
        public double T_out_C     { get; init; }
        public double RH_out_Pct  { get; init; }
        public double B_Pa        { get; init; }
        public double p_ws_out    { get; init; }   // Налягане на насищане [Pa]
        public double p_w_out     { get; init; }   // Парциално налягане [Pa]
        public double x_out       { get; init; }   // Влагосъдържание [kg/kg]
        public double rho_da_out  { get; init; }   // Плътност сух въздух [kg/m³]
        public double rho_out     { get; init; }   // Плътност влажен въздух [kg/m³]
        public double h_out       { get; init; }   // Енталпия [kJ/kg_da]
        public double rhoh_out    { get; init; }   // ρ·h на външен въздух [kJ/m³]
        public double x_sup       { get; init; }   // Влагосъдържание подаван въздух [kg/kg]
        public double h_sup       { get; init; }   // Енталпия подаван въздух [kJ/kg_da]
        public double rhoh_sup    { get; init; }   // ρ·h на подавания въздух [kJ/m³]
        public double delta_h     { get; init; }   // rhoh_out – rhoh_sup [kJ/m³] (+ = охлаждане)
        public double E_cool_hour { get; init; }   // Охладителна енергия за 1 час [kWh] (whole zone)
        public double E_heat_hour { get; init; }   // Топлинна енергия за 1 час [kWh]
        public double E_dry_hour  { get; init; }   // Латентна (изсушаване) [kWh]
    public double E_cool_month_kWhm2 { get; init; } // Охла.дневно (месечно) per m2 [kWh/m2]
    public double E_heat_month_kWhm2 { get; init; } // Топл. месечно per m2 [kWh/m2]
    public double E_dry_month_kWhm2  { get; init; } // Латентно месечно per m2 [kWh/m2]
        public int    Workdays    { get; init; }   // Работни дни в месеца
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

        /// <summary>
        /// 24 реда (час 0..23) с всички психрометрични междинни стойности.
        /// Попълва се само когато EngineV2 е стартиран с enableHourlyDebug=true.
        /// Използва се за export в CSV / сравнение с Excel референцията.
        /// </summary>
        public List<VentCoolingHourlyDebugRow> HourlyDebugRows { get; init; } = new();
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

    /// <summary>Потребна доставена ЕИ1 [kWh] (абсолютна за зоната)</summary>
    public double FinalEnergyEI1_kWh { get; set; }

    /// <summary>Потребна доставена ЕИ2 [kWh] (абсолютна за зоната)</summary>
    public double FinalEnergyEI2_kWh { get; set; }

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

        // ── CSV Export ────────────────────────────────────────────────────────
        /// <summary>
        /// Генерира CSV с debug таблицата за всички месеци: Hour, Run, T_out, RH_out, B,
        /// p_ws_out, p_w_out, x_out, rho_da_out, rho_out, h_out, x_sup, h_sup,
        /// Δh, E_cool_hour, E_heat_hour, E_dry_hour, Workdays.
        ///
        /// Може директно да се сравнява с Excel референцията 1:1.
        /// </summary>
        public string BuildHourlyDebugCsv()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Month;MonthName;Hour;Run;T_out_C;RH_out_Pct;B_Pa;" +
                          "p_ws_out_Pa;p_w_out_Pa;x_out;rho_da_out;rho_out;" +
                          "h_out_kJkg;rhoh_out_kJm3;x_sup;h_sup_kJkg;rhoh_sup_kJm3;delta_rhoh;E_cool_hour_kWh;" +
                          "E_heat_hour_kWh;E_dry_hour_kWh;E_cool_month_kWhm2;E_heat_month_kWhm2;E_dry_month_kWhm2;Workdays");

            foreach (var mr in MonthlyResults)
            {
                foreach (var r in mr.HourlyDebugRows)
                {
                    sb.AppendLine(string.Join(";", new object[]
                    {
                        mr.MonthNumber,
                        mr.MonthName,
                        r.Hour,
                        r.Run,
                        r.T_out_C.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
                        r.RH_out_Pct.ToString("F1", System.Globalization.CultureInfo.InvariantCulture),
                        r.B_Pa.ToString("F0", System.Globalization.CultureInfo.InvariantCulture),
                        r.p_ws_out.ToString("F3", System.Globalization.CultureInfo.InvariantCulture),
                        r.p_w_out.ToString("F3", System.Globalization.CultureInfo.InvariantCulture),
                        r.x_out.ToString("F6", System.Globalization.CultureInfo.InvariantCulture),
                        r.rho_da_out.ToString("F6", System.Globalization.CultureInfo.InvariantCulture),
                        r.rho_out.ToString("F6", System.Globalization.CultureInfo.InvariantCulture),
                        r.h_out.ToString("F4", System.Globalization.CultureInfo.InvariantCulture),
                        r.rhoh_out.ToString("F6", System.Globalization.CultureInfo.InvariantCulture),
                        r.x_sup.ToString("F6", System.Globalization.CultureInfo.InvariantCulture),
                        r.h_sup.ToString("F4", System.Globalization.CultureInfo.InvariantCulture),
                        r.rhoh_sup.ToString("F6", System.Globalization.CultureInfo.InvariantCulture),
                        r.delta_h.ToString("F4", System.Globalization.CultureInfo.InvariantCulture),
                        r.E_cool_hour.ToString("F6", System.Globalization.CultureInfo.InvariantCulture),
                        r.E_heat_hour.ToString("F6", System.Globalization.CultureInfo.InvariantCulture),
                        r.E_dry_hour.ToString("F6", System.Globalization.CultureInfo.InvariantCulture),
                        r.E_cool_month_kWhm2.ToString("F6", System.Globalization.CultureInfo.InvariantCulture),
                        r.E_heat_month_kWhm2.ToString("F6", System.Globalization.CultureInfo.InvariantCulture),
                        r.E_dry_month_kWhm2.ToString("F6", System.Globalization.CultureInfo.InvariantCulture),
                        r.Workdays
                    }));
                }
            }
            return sb.ToString();
        }
    }
}
