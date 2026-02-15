namespace EE.Doklad.Models
{
    /// <summary>
    /// Избор на климатична база данни за изчисление на вентилация охлаждане (Секция 14).
    /// </summary>
    public enum ClimateDatabase
    {
        /// <summary>
        /// Български стандарт БДС (локални климатични данни).
        /// </summary>
        BG = 0,

        /// <summary>
        /// ASHRAE стандарт (международни климатични данни).
        /// </summary>
        ASHRAE = 1
    }
}
