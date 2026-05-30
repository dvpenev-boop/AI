using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EE.Doklad.Services.EecalcClimate;
using Xunit;

namespace EE.Doklad.Tests.Validation.FullOracle
{
    public sealed class EECalcFullOracleTests
    {
        [Fact]
        public void FullOracle_ExportsAllRequiredDebugCsvFiles()
        {
            var snapshot = LoadFixture001();
            var context = new EECalcOracleContext(snapshot.Fixture, input: snapshot.Input);
            var result = new EECalcFullOracle().Run(context);
            var debugDirectory = Path.Combine(FindRepositoryRoot(), "test-results", "validation", "full_oracle_export_files");

            result.ExportDebugCsv(debugDirectory);

            Assert.Contains(result.DebugRows, row => row.Module == EECalcOracleModule.R1_R2_CalendarAndDegreeHours.ToString());
            Assert.Contains(result.DebugRows, row => row.Module == EECalcOracleModule.R10_AggregationPrimaryCo2Class.ToString()
                && row.Fields["FuelInputEnum"] == "Fuel1"
                && row.Fields["FuelReportBucket"] == "Fuel8");
            Assert.All(RequiredDebugFiles, file => Assert.True(File.Exists(Path.Combine(debugDirectory, file)), file));
            Assert.True(File.Exists(Path.Combine(debugDirectory, "full_result.csv")));
        }

        [Fact]
        public void StrictClimateProvider_PreservesKdData001JanuaryTemperatures()
        {
            var provider = new LegacyEecalcXmlClimateDataProvider(ClimateProviderMode.LegacyEECalcStrict);

            Assert.Equal(-1.9, provider.GetMonthlyAvgTemp(1, Month.January), precision: 12);
            Assert.Equal(-0.5, provider.GetMonthlyAvgTemp(2, Month.January), precision: 12);
            Assert.Equal(-0.1, provider.GetMonthlyAvgTemp(3, Month.January), precision: 12);
        }

        [Fact]
        public void LegacyAggregation_PreservesKdA001AndKdA009()
        {
            var values = new Dictionary<EECalcFuel, double>
            {
                [EECalcFuel.Fuel1] = 10.0,
                [EECalcFuel.Fuel2] = 2.0,
                [EECalcFuel.Fuel8] = 8.0
            };

            Assert.Equal(30.0, EECalcLegacyAggregation.CalculateTotalFuelWithDuplicateFuel1(values), precision: 12);
            Assert.Equal(EECalcFuel.Fuel8, EECalcLegacyAggregation.MapFuelReportBucket(EECalcFuel.Fuel1));
            Assert.Equal(EECalcFuel.Fuel1, EECalcLegacyAggregation.MapFuelReportBucket(EECalcFuel.Fuel8));
            Assert.Equal(EECalcFuel.Fuel2, EECalcLegacyAggregation.MapFuelReportBucket(EECalcFuel.Fuel2));
        }

        [Fact]
        public void CsvParityComparer_ReportsVisibleClassifiedMismatches()
        {
            var real = new[]
            {
                new OracleCsvRecord
                {
                    FixtureId = "f",
                    Module = "R5_HeatingBalance",
                    FormulaField = "Qnd",
                    Variant = "Actual",
                    ZoneIndex = 1,
                    Month = 1,
                    RawValue = "10",
                    Value = 10.0
                }
            };
            var oracle = new[]
            {
                new OracleCsvRecord
                {
                    FixtureId = "f",
                    Module = "R5_HeatingBalance",
                    FormulaField = "Qnd",
                    Variant = "Actual",
                    ZoneIndex = 1,
                    Month = 1,
                    RawValue = "12",
                    Value = 12.0
                }
            };

            var comparison = new CsvParityComparer().Compare(real, oracle);

            var row = Assert.Single(comparison.Rows);
            Assert.False(row.ExactMatch);
            Assert.Equal("10", row.ExpectedValue);
            Assert.Equal("12", row.ActualValue);
            Assert.Equal(2.0, row.AbsoluteDelta, precision: 12);
            Assert.Equal(CsvMismatchClassification.Unclassified, row.Classification);

            var reportPath = Path.Combine(
                FindRepositoryRoot(),
                "test-results",
                "validation",
                "full_oracle",
                "csv_parity_report_shape.csv");

            new CsvParityComparer().WriteReport(reportPath, comparison);

            var lines = File.ReadAllLines(reportPath);
            Assert.StartsWith("ExactMatch,ExpectedValue,ActualValue,AbsoluteDelta,RelativeDelta,", lines[0], StringComparison.Ordinal);
            Assert.Contains(",10,12,2,", lines[1], StringComparison.Ordinal);
        }

        [Fact]
        public void FullOracle_Fixture001_ProducesComparableFinalTables()
        {
            var snapshot = LoadFixture001();
            var context = new EECalcOracleContext(
                snapshot.Fixture,
                input: snapshot.Input);
            var result = new EECalcFullOracle().Run(context);
            var debugDirectory = Path.Combine(FindRepositoryRoot(), "test-results", "validation", "full_oracle_fixture001");
            var oracleRows = new DecompiledOracleCsvExporter().Export(result, debugDirectory);
            var expectedPath = Path.Combine(
                FindRepositoryRoot(),
                "EE.Doklad.Tests",
                "Validation",
                "FullOracle",
                "Fixtures",
                "fixture_001_expected.json");
            var expectedRows = new RealEECalcTableSnapshotImporter().Load(expectedPath);
            var comparison = new CsvParityComparer().Compare(expectedRows, oracleRows);
            var reportPath = Path.Combine(debugDirectory, "parity_mismatch_report.csv");

            new CsvParityComparer().WriteReport(reportPath, comparison);

            Assert.True(File.Exists(reportPath));
            Assert.All(FinalTables, table => Assert.Contains(result.FullValues.Keys, key => key.StartsWith(table + ".", StringComparison.Ordinal)));
            Assert.DoesNotContain(result.DebugRows, IsPlaceholderRow);
            Assert.True(File.Exists(Path.Combine(debugDirectory, "r7_ventilation.csv")));
            Assert.True(File.Exists(Path.Combine(debugDirectory, "r8_dhw_bgv.csv")));
            Assert.True(File.Exists(Path.Combine(debugDirectory, "r9_lighting_devices.csv")));
            Assert.True(File.Exists(Path.Combine(debugDirectory, "r10_aggregation.csv")));
            Assert.True(File.Exists(Path.Combine(debugDirectory, "full_result.csv")));
        }

        [Fact]
        public void FullOracle_NoModuleEmitsPlaceholderMarkers()
        {
            var snapshot = LoadFixture001();
            var result = new EECalcFullOracle().Run(new EECalcOracleContext(
                snapshot.Fixture,
                input: snapshot.Input));

            Assert.DoesNotContain(result.DebugRows, IsPlaceholderRow);
        }

        [Fact]
        public void FullOracle_HasCoolingFalse_KeepsR6DebugButFiltersCoolingFinalTables()
        {
            var snapshot = LoadFixture001();
            var input = WithAggregation(snapshot.Input, hasCooling: false);
            var result = new EECalcFullOracle().Run(new EECalcOracleContext(snapshot.Fixture, input: input));

            Assert.Contains(result.DebugRows, row => row.Module == EECalcOracleModule.R6_Cooling.ToString());
            Assert.Equal(0.0, result.FullValues["NeededEnergyTable.Cooling.Actual"], precision: 12);
            Assert.Equal(0.0, result.FullValues["NeededEnergyTable.CoolingVentilation.Actual"], precision: 12);
            Assert.Equal(0.0, result.FullValues["PrimaryEnergyTable.Cooling.Actual"], precision: 12);
        }

        [Fact]
        public void FullOracle_HasHeatingFalse_FiltersHeatingAndHeatingVentilationFinalTables()
        {
            var snapshot = LoadFixture001();
            var input = WithAggregation(snapshot.Input, hasHeating: false);
            var result = new EECalcFullOracle().Run(new EECalcOracleContext(snapshot.Fixture, input: input));

            Assert.Contains(result.DebugRows, row => row.Module == EECalcOracleModule.R5_HeatingBalance.ToString());
            Assert.True(result.FullValues["HeatingFinalQndPerArea"] != 0.0);
            Assert.Equal(0.0, result.FullValues["NeededEnergyTable.Heating.Actual"], precision: 12);
            Assert.Equal(0.0, result.FullValues["NeededEnergyTable.HeatingVentilation.Actual"], precision: 12);
            Assert.Equal(0.0, result.FullValues["PrimaryEnergyTable.Heating.Actual"], precision: 12);
        }

        [Fact]
        public void FullOracle_FixtureInputJsonLoader_FailsOnMissingRequiredFields()
        {
            var path = Path.Combine(
                FindRepositoryRoot(),
                "test-results",
                "validation",
                "full_oracle",
                "missing_fixture_input.json");
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, """{"fixtureId":"fixture_001","mode":"LegacyEECalcStrict"}""");

            var exception = Assert.Throws<InvalidOperationException>(() => new RealEECalcInputSnapshotImporter().Load(path));

            Assert.StartsWith("Missing fixture input: ", exception.Message, StringComparison.Ordinal);
        }

        [Fact]
        public void FullOracle_Input_DoesNotAcceptRefPrimaryTotals()
        {
            var json = File.ReadAllText(Path.Combine(
                FindRepositoryRoot(),
                "test-data",
                "full_oracle",
                "fixture_001_input.json"));

            Assert.DoesNotContain("ref1PrimaryTotal", json, StringComparison.Ordinal);
            Assert.DoesNotContain("ref2PrimaryTotal", json, StringComparison.Ordinal);
        }

        [Fact]
        public void FullOracle_EnergyScale_UsesCalculatedRefTotals()
        {
            var snapshot = LoadFixture001();
            var result = new EECalcFullOracle().Run(new EECalcOracleContext(
                snapshot.Fixture,
                input: snapshot.Input));

            Assert.DoesNotContain("EnergyClassScale.Aplus.Max", result.FullValues.Keys);
            Assert.DoesNotContain("EnergyClassScale.G.Min", result.FullValues.Keys);
            Assert.Contains(result.DebugRows, row =>
                row.Module == EECalcOracleModule.R10_AggregationPrimaryCo2Class.ToString()
                && row.Fields.TryGetValue("FormulaField", out var field)
                && field == "EnergyClassScaleUnavailable"
                && row.Fields.TryGetValue("Reason", out var reason)
                && reason == "Ref1/Ref2 primary totals are not calculated yet.");
        }

        private static readonly string[] RequiredDebugFiles =
        {
            "r1_r2_calendar_degreehours.csv",
            "r3_transmission.csv",
            "r4_heating_gains.csv",
            "r5_heating_balance.csv",
            "r6_cooling.csv",
            "r7_ventilation.csv",
            "r8_dhw_bgv.csv",
            "r9_lighting_devices.csv",
            "r10_aggregation.csv"
        };

        private static readonly string[] FinalTables =
        {
            "NeededEnergyTable",
            "NetEnergyTable",
            "NoInputsNetEnergyTable",
            "PrimaryEnergyTable",
            "PrimaryEnergyFuelTable",
            "FuelEnergyTable",
            "EmissionNeededEnergyTable",
            "EmissionEnergySupplyTable",
            "VEI",
            "EnergyClassScale"
        };

        private static RealEECalcInputSnapshot LoadFixture001()
        {
            return new RealEECalcInputSnapshotImporter().Load(Path.Combine(
                FindRepositoryRoot(),
                "test-data",
                "full_oracle",
                "fixture_001_input.json"));
        }

        private static EECalcFullOracleInput WithAggregation(
            EECalcFullOracleInput input,
            bool? hasHeating = null,
            bool? hasCooling = null,
            bool? isBgvUsed = null)
        {
            return new EECalcFullOracleInput
            {
                Ventilation = input.Ventilation,
                DhwBgv = input.DhwBgv,
                LightingDevices = input.LightingDevices,
                Aggregation = new EECalcAggregationInput
                {
                    HasHeating = hasHeating ?? input.Aggregation.HasHeating,
                    HasCooling = hasCooling ?? input.Aggregation.HasCooling,
                    IsBgvUsed = isBgvUsed ?? input.Aggregation.IsBgvUsed,
                    FansAndPumps = input.Aggregation.FansAndPumps,
                    Other = input.Aggregation.Other
                }
            };
        }

        private static bool IsPlaceholderRow(EECalcDebugRow row)
        {
            return row.Fields.Values.Any(value =>
                value.Contains("oracle extraction is not wired yet", StringComparison.OrdinalIgnoreCase)
                || value.Contains("placeholder", StringComparison.OrdinalIgnoreCase)
                || value.Contains("not implemented", StringComparison.OrdinalIgnoreCase));
        }

        private static string FindRepositoryRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null && !File.Exists(Path.Combine(directory.FullName, "EE Doklad.sln")))
            {
                directory = directory.Parent;
            }

            return directory?.FullName ?? Directory.GetCurrentDirectory();
        }
    }
}
