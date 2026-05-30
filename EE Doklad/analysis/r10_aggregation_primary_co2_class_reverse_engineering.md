# R10 Aggregation / Primary Energy / CO2 / Energy Class Reverse Engineering

Scope: final EECalc result aggregation layer only. No oracle, tests, parity run, production-code change, or provider rewiring is included.

Primary source:

- `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs`
- `reference/eecalc-decompiled/EECalcCore.Calculations.BuildingTypesManager.cs`

Cross-references:

- `analysis/docs/06_building_aggregation.md`
- `analysis/r4_qgn_gains_reverse_engineering.md`
- `analysis/r5_gamma_ni_reverse_engineering.md`
- `analysis/r6_cooling_reverse_engineering.md`
- `analysis/r7_ventilation_reverse_engineering.md`
- `analysis/r7_ventilation_edge_cases_for_oracle.md`
- `analysis/r8_dhw_bgv_reverse_engineering.md`
- `analysis/r9_lighting_devices_reverse_engineering.md`
- `analysis/validation_known_differences.md`
- `analysis/reference-data/climate_provider_review.md`

## 1. Summary

EECalc has a two-stage final result model:

1. Zone calculations create per-zone needed/source/net/primary/fuel/CO2 tables.
2. Building calculations aggregate selected zone rows and then run final table-specific passes.

The final building result is not a single clean sum. Important EECalc-compatible behaviors:

- Heating and heating ventilation aggregate only zones where `HasHeating`.
- Cooling and cooling ventilation aggregate only zones where `HasCooling`.
- Lights, devices, fans/pumps, and `Other` sum all zones.
- BGV/DHW is treated as a building-level value carried by the first zone.
- Solar `BGVPumpsTotal` is also taken from the first zone.
- Several fields named `Area` are temporary absolute accumulators before later division by total heated area.
- `Fuel.Fuel1` is used in formula code for direct electrical loads, but reporting bucket helpers store it under `Fuel8`.
- `Fuel.Fuel8` input maps back to reporting bucket `Fuel1`.
- Primary energy and CO2 coefficients are hardcoded.
- Energy scale values are set from `BuildingTypesManager` and the final primary-energy table; the decompiled EECalc core sets scale bands and pointer values, not a separate class enum label.

## 2. Final execution flow

Building flow in `BuildingCalculations`:

```text
BuildingCalculations(buildingBalanceResult, calcInput, zoneBalanceResult)
  -> GetBuildingData
  -> GetConditionedArea
  -> ClearNeededVEIenergy
  -> UpdateRefsState
  -> UpdateActualState
  -> UpdateBaseLineState
  -> UpdateEsmState
  -> CalculateTotalsNeededEnergyTable(isBGVused: true)
  -> ClearFuelCells
  -> ClearNetEnergy
  -> ClearNetEnergyWithoutInputs
  -> ClearPrimaryEnergy
  -> ClearPrimaryEnergyFuelTableValues
  -> foreach BuildingZone:
       -> CalculateNetEnergyByTechnologiesBuilding
       -> CalculateNetWithoutInputsEnergyByTechnologies
       -> CalculatePrimaryEnergyByTechnologies(isBGVused: true, totalHeatedArea, isFirstBuildingZone)
       -> GetPrimaryFuelTypeAndValues(isBGVused: true, totalHeatedArea, isFirstBuildingZone)
       -> GetFuelTypeAndValues(isBGVused: true, zone.HeatedArea, totalHeatedArea, isFirstBuildingZone)
  -> add first-zone solar BGVPumpsTotal to FuelEnergyTable.Fuel8 Actual/BaseLine/ESMArea
  -> SetFuelValue(totalHeatedArea)
  -> CalculateNetEnergyPerArea
  -> CalculateNetWithoutInputsEnergyByTechnologiesPerArea
  -> CalculatePrimaryEnergyPerArea(totalHeatedArea, isBGVused: true)
  -> CalculatePrimaryFuelTypeAndValuesPerArea(totalHeatedArea)
  -> BuildingCO2Calculations
  -> CalculateTotalFuelEnergy
  -> CalculatePrimaryEnergyFuelTotal
  -> CalculatePrimaryTotalEnergy
  -> CalculateBuildingPowerEnergy
  -> CalculateTotalVei(isBGVused: true)
  -> SetScaleValues
```

Zone flow in `ZoneCalculations`:

```text
ZoneCalculations(zoneBalanceResult, calcInput, zone)
  -> CalculateZonePowerEnergy
  -> ZoneCO2Calculations(isBGVused: false)
  -> ClearFuelCells
  -> ClearNeededVEIenergy
  -> GetFuelTypeAndValues(isBGVused: false, area: 1, totalArea: 1)
  -> CalculateTotalFuelEnergy
  -> set NeededEnergyTable.ConditionedArea = zone.HeatedArea
  -> set NetEnergyTable.ConditionedArea = zone.HeatedArea
  -> CalculateTotalsNeededEnergyTable(isBGVused: false)
  -> ClearNetEnergy
  -> ClearNetEnergyWithoutInputs
  -> CalculateNetEnergyByTechnologies
  -> CalculateNetWithoutInputsEnergyByTechnologies
  -> ClearPrimaryEnergy
  -> CalculatePrimaryEnergyByTechnologies(isBGVused: false)
  -> CalculatePrimaryEnergyPerArea(zone.HeatedArea, isBGVused: false)
  -> ClearPrimaryEnergyFuelTableValues
  -> GetPrimaryFuelTypeAndValues(isBGVused: false, totalArea: 1)
  -> CalculatePrimaryFuelTypeAndValuesPerArea(zone.HeatedArea)
  -> CalculatePrimaryEnergyFuelTotal
  -> CalculatePrimaryTotalEnergy
  -> CalculateTotalVei(isBGVused: false)
```

## 3. Full call graph

Final aggregation graph:

```text
BuildingCalculations
  -> GetBuildingData
  -> GetConditionedArea
  -> UpdateRefsState
  -> UpdateActualState
  -> UpdateBaseLineState
  -> UpdateEsmState
  -> CalculateTotalsNeededEnergyTable
      -> CalculateTotalActual
      -> CalculateTotalActualYearly
      -> CalculateTotalBaseLine
      -> CalculateTotalBaseLineYearly
      -> CalculateTotalEsm
      -> CalculateTotalEsmYearly
      -> CalculateTotalRefs
      -> CalculateTotalRefsYearly
  -> CalculateNetEnergyByTechnologiesBuilding
  -> CalculateNetWithoutInputsEnergyByTechnologies
  -> CalculatePrimaryEnergyByTechnologies
      -> GetPrimaryEnergyCoeficient
  -> GetPrimaryFuelTypeAndValues
      -> GetPrimaryFuelTypeRef1
      -> GetPrimaryFuelTypeRef2
      -> GetPrimaryFuelType
      -> GetPrimaryFuelTypeBaseLine
      -> GetPrimaryFuelTypeEsm
  -> GetFuelTypeAndValues
      -> GetFuelTypeRef1
      -> GetFuelTypeRef2
      -> GetFuelType
      -> GetFuelTypeBaseLine
      -> GetFuelTypeEsm
      -> GetVeiHeating
      -> GetVeiHeatVentilation
      -> GetVeiBGV
      -> CalculateElectricityVEI
  -> SetFuelValue
  -> CalculateNetEnergyPerArea
  -> CalculateNetWithoutInputsEnergyByTechnologiesPerArea
  -> CalculatePrimaryEnergyPerArea
  -> CalculatePrimaryFuelTypeAndValuesPerArea
  -> BuildingCO2Calculations
      -> ClearValuesCO2
      -> ClearFuelCellsCO2
      -> CalculateCO2Emissions
          -> CalculateCO2EmissionsRef1
          -> CalculateCO2EmissionsRef2
          -> CalculateCO2EmissionsActual
          -> CalculateCO2EmissionsBaseLine
          -> CalculateCO2EmissionsESM
          -> GetEkoCoeficient
      -> Co2GetFuelTypesBuilding
          -> Co2EnergyCalculationBuildingRef1
          -> Co2EnergyCalculationBuildingRef2
          -> Co2EnergyCalculationBuildingActual
          -> Co2EnergyCalculationBuildingBaseLine
          -> Co2EnergyCalculationBuildingESM
          -> GetFuelTypeCo2Ref1/Ref2/Actual/BaseLine/ESM
      -> Co2CalculateEmissionEnergySupplyBuilding
      -> Co2EnergyCalculateTotal
      -> CalculateSavings(EmissionNeededEnergyTable)
      -> CalculateFuelSavings(EmissionEnergySupplyTable)
  -> CalculateTotalFuelEnergy
  -> CalculatePrimaryEnergyFuelTotal
  -> CalculatePrimaryTotalEnergy
  -> CalculateBuildingPowerEnergy
  -> CalculateTotalVei
  -> SetScaleValues
      -> BuildingTypesManager.GetClimateZoneParams
      -> SetScaleType
```

## 4. Zone-to-building aggregation

Total heated area:

```text
TotalHeatedArea =
  sum(zone.Heating.Area.HeatedArea)
  - sum(zone.Heating.Area.OtherArea)
```

Total volume:

```text
TotalVolume =
  sum(zone.Heating.Area.HeatedVolume)
  - sum(zone.Heating.Area.OtherVolume)
```

Envelope element totals sum all zones separately for `Actual` and `Esm`.

Needed-energy aggregation:

| Category | Building aggregation |
| --- | --- |
| `Heating` | Sum zones where `HasHeating`; area row divides by total building heated area. |
| `HeatingVentilation` | Sum zones where `HasHeating`; area row divides by total building heated area. |
| `Cooling` | Sum zones where `HasCooling`; area row divides by total building heated area. |
| `CoolingVentilation` | Sum zones where `HasCooling`; area row divides by total building heated area. |
| `BGV` | First zone only for all variants. |
| `BGVPumps` Ref1/Ref2 | Sum zone `BGVPumps.Ref*`, then add first-zone Actual solar `BGVPumpsTotal`. |
| `BGVPumps` Actual/BaseLine/ESM | First-zone `BGVPumps.*Area` multiplied by total area, then add matching first-zone solar `BGVPumpsTotal`. |
| `FansAndPumps` | Sum all zones. |
| `Lights` | Sum all zones. |
| `HeatAffectingDevices` | Sum all zones. |
| `NonHeatAffectingDevices` | Sum all zones. |
| `Other` | Sum all zones. |

First-zone vs sum-all-zones behavior is central for R10. BGV and solar BGV pump values must not be blindly summed across zones in an EECalc-compatible oracle.

## 5. Needed energy table formulas

`CalculateTotalsNeededEnergyTable` computes category totals from the already-filled category rows.

For each variant:

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

Area-normalized total rows use the same category list with `*Area` properties.

`isBGVused` behavior:

- Building calls pass `true`; BGV and BGVPumps remain in totals.
- Zone calls pass `false`; BGV and BGVPumps rows are zeroed before zone totals.

NaN handling is variant-specific:

- `CalculateTotalActual` and `CalculateTotalActualYearly` use `CheckForNaN` on each component.
- `CalculateTotalRefsYearly` uses `CheckForNaN` for Ref1/Ref2 area rows.
- `CalculateTotalRefs`, `CalculateTotalBaseLine`, `CalculateTotalBaseLineYearly`, `CalculateTotalEsm`, and `CalculateTotalEsmYearly` use direct sums in the decompiled source.

## 6. Primary energy table formulas

Technology primary energy is calculated by `CalculatePrimaryEnergyByTechnologies`.

For fuel-backed two-source technologies:

```text
PrimaryTechnology =
  GetPrimaryEnergyCoeficient(Fuel1, ResultSourceEnergy1) * area
  + GetPrimaryEnergyCoeficient(Fuel2, ResultSourceEnergy2) * area
```

Area multiplier:

- Heating/cooling/ventilation use the zone heated area.
- BGV/DHW uses total building heated area and is guarded by `isFirstBuildingZone`.

Direct electrical loads use `Fuel.Fuel1`:

```text
PrimaryFansAndPumps = GetPrimaryEnergyCoeficient(Fuel.Fuel1, CoolNeededEnergy + PumpNeededEnergy) * heatedArea
PrimaryLights = GetPrimaryEnergyCoeficient(Fuel.Fuel1, Lights.General.DevicesNeededEnergy) * heatedArea
PrimaryHeatAffectingDevices = GetPrimaryEnergyCoeficient(Fuel.Fuel1, BalancedDevices.General.DevicesNeededEnergy) * heatedArea
PrimaryNonHeatAffectingDevices = GetPrimaryEnergyCoeficient(Fuel.Fuel1, NonBalancedDevices.General.DevicesNeededEnergy) * heatedArea
PrimaryOther = GetPrimaryEnergyCoeficient(Fuel.Fuel1, FansAndPumps.OtherResultCooling) * heatedArea
```

BGV pumps have a special direct table assignment inside the first-zone BGV block:

```text
PrimaryEnergyTable.BGVPumps.Ref1 = NeededEnergyTable.BGVPumps.Ref1 * 3.0
PrimaryEnergyTable.BGVPumps.Ref2 = NeededEnergyTable.BGVPumps.Ref2 * 3.0
PrimaryEnergyTable.BGVPumps.Actual = NeededEnergyTable.BGVPumps.Actual * 3.0
PrimaryEnergyTable.BGVPumps.BaseLine = NeededEnergyTable.BGVPumps.BaseLine * 3.0
PrimaryEnergyTable.BGVPumps.ESM = NeededEnergyTable.BGVPumps.ESM * 3.0
```

Primary-energy coefficients:

| Input fuel enum | Coefficient |
| --- | ---: |
| `Fuel.Fuel1` | 3.0 |
| `Fuel.Fuel2` | 1.1 |
| `Fuel.Fuel3` | 1.1 |
| `Fuel.Fuel4` | 1.2 |
| `Fuel.Fuel5` | 1.2 |
| `Fuel.Fuel6` | 1.05 |
| `Fuel.Fuel7` | 1.25 |
| `Fuel.Fuel8` | 1.1 |
| `Fuel.Fuel9` | 1.3 |
| `Fuel.Fuel10` | 1.1 |
| `Fuel.Fuel11` | 1.2 |

`GetPrimaryEnergyCoeficient` returns `quantity * coefficient`, with NaN/infinity quantity coerced to `0`.

`CalculatePrimaryEnergyPerArea` divides accumulated primary energy by the area argument and sets category savings as:

```text
Savings = BaseLine - ESM
```

## 7. Primary energy fuel table mapping

`GetPrimaryFuelTypeAndValues` fills `PrimaryEnergyFuelTable` by reporting fuel bucket.

For source technologies it receives the actual source fuel enum. For lights, devices, fans, `Other`, and BGVPumps it passes `Fuel.Fuel1`.

Mapping in `GetPrimaryFuelType*`:

| Input fuel enum | Reporting bucket | Primary factor |
| --- | --- | ---: |
| `Fuel.Fuel1` | `PrimaryEnergyFuelTable.Fuel8` | 3.0 |
| `Fuel.Fuel2` | `PrimaryEnergyFuelTable.Fuel2` | 1.1 |
| `Fuel.Fuel3` | `PrimaryEnergyFuelTable.Fuel3` | 1.1 |
| `Fuel.Fuel4` | `PrimaryEnergyFuelTable.Fuel4` | 1.2 |
| `Fuel.Fuel5` | `PrimaryEnergyFuelTable.Fuel5` | 1.2 |
| `Fuel.Fuel6` | `PrimaryEnergyFuelTable.Fuel6` | 1.05 |
| `Fuel.Fuel7` | `PrimaryEnergyFuelTable.Fuel7` | 1.25 |
| `Fuel.Fuel8` | `PrimaryEnergyFuelTable.Fuel1` | 1.1 |
| `Fuel.Fuel9` | `PrimaryEnergyFuelTable.Fuel9` | 1.3 |
| `Fuel.Fuel10` | `PrimaryEnergyFuelTable.Fuel10` | 1.1 |
| `Fuel.Fuel11` | `PrimaryEnergyFuelTable.Fuel11` | 1.2 |

The helper stores absolute values first:

```text
PrimaryEnergyFuelTable.<bucket>.<variant> += quantity * area * coefficient
```

Then `CalculatePrimaryFuelTypeAndValuesPerArea` divides all buckets by total heated area and sets fuel savings:

```text
Fuel.Savings = Fuel.BaseLine - Fuel.ESM
```

`CalculatePrimaryEnergyFuelTotal` sums `Fuel1` through `Fuel11` for Ref1, Ref2, Actual, BaseLine, ESM, and Savings.

## 8. CO2 needed/source emission formulas

EECalc writes two emission tables:

- `EmissionNeededEnergyTable`: technology/category emissions.
- `EmissionEnergySupplyTable`: reporting-fuel-bucket emissions.

Technology emissions use `CalculateCO2Emissions*`:

```text
EmissionNeededEnergyTable.Category.Variant +=
  (GetEkoCoeficient(Fuel1, SourceEnergy1)
   + GetEkoCoeficient(Fuel2, SourceEnergy2))
  * heatedArea / 1000000
```

Direct electrical categories use `Fuel.Fuel1`:

```text
EmissionNeededEnergyTable.Lights += GetEkoCoeficient(Fuel.Fuel1, DevicesNeededEnergy) * heatedArea / 1000000
```

CO2 coefficients in `GetEkoCoeficient`:

| Input fuel enum | Coefficient |
| --- | ---: |
| `Fuel.Fuel1` | 819 |
| `Fuel.Fuel2` | 202 |
| `Fuel.Fuel3` | 227 |
| `Fuel.Fuel4` | 341 |
| `Fuel.Fuel5` | 364 |
| `Fuel.Fuel6` | 43 |
| `Fuel.Fuel7` | 351 |
| `Fuel.Fuel8` | 267 |
| `Fuel.Fuel9` | 290 |
| `Fuel.Fuel10` | 279 |
| `Fuel.Fuel11` | 354 |

`GetEkoCoeficient` returns `quantity * coefficient`, with NaN/infinity quantity coerced to `0`.

Emission supply table flow:

```text
Co2GetFuelTypesBuilding
  -> GetFuelTypeCo2* accumulates source energy / 1000 by reporting bucket
Co2CalculateEmissionEnergySupplyBuilding
  -> multiply each bucket by its CO2 coefficient / 1000
Co2EnergyCalculateTotal
  -> sum Fuel1..Fuel11
```

Reporting-bucket CO2 coefficients after the Fuel1/Fuel8 inversion:

| Reporting bucket | Coefficient applied |
| --- | ---: |
| `Fuel1` | 267 |
| `Fuel2` | 202 |
| `Fuel3` | 227 |
| `Fuel4` | 341 |
| `Fuel5` | 364 |
| `Fuel6` | 43 |
| `Fuel7` | 351 |
| `Fuel8` | 819 |
| `Fuel9` | 290 |
| `Fuel10` | 279 |
| `Fuel11` | 354 |

`BuildingCO2Calculations` adds solar BGV pump emissions after the zone loop:

```text
BGVPumps.Ref1 += FirstZone.Actual.BGVPumpsTotal * 819 / 1000000
BGVPumps.Ref2 += FirstZone.Actual.BGVPumpsTotal * 819 / 1000000
BGVPumps.Actual += FirstZone.Actual.BGVPumpsTotal * 819 / 1000000
BGVPumps.BaseLine += FirstZone.BaseLine.BGVPumpsTotal * 819 / 1000000
BGVPumps.ESM += FirstZone.ESM.BGVPumpsTotal * 819 / 1000000
```

## 9. Fuel mapping matrix

The final layer has three related mappings.

### Primary technology coefficient

`GetPrimaryEnergyCoeficient` keeps the input enum identity and returns `quantity * coefficient`.

### Primary/fuel reporting table

`GetPrimaryFuelType*` stores source energy in reporting buckets:

| Input enum | Reporting bucket |
| --- | --- |
| `Fuel1` | `Fuel8` |
| `Fuel2` | `Fuel2` |
| `Fuel3` | `Fuel3` |
| `Fuel4` | `Fuel4` |
| `Fuel5` | `Fuel5` |
| `Fuel6` | `Fuel6` |
| `Fuel7` | `Fuel7` |
| `Fuel8` | `Fuel1` |
| `Fuel9` | `Fuel9` |
| `Fuel10` | `Fuel10` |
| `Fuel11` | `Fuel11` |

### Fuel-energy and CO2 supply reporting tables

`GetFuelType*` and `GetFuelTypeCo2*` use the same reporting-bucket inversion:

| Input enum | Reporting bucket |
| --- | --- |
| `Fuel1` | `Fuel8` |
| `Fuel8` | `Fuel1` |
| Other fuel enums | Same-number bucket |

This has been verified in ILSpy for `FuelEnergyTable`, `PrimaryEnergyFuelTable`, and `EmissionEnergySupplyTable` across Ref1, Ref2, Actual, BaseLine, and ESM. It is confirmed legacy reporting-bucket mapping, not a formula defect or calculation error.

## 10. Electricity mapping: Fuel1 -> Fuel8 behavior

Formula code treats direct electrical demand as `Fuel.Fuel1` in these paths:

- `FansAndPumps`
- `Lights`
- `HeatAffectingDevices`
- `NonHeatAffectingDevices`
- `Other`
- normal `BGVPumps`
- solar `BGVPumpsTotal` direct additions

Reporting behavior:

- `GetFuelType*` maps `Fuel.Fuel1` to `FuelEnergyTable.Fuel8`.
- `GetPrimaryFuelType*` maps `Fuel.Fuel1` to `PrimaryEnergyFuelTable.Fuel8` and applies primary factor `3.0`.
- `GetPrimaryFuelType*` maps `Fuel.Fuel8` to `PrimaryEnergyFuelTable.Fuel1` and applies primary factor `1.1`.
- `GetFuelTypeCo2*` maps `Fuel.Fuel1` to `EmissionEnergySupplyTable.Fuel8`.
- `GetFuelTypeCo2*` maps `Fuel.Fuel8` to `EmissionEnergySupplyTable.Fuel1`.
- `GetEkoCoeficient(Fuel.Fuel1, ...)` applies CO2 factor `819`.

Therefore, an EECalc-compatible oracle should expose both columns:

```text
FuelInputEnum = Fuel1
FuelReportBucket = Fuel8
```

This behavior matches R9 `KD-LD011`.

## 11. BGV and BGVPumps special handling

BGV/DHW is building-level but stored on the first zone.

Needed energy:

- `NeededEnergyTable.BGV.*` is copied from `calcInput.BuildingZones.First().ZoneResults`.
- Ref1/Ref2 `BGVPumps` sum normal zone values and add first-zone Actual solar `BGVPumpsTotal`.
- Actual/BaseLine/ESM `BGVPumps` use first-zone `BGVPumps.*Area` multiplied by total area and add matching first-zone solar `BGVPumpsTotal`.

Primary energy:

- Ordinary BGV source energy is handled only when `isBGVused && isFirstBuildingZone`.
- BGV source energy uses total heated area, not individual zone area.
- BGVPumps primary table directly sets `NeededEnergyTable.BGVPumps.* * 3.0` in the same first-zone block.
- Primary fuel table uses `Fuel.Fuel1` and `NeededEnergyTable.BGVPumps.*Area * totalArea`, also guarded by `isFirstBuildingZone`.

Fuel energy:

- Ordinary BGV fuel-source rows are added only when `isBGVused && isFirstBuildingZone`.
- Normal BGVPumps are added to `FuelEnergyTable.Fuel8.*Area` inside `GetFuelTypeAndValues`.
- Solar BGV pump energy is added after the building zone loop only for Actual/BaseLine/ESM:

```text
Fuel8.ActualArea += FirstZone.SunEnergyCalculations.Actual.SunEnergyResTable.BGVPumpsTotal
Fuel8.BaseLineArea += FirstZone.SunEnergyCalculations.BaseLine.SunEnergyResTable.BGVPumpsTotal
Fuel8.ESMArea += FirstZone.SunEnergyCalculations.ESM.SunEnergyResTable.BGVPumpsTotal
```

CO2:

- Normal BGVPumps use `Fuel.Fuel1`.
- Solar `BGVPumpsTotal` is added directly to `EmissionNeededEnergyTable.BGVPumps` with coefficient `819 / 1000000`.
- Ref1 and Ref2 use the Actual solar `BGVPumpsTotal` in this direct CO2 addition.

## 12. Solar/DHW hooks

Solar/DHW calculation itself belongs to R8 and uses `SunEnergyCalculations`, not the R10 final aggregation formulas.

R10 hooks:

- Ordinary BGV net/source rows feed needed, primary, fuel, and emission tables from the first zone.
- `SunEnergyResTable.BGVPumpsTotal` enters building aggregation separately from ordinary hot water source energy.
- `NeededEnergyTable.BGVPumps` includes solar pump energy before primary and CO2 table passes.
- `FuelEnergyTable.Fuel8` receives solar pump energy after the zone loop for Actual/BaseLine/ESM.
- `EmissionNeededEnergyTable.BGVPumps` receives solar pump energy inside `BuildingCO2Calculations`.
- `NeededEnergyTable.BGV.GeneralVEI` receives solar useful energy through:

```text
Min(SunEnergyCalculations.ESM.SunEnergyResTable.BGVSunEnergy,
    HotWaterCalculations.ResulNetEnergyESM) * totalArea
```

## 13. VEI / renewable contribution logic

VEI is written from ESM source-energy paths only in `GetFuelTypeAndValues`.

Heating:

```text
GetVeiHeating(Fuel1ESM, GeneratorHeatEfficiency1ESM, ResultSourceEnergyESM, area)
GetVeiHeating(Fuel2ESM, GeneratorHeatEfficiency2ESM, ResultSourceEnergy2ESM, area)
```

Heating ventilation:

```text
GetVeiHeatVentilation(Fuel1ESM, GeneratorHeatEfficiency1ESM, ResultSourceEnergyESM, heatedArea)
GetVeiHeatVentilation(Fuel2ESM, GeneratorHeatEfficiency2ESM, ResultSourceEnergy2ESM, heatedArea)
```

BGV:

```text
GetVeiBGV(Fuel1ESM, GeneratorHeatEfficiency1ESM, ResultSourceEnergyESM, totalArea)
GetVeiBGV(Fuel2ESM, GeneratorHeatEfficiency2ESM, ResultSourceEnergy2ESM, totalArea)
BGV.GeneralVEI += min(SunEnergyESM, ResulNetEnergyESM) * totalArea
```

Fuel-specific VEI behavior:

| Fuel | VEI behavior |
| --- | --- |
| `Fuel1` | Adds to `GeneralVEI` only when efficiency > 100: `quantity * ((efficiency - 100) / 100) * area`. |
| `Fuel6` | Adds `quantity * area` to both `VEI` and `GeneralVEI`. |
| `Fuel7` | Adds `quantity * area` to both `VEI` and `GeneralVEI`. |
| Other fuels | No VEI contribution. |

`CalculateTotalVei` sums category `VEI` and `GeneralVEI`. If `isBGVused` is false, BGV and BGVPumps VEI fields are zeroed first.

No cooling VEI hook was found in the final aggregation layer.

## 14. Total fuel energy calculation

`FuelEnergyTable` stores final-energy/source-energy values by reporting fuel bucket.

`GetFuelTypeAndValues` first accumulates absolute values in `FuelEnergyTable.<Fuel>.<Variant>Area`.

`SetFuelValue(buildingBalanceResult, totalHeatedArea)` divides all fuel `*Area` rows by total heated area:

```text
FuelEnergyTable.FuelN.ActualArea = FuelEnergyTable.FuelN.ActualArea / totalHeatedArea
```

`CalculateTotalFuelEnergy` then computes area-normalized totals:

```text
FuelEnergyTable.Total.VariantArea =
  Fuel1.VariantArea
  + Fuel1.VariantArea
  + Fuel2.VariantArea
  + Fuel3.VariantArea
  + Fuel4.VariantArea
  + Fuel5.VariantArea
  + Fuel6.VariantArea
  + Fuel7.VariantArea
  + Fuel8.VariantArea
  + Fuel9.VariantArea
  + Fuel10.VariantArea
  + Fuel11.VariantArea
```

`Fuel1` is included twice in all variants. ILSpy verification confirms this legacy aggregation defect in:

- `CalculateTotalFuelRef1`
- `CalculateTotalFuelRef2`
- `CalculateTotalFuelActual`
- `CalculateTotalFuelBaseLine`
- `CalculateTotalFuelESM`

Strict aggregation parity must preserve it. Corrected mode may fix it.

## 15. Energy class / scale calculation

`SetScaleValues`:

```text
Scale climateZoneParams =
  BuildingTypesManager.GetClimateZoneParams(calcInput.General.InvestigationMethod)
SetScaleType(climateZoneParams, calcInput.General.BuildingResults)
```

`BuildingTypesManager` loads embedded XML from the EECalcCore assembly manifest and deserializes `BuildingCategories`. It returns:

```text
Parameters.ScaleType.Single(z => z.Type == (InvestigationType)investigationType)
```

`SetScaleType` writes:

```text
BuildingScaleType.PoiterValue = (int)PrimaryEnergyTable.Total.ESM
BuildingScaleType.PoiterValueBaseLine = (int)PrimaryEnergyTable.Total.BaseLine
```

If `investigationMethod.Type == InvestigationType.ReferentValues`, thresholds are derived from final primary Ref1/Ref2:

```text
ref1 = PrimaryEnergyTable.Total.Ref1
ref2 = PrimaryEnergyTable.Total.Ref2

Aplus.Max = int(0.25 * ref2)
A.Max = int(0.5 * ref2)
A.Min = int(0.25 * ref2)
B.Max = int(ref2)
B.Min = int(0.5 * ref2 + 1)
C.Max = int(0.5 * (ref2 + ref1))
C.Min = int(ref2 + 1)
D.Max = int(ref1)
D.Min = int(0.5 * (ref2 + ref1) + 1)
E.Max = int(1.25 * ref1)
E.Min = int(ref1 + 1)
F.Max = int(1.5 * ref1)
F.Min = int(1.25 * ref1 + 1)
G.Max = int(1.5 * ref1)
G.Min = int(1.5 * ref1)
```

Otherwise, thresholds are copied from the embedded `Scale` rows:

```text
BuildingScaleType.Aplus.Max = investigationMethod.Aplus.EPmax
...
BuildingScaleType.G.Min = investigationMethod.G.EPmin
```

No separate final `CalculateClass` assignment was found in the decompiled EECalc final layer. The core sets scale thresholds and pointer values; UI/reporting can infer the displayed class from those fields.

## 16. Ref1 / Ref2 / Actual / BaseLine / ESM differences

Ref1/Ref2:

- Needed energy includes reference rows for all major categories.
- Reference ventilation schedules are expected to reuse baseline schedules, per R7 reclassification.
- BGV is first-zone only.
- Ref1/Ref2 BGVPumps needed-energy aggregation adds Actual solar `BGVPumpsTotal`.
- Primary energy and primary fuel tables include Ref1/Ref2.
- CO2 needed and supply tables include Ref1/Ref2.
- Fuel energy table includes Ref1/Ref2 area rows.

Actual:

- Uses actual zone rows, heating/cooling flags, and actual source fuels/efficiencies.
- BGV is first-zone only.
- BGVPumps solar addition uses Actual `BGVPumpsTotal`.

BaseLine:

- Uses baseline rows and is the savings comparison base.
- BGV is first-zone only.
- BGVPumps solar addition uses BaseLine `BGVPumpsTotal`.
- Savings are generally `BaseLine - ESM`.

ESM:

- Uses ESM rows.
- VEI hooks are based on ESM source-energy paths.
- BGVPumps solar addition uses ESM `BGVPumpsTotal`.

Tables with Ref1/Ref2:

- `NeededEnergyTable`
- `NetEnergyTable`
- `NoInputsNetEnergyTable`
- `PrimaryEnergyTable`
- `PrimaryEnergyFuelTable`
- `FuelEnergyTable`
- `EmissionNeededEnergyTable`
- `EmissionEnergySupplyTable`

Tables/savings that are Actual/BaseLine/ESM-centered:

- `PowerBudgetTable` final building power rows are Actual/BaseLine/ESM.
- Most savings fields are `BaseLine - ESM`.
- `BuildingScaleType.PoiterValue` uses ESM; `PoiterValueBaseLine` uses BaseLine.
- VEI totals are not variant-split; they are derived from ESM paths and solar ESM contribution.

## 17. Known quirks / KD candidates

Confirmed cross-phase behaviors relevant to R10:

- `KD-LD011`: direct electrical loads pass `Fuel.Fuel1` but report under `Fuel8`.
- `KD-HW009`: Ref1/Ref2 BGVPumps aggregation uses Actual solar `BGVPumpsTotal`.
- `KD-HW010`: solar `BGVPumpsTotal` is added to `Fuel8.ActualArea/BaseLineArea/ESMArea`, but not to Ref1/Ref2 in the same post-zone hook.
- `KD-HW013`: several building aggregation paths use first-zone BGV values instead of summing all zones.

R10 KD candidates:

- `KD-A001`: confirmed legacy aggregation defect. `CalculateTotalFuelRef1`, `CalculateTotalFuelRef2`, `CalculateTotalFuelActual`, `CalculateTotalFuelBaseLine`, and `CalculateTotalFuelESM` double-count `FuelEnergyTable.Fuel1.*Area`.
- `KD-A002`: `UpdateActualState`, `UpdateBaseLineState`, and `UpdateEsmState` compute building `BGVPumps.*` from first-zone `BGVPumps.*Area * totalArea`, not a sum of all zone absolute BGVPumps.
- `KD-A003`: `CalculatePrimaryEnergyPerArea` initially totals `PrimaryEnergyTable.Devices`, while later `CalculatePrimaryTotalEnergy` overwrites totals with `PrimaryEnergyTable.Other`.
- `KD-A004`: `BuildingCO2Calculations` adds Actual solar `BGVPumpsTotal` to Ref1 and Ref2 BGVPumps emissions.
- `KD-A005`: Zone net-energy Ref2 heating ventilation absolute value uses `ResultEnergyForHeatingRef1` in the decompiled zone-level path; the area row uses Ref2. This is inherited by any downstream zone result consumer.
- `KD-A006`: Some CO2 fuel-supply hot-water variant calls use reference fuel fields for BaseLine/ESM source-energy rows. Building CO2 path uses matching Ref1/Ref2 for references, but BaseLine/ESM should be validated separately before oracle implementation.
- `KD-A007`: `GetPrimaryFuelTypeRef2` lacks the same explicit NaN/infinity guard pattern seen in the other primary fuel helper variants.
- `KD-A008`: `SetScaleType` writes `PoiterValue` / `PoiterValueBaseLine` spelling as decompiled.
- `KD-A009`: confirmed legacy reporting-bucket mapping. `Fuel.Fuel1` writes to reporting bucket `Fuel8`, `Fuel.Fuel8` writes to reporting bucket `Fuel1`, and all other fuel enum values map to same-number reporting buckets across `FuelEnergyTable`, `PrimaryEnergyFuelTable`, and `EmissionEnergySupplyTable` for Ref1, Ref2, Actual, BaseLine, and ESM. This is not a formula defect or calculation error.

## 18. Required inputs

Building structure:

- `calcInput.BuildingZones`
- `zone.Heating.Area.HeatedArea`
- `zone.Heating.Area.OtherArea`
- `zone.Heating.Area.HeatedVolume`
- `zone.Heating.Area.OtherVolume`
- `zone.Heating.Area.ZoneAreaElements.*`
- `zone.HasHeating`
- `zone.HasCooling`

Per-zone result tables:

- `zone.ZoneResults.NeededEnergyTable.*`
- `zone.ZoneResults.NetEnergyTable.*`
- `zone.ZoneResults.NoInputsNetEnergyTable.*`

Technology source energy:

- Heating `CalculationData.ResultSourceEnergy*`, `ResultSourceEnergy2*`, `Fuel1*`, `Fuel2*`
- Cooling `CalculationData.ResultSourceEnergy*`, `ResultSourceEnergy2*`, `Fuel1*`, `Fuel2*`
- Heating ventilation `CalculationData.ResultSourceEnergy*`, `ResultSourceEnergy2*`, `Fuel1*`, `Fuel2*`
- Cooling ventilation `CalculationData.ResultSourceEnergy*`, `ResultSourceEnergy2*`, `Fuel1*`, `Fuel2*`
- Hot water `HotWaterCalculations.ResultSourceEnergy*`, `ResultSourceEnergy2*`, `Fuel1*`, `Fuel2*`

Direct electrical/device energy:

- `FansAndPumps.CoolNeededEnergy*`
- `FansAndPumps.PumpNeededEnergy*`
- `FansAndPumps.OtherResultCooling*`
- `LightAndDevices.Lights.General.DevicesNeededEnergy*`
- `LightAndDevices.BalancedDevices.General.DevicesNeededEnergy*`
- `LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergy*`
- `LightAndDevices.HotWaterPumps.General.DevicesNeededEnergy*`

Solar/DHW hooks:

- first-zone `HotWaterCalculations.SunEnergyCalculations.Actual.SunEnergyResTable.BGVPumpsTotal`
- first-zone `HotWaterCalculations.SunEnergyCalculations.BaseLine.SunEnergyResTable.BGVPumpsTotal`
- first-zone `HotWaterCalculations.SunEnergyCalculations.ESM.SunEnergyResTable.BGVPumpsTotal`
- first-zone `HotWaterCalculations.SunEnergyCalculations.ESM.SunEnergyResTable.BGVSunEnergy`
- first-zone `HotWaterCalculations.ResulNetEnergyESM`

Scale:

- `calcInput.General.InvestigationMethod`
- embedded EECalc `BuildingCategories` scale rows through `BuildingTypesManager`
- final `PrimaryEnergyTable.Total.Ref1`
- final `PrimaryEnergyTable.Total.Ref2`
- final `PrimaryEnergyTable.Total.BaseLine`
- final `PrimaryEnergyTable.Total.ESM`

## 19. Proposed aggregation oracle design

Do not implement yet.

Suggested future modules:

- `EecalcAggregationOracle`
  - `GetBuildingData`
  - `AggregateNeededEnergy`
  - `AggregateNetEnergy`
  - `AggregateNoInputsNetEnergy`
  - `AggregatePrimaryEnergyByTechnology`
  - `AggregatePrimaryEnergyFuelTable`
  - `AggregateFuelEnergyTable`
  - `AggregateEmissionNeededEnergy`
  - `AggregateEmissionEnergySupply`
  - `AggregateVei`
  - `CalculateScaleType`

Provider-free inputs:

- The R10 oracle should consume already-calculated zone/technology outputs.
- It should not read climate XML/JSON.
- It should not call heating/cooling/ventilation/DHW formulas.

Recommended debug columns:

- `Scope` (`Zone` / `Building`)
- `ZoneIndex`
- `Variant`
- `Category`
- `HasHeating`
- `HasCooling`
- `HeatedArea`
- `TotalHeatedArea`
- `SourceEnergy1`
- `SourceEnergy2`
- `FuelInput1`
- `FuelInput2`
- `FuelReportBucket1`
- `FuelReportBucket2`
- `NeededAbsolute`
- `NeededArea`
- `NetAbsolute`
- `NetArea`
- `PrimaryTechnology`
- `PrimaryFuelBucket`
- `FuelEnergyBucket`
- `EmissionNeeded`
- `EmissionSupplyBucket`
- `IsFirstBuildingZone`
- `BgvFirstZoneUsed`
- `SolarBGVPumpsTotal`
- `Vei`
- `GeneralVei`
- `ScalePointerEsm`
- `ScalePointerBaseLine`

Mode switches for future oracle:

- `LegacyEECalcStrict`: preserve all behaviors documented here.
- `LegacyEECalcCorrectedData`: same aggregation behavior; only data-provider climate corrections affect upstream calculations.
- `CurrentOrdinance`: same aggregation behavior unless an explicit future product-correctness mode changes final aggregation quirks.

## 20. Minimal fixtures

Fixture A: one-zone complete building.

- One heated and cooled zone.
- Nonzero heating, cooling, ventilation, BGV, lights, devices, fans/pumps, and other.
- Validates basic category totals, primary table, fuel table, CO2 table, VEI, and scale pointer values.

Fixture B: two-zone heating/cooling flags.

- Zone 1 `HasHeating=true`, `HasCooling=false`.
- Zone 2 `HasHeating=false`, `HasCooling=true`.
- Validates heating/cooling category filters and total heated area normalization.

Fixture C: first-zone BGV behavior.

- Two zones with different BGV values.
- Expected building BGV uses first zone only.
- Expected direct device categories still sum both zones.

Fixture D: BGVPumps solar behavior.

- First zone has normal BGVPumps and solar `BGVPumpsTotal`.
- Second zone has different normal BGVPumps.
- Validates Ref1/Ref2 Actual solar reuse, Actual/BaseLine/ESM matching solar variants, and Fuel8 post-loop additions.

Fixture E: Fuel1/Fuel8 inversion.

- Direct electrical lights/devices and a separate source using input `Fuel.Fuel8`.
- Validates `Fuel.Fuel1 -> Fuel8` and `Fuel.Fuel8 -> Fuel1` in fuel, primary fuel, and CO2 supply tables.

Fixture F: hardcoded primary/CO2 coefficients.

- One nonzero source-energy value per `Fuel1` through `Fuel11`.
- Validates primary coefficients and CO2 coefficients exactly.

Fixture G: total fuel double-count.

- Nonzero only in reporting `Fuel1`.
- Expected `FuelEnergyTable.Total.*Area = Fuel1 + Fuel1`.
- Confirms `KD-A001`.

Fixture H: VEI fuels.

- ESM heating, heating ventilation, and BGV source rows using `Fuel1`, `Fuel6`, `Fuel7`, and a non-VEI fuel.
- Validate efficiency > 100 electricity VEI, direct renewable VEI, and no contribution for other fuels.

Fixture I: primary total overwrite.

- Nonzero `PrimaryEnergyTable.Devices` and `PrimaryEnergyTable.Other`.
- Validate that final result after `CalculatePrimaryTotalEnergy` uses `Other`, not `Devices`.

Fixture J: referent scale.

- Investigation type `ReferentValues`.
- Controlled `PrimaryEnergyTable.Total.Ref1` and `Ref2`.
- Validate derived A+/A/B/C/D/E/F/G thresholds and `PoiterValue` fields.

Fixture K: fixed scale.

- Non-reference investigation type.
- Validate thresholds are copied from embedded `BuildingTypesManager` scale rows.

## Files read

- `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs`
- `reference/eecalc-decompiled/EECalcCore.Calculations.BuildingTypesManager.cs`
- `analysis/docs/06_building_aggregation.md`
- `analysis/r8_dhw_bgv_reverse_engineering.md`
- `analysis/r9_lighting_devices_reverse_engineering.md`
- `analysis/validation_known_differences.md`
- `analysis/reference-data/climate_provider_review.md`
- `analysis/docs/01_call_graph.md`
- `analysis/docs/02_method_index.md`
- `analysis/docs/03_formula_catalog.md`

## Methods analyzed

- `BuildingCalculations`
- `ZoneCalculations`
- `GetBuildingData`
- `GetConditionedArea`
- `UpdateRefsState`
- `UpdateActualState`
- `UpdateBaseLineState`
- `UpdateEsmState`
- `CalculateTotalsNeededEnergyTable`
- `CalculateTotalActual`
- `CalculateTotalActualYearly`
- `CalculateTotalBaseLine`
- `CalculateTotalBaseLineYearly`
- `CalculateTotalEsm`
- `CalculateTotalEsmYearly`
- `CalculateTotalRefs`
- `CalculateTotalRefsYearly`
- `CalculateNetEnergyByTechnologiesBuilding`
- `CalculateNetEnergyByTechnologies`
- `CalculateNetWithoutInputsEnergyByTechnologies`
- `CalculatePrimaryEnergyByTechnologies`
- `GetPrimaryEnergyCoeficient`
- `GetPrimaryFuelTypeAndValues`
- `GetPrimaryFuelTypeRef1`
- `GetPrimaryFuelTypeRef2`
- `GetPrimaryFuelType`
- `GetPrimaryFuelTypeBaseLine`
- `GetPrimaryFuelTypeEsm`
- `CalculatePrimaryEnergyPerArea`
- `CalculatePrimaryFuelTypeAndValuesPerArea`
- `CalculatePrimaryEnergyFuelTotal`
- `CalculatePrimaryTotalEnergy`
- `GetFuelTypeAndValues`
- `GetFuelTypeRef1`
- `GetFuelTypeRef2`
- `GetFuelType`
- `GetFuelTypeBaseLine`
- `GetFuelTypeEsm`
- `SetFuelValue`
- `CalculateTotalFuelEnergy`
- `BuildingCO2Calculations`
- `ZoneCO2Calculations`
- `CalculateCO2Emissions`
- `CalculateCO2EmissionsRef1`
- `CalculateCO2EmissionsRef2`
- `CalculateCO2EmissionsActual`
- `CalculateCO2EmissionsBaseLine`
- `CalculateCO2EmissionsESM`
- `GetEkoCoeficient`
- `Co2GetFuelTypesBuilding`
- `Co2EnergyCalculationBuildingRef1`
- `Co2EnergyCalculationBuildingRef2`
- `Co2EnergyCalculationBuildingActual`
- `Co2EnergyCalculationBuildingBaseLine`
- `Co2EnergyCalculationBuildingESM`
- `GetFuelTypeCo2Ref1`
- `GetFuelTypeCo2Ref2`
- `GetFuelTypeCo2Actual`
- `GetFuelTypeCo2BaseLine`
- `GetFuelTypeCo2ESM`
- `Co2CalculateEmissionEnergySupplyBuilding`
- `Co2EnergyCalculateTotal`
- `CalculateSavings`
- `CalculateFuelSavings`
- `ClearNeededVEIenergy`
- `GetVeiHeating`
- `GetVeiHeatVentilation`
- `GetVeiBGV`
- `CalculateElectricityVEI`
- `CalculateTotalVei`
- `CalculateBuildingPowerEnergy`
- `SetScaleValues`
- `SetScaleType`
- `BuildingTypesManager.GetClimateZoneParams`
