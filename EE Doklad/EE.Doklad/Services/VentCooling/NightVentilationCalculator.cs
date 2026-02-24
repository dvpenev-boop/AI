using System;
using System.Collections.Generic;
using System.Text;

namespace EE.Doklad.Services.VentCooling
{
    // ════════════════════════════════════════════════════════════════════════════
    // NightVentilationCalculator
    //
    // Изчислява приноса от нощно вентилиране (free cooling) с НЕобработен
    // външен въздух — само sensible (топлинна), без RH / влагосъдържание /
    // енталпия.
    //
    // Формули (ρ·ca = 0.34 Wh/(m³·K)):
    //   VdotNight   = VdotSpecNight [m³/h·m²] × A [m²]          [m³/h]
    //   E_hour_kWh  = 0.34 × VdotNight × max(0, Ti − Te) / 1000  [kWh / h]
    //   monthEnergy = Σ_dayType (sumDayType × daysOfDayType)       [kWh]
    //   seasonEnergy = Σ_month  monthEnergy                        [kWh]
    //
    // Зависимости: само System.* — без психрометрия, без JSON, без EpwParser.
    // ════════════════════════════════════════════════════════════════════════════

    // ── Enums ────────────────────────────────────────────────────────────────────

    /// <summary>Тип на деня за агрегация на нощната вентилация.</summary>
    public enum NightVentDayType { Weekday, Saturday, Sunday }

    // ── Value objects ────────────────────────────────────────────────────────────

    /// <summary>
    /// Осреднен почасов климатичен профил за един месец (24 стойности на
    /// температурата, индексирани по час 0..23).
    /// </summary>
    public sealed record ClimateHourlyProfile(double[] OutdoorTempCByHour)
    {
        /// <summary>Температура [°C] за час <paramref name="hour"/> (0..23).</summary>
        public double TempAt(int hour) => OutdoorTempCByHour[hour];
    }

    /// <summary>
    /// График за нощно вентилиране: за всеки от трите типа дни съдържа масив
    /// от 24 bool стойности, дали вентилацията е активна в съответния час.
    /// </summary>
    public sealed record NightVentSchedule(
        bool[] ActiveByHourWeekday,
        bool[] ActiveByHourSaturday,
        bool[] ActiveByHourSunday)
    {
        /// <summary>Дали вентилацията е активна за даден тип ден и час.</summary>
        public bool IsActive(NightVentDayType dayType, int hour) => dayType switch
        {
            NightVentDayType.Weekday  => ActiveByHourWeekday[hour],
            NightVentDayType.Saturday => ActiveByHourSaturday[hour],
            NightVentDayType.Sunday   => ActiveByHourSunday[hour],
            _                         => false,
        };

        /// <summary>
        /// Брои активните часове за даден тип ден.
        /// </summary>
        public int ActiveHours(NightVentDayType dayType) => dayType switch
        {
            NightVentDayType.Weekday  => CountActive(ActiveByHourWeekday),
            NightVentDayType.Saturday => CountActive(ActiveByHourSaturday),
            NightVentDayType.Sunday   => CountActive(ActiveByHourSunday),
            _                         => 0,
        };

        private static int CountActive(bool[] arr)
        {
            int n = 0;
            foreach (var b in arr) if (b) n++;
            return n;
        }
    }

    /// <summary>
    /// Входен DTO за <see cref="NightVentilationCalculator.Calculate"/>.
    /// </summary>
    public sealed record NightVentInput(
        /// <summary>Обслужвана охлаждаема площ A [m²]. Трябва да е > 0.</summary>
        double AreaM2,
        /// <summary>
        /// Специфичен дебит за нощна вентилация [m³/h·m²].
        /// VdotNight = VdotSpecNight × AreaM2.
        /// </summary>
        double SpecAirflowM3phM2,
        /// <summary>Вътрешен setpoint за охлаждане Ti [°C].</summary>
        double IndoorCoolingSetpointC,
        /// <summary>Месеци в охладителния сезон (1..12).</summary>
        IReadOnlyList<int> CoolingSeasonMonths,
        /// <summary>
        /// Почасов климатичен профил по месеци (ключ = месец 1..12).
        /// Всеки профил съдържа 24 стойности на температурата.
        /// </summary>
        IReadOnlyDictionary<int, ClimateHourlyProfile> ClimateProfiles,
        /// <summary>График за нощно вентилиране (Weekday/Saturday/Sunday, 24 часа).</summary>
        NightVentSchedule Schedule,
        /// <summary>
        /// Брой дни в месеца (ключ = месец 1..12).
        /// Ако null, се използват стандартните стойности за 2024.
        /// </summary>
        IReadOnlyDictionary<int, int>? DaysInMonth = null,
        /// <summary>
        /// Опционална разбивка по тип на ден (Weekdays/Saturdays/Sundays) за всеки месец.
        /// Ако присъства, се използва предвид броя дни в месеца при агрегацията
        /// (поддържа частични месеци: напр. 15.08–15.09).
        /// Ключ = месец (1..12).
        /// </summary>
        IReadOnlyDictionary<int, (int Weekdays, int Saturdays, int Sundays)>? DayTypeCountsPerMonth = null
    );

    // ── Debug structures ─────────────────────────────────────────────────────────

    /// <summary>
    /// Debug-стойности за един час в рамките на осреднен ден за месеца.
    /// </summary>
    public sealed record NightVentHourDebug(
        int    Hour,
        double Te,
        double DT,
        bool   ActiveWeekday,
        bool   ActiveSaturday,
        bool   ActiveSunday,
        double EHourWeekday_kWh,
        double EHourSaturday_kWh,
        double EHourSunday_kWh
    );

    /// <summary>
    /// Debug-стойности за един месец от охладителния сезон.
    /// </summary>
    public sealed record NightVentMonthDebug(
        int                         Month,
        int                         Weekdays,
        int                         Saturdays,
        int                         Sundays,
        double                      SumWeekday_kWh,
        double                      SumSaturday_kWh,
        double                      SumSunday_kWh,
        double                      MonthEnergy_kWh,
        IReadOnlyList<NightVentHourDebug> Hours
    );

    /// <summary>
    /// Пълни debug-данни от изчислението.
    /// </summary>
    public sealed record NightVentDebugDetails(
        double                             VdotNight_m3ph,
        double                             SeasonEnergy_kWh,
        double                             SeasonSpecific_kWhPerM2,
        IReadOnlyList<NightVentMonthDebug> PerMonth
    );

    // ── Result DTO ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Изходен DTO от <see cref="NightVentilationCalculator.Calculate"/>.
    /// </summary>
    public sealed record NightVentResult(
        /// <summary>Принос за целия охладителен сезон [kWh].</summary>
        double TotalKWh,
        /// <summary>Специфичен принос [kWh/m²].</summary>
        double SpecificKWhPerM2,
        /// <summary>Месечна разбивка [kWh] (ключ = месец 1..12).</summary>
        Dictionary<int, double> PerMonthKWh,
        /// <summary>Детайлни debug-данни (null ако не са поискани).</summary>
        NightVentDebugDetails? DebugDetails,
        /// <summary>true при успешно изчисление.</summary>
        bool IsValid = true,
        /// <summary>Съобщение при грешка.</summary>
        string? ErrorMessage = null
    )
    {
        /// <summary>Създава failed резултат с грешка.</summary>
        public static NightVentResult Fail(string message) =>
            new(0, 0, new Dictionary<int, double>(), null, false, message);
    }

    // ── Calculator ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Калкулатор за принос от нощно вентилиране (sensible-only free cooling).
    /// </summary>
    public static class NightVentilationCalculator
    {
        /// <summary>
        /// ρ·ca за въздух = 0.34 Wh/(m³·K).
        /// </summary>
        private const double RhoTimesCA_Wh_m3K = 0.34;

        /// <summary>
        /// Стандартни дни в месеца (невисокосна 2024 за съответствие с останалите калкулатори).
        /// </summary>
        private static readonly int[] DefaultDaysInMonth2024 =
            { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        // ── Public API ────────────────────────────────────────────────────────────

        /// <summary>
        /// Изчислява приноса от нощно вентилиране с НЕобработен външен въздух
        /// (sensible-only) за охладителния сезон.
        /// </summary>
        /// <param name="input">Входни параметри.</param>
        /// <param name="collectDebug">
        /// Ако <c>true</c>, попълва <see cref="NightVentResult.DebugDetails"/>
        /// с почасова разбивка по месеци.
        /// </param>
        /// <returns>Резултат с общия принос [kWh] и [kWh/m²].</returns>
        /// <exception cref="ArgumentNullException">При null аргументи.</exception>
        public static NightVentResult Calculate(NightVentInput input, bool collectDebug = false)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            // ── Валидация ──────────────────────────────────────────────────────
            if (input.AreaM2 <= 0)
                return NightVentResult.Fail($"AreaM2 трябва да е > 0 (получено {input.AreaM2}).");

            if (input.SpecAirflowM3phM2 < 0)
                return NightVentResult.Fail($"SpecAirflowM3phM2 трябва да е ≥ 0 (получено {input.SpecAirflowM3phM2}).");

            if (input.CoolingSeasonMonths == null || input.CoolingSeasonMonths.Count == 0)
                return NightVentResult.Fail("CoolingSeasonMonths не може да е празен.");

            if (input.ClimateProfiles == null)
                return NightVentResult.Fail("ClimateProfiles не може да е null.");

            if (input.Schedule == null)
                return NightVentResult.Fail("Schedule не може да е null.");

            foreach (int m in input.CoolingSeasonMonths)
            {
                if (m < 1 || m > 12)
                    return NightVentResult.Fail($"Невалиден месец в CoolingSeasonMonths: {m}.");

                if (!input.ClimateProfiles.ContainsKey(m))
                    return NightVentResult.Fail($"Липсва климатичен профил за месец {m}.");
            }

            // ── Нулев дебит — пряко 0 ─────────────────────────────────────────
            if (input.SpecAirflowM3phM2 == 0.0)
            {
                var zeroMonths = new Dictionary<int, double>();
                foreach (int m in input.CoolingSeasonMonths) zeroMonths[m] = 0.0;

                NightVentDebugDetails? zeroDebug = null;
                if (collectDebug)
                {
                    var zeroPerMonth = new List<NightVentMonthDebug>();
                    foreach (int m in input.CoolingSeasonMonths)
                        zeroPerMonth.Add(BuildMonthDebug(m, 0.0, input, GetDays(input.DaysInMonth, m)));
                    zeroDebug = new NightVentDebugDetails(0.0, 0.0, 0.0, zeroPerMonth);
                }

                return new NightVentResult(0.0, 0.0, zeroMonths, zeroDebug);
            }

            // ── Изчисление ────────────────────────────────────────────────────
            double vdotNight = input.SpecAirflowM3phM2 * input.AreaM2;   // [m³/h]

            double seasonEnergy_kWh = 0.0;
            var perMonthKWh = new Dictionary<int, double>();
            var debugMonths = collectDebug ? new List<NightVentMonthDebug>() : null;

            foreach (int month in input.CoolingSeasonMonths)
            {
                var profile  = input.ClimateProfiles[month];
                // Prefer exact day-type counts when provided (supports partial-month seasons).
                // Fall back to the approximation only when no precise counts are supplied.
                var days = (input.DayTypeCountsPerMonth != null &&
                            input.DayTypeCountsPerMonth.TryGetValue(month, out var exactCounts))
                           ? exactCounts
                           : ComputeDayTypeCounts(input.DaysInMonth, month);

                // Суми на E_hour_kWh по тип ден за осреднения ден
                double sumWd = 0, sumSat = 0, sumSun = 0;
                var hourDebugList = collectDebug ? new List<NightVentHourDebug>(24) : null;

                for (int h = 0; h < 24; h++)
                {
                    double te = profile.TempAt(h);
                    double dt = Math.Max(0.0, input.IndoorCoolingSetpointC - te);
                    double eH = RhoTimesCA_Wh_m3K * vdotNight * dt / 1000.0; // kWh per hour

                    bool activeWd  = input.Schedule.IsActive(NightVentDayType.Weekday,  h);
                    bool activeSat = input.Schedule.IsActive(NightVentDayType.Saturday, h);
                    bool activeSun = input.Schedule.IsActive(NightVentDayType.Sunday,   h);

                    if (activeWd)  sumWd  += eH;
                    if (activeSat) sumSat += eH;
                    if (activeSun) sumSun += eH;

                    if (collectDebug)
                    {
                        hourDebugList!.Add(new NightVentHourDebug(
                            h, te, dt,
                            activeWd, activeSat, activeSun,
                            activeWd  ? eH : 0,
                            activeSat ? eH : 0,
                            activeSun ? eH : 0
                        ));
                    }
                }

                // Агрегиране върху всички дни в месеца
                double monthEnergy = sumWd * days.Weekdays
                                   + sumSat * days.Saturdays
                                   + sumSun * days.Sundays;

                perMonthKWh[month] = monthEnergy;
                seasonEnergy_kWh  += monthEnergy;

                if (collectDebug)
                {
                    debugMonths!.Add(new NightVentMonthDebug(
                        month,
                        days.Weekdays, days.Saturdays, days.Sundays,
                        sumWd, sumSat, sumSun,
                        monthEnergy,
                        hourDebugList!
                    ));
                }
            }

            double specificKWhM2 = seasonEnergy_kWh / input.AreaM2;

            NightVentDebugDetails? debugDetails = null;
            if (collectDebug)
            {
                debugDetails = new NightVentDebugDetails(
                    vdotNight, seasonEnergy_kWh, specificKWhM2, debugMonths!);
            }

            return new NightVentResult(seasonEnergy_kWh, specificKWhM2, perMonthKWh, debugDetails);
        }

        /// <summary>
        /// Конвертира <see cref="WeeklyTimeRange"/> (StartTime / EndTime) от
        /// <see cref="EE.Doklad.Models.WeeklyTimeRange"/> в масив от 24 bool стойности.
        ///
        /// Ако StartTime == EndTime → нито един час не е активен.
        /// Overnight диапазон (End &lt; Start) се поддържа.
        /// </summary>
        /// <param name="startTime">Начало на активния период.</param>
        /// <param name="endTime">Край на активния период (включително).</param>
        /// <returns>Масив от 24 bool стойности.</returns>
        public static bool[] BuildActiveHoursFromTimeRange(TimeSpan startTime, TimeSpan endTime)
        {
            var active = new bool[24];

            int startH = (int)startTime.TotalHours % 24;
            int endH   = (int)endTime.TotalHours   % 24;

            if (startTime == endTime)
                return active;  // Няма обитаване

            if (endH >= startH)
            {
                // Нормален диапазон: [startH, endH)
                for (int h = startH; h < endH && h < 24; h++)
                    active[h] = true;
            }
            else
            {
                // Overnight: [startH, 24) ∪ [0, endH)
                for (int h = startH; h < 24; h++) active[h] = true;
                for (int h = 0; h < endH; h++)    active[h] = true;
            }

            return active;
        }

        /// <summary>
        /// Пример за употреба (без unit-test framework).
        /// Показва изчисление с примерни данни за зона 7 (София),
        /// май–август, Ti=25°C, VdotSpec=2 m³/h·m², A=500 m², активна вентилация 22:00–06:00.
        /// </summary>
        public static string Example()
        {
            // ── Климатичен профил ──────────────────────────────────────────────
            // Синтетичен профил: варира от 12°C (нощ) до 28°C (обед)
            static double[] SyntheticDayProfile(double minT, double maxT)
            {
                var p = new double[24];
                for (int h = 0; h < 24; h++)
                    p[h] = minT + (maxT - minT) * (1 - Math.Cos((h - 6) * Math.PI / 12.0)) / 2.0;
                return p;
            }

            var profiles = new Dictionary<int, ClimateHourlyProfile>
            {
                { 5, new ClimateHourlyProfile(SyntheticDayProfile(11, 22)) },
                { 6, new ClimateHourlyProfile(SyntheticDayProfile(14, 28)) },
                { 7, new ClimateHourlyProfile(SyntheticDayProfile(16, 30)) },
                { 8, new ClimateHourlyProfile(SyntheticDayProfile(14, 27)) },
            };

            // ── График: активен 22:00 до 06:00 (overnight) ───────────────────
            var nightActive = BuildActiveHoursFromTimeRange(
                TimeSpan.FromHours(22), TimeSpan.FromHours(6));

            var schedule = new NightVentSchedule(nightActive, nightActive, nightActive);

            // ── Входен DTO ────────────────────────────────────────────────────
            var input = new NightVentInput(
                AreaM2:                   500.0,
                SpecAirflowM3phM2:        2.0,
                IndoorCoolingSetpointC:   25.0,
                CoolingSeasonMonths:      new[] { 5, 6, 7, 8 },
                ClimateProfiles:          profiles,
                Schedule:                 schedule
            );

            var result = Calculate(input, collectDebug: true);

            var sb = new StringBuilder();
            sb.AppendLine("=== NightVentilationCalculator.Example() ===");
            sb.AppendLine($"AreaM2              = {input.AreaM2} m²");
            sb.AppendLine($"VdotSpecNight       = {input.SpecAirflowM3phM2} m³/h·m²");
            sb.AppendLine($"VdotNight           = {result.DebugDetails?.VdotNight_m3ph:F1} m³/h");
            sb.AppendLine($"Ti                  = {input.IndoorCoolingSetpointC} °C");
            sb.AppendLine($"Season              = май–август");
            sb.AppendLine($"TotalKWh            = {result.TotalKWh:F2} kWh");
            sb.AppendLine($"SpecificKWhPerM2    = {result.SpecificKWhPerM2:F3} kWh/m²");
            sb.AppendLine();

            if (result.DebugDetails != null)
            {
                foreach (var m in result.DebugDetails.PerMonth)
                {
                    sb.AppendLine($"  Месец {m.Month,2}: Weekdays={m.Weekdays} Sat={m.Saturdays} Sun={m.Sundays}  " +
                                  $"sumWd={m.SumWeekday_kWh:F3}  energy={m.MonthEnergy_kWh:F2} kWh");
                }
            }

            return sb.ToString();
        }

        // ── Private helpers ───────────────────────────────────────────────────────

        /// <summary>
        /// Изчислява брой дни по тип (weekday/saturday/sunday) за даден месец.
        /// Закръглянето се извършва с гаранция сборът да е = daysInMonth.
        /// </summary>
        private static (int Weekdays, int Saturdays, int Sundays) ComputeDayTypeCounts(
            IReadOnlyDictionary<int, int>? daysInMonthDict, int month)
        {
            int total = GetDays(daysInMonthDict, month);

            // Закръглена разбивка
            int saturdays = (int)Math.Round(total * 1.0 / 7.0);
            int sundays   = (int)Math.Round(total * 1.0 / 7.0);
            int weekdays  = total - saturdays - sundays;

            // Гаранция: weekdays ≥ 0
            if (weekdays < 0)
            {
                weekdays  = 0;
                int excess = saturdays + sundays - total;
                saturdays = Math.Max(0, saturdays - (excess + 1) / 2);
                sundays   = total - weekdays - saturdays;
            }

            return (weekdays, saturdays, sundays);
        }

        private static int GetDays(IReadOnlyDictionary<int, int>? dict, int month)
        {
            if (dict != null && dict.TryGetValue(month, out int d) && d > 0)
                return d;
            return DefaultDaysInMonth2024[month - 1];
        }

        /// <summary>
        /// Помощен метод: изгражда NightVentMonthDebug при нулев дебит.
        /// </summary>
        private static NightVentMonthDebug BuildMonthDebug(
            int month, double vdotNight, NightVentInput input, int totalDays)
        {
            var days = ComputeDayTypeCounts(input.DaysInMonth, month);
            var hours = new List<NightVentHourDebug>(24);
            var profile = input.ClimateProfiles.TryGetValue(month, out var p) ? p : null;

            for (int h = 0; h < 24; h++)
            {
                double te = profile?.TempAt(h) ?? 0;
                double dt = Math.Max(0, input.IndoorCoolingSetpointC - te);
                hours.Add(new NightVentHourDebug(h, te, dt, false, false, false, 0, 0, 0));
            }

            return new NightVentMonthDebug(month, days.Weekdays, days.Saturdays, days.Sundays,
                0, 0, 0, 0, hours);
        }
    }
}
