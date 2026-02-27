using System;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Models;

namespace EE.Doklad.Services
{
    // ══════════════════════════════════════════════════════════════════════════
    // IDhwDistributionLossService – интерфейс
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Сервис за изчисляване на регенерируеми загуби от БГВ разпределителна система.
    /// Имплементира EN 15316-3 / ISO 52003 методика.
    /// </summary>
    public interface IDhwDistributionLossService
    {
        /// <summary>
        /// Изчислява регенерируемите загуби по зададения режим.
        /// </summary>
        DhwLossResult Calculate(DhwLossInputs inputs, DhwLossMode mode);

        /// <summary>
        /// Изчислява Ψ [W/(m·K)] за един тръбен сегмент по формулите (1.3)–(1.6).
        /// </summary>
        double ComputePsi(PipeSegment segment);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // DhwDistributionLossService – имплементация
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Изчислява регенерируеми загуби от разпределителна система за БГВ.
    ///
    /// МАТЕМАТИЧЕСКИ МОДЕЛ – ФОРМУЛИ (по EN 15316-3):
    ///
    /// (1.3) Ψ за изолирана тръба във въздух:
    ///       Ψ = π / ( (1/(2·λ_D))·ln(d_a/d_i) + 1/(h_a·d_a) )
    ///
    /// (1.4) Ψ_em за вградена тръба в материал:
    ///       Ψ = π / ( 0.5·( (1/λ_D)·ln(d_a/d_i) + (1/λ_em)·ln(4·z/d_a) ) )
    ///
    /// (1.5) Ψ_non за неизолирана тръба:
    ///       Ψ = π / ( (1/(2·λ_p))·ln(d_p,a/d_p,i) + 1/(h_a·d_p,a) )
    ///
    /// (1.6) Приближение: Ψ_non ≈ h_a · π · d_p,a
    ///
    /// (1.7) Загуби по ТРЪБИ по ВРЕМЕ на работа:
    ///       Q_dis,ls = (1/1000) · Σ Ψ_i · (θ_w – θ_amb) · (L + L_equi) · t_op
    ///
    /// (1.8) Stub загуби по време на работа:
    ///       Q_stub = ṁ_stub · c_w · (θ_w – θ_amb) · t_op
    ///
    /// (1.9) Масов дебит в stub:
    ///       ṁ_stub = V_stub · ρ_w · n_tap   [kg/h]
    ///
    /// (1.10) Загуби ИЗВЪН работа:
    ///        Q_nom = (1/1000) · Σ Ψ_i · (θ_w,avg – θ_amb) · (L + L_equi) · t_nom
    ///
    /// (1.15) Опростена средна температура:
    ///        θ_w,mean = 25 · Ψ^(−0.2)
    ///
    /// (1.16) Общо: Q_total = Q_ls + Q_nom + Q_stub
    ///
    /// (1.17) Дял: f_rbl = Q_cond / Q_total
    ///
    /// (1.18) Регенерируеми загуби: Q_rbl = f_rbl · Q_total
    /// </summary>
    public sealed class DhwDistributionLossService : IDhwDistributionLossService
    {
        // ── Публичен API ──────────────────────────────────────────────────────

        /// <inheritdoc />
        public DhwLossResult Calculate(DhwLossInputs inputs, DhwLossMode mode)
        {
            if (inputs is null) throw new ArgumentNullException(nameof(inputs));

            return mode switch
            {
                DhwLossMode.Manual      => CalculateManual(inputs),
                DhwLossMode.PercentShare => CalculatePercentShare(inputs),
                DhwLossMode.Automatic   => CalculateAutomatic(inputs),
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        /// <inheritdoc />
        public double ComputePsi(PipeSegment segment)
        {
            if (segment is null) throw new ArgumentNullException(nameof(segment));
            return CalcPsi(segment);
        }

        // ── Режим A: Ръчно ───────────────────────────────────────────────────

        /// <summary>
        /// Режим A: Стойността се пази без промяна.
        /// Стъпка 0: Ако userManualValueKwhYear > 0 → запиши го и край.
        /// </summary>
        private static DhwLossResult CalculateManual(DhwLossInputs inputs)
        {
            // Guard: не изчисляваме нищо – директно връщаме ръчната стойност
            double value = Math.Max(0.0, inputs.ManualRecoverableLoss_kWh);

            return new DhwLossResult
            {
                Mode        = DhwLossMode.Manual,
                Q_rbl_year  = value,
                F_rbl       = 0.0, // не е изчислено
                DiagnosticInfo = "Режим A: Ръчно въведена стойност."
            };
        }

        // ── Режим C: % Дял ───────────────────────────────────────────────────

        /// <summary>
        /// Режим C: Q_rbl = Q_total_losses * percent / 100
        /// Стъпка 0: Ако userPercentShare > 0 и имаме Q_total_losses:
        ///   Q_rbl_year = Q_total_losses * percent / 100
        /// </summary>
        private static DhwLossResult CalculatePercentShare(DhwLossInputs inputs)
        {
            // Guard clauses
            if (inputs.PercentShare <= 0.0)
            {
                return new DhwLossResult
                {
                    Mode           = DhwLossMode.PercentShare,
                    Q_rbl_year     = 0.0,
                    DiagnosticInfo = "Режим C: Дялът е 0 – резултатът е 0."
                };
            }

            double pct   = Math.Clamp(inputs.PercentShare, 0.0, 100.0);
            double total = Math.Max(0.0, inputs.TotalSystemLosses_kWh);
            double qRbl  = total * pct / 100.0;

            return new DhwLossResult
            {
                Mode           = DhwLossMode.PercentShare,
                Q_total        = total,
                Q_cond         = qRbl,
                F_rbl          = pct / 100.0,
                Q_rbl_year     = Math.Round(qRbl, 2),
                DiagnosticInfo = $"Режим C: {total:0.00} kWh × {pct:0.##}% = {qRbl:0.00} kWh"
            };
        }

        // ── Режим B: Автоматично изчисление ─────────────────────────────────

        /// <summary>
        /// Режим B: Изчисление по методиката – стъпки 1–10.
        /// </summary>
        private DhwLossResult CalculateAutomatic(DhwLossInputs inputs)
        {
            // ── Стъпка 1: Определяме t_year, t_op, t_nom ─────────────────────
            //    t_year = (WorkingDaysPerYear > 0) ? WorkingDaysPerYear * 24 : 8760
            double tYear = inputs.WorkingDaysPerYear > 0
                ? inputs.WorkingDaysPerYear * 24.0
                : 8760.0;

            // Приоритет:
            //  1) OperatingHoursPerDay_hPerDay > 0  → t_op = h/д * t_year / 24
            //     Пример: 16 h/д → 16/24 = 66.7% от годишните часове
            //  2) OperatingHours_hPerYear > 0        → директно зададени h/год
            //  3) Иначе → 24/7 циркулация → t_op = t_year
            double tOp;
            if (inputs.OperatingHoursPerDay_hPerDay > 0)
            {
                double hpd = Math.Clamp(inputs.OperatingHoursPerDay_hPerDay, 0.0, 24.0);
                tOp = hpd / 24.0 * tYear;
            }
            else if (inputs.OperatingHours_hPerYear > 0)
            {
                tOp = Math.Clamp(inputs.OperatingHours_hPerYear, 0.0, tYear);
            }
            else
            {
                tOp = tYear; // 24/7
            }

            //    t_nom = t_year - t_op  (часовете когато помпата НЕ работи, но водата е топла)
            double tNom = Math.Max(0.0, tYear - tOp);

            var segments = inputs.PipeSegments?.ToList() ?? new List<PipeSegment>();
            if (segments.Count == 0)
            {
                return new DhwLossResult
                {
                    Mode           = DhwLossMode.Automatic,
                    T_year         = tYear,
                    T_op           = tOp,
                    T_nom          = tNom,
                    Q_rbl_year     = 0.0,
                    DiagnosticInfo = "Режим B: Няма дефинирани тръбни сегменти – резултатът е 0."
                };
            }

            // ── Стъпки 2–8: Обхождаме сегментите ────────────────────────────
            double qLsTotal     = 0, qNomTotal     = 0;
            double qLsCond      = 0, qNomCond      = 0;

            double thetaW = inputs.HotWaterTemperature_degC;

            foreach (var seg in segments)
            {
                if (seg.IsStub) continue; // stub-овете се обработват отделно

                // ── Стъпка 2: Ψ_i ─────────────────────────────────────────
                double psi   = CalcPsi(seg);
                seg.ComputedPsi_WmK = psi;

                double lEff  = seg.Length_m + seg.EquivalentLength_m;  // L + L_equi

                // ── Стъпка 3: θ_amb ──────────────────────────────────────
                double thetaAmb = seg.ZoneType == PipeZoneType.Conditioned
                    ? inputs.AmbientTempConditioned_degC
                    : inputs.AmbientTempUnconditioned_degC;

                // θ_w за работа = θ_w_set
                double thetaWMeanOp = thetaW;

                // θ_w за извън работа:
                // Default: θ_w_avg = θ_w_set
                // Ако UseSimplifiedMeanTemp: θ_w,mean = 25 * Ψ^(−0.2) – формула (1.15)
                double thetaWMeanNom = inputs.UseSimplifiedMeanTemp && psi > 0
                    ? 25.0 * Math.Pow(psi, -0.2)
                    : thetaW;

                // ── Стъпка 4: Q_ls (работа) – формула (1.7) ──────────────
                //    Q_ls_i = (1/1000) · Ψ · (θ_w – θ_amb) · (L + L_equi) · t_op
                double qLs_i = (1.0 / 1000.0) * psi * (thetaWMeanOp - thetaAmb) * lEff * tOp;
                qLs_i = Math.Max(0.0, qLs_i);
                qLsTotal += qLs_i;

                // ── Стъпка 5: Q_nom (извън работа) – формула (1.10) ──────
                //    Q_nom_i = (1/1000) · Ψ · (θ_w,avg – θ_amb) · (L + L_equi) · t_nom
                double qNom_i = (1.0 / 1000.0) * psi * (thetaWMeanNom - thetaAmb) * lEff * tNom;
                qNom_i = Math.Max(0.0, qNom_i);
                qNomTotal += qNom_i;

                // ── Стъпка 8: Само кондиционирана зона ───────────────────
                if (seg.ZoneType == PipeZoneType.Conditioned)
                {
                    qLsCond  += qLs_i;
                    qNomCond += qNom_i;
                }
            }

            // ── Стъпка 6: Stub загуби – формули (1.8) и (1.9) ─────────────
            //    ṁ_stub_j = V_stub_j · ρ_w · n_tap_j       [kg/h]
            //    Q_stub_j = ṁ_stub_j · c_w · (θ_w – θ_amb) · t_op  [kWh]
            double qStubTotal = 0, qStubCond = 0;
            var stubZones = inputs.StubZones?.ToList() ?? new List<StubZoneData>();

            foreach (var stub in stubZones)
            {
                double thetaAmbStub = stub.ZoneType == PipeZoneType.Conditioned
                    ? inputs.AmbientTempConditioned_degC
                    : inputs.AmbientTempUnconditioned_degC;

                // (1.9): ṁ = V * ρ_w * n_tap  [kg/h]
                double mDot = stub.StubVolume_m3 * inputs.WaterDensity_kgm3 * stub.TappingFrequency_perHour;

                // (1.8): Q = ṁ * c_w * (θ_w – θ_amb) * t_op
                double qStub = mDot * inputs.WaterHeatCapacity_kWhkgK
                    * Math.Max(0.0, thetaW - thetaAmbStub)
                    * tOp;

                qStub      = Math.Max(0.0, qStub);
                qStubTotal += qStub;

                if (stub.ZoneType == PipeZoneType.Conditioned)
                    qStubCond += qStub;
            }

            // ── Стъпка 7: Общо Q_total – формула (1.16) ──────────────────────
            //    Q_total = Q_ls + Q_nom + Q_stub
            double qTotal    = qLsTotal  + qNomTotal  + qStubTotal;
            double qCondTotal = qLsCond  + qNomCond   + qStubCond;

            // ── Стъпка 9: f_rbl – формула (1.17) ─────────────────────────────
            //    f_rbl = Q_cond / Q_total  (ако Q_total > 0, иначе 0)
            double fRbl = qTotal > 0.0 ? qCondTotal / qTotal : 0.0;
            fRbl = Math.Clamp(fRbl, 0.0, 1.0);

            // ── Стъпка 10: Q_rbl – формула (1.18) ────────────────────────────
            //    Q_rbl_year = f_rbl · Q_total
            double qRblYear = Math.Round(fRbl * qTotal, 2);

            return new DhwLossResult
            {
                Mode              = DhwLossMode.Automatic,
                Q_dis_ls          = Math.Round(qLsTotal,   2),
                Q_dis_nom         = Math.Round(qNomTotal,  2),
                Q_dis_stub        = Math.Round(qStubTotal, 2),
                Q_total           = Math.Round(qTotal,     2),
                Q_dis_ls_cond     = Math.Round(qLsCond,    2),
                Q_dis_nom_cond    = Math.Round(qNomCond,   2),
                Q_dis_stub_cond   = Math.Round(qStubCond,  2),
                Q_cond            = Math.Round(qCondTotal, 2),
                F_rbl             = Math.Round(fRbl,       4),
                Q_rbl_year        = qRblYear,
                T_year            = tYear,
                T_op              = tOp,
                T_nom             = tNom,
                DiagnosticInfo    =
                    $"Режим B: t_year={tYear:0}h, t_op={tOp:0}h, t_nom={tNom:0}h | " +
                    $"Q_ls={qLsTotal:0.##}, Q_nom={qNomTotal:0.##}, Q_stub={qStubTotal:0.##} | " +
                    $"Q_total={qTotal:0.##} kWh, Q_cond={qCondTotal:0.##} kWh, f_rbl={fRbl:P2}"
            };
        }

        // ── Вътрешна: изчислява Ψ за сегмент ────────────────────────────────

        /// <summary>
        /// Изчислява Ψ [W/(m·K)] за тръбен сегмент по формули (1.3)–(1.6).
        /// </summary>
        private static double CalcPsi(PipeSegment seg)
        {
            // Guard: ако е DirectPsi – връщаме директно
            if (seg.InsulationType == PipeInsulationType.DirectPsi)
                return Math.Max(0.0, seg.Psi_WmK);

            switch (seg.InsulationType)
            {
                // ── (1.3) Изолирана тръба във въздух ─────────────────────────
                // Ψ = π / ( (1/(2·λ_D)) · ln(d_a/d_i) + 1/(h_a·d_a) )
                case PipeInsulationType.InsulatedInAir:
                {
                    double di    = seg.InnerDiameter_m;
                    double da    = seg.OuterDiameterWithInsulation_m;
                    double lamD  = seg.InsulationLambda_WmK;
                    double ha    = seg.SurfaceHeatTransfer_WmK;

                    if (di <= 0 || da <= di || lamD <= 0 || ha <= 0) return 0.0;

                    double denom = (1.0 / (2.0 * lamD)) * Math.Log(da / di)
                                   + 1.0 / (ha * da);
                    if (denom <= 0.0) return 0.0;
                    return Math.PI / denom;
                }

                // ── (1.4) Вградена тръба в материал ──────────────────────────
                // Ψ = π / ( 0.5 · ( (1/λ_D)·ln(d_a/d_i) + (1/λ_em)·ln(4·z/d_a) ) )
                case PipeInsulationType.EmbeddedInMaterial:
                {
                    double di    = seg.InnerDiameter_m;
                    double da    = seg.OuterDiameterWithInsulation_m;
                    double lamD  = seg.InsulationLambda_WmK;
                    double lamEm = seg.EmbeddingMaterialLambda_WmK;
                    double z     = seg.DepthFromSurface_m;

                    if (di <= 0 || da <= di || lamD <= 0 || lamEm <= 0 || z <= 0) return 0.0;
                    if (4.0 * z <= da) return 0.0; // невалидна геометрия

                    double term1 = (1.0 / lamD) * Math.Log(da / di);
                    double term2 = (1.0 / lamEm) * Math.Log(4.0 * z / da);
                    double denom = 0.5 * (term1 + term2);
                    if (denom <= 0.0) return 0.0;
                    return Math.PI / denom;
                }

                // ── (1.5)/(1.6) Неизолирана тръба ────────────────────────────
                // (1.5): Ψ = π / ( (1/(2·λ_p))·ln(d_p,a/d_p,i) + 1/(h_a·d_p,a) )
                // (1.6): Ψ ≈ h_a · π · d_p,a
                case PipeInsulationType.Uninsulated:
                {
                    double dpi   = seg.PipeInnerDiameter_m;
                    double dpa   = seg.PipeOuterDiameter_m;
                    double lamP  = seg.PipeMaterialLambda_WmK;
                    double ha    = seg.SurfaceHeatTransfer_WmK;

                    if (dpa <= 0 || ha <= 0) return 0.0;

                    if (seg.UseApproximatePsiForUninsulated)
                    {
                        // (1.6): Ψ ≈ h_a · π · d_p,a
                        return ha * Math.PI * dpa;
                    }
                    else
                    {
                        // (1.5)
                        if (dpi <= 0 || dpa <= dpi || lamP <= 0) return 0.0;
                        double denom = (1.0 / (2.0 * lamP)) * Math.Log(dpa / dpi)
                                       + 1.0 / (ha * dpa);
                        if (denom <= 0.0) return 0.0;
                        return Math.PI / denom;
                    }
                }

                default:
                    return 0.0;
            }
        }
    }
}
