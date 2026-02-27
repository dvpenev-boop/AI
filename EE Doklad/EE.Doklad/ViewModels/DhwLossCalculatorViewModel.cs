using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.ViewModels
{
    // ══════════════════════════════════════════════════════════════════════════
    // DhwLossCalculatorViewModel
    // ViewModel за прозореца „Методика (БГВ)" – регенерируеми загуби
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// ViewModel за изчисляване на регенерируеми загуби от БГВ разпределение.
    /// Обвързва се с прозореца DhwLossMethodologyWindow.
    ///
    /// Поддържа три режима (DhwLossMode):
    ///   A) Manual      – потребителят въвежда kWh/год директно
    ///   B) Automatic   – изчисление по методиката с тръбни сегменти
    ///   C) PercentShare – % от общите загуби
    /// </summary>
    public partial class DhwLossCalculatorViewModel : ObservableObject
    {
        private readonly IDhwDistributionLossService _svc;
        private readonly IEquivalentLengthService _leqSvc;

        // Callback – извиква се при потвърждаване на резултата от прозореца
        private readonly Action<double, double>? _onConfirm;

        // ── Конструктор ───────────────────────────────────────────────────────

        public DhwLossCalculatorViewModel(
            IDhwDistributionLossService service,
            double workingDaysPerYear       = 251,
            double hotWaterTemperature_degC = 55,
            Action<double, double>? onConfirm = null,
            IEquivalentLengthService? leqService = null)
        {
            _svc       = service ?? throw new ArgumentNullException(nameof(service));
            _leqSvc    = leqService ?? new EquivalentLengthService();
            _onConfirm = onConfirm;

            Inputs = new DhwLossInputs
            {
                WorkingDaysPerYear        = workingDaysPerYear,
                HotWaterTemperature_degC  = hotWaterTemperature_degC
            };

            // Добавяме два примерни сегмента (conditioned + unconditioned)
            AddDefaultPipeSegments();

            // Подписваме се за промени в колекцията, за да следим нови/премахнати сегменти
            Inputs.PipeSegments.CollectionChanged += PipeSegments_CollectionChanged;
            foreach (var seg in Inputs.PipeSegments)
                SubscribeSegment(seg);
        }

        // ── Входни данни ─────────────────────────────────────────────────────

        /// <summary>Всички входни параметри за изчислението</summary>
        public DhwLossInputs Inputs { get; }

        // ── Режим ──────────────────────────────────────────────────────────

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsManualMode))]
        [NotifyPropertyChangedFor(nameof(IsAutomaticMode))]
        [NotifyPropertyChangedFor(nameof(IsPercentMode))]
        [NotifyPropertyChangedFor(nameof(PercentFieldEnabled))]
        private DhwLossMode _selectedMode = DhwLossMode.Automatic;

        public bool IsManualMode
        {
            get => SelectedMode == DhwLossMode.Manual;
            set { if (value) SelectedMode = DhwLossMode.Manual; }
        }
        public bool IsAutomaticMode
        {
            get => SelectedMode == DhwLossMode.Automatic;
            set { if (value) SelectedMode = DhwLossMode.Automatic; }
        }
        public bool IsPercentMode
        {
            get => SelectedMode == DhwLossMode.PercentShare;
            set { if (value) SelectedMode = DhwLossMode.PercentShare; }
        }

        /// <summary>% полето е активно само при PercentShare режим</summary>
        public bool PercentFieldEnabled => SelectedMode == DhwLossMode.PercentShare;

        // ── Резултати ─────────────────────────────────────────────────────────

        [ObservableProperty] private DhwLossResult? _lastResult;

        [ObservableProperty] private string _resultSummary = "Натиснете \u201eИзчисли\u201c";

        [ObservableProperty] private string _diagnosticText = string.Empty;

        // Показвани резултати (binding-friendly)
        [ObservableProperty] private double _resultQrblYear = 0.0;
        [ObservableProperty] private double _resultFrblPct = 0.0;
        [ObservableProperty] private double _resultQTotal = 0.0;
        [ObservableProperty] private double _resultQCond = 0.0;
        [ObservableProperty] private double _resultQls = 0.0;
        [ObservableProperty] private double _resultQnom = 0.0;
        [ObservableProperty] private double _resultQstub = 0.0;
        [ObservableProperty] private double _resultTYear = 0.0;
        [ObservableProperty] private double _resultTOp = 0.0;
        [ObservableProperty] private double _resultTNom = 0.0;
        /// <summary>Изчислено t_op в h/ден = T_op / t_year * 24 – за показване в UI</summary>
        [ObservableProperty] private double _resultTOpPerDay = 0.0;

        // ── Тръбни сегменти (за UI таблица) ──────────────────────────────────

        /// <summary>Прокси към Inputs.PipeSegments за binding в DataGrid</summary>
        public ObservableCollection<PipeSegment> PipeSegments => Inputs.PipeSegments;

        /// <summary>Прокси към Inputs.StubZones за binding</summary>
        public ObservableCollection<StubZoneData> StubZones => Inputs.StubZones;

        /// <summary>Списък от стойности на PipeZoneType за ComboBox в DataGrid</summary>
        public static IReadOnlyList<PipeZoneType> ZoneTypes { get; } =
            (PipeZoneType[])Enum.GetValues(typeof(PipeZoneType));

        /// <summary>Списък от стойности на PipeInsulationType за ComboBox в DataGrid</summary>
        public static IReadOnlyList<PipeInsulationType> InsulationTypes { get; } =
            (PipeInsulationType[])Enum.GetValues(typeof(PipeInsulationType));

        [ObservableProperty] private PipeSegment? _selectedSegment;

        // ── Команди ──────────────────────────────────────────────────────────

        [RelayCommand]
        private void Calculate()
        {
            try
            {
                var result = _svc.Calculate(Inputs, SelectedMode);
                LastResult = result;

                ResultQrblYear = result.Q_rbl_year;
                ResultFrblPct  = Math.Round(result.F_rbl * 100.0, 2);
                ResultQTotal   = result.Q_total;
                ResultQCond    = result.Q_cond;
                ResultQls      = result.Q_dis_ls;
                ResultQnom     = result.Q_dis_nom;
                ResultQstub    = result.Q_dis_stub;
                ResultTYear    = result.T_year;
                ResultTOp      = result.T_op;
                ResultTNom     = result.T_nom;
                ResultTOpPerDay = result.T_year > 0
                    ? Math.Round(result.T_op / result.T_year * 24.0, 2)
                    : 0.0;

                // Принудително изчисли ComputedPsi_WmK за всеки сегмент и известяваме DataGrid.
                // CalculateAutomatic вече го прави за Automatic режим, но не за останалите.
                // Правим го тук универсално за всички режими.
                foreach (var seg in Inputs.PipeSegments)
                    seg.ComputedPsi_WmK = _svc.ComputePsi(seg);

                ResultSummary  = $"✓ Q_rbl = {result.Q_rbl_year:0.00} kWh/год  |  f_rbl = {ResultFrblPct:0.##}%";
                DiagnosticText = result.DiagnosticInfo ?? string.Empty;
            }
            catch (Exception ex)
            {
                ResultSummary  = $"✗ Грешка: {ex.Message}";
                DiagnosticText = ex.ToString();
            }
        }

        [RelayCommand]
        private void Confirm()
        {
            // Изчисли преди потвърждаване ако няма резултат
            if (LastResult is null) Calculate();
            if (LastResult is null) return;

            _onConfirm?.Invoke(LastResult.Q_rbl_year, ResultFrblPct);
        }

        [RelayCommand]
        private void AddSegment()
        {
            PipeSegments.Add(new PipeSegment
            {
                Name          = $"Сегмент {PipeSegments.Count + 1}",
                ZoneType      = PipeZoneType.Conditioned,
                Length_m      = 5.0,
                InsulationType = PipeInsulationType.DirectPsi,
                Psi_WmK       = 0.5
            });
        }

        [RelayCommand]
        private void RemoveSegment()
        {
            if (SelectedSegment is not null && PipeSegments.Contains(SelectedSegment))
                PipeSegments.Remove(SelectedSegment);
        }

        [RelayCommand]
        private void AddStubZone()
        {
            StubZones.Add(new StubZoneData
            {
                ZoneType             = PipeZoneType.Conditioned,
                StubVolume_m3        = 0.001,
                TappingFrequency_perHour = 0.5
            });
        }

        [RelayCommand]
        private void RemoveStubZone()
        {
            // Премахваме последно добавения (или може да ползваме SelectedStubZone)
            if (StubZones.Count > 0) StubZones.RemoveAt(StubZones.Count - 1);
        }

        // ── Вградена помощна: примерни сегменти ─────────────────────────────

        private void AddDefaultPipeSegments()
        {
            // Пример с 2 сегмента: 1 кондициониран + 1 некондициониран
            // Демонстрация: сегмент 1 – кондиционирана зона, директно Ψ = 0.5 W/(m·K), L=10 m
            PipeSegments.Add(new PipeSegment
            {
                Name          = "Подаващ (отопляема зона)",
                ZoneType      = PipeZoneType.Conditioned,
                Length_m      = 10.0,
                EquivalentLength_m = 2.0,
                InsulationType = PipeInsulationType.DirectPsi,
                Psi_WmK       = 0.50
            });

            // Сегмент 2 – некондиционирана зона (мазе/таван), изолирана тръба
            PipeSegments.Add(new PipeSegment
            {
                Name          = "Захранващ (неотопляема зона)",
                ZoneType      = PipeZoneType.Unconditioned,
                Length_m      = 8.0,
                EquivalentLength_m = 1.5,
                InsulationType     = PipeInsulationType.InsulatedInAir,
                InnerDiameter_m               = 0.020,
                OuterDiameterWithInsulation_m  = 0.060,
                InsulationLambda_WmK           = 0.04,
                SurfaceHeatTransfer_WmK        = 8.0
            });
        }

        // ── L_equi auto-recalc ──────────────────────────────────────────────

        /// <summary>Свойства на PipeSegment, чиято промяна трябва да преизчисли L_equi.</summary>
        private static readonly HashSet<string> _lequiTriggerProps = new(StringComparer.Ordinal)
        {
            nameof(PipeSegment.OuterDiameterWithInsulation_m),
            nameof(PipeSegment.Elbow90Count),
            nameof(PipeSegment.TeeBranchCount),
            nameof(PipeSegment.BallValveCount),
            nameof(PipeSegment.AutoLequi),
        };

        private void PipeSegments_CollectionChanged(object? sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (PipeSegment seg in e.OldItems)
                    seg.PropertyChanged -= Segment_PropertyChanged;

            if (e.NewItems != null)
                foreach (PipeSegment seg in e.NewItems)
                    SubscribeSegment(seg);
        }

        private void SubscribeSegment(PipeSegment seg)
        {
            seg.PropertyChanged -= Segment_PropertyChanged; // prevent double-subscribe
            seg.PropertyChanged += Segment_PropertyChanged;
            // Initial recalc
            RecalcLequi(seg);
        }

        private void Segment_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is PipeSegment seg && e.PropertyName != null && _lequiTriggerProps.Contains(e.PropertyName))
                RecalcLequi(seg);
        }

        /// <summary>
        /// Преизчислява EquivalentLength_m за даден сегмент ако AutoLequi == true.
        /// Попълва SelectedDnDisplay и DnWarning.
        /// </summary>
        private void RecalcLequi(PipeSegment seg)
        {
            var detail = _leqSvc.CalcLequiDetailed(
                seg.OuterDiameterWithInsulation_m,
                seg.Elbow90Count,
                seg.TeeBranchCount,
                seg.BallValveCount);

            seg.SelectedDnDisplay = detail.Dn.ToString();
            seg.DnWarning = detail.IsOutOfRange
                ? $"⚠ da отклонение {detail.DaDeviation * 1000:0.0} mm от {detail.Dn}"
                : "";

            if (seg.AutoLequi)
                seg.EquivalentLength_m = detail.Lequi;
        }

        // ── Текст на формулите (вграден ресурс) ─────────────────────────────

        /// <summary>
        /// Пълен текст на методиката – формули, входни данни, алгоритъм.
        /// Вграден директно в кода (без외 файлове).
        /// </summary>
        public static string MethodologyText => """
            ══════════════════════════════════════════════════════════════════
             МЕТОДИКА: Регенерируеми загуби от БГВ разпределение
             (по EN 15316-3 / ISO 52003)
            ══════════════════════════════════════════════════════════════════

            1. ОБОЗНАЧЕНИЯ
            ─────────────
            Ψ      [W/(m·K)]  Линейно топлопреминаване (тръба + изолация)
            L      [m]        Реална дължина на тръбата
            L_equi [m]        Еквивалентна дължина (вентили, окачвания)
            θ_w    [°C]       Температура на горещата вода
            θ_amb  [°C]       Температура на средата в зоната
            t_op   [h]        Часове работа на циркулацията
            t_nom  [h]        Часове извън работа
            t_year [h]        Общо часове за годината
            c_w    [kWh/(kg·K)] = 0.001163 (специфичен топлинен капацитет)
            ρ_w    [kg/m³]    = 1000 (плътност на водата)

            2. ФОРМУЛИ ЗА Ψ
            ───────────────
            (1.3) Изолирана тръба във въздух:
                  Ψ = π / [ (1/(2·λ_D))·ln(d_a/d_i) + 1/(h_a·d_a) ]

            (1.4) Вградена тръба в материал:
                  Ψ = π / [ 0.5·( (1/λ_D)·ln(d_a/d_i) + (1/λ_em)·ln(4z/d_a) ) ]

            (1.5) Неизолирана тръба:
                  Ψ = π / [ (1/(2·λ_p))·ln(d_p,a/d_p,i) + 1/(h_a·d_p,a) ]

            (1.6) Приближение (неизолирана):
                  Ψ ≈ h_a · π · d_p,a

            3. ЗАГУБИ ПО ТРЪБИ
            ──────────────────
            (1.7) По ВРЕМЕ на работа:
                  Q_ls = (1/1000) · Σ_i [ Ψ_i · (θ_w – θ_amb) · (L_i + L_equi,i) · t_op ]

            (1.10) ИЗВЪН работа (t_nom = t_year − t_op):
                  Q_nom = (1/1000) · Σ_i [ Ψ_i · (θ_w,avg – θ_amb) · (L_i + L_equi,i) · t_nom ]

            4. STUB ЗАГУБИ (отклонения с отворена циркулация)
            ──────────────────────────────────────────────────
            (1.9)  ṁ_stub = V_stub · ρ_w · n_tap       [kg/h]
            (1.8)  Q_stub = ṁ_stub · c_w · (θ_w – θ_amb) · t_op  [kWh]

            5. СУМИРАНЕ И ДЯЛОВЕ
            ─────────────────────
            (1.16) Q_total = Q_ls + Q_nom + Q_stub

            (1.17) f_rbl = Q_cond / Q_total
                   (Q_cond = загуби само в кондиционирана зона)

            (1.18) Q_rbl_year = f_rbl · Q_total    [kWh/год] → подава се в Секция 23

            6. ОПРОСТЕНА СРЕДНА ТЕМПЕРАТУРА (по избор)
            ───────────────────────────────────────────
            (1.15) θ_w,mean = 25 · Ψ^(−0.2)    [°C]
                   (ползва се за извънработните загуби вместо θ_w,set)

            7. АЛГОРИТЪМ – СТЪПКИ
            ──────────────────────
            Стъпка 0: Определи режим
              · Режим A (Ръчно): директна стойност, без изчисление
              · Режим C (% дял): Q_rbl = Q_total_losses × % / 100
              · Режим B (Автоматично): следвай стъпки 1–10

            Стъпка 1: t_year = Days×24 (или 8760)
                      t_op   = clamp(input, 0..t_year)
                      t_nom  = t_year − t_op

            Стъпка 2: Изчисли Ψ_i за всеки сегмент (формули 1.3–1.6)

            Стъпка 3: θ_amb = 20°C (кондиционирана) / 12°C (некондиционирана)
                      θ_w,avg = θ_w,set (или формула 1.15)

            Стъпка 4: Q_ls_i = (1/1000) · Ψ · ΔΘ · (L+L_equi) · t_op

            Стъпка 5: Q_nom_i = (1/1000) · Ψ · ΔΘ_avg · (L+L_equi) · t_nom

            Стъпка 6: Q_stub_j = ṁ_stub · c_w · ΔΘ · t_op

            Стъпка 7: Q_total = ΣQ_ls + ΣQ_nom + ΣQ_stub

            Стъпка 8: Q_cond = сумирай само кондиционираните сегменти

            Стъпка 9: f_rbl = Q_cond / Q_total

            Стъпка 10: Q_rbl_year = f_rbl × Q_total → Секция 23

            8. ПРИМЕРНИ СТОЙНОСТИ
            ──────────────────────
            · Ψ за добре изолирана тръба: ~ 0.1–0.3 W/(m·K)
            · Ψ за слабо изолирана тръба: ~ 0.4–0.8 W/(m·K)
            · θ_amb_cond = 20°C (отопляема зона)
            · θ_amb_uncond = 12°C (мазе, таван)
            · θ_w,set = 55°C (типична температура на БГВ)
            · t_year = 251 дни × 24 = 6024 h (работен сезон)
                    или 8760 h (денонощна)
            ══════════════════════════════════════════════════════════════════
            """;
    }
}
