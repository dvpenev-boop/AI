using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    // ══════════════════════════════════════════════════════════════════════════
    // Модели за изчисление на регенерируеми загуби от разпределителна система
    // БГВ – по EN 15316-3 / ISO 52003 методика
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Режим за въвеждане на регенерируеми загуби от БГВ разпределение
    /// </summary>
    public enum DhwLossMode
    {
        /// <summary>
        /// Режим A: Ръчно въвеждане на kWh/год – стойността се пази без изчисление
        /// </summary>
        Manual = 0,

        /// <summary>
        /// Режим B: Автоматично изчисление по методиката с тръбни сегменти
        /// </summary>
        Automatic = 1,

        /// <summary>
        /// Режим C: % дял от общите загуби (ако kWh/год е 0/празно)
        /// </summary>
        PercentShare = 2
    }

    /// <summary>
    /// Тип тръба за изчисляване на Ψ
    /// </summary>
    public enum PipeInsulationType
    {
        /// <summary>Изолирана тръба във въздух – формула (1.3)</summary>
        InsulatedInAir = 0,

        /// <summary>Вградена тръба в материал – формула (1.4)</summary>
        EmbeddedInMaterial = 1,

        /// <summary>Неизолирана тръба – формула (1.5)/(1.6)</summary>
        Uninsulated = 2,

        /// <summary>Директно въведена Ψ стойност [W/(m·K)]</summary>
        DirectPsi = 3
    }

    /// <summary>
    /// Зонален тип за тръбен сегмент
    /// </summary>
    public enum PipeZoneType
    {
        /// <summary>Кондиционирана зона (загубите са регенерируеми)</summary>
        Conditioned = 0,

        /// <summary>Некондиционирана зона (загубите не са регенерируеми)</summary>
        Unconditioned = 1
    }

    // ──────────────────────────────────────────────────────────────────────────
    // PipeSegment – един тръбен участък
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Един тръбен участък в БГВ разпределителната система.
    /// Поддържа всички типове тръби: изолирани, вградени, неизолирани, директно Ψ.
    /// </summary>
    public partial class PipeSegment : ObservableObject
    {
        [ObservableProperty] private string _name = "Сегмент";

        /// <summary>Зонален тип – кондиционирана / некондиционирана</summary>
        [ObservableProperty] private PipeZoneType _zoneType = PipeZoneType.Conditioned;

        /// <summary>Реална дължина L [m]</summary>
        [ObservableProperty] private double _length_m = 10.0;

        /// <summary>Еквивалентна дължина L_equi [m] (за вентили, окачвания и др.)</summary>
        [ObservableProperty] private double _equivalentLength_m = 0.0;

        /// <summary>Тип изолация/тръба – определя коя формула се ползва за Ψ</summary>
        [ObservableProperty] private PipeInsulationType _insulationType = PipeInsulationType.DirectPsi;

        // ── Директна Ψ ──────────────────────────────────────────────────────
        /// <summary>
        /// Директно въведена Ψ [W/(m·K)] (ако InsulationType == DirectPsi)
        /// Формула (1.3): Ψ = π / ( (1/(2·λ_D))·ln(d_a/d_i) + 1/(h_a·d_a) )
        /// </summary>
        [ObservableProperty] private double _psi_WmK = 0.5;

        // ── Параметри за изолирана тръба (InsulatedInAir) ──────────────────
        /// <summary>Вътрешен диаметър d_i [m] (без изолация)</summary>
        [ObservableProperty] private double _innerDiameter_m = 0.020;

        /// <summary>Външен диаметър с изолация d_a [m]</summary>
        [ObservableProperty] private double _outerDiameterWithInsulation_m = 0.060;

        /// <summary>Топлопроводност на изолацията λ_D [W/(m·K)]</summary>
        [ObservableProperty] private double _insulationLambda_WmK = 0.04;

    /// <summary>Коефициент топлоотдаване на външна повърхност h_a [W/(m²·K)]</summary>
    [ObservableProperty] private double _surfaceHeatTransfer_WmK = 8.0;

        // ── Параметри за вградена тръба (EmbeddedInMaterial) ───────────────
        /// <summary>Топлопроводност на обкръжаващия материал λ_em [W/(m·K)]</summary>
        [ObservableProperty] private double _embeddingMaterialLambda_WmK = 1.5;

        /// <summary>Дълбочина на тръбата от повърхността z [m]</summary>
        [ObservableProperty] private double _depthFromSurface_m = 0.05;

        // ── Параметри за неизолирана тръба (Uninsulated) ───────────────────
        /// <summary>Топлопроводност на материала на тръбата λ_p [W/(m·K)]</summary>
        [ObservableProperty] private double _pipeMaterialLambda_WmK = 50.0;

        /// <summary>Вътрешен диаметър на тръбата d_p,i [m]</summary>
        [ObservableProperty] private double _pipeInnerDiameter_m = 0.020;

        /// <summary>Външен диаметър на тръбата d_p,a [m]</summary>
        [ObservableProperty] private double _pipeOuterDiameter_m = 0.025;

        /// <summary>
        /// Приближена формула за Ψ_non ≈ h_a · π · d_p,a – формула (1.6)
        /// Ако true, ползва опростената формула вместо (1.5)
        /// </summary>
        [ObservableProperty] private bool _useApproximatePsiForUninsulated = false;

        // ── Stub (отворено отклонение) ──────────────────────────────────────
        /// <summary>True ако сегментът е „stub" отклонение с отворена циркулация</summary>
        [ObservableProperty] private bool _isStub = false;

        /// <summary>Изчислена Ψ стойност [W/(m·K)] след последно изчисление – известява UI</summary>
        [ObservableProperty] private double _computedPsi_WmK = 0.0;

        // ── Фитинги (за автоматично изчисляване на L_equi) ──────────────────

        /// <summary>
        /// AutoLequi = true → L_equi се преизчислява автоматично от броя фитинги.
        /// AutoLequi = false → L_equi се въвежда ръчно.
        /// </summary>
        [ObservableProperty] private bool _autoLequi = true;

        /// <summary>Брой колена 90° за сегмента</summary>
        [ObservableProperty] private int _elbow90Count = 0;

        /// <summary>Брой тройници (разклонения) за сегмента</summary>
        [ObservableProperty] private int _teeBranchCount = 0;

        /// <summary>Брой сферични вентили за сегмента</summary>
        [ObservableProperty] private int _ballValveCount = 0;

        /// <summary>
        /// Показва избрания DN (текстово) – за визуализация в UI.
        /// Попълва се от ViewModel при преизчисление на L_equi.
        /// </summary>
        [ObservableProperty] private string _selectedDnDisplay = "";

        /// <summary>
        /// Warning текст ако da не съвпада добре с DN таблицата (|da - dnRef| > 10 mm).
        /// Празен string = няма предупреждение.
        /// </summary>
        [ObservableProperty] private string _dnWarning = "";
    }

    // ──────────────────────────────────────────────────────────────────────────
    // StubZoneData – отворени отклонения в зона
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Данни за stub отклонения (отворена циркулация) в дадена зона.
    /// Използва се за формула (1.8) и (1.9).
    /// </summary>
    public partial class StubZoneData : ObservableObject
    {
        [ObservableProperty] private PipeZoneType _zoneType = PipeZoneType.Conditioned;

        /// <summary>
        /// Общ обем на тръбите в отклоненията V_stub,j [m³]
        /// Формула (1.9): ṁ_w,dis,stub = Σ_j V_stub,j · ρ_w · n_tap,j
        /// </summary>
        [ObservableProperty] private double _stubVolume_m3 = 0.001;

        /// <summary>
        /// Брой источвания на час n_tap,j [1/h]
        /// </summary>
        [ObservableProperty] private double _tappingFrequency_perHour = 0.5;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // DhwLossInputs – всички входни данни за изчислението
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Входни параметри за изчисляване на регенерируеми загуби
    /// от БГВ разпределителна система.
    /// </summary>
    public partial class DhwLossInputs : ObservableObject
    {
        // ── Температури ─────────────────────────────────────────────────────

        /// <summary>
        /// Зададена температура на БГВ θ_w,set [°C]
        /// Ако е 0, се изчислява от TemperatureDifference + θ_amb_cond
        /// </summary>
        [ObservableProperty] private double _hotWaterTemperature_degC = 55.0;

        /// <summary>
        /// Температура на КВ (студена вода) θ_cold [°C] – за изчисляване на θ_w,set от ΔT
        /// </summary>
        [ObservableProperty] private double _coldWaterTemperature_degC = 10.0;

        /// <summary>
        /// Температура на средата в кондиционирана зона θ_amb,cond [°C]
        /// Default: 20°C
        /// </summary>
        [ObservableProperty] private double _ambientTempConditioned_degC = 20.0;

        /// <summary>
        /// Температура на средата в некондиционирана зона θ_amb,uncond [°C]
        /// Default: 12°C
        /// </summary>
        [ObservableProperty] private double _ambientTempUnconditioned_degC = 12.0;

        // ── Работно време ────────────────────────────────────────────────────

        /// <summary>
        /// Брой работни дни (от Section 16). Ако > 0: t_year = Days * 24, иначе 8760.
        /// </summary>
        [ObservableProperty] private double _workingDaysPerYear = 251.0;

        /// <summary>
        /// Часове работа на ДЕНОНОЩИЕ [h/д] (0 = 24 ч/д).
        /// Приоритет пред OperatingHours_hPerYear.
        /// Пример: помпа работи 16 h/д → t_op = 16 * t_year / 24
        /// </summary>
        [ObservableProperty] private double _operatingHoursPerDay_hPerDay = 0.0;

        /// <summary>
        /// Часове работа на циркулацията t_op [h/год].
        /// Ползва се само ако OperatingHoursPerDay_hPerDay == 0.
        /// 0 означава „използвай t_year" (24/7).
        /// </summary>
        [ObservableProperty] private double _operatingHours_hPerYear = 0.0;

        /// <summary>True ако има принудителна циркулация</summary>
        [ObservableProperty] private bool _hasCirculation = true;

        // ── Тръбни сегменти ──────────────────────────────────────────────────

        /// <summary>Списък от тръбни сегменти</summary>
        public ObservableCollection<PipeSegment> PipeSegments { get; set; } = new();

        // ── Stub отклонения ──────────────────────────────────────────────────

        /// <summary>Stub зони (за формули 1.8 / 1.9)</summary>
        public ObservableCollection<StubZoneData> StubZones { get; set; } = new();

        // ── Физически константи ──────────────────────────────────────────────

        /// <summary>Плътност на водата ρ_w [kg/m³] – default 1000</summary>
        [ObservableProperty] private double _waterDensity_kgm3 = 1000.0;

        /// <summary>
        /// Специфичен топлинен капацитет на водата c_w [kWh/(kg·K)]
        /// = 4.186 kJ/(kgK) / 3600 = 0.001163 kWh/(kg·K)
        /// </summary>
        [ObservableProperty] private double _waterHeatCapacity_kWhkgK = 0.001163;

        // ── Ръчно въвеждане (Режим A) ─────────────────────────────────────

        /// <summary>
        /// Режим A: Ръчно въведена стойност [kWh/год].
        /// При DhwLossMode.Manual тази стойност се използва директно.
        /// </summary>
        [ObservableProperty] private double _manualRecoverableLoss_kWh = 0.0;

        // ── % Дял (Режим C) ──────────────────────────────────────────────────

        /// <summary>
        /// Режим C: Дял [%] от общите загуби.
        /// При DhwLossMode.PercentShare: Q_rbl = Q_total_losses * percent / 100
        /// </summary>
        [ObservableProperty] private double _percentShare = 0.0;

        /// <summary>
        /// Режим C: Общи разпределителни загуби [kWh/год] (въведени ръчно или от изчислението)
        /// </summary>
        [ObservableProperty] private double _totalSystemLosses_kWh = 0.0;

        /// <summary>
        /// Опростено изчисление на θ_w,mean по формула (1.15):
        /// θ_w,mean = 25 · Ψ^(−0.2)
        /// Ако true, ползва тази формула за извънработни загуби вместо θ_w,set
        /// </summary>
        [ObservableProperty] private bool _useSimplifiedMeanTemp = false;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // DhwLossResult – резултат от изчислението
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Резултат от изчислението на регенерируеми загуби
    /// от БГВ разпределителна система.
    /// Всички стойности са годишни [kWh/год].
    /// </summary>
    public sealed class DhwLossResult
    {
        // ── Компоненти по формули ────────────────────────────────────────────

        /// <summary>
        /// Q_w,dis,ls – загуби по тръби по ВРЕМЕ на работа на циркулацията [kWh]
        /// Формула (1.7): Q = (1/1000) · Σ_i Ψ_i · (θ_w,set – θ_amb) · (L+L_equi) · t_op
        /// </summary>
        public double Q_dis_ls { get; init; }

        /// <summary>
        /// Q_w,dis,nom – загуби по тръби ИЗВЪН работа (когато циркулацията спира) [kWh]
        /// Формула (1.10): Q = (1/1000) · Σ_i Ψ_i · (θ_w,avg – θ_amb) · (L+L_equi) · t_nom
        /// </summary>
        public double Q_dis_nom { get; init; }

        /// <summary>
        /// Q_w,dis,stub – загуби от stub отклонения [kWh]
        /// Формула (1.8): Q = ṁ_stub · c_w · (θ_w – θ_amb) · t_op
        /// </summary>
        public double Q_dis_stub { get; init; }

        /// <summary>
        /// Q_w,dis,total – общо загуби [kWh]
        /// Формула (1.16): Q_total = Q_ls + Q_nom + Q_stub
        /// </summary>
        public double Q_total { get; init; }

        // ── Кондиционирана зона ──────────────────────────────────────────────

        /// <summary>Загуби Q_ls само за кондиционирана зона [kWh]</summary>
        public double Q_dis_ls_cond { get; init; }

        /// <summary>Загуби Q_nom само за кондиционирана зона [kWh]</summary>
        public double Q_dis_nom_cond { get; init; }

        /// <summary>Stub загуби само за кондиционирана зона [kWh]</summary>
        public double Q_dis_stub_cond { get; init; }

        /// <summary>Общо загуби в кондиционирана зона [kWh]</summary>
        public double Q_cond { get; init; }

        // ── Резултати ────────────────────────────────────────────────────────

        /// <summary>
        /// f_rbl – дял на кондиционираните загуби [-]
        /// Формула (1.17): f_rbl = Q_cond / Q_total
        /// </summary>
        public double F_rbl { get; init; }

        /// <summary>
        /// Q_w,dis,rbl,year – регенерируеми загуби към зоната [kWh/год]
        /// Формула (1.18): Q_rbl = f_rbl · Q_total
        /// </summary>
        public double Q_rbl_year { get; init; }

        /// <summary>Режимът, при който е изчислен резултатът</summary>
        public DhwLossMode Mode { get; init; }

        /// <summary>Евентуална диагностична информация или предупреждения</summary>
        public string? DiagnosticInfo { get; init; }

        // ── Помощни параметри (за диагностика) ───────────────────────────────

        /// <summary>Изчислени часове t_year [h]</summary>
        public double T_year { get; init; }

        /// <summary>Изчислени часове работа t_op [h]</summary>
        public double T_op { get; init; }

        /// <summary>Изчислени часове извън работа t_nom [h]</summary>
        public double T_nom { get; init; }
    }
}
