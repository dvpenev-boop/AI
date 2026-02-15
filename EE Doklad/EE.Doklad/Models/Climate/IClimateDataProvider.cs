using System;
using System.Collections.Generic;

namespace EE.Doklad.Models.Climate
{
    /// <summary>
    /// Абстрактен интерфейс за климатични данни.
    /// Позволява единен достъп до BG типични дни или EPW 8760 записа.
    /// </summary>
    public interface IClimateDataProvider
    {
        /// <summary>
        /// Източник на данните.
        /// </summary>
        ClimateDataSource Source { get; }

        /// <summary>
        /// Връща климатична точка за конкретен момент.
        /// За BG: използва типичния ден за дадения месец/час.
        /// За EPW: намира най-близката точка за дадената дата/час.
        /// </summary>
        ClimatePoint GetPoint(DateTime localDateTime);

        /// <summary>
        /// Връща последователност от климатични точки за период [from, to).
        /// </summary>
        /// <param name="from">Начална дата (включително)</param>
        /// <param name="to">Крайна дата (изключително)</param>
        IEnumerable<ClimatePoint> GetRange(DateTime from, DateTime to);
    }
}
