using System;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Models;

namespace EE.Doklad.Services
{
    // ══════════════════════════════════════════════════════════════════════════
    // Входен контейнер за InternalGainsAggregator
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Всички данни, нужни на InternalGainsAggregator за да изчисли
    /// месечните вътрешни топлинни печалби по 6 компонента.
    /// </summary>
    public sealed class InternalGainsAggregatorInput
    {
        // ── Площ ─────────────────────────────────────────────────────────────
        /// <summary>Използваема (отопляема) площ A_use [m²]</summary>
        public double A_use_m2 { get; set; }

        // ── Отоплителен сезон ─────────────────────────────────────────────────
        public int HeatingStartMonth { get; set; }
        public int HeatingStartDay   { get; set; } = 1;
        public int HeatingEndMonth   { get; set; }
        public int HeatingEndDay     { get; set; }
        /// <summary>Часове активност на ден за отопление</summary>
        public double HeatingHoursPerDay { get; set; } = 24.0;

        // ── Охладителен сезон ────────────────────────────────────────────────
        public int? CoolingStartMonth { get; set; }
        public int? CoolingStartDay   { get; set; }
        public int? CoolingEndMonth   { get; set; }
        public int? CoolingEndDay     { get; set; }
        /// <summary>Часове активност на ден за охлаждане</summary>
        public double CoolingHoursPerDay { get; set; } = 24.0;

        /// <summary>Референтна година</summary>
        public int YearRef { get; set; } = 2024;

        // ── Компонент 1: Обитатели ────────────────────────────────────────────
        /// <summary>Явна топлина на обитател за отопление [W/person] (Секция 11)</summary>
        public double OccupantsSensibleHeat_H_W  { get; set; }
        /// <summary>Явна топлина на обитател за охлаждане [W/person] (Секция 13)</summary>
        public double OccupantsSensibleHeat_C_W  { get; set; }
        /// <summary>Брой хора</summary>
        public int    NumberOfOccupants          { get; set; }

        // ── Компонент 2: Уреди (Секция 18 – агрегирано) ──────────────────────
        /// <summary>Едновременна инсталирана мощност [W] (TotalSimultaneousPower_W)</summary>
        public double Appliances_TotalPower_W          { get; set; }
        /// <summary>Годишна енергия [kWh/год] (TotalAnnualEnergy_kWh) – ако > 0 ползва се</summary>
        public double Appliances_TotalAnnualEnergy_kWh { get; set; }
        /// <summary>Годишни работни часове (AnnualOperatingHours) – за разпределяне</summary>
        public double Appliances_AnnualOperatingHours  { get; set; }

        // ── Компонент 3: Осветление (Секция 17 – агрегирано) ─────────────────
        /// <summary>Обща инсталирана мощност [W] (TotalLightingPower_W = TotalPower_kW * 1000)</summary>
        public double Lighting_TotalPower_W          { get; set; }
        /// <summary>Годишна енергия [kWh/год] (TotalAnnualLightingEnergy_kWh)</summary>
        public double Lighting_TotalAnnualEnergy_kWh { get; set; }
        /// <summary>Годишни работни часове за осветление</summary>
        public double Lighting_AnnualOperatingHours  { get; set; }

        // ── Компонент 4: Топла вода / ВиК (Секция 16) ────────────────────────
        /// <summary>
        /// Регенерируеми загуби от ВиК системата към зоната [kWh/год].
        /// 0 → компонентът е 0.
        /// </summary>
        public double WaterSystem_RecoverableHeat_kWh_Annual { get; set; }

        // ── Компонент 5: Помпи и вентилатори (Секция 15) ─────────────────────
        // За HVAC НЕ ползваме SeasonMask часове, а директните часове от Секция 15.
        /// <summary>Обща инсталирана мощност за отопление [W] (Φ_HVAC_H)</summary>
        public double HVAC_HeatingTotalPower_W   { get; set; }
        /// <summary>Годишни работни часове за отопление (от Секция 15.1)</summary>
        public double HVAC_HeatingAnnualHours    { get; set; }
        /// <summary>Годишна консумация за отопление [kWh] (от Секция 15.1)</summary>
        public double HVAC_HeatingAnnualConsumption_kWh { get; set; }
        /// <summary>Обща инсталирана мощност за охлаждане [W] (Φ_HVAC_C)</summary>
        public double HVAC_CoolingTotalPower_W   { get; set; }
        /// <summary>Годишни работни часове за охлаждане (от Секция 15.2)</summary>
        public double HVAC_CoolingAnnualHours    { get; set; }
        /// <summary>Годишна консумация за охлаждане [kWh] (от Секция 15.2)</summary>
        public double HVAC_CoolingAnnualConsumption_kWh { get; set; }

        // ── Компонент 6: Процесна топлина (Секция 23 входове) ─────────────────
        /// <summary>Процесна мощност [W] (положителна = топлинна; отрицателна = охладителна)</summary>
        public double ProcessHeat_W           { get; set; }
        /// <summary>Годишни работни часове за процесна топлина</summary>
        public double ProcessAnnualHours      { get; set; }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Изходен контейнер
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Резултат от InternalGainsAggregator: две таблици (Heating + Cooling), 12 реда всяка.
    /// </summary>
    public sealed class InternalGainsAggregatorResult
    {
        /// <summary>Месечна таблица за отопление (12 реда, индекс 0 = Януари).</summary>
        public MonthlyGainsRow[] HeatingTable { get; } = BuildEmpty();

        /// <summary>Месечна таблица за охлаждане (12 реда, индекс 0 = Януари).</summary>
        public MonthlyGainsRow[] CoolingTable { get; } = BuildEmpty();

        /// <summary>Обща A_use [m²] (за информация).</summary>
        public double A_use_m2 { get; set; }

        private static MonthlyGainsRow[] BuildEmpty()
        {
            var arr = new MonthlyGainsRow[12];
            for (int i = 0; i < 12; i++)
                arr[i] = new MonthlyGainsRow { Month = i + 1 };
            return arr;
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // InternalGainsAggregator
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Изчислява автоматично месечните вътрешни топлинни печалби (Секция 23)
    /// по формули 3.32 и 3.33 от EN ISO 52016-1.
    ///
    /// Входни данни: InternalGainsAggregatorInput (агрегирани стойности от Секции 11/13/15/16/17/18).
    /// Изход:        InternalGainsAggregatorResult (12-редови таблици Heating + Cooling).
    ///
    /// Компоненти:
    ///   1. Oc   – обитатели (явна топлина × часове по сезон)
    ///   2. A    – уреди (от Секция 18, разпределение по часове)
    ///   3. L    – осветление (от Секция 17, разпределение по часове)
    ///   4. WA   – ВиК регенерируеми загуби (от Секция 16, разпределение по маска)
    ///   5. HVAC – помпи и вентилатори (от Секция 15, директни часове БЕЗ сезонна маска)
    ///   6. Proc – процесна топлина (ръчен вход в Секция 23, разпределение по маска)
    /// </summary>
    public static class InternalGainsAggregator
    {
        private const double Eps = 1e-9;

        // ──────────────────────────────────────────────────────────────────────
        // Главна входна точка
        // ──────────────────────────────────────────────────────────────────────

        public static InternalGainsAggregatorResult Compute(InternalGainsAggregatorInput inp)
        {
            if (inp == null) throw new ArgumentNullException(nameof(inp));

            var result = new InternalGainsAggregatorResult { A_use_m2 = inp.A_use_m2 };

            // ── Сезонни маски ────────────────────────────────────────────────
            var heatMask = BuildHeatingMask(inp);
            var coolMask = BuildCoolingMask(inp);

            // ── Компонент 1: Обитатели ────────────────────────────────────────
            ComputeOccupants(inp, heatMask, coolMask, result);

            // ── Компонент 2: Уреди ────────────────────────────────────────────
            ComputeAppliances(inp, heatMask, coolMask, result);

            // ── Компонент 3: Осветление ───────────────────────────────────────
            ComputeLighting(inp, heatMask, coolMask, result);

            // ── Компонент 4: ВиК (WA) ────────────────────────────────────────
            ComputeWaterSystem(inp, heatMask, coolMask, result);

            // ── Компонент 5: HVAC (помпи и вентилатори) ──────────────────────
            ComputeHvac(inp, result);

            // ── Компонент 6: Процесна топлина ────────────────────────────────
            ComputeProcess(inp, heatMask, coolMask, result);

            // ── Изчисли TotalPerM2 ────────────────────────────────────────────
            FinalizeRows(result, inp.A_use_m2);

            return result;
        }

        // ──────────────────────────────────────────────────────────────────────
        // Сезонни маски
        // ──────────────────────────────────────────────────────────────────────

        private static SeasonMaskResult BuildHeatingMask(InternalGainsAggregatorInput inp)
        {
            return SeasonMaskService.Compute(new SeasonParams
            {
                StartMonth  = inp.HeatingStartMonth,
                StartDay    = inp.HeatingStartDay,
                EndMonth    = inp.HeatingEndMonth,
                EndDay      = inp.HeatingEndDay,
                HoursPerDay = inp.HeatingHoursPerDay,
                YearRef     = inp.YearRef
            });
        }

        private static SeasonMaskResult? BuildCoolingMask(InternalGainsAggregatorInput inp)
        {
            if (inp.CoolingStartMonth == null || inp.CoolingEndMonth == null)
                return null;

            return SeasonMaskService.Compute(new SeasonParams
            {
                StartMonth  = inp.CoolingStartMonth.Value,
                StartDay    = inp.CoolingStartDay ?? 1,
                EndMonth    = inp.CoolingEndMonth.Value,
                EndDay      = inp.CoolingEndDay
                    ?? DateTime.DaysInMonth(inp.YearRef, inp.CoolingEndMonth.Value),
                HoursPerDay = inp.CoolingHoursPerDay,
                YearRef     = inp.YearRef
            });
        }

        // ──────────────────────────────────────────────────────────────────────
        // 1. Обитатели
        // Q_oc,H[m] = Φ_sens,H * N * t_H[m] / 1000
        // Q_oc,C[m] = Φ_sens,C * N * t_C[m] / 1000
        // ──────────────────────────────────────────────────────────────────────

        private static void ComputeOccupants(
            InternalGainsAggregatorInput inp,
            SeasonMaskResult heatMask,
            SeasonMaskResult? coolMask,
            InternalGainsAggregatorResult result)
        {
            double phi_H = inp.OccupantsSensibleHeat_H_W * inp.NumberOfOccupants;
            double phi_C = inp.OccupantsSensibleHeat_C_W * inp.NumberOfOccupants;

            for (int m = 0; m < 12; m++)
            {
                result.HeatingTable[m].Oc = phi_H * heatMask.Hours[m] / 1000.0;
                result.CoolingTable[m].Oc = coolMask != null
                    ? phi_C * coolMask.Hours[m] / 1000.0
                    : 0.0;
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // 2. Уреди
        // Ако TotalAnnualEnergy_kWh > 0:
        //   Q_A[m] = TotalAnnualEnergy * (operatingHoursInMonth / totalAnnualHours)
        // Иначе:
        //   Q_A[m] = TotalPower_W * operatingHoursInMonth / 1000
        //
        // operatingHoursInMonth за сезона = mask.Hours[m]
        // ──────────────────────────────────────────────────────────────────────

        private static void ComputeAppliances(
            InternalGainsAggregatorInput inp,
            SeasonMaskResult heatMask,
            SeasonMaskResult? coolMask,
            InternalGainsAggregatorResult result)
        {
            bool useAnnual = inp.Appliances_TotalAnnualEnergy_kWh > Eps;
            double annualHours = inp.Appliances_AnnualOperatingHours;
            double annualEnergy = inp.Appliances_TotalAnnualEnergy_kWh;
            double power_W = inp.Appliances_TotalPower_W;

            for (int m = 0; m < 12; m++)
            {
                result.HeatingTable[m].A = DistributeByHours(
                    useAnnual, annualEnergy, annualHours, power_W, heatMask.Hours[m]);
                result.CoolingTable[m].A = coolMask != null
                    ? DistributeByHours(useAnnual, annualEnergy, annualHours, power_W, coolMask.Hours[m])
                    : 0.0;
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // 3. Осветление – същата логика като Уреди
        // ──────────────────────────────────────────────────────────────────────

        private static void ComputeLighting(
            InternalGainsAggregatorInput inp,
            SeasonMaskResult heatMask,
            SeasonMaskResult? coolMask,
            InternalGainsAggregatorResult result)
        {
            bool useAnnual = inp.Lighting_TotalAnnualEnergy_kWh > Eps;
            double annualHours = inp.Lighting_AnnualOperatingHours;
            double annualEnergy = inp.Lighting_TotalAnnualEnergy_kWh;
            double power_W = inp.Lighting_TotalPower_W;

            for (int m = 0; m < 12; m++)
            {
                result.HeatingTable[m].L = DistributeByHours(
                    useAnnual, annualEnergy, annualHours, power_W, heatMask.Hours[m]);
                result.CoolingTable[m].L = coolMask != null
                    ? DistributeByHours(useAnnual, annualEnergy, annualHours, power_W, coolMask.Hours[m])
                    : 0.0;
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // 4. ВиК (WA) – разпределение по сезонна маска
        // Q_WA[m] = Q_WA_annual * (seasonHours[m] / totalSeasonHours)
        // ──────────────────────────────────────────────────────────────────────

        private static void ComputeWaterSystem(
            InternalGainsAggregatorInput inp,
            SeasonMaskResult heatMask,
            SeasonMaskResult? coolMask,
            InternalGainsAggregatorResult result)
        {
            double annual = inp.WaterSystem_RecoverableHeat_kWh_Annual;
            if (annual < Eps) return; // нищо за разпределяне

            // Разпределяме пропорционално на часовете на всеки сезон.
            // Общо часове = отоплителни + охладителни (без припокриване по дефиниция).
            double hH = heatMask.TotalHours;
            double hC = coolMask?.TotalHours ?? 0;
            double hTotal = hH + hC;
            if (hTotal < Eps) return;

            double annualH = annual * hH / hTotal;
            double annualC = annual * hC / hTotal;

            for (int m = 0; m < 12; m++)
            {
                result.HeatingTable[m].WA = hH > Eps
                    ? annualH * heatMask.Hours[m] / hH
                    : 0.0;
                result.CoolingTable[m].WA = (coolMask != null && hC > Eps)
                    ? annualC * coolMask.Hours[m] / hC
                    : 0.0;
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // 5. HVAC – помпи и вентилатори (Секция 15)
        //
        // Специфика: НЕ ползваме сезонната маска за часове.
        // Ползваме директно Hours/year от Секция 15.
        //
        // Q_HVAC_H_annual = Φ_HVAC_H [W] * t_HVAC_H [h] / 1000
        //   или директно = HeatingAnnualConsumption_kWh (ако е > 0)
        //
        // Разпределяме годишните кWh по месеци пропорционално на дните
        // от сезонната маска (proxy за натоварване).
        // ──────────────────────────────────────────────────────────────────────

        private static void ComputeHvac(
            InternalGainsAggregatorInput inp,
            InternalGainsAggregatorResult result)
        {
            // ── Отопление ───────────────────────────────────────────────────
            double hvacH_annual = inp.HVAC_HeatingAnnualConsumption_kWh > Eps
                ? inp.HVAC_HeatingAnnualConsumption_kWh
                : inp.HVAC_HeatingTotalPower_W * inp.HVAC_HeatingAnnualHours / 1000.0;

            // ── Охлаждане ────────────────────────────────────────────────────
            double hvacC_annual = inp.HVAC_CoolingAnnualConsumption_kWh > Eps
                ? inp.HVAC_CoolingAnnualConsumption_kWh
                : inp.HVAC_CoolingTotalPower_W * inp.HVAC_CoolingAnnualHours / 1000.0;

            // ── Разпределяме по месеци пропорционално на дните на сезонната маска
            // (директните часове от Секция 15 са годишни и нямаме месечно разбивка
            //  – ползваме SeasonMask като proxy)
            var heatMask = BuildHeatingMask(inp);
            var coolMask = BuildCoolingMask(inp);

            double totalHeatDays = heatMask.TotalDays;
            double totalCoolDays = coolMask?.TotalDays ?? 0;

            for (int m = 0; m < 12; m++)
            {
                result.HeatingTable[m].HVAC = totalHeatDays > Eps
                    ? hvacH_annual * heatMask.Days[m] / totalHeatDays
                    : 0.0;
                result.CoolingTable[m].HVAC = (coolMask != null && totalCoolDays > Eps)
                    ? hvacC_annual * coolMask.Days[m] / totalCoolDays
                    : 0.0;
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // 6. Процесна топлина
        // Q_proc_annual = Φ_proc [W] * AnnualHours / 1000
        // Разпределяме по сезонна маска (пропорционално на часовете).
        // Отрицателна стойност ако е охладителен процес (Φ_proc < 0).
        // ──────────────────────────────────────────────────────────────────────

        private static void ComputeProcess(
            InternalGainsAggregatorInput inp,
            SeasonMaskResult heatMask,
            SeasonMaskResult? coolMask,
            InternalGainsAggregatorResult result)
        {
            if (Math.Abs(inp.ProcessHeat_W) < Eps || inp.ProcessAnnualHours < Eps)
                return;

            double annual = inp.ProcessHeat_W * inp.ProcessAnnualHours / 1000.0;

            double hH = heatMask.TotalHours;
            double hC = coolMask?.TotalHours ?? 0;
            double hTotal = hH + hC;
            if (hTotal < Eps) return;

            double annualH = annual * hH / hTotal;
            double annualC = annual * hC / hTotal;

            for (int m = 0; m < 12; m++)
            {
                result.HeatingTable[m].Proc = hH > Eps
                    ? annualH * heatMask.Hours[m] / hH
                    : 0.0;
                result.CoolingTable[m].Proc = (coolMask != null && hC > Eps)
                    ? annualC * coolMask.Hours[m] / hC
                    : 0.0;
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // Спомагателни методи
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Разпределя енергия за даден месец.
        /// Ако useAnnual = true и annualHours > 0:
        ///     E_month = annualEnergy * monthHours / annualHours
        /// Иначе:
        ///     E_month = power_W * monthHours / 1000
        /// </summary>
        private static double DistributeByHours(
            bool useAnnual,
            double annualEnergy,
            double annualHours,
            double power_W,
            double monthHours)
        {
            if (monthHours < Eps) return 0.0;

            if (useAnnual && annualHours > Eps)
                return annualEnergy * monthHours / annualHours;

            return power_W * monthHours / 1000.0;
        }

        /// <summary>
        /// Попълва TotalPerM2 за всеки ред.
        /// </summary>
        private static void FinalizeRows(InternalGainsAggregatorResult result, double a_use)
        {
            foreach (var row in result.HeatingTable.Concat(result.CoolingTable))
            {
                row.TotalPerM2 = a_use > Eps ? row.Total / a_use : 0.0;
            }
        }
    }
}
