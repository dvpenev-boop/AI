# Methodology Map

This document maps the clean-room code to the EECalc calculation concepts.

## Entry Point

`Program.cs` wires the reference fixture, climate XML providers, ECM states, and verification modes.

Primary modes:

- `--verify-envelope-savings`
- `--verify-ventilation-savings`
- `--verify-component-savings`
- `--verify-full-project`
- `--verify-cooling-details`

## Climate Data

Code:

- `clean_code/climate/LegacyEecalcXmlClimateDataProvider.cs`
- `clean_code/climate/LegacyEecalcXmlSunEnergyDataProvider.cs`

Data:

- `reference/eecalc-config/DefaultParams.xml`
- `reference/eecalc-config/DefaultSunParams.xml`

Important mapping:

- Public climate zone id `N` maps to EECalc XML climate zone `Number = N - 1`.
- The validated mode is `LegacyEECalcStrict`.

## Calendar and Schedules

Code:

- `clean_code/formulas/EecalcMonthlyDaysOracle.cs`

Behavior copied from EECalc:

- Reference calendar year is 2006.
- Holidays are removed from workdays.
- Holidays use the Sunday/non-project path in several heating/cooling formulas.
- Cooling season can be overridden from CLI:
  - `--cooling-first-month`
  - `--cooling-first-day`
  - `--cooling-last-month`
  - `--cooling-last-day`
- Holidays can be overridden from CLI:
  - `--holiday <month> <count>`

## Heating

Code:

- `clean_code/formulas/EecalcMonthlyHeatingOracle.cs`
- `clean_code/formulas/EecalcHtrQtrOracle.cs`
- `clean_code/formulas/EecalcEnvelopeEsmCalculator.cs`
- `clean_code/formulas/EecalcEnvelopeSavingsOracle.cs`

Validated envelope ECM target:

- Baseline: `80.388 kWh/m2`
- ESM: `22.844 kWh/m2`
- Saving: `57.544 kWh/m2`
- Outer walls: `39.669`
- Windows: `2.532`
- Non-transparent roof: `11.826`
- Floor: `3.517`

## Cooling

Code:

- `clean_code/formulas/EecalcMonthlyCoolingOracle.cs`
- `clean_code/formulas/EecalcCoolingEsmCalculator.cs`

Important reverse-engineered finding:

- The EECalc cooling ECM table distributes needed/source EI1 energy, not net energy.
- EI1 is calculated from net cooling energy through:

```text
source = net / (TransmitTempEfficiency * SupplyNetEfficiency * Automatic * EnergyManagement * GeneratorColdEfficiency)
needed = source + sourceEnergy2
```

The clean implementation therefore exposes needed/source cooling values in verification and ECM distribution.

ESM cooling may include reduced lighting cooling gains:

```text
lights cooling 0.6/56 -> 0.4/40
```

This is why an envelope-only ESM cooling value can differ from the final ESM cooling value.

## Ventilation

Code:

- `clean_code/formulas/EECalcR7R8R9Oracles.cs`
- `clean_code/formulas/EecalcVentilationEsmCalculator.cs`
- `clean_code/formulas/EecalcQveOracle.cs`

Validated behaviors:

- Ventilation heating and ventilation cooling are separate full-project rows.
- Cooling ventilation contribution is transferred into the cooling core row as `ResulVentilationInputs`.
- Ventilation cooling needed energy uses the cooling generator efficiency chain.

## DHW/BGV

Code:

- `clean_code/formulas/EECalcR7R8R9Oracles.cs`

Key class:

- `EECalcDhwBgvOracle`

Implemented:

- DHW without solar for Current/Normalized.
- DHW with solar collectors for ESM.
- Solar pump electricity row.
- Solar rows via `CalculateSolarRows`.

Validated solar output:

- Solar useful energy: about `17931.6 kWh`
- Solar energy per area: about `17.932 kWh/m2`
- Pump electricity: about `284.8 kWh`, or `0.285 kWh/m2`

## Lighting, Devices, Fans and Pumps

Code:

- `clean_code/formulas/EecalcLightingDevicesEsmCalculator.cs`
- `clean_code/formulas/EecalcFansPumpsEsmCalculator.cs`
- `clean_code/formulas/EecalcComponentSavingsOracle.cs`

Implemented rows:

- Lighting.
- Devices affecting heat balance.
- Devices not affecting heat balance.
- Fans and pumps.
- DHW pumps.
- Other.

## Full Project Aggregation

Code:

- `Program.cs`
- `clean_code/models/EECalcFullOracleModels.cs`

The valid full-project state model has three states:

- Current / Actual
- Normalized / Baseline
- ESM

The implementation intentionally does not include a general combined ECM allocator across all measures. Individual aggregators are validated separately.

