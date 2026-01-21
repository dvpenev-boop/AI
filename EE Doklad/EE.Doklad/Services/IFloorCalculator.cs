using EE.Doklad.Models;

namespace EE.Doklad.Services
{
    /// <summary>
    /// Интерфейс за калкулатор на подове
    /// </summary>
    public interface IFloorCalculator
    {
        FloorCalculationResult Calculate(FloorType type, object input);
    }
}
