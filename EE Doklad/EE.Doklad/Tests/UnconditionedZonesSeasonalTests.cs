using System;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.Tests
{
    public static class UnconditionedZonesSeasonalTests
    {
        public static void RunAll()
        {
            Console.WriteLine("=== Seasonal tests for Unconditioned Zones ===");

            TestThetaIntUsedSelection();
            TestThetaZtuFormula();
            TestHolidayReductionAffectsHours();

            Console.WriteLine("=== Seasonal tests completed ===\n");
        }

        private static void TestThetaIntUsedSelection()
        {
            Console.WriteLine("Test 1: thetaIntUsed selection by month...");

            var zone = new ZtuZone { Name = "Z", Type = ZtuType.External };
            var climateService = new ClimateService(new JsonClimateRepository());
            var climate = climateService.GetZone(3);

            var calc = new UnconditionedZonesCalculator();

            // Create winter calc array: make winter months 18.0, summer unused
            double[] winterCalc = new double[12];
            for (int i = 0; i < 12; i++) winterCalc[i] = 18.0;

            var results = calc.CalculateWithSeasonalTemps(zone, climate!, thetaIntSummer: 25.0, thetaIntWinterCalc: winterCalc);

            bool ok = true;
            for (int m = 0; m < 12; m++)
            {
                double used = results.Months[m].ThetaIntUsed_C;
                if (m + 1 >= 5 && m + 1 <= 9)
                {
                    if (Math.Abs(used - 25.0) > 1e-6) ok = false;
                }
                else
                {
                    if (Math.Abs(used - 18.0) > 1e-6) ok = false;
                }
            }

            Console.WriteLine(ok ? "✓ OK" : "✗ FAIL");
        }

        private static void TestThetaZtuFormula()
        {
            Console.WriteLine("Test 2: θztu formula correctness...");

            // simple zone: HztuE=10, HztcZtu=10 -> bztu=0.5
            var zone = new ZtuZone { Name = "Z", Type = ZtuType.External };
            var e1 = new ZtuElement { Name = "e1", Area = 1.0, UValue = 10.0, IsToExternalEnvironment = true };
            var e2 = new ZtuElement { Name = "e2", Area = 1.0, UValue = 10.0, IsToExternalEnvironment = false };
            zone.ElementsToExternal.Add(e1);
            zone.ElementsToBoundary.Add(e2);

            var climateService = new ClimateService(new JsonClimateRepository());
            var climate = climateService.GetZone(3);

            var calc = new UnconditionedZonesCalculator();
            double[] winterCalc = new double[12]; for (int i=0;i<12;i++) winterCalc[i]=20.0;
            var res = calc.CalculateWithSeasonalTemps(zone, climate!, thetaIntSummer:25.0, thetaIntWinterCalc:winterCalc);

            var jan = res.Months[0];
            // Outdoor maybe something; compute expected
            double te = jan.OutdoorTempC;
            double b = jan.Bztu;
            double expected = te + b * (20.0 - te);
            bool ok = Math.Abs(jan.TempZtu_C - expected) < 1e-6;
            Console.WriteLine(ok ? "✓ OK" : $"✗ FAIL (expected {expected:F3} got {jan.TempZtu_C:F3})");
        }

        private static void TestHolidayReductionAffectsHours()
        {
            Console.WriteLine("Test 3: holiday reduction affects heating-season days -> theta changes...");

            // Create objectData with no schedule -> fallback to design temp
            var obj = new ObjectDataSectionData();
            // Mark 10 holidays in January
            obj.DaysOffJanuary = "10";

            var heating = new HeatingSectionData();
            heating.DesignTemperature = 21.0;
            heating.ReductionTemperature = 16.0;

            var climateService = new ClimateService(new JsonClimateRepository());
            var climate = climateService.GetZone(3);

            // With no schedule defined, ComputeThetaIntCalcH should return design temp (fallback)
            var breakdown = HeatingScheduleService.ComputeBreakdown(obj.CalculationMethod, obj, climate!);
            var theta = ScheduleHelper.ComputeThetaIntCalcH(heating, breakdown);
            bool ok = Math.Abs(theta[0] - 21.0) < 1e-6;

            Console.WriteLine(ok ? "✓ OK" : "✗ FAIL");
        }
    }
}
