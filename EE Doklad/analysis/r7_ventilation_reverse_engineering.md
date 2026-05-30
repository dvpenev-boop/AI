# R7 Ventilation Systems Reverse Engineering

Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs`

Scope: ventilation heating, ventilation cooling, needed/source energy conversion, baseline/actual/ESM/reference behavior, and savings mechanics. This report is analysis only. No production comparison, oracle implementation, or tests are included.

## 1. Summary

EECalc treats ventilation as a separate heating/cooling subsystem with its own demand calculations, input-energy estimates, source-energy conversion, and savings workflow.

Heating ventilation is driven by the heating season calendar, the ventilation working schedule, the heating schedule project/non-project temperatures, outdoor average monthly temperature, average monthly humidity, heat recovery efficiencies, and generator/fuel efficiencies. The main output is `ResultEnergyForHeating*`, then `CalculateVentNeededEnergy*` converts it to source/needed energy.

Cooling ventilation is driven by the cooling season calendar, ventilation working schedule, hourly outdoor temperature/humidity, flow temperature/humidity, project humidity, and cooling generator efficiencies. It produces cooling, heating, withering/dehumidification, and ventilation input totals. Only `ResultEnergyForCooling*` is converted by `CalculateVentCoolNeededEnergy*`.

Savings logic is implemented by cloning calculation data, building baseline rows, changing one ventilation field at a time, recalculating baseline energy, distributing final bundled savings by individual saving shares, and then attaching savings to the zone.

The implementation has several EECalc quirks that an oracle should preserve, especially schedule source choices, missing baseline rows, off-by-one hour checks, and the special `SecondRecEfficiency > 100` heating path.

## 2. Heating ventilation execution flow

Entry points:

- `VentilationHeatEnergyRef1`
- `VentilationHeatEnergyRef2`
- `VentilationHeatEnergyActual`
- `VentilationHeatEnergyBaseLine`
- `VentilationHeatEnergyEsm`

Per variant, EECalc loops over `section.HeatingSeasons.CalcPeriod*` months.

For each active month:

1. `CalculateMontlyHeatEnergy*` returns the monthly sensible ventilation heating energy and an out parameter `thermoPumpEnergy`.
2. `thermoPumpEnergy` is accumulated separately.
3. If the monthly sensible value is not `NaN`, EECalc adds `monthlySensible + thermoPumpEnergy` to the monthly result list.
4. `ResulHeatingInputs*` is accumulated with:

   ```text
   Debit * 0.34 * (FlowTemperature - ProjectTemperature) * monthHours / 1000
   ```

5. Actual/BaseLine/ESM variants also write January and March ETLine ventilation heating fields using `monthlySensible * HeatedArea`.
6. Actual/BaseLine/ESM update the static/global `innerTemp` from `CalculateAverageVentHeatTemp*`.

After month iteration:

- `ResulHeatingInputs* = sum(monthly heating inputs)`.
- `heatCalculations.HeatingResult.ResulVentilationInputs*` receives the same value.
- `ResultEnergyForHeating* = sum(monthlySensible + thermoPumpEnergy)` only when every month produced a non-NaN entry. If one month is skipped, the result is forced to `0`.

Special heating recovery/source split:

- If `SecondRecEfficiency* > 100`, `VentilationHeatEnergy*` precomputes:

  ```text
  ResultSourceEnergy* = sum(thermoPumpEnergy)
  ResultSourceEnergy2* = sum(monthlySensible + thermoPumpEnergy) - ResultSourceEnergy*
  Part1* = ResultSourceEnergy* / total * 100
  ```

- If `Part1*` becomes `NaN` or infinity, EECalc sets it to `100`.

The heating needed-energy conversion is not part of `VentilationHeatEnergy*`; it is performed later by `CalculateVentNeededEnergy*`.

## 3. Cooling ventilation execution flow

Entry points:

- `VentilationCoolEnergyRef1`
- `VentilationCoolEnergyRef2`
- `VentilationCoolEnergyActual`
- `VentilationCoolEnergyBaseLine`
- `VentilationCoolEnergyEsm`

Per variant, EECalc loops over `section.CoolingSeasons.CalcPeriod*` months.

For each active month:

1. `CalculateMontlyCoolEnergy*` returns two out values:
   - `powHeating`: ventilation creates a heating load during cooling season.
   - `powCooling`: ventilation creates a cooling load.
2. Non-NaN `powCooling` values are accumulated into cooling energy.
3. Non-NaN `powHeating` values are accumulated into heating energy.
4. `CalculateWitheringEnergy*` is accumulated into withering/dehumidification energy.
5. `CalculateCoolingInputs*` is accumulated into ventilation cooling inputs.

After month iteration:

- `ResulCoolingInputs* = sum(CalculateCoolingInputs*)`.
- `ventCoolingCalculations.CoolingResult.ResulVentilationInputs*` receives the same value.
- `ResultEnergyForCooling* = sum(powCooling)`.
- `ResultEnergyForHeating* = sum(powHeating)`.
- `ResultEnergyForWithering* = sum(withering energy)`.

The cooling needed-energy conversion is performed later by `CalculateVentCoolNeededEnergy*`, and it uses only `ResultEnergyForCooling*`.

## 4. Formula catalog

Heating monthly input energy:

```text
ResulHeatingInputs month =
  Debit * 0.34 * (FlowTemperature - HeatingProjectTemperature) * monthHours / 1000
```

Heating average ventilation temperature:

```text
AverageVentHeatTemp =
  (projectHours * ProjectTemperature + nonProjectHours * NonProjectTemperature)
  / (projectHours + nonProjectHours)
```

Heating first recovery intermediate:

```text
num  = innerTemp - FirstRecEfficiency / 100 * (innerTemp - avgOutdoorTemp)
num2 = innerTemp - num + avgOutdoorTemp
     = avgOutdoorTemp + FirstRecEfficiency / 100 * (innerTemp - avgOutdoorTemp)
num3 = num2
```

Heating enthalpy helper `CalcEntalpia(temp, humidity, pb)`:

```text
T_kelvin = 273.15 + temp
satPressure = exp(77.345 + 0.0057 * T_kelvin - 7235 / T_kelvin) / T_kelvin^8.2
partialPressure = humidity * satPressure / 100
x = 0.62198 * partialPressure / (pb - partialPressure)
enthalpy = 1.006 * temp + x * (1.805 * temp + 2501)
```

Heating second recovery path, only when `SecondRecEfficiency > 0` and `3 <= HeatingAirDifference <= 8`:

```text
h1 = CalcEntalpia(num, humidity, Pb)
h2 = CalcEntalpia(MinimumEndTemperature, humidity, Pb)
q = Debit * 1.2 * (h1 - h2) * monthHours / 3600
thermoPumpEnergy = q / (1 - 100 / SecondRecEfficiency)
airLift = thermoPumpEnergy * 1000 / (Debit * 0.34 * monthHours)
```

Then:

```text
if airLift >= HeatingAirDifference:
  thermoPumpEnergy = Debit * 0.34 * HeatingAirDifference * monthHours / 1000

if airLift < FlowTemperature - num2:
  num3 = FlowTemperature - num2 - airLift
  monthlyHeat = Debit * 0.34 * (FlowTemperature - num3) * monthHours / 1000
else:
  thermoPumpEnergy = Debit * 0.34 * (FlowTemperature - num2) * monthHours / 1000
  monthlyHeat = 0
```

Heating without second recovery:

```text
monthlyHeat = Debit * 0.34 * (FlowTemperature - num3) * monthHours / 1000
thermoPumpEnergy = 0
```

Cooling input energy per scheduled hour:

```text
hourInput = Debit * 0.34 * (selectedCoolingTemp - FlowTemperature) / 1000
```

The monthly total is the sum of workday, Saturday, and Sunday hour buckets multiplied by the corresponding day counts.

Cooling enthalpy:

```text
x = CalcAirX(temp, humidity)
enthalpy = 1.006 * temp + x * (2501 + 1.805 * temp)
```

Cooling withering enthalpy:

```text
x = CalcAirX(temp, humidity)
witheringEntalpy = x * (2501 + 1.805 * temp)
```

Cooling monthly sensible load per scheduled hour:

```text
delta = Debit * (density(outdoor) * enthalpy(outdoor)
              - density(flow)    * enthalpy(flow))

if delta < 0:
  heating += abs(delta)
else:
  cooling += delta
```

Then each bucket is converted with:

```text
bucketKWh = bucketSum / 3600 * dayCount
```

Cooling withering monthly energy:

```text
delta = Debit * (CalcRoW(outdoorTemp) * witheringEntalpy(outdoor)
              - CalcRoW(flowTemp)    * witheringEntalpy(flow))
monthlyWithering = sum(delta) / 3600 * dayCount
```

Withering energy is not split by sign.

## 5. Schedule/hour logic

Heating month hours:

```text
monthHours =
  WorkDays  * (WorkEnd - WorkStart)
  + Sundays * (SunEnd  - SunStart), only when Sundays > 0
  + Saturdays * (SatEnd - SatStart), only when Saturdays > 0
```

Heating month hours ignore holidays and do not handle overnight schedules.

Heating average ventilation temperature counts each ventilation schedule hour and checks whether it overlaps the heating schedule:

```text
project if HeatingScheduleStart <= hour && HeatingScheduleEnd > hour
otherwise non-project
```

It does this separately for workday, Saturday, and Sunday schedules, then multiplies by the monthly day counts. Holidays are ignored.

Cooling weekly hours helpers:

```text
weekHours =
  5 * (WorkEnd - WorkStart)
  + (SunEnd - SunStart)
  + (SatEnd - SatStart)
```

For reference weekly hours, EECalc copies baseline ventilation schedules into both `WorkingScheduleRef` and `WorkingScheduleRef2`.

Cooling hourly climate lookup uses `GetDaysHours(month)`:

```text
result = [month.Hours[23], month.Hours[0], month.Hours[1], ..., month.Hours[23]]
```

This means the hour list has 25 entries and loop index `i` reads a shifted previous-hour style value for `i = 0..23`.

Cooling input temperature selection:

- Workday uses `CoolingStart <= i && CoolingEnd > i`.
- Saturday/Sunday use `CoolingStart <= i && CoolingEnd >= i`.

That is an explicit end-boundary difference between workdays and weekends.

Cooling monthly energy loops over ventilation schedule ranges for workday, Saturday, and Sunday. Holidays are not used in the observed ventilation calculations.

## 6. Climate dependencies

Heating ventilation uses:

- `Climate.SolarRadiation.Months[month].AvgTemp` as monthly average outdoor temperature.
- `Climate.TempHumidity.Months[month].Hours[*].Humidity` averaged across the month for humidity.
- `Climate.Pb` in `CalcEntalpia`.

Cooling ventilation uses:

- `Climate.TempHumidity.Months[month].Hours[*].Temp`.
- `Climate.TempHumidity.Months[month].Hours[*].Humidity`.
- The shifted list from `GetDaysHours`.
- `CalcAirX`, `CalcRo`, and `CalcRoW` helpers.

Reference cooling temperatures:

- `CalculateCoolingInputsRef1/Ref2` use cooling project/non-project temperatures from reference cooling calculations.
- Ref1/Ref2 monthly ventilation cooling reuse baseline/reference-building schedules for hourly loops while replacing selected physical parameters.

## 7. Energy conversion / efficiency logic

Heating needed-energy conversion:

If `SecondRecEfficiency* > 100`, EECalc does not split `ResultEnergyForHeating*` by `Part1/Part2`. Instead it uses the prefilled source-energy buckets from `VentilationHeatEnergy*`:

```text
source1BeforeEfficiency = ResultSourceEnergy*
source2BeforeEfficiency = ResultSourceEnergy2*
```

Each source is divided by:

```text
TransmitTempEfficiency / 100
* SupplyNetEfficiency / 100
* Automatic / 100
* EnergyManagement / 100
* GeneratorHeatEfficiency / 100
```

If `SecondRecEfficiency* <= 100`, EECalc splits:

```text
source1BeforeEfficiency = ResultEnergyForHeating* * Part1 / 100
source2BeforeEfficiency = ResultEnergyForHeating* * Part2 / 100
```

Then applies the same heating efficiency chain.

Cooling needed-energy conversion:

```text
source1BeforeEfficiency = ResultEnergyForCooling* * Part1 / 100
source2BeforeEfficiency = ResultEnergyForCooling* * Part2 / 100
```

Each source is divided by:

```text
TransmitTempEfficiency / 100
* SupplyNetEfficiency / 100
* Automatic / 100
* EnergyManagement / 100
* GeneratorColdEfficiency / 100
```

For both heating and cooling:

- `NaN` and infinity source results are replaced with `0`.
- `ResultNeededEnergy* = ResultSourceEnergy* + ResultSourceEnergy2*`.
- ESM variants set `ResultNeededEnergySavings = (ResultNeededEnergyBaseLine - ResultNeededEnergyESM).ToString("F3")`.

## 8. Baseline / Actual / ESM / Ref1 / Ref2 differences

Heating:

- Actual uses current ventilation fields and current heating schedule/temperatures.
- BaseLine uses baseline ventilation fields and baseline heating schedule/temperatures.
- ESM uses ESM ventilation fields and ESM heating schedule/temperatures.
- Ref1 and Ref2 use reference ventilation scalar fields but call baseline schedule/hour and baseline average ventilation temperature methods.
- Actual/BaseLine/ESM write ETLine data for January and March. Ref1/Ref2 do not.

Cooling:

- Actual uses actual/current ventilation fields, current cooling schedule, and current cooling project/non-project temperatures.
- BaseLine uses baseline ventilation fields, baseline cooling schedule, and baseline project/non-project temperatures.
- ESM uses ESM ventilation fields, ESM cooling schedule, and ESM project/non-project temperatures.
- Ref1/Ref2 reuse baseline/reference-building schedules while applying reference ventilation scalar fields and reference cooling temperatures. This is expected reference-building behavior: schedules remain baseline-like, while selected physical parameters are replaced.
- `GetWeekHoursCoolingReferences` copies baseline ventilation schedule into both reference schedules.

Baseline rows:

`GetVentilationBaseLine` returns editable rows for ventilation baseline fields including schedules, debit, flow temperature, humidity values, recovery settings, fuel/efficiency fields, input totals, and needed energy.

`SetVentilationBaseLine` writes those rows back to baseline fields before recalculation.

Important mismatch:

- `SetVentilationBaseLine` reads `ResultEnergyForCooling`.
- `GetVentilationBaseLine` does not emit a `ResultEnergyForCooling` row.

## 9. Savings logic

Heating savings entry point: `CalculateVentilationHeatingSavings`.

Cooling savings entry point: `CalculateVentilationCoolingSavings`.

Shared execution pattern:

1. Return immediately if `zone == null`.
2. If the zone does not have the relevant mode (`HasHeating` or `HasCooling`), add an empty savings list to the zone and return.
3. Clone `CalculationData`.
4. Build candidate savings with `CheckForVentilationSavings`.
5. Add fuel-source savings with `CheckForFuelSavings`.
6. Normalize labels/values with `SetVentilationSavingsValues`.
7. Capture baseline rows from `GetVentilationBaseLine`.
8. Store virtual baseline needed energy from the `ResultNeededEnergy` row.
9. For each individual saving:
   - Clone calculation data and section.
   - Apply the single changed row.
   - For `WorkingSchedule`, call the ventilation schedule copy helper.
   - Write rows back with `SetVentilationBaseLine`.
   - Recalculate baseline ventilation energy.
   - Recalculate needed energy and generator efficiency.
   - Store individual `NetEnergy` and `Saving`.
10. Compute each saving `Part = Saving / sum(Saving)`.
11. Apply all savings together to a fresh clone.
12. Recalculate baseline ventilation energy and needed energy.
13. Compute combined savings:

   ```text
   combinedSaving = originalBaselineResultNeededEnergy - recalculatedResultNeededEnergy
   ActualSaving = combinedSaving * Part
   ```

14. If positive and negative actual savings coexist, call `CheckAndCalculateNegativeSavings`.
15. Add savings to the zone with the relevant Bulgarian label:
   - `Вентилация - Отопление`
   - `Вентилация - Охлаждане`

Working schedule savings are special:

- Heating calls `CopyVentilationHeatingWorkingSchedule`.
- Cooling calls `CopyVentilationCoolingWorkingSchedule`.
- Both copy ESM schedule fields into BaseLine schedule fields on the cloned section.

## 10. EECalc quirks / KD candidates

KD-V001: `GetVentilationBaseLine` omits `ResultEnergyForCooling`, but `SetVentilationBaseLine` reads `ResultEnergyForCooling`. This can silently set cooling baseline energy to default/zero depending on `GetValue`.

KD-V002: Working schedule savings copy ESM schedule fields into BaseLine fields. They do not copy Actual into BaseLine.

KD-V003: Savings share calculation uses `Part = Saving / totalSaving` with no observed zero guard.

KD-V004: Heating `SecondRecEfficiency > 100` triggers a special source split based on `thermoPumpEnergy`, then `CalculateVentNeededEnergy*` divides the prefilled source buckets rather than splitting `ResultEnergyForHeating*` by parts.

KD-V005: Heating second recovery only operates when `HeatingAirDifference` is between `3` and `8` inclusive. If `SecondRecEfficiency > 0` and the difference is outside that range, monthly heating returns `0`.

KD-V006: Cooling input schedule end comparison differs by day type: workday uses `< End`, while Saturday/Sunday use `<= End`.

KD-V008: `GetDaysHours` prepends hour 23 before hours 0-23, creating a 25-item shifted climate sequence.

KD-V009: Cooling density helper use differs by variant. Actual/BaseLine/ESM monthly cooling use `CalcRo`; Ref2 uses `CalcRoW`; Ref1 uses `CalcRoW` for work/Saturday but Sunday uses `CalcRo` for outdoor air and `CalcRoW` for flow air.

KD-V010: Heating month hours and average ventilation temperature ignore holidays.

KD-V011: `VentilationHeatEnergy*` sets `ResultEnergyForHeating*` to `0` unless every heating-season month contributes a non-NaN monthly value.

KD-V012: Heating input energy is not clamped; it can be negative if `FlowTemperature < ProjectTemperature`.

KD-V013: Actual/BaseLine/ESM ETLine updates are limited to January and March and use `monthlySensible * HeatedArea`, while the main monthly total includes `thermoPumpEnergy`.

KD-V014: Cooling withering energy is stored in `ResultEnergyForWithering*` but is not included in `CalculateVentCoolNeededEnergy*`.

KD-V015: Cooling-season ventilation heating (`powHeating`) is stored in `ResultEnergyForHeating*`, but cooling needed-energy conversion ignores it.

## Expected Design Behaviors

Ref1/Ref2 reuse baseline schedules. Reference-building calculations are expected to keep baseline schedules and replace only selected physical parameters such as temperatures, infiltration-related values, ventilation scalar inputs, and generator/efficiency assumptions. This is not classified as a KD item.

## 11. Required input fields

Shared ventilation fields per variant:

- `Debit*`
- `FlowTemperature*`
- `RelativeHumidity*`
- `ProjectHumidity*`
- `Part1*`, `Part2*`
- `Fuel1*`, `Fuel2*`
- `TransmitTempEfficiency*`, `TransmitTempEfficiency2*`
- `SupplyNetEfficiency*`, `SupplyNetEfficiency2*`
- `Automatic*`, `Automatic2*`
- `EnergyManagement*`, `EnergyManagement2*`
- `GeneratorHeatEfficiency1*`, `GeneratorHeatEfficiency2*`
- `GeneratorColdEfficiency1*`, `GeneratorColdEfficiency2*`

Heating-specific ventilation fields:

- `FirstRecEfficiency*`
- `SecondRecEfficiency*`
- `HeatingAirDifference*`
- `MinimumEndTemperature*`
- Heating ventilation work/Saturday/Sunday start/end schedule fields for current, baseline, and ESM.
- Heating season calc-period month flags.
- Heating season monthly day counts: workdays, Saturdays, Sundays.
- Heating project/non-project temperatures per variant.
- Heating schedule start/end fields per variant.
- `HeatedArea` for ETLine writes.

Cooling-specific ventilation fields:

- Cooling ventilation work/Saturday/Sunday start/end schedule fields for current, baseline, and ESM.
- Cooling season calc-period month flags.
- Cooling season monthly day counts: workdays, Saturdays, Sundays.
- Cooling project/non-project temperatures per variant.
- Cooling schedule start/end fields per variant.

Climate fields:

- Monthly average outdoor temperature from solar radiation climate.
- Hourly temperature/humidity from temp/humidity climate.
- Barometric pressure `Pb`.

Source binding:

- monthly average outdoor temperature comes from `DefaultParams.xml` `SolarRadiation/Months/Month/AvgTemp` through `PreferencesManager`.
- hourly outdoor temperature comes from `DefaultParams.xml` `TempHumidity/Months/Month/Hours/Temp`.
- hourly outdoor humidity comes from `DefaultParams.xml` `TempHumidity/Months/Month/Hours/Humidity`.
- `Pb` comes from `DefaultParams.xml` `ClimateZone/Pb` and is used by heating ventilation `CalcEntalpia`.
- `DefaultSunParams.xml` is not used by R7 heating or cooling ventilation.
- `calcInput.General.ClimateZone` is matched to `DefaultParams.xml` `ClimateZone.Number` as a zero-based EECalc climate-zone value. For current ordinance/json comparisons, `ZoneId = Number + 1`.

See `analysis/reference-data/source_binding_audit_heating_cooling_ventilation.md` for the consolidated R1-R7 data-source binding audit.

Savings/baseLine fields:

- Baseline editable row tags returned by `GetVentilationBaseLine`.
- Zone flags `HasHeating` and `HasCooling`.
- Existing fuel/source savings infrastructure.

## 12. Proposed oracle design

Do not implement yet. For a future oracle, keep ventilation separate from the R5/R6 core cooling and heating envelope oracles.

Suggested structure:

- `EecalcVentilationOracle`
  - `HeatingVentilation`
    - `MonthHours`
    - `AverageVentHeatTemp`
    - `MonthlyHeatEnergy`
    - `VentilationHeatEnergy`
    - `VentNeededEnergy`
  - `CoolingVentilation`
    - `DaysHours`
    - `CoolingInputs`
    - `MonthlyCoolEnergy`
    - `WitheringEnergy`
    - `VentilationCoolEnergy`
    - `VentCoolNeededEnergy`
  - `Savings`
    - `GetVentilationBaseLineRows`
    - `SetVentilationBaseLineRows`
    - `CopyHeatingWorkingSchedule`
    - `CopyCoolingWorkingSchedule`
    - single-change and bundled saving calculation helpers

Preserve variant-specific behavior rather than trying to generalize too early. Ref1/Ref2 deliberately reuse baseline schedules while applying reference physical parameters; density helper behavior remains variant-specific.

Recommended debug rows for future CSV fixtures:

- Heating: month, monthHours, averageVentHeatTemp, firstRecoveryTemp, postRecoveryTemp, thermoPumpEnergy, monthlyHeat, heatingInputs.
- Cooling: month, shiftedHourTemp/Humidity source marker, powHeating, powCooling, witheringEnergy, coolingInputs.
- Needed energy: source1BeforeEfficiency, source2BeforeEfficiency, source1AfterEfficiency, source2AfterEfficiency, resultNeededEnergy.
- Savings: tag, baselineValue, changedValue, netEnergy, saving, part, actualSaving.

## 13. Minimal fixtures

Fixture A: Heating ventilation, no second recovery.

- One heating month enabled.
- Workday ventilation schedule only.
- `FirstRecEfficiency > 0`, `SecondRecEfficiency = 0`.
- Project and non-project heating temperatures differ.
- Expected to validate month hours, average ventilation temperature, first recovery, monthly heat, heating inputs, and standard needed-energy split.

Fixture B: Heating ventilation, second recovery normal branch.

- One heating month enabled.
- `SecondRecEfficiency > 0`.
- `HeatingAirDifference` between `3` and `8`.
- Inputs chosen so `airLift < FlowTemperature - num2`.
- Expected to validate `thermoPumpEnergy`, adjusted `num3`, and combined `monthlyHeat + thermoPumpEnergy`.

Fixture C: Heating ventilation, `SecondRecEfficiency > 100` source split.

- Same as Fixture B but with `SecondRecEfficiency > 100`.
- Expected to validate prefilled `ResultSourceEnergy`, `ResultSourceEnergy2`, `Part1`, and alternate `CalculateVentNeededEnergy` path.

Fixture D: Cooling ventilation, positive cooling load.

- One cooling month enabled.
- Workday ventilation schedule only.
- Outdoor enthalpy above flow enthalpy.
- Expected to validate shifted `GetDaysHours`, `powCooling`, cooling inputs, and cold-generator needed-energy conversion.

Fixture E: Cooling ventilation, negative load during cooling season.

- One cooling month enabled.
- Outdoor enthalpy below flow enthalpy.
- Expected to validate `powHeating` accumulation and confirm it does not enter `CalculateVentCoolNeededEnergy`.

Fixture F: Cooling withering.

- One cooling month enabled.
- Relative humidity and project humidity chosen to produce non-zero withering energy.
- Expected to validate `CalculateWitheringEnergy*` and confirm it remains separate from cooling needed-energy conversion.

Fixture G: Savings schedule copy.

- Baseline and ESM ventilation schedules intentionally different.
- Apply only `WorkingSchedule` saving.
- Expected to validate that ESM schedule fields are copied into baseline fields before baseline recalculation.

Fixture H: Baseline row roundtrip.

- Build rows from `GetVentilationBaseLine`, then apply `SetVentilationBaseLine`.
- Expected to expose that `ResultEnergyForCooling` is read by setter but not emitted by getter.
