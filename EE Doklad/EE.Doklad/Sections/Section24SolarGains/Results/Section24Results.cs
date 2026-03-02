namespace EE.Doklad.Sections.Section24SolarGains.Results
{
    /// <summary>
    /// Обобщени резултати за един месец (сума от всички прозорци и непрозрачни елементи).
    /// Формула (3.36): Q_sol_total = Σ Q_sol_window + Σ Q_sol_opaque.
    /// </summary>
    public class MonthlyTotalResult
    {
        /// <summary>Индекс на месеца 0–11.</summary>
        public int MonthIndex { get; init; }

        /// <summary>Кратко наименование на месеца.</summary>
        public string MonthName { get; init; } = string.Empty;

        /// <summary>Σ Q_sol_window за всички прозорци за месеца, kWh.</summary>
        public double SumQ_sol_windows { get; init; }

        /// <summary>Σ Q_sol_opaque за всички непрозрачни елементи за месеца, kWh.</summary>
        public double SumQ_sol_opaque { get; init; }

        /// <summary>Σ Q_sky за всички елементи за месеца, kWh.</summary>
        public double SumQ_sky { get; init; }

        /// <summary>
        /// Общи слънчеви топлинни печалби за месеца:
        /// Q_sol_total = SumQ_sol_windows + SumQ_sol_opaque, kWh.
        /// Формула (3.36).
        /// </summary>
        public double Q_sol_total { get; init; }
    }

    /// <summary>
    /// Пълни резултати за Секция 24 – топлинни печалби от слънчево греене.
    /// </summary>
    public class Section24Results
    {
        /// <summary>Резултати по прозорци.</summary>
        public WindowElementResult[] WindowResults { get; init; } = Array.Empty<WindowElementResult>();

        /// <summary>Резултати по непрозрачни елементи.</summary>
        public OpaqueElementResult[] OpaqueResults { get; init; } = Array.Empty<OpaqueElementResult>();

        /// <summary>Месечни обобщени резултати (12 реда).</summary>
        public MonthlyTotalResult[] MonthlyTotals { get; init; } = new MonthlyTotalResult[12];

        /// <summary>Годишна сума Q_sol_total [kWh].</summary>
        public double AnnualQ_sol_total
            => MonthlyTotals.Sum(r => r.Q_sol_total);

        /// <summary>Годишна Σ Q_sol_windows [kWh].</summary>
        public double AnnualQ_sol_windows
            => MonthlyTotals.Sum(r => r.SumQ_sol_windows);

        /// <summary>Годишна Σ Q_sol_opaque [kWh].</summary>
        public double AnnualQ_sol_opaque
            => MonthlyTotals.Sum(r => r.SumQ_sol_opaque);

        /// <summary>Годишна Σ Q_sky [kWh].</summary>
        public double AnnualQ_sky
            => MonthlyTotals.Sum(r => r.SumQ_sky);
    }
}
