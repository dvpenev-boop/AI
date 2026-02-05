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

            // Clear default rows and add deterministic ones
            data.HeatingRows.Clear();
            data.HeatingRows.Add(new PumpFanHeatingRow { DeviceType = "Помпи отопление", NominalPower = "100", Quantity = "1" });
            data.HeatingRows.Add(new PumpFanHeatingRow { DeviceType = "Вентилатори", NominalPower = "50", Quantity = "1" });

            var vm = new PumpsAndFansSectionViewModel(data, obj);

            // Totals relation
            Assert.Equal(vm.HeatingTotalAnnualConsumption, vm.HeatingPumpsTotalAnnualConsumption + vm.HeatingFansTotalAnnualConsumption, 6);

            // Each subgroup equals sum of matching rows
            var pumpRow = data.HeatingRows.First(r => !(r.DeviceType ?? string.Empty).Contains("Вентил"));
            var fanRow = data.HeatingRows.First(r => (r.DeviceType ?? string.Empty).Contains("Вентил"));

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

            data.CoolingRows.Clear();
            data.CoolingRows.Add(new PumpFanCoolingRow { DeviceType = "Помпи охлаждане", NominalPower = "200", Quantity = "1" });
            data.CoolingRows.Add(new PumpFanCoolingRow { DeviceType = "Вентилатори (вентилация)", NominalPower = "80", Quantity = "1" });

            var vm = new PumpsAndFansSectionViewModel(data, obj);

            Assert.Equal(vm.CoolingTotalAnnualConsumption, vm.CoolingPumpsTotalAnnualConsumption + vm.CoolingFansTotalAnnualConsumption, 6);

            var pumpRow = data.CoolingRows.First(r => !(r.DeviceType ?? string.Empty).Contains("Вентил"));
            var fanRow = data.CoolingRows.First(r => (r.DeviceType ?? string.Empty).Contains("Вентил"));

            Assert.Equal(vm.CoolingPumpsTotalAnnualConsumption, pumpRow.AnnualConsumption, 6);
            Assert.Equal(vm.CoolingFansTotalAnnualConsumption, fanRow.AnnualConsumption, 6);
        }
    }
}
