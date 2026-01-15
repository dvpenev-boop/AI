using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Тип на секцията
    /// </summary>
    public enum SectionType
    {
        Normal,         // Обикновена секция с таблици и текст
        CoverPage,      // Челна страница (фиксирана на позиция №1)
        Certificates,   // Удостоверения (фиксирана на позиция №2)
        ObjectData      // Данни за обекта (фиксирана на позиция №3)
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

        /// <summary>
        /// Данни за секция "Удостоверения" (само за Certificates секции)
        /// </summary>
        public CertificatesSectionData? CertificatesData { get; set; }

        /// <summary>
        /// Данни за раздел "Данни за обекта" (само за ObjectData секции)
        /// </summary>
        public ObjectDataSectionData? ObjectDataSectionData { get; set; }

        public int Order { get; set; }

        /// <summary>
        /// Дали секцията е системна и не може да се изтрие/премести
        /// </summary>
        public bool IsSystemSection => Type == SectionType.CoverPage || Type == SectionType.Certificates || Type == SectionType.ObjectData;
    }
}
