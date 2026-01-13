using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Секция/лист в доклада (напр. "Обща информация", "Таблица 1", и т.н.)
    /// </summary>
    public partial class Section : ObservableObject
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();

        [ObservableProperty]
        private string _title = string.Empty;

        public string StaticText { get; set; } = string.Empty;
        public List<Table> Tables { get; set; } = new();
        public int Order { get; set; }
    }
}
