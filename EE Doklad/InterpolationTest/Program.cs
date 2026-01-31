using System;
using EE.Doklad.Models;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=== ТЕСТВАНЕ НА ЛИНЕЙНА ИНТЕРПОЛАЦИЯ ЗА ОТОПЛЕНИЕ ===\n");

// TEST 1: Точно съвпадение
Console.WriteLine("TEST 1: Точно съвпадение при T=20°C за Cinema");
var (s1, l1) = ActivityDataService.CalculateHeatForTemperature(ActivityLevel.Cinema, 20.0);
Console.WriteLine($"  Резултат: sensible={s1:F2}W, latent={l1:F2}W");
Console.WriteLine($"  Очаквано: sensible=79.00W, latent=21.00W");
Console.WriteLine($"  Статус: {(Math.Abs(s1 - 79.0) < 0.01 && Math.Abs(l1 - 21.0) < 0.01 ? "✓ УСПЕХ" : "✗ ГРЕШКА")}\n");

// TEST 2: Линейна интерполация T=21°C
Console.WriteLine("TEST 2: Линейна интерполация при T=21°C за Cinema (КЛЮЧОВ ACCEPTANCE TEST)");
Console.WriteLine("  При T=20°C: sensible=79W, latent=21W");
Console.WriteLine("  При T=22°C: sensible=72W, latent=28W");
Console.WriteLine("  alpha = (21-20)/(22-20) = 0.5");
Console.WriteLine("  sensible(21) = 79 + 0.5*(72-79) = 75.5W");
Console.WriteLine("  latent(21) = 21 + 0.5*(28-21) = 24.5W");

var (s2, l2) = ActivityDataService.CalculateHeatForTemperature(ActivityLevel.Cinema, 21.0);
Console.WriteLine($"  Резултат: sensible={s2:F2}W, latent={l2:F2}W");
Console.WriteLine($"  Статус: {(Math.Abs(s2 - 75.5) < 0.01 && Math.Abs(l2 - 24.5) < 0.01 ? "✓ УСПЕХ" : "✗ ГРЕШКА")}\n");

// TEST 3: Интерполация T=25°C
Console.WriteLine("TEST 3: Линейна интерполация при T=25°C за Office");
var (s3, l3) = ActivityDataService.CalculateHeatForTemperature(ActivityLevel.Office, 25.0);
Console.WriteLine($"  Резултат: sensible={s3:F2}W, latent={l3:F2}W");
Console.WriteLine($"  Очаквано: sensible=65.00W, latent=55.00W");
Console.WriteLine($"  Статус: {(Math.Abs(s3 - 65.0) < 0.01 && Math.Abs(l3 - 55.0) < 0.01 ? "✓ УСПЕХ" : "✗ ГРЕШКА")}\n");

// TEST 4: Clamp below minimum
Console.WriteLine("TEST 4: Температура под минимума (T=18°C) - Clamp към 20°C");
var (s4, l4) = ActivityDataService.CalculateHeatForTemperature(ActivityLevel.Cinema, 18.0);
Console.WriteLine($"  Резултат: sensible={s4:F2}W, latent={l4:F2}W");
Console.WriteLine($"  Очаквано: sensible=79.00W, latent=21.00W");
Console.WriteLine($"  Статус: {(Math.Abs(s4 - 79.0) < 0.01 && Math.Abs(l4 - 21.0) < 0.01 ? "✓ УСПЕХ" : "✗ ГРЕШКА")}\n");

// TEST 5: Clamp above maximum
Console.WriteLine("TEST 5: Температура над максимума (T=30°C) - Clamp към 28°C");
var (s5, l5) = ActivityDataService.CalculateHeatForTemperature(ActivityLevel.Cinema, 30.0);
Console.WriteLine($"  Резултат: sensible={s5:F2}W, latent={l5:F2}W");
Console.WriteLine($"  Очаквано: sensible=50.00W, latent=50.00W");
Console.WriteLine($"  Статус: {(Math.Abs(s5 - 50.0) < 0.01 && Math.Abs(l5 - 50.0) < 0.01 ? "✓ УСПЕХ" : "✗ ГРЕШКА")}\n");

// TEST 6: Decimal temperature
Console.WriteLine("TEST 6: Десетична температура T=20.25°C за Cinema");
var (s6, l6) = ActivityDataService.CalculateHeatForTemperature(ActivityLevel.Cinema, 20.25);
Console.WriteLine($"  Резултат: sensible={s6:F2}W, latent={l6:F2}W");
Console.WriteLine($"  Очаквано: sensible≈78.13W, latent≈21.88W");
Console.WriteLine($"  Статус: {(Math.Abs(s6 - 78.125) < 0.02 && Math.Abs(l6 - 21.875) < 0.02 ? "✓ УСПЕХ" : "✗ ГРЕШКА")}\n");

// TEST 7: Total heat for multiple occupants
Console.WriteLine("TEST 7: Обща топлина за 20 обитатели при T=21°C (Cinema)");
var (sPerPerson, lPerPerson) = ActivityDataService.CalculateHeatForTemperature(ActivityLevel.Cinema, 21.0);
var occupants = 20;
var totalS = sPerPerson * occupants;
var totalL = lPerPerson * occupants;

Console.WriteLine($"  Топлина на човек: sensible={sPerPerson:F2}W, latent={lPerPerson:F2}W");
Console.WriteLine($"  Обща топлина ({occupants} човека): sensible={totalS:F2}W, latent={totalL:F2}W");
Console.WriteLine($"  Очаквано: sensible=1510.00W, latent=490.00W");
Console.WriteLine($"  Статус: {(Math.Abs(totalS - 1510.0) < 0.01 && Math.Abs(totalL - 490.0) < 0.01 ? "✓ УСПЕХ" : "✗ ГРЕШКА")}\n");

// TEST 8: Различни активности
Console.WriteLine("TEST 8: Проверка на различни активности при T=24°C");
var activities = new[] { 
    ActivityLevel.Cinema, 
    ActivityLevel.Office, 
    ActivityLevel.ModerateWork,
    ActivityLevel.HeavyWork 
};

foreach (var activity in activities)
{
    var (s, l) = ActivityDataService.CalculateHeatForTemperature(activity, 24.0);
    Console.WriteLine($"  {activity,-15}: sensible={s,6:F2}W, latent={l,6:F2}W, total={s+l,6:F2}W");
}

Console.WriteLine("\n=== ВСИЧКИ ТЕСТОВЕ ЗАВЪРШИХА ===");
Console.WriteLine("\nНатиснете Enter за изход...");
Console.ReadLine();
