# Parity Strategy

Current goal:

```text
Real EECalc Binary
==
Reverse Engineered Implementation
```

Normative correctness is not the current goal.

## Current Mode

Use `LegacyEECalcStrict`.

This means:

- `DefaultParams.xml` is used exactly as-is.
- `DefaultSunParams.xml` is used exactly as-is.
- Confirmed legacy defects are preserved.
- Confirmed legacy reporting behaviors are preserved.
- No formula corrections are introduced.
- No climate corrections are introduced.

## Work Order

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

## Debug Requirements

Every oracle should emit enough intermediate values to locate mismatches before final totals are compared.

At minimum, debug rows should include:

- module
- variant
- month
- source data mode
- raw formula inputs
- intermediate values
- final table values
- known-difference flags where applicable

Provider rewiring and corrected-data modes are deferred until strict binary parity is complete.
