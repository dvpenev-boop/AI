using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Данни за раздел №10 - "Отопление"
    /// </summary>
    public partial class HeatingSectionData : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Отопление";

        [ObservableProperty]
        private string? _description;

        // ========== РЪЧНИ ВХОДОВЕ ==========

        /// <summary>
        /// Инфилтрация [1/ч] - десетична стойност, >= 0
        /// </summary>
        [ObservableProperty]
        private double _infiltration = 0.5;

        /// <summary>
        /// Проектна температура [°C] - може да има дробна стойност (напр. 21.00)
        /// </summary>
        [ObservableProperty]
        private double _designTemperature = 20.0;

        /// <summary>
        /// Temperatura на понижение [°C]
        /// </summary>
        [ObservableProperty]
        private double _reductionTemperature = 16.0;

        /// <summary>
        /// Ефективност на отдаване [%] (0-100)
        /// </summary>
        [ObservableProperty]
        private double _emissionEfficiency = 100.0;

        /// <summary>
        /// Ефективност на разпределителна мрежа [%] (0-100)
        /// </summary>
        [ObservableProperty]
        private double _distributionEfficiency = 100.0;

        /// <summary>
        /// Автоматично управление [%] (0-100)
        /// </summary>
        [ObservableProperty]
        private double _automaticControl = 96.0;

        /// <summary>
        /// Енергиен мениджмънт [%] (0-100)
        /// </summary>
        [ObservableProperty]
        private double _energyManagement = 96.0;

        /// <summary>
        /// КПД на топлоснабдяване [%] (0-100)
        /// </summary>
        [ObservableProperty]
        private double _heatingEfficiency = 100.0;

        // ========== ОБИТАТЕЛИ ==========

        /// <summary>
        /// Избрана степен на активност
        /// </summary>
        [ObservableProperty]
        private ActivityLevel _selectedActivityLevel = ActivityLevel.Cinema;
    }
}
