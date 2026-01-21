using EE.Doklad.Models;
using System;
using System.Linq;

namespace EE.Doklad.Services.FloorStrategies
{
    /// <summary>
    /// Стратегия за под към земя по ISO 13370
    /// </summary>
    public class GroundFloorStrategy : IFloorStrategy<FloorGroundInput>
    {
        public FloorCalculationResult Calculate(FloorGroundInput input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var result = new FloorCalculationResult
            {
                FloorType = FloorType.Ground,
                Area = input.Area,
                Components = new System.Collections.Generic.List<FloorCalcComponent>()
            };

            // Calculate floor thermal resistance
            double sumR = input.Layers.Sum(l => l.R);
            double totalR = input.Rsi + sumR;
            double Uf = totalR > 0 ? 1.0 / totalR : 0;

            // Calculate characteristic dimension
            double B = input.Area > 0 && input.Perimeter > 0 ? input.Area / (0.5 * input.Perimeter) : 0;

            // Calculate U-value based on insulation type
            double Uequiv = 0;
            switch (input.InsulationType)
            {
                case GroundInsulationType.None:
                    // No edge insulation - simplified calculation
                    if (B > 0)
                    {
                        double dt = input.LambdaGround / (Math.PI * B);
                        Uequiv = dt > 0 ? 1.0 / (totalR + dt) : 0;
                    }
                    break;

                case GroundInsulationType.Edge:
                    // Edge insulation - more complex calculation
                    // Simplified: assume similar to no insulation but with better performance
                    if (B > 0)
                    {
                        double dt = input.LambdaGround / (Math.PI * B + input.InsulationWidth);
                        Uequiv = dt > 0 ? 1.0 / (totalR + dt) : 0;
                    }
                    break;

                case GroundInsulationType.UnderSlab:
                    // Full insulation under slab
                    if (B > 0)
                    {
                        double dt = input.LambdaGround / (Math.PI * B);
                        Uequiv = dt > 0 ? 1.0 / (totalR + dt) : 0;
                    }
                    break;
            }

            result.U = Uequiv;
            
            result.Components.Add(new FloorCalcComponent
            {
                Name = "Под към земя",
                U = Uequiv,
                Area = input.Area,
                Description = $"B' = {B:F2} m, λg = {input.LambdaGround:F2} W/mK"
            });

            result.Assumptions.Add($"Характеристичен размер B' = {B:F2} m");
            result.Assumptions.Add($"Топлопроводност на земята λg = {input.LambdaGround:F2} W/mK");
            result.Assumptions.Add($"Тип изолация: {input.InsulationType}");

            return result;
        }
    }
}
