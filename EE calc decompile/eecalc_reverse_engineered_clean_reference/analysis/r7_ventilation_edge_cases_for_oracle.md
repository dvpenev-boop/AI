# R7 Ventilation Edge Cases for Oracle

Source: `analysis/r7_ventilation_reverse_engineering.md`

Scope: risky ventilation behaviors that must be clarified before oracle implementation. No implementation, production comparison, or production code changes are included.

## 1. Ref1/Ref2 vs Actual/BaseLine/ESM Differences

### Exact behavior

Heating ventilation:

- Actual uses current ventilation fields and current heating schedules/temperatures.
- BaseLine uses baseline ventilation fields and baseline heating schedules/temperatures.
- ESM uses ESM ventilation fields and ESM heating schedules/temperatures.
- Ref1/Ref2 use reference ventilation scalar fields but call baseline schedule/hour and baseline average ventilation temperature methods.
- Actual/BaseLine/ESM write ETLine data for January and March; Ref1/Ref2 do not.

Cooling ventilation:

- Actual uses actual/current ventilation fields, current cooling schedule, and current project/non-project cooling temperatures.
- BaseLine uses baseline ventilation fields, baseline cooling schedule, and baseline project/non-project cooling temperatures.
- ESM uses ESM ventilation fields, ESM cooling schedule, and ESM project/non-project cooling temperatures.
- Ref1/Ref2 reuse baseline/reference-building schedules while applying reference ventilation scalar fields and reference cooling temperatures.
- `GetWeekHoursCoolingReferences` copies baseline ventilation schedule into both `WorkingScheduleRef` and `WorkingScheduleRef2`.

Ref1/Ref2 baseline schedule reuse is expected reference-building behavior, not a KD item.

### Why it matters

The oracle must not generalize all variants into a single "mode" mapper. The schedule source and the physical/scalar source are intentionally different for Ref1/Ref2. If the oracle uses reference schedules where EECalc reuses baseline schedules, Ref1/Ref2 savings and reference energy will drift even when formulas are correct.

### Oracle test fixture needed

Create one fixture where baseline, actual, ESM, Ref1, and Ref2 values are intentionally different:

- baseline ventilation schedule differs from current and ESM.
- reference temperatures differ from baseline temperatures.
- reference debit/flow/humidity/efficiency values differ from baseline.
- run both heating and cooling ventilation paths.

Expected assertion:

- Ref1/Ref2 debug rows use baseline schedule hours.
- Ref1/Ref2 debug rows use reference temperatures and scalar values.
- Actual/BaseLine/ESM each use their own schedule and scalar values.

### Expected debug columns

- `Mode`
- `Month`
- `ScheduleSource`
- `ScalarSource`
- `TemperatureSource`
- `WorkStart`
- `WorkEnd`
- `SatStart`
- `SatEnd`
- `SunStart`
- `SunEnd`
- `ProjectTemperature`
- `NonProjectTemperature`
- `Debit`
- `FlowTemperature`
- `RelativeHumidity`
- `AverageVentHeatTemp`
- `MonthHours`
- `MonthlyHeatEnergy`
- `PowCooling`
- `PowHeating`

## 2. SecondRecEfficiency > 100 Source Split

### Exact behavior

When `SecondRecEfficiency* > 100`, `VentilationHeatEnergy*` precomputes source buckets:

```text
ResultSourceEnergy* = sum(thermoPumpEnergy)
ResultSourceEnergy2* = sum(monthlySensible + thermoPumpEnergy) - ResultSourceEnergy*
Part1* = ResultSourceEnergy* / total * 100
```

If `Part1*` becomes `NaN` or infinity, EECalc sets `Part1* = 100`.

Later, `CalculateVentNeededEnergy*` does not split `ResultEnergyForHeating*` by `Part1/Part2`. Instead it uses the already-prefilled buckets:

```text
source1BeforeEfficiency = ResultSourceEnergy*
source2BeforeEfficiency = ResultSourceEnergy2*
```

For `SecondRecEfficiency* <= 100`, needed-energy conversion uses the standard split:

```text
source1BeforeEfficiency = ResultEnergyForHeating* * Part1 / 100
source2BeforeEfficiency = ResultEnergyForHeating* * Part2 / 100
```

The second-recovery calculation itself only operates when:

```text
SecondRecEfficiency > 0
3 <= HeatingAirDifference <= 8
```

If `SecondRecEfficiency > 0` but `HeatingAirDifference` is outside that range, monthly heating returns `0`.

### Why it matters

This branch changes the meaning of `ResultSourceEnergy*` and `Part1*`. An oracle that always uses `ResultEnergyForHeating * Part / 100` will fail the `SecondRecEfficiency > 100` path even if monthly heating energy matches.

The `Part1 = 100` fallback is also important for zero or invalid totals.

### Oracle test fixture needed

Create three heating fixtures:

- Normal no-second-recovery fixture: `SecondRecEfficiency = 0`.
- Normal second-recovery fixture: `0 < SecondRecEfficiency <= 100`, `HeatingAirDifference` between `3` and `8`.
- Source-split fixture: `SecondRecEfficiency > 100`, same geometry/schedules as normal second recovery.

Add a fourth edge fixture with `SecondRecEfficiency > 100` and a zero/invalid total to validate `Part1 = 100`.

Expected assertion:

- standard branch uses `ResultEnergyForHeating * Part1/Part2`.
- `>100` branch uses prefilled `ResultSourceEnergy` and `ResultSourceEnergy2`.
- source buckets are captured before efficiency division.

### Expected debug columns

- `Mode`
- `Month`
- `SecondRecEfficiency`
- `HeatingAirDifference`
- `AvgOutdoorTemp`
- `AverageVentHeatTemp`
- `FirstRecoveryTemp`
- `PostRecoveryTemp`
- `H1`
- `H2`
- `QSecondRecovery`
- `ThermoPumpEnergy`
- `AirLift`
- `MonthlySensibleHeat`
- `MonthlyHeatPlusThermoPump`
- `ResultEnergyForHeating`
- `ResultSourceEnergyBeforeNeeded`
- `ResultSourceEnergy2BeforeNeeded`
- `Part1`
- `Part2`
- `NeededSource1BeforeEfficiency`
- `NeededSource2BeforeEfficiency`
- `NeededSource1AfterEfficiency`
- `NeededSource2AfterEfficiency`

## 3. powCooling / powHeating / Withering / CoolingInputs Separation

### Exact behavior

For each cooling-season month, `CalculateMontlyCoolEnergy*` returns:

- `powHeating`: ventilation creates a heating load during cooling season.
- `powCooling`: ventilation creates a cooling load.

`VentilationCoolEnergy*` accumulates:

```text
ResultEnergyForCooling* = sum(powCooling)
ResultEnergyForHeating* = sum(powHeating)
ResultEnergyForWithering* = sum(CalculateWitheringEnergy*)
ResulCoolingInputs* = sum(CalculateCoolingInputs*)
```

`ventCoolingCalculations.CoolingResult.ResulVentilationInputs*` receives `ResulCoolingInputs*`.

`CalculateVentCoolNeededEnergy*` converts only:

```text
ResultEnergyForCooling*
```

It ignores:

- `ResultEnergyForHeating*` from cooling-season ventilation.
- `ResultEnergyForWithering*`.
- `ResulCoolingInputs*` except as a separate result/input total.

### Why it matters

These values look like related cooling terms but have separate downstream meaning. Folding `powHeating`, withering, or cooling inputs into cooling needed-energy would change EECalc parity.

### Oracle test fixture needed

Create three cooling fixtures:

- positive cooling load: outdoor enthalpy above flow enthalpy, producing `powCooling > 0`.
- negative cooling load: outdoor enthalpy below flow enthalpy, producing `powHeating > 0`.
- non-zero withering: humidity setup produces `ResultEnergyForWithering > 0`.

Expected assertion:

- `ResultEnergyForCooling` only contains `powCooling`.
- `ResultEnergyForHeating` only contains `powHeating`.
- `ResultEnergyForWithering` is populated but excluded from `CalculateVentCoolNeededEnergy`.
- `ResulCoolingInputs` is separate from source-energy conversion.

### Expected debug columns

- `Mode`
- `Month`
- `Hour`
- `OutdoorTemp`
- `OutdoorHumidity`
- `FlowTemperature`
- `RelativeHumidity`
- `OutdoorEnthalpy`
- `FlowEnthalpy`
- `OutdoorDensity`
- `FlowDensity`
- `HourDelta`
- `PowCoolingHour`
- `PowHeatingHour`
- `PowCoolingMonth`
- `PowHeatingMonth`
- `WitheringHour`
- `WitheringMonth`
- `CoolingInputsMonth`
- `ResultEnergyForCooling`
- `ResultEnergyForHeating`
- `ResultEnergyForWithering`
- `ResultNeededEnergyCooling`

## 4. GetDaysHours 25-Hour Shifted Climate List

### Exact behavior

Cooling hourly climate lookup uses `GetDaysHours(month)`:

```text
result = [month.Hours[23], month.Hours[0], month.Hours[1], ..., month.Hours[23]]
```

The resulting list has 25 entries. Loops using index `i = 0..23` read the previous-hour shifted climate value:

- `i = 0` reads original hour `23`.
- `i = 1` reads original hour `0`.
- ...
- `i = 23` reads original hour `22`.

The extra final item is original hour `23` at index `24`.

### Why it matters

If the oracle uses direct hour `i -> climate hour i`, all hourly cooling ventilation, latent, and withering calculations can be shifted by one hour. The error is subtle because totals may be close for smooth weather profiles.

### Oracle test fixture needed

Use an hourly weather profile with strongly distinct values by hour:

- hour 23 very different from hour 0.
- monotonic or tagged temperatures/humidity for all 24 hours.

Expected assertion:

- debug row for schedule hour `0` uses source climate hour `23`.
- debug row for schedule hour `1` uses source climate hour `0`.
- debug row for schedule hour `23` uses source climate hour `22`.

### Expected debug columns

- `Mode`
- `Month`
- `LoopHour`
- `ShiftedClimateIndex`
- `SourceClimateHour`
- `OutdoorTemp`
- `OutdoorHumidity`
- `ScheduleDayType`
- `VentilationActive`
- `PowCoolingHour`
- `PowHeatingHour`
- `WitheringHour`

## 5. ResultEnergyForCooling Baseline Row Mismatch

### Exact behavior

`GetVentilationBaseLine` emits editable rows for baseline ventilation fields, but it does not emit a `ResultEnergyForCooling` row.

`SetVentilationBaseLine` reads `ResultEnergyForCooling`.

This means a baseline row round-trip can silently set the cooling baseline energy from a missing/default value depending on `GetValue` behavior.

Classification: KD-V001.

### Why it matters

Savings logic captures baseline rows, mutates them, then writes them back. If a missing `ResultEnergyForCooling` row becomes zero/default on write-back, later recalculation and savings may include a hidden state change unrelated to the intended ESM candidate.

### Oracle test fixture needed

Create a baseline row round-trip fixture:

1. Start with non-zero `ResultEnergyForCoolingBaseLine`.
2. Call oracle equivalent of `GetVentilationBaseLine`.
3. Confirm `ResultEnergyForCooling` is absent.
4. Call oracle equivalent of `SetVentilationBaseLine`.
5. Confirm the resulting baseline cooling-energy field matches EECalc-compatible missing-row behavior.

Expected assertion:

- row list omits `ResultEnergyForCooling`.
- setter attempts to read it.
- debug output marks whether default/zero was applied.

### Expected debug columns

- `RowTag`
- `EmittedByGetter`
- `ReadBySetter`
- `InputValueBeforeRoundTrip`
- `GetterValue`
- `SetterResolvedValue`
- `BaselineResultEnergyForCoolingBefore`
- `BaselineResultEnergyForCoolingAfter`
- `MissingRowDefaultApplied`

## 6. Holiday Handling

### Exact behavior

Heating ventilation:

- month hours ignore holidays.
- average ventilation temperature ignores holidays.
- heating month hours count workdays, Saturdays, and Sundays only.

Cooling ventilation:

- monthly cooling energy loops over workday, Saturday, and Sunday ranges.
- holidays are not used in the observed ventilation cooling calculations.

Related cooling core note from R6, not ventilation-specific:

- some cooling free-cooling paths reuse Sunday schedule for holidays, but R7 ventilation monthly cooling energy does not use holidays in the observed ventilation calculations.

### Why it matters

Ventilation `MonthlyDays` includes `Holydays`, and other subsystems use holidays. An oracle that subtracts or separately handles holidays in R7 ventilation would diverge from EECalc.

### Oracle test fixture needed

Create a fixture where a month has non-zero `Holydays`, and schedules differ by day type:

- workday schedule non-zero.
- Saturday schedule non-zero and distinct.
- Sunday schedule non-zero and distinct.
- holiday count large enough to be visible.

Expected assertion:

- heating `MonthHours` does not change when holiday count changes, except through any already-adjusted `WorkDays` value supplied by `MonthlyDays`.
- ventilation cooling monthly energy has no separate holiday bucket.
- no holiday schedule/source appears in R7 ventilation debug rows.

### Expected debug columns

- `Mode`
- `Month`
- `WorkDays`
- `Saturdays`
- `Sundays`
- `Holydays`
- `WorkHours`
- `SaturdayHours`
- `SundayHours`
- `HolidayHoursUsed`
- `MonthHours`
- `AverageVentHeatTemp`
- `PowCoolingMonth`
- `PowHeatingMonth`
- `CoolingInputsMonth`

## 7. Density Helper Differences by Variant

### Exact behavior

Cooling monthly sensible load uses variant-specific density helpers:

- Actual/BaseLine/ESM monthly cooling use `CalcRo`.
- Ref2 uses `CalcRoW`.
- Ref1 uses `CalcRoW` for workday and Saturday branches.
- Ref1 Sunday branch uses `CalcRo` for outdoor air and `CalcRoW` for flow air.

Withering uses `CalcRoW` in the documented formula:

```text
delta = Debit * (CalcRoW(outdoorTemp) * witheringEntalpy(outdoor)
              - CalcRoW(flowTemp)    * witheringEntalpy(flow))
```

Classification: KD-V009.

### Why it matters

`CalcRo` and `CalcRoW` produce different densities because `CalcRo` includes humidity ratio while `CalcRoW` is dry-air density. Variant-specific helper selection can change the sign and magnitude of `powCooling`/`powHeating`, especially in high-humidity fixtures.

### Oracle test fixture needed

Create a cooling fixture with high humidity and non-trivial flow humidity:

- run Actual, BaseLine, ESM, Ref1, and Ref2 on the same hourly profile.
- include workday, Saturday, and Sunday schedule buckets.
- choose temperatures/humidity so `CalcRo` and `CalcRoW` deltas differ measurably.

Expected assertion:

- Actual/BaseLine/ESM debug rows identify `CalcRo`.
- Ref2 debug rows identify `CalcRoW`.
- Ref1 workday/Saturday identify `CalcRoW`.
- Ref1 Sunday identifies mixed outdoor `CalcRo` and flow `CalcRoW`.

### Expected debug columns

- `Mode`
- `Month`
- `DayType`
- `Hour`
- `OutdoorTemp`
- `OutdoorHumidity`
- `FlowTemperature`
- `FlowHumidity`
- `OutdoorDensityHelper`
- `FlowDensityHelper`
- `OutdoorDensity`
- `FlowDensity`
- `OutdoorEnthalpy`
- `FlowEnthalpy`
- `HourDelta`
- `PowCoolingHour`
- `PowHeatingHour`
