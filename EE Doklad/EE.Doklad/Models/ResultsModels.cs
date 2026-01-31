using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Енергиен носител (енергоресурс) според Приложение №3
    /// </summary>
    public enum EnergyCarrierCode
    {
        // ИЗКОПАЕМИ ГОРИВА
        FossilSolid = 1,              // Твърдо
        FossilLiquid = 2,             // Течно
        FossilGas = 3,                // Газообразно

        // БИОГОРИВА
        BioSolid = 10,                // Биогориво твърдо
        BioLiquid = 11,               // Биогориво течно
        BioGas = 12,                  // Биогориво газообразно

        // ЦЕНТРАЛИЗИРАНО ТОПЛОСНАБДЯВАНЕ
        DistrictHeating = 20,         // Топлина от централизирано топлоснабдяване

        // ЕЛЕКТРИЧЕСТВО ОТ ОТДАЛЕЧЕН ИЗТОЧНИК
        Electricity = 30,             // Електричество

        // ЕНЕРГИЯ, ПОДАВАНА НА МЯСТО И В БЛИЗОСТ
        SolarPV = 40,                 // Слънчева – PV електричество
        SolarThermal = 41,            // Слънчева – термална
        Wind = 42,                    // Вятърна
        AmbientEnergy = 43            // От околната среда: гео-, аеро-, хидротермална
    }

    /// <summary>
    /// Информация за енергиен носител с коефициенти
    /// </summary>
    public class EnergyCarrierInfo
    {
        public EnergyCarrierCode Code { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        // Коефициенти първична енергия
        public double FpNren { get; set; }  // fp,nren - първична невъзобновяема
        public double FpRen { get; set; }   // fp,ren  - първична възобновяема
        public double FpTot { get; set; }   // fp,tot  - първична общо

        // Коефициент за емисии CO2
        public double KCO2e { get; set; }   // KCO2e [g CO2e/kWh]

        public EnergyCarrierInfo(
            EnergyCarrierCode code,
            string displayName,
            string category,
            double fpNren,
            double fpRen,
            double fpTot,
            double kco2e)
        {
            Code = code;
            DisplayName = displayName;
            Category = category;
            FpNren = fpNren;
            FpRen = fpRen;
            FpTot = fpTot;
            KCO2e = kco2e;
        }

        /// <summary>
        /// Всички налични енергийни носители според таблицата в изискванията
        /// </summary>
        public static List<EnergyCarrierInfo> All { get; } = new()
        {
            // ИЗКОПАЕМИ ГОРИВА
            new EnergyCarrierInfo(
                EnergyCarrierCode.FossilSolid,
                "Твърдо",
                "Изкопаеми горива",
                1.1, 0.0, 1.1, 360),

            new EnergyCarrierInfo(
                EnergyCarrierCode.FossilLiquid,
                "Течно",
                "Изкопаеми горива",
                1.1, 0.0, 1.1, 290),

            new EnergyCarrierInfo(
                EnergyCarrierCode.FossilGas,
                "Газообразно",
                "Изкопаеми горива",
                1.1, 0.0, 1.1, 220),

            // БИОГОРИВА
            new EnergyCarrierInfo(
                EnergyCarrierCode.BioSolid,
                "Биогориво твърдо",
                "Биогорива",
                0.2, 1.0, 1.2, 40),

            new EnergyCarrierInfo(
                EnergyCarrierCode.BioLiquid,
                "Биогориво течно",
                "Биогорива",
                0.5, 1.0, 1.5, 70),

            new EnergyCarrierInfo(
                EnergyCarrierCode.BioGas,
                "Биогориво газообразно",
                "Биогорива",
                0.4, 1.0, 1.4, 100),

            // ЦЕНТРАЛИЗИРАНО ТОПЛОСНАБДЯВАНЕ
            new EnergyCarrierInfo(
                EnergyCarrierCode.DistrictHeating,
                "Топлина от централизирано топлоснабдяване",
                "Централизирано топлоснабдяване",
                1.3, 0.0, 1.3, 290),

            // ЕЛЕКТРИЧЕСТВО ОТ ОТДАЛЕЧЕН ИЗТОЧНИК
            new EnergyCarrierInfo(
                EnergyCarrierCode.Electricity,
                "Електричество",
                "Електричество от отдалечен източник",
                2.3, 0.2, 2.5, 486),

            // ЕНЕРГИЯ НА МЯСТО/В БЛИЗОСТ (ВЕИ)
            new EnergyCarrierInfo(
                EnergyCarrierCode.SolarPV,
                "Слънчева – PV електричество",
                "Енергия, подавана на място и в близост",
                0.0, 1.0, 1.0, 0),

            new EnergyCarrierInfo(
                EnergyCarrierCode.SolarThermal,
                "Слънчева – термална",
                "Енергия, подавана на място и в близост",
                0.0, 1.0, 1.0, 0),

            new EnergyCarrierInfo(
                EnergyCarrierCode.Wind,
                "Вятърна",
                "Енергия, подавана на място и в близост",
                0.0, 1.0, 1.0, 0),

            new EnergyCarrierInfo(
                EnergyCarrierCode.AmbientEnergy,
                "От околната среда: гео-, аеро-, хидротермална",
                "Енергия, подавана на място и в близост",
                0.0, 1.0, 1.0, 0)
        };

        /// <summary>
        /// Групирани енергийни носители по категория
        /// </summary>
        public static IEnumerable<IGrouping<string, EnergyCarrierInfo>> Grouped =>
            All.GroupBy(x => x.Category);

        /// <summary>
        /// Намиране на EnergyCarrierInfo по код
        /// </summary>
        public static EnergyCarrierInfo? GetByCode(EnergyCarrierCode code) =>
            All.FirstOrDefault(x => x.Code == code);
    }

    /// <summary>
    /// Ред от таблицата "Резултати" (потребена енергия по енергоносител)
    /// </summary>
    public partial class ResultsRowData : ObservableObject
    {
        /// <summary>
        /// Име на реда (фиксирано)
        /// </summary>
        public string RowName { get; set; } = string.Empty;

        /// <summary>
        /// Дали редът е изчислен автоматично (сбор на подредове)
        /// </summary>
        public bool IsCalculated { get; set; }

        /// <summary>
        /// Индекси на подредовете, ако IsCalculated = true
        /// </summary>
        public int[]? SubRowIndices { get; set; }

        [ObservableProperty]
        private string? _specificConsumption; // Специфичен разход [kWh/m²] - auto calculated

        [ObservableProperty]
        private string? _consumedEnergy; // Потребена енергия Q [kWh]

        [ObservableProperty]
        private EnergyCarrierCode? _energyCarrier; // Енергиен носител

        // Автоматично изчислени полета (read-only)

        /// <summary>
        /// Първична невъзобновяема енергия [kWh]
        /// fpNren_kWh = Q * fp,nren
        /// </summary>
        public double FpNrenKWh
        {
            get
            {
                if (!EnergyCarrier.HasValue) return 0;
                var info = EnergyCarrierInfo.GetByCode(EnergyCarrier.Value);
                if (info == null) return 0;

                if (!double.TryParse(ConsumedEnergy, out double q)) return 0;
                return q * info.FpNren;
            }
        }

        /// <summary>
        /// Първична възобновяема енергия [kWh]
        /// fpRen_kWh = Q * fp,ren
        /// </summary>
        public double FpRenKWh
        {
            get
            {
                if (!EnergyCarrier.HasValue) return 0;
                var info = EnergyCarrierInfo.GetByCode(EnergyCarrier.Value);
                if (info == null) return 0;

                if (!double.TryParse(ConsumedEnergy, out double q)) return 0;
                return q * info.FpRen;
            }
        }

        /// <summary>
        /// Първична обща енергия [kWh]
        /// fpTot_kWh = Q * fp,tot
        /// </summary>
        public double FpTotKWh
        {
            get
            {
                if (!EnergyCarrier.HasValue) return 0;
                var info = EnergyCarrierInfo.GetByCode(EnergyCarrier.Value);
                if (info == null) return 0;

                if (!double.TryParse(ConsumedEnergy, out double q)) return 0;
                return q * info.FpTot;
            }
        }

        /// <summary>
        /// Емисии CO2 [tCO2]
        /// EmCO2_t = Q * KCO2e * 1e-6
        /// </summary>
        public double EmCO2Tonnes
        {
            get
            {
                if (!EnergyCarrier.HasValue) return 0;
                var info = EnergyCarrierInfo.GetByCode(EnergyCarrier.Value);
                if (info == null) return 0;

                if (!double.TryParse(ConsumedEnergy, out double q)) return 0;
                return q * info.KCO2e * 1e-6; // KCO2e е в g/kWh, резултатът е в tCO2
            }
        }

        // Notify computed properties when dependencies change
        partial void OnConsumedEnergyChanged(string? value)
        {
            OnPropertyChanged(nameof(FpNrenKWh));
            OnPropertyChanged(nameof(FpRenKWh));
            OnPropertyChanged(nameof(FpTotKWh));
            OnPropertyChanged(nameof(EmCO2Tonnes));
        }

        partial void OnEnergyCarrierChanged(EnergyCarrierCode? value)
        {
            OnPropertyChanged(nameof(FpNrenKWh));
            OnPropertyChanged(nameof(FpRenKWh));
            OnPropertyChanged(nameof(FpTotKWh));
            OnPropertyChanged(nameof(EmCO2Tonnes));
        }
    }

    /// <summary>
    /// Данни за секция "Резултати"
    /// </summary>
    public partial class ResultsSectionData : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Резултати сграда";

        [ObservableProperty]
        private string? _description;

        /// <summary>
        /// Отопляема площ [m²] - от ObjectData
        /// </summary>
        [ObservableProperty]
        private double? _heatedArea;

        /// <summary>
        /// Редове в таблицата с резултати (фиксирани)
        /// </summary>
        public ObservableCollection<ResultsRowData> Rows { get; set; } = new();

        /// <summary>
        /// Обща първична невъзобновяема енергия (сума) [kWh]
        /// </summary>
        public double TotalFpNrenKWh => Rows.Sum(r => r.FpNrenKWh);

        /// <summary>
        /// Обща първична възобновяема енергия (сума) [kWh]
        /// </summary>
        public double TotalFpRenKWh => Rows.Sum(r => r.FpRenKWh);

        /// <summary>
        /// Обща първична енергия (сума) [kWh]
        /// </summary>
        public double TotalFpTotKWh => Rows.Sum(r => r.FpTotKWh);

        /// <summary>
        /// Общи емисии CO2 (сума) [tCO2]
        /// </summary>
        public double TotalEmCO2Tonnes => Rows.Sum(r => r.EmCO2Tonnes);

        /// <summary>
        /// EP - Годишна специфична енергия от ред "ОБЩО" [kWh/m²]
        /// Използва се в секция 19 "Клас на енергопотребление"
        /// </summary>
        public double? TotalSpecificConsumption
        {
            get
            {
                var totalRow = Rows.FirstOrDefault(r => r.RowName == "Общо");
                if (totalRow == null)
                    return null;

                if (string.IsNullOrWhiteSpace(totalRow.SpecificConsumption) || totalRow.SpecificConsumption == "—")
                    return null;

                if (double.TryParse(totalRow.SpecificConsumption, out double value))
                    return value;

                return null;
            }
        }

        public ResultsSectionData()
        {
            InitializeFixedRows();
            
            // Абонираме се за промени в редовете
            Rows.CollectionChanged += (s, e) =>
            {
                if (e.OldItems != null)
                {
                    foreach (ResultsRowData row in e.OldItems)
                    {
                        row.PropertyChanged -= Row_PropertyChanged;
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (ResultsRowData row in e.NewItems)
                    {
                        row.PropertyChanged += Row_PropertyChanged;
                    }
                }

                NotifyTotalsChanged();
            };

            // Subscribe to all existing rows
            foreach (var row in Rows)
            {
                row.PropertyChanged += Row_PropertyChanged;
            }
        }

        /// <summary>
        /// Инициализира фиксираните редове на таблицата
        /// </summary>
        private void InitializeFixedRows()
        {
            Rows.Clear();
            
            Rows.Add(new ResultsRowData { RowName = "Отопление", IsCalculated = true, SubRowIndices = new[] { 1, 2 } });
            Rows.Add(new ResultsRowData { RowName = "Отопление Енергоносител 1" });
            Rows.Add(new ResultsRowData { RowName = "Отопление Енергоносител 2" });
            Rows.Add(new ResultsRowData { RowName = "Охлаждане" });
            Rows.Add(new ResultsRowData { RowName = "Вентилация Отопление", IsCalculated = true, SubRowIndices = new[] { 5, 6 } });
            Rows.Add(new ResultsRowData { RowName = "Вентилация Енергоносител 1" });
            Rows.Add(new ResultsRowData { RowName = "Вентилация Енергоносител 2" });
            Rows.Add(new ResultsRowData { RowName = "Вентилация Охлаждане" });
            Rows.Add(new ResultsRowData { RowName = "БГВ", IsCalculated = true, SubRowIndices = new[] { 9, 10 } });
            Rows.Add(new ResultsRowData { RowName = "БГВ енергоносител 1" });
            Rows.Add(new ResultsRowData { RowName = "БГВ енергоносител 2" });
            Rows.Add(new ResultsRowData { RowName = "Осветление" });
            Rows.Add(new ResultsRowData { RowName = "Помпи и Вентилатори" });
            Rows.Add(new ResultsRowData { RowName = "Уреди влияещи" });
            Rows.Add(new ResultsRowData { RowName = "Уреди невлияещи" });
            Rows.Add(new ResultsRowData { RowName = "Други" });
            Rows.Add(new ResultsRowData { RowName = "Общо", IsCalculated = true });
            Rows.Add(new ResultsRowData { RowName = "ВЕИ Термопомпа/пелети" });
            Rows.Add(new ResultsRowData { RowName = "ВЕИ Слънчеви колектори за БГВ" });
            Rows.Add(new ResultsRowData { RowName = "PV централа усвоена ен." });
            Rows.Add(new ResultsRowData { RowName = "PV централа отдадено към мрежата" });
            Rows.Add(new ResultsRowData { RowName = "PV централа наблизо общо" });
        }

        private void Row_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not ResultsRowData changedRow) return;

            // Когато се промени ConsumedEnergy, обновяваме изчислените редове и SpecificConsumption
            if (e.PropertyName == nameof(ResultsRowData.ConsumedEnergy))
            {
                UpdateCalculatedRows();
                UpdateSpecificConsumption(changedRow);
                NotifyTotalsChanged();
            }
            
            // Когато се промени EnergyCarrier, обновяваме computed properties
            if (e.PropertyName == nameof(ResultsRowData.FpNrenKWh) ||
                e.PropertyName == nameof(ResultsRowData.FpRenKWh) ||
                e.PropertyName == nameof(ResultsRowData.FpTotKWh) ||
                e.PropertyName == nameof(ResultsRowData.EmCO2Tonnes))
            {
                NotifyTotalsChanged();
            }
        }

        /// <summary>
        /// Обновява изчислените редове (Отопление, Вентилация Отопление, БГВ, Общо)
        /// </summary>
        private void UpdateCalculatedRows()
        {
            foreach (var row in Rows.Where(r => r.IsCalculated))
            {
                if (row.RowName == "Общо")
                {
                    // Общо е сбор от всички редове освен самото Общо и ВЕИ/PV редовете
                    var sum = Rows.Where(r => !r.IsCalculated && 
                                               !r.RowName.StartsWith("ВЕИ") && 
                                               !r.RowName.StartsWith("PV") &&
                                               r.RowName != "Общо")
                                   .Sum(r => double.TryParse(r.ConsumedEnergy, out double val) ? val : 0);
                    row.ConsumedEnergy = sum.ToString("F2");
                }
                else if (row.SubRowIndices != null)
                {
                    // Сбор от подредовете
                    var sum = row.SubRowIndices.Sum(idx =>
                    {
                        if (idx < Rows.Count)
                        {
                            var subRow = Rows[idx];
                            return double.TryParse(subRow.ConsumedEnergy, out double val) ? val : 0;
                        }
                        return 0;
                    });
                    row.ConsumedEnergy = sum.ToString("F2");
                }
            }
        }

        /// <summary>
        /// Обновява специфичен разход (kWh/m²) за даден ред
        /// </summary>
        private void UpdateSpecificConsumption(ResultsRowData row)
        {
            if (!HeatedArea.HasValue || HeatedArea.Value <= 0)
            {
                row.SpecificConsumption = "—";
                return;
            }

            if (double.TryParse(row.ConsumedEnergy, out double consumed))
            {
                var specific = consumed / HeatedArea.Value;
                row.SpecificConsumption = specific.ToString("F2");
            }
            else
            {
                row.SpecificConsumption = "—";
            }

            // Notify TotalSpecificConsumption when "Общо" row changes
            if (row.RowName == "Общо")
            {
                OnPropertyChanged(nameof(TotalSpecificConsumption));
            }
        }

        /// <summary>
        /// Обновява специфичен разход за всички редове
        /// </summary>
        public void UpdateAllSpecificConsumptions()
        {
            foreach (var row in Rows)
            {
                UpdateSpecificConsumption(row);
            }
        }

        partial void OnHeatedAreaChanged(double? value)
        {
            UpdateAllSpecificConsumptions();
        }

        private void NotifyTotalsChanged()
        {
            OnPropertyChanged(nameof(TotalFpNrenKWh));
            OnPropertyChanged(nameof(TotalFpRenKWh));
            OnPropertyChanged(nameof(TotalFpTotKWh));
            OnPropertyChanged(nameof(TotalEmCO2Tonnes));
            OnPropertyChanged(nameof(TotalSpecificConsumption));
        }
    }
}
