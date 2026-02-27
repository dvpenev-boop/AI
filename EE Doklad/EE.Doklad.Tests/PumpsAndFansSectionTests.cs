using System.Linq;
using EE.Doklad.Models;
using EE.Doklad.ViewModels;
using Xunit;

namespace EE.Doklad.Tests
{
    public class PumpsAndFansSectionTests
    {
        [Fact]
        public void Heating_PumpsAndFans_AreSplitAndSumsMatch()
        {
            var data = new PumpsAndFansSectionData();
            var obj = new ObjectDataSectionData
            {
                HeatedArea = "100",
                ClimateZone = 1,
                HeatingWorkdaysHours = "8",
                HeatingSaturdayHours = "0",
                HeatingSundayHours = "0",
                VentilationWorkdaysHours = "8",
                VentilationSaturdayHours = "0",
                VentilationSundayHours = "0"
            };

            // Add deterministic rows directly to split collections
            data.HeatingPumpRows.Add(new PumpFanHeatingRow { DeviceType = "Помпи отопление", NominalPower = "100", Quantity = "1" });
            data.HeatingFanRows.Add(new PumpFanHeatingRow { DeviceType = "Вентилатори", NominalPower = "50", Quantity = "1" });

            var vm = new PumpsAndFansSectionViewModel(data, obj);

            // Totals relation
            Assert.Equal(vm.HeatingTotalAnnualConsumption, vm.HeatingPumpsTotalAnnualConsumption + vm.HeatingFansTotalAnnualConsumption, 6);

            // Each subgroup equals sum of matching rows
            var pumpRow = data.HeatingPumpRows.First();
            var fanRow = data.HeatingFanRows.First();

            Assert.Equal(vm.HeatingPumpsTotalAnnualConsumption, pumpRow.AnnualConsumption, 6);
            Assert.Equal(vm.HeatingFansTotalAnnualConsumption, fanRow.AnnualConsumption, 6);
        }

        [Fact]
        public void Cooling_PumpsAndFans_AreSplitAndSumsMatch()
        {
            var data = new PumpsAndFansSectionData();
            var obj = new ObjectDataSectionData
            {
                HeatedArea = "100",
                ClimateZone = 1,
                CoolingWorkdaysHours = "8",
                CoolingSaturdayHours = "0",
                CoolingSundayHours = "0",
                VentilationWorkdaysHours = "8",
                VentilationSaturdayHours = "0",
                VentilationSundayHours = "0"
            };

            data.CoolingPumpRows.Add(new PumpFanCoolingRow { DeviceType = "Помпи охлаждане", NominalPower = "200", Quantity = "1" });
            data.CoolingFanRows.Add(new PumpFanCoolingRow { DeviceType = "Вентилатори (вентилация)", NominalPower = "80", Quantity = "1" });

            var vm = new PumpsAndFansSectionViewModel(data, obj);

            Assert.Equal(vm.CoolingTotalAnnualConsumption, vm.CoolingPumpsTotalAnnualConsumption + vm.CoolingFansTotalAnnualConsumption, 6);

            var pumpRow = data.CoolingPumpRows.First();
            var fanRow = data.CoolingFanRows.First();

            Assert.Equal(vm.CoolingPumpsTotalAnnualConsumption, pumpRow.AnnualConsumption, 6);
            Assert.Equal(vm.CoolingFansTotalAnnualConsumption, fanRow.AnnualConsumption, 6);
        }
    }
}
