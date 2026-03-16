using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EE.Doklad.Models
{
    // ══════════════════════════════════════════════════════════════════════════
    // Раздел 23 – Debug вътрешни топлинни печалби (формули 3.30–3.33)
    // EN ISO 52016-1 / BDS EN 7257_1
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Начин на задаване на вътрешен топлинен источник.
    /// </summary>
    public enum InternalGainsSourceKind
    {
        /// <summary>Мощност (W) × часове → Q_int,k,m = Φ_int,k * t_m / 1000   (формула 3.33)</summary>
        PowerWatts,

        /// <summary>Специфична годишна стойност (kWh/m².year) → разпределя по t_m</summary>
        SpecificAnnual_kWhM2Year
    }

    /// <summary>
    /// Категория на вътрешния топлинен источник (кореспондира с 3.32).
    /// </summary>
    public enum InternalGainsCategory
    {
        Occupants,   // Q_H/C;spec;int;oc;z;m  – метаболитна топлина
        Appliances,  // Q_H/C;spec;int;A;z;m   – уреди
        Lighting,    // Q_H/C;spec;int;L;z;m   – осветление
        WaterSystem, // Q_H/C;spec;int;WA;z;m  – ВиК загуби
        HVAC,        // Q_H/C;spec;int;HVAC;z;m
        Process      // Q_H/C;spec;int;proc;z;m
    }

    /// <summary>
    /// Описание на един вътрешен топлинен источник за Debug секцията.
    /// Поддържа и двата входни типа (3.33a – W×h, 3.33b – kWh/m²/year).
    /// </summary>
    public sealed class InternalGainsSourceInput
    {
        /// <summary>Уникален ключ за идентификация (напр. "occ-1", "light-led").</summary>
        public string SourceId { get; set; } = string.Empty;

        /// <summary>Показвано описание.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Категория по 3.32.</summary>
        public InternalGainsCategory Category { get; set; }

        /// <summary>Начин на задаване на входната стойност.</summary>
        public InternalGainsSourceKind Kind { get; set; }

        // ── Тип A: Мощност (W) ──────────────────────────────────────────────
        /// <summary>Средна мощност Φ_int,k [W]. Използва се при Kind = PowerWatts.</summary>
        public double Power_W { get; set; }

        // ── Тип B: Специфична годишна стойност ──────────────────────────────
        /// <summary>Специфична годишна стойност [kWh/m².year]. Използва се при Kind = SpecificAnnual_kWhM2Year.</summary>
        public double SpecificAnnual_kWhM2Year { get; set; }

        /// <summary>
        /// Дали источникът е „студ" (отрицателен знак по 3.32).
        /// Например: локален охладител, рекуперация с охладителен ефект.
        /// При true → Q_int,k,m се прибавя като отрицателна стойност в баланса.
        /// </summary>
        public bool IsColdSource { get; set; } = false;
    }

    /// <summary>
    /// Входни данни за Debug изчислението на вътрешни топлинни печалби.
    /// </summary>
    public sealed class InternalGainsDebugInput
    {
        public int ZoneId { get; set; } = 1;

        /// <summary>Месец 1..12</summary>
        public int Month { get; set; }

        /// <summary>Heating или Cooling</summary>
        public EpbMode Mode { get; set; }

        // ── Площи ────────────────────────────────────────────────────────────
        /// <summary>Отопляема площ A_heat [m²]</summary>
        public double AreaHeat_m2 { get; set; }

        /// <summary>Охлаждаема площ A_cool [m²] (може да се различава от A_heat)</summary>
        public double AreaCool_m2 { get; set; }

        // ── Графици от Секция 5 ───────────────────────────────────────────────
        /// <summary>Работни дни (Пн–Пт) – часове активност [h/day] за отоплителен режим.</summary>
        public double? HeatingWorkdaysHours { get; set; }
        public double? HeatingSaturdayHours { get; set; }
        public double? HeatingSundayHours { get; set; }

        public double? CoolingWorkdaysHours { get; set; }
        public double? CoolingSaturdayHours { get; set; }
        public double? CoolingSundayHours { get; set; }

        // ── Отоплителен сезон (от климатична зона) ───────────────────────────
        public int HeatingSeasonStartMonth { get; set; }
        public int HeatingSeasonStartDay   { get; set; }
        public int HeatingSeasonEndMonth   { get; set; }
        public int HeatingSeasonEndDay     { get; set; }

        // ── Охладителен сезон (от Секция 5) ──────────────────────────────────
        public int? CoolingSeasonStartMonth { get; set; }
        public int? CoolingSeasonStartDay   { get; set; }
        public int? CoolingSeasonEndMonth   { get; set; }
        public int? CoolingSeasonEndDay     { get; set; }

        // ── DaysOff от Секция 5 (масив 12 елемента) ──────────────────────
        /// <summary>Дни почивни по месеци, индекс 0 = януари.</summary>
        public int[] DaysOff { get; set; } = new int[12];

        // ── Референтна година за календарни изчисления ───────────────────────
        public int YearRef { get; set; } = global::EE.Doklad.CalendarDefaults.ReferenceYear;

        // ── Вътрешни топлинни източници ───────────────────────────────────────
        public List<InternalGainsSourceInput> Sources { get; set; } = new();

        // ── Процесна топлина (Секция 23 входове) ──────────────────────────────

        /// <summary>
        /// Процесна мощност [W].
        /// Положителна стойност = топлинен процес (загрева).
        /// Отрицателна стойност = охладителен процес (охлажда).
        /// 0 → компонентът не участва.
        /// </summary>
        public double ProcessHeat_W { get; set; } = 0.0;

        /// <summary>
        /// Годишни работни часове на процесното оборудване [h/год].
        /// </summary>
        public double ProcessAnnualHours { get; set; } = 0.0;

        /// <summary>
        /// Persisted monthly results for the heating season, produced by Section 23.
        /// Section 11 reads these values directly and must not recompute them.
        /// </summary>
        public ObservableCollection<InternalGainsMonthlyResult> HeatingMonths { get; } = [];

        /// <summary>
        /// Persisted monthly results for the cooling season, produced by Section 23.
        /// </summary>
        public ObservableCollection<InternalGainsMonthlyResult> CoolingMonths { get; } = [];
    }

    /// <summary>
    /// Persisted monthly internal-gains row shared between Section 23 and downstream readers.
    /// All values are absolute monthly energies in kWh.
    /// </summary>
    public sealed class InternalGainsMonthlyResult
    {
        public int Month { get; set; }
        public double Oc_kWh { get; set; }
        public double A_kWh { get; set; }
        public double L_kWh { get; set; }
        public double WA_kWh { get; set; }
        public double HVAC_kWh { get; set; }
        public double Proc_kWh { get; set; }
        public double Total_kWh { get; set; }
        public double Total_kWh_m2 { get; set; }
    }

    /// <summary>
    /// Режим на EPB изчислението.
    /// </summary>
    public enum EpbMode
    {
        Heating,
        Cooling
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Резултатни модели
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Debug ред за един вътрешен топлинен источник k, месец m.
    /// Съдържа всички междинни стойности по формула 3.33.
    /// </summary>
    public sealed class InternalGainsSourceDebugRow
    {
        // ── Идентификация ─────────────────────────────────────────────────────
        public string SourceId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public InternalGainsCategory Category { get; set; }
        public InternalGainsSourceKind InputKind { get; set; }
        public bool IsColdSource { get; set; }

        // ── Входни стойности ─────────────────────────────────────────────────
        /// <summary>Мощност Φ_int,k [W] (при PowerWatts) или 0 ако не се прилага.</summary>
        public double Phi_W { get; set; }

        /// <summary>Специфична годишна стойност [kWh/m².year] (при SpecificAnnual) или 0.</summary>
        public double SpecificAnnual_kWhM2Year { get; set; }

        // ── Времево изчисление ────────────────────────────────────────────────
        /// <summary>Изчислена площ за режима A_use [m²].</summary>
        public double AreaUsed_m2 { get; set; }

        /// <summary>Брой активни дни в месеца за режима (след приспадане на DaysOff).</summary>
        public double ActiveDays { get; set; }

        /// <summary>Работни часове t_m [h] за месеца (формула 3.33: t = активни дни × h/ден).</summary>
        public double ActiveHours_t_m { get; set; }

        // ── Резултат по 3.33 ──────────────────────────────────────────────────
        /// <summary>
        /// Q_int,k,m [kWh] по формула 3.33: = Φ_int,k * t_m / 1000
        /// При SpecificAnnual: = spec * A * (t_m / t_year) → еквивалентен поток
        /// Знакът е включен: отрицателен ако IsColdSource = true.
        /// </summary>
        public double Q_int_k_m_kWh { get; set; }

        // ── Специфична стойност ───────────────────────────────────────────────
        /// <summary>Q_int,k,m / A_use [kWh/m²] – специфичен принос.</summary>
        public double Q_int_k_m_specific_kWhM2 { get; set; }

        // ── Формула трейс ─────────────────────────────────────────────────────
        /// <summary>Приложена формула като текст за debug UI.</summary>
        public string FormulaTrace { get; set; } = string.Empty;

        /// <summary>Предупреждение ако е използван fallback.</summary>
        public string? FallbackWarning { get; set; }

        /// <summary>Дали е изчислен или е fallback/нула.</summary>
        public bool IsCalculated { get; set; }
    }

    /// <summary>
    /// Агрегиран резултат по категория (съответства на сумите в 3.32).
    /// </summary>
    public sealed class InternalGainsCategorySum
    {
        public InternalGainsCategory Category { get; set; }

        /// <summary>Сума Q [kWh/m²] за категорията (специфична).</summary>
        public double Q_spec_kWhM2 { get; set; }

        /// <summary>Сума Q [kWh] за категорията (зонална).</summary>
        public double Q_zone_kWh { get; set; }

        /// <summary>Брой активни източници в тази категория.</summary>
        public int SourceCount { get; set; }
    }

    /// <summary>
    /// Информация за времевото изчисление (Стъпка 2).
    /// </summary>
    public sealed class InternalGainsTimeInfo
    {
        /// <summary>Начало на сезона за месеца (ден в месеца: 1 ако не е частичен).</summary>
        public int SeasonStartDayInMonth { get; set; }

        /// <summary>Край на сезона за месеца (ден в месеца: последен ден ако не е частичен).</summary>
        public int SeasonEndDayInMonth { get; set; }

        /// <summary>Дали месецът е частично в сезона (start/end попадат в него).</summary>
        public bool IsPartialMonth { get; set; }

        /// <summary>Брой активни дни (след DaysOff) по тип.</summary>
        public double ActiveWeekdays { get; set; }
        public double ActiveSaturdays { get; set; }
        public double ActiveSundays { get; set; }
        public double TotalActiveDays { get; set; }

        /// <summary>Приложени DaysOff за месеца.</summary>
        public int DaysOffApplied { get; set; }

        /// <summary>Общо активни работни часове t_m [h] по режима.</summary>
        public double TotalActiveHours_t_m { get; set; }

        /// <summary>Часове по тип ден: h/ден</summary>
        public double WorkdaysHoursPerDay { get; set; }
        public double SaturdayHoursPerDay { get; set; }
        public double SundayHoursPerDay   { get; set; }

        /// <summary>Дали е използван fallback за часовете (липсва график).</summary>
        public bool HoursFallbackUsed { get; set; }
        public string? HoursFallbackReason { get; set; }
    }

    /// <summary>
    /// Пълен debug резултат за Секция 23 – Вътрешни топлинни печалби.
    /// </summary>
    public sealed class InternalGainsDebugResult
    {
        // ── Входни параметри (за трейс) ───────────────────────────────────────
        public int ZoneId { get; set; }
        public int Month { get; set; }
        public EpbMode Mode { get; set; }
        public double AreaUsed_m2 { get; set; }

        // ── Стъпка 1: Валидация ───────────────────────────────────────────────
        public bool InputValid { get; set; }
        public List<string> ValidationErrors { get; set; } = new();
        public List<string> ValidationWarnings { get; set; } = new();

        // ── Стъпка 2: Времева информация ──────────────────────────────────────
        public InternalGainsTimeInfo TimeInfo { get; set; } = new();

        // ── Стъпка 3: Ред по ред (формула 3.33) ──────────────────────────────
        public List<InternalGainsSourceDebugRow> SourceRows { get; set; } = new();

        // ── Стъпка 4: Агрегиране по категория (за 3.32) ──────────────────────
        public List<InternalGainsCategorySum> CategorySums { get; set; } = new();

        // ── Стъпка 5: Приложение в баланса (3.32 → 3.30) ─────────────────────
        /// <summary>
        /// Q_H/C;int;dir;z;m [kWh] – директни вътрешни печалби в самата зона (3.32).
        /// Формула: (Q_oc + Q_A + Q_L + Q_WA + Q_HVAC + Q_proc) * A_use
        /// </summary>
        public double Q_HC_int_dir_z_m_kWh { get; set; }

        /// <summary>
        /// Q_H/C;int;dir;z;m / A_use [kWh/m²] – специфична стойност.
        /// </summary>
        public double Q_HC_int_dir_z_m_specific_kWhM2 { get; set; }

        /// <summary>
        /// Q_H/C;int;ztc;m [kWh] – агрегиран месечен резултат по 3.30.
        /// Ако няма съседни неклиматизирани зони (3.31 = 0): = Q_HC_int_dir_z_m.
        /// </summary>
        public double Q_HC_int_ztc_m_kWh { get; set; }

        /// <summary>
        /// Принос от съседни некондиционирани зони [kWh] по 3.31.
        /// 0 ако не са настроени.
        /// </summary>
        public double Q_HC_int_uncond_contribution_kWh { get; set; }

        // ── Стъпка 6: Формула трейс ───────────────────────────────────────────
        public string Formula330Trace { get; set; } = string.Empty;
        public string Formula332Trace { get; set; } = string.Empty;
        public string Formula333Summary { get; set; } = string.Empty;

        /// <summary>Списък с всички приложени fallback-и.</summary>
        public List<string> FallbacksUsed { get; set; } = new();

        /// <summary>Дали изчислението е успешно.</summary>
        public bool IsSuccess { get; set; }
    }
}
