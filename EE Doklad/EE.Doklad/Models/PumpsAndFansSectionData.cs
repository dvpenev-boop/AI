using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Модел за Секция 15: Помпи и вентилатори
    /// </summary>
    public partial class PumpsAndFansSectionData : ObservableObject
    {
        public PumpsAndFansSectionData()
        {
            // Добавяме по един начален ред за да се вижда структурата на таблиците
            HeatingRows.Add(new PumpFanHeatingRow());
            CoolingRows.Add(new PumpFanCoolingRow());
        }

        [ObservableProperty]
        private string _title = "Помпи и вентилатори";

        [ObservableProperty]
        private string? _description;

        // ========== 15.1 ОТОПЛЕНИЕ ==========

        [ObservableProperty]
        private ObservableCollection<PumpFanHeatingRow> _heatingRows = new();

        [ObservableProperty]
        private string? _heatingEM = "0.96"; // Енергиен мениджмънт и поддръжка

        // Автоматично изчислени стойности
        [ObservableProperty]
        private double _heatingTotalAnnualConsumption; // kWh/год

        [ObservableProperty]
        private double _heatingSpecificPower; // W/m²

        [ObservableProperty]
        private double _heatingAnnualHours; // часове/год (read-only)

        // ========== 15.2 ОХЛАЖДАНЕ ==========

        [ObservableProperty]
        private ObservableCollection<PumpFanCoolingRow> _coolingRows = new();

        [ObservableProperty]
        private string? _coolingEM = "0.96"; // Енергиен мениджмънт и поддръжка

        // Автоматично изчислени стойности
        [ObservableProperty]
        private double _coolingTotalAnnualConsumption; // kWh/год

        [ObservableProperty]
        private double _coolingSpecificPower; // W/m²

        [ObservableProperty]
        private double _coolingAnnualHours; // часове/год (read-only)

        // ========== 15.3 БГВ ==========

        [ObservableProperty]
        private string? _dhwPumpNominalPower; // W

        [ObservableProperty]
        private string? _dhwPumpQuantity = "1";

        [ObservableProperty]
        private string? _dhwPumpHoursPerDay; // Ръчно въвеждане на часове/ден

        [ObservableProperty]
        private string? _dhwPumpMode; // Режим (постоянен / честотно управляем)

        [ObservableProperty]
        private string? _dhwEM = "0.96"; // Енергиен мениджмънт и поддръжка

        // Автоматично изчислени стойности
        [ObservableProperty]
        private double _dhwTotalAnnualConsumption; // kWh/год

        [ObservableProperty]
        private double _dhwSpecificPower; // W/m²

        [ObservableProperty]
        private double _dhwAnnualHours; // часове/год (read-only)

        // ========== ОБЩО ЗА СЕКЦИЯ 15 ==========

        [ObservableProperty]
        private double _totalAnnualConsumption; // kWh/год

        [ObservableProperty]
        private double _totalSpecificPower; // W/m²

        // ========== ГЕНЕРИРАН ТЕКСТ ЗА ДОКЛАД ==========

        public string GeneratedReportText =>
            "Електрическата консумация на помпите и вентилаторите е определена " +
            "на база реални инсталирани мощности и автоматично изчислени " +
            "годишни часове на работа, съобразени с климатичната зона, " +
            "отоплителния сезон, фиксирания период за охлаждане, " +
            "графиците на експлоатация и почивните дни на сградата. " +
            "Специфичната мощност [W/m²] е изчислена чрез нормализиране " +
            "спрямо отопляемата площ и коефициент за енергиен мениджмънт " +
            "и поддръжка.";
    }

    /// <summary>
    /// Ред за под-секция 15.1 ОТОПЛЕНИЕ
    /// </summary>
    public partial class PumpFanHeatingRow : ObservableObject
    {
        [ObservableProperty]
        private string? _deviceType; // Тип устройство

        [ObservableProperty]
        private string? _nominalPower; // Номинална мощност [W]

        [ObservableProperty]
        private string? _quantity = "1"; // Брой устройства

        [ObservableProperty]
        private string? _mode; // Режим (постоянен / честотно управляем)

        [ObservableProperty]
        private double _annualHours; // Годишни часове (read-only, автоматично)

        [ObservableProperty]
        private double _annualConsumption; // kWh/год (автоматично)

        // Предефинирани типове устройства за под-секция 15.1
        public static readonly string[] PredefinedTypes = new[]
        {
            "Вентилатори",
            "Помпи вентилация",
            "Помпи отопление",
            "Други (отопление)"
        };
    }

    /// <summary>
    /// Ред за под-секция 15.2 ОХЛАЖДАНЕ
    /// </summary>
    public partial class PumpFanCoolingRow : ObservableObject
    {
        [ObservableProperty]
        private string? _deviceType; // Тип устройство

        [ObservableProperty]
        private string? _nominalPower; // Номинална мощност [W]

        [ObservableProperty]
        private string? _quantity = "1"; // Брой устройства

        [ObservableProperty]
        private string? _mode; // Режим (постоянен / честотно управляем)

        [ObservableProperty]
        private double _annualHours; // Годишни часове (read-only, автоматично)

        [ObservableProperty]
        private double _annualConsumption; // kWh/год (автоматично)

        // Предефинирани типове устройства за под-секция 15.2
        public static readonly string[] PredefinedTypes = new[]
        {
            "Вентилатори (вентилация)",
            "Вентилатори (външен въздух без терм. обработка)",
            "Помпи вентилация",
            "Помпи охлаждане",
            "Други (вентилация)",
            "Други (охлаждане)"
        };
    }
}
