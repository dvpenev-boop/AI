using System;
using System.IO;
using System.Threading.Tasks;
using EE.Doklad.Models;
using Newtonsoft.Json;

namespace EE.Doklad.Services
{
    /// <summary>
    /// Сервиз за запазване и зареждане на доклади от JSON файлове
    /// </summary>
    public class ReportStorageService
    {
        public async Task<Report?> LoadFromFileAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return null;

                var json = await File.ReadAllTextAsync(filePath);
                var report = JsonConvert.DeserializeObject<Report>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto // за полиморфни типове (FixedTable/DynamicTable)
                });

                return report;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Грешка при зареждане на доклад: {ex.Message}", ex);
            }
        }

        public async Task SaveToFileAsync(Report report, string filePath)
        {
            try
            {
                report.ModifiedDate = DateTime.Now;

                var json = JsonConvert.SerializeObject(report, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                });

                await File.WriteAllTextAsync(filePath, json);
                report.IsDirty = false;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Грешка при запазване на доклад: {ex.Message}", ex);
            }
        }
    }
}
