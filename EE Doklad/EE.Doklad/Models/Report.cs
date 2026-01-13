using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Представлява цял доклад, съдържащ множество секции
    /// </summary>
    public class Report
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "Нов доклад";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public ObservableCollection<Section> Sections { get; set; } = new();

        [JsonIgnore]
        public bool IsDirty { get; set; }
    }
}
