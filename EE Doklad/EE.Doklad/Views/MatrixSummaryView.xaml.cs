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

        public static readonly DependencyProperty BatchesProperty = DependencyProperty.Register(
            "Batches", typeof(ObservableCollection<WindowBatch>), typeof(MatrixSummaryView),
            new PropertyMetadata(null, OnBatchesChanged));

        public ObservableCollection<WindowBatch> Batches
        {
            get => (ObservableCollection<WindowBatch>)GetValue(BatchesProperty);
            set => SetValue(BatchesProperty, value);
        }

        private static void OnBatchesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MatrixSummaryView view)
            {
                if (e.OldValue is ObservableCollection<WindowBatch> old)
                {
                    old.CollectionChanged -= (s, ev) => view.RebuildMatrix();
                }
                if (e.NewValue is ObservableCollection<WindowBatch> coll)
                {
                    coll.CollectionChanged += (s, ev) => view.RebuildMatrix();
                }
                view.RebuildMatrix();
            }
        }

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

        private void RebuildMatrix()
        {
            MatrixGrid.Children.Clear();
            MatrixGrid.ColumnDefinitions.Clear();
            MatrixGrid.RowDefinitions.Clear();

            if (Batches == null) return;

            var groups = WindowCalculator.GroupBatches(Batches);
            // group by type signature across orientations
            var typeGroups = groups.GroupBy(g => g.TypeSignature)
                                   .Select(g => new
                                   {
                                       TypeSignature = g.Key,
                                       TypeName = g.First().TypeName,
                                       Groups = g.ToList()
                                   })
                                   .OrderByDescending(t => t.Groups.Sum(x => x.ATotalGross))
                                   .ToList();

            // columns: fixed left block (№, L, h, A, U) => 5 cols
            int fixedCols = 5;
            int orientationCols = orientationsOrder.Length * 3; // n,g,A per orientation
            int rightCols = 2; // A_total and Details
            int totalCols = fixedCols + orientationCols + rightCols;

            for (int i = 0; i < totalCols; i++)
            {
                MatrixGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            // Header row
            MatrixGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            int col = 0;
            AddHeaderCell("#", 0, col++);
            AddHeaderCell("L [m]", 0, col++);
            AddHeaderCell("h [m]", 0, col++);
            AddHeaderCell("A [m²]", 0, col++);
            AddHeaderCell("U [W/m²K]", 0, col++);

            // Orientation group headers - we'll add small 2-row header: top label spans 3 cols
            for (int i = 0; i < orientationsOrder.Length; i++)
            {
                var label = WindowCalculator.GetOrientationLabel(orientationsOrder[i]);
                // Top header spanning 3 columns
                var tb = new TextBlock { Text = label, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center };
                Border b = new Border { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(0.5), Child = tb, Padding = new Thickness(4) };
                Grid.SetRow(b, 0);
                Grid.SetColumn(b, col);
                Grid.SetColumnSpan(b, 3);
                MatrixGrid.Children.Add(b);
                col += 3;
            }

            AddHeaderCell("A total [m²]", 0, col++);
            AddHeaderCell("", 0, col++); // details

            // Second header row: n | g | A for each orientation
            MatrixGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            int headerRow2 = 1;
            col = fixedCols;
            for (int i = 0; i < orientationsOrder.Length; i++)
            {
                AddHeaderCell("n", headerRow2, col++);
                AddHeaderCell("g", headerRow2, col++);
                AddHeaderCell("A", headerRow2, col++);
            }
            // empty cells under fixed cols
            // add empty cells for A total and details header row2
            AddHeaderCell("", headerRow2, col++);
            AddHeaderCell("", headerRow2, col++);

            // Data rows
            int rowIndex = 2;
            int index = 1;
            foreach (var tg in typeGroups)
            {
                MatrixGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                col = 0;
                // index
                AddTextCell(index.ToString(), rowIndex, col++);
                // L, h, A (take from first group's first batch)
                var sampleBatch = tg.Groups.SelectMany(g => g.Batches).FirstOrDefault();
                string l = sampleBatch != null && sampleBatch.Width > 0 ? sampleBatch.Width.ToString("F2") : "—";
                string h = sampleBatch != null && sampleBatch.Height > 0 ? sampleBatch.Height.ToString("F2") : "—";
                string a = sampleBatch != null ? sampleBatch.AreaGross.ToString("F2") : "—";
                double uVal = tg.Groups.FirstOrDefault()?.UAvg ?? 0.0;
                string u = uVal.ToString("F3");
                AddTextCell(l, rowIndex, col++);
                AddTextCell(h, rowIndex, col++);
                AddTextCell(a, rowIndex, col++);
                AddTextCell(u, rowIndex, col++);

                // orientation cells
                foreach (var orientation in orientationsOrder)
                {
                    var grow = tg.Groups.FirstOrDefault(g => g.Orientation == orientation);
                    int n = grow?.TotalCount ?? 0;
                    double Acell = grow?.ATotalGross ?? 0.0;
                    double gval = grow?.GAvg ?? 0.0;
                    AddTextCell(n.ToString(), rowIndex, col++ , HorizontalAlignment.Center);
                    AddTextCell(gval.ToString("F2"), rowIndex, col++ , HorizontalAlignment.Center);
                    AddTextCell(Acell.ToString("F2"), rowIndex, col++ , HorizontalAlignment.Right);
                }

                // A_total
                double Atotal = tg.Groups.Sum(g => g.ATotalGross);
                AddTextCell(Atotal.ToString("F2"), rowIndex, col++ , HorizontalAlignment.Right);

                // Details button
                var btn = new Button { Content = "Детайли >", Background = new SolidColorBrush(Color.FromRgb(255,152,0)), Foreground = Brushes.White, Padding = new Thickness(6,2,6,2), Tag = tg.TypeSignature };
                btn.Click += DetailsButton_Click;
                Border bbtn = new Border { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(0.5), Child = btn };
                Grid.SetRow(bbtn, rowIndex);
                Grid.SetColumn(bbtn, col -1);
                MatrixGrid.Children.Add(bbtn);

                rowIndex++;
                index++;
            }

            // Summary rows: A total per facade, g avg per facade, U avg per facade
            // Compute per orientation using raw batches
            var orientationLabels = orientationsOrder.Select(o => WindowCalculator.GetOrientationLabel(o)).ToList();
            var facadeTotals = new Dictionary<string, double>();
            var facadeGnum = new Dictionary<string, double>();
            var facadeGden = new Dictionary<string, double>();
            var facadeUnum = new Dictionary<string, double>();
            var facadeUden = new Dictionary<string, double>();

            foreach (var lbl in orientationLabels)
            {
                facadeTotals[lbl] = 0.0;
                facadeGnum[lbl] = 0.0;
                facadeGden[lbl] = 0.0;
                facadeUnum[lbl] = 0.0;
                facadeUden[lbl] = 0.0;
            }

            foreach (var b in Batches ?? System.Linq.Enumerable.Empty<WindowBatch>())
            {
                var weight = b.AreaGlass > 0 ? b.AreaGlass : b.AreaGross;
                var A = b.Count * b.AreaGross;
                var lbl = WindowCalculator.GetOrientationLabel(b.Orientation);
                if (!facadeTotals.ContainsKey(lbl)) continue;
                facadeTotals[lbl] += A;
                facadeGnum[lbl] += b.Count * weight * b.GEff;
                facadeGden[lbl] += b.Count * weight;
                facadeUnum[lbl] += b.Count * b.AreaGross * b.UValue;
                facadeUden[lbl] += b.Count * b.AreaGross;
            }

            // A total row
            MatrixGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            int aRow = rowIndex;
            col = 0;
            AddTextCellBold("A общо", aRow, col++ );
            AddTextCell("", aRow, col++);
            AddTextCell("", aRow, col++);
            AddTextCell("", aRow, col++);
            AddTextCell("", aRow, col++);
            foreach (var o in orientationLabels)
            {
                AddTextCell(((double)facadeTotals[o]).ToString("F2"), aRow, col++ , HorizontalAlignment.Right);
                AddTextCell("", aRow, col++);
                AddTextCell("", aRow, col++);
            }
            AddTextCell((Batches?.Sum(b => b.Count * b.AreaGross) ?? 0.0).ToString("F2"), aRow, col++ , HorizontalAlignment.Right);
            AddTextCell("", aRow, col++);

            // g average row
            MatrixGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            int gRow = aRow + 1;
            col = 0;
            AddTextCellBold("g средно", gRow, col++ );
            AddTextCell("", gRow, col++);
            AddTextCell("", gRow, col++);
            AddTextCell("", gRow, col++);
            AddTextCell("", gRow, col++);
            foreach (var o in orientationLabels)
            {
                double gavg = facadeGden[o] > 0 ? facadeGnum[o] / facadeGden[o] : 0.0;
                AddTextCell(gavg.ToString("F2"), gRow, col++ , HorizontalAlignment.Center);
                AddTextCell("", gRow, col++);
                AddTextCell("", gRow, col++);
            }
            AddTextCell("", gRow, col++);
            AddTextCell("", gRow, col++);

            // U average row
            MatrixGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            int uRow = gRow + 1;
            col = 0;
            AddTextCellBold("U средно", uRow, col++ );
            AddTextCell("", uRow, col++);
            AddTextCell("", uRow, col++);
            AddTextCell("", uRow, col++);
            AddTextCell("", uRow, col++);
            foreach (var o in orientationLabels)
            {
                double uavg = facadeUden[o] > 0 ? facadeUnum[o] / facadeUden[o] : 0.0;
                AddTextCell(uavg.ToString("F2"), uRow, col++ , HorizontalAlignment.Center);
                AddTextCell("", uRow, col++);
                AddTextCell("", uRow, col++);
            }
            AddTextCell("", uRow, col++);
            AddTextCell("", uRow, col++);
        }

        private void DetailsButton_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string typeSignature)
            {
                // find all batches matching typeSignature
                var groups = WindowCalculator.GroupBatches(Batches);
                var filtered = groups.Where(g => g.TypeSignature == typeSignature).ToList();
                var allBatches = Batches;
                // Build a combined summary row using first group's TypeName and aggregate batches across orientations
                var combinedBatches = filtered.SelectMany(g => g.Batches).ToList();
                if (combinedBatches.Count == 0) return;
                var summary = new WindowSummaryRow
                {
                    TypeName = filtered.First().TypeName,
                    TypeSignature = typeSignature,
                    Batches = combinedBatches,
                    TotalCount = combinedBatches.Sum(b => b.Count),
                    ATotalGross = combinedBatches.Sum(b => b.Count * b.AreaGross),
                    ATotalGlass = combinedBatches.Sum(b => b.Count * b.AreaGlass),
                    UAvg = combinedBatches.Sum(b => b.Count * b.AreaGross * b.UValue) / Math.Max(1e-9, combinedBatches.Sum(b => b.Count * b.AreaGross)),
                    GAvg = combinedBatches.Sum(b => b.Count * (b.AreaGlass > 0 ? b.AreaGlass : b.AreaGross) * b.GEff) / Math.Max(1e-9, combinedBatches.Sum(b => b.Count * (b.AreaGlass > 0 ? b.AreaGlass : b.AreaGross)))
                };

                // Use orientation of first batch for dialog title
                summary.Orientation = combinedBatches.First().Orientation;

                var dialog = new WindowBatchDetailsDialog(summary, Batches);
                dialog.ShowDialog();

                // After dialog closes, rebuild matrix
                RebuildMatrix();
            }
        }

        private void AddHeaderCell(string text, int row, int col)
        {
            AddHeaderCell(text, row, col, HorizontalAlignment.Center);
        }

        private void AddHeaderCell(string text, int row, int col, HorizontalAlignment halign)
        {
            var tb = new TextBlock { Text = text, FontWeight = FontWeights.SemiBold, HorizontalAlignment = halign };
            Border b = new Border { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(0.5), Child = tb, Padding = new Thickness(6) };
            Grid.SetRow(b, row);
            Grid.SetColumn(b, col);
            MatrixGrid.Children.Add(b);
        }

        private void AddTextCell(string text, int row, int col, HorizontalAlignment halign = HorizontalAlignment.Left)
        {
            var tb = new TextBlock { Text = text, HorizontalAlignment = halign, Margin = new Thickness(4,2,4,2) };
            Border b = new Border { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(0.5), Child = tb, Padding = new Thickness(2) };
            Grid.SetRow(b, row);
            Grid.SetColumn(b, col);
            MatrixGrid.Children.Add(b);
        }

        private void AddTextCellBold(string text, int row, int col, HorizontalAlignment halign = HorizontalAlignment.Left)
        {
            var tb = new TextBlock { Text = text, HorizontalAlignment = halign, Margin = new Thickness(4,2,4,2), FontWeight = FontWeights.Bold };
            Border b = new Border { BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(0.5), Child = tb, Padding = new Thickness(2) };
            Grid.SetRow(b, row);
            Grid.SetColumn(b, col);
            MatrixGrid.Children.Add(b);
        }
    }
}
