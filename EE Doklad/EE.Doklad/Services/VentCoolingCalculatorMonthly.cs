using System;
using System.Collections.Generic;
using System.Globalization;
using EE.Doklad.Models;

namespace EE.Doklad.Services
{
    /// <summary>
    /// Calculator for Section 14 - Ventilation Cooling (monthly).
    /// </summary>
    public sealed class VentCoolingCalculatorMonthly
    {
        private static readonly string[] MonthNames =
        {
            "Януари", "Февруари", "Март", "Април", "Май", "Юни",
            "Юли", "Август", "Септември", "Октомври", "Ноември", "Декември"
        };

        private const double AirDensity_kg_m3 = 1.2; // rho
        private const double AirSpecificHeat_kJ_kgK = 1.005; // cp
        private const double StandardPressure_Pa = 101325.0;

        public VentilationCoolingCalculationOutput Calculate(
            VentilationSectionData data,
            ObjectDataSectionData? objectData,
            ClimateZoneData? climateData,
            CoolingSectionData? coolingData,
            int yearRef = 2024)
        {
            var output = new VentilationCoolingCalculationOutput();
            var result = output.Result;
            var debug = output.Debug;

            if (data == null)
            {
                result.IsValid = false;
                result.ErrorMessage = "Липсват входни данни за секция 14.";
                return output;
            }

            if (objectData == null)
            {
                result.IsValid = false;
                result.ErrorMessage = "Липсват данни от секция 5.";
                return output;
            }

            if (climateData == null)
            {
                result.IsValid = false;
                result.ErrorMessage = "Липсват климатични данни.";
                return output;
            }

            result.ClimateZoneId = objectData.ClimateZone;
            result.ClimateZoneName = climateData.Name;
            debug.ClimateZoneName = climateData.Name;

            result.CoolingSeasonEnabled = objectData.CoolingSeasonEnabled;
            debug.SeasonEnabled = objectData.CoolingSeasonEnabled;

            if (!objectData.CoolingSeasonEnabled)
            {
                result.IsValid = true;
                result.ErrorMessage = null;
                debug.HolidaysSourceNote = "Охладителният сезон не е активен.";
                return output;
            }

            if (!TryBuildSeasonRange(objectData, yearRef, out var seasonStart, out var seasonEnd))
            {
                result.IsValid = false;
                result.ErrorMessage = "Невалидни дати за охладителния сезон.";
                return output;
            }

            result.SeasonStart = seasonStart;
            result.SeasonEnd = seasonEnd;
            debug.SeasonStart = seasonStart;
            debug.SeasonEnd = seasonEnd;

            var area = ParseDouble(objectData.CooledArea);
            if (area <= 0)
            {
                result.IsValid = false;
                result.ErrorMessage = "Охлаждаемата площ трябва да бъде положителна.";
                return output;
            }

            result.CooledArea_m2 = area;
            debug.AreaCooled_m2 = area;

            result.AirflowRatePerM2 = data.AirflowRatePerM2;
            result.SupplyTemperature_C = data.SupplyTemperature;
            result.SupplyRelativeHumidityPercent = data.RelativeHumidity;
            debug.AirflowRatePerM2 = data.AirflowRatePerM2;
            debug.SupplyTemperature_C = data.SupplyTemperature;
            debug.SupplyRH_percent = data.RelativeHumidity;

            var workdayHours = ParseDouble(objectData.VentilationCoolingWorkdaysHours);
            var saturdayHours = ParseDouble(objectData.VentilationCoolingSaturdayHours);
            var sundayHours = ParseDouble(objectData.VentilationCoolingSundayHours);
            // If the user provided an explicit operating hours per week in section data (similar to Heating),
            // prefer that value and compute monthly hours from it. This makes cooling behavior consistent
            // with the heating calculator when the user wants to override schedule-derived hours.
            bool useOperatingHoursPerWeek = data.OperatingHoursPerWeek > 0.0;
            double hoursPerDayOverride = useOperatingHoursPerWeek ? data.OperatingHoursPerWeek / 7.0 : 0.0;
            debug.WorkdayHours = workdayHours;
            debug.SaturdayHours = saturdayHours;
            debug.SundayHours = sundayHours;

            var monthlyDaysOff = BuildMonthlyDaysOff(objectData);
            var monthlyHolidays = BuildMonthlyOfficialHolidays(yearRef);
            debug.HolidaysSourceNote = monthlyHolidays is null ? "Официални празници: няма отделен списък (използват се само Дни почивни)." : "Официални празници: отделен списък.";

            double qv_spec = Math.Max(0.0, data.AirflowRatePerM2);
            double qv_total = qv_spec * area;
            double m_dot = AirDensity_kg_m3 * qv_total;
            debug.AirflowTotal_m3h = qv_total;
            debug.MassFlow_kg_h = m_dot;

            debug.Mode = data.CoolingCalculationMode;
            debug.RecirculationPercent = Math.Clamp(data.RecirculationPercent, 0.0, 100.0);

            double t_in = coolingData?.DesignTemperature ?? 24.0;
            double rh_in = 50.0;
            debug.T_in_C = t_in;
            debug.RH_in_percent = rh_in;
            debug.RH_in_assumed = true;

            double totalSensibleCool_kWh = 0.0;
            double totalSensibleHeat_kWh = 0.0;
            double totalLatent_kWh = 0.0;
            double totalWorkdays = 0.0;
            double totalHours = 0.0;
            int totalSeasonDays = 0;

            for (int m = 0; m < 12; m++)
            {
                int monthNumber = m + 1;
                int daysInMonth = DateTime.DaysInMonth(yearRef, monthNumber);

                int seasonDays = CountSeasonDaysInMonth(yearRef, monthNumber, seasonStart, seasonEnd);
                if (seasonDays <= 0)
                {
                    continue;
                }

                totalSeasonDays += seasonDays;
                var (weekdayCount, saturdayCount, sundayCount) = CountSeasonDayTypes(yearRef, monthNumber, seasonStart, seasonEnd);

                int restDays = monthlyDaysOff[m];
                int holidayDays = monthlyHolidays != null ? monthlyHolidays[m] : 0;

                // declare common vars used downstream (debug and energy calcs)
                double hours_m = 0.0;
                double workdays = 0.0;
                double workdaysWeekday = 0.0;
                double workdaysSaturday = 0.0;
                double workdaysSunday = 0.0;

                if (useOperatingHoursPerWeek)
                {
                    // Follow heating-style calculation: use user-entered OperatingHoursPerWeek -> hours per day
                    // and multiply by the number of season days in the month after subtracting days-off/holidays.
                    int adjustedDays = Math.Max(0, seasonDays - restDays - holidayDays);
                    hours_m = hoursPerDayOverride * adjustedDays;
                    workdays = adjustedDays;
                    workdaysWeekday = adjustedDays; // heating-style: don't split by type in the UI breakdown
                    workdaysSaturday = 0.0;
                    workdaysSunday = 0.0;

                    totalWorkdays += workdays;
                    totalHours += hours_m;
                }
                else
                {
                    // Candidate days are only those day-types that have scheduled hours configured.
                    // This prevents weekends from being counted as working days when Saturday/Sunday
                    // hours are left blank/zero in the Building Schedules (секция 5).
                    int candidateDays = 0;
                    if (workdayHours > 0.0) candidateDays += weekdayCount;
                    if (saturdayHours > 0.0) candidateDays += saturdayCount;
                    if (sundayHours > 0.0) candidateDays += sundayCount;

                    workdays = Math.Max(0.0, candidateDays - restDays - holidayDays);
                    totalWorkdays += workdays;

                    double ratio = candidateDays > 0 ? workdays / candidateDays : 0.0;

                    workdaysWeekday = weekdayCount * ratio;
                    workdaysSaturday = saturdayCount * ratio;
                    workdaysSunday = sundayCount * ratio;

                    hours_m = workdaysWeekday * workdayHours + workdaysSaturday * saturdayHours + workdaysSunday * sundayHours;
                    totalHours += hours_m;
                }

                double te_m = climateData.Monthly.AvgMonthlyTempC[m];
                double? rh_m = GetRhForMonth(climateData, monthNumber);
                bool hasRh = rh_m.HasValue;

                double qv_fresh = qv_total;
                double qv_rec = 0.0;
                double h_in = 0.0;
                double h_mix = 0.0;
                double t_mix = te_m;

                if (data.CoolingCalculationMode == VentilationCoolingCalculationMode.MechanicalRecirculation3112)
                {
                    double recircPct = Math.Clamp(data.RecirculationPercent, 0.0, 100.0) / 100.0;
                    qv_fresh = qv_total * (1.0 - recircPct);
                    qv_rec = qv_total - qv_fresh;
                    t_mix = (qv_fresh * te_m + qv_rec * t_in) / Math.Max(1e-6, qv_total);

                    if (hasRh)
                    {
                        h_in = ComputeEnthalpy(t_in, rh_in);
                        double h_e = ComputeEnthalpy(te_m, rh_m!.Value);
                        h_mix = (qv_fresh * h_e + qv_rec * h_in) / Math.Max(1e-6, qv_total);
                    }
                }

                double deltaT_cool = Math.Max(0.0, t_mix - data.SupplyTemperature);
                double deltaT_heat = Math.Max(0.0, data.SupplyTemperature - t_mix);

                double p_sens_cool = m_dot * AirSpecificHeat_kJ_kgK * deltaT_cool; // kJ/h
                double p_sens_heat = m_dot * AirSpecificHeat_kJ_kgK * deltaT_heat; // kJ/h
                double e_sens_cool = p_sens_cool * hours_m / 3600.0; // kWh
                double e_sens_heat = p_sens_heat * hours_m / 3600.0; // kWh

                double h_e_m = 0.0;
                double h_sup = 0.0;
                double deltaH = 0.0;
                double e_tot = 0.0;
                double e_lat = 0.0;

                if (hasRh)
                {
                    h_e_m = ComputeEnthalpy(te_m, rh_m!.Value);
                    h_sup = ComputeEnthalpy(data.SupplyTemperature, data.RelativeHumidity);

                    double h_source = data.CoolingCalculationMode == VentilationCoolingCalculationMode.MechanicalRecirculation3112 ? h_mix : h_e_m;
                    deltaH = Math.Max(0.0, h_source - h_sup);

                    double p_tot = m_dot * deltaH; // kJ/h
                    e_tot = p_tot * hours_m / 3600.0; // kWh
                    e_lat = Math.Max(0.0, e_tot - e_sens_cool);
                }

                totalSensibleCool_kWh += e_sens_cool;
                totalSensibleHeat_kWh += e_sens_heat;
                totalLatent_kWh += e_lat;

                result.MonthlyResults.Add(new VentilationCoolingMonthlyResult
                {
                    MonthNumber = monthNumber,
                    MonthName = MonthNames[m],
                    OutdoorTemperature_C = te_m,
                    OutdoorRelativeHumidityPercent = rh_m,
                    HasHumidityData = hasRh,
                    WorkingHours_h = hours_m,
                    WorkingDays = workdays,
                    WorkingDaysWeekday = workdaysWeekday,
                    WorkingDaysSaturday = workdaysSaturday,
                    WorkingDaysSunday = workdaysSunday,
                    SensibleCoolingEnergy_kWh = e_sens_cool,
                    SensibleHeatingEnergy_kWh = e_sens_heat,
                    TotalCoolingEnergy_kWh = e_tot,
                    LatentEnergy_kWh = e_lat
                });

                debug.Months.Add(new VentilationCoolingMonthlyDebug
                {
                    MonthNumber = monthNumber,
                    MonthName = MonthNames[m],
                    DaysInSeason = seasonDays,
                    RestDays = restDays,
                    Holidays = holidayDays,
                    WorkingDays = workdays,
                    WorkdaysWeekday = workdaysWeekday,
                    WorkdaysSaturday = workdaysSaturday,
                    WorkdaysSunday = workdaysSunday,
                    WorkingHours_h = hours_m,
                    Te_m_C = te_m,
                    RH_m_percent = rh_m,
                    HasRH = hasRh,
                    h_e_kJkg = h_e_m,
                    h_sup_kJkg = h_sup,
                    DeltaH_kJkg = deltaH,
                    SensibleCooling_kWh = e_sens_cool,
                    SensibleHeating_kWh = e_sens_heat,
                    TotalCooling_kWh = e_tot,
                    Latent_kWh = e_lat,
                    qv_fresh_m3h = qv_fresh,
                    qv_rec_m3h = qv_rec,
                    h_in_kJkg = h_in,
                    h_mix_kJkg = h_mix,
                    T_mix_C = t_mix
                });
            }

            result.TotalWorkingDays = totalWorkdays;
            result.TotalWorkingHours = totalHours;
            debug.TotalWorkdays = totalWorkdays;
            debug.TotalHours = totalHours;

            double weeksInSeason = totalSeasonDays / 7.0;
            result.OperatingHoursPerWeek = weeksInSeason > 0 ? totalHours / weeksInSeason : 0.0;

            result.SensibleCoolingEnergy_kWh = totalSensibleCool_kWh;
            result.SensibleCoolingEnergy_kWh_m2 = totalSensibleCool_kWh / area;
            result.SensibleHeatingEnergy_kWh = totalSensibleHeat_kWh;
            result.SensibleHeatingEnergy_kWh_m2 = totalSensibleHeat_kWh / area;
            result.LatentEnergy_kWh = totalLatent_kWh;
            result.LatentEnergy_kWh_m2 = totalLatent_kWh / area;

            result.NetCoolingContribution_kWh = totalSensibleCool_kWh + totalLatent_kWh;
            result.NetCoolingContribution_kWh_m2 = result.NetCoolingContribution_kWh / area;

            debug.SensibleCooling_kWh_m2 = result.SensibleCoolingEnergy_kWh_m2;
            debug.SensibleHeating_kWh_m2 = result.SensibleHeatingEnergy_kWh_m2;
            debug.Latent_kWh_m2 = result.LatentEnergy_kWh_m2;
            debug.NetCoolingContribution_kWh_m2 = result.NetCoolingContribution_kWh_m2;

            double netEnergyTotal = totalSensibleCool_kWh + totalSensibleHeat_kWh + totalLatent_kWh;
            result.NetEnergyTotal_kWh = netEnergyTotal;
            debug.NetEnergyTotal_kWh_m2 = netEnergyTotal / area;

            ApplyEnergySources(data, netEnergyTotal, area, result, debug);
            result.IsValid = true;
            return output;
        }

        private void ApplyEnergySources(
            VentilationSectionData data,
            double netEnergyTotal_kWh,
            double area,
            VentilationCoolingCalculationResult result,
            VentilationCoolingDebugInfo debug)
        {
            if (netEnergyTotal_kWh <= 0)
            {
                result.FinalEnergySource1_kWh = 0;
                result.FinalEnergySource2_kWh = 0;
                result.TotalFinalEnergy_kWh = 0;
                result.SpecificFinalEnergy_kWh_m2 = 0;
                debug.CombinedEfficiency1 = data.EnergySource1.TotalEfficiency;
                debug.CombinedEfficiency2 = data.EnergySource2?.TotalEfficiency ?? 0;
                return;
            }

            double share1 = data.EnergySource1.Share / 100.0;
            double eff1 = data.EnergySource1.TotalEfficiency;
            debug.CombinedEfficiency1 = eff1;

            double need1 = eff1 > 0 ? (netEnergyTotal_kWh * share1) / eff1 : 0.0;
            result.FinalEnergySource1_kWh = need1;
            debug.NeedEnergy1_kWh = need1;

            double need2 = 0.0;
            if (data.UseSecondEnergySource && data.EnergySource2 != null)
            {
                double share2 = data.EnergySource2.Share / 100.0;
                double eff2 = data.EnergySource2.TotalEfficiency;
                debug.CombinedEfficiency2 = eff2;
                need2 = eff2 > 0 ? (netEnergyTotal_kWh * share2) / eff2 : 0.0;
                result.FinalEnergySource2_kWh = need2;
                debug.NeedEnergy2_kWh = need2;
            }

            result.TotalFinalEnergy_kWh = need1 + need2;
            result.SpecificFinalEnergy_kWh_m2 = area > 0 ? result.TotalFinalEnergy_kWh / area : 0.0;
        }

        private static double? GetRhForMonth(ClimateZoneData climateData, int monthNumber)
        {
            // RH array is May..Sep (5 values)
            if (monthNumber < 5 || monthNumber > 9)
            {
                return null;
            }

            int index = monthNumber - 5;
            if (climateData.Monthly.AvgMonthlyRelHumidityPercentMayToSep == null ||
                climateData.Monthly.AvgMonthlyRelHumidityPercentMayToSep.Length <= index)
            {
                return null;
            }

            return climateData.Monthly.AvgMonthlyRelHumidityPercentMayToSep[index];
        }

        private static int[] BuildMonthlyDaysOff(ObjectDataSectionData objectData)
        {
            int Parse(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return 0;
                if (int.TryParse(s.Trim(), out var v)) return Math.Max(0, v);
                return 0;
            }

            return new[]
            {
                Parse(objectData.DaysOffJanuary),
                Parse(objectData.DaysOffFebruary),
                Parse(objectData.DaysOffMarch),
                Parse(objectData.DaysOffApril),
                Parse(objectData.DaysOffMay),
                Parse(objectData.DaysOffJune),
                Parse(objectData.DaysOffJuly),
                Parse(objectData.DaysOffAugust),
                Parse(objectData.DaysOffSeptember),
                Parse(objectData.DaysOffOctober),
                Parse(objectData.DaysOffNovember),
                Parse(objectData.DaysOffDecember)
            };
        }

        private static int[]? BuildMonthlyOfficialHolidays(int yearRef)
        {
            _ = yearRef;
            // No explicit holiday list in the project; return null to signal "no extra holidays".
            // This keeps the behavior explicit and allows the debug note to describe the source.
            return null;
        }

        private static double ParseDouble(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0.0;
            if (double.TryParse(s.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v)) return v;
            if (double.TryParse(s.Trim(), out v)) return v;
            return 0.0;
        }

        private static bool TryBuildSeasonRange(ObjectDataSectionData objectData, int yearRef, out DateTime start, out DateTime end)
        {
            start = default;
            end = default;

            if (!objectData.CoolingSeasonStartDay.HasValue || !objectData.CoolingSeasonStartMonth.HasValue ||
                !objectData.CoolingSeasonEndDay.HasValue || !objectData.CoolingSeasonEndMonth.HasValue)
            {
                return false;
            }

            int sm = objectData.CoolingSeasonStartMonth.Value;
            int sd = objectData.CoolingSeasonStartDay.Value;
            int em = objectData.CoolingSeasonEndMonth.Value;
            int ed = objectData.CoolingSeasonEndDay.Value;

            start = new DateTime(yearRef, sm, Math.Min(sd, DateTime.DaysInMonth(yearRef, sm)));
            end = new DateTime(yearRef, em, Math.Min(ed, DateTime.DaysInMonth(yearRef, em)));
            if (end < start)
            {
                end = end.AddYears(1);
            }

            return true;
        }

        private static int CountSeasonDaysInMonth(int yearRef, int monthNumber, DateTime seasonStart, DateTime seasonEnd)
        {
            int daysInMonth = DateTime.DaysInMonth(yearRef, monthNumber);
            int count = 0;
            for (int d = 1; d <= daysInMonth; d++)
            {
                var dt = new DateTime(yearRef, monthNumber, d);
                if (IsInRange(dt, seasonStart, seasonEnd) || IsInRange(dt.AddYears(1), seasonStart, seasonEnd))
                {
                    count++;
                }
            }

            return count;
        }

        private static (int weekday, int saturday, int sunday) CountSeasonDayTypes(int yearRef, int monthNumber, DateTime seasonStart, DateTime seasonEnd)
        {
            int daysInMonth = DateTime.DaysInMonth(yearRef, monthNumber);
            int weekday = 0, saturday = 0, sunday = 0;
            for (int d = 1; d <= daysInMonth; d++)
            {
                var dt = new DateTime(yearRef, monthNumber, d);
                bool inSeason = IsInRange(dt, seasonStart, seasonEnd) || IsInRange(dt.AddYears(1), seasonStart, seasonEnd);
                if (!inSeason) continue;

                switch (dt.DayOfWeek)
                {
                    case DayOfWeek.Saturday: saturday++; break;
                    case DayOfWeek.Sunday: sunday++; break;
                    default: weekday++; break;
                }
            }

            return (weekday, saturday, sunday);
        }

        private static bool IsInRange(DateTime dt, DateTime start, DateTime end)
        {
            return dt >= start && dt <= end;
        }

        private static double ComputeEnthalpy(double tempC, double rhPercent)
        {
            double rh = Math.Clamp(rhPercent, 0.0, 100.0) / 100.0;
            double psat = 610.94 * Math.Exp((17.625 * tempC) / (tempC + 243.04)); // Pa
            double pw = rh * psat;
            double w = 0.62198 * pw / Math.Max(1e-6, (StandardPressure_Pa - pw));
            double h = 1.006 * tempC + w * (2501.0 + 1.86 * tempC); // kJ/kg dry air
            return h;
        }
    }
}
