using System.Collections.Generic;
using EE.Doklad.Services.VentCooling;

namespace EE.Doklad.Services.Climate
{
    /// <summary>
    /// Единен интерфейс за климатичен доставчик (BG_avg и EPW).
    /// Връща почасови данни за дадения месец.
    ///
    /// BG_avg: 24 точки (типичен ден) – умножават се по WorkingDays в Engine.
    /// EPW:    всички часове в месеца (филтрирани по schedule) – summed directly.
    /// </summary>
    public interface IClimateProvider
    {
        /// <summary>Режим на доставчика (влияе на Engine умножителя).</summary>
        bool IsBgAvgMode { get; }

        /// <summary>
        /// Връща климатичните точки за дадения месец.
        /// </summary>
        IReadOnlyList<ClimateHourPoint> GetHourlyData(int month);

        /// <summary>
        /// Барометрично налягане [Pa] – зонова стойност за BG_avg; може да е null при EPW ако е вградено в точките.
        /// </summary>
        double BarometricPressure_Pa { get; }
    }
}
