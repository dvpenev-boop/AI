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

        /// <summary>
        /// Representative barometric pressure [Pa] for the climate zone (altitude-based).
        /// Used when BG_avg climate dataset is active (no hourly B data).
        /// Populated from <see cref="ClimateZonePressureDefaults.GetPressure"/> if not set in the JSON.
        /// </summary>
        public double? BarometricPressure_Pa { get; set; }

        /// <summary>
        /// Returns the effective barometric pressure: value from JSON if present,
        /// otherwise the zone-altitude default from <see cref="ClimateZonePressureDefaults"/>.
        /// </summary>
        public double GetEffectiveBarometricPressure()
            => BarometricPressure_Pa is > 0
                ? BarometricPressure_Pa.Value
                : ClimateZonePressureDefaults.GetPressure(Id);

        public MonthlyClimateData Monthly { get; set; } = new();
    }

    /// <summary>
    /// Default barometric pressures [Pa] per BG climate zone (altitude-derived).
    /// Source: Наредба 7257_1, табл. 3.14 / зонови средни стойности.
    /// Used when the JSON climate dataset does not contain explicit B values.
    /// Trade-off: single scalar per zone is accurate enough for monthly BG_avg calculations
    /// (max altitude variation within a zone is ~300 m ⇒ ~3600 Pa ≈ 3.6% error on x).
    /// For EPW/ASHRAE data the hourly B from the file should be used directly.
    /// </summary>
    public static class ClimateZonePressureDefaults
    {
        private static readonly double[] _pressuresByZone = new double[]
        {
            101000.0, // Zone 1
            99200.0,  // Zone 2
            101200.0, // Zone 3
            98300.0,  // Zone 4
            100900.0, // Zone 5
            99400.0,  // Zone 6
            94400.0,  // Zone 7 (highest altitude)
            100600.0, // Zone 8
            99400.0,  // Zone 9
        };

        /// <param name="zoneId">1-based zone identifier (1..9).</param>
        public static double GetPressure(int zoneId)
        {
            if (zoneId < 1 || zoneId > _pressuresByZone.Length)
                return 101325.0; // ISA sea-level fallback
            return _pressuresByZone[zoneId - 1];
        }
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
