using System.Linq;
using EE.Doklad.Models;

namespace EE.Doklad.Sections.ThermalBridges
{
    /// <summary>
    /// Изчислява Hel, Htb и Htotal за топлинни мостове.
    /// </summary>
    public static class ThermalBridgeCalculator
    {
        /// <summary>
        /// Актуализира Hel, Htb и Htotal за произволен елемент.
        /// </summary>
        /// <param name="settings">Настройките на термомостовете.</param>
        /// <param name="u">U-стойност [W/(m²·K)].</param>
        /// <param name="area">Площ [m²].</param>
        /// <param name="useFacadeMultiplier">
        ///   true  → Manual режим: всеки мост се умножава по FacadeCount (използва TotalLoss).
        ///   false → Manual режим: всеки мост се брои веднъж (използва TotalLossNoFacade) — за Покрив.
        /// </param>
        public static void Recalculate(
            WallThermalBridgeSettings settings,
            double u,
            double area,
            bool useFacadeMultiplier = true)
        {
            double hel = u * area;
            double htb;

            switch (settings.Mode)
            {
                case ThermalBridgeMode.None:
                    htb = 0.0;
                    break;

                case ThermalBridgeMode.GlobalPercentage:
                    htb = hel * settings.GlobalPercent / 100.0;
                    break;

                case ThermalBridgeMode.Manual:
                    // Външни стени: Σ (L×ψ + χ) × FacadeCount
                    // Покрив:       Σ (L×ψ + χ)          (без фасаден множител)
                    htb = useFacadeMultiplier
                        ? settings.Items.Sum(item => item.TotalLoss)
                        : settings.Items.Sum(item => item.TotalLossNoFacade);
                    break;

                default:
                    htb = 0.0;
                    break;
            }

            settings.Hel    = hel;
            settings.Htb    = htb;
            settings.Htotal = hel + htb;
        }

        /// <summary>
        /// Overload за <see cref="ExternalWallType"/> — прилага фасаден множител.
        /// </summary>
        public static void Recalculate(ExternalWallType wall)
            => Recalculate(wall.ThermalBridges, wall.Uw, wall.Area, useFacadeMultiplier: true);

        /// <summary>
        /// Overload за <see cref="RoofType"/> — БЕЗ фасаден множител (покривът няма фасади).
        /// </summary>
        public static void Recalculate(RoofType roof)
            => Recalculate(roof.ThermalBridges, roof.UValue, roof.Area, useFacadeMultiplier: false);
    }
}
