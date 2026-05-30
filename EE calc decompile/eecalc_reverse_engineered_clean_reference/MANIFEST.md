# Manifest

| File | Reason included | Category |
| --- | --- | --- |
| analysis/eecalc_master_engine_specification.md | Core reverse-engineering analysis document. | AnalysisDoc |
| analysis/ilspy_verified_findings.md | Core reverse-engineering analysis document. | AnalysisDoc |
| analysis/r10_aggregation_primary_co2_class_reverse_engineering.md | R-series reverse-engineering analysis for reconstructed EECalc behavior. | AnalysisDoc |
| analysis/r3_oracle_review_report.md | R-series reverse-engineering analysis for reconstructed EECalc behavior. | AnalysisDoc |
| analysis/r3_qtr_htr_reverse_engineering.md | R-series reverse-engineering analysis for reconstructed EECalc behavior. | AnalysisDoc |
| analysis/r4_qgn_gains_reverse_engineering.md | R-series reverse-engineering analysis for reconstructed EECalc behavior. | AnalysisDoc |
| analysis/r5_gamma_ni_reverse_engineering.md | R-series reverse-engineering analysis for reconstructed EECalc behavior. | AnalysisDoc |
| analysis/r6_cooling_reverse_engineering.md | R-series reverse-engineering analysis for reconstructed EECalc behavior. | AnalysisDoc |
| analysis/r7_ventilation_edge_cases_for_oracle.md | R-series reverse-engineering analysis for reconstructed EECalc behavior. | AnalysisDoc |
| analysis/r7_ventilation_reverse_engineering.md | R-series reverse-engineering analysis for reconstructed EECalc behavior. | AnalysisDoc |
| analysis/r7_ventilation_review_addendum.md | R-series reverse-engineering analysis for reconstructed EECalc behavior. | AnalysisDoc |
| analysis/r8_dhw_bgv_reverse_engineering.md | R-series reverse-engineering analysis for reconstructed EECalc behavior. | AnalysisDoc |
| analysis/r9_lighting_devices_reverse_engineering.md | R-series reverse-engineering analysis for reconstructed EECalc behavior. | AnalysisDoc |
| analysis/reference-data/climate_provider_review.md | Reference-data audit supporting strict legacy behavior and data-source decisions. | AnalysisDoc |
| analysis/reference-data/climate_temperature_xml_vs_json_audit.md | Reference-data audit supporting strict legacy behavior and data-source decisions. | AnalysisDoc |
| analysis/reference-data/source_binding_audit_heating_cooling_ventilation.md | Reference-data audit supporting strict legacy behavior and data-source decisions. | AnalysisDoc |
| analysis/validation_known_differences.md | Core reverse-engineering analysis document. | AnalysisDoc |
| clean_code/climate/ClimateProviderMode.cs | Mode enum documenting LegacyEECalcStrict versus corrected/current provider behavior. | ClimateProvider |
| clean_code/climate/CorrectedJsonClimateDataProvider.cs | Clean climate provider implementation used for corrected JSON comparison mode. | ClimateProvider |
| clean_code/climate/EecalcDataPathResolver.cs | Path resolver needed by the extracted EECalc XML climate/sun providers. | ClimateProvider |
| clean_code/climate/HourlyClimateData.cs | Climate DTO needed by climate and ventilation formulas. | Model |
| clean_code/climate/IClimateDataProvider.cs | Provider interface for strict EECalc XML climate data access. | ClimateProvider |
| clean_code/climate/ISunEnergyDataProvider.cs | Provider interface for strict EECalc XML sun-energy data access. | ClimateProvider |
| clean_code/climate/LegacyEecalcXmlClimateDataProvider.cs | Strict XML climate provider preserving DefaultParams.xml values including known January data issue. | ClimateProvider |
| clean_code/climate/LegacyEecalcXmlSunEnergyDataProvider.cs | Strict XML sun-energy provider backed by DefaultSunParams.xml. | ClimateProvider |
| clean_code/climate/Month.cs | Month enum required by extracted climate/sun providers. | Model |
| clean_code/climate/SolarRadiationData.cs | Solar radiation DTO required by strict XML sun provider. | Model |
| clean_code/formulas/EecalcHtrQtrOracle.cs | R3 transmission and degree-hours formula code. | CleanFormulaCode |
| clean_code/formulas/EecalcMonthlyCoolingOracle.cs | R6 cooling balance formula code. | CleanFormulaCode |
| clean_code/formulas/EecalcMonthlyDaysOracle.cs | R1/R2 monthly calendar and degree-hours formula code. | CleanFormulaCode |
| clean_code/formulas/EecalcMonthlyHeatingOracle.cs | R4/R5 monthly gains and heating balance formula code. | CleanFormulaCode |
| clean_code/formulas/EecalcQveOracle.cs | Ventilation heat-transfer formula code used by heating balance. | CleanFormulaCode |
| clean_code/formulas/EECalcR7R8R9Oracles.cs | R7 ventilation, R8 DHW/BGV, and R9 lighting/devices formula code without test methods. | CleanFormulaCode |
| clean_code/legacy/EECalcLegacyAggregation.cs | Legacy mappings for KD-A001 duplicate Fuel1 total and KD-A009 Fuel1/Fuel8 report bucket inversion. | LegacyMapping |
| clean_code/models/EecalcEnvelopeFixture.cs | Envelope model used by R3/R5/R6 formula classes. | Model |
| clean_code/models/EecalcEnvelopeSnapshotRow.cs | Envelope row DTO used by transmission formula extraction. | Model |
| clean_code/models/EECalcFullOracleModels.cs | Input/state/math DTOs and helper formulas required by R7-R10 extracted formula code. | Model |
| clean_code/models/EecalcMonthlySnapshotRow.cs | Monthly snapshot DTO supporting extracted heating/cooling formula rows. | Model |
| clean_code/models/EecalcValidationFixture.cs | Minimal fixture/model DTO required by extracted R1-R6 formula classes. | Model |
| EXCLUDED.md | Documents intentionally omitted test, parity, fixture, CSV, and generated/debug material. | AnalysisDoc |
| README.md | Package overview and strict-mode caveats requested for the clean export. | AnalysisDoc |
| reference/eecalc-config/DefaultParams.xml | Original EECalc DefaultParams XML used by strict legacy climate/data mode. | ConfigXml |
| reference/eecalc-config/DefaultSunParams.xml | Original EECalc DefaultSunParams XML used by strict legacy sun/radiation mode. | ConfigXml |
| reference/eecalc-decompiled/EECalcCore.Calculations.BuildingTypesManager.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
| reference/eecalc-decompiled/EECalcCore.Calculations.Calculator.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
| reference/eecalc-decompiled/EECalcCore.Calculations.DataRow.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
| reference/eecalc-decompiled/EECalcCore.Calculations.InputDataCalc.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
| reference/eecalc-decompiled/EECalcCore.Calculations.MonthData.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
| reference/eecalc-decompiled/EECalcCore.Calculations.MonthDataCooling.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
| reference/eecalc-decompiled/EECalcCore.Calculations.MonthlyDays.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
| reference/eecalc-decompiled/EECalcCore.Calculations.PreferencesManager.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
| reference/eecalc-decompiled/EECalcCore.Calculations.SavingsData.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
| reference/eecalc-decompiled/EECalcCore.Calculations.SunEnergyPreferencesManager.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
| reference/eecalc-decompiled/EECalcCore.Calculations.SunMonth.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
| reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.BaseLineData.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
| reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.FloorTableCalc.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
| reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
| reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
| reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.TempBridgeCalc.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
| reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
| reference/eecalc-decompiled/EECalcCore.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
| reference/eecalc-decompiled/EECalcCore.Preferences.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
| reference/eecalc-decompiled/EECalcCore.SunPreferences.cs | Decompiled EECalc reference source used to reconstruct formulas and behavior. | DecompiledReference |
