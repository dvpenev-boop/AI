# ILSpy Verified Findings

Scope: documentation-only reclassification of R10 aggregation findings after ILSpy verification. No production code, oracle code, tests, or provider wiring were modified.

## 1. Verified defects

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

Observed code behavior:

```text
Fuel1 is added twice in every total fuel variant calculation.
```

Classification:

Confirmed legacy aggregation defect.

Mode decision:

- `LegacyEECalcStrict`: preserve.
- Corrected mode: may fix.

## 2. Verified legacy behaviors

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

Observed behavior:

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

Results appear consistent with historical EECalc reporting. This should not be treated as a formula defect or calculation error.

Mode decision:

- `LegacyEECalcStrict`: preserve.
- `CurrentOrdinance`: preserve unless the reporting model is intentionally redesigned.

## 3. Findings reclassified after ILSpy review

| Finding | Previous treatment | ILSpy classification | Decision |
| --- | --- | --- | --- |
| `KD-A001` | R10 KD candidate | Confirmed legacy aggregation defect across all total fuel variants | Preserve in strict mode; corrected mode may fix. |
| `KD-A009` | R10 KD candidate / quirk | Confirmed legacy reporting-bucket mapping across fuel, primary fuel, and CO2 supply tables for all variants | Preserve in strict and current ordinance modes unless reporting is redesigned. |

The key distinction is that `KD-A001` changes a numeric total through duplicate addition, while `KD-A009` is a stable reporting-bucket convention. The latter affects labels and table placement, but it is not currently evidence of a calculation defect.

## 4. Impact on parity implementation

Strict parity must reproduce both findings:

- `KD-A001`: total fuel calculations must include the duplicated `Fuel1` term for Ref1, Ref2, Actual, BaseLine, and ESM where EECalc does.
- `KD-A009`: fuel input enum and reporting bucket must remain separate concepts across `FuelEnergyTable`, `PrimaryEnergyFuelTable`, and `EmissionEnergySupplyTable`.

Recommended debug fields for aggregation parity:

- `FuelInputEnum`
- `FuelReportBucket`
- `FuelQuantityBeforeBucket`
- `FuelQuantityAfterBucket`
- `TotalFuelIncludesDuplicateFuel1`
- `AggregationMode`

Validation should not compare fuel bucket names by enum identity. It should compare the final EECalc reporting buckets after applying the legacy mapping.

## 5. Impact on future ordinance mode

Future ordinance mode should treat the two findings differently:

- `KD-A001` may be corrected because it is a confirmed aggregation defect.
- `KD-A009` should remain unless the reporting model is intentionally redesigned, because it is confirmed historical reporting behavior rather than a formula error.

If `KD-A009` is ever redesigned, that change should be documented as a reporting-schema migration, not as a calculation correction.
