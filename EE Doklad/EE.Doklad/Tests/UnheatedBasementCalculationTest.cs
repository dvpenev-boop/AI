
using System;
using System.Collections.ObjectModel;
using EE.Doklad.Models;
using EE.Doklad.Services;
using EE.Doklad.Services.FloorStrategies;

namespace EE.Doklad.Tests
{
    /// <summary>
    /// Тестов клас за проверка на изчисленията на UnheatedBasementFloorStrategy
    /// </summary>
    public class UnheatedBasementCalculationTest
    {
        public static void RunTest()
        {
            Console.WriteLine("=== Тест на модула за под към неотопляем сутерен ===\n");

            // Примерни входни данни (според спецификацията)
            var input = new FloorUnheatedBasementInput
            {
                // Геометрия
                Area = 200.0,                    // m²
                Perimeter = 20.0 + 10.0,        // P = 2*(a+b), за 20x10 помещение = 60 m
                DepthBelowGround = 2.0,         // z = 2 m под терена
                HeightAboveGround = 0.3,        // h = 0.3 m над терена
                Volume = 200.0 * 2.3,           // V = 460 m³ (височина на сутерена ~2.3m)
                
                // Параметри за земята
                LambdaGround = 2.0,             // λg = 2.0 W/m·K
                WallThicknessAtGrade = 0.3,     // d_we = 0.3 m
                
                // Вентилация
                AirChangeRate = 0.3,            // n = 0.3 1/h
                
                // Топлинни съпротивления
                RsiFloorToBasement = 0.17,
                RseFloorToBasement = 0.17,
                RsiBasementFloor = 0.17,
                RseBasementFloor = 0.0,
                RsiBasementWall = 0.13,
                RseBasementWall = 0.0,
                RsiWallAboveGrade = 0.13,
                RseWallAboveGrade = 0.04,
                // Нови членове за прозорци и врати
                WindowArea = 2.0, // 2 m² прозорци
                DoorArea = 1.0,   // 1 m² врата
                WindowUValue = 1.2, // U-стойност на прозорци
                DoorUValue = 1.6    // U-стойност на врата
            };

            // Добавяне на слоеве за под между отопляемото и сутерена
            input.FloorToBasementLayers.Add(new FloorLayer
            {
                Material = "Бетон",
                Thickness = 0.2,      // 20 cm
                Lambda = 1.7          // W/m·K
            });

            // Добавяне на слоеве за подова плоча на сутерена
            input.BasementFloorLayers.Add(new FloorLayer
            {
                Material = "Бетон",
                Thickness = 0.2,
                Lambda = 1.7
            });
            input.BasementFloorLayers.Add(new FloorLayer
            {
                Material = "XPS изолация",
                Thickness = 0.05,     // 5 cm
                Lambda = 0.035
            });

            // Добавяне на слоеве за стени на сутерена
            input.BasementWallLayers.Add(new FloorLayer
            {
                Material = "Бетон",
                Thickness = 0.25,
                Lambda = 1.7
            });
            input.BasementWallLayers.Add(new FloorLayer
            {
                Material = "XPS изолация",
                Thickness = 0.08,     // 8 cm
                Lambda = 0.035
            });

            // Добавяне на слоеве за стени над терена
            input.WallAboveGradeLayers.Add(new FloorLayer
            {
                Material = "Бетон",
                Thickness = 0.25,
                Lambda = 1.7
            });

            // Изчисление
            var strategy = new UnheatedBasementFloorStrategy();
            var result = strategy.Calculate(input);

            // Отпечатване на резултатите
            Console.WriteLine($"Изчислението е валидно: {result.IsValid}");
            if (!result.IsValid)
            {
                Console.WriteLine($"Грешка: {result.ErrorMessage}");
                return;
            }

            Console.WriteLine($"\n--- ОСНОВЕН РЕЗУЛТАТ ---");
            Console.WriteLine($"Uub (еквивалентен коефициент): {result.U:F4} W/m²K\n");

            Console.WriteLine($"--- ДИАГНОСТИЧНИ КОМПОНЕНТИ ---");
            foreach (var component in result.Components)
            {
                Console.WriteLine($"{component.Name,-10} = {component.Value,8:F4} {component.Unit,-10} // {component.Description}");
            }

            Console.WriteLine($"\n--- ПРЕДПОЛОЖЕНИЯ ---");
            foreach (var assumption in result.Assumptions)
            {
                Console.WriteLine($"  - {assumption}");
            }

            // Проверка на очаквани стойности
            Console.WriteLine($"\n--- ПРОВЕРКИ ---");
            
            double expectedB = input.Area / (0.5 * input.Perimeter);
            Console.WriteLine($"Очакван B: {expectedB:F3} m");
            
            var bComponent = result.Components.Find(c => c.Name == "B");
            if (bComponent != null)
            {
                Console.WriteLine($"Изчислен B: {bComponent.Value:F3} m");
                Console.WriteLine($"B проверка: {(Math.Abs(bComponent.Value - expectedB) < 0.001 ? "✓ OK" : "✗ ГРЕШКА")}");
            }

            Console.WriteLine($"\nТест завършен!");
        }
    }
}
