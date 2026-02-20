using System;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Calculation mode options for the legacy monthly ventilation calculator.
    /// Kept minimal so other code (enums/VM options) continues to compile.
    /// </summary>
    public enum VentilationCoolingCalculationMode
    {
        MechanicalRecirculation3112,
        FreshAirProcessed3113
    }

    public sealed class VentilationCoolingModeOption
    {
        public VentilationCoolingCalculationMode Mode { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }
}
