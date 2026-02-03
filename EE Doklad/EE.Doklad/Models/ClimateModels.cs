using System.Collections.Generic;

namespace EE.Doklad.Models
{
    public sealed class ClimateSeed
    {
        public string ImportedBy { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Revision { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty; // ISO date string

        /// <summary>
        /// Month labels (Jan..Dec) for the 12-value vectors.
        /// </summary>
        public List<string> MonthsOrder { get; set; } = new();

        /// <summary>
        /// Month labels for RH vector (May..Sep) – used for cooling module.
        /// </summary>
        public List<string> RelHumidityMonths { get; set; } = new();

        public List<ClimateZoneData> Zones { get; set; } = new();
    }

    public sealed class ClimateZoneData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public HeatingSeasonInfo HeatingSeason { get; set; } = new();

        /// <summary>
        /// Design outdoor temperature [°C] ("изчислителна външна температура").
        /// </summary>
        public double DesignOutdoorTempC { get; set; }

        /// <summary>
        /// Degree-days for indoor 19°C.
        /// </summary>
        public int DegreeDays19C { get; set; }

        public MonthlyClimateData Monthly { get; set; } = new();
    }

    public sealed class HeatingSeasonInfo
    {
        /// <summary>
        /// Format: "MM-dd" (e.g. "10-21")
        /// </summary>
        public string Start { get; set; } = string.Empty;

        /// <summary>
        /// Format: "MM-dd" (e.g. "04-20")
        /// </summary>
        public string End { get; set; } = string.Empty;
    }

    public sealed class MonthlyClimateData
    {
        /// <summary>
        /// Average outside air temperature per month [°C], length 12.
        /// </summary>
        public double[] AvgMonthlyTempC { get; set; } = new double[12];

        /// <summary>
        /// Optional: Number of heating days in each month (length 12). If provided, used directly.
        /// Values should be between 0 and daysInMonth.
        /// </summary>
        public int[]? HeatingDays { get; set; } = null;

        /// <summary>
        /// Optional: Flags per month whether the month is a heating month (length 12).
        /// If provided and HeatingDays is null, IsHeatingMonth[m]=true means full month counts.
        /// </summary>
        public bool[]? IsHeatingMonth { get; set; } = null;

        /// <summary>
        /// Average monthly relative humidity [%] for months May..Sep only (length 5).
        /// </summary>
        public double[] AvgMonthlyRelHumidityPercentMayToSep { get; set; } = new double[5];

        /// <summary>
        /// Average intensity of total solar radiation on vertical surfaces [W/m²].
        /// Keys: N, E, W, S (vertical) and H (horizontal surface). Values are length 12.
        /// </summary>
        public Dictionary<string, double[]> AvgFullSolarVerticalWm2 { get; set; } = new();
    }
}
