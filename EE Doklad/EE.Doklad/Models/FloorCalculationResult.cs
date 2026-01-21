using System.Collections.Generic;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Individual component of floor calculation
    /// </summary>
    public class FloorCalcComponent
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Value { get; set; }
        public string Unit { get; set; } = string.Empty;
        
        // Additional properties for detailed calculations
        public double U { get; set; }
        public double Area { get; set; }
        public double DeltaT { get; set; }
        public double Q { get; set; }
    }

    /// <summary>
    /// Floor component with U-value and area
    /// </summary>
    public class FloorComponent
    {
        public string Name { get; set; } = string.Empty;
        public double UValue { get; set; }
        public double Area { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Q => UValue * Area; // Heat loss
        public double HeatLoss => UValue * Area;
    }

    /// <summary>
    /// Result of floor thermal calculation
    /// </summary>
    public class FloorCalculationResult
    {
        public FloorType FloorType { get; set; }
        
        public double Area { get; set; }
        
        public double U { get; set; }
        
        public List<FloorCalcComponent> Components { get; set; } = new List<FloorCalcComponent>();
        
        public List<string> Assumptions { get; set; } = new List<string>();
        
        public bool IsValid { get; set; } = true;
        
        public string ErrorMessage { get; set; } = string.Empty;

        public double TotalUValue
        {
            get
            {
                return U;
            }
        }

        public double TotalArea
        {
            get
            {
                return Area;
            }
        }
    }
}