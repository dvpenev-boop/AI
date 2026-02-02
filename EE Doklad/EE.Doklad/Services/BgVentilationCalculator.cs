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

        /// <summary>
        /// Изчислява вентилационната енергия по българска методология
        /// </summary>
        /// <param name="data">Входни данни за вентилация</param>
        /// <param name="climateData">Климатични данни (месечни температури)</param>
        /// <returns>Резултат от изчислението</returns>
        public VentilationCalculationResult Calculate(
            VentilationSectionData data,
            ClimateZoneData? climateData)
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
                hVe_WK);

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
            double hVe = AirDensity_kg_m3 * AirSpecificHeat_Wh_kgK * airflow_m3_h;
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

            // Максималната температура на подаване не може да надвиши вътрешната температура
            if (supplyTemp > indoorTemp_C)
            {
                supplyTemp = indoorTemp_C;
            }

            return supplyTemp;
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

            // Коефициентът трябва да бъде между 0 и 1
            if (bVe < 0) bVe = 0;
            if (bVe > 1) bVe = 1;

            return bVe;
        }

        /// <summary>
        /// Изчислява месечните резултати
        /// </summary>
        private List<VentilationMonthlyResult> CalculateMonthlyResults(
            VentilationSectionData data,
            ClimateZoneData climateData,
            double hVe_WK)
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

                // Дни в месеца
                int daysInMonth = DateTime.DaysInMonth(2024, monthNumber); // Използваме 2024 като референтна година
                double monthlyOperatingTime_h = hoursPerDay * daysInMonth;

                // Стъпка 2: Изчисляване на температурата на подаване θₛᵤₚ
                // Наредба RD-02-20-3, Section 12, item 3.5
                double supplyTemp_C = CalculateSupplyTemperature(
                    outdoorTemp_C,
                    indoorTemp_C,
                    data.FirstStageRecuperationEfficiency,
                    data.SecondStageRecuperationEfficiency,
                    data.MaxTemperatureDifferenceSecondStage,
                    data.MinExhaustAirTemperature);

                // Стъпка 3: Изчисляване на коефициента на температурен контрол bᵥₑ,ₖ
                // Наредба RD-02-20-3, Section 12, formula (3.28)
                double bVe = CalculateTemperatureControlCoefficient(
                    indoorTemp_C,
                    supplyTemp_C,
                    outdoorTemp_C);

                // Стъпка 4: Изчисляване на месечната вентилационна енергия за отопление
                // Наредба RD-02-20-3, Section 12, formula (3.26)
                // Qᵥₑ,ₘ = Hᵥₑ × bᵥₑ,ₖ × (θᵢ - θₑ) × tₘ
                double qVe_m_Wh = 0;
                
                // Изчисляваме само когато θᵢ > θₑ (отоплителен режим)
                if (indoorTemp_C > outdoorTemp_C)
                {
                    qVe_m_Wh = hVe_WK * bVe * (indoorTemp_C - outdoorTemp_C) * monthlyOperatingTime_h;
                }

                double qVe_m_kWh = qVe_m_Wh / 1000.0;

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
                    VentilationHeatingEnergy_kWh = qVe_m_kWh
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
            result.Assumptions.Add($"Енергиен източник 1 ({data.EnergySource1.Type}): Дял = {data.EnergySource1.Share:F1}%, Ефективност = {efficiency1:F4}, Крайна енергия = {finalEnergy1:F2} kWh/a");

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
                result.Assumptions.Add($"Енергиен източник 2 ({data.EnergySource2.Type}): Дял = {data.EnergySource2.Share:F1}%, Ефективност = {efficiency2:F4}, Крайна енергия = {finalEnergy2:F2} kWh/a");
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
