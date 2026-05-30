# Master EECalc reconstruction roadmap

## Източници

Този roadmap използва текущите analysis документи като работна карта:

- `analysis/heating_engine_complete_oracle_report.md`
- `analysis/r3_qtr_htr_reverse_engineering.md`
- `analysis/r4_qgn_gains_reverse_engineering.md`
- `analysis/r5_gamma_ni_reverse_engineering.md`
- `analysis/r3_oracle_review_report.md`
- `analysis/validation_known_differences.md`
- `analysis/zone7_forensic_report.md`
- `analysis/docs/01_call_graph.md`
- `analysis/docs/02_method_index.md`
- `analysis/docs/03_formula_catalog.md`
- `analysis/docs/04_heating_engine.md`
- `analysis/docs/05_cooling_engine.md`
- `analysis/docs/06_building_aggregation.md`
- `analysis/docs/07_data_model.md`
- `analysis/docs/08_execution_flow.md`

Правило: EECalc decompiled C# остава source of truth. Roadmap-ът не въвежда нови формули; той подрежда блоковете за reverse engineering, oracle implementation и parity validation.

## 1. Heating zone balance - status and remaining work

**EECalc methods**

- `InputDataCalc.CalcPeriod`, `InputDataCalc.CalculateMonthlyDays`, `InputDataCalc.CalcHours`
- `HeatingAndCoolingResultCalc.Calculations` at `HeatingAndCoolingResultCalc.cs:3243`
- `CalculateActual`, `CalculateBaseLine`, `CalculateEsm`, `CalculateRef1`, `CalculateRef2`
- `CalculateParameterQve`, `CalcParameterHve`, `CalcAvgProjectTemp`, `CalcAvgNonProjectTemp`
- `CalculateParameterQtr`, `CalculateParameterHtr`, `CalculateParameterHdCurrent`, `CalculateParameterHgCurrent`
- `SumWallDirecrionsHu1`, `CalcWallDirectionParameterHu1`, `CalcCeilingsParameterHu2`, `CalcFloorsParameterHu3`
- `CalculateParameterQgn`, `CalculateParameterQgnBaseLine`, `CalculateParameterQgnEsm`
- `CalculateTransparentFsol`, `CalculateTrasparentFsol`, `CalculateNonTransparentFsol`, `CalculateNonTrasparentFsol`
- `CalculateParameterNign`, `CalculateParameterNignBaseLine`, `CalculateParameterNiEsm`, `CalculateParameterNignRef1`, `CalculateParameterNignRef2`

**Input models**

- `Section`
- `CalculationData`
- `CalculationInput`
- `BuildingZone`
- `MonthlyDays`
- `ClimateZone` via `PreferencesManager.GetClimateZoneParams`
- wall/roof/floor state objects under `Section`

**Expected outputs**

- Monthly `WorkDays`, `Saturdays`, `Sundays`, `Holydays`, `Weeks`
- `Hve`, `Qve`
- `Hd`, `Hg`, `Hu`, `Htr`, `Qtr`
- `Qgn`
- `Gamma`, `aH`, `Ni`
- raw and final `Qnd`
- `ResulNoInputsNetEnergy*`, `ResulNetEnergy*`
- debug rows for monthly heating pipeline

**Risk level**

High for parity, because small differences in days, Htr, Qgn, Gamma/Ni, or final area normalization cascade into all system-level results.

**Current status**

- R1 MonthlyDays: validated.
- R2 Qve: validated.
- R3 Htr/Qtr: oracle reviewed and validated internally.
- R4 Qgn/Gains: reverse engineering complete.
- R5 Gamma/Ni/Qnd: reverse engineering complete.
- Complete standalone monthly heating oracle exists for Actual mode.
- Known differences tracked:
  - KD-004 `SumWallDirecrionsHu1` uses NorthWalls eight times.
  - `InnerA5 * InnerA5` and `CeilingA5 * CeilingA5`.
  - Baseline current-envelope behavior.
  - Qgn is solar only.
  - Ref1/Ref2 use baseline Qgn.
  - Gamma tolerance edge bug.
  - Double Qnd assignment.

**Next task**

Run parity against EE.Doklad for Actual heating using the full oracle. Then extend the oracle to BaseLine, ESM, Ref1 and Ref2 without changing formulas.

## 2. Cooling zone balance - methods to reverse engineer

**EECalc methods**

- `HeatingAndCoolingResultCalc.CoolingCalculations` at `HeatingAndCoolingResultCalc.cs:123`
- `CalculateCoolingEnergyRef1`, `CalculateCoolingEnergyRef2`, `CalculateCoolingEnergyActual`, `CalculateCoolingEnergyBaseLine`, `CalculateCoolingEnergyESM`
- `CalculateCoolingQtrRef1`, `CalculateCoolingQtrRef2`, `CalculateCoolingQtr`, `CalculateCoolingQtrBaseLine`, `CalculateCoolingQtrESM`
- `CalculateQinfRef1`, `CalculateQinfRef2`, `CalculateQinf`, `CalculateQinfBaseLine`, `CalculateQinfESM`
- `CalculateQveRef1`, `CalculateQveRef2`, `CalculateQve`, `CalculateQveBaseLine`, `CalculateQveESM`
- `CalculateQgainRef1`, `CalculateQgainRef2`, `CalculateQgain`, `CalculateQgainBaseLine`, `CalculateQgainESM`
- `CalculateQsolRef1`, `CalculateQsolRef2`, `CalculateQsol`, `CalculateQsolBaseLine`, `CalculateQsolESM`
- `CalculateQintRef1`, `CalculateQintRef2`, `CalculateQint`, `CalculateQintBaseLine`, `CalculateQintESM`
- `CalculateQoccupants`, `CalculateQoccupantsBaseLine`, `CalculateQoccupantsESM`
- `CalculateETA`, `CalculateAc*`
- `CalculateLatentHeatsInf*`, `CalculateLatentHeatsVent*`
- `CalcAirX`, `CalcRoW`, `CalcRo`, `GetDaysHours`
- `ClaculateQfreecooling*` typo preserved from EECalc

**Input models**

- `Section.CoolingSeasons`
- `CalculationData` for cooling temperatures, humidity, infiltration, gains and ventilation
- `CalculationData lightsAndDevicesCalculationData`
- `CalculationData ventCool`
- `MonthlyDays`
- climate hourly temp/humidity via `PreferencesManager.GetClimateZoneParams(...).TempHumidity`
- solar radiation via `SolarRadiation.Months`

**Expected outputs**

- `MonthDataCoolingList`
- monthly cooling no-input energy
- monthly cooling inputs from gains and ventilation
- `ResulNoInputsNetEnergy*`
- `ResulCoolingInputs*`
- `ResulVentilationInputs*`
- `ResulNetEnergy*`
- latent heat and free-cooling intermediate rows

**Risk level**

Very high. Cooling combines sensible transmission, infiltration, internal gains, solar gains, latent humidity calculations, cooling ventilation and free cooling. It also uses hourly climate data, not only monthly averages.

**Current status**

Call graph and formula extraction exist in `analysis/docs/05_cooling_engine.md`, but no focused reverse-engineering report and no oracle exist yet.

**Next task**

Create `analysis/r6_cooling_reverse_engineering.md` covering `CoolingCalculations`, all `CalculateCoolingEnergy*` methods, `CalculateETA`, `CalculateAc*`, `Qinf`, `Qve`, `Qgain`, latent heat and free-cooling paths. Then build a minimal cooling oracle with no latent/free-cooling first.

## 3. Ventilation systems - methods to reverse engineer

**EECalc methods**

Heating ventilation:

- `VentilationHeatEnergyRef1`, `VentilationHeatEnergyRef2`, `VentilationHeatEnergyActual`, `VentilationHeatEnergyBaseLine`, `VentilationHeatEnergyESM`
- `CalculateMontlyHeatEnergyRef1`, `CalculateMontlyHeatEnergyRef2`, `CalculateMontlyHeatEnergyActual`, `CalculateMontlyHeatEnergyBaseLine`, `CalculateMontlyHeatEnergyESM`
- `CalculateAverageVentHeatTempRef1`, `CalculateAverageVentHeatTempRef2`, `CalculateAverageVentHeatTempActual`, `CalculateAverageVentHeatTempBaseLine`, `CalculateAverageVentHeatTempESM`
- `GetMonthHoursActual`, `GetMonthHoursBaseLine`, `GetMonthHoursESM`
- `CalculateVentNeededEnergyRef1`, `CalculateVentNeededEnergyRef2`, `CalculateVentNeededEnergyActual`, `CalculateVentNeededEnergyBaseLine`, `CalculateVentNeededEnergyEsm`

Cooling ventilation:

- `VentilationCoolEnergyRef1`, `VentilationCoolEnergyRef2`, `VentilationCoolEnergyActual`, `VentilationCoolEnergyBaseLine`, `VentilationCoolEnergyEsm`
- `CalculateCoolingInputsRef1`, `CalculateCoolingInputsRef2`, `CalculateCoolingInputs`, `CalculateCoolingInputsBaseLine`, `CalculateCoolingInputsESM`
- `CalculateMontlyCoolEnergyRef1`, `CalculateMontlyCoolEnergyRef2`, `CalculateMontlyCoolEnergyActual`, `CalculateMontlyCoolEnergyBaseLine`, `CalculateMontlyCoolEnergyESM`
- `CalculateWitheringEnergyRef1`, `CalculateWitheringEnergyRef2`, `CalculateWitheringEnergyActual`, `CalculateWitheringEnergyBaseLine`, `CalculateWitheringEnergyESM`
- `CalculateVentCoolNeededEnergyRef1`, `CalculateVentCoolNeededEnergyRef2`, `CalculateVentCoolNeededEnergyActual`, `CalculateVentCoolNeededEnergyBaseLine`, `CalculateVentCoolNeededEnergyEsm`

System savings:

- `CalculateVentilationHeatingSavings`
- `CalculateVentilationCoolingSavings`
- `CopyVentilationHeatingWorkingSchedule`
- `CopyVentilationCoolingWorkingSchedule`
- `GetVentilationBaseLine`, `SetVentilationBaseLine`

**Input models**

- ventilation `CalculationData`
- `HeatingCalculations` / `CoolingCalculations`
- `Section.HeatingSeasons` and `Section.CoolingSeasons`
- flow/debit fields
- flow temperature and humidity fields
- `MonthlyDays`
- climate hourly temp/humidity
- generator efficiency fields for ventilation heating/cooling

**Expected outputs**

- monthly ventilation heat/cool energy
- ventilation inputs by mode
- ventilation needed/source energy
- ventilation savings
- `ResultEnergyForHeating*`
- `ResultEnergyForCooling*`
- `ResultNeededEnergy*`, `ResultSourceEnergy*`

**Risk level**

Very high. There are separate heating and cooling ventilation systems, hourly climate dependencies, enthalpy/watering calculations, and source energy conversion.

**Current status**

Indexed in `analysis/docs/01_call_graph.md`, `02_method_index.md`, `03_formula_catalog.md`, and diagrams. No focused reverse-engineering report and no oracle.

**Next task**

Reverse engineer heating ventilation first, because it can reuse MonthlyDays and degree-hour patterns. Then reverse engineer cooling ventilation and latent/withering paths.

## 4. Heating systems / source energy - methods to reverse engineer

**EECalc methods**

- `CalculateNeededEnergyRef1`, `CalculateNeededEnergyRef2`, `CalculateNeededEnergyActual`, `CalculateNeededEnergyBaseLine`, `CalculateNeededEnergyEsm`
- `CalculateGeneratorHeatEfficiencyRef1`, `CalculateGeneratorHeatEfficiencyRef2`, `CalculateGeneratorHeatEfficiencyActual`, `CalculateGeneratorHeatEfficiencyBaseLine`, `CalculateGeneratorHeatEfficiencyEsm`
- `CalculateHeatingSavings`
- `CalculateEnergy`, `CalculateEnergyESM`
- `CreateHeatingVirtualBaseLine`, `CreateHeatingVirtualESM`
- `CheckForDifferentFuelSources`, `CheckForDifferentFuelSourcesESM`
- `CheckForFuelSavings`, `CheckForSavings`, `CheckAndCalculateNegativeSavings`
- `AddSavingsToZone`

**Input models**

- `CalculationData`
- heating result fields from zone balance
- `Fuel1`, `Fuel2`
- `Part1*`, `Part2*`
- `TransmitTempEfficiency*`
- `SupplyNetEfficiency*`
- `Automatic*`
- `EnergyManagement*`
- `GeneratorHeatEfficiency1*`, `GeneratorHeatEfficiency2*`
- savings metadata

**Expected outputs**

- `ResultSourceEnergy*`
- `ResultSourceEnergy2*`
- `ResultNeededEnergy*`
- `HeatEfficiencyGenerating*`
- savings rows and fuel source diagnostics

**Risk level**

High. This block amplifies net-energy differences through chained efficiency products and has extensive NaN/Infinity fallback behavior.

**Current status**

Formulas are indexed in `analysis/docs/03_formula_catalog.md`; no dedicated system-source-energy reverse-engineering report and no oracle.

**Next task**

Create `analysis/r7_heating_systems_source_energy_reverse_engineering.md`; implement a source-energy oracle only after heating zone parity is stable.

## 5. Domestic hot water / BGV - methods to reverse engineer

**EECalc methods**

Basic hot water:

- `HotWaterCalculationReferences`
- `HotWaterCalculationActual`
- `HotWaterCalculationBaseLine`
- `HotWaterCalculationESM`
- `CalculateHotWaterNeededEnergyRef1`
- `CalculateHotWaterNeededEnergyRef2`
- `CalculateHotWaterNeededEnergyActual`
- `CalculateHotWaterNeededEnergyBaseLine`
- `CalculateHotWaterNeededEnergyEsm`
- `CalculateGeneratorHotWaterEfficiencyRef1`
- `CalculateGeneratorHotWaterEfficiencyRef2`
- `CalculateGeneratorHotWaterEfficiencyActual`
- `CalculateGeneratorHotWaterEfficiencyBaseLine`
- `CalculateGeneratorHotWaterEfficiencyEsm`

Solar hot water:

- `CalculateHotWaterNeededPower`
- `HotWaterNeededPower`
- `HotWaterNeededPowerTotal`
- `CalculateParameterF`
- `CalculateParameterHtMonthly`
- `CalculateParameterX`
- `CalculateParameterY`
- `CalculateXwithCorrection`
- `SumCollectorsArea`
- `SetTableResults`
- `ClearTableValues`
- `SunEnergyPreferencesManager.GetClimateZoneParams`

Savings:

- `CalculateHotWaterSavings`
- `GetHotWaterBaseLine`
- `SetHotWaterBaseLine`
- `CheckForHotWaterSavings`
- `AddSavingsToBuilding`

**Input models**

- `CalculationData` for hot water
- `SunEnergyCalculationData`
- `Section`
- `CalculationInput`
- climate zone and sun-energy climate parameters
- hot water demand, generator efficiency, fuel split, collectors area and solar coefficients

**Expected outputs**

- hot water needed/source energy
- generator hot water efficiency
- solar hot water monthly table
- hot water savings
- BGV rows in building/zone aggregation tables

**Risk level**

High. Solar hot water has separate climate source and several intermediate parameters. BGV also feeds primary energy, fuels and CO2 tables.

**Current status**

Methods are indexed in `analysis/docs/01_call_graph.md` and `02_method_index.md`; no focused reverse engineering and no oracle.

**Next task**

Reverse engineer non-solar hot water first. Then reverse engineer `CalculateHotWaterNeededPower` and solar correction parameters.

## 6. Lighting - methods to reverse engineer

**EECalc methods**

- `CalculateLightsAndDevicesInputs`
- `GetLightsAndDevicesInputs`
- `SumItemsList`
- `CalcAvgMonthPower`
- `CalcWeekPower`
- `CalculatePeriodsReference`
- `CalculatePeriodsActual`
- `CalculatePeriodsBaseLine`
- `CalculatePeriodsESM`
- `CalculateLightsSavings`
- `CalculateBalancedDevicesSavings`
- `CalculateNonBalancedDevicesSavings`
- `CalculateHotWaterPumpsSavings`
- period-specific helpers such as `CalculateHeatingPeriodActual`, `CalculateCoolingPeriodActual`, `CalculateAnnualPeriodActual` and their Ref/BaseLine/ESM variants

**Input models**

- `CalculationData.Lights`
- `CalculationData.BalancedDevices`
- `CalculationData.NonBalancedDevices`
- `CalculationData.HotWaterPumps`
- `ScheduleMonth`, `MonthState`
- `MonthlyDays`
- heating/cooling/annual schedules and power values
- utilization factors from zone balance for heat-affecting inputs

**Expected outputs**

- `DevicesNeededEnergy*`
- `Power*`
- `WorkSchedule*`
- `ResulLightInputs*`
- `ResulAppliancesInputs*`
- lighting/device savings
- needed-energy table rows for lights, heat-affecting devices, non-heat-affecting devices and pumps

**Risk level**

Medium-high. Formula shape is simpler than cooling, but `weekRegime` is static mutable state and `ByMonths` changes the formula path.

**Current status**

Heating interaction is reverse-engineered in R4. Full lighting/devices period engine is only indexed.

**Next task**

Create a focused reverse-engineering report for `CalculatePeriods*`, `CalcAvgMonthPower`, `CalcWeekPower`, and device/light savings. Build oracle after heating/cooling utilization factor behavior is locked.

## 7. Results aggregation - methods to reverse engineer

**EECalc methods**

- `BuildingCalculations` at `HeatingAndCoolingResultCalc.cs:8524`
- `ZoneCalculations` at `HeatingAndCoolingResultCalc.cs:8567`
- `CalculateNetEnergyByTechnologies`
- `CalculateNetEnergyByTechnologiesBuilding`
- `CalculateNetEnergyPerArea`
- `CalculateNetWithoutInputsEnergyByTechnologies`
- `CalculateNetWithoutInputsEnergyByTechnologiesPerArea`
- `CalculateTotalsNeededEnergyTable`
- `CalculateTotalVei`
- `CalculateBuildingPowerEnergy`
- `CalculateSavings`
- `CheckForNaN`

**Input models**

- `Results`
- `BuildingZone`
- `CalculationInput`
- all completed zone calculation result fields
- total heated area and technology tables

**Expected outputs**

- zone and building net energy tables
- no-input net energy tables
- needed energy tables
- per-area normalized values
- total VEI
- building power/energy rollups

**Risk level**

High. Aggregation is straightforward only after every upstream block is proven; otherwise it hides the source of mismatch.

**Current status**

Call graph exists in `analysis/docs/06_building_aggregation.md` and `analysis/docs/building_engine.mmd`; no focused aggregation oracle.

**Next task**

Implement snapshot-only aggregation reporter first. Build oracle after heating, cooling, ventilation, BGV and lighting blocks have parity.

## 8. Primary energy / fuels / CO2 / scale

**EECalc methods**

Primary energy:

- `CalculatePrimaryEnergyByTechnologies`
- `CalculatePrimaryEnergyFuelTotal`
- `CalculatePrimaryEnergyPerArea`
- `CalculatePrimaryFuelTypeAndValuesPerArea`
- `CalculatePrimaryTotalEnergy`
- `CalculateTotalPrimaryActual`
- `CalculateTotalPrimaryBaseLine`
- `CalculateTotalPrimaryEsm`
- `CalculateTotalPrimaryRef1`
- `CalculateTotalPrimaryRef2`
- `CalculateTotalPrimaryFuelActual`
- `CalculateTotalPrimaryFuelBaseLine`
- `CalculateTotalPrimaryFuelESM`
- `CalculateTotalPrimaryFuelRef1`
- `CalculateTotalPrimaryFuelRef2`
- `GetPrimaryEnergyCoeficient`
- `GetPrimaryFuelType*`

Fuels:

- `CalculateTotalFuelEnergy`
- `CalculateTotalFuelActual`
- `CalculateTotalFuelBaseLine`
- `CalculateTotalFuelESM`
- `CalculateTotalFuelRef1`
- `CalculateTotalFuelRef2`
- `CalculateFuelActual`, `CalculateFuelBaseLine`, `CalculateFuelESM`, `CalculateFuelRef1`, `CalculateFuelRef2`
- `CalculateFuelValue*`, `CalculateFuelAreaValue*`
- `CalculateEnergySourcePowerFuel1*`, `CalculateEnergySourcePowerFuel2*`
- `GetFuelType*`, `SetFuelValue`

CO2:

- `BuildingCO2Calculations`
- `ZoneCO2Calculations`
- `CO2EnergyZoneCalculations`
- `CalculateCO2Emissions`
- `CalculateCO2EmissionsActual`, `BaseLine`, `ESM`, `Ref1`, `Ref2`
- `CalculateTotalCO2Actual`, `BaseLine`, `ESM`, `Ref1`, `Ref2`
- `Co2GetFuelTypesBuilding`
- `GetFuelTypeCo2*`

Scale:

- `BuildingTypesManager.GetClimateZoneParams`
- `SetScaleValues`
- `SetScaleType`

**Input models**

- `Results`
- `Fuel`
- primary energy coefficient tables
- CO2 coefficient tables
- scale/category tables
- all source/needed energy fields by technology

**Expected outputs**

- primary energy tables
- total primary energy by fuel and technology
- fuel quantities and per-area quantities
- CO2 emissions by fuel and total
- building scale/classification values

**Risk level**

Medium-high. These are mostly algebraic rollups, but they depend on fuel mapping and coefficient lookup behavior.

**Current status**

Call graph exists in `analysis/docs/building_engine.mmd` and `analysis/docs/06_building_aggregation.md`. No oracle.

**Next task**

Reverse engineer fuel/primary/CO2 lookup and mapping rules before implementing a rollup oracle.

## 9. Proposed order of implementation

1. Stabilize test project compilation so filtered validation tests can run.
2. Heating Actual parity against EE.Doklad using complete monthly heating oracle.
3. Extend heating oracle to BaseLine, ESM, Ref1 and Ref2.
4. Heating systems/source energy oracle.
5. Heating ventilation oracle.
6. Lighting/devices period oracle.
7. Cooling minimal sensible oracle.
8. Cooling full oracle with latent humidity, free cooling and ventilation cooling.
9. Domestic hot water non-solar oracle.
10. Solar hot water oracle.
11. Results aggregation oracle.
12. Primary energy/fuels/CO2/scale oracle.
13. End-to-end building parity snapshots.

## 10. Which blocks need oracle first

Priority order for oracles:

1. Heating BaseLine/ESM/Ref1/Ref2 extension.
2. Heating source/needed energy.
3. Heating ventilation.
4. Lighting/devices period engine.
5. Cooling zone balance.
6. Cooling ventilation.
7. Domestic hot water.
8. Results aggregation.
9. Primary energy/fuels/CO2/scale.

Rationale: each later block consumes outputs from earlier blocks. Building an aggregation or primary-energy oracle before source blocks have parity will produce noisy failures.

## 11. Which blocks need parity against EE.Doklad first

First parity targets:

1. Heating Actual monthly rows: `MonthlyDays`, climate, `Hve`, `Qve`, `Hd`, `Hg`, `Hu`, `Htr`, `Qtr`, `Qgn`, `Gamma`, `aH`, `Ni`, `RawQnd`, `FinalQnd`.
2. Heating BaseLine/ESM/Ref1/Ref2 monthly rows.
3. Heating source/needed energy: `ResultSourceEnergy*`, `ResultSourceEnergy2*`, `ResultNeededEnergy*`.
4. Heating ventilation: monthly ventilation energy and needed energy.
5. Lighting/devices: `DevicesNeededEnergy*`, `WorkSchedule*`, `ResulLightInputs*`, `ResulAppliancesInputs*`.
6. Cooling no-input balance before inputs subtraction.
7. Cooling gains/ventilation/latent/free-cooling contributions separately.
8. Domestic hot water needed/source energy.
9. Zone aggregation.
10. Building aggregation.
11. Primary energy/fuel/CO2/scale.

Parity snapshots should always include:

- fixture id
- mode: Actual/BaseLine/ESM/Ref1/Ref2
- EECalc method provenance
- expected EECalc value
- EE.Doklad actual value
- absolute and relative delta
- known-difference code where applicable

