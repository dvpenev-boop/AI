using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Фаза на проектиране
    /// </summary>
    public enum ProjectPhase
    {
        Ideynyi,      // Идеен проект
        Tehnicheski,  // Технически проект
        Raboten       // Работен проект
    }

    /// <summary>
    /// Разработил/Експерт в екипа
    /// </summary>
    public partial class Developer : ObservableObject
    {
        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _position = string.Empty;
    }

    /// <summary>
    /// Данни за челната страница на доклада
    /// </summary>
    public partial class CoverPageData : ObservableObject
    {
        /// <summary>
        /// Път до лого на фирмата (optional)
        /// </summary>
        [ObservableProperty]
        private string? _logoPath;

        /// <summary>
        /// Име на фирмата (required)
        /// </summary>
        [ObservableProperty]
        private string _companyName = string.Empty;

        /// <summary>
        /// Лиценз номер (required)
        /// </summary>
        [ObservableProperty]
        private string _licenseNumber = string.Empty;

        /// <summary>
        /// Име на обекта (required)
        /// </summary>
        [ObservableProperty]
        private string _objectName = string.Empty;

        /// <summary>
        /// Адрес на обекта (required, multi-line)
        /// </summary>
        [ObservableProperty]
        private string _objectAddress = string.Empty;

        /// <summary>
        /// Фаза на проектиране
        /// </summary>
        [ObservableProperty]
        private ProjectPhase _phase = ProjectPhase.Tehnicheski;

        /// <summary>
        /// Име на управител
        /// </summary>
        [ObservableProperty]
        private string _managerName = string.Empty;

        /// <summary>
        /// Списък на разработилите (1..N)
        /// </summary>
        public ObservableCollection<Developer> Developers { get; set; } = new();

        public CoverPageData()
        {
            // Поне един разработил по подразбиране
            Developers.Add(new Developer { Name = "", Position = "Енергиен експерт" });
        }
    }
}
