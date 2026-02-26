namespace EE.Doklad.Models
{
    // ══════════════════════════════════════════════════════════════════════════
    // Раздел 23 – Автоматично месечно изчисление на вътрешни топлинни печалби
    // Формули 3.32 и 3.33  •  EN ISO 52016-1
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Компонент на вътрешните топлинни печалби (отговаря на 6-те члена на 3.32).
    /// </summary>
    public enum GainComponent
    {
        /// <summary>Обитатели – Q_spec;int;oc</summary>
        Oc,
        /// <summary>Уреди – Q_spec;int;A</summary>
        A,
        /// <summary>Осветление – Q_spec;int;L</summary>
        L,
        /// <summary>Топла вода / ВиК загуби – Q_spec;int;WA</summary>
        WA,
        /// <summary>Помпи и вентилатори – Q_spec;int;HVAC</summary>
        HVAC,
        /// <summary>Процесна топлина – Q_spec;int;proc</summary>
        Proc
    }

    /// <summary>
    /// Ред в месечната таблица на вътрешните топлинни печалби.
    /// Съответства на един ред „Месец | Oc | A | L | WA | HVAC | Proc | Total | kWh/m²".
    /// Всички стойности са в kWh.
    /// </summary>
    public sealed class MonthlyGainsRow
    {
        /// <summary>Номер на месеца 1..12.</summary>
        public int Month { get; set; }

        /// <summary>Обитатели [kWh]</summary>
        public double Oc   { get; set; }

        /// <summary>Уреди [kWh]</summary>
        public double A    { get; set; }

        /// <summary>Осветление [kWh]</summary>
        public double L    { get; set; }

        /// <summary>Топла вода / ВиК регенерируеми загуби [kWh]</summary>
        public double WA   { get; set; }

        /// <summary>Помпи и вентилатори [kWh]</summary>
        public double HVAC { get; set; }

        /// <summary>Процесна топлина [kWh]  (може да е отрицателна за охладителен процес)</summary>
        public double Proc { get; set; }

        /// <summary>Сума на 6-те компонента [kWh]</summary>
        public double Total => Oc + A + L + WA + HVAC + Proc;

        /// <summary>Специфична стойност [kWh/m²] = Total / A_use</summary>
        public double TotalPerM2 { get; set; }
    }
}
