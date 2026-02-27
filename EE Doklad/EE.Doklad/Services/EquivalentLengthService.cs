using System;
using System.Collections.Generic;
using System.Linq;

namespace EE.Doklad.Services
{
    // ══════════════════════════════════════════════════════════════════════════
    // DnSize – номинален диаметър (DN) за тръбни принадлежности
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Номинален диаметър DN – използва се за таблици с еквивалентни дължини.
    /// </summary>
    public enum DnSize
    {
        DN8  = 8,
        DN10 = 10,
        DN15 = 15,
        DN20 = 20,
        DN25 = 25,
        DN32 = 32,
        DN40 = 40,
        DN50 = 50
    }

    // ══════════════════════════════════════════════════════════════════════════
    // IEquivalentLengthService – интерфейс
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Сервиз за изчисляване на еквивалентна дължина L_equi [m]
    /// на база брой фитинги (колена, тройници, вентили) и DN(da).
    /// </summary>
    public interface IEquivalentLengthService
    {
        /// <summary>
        /// Намира най-близкия DN по зададен външен диаметър da [m].
        /// Ако da ≤ 0 → fallback DN20.
        /// </summary>
        DnSize GetNearestDn(double daMeters);

        /// <summary>
        /// Изчислява L_equi = N_elbow90 * Leq_elbow(DN) + N_tee * Leq_tee(DN) + N_valve * Leq_valve(DN).
        /// </summary>
        double CalcLequi(double daMeters, int elbows90, int teeBranch, int ballValve);

        /// <summary>
        /// Детайлно изчисление с допълнителна информация (DN, isOutOfRange).
        /// </summary>
        LequiDetailedResult CalcLequiDetailed(double daMeters, int elbows90, int teeBranch, int ballValve);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // LequiDetailedResult – резултат с допълнителна диагностика
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Резултат от детайлно изчисление на L_equi.
    /// </summary>
    public sealed class LequiDetailedResult
    {
        /// <summary>Изчислена еквивалентна дължина [m]</summary>
        public double Lequi { get; init; }

        /// <summary>Избраният DN</summary>
        public DnSize Dn { get; init; }

        /// <summary>Референтен da за избрания DN [m]</summary>
        public double DnRefDa { get; init; }

        /// <summary>
        /// True ако |da – dnRefDa| > 0.010 m (10 mm).
        /// Показва warning: „da не съвпада добре с DN таблицата".
        /// </summary>
        public bool IsOutOfRange { get; init; }

        /// <summary>Абсолютна разлика |da – dnRefDa| [m]</summary>
        public double DaDeviation { get; init; }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // EquivalentLengthService – имплементация
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Имплементация на <see cref="IEquivalentLengthService"/>.
    /// Данните за еквивалентни дължини са ТОПЛИННИ (не хидравлични).
    /// </summary>
    public class EquivalentLengthService : IEquivalentLengthService
    {
        // ── Референтни da [m] за всеки DN ────────────────────────────────────

        /// <summary>Ориентировъчни външни диаметри за DN, използвани за mapping da→DN.</summary>
        public static readonly IReadOnlyDictionary<DnSize, double> DnRefDa =
            new Dictionary<DnSize, double>
            {
                { DnSize.DN8,  0.013 },
                { DnSize.DN10, 0.017 },
                { DnSize.DN15, 0.021 },
                { DnSize.DN20, 0.027 },
                { DnSize.DN25, 0.034 },
                { DnSize.DN32, 0.042 },
                { DnSize.DN40, 0.048 },
                { DnSize.DN50, 0.060 },
            };

        // ── Еквивалентни дължини по DN (1 бр.) [m] ─────────────────────────

        /// <summary>Колено 90° – Leq [m] за 1 бр.</summary>
        public static readonly IReadOnlyDictionary<DnSize, double> Elbow90Leq =
            new Dictionary<DnSize, double>
            {
                { DnSize.DN8,  0.10 },
                { DnSize.DN10, 0.14 },
                { DnSize.DN15, 0.17 },
                { DnSize.DN20, 0.22 },
                { DnSize.DN25, 0.27 },
                { DnSize.DN32, 0.34 },
                { DnSize.DN40, 0.38 },
                { DnSize.DN50, 0.48 },
            };

        /// <summary>Тройник (разклонение) – Leq [m] за 1 бр.</summary>
        public static readonly IReadOnlyDictionary<DnSize, double> TeeBranchLeq =
            new Dictionary<DnSize, double>
            {
                { DnSize.DN8,  0.23 },
                { DnSize.DN10, 0.31 },
                { DnSize.DN15, 0.38 },
                { DnSize.DN20, 0.49 },
                { DnSize.DN25, 0.61 },
                { DnSize.DN32, 0.76 },
                { DnSize.DN40, 0.86 },
                { DnSize.DN50, 1.08 },
            };

        /// <summary>Сферичен вентил – Leq [m] за 1 бр.</summary>
        public static readonly IReadOnlyDictionary<DnSize, double> BallValveLeq =
            new Dictionary<DnSize, double>
            {
                { DnSize.DN8,  0.21 },
                { DnSize.DN10, 0.27 },
                { DnSize.DN15, 0.34 },
                { DnSize.DN20, 0.43 },
                { DnSize.DN25, 0.54 },
                { DnSize.DN32, 0.67 },
                { DnSize.DN40, 0.77 },
                { DnSize.DN50, 0.96 },
            };

        // ── OUT-OF-RANGE праг ────────────────────────────────────────────────

        /// <summary>Ако |da – dnRefDa| > тази стойност → warning.</summary>
        public const double OutOfRangeThreshold_m = 0.010; // 10 mm

        // ── Публични методи ──────────────────────────────────────────────────

        /// <inheritdoc/>
        public DnSize GetNearestDn(double daMeters)
        {
            if (daMeters <= 0.0)
                return DnSize.DN20; // fallback

            DnSize best = DnSize.DN20;
            double bestDiff = double.MaxValue;

            foreach (var kv in DnRefDa)
            {
                double diff = Math.Abs(daMeters - kv.Value);
                if (diff < bestDiff)
                {
                    bestDiff = diff;
                    best = kv.Key;
                }
            }

            return best;
        }

        /// <inheritdoc/>
        public double CalcLequi(double daMeters, int elbows90, int teeBranch, int ballValve)
        {
            var dn = GetNearestDn(daMeters);

            double leq = Math.Max(0, elbows90)  * Elbow90Leq[dn]
                       + Math.Max(0, teeBranch)  * TeeBranchLeq[dn]
                       + Math.Max(0, ballValve)  * BallValveLeq[dn];

            return Math.Round(leq, 2);
        }

        /// <inheritdoc/>
        public LequiDetailedResult CalcLequiDetailed(double daMeters, int elbows90, int teeBranch, int ballValve)
        {
            var dn = GetNearestDn(daMeters);
            double dnRef = DnRefDa[dn];
            double deviation = Math.Abs(daMeters - dnRef);

            double leq = Math.Max(0, elbows90)  * Elbow90Leq[dn]
                       + Math.Max(0, teeBranch)  * TeeBranchLeq[dn]
                       + Math.Max(0, ballValve)  * BallValveLeq[dn];

            return new LequiDetailedResult
            {
                Lequi        = Math.Round(leq, 2),
                Dn           = dn,
                DnRefDa      = dnRef,
                IsOutOfRange = daMeters > 0 && deviation > OutOfRangeThreshold_m,
                DaDeviation  = Math.Round(deviation, 4),
            };
        }
    }
}
