using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.ViewModels
{
    public partial class WindowsSectionViewModel : ObservableObject
    {
        private readonly WindowsSectionData _data;
        private readonly Report? _report;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private ObservableCollection<WindowSummaryRow> summaryRows = new();

        [ObservableProperty]
        private WindowSummaryRow? selectedSummaryRow;

        private ObjectDataSectionData? GetObjectData()
        {
            if (_report?.Sections == null) return null;
            var section = _report.Sections.FirstOrDefault(s => s.Type == SectionType.ObjectData)
                          ?? _report.Sections.FirstOrDefault(s => s.Type == SectionType.Normal
                                                                 && !string.IsNullOrEmpty(s.Title)
                                                                 && s.Title.Contains("Данни за обекта"));
            return section?.ObjectDataSectionData;
        }

        /// <summary>
        /// Отоплителният сезон е активен (от Секция 5)
        /// </summary>
        public bool HeatingEnabled => GetObjectData()?.HeatingSeasonEnabled ?? true;

        /// <summary>
        /// Охладителният сезон е активен (от Секция 5)
        /// </summary>
        public bool CoolingEnabled => GetObjectData()?.CoolingSeasonEnabled ?? true;

        /// <summary>
        /// Климатична зона (от Секция 5)
        /// </summary>
        public int? ClimateZone => GetObjectData()?.ClimateZone;

        /// <summary>
        /// Начален месец на охладителния сезон (1-12, от Секция 5)
        /// </summary>
        public int? CoolingStartMonth => GetObjectData()?.CoolingSeasonStartMonth;

        /// <summary>
        /// Краен месец на охладителния сезон (1-12, от Секция 5)
        /// </summary>
        public int? CoolingEndMonth => GetObjectData()?.CoolingSeasonEndMonth;

        public WindowsSectionViewModel(WindowsSectionData data, Report? report = null)
        {
            _data = data;
            _report = report;
            Description = data.Description;

            // Слушаме за промени в партидите
            _data.WindowBatches.CollectionChanged += (s, e) => RefreshSummary();

            // Слушаме за промени в ObjectDataSectionData (сезони)
            var objData = GetObjectData();
            if (objData != null)
            {
                objData.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName is nameof(ObjectDataSectionData.HeatingSeasonEnabled)
                                       or nameof(ObjectDataSectionData.CoolingSeasonEnabled)
                                       or nameof(ObjectDataSectionData.ClimateZone)
                                       or nameof(ObjectDataSectionData.CoolingSeasonStartMonth)
                                       or nameof(ObjectDataSectionData.CoolingSeasonEndMonth))
                    {
                        OnPropertyChanged(nameof(HeatingEnabled));
                        OnPropertyChanged(nameof(CoolingEnabled));
                        OnPropertyChanged(nameof(ClimateZone));
                        OnPropertyChanged(nameof(CoolingStartMonth));
                        OnPropertyChanged(nameof(CoolingEndMonth));
                    }
                };
            }

            RefreshSummary();
        }

        // Expose batches for matrix view binding
        public ObservableCollection<WindowBatch> WindowBatches => _data.WindowBatches;

        /// <summary>
        /// Презарежда обобщената таблица от партидите
        /// </summary>
        private void RefreshSummary()
        {
            var groups = WindowCalculator.GroupBatches(_data.WindowBatches);
            SummaryRows.Clear();
            foreach (var group in groups)
            {
                SummaryRows.Add(group);
            }
        }

        [RelayCommand]
        private void AddWindow()
        {
            // Опитваме се да намерим ObjectDataSectionData от Report
            var objectData = GetObjectData();
            
            int? climateZone = objectData?.ClimateZone;
            bool heatingEnabled = objectData?.HeatingSeasonEnabled ?? true;
            bool coolingEnabled = objectData?.CoolingSeasonEnabled ?? true;
            int? coolingStartMonth = objectData?.CoolingSeasonStartMonth;
            int? coolingEndMonth   = objectData?.CoolingSeasonEndMonth;

            var dialog = new Views.AddWindowFullDialog(
                existingBatch: null,
                climateZone: climateZone,
                heatingEnabled: heatingEnabled,
                coolingEnabled: coolingEnabled,
                coolingStartMonth: coolingStartMonth,
                coolingEndMonth: coolingEndMonth
            );
            
            if (dialog.ShowDialog() == true)
            {
                _data.WindowBatches.Add(dialog.Result);
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        private void EditSelected()
        {
            if (SelectedSummaryRow == null) return;

            // TODO: Отваряме Details dialog за групата
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
        private void DeleteSelected()
        {
            if (SelectedSummaryRow == null) return;

            // Изтриваме всички партиди от групата
            foreach (var batch in SelectedSummaryRow.Batches.ToList())
            {
                _data.WindowBatches.Remove(batch);
            }
        }

        [RelayCommand]
        private void OpenDetails()
        {
            if (SelectedSummaryRow == null) return;

            // Взимаме ObjectDataSectionData за да предадем климатичната зона
            var objectData = GetObjectData();
            
            int? climateZone = objectData?.ClimateZone;
            bool heatingEnabled = objectData?.HeatingSeasonEnabled ?? true;
            bool coolingEnabled = objectData?.CoolingSeasonEnabled ?? true;
            int? coolingStartMonth = objectData?.CoolingSeasonStartMonth;
            int? coolingEndMonth   = objectData?.CoolingSeasonEndMonth;

            var dialog = new Views.WindowBatchDetailsDialog(SelectedSummaryRow, _data.WindowBatches, 
                                                             climateZone, heatingEnabled, coolingEnabled,
                                                             coolingStartMonth, coolingEndMonth);
            dialog.ShowDialog();

            // Refresh summary after dialog closes
            RefreshSummary();
        }

        private bool CanEditOrDelete() => SelectedSummaryRow != null;

        partial void OnSelectedSummaryRowChanged(WindowSummaryRow? value)
        {
            EditSelectedCommand.NotifyCanExecuteChanged();
            DeleteSelectedCommand.NotifyCanExecuteChanged();
        }
    }
}
