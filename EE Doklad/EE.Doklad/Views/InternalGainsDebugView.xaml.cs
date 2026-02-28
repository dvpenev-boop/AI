using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.Views
{
    public partial class InternalGainsDebugView : UserControl
    {
        private readonly Report? _report;
        private readonly InternalGainsDebugInput _input;
        private readonly ObjectDataSectionData? _objData;

        private static readonly (int sm, int sd, int em, int ed)[] HeatingSeason =
        {
            (10, 21, 4, 20),
            (10, 21, 4, 25),
            (10, 23, 4, 15),
            (10, 16, 4, 23),
            (10, 25, 4, 19),
            (10, 24, 4,  6),
            (10, 15, 4, 23),
            (10, 28, 4,  6),
            (10, 28, 4,  5),
        };

        private static readonly string[] MonthNames =
        {
            "01 Яну", "02 Фев", "03 Мар",
            "04 Апр", "05 Май", "06 Юни",
            "07 Юли", "08 Авг", "09 Сеп",
            "10 Окт", "11 Ное", "12 Дек"
        };

        public InternalGainsDebugView()
        {
            InitializeComponent();
            _input = new InternalGainsDebugInput { ZoneId = 1 };
            Initialize();
        }

        public InternalGainsDebugView(InternalGainsDebugInput input,
                                      ObjectDataSectionData? objectData = null,
                                      Report? report = null)
        {
            InitializeComponent();
            _input   = input ?? new InternalGainsDebugInput { ZoneId = 1 };
            _objData = objectData;
            _report  = report;
            Initialize();
        }

        private void Initialize()
        {
            PopulateFromObjectData();
            TxtProcessHeat.Text  = _input.ProcessHeat_W.ToString("F1");
            TxtProcessHours.Text = _input.ProcessAnnualHours.ToString("F1");
        }

        private void PopulateFromObjectData()
        {
            if (_objData == null)
            {
                TxtNoObjectData.Visibility = Visibility.Visible;
                TxtAreaHeatRO.Text    = "--";
                TxtAreaCoolRO.Text    = "--";
                TxtHeatSeasonRO.Text  = "--";
                TxtCoolSeasonRO.Text  = "--";
                TxtHeatSchedRO.Text   = "--";
                TxtCoolSchedRO.Text   = "--";
                TxtActiveModesRO.Text = "--";
                TxtAuse.Text          = "--";
                return;
            }

            bool heatOn = _objData.HeatingSeasonEnabled;
            bool coolOn = _objData.CoolingSeasonEnabled;
            TxtActiveModesRO.Text = heatOn && coolOn ? "Отопление + Охлаждане"
                                  : heatOn           ? "Само отопление"
                                  : coolOn           ? "Само охлаждане"
                                  : "Нито един";

            TxtAreaHeatRO.Text = ParseAreaDisplay(_objData.HeatedArea);
            TxtAreaCoolRO.Text = ParseAreaDisplay(_objData.CooledArea);
            TxtAuse.Text       = ParseAreaDisplay(_objData.HeatedArea);

            int zone = Math.Clamp(_objData.ClimateZone, 1, 9);
            var hs = HeatingSeason[zone - 1];
            TxtHeatSeasonRO.Text = heatOn
                ? $"{hs.sd:D2}.{hs.sm:D2} - {hs.ed:D2}.{hs.em:D2}"
                : "Неактивен";

            if (coolOn && _objData.CoolingSeasonStartMonth.HasValue && _objData.CoolingSeasonEndMonth.HasValue)
            {
                TxtCoolSeasonRO.Text =
                    $"{_objData.CoolingSeasonStartDay:D2}.{_objData.CoolingSeasonStartMonth:D2} - " +
                    $"{_objData.CoolingSeasonEndDay:D2}.{_objData.CoolingSeasonEndMonth:D2}";
            }
            else
            {
                TxtCoolSeasonRO.Text = coolOn ? "Дати не са въведени" : "Неактивен";
            }

            TxtHeatSchedRO.Text = heatOn
                ? FormatSchedule(_objData.HeatingWorkdaysHours,
                                 _objData.HeatingSaturdayHours,
                                 _objData.HeatingSundayHours)
                : "--";

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
                        sunH > 0 ? sunH.ToString("F1") : null);
                }
                else
                {
                    TxtCoolSchedRO.Text = "Не е въведен";
                }
            }
            else
            {
                TxtCoolSchedRO.Text = "--";
            }
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            PanelErrors.Visibility    = Visibility.Collapsed;
            PanelResults.Visibility   = Visibility.Collapsed;
            PanelFallbacks.Visibility = Visibility.Collapsed;

            _input.ProcessHeat_W      = ParseDoubleUI(TxtProcessHeat.Text);
            _input.ProcessAnnualHours = ParseDoubleUI(TxtProcessHours.Text);

            var inp = BuildAggregatorInput();

            if (inp.A_use_m2 <= 0)
            {
                ShowWarning("A_use (отопляема площ) = 0. Моля попълнете Секция 5.");
                return;
            }

            var result = InternalGainsAggregator.Compute(inp);
            PopulateResults(result, inp);
            PanelResults.Visibility = Visibility.Visible;
        }

        private InternalGainsAggregatorInput BuildAggregatorInput()
        {
            var inp = new InternalGainsAggregatorInput();

            inp.A_use_m2 = _objData != null ? ParseDouble(_objData.HeatedArea) : 0;

            if (_objData != null)
            {
                int zone = Math.Clamp(_objData.ClimateZone, 1, 9);
                var hs = HeatingSeason[zone - 1];
                inp.HeatingStartMonth  = hs.sm;
                inp.HeatingStartDay    = hs.sd;
                inp.HeatingEndMonth    = hs.em;
                inp.HeatingEndDay      = hs.ed;
                inp.HeatingHoursPerDay = 24.0;  // за SeasonMask (дни) – реалните часове се задават по-долу

                // График на обитаване – отопление (от Секция 5, ред "График на обитаване")
                double occWd_H  = ParseDouble(_objData.OccupancyWorkdaysHours);
                double occSat_H = ParseDouble(_objData.OccupancySaturdayHours);
                double occSun_H = ParseDouble(_objData.OccupancySundayHours);
                inp.Occupancy_HeatingWorkdaysH  = occWd_H;
                inp.Occupancy_HeatingSaturdayH  = occSat_H;
                inp.Occupancy_HeatingSundayH    = occSun_H;

                // Почивни дни по месеци (от Секция 5 – таблицата "Дни почивни по месеци")
                inp.DaysOffPerMonth = new double[12]
                {
                    ParseDouble(_objData.DaysOffJanuary),
                    ParseDouble(_objData.DaysOffFebruary),
                    ParseDouble(_objData.DaysOffMarch),
                    ParseDouble(_objData.DaysOffApril),
                    ParseDouble(_objData.DaysOffMay),
                    ParseDouble(_objData.DaysOffJune),
                    ParseDouble(_objData.DaysOffJuly),
                    ParseDouble(_objData.DaysOffAugust),
                    ParseDouble(_objData.DaysOffSeptember),
                    ParseDouble(_objData.DaysOffOctober),
                    ParseDouble(_objData.DaysOffNovember),
                    ParseDouble(_objData.DaysOffDecember),
                };

                if (_objData.CoolingSeasonEnabled &&
                    _objData.CoolingSeasonStartMonth.HasValue &&
                    _objData.CoolingSeasonEndMonth.HasValue)
                {
                    inp.CoolingStartMonth  = _objData.CoolingSeasonStartMonth;
                    inp.CoolingStartDay    = _objData.CoolingSeasonStartDay ?? 1;
                    inp.CoolingEndMonth    = _objData.CoolingSeasonEndMonth;
                    inp.CoolingEndDay      = _objData.CoolingSeasonEndDay
                        ?? DateTime.DaysInMonth(inp.YearRef, _objData.CoolingSeasonEndMonth.Value);
                    inp.CoolingHoursPerDay = 24.0;  // за SeasonMask

                    // График на обитаване – охлаждане (от Секция 5, "График на обитаване" охладителен период)
                    var occCoolSched = _objData.CoolingSchedules?.OccupancyCoolingSchedule;
                    if (occCoolSched != null)
                    {
                        inp.Occupancy_CoolingWorkdaysH  = occCoolSched.Workdays.GetHours();
                        inp.Occupancy_CoolingSaturdayH  = occCoolSched.Saturday.GetHours();
                        inp.Occupancy_CoolingSundayH    = occCoolSched.Sunday.GetHours();
                    }
                    else
                    {
                        // Ако няма cooling occupancy график – ползваме heating стойностите
                        inp.Occupancy_CoolingWorkdaysH  = occWd_H;
                        inp.Occupancy_CoolingSaturdayH  = occSat_H;
                        inp.Occupancy_CoolingSundayH    = occSun_H;
                    }
                }
            }

            inp.NumberOfOccupants = ParseInt(_objData?.NumberOfOccupants);
            var (phiH, phiC) = GetSensibleHeatValues();
            inp.OccupantsSensibleHeat_H_W = phiH;
            inp.OccupantsSensibleHeat_C_W = phiC;

            var appData = GetSection(SectionType.AppliancesAffecting)?.AppliancesAffectingSectionData;
            if (appData != null)
            {
                // TotalPower_kW is computed from LineItems (no area dependency)
                // Do NOT use SimultaneousPower_W – it requires SetHeatedArea() to have been called
                double appPower_W = appData.TotalPower_kW * 1000.0;
                inp.Appliances_TotalPower_W          = appPower_W;
                inp.Appliances_TotalAnnualEnergy_kWh = appData.TotalAnnualEnergy_kWh;
                inp.Appliances_AnnualOperatingHours  = appPower_W > 1e-9 && appData.TotalAnnualEnergy_kWh > 1e-9
                    ? appData.TotalAnnualEnergy_kWh / (appPower_W / 1000.0)
                    : 0;
            }

            var lightData = GetSection(SectionType.Lighting)?.LightingSectionData;
            if (lightData != null)
            {
                // TotalPower_kW is computed from LineItems (no area dependency)
                double lightPower_W = lightData.TotalPower_kW * 1000.0;
                inp.Lighting_TotalPower_W          = lightPower_W;
                inp.Lighting_TotalAnnualEnergy_kWh = lightData.TotalAnnualEnergy_kWh;
                inp.Lighting_AnnualOperatingHours  = lightPower_W > 1e-9 && lightData.TotalAnnualEnergy_kWh > 1e-9
                    ? lightData.TotalAnnualEnergy_kWh / (lightPower_W / 1000.0)
                    : 0;
            }

            var hwData = GetSection(SectionType.HotWater)?.HotWaterSectionData;
            if (hwData != null)
                inp.WaterSystem_RecoverableHeat_kWh_Annual = hwData.EffectiveRecoverableHeat_kWh;

            var pfData = GetSection(SectionType.PumpsAndFans)?.PumpsAndFansSectionData;
            if (pfData != null)
            {
                inp.HVAC_HeatingTotalPower_W          = SumPower(pfData.HeatingRows);
                inp.HVAC_HeatingAnnualHours           = pfData.HeatingAnnualHours;
                inp.HVAC_HeatingAnnualConsumption_kWh = pfData.HeatingTotalAnnualConsumption;

                inp.HVAC_CoolingTotalPower_W          = SumPower(pfData.CoolingRows);
                inp.HVAC_CoolingAnnualHours           = pfData.CoolingAnnualHours;
                inp.HVAC_CoolingAnnualConsumption_kWh = pfData.CoolingTotalAnnualConsumption;
            }

            inp.ProcessHeat_W      = _input.ProcessHeat_W;
            inp.ProcessAnnualHours = _input.ProcessAnnualHours;

            return inp;
        }

        private void PopulateResults(InternalGainsAggregatorResult result, InternalGainsAggregatorInput inp)
        {
            int zone = _objData != null ? Math.Clamp(_objData.ClimateZone, 1, 9) : 1;
            var hs = HeatingSeason[zone - 1];
            TxtHeatSeasonInfo.Text = $"Период: {hs.sd:D2}.{hs.sm:D2} - {hs.ed:D2}.{hs.em:D2}   A_use = {result.A_use_m2:F1} m2";
            TxtCoolSeasonInfo.Text = inp.CoolingStartMonth.HasValue
                ? $"Период: {inp.CoolingStartDay:D2}.{inp.CoolingStartMonth:D2} - {inp.CoolingEndDay:D2}.{inp.CoolingEndMonth:D2}   A_use = {result.A_use_m2:F1} m2"
                : "Охладителен сезон не е зададен - таблицата е нула.";

            var heatRows = result.HeatingTable
                .Select((r, i) => new MonthlyGainsRowDisplay(r, MonthNames[i]))
                .ToList();
            DgHeating.ItemsSource = heatRows;

            double hTotal    = result.HeatingTable.Sum(r => r.Total);
            double hSpecific = result.A_use_m2 > 1e-9 ? hTotal / result.A_use_m2 : 0;
            TxtHeatTotal.Text      = $"{hTotal:F2} kWh";
            TxtHeatTotalPerM2.Text = $"{hSpecific:F4} kWh/m2";

            var coolRows = result.CoolingTable
                .Select((r, i) => new MonthlyGainsRowDisplay(r, MonthNames[i]))
                .ToList();
            DgCooling.ItemsSource = coolRows;

            double cTotal    = result.CoolingTable.Sum(r => r.Total);
            double cSpecific = result.A_use_m2 > 1e-9 ? cTotal / result.A_use_m2 : 0;
            TxtCoolTotal.Text      = $"{cTotal:F2} kWh";
            TxtCoolTotalPerM2.Text = $"{cSpecific:F4} kWh/m2";

            var notes = BuildNotes(inp);
            if (notes.Count > 0)
            {
                TxtFallbacks.Text = string.Join("\n", notes.Select((n, i) => $"  {i + 1}. {n}"));
                PanelFallbacks.Visibility = Visibility.Visible;
            }
        }

        private (double phiH, double phiC) GetSensibleHeatValues()
        {
            double phiH = 70.0;

            if (_report != null)
            {
                var heatSection = GetSection(SectionType.Heating);
                if (heatSection?.HeatingSectionData != null)
                {
                    phiH = GetSensibleHeatFromActivity(
                        heatSection.HeatingSectionData.SelectedActivityLevel,
                        isHeating: true);
                }
            }

            double phiC = Math.Max(phiH * 0.85, 55.0);
            return (phiH, phiC);
        }

        private static double GetSensibleHeatFromActivity(ActivityLevel level, bool isHeating)
        {
            return level switch
            {
                ActivityLevel.Cinema               => isHeating ? 75  : 60,
                ActivityLevel.Office               => isHeating ? 75  : 65,
                ActivityLevel.HotelReceptionKasier => isHeating ? 80  : 70,
                ActivityLevel.StandingLightWork    => isHeating ? 90  : 75,
                ActivityLevel.WalkingSeated        => isHeating ? 100 : 85,
                ActivityLevel.ModerateWork         => isHeating ? 110 : 95,
                ActivityLevel.LightWorkSeated      => isHeating ? 105 : 90,
                ActivityLevel.Dancing              => isHeating ? 140 : 120,
                ActivityLevel.FastWalking          => isHeating ? 165 : 150,
                ActivityLevel.HeavyWork            => isHeating ? 210 : 185,
                _                                  => 70
            };
        }

        private Section? GetSection(SectionType type)
            => _report?.Sections?.FirstOrDefault(s => s.Type == type);

        private static double SumPower(System.Collections.IEnumerable rows)
        {
            double total = 0;
            if (rows == null) return total;
            foreach (var r in rows)
            {
                if (r == null) continue;
                double pw  = GetDoubleProperty(r, "NominalPower");
                double qty = GetDoubleProperty(r, "Quantity");
                if (qty < 1e-9) qty = 1.0;
                total += pw * qty;
            }
            return total;
        }

        private static double GetDoubleProperty(object obj, string propName)
        {
            var prop = obj.GetType().GetProperty(propName);
            if (prop == null) return 0;
            var raw = prop.GetValue(obj)?.ToString()?.Replace(",", ".");
            return double.TryParse(raw,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double v) ? v : 0;
        }

        private static List<string> BuildNotes(InternalGainsAggregatorInput inp)
        {
            var notes = new List<string>();
            if (inp.OccupantsSensibleHeat_H_W <= 0)
                notes.Add("Phi_sens,H = 0 - обитателите не допринасят. Проверете Секция Отопление.");
            if (inp.Appliances_TotalPower_W <= 0 && inp.Appliances_TotalAnnualEnergy_kWh <= 0)
                notes.Add("Уреди: нулева мощност/енергия - добавете данни в Секция 18.");
            if (inp.Lighting_TotalPower_W <= 0 && inp.Lighting_TotalAnnualEnergy_kWh <= 0)
                notes.Add("Осветление: нулева мощност/енергия - добавете данни в Секция 17.");
            if (inp.HVAC_HeatingAnnualConsumption_kWh <= 0 && inp.HVAC_HeatingTotalPower_W <= 0)
                notes.Add("HVAC отопление: няма данни в Секция 15 или Q_HVAC_H = 0.");
            if (inp.WaterSystem_RecoverableHeat_kWh_Annual <= 0)
                notes.Add("WA: регенерируеми загуби = 0 - попълнете полето в Секция 16 ако е приложимо.");
            return notes;
        }

        private void ShowWarning(string msg)
        {
            TxtErrorsTitle.Text    = "Предупреждение:";
            TxtErrors.Text         = msg;
            PanelErrors.Background = System.Windows.Media.Brushes.LightYellow;
            PanelErrors.Visibility = Visibility.Visible;
        }

        private static string ParseAreaDisplay(string? val)
        {
            if (string.IsNullOrWhiteSpace(val)) return "--";
            return double.TryParse(val.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double d)
                ? $"{d:F1} m2" : val;
        }

        private static string FormatSchedule(string? wd, string? sat, string? sun)
        {
            string F(string? v) => string.IsNullOrWhiteSpace(v) || v.Trim() == "0" ? "0" : v!.Trim();
            return $"{F(wd)} / {F(sat)} / {F(sun)} h/den";
        }

        private static double ParseDouble(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            return double.TryParse(text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double v) ? v : 0;
        }

        private static double ParseDoubleUI(string? text)
        {
            var t = text?.Trim().Replace(",", ".") ?? "0";
            return double.TryParse(t,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double v) ? v : 0;
        }

        private static int ParseInt(string? text)
            => int.TryParse(text?.Trim(), out int v) ? v : 0;
    }

    internal sealed class MonthlyGainsRowDisplay
    {
        private readonly MonthlyGainsRow _row;
        public string MonthName  { get; }
        public double Oc         => _row.Oc;
        public double A          => _row.A;
        public double L          => _row.L;
        public double WA         => _row.WA;
        public double HVAC       => _row.HVAC;
        public double Proc       => _row.Proc;
        public double Total      => _row.Total;
        public double TotalPerM2 => _row.TotalPerM2;

        public MonthlyGainsRowDisplay(MonthlyGainsRow row, string monthName)
        {
            _row      = row;
            MonthName = monthName;
        }
    }
}