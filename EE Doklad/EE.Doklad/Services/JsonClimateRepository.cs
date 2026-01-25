using System;
using System.IO;
using System.Linq;
using System.Reflection;
using EE.Doklad.Models;
using Newtonsoft.Json;

namespace EE.Doklad.Services
{
    public sealed class JsonClimateRepository : IClimateRepository
    {
        private readonly string _resourceName;

        public JsonClimateRepository(string resourceName = "EE.Doklad.Data.climate_zones.json")
        {
            _resourceName = resourceName;
        }

        public ClimateSeed LoadSeed()
        {
            var asm = Assembly.GetExecutingAssembly();
            using var stream = asm.GetManifestResourceStream(_resourceName);
            if (stream == null)
            {
                throw new InvalidOperationException(
                    $"Не е намерен embedded ресурс '{_resourceName}'. " +
                    "Провери EE.Doklad.csproj дали JSON файлът е маркиран като EmbeddedResource.");
            }

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();

            var seed = JsonConvert.DeserializeObject<ClimateSeed>(json)
                       ?? throw new InvalidOperationException("Невалиден climate seed JSON.");

            ValidateSeed(seed);

            return seed;
        }

        private static void ValidateSeed(ClimateSeed seed)
        {
            if (seed.Zones == null || seed.Zones.Count == 0)
                throw new InvalidOperationException("Липсват климатични зони в climate_zones.json.");

            foreach (var z in seed.Zones)
            {
                if (z.Id < 1 || z.Id > 9)
                    throw new InvalidOperationException($"Невалидна климатична зона Id={z.Id}.");

                var t = z.Monthly?.AvgMonthlyTempC;
                if (t == null || t.Length != 12)
                    throw new InvalidOperationException($"Зона {z.Id}: Monthly.AvgMonthlyTempC трябва да има 12 стойности.");

                var rh = z.Monthly?.AvgMonthlyRelHumidityPercentMayToSep;
                if (rh == null || rh.Length != 5)
                    throw new InvalidOperationException($"Зона {z.Id}: Monthly.AvgMonthlyRelHumidityPercentMayToSep трябва да има 5 стойности (May–Sep).");

                var solar = z.Monthly?.AvgFullSolarVerticalWm2;
                if (solar == null)
                    throw new InvalidOperationException($"Зона {z.Id}: Monthly.AvgFullSolarVerticalWm2 липсва.");

                var required = new[] { "N", "E", "W", "S", "H" };
                foreach (var k in required)
                {
                    if (!solar.TryGetValue(k, out var arr) || arr == null || arr.Length != 12)
                        throw new InvalidOperationException($"Зона {z.Id}: AvgFullSolarVerticalWm2['{k}'] трябва да има 12 стойности.");
                }
            }

            // Optional: normalize month orders if missing
            seed.MonthsOrder ??= new();
            seed.RelHumidityMonths ??= new();
            if (seed.MonthsOrder.Count == 0)
                seed.MonthsOrder = new() { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            if (seed.RelHumidityMonths.Count == 0)
                seed.RelHumidityMonths = new() { "May", "Jun", "Jul", "Aug", "Sep" };
        }
    }
}
