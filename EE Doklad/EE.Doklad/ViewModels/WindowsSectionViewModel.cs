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

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private ObservableCollection<WindowSummaryRow> summaryRows = new();

        [ObservableProperty]
        private WindowSummaryRow? selectedSummaryRow;

        public WindowsSectionViewModel(WindowsSectionData data)
        {
            _data = data;
            Description = data.Description;

            // Слушаме за промени в партидите
            _data.WindowBatches.CollectionChanged += (s, e) => RefreshSummary();

            RefreshSummary();
        }

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
            var dialog = new Views.AddWindowWizardDialog();
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

            var dialog = new Views.WindowBatchDetailsDialog(SelectedSummaryRow, _data.WindowBatches);
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
