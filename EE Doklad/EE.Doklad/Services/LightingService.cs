using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EE.Doklad.Models;
using Newtonsoft.Json;

namespace EE.Doklad.Services
{
    public interface ILightingRepository
    {
        IReadOnlyList<LightingSeed> LoadSeed();
        IReadOnlyList<LightingUser> LoadUser();
        void SaveUser(IReadOnlyList<LightingUser> items);
    }

    public sealed class JsonLightingRepository : ILightingRepository
    {
        private readonly string _seedResourceName;
        private readonly string _userFilePath;

        public JsonLightingRepository(
            string seedResourceName = "EE.Doklad.Data.lighting.seed.json",
            string? userFilePath = null)
        {
            _seedResourceName = seedResourceName;
            _userFilePath = userFilePath ?? GetDefaultUserFilePath();
        }

        private static string GetDefaultUserFilePath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dir = Path.Combine(appData, "EEDoklad");
            return Path.Combine(dir, "lighting.user.json");
        }

        public IReadOnlyList<LightingSeed> LoadSeed()
        {
            var asm = Assembly.GetExecutingAssembly();
            using var stream = asm.GetManifestResourceStream(_seedResourceName);
            if (stream == null)
                throw new InvalidOperationException($"Не е намерен embedded ресурс '{_seedResourceName}'.");

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var items = JsonConvert.DeserializeObject<List<LightingSeed>>(json)
                        ?? throw new InvalidOperationException("Невалиден lighting seed JSON.");
            return items;
        }

        public IReadOnlyList<LightingUser> LoadUser()
        {
            if (!File.Exists(_userFilePath))
                return Array.Empty<LightingUser>();

            var json = File.ReadAllText(_userFilePath);
            var items = JsonConvert.DeserializeObject<List<LightingUser>>(json);
            return items ?? new List<LightingUser>();
        }

        public void SaveUser(IReadOnlyList<LightingUser> items)
        {
            var dir = Path.GetDirectoryName(_userFilePath);
            if (!string.IsNullOrWhiteSpace(dir))
                Directory.CreateDirectory(dir);

            var json = JsonConvert.SerializeObject(items, Formatting.Indented);
            File.WriteAllText(_userFilePath, json);
        }
    }

    public sealed class LightingService
    {
        private readonly ILightingRepository _repo;

        public LightingService(ILightingRepository repo)
        {
            _repo = repo;
        }

        public IReadOnlyList<LightingSeed> GetSeed() => _repo.LoadSeed();
        public IReadOnlyList<LightingUser> GetUser() => _repo.LoadUser();

        public IReadOnlyList<LightingRow> GetCombinedRows(bool includeSeed, bool includeUser)
        {
            var rows = new List<LightingRow>();

            if (includeSeed)
            {
                var seed = _repo.LoadSeed();
                foreach (var item in seed)
                {
                    rows.Add(new LightingRow
                    {
                        Id = $"seed-{item.Name}",
                        Name = item.Name,
                        PowerW = item.PowerW,
                        IsSeed = true
                    });
                }
            }

            if (includeUser)
            {
                var user = _repo.LoadUser();
                foreach (var item in user)
                {
                    rows.Add(new LightingRow
                    {
                        Id = item.Id,
                        Name = item.Name,
                        PowerW = item.PowerW,
                        IsSeed = false
                    });
                }
            }

            return rows;
        }

        public void AddUserItem(LightingUser item)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
                item.Id = "user-" + Guid.NewGuid().ToString("N");

            var list = new List<LightingUser>(_repo.LoadUser());
            list.Add(item);
            _repo.SaveUser(list);
        }

        public void UpdateUserItem(LightingUser item)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
                throw new InvalidOperationException("Липсва Id.");

            var list = new List<LightingUser>(_repo.LoadUser());
            var idx = list.FindIndex(x => x.Id == item.Id);
            if (idx < 0)
                throw new InvalidOperationException("Елементът не е намерен.");

            list[idx] = item;
            _repo.SaveUser(list);
        }

        public void DeleteUserItem(string id)
        {
            var list = new List<LightingUser>(_repo.LoadUser());
            list.RemoveAll(x => x.Id == id);
            _repo.SaveUser(list);
        }
    }
}
