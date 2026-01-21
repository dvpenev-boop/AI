using System;
using System.Linq;
using EE.Doklad.Models;

namespace EE.Doklad.Services.FloorStrategies
{
    /// <summary>
    /// Стратегия за под към външен въздух (като топъл покрив)
    /// </summary>
    public class ExternalAirFloorStrategy : IFloorStrategy<FloorExternalAirInput>
    {
        public FloorCalculationResult Calculate(FloorExternalAirInput input)
        {
            var result = new FloorCalculationResult
            {
                FloorType = FloorType.ExternalAir,
                Area = input.Area
            };

            try
            {
                // Изчисление на R по ISO 6946
                double rw = input.Layers.Sum(layer => layer.Thickness / layer.Lambda);
                double rtotal = input.Rsi + rw + input.Rse;
                
                if (rtotal <= 0)
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Невалидно топлинно съпротивление.";
                    return result;
                }

                double u = 1.0 / rtotal;
                result.U = u;
                
                result.Components.Add(new FloorCalcComponent
                {
                    Name = "Под към външен въздух",
                    U = u,
                    Area = input.Area,
                    DeltaT = input.Ti - input.Te,
                    Q = u * input.Area * (input.Ti - input.Te)
                });

                result.Assumptions.Add($"Rsi = {input.Rsi:0.00} m²K/W");
                result.Assumptions.Add($"Rse = {input.Rse:0.00} m²K/W");
                result.Assumptions.Add($"Rtotal = {rtotal:0.000} m²K/W");

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
