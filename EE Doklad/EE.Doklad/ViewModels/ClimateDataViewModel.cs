using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.ViewModels
{
    public partial class ClimateDataViewModel : ObservableObject
    {
        public ClimateDataViewModel() : this(new ClimateService(new JsonClimateRepository()))
        {
        }

        public ClimateDataViewModel(ClimateService climateService)
        {
            ClimateService = climateService;

            Zones = new ObservableCollection<ClimateZoneData>(climateService.GetAllZones());
            SelectedZone = Zones.Count > 0 ? Zones[0] : null;
        }

        public ClimateService ClimateService { get; }

        public ObservableCollection<ClimateZoneData> Zones { get; }

    public ObservableCollection<MonthlyRow> MonthlyRows { get; } = new();

    public ObservableCollection<HumidityRow> HumidityRows { get; } = new();

        [ObservableProperty]
        private ClimateZoneData? _selectedZone;

        partial void OnSelectedZoneChanged(ClimateZoneData? value)
        {
            MonthlyRows.Clear();
            HumidityRows.Clear();
            if (value?.Monthly == null)
                return;

            var monthLabels = TryGetMonthsOrder() ?? CultureInfo.GetCultureInfo("bg-BG").DateTimeFormat.AbbreviatedMonthNames.Take(12).ToArray();

            for (int i = 0; i < 12; i++)
            {
                var name = monthLabels.Length > i ? monthLabels[i] : (i + 1).ToString();
                if (string.IsNullOrWhiteSpace(name))
                    name = (i + 1).ToString();

                MonthlyRows.Add(new MonthlyRow
                {
                    Month = name,
                    Te = SafeGet(value.Monthly.AvgMonthlyTempC, i),
                    SolarN = SafeGet(value.Monthly.AvgFullSolarVerticalWm2, "N", i),
                    SolarE = SafeGet(value.Monthly.AvgFullSolarVerticalWm2, "E", i),
                    SolarW = SafeGet(value.Monthly.AvgFullSolarVerticalWm2, "W", i),
                    SolarS = SafeGet(value.Monthly.AvgFullSolarVerticalWm2, "S", i),
                    SolarH = SafeGet(value.Monthly.AvgFullSolarVerticalWm2, "H", i)
                });
            }

            var rhMonths = TryGetRelHumidityMonths() ?? new[] { "May", "Jun", "Jul", "Aug", "Sep" };
            for (int i = 0; i < 5; i++)
            {
                HumidityRows.Add(new HumidityRow
                {
                    Month = rhMonths.Length > i ? rhMonths[i] : (i + 1).ToString(),
                    RhPercent = SafeGet(value.Monthly.AvgMonthlyRelHumidityPercentMayToSep, i)
                });
            }

            OnPropertyChanged(nameof(SelectedZoneMeta));
        }

        public string SelectedZoneMeta
        {
            get
            {
                if (SelectedZone == null) return string.Empty;
                return $"Тe,изч = {SelectedZone.DesignOutdoorTempC:F1} °C | Денградуси (19°C) = {SelectedZone.DegreeDays19C}";
            }
        }

        private string[]? TryGetMonthsOrder()
        {
            var list = ClimateService.GetAllZones();
            // MonthsOrder is stored on seed, not zones; we expose it via ClimateService currently only as raw seed.
            // In this app version we can safely return null and rely on bg-BG abbreviations.
            return null;
        }

        private string[]? TryGetRelHumidityMonths()
        {
            return null;
        }

        private static double SafeGet(double[]? arr, int idx)
        {
            if (arr == null || arr.Length <= idx) return 0;
            return arr[idx];
        }

        private static double SafeGet(System.Collections.Generic.Dictionary<string, double[]>? dict, string key, int idx)
        {
            if (dict == null) return 0;
            if (!dict.TryGetValue(key, out var arr) || arr == null) return 0;
            if (arr.Length <= idx) return 0;
            return arr[idx];
        }

        public string MetaLine => $"Източник: {ClimateService.Source} | Ревизия: {ClimateService.Revision} | Дата: {ClimateService.Date}";
        public string ImportedBy => ClimateService.ImportedBy;
    }

    public sealed class MonthlyRow
    {
        public string Month { get; set; } = string.Empty;
        public double Te { get; set; }
        public double SolarN { get; set; }
        public double SolarE { get; set; }
        public double SolarW { get; set; }
        public double SolarS { get; set; }
        public double SolarH { get; set; }
    }

    public sealed class HumidityRow
    {
        public string Month { get; set; } = string.Empty;
        public double RhPercent { get; set; }
    }
}
