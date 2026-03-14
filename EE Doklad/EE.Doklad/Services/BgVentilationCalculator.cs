using System;
using System.Collections.Generic;
using System.Globalization;
using EE.Doklad.Models;

namespace EE.Doklad.Services
{
    /// <summary>
    /// Калкулатор за вентилация по българска методология
    /// Наредба RD-02-20-3, Секция 12 - Вентилация
    /// </summary>
    public class BgVentilationCalculator
    {
        private static readonly string[] MonthNames = new[]
        {
            "Януари", "Февруари", "Март", "Април", "Май", "Юни",
            "Юли", "Август", "Септември", "Октомври", "Ноември", "Декември"
        };

    private const double AirDensity_kg_m3 = 1.2; // Плътност на въздуха [kg/m³]
    private const double AirSpecificHeat_Wh_kgK = 0.28; // Специфичен топлинен капацитет [Wh/(kg·K)]
    // Use explicit rho*c per official software (Wh/(m3·K))
    private const double RhoCp_Wh_per_m3K = 0.34;

        /// <summary>
        /// Изчислява вентилационната енергия по българска методология
        /// </summary>
        /// <param name="data">Входни данни за вентилация</param>
        /// <param name="climateData">Климатични данни (месечни температури)</param>
        /// <returns>Резултат от изчислението</returns>
        public VentilationCalculationResult Calculate(
            VentilationSectionData data,
            ClimateZoneData? climateData,
            int[]? monthlyDaysOff = null)
        {
            var result = new VentilationCalculationResult
            {
                Methodology = VentilationMethodology.BG,
                IsValid = true
            };

            // Валидация
            if (!ValidateInputs(data, climateData, result))
            {
                return result;
            }

            // Входни параметри
            result.HeatedArea_m2 = data.HeatedArea_m2;
            result.AirflowRatePerM2 = data.AirflowRatePerM2;
            result.OperatingHoursPerWeek = data.OperatingHoursPerWeek;

            // Стъпка 1: Изчисляване на коефициента на вентилационна загуба Hᵥₑ
            // Наредба RD-02-20-3, Section 12, formula (3.27)
            double totalAirflow_m3_h = data.AirflowRatePerM2 * data.HeatedArea_m2;
            double hVe_WK = CalculateVentilationLossCoefficient(totalAirflow_m3_h);
            result.VentilationLossCoefficient_WK = hVe_WK;
            result.Assumptions.Add($"Hᵥₑ (формула 3.27) = {hVe_WK:F2} W/K");

            // Стъпка 2-4: Месечни изчисления
            result.MonthlyResults = CalculateMonthlyResults(
                data,
                climateData!,
                hVe_WK,
                monthlyDaysOff);

            // Стъпка 5: Годишни резултати
            double annualEnergy_kWh = 0;
            foreach (var monthly in result.MonthlyResults)
            {
                annualEnergy_kWh += monthly.VentilationHeatingEnergy_kWh;
            }

            result.AnnualVentilationHeatingEnergy_kWh_a = annualEnergy_kWh;
            result.SpecificVentilationHeatingEnergy_kWh_m2a =
                data.HeatedArea_m2 > 0 ? annualEnergy_kWh / data.HeatedArea_m2 : 0;

            result.Assumptions.Add($"Годишна вентилационна енергия за отопление = {annualEnergy_kWh:F2} kWh/a");
            result.Assumptions.Add($"Специфична вентилационна енергия = {result.SpecificVentilationHeatingEnergy_kWh_m2a:F2} kWh/m²·a");

            // Стъпка 5б: Изчисляване на нетен принос към отоплението от вентилация (нова функционалност)
            CalculateNetHeatingContribution(data, result);

            // Стъпка 6: Изчисляване на крайна енергия с ефективности
            CalculateFinalEnergy(data, result);

            return result;
        }

        /// <summary>
        /// Валидира входните данни
        /// </summary>
        private bool ValidateInputs(
            VentilationSectionData data,
            ClimateZoneData? climateData,
            VentilationCalculationResult result)
        {
            if (data == null)
            {
                result.IsValid = false;
                result.ErrorMessage = "Липсват входни данни за вентилация.";
                return false;
            }

            if (climateData == null)
            {
                result.IsValid = false;
                result.ErrorMessage = "Липсват климатични данни.";
                return false;
            }

            if (data.HeatedArea_m2 <= 0)
            {
                result.IsValid = false;
                result.ErrorMessage = "Отопляемата площ трябва да бъде положителна.";
                return false;
            }

            if (data.AirflowRatePerM2 < 0)
            {
                result.IsValid = false;
                result.ErrorMessage = "Дебитът не може да бъде отрицателен.";
                return false;
            }

            if (data.OperatingHoursPerWeek < 0 || data.OperatingHoursPerWeek > 168)
            {
                result.IsValid = false;
                result.ErrorMessage = "Работният режим трябва да бъде между 0 и 168 h/week.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Изчислява коефициента на вентилационна загуба Hᵥₑ [W/K]
        /// Наредба RD-02-20-3, Section 12, formula (3.27)
        /// Hᵥₑ = ρ × c × V̇
        /// където:
        ///   ρ = плътност на въздуха [kg/m³]
        ///   c = специфичен топлинен капацитет [Wh/(kg·K)]
        ///   V̇ = въздушен дебит [m³/h]
        /// </summary>
        private double CalculateVentilationLossCoefficient(double airflow_m3_h)
        {
            // Наредба RD-02-20-3, Section 12, formula (3.27)
            // Hve = rho*c * Vdot  (use explicit rho*c value to match official software)
            double hVe = RhoCp_Wh_per_m3K * airflow_m3_h;
            return hVe;
        }

        /// <summary>
        /// Изчислява температурата на подаване θₛᵤₚ с рекуперация
        /// Наредба RD-02-20-3, Section 12, item 3.5
        /// </summary>
        private double CalculateSupplyTemperature(
            double outdoorTemp_C,
            double indoorTemp_C,
            double firstStageEfficiency,
            double secondStageEfficiency,
            double maxTempDiffSecondStage,
            double minExhaustTemp_C)
        {
            // Наредба RD-02-20-3, Section 12, item 3.5
            
            // Температура след първа степен на рекуперация
            double tempAfterFirstStage = outdoorTemp_C + 
                (firstStageEfficiency / 100.0) * (indoorTemp_C - outdoorTemp_C);

            // Температура след втора степен на рекуперация (ако има)
            double tempAfterSecondStage = tempAfterFirstStage +
                (secondStageEfficiency / 100.0) * Math.Min(maxTempDiffSecondStage, indoorTemp_C - tempAfterFirstStage);

            // Ограничения
            double supplyTemp = tempAfterSecondStage;

            // Минимална температура на подаване (не може да бъде под минималната температура на отработения въздух)
            if (supplyTemp < minExhaustTemp_C)
            {
                supplyTemp = minExhaustTemp_C;
            }

            // NOTE: Clamp to indoor temperature REMOVED - supply temp can be > Ti (heating contribution) or < Ti (loss)

            return supplyTemp;
        }

        /// <summary>
        /// Compute the post-heat-recovery temperature (temperature after HR stages,
        /// before any minimum exhaust-air clamp). This is used to compute the
        /// energy required to heat air from post-HR to Tsup.
        /// </summary>
        private double CalculatePostHeatRecoveryTemperature(
            double outdoorTemp_C,
            double indoorTemp_C,
            double firstStageEfficiency,
            double secondStageEfficiency,
            double maxTempDiffSecondStage)
        {
            // First stage
            double tempAfterFirstStage = outdoorTemp_C +
                (firstStageEfficiency / 100.0) * (indoorTemp_C - outdoorTemp_C);

            // Second stage (limited by maximum allowed temperature rise in second stage)
            double tempAfterSecondStage = tempAfterFirstStage +
                (secondStageEfficiency / 100.0) * Math.Min(maxTempDiffSecondStage, indoorTemp_C - tempAfterFirstStage);

            return tempAfterSecondStage;
        }

        /// <summary>
        /// Изчислява коефициента на температурен контрол bᵥₑ,ₖ
        /// Наредба RD-02-20-3, Section 12, formula (3.28)
        /// bᵥₑ,ₖ = (θᵢ - θₛᵤₚ) / (θᵢ - θₑ)
        /// </summary>
        private double CalculateTemperatureControlCoefficient(
            double indoorTemp_C,
            double supplyTemp_C,
            double outdoorTemp_C)
        {
            // Наредба RD-02-20-3, Section 12, formula (3.28)
            double denominator = indoorTemp_C - outdoorTemp_C;

            if (Math.Abs(denominator) < 0.001)
            {
                // Ако няма температурна разлика, коефициентът е 0
                return 0;
            }

            double bVe = (indoorTemp_C - supplyTemp_C) / denominator;

            // NOTE: Do not clamp bVe to [0,1] — return the raw value (can be negative or >1)
            return bVe;
        }

        /// <summary>
        /// Изчислява нетен принос към отоплението от вентилация
        /// Положителен = принос (Tsup > Ti, намалява отоплителна нужда)
        /// Отрицателен = загуба (Tsup < Ti, увеличава отоплителна нужда)
        /// </summary>
        private void CalculateNetHeatingContribution(
            VentilationSectionData data,
            VentilationCalculationResult result)
        {
            // Compute signed net contribution as the sum of monthly contributions so the
            // annual value equals the debug/monthly breakdown shown in the UI.

            double totalAirflow_m3ph = data.AirflowRatePerM2 * data.HeatedArea_m2;
            double netContribution_kWh = 0.0;

            foreach (var monthly in result.MonthlyResults)
            {
                double supplyTemp_C = monthly.SupplyTemperature_C;
                double indoorTemp_C = monthly.IndoorTemperature_C;

                // Use the monthly operating time already computed for this month
                // (this takes heating-season days into account).
                double hours_m = monthly.MonthlyOperatingTime_h;

                // Use Hve (ventilation loss coefficient) to compute signed contribution
                double hVe_m_WK = monthly.VentilationLossCoefficient_WK;
                // Q_contrib_m = Hve * (Tsup - Ti) * hours_m / 1000
                double Q_contrib_m_kWh = hVe_m_WK * (supplyTemp_C - indoorTemp_C) * hours_m / 1000.0;

                netContribution_kWh += Q_contrib_m_kWh;
            }

            result.VentilationHeatingNetContribution_kWh = netContribution_kWh;
            result.VentilationHeatingNetContribution_kWh_m2a =
                data.HeatedArea_m2 > 0 ? netContribution_kWh / data.HeatedArea_m2 : 0;

            result.Assumptions.Add($"Нетен принос към отоплението от вентилация = {netContribution_kWh:F2} kWh/a ({result.VentilationHeatingNetContribution_kWh_m2a:F2} kWh/m²·a)");
        }

        /// <summary>
        /// Връща броя дни от дадения месец, които попадат в отоплителния сезон, зададен в climateData.HeatingSeason
        /// Формат на HeatingSeason.Start/End: "MM-dd" (пример: "10-21").
        /// Поддържа сезони, които пресичат края на календарната година (wrap-around).
        /// </summary>
        private int GetHeatingSeasonDaysInMonth(int yearRef, int monthNumber, ClimateZoneData climateData)
        {
            if (climateData?.HeatingSeason == null || string.IsNullOrWhiteSpace(climateData.HeatingSeason.Start) || string.IsNullOrWhiteSpace(climateData.HeatingSeason.End))
            {
                return DateTime.DaysInMonth(yearRef, monthNumber);
            }

            // Parse start and end
            if (!TryParseMonthDay(climateData.HeatingSeason.Start, out int startM, out int startD) ||
                !TryParseMonthDay(climateData.HeatingSeason.End, out int endM, out int endD))
            {
                return DateTime.DaysInMonth(yearRef, monthNumber);
            }

            bool wrapsYear = endM < startM || (endM == startM && endD < startD);
            int effectiveYear = wrapsYear && monthNumber <= endM
                ? yearRef + 1
                : yearRef;
            int daysInMonth = DateTime.DaysInMonth(effectiveYear, monthNumber);
            int startMonthDays = DateTime.DaysInMonth(yearRef, startM);
            int endMonthDays = DateTime.DaysInMonth(wrapsYear ? yearRef + 1 : yearRef, endM);

            startD = Math.Min(startD, startMonthDays);
            endD = Math.Min(endD, endMonthDays);

            bool monthInSeason = wrapsYear
                ? monthNumber >= startM || monthNumber <= endM
                : monthNumber >= startM && monthNumber <= endM;

            if (!monthInSeason)
            {
                return 0;
            }

            if (startM == endM && !wrapsYear)
            {
                return Math.Max(0, endD - startD);
            }

            if (monthNumber == startM)
            {
                return Math.Max(0, daysInMonth - startD);
            }

            if (monthNumber == endM)
            {
                return Math.Max(0, Math.Min(endD, daysInMonth));
            }

            return daysInMonth;
        }

        private bool TryParseMonthDay(string s, out int month, out int day)
        {
            month = 1; day = 1;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var parts = s.Split('-');
            if (parts.Length != 2) return false;
            if (!int.TryParse(parts[0], out month)) return false;
            if (!int.TryParse(parts[1], out day)) return false;
            return true;
        }

        private bool IsDateInRange(DateTime dt, DateTime start, DateTime end)
        {
            return dt >= start && dt <= end;
        }

        /// <summary>
        /// Изчислява месечните резултати
        /// </summary>
        private List<VentilationMonthlyResult> CalculateMonthlyResults(
            VentilationSectionData data,
            ClimateZoneData climateData,
            double hVe_WK,
            int[]? monthlyDaysOff = null)
        {
            var monthlyResults = new List<VentilationMonthlyResult>();

            // Месечно работно време [h]
            // tₘ = (часове на седмица / 7) × дни в месеца
            double hoursPerDay = data.OperatingHoursPerWeek / 7.0;

            for (int month = 0; month < 12; month++)
            {
                int monthNumber = month + 1;
                string monthName = MonthNames[month];
                double outdoorTemp_C = climateData.Monthly.AvgMonthlyTempC[month];
                double indoorTemp_C = data.IndoorTemperature_C;

                // Дни в отоплителния сезон за този месец (вземаме под внимание отоплителния период на климатичната зона)
                int daysInMonth = GetHeatingSeasonDaysInMonth(2024, monthNumber, climateData);

                // If the caller supplied monthly days-off (from section 5), subtract the
                // number of holidays that fall within the heating-season portion of the month.
                // We only have a count of holidays per month (not dates). The rule used here
                // follows the product requirement: if the month has any heating-season days
                // (even partially), assume holidays entered for that month fall inside the
                // heating season and subtract them from the heating-season days. Do not
                // subtract more days than the heating-season days in the month.
                if (monthlyDaysOff != null && monthlyDaysOff.Length >= 12)
                {
                    int holidays = Math.Max(0, monthlyDaysOff[month]); // month is zero-based index
                    if (holidays > 0 && daysInMonth > 0)
                    {
                        daysInMonth = Math.Max(0, daysInMonth - Math.Min(holidays, daysInMonth));
                    }
                }

                // Compute monthly operating hours from heating-season days (precise fractional value)
                double monthlyOperatingTime_h = hoursPerDay * daysInMonth;

                // Стъпка 2: Изчисляване на температурата на подаване θₛᵤₚ
                // BUG FIX: Use user-entered supply temperature if explicitly provided; otherwise compute
                double supplyTemp_C;
                if (data.SupplyTemperatureIsUserDefined)
                {
                    supplyTemp_C = data.SupplyTemperature;
                }
                else
                {
                    // Compute from heat recovery efficiencies (Наредба RD-02-20-3, Section 12, item 3.5)
                    supplyTemp_C = CalculateSupplyTemperature(
                        outdoorTemp_C,
                        indoorTemp_C,
                        data.FirstStageRecuperationEfficiency,
                        data.SecondStageRecuperationEfficiency,
                        data.MaxTemperatureDifferenceSecondStage,
                        data.MinExhaustAirTemperature);
                }

                // Стъпка 3: Изчисляване на коефициента на температурен контрол bᵥₑ,ₖ
                // Наредба RD-02-20-3, Section 12, formula (3.28)
                double bVe = CalculateTemperatureControlCoefficient(
                    indoorTemp_C,
                    supplyTemp_C,
                    outdoorTemp_C);
                // Стъпка 4: Изчисляване на месечната енергия за загряване на подавания въздух (Q_airheat)
                // Q_airheat_m = Hᵥₑ × max(0, Tsup - T_afterHR) × tₘ  (kWh)
                double tAfterHR = CalculatePostHeatRecoveryTemperature(
                    outdoorTemp_C,
                    indoorTemp_C,
                    data.FirstStageRecuperationEfficiency,
                    data.SecondStageRecuperationEfficiency,
                    data.MaxTemperatureDifferenceSecondStage);

                double deltaHeat_K = Math.Max(0.0, supplyTemp_C - tAfterHR);
                double qAirHeat_m_kWh = hVe_WK * deltaHeat_K * monthlyOperatingTime_h / 1000.0;

                // Also compute signed monthly contribution (kept separate):
                // Q_contrib_m = Hᵥₑ × (Tsup - Ti) × tₘ  (kWh) — may be +/−
                double qContrib_m_kWh = hVe_WK * (supplyTemp_C - indoorTemp_C) * monthlyOperatingTime_h / 1000.0;

                monthlyResults.Add(new VentilationMonthlyResult
                {
                    MonthNumber = monthNumber,
                    MonthName = monthName,
                    OutdoorTemperature_C = outdoorTemp_C,
                    IndoorTemperature_C = indoorTemp_C,
                    SupplyTemperature_C = supplyTemp_C,
                    TemperatureControlCoefficient = bVe,
                    VentilationLossCoefficient_WK = hVe_WK,
                    MonthlyOperatingTime_h = monthlyOperatingTime_h,
                    // Keep existing field but repurpose to mean "energy for heating supply air" (non-negative)
                    VentilationHeatingEnergy_kWh = qAirHeat_m_kWh
                });
            }

            return monthlyResults;
        }

        /// <summary>
        /// Изчислява крайната енергия с отчитане на ефективностите на енергийните източници
        /// </summary>
        private void CalculateFinalEnergy(
            VentilationSectionData data,
            VentilationCalculationResult result)
        {
            double heatingEnergy_kWh = result.AnnualVentilationHeatingEnergy_kWh_a;

            // Ако няма енергия за отопление, не изчисляваме крайна енергия
            if (heatingEnergy_kWh <= 0)
            {
                result.FinalEnergySource1_kWh_a = 0;
                result.FinalEnergySource2_kWh_a = 0;
                result.TotalFinalEnergy_kWh_a = 0;
                result.SpecificFinalEnergy_kWh_m2a = 0;
                return;
            }

            // Източник 1
            double share1 = data.EnergySource1.Share / 100.0;
            double efficiency1 = data.EnergySource1.TotalEfficiency;
            double finalEnergy1 = 0;

            if (efficiency1 > 0)
            {
                finalEnergy1 = (heatingEnergy_kWh * share1) / efficiency1;
            }

            result.FinalEnergySource1_kWh_a = finalEnergy1;
            // Show a human-friendly energy carrier if available (from section 18 mapping), otherwise fall back to the energy source type
            string source1Name = data.EnergySource1.EnergyCarrier.HasValue
                ? EnergyCarrierInfo.GetByCode(data.EnergySource1.EnergyCarrier.Value)?.DisplayName ?? data.EnergySource1.Type.ToString()
                : data.EnergySource1.Type.ToString();
            result.Assumptions.Add($"Енергиен източник 1 ({source1Name}): Дял = {data.EnergySource1.Share:F1}%, Ефективност = {efficiency1:F4}, Крайна енергия = {finalEnergy1:F2} kWh/a");

            // Източник 2 (ако е активиран)
            double finalEnergy2 = 0;
            if (data.UseSecondEnergySource && data.EnergySource2 != null)
            {
                double share2 = data.EnergySource2.Share / 100.0;
                double efficiency2 = data.EnergySource2.TotalEfficiency;

                if (efficiency2 > 0)
                {
                    finalEnergy2 = (heatingEnergy_kWh * share2) / efficiency2;
                }

                result.FinalEnergySource2_kWh_a = finalEnergy2;
                string source2Name = data.EnergySource2.EnergyCarrier.HasValue
                    ? EnergyCarrierInfo.GetByCode(data.EnergySource2.EnergyCarrier.Value)?.DisplayName ?? data.EnergySource2.Type.ToString()
                    : data.EnergySource2.Type.ToString();
                result.Assumptions.Add($"Енергиен източник 2 ({source2Name}): Дял = {data.EnergySource2.Share:F1}%, Ефективност = {efficiency2:F4}, Крайна енергия = {finalEnergy2:F2} kWh/a");
            }

            // Обща крайна енергия
            result.TotalFinalEnergy_kWh_a = finalEnergy1 + finalEnergy2;
            result.SpecificFinalEnergy_kWh_m2a =
                data.HeatedArea_m2 > 0 ? result.TotalFinalEnergy_kWh_a / data.HeatedArea_m2 : 0;

            result.Assumptions.Add($"Обща крайна енергия = {result.TotalFinalEnergy_kWh_a:F2} kWh/a");
            result.Assumptions.Add($"Специфична крайна енергия = {result.SpecificFinalEnergy_kWh_m2a:F2} kWh/m²·a");
        }

        /// <summary>
        /// Изчисляване по DIN методология (НЕ Е ИМПЛЕМЕНТИРАНО)
        /// </summary>
        public VentilationCalculationResult CalculateDIN(
            VentilationSectionData data,
            ClimateZoneData? climateData)
        {
            throw new NotImplementedException("DIN методологията за вентилация не е имплементирана.");
        }
    }
}
