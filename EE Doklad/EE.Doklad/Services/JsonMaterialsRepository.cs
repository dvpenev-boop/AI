using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EE.Doklad.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EE.Doklad.Services
{
    public sealed class JsonMaterialsRepository : IMaterialsRepository
    {
        private readonly string _seedResourceName;
        private readonly string _userFilePath;

        public JsonMaterialsRepository(
            string seedResourceName = "EE.Doklad.Data.materials.seed.json",
            string? userFilePath = null)
        {
            _seedResourceName = seedResourceName;
            _userFilePath = userFilePath ?? GetDefaultUserFilePath();
        }

        // Try to load an optional embedded "typical" materials file. If not present, return empty list.
        public IReadOnlyList<BuildingMaterialSeed> LoadTypical()
        {
            var asm = Assembly.GetExecutingAssembly();
            const string typicalName = "EE.Doklad.Data.materials.typical.json";
            using var stream = asm.GetManifestResourceStream(typicalName);
            if (stream == null)
            {
                return Array.Empty<BuildingMaterialSeed>();
            }

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };

            var items = JsonConvert.DeserializeObject<List<BuildingMaterialSeed>>(json, settings);
            return items ?? new List<BuildingMaterialSeed>();
        }

        public IReadOnlyList<BuildingMaterialSeed> LoadSeed()
        {
            var asm = Assembly.GetExecutingAssembly();
            using var stream = asm.GetManifestResourceStream(_seedResourceName);
            if (stream == null)
            {
                throw new InvalidOperationException(
                    $"Не е намерен embedded ресурс '{_seedResourceName}'. " +
                    "Провери EE.Doklad.csproj дали materials.seed.json е маркиран като EmbeddedResource.");
            }

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };

            var items = JsonConvert.DeserializeObject<List<BuildingMaterialSeed>>(json, settings)
                        ?? throw new InvalidOperationException("Невалиден materials seed JSON.");

            return items;
        }

        public IReadOnlyList<BuildingMaterialUser> LoadUser()
        {
            if (!File.Exists(_userFilePath))
                return Array.Empty<BuildingMaterialUser>();

            var json = File.ReadAllText(_userFilePath);
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };

            var items = JsonConvert.DeserializeObject<List<BuildingMaterialUser>>(json, settings);

            return items ?? new List<BuildingMaterialUser>();
        }

        public void SaveUser(IReadOnlyList<BuildingMaterialUser> materials)
        {
            var dir = Path.GetDirectoryName(_userFilePath);
            if (!string.IsNullOrWhiteSpace(dir))
                Directory.CreateDirectory(dir);

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };

            var json = JsonConvert.SerializeObject(materials, Formatting.Indented, settings);
            File.WriteAllText(_userFilePath, json);
        }

        private static string GetDefaultUserFilePath()
        {
            var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(baseDir, "EE.Doklad", "materials.user.json");
        }
    }
}
