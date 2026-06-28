// TestFixture.cs — пълен fixture за реалния обект от Decompile_proba.pdf
// Зона 7 (Sofia), Жилищна сграда, 1000 m² кондиционирана площ
// Отопителен сезон: 15 октомври – 23 април
// Режим: LegacyEECalcStrict (DefaultParams.xml, включително KD-DATA-001)

using System.Collections.Generic;
using EE.Doklad.Tests.Validation;
using EE.Doklad.Tests.Validation.FullOracle;

namespace EecalcTest
{
    public static class TestFixture
    {
        // ----------------------------------------------------------------
        // ВАЖНО: стойностите за α (alfa/e) са от колоната "α" в таблицата.
        // Всички прозорци използват ε = 0.5 (стандартна стойност по подразбиране).
        // ΣL и ΣX = 0 за всички елементи (няма въведени топлинни мостове).
        // ----------------------------------------------------------------

        public static EecalcEnvelopeFixture Build() => new()
        {
            Id = "Проба_05.2026",

            Calculation = new EecalcValidationFixture
            {
                ClimateZoneId      = 7,        // XML Number 6 (JSON ZoneId = Number + 1)
                HeatedArea         = 1000.0,
                HeatedVolume       = 2500.0,   // не е видимо в PDF → стандартна оценка 2.5×площ
                Infiltration       = 0.50,
                HeatCapacity       = 46.0,
                MetabolicHeat      = 3.16,
                LatentMetabolicHeat = 0.84,
                ProjectTemperature = 20.0,
                NonProjectTemperature = 16.0,
                ProjectHumidity = 50.0,
                FlowTemperature = 22.0,
                FlowRelativeHumidity = 50.0,
                VentilationDebit = 0.500,
                LightsCoolingPower = 0.6,
                BalancedDevicesCoolingPower = 3.8,
                LightsCoolingWorkSchedule = 56.0,
                BalancedDevicesCoolingWorkSchedule = 25.0,

                // Отопителен сезон
                FirstMonth = 10,
                LastMonth  = 4,
                FirstDay   = 15,
                LastDay    = 23,

                // Работни графици (жилищна — 0–24ч всеки ден)
                WorkdaySchedule  = new EecalcDailySchedule { StartHour = 0, EndHour = 24 },
                SaturdaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 24 },
                SundaySchedule   = new EecalcDailySchedule { StartHour = 0, EndHour = 0 },

                OccupantsWorkdaySchedule  = new EecalcDailySchedule { StartHour = 0, EndHour = 24 },
                OccupantsSaturdaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 0 },
                OccupantsSundaySchedule   = new EecalcDailySchedule { StartHour = 0, EndHour = 0 },

                VentilationWorkdaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 20 },
                VentilationSaturdaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 20 },
                VentilationSundaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 0 },
                NightVentilationWorkdaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 0 },
                NightVentilationSaturdaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 0 },
                NightVentilationSundaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 0 },

                HolidaysByMonth = new Dictionary<int, int>
                {
                    { 1, 1 },
                    { 3, 2 },
                    { 4, 1 },
                    { 11, 2 },
                    { 12, 3 }
                },
            },

            // ============================================================
            // ФАСАДИ — Външни стени + Прозорци по посока
            // Колони: A(m²) | U(W/m²K) | ΣLψ(W/K) | ΣXχ(W/K) | ε | α
            // Прозорци: A(m²) | U(W/m²K) | g | ε
            // ============================================================

            // --- СЕВЕР: стена 50 m², U=0.250, ε=0.90, α=0.30
            //            прозорец 20 m², U=1.400, g=0.61, ε=0.84
            NorthWalls = new EecalcWallDirectionFixture
            {
                OuterA   = new[] { 50.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterU   = new[] { 0.250, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterSumL = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterSumX = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                AccumulateOuterA    = 50.0,
                AccumulateOuterU    = 0.250,
                AccumulateOuterAlfa = 0.30,
                AccumulateOuterE    = 0.90,
                AccumulateWindowA = 20.0,
                AccumulateWindowU = 1.400,
                AccumulateWindowG = 0.50,
                AccumulateWindowE = 0.80,
            },

            // --- СЕВЕРОИЗТОК: стена 50 m², U=0.250, ε=0.90, α=0.30
            //                  прозорец 0 m² (няма прозорци)
            NorthEastWalls = new EecalcWallDirectionFixture
            {
                OuterA   = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterU   = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterSumL = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterSumX = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                AccumulateOuterA    = 0.0,
                AccumulateOuterU    = 0.00,
                AccumulateOuterAlfa = 0.00,
                AccumulateOuterE    = 0.00,
                AccumulateWindowA = 0.0,
                AccumulateWindowU = 0.0,
                AccumulateWindowG = 0.54,
                AccumulateWindowE = 0.5,
            },

            // --- ИЗТОК: стена 50 m², U=0.250, ε=0.90, α=0.30
            //            прозорец 20 m², U=1.400, g=0.54, ε=0.84
            EastWalls = new EecalcWallDirectionFixture
            {
                OuterA   = new[] { 50.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterU   = new[] { 0.250, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterSumL = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterSumX = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                AccumulateOuterA    = 50.0,
                AccumulateOuterU    = 0.250,
                AccumulateOuterAlfa = 0.30,
                AccumulateOuterE    = 0.90,
                AccumulateWindowA = 0.0,
                AccumulateWindowU = 0.000,
                AccumulateWindowG = 0.0,
                AccumulateWindowE = 0.0,
            },

            // --- ЮГОИЗТОК: стена 50 m², U=0.250, ε=0.90, α=0.30
            //               прозорец 20 m², U=1.400, g=0.54, ε=0.84
            SouthEastWalls = new EecalcWallDirectionFixture
            {
                OuterA   = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterU   = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterSumL = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterSumX = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                AccumulateOuterA    = 0.0,
                AccumulateOuterU    = 0.0,
                AccumulateOuterAlfa = 0.0,
                AccumulateOuterE    = 0.0,
                AccumulateWindowA = 0.0,
                AccumulateWindowU = 0.000,
                AccumulateWindowG = 0.0,
                AccumulateWindowE = 0.0,
            },

            // --- ЮГ: стена 50 m², U=0.250, ε=0.90, α=0.30
            //         прозорец 20 m², U=1.400, g=0.54, ε=0.84
            SouthWalls = new EecalcWallDirectionFixture
            {
                OuterA   = new[] { 50.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterU   = new[] { 0.250, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterSumL = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterSumX = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                AccumulateOuterA    = 50.0,
                AccumulateOuterU    = 0.250,
                AccumulateOuterAlfa = 0.30,
                AccumulateOuterE    = 0.90,
                AccumulateWindowA = 20.0,
                AccumulateWindowU = 1.400,
                AccumulateWindowG = 0.50,
                AccumulateWindowE = 0.80,
            },

            // --- ЮГОЗАПАД: стена 50 m², U=0.250, ε=0.90, α=0.30
            //               прозорец 0 m² (няма)
            SouthWestWalls = new EecalcWallDirectionFixture
            {
                OuterA   = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterU   = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterSumL = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterSumX = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                AccumulateOuterA    = 0.0,
                AccumulateOuterU    = 0.0,
                AccumulateOuterAlfa = 0.00,
                AccumulateOuterE    = 0.00,
                AccumulateWindowA = 0.0,
                AccumulateWindowU = 0.0,
                AccumulateWindowG = 0.0,
                AccumulateWindowE = 0.0,
            },

            // --- ЗАПАД: стена 50 m², U=3.092, ε=0.90, α=0.30
            //            прозорец 0 m² (няма прозорци — от таблицата g=0.00, A=0.00)
            // ЗАБЕЛЕЖКА: U=3.092 е видимо само на таб Запад — различна конструкция!
            WestWalls = new EecalcWallDirectionFixture
            {
                OuterA   = new[] { 50.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterU   = new[] { 0.250, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterSumL = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterSumX = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                AccumulateOuterA    = 50.0,
                AccumulateOuterU    = 0.250,
                AccumulateOuterAlfa = 0.30,
                AccumulateOuterE    = 0.90,
                AccumulateWindowA = 0.0,
                AccumulateWindowU = 0.0,
                AccumulateWindowG = 0.0,
                AccumulateWindowE = 0.5,
            },

            // --- СЕВЕРОЗАПАД: стена 50 m², U=0.250, ε=0.90, α=0.30
            //                  прозорец 0 m²
            NorthWestWalls = new EecalcWallDirectionFixture
            {
                OuterA   = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterU   = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterSumL = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OuterSumX = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                AccumulateOuterA    = 0.0,
                AccumulateOuterU    = 0.0,
                AccumulateOuterAlfa = 0.0,
                AccumulateOuterE    = 0.0,
                AccumulateWindowA = 0.0,
                AccumulateWindowU = 0.0,
                AccumulateWindowG = 0.0,
                AccumulateWindowE = 0.0,
            },

            // ============================================================
            // ПОКРИВ — таб Покрив, страница 4
            // Непрозрачна: A=100 m², U=0.250, ε=0.90, α=0.03
            // Прозрачни елементи на покрив: 0 (всички нули)
            // Тавани към съседна зона: 0
            // ============================================================
            Roof = new EecalcRoofFixture
            {
                NonTransparentA   = new[] { 100.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                NonTransparentU   = new[] { 0.250, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                NonTransparentSumL = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                NonTransparentSumX = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                TransparentA = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                TransparentU = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                TransparentG = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                TransparentE = new[] { 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5 },
                AccumulateNonTransparentA    = 100.0,
                AccumulateNonTransparentU    = 0.250,
                AccumulateNonTransparentAlfa = 0.85,
                AccumulateNonTransparentE    = 0.90,
                // Тавани към съседна зона: 0
                CeilingA = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                CeilingU = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                CeilingW = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
            },

            // ============================================================
            // ПОД — таб Под, страница 4
            // Под (НПБ/ОПБ/външен въздух/земя): A=100 m², U=0.452
            // Под над друга зона: 0
            // ============================================================
            Floor = new EecalcFloorFixture
            {
                AccumulateFloorA = 100.0,
                AccumulateFloorU = 0.452,
                OtherFloorA = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OtherFloorU = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OtherFloorW = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                OtherFloorCoolingS = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
            },
        };

        public static EECalcVentilationInput BuildVentilation() => new()
        {
            Debit = 0.500,
            FlowTemperature = 22.0,
            FlowRelativeHumidity = 50.0,
            ProjectHumidity = 50.0,
            FirstRecEfficiency = 70.0,
            SecondRecEfficiency = 0.0,
            HeatingAirDifference = 4.0,
            MinimumEndTemperature = 3.0,
            Part1 = 100.0,
            Part2 = 0.0,
            WorkdaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 20 },
            SaturdaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 20 },
            SundaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 0 },
        };

        public static EECalcVentilationInput BuildCoolingVentilation() => new()
        {
            Debit = 0.500,
            FlowTemperature = 22.0,
            FlowRelativeHumidity = 40.0,
            ProjectHumidity = 60.0,
            FirstRecEfficiency = 0.0,
            SecondRecEfficiency = 0.0,
            Part1 = 100.0,
            Part2 = 0.0,
            CoolingEfficiency1 = new EECalcEfficiencyChain
            {
                TransmitTempEfficiency = 100.0,
                SupplyNetEfficiency = 100.0,
                Automatic = 100.0,
                EnergyManagement = 96.0,
                GeneratorEfficiency = 97.0,
            },
            WorkdaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 20 },
            SaturdaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 20 },
            SundaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 20 },
        };

        public static EECalcCoolingFansAndPumpsInput BuildCoolingFansAndPumps() => new()
        {
            VentilatorsCool = 0.80,
            VentilatorsOutdoorAirCool = 0.00,
            PumpVentilationCool = 0.20,
            CoolingPump = 1.00,
            EnergyManagement = 96.0,
            OtherCoolingVentilation = 0.80,
            OtherCooling = 0.00,
        };

        public static EECalcDhwBgvInput BuildDhwBgvWithoutSolar() => new()
        {
            Consumption = 600.0,
            TempDifference = 35.0,
            SunEnergy = 0.0,
            Part1 = 100.0,
            Part2 = 0.0,
            Efficiency1 = EECalcDhwEfficiencyChain.OneHundred,
            Efficiency2 = EECalcDhwEfficiencyChain.OneHundred,
        };

        public static EECalcDhwBgvInput BuildDhwBgvWithSolarCollectors(double sunEnergy = 0.0) => new()
        {
            Consumption = 600.0,
            TempDifference = 35.0,
            SunEnergy = sunEnergy,
            Part1 = 100.0,
            Part2 = 0.0,
            Efficiency1 = EECalcDhwEfficiencyChain.OneHundred,
            Efficiency2 = EECalcDhwEfficiencyChain.OneHundred,
            SolarHotWaterTemperature = 55.0,
            SolarColdWaterTemperature = 10.0,
            SolarWaterUsage = 4500.0,
            SolarDaysInWeek = 7.0,
            SolarStartMonth = 1,
            SolarEndMonth = 12,
            AbsorbingSurface = 2.0,
            CollectorsCount = 4.0,
            FR = 0.40,
            FRta = 1.0,
            TrasparentCoverings = 1,
            AcumulatorVolume = 1500.0,
            Pitch = 45.0,
            ImpactEnvironment = 8.0,
            Scheme1Selected = false,
            Scheme2Selected = false,
            CollectorDebit = 1.0,
            SpecialHeatCapacity = 4187.0,
            MTOAEfficiency = 98.0,
            MTOADebit = 1.0,
            MTOASpecialHeatCapacity = 4187.0,
            SerpentineEfficiencyIsUsed = true,
            SerpentineEfficiency = 100.0,
            PumpsVolume = 0.10,
        };

        public static EECalcLightingDevicesInput BuildLightingAndDevices() => new()
        {
            Lights = new EECalcEquipmentInput
            {
                HeatingPower = 0.6,
                HeatingWorkSchedule = 56.0,
                CoolingPower = 0.6,
                CoolingWorkSchedule = 56.0,
                GeneralPower = 0.6,
                GeneralWorkSchedule = 56.0,
                ByMonths = false,
            },
            BalancedDevices = new EECalcEquipmentInput
            {
                HeatingPower = 3.8,
                HeatingWorkSchedule = 25.0,
                CoolingPower = 3.8,
                CoolingWorkSchedule = 25.0,
                GeneralPower = 3.8,
                GeneralWorkSchedule = 25.0,
                ByMonths = false,
            },
            NonBalancedDevices = new EECalcEquipmentInput
            {
                HeatingPower = 2.1,
                HeatingWorkSchedule = 37.0,
                CoolingPower = 2.1,
                CoolingWorkSchedule = 37.0,
                GeneralPower = 2.1,
                GeneralWorkSchedule = 37.0,
                ByMonths = false,
            },
        };
    }
}

// ============================================================
// РЕЗЮМЕ НА ДАННИТЕ ОТ PDF (за верификация):
//
// Фасади — Външни стени по посока (A=50 m² навсякъде):
//   Север:      U=0.250, ε=0.90, α=0.30 | Прозорец: 20m², U=1.400, g=0.61, ε=0.84
//   Североизток: U=0.250                  | Прозорец: 0 m²
//   Изток:      U=0.250                  | Прозорец: 20m², U=1.400, g=0.54, ε=0.84
//   Югоизток:   U=0.250                  | Прозорец: 20m², U=1.400, g=0.54, ε=0.84
//   Юг:         U=0.250                  | Прозорец: 20m², U=1.400, g=0.54, ε=0.84
//   Югозапад:   U=0.250                  | Прозорец: 0 m²
//   ЗАПАД:      U=3.092  ← РАЗЛИЧНА!     | Прозорец: 0 m²
//   Северозапад: U=0.250                  | Прозорец: 0 m²
//
// Покрив: A=100 m², U=0.250, α=0.85, e=0.90
// Под:    A=100 m², U=0.452
//
// Обща площ (от таб Общи стр.6): Стени=200m², Прозорци=400m²,
//   Покрив=100m², Под=100m² → Обща=1100m²
//   (стените от 8 посоки × 50 = 400 m² стени + 80 m² прозорци)
//
// ПРЕДУПРЕЖДЕНИЕ: U=3.092 на Запад е аномалия — вероятно е наружна врата
// или остъклена повърхност. Проверете преди финален тест.
// ============================================================
