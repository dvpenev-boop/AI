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
        /// Барометрично налягане [Pa].
        /// За EPW: от колона 9 (Station Pressure) с imputation.
        /// За BG: 0 (използва се зоновата стойност).
        /// </summary>
        public double BarometricPressurePa { get; set; }

        /// <summary>
        /// Constructor за лесно създаване (BG – без налягане).
        /// </summary>
        public ClimatePoint(DateTime localTime, double dryBulbC, double rh)
        {
            LocalTime = localTime;
            Month = localTime.Month;
            Hour = localTime.Hour;
            DryBulbC = dryBulbC;
            RH = Math.Clamp(rh, 0.0, 100.0);
            BarometricPressurePa = 0.0;
        }

        /// <summary>
        /// Constructor с барометрично налягане (EPW).
        /// </summary>
        public ClimatePoint(DateTime localTime, double dryBulbC, double rh, double barometricPressurePa)
        {
            LocalTime = localTime;
            Month = localTime.Month;
            Hour = localTime.Hour;
            DryBulbC = dryBulbC;
            RH = Math.Clamp(rh, 0.0, 100.0);
            BarometricPressurePa = barometricPressurePa;
        }

        public override string ToString()
        {
            return $"{LocalTime:yyyy-MM-dd HH:mm} | T={DryBulbC:F1}°C, RH={RH:F1}%, B={BarometricPressurePa:F0} Pa";
        }
    }
}
