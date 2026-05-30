# EECalc Reverse Engineering Reference

## Status

Reverse-engineering phase completed.

Modules analyzed:

- R1-R5 Heating
- R6 Cooling
- R7 Ventilation
- R8 DHW/BGV
- R9 Lighting/Devices
- R10 Aggregation / Primary Energy / CO2 / Energy Class

This package is documentation and reference data only. It is not executable code.

## Confirmed ILSpy Findings

### KD-A001

Confirmed legacy aggregation defect.

```text
Fuel1 + Fuel1 + Fuel2 + Fuel3 + ...
```

Fuel1 is added twice.

Preserve in strict parity mode.

### KD-A009

Confirmed legacy reporting behavior.

```text
Fuel1 -> Fuel8 reporting bucket
Fuel8 -> Fuel1 reporting bucket
```

Not a formula defect.

Preserve in strict parity mode.

## Climate Data

### KD-DATA-001

`DefaultParams.xml` contains January sign errors for climate zones 1-3.

For parity validation:

```text
DefaultParams.xml is authoritative.
```

No correction is applied in parity mode.

## Current Goal

Primary objective:

```text
Real EECalc Binary
==
Reverse Engineered Implementation
```

Normative correctness is NOT the current goal.

Parity is the current goal.

## Mode Definitions

### LegacyEECalcStrict

Use:

- `DefaultParams.xml` exactly as-is
- `DefaultSunParams.xml` exactly as-is
- all confirmed legacy defects
- all confirmed legacy reporting behaviors

Purpose:

Exact EECalc parity.

### LegacyEECalcCorrectedData

Reserved for future use.

Not part of current parity work.

### CurrentOrdinance

Reserved for future use.

Not part of current parity work.

## Parity Strategy

Current order of work:

```text
1. Freeze reverse-engineering reference package
2. Implement EecalcHeatingOracle
3. Build debug CSV pipeline
4. Compare against real EECalc outputs
5. Resolve mismatches
6. Extend parity to all modules
7. Only after parity:
   provider rewiring
```

## Package Use

Use this package as the single source of truth for oracle implementation and parity validation.

Do not introduce formula corrections or climate-data corrections during strict parity work.
