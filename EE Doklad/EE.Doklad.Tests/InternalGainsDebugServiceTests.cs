using System;
using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Models;
using EE.Doklad.Services;
using Xunit;

namespace EE.Doklad.Tests
{
    // ══════════════════════════════════════════════════════════════════════════
    // Секция 23 – Unit тестове за вътрешни топлинни печалби (формули 3.30–3.33)
    // ══════════════════════════════════════════════════════════════════════════

    public class InternalGainsDebugServiceTests
    {
        private const double Tol = 1e-4; // tolerance за floating сравнения
        private readonly InternalGainsDebugService _svc = new();

        // ── Фабрики за базови входни данни ────────────────────────────────────

        /// <summary>
        /// Базов Heating вход: Зона 1 (21 окт – 20 апр), пълен февруари, 8h работни дни.
        /// </summary>
        private static InternalGainsDebugInput BaseHeating(int month = 2)
            => new()
            {
                ZoneId  = 1,
                Month   = month,
                Mode    = EpbMode.Heating,
                AreaHeat_m2 = 100.0,
                AreaCool_m2 = 80.0,
                HeatingWorkdaysHours = 8.0,
                HeatingSaturdayHours = 0.0,
                HeatingSundayHours   = 0.0,
                HeatingSeasonStartMonth = 10, HeatingSeasonStartDay = 21,
                HeatingSeasonEndMonth   = 4,  HeatingSeasonEndDay   = 20,
                DaysOff = new int[12], // нула дни почивни
                YearRef = 2024
            };

        /// <summary>
        /// Базов Cooling вход: 1 юни – 30 септ, 10h работни дни.
        /// </summary>
        private static InternalGainsDebugInput BaseCooling(int month = 7)
            => new()
            {
                ZoneId  = 1,
                Month   = month,
                Mode    = EpbMode.Cooling,
                AreaHeat_m2 = 100.0,
                AreaCool_m2 = 80.0,
                CoolingWorkdaysHours = 10.0,
                CoolingSaturdayHours =  5.0,
                CoolingSundayHours   =  0.0,
                CoolingSeasonStartMonth = 6, CoolingSeasonStartDay = 1,
                CoolingSeasonEndMonth   = 9, CoolingSeasonEndDay   = 30,
                DaysOff = new int[12],
                YearRef = 2024
            };

        private static InternalGainsSourceInput PowerSource(
            double powerW, InternalGainsCategory cat = InternalGainsCategory.Occupants,
            bool isCold = false)
            => new()
            {
                SourceId    = "src-1",
                Description = "test source",
                Kind        = InternalGainsSourceKind.PowerWatts,
                Power_W     = powerW,
                Category    = cat,
                IsColdSource = isCold
            };

        private static InternalGainsSourceInput SpecificSource(
            double kWhM2Year, InternalGainsCategory cat = InternalGainsCategory.Lighting)
            => new()
            {
                SourceId    = "src-spec",
                Description = "test specific",
                Kind        = InternalGainsSourceKind.SpecificAnnual_kWhM2Year,
                SpecificAnnual_kWhM2Year = kWhM2Year,
                Category    = cat,
                IsColdSource = false
            };

        // ══════════════════════════════════════════════════════════════════════
        // Тест 1: Формула 3.33 – Power source в пълен месец (Heating)
        // Q = Φ * t_m / 1000
        // ══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Test01_Formula333_PowerSource_FullMonth_Heating()
        {
            var input = BaseHeating(month: 2); // февруари 2024 – изцяло в сезона
            input.Sources.Add(PowerSource(1000.0)); // 1000 W

            var r = _svc.Calculate(input);

            Assert.True(r.IsSuccess, string.Join("; ", r.ValidationErrors));
            Assert.True(r.TimeInfo.TotalActiveHours_t_m > 0, "t_m трябва да е > 0 за февруари в отоплителен сезон.");

            // Q = 1000 W * t_m h / 1000 = t_m kWh
            double expected = r.TimeInfo.TotalActiveHours_t_m; // kWh = W * h / 1000 when W=1000
            Assert.InRange(r.SourceRows[0].Q_int_k_m_kWh, expected - Tol, expected + Tol);
        }

        // ══════════════════════════════════════════════════════════════════════
        // Тест 2: Знак на студен источник – трябва да е отрицателен
        // ══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Test02_ColdSource_NegativeSign()
        {
            var input = BaseHeating(month: 2);
            input.Sources.Add(PowerSource(500.0, isCold: true));

            var r = _svc.Calculate(input);

            Assert.True(r.IsSuccess);
            Assert.True(r.SourceRows[0].Q_int_k_m_kWh < 0,
                "Студеният источник трябва да дава отрицателен принос.");
            Assert.True(r.Q_HC_int_ztc_m_kWh < 0,
                "Крайният Q трябва да е отрицателен при само студен источник.");
        }

        // ══════════════════════════════════════════════════════════════════════
        // Тест 3: Месец извън сезона → t_m = 0, Q = 0
        // ══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Test03_MonthOutsideSeason_ZeroGains()
        {
            // Отоплителен сезон окт-апр → юли е извън него
            var input = BaseHeating(month: 7);
            input.Sources.Add(PowerSource(2000.0));

            var r = _svc.Calculate(input);

            Assert.True(r.IsSuccess);
            Assert.Equal(0.0, r.TimeInfo.TotalActiveHours_t_m, precision: 5);
            Assert.Equal(0.0, r.Q_HC_int_ztc_m_kWh, precision: 5);
        }

        // ══════════════════════════════════════════════════════════════════════
        // Тест 4: Частичен месец – начало на охладителния сезон (1 юни)
        // Сезон 15 юни – 30 септ → юни е частичен (само 16 дни от 30)
        // ══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Test04_PartialMonth_CoolingStart_June()
        {
            var input = BaseCooling(month: 6);
            // Сезон 15 юни – 30 септ
            input.CoolingSeasonStartMonth = 6; input.CoolingSeasonStartDay = 15;
            input.CoolingSeasonEndMonth   = 9; input.CoolingSeasonEndDay   = 30;
            input.Sources.Add(PowerSource(500.0, InternalGainsCategory.Appliances));

            var r = _svc.Calculate(input);

            Assert.True(r.IsSuccess);
            Assert.True(r.TimeInfo.IsPartialMonth, "Юни 15-30 трябва да е частичен.");

            // t_m при юни 15-30 трябва да е по-малко от пълния юни
            var inputFull = BaseCooling(month: 6);
            inputFull.Sources.Add(PowerSource(500.0, InternalGainsCategory.Appliances));
            var rFull = _svc.Calculate(inputFull);

            Assert.True(r.TimeInfo.TotalActiveHours_t_m < rFull.TimeInfo.TotalActiveHours_t_m,
                "Частичният юни трябва да има по-малко часове от пълния.");
        }

        // ══════════════════════════════════════════════════════════════════════
        // Тест 5: Частичен месец – край на отоплителния сезон (20 април)
        // ══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Test05_PartialMonth_HeatingEnd_April()
        {
            var input = BaseHeating(month: 4); // 21 окт – 20 апр, април е частичен
            input.Sources.Add(PowerSource(1200.0));

            var r = _svc.Calculate(input);

            Assert.True(r.IsSuccess);
            Assert.True(r.TimeInfo.IsPartialMonth, "Април (до 20-и) трябва да е частичен.");
            Assert.True(r.TimeInfo.TotalActiveDays <= 20,
                $"Активните дни в частичен апр трябва да са ≤20, получено: {r.TimeInfo.TotalActiveDays}");
        }

        // ══════════════════════════════════════════════════════════════════════
        // Тест 6: DaysOff намалява активните дни
        // ══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Test06_DaysOff_ReducesActiveHours()
        {
            // Февруари, 0 DaysOff
            var inputNoDays = BaseHeating(month: 2);
            inputNoDays.Sources.Add(PowerSource(1000.0));
            var rNoDays = _svc.Calculate(inputNoDays);

            // Февруари, 5 DaysOff
            var inputWith5 = BaseHeating(month: 2);
            inputWith5.DaysOff[1] = 5; // февруари = индекс 1
            inputWith5.Sources.Add(PowerSource(1000.0));
            var rWith5 = _svc.Calculate(inputWith5);

            Assert.True(rNoDays.TimeInfo.TotalActiveDays > rWith5.TimeInfo.TotalActiveDays,
                "5 DaysOff трябва да намалят активните дни.");
            Assert.True(rNoDays.Q_HC_int_ztc_m_kWh > rWith5.Q_HC_int_ztc_m_kWh,
                "По-малко дни → по-малко Q.");
        }

        // ══════════════════════════════════════════════════════════════════════
        // Тест 7: SpecificAnnual источник – единици и разпределяне
        // Ако t_year > 0 → Q_m = spec * A * t_m / t_year
        // ══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Test07_SpecificAnnualSource_UnitConsistency()
        {
            var input = BaseCooling(month: 7);
            double spec = 10.0; // kWh/m²/year
            double area = input.AreaCool_m2;
            input.Sources.Add(SpecificSource(spec));

            var r = _svc.Calculate(input);

            Assert.True(r.IsSuccess);
            Assert.True(r.SourceRows[0].Q_int_k_m_kWh > 0,
                "SpecificAnnual с t_m > 0 трябва да дава Q > 0.");

            // Q_m / A трябва да е <= spec (само месечна дялва)
            double q_spec = r.SourceRows[0].Q_int_k_m_specific_kWhM2;
            Assert.True(q_spec <= spec + Tol,
                $"Месечната специфична стойност {q_spec:F4} не трябва да надвишава годишната {spec}.");
        }

        // ══════════════════════════════════════════════════════════════════════
        // Тест 8: Множество источници – формула 3.32 сумира правилно
        // Q_total = Q_occ + Q_appliances + Q_light (без студените)
        // ══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Test08_MultipleSourcesAggregation_Formula332()
        {
            var input = BaseHeating(month: 1);
            input.Sources.Add(new InternalGainsSourceInput
            {
                SourceId = "occ",  Category = InternalGainsCategory.Occupants,
                Kind = InternalGainsSourceKind.PowerWatts, Power_W = 300
            });
            input.Sources.Add(new InternalGainsSourceInput
            {
                SourceId = "app",  Category = InternalGainsCategory.Appliances,
                Kind = InternalGainsSourceKind.PowerWatts, Power_W = 500
            });
            input.Sources.Add(new InternalGainsSourceInput
            {
                SourceId = "light", Category = InternalGainsCategory.Lighting,
                Kind = InternalGainsSourceKind.PowerWatts, Power_W = 200,
                IsColdSource = false
            });

            var r = _svc.Calculate(input);

            Assert.True(r.IsSuccess);
            double sumFromRows = r.SourceRows.Sum(s => s.Q_int_k_m_kWh);
            Assert.InRange(r.Q_HC_int_dir_z_m_kWh, sumFromRows - Tol, sumFromRows + Tol);
        }

        // ══════════════════════════════════════════════════════════════════════
        // Тест 9: Fallback при липсващ график за Heating
        // ══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Test09_FallbackSchedule_WhenNoHeatingHours()
        {
            var input = BaseHeating(month: 1);
            input.HeatingWorkdaysHours = 0;
            input.HeatingSaturdayHours = 0;
            input.HeatingSundayHours   = 0;
            input.Sources.Add(PowerSource(1000.0));

            var r = _svc.Calculate(input);

            Assert.True(r.IsSuccess);
            Assert.True(r.TimeInfo.HoursFallbackUsed, "При липса на график трябва да се ползва fallback.");
            Assert.NotEmpty(r.FallbacksUsed);
            // Fallback дава 10h/ден → трябва да има ненулев t_m
            Assert.True(r.TimeInfo.TotalActiveHours_t_m > 0,
                "Fallback трябва да осигури ненулев t_m.");
        }

        // ══════════════════════════════════════════════════════════════════════
        // Тест 10: Валидация – невалиден месец
        // ══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Test10_Validation_InvalidMonth_ReturnsError()
        {
            var input = BaseHeating(month: 13);
            var r = _svc.Calculate(input);

            Assert.False(r.IsSuccess);
            Assert.NotEmpty(r.ValidationErrors);
            Assert.Contains(r.ValidationErrors, e => e.Contains("13"));
        }

        // ══════════════════════════════════════════════════════════════════════
        // Тест 11: Валидация – отрицателна мощност → грешка
        // ══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Test11_Validation_NegativePower_ReturnsError()
        {
            var input = BaseHeating(month: 2);
            input.Sources.Add(PowerSource(-100.0)); // отрицателна без IsColdSource

            var r = _svc.Calculate(input);

            Assert.False(r.IsSuccess);
            Assert.Contains(r.ValidationErrors, e => e.Contains("отрицателен"));
        }

        // ══════════════════════════════════════════════════════════════════════
        // Тест 12: Формула 3.30 = 3.32 при липса на некондиционирани зони
        // ══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Test12_Formula330_EqualsFormula332_WhenNoUncondZones()
        {
            var input = BaseHeating(month: 3);
            input.Sources.Add(PowerSource(800.0));

            var r = _svc.Calculate(input);

            Assert.True(r.IsSuccess);
            Assert.InRange(r.Q_HC_int_ztc_m_kWh,
                r.Q_HC_int_dir_z_m_kWh - Tol,
                r.Q_HC_int_dir_z_m_kWh + Tol);
            Assert.Equal(0.0, r.Q_HC_int_uncond_contribution_kWh, precision: 5);
        }

        // ══════════════════════════════════════════════════════════════════════
        // Тест 13: Cooling режим – A_cool се използва (не A_heat)
        // ══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Test13_CoolingMode_UsesAreaCool()
        {
            var input = BaseCooling(month: 8);
            input.AreaHeat_m2 = 200.0;  // различна площ
            input.AreaCool_m2 = 80.0;
            input.Sources.Add(PowerSource(600.0, InternalGainsCategory.Appliances));

            var r = _svc.Calculate(input);

            Assert.True(r.IsSuccess);
            Assert.InRange(r.AreaUsed_m2, 80.0 - Tol, 80.0 + Tol);
        }

        // ══════════════════════════════════════════════════════════════════════
        // Тест 14: Частичен месец – начало на отоплителния сезон (21 октомври)
        // Октомври е частичен – само 11 дни (21-31)
        // ══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Test14_PartialMonth_HeatingStart_October()
        {
            var input = BaseHeating(month: 10);
            input.Sources.Add(PowerSource(1000.0));

            var r = _svc.Calculate(input);

            Assert.True(r.IsSuccess);
            Assert.True(r.TimeInfo.IsPartialMonth, "Октомври (от 21-и) трябва да е частичен.");
            // Октомври има 31 дни, но само 11 са в сезона (21-31)
            Assert.True(r.TimeInfo.TotalActiveDays <= 11,
                $"Активните дни в частичен окт трябва да са ≤11, получено: {r.TimeInfo.TotalActiveDays}");
        }

        // ══════════════════════════════════════════════════════════════════════
        // Тест 15: Единици – W × h / 1000 = kWh
        // ══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Test15_Units_WattsTimesHoursDivBy1000_EqualskWh()
        {
            var input = BaseHeating(month: 1);
            input.HeatingWorkdaysHours = 8.0;
            input.Sources.Add(PowerSource(1000.0)); // 1000 W

            var r = _svc.Calculate(input);

            Assert.True(r.IsSuccess);
            double t_m = r.TimeInfo.TotalActiveHours_t_m;
            double expected_kWh = 1000.0 * t_m / 1000.0; // = t_m kWh

            Assert.InRange(r.SourceRows[0].Q_int_k_m_kWh,
                expected_kWh - Tol,
                expected_kWh + Tol);
        }

        // ══════════════════════════════════════════════════════════════════════
        // Тест 16: Debug трейс е непразен при успешно изчисление
        // ══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Test16_DebugTrace_PopulatedOnSuccess()
        {
            var input = BaseHeating(month: 2);
            input.Sources.Add(PowerSource(500.0));

            var r = _svc.Calculate(input);

            Assert.True(r.IsSuccess);
            Assert.False(string.IsNullOrWhiteSpace(r.Formula333Summary));
            Assert.False(string.IsNullOrWhiteSpace(r.Formula332Trace));
            Assert.False(string.IsNullOrWhiteSpace(r.Formula330Trace));
            Assert.NotNull(r.SourceRows[0].FormulaTrace);
            Assert.Contains("3.33", r.Formula333Summary);
            Assert.Contains("3.32", r.Formula332Trace);
            Assert.Contains("3.30", r.Formula330Trace);
        }
    }
}
