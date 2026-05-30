# R9 Lighting / Devices Reverse Engineering

## 1. Summary

R9 covers EECalc lighting and device calculations in `HeatingAndCoolingResultCalc.cs`.

Lighting and devices have two separate roles:

- Thermal input role: `Lights` and `BalancedDevices` are heat-affecting gains. They reduce heating net demand through `CalculateLightsAndDevicesInputs` and increase cooling internal gains through `CalculateQint*`.
- Annual/device energy role: `Lights`, `BalancedDevices`, and `NonBalancedDevices` all calculate `DevicesNeededEnergy*` for `Heating`, `Cooling`, and `General` periods.

`NonBalancedDevices` do not enter heating/cooling thermal balance. They are only handled as non-heat-affecting electrical/device energy in period, savings, primary energy, CO2, and building aggregation tables.

No direct climate data dependency was found. The only calendar dependency is through `Section.CalcPeriod(...)`, which returns `MonthlyDays` and `Weeks`. Climate providers should not be rewired for this phase.

## 2. Execution flow

Heating thermal inputs:

1. Heating `Calculations(...)` iterates heating months from `section.CalcPeriod(...)`.
2. Monthly `ParameterNi`/eta values are calculated by the heating engine.
3. `CalculateLightsAndDevicesInputs(lightsAndDevicesCalculationData, month, etaRef1, etaRef2, etaActual, etaBaseLine, etaESM)` is called.
4. The method appends monthly light/device useful gains to static lists.
5. `GetLightsAndDevicesInputs(calcData)` sums the static lists and writes `ResulLightInputs*` and `ResulAppliancesInputs*`.
6. Heating net energy subtracts ventilation, light, and appliance inputs:
   `ResulNetEnergy = ResulNoInputsNetEnergy - (ResulVentilationInputs + ResulLightInputs + ResulAppliancesInputs)`.

Cooling thermal inputs:

1. Cooling `CalculateQint*` methods calculate internal gains.
2. `Lights` and `BalancedDevices` are added.
3. Cooling multiplies the sum by heated area.
4. `NonBalancedDevices` are not included.

Period device energy:

1. `CalculatePeriodsReference*`, `CalculatePeriodsActual*`, `CalculatePeriodsBaseLine*`, and `CalculatePeriodsESM*` wrappers call heating, cooling, and annual period calculators.
2. Period calculators use `section.CalcPeriod(...)` to get total `Weeks`.
3. Non-monthly mode uses stored period `Power*` and `WorkSchedule*`.
4. Monthly mode calculates a weighted monthly power and schedule from month schedules.

Savings:

1. `CalculateLightsSavings`, `CalculateBalancedDevicesSavings`, and `CalculateNonBalancedDevicesSavings` call the shared `CalculatePeriod`.
2. The shared savings code compares only `BaseLine` and `ESM`.
3. Only `General` period savings are added to the zone savings list.

## 3. Full call graph

```text
Heating Calculations(...)
  -> section.CalcPeriod(heating season)
  -> monthly heating pipeline
  -> CalculateLightsAndDevicesInputs(...)
       -> CalcAvgMonthPower(...) when ByMonths
            -> CalcWeekPower(...)
       -> static LigthsList* / DevicesList*
  -> GetLightsAndDevicesInputs(...)
  -> CalculateResultNetEnergy(...)

CoolingCalculations(...)
  -> CalculateQgain*
       -> CalculateQint*
            -> CalcAvgMonthPower(...) when ByMonths
                 -> CalcWeekPower(...)
            -> Lights + BalancedDevices only

CalculatePeriodsReference(...)
  -> CalculateHeatingPeriodRef1/Ref2
  -> CalculateCoolingPeriodRef1/Ref2
  -> CalculateAnnualPeriodRef1/Ref2

CalculatePeriodsReferenceBalanced(...)
  -> CalculateHeatingPeriodRef1/Ref2Balanced
  -> CalculateCoolingPeriodRef1/Ref2Balanced
  -> CalculateAnnualPeriodRef1/Ref2Balanced

CalculatePeriodsReferenceNonBalanced(...)
  -> CalculateHeatingPeriodRef1/Ref2NonBalanced
  -> CalculateCoolingPeriodRef1/Ref2NonBalanced
  -> CalculateAnnualPeriodRef1/Ref2NonBalanced

Actual/BaseLine/ESM period wrappers
  -> Annual period
  -> Heating period
  -> Cooling period
  -> CalcAvgMonthPower(...) when ByMonths
       -> CalcWeekPower(...)

CalculateLightsSavings(...)
CalculateBalancedDevicesSavings(...)
CalculateNonBalancedDevicesSavings(...)
  -> CalculatePeriod(...)
       -> CheckLightsAndDevicesSavings(...)
       -> SetLightsAndDevicesSavingsvalues(...)
       -> AddSavingsToZone(...) for General only

Building/zone aggregation
  -> NeededEnergyTable.Lights / HeatAffectingDevices / NonHeatAffectingDevices
  -> PrimaryEnergyTable.*
  -> PrimaryEnergyFuelTable.Fuel8 through Fuel.Fuel1 mapping
  -> EmissionNeededEnergyTable.*
  -> EmissionEnergySupplyTable.Fuel8 through Fuel.Fuel1 mapping
```

## 4. Formula catalog

| Area | Formula |
|---|---|
| Non-monthly period energy | `DevicesNeededEnergy = WorkSchedule * Power * Sum(Weeks) / 1000` |
| Monthly week regime | `weekRegime = WorkDays * 5 + Saturdays + Sundays` |
| Monthly average power | `(WorkDays * WorkDaysUsedEnergy * 5 + Saturdays * SaturdaysUsedEnergy + Sundays * SundaysUsedEnergy) / weekRegime` |
| Monthly period weighted power | `AvgPower = Sum(MonthPower_i * weekRegime_i * Weeks_i) / Sum(weekRegime_i * Weeks_i)` |
| Monthly period schedule | `WorkSchedule = Sum(weekRegime_i * Weeks_i) / Sum(Weeks_i)` |
| Monthly period energy | `DevicesNeededEnergy = AvgPower * Sum(weekRegime_i * Weeks_i) / 1000` |
| Heating light input | `LightInputMonth = LightEnergyMonth * ParameterEta` |
| Heating balanced device input | `DeviceInputMonth = DeviceEnergyMonth * ParameterEta` |
| Cooling Qint Ref1/Ref2 | `(Lights.Cooling.PowerRef* * Lights.Cooling.WorkScheduleRef* * Weeks / 1000 + BalancedDevices.Cooling.PowerRef* * BalancedDevices.Cooling.WorkScheduleRef* * Weeks / 1000) * Area` |
| Cooling Qint Actual/BaseLine/ESM non-monthly | `(Lights.Cooling.Power* * Lights.Cooling.WorkSchedule* * Weeks / 1000 + BalancedDevices.Cooling.Power* * BalancedDevices.Cooling.WorkSchedule* * Weeks / 1000) * Area` |
| Cooling Qint Actual/BaseLine/ESM monthly | `(CalcAvgMonthPower(Lights.*) * weekRegime * Weeks / 1000 + CalcAvgMonthPower(BalancedDevices.*) * weekRegime * Weeks / 1000) * Area` |
| Device savings string | `(DevicesNeededEnergyBaseLine - DevicesNeededEnergyESM).ToString("F3")` |
| Savings by power | `WS_BaseLine * Power_BaseLine * Weeks / 1000 - WS_BaseLine * Power_ESM * Weeks / 1000` |
| Savings by schedule | `WS_BaseLine * Power_BaseLine * Weeks / 1000 - WS_ESM * Power_BaseLine * Weeks / 1000` |

## 5. Heating lighting inputs

`CalculateLightsAndDevicesInputs` handles heating useful gains for `Lights` and `BalancedDevices`.

For `Lights.ByMonths == true`:

- Ref1/Ref2 use heating period scalar fields: `Lights.Heating.PowerRef*` and `Lights.Heating.WorkScheduleRef*`.
- Actual/BaseLine/ESM use monthly schedules: `Lights.Actual`, `Lights.BaseLine`, `Lights.Esm`.
- Each monthly energy is multiplied by the current heating eta/`ParameterNi` value before being stored.

For `Lights.ByMonths == false`:

- All variants use `Lights.Heating.Power*` and `Lights.Heating.WorkSchedule*`.
- Each monthly energy is multiplied by eta.

Reference values are added only when `currentZone.HasRefenceValues` is true.

Outputs:

- `ResulLightInputsRef1`
- `ResulLightInputsref2` spelling preserved from EECalc
- `ResulLightInputsActual`
- `ResulLightInputsBaseLine`
- `ResulLightInputsESM`

## 6. Cooling lighting inputs

Cooling lighting inputs are inside `CalculateQintRef1`, `CalculateQintRef2`, `CalculateQint`, `CalculateQintBaseLine`, and `CalculateQintESM`.

Reference variants ignore monthly lighting schedules and use:

```text
Lights.Cooling.PowerRef* * (Lights.Cooling.WorkScheduleRef* * month.Weeks) / 1000
```

Actual/BaseLine/ESM use monthly schedule data only when `Lights.ByMonths` is true. Otherwise they use the cooling period scalar fields.

Cooling Qint includes lighting and balanced devices only. It is multiplied by `section.Area.HeatedArea`.

## 7. Annual/general lighting

Annual/general lighting energy is calculated by `CalculateAnnualPeriod*`.

The annual period is hardcoded as:

```text
section.CalcPeriod(0, 11, 1, 31)
```

Ref1/Ref2 always use scalar `Lights.General.PowerRef*` and `Lights.General.WorkScheduleRef*`.

Actual/BaseLine/ESM use monthly schedules if `Lights.ByMonths` is true. In monthly mode, EECalc computes weighted average power and aggregate work schedule, writes `Lights.General.DevicesNeededEnergy*`, and may overwrite `Lights.General.Power*` and `Lights.General.WorkSchedule*` with the derived values.

## 8. Balanced devices

`BalancedDevices` are heat-affecting appliances.

They mirror lighting in all major paths:

- Heating inputs are included in `CalculateLightsAndDevicesInputs`.
- Cooling internal gains are included in `CalculateQint*`.
- Heating, cooling, and annual `DevicesNeededEnergy*` are calculated by `CalculatePeriods*Balanced`.
- General-period savings are added through `CalculateBalancedDevicesSavings`.
- Aggregation maps them to `HeatAffectingDevices`.

The formulas match lighting with `BalancedDevices` substituted for `Lights`.

## 9. Non-balanced devices

`NonBalancedDevices` are non-heat-affecting appliances.

They are not used in:

- `CalculateLightsAndDevicesInputs`
- `CalculateQint*`
- heating net-energy input subtraction
- cooling internal gains

They are used in:

- `CalculatePeriods*NonBalanced`
- `CalculateNonBalancedDevicesSavings`
- `NeededEnergyTable.NonHeatAffectingDevices`
- `PrimaryEnergyTable.NonHeatAffectingDevices`
- `EmissionNeededEnergyTable.NonHeatAffectingDevices`
- primary/emission fuel tables through the electricity mapping

In monthly Actual mode, `NonBalancedDevices` calculate `DevicesNeededEnergyActual` but do not overwrite period `PowerActual` or `WorkScheduleActual`. BaseLine and ESM monthly modes do overwrite derived period fields.

## 10. Monthly schedule logic

Monthly schedules use `CalcAvgMonthPower(schedule, month)`.

The method selects one of:

- `schedule.January`
- `schedule.February`
- `schedule.March`
- `schedule.April`
- `schedule.May`
- `schedule.June`
- `schedule.July`
- `schedule.August`
- `schedule.September`
- `schedule.October`
- `schedule.November`
- `schedule.December`

It then calls `CalcWeekPower(monthState)`.

`CalcWeekPower` writes the static field `weekRegime`, then returns average weekly power. If the average is NaN or Infinity, it returns `0`.

The monthly period calculators rely on the side effect that `weekRegime` contains the value from the immediately preceding `CalcAvgMonthPower` call.

## 11. WorkDays/Saturdays/Sundays/Holidays logic

Lighting and devices do not directly inspect holiday calendars.

Inputs used by `CalcWeekPower`:

- `WorkDays`
- `Saturdays`
- `Sundays`
- `WorkDaysUsedEnergy`
- `SaturdaysUsedEnergy`
- `SundaysUsedEnergy`

`WorkDays` are multiplied by `5`, while Saturdays and Sundays are not:

```text
weekRegime = WorkDays * 5 + Saturdays + Sundays
```

`MonthlyDays.Weeks` comes from `Section.CalcPeriod(...)`, so partial months and year/month day counts affect the result through `Weeks`. Holiday behavior, if any, is upstream in period/month construction and not in lighting/device formulas.

## 12. Needed energy / source energy conversion

The period methods calculate needed device energy in kWh/m2-style per-area values by dividing by `1000`.

No generator, distribution, transmission, or source efficiency conversion is applied inside the lighting/device period methods. Lighting/devices are treated as direct electrical demand for aggregation purposes.

Primary energy conversion uses `Fuel.Fuel1` for lights/devices and `GetPrimaryEnergyCoeficient(Fuel.Fuel1, quantity)`, which applies factor `3.0`.

The primary fuel table also passes `Fuel.Fuel1`, but the helper stores it under `PrimaryEnergyFuelTable.Fuel8` and applies factor `3.0`. This means EECalc's `Fuel.Fuel1` enum acts as the electricity input for these direct electrical loads, while the reporting bucket is `Fuel8`.

## 13. Savings logic

Savings methods:

- `CalculateLightsSavings`
- `CalculateBalancedDevicesSavings`
- `CalculateNonBalancedDevicesSavings`

Each method:

1. Sets `publicCalculationData`.
2. Calls shared `CalculatePeriod` for `Heating`, `Cooling`, and `General`.
3. Writes `DevicesNeededEnergySavings` strings for all three periods as `BaseLine - ESM` formatted with `"F3"`.

`CalculatePeriod` checks only:

- `WorkScheduleBaseLine != WorkScheduleESM`
- `PowerBaseLine != PowerESM`

When a difference exists, it calculates virtual savings for schedule and/or power. It then distributes the combined baseline-to-ESM energy delta by each item part. Positive and negative savings are adjusted by `CheckAndCalculateNegativeSavings` when mixed signs occur.

Only `General` period savings are added to the zone savings list. Heating and cooling period savings strings are still written but are not added to the zone savings collection in the shared method.

## 14. Building aggregation hooks

Zone and building aggregation use the `General.DevicesNeededEnergy*` values:

- `Lights.General` -> `NeededEnergyTable.Lights`
- `BalancedDevices.General` -> `NeededEnergyTable.HeatAffectingDevices`
- `NonBalancedDevices.General` -> `NeededEnergyTable.NonHeatAffectingDevices`

Building tables sum the corresponding zone result rows and calculate area-normalized values.

Primary energy aggregation uses the same three categories:

- `PrimaryEnergyTable.Lights`
- `PrimaryEnergyTable.HeatAffectingDevices`
- `PrimaryEnergyTable.NonHeatAffectingDevices`

CO2 aggregation uses:

- `EmissionNeededEnergyTable.Lights`
- `EmissionNeededEnergyTable.HeatAffectingDevices`
- `EmissionNeededEnergyTable.NonHeatAffectingDevices`
- `EmissionEnergySupplyTable` through `GetFuelTypeCo2*`

Heating net-energy aggregation does not include these annual/general device values directly. The thermal effect of lights and balanced devices is already included through `ResulLightInputs*` and `ResulAppliancesInputs*`.

## 15. Fuel/electricity mapping

Lights, balanced devices, and non-balanced devices are passed as `Fuel.Fuel1` in primary and emission helper calls:

```text
GetPrimaryFuelType*(..., Fuel.Fuel1, DevicesNeededEnergy*, heatedArea)
GetFuelTypeCo2*(..., Fuel.Fuel1, DevicesNeededEnergy*, heatedArea)
GetEkoCoeficient(Fuel.Fuel1, DevicesNeededEnergy*)
```

The helper mappings are not identity mappings:

- `GetPrimaryFuelType*` maps `Fuel.Fuel1` into `PrimaryEnergyFuelTable.Fuel8` with factor `3.0`.
- `GetFuelTypeCo2*` maps `Fuel.Fuel1` into `EmissionEnergySupplyTable.Fuel8`.
- `GetPrimaryEnergyCoeficient(Fuel.Fuel1, quantity)` returns `quantity * 3.0`.
- `GetEkoCoeficient(Fuel.Fuel1, quantity)` returns `quantity * 819.0`.

Therefore the EECalc-compatible interpretation is:

- formula code passes `Fuel.Fuel1`;
- reporting tables place this direct electrical load in `Fuel8`;
- electricity primary factor is `3.0`;
- electricity CO2 factor in these needed/source paths is `819.0`.

This is spelling/behavior-preserving documentation, not a recommendation to rename enum values.

## 16. EECalc quirks / KD candidates

KD-LD001: `LigthsList*` spelling is preserved in the decompiled code.

KD-LD002: Ref2 lighting input property is spelled `ResulLightInputsref2`, while other Ref2 properties use `Ref2`.

KD-LD003: Monthly schedule logic depends on the static mutable `weekRegime` side effect from `CalcWeekPower`.

KD-LD004: `CalcWeekPower` multiplies `WorkDays` by `5` but not Saturdays or Sundays. This may be intentional schedule semantics, but the oracle must preserve it.

KD-LD005: Ref1/Ref2 lighting and balanced-device thermal inputs ignore `ByMonths` monthly schedules and use reference scalar period fields even when `ByMonths` is true.

KD-LD006: `NonBalancedDevices` are excluded from thermal gains but included in annual/general energy and savings.

KD-LD007: Some monthly period field updates guard `Power*` assignment with `Math.Abs(num5) > 0.01` instead of `Math.Abs(num4) > 0.01`, especially ESM/general variants. Preserve variant-specific guards.

KD-LD008: Actual monthly `NonBalancedDevices` compute energy but do not update derived `PowerActual` or `WorkScheduleActual`; BaseLine/ESM do update derived fields.

KD-LD009: Shared savings distribution divides by total savings without an explicit zero guard before assigning `Part`.

KD-LD010: Only `General` period savings are added to zone savings, although heating/cooling savings strings are calculated.

KD-LD011: Fuel/electricity mapping passes `Fuel.Fuel1` but stores/report electricity under `Fuel8` in primary/emission fuel tables.

## 17. Required inputs

Common:

- `Section.HeatingSeason.FirstMonthHeat`
- `Section.HeatingSeason.LastMonthHeat`
- `Section.HeatingSeason.FirstDayHeat`
- `Section.HeatingSeason.LastDayHeat`
- `Section.CoolingSeason.FirstMonthCool`
- `Section.CoolingSeason.LastMonthCool`
- `Section.CoolingSeason.FirstDayCool`
- `Section.CoolingSeason.LastDayCool`
- `Section.Area.HeatedArea`
- `currentZone.HasRefenceValues`
- `MonthlyDays.Month`
- `MonthlyDays.Weeks`

For each of `Lights`, `BalancedDevices`, `NonBalancedDevices`:

- `ByMonths`
- `Heating.PowerRef1/Ref2/Actual/BaseLine/ESM`
- `Heating.WorkScheduleRef1/Ref2/Actual/BaseLine/ESM`
- `Cooling.PowerRef1/Ref2/Actual/BaseLine/ESM`
- `Cooling.WorkScheduleRef1/Ref2/Actual/BaseLine/ESM`
- `General.PowerRef1/Ref2/Actual/BaseLine/ESM`
- `General.WorkScheduleRef1/Ref2/Actual/BaseLine/ESM`
- Monthly schedules: `Actual`, `BaseLine`, `Esm`
- For each month schedule: `WorkDays`, `Saturdays`, `Sundays`, `WorkDaysUsedEnergy`, `SaturdaysUsedEnergy`, `SundaysUsedEnergy`

Heating inputs also require monthly eta values already calculated by R5:

- `parameterEtaRef1`
- `parameterEtaRef2`
- `parameterEta`
- `parameterEtaBaseLine`
- `parameterEtaESM`

## 18. Proposed oracle design

Create a documentation-first R9 oracle later with the following split:

- `LightingDevicesPeriodOracle`: computes heating, cooling, and annual `DevicesNeededEnergy*` for one equipment group.
- `LightingDevicesMonthlyScheduleOracle`: implements `CalcAvgMonthPower`, `CalcWeekPower`, and the `weekRegime` side effect explicitly.
- `LightingDevicesHeatingInputOracle`: computes `ResulLightInputs*` and `ResulAppliancesInputs*` from monthly heating eta values.
- `LightingDevicesCoolingQintOracle`: computes `Qint*` for cooling with lights and balanced devices only.
- `LightingDevicesSavingsOracle`: reproduces `CalculatePeriod` and the general-only zone savings behavior.
- `LightingDevicesAggregationOracle`: maps `General.DevicesNeededEnergy*` into needed, primary, emission, and Fuel8/electricity reporting buckets.

The oracle should preserve:

- all spelling quirks in debug/output fields;
- static-list accumulation behavior as deterministic local lists;
- exact `Fuel.Fuel1 -> Fuel8` reporting behavior;
- per-variant monthly guard differences;
- absence of direct climate dependencies.

Recommended debug columns:

- `Period`
- `Group`
- `Variant`
- `ByMonths`
- `Month`
- `Weeks`
- `WeekRegime`
- `MonthPower`
- `WeightedMonthHours`
- `DerivedPower`
- `DerivedWorkSchedule`
- `DevicesNeededEnergy`
- `HeatingEta`
- `HeatingInput`
- `CoolingQintContribution`
- `SavingsPower`
- `SavingsWorkSchedule`
- `SavingsActual`
- `NeededEnergyCategory`
- `FuelInputEnum`
- `FuelReportBucket`

## 19. Minimal fixtures

Fixture 1: Lighting non-monthly scalar periods

- One zone with heating, cooling, and annual periods.
- `Lights.ByMonths = false`.
- Distinct `Power*` and `WorkSchedule*` for all variants.
- Validates scalar period energy, heating inputs, cooling Qint, and `General` aggregation.

Fixture 2: Lighting monthly schedules

- `Lights.ByMonths = true`.
- Different month schedules for Actual/BaseLine/ESM.
- Heating/cooling seasons covering partial and full months.
- Validates `CalcAvgMonthPower`, `weekRegime`, weighted derived power, derived work schedule, and Ref1/Ref2 scalar reference behavior.

Fixture 3: Balanced devices monthly vs scalar

- `BalancedDevices.ByMonths = true`.
- Nonzero heating eta values.
- Validates parallel behavior with lights and aggregation into `HeatAffectingDevices`.

Fixture 4: Non-balanced devices monthly actual/base/ESM

- `NonBalancedDevices.ByMonths = true`.
- Actual, BaseLine, and ESM schedules differ.
- Validates that Actual monthly energy is calculated without overwriting `PowerActual`/`WorkScheduleActual`, while BaseLine/ESM overwrite derived fields.

Fixture 5: Thermal exclusion

- Nonzero `NonBalancedDevices` and zero lights/balanced devices.
- Validates no heating light/appliance input and no cooling Qint contribution from non-balanced devices.

Fixture 6: Savings split

- BaseLine and ESM differ by both `Power` and `WorkSchedule`.
- Validates shared savings distribution, `DevicesNeededEnergySavings` string formatting, and general-only zone savings insertion.

Fixture 7: Fuel/electricity mapping

- Nonzero `General.DevicesNeededEnergy*` for all three groups.
- Validates `Fuel.Fuel1` input, `Fuel8` primary/emission reporting bucket, primary factor `3.0`, and CO2 factor `819.0`.

## Files read

- `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs`
- `analysis/docs/01_call_graph.md`
- `analysis/docs/02_method_index.md`
- `analysis/docs/03_formula_catalog.md`

## Methods analyzed

- `CalculateLightsAndDevicesInputs`
- `GetLightsAndDevicesInputs`
- `CalculateQintRef1`
- `CalculateQintRef2`
- `CalculateQint`
- `CalculateQintBaseLine`
- `CalculateQintESM`
- `CalculatePeriodsReference`
- `CalculatePeriodsActual`
- `CalculatePeriodsBaseLine`
- `CalculatePeriodsESM`
- `CalculatePeriodsReferenceBalanced`
- `CalculatePeriodsActualBalanced`
- `CalculatePeriodsBaseLineBalanced`
- `CalculatePeriodsESMBalanced`
- `CalculatePeriodsReferenceNonBalanced`
- `CalculatePeriodsActualNonBalanced`
- `CalculatePeriodsBaseLineNonBalanced`
- `CalculatePeriodsESMNonBalanced`
- `CalculateHeatingPeriod*`
- `CalculateCoolingPeriod*`
- `CalculateAnnualPeriod*`
- `CalcAvgMonthPower`
- `CalcWeekPower`
- `CalculateLightsSavings`
- `CalculateBalancedDevicesSavings`
- `CalculateNonBalancedDevicesSavings`
- `CalculatePeriod`
- `CheckLightsAndDevicesSavings`
- `SetLightsAndDevicesSavingsvalues`
- `CalculatePrimaryEnergyByTechnologies`
- `GetPrimaryFuelTypeAndValues`
- `GetPrimaryFuelType*`
- `GetPrimaryEnergyCoeficient`
- `CalculateCO2Emissions*`
- `GetFuelTypeCo2*`
- `GetEkoCoeficient`
