using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.Views
{
    /// <summary>
    /// РЎРµРєС†РёСЏ 23 вЂ“ Р”РµР±СЉРі РІСЉС‚СЂРµС€РЅРё С‚РѕРїР»РёРЅРЅРё РїРµС‡Р°Р»Р±Рё (С„РѕСЂРјСѓР»Рё 3.30вЂ“3.33).
    /// Р’СЃРёС‡РєРё РІС…РѕРґРЅРё РґР°РЅРЅРё РѕСЃРІРµРЅ "РњРµСЃРµС†" Рё "Р РµР¶РёРј" СЃРµ РІР·РёРјР°С‚ РѕС‚ ObjectDataSectionData (РЎРµРєС†РёСЏ 5).
    /// </summary>
    public partial class InternalGainsDebugView : UserControl
    {
        // в”Ђв”Ђ РџРѕР»РµС‚Р° в”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђ
        private readonly InternalGainsDebugInput _input;
        private readonly ObjectDataSectionData? _objData;
        private readonly InternalGainsDebugService _service;
        private readonly ObservableCollection<InternalGainsSourceInput> _sources;

        private static readonly string[] MonthNames =
        {
            "01 – Януари", "02 – Февруари", "03 – Март",
            "04 – Април",  "05 – Май",      "06 – Юни",
            "07 – Юли",    "08 – Август",   "09 – Септември",
            "10 – Октомври", "11 – Ноември", "12 – Декември"
        };

        // РўР°Р±Р»РёС†Р°: РєР»РёРјР°С‚РёС‡РЅР° Р·РѕРЅР° в†’ (startMonth, startDay, endMonth, endDay)
        private static readonly (int sm, int sd, int em, int ed)[] HeatingSeason =
        {
            (10, 21, 4, 20), // Р·РѕРЅР° 1 вЂ“ РЎРµРІРµСЂРЅРѕ Р§РµСЂРЅРѕРјРѕСЂРёРµ
            (10, 21, 4, 25), // Р·РѕРЅР° 2
            (10, 23, 4, 15), // Р·РѕРЅР° 3
            (10, 16, 4, 23), // Р·РѕРЅР° 4
            (10, 25, 4, 19), // Р·РѕРЅР° 5
            (10, 24, 4,  6), // Р·РѕРЅР° 6
            (10, 15, 4, 23), // Р·РѕРЅР° 7
            (10, 28, 4,  6), // Р·РѕРЅР° 8
            (10, 28, 4,  5), // Р·РѕРЅР° 9
        };

        // в”Ђв”Ђ РљРѕРЅСЃС‚СЂСѓРєС‚РѕСЂРё в”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђ
        public InternalGainsDebugView()
        {
            InitializeComponent();
            _input   = new InternalGainsDebugInput { ZoneId = 1 };
            _service = new InternalGainsDebugService();
            _sources = new ObservableCollection<InternalGainsSourceInput>(_input.Sources);
            Initialize();
        }

        public InternalGainsDebugView(InternalGainsDebugInput input, ObjectDataSectionData? objectData = null)
        {
            InitializeComponent();
            _input   = input ?? throw new ArgumentNullException(nameof(input));
            _objData = objectData;
            _service = new InternalGainsDebugService();
            _sources = new ObservableCollection<InternalGainsSourceInput>(_input.Sources);
            Initialize();
        }

        // в”Ђв”Ђ РРЅРёС†РёР°Р»РёР·Р°С†РёСЏ в”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђ
        private void Initialize()
        {
            // Enum ItemsSource Р·Р° DataGrid
            ColCategory.ItemsSource = Enum.GetValues(typeof(InternalGainsCategory));
            ColKind.ItemsSource     = Enum.GetValues(typeof(InternalGainsSourceKind));

            // DataGrid РёР·С‚РѕС‡РЅРёС†Рё
            DgSources.ItemsSource = _sources;

            // РњРµСЃРµС† ComboBox
            for (int i = 0; i < 12; i++)
                CmbMonth.Items.Add(MonthNames[i]);
            CmbMonth.SelectedIndex = _input.Month > 0 ? _input.Month - 1 : 0;

            // РџРѕРїСЉР»РЅРё read-only РґР°РЅРЅРё РѕС‚ РЎРµРєС†РёСЏ 5
            PopulateFromObjectData();

            // Р РµР¶РёРј ComboBox / read-only
            SetupModeControl();
        }

        // в”Ђв”Ђ РџРѕРїСЉР»РІР°РЅРµ РѕС‚ ObjectDataSectionData в”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђ
        private void PopulateFromObjectData()
        {
            if (_objData == null)
            {
                TxtNoObjectData.Visibility = Visibility.Visible;
                TxtAreaHeatRO.Text  = "–";
                TxtAreaCoolRO.Text  = "–";
                TxtHeatSeasonRO.Text = "–";
                TxtCoolSeasonRO.Text = "–";
                TxtHeatSchedRO.Text = "–";
                TxtCoolSchedRO.Text = "–";
                TxtActiveModesRO.Text = "–";
                return;
            }

            // РђРєС‚РёРІРЅРё СЂРµР¶РёРјРё
            bool heatOn = _objData.HeatingSeasonEnabled;
            bool coolOn = _objData.CoolingSeasonEnabled;
            TxtActiveModesRO.Text = heatOn && coolOn ? "Отопление + Охлаждане"
                                  : heatOn           ? "Само отопление"
                                  : coolOn           ? "Само охлаждане"
                                  : "Нито един";

            // РџР»РѕС‰Рё
            TxtAreaHeatRO.Text = ParseAreaDisplay(_objData.HeatedArea);
            TxtAreaCoolRO.Text = ParseAreaDisplay(_objData.CooledArea);

            // РћС‚РѕРїР»РёС‚РµР»РµРЅ СЃРµР·РѕРЅ РѕС‚ РєР»РёРјР°С‚РёС‡РЅР° Р·РѕРЅР°
            int zone = Math.Clamp(_objData.ClimateZone, 1, 9);
            var hs = HeatingSeason[zone - 1];
            TxtHeatSeasonRO.Text = heatOn
                ? $"{hs.sd:D2}.{hs.sm:D2} – {hs.ed:D2}.{hs.em:D2}"
                : "Неактивен";

            // РћС…Р»Р°РґРёС‚РµР»РµРЅ СЃРµР·РѕРЅ
            if (coolOn && _objData.CoolingSeasonStartMonth.HasValue && _objData.CoolingSeasonEndMonth.HasValue)
            {
                TxtCoolSeasonRO.Text =
                    $"{_objData.CoolingSeasonStartDay:D2}.{_objData.CoolingSeasonStartMonth:D2} – " +
                    $"{_objData.CoolingSeasonEndDay:D2}.{_objData.CoolingSeasonEndMonth:D2}";
            }
            else
            {
                TxtCoolSeasonRO.Text = coolOn ? "Дати не са въведени" : "Неактивен";
            }

            // РћС‚РѕРїР»РёС‚РµР»РµРЅ РіСЂР°С„РёРє (С‡Р°СЃРѕРІРµ/РґРµРЅ)
            TxtHeatSchedRO.Text = heatOn
                ? FormatSchedule(_objData.HeatingWorkdaysHours,
                                 _objData.HeatingSaturdayHours,
                                 _objData.HeatingSundayHours, " h/ден")
                : "–";

            // РћС…Р»Р°РґРёС‚РµР»РµРЅ РіСЂР°С„РёРє вЂ” РѕС‚ CoolingSchedules (TimeSpan РЅР°С‡Р°Р»Рѕ/РєСЂР°Р№ в†’ GetHours())
            if (coolOn)
            {
            var cs = _objData.CoolingSchedules?.CoolingSchedule;
                if (cs != null)
                {
                    double wdH  = cs.Workdays.GetHours();
                    double satH = cs.Saturday.GetHours();
                    double sunH = cs.Sunday.GetHours();
                    TxtCoolSchedRO.Text = FormatSchedule(
                        wdH  > 0 ? wdH.ToString("F1")  : null,
                        satH > 0 ? satH.ToString("F1") : null,
                        sunH > 0 ? sunH.ToString("F1") : null, " h/ден");
                }
                else
                {
                    TxtCoolSchedRO.Text = "Не е въведен";
                }
            }
            else
            {
                TxtCoolSchedRO.Text = "–";
            }
        }

        private static string ParseAreaDisplay(string? val)
        {
            if (string.IsNullOrWhiteSpace(val)) return "–";
            return double.TryParse(val.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double d)
                ? $"{d:F1} m²" : val;
        }

        private static string FormatSchedule(string? wd, string? sat, string? sun, string suffix)
        {
            string F(string? v) => string.IsNullOrWhiteSpace(v) || v == "0" ? "0" : v!.Trim();
            return $"{F(wd)} / {F(sat)} / {F(sun)}{suffix}";
        }

        // в”Ђв”Ђ Р РµР¶РёРј вЂ“ РµРґРёРЅРёС‡РµРЅ РёР»Рё ComboBox в”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђ
        private void SetupModeControl()
        {
            bool heatOn = _objData?.HeatingSeasonEnabled ?? true;
            bool coolOn = _objData?.CoolingSeasonEnabled ?? false;

            if (heatOn && coolOn)
            {
                // Р РґРІР°С‚Р° СЃР° Р°РєС‚РёРІРЅРё в†’ ComboBox
                CmbMode.Items.Add("Отопление (Heating)");
                CmbMode.Items.Add("Охлаждане (Cooling)");
                CmbMode.SelectedIndex = _input.Mode == EpbMode.Cooling ? 1 : 0;
                CmbMode.Visibility    = Visibility.Visible;
                TxtModeSingle.Visibility = Visibility.Collapsed;
            }
            else
            {
                // РЎР°РјРѕ РµРґРёРЅ СЂРµР¶РёРј вЂ” РїРѕРєР°Р·РІР°РјРµ read-only С‚РµРєСЃС‚
                TxtModeSingle.Text = heatOn ? "Отопление (автоматично)" : "Охлаждане (автоматично)";
                CmbMode.Visibility    = Visibility.Collapsed;
                TxtModeSingle.Visibility = Visibility.Visible;
                _input.Mode = heatOn ? EpbMode.Heating : EpbMode.Cooling;
            }
        }

        // в”Ђв”Ђ Р”РѕР±Р°РІСЏРЅРµ / РёР·С‚СЂРёРІР°РЅРµ РЅР° СЂРµРґ в”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђ
        private void BtnAddSource_Click(object sender, RoutedEventArgs e)
        {
            int idx = _sources.Count + 1;
            _sources.Add(new InternalGainsSourceInput
            {
                SourceId    = $"src-{idx}",
                Description = $"Нов източник {idx}",
                Category    = InternalGainsCategory.Appliances,
                Kind        = InternalGainsSourceKind.PowerWatts,
                Power_W     = 100
            });
        }

        private void BtnDeleteSource_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is InternalGainsSourceInput src)
                _sources.Remove(src);
        }

        // в”Ђв”Ђ РР·С‡РёСЃР»СЏРІР°РЅРµ в”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђ
        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            PanelErrors.Visibility    = Visibility.Collapsed;
            PanelResults.Visibility   = Visibility.Collapsed;
            PanelFallbacks.Visibility = Visibility.Collapsed;

            SyncInputFromObjectData();

            _input.Month   = CmbMonth.SelectedIndex + 1;
            _input.Sources = _sources.ToList();

            if (CmbMode.Visibility == Visibility.Visible)
                _input.Mode = CmbMode.SelectedIndex == 1 ? EpbMode.Cooling : EpbMode.Heating;

            var result = _service.Calculate(_input);

            if (!result.InputValid)
            {
                ShowErrors(result.ValidationErrors, isError: true);
                return;
            }
            if (result.ValidationWarnings.Count > 0)
                ShowErrors(result.ValidationWarnings, isError: false);

            PopulateResults(result);
            PanelResults.Visibility = Visibility.Visible;
        }

        // в”Ђв”Ђ РЎРёРЅС…СЂРѕРЅРёР·Р°С†РёСЏ _input РѕС‚ ObjectData в”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђ
        private void SyncInputFromObjectData()
        {
            if (_objData == null) return;

            // РџР»РѕС‰Рё
            _input.AreaHeat_m2 = ParseDouble(_objData.HeatedArea);
            _input.AreaCool_m2 = ParseDouble(_objData.CooledArea);

            // РћС‚РѕРїР»РёС‚РµР»РµРЅ СЃРµР·РѕРЅ РѕС‚ РєР»РёРјР°С‚РёС‡РЅР° Р·РѕРЅР°
            int zone = Math.Clamp(_objData.ClimateZone, 1, 9);
            var hs = HeatingSeason[zone - 1];
            _input.HeatingSeasonStartMonth = hs.sm;
            _input.HeatingSeasonStartDay   = hs.sd;
            _input.HeatingSeasonEndMonth   = hs.em;
            _input.HeatingSeasonEndDay     = hs.ed;

            // РћС…Р»Р°РґРёС‚РµР»РµРЅ СЃРµР·РѕРЅ
            _input.CoolingSeasonStartMonth = _objData.CoolingSeasonStartMonth;
            _input.CoolingSeasonStartDay   = _objData.CoolingSeasonStartDay;
            _input.CoolingSeasonEndMonth   = _objData.CoolingSeasonEndMonth;
            _input.CoolingSeasonEndDay     = _objData.CoolingSeasonEndDay;

            // РћС‚РѕРїР»РёС‚РµР»РµРЅ РіСЂР°С„РёРє (С‡Р°СЃРѕРІРµ/РґРµРЅ, СЃС‚СЂРёРЅРі)
            _input.HeatingWorkdaysHours  = ParseDoubleNull(_objData.HeatingWorkdaysHours);
            _input.HeatingSaturdayHours  = ParseDoubleNull(_objData.HeatingSaturdayHours);
            _input.HeatingSundayHours    = ParseDoubleNull(_objData.HeatingSundayHours);

            // РћС…Р»Р°РґРёС‚РµР»РµРЅ РіСЂР°С„РёРє в†’ GetHours() РѕС‚ TimeSpan РЅР°С‡Р°Р»Рѕ/РєСЂР°Р№
            var cs = _objData.CoolingSchedules?.CoolingSchedule;
            if (cs != null)
            {
                double wdH  = cs.Workdays.GetHours();
                double satH = cs.Saturday.GetHours();
                double sunH = cs.Sunday.GetHours();
                _input.CoolingWorkdaysHours  = wdH  > 0 ? wdH  : (double?)null;
                _input.CoolingSaturdayHours  = satH > 0 ? satH : (double?)null;
                _input.CoolingSundayHours    = sunH > 0 ? sunH : (double?)null;
            }
            else
            {
                // РЎС‚Р°СЂ fallback РєСЉРј string РїРѕР»РµС‚Р°
                _input.CoolingWorkdaysHours  = ParseDoubleNull(_objData.CoolingWorkdaysHours);
                _input.CoolingSaturdayHours  = ParseDoubleNull(_objData.CoolingSaturdayHours);
                _input.CoolingSundayHours    = ParseDoubleNull(_objData.CoolingSundayHours);
            }

            // DaysOff РѕС‚ РЎРµРєС†РёСЏ 5
            _input.DaysOff = new[]
            {
                ParseInt(_objData.DaysOffJanuary),
                ParseInt(_objData.DaysOffFebruary),
                ParseInt(_objData.DaysOffMarch),
                ParseInt(_objData.DaysOffApril),
                ParseInt(_objData.DaysOffMay),
                ParseInt(_objData.DaysOffJune),
                ParseInt(_objData.DaysOffJuly),
                ParseInt(_objData.DaysOffAugust),
                ParseInt(_objData.DaysOffSeptember),
                ParseInt(_objData.DaysOffOctober),
                ParseInt(_objData.DaysOffNovember),
                ParseInt(_objData.DaysOffDecember),
            };
        }

        // в”Ђв”Ђ РџРѕРїСЉР»РІР°РЅРµ РЅР° СЂРµР·СѓР»С‚Р°С‚РёС‚Рµ в”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђ
        private void PopulateResults(InternalGainsDebugResult r)
        {
            var ti = r.TimeInfo;

            TxtIsPartial.Text       = ti.IsPartialMonth ? "Да" : "Не";
            TxtDaysOffApplied.Text  = ti.DaysOffApplied.ToString();
            TxtActiveDays.Text      = $"{ti.ActiveWeekdays:F1} / {ti.ActiveSaturdays:F1} / {ti.ActiveSundays:F1}";
            TxtTotalActiveDays.Text = ti.TotalActiveDays.ToString("F2");
            TxtHoursPerDay.Text     = $"{ti.WorkdaysHoursPerDay:F1} / {ti.SaturdayHoursPerDay:F1} / {ti.SundayHoursPerDay:F1}";
            TxtFallbackHours.Text   = ti.HoursFallbackUsed ? (ti.HoursFallbackReason ?? "Fallback") : "–";
            TxtTotalHours.Text      = $"{ti.TotalActiveHours_t_m:F4} h";

            DgResults.ItemsSource      = r.SourceRows;
            DgCategorySums.ItemsSource = r.CategorySums;

            TxtQDir.Text     = $"{r.Q_HC_int_dir_z_m_kWh:F4} kWh";
            TxtQDirSpec.Text = $"{r.Q_HC_int_dir_z_m_specific_kWhM2:F6} kWh/m²";
            TxtQUncond.Text  = $"{r.Q_HC_int_uncond_contribution_kWh:F4} kWh";
            TxtQZtc.Text     = $"{r.Q_HC_int_ztc_m_kWh:F4} kWh";
            TxtAreaUsed.Text = $"{r.AreaUsed_m2:F2} m²";

            TxtTrace333.Text = r.Formula333Summary;
            TxtTrace332.Text = r.Formula332Trace;
            TxtTrace330.Text = r.Formula330Trace;

            if (r.FallbacksUsed.Count > 0)
            {
                TxtFallbacks.Text         = string.Join("\n", r.FallbacksUsed);
                PanelFallbacks.Visibility = Visibility.Visible;
            }
        }

        // в”Ђв”Ђ Р“СЂРµС€РєРё / РїСЂРµРґСѓРїСЂРµР¶РґРµРЅРёСЏ в”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђ
        private void ShowErrors(IEnumerable<string> msgs, bool isError)
        {
            TxtErrorsTitle.Text = isError ? "✖ Грешки при валидация:" : "⚠ Предупреждения:";
            TxtErrors.Text = string.Join("\n", msgs.Select((m, i) => $"  {i + 1}. {m}"));
            PanelErrors.Background = isError
                ? System.Windows.Media.Brushes.MistyRose
                : System.Windows.Media.Brushes.LightYellow;
            PanelErrors.Visibility = Visibility.Visible;
        }

        // в”Ђв”Ђ РџРѕРјРѕС‰РЅРё РјРµС‚РѕРґРё в”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђв”Ђ
        private static double ParseDouble(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            return double.TryParse(text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double v) ? v : 0;
        }

        private static double? ParseDoubleNull(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            return double.TryParse(text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double v) ? v : null;
        }

        private static int ParseInt(string? text)
            => int.TryParse(text?.Trim(), out int v) ? v : 0;
    }
}

