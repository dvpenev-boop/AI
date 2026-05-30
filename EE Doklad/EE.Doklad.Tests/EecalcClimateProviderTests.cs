using EE.Doklad.Services.EecalcClimate;
using Xunit;

namespace EE.Doklad.Tests
{
    public sealed class EecalcClimateProviderTests
    {
        [Fact]
        public void LegacyEECalcStrict_ReturnsXmlValues()
        {
            IClimateDataProvider provider = new LegacyEecalcXmlClimateDataProvider(
                ClimateProviderMode.LegacyEECalcStrict);

            Assert.Equal(-1.9, provider.GetMonthlyAvgTemp(1, Month.January), 6);
            Assert.Equal(-0.5, provider.GetMonthlyAvgTemp(2, Month.January), 6);
            Assert.Equal(-0.1, provider.GetMonthlyAvgTemp(3, Month.January), 6);
            Assert.Equal(101000, provider.GetPb(1), 6);

            var hourly = provider.GetHourlyClimateData(1, Month.January);
            Assert.Equal(24, hourly.Count);
            Assert.Equal(0, hourly[0].Hour);
            Assert.Equal(2.8, hourly[0].Temperature, 6);
            Assert.Equal(82, hourly[0].Humidity, 6);
        }

        [Fact]
        public void LegacyEECalcCorrectedData_CorrectsJanuaryValues()
        {
            IClimateDataProvider provider = new LegacyEecalcXmlClimateDataProvider(
                ClimateProviderMode.LegacyEECalcCorrectedData);

            Assert.Equal(1.9, provider.GetMonthlyAvgTemp(1, Month.January), 6);
            Assert.Equal(0.5, provider.GetMonthlyAvgTemp(2, Month.January), 6);
            Assert.Equal(0.1, provider.GetMonthlyAvgTemp(3, Month.January), 6);

            Assert.Equal(2.7, provider.GetMonthlyAvgTemp(1, Month.February), 6);
            Assert.Equal(22.9, provider.GetSolarRadiation(1, Month.January).N, 6);
        }

        [Fact]
        public void CurrentOrdinance_ReturnsJsonValues()
        {
            IClimateDataProvider provider = new CorrectedJsonClimateDataProvider();

            Assert.Equal(1.9, provider.GetMonthlyAvgTemp(1, Month.January), 6);
            Assert.Equal(0.5, provider.GetMonthlyAvgTemp(2, Month.January), 6);
            Assert.Equal(0.1, provider.GetMonthlyAvgTemp(3, Month.January), 6);

            var solar = provider.GetSolarRadiation(1, Month.January);
            Assert.Equal(22.9, solar.N, 6);
            Assert.Equal(40.4, solar.E, 6);
            Assert.Equal(72.7, solar.S, 6);
            Assert.Equal(40.4, solar.W, 6);
            Assert.Equal(50.1, solar.H, 6);

            var hourly = provider.GetHourlyClimateData(1, Month.January);
            Assert.Equal(24, hourly.Count);
            Assert.Equal(0, hourly[0].Hour);
            Assert.Equal(1.9, hourly[0].Temperature, 6);
            Assert.Equal(50.0, hourly[0].Humidity, 6);
        }

        [Fact]
        public void OrientationMapping_ReturnsExpectedValues()
        {
            IClimateDataProvider provider = new LegacyEecalcXmlClimateDataProvider(
                ClimateProviderMode.LegacyEECalcStrict);

            var solar = provider.GetSolarRadiation(1, Month.January);

            Assert.Equal(22.9, solar.N, 6);
            Assert.Equal(39.4, solar.E, 6);
            Assert.Equal(70.1, solar.S, 6);
            Assert.Equal(39.4, solar.W, 6);
            Assert.Equal(49.6, solar.H, 6);
            Assert.Equal((22.9 + 39.4) / 2.0, solar.NE, 6);
            Assert.Equal((70.1 + 39.4) / 2.0, solar.SE, 6);
            Assert.Equal((70.1 + 39.4) / 2.0, solar.SW, 6);
            Assert.Equal((22.9 + 39.4) / 2.0, solar.NW, 6);
        }
    }
}
