using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Данни за раздел №12 - "Охлаждане"
    /// </summary>
    public partial class CoolingSectionData : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Охлаждане";

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
    private double _designTemperature = 25.0;

        /// <summary>
        /// Temperatura на понижение [°C]
        /// </summary>
        [ObservableProperty]
    private double _reductionTemperature = 25.0;

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
        /// КПД на охладителната система [%] (>=0, може да бъде над 100)
        /// </summary>
        [ObservableProperty]
        private double _coolingEfficiency = 100.0;

        // ========== ЕНЕРГИЕН ИЗТОЧНИК ЗА ОХЛАЖДАНЕ ==========

        /// <summary>
        /// Енергиен източник 1 (ЕИ1)
        /// </summary>
        [ObservableProperty]
        private VentilationEnergySource _energySource1 = new();

        /// <summary>
        /// Енергиен източник 2 (ЕИ2) - опционален
        /// </summary>
        [ObservableProperty]
        private VentilationEnergySource? _energySource2 = null;

        /// <summary>
        /// Използва ли се втори енергиен източник
        /// </summary>
        [ObservableProperty]
        private bool _useSecondEnergySource = false;

        // ========== ОБИТАТЕЛИ ==========

        /// <summary>
        /// Избрана степен на активност
        /// </summary>
        [ObservableProperty]
        private ActivityLevel _selectedActivityLevel = ActivityLevel.Cinema;

    /// <summary>
    /// Относителна влажност [%] - десетична стойност 0-100
    /// </summary>
    [ObservableProperty]
    private double _relativeHumidity = 50.0;
    }
}
