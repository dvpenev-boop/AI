# EECalc Cleanroom Methodology

Standalone .NET implementation package for the reverse-engineered EECalc energy and ECM/ESM methodology.

This folder is intentionally separate from the working reverse-engineering repo. It contains the code and reference XML data needed to compile and run verification scenarios from scratch.

## Contents

- `EecalcCleanroomMethodology.csproj` - standalone console project.
- `Program.cs` - verification runner and scenario wiring.
- `TestFixture.cs` - current reference building fixture and component inputs.
- `CompatibilityModels.cs` - compatibility enum/model shims.
- `clean_code/` - clean-room implementation of formulas, models, climate providers, and ECM aggregators.
- `reference/eecalc-config/DefaultParams.xml` - EECalc climate and default parameter XML.
- `reference/eecalc-config/DefaultSunParams.xml` - EECalc solar radiation XML.
- `docs/` - implementation map, validation notes, and known gaps.

No files from `bin/`, `obj/`, or the decompiled EECalc source are required for this package.

## Build

This copy targets `net10.0` because the current machine has .NET 10 reference packs installed locally. The code has no external NuGet package dependencies.

```powershell
dotnet build --no-restore -p:UseSharedCompilation=false
```

If restore is needed on a fresh machine:

```powershell
dotnet restore
dotnet build -p:UseSharedCompilation=false
```

## Main Verification Commands

```powershell
dotnet run --no-build -- --verify-envelope-savings
dotnet run --no-build -- --verify-ventilation-savings
dotnet run --no-build -- --verify-component-savings
dotnet run --no-build -- --verify-full-project
dotnet run --no-build -- --verify-cooling-details
```

Useful cooling scenario switches:

```powershell
--climate-zone 9
--cooling-flow-temp 18
--cooling-esm-flow-temp 18
--cooling-vent-debit 0.5
--cooling-free-debit 0
--cooling-esm-free-debit 2
--cooling-free-work 23 6
--cooling-free-sat 0 0
--cooling-free-sun 0 0
--holiday 8 10
```

## Current Scope

Implemented and validated to working precision:

- Envelope heating ECM savings.
- Ventilation heating and cooling component calculations.
- Component aggregators for lighting, devices, fans and pumps.
- DHW/BGV including solar collectors and solar pump rows.
- Full-project needed-energy table for Current, Normalized, and ESM states.
- Cooling core table using needed/source EI1 for ECM measure distribution.

Known not 100% finalized:

- Cooling with untreated outdoor air / night ventilation. The core methodology is implemented and follows the decompiled formulas, but the last small mismatch against EECalc remains unresolved in some scenarios. See `docs/KnownGaps.md`.
