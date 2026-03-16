using System;
using EE.Doklad.Models;
using EE.Doklad.Services;
using Xunit;

namespace EE.Doklad.Tests
{
    public class ZtuQtrTests
    {
        [Fact]
        public void Cooling_Qtr_Calculation_July_Example()
        {
            // ΣUA_sep = 100 W/K, thetaAdjSummer = 35°C, cooling project temp = 25°C
            // July days = 31 -> hours = 744
            var zone = new ZtuZone { Name = "Z1", Type = ZtuType.External };

            var monthlyResults = new ZtuMonthlyResults();
            for (int m = 0; m < 12; m++)
            {
                monthlyResults.Months.Add(new ZtuMonthlyResult
                {
                    MonthNumber = m + 1,
                    MonthName = "",
                    OutdoorTempC = 20.0,
                    HztcZtu_WK = (m == 6 ? 100.0 : 0.0) // July -> index 6
                });
            }

            var objectData = new ObjectDataSectionData();
            // Ensure cooling schedule is present so cooling hours are non-zero
            objectData.CoolingWorkdaysHours = "24";
            objectData.CoolingSaturdayHours = "24";
            objectData.CoolingSundayHours = "24";
            var heatingData = new HeatingSectionData();
            var unconditioned = new UnconditionedZoneSectionData { ThetaAdjSummer = 35.0 };

            // Minimal climate data (not used for cooling hours calculation)
            var climate = new ClimateZoneData();

            var calc = new UnconditionedZonesCalculator();
            var qtr = calc.CalculateQtrResults(zone, monthlyResults, objectData, heatingData, unconditioned, climate, global::EE.Doklad.CalendarDefaults.ReferenceYear);

            // Find July result
            var july = qtr.Months[6];
            double expected = 100.0 * (35.0 - 25.0) * 31.0 * 24.0 / 1000.0; // = 744 kWh
            Assert.InRange(july.Qtr_cool_kWh, expected - 0.1, expected + 0.1);
        }

        [Fact]
        public void HeatingHours_Holiday_Reduction_Test()
        {
            // Schedule: workday 8h, no weekend hours -> hoursPerWeek=40 => per day=40/7
            // July days 31, holidays=10 -> active days=21
            var objectData = new ObjectDataSectionData
            {
                HeatingWorkdaysHours = "8",
                HeatingSaturdayHours = "0",
                HeatingSundayHours = "0",
                DaysOffJuly = "10"
            };

            var climate = new ClimateZoneData();
            // define heating season full year
            climate.HeatingSeason = new HeatingSeasonInfo { Start = "1-1", End = "12-31" };

            var hours = HeatingScheduleService.ComputeHeatingHoursPerMonth(objectData, climate, global::EE.Doklad.CalendarDefaults.ReferenceYear);
            // Compute expected using calendar counts (consistent with implementation)
            int month = 7; // July
            int year = global::EE.Doklad.CalendarDefaults.ReferenceYear;
            int workdayCount = 0, satCount = 0, sunCount = 0, heatingSeasonDays = 0;
            for (int d = 1; d <= DateTime.DaysInMonth(year, month); d++)
            {
                var dt = new DateTime(year, month, d);
                // heating season set to full year in this test
                heatingSeasonDays++;
                switch (dt.DayOfWeek)
                {
                    case DayOfWeek.Saturday: satCount++; break;
                    case DayOfWeek.Sunday: sunCount++; break;
                    default: workdayCount++; break;
                }
            }

            double baseHours = workdayCount * 8.0 + satCount * 0.0 + sunCount * 0.0;
            double avgDaily = baseHours / (double)heatingSeasonDays;
            double reduction = Math.Min(baseHours, 10 * avgDaily);
            double expected = baseHours - reduction;

            Assert.InRange(hours[6], expected - 0.5, expected + 0.5);
        }

        [Fact]
        public void Heating_Hours_AllZero_LeadsToZero_QtrHeat()
        {
            var zone = new ZtuZone { Name = "Z1", Type = ZtuType.External };
            var monthlyResults = new ZtuMonthlyResults();
            for (int m = 0; m < 12; m++) monthlyResults.Months.Add(new ZtuMonthlyResult { MonthNumber = m + 1, MonthName = "M", OutdoorTempC = 0.0, HztcZtu_WK = 100.0 });

            var objectData = new ObjectDataSectionData
            {
                HeatingWorkdaysHours = "0",
                HeatingSaturdayHours = "0",
                HeatingSundayHours = "0"
            };

            var heatingData = new HeatingSectionData { DesignTemperature = 20.0, ReductionTemperature = 16.0 };
            var unconditioned = new UnconditionedZoneSectionData { ThetaAdjWinter = 5.0 };
            var climate = new ClimateZoneData();

            var calc = new UnconditionedZonesCalculator();
            var qtr = calc.CalculateQtrResults(zone, monthlyResults, objectData, heatingData, unconditioned, climate, global::EE.Doklad.CalendarDefaults.ReferenceYear);

            foreach (var m in qtr.Months)
            {
                Assert.Equal(0.0, m.HeatingHours_h);
                Assert.Equal(0.0, m.Qtr_heat_kWh);
            }
        }

        [Fact]
        public void Full24hSchedule_Produces_HoursEqualTo_HoursInMonth()
        {
            var objectData = new ObjectDataSectionData
            {
                HeatingWorkdaysHours = "24",
                HeatingSaturdayHours = "24",
                HeatingSundayHours = "24"
            };
            var climate = new ClimateZoneData();
            climate.HeatingSeason = new HeatingSeasonInfo { Start = "1-1", End = "12-31" };

            var hours = HeatingScheduleService.ComputeHeatingHoursPerMonth(objectData, climate, global::EE.Doklad.CalendarDefaults.ReferenceYear);
            for (int m = 0; m < 12; m++)
            {
                int days = DateTime.DaysInMonth(global::EE.Doklad.CalendarDefaults.ReferenceYear, m + 1);
                Assert.InRange(hours[m], days * 24.0 - 0.1, days * 24.0 + 0.1);
            }
        }

        [Fact]
        public void EffectiveTemp_Halftime_Gives_Midpoint()
        {
            double design = 20.0;
            double setback = 18.0;
            double hoursInMonth = 100.0;
            double heatingHours = 50.0; // 50% -> mid

            double eff = ScheduleHelper.GetEffectiveHeatingIndoorTemp(design, setback, heatingHours, hoursInMonth);
            Assert.InRange(eff, 19.0 - 1e-6, 19.0 + 1e-6);
        }

        [Fact]
        public void Cooling_Qtr_Changes_When_ThetaAdjSummer_Changes()
        {
            var zone = new ZtuZone { Name = "Z1", Type = ZtuType.External };
            var monthlyResults = new ZtuMonthlyResults();
            for (int m = 0; m < 12; m++) monthlyResults.Months.Add(new ZtuMonthlyResult { MonthNumber = m + 1, MonthName = "M", OutdoorTempC = 0.0, HztcZtu_WK = (m>=4 && m<=8?100.0:0.0) });

            var objectData = new ObjectDataSectionData();
            // Ensure cooling schedule makes cooling hours non-zero (e.g., full May-Sep by schedule)
            objectData.CoolingWorkdaysHours = "24";
            objectData.CoolingSaturdayHours = "24";
            objectData.CoolingSundayHours = "24";

            var heatingData = new HeatingSectionData();
            var climate = new ClimateZoneData();

            var calc = new UnconditionedZonesCalculator();

            var uncondA = new UnconditionedZoneSectionData { ThetaAdjSummer = 30.0 };
            var qA = calc.CalculateQtrResults(zone, monthlyResults, objectData, heatingData, uncondA, climate, global::EE.Doklad.CalendarDefaults.ReferenceYear);

            var uncondB = new UnconditionedZoneSectionData { ThetaAdjSummer = 35.0 };
            var qB = calc.CalculateQtrResults(zone, monthlyResults, objectData, heatingData, uncondB, climate, global::EE.Doklad.CalendarDefaults.ReferenceYear);

            // Sum cooling May..Sep should be larger for B
            Assert.True(qB.Annual_Qtr_cool_kWh > qA.Annual_Qtr_cool_kWh);
        }

        [Fact]
        public void CoolingHours_Filter_Applies_JanZero_JulNonZero()
        {
            var zone = new ZtuZone { Name = "Z1", Type = ZtuType.External };
            var monthlyResults = new ZtuMonthlyResults();
            for (int m = 0; m < 12; m++) monthlyResults.Months.Add(new ZtuMonthlyResult { MonthNumber = m + 1, MonthName = "M", OutdoorTempC = 0.0, HztcZtu_WK = (m>=4 && m<=8?100.0:0.0) });

            var objectData = new ObjectDataSectionData();
            // Ensure cooling schedule is present so service returns non-zero hours
            objectData.CoolingWorkdaysHours = "24";
            objectData.CoolingSaturdayHours = "24";
            objectData.CoolingSundayHours = "24";

            var heatingData = new HeatingSectionData();
            var climate = new ClimateZoneData();

            var calc = new UnconditionedZonesCalculator();
            var uncond = new UnconditionedZoneSectionData { ThetaAdjSummer = 35.0 };

            var q = calc.CalculateQtrResults(zone, monthlyResults, objectData, heatingData, uncond, climate, global::EE.Doklad.CalendarDefaults.ReferenceYear);

            // January index 0 must have zero cooling hours per temporary filter
            Assert.Equal(0.0, q.Months[0].CoolingHours_h);
            // July index 6 should have non-zero cooling hours
            Assert.True(q.Months[6].CoolingHours_h > 0.0);
        }
    }
}
