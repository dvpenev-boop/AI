namespace EE.Doklad.Models
{
    /// <summary>
    /// Типове подове според ISO 13370 и ISO 6946
    /// </summary>
    public enum FloorType
    {
        /// <summary>
        /// Не е избран тип
        /// </summary>
        Unselected,
        
        /// <summary>
        /// Под към външен въздух (еквивалент на топъл покрив)
        /// </summary>
        ExternalAir,
        
        /// <summary>
        /// Под към земя
        /// </summary>
        Ground,
        
        /// <summary>
        /// Под към неотопляемо помещение/въздушно пространство
        /// </summary>
        UnheatedSpace,
        
        /// <summary>
        /// Под над отопляем сутерен + стени на сутерена към земя
        /// </summary>
        HeatedBasement
    }

    /// <summary>
    /// Подтип изолация за под към земя
    /// </summary>
    public enum GroundInsulationType
    {
        None,             // Без изолация
        Edge,             // Краева изолация
        UnderSlab         // Изолация под плочата
    }

    /// <summary>
    /// Режим на вентилация за неотопляемо пространство
    /// </summary>
    public enum VentilationMode
    {
        None,             // Без вентилация
        Natural,          // Естествена вентилация
        Mechanical        // Механична вентилация
    }
}
