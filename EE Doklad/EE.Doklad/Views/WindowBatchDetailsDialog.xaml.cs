using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.Views
{
    /// <summary>
    /// Dialog за показване и редакция на партидите в една група
    /// </summary>
    public partial class WindowBatchDetailsDialog : Window
    {
        private WindowSummaryRow _summaryRow;
        private ObservableCollection<WindowBatch> _batchesCollection;
        private int? _climateZone;
        private bool _heatingEnabled;
        private bool _coolingEnabled;
        private int? _coolingStartMonth;
        private int? _coolingEndMonth;

        public WindowBatchDetailsDialog(WindowSummaryRow summaryRow, ObservableCollection<WindowBatch> allBatches,
                                        int? climateZone = null, bool heatingEnabled = true, bool coolingEnabled = true,
                                        int? coolingStartMonth = null, int? coolingEndMonth = null)
        {
            InitializeComponent();

            _summaryRow = summaryRow;
            _batchesCollection = allBatches;
            _climateZone = climateZone;
            _heatingEnabled = heatingEnabled;
            _coolingEnabled = coolingEnabled;
            _coolingStartMonth = coolingStartMonth;
            _coolingEndMonth = coolingEndMonth;

            // Set title
            Title = $"Детайли за група: {summaryRow.TypeName} / {WindowCalculator.GetOrientationLabel(summaryRow.Orientation)}";

            // Bind to batches for both grids
            BatchesDataGridHeat.ItemsSource = _summaryRow.Batches;
            BatchesDataGridCool.ItemsSource = _summaryRow.Batches;

            UpdateSummary();
        }

        private void UpdateSummary()
        {
            SummaryTextBlock.Text = $"Общо партиди: {_summaryRow.Batches.Count} | " +
                                    $"Общо брой: {_summaryRow.TotalCount} | " +
                                    $"A_брутна: {_summaryRow.ATotalGross:F2} m² | " +
                                    $"A_стъкло: {_summaryRow.ATotalGlass:F2} m² | " +
                                    $"Ū: {_summaryRow.UAvg:F3} W/m²K | " +
                                    $"ḡ: {_summaryRow.GAvg:F3}";
        }

        private void AddBatchButton_Click(object sender, RoutedEventArgs e)
        {
            // Prefill with group data
            var newBatch = new WindowBatch
            {
                Orientation = _summaryRow.Orientation,
                Kind = _summaryRow.Batches.First().Kind,
                Width = _summaryRow.Batches.First().Width,
                Height = _summaryRow.Batches.First().Height,
                UValue = _summaryRow.Batches.First().UValue,
                UseDetailedUwMode = _summaryRow.Batches.First().UseDetailedUwMode,
                ProfileSystemId = _summaryRow.Batches.First().ProfileSystemId,
                ProfileSystemLabel = _summaryRow.Batches.First().ProfileSystemLabel,
                ProfileMountingDepthMm = _summaryRow.Batches.First().ProfileMountingDepthMm,
                ProfileVisibleHeightMm = _summaryRow.Batches.First().ProfileVisibleHeightMm,
                ProfileUFrame = _summaryRow.Batches.First().ProfileUFrame,
                ProfileUGlass = _summaryRow.Batches.First().ProfileUGlass,
                HasThermalBridge = _summaryRow.Batches.First().HasThermalBridge,
                ThermalBridgeTypeId = _summaryRow.Batches.First().ThermalBridgeTypeId,
                ThermalBridgeTypeLabel = _summaryRow.Batches.First().ThermalBridgeTypeLabel,
                ThermalBridgePsi = _summaryRow.Batches.First().ThermalBridgePsi,
                GN = _summaryRow.Batches.First().GN,
                OpticalType = _summaryRow.Batches.First().OpticalType,
                FrameFraction = _summaryRow.Batches.First().FrameFraction,
                TypeName = _summaryRow.TypeName
            };

            var dialog = new AddWindowFullDialog(newBatch, _climateZone, _heatingEnabled, _coolingEnabled,
                                                 _coolingStartMonth, _coolingEndMonth);
            if (dialog.ShowDialog() == true)
            {
                _batchesCollection.Add(dialog.Result);
                _summaryRow.Batches.Add(dialog.Result);
                BatchesDataGridHeat.Items.Refresh();
                BatchesDataGridCool.Items.Refresh();
                UpdateSummary();
            }
        }

        private void EditBatchButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button && button.DataContext is WindowBatch batch)
            {
                var dialog = new AddWindowFullDialog(batch, _climateZone, _heatingEnabled, _coolingEnabled,
                                                     _coolingStartMonth, _coolingEndMonth);
                if (dialog.ShowDialog() == true)
                {
                    // Update batch in place
                    var index = _batchesCollection.IndexOf(batch);
                    if (index >= 0)
                    {
                        _batchesCollection[index] = dialog.Result;
                    }

                    var summaryIndex = _summaryRow.Batches.IndexOf(batch);
                    if (summaryIndex >= 0)
                    {
                        _summaryRow.Batches[summaryIndex] = dialog.Result;
                    }

                    BatchesDataGridHeat.Items.Refresh();
                    BatchesDataGridCool.Items.Refresh();
                    UpdateSummary();
                }
            }
        }

        private void DeleteBatchButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button && button.DataContext is WindowBatch batch)
            {
                var result = MessageBox.Show($"Изтрий партида с {batch.Count} бр.?", 
                    "Потвърждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _batchesCollection.Remove(batch);
                    _summaryRow.Batches.Remove(batch);
                    BatchesDataGridHeat.Items.Refresh();
                    BatchesDataGridCool.Items.Refresh();
                    UpdateSummary();

                    // If no more batches, close dialog
                    if (_summaryRow.Batches.Count == 0)
                    {
                        MessageBox.Show("Всички партиди са изтрити. Групата ще бъде премахната.", 
                            "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        Close();
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
