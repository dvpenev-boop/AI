using System;
using System.Collections.ObjectModel;
using EE.Doklad.Models;
using EE.Doklad.Services;
using Xunit;

namespace EE.Doklad.Tests
{
    /// <summary>
    /// Unit тестове за DhwDistributionLossService.
    ///
    /// Сценарии:
    ///   1) Режим A (Manual):   стойността се пази без промяна
    ///   2) Режим C (PercentShare): Q_rbl = Q_total * p/100
    ///   3) Режим B (Automatic): сегменти кондиционирана/некондиционирана – f_rbl коректен
    ///   4) Режим B: само кондиционирани сегменти → f_rbl = 1.0
    ///   5) Режим B: само некондиционирани сегменти → f_rbl = 0.0
    ///   6) Формула (1.3): Ψ за изолирана тръба
    ///   7) Формула (1.6): Ψ приближение за неизолирана тръба
    ///   8) Формула (1.4): Ψ за вградена тръба
    ///   9) Guard clauses – нулеви/невалидни входни данни
    /// </summary>
    public class DhwDistributionLossServiceTests
    {
        private readonly IDhwDistributionLossService _svc = new DhwDistributionLossService();

        // ══════════════════════════════════════════════════════════════════════
        // Сценарий 1: Режим A – Ръчно въвеждане
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void Manual_Mode_Returns_Exactly_ManualValue_Without_Calculation()
        {
            // Arrange
            var inputs = new DhwLossInputs
            {
                ManualRecoverableLoss_kWh = 800.0
            };

            // Act
            var result = _svc.Calculate(inputs, DhwLossMode.Manual);

            // Assert
            Assert.Equal(DhwLossMode.Manual, result.Mode);
            Assert.Equal(800.0, result.Q_rbl_year, precision: 2);
            // При ръчен режим не трябва да се изчислява f_rbl
            Assert.Equal(0.0, result.F_rbl, precision: 4);
            // Q_total не е изчислено
            Assert.Equal(0.0, result.Q_total, precision: 4);
        }

        [Fact]
        public void Manual_Mode_With_Zero_Returns_Zero()
        {
            var inputs = new DhwLossInputs { ManualRecoverableLoss_kWh = 0.0 };
            var result = _svc.Calculate(inputs, DhwLossMode.Manual);
            Assert.Equal(0.0, result.Q_rbl_year, precision: 2);
        }

        [Fact]
        public void Manual_Mode_Negative_Value_Is_Clamped_To_Zero()
        {
            var inputs = new DhwLossInputs { ManualRecoverableLoss_kWh = -100.0 };
            var result = _svc.Calculate(inputs, DhwLossMode.Manual);
            Assert.Equal(0.0, result.Q_rbl_year, precision: 2);
        }

        // ══════════════════════════════════════════════════════════════════════
        // Сценарий 2: Режим C – % дял
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void PercentShare_Mode_Calculates_Q_rbl_From_Percent_And_Total()
        {
            // Arrange
            // Q_rbl = 2000 kWh * 40% / 100 = 800 kWh
            var inputs = new DhwLossInputs
            {
                TotalSystemLosses_kWh = 2000.0,
                PercentShare          = 40.0
            };

            // Act
            var result = _svc.Calculate(inputs, DhwLossMode.PercentShare);

            // Assert
            Assert.Equal(DhwLossMode.PercentShare, result.Mode);
            Assert.Equal(800.0, result.Q_rbl_year, precision: 2);
            Assert.Equal(0.40, result.F_rbl, precision: 4);
        }

        [Fact]
        public void PercentShare_Mode_With_Zero_Percent_Returns_Zero()
        {
            var inputs = new DhwLossInputs
            {
                TotalSystemLosses_kWh = 2000.0,
                PercentShare          = 0.0
            };
            var result = _svc.Calculate(inputs, DhwLossMode.PercentShare);
            Assert.Equal(0.0, result.Q_rbl_year, precision: 2);
        }

        [Fact]
        public void PercentShare_Mode_Clamps_Percent_To_100()
        {
            // 120% → трябва да се clamp до 100%
            var inputs = new DhwLossInputs
            {
                TotalSystemLosses_kWh = 1000.0,
                PercentShare          = 120.0
            };
            var result = _svc.Calculate(inputs, DhwLossMode.PercentShare);
            Assert.Equal(1000.0, result.Q_rbl_year, precision: 2);
        }

        // ══════════════════════════════════════════════════════════════════════
        // Сценарий 3: Режим B – Автоматично с 2 сегмента (mixed zones)
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void Automatic_Mode_TwoSegments_Mixed_Zones_FrblIsCorrect()
        {
            // Arrange
            // Сегмент 1: Кондиционирана зона, L=10m, L_equi=0, Ψ=0.5 W/mK
            // Сегмент 2: Некондиционирана зона, L=10m, L_equi=0, Ψ=0.5 W/mK
            //
            // t_year = 251 * 24 = 6024 h (t_op = 6024, t_nom = 0)
            // θ_w = 55, θ_amb_cond = 20, θ_amb_uncond = 12
            //
            // Q_ls_cond  = (1/1000) * 0.5 * (55-20) * 10 * 6024 = 1054.2 kWh
            // Q_ls_uncond = (1/1000) * 0.5 * (55-12) * 10 * 6024 = 1295.16 kWh
            //
            // Q_total = 1054.2 + 1295.16 = 2349.36 kWh
            // Q_cond  = 1054.2 kWh
            // f_rbl   = 1054.2 / 2349.36 ≈ 0.4487

            var inputs = new DhwLossInputs
            {
                WorkingDaysPerYear             = 251,
                OperatingHours_hPerYear        = 0,   // 0 = t_year
                HotWaterTemperature_degC       = 55,
                AmbientTempConditioned_degC    = 20,
                AmbientTempUnconditioned_degC  = 12,
                PipeSegments = new ObservableCollection<PipeSegment>
                {
                    new PipeSegment
                    {
                        Name           = "Кондиционирана",
                        ZoneType       = PipeZoneType.Conditioned,
                        Length_m       = 10.0,
                        EquivalentLength_m = 0.0,
                        InsulationType = PipeInsulationType.DirectPsi,
                        Psi_WmK        = 0.5
                    },
                    new PipeSegment
                    {
                        Name           = "Некондиционирана",
                        ZoneType       = PipeZoneType.Unconditioned,
                        Length_m       = 10.0,
                        EquivalentLength_m = 0.0,
                        InsulationType = PipeInsulationType.DirectPsi,
                        Psi_WmK        = 0.5
                    }
                }
            };

            // Act
            var result = _svc.Calculate(inputs, DhwLossMode.Automatic);

            // Assert
            Assert.Equal(DhwLossMode.Automatic, result.Mode);

            double tYear = 251 * 24.0;  // 6024 h
            double qLsCond   = (1.0 / 1000.0) * 0.5 * (55 - 20) * 10 * tYear;
            double qLsUncond = (1.0 / 1000.0) * 0.5 * (55 - 12) * 10 * tYear;
            double qTotal    = qLsCond + qLsUncond;
            double fRblExp   = qLsCond / qTotal;

            Assert.Equal(Math.Round(qTotal, 2), result.Q_total, precision: 1);
            Assert.Equal(Math.Round(fRblExp, 4), result.F_rbl, precision: 4);
            Assert.Equal(Math.Round(fRblExp * qTotal, 2), result.Q_rbl_year, precision: 1);

            // Проверяваме, че f_rbl е между 0 и 1
            Assert.InRange(result.F_rbl, 0.0, 1.0);

            // f_rbl трябва да е около 0.4487 (35/78 ≈ 0.4487)
            Assert.True(result.F_rbl > 0.4 && result.F_rbl < 0.5,
                $"Очакван f_rbl ≈ 0.4487, получен {result.F_rbl}");
        }

        // ══════════════════════════════════════════════════════════════════════
        // Сценарий 4: Само кондиционирани → f_rbl = 1.0
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void Automatic_Mode_AllConditioned_FrblIsOne()
        {
            var inputs = new DhwLossInputs
            {
                WorkingDaysPerYear            = 200,
                HotWaterTemperature_degC      = 55,
                AmbientTempConditioned_degC   = 20,
                PipeSegments = new ObservableCollection<PipeSegment>
                {
                    new PipeSegment
                    {
                        ZoneType       = PipeZoneType.Conditioned,
                        Length_m       = 15.0,
                        InsulationType = PipeInsulationType.DirectPsi,
                        Psi_WmK        = 0.4
                    },
                    new PipeSegment
                    {
                        ZoneType       = PipeZoneType.Conditioned,
                        Length_m       = 8.0,
                        InsulationType = PipeInsulationType.DirectPsi,
                        Psi_WmK        = 0.6
                    }
                }
            };

            var result = _svc.Calculate(inputs, DhwLossMode.Automatic);

            Assert.Equal(1.0, result.F_rbl, precision: 4);
            Assert.Equal(result.Q_total, result.Q_rbl_year, precision: 2);
        }

        // ══════════════════════════════════════════════════════════════════════
        // Сценарий 5: Само некондиционирани → f_rbl = 0.0
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void Automatic_Mode_AllUnconditioned_FrblIsZero()
        {
            var inputs = new DhwLossInputs
            {
                WorkingDaysPerYear              = 200,
                HotWaterTemperature_degC        = 55,
                AmbientTempUnconditioned_degC   = 12,
                PipeSegments = new ObservableCollection<PipeSegment>
                {
                    new PipeSegment
                    {
                        ZoneType       = PipeZoneType.Unconditioned,
                        Length_m       = 12.0,
                        InsulationType = PipeInsulationType.DirectPsi,
                        Psi_WmK        = 0.5
                    }
                }
            };

            var result = _svc.Calculate(inputs, DhwLossMode.Automatic);

            Assert.Equal(0.0, result.F_rbl, precision: 4);
            Assert.Equal(0.0, result.Q_rbl_year, precision: 2);
            Assert.True(result.Q_total > 0, "Q_total трябва да е > 0 въпреки f_rbl = 0");
        }

        // ══════════════════════════════════════════════════════════════════════
        // Сценарий 6: Формула (1.3) – Ψ за изолирана тръба
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void ComputePsi_InsulatedInAir_Formula_1_3_IsCorrect()
        {
            // Ψ = π / ( (1/(2·λ_D))·ln(d_a/d_i) + 1/(h_a·d_a) )
            // d_i = 0.020 m, d_a = 0.060 m, λ_D = 0.04, h_a = 10
            // (1/(2*0.04))*ln(3) + 1/(10*0.060)
            // = 12.5 * 1.0986 + 1.6667
            // = 13.7325 + 1.6667 = 15.3992
            // Ψ = π / 15.3992 ≈ 0.2038

            var seg = new PipeSegment
            {
                InsulationType                = PipeInsulationType.InsulatedInAir,
                InnerDiameter_m              = 0.020,
                OuterDiameterWithInsulation_m = 0.060,
                InsulationLambda_WmK         = 0.04,
                SurfaceHeatTransfer_WmK      = 10.0
            };

            double psi = _svc.ComputePsi(seg);

            double expected = Math.PI / ((1.0 / (2 * 0.04)) * Math.Log(0.060 / 0.020) + 1.0 / (10 * 0.060));
            Assert.Equal(expected, psi, precision: 5);
            Assert.True(psi > 0.1 && psi < 0.5, $"Неочаквана Ψ стойност: {psi}");
        }

        // ══════════════════════════════════════════════════════════════════════
        // Сценарий 7: Формула (1.6) – Ψ приближение за неизолирана тръба
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void ComputePsi_Uninsulated_Approximate_Formula_1_6_IsCorrect()
        {
            // Ψ ≈ h_a · π · d_p,a = 10 * π * 0.025 ≈ 0.7854

            var seg = new PipeSegment
            {
                InsulationType               = PipeInsulationType.Uninsulated,
                PipeOuterDiameter_m          = 0.025,
                SurfaceHeatTransfer_WmK      = 10.0,
                UseApproximatePsiForUninsulated = true
            };

            double psi      = _svc.ComputePsi(seg);
            double expected = 10.0 * Math.PI * 0.025;

            Assert.Equal(expected, psi, precision: 6);
        }

        // ══════════════════════════════════════════════════════════════════════
        // Сценарий 8: Формула (1.4) – Ψ за вградена тръба
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void ComputePsi_EmbeddedInMaterial_Formula_1_4_IsCorrect()
        {
            // Ψ = π / ( 0.5 · ( (1/λ_D)·ln(d_a/d_i) + (1/λ_em)·ln(4z/d_a) ) )
            // d_i=0.020, d_a=0.060, λ_D=0.04, λ_em=1.5, z=0.05
            // term1 = (1/0.04) * ln(3) = 25 * 1.0986 = 27.466
            // term2 = (1/1.5) * ln(4*0.05/0.06) = 0.6667 * ln(3.333) = 0.6667 * 1.2040 = 0.8027
            // denom = 0.5 * (27.466 + 0.8027) = 14.134
            // Ψ = π / 14.134 ≈ 0.2221

            var seg = new PipeSegment
            {
                InsulationType               = PipeInsulationType.EmbeddedInMaterial,
                InnerDiameter_m              = 0.020,
                OuterDiameterWithInsulation_m = 0.060,
                InsulationLambda_WmK         = 0.04,
                EmbeddingMaterialLambda_WmK  = 1.5,
                DepthFromSurface_m           = 0.05
            };

            double psi = _svc.ComputePsi(seg);

            double term1  = (1.0 / 0.04) * Math.Log(0.060 / 0.020);
            double term2  = (1.0 / 1.5)  * Math.Log(4 * 0.05 / 0.060);
            double expected = Math.PI / (0.5 * (term1 + term2));

            Assert.Equal(expected, psi, precision: 5);
            Assert.True(psi > 0.0, "Ψ трябва да е > 0");
        }

        // ══════════════════════════════════════════════════════════════════════
        // Сценарий 9: Guard clauses – липсващи сегменти
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void Automatic_Mode_No_Segments_Returns_Zero()
        {
            var inputs = new DhwLossInputs
            {
                WorkingDaysPerYear       = 251,
                HotWaterTemperature_degC = 55,
                PipeSegments = new ObservableCollection<PipeSegment>() // празна колекция
            };

            var result = _svc.Calculate(inputs, DhwLossMode.Automatic);

            Assert.Equal(0.0, result.Q_rbl_year, precision: 2);
            Assert.Equal(0.0, result.Q_total, precision: 2);
            Assert.Equal(0.0, result.F_rbl, precision: 4);
        }

        [Fact]
        public void Calculate_NullInputs_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _svc.Calculate(null!, DhwLossMode.Manual));
        }

        // ══════════════════════════════════════════════════════════════════════
        // Сценарий 10: Stub загуби се включват в Q_total
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void Automatic_Mode_Stub_Losses_Are_Added_To_Total()
        {
            // 1 сегмент (некондициониран) + 1 stub (кондициониран)
            // Stub загубата трябва да влезе в Q_cond → f_rbl > 0

            var inputs = new DhwLossInputs
            {
                WorkingDaysPerYear             = 200,
                OperatingHours_hPerYear        = 200 * 24.0,
                HotWaterTemperature_degC       = 55,
                AmbientTempConditioned_degC    = 20,
                AmbientTempUnconditioned_degC  = 12,
                WaterDensity_kgm3              = 1000,
                WaterHeatCapacity_kWhkgK       = 0.001163,
                PipeSegments = new ObservableCollection<PipeSegment>
                {
                    new PipeSegment
                    {
                        ZoneType       = PipeZoneType.Unconditioned,
                        Length_m       = 10.0,
                        InsulationType = PipeInsulationType.DirectPsi,
                        Psi_WmK        = 0.5
                    }
                },
                StubZones = new ObservableCollection<StubZoneData>
                {
                    new StubZoneData
                    {
                        ZoneType               = PipeZoneType.Conditioned,
                        StubVolume_m3          = 0.001,   // 1 литър
                        TappingFrequency_perHour = 1.0
                    }
                }
            };

            var result = _svc.Calculate(inputs, DhwLossMode.Automatic);

            // Stub загубата е кондиционирана → f_rbl > 0
            Assert.True(result.Q_dis_stub > 0, "Q_stub трябва да е > 0");
            Assert.True(result.F_rbl > 0.0, "f_rbl трябва да е > 0 заради stub в кондиционирана зона");
            Assert.True(result.Q_total > result.Q_dis_ls, "Q_total трябва да е по-голямо от Q_ls");
        }

        // ══════════════════════════════════════════════════════════════════════
        // Сценарий 11: t_op = 0 (24/7 работа) – t_year трябва да се ползва
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void Automatic_Mode_ZeroOperatingHours_UsesFullYear()
        {
            var inputs = new DhwLossInputs
            {
                WorkingDaysPerYear        = 251,
                OperatingHours_hPerYear   = 0, // 0 означава "ползвай t_year"
                HotWaterTemperature_degC  = 55,
                AmbientTempConditioned_degC = 20,
                PipeSegments = new ObservableCollection<PipeSegment>
                {
                    new PipeSegment
                    {
                        ZoneType       = PipeZoneType.Conditioned,
                        Length_m       = 10.0,
                        InsulationType = PipeInsulationType.DirectPsi,
                        Psi_WmK        = 0.5
                    }
                }
            };

            var result = _svc.Calculate(inputs, DhwLossMode.Automatic);

            double expected_tYear = 251 * 24.0;
            Assert.Equal(expected_tYear, result.T_year, precision: 0);
            Assert.Equal(expected_tYear, result.T_op,   precision: 0);
            Assert.Equal(0.0, result.T_nom, precision: 0);
        }

        // ══════════════════════════════════════════════════════════════════════
        // Регресионни тестове: ComputedPsi_WmK се записва след Calculate
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void Automatic_Mode_Sets_ComputedPsi_OnSegments_InsulatedInAir()
        {
            // Arrange – реалистични стойности: di=0.022 m, da=0.060 m, lamD=0.040, ha=10
            var seg = new PipeSegment
            {
                ZoneType      = PipeZoneType.Conditioned,
                Length_m      = 10.0,
                InsulationType = PipeInsulationType.InsulatedInAir,
                InnerDiameter_m              = 0.022,
                OuterDiameterWithInsulation_m = 0.060,
                InsulationLambda_WmK          = 0.040,
                SurfaceHeatTransfer_WmK       = 10.0
            };

            var inputs = new DhwLossInputs
            {
                HotWaterTemperature_degC      = 55,
                AmbientTempConditioned_degC   = 20,
                WorkingDaysPerYear            = 251,
                PipeSegments = new ObservableCollection<PipeSegment> { seg }
            };

            // Act
            _svc.Calculate(inputs, DhwLossMode.Automatic);

            // Assert – ComputedPsi_WmK трябва да е изчислено и > 0
            Assert.True(seg.ComputedPsi_WmK > 0,
                $"ComputedPsi_WmK трябва > 0, но е {seg.ComputedPsi_WmK}");
        }

        [Fact]
        public void ComputePsi_InsulatedInAir_LargerDa_GivesLowerPsi()
        {
            // По-дебела изолация (по-голямо da) → по-малко Ψ (по-добра изолация)
            var segThin = new PipeSegment
            {
                InsulationType               = PipeInsulationType.InsulatedInAir,
                InnerDiameter_m              = 0.022,
                OuterDiameterWithInsulation_m = 0.044,   // тънка изолация
                InsulationLambda_WmK          = 0.040,
                SurfaceHeatTransfer_WmK       = 10.0
            };
            var segThick = new PipeSegment
            {
                InsulationType               = PipeInsulationType.InsulatedInAir,
                InnerDiameter_m              = 0.022,
                OuterDiameterWithInsulation_m = 0.100,   // дебела изолация
                InsulationLambda_WmK          = 0.040,
                SurfaceHeatTransfer_WmK       = 10.0
            };

            double psiThin  = _svc.ComputePsi(segThin);
            double psiThick = _svc.ComputePsi(segThick);

            Assert.True(psiThin  > 0, $"psiThin трябва > 0, но е {psiThin}");
            Assert.True(psiThick > 0, $"psiThick трябва > 0, но е {psiThick}");
            Assert.True(psiThick < psiThin,
                $"По-дебела изолация трябва да дава по-малко Ψ: thick={psiThick:F4}, thin={psiThin:F4}");
        }

        [Fact]
        public void ComputePsi_InsulatedInAir_ZoneDoeNotAffectPsi_OnlyFormula()
        {
            // ZoneType НЕ влияе директно върху Ψ – само определя θ_amb при интегралното изчисление
            // Тестваме, че Ψ е идентично за Conditioned и Unconditioned при еднакви геометрични параметри
            var segCond = new PipeSegment
            {
                ZoneType = PipeZoneType.Conditioned,
                InsulationType               = PipeInsulationType.InsulatedInAir,
                InnerDiameter_m              = 0.022,
                OuterDiameterWithInsulation_m = 0.060,
                InsulationLambda_WmK          = 0.040,
                SurfaceHeatTransfer_WmK       = 10.0
            };
            var segUncond = new PipeSegment
            {
                ZoneType = PipeZoneType.Unconditioned,
                InsulationType               = PipeInsulationType.InsulatedInAir,
                InnerDiameter_m              = 0.022,
                OuterDiameterWithInsulation_m = 0.060,
                InsulationLambda_WmK          = 0.040,
                SurfaceHeatTransfer_WmK       = 10.0
            };

            double psiCond   = _svc.ComputePsi(segCond);
            double psiUncond = _svc.ComputePsi(segUncond);

            Assert.True(psiCond > 0);
            // Ψ е едно и също – зоната определя само θ_amb в енергийното изчисление
            Assert.Equal(psiCond, psiUncond, precision: 6);
        }

        [Fact]
        public void Automatic_ZoneType_AffectsQcond_NotPsi()
        {
            // При кондиционирана зона – загубите отиват в Q_cond; при некондиционирана – не
            // Двата сегмента са идентични, само зоната е различна
            var inputs = new DhwLossInputs
            {
                HotWaterTemperature_degC      = 55,
                AmbientTempConditioned_degC   = 20,
                AmbientTempUnconditioned_degC = 12,
                WorkingDaysPerYear            = 251,
                PipeSegments = new ObservableCollection<PipeSegment>
                {
                    new PipeSegment
                    {
                        ZoneType       = PipeZoneType.Conditioned,
                        Length_m       = 10.0,
                        InsulationType = PipeInsulationType.InsulatedInAir,
                        InnerDiameter_m              = 0.022,
                        OuterDiameterWithInsulation_m = 0.060,
                        InsulationLambda_WmK          = 0.040,
                        SurfaceHeatTransfer_WmK       = 10.0
                    },
                    new PipeSegment
                    {
                        ZoneType       = PipeZoneType.Unconditioned,
                        Length_m       = 10.0,
                        InsulationType = PipeInsulationType.InsulatedInAir,
                        InnerDiameter_m              = 0.022,
                        OuterDiameterWithInsulation_m = 0.060,
                        InsulationLambda_WmK          = 0.040,
                        SurfaceHeatTransfer_WmK       = 10.0
                    }
                }
            };

            var result = _svc.Calculate(inputs, DhwLossMode.Automatic);

            // Трябва да има загуби и в двете зони
            Assert.True(result.Q_total > 0, "Q_total трябва > 0");
            // Q_cond трябва да е по-малко от Q_total (само кондиционираният сегмент)
            Assert.True(result.Q_cond < result.Q_total,
                $"Q_cond ({result.Q_cond}) трябва < Q_total ({result.Q_total})");
            // f_rbl трябва да е между 0 и 1 (не 0 и не 1 точно)
            Assert.True(result.F_rbl > 0 && result.F_rbl < 1.0,
                $"f_rbl трябва между 0 и 1, но е {result.F_rbl}");
        }

        // ══════════════════════════════════════════════════════════════════════
        // Часово разпределение: t_op логика (3-priority)
        // ══════════════════════════════════════════════════════════════════════

        [Fact]
        public void Automatic_16hPerDay_Gives_tOp_TwoThirds_Of_tYear()
        {
            // Arrange — прост сегмент, изолиран в кондиционирана зона
            var inputs = new DhwLossInputs
            {
                HotWaterTemperature_degC             = 55,
                AmbientTempConditioned_degC   = 20,
                AmbientTempUnconditioned_degC = 12,
                WorkingDaysPerYear            = 365,
                OperatingHoursPerDay_hPerDay  = 16.0,   // 16h/ден
                PipeSegments = new ObservableCollection<PipeSegment>
                {
                    new PipeSegment
                    {
                        ZoneType       = PipeZoneType.Conditioned,
                        Length_m       = 10.0,
                        InsulationType = PipeInsulationType.InsulatedInAir,
                        InnerDiameter_m               = 0.022,
                        OuterDiameterWithInsulation_m = 0.060,
                        InsulationLambda_WmK          = 0.040,
                        SurfaceHeatTransfer_WmK       = 10.0
                    }
                }
            };

            // Act
            var result = _svc.Calculate(inputs, DhwLossMode.Automatic);

            // t_year = 365 * 24 = 8760 h
            // t_op   = 16/24 * 8760 = 5840 h
            Assert.Equal(8760, result.T_year, precision: 0);
            Assert.Equal(5840, result.T_op,   precision: 0);
            Assert.Equal(2920, result.T_nom,  precision: 0);
        }

        [Fact]
        public void Automatic_HPerDay_HasPriority_Over_HPerYear()
        {
            // Ако са зададени и двете — h/ден трябва да има приоритет
            var inputs = new DhwLossInputs
            {
                HotWaterTemperature_degC             = 55,
                AmbientTempConditioned_degC   = 20,
                AmbientTempUnconditioned_degC = 12,
                WorkingDaysPerYear            = 365,
                OperatingHoursPerDay_hPerDay  = 16.0,  // → t_op = 5840 h
                OperatingHours_hPerYear       = 2200,  // игнорира се
                PipeSegments = new ObservableCollection<PipeSegment>
                {
                    new PipeSegment
                    {
                        ZoneType       = PipeZoneType.Conditioned,
                        Length_m       = 5.0,
                        InsulationType = PipeInsulationType.InsulatedInAir,
                        InnerDiameter_m               = 0.022,
                        OuterDiameterWithInsulation_m = 0.050,
                        InsulationLambda_WmK          = 0.040,
                        SurfaceHeatTransfer_WmK       = 10.0
                    }
                }
            };

            var result = _svc.Calculate(inputs, DhwLossMode.Automatic);

            // h/ден = 16 → t_op = 5840, НЕ 2200
            Assert.Equal(5840, result.T_op, precision: 0);
        }

        [Fact]
        public void Automatic_HPerYear_Used_When_HPerDay_Is_Zero()
        {
            var inputs = new DhwLossInputs
            {
                HotWaterTemperature_degC             = 55,
                AmbientTempConditioned_degC   = 20,
                AmbientTempUnconditioned_degC = 12,
                WorkingDaysPerYear            = 365,
                OperatingHoursPerDay_hPerDay  = 0.0,   // не е зададено
                OperatingHours_hPerYear       = 2200,  // трябва да се ползва
                PipeSegments = new ObservableCollection<PipeSegment>
                {
                    new PipeSegment
                    {
                        ZoneType       = PipeZoneType.Conditioned,
                        Length_m       = 5.0,
                        InsulationType = PipeInsulationType.InsulatedInAir,
                        InnerDiameter_m               = 0.022,
                        OuterDiameterWithInsulation_m = 0.050,
                        InsulationLambda_WmK          = 0.040,
                        SurfaceHeatTransfer_WmK       = 10.0
                    }
                }
            };

            var result = _svc.Calculate(inputs, DhwLossMode.Automatic);

            Assert.Equal(2200, result.T_op, precision: 0);
        }

        [Fact]
        public void Automatic_24h_Per_Day_Gives_tOp_Equal_tYear()
        {
            // 24h/ден = 24/7 = t_op == t_year, t_nom == 0
            var inputs = new DhwLossInputs
            {
                HotWaterTemperature_degC             = 55,
                AmbientTempConditioned_degC   = 20,
                AmbientTempUnconditioned_degC = 12,
                WorkingDaysPerYear            = 365,
                OperatingHoursPerDay_hPerDay  = 24.0,
                PipeSegments = new ObservableCollection<PipeSegment>
                {
                    new PipeSegment
                    {
                        ZoneType       = PipeZoneType.Conditioned,
                        Length_m       = 5.0,
                        InsulationType = PipeInsulationType.InsulatedInAir,
                        InnerDiameter_m               = 0.022,
                        OuterDiameterWithInsulation_m = 0.050,
                        InsulationLambda_WmK          = 0.040,
                        SurfaceHeatTransfer_WmK       = 10.0
                    }
                }
            };

            var result = _svc.Calculate(inputs, DhwLossMode.Automatic);

            Assert.Equal(result.T_year, result.T_op,  precision: 0);
            Assert.Equal(0,             result.T_nom, precision: 0);
        }

        [Fact]
        public void Automatic_Changing_tOp_Changes_Qls_But_Qtotal_Stays_Constant()
        {
            // При UseSimplifiedMeanTemp=false, θ_w,nom = θ_w,set = 55°C.
            // Следователно: Q_total = Ψ * ΔΘ * L * (t_op + t_nom) / 1000 = Ψ * ΔΘ * L * t_year / 1000
            // Q_total е константно, независимо от t_op!
            // САМО Q_ls (работа) и Q_nom (извън работа) се разпределят различно.
            // Q_rbl = Q_cond остава ПОСТОЯННО (при само кондиционирана зона).
            PipeSegment MakeSeg() => new PipeSegment
            {
                ZoneType       = PipeZoneType.Conditioned,
                Length_m       = 20.0,
                InsulationType = PipeInsulationType.InsulatedInAir,
                InnerDiameter_m               = 0.022,
                OuterDiameterWithInsulation_m = 0.060,
                InsulationLambda_WmK          = 0.040,
                SurfaceHeatTransfer_WmK       = 10.0
            };

            var inputs8760 = new DhwLossInputs
            {
                HotWaterTemperature_degC      = 55,
                AmbientTempConditioned_degC   = 20,
                AmbientTempUnconditioned_degC = 12,
                WorkingDaysPerYear            = 365,
                OperatingHours_hPerYear       = 0,     // → 24/7 = 8760h, t_nom=0
                PipeSegments = new ObservableCollection<PipeSegment> { MakeSeg() }
            };
            var inputs2200 = new DhwLossInputs
            {
                HotWaterTemperature_degC      = 55,
                AmbientTempConditioned_degC   = 20,
                AmbientTempUnconditioned_degC = 12,
                WorkingDaysPerYear            = 365,
                OperatingHours_hPerYear       = 2200,  // t_op=2200, t_nom=6560
                PipeSegments = new ObservableCollection<PipeSegment> { MakeSeg() }
            };

            var res8760 = _svc.Calculate(inputs8760, DhwLossMode.Automatic);
            var res2200 = _svc.Calculate(inputs2200, DhwLossMode.Automatic);

            // Q_total = Ψ * ΔΘ * L * t_year / 1000 → трябва да е ЕДНАКВО
            Assert.Equal(res8760.Q_total, res2200.Q_total, precision: 1);

            // Q_dis_ls (работа) трябва да е по-малко при 2200h
            Assert.True(res2200.Q_dis_ls < res8760.Q_dis_ls,
                $"Q_dis_ls при 2200h ({res2200.Q_dis_ls:0.0}) трябва < Q_dis_ls при 8760h ({res8760.Q_dis_ls:0.0})");

            // Q_dis_nom (извън работа) при 2200h > 0
            Assert.True(res2200.Q_dis_nom > 0,
                $"Q_dis_nom при 2200h трябва > 0, но е {res2200.Q_dis_nom:0.0}");

            // При само кондиционирани сегменти: Q_rbl = Q_total (константно)
            Assert.Equal(res8760.Q_rbl_year, res2200.Q_rbl_year, precision: 1);
        }
    }
}
