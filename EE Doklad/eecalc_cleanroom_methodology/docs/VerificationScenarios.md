# Verification Scenarios

Run from the package root.

## Build

```powershell
dotnet build --no-restore -p:UseSharedCompilation=false
```

## Baseline Modes

```powershell
dotnet run --no-build -- --verify-envelope-savings
dotnet run --no-build -- --verify-ventilation-savings
dotnet run --no-build -- --verify-component-savings
dotnet run --no-build -- --verify-full-project
dotnet run --no-build -- --verify-cooling-details
```

## Zone 9 Cooling With Ventilation Cooling at 18 C

```powershell
dotnet run --no-build -- --verify-full-project --climate-zone 9 --cooling-flow-temp 18 --cooling-esm-flow-temp 18 --cooling-vent-debit 0.5
```

## Zone 9 With 10 August Holidays and No-Treatment Cooling Debit 2

```powershell
dotnet run --no-build -- --verify-full-project --climate-zone 9 --cooling-flow-temp 18 --cooling-esm-flow-temp 18 --cooling-vent-debit 0.5 --cooling-free-debit 2 --cooling-free-work 23 6 --cooling-free-sat 0 0 --cooling-free-sun 0 0 --holiday 8 10
```

## ESM-Only No-Treatment Cooling Measure

Current/Normalized no-treatment debit is `0`; ESM no-treatment debit is `2`.

```powershell
dotnet run --no-build -- --verify-full-project --climate-zone 9 --cooling-flow-temp 18 --cooling-esm-flow-temp 18 --cooling-vent-debit 0.5 --cooling-free-debit 0 --cooling-esm-free-debit 2 --cooling-free-work 23 6 --cooling-free-sat 0 0 --cooling-free-sun 0 0 --holiday 8 10
```

Detailed cooling breakdown:

```powershell
dotnet run --no-build -- --verify-cooling-details --climate-zone 9 --cooling-flow-temp 18 --cooling-esm-flow-temp 18 --cooling-vent-debit 0.5 --cooling-free-debit 0 --cooling-esm-free-debit 2 --cooling-free-work 23 6 --cooling-free-sat 0 0 --cooling-free-sun 0 0 --holiday 8 10
```

