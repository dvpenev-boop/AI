# Execution Order

Current parity implementation order:

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

Full engine execution order for future oracles:

```text
Data providers
  -> MonthlyDays / calendar
  -> heating, cooling, ventilation, DHW/BGV, lighting/devices modules
  -> zone result tables
  -> building aggregation
  -> primary energy / fuel / CO2 / VEI / scale
  -> debug CSVs
  -> real EECalc binary comparison
```

Provider rewiring is explicitly out of scope until binary parity has been established.
