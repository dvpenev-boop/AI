using System;
using System.Collections.Generic;
using EE.Doklad.Models;
using EE.Doklad.Services.VentCooling;

namespace EE.Doklad.Services.Climate
{
    /// <summary>
    /// Адаптер за BG_avg климатичен набор (24 часа за типичен ден на месеца).
    ///
    /// Данни: <see cref="MonthlyClimateData.AvgMonthlyTempC"/> (12 стойности) + RH (May-Sep, 5 стойности).
    /// Барометрично налягане: от <see cref="ClimateZoneData.GetEffectiveBarometricPressure"/>.
    ///
    /// Модел: за всеки час от 0..23 се използват еднакви T_out и RH_out (месечен средни).
    /// Това е напълно коректно за BG_avg – нямаме почасово разрешение по дни.
    ///
    /// За реалистична дневна крива може в бъдеще да се добави почасов профил per zone (upgrade path).
    /// </summary>
    public sealed class BgAvgClimateProvider : IClimateProvider
    {
        private readonly ClimateZoneData _zoneData;
        private readonly double _bPa;

        /// <inheritdoc/>
        public bool IsBgAvgMode => true;

        /// <inheritdoc/>
        public double BarometricPressure_Pa => _bPa;

        public BgAvgClimateProvider(ClimateZoneData zoneData)
        {
            _zoneData = zoneData ?? throw new ArgumentNullException(nameof(zoneData));
            _bPa = zoneData.GetEffectiveBarometricPressure();
        }

        /// <inheritdoc/>
        public IReadOnlyList<ClimateHourPoint> GetHourlyData(int month)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));

            int idx = month - 1;
            double t_out = _zoneData.Monthly.AvgMonthlyTempC[idx];

            // RH is available only May..Sep (index 0..4 = month 5..9)
            double rh_out = 50.0; // fallback
            if (month >= 5 && month <= 9)
            {
                var rhArr = _zoneData.Monthly.AvgMonthlyRelHumidityPercentMayToSep;
                int rhIdx = month - 5;
                if (rhArr != null && rhArr.Length > rhIdx && rhArr[rhIdx] > 0)
                    rh_out = rhArr[rhIdx];
            }

            // Build 24 identical points (one per hour)
            var points = new List<ClimateHourPoint>(24);
            for (int h = 0; h < 24; h++)
            {
                points.Add(new ClimateHourPoint
                {
                    Hour       = h,
                    T_out_C    = t_out,
                    RH_out_Pct = rh_out,
                    B_Pa       = _bPa,
                });
            }
            return points;
        }
    }
}
