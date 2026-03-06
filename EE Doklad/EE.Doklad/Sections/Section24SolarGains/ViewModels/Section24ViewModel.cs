using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using EE.Doklad.Models;
using EE.Doklad.Sections.Section24SolarGains.Calculator;
using EE.Doklad.Sections.Section24SolarGains.Models;
using EE.Doklad.Sections.Section24SolarGains.Results;
using EE.Doklad.Sections.Section24SolarGains.Services;
using EE.Doklad.Sections.Section24SolarGains.Validation;

namespace EE.Doklad.Sections.Section24SolarGains.ViewModels
{
    // ======================================================================
    //  ROW VIEW-MODELS (за DataGrid-ове)
    // ======================================================================

    /// <summary>Ред с месечни данни за прозорец – показва се в Expander (Таб 2).</summary>
    public class WindowMonthlyRow
    {
        public string Month          { get; set; } = string.Empty;
        public double A_gl           { get; set; }
        public double G_gl           { get; set; }
        public double F_sh_obst      { get; set; }
        public double H_sol          { get; set; }
        public double SolarFactor    { get; set; }
        public double H_lr           { get; set; }
        public double Q_sky          { get; set; }
        public double Q_sol_window   { get; set; }
    }

    /// <summary>Ред с месечни данни за непрозрачен елемент – показва се в Expander (Таб 2).</summary>
    public class OpaqueMonthlyRow
    {
        public string Month             { get; set; } = string.Empty;
        public double Alpha_sol         { get; set; }
        public double R_se              { get; set; }
        public double U_c               { get; set; }
        public double A_c               { get; set; }
        public double F_sh_obst         { get; set; }
        public double H_sol             { get; set; }
        public double SolarFactorOpaque { get; set; }
        public double H_lr              { get; set; }
        public double Q_sky             { get; set; }
        public double Q_sol_opaque      { get; set; }
    }

    /// <summary>Група от редове за един прозорец – използва се в Expander.</summary>
    public class WindowExpanderGroup
    {
        public string Header { get; set; } = string.Empty;
        public ObservableCollection<WindowMonthlyRow> Rows { get; } = [];
    }

    /// <summary>Група от редове за един непрозрачен елемент – използва се в Expander.</summary>
    public class OpaqueExpanderGroup
    {
        public string Header { get; set; } = string.Empty;
        public ObservableCollection<OpaqueMonthlyRow> Rows { get; } = [];
    }

    /// <summary>Ред за общата таблица по месеци (Таб 3).</summary>
    public class TotalMonthlyRow
    {
        public string Month            { get; set; } = string.Empty;
        public double SumWindows       { get; set; }
        public double SumOpaque        { get; set; }
        public double SumQsky          { get; set; }
        public double Q_sol_total      { get; set; }
        public double Q_sol_heating    { get; set; }
        public double Q_sol_cooling    { get; set; }
    }

    // ======================================================================
    //  ГЛАВЕН VIEW-MODEL
    // ======================================================================

    /// <summary>
    /// ViewModel за Секция 24 – топлинни печалби от слънчево греене.
    /// <para>Архитектура: MVVM, INotifyPropertyChanged.</para>
    /// </summary>
    public class Section24ViewModel : INotifyPropertyChanged
    {
        private readonly Section24SolarGainsData _data;
        private readonly Report? _report;
        private readonly Section24SyncService _syncService = new();

        private bool _hasErrors;
        private bool _hasWarnings;
        private string _validationSummary = string.Empty;
        private Section24Results? _results;
        private bool _isCalculated;
        private bool _autoSyncEnabled;
        private bool _autoSyncWired;

        // ------------------------------------------------------------------ //

        public Section24ViewModel(Section24SolarGainsData data, Report? report = null, bool autoSyncEnabled = false)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _report = report;
            _autoSyncEnabled = autoSyncEnabled;

            CalculateCommand = new RelayCommand(ExecuteCalculate);
            SyncFromSectionsCommand = new RelayCommand(ExecuteSyncFromSections, () => _report != null);
            AddWindowCommand  = new RelayCommand(ExecuteAddWindow);
            RemoveWindowCommand = new RelayCommand<WindowElement>(ExecuteRemoveWindow);
            AddOpaqueCommand  = new RelayCommand(ExecuteAddOpaque);
            RemoveOpaqueCommand = new RelayCommand<OpaqueElement>(ExecuteRemoveOpaque);

            // Subscribe to collection changes
            _data.Windows.CollectionChanged       += (_, _) => { IsCalculated = false; };
            _data.OpaqueElements.CollectionChanged += (_, _) => { IsCalculated = false; };

            if (_report != null)
            {
                _syncService.SyncFromReport(_report, _data);
                ValidationSummary = "Synced from sections 6/7/9.";
            }

            if (_autoSyncEnabled)
                EnsureAutoSyncWired();
        }

        // ------------------------------------------------------------------ //
        //  BINDABLE DATA
        // ------------------------------------------------------------------ //

        public Section24SolarGainsData Data => _data;

        /// <summary>Прозорци – директно за DataGrid в Таб 1.</summary>
        public ObservableCollection<WindowElement> Windows => _data.Windows;

        /// <summary>Непрозрачни елементи – директно за DataGrid в Таб 1.</summary>
        public ObservableCollection<OpaqueElement> OpaqueElements => _data.OpaqueElements;

        /// <summary>Общи месечни данни – 12 реда за DataGrid в Таб 1.</summary>
        public MonthlyGeneralData[] MonthlyData => _data.MonthlyData;

        // ------------------------------------------------------------------ //
        //  VALIDATION STATE
        // ------------------------------------------------------------------ //

        public bool HasErrors
        {
            get => _hasErrors;
            private set { _hasErrors = value; OnPropertyChanged(); }
        }

        public bool HasWarnings
        {
            get => _hasWarnings;
            private set { _hasWarnings = value; OnPropertyChanged(); }
        }

        public string ValidationSummary
        {
            get => _validationSummary;
            private set { _validationSummary = value; OnPropertyChanged(); }
        }

        public bool IsCalculated
        {
            get => _isCalculated;
            private set { _isCalculated = value; OnPropertyChanged(); }
        }

        public bool AutoSyncEnabled
        {
            get => _autoSyncEnabled;
            set
            {
                if (_autoSyncEnabled == value) return;
                _autoSyncEnabled = value;
                if (_autoSyncEnabled)
                    EnsureAutoSyncWired();
                OnPropertyChanged();
            }
        }

        // ------------------------------------------------------------------ //
        //  RESULT PROPERTIES (за Таб 3)
        // ------------------------------------------------------------------ //

        private ObservableCollection<TotalMonthlyRow> _totalRows = [];
        public ObservableCollection<TotalMonthlyRow> TotalRows
        {
            get => _totalRows;
            private set { _totalRows = value; OnPropertyChanged(); }
        }

        private ObservableCollection<WindowExpanderGroup> _windowGroups = [];
        public ObservableCollection<WindowExpanderGroup> WindowGroups
        {
            get => _windowGroups;
            private set { _windowGroups = value; OnPropertyChanged(); }
        }

        private ObservableCollection<OpaqueExpanderGroup> _opaqueGroups = [];
        public ObservableCollection<OpaqueExpanderGroup> OpaqueGroups
        {
            get => _opaqueGroups;
            private set { _opaqueGroups = value; OnPropertyChanged(); }
        }

        // Годишни суми
        private double _annualQ_sol_total;
        public double AnnualQ_sol_total
        {
            get => _annualQ_sol_total;
            private set { _annualQ_sol_total = value; OnPropertyChanged(); }
        }

        private double _annualQ_sol_heating;
        public double AnnualQ_sol_heating
        {
            get => _annualQ_sol_heating;
            private set { _annualQ_sol_heating = value; OnPropertyChanged(); }
        }

        private double _annualQ_sol_cooling;
        public double AnnualQ_sol_cooling
        {
            get => _annualQ_sol_cooling;
            private set { _annualQ_sol_cooling = value; OnPropertyChanged(); }
        }

        private double _annualQ_sol_windows;
        public double AnnualQ_sol_windows
        {
            get => _annualQ_sol_windows;
            private set { _annualQ_sol_windows = value; OnPropertyChanged(); }
        }

        private double _annualQ_sol_opaque;
        public double AnnualQ_sol_opaque
        {
            get => _annualQ_sol_opaque;
            private set { _annualQ_sol_opaque = value; OnPropertyChanged(); }
        }

        private double _annualQ_sky;
        public double AnnualQ_sky
        {
            get => _annualQ_sky;
            private set { _annualQ_sky = value; OnPropertyChanged(); }
        }

        // ------------------------------------------------------------------ //
        //  COMMANDS
        // ------------------------------------------------------------------ //

        public RelayCommand CalculateCommand { get; }
        public RelayCommand SyncFromSectionsCommand { get; }
        public RelayCommand AddWindowCommand { get; }
        public RelayCommand<WindowElement> RemoveWindowCommand { get; }
        public RelayCommand AddOpaqueCommand { get; }
        public RelayCommand<OpaqueElement> RemoveOpaqueCommand { get; }

        // ------------------------------------------------------------------ //
        //  EXECUTE
        // ------------------------------------------------------------------ //

        private void ExecuteCalculate()
        {
            // Always refresh Section 24 input from sections 6/7/9 (and climate zone from section 5)
            // before calculation, so intermediate results cannot use stale climate/orientation data.
            if (_report != null)
                _syncService.SyncFromReport(_report, _data);

            // Validate
            var validation = Section24Validator.ValidateAll(_data);
            HasErrors   = !validation.IsValid;
            HasWarnings = validation.Warnings.Count > 0;

            var sb = new System.Text.StringBuilder();
            foreach (var e in validation.Errors)   sb.AppendLine("❌ " + e);
            foreach (var w in validation.Warnings) sb.AppendLine("⚠️ " + w);
            ValidationSummary = sb.ToString().TrimEnd();

            if (HasErrors) return;

            // Calculate
            _results = Section24Calculator.Calculate(_data);

            // Populate Taб 2 – Expander groups for windows
            var winGroups = new ObservableCollection<WindowExpanderGroup>();
            foreach (var wr in _results.WindowResults)
            {
                var grp = new WindowExpanderGroup { Header = $"Прозорец: {wr.ElementId}" };
                foreach (var mr in wr.MonthlyResults)
                {
                    grp.Rows.Add(new WindowMonthlyRow
                    {
                        Month        = mr.MonthName,
                        A_gl         = mr.A_gl,
                        G_gl         = mr.G_gl,
                        F_sh_obst    = mr.F_sh_obst,
                        H_sol        = mr.H_sol,
                        SolarFactor  = mr.SolarFactor,
                        H_lr         = mr.H_lr,
                        Q_sky        = mr.Q_sky,
                        Q_sol_window = mr.Q_sol_window
                    });
                }
                winGroups.Add(grp);
            }
            WindowGroups = winGroups;

            // Populate Таб 2 – Expander groups for opaque
            var opaqueGroups = new ObservableCollection<OpaqueExpanderGroup>();
            foreach (var or_ in _results.OpaqueResults)
            {
                var grp = new OpaqueExpanderGroup { Header = $"Непрозрачен: {or_.ElementId}" };
                foreach (var mr in or_.MonthlyResults)
                {
                    grp.Rows.Add(new OpaqueMonthlyRow
                    {
                        Month             = mr.MonthName,
                        Alpha_sol         = mr.Alpha_sol,
                        R_se              = mr.R_se,
                        U_c               = mr.U_c,
                        A_c               = mr.A_c,
                        F_sh_obst         = mr.F_sh_obst,
                        H_sol             = mr.H_sol,
                        SolarFactorOpaque = mr.SolarFactorOpaque,
                        H_lr              = mr.H_lr,
                        Q_sky             = mr.Q_sky,
                        Q_sol_opaque      = mr.Q_sol_opaque
                    });
                }
                opaqueGroups.Add(grp);
            }
            OpaqueGroups = opaqueGroups;

            // Populate Таб 3
            var totalRows = new ObservableCollection<TotalMonthlyRow>();
            foreach (var mt in _results.MonthlyTotals)
            {
                totalRows.Add(new TotalMonthlyRow
                {
                    Month       = mt.MonthName,
                    SumWindows  = mt.SumQ_sol_windows,
                    SumOpaque   = mt.SumQ_sol_opaque,
                    SumQsky     = mt.SumQ_sky,
                    Q_sol_total = mt.Q_sol_total,
                    Q_sol_heating = mt.Q_sol_heating,
                    Q_sol_cooling = mt.Q_sol_cooling
                });
            }
            TotalRows = totalRows;

            AnnualQ_sol_total   = _results.AnnualQ_sol_total;
            AnnualQ_sol_heating = _results.AnnualQ_sol_heating;
            AnnualQ_sol_cooling = _results.AnnualQ_sol_cooling;
            AnnualQ_sol_windows = _results.AnnualQ_sol_windows;
            AnnualQ_sol_opaque  = _results.AnnualQ_sol_opaque;
            AnnualQ_sky         = _results.AnnualQ_sky;

            IsCalculated = true;
        }

        private void ExecuteSyncFromSections()
        {
            if (_report == null)
                return;

            _syncService.SyncFromReport(_report, _data);
            IsCalculated = false;
            HasErrors = false;
            HasWarnings = false;
            ValidationSummary = "Синхронизацията от секции 6, 7 и 9 е изпълнена.";
        }

        private void ExecuteAddWindow()
        {
            int n = _data.Windows.Count + 1;
            _data.Windows.Add(new WindowElement
            {
                Id      = $"W{n}",
                A_wi    = 1.5,
                F_fr    = 0.20,
                U_c     = 1.3,
                R_se    = 0.13,
                F_sky   = 0.5,
                Epsilon = 0.9,
                ThetaSs = 10.0,
                H_sol    = new double[12],
                F_sh_obst = Enumerable.Repeat(1.0, 12).ToArray(),
                G_gl      = Enumerable.Repeat(0.67, 12).ToArray()
            });
        }

        private void ExecuteRemoveWindow(WindowElement? win)
        {
            if (win != null && _data.Windows.Contains(win))
                _data.Windows.Remove(win);
        }

        private void ExecuteAddOpaque()
        {
            int n = _data.OpaqueElements.Count + 1;
            _data.OpaqueElements.Add(new OpaqueElement
            {
                Id       = $"OP{n}",
                A_c      = 10.0,
                Alpha_sol = 0.6,
                U_c      = 0.3,
                R_se     = 0.13,
                F_sky    = 0.5,
                Epsilon  = 0.9,
                ThetaSs  = 10.0,
                H_sol    = new double[12],
                F_sh_obst = Enumerable.Repeat(1.0, 12).ToArray()
            });
        }

        private void ExecuteRemoveOpaque(OpaqueElement? op)
        {
            if (op != null && _data.OpaqueElements.Contains(op))
                _data.OpaqueElements.Remove(op);
        }

        private void EnsureAutoSyncWired()
        {
            if (_autoSyncWired)
                return;

            if (_report?.Sections == null)
                return;

            _autoSyncWired = true;

            var objectData = _report.Sections.FirstOrDefault(s => s.Type == SectionType.ObjectData)?.ObjectDataSectionData;
            if (objectData != null)
            {
                objectData.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(ObjectDataSectionData.ClimateZone))
                        OnSourceDataChanged();
                };
            }

            var windowsData = _report.Sections.FirstOrDefault(s => s.Type == SectionType.Windows)?.WindowsSectionData;
            if (windowsData != null)
            {
                foreach (var b in windowsData.WindowBatches)
                    b.PropertyChanged += SourceItem_PropertyChanged;

                windowsData.WindowBatches.CollectionChanged += (s, e) =>
                {
                    if (e.OldItems != null)
                    {
                        foreach (WindowBatch b in e.OldItems)
                            b.PropertyChanged -= SourceItem_PropertyChanged;
                    }
                    if (e.NewItems != null)
                    {
                        foreach (WindowBatch b in e.NewItems)
                            b.PropertyChanged += SourceItem_PropertyChanged;
                    }
                    OnSourceDataChanged();
                };
            }

            var wallsData = _report.Sections.FirstOrDefault(s => s.Type == SectionType.ExternalWalls)?.ExternalWallsSectionData;
            if (wallsData != null)
            {
                foreach (var w in wallsData.WallTypes)
                    w.PropertyChanged += SourceItem_PropertyChanged;

                wallsData.WallTypes.CollectionChanged += (s, e) =>
                {
                    if (e.OldItems != null)
                    {
                        foreach (ExternalWallType w in e.OldItems)
                            w.PropertyChanged -= SourceItem_PropertyChanged;
                    }
                    if (e.NewItems != null)
                    {
                        foreach (ExternalWallType w in e.NewItems)
                            w.PropertyChanged += SourceItem_PropertyChanged;
                    }
                    OnSourceDataChanged();
                };
            }

            var roofData = _report.Sections.FirstOrDefault(s => s.Type == SectionType.Roof)?.RoofSectionData;
            if (roofData != null)
            {
                foreach (var r in roofData.RoofTypes)
                    r.PropertyChanged += SourceItem_PropertyChanged;

                roofData.RoofTypes.CollectionChanged += (s, e) =>
                {
                    if (e.OldItems != null)
                    {
                        foreach (RoofType r in e.OldItems)
                            r.PropertyChanged -= SourceItem_PropertyChanged;
                    }
                    if (e.NewItems != null)
                    {
                        foreach (RoofType r in e.NewItems)
                            r.PropertyChanged += SourceItem_PropertyChanged;
                    }
                    OnSourceDataChanged();
                };
            }
        }

        private void SourceItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
            => OnSourceDataChanged();

        private void OnSourceDataChanged()
        {
            IsCalculated = false;
            if (AutoSyncEnabled)
                ExecuteSyncFromSections();
        }

        // ------------------------------------------------------------------ //

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // ======================================================================
    //  RELAY COMMAND (generic + non-generic)
    // ======================================================================

    /// <summary>
    /// Прост ICommand, подходящ за MVVM без CommunityToolkit.
    /// </summary>
    public class RelayCommand : System.Windows.Input.ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute    = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add    => System.Windows.Input.CommandManager.RequerySuggested += value;
            remove => System.Windows.Input.CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object? parameter)     => _execute();
    }

    /// <summary>
    /// Prekladen ICommand с параметър T.
    /// </summary>
    public class RelayCommand<T> : System.Windows.Input.ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Func<T?, bool>? _canExecute;

        public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        {
            _execute    = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add    => System.Windows.Input.CommandManager.RequerySuggested += value;
            remove => System.Windows.Input.CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke((T?)parameter) ?? true;
        public void Execute(object? parameter)     => _execute((T?)parameter);
    }
}
