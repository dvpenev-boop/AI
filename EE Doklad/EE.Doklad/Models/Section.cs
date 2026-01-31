using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using EE.Doklad.Models;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Тип на секцията
    /// </summary>
    public enum SectionType
    {
        Normal,                     // Обикновена секция с таблици и текст
        CoverPage,                  // Челна страница (фиксирана на позиция №1)
        Certificates,               // Удостоверения (фиксирана на позиция №2)
        ObjectData,                 // Данни за обекта (фиксирана на позиция №3)
        ExternalWalls,              // Външни стени (секция №6)
        Roof,                       // Покрив (секция №7)
        Floor,                      // Под (секция №8)
        Windows,                    // Прозорци и врати (секция №9)
        Heating,                    // Отопление (секция №10)
        Lighting,                   // Осветление (секция №15)
        AppliancesAffecting,        // Други разходи влияещи (секция №16)
        AppliancesNotAffecting,     // Други разходи невлияещи (секция №17)
        Results,                    // Резултати (секция за изчисления)
        EnergyClass                 // Клас на енергопотребление
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


        /// <summary>
        /// Данни за раздел "Външни стени" (само за ExternalWalls секции)
        /// </summary>
        public ExternalWallsSectionData? ExternalWallsSectionData { get; set; }

        /// <summary>
        /// Данни за раздел "Покрив" (само за Roof секции)
        /// </summary>
        public RoofSectionData? RoofSectionData { get; set; }

        /// <summary>
        /// Данни за раздел "Под" (само за Floor секции)
        /// </summary>
        public FloorSectionData? FloorSectionData { get; set; }

        /// <summary>
        /// Данни за раздел "Прозорци и врати" (само за Windows секции)
        /// </summary>
        public WindowsSectionData? WindowsSectionData { get; set; }

        /// <summary>
        /// Данни за раздел "Отопление" (само за Heating секции)
        /// </summary>
        public HeatingSectionData? HeatingSectionData { get; set; }

        /// <summary>
        /// Данни за раздел "Осветление" (само за Lighting секции)
        /// </summary>
        public LightingSectionData? LightingSectionData { get; set; }

        /// <summary>
        /// Данни за раздел "Други разходи влияещи" (само за AppliancesAffecting секции)
        /// </summary>
        public AppliancesSectionData? AppliancesAffectingSectionData { get; set; }

        /// <summary>
        /// Данни за раздел "Други разходи невлияещи" (само за AppliancesNotAffecting секции)
        /// </summary>
        public AppliancesSectionData? AppliancesNotAffectingSectionData { get; set; }

        /// <summary>
        /// Данни за раздел "Резултати" (само за Results секции)
        /// </summary>
        public ResultsSectionData? ResultsSectionData { get; set; }

        /// <summary>
        /// Данни за раздел "Клас на енергопотребление" (само за EnergyClass секции)
        /// </summary>
        public EnergyClassSectionData? EnergyClassSectionData { get; set; }

        public int Order { get; set; }

        /// <summary>
        /// Дали секцията е системна и не може да се изтрие/премести
        /// </summary>
        public bool IsSystemSection => Type == SectionType.CoverPage || Type == SectionType.Certificates || Type == SectionType.ObjectData;
    }
}
