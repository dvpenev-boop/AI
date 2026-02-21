using System;
using EE.Doklad.Models;
using EE.Doklad.Services.Schedule;

namespace EE.Doklad.Services.VentCooling
{
    // ─────────────────────────────────────────────────────────────────────────────
    // VentCoolingContributionInput
    //
    // Входен DTO за VentCoolingContributionCalculator.
    // Всички полета идват директно от UI / сформирани в ViewModel-а.
    // ─────────────────────────────────────────────────────────────────────────────

    public sealed class VentCoolingContributionInput
    {
        // ── От „Основни параметри" (Секция 14) ───────────────────────────────────

        /// <summary>
        /// Специфичен дебит на приточен въздух [m³/h·m²]  (q_spec).
        /// Съответства на UI полето „Специфичен дебит на приточен въздух".
        /// Трябва да е ≥ 0.
        /// </summary>
        public double Airflow_m3ph_per_m2 { get; init; }

        /// <summary>
        /// Температура на подавания въздух [°C]  (T_supply).
        /// Съответства на UI полето „Температура на подавания въздух".
        /// </summary>
        public double SupplyAirTemp_C { get; init; }

        /// <summary>
        /// Общ брой работни часове за сезона [h]  (VentSeasonHours).
        /// Директно от UI полето „Общ брой работни часове за сезона" (Секция 14).
        /// НЕ се преизчислява – приема се като готова стойност.
        /// Трябва да е ≥ 0.
        /// </summary>
        public double TotalWorkHoursSeason { get; init; }

        // ── От „Ръчни параметри" (Секция 12 / Cooling) ───────────────────────────

        /// <summary>
        /// Проектна температура на помещението [°C]  (T_room_design).
        /// Съответства на UI полето „Проектна температура".
        /// </summary>
        public double RoomTemp_Design_C { get; init; }

        /// <summary>
        /// Температура с повишение [°C]  (T_room_raised).
        /// Съответства на UI полето „Температура с повишение".
        /// </summary>
        public double RoomTemp_Raised_C { get; init; }

        // ── Площ ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Охлаждаема площ [m²]  (A_zone).
        /// Може да е 0 → Net_kWh = 0, но Net_kWhm2 се изчислява нормално.
        /// </summary>
        public double CoolingArea_m2 { get; init; }

        // ── Графици (Секция 5) ────────────────────────────────────────────────────

        /// <summary>
        /// B) График за охлаждане (CoolingSchedule) – три типа дни.
        /// </summary>
        public WeeklySchedule? CoolingSchedule { get; init; }

        /// <summary>
        /// C) График за вентилация охлаждане (VentCoolingSchedule) – три типа дни.
        /// </summary>
        public WeeklySchedule? VentCoolingSchedule { get; init; }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // VentCoolingContributionCalculator
    //
    // Изчислява „Принос към охлаждането от вентилация (нетен)" по формулата:
    //
    //   BaseFactor          = ρCp · q_spec · H_season / 1000    [kWh/m²·K]
    //                         (ρCp = 0.34 Wh/(m³·K))
    //
    //   ScenarioDesign      = BaseFactor × (T_design  − T_supply)  [kWh/m²]
    //   ScenarioRaised      = BaseFactor × (T_raised  − T_supply)  [kWh/m²]
    //
    //   f_on                = WeeklyOverlapHours(Cooling ∩ Vent) / WeeklyVentHours
    //                         (clamp 0..1; wrap-around поддържан)
    //
    //   Net_kWhm2           = f_on · ScenarioDesign + (1−f_on) · ScenarioRaised
    //   Net_kWh             = Net_kWhm2 × CoolingArea_m2
    //
    // Знакова конвенция: резултатът може да е < 0 (не се клипва).
    // ─────────────────────────────────────────────────────────────────────────────

    public static class VentCoolingContributionCalculator
    {
        /// <summary>ρ·Cp за въздух [Wh/(m³·K)]</summary>
        public const double RhoCpAir_Whm3K = 0.34;

        /// <summary>
        /// Изчислява нетния принос към охлаждането от вентилация.
        /// </summary>
        /// <param name="input">Входен DTO.</param>
        /// <returns>Резултатен DTO с всички стъпки и крайните стойности.</returns>
        public static VentCoolingContributionResult Calculate(VentCoolingContributionInput input)
        {
            var result = new VentCoolingContributionResult();

            // ── Валидации ─────────────────────────────────────────────────────────
            if (input is null)
            {
                result.IsValid = false;
                result.ErrorMessage = "Липсват входни данни.";
                return result;
            }

            if (input.Airflow_m3ph_per_m2 < 0.0)
            {
                result.IsValid = false;
                result.ErrorMessage = $"Специфичният дебит трябва да е ≥ 0 (получено: {input.Airflow_m3ph_per_m2}).";
                return result;
            }

            if (input.TotalWorkHoursSeason < 0.0)
            {
                result.IsValid = false;
                result.ErrorMessage = $"Работните часове за сезона трябва да са ≥ 0 (получено: {input.TotalWorkHoursSeason}).";
                return result;
            }

            if (input.CoolingArea_m2 < 0.0)
            {
                result.IsValid = false;
                result.ErrorMessage = $"Охлаждаемата площ трябва да е ≥ 0 (получено: {input.CoolingArea_m2}).";
                return result;
            }

            // ── f_on (коефициент на застъпване на графиците) ───────────────────────
            double f_on = 1.0; // По подразбиране: вентилацията е изцяло в режим на охлаждане

            if (input.VentCoolingSchedule is null || input.CoolingSchedule is null)
            {
                result.Warnings.Add("Липсва един или двата графика (Cooling / VentCooling) → f_on = 1.0 (консервативно).");
            }
            else
            {
                f_on = OverlapCalculator.ComputeFon(
                    input.VentCoolingSchedule,
                    input.CoolingSchedule,
                    out string? fonWarning);

                if (fonWarning is not null)
                    result.Warnings.Add(fonWarning);
            }

            result.F_on = f_on;

            // ── BaseFactor [kWh/m²·K] ─────────────────────────────────────────────
            // BaseFactor = ρCp [Wh/m³K] × q [m³/h·m²] × H_season [h] / 1000
            double baseFactor = RhoCpAir_Whm3K * input.Airflow_m3ph_per_m2 * input.TotalWorkHoursSeason / 1000.0;
            result.BaseFactor = baseFactor;

            // ── Сценарии [kWh/m²] ────────────────────────────────────────────────
            double designDelta  = input.RoomTemp_Design_C - input.SupplyAirTemp_C;
            double raisedDelta  = input.RoomTemp_Raised_C - input.SupplyAirTemp_C;

            double scenarioDesign = baseFactor * designDelta;
            double scenarioRaised = baseFactor * raisedDelta;

            result.ScenarioDesign_kWhm2 = scenarioDesign;
            result.ScenarioRaised_kWhm2 = scenarioRaised;
            result.Min_kWhm2 = Math.Min(scenarioDesign, scenarioRaised);
            result.Max_kWhm2 = Math.Max(scenarioDesign, scenarioRaised);

            // ── Нетен принос (kWh/m²) ─────────────────────────────────────────────
            // Net = f_on · ScenarioDesign + (1 − f_on) · ScenarioRaised
            // Не се клипва — може да е отрицателно.
            double net_kWhm2 = f_on * scenarioDesign + (1.0 - f_on) * scenarioRaised;
            result.Net_kWhm2 = net_kWhm2;

            // ── Абсолютна стойност [kWh] ──────────────────────────────────────────
            result.Net_kWh = (input.CoolingArea_m2 > 0.0) ? net_kWhm2 * input.CoolingArea_m2 : 0.0;

            result.IsValid = true;
            return result;
        }
    }
}
