using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Данни за раздел №5 - "Данни за обекта"
    /// </summary>
    public partial class ObjectDataSectionData : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Данни за обекта";

        [ObservableProperty]
        private string? _description;

        [ObservableProperty]
        private string? _buildingName;

        [ObservableProperty]
        private string? _address;

        [ObservableProperty]
        private string? _buildingType;

        [ObservableProperty]
        private string? _ownership;

        [ObservableProperty]
        private string? _yearOfConstruction;

        [ObservableProperty]
        private string? _numberOfOccupants;

        [ObservableProperty]
        private string _occupancySchedule = "24 ч./ден за всички дни";

        [ObservableProperty]
        private string _heatingSchedule = "24 ч./ден за всички дни";
    }
}
