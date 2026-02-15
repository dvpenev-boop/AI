using System;

namespace EE.Doklad.Models.Climate
{
    /// <summary>
    /// Климатична точка - едно часово наблюдение.
    /// </summary>
    public struct ClimatePoint
    {
        /// <summary>
        /// Локално време на наблюдението.
        /// </summary>
        public DateTime LocalTime { get; set; }

        /// <summary>
        /// Месец (1-12).
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Час от деня (0-23).
        /// </summary>
        public int Hour { get; set; }

        /// <summary>
        /// Температура на сух термометър [°C].
        /// </summary>
        public double DryBulbC { get; set; }

        /// <summary>
        /// Относителна влажност [%] (0-100).
        /// </summary>
        public double RH { get; set; }

        /// <summary>
        /// Constructor за лесно създаване.
        /// </summary>
        public ClimatePoint(DateTime localTime, double dryBulbC, double rh)
        {
            LocalTime = localTime;
            Month = localTime.Month;
            Hour = localTime.Hour;
            DryBulbC = dryBulbC;
            RH = Math.Clamp(rh, 0.0, 100.0); // Clamp RH 0-100
        }

        public override string ToString()
        {
            return $"{LocalTime:yyyy-MM-dd HH:mm} | T={DryBulbC:F1}°C, RH={RH:F1}%";
        }
    }
}
