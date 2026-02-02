using System.Collections.Generic;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Месечен резултат за вентилационна енергия
    /// </summary>
    public class VentilationMonthlyResult
    {
        /// <summary>
        /// Номер на месеца (1-12)
        /// </summary>
        public int MonthNumber { get; set; }

        /// <summary>
        /// Име на месеца
        /// </summary>
        public string MonthName { get; set; } = string.Empty;

        /// <summary>
        /// Средна външна температура [°C]
        /// </summary>
        public double OutdoorTemperature_C { get; set; }

        /// <summary>
        /// Вътрешна референтна температура [°C]
        /// </summary>
        public double IndoorTemperature_C { get; set; }

        /// <summary>
        /// Температура на подаване след рекуперация [°C]
        /// Наредба RD-02-20-3, Section 12, item 3.5
        /// </summary>
        public double SupplyTemperature_C { get; set; }

        /// <summary>
        /// Коефициент на температурен контрол bᵥₑ,ₖ [-]
        /// Наредба RD-02-20-3, Section 12, formula (3.28)
        /// </summary>
        public double TemperatureControlCoefficient { get; set; }

        /// <summary>
        /// Коефициент на вентилационна загуба Hᵥₑ [W/K]
        /// Наредба RD-02-20-3, Section 12, formula (3.27)
        /// </summary>
        public double VentilationLossCoefficient_WK { get; set; }

        /// <summary>
        /// Месечно работно време [h]
        /// </summary>
        public double MonthlyOperatingTime_h { get; set; }

        /// <summary>
        /// Месечна вентилационна енергия за отопление [kWh]
        /// Наредба RD-02-20-3, Section 12, formula (3.26)
        /// </summary>
        public double VentilationHeatingEnergy_kWh { get; set; }
    }

    /// <summary>
    /// Резултат от изчисление на вентилация
    /// Съгласно Наредба RD-02-20-3, Секция 12
    /// </summary>
    public class VentilationCalculationResult
    {
        /// <summary>
        /// Използвана методология
        /// </summary>
        public VentilationMethodology Methodology { get; set; }

        /// <summary>
        /// Валиден ли е резултатът
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Съобщение за грешка (ако IsValid = false)
        /// </summary>
        public string? ErrorMessage { get; set; }

        // ========== ВХОДНИ ПАРАМЕТРИ ==========

        /// <summary>
        /// Отопляема площ [m²]
        /// </summary>
        public double HeatedArea_m2 { get; set; }

        /// <summary>
        /// Въздушен дебит на единица площ [m³/h·m²]
        /// </summary>
        public double AirflowRatePerM2 { get; set; }

        /// <summary>
        /// Работен режим [h/week]
        /// </summary>
        public double OperatingHoursPerWeek { get; set; }

        // ========== ИЗЧИСЛЕНИ ПАРАМЕТРИ ==========

        /// <summary>
        /// Коефициент на вентилационна загуба Hᵥₑ [W/K]
        /// Наредба RD-02-20-3, Section 12, formula (3.27)
        /// </summary>
        public double VentilationLossCoefficient_WK { get; set; }

        // ========== МЕСЕЧНИ РЕЗУЛТАТИ ==========

        /// <summary>
        /// Месечни резултати (12 месеца)
        /// </summary>
        public List<VentilationMonthlyResult> MonthlyResults { get; set; } = new();

        // ========== ГОДИШНИ РЕЗУЛТАТИ ==========

        /// <summary>
        /// Годишна вентилационна енергия за отопление [kWh/a]
        /// Сума от всички месечни Qᵥₑ,ₘ
        /// </summary>
        public double AnnualVentilationHeatingEnergy_kWh_a { get; set; }

        /// <summary>
        /// Специфична вентилационна енергия за отопление [kWh/m²·a]
        /// </summary>
        public double SpecificVentilationHeatingEnergy_kWh_m2a { get; set; }

        // ========== КРАЙНА ЕНЕРГИЯ (с ефективности) ==========

        /// <summary>
        /// Потребна крайна енергия от източник 1 [kWh/a]
        /// </summary>
        public double FinalEnergySource1_kWh_a { get; set; }

        /// <summary>
        /// Потребна крайна енергия от източник 2 [kWh/a]
        /// </summary>
        public double FinalEnergySource2_kWh_a { get; set; }

        /// <summary>
        /// Обща потребна крайна енергия [kWh/a]
        /// </summary>
        public double TotalFinalEnergy_kWh_a { get; set; }

        /// <summary>
        /// Специфична крайна енергия [kWh/m²·a]
        /// </summary>
        public double SpecificFinalEnergy_kWh_m2a { get; set; }

        // ========== ДОПЪЛНИТЕЛНА ИНФОРМАЦИЯ ==========

        /// <summary>
        /// Предположения и бележки към изчислението
        /// </summary>
        public List<string> Assumptions { get; set; } = new();
    }
}
