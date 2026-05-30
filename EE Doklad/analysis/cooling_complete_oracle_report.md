# Cooling complete oracle report

## Scope

Created a test-side EECalc-compatible cooling oracle. Production code was not modified.

New files:

- `EE.Doklad.Tests/Validation/EecalcMonthlyCoolingOracle.cs`
- `EE.Doklad.Tests/Validation/EecalcMonthlyCoolingOracleTests.cs`
- `analysis/cooling_complete_oracle_report.md`
- `test-results/validation/debug_cooling_full_pipeline.csv`

Extended validation-only fixture models:

- `EE.Doklad.Tests/Validation/EecalcValidationFixture.cs`
- `EE.Doklad.Tests/Validation/EecalcEnvelopeFixture.cs`

## Implemented pipeline

`EecalcMonthlyCoolingOracle` implements:

- `MonthlyDays`
- `Qsol`
- `Qint`
- `Qoccupants`
- `Qgain`
- `HtrCooling`
- `QtrCooling`
- `Hinf`
- `Qinf`
- `Ac`
- `Eta`
- `QLatentOccupants`
- `QLatentInf`
- `QLatentVent`
- `QfreeCooling`
- `QveCooling`
- `QcoolRaw`
- `QcoolWithInputs`
- `ResultNoInputsNetEnergy`
- `ResultCoolingInputs`
- `ResultNetEnergy`

Core monthly balance:

```text
Qgain = Qsol + Qint + Qoccupants
Qloss = QtrCooling + Qinf
Ac = 1 + (HeatedArea * HeatCapacity / (HtrCooling + Hinf)) / 15
Eta = cooling utilization factor from gamma = Qgain / Qloss

QcoolRaw =
    Qgain
    - Eta * Qloss
    + QLatentOccupants
    + QLatentInf
    + QLatentVent

QcoolWithInputs = QcoolRaw + QfreeCooling + QveCooling

ResultNoInputsNetEnergy = Sum(QcoolRaw) / HeatedArea
ResultCoolingInputs = Sum(QfreeCooling)
ResultNetEnergy = ResultNoInputsNetEnergy - ResultCoolingInputs - ResultVentilationInputs
```

## Preserved quirks

The oracle explicitly preserves the confirmed cooling quirks:

- `KD-C001`: `SumWallDirecrionsHu1Cooling` uses north walls eight times.
- `KD-C002`: cooling wall layer 5 uses `InnerA5 * InnerA5`.
- `KD-C003`: cooling ceiling layer 5 uses `CeilingA5 * CeilingA5`.
- `KD-C004`: cooling floor layer 6 uses `OtherFloorS4` for the temperature delta.
- `KD-C005`: latent ventilation Saturday post-ventilation hours multiply by `Debit` twice.
- `KD-C006`: free-cooling holidays reuse the Sunday night-ventilation schedule.

## Debug CSV

Generated:

`debug_cooling_full_pipeline.csv`

Also copied to:

`test-results/validation/debug_cooling_full_pipeline.csv`

Columns:

```text
Month,Qsol,Qint,Qoccupants,Qgain,QtrCooling,Qinf,Qloss,Ac,Eta,QLatentOccupants,QLatentInf,QLatentVent,QcoolRaw,QfreeCooling,QveCooling,QcoolWithInputs
```

The CSV was produced from the oracle through an isolated temporary console harness, then the temporary harness was removed.

## Tests

Created self-consistency tests in `EecalcMonthlyCoolingOracleTests`:

- Monthly pipeline identity checks:
  - `Qgain = Qsol + Qint + Qoccupants`
  - `Qloss = QtrCooling + Qinf`
  - `QcoolRaw = Qgain - Eta * Qloss + latent terms`
  - `QcoolWithInputs = QcoolRaw + QfreeCooling + QveCooling`
  - result aggregation identities
- `Eta` branch behavior:
  - positive negative-power branch
  - near-one branch
  - negative-gamma branch
  - fallback-zero branch including exact `0.99` and `1.01`
- representative transmission quirk checks.

No tests compare against production yet.

## Verification status

Completed:

- The oracle compiled and ran in an isolated temporary harness containing only the validation fixture files, monthly-days oracle, and cooling oracle.
- The debug CSV was generated successfully.

Blocked:

- `dotnet test EE.Doklad.Tests/EE.Doklad.Tests.csproj --filter EecalcMonthlyCoolingOracleTests --no-restore` does not currently reach the new tests because the existing test project has unrelated compile errors in:
  - `HeatingCalculationServiceTests.cs`
  - `ZtuQtrTests.cs`

Those errors predate this oracle work and are not caused by production changes in this task.
