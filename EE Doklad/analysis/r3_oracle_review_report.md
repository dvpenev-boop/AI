# R3 Htr/Qtr oracle review report

## 1. Summary

Reviewed `EecalcHtrQtrOracle.cs` against `analysis/r3_qtr_htr_reverse_engineering.md` and the confirmed known difference `KD-004`.

Result: PASS. The test-only oracle matches the documented LegacyEECalc behavior for R3 Htr/Qtr. No code fixes were needed.

Scope explicitly excluded: Qgn, Gamma, Ni, NetEnergy, production code, UI.

## 2. Files reviewed

- `EE.Doklad.Tests/Validation/EecalcHtrQtrOracle.cs`
- `EE.Doklad.Tests/Validation/EecalcR3HtrQtrValidationTests.cs`
- `EE.Doklad.Tests/Validation/EecalcEnvelopeFixture.cs`
- `EE.Doklad.Tests/Validation/EecalcEnvelopeSnapshotRow.cs`
- `analysis/r3_qtr_htr_reverse_engineering.md`
- `analysis/validation_known_differences.md`
- `test-results/validation/debug_r3_htr_qtr.csv`

Note: `docs/known-differences.md` was not present in the workspace; the available known-differences source is `analysis/validation_known_differences.md`.

## 3. Method-by-method compliance table

| Method | Expected | Actual | Status | Notes |
|---|---|---|---|---|
| `CalculateParameterHgCurrent` | `Floor.AccumulateFloorA * Floor.AccumulateFloorU` | Returns `fixture.Floor.AccumulateFloorA * fixture.Floor.AccumulateFloorU` | PASS | Direct match. |
| `CalculateItemsWalls` | `sum(OuterA_i * OuterU_i) + sum(Outer_i.SumL) + sum(Outer_i.SumX)`, `i=1..6` | `SumProduct(OuterA, OuterU, 6) + Sum(OuterSumL, 6) + Sum(OuterSumX, 6)` | PASS | Direct match with array-backed fixture fields. |
| `SumAllDirectionsWallsCurrent` | North + NorthEast + East + SouthEast + South + SouthWest + West + NorthWest | `WallDirections(fixture).Sum(CalculateItemsWalls)` over the eight directions in that order | PASS | Direct match. |
| `SumAllDirectionWindowsCurrent` | `sum(AccumulateWindowU * AccumulateWindowA)` for all eight directions | `WallDirections(fixture).Sum(wall => wall.AccumulateWindowU * wall.AccumulateWindowA)` | PASS | Direct match. |
| `SumNonTrasparentRoof` | `sum(NonTransparentA_i * NonTransparentU_i) + sum(SumL_i) + sum(SumX_i)`, `i=1..9` | `SumProduct(NonTransparentA, NonTransparentU, 9) + Sum(NonTransparentSumL, 9) + Sum(NonTransparentSumX, 9)` | PASS | Element 6 is represented positionally as array index `5`; this preserves the decompiled element-6 slot without carrying the Cyrillic property spelling into the test model. |
| `SumTrasparentRoof` | `sum(TransparentA_i * TransparentU_i)`, `i=1..9` | `SumProduct(TransparentA, TransparentU, 9)` | PASS | Element 6 is represented positionally as array index `5`; this preserves the decompiled element-6 slot. |
| `CalculateParameterHdCurrent` | walls + windows + non-transparent roof + transparent roof | Calls the four required sub-methods and sums them | PASS | Direct match. |
| `CalcWallDirectionParameterHu1` | components 1-4 use `InnerA_i * InnerU_i`; component 5 uses `IneerA5 * IneerA5`; component 6 uses `InnerA6 * InnerU6` | Indexes 0-3 use `InnerA * InnerU`; index 4 uses `InnerA[4] * InnerA[4]`; index 5 uses `InnerA[5] * InnerU[5]` | PASS | Preserves the EECalc component-5 quirk through array position 5. |
| `SumWallDirecrionsHu1` | LegacyEECalc: `8 * CalcWallDirectionParameterHu1(NorthWalls.Current, ...)` | Returns `8.0 * CalcWallDirectionParameterHu1(fixture.NorthWalls, ...)` | PASS | KD-004 preserved. No CurrentCorrect behavior was implemented. |
| `CalcCeilingsParameterHu2` | components 1-4 use `CeilingA_i * CeilingU_i`; component 5 uses `CeilingA5 * CeilingA5`; component 6 uses `CeilingA6 * CeilingU6` | Indexes 0-3 use `CeilingA * CeilingU`; index 4 uses `CeilingA[4] * CeilingA[4]`; index 5 uses `CeilingA[5] * CeilingU[5]` | PASS | Preserves the EECalc component-5 quirk. |
| `CalcFloorsParameterHu3` | components 1-6 use `OtherFloorA_i * OtherFloorU_i` | Indexes 0-5 use `OtherFloorA * OtherFloorU` | PASS | Direct match. |
| `CalculateAverageHeatTempCurrent` | Uses direct `End - Start`, not `CalcHours`; weighted average from project/non-project hours | Uses schedule `EndHour - StartHour` through `Duration`, then weighted average | PASS | Direct match. |
| `CalculateParameterHtr` | `Hu = HuWalls + HuCeilings + HuFloors`; `Htr = Hd + Hg + Hu` | Computes wall/ceiling/floor Hu, Hd, Hg and returns their sum | PASS | Direct match. |
| `CalculateParameterQtr` | `Qtr = Htr * DegreeHours / 1000`; `DegreeHours` reuses R2 Qve degree-hour logic | Calls `CalculateDegreeHours`, then `qtr = htr * degreeHours / 1000.0` | PASS | Degree-hour formula matches R2 Qve structure. |

## 4. Confirmed preserved EECalc quirks

- KD-004 NorthWalls repeated 8 times: PASS. `SumWallDirecrionsHu1` returns `8.0 * CalcWallDirectionParameterHu1(fixture.NorthWalls, ...)`.
- `IneerA5 * IneerA5`: PASS. The oracle uses `wall.InnerA[4] * wall.InnerA[4]` for component 5.
- `CeilingA5 * CeilingA5`: PASS. The oracle uses `roof.CeilingA[4] * roof.CeilingA[4]` for component 5.
- Cyrillic A property mapping: PASS by positional mapping. The test fixture uses arrays, so decompiled element 6 maps to index `5` for non-transparent and transparent roof arrays.
- Direct `End - Start` in `CalculateAverageHeatTempCurrent`: PASS. The oracle uses `EndHour - StartHour` and does not call `CalcHours`.

## 5. CSV sanity check from `debug_r3_htr_qtr.csv`

| Fixture | Month | Hu relation | Htr relation | Qtr relation | Status |
|---|---|---|---|---|---|
| `r3_minimal_envelope` | January | `21.379310344827584 = 21.379310344827584 + 0 + 0` | `75.379310344827587 = 4 + 50 + 21.379310344827584` | `34.976 = 75.379310344827587 * 464 / 1000` | PASS |

Sanity formulas checked:

```text
Hu = HuWalls + HuCeilings + HuFloors
Htr = Hd + Hg + Hu
Qtr = Htr * DegreeHours / 1000
```

## 6. Fixes

No fixes were needed. No code files were changed during this review.

## 7. R3 readiness

The R3 test-only oracle is ready for R3 parity comparison against EE.Doklad, once the existing unrelated test project compile errors are resolved or isolated.
