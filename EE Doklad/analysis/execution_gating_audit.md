# EECalc Execution Gating Audit

## 1. Summary

The decompiled EECalc calculation methods for heating, cooling, ventilation, DHW/BGV, and lighting/devices generally do not contain local technology-use guards. The evidence available in `HeatingAndCoolingResultCalc.cs` points to this legacy pattern:

1. Calculation routines execute when their caller invokes them.
2. Building/zone final tables filter included values later using `HasHeating`, `HasCooling`, `isBGVused`, and first-zone BGV rules.
3. Savings routines do contain early `HasHeating` / `HasCooling` guards, but those are savings paths, not the main energy calculation paths.

For strict parity, `EECalcFullOracle` should keep running R1-R9 and apply technology inclusion rules in R10. This audit found that the current oracle already ran R6/R7/R8/R9 unconditionally, but R10 incorrectly included heating/cooling categories without `HasHeating` / `HasCooling` filtering. The minimum parity-oriented fix is aggregation-only gating in R10.

## 2. Module Execution Matrix

| Module | Legacy execution behavior | Guard condition | Evidence method | Oracle behavior | Action |
| --- | --- | --- | --- | --- | --- |
| R1/R2 Calendar/DegreeHours | AlwaysCalculated | None found in monthly period helpers; consumers call `section.CalcPeriod(...)`. | `CoolingCalculations` at `HeatingAndCoolingResultCalc.cs:123`; heating `Calculations` at `:3243`; ventilation heat at `:15710`. | Always runs in `EECalcFullOracle`. | Keep. |
| R3 Transmission | CalculatedButFilteredLater | Heating/cooling final inclusion is gated later by `HasHeating` / `HasCooling`; no local guard found in heating/cooling calculation bodies. | Heating `Calculations` at `:3243`; cooling `CoolingCalculations` at `:123`; aggregation gates at `UpdateActualState` `:8781`. | Runs as part of R3. | Keep calculation; filter final heating/cooling categories in R10. |
| R4 Heating Gains | CalculatedButFilteredLater | No pre-calculation `HasHeating` guard found inside `Calculations`; building final rows gate heating categories. | `Calculations` at `:3243`; `CalculateNetEnergyByTechnologiesBuilding` at `:9715`. | Runs. | Keep calculation; filter final heating categories in R10. |
| R5 Heating Balance | CalculatedButFilteredLater | Main heating `Calculations` body has no early `if (!zone.HasHeating) return`; final rows gate `HasHeating`. | `Calculations` at `:3243`; `UpdateActualState` at `:8781`; `CalculatePrimaryEnergyByTechnologies` at `:7686`. | Runs. | Keep calculation; filter final heating and heating ventilation categories in R10. |
| R6 Cooling | CalculatedButFilteredLater | `CoolingCalculations` has no `HasCooling` guard; savings path has a guard, but final tables gate `HasCooling`. | `CoolingCalculations` at `:123`; savings guard at `CalculateCoolingSavings` `:11456`; final gates at `UpdateActualState` `:8781`. | Runs unconditionally. | Keep calculation; filter final cooling categories in R10. |
| R7 Ventilation | CalculatedButFilteredLater | Ventilation heat/cool methods have no local `HasHeating` / `HasCooling` guard; final aggregation gates heating ventilation by `HasHeating` and cooling ventilation by `HasCooling`. | `VentilationHeatEnergyActual` at `:15710`; `VentilationCoolEnergyActual` at `:14870`; final gates at `UpdateActualState` `:8781`. | Runs unconditionally. | Keep calculation; filter `HeatingVentilation` and `CoolingVentilation` in R10. |
| R8 DHW/BGV | CalculatedButFilteredLater | Ordinary DHW methods have no local BGV-use guard; final aggregation uses `isBGVused` and first-zone values. | `HotWaterCalculationActual` at `:4778`; `BuildingCalculations` calls `CalculateTotalsNeededEnergyTable(..., isBGVused: true)` at `:8524`; zone uses `isBGVused: false` at `:8567`. | Runs unconditionally. | Keep calculation; add/use `IsBgvUsed` R10 gate. |
| R9 Lighting/Devices | AlwaysCalculated | Period wrappers have no `HasLighting` / `HasDevices` guards; final aggregation sums all zones. | `CalculatePeriodsActual` at `:4978`; balanced `:5009`; non-balanced `:5040`; primary direct loads after `CalculatePrimaryEnergyByTechnologies` `:7686`. | Runs unconditionally. | Keep; no heating/cooling final filter for general device categories. |

## 3. Aggregation Inclusion Matrix

| Category | Legacy inclusion rule | Evidence method | Oracle current rule | Action |
| --- | --- | --- | --- | --- |
| Heating | Sum zones where `HasHeating`. | `UpdateActualState` `:8781`; `CalculateNetEnergyByTechnologiesBuilding` `:9715`; `CalculatePrimaryEnergyByTechnologies` `:7686`. | Now gated by `Aggregation.HasHeating`. | Fixed. |
| HeatingVentilation | Sum zones where `HasHeating`. | `UpdateActualState` `:8781`; `CalculateNetEnergyByTechnologiesBuilding` `:9715`; primary at `:7686`. | Now gated by `Aggregation.HasHeating`. | Fixed. |
| Cooling | Sum zones where `HasCooling`. | `UpdateActualState` `:8781`; `CalculateNetEnergyByTechnologiesBuilding` `:9715`; primary at `:7686`. | Now gated by `Aggregation.HasCooling`. | Fixed. |
| CoolingVentilation | Sum zones where `HasCooling`. | `UpdateActualState` `:8781`; `CalculateNetEnergyByTechnologiesBuilding` `:9715`; primary at `:7686`. | Now gated by `Aggregation.HasCooling`. | Fixed. |
| BGV | First-zone/building-level value when `isBGVused` is true. | `UpdateActualState` `:8781`; `CalculatePrimaryEnergyByTechnologies` `:7686`; `BuildingCalculations` `:8524`. | Now gated by `Aggregation.IsBgvUsed`; still single oracle fixture/zone. | Fixed for current oracle shape. |
| BGVPumps | First-zone normal pump plus first-zone solar `BGVPumpsTotal`; building post-loop adds solar pump fuel hooks. | `UpdateActualState` `:8781`; `BuildingCalculations` `:8524`. | Now gated by `Aggregation.IsBgvUsed`; single-zone source. | Fixed for current oracle shape. |
| FansAndPumps | Sum all zones. | `UpdateActualState` `:8781`; primary direct load path in `CalculatePrimaryEnergyByTechnologies` `:7686`. | Always included. | Keep. |
| Lights | Sum all zones. | `UpdateActualState` `:8781`; direct electrical primary path at `:7686`. | Always included. | Keep. |
| HeatAffectingDevices | Sum all zones. | `UpdateActualState` `:8781`; direct electrical primary path at `:7686`. | Always included. | Keep. |
| NonHeatAffectingDevices | Sum all zones. | `UpdateActualState` `:8781`; direct electrical primary path at `:7686`. | Always included. | Keep. |
| Other | Sum all zones. | `UpdateActualState` `:8781`; direct electrical primary path at `:7686`. | Always included. | Keep. |

## 4. Cooling-Specific Findings

| Question | Finding | Evidence | Impact |
| --- | --- | --- | --- |
| Does legacy EECalc calculate R6 cooling even if `HasCooling=false`? | The cooling method itself has no local `HasCooling` guard. Whether it is invoked by the UI orchestration cannot be proven from `Calculator.Calculate`, which is empty in the decompiled output, but the calculation routine is not guarded. | `CoolingCalculations` at `HeatingAndCoolingResultCalc.cs:123` directly builds months and calls all variants. | Treat as `CalculatedButFilteredLater` for oracle parity. |
| Where is cooling filtered out? | Building needed/net/primary/fuel/CO2 tables use `Where(z => z.HasCooling)` or `if (zone.HasCooling)`. | `UpdateActualState` at `:8781`; `CalculateNetEnergyByTechnologiesBuilding` at `:9715`; `CalculatePrimaryEnergyByTechnologies` at `:7686`; `GetFuelTypeAndValues` at `:9041`. | R10 must zero final Cooling when `HasCooling=false`, while keeping R6 debug rows. |
| Are savings also filtered? | Savings cooling path has an early `if (!buildingZone.HasCooling)` guard. | `CalculateCoolingSavings` at `:11456`. | This does not justify skipping R6 in strict oracle; it only applies to savings. |

## 5. Ventilation-Specific Findings

| Question | Finding | Evidence | Impact |
| --- | --- | --- | --- |
| Does EECalc compute ventilation cooling without a local cooling-system guard? | The ventilation cooling method has no local `HasCooling` guard in the method body. | `VentilationCoolEnergyActual` at `HeatingAndCoolingResultCalc.cs:14870`. | Keep R7 cooling calculation available in oracle debug. |
| Is cooling ventilation included in final tables only when `HasCooling=true`? | Yes. Needed, net, primary, fuel, and CO2 paths gate cooling ventilation with `HasCooling`. | `UpdateActualState` at `:8781`; `CalculateNetEnergyByTechnologiesBuilding` at `:9715`; `CalculatePrimaryEnergyByTechnologies` at `:7686`; `GetFuelTypeAndValues` at `:9041`. | R10 must zero `CoolingVentilation` final fields when `HasCooling=false`. |
| Is heating ventilation included only when `HasHeating=true`? | Yes. The same aggregation methods gate heating ventilation with `HasHeating`. | Same methods as above. | R10 must zero `HeatingVentilation` final fields when `HasHeating=false`. |
| Do savings paths guard ventilation? | Yes, ventilation savings has early `HasHeating` / `HasCooling` guards. | `CalculateVentilationHeatingSavings` `:11596`; `CalculateVentilationCoolingSavings` `:11694`. | Savings guards are separate from main calculation behavior. |

## 6. Oracle Mismatch Findings

Before this audit fix, current `EECalcFullOracle` behavior was:

- R6 ran unconditionally: aligned with calculated-but-filtered-later interpretation.
- R7 cooling ran unconditionally: aligned with calculated-but-filtered-later interpretation.
- R10 included Cooling even when a fixture should represent `HasCooling=false`: mismatch.
- R10 included CoolingVentilation even when `HasCooling=false`: mismatch.
- R10 included Heating and HeatingVentilation even when `HasHeating=false`: mismatch.
- R10 did not expose an explicit BGV-use switch: incomplete for `isBGVused` parity.

The implemented minimum fix adds `EECalcAggregationInput.HasHeating`, `HasCooling`, and `IsBgvUsed`, defaulting to `true`, and applies them only in R10 via `ApplyLegacyAggregationGates`.

## 7. Required Oracle Changes

Implemented:

- Add aggregation flags in `EECalcFullOracleModels.cs`:
  - `HasHeating`
  - `HasCooling`
  - `IsBgvUsed`
- Keep R1-R9 module execution unchanged.
- Add R10-only filtering:
  - `!HasHeating` zeroes `Heating` and `HeatingVentilation`.
  - `!HasCooling` zeroes `Cooling` and `CoolingVentilation`.
  - `!IsBgvUsed` zeroes `BGV` and `BGVPumps`.
- Preserve `KD-A001` and `KD-A009`.

Still future work:

- Multi-zone oracle inputs should eventually distinguish per-zone `HasHeating` / `HasCooling`, because legacy aggregation filters per zone. Current `EECalcFullOracle` is still a single-fixture/single-zone oracle harness.
- If a future recovered top-level UI caller proves that modules are skipped before calculation, reclassify that specific module. No such pre-module guard was found in the inspected decompiled methods.

## 8. Tests To Add

Implemented tests:

- `FullOracle_HasCoolingFalse_KeepsR6DebugButFiltersCoolingFinalTables`
  - Confirms R6 still emits debug rows.
  - Confirms raw `CoolingNetEnergy` can remain nonzero.
  - Confirms final `NeededEnergyTable.Cooling.Actual`, `NeededEnergyTable.CoolingVentilation.Actual`, and `PrimaryEnergyTable.Cooling.Actual` are zero.
- `FullOracle_HasHeatingFalse_FiltersHeatingAndHeatingVentilationFinalTables`
  - Confirms R5 still emits debug rows.
  - Confirms raw `HeatingFinalQndPerArea` can remain nonzero.
  - Confirms final `NeededEnergyTable.Heating.Actual`, `NeededEnergyTable.HeatingVentilation.Actual`, and `PrimaryEnergyTable.Heating.Actual` are zero.

Recommended future tests:

- `IsBgvUsed=false` filters `BGV` and `BGVPumps` final rows while preserving R8 debug output.
- Multi-zone fixture: zone 1 heated only, zone 2 cooled only, direct devices in both zones.
- Multi-zone BGV fixture: BGV first-zone value is used while lights/devices sum all zones.
