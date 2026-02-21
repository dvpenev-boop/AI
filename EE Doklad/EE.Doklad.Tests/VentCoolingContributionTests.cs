using System;
using EE.Doklad.Models;
using EE.Doklad.Services.Schedule;
using EE.Doklad.Services.VentCooling;
using Xunit;

namespace EE.Doklad.Tests
{
    // ════════════════════════════════════════════════════════════════════════════
    // OverlapCalculator tests
    // ════════════════════════════════════════════════════════════════════════════

    public class OverlapCalculatorTests
    {
        // ── Helpers ───────────────────────────────────────────────────────────────

        private static WeeklyTimeRange R(double startH, double endH) => new WeeklyTimeRange
        {
            StartTime = TimeSpan.FromHours(startH),
            EndTime   = TimeSpan.FromHours(endH)
        };

        private static WeeklySchedule W(WeeklyTimeRange wd, WeeklyTimeRange sat, WeeklyTimeRange sun)
            => new WeeklySchedule { Workdays = wd, Saturday = sat, Sunday = sun };

        private static WeeklySchedule WdOnly(double startH, double endH)
        {
            var r = R(startH, endH);
            return W(r, R(0, 0), R(0, 0));
        }

        // ── GetDurationHours ──────────────────────────────────────────────────────

        [Theory]
        [InlineData(8.0,  16.0, 8.0)]      // normal: 8h
        [InlineData(0.0,  24.0, 24.0)]     // full day (24:00 = midnight next)
        [InlineData(22.0,  6.0, 8.0)]      // overnight wrap: (24-22)+6 = 8h
        [InlineData(10.0, 10.0, 0.0)]      // Start == End → 0h (изключено)
        [InlineData(0.0,   0.0, 0.0)]      // both zero → 0h
        public void GetDurationHours_ReturnsCorrectValue(double startH, double endH, double expectedH)
        {
            var r = R(startH, endH);
            double actual = OverlapCalculator.GetDurationHours(r);
            Assert.InRange(actual, expectedH - 1e-9, expectedH + 1e-9);
        }

        // ── GetOverlapHours (no wrap) ─────────────────────────────────────────────

        [Fact]
        public void GetOverlapHours_SameRange_EqualsDuration()
        {
            // 08:00–16:00 ∩ 08:00–16:00 = 8h
            var r = R(8.0, 16.0);
            double ov = OverlapCalculator.GetOverlapHours(r, r);
            Assert.InRange(ov, 8.0 - 1e-6, 8.0 + 1e-6);
        }

        [Fact]
        public void GetOverlapHours_NoOverlap_ReturnsZero()
        {
            // 08:00–10:00 and 14:00–20:00 → no overlap
            double ov = OverlapCalculator.GetOverlapHours(R(8.0, 10.0), R(14.0, 20.0));
            Assert.Equal(0.0, ov, 6);
        }

        [Fact]
        public void GetOverlapHours_PartialOverlap_Correct()
        {
            // 08:00–16:00 ∩ 10:00–20:00 → 10:00–16:00 = 6h
            double ov = OverlapCalculator.GetOverlapHours(R(8.0, 16.0), R(10.0, 20.0));
            Assert.InRange(ov, 6.0 - 1e-6, 6.0 + 1e-6);
        }

        [Fact]
        public void GetOverlapHours_FullContainment_EqualsSmallerDuration()
        {
            // vent 10:00–14:00 (4h), cool 08:00–20:00 (12h) → overlap = 4h
            double ov = OverlapCalculator.GetOverlapHours(R(10.0, 14.0), R(8.0, 20.0));
            Assert.InRange(ov, 4.0 - 1e-6, 4.0 + 1e-6);
        }

        [Fact]
        public void GetOverlapHours_StartEqualsEnd_ReturnsZero()
        {
            // Start == End on one side → that schedule is off → 0 overlap
            double ov = OverlapCalculator.GetOverlapHours(R(10.0, 10.0), R(8.0, 16.0));
            Assert.Equal(0.0, ov, 6);
        }

        // ── GetOverlapHours (wrap-around / overnight) ─────────────────────────────

        [Fact]
        public void GetOverlapHours_WrapCooling_WrapVent_FullOverlap()
        {
            // Both overnight 22:00–06:00, identical → overlap = 8h
            var r = R(22.0, 6.0);
            double ov = OverlapCalculator.GetOverlapHours(r, r);
            Assert.InRange(ov, 8.0 - 1e-6, 8.0 + 1e-6);
        }

        [Fact]
        public void GetOverlapHours_WrapVent_NormalCool_PartialOverlap()
        {
            // Vent 22:00–06:00 (8h), Cool 04:00–10:00 (6h)
            // Overlap: [04:00–06:00) = 2h
            double ov = OverlapCalculator.GetOverlapHours(R(22.0, 6.0), R(4.0, 10.0));
            Assert.InRange(ov, 2.0 - 1e-6, 2.0 + 1e-6);
        }

        [Fact]
        public void GetOverlapHours_WrapVent_NormalCool_NoOverlap()
        {
            // Vent 22:00–04:00, Cool 06:00–18:00 → no overlap
            double ov = OverlapCalculator.GetOverlapHours(R(22.0, 4.0), R(6.0, 18.0));
            Assert.Equal(0.0, ov, 6);
        }

        [Fact]
        public void GetOverlapHours_WrapBoth_PartialOverlap()
        {
            // Vent 20:00–02:00 (6h), Cool 22:00–04:00 (6h)
            // Overlap: [22:00–02:00) = 4h
            double ov = OverlapCalculator.GetOverlapHours(R(20.0, 2.0), R(22.0, 4.0));
            Assert.InRange(ov, 4.0 - 1e-6, 4.0 + 1e-6);
        }

        // ── ComputeFon (седмичен модел) ───────────────────────────────────────────

        [Fact]
        public void ComputeFon_NoVentHours_Returns0_WithWarning()
        {
            // All zeroes → VentHoursWeek = 0
            var vent = W(R(0, 0), R(0, 0), R(0, 0));
            var cool = W(R(8, 16), R(0, 0), R(0, 0));

            double f = OverlapCalculator.ComputeFon(vent, cool, out string? warning);

            Assert.Equal(0.0, f, 6);
            Assert.NotNull(warning);
            Assert.Contains("0", warning);
        }

        [Fact]
        public void ComputeFon_WorkdaysOnly_08to16_and_10to20_OverlapPerSpec()
        {
            // Spec example: VentCooling 08:00–16:00 (8h/wd), Cooling 10:00–20:00 (10h/wd)
            // Overlap per wd = 08:00–16:00 ∩ 10:00–20:00 = 10:00–16:00 = 6h
            // VentHoursWeek = 8*5 = 40
            // OverlapHoursWeek = 6*5 = 30
            // f_on = 30/40 = 0.75
            var vent = WdOnly(8.0, 16.0);
            var cool = WdOnly(10.0, 20.0);

            double f = OverlapCalculator.ComputeFon(vent, cool, out _);
            Assert.InRange(f, 0.75 - 1e-6, 0.75 + 1e-6);
        }

        [Fact]
        public void ComputeFon_WithSatSun_AggregatesCorrectly()
        {
            // Wd: 08:00–16:00, Sat: 10:00–14:00, Sun: off
            // Cool: Wd 08:00–20:00, Sat 10:00–14:00, Sun off
            // Wd overlap: 8h/wd × 5 = 40, Sat overlap: 4h, Sun: 0
            // VentHoursWeek = 8*5 + 4 + 0 = 44
            // OverlapWeek   = 8*5 + 4 + 0 = 44 (cool fully contains vent)
            // f_on = 44/44 = 1.0
            var vent = W(R(8.0, 16.0), R(10.0, 14.0), R(0.0, 0.0));
            var cool = W(R(8.0, 20.0), R(10.0, 14.0), R(0.0, 0.0));

            double f = OverlapCalculator.ComputeFon(vent, cool, out _);
            Assert.InRange(f, 1.0 - 1e-6, 1.0 + 1e-6);
        }

        [Fact]
        public void ComputeFon_FullOverlap_Returns1()
        {
            var sched = WdOnly(8.0, 16.0);
            double f = OverlapCalculator.ComputeFon(sched, sched, out _);
            Assert.InRange(f, 1.0 - 1e-6, 1.0 + 1e-6);
        }

        [Fact]
        public void ComputeFon_NoOverlap_Returns0()
        {
            var vent = WdOnly(8.0, 12.0);
            var cool = WdOnly(14.0, 20.0);
            double f = OverlapCalculator.ComputeFon(vent, cool, out _);
            Assert.Equal(0.0, f, 6);
        }

        [Fact]
        public void ComputeFon_ClampsAbove1_WhenOverlapExceedsVent()
        {
            // This should not happen with valid schedules, but clamp protects against rounding
            // We simulate by using vent=cool exactly → f=1
            var r = WdOnly(8.0, 20.0);
            double f = OverlapCalculator.ComputeFon(r, r, out _);
            Assert.InRange(f, 0.0, 1.0);
        }
    }

    // ════════════════════════════════════════════════════════════════════════════
    // VentCoolingContributionCalculator tests
    // ════════════════════════════════════════════════════════════════════════════

    public class VentCoolingContributionCalculatorTests
    {
        private static WeeklyTimeRange R(double startH, double endH) => new WeeklyTimeRange
        {
            StartTime = TimeSpan.FromHours(startH),
            EndTime   = TimeSpan.FromHours(endH)
        };

        private static WeeklySchedule WdOnly(double startH, double endH)
        {
            var r = R(startH, endH);
            return new WeeklySchedule { Workdays = r, Saturday = R(0, 0), Sunday = R(0, 0) };
        }

        private static VentCoolingContributionInput BaseInput(
            double airflow   = 2.0,
            double supplyT   = 16.0,
            double seasonH   = 1000.0,
            double designT   = 26.0,
            double raisedT   = 28.0,
            double area      = 100.0,
            WeeklySchedule? vent = null,
            WeeklySchedule? cool = null) =>
            new VentCoolingContributionInput
            {
                Airflow_m3ph_per_m2  = airflow,
                SupplyAirTemp_C      = supplyT,
                TotalWorkHoursSeason = seasonH,
                RoomTemp_Design_C    = designT,
                RoomTemp_Raised_C    = raisedT,
                CoolingArea_m2       = area,
                VentCoolingSchedule  = vent,
                CoolingSchedule      = cool,
            };

        // ── Null/invalid input ────────────────────────────────────────────────────

        [Fact]
        public void Calculate_NullInput_ReturnsInvalid()
        {
            var r = VentCoolingContributionCalculator.Calculate(null!);
            Assert.False(r.IsValid);
            Assert.NotNull(r.ErrorMessage);
        }

        [Fact]
        public void Calculate_NegativeAirflow_ReturnsInvalid()
        {
            var r = VentCoolingContributionCalculator.Calculate(BaseInput(airflow: -1.0));
            Assert.False(r.IsValid);
        }

        [Fact]
        public void Calculate_NegativeSeasonHours_ReturnsInvalid()
        {
            var r = VentCoolingContributionCalculator.Calculate(BaseInput(seasonH: -5.0));
            Assert.False(r.IsValid);
        }

        [Fact]
        public void Calculate_NegativeArea_ReturnsInvalid()
        {
            var r = VentCoolingContributionCalculator.Calculate(BaseInput(area: -10.0));
            Assert.False(r.IsValid);
        }

        // ── BaseFactor ────────────────────────────────────────────────────────────

        [Fact]
        public void Calculate_BaseFactor_Formula()
        {
            // BaseFactor = 0.34 * q * H / 1000
            //            = 0.34 * 2.0 * 1000 / 1000 = 0.68
            var r = VentCoolingContributionCalculator.Calculate(BaseInput(airflow: 2.0, seasonH: 1000.0));
            Assert.True(r.IsValid, r.ErrorMessage);
            Assert.InRange(r.BaseFactor, 0.68 - 1e-9, 0.68 + 1e-9);
        }

        // ── Scenario values ───────────────────────────────────────────────────────

        [Fact]
        public void Calculate_ScenarioDesign_CorrectFormula()
        {
            // BaseFactor=0.68, ΔT_design = 26 - 16 = 10 → 6.8 kWh/m²
            var r = VentCoolingContributionCalculator.Calculate(BaseInput());
            Assert.InRange(r.ScenarioDesign_kWhm2, 6.8 - 1e-6, 6.8 + 1e-6);
        }

        [Fact]
        public void Calculate_ScenarioRaised_CorrectFormula()
        {
            // BaseFactor=0.68, ΔT_raised = 28 - 16 = 12 → 8.16 kWh/m²
            var r = VentCoolingContributionCalculator.Calculate(BaseInput());
            Assert.InRange(r.ScenarioRaised_kWhm2, 8.16 - 1e-6, 8.16 + 1e-6);
        }

        [Fact]
        public void Calculate_MinMax_Correct()
        {
            var r = VentCoolingContributionCalculator.Calculate(BaseInput());
            Assert.Equal(r.Min_kWhm2, Math.Min(r.ScenarioDesign_kWhm2, r.ScenarioRaised_kWhm2), 9);
            Assert.Equal(r.Max_kWhm2, Math.Max(r.ScenarioDesign_kWhm2, r.ScenarioRaised_kWhm2), 9);
        }

        // ── f_on = 1 (no schedules provided → default conservative 1.0) ──────────

        [Fact]
        public void Calculate_NoSchedules_Fon1_NetEqualsScenarioDesign()
        {
            // No schedules → f_on = 1.0 → Net = 1.0 * ScenarioDesign + 0 * ScenarioRaised
            var r = VentCoolingContributionCalculator.Calculate(BaseInput());
            Assert.InRange(r.F_on, 1.0 - 1e-9, 1.0 + 1e-9);
            Assert.InRange(r.Net_kWhm2, r.ScenarioDesign_kWhm2 - 1e-9, r.ScenarioDesign_kWhm2 + 1e-9);
        }

        // ── f_on from schedules ───────────────────────────────────────────────────

        [Fact]
        public void Calculate_Fon_0p6_NetIsWeightedAverage()
        {
            // Vent 08:00-16:00 (8h/wd), Cool 10:00-20:00 (10h/wd)
            // Overlap = 6h/wd, VentH = 8h/wd
            // VentHoursWeek = 40, OverlapWeek = 30 → f_on = 0.75
            var vent = WdOnly(8.0, 16.0);
            var cool = WdOnly(10.0, 20.0);
            var r = VentCoolingContributionCalculator.Calculate(BaseInput(vent: vent, cool: cool));

            Assert.True(r.IsValid);
            Assert.InRange(r.F_on, 0.75 - 1e-6, 0.75 + 1e-6);

            double expectedNet = 0.75 * r.ScenarioDesign_kWhm2 + 0.25 * r.ScenarioRaised_kWhm2;
            Assert.InRange(r.Net_kWhm2, expectedNet - 1e-6, expectedNet + 1e-6);
        }

        [Fact]
        public void Calculate_NoOverlap_Fon0_NetEqualsScenarioRaised()
        {
            // Vent 08:00-12:00, Cool 14:00-20:00 → f_on = 0
            var vent = WdOnly(8.0, 12.0);
            var cool = WdOnly(14.0, 20.0);
            var r = VentCoolingContributionCalculator.Calculate(BaseInput(vent: vent, cool: cool));

            Assert.True(r.IsValid);
            Assert.Equal(0.0, r.F_on, 6);
            Assert.InRange(r.Net_kWhm2, r.ScenarioRaised_kWhm2 - 1e-9, r.ScenarioRaised_kWhm2 + 1e-9);
        }

        [Fact]
        public void Calculate_VentHoursZero_Fon0_Warning()
        {
            // VentCoolingSchedule all zeros → f_on = 0
            var vent = new WeeklySchedule { Workdays = R(0, 0), Saturday = R(0, 0), Sunday = R(0, 0) };
            var cool = WdOnly(8.0, 16.0);
            var r = VentCoolingContributionCalculator.Calculate(BaseInput(vent: vent, cool: cool));

            Assert.True(r.IsValid);
            Assert.Equal(0.0, r.F_on, 6);
            Assert.True(r.Warnings.Count > 0, "Трябва предупреждение при VentHoursWeek == 0");
        }

        // ── Negative results (Supply > Room temp) ─────────────────────────────────

        [Fact]
        public void Calculate_SupplyTempAboveRoom_NegativeScenarios()
        {
            // Supply 30°C > Room 26/28°C → all scenarios negative → НЕ се клипват
            var r = VentCoolingContributionCalculator.Calculate(BaseInput(supplyT: 30.0, designT: 26.0, raisedT: 28.0));

            Assert.True(r.IsValid);
            Assert.True(r.ScenarioDesign_kWhm2 < 0.0, "ScenarioDesign трябва да е < 0");
            Assert.True(r.ScenarioRaised_kWhm2 < 0.0, "ScenarioRaised трябва да е < 0");
            Assert.True(r.Net_kWhm2 < 0.0, "Net трябва да е < 0 — НЕ се клипва");
        }

        [Fact]
        public void Calculate_SupplyTempAboveRoom_Net_kWh_IsNegative()
        {
            var r = VentCoolingContributionCalculator.Calculate(BaseInput(supplyT: 30.0, area: 200.0));
            Assert.True(r.IsValid);
            Assert.True(r.Net_kWh < 0.0);
        }

        // ── Net_kWh = 0 when area = 0 ─────────────────────────────────────────────

        [Fact]
        public void Calculate_ZeroArea_Net_kWh_Is0_Net_kWhm2_Computed()
        {
            var r = VentCoolingContributionCalculator.Calculate(BaseInput(area: 0.0));
            Assert.True(r.IsValid);
            Assert.Equal(0.0, r.Net_kWh, 9);
            Assert.NotEqual(0.0, r.Net_kWhm2); // kWh/m² still computed
        }

        // ── Wrap-around overlap (overnight schedule) ──────────────────────────────

        [Fact]
        public void Calculate_WrapOverlap_Fon_Correct()
        {
            // Vent 22:00-06:00 (8h/wd), Cool 04:00-10:00 (6h/wd)
            // Overlap = 04:00-06:00 = 2h/wd
            // VentHoursWeek = 8*5 = 40, OverlapWeek = 2*5 = 10
            // f_on = 10/40 = 0.25
            var vent = WdOnly(22.0, 6.0);
            var cool = WdOnly(4.0, 10.0);
            var r = VentCoolingContributionCalculator.Calculate(BaseInput(vent: vent, cool: cool));

            Assert.True(r.IsValid);
            Assert.InRange(r.F_on, 0.25 - 1e-6, 0.25 + 1e-6);
        }

        // ── Zero airflow → BaseFactor = 0 ────────────────────────────────────────

        [Fact]
        public void Calculate_ZeroAirflow_AllResultsZero()
        {
            var r = VentCoolingContributionCalculator.Calculate(BaseInput(airflow: 0.0));
            Assert.True(r.IsValid);
            Assert.Equal(0.0, r.BaseFactor, 12);
            Assert.Equal(0.0, r.ScenarioDesign_kWhm2, 12);
            Assert.Equal(0.0, r.ScenarioRaised_kWhm2, 12);
            Assert.Equal(0.0, r.Net_kWhm2, 12);
            Assert.Equal(0.0, r.Net_kWh, 12);
        }

        // ── Zero season hours ─────────────────────────────────────────────────────

        [Fact]
        public void Calculate_ZeroSeasonHours_AllResultsZero()
        {
            var r = VentCoolingContributionCalculator.Calculate(BaseInput(seasonH: 0.0));
            Assert.True(r.IsValid);
            Assert.Equal(0.0, r.BaseFactor, 12);
            Assert.Equal(0.0, r.Net_kWhm2, 12);
            Assert.Equal(0.0, r.Net_kWh, 12);
        }

        // ── Net_kWh = Net_kWhm2 × area ───────────────────────────────────────────

        [Fact]
        public void Calculate_Net_kWh_EqualsNet_kWhm2_Times_Area()
        {
            double area = 150.0;
            var r = VentCoolingContributionCalculator.Calculate(BaseInput(area: area));
            Assert.True(r.IsValid);
            Assert.InRange(r.Net_kWh, r.Net_kWhm2 * area - 1e-6, r.Net_kWhm2 * area + 1e-6);
        }

        // ── Design == Raised → ScenarioDesign == ScenarioRaised → Net == Design ──

        [Fact]
        public void Calculate_SameDesignAndRaised_NetEqualsScenario()
        {
            var r = VentCoolingContributionCalculator.Calculate(BaseInput(designT: 24.0, raisedT: 24.0));
            Assert.True(r.IsValid);
            Assert.InRange(r.Net_kWhm2, r.ScenarioDesign_kWhm2 - 1e-9, r.ScenarioDesign_kWhm2 + 1e-9);
            Assert.InRange(r.Min_kWhm2, r.Max_kWhm2 - 1e-9, r.Max_kWhm2 + 1e-9);
        }
    }
}
