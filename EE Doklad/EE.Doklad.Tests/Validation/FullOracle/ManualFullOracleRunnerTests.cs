using Xunit;

namespace EE.Doklad.Tests.Validation.FullOracle
{
    public sealed class ManualFullOracleRunnerTests
    {
        [Fact]
        public void ManualFullOracleRunner_HeatingOnly_ReturnsReadableResult()
        {
            var input = new ManualEecalcInput
            {
                ClimateZoneId = 7,
                HeatedArea = 1000,
                HeatedVolume = 2500,
                HeatCapacity = 46,
                MetabolicHeat = 3.16,
                LatentMetabolicHeat = 0.84,
                HeatingStartDay = 15,
                HeatingStartMonth = 10,
                HeatingEndDay = 23,
                HeatingEndMonth = 4,
                Infiltration = 0.5,
                ProjectTemperature = 20,
                NonProjectTemperature = 16,
                HasCooling = false,
                HasMechanicalVentilation = false,
                IsBgvUsed = false,
                HasLighting = false,
                HasDevices = false
            };

            var result = ManualFullOracleRunner.Calculate(input);
            var text = result.ToReadableText();

            Assert.NotNull(result);
            Assert.NotNull(result.HeatingBreakdown);
            Assert.Contains("Heating", text);
            Assert.Contains("Qtr", text);
            Assert.Contains("Qve", text);
            Assert.Contains("Final Qnd", text);
        }
    }
}
