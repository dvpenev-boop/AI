# EECalc Reverse Engineering Reference Manifest

Package folder: `exports/eecalc_reverse_engineering_reference`

This manifest lists every file included in the package and why it was included.

## Included files

- `analysis/eecalc_master_engine_specification.md` - Authoritative consolidated implementation specification for future clean-room oracles.
- `analysis/ilspy_verified_findings.md` - ILSpy-confirmed classification of aggregation findings KD-A001 and KD-A009.
- `analysis/r10_aggregation_primary_co2_class_reverse_engineering.md` - R-series reverse-engineering report for an EECalc engine area.
- `analysis/r3_oracle_review_report.md` - R-series reverse-engineering report for an EECalc engine area.
- `analysis/r3_qtr_htr_reverse_engineering.md` - R-series reverse-engineering report for an EECalc engine area.
- `analysis/r4_qgn_gains_reverse_engineering.md` - R-series reverse-engineering report for an EECalc engine area.
- `analysis/r5_gamma_ni_reverse_engineering.md` - R-series reverse-engineering report for an EECalc engine area.
- `analysis/r6_cooling_reverse_engineering.md` - R-series reverse-engineering report for an EECalc engine area.
- `analysis/r7_ventilation_edge_cases_for_oracle.md` - R-series reverse-engineering report for an EECalc engine area.
- `analysis/r7_ventilation_reverse_engineering.md` - R-series reverse-engineering report for an EECalc engine area.
- `analysis/r7_ventilation_review_addendum.md` - R-series reverse-engineering report for an EECalc engine area.
- `analysis/r8_dhw_bgv_reverse_engineering.md` - R-series reverse-engineering report for an EECalc engine area.
- `analysis/r9_lighting_devices_reverse_engineering.md` - R-series reverse-engineering report for an EECalc engine area.
- `analysis/reference-data/climate_provider_review.md` - Review of provider architecture and mode behavior; notes provider layer is not wired into production calculations.
- `analysis/reference-data/climate_temperature_xml_vs_json_audit.md` - Reference-data audit documenting XML vs JSON climate temperature differences.
- `analysis/reference-data/source_binding_audit_heating_cooling_ventilation.md` - Source binding map for climate, solar, humidity, temperature, and radiation inputs.
- `analysis/validation_known_differences.md` - Confirmed known differences registry required for parity and mode decisions.
- `README.md` - Package guide: purpose, contents, exclusions, reading order, and modes.
- `reference/eecalc-config/DefaultParams.xml` - Legacy EECalc climate/radiation/hourly/Pb configuration reference data.
- `reference/eecalc-config/DefaultSunParams.xml` - Legacy EECalc solar DHW configuration reference data.

## Excluded folders and patterns

- `reference/eecalc-decompiled/**`
- `**/*.cs`
- `**/*.dll`
- `**/*.exe`
- `**/*.pdb`
- `**/bin/**`
- `**/obj/**`
- `test-results/**`
- `debug_*.csv`
- `EE.Doklad/**`
- `EE.Doklad.Tests/**`

Included file count: 20
