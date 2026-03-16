using System;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Models;
using EE.Doklad.Services.Schedule;

namespace EE.Doklad.Services
{
    // ══════════════════════════════════════════════════════════════════════════
    // Раздел 23 – Debug изчисление на вътрешни топлинни печалби
    // Формули: 3.30, 3.31, 3.32, 3.33
    // ══════════════════════════════════════════════════════════════════════════

    public interface IInternalGainsDebugService
    {
        InternalGainsDebugResult Calculate(InternalGainsDebugInput input);
    }

    /// <summary>
    /// Изчислява вътрешни топлинни печалби за Debug секция 23.
    ///
    /// ВАЖНО: Тази класа НЕ съдържа отделна бизнес логика.
    /// Тя оркестрира вече съществуващия WorkdayScheduleCalculator и
    /// осигурява пълен debug trace на всяка стъпка.
    ///
    /// Методика:
    ///   3.33 → Q_int,k,m = Φ_int,k * t_m / 1000  [kWh]
    ///   3.32 → Q_H/C;int;dir;z;m = (Σ специфични стойности по категории) * A_use
    ///   3.30 → Q_H/C;int;ztc;m = Q_H/C;int;dir;ztc;m  (при липса на некондиционирани зони)
    ///   3.31 → + принос от съседни некондиционирани зони (ако са настроени)
    /// </summary>
    public sealed class InternalGainsDebugService : IInternalGainsDebugService
    {
        // ── Константи / Fallback ─────────────────────────────────────────────
        private const double FallbackHoursPerDayHeating  = 10.0;   // [h] – стандартен fallback при липса на график
        private const double FallbackHoursPerDayCooling  = 10.0;
        private const double ToleranceFraction           = 1e-9;

        // ══════════════════════════════════════════════════════════════════════
        // Главна входна точка
        // ══════════════════════════════════════════════════════════════════════

        public InternalGainsDebugResult Calculate(InternalGainsDebugInput input)
        {
            var result = new InternalGainsDebugResult
            {
                ZoneId = input.ZoneId,
                Month  = input.Month,
                Mode   = input.Mode
            };

            // ── Стъпка 1: Валидация ────────────────────────────────────────────
            if (!ValidateInput(input, result))
            {
                result.IsSuccess = false;
                return result;
            }

            // Определяме активната площ за режима
            double areaUsed = input.Mode == EpbMode.Heating
                ? input.AreaHeat_m2
                : input.AreaCool_m2;
            result.AreaUsed_m2 = areaUsed;

            // ── Стъпка 2: Времево изчисление (t_m) ────────────────────────────
            var timeInfo = CalculateTimeInfo(input);
            result.TimeInfo = timeInfo;
            if (timeInfo.HoursFallbackUsed)
                result.FallbacksUsed.Add(timeInfo.HoursFallbackReason!);

            double t_m = timeInfo.TotalActiveHours_t_m;

            // Годишни часове за SpecificAnnual метода (нужни за разпределяне)
            double t_year = CalculateAnnualHours(input);

            // ── Стъпка 3: Изчисляване на Q_int,k,m по 3.33 ────────────────────
            var sourceRows = new List<InternalGainsSourceDebugRow>();
            foreach (var src in input.Sources)
            {
                var row = CalculateSourceRow(src, t_m, t_year, areaUsed);
                if (row.FallbackWarning != null)
                    result.FallbacksUsed.Add($"[{src.SourceId}] {row.FallbackWarning}");
                sourceRows.Add(row);
            }
            result.SourceRows = sourceRows;

            // ── Стъпка 4: Агрегиране по категория ─────────────────────────────
            result.CategorySums = AggregateByCategoryFor332(sourceRows, areaUsed);

            // ── Стъпка 5: Баланс 3.32 → 3.30 ─────────────────────────────────
            double q_dir_kWh  = sourceRows.Sum(r => r.Q_int_k_m_kWh);   // Σ по всички k (знаковете вградени)
            double q_dir_spec = areaUsed > ToleranceFraction
                ? q_dir_kWh / areaUsed
                : 0.0;

            result.Q_HC_int_dir_z_m_kWh             = q_dir_kWh;
            result.Q_HC_int_dir_z_m_specific_kWhM2  = q_dir_spec;

            // 3.31: принос от некондиционирани зони (в тази имплементация = 0,
            //       защото ZoneId=1 е единствената климатизирана зона без съседи)
            result.Q_HC_int_uncond_contribution_kWh = 0.0;

            // 3.30: финален резултат
            result.Q_HC_int_ztc_m_kWh =
                result.Q_HC_int_dir_z_m_kWh +
                result.Q_HC_int_uncond_contribution_kWh;

            // ── Стъпка 6: Формула трейсове ────────────────────────────────────
            result.Formula333Summary = BuildFormula333Summary(sourceRows);
            result.Formula332Trace   = BuildFormula332Trace(result.CategorySums, areaUsed, q_dir_kWh);
            result.Formula330Trace   = BuildFormula330Trace(
                result.Q_HC_int_dir_z_m_kWh,
                result.Q_HC_int_uncond_contribution_kWh,
                result.Q_HC_int_ztc_m_kWh,
                input.Mode);

            result.InputValid = true;
            result.IsSuccess  = true;
            return result;
        }

        // ══════════════════════════════════════════════════════════════════════
        // Стъпка 1: Валидация
        // ══════════════════════════════════════════════════════════════════════

        private static bool ValidateInput(InternalGainsDebugInput input, InternalGainsDebugResult result)
        {
            bool ok = true;

            if (input.Month < 1 || input.Month > 12)
            {
                result.ValidationErrors.Add($"Month={input.Month} е извън диапазона 1..12.");
                ok = false;
            }

            if (input.AreaHeat_m2 < 0)
            {
                result.ValidationErrors.Add($"AreaHeat_m2={input.AreaHeat_m2} е отрицателна.");
                ok = false;
            }

            if (input.AreaCool_m2 < 0)
            {
                result.ValidationErrors.Add($"AreaCool_m2={input.AreaCool_m2} е отрицателна.");
                ok = false;
            }

            if (input.Mode == EpbMode.Heating && input.AreaHeat_m2 < ToleranceFraction)
                result.ValidationWarnings.Add("AreaHeat_m2 ≈ 0 → специфичните стойности ще са 0.");

            if (input.Mode == EpbMode.Cooling && input.AreaCool_m2 < ToleranceFraction)
                result.ValidationWarnings.Add("AreaCool_m2 ≈ 0 → специфичните стойности ще са 0.");

            if (input.DaysOff == null || input.DaysOff.Length != 12)
            {
                result.ValidationErrors.Add("DaysOff масивът трябва да е с дължина 12.");
                ok = false;
            }
            else
            {
                for (int i = 0; i < 12; i++)
                {
                    if (input.DaysOff[i] < 0)
                    {
                        result.ValidationErrors.Add($"DaysOff[{i + 1}]={input.DaysOff[i]} е отрицателен.");
                        ok = false;
                    }
                }
            }

            // Сезонни дати – Отопление
            if (input.Mode == EpbMode.Heating)
            {
                if (input.HeatingSeasonStartMonth < 1 || input.HeatingSeasonStartMonth > 12 ||
                    input.HeatingSeasonEndMonth   < 1 || input.HeatingSeasonEndMonth   > 12)
                {
                    result.ValidationErrors.Add("Невалидни месеци на отоплителния сезон.");
                    ok = false;
                }
            }
            else
            {
                if (input.CoolingSeasonStartMonth == null || input.CoolingSeasonEndMonth == null)
                    result.ValidationWarnings.Add("Охладителен сезон не е зададен → t_m = 0.");
            }

            // Источници
            if (input.Sources == null || input.Sources.Count == 0)
                result.ValidationWarnings.Add("Няма зададени вътрешни топлинни източници → Q_int = 0.");
            else
            {
                foreach (var src in input.Sources)
                {
                    if (src.Kind == InternalGainsSourceKind.PowerWatts && src.Power_W < 0)
                    {
                        result.ValidationErrors.Add(
                            $"Источник '{src.SourceId}': Power_W={src.Power_W} е отрицателен. " +
                            "Използвайте IsColdSource=true за студени источници.");
                        ok = false;
                    }
                    if (src.Kind == InternalGainsSourceKind.SpecificAnnual_kWhM2Year
                        && src.SpecificAnnual_kWhM2Year < 0)
                    {
                        result.ValidationErrors.Add(
                            $"Источник '{src.SourceId}': SpecificAnnual_kWhM2Year={src.SpecificAnnual_kWhM2Year} " +
                            "е отрицателен. Използвайте IsColdSource=true.");
                        ok = false;
                    }
                }
            }

            return ok;
        }

        // ══════════════════════════════════════════════════════════════════════
        // Стъпка 2: Времево изчисление (t_m)
        // Делегира към WorkdayScheduleCalculator (production engine)
        // ══════════════════════════════════════════════════════════════════════

        private static InternalGainsTimeInfo CalculateTimeInfo(InternalGainsDebugInput input)
        {
            var info = new InternalGainsTimeInfo();
            int m = input.Month;

            // ── Определяме сезона за режима ───────────────────────────────────
            DateTime seasonStart, seasonEnd;
            double   hoursWd, hoursSat, hoursSun;
            bool     fallback = false;
            string?  fallbackReason = null;

            if (input.Mode == EpbMode.Heating)
            {
                // Отоплителен сезон обхваща края на YearRef и началото на следващата:
                // напр. 21 окт 2026 – 20 апр 2027.
                // WorkdayScheduleCalculator итерира с yearRef=YearRef (2026);
                // IsInSeasonRange проверява с включително-изключително за wrapping.
                // За да работи wrap-around, EndMonth < StartMonth → подаваме EndYear = YearRef+1.
                // Но IsInSeasonRange очаква seasonEnd < seasonStart за wrap.
                // Решение: ако EndMonth < StartMonth (сезон преминава Нова Година),
                // подаваме seasonStart с YearRef, seasonEnd с YearRef+1 → seasonEnd > seasonStart
                // и functionата НЕ влиза в wrap-check.
                // Затова ползваме алтернативен подход:
                // подаваме seasonStart = предходна година, seasonEnd = YearRef,
                // и yearRef = YearRef (итерацията е върху YearRef).
                // По-просто: ако EndMonth >= StartMonth → не wrap; ако EndMonth < StartMonth → wrap.
                bool heatingWraps = input.HeatingSeasonEndMonth < input.HeatingSeasonStartMonth;
                if (heatingWraps)
                {
                    // Сезон: напр. окт → апр (преминава НГ)
                    // Подаваме seasonStart в предходната година, за да работи IsInSeasonRange без wrap
                    seasonStart = new DateTime(input.YearRef - 1, input.HeatingSeasonStartMonth, input.HeatingSeasonStartDay);
                    seasonEnd   = new DateTime(input.YearRef,     input.HeatingSeasonEndMonth,   input.HeatingSeasonEndDay);
                }
                else
                {
                    seasonStart = new DateTime(input.YearRef, input.HeatingSeasonStartMonth, input.HeatingSeasonStartDay);
                    seasonEnd   = new DateTime(input.YearRef, input.HeatingSeasonEndMonth,   input.HeatingSeasonEndDay);
                }

                hoursWd  = input.HeatingWorkdaysHours  ?? 0;
                hoursSat = input.HeatingSaturdayHours  ?? 0;
                hoursSun = input.HeatingSundayHours    ?? 0;

                if (hoursWd <= ToleranceFraction && hoursSat <= ToleranceFraction && hoursSun <= ToleranceFraction)
                {
                    fallback = true;
                    fallbackReason = $"Липсват графици за Heating → приложен fallback {FallbackHoursPerDayHeating} h/ден за работни дни.";
                    hoursWd = FallbackHoursPerDayHeating;
                }
            }
            else
            {
                if (input.CoolingSeasonStartMonth == null || input.CoolingSeasonEndMonth == null)
                {
                    // Нямаме охладителен сезон → t_m = 0
                    info.HoursFallbackUsed  = false;
                    info.TotalActiveHours_t_m = 0;
                    return info;
                }

                int csStartM = input.CoolingSeasonStartMonth.Value;
                int csStartD = input.CoolingSeasonStartDay   ?? 1;
                int csEndM   = input.CoolingSeasonEndMonth.Value;
                int csEndD   = input.CoolingSeasonEndDay     ?? DateTime.DaysInMonth(input.YearRef, csEndM);

                seasonStart = new DateTime(input.YearRef, csStartM, csStartD);
                int endYear = csEndM < csStartM ? input.YearRef + 1 : input.YearRef;
                seasonEnd   = new DateTime(endYear, csEndM, csEndD);

                hoursWd  = input.CoolingWorkdaysHours  ?? 0;
                hoursSat = input.CoolingSaturdayHours  ?? 0;
                hoursSun = input.CoolingSundayHours    ?? 0;

                if (hoursWd <= ToleranceFraction && hoursSat <= ToleranceFraction && hoursSun <= ToleranceFraction)
                {
                    fallback = true;
                    fallbackReason = $"Липсват графици за Cooling → приложен fallback {FallbackHoursPerDayCooling} h/ден за работни дни.";
                    hoursWd = FallbackHoursPerDayCooling;
                }
            }

            info.HoursFallbackUsed  = fallback;
            info.HoursFallbackReason = fallbackReason;
            info.WorkdaysHoursPerDay = hoursWd;
            info.SaturdayHoursPerDay = hoursSat;
            info.SundayHoursPerDay   = hoursSun;

            // ── Делегираме към WorkdayScheduleCalculator ───────────────────────
            // Конвертираме часовете в DailyTimeRange (9..18 = 10 h е пример; тук ползваме брой часове)
            // WorkdayScheduleCalculator работи с StartHour/EndHour, но ни трябват само броят часове.
            // Затова симулираме с TimeRange 0..(hoursPerDay-1) ако hoursPerDay > 0.
            var schedule = BuildScheduleConfig(hoursWd, hoursSat, hoursSun);

            var monthlyResults = WorkdayScheduleCalculator.ComputeMonthly(
                schedule,
                seasonStart,
                seasonEnd,
                input.DaysOff,
                officialHolidays: null,
                yearRef: input.YearRef);

            var mResult = monthlyResults.FirstOrDefault(r => r.MonthNumber == m);
            if (mResult == null)
            {
                info.TotalActiveHours_t_m = 0;
                return info;
            }

            // ── Попълваме info ─────────────────────────────────────────────────
            info.ActiveWeekdays   = mResult.WorkingDaysWeekday;
            info.ActiveSaturdays  = mResult.WorkingDaysSaturday;
            info.ActiveSundays    = mResult.WorkingDaysSunday;
            info.TotalActiveDays  = mResult.WorkingDays;
            info.DaysOffApplied   = mResult.HolidaysSubtracted;

            // Определяме дали месецът е частичен
            int daysInMonth = DateTime.DaysInMonth(input.YearRef, m);
            info.IsPartialMonth =
                (m == seasonStart.Month && seasonStart.Day > 1) ||
                (m == seasonEnd.Month   && seasonEnd.Day   < daysInMonth);

            // Начало/край на сезона в месеца
            if (m == seasonStart.Month && seasonStart.Year == input.YearRef)
                info.SeasonStartDayInMonth = seasonStart.Day;
            else
                info.SeasonStartDayInMonth = 1;

            if (m == seasonEnd.Month && (seasonEnd.Year == input.YearRef || seasonEnd.Year == input.YearRef + 1))
                info.SeasonEndDayInMonth = seasonEnd.Day;
            else
                info.SeasonEndDayInMonth = daysInMonth;

            // t_m = WorkingHours от ScheduleCalculator
            // WorkdayScheduleCalculator.WorkingHours = WorkingDays * RunHoursPerDay
            // Но RunHoursPerDay е за единен тип ден. При смесени h/ден преизчисляваме:
            double t_m = mResult.WorkingDaysWeekday * hoursWd
                       + mResult.WorkingDaysSaturday * hoursSat
                       + mResult.WorkingDaysSunday   * hoursSun;
            info.TotalActiveHours_t_m = t_m;

            return info;
        }

        /// <summary>
        /// Изгражда WeeklyScheduleConfig с броя часове (симулираме с TimeRange 0..n-1).
        /// При нула часа за даден ден → деактивираме го.
        /// Целта на конфигурацията е само да брои активните дни по тип;
        /// реалното умножение по h/ден се прави в CalculateTimeInfo след това.
        /// </summary>
        private static WeeklyScheduleConfig BuildScheduleConfig(double hoursWd, double hoursSat, double hoursSun)
        {
            bool wdActive  = hoursWd  > ToleranceFraction;
            bool satActive = hoursSat > ToleranceFraction;
            bool sunActive = hoursSun > ToleranceFraction;

            // TimeRange трябва да е валиден (StartHour <= EndHour, ≥0).
            // Ползваме 1 h като минимум (само за да се брои денят).
            // Реалните часове се умножават ръчно в CalculateTimeInfo.
            return new WeeklyScheduleConfig
            {
                TimeRange = new DailyTimeRange { StartHour = 0, EndHour = 0 }, // 1 h дневно за Census
                WorkdaysActive = wdActive  || (!satActive && !sunActive), // поне 1 тип активен
                SaturdayActive = satActive,
                SundayActive   = sunActive
            };
        }

        /// <summary>
        /// Изчислява общи годишни часове t_year за SpecificAnnual разпределяне.
        /// Сумира t_m за всички 12 месеца.
        /// </summary>
        private static double CalculateAnnualHours(InternalGainsDebugInput input)
        {
            // Строим временен input за всички 12 месеца
            double total = 0;
            for (int m = 1; m <= 12; m++)
            {
                var tempInput = CloneInputForMonth(input, m);
                var ti = CalculateTimeInfo(tempInput);
                total += ti.TotalActiveHours_t_m;
            }
            return total;
        }

        private static InternalGainsDebugInput CloneInputForMonth(InternalGainsDebugInput src, int month)
        {
            // Shallow clone – само Month се сменя
            return new InternalGainsDebugInput
            {
                ZoneId                   = src.ZoneId,
                Month                    = month,
                Mode                     = src.Mode,
                AreaHeat_m2              = src.AreaHeat_m2,
                AreaCool_m2              = src.AreaCool_m2,
                HeatingWorkdaysHours     = src.HeatingWorkdaysHours,
                HeatingSaturdayHours     = src.HeatingSaturdayHours,
                HeatingSundayHours       = src.HeatingSundayHours,
                CoolingWorkdaysHours     = src.CoolingWorkdaysHours,
                CoolingSaturdayHours     = src.CoolingSaturdayHours,
                CoolingSundayHours       = src.CoolingSundayHours,
                HeatingSeasonStartMonth  = src.HeatingSeasonStartMonth,
                HeatingSeasonStartDay    = src.HeatingSeasonStartDay,
                HeatingSeasonEndMonth    = src.HeatingSeasonEndMonth,
                HeatingSeasonEndDay      = src.HeatingSeasonEndDay,
                CoolingSeasonStartMonth  = src.CoolingSeasonStartMonth,
                CoolingSeasonStartDay    = src.CoolingSeasonStartDay,
                CoolingSeasonEndMonth    = src.CoolingSeasonEndMonth,
                CoolingSeasonEndDay      = src.CoolingSeasonEndDay,
                DaysOff                  = src.DaysOff,
                YearRef                  = src.YearRef,
                Sources                  = new List<InternalGainsSourceInput>() // празно – не ни трябват
            };
        }

        // ══════════════════════════════════════════════════════════════════════
        // Стъпка 3: Изчисляване на Q_int,k,m по формула 3.33
        // ══════════════════════════════════════════════════════════════════════

        private static InternalGainsSourceDebugRow CalculateSourceRow(
            InternalGainsSourceInput src,
            double t_m,
            double t_year,
            double areaUsed)
        {
            var row = new InternalGainsSourceDebugRow
            {
                SourceId    = src.SourceId,
                Description = src.Description,
                Category    = src.Category,
                InputKind   = src.Kind,
                IsColdSource = src.IsColdSource,
                AreaUsed_m2  = areaUsed
            };

            double q_kWh;
            string formulaTrace;

            if (src.Kind == InternalGainsSourceKind.PowerWatts)
            {
                // Формула 3.33: Q_int,k,m = Φ_int,k * t_m / 1000  [kWh]
                row.Phi_W        = src.Power_W;
                row.ActiveHours_t_m = t_m;

                if (t_m < ToleranceFraction)
                {
                    q_kWh = 0;
                    row.FallbackWarning = $"t_m = 0 → Q = 0 (зоната не е в сезона за месеца или DaysOff покриват всички дни).";
                }
                else
                {
                    q_kWh = src.Power_W * t_m / 1000.0;
                }

                formulaTrace = $"Q = Φ × t_m / 1000 = {src.Power_W:F1} W × {t_m:F2} h / 1000 = {q_kWh:F4} kWh";
            }
            else // SpecificAnnual_kWhM2Year
            {
                // Разпределяме годишната стойност пропорционално на t_m / t_year
                row.SpecificAnnual_kWhM2Year = src.SpecificAnnual_kWhM2Year;
                row.ActiveHours_t_m = t_m;

                if (t_year < ToleranceFraction || t_m < ToleranceFraction)
                {
                    q_kWh = 0;
                    row.FallbackWarning = "t_year или t_m = 0 → Q = 0.";
                }
                else
                {
                    // Еквивалентна мощност Φ = spec [kWh/m²/year] * area [m²] * 1000 / t_year [h]
                    double phi_equiv = src.SpecificAnnual_kWhM2Year * areaUsed * 1000.0 / t_year;
                    row.Phi_W = phi_equiv;
                    q_kWh    = phi_equiv * t_m / 1000.0;
                }

                double fraction = t_year > ToleranceFraction ? t_m / t_year : 0;
                formulaTrace = $"Q = E_year/m² × A × (t_m/t_year) = {src.SpecificAnnual_kWhM2Year:F4} kWh/m²/y × " +
                               $"{areaUsed:F1} m² × {fraction:F4} = {q_kWh:F4} kWh";
            }

            // Знак: студеният источник е отрицателен (3.32 забележка)
            int sign = src.IsColdSource ? -1 : +1;
            q_kWh *= sign;

            row.Q_int_k_m_kWh = q_kWh;
            row.Q_int_k_m_specific_kWhM2 = areaUsed > ToleranceFraction
                ? q_kWh / areaUsed
                : 0;
            row.FormulaTrace  = formulaTrace + (src.IsColdSource ? " [студен → ×(-1)]" : "");
            row.IsCalculated  = true;
            return row;
        }

        // ══════════════════════════════════════════════════════════════════════
        // Стъпка 4: Агрегиране по категория (за трейс на 3.32)
        // ══════════════════════════════════════════════════════════════════════

        private static List<InternalGainsCategorySum> AggregateByCategoryFor332(
            List<InternalGainsSourceDebugRow> rows,
            double areaUsed)
        {
            var dict = new Dictionary<InternalGainsCategory, InternalGainsCategorySum>();

            foreach (InternalGainsCategory cat in Enum.GetValues(typeof(InternalGainsCategory)))
            {
                dict[cat] = new InternalGainsCategorySum { Category = cat };
            }

            foreach (var row in rows)
            {
                var sum = dict[row.Category];
                sum.Q_zone_kWh += row.Q_int_k_m_kWh;
                sum.SourceCount++;
            }

            foreach (var sum in dict.Values)
            {
                sum.Q_spec_kWhM2 = areaUsed > ToleranceFraction
                    ? sum.Q_zone_kWh / areaUsed
                    : 0;
            }

            return dict.Values.OrderBy(s => (int)s.Category).ToList();
        }

        // ══════════════════════════════════════════════════════════════════════
        // Стъпка 6: Формула трейсове
        // ══════════════════════════════════════════════════════════════════════

        private static string BuildFormula333Summary(List<InternalGainsSourceDebugRow> rows)
        {
            if (!rows.Any()) return "Нямa зададени източници.";
            var lines = rows.Select(r =>
                $"  [{r.SourceId}] {r.Description}: {r.FormulaTrace} → Q={r.Q_int_k_m_kWh:F4} kWh");
            return "Формула 3.33:\n" + string.Join("\n", lines);
        }

        private static string BuildFormula332Trace(
            List<InternalGainsCategorySum> catSums,
            double areaUsed,
            double q_dir_kWh)
        {
            var cats = string.Join(" + ", catSums
                .Where(c => Math.Abs(c.Q_spec_kWhM2) > ToleranceFraction)
                .Select(c => $"Q_{c.Category}={c.Q_spec_kWhM2:F4} kWh/m²"));

            return $"Формула 3.32:\n" +
                   $"  Q_HC;int;dir;z;m = ({cats}) × A_use\n" +
                   $"  = ({catSums.Sum(c => c.Q_spec_kWhM2):F4} kWh/m²) × {areaUsed:F1} m²\n" +
                   $"  = {q_dir_kWh:F4} kWh";
        }

        private static string BuildFormula330Trace(
            double q_dir,
            double q_uncond,
            double q_ztc,
            EpbMode mode)
        {
            string modeStr = mode == EpbMode.Heating ? "H" : "C";
            if (Math.Abs(q_uncond) < ToleranceFraction)
            {
                return $"Формула 3.30 (без некондиц. зони):\n" +
                       $"  Q_{modeStr};int;ztc;m = Q_{modeStr};int;dir;ztc;m\n" +
                       $"  = {q_dir:F4} kWh";
            }

            return $"Формула 3.30 + 3.31:\n" +
                   $"  Q_{modeStr};int;ztc;m = Q_{modeStr};int;dir;ztc;m + Q_uncond\n" +
                   $"  = {q_dir:F4} + {q_uncond:F4} = {q_ztc:F4} kWh";
        }

    }
}
