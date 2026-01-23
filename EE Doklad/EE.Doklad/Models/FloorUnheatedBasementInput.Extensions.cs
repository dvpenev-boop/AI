using System.Collections.ObjectModel;

namespace EE.Doklad.Models
{
    public partial class FloorUnheatedBasementInput
    {
        /// <summary>
        /// Площ на прозорците в надтеренната част на сутерена (m²)
        /// </summary>
        public double WindowArea { get; set; }

        /// <summary>
        /// Площ на вратите в надтеренната част на сутерена (m²)
        /// </summary>
        public double DoorArea { get; set; }

        /// <summary>
        /// U-стойност на прозорците (W/m²K)
        /// </summary>
        public double WindowUValue { get; set; } = 1.3;

        /// <summary>
        /// U-стойност на вратите (W/m²K)
        /// </summary>
        public double DoorUValue { get; set; } = 1.5;
    }
}
