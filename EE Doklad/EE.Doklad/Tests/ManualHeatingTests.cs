using System;
using EE.Doklad.Models;

namespace EE.Doklad.Tests
{
    /// <summary>
    /// Ръчни тестове за валидиране на интерполацията
    /// </summary>
    public class ManualHeatingTests
    {
        public static void RunAllTests()
        {
            Console.WriteLine("=== ТЕСТВАНЕ НА ЛИНЕЙНА ИНТЕРПОЛАЦИЯ ===\n");

            TestExactMatch();
            TestLinearInterpolation_T21_Cinema();
            TestLinearInterpolation_T25_Office();
            TestBelowMinimum();
            TestAboveMaximum();
            TestComplexInterpolation_T23();
            TestDecimalTemperature();
            TestTotalOccupantHeat();

            Console.WriteLine("\n=== ВСИЧКИ ТЕСТОВЕ ЗАВЪРШИХА ===");
        }

        private static void TestExactMatch()
        {
            Console.WriteLine("TEST 1: Точно съвпадение при T=20°C за Cinema");
            var (sensible, latent) = ActivityDataService.CalculateHeatForTemperature(ActivityLevel.Cinema, 20.0);
            Console.WriteLine($"  Резултат: sensible={sensible:F2}W, latent={latent:F2}W");
            Console.WriteLine($"  Очаквано: sensible=79.00W, latent=21.00W");
            Console.WriteLine($"  Статус: {(Math.Abs(sensible - 79.0) < 0.01 && Math.Abs(latent - 21.0) < 0.01 ? "✓ УСПЕХ" : "✗ ГРЕШКА")}\n");
        }

        private static void TestLinearInterpolation_T21_Cinema()
        {
            Console.WriteLine("TEST 2: Линейна интерполация при T=21°C за Cinema");
            Console.WriteLine("  При T=20°C: sensible=79W, latent=21W");
            Console.WriteLine("  При T=22°C: sensible=72W, latent=28W");
            Console.WriteLine("  alpha = (21-20)/(22-20) = 0.5");
            Console.WriteLine("  sensible(21) = 79 + 0.5*(72-79) = 75.5W");
            Console.WriteLine("  latent(21) = 21 + 0.5*(28-21) = 24.5W");
            
            var (sensible, latent) = ActivityDataService.CalculateHeatForTemperature(ActivityLevel.Cinema, 21.0);
            Console.WriteLine($"  Резултат: sensible={sensible:F2}W, latent={latent:F2}W");
            Console.WriteLine($"  Статус: {(Math.Abs(sensible - 75.5) < 0.01 && Math.Abs(latent - 24.5) < 0.01 ? "✓ УСПЕХ" : "✗ ГРЕШКА")}\n");
        }

        private static void TestLinearInterpolation_T25_Office()
        {
            Console.WriteLine("TEST 3: Линейна интерполация при T=25°C за Office");
            Console.WriteLine("  При T=24°C: sensible=70W, latent=50W");
            Console.WriteLine("  При T=26°C: sensible=60W, latent=60W");
            Console.WriteLine("  alpha = 0.5");
            Console.WriteLine("  sensible(25) = 70 + 0.5*(60-70) = 65W");
            Console.WriteLine("  latent(25) = 50 + 0.5*(60-50) = 55W");
            
            var (sensible, latent) = ActivityDataService.CalculateHeatForTemperature(ActivityLevel.Office, 25.0);
            Console.WriteLine($"  Резултат: sensible={sensible:F2}W, latent={latent:F2}W");
            Console.WriteLine($"  Статус: {(Math.Abs(sensible - 65.0) < 0.01 && Math.Abs(latent - 55.0) < 0.01 ? "✓ УСПЕХ" : "✗ ГРЕШКА")}\n");
        }

        private static void TestBelowMinimum()
        {
            Console.WriteLine("TEST 4: Температура под минимума - Clamp към 20°C");
            var (sensible, latent) = ActivityDataService.CalculateHeatForTemperature(ActivityLevel.Cinema, 18.0);
            Console.WriteLine($"  Резултат: sensible={sensible:F2}W, latent={latent:F2}W");
            Console.WriteLine($"  Очаквано: sensible=79.00W, latent=21.00W (clamp към 20°C)");
            Console.WriteLine($"  Статус: {(Math.Abs(sensible - 79.0) < 0.01 && Math.Abs(latent - 21.0) < 0.01 ? "✓ УСПЕХ" : "✗ ГРЕШКА")}\n");
        }

        private static void TestAboveMaximum()
        {
            Console.WriteLine("TEST 5: Температура над максимума - Clamp към 28°C");
            var (sensible, latent) = ActivityDataService.CalculateHeatForTemperature(ActivityLevel.Cinema, 30.0);
            Console.WriteLine($"  Резултат: sensible={sensible:F2}W, latent={latent:F2}W");
            Console.WriteLine($"  Очаквано: sensible=50.00W, latent=50.00W (clamp към 28°C)");
            Console.WriteLine($"  Статус: {(Math.Abs(sensible - 50.0) < 0.01 && Math.Abs(latent - 50.0) < 0.01 ? "✓ УСПЕХ" : "✗ ГРЕШКА")}\n");
        }

        private static void TestComplexInterpolation_T23()
        {
            Console.WriteLine("TEST 6: Сложна интерполация при T=23°C за HeavyWork");
            Console.WriteLine("  При T=22°C: sensible=170W, latent=260W");
            Console.WriteLine("  При T=24°C: sensible=154W, latent=276W");
            Console.WriteLine("  alpha = 0.5");
            Console.WriteLine("  sensible(23) = 170 + 0.5*(154-170) = 162W");
            Console.WriteLine("  latent(23) = 260 + 0.5*(276-260) = 268W");
            
            var (sensible, latent) = ActivityDataService.CalculateHeatForTemperature(ActivityLevel.HeavyWork, 23.0);
            Console.WriteLine($"  Резултат: sensible={sensible:F2}W, latent={latent:F2}W");
            Console.WriteLine($"  Статус: {(Math.Abs(sensible - 162.0) < 0.01 && Math.Abs(latent - 268.0) < 0.01 ? "✓ УСПЕХ" : "✗ ГРЕШКА")}\n");
        }

        private static void TestDecimalTemperature()
        {
            Console.WriteLine("TEST 7: Десетична температура T=20.25°C за Cinema");
            Console.WriteLine("  alpha = 0.125");
            Console.WriteLine("  sensible(20.25) = 79 + 0.125*(72-79) = 78.125W");
            Console.WriteLine("  latent(20.25) = 21 + 0.125*(28-21) = 21.875W");
            
            var (sensible, latent) = ActivityDataService.CalculateHeatForTemperature(ActivityLevel.Cinema, 20.25);
            Console.WriteLine($"  Резултат: sensible={sensible:F2}W, latent={latent:F2}W");
            Console.WriteLine($"  Статус: {(Math.Abs(sensible - 78.125) < 0.01 && Math.Abs(latent - 21.875) < 0.01 ? "✓ УСПЕХ" : "✗ ГРЕШКА")}\n");
        }

        private static void TestTotalOccupantHeat()
        {
            Console.WriteLine("TEST 8: Обща топлина за 20 обитатели при T=21°C (Cinema)");
            var (sensiblePerPerson, latentPerPerson) = ActivityDataService.CalculateHeatForTemperature(ActivityLevel.Cinema, 21.0);
            var occupants = 20;
            var totalSensible = sensiblePerPerson * occupants;
            var totalLatent = latentPerPerson * occupants;
            
            Console.WriteLine($"  Топлина на човек: sensible={sensiblePerPerson:F2}W, latent={latentPerPerson:F2}W");
            Console.WriteLine($"  Обща топлина: sensible={totalSensible:F2}W, latent={totalLatent:F2}W");
            Console.WriteLine($"  Очаквано: sensible=1510.00W, latent=490.00W");
            Console.WriteLine($"  Статус: {(Math.Abs(totalSensible - 1510.0) < 0.01 && Math.Abs(totalLatent - 490.0) < 0.01 ? "✓ УСПЕХ" : "✗ ГРЕШКА")}\n");
        }
    }
}
