using System.Collections.Generic;
using EE.Doklad.Models;

namespace EE.Doklad.Services
{
    /// <summary>
    /// Клас на енергопотребление (A-G)
    /// </summary>
    public enum EnergyClass
    {
        A, B, C, D, E, F, G
    }

    /// <summary>
    /// Прагове за определяне на енергиен клас според типа сграда
    /// </summary>
    public class EnergyClassThresholds
    {
        public BuildingTypeCode BuildingType { get; set; }
        public double A { get; set; }  // EP < A
        public double B { get; set; }  // A ≤ EP < B
        public double C { get; set; }  // B ≤ EP < C
        public double D { get; set; }  // C ≤ EP < D
        public double E { get; set; }  // D ≤ EP < E
        public double F { get; set; }  // E ≤ EP < F
        // G: EP ≥ F
    }

    /// <summary>
    /// Service за изчисляване на клас на енергопотребление
    /// </summary>
    public static class EnergyClassCalculator
    {
        /// <summary>
        /// Таблица с прагове за всички типове сгради според Приложение №2
        /// </summary>
        private static readonly Dictionary<BuildingTypeCode, EnergyClassThresholds> _thresholds = new()
        {
            // Жилищни сгради
            {
                BuildingTypeCode.MultiFamilyResidential,
                new EnergyClassThresholds
                {
                    BuildingType = BuildingTypeCode.MultiFamilyResidential,
                    A = 90, B = 180, C = 235, D = 290, E = 363, F = 435
                }
            },
            {
                BuildingTypeCode.SingleFamilyResidential,
                new EnergyClassThresholds
                {
                    BuildingType = BuildingTypeCode.SingleFamilyResidential,
                    A = 83, B = 166, C = 203, D = 240, E = 300, F = 360
                }
            },

            // Административни (офиси)
            {
                BuildingTypeCode.Administrative,
                new EnergyClassThresholds
                {
                    BuildingType = BuildingTypeCode.Administrative,
                    A = 134, B = 268, C = 329, D = 390, E = 488, F = 585
                }
            },

            // Сгради за образование и наука
            {
                BuildingTypeCode.Schools,
                new EnergyClassThresholds
                {
                    BuildingType = BuildingTypeCode.Schools,
                    A = 35, B = 70, C = 110, D = 150, E = 188, F = 225
                }
            },
            {
                BuildingTypeCode.Universities,
                new EnergyClassThresholds
                {
                    BuildingType = BuildingTypeCode.Universities,
                    A = 85, B = 170, C = 215, D = 260, E = 325, F = 390
                }
            },
            {
                BuildingTypeCode.Kindergartens,
                new EnergyClassThresholds
                {
                    BuildingType = BuildingTypeCode.Kindergartens,
                    A = 60, B = 120, C = 190, D = 260, E = 325, F = 390
                }
            },

            // Здравеопазване
            {
                BuildingTypeCode.Healthcare,
                new EnergyClassThresholds
                {
                    BuildingType = BuildingTypeCode.Healthcare,
                    A = 135, B = 270, C = 355, D = 440, E = 550, F = 660
                }
            },

            // Хотели и ресторанти
            {
                BuildingTypeCode.HotelsRestaurants,
                new EnergyClassThresholds
                {
                    BuildingType = BuildingTypeCode.HotelsRestaurants,
                    A = 165, B = 330, C = 385, D = 440, E = 550, F = 660
                }
            },

            // Търговия
            {
                BuildingTypeCode.Trade,
                new EnergyClassThresholds
                {
                    BuildingType = BuildingTypeCode.Trade,
                    A = 275, B = 550, C = 600, D = 650, E = 813, F = 975
                }
            },

            // Спорт
            {
                BuildingTypeCode.Sports,
                new EnergyClassThresholds
                {
                    BuildingType = BuildingTypeCode.Sports,
                    A = 175, B = 350, C = 400, D = 450, E = 563, F = 675
                }
            },

            // Култура и изкуства
            {
                BuildingTypeCode.CultureArts,
                new EnergyClassThresholds
                {
                    BuildingType = BuildingTypeCode.CultureArts,
                    A = 110, B = 220, C = 270, D = 320, E = 400, F = 480
                }
            }
        };

        /// <summary>
        /// Изчислява енергийния клас (A-G) на база тип сграда и EP стойност
        /// </summary>
        /// <param name="buildingType">Тип на сградата</param>
        /// <param name="ep">Годишна специфична енергия EP [kWh/m²]</param>
        /// <returns>Енергиен клас (A-G) или null ако типът сграда не е намерен</returns>
        public static EnergyClass? CalculateClass(BuildingTypeCode buildingType, double ep)
        {
            if (!_thresholds.TryGetValue(buildingType, out var thresholds))
                return null;

            if (ep < thresholds.A) return EnergyClass.A;
            if (ep < thresholds.B) return EnergyClass.B;
            if (ep < thresholds.C) return EnergyClass.C;
            if (ep < thresholds.D) return EnergyClass.D;
            if (ep < thresholds.E) return EnergyClass.E;
            if (ep < thresholds.F) return EnergyClass.F;
            return EnergyClass.G;
        }

        /// <summary>
        /// Връща праговете за конкретен тип сграда
        /// </summary>
        public static EnergyClassThresholds? GetThresholds(BuildingTypeCode buildingType)
        {
            _thresholds.TryGetValue(buildingType, out var result);
            return result;
        }

        /// <summary>
        /// Връща описание на класа с диапазон
        /// </summary>
        public static string GetClassDescription(BuildingTypeCode buildingType, EnergyClass energyClass)
        {
            var thresholds = GetThresholds(buildingType);
            if (thresholds == null)
                return energyClass.ToString();

            return energyClass switch
            {
                EnergyClass.A => $"A (EP < {thresholds.A} kWh/m²)",
                EnergyClass.B => $"B ({thresholds.A} ≤ EP < {thresholds.B} kWh/m²)",
                EnergyClass.C => $"C ({thresholds.B} ≤ EP < {thresholds.C} kWh/m²)",
                EnergyClass.D => $"D ({thresholds.C} ≤ EP < {thresholds.D} kWh/m²)",
                EnergyClass.E => $"E ({thresholds.D} ≤ EP < {thresholds.E} kWh/m²)",
                EnergyClass.F => $"F ({thresholds.E} ≤ EP < {thresholds.F} kWh/m²)",
                EnergyClass.G => $"G (EP ≥ {thresholds.F} kWh/m²)",
                _ => energyClass.ToString()
            };
        }
    }
}
