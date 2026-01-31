using System.Collections.Generic;
using System.Linq;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Код на типа сграда според Приложение №2 (Скала на класовете на енергопотребление)
    /// </summary>
    public enum BuildingTypeCode
    {
        // Жилищни сгради
        MultiFamilyResidential = 1,    // Многофамилни жилищни сгради (жилищни блокове)
        SingleFamilyResidential = 2,   // Еднофамилни жилищни сгради (еднофамилни къщи)

        // Сгради за обществено обслужване
        Administrative = 10,           // Административни сгради (офиси)
        
        // Сгради за образование и наука
        Schools = 20,                  // Училища
        Universities = 21,             // Университети
        Kindergartens = 22,            // Детски градини и детски ясли
        
        Healthcare = 30,               // Сгради за здравеопазване (болници, извънболнична помощ, мед. центрове)
        HotelsRestaurants = 40,        // Хотели и ресторанти (хотелиерство, ресторантьорство и обществено хранене)
        Trade = 50,                    // Сгради за търговия (търговски услуги на едро и дребно)
        Sports = 60,                   // Сгради за спорт
        CultureArts = 70               // Сгради за култура и изкуства
    }

    /// <summary>
    /// Информация за типа сграда (display name, категория)
    /// </summary>
    public class BuildingTypeInfo
    {
        public BuildingTypeCode Code { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public BuildingTypeInfo(BuildingTypeCode code, string displayName, string category)
        {
            Code = code;
            DisplayName = displayName;
            Category = category;
        }

        /// <summary>
        /// Пълно име с категория (за display в ComboBox)
        /// </summary>
        public string FullDisplayName => $"{DisplayName}";

        /// <summary>
        /// Всички налични типове сгради
        /// </summary>
        public static List<BuildingTypeInfo> All { get; } = new()
        {
            // Жилищни сгради
            new BuildingTypeInfo(BuildingTypeCode.MultiFamilyResidential, 
                "Многофамилни жилищни сгради (жилищни блокове)", 
                "Жилищни сгради"),
            new BuildingTypeInfo(BuildingTypeCode.SingleFamilyResidential, 
                "Еднофамилни жилищни сгради (еднофамилни къщи)", 
                "Жилищни сгради"),

            // Сгради за обществено обслужване
            new BuildingTypeInfo(BuildingTypeCode.Administrative, 
                "Административни сгради (офиси)", 
                "Сгради за обществено обслужване"),
            
            // Сгради за образование и наука
            new BuildingTypeInfo(BuildingTypeCode.Schools, 
                "Училища", 
                "Сгради за образование и наука"),
            new BuildingTypeInfo(BuildingTypeCode.Universities, 
                "Университети", 
                "Сгради за образование и наука"),
            new BuildingTypeInfo(BuildingTypeCode.Kindergartens, 
                "Детски градини и детски ясли", 
                "Сгради за образование и наука"),
            
            new BuildingTypeInfo(BuildingTypeCode.Healthcare, 
                "Сгради за здравеопазване (болници, извънболнична помощ, мед. центрове)", 
                "Сгради за обществено обслужване"),
            new BuildingTypeInfo(BuildingTypeCode.HotelsRestaurants, 
                "Хотели и ресторанти (хотелиерство, ресторантьорство и обществено хранене)", 
                "Сгради за обществено обслужване"),
            new BuildingTypeInfo(BuildingTypeCode.Trade, 
                "Сгради за търговия (търговски услуги на едро и дребно)", 
                "Сгради за обществено обслужване"),
            new BuildingTypeInfo(BuildingTypeCode.Sports, 
                "Сгради за спорт", 
                "Сгради за обществено обслужване"),
            new BuildingTypeInfo(BuildingTypeCode.CultureArts, 
                "Сгради за култура и изкуства", 
                "Сгради за обществено обслужване")
        };

        /// <summary>
        /// Групирани типове сгради по категория
        /// </summary>
        public static IEnumerable<IGrouping<string, BuildingTypeInfo>> Grouped =>
            All.GroupBy(x => x.Category);

        /// <summary>
        /// Намиране на BuildingTypeInfo по код
        /// </summary>
        public static BuildingTypeInfo? GetByCode(BuildingTypeCode code) =>
            All.FirstOrDefault(x => x.Code == code);

        /// <summary>
        /// Опит за mapване на текстова стойност към BuildingTypeCode (за backward compatibility)
        /// </summary>
        public static BuildingTypeCode? TryMapFromString(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            text = text.Trim().ToLowerInvariant();

            // Директно съвпадение по display name
            var match = All.FirstOrDefault(x => x.DisplayName.ToLowerInvariant().Contains(text) || 
                                                 text.Contains(x.DisplayName.ToLowerInvariant()));
            if (match != null)
                return match.Code;

            // Опити за частично съвпадение (best effort)
            if (text.Contains("многофамил") || text.Contains("блок"))
                return BuildingTypeCode.MultiFamilyResidential;
            if (text.Contains("еднофамил") || text.Contains("къща"))
                return BuildingTypeCode.SingleFamilyResidential;
            if (text.Contains("офис") || text.Contains("администрати"))
                return BuildingTypeCode.Administrative;
            if (text.Contains("учили"))
                return BuildingTypeCode.Schools;
            if (text.Contains("университ"))
                return BuildingTypeCode.Universities;
            if (text.Contains("градин") || text.Contains("ясл"))
                return BuildingTypeCode.Kindergartens;
            if (text.Contains("болниц") || text.Contains("здраве"))
                return BuildingTypeCode.Healthcare;
            if (text.Contains("хотел") || text.Contains("ресторант"))
                return BuildingTypeCode.HotelsRestaurants;
            if (text.Contains("търгов"))
                return BuildingTypeCode.Trade;
            if (text.Contains("спорт"))
                return BuildingTypeCode.Sports;
            if (text.Contains("култур") || text.Contains("изкуств"))
                return BuildingTypeCode.CultureArts;

            return null;
        }
    }
}
