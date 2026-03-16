using System;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Models.Climate;
using EE.Doklad.Services.VentCooling;

namespace EE.Doklad.Services.Climate
{
    /// <summary>
    /// Климатичен провайдър за EPW данни, имплементиращ <see cref="IClimateProvider"/>.
    ///
    /// Разлика от BgAvgClimateProvider:
    ///   - IsBgAvgMode = false → Engine НЕ умножава по WorkingDays.
    ///   - GetHourlyData(month) връща ВСИЧКИ реални часове от месеца (672..744 точки).
    ///   - Всяка точка носи собствено B_Pa (от EPW колона 9, с imputation).
    ///   - LocalTime е реална дата/час за DayOfWeek / season / schedule филтриране в Engine.
    ///   - BarometricPressure_Pa е средната стойност от всички 8760 точки (за fallback).
    /// </summary>
    public sealed class EpwClimateProvider : IClimateProvider
    {
        private readonly ClimatePoint[] _hourlyData; // 8760 points
        private readonly Dictionary<int, List<ClimateHourPoint>> _byMonth; // 1..12 → points

        /// <inheritdoc/>
        public bool IsBgAvgMode => false;

        /// <inheritdoc/>
        public double BarometricPressure_Pa { get; }

        /// <summary>
        /// Създава EPW climate provider от 8760 ClimatePoint записа.
        /// </summary>
        /// <param name="hourlyData">Масив с точно 8760 точки, всяка с BarometricPressurePa > 0.</param>
        /// <param name="fixedYear">Фиксираната референтна година използвана в LocalTime.</param>
        public EpwClimateProvider(ClimatePoint[] hourlyData, int fixedYear = global::EE.Doklad.CalendarDefaults.ReferenceYear)
        {
            if (hourlyData == null || hourlyData.Length != 8760)
                throw new ArgumentException("EPW данни трябва да съдържат точно 8760 часови записа.", nameof(hourlyData));

            _hourlyData = hourlyData;

            // Средно налягане за fallback
            BarometricPressure_Pa = _hourlyData.Average(p => p.BarometricPressurePa);

            // Групираме по месец за бърз достъп
            _byMonth = new Dictionary<int, List<ClimateHourPoint>>(12);
            for (int m = 1; m <= 12; m++)
                _byMonth[m] = new List<ClimateHourPoint>();

            foreach (var pt in _hourlyData)
            {
                _byMonth[pt.Month].Add(new ClimateHourPoint
                {
                    Hour       = pt.Hour,
                    LocalTime  = pt.LocalTime,
                    T_out_C    = pt.DryBulbC,
                    RH_out_Pct = pt.RH,
                    B_Pa       = pt.BarometricPressurePa,
                });
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Връща ВСИЧКИ реални часове от месеца (672..744 точки).
        /// Engine V2 при isBgAvgMode=false итерира всеки час и проверява
        /// season/DayOfWeek/schedule/holidays.
        /// </remarks>
        public IReadOnlyList<ClimateHourPoint> GetHourlyData(int month)
        {
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(nameof(month));

            return _byMonth[month];
        }
    }
}
