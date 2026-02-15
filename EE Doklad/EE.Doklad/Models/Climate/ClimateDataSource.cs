namespace EE.Doklad.Models.Climate
{
    /// <summary>
    /// Източник на климатични данни за изчисления на охлаждане.
    /// </summary>
    public enum ClimateDataSource
    {
        /// <summary>
        /// BG база данни: типични дни (12 месеца × 24 часа).
        /// </summary>
        BG = 0,

        /// <summary>
        /// ASHRAE/EPW: пълна година с часови данни (8760 записа).
        /// </summary>
        ASHRAE_EPW = 1
    }
}
