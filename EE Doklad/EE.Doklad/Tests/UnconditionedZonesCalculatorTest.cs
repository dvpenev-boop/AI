using System;
using System.Collections.ObjectModel;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.Tests
{
    /// <summary>
    /// Тестов клас за проверка на изчисленията на неклиматизирани зони
    /// </summary>
    public class UnconditionedZonesCalculatorTest
    {
        public static void RunTest()
        {
            Console.WriteLine("=== Тест на модула за неклиматизирани зони (ztu) ===\n");

            // Създаваме тестова зона с елементи
            var zone = CreateTestZone();

            // Извличаме климатични данни
            var climateService = new ClimateService(new JsonClimateRepository());
            var climateData = climateService.GetZone(3); // София

            if (climateData == null)
            {
                Console.WriteLine("ГРЕШКА: Не може да се зареди климатична зона 3");
                return;
            }

            // Изчисляваме месечните параметри
            var calculator = new UnconditionedZonesCalculator();
            double indoorTemp = 20.0; // °C

            var results = calculator.Calculate(zone, climateData, indoorTemp);

            // Показваме резултатите
            Console.WriteLine($"Зона: {results.ZoneName} ({results.ZoneType})");
            Console.WriteLine($"Вътрешна температура: {indoorTemp} °C\n");

            Console.WriteLine("Месец | θe(°C) | Hztu,e(W/K) | Hztc-ztu(W/K) | Hztu,tot(W/K) | bztu    | θztu(°C)");
            Console.WriteLine("------+--------+-------------+---------------+---------------+---------+---------");

            foreach (var month in results.Months)
            {
                Console.WriteLine($"{month.MonthNumber,2}    | " +
                    $"{month.OutdoorTempC,6:F1} | " +
                    $"{month.HztuE_WK,11:F2} | " +
                    $"{month.HztcZtu_WK,13:F2} | " +
                    $"{month.HztuTot_WK,13:F2} | " +
                    $"{month.Bztu,7:F3} | " +
                    $"{month.TempZtu_C,7:F1}");
            }

            Console.WriteLine();

            // Проверка на границите
            TestBztuRange(results);
            TestTemperatureBounds(results, indoorTemp, climateData);
            TestEdgeCases();

            Console.WriteLine("\n=== Тест завърши успешно ===");
        }

        private static ZtuZone CreateTestZone()
        {
            // Тестова зона: външна неклиматизирана зона (ztue)
            // Пример: неотопляем таван/подпокривно пространство
            var zone = new ZtuZone
            {
                Name = "Неотопляем таван",
                Type = ZtuType.External,
                Notes = "Тестова зона за валидация"
            };

            // Елемент 1: Покривна конструкция към външна среда
            var roof = new ZtuElement
            {
                Name = "Покрив",
                Kind = ElementKind.Roof,
                Area = 50.0, // m²
                IsToExternalEnvironment = true
            };

            // Слоеве на покрива (отвън навътре)
            roof.Layers.Add(new ZtuLayer
            {
                MaterialName = "Керемиди",
                Thickness = 20.0,  // mm
                Lambda = 1.0       // W/(m·K)
            });

            roof.Layers.Add(new ZtuLayer
            {
                MaterialName = "Минерална вата",
                Thickness = 100.0, // mm
                Lambda = 0.04      // W/(m·K)
            });

            roof.Layers.Add(new ZtuLayer
            {
                MaterialName = "Гипсокартон",
                Thickness = 12.5,  // mm
                Lambda = 0.25      // W/(m·K)
            });

            // Изчисляваме U-стойността
            double sumR = 0.0;
            foreach (var layer in roof.Layers)
            {
                sumR += layer.R;
            }
            // Rsi за покрив = 0.10, Rse за външна среда = 0.04
            roof.UValue = 1.0 / (0.10 + sumR + 0.04);

            zone.ElementsToExternal.Add(roof);

            // Елемент 2: Под на тавана = таван на отопляемото помещение
            var floor = new ZtuElement
            {
                Name = "Таван на помещение",
                Kind = ElementKind.Floor,
                Area = 50.0, // m²
                IsToExternalEnvironment = false
            };

            // Същата конструкция, но от гледна точка на ztu към отопляемата зона
            floor.Layers.Add(new ZtuLayer
            {
                MaterialName = "Гипсокартон",
                Thickness = 12.5,  // mm
                Lambda = 0.25      // W/(m·K)
            });

            floor.Layers.Add(new ZtuLayer
            {
                MaterialName = "Минерална вата",
                Thickness = 100.0, // mm
                Lambda = 0.04      // W/(m·K)
            });

            floor.Layers.Add(new ZtuLayer
            {
                MaterialName = "Дървен под",
                Thickness = 20.0,  // mm
                Lambda = 0.15      // W/(m·K)
            });

            sumR = 0.0;
            foreach (var layer in floor.Layers)
            {
                sumR += layer.R;
            }
            // Rsi за под = 0.17, Rsi отдолу (към отопляема зона) = 0.17
            floor.UValue = 1.0 / (0.17 + sumR + 0.17);

            zone.ElementsToBoundary.Add(floor);

            return zone;
        }

        private static void TestBztuRange(ZtuMonthlyResults results)
        {
            Console.WriteLine("\n=== Тест 1: bztu трябва да е в диапазона [0..1] ===");
            bool passed = true;

            foreach (var month in results.Months)
            {
                if (month.Bztu < 0.0 || month.Bztu > 1.0)
                {
                    Console.WriteLine($"ГРЕШКА: Месец {month.MonthNumber}: bztu = {month.Bztu:F3} (извън диапазон [0..1])");
                    passed = false;
                }
            }

            if (passed)
            {
                Console.WriteLine("✓ УСПЕХ: Всички bztu стойности са в диапазона [0..1]");
            }
        }

        private static void TestTemperatureBounds(
            ZtuMonthlyResults results,
            double indoorTemp,
            ClimateZoneData climateData)
        {
            Console.WriteLine("\n=== Тест 2: θztu трябва да е между θe и θint ===");
            bool passed = true;

            for (int i = 0; i < results.Months.Count; i++)
            {
                var month = results.Months[i];
                double outdoorTemp = climateData.Monthly.AvgMonthlyTempC[i];

                double minTemp = Math.Min(outdoorTemp, indoorTemp);
                double maxTemp = Math.Max(outdoorTemp, indoorTemp);

                // Допускаме малка грешка поради закръгляне
                if (month.TempZtu_C < minTemp - 0.01 || month.TempZtu_C > maxTemp + 0.01)
                {
                    Console.WriteLine($"ГРЕШКА: Месец {month.MonthNumber}: " +
                        $"θztu = {month.TempZtu_C:F1}°C не е между " +
                        $"θe = {outdoorTemp:F1}°C и θint = {indoorTemp:F1}°C");
                    passed = false;
                }
            }

            if (passed)
            {
                Console.WriteLine("✓ УСПЕХ: Всички θztu стойности са между θe и θint");
            }
        }

        private static void TestEdgeCases()
        {
            Console.WriteLine("\n=== Тест 3: Ръбови случаи ===");

            var calculator = new UnconditionedZonesCalculator();
            var climateService = new ClimateService(new JsonClimateRepository());
            var climateData = climateService.GetZone(3);

            if (climateData == null)
            {
                Console.WriteLine("ГРЕШКА: Не може да се зареди климатична зона");
                return;
            }

            // Случай 1: Празна зона (Htot = 0)
            var emptyZone = new ZtuZone
            {
                Name = "Празна зона",
                Type = ZtuType.External
            };

            var results = calculator.Calculate(emptyZone, climateData, 20.0);
            bool case1Pass = true;

            foreach (var month in results.Months)
            {
                if (month.HztuTot_WK != 0.0 || month.Bztu != 0.0)
                {
                    Console.WriteLine($"ГРЕШКА: Празна зона - Месец {month.MonthNumber}: " +
                        $"Htot = {month.HztuTot_WK:F2}, bztu = {month.Bztu:F3} (очаква се 0)");
                    case1Pass = false;
                }

                // При празна зона θztu трябва да е равна на θe
                double expectedTemp = climateData.Monthly.AvgMonthlyTempC[month.MonthNumber - 1];
                if (Math.Abs(month.TempZtu_C - expectedTemp) > 0.01)
                {
                    Console.WriteLine($"ГРЕШКА: Празна зона - Месец {month.MonthNumber}: " +
                        $"θztu = {month.TempZtu_C:F1}°C, очаква се {expectedTemp:F1}°C");
                    case1Pass = false;
                }
            }

            if (case1Pass)
            {
                Console.WriteLine("✓ Случай 1 (празна зона): УСПЕХ");
            }

            // Случай 2: Само елементи към външна среда (Hztc-ztu = 0)
            var externalOnlyZone = new ZtuZone
            {
                Name = "Само външни елементи",
                Type = ZtuType.External
            };

            var element = new ZtuElement
            {
                Name = "Стена",
                Kind = ElementKind.Wall,
                Area = 10.0,
                UValue = 1.0,
                IsToExternalEnvironment = true
            };

            externalOnlyZone.ElementsToExternal.Add(element);

            results = calculator.Calculate(externalOnlyZone, climateData, 20.0);
            bool case2Pass = true;

            foreach (var month in results.Months)
            {
                if (Math.Abs(month.Bztu - 1.0) > 0.001)
                {
                    Console.WriteLine($"ГРЕШКА: Само външни елементи - Месец {month.MonthNumber}: " +
                        $"bztu = {month.Bztu:F3} (очаква се 1.0)");
                    case2Pass = false;
                }

                // При bztu = 1 → θztu = θe
                double expectedTemp = climateData.Monthly.AvgMonthlyTempC[month.MonthNumber - 1];
                if (Math.Abs(month.TempZtu_C - expectedTemp) > 0.01)
                {
                    Console.WriteLine($"ГРЕШКА: Само външни елементи - Месец {month.MonthNumber}: " +
                        $"θztu = {month.TempZtu_C:F1}°C, очаква се {expectedTemp:F1}°C");
                    case2Pass = false;
                }
            }

            if (case2Pass)
            {
                Console.WriteLine("✓ Случай 2 (само външни елементи, bztu=1): УСПЕХ");
            }

            // Случай 3: Само разделящи елементи (Hztu,e = 0, bztu = 0)
            var boundaryOnlyZone = new ZtuZone
            {
                Name = "Само разделящи елементи",
                Type = ZtuType.Internal
            };

            var boundaryElement = new ZtuElement
            {
                Name = "Преградна стена",
                Kind = ElementKind.Wall,
                Area = 15.0,
                UValue = 0.8,
                IsToExternalEnvironment = false
            };

            boundaryOnlyZone.ElementsToBoundary.Add(boundaryElement);

            results = calculator.Calculate(boundaryOnlyZone, climateData, 20.0);
            bool case3Pass = true;

            foreach (var month in results.Months)
            {
                if (Math.Abs(month.Bztu - 0.0) > 0.001)
                {
                    Console.WriteLine($"ГРЕШКА: Само разделящи елементи - Месец {month.MonthNumber}: " +
                        $"bztu = {month.Bztu:F3} (очаква се 0.0)");
                    case3Pass = false;
                }

                // При bztu = 0 → θztu = θint
                if (Math.Abs(month.TempZtu_C - 20.0) > 0.01)
                {
                    Console.WriteLine($"ГРЕШКА: Само разделящи елементи - Месец {month.MonthNumber}: " +
                        $"θztu = {month.TempZtu_C:F1}°C, очаква се 20.0°C");
                    case3Pass = false;
                }
            }

            if (case3Pass)
            {
                Console.WriteLine("✓ Случай 3 (само разделящи елементи, bztu=0): УСПЕХ");
            }
        }
    }
}
