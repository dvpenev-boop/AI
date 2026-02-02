using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Вентилационна методология
    /// </summary>
    public enum VentilationMethodology
    {
        /// <summary>
        /// Българска методология (Наредба RD-02-20-3)
        /// </summary>
        BG,

        /// <summary>
        /// DIN методология (не е имплементирана)
        /// </summary>
        DIN
    }

    /// <summary>
    /// Тип енергиен източник за вентилация
    /// </summary>
    public enum EnergySourceType
    {
        Electricity,
        NaturalGas,
        DistrictHeating,
        Biomass,
        Solar,
        HeatPump,
        Other
    }

    /// <summary>
    /// Енергиен източник за вентилация с ефективности
    /// </summary>
    public partial class VentilationEnergySource : ObservableObject
    {
        /// <summary>
        /// Тип на енергийния източник
        /// </summary>
        [ObservableProperty]
        private EnergySourceType _type = EnergySourceType.Electricity;

        /// <summary>
        /// Дял на участие [%] (0-100)
        /// </summary>
        [ObservableProperty]
        private double _share = 100.0;

        /// <summary>
        /// Ефективност на отдаване [%] (0-100)
        /// Default = 100%
        /// </summary>
        [ObservableProperty]
        private double _emissionEfficiency = 100.0;

        /// <summary>
        /// Ефективност на разпределителната мрежа [%] (0-100)
        /// Default = 100%
        /// </summary>
        [ObservableProperty]
        private double _distributionEfficiency = 100.0;

        /// <summary>
        /// Автоматично управление [%] (0-100)
        /// Default = 100%
        /// </summary>
        [ObservableProperty]
        private double _automaticControl = 100.0;

        /// <summary>
        /// Енергиен мениджмънт (ЕМ) и поддръжка [%] (0-100)
        /// Default = 100%
        /// </summary>
        [ObservableProperty]
        private double _energyManagement = 100.0;

        /// <summary>
        /// Ефективност на генератора на топлина [%] (може да бъде >100 за термопомпи)
        /// Default = 100%
        /// </summary>
        [ObservableProperty]
        private double _generationEfficiency = 100.0;

        /// <summary>
        /// Изчислява общата ефективност на веригата
        /// </summary>
        public double TotalEfficiency =>
            (EmissionEfficiency / 100.0) *
            (DistributionEfficiency / 100.0) *
            (AutomaticControl / 100.0) *
            (EnergyManagement / 100.0) *
            (GenerationEfficiency / 100.0);
    }
}
