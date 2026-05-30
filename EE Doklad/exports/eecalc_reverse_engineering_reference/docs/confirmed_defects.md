# Confirmed Defects and Legacy Behaviors

## Confirmed Defects

### KD-A001

Confirmed legacy aggregation defect.

Verified total fuel variants:

- `CalculateTotalFuelRef1`
- `CalculateTotalFuelRef2`
- `CalculateTotalFuelActual`
- `CalculateTotalFuelBaseLine`
- `CalculateTotalFuelESM`

Behavior:

```text
Fuel1 + Fuel1 + Fuel2 + Fuel3 + ...
```

Fuel1 is added twice.

Strict parity mode must preserve this defect.

### KD-DATA-001

Confirmed legacy XML data error.

`DefaultParams.xml` contains January sign errors for climate zones 1-3. Strict parity mode preserves the XML exactly.

## Confirmed Legacy Reporting Behavior

### KD-A009

Verified tables:

- `FuelEnergyTable`
- `PrimaryEnergyFuelTable`
- `EmissionEnergySupplyTable`

Verified variants:

- Ref1
- Ref2
- Actual
- BaseLine
- ESM

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

KD-A009 is not a formula defect and not a calculation error.

Strict parity mode must preserve it.
