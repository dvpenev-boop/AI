using System;
using System.Linq;
using EE.Doklad.Models;

namespace EE.Doklad.Services.FloorStrategies
{
    /// <summary>
    /// Стратегия за под към неотопляемо пространство по ISO 13789
    /// </summary>
    public class UnheatedSpaceFloorStrategy : IFloorStrategy<FloorUnheatedSpaceInput>
    {
        public FloorCalculationResult Calculate(FloorUnheatedSpaceInput input)
        {
            var result = new FloorCalculationResult
            {
                FloorType = FloorType.UnheatedSpace,
                Area = input.Area
            };

            try
            {
                // Rf (съпротивление на пода)
                double rf = input.Layers.Sum(layer => layer.Thickness / layer.Lambda);
                double rtotalFloor = input.Rsi + rf + input.Rse;

                // Приблизителна температура в неотопляемото пространство
                // θu ≈ (θi + θe) / 2 (опростено; може да се усложни с енергиен баланс)
                double thetaU = (input.Ti + input.Te) / 2.0;

                // U на пода
                double uFloor = rtotalFloor > 0 ? 1.0 / rtotalFloor : 0;

                // Ефективен U с оглед на междинната температура
                double deltaT = input.Ti - thetaU;
                
                result.U = uFloor;
                
                result.Components.Add(new FloorCalcComponent
                {
                    Name = "Под към неотопляемо пространство",
                    U = uFloor,
                    Area = input.Area,
                    DeltaT = deltaT,
                    Q = uFloor * input.Area * deltaT
                });

                result.Assumptions.Add($"θu (прибл.) = {thetaU:0.0} °C");
                result.Assumptions.Add($"Rf = {rf:0.000} m²K/W");
                result.Assumptions.Add($"Вентилация: {input.VentilationMode}");
                if (input.VentilationMode != VentilationMode.None)
                {
                    result.Assumptions.Add($"n = {input.N:0.00} 1/h");
                }

                result.IsValid = true;
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = $"Грешка при изчисление: {ex.Message}";
            }

            return result;
        }
    }
}
