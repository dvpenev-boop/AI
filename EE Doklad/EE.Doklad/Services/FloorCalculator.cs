using System;
using EE.Doklad.Models;
using EE.Doklad.Services.FloorStrategies;

namespace EE.Doklad.Services
{
    /// <summary>
    /// Главен калкулатор за подове
    /// </summary>
    public class FloorCalculator : IFloorCalculator
    {
        private readonly ExternalAirFloorStrategy _externalAirStrategy = new();
        private readonly GroundFloorStrategy _groundStrategy = new();
        private readonly UnheatedSpaceFloorStrategy _unheatedSpaceStrategy = new();
        private readonly HeatedBasementFloorStrategy _heatedBasementStrategy = new();
        private readonly UnheatedBasementFloorStrategy _unheatedBasementStrategy = new();

        public FloorCalculationResult Calculate(FloorType type, object input)
        {
            return type switch
            {
                FloorType.ExternalAir when input is FloorExternalAirInput externalAirInput =>
                    _externalAirStrategy.Calculate(externalAirInput),
                
                FloorType.Ground when input is FloorGroundInput groundInput =>
                    _groundStrategy.Calculate(groundInput),
                
                FloorType.UnheatedSpace when input is FloorUnheatedSpaceInput unheatedSpaceInput =>
                    _unheatedSpaceStrategy.Calculate(unheatedSpaceInput),
                
                FloorType.HeatedBasement when input is FloorHeatedBasementInput heatedBasementInput =>
                    _heatedBasementStrategy.Calculate(heatedBasementInput),
                
                FloorType.UnheatedBasement when input is FloorUnheatedBasementInput unheatedBasementInput =>
                    _unheatedBasementStrategy.Calculate(unheatedBasementInput),
                
                _ => new FloorCalculationResult
                {
                    FloorType = type,
                    IsValid = false,
                    ErrorMessage = "Невалиден тип под или входни данни."
                }
            };
        }
    }
}
