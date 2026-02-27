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
            // Таблиците стартират празни - потребителят добавя редове с бутона
        }

        [ObservableProperty]
        private string _title = "Помпи и вентилатори";

        [ObservableProperty]
        private string? _description;

        // ========== 15.1 ОТОПЛЕНИЕ ==========

        // Разделени колекции: помпи и вентилатори поотделно
        [ObservableProperty]
        private ObservableCollection<PumpFanHeatingRow> _heatingPumpRows = new();

        [ObservableProperty]
        private ObservableCollection<PumpFanHeatingRow> _heatingFanRows = new();

        /// <summary>
        /// Обединена колекция за JSON сериализация и изчисления (обратна съвместимост)
        /// </summary>
        public ObservableCollection<PumpFanHeatingRow> HeatingRows
        {
            get
            {
                var all = new ObservableCollection<PumpFanHeatingRow>();
                foreach (var r in HeatingPumpRows) all.Add(r);
                foreach (var r in HeatingFanRows) all.Add(r);
                return all;
            }
            set
            {
                // При десериализация: разпределяме редовете по тип
                HeatingPumpRows.Clear();
                HeatingFanRows.Clear();
                if (value != null)
                {
                    foreach (var r in value)
                    {
                        if ((r.DeviceType ?? string.Empty).IndexOf("Вентил", System.StringComparison.OrdinalIgnoreCase) >= 0)
                            HeatingFanRows.Add(r);
                        else
                            HeatingPumpRows.Add(r);
                    }
                }
            }
        }

        [ObservableProperty]
        private string? _heatingEM = "0.96"; // Енергиен мениджмънт и поддръжка

        // Автоматично изчислени стойности
        [ObservableProperty]
        private double _heatingTotalAnnualConsumption; // kWh/год

        [ObservableProperty]
        private double _heatingSpecificPower; // W/m²

        [ObservableProperty]
        private double _heatingAnnualHours; // часове/год (read-only)

    // Под-групи за 15.1: Помпи / Вентилатори
    [ObservableProperty]
    private double _heatingPumpsTotalAnnualConsumption; // kWh/год

    [ObservableProperty]
    private double _heatingPumpsSpecificPower; // W/m²

    [ObservableProperty]
    private double _heatingFansTotalAnnualConsumption; // kWh/год

    [ObservableProperty]
    private double _heatingFansSpecificPower; // W/m²

        // ========== 15.2 ОХЛАЖДАНЕ ==========

        // Разделени колекции: помпи и вентилатори поотделно
        [ObservableProperty]
        private ObservableCollection<PumpFanCoolingRow> _coolingPumpRows = new();

        [ObservableProperty]
        private ObservableCollection<PumpFanCoolingRow> _coolingFanRows = new();

        /// <summary>
        /// Обединена колекция за JSON сериализация и изчисления (обратна съвместимост)
        /// </summary>
        public ObservableCollection<PumpFanCoolingRow> CoolingRows
        {
            get
            {
                var all = new ObservableCollection<PumpFanCoolingRow>();
                foreach (var r in CoolingPumpRows) all.Add(r);
                foreach (var r in CoolingFanRows) all.Add(r);
                return all;
            }
            set
            {
                // При десериализация: разпределяме редовете по тип
                CoolingPumpRows.Clear();
                CoolingFanRows.Clear();
                if (value != null)
                {
                    foreach (var r in value)
                    {
                        if ((r.DeviceType ?? string.Empty).IndexOf("Вентил", System.StringComparison.OrdinalIgnoreCase) >= 0)
                            CoolingFanRows.Add(r);
                        else
                            CoolingPumpRows.Add(r);
                    }
                }
            }
        }

        [ObservableProperty]
        private string? _coolingEM = "0.96"; // Енергиен мениджмънт и поддръжка

        // Автоматично изчислени стойности
        [ObservableProperty]
        private double _coolingTotalAnnualConsumption; // kWh/год

        [ObservableProperty]
        private double _coolingSpecificPower; // W/m²

        [ObservableProperty]
        private double _coolingAnnualHours; // часове/год (read-only)

    // Под-групи за 15.2: Помпи / Вентилатори
    [ObservableProperty]
    private double _coolingPumpsTotalAnnualConsumption; // kWh/год

    [ObservableProperty]
    private double _coolingPumpsSpecificPower; // W/m²

    [ObservableProperty]
    private double _coolingFansTotalAnnualConsumption; // kWh/год

    [ObservableProperty]
    private double _coolingFansSpecificPower; // W/m²

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

        public string GeneratedReportText
        {
            get
            {
                // Compose a concise report including subgroup totals
                string nl = System.Environment.NewLine;
                return
                    $"Електрическата консумация на помпите и вентилаторите е определена на база реални инсталирани мощности и автоматично изчислени годишни часове на работа.{nl}{nl}" +
                    $"15.1 Отопление:{nl}" +
                    $"  - Общо: {HeatingTotalAnnualConsumption:F2} kWh/год; Специфична мощност: {HeatingSpecificPower:F3} W/m²{nl}" +
                    $"  - 15.1.1 Помпи (отопление): {HeatingPumpsTotalAnnualConsumption:F2} kWh/год; {HeatingPumpsSpecificPower:F3} W/m²{nl}" +
                    $"  - 15.1.2 Вентилатори (вентилация): {HeatingFansTotalAnnualConsumption:F2} kWh/год; {HeatingFansSpecificPower:F3} W/m²{nl}{nl}" +
                    $"15.2 Охлаждане:{nl}" +
                    $"  - Общо: {CoolingTotalAnnualConsumption:F2} kWh/год; Специфична мощност: {CoolingSpecificPower:F3} W/m²{nl}" +
                    $"  - 15.2.1 Помпи (охлаждане): {CoolingPumpsTotalAnnualConsumption:F2} kWh/год; {CoolingPumpsSpecificPower:F3} W/m²{nl}" +
                    $"  - 15.2.2 Вентилатори (вентилация): {CoolingFansTotalAnnualConsumption:F2} kWh/год; {CoolingFansSpecificPower:F3} W/m²{nl}{nl}" +
                    $"15.3 Помпа за БГВ:{nl}" +
                    $"  - Общо: {DhwTotalAnnualConsumption:F2} kWh/год; Специфична мощност: {DhwSpecificPower:F3} W/m²{nl}{nl}" +
                    $"ОБЩО ЗА СЕКЦИЯ 15: {TotalAnnualConsumption:F2} kWh/год; Обща специфична мощност: {TotalSpecificPower:F3} W/m²";
            }
        }
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
