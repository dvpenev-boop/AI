# 01 Call Graph

Internal edges are resolved from `Class.Method(...)` where possible. Unqualified and extension-style calls are resolved by method name; overloaded same-name methods can therefore appear as multiple possible internal targets.

- Methods: `734`
- Internal edges: `1182`

## Roots / Public Entrypoints

- `BuildingTypesManager.GetClimateZoneParams` (`reference/eecalc-decompiled/EECalcCore.Calculations.BuildingTypesManager.cs:27`) -> _None_
- `Calculator.Calculate` (`reference/eecalc-decompiled/EECalcCore.Calculations.Calculator.cs:12`) -> _None_
- `Calculator.AcumulateWeight` (`reference/eecalc-decompiled/EECalcCore.Calculations.Calculator.cs:16`) -> _None_
- `Calculator.SumFields` (`reference/eecalc-decompiled/EECalcCore.Calculations.Calculator.cs:43`) -> _None_
- `FloorTableCalc.CalculateFloorArea` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.FloorTableCalc.cs:15`) -> `Calculator.SumFields`
- `FloorTableCalc.CalculateFloorU` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.FloorTableCalc.cs:20`) -> `Calculator.AcumulateWeight`
- `FloorTableCalc.CalculateOtherFloorArea` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.FloorTableCalc.cs:26`) -> `Calculator.SumFields`
- `FloorTableCalc.CalculateOtherFloorU` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.FloorTableCalc.cs:31`) -> `Calculator.AcumulateWeight`
- `FloorTableCalc.SumX` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.FloorTableCalc.cs:37`) -> `Calculator.SumFields`
- `FloorTableCalc.SumL` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.FloorTableCalc.cs:51`) -> `Calculator.SumFields`
- `HeatingAndCoolingResultCalc.CoolingCalculations` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:123`) -> `HeatingAndCoolingResultCalc.CalculateCoolingEnergyActual`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef1`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef2`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultActual` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:137`) -> `InputDataCalc.CalcHours`
- `HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:145`) -> `InputDataCalc.CalcHours`
- `HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultEsm` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:153`) -> `InputDataCalc.CalcHours`
- `HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultReferences` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:161`) -> `InputDataCalc.CalcHours`
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingRef1` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2255`) -> `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursBaseLine`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingRef2` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2271`) -> `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursBaseLine`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingActual` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2287`) -> `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursActual`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2303`) -> `HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursBaseLine`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingEsm` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2320`) -> `HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursEsm`, `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursEsm`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingRef1` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2338`) -> `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursBaseLine`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingRef2` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2365`) -> `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursBaseLine`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingActual` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2391`) -> `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursActual`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursActual`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2417`) -> `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursBaseLine`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingEsm` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2443`) -> `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursEsm`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursEsm`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.CalculateUouterWallsCurrent` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2554`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateUouterWallsEsm` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2583`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateUinnerWallsCurrent` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2611`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateUinnerWallsEsm` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2640`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateUwindowsCurrent` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2668`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateUwindowsEsm` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2700`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGcurrent` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2731`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGesm` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2763`) -> _None_
- `HeatingAndCoolingResultCalc.GetUnonTrasparentRoof` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2794`) -> _None_
- `HeatingAndCoolingResultCalc.GetUceiling` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2801`) -> _None_
- `HeatingAndCoolingResultCalc.GetUfloor` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2808`) -> _None_
- `HeatingAndCoolingResultCalc.GetUotherFloor` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2815`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateNetEnergy` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2822`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyRef1` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2831`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyRef2` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2848`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyActual` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2865`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2882`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyEsm` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2899`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyRef1` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2917`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyRef2` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2933`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyActual` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2949`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2965`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyEsm` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2981`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyRef1` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2997`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyRef2` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3013`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyActual` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3029`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3045`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyESM` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3061`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyRef1` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3077`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyRef2` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3093`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyActual` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3109`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3125`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyESM` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3141`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingRef1` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3157`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingRef2` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3174`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingActual` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3191`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3208`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingESM` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3225`) -> _None_
- `HeatingAndCoolingResultCalc.Calculations` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3243`) -> `HeatingAndCoolingResultCalc.ApplyValuesToTempSectionRef1`, `HeatingAndCoolingResultCalc.ApplyValuesToTempSectionRef2`, `HeatingAndCoolingResultCalc.CalculateActual`, `HeatingAndCoolingResultCalc.CalculateBaseLine`, `HeatingAndCoolingResultCalc.CalculateEsm`, `HeatingAndCoolingResultCalc.CalculateLightsAndDevicesInputs`, `HeatingAndCoolingResultCalc.CalculateRef1`, `HeatingAndCoolingResultCalc.CalculateRef2`, `HeatingAndCoolingResultCalc.CheckForNaN`, `HeatingAndCoolingResultCalc.GetLightsAndDevicesInputs`, ... (+5)
- `HeatingAndCoolingResultCalc.GetWeekHoursResultReferences` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3402`) -> `InputDataCalc.CalcHours`
- `HeatingAndCoolingResultCalc.GetWeekHoursResultActual` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3419`) -> `InputDataCalc.CalcHours`
- `HeatingAndCoolingResultCalc.GetWeekHoursResultBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3435`) -> `InputDataCalc.CalcHours`
- `HeatingAndCoolingResultCalc.GetWeekHoursResultEsm` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3451`) -> `InputDataCalc.CalcHours`
- `HeatingAndCoolingResultCalc.HotWaterCalculationReferences` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4767`) -> _None_
- `HeatingAndCoolingResultCalc.HotWaterCalculationActual` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4778`) -> _None_
- `HeatingAndCoolingResultCalc.HotWaterCalculationBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4786`) -> _None_
- `HeatingAndCoolingResultCalc.HotWaterCalculationESM` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4794`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorHotWaterEfficiencyRef1` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4802`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorHotWaterEfficiencyRef2` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4818`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorHotWaterEfficiencyActual` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4834`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorHotWaterEfficiencyBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4850`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorHotWaterEfficiencyEsm` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4866`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededEnergyRef1` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4882`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededEnergyRef2` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4899`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededEnergyActual` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4916`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededEnergyBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4933`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededEnergyEsm` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4950`) -> _None_
- `HeatingAndCoolingResultCalc.CalculatePeriodsReference` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4968`) -> `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2`
- `HeatingAndCoolingResultCalc.CalculatePeriodsActual` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4978`) -> `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActual`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActual`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActual`
- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4985`) -> `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLine`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLine`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLine`
- `HeatingAndCoolingResultCalc.CalculatePeriodsESM` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4992`) -> `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESM`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESM`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESM`
- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceBalanced` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4999`) -> `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1Balanced`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2Balanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1Balanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2Balanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1Balanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2Balanced`
- `HeatingAndCoolingResultCalc.CalculatePeriodsActualBalanced` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5009`) -> `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActualBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualBalanced`
- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLineBalanced` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5016`) -> `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLineBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineBalanced`
- `HeatingAndCoolingResultCalc.CalculatePeriodsESMBalanced` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5023`) -> `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESMBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMBalanced`
- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceNonBalanced` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5030`) -> `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1NonBalanced`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2NonBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1NonBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2NonBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1NonBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2NonBalanced`
- `HeatingAndCoolingResultCalc.CalculatePeriodsActualNonBalanced` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5040`) -> `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActualNonBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualNonBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualNonBalanced`
- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLineNonBalanced` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5047`) -> `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLineNonBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineNonBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineNonBalanced`
- `HeatingAndCoolingResultCalc.CalculatePeriodsESMNonBalanced` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5054`) -> `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESMNonBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMNonBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMNonBalanced`
- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceHotWaterPumps` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5061`) -> `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1HotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2HotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1HotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2HotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1HotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2HotWaterPumps`
- `HeatingAndCoolingResultCalc.CalculatePeriodsActualHotWaterPumps` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5071`) -> `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActualHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualHotWaterPumps`
- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLineHotWaterPumps` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5078`) -> `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLineHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineHotWaterPumps`
- `HeatingAndCoolingResultCalc.CalculatePeriodsESMHotWaterPumps` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5085`) -> `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESMHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMHotWaterPumps`
- `HeatingAndCoolingResultCalc.CalculateZonePowerEnergy` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6836`) -> `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1Area`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1BaseLine`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1BaseLineArea`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1Esm`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1EsmArea`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2Area`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2BaseLine`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2BaseLineArea`, ... (+4)
- `HeatingAndCoolingResultCalc.CalculateBuildingPowerEnergy` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6854`) -> `HeatingAndCoolingResultCalc.CalculateBuildingHeatingPower`, `HeatingAndCoolingResultCalc.CalculateBuildingSourcePower`, `HeatingAndCoolingResultCalc.CalculateZonePowerEnergy`
- `HeatingAndCoolingResultCalc.CalculateBuildingHeatingPower` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6864`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateBuildingSourcePower` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6878`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateHeatingPower` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6915`) -> `HeatingAndCoolingResultCalc.CalcParameterHve`, `HeatingAndCoolingResultCalc.CalcParameterHveBaseLine`, `HeatingAndCoolingResultCalc.CalcParameterHveEsm`, `HeatingAndCoolingResultCalc.CalculateParameterHtr`, `HeatingAndCoolingResultCalc.CalculateParameterHtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterHtrEsm`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6941`) -> `HeatingAndCoolingResultCalc.CalculateFuelValue`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1Area` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6981`) -> `HeatingAndCoolingResultCalc.CalculateFuelAreaValue`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7021`) -> `HeatingAndCoolingResultCalc.CalculateFuel2Value`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2Area` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7061`) -> `HeatingAndCoolingResultCalc.CalculateFuel2AreaValue`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1BaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7101`) -> `HeatingAndCoolingResultCalc.CalculateFuelValueBaseLine`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1BaseLineArea` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7141`) -> `HeatingAndCoolingResultCalc.CalculateFuelAreaValueBaseLine`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2BaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7181`) -> `HeatingAndCoolingResultCalc.CalculateFuel2ValueBaseLine`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2BaseLineArea` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7221`) -> `HeatingAndCoolingResultCalc.CalculateFuel2AreaValueBaseLine`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1Esm` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7261`) -> `HeatingAndCoolingResultCalc.CalculateFuelValueEsm`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1EsmArea` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7301`) -> `HeatingAndCoolingResultCalc.CalculateFuelAreaValueEsm`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2Esm` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7341`) -> `HeatingAndCoolingResultCalc.CalculateFuel2ValueEsm`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2EsmArea` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7381`) -> `HeatingAndCoolingResultCalc.CalculateFuel2AreaValueEsm`
- `HeatingAndCoolingResultCalc.ClearFuelCellsPowerTable` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7421`) -> _None_
- `HeatingAndCoolingResultCalc.ClearFuelCellsPowerTableBuilding` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7491`) -> _None_
- `HeatingAndCoolingResultCalc.BuildingCalculations` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8524`) -> `HeatingAndCoolingResultCalc.BuildingCO2Calculations`, `HeatingAndCoolingResultCalc.CalculateBuildingPowerEnergy`, `HeatingAndCoolingResultCalc.CalculateNetEnergyByTechnologiesBuilding`, `HeatingAndCoolingResultCalc.CalculateNetEnergyPerArea`, `HeatingAndCoolingResultCalc.CalculateNetWithoutInputsEnergyByTechnologies`, `HeatingAndCoolingResultCalc.CalculateNetWithoutInputsEnergyByTechnologiesPerArea`, `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyByTechnologies`, `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyFuelTotal`, `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyPerArea`, `HeatingAndCoolingResultCalc.CalculatePrimaryFuelTypeAndValuesPerArea`, ... (+20)
- `HeatingAndCoolingResultCalc.ZoneCalculations` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8567`) -> `HeatingAndCoolingResultCalc.CalculateNetEnergyByTechnologies`, `HeatingAndCoolingResultCalc.CalculateNetWithoutInputsEnergyByTechnologies`, `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyByTechnologies`, `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyFuelTotal`, `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyPerArea`, `HeatingAndCoolingResultCalc.CalculatePrimaryFuelTypeAndValuesPerArea`, `HeatingAndCoolingResultCalc.CalculatePrimaryTotalEnergy`, `HeatingAndCoolingResultCalc.CalculateTotalFuelEnergy`, `HeatingAndCoolingResultCalc.CalculateTotalVei`, `HeatingAndCoolingResultCalc.CalculateTotalsNeededEnergyTable`, ... (+10)
- `HeatingAndCoolingResultCalc.CalculateHeatingSavings` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11208`) -> `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CalculateEnergy`, `HeatingAndCoolingResultCalc.CalculateEnergyESM`, `HeatingAndCoolingResultCalc.CalculateUsavingType`, `HeatingAndCoolingResultCalc.CalculateUsavingTypeESM`, `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSources`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSourcesESM`, `HeatingAndCoolingResultCalc.CheckForFuelSavings`, `HeatingAndCoolingResultCalc.CheckForSavings`, ... (+9)
- `HeatingAndCoolingResultCalc.CheckForDifferentFuelSources` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11314`) -> _None_
- `HeatingAndCoolingResultCalc.CheckForDifferentFuelSourcesESM` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11362`) -> _None_
- `HeatingAndCoolingResultCalc.CreateHeatingVirtualBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11410`) -> `HeatingAndCoolingResultCalc.CalculateEnergy`, `HeatingAndCoolingResultCalc.GetBaseLine`, `HeatingAndCoolingResultCalc.SetBaseLine`
- `HeatingAndCoolingResultCalc.CreateHeatingVirtualESM` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11433`) -> `HeatingAndCoolingResultCalc.CalculateEnergy`, `HeatingAndCoolingResultCalc.GetESM`, `HeatingAndCoolingResultCalc.SetESM`
- `HeatingAndCoolingResultCalc.CalculateCoolingSavings` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11456`) -> `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM`, `HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingBaseLine`, `HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingESM`, `HeatingAndCoolingResultCalc.CalculateUsavingType`, `HeatingAndCoolingResultCalc.CalculateUsavingTypeESM`, `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSources`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSourcesESM`, ... (+12)
- `HeatingAndCoolingResultCalc.CreateCoolingVirtualBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11560`) -> `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine`, `HeatingAndCoolingResultCalc.GetBaseLine`, `HeatingAndCoolingResultCalc.SetBaseLine`
- `HeatingAndCoolingResultCalc.CreateCoolingVirtualESM` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11578`) -> `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM`, `HeatingAndCoolingResultCalc.GetESM`, `HeatingAndCoolingResultCalc.SetESM`
- `HeatingAndCoolingResultCalc.CalculateVentilationHeatingSavings` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11596`) -> `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyBaseLine`, `HeatingAndCoolingResultCalc.CalculateVentNeededEnergyBaseLine`, `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSources`, `HeatingAndCoolingResultCalc.CheckForFuelSavings`, `HeatingAndCoolingResultCalc.CheckForVentilationSavings`, `HeatingAndCoolingResultCalc.CopyVentilationHeatingWorkingSchedule`, `HeatingAndCoolingResultCalc.GetVentilationBaseLine`, `HeatingAndCoolingResultCalc.SetVentilationBaseLine`, ... (+2)
- `HeatingAndCoolingResultCalc.CalculateVentilationCoolingSavings` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11694`) -> `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyBaseLine`, `HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyBaseLine`, `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSources`, `HeatingAndCoolingResultCalc.CheckForFuelSavings`, `HeatingAndCoolingResultCalc.CheckForVentilationSavings`, `HeatingAndCoolingResultCalc.CopyVentilationCoolingWorkingSchedule`, `HeatingAndCoolingResultCalc.GetVentilationBaseLine`, `HeatingAndCoolingResultCalc.SetVentilationBaseLine`, ... (+2)
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingSavings` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11792`) -> `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings`, `HeatingAndCoolingResultCalc.CheckHeatingForFansAndPumpsSavings`, `HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursEsm`, `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursEsm`, `HeatingAndCoolingResultCalc.SetHeatingFansAndPumpsSavingsValues`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.SetHeatingFansAndPumpsSavingsValues` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11844`) -> `HeatingAndCoolingResultCalc.GetSaving`
- `HeatingAndCoolingResultCalc.CheckHeatingForFansAndPumpsSavings` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11852`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingSavings` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11902`) -> `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CheckCoolingForFansAndPumpsSavings`, `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursEsm`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursEsm`, `HeatingAndCoolingResultCalc.SetCoolingFansAndPumpsSavingsValues`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.SetCoolingFansAndPumpsSavingsValues` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11964`) -> `HeatingAndCoolingResultCalc.GetSaving`
- `HeatingAndCoolingResultCalc.CheckCoolingForFansAndPumpsSavings` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11974`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateLightsSavings` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12046`) -> `HeatingAndCoolingResultCalc.CalculatePeriod`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.CalculateBalancedDevicesSavings` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12067`) -> `HeatingAndCoolingResultCalc.CalculatePeriod`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.CalculateNonBalancedDevicesSavings` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12088`) -> `HeatingAndCoolingResultCalc.CalculatePeriod`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.CalculateHotWaterPumpsSavings` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12109`) -> `HeatingAndCoolingResultCalc.CalculatePeriod`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.CalculatePeriod` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12127`) -> `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings`, `HeatingAndCoolingResultCalc.CheckLightsAndDevicesSavings`, `HeatingAndCoolingResultCalc.SetLightsAndDevicesSavingsvalues`
- `HeatingAndCoolingResultCalc.SetLightsAndDevicesSavingsvalues` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12197`) -> `HeatingAndCoolingResultCalc.GetSaving`
- `HeatingAndCoolingResultCalc.CheckLightsAndDevicesSavings` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12203`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateHotWaterSavings` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12231`) -> `HeatingAndCoolingResultCalc.AddSavingsToBuilding`, `HeatingAndCoolingResultCalc.CalculateGeneratorHotWaterEfficiencyBaseLine`, `HeatingAndCoolingResultCalc.CalculateHotWaterNeededEnergyBaseLine`, `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSources`, `HeatingAndCoolingResultCalc.CheckForFuelSavings`, `HeatingAndCoolingResultCalc.CheckForHotWaterSavings`, `HeatingAndCoolingResultCalc.GetHotWaterBaseLine`, `HeatingAndCoolingResultCalc.HotWaterCalculationBaseLine`, `HeatingAndCoolingResultCalc.SetHotWaterBaseLine`, ... (+1)
- `HeatingAndCoolingResultCalc.SetScaleType` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14386`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededPower` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14431`) -> `HeatingAndCoolingResultCalc.CalculateParameterF`, `HeatingAndCoolingResultCalc.CalculateParameterHtMonthly`, `HeatingAndCoolingResultCalc.CalculateParameterX`, `HeatingAndCoolingResultCalc.CalculateParameterY`, `HeatingAndCoolingResultCalc.CalculateXwithCorrection`, `HeatingAndCoolingResultCalc.ClearTableValues`, `HeatingAndCoolingResultCalc.HotWaterNeededPower`, `HeatingAndCoolingResultCalc.HotWaterNeededPowerTotal`, `HeatingAndCoolingResultCalc.SetTableResults`, `HeatingAndCoolingResultCalc.SumCollectorsArea`, ... (+2)
- `HeatingAndCoolingResultCalc.VentilationCoolEnergyRef1` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14814`) -> `HeatingAndCoolingResultCalc.CalculateCoolingInputsRef1`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyRef1`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyRef1`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.VentilationCoolEnergyRef2` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14842`) -> `HeatingAndCoolingResultCalc.CalculateCoolingInputsRef2`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyRef2`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyRef2`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.VentilationCoolEnergyActual` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14870`) -> `HeatingAndCoolingResultCalc.CalculateCoolingInputs`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyActual`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyActual`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.VentilationCoolEnergyBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14898`) -> `HeatingAndCoolingResultCalc.CalculateCoolingInputsBaseLine`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyBaseLine`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.VentilationCoolEnergyEsm` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14926`) -> `HeatingAndCoolingResultCalc.CalculateCoolingInputsESM`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyESM`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyESM`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.GetWeekHoursCoolingReferences` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14954`) -> `InputDataCalc.CalcHours`
- `HeatingAndCoolingResultCalc.GetWeekHoursCoolingActual` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14962`) -> `InputDataCalc.CalcHours`
- `HeatingAndCoolingResultCalc.GetWeekHoursCoolingBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14970`) -> `InputDataCalc.CalcHours`
- `HeatingAndCoolingResultCalc.GetWeekHoursCoolingEsm` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14978`) -> `InputDataCalc.CalcHours`
- `HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyRef1` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14986`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyRef2` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15005`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyActual` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15024`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15043`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyEsm` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15062`) -> _None_
- `HeatingAndCoolingResultCalc.VentilationHeatEnergyRef1` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15642`) -> `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef1`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.VentilationHeatEnergyRef2` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15676`) -> `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef2`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.VentilationHeatEnergyActual` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15710`) -> `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempActual`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyActual`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.VentilationHeatEnergyBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15753`) -> `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempBaseLine`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyBaseLine`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.VentilationHeatEnergyESM` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15796`) -> `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempESM`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyESM`, `InputDataCalc.CalcPeriod`
- `HeatingAndCoolingResultCalc.CalculateVentNeededEnergyRef1` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15839`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateVentNeededEnergyRef2` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15871`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateVentNeededEnergyActual` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15903`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateVentNeededEnergyBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15935`) -> _None_
- `HeatingAndCoolingResultCalc.CalculateVentNeededEnergyEsm` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15967`) -> _None_
- `HeatingAndCoolingResultCalc.GetWeekHoursReferences` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16000`) -> `InputDataCalc.CalcHours`
- `HeatingAndCoolingResultCalc.GetWeekHoursActual` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16017`) -> `InputDataCalc.CalcHours`
- `HeatingAndCoolingResultCalc.GetWeekHoursBaseLine` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16033`) -> `InputDataCalc.CalcHours`
- `HeatingAndCoolingResultCalc.GetWeekHoursESM` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16049`) -> `InputDataCalc.CalcHours`
- `InputDataCalc.CalcPeriod` (`reference/eecalc-decompiled/EECalcCore.Calculations.InputDataCalc.cs:13`) -> `InputDataCalc.CalculateMonthlyDays`
- `InputDataCalc.CalcHours` (`reference/eecalc-decompiled/EECalcCore.Calculations.InputDataCalc.cs:43`) -> _None_
- `InputDataCalc.CalculateMonthlyDays` (`reference/eecalc-decompiled/EECalcCore.Calculations.InputDataCalc.cs:48`) -> `InputDataCalc.GetHollydays`, `InputDataCalc.GetWeeksInMonth`
- `MonthlyDays.MonthlyDays` (`reference/eecalc-decompiled/EECalcCore.Calculations.MonthlyDays.cs:23`) -> _None_
- `MonthlyDays.MonthlyDays` (`reference/eecalc-decompiled/EECalcCore.Calculations.MonthlyDays.cs:32`) -> _None_
- `PreferencesManager.GetClimateZoneParams` (`reference/eecalc-decompiled/EECalcCore.Calculations.PreferencesManager.cs:21`) -> _None_
- `RoofTableCalc.CalculateArea` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:17`) -> `Calculator.SumFields`
- `RoofTableCalc.CalculateNonTranspU` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:22`) -> `Calculator.AcumulateWeight`
- `RoofTableCalc.SumL` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:28`) -> `Calculator.SumFields`
- `RoofTableCalc.SumX` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:45`) -> `Calculator.SumFields`
- `RoofTableCalc.CalculateEpsilon` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:62`) -> `Calculator.AcumulateWeight`
- `RoofTableCalc.CalculateAlfa` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:68`) -> `Calculator.AcumulateWeight`
- `RoofTableCalc.SumTrasparentArea` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:74`) -> `Calculator.SumFields`
- `RoofTableCalc.CalculateTrasparentU` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:79`) -> `Calculator.AcumulateWeight`
- `RoofTableCalc.CalculateTrasparentG` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:85`) -> `Calculator.AcumulateWeight`
- `RoofTableCalc.SumCeilingArea` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:91`) -> `Calculator.SumFields`
- `RoofTableCalc.CalculateCeilingU` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:96`) -> `Calculator.AcumulateWeight`
- `SunEnergyPreferencesManager.GetClimateZoneParams` (`reference/eecalc-decompiled/EECalcCore.Calculations.SunEnergyPreferencesManager.cs:21`) -> _None_
- `TempBridgeCalc.CalculateSums` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.TempBridgeCalc.cs:9`) -> _None_
- `WallsTableCalc.SumColumnOuterArea` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:17`) -> `Calculator.SumFields`
- `WallsTableCalc.AccumulateOuterU` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:22`) -> `Calculator.AcumulateWeight`
- `WallsTableCalc.SumColumnOuterL` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:28`) -> `Calculator.SumFields`
- `WallsTableCalc.SumColumnOuterX` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:42`) -> `Calculator.SumFields`
- `WallsTableCalc.AcumulateOuterEpsilon` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:56`) -> `Calculator.AcumulateWeight`
- `WallsTableCalc.AcumulateOuterAlfa` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:62`) -> `Calculator.AcumulateWeight`
- `WallsTableCalc.SumColumnInnerArea` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:68`) -> `Calculator.SumFields`
- `WallsTableCalc.CalculateInnerU` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:73`) -> `Calculator.AcumulateWeight`
- `WallsTableCalc.SumWindowArea` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:79`) -> `Calculator.SumFields`
- `WallsTableCalc.CalculateWindowU` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:84`) -> `Calculator.AcumulateWeight`
- `WallsTableCalc.CalculateWindowG` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:90`) -> `Calculator.AcumulateWeight`
- `WallsTableCalc.CalculateWindowE` (`reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:96`) -> `Calculator.AcumulateWeight`

## Internal Dependency Graph

### BuildingTypesManager.GetClimateZoneParams
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.BuildingTypesManager.cs:27`
- Internal calls: _None_
- External/API calls: `ScaleType.Single`

### Calculator.Calculate
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.Calculator.cs:12`
- Internal calls: _None_
- External/API calls: _None_

### Calculator.AcumulateWeight
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.Calculator.cs:16`
- Internal calls: _None_
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `valuesList.Count`

### Calculator.SumFields
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.Calculator.cs:43`
- Internal calls: _None_
- External/API calls: `Math.Abs`

### DataRow.OnPropertyChanged
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.DataRow.cs:57`
- Internal calls: _None_
- External/API calls: `PropertyChangedEventArgs`, `this.PropertyChanged`

### FloorTableCalc.CalculateFloorArea
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.FloorTableCalc.cs:15`
- Internal calls: `Calculator.SumFields`
- External/API calls: _None_

### FloorTableCalc.CalculateFloorU
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.FloorTableCalc.cs:20`
- Internal calls: `Calculator.AcumulateWeight`
- External/API calls: _None_

### FloorTableCalc.CalculateOtherFloorArea
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.FloorTableCalc.cs:26`
- Internal calls: `Calculator.SumFields`
- External/API calls: _None_

### FloorTableCalc.CalculateOtherFloorU
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.FloorTableCalc.cs:31`
- Internal calls: `Calculator.AcumulateWeight`
- External/API calls: _None_

### FloorTableCalc.SumX
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.FloorTableCalc.cs:37`
- Internal calls: `Calculator.SumFields`
- External/API calls: _None_

### FloorTableCalc.SumL
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.FloorTableCalc.cs:51`
- Internal calls: `Calculator.SumFields`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CoolingCalculations
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:123`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateCoolingEnergyActual`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef1`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef2`, `InputDataCalc.CalcPeriod`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:137`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:145`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:153`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultReferences
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:161`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:169`
- Internal calls: `HeatingAndCoolingResultCalc.ApplyValuesToTempSectionRef1`, `HeatingAndCoolingResultCalc.CalculateAcRef1`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrRef1`, `HeatingAndCoolingResultCalc.CalculateETA`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfRef1`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentRef1`, `HeatingAndCoolingResultCalc.CalculateQLatentOccupantsRef1`, `HeatingAndCoolingResultCalc.CalculateQgainRef1`, `HeatingAndCoolingResultCalc.CalculateQinfRef1`, `HeatingAndCoolingResultCalc.CalculateQveRef1`, `HeatingAndCoolingResultCalc.ClaculateQfreecoolingRef1`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: `Deserialize`, `MonthDataCooling`, `list.Add`, `list2.Add`, `list2.Aggregate`, `list3.Add`, `list3.Aggregate`, `list4.Add`, `section.Serialize`

### HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:210`
- Internal calls: `HeatingAndCoolingResultCalc.ApplyValuesToTempSectionRef2`, `HeatingAndCoolingResultCalc.CalculateAcRef2`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrRef2`, `HeatingAndCoolingResultCalc.CalculateETA`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfRef2`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentRef2`, `HeatingAndCoolingResultCalc.CalculateQLatentOccupantsRef2`, `HeatingAndCoolingResultCalc.CalculateQgainRef2`, `HeatingAndCoolingResultCalc.CalculateQinfRef2`, `HeatingAndCoolingResultCalc.CalculateQveRef2`, `HeatingAndCoolingResultCalc.ClaculateQfreecoolingRef2`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: `Deserialize`, `MonthDataCooling`, `list.Add`, `list2.Add`, `list2.Aggregate`, `list3.Add`, `list3.Aggregate`, `list4.Add`, `section.Serialize`

### HeatingAndCoolingResultCalc.CalculateCoolingEnergyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:251`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAc`, `HeatingAndCoolingResultCalc.CalculateCoolingQtr`, `HeatingAndCoolingResultCalc.CalculateETA`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInf`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVent`, `HeatingAndCoolingResultCalc.CalculateQLatentOccupants`, `HeatingAndCoolingResultCalc.CalculateQgain`, `HeatingAndCoolingResultCalc.CalculateQinf`, `HeatingAndCoolingResultCalc.CalculateQve`, `HeatingAndCoolingResultCalc.ClaculateQfreecooling`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: `MonthDataCooling`, `list.Add`, `list2.Add`, `list2.Aggregate`, `list3.Add`, `list4.Add`, `list4.Aggregate`

### HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:290`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAcBaseLine`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateETA`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfBaseLine`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentBaseLine`, `HeatingAndCoolingResultCalc.CalculateQLatentOccupantsBaseLine`, `HeatingAndCoolingResultCalc.CalculateQgainBaseLine`, `HeatingAndCoolingResultCalc.CalculateQinfBaseLine`, `HeatingAndCoolingResultCalc.CalculateQveBaseLine`, `HeatingAndCoolingResultCalc.ClaculateQfreecoolingBaseLine`
- External/API calls: `list.Add`, `list.Aggregate`, `list2.Add`, `list3.Add`, `list3.Aggregate`

### HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:315`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAcESM`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrESM`, `HeatingAndCoolingResultCalc.CalculateETA`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfESM`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentESM`, `HeatingAndCoolingResultCalc.CalculateQLatentOccupantsESM`, `HeatingAndCoolingResultCalc.CalculateQgainESM`, `HeatingAndCoolingResultCalc.CalculateQinfESM`, `HeatingAndCoolingResultCalc.CalculateQveESM`, `HeatingAndCoolingResultCalc.ClaculateQfreecoolingEsm`
- External/API calls: `list.Add`, `list.Aggregate`, `list2.Add`, `list3.Add`, `list3.Aggregate`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsInfRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:340`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list.Clear`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsInfRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:404`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list.Clear`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsInf
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:468`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list.Clear`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsInfBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:532`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list.Clear`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsInfESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:596`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list.Clear`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsVentRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:660`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsVentRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:721`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsVent
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:782`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsVentBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:843`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsVentESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:904`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalcAirX
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:965`
- Internal calls: _None_
- External/API calls: `Math.Pow`

### HeatingAndCoolingResultCalc.CalcRoW
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:973`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcRo
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:979`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRoW`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateETA
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:985`
- Internal calls: _None_
- External/API calls: `Math.Abs`, `Math.Pow`

### HeatingAndCoolingResultCalc.CalculateAcRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1004`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempRef1`, `HeatingAndCoolingResultCalc.CalculateCoolingHtr`, `HeatingAndCoolingResultCalc.CalculateHinfRef1`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateAcRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1014`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempRef2`, `HeatingAndCoolingResultCalc.CalculateCoolingHtr`, `HeatingAndCoolingResultCalc.CalculateHinfRef2`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateAc
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1024`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempCurrent`, `HeatingAndCoolingResultCalc.CalculateCoolingHtr`, `HeatingAndCoolingResultCalc.CalculateHinf`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateAcBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1034`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempBaseLine`, `HeatingAndCoolingResultCalc.CalculateCoolingHtr`, `HeatingAndCoolingResultCalc.CalculateHinfBaseLine`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateAcESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1044`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempESM`, `HeatingAndCoolingResultCalc.CalculateCoolingHtrESM`, `HeatingAndCoolingResultCalc.CalculateHinfESM`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetNightWorkingHours
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1054`
- Internal calls: _None_
- External/API calls: `list.Add`

### HeatingAndCoolingResultCalc.ClaculateQfreecoolingRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1082`
- Internal calls: `HeatingAndCoolingResultCalc.GetNightWorkingHours`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: `list.Add`, `list.Aggregate`, `list2.Add`, `list2.Aggregate`, `list3.Add`, `list3.Aggregate`, `list4.Add`, `list4.Aggregate`

### HeatingAndCoolingResultCalc.ClaculateQfreecoolingRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1121`
- Internal calls: `HeatingAndCoolingResultCalc.GetNightWorkingHours`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: `list.Add`, `list.Aggregate`, `list2.Add`, `list2.Aggregate`, `list3.Add`, `list3.Aggregate`, `list4.Add`, `list4.Aggregate`

### HeatingAndCoolingResultCalc.ClaculateQfreecooling
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1160`
- Internal calls: `HeatingAndCoolingResultCalc.GetNightWorkingHours`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: `list.Add`, `list.Aggregate`, `list2.Add`, `list2.Aggregate`, `list3.Add`, `list3.Aggregate`, `list4.Add`, `list4.Aggregate`

### HeatingAndCoolingResultCalc.ClaculateQfreecoolingBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1199`
- Internal calls: `HeatingAndCoolingResultCalc.GetNightWorkingHours`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: `list.Add`, `list.Aggregate`, `list2.Add`, `list2.Aggregate`, `list3.Add`, `list3.Aggregate`, `list4.Add`, `list4.Aggregate`

### HeatingAndCoolingResultCalc.ClaculateQfreecoolingEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1238`
- Internal calls: `HeatingAndCoolingResultCalc.GetNightWorkingHours`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: `list.Add`, `list.Aggregate`, `list2.Add`, `list2.Aggregate`, `list3.Add`, `list3.Aggregate`, `list4.Add`, `list4.Aggregate`

### HeatingAndCoolingResultCalc.CalculateQgainRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1277`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateQintRef1`, `HeatingAndCoolingResultCalc.CalculateQoccupantsBaseLine`, `HeatingAndCoolingResultCalc.CalculateQsolRef1`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQgainRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1285`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateQintRef2`, `HeatingAndCoolingResultCalc.CalculateQoccupantsBaseLine`, `HeatingAndCoolingResultCalc.CalculateQsolRef2`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQgain
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1293`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateQint`, `HeatingAndCoolingResultCalc.CalculateQoccupants`, `HeatingAndCoolingResultCalc.CalculateQsol`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQgainBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1301`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateQintBaseLine`, `HeatingAndCoolingResultCalc.CalculateQoccupantsBaseLine`, `HeatingAndCoolingResultCalc.CalculateQsolBaseLine`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQgainESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1309`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateQintESM`, `HeatingAndCoolingResultCalc.CalculateQoccupantsESM`, `HeatingAndCoolingResultCalc.CalculateQsolESM`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQintRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1317`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQintRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1324`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQint
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1331`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQintBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1338`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQintESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1345`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQoccupants
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1352`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateOccupantshours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQoccupantsBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1359`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateOccupantshoursBaseLine`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQoccupantsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1366`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateOccupantshoursESM`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQLatentOccupantsRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1373`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateOccupantshours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQLatentOccupantsRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1380`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateOccupantshours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQLatentOccupants
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1387`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateOccupantshours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQLatentOccupantsBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1394`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateOccupantshoursBaseLine`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQLatentOccupantsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1401`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateOccupantshoursESM`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateOccupantshours
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1408`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateOccupantshoursBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1416`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateOccupantshoursESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1424`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQveRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1432`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateHveRef1`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQveRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1491`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateHveRef2`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQve
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1556`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateHve`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQveBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1621`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateHveBaseLine`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQveESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1686`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateHveESM`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateHveRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1751`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateHveRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1756`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateHve
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1761`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateHveBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1766`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateHveESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1771`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQsolRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1776`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsol`, `HeatingAndCoolingResultCalc.CalculateTrasparentFsol`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQsolRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1790`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsol`, `HeatingAndCoolingResultCalc.CalculateTrasparentFsol`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQsol
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1804`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsol`, `HeatingAndCoolingResultCalc.CalculateTrasparentFsol`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQsolBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1818`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsol`, `HeatingAndCoolingResultCalc.CalculateTrasparentFsol`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQsolESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1832`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsolEsm`, `HeatingAndCoolingResultCalc.CalculateTrasparentFsolEsm`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQinfRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1846`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingRef1`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingRef1`, `HeatingAndCoolingResultCalc.CalculateHinfRef1`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQinfRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1852`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingRef2`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingRef2`, `HeatingAndCoolingResultCalc.CalculateHinfRef2`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQinf
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1858`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCooling`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempCooling`, `HeatingAndCoolingResultCalc.CalculateHinf`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQinfBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1864`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingBaseLine`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingBaseLine`, `HeatingAndCoolingResultCalc.CalculateHinfBaseLine`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateQinfESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1870`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingESM`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingESM`, `HeatingAndCoolingResultCalc.CalculateHinfESM`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateHinfRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1876`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateHinfRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1881`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateHinf
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1886`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateHinfBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1891`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateHinfESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1896`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateCoolingQtrRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1901`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingRef1`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingRef1`, `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempRef1`, `HeatingAndCoolingResultCalc.CalculateCoolingHtr`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateCoolingQtrRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1910`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingRef2`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingRef2`, `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempRef2`, `HeatingAndCoolingResultCalc.CalculateCoolingHtr`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateCoolingQtr
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1919`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCooling`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempCooling`, `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempCurrent`, `HeatingAndCoolingResultCalc.CalculateCoolingHtr`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateCoolingQtrBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1928`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingBaseLine`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingBaseLine`, `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempBaseLine`, `HeatingAndCoolingResultCalc.CalculateCoolingHtr`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateCoolingQtrESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1936`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingESM`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingESM`, `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempESM`, `HeatingAndCoolingResultCalc.CalculateCoolingHtrESM`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateCoolingHtr
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1944`
- Internal calls: `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2Cooling`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3Cooling`, `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHgCurrent`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1Cooling`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateCoolingHtrESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1958`
- Internal calls: `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2Cooling`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3Cooling`, `HeatingAndCoolingResultCalc.CalculateParameterHdEsm`, `HeatingAndCoolingResultCalc.CalculateParameterHgEsm`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1CoolingESM`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateAverageCoolingTempRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1972`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateAverageCoolingTempRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1986`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateAverageCoolingTempCurrent
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2000`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateAverageCoolingTempBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2014`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateAverageCoolingTempESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2028`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2042`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2050`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgProjectTempCooling
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2058`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2066`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2074`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2082`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2091`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCooling
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2100`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2109`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2118`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.SumWallDirecrionsHu1Cooling
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2127`
- Internal calls: `HeatingAndCoolingResultCalc.CalcWallDirectionParameterHu1Cooling`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.SumWallDirecrionsHu1CoolingESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2140`
- Internal calls: `HeatingAndCoolingResultCalc.CalcWallDirectionParameterHu1Cooling`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcWallDirectionParameterHu1Cooling
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2153`
- Internal calls: _None_
- External/API calls: `object.Equals`

### HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2Cooling
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2187`
- Internal calls: _None_
- External/API calls: `object.Equals`

### HeatingAndCoolingResultCalc.CalcFloorsParameterHu3Cooling
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2221`
- Internal calls: _None_
- External/API calls: `object.Equals`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2255`
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursBaseLine`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `source.Sum`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2271`
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursBaseLine`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `source.Sum`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2287`
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursActual`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `source.Sum`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2303`
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursBaseLine`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `source.Sum`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2320`
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursEsm`, `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursEsm`, `InputDataCalc.CalcPeriod`
- External/API calls: `ToString`, `double.IsInfinity`, `double.IsNaN`, `source.Sum`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2338`
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursBaseLine`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `source.Sum`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2365`
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursBaseLine`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `source.Sum`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2391`
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursActual`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursActual`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `source.Sum`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2417`
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursBaseLine`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `source.Sum`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2443`
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursEsm`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursEsm`, `InputDataCalc.CalcPeriod`
- External/API calls: `ToString`, `double.IsInfinity`, `double.IsNaN`, `source.Sum`

### HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2470`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2477`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2484`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2491`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2498`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2505`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2512`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2519`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2526`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2533`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2540`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2547`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateUouterWallsCurrent
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2554`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateUouterWallsEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2583`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateUinnerWallsCurrent
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2611`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateUinnerWallsEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2640`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateUwindowsCurrent
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2668`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateUwindowsEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2700`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGcurrent
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2731`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGesm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2763`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.GetUnonTrasparentRoof
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2794`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetUceiling
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2801`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetUfloor
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2808`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetUotherFloor
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2815`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateNetEnergy
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2822`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateNeededEnergyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2831`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateNeededEnergyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2848`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateNeededEnergyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2865`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateNeededEnergyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2882`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateNeededEnergyEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2899`
- Internal calls: _None_
- External/API calls: `ToString`, `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2917`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2933`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2949`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2965`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2981`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2997`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3013`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3029`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3045`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3061`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3077`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3093`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3109`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3125`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3141`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3157`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3174`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3191`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3208`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3225`
- Internal calls: _None_
- External/API calls: `ToString`, `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.Calculations
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3243`
- Internal calls: `HeatingAndCoolingResultCalc.ApplyValuesToTempSectionRef1`, `HeatingAndCoolingResultCalc.ApplyValuesToTempSectionRef2`, `HeatingAndCoolingResultCalc.CalculateActual`, `HeatingAndCoolingResultCalc.CalculateBaseLine`, `HeatingAndCoolingResultCalc.CalculateEsm`, `HeatingAndCoolingResultCalc.CalculateLightsAndDevicesInputs`, `HeatingAndCoolingResultCalc.CalculateRef1`, `HeatingAndCoolingResultCalc.CalculateRef2`, `HeatingAndCoolingResultCalc.CheckForNaN`, `HeatingAndCoolingResultCalc.GetLightsAndDevicesInputs`, `HeatingAndCoolingResultCalc.OccupantHours`, `HeatingAndCoolingResultCalc.OccupantsHoursBaseLine`, `HeatingAndCoolingResultCalc.OccupantsHoursEsm`, `InputDataCalc.CalcPeriod`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: `Deserialize`, `MonthData`, `Section`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list10.Add`, `list10.Aggregate`, `list11.Add`, `list11.Aggregate`, `list2.Aggregate`, `list3.Aggregate`, `list4.Add`, `list4.Aggregate`, `list5.Add`, `list5.Aggregate`, `list6.Add`, `list6.Aggregate`, `list7.Aggregate`, `list8.Aggregate`, `list9.Add`, `list9.Aggregate`, `section.Serialize`

### HeatingAndCoolingResultCalc.GetWeekHoursResultReferences
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3402`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekHoursResultActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3419`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekHoursResultBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3435`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekHoursResultEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3451`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.OccupantHours
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3467`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetTestValue
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3472`
- Internal calls: `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3483`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateParameterNign`, `HeatingAndCoolingResultCalc.CalculateParameterQgn`, `HeatingAndCoolingResultCalc.CalculateParameterQtr`, `HeatingAndCoolingResultCalc.CalculateParameterQve`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateLightsAndDevicesInputs
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3494`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`
- External/API calls: `DevicesList.Add`, `DevicesListBaseLine.Add`, `DevicesListESM.Add`, `DevicesRef1.Add`, `DevicesRef2.Add`, `LigthsList.Add`, `LigthsListBaseLine.Add`, `LigthsListESM.Add`, `LigthsListRef1.Add`, `LigthsListRef2.Add`

### HeatingAndCoolingResultCalc.GetLightsAndDevicesInputs
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3562`
- Internal calls: `HeatingAndCoolingResultCalc.SumItemsList`
- External/API calls: `DevicesList.Aggregate`, `DevicesList.Clear`, `DevicesListBaseLine.Aggregate`, `DevicesListBaseLine.Clear`, `DevicesListESM.Aggregate`, `DevicesListESM.Clear`, `DevicesRef1.Aggregate`, `DevicesRef1.Clear`, `DevicesRef2.Aggregate`, `DevicesRef2.Clear`, `LigthsList.Clear`, `LigthsListBaseLine.Clear`, `LigthsListESM.Clear`, `LigthsListRef1.Clear`, `LigthsListRef2.Clear`, `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.SumItemsList
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3622`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `itemsList.Aggregate`

### HeatingAndCoolingResultCalc.CalculateParameterNign
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3632`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateaH`
- External/API calls: `Math.Abs`, `Math.Pow`

### HeatingAndCoolingResultCalc.CalculateaH
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3650`
- Internal calls: `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3`, `HeatingAndCoolingResultCalc.CalcParameterHve`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHgCurrent`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterQve
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3663`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTemp`, `HeatingAndCoolingResultCalc.CalcAvgProjectTemp`, `HeatingAndCoolingResultCalc.CalcParameterHve`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgProjectTemp
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3668`
- Internal calls: `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTemp
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3677`
- Internal calls: `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcParameterHve
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3687`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterQtr
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3693`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTemp`, `HeatingAndCoolingResultCalc.CalcAvgProjectTemp`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHtr`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterHtr
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3702`
- Internal calls: `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3`, `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHgCurrent`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterHdCurrent
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3713`
- Internal calls: `HeatingAndCoolingResultCalc.SumAllDirectionWindowsCurrent`, `HeatingAndCoolingResultCalc.SumAllDirectionsWallsCurrent`, `HeatingAndCoolingResultCalc.SumNonTrasparentRoof`, `HeatingAndCoolingResultCalc.SumTrasparentRoof`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterHgCurrent
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3718`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.SumAllDirectionsWallsCurrent
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3723`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateItemsWalls`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateItemsWalls
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3736`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.SumAllDirectionWindowsCurrent
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3762`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.SumNonTrasparentRoof
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3775`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.SumTrasparentRoof
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3810`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateAverageHeatTempCurrent
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3824`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.SumWallDirecrionsHu1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3838`
- Internal calls: `HeatingAndCoolingResultCalc.CalcWallDirectionParameterHu1`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcWallDirectionParameterHu1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3851`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3881`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcFloorsParameterHu3
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3911`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterQgn
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3941`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsol`, `HeatingAndCoolingResultCalc.CalculateTrasparentFsol`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTransparentFsol
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3953`
- Internal calls: _None_
- External/API calls: `Math.Pow`

### HeatingAndCoolingResultCalc.CalculateTrasparentFsol
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3965`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateTransparentFsol`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateNonTransparentFsol
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3997`
- Internal calls: _None_
- External/API calls: `Math.Pow`

### HeatingAndCoolingResultCalc.CalculateNonTrasparentFsol
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4010`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateNonTransparentFsol`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.OccupantsHoursEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4034`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4039`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateParameterNiEsm`, `HeatingAndCoolingResultCalc.CalculateParameterQgnEsm`, `HeatingAndCoolingResultCalc.CalculateParameterQtrEsm`, `HeatingAndCoolingResultCalc.CalculateParameterQveEsm`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterNiEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4050`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateaHesm`
- External/API calls: `Math.Abs`, `Math.Pow`

### HeatingAndCoolingResultCalc.CalculateaHesm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4068`
- Internal calls: `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3`, `HeatingAndCoolingResultCalc.CalcParameterHveEsm`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempEsm`, `HeatingAndCoolingResultCalc.CalculateParameterHdEsm`, `HeatingAndCoolingResultCalc.CalculateParameterHgEsm`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1Esm`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterQveEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4081`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempEsm`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempEsm`, `HeatingAndCoolingResultCalc.CalcParameterHveEsm`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterHtrEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4087`
- Internal calls: `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3`, `HeatingAndCoolingResultCalc.CalculateParameterHdEsm`, `HeatingAndCoolingResultCalc.CalculateParameterHgEsm`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1Esm`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcParameterHveEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4098`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterQtrEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4103`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempEsm`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempEsm`, `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempEsm`, `HeatingAndCoolingResultCalc.CalculateParameterHdEsm`, `HeatingAndCoolingResultCalc.CalculateParameterHgEsm`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1Esm`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateAverageHeatTempEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4114`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTempEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4128`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgProjectTempEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4137`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterHdEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4145`
- Internal calls: `HeatingAndCoolingResultCalc.SumAllDirectionWindowsEsm`, `HeatingAndCoolingResultCalc.SumAllDirectionsWallsEsm`, `HeatingAndCoolingResultCalc.SumNonTrasparentRoof`, `HeatingAndCoolingResultCalc.SumTrasparentRoof`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.SumAllDirectionsWallsEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4150`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateItemsWalls`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.SumAllDirectionWindowsEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4163`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterHgEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4176`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.SumWallDirecrionsHu1Esm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4181`
- Internal calls: `HeatingAndCoolingResultCalc.CalcWallDirectionParameterHu1`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterQgnEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4194`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsolEsm`, `HeatingAndCoolingResultCalc.CalculateTrasparentFsolEsm`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTrasparentFsolEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4206`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateTransparentFsol`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateNonTrasparentFsolEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4246`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateNonTransparentFsol`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.OccupantsHoursBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4270`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4275`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateParameterNignBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterQgnBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterQtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterQveBaseLIne`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateaHbaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4286`
- Internal calls: `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3`, `HeatingAndCoolingResultCalc.CalcParameterHveBaseLine`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHgCurrent`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterNignBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4299`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateaHbaseLine`
- External/API calls: `Math.Abs`, `Math.Pow`

### HeatingAndCoolingResultCalc.CalculateParameterQtrBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4317`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempBaseLine`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempBaseLine`, `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHgCurrent`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTempBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4328`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgProjectTempBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4337`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateAverageHeatTempBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4345`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterQveBaseLIne
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4359`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempBaseLine`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempBaseLine`, `HeatingAndCoolingResultCalc.CalcParameterHveBaseLine`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterHtrBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4365`
- Internal calls: `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3`, `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHgCurrent`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcParameterHveBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4376`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterQgnBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4382`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsol`, `HeatingAndCoolingResultCalc.CalculateTrasparentFsol`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.OccupantsHoursRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4394`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4399`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateParameterNignRef1`, `HeatingAndCoolingResultCalc.CalculateParameterQgnBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterQtrRef1`, `HeatingAndCoolingResultCalc.CalculateParameterQveRef1`, `HeatingAndCoolingResultCalc.OccupantHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4413`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateRef1`, `HeatingAndCoolingResultCalc.OccupantsHoursRef1`
- External/API calls: `energyByMonthsListRef1.Add`, `latentHeatListRef1.Add`

### HeatingAndCoolingResultCalc.CalculateParameterQveRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4421`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempRef1`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempRef1`, `HeatingAndCoolingResultCalc.CalcParameterHveRef1`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcParameterHveRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4426`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterQtrRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4431`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempRef1`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempRef1`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempRef1`, `HeatingAndCoolingResultCalc.CalculateParameterHtr`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterHtrRef
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4440`
- Internal calls: `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3`, `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHgCurrent`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateAverageHeatTempRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4451`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgProjectTempRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4465`
- Internal calls: `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTempRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4474`
- Internal calls: `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateaHref1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4484`
- Internal calls: `HeatingAndCoolingResultCalc.CalcParameterHveRef1`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempRef1`, `HeatingAndCoolingResultCalc.CalculateParameterHtr`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterNignRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4494`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateaHref1`
- External/API calls: `Math.Abs`, `Math.Pow`

### HeatingAndCoolingResultCalc.ApplyValuesToTempSectionRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4512`
- Internal calls: `HeatingAndCoolingResultCalc.ApplyCoefficientG`, `HeatingAndCoolingResultCalc.ApplyUdirectionWalls`, `HeatingAndCoolingResultCalc.ApplyUroofsAndCeilings`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ApplyCoefficientG
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4519`
- Internal calls: `HeatingAndCoolingResultCalc.CopyGbyOrientation`, `RoofTableCalc.CalculateTrasparentG`, `RoofTableCalc.SumTrasparentArea`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ApplyUroofsAndCeilings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4542`
- Internal calls: `FloorTableCalc.CalculateFloorArea`, `FloorTableCalc.CalculateFloorU`, `FloorTableCalc.CalculateOtherFloorArea`, `FloorTableCalc.CalculateOtherFloorU`, `RoofTableCalc.CalculateArea`, `RoofTableCalc.CalculateCeilingU`, `RoofTableCalc.CalculateNonTranspU`, `RoofTableCalc.SumCeilingArea`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ApplyUdirectionWalls
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4584`
- Internal calls: `HeatingAndCoolingResultCalc.ApplyToTrasparentRoofs`, `HeatingAndCoolingResultCalc.CopyByOrientation`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ApplyToTrasparentRoofs
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4597`
- Internal calls: `RoofTableCalc.CalculateTrasparentU`, `RoofTableCalc.SumTrasparentArea`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyByOrientation
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4612`
- Internal calls: `WallsTableCalc.AccumulateOuterU`, `WallsTableCalc.CalculateInnerU`, `WallsTableCalc.CalculateWindowU`, `WallsTableCalc.SumColumnInnerArea`, `WallsTableCalc.SumColumnOuterArea`, `WallsTableCalc.SumWindowArea`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyGbyOrientation
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4640`
- Internal calls: `WallsTableCalc.CalculateWindowG`, `WallsTableCalc.SumWindowArea`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.OccupantsHoursRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4652`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4657`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateParameterNignRef2`, `HeatingAndCoolingResultCalc.CalculateParameterQgnBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterQtrRef2`, `HeatingAndCoolingResultCalc.CalculateParameterQveRef2`, `HeatingAndCoolingResultCalc.OccupantHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4671`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateRef2`, `HeatingAndCoolingResultCalc.OccupantsHoursRef2`
- External/API calls: `energyByMonthsListRef2.Add`, `latentHeatListRef2.Add`

### HeatingAndCoolingResultCalc.CalculateParameterQveRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4679`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempRef2`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempRef2`, `HeatingAndCoolingResultCalc.CalcParameterHveRef2`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcParameterHveRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4685`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterQtrRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4690`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempRef2`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempRef2`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempRef2`, `HeatingAndCoolingResultCalc.CalculateParameterHtr`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateAverageHeatTempRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4699`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgProjectTempRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4713`
- Internal calls: `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTempRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4722`
- Internal calls: `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateaHref2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4732`
- Internal calls: `HeatingAndCoolingResultCalc.CalcParameterHveRef2`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempRef2`, `HeatingAndCoolingResultCalc.CalculateParameterHtrRef`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterNignRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4742`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateaHref2`
- External/API calls: `Math.Abs`, `Math.Pow`

### HeatingAndCoolingResultCalc.ApplyValuesToTempSectionRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4760`
- Internal calls: `HeatingAndCoolingResultCalc.ApplyCoefficientG`, `HeatingAndCoolingResultCalc.ApplyUdirectionWalls`, `HeatingAndCoolingResultCalc.ApplyUroofsAndCeilings`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.HotWaterCalculationReferences
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4767`
- Internal calls: _None_
- External/API calls: `BuildingZones.Sum`, `Math.Max`

### HeatingAndCoolingResultCalc.HotWaterCalculationActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4778`
- Internal calls: _None_
- External/API calls: `BuildingZones.Sum`, `Math.Max`

### HeatingAndCoolingResultCalc.HotWaterCalculationBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4786`
- Internal calls: _None_
- External/API calls: `BuildingZones.Sum`, `Math.Max`

### HeatingAndCoolingResultCalc.HotWaterCalculationESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4794`
- Internal calls: _None_
- External/API calls: `BuildingZones.Sum`, `Math.Max`

### HeatingAndCoolingResultCalc.CalculateGeneratorHotWaterEfficiencyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4802`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGeneratorHotWaterEfficiencyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4818`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGeneratorHotWaterEfficiencyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4834`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGeneratorHotWaterEfficiencyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4850`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateGeneratorHotWaterEfficiencyEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4866`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateHotWaterNeededEnergyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4882`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateHotWaterNeededEnergyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4899`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateHotWaterNeededEnergyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4916`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateHotWaterNeededEnergyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4933`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateHotWaterNeededEnergyEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4950`
- Internal calls: _None_
- External/API calls: `ToString`, `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculatePeriodsReference
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4968`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculatePeriodsActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4978`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActual`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActual`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActual`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculatePeriodsBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4985`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLine`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLine`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLine`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculatePeriodsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4992`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESM`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESM`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESM`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculatePeriodsReferenceBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4999`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1Balanced`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2Balanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1Balanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2Balanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1Balanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2Balanced`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculatePeriodsActualBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5009`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActualBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualBalanced`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculatePeriodsBaseLineBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5016`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLineBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineBalanced`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculatePeriodsESMBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5023`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESMBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMBalanced`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculatePeriodsReferenceNonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5030`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1NonBalanced`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2NonBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1NonBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2NonBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1NonBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2NonBalanced`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculatePeriodsActualNonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5040`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActualNonBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualNonBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualNonBalanced`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculatePeriodsBaseLineNonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5047`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLineNonBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineNonBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineNonBalanced`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculatePeriodsESMNonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5054`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESMNonBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMNonBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMNonBalanced`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculatePeriodsReferenceHotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5061`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1HotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2HotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1HotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2HotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1HotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2HotWaterPumps`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculatePeriodsActualHotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5071`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActualHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualHotWaterPumps`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculatePeriodsBaseLineHotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5078`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLineHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineHotWaterPumps`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculatePeriodsESMHotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5085`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESMHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMHotWaterPumps`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5092`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5099`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5106`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5113`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5120`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5127`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5134`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5178`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5222`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5266`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5310`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5354`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5398`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5442`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5486`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1Balanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5530`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1Balanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5537`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1Balanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5544`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2Balanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5551`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2Balanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5558`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2Balanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5565`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5572`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5616`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodActualBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5660`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5704`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5748`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLineBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5792`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5836`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5880`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodESMBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5924`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1NonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5968`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2NonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5975`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1NonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5982`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2NonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5989`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1NonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5996`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2NonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6003`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualNonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6010`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualNonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6046`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodActualNonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6082`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineNonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6118`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineNonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6162`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLineNonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6206`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMNonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6250`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMNonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6294`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodESMNonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6338`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1HotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6382`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1HotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6389`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1HotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6396`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2HotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6403`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2HotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6410`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2HotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6417`
- Internal calls: `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualHotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6424`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualHotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6460`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodActualHotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6496`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineHotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6532`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineHotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6576`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLineHotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6620`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMHotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6664`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `ToString`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMHotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6709`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `ToString`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalculateAnnualPeriodESMHotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6754`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- External/API calls: `Math.Abs`, `ToString`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list2.Add`, `list2.Aggregate`

### HeatingAndCoolingResultCalc.CalcAvgMonthPower
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6799`
- Internal calls: `HeatingAndCoolingResultCalc.CalcWeekPower`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcWeekPower
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6819`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateZonePowerEnergy
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6836`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1Area`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1BaseLine`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1BaseLineArea`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1Esm`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1EsmArea`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2Area`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2BaseLine`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2BaseLineArea`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2Esm`, `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2EsmArea`, `HeatingAndCoolingResultCalc.CalculateHeatingPower`, `HeatingAndCoolingResultCalc.ClearFuelCellsPowerTable`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateBuildingPowerEnergy
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6854`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateBuildingHeatingPower`, `HeatingAndCoolingResultCalc.CalculateBuildingSourcePower`, `HeatingAndCoolingResultCalc.CalculateZonePowerEnergy`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateBuildingHeatingPower
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6864`
- Internal calls: _None_
- External/API calls: `BuildingZones.Any`, `BuildingZones.Sum`

### HeatingAndCoolingResultCalc.CalculateBuildingSourcePower
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6878`
- Internal calls: _None_
- External/API calls: `BuildingZones.Sum`

### HeatingAndCoolingResultCalc.CalculateHeatingPower
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6915`
- Internal calls: `HeatingAndCoolingResultCalc.CalcParameterHve`, `HeatingAndCoolingResultCalc.CalcParameterHveBaseLine`, `HeatingAndCoolingResultCalc.CalcParameterHveEsm`, `HeatingAndCoolingResultCalc.CalculateParameterHtr`, `HeatingAndCoolingResultCalc.CalculateParameterHtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterHtrEsm`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6941`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuelValue`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1Area
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6981`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuelAreaValue`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7021`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuel2Value`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2Area
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7061`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuel2AreaValue`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1BaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7101`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuelValueBaseLine`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1BaseLineArea
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7141`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuelAreaValueBaseLine`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2BaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7181`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuel2ValueBaseLine`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2BaseLineArea
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7221`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuel2AreaValueBaseLine`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1Esm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7261`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuelValueEsm`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1EsmArea
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7301`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuelAreaValueEsm`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2Esm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7341`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuel2ValueEsm`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2EsmArea
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7381`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuel2AreaValueEsm`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ClearFuelCellsPowerTable
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7421`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ClearFuelCellsPowerTableBuilding
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7491`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateFuelValue
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7561`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateFuelValueBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7567`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateFuelValueEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7573`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateFuel2Value
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7579`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateFuel2ValueBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7585`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateFuel2ValueEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7591`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateFuelAreaValue
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7597`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuelValue`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateFuelAreaValueBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7602`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuelValueBaseLine`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateFuelAreaValueEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7607`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuelValueEsm`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateFuel2AreaValue
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7612`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuel2Value`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateFuel2AreaValueBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7617`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuel2ValueBaseLine`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateFuel2AreaValueEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7622`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuel2ValueEsm`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ClearPrimaryEnergy
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7627`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculatePrimaryEnergyByTechnologies
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7686`
- Internal calls: `HeatingAndCoolingResultCalc.GetPrimaryEnergyCoeficient`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculatePrimaryEnergyPerArea
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:7916`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculatePrimaryFuelTypeAndValuesPerArea
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8014`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculatePrimaryTotalEnergy
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8084`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateTotalPrimaryActual`, `HeatingAndCoolingResultCalc.CalculateTotalPrimaryBaseLine`, `HeatingAndCoolingResultCalc.CalculateTotalPrimaryEsm`, `HeatingAndCoolingResultCalc.CalculateTotalPrimaryRef1`, `HeatingAndCoolingResultCalc.CalculateTotalPrimaryRef2`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalPrimaryRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8093`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalPrimaryRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8098`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalPrimaryEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8103`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalPrimaryBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8108`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalPrimaryActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8113`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateFuelSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8118`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetPrimaryFuelTypeAndValues
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8133`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuelSavings`, `HeatingAndCoolingResultCalc.GetPrimaryFuelType`, `HeatingAndCoolingResultCalc.GetPrimaryFuelTypeBaseLine`, `HeatingAndCoolingResultCalc.GetPrimaryFuelTypeEsm`, `HeatingAndCoolingResultCalc.GetPrimaryFuelTypeRef1`, `HeatingAndCoolingResultCalc.GetPrimaryFuelTypeRef2`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetPrimaryFuelTypeRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8245`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.GetPrimaryFuelTypeRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8289`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetPrimaryFuelType
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8329`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.GetPrimaryFuelTypeBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8373`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.GetPrimaryFuelTypeEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8417`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculatePrimaryEnergyFuelTotal
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8461`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateTotalPrimaryFuelActual`, `HeatingAndCoolingResultCalc.CalculateTotalPrimaryFuelBaseLine`, `HeatingAndCoolingResultCalc.CalculateTotalPrimaryFuelESM`, `HeatingAndCoolingResultCalc.CalculateTotalPrimaryFuelRef1`, `HeatingAndCoolingResultCalc.CalculateTotalPrimaryFuelRef2`, `HeatingAndCoolingResultCalc.CalculateTotalPrimaryFuelSavings`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalPrimaryFuelRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8471`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalPrimaryFuelRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8476`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalPrimaryFuelActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8481`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalPrimaryFuelBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8486`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalPrimaryFuelESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8491`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalPrimaryFuelSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8496`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetPrimaryEnergyCoeficient
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8501`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.BuildingCalculations
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8524`
- Internal calls: `HeatingAndCoolingResultCalc.BuildingCO2Calculations`, `HeatingAndCoolingResultCalc.CalculateBuildingPowerEnergy`, `HeatingAndCoolingResultCalc.CalculateNetEnergyByTechnologiesBuilding`, `HeatingAndCoolingResultCalc.CalculateNetEnergyPerArea`, `HeatingAndCoolingResultCalc.CalculateNetWithoutInputsEnergyByTechnologies`, `HeatingAndCoolingResultCalc.CalculateNetWithoutInputsEnergyByTechnologiesPerArea`, `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyByTechnologies`, `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyFuelTotal`, `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyPerArea`, `HeatingAndCoolingResultCalc.CalculatePrimaryFuelTypeAndValuesPerArea`, `HeatingAndCoolingResultCalc.CalculatePrimaryTotalEnergy`, `HeatingAndCoolingResultCalc.CalculateTotalFuelEnergy`, `HeatingAndCoolingResultCalc.CalculateTotalVei`, `HeatingAndCoolingResultCalc.CalculateTotalsNeededEnergyTable`, `HeatingAndCoolingResultCalc.ClearFuelCells`, `HeatingAndCoolingResultCalc.ClearNeededVEIenergy`, `HeatingAndCoolingResultCalc.ClearNetEnergy`, `HeatingAndCoolingResultCalc.ClearNetEnergyWithoutInputs`, `HeatingAndCoolingResultCalc.ClearPrimaryEnergy`, `HeatingAndCoolingResultCalc.ClearPrimaryEnergyFuelTableValues`, `HeatingAndCoolingResultCalc.GetBuildingData`, `HeatingAndCoolingResultCalc.GetConditionedArea`, `HeatingAndCoolingResultCalc.GetFuelTypeAndValues`, `HeatingAndCoolingResultCalc.GetPrimaryFuelTypeAndValues`, `HeatingAndCoolingResultCalc.SetFuelValue`, `HeatingAndCoolingResultCalc.SetScaleValues`, `HeatingAndCoolingResultCalc.UpdateActualState`, `HeatingAndCoolingResultCalc.UpdateBaseLineState`, `HeatingAndCoolingResultCalc.UpdateEsmState`, `HeatingAndCoolingResultCalc.UpdateRefsState`
- External/API calls: `BuildingZones.First`

### HeatingAndCoolingResultCalc.ZoneCalculations
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8567`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateNetEnergyByTechnologies`, `HeatingAndCoolingResultCalc.CalculateNetWithoutInputsEnergyByTechnologies`, `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyByTechnologies`, `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyFuelTotal`, `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyPerArea`, `HeatingAndCoolingResultCalc.CalculatePrimaryFuelTypeAndValuesPerArea`, `HeatingAndCoolingResultCalc.CalculatePrimaryTotalEnergy`, `HeatingAndCoolingResultCalc.CalculateTotalFuelEnergy`, `HeatingAndCoolingResultCalc.CalculateTotalVei`, `HeatingAndCoolingResultCalc.CalculateTotalsNeededEnergyTable`, `HeatingAndCoolingResultCalc.CalculateZonePowerEnergy`, `HeatingAndCoolingResultCalc.ClearFuelCells`, `HeatingAndCoolingResultCalc.ClearNeededVEIenergy`, `HeatingAndCoolingResultCalc.ClearNetEnergy`, `HeatingAndCoolingResultCalc.ClearNetEnergyWithoutInputs`, `HeatingAndCoolingResultCalc.ClearPrimaryEnergy`, `HeatingAndCoolingResultCalc.ClearPrimaryEnergyFuelTableValues`, `HeatingAndCoolingResultCalc.GetFuelTypeAndValues`, `HeatingAndCoolingResultCalc.GetPrimaryFuelTypeAndValues`, `HeatingAndCoolingResultCalc.ZoneCO2Calculations`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.SetScaleValues
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8594`
- Internal calls: `BuildingTypesManager.GetClimateZoneParams`, `HeatingAndCoolingResultCalc.SetScaleType`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.BuildingCO2Calculations
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8600`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateCO2Emissions`, `HeatingAndCoolingResultCalc.CalculateFuelSavings`, `HeatingAndCoolingResultCalc.CalculateSavings`, `HeatingAndCoolingResultCalc.ClearFuelCellsCO2`, `HeatingAndCoolingResultCalc.ClearValuesCO2`, `HeatingAndCoolingResultCalc.Co2CalculateEmissionEnergySupplyBuilding`, `HeatingAndCoolingResultCalc.Co2EnergyCalculateTotal`, `HeatingAndCoolingResultCalc.Co2GetFuelTypesBuilding`
- External/API calls: `BuildingZones.First`

### HeatingAndCoolingResultCalc.ZoneCO2Calculations
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8620`
- Internal calls: `HeatingAndCoolingResultCalc.CO2EnergyZoneCalculations`, `HeatingAndCoolingResultCalc.CalculateCO2Emissions`, `HeatingAndCoolingResultCalc.CalculateFuelSavings`, `HeatingAndCoolingResultCalc.CalculateSavings`, `HeatingAndCoolingResultCalc.ClearFuelCellsCO2`, `HeatingAndCoolingResultCalc.ClearValuesCO2`, `HeatingAndCoolingResultCalc.Co2EnergyCalculateTotal`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetBuildingData
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8631`
- Internal calls: _None_
- External/API calls: `BuildingZones.Sum`

### HeatingAndCoolingResultCalc.ClearPrimaryEnergyFuelTableValues
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8645`
- Internal calls: `HeatingAndCoolingResultCalc.ClearValuesFuelActual`, `HeatingAndCoolingResultCalc.ClearValuesFuelBaseLine`, `HeatingAndCoolingResultCalc.ClearValuesFuelESM`, `HeatingAndCoolingResultCalc.ClearValuesFuelRef1`, `HeatingAndCoolingResultCalc.ClearValuesFuelRef2`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ClearValuesFuelRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8654`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ClearValuesFuelRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8669`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ClearValuesFuelActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8684`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ClearValuesFuelBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8699`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ClearValuesFuelESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8714`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.UpdateRefsState
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8729`
- Internal calls: `HeatingAndCoolingResultCalc.CalcTotalArea`
- External/API calls: `BuildingZones.First`, `BuildingZones.Sum`, `BuildingZones.Where`, `Sum`

### HeatingAndCoolingResultCalc.UpdateActualState
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8781`
- Internal calls: `HeatingAndCoolingResultCalc.CalcTotalArea`
- External/API calls: `BuildingZones.First`, `BuildingZones.Sum`, `BuildingZones.Where`, `Sum`

### HeatingAndCoolingResultCalc.UpdateBaseLineState
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8811`
- Internal calls: `HeatingAndCoolingResultCalc.CalcTotalArea`
- External/API calls: `BuildingZones.First`, `BuildingZones.Sum`, `BuildingZones.Where`, `Sum`

### HeatingAndCoolingResultCalc.UpdateEsmState
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8841`
- Internal calls: `HeatingAndCoolingResultCalc.CalcTotalArea`
- External/API calls: `BuildingZones.First`, `BuildingZones.Sum`, `BuildingZones.Where`, `Sum`

### HeatingAndCoolingResultCalc.GetVeiHeating
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8871`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateElectricityVEI`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetVeiHeatVentilation
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8889`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateElectricityVEI`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetVeiBGV
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8907`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateElectricityVEI`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateElectricityVEI
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8925`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalsNeededEnergyTable
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8930`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateTotalActual`, `HeatingAndCoolingResultCalc.CalculateTotalActualYearly`, `HeatingAndCoolingResultCalc.CalculateTotalBaseLine`, `HeatingAndCoolingResultCalc.CalculateTotalBaseLineYearly`, `HeatingAndCoolingResultCalc.CalculateTotalEsm`, `HeatingAndCoolingResultCalc.CalculateTotalEsmYearly`, `HeatingAndCoolingResultCalc.CalculateTotalRefs`, `HeatingAndCoolingResultCalc.CalculateTotalRefsYearly`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalRefsYearly
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8942`
- Internal calls: `HeatingAndCoolingResultCalc.CheckForNaN`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalRefs
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8955`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalEsmYearly
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8968`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalVei
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8978`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8991`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalBaseLineYearly
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9001`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9011`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalActualYearly
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9021`
- Internal calls: `HeatingAndCoolingResultCalc.CheckForNaN`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9031`
- Internal calls: `HeatingAndCoolingResultCalc.CheckForNaN`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetFuelTypeAndValues
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9041`
- Internal calls: `HeatingAndCoolingResultCalc.GetFuelType`, `HeatingAndCoolingResultCalc.GetFuelTypeBaseLine`, `HeatingAndCoolingResultCalc.GetFuelTypeEsm`, `HeatingAndCoolingResultCalc.GetFuelTypeRef1`, `HeatingAndCoolingResultCalc.GetFuelTypeRef2`, `HeatingAndCoolingResultCalc.GetVeiBGV`, `HeatingAndCoolingResultCalc.GetVeiHeatVentilation`, `HeatingAndCoolingResultCalc.GetVeiHeating`
- External/API calls: `Math.Min`

### HeatingAndCoolingResultCalc.GetFuelTypeRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9191`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.GetFuelTypeRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9235`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.GetFuelType
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9279`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.GetFuelTypeBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9323`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.GetFuelTypeEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9367`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateTotalFuelEnergy
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9411`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateTotalFuelActual`, `HeatingAndCoolingResultCalc.CalculateTotalFuelBaseLine`, `HeatingAndCoolingResultCalc.CalculateTotalFuelESM`, `HeatingAndCoolingResultCalc.CalculateTotalFuelRef1`, `HeatingAndCoolingResultCalc.CalculateTotalFuelRef2`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalFuelRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9420`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalFuelRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9425`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalFuelActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9430`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalFuelBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9435`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalFuelESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9440`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ClearFuelCells
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9445`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ClearFuelCellsCO2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9526`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CheckForNaN
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9585`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateNetEnergyByTechnologies
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9594`
- Internal calls: `HeatingAndCoolingResultCalc.CheckForNaN`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateNetEnergyPerArea
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9655`
- Internal calls: `HeatingAndCoolingResultCalc.CheckForNaN`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateNetWithoutInputsEnergyByTechnologiesPerArea
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9687`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateNetEnergyByTechnologiesBuilding
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9715`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ClearNeededVEIenergy
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9796`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ClearNetEnergy
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9808`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateNetWithoutInputsEnergyByTechnologies
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9862`
- Internal calls: `HeatingAndCoolingResultCalc.CheckForNaN`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ClearNetEnergyWithoutInputs
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9923`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ClearValuesCO2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9975`
- Internal calls: `HeatingAndCoolingResultCalc.ClearValuesCO2Actual`, `HeatingAndCoolingResultCalc.ClearValuesCO2BaseLine`, `HeatingAndCoolingResultCalc.ClearValuesCO2ESM`, `HeatingAndCoolingResultCalc.ClearValuesCO2Ref1`, `HeatingAndCoolingResultCalc.ClearValuesCO2Ref2`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ClearValuesCO2Ref1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:9984`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ClearValuesCO2Ref2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10001`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ClearValuesCO2Actual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10018`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ClearValuesCO2BaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10035`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.ClearValuesCO2ESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10052`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateCO2Emissions
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10069`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateCO2EmissionsActual`, `HeatingAndCoolingResultCalc.CalculateCO2EmissionsBaseLine`, `HeatingAndCoolingResultCalc.CalculateCO2EmissionsESM`, `HeatingAndCoolingResultCalc.CalculateCO2EmissionsRef1`, `HeatingAndCoolingResultCalc.CalculateCO2EmissionsRef2`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateCO2EmissionsRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10078`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateTotalCO2Ref1`, `HeatingAndCoolingResultCalc.GetEkoCoeficient`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalCO2Ref1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10119`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateCO2EmissionsRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10125`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateTotalCO2Ref2`, `HeatingAndCoolingResultCalc.GetEkoCoeficient`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalCO2Ref2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10166`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateCO2EmissionsActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10172`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateTotalCO2Actual`, `HeatingAndCoolingResultCalc.GetEkoCoeficient`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalCO2Actual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10219`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateCO2EmissionsBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10225`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateTotalCO2BaseLine`, `HeatingAndCoolingResultCalc.GetEkoCoeficient`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalCO2BaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10272`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateCO2EmissionsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10278`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateTotalCO2ESM`, `HeatingAndCoolingResultCalc.GetEkoCoeficient`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTotalCO2ESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10325`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10331`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateFuelSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10347`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CO2EnergyZoneCalculations
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10363`
- Internal calls: `HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneActual`, `HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneBaseLine`, `HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneESM`, `HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneRef1`, `HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneRef2`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10372`
- Internal calls: `HeatingAndCoolingResultCalc.GetFuelTypeCo2Ref1`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10413`
- Internal calls: `HeatingAndCoolingResultCalc.GetFuelTypeCo2Ref2`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10454`
- Internal calls: `HeatingAndCoolingResultCalc.GetFuelTypeCo2Actual`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10501`
- Internal calls: `HeatingAndCoolingResultCalc.GetFuelTypeCo2BaseLine`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10548`
- Internal calls: `HeatingAndCoolingResultCalc.GetFuelTypeCo2ESM`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.Co2GetFuelTypesBuilding
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10595`
- Internal calls: `HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingActual`, `HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingBaseLine`, `HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingESM`, `HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingRef1`, `HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingRef2`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10604`
- Internal calls: `HeatingAndCoolingResultCalc.GetFuelTypeCo2Ref1`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10634`
- Internal calls: `HeatingAndCoolingResultCalc.GetFuelTypeCo2Ref2`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10664`
- Internal calls: `HeatingAndCoolingResultCalc.GetFuelTypeCo2Actual`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10700`
- Internal calls: `HeatingAndCoolingResultCalc.GetFuelTypeCo2BaseLine`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10736`
- Internal calls: `HeatingAndCoolingResultCalc.GetFuelTypeCo2ESM`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.Co2CalculateEmissionEnergySupplyBuilding
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10772`
- Internal calls: `HeatingAndCoolingResultCalc.Co2EnergyCalcBuildingActual`, `HeatingAndCoolingResultCalc.Co2EnergyCalcBuildingBaseLine`, `HeatingAndCoolingResultCalc.Co2EnergyCalcBuildingESM`, `HeatingAndCoolingResultCalc.Co2EnergyCalcBuildingRef1`, `HeatingAndCoolingResultCalc.Co2EnergyCalcBuildingRef2`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.Co2EnergyCalcBuildingRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10781`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.Co2EnergyCalcBuildingRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10797`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.Co2EnergyCalcBuildingActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10813`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.Co2EnergyCalcBuildingBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10829`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.Co2EnergyCalcBuildingESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10845`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.Co2EnergyCalculateTotal
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10861`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetFuelTypeCo2Ref1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10870`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.GetFuelTypeCo2Ref2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10914`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.GetFuelTypeCo2Actual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:10958`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.GetFuelTypeCo2BaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11002`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.GetFuelTypeCo2ESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11046`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.GetEkoCoeficient
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11090`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalcTotalArea
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11113`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetConditionedArea
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11118`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.SetFuelValue
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11124`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateFuelActual`, `HeatingAndCoolingResultCalc.CalculateFuelBaseLine`, `HeatingAndCoolingResultCalc.CalculateFuelESM`, `HeatingAndCoolingResultCalc.CalculateFuelRef1`, `HeatingAndCoolingResultCalc.CalculateFuelRef2`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateFuelESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11133`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateFuelBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11148`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateFuelActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11163`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateFuelRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11178`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateFuelRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11193`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateHeatingSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11208`
- Internal calls: `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CalculateEnergy`, `HeatingAndCoolingResultCalc.CalculateEnergyESM`, `HeatingAndCoolingResultCalc.CalculateUsavingType`, `HeatingAndCoolingResultCalc.CalculateUsavingTypeESM`, `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSources`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSourcesESM`, `HeatingAndCoolingResultCalc.CheckForFuelSavings`, `HeatingAndCoolingResultCalc.CheckForSavings`, `HeatingAndCoolingResultCalc.CopyHeatingWorkingSchedule`, `HeatingAndCoolingResultCalc.CopyHeatingWorkingScheduleESM`, `HeatingAndCoolingResultCalc.CreateHeatingVirtualBaseLine`, `HeatingAndCoolingResultCalc.CreateHeatingVirtualESM`, `HeatingAndCoolingResultCalc.GetBaseLine`, `HeatingAndCoolingResultCalc.GetESM`, `HeatingAndCoolingResultCalc.SetBaseLine`, `HeatingAndCoolingResultCalc.SetESM`, `HeatingAndCoolingResultCalc.SetSavingsValues`
- External/API calls: `Convert.ToDouble`, `Deserialize`, `Tag.StartsWith`, `ToList`, `calcData.Clone`, `list.Any`, `list.Sum`, `list2.FirstOrDefault`, `list3.FirstOrDefault`, `section.Serialize`, `source.FirstOrDefault`, `source2.FirstOrDefault`

### HeatingAndCoolingResultCalc.CheckForDifferentFuelSources
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11314`
- Internal calls: _None_
- External/API calls: `Convert.ToInt32`

### HeatingAndCoolingResultCalc.CheckForDifferentFuelSourcesESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11362`
- Internal calls: _None_
- External/API calls: `Convert.ToInt32`

### HeatingAndCoolingResultCalc.CreateHeatingVirtualBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11410`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateEnergy`, `HeatingAndCoolingResultCalc.GetBaseLine`, `HeatingAndCoolingResultCalc.SetBaseLine`
- External/API calls: `baseLine.FirstOrDefault`

### HeatingAndCoolingResultCalc.CreateHeatingVirtualESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11433`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateEnergy`, `HeatingAndCoolingResultCalc.GetESM`, `HeatingAndCoolingResultCalc.SetESM`
- External/API calls: `eSM.FirstOrDefault`

### HeatingAndCoolingResultCalc.CalculateCoolingSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11456`
- Internal calls: `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM`, `HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingBaseLine`, `HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingESM`, `HeatingAndCoolingResultCalc.CalculateUsavingType`, `HeatingAndCoolingResultCalc.CalculateUsavingTypeESM`, `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSources`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSourcesESM`, `HeatingAndCoolingResultCalc.CheckForFuelSavings`, `HeatingAndCoolingResultCalc.CheckForSavings`, `HeatingAndCoolingResultCalc.CopyCoolingWorkingSchedule`, `HeatingAndCoolingResultCalc.CopyCoolingWorkingScheduleESM`, `HeatingAndCoolingResultCalc.CreateCoolingVirtualBaseLine`, `HeatingAndCoolingResultCalc.CreateCoolingVirtualESM`, `HeatingAndCoolingResultCalc.GetBaseLine`, `HeatingAndCoolingResultCalc.GetESM`, `HeatingAndCoolingResultCalc.SetBaseLine`, `HeatingAndCoolingResultCalc.SetESM`, `HeatingAndCoolingResultCalc.SetSavingsValues`, `InputDataCalc.CalcPeriod`
- External/API calls: `Convert.ToDouble`, `Deserialize`, `Tag.StartsWith`, `ToList`, `calcData.Clone`, `list.Any`, `list.Sum`, `list2.FirstOrDefault`, `list3.FirstOrDefault`, `section.Serialize`, `source.FirstOrDefault`, `source2.FirstOrDefault`

### HeatingAndCoolingResultCalc.CreateCoolingVirtualBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11560`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine`, `HeatingAndCoolingResultCalc.GetBaseLine`, `HeatingAndCoolingResultCalc.SetBaseLine`
- External/API calls: `baseLine.FirstOrDefault`

### HeatingAndCoolingResultCalc.CreateCoolingVirtualESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11578`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM`, `HeatingAndCoolingResultCalc.GetESM`, `HeatingAndCoolingResultCalc.SetESM`
- External/API calls: `eSM.FirstOrDefault`

### HeatingAndCoolingResultCalc.CalculateVentilationHeatingSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11596`
- Internal calls: `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyBaseLine`, `HeatingAndCoolingResultCalc.CalculateVentNeededEnergyBaseLine`, `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSources`, `HeatingAndCoolingResultCalc.CheckForFuelSavings`, `HeatingAndCoolingResultCalc.CheckForVentilationSavings`, `HeatingAndCoolingResultCalc.CopyVentilationHeatingWorkingSchedule`, `HeatingAndCoolingResultCalc.GetVentilationBaseLine`, `HeatingAndCoolingResultCalc.SetVentilationBaseLine`, `HeatingAndCoolingResultCalc.SetVentilationSavingsValues`, `HeatingAndCoolingResultCalc.VentilationHeatEnergyBaseLine`
- External/API calls: `Convert.ToDouble`, `Deserialize`, `ToList`, `calcData.Clone`, `list.Any`, `list.Sum`, `list2.FirstOrDefault`, `list3.FirstOrDefault`, `section.Serialize`, `ventilationBaseLine.FirstOrDefault`

### HeatingAndCoolingResultCalc.CalculateVentilationCoolingSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11694`
- Internal calls: `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyBaseLine`, `HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyBaseLine`, `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSources`, `HeatingAndCoolingResultCalc.CheckForFuelSavings`, `HeatingAndCoolingResultCalc.CheckForVentilationSavings`, `HeatingAndCoolingResultCalc.CopyVentilationCoolingWorkingSchedule`, `HeatingAndCoolingResultCalc.GetVentilationBaseLine`, `HeatingAndCoolingResultCalc.SetVentilationBaseLine`, `HeatingAndCoolingResultCalc.SetVentilationSavingsValues`, `HeatingAndCoolingResultCalc.VentilationCoolEnergyBaseLine`
- External/API calls: `Convert.ToDouble`, `Deserialize`, `ToList`, `calcData.Clone`, `list.Any`, `list.Sum`, `list2.FirstOrDefault`, `list3.FirstOrDefault`, `section.Serialize`, `ventilationBaseLine.FirstOrDefault`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11792`
- Internal calls: `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings`, `HeatingAndCoolingResultCalc.CheckHeatingForFansAndPumpsSavings`, `HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursEsm`, `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursEsm`, `HeatingAndCoolingResultCalc.SetHeatingFansAndPumpsSavingsValues`, `InputDataCalc.CalcPeriod`
- External/API calls: `Convert.ToDouble`, `list.Any`, `source.Sum`

### HeatingAndCoolingResultCalc.SetHeatingFansAndPumpsSavingsValues
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11844`
- Internal calls: `HeatingAndCoolingResultCalc.GetSaving`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CheckHeatingForFansAndPumpsSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11852`
- Internal calls: _None_
- External/API calls: `list.Add`, `object.Equals`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11902`
- Internal calls: `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CheckCoolingForFansAndPumpsSavings`, `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursEsm`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursEsm`, `HeatingAndCoolingResultCalc.SetCoolingFansAndPumpsSavingsValues`, `InputDataCalc.CalcPeriod`
- External/API calls: `list.Any`, `source.Sum`

### HeatingAndCoolingResultCalc.SetCoolingFansAndPumpsSavingsValues
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11964`
- Internal calls: `HeatingAndCoolingResultCalc.GetSaving`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CheckCoolingForFansAndPumpsSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11974`
- Internal calls: _None_
- External/API calls: `list.Add`, `object.Equals`

### HeatingAndCoolingResultCalc.CalculateLightsSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12046`
- Internal calls: `HeatingAndCoolingResultCalc.CalculatePeriod`, `InputDataCalc.CalcPeriod`
- External/API calls: `ToString`, `source.Sum`

### HeatingAndCoolingResultCalc.CalculateBalancedDevicesSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12067`
- Internal calls: `HeatingAndCoolingResultCalc.CalculatePeriod`, `InputDataCalc.CalcPeriod`
- External/API calls: `ToString`, `source.Sum`

### HeatingAndCoolingResultCalc.CalculateNonBalancedDevicesSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12088`
- Internal calls: `HeatingAndCoolingResultCalc.CalculatePeriod`, `InputDataCalc.CalcPeriod`
- External/API calls: `ToString`, `source.Sum`

### HeatingAndCoolingResultCalc.CalculateHotWaterPumpsSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12109`
- Internal calls: `HeatingAndCoolingResultCalc.CalculatePeriod`, `InputDataCalc.CalcPeriod`
- External/API calls: `source.Sum`

### HeatingAndCoolingResultCalc.CalculatePeriod
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12127`
- Internal calls: `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings`, `HeatingAndCoolingResultCalc.CheckLightsAndDevicesSavings`, `HeatingAndCoolingResultCalc.SetLightsAndDevicesSavingsvalues`
- External/API calls: `Convert.ToDouble`, `list.Any`, `list.Sum`

### HeatingAndCoolingResultCalc.SetLightsAndDevicesSavingsvalues
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12197`
- Internal calls: `HeatingAndCoolingResultCalc.GetSaving`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CheckLightsAndDevicesSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12203`
- Internal calls: _None_
- External/API calls: `list.Add`, `object.Equals`

### HeatingAndCoolingResultCalc.CalculateHotWaterSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12231`
- Internal calls: `HeatingAndCoolingResultCalc.AddSavingsToBuilding`, `HeatingAndCoolingResultCalc.CalculateGeneratorHotWaterEfficiencyBaseLine`, `HeatingAndCoolingResultCalc.CalculateHotWaterNeededEnergyBaseLine`, `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSources`, `HeatingAndCoolingResultCalc.CheckForFuelSavings`, `HeatingAndCoolingResultCalc.CheckForHotWaterSavings`, `HeatingAndCoolingResultCalc.GetHotWaterBaseLine`, `HeatingAndCoolingResultCalc.HotWaterCalculationBaseLine`, `HeatingAndCoolingResultCalc.SetHotWaterBaseLine`, `HeatingAndCoolingResultCalc.SetHotWaterSavingsValues`
- External/API calls: `Convert.ToDouble`, `ToList`, `calcData.Clone`, `hotWaterBaseLine.FirstOrDefault`, `list.Add`, `list.Any`, `list.Sum`, `list2.FirstOrDefault`, `list3.FirstOrDefault`, `object.Equals`

### HeatingAndCoolingResultCalc.CheckForHotWaterSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12319`
- Internal calls: _None_
- External/API calls: `list.Add`, `object.Equals`

### HeatingAndCoolingResultCalc.SetHotWaterSavingsValues
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12358`
- Internal calls: `HeatingAndCoolingResultCalc.GetSaving`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.AddSavingsToZone
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12377`
- Internal calls: _None_
- External/API calls: `ActualSaving.ToString`, `ZoneSavings.Add`, `ZoneSavings.Any`, `ZoneSavings.Remove`, `ZoneSavings.ToList`, `double.IsInfinity`, `double.IsNaN`, `savings.Any`, `source.Where`

### HeatingAndCoolingResultCalc.AddSavingsToBuilding
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12408`
- Internal calls: _None_
- External/API calls: `ActualSaving.ToString`, `BuildingSavings.Add`, `BuildingSavings.Any`, `BuildingSavings.Remove`, `BuildingSavings.ToList`, `BuildingSavings.Where`, `ToList`, `double.IsInfinity`, `double.IsNaN`, `list.Any`, `savings.Any`, `source.Where`

### HeatingAndCoolingResultCalc.SetSavingsValues
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12454`
- Internal calls: `HeatingAndCoolingResultCalc.GetSaving`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CheckForSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12486`
- Internal calls: _None_
- External/API calls: `list.Add`, `object.Equals`

### HeatingAndCoolingResultCalc.CheckForFuelSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12648`
- Internal calls: _None_
- External/API calls: `list.Add`, `object.Equals`

### HeatingAndCoolingResultCalc.SetVentilationSavingsValues
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12798`
- Internal calls: `HeatingAndCoolingResultCalc.GetSaving`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CheckForVentilationSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12825`
- Internal calls: _None_
- External/API calls: `list.Add`, `object.Equals`

### HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12930`
- Internal calls: _None_
- External/API calls: `Convert.ToDouble`, `Math.Abs`, `Sum`, `ToList`, `savings.Where`, `source.Sum`

### HeatingAndCoolingResultCalc.GetValue
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12945`
- Internal calls: _None_
- External/API calls: `baseLine.FirstOrDefault`

### HeatingAndCoolingResultCalc.GetSaving
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12956`
- Internal calls: _None_
- External/API calls: `ActualSaving.ToString`, `savings.FirstOrDefault`

### HeatingAndCoolingResultCalc.CalculateEnergy
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12966`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyBaseLine`, `HeatingAndCoolingResultCalc.CalculateNeededEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateNetEnergy`, `HeatingAndCoolingResultCalc.Calculations`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateEnergyESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12974`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyEsm`, `HeatingAndCoolingResultCalc.CalculateNeededEnergyEsm`, `HeatingAndCoolingResultCalc.CalculateNetEnergy`, `HeatingAndCoolingResultCalc.Calculations`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:12982`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13189`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetVentilationBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13396`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.SetBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13563`
- Internal calls: `HeatingAndCoolingResultCalc.GetValue`
- External/API calls: `baseLine.FirstOrDefault`

### HeatingAndCoolingResultCalc.SetESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13623`
- Internal calls: `HeatingAndCoolingResultCalc.GetValue`
- External/API calls: `esm.FirstOrDefault`

### HeatingAndCoolingResultCalc.SetVentilationBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13683`
- Internal calls: `HeatingAndCoolingResultCalc.GetValue`
- External/API calls: `baseLine.FirstOrDefault`

### HeatingAndCoolingResultCalc.GetHotWaterBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13736`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.SetHotWaterBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13868`
- Internal calls: `HeatingAndCoolingResultCalc.GetValue`
- External/API calls: `baseLine.FirstOrDefault`

### HeatingAndCoolingResultCalc.CalculateUsavingType
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13912`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateGsavings`, `HeatingAndCoolingResultCalc.CalculateUInnerWallsSaving`, `HeatingAndCoolingResultCalc.CalculateUOuterWallsSaving`, `HeatingAndCoolingResultCalc.CalculateUceilingsavings`, `HeatingAndCoolingResultCalc.CalculateUfloorOthersavings`, `HeatingAndCoolingResultCalc.CalculateUfloorSavings`, `HeatingAndCoolingResultCalc.CalculateUnonTransparentSavings`, `HeatingAndCoolingResultCalc.CalculateUwindowsSavings`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateUsavingTypeESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13941`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateGsavingsESM`, `HeatingAndCoolingResultCalc.CalculateUInnerWallsSavingESM`, `HeatingAndCoolingResultCalc.CalculateUOuterWallsSavingESM`, `HeatingAndCoolingResultCalc.CalculateUceilingsavingsESM`, `HeatingAndCoolingResultCalc.CalculateUfloorOthersavingsESM`, `HeatingAndCoolingResultCalc.CalculateUfloorSavingsESM`, `HeatingAndCoolingResultCalc.CalculateUnonTransparentSavingsESM`, `HeatingAndCoolingResultCalc.CalculateUwindowsSavingsESM`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyHeatingWorkingSchedule
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13972`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyHeatingWorkingScheduleESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13982`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyVentilationHeatingWorkingSchedule
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13992`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyCoolingWorkingSchedule
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14002`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyCoolingWorkingScheduleESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14012`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyVentilationCoolingWorkingSchedule
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14022`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateGsavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14032`
- Internal calls: `HeatingAndCoolingResultCalc.CopyTrasparentGelements`, `HeatingAndCoolingResultCalc.CopyWindowselements`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateGsavingsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14045`
- Internal calls: `HeatingAndCoolingResultCalc.CopyTrasparentGelementsESM`, `HeatingAndCoolingResultCalc.CopyWindowselementsESM`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyWindowselements
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14058`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyWindowselementsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14069`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyTrasparentGelements
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14080`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyTrasparentGelementsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14094`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateUfloorSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14108`
- Internal calls: `HeatingAndCoolingResultCalc.CopyFloorElements`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateUfloorSavingsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14113`
- Internal calls: `HeatingAndCoolingResultCalc.CopyFloorElementsESM`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyFloorElements
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14118`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyFloorElementsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14129`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateUfloorOthersavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14140`
- Internal calls: `HeatingAndCoolingResultCalc.CopyOtherFloorElements`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateUfloorOthersavingsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14145`
- Internal calls: `HeatingAndCoolingResultCalc.CopyOtherFloorElementsESM`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyOtherFloorElements
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14150`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyOtherFloorElementsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14161`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateUOuterWallsSaving
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14172`
- Internal calls: `HeatingAndCoolingResultCalc.CopyOuterWallsElements`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateUOuterWallsSavingESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14184`
- Internal calls: `HeatingAndCoolingResultCalc.CopyOuterWallsElementsESM`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateUInnerWallsSaving
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14196`
- Internal calls: `HeatingAndCoolingResultCalc.CopyInnerWallsElements`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateUInnerWallsSavingESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14208`
- Internal calls: `HeatingAndCoolingResultCalc.CopyInnerWallsElementsESM`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyOuterWallsElements
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14220`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyOuterWallsElementsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14231`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyInnerWallsElements
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14242`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyInnerWallsElementsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14253`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateUwindowsSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14264`
- Internal calls: `HeatingAndCoolingResultCalc.CopyWindowsElements`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateUwindowsSavingsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14276`
- Internal calls: `HeatingAndCoolingResultCalc.CopyWindowsElementsESM`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyWindowsElements
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14288`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyWindowsElementsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14299`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateUnonTransparentSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14310`
- Internal calls: `HeatingAndCoolingResultCalc.CopyNonTrasparentElements`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateUnonTransparentSavingsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14315`
- Internal calls: `HeatingAndCoolingResultCalc.CopyNonTrasparentElementsESM`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyNonTrasparentElements
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14320`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyNonTrasparentElementsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14334`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateUceilingsavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14348`
- Internal calls: `HeatingAndCoolingResultCalc.CopyCeilingElements`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateUceilingsavingsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14353`
- Internal calls: `HeatingAndCoolingResultCalc.CopyCeilingElementsESM`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyCeilingElements
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14358`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CopyCeilingElementsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14372`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.SetScaleType
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14386`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateHotWaterNeededPower
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14431`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateParameterF`, `HeatingAndCoolingResultCalc.CalculateParameterHtMonthly`, `HeatingAndCoolingResultCalc.CalculateParameterX`, `HeatingAndCoolingResultCalc.CalculateParameterY`, `HeatingAndCoolingResultCalc.CalculateXwithCorrection`, `HeatingAndCoolingResultCalc.ClearTableValues`, `HeatingAndCoolingResultCalc.HotWaterNeededPower`, `HeatingAndCoolingResultCalc.HotWaterNeededPowerTotal`, `HeatingAndCoolingResultCalc.SetTableResults`, `HeatingAndCoolingResultCalc.SumCollectorsArea`, `InputDataCalc.CalcPeriod`, `SunEnergyPreferencesManager.GetClimateZoneParams`
- External/API calls: `Math.Abs`, `Math.Round`, `MessageBox.Show`, `SunMonth`, `d.ToString`, `d2.ToString`, `d3.ToString`, `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list.Aggregate`, `list2.Add`, `list2.Aggregate`, `list3.Add`, `list3.Aggregate`, `list4.Add`, `list4.Aggregate`

### HeatingAndCoolingResultCalc.CalculateXwithCorrection
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14514`
- Internal calls: _None_
- External/API calls: `Math.Pow`

### HeatingAndCoolingResultCalc.ClearTableValues
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14528`
- Internal calls: `HeatingAndCoolingResultCalc.SetNullValues`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.SetNullValues
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14575`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.SetTableResults
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14587`
- Internal calls: `HeatingAndCoolingResultCalc.SetMonthRowValues`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.SetMonthRowValues
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14631`
- Internal calls: _None_
- External/API calls: `int?`

### HeatingAndCoolingResultCalc.SumCollectorsArea
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14644`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterF
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14649`
- Internal calls: _None_
- External/API calls: `Math.Pow`

### HeatingAndCoolingResultCalc.CalculateParameterX
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14654`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateTOAeffect`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateParameterY
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14666`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateParameterHtMonthly`, `HeatingAndCoolingResultCalc.CalculateTOAeffect`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateTOAeffect
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14681`
- Internal calls: _None_
- External/API calls: `Math.Pow`, `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.HotWaterNeededPower
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14706`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.HotWaterNeededPowerTotal
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14711`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.DefuseradiationHd
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14716`
- Internal calls: `SunEnergyPreferencesManager.GetClimateZoneParams`
- External/API calls: `Math.Pow`

### HeatingAndCoolingResultCalc.SunDeclination
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14722`
- Internal calls: _None_
- External/API calls: `Math.Sin`

### HeatingAndCoolingResultCalc.SunsetHour
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14767`
- Internal calls: `HeatingAndCoolingResultCalc.SunDeclination`
- External/API calls: `Math.Acos`, `Math.Tan`

### HeatingAndCoolingResultCalc.SunsetHourPrim
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14773`
- Internal calls: `HeatingAndCoolingResultCalc.SubAngles`, `HeatingAndCoolingResultCalc.SunDeclination`, `HeatingAndCoolingResultCalc.SunsetHour`
- External/API calls: `Math.Acos`, `Math.Tan`

### HeatingAndCoolingResultCalc.CalculateMonthlyHorizontalRadiation
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14783`
- Internal calls: `HeatingAndCoolingResultCalc.SubAngles`, `HeatingAndCoolingResultCalc.SunDeclination`, `HeatingAndCoolingResultCalc.SunsetHour`, `HeatingAndCoolingResultCalc.SunsetHourPrim`
- External/API calls: `Math.Cos`, `Math.Sin`

### HeatingAndCoolingResultCalc.SubAngles
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14795`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateProjectionCoeficient
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14800`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateMonthlyHorizontalRadiation`, `HeatingAndCoolingResultCalc.DefuseradiationHd`
- External/API calls: `Math.Cos`

### HeatingAndCoolingResultCalc.CalculateParameterHtMonthly
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14809`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateProjectionCoeficient`, `SunEnergyPreferencesManager.GetClimateZoneParams`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.VentilationCoolEnergyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14814`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateCoolingInputsRef1`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyRef1`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyRef1`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsNaN`, `list.Add`, `list.Aggregate`, `list2.Add`, `list2.Aggregate`, `list3.Add`, `list3.Aggregate`, `list4.Add`, `list4.Aggregate`

### HeatingAndCoolingResultCalc.VentilationCoolEnergyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14842`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateCoolingInputsRef2`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyRef2`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyRef2`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsNaN`, `list.Add`, `list.Aggregate`, `list2.Add`, `list2.Aggregate`, `list3.Add`, `list3.Aggregate`, `list4.Add`, `list4.Aggregate`

### HeatingAndCoolingResultCalc.VentilationCoolEnergyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14870`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateCoolingInputs`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyActual`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyActual`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsNaN`, `list.Add`, `list.Aggregate`, `list2.Add`, `list2.Aggregate`, `list3.Add`, `list3.Aggregate`, `list4.Add`, `list4.Aggregate`

### HeatingAndCoolingResultCalc.VentilationCoolEnergyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14898`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateCoolingInputsBaseLine`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyBaseLine`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsNaN`, `list.Add`, `list.Aggregate`, `list2.Add`, `list2.Aggregate`, `list3.Add`, `list3.Aggregate`, `list4.Add`, `list4.Aggregate`

### HeatingAndCoolingResultCalc.VentilationCoolEnergyEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14926`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateCoolingInputsESM`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyESM`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyESM`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsNaN`, `list.Add`, `list.Aggregate`, `list2.Add`, `list2.Aggregate`, `list3.Add`, `list3.Aggregate`, `list4.Add`, `list4.Aggregate`

### HeatingAndCoolingResultCalc.GetWeekHoursCoolingReferences
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14954`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekHoursCoolingActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14962`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekHoursCoolingBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14970`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekHoursCoolingEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14978`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14986`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15005`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15024`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15043`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15062`
- Internal calls: _None_
- External/API calls: `ToString`, `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateCoolingInputsRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15082`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateCoolingInputsRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15108`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateCoolingInputs
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15134`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateCoolingInputsBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15160`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateCoolingInputsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15186`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetDaysHours
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15212`
- Internal calls: `PreferencesManager.GetClimateZoneParams`
- External/API calls: `list.AddRange`

### HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15220`
- Internal calls: `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.CalculateEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: `Math.Abs`

### HeatingAndCoolingResultCalc.CalculateWitheringEnergyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15275`
- Internal calls: `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.CalculateWitheringEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15302`
- Internal calls: `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.CalculateEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: `Math.Abs`

### HeatingAndCoolingResultCalc.CalculateWitheringEnergyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15357`
- Internal calls: `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.CalculateWitheringEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15384`
- Internal calls: `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.CalculateEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: `Math.Abs`

### HeatingAndCoolingResultCalc.CalculateWitheringEnergyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15439`
- Internal calls: `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.CalculateWitheringEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15466`
- Internal calls: `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.CalculateEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: `Math.Abs`

### HeatingAndCoolingResultCalc.CalculateWitheringEnergyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15521`
- Internal calls: `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.CalculateWitheringEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15548`
- Internal calls: `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.CalculateEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: `Math.Abs`

### HeatingAndCoolingResultCalc.CalculateWitheringEnergyESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15603`
- Internal calls: `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.CalculateWitheringEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateEntalpia
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15630`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateWitheringEntalpia
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15636`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.VentilationHeatEnergyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15642`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef1`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list.Aggregate`, `list2.Add`, `list2.Aggregate`, `list3.Add`, `list3.Aggregate`

### HeatingAndCoolingResultCalc.VentilationHeatEnergyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15676`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef2`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list.Aggregate`, `list2.Add`, `list2.Aggregate`, `list3.Add`, `list3.Aggregate`

### HeatingAndCoolingResultCalc.VentilationHeatEnergyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15710`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempActual`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyActual`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list.Aggregate`, `list2.Add`, `list2.Aggregate`, `list3.Add`, `list3.Aggregate`

### HeatingAndCoolingResultCalc.VentilationHeatEnergyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15753`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempBaseLine`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyBaseLine`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list.Aggregate`, `list2.Add`, `list2.Aggregate`, `list3.Add`, `list3.Aggregate`

### HeatingAndCoolingResultCalc.VentilationHeatEnergyESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15796`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempESM`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyESM`, `InputDataCalc.CalcPeriod`
- External/API calls: `double.IsInfinity`, `double.IsNaN`, `list.Add`, `list.Aggregate`, `list2.Add`, `list2.Aggregate`, `list3.Add`, `list3.Aggregate`

### HeatingAndCoolingResultCalc.CalculateVentNeededEnergyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15839`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateVentNeededEnergyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15871`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateVentNeededEnergyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15903`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateVentNeededEnergyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15935`
- Internal calls: _None_
- External/API calls: `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.CalculateVentNeededEnergyEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15967`
- Internal calls: _None_
- External/API calls: `ToString`, `double.IsInfinity`, `double.IsNaN`

### HeatingAndCoolingResultCalc.GetWeekHoursReferences
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16000`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekHoursActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16017`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekHoursBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16033`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetWeekHoursESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16049`
- Internal calls: `InputDataCalc.CalcHours`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16065`
- Internal calls: `HeatingAndCoolingResultCalc.CalcEntalpia`, `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempRef1`, `HeatingAndCoolingResultCalc.GetMonthHoursBaseLine`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: `Aggregate`, `hours.Select`, `object.Equals`

### HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16103`
- Internal calls: `HeatingAndCoolingResultCalc.CalcEntalpia`, `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempRef2`, `HeatingAndCoolingResultCalc.GetMonthHoursBaseLine`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: `Aggregate`, `hours.Select`, `object.Equals`

### HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16141`
- Internal calls: `HeatingAndCoolingResultCalc.CalcEntalpia`, `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempActual`, `HeatingAndCoolingResultCalc.GetMonthHoursActual`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: `Aggregate`, `hours.Select`, `object.Equals`

### HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16179`
- Internal calls: `HeatingAndCoolingResultCalc.CalcEntalpia`, `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempBaseLine`, `HeatingAndCoolingResultCalc.GetMonthHoursBaseLine`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: `Aggregate`, `hours.Select`, `object.Equals`

### HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16217`
- Internal calls: `HeatingAndCoolingResultCalc.CalcEntalpia`, `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempESM`, `HeatingAndCoolingResultCalc.GetMonthHoursESM`, `PreferencesManager.GetClimateZoneParams`
- External/API calls: `Aggregate`, `hours.Select`, `object.Equals`

### HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16255`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempBaseLine`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16260`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempBaseLine`
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16265`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16320`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16375`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetMonthHoursActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16430`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetMonthHoursBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16438`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.GetMonthHoursESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16446`
- Internal calls: _None_
- External/API calls: _None_

### HeatingAndCoolingResultCalc.CalcEntalpia
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16454`
- Internal calls: _None_
- External/API calls: `Math.Pow`

### InputDataCalc.CalcPeriod
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.InputDataCalc.cs:13`
- Internal calls: `InputDataCalc.CalculateMonthlyDays`
- External/API calls: `Enum.GetValues`, `ToList`, `list.Add`

### InputDataCalc.CalcHours
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.InputDataCalc.cs:43`
- Internal calls: _None_
- External/API calls: _None_

### InputDataCalc.CalculateMonthlyDays
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.InputDataCalc.cs:48`
- Internal calls: `InputDataCalc.GetHollydays`, `InputDataCalc.GetWeeksInMonth`
- External/API calls: `DateTime`, `DateTime.DaysInMonth`, `list.Add`, `period.First`, `period.Last`

### InputDataCalc.GetWeeksInMonth
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.InputDataCalc.cs:272`
- Internal calls: _None_
- External/API calls: _None_

### InputDataCalc.GetHollydays
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.InputDataCalc.cs:281`
- Internal calls: _None_
- External/API calls: _None_

### MonthlyDays.MonthlyDays
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.MonthlyDays.cs:23`
- Internal calls: _None_
- External/API calls: _None_

### MonthlyDays.MonthlyDays
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.MonthlyDays.cs:32`
- Internal calls: _None_
- External/API calls: _None_

### PreferencesManager.GetClimateZoneParams
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.PreferencesManager.cs:21`
- Internal calls: _None_
- External/API calls: `ClimateZones.Single`

### RoofTableCalc.CalculateArea
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:17`
- Internal calls: `Calculator.SumFields`
- External/API calls: _None_

### RoofTableCalc.CalculateNonTranspU
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:22`
- Internal calls: `Calculator.AcumulateWeight`
- External/API calls: _None_

### RoofTableCalc.SumL
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:28`
- Internal calls: `Calculator.SumFields`
- External/API calls: _None_

### RoofTableCalc.SumX
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:45`
- Internal calls: `Calculator.SumFields`
- External/API calls: _None_

### RoofTableCalc.CalculateEpsilon
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:62`
- Internal calls: `Calculator.AcumulateWeight`
- External/API calls: _None_

### RoofTableCalc.CalculateAlfa
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:68`
- Internal calls: `Calculator.AcumulateWeight`
- External/API calls: _None_

### RoofTableCalc.SumTrasparentArea
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:74`
- Internal calls: `Calculator.SumFields`
- External/API calls: _None_

### RoofTableCalc.CalculateTrasparentU
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:79`
- Internal calls: `Calculator.AcumulateWeight`
- External/API calls: _None_

### RoofTableCalc.CalculateTrasparentG
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:85`
- Internal calls: `Calculator.AcumulateWeight`
- External/API calls: _None_

### RoofTableCalc.SumCeilingArea
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:91`
- Internal calls: `Calculator.SumFields`
- External/API calls: _None_

### RoofTableCalc.CalculateCeilingU
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs:96`
- Internal calls: `Calculator.AcumulateWeight`
- External/API calls: _None_

### SavingsData.OnPropertyChanged
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.SavingsData.cs:176`
- Internal calls: _None_
- External/API calls: `PropertyChangedEventArgs`, `this.PropertyChanged`

### SunEnergyPreferencesManager.GetClimateZoneParams
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.SunEnergyPreferencesManager.cs:21`
- Internal calls: _None_
- External/API calls: `ClimateZones.Single`

### TempBridgeCalc.CalculateSums
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.TempBridgeCalc.cs:9`
- Internal calls: _None_
- External/API calls: _None_

### WallsTableCalc.SumColumnOuterArea
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:17`
- Internal calls: `Calculator.SumFields`
- External/API calls: _None_

### WallsTableCalc.AccumulateOuterU
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:22`
- Internal calls: `Calculator.AcumulateWeight`
- External/API calls: _None_

### WallsTableCalc.SumColumnOuterL
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:28`
- Internal calls: `Calculator.SumFields`
- External/API calls: _None_

### WallsTableCalc.SumColumnOuterX
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:42`
- Internal calls: `Calculator.SumFields`
- External/API calls: _None_

### WallsTableCalc.AcumulateOuterEpsilon
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:56`
- Internal calls: `Calculator.AcumulateWeight`
- External/API calls: _None_

### WallsTableCalc.AcumulateOuterAlfa
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:62`
- Internal calls: `Calculator.AcumulateWeight`
- External/API calls: _None_

### WallsTableCalc.SumColumnInnerArea
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:68`
- Internal calls: `Calculator.SumFields`
- External/API calls: _None_

### WallsTableCalc.CalculateInnerU
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:73`
- Internal calls: `Calculator.AcumulateWeight`
- External/API calls: _None_

### WallsTableCalc.SumWindowArea
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:79`
- Internal calls: `Calculator.SumFields`
- External/API calls: _None_

### WallsTableCalc.CalculateWindowU
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:84`
- Internal calls: `Calculator.AcumulateWeight`
- External/API calls: _None_

### WallsTableCalc.CalculateWindowG
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:90`
- Internal calls: `Calculator.AcumulateWeight`
- External/API calls: _None_

### WallsTableCalc.CalculateWindowE
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs:96`
- Internal calls: `Calculator.AcumulateWeight`
- External/API calls: _None_

## Reverse Callers

- `BuildingTypesManager.GetClimateZoneParams` <- `HeatingAndCoolingResultCalc.SetScaleValues`
- `Calculator.AcumulateWeight` <- `FloorTableCalc.CalculateFloorU`, `FloorTableCalc.CalculateOtherFloorU`, `RoofTableCalc.CalculateAlfa`, `RoofTableCalc.CalculateCeilingU`, `RoofTableCalc.CalculateEpsilon`, `RoofTableCalc.CalculateNonTranspU`, `RoofTableCalc.CalculateTrasparentG`, `RoofTableCalc.CalculateTrasparentU`, `WallsTableCalc.AccumulateOuterU`, `WallsTableCalc.AcumulateOuterAlfa`, `WallsTableCalc.AcumulateOuterEpsilon`, `WallsTableCalc.CalculateInnerU`, `WallsTableCalc.CalculateWindowE`, `WallsTableCalc.CalculateWindowG`, `WallsTableCalc.CalculateWindowU`
- `Calculator.Calculate` <- _None_
- `Calculator.SumFields` <- `FloorTableCalc.CalculateFloorArea`, `FloorTableCalc.CalculateOtherFloorArea`, `FloorTableCalc.SumL`, `FloorTableCalc.SumX`, `RoofTableCalc.CalculateArea`, `RoofTableCalc.SumCeilingArea`, `RoofTableCalc.SumL`, `RoofTableCalc.SumTrasparentArea`, `RoofTableCalc.SumX`, `WallsTableCalc.SumColumnInnerArea`, `WallsTableCalc.SumColumnOuterArea`, `WallsTableCalc.SumColumnOuterL`, `WallsTableCalc.SumColumnOuterX`, `WallsTableCalc.SumWindowArea`
- `DataRow.OnPropertyChanged` <- _None_
- `FloorTableCalc.CalculateFloorArea` <- `HeatingAndCoolingResultCalc.ApplyUroofsAndCeilings`
- `FloorTableCalc.CalculateFloorU` <- `HeatingAndCoolingResultCalc.ApplyUroofsAndCeilings`
- `FloorTableCalc.CalculateOtherFloorArea` <- `HeatingAndCoolingResultCalc.ApplyUroofsAndCeilings`
- `FloorTableCalc.CalculateOtherFloorU` <- `HeatingAndCoolingResultCalc.ApplyUroofsAndCeilings`
- `FloorTableCalc.SumL` <- _None_
- `FloorTableCalc.SumX` <- _None_
- `HeatingAndCoolingResultCalc.AddSavingsToBuilding` <- `HeatingAndCoolingResultCalc.CalculateHotWaterSavings`
- `HeatingAndCoolingResultCalc.AddSavingsToZone` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingSavings`, `HeatingAndCoolingResultCalc.CalculateHeatingSavings`, `HeatingAndCoolingResultCalc.CalculatePeriod`, `HeatingAndCoolingResultCalc.CalculateVentilationCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateVentilationHeatingSavings`
- `HeatingAndCoolingResultCalc.ApplyCoefficientG` <- `HeatingAndCoolingResultCalc.ApplyValuesToTempSectionRef1`, `HeatingAndCoolingResultCalc.ApplyValuesToTempSectionRef2`
- `HeatingAndCoolingResultCalc.ApplyToTrasparentRoofs` <- `HeatingAndCoolingResultCalc.ApplyUdirectionWalls`
- `HeatingAndCoolingResultCalc.ApplyUdirectionWalls` <- `HeatingAndCoolingResultCalc.ApplyValuesToTempSectionRef1`, `HeatingAndCoolingResultCalc.ApplyValuesToTempSectionRef2`
- `HeatingAndCoolingResultCalc.ApplyUroofsAndCeilings` <- `HeatingAndCoolingResultCalc.ApplyValuesToTempSectionRef1`, `HeatingAndCoolingResultCalc.ApplyValuesToTempSectionRef2`
- `HeatingAndCoolingResultCalc.ApplyValuesToTempSectionRef1` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef1`, `HeatingAndCoolingResultCalc.Calculations`
- `HeatingAndCoolingResultCalc.ApplyValuesToTempSectionRef2` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef2`, `HeatingAndCoolingResultCalc.Calculations`
- `HeatingAndCoolingResultCalc.BuildingCO2Calculations` <- `HeatingAndCoolingResultCalc.BuildingCalculations`
- `HeatingAndCoolingResultCalc.BuildingCalculations` <- _None_
- `HeatingAndCoolingResultCalc.CO2EnergyZoneCalculations` <- `HeatingAndCoolingResultCalc.ZoneCO2Calculations`
- `HeatingAndCoolingResultCalc.CalcAirX` <- `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.CalculateEntalpia`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInf`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfBaseLine`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfESM`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfRef1`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfRef2`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVent`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentBaseLine`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentESM`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentRef1`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentRef2`, `HeatingAndCoolingResultCalc.CalculateWitheringEntalpia`
- `HeatingAndCoolingResultCalc.CalcAvgMonthPower` <- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActual`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActualBalanced`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActualHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActualNonBalanced`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLine`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLineBalanced`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLineHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLineNonBalanced`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESM`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESMBalanced`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESMHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESMNonBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActual`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualNonBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLine`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineNonBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESM`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMNonBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActual`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualNonBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLine`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineNonBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESM`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMNonBalanced`, `HeatingAndCoolingResultCalc.CalculateLightsAndDevicesInputs`, `HeatingAndCoolingResultCalc.CalculateQint`, `HeatingAndCoolingResultCalc.CalculateQintBaseLine`, `HeatingAndCoolingResultCalc.CalculateQintESM`
- `HeatingAndCoolingResultCalc.CalcAvgNonProjectTemp` <- `HeatingAndCoolingResultCalc.CalculateParameterQtr`, `HeatingAndCoolingResultCalc.CalculateParameterQve`
- `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempBaseLine` <- `HeatingAndCoolingResultCalc.CalculateParameterQtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterQveBaseLIne`
- `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCooling` <- `HeatingAndCoolingResultCalc.CalculateCoolingQtr`, `HeatingAndCoolingResultCalc.CalculateQinf`
- `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingBaseLine` <- `HeatingAndCoolingResultCalc.CalculateCoolingQtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateQinfBaseLine`
- `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingESM` <- `HeatingAndCoolingResultCalc.CalculateCoolingQtrESM`, `HeatingAndCoolingResultCalc.CalculateQinfESM`
- `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingRef1` <- `HeatingAndCoolingResultCalc.CalculateCoolingQtrRef1`, `HeatingAndCoolingResultCalc.CalculateQinfRef1`
- `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingRef2` <- `HeatingAndCoolingResultCalc.CalculateCoolingQtrRef2`, `HeatingAndCoolingResultCalc.CalculateQinfRef2`
- `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempEsm` <- `HeatingAndCoolingResultCalc.CalculateParameterQtrEsm`, `HeatingAndCoolingResultCalc.CalculateParameterQveEsm`
- `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempRef1` <- `HeatingAndCoolingResultCalc.CalculateParameterQtrRef1`, `HeatingAndCoolingResultCalc.CalculateParameterQveRef1`
- `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempRef2` <- `HeatingAndCoolingResultCalc.CalculateParameterQtrRef2`, `HeatingAndCoolingResultCalc.CalculateParameterQveRef2`
- `HeatingAndCoolingResultCalc.CalcAvgProjectTemp` <- `HeatingAndCoolingResultCalc.CalculateParameterQtr`, `HeatingAndCoolingResultCalc.CalculateParameterQve`
- `HeatingAndCoolingResultCalc.CalcAvgProjectTempBaseLine` <- `HeatingAndCoolingResultCalc.CalculateParameterQtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterQveBaseLIne`
- `HeatingAndCoolingResultCalc.CalcAvgProjectTempCooling` <- `HeatingAndCoolingResultCalc.CalculateCoolingQtr`, `HeatingAndCoolingResultCalc.CalculateQinf`
- `HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingBaseLine` <- `HeatingAndCoolingResultCalc.CalculateCoolingQtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateQinfBaseLine`
- `HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingESM` <- `HeatingAndCoolingResultCalc.CalculateCoolingQtrESM`, `HeatingAndCoolingResultCalc.CalculateQinfESM`
- `HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingRef1` <- `HeatingAndCoolingResultCalc.CalculateCoolingQtrRef1`, `HeatingAndCoolingResultCalc.CalculateQinfRef1`
- `HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingRef2` <- `HeatingAndCoolingResultCalc.CalculateCoolingQtrRef2`, `HeatingAndCoolingResultCalc.CalculateQinfRef2`
- `HeatingAndCoolingResultCalc.CalcAvgProjectTempEsm` <- `HeatingAndCoolingResultCalc.CalculateParameterQtrEsm`, `HeatingAndCoolingResultCalc.CalculateParameterQveEsm`
- `HeatingAndCoolingResultCalc.CalcAvgProjectTempRef1` <- `HeatingAndCoolingResultCalc.CalculateParameterQtrRef1`, `HeatingAndCoolingResultCalc.CalculateParameterQveRef1`
- `HeatingAndCoolingResultCalc.CalcAvgProjectTempRef2` <- `HeatingAndCoolingResultCalc.CalculateParameterQtrRef2`, `HeatingAndCoolingResultCalc.CalculateParameterQveRef2`
- `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2` <- `HeatingAndCoolingResultCalc.CalculateParameterHtr`, `HeatingAndCoolingResultCalc.CalculateParameterHtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterHtrEsm`, `HeatingAndCoolingResultCalc.CalculateParameterHtrRef`, `HeatingAndCoolingResultCalc.CalculateParameterQtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterQtrEsm`, `HeatingAndCoolingResultCalc.CalculateaH`, `HeatingAndCoolingResultCalc.CalculateaHbaseLine`, `HeatingAndCoolingResultCalc.CalculateaHesm`
- `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2Cooling` <- `HeatingAndCoolingResultCalc.CalculateCoolingHtr`, `HeatingAndCoolingResultCalc.CalculateCoolingHtrESM`
- `HeatingAndCoolingResultCalc.CalcEntalpia` <- `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyActual`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyESM`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef1`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef2`
- `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3` <- `HeatingAndCoolingResultCalc.CalculateParameterHtr`, `HeatingAndCoolingResultCalc.CalculateParameterHtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterHtrEsm`, `HeatingAndCoolingResultCalc.CalculateParameterHtrRef`, `HeatingAndCoolingResultCalc.CalculateParameterQtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterQtrEsm`, `HeatingAndCoolingResultCalc.CalculateaH`, `HeatingAndCoolingResultCalc.CalculateaHbaseLine`, `HeatingAndCoolingResultCalc.CalculateaHesm`
- `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3Cooling` <- `HeatingAndCoolingResultCalc.CalculateCoolingHtr`, `HeatingAndCoolingResultCalc.CalculateCoolingHtrESM`
- `HeatingAndCoolingResultCalc.CalcParameterHve` <- `HeatingAndCoolingResultCalc.CalculateHeatingPower`, `HeatingAndCoolingResultCalc.CalculateParameterQve`, `HeatingAndCoolingResultCalc.CalculateaH`
- `HeatingAndCoolingResultCalc.CalcParameterHveBaseLine` <- `HeatingAndCoolingResultCalc.CalculateHeatingPower`, `HeatingAndCoolingResultCalc.CalculateParameterQveBaseLIne`, `HeatingAndCoolingResultCalc.CalculateaHbaseLine`
- `HeatingAndCoolingResultCalc.CalcParameterHveEsm` <- `HeatingAndCoolingResultCalc.CalculateHeatingPower`, `HeatingAndCoolingResultCalc.CalculateParameterQveEsm`, `HeatingAndCoolingResultCalc.CalculateaHesm`
- `HeatingAndCoolingResultCalc.CalcParameterHveRef1` <- `HeatingAndCoolingResultCalc.CalculateParameterQveRef1`, `HeatingAndCoolingResultCalc.CalculateaHref1`
- `HeatingAndCoolingResultCalc.CalcParameterHveRef2` <- `HeatingAndCoolingResultCalc.CalculateParameterQveRef2`, `HeatingAndCoolingResultCalc.CalculateaHref2`
- `HeatingAndCoolingResultCalc.CalcRo` <- `HeatingAndCoolingResultCalc.CalculateLatentHeatsInf`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfBaseLine`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfESM`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfRef1`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfRef2`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyActual`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyESM`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyRef1`
- `HeatingAndCoolingResultCalc.CalcRoW` <- `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVent`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentBaseLine`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentESM`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentRef1`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentRef2`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyRef1`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyRef2`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyActual`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyESM`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyRef1`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyRef2`
- `HeatingAndCoolingResultCalc.CalcTotalArea` <- `HeatingAndCoolingResultCalc.UpdateActualState`, `HeatingAndCoolingResultCalc.UpdateBaseLineState`, `HeatingAndCoolingResultCalc.UpdateEsmState`, `HeatingAndCoolingResultCalc.UpdateRefsState`
- `HeatingAndCoolingResultCalc.CalcWallDirectionParameterHu1` <- `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1Esm`
- `HeatingAndCoolingResultCalc.CalcWallDirectionParameterHu1Cooling` <- `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1Cooling`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1CoolingESM`
- `HeatingAndCoolingResultCalc.CalcWeekPower` <- `HeatingAndCoolingResultCalc.CalcAvgMonthPower`
- `HeatingAndCoolingResultCalc.CalculateAc` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyActual`
- `HeatingAndCoolingResultCalc.CalculateAcBaseLine` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine`
- `HeatingAndCoolingResultCalc.CalculateAcESM` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM`
- `HeatingAndCoolingResultCalc.CalculateAcRef1` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef1`
- `HeatingAndCoolingResultCalc.CalculateAcRef2` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef2`
- `HeatingAndCoolingResultCalc.CalculateActual` <- `HeatingAndCoolingResultCalc.Calculations`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActual` <- `HeatingAndCoolingResultCalc.CalculatePeriodsActual`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActualBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsActualBalanced`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActualHotWaterPumps` <- `HeatingAndCoolingResultCalc.CalculatePeriodsActualHotWaterPumps`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActualNonBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsActualNonBalanced`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLine` <- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLine`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLineBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLineBalanced`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLineHotWaterPumps` <- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLineHotWaterPumps`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLineNonBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLineNonBalanced`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESM` <- `HeatingAndCoolingResultCalc.CalculatePeriodsESM`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESMBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsESMBalanced`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESMHotWaterPumps` <- `HeatingAndCoolingResultCalc.CalculatePeriodsESMHotWaterPumps`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESMNonBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsESMNonBalanced`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReference`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1Balanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceBalanced`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1HotWaterPumps` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceHotWaterPumps`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1NonBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceNonBalanced`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReference`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2Balanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceBalanced`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2HotWaterPumps` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceHotWaterPumps`
- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2NonBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceNonBalanced`
- `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempBaseLine` <- `HeatingAndCoolingResultCalc.CalculateAcBaseLine`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrBaseLine`
- `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempCurrent` <- `HeatingAndCoolingResultCalc.CalculateAc`, `HeatingAndCoolingResultCalc.CalculateCoolingQtr`
- `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempESM` <- `HeatingAndCoolingResultCalc.CalculateAcESM`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrESM`
- `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempRef1` <- `HeatingAndCoolingResultCalc.CalculateAcRef1`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrRef1`
- `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempRef2` <- `HeatingAndCoolingResultCalc.CalculateAcRef2`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrRef2`
- `HeatingAndCoolingResultCalc.CalculateAverageHeatTempBaseLine` <- `HeatingAndCoolingResultCalc.CalculateParameterQtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateaHbaseLine`
- `HeatingAndCoolingResultCalc.CalculateAverageHeatTempCurrent` <- `HeatingAndCoolingResultCalc.CalculateParameterQtr`, `HeatingAndCoolingResultCalc.CalculateaH`
- `HeatingAndCoolingResultCalc.CalculateAverageHeatTempEsm` <- `HeatingAndCoolingResultCalc.CalculateParameterQtrEsm`, `HeatingAndCoolingResultCalc.CalculateaHesm`
- `HeatingAndCoolingResultCalc.CalculateAverageHeatTempRef1` <- `HeatingAndCoolingResultCalc.CalculateParameterQtrRef1`, `HeatingAndCoolingResultCalc.CalculateaHref1`
- `HeatingAndCoolingResultCalc.CalculateAverageHeatTempRef2` <- `HeatingAndCoolingResultCalc.CalculateParameterQtrRef2`, `HeatingAndCoolingResultCalc.CalculateaHref2`
- `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempActual` <- `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyActual`, `HeatingAndCoolingResultCalc.VentilationHeatEnergyActual`
- `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempBaseLine` <- `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempRef1`, `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempRef2`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyBaseLine`, `HeatingAndCoolingResultCalc.VentilationHeatEnergyBaseLine`
- `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempESM` <- `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyESM`, `HeatingAndCoolingResultCalc.VentilationHeatEnergyESM`
- `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempRef1` <- `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef1`
- `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempRef2` <- `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef2`
- `HeatingAndCoolingResultCalc.CalculateBalancedDevicesSavings` <- _None_
- `HeatingAndCoolingResultCalc.CalculateBaseLine` <- `HeatingAndCoolingResultCalc.Calculations`
- `HeatingAndCoolingResultCalc.CalculateBuildingHeatingPower` <- `HeatingAndCoolingResultCalc.CalculateBuildingPowerEnergy`
- `HeatingAndCoolingResultCalc.CalculateBuildingPowerEnergy` <- `HeatingAndCoolingResultCalc.BuildingCalculations`
- `HeatingAndCoolingResultCalc.CalculateBuildingSourcePower` <- `HeatingAndCoolingResultCalc.CalculateBuildingPowerEnergy`
- `HeatingAndCoolingResultCalc.CalculateCO2Emissions` <- `HeatingAndCoolingResultCalc.BuildingCO2Calculations`, `HeatingAndCoolingResultCalc.ZoneCO2Calculations`
- `HeatingAndCoolingResultCalc.CalculateCO2EmissionsActual` <- `HeatingAndCoolingResultCalc.CalculateCO2Emissions`
- `HeatingAndCoolingResultCalc.CalculateCO2EmissionsBaseLine` <- `HeatingAndCoolingResultCalc.CalculateCO2Emissions`
- `HeatingAndCoolingResultCalc.CalculateCO2EmissionsESM` <- `HeatingAndCoolingResultCalc.CalculateCO2Emissions`
- `HeatingAndCoolingResultCalc.CalculateCO2EmissionsRef1` <- `HeatingAndCoolingResultCalc.CalculateCO2Emissions`
- `HeatingAndCoolingResultCalc.CalculateCO2EmissionsRef2` <- `HeatingAndCoolingResultCalc.CalculateCO2Emissions`
- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyActual` <- `HeatingAndCoolingResultCalc.CoolingCalculations`
- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`, `HeatingAndCoolingResultCalc.CoolingCalculations`, `HeatingAndCoolingResultCalc.CreateCoolingVirtualBaseLine`
- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`, `HeatingAndCoolingResultCalc.CoolingCalculations`, `HeatingAndCoolingResultCalc.CreateCoolingVirtualESM`
- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef1` <- `HeatingAndCoolingResultCalc.CoolingCalculations`
- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef2` <- `HeatingAndCoolingResultCalc.CoolingCalculations`
- `HeatingAndCoolingResultCalc.CalculateCoolingHtr` <- `HeatingAndCoolingResultCalc.CalculateAc`, `HeatingAndCoolingResultCalc.CalculateAcBaseLine`, `HeatingAndCoolingResultCalc.CalculateAcRef1`, `HeatingAndCoolingResultCalc.CalculateAcRef2`, `HeatingAndCoolingResultCalc.CalculateCoolingQtr`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrRef1`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrRef2`
- `HeatingAndCoolingResultCalc.CalculateCoolingHtrESM` <- `HeatingAndCoolingResultCalc.CalculateAcESM`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrESM`
- `HeatingAndCoolingResultCalc.CalculateCoolingInputs` <- `HeatingAndCoolingResultCalc.VentilationCoolEnergyActual`
- `HeatingAndCoolingResultCalc.CalculateCoolingInputsBaseLine` <- `HeatingAndCoolingResultCalc.VentilationCoolEnergyBaseLine`
- `HeatingAndCoolingResultCalc.CalculateCoolingInputsESM` <- `HeatingAndCoolingResultCalc.VentilationCoolEnergyEsm`
- `HeatingAndCoolingResultCalc.CalculateCoolingInputsRef1` <- `HeatingAndCoolingResultCalc.VentilationCoolEnergyRef1`
- `HeatingAndCoolingResultCalc.CalculateCoolingInputsRef2` <- `HeatingAndCoolingResultCalc.VentilationCoolEnergyRef2`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActual` <- `HeatingAndCoolingResultCalc.CalculatePeriodsActual`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsActualBalanced`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualHotWaterPumps` <- `HeatingAndCoolingResultCalc.CalculatePeriodsActualHotWaterPumps`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualNonBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsActualNonBalanced`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLine` <- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLine`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLineBalanced`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineHotWaterPumps` <- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLineHotWaterPumps`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineNonBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLineNonBalanced`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESM` <- `HeatingAndCoolingResultCalc.CalculatePeriodsESM`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsESMBalanced`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMHotWaterPumps` <- `HeatingAndCoolingResultCalc.CalculatePeriodsESMHotWaterPumps`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMNonBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsESMNonBalanced`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReference`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1Balanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceBalanced`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1HotWaterPumps` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceHotWaterPumps`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1NonBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceNonBalanced`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReference`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2Balanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceBalanced`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2HotWaterPumps` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceHotWaterPumps`
- `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2NonBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceNonBalanced`
- `HeatingAndCoolingResultCalc.CalculateCoolingQtr` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyActual`
- `HeatingAndCoolingResultCalc.CalculateCoolingQtrBaseLine` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine`
- `HeatingAndCoolingResultCalc.CalculateCoolingQtrESM` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM`
- `HeatingAndCoolingResultCalc.CalculateCoolingQtrRef1` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef1`
- `HeatingAndCoolingResultCalc.CalculateCoolingQtrRef2` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef2`
- `HeatingAndCoolingResultCalc.CalculateCoolingSavings` <- _None_
- `HeatingAndCoolingResultCalc.CalculateETA` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyActual`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef1`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef2`
- `HeatingAndCoolingResultCalc.CalculateElectricityVEI` <- `HeatingAndCoolingResultCalc.GetVeiBGV`, `HeatingAndCoolingResultCalc.GetVeiHeatVentilation`, `HeatingAndCoolingResultCalc.GetVeiHeating`
- `HeatingAndCoolingResultCalc.CalculateEnergy` <- `HeatingAndCoolingResultCalc.CalculateHeatingSavings`, `HeatingAndCoolingResultCalc.CreateHeatingVirtualBaseLine`, `HeatingAndCoolingResultCalc.CreateHeatingVirtualESM`
- `HeatingAndCoolingResultCalc.CalculateEnergyESM` <- `HeatingAndCoolingResultCalc.CalculateHeatingSavings`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1` <- `HeatingAndCoolingResultCalc.CalculateZonePowerEnergy`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1Area` <- `HeatingAndCoolingResultCalc.CalculateZonePowerEnergy`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1BaseLine` <- `HeatingAndCoolingResultCalc.CalculateZonePowerEnergy`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1BaseLineArea` <- `HeatingAndCoolingResultCalc.CalculateZonePowerEnergy`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1Esm` <- `HeatingAndCoolingResultCalc.CalculateZonePowerEnergy`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1EsmArea` <- `HeatingAndCoolingResultCalc.CalculateZonePowerEnergy`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2` <- `HeatingAndCoolingResultCalc.CalculateZonePowerEnergy`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2Area` <- `HeatingAndCoolingResultCalc.CalculateZonePowerEnergy`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2BaseLine` <- `HeatingAndCoolingResultCalc.CalculateZonePowerEnergy`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2BaseLineArea` <- `HeatingAndCoolingResultCalc.CalculateZonePowerEnergy`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2Esm` <- `HeatingAndCoolingResultCalc.CalculateZonePowerEnergy`
- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2EsmArea` <- `HeatingAndCoolingResultCalc.CalculateZonePowerEnergy`
- `HeatingAndCoolingResultCalc.CalculateEntalpia` <- `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyActual`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyESM`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyRef1`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyRef2`
- `HeatingAndCoolingResultCalc.CalculateEsm` <- `HeatingAndCoolingResultCalc.Calculations`
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingActual` <- _None_
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingBaseLine` <- _None_
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingEsm` <- _None_
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingRef1` <- _None_
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingRef2` <- _None_
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingSavings` <- _None_
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingActual` <- _None_
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingBaseLine` <- _None_
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingEsm` <- _None_
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingRef1` <- _None_
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingRef2` <- _None_
- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingSavings` <- _None_
- `HeatingAndCoolingResultCalc.CalculateFuel2AreaValue` <- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2Area`
- `HeatingAndCoolingResultCalc.CalculateFuel2AreaValueBaseLine` <- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2BaseLineArea`
- `HeatingAndCoolingResultCalc.CalculateFuel2AreaValueEsm` <- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2EsmArea`
- `HeatingAndCoolingResultCalc.CalculateFuel2Value` <- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2`, `HeatingAndCoolingResultCalc.CalculateFuel2AreaValue`
- `HeatingAndCoolingResultCalc.CalculateFuel2ValueBaseLine` <- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2BaseLine`, `HeatingAndCoolingResultCalc.CalculateFuel2AreaValueBaseLine`
- `HeatingAndCoolingResultCalc.CalculateFuel2ValueEsm` <- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel2Esm`, `HeatingAndCoolingResultCalc.CalculateFuel2AreaValueEsm`
- `HeatingAndCoolingResultCalc.CalculateFuelActual` <- `HeatingAndCoolingResultCalc.SetFuelValue`
- `HeatingAndCoolingResultCalc.CalculateFuelAreaValue` <- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1Area`
- `HeatingAndCoolingResultCalc.CalculateFuelAreaValueBaseLine` <- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1BaseLineArea`
- `HeatingAndCoolingResultCalc.CalculateFuelAreaValueEsm` <- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1EsmArea`
- `HeatingAndCoolingResultCalc.CalculateFuelBaseLine` <- `HeatingAndCoolingResultCalc.SetFuelValue`
- `HeatingAndCoolingResultCalc.CalculateFuelESM` <- `HeatingAndCoolingResultCalc.SetFuelValue`
- `HeatingAndCoolingResultCalc.CalculateFuelRef1` <- `HeatingAndCoolingResultCalc.SetFuelValue`
- `HeatingAndCoolingResultCalc.CalculateFuelRef2` <- `HeatingAndCoolingResultCalc.SetFuelValue`
- `HeatingAndCoolingResultCalc.CalculateFuelSavings` <- `HeatingAndCoolingResultCalc.BuildingCO2Calculations`, `HeatingAndCoolingResultCalc.GetPrimaryFuelTypeAndValues`, `HeatingAndCoolingResultCalc.ZoneCO2Calculations`
- `HeatingAndCoolingResultCalc.CalculateFuelSavings` <- `HeatingAndCoolingResultCalc.BuildingCO2Calculations`, `HeatingAndCoolingResultCalc.GetPrimaryFuelTypeAndValues`, `HeatingAndCoolingResultCalc.ZoneCO2Calculations`
- `HeatingAndCoolingResultCalc.CalculateFuelValue` <- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1`, `HeatingAndCoolingResultCalc.CalculateFuelAreaValue`
- `HeatingAndCoolingResultCalc.CalculateFuelValueBaseLine` <- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1BaseLine`, `HeatingAndCoolingResultCalc.CalculateFuelAreaValueBaseLine`
- `HeatingAndCoolingResultCalc.CalculateFuelValueEsm` <- `HeatingAndCoolingResultCalc.CalculateEnergySourcePowerFuel1Esm`, `HeatingAndCoolingResultCalc.CalculateFuelAreaValueEsm`
- `HeatingAndCoolingResultCalc.CalculateGcurrent` <- _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyActual` <- _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyBaseLine` <- _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyESM` <- _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyRef1` <- _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyRef2` <- _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyActual` <- _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyBaseLine` <- `HeatingAndCoolingResultCalc.CalculateEnergy`, `HeatingAndCoolingResultCalc.CalculateVentilationHeatingSavings`
- `HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyEsm` <- `HeatingAndCoolingResultCalc.CalculateEnergyESM`
- `HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyRef1` <- _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyRef2` <- _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorHotWaterEfficiencyActual` <- _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorHotWaterEfficiencyBaseLine` <- `HeatingAndCoolingResultCalc.CalculateHotWaterSavings`
- `HeatingAndCoolingResultCalc.CalculateGeneratorHotWaterEfficiencyEsm` <- _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorHotWaterEfficiencyRef1` <- _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorHotWaterEfficiencyRef2` <- _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyActual` <- _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyBaseLine` <- `HeatingAndCoolingResultCalc.CalculateVentilationCoolingSavings`
- `HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyESM` <- _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyRef1` <- _None_
- `HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyRef2` <- _None_
- `HeatingAndCoolingResultCalc.CalculateGesm` <- _None_
- `HeatingAndCoolingResultCalc.CalculateGsavings` <- `HeatingAndCoolingResultCalc.CalculateUsavingType`
- `HeatingAndCoolingResultCalc.CalculateGsavingsESM` <- `HeatingAndCoolingResultCalc.CalculateUsavingTypeESM`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActual` <- `HeatingAndCoolingResultCalc.CalculatePeriodsActual`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsActualBalanced`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualHotWaterPumps` <- `HeatingAndCoolingResultCalc.CalculatePeriodsActualHotWaterPumps`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualNonBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsActualNonBalanced`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLine` <- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLine`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLineBalanced`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineHotWaterPumps` <- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLineHotWaterPumps`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineNonBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLineNonBalanced`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESM` <- `HeatingAndCoolingResultCalc.CalculatePeriodsESM`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsESMBalanced`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMHotWaterPumps` <- `HeatingAndCoolingResultCalc.CalculatePeriodsESMHotWaterPumps`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMNonBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsESMNonBalanced`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReference`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1Balanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceBalanced`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1HotWaterPumps` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceHotWaterPumps`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1NonBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceNonBalanced`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReference`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2Balanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceBalanced`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2HotWaterPumps` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceHotWaterPumps`
- `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2NonBalanced` <- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceNonBalanced`
- `HeatingAndCoolingResultCalc.CalculateHeatingPower` <- `HeatingAndCoolingResultCalc.CalculateZonePowerEnergy`
- `HeatingAndCoolingResultCalc.CalculateHeatingSavings` <- _None_
- `HeatingAndCoolingResultCalc.CalculateHinf` <- `HeatingAndCoolingResultCalc.CalculateAc`, `HeatingAndCoolingResultCalc.CalculateQinf`
- `HeatingAndCoolingResultCalc.CalculateHinfBaseLine` <- `HeatingAndCoolingResultCalc.CalculateAcBaseLine`, `HeatingAndCoolingResultCalc.CalculateQinfBaseLine`
- `HeatingAndCoolingResultCalc.CalculateHinfESM` <- `HeatingAndCoolingResultCalc.CalculateAcESM`, `HeatingAndCoolingResultCalc.CalculateQinfESM`
- `HeatingAndCoolingResultCalc.CalculateHinfRef1` <- `HeatingAndCoolingResultCalc.CalculateAcRef1`, `HeatingAndCoolingResultCalc.CalculateQinfRef1`
- `HeatingAndCoolingResultCalc.CalculateHinfRef2` <- `HeatingAndCoolingResultCalc.CalculateAcRef2`, `HeatingAndCoolingResultCalc.CalculateQinfRef2`
- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededEnergyActual` <- _None_
- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededEnergyBaseLine` <- `HeatingAndCoolingResultCalc.CalculateHotWaterSavings`
- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededEnergyEsm` <- _None_
- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededEnergyRef1` <- _None_
- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededEnergyRef2` <- _None_
- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededPower` <- _None_
- `HeatingAndCoolingResultCalc.CalculateHotWaterPumpsSavings` <- _None_
- `HeatingAndCoolingResultCalc.CalculateHotWaterSavings` <- _None_
- `HeatingAndCoolingResultCalc.CalculateHve` <- `HeatingAndCoolingResultCalc.CalculateQve`
- `HeatingAndCoolingResultCalc.CalculateHveBaseLine` <- `HeatingAndCoolingResultCalc.CalculateQveBaseLine`
- `HeatingAndCoolingResultCalc.CalculateHveESM` <- `HeatingAndCoolingResultCalc.CalculateQveESM`
- `HeatingAndCoolingResultCalc.CalculateHveRef1` <- `HeatingAndCoolingResultCalc.CalculateQveRef1`
- `HeatingAndCoolingResultCalc.CalculateHveRef2` <- `HeatingAndCoolingResultCalc.CalculateQveRef2`
- `HeatingAndCoolingResultCalc.CalculateItemsWalls` <- `HeatingAndCoolingResultCalc.SumAllDirectionsWallsCurrent`, `HeatingAndCoolingResultCalc.SumAllDirectionsWallsEsm`
- `HeatingAndCoolingResultCalc.CalculateLatentHeatsInf` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyActual`
- `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfBaseLine` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine`
- `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfESM` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM`
- `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfRef1` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef1`
- `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfRef2` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef2`
- `HeatingAndCoolingResultCalc.CalculateLatentHeatsVent` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyActual`
- `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentBaseLine` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine`
- `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentESM` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM`
- `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentRef1` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef1`
- `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentRef2` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef2`
- `HeatingAndCoolingResultCalc.CalculateLightsAndDevicesInputs` <- `HeatingAndCoolingResultCalc.Calculations`
- `HeatingAndCoolingResultCalc.CalculateLightsSavings` <- _None_
- `HeatingAndCoolingResultCalc.CalculateMonthlyHorizontalRadiation` <- `HeatingAndCoolingResultCalc.CalculateProjectionCoeficient`
- `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyActual` <- `HeatingAndCoolingResultCalc.VentilationCoolEnergyActual`
- `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyBaseLine` <- `HeatingAndCoolingResultCalc.VentilationCoolEnergyBaseLine`
- `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyESM` <- `HeatingAndCoolingResultCalc.VentilationCoolEnergyEsm`
- `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyRef1` <- `HeatingAndCoolingResultCalc.VentilationCoolEnergyRef1`
- `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyRef2` <- `HeatingAndCoolingResultCalc.VentilationCoolEnergyRef2`
- `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyActual` <- `HeatingAndCoolingResultCalc.VentilationHeatEnergyActual`
- `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyBaseLine` <- `HeatingAndCoolingResultCalc.VentilationHeatEnergyBaseLine`
- `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyESM` <- `HeatingAndCoolingResultCalc.VentilationHeatEnergyESM`
- `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef1` <- `HeatingAndCoolingResultCalc.VentilationHeatEnergyRef1`
- `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef2` <- `HeatingAndCoolingResultCalc.VentilationHeatEnergyRef2`
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyActual` <- _None_
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyBaseLine` <- `HeatingAndCoolingResultCalc.CalculateEnergy`
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingActual` <- _None_
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingBaseLine` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingESM` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingRef1` <- _None_
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingRef2` <- _None_
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyEsm` <- `HeatingAndCoolingResultCalc.CalculateEnergyESM`
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyRef1` <- _None_
- `HeatingAndCoolingResultCalc.CalculateNeededEnergyRef2` <- _None_
- `HeatingAndCoolingResultCalc.CalculateNetEnergy` <- `HeatingAndCoolingResultCalc.CalculateEnergy`, `HeatingAndCoolingResultCalc.CalculateEnergyESM`
- `HeatingAndCoolingResultCalc.CalculateNetEnergyByTechnologies` <- `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.CalculateNetEnergyByTechnologiesBuilding` <- `HeatingAndCoolingResultCalc.BuildingCalculations`
- `HeatingAndCoolingResultCalc.CalculateNetEnergyPerArea` <- `HeatingAndCoolingResultCalc.BuildingCalculations`
- `HeatingAndCoolingResultCalc.CalculateNetWithoutInputsEnergyByTechnologies` <- `HeatingAndCoolingResultCalc.BuildingCalculations`, `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.CalculateNetWithoutInputsEnergyByTechnologiesPerArea` <- `HeatingAndCoolingResultCalc.BuildingCalculations`
- `HeatingAndCoolingResultCalc.CalculateNonBalancedDevicesSavings` <- _None_
- `HeatingAndCoolingResultCalc.CalculateNonTransparentFsol` <- `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsol`, `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsolEsm`
- `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsol` <- `HeatingAndCoolingResultCalc.CalculateParameterQgn`, `HeatingAndCoolingResultCalc.CalculateParameterQgnBaseLine`, `HeatingAndCoolingResultCalc.CalculateQsol`, `HeatingAndCoolingResultCalc.CalculateQsolBaseLine`, `HeatingAndCoolingResultCalc.CalculateQsolRef1`, `HeatingAndCoolingResultCalc.CalculateQsolRef2`
- `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsolEsm` <- `HeatingAndCoolingResultCalc.CalculateParameterQgnEsm`, `HeatingAndCoolingResultCalc.CalculateQsolESM`
- `HeatingAndCoolingResultCalc.CalculateOccupantshours` <- `HeatingAndCoolingResultCalc.CalculateQLatentOccupants`, `HeatingAndCoolingResultCalc.CalculateQLatentOccupantsRef1`, `HeatingAndCoolingResultCalc.CalculateQLatentOccupantsRef2`, `HeatingAndCoolingResultCalc.CalculateQoccupants`
- `HeatingAndCoolingResultCalc.CalculateOccupantshoursBaseLine` <- `HeatingAndCoolingResultCalc.CalculateQLatentOccupantsBaseLine`, `HeatingAndCoolingResultCalc.CalculateQoccupantsBaseLine`
- `HeatingAndCoolingResultCalc.CalculateOccupantshoursESM` <- `HeatingAndCoolingResultCalc.CalculateQLatentOccupantsESM`, `HeatingAndCoolingResultCalc.CalculateQoccupantsESM`
- `HeatingAndCoolingResultCalc.CalculateParameterF` <- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededPower`
- `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent` <- `HeatingAndCoolingResultCalc.CalculateCoolingHtr`, `HeatingAndCoolingResultCalc.CalculateParameterHtr`, `HeatingAndCoolingResultCalc.CalculateParameterHtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterHtrRef`, `HeatingAndCoolingResultCalc.CalculateParameterQtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateaH`, `HeatingAndCoolingResultCalc.CalculateaHbaseLine`
- `HeatingAndCoolingResultCalc.CalculateParameterHdEsm` <- `HeatingAndCoolingResultCalc.CalculateCoolingHtrESM`, `HeatingAndCoolingResultCalc.CalculateParameterHtrEsm`, `HeatingAndCoolingResultCalc.CalculateParameterQtrEsm`, `HeatingAndCoolingResultCalc.CalculateaHesm`
- `HeatingAndCoolingResultCalc.CalculateParameterHgCurrent` <- `HeatingAndCoolingResultCalc.CalculateCoolingHtr`, `HeatingAndCoolingResultCalc.CalculateParameterHtr`, `HeatingAndCoolingResultCalc.CalculateParameterHtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterHtrRef`, `HeatingAndCoolingResultCalc.CalculateParameterQtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateaH`, `HeatingAndCoolingResultCalc.CalculateaHbaseLine`
- `HeatingAndCoolingResultCalc.CalculateParameterHgEsm` <- `HeatingAndCoolingResultCalc.CalculateCoolingHtrESM`, `HeatingAndCoolingResultCalc.CalculateParameterHtrEsm`, `HeatingAndCoolingResultCalc.CalculateParameterQtrEsm`, `HeatingAndCoolingResultCalc.CalculateaHesm`
- `HeatingAndCoolingResultCalc.CalculateParameterHtMonthly` <- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededPower`, `HeatingAndCoolingResultCalc.CalculateParameterY`
- `HeatingAndCoolingResultCalc.CalculateParameterHtr` <- `HeatingAndCoolingResultCalc.CalculateHeatingPower`, `HeatingAndCoolingResultCalc.CalculateParameterQtr`, `HeatingAndCoolingResultCalc.CalculateParameterQtrRef1`, `HeatingAndCoolingResultCalc.CalculateParameterQtrRef2`, `HeatingAndCoolingResultCalc.CalculateaHref1`
- `HeatingAndCoolingResultCalc.CalculateParameterHtrBaseLine` <- `HeatingAndCoolingResultCalc.CalculateHeatingPower`
- `HeatingAndCoolingResultCalc.CalculateParameterHtrEsm` <- `HeatingAndCoolingResultCalc.CalculateHeatingPower`
- `HeatingAndCoolingResultCalc.CalculateParameterHtrRef` <- `HeatingAndCoolingResultCalc.CalculateaHref2`
- `HeatingAndCoolingResultCalc.CalculateParameterNiEsm` <- `HeatingAndCoolingResultCalc.CalculateEsm`
- `HeatingAndCoolingResultCalc.CalculateParameterNign` <- `HeatingAndCoolingResultCalc.CalculateActual`
- `HeatingAndCoolingResultCalc.CalculateParameterNignBaseLine` <- `HeatingAndCoolingResultCalc.CalculateBaseLine`
- `HeatingAndCoolingResultCalc.CalculateParameterNignRef1` <- `HeatingAndCoolingResultCalc.CalculateRef1`
- `HeatingAndCoolingResultCalc.CalculateParameterNignRef2` <- `HeatingAndCoolingResultCalc.CalculateRef2`
- `HeatingAndCoolingResultCalc.CalculateParameterQgn` <- `HeatingAndCoolingResultCalc.CalculateActual`
- `HeatingAndCoolingResultCalc.CalculateParameterQgnBaseLine` <- `HeatingAndCoolingResultCalc.CalculateBaseLine`, `HeatingAndCoolingResultCalc.CalculateRef1`, `HeatingAndCoolingResultCalc.CalculateRef2`
- `HeatingAndCoolingResultCalc.CalculateParameterQgnEsm` <- `HeatingAndCoolingResultCalc.CalculateEsm`
- `HeatingAndCoolingResultCalc.CalculateParameterQtr` <- `HeatingAndCoolingResultCalc.CalculateActual`
- `HeatingAndCoolingResultCalc.CalculateParameterQtrBaseLine` <- `HeatingAndCoolingResultCalc.CalculateBaseLine`
- `HeatingAndCoolingResultCalc.CalculateParameterQtrEsm` <- `HeatingAndCoolingResultCalc.CalculateEsm`
- `HeatingAndCoolingResultCalc.CalculateParameterQtrRef1` <- `HeatingAndCoolingResultCalc.CalculateRef1`
- `HeatingAndCoolingResultCalc.CalculateParameterQtrRef2` <- `HeatingAndCoolingResultCalc.CalculateRef2`
- `HeatingAndCoolingResultCalc.CalculateParameterQve` <- `HeatingAndCoolingResultCalc.CalculateActual`
- `HeatingAndCoolingResultCalc.CalculateParameterQveBaseLIne` <- `HeatingAndCoolingResultCalc.CalculateBaseLine`
- `HeatingAndCoolingResultCalc.CalculateParameterQveEsm` <- `HeatingAndCoolingResultCalc.CalculateEsm`
- `HeatingAndCoolingResultCalc.CalculateParameterQveRef1` <- `HeatingAndCoolingResultCalc.CalculateRef1`
- `HeatingAndCoolingResultCalc.CalculateParameterQveRef2` <- `HeatingAndCoolingResultCalc.CalculateRef2`
- `HeatingAndCoolingResultCalc.CalculateParameterX` <- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededPower`
- `HeatingAndCoolingResultCalc.CalculateParameterY` <- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededPower`
- `HeatingAndCoolingResultCalc.CalculatePeriod` <- `HeatingAndCoolingResultCalc.CalculateBalancedDevicesSavings`, `HeatingAndCoolingResultCalc.CalculateHotWaterPumpsSavings`, `HeatingAndCoolingResultCalc.CalculateLightsSavings`, `HeatingAndCoolingResultCalc.CalculateNonBalancedDevicesSavings`
- `HeatingAndCoolingResultCalc.CalculatePeriodsActual` <- _None_
- `HeatingAndCoolingResultCalc.CalculatePeriodsActualBalanced` <- _None_
- `HeatingAndCoolingResultCalc.CalculatePeriodsActualHotWaterPumps` <- _None_
- `HeatingAndCoolingResultCalc.CalculatePeriodsActualNonBalanced` <- _None_
- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLine` <- _None_
- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLineBalanced` <- _None_
- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLineHotWaterPumps` <- _None_
- `HeatingAndCoolingResultCalc.CalculatePeriodsBaseLineNonBalanced` <- _None_
- `HeatingAndCoolingResultCalc.CalculatePeriodsESM` <- _None_
- `HeatingAndCoolingResultCalc.CalculatePeriodsESMBalanced` <- _None_
- `HeatingAndCoolingResultCalc.CalculatePeriodsESMHotWaterPumps` <- _None_
- `HeatingAndCoolingResultCalc.CalculatePeriodsESMNonBalanced` <- _None_
- `HeatingAndCoolingResultCalc.CalculatePeriodsReference` <- _None_
- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceBalanced` <- _None_
- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceHotWaterPumps` <- _None_
- `HeatingAndCoolingResultCalc.CalculatePeriodsReferenceNonBalanced` <- _None_
- `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyByTechnologies` <- `HeatingAndCoolingResultCalc.BuildingCalculations`, `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyFuelTotal` <- `HeatingAndCoolingResultCalc.BuildingCalculations`, `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyPerArea` <- `HeatingAndCoolingResultCalc.BuildingCalculations`, `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.CalculatePrimaryFuelTypeAndValuesPerArea` <- `HeatingAndCoolingResultCalc.BuildingCalculations`, `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.CalculatePrimaryTotalEnergy` <- `HeatingAndCoolingResultCalc.BuildingCalculations`, `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.CalculateProjectionCoeficient` <- `HeatingAndCoolingResultCalc.CalculateParameterHtMonthly`
- `HeatingAndCoolingResultCalc.CalculateQLatentOccupants` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyActual`
- `HeatingAndCoolingResultCalc.CalculateQLatentOccupantsBaseLine` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine`
- `HeatingAndCoolingResultCalc.CalculateQLatentOccupantsESM` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM`
- `HeatingAndCoolingResultCalc.CalculateQLatentOccupantsRef1` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef1`
- `HeatingAndCoolingResultCalc.CalculateQLatentOccupantsRef2` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef2`
- `HeatingAndCoolingResultCalc.CalculateQgain` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyActual`
- `HeatingAndCoolingResultCalc.CalculateQgainBaseLine` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine`
- `HeatingAndCoolingResultCalc.CalculateQgainESM` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM`
- `HeatingAndCoolingResultCalc.CalculateQgainRef1` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef1`
- `HeatingAndCoolingResultCalc.CalculateQgainRef2` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef2`
- `HeatingAndCoolingResultCalc.CalculateQinf` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyActual`
- `HeatingAndCoolingResultCalc.CalculateQinfBaseLine` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine`
- `HeatingAndCoolingResultCalc.CalculateQinfESM` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM`
- `HeatingAndCoolingResultCalc.CalculateQinfRef1` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef1`
- `HeatingAndCoolingResultCalc.CalculateQinfRef2` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef2`
- `HeatingAndCoolingResultCalc.CalculateQint` <- `HeatingAndCoolingResultCalc.CalculateQgain`
- `HeatingAndCoolingResultCalc.CalculateQintBaseLine` <- `HeatingAndCoolingResultCalc.CalculateQgainBaseLine`
- `HeatingAndCoolingResultCalc.CalculateQintESM` <- `HeatingAndCoolingResultCalc.CalculateQgainESM`
- `HeatingAndCoolingResultCalc.CalculateQintRef1` <- `HeatingAndCoolingResultCalc.CalculateQgainRef1`
- `HeatingAndCoolingResultCalc.CalculateQintRef2` <- `HeatingAndCoolingResultCalc.CalculateQgainRef2`
- `HeatingAndCoolingResultCalc.CalculateQoccupants` <- `HeatingAndCoolingResultCalc.CalculateQgain`
- `HeatingAndCoolingResultCalc.CalculateQoccupantsBaseLine` <- `HeatingAndCoolingResultCalc.CalculateQgainBaseLine`, `HeatingAndCoolingResultCalc.CalculateQgainRef1`, `HeatingAndCoolingResultCalc.CalculateQgainRef2`
- `HeatingAndCoolingResultCalc.CalculateQoccupantsESM` <- `HeatingAndCoolingResultCalc.CalculateQgainESM`
- `HeatingAndCoolingResultCalc.CalculateQsol` <- `HeatingAndCoolingResultCalc.CalculateQgain`
- `HeatingAndCoolingResultCalc.CalculateQsolBaseLine` <- `HeatingAndCoolingResultCalc.CalculateQgainBaseLine`
- `HeatingAndCoolingResultCalc.CalculateQsolESM` <- `HeatingAndCoolingResultCalc.CalculateQgainESM`
- `HeatingAndCoolingResultCalc.CalculateQsolRef1` <- `HeatingAndCoolingResultCalc.CalculateQgainRef1`
- `HeatingAndCoolingResultCalc.CalculateQsolRef2` <- `HeatingAndCoolingResultCalc.CalculateQgainRef2`
- `HeatingAndCoolingResultCalc.CalculateQve` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyActual`
- `HeatingAndCoolingResultCalc.CalculateQveBaseLine` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine`
- `HeatingAndCoolingResultCalc.CalculateQveESM` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM`
- `HeatingAndCoolingResultCalc.CalculateQveRef1` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef1`
- `HeatingAndCoolingResultCalc.CalculateQveRef2` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef2`
- `HeatingAndCoolingResultCalc.CalculateRef1` <- `HeatingAndCoolingResultCalc.CalculateRef1`, `HeatingAndCoolingResultCalc.Calculations`
- `HeatingAndCoolingResultCalc.CalculateRef1` <- `HeatingAndCoolingResultCalc.CalculateRef1`, `HeatingAndCoolingResultCalc.Calculations`
- `HeatingAndCoolingResultCalc.CalculateRef2` <- `HeatingAndCoolingResultCalc.CalculateRef2`, `HeatingAndCoolingResultCalc.Calculations`
- `HeatingAndCoolingResultCalc.CalculateRef2` <- `HeatingAndCoolingResultCalc.CalculateRef2`, `HeatingAndCoolingResultCalc.Calculations`
- `HeatingAndCoolingResultCalc.CalculateSavings` <- `HeatingAndCoolingResultCalc.BuildingCO2Calculations`, `HeatingAndCoolingResultCalc.ZoneCO2Calculations`
- `HeatingAndCoolingResultCalc.CalculateTOAeffect` <- `HeatingAndCoolingResultCalc.CalculateParameterX`, `HeatingAndCoolingResultCalc.CalculateParameterY`
- `HeatingAndCoolingResultCalc.CalculateTotalActual` <- `HeatingAndCoolingResultCalc.CalculateTotalsNeededEnergyTable`
- `HeatingAndCoolingResultCalc.CalculateTotalActualYearly` <- `HeatingAndCoolingResultCalc.CalculateTotalsNeededEnergyTable`
- `HeatingAndCoolingResultCalc.CalculateTotalBaseLine` <- `HeatingAndCoolingResultCalc.CalculateTotalsNeededEnergyTable`
- `HeatingAndCoolingResultCalc.CalculateTotalBaseLineYearly` <- `HeatingAndCoolingResultCalc.CalculateTotalsNeededEnergyTable`
- `HeatingAndCoolingResultCalc.CalculateTotalCO2Actual` <- `HeatingAndCoolingResultCalc.CalculateCO2EmissionsActual`
- `HeatingAndCoolingResultCalc.CalculateTotalCO2BaseLine` <- `HeatingAndCoolingResultCalc.CalculateCO2EmissionsBaseLine`
- `HeatingAndCoolingResultCalc.CalculateTotalCO2ESM` <- `HeatingAndCoolingResultCalc.CalculateCO2EmissionsESM`
- `HeatingAndCoolingResultCalc.CalculateTotalCO2Ref1` <- `HeatingAndCoolingResultCalc.CalculateCO2EmissionsRef1`
- `HeatingAndCoolingResultCalc.CalculateTotalCO2Ref2` <- `HeatingAndCoolingResultCalc.CalculateCO2EmissionsRef2`
- `HeatingAndCoolingResultCalc.CalculateTotalEsm` <- `HeatingAndCoolingResultCalc.CalculateTotalsNeededEnergyTable`
- `HeatingAndCoolingResultCalc.CalculateTotalEsmYearly` <- `HeatingAndCoolingResultCalc.CalculateTotalsNeededEnergyTable`
- `HeatingAndCoolingResultCalc.CalculateTotalFuelActual` <- `HeatingAndCoolingResultCalc.CalculateTotalFuelEnergy`
- `HeatingAndCoolingResultCalc.CalculateTotalFuelBaseLine` <- `HeatingAndCoolingResultCalc.CalculateTotalFuelEnergy`
- `HeatingAndCoolingResultCalc.CalculateTotalFuelESM` <- `HeatingAndCoolingResultCalc.CalculateTotalFuelEnergy`
- `HeatingAndCoolingResultCalc.CalculateTotalFuelEnergy` <- `HeatingAndCoolingResultCalc.BuildingCalculations`, `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.CalculateTotalFuelRef1` <- `HeatingAndCoolingResultCalc.CalculateTotalFuelEnergy`
- `HeatingAndCoolingResultCalc.CalculateTotalFuelRef2` <- `HeatingAndCoolingResultCalc.CalculateTotalFuelEnergy`
- `HeatingAndCoolingResultCalc.CalculateTotalPrimaryActual` <- `HeatingAndCoolingResultCalc.CalculatePrimaryTotalEnergy`
- `HeatingAndCoolingResultCalc.CalculateTotalPrimaryBaseLine` <- `HeatingAndCoolingResultCalc.CalculatePrimaryTotalEnergy`
- `HeatingAndCoolingResultCalc.CalculateTotalPrimaryEsm` <- `HeatingAndCoolingResultCalc.CalculatePrimaryTotalEnergy`
- `HeatingAndCoolingResultCalc.CalculateTotalPrimaryFuelActual` <- `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyFuelTotal`
- `HeatingAndCoolingResultCalc.CalculateTotalPrimaryFuelBaseLine` <- `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyFuelTotal`
- `HeatingAndCoolingResultCalc.CalculateTotalPrimaryFuelESM` <- `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyFuelTotal`
- `HeatingAndCoolingResultCalc.CalculateTotalPrimaryFuelRef1` <- `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyFuelTotal`
- `HeatingAndCoolingResultCalc.CalculateTotalPrimaryFuelRef2` <- `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyFuelTotal`
- `HeatingAndCoolingResultCalc.CalculateTotalPrimaryFuelSavings` <- `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyFuelTotal`
- `HeatingAndCoolingResultCalc.CalculateTotalPrimaryRef1` <- `HeatingAndCoolingResultCalc.CalculatePrimaryTotalEnergy`
- `HeatingAndCoolingResultCalc.CalculateTotalPrimaryRef2` <- `HeatingAndCoolingResultCalc.CalculatePrimaryTotalEnergy`
- `HeatingAndCoolingResultCalc.CalculateTotalRefs` <- `HeatingAndCoolingResultCalc.CalculateTotalsNeededEnergyTable`
- `HeatingAndCoolingResultCalc.CalculateTotalRefsYearly` <- `HeatingAndCoolingResultCalc.CalculateTotalsNeededEnergyTable`
- `HeatingAndCoolingResultCalc.CalculateTotalVei` <- `HeatingAndCoolingResultCalc.BuildingCalculations`, `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.CalculateTotalsNeededEnergyTable` <- `HeatingAndCoolingResultCalc.BuildingCalculations`, `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.CalculateTransparentFsol` <- `HeatingAndCoolingResultCalc.CalculateTrasparentFsol`, `HeatingAndCoolingResultCalc.CalculateTrasparentFsolEsm`
- `HeatingAndCoolingResultCalc.CalculateTrasparentFsol` <- `HeatingAndCoolingResultCalc.CalculateParameterQgn`, `HeatingAndCoolingResultCalc.CalculateParameterQgnBaseLine`, `HeatingAndCoolingResultCalc.CalculateQsol`, `HeatingAndCoolingResultCalc.CalculateQsolBaseLine`, `HeatingAndCoolingResultCalc.CalculateQsolRef1`, `HeatingAndCoolingResultCalc.CalculateQsolRef2`
- `HeatingAndCoolingResultCalc.CalculateTrasparentFsolEsm` <- `HeatingAndCoolingResultCalc.CalculateParameterQgnEsm`, `HeatingAndCoolingResultCalc.CalculateQsolESM`
- `HeatingAndCoolingResultCalc.CalculateUInnerWallsSaving` <- `HeatingAndCoolingResultCalc.CalculateUsavingType`
- `HeatingAndCoolingResultCalc.CalculateUInnerWallsSavingESM` <- `HeatingAndCoolingResultCalc.CalculateUsavingTypeESM`
- `HeatingAndCoolingResultCalc.CalculateUOuterWallsSaving` <- `HeatingAndCoolingResultCalc.CalculateUsavingType`
- `HeatingAndCoolingResultCalc.CalculateUOuterWallsSavingESM` <- `HeatingAndCoolingResultCalc.CalculateUsavingTypeESM`
- `HeatingAndCoolingResultCalc.CalculateUceilingsavings` <- `HeatingAndCoolingResultCalc.CalculateUsavingType`
- `HeatingAndCoolingResultCalc.CalculateUceilingsavingsESM` <- `HeatingAndCoolingResultCalc.CalculateUsavingTypeESM`
- `HeatingAndCoolingResultCalc.CalculateUfloorOthersavings` <- `HeatingAndCoolingResultCalc.CalculateUsavingType`
- `HeatingAndCoolingResultCalc.CalculateUfloorOthersavingsESM` <- `HeatingAndCoolingResultCalc.CalculateUsavingTypeESM`
- `HeatingAndCoolingResultCalc.CalculateUfloorSavings` <- `HeatingAndCoolingResultCalc.CalculateUsavingType`
- `HeatingAndCoolingResultCalc.CalculateUfloorSavingsESM` <- `HeatingAndCoolingResultCalc.CalculateUsavingTypeESM`
- `HeatingAndCoolingResultCalc.CalculateUinnerWallsCurrent` <- _None_
- `HeatingAndCoolingResultCalc.CalculateUinnerWallsEsm` <- _None_
- `HeatingAndCoolingResultCalc.CalculateUnonTransparentSavings` <- `HeatingAndCoolingResultCalc.CalculateUsavingType`
- `HeatingAndCoolingResultCalc.CalculateUnonTransparentSavingsESM` <- `HeatingAndCoolingResultCalc.CalculateUsavingTypeESM`
- `HeatingAndCoolingResultCalc.CalculateUouterWallsCurrent` <- _None_
- `HeatingAndCoolingResultCalc.CalculateUouterWallsEsm` <- _None_
- `HeatingAndCoolingResultCalc.CalculateUsavingType` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateHeatingSavings`
- `HeatingAndCoolingResultCalc.CalculateUsavingTypeESM` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateHeatingSavings`
- `HeatingAndCoolingResultCalc.CalculateUwindowsCurrent` <- _None_
- `HeatingAndCoolingResultCalc.CalculateUwindowsEsm` <- _None_
- `HeatingAndCoolingResultCalc.CalculateUwindowsSavings` <- `HeatingAndCoolingResultCalc.CalculateUsavingType`
- `HeatingAndCoolingResultCalc.CalculateUwindowsSavingsESM` <- `HeatingAndCoolingResultCalc.CalculateUsavingTypeESM`
- `HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyActual` <- _None_
- `HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyBaseLine` <- `HeatingAndCoolingResultCalc.CalculateVentilationCoolingSavings`
- `HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyEsm` <- _None_
- `HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyRef1` <- _None_
- `HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyRef2` <- _None_
- `HeatingAndCoolingResultCalc.CalculateVentNeededEnergyActual` <- _None_
- `HeatingAndCoolingResultCalc.CalculateVentNeededEnergyBaseLine` <- `HeatingAndCoolingResultCalc.CalculateVentilationHeatingSavings`
- `HeatingAndCoolingResultCalc.CalculateVentNeededEnergyEsm` <- _None_
- `HeatingAndCoolingResultCalc.CalculateVentNeededEnergyRef1` <- _None_
- `HeatingAndCoolingResultCalc.CalculateVentNeededEnergyRef2` <- _None_
- `HeatingAndCoolingResultCalc.CalculateVentilationCoolingSavings` <- _None_
- `HeatingAndCoolingResultCalc.CalculateVentilationHeatingSavings` <- _None_
- `HeatingAndCoolingResultCalc.CalculateWitheringEnergyActual` <- `HeatingAndCoolingResultCalc.VentilationCoolEnergyActual`
- `HeatingAndCoolingResultCalc.CalculateWitheringEnergyBaseLine` <- `HeatingAndCoolingResultCalc.VentilationCoolEnergyBaseLine`
- `HeatingAndCoolingResultCalc.CalculateWitheringEnergyESM` <- `HeatingAndCoolingResultCalc.VentilationCoolEnergyEsm`
- `HeatingAndCoolingResultCalc.CalculateWitheringEnergyRef1` <- `HeatingAndCoolingResultCalc.VentilationCoolEnergyRef1`
- `HeatingAndCoolingResultCalc.CalculateWitheringEnergyRef2` <- `HeatingAndCoolingResultCalc.VentilationCoolEnergyRef2`
- `HeatingAndCoolingResultCalc.CalculateWitheringEntalpia` <- `HeatingAndCoolingResultCalc.CalculateWitheringEnergyActual`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyESM`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyRef1`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyRef2`
- `HeatingAndCoolingResultCalc.CalculateXwithCorrection` <- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededPower`
- `HeatingAndCoolingResultCalc.CalculateZonePowerEnergy` <- `HeatingAndCoolingResultCalc.CalculateBuildingPowerEnergy`, `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.CalculateaH` <- `HeatingAndCoolingResultCalc.CalculateParameterNign`
- `HeatingAndCoolingResultCalc.CalculateaHbaseLine` <- `HeatingAndCoolingResultCalc.CalculateParameterNignBaseLine`
- `HeatingAndCoolingResultCalc.CalculateaHesm` <- `HeatingAndCoolingResultCalc.CalculateParameterNiEsm`
- `HeatingAndCoolingResultCalc.CalculateaHref1` <- `HeatingAndCoolingResultCalc.CalculateParameterNignRef1`
- `HeatingAndCoolingResultCalc.CalculateaHref2` <- `HeatingAndCoolingResultCalc.CalculateParameterNignRef2`
- `HeatingAndCoolingResultCalc.Calculations` <- `HeatingAndCoolingResultCalc.CalculateEnergy`, `HeatingAndCoolingResultCalc.CalculateEnergyESM`
- `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingSavings`, `HeatingAndCoolingResultCalc.CalculateHeatingSavings`, `HeatingAndCoolingResultCalc.CalculateHotWaterSavings`, `HeatingAndCoolingResultCalc.CalculatePeriod`, `HeatingAndCoolingResultCalc.CalculateVentilationCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateVentilationHeatingSavings`
- `HeatingAndCoolingResultCalc.CheckCoolingForFansAndPumpsSavings` <- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingSavings`
- `HeatingAndCoolingResultCalc.CheckForDifferentFuelSources` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateHeatingSavings`, `HeatingAndCoolingResultCalc.CalculateHotWaterSavings`, `HeatingAndCoolingResultCalc.CalculateVentilationCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateVentilationHeatingSavings`
- `HeatingAndCoolingResultCalc.CheckForDifferentFuelSourcesESM` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateHeatingSavings`
- `HeatingAndCoolingResultCalc.CheckForFuelSavings` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateHeatingSavings`, `HeatingAndCoolingResultCalc.CalculateHotWaterSavings`, `HeatingAndCoolingResultCalc.CalculateVentilationCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateVentilationHeatingSavings`
- `HeatingAndCoolingResultCalc.CheckForHotWaterSavings` <- `HeatingAndCoolingResultCalc.CalculateHotWaterSavings`
- `HeatingAndCoolingResultCalc.CheckForNaN` <- `HeatingAndCoolingResultCalc.CalculateNetEnergyByTechnologies`, `HeatingAndCoolingResultCalc.CalculateNetEnergyPerArea`, `HeatingAndCoolingResultCalc.CalculateNetWithoutInputsEnergyByTechnologies`, `HeatingAndCoolingResultCalc.CalculateTotalActual`, `HeatingAndCoolingResultCalc.CalculateTotalActualYearly`, `HeatingAndCoolingResultCalc.CalculateTotalRefsYearly`, `HeatingAndCoolingResultCalc.Calculations`
- `HeatingAndCoolingResultCalc.CheckForSavings` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateHeatingSavings`
- `HeatingAndCoolingResultCalc.CheckForVentilationSavings` <- `HeatingAndCoolingResultCalc.CalculateVentilationCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateVentilationHeatingSavings`
- `HeatingAndCoolingResultCalc.CheckHeatingForFansAndPumpsSavings` <- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingSavings`
- `HeatingAndCoolingResultCalc.CheckLightsAndDevicesSavings` <- `HeatingAndCoolingResultCalc.CalculatePeriod`
- `HeatingAndCoolingResultCalc.ClaculateQfreecooling` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyActual`
- `HeatingAndCoolingResultCalc.ClaculateQfreecoolingBaseLine` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine`
- `HeatingAndCoolingResultCalc.ClaculateQfreecoolingEsm` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM`
- `HeatingAndCoolingResultCalc.ClaculateQfreecoolingRef1` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef1`
- `HeatingAndCoolingResultCalc.ClaculateQfreecoolingRef2` <- `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef2`
- `HeatingAndCoolingResultCalc.ClearFuelCells` <- `HeatingAndCoolingResultCalc.BuildingCalculations`, `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.ClearFuelCellsCO2` <- `HeatingAndCoolingResultCalc.BuildingCO2Calculations`, `HeatingAndCoolingResultCalc.ZoneCO2Calculations`
- `HeatingAndCoolingResultCalc.ClearFuelCellsPowerTable` <- `HeatingAndCoolingResultCalc.CalculateZonePowerEnergy`
- `HeatingAndCoolingResultCalc.ClearFuelCellsPowerTableBuilding` <- _None_
- `HeatingAndCoolingResultCalc.ClearNeededVEIenergy` <- `HeatingAndCoolingResultCalc.BuildingCalculations`, `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.ClearNetEnergy` <- `HeatingAndCoolingResultCalc.BuildingCalculations`, `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.ClearNetEnergyWithoutInputs` <- `HeatingAndCoolingResultCalc.BuildingCalculations`, `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.ClearPrimaryEnergy` <- `HeatingAndCoolingResultCalc.BuildingCalculations`, `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.ClearPrimaryEnergyFuelTableValues` <- `HeatingAndCoolingResultCalc.BuildingCalculations`, `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.ClearTableValues` <- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededPower`
- `HeatingAndCoolingResultCalc.ClearValuesCO2` <- `HeatingAndCoolingResultCalc.BuildingCO2Calculations`, `HeatingAndCoolingResultCalc.ZoneCO2Calculations`
- `HeatingAndCoolingResultCalc.ClearValuesCO2Actual` <- `HeatingAndCoolingResultCalc.ClearValuesCO2`
- `HeatingAndCoolingResultCalc.ClearValuesCO2BaseLine` <- `HeatingAndCoolingResultCalc.ClearValuesCO2`
- `HeatingAndCoolingResultCalc.ClearValuesCO2ESM` <- `HeatingAndCoolingResultCalc.ClearValuesCO2`
- `HeatingAndCoolingResultCalc.ClearValuesCO2Ref1` <- `HeatingAndCoolingResultCalc.ClearValuesCO2`
- `HeatingAndCoolingResultCalc.ClearValuesCO2Ref2` <- `HeatingAndCoolingResultCalc.ClearValuesCO2`
- `HeatingAndCoolingResultCalc.ClearValuesFuelActual` <- `HeatingAndCoolingResultCalc.ClearPrimaryEnergyFuelTableValues`
- `HeatingAndCoolingResultCalc.ClearValuesFuelBaseLine` <- `HeatingAndCoolingResultCalc.ClearPrimaryEnergyFuelTableValues`
- `HeatingAndCoolingResultCalc.ClearValuesFuelESM` <- `HeatingAndCoolingResultCalc.ClearPrimaryEnergyFuelTableValues`
- `HeatingAndCoolingResultCalc.ClearValuesFuelRef1` <- `HeatingAndCoolingResultCalc.ClearPrimaryEnergyFuelTableValues`
- `HeatingAndCoolingResultCalc.ClearValuesFuelRef2` <- `HeatingAndCoolingResultCalc.ClearPrimaryEnergyFuelTableValues`
- `HeatingAndCoolingResultCalc.Co2CalculateEmissionEnergySupplyBuilding` <- `HeatingAndCoolingResultCalc.BuildingCO2Calculations`
- `HeatingAndCoolingResultCalc.Co2EnergyCalcBuildingActual` <- `HeatingAndCoolingResultCalc.Co2CalculateEmissionEnergySupplyBuilding`
- `HeatingAndCoolingResultCalc.Co2EnergyCalcBuildingBaseLine` <- `HeatingAndCoolingResultCalc.Co2CalculateEmissionEnergySupplyBuilding`
- `HeatingAndCoolingResultCalc.Co2EnergyCalcBuildingESM` <- `HeatingAndCoolingResultCalc.Co2CalculateEmissionEnergySupplyBuilding`
- `HeatingAndCoolingResultCalc.Co2EnergyCalcBuildingRef1` <- `HeatingAndCoolingResultCalc.Co2CalculateEmissionEnergySupplyBuilding`
- `HeatingAndCoolingResultCalc.Co2EnergyCalcBuildingRef2` <- `HeatingAndCoolingResultCalc.Co2CalculateEmissionEnergySupplyBuilding`
- `HeatingAndCoolingResultCalc.Co2EnergyCalculateTotal` <- `HeatingAndCoolingResultCalc.BuildingCO2Calculations`, `HeatingAndCoolingResultCalc.ZoneCO2Calculations`
- `HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingActual` <- `HeatingAndCoolingResultCalc.Co2GetFuelTypesBuilding`
- `HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingBaseLine` <- `HeatingAndCoolingResultCalc.Co2GetFuelTypesBuilding`
- `HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingESM` <- `HeatingAndCoolingResultCalc.Co2GetFuelTypesBuilding`
- `HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingRef1` <- `HeatingAndCoolingResultCalc.Co2GetFuelTypesBuilding`
- `HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingRef2` <- `HeatingAndCoolingResultCalc.Co2GetFuelTypesBuilding`
- `HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneActual` <- `HeatingAndCoolingResultCalc.CO2EnergyZoneCalculations`
- `HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneBaseLine` <- `HeatingAndCoolingResultCalc.CO2EnergyZoneCalculations`
- `HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneESM` <- `HeatingAndCoolingResultCalc.CO2EnergyZoneCalculations`
- `HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneRef1` <- `HeatingAndCoolingResultCalc.CO2EnergyZoneCalculations`
- `HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneRef2` <- `HeatingAndCoolingResultCalc.CO2EnergyZoneCalculations`
- `HeatingAndCoolingResultCalc.Co2GetFuelTypesBuilding` <- `HeatingAndCoolingResultCalc.BuildingCO2Calculations`
- `HeatingAndCoolingResultCalc.CoolingCalculations` <- _None_
- `HeatingAndCoolingResultCalc.CopyByOrientation` <- `HeatingAndCoolingResultCalc.ApplyUdirectionWalls`
- `HeatingAndCoolingResultCalc.CopyCeilingElements` <- `HeatingAndCoolingResultCalc.CalculateUceilingsavings`
- `HeatingAndCoolingResultCalc.CopyCeilingElementsESM` <- `HeatingAndCoolingResultCalc.CalculateUceilingsavingsESM`
- `HeatingAndCoolingResultCalc.CopyCoolingWorkingSchedule` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`
- `HeatingAndCoolingResultCalc.CopyCoolingWorkingScheduleESM` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`
- `HeatingAndCoolingResultCalc.CopyFloorElements` <- `HeatingAndCoolingResultCalc.CalculateUfloorSavings`
- `HeatingAndCoolingResultCalc.CopyFloorElementsESM` <- `HeatingAndCoolingResultCalc.CalculateUfloorSavingsESM`
- `HeatingAndCoolingResultCalc.CopyGbyOrientation` <- `HeatingAndCoolingResultCalc.ApplyCoefficientG`
- `HeatingAndCoolingResultCalc.CopyHeatingWorkingSchedule` <- `HeatingAndCoolingResultCalc.CalculateHeatingSavings`
- `HeatingAndCoolingResultCalc.CopyHeatingWorkingScheduleESM` <- `HeatingAndCoolingResultCalc.CalculateHeatingSavings`
- `HeatingAndCoolingResultCalc.CopyInnerWallsElements` <- `HeatingAndCoolingResultCalc.CalculateUInnerWallsSaving`
- `HeatingAndCoolingResultCalc.CopyInnerWallsElementsESM` <- `HeatingAndCoolingResultCalc.CalculateUInnerWallsSavingESM`
- `HeatingAndCoolingResultCalc.CopyNonTrasparentElements` <- `HeatingAndCoolingResultCalc.CalculateUnonTransparentSavings`
- `HeatingAndCoolingResultCalc.CopyNonTrasparentElementsESM` <- `HeatingAndCoolingResultCalc.CalculateUnonTransparentSavingsESM`
- `HeatingAndCoolingResultCalc.CopyOtherFloorElements` <- `HeatingAndCoolingResultCalc.CalculateUfloorOthersavings`
- `HeatingAndCoolingResultCalc.CopyOtherFloorElementsESM` <- `HeatingAndCoolingResultCalc.CalculateUfloorOthersavingsESM`
- `HeatingAndCoolingResultCalc.CopyOuterWallsElements` <- `HeatingAndCoolingResultCalc.CalculateUOuterWallsSaving`
- `HeatingAndCoolingResultCalc.CopyOuterWallsElementsESM` <- `HeatingAndCoolingResultCalc.CalculateUOuterWallsSavingESM`
- `HeatingAndCoolingResultCalc.CopyTrasparentGelements` <- `HeatingAndCoolingResultCalc.CalculateGsavings`
- `HeatingAndCoolingResultCalc.CopyTrasparentGelementsESM` <- `HeatingAndCoolingResultCalc.CalculateGsavingsESM`
- `HeatingAndCoolingResultCalc.CopyVentilationCoolingWorkingSchedule` <- `HeatingAndCoolingResultCalc.CalculateVentilationCoolingSavings`
- `HeatingAndCoolingResultCalc.CopyVentilationHeatingWorkingSchedule` <- `HeatingAndCoolingResultCalc.CalculateVentilationHeatingSavings`
- `HeatingAndCoolingResultCalc.CopyWindowsElements` <- `HeatingAndCoolingResultCalc.CalculateUwindowsSavings`
- `HeatingAndCoolingResultCalc.CopyWindowsElementsESM` <- `HeatingAndCoolingResultCalc.CalculateUwindowsSavingsESM`
- `HeatingAndCoolingResultCalc.CopyWindowselements` <- `HeatingAndCoolingResultCalc.CalculateGsavings`
- `HeatingAndCoolingResultCalc.CopyWindowselementsESM` <- `HeatingAndCoolingResultCalc.CalculateGsavingsESM`
- `HeatingAndCoolingResultCalc.CreateCoolingVirtualBaseLine` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`
- `HeatingAndCoolingResultCalc.CreateCoolingVirtualESM` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`
- `HeatingAndCoolingResultCalc.CreateHeatingVirtualBaseLine` <- `HeatingAndCoolingResultCalc.CalculateHeatingSavings`
- `HeatingAndCoolingResultCalc.CreateHeatingVirtualESM` <- `HeatingAndCoolingResultCalc.CalculateHeatingSavings`
- `HeatingAndCoolingResultCalc.DefuseradiationHd` <- `HeatingAndCoolingResultCalc.CalculateProjectionCoeficient`
- `HeatingAndCoolingResultCalc.GetBaseLine` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateHeatingSavings`, `HeatingAndCoolingResultCalc.CreateCoolingVirtualBaseLine`, `HeatingAndCoolingResultCalc.CreateHeatingVirtualBaseLine`
- `HeatingAndCoolingResultCalc.GetBuildingData` <- `HeatingAndCoolingResultCalc.BuildingCalculations`
- `HeatingAndCoolingResultCalc.GetConditionedArea` <- `HeatingAndCoolingResultCalc.BuildingCalculations`
- `HeatingAndCoolingResultCalc.GetDaysHours` <- `HeatingAndCoolingResultCalc.CalculateLatentHeatsInf`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfBaseLine`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfESM`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfRef1`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfRef2`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVent`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentBaseLine`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentESM`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentRef1`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentRef2`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyActual`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyESM`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyRef1`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyRef2`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyActual`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyESM`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyRef1`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyRef2`
- `HeatingAndCoolingResultCalc.GetESM` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateHeatingSavings`, `HeatingAndCoolingResultCalc.CreateCoolingVirtualESM`, `HeatingAndCoolingResultCalc.CreateHeatingVirtualESM`
- `HeatingAndCoolingResultCalc.GetEkoCoeficient` <- `HeatingAndCoolingResultCalc.CalculateCO2EmissionsActual`, `HeatingAndCoolingResultCalc.CalculateCO2EmissionsBaseLine`, `HeatingAndCoolingResultCalc.CalculateCO2EmissionsESM`, `HeatingAndCoolingResultCalc.CalculateCO2EmissionsRef1`, `HeatingAndCoolingResultCalc.CalculateCO2EmissionsRef2`
- `HeatingAndCoolingResultCalc.GetFuelType` <- `HeatingAndCoolingResultCalc.GetFuelTypeAndValues`
- `HeatingAndCoolingResultCalc.GetFuelTypeAndValues` <- `HeatingAndCoolingResultCalc.BuildingCalculations`, `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.GetFuelTypeBaseLine` <- `HeatingAndCoolingResultCalc.GetFuelTypeAndValues`
- `HeatingAndCoolingResultCalc.GetFuelTypeCo2Actual` <- `HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingActual`, `HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneActual`
- `HeatingAndCoolingResultCalc.GetFuelTypeCo2BaseLine` <- `HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingBaseLine`, `HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneBaseLine`
- `HeatingAndCoolingResultCalc.GetFuelTypeCo2ESM` <- `HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingESM`, `HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneESM`
- `HeatingAndCoolingResultCalc.GetFuelTypeCo2Ref1` <- `HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingRef1`, `HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneRef1`
- `HeatingAndCoolingResultCalc.GetFuelTypeCo2Ref2` <- `HeatingAndCoolingResultCalc.Co2EnergyCalculationBuildingRef2`, `HeatingAndCoolingResultCalc.Co2EnergyCalculationZoneRef2`
- `HeatingAndCoolingResultCalc.GetFuelTypeEsm` <- `HeatingAndCoolingResultCalc.GetFuelTypeAndValues`
- `HeatingAndCoolingResultCalc.GetFuelTypeRef1` <- `HeatingAndCoolingResultCalc.GetFuelTypeAndValues`
- `HeatingAndCoolingResultCalc.GetFuelTypeRef2` <- `HeatingAndCoolingResultCalc.GetFuelTypeAndValues`
- `HeatingAndCoolingResultCalc.GetHotWaterBaseLine` <- `HeatingAndCoolingResultCalc.CalculateHotWaterSavings`
- `HeatingAndCoolingResultCalc.GetLightsAndDevicesInputs` <- `HeatingAndCoolingResultCalc.Calculations`
- `HeatingAndCoolingResultCalc.GetMonthHoursActual` <- `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyActual`
- `HeatingAndCoolingResultCalc.GetMonthHoursBaseLine` <- `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef1`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef2`
- `HeatingAndCoolingResultCalc.GetMonthHoursESM` <- `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyESM`
- `HeatingAndCoolingResultCalc.GetNightWorkingHours` <- `HeatingAndCoolingResultCalc.ClaculateQfreecooling`, `HeatingAndCoolingResultCalc.ClaculateQfreecoolingBaseLine`, `HeatingAndCoolingResultCalc.ClaculateQfreecoolingEsm`, `HeatingAndCoolingResultCalc.ClaculateQfreecoolingRef1`, `HeatingAndCoolingResultCalc.ClaculateQfreecoolingRef2`
- `HeatingAndCoolingResultCalc.GetPrimaryEnergyCoeficient` <- `HeatingAndCoolingResultCalc.CalculatePrimaryEnergyByTechnologies`
- `HeatingAndCoolingResultCalc.GetPrimaryFuelType` <- `HeatingAndCoolingResultCalc.GetPrimaryFuelTypeAndValues`
- `HeatingAndCoolingResultCalc.GetPrimaryFuelTypeAndValues` <- `HeatingAndCoolingResultCalc.BuildingCalculations`, `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.GetPrimaryFuelTypeBaseLine` <- `HeatingAndCoolingResultCalc.GetPrimaryFuelTypeAndValues`
- `HeatingAndCoolingResultCalc.GetPrimaryFuelTypeEsm` <- `HeatingAndCoolingResultCalc.GetPrimaryFuelTypeAndValues`
- `HeatingAndCoolingResultCalc.GetPrimaryFuelTypeRef1` <- `HeatingAndCoolingResultCalc.GetPrimaryFuelTypeAndValues`
- `HeatingAndCoolingResultCalc.GetPrimaryFuelTypeRef2` <- `HeatingAndCoolingResultCalc.GetPrimaryFuelTypeAndValues`
- `HeatingAndCoolingResultCalc.GetSaving` <- `HeatingAndCoolingResultCalc.SetCoolingFansAndPumpsSavingsValues`, `HeatingAndCoolingResultCalc.SetHeatingFansAndPumpsSavingsValues`, `HeatingAndCoolingResultCalc.SetHotWaterSavingsValues`, `HeatingAndCoolingResultCalc.SetLightsAndDevicesSavingsvalues`, `HeatingAndCoolingResultCalc.SetSavingsValues`, `HeatingAndCoolingResultCalc.SetVentilationSavingsValues`
- `HeatingAndCoolingResultCalc.GetTestValue` <- _None_
- `HeatingAndCoolingResultCalc.GetUceiling` <- _None_
- `HeatingAndCoolingResultCalc.GetUfloor` <- _None_
- `HeatingAndCoolingResultCalc.GetUnonTrasparentRoof` <- _None_
- `HeatingAndCoolingResultCalc.GetUotherFloor` <- _None_
- `HeatingAndCoolingResultCalc.GetValue` <- `HeatingAndCoolingResultCalc.SetBaseLine`, `HeatingAndCoolingResultCalc.SetESM`, `HeatingAndCoolingResultCalc.SetHotWaterBaseLine`, `HeatingAndCoolingResultCalc.SetVentilationBaseLine`
- `HeatingAndCoolingResultCalc.GetVeiBGV` <- `HeatingAndCoolingResultCalc.GetFuelTypeAndValues`
- `HeatingAndCoolingResultCalc.GetVeiHeatVentilation` <- `HeatingAndCoolingResultCalc.GetFuelTypeAndValues`
- `HeatingAndCoolingResultCalc.GetVeiHeating` <- `HeatingAndCoolingResultCalc.GetFuelTypeAndValues`
- `HeatingAndCoolingResultCalc.GetVentilationBaseLine` <- `HeatingAndCoolingResultCalc.CalculateVentilationCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateVentilationHeatingSavings`
- `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursActual` <- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingActual`
- `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursBaseLine` <- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingBaseLine`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingRef1`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingRef2`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingSavings`
- `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursEsm` <- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingEsm`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingSavings`
- `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursActual` <- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingActual`
- `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursBaseLine` <- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingBaseLine`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingRef1`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingRef2`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingSavings`
- `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursEsm` <- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingEsm`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingSavings`
- `HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursActual` <- _None_
- `HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursBaseLine` <- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingBaseLine`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingSavings`
- `HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursEsm` <- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingEsm`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingSavings`
- `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursActual` <- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingActual`
- `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursBaseLine` <- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingBaseLine`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingRef1`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingRef2`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingSavings`
- `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursEsm` <- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingEsm`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingSavings`
- `HeatingAndCoolingResultCalc.GetWeekHoursActual` <- _None_
- `HeatingAndCoolingResultCalc.GetWeekHoursBaseLine` <- _None_
- `HeatingAndCoolingResultCalc.GetWeekHoursCoolingActual` <- _None_
- `HeatingAndCoolingResultCalc.GetWeekHoursCoolingBaseLine` <- _None_
- `HeatingAndCoolingResultCalc.GetWeekHoursCoolingEsm` <- _None_
- `HeatingAndCoolingResultCalc.GetWeekHoursCoolingReferences` <- _None_
- `HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultActual` <- _None_
- `HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultBaseLine` <- _None_
- `HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultEsm` <- _None_
- `HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultReferences` <- _None_
- `HeatingAndCoolingResultCalc.GetWeekHoursESM` <- _None_
- `HeatingAndCoolingResultCalc.GetWeekHoursReferences` <- _None_
- `HeatingAndCoolingResultCalc.GetWeekHoursResultActual` <- _None_
- `HeatingAndCoolingResultCalc.GetWeekHoursResultBaseLine` <- _None_
- `HeatingAndCoolingResultCalc.GetWeekHoursResultEsm` <- _None_
- `HeatingAndCoolingResultCalc.GetWeekHoursResultReferences` <- _None_
- `HeatingAndCoolingResultCalc.HotWaterCalculationActual` <- _None_
- `HeatingAndCoolingResultCalc.HotWaterCalculationBaseLine` <- `HeatingAndCoolingResultCalc.CalculateHotWaterSavings`
- `HeatingAndCoolingResultCalc.HotWaterCalculationESM` <- _None_
- `HeatingAndCoolingResultCalc.HotWaterCalculationReferences` <- _None_
- `HeatingAndCoolingResultCalc.HotWaterNeededPower` <- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededPower`
- `HeatingAndCoolingResultCalc.HotWaterNeededPowerTotal` <- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededPower`
- `HeatingAndCoolingResultCalc.OccupantHours` <- `HeatingAndCoolingResultCalc.CalculateRef1`, `HeatingAndCoolingResultCalc.CalculateRef2`, `HeatingAndCoolingResultCalc.Calculations`
- `HeatingAndCoolingResultCalc.OccupantsHoursBaseLine` <- `HeatingAndCoolingResultCalc.Calculations`
- `HeatingAndCoolingResultCalc.OccupantsHoursEsm` <- `HeatingAndCoolingResultCalc.Calculations`
- `HeatingAndCoolingResultCalc.OccupantsHoursRef1` <- `HeatingAndCoolingResultCalc.CalculateRef1`
- `HeatingAndCoolingResultCalc.OccupantsHoursRef2` <- `HeatingAndCoolingResultCalc.CalculateRef2`
- `HeatingAndCoolingResultCalc.SetBaseLine` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateHeatingSavings`, `HeatingAndCoolingResultCalc.CreateCoolingVirtualBaseLine`, `HeatingAndCoolingResultCalc.CreateHeatingVirtualBaseLine`
- `HeatingAndCoolingResultCalc.SetCoolingFansAndPumpsSavingsValues` <- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingSavings`
- `HeatingAndCoolingResultCalc.SetESM` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateHeatingSavings`, `HeatingAndCoolingResultCalc.CreateCoolingVirtualESM`, `HeatingAndCoolingResultCalc.CreateHeatingVirtualESM`
- `HeatingAndCoolingResultCalc.SetFuelValue` <- `HeatingAndCoolingResultCalc.BuildingCalculations`
- `HeatingAndCoolingResultCalc.SetHeatingFansAndPumpsSavingsValues` <- `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingSavings`
- `HeatingAndCoolingResultCalc.SetHotWaterBaseLine` <- `HeatingAndCoolingResultCalc.CalculateHotWaterSavings`
- `HeatingAndCoolingResultCalc.SetHotWaterSavingsValues` <- `HeatingAndCoolingResultCalc.CalculateHotWaterSavings`
- `HeatingAndCoolingResultCalc.SetLightsAndDevicesSavingsvalues` <- `HeatingAndCoolingResultCalc.CalculatePeriod`
- `HeatingAndCoolingResultCalc.SetMonthRowValues` <- `HeatingAndCoolingResultCalc.SetTableResults`
- `HeatingAndCoolingResultCalc.SetNullValues` <- `HeatingAndCoolingResultCalc.ClearTableValues`
- `HeatingAndCoolingResultCalc.SetSavingsValues` <- `HeatingAndCoolingResultCalc.CalculateCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateHeatingSavings`
- `HeatingAndCoolingResultCalc.SetScaleType` <- `HeatingAndCoolingResultCalc.SetScaleValues`
- `HeatingAndCoolingResultCalc.SetScaleValues` <- `HeatingAndCoolingResultCalc.BuildingCalculations`
- `HeatingAndCoolingResultCalc.SetTableResults` <- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededPower`
- `HeatingAndCoolingResultCalc.SetVentilationBaseLine` <- `HeatingAndCoolingResultCalc.CalculateVentilationCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateVentilationHeatingSavings`
- `HeatingAndCoolingResultCalc.SetVentilationSavingsValues` <- `HeatingAndCoolingResultCalc.CalculateVentilationCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateVentilationHeatingSavings`
- `HeatingAndCoolingResultCalc.SubAngles` <- `HeatingAndCoolingResultCalc.CalculateMonthlyHorizontalRadiation`, `HeatingAndCoolingResultCalc.SunsetHourPrim`
- `HeatingAndCoolingResultCalc.SumAllDirectionWindowsCurrent` <- `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent`
- `HeatingAndCoolingResultCalc.SumAllDirectionWindowsEsm` <- `HeatingAndCoolingResultCalc.CalculateParameterHdEsm`
- `HeatingAndCoolingResultCalc.SumAllDirectionsWallsCurrent` <- `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent`
- `HeatingAndCoolingResultCalc.SumAllDirectionsWallsEsm` <- `HeatingAndCoolingResultCalc.CalculateParameterHdEsm`
- `HeatingAndCoolingResultCalc.SumCollectorsArea` <- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededPower`
- `HeatingAndCoolingResultCalc.SumItemsList` <- `HeatingAndCoolingResultCalc.GetLightsAndDevicesInputs`
- `HeatingAndCoolingResultCalc.SumNonTrasparentRoof` <- `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHdEsm`
- `HeatingAndCoolingResultCalc.SumTrasparentRoof` <- `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHdEsm`
- `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1` <- `HeatingAndCoolingResultCalc.CalculateParameterHtr`, `HeatingAndCoolingResultCalc.CalculateParameterHtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterHtrRef`, `HeatingAndCoolingResultCalc.CalculateParameterQtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateaH`, `HeatingAndCoolingResultCalc.CalculateaHbaseLine`
- `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1Cooling` <- `HeatingAndCoolingResultCalc.CalculateCoolingHtr`
- `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1CoolingESM` <- `HeatingAndCoolingResultCalc.CalculateCoolingHtrESM`
- `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1Esm` <- `HeatingAndCoolingResultCalc.CalculateParameterHtrEsm`, `HeatingAndCoolingResultCalc.CalculateParameterQtrEsm`, `HeatingAndCoolingResultCalc.CalculateaHesm`
- `HeatingAndCoolingResultCalc.SunDeclination` <- `HeatingAndCoolingResultCalc.CalculateMonthlyHorizontalRadiation`, `HeatingAndCoolingResultCalc.SunsetHour`, `HeatingAndCoolingResultCalc.SunsetHourPrim`
- `HeatingAndCoolingResultCalc.SunsetHour` <- `HeatingAndCoolingResultCalc.CalculateMonthlyHorizontalRadiation`, `HeatingAndCoolingResultCalc.SunsetHourPrim`
- `HeatingAndCoolingResultCalc.SunsetHourPrim` <- `HeatingAndCoolingResultCalc.CalculateMonthlyHorizontalRadiation`
- `HeatingAndCoolingResultCalc.UpdateActualState` <- `HeatingAndCoolingResultCalc.BuildingCalculations`
- `HeatingAndCoolingResultCalc.UpdateBaseLineState` <- `HeatingAndCoolingResultCalc.BuildingCalculations`
- `HeatingAndCoolingResultCalc.UpdateEsmState` <- `HeatingAndCoolingResultCalc.BuildingCalculations`
- `HeatingAndCoolingResultCalc.UpdateRefsState` <- `HeatingAndCoolingResultCalc.BuildingCalculations`
- `HeatingAndCoolingResultCalc.VentilationCoolEnergyActual` <- _None_
- `HeatingAndCoolingResultCalc.VentilationCoolEnergyBaseLine` <- `HeatingAndCoolingResultCalc.CalculateVentilationCoolingSavings`
- `HeatingAndCoolingResultCalc.VentilationCoolEnergyEsm` <- _None_
- `HeatingAndCoolingResultCalc.VentilationCoolEnergyRef1` <- _None_
- `HeatingAndCoolingResultCalc.VentilationCoolEnergyRef2` <- _None_
- `HeatingAndCoolingResultCalc.VentilationHeatEnergyActual` <- _None_
- `HeatingAndCoolingResultCalc.VentilationHeatEnergyBaseLine` <- `HeatingAndCoolingResultCalc.CalculateVentilationHeatingSavings`
- `HeatingAndCoolingResultCalc.VentilationHeatEnergyESM` <- _None_
- `HeatingAndCoolingResultCalc.VentilationHeatEnergyRef1` <- _None_
- `HeatingAndCoolingResultCalc.VentilationHeatEnergyRef2` <- _None_
- `HeatingAndCoolingResultCalc.ZoneCO2Calculations` <- `HeatingAndCoolingResultCalc.ZoneCalculations`
- `HeatingAndCoolingResultCalc.ZoneCalculations` <- _None_
- `InputDataCalc.CalcHours` <- `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursActual`, `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursEsm`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursActual`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursEsm`, `HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursActual`, `HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursEsm`, `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursActual`, `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursEsm`, `HeatingAndCoolingResultCalc.GetWeekHoursActual`, `HeatingAndCoolingResultCalc.GetWeekHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekHoursCoolingActual`, `HeatingAndCoolingResultCalc.GetWeekHoursCoolingBaseLine`, `HeatingAndCoolingResultCalc.GetWeekHoursCoolingEsm`, `HeatingAndCoolingResultCalc.GetWeekHoursCoolingReferences`, `HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultActual`, `HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultBaseLine`, `HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultEsm`, `HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultReferences`, `HeatingAndCoolingResultCalc.GetWeekHoursESM`, `HeatingAndCoolingResultCalc.GetWeekHoursReferences`, `HeatingAndCoolingResultCalc.GetWeekHoursResultActual`, `HeatingAndCoolingResultCalc.GetWeekHoursResultBaseLine`, `HeatingAndCoolingResultCalc.GetWeekHoursResultEsm`, `HeatingAndCoolingResultCalc.GetWeekHoursResultReferences`
- `InputDataCalc.CalcPeriod` <- `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActual`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActualBalanced`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActualHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodActualNonBalanced`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLine`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLineBalanced`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLineHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodBaseLineNonBalanced`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESM`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESMBalanced`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESMHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodESMNonBalanced`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1Balanced`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1HotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef1NonBalanced`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2Balanced`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2HotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateAnnualPeriodRef2NonBalanced`, `HeatingAndCoolingResultCalc.CalculateBalancedDevicesSavings`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActual`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualNonBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLine`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineNonBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESM`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMNonBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1Balanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1HotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1NonBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2Balanced`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2HotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2NonBalanced`, `HeatingAndCoolingResultCalc.CalculateCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingActual`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingBaseLine`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingEsm`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingRef1`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingRef2`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingSavings`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingActual`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingBaseLine`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingEsm`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingRef1`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingRef2`, `HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingSavings`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActual`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualNonBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLine`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineNonBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESM`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMHotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMNonBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1Balanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1HotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1NonBalanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2Balanced`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2HotWaterPumps`, `HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2NonBalanced`, `HeatingAndCoolingResultCalc.CalculateHotWaterNeededPower`, `HeatingAndCoolingResultCalc.CalculateHotWaterPumpsSavings`, `HeatingAndCoolingResultCalc.CalculateLightsSavings`, `HeatingAndCoolingResultCalc.CalculateNonBalancedDevicesSavings`, `HeatingAndCoolingResultCalc.Calculations`, `HeatingAndCoolingResultCalc.CoolingCalculations`, ... (+10)
- `InputDataCalc.CalculateMonthlyDays` <- `InputDataCalc.CalcPeriod`
- `InputDataCalc.GetHollydays` <- `InputDataCalc.CalculateMonthlyDays`
- `InputDataCalc.GetWeeksInMonth` <- `InputDataCalc.CalculateMonthlyDays`
- `MonthlyDays.MonthlyDays` <- _None_
- `MonthlyDays.MonthlyDays` <- _None_
- `PreferencesManager.GetClimateZoneParams` <- `HeatingAndCoolingResultCalc.CalcAvgNonProjectTemp`, `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempRef1`, `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempRef2`, `HeatingAndCoolingResultCalc.CalcAvgProjectTemp`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempRef1`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempRef2`, `HeatingAndCoolingResultCalc.CalculateAc`, `HeatingAndCoolingResultCalc.CalculateAcBaseLine`, `HeatingAndCoolingResultCalc.CalculateAcESM`, `HeatingAndCoolingResultCalc.CalculateAcRef1`, `HeatingAndCoolingResultCalc.CalculateAcRef2`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyActual`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef1`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef2`, `HeatingAndCoolingResultCalc.CalculateCoolingQtr`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrESM`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrRef1`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrRef2`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyActual`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyESM`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef1`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef2`, `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsol`, `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsolEsm`, `HeatingAndCoolingResultCalc.CalculateParameterQtr`, `HeatingAndCoolingResultCalc.CalculateParameterQtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterQtrEsm`, `HeatingAndCoolingResultCalc.CalculateParameterQtrRef1`, `HeatingAndCoolingResultCalc.CalculateParameterQtrRef2`, `HeatingAndCoolingResultCalc.CalculateParameterQveBaseLIne`, `HeatingAndCoolingResultCalc.CalculateParameterQveEsm`, `HeatingAndCoolingResultCalc.CalculateParameterQveRef2`, `HeatingAndCoolingResultCalc.CalculateParameterX`, `HeatingAndCoolingResultCalc.CalculateQinf`, `HeatingAndCoolingResultCalc.CalculateQinfBaseLine`, `HeatingAndCoolingResultCalc.CalculateQinfESM`, `HeatingAndCoolingResultCalc.CalculateQinfRef1`, `HeatingAndCoolingResultCalc.CalculateQinfRef2`, `HeatingAndCoolingResultCalc.CalculateTrasparentFsol`, `HeatingAndCoolingResultCalc.CalculateTrasparentFsolEsm`, `HeatingAndCoolingResultCalc.CalculateaH`, `HeatingAndCoolingResultCalc.CalculateaHbaseLine`, `HeatingAndCoolingResultCalc.CalculateaHesm`, `HeatingAndCoolingResultCalc.CalculateaHref1`, `HeatingAndCoolingResultCalc.CalculateaHref2`, `HeatingAndCoolingResultCalc.Calculations`, `HeatingAndCoolingResultCalc.ClaculateQfreecooling`, `HeatingAndCoolingResultCalc.ClaculateQfreecoolingBaseLine`, `HeatingAndCoolingResultCalc.ClaculateQfreecoolingEsm`, `HeatingAndCoolingResultCalc.ClaculateQfreecoolingRef1`, `HeatingAndCoolingResultCalc.ClaculateQfreecoolingRef2`, `HeatingAndCoolingResultCalc.GetDaysHours`, `HeatingAndCoolingResultCalc.GetTestValue`
- `RoofTableCalc.CalculateAlfa` <- _None_
- `RoofTableCalc.CalculateArea` <- `HeatingAndCoolingResultCalc.ApplyUroofsAndCeilings`
- `RoofTableCalc.CalculateCeilingU` <- `HeatingAndCoolingResultCalc.ApplyUroofsAndCeilings`
- `RoofTableCalc.CalculateEpsilon` <- _None_
- `RoofTableCalc.CalculateNonTranspU` <- `HeatingAndCoolingResultCalc.ApplyUroofsAndCeilings`
- `RoofTableCalc.CalculateTrasparentG` <- `HeatingAndCoolingResultCalc.ApplyCoefficientG`
- `RoofTableCalc.CalculateTrasparentU` <- `HeatingAndCoolingResultCalc.ApplyToTrasparentRoofs`
- `RoofTableCalc.SumCeilingArea` <- `HeatingAndCoolingResultCalc.ApplyUroofsAndCeilings`
- `RoofTableCalc.SumL` <- _None_
- `RoofTableCalc.SumTrasparentArea` <- `HeatingAndCoolingResultCalc.ApplyCoefficientG`, `HeatingAndCoolingResultCalc.ApplyToTrasparentRoofs`
- `RoofTableCalc.SumX` <- _None_
- `SavingsData.OnPropertyChanged` <- _None_
- `SunEnergyPreferencesManager.GetClimateZoneParams` <- `HeatingAndCoolingResultCalc.CalculateHotWaterNeededPower`, `HeatingAndCoolingResultCalc.CalculateParameterHtMonthly`, `HeatingAndCoolingResultCalc.DefuseradiationHd`
- `TempBridgeCalc.CalculateSums` <- _None_
- `WallsTableCalc.AccumulateOuterU` <- `HeatingAndCoolingResultCalc.CopyByOrientation`
- `WallsTableCalc.AcumulateOuterAlfa` <- _None_
- `WallsTableCalc.AcumulateOuterEpsilon` <- _None_
- `WallsTableCalc.CalculateInnerU` <- `HeatingAndCoolingResultCalc.CopyByOrientation`
- `WallsTableCalc.CalculateWindowE` <- _None_
- `WallsTableCalc.CalculateWindowG` <- `HeatingAndCoolingResultCalc.CopyGbyOrientation`
- `WallsTableCalc.CalculateWindowU` <- `HeatingAndCoolingResultCalc.CopyByOrientation`
- `WallsTableCalc.SumColumnInnerArea` <- `HeatingAndCoolingResultCalc.CopyByOrientation`
- `WallsTableCalc.SumColumnOuterArea` <- `HeatingAndCoolingResultCalc.CopyByOrientation`
- `WallsTableCalc.SumColumnOuterL` <- _None_
- `WallsTableCalc.SumColumnOuterX` <- _None_
- `WallsTableCalc.SumWindowArea` <- `HeatingAndCoolingResultCalc.CopyByOrientation`, `HeatingAndCoolingResultCalc.CopyGbyOrientation`