using System;
using EE.Doklad.Services;
using Xunit;

namespace EE.Doklad.Tests
{
    /// <summary>
    /// Unit тестове за EquivalentLengthService:
    ///   1) Nearest DN selection
    ///   2) Lequi calculation
    ///   3) Out-of-range warning
    ///   4) da=0 fallback → DN20
    ///   5) Пример: da=0.060, elbows=2, tees=1, valves=1
    /// </summary>
    public class EquivalentLengthServiceTests
    {
        private readonly IEquivalentLengthService _svc = new EquivalentLengthService();

        // ══════════════════════════════════════════════════════════════════════
        // 1. Nearest DN selection
        // ══════════════════════════════════════════════════════════════════════

        [Theory]
        [InlineData(0.013, DnSize.DN8)]
        [InlineData(0.017, DnSize.DN10)]
        [InlineData(0.021, DnSize.DN15)]
        [InlineData(0.027, DnSize.DN20)]
        [InlineData(0.034, DnSize.DN25)]
        [InlineData(0.042, DnSize.DN32)]
        [InlineData(0.048, DnSize.DN40)]
        [InlineData(0.060, DnSize.DN50)]
        public void GetNearestDn_ExactRefDa_ReturnsCorrectDn(double da, DnSize expected)
        {
            Assert.Equal(expected, _svc.GetNearestDn(da));
        }

        [Theory]
        [InlineData(0.014, DnSize.DN8)]    // между DN8(0.013) и DN10(0.017) → DN8
        [InlineData(0.016, DnSize.DN10)]   // по-близо до 0.017
        [InlineData(0.025, DnSize.DN20)]   // |0.025-0.021|=0.004 vs |0.025-0.027|=0.002 → DN20
        [InlineData(0.039, DnSize.DN32)]   // |0.039-0.034|=0.005 vs |0.039-0.042|=0.003 → DN32
        [InlineData(0.055, DnSize.DN50)]   // между DN40(0.048) и DN50(0.060)
        [InlineData(0.100, DnSize.DN50)]   // далеч извън обхвата → DN50 (най-близък)
        public void GetNearestDn_IntermediateValues_ReturnsBestMatch(double da, DnSize expected)
        {
            Assert.Equal(expected, _svc.GetNearestDn(da));
        }

        // ══════════════════════════════════════════════════════════════════════
        // 2. da=0 fallback → DN20
        // ══════════════════════════════════════════════════════════════════════

        [Theory]
        [InlineData(0.0)]
        [InlineData(-0.01)]
        [InlineData(-5.0)]
        public void GetNearestDn_ZeroOrNegative_ReturnsDN20(double da)
        {
            Assert.Equal(DnSize.DN20, _svc.GetNearestDn(da));
        }

        // ══════════════════════════════════════════════════════════════════════
        // 3. Lequi calculation
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void CalcLequi_ZeroCounts_ReturnsZero()
        {
            double result = _svc.CalcLequi(0.060, 0, 0, 0);
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void CalcLequi_SingleElbow90_DN50()
        {
            // DN50: elbow90 = 0.48 m
            double result = _svc.CalcLequi(0.060, 1, 0, 0);
            Assert.Equal(0.48, result, precision: 2);
        }

        [Fact]
        public void CalcLequi_SingleTeeBranch_DN20()
        {
            // da=0.027 → DN20, tee = 0.49 m
            double result = _svc.CalcLequi(0.027, 0, 1, 0);
            Assert.Equal(0.49, result, precision: 2);
        }

        [Fact]
        public void CalcLequi_SingleBallValve_DN15()
        {
            // da=0.021 → DN15, valve = 0.34 m
            double result = _svc.CalcLequi(0.021, 0, 0, 1);
            Assert.Equal(0.34, result, precision: 2);
        }

        [Fact]
        public void CalcLequi_NegativeCounts_TreatedAsZero()
        {
            // Negative counts → clamped to 0
            double result = _svc.CalcLequi(0.060, -3, -1, -2);
            Assert.Equal(0.0, result);
        }

        // ══════════════════════════════════════════════════════════════════════
        // 4. Пример: da=0.060, elbows=2, tees=1, valves=1  → DN50
        //    Lequi = 2*0.48 + 1*1.08 + 1*0.96 = 0.96 + 1.08 + 0.96 = 3.00
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void CalcLequi_Example_Da060_2Elbows_1Tee_1Valve()
        {
            // da=0.060 → DN50
            // Lequi = 2 * 0.48 + 1 * 1.08 + 1 * 0.96
            //       = 0.96 + 1.08 + 0.96 = 3.00 m
            double result = _svc.CalcLequi(0.060, 2, 1, 1);
            Assert.Equal(3.00, result, precision: 2);
        }

        [Fact]
        public void CalcLequiDetailed_Example_Da060_2Elbows_1Tee_1Valve()
        {
            var detail = _svc.CalcLequiDetailed(0.060, 2, 1, 1);

            Assert.Equal(3.00, detail.Lequi, precision: 2);
            Assert.Equal(DnSize.DN50, detail.Dn);
            Assert.Equal(0.060, detail.DnRefDa);
            Assert.False(detail.IsOutOfRange); // exact match
            Assert.Equal(0.0, detail.DaDeviation, precision: 4);
        }

        // ══════════════════════════════════════════════════════════════════════
        // 5. Out-of-range warning
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void CalcLequiDetailed_DaFarFromAnyDn_IsOutOfRange_True()
        {
            // da=0.100 → nearest DN50 (ref=0.060), deviation = 0.040 > 0.010
            var detail = _svc.CalcLequiDetailed(0.100, 1, 0, 0);

            Assert.Equal(DnSize.DN50, detail.Dn);
            Assert.True(detail.IsOutOfRange);
            Assert.Equal(0.040, detail.DaDeviation, precision: 3);
        }

        [Fact]
        public void CalcLequiDetailed_DaCloseEnough_IsOutOfRange_False()
        {
            // da=0.065 → DN50 (ref=0.060), deviation = 0.005 < 0.010
            var detail = _svc.CalcLequiDetailed(0.065, 1, 0, 0);

            Assert.Equal(DnSize.DN50, detail.Dn);
            Assert.False(detail.IsOutOfRange);
            Assert.Equal(0.005, detail.DaDeviation, precision: 3);
        }

        [Fact]
        public void CalcLequiDetailed_DaZero_FallbackDN20_NoOutOfRange()
        {
            // da=0 → fallback DN20, IsOutOfRange = false (special case: da>0 check)
            var detail = _svc.CalcLequiDetailed(0.0, 1, 1, 1);

            Assert.Equal(DnSize.DN20, detail.Dn);
            Assert.False(detail.IsOutOfRange);
            // Lequi = 1*0.22 + 1*0.49 + 1*0.43 = 1.14
            Assert.Equal(1.14, detail.Lequi, precision: 2);
        }

        // ══════════════════════════════════════════════════════════════════════
        // 6. DN8 – edge case (smallest DN)
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void CalcLequi_DN8_AllFittings()
        {
            // da=0.013 → DN8
            // 3 elbows: 3*0.10 = 0.30
            // 2 tees:   2*0.23 = 0.46
            // 1 valve:  1*0.21 = 0.21
            // Total = 0.97
            double result = _svc.CalcLequi(0.013, 3, 2, 1);
            Assert.Equal(0.97, result, precision: 2);
        }

        // ══════════════════════════════════════════════════════════════════════
        // 7. Boundary between DN40 and DN50
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void GetNearestDn_Boundary_DN40_DN50()
        {
            // DN40 ref=0.048, DN50 ref=0.060
            // Midpoint = 0.054 → by abs distance: |0.054-0.048|=0.006 vs |0.054-0.060|=0.006
            // Tie-break: depends on iteration order; either is acceptable
            var dn = _svc.GetNearestDn(0.054);
            Assert.True(dn == DnSize.DN40 || dn == DnSize.DN50,
                $"At midpoint 0.054 expected DN40 or DN50, got {dn}");
        }
    }
}
