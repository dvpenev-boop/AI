using System;
using EE.Doklad.Models;
using EE.Doklad.Sections.Section11Heating.Services;
using Xunit;

namespace EE.Doklad.Tests
{
    public class HeatingCalculationServiceTests
    {
        private readonly HeatingCalculationService _service = new();

        [Fact]
        public void LegacyCase_NoSolar_QH_EqualsQht()
        {
            var walls = new[] { new WallData(350.0, 1.078, 0.0, 0.0, "N") };
            var windows = Array.Empty<WindowData>();
            var roofs = Array.Empty<RoofData>();

            var (_, annual) = _service.Calculate(
                HeatingCalculationMethod.AuerSoftware,
                walls,
                windows,
                roofs,
                htr: 377.3,
                hve: 425.0,
                cm: 45830.0,
                thetaI: 20.0,
                area: 1000.0,
                climateZone: 7,
                heatingMonths: new[] { 10 },
                getQint: _ => 0.0);

            Assert.True(annual.IsValid);
            Assert.True(annual.QH_per_m2 > 0.0);
            Assert.InRange(Math.Abs(annual.Qht_total - annual.QH_total_kWh), 0.0, 1.0);
        }

        [Fact]
        public void AuerVsRd_SameWindow_DifferentQgn()
        {
            var windows = new[] { new WindowData(200.0, 1.4, 0.6, 0.8, "N") };
            var walls = Array.Empty<WallData>();
            var roofs = Array.Empty<RoofData>();
            var months = new[] { 10 };

            var (_, auer) = _service.Calculate(
                HeatingCalculationMethod.AuerSoftware,
                walls,
                windows,
                roofs,
                htr: 280.0,
                hve: 0.0,
                cm: 45830.0,
                thetaI: 20.0,
                area: 1000.0,
                climateZone: 7,
                heatingMonths: months,
                getQint: _ => 0.0);

            var (_, rd) = _service.Calculate(
                HeatingCalculationMethod.Rd0220_3,
                walls,
                windows,
                roofs,
                htr: 280.0,
                hve: 0.0,
                cm: 45830.0,
                thetaI: 20.0,
                area: 1000.0,
                climateZone: 7,
                heatingMonths: months,
                getQint: _ => 0.0);

            Assert.True(auer.IsValid);
            Assert.True(rd.IsValid);
            Assert.True(auer.Qgn_total > rd.Qgn_total);
            Assert.True(auer.QH_total_kWh < rd.QH_total_kWh);
        }

        [Fact]
        public void MethodChange_OnlySolarDiffers_HtrHveUnchanged()
        {
            const double htr = 1000.0;
            const double hve = 425.0;
            const double cm = 45830.0;

            var (_, auer) = _service.Calculate(
                HeatingCalculationMethod.AuerSoftware,
                Array.Empty<WallData>(),
                Array.Empty<WindowData>(),
                Array.Empty<RoofData>(),
                htr,
                hve,
                cm,
                thetaI: 20.0,
                area: 1000.0,
                climateZone: 7,
                heatingMonths: new[] { 10 },
                getQint: _ => 0.0);

            var (_, rd) = _service.Calculate(
                HeatingCalculationMethod.Rd0220_3,
                Array.Empty<WallData>(),
                Array.Empty<WindowData>(),
                Array.Empty<RoofData>(),
                htr,
                hve,
                cm,
                thetaI: 20.0,
                area: 1000.0,
                climateZone: 7,
                heatingMonths: new[] { 10 },
                getQint: _ => 0.0);

            Assert.InRange(Math.Abs(auer.Tau - rd.Tau), 0.0, 0.001);
            Assert.InRange(Math.Abs(auer.AH - rd.AH), 0.0, 0.001);
            Assert.InRange(Math.Abs(auer.Htr - rd.Htr), 0.0, 0.001);
            Assert.InRange(Math.Abs(auer.Hve - rd.Hve), 0.0, 0.001);
        }

        [Fact]
        public void Ashrae_Placeholder_ReturnsInvalidResult()
        {
            var (_, result) = _service.Calculate(
                HeatingCalculationMethod.Ashrae8760,
                Array.Empty<WallData>(),
                Array.Empty<WindowData>(),
                Array.Empty<RoofData>(),
                htr: 280.0,
                hve: 425.0,
                cm: 45830.0,
                thetaI: 20.0,
                area: 1000.0,
                climateZone: 7,
                heatingMonths: new[] { 10 },
                getQint: _ => 0.0);

            Assert.False(result.IsValid);
            Assert.False(string.IsNullOrWhiteSpace(result.ErrorMessage));
        }
    }
}
