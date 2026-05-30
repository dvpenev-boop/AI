# EECalc Climate Provider Review

Scope: review of the EECalc climate provider implementation only.

Reviewed files:

- `EE.Doklad/Services/EecalcClimate/IClimateDataProvider.cs`
- `EE.Doklad/Services/EecalcClimate/ISunEnergyDataProvider.cs`
- `EE.Doklad/Services/EecalcClimate/SolarRadiationData.cs`
- `EE.Doklad/Services/EecalcClimate/HourlyClimateData.cs`
- `EE.Doklad/Services/EecalcClimate/ClimateProviderMode.cs`
- `EE.Doklad/Services/EecalcClimate/LegacyEecalcXmlClimateDataProvider.cs`
- `EE.Doklad/Services/EecalcClimate/LegacyEecalcXmlSunEnergyDataProvider.cs`
- `EE.Doklad/Services/EecalcClimate/CorrectedJsonClimateDataProvider.cs`
- `EE.Doklad/Services/EecalcClimate/EecalcClimateServiceCollectionExtensions.cs`
- `EE.Doklad.Tests/EecalcClimateProviderTests.cs`

No production calculation files were modified during this review.

## Summary

Review result: PASS with one provider documentation fix.

The implementation adds provider abstractions and tests without rewiring existing formulas or calculation behavior. `LegacyEECalcStrict` preserves the legacy XML values, including KD-DATA-001. `LegacyEECalcCorrectedData` only changes January monthly average temperatures for zones 1-3. `CurrentOrdinance` reads `climate_zones.json` through the existing `JsonClimateRepository` path and does not read `DefaultParams.xml`.

The only issue found was that the temporary hourly fallback in `CorrectedJsonClimateDataProvider` was not documented directly in provider code. A comment was added there; no formula or calculation behavior was changed.

## Check Results

| Check | Result | Evidence |
| --- | --- | --- |
| 1. No formulas were changed | PASS | New code is isolated under `EE.Doklad/Services/EecalcClimate`; existing heating/cooling/ventilation formula files were not edited. |
| 2. No calculation behavior was rewired yet | PASS | `rg` shows `EE.Doklad.Services.EecalcClimate` is referenced by provider files and tests only; no calculator/viewmodel consumes the new provider yet. |
| 3. `LegacyEECalcStrict` preserves XML exactly | PASS | `LegacyEecalcXmlClimateDataProvider` parses `DefaultParams.xml` values directly; tests assert January zone values `-1.9`, `-0.5`, `-0.1` and hourly XML values. |
| 4. `LegacyEECalcCorrectedData` only corrects KD-DATA-001 | PASS | Correction branch exists only in `GetMonthlyAvgTemp`, only when `month == Month.January`, only for `zoneId` 1, 2, and 3. Solar, hourly data, and `Pb` remain XML-backed. |
| 5. `CurrentOrdinance` does not read `DefaultParams.xml` | PASS | `CorrectedJsonClimateDataProvider` depends on `IClimateRepository` / `JsonClimateRepository`, whose default resource is `EE.Doklad.Data.climate_zones.json`; it has no reference to `DefaultParams.xml`. |
| 6. Zone mapping is explicit | PASS | XML providers use `ToXmlNumber(zoneId)` with validation and `return zoneId - 1`; lookup is by XML `ClimateZone.Number`, not array index. JSON provider looks up by `Zones[].Id`. |
| 7. NE/SE/SW/NW are derived, not loaded | PASS | `SolarRadiationData` stores only constructor values `N`, `E`, `S`, `W`, `H`; diagonal orientations are expression-bodied properties. |
| 8. Hourly fallback in `CurrentOrdinance` is documented as temporary limitation | PASS after fix | `CorrectedJsonClimateDataProvider.GetHourlyClimateData` now documents that `climate_zones.json` has no hourly profile and uses a 24-hour monthly fallback until an authoritative hourly source exists. |

## Mode Details

### LegacyEECalcStrict

Source:

- climate: `reference/eecalc-config/DefaultParams.xml`
- sun energy: `reference/eecalc-config/DefaultSunParams.xml`

Behavior:

- no corrections.
- `GetMonthlyAvgTemp(1, January) == -1.9`.
- `GetMonthlyAvgTemp(2, January) == -0.5`.
- `GetMonthlyAvgTemp(3, January) == -0.1`.
- hourly temperature/humidity and `Pb` are loaded from `DefaultParams.xml`.

### LegacyEECalcCorrectedData

Source:

- XML structure and all non-corrected data from `DefaultParams.xml`.

Only correction:

| ZoneId | Month | Corrected value |
| --- | --- | ---: |
| 1 | January | 1.9 |
| 2 | January | 0.5 |
| 3 | January | 0.1 |

No correction is applied to:

- February-December temperatures.
- solar `N/E/S/W/H`.
- hourly temperature/humidity.
- `Pb`.
- `DefaultSunParams.xml`.

### CurrentOrdinance

Source:

- climate: `EE.Doklad/Data/climate_zones.json` via embedded `JsonClimateRepository`.

Behavior:

- monthly average temperature and solar radiation come from JSON.
- `Pb` comes from `ClimateZoneData.GetEffectiveBarometricPressure()`.
- hourly climate data is a temporary 24-hour monthly fallback because `climate_zones.json` has monthly averages, not an hourly profile.
- no `DefaultParams.xml` access.

## Registration Review

`EecalcClimateServiceCollectionExtensions.AddEecalcClimateProviders(mode)` registers:

- `IClimateDataProvider` according to `ClimateProviderMode`.
- `ISunEnergyDataProvider` as `LegacyEecalcXmlSunEnergyDataProvider`.

This adds DI registration capability but does not wire existing calculations to the providers. That satisfies the current phase requirement: provider layer exists, formulas are not changed, and parity behavior is not modified.

## Tests Reviewed

Provider tests cover:

- `LegacyEECalcStrict_ReturnsXmlValues`
- `LegacyEECalcCorrectedData_CorrectsJanuaryValues`
- `CurrentOrdinance_ReturnsJsonValues`
- `OrientationMapping_ReturnsExpectedValues`

The tests validate KD-DATA-001 strict/corrected behavior, JSON values for current ordinance mode, explicit orientation derivation, `Pb`, and representative hourly XML/fallback data.

## Residual Notes

The provider namespace intentionally uses `EE.Doklad.Services.EecalcClimate` because the application already has unrelated climate-provider interfaces under `EE.Doklad.Models.Climate` and `EE.Doklad.Services.Climate`. This avoids changing existing UI/service behavior during the provider introduction phase.

The `CurrentOrdinance` hourly fallback is not suitable for final R6/R7 hourly parity or ordinance-accurate psychrometric calculations. It is an explicit temporary limitation pending an authoritative current hourly climate source.
