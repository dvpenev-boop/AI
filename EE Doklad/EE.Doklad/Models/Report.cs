using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using EE.Doklad.Models.Climate;

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

        /// <summary>
        /// Включени секции за експорт в PDF (по SectionType).
        /// Ако е null или празен - всички секции са включени.
        /// </summary>
        public HashSet<string>? EnabledSections { get; set; }

        /// <summary>
        /// Вградени EPW климатични данни (ако е избран ASHRAE/EPW източник).
        /// Сериализира се заедно с доклада - пер-доклад данни без external dependencies.
        /// </summary>
        public EpwEmbeddedData? EmbeddedEpwData { get; set; }

        /// <summary>
        /// Проверява дали дадена секция е включена за експорт.
        /// По подразбиране всички секции са включени.
        /// </summary>
        public bool IsSectionEnabled(SectionType sectionType)
        {
            if (EnabledSections == null || EnabledSections.Count == 0)
                return true; // Всички секции са включени по подразбиране

            return EnabledSections.Contains(sectionType.ToString());
        }

        [JsonIgnore]
        public bool IsDirty { get; set; }
    }
}
