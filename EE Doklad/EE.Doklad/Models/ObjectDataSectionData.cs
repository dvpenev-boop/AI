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

        [ObservableProperty]
        private int _climateZone = 1; // Климатична зона (1-9)

        // Notify that HeatingSeasonInfo changed when ClimateZone changes
        partial void OnClimateZoneChanged(int value)
        {
            OnPropertyChanged(nameof(HeatingSeasonInfo));
        }

        public string HeatingSeasonInfo => ClimateZone switch
        {
            1 => "Начало: 21 октомври; Край: 20 април",
            2 => "Начало: 21 октомври; Край: 25 април",
            3 => "Начало: 23 октомври; Край: 15 април",
            4 => "Начало: 16 октомври; Край: 23 април",
            5 => "Начало: 25 октомври; Край: 19 април",
            6 => "Начало: 24 октомври; Край: 6 април",
            7 => "Начало: 15 октомври; Край: 23 април",
            8 => "Начало: 28 октомври; Край: 6 април",
            9 => "Начало: 28 октомври; Край: 5 април",
            _ => string.Empty
        };
    }
}
