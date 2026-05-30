# Manual Full Oracle Runner Audit

## 1. Needed files

### Manual entry point

| File | Classes | Role | Called by | Returns |
| --- | --- | --- | --- | --- |
| `EE.Doklad.Tests/Validation/FullOracle/ManualFullOracleRunner.cs` | `ManualEecalcInput`, `ManualHeatingBreakdown`, `ManualEecalcResult`, `ManualFullOracleRunner` | Manual, non-CSV facade over the existing FullOracle. Maps hand-entered input to `EecalcEnvelopeFixture` and `EECalcFullOracleInput`, runs `EECalcFullOracle`, then projects debug/full values into a readable result. | Manual tests, console snippets, Visual Studio Immediate Window/test code. | `ManualEecalcResult` plus `ToReadableText()`. |
| `EE.Doklad.Tests/Validation/FullOracle/ManualFullOracleRunnerTests.cs` | `ManualFullOracleRunnerTests` | Unit test for one heating-only manual calculation. It does not read JSON fixtures and does not write CSV. | xUnit test runner. | Test pass/fail. |

### Input and fixture models

| File | Classes | Role | Called by | Returns |
| --- | --- | --- | --- | --- |
| `EE.Doklad.Tests/Validation/EecalcValidationFixture.cs` | `EecalcValidationFixture`, `EecalcDailySchedule`, `EecalcSolarRadiationFixture`, `EecalcHourlyWeatherFixture` | Core calculation input model: climate zone, heating period, area, volume, schedules, temperatures, humidity, infiltration, climate maps. | `ManualFullOracleRunner`, `RealEECalcInputSnapshotImporter`, R1/R2, R5, R6, R7, R8, R9 oracles. | Data object consumed by oracles. |
| `EE.Doklad.Tests/Validation/EecalcEnvelopeFixture.cs` | `EecalcEnvelopeFixture`, `EecalcWallDirectionFixture`, `EecalcRoofFixture`, `EecalcFloorFixture` | Envelope model for R3/R4/R5/R6: walls, windows, roof, floor, inner surfaces. In the minimal manual flow these are left at zero, so Qtr is calculated but may be zero unless envelope fields are later added. | `ManualFullOracleRunner`, `RealEECalcInputSnapshotImporter`, `EecalcHtrQtrOracle`, heating/cooling oracles. | Data object consumed by envelope and balance formulas. |
| `EE.Doklad.Tests/Validation/FullOracle/EECalcFullOracleModels.cs` | `EECalcFullOracleInput`, `EECalcVentilationInput`, `EECalcDhwBgvInput`, `EECalcLightingDevicesInput`, `EECalcEquipmentInput`, `EECalcMonthlyEquipmentSchedule`, `EECalcAggregationInput`, `EECalcEfficiencyChain`, `EECalcDhwEfficiencyChain`, `EECalcFullOracleState`, `EECalcMath` | Non-envelope module input and aggregation state. Holds R7/R8/R9 inputs and R10 feature gates. | `ManualFullOracleRunner`, `EECalcFullOracle`, R7/R8/R9/R10 oracles. | Data objects and helper calculations for efficiencies, primary energy, CO2. |
| `EE.Doklad.Tests/Validation/EecalcEnvelopeSnapshotRow.cs` | `EecalcEnvelopeSnapshotRow` | R3 debug snapshot for Htr/Qtr internals. | `EecalcHtrQtrOracle.CalculateParameterQtr`. | One monthly envelope snapshot. |
| `EE.Doklad.Tests/Validation/EecalcMonthlySnapshotRow.cs` | `EecalcMonthlySnapshotRow` | Older monthly validation snapshot DTO. Not needed by the new manual runner, but part of nearby validation model surface. | `EecalcQveOracle.CreateExpectedSnapshot`, comparison helpers. | Snapshot rows for parity-style validation. |

### Oracle runner

| File | Classes | Role | Called by | Returns |
| --- | --- | --- | --- | --- |
| `EE.Doklad.Tests/Validation/FullOracle/EECalcFullOracle.cs` | `EECalcFullOracle`, `EECalcOracleContext`, `EECalcOracleMode`, `EECalcOracleModule`, `EECalcOracleResult`, `EECalcDebugRow` | Main orchestrator. Runs R1/R2 through R10 in order and stores module debug rows plus final table values. The manual flow uses `Run()` only, not `ExportDebugCsv()`. | `ManualFullOracleRunner`, existing FullOracle tests. | `EECalcOracleResult`. |

### Climate providers

| File | Classes | Role | Called by | Returns |
| --- | --- | --- | --- | --- |
| `EE.Doklad/Services/EecalcClimate/IClimateDataProvider.cs` | `IClimateDataProvider` | Interface for monthly temperature, solar radiation, hourly climate data, barometric pressure. | Manual mapper, importer, climate implementations. | Climate values by zone/month. |
| `EE.Doklad/Services/EecalcClimate/LegacyEecalcXmlClimateDataProvider.cs` | `LegacyEecalcXmlClimateDataProvider` | Strict EECalc-compatible climate source from `reference/eecalc-config/DefaultParams.xml`. Used by the manual runner to avoid CSV/snapshot inputs. | `ManualFullOracleRunner`, `EECalcFullOracle.RunR1R2`, R7/R8 providers. | Monthly average temperature, solar radiation, hourly temperature/humidity, pressure. |
| `EE.Doklad/Services/EecalcClimate/CorrectedJsonClimateDataProvider.cs` | `CorrectedJsonClimateDataProvider` | Current/corrected climate provider backed by app JSON seed data. Not used by strict FullOracle manual flow. | Current climate tests/app services. | Current ordinance climate values. |
| `EE.Doklad/Services/EecalcClimate/LegacyEecalcXmlSunEnergyDataProvider.cs` | `LegacyEecalcXmlSunEnergyDataProvider` | Solar DHW helper for R8. | `EECalcDhwBgvOracle`. | Monthly radiation and cloudiness for solar DHW. |
| `EE.Doklad/Services/EecalcClimate/EecalcDataPathResolver.cs` | `EecalcDataPathResolver` | Finds `DefaultParams.xml`/sun XML files from test runtime paths. | Legacy XML providers. | Resolved file path or exception. |
| `EE.Doklad/Services/EecalcClimate/Month.cs` | `Month` | Zero-based month enum used by climate providers. | Climate providers and mappers. | Enum value. |
| `EE.Doklad/Services/EecalcClimate/SolarRadiationData.cs` | `SolarRadiationData` | Climate provider solar DTO. | `IClimateDataProvider.GetSolarRadiation`. | N/E/S/W/H radiation tuple. |
| `EE.Doklad/Services/EecalcClimate/HourlyClimateData.cs` | `HourlyClimateData` | Climate provider hourly DTO. | R6/R7 and manual mapper. | Hour, temperature, humidity tuple. |
| `EE.Doklad/Services/EecalcClimate/ClimateProviderMode.cs` | `ClimateProviderMode` | Selects strict/corrected/current modes. | Legacy/current providers. | Enum value. |
| `EE.Doklad/Services/Climate/*` | `IClimateProvider`, `BgAvgClimateProvider`, `EpwClimateProvider`, `ClimateProviderFactory`, `EpwParser`, `EpwParseResult` | App-level climate providers. They are related climate infrastructure but not on the strict FullOracle manual call path. | App services/tests outside FullOracle. | App climate records/results. |

### R1-R10 modules and aggregation

| File | Classes | Role | Called by | Returns |
| --- | --- | --- | --- | --- |
| `EE.Doklad.Tests/Validation/EecalcMonthlyDaysOracle.cs` | `EecalcMonthlyDaysOracle`, `EecalcMonthlyDaysOracleRow` | R1/R2 calendar period and working/non-working day counts. | `EECalcFullOracle.RunR1R2`, R5/R6/R7/R8/R9. | Monthly rows with days, holidays, weeks. |
| `EE.Doklad.Tests/Validation/EecalcHtrQtrOracle.cs` | `EecalcHtrQtrOracle` | R3 transmission: Hd/Hg/Hu/Htr, degree hours, Qtr. | `EECalcFullOracle.RunR3`, `EecalcMonthlyHeatingOracle`, R3 tests. | Qtr and `EecalcEnvelopeSnapshotRow`. |
| `EE.Doklad.Tests/Validation/EecalcMonthlyHeatingOracle.cs` | `EecalcMonthlyHeatingOracle`, `EecalcMonthlyHeatingOracleRow` | R4/R5 heating gains and balance: solar/internal gains, Qtr, Qve, Qht, Gamma, Ni, FinalQnd. | `EECalcFullOracle.RunR4`, `EECalcFullOracle.RunR5`, R9 thermal input calculations. | Monthly heating rows. |
| `EE.Doklad.Tests/Validation/EecalcMonthlyCoolingOracle.cs` | `EecalcMonthlyCoolingOracle`, `EecalcMonthlyCoolingOracleResult`, `EecalcMonthlyCoolingOracleRow` | R6 cooling calculation. The manual heating-only flow still lets FullOracle execute R6, then R10 gates cooling totals to zero when `HasCooling=false`. | `EECalcFullOracle.RunR6`. | Cooling monthly rows and totals. |
| `EE.Doklad.Tests/Validation/FullOracle/EECalcR7R8R9Oracles.cs` | `EECalcVentilationOracle`, `EECalcDhwBgvOracle`, `EECalcLightingDevicesOracle`, result/row DTOs | R7 mechanical ventilation, R8 DHW/BGV, R9 lighting/devices. Manual heating-only input maps these to zero/default inputs and R10 gates disabled categories. | `EECalcFullOracle.RunR7`, `RunR8`, `RunR9`. | Module result objects and debug rows. |
| `EE.Doklad.Tests/Validation/FullOracle/EECalcLegacyAggregation.cs` | `EECalcFuel`, `EECalcLegacyAggregation` | R10 fuel bucket mapping and duplicate Fuel1 behavior for strict legacy parity. | `EECalcFullOracle.RunR10`, R9 rows. | Fuel bucket and fuel total. |
| `EE.Doklad.Tests/Validation/EecalcQveOracle.cs` | `EecalcQveOracle`, `EecalcHeatingMonthlyBalanceRow` | Older focused Qve oracle. It is conceptually related to R5 Qve, but `EECalcFullOracle` now uses `EecalcMonthlyHeatingOracle.CalculateHve` instead. | Older R1/R2/Qve tests. | Monthly Qve balance rows. |

### Tests and helpers

| File | Classes | Role | Called by | Returns |
| --- | --- | --- | --- | --- |
| `EE.Doklad.Tests/Validation/FullOracle/EECalcFullOracleTests.cs` | `EECalcFullOracleTests` | Existing strict parity/full oracle tests. Several tests read fixture JSON and write/compare CSV; those are not part of manual flow. | xUnit. | Test pass/fail. |
| `EE.Doklad.Tests/Validation/EecalcMonthlyHeatingOracleTests.cs` | `EecalcMonthlyHeatingOracleTests` | Focused R4/R5 formula tests; includes a hand-built fixture example useful as reference. | xUnit. | Test pass/fail and debug CSV in existing test. |
| `EE.Doklad.Tests/Validation/EecalcR3HtrQtrValidationTests.cs` | `EecalcR3HtrQtrValidationTests` | Focused R3 tests; includes a minimal envelope fixture example. | xUnit. | Test pass/fail and debug CSV in existing test. |
| `EE.Doklad.Tests/Validation/EecalcMonthlyCoolingOracleTests.cs` | `EecalcMonthlyCoolingOracleTests` | Focused R6 tests. | xUnit. | Test pass/fail. |
| `EE.Doklad.Tests/Validation/EecalcR1R2ValidationTests.cs` | `EecalcR1R2ValidationTests` | Focused R1/R2 and Qve tests. | xUnit. | Test pass/fail. |
| `EE.Doklad.Tests/Validation/EecalcValidationFixtures.cs` | `EecalcValidationFixtures` | Shared older fixture factory. Not used by the new manual runner. | Older validation tests. | Fixture objects. |
| `EE.Doklad.Tests/Validation/EecalcValidationDebugWriter.cs` | `EecalcValidationDebugWriter` | CSV/debug writer for older validation. Not used by manual flow. | Older validation tests. | Files on disk. |
| `EE.Doklad.Tests/Validation/EecalcValidationReporter.cs`, `EecalcComparisonResult.cs`, `EecalcActualSnapshot.cs`, `EecalcExpectedSnapshot.cs`, `EeDokladHeatingActualAdapter.cs` | Reporter/comparison/snapshot/adapter DTOs | Snapshot/parity helpers. Not used by manual flow. | Older validation tests. | Comparison objects/reports. |

## 2. Call graph for one manual calculation

```text
Manual input
  -> ManualEecalcInput
  -> ManualFullOracleRunner.Calculate(input)
     -> CreateFixture(input)
        -> LegacyEecalcXmlClimateDataProvider
           -> DefaultParams.xml monthly avg temp
           -> DefaultParams.xml solar radiation
           -> DefaultParams.xml hourly temp/humidity
        -> EecalcValidationFixture
        -> EecalcEnvelopeFixture
     -> CreateFullOracleInput(input)
        -> EECalcFullOracleInput
        -> EECalcAggregationInput
     -> new EECalcFullOracle().Run(new EECalcOracleContext(fixture, input))
        -> R1/R2: RunR1R2
           -> EecalcMonthlyDaysOracle.Calculate
           -> LegacyEecalcXmlClimateDataProvider.GetMonthlyAvgTemp
        -> R3: RunR3
           -> EecalcHtrQtrOracle.CalculateParameterQtr
           -> CalculateParameterHtr
           -> CalculateDegreeHours
        -> R4: RunR4
           -> EecalcMonthlyHeatingOracle.CalculateMonth
           -> CalculateTransparentFsol / CalculateNonTransparentFsol
        -> R5: RunR5
           -> EecalcMonthlyHeatingOracle.Calculate
           -> CalculateHve
           -> EecalcHtrQtrOracle.CalculateParameterHtr
           -> CalculateNi
           -> monthly FinalQnd
        -> R6: RunR6
           -> EecalcMonthlyCoolingOracle.Calculate
           -> R10 later zeros cooling totals when HasCooling=false
        -> R7: RunR7
           -> EECalcVentilationOracle.Calculate
           -> zero/default input when HasMechanicalVentilation=false
        -> R8: RunR8
           -> EECalcDhwBgvOracle.Calculate
           -> zero/default input when IsBgvUsed=false
           -> R10 later zeros BGV totals when IsBgvUsed=false
        -> R9: RunR9
           -> EECalcLightingDevicesOracle.Calculate
           -> zero/default equipment when HasLighting=false and HasDevices=false
        -> R10: RunR10
           -> ApplyLegacyAggregationGates
           -> AddFinalTableValues
           -> AddPrimaryRows
           -> AddEmissionRows
           -> EECalcLegacyAggregation.CalculateTotalFuelWithDuplicateFuel1
     -> CreateManualResult(input, fullResult)
        -> aggregate R5 debug rows into ManualHeatingBreakdown
        -> read FullValues totals into ManualEecalcResult
     -> ManualEecalcResult.ToReadableText()
```

No CSV read/write is needed in this graph. `EECalcOracleResult.ExportDebugCsv()` exists but is not called.

## 3. Minimal manual input

The implemented minimal flow intentionally supports a simple heating-only project with no envelope fields. That means R3 is still executed, but Qtr can be `0` unless explicit envelope fields are added in a later extension. Qve and final heating demand still calculate from volume, infiltration, climate, schedule, temperatures, area, heat capacity and metabolic heat.

| Required field | Type | Example | Goes to model | Used by formula/module |
| --- | --- | --- | --- | --- |
| `ClimateZoneId` | `int` | `7` | `EecalcValidationFixture.ClimateZoneId` | Climate lookup in R1/R2, R6, R7, R8; monthly temperatures/radiation for R3-R5. |
| `HeatedArea` | `double` | `1000` | `EecalcValidationFixture.HeatedArea` | R5 `tau`, metabolic heat total, `FinalQnd`; R8/R10 totals. |
| `HeatedVolume` | `double` | `2500` | `EecalcValidationFixture.HeatedVolume` | R5 `Hve = volume * infiltration * 0.34`; Qve. |
| `HeatCapacity` | `double` | `46` | `EecalcValidationFixture.HeatCapacity` | R5 `tau = area * heatCapacity / (htr + hve)`, then `aH` and `Ni`. |
| `MetabolicHeat` | `double` | `3.16` | `EecalcValidationFixture.MetabolicHeat` | R5 internal metabolic gain per area and `Gamma`/`FinalQnd`. |
| `LatentMetabolicHeat` | `double` | `0.84` | `EecalcValidationFixture.LatentMetabolicHeat` | R6 latent cooling occupant term; retained even when cooling is gated off. |
| `HeatingStartDay` | `int` | `15` | `EecalcValidationFixture.FirstDay` | R1/R2 heating period day counts. |
| `HeatingStartMonth` | `int` | `10` | `EecalcValidationFixture.FirstMonth` | R1/R2 heating period month list. |
| `HeatingEndDay` | `int` | `23` | `EecalcValidationFixture.LastDay` | R1/R2 heating period day counts. |
| `HeatingEndMonth` | `int` | `4` | `EecalcValidationFixture.LastMonth` | R1/R2 heating period month list. |
| `Infiltration` | `double` | `0.5` | `EecalcValidationFixture.Infiltration` | R5 `Hve`, `Qve`, `Qht`, `Gamma`, `FinalQnd`. |
| `ProjectTemperature` | `double` | `20` | `EecalcValidationFixture.ProjectTemperature` | R3/R5 average inner heat temperature and degree-hours. |
| `NonProjectTemperature` | `double` | `16` | `EecalcValidationFixture.NonProjectTemperature` | R3/R5 average inner heat temperature and degree-hours. |
| `HasCooling` | `bool` | `false` | `EECalcAggregationInput.HasCooling` | R10 gate for cooling and cooling ventilation totals. |
| `HasMechanicalVentilation` | `bool` | `false` | `EECalcVentilationInput` default/zero mapping | Keeps R7 input zero for heating-only flow. |
| `IsBgvUsed` | `bool` | `false` | `EECalcAggregationInput.IsBgvUsed` | R10 gate for DHW/BGV and BGV pump totals. |
| `HasLighting` | `bool` | `false` | `EECalcLightingDevicesInput` default/zero mapping | Keeps R9 lighting input zero for heating-only flow. |
| `HasDevices` | `bool` | `false` | `EECalcLightingDevicesInput` default/zero mapping | Keeps R9 device input zero for heating-only flow. |

Default assumptions in the manual runner:

- Workday, Saturday and Sunday schedules are `0..24`, so all active days use project temperature.
- Holidays are `0` for all months.
- Envelope geometry/U-values are zero by default.
- Mechanical ventilation, DHW/BGV, lighting, devices, fans/pumps and other loads are zero/default unless the runner is extended.
- Climate comes from `LegacyEecalcXmlClimateDataProvider(ClimateProviderMode.LegacyEECalcStrict)`.

## 4. Implemented/proposed ManualFullOracleRunner

The new API supports the intended code shape:

```csharp
var input = new ManualEecalcInput
{
    ClimateZoneId = 7,
    HeatedArea = 1000,
    HeatedVolume = 2500,
    HeatCapacity = 46,
    MetabolicHeat = 3.16,
    LatentMetabolicHeat = 0.84,
    HeatingStartDay = 15,
    HeatingStartMonth = 10,
    HeatingEndDay = 23,
    HeatingEndMonth = 4,
    Infiltration = 0.5,
    ProjectTemperature = 20,
    NonProjectTemperature = 16,
    HasCooling = false,
    HasMechanicalVentilation = false,
    IsBgvUsed = false,
    HasLighting = false,
    HasDevices = false
};

var result = ManualFullOracleRunner.Calculate(input);

Console.WriteLine(result.ToReadableText());
```

Current implementation details:

- `ManualFullOracleRunner.Calculate` is the only entry point.
- It does not read `fixture_001_input.json`.
- It does not read `fixture_001_expected.json`.
- It does not read or write `parity_mismatch_report.csv`.
- It calls `EECalcFullOracle.Run` directly and only consumes in-memory `DebugRows` and `FullValues`.
- `ManualEecalcResult` exposes object properties and `ToReadableText()`.

Potential next extension:

- Add optional manual envelope fields such as wall area/U, roof area/U, floor area/U and window area/U if Qtr must be non-zero for realistic transmission losses.

## 5. CSV/parity parts not involved

The following files remain part of the existing parity harness, but are not used by the manual flow:

| File | Why not involved |
| --- | --- |
| `EE.Doklad.Tests/Validation/FullOracle/CsvParityPipeline.cs` | Exports and compares CSV-shaped rows. Manual flow does not export or compare CSV. |
| `EE.Doklad.Tests/Validation/FullOracle/EECalcDebugCsv.cs` | Writes debug CSV. Manual flow does not call `ExportDebugCsv()`. |
| `EE.Doklad.Tests/Validation/FullOracle/RealEECalcInputSnapshotImporter.cs` | Reads `fixture_001_input.json`. Manual flow builds models from `ManualEecalcInput`. |
| `EE.Doklad.Tests/Validation/FullOracle/RealEECalcTableSnapshotImporter.cs` | Reads expected table snapshot JSON. Manual flow has no expected/snapshot comparison. |
| `EE.Doklad.Tests/Validation/FullOracle/Fixtures/fixture_001_expected.json` | Expected snapshot fixture; not read. |
| `test-data/full_oracle/fixture_001_input.json` | Input snapshot fixture; not read. |
| `test-results/validation/full_oracle_fixture001/parity_mismatch_report.csv` | Existing parity output; not read or written. |
| `test-results/validation/full_oracle_fixture001/*.csv` | Existing debug exports; not read or written. |

## 6. How to run one manual calculation from Visual Studio

Option A: run the unit test.

1. Open `EE Doklad.sln`.
2. Open Test Explorer.
3. Run `ManualFullOracleRunner_HeatingOnly_ReturnsReadableResult`.
4. Put a breakpoint after `var result = ManualFullOracleRunner.Calculate(input);`.
5. Inspect `result` or evaluate `result.ToReadableText()` in the Watch/Immediate window.

Option B: create a temporary console/test snippet in the test project.

```csharp
using EE.Doklad.Tests.Validation.FullOracle;

var input = new ManualEecalcInput
{
    ClimateZoneId = 7,
    HeatedArea = 1000,
    HeatedVolume = 2500,
    HeatCapacity = 46,
    MetabolicHeat = 3.16,
    LatentMetabolicHeat = 0.84,
    HeatingStartDay = 15,
    HeatingStartMonth = 10,
    HeatingEndDay = 23,
    HeatingEndMonth = 4,
    Infiltration = 0.5,
    ProjectTemperature = 20,
    NonProjectTemperature = 16,
    HasCooling = false,
    HasMechanicalVentilation = false,
    IsBgvUsed = false,
    HasLighting = false,
    HasDevices = false
};

var result = ManualFullOracleRunner.Calculate(input);
Console.WriteLine(result.ToReadableText());
```

Command-line test run:

```powershell
dotnet test EE.Doklad.Tests\EE.Doklad.Tests.csproj --filter ManualFullOracleRunner_HeatingOnly_ReturnsReadableResult --no-restore
```

Verified on this audit pass: the targeted test passed with 1 test executed.
