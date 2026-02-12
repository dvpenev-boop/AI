using System;
using System.Linq;
using EE.Doklad.Models;
using EE.Doklad.Services;
using Xunit;

namespace EE.Doklad.Tests
{
    public class VentCoolingCalculatorMonthlyTests
    {
        [Fact]
        public void FreshAirProcessed_EnthalpyDifference_Produces_Latent_Component()
        {
            var data = new VentilationSectionData
            {
                AirflowRatePerM2 = 5.0,
                SupplyTemperature = 18.0,
                RelativeHumidity = 90.0,
                CoolingCalculationMode = VentilationCoolingCalculationMode.FreshAirProcessed3113
            };

            var objectData = new ObjectDataSectionData
            {
                CoolingSeasonEnabled = true,
                CoolingSeasonStartDay = 1,
                CoolingSeasonStartMonth = 7,
                CoolingSeasonEndDay = 31,
                CoolingSeasonEndMonth = 7,
                VentilationCoolingWorkdaysHours = "10",
                VentilationCoolingSaturdayHours = "0",
                VentilationCoolingSundayHours = "0",
                CooledArea = "100"
            };

            var climate = new ClimateZoneData
            {
                Id = 1,
                Name = "Test",
                Monthly = new MonthlyClimateData
                {
                    AvgMonthlyTempC = new double[12],
                    AvgMonthlyRelHumidityPercentMayToSep = new[] { 60.0, 60.0, 60.0, 60.0, 60.0 }
                }
            };
            climate.Monthly.AvgMonthlyTempC[6] = 30.0; // July

            var cooling = new CoolingSectionData { DesignTemperature = 24.0 };
            var calc = new VentCoolingCalculatorMonthly();

            var output = calc.Calculate(data, objectData, climate, cooling, 2024);
            var july = output.Debug.Months.Single(m => m.MonthNumber == 7);

            Assert.True(july.Q_lat_kWh > 0.0);
            Assert.InRange(july.Q_total_kWh, july.Q_sens_kWh + july.Q_lat_kWh - 0.01, july.Q_sens_kWh + july.Q_lat_kWh + 0.01);
        }
    }
}
