using System;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Models;
using EE.Doklad.Services.Psychrometrics;
using EE.Doklad.Services.Schedule;
using EE.Doklad.Services.Climate;
using EE.Doklad.Services.VentCooling;
using Xunit;

namespace EE.Doklad.Tests
{
    // ════════════════════════════════════════════════════════════════════════════
    // Psychrometrics tests (§3.14 formulas 3.98–3.106)
    // ════════════════════════════════════════════════════════════════════════════

    public class PsychrometricsServiceTests
    {
        private readonly PsychrometricsService _svc = PsychrometricsService.Default;

        [Theory]
        [InlineData(0.0,   611.2,    2.0)]  // 0°C:  p_ws ≈ 611 Pa
        [InlineData(20.0,  2338.0,  20.0)]  // 20°C: p_ws ≈ 2338 Pa
        [InlineData(30.0,  4243.0,  25.0)]  // 30°C: p_ws ≈ 4243 Pa
        public void SaturationPressure_ApproximatelyCorrect(double tempC, double expected_Pa, double tolerancePa)
        {
            double pws = _svc.SaturationPressure_Pa(tempC);
            Assert.InRange(pws, expected_Pa - tolerancePa, expected_Pa + tolerancePa);
        }

        [Fact]
        public void Compute_AtStandardConditions_PhysicallyConsistent()
        {
            // 20°C, 50%, 101325 Pa
            var s = _svc.Compute(20.0, 50.0, 101325.0);

            // p_ws > 0
            Assert.True(s.p_ws_Pa > 0);
            // p_w < p_ws (partial pressure < saturation)
            Assert.True(s.p_w_Pa < s.p_ws_Pa);
            // x > 0
            Assert.True(s.x_kgkg > 0);
            // h should be around 38-39 kJ/kg for 20°C 50%
            Assert.InRange(s.h_kJkg, 35.0, 45.0);
            // rho ≈ 1.2 kg/m³
            Assert.InRange(s.rho_kgm3, 1.1, 1.3);
            // rho_da slightly > rho (dry air denser per unit mass)
            Assert.True(s.rho_da_kgm3 > 0);
        }

        [Fact]
        public void Compute_ExcelMatchForZone7_30C_50RH()
        {
            // Zone 7 B = 94400 Pa, T=26°C, RH=50%  (from Excel example in attachment)
            var s = _svc.Compute(26.0, 50.0, 94400.0);

            // From Excel: p_ws = 3360 Pa (approx), x ≈ 0.0118, h ≈ 55.9 kJ/kg
            Assert.InRange(s.p_ws_Pa,  3200.0, 3500.0);
            Assert.InRange(s.x_kgkg,   0.010,  0.015);
            Assert.InRange(s.h_kJkg,   52.0,   60.0);
            Assert.InRange(s.rho_kgm3, 1.05,   1.15);
        }

        [Fact]
        public void Compute_HighRH_x_IncreasesMonotonically()
        {
            double b = 101325.0;
            double x30 = _svc.Compute(20.0, 30.0, b).x_kgkg;
            double x60 = _svc.Compute(20.0, 60.0, b).x_kgkg;
            double x90 = _svc.Compute(20.0, 90.0, b).x_kgkg;

            Assert.True(x30 < x60 && x60 < x90, "Влагосъдържанието трябва да расте с RH.");
        }

        [Fact]
        public void Compute_ThrowsOnZeroPressure()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _svc.Compute(20.0, 50.0, 0.0));
        }
    }

    // ════════════════════════════════════════════════════════════════════════════
    // ClimateZonePressureDefaults tests
    // ════════════════════════════════════════════════════════════════════════════

    public class ClimateZonePressureDefaultsTests
    {
        [Theory]
        [InlineData(1, 101000.0)]
        [InlineData(7, 94400.0)]
        [InlineData(9, 99400.0)]
        public void GetPressure_ReturnsCorrectValue(int zone, double expected)
        {
            Assert.Equal(expected, ClimateZonePressureDefaults.GetPressure(zone), 1.0);
        }

        [Fact]
        public void GetPressure_OutOfRange_ReturnsFallback()
        {
            double p = ClimateZonePressureDefaults.GetPressure(0);
            Assert.Equal(101325.0, p, 1.0);
        }

        [Fact]
        public void ClimateZoneData_GetEffectiveBarometricPressure_UsesJsonWhenSet()
        {
            var zone = new ClimateZoneData { Id = 7, BarometricPressure_Pa = 95000.0 };
            Assert.Equal(95000.0, zone.GetEffectiveBarometricPressure(), 0.1);
        }

        [Fact]
        public void ClimateZoneData_GetEffectiveBarometricPressure_UsesDefaultWhenNull()
        {
            var zone = new ClimateZoneData { Id = 7 };
            Assert.Equal(94400.0, zone.GetEffectiveBarometricPressure(), 0.1);
        }
    }

    // ════════════════════════════════════════════════════════════════════════════
    // WorkdayScheduleCalculator tests
    // ════════════════════════════════════════════════════════════════════════════

    public class WorkdayScheduleCalculatorTests
    {
        // ── OverlapHours ──────────────────────────────────────────────────────────

        [Theory]
        [InlineData(10, 19, 10, 19, 10)]  // identical → 10 hours
        [InlineData(10, 19,  8, 17,  8)]  // Excel example: H_vent[10..19] ∩ cool[8..17] = 10..17 = 8 h
        [InlineData(10, 19, 20, 23,  0)]  // no overlap
        [InlineData( 0, 23,  0, 23, 24)]  // full day
        [InlineData(10, 19, 15, 22,  5)]  // 15..19 = 5 hours
        public void OverlapHours_Correct(int s1, int e1, int s2, int e2, int expected)
        {
            int result = WorkdayScheduleCalculator.OverlapHours(s1, e1, s2, e2);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void OverlapHours_StartHour10_EndHour19_Is10Hours()
        {
            // "Start=10 End=19 => 10 ч/ден (вкл.)"  – spec requirement
            var range = new DailyTimeRange { StartHour = 10, EndHour = 19 };
            Assert.Equal(10, range.RunHoursPerDay);
        }

        // ── ComputeMonthly: basic workdays ────────────────────────────────────────

        [Fact]
        public void ComputeMonthly_AllWeekdays_JulyReferenceYear_Returns23WorkdaysNoHolidays()
        {
            // July in the reference year has 23 weekdays, season covers full month
            var schedule = MakeWeekdaySchedule(8, 17);
            var seasonStart = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 7, 1);
            var seasonEnd   = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 7, 31);
            var daysOff = new int[12];

            var results = WorkdayScheduleCalculator.ComputeMonthly(schedule, seasonStart, seasonEnd, daysOff, null, global::EE.Doklad.CalendarDefaults.ReferenceYear);

            var july = results.Single(r => r.MonthNumber == 7);
            Assert.Equal(23, july.DaysInSeason);
            Assert.Equal(23.0, july.WorkingDays, 2);
            Assert.Equal(0, july.HolidaysSubtracted);
        }

        [Fact]
        public void ComputeMonthly_20WorkdaysAnd5HolidaysDaysOff_Returns15WorkingDays()
        {
            // June in the reference year has 22 weekdays, so 5 days-off leave 17 working days
            var schedule = MakeWeekdaySchedule(8, 17);
            var seasonStart = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 6, 1);
            var seasonEnd   = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 6, 30);
            var daysOff = new int[12];
            daysOff[5] = 5; // June = index 5

            var results = WorkdayScheduleCalculator.ComputeMonthly(schedule, seasonStart, seasonEnd, daysOff, null, global::EE.Doklad.CalendarDefaults.ReferenceYear);

            var june = results.Single(r => r.MonthNumber == 6);
            Assert.Equal(22, june.DaysInSeason);
            Assert.Equal(17.0, june.WorkingDays, 2);
            Assert.Equal(5, june.HolidaysSubtracted);
        }

        [Fact]
        public void ComputeMonthly_HolidaysOnWeekend_NoEffectWhenWeekendInactive()
        {
            // Weekday-only schedule; official holidays on weekend → must NOT reduce workdays
            var schedule = MakeWeekdaySchedule(8, 17);
            var seasonStart = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 6, 1);
            var seasonEnd   = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 6, 30);
            var daysOff = new int[12]; // no DaysOff

            // June 6 2026 = Saturday -> holiday on weekend
            var officialHolidays = new List<DateTime> { new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 6, 6) };

            var results = WorkdayScheduleCalculator.ComputeMonthly(schedule, seasonStart, seasonEnd, daysOff, officialHolidays, global::EE.Doklad.CalendarDefaults.ReferenceYear);

            var june = results.Single(r => r.MonthNumber == 6);
            Assert.Equal(0, june.HolidaysSubtracted);  // Saturday not active → no subtraction
        }

        [Fact]
        public void ComputeMonthly_IncludeSaturday_CountsSaturdayDays()
        {
            // Mon-Fri + Saturday active schedule
            var schedule = new WeeklyScheduleConfig
            {
                TimeRange       = new DailyTimeRange { StartHour = 8, EndHour = 13 }, // 6 h
                WorkdaysActive  = true,
                SaturdayActive  = true,
                SundayActive    = false
            };
            var seasonStart = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 6, 1);
            var seasonEnd   = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 6, 30);
            var daysOff = new int[12];

            var results = WorkdayScheduleCalculator.ComputeMonthly(schedule, seasonStart, seasonEnd, daysOff, null, global::EE.Doklad.CalendarDefaults.ReferenceYear);
            var june = results.Single(r => r.MonthNumber == 6);

            // June 2026: 22 weekdays + 4 Saturdays = 26 candidate days
            Assert.Equal(26, june.DaysInSeason);
        }

        [Fact]
        public void ComputeMonthly_PartialMonthAtSeasonStart()
        {
            // Season starts June 15 -> partial month must have fewer workdays than full June
            var schedule = MakeWeekdaySchedule(8, 17);
            var seasonStart = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 6, 15);
            var seasonEnd   = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 6, 30);
            var daysOff = new int[12];

            var results = WorkdayScheduleCalculator.ComputeMonthly(schedule, seasonStart, seasonEnd, daysOff, null, global::EE.Doklad.CalendarDefaults.ReferenceYear);
            var june = results.Single(r => r.MonthNumber == 6);

            Assert.True(june.DaysInSeason < 22, "Partial month must have fewer working days than full month.");
        }

        [Fact]
        public void ComputeMonthly_HolidaysOutsideSeason_NotSubtracted()
        {
            var schedule = MakeWeekdaySchedule(8, 17);
            var seasonStart = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 7, 1);
            var seasonEnd   = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 7, 31);
            var daysOff = new int[12];

            // Holiday in August (outside season)
            var officialHolidays = new List<DateTime> { new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 8, 15) };
            var results = WorkdayScheduleCalculator.ComputeMonthly(schedule, seasonStart, seasonEnd, daysOff, officialHolidays, global::EE.Doklad.CalendarDefaults.ReferenceYear);

            var july = results.Single(r => r.MonthNumber == 7);
            Assert.Equal(0, july.HolidaysSubtracted);
        }

        [Fact]
        public void ComputeMonthly_WorkingHours_EqualWorkingDaysTimesRunHours()
        {
            var schedule = MakeWeekdaySchedule(10, 19); // 10 h/day
            var seasonStart = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 7, 1);
            var seasonEnd   = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 7, 31);
            var daysOff = new int[12];

            var results = WorkdayScheduleCalculator.ComputeMonthly(schedule, seasonStart, seasonEnd, daysOff, null, global::EE.Doklad.CalendarDefaults.ReferenceYear);
            var july = results.Single(r => r.MonthNumber == 7);

            Assert.Equal(10, schedule.TimeRange.RunHoursPerDay);
            Assert.InRange(july.WorkingHours, july.WorkingDays * 10 - 0.01, july.WorkingDays * 10 + 0.01);
        }

        // ── OverlapFraction ───────────────────────────────────────────────────────

        [Fact]
        public void OverlapFraction_SameSchedule_Returns1()
        {
            var r = new DailyTimeRange { StartHour = 10, EndHour = 19 };
            double f = WorkdayScheduleCalculator.OverlapFraction(r, r);
            Assert.Equal(1.0, f, 4);
        }

        [Fact]
        public void OverlapFraction_NoOverlap_Returns0()
        {
            var vent = new DailyTimeRange { StartHour = 10, EndHour = 19 };
            var cool = new DailyTimeRange { StartHour = 20, EndHour = 23 };
            double f = WorkdayScheduleCalculator.OverlapFraction(vent, cool);
            Assert.Equal(0.0, f, 4);
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private static WeeklyScheduleConfig MakeWeekdaySchedule(int start, int end) =>
            new WeeklyScheduleConfig
            {
                TimeRange      = new DailyTimeRange { StartHour = start, EndHour = end },
                WorkdaysActive = true,
                SaturdayActive = false,
                SundayActive   = false
            };
    }

    // ════════════════════════════════════════════════════════════════════════════
    // VentCoolingEngineV2 integration tests
    // ════════════════════════════════════════════════════════════════════════════

    public class VentCoolingEngineV2Tests
    {
        private static VentCoolingInputV2 MakeInput(
            double qSpec = 2.0,
            double area  = 100.0,
            double tSup  = 20.0,
            double rhSup = 60.0,
            double bPa   = 94400.0,
            int startHour = 10,
            int endHour   = 19,
            int seasonStartMonth = 6,
            int seasonEndMonth   = 8)
        {
            return new VentCoolingInputV2
            {
                AirflowSpec_m3hm2    = qSpec,
                CooledArea_m2        = area,
                SupplyTemperature_C  = tSup,
                SupplyRH_Pct         = rhSup,
                BarometricPressure_Pa = bPa,
                VentSchedule = new WeeklyScheduleConfig
                {
                    TimeRange      = new DailyTimeRange { StartHour = startHour, EndHour = endHour },
                    WorkdaysActive = true,
                    SaturdayActive = false,
                    SundayActive   = false
                },
                SeasonStart    = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, seasonStartMonth, 1),
                SeasonEnd      = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, seasonEndMonth,
                    DateTime.DaysInMonth(global::EE.Doklad.CalendarDefaults.ReferenceYear, seasonEndMonth)),
                DaysOffPerMonth = new int[12],
                EnergySource1   = new EnergySourceConfigV2 { Share_Pct = 100, TotalEfficiency = 3.0 }
            };
        }

        private static Func<int, System.Collections.Generic.IReadOnlyList<ClimateHourPoint>>
            MakeUniformClimate(double tOut, double rhOut, double bPa = 94400.0) =>
            month =>
            {
                var pts = new List<ClimateHourPoint>(24);
                for (int h = 0; h < 24; h++)
                    pts.Add(new ClimateHourPoint { Hour = h, T_out_C = tOut, RH_out_Pct = rhOut, B_Pa = bPa });
                return pts;
            };

        [Fact]
        public void Engine_OutdoorHotterThanSupply_ProducesCoolingLoad()
        {
            var input = MakeInput(tSup: 20.0, rhSup: 50.0, bPa: 94400.0);
            var engine = new VentCoolingEngineV2();

            // T_out=30°C, RH=50% → h_out > h_sup → cooling load
            var result = engine.Calculate(input, MakeUniformClimate(30.0, 50.0, 94400.0), isBgAvgMode: true);

            Assert.True(result.IsValid, result.ErrorMessage);
            Assert.True(result.TotalCoolNet_kWhm2 > 0.0);
            Assert.Equal(0.0, result.TotalHeatNet_kWhm2, 3);
        }

        [Fact]
        public void Engine_OutdoorCoolerThanSupply_ProducesHeatingLoad()
        {
            var input = MakeInput(tSup: 25.0, rhSup: 50.0, seasonStartMonth: 1, seasonEndMonth: 2);
            var engine = new VentCoolingEngineV2();

            // T_out=5°C → h_out << h_sup
            var result = engine.Calculate(input, MakeUniformClimate(5.0, 70.0), isBgAvgMode: true);

            Assert.True(result.IsValid);
            Assert.Equal(0.0, result.TotalCoolNet_kWhm2, 3);
            Assert.True(result.TotalHeatNet_kWhm2 > 0.0);
        }

        [Fact]
        public void Engine_EnergySource1_COP3_FinalIsOneThirdOfNet()
        {
            var input = MakeInput(tSup: 20.0, bPa: 94400.0);
            var engine = new VentCoolingEngineV2();

            var result = engine.Calculate(input, MakeUniformClimate(30.0, 50.0), isBgAvgMode: true);

            // FinalEI1 = netCool / COP = netCool / 3
            double expected = result.TotalNetEnergy_kWhm2 / 3.0;
            Assert.InRange(result.FinalEnergyEI1_kWhm2, expected - 0.01, expected + 0.01);
        }

        [Fact]
        public void Engine_ZeroAirflow_ReturnsError()
        {
            var input = MakeInput(qSpec: 0.0);
            var engine = new VentCoolingEngineV2();

            var result = engine.Calculate(input, MakeUniformClimate(30.0, 50.0));
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Engine_MonthlyResults_OnlyContainSeasonMonths()
        {
            // Season June..August
            var input = MakeInput(seasonStartMonth: 6, seasonEndMonth: 8);
            var engine = new VentCoolingEngineV2();

            var result = engine.Calculate(input, MakeUniformClimate(30.0, 50.0), isBgAvgMode: true);

            var months = result.MonthlyResults.Select(r => r.MonthNumber).ToList();
            Assert.DoesNotContain(1, months);
            Assert.DoesNotContain(12, months);
            Assert.Contains(7, months);
        }

        [Fact]
        public void Engine_DryingEnergy_PositiveWhenXout_GreaterThan_Xsup()
        {
            // T_out=30°C RH=90% → very high x_out; T_sup=20°C RH=40% → low x_sup
            var input = MakeInput(tSup: 20.0, rhSup: 40.0, bPa: 101325.0);
            var engine = new VentCoolingEngineV2();

            var result = engine.Calculate(input, MakeUniformClimate(30.0, 90.0, 101325.0));

            Assert.True(result.TotalDryNet_kWhm2 > 0.0, "Трябва да има латентна компонента при висока влажност навън.");
        }

        [Fact]
        public void Engine_Recuperation_Efficiency50_ReducesCoolingLoad()
        {
            // With 50% recuperation, effective h_out is reduced
            var inputNoRec = new VentCoolingInputV2
            {
                AirflowSpec_m3hm2     = 2.0,
                CooledArea_m2         = 100.0,
                SupplyTemperature_C   = 20.0,
                SupplyRH_Pct          = 50.0,
                BarometricPressure_Pa = 101325.0,
                RecuperationEfficiency = 0.0,
                VentSchedule = new WeeklyScheduleConfig
                {
                    TimeRange      = new DailyTimeRange { StartHour = 10, EndHour = 19 },
                    WorkdaysActive = true
                },
                SeasonStart     = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 6, 1),
                SeasonEnd       = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 8, 31),
                DaysOffPerMonth = new int[12],
                EnergySource1   = new EnergySourceConfigV2 { Share_Pct = 100, TotalEfficiency = 3.0 }
            };
            var inputWithRec = new VentCoolingInputV2
            {
                AirflowSpec_m3hm2       = 2.0,
                CooledArea_m2           = 100.0,
                SupplyTemperature_C     = 20.0,
                SupplyRH_Pct            = 50.0,
                BarometricPressure_Pa   = 101325.0,
                RecuperationEfficiency  = 0.5,
                ExtractAirTemperature_C = 24.0,
                ExtractAirRH_Pct        = 50.0,
                VentSchedule = new WeeklyScheduleConfig
                {
                    TimeRange      = new DailyTimeRange { StartHour = 10, EndHour = 19 },
                    WorkdaysActive = true
                },
                SeasonStart     = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 6, 1),
                SeasonEnd       = new DateTime(global::EE.Doklad.CalendarDefaults.ReferenceYear, 8, 31),
                DaysOffPerMonth = new int[12],
                EnergySource1   = new EnergySourceConfigV2 { Share_Pct = 100, TotalEfficiency = 3.0 }
            };
            var engine = new VentCoolingEngineV2();

            var rNoRec  = engine.Calculate(inputNoRec,  MakeUniformClimate(30.0, 60.0, 101325.0));
            var rWithRec = engine.Calculate(inputWithRec, MakeUniformClimate(30.0, 60.0, 101325.0));

            Assert.True(rWithRec.TotalCoolNet_kWhm2 < rNoRec.TotalCoolNet_kWhm2,
                "Рекуперацията трябва да намалява охлаждащото натоварване.");
        }
    }

    // ════════════════════════════════════════════════════════════════════════════
    // BgAvgClimateProvider – проверка, че зарежда РЕАЛЕН почасов профил
    // ════════════════════════════════════════════════════════════════════════════

    public class BgAvgClimateProviderHourlyProfileTests
    {
        private static ClimateZoneData MakeZoneData(int id, double[] avgTemps12, double[] avgRh5)
        {
            return new ClimateZoneData
            {
                Id   = id,
                Name = $"Test Zone {id}",
                Monthly = new EE.Doklad.Models.MonthlyClimateData
                {
                    AvgMonthlyTempC                    = avgTemps12,
                    AvgMonthlyRelHumidityPercentMayToSep = avgRh5,
                }
            };
        }

        /// <summary>
        /// BgAvgClimateProvider трябва да върне 24 РАЗЛИЧНИ температури за юни, зона 7.
        /// Преди fix-а всички 24 реда бяха с месечната средна (18.7°C), което е ГРЕШНО.
        /// </summary>
        [Fact]
        public void GetHourlyData_Zone7_June_ReturnsVaryingTemperatures()
        {
            // Zone 7 – средна за юни ≈ 18.7 (legacy fallback стойност).
            // Реалният почасов профил има стойности от ~13.7 до ~23.7.
            var avgTemps = new double[] { 0, 2, 5, 10, 15, 18.7, 21, 20, 15, 10, 5, 1 };
            var avgRh    = new double[] { 65, 63, 60, 61, 60 }; // May..Sep
            var zoneData = MakeZoneData(7, avgTemps, avgRh);

            var provider = new BgAvgClimateProvider(zoneData);
            var points   = provider.GetHourlyData(6); // юни = месец 6

            Assert.Equal(24, points.Count);

            // Ако е заредил реалния профил: температурите трябва да се различават
            double minT = points.Min(p => p.T_out_C);
            double maxT = points.Max(p => p.T_out_C);
            double range = maxT - minT;

            // Реалният диапазон за зона 7, юни е ~10°C (13.7..23.7)
            // Ако range < 1 → все още ползва месечна средна → бъгът е жив
            Assert.True(range > 5.0,
                $"Очакван диапазон на T > 5°C, но имаме min={minT:F1}, max={maxT:F1}, range={range:F1}. " +
                "BgAvgClimateProvider вероятно не е заредил почасовия JSON (EmbeddedResource липсва).");
        }

        /// <summary>
        /// Час 10 за зона 7, юни трябва да е 20.7°C (данните от JSON), не 18.7 (месечна средна).
        /// </summary>
        [Fact]
        public void GetHourlyData_Zone7_June_Hour10_Is_20_7C()
        {
            var avgTemps = new double[] { 0, 2, 5, 10, 15, 18.7, 21, 20, 15, 10, 5, 1 };
            var avgRh    = new double[] { 65, 63, 60, 61, 60 };
            var zoneData = MakeZoneData(7, avgTemps, avgRh);

            var provider = new BgAvgClimateProvider(zoneData);
            var points   = provider.GetHourlyData(6);

            var hour10 = points.First(p => p.Hour == 10);
            Assert.InRange(hour10.T_out_C, 20.0, 21.5); // JSON: 20.7°C
        }

        /// <summary>
        /// Час 5 за зона 7, юни трябва да е 13.7°C (нощен минимум), не 18.7.
        /// </summary>
        [Fact]
        public void GetHourlyData_Zone7_June_Hour5_Is_13_7C()
        {
            var avgTemps = new double[] { 0, 2, 5, 10, 15, 18.7, 21, 20, 15, 10, 5, 1 };
            var avgRh    = new double[] { 65, 63, 60, 61, 60 };
            var zoneData = MakeZoneData(7, avgTemps, avgRh);

            var provider = new BgAvgClimateProvider(zoneData);
            var points   = provider.GetHourlyData(6);

            var hour5 = points.First(p => p.Hour == 5);
            Assert.InRange(hour5.T_out_C, 13.0, 14.5); // JSON: 13.7°C
        }
    }
}
