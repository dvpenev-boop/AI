using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using EE.Doklad.Models;

namespace EE.Doklad.Views
{
    /// <summary>
    /// Interaction logic for ObjectDataSectionEditor.xaml
    /// </summary>
    public partial class ObjectDataSectionEditor : UserControl
    {
        // Prevent re-entrant SelectionChanged handling when programmatically changing SelectedValue
        private bool _suppressEvents = false;
        // Cached month names used to (re)build end-month items
        private readonly string[] _monthNames = new[] { "януари", "февруари", "март", "април", "май", "юни", "юли", "август", "септември", "октомври", "ноември", "декември" };

        public ObjectDataSectionEditor()
        {
            InitializeComponent();
            Loaded += ObjectDataSectionEditor_Loaded;
        }

        private void ObjectDataSectionEditor_Loaded(object sender, RoutedEventArgs e)
        {
            // Намираме ComboBox за типа сграда и зареждаме ItemsSource с групирани данни
            var buildingTypeCombo = FindName("BuildingTypeCombo") as ComboBox;
            if (buildingTypeCombo == null)
            {
                // Ако няма име, търсим по Grid.Column
                buildingTypeCombo = FindBuildingTypeComboBox(this);
            }

            if (buildingTypeCombo != null)
            {
                var groupedData = CollectionViewSource.GetDefaultView(BuildingTypeInfo.All);
                groupedData.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
                buildingTypeCombo.ItemsSource = groupedData;
            }

            // Measure row heights for the 'Данни за обекта' table after layout completes and show them.
            this.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                try
                {
                    var border = this.FindName("ObjectDataBorder") as Border;
                    if (border != null && border.Child is Panel panel)
                    {
                        var sb = new System.Text.StringBuilder();
                        sb.AppendLine("Row heights for 'Данни за обекта':");
                        int idx = 0;
                        foreach (var child in panel.Children)
                        {
                            if (child is Border rowBorder)
                            {
                                // Force layout update
                                rowBorder.UpdateLayout();
                                double h = rowBorder.ActualHeight;
                                sb.AppendLine($"Row {idx}: {Math.Round(h, 1)} px");
                                idx++;
                            }
                        }
                        // Also include total height
                        sb.AppendLine($"Total rows measured: {idx}");
                        // Write heights to Debug output instead of showing a MessageBox
                        System.Diagnostics.Debug.WriteLine(sb.ToString());
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error measuring rows: " + ex.Message);
                }
            }), System.Windows.Threading.DispatcherPriority.Loaded);

            // After layout, initialize start/end day/month lists and ensure consistency
            this.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                try
                {
                    var startMonthCombo = this.FindName("CoolingStartMonthCombo") as ComboBox;
                    var startDayCombo = this.FindName("CoolingStartDayCombo") as ComboBox;
                    var endMonthCombo = this.FindName("CoolingEndMonthCombo") as ComboBox;
                    var endDayCombo = this.FindName("CoolingEndDayCombo") as ComboBox;

                    int startMonth = 1;
                    if (startMonthCombo?.SelectedValue is int sm) startMonth = sm;
                    else if (startMonthCombo?.SelectedValue is string ssm && int.TryParse(ssm, out var sp)) startMonth = sp;

                    // Ensure start day list is valid for selected start month
                    UpdateStartDays(startMonth);

                    // Ensure end month list starts from startMonth..12 and update end days
                    UpdateEndMonths(startMonth);
                    int endMonth = 1;
                    if (endMonthCombo?.SelectedValue is int em) endMonth = em;
                    else if (endMonthCombo?.SelectedValue is string esm && int.TryParse(esm, out var ep)) endMonth = ep;
                    UpdateEndDays(endMonth);

                    // Final sanity: ensure end date >= start date
                    EnsureEndDateNotBeforeStart();
                }
                catch { }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

    private void UpdateEndMonths(int startMonth)
        {
            // Ensure range is within 1..12
            if (startMonth < 1) startMonth = 1;
            if (startMonth > 12) startMonth = 12;

            var endCombo = this.FindName("CoolingEndMonthCombo") as ComboBox;
            if (endCombo == null) return;

            if (_suppressEvents) return;
            _suppressEvents = true;
            try
            {
                // Preserve currently selected end month if still valid
                int? currentEnd = null;
                if (endCombo.SelectedValue is int cv) currentEnd = cv;
                else if (endCombo.SelectedValue is string csv && int.TryParse(csv, out var t)) currentEnd = t;

                endCombo.Items.Clear();
                for (int m = startMonth; m <= 12; m++)
                {
                    var item = new ComboBoxItem { Tag = m, Content = _monthNames[m - 1] };
                    endCombo.Items.Add(item);
                }

                // If previous selection is valid (>= startMonth) re-select it, otherwise leave selection empty
                if (currentEnd.HasValue && currentEnd.Value >= startMonth && currentEnd.Value <= 12)
                {
                    endCombo.SelectedValue = currentEnd.Value;
                }
                else
                {
                    // do not force a selected value; allow empty selection
                    endCombo.SelectedIndex = -1;
                }
            }
            finally
            {
                _suppressEvents = false;
            }
        }

    private void UpdateStartDays(int startMonth)
        {
            if (startMonth < 1) startMonth = 1;
            if (startMonth > 12) startMonth = 12;

            var startDayCombo = this.FindName("CoolingStartDayCombo") as ComboBox;
            if (startDayCombo == null) return;

            if (_suppressEvents) return;
            _suppressEvents = true;
            try
            {
                int? currentStartDay = null;
                if (startDayCombo.SelectedValue is int sv) currentStartDay = sv;
                else if (startDayCombo.SelectedValue is string s && int.TryParse(s, out var t)) currentStartDay = t;

                startDayCombo.Items.Clear();
                int days = DaysInMonth(startMonth);
                for (int d = 1; d <= days; d++)
                {
                    var item = new ComboBoxItem { Tag = d, Content = d.ToString() };
                    startDayCombo.Items.Add(item);
                }

                // If previous selection is valid keep it, otherwise leave it empty (no forced default)
                if (currentStartDay.HasValue && currentStartDay.Value >= 1 && currentStartDay.Value <= days)
                {
                    startDayCombo.SelectedValue = currentStartDay.Value;
                }
                else
                {
                    startDayCombo.SelectedIndex = -1;
                }
            }
            finally
            {
                _suppressEvents = false;
            }
        }

    private void UpdateEndDays(int endMonth)
        {
            if (endMonth < 1) endMonth = 1;
            if (endMonth > 12) endMonth = 12;

            var endDayCombo = this.FindName("CoolingEndDayCombo") as ComboBox;
            if (endDayCombo == null) return;

            if (_suppressEvents) return;
            _suppressEvents = true;
            try
            {
                int? currentEndDay = null;
                if (endDayCombo.SelectedValue is int cv) currentEndDay = cv;
                else if (endDayCombo.SelectedValue is string s && int.TryParse(s, out var t)) currentEndDay = t;

                endDayCombo.Items.Clear();
                int days = DaysInMonth(endMonth);
                for (int d = 1; d <= days; d++)
                {
                    var item = new ComboBoxItem { Tag = d, Content = d.ToString() };
                    endDayCombo.Items.Add(item);
                }

                if (currentEndDay.HasValue && currentEndDay.Value >= 1 && currentEndDay.Value <= days)
                {
                    endDayCombo.SelectedValue = currentEndDay.Value;
                }
                else
                {
                    // leave empty selection rather than forcing default
                    endDayCombo.SelectedIndex = -1;
                }
            }
            finally
            {
                _suppressEvents = false;
            }
        }

        private int DaysInMonth(int month)
        {
            // Use a non-leap year (2001) to determine days in month; February -> 28
            try
            {
                return System.DateTime.DaysInMonth(2001, month);
            }
            catch
            {
                return 31;
            }
        }

        private void EnsureEndDateNotBeforeStart()
        {
            var startMonthCombo = this.FindName("CoolingStartMonthCombo") as ComboBox;
            var startDayCombo = this.FindName("CoolingStartDayCombo") as ComboBox;
            var endMonthCombo = this.FindName("CoolingEndMonthCombo") as ComboBox;
            var endDayCombo = this.FindName("CoolingEndDayCombo") as ComboBox;
            if (startMonthCombo == null || startDayCombo == null || endMonthCombo == null || endDayCombo == null)
                return;

            // If any of the relevant selections is empty, don't auto-correct — allow the user to complete both sides
            if (startMonthCombo.SelectedValue == null || startDayCombo.SelectedValue == null)
                return;
            if (endMonthCombo.SelectedValue == null || endDayCombo.SelectedValue == null)
                return;

            int sMonth = 1, sDay = 1, eMonth = 1, eDay = 1;
            if (startMonthCombo.SelectedValue is int sm) sMonth = sm;
            else if (startMonthCombo.SelectedValue is string ssm && int.TryParse(ssm, out var sp)) sMonth = sp;
            if (startDayCombo.SelectedValue is int sd) sDay = sd;
            else if (startDayCombo.SelectedValue is string ssd && int.TryParse(ssd, out var sdp)) sDay = sdp;

            if (endMonthCombo.SelectedValue is int em) eMonth = em;
            else if (endMonthCombo.SelectedValue is string esm && int.TryParse(esm, out var ep)) eMonth = ep;
            if (endDayCombo.SelectedValue is int ed) eDay = ed;
            else if (endDayCombo.SelectedValue is string esd && int.TryParse(esd, out var edp)) eDay = edp;

            bool endBeforeStart = (eMonth < sMonth) || (eMonth == sMonth && eDay < sDay);
            if (endBeforeStart)
            {
                // move end to start only when both sides had values — do it under suppression
                if (_suppressEvents) return;
                _suppressEvents = true;
                try
                {
                    endMonthCombo.SelectedValue = sMonth;
                    UpdateEndDays(sMonth);
                    // clamp end day to days in month
                    int days = DaysInMonth(sMonth);
                    int newEndDay = Math.Min(sDay, days);
                    endDayCombo.SelectedValue = newEndDay;
                }
                finally
                {
                    _suppressEvents = false;
                }
            }
        }

        private ComboBox? FindBuildingTypeComboBox(DependencyObject parent)
        {
            int childrenCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is ComboBox combo && combo.DisplayMemberPath == "DisplayName")
                {
                    return combo;
                }

                var result = FindBuildingTypeComboBox(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private static readonly Regex _digitsRegex = new Regex("^[0-9]+$", RegexOptions.Compiled);

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only digits
            e.Handled = !_digitsRegex.IsMatch(e.Text);
        }

        private void NumberOnly_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                var text = e.DataObject.GetData(DataFormats.Text) as string ?? string.Empty;
                if (!_digitsRegex.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void CoolingStartMonthCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressEvents) return;
            try
            {
                var combo = sender as ComboBox;
                int startMonth = 1;
                if (combo?.SelectedValue is int sv) startMonth = sv;
                else if (combo?.SelectedValue is string s && int.TryParse(s, out var p)) startMonth = p;

                // Update start-day valid values and end-months
                UpdateStartDays(startMonth);
                UpdateEndMonths(startMonth);

                // If end month has a value, update its days; otherwise clear end-day list
                var endMonthCombo = this.FindName("CoolingEndMonthCombo") as ComboBox;
                var endDayCombo = this.FindName("CoolingEndDayCombo") as ComboBox;
                if (endMonthCombo?.SelectedValue != null)
                {
                    int endMonth = startMonth;
                    if (endMonthCombo.SelectedValue is int em) endMonth = em;
                    else if (endMonthCombo.SelectedValue is string esm && int.TryParse(esm, out var ep)) endMonth = ep;
                    UpdateEndDays(endMonth);
                }
                else if (endDayCombo != null)
                {
                    // No selected end month: clear end-day items
                    endDayCombo.Items.Clear();
                    endDayCombo.SelectedIndex = -1;
                }

                EnsureEndDateNotBeforeStart();
            }
            catch { }
        }

        private void CoolingStartDayCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressEvents) return;
            try
            {
                // If user moves start day forward past end day in same month, ensure end >= start
                EnsureEndDateNotBeforeStart();
            }
            catch { }
        }

        private void CoolingEndMonthCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressEvents) return;
            try
            {
                var combo = sender as ComboBox;
                int endMonth = 1;
                if (combo?.SelectedValue is int sv) endMonth = sv;
                else if (combo?.SelectedValue is string s && int.TryParse(s, out var p)) endMonth = p;

                UpdateEndDays(endMonth);
                EnsureEndDateNotBeforeStart();
            }
            catch { }
        }

        private void CoolingEndDayCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressEvents) return;
            try
            {
                EnsureEndDateNotBeforeStart();
            }
            catch { }
        }
    }
}
