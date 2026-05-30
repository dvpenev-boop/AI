using System;
using System.Collections.Generic;
using System.Linq;

namespace EE.Doklad.Tests.Validation.FullOracle
{
    public enum EECalcFuel
    {
        Fuel1 = 1,
        Fuel2 = 2,
        Fuel3 = 3,
        Fuel4 = 4,
        Fuel5 = 5,
        Fuel6 = 6,
        Fuel7 = 7,
        Fuel8 = 8,
        Fuel9 = 9,
        Fuel10 = 10,
        Fuel11 = 11
    }

    public static class EECalcLegacyAggregation
    {
        public static double CalculateTotalFuelWithDuplicateFuel1(IReadOnlyDictionary<EECalcFuel, double> fuelValues)
        {
            ArgumentNullException.ThrowIfNull(fuelValues);

            return Value(fuelValues, EECalcFuel.Fuel1)
                + Value(fuelValues, EECalcFuel.Fuel1)
                + Enum.GetValues<EECalcFuel>()
                    .Where(fuel => fuel != EECalcFuel.Fuel1)
                    .Sum(fuel => Value(fuelValues, fuel));
        }

        public static EECalcFuel MapFuelReportBucket(EECalcFuel inputFuel)
        {
            return inputFuel switch
            {
                EECalcFuel.Fuel1 => EECalcFuel.Fuel8,
                EECalcFuel.Fuel8 => EECalcFuel.Fuel1,
                _ => inputFuel
            };
        }

        private static double Value(IReadOnlyDictionary<EECalcFuel, double> fuelValues, EECalcFuel fuel)
        {
            return fuelValues.TryGetValue(fuel, out var value) ? value : 0.0;
        }
    }
}
