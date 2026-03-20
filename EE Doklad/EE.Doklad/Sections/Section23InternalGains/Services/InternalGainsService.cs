using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.Sections.Section23InternalGains.Services;

/// <summary>
/// Shared access point for Section 23 calculations so downstream sections can reuse
/// the same aggregator without depending on the Section 23 UI.
/// </summary>
public class InternalGainsService
{
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

    private readonly InternalGainsDebugInput _input;
    private readonly ObjectDataSectionData? _objectData;
    private readonly Report? _report;
    private InternalGainsAggregatorResult? _lastResult;

    public InternalGainsService(
        InternalGainsDebugInput input,
        ObjectDataSectionData? objectData = null,
        Report? report = null)
    {
        _input = input;
        _objectData = objectData;
        _report = report;
    }

    public InternalGainsAggregatorResult? LastResult => _lastResult;

    /// <summary>
    /// Ensures A_use_m2 is current when downstream sections recalculate before
    /// the ObjectData UI has pushed the heated-area text back into the model.
    /// </summary>
    public void UpdateArea(double area)
    {
        if (_objectData != null && area > 0)
        {
            _objectData.HeatedArea = area.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    public InternalGainsAggregatorResult? Recalculate(bool persist = true)
    {
        var aggregatorInput = BuildAggregatorInput();
        if (aggregatorInput.A_use_m2 <= 0)
        {
            if (persist)
            {
                _input.HeatingMonths.Clear();
                _input.CoolingMonths.Clear();
            }
            _lastResult = null;
            return null;
        }

        _lastResult = InternalGainsAggregator.Compute(aggregatorInput);
        if (persist)
        {
            StoreMonthlyResults(_lastResult);
        }
        return _lastResult;
    }

    public double GetMonthlyTotal_kWh(int monthIndex)
    {
        if (_input.HeatingMonths.Count == 0)
        {
            Recalculate();
        }

        int month = monthIndex + 1;
        return _input.HeatingMonths.FirstOrDefault(m => m.Month == month)?.Total_kWh ?? 0.0;
    }

    private InternalGainsAggregatorInput BuildAggregatorInput()
    {
        var input = new InternalGainsAggregatorInput
        {
            A_use_m2 = _objectData != null ? ParseDouble(_objectData.HeatedArea) : 0.0
        };

        if (_objectData != null)
        {
            int zone = Math.Clamp(_objectData.ClimateZone, 1, 9);
            var heatingSeason = HeatingSeason[zone - 1];
            input.HeatingStartMonth = heatingSeason.sm;
            input.HeatingStartDay = heatingSeason.sd;
            input.HeatingEndMonth = heatingSeason.em;
            input.HeatingEndDay = heatingSeason.ed;
            input.HeatingHoursPerDay = 24.0;

            double occWd = ParseDouble(_objectData.OccupancyWorkdaysHours);
            double occSat = ParseDouble(_objectData.OccupancySaturdayHours);
            double occSun = ParseDouble(_objectData.OccupancySundayHours);
            input.Occupancy_HeatingWorkdaysH = occWd;
            input.Occupancy_HeatingSaturdayH = occSat;
            input.Occupancy_HeatingSundayH = occSun;

            input.DaysOffPerMonth = new double[12]
            {
                ParseDouble(_objectData.DaysOffJanuary),
                ParseDouble(_objectData.DaysOffFebruary),
                ParseDouble(_objectData.DaysOffMarch),
                ParseDouble(_objectData.DaysOffApril),
                ParseDouble(_objectData.DaysOffMay),
                ParseDouble(_objectData.DaysOffJune),
                ParseDouble(_objectData.DaysOffJuly),
                ParseDouble(_objectData.DaysOffAugust),
                ParseDouble(_objectData.DaysOffSeptember),
                ParseDouble(_objectData.DaysOffOctober),
                ParseDouble(_objectData.DaysOffNovember),
                ParseDouble(_objectData.DaysOffDecember),
            };

            if (_objectData.CoolingSeasonEnabled &&
                _objectData.CoolingSeasonStartMonth.HasValue &&
                _objectData.CoolingSeasonEndMonth.HasValue)
            {
                input.CoolingStartMonth = _objectData.CoolingSeasonStartMonth;
                input.CoolingStartDay = _objectData.CoolingSeasonStartDay ?? 1;
                input.CoolingEndMonth = _objectData.CoolingSeasonEndMonth;
                input.CoolingEndDay = _objectData.CoolingSeasonEndDay
                    ?? DateTime.DaysInMonth(input.YearRef, _objectData.CoolingSeasonEndMonth.Value);
                input.CoolingHoursPerDay = 24.0;

                var coolingSchedule = _objectData.CoolingSchedules?.OccupancyCoolingSchedule;
                if (coolingSchedule != null)
                {
                    input.Occupancy_CoolingWorkdaysH = coolingSchedule.Workdays.GetHours();
                    input.Occupancy_CoolingSaturdayH = coolingSchedule.Saturday.GetHours();
                    input.Occupancy_CoolingSundayH = coolingSchedule.Sunday.GetHours();
                }
                else
                {
                    input.Occupancy_CoolingWorkdaysH = occWd;
                    input.Occupancy_CoolingSaturdayH = occSat;
                    input.Occupancy_CoolingSundayH = occSun;
                }
            }
        }

        input.NumberOfOccupants = ParseInt(_objectData?.NumberOfOccupants);
        var (phiH, phiC) = GetSensibleHeatValues();
        input.OccupantsSensibleHeat_H_W = phiH;
        input.OccupantsSensibleHeat_C_W = phiC;

        var appliances = GetSection(SectionType.AppliancesAffecting)?.AppliancesAffectingSectionData;
        if (appliances != null)
        {
            double powerW = appliances.TotalPower_kW * 1000.0;
            input.Appliances_TotalPower_W = powerW;
            input.Appliances_TotalAnnualEnergy_kWh = appliances.TotalAnnualEnergy_kWh;
            input.Appliances_AnnualOperatingHours = powerW > 1e-9 && appliances.TotalAnnualEnergy_kWh > 1e-9
                ? appliances.TotalAnnualEnergy_kWh / (powerW / 1000.0)
                : 0.0;
        }

        var lighting = GetSection(SectionType.Lighting)?.LightingSectionData;
        if (lighting != null)
        {
            double powerW = lighting.TotalPower_kW * 1000.0;
            input.Lighting_TotalPower_W = powerW;
            input.Lighting_TotalAnnualEnergy_kWh = lighting.TotalAnnualEnergy_kWh;
            input.Lighting_AnnualOperatingHours = powerW > 1e-9 && lighting.TotalAnnualEnergy_kWh > 1e-9
                ? lighting.TotalAnnualEnergy_kWh / (powerW / 1000.0)
                : 0.0;
        }

        var hotWater = GetSection(SectionType.HotWater)?.HotWaterSectionData;
        if (hotWater != null)
        {
            input.WaterSystem_RecoverableHeat_kWh_Annual = hotWater.EffectiveRecoverableHeat_kWh;
        }

        var pumpsAndFans = GetSection(SectionType.PumpsAndFans)?.PumpsAndFansSectionData;
        if (pumpsAndFans != null)
        {
            input.HVAC_HeatingTotalPower_W = SumPower(pumpsAndFans.HeatingRows);
            input.HVAC_HeatingAnnualHours = pumpsAndFans.HeatingAnnualHours;
            input.HVAC_HeatingAnnualConsumption_kWh = pumpsAndFans.HeatingTotalAnnualConsumption;
            input.HVAC_CoolingTotalPower_W = SumPower(pumpsAndFans.CoolingRows);
            input.HVAC_CoolingAnnualHours = pumpsAndFans.CoolingAnnualHours;
            input.HVAC_CoolingAnnualConsumption_kWh = pumpsAndFans.CoolingTotalAnnualConsumption;
        }

        input.ProcessHeat_W = _input.ProcessHeat_W;
        input.ProcessAnnualHours = _input.ProcessAnnualHours;

        return input;
    }

    private (double phiH, double phiC) GetSensibleHeatValues()
    {
        double phiH = 70.0;
        double phiC = 67.0;

        if (_report != null)
        {
            var heatingSection = GetSection(SectionType.Heating);
            if (heatingSection?.HeatingSectionData != null)
            {
                phiH = GetSensibleHeatFromSection(
                    heatingSection.HeatingSectionData.SelectedActivityLevel,
                    heatingSection.HeatingSectionData.DesignTemperature,
                    phiH);
            }

            var coolingSection = GetCoolingSection();
            if (coolingSection?.CoolingSectionData != null)
            {
                phiC = GetSensibleHeatFromSection(
                    coolingSection.CoolingSectionData.SelectedActivityLevel,
                    coolingSection.CoolingSectionData.DesignTemperature,
                    phiC);
            }
        }

        return (phiH, phiC);
    }

    private static double GetSensibleHeatFromSection(ActivityLevel level, double temperature, double fallbackValue)
    {
        var (sensible, _) = ActivityDataService.CalculateHeatForTemperature(level, temperature);
        return sensible > 0 ? sensible : fallbackValue;
    }

    private void StoreMonthlyResults(InternalGainsAggregatorResult result)
    {
        ReplaceMonthlyResults(_input.HeatingMonths, result.HeatingTable);
        ReplaceMonthlyResults(_input.CoolingMonths, result.CoolingTable);
    }

    private static void ReplaceMonthlyResults(
        System.Collections.ObjectModel.ObservableCollection<InternalGainsMonthlyResult> target,
        IEnumerable<MonthlyGainsRow> rows)
    {
        target.Clear();
        foreach (var row in rows.Where(r => Math.Abs(r.Total) > 1e-9 || Math.Abs(r.TotalPerM2) > 1e-9))
        {
            target.Add(new InternalGainsMonthlyResult
            {
                Month = row.Month,
                Oc_kWh = row.Oc,
                A_kWh = row.A,
                L_kWh = row.L,
                WA_kWh = row.WA,
                HVAC_kWh = row.HVAC,
                Proc_kWh = row.Proc,
                Total_kWh = row.Total,
                Total_kWh_m2 = row.TotalPerM2
            });
        }
    }

    private Section? GetSection(SectionType type)
        => _report?.Sections?.FirstOrDefault(s => s.Type == type);

    private Section? GetCoolingSection()
        => _report?.Sections?.FirstOrDefault(s =>
            s.CoolingSectionData != null &&
            (s.Type == SectionType.Normal ||
             (s.Title?.Contains("Охлаждане", StringComparison.OrdinalIgnoreCase) ?? false)));

    private static int ParseInt(string? value)
        => int.TryParse(value?.Trim(), out int result) ? result : 0;

    private static double ParseDouble(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return 0.0;
        }

        string normalized = value.Replace(',', '.');
        return double.TryParse(normalized, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double result)
            ? result
            : 0.0;
    }

    private static double SumPower(IEnumerable rows)
    {
        double total = 0.0;
        foreach (var row in rows)
        {
            if (row == null)
            {
                continue;
            }

            total += GetDoubleProperty(row, "NominalPower");
        }

        return total;
    }

    private static double GetDoubleProperty(object source, string propertyName)
    {
        var property = source.GetType().GetProperty(propertyName);
        if (property == null)
        {
            return 0.0;
        }

        var value = property.GetValue(source);
        return value is double number ? number : 0.0;
    }
}
