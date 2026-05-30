# Validation known differences

This file records confirmed behavioral differences that the validation harness must treat explicitly instead of silently normalizing.

## KD-004: EECalc internal wall Hu uses NorthWalls eight times

Status: confirmed.

Source:

- `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1`
- `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3838`

Finding:

`SumWallDirecrionsHu1` calls:

```text
CalcWallDirectionParameterHu1(section.NorthWalls.Current, averageMontlyTemp, averageInnerHeatTemp)
```

eight times. No other wall directions are used in this method.

Decision:

- `LegacyEECalc` mode preserves EECalc behavior as `NorthWalls.Current * 8`.
- `CurrentCorrect` mode implements corrected behavior by summing:
  - North
  - NorthEast
  - East
  - SouthEast
  - South
  - SouthWest
  - West
  - NorthWest

Validation impact:

- R3 Htr/Qtr oracle must expose mode-specific behavior.
- Golden EECalc parity tests must use `LegacyEECalc`.
- Product-correctness diagnostics may use `CurrentCorrect` and report the delta as KD-004, not as an unexplained formula mismatch.

## R7 ventilation confirmed KD items

Status: confirmed for reverse-engineering documentation. These are EECalc-compatible behaviors that a future ventilation oracle must preserve unless explicitly running in a corrected/diagnostic mode.

- KD-V001: `GetVentilationBaseLine` omits `ResultEnergyForCooling`, while `SetVentilationBaseLine` reads it.
- KD-V002: Working schedule savings copy ESM ventilation schedule fields into BaseLine fields.
- KD-V003: Savings share calculation uses `Part = Saving / totalSaving` with no observed zero guard.
- KD-V004: Heating `SecondRecEfficiency > 100` uses the special thermo-pump/source split path.
- KD-V005: Heating second recovery only operates when `HeatingAirDifference` is between `3` and `8` inclusive.
- KD-V006: Cooling input schedule end comparison differs by day type: workday uses `< End`, Saturday/Sunday use `<= End`.
- KD-V008: `GetDaysHours` prepends hour 23 before hours 0-23.
- KD-V009: Cooling density helper use differs by variant.
- KD-V010: Heating month hours and average ventilation temperature ignore holidays.
- KD-V011: `VentilationHeatEnergy*` sets `ResultEnergyForHeating*` to `0` unless every heating-season month contributes a non-NaN value.
- KD-V012: Heating input energy is not clamped and can be negative.
- KD-V013: Actual/BaseLine/ESM ETLine updates are limited to January and March and use `monthlySensible * HeatedArea`.
- KD-V014: Cooling withering energy is stored separately and is not included in cooling needed-energy conversion.
- KD-V015: Cooling-season ventilation heating is stored in `ResultEnergyForHeating*`, but cooling needed-energy conversion ignores it.

Expected design behavior, not KD:

- Ref1/Ref2 reuse baseline schedules. Reference buildings keep the baseline schedule pattern and replace selected physical parameters such as temperatures, infiltration-related values, ventilation scalar inputs, and efficiencies.

## KD-DATA-001: DefaultParams.xml January temperature sign error

Status: confirmed.

Classification:

Confirmed legacy XML data error, not a calculation/formula error.

Sources:

- `reference/eecalc-config/DefaultParams.xml`
- `EE.Doklad/Data/climate_zones.json`
- `analysis/reference-data/climate_temperature_xml_vs_json_audit.md`
- `analysis/reference-data/source_binding_audit_heating_cooling_ventilation.md`

Finding:

`DefaultParams.xml` contains negative January average temperatures for climate zones 1-3.
The correct ordinance values are the positive January temperatures in `EE.Doklad/Data/climate_zones.json`.

| ZoneId | Month | Wrong legacy XML AvgTemp | Correct ordinance AvgTemp |
| --- | --- | ---: | ---: |
| 1 | January | -1.9 | 1.9 |
| 2 | January | -0.5 | 0.5 |
| 3 | January | -0.1 | 0.1 |

Decision:

- `LegacyEECalcStrict` preserves `DefaultParams.xml` exactly, including the sign error.
- `LegacyEECalcCorrectedData` uses corrected January values from `EE.Doklad/Data/climate_zones.json`.
- `CurrentOrdinance` uses `EE.Doklad/Data/climate_zones.json`.

Validation impact:

- EECalc parity tests that target legacy XML behavior must use `LegacyEECalcStrict`.
- Corrected legacy-data validation must use `LegacyEECalcCorrectedData`.
- Current ordinance validation must use `CurrentOrdinance`.
- Do not silently normalize these values; report the mismatch as `KD-DATA-001`.

Source-binding impact:

- R1-R7 monthly outdoor `AvgTemp` lookups use `PreferencesManager`, which loads `DefaultParams.xml`.
- Heating R2/R3/R5, cooling R6, and ventilation R7 are therefore affected by this data difference in `LegacyEECalcStrict` mode.
- `DefaultSunParams.xml` is a separate solar/DHW source through `SunEnergyPreferencesManager`; it is not the source for heating/cooling/ventilation `AvgTemp` in R1-R7.

## R10 aggregation ILSpy-verified findings

These findings were reclassified after direct ILSpy verification of the legacy EECalc binary/decompiled code.

### KD-A001: Total fuel calculations add Fuel1 twice

Status: confirmed defect.

Evidence:

- Verified directly in ILSpy.

Methods:

- `CalculateTotalFuelRef1`
- `CalculateTotalFuelRef2`
- `CalculateTotalFuelActual`
- `CalculateTotalFuelBaseLine`
- `CalculateTotalFuelESM`

Finding:

`Fuel1` is added twice in every total fuel variant calculation.

Classification:

Confirmed legacy aggregation defect.

Decision:

- `LegacyEECalcStrict` preserves the duplicate `Fuel1` addition.
- Corrected mode may fix the total fuel aggregation.

Validation impact:

- Strict parity must not silently normalize the total fuel row.
- Corrected-mode assertions should explicitly document any removal of the duplicate `Fuel1` term.

### KD-A009: Fuel1/Fuel8 reporting-bucket inversion

Status: confirmed behavior.

Evidence:

- Verified directly in ILSpy.

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

Representative methods:

- `GetFuelTypeRef1`
- `GetPrimaryFuelTypeRef1`
- `GetFuelTypeCo2Ref1`

Finding:

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

Classification:

Legacy reporting-bucket mapping.

Interpretation:

Results appear consistent with historical EECalc reporting. This is not classified as a formula defect or calculation error.

Decision:

- `LegacyEECalcStrict` preserves the bucket mapping.
- `CurrentOrdinance` preserves the bucket mapping unless the reporting model is intentionally redesigned.

Validation impact:

- Oracles should expose both the input fuel enum and the reporting bucket.
- Fuel bucket comparisons must compare against EECalc reporting buckets, not against enum names.
