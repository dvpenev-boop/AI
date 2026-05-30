# R6 Cooling reverse engineering

## 1. Summary

Source of truth:

`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs`

Scope of this document:

- `CoolingCalculations`
- `CalculateCoolingEnergyActual/BaseLine/ESM/Ref1/Ref2`
- `CalculateCoolingQtr*`
- `CalculateQinf*`
- `CalculateQve*`
- `CalculateQgain*`
- `CalculateQsol*`
- `CalculateQint*`
- `CalculateETA`
- `CalculateAc*`
- `CalculateLatentHeats*`
- `ClaculateQfreecooling*`

No production code was changed. This is the reverse-engineering basis for a future cooling oracle.

High-level result:

```text
Qloss = QtrCooling + Qinf
Eta = cooling utilization factor from gamma = Qgain / Qloss
QcoolRaw = Qgain - Eta * Qloss + QlatentOccupants + QlatentInf + QlatentVent
QcoolWithInputs = QcoolRaw + QfreeCooling + Qve
ResultNoInputs = Sum(QcoolRaw) / HeatedArea
ResultNetEnergy = ResultNoInputs - Sum(QfreeCooling) - ResulVentilationInputs*
```

Important naming issue: the decompiled code spells free cooling as `ClaculateQfreecooling*`.

## 2. Entry point and scenario order

`CoolingCalculations` is at lines 123-135.

```text
months = section.CalcPeriod(FirstMonthCool, LastMonthCool, FirstDayCool, LastDayCool)

if buildingZone.HasRefenceValues:
    CalculateCoolingEnergyRef1(...)
    CalculateCoolingEnergyRef2(...)

CalculateCoolingEnergyActual(...)
CalculateCoolingEnergyBaseLine(...)
CalculateCoolingEnergyESM(...)
```

Unlike the heating monthly engine, the cooling engine only uses the cooling season period.

Reference quirk: `CalculateCoolingEnergyRef1` and `CalculateCoolingEnergyRef2` create cloned temp sections and call `ApplyValuesToTempSectionRef1/Ref2` at lines 175-176 and 216-217, but the subsequent monthly calculations still pass `section`, not `tempSection`. In this decompiled form the temp section has no visible effect in the cooling loop.

## 3. Monthly cooling pipeline

The five scenario methods are structurally the same:

- Ref1: lines 169-208.
- Ref2: lines 210-249.
- Actual: lines 251-288.
- BaseLine: lines 290-313.
- ESM: lines 315-337.

Per month:

```text
Qgain = CalculateQgain*(...)
Qloss = CalculateCoolingQtr*(...) + CalculateQinf*(...)
Ac = CalculateAc*(...)
Eta = CalculateETA(Ac, Qloss, Qgain, section)

QcoolRaw =
    Qgain
    - Eta * Qloss
    + CalculateQLatentOccupants*(...)
    + CalculateLatentHeatsInf*(...)
    + CalculateLatentHeatsVent*(...)

Qfree = ClaculateQfreecooling*(...)
Qve = CalculateQve*(...)
QcoolWithInputs = QcoolRaw + Qfree + Qve
```

`MonthDataCooling` is populated only for Ref1, Ref2 and Actual in the shown code. BaseLine and ESM aggregate totals but do not populate `MonthDataCoolingList`.

Monthly `MonthDataCooling.ParameterQtr` is actually `Qloss = QtrCooling + Qinf`, not only transmission. The `ParameterNi` field stores cooling `Eta`.

## 4. Final result aggregation

For every scenario:

```text
sumRaw = Sum(QcoolRaw)
ResultNoInputs = sumRaw / HeatedArea
CoolingInputs = Sum(Qfree)
ResultNetEnergy = ResultNoInputs - CoolingInputs - ResulVentilationInputs*
```

Concrete lines:

- Ref1: lines 204-207.
- Ref2: lines 245-248.
- Actual: lines 284-287.
- BaseLine: lines 309-312.
- ESM: lines 334-337.

The monthly `QcoolWithInputs` list is built but not used directly in the final net-energy formula in these methods. Ventilation input subtraction depends on `ResulVentilationInputs*`, which is written by the ventilation-cooling engine elsewhere.

## 5. CalculateETA

`CalculateETA` is at lines 985-1002.

```text
gamma = gainings / loses
section.Test.ParameterGamma = gamma

if gamma > 0 and Abs(gamma - 1) > 0.01:
    Eta = (1 - gamma^(-Ac)) / (1 - gamma^(-(Ac + 1)))
elif Abs(gamma - 1) < 0.01:
    Eta = Ac / (Ac + 1)
elif gamma < 0:
    Eta = 1
else:
    Eta = 0
```

Boundary behavior mirrors R5 except the exponent is negative:

- `gamma == 0` returns `0`.
- `gamma == 0.99` or `1.01` exactly returns `0`, because both comparisons are strict.
- `loses == 0` can produce `Infinity`/`NaN`; there is no local guard.

## 6. CalculateAc

`CalculateAc*` is at lines 1004-1051.

Conceptual formula:

```text
avgOutdoor = climate.SolarRadiation.Months[month].AvgTemp
avgInnerCool = weighted project/non-project cooling setpoint
HtrCooling = CalculateCoolingHtr*(section, avgOutdoor, avgInnerCool)
Hinf = HeatedVolume * Infiltracion* * 0.34
tau = HeatedArea * HeatCapacity / (HtrCooling + Hinf)
Ac = 1 + tau / 15
```

Scenario mapping:

- Ref1/Ref2 use baseline cooling schedules for average cooling temperature but Ref1/Ref2 temperatures and infiltration values.
- Actual uses current cooling schedules and actual temperatures/infiltration.
- BaseLine uses baseline cooling schedules and baseline temperatures/infiltration.
- ESM uses ESM cooling schedules, ESM temperatures/infiltration, and `CalculateCoolingHtrESM`.

There is no clamping or divide-by-zero guard in `CalculateAc*`.

## 7. QtrCooling and HtrCooling

`CalculateCoolingQtr*` is at lines 1901-1942.

```text
avgOutdoor = climate.SolarRadiation.Months[month].AvgTemp
avgInnerCool = CalculateAverageCoolingTemp*(...)
HtrCooling = CalculateCoolingHtr*(...)
QtrCooling = HtrCooling * (ProjectDegreeHours + NonProjectDegreeHours) / 1000
```

Where:

```text
ProjectDegreeHours =
    (ProjectTemperature* - avgOutdoor) * occupiedCoolingHours

NonProjectDegreeHours =
    (NonProjectTemperature* - avgOutdoor) * unoccupiedCoolingHours
```

The average cooling setpoint methods at lines 1972-2040 compute:

```text
avgInnerCool =
    (occupiedCoolingHours * ProjectTemperature*
     + unoccupiedCoolingHours * NonProjectTemperature*)
    / (occupiedCoolingHours + unoccupiedCoolingHours)
```

`CalculateCoolingHtr` at lines 1944-1956:

```text
Hu = SumWallDirecrionsHu1Cooling(Current)
   + CalcCeilingsParameterHu2Cooling(Roof.Current)
   + CalcFloorsParameterHu3Cooling(Floor.Current)

Hd = CalculateParameterHdCurrent(section)
Hg = CalculateParameterHgCurrent(section)
Htr = Hd + Hg + Hu
```

`CalculateCoolingHtrESM` at lines 1958-1969 is the same but uses ESM wall/roof/floor and ESM Hd/Hg.

Confirmed cooling transmission quirks:

- `SumWallDirecrionsHu1Cooling` uses `section.NorthWalls.Current` eight times, lines 2127-2137.
- `SumWallDirecrionsHu1CoolingESM` uses `section.NorthWalls.Esm` eight times, lines 2140-2150.
- `CalcWallDirectionParameterHu1Cooling` uses `IneerA5` as both area and U value, lines 2176-2179.
- `CalcCeilingsParameterHu2Cooling` uses `CeilingA5` as both area and U value, lines 2210-2213.
- `CalcFloorsParameterHu3Cooling` uses `OtherFloorS4` for both floor layer 4 and layer 6 temperature deltas, lines 2242 and 2250.

## 8. Qinf

`CalculateQinf*` is at lines 1846-1874.

```text
Hinf = HeatedVolume * Infiltracion* * 0.34
Qinf = Hinf * (ProjectDegreeHours + NonProjectDegreeHours) / 1000
```

Scenario mapping:

- Ref1 uses `InfiltracionRef1`, `ProjectTemperatureRef1`, `NonProjectTemperatureRef1`, and baseline cooling schedules.
- Ref2 uses `InfiltracionRef2`, `ProjectTemperatureRef2`, `NonProjectTemperatureRef2`, and baseline cooling schedules.
- Actual uses `InfiltracionActual` and current cooling schedules.
- BaseLine uses `InfiltracionBaseLine` and baseline cooling schedules.
- ESM uses `InfiltracionESM` and ESM cooling schedules.

## 9. Qgain

`CalculateQgain*` is at lines 1277-1315.

```text
Qgain = Qsol + Qint + Qoccupants
```

This differs from R4 heating `Qgn`: cooling `Qgain` includes solar, lights, balanced devices, and sensible occupants.

Occupants:

```text
Qoccupants = MetabolicHeat * OccupantHours* / 1000 * HeatedArea
```

Ref1 and Ref2 use `CalculateQoccupantsBaseLine` in `CalculateQgainRef1/Ref2`, so sensible occupant schedules are baseline for references.

## 10. Qsol

`CalculateQsol*` is at lines 1776-1844.

```text
occupiedCoolingHours =
    WorkDays * coolingWorkHours
    + Saturdays * coolingSatHours
    + Sundays * coolingSunHours

unoccupiedCoolingHours =
    WorkDays * (24 - coolingWorkHours)
    + Saturdays * (24 - coolingSatHours)
    + Sundays * (24 - coolingSunHours)
    + Holydays * 24

Qsol = (CalculateTrasparentFsol* + CalculateNonTrasparentFsol*) *
       (occupiedCoolingHours + unoccupiedCoolingHours) / 1000
```

Because occupied plus unoccupied hours is the whole cooling month period in hours, Qsol effectively uses all hours in the monthly cooling period, not only active cooling hours.

Scenario mapping:

- Ref1, Ref2, and BaseLine use baseline cooling schedules and non-ESM Fsol methods.
- Actual uses current cooling schedules and non-ESM Fsol methods.
- ESM uses ESM cooling schedules and ESM Fsol methods.

## 11. Qint

`CalculateQint*` is at lines 1317-1350.

```text
LightsKWhPerM2 =
    if ByMonths:
        CalcAvgMonthPower(schedule, month) * (weekRegime * month.Weeks) / 1000
    else:
        Cooling.Power* * (Cooling.WorkSchedule* * month.Weeks) / 1000

BalancedDevicesKWhPerM2 =
    same pattern as lights

Qint = LightsKWhPerM2 * HeatedArea + BalancedDevicesKWhPerM2 * HeatedArea
```

`weekRegime` is a static field. It is assigned by `CalcWeekPower` around lines 6826-6827. A cooling oracle should either reproduce the same call ordering or avoid depending on stale `weekRegime` by explicitly matching how `CalcAvgMonthPower` is reached in EECalc.

`CalculateQintRef1/Ref2` do not use `ByMonths`; they use `Lights.Cooling.PowerRef*`, `BalancedDevices.Cooling.PowerRef*`, and their cooling work schedules.

## 12. Latent occupants

`CalculateQLatentOccupants*` is at lines 1373-1406.

```text
QLatentOccupants =
    LatentMetabolicHeat * OccupantHours* / 1000 * HeatedArea
```

Scenario mapping:

- Ref1, Ref2, and Actual call `CalculateOccupantshours` and therefore current occupant schedules.
- BaseLine calls `CalculateOccupantshoursBaseLine`.
- ESM calls `CalculateOccupantshoursESM`.

This is asymmetric with sensible Ref1/Ref2 occupant gains, where `CalculateQgainRef1/Ref2` use baseline occupant schedules.

## 13. Latent infiltration

`CalculateLatentHeatsInf*` starts at lines 340, 404, 468, 532 and 596.

For each day type it sums hourly moisture-density deltas:

```text
outside = CalcRo(outdoorTemp, outdoorRH) * CalcAirX(outdoorTemp, outdoorRH)
insideOccupied = CalcRo(ProjectTemperature*, ProjectHumidity*) * CalcAirX(ProjectTemperature*, ProjectHumidity*)
insideUnoccupied = CalcRo(NonProjectTemperature*, ProjectHumidity*) * CalcAirX(NonProjectTemperature*, ProjectHumidity*)

hourDelta = outside - insideOccupied/insideUnoccupied
```

Then:

```text
QLatentInf =
    HeatedVolume * Infiltracion* / HeatedArea
    * Sum(dayTypeHourDeltas * dayCount)
    * 0.6947222222222222
```

The result is forced to `0` when it is `NaN` or `Infinity`.

The constant `0.6947222222222222` is used consistently in latent infiltration and latent ventilation. Preserve it exactly in the oracle.

## 14. Latent ventilation

`CalculateLatentHeatsVent*` starts at lines 660, 721, 782, 843 and 904.

For each ventilation schedule hour:

```text
supply = CalcRoW(FlowTemperature*) * CalcAirX(FlowTemperature*, RelativeHumidity*)
outside = CalcRoW(outdoorTemp) * CalcAirX(outdoorTemp, outdoorRH)

hourLatentVent = Debit* * (supply - outside) * 0.6947222222222222
```

The method sums workday, Saturday, Sunday and holiday blocks and zeros any `NaN`/`Infinity` day-type subtotal.

Important quirks:

- Saturday post-ventilation hours multiply by `Debit*` twice in all five scenarios, e.g. Ref1 line 691 and Actual line 813.
- Holiday latent ventilation compares against `CalcRoW(NonProjectTemperature*) * CalcAirX(NonProjectTemperature*, ProjectHumidity*)`, not against outdoor hourly weather.

## 15. Psychrometric helpers

Lines 965-983:

```text
Tkelvin = 273.15 + temp
satPressure = e^(77.345 + 0.0057 * Tkelvin - 7235 / Tkelvin) / Tkelvin^8.2
vapourPressure = humidity * satPressure / 100
CalcAirX = 0.62198 * vapourPressure / (101325 - vapourPressure)

CalcRoW = 101325 / (286.9 * (temp + 273.15))
CalcRo = CalcRoW(temp) * (1 + x) / (1 + 1.609 * x)
```

`CalcRo` uses humid-air density. `CalcRoW` is dry-air density and is used in latent ventilation.

## 16. Qve

`CalculateQve*` starts at lines 1432, 1491, 1556, 1621 and 1686.

For each day type:

```text
Hve = Debit* * 0.34
hourQve = Hve * (selectedIndoorTemperature - FlowTemperature*) / 1000
Qve = Sum(hourQve across ventilation schedule partition) * dayCount
```

The selected indoor temperature is project temperature during occupant schedule hours and non-project temperature otherwise.

Scenario mapping:

- Ref1/Ref2 use baseline ventilation schedules but current occupant schedules for the project/non-project choice, and Ref1/Ref2 temperatures/flow temperatures.
- Actual uses current ventilation and occupant schedules.
- BaseLine uses baseline ventilation and occupant schedules.
- ESM uses ESM ventilation and occupant schedules.

Holiday handling:

- Ref1 has no holiday block in the visible method and returns workdays + Saturdays + Sundays at line 1488.
- Ref2, Actual, BaseLine and ESM add 24 holiday hours using non-project temperature, lines 1547-1553, 1612-1618, 1677-1683 and 1742-1748.

## 17. Free cooling

`ClaculateQfreecooling*` is at lines 1082-1275.

```text
Hfree = Debit* * 0.34

nightHours = GetNightWorkingHours(start, end)
hourFree = Hfree * (ProjectTemperature* - outdoorHourlyTemp) / 1000

Qfree =
    Sum(workNightHours at project temp) * WorkDays
    + Sum(satNightHours at project temp) * Saturdays
    + Sum(sunNightHours at project temp) * Sundays
    + Sum(sunNightHours at non-project temp) * Holydays
```

`GetNightWorkingHours` at lines 1054-1080 supports schedules that cross midnight:

```text
if start == end: no hours
if start > end: [0..end-1] + [start..23]
else: [start..end-1]
```

Important quirks:

- Holiday free cooling reuses the Sunday night-ventilation schedule, not a holiday-specific schedule.
- Hour indexes beyond available climate hours fall back to index `0`.
- Positive `Qfree` is later subtracted from final net energy; negative values are not clamped locally.

## 18. Oracle implementation checklist

## 18a. Source binding addendum

R6 cooling uses `PreferencesManager.GetClimateZoneParams(climateZone)`. `PreferencesManager` loads `Xml/DefaultParams.xml`.

Climate and solar bindings:

- monthly `AvgTemp`: `DefaultParams.xml` `SolarRadiation/Months/Month/AvgTemp`; used by `CalculateAc*`, `CalculateCoolingQtr*`, `CalculateQinf*`, and visible monthly result `AvgTemp` assignments.
- orientation solar `N/E/S/W/H`: `DefaultParams.xml` `SolarRadiation/Months/Month/N/E/S/W/H`; used by cooling `Qsol*` through `CalculateTrasparentFsol*` and `CalculateNonTrasparentFsol*`.
- hourly outdoor temperature: `DefaultParams.xml` `TempHumidity/Months/Month/Hours/Temp`; used by latent and free-cooling paths.
- hourly outdoor humidity: `DefaultParams.xml` `TempHumidity/Months/Month/Hours/Humidity`; used by latent infiltration/ventilation psychrometric deltas.

`DefaultSunParams.xml` is not used by R6 cooling. It is reserved for the separate solar hot-water / `SunEnergyPreferencesManager` path.

Indoor/project temperatures, indoor humidity, infiltration, heat capacity, lighting/device inputs, and ventilation-flow inputs are sourced from `CalculationData`. Geometry, heated area/volume, envelope inputs, and schedules are sourced from `Section`.

Cooling psychrometric helpers use hardcoded pressure `101325` in `CalcAirX`/`CalcRoW`; they do not use `ClimateZone.Pb`.

See `analysis/reference-data/source_binding_audit_heating_cooling_ventilation.md` for the consolidated R1-R7 data-source binding audit.

For a cooling oracle, implement in this order:

1. Cooling-period `MonthlyDays` parity.
2. Hour counters for cooling, occupants, ventilation and night ventilation.
3. `Qsol`, `Qint`, sensible occupants and `Qgain`.
4. `HtrCooling`, `QtrCooling`, `Hinf` and `Qinf`, including the NorthWalls and A5 quirks.
5. `Ac`, `ETA`, and raw monthly cooling demand.
6. Latent occupants, latent infiltration and latent ventilation, preserving the `0.6947222222222222` constant and NaN/Infinity zeroing.
7. `Qve` and `Qfree`.
8. Final result aggregation with `ResulVentilationInputs*` supplied by the ventilation-cooling engine.

Validation probes to capture:

- Per-month `Qgain`, `Qtr+Qinf`, `Ac`, `Eta`, `QcoolRaw`, `Qfree`, `Qve`.
- Final `ResulNoInputsNetEnergy*`, `ResulCoolingInputs*`, `ResulVentilationInputs*`, `ResulNetEnergy*`.
- Cases with zero cooling losses, zero active cooling hours, night ventilation crossing midnight, holidays, and monthly `ByMonths` schedules.
