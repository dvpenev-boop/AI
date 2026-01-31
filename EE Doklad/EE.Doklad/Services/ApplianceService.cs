using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EE.Doklad.Models;
using Newtonsoft.Json;

namespace EE.Doklad.Services
{
    public interface IApplianceRepository
    {
        IReadOnlyList<ApplianceSeed> LoadSeed();
        IReadOnlyList<ApplianceUser> LoadUser();
        void SaveUser(IReadOnlyList<ApplianceUser> items);
    }

    public sealed class JsonApplianceRepository : IApplianceRepository
    {
        private readonly string _seedResourceName;
        private readonly string _userFilePath;

        public JsonApplianceRepository(
            string seedResourceName = "EE.Doklad.Data.appliances.seed.json",
            string? userFilePath = null)
        {
            _seedResourceName = seedResourceName;
            _userFilePath = userFilePath ?? GetDefaultUserFilePath();
        }

        private static string GetDefaultUserFilePath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dir = Path.Combine(appData, "EEDoklad");
            return Path.Combine(dir, "appliances.user.json");
        }

        public IReadOnlyList<ApplianceSeed> LoadSeed()
        {
            var asm = Assembly.GetExecutingAssembly();
            using var stream = asm.GetManifestResourceStream(_seedResourceName);
            if (stream == null)
                throw new InvalidOperationException($"Не е намерен embedded ресурс '{_seedResourceName}'.");

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var items = JsonConvert.DeserializeObject<List<ApplianceSeed>>(json)
                        ?? throw new InvalidOperationException("Невалиден appliances seed JSON.");
            return items;
        }

        public IReadOnlyList<ApplianceUser> LoadUser()
        {
            if (!File.Exists(_userFilePath))
                return Array.Empty<ApplianceUser>();

            var json = File.ReadAllText(_userFilePath);
            var items = JsonConvert.DeserializeObject<List<ApplianceUser>>(json);
            return items ?? new List<ApplianceUser>();
        }

        public void SaveUser(IReadOnlyList<ApplianceUser> items)
        {
            var dir = Path.GetDirectoryName(_userFilePath);
            if (!string.IsNullOrWhiteSpace(dir))
                Directory.CreateDirectory(dir);

            var json = JsonConvert.SerializeObject(items, Formatting.Indented);
            File.WriteAllText(_userFilePath, json);
        }
    }

    public sealed class ApplianceService
    {
        private readonly IApplianceRepository _repo;

        public ApplianceService(IApplianceRepository repo)
        {
            _repo = repo;
        }

        public IReadOnlyList<ApplianceSeed> GetSeed() => _repo.LoadSeed();
        public IReadOnlyList<ApplianceUser> GetUser() => _repo.LoadUser();

        public IReadOnlyList<ApplianceRow> GetCombinedRows(bool includeSeed, bool includeUser)
        {
            var rows = new List<ApplianceRow>();

            if (includeSeed)
            {
                var seed = _repo.LoadSeed();
                foreach (var item in seed)
                {
                    rows.Add(new ApplianceRow
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
                    rows.Add(new ApplianceRow
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

        public void AddUserItem(ApplianceUser item)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
                item.Id = "user-" + Guid.NewGuid().ToString("N");

            var list = new List<ApplianceUser>(_repo.LoadUser());
            list.Add(item);
            _repo.SaveUser(list);
        }

        public void UpdateUserItem(ApplianceUser item)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
                throw new InvalidOperationException("Липсва Id.");

            var list = new List<ApplianceUser>(_repo.LoadUser());
            var idx = list.FindIndex(x => x.Id == item.Id);
            if (idx < 0)
                throw new InvalidOperationException("Елементът не е намерен.");

            list[idx] = item;
            _repo.SaveUser(list);
        }

        public void DeleteUserItem(string id)
        {
            var list = new List<ApplianceUser>(_repo.LoadUser());
            list.RemoveAll(x => x.Id == id);
            _repo.SaveUser(list);
        }
    }
}
