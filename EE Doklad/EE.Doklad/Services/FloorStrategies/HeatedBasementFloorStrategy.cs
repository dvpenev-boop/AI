using System;
using System.Linq;
using EE.Doklad.Models;

namespace EE.Doklad.Services.FloorStrategies
{
    /// <summary>
    /// Стратегия за под над отопляем сутерен + стени на сутерена към земя
    /// </summary>
    public class HeatedBasementFloorStrategy : IFloorStrategy<FloorHeatedBasementInput>
    {
        public FloorCalculationResult Calculate(FloorHeatedBasementInput input)
        {
            var result = new FloorCalculationResult
            {
                FloorType = FloorType.HeatedBasement,
                Area = input.AreaFloor
            };

            try
            {
                // 1. Междуетажна плоча (Ti → Tb)
                double rfFloor = input.FloorLayers.Sum(layer => layer.Thickness / layer.Lambda);
                double rtotalFloor = input.RsiFloor + rfFloor + input.RseFloor;
                double uFloor = rtotalFloor > 0 ? 1.0 / rtotalFloor : 0;

                result.Components.Add(new FloorCalcComponent
                {
                    Name = "Междуетажна плоча (Ti → Tb)",
                    U = uFloor,
                    Area = input.AreaFloor,
                    DeltaT = input.Ti - input.Tb,
                    Q = uFloor * input.AreaFloor * (input.Ti - input.Tb)
                });

                // 2. Стени на сутерена към земя (Tb → Te/ground)
                double rwWall = input.WallLayers.Sum(layer => layer.Thickness / layer.Lambda);
                double rtotalWall = input.RsiWall + rwWall;
                double uWall = rtotalWall > 0 ? 1.0 / rtotalWall : 0;

                result.Components.Add(new FloorCalcComponent
                {
                    Name = "Стени на сутерена към земя (Tb → Te)",
                    U = uWall,
                    Area = input.WallAreaToGround,
                    DeltaT = input.Tb - input.Te,
                    Q = uWall * input.WallAreaToGround * (input.Tb - input.Te)
                });

                // Общ U (ефективен) = средно-претеглен
                double totalQ = result.Components.Sum(c => c.Q);
                double totalArea = input.AreaFloor;
                double avgDeltaT = input.Ti - input.Tb;
                
                result.U = totalArea > 0 && avgDeltaT > 0 ? totalQ / (totalArea * avgDeltaT) : 0;

                result.Assumptions.Add($"Rf (плоча) = {rfFloor:0.000} m²K/W");
                result.Assumptions.Add($"Rw (стени) = {rwWall:0.000} m²K/W");
                result.Assumptions.Add($"z (дълбочина сутерен) = {input.Z:0.00} m");
                result.Assumptions.Add($"Ti = {input.Ti:0.0} °C, Tb = {input.Tb:0.0} °C, Te = {input.Te:0.0} °C");

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
