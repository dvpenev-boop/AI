using System.Collections.Generic;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Секция/лист в доклада (напр. "Обща информация", "Таблица 1", и т.н.)
    /// </summary>
    public class Section
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string StaticText { get; set; } = string.Empty;
        public List<Table> Tables { get; set; } = new();
        public int Order { get; set; }
    }
}
