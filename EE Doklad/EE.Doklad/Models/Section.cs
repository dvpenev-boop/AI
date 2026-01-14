using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Тип на секцията
    /// </summary>
    public enum SectionType
    {
        Normal,      // Обикновена секция с таблици и текст
        CoverPage    // Челна страница (фиксирана на позиция №1)
    }

    /// <summary>
    /// Секция/лист в доклада (напр. "Обща информация", "Таблица 1", и т.н.)
    /// </summary>
    public partial class Section : ObservableObject
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();

        [ObservableProperty]
        private string _title = string.Empty;

        /// <summary>
        /// Тип на секцията (Normal или CoverPage)
        /// </summary>
        public SectionType Type { get; set; } = SectionType.Normal;

        /// <summary>
        /// Статичен текст (само за Normal секции)
        /// </summary>
        public string StaticText { get; set; } = string.Empty;

        /// <summary>
        /// Таблици (само за Normal секции)
        /// </summary>
        public List<Table> Tables { get; set; } = new();

        /// <summary>
        /// Данни за челна страница (само за CoverPage секции)
        /// </summary>
        public CoverPageData? CoverPageData { get; set; }

        public int Order { get; set; }
    }
}
