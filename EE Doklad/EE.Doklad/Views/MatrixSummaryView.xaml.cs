using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.Views
{
    public partial class MatrixSummaryView : UserControl
    {
        public MatrixSummaryView()
        {
            InitializeComponent();
        }

        // Dependency properties

        public static readonly DependencyProperty BatchesProperty = DependencyProperty.Register(
            "Batches", typeof(ObservableCollection<WindowBatch>), typeof(MatrixSummaryView),
            new PropertyMetadata(null, OnBatchesChanged));

        public ObservableCollection<WindowBatch> Batches
        {
            get => (ObservableCollection<WindowBatch>)GetValue(BatchesProperty);
            set => SetValue(BatchesProperty, value);
        }

        public static readonly DependencyProperty HeatingEnabledProperty = DependencyProperty.Register(
            "HeatingEnabled", typeof(bool), typeof(MatrixSummaryView),
            new PropertyMetadata(true, OnSeasonChanged));

        public bool HeatingEnabled
        {
            get => (bool)GetValue(HeatingEnabledProperty);
            set => SetValue(HeatingEnabledProperty, value);
        }

        public static readonly DependencyProperty CoolingEnabledProperty = DependencyProperty.Register(
            "CoolingEnabled", typeof(bool), typeof(MatrixSummaryView),
            new PropertyMetadata(true, OnSeasonChanged));

        public bool CoolingEnabled
        {
            get => (bool)GetValue(CoolingEnabledProperty);
            set => SetValue(CoolingEnabledProperty, value);
        }

        // Change callbacks

        private static void OnBatchesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MatrixSummaryView view)
            {
                if (e.OldValue is ObservableCollection<WindowBatch> old)
                    old.CollectionChanged -= (s, ev) => view.RebuildMatrix();
                if (e.NewValue is ObservableCollection<WindowBatch> coll)
                    coll.CollectionChanged += (s, ev) => view.RebuildMatrix();
                view.RebuildMatrix();
            }
        }

        private static void OnSeasonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MatrixSummaryView view) view.RebuildMatrix();
        }

        // Orientation order

        private readonly EE.Doklad.Models.Orientation[] orientationsOrder = new[]
        {
            EE.Doklad.Models.Orientation.East,
            EE.Doklad.Models.Orientation.NorthEast,
            EE.Doklad.Models.Orientation.North,
            EE.Doklad.Models.Orientation.NorthWest,
            EE.Doklad.Models.Orientation.West,
            EE.Doklad.Models.Orientation.SouthWest,
            EE.Doklad.Models.Orientation.South,
            EE.Doklad.Models.Orientation.SouthEast
        };

        // Main rebuild

        private void RebuildMatrix()
        {
            MatrixGrid.Children.Clear();
            MatrixGrid.ColumnDefinitions.Clear();
            MatrixGrid.RowDefinitions.Clear();

            if (Batches == null) return;

            var outerStack = new StackPanel { Orientation = System.Windows.Controls.Orientation.Vertical };

            if (HeatingEnabled)
                outerStack.Children.Add(BuildSummaryTable(WindowSummarizationMode.Heating));

            if (CoolingEnabled)
            {
                if (HeatingEnabled)
                    outerStack.Children.Add(new FrameworkElement { Height = 12 }); // spacer
                outerStack.Children.Add(BuildSummaryTable(WindowSummarizationMode.Cooling));
            }

            MatrixGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            MatrixGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            Grid.SetRow(outerStack, 0);
            Grid.SetColumn(outerStack, 0);
            MatrixGrid.Children.Add(outerStack);
        }

        private FrameworkElement BuildSummaryTable(WindowSummarizationMode mode)
        {
            var container = new StackPanel { Orientation = System.Windows.Controls.Orientation.Vertical };

            string title = mode == WindowSummarizationMode.Heating
                ? "Обобщена таблица Отопление:"
                : "Обобщена таблица Охлаждане:";
            container.Children.Add(new TextBlock
            {
                Text = title,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 4)
            });

            var tableGrid = new Grid();
            container.Children.Add(new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Content = tableGrid
            });

            if (Batches == null || Batches.Count == 0)
            {
                tableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                tableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                var empty = new TextBlock { Text = "Няма въведени елементи.", Foreground = Brushes.Gray, Margin = new Thickness(4) };
                Grid.SetRow(empty, 0); Grid.SetColumn(empty, 0);
                tableGrid.Children.Add(empty);
                return container;
            }

            var groups = WindowCalculator.GroupBatchesForMode(Batches, mode);
            var typeGroups = groups
                .GroupBy(g => g.TypeSignature)
                .Select(g => new
                {
                    TypeSignature = g.Key,
                    TypeName = g.First().TypeName,
                    FirstBatch = g.First().Batches.FirstOrDefault(),
                    Groups = g.ToList()
                })
                .OrderByDescending(t => t.Groups.Sum(x => x.ATotalGross))
                .ToList();

            // Columns: #, Вид, L, h, A, U, [n,g,A]*8, A_total, Details
            int fixedCols = 6;
            int orientationCols = orientationsOrder.Length * 3;
            int rightCols = 2;
            int totalCols = fixedCols + orientationCols + rightCols;

            for (int i = 0; i < totalCols; i++)
                tableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Header row 1
            tableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            int col = 0;
            AddHeaderCell(tableGrid, "#",          0, col++);
            AddHeaderCell(tableGrid, "Вид",        0, col++);
            AddHeaderCell(tableGrid, "L [m]",      0, col++);
            AddHeaderCell(tableGrid, "h [m]",      0, col++);
            AddHeaderCell(tableGrid, "A [m\u00B2]",     0, col++);
            AddHeaderCell(tableGrid, "U [W/m\u00B2K]",  0, col++);

            for (int i = 0; i < orientationsOrder.Length; i++)
            {
                var label = WindowCalculator.GetOrientationLabel(orientationsOrder[i]);
                var tb = new TextBlock { Text = label, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center };
                var b = new Border { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(0.5), Child = tb, Padding = new Thickness(4) };
                Grid.SetRow(b, 0); Grid.SetColumn(b, col); Grid.SetColumnSpan(b, 3);
                tableGrid.Children.Add(b);
                col += 3;
            }
            AddHeaderCell(tableGrid, "A total [m\u00B2]", 0, col++);
            AddHeaderCell(tableGrid, "", 0, col++);

            // Header row 2: n|g|A
            tableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            int hRow2 = 1;
            col = fixedCols;
            for (int i = 0; i < orientationsOrder.Length; i++)
            {
                AddHeaderCell(tableGrid, "n", hRow2, col++);
                AddHeaderCell(tableGrid, "g", hRow2, col++);
                AddHeaderCell(tableGrid, "A", hRow2, col++);
            }
            AddHeaderCell(tableGrid, "", hRow2, col++);
            AddHeaderCell(tableGrid, "", hRow2, col++);

            // Data rows
            int rowIndex = 2;
            int index = 1;
            foreach (var tg in typeGroups)
            {
                tableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                col = 0;

                AddTextCell(tableGrid, index.ToString(), rowIndex, col++);

                string kindLabel = tg.FirstBatch != null
                    ? (tg.FirstBatch.Kind == WindowKind.Door ? "ВР" : "ПР")
                    : "ПР";
                AddTextCell(tableGrid, kindLabel, rowIndex, col++, HorizontalAlignment.Center);

                string lv = tg.FirstBatch != null && tg.FirstBatch.Width  > 0 ? tg.FirstBatch.Width.ToString("F2")  : "-";
                string hv = tg.FirstBatch != null && tg.FirstBatch.Height > 0 ? tg.FirstBatch.Height.ToString("F2") : "-";
                string av = tg.FirstBatch != null ? tg.FirstBatch.AreaGross.ToString("F2") : "-";
                double uVal = tg.Groups.FirstOrDefault()?.UAvg ?? 0.0;
                AddTextCell(tableGrid, lv, rowIndex, col++);
                AddTextCell(tableGrid, hv, rowIndex, col++);
                AddTextCell(tableGrid, av, rowIndex, col++);
                AddTextCell(tableGrid, uVal.ToString("F3"), rowIndex, col++);

                foreach (var orientation in orientationsOrder)
                {
                    var grow = tg.Groups.FirstOrDefault(g => g.Orientation == orientation);
                    int    n     = grow?.TotalCount  ?? 0;
                    double Acell = grow?.ATotalGross ?? 0.0;
                    double gval  = grow?.GAvg        ?? 0.0;
                    AddTextCell(tableGrid, n.ToString(),         rowIndex, col++, HorizontalAlignment.Center);
                    AddTextCell(tableGrid, gval.ToString("F2"),  rowIndex, col++, HorizontalAlignment.Center);
                    AddTextCell(tableGrid, Acell.ToString("F2"), rowIndex, col++, HorizontalAlignment.Right);
                }

                double Atotal = tg.Groups.Sum(g => g.ATotalGross);
                AddTextCell(tableGrid, Atotal.ToString("F2"), rowIndex, col++, HorizontalAlignment.Right);

                var btn = new Button
                {
                    Content = "Детайли >",
                    Background = new SolidColorBrush(Color.FromRgb(255, 152, 0)),
                    Foreground = Brushes.White,
                    Padding = new Thickness(6, 2, 6, 2),
                    Tag = new { tg.TypeSignature, Mode = mode }
                };
                btn.Click += DetailsButton_Click;
                var bbtn = new Border { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(0.5), Child = btn };
                Grid.SetRow(bbtn, rowIndex); Grid.SetColumn(bbtn, col - 1);
                tableGrid.Children.Add(bbtn);

                rowIndex++;
                index++;
            }

            // Summary rows
            var orientationLabels = orientationsOrder.Select(o => WindowCalculator.GetOrientationLabel(o)).ToList();
            var facadeTotals = orientationLabels.ToDictionary(l => l, _ => 0.0);
            var facadeGnum   = orientationLabels.ToDictionary(l => l, _ => 0.0);
            var facadeGden   = orientationLabels.ToDictionary(l => l, _ => 0.0);
            var facadeUnum   = orientationLabels.ToDictionary(l => l, _ => 0.0);
            var facadeUden   = orientationLabels.ToDictionary(l => l, _ => 0.0);

            foreach (var bt in Batches ?? Enumerable.Empty<WindowBatch>())
            {
                double gEffMode = WindowCalculator.GetGEffForMode(bt, mode);
                double weight = bt.AreaGlass > 0 ? bt.AreaGlass : bt.AreaGross;
                double A = bt.Count * bt.AreaGross;
                var lbl = WindowCalculator.GetOrientationLabel(bt.Orientation);
                if (!facadeTotals.ContainsKey(lbl)) continue;
                facadeTotals[lbl] += A;
                facadeGnum[lbl]   += bt.Count * weight * gEffMode;
                facadeGden[lbl]   += bt.Count * weight;
                facadeUnum[lbl]   += bt.Count * bt.AreaGross * bt.UValue;
                facadeUden[lbl]   += bt.Count * bt.AreaGross;
            }

            // A total row
            tableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            int aRow = rowIndex;
            col = 0;
            AddTextCellBold(tableGrid, "A общо", aRow, col++);
            for (int i = 1; i < fixedCols; i++) AddTextCell(tableGrid, "", aRow, col++);
            foreach (var o in orientationLabels)
            {
                AddTextCell(tableGrid, facadeTotals[o].ToString("F2"), aRow, col++, HorizontalAlignment.Right);
                AddTextCell(tableGrid, "", aRow, col++);
                AddTextCell(tableGrid, "", aRow, col++);
            }
            AddTextCell(tableGrid, (Batches?.Sum(bt => bt.Count * bt.AreaGross) ?? 0.0).ToString("F2"), aRow, col++, HorizontalAlignment.Right);
            AddTextCell(tableGrid, "", aRow, col++);

            // g average row
            tableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            int gRow = aRow + 1;
            col = 0;
            AddTextCellBold(tableGrid, "g средно", gRow, col++);
            for (int i = 1; i < fixedCols; i++) AddTextCell(tableGrid, "", gRow, col++);
            foreach (var o in orientationLabels)
            {
                double gavg = facadeGden[o] > 0 ? facadeGnum[o] / facadeGden[o] : 0.0;
                AddTextCell(tableGrid, gavg.ToString("F2"), gRow, col++, HorizontalAlignment.Center);
                AddTextCell(tableGrid, "", gRow, col++);
                AddTextCell(tableGrid, "", gRow, col++);
            }
            AddTextCell(tableGrid, "", gRow, col++);
            AddTextCell(tableGrid, "", gRow, col++);

            // U average row
            tableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            int uRow = gRow + 1;
            col = 0;
            AddTextCellBold(tableGrid, "U средно", uRow, col++);
            for (int i = 1; i < fixedCols; i++) AddTextCell(tableGrid, "", uRow, col++);
            foreach (var o in orientationLabels)
            {
                double uavg = facadeUden[o] > 0 ? facadeUnum[o] / facadeUden[o] : 0.0;
                AddTextCell(tableGrid, uavg.ToString("F2"), uRow, col++, HorizontalAlignment.Center);
                AddTextCell(tableGrid, "", uRow, col++);
                AddTextCell(tableGrid, "", uRow, col++);
            }
            AddTextCell(tableGrid, "", uRow, col++);
            AddTextCell(tableGrid, "", uRow, col++);

            return container;
        }

        // Details button handler

        private void DetailsButton_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                dynamic? tag = btn.Tag;
                if (tag == null) return;
                string typeSignature = (string)tag.TypeSignature;

                var allGroups = WindowCalculator.GroupBatches(Batches);
                var filtered = allGroups.Where(g => g.TypeSignature == typeSignature).ToList();
                var combinedBatches = filtered.SelectMany(g => g.Batches).ToList();
                if (combinedBatches.Count == 0) return;

                var summary = new WindowSummaryRow
                {
                    TypeName      = filtered.First().TypeName,
                    TypeSignature = typeSignature,
                    Batches       = combinedBatches,
                    TotalCount    = combinedBatches.Sum(b => b.Count),
                    ATotalGross   = combinedBatches.Sum(b => b.Count * b.AreaGross),
                    ATotalGlass   = combinedBatches.Sum(b => b.Count * b.AreaGlass),
                    UAvg          = combinedBatches.Sum(b => b.Count * b.AreaGross * b.UValue) / Math.Max(1e-9, combinedBatches.Sum(b => b.Count * b.AreaGross)),
                    GAvg          = combinedBatches.Sum(b => b.Count * (b.AreaGlass > 0 ? b.AreaGlass : b.AreaGross) * b.GEff) / Math.Max(1e-9, combinedBatches.Sum(b => b.Count * (b.AreaGlass > 0 ? b.AreaGlass : b.AreaGross))),
                    Orientation   = combinedBatches.First().Orientation
                };

                var dialog = new WindowBatchDetailsDialog(summary, Batches);
                dialog.ShowDialog();
                RebuildMatrix();
            }
        }

        // Cell helpers

        private static void AddHeaderCell(Grid grid, string text, int row, int col)
            => AddHeaderCell(grid, text, row, col, HorizontalAlignment.Center);

        private static void AddHeaderCell(Grid grid, string text, int row, int col, HorizontalAlignment halign)
        {
            var tb = new TextBlock { Text = text, FontWeight = FontWeights.SemiBold, HorizontalAlignment = halign };
            var b = new Border { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(0.5), Child = tb, Padding = new Thickness(6) };
            Grid.SetRow(b, row); Grid.SetColumn(b, col);
            grid.Children.Add(b);
        }

        private static void AddTextCell(Grid grid, string text, int row, int col, HorizontalAlignment halign = HorizontalAlignment.Left)
        {
            var tb = new TextBlock { Text = text, HorizontalAlignment = halign, Margin = new Thickness(4, 2, 4, 2) };
            var b = new Border { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(0.5), Child = tb, Padding = new Thickness(2) };
            Grid.SetRow(b, row); Grid.SetColumn(b, col);
            grid.Children.Add(b);
        }

        private static void AddTextCellBold(Grid grid, string text, int row, int col, HorizontalAlignment halign = HorizontalAlignment.Left)
        {
            var tb = new TextBlock { Text = text, HorizontalAlignment = halign, Margin = new Thickness(4, 2, 4, 2), FontWeight = FontWeights.Bold };
            var b = new Border { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(0.5), Child = tb, Padding = new Thickness(2) };
            Grid.SetRow(b, row); Grid.SetColumn(b, col);
            grid.Children.Add(b);
        }
    }
}