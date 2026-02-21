using System;
using System.Collections.Generic;
using EE.Doklad.Services.Psychrometrics;
using EE.Doklad.Services.Schedule;

namespace EE.Doklad.Services.VentCooling
{
    /// <summary>
    /// Секция 14 – Вентилация/Охлаждане: чист числов двигател v2.
    ///
    /// Pipeline:
    ///   1. Validate inputs
    ///   2. Compute supply-air psychrometric state (constant for all hours)
    ///   3. Per month:
    ///      a. Get climate hourly data (24 points for BG_avg or real EPW)
    ///      b. Compute WorkdaySchedule (workdays, holidays)
    ///      c. For each active hour j: compute outdoor AirState, apply recuperation, integrate energy
    ///      d. Multiply by WorkingDays (BG_avg) or sum real hours (EPW)
    ///   4. Apply EI1/EI2
    ///   5. Return VentCoolingOutputV2
    ///
    /// Единици и конвенции:
    ///   - Дебит: q_spec [m³/h/m²] × A_zone [m²] = q_total [m³/h]
    ///   - Масов дебит сух въздух: m_da = rho_da_out × q_total / 3600  [kg_da/s]
    ///     или за почасова интеграция:  m_da_h = rho_da_out × q_total   [kg_da/h]
    ///   - Енергия за 1 час: E_h = m_da_h × Δh / 3600  [kWh]
    ///     (m_da_h в kg_da/h, Δh в kJ/kg_da → kJ/h → /3600 → kWh)
    ///   - Резултати нормализирани по m²: E_h_m2 = E_h / A_zone  [kWh/m²]
    ///   - Нетна охлаждаща енергия: само когато h_out_eff > h_sup (знакова конвенция + → охлаждане)
    ///   - Изсушаване: латентна компонента когато x_out > x_sup
    /// </summary>
    public sealed class VentCoolingEngineV2
    {
        private static readonly string[] _monthNames =
        {
            "Януари", "Февруари", "Март", "Април", "Май", "Юни",
            "Юли", "Август", "Септември", "Октомври", "Ноември", "Декември"
        };

        private readonly IPsychrometrics _psych;

        public VentCoolingEngineV2(IPsychrometrics? psychrometrics = null)
        {
            _psych = psychrometrics ?? PsychrometricsService.Default;
        }

        // ── Public API ────────────────────────────────────────────────────────────

        /// <summary>
        /// Основен метод за изчисление на Секция 14 по месечен метод с почасова интеграция.
        /// </summary>
        /// <param name="input">Входен DTO.</param>
        /// <param name="getHourlyData">
        ///   Функция, която за даден месец (1..12) връща списък с <see cref="ClimateHourPoint"/>.
        ///   За BG_avg: 24 точки (типичен ден); за EPW: всички часове в месеца.
        /// </param>
        /// <param name="isBgAvgMode">
        ///   true = BG_avg (24 часа × WorkingDays); false = EPW (реален календар, без умножение).
        /// </param>
        /// <param name="yearRef">Референтна година за Calendar изчисления.</param>
        public VentCoolingOutputV2 Calculate(
            VentCoolingInputV2 input,
            Func<int, IReadOnlyList<ClimateHourPoint>> getHourlyData,
            bool isBgAvgMode = true,
            int yearRef = 2024)
        {
            var output = new VentCoolingOutputV2();

            // ── Validate ──────────────────────────────────────────────────────────
            if (input == null)             { output.IsValid = false; output.ErrorMessage = "Липсват входни данни."; return output; }
            if (input.CooledArea_m2 <= 0)  { output.IsValid = false; output.ErrorMessage = "Охлаждаемата площ трябва да е > 0."; return output; }
            if (input.AirflowSpec_m3hm2 <= 0) { output.IsValid = false; output.ErrorMessage = "Специфичният дебит трябва да е > 0."; return output; }
            if (!input.VentSchedule.IsValid)  { output.IsValid = false; output.ErrorMessage = "Невалиден график за вентилация."; return output; }
            if (input.BarometricPressure_Pa <= 0) { output.IsValid = false; output.ErrorMessage = "Барометричното налягане трябва да е > 0."; return output; }

            double area = input.CooledArea_m2;
            double q_total = input.AirflowSpec_m3hm2 * area;  // m³/h total (for whole zone)

            // ── Supply-air state (constant) ───────────────────────────────────────
            AirState supState;
            try
            {
                supState = _psych.Compute(input.SupplyTemperature_C, input.SupplyRH_Pct, input.BarometricPressure_Pa);
            }
            catch (Exception ex)
            {
                output.IsValid = false;
                output.ErrorMessage = $"Грешка при изчисление на supply air state: {ex.Message}";
                return output;
            }

            // Extract-air state for recuperation (constant)
            AirState extractState = default;
            bool hasRecuperation = input.RecuperationEfficiency > 0.0 && input.RecuperationEfficiency <= 1.0;
            if (hasRecuperation)
            {
                double t_ex = input.ExtractAirTemperature_C ?? input.SupplyTemperature_C;
                double rh_ex = input.ExtractAirRH_Pct ?? 50.0;
                extractState = _psych.Compute(t_ex, rh_ex, input.BarometricPressure_Pa);
            }

            // ── Workday schedule ──────────────────────────────────────────────────
            var scheduleResults = WorkdayScheduleCalculator.ComputeMonthly(
                input.VentSchedule,
                input.SeasonStart,
                input.SeasonEnd,
                input.DaysOffPerMonth,
                input.OfficialHolidays,
                yearRef);

            // ── Debug input summary ───────────────────────────────────────────────
            output.DebugInputSummary =
                $"qv_spec = {input.AirflowSpec_m3hm2:F3} m³/h·m²\n" +
                $"qv = {q_total:F2} m³/h\n" +
                $"Tsup = {input.SupplyTemperature_C:F1} °C  RHsup = {input.SupplyRH_Pct:F1}%  " +
                $"h_sup = {supState.h_kJkg:F2} kJ/kg  x_sup = {supState.x_kgkg:F4} kg/kg\n" +
                $"η_r = {input.RecuperationEfficiency * 100.0:F1}%  " +
                (hasRecuperation
                    ? $"T_extract = {input.ExtractAirTemperature_C ?? input.SupplyTemperature_C:F1} °C  " +
                      $"h_extract = {extractState.h_kJkg:F2} kJ/kg  x_extract = {extractState.x_kgkg:F4} kg/kg"
                    : "(без рекуперация)") + "\n" +
                $"Graf: wd={input.VentSchedule.TimeRange.StartHour}..{input.VentSchedule.TimeRange.EndHour} " +
                $"({input.VentSchedule.TimeRange.RunHoursPerDay} h/d)  " +
                $"Workdays={input.VentSchedule.WorkdaysActive}  Sat={input.VentSchedule.SaturdayActive}  Sun={input.VentSchedule.SundayActive}\n" +
                $"Сезон: {input.SeasonStart:dd.MM} – {input.SeasonEnd:dd.MM}  B = {input.BarometricPressure_Pa / 100.0:F0} hPa";

            // ── Overlap fraction (per day, constant if schedules don't change month-to-month) ──
            double f_on = 1.0; // default: ventilation contributes 100% to cooling
            if (input.CoolSchedule != null && input.CoolSchedule.IsValid)
            {
                f_on = WorkdayScheduleCalculator.OverlapFraction(
                    input.VentSchedule.TimeRange,
                    input.CoolSchedule.TimeRange);
            }

            // ── Per-month integration ─────────────────────────────────────────────
            double totalCool = 0, totalHeat = 0, totalDry = 0, totalContrib = 0;
            double totalWorkDays = 0, totalWorkHours = 0;

            for (int m = 1; m <= 12; m++)
            {
                var sched = scheduleResults[m - 1];
                if (sched.WorkingDays <= 0.0) continue;

                IReadOnlyList<ClimateHourPoint> hourlyData;
                try { hourlyData = getHourlyData(m); }
                catch (Exception ex) { output.Warnings.Add($"Месец {m}: грешка при четене на климатични данни – {ex.Message}"); continue; }

                if (hourlyData == null || hourlyData.Count == 0) continue;

                // Determine the active hours set from the vent schedule
                int schedStart = input.VentSchedule.TimeRange.StartHour;
                int schedEnd   = input.VentSchedule.TimeRange.EndHour;

                // Integrators for this month's "typical day"
                double day_cool = 0, day_heat = 0, day_dry = 0, day_contrib = 0;
                double sum_h_out = 0, sum_h_sup = 0, sum_x_out = 0, sum_x_sup = 0, sum_rho_out = 0;
                int activeHourCount = 0;
                int workDaysInt = (int)Math.Round(sched.WorkingDays);

                var hourlyDebugRows = new List<VentCoolingHourlyDebugRow>(hourlyData.Count);

                foreach (var pt in hourlyData)
                {
                    // Run flag: 1 if the hour is within the ventilation schedule, 0 otherwise
                    bool isActive = (pt.Hour >= schedStart && pt.Hour <= schedEnd);
                    int run = isActive ? 1 : 0;

                    // Use point B if it has one, else use zone default
                    double b = pt.B_Pa > 0 ? pt.B_Pa : input.BarometricPressure_Pa;

                    AirState outState;
                    try { outState = _psych.Compute(pt.T_out_C, pt.RH_out_Pct, b); }
                    catch
                    {
                        output.Warnings.Add($"Месец {m} час {pt.Hour}: невалидни psychrometric данни.");
                        // Emit a zero-row in debug so the hour is still visible
                        hourlyDebugRows.Add(new VentCoolingHourlyDebugRow
                        {
                            Hour = pt.Hour, Run = 0,
                            T_out_C = pt.T_out_C, RH_out_Pct = pt.RH_out_Pct, B_Pa = b,
                            x_sup = supState.x_kgkg, h_sup = supState.h_kJkg,
                            Workdays = workDaysInt,
                        });
                        continue;
                    }

                    // Apply recuperation: shift outdoor air state toward extract-air state.
                    double h_out_eff = outState.h_kJkg;
                    double x_out_eff = outState.x_kgkg;
                    if (hasRecuperation)
                    {
                        double eta = input.RecuperationEfficiency;
                        h_out_eff = outState.h_kJkg + eta * (extractState.h_kJkg - outState.h_kJkg);
                        x_out_eff = outState.x_kgkg + eta * (extractState.x_kgkg - outState.x_kgkg);
                    }

                    // ── rhoh = ρ · h (kJ/m³) за външния въздух ───────────────────
                    double rhoh_out_eff = outState.rho_kgm3 * h_out_eff;   // kJ/m³
                    double rhoh_sup     = supState.rho_kgm3 * supState.h_kJkg; // kJ/m³ (константа)

                    // ── Формула за енергия per m² per час (kWh/m²) ───────────────
                    //   E = (qv_spec / 3600) · Δrhoh
                    //   qv_spec = q_total / area [m³/h/m²]
                    double qv_spec = q_total / area; // m³/h/m²
                    double factor  = qv_spec / 3600.0; // kWh·m³ / (m²·kJ)  →  при умножение по kJ/m³ дава kWh/m²

                    double delta_rhoh = rhoh_out_eff - rhoh_sup; // kJ/m³ (положително = охлаждане)

                    // E_cool (явна охлаждаща): само ако delta_rhoh > 0 (kWh — за зоната, за 1 час)
                    double e_cool_h = (delta_rhoh > 0.0) ? factor * delta_rhoh * area : 0.0;

                    // E_heat (явна отоплителна): само ако delta_rhoh < 0 (kWh — за зоната, за 1 час)
                    double e_heat_h = (delta_rhoh < 0.0) ? factor * Math.Abs(delta_rhoh) * area : 0.0;

                    // ── E_dry (латентна / изсушаване): (x_out − x_sup) · 2501 · qv_spec / 3600
                    //   може да е отрицателно (овлажняване) или положително (изсушаване)
                    double delta_x = x_out_eff - supState.x_kgkg;
                    double e_dry_h = factor * delta_x * PsychrometricsService.H_WE_kJkg * area; // kWh (зона)

                    // ── Build debug row BEFORE the Run=0 skip, so every hour appears ──
                    double e_cool_dbg = isActive ? e_cool_h : 0.0;
                    double e_heat_dbg = isActive ? e_heat_h : 0.0;
                    double e_dry_dbg  = isActive ? e_dry_h  : 0.0;

                    hourlyDebugRows.Add(new VentCoolingHourlyDebugRow
                    {
                        Hour        = pt.Hour,
                        Run         = run,
                        T_out_C     = pt.T_out_C,
                        RH_out_Pct  = pt.RH_out_Pct,
                        B_Pa        = b,
                        p_ws_out    = outState.p_ws_Pa,
                        p_w_out     = outState.p_w_Pa,
                        x_out       = x_out_eff,
                        rho_da_out  = outState.rho_da_kgm3,
                        rho_out     = outState.rho_kgm3,
                        h_out       = h_out_eff,
                        rhoh_out    = rhoh_out_eff,
                        x_sup       = supState.x_kgkg,
                        h_sup       = supState.h_kJkg,
                        rhoh_sup    = rhoh_sup,
                        delta_h     = isActive ? delta_rhoh : 0.0,
                        E_cool_hour = e_cool_dbg / area,   // per m² for debug display
                        E_heat_hour = e_heat_dbg / area,
                        E_dry_hour  = e_dry_dbg  / area,
                        E_cool_month_kWhm2 = (e_cool_dbg / area) * workDaysInt,
                        E_heat_month_kWhm2 = (e_heat_dbg / area) * workDaysInt,
                        E_dry_month_kWhm2  = (e_dry_dbg  / area) * workDaysInt,
                        Workdays    = workDaysInt,
                    });

                    // ── Skip inactive hours from energy integration ────────────────
                    if (!isActive) continue;

                    day_cool += e_cool_h;
                    day_heat += e_heat_h;
                    day_dry  += e_dry_h;

                    // Contribution to cooling (only hours in overlap with cool schedule)
                    bool inCoolOverlap = input.CoolSchedule == null
                        || (pt.Hour >= input.CoolSchedule.TimeRange.StartHour
                            && pt.Hour <= input.CoolSchedule.TimeRange.EndHour);
                    if (e_cool_h > 0.0 && inCoolOverlap)
                        day_contrib += e_cool_h;

                    // Debug accumulators
                    sum_h_out   += h_out_eff;
                    sum_h_sup   += supState.h_kJkg;
                    sum_x_out   += x_out_eff;
                    sum_x_sup   += supState.x_kgkg;
                    sum_rho_out += outState.rho_kgm3;
                    activeHourCount++;
                }

                // ── Scale from "typical day" to "monthly total" ────────────────────
                double workDays = sched.WorkingDays;
                double month_cool, month_heat, month_dry, month_contrib;

                if (isBgAvgMode)
                {
                    // BG_avg: 24-hour typical day → multiply by work days
                    month_cool    = day_cool    * workDays;
                    month_heat    = day_heat    * workDays;
                    month_dry     = day_dry     * workDays;
                    month_contrib = day_contrib * workDays;
                }
                else
                {
                    // EPW: hourlyData already contains all hours of the month filtered by schedule
                    // → sum is already the monthly total (no multiplication)
                    month_cool    = day_cool;
                    month_heat    = day_heat;
                    month_dry     = day_dry;
                    month_contrib = day_contrib;
                }

                // Normalize per m²
                double inv = 1.0 / area;

                int avg_n = Math.Max(1, activeHourCount);
                output.MonthlyResults.Add(new VentCoolingMonthlyResultV2
                {
                    MonthNumber             = m,
                    MonthName               = _monthNames[m - 1],
                    WorkingDays             = workDays,
                    WorkingHours            = sched.WorkingHours,
                    HolidaysSubtracted      = sched.HolidaysSubtracted,
                    E_cool_net_kWhm2        = month_cool    * inv,
                    E_heat_net_kWhm2        = month_heat    * inv,
                    E_dry_net_kWhm2         = month_dry     * inv,
                    E_vent_contrib_net_kWhm2 = month_contrib * inv,
                    Avg_h_out_kJkg          = sum_h_out  / avg_n,
                    Avg_h_sup_kJkg          = sum_h_sup  / avg_n,
                    Avg_x_out_kgkg          = sum_x_out  / avg_n,
                    Avg_x_sup_kgkg          = sum_x_sup  / avg_n,
                    Avg_rho_out_kgm3        = sum_rho_out / avg_n,
                    HourlyDebugRows         = hourlyDebugRows,
                });

                totalCool    += month_cool;
                totalHeat    += month_heat;
                totalDry     += month_dry;
                totalContrib += month_contrib;
                totalWorkDays  += workDays;
                totalWorkHours += sched.WorkingHours;
            }

            // ── Seasonal totals ───────────────────────────────────────────────────
            output.TotalCoolNet_kWh  = totalCool;
            output.TotalHeatNet_kWh  = totalHeat;
            output.TotalDryNet_kWh   = totalDry;

            output.TotalCoolNet_kWhm2  = totalCool  / area;
            output.TotalHeatNet_kWhm2  = totalHeat  / area;
            output.TotalDryNet_kWhm2   = totalDry   / area;
            output.TotalVentContrib_kWhm2 = totalContrib / area;

            // Net total: охлаждане + изсушаване (heating contribution ≠ load – it reduces net demand)
            double netTotal = totalCool + totalDry;
            output.TotalNetEnergy_kWhm2 = netTotal / area;

            output.TotalWorkingDays  = totalWorkDays;
            output.TotalWorkingHours = totalWorkHours;

            // ── Energy sources (EI1/EI2) ──────────────────────────────────────────
            ApplyEnergySources(input, netTotal, area, output);

            // ── Debug input snapshot (extended) ───────────────────────────────────
            double q_total_dbg = input.AirflowSpec_m3hm2 * area;
            var ds = new System.Text.StringBuilder();
            ds.AppendLine($"qv_spec = {input.AirflowSpec_m3hm2:F4} m³/h·m²   (A={area:F0} m²  →  qv = {q_total_dbg:F1} m³/h)");
            ds.AppendLine($"Tsup = {input.SupplyTemperature_C:F1} °C   RHsup = {input.SupplyRH_Pct:F1}%   B = {input.BarometricPressure_Pa / 1000.0:F2} kPa");
            ds.AppendLine($"η_r = {input.RecuperationEfficiency * 100.0:F1}%   T_extract = {(input.ExtractAirTemperature_C.HasValue ? input.ExtractAirTemperature_C.Value.ToString("F1") : "n/a")} °C   RH_extract = {(input.ExtractAirRH_Pct.HasValue ? input.ExtractAirRH_Pct.Value.ToString("F1") : "50.0")}%");
            ds.AppendLine($"График: {input.VentSchedule.TimeRange}   Пн-Пт={input.VentSchedule.WorkdaysActive}  Съб={input.VentSchedule.SaturdayActive}  Нед={input.VentSchedule.SundayActive}");
            ds.AppendLine($"Сезон: {input.SeasonStart:dd.MM} – {input.SeasonEnd:dd.MM}");
            ds.AppendLine();
            ds.AppendLine($"Сезонни суми (kWh): totalCool={totalCool:F3}  totalDry={totalDry:F3}  totalContrib={totalContrib:F3}  netTotal={netTotal:F3}");
            ds.AppendLine($"Normalized (kWh/m2): TotalNet={output.TotalNetEnergy_kWhm2:F4}  TotalFinal={output.TotalFinalEnergy_kWhm2:F4}");
            ds.AppendLine($"EnergySource1: Share={input.EnergySource1.Share_Pct:F1}%  TotalEfficiency={input.EnergySource1.TotalEfficiency:F3}");
            if (input.EnergySource2 != null)
                ds.AppendLine($"EnergySource2: Share={input.EnergySource2.Share_Pct:F1}%  TotalEfficiency={input.EnergySource2.TotalEfficiency:F3}");

            if (netTotal <= 0.0)
                ds.AppendLine("NOTE: Нетна енергия <= 0 → няма необходима доставена енергия (ЕИ1/ЕИ2 ще бъдат 0).");

            output.DebugInputSummary = ds.ToString();

            output.IsValid = true;
            return output;
        }

        // ── Private helpers ───────────────────────────────────────────────────────

        private static void ApplyEnergySources(
            VentCoolingInputV2 input,
            double netTotal_kWh,
            double area,
            VentCoolingOutputV2 output)
        {
            if (netTotal_kWh <= 0.0) return;

            double share1 = Math.Clamp(input.EnergySource1.Share_Pct / 100.0, 0.0, 1.0);
            double eff1   = input.EnergySource1.TotalEfficiency;
            double need1  = eff1 > 0 ? netTotal_kWh * share1 / eff1 : 0.0;
            output.FinalEnergyEI1_kWhm2 = need1 / area;

            double need2 = 0.0;
            if (input.EnergySource2 != null)
            {
                double share2 = Math.Clamp(input.EnergySource2.Share_Pct / 100.0, 0.0, 1.0);
                double eff2   = input.EnergySource2.TotalEfficiency;
                need2 = eff2 > 0 ? netTotal_kWh * share2 / eff2 : 0.0;
                output.FinalEnergyEI2_kWhm2 = need2 / area;
            }

            output.TotalFinalEnergy_kWh    = need1 + need2;
            output.TotalFinalEnergy_kWhm2  = (need1 + need2) / area;
        }
    }
}
