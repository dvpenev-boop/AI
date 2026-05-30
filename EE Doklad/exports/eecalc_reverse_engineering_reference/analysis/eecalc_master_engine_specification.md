# EECalc Master Engine Specification

Status: implementation specification for future EECalc-compatible oracles. This document consolidates the existing reverse-engineering reports only. It does not introduce new formulas, modify production code, run parity, or rewire providers.

## 1. Executive summary

The EECalc engine is a monthly, zone-first calculation pipeline with a final building aggregation layer.

Confirmed high-level structure:

- R1/R2 establish calendar and heating ventilation/transmission degree-hour foundations.
- R3 computes transmission coefficients and monthly `Qtr`.
- R4 computes heating `Qgn` as solar gains only; occupant, lighting, and appliance gains are handled separately.
- R5 computes `Gamma`, `aH`, `Ni`, and heating `Qnd`.
- R6 computes cooling losses, gains, latent terms, free cooling, ventilation inputs, and final cooling net energy.
- R7 computes ventilation systems separately for heating and cooling, including needed/source energy and savings behavior.
- R8 computes ordinary DHW/BGV, solar hot water, and BGV pumps.
- R9 computes lighting and devices as both thermal inputs and direct electrical/general energy.
- R10 aggregates zone results into building results, primary energy, fuel tables, CO2, VEI, and energy scale rows.

The future oracle suite should support three modes:

- `LegacyEECalcStrict`: preserve EECalc behavior exactly, including confirmed defects and legacy data issues.
- `LegacyEECalcCorrectedData`: preserve formulas/aggregation but correct confirmed data errors such as `KD-DATA-001`.
- `CurrentOrdinance`: use current ordinance data providers while preserving legacy reporting behavior unless explicitly redesigned.

Important implementation boundary:

- The climate provider layer exists, but it has not yet been wired into production calculations.
- Formula code in future oracles should consume providers; production formulas currently still use legacy XML access paths.

## 2. Engine module map

Recommended oracle modules:

| Module | Source docs | Responsibility |
| --- | --- | --- |
| `EecalcMonthlyDaysOracle` | R1 material in heating oracle report and source binding audit | Fixed-year monthly day/week rows from section periods. |
| `EecalcHeatingVentilationOracle` | R2 material in heating oracle report | `Hve = 0.34 * V * n`, monthly `Qve`, degree-hour helpers. |
| `EecalcTransmissionOracle` | `r3_qtr_htr_reverse_engineering.md` | `Hd`, `Hg`, `Hu`, `Htr`, `Qtr`. |
| `EecalcQgnOracle` | `r4_qgn_gains_reverse_engineering.md` | Heating solar-only `Qgn`, solar Fsol primitives, occupant hour inputs. |
| `EecalcGammaNiOracle` | `r5_gamma_ni_reverse_engineering.md` | Heating `Gamma`, `aH`, `Ni`, `Qnd`. |
| `EecalcMonthlyCoolingOracle` | `r6_cooling_reverse_engineering.md`, `cooling_complete_oracle_report.md` | Cooling monthly balance and result aggregation. |
| `EecalcVentilationOracle` | `r7_ventilation_reverse_engineering.md`, edge-case report | Heating/cooling ventilation systems, inputs, withering, needed/source energy, savings. |
| `EecalcDhwBgvOracle` | `r8_dhw_bgv_reverse_engineering.md` | Ordinary hot water, solar DHW, BGV pumps, DHW savings. |
| `EecalcLightingDevicesOracle` | `r9_lighting_devices_reverse_engineering.md` | Lighting, balanced devices, non-balanced devices, schedules, savings. |
| `EecalcAggregationOracle` | `r10_aggregation_primary_co2_class_reverse_engineering.md` | Building aggregation, primary energy, fuel, CO2, VEI, scale. |

Execution ordering for a full building oracle:

```text
Data providers
  -> MonthlyDays / calendar
  -> zone heating/cooling/ventilation/DHW/lights/devices modules
  -> zone result tables
  -> building aggregation
  -> primary energy / fuel / CO2 / VEI / scale
  -> debug CSVs and parity assertions
```

## 3. Data sources and providers

Authoritative legacy sources:

- `reference/eecalc-config/DefaultParams.xml`
- `reference/eecalc-config/DefaultSunParams.xml`
- decompiled EECalc code under `reference/eecalc-decompiled`

Current ordinance source:

- `EE.Doklad/Data/climate_zones.json`

Provider interfaces already implemented but not wired into production calculations:

```text
IClimateDataProvider
  GetMonthlyAvgTemp(zoneId, month)
  GetSolarRadiation(zoneId, month)
  GetHourlyClimateData(zoneId, month)
  GetPb(zoneId)

ISunEnergyDataProvider
  GetMonthlyAvgTemp(zoneId, month)
  GetMonthlyRadiation(zoneId, month)
  GetMonthlyCloudiness(zoneId, month)
```

Climate source binding for R1-R7:

| Input | Legacy source | Formula users |
| --- | --- | --- |
| monthly `AvgTemp` | `DefaultParams.xml` `SolarRadiation/Months/Month/AvgTemp` | Heating Qtr/Qve/aH, cooling Qtr/Qinf/Ac, ventilation heating. |
| solar `N/E/S/W/H` | `DefaultParams.xml` `SolarRadiation/Months/Month/N/E/S/W/H` | Heating `Qgn`, cooling `Qsol`. |
| hourly temperature | `DefaultParams.xml` `TempHumidity` | Cooling latent/free-cooling, ventilation cooling/withering. |
| hourly humidity | `DefaultParams.xml` `TempHumidity` | Cooling latent, ventilation enthalpy/psychrometrics. |
| `Pb` | `DefaultParams.xml` `ClimateZone/Pb` | Heating ventilation enthalpy. |

Solar DHW source binding:

| Input | Legacy source | Formula users |
| --- | --- | --- |
| monthly sun `Radiation` | `DefaultSunParams.xml` | `CalculateParameterHtMonthly`, solar DHW month rows. |
| monthly sun `AvgTemp` | `DefaultSunParams.xml` | solar DHW `SunMonth.Tm`. |
| monthly `Cloudiness` | `DefaultSunParams.xml` | diffuse/cloudiness correction. |
| ambient delta in `CalculateParameterX` | `DefaultParams.xml` monthly `AvgTemp` | `deltaT = 100 - AvgTemp`. |

Zone mapping:

```text
DefaultParams.xml ClimateZone.Number = 0..8
DefaultSunParams.xml ClimateZone.Number = 0..8
climate_zones.json ZoneId = 1..9
Json ZoneId = XML Number + 1
```

Orientation rules:

```text
NE = (N + E) / 2
SE = (S + E) / 2
SW = (S + W) / 2
NW = (N + W) / 2
```

Diagonal orientations must be derived, not stored.

`CurrentOrdinance` hourly data limitation:

- `climate_zones.json` does not provide authoritative hourly profiles.
- `CorrectedJsonClimateDataProvider.GetHourlyClimateData` currently uses a temporary 24-hour monthly fallback.
- This is not suitable for final R6/R7 hourly parity.

## 4. Calculation modes

### LegacyEECalcStrict

Purpose: exact EECalc parity.

Rules:

- Use `DefaultParams.xml` exactly, including `KD-DATA-001`.
- Use `DefaultSunParams.xml` exactly.
- Preserve all confirmed EECalc behaviors and confirmed defects.
- Preserve `Fuel.Fuel1 -> Fuel8` reporting-bucket mapping.
- Preserve `KD-A001` duplicate `Fuel1` addition in all total fuel variant calculations.

### LegacyEECalcCorrectedData

Purpose: legacy formulas with corrected known data errors.

Rules:

- Preserve formulas and aggregation behavior.
- Correct January `AvgTemp` data for zones 1-3:
  - Zone 1 January: `1.9`
  - Zone 2 January: `0.5`
  - Zone 3 January: `0.1`
- Do not apply formula corrections unless the future oracle explicitly exposes a corrected-formula mode.

### CurrentOrdinance

Purpose: current ordinance data with a stable EECalc-compatible calculation/reporting model.

Rules:

- Use `climate_zones.json` for current ordinance climate data.
- Preserve `Fuel.Fuel1 -> Fuel8` reporting unless the reporting model is intentionally redesigned.
- Do not treat legacy reporting buckets as formula errors.
- Any correction of confirmed defects, such as `KD-A001`, must be an explicit design decision.

## 5. MonthlyDays/calendar rules

Confirmed R1/R2 calendar behavior:

- EECalc monthly period rows are based on section period inputs, not XML `HeatingSeason`/`CoolingSeason` nodes.
- The reconstructed oracle uses fixed year `2006`.
- `MonthlyDays` contains:
  - `Month`
  - `WorkDays`
  - `Saturdays`
  - `Sundays`
  - `Holydays`
  - `Weeks`
- Heating and cooling modules call section `CalcPeriod(...)` for the selected season.
- Qtr and Qve must reuse the same degree-hour period logic.

Known schedule details:

- R4 heating `Qgn` uses direct `End - Start` durations.
- R6 `GetNightWorkingHours` supports crossing midnight.
- R7 `GetDaysHours` prepends hour 23 before hours 0-23, creating a 25-item shifted climate list (`KD-V008`).
- R7 heating month hours and average ventilation temperature ignore holidays (`KD-V010`).
- R9 lighting/devices do not inspect holidays directly; they depend on `MonthlyDays.Weeks` and monthly schedule fields.

## 6. Heating engine formulas

### R2 ventilation heat transfer

Confirmed formula:

```text
Hve = 0.34 * HeatedVolume * Infiltration
Qve = Hve * DegreeHours / 1000
```

Degree hours come from the R1/R2 project/non-project helper logic and must be shared with `Qtr`.

### R3 transmission

Confirmed formula:

```text
Qtr = Htr * (CalcAvgProjectTemp + CalcAvgNonProjectTemp) / 1000
Htr = Hd + Hg + Hu
Hd = SumAllDirectionsWallsCurrent + SumAllDirectionWindowsCurrent + SumNonTrasparentRoof + SumTrasparentRoof
Hg = Floor.Current.AccumulateFloorA * Floor.Current.AccumulateFloorU
Hu = HuWalls + HuCeilings + HuFloors
```

Confirmed legacy behavior / defect:

- `KD-004`: `SumWallDirecrionsHu1` uses `NorthWalls.Current` eight times.

Confirmed formulas with spelling preserved:

```text
CalcWallDirectionParameterHu1 component 5 uses IneerA5 * IneerA5
CalcCeilingsParameterHu2 component 5 uses CeilingA5 * CeilingA5
```

### R4 heating gains

Confirmed formula:

```text
QgnRaw =
  (CalculateNonTrasparentFsol + CalculateTrasparentFsol)
  * (projectHours + nonProjectHours)

Qgn = QgnRaw / 1000
```

Confirmed behavior:

- Heating `Qgn` is solar gains only.
- Occupant metabolic heat is added separately into `Gamma`.
- Lights and balanced devices are accumulated separately by `CalculateLightsAndDevicesInputs`.
- Non-balanced devices are not thermal heating gains.
- Ref1 and Ref2 use baseline `Qgn`; no dedicated `CalculateParameterQgnRef1/Ref2` methods were found.

Solar primitive formulas:

```text
TransparentFsol =
  windowA * windowG * radiation
  - directionFactor * (0.04 * windowG * windowA * 11 * 4 * windowE * 0.0000000567 * 283^3)

NonTransparentFsol =
  outerWallAlfa * 0.04 * outerWallU * outerWallArea * radiation
  - directionFactor * (0.04 * outerWallU * outerWallArea * 11 * 4 * outerWallEpsi * 0.0000000567 * 283^3)
```

`directionFactor = 1.0` for horizontal, `0.5` otherwise.

### R5 Gamma / Ni / Qnd

Confirmed formulas:

```text
Qht = Qtr + Qve
Gamma = (Qgn + MetabolicHeat) / Qht
tau = HeatedArea * HeatCapacity / (Htr + Hve)
aH = 1 + tau / 15
```

Ni formula:

```text
if gamma > 0 and abs(gamma - 1) > 0.01:
    Ni = (1 - gamma^aH) / (1 - gamma^(aH + 1))
if gamma < 0:
    Ni = 1
if abs(gamma - 1) < 0.01:
    Ni = aH / (aH + 1)
else:
    Ni = 0
```

Confirmed edge behavior:

- `gamma == 0.99` and `gamma == 1.01` return `0`.
- No Ni clamping to `[0, 1]`.
- No local NaN/infinity guard in Ni methods.

Heating actual final monthly net:

```text
RawQnd = Qht - Ni * Qgn
FinalQnd = RawQnd / HeatedArea - Ni * MetabolicHeatPerArea
```

Baseline/ref differences:

- Baseline `aH` uses Current envelope Htr with baseline Hve/temperature.
- Ref1/Ref2 use baseline Qgn.
- ESM uses ESM Htr/Hve/Qgn paths.

## 7. Cooling engine formulas

Confirmed monthly cooling balance:

```text
Qgain = Qsol + Qint + Qoccupants
Qloss = QtrCooling + Qinf
gamma = Qgain / Qloss
Ac = 1 + (HeatedArea * HeatCapacity / (HtrCooling + Hinf)) / 15
Eta = cooling utilization factor

QcoolRaw =
  Qgain
  - Eta * Qloss
  + QLatentOccupants
  + QLatentInf
  + QLatentVent

QcoolWithInputs = QcoolRaw + QfreeCooling + QveCooling
ResultNoInputsNetEnergy = Sum(QcoolRaw) / HeatedArea
ResultCoolingInputs = Sum(QfreeCooling)
ResultNetEnergy = ResultNoInputsNetEnergy - ResultCoolingInputs - ResulVentilationInputs
```

Cooling Eta:

```text
if gamma > 0 and abs(gamma - 1) > 0.01:
    Eta = (1 - gamma^(-Ac)) / (1 - gamma^(-(Ac + 1)))
elif abs(gamma - 1) < 0.01:
    Eta = Ac / (Ac + 1)
elif gamma < 0:
    Eta = 1
else:
    Eta = 0
```

Confirmed formulas:

```text
Hinf = HeatedVolume * Infiltracion * 0.34
Qinf = Hinf * (ProjectDegreeHours + NonProjectDegreeHours) / 1000
Qsol = (CalculateTrasparentFsol + CalculateNonTrasparentFsol) * totalCoolingHours / 1000
Qint = (LightsCoolingEnergy + BalancedDevicesCoolingEnergy) * HeatedArea
Qoccupants = MetabolicHeat * OccupantHours / 1000 * HeatedArea
QLatentOccupants = LatentMetabolicHeat * OccupantHours / 1000 * HeatedArea
```

Psychrometric helpers:

```text
Tkelvin = 273.15 + temp
satPressure = e^(77.345 + 0.0057 * Tkelvin - 7235 / Tkelvin) / Tkelvin^8.2
vapourPressure = humidity * satPressure / 100
CalcAirX = 0.62198 * vapourPressure / (101325 - vapourPressure)
CalcRoW = 101325 / (286.9 * (temp + 273.15))
CalcRo = CalcRoW(temp) * (1 + x) / (1 + 1.609 * x)
```

Confirmed cooling quirks:

- `KD-C001`: `SumWallDirecrionsHu1Cooling` uses north walls eight times.
- `KD-C002`: cooling wall layer 5 uses `InnerA5 * InnerA5`.
- `KD-C003`: cooling ceiling layer 5 uses `CeilingA5 * CeilingA5`.
- `KD-C004`: cooling floor layer 6 uses `OtherFloorS4` for the temperature delta.
- `KD-C005`: latent ventilation Saturday post-ventilation hours multiply by `Debit` twice.
- `KD-C006`: free-cooling holidays reuse the Sunday night-ventilation schedule.

## 8. Ventilation engine formulas

Heating ventilation flow:

```text
VentilationHeatEnergy*
  -> monthly heating ventilation loop
  -> CalculateMontlyHeatEnergy*
  -> CalculateAverageVentHeatTemp*
  -> ResultEnergyForHeating*
  -> ResulVentilationInputs*
  -> CalculateVentNeededEnergy*
```

Cooling ventilation flow:

```text
VentilationCoolEnergy*
  -> CalculateCoolingInputs*
  -> CalculateMontlyCoolEnergy*
  -> CalculateWitheringEnergy*
  -> ResultEnergyForCooling*
  -> ResultEnergyForWithering*
  -> CalculateVentCoolNeededEnergy*
```

Needed/source energy pattern:

```text
source1Demand = ResultEnergy * Part1 / 100
ResultSourceEnergy = source1Demand / efficiencyChain1
source2Demand = ResultEnergy * Part2 / 100
ResultSourceEnergy2 = source2Demand / efficiencyChain2
ResultNeededEnergy = ResultSourceEnergy + ResultSourceEnergy2
```

Important confirmed ventilation behaviors:

- Ref1/Ref2 reuse baseline schedules. This is expected reference-building behavior, not a KD.
- Heating `SecondRecEfficiency > 100` uses a special thermo-pump/source split path (`KD-V004`).
- Heating second recovery only operates when `HeatingAirDifference` is between `3` and `8` inclusive (`KD-V005`).
- Heating input energy is not clamped and can be negative (`KD-V012`).
- Cooling withering is stored separately and not included in cooling needed-energy conversion (`KD-V014`).
- Cooling-season ventilation heating is stored in `ResultEnergyForHeating*`, but cooling needed-energy conversion ignores it (`KD-V015`).

Risky edge cases for oracle fixtures:

- Ref1/Ref2 vs Actual/BaseLine/ESM schedule/physical parameter split.
- `SecondRecEfficiency > 100` source split.
- `powCooling`, `powHeating`, withering, and cooling input separation.
- `GetDaysHours` 25-hour shifted climate list.
- `ResultEnergyForCooling` baseline row mismatch.
- Holiday handling.
- Density helper differences by variant.

## 9. DHW/BGV formulas

Ordinary DHW/BGV:

```text
MixedWater = Consumption * totalHeatedArea / 1000
ResulNetEnergy = 1.161 * TempDifference * 0.98 * Consumption / 1000
ResultEnergyForHeating = max(0, ResulNetEnergy - SunEnergy)
```

Needed/source energy:

```text
source1Demand = ResultEnergyForHeating * Part1 / 100
ResultSourceEnergy =
  source1Demand /
  (SupplyNetEfficiency/100
   * Automatic/100
   * EnergyManagement/100
   * GeneratorHeatEfficiency1/100)

source2Demand = ResultEnergyForHeating * Part2 / 100
ResultSourceEnergy2 =
  source2Demand /
  (SupplyNetEfficiency2/100
   * Automatic2/100
   * EnergyManagement2/100
   * GeneratorHeatEfficiency2/100)

ResultNeededEnergy = ResultSourceEnergy + ResultSourceEnergy2
```

Generator hot water efficiency:

```text
HeatEfficiencyGenerating =
  (ResultSourceEnergy * GeneratorHeatEfficiency1
   + ResultSourceEnergy2 * GeneratorHeatEfficiency2)
  / (ResultSourceEnergy + ResultSourceEnergy2)
```

Confirmed DHW/BGV behaviors:

- Ordinary DHW needed-energy conversion omits `TransmitTempEfficiency`.
- `MixedWater` uses total heated area, but `ResulNetEnergy` uses `Consumption`, not `MixedWater`.
- BGV building aggregation uses first-zone values.
- Normal BGV pumps and solar `BGVPumpsTotal` are separate concepts.

## 10. Solar DHW formulas

Solar hot-water flow:

```text
CalculateHotWaterNeededPower
  -> ClearTableValues
  -> SumCollectorsArea
  -> HotWaterNeededPower
  -> HotWaterNeededPowerTotal
  -> CalculateParameterHtMonthly
  -> CalculateParameterX
  -> CalculateParameterY
  -> CalculateXwithCorrection
  -> CalculateParameterF
  -> SetTableResults
```

Confirmed formulas:

```text
CollectorsArea = AbsorbingSurface * CollectorsCount

HotWaterNeededPower =
  WaterUsage * (HotWaterTemperature - ColdWaterTemperature)
  * 1.163 / 1000
  * (DaysInWeek * month.Weeks)

HotWaterNeededPowerTotal =
  WaterUsage * (HotWaterTemperature - ColdWaterTemperature)
  * 1.163 / 1000
  * month.TotalDays

F = 1.029*y - 0.065*x - 0.245*y^2 + 0.0018*x^2 + 0.0215*y^3
```

Storage correction:

```text
if 37.5 < AcumulatorVolume / CollectorsArea < 300:
    correction = (AcumulatorVolume / CollectorsArea / 75)^-0.25
    correctedX = correction * x
else:
    correctedX = x
```

Solar pump energy:

```text
monthPumpEnergy = sunMonth.Days * 8 * PumpsVolume
BGVPumpsTotal = round(sum(monthPumpEnergy) * totalHeatedArea / 1000, 1)
```

Source binding:

- `DefaultSunParams.xml` supplies `Radiation`, `AvgTemp`, and `Cloudiness`.
- `CalculateParameterX` also reads `DefaultParams.xml` monthly `AvgTemp`.

## 11. Lighting/devices formulas

Lighting/devices roles:

- `Lights` and `BalancedDevices` affect heating thermal inputs.
- `Lights` and `BalancedDevices` affect cooling `Qint`.
- `NonBalancedDevices` do not affect thermal balance.
- All three groups contribute annual/general electrical energy and aggregation tables.

Non-monthly period energy:

```text
DevicesNeededEnergy = WorkSchedule * Power * Sum(Weeks) / 1000
```

Monthly schedule formulas:

```text
weekRegime = WorkDays * 5 + Saturdays + Sundays

CalcWeekPower =
  (WorkDays * WorkDaysUsedEnergy * 5
   + Saturdays * SaturdaysUsedEnergy
   + Sundays * SundaysUsedEnergy)
  / weekRegime

DevicesNeededEnergy =
  Sum(CalcWeekPower(month) * weekRegime(month) * month.Weeks) / 1000
```

Heating input:

```text
LightInputMonth = LightEnergyMonth * ParameterEta
DeviceInputMonth = BalancedDeviceEnergyMonth * ParameterEta
```

Cooling `Qint`:

```text
Qint = (LightsCoolingEnergy + BalancedDevicesCoolingEnergy) * HeatedArea
```

Confirmed lighting/device behaviors:

- `CalcWeekPower` mutates static `weekRegime`.
- Ref1/Ref2 thermal inputs ignore monthly schedules.
- Actual monthly non-balanced devices compute energy but do not overwrite derived scalar fields; BaseLine/ESM do.
- Only `General` savings are added to zone savings.
- Direct electrical loads pass `Fuel.Fuel1` and report to `Fuel8`.

## 12. Aggregation / primary / CO2 / fuel formulas

Building aggregation:

```text
TotalHeatedArea =
  sum(zone.Heating.Area.HeatedArea)
  - sum(zone.Heating.Area.OtherArea)
```

Needed total:

```text
NeededEnergyTable.Total =
  Heating
  + Cooling
  + HeatingVentilation
  + CoolingVentilation
  + BGV
  + BGVPumps
  + FansAndPumps
  + Lights
  + HeatAffectingDevices
  + NonHeatAffectingDevices
  + Other
```

Zone-to-building aggregation rules:

- Heating/heating ventilation sum zones where `HasHeating`.
- Cooling/cooling ventilation sum zones where `HasCooling`.
- BGV uses the first zone.
- Solar `BGVPumpsTotal` uses the first zone.
- Lights/devices/fans/pumps/other sum all zones.

Primary energy:

```text
PrimaryTechnology =
  GetPrimaryEnergyCoeficient(Fuel1, SourceEnergy1) * area
  + GetPrimaryEnergyCoeficient(Fuel2, SourceEnergy2) * area
```

Primary coefficients:

| Fuel | Coefficient |
| --- | ---: |
| `Fuel1` | 3.0 |
| `Fuel2` | 1.1 |
| `Fuel3` | 1.1 |
| `Fuel4` | 1.2 |
| `Fuel5` | 1.2 |
| `Fuel6` | 1.05 |
| `Fuel7` | 1.25 |
| `Fuel8` | 1.1 |
| `Fuel9` | 1.3 |
| `Fuel10` | 1.1 |
| `Fuel11` | 1.2 |

CO2 coefficients:

| Fuel | Coefficient |
| --- | ---: |
| `Fuel1` | 819 |
| `Fuel2` | 202 |
| `Fuel3` | 227 |
| `Fuel4` | 341 |
| `Fuel5` | 364 |
| `Fuel6` | 43 |
| `Fuel7` | 351 |
| `Fuel8` | 267 |
| `Fuel9` | 290 |
| `Fuel10` | 279 |
| `Fuel11` | 354 |

Reporting bucket behavior:

```text
Fuel.Fuel1 -> Fuel8 reporting bucket
Fuel.Fuel8 -> Fuel1 reporting bucket
all other Fuel enum values -> same-number reporting bucket
```

`KD-A009` confirms this across `FuelEnergyTable`, `PrimaryEnergyFuelTable`, and `EmissionEnergySupplyTable` for Ref1, Ref2, Actual, BaseLine, and ESM. It is legacy reporting-bucket mapping, not a formula defect or calculation error.

Primary fuel table factors for the inverted buckets:

```text
Fuel.Fuel1 -> Fuel8 with factor 3.0
Fuel.Fuel8 -> Fuel1 with factor 1.1
```

CO2 supply table uses the same bucket inversion:

```text
Fuel.Fuel1 -> Fuel8
Fuel.Fuel8 -> Fuel1
```

Total fuel calculation:

```text
FuelEnergyTable.Total.VariantArea =
  Fuel1 + Fuel1 + Fuel2 + Fuel3 + Fuel4 + Fuel5
  + Fuel6 + Fuel7 + Fuel8 + Fuel9 + Fuel10 + Fuel11
```

`KD-A001` confirms the duplicate `Fuel1` term as a legacy aggregation defect for all total fuel variants: `CalculateTotalFuelRef1`, `CalculateTotalFuelRef2`, `CalculateTotalFuelActual`, `CalculateTotalFuelBaseLine`, and `CalculateTotalFuelESM`.

## 13. Energy class / scale logic

`SetScaleValues`:

```text
Scale climateZoneParams =
  BuildingTypesManager.GetClimateZoneParams(calcInput.General.InvestigationMethod)
SetScaleType(climateZoneParams, calcInput.General.BuildingResults)
```

`SetScaleType` writes:

```text
BuildingScaleType.PoiterValue = (int)PrimaryEnergyTable.Total.ESM
BuildingScaleType.PoiterValueBaseLine = (int)PrimaryEnergyTable.Total.BaseLine
```

For `InvestigationType.ReferentValues`, thresholds are derived from final primary Ref1/Ref2:

```text
Aplus.Max = int(0.25 * Ref2)
A.Max = int(0.5 * Ref2)
A.Min = int(0.25 * Ref2)
B.Max = int(Ref2)
B.Min = int(0.5 * Ref2 + 1)
C.Max = int(0.5 * (Ref2 + Ref1))
C.Min = int(Ref2 + 1)
D.Max = int(Ref1)
D.Min = int(0.5 * (Ref2 + Ref1) + 1)
E.Max = int(1.25 * Ref1)
E.Min = int(Ref1 + 1)
F.Max = int(1.5 * Ref1)
F.Min = int(1.25 * Ref1 + 1)
G.Max = int(1.5 * Ref1)
G.Min = int(1.5 * Ref1)
```

For other investigation types, thresholds come from embedded `BuildingTypesManager` scale rows.

No final `CalculateClass` assignment was found in the decompiled EECalc final layer. The core sets scale thresholds and pointer values; report/UI code can infer the displayed class.

## 14. Known differences index

Confirmed from `validation_known_differences.md`:

| ID | Classification | Summary |
| --- | --- | --- |
| `KD-004` | Confirmed legacy defect/quirk | `SumWallDirecrionsHu1` uses `NorthWalls.Current` eight times. |
| `KD-DATA-001` | Confirmed legacy XML data error | Zones 1-3 January `DefaultParams.xml` temperatures have wrong negative signs. |
| `KD-V001` | Confirmed ventilation KD | `GetVentilationBaseLine` omits `ResultEnergyForCooling`, `SetVentilationBaseLine` reads it. |
| `KD-V002` | Confirmed ventilation KD | Working schedule savings copy ESM fields into BaseLine fields. |
| `KD-V003` | Confirmed ventilation KD | Savings share uses `Part = Saving / totalSaving` with no observed zero guard. |
| `KD-V004` | Confirmed ventilation KD | Heating `SecondRecEfficiency > 100` uses special thermo-pump/source split. |
| `KD-V005` | Confirmed ventilation KD | Second recovery only operates when `HeatingAirDifference` is 3..8 inclusive. |
| `KD-V006` | Confirmed ventilation KD | Cooling input schedule end comparison differs by day type. |
| `KD-V008` | Confirmed ventilation KD | `GetDaysHours` prepends hour 23 before hours 0-23. |
| `KD-V009` | Confirmed ventilation KD | Cooling density helper use differs by variant. |
| `KD-V010` | Confirmed ventilation KD | Heating month hours and average ventilation temperature ignore holidays. |
| `KD-V011` | Confirmed ventilation KD | `VentilationHeatEnergy*` sets heating result to 0 unless all heating-season months contribute non-NaN. |
| `KD-V012` | Confirmed ventilation KD | Heating input energy is not clamped and can be negative. |
| `KD-V013` | Confirmed ventilation KD | ETLine updates limited to January and March and use `monthlySensible * HeatedArea`. |
| `KD-V014` | Confirmed ventilation KD | Cooling withering is stored separately and excluded from cooling needed-energy conversion. |
| `KD-V015` | Confirmed ventilation KD | Cooling-season ventilation heating is stored but ignored by cooling needed-energy conversion. |
| `KD-A001` | Confirmed legacy aggregation defect | All total fuel variants add `Fuel1` twice. |
| `KD-A009` | Confirmed legacy reporting behavior | `Fuel1/Fuel8` reporting-bucket inversion across fuel, primary fuel, and CO2 supply tables for all variants. |

Cooling confirmed quirks from R6/cooling oracle report:

| ID | Classification | Summary |
| --- | --- | --- |
| `KD-C001` | Confirmed cooling quirk | `SumWallDirecrionsHu1Cooling` uses north walls eight times. |
| `KD-C002` | Confirmed cooling quirk | Cooling wall layer 5 uses `InnerA5 * InnerA5`. |
| `KD-C003` | Confirmed cooling quirk | Cooling ceiling layer 5 uses `CeilingA5 * CeilingA5`. |
| `KD-C004` | Confirmed cooling quirk | Cooling floor layer 6 uses `OtherFloorS4` for the temperature delta. |
| `KD-C005` | Confirmed cooling quirk | Latent ventilation Saturday post-ventilation hours multiply by `Debit` twice. |
| `KD-C006` | Confirmed cooling quirk | Free-cooling holidays reuse Sunday night-ventilation schedule. |

R10 unresolved/candidate findings:

| ID | Current classification | Summary |
| --- | --- | --- |
| `KD-A002` | Unresolved candidate | Actual/BaseLine/ESM BGVPumps use first-zone area row times total area instead of summing zones. |
| `KD-A003` | Unresolved candidate | `CalculatePrimaryEnergyPerArea` initially totals `Devices`, later total uses `Other`. |
| `KD-A004` | Unresolved candidate | Building CO2 adds Actual solar `BGVPumpsTotal` to Ref1/Ref2 emissions. |
| `KD-A005` | Unresolved candidate | Zone net-energy Ref2 heating ventilation absolute uses Ref1 value in observed path. |
| `KD-A006` | Unresolved candidate | Some CO2 hot-water fuel-source calls use reference fuel fields for BaseLine/ESM rows. |
| `KD-A007` | Unresolved candidate | `GetPrimaryFuelTypeRef2` lacks explicit NaN/infinity guard pattern. |
| `KD-A008` | Confirmed spelling behavior | `SetScaleType` writes `PoiterValue` spelling. |

Other unresolved candidates remain in R4, R5, R8, and R9 source docs and should be promoted only after binary/parity confirmation.

## 15. Confirmed ILSpy findings

From `analysis/ilspy_verified_findings.md`:

### KD-A001

Status: confirmed defect.

Methods:

- `CalculateTotalFuelRef1`
- `CalculateTotalFuelRef2`
- `CalculateTotalFuelActual`
- `CalculateTotalFuelBaseLine`
- `CalculateTotalFuelESM`

Behavior:

- `Fuel1` is added twice in every total fuel variant calculation.

Mode decision:

- `LegacyEECalcStrict`: preserve.
- Corrected mode: may fix.

### KD-A009

Status: confirmed legacy behavior.

Methods:

- `GetFuelTypeRef1`
- `GetPrimaryFuelTypeRef1`
- `GetFuelTypeCo2Ref1`

Verified tables:

- `FuelEnergyTable`
- `PrimaryEnergyFuelTable`
- `EmissionEnergySupplyTable`

Verified variants:

- `Ref1`
- `Ref2`
- `Actual`
- `BaseLine`
- `ESM`

Behavior:

```text
Fuel.Fuel1 -> Fuel8 reporting bucket
Fuel.Fuel8 -> Fuel1 reporting bucket
All other fuel enum values -> same-number reporting buckets
```

Primary factors:

```text
Fuel.Fuel1 -> Fuel8 with factor 3.0
Fuel.Fuel8 -> Fuel1 with factor 1.1
```

CO2 supply table:

```text
Fuel.Fuel1 -> Fuel8
Fuel.Fuel8 -> Fuel1
```

Mode decision:

- `LegacyEECalcStrict`: preserve.
- `CurrentOrdinance`: preserve unless reporting is intentionally redesigned.

## 16. Legacy behaviors vs confirmed defects

Confirmed formulas:

- `Hve = 0.34 * V * n`.
- `Qtr = Htr * degreeHours / 1000`.
- Heating `Qgn` is solar-only.
- Heating `Gamma = (Qgn + MetabolicHeat) / Qht`.
- `aH = 1 + (HeatedArea * HeatCapacity / (Htr + Hve)) / 15`.
- Cooling `Qgain`, `Qloss`, `Ac`, `Eta`, and `QcoolRaw` formulas.
- DHW `ResulNetEnergy`, source split, and solar DHW formulas.
- Lighting/device period formulas.
- Primary energy and CO2 hardcoded coefficients.

Confirmed legacy behaviors:

- Ref1/Ref2 ventilation reuse baseline schedules.
- `Fuel.Fuel1 -> Fuel8` and `Fuel.Fuel8 -> Fuel1` reporting buckets across fuel, primary fuel, and CO2 supply tables (`KD-A009`).
- BGV/DHW is carried by the first zone in building aggregation.
- `PoiterValue` spelling is preserved in scale output.

Confirmed defects or legacy data errors:

- `KD-004`: heating `SumWallDirecrionsHu1` uses north walls eight times.
- `KD-DATA-001`: January sign error in legacy XML for climate zones 1-3.
- `KD-A001`: all total fuel variants add `Fuel1` twice.

Unresolved candidates:

- R4/R5/R8/R9 KD candidates not listed in `validation_known_differences.md`.
- R10 `KD-A002` through `KD-A007`.
- Any candidate requiring live binary parity rather than decompiled source review.

## 17. Oracle implementation roadmap

Recommended order:

1. Freeze provider-mode semantics and debug source labels.
2. Keep existing heating and cooling test-side oracles as module references.
3. Implement a shared `MonthlyDays` and degree-hour package.
4. Implement heating modules in order: R2, R3, R4, R5.
5. Implement cooling R6 with hourly climate provider seams and temporary limitation markers.
6. Implement ventilation R7 with explicit edge-case fixtures before full parity.
7. Implement DHW/BGV R8, separating ordinary DHW, solar DHW, and BGV pumps.
8. Implement lighting/devices R9, including schedule side effects.
9. Implement aggregation R10 last, consuming already-calculated zone outputs.
10. Add parity harnesses only after each oracle can emit full debug CSV rows.

Implementation principles:

- Do not read XML/JSON inside formula code.
- Preserve EECalc naming/spelling in debug columns where it affects traceability.
- Separate input enum from reporting bucket for fuels.
- Keep strict/corrected/current mode decisions explicit.
- Classify every mismatch as formula, data, reporting, aggregation, or unresolved before changing code.

## 18. Debug CSV schema plan

Shared columns:

- `FixtureId`
- `Mode`
- `ZoneId`
- `Month`
- `SourceDoc`
- `DataSource`
- `Variant` (`Ref1`, `Ref2`, `Actual`, `BaseLine`, `ESM`)

Heating monthly CSV:

- `MonthlyDaysWorkDays`
- `MonthlyDaysSaturdays`
- `MonthlyDaysSundays`
- `MonthlyDaysHolydays`
- `Weeks`
- `AvgTemp`
- `Hve`
- `Qve`
- `Hd`
- `Hg`
- `Hu`
- `Htr`
- `Qtr`
- `Qht`
- `Qgn`
- `MetabolicHeat`
- `Gamma`
- `aH`
- `Ni`
- `RawQnd`
- `FinalQnd`

Cooling monthly CSV:

- `Qsol`
- `Qint`
- `Qoccupants`
- `Qgain`
- `QtrCooling`
- `Qinf`
- `Qloss`
- `Hinf`
- `Ac`
- `Eta`
- `QLatentOccupants`
- `QLatentInf`
- `QLatentVent`
- `QcoolRaw`
- `QfreeCooling`
- `QveCooling`
- `QcoolWithInputs`

Ventilation CSV:

- `VentilationKind`
- `Debit`
- `FlowTemperature`
- `RelativeHumidity`
- `SecondRecEfficiency`
- `HeatingAirDifference`
- `powHeating`
- `powCooling`
- `withering`
- `coolingInputs`
- `ResultEnergyForHeating`
- `ResultEnergyForCooling`
- `ResultEnergyForWithering`
- `ResultSourceEnergy`
- `ResultSourceEnergy2`
- `ResultNeededEnergy`

DHW/Solar CSV:

- `Consumption`
- `TempDifference`
- `SunEnergy`
- `ResulNetEnergy`
- `ResultEnergyForHeating`
- `ResultSourceEnergy`
- `ResultSourceEnergy2`
- `ResultNeededEnergy`
- `CollectorsArea`
- `Ht`
- `X`
- `Y`
- `XwithCorrection`
- `F`
- `BGVSunEnergy`
- `BGVPumpsTotal`

Lighting/devices CSV:

- `Group`
- `Period`
- `ByMonths`
- `WeekRegime`
- `MonthPower`
- `DerivedPower`
- `DerivedWorkSchedule`
- `DevicesNeededEnergy`
- `HeatingEta`
- `HeatingInput`
- `CoolingQintContribution`

Aggregation CSV:

- `Category`
- `AbsoluteValue`
- `AreaValue`
- `HeatedArea`
- `TotalHeatedArea`
- `IsFirstBuildingZone`
- `FuelInputEnum`
- `FuelReportBucket`
- `PrimaryCoefficient`
- `CO2Coefficient`
- `PrimaryEnergy`
- `FuelEnergy`
- `EmissionNeeded`
- `EmissionSupply`
- `Vei`
- `GeneralVei`
- `ScalePointer`

## 19. Minimal fixture matrix

| Fixture | Purpose |
| --- | --- |
| Calendar single month | Validate fixed-year `MonthlyDays` and partial period counting. |
| Heating no gains | Validate `Qtr`, `Qve`, `Qht`, zero `Qgn`, and Qnd path. |
| Heating solar only | Validate R4 Fsol and solar-only `Qgn`. |
| Heating occupant only | Validate metabolic contribution to Gamma outside `Qgn`. |
| Heating gamma boundary | Validate `gamma == 0.99`, `1.01`, `0`, negative, NaN-like guards. |
| Transmission north-wall defect | Validate `KD-004` strict vs corrected behavior. |
| Cooling no latent | Validate `Qgain`, `Qloss`, `Ac`, `Eta`, raw cooling. |
| Cooling latent/free cooling | Validate hourly weather, `KD-C005`, `KD-C006`. |
| Ventilation second recovery | Validate `SecondRecEfficiency > 100` split and 3..8 condition. |
| Ventilation shifted hours | Validate `GetDaysHours` 25-hour sequence. |
| DHW no solar | Validate ordinary DHW demand/source conversion. |
| DHW with solar cap | Validate `max(0, ResulNetEnergy - SunEnergy)`. |
| Solar DHW one month | Validate `Ht`, `X`, `Y`, `F`, and `BGVPumpsTotal`. |
| Lighting monthly schedule | Validate `weekRegime` side effect and derived fields. |
| Non-balanced thermal exclusion | Validate non-balanced devices excluded from heating/cooling gains. |
| Fuel bucket inversion | Validate `KD-A009` for fuel, primary fuel, and CO2 supply tables. |
| Total fuel duplicate | Validate `KD-A001` strict behavior. |
| Multi-zone BGV | Validate first-zone BGV vs sum-all-zone categories. |
| VEI fuels | Validate Fuel1 >100 efficiency, Fuel6/Fuel7 renewable handling. |
| Referent scale | Validate Ref1/Ref2-derived scale thresholds. |

## 20. Open questions / items requiring parity validation

Unresolved items:

- No separate `analysis/r1*` or `analysis/r2*` docs exist in this workspace; R1/R2 are represented through heating oracle reports and source binding audit.
- R4 candidate items need binary/parity confirmation before promotion to global KD IDs.
- R5 candidates need parity confirmation, especially Gamma edge handling across all variants.
- R6 confirmed quirks are implemented in the cooling oracle, but production parity has not been run.
- R7 risky edge cases require fixtures before ventilation oracle implementation.
- R8 CO2 helper fuel-field mismatches need separate CO2-focused validation.
- R9 monthly guard differences and savings distribution need fixture confirmation.
- R10 `KD-A002` through `KD-A007` remain candidates unless separately ILSpy/parity confirmed.
- `CurrentOrdinance` hourly climate data needs an authoritative hourly source before R6/R7 current-mode validation.
- `DefaultSunParams.xml` January sign behavior should be audited separately if solar-current modes are required.
- Final displayed energy class label is outside the decompiled final aggregation core; only scale bands and pointers are documented here.

## Source docs used

- `analysis/heating_engine_complete_oracle_report.md`
- `analysis/cooling_complete_oracle_report.md`
- `analysis/r3_qtr_htr_reverse_engineering.md`
- `analysis/r4_qgn_gains_reverse_engineering.md`
- `analysis/r5_gamma_ni_reverse_engineering.md`
- `analysis/r6_cooling_reverse_engineering.md`
- `analysis/r7_ventilation_reverse_engineering.md`
- `analysis/r7_ventilation_edge_cases_for_oracle.md`
- `analysis/r8_dhw_bgv_reverse_engineering.md`
- `analysis/r9_lighting_devices_reverse_engineering.md`
- `analysis/r10_aggregation_primary_co2_class_reverse_engineering.md`
- `analysis/reference-data/source_binding_audit_heating_cooling_ventilation.md`
- `analysis/reference-data/climate_provider_review.md`
- `analysis/validation_known_differences.md`
- `analysis/ilspy_verified_findings.md`
