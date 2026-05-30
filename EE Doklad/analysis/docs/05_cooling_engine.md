# 05 Cooling Engine

Cooling-related methods and formulas extracted from the decompiled source, including cooling balance, gains, ventilation cooling, latent/withering calculations, and result conversion.

- Covered methods: `164`

## Method Flow

### HeatingAndCoolingResultCalc.CoolingCalculations
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:123`
- Inputs: `this CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `BuildingZone zone`, `CalculationData lightsAndDevicesCalculationData`, `CalculationData ventCool`
- Outputs: writes `buildingZone (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:124`; `monthslist (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:125`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateCoolingEnergyActual`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef1`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef2`, `InputDataCalc.CalcPeriod`
- Formulas: _None extracted_

### HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:137`
- Inputs: `this CalculationData coolingCalc`, `Section section`
- Outputs: writes `coolingCalc.WorkingScheduleActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:141`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:139`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:140`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:138`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:138` `double num = 5.0 * section.CalcHours(section.CoolingSeasons.Cooling.WorkCurrentStart, section.CoolingSeasons.Cooling.WorkCurrentEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:139` `num += section.CalcHours(section.CoolingSeasons.Cooling.SunCurrentStart, section.CoolingSeasons.Cooling.SunCurrentEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:140` `num += section.CalcHours(section.CoolingSeasons.Cooling.SatCurrentStart, section.CoolingSeasons.Cooling.SatCurrentEnd);`

### HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:145`
- Inputs: `this CalculationData coolingCalc`, `Section section`
- Outputs: writes `coolingCalc.WorkingScheduleBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:149`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:147`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:148`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:146`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:146` `double num = 5.0 * section.CalcHours(section.CoolingSeasons.Cooling.WorkBaseStart, section.CoolingSeasons.Cooling.WorkBaseEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:147` `num += section.CalcHours(section.CoolingSeasons.Cooling.SunBaseStart, section.CoolingSeasons.Cooling.SunBaseEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:148` `num += section.CalcHours(section.CoolingSeasons.Cooling.SatBaseStart, section.CoolingSeasons.Cooling.SatBaseEnd);`

### HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:153`
- Inputs: `this CalculationData coolingCalc`, `Section section`
- Outputs: writes `coolingCalc.WorkingScheduleESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:157`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:155`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:156`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:154`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:154` `double num = 5.0 * section.CalcHours(section.CoolingSeasons.Cooling.WorkEsmStart, section.CoolingSeasons.Cooling.WorkEsmEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:155` `num += section.CalcHours(section.CoolingSeasons.Cooling.SunEsmStart, section.CoolingSeasons.Cooling.SunEsmEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:156` `num += section.CalcHours(section.CoolingSeasons.Cooling.SatEsmStart, section.CoolingSeasons.Cooling.SatEsmEnd);`

### HeatingAndCoolingResultCalc.GetWeekHoursCoolingResultReferences
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:161`
- Inputs: `this CalculationData coolingCalc`, `Section section`
- Outputs: writes `coolingCalc.WorkingScheduleRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:165`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:163`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:162`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:164`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:162` `double num = 5.0 * section.CalcHours(section.CoolingSeasons.Cooling.WorkBaseStart, section.CoolingSeasons.Cooling.WorkBaseEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:163` `num += section.CalcHours(section.CoolingSeasons.Cooling.SunBaseStart, section.CoolingSeasons.Cooling.SunBaseEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:164` `num = (coolingCalc.WorkingScheduleRef = num + section.CalcHours(section.CoolingSeasons.Cooling.SatBaseStart, section.CoolingSeasons.Cooling.SatBaseEnd));`

### HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:169`
- Inputs: `List<MonthlyDays> monthslist`, `CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `CalculationData lightsAndDevicesCalculationData`, `CalculationData ventCool`
- Outputs: writes `MonthDataCoolingList (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:202`; `calcData.ResulNetEnergyRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:206`; `calcData.ResulNoInputsNetEnergyRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:204`; `item (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:188`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:170`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:171`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:172`; `list4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:173`; `monthDataCooling (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:178`; `monthDataCooling.AvgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:190`; ... (+19)
- Internal calls: `HeatingAndCoolingResultCalc.ApplyValuesToTempSectionRef1`, `HeatingAndCoolingResultCalc.CalculateAcRef1`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrRef1`, `HeatingAndCoolingResultCalc.CalculateETA`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfRef1`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentRef1`, `HeatingAndCoolingResultCalc.CalculateQLatentOccupantsRef1`, `HeatingAndCoolingResultCalc.CalculateQgainRef1`, `HeatingAndCoolingResultCalc.CalculateQinfRef1`, `HeatingAndCoolingResultCalc.CalculateQveRef1`, `HeatingAndCoolingResultCalc.ClaculateQfreecoolingRef1`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:180` `double num2 = CalculateCoolingQtrRef1(calcData, section, calcInput.General.ClimateZone, item2) + CalculateQinfRef1(section, calcInput.General.ClimateZone, calcData, item2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:183` `double num4 = num - num3 * num2 + CalculateQLatentOccupantsRef1(section, item2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:184` `num4 = num4 + CalculateLatentHeatsInfRef1(section, calcData, item2, calcInput.General.ClimateZone) + CalculateLatentHeatsVentRef1(section, calcData, item2, calcInput.General.ClimateZone, calcData);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:188` `double item = num4 + num5 + CalculateQveRef1(section, calcData, ventCool, item2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:203` `double num6 = list3.Aggregate(0.0, (double num9, double num10) => num9 + num10);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:204` `calcData.ResulNoInputsNetEnergyRef1 = num6 / section.Area.HeatedArea;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:205` `double num7 = (calcData.ResulCoolingInputsRef1 = list2.Aggregate(0.0, (double num9, double num10) => num9 + num10));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:206` `calcData.ResulNetEnergyRef1 = num6 / section.Area.HeatedArea - num7 - calcData.ResulVentilationInputsRef1;`

### HeatingAndCoolingResultCalc.CalculateCoolingEnergyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:210`
- Inputs: `List<MonthlyDays> monthslist`, `CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `CalculationData lightsAndDevicesCalculationData`, `CalculationData ventCool`
- Outputs: writes `MonthDataCoolingList (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:243`; `calcData.ResulNetEnergyRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:247`; `calcData.ResulNoInputsNetEnergyRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:245`; `item (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:229`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:211`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:212`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:213`; `list4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:214`; `monthDataCooling (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:219`; `monthDataCooling.AvgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:231`; ... (+19)
- Internal calls: `HeatingAndCoolingResultCalc.ApplyValuesToTempSectionRef2`, `HeatingAndCoolingResultCalc.CalculateAcRef2`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrRef2`, `HeatingAndCoolingResultCalc.CalculateETA`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfRef2`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentRef2`, `HeatingAndCoolingResultCalc.CalculateQLatentOccupantsRef2`, `HeatingAndCoolingResultCalc.CalculateQgainRef2`, `HeatingAndCoolingResultCalc.CalculateQinfRef2`, `HeatingAndCoolingResultCalc.CalculateQveRef2`, `HeatingAndCoolingResultCalc.ClaculateQfreecoolingRef2`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:221` `double num2 = CalculateCoolingQtrRef2(calcData, section, calcInput.General.ClimateZone, item2) + CalculateQinfRef2(section, calcInput.General.ClimateZone, calcData, item2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:224` `double num4 = num - num3 * num2 + CalculateQLatentOccupantsRef2(section, item2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:225` `num4 = num4 + CalculateLatentHeatsInfRef2(section, calcData, item2, calcInput.General.ClimateZone) + CalculateLatentHeatsVentRef2(section, calcData, item2, calcInput.General.ClimateZone, calcData);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:229` `double item = num4 + num5 + CalculateQveRef2(section, calcData, ventCool, item2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:244` `double num6 = list3.Aggregate(0.0, (double num9, double num10) => num9 + num10);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:245` `calcData.ResulNoInputsNetEnergyRef2 = num6 / section.Area.HeatedArea;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:246` `double num7 = (calcData.ResulCoolingInputsRef2 = list2.Aggregate(0.0, (double num9, double num10) => num9 + num10));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:247` `calcData.ResulNetEnergyRef2 = num6 / section.Area.HeatedArea - num7 - calcData.ResulVentilationInputsRef2;`

### HeatingAndCoolingResultCalc.CalculateCoolingEnergyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:251`
- Inputs: `List<MonthlyDays> monthslist`, `CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `CalculationData lightsAndDevicesCalculationData`, `CalculationData ventCool`
- Outputs: writes `MonthDataCoolingList (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:282`; `calcData.ResulNetEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:286`; `calcData.ResulNoInputsNetEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:284`; `item (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:268`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:252`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:253`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:254`; `list4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:255`; `monthDataCooling (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:258`; `monthDataCooling.AvgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:270`; ... (+18)
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAc`, `HeatingAndCoolingResultCalc.CalculateCoolingQtr`, `HeatingAndCoolingResultCalc.CalculateETA`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInf`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVent`, `HeatingAndCoolingResultCalc.CalculateQLatentOccupants`, `HeatingAndCoolingResultCalc.CalculateQgain`, `HeatingAndCoolingResultCalc.CalculateQinf`, `HeatingAndCoolingResultCalc.CalculateQve`, `HeatingAndCoolingResultCalc.ClaculateQfreecooling`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:260` `double num2 = CalculateCoolingQtr(calcData, section, calcInput.General.ClimateZone, item2) + CalculateQinf(section, calcInput.General.ClimateZone, calcData, item2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:263` `double num4 = num - num3 * num2 + CalculateQLatentOccupants(section, item2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:264` `num4 = num4 + CalculateLatentHeatsInf(section, calcData, item2, calcInput.General.ClimateZone) + CalculateLatentHeatsVent(section, calcData, item2, calcInput.General.ClimateZone, calcData);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:268` `double item = num4 + num5 + CalculateQve(section, calcData, ventCool, item2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:283` `double num6 = list2.Aggregate(0.0, (double num9, double num10) => num9 + num10);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:284` `calcData.ResulNoInputsNetEnergyActual = num6 / section.Area.HeatedArea;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:285` `double num7 = (calcData.ResulCoolingInputsActual = list4.Aggregate(0.0, (double num9, double num10) => num9 + num10));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:286` `calcData.ResulNetEnergyActual = num6 / section.Area.HeatedArea - num7 - calcData.ResulVentilationInputsActual;`

### HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:290`
- Inputs: `List<MonthlyDays> monthslist`, `CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `CalculationData lightsAndDevicesCalculationData`, `CalculationData ventCool`
- Outputs: writes `calcData.ResulNetEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:311`; `calcData.ResulNoInputsNetEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:309`; `item (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:305`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:291`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:292`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:293`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:296`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:297`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:299`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:300`; ... (+5)
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAcBaseLine`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateETA`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfBaseLine`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentBaseLine`, `HeatingAndCoolingResultCalc.CalculateQLatentOccupantsBaseLine`, `HeatingAndCoolingResultCalc.CalculateQgainBaseLine`, `HeatingAndCoolingResultCalc.CalculateQinfBaseLine`, `HeatingAndCoolingResultCalc.CalculateQveBaseLine`, `HeatingAndCoolingResultCalc.ClaculateQfreecoolingBaseLine`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:297` `double num2 = CalculateCoolingQtrBaseLine(calcData, section, calcInput.General.ClimateZone, item2) + CalculateQinfBaseLine(section, calcInput.General.ClimateZone, calcData, item2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:300` `double num4 = num - num3 * num2 + CalculateQLatentOccupantsBaseLine(section, item2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:301` `num4 = num4 + CalculateLatentHeatsInfBaseLine(section, calcData, item2, calcInput.General.ClimateZone) + CalculateLatentHeatsVentBaseLine(section, calcData, item2, calcInput.General.ClimateZone, calcData);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:305` `double item = num4 + num5 + CalculateQveBaseLine(section, calcData, ventCool, item2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:308` `double num6 = list.Aggregate(0.0, (double num9, double num10) => num9 + num10);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:309` `calcData.ResulNoInputsNetEnergyBaseLine = num6 / section.Area.HeatedArea;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:310` `double num7 = (calcData.ResulCoolingInputsBaseLine = list3.Aggregate(0.0, (double num9, double num10) => num9 + num10));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:311` `calcData.ResulNetEnergyBaseLine = num6 / section.Area.HeatedArea - num7 - calcData.ResulVentilationInputsBaseLine;`

### HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:315`
- Inputs: `List<MonthlyDays> monthslist`, `CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `CalculationData lightsAndDevicesCalculationData`, `CalculationData ventCool`
- Outputs: writes `calcData.ResulNetEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:336`; `calcData.ResulNoInputsNetEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:334`; `item (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:330`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:316`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:317`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:318`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:321`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:322`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:324`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:325`; ... (+5)
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAcESM`, `HeatingAndCoolingResultCalc.CalculateCoolingQtrESM`, `HeatingAndCoolingResultCalc.CalculateETA`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsInfESM`, `HeatingAndCoolingResultCalc.CalculateLatentHeatsVentESM`, `HeatingAndCoolingResultCalc.CalculateQLatentOccupantsESM`, `HeatingAndCoolingResultCalc.CalculateQgainESM`, `HeatingAndCoolingResultCalc.CalculateQinfESM`, `HeatingAndCoolingResultCalc.CalculateQveESM`, `HeatingAndCoolingResultCalc.ClaculateQfreecoolingEsm`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:322` `double num2 = CalculateCoolingQtrESM(calcData, section, calcInput.General.ClimateZone, item2) + CalculateQinfESM(section, calcInput.General.ClimateZone, calcData, item2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:325` `double num4 = num - num3 * num2 + CalculateQLatentOccupantsESM(section, item2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:326` `num4 = num4 + CalculateLatentHeatsInfESM(section, calcData, item2, calcInput.General.ClimateZone) + CalculateLatentHeatsVentESM(section, calcData, item2, calcInput.General.ClimateZone, calcData);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:330` `double item = num4 + num5 + CalculateQveESM(section, calcData, ventCool, item2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:333` `double num6 = list.Aggregate(0.0, (double num9, double num10) => num9 + num10);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:334` `calcData.ResulNoInputsNetEnergyESM = num6 / section.Area.HeatedArea;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:335` `double num7 = (calcData.ResulCoolingInputsESM = list3.Aggregate(0.0, (double num9, double num10) => num9 + num10));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:336` `calcData.ResulNetEnergyESM = num6 / section.Area.HeatedArea - num7 - calcData.ResulVentilationInputsESM;`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsInfRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:340`
- Inputs: `Section section`, `CalculationData calcData`, `MonthlyDays month`, `ClimateZones climateZone`
- Outputs: returns `double`; writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:341`; `index (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:353`; `index2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:371`; `index3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:387`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:342`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:347`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:359`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:343`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:394`; `num13 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:395`; ... (+15)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:347` `num += CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRe...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:348` `list.Add(CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidity...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:353` `int index = ((j < daysHours.Count) ? j : 0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:354` `list.Add(CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.ProjectTemperatureRef1, calcData.Proje...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:355` `num2 += CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.ProjectTemperatureRef1, calcData.Projec...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:359` `num += CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRe...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:360` `list.Add(CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidity...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:362` `double num3 = (num + num2) * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:367` `num5 += CalcRo(daysHours[l].Temp, daysHours[l].Humidity) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity) - CalcRo(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityR...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:371` `int index2 = ((m < daysHours.Count) ? m : 0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:372` `num4 += CalcRo(daysHours[index2].Temp, daysHours[index2].Humidity) * CalcAirX(daysHours[index2].Temp, daysHours[index2].Humidity) - CalcRo(calcData.ProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.ProjectTemperatureRef1, calcData.Pr...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:376` `num5 += CalcRo(daysHours[n].Temp, daysHours[n].Humidity) * CalcAirX(daysHours[n].Temp, daysHours[n].Humidity) - CalcRo(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityR...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:378` `double num6 = (num4 + num5) * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:383` `num8 += CalcRo(daysHours[num9].Temp, daysHours[num9].Humidity) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity) - CalcRo(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.NonProjectTemperatureRef1, calcData.Proj...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:387` `int index3 = ((num10 < daysHours.Count) ? num10 : 0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:388` `num7 += CalcRo(daysHours[index3].Temp, daysHours[index3].Humidity) * CalcAirX(daysHours[index3].Temp, daysHours[index3].Humidity) - CalcRo(calcData.ProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.ProjectTemperatureRef1, calcData.Pr...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:392` `num8 += CalcRo(daysHours[num11].Temp, daysHours[num11].Humidity) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity) - CalcRo(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.NonProjectTemperatureRef1, calcData....`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:394` `double num12 = (num7 + num8) * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:395` `double num13 = section.Area.HeatedVolume * calcData.InfiltracionRef1 * (1.0 / section.Area.HeatedArea) * (num3 + num6 + num12) * 0.6947222222222222;`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsInfRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:404`
- Inputs: `Section section`, `CalculationData calcData`, `MonthlyDays month`, `ClimateZones climateZone`
- Outputs: returns `double`; writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:405`; `index (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:417`; `index2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:435`; `index3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:451`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:406`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:411`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:423`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:407`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:458`; `num13 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:459`; ... (+15)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:411` `num += CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRe...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:412` `list.Add(CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidity...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:417` `int index = ((j < daysHours.Count) ? j : 0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:418` `list.Add(CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.ProjectTemperatureRef2, calcData.Proje...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:419` `num2 += CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.ProjectTemperatureRef2, calcData.Projec...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:423` `num += CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRe...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:424` `list.Add(CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidity...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:426` `double num3 = (num + num2) * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:431` `num5 += CalcRo(daysHours[l].Temp, daysHours[l].Humidity) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity) - CalcRo(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityR...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:435` `int index2 = ((m < daysHours.Count) ? m : 0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:436` `num4 += CalcRo(daysHours[index2].Temp, daysHours[index2].Humidity) * CalcAirX(daysHours[index2].Temp, daysHours[index2].Humidity) - CalcRo(calcData.ProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.ProjectTemperatureRef2, calcData.Pr...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:440` `num5 += CalcRo(daysHours[n].Temp, daysHours[n].Humidity) * CalcAirX(daysHours[n].Temp, daysHours[n].Humidity) - CalcRo(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityR...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:442` `double num6 = (num4 + num5) * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:447` `num8 += CalcRo(daysHours[num9].Temp, daysHours[num9].Humidity) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity) - CalcRo(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.NonProjectTemperatureRef2, calcData.Proj...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:451` `int index3 = ((num10 < daysHours.Count) ? num10 : 0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:452` `num7 += CalcRo(daysHours[index3].Temp, daysHours[index3].Humidity) * CalcAirX(daysHours[index3].Temp, daysHours[index3].Humidity) - CalcRo(calcData.ProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.ProjectTemperatureRef2, calcData.Pr...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:456` `num8 += CalcRo(daysHours[num11].Temp, daysHours[num11].Humidity) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity) - CalcRo(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.NonProjectTemperatureRef2, calcData....`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:458` `double num12 = (num7 + num8) * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:459` `double num13 = section.Area.HeatedVolume * calcData.InfiltracionRef2 * (1.0 / section.Area.HeatedArea) * (num3 + num6 + num12) * 0.6947222222222222;`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsInf
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:468`
- Inputs: `Section section`, `CalculationData calcData`, `MonthlyDays month`, `ClimateZones climateZone`
- Outputs: returns `double`; writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:469`; `index (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:481`; `index2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:499`; `index3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:515`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:470`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:475`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:487`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:471`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:522`; `num13 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:523`; ... (+15)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:475` `num += CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.NonProjectTemperatureActual, calcData.ProjectHumi...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:476` `list.Add(CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.NonProjectTemperatureActual, calcData.ProjectHu...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:481` `int index = ((j < daysHours.Count) ? j : 0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:482` `list.Add(CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.ProjectTemperatureActual, calcData...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:483` `num2 += CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.ProjectTemperatureActual, calcData....`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:487` `num += CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.NonProjectTemperatureActual, calcData.ProjectHumi...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:488` `list.Add(CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.NonProjectTemperatureActual, calcData.ProjectHu...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:490` `double num3 = (num + num2) * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:495` `num5 += CalcRo(daysHours[l].Temp, daysHours[l].Humidity) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity) - CalcRo(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.NonProjectTemperatureActual, calcData.ProjectHum...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:499` `int index2 = ((m < daysHours.Count) ? m : 0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:500` `num4 += CalcRo(daysHours[index2].Temp, daysHours[index2].Humidity) * CalcAirX(daysHours[index2].Temp, daysHours[index2].Humidity) - CalcRo(calcData.ProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.ProjectTemperatureActual, calcD...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:504` `num5 += CalcRo(daysHours[n].Temp, daysHours[n].Humidity) * CalcAirX(daysHours[n].Temp, daysHours[n].Humidity) - CalcRo(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.NonProjectTemperatureActual, calcData.ProjectHum...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:506` `double num6 = (num4 + num5) * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:511` `num8 += CalcRo(daysHours[num9].Temp, daysHours[num9].Humidity) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity) - CalcRo(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.NonProjectTemperatureActual, calcDat...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:515` `int index3 = ((num10 < daysHours.Count) ? num10 : 0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:516` `num7 += CalcRo(daysHours[index3].Temp, daysHours[index3].Humidity) * CalcAirX(daysHours[index3].Temp, daysHours[index3].Humidity) - CalcRo(calcData.ProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.ProjectTemperatureActual, calcD...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:520` `num8 += CalcRo(daysHours[num11].Temp, daysHours[num11].Humidity) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity) - CalcRo(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.NonProjectTemperatureActual, cal...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:522` `double num12 = (num7 + num8) * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:523` `double num13 = section.Area.HeatedVolume * calcData.InfiltracionActual * (num3 + num6 + num12) * 0.6947222222222222 / section.Area.HeatedArea;`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsInfBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:532`
- Inputs: `Section section`, `CalculationData calcData`, `MonthlyDays month`, `ClimateZones climateZone`
- Outputs: returns `double`; writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:533`; `index (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:545`; `index2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:563`; `index3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:579`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:534`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:539`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:551`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:535`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:586`; `num13 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:587`; ... (+15)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:539` `num += CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.NonProjectTemperatureBaseLine, calcData.Proje...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:540` `list.Add(CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.NonProjectTemperatureBaseLine, calcData.Pro...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:545` `int index = ((j < daysHours.Count) ? j : 0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:546` `list.Add(CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.ProjectTemperatureBaseLine, ca...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:547` `num2 += CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.ProjectTemperatureBaseLine, cal...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:551` `num += CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.NonProjectTemperatureBaseLine, calcData.Proje...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:552` `list.Add(CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.NonProjectTemperatureBaseLine, calcData.Pro...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:554` `double num3 = (num + num2) * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:559` `num5 += CalcRo(daysHours[l].Temp, daysHours[l].Humidity) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity) - CalcRo(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.NonProjectTemperatureBaseLine, calcData.Proj...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:563` `int index2 = ((m < daysHours.Count) ? m : 0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:564` `num4 += CalcRo(daysHours[index2].Temp, daysHours[index2].Humidity) * CalcAirX(daysHours[index2].Temp, daysHours[index2].Humidity) - CalcRo(calcData.ProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.ProjectTemperatureBaseLine,...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:568` `num5 += CalcRo(daysHours[n].Temp, daysHours[n].Humidity) * CalcAirX(daysHours[n].Temp, daysHours[n].Humidity) - CalcRo(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.NonProjectTemperatureBaseLine, calcData.Proj...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:570` `double num6 = (num4 + num5) * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:575` `num8 += CalcRo(daysHours[num9].Temp, daysHours[num9].Humidity) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity) - CalcRo(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.NonProjectTemperatureBaseLine, c...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:579` `int index3 = ((num10 < daysHours.Count) ? num10 : 0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:580` `num7 += CalcRo(daysHours[index3].Temp, daysHours[index3].Humidity) * CalcAirX(daysHours[index3].Temp, daysHours[index3].Humidity) - CalcRo(calcData.ProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.ProjectTemperatureBaseLine,...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:584` `num8 += CalcRo(daysHours[num11].Temp, daysHours[num11].Humidity) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity) - CalcRo(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.NonProjectTemperatureBaseLin...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:586` `double num12 = (num7 + num8) * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:587` `double num13 = section.Area.HeatedVolume * calcData.InfiltracionBaseLine * (1.0 / section.Area.HeatedArea) * (num3 + num6 + num12) * 0.6947222222222222;`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsInfESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:596`
- Inputs: `Section section`, `CalculationData calcData`, `MonthlyDays month`, `ClimateZones climateZone`
- Outputs: returns `double`; writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:597`; `index (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:609`; `index2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:627`; `index3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:643`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:598`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:603`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:615`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:599`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:650`; `num13 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:651`; ... (+15)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:603` `num += CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:604` `list.Add(CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:609` `int index = ((j < daysHours.Count) ? j : 0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:610` `list.Add(CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.ProjectTemperatureESM, calcData.ProjectH...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:611` `num2 += CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.ProjectTemperatureESM, calcData.ProjectHu...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:615` `num += CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:616` `list.Add(CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:618` `double num3 = (num + num2) * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:623` `num5 += CalcRo(daysHours[l].Temp, daysHours[l].Humidity) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity) - CalcRo(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:627` `int index2 = ((m < daysHours.Count) ? m : 0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:628` `num4 += CalcRo(daysHours[index2].Temp, daysHours[index2].Humidity) * CalcAirX(daysHours[index2].Temp, daysHours[index2].Humidity) - CalcRo(calcData.ProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.ProjectTemperatureESM, calcData.Proje...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:632` `num5 += CalcRo(daysHours[n].Temp, daysHours[n].Humidity) * CalcAirX(daysHours[n].Temp, daysHours[n].Humidity) - CalcRo(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:634` `double num6 = (num4 + num5) * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:639` `num8 += CalcRo(daysHours[num9].Temp, daysHours[num9].Humidity) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity) - CalcRo(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.NonProjectTemperatureESM, calcData.Project...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:643` `int index3 = ((num10 < daysHours.Count) ? num10 : 0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:644` `num7 += CalcRo(daysHours[index3].Temp, daysHours[index3].Humidity) * CalcAirX(daysHours[index3].Temp, daysHours[index3].Humidity) - CalcRo(calcData.ProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.ProjectTemperatureESM, calcData.Proje...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:648` `num8 += CalcRo(daysHours[num11].Temp, daysHours[num11].Humidity) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity) - CalcRo(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.NonProjectTemperatureESM, calcData.Pro...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:650` `double num12 = (num7 + num8) * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:651` `double num13 = section.Area.HeatedVolume * calcData.InfiltracionESM * (1.0 / section.Area.HeatedArea) * (num3 + num6 + num12) * 0.6947222222222222;`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsVentRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:660`
- Inputs: `Section section`, `CalculationData calcData`, `MonthlyDays month`, `ClimateZones climateZone`, `CalculationData ventCool`
- Outputs: returns `double`; writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:661`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:670`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:662`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:708`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:709`; `num13 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:713`; `num13 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:710`; `num15 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:715`; `num15 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:716`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:666`; ... (+16)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:666` `num2 += ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(daysHours[i].Temp) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:670` `num += ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(daysHours[j].Temp) * CalcAirX(daysHours[j].Temp, daysHours[j].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:674` `num2 += ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(daysHours[k].Temp) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:676` `double num3 = (num + num2) * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:677` `num3 = ((double.IsNaN(num3) || double.IsInfinity(num3)) ? 0.0 : num3);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:682` `num5 += ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(daysHours[l].Temp) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:686` `num4 += ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(daysHours[m].Temp) * CalcAirX(daysHours[m].Temp, daysHours[m].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:690` `num5 += ventCool.DebitRef1 * (ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(daysHours[n].Temp) * CalcAirX(daysHours[n].Temp, daysHours[n].Humidity))) * 0.6947222...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:692` `double num6 = (num4 + num5) * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:693` `num6 = ((double.IsNaN(num6) || double.IsInfinity(num6)) ? 0.0 : num6);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:698` `num8 += ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(daysHours[num9].Temp) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:702` `num7 += ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(daysHours[num10].Temp) * CalcAirX(daysHours[num10].Temp, daysHours[num10].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:706` `num8 += ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(daysHours[num11].Temp) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:708` `double num12 = (num7 + num8) * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:709` `num12 = ((double.IsNaN(num12) || double.IsInfinity(num12)) ? 0.0 : num12);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:713` `num13 += ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(calcData.NonProjectTemperatureRef1) * CalcAirX(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidit...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:715` `double num15 = num13 * (double)month.Holydays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:716` `num15 = ((double.IsNaN(num15) || double.IsInfinity(num15)) ? 0.0 : num15);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:717` `return num3 + num6 + num12 + num15;`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsVentRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:721`
- Inputs: `Section section`, `CalculationData calcData`, `MonthlyDays month`, `ClimateZones climateZone`, `CalculationData ventCool`
- Outputs: returns `double`; writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:722`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:731`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:723`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:769`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:770`; `num13 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:774`; `num13 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:771`; `num15 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:776`; `num15 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:777`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:727`; ... (+16)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:727` `num2 += ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(daysHours[i].Temp) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:731` `num += ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(daysHours[j].Temp) * CalcAirX(daysHours[j].Temp, daysHours[j].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:735` `num2 += ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(daysHours[k].Temp) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:737` `double num3 = (num + num2) * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:738` `num3 = ((double.IsNaN(num3) || double.IsInfinity(num3)) ? 0.0 : num3);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:743` `num5 += ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(daysHours[l].Temp) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:747` `num4 += ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(daysHours[m].Temp) * CalcAirX(daysHours[m].Temp, daysHours[m].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:751` `num5 += ventCool.DebitRef2 * (ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(daysHours[n].Temp) * CalcAirX(daysHours[n].Temp, daysHours[n].Humidity))) * 0.6947222...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:753` `double num6 = (num4 + num5) * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:754` `num6 = ((double.IsNaN(num6) || double.IsInfinity(num6)) ? 0.0 : num6);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:759` `num8 += ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(daysHours[num9].Temp) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:763` `num7 += ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(daysHours[num10].Temp) * CalcAirX(daysHours[num10].Temp, daysHours[num10].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:767` `num8 += ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(daysHours[num11].Temp) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:769` `double num12 = (num7 + num8) * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:770` `num12 = ((double.IsNaN(num12) || double.IsInfinity(num12)) ? 0.0 : num12);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:774` `num13 += ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(calcData.NonProjectTemperatureRef2) * CalcAirX(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidit...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:776` `double num15 = num13 * (double)month.Holydays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:777` `num15 = ((double.IsNaN(num15) || double.IsInfinity(num15)) ? 0.0 : num15);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:778` `return num3 + num6 + num12 + num15;`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsVent
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:782`
- Inputs: `Section section`, `CalculationData calcData`, `MonthlyDays month`, `ClimateZones climateZone`, `CalculationData ventCool`
- Outputs: returns `double`; writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:783`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:792`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:784`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:830`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:831`; `num13 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:835`; `num13 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:832`; `num15 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:837`; `num15 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:838`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:788`; ... (+16)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:788` `num2 += ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(daysHours[i].Temp) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:792` `num += ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(daysHours[j].Temp) * CalcAirX(daysHours[j].Temp, daysHours[j].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:796` `num2 += ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(daysHours[k].Temp) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:798` `double num3 = (num + num2) * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:799` `num3 = ((double.IsNaN(num3) || double.IsInfinity(num3)) ? 0.0 : num3);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:804` `num5 += ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(daysHours[l].Temp) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:808` `num4 += ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(daysHours[m].Temp) * CalcAirX(daysHours[m].Temp, daysHours[m].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:812` `num5 += ventCool.DebitActual * (ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(daysHours[n].Temp) * CalcAirX(daysHours[n].Temp, daysHours[n].Humidity))) *...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:814` `double num6 = (num4 + num5) * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:815` `num6 = ((double.IsNaN(num6) || double.IsInfinity(num6)) ? 0.0 : num6);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:820` `num8 += ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(daysHours[num9].Temp) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity)) * 0.6947222222222...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:824` `num7 += ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(daysHours[num10].Temp) * CalcAirX(daysHours[num10].Temp, daysHours[num10].Humidity)) * 0.6947222222...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:828` `num8 += ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(daysHours[num11].Temp) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity)) * 0.6947222222...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:830` `double num12 = (num7 + num8) * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:831` `num12 = ((double.IsNaN(num12) || double.IsInfinity(num12)) ? 0.0 : num12);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:835` `num13 += ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(calcData.NonProjectTemperatureActual) * CalcAirX(calcData.NonProjectTemperatureActual, calcData.Pr...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:837` `double num15 = num13 * (double)month.Holydays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:838` `num15 = ((double.IsNaN(num15) || double.IsInfinity(num15)) ? 0.0 : num15);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:839` `return num3 + num6 + num12 + num15;`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsVentBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:843`
- Inputs: `Section section`, `CalculationData calcData`, `MonthlyDays month`, `ClimateZones climateZone`, `CalculationData ventCool`
- Outputs: returns `double`; writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:844`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:853`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:845`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:891`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:892`; `num13 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:896`; `num13 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:893`; `num15 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:898`; `num15 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:899`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:849`; ... (+16)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:849` `num2 += ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(daysHours[i].Temp) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:853` `num += ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(daysHours[j].Temp) * CalcAirX(daysHours[j].Temp, daysHours[j].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:857` `num2 += ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(daysHours[k].Temp) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:859` `double num3 = (num + num2) * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:860` `num3 = ((double.IsNaN(num3) || double.IsInfinity(num3)) ? 0.0 : num3);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:865` `num5 += ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(daysHours[l].Temp) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:869` `num4 += ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(daysHours[m].Temp) * CalcAirX(daysHours[m].Temp, daysHours[m].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:873` `num5 += ventCool.DebitBaseLine * (ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(daysHours[n].Temp) * CalcAirX(daysHours[n].Temp, daysHours[n].Hum...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:875` `double num6 = (num4 + num5) * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:876` `num6 = ((double.IsNaN(num6) || double.IsInfinity(num6)) ? 0.0 : num6);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:881` `num8 += ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(daysHours[num9].Temp) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity)) * 0.69472...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:885` `num7 += ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(daysHours[num10].Temp) * CalcAirX(daysHours[num10].Temp, daysHours[num10].Humidity)) * 0.69...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:889` `num8 += ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(daysHours[num11].Temp) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity)) * 0.69...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:891` `double num12 = (num7 + num8) * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:892` `num12 = ((double.IsNaN(num12) || double.IsInfinity(num12)) ? 0.0 : num12);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:896` `num13 += ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(calcData.NonProjectTemperatureBaseLine) * CalcAirX(calcData.NonProjectTemperatureBaseLine,...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:898` `double num15 = num13 * (double)month.Holydays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:899` `num15 = ((double.IsNaN(num15) || double.IsInfinity(num15)) ? 0.0 : num15);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:900` `return num3 + num6 + num12 + num15;`

### HeatingAndCoolingResultCalc.CalculateLatentHeatsVentESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:904`
- Inputs: `Section section`, `CalculationData calcData`, `MonthlyDays month`, `ClimateZones climateZone`, `CalculationData ventCool`
- Outputs: returns `double`; writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:905`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:914`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:906`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:952`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:953`; `num13 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:957`; `num13 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:954`; `num15 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:959`; `num15 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:960`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:910`; ... (+16)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`, `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:910` `num2 += ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(daysHours[i].Temp) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:914` `num += ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(daysHours[j].Temp) * CalcAirX(daysHours[j].Temp, daysHours[j].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:918` `num2 += ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(daysHours[k].Temp) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:920` `double num3 = (num + num2) * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:921` `num3 = ((double.IsNaN(num3) || double.IsInfinity(num3)) ? 0.0 : num3);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:926` `num5 += ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(daysHours[l].Temp) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:930` `num4 += ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(daysHours[m].Temp) * CalcAirX(daysHours[m].Temp, daysHours[m].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:934` `num5 += ventCool.DebitESM * (ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(daysHours[n].Temp) * CalcAirX(daysHours[n].Temp, daysHours[n].Humidity))) * 0.694722222222...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:936` `double num6 = (num4 + num5) * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:937` `num6 = ((double.IsNaN(num6) || double.IsInfinity(num6)) ? 0.0 : num6);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:942` `num8 += ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(daysHours[num9].Temp) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:946` `num7 += ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(daysHours[num10].Temp) * CalcAirX(daysHours[num10].Temp, daysHours[num10].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:950` `num8 += ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(daysHours[num11].Temp) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity)) * 0.6947222222222222;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:952` `double num12 = (num7 + num8) * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:953` `num12 = ((double.IsNaN(num12) || double.IsInfinity(num12)) ? 0.0 : num12);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:957` `num13 += ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(calcData.NonProjectTemperatureESM) * CalcAirX(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM))...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:959` `double num15 = num13 * (double)month.Holydays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:960` `num15 = ((double.IsNaN(num15) || double.IsInfinity(num15)) ? 0.0 : num15);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:961` `return num3 + num6 + num12 + num15;`

### HeatingAndCoolingResultCalc.CalculateQgainRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1277`
- Inputs: `Section section`, `ClimateZones climateZone`, `MonthlyDays month`, `CalculationData lightsAndDevicesCalculationData`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1278`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1279`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1280`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateQintRef1`, `HeatingAndCoolingResultCalc.CalculateQoccupantsBaseLine`, `HeatingAndCoolingResultCalc.CalculateQsolRef1`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1281` `return num + num2 + num3;`

### HeatingAndCoolingResultCalc.CalculateQgainRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1285`
- Inputs: `Section section`, `ClimateZones climateZone`, `MonthlyDays month`, `CalculationData lightsAndDevicesCalculationData`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1286`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1287`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1288`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateQintRef2`, `HeatingAndCoolingResultCalc.CalculateQoccupantsBaseLine`, `HeatingAndCoolingResultCalc.CalculateQsolRef2`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1289` `return num + num2 + num3;`

### HeatingAndCoolingResultCalc.CalculateQgain
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1293`
- Inputs: `Section section`, `ClimateZones climateZone`, `MonthlyDays month`, `CalculationData lightsAndDevicesCalculationData`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1294`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1295`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1296`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateQint`, `HeatingAndCoolingResultCalc.CalculateQoccupants`, `HeatingAndCoolingResultCalc.CalculateQsol`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1297` `return num + num2 + num3;`

### HeatingAndCoolingResultCalc.CalculateQgainBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1301`
- Inputs: `Section section`, `ClimateZones climateZone`, `MonthlyDays month`, `CalculationData lightsAndDevicesCalculationData`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1302`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1303`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1304`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateQintBaseLine`, `HeatingAndCoolingResultCalc.CalculateQoccupantsBaseLine`, `HeatingAndCoolingResultCalc.CalculateQsolBaseLine`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1305` `return num + num2 + num3;`

### HeatingAndCoolingResultCalc.CalculateQgainESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1309`
- Inputs: `Section section`, `ClimateZones climateZone`, `MonthlyDays month`, `CalculationData lightsAndDevicesCalculationData`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1310`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1311`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1312`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateQintESM`, `HeatingAndCoolingResultCalc.CalculateQoccupantsESM`, `HeatingAndCoolingResultCalc.CalculateQsolESM`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1313` `return num + num2 + num3;`

### HeatingAndCoolingResultCalc.CalculateQintRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1317`
- Inputs: `CalculationData lightsAndDevicesCalculationData`, `MonthlyDays month`, `double area`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1318`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1319`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1318` `double num = lightsAndDevicesCalculationData.Lights.Cooling.PowerRef1 * (lightsAndDevicesCalculationData.Lights.Cooling.WorkScheduleRef1 * month.Weeks) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1319` `double num2 = lightsAndDevicesCalculationData.BalancedDevices.Cooling.PowerRef1 * (lightsAndDevicesCalculationData.BalancedDevices.Cooling.WorkScheduleRef1 * month.Weeks) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1320` `return num * area + num2 * area;`

### HeatingAndCoolingResultCalc.CalculateQintRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1324`
- Inputs: `CalculationData lightsAndDevicesCalculationData`, `MonthlyDays month`, `double area`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1325`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1326`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1325` `double num = lightsAndDevicesCalculationData.Lights.Cooling.PowerRef2 * (lightsAndDevicesCalculationData.Lights.Cooling.WorkScheduleRef2 * month.Weeks) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1326` `double num2 = lightsAndDevicesCalculationData.BalancedDevices.Cooling.PowerRef2 * (lightsAndDevicesCalculationData.BalancedDevices.Cooling.WorkScheduleRef2 * month.Weeks) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1327` `return num * area + num2 * area;`

### HeatingAndCoolingResultCalc.CalculateQint
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1331`
- Inputs: `CalculationData lightsAndDevicesCalculationData`, `MonthlyDays month`, `double area`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1332`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1333`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1332` `double num = (lightsAndDevicesCalculationData.Lights.ByMonths ? (CalcAvgMonthPower(lightsAndDevicesCalculationData.Lights.Actual, month) * (weekRegime * month.Weeks) / 1000.0) : (lightsAndDevicesCalculationData.Lights.Cooling.PowerActual * (lightsAndDevices...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1333` `double num2 = (lightsAndDevicesCalculationData.BalancedDevices.ByMonths ? (CalcAvgMonthPower(lightsAndDevicesCalculationData.BalancedDevices.Actual, month) * (weekRegime * month.Weeks) / 1000.0) : (lightsAndDevicesCalculationData.BalancedDevices.Cooling.Pow...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1334` `return num * area + num2 * area;`

### HeatingAndCoolingResultCalc.CalculateQintBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1338`
- Inputs: `CalculationData lightsAndDevicesCalculationData`, `MonthlyDays month`, `double area`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1339`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1340`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1339` `double num = (lightsAndDevicesCalculationData.Lights.ByMonths ? (CalcAvgMonthPower(lightsAndDevicesCalculationData.Lights.BaseLine, month) * (weekRegime * month.Weeks) / 1000.0) : (lightsAndDevicesCalculationData.Lights.Cooling.PowerBaseLine * (lightsAndDev...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1340` `double num2 = (lightsAndDevicesCalculationData.BalancedDevices.ByMonths ? (CalcAvgMonthPower(lightsAndDevicesCalculationData.BalancedDevices.BaseLine, month) * (weekRegime * month.Weeks) / 1000.0) : (lightsAndDevicesCalculationData.BalancedDevices.Cooling.P...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1341` `return num * area + num2 * area;`

### HeatingAndCoolingResultCalc.CalculateQintESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1345`
- Inputs: `CalculationData lightsAndDevicesCalculationData`, `MonthlyDays month`, `double area`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1346`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1347`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1346` `double num = (lightsAndDevicesCalculationData.Lights.ByMonths ? (CalcAvgMonthPower(lightsAndDevicesCalculationData.Lights.Esm, month) * (weekRegime * month.Weeks) / 1000.0) : (lightsAndDevicesCalculationData.Lights.Cooling.PowerESM * (lightsAndDevicesCalcul...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1347` `double num2 = (lightsAndDevicesCalculationData.BalancedDevices.ByMonths ? (CalcAvgMonthPower(lightsAndDevicesCalculationData.BalancedDevices.Esm, month) * (weekRegime * month.Weeks) / 1000.0) : (lightsAndDevicesCalculationData.BalancedDevices.Cooling.PowerE...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1348` `return num * area + num2 * area;`

### HeatingAndCoolingResultCalc.CalculateQoccupants
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1352`
- Inputs: `Section section`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1353`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1354`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateOccupantshours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1354` `double num2 = section.Area.MetabolicHeat * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1355` `return num2 * section.Area.HeatedArea;`

### HeatingAndCoolingResultCalc.CalculateQoccupantsBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1359`
- Inputs: `Section section`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1360`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1361`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateOccupantshoursBaseLine`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1361` `double num2 = section.Area.MetabolicHeat * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1362` `return num2 * section.Area.HeatedArea;`

### HeatingAndCoolingResultCalc.CalculateQoccupantsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1366`
- Inputs: `Section section`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1367`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1368`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateOccupantshoursESM`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1368` `double num2 = section.Area.MetabolicHeat * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1369` `return num2 * section.Area.HeatedArea;`

### HeatingAndCoolingResultCalc.CalculateQLatentOccupantsRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1373`
- Inputs: `Section section`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1374`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1375`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateOccupantshours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1375` `double num2 = section.Area.LatentMetabolicHeat * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1376` `return num2 * section.Area.HeatedArea;`

### HeatingAndCoolingResultCalc.CalculateQLatentOccupantsRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1380`
- Inputs: `Section section`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1381`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1382`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateOccupantshours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1382` `double num2 = section.Area.LatentMetabolicHeat * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1383` `return num2 * section.Area.HeatedArea;`

### HeatingAndCoolingResultCalc.CalculateQLatentOccupants
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1387`
- Inputs: `Section section`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1388`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1389`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateOccupantshours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1389` `double num2 = section.Area.LatentMetabolicHeat * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1390` `return num2 * section.Area.HeatedArea;`

### HeatingAndCoolingResultCalc.CalculateQLatentOccupantsBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1394`
- Inputs: `Section section`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1395`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1396`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateOccupantshoursBaseLine`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1396` `double num2 = section.Area.LatentMetabolicHeat * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1397` `return num2 * section.Area.HeatedArea;`

### HeatingAndCoolingResultCalc.CalculateQLatentOccupantsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1401`
- Inputs: `Section section`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1402`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1403`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateOccupantshoursESM`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1403` `double num2 = section.Area.LatentMetabolicHeat * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1404` `return num2 * section.Area.HeatedArea;`

### HeatingAndCoolingResultCalc.CalculateQsolRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1776`
- Inputs: `Section section`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1777`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1778`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1779`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1780`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1781`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1782`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1783`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1784`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1785`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsol`, `HeatingAndCoolingResultCalc.CalculateTrasparentFsol`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1777` `int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1778` `num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1779` `num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1780` `int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1781` `num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1782` `num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1783` `num2 = month.Holydays * 24 + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1786` `return (num4 + num3) * (double)(num + num2) / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateQsolRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1790`
- Inputs: `Section section`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1791`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1792`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1793`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1794`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1795`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1796`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1797`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1798`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1799`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsol`, `HeatingAndCoolingResultCalc.CalculateTrasparentFsol`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1791` `int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1792` `num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1793` `num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1794` `int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1795` `num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1796` `num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1797` `num2 = month.Holydays * 24 + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1800` `return (num4 + num3) * (double)(num + num2) / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateQsol
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1804`
- Inputs: `Section section`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1805`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1806`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1807`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1808`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1809`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1810`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1811`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1812`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1813`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsol`, `HeatingAndCoolingResultCalc.CalculateTrasparentFsol`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1805` `int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkCurrentEnd - section.CoolingSeasons.Cooling.WorkCurrentStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1806` `num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunCurrentEnd - section.CoolingSeasons.Cooling.SunCurrentStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1807` `num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatCurrentEnd - section.CoolingSeasons.Cooling.SatCurrentStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1808` `int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkCurrentEnd - section.CoolingSeasons.Cooling.WorkCurrentStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1809` `num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatCurrentEnd - section.CoolingSeasons.Cooling.SatCurrentStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1810` `num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunCurrentEnd - section.CoolingSeasons.Cooling.SunCurrentStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1811` `num2 = month.Holydays * 24 + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1814` `return (num4 + num3) * (double)(num + num2) / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateQsolBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1818`
- Inputs: `Section section`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1819`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1820`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1821`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1822`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1823`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1824`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1825`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1826`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1827`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsol`, `HeatingAndCoolingResultCalc.CalculateTrasparentFsol`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1819` `int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1820` `num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1821` `num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1822` `int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1823` `num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1824` `num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1825` `num2 = month.Holydays * 24 + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1828` `return (num4 + num3) * (double)(num + num2) / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateQsolESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1832`
- Inputs: `Section section`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1833`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1834`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1835`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1836`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1837`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1838`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1839`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1840`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1841`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsolEsm`, `HeatingAndCoolingResultCalc.CalculateTrasparentFsolEsm`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1833` `int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkEsmEnd - section.CoolingSeasons.Cooling.WorkEsmStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1834` `num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunEsmEnd - section.CoolingSeasons.Cooling.SunEsmStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1835` `num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatEsmEnd - section.CoolingSeasons.Cooling.SatEsmStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1836` `int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkEsmEnd - section.CoolingSeasons.Cooling.WorkEsmStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1837` `num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatEsmEnd - section.CoolingSeasons.Cooling.SatEsmStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1838` `num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunEsmEnd - section.CoolingSeasons.Cooling.SunEsmStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1839` `num2 = month.Holydays * 24 + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1842` `return (num4 + num3) * (double)(num + num2) / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHinfRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1876`
- Inputs: `Section section`, `CalculationData calcData`
- Outputs: returns `double`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1877` `return section.Area.HeatedVolume * calcData.InfiltracionRef1 * 0.34;`

### HeatingAndCoolingResultCalc.CalculateHinfRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1881`
- Inputs: `Section section`, `CalculationData calcData`
- Outputs: returns `double`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1882` `return section.Area.HeatedVolume * calcData.InfiltracionRef2 * 0.34;`

### HeatingAndCoolingResultCalc.CalculateHinf
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1886`
- Inputs: `Section section`, `CalculationData calcData`
- Outputs: returns `double`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1887` `return section.Area.HeatedVolume * calcData.InfiltracionActual * 0.34;`

### HeatingAndCoolingResultCalc.CalculateHinfBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1891`
- Inputs: `Section section`, `CalculationData calcData`
- Outputs: returns `double`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1892` `return section.Area.HeatedVolume * calcData.InfiltracionBaseLine * 0.34;`

### HeatingAndCoolingResultCalc.CalculateHinfESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1896`
- Inputs: `Section section`, `CalculationData calcData`
- Outputs: returns `double`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1897` `return section.Area.HeatedVolume * calcData.InfiltracionESM * 0.34;`

### HeatingAndCoolingResultCalc.CalculateCoolingQtrRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1901`
- Inputs: `CalculationData calcData`, `Section section`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `averageInnerCoolTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1903`; `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1902`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1904`; `section.Test.ParameterHtr (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1905`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingRef1`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingRef1`, `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempRef1`, `HeatingAndCoolingResultCalc.CalculateCoolingHtr`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1906` `return num * (CalcAvgProjectTempCoolingRef1(section, avgTemp, calcData, month) + CalcAvgNonProjectTempCoolingRef1(section, avgTemp, calcData, month)) / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingQtrRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1910`
- Inputs: `CalculationData calcData`, `Section section`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `averageInnerCoolTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1912`; `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1911`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1913`; `section.Test.ParameterHtr (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1914`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingRef2`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingRef2`, `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempRef2`, `HeatingAndCoolingResultCalc.CalculateCoolingHtr`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1915` `return num * (CalcAvgProjectTempCoolingRef2(section, avgTemp, calcData, month) + CalcAvgNonProjectTempCoolingRef2(section, avgTemp, calcData, month)) / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingQtr
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1919`
- Inputs: `CalculationData calcData`, `Section section`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `averageInnerCoolTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1921`; `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1920`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1922`; `section.Test.ParameterHtr (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1923`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCooling`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempCooling`, `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempCurrent`, `HeatingAndCoolingResultCalc.CalculateCoolingHtr`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1924` `return num * (CalcAvgProjectTempCooling(section, avgTemp, calcData, month) + CalcAvgNonProjectTempCooling(section, avgTemp, calcData, month)) / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingQtrBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1928`
- Inputs: `CalculationData calculationData`, `Section section`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `averageInnerCoolTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1930`; `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1929`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1931`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingBaseLine`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingBaseLine`, `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempBaseLine`, `HeatingAndCoolingResultCalc.CalculateCoolingHtr`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1932` `return num * (CalcAvgProjectTempCoolingBaseLine(section, avgTemp, calculationData, month) + CalcAvgNonProjectTempCoolingBaseLine(section, avgTemp, calculationData, month)) / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingQtrESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1936`
- Inputs: `CalculationData calculationData`, `Section section`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `averageInnerCoolTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1938`; `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1937`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1939`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingESM`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingESM`, `HeatingAndCoolingResultCalc.CalculateAverageCoolingTempESM`, `HeatingAndCoolingResultCalc.CalculateCoolingHtrESM`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1940` `return num * (CalcAvgProjectTempCoolingESM(section, avgTemp, calculationData, month) + CalcAvgNonProjectTempCoolingESM(section, avgTemp, calculationData, month)) / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingHtr
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1944`
- Inputs: `Section section`, `double averageMontlyTemp`, `double averageInnerCoolTemp`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1945`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1946`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1947`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1948`; `num5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1949`; `num6 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1950`; `section.Test.ParameterHd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1952`; `section.Test.ParameterHg (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1953`; `section.Test.ParameterHu (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1951`
- Internal calls: `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2Cooling`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3Cooling`, `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHgCurrent`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1Cooling`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1948` `double num4 = num + num2 + num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1951` `section.Test.ParameterHu = num + num2 + num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1954` `return num5 + num6 + num4;`

### HeatingAndCoolingResultCalc.CalculateCoolingHtrESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1958`
- Inputs: `Section section`, `double averageMontlyTemp`, `double averageInnerCoolTemp`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1959`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1960`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1961`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1962`; `num5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1963`; `num6 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1964`; `section.Test.ParameterHd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1966`; `section.Test.ParameterHg (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1967`; `section.Test.ParameterHu (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1965`
- Internal calls: `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2Cooling`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3Cooling`, `HeatingAndCoolingResultCalc.CalculateParameterHdEsm`, `HeatingAndCoolingResultCalc.CalculateParameterHgEsm`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1CoolingESM`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1962` `double num4 = num + num2 + num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1965` `section.Test.ParameterHu = num + num2 + num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1968` `return num5 + num6 + num4;`

### HeatingAndCoolingResultCalc.CalculateAverageCoolingTempRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1972`
- Inputs: `Section section`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `nonProjectTemperatureRef (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1981`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1973`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1974`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1975`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1977`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1978`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1979`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1980`; `projectTemperatureRef (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1976`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1973` `int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1974` `num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1975` `num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1977` `int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1978` `num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1979` `num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1980` `num2 = month.Holydays * 24 + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1982` `return ((double)num * projectTemperatureRef + (double)num2 * nonProjectTemperatureRef) / (double)(num + num2);`

### HeatingAndCoolingResultCalc.CalculateAverageCoolingTempRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1986`
- Inputs: `Section section`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `nonProjectTemperatureRef (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1995`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1987`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1988`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1989`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1991`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1992`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1993`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1994`; `projectTemperatureRef (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1990`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1987` `int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1988` `num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1989` `num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1991` `int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1992` `num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1993` `num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1994` `num2 = month.Holydays * 24 + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1996` `return ((double)num * projectTemperatureRef + (double)num2 * nonProjectTemperatureRef) / (double)(num + num2);`

### HeatingAndCoolingResultCalc.CalculateAverageCoolingTempCurrent
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2000`
- Inputs: `Section section`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `nonProjectTemperatureActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2009`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2001`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2002`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2003`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2005`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2006`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2007`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2008`; `projectTemperatureActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2004`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2001` `int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkCurrentEnd - section.CoolingSeasons.Cooling.WorkCurrentStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2002` `num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunCurrentEnd - section.CoolingSeasons.Cooling.SunCurrentStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2003` `num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatCurrentEnd - section.CoolingSeasons.Cooling.SatCurrentStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2005` `int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkCurrentEnd - section.CoolingSeasons.Cooling.WorkCurrentStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2006` `num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatCurrentEnd - section.CoolingSeasons.Cooling.SatCurrentStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2007` `num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunCurrentEnd - section.CoolingSeasons.Cooling.SunCurrentStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2008` `num2 = month.Holydays * 24 + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2010` `return ((double)num * projectTemperatureActual + (double)num2 * nonProjectTemperatureActual) / (double)(num + num2);`

### HeatingAndCoolingResultCalc.CalculateAverageCoolingTempBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2014`
- Inputs: `Section section`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `nonProjectTemperatureBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2023`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2015`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2016`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2017`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2019`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2020`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2021`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2022`; `projectTemperatureBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2018`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2015` `int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2016` `num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2017` `num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2019` `int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2020` `num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2021` `num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2022` `num2 = month.Holydays * 24 + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2024` `return ((double)num * projectTemperatureBaseLine + (double)num2 * nonProjectTemperatureBaseLine) / (double)(num + num2);`

### HeatingAndCoolingResultCalc.CalculateAverageCoolingTempESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2028`
- Inputs: `Section section`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `nonProjectTemperatureESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2037`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2029`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2030`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2031`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2033`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2034`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2035`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2036`; `projectTemperatureESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2032`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2029` `int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkEsmEnd - section.CoolingSeasons.Cooling.WorkEsmStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2030` `num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunEsmEnd - section.CoolingSeasons.Cooling.SunEsmStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2031` `num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatEsmEnd - section.CoolingSeasons.Cooling.SatEsmStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2033` `int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkEsmEnd - section.CoolingSeasons.Cooling.WorkEsmStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2034` `num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatEsmEnd - section.CoolingSeasons.Cooling.SatEsmStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2035` `num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunEsmEnd - section.CoolingSeasons.Cooling.SunEsmStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2036` `num2 = month.Holydays * 24 + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2038` `return ((double)num * projectTemperatureESM + (double)num2 * nonProjectTemperatureESM) / (double)(num + num2);`

### HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2042`
- Inputs: `Section section`, `double averageMontlyTemp`, `CalculationData calcData`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2043`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2044`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2045`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2043` `int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2044` `int num2 = month.Sundays * (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2045` `int num3 = month.Saturdays * (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2046` `return (calcData.ProjectTemperatureRef1 - averageMontlyTemp) * (double)(num + num3 + num2);`

### HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2050`
- Inputs: `Section section`, `double averageMontlyTemp`, `CalculationData calcData`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2051`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2052`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2053`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2051` `int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2052` `int num2 = month.Sundays * (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2053` `int num3 = month.Saturdays * (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2054` `return (calcData.ProjectTemperatureRef2 - averageMontlyTemp) * (double)(num + num3 + num2);`

### HeatingAndCoolingResultCalc.CalcAvgProjectTempCooling
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2058`
- Inputs: `Section section`, `double averageMontlyTemp`, `CalculationData calcData`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2059`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2060`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2061`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2059` `int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkCurrentEnd - section.CoolingSeasons.Cooling.WorkCurrentStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2060` `int num2 = month.Sundays * (section.CoolingSeasons.Cooling.SunCurrentEnd - section.CoolingSeasons.Cooling.SunCurrentStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2061` `int num3 = month.Saturdays * (section.CoolingSeasons.Cooling.SatCurrentEnd - section.CoolingSeasons.Cooling.SatCurrentStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2062` `return (calcData.ProjectTemperatureActual - averageMontlyTemp) * (double)(num + num3 + num2);`

### HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2066`
- Inputs: `Section section`, `double averageMontlyTemp`, `CalculationData calcData`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2067`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2068`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2069`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2067` `int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2068` `int num2 = month.Sundays * (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2069` `int num3 = month.Saturdays * (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2070` `return (calcData.ProjectTemperatureBaseLine - averageMontlyTemp) * (double)(num + num3 + num2);`

### HeatingAndCoolingResultCalc.CalcAvgProjectTempCoolingESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2074`
- Inputs: `Section section`, `double averageMontlyTemp`, `CalculationData calcData`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2075`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2076`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2077`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2075` `int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkEsmEnd - section.CoolingSeasons.Cooling.WorkEsmStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2076` `int num2 = month.Sundays * (section.CoolingSeasons.Cooling.SunEsmEnd - section.CoolingSeasons.Cooling.SunEsmStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2077` `int num3 = month.Saturdays * (section.CoolingSeasons.Cooling.SatEsmEnd - section.CoolingSeasons.Cooling.SatEsmStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2078` `return (calcData.ProjectTemperatureESM - averageMontlyTemp) * (double)(num + num3 + num2);`

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2082`
- Inputs: `Section section`, `double averageMontlyTemp`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2083`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2084`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2085`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2086`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2083` `int num = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2084` `int num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2085` `int num3 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2086` `int num4 = month.Holydays * 24;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2087` `return (calculationData.NonProjectTemperatureRef1 - averageMontlyTemp) * (double)(num + num2 + num3 + num4);`

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2091`
- Inputs: `Section section`, `double averageMontlyTemp`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2092`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2093`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2094`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2095`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2092` `int num = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2093` `int num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2094` `int num3 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2095` `int num4 = month.Holydays * 24;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2096` `return (calculationData.NonProjectTemperatureRef2 - averageMontlyTemp) * (double)(num + num2 + num3 + num4);`

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCooling
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2100`
- Inputs: `Section section`, `double averageMontlyTemp`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2101`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2102`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2103`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2104`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2101` `int num = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkCurrentEnd - section.CoolingSeasons.Cooling.WorkCurrentStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2102` `int num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatCurrentEnd - section.CoolingSeasons.Cooling.SatCurrentStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2103` `int num3 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunCurrentEnd - section.CoolingSeasons.Cooling.SunCurrentStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2104` `int num4 = month.Holydays * 24;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2105` `return (calculationData.NonProjectTemperatureActual - averageMontlyTemp) * (double)(num + num2 + num3 + num4);`

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2109`
- Inputs: `Section section`, `double averageMontlyTemp`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2110`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2111`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2112`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2113`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2110` `int num = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2111` `int num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2112` `int num3 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2113` `int num4 = month.Holydays * 24;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2114` `return (calculationData.NonProjectTemperatureBaseLine - averageMontlyTemp) * (double)(num + num2 + num3 + num4);`

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTempCoolingESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2118`
- Inputs: `Section section`, `double averageMontlyTemp`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2119`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2120`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2121`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2122`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2119` `int num = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkEsmEnd - section.CoolingSeasons.Cooling.WorkEsmStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2120` `int num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatEsmEnd - section.CoolingSeasons.Cooling.SatEsmStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2121` `int num3 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunEsmEnd - section.CoolingSeasons.Cooling.SunEsmStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2122` `int num4 = month.Holydays * 24;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2123` `return (calculationData.NonProjectTemperatureESM - averageMontlyTemp) * (double)(num + num2 + num3 + num4);`

### HeatingAndCoolingResultCalc.SumWallDirecrionsHu1Cooling
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2127`
- Inputs: `Section section`, `double averageMontlyTemp`, `double averageInnerCoolTemp`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2128`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2129`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2130`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2131`; `num5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2132`; `num6 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2133`; `num7 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2134`; `num8 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2135`
- Internal calls: `HeatingAndCoolingResultCalc.CalcWallDirectionParameterHu1Cooling`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2136` `return num + num2 + num3 + num4 + num5 + num6 + num7 + num8;`

### HeatingAndCoolingResultCalc.SumWallDirecrionsHu1CoolingESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2140`
- Inputs: `Section section`, `double averageMontlyTemp`, `double averageInnerCoolTemp`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2141`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2142`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2143`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2144`; `num5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2145`; `num6 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2146`; `num7 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2147`; `num8 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2148`
- Internal calls: `HeatingAndCoolingResultCalc.CalcWallDirectionParameterHu1Cooling`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2149` `return num + num2 + num3 + num4 + num5 + num6 + num7 + num8;`

### HeatingAndCoolingResultCalc.CalcWallDirectionParameterHu1Cooling
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2153`
- Inputs: `Walls wall`, `double averageMontlyTemp`, `double averageInnerCoolTemp`
- Outputs: returns `double`; writes `innerA (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2159`; `innerA (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2163`; `innerA (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2167`; `innerA (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2171`; `innerA (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2175`; `innerA (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2179`; `innerU (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2160`; `innerU (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2164`; `innerU (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2168`; `innerU (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2172`; ... (+15)
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2154` `double num = averageInnerCoolTemp - averageMontlyTemp;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2161` `double num2 = averageInnerCoolTemp - (double)wall.InnerS1;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2162` `double num3 = innerA * innerU * num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2165` `num2 = averageInnerCoolTemp - (double)wall.InnerS2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2166` `double num4 = innerA * innerU * num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2169` `num2 = averageInnerCoolTemp - (double)wall.InnerS3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2170` `double num5 = innerA * innerU * num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2173` `num2 = averageInnerCoolTemp - (double)wall.InnerS4;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2174` `double num6 = innerA * innerU * num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2177` `num2 = averageInnerCoolTemp - (double)wall.InnerS5;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2178` `double num7 = innerA * innerU * num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2181` `num2 = averageInnerCoolTemp - (double)wall.InnerS6;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2182` `double num8 = innerA * innerU * num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2183` `return num3 + num4 + num5 + num6 + num7 + num8;`

### HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2Cooling
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2187`
- Inputs: `Roof roof`, `double averageMontlyTemp`, `double averageInnerCoolTemp`
- Outputs: returns `double`; writes `ceilingA (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2193`; `ceilingA (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2197`; `ceilingA (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2201`; `ceilingA (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2205`; `ceilingA (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2209`; `ceilingA (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2213`; `ceilingU (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2194`; `ceilingU (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2198`; `ceilingU (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2202`; `ceilingU (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2206`; ... (+15)
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2188` `double num = averageInnerCoolTemp - averageMontlyTemp;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2195` `double num2 = averageInnerCoolTemp - (double)roof.CeilingS1;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2196` `double num3 = ceilingA * ceilingU * num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2199` `num2 = averageInnerCoolTemp - (double)roof.CeilingS2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2200` `double num4 = ceilingA * ceilingU * num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2203` `num2 = averageInnerCoolTemp - (double)roof.CeilingS3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2204` `double num5 = ceilingA * ceilingU * num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2207` `num2 = averageInnerCoolTemp - (double)roof.CeilingS4;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2208` `double num6 = ceilingA * ceilingU * num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2211` `num2 = averageInnerCoolTemp - (double)roof.CeilingS5;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2212` `double num7 = ceilingA * ceilingU * num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2215` `num2 = averageInnerCoolTemp - (double)roof.CeilingS6;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2216` `double num8 = ceilingA * ceilingU * num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2217` `return num3 + num4 + num5 + num6 + num7 + num8;`

### HeatingAndCoolingResultCalc.CalcFloorsParameterHu3Cooling
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2221`
- Inputs: `Floor floor`, `double averageMontlyTemp`, `double averageInnerCoolTemp`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2222`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2229`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2233`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2237`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2241`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2245`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2249`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2230`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2234`; `num5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2238`; ... (+15)
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2222` `double num = averageInnerCoolTemp - averageMontlyTemp;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2229` `double num2 = averageInnerCoolTemp - (double)floor.OtherFloorS1;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2230` `double num3 = otherFloorA * otherFloorU * num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2233` `num2 = averageInnerCoolTemp - (double)floor.OtherFloorS2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2234` `double num4 = otherFloorA * otherFloorU * num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2237` `num2 = averageInnerCoolTemp - (double)floor.OtherFloorS3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2238` `double num5 = otherFloorA * otherFloorU * num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2241` `num2 = averageInnerCoolTemp - (double)floor.OtherFloorS4;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2242` `double num6 = otherFloorA * otherFloorU * num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2245` `num2 = averageInnerCoolTemp - (double)floor.OtherFloorS5;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2246` `double num7 = otherFloorA * otherFloorU * num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2249` `num2 = averageInnerCoolTemp - (double)floor.OtherFloorS4;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2250` `double num8 = otherFloorA * otherFloorU * num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2251` `return num3 + num4 + num5 + num6 + num7 + num8;`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2338`
- Inputs: `this HeatingCalculations calc`, `Section section`
- Outputs: writes `calc.FansAndPumps.CoolNeededEnergyRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2352`; `calc.FansAndPumps.OtherResultCoolingRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2361`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2340`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2343`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2344`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2346`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2342`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2347`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2350`; `num3 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2356`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursBaseLine`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2340` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2342` `double num2 = calc.FansAndPumps.VentilatorsCoolRef1 * weekCoolingVentilationHoursBaseLine * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2343` `num2 += calc.FansAndPumps.PumpVentilationCoolRef1 * weekCoolingVentilationHoursBaseLine * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2344` `num2 += calc.FansAndPumps.VentilatorsOutdoorAirCoolRef1 * weekCoolingVentilationHoursBaseLine * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2346` `num2 += calc.FansAndPumps.CoolingPumpRef1 * 24.0 * 7.0 * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2347` `num2 = num2 / calc.FansAndPumps.EnergyManagement2Ref2 * 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2354` `double num3 = calc.FansAndPumps.OtherCoolingVentilationRef1 * weekCoolingVentilationHoursBaseLine * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2356` `num3 += calc.FansAndPumps.OtherCoolingRef1 * weekCoolingVentilationHoursBaseLine * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2365`
- Inputs: `this HeatingCalculations calc`, `Section section`
- Outputs: writes `calc.FansAndPumps.CoolNeededEnergyRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2378`; `calc.FansAndPumps.OtherResultCoolingRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2387`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2367`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2370`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2371`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2372`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2369`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2373`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2376`; `num3 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2382`; ... (+6)
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursBaseLine`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2367` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2369` `double num2 = calc.FansAndPumps.VentilatorsCoolRef2 * weekCoolingVentilationHoursBaseLine * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2370` `num2 += calc.FansAndPumps.PumpVentilationCoolRef2 * weekCoolingVentilationHoursBaseLine * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2371` `num2 += calc.FansAndPumps.VentilatorsOutdoorAirCoolRef2 * weekCoolingVentilationHoursBaseLine * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2372` `num2 += calc.FansAndPumps.CoolingPumpRef2 * 24.0 * 7.0 * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2373` `num2 = num2 / calc.FansAndPumps.EnergyManagement2Ref2 * 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2380` `double num3 = calc.FansAndPumps.OtherCoolingVentilationRef2 * weekCoolingVentilationHoursBaseLine * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2382` `num3 += calc.FansAndPumps.OtherCoolingRef2 * weekCoolingVentilationHoursBaseLine * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2391`
- Inputs: `this HeatingCalculations calc`, `Section section`
- Outputs: writes `calc.FansAndPumps.CoolNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2404`; `calc.FansAndPumps.OtherResultCoolingActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2413`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2393`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2396`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2397`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2398`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2395`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2399`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2402`; `num3 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2408`; ... (+6)
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursActual`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursActual`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2393` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2395` `double num2 = calc.FansAndPumps.VentilatorsCoolActual * weekCoolingVentilationHoursActual * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2396` `num2 += calc.FansAndPumps.PumpVentilationCoolActual * weekCoolingVentilationHoursActual * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2397` `num2 += calc.FansAndPumps.VentilatorsOutdoorAirCoolActual * weekCoolingVentilationHoursActual * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2398` `num2 += calc.FansAndPumps.CoolingPumpActual * 24.0 * 7.0 * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2399` `num2 = num2 / calc.FansAndPumps.EnergyManagement2Actual * 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2406` `double num3 = calc.FansAndPumps.OtherCoolingVentilationActual * weekCoolingVentilationHoursActual * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2408` `num3 += calc.FansAndPumps.OtherCoolingActual * weekCoolingVentilationHoursActual * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2417`
- Inputs: `this HeatingCalculations calc`, `Section section`
- Outputs: writes `calc.FansAndPumps.CoolNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2430`; `calc.FansAndPumps.OtherResultCoolingBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2439`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2419`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2422`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2423`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2424`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2421`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2425`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2428`; `num3 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2434`; ... (+6)
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursBaseLine`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2419` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2421` `double num2 = calc.FansAndPumps.VentilatorsCoolBaseLine * weekCoolingVentilationHoursBaseLine * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2422` `num2 += calc.FansAndPumps.PumpVentilationCoolBaseLine * weekCoolingVentilationHoursBaseLine * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2423` `num2 += calc.FansAndPumps.VentilatorsOutdoorAirCoolBaseLine * weekCoolingVentilationHoursBaseLine * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2424` `num2 += calc.FansAndPumps.CoolingPumpBaseLine * 24.0 * 7.0 * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2425` `num2 = num2 / calc.FansAndPumps.EnergyManagement2BaseLine * 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2432` `double num3 = calc.FansAndPumps.OtherCoolingVentilationBaseLine * weekCoolingVentilationHoursBaseLine * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2434` `num3 += calc.FansAndPumps.OtherCoolingBaseLine * weekCoolingVentilationHoursBaseLine * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2443`
- Inputs: `this HeatingCalculations calc`, `Section section`
- Outputs: writes `calc.FansAndPumps.CoolNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2456`; `calc.FansAndPumps.OtherResultCoolingESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2465`; `calc.FansAndPumps.OtherResultCoolingSavings (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2466`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2445`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2448`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2449`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2450`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2447`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2451`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2454`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursEsm`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursEsm`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2445` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2447` `double num2 = calc.FansAndPumps.VentilatorsCoolESM * weekCoolingVentilationHoursEsm * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2448` `num2 += calc.FansAndPumps.PumpVentilationCoolESM * weekCoolingVentilationHoursEsm * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2449` `num2 += calc.FansAndPumps.VentilatorsOutdoorAirCoolESM * weekCoolingVentilationHoursEsm * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2450` `num2 += calc.FansAndPumps.CoolingPumpESM * 24.0 * 7.0 * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2451` `num2 = num2 / calc.FansAndPumps.EnergyManagement2ESM * 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2458` `double num3 = calc.FansAndPumps.OtherCoolingVentilationESM * weekCoolingVentilationHoursEsm * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2460` `num3 += calc.FansAndPumps.OtherCoolingESM * weekCoolingVentilationHoursEsm * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2466` `calc.FansAndPumps.OtherResultCoolingSavings = (calc.FansAndPumps.OtherResultCoolingBaseLine - calc.FansAndPumps.OtherResultCoolingESM).ToString("F3");`

### HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2512`
- Inputs: `Section section`
- Outputs: returns `double`; writes `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2514`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2513`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2513` `double num = 5.0 * section.CalcHours(section.CoolingSeasons.Cooling.WorkCurrentStart, section.CoolingSeasons.Cooling.WorkCurrentEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2514` `num += section.CalcHours(section.CoolingSeasons.Cooling.SunCurrentStart, section.CoolingSeasons.Cooling.SunCurrentEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2515` `return num + section.CalcHours(section.CoolingSeasons.Cooling.SatCurrentStart, section.CoolingSeasons.Cooling.SatCurrentEnd);`

### HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2519`
- Inputs: `Section section`
- Outputs: returns `double`; writes `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2521`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2520`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2520` `double num = 5.0 * section.CalcHours(section.CoolingSeasons.Cooling.WorkBaseStart, section.CoolingSeasons.Cooling.WorkBaseEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2521` `num += section.CalcHours(section.CoolingSeasons.Cooling.SunBaseStart, section.CoolingSeasons.Cooling.SunBaseEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2522` `return num + section.CalcHours(section.CoolingSeasons.Cooling.SatBaseStart, section.CoolingSeasons.Cooling.SatBaseEnd);`

### HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2526`
- Inputs: `Section section`
- Outputs: returns `double`; writes `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2528`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2527`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2527` `double num = 5.0 * section.CalcHours(section.CoolingSeasons.Cooling.WorkEsmStart, section.CoolingSeasons.Cooling.WorkEsmEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2528` `num += section.CalcHours(section.CoolingSeasons.Cooling.SunEsmStart, section.CoolingSeasons.Cooling.SunEsmEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2529` `return num + section.CalcHours(section.CoolingSeasons.Cooling.SatEsmStart, section.CoolingSeasons.Cooling.SatEsmEnd);`

### HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2533`
- Inputs: `Section section`
- Outputs: returns `double`; writes `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2535`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2534`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2534` `double num = 5.0 * section.CalcHours(section.CoolingSeasons.Ventilation.WorkCurrentStart, section.CoolingSeasons.Ventilation.WorkCurrentEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2535` `num += section.CalcHours(section.CoolingSeasons.Ventilation.SunCurrentStart, section.CoolingSeasons.Ventilation.SunCurrentEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2536` `return num + section.CalcHours(section.CoolingSeasons.Ventilation.SatCurrentStart, section.CoolingSeasons.Ventilation.SatCurrentEnd);`

### HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2540`
- Inputs: `Section section`
- Outputs: returns `double`; writes `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2542`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2541`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2541` `double num = 5.0 * section.CalcHours(section.CoolingSeasons.Ventilation.WorkBaseStart, section.CoolingSeasons.Ventilation.WorkBaseEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2542` `num += section.CalcHours(section.CoolingSeasons.Ventilation.SunBaseStart, section.CoolingSeasons.Ventilation.SunBaseEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2543` `return num + section.CalcHours(section.CoolingSeasons.Ventilation.SatBaseStart, section.CoolingSeasons.Ventilation.SatBaseEnd);`

### HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2547`
- Inputs: `Section section`
- Outputs: returns `double`; writes `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2549`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2548`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2548` `double num = 5.0 * section.CalcHours(section.CoolingSeasons.Ventilation.WorkEsmStart, section.CoolingSeasons.Ventilation.WorkEsmEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2549` `num += section.CalcHours(section.CoolingSeasons.Ventilation.SunEsmStart, section.CoolingSeasons.Ventilation.SunEsmEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2550` `return num + section.CalcHours(section.CoolingSeasons.Ventilation.SatEsmStart, section.CoolingSeasons.Ventilation.SatEsmEnd);`

### HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2997`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.ColdEfficiencyGeneratingRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3000`; `coolCalc.ColdEfficiencyGeneratingRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3003`; `coolCalc.ColdEfficiencyGeneratingRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3008`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3000` `coolCalc.ColdEfficiencyGeneratingRef1 = (coolCalc.ResultSourceEnergyRef1 * coolCalc.GeneratorColdEfficiency1Ref1 + coolCalc.ResultSourceEnergy2Ref1 * coolCalc.GeneratorColdEfficiency2Ref1) / (coolCalc.ResultSourceEnergyRef1 + coolCalc.ResultSourceEnergy2Ref1);`

### HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3013`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.ColdEfficiencyGeneratingRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3016`; `coolCalc.ColdEfficiencyGeneratingRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3019`; `coolCalc.ColdEfficiencyGeneratingRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3024`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3016` `coolCalc.ColdEfficiencyGeneratingRef2 = (coolCalc.ResultSourceEnergyRef2 * coolCalc.GeneratorColdEfficiency1Ref2 + coolCalc.ResultSourceEnergy2Ref2 * coolCalc.GeneratorColdEfficiency2Ref2) / (coolCalc.ResultSourceEnergyRef2 + coolCalc.ResultSourceEnergy2Ref2);`

### HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3029`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.ColdEfficiencyGeneratingActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3032`; `coolCalc.ColdEfficiencyGeneratingActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3035`; `coolCalc.ColdEfficiencyGeneratingActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3040`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3032` `coolCalc.ColdEfficiencyGeneratingActual = (coolCalc.ResultSourceEnergyActual * coolCalc.GeneratorColdEfficiency1Actual + coolCalc.ResultSourceEnergy2Actual * coolCalc.GeneratorColdEfficiency2Actual) / (coolCalc.ResultSourceEnergyActual + coolCalc.ResultSour...`

### HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3045`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.ColdEfficiencyGeneratingBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3048`; `coolCalc.ColdEfficiencyGeneratingBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3051`; `coolCalc.ColdEfficiencyGeneratingBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3056`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3048` `coolCalc.ColdEfficiencyGeneratingBaseLine = (coolCalc.ResultSourceEnergyBaseLine * coolCalc.GeneratorColdEfficiency1BaseLine + coolCalc.ResultSourceEnergy2BaseLine * coolCalc.GeneratorColdEfficiency2BaseLine) / (coolCalc.ResultSourceEnergyBaseLine + coolCal...`

### HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3061`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.ColdEfficiencyGeneratingESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3064`; `coolCalc.ColdEfficiencyGeneratingESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3067`; `coolCalc.ColdEfficiencyGeneratingESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3072`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3064` `coolCalc.ColdEfficiencyGeneratingESM = (coolCalc.ResultSourceEnergyESM * coolCalc.GeneratorColdEfficiency1ESM + coolCalc.ResultSourceEnergy2ESM * coolCalc.GeneratorColdEfficiency2ESM) / (coolCalc.ResultSourceEnergyESM + coolCalc.ResultSourceEnergy2ESM);`

### HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3077`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.HeatEfficiencyGeneratingRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3080`; `coolCalc.HeatEfficiencyGeneratingRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3083`; `coolCalc.HeatEfficiencyGeneratingRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3088`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3080` `coolCalc.HeatEfficiencyGeneratingRef1 = (coolCalc.ResultSourceEnergyRef1 * coolCalc.GeneratorColdEfficiency1Ref1 + coolCalc.ResultSourceEnergy2Ref1 * coolCalc.GeneratorColdEfficiency2Ref1) / (coolCalc.ResultSourceEnergyRef1 + coolCalc.ResultSourceEnergy2Ref1);`

### HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3093`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.HeatEfficiencyGeneratingRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3096`; `coolCalc.HeatEfficiencyGeneratingRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3099`; `coolCalc.HeatEfficiencyGeneratingRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3104`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3096` `coolCalc.HeatEfficiencyGeneratingRef2 = (coolCalc.ResultSourceEnergyRef2 * coolCalc.GeneratorColdEfficiency1Ref2 + coolCalc.ResultSourceEnergy2Ref2 * coolCalc.GeneratorColdEfficiency2Ref2) / (coolCalc.ResultSourceEnergyRef2 + coolCalc.ResultSourceEnergy2Ref2);`

### HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3109`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.HeatEfficiencyGeneratingActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3112`; `coolCalc.HeatEfficiencyGeneratingActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3115`; `coolCalc.HeatEfficiencyGeneratingActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3120`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3112` `coolCalc.HeatEfficiencyGeneratingActual = (coolCalc.ResultSourceEnergyActual * coolCalc.GeneratorColdEfficiency1Actual + coolCalc.ResultSourceEnergy2Actual * coolCalc.GeneratorColdEfficiency2Actual) / (coolCalc.ResultSourceEnergyActual + coolCalc.ResultSour...`

### HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3125`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.HeatEfficiencyGeneratingBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3128`; `coolCalc.HeatEfficiencyGeneratingBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3131`; `coolCalc.HeatEfficiencyGeneratingBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3136`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3128` `coolCalc.HeatEfficiencyGeneratingBaseLine = (coolCalc.ResultSourceEnergyBaseLine * coolCalc.GeneratorColdEfficiency1BaseLine + coolCalc.ResultSourceEnergy2BaseLine * coolCalc.GeneratorColdEfficiency2BaseLine) / (coolCalc.ResultSourceEnergyBaseLine + coolCal...`

### HeatingAndCoolingResultCalc.CalculateGeneratorCoolEfficiencyESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3141`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.HeatEfficiencyGeneratingESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3144`; `coolCalc.HeatEfficiencyGeneratingESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3147`; `coolCalc.HeatEfficiencyGeneratingESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3152`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3144` `coolCalc.HeatEfficiencyGeneratingESM = (coolCalc.ResultSourceEnergyESM * coolCalc.GeneratorColdEfficiency1ESM + coolCalc.ResultSourceEnergy2ESM * coolCalc.GeneratorColdEfficiency2ESM) / (coolCalc.ResultSourceEnergyESM + coolCalc.ResultSourceEnergy2ESM);`

### HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3157`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.ResultNeededEnergyRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3170`; `coolCalc.ResultSourceEnergy2Ref1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3165`; `coolCalc.ResultSourceEnergy2Ref1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3168`; `coolCalc.ResultSourceEnergyRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3159`; `coolCalc.ResultSourceEnergyRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3162`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3158`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3164`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3158` `double num = coolCalc.ResulNetEnergyRef1 * coolCalc.Part1Ref1 / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3159` `coolCalc.ResultSourceEnergyRef1 = num / (coolCalc.TransmitTempEfficiencyRef1 / 100.0 * (coolCalc.SupplyNetEfficiencyRef1 / 100.0) * (coolCalc.AutomaticRef1 / 100.0) * (coolCalc.EnergyManagementRef1 / 100.0) * (coolCalc.GeneratorColdEfficiency1Ref1 / 100.0));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3164` `double num2 = coolCalc.ResulNetEnergyRef1 * coolCalc.Part2Ref1 / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3165` `coolCalc.ResultSourceEnergy2Ref1 = num2 / (coolCalc.TransmitTempEfficiency2Ref1 / 100.0 * (coolCalc.SupplyNetEfficiency2Ref1 / 100.0) * (coolCalc.Automatic2Ref1 / 100.0) * (coolCalc.EnergyManagement2Ref1 / 100.0) * (coolCalc.GeneratorColdEfficiency2Ref1 / 1...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3170` `coolCalc.ResultNeededEnergyRef1 = coolCalc.ResultSourceEnergyRef1 + coolCalc.ResultSourceEnergy2Ref1;`

### HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3174`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.ResultNeededEnergyRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3187`; `coolCalc.ResultSourceEnergy2Ref2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3182`; `coolCalc.ResultSourceEnergy2Ref2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3185`; `coolCalc.ResultSourceEnergyRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3176`; `coolCalc.ResultSourceEnergyRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3179`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3175`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3181`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3175` `double num = coolCalc.ResulNetEnergyRef2 * coolCalc.Part1Ref2 / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3176` `coolCalc.ResultSourceEnergyRef2 = num / (coolCalc.TransmitTempEfficiencyRef2 / 100.0 * (coolCalc.SupplyNetEfficiencyRef2 / 100.0) * (coolCalc.AutomaticRef2 / 100.0) * (coolCalc.EnergyManagementRef2 / 100.0) * (coolCalc.GeneratorColdEfficiency1Ref2 / 100.0));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3181` `double num2 = coolCalc.ResulNetEnergyRef2 * coolCalc.Part2Ref2 / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3182` `coolCalc.ResultSourceEnergy2Ref2 = num2 / (coolCalc.TransmitTempEfficiency2Ref2 / 100.0 * (coolCalc.SupplyNetEfficiency2Ref2 / 100.0) * (coolCalc.Automatic2Ref2 / 100.0) * (coolCalc.EnergyManagement2Ref2 / 100.0) * (coolCalc.GeneratorColdEfficiency2Ref2 / 1...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3187` `coolCalc.ResultNeededEnergyRef2 = coolCalc.ResultSourceEnergyRef2 + coolCalc.ResultSourceEnergy2Ref2;`

### HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3191`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.ResultNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3204`; `coolCalc.ResultSourceEnergy2Actual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3199`; `coolCalc.ResultSourceEnergy2Actual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3202`; `coolCalc.ResultSourceEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3193`; `coolCalc.ResultSourceEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3196`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3192`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3198`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3192` `double num = coolCalc.ResulNetEnergyActual * coolCalc.Part1Actual / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3193` `coolCalc.ResultSourceEnergyActual = num / (coolCalc.TransmitTempEfficiencyActual / 100.0 * (coolCalc.SupplyNetEfficiencyActual / 100.0) * (coolCalc.AutomaticActual / 100.0) * (coolCalc.EnergyManagementActual / 100.0) * (coolCalc.GeneratorColdEfficiency1Actu...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3198` `double num2 = coolCalc.ResulNetEnergyActual * coolCalc.Part2Actual / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3199` `coolCalc.ResultSourceEnergy2Actual = num2 / (coolCalc.TransmitTempEfficiency2Actual / 100.0 * (coolCalc.SupplyNetEfficiency2Actual / 100.0) * (coolCalc.Automatic2Actual / 100.0) * (coolCalc.EnergyManagement2Actual / 100.0) * (coolCalc.GeneratorColdEfficienc...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3204` `coolCalc.ResultNeededEnergyActual = coolCalc.ResultSourceEnergyActual + coolCalc.ResultSourceEnergy2Actual;`

### HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3208`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.ResultNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3221`; `coolCalc.ResultSourceEnergy2BaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3216`; `coolCalc.ResultSourceEnergy2BaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3219`; `coolCalc.ResultSourceEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3210`; `coolCalc.ResultSourceEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3213`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3209`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3215`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3209` `double num = coolCalc.ResulNetEnergyBaseLine * coolCalc.Part1BaseLine / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3210` `coolCalc.ResultSourceEnergyBaseLine = num / (coolCalc.TransmitTempEfficiencyBaseLine / 100.0 * (coolCalc.SupplyNetEfficiencyBaseLine / 100.0) * (coolCalc.AutomaticBaseLine / 100.0) * (coolCalc.EnergyManagementBaseLine / 100.0) * (coolCalc.GeneratorColdEffic...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3215` `double num2 = coolCalc.ResulNetEnergyBaseLine * coolCalc.Part2BaseLine / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3216` `coolCalc.ResultSourceEnergy2BaseLine = num2 / (coolCalc.TransmitTempEfficiency2BaseLine / 100.0 * (coolCalc.SupplyNetEfficiency2BaseLine / 100.0) * (coolCalc.Automatic2BaseLine / 100.0) * (coolCalc.EnergyManagement2BaseLine / 100.0) * (coolCalc.GeneratorCol...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3221` `coolCalc.ResultNeededEnergyBaseLine = coolCalc.ResultSourceEnergyBaseLine + coolCalc.ResultSourceEnergy2BaseLine;`

### HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3225`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.ResultNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3238`; `coolCalc.ResultNeededEnergySavings (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3239`; `coolCalc.ResultSourceEnergy2ESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3233`; `coolCalc.ResultSourceEnergy2ESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3236`; `coolCalc.ResultSourceEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3227`; `coolCalc.ResultSourceEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3230`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3226`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3232`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3226` `double num = coolCalc.ResulNetEnergyESM * coolCalc.Part1ESM / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3227` `coolCalc.ResultSourceEnergyESM = num / (coolCalc.TransmitTempEfficiencyESM / 100.0 * (coolCalc.SupplyNetEfficiencyESM / 100.0) * (coolCalc.AutomaticESM / 100.0) * (coolCalc.EnergyManagementESM / 100.0) * (coolCalc.GeneratorColdEfficiency1ESM / 100.0));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3232` `double num2 = coolCalc.ResulNetEnergyESM * coolCalc.Part2ESM / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3233` `coolCalc.ResultSourceEnergy2ESM = num2 / (coolCalc.TransmitTempEfficiency2ESM / 100.0 * (coolCalc.SupplyNetEfficiency2ESM / 100.0) * (coolCalc.Automatic2ESM / 100.0) * (coolCalc.EnergyManagement2ESM / 100.0) * (coolCalc.GeneratorColdEfficiency2ESM / 100.0));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3238` `coolCalc.ResultNeededEnergyESM = coolCalc.ResultSourceEnergyESM + coolCalc.ResultSourceEnergy2ESM;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3239` `coolCalc.ResultNeededEnergySavings = (coolCalc.ResultNeededEnergyBaseLine - coolCalc.ResultNeededEnergyESM).ToString("F3");`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5106`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.Lights.Cooling.DevicesNeededEnergyRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5109`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5108`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5107`
- Internal calls: `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5108` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5109` `calcData.Lights.Cooling.DevicesNeededEnergyRef1 = calcData.Lights.Cooling.WorkScheduleRef1 * calcData.Lights.Cooling.PowerRef1 * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5113`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.Lights.Cooling.DevicesNeededEnergyRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5116`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5115`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5114`
- Internal calls: `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5115` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5116` `calcData.Lights.Cooling.DevicesNeededEnergyRef2 = calcData.Lights.Cooling.WorkScheduleRef2 * calcData.Lights.Cooling.PowerRef2 * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5178`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.Lights.Cooling.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5201`; `calcData.Lights.Cooling.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5212`; `calcData.Lights.Cooling.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5217`; `calcData.Lights.Cooling.PowerActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5204`; `calcData.Lights.Cooling.WorkScheduleActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5208`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5179`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5180`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5182`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5188`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5181`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5187` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5188` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5196` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5197` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5199` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5200` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5201` `calcData.Lights.Cooling.DevicesNeededEnergyActual = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5208` `calcData.Lights.Cooling.WorkScheduleActual = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5217` `calcData.Lights.Cooling.DevicesNeededEnergyActual = calcData.Lights.Cooling.WorkScheduleActual * calcData.Lights.Cooling.PowerActual * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5310`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.Lights.Cooling.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5333`; `calcData.Lights.Cooling.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5344`; `calcData.Lights.Cooling.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5349`; `calcData.Lights.Cooling.PowerBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5336`; `calcData.Lights.Cooling.WorkScheduleBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5340`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5311`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5312`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5314`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5320`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5313`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5319` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5320` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5328` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5329` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5331` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5332` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5333` `calcData.Lights.Cooling.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5340` `calcData.Lights.Cooling.WorkScheduleBaseLine = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5349` `calcData.Lights.Cooling.DevicesNeededEnergyBaseLine = calcData.Lights.Cooling.WorkScheduleBaseLine * calcData.Lights.Cooling.PowerBaseLine * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5442`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.Lights.Cooling.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5465`; `calcData.Lights.Cooling.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5476`; `calcData.Lights.Cooling.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5481`; `calcData.Lights.Cooling.PowerESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5468`; `calcData.Lights.Cooling.WorkScheduleESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5472`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5443`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5444`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5446`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5452`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5445`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5451` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5452` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5460` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5461` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5463` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5464` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5465` `calcData.Lights.Cooling.DevicesNeededEnergyESM = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5472` `calcData.Lights.Cooling.WorkScheduleESM = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5481` `calcData.Lights.Cooling.DevicesNeededEnergyESM = calcData.Lights.Cooling.WorkScheduleESM * calcData.Lights.Cooling.PowerESM * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1Balanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5537`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.BalancedDevices.Cooling.DevicesNeededEnergyRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5540`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5539`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5538`
- Internal calls: `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5539` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5540` `calcData.BalancedDevices.Cooling.DevicesNeededEnergyRef1 = calcData.BalancedDevices.Cooling.WorkScheduleRef1 * calcData.BalancedDevices.Cooling.PowerRef1 * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2Balanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5558`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.BalancedDevices.Cooling.DevicesNeededEnergyRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5561`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5560`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5559`
- Internal calls: `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5560` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5561` `calcData.BalancedDevices.Cooling.DevicesNeededEnergyRef2 = calcData.BalancedDevices.Cooling.WorkScheduleRef2 * calcData.BalancedDevices.Cooling.PowerRef2 * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5616`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.BalancedDevices.Cooling.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5639`; `calcData.BalancedDevices.Cooling.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5650`; `calcData.BalancedDevices.Cooling.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5655`; `calcData.BalancedDevices.Cooling.PowerActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5642`; `calcData.BalancedDevices.Cooling.WorkScheduleActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5646`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5617`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5618`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5620`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5626`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5619`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5625` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5626` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5634` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5635` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5637` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5638` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5639` `calcData.BalancedDevices.Cooling.DevicesNeededEnergyActual = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5646` `calcData.BalancedDevices.Cooling.WorkScheduleActual = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5655` `calcData.BalancedDevices.Cooling.DevicesNeededEnergyActual = calcData.BalancedDevices.Cooling.WorkScheduleActual * calcData.BalancedDevices.Cooling.PowerActual * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5748`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.BalancedDevices.Cooling.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5771`; `calcData.BalancedDevices.Cooling.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5782`; `calcData.BalancedDevices.Cooling.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5787`; `calcData.BalancedDevices.Cooling.PowerBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5774`; `calcData.BalancedDevices.Cooling.WorkScheduleBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5778`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5749`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5750`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5752`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5758`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5751`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5757` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5758` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5766` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5767` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5769` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5770` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5771` `calcData.BalancedDevices.Cooling.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5778` `calcData.BalancedDevices.Cooling.WorkScheduleBaseLine = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5787` `calcData.BalancedDevices.Cooling.DevicesNeededEnergyBaseLine = calcData.BalancedDevices.Cooling.WorkScheduleBaseLine * calcData.BalancedDevices.Cooling.PowerBaseLine * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5880`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.BalancedDevices.Cooling.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5903`; `calcData.BalancedDevices.Cooling.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5914`; `calcData.BalancedDevices.Cooling.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5919`; `calcData.BalancedDevices.Cooling.PowerESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5906`; `calcData.BalancedDevices.Cooling.WorkScheduleESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5910`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5881`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5882`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5884`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5890`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5883`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5889` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5890` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5898` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5899` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5901` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5902` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5903` `calcData.BalancedDevices.Cooling.DevicesNeededEnergyESM = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5910` `calcData.BalancedDevices.Cooling.WorkScheduleESM = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5919` `calcData.BalancedDevices.Cooling.DevicesNeededEnergyESM = calcData.BalancedDevices.Cooling.WorkScheduleESM * calcData.BalancedDevices.Cooling.PowerESM * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1NonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5982`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5985`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5984`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5983`
- Internal calls: `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5984` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5985` `calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyRef1 = calcData.NonBalancedDevices.Cooling.WorkScheduleRef1 * calcData.NonBalancedDevices.Cooling.PowerRef1 * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2NonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5989`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5992`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5991`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5990`
- Internal calls: `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5991` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5992` `calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyRef2 = calcData.NonBalancedDevices.Cooling.WorkScheduleRef2 * calcData.NonBalancedDevices.Cooling.PowerRef2 * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualNonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6046`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6069`; `calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6072`; `calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6077`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6047`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6048`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6050`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6056`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6049`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6064`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6060`; ... (+5)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6055` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6056` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6064` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6065` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6067` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6068` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6069` `calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyActual = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6077` `calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyActual = calcData.NonBalancedDevices.Cooling.WorkScheduleActual * calcData.NonBalancedDevices.Cooling.PowerActual * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineNonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6162`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6185`; `calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6196`; `calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6201`; `calcData.NonBalancedDevices.Cooling.PowerBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6188`; `calcData.NonBalancedDevices.Cooling.WorkScheduleBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6192`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6163`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6164`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6166`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6172`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6165`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6171` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6172` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6180` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6181` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6183` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6184` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6185` `calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6192` `calcData.NonBalancedDevices.Cooling.WorkScheduleBaseLine = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6201` `calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyBaseLine = calcData.NonBalancedDevices.Cooling.WorkScheduleBaseLine * calcData.NonBalancedDevices.Cooling.PowerBaseLine * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMNonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6294`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6317`; `calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6328`; `calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6333`; `calcData.NonBalancedDevices.Cooling.PowerESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6320`; `calcData.NonBalancedDevices.Cooling.WorkScheduleESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6324`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6295`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6296`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6298`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6304`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6297`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6303` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6304` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6312` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6313` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6315` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6316` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6317` `calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyESM = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6324` `calcData.NonBalancedDevices.Cooling.WorkScheduleESM = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6333` `calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyESM = calcData.NonBalancedDevices.Cooling.WorkScheduleESM * calcData.NonBalancedDevices.Cooling.PowerESM * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef1HotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6389`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.HotWaterPumps.Cooling.DevicesNeededEnergyRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6392`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6391`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6390`
- Internal calls: `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6391` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6392` `calcData.HotWaterPumps.Cooling.DevicesNeededEnergyRef1 = calcData.HotWaterPumps.Cooling.WorkScheduleRef1 * calcData.HotWaterPumps.Cooling.PowerRef1 * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodRef2HotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6410`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.HotWaterPumps.Cooling.DevicesNeededEnergyRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6413`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6412`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6411`
- Internal calls: `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6412` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6413` `calcData.HotWaterPumps.Cooling.DevicesNeededEnergyRef2 = calcData.HotWaterPumps.Cooling.WorkScheduleRef2 * calcData.HotWaterPumps.Cooling.PowerRef2 * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodActualHotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6460`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.HotWaterPumps.Cooling.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6483`; `calcData.HotWaterPumps.Cooling.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6486`; `calcData.HotWaterPumps.Cooling.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6491`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6461`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6462`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6464`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6470`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6463`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6478`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6474`; ... (+5)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6469` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6470` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6478` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6479` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6481` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6482` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6483` `calcData.HotWaterPumps.Cooling.DevicesNeededEnergyActual = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6491` `calcData.HotWaterPumps.Cooling.DevicesNeededEnergyActual = calcData.HotWaterPumps.Cooling.WorkScheduleActual * calcData.HotWaterPumps.Cooling.PowerActual * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodBaseLineHotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6576`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.HotWaterPumps.Cooling.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6599`; `calcData.HotWaterPumps.Cooling.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6610`; `calcData.HotWaterPumps.Cooling.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6615`; `calcData.HotWaterPumps.Cooling.PowerBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6602`; `calcData.HotWaterPumps.Cooling.WorkScheduleBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6606`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6577`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6578`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6580`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6586`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6579`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6585` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6586` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6594` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6595` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6597` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6598` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6599` `calcData.HotWaterPumps.Cooling.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6606` `calcData.HotWaterPumps.Cooling.WorkScheduleBaseLine = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6615` `calcData.HotWaterPumps.Cooling.DevicesNeededEnergyBaseLine = calcData.HotWaterPumps.Cooling.WorkScheduleBaseLine * calcData.HotWaterPumps.Cooling.PowerBaseLine * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateCoolingPeriodESMHotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6709`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.HotWaterPumps.Cooling.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6732`; `calcData.HotWaterPumps.Cooling.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6743`; `calcData.HotWaterPumps.Cooling.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6748`; `calcData.HotWaterPumps.Cooling.DevicesNeededEnergySavings (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6749`; `calcData.HotWaterPumps.Cooling.PowerESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6735`; `calcData.HotWaterPumps.Cooling.WorkScheduleESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6739`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6710`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6711`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6713`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6719`; ... (+8)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6718` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6719` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6727` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6728` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6730` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6731` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6732` `calcData.HotWaterPumps.Cooling.DevicesNeededEnergyESM = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6739` `calcData.HotWaterPumps.Cooling.WorkScheduleESM = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6748` `calcData.HotWaterPumps.Cooling.DevicesNeededEnergyESM = calcData.HotWaterPumps.Cooling.WorkScheduleESM * calcData.HotWaterPumps.Cooling.PowerESM * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6749` `calcData.HotWaterPumps.Cooling.DevicesNeededEnergySavings = (calcData.HotWaterPumps.Cooling.DevicesNeededEnergyBaseLine - calcData.HotWaterPumps.Cooling.DevicesNeededEnergyESM).ToString("F3");`

### HeatingAndCoolingResultCalc.CalculateCoolingSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11456`
- Inputs: `this CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `BuildingZone zone`, `CalculationData lightsAndDevicesCalculationData`, `CalculationData ventCool`
- Outputs: writes `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11467`; `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11481`; `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11498`; `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11506`; `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11523`; `dataRow (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11485`; `dataRow (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11510`; `dataRow.Value (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11496`; `dataRow.Value (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11521`; `dataRow2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11503`; ... (+27)
- Internal calls: `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM`, `HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingBaseLine`, `HeatingAndCoolingResultCalc.CalculateNeededEnergyCoolingESM`, `HeatingAndCoolingResultCalc.CalculateUsavingType`, `HeatingAndCoolingResultCalc.CalculateUsavingTypeESM`, `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSources`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSourcesESM`, `HeatingAndCoolingResultCalc.CheckForFuelSavings`, `HeatingAndCoolingResultCalc.CheckForSavings`, `HeatingAndCoolingResultCalc.CopyCoolingWorkingSchedule`, `HeatingAndCoolingResultCalc.CopyCoolingWorkingScheduleESM`, `HeatingAndCoolingResultCalc.CreateCoolingVirtualBaseLine`, `HeatingAndCoolingResultCalc.CreateCoolingVirtualESM`, `HeatingAndCoolingResultCalc.GetBaseLine`, `HeatingAndCoolingResultCalc.GetESM`, `HeatingAndCoolingResultCalc.SetBaseLine`, `HeatingAndCoolingResultCalc.SetESM`, `HeatingAndCoolingResultCalc.SetSavingsValues`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11505` `saving.Saving = virtualBaseLineNetEnergy - saving.NetEnergy;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11530` `saving.SavingNMinusOne = saving.NetEnergyNMinusOne - virtualESMNetEnergy;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11532` `double num = list.Sum((SavingsData o) => o.Saving);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11535` `item.Part = item.Saving / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11537` `double num2 = virtualBaseLineNetEnergy - virtualESMNetEnergy;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11538` `double num3 = num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11541` `item2.ActualSaving = num2 * (item2.Saving / num2 * num3 + item2.SavingNMinusOne / num2) / 2.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11543` `double num4 = list.Sum((SavingsData o) => o.ActualSaving);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11544` `double num5 = (virtualBaseLineNetEnergy - virtualESMNetEnergy) / num4;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11547` `item3.ActualSaving *= num5;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11553` `double num6 = virtualBaseLineNetEnergy - virtualESMNetEnergy - list.Sum((SavingsData o) => o.ActualSaving);`

### HeatingAndCoolingResultCalc.CreateCoolingVirtualBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11560`
- Inputs: `CalculationData tempCalculationdata`, `Section section`, `CalculationInput calcInput`, `CalculationData lightsAndDevicesCalculationData`, `List<MonthlyDays> monthslist`, `CalculationData ventCool`
- Outputs: returns `List<DataRow>`; writes `baseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11561`; `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11572`; `dataRow (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11562`; `dataRow.Value (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11565`; `dataRow2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11567`; `dataRow2.Value (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11570`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateCoolingEnergyBaseLine`, `HeatingAndCoolingResultCalc.GetBaseLine`, `HeatingAndCoolingResultCalc.SetBaseLine`
- Formulas: _None extracted_

### HeatingAndCoolingResultCalc.CreateCoolingVirtualESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11578`
- Inputs: `CalculationData tempCalculationdata`, `Section section`, `CalculationInput calcInput`, `CalculationData lightsAndDevicesCalculationData`, `List<MonthlyDays> monthslist`, `CalculationData ventCool`
- Outputs: returns `List<DataRow>`; writes `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11590`; `dataRow (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11580`; `dataRow.Value (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11583`; `dataRow2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11585`; `dataRow2.Value (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11588`; `eSM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11579`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateCoolingEnergyESM`, `HeatingAndCoolingResultCalc.GetESM`, `HeatingAndCoolingResultCalc.SetESM`
- Formulas: _None extracted_

### HeatingAndCoolingResultCalc.CalculateVentilationCoolingSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11694`
- Inputs: `this CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `BuildingZone zone`, `CoolingCalculations coolCalculations`
- Outputs: writes `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11705`; `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11716`; `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11732`; `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11747`; `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11766`; `dataRow (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11720`; `dataRow.Value (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11729`; `dataRow2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11738`; `dataRow3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11753`; `dataRow3.Value (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11762`; ... (+18)
- Internal calls: `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CalculateGeneratorVentilationCoolEfficiencyBaseLine`, `HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyBaseLine`, `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSources`, `HeatingAndCoolingResultCalc.CheckForFuelSavings`, `HeatingAndCoolingResultCalc.CheckForVentilationSavings`, `HeatingAndCoolingResultCalc.CopyVentilationCoolingWorkingSchedule`, `HeatingAndCoolingResultCalc.GetVentilationBaseLine`, `HeatingAndCoolingResultCalc.SetVentilationBaseLine`, `HeatingAndCoolingResultCalc.SetVentilationSavingsValues`, `HeatingAndCoolingResultCalc.VentilationCoolEnergyBaseLine`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11706` `IList<SavingsData> list = CheckForVentilationSavings("Вентилация - Охлаждане");`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11740` `saving.Saving = virtualBaseLineNetEnergy - saving.NetEnergy;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11742` `double num = list.Sum((SavingsData o) => o.Saving);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11745` `item.Part = item.Saving / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11776` `double num2 = resultNeededEnergyBaseLine - dataRow4.Value;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11779` `item2.ActualSaving = num2 * item2.Part;`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsCoolingSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11902`
- Inputs: `this HeatingCalculations calc`, `Section section`, `BuildingZone zone`
- Outputs: writes `fansAndPumnps (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11903`; `item.ActualSaving (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11918`; `item.ActualSaving (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11925`; `item.ActualSaving (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11932`; `item.ActualSaving (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11939`; `item.ActualSaving (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11946`; `item.ActualSaving (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11953`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11904`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11909`; `num10 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11923`; ... (+12)
- Internal calls: `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CheckCoolingForFansAndPumpsSavings`, `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekCoolingSeasonHoursEsm`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekCoolingVentilationHoursEsm`, `HeatingAndCoolingResultCalc.SetCoolingFansAndPumpsSavingsValues`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11904` `IList<SavingsData> list = CheckCoolingForFansAndPumpsSavings("Помпи и вентилатори - Охлаждане");`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11909` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11916` `double num12 = calc.FansAndPumps.VentilatorsCoolBaseLine * GetWeekCoolingVentilationHoursBaseLine(section) * num / (calc.FansAndPumps.EnergyManagement2BaseLine / 100.0) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11917` `double num13 = calc.FansAndPumps.VentilatorsCoolESM * GetWeekCoolingVentilationHoursEsm(section) * num / (calc.FansAndPumps.EnergyManagement2ESM / 100.0) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11918` `item.ActualSaving = num12 - num13;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11923` `double num10 = calc.FansAndPumps.PumpVentilationCoolBaseLine * GetWeekCoolingVentilationHoursBaseLine(section) * num / (calc.FansAndPumps.EnergyManagement2BaseLine / 100.0) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11924` `double num11 = calc.FansAndPumps.PumpVentilationCoolESM * GetWeekCoolingVentilationHoursEsm(section) * num / (calc.FansAndPumps.EnergyManagement2ESM / 100.0) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11925` `item.ActualSaving = num10 - num11;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11930` `double num8 = calc.FansAndPumps.VentilatorsOutdoorAirCoolBaseLine * GetWeekCoolingVentilationHoursBaseLine(section) * num / (calc.FansAndPumps.EnergyManagement2BaseLine / 100.0) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11931` `double num9 = calc.FansAndPumps.VentilatorsOutdoorAirCoolESM * GetWeekCoolingVentilationHoursEsm(section) * num / (calc.FansAndPumps.EnergyManagement2ESM / 100.0) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11932` `item.ActualSaving = num8 - num9;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11937` `double num6 = calc.FansAndPumps.CoolingPumpBaseLine * 24.0 * 7.0 * num / (calc.FansAndPumps.EnergyManagement2BaseLine / 100.0) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11938` `double num7 = calc.FansAndPumps.CoolingPumpESM * 24.0 * 7.0 * num / (calc.FansAndPumps.EnergyManagement2ESM / 100.0) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11939` `item.ActualSaving = num6 - num7;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11944` `double num4 = calc.FansAndPumps.OtherCoolingVentilationBaseLine * GetWeekCoolingSeasonHoursBaseLine(section) * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11945` `double num5 = calc.FansAndPumps.OtherCoolingVentilationESM * GetWeekCoolingSeasonHoursEsm(section) * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11946` `item.ActualSaving = num4 - num5;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11951` `double num2 = calc.FansAndPumps.OtherCoolingBaseLine * GetWeekCoolingSeasonHoursBaseLine(section) * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11952` `double num3 = calc.FansAndPumps.OtherCoolingESM * GetWeekCoolingSeasonHoursEsm(section) * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11953` `item.ActualSaving = num2 - num3;`

### HeatingAndCoolingResultCalc.SetCoolingFansAndPumpsSavingsValues
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11964`
- Inputs: `IList<SavingsData> savings`
- Outputs: writes `fansAndPumnps.CoolingPumpSavings (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11968`; `fansAndPumnps.OtherCoolingSavings (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11970`; `fansAndPumnps.OtherCoolingVentilationSavings (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11969`; `fansAndPumnps.PumpVentilationCoolSavings (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11967`; `fansAndPumnps.VentilatorsCoolSavings (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11965`; `fansAndPumnps.VentilatorsOutdoorAirCoolSavings (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11966`
- Internal calls: `HeatingAndCoolingResultCalc.GetSaving`
- Formulas: _None extracted_

### HeatingAndCoolingResultCalc.CheckCoolingForFansAndPumpsSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11974`
- Inputs: `string technology`
- Outputs: returns `IList<SavingsData>`; writes `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11975`
- Internal calls: _None_
- Formulas: _None extracted_

### HeatingAndCoolingResultCalc.CopyCoolingWorkingSchedule
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14002`
- Inputs: `Section tempSection`, `Section section`
- Outputs: writes `tempSection.CoolingSeasons.Cooling.SatBaseEnd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14006`; `tempSection.CoolingSeasons.Cooling.SatBaseStart (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14005`; `tempSection.CoolingSeasons.Cooling.SunBaseEnd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14008`; `tempSection.CoolingSeasons.Cooling.SunBaseStart (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14007`; `tempSection.CoolingSeasons.Cooling.WorkBaseEnd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14004`; `tempSection.CoolingSeasons.Cooling.WorkBaseStart (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14003`
- Internal calls: _None_
- Formulas: _None extracted_

### HeatingAndCoolingResultCalc.CopyCoolingWorkingScheduleESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14012`
- Inputs: `Section tempSection`, `Section section`
- Outputs: writes `tempSection.CoolingSeasons.Cooling.SatEsmEnd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14016`; `tempSection.CoolingSeasons.Cooling.SatEsmStart (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14015`; `tempSection.CoolingSeasons.Cooling.SunEsmEnd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14018`; `tempSection.CoolingSeasons.Cooling.SunEsmStart (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14017`; `tempSection.CoolingSeasons.Cooling.WorkEsmEnd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14014`; `tempSection.CoolingSeasons.Cooling.WorkEsmStart (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14013`
- Internal calls: _None_
- Formulas: _None extracted_

### HeatingAndCoolingResultCalc.CopyVentilationCoolingWorkingSchedule
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14022`
- Inputs: `Section tempSection`, `Section section`
- Outputs: writes `tempSection.CoolingSeasons.Ventilation.SatBaseEnd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14026`; `tempSection.CoolingSeasons.Ventilation.SatBaseStart (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14025`; `tempSection.CoolingSeasons.Ventilation.SunBaseEnd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14028`; `tempSection.CoolingSeasons.Ventilation.SunBaseStart (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14027`; `tempSection.CoolingSeasons.Ventilation.WorkBaseEnd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14024`; `tempSection.CoolingSeasons.Ventilation.WorkBaseStart (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14023`
- Internal calls: _None_
- Formulas: _None extracted_

### HeatingAndCoolingResultCalc.VentilationCoolEnergyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14814`
- Inputs: `this CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `CoolingCalculations ventCoolingCalculations`
- Outputs: writes `calcData.ResulCoolingInputsRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14834`; `calcData.ResultEnergyForCoolingRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14836`; `calcData.ResultEnergyForHeatingRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14837`; `calcData.ResultEnergyForWitheringRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14838`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14815`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14816`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14817`; `list4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14818`; `list5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14819`; `ventCoolingCalculations.CoolingResult.ResulVentilationInputsRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14835`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateCoolingInputsRef1`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyRef1`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyRef1`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14834` `calcData.ResulCoolingInputsRef1 = list3.Aggregate(0.0, (double num, double item) => num + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14836` `calcData.ResultEnergyForCoolingRef1 = list.Aggregate(0.0, (double num, double item) => num + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14837` `calcData.ResultEnergyForHeatingRef1 = list2.Aggregate(0.0, (double num, double item) => num + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14838` `calcData.ResultEnergyForWitheringRef1 = list4.Aggregate(0.0, (double num, double item) => num + item);`

### HeatingAndCoolingResultCalc.VentilationCoolEnergyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14842`
- Inputs: `this CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `CoolingCalculations ventCoolingCalculations`
- Outputs: writes `calcData.ResulCoolingInputsRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14862`; `calcData.ResultEnergyForCoolingRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14864`; `calcData.ResultEnergyForHeatingRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14865`; `calcData.ResultEnergyForWitheringRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14866`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14843`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14844`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14845`; `list4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14846`; `list5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14847`; `ventCoolingCalculations.CoolingResult.ResulVentilationInputsRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14863`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateCoolingInputsRef2`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyRef2`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyRef2`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14862` `calcData.ResulCoolingInputsRef2 = list3.Aggregate(0.0, (double num, double item) => num + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14864` `calcData.ResultEnergyForCoolingRef2 = list.Aggregate(0.0, (double num, double item) => num + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14865` `calcData.ResultEnergyForHeatingRef2 = list2.Aggregate(0.0, (double num, double item) => num + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14866` `calcData.ResultEnergyForWitheringRef2 = list4.Aggregate(0.0, (double num, double item) => num + item);`

### HeatingAndCoolingResultCalc.VentilationCoolEnergyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14870`
- Inputs: `this CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `CoolingCalculations ventCoolingCalculations`
- Outputs: writes `calcData.ResulCoolingInputsActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14890`; `calcData.ResultEnergyForCoolingActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14892`; `calcData.ResultEnergyForHeatingActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14893`; `calcData.ResultEnergyForWitheringActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14894`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14871`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14872`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14873`; `list4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14874`; `list5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14875`; `ventCoolingCalculations.CoolingResult.ResulVentilationInputsActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14891`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateCoolingInputs`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyActual`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyActual`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14890` `calcData.ResulCoolingInputsActual = list3.Aggregate(0.0, (double num, double item) => num + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14892` `calcData.ResultEnergyForCoolingActual = list.Aggregate(0.0, (double num, double item) => num + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14893` `calcData.ResultEnergyForHeatingActual = list2.Aggregate(0.0, (double num, double item) => num + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14894` `calcData.ResultEnergyForWitheringActual = list4.Aggregate(0.0, (double num, double item) => num + item);`

### HeatingAndCoolingResultCalc.VentilationCoolEnergyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14898`
- Inputs: `this CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `CoolingCalculations ventCoolingCalculations`
- Outputs: writes `calcData.ResulCoolingInputsBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14918`; `calcData.ResultEnergyForCoolingBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14920`; `calcData.ResultEnergyForHeatingBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14921`; `calcData.ResultEnergyForWitheringBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14922`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14899`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14900`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14901`; `list4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14902`; `list5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14903`; `ventCoolingCalculations.CoolingResult.ResulVentilationInputsBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14919`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateCoolingInputsBaseLine`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyBaseLine`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyBaseLine`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14918` `calcData.ResulCoolingInputsBaseLine = list3.Aggregate(0.0, (double num, double item) => num + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14920` `calcData.ResultEnergyForCoolingBaseLine = list.Aggregate(0.0, (double num, double item) => num + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14921` `calcData.ResultEnergyForHeatingBaseLine = list2.Aggregate(0.0, (double num, double item) => num + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14922` `calcData.ResultEnergyForWitheringBaseLine = list4.Aggregate(0.0, (double num, double item) => num + item);`

### HeatingAndCoolingResultCalc.VentilationCoolEnergyEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14926`
- Inputs: `this CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `CoolingCalculations ventCoolingCalculations`
- Outputs: writes `calcData.ResulCoolingInputsESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14946`; `calcData.ResultEnergyForCoolingESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14948`; `calcData.ResultEnergyForHeatingESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14949`; `calcData.ResultEnergyForWitheringESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14950`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14927`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14928`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14929`; `list4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14930`; `list5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14931`; `ventCoolingCalculations.CoolingResult.ResulVentilationInputsESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14947`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateCoolingInputsESM`, `HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyESM`, `HeatingAndCoolingResultCalc.CalculateWitheringEnergyESM`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14946` `calcData.ResulCoolingInputsESM = list3.Aggregate(0.0, (double num, double item) => num + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14948` `calcData.ResultEnergyForCoolingESM = list.Aggregate(0.0, (double num, double item) => num + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14949` `calcData.ResultEnergyForHeatingESM = list2.Aggregate(0.0, (double num, double item) => num + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14950` `calcData.ResultEnergyForWitheringESM = list4.Aggregate(0.0, (double num, double item) => num + item);`

### HeatingAndCoolingResultCalc.GetWeekHoursCoolingReferences
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14954`
- Inputs: `this CalculationData coolingCalc`, `Section section`
- Outputs: writes `coolingCalc.WorkingScheduleRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14958`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14956`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14955`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14957`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14955` `double num = 5.0 * section.CalcHours(section.CoolingSeasons.Ventilation.WorkBaseStart, section.CoolingSeasons.Ventilation.WorkBaseEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14956` `num += section.CalcHours(section.CoolingSeasons.Ventilation.SunBaseStart, section.CoolingSeasons.Ventilation.SunBaseEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14957` `num = (coolingCalc.WorkingScheduleRef = num + section.CalcHours(section.CoolingSeasons.Ventilation.SatBaseStart, section.CoolingSeasons.Ventilation.SatBaseEnd));`

### HeatingAndCoolingResultCalc.GetWeekHoursCoolingActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14962`
- Inputs: `this CalculationData coolingCalc`, `Section section`
- Outputs: writes `coolingCalc.WorkingScheduleActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14966`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14964`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14965`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14963`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14963` `double num = 5.0 * section.CalcHours(section.CoolingSeasons.Ventilation.WorkCurrentStart, section.CoolingSeasons.Ventilation.WorkCurrentEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14964` `num += section.CalcHours(section.CoolingSeasons.Ventilation.SunCurrentStart, section.CoolingSeasons.Ventilation.SunCurrentEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14965` `num += section.CalcHours(section.CoolingSeasons.Ventilation.SatCurrentStart, section.CoolingSeasons.Ventilation.SatCurrentEnd);`

### HeatingAndCoolingResultCalc.GetWeekHoursCoolingBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14970`
- Inputs: `this CalculationData coolingCalc`, `Section section`
- Outputs: writes `coolingCalc.WorkingScheduleBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14974`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14972`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14973`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14971`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14971` `double num = 5.0 * section.CalcHours(section.CoolingSeasons.Ventilation.WorkBaseStart, section.CoolingSeasons.Ventilation.WorkBaseEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14972` `num += section.CalcHours(section.CoolingSeasons.Ventilation.SunBaseStart, section.CoolingSeasons.Ventilation.SunBaseEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14973` `num += section.CalcHours(section.CoolingSeasons.Ventilation.SatBaseStart, section.CoolingSeasons.Ventilation.SatBaseEnd);`

### HeatingAndCoolingResultCalc.GetWeekHoursCoolingEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14978`
- Inputs: `this CalculationData coolingCalc`, `Section section`
- Outputs: writes `coolingCalc.WorkingScheduleESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14982`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14980`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14981`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14979`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14979` `double num = 5.0 * section.CalcHours(section.CoolingSeasons.Ventilation.WorkEsmStart, section.CoolingSeasons.Ventilation.WorkEsmEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14980` `num += section.CalcHours(section.CoolingSeasons.Ventilation.SunEsmStart, section.CoolingSeasons.Ventilation.SunEsmEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14981` `num += section.CalcHours(section.CoolingSeasons.Ventilation.SatEsmStart, section.CoolingSeasons.Ventilation.SatEsmEnd);`

### HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14986`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.ResultNeededEnergyRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15001`; `coolCalc.ResultSourceEnergy2Ref1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14996`; `coolCalc.ResultSourceEnergy2Ref1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14999`; `coolCalc.ResultSourceEnergyRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14989`; `coolCalc.ResultSourceEnergyRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14992`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14988`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14995`; `resultEnergyForCoolingRef (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14987`; `resultEnergyForCoolingRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14994`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14988` `double num = resultEnergyForCoolingRef * coolCalc.Part1Ref1 / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14989` `coolCalc.ResultSourceEnergyRef1 = num / (coolCalc.TransmitTempEfficiencyRef1 / 100.0 * (coolCalc.SupplyNetEfficiencyRef1 / 100.0) * (coolCalc.AutomaticRef1 / 100.0) * (coolCalc.EnergyManagementRef1 / 100.0) * (coolCalc.GeneratorColdEfficiency1Ref1 / 100.0));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14995` `double num2 = resultEnergyForCoolingRef2 * coolCalc.Part2Ref1 / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:14996` `coolCalc.ResultSourceEnergy2Ref1 = num2 / (coolCalc.TransmitTempEfficiency2Ref1 / 100.0 * (coolCalc.SupplyNetEfficiency2Ref1 / 100.0) * (coolCalc.Automatic2Ref1 / 100.0) * (coolCalc.EnergyManagement2Ref1 / 100.0) * (coolCalc.GeneratorColdEfficiency2Ref1 / 1...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15001` `coolCalc.ResultNeededEnergyRef1 = coolCalc.ResultSourceEnergyRef1 + coolCalc.ResultSourceEnergy2Ref1;`

### HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15005`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.ResultNeededEnergyRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15020`; `coolCalc.ResultSourceEnergy2Ref2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15015`; `coolCalc.ResultSourceEnergy2Ref2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15018`; `coolCalc.ResultSourceEnergyRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15008`; `coolCalc.ResultSourceEnergyRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15011`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15007`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15014`; `resultEnergyForCoolingRef (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15006`; `resultEnergyForCoolingRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15013`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15007` `double num = resultEnergyForCoolingRef * coolCalc.Part1Ref2 / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15008` `coolCalc.ResultSourceEnergyRef2 = num / (coolCalc.TransmitTempEfficiencyRef2 / 100.0 * (coolCalc.SupplyNetEfficiencyRef2 / 100.0) * (coolCalc.AutomaticRef2 / 100.0) * (coolCalc.EnergyManagementRef2 / 100.0) * (coolCalc.GeneratorColdEfficiency1Ref2 / 100.0));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15014` `double num2 = resultEnergyForCoolingRef2 * coolCalc.Part2Ref2 / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15015` `coolCalc.ResultSourceEnergy2Ref2 = num2 / (coolCalc.TransmitTempEfficiency2Ref2 / 100.0 * (coolCalc.SupplyNetEfficiency2Ref2 / 100.0) * (coolCalc.Automatic2Ref2 / 100.0) * (coolCalc.EnergyManagement2Ref2 / 100.0) * (coolCalc.GeneratorColdEfficiency2Ref2 / 1...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15020` `coolCalc.ResultNeededEnergyRef2 = coolCalc.ResultSourceEnergyRef2 + coolCalc.ResultSourceEnergy2Ref2;`

### HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15024`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.ResultNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15039`; `coolCalc.ResultSourceEnergy2Actual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15034`; `coolCalc.ResultSourceEnergy2Actual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15037`; `coolCalc.ResultSourceEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15027`; `coolCalc.ResultSourceEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15030`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15026`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15033`; `resultEnergyForCoolingActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15025`; `resultEnergyForCoolingActual2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15032`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15026` `double num = resultEnergyForCoolingActual * coolCalc.Part1Actual / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15027` `coolCalc.ResultSourceEnergyActual = num / (coolCalc.TransmitTempEfficiencyActual / 100.0 * (coolCalc.SupplyNetEfficiencyActual / 100.0) * (coolCalc.AutomaticActual / 100.0) * (coolCalc.EnergyManagementActual / 100.0) * (coolCalc.GeneratorColdEfficiency1Actu...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15033` `double num2 = resultEnergyForCoolingActual2 * coolCalc.Part2Actual / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15034` `coolCalc.ResultSourceEnergy2Actual = num2 / (coolCalc.TransmitTempEfficiency2Actual / 100.0 * (coolCalc.SupplyNetEfficiency2Actual / 100.0) * (coolCalc.Automatic2Actual / 100.0) * (coolCalc.EnergyManagement2Actual / 100.0) * (coolCalc.GeneratorColdEfficienc...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15039` `coolCalc.ResultNeededEnergyActual = coolCalc.ResultSourceEnergyActual + coolCalc.ResultSourceEnergy2Actual;`

### HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15043`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.ResultNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15058`; `coolCalc.ResultSourceEnergy2BaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15053`; `coolCalc.ResultSourceEnergy2BaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15056`; `coolCalc.ResultSourceEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15046`; `coolCalc.ResultSourceEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15049`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15045`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15052`; `resultEnergyForCoolingBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15044`; `resultEnergyForCoolingBaseLine2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15051`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15045` `double num = resultEnergyForCoolingBaseLine * coolCalc.Part1BaseLine / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15046` `coolCalc.ResultSourceEnergyBaseLine = num / (coolCalc.TransmitTempEfficiencyBaseLine / 100.0 * (coolCalc.SupplyNetEfficiencyBaseLine / 100.0) * (coolCalc.AutomaticBaseLine / 100.0) * (coolCalc.EnergyManagementBaseLine / 100.0) * (coolCalc.GeneratorColdEffic...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15052` `double num2 = resultEnergyForCoolingBaseLine2 * coolCalc.Part2BaseLine / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15053` `coolCalc.ResultSourceEnergy2BaseLine = num2 / (coolCalc.TransmitTempEfficiency2BaseLine / 100.0 * (coolCalc.SupplyNetEfficiency2BaseLine / 100.0) * (coolCalc.Automatic2BaseLine / 100.0) * (coolCalc.EnergyManagement2BaseLine / 100.0) * (coolCalc.GeneratorCol...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15058` `coolCalc.ResultNeededEnergyBaseLine = coolCalc.ResultSourceEnergyBaseLine + coolCalc.ResultSourceEnergy2BaseLine;`

### HeatingAndCoolingResultCalc.CalculateVentCoolNeededEnergyEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15062`
- Inputs: `this CalculationData coolCalc`
- Outputs: writes `coolCalc.ResultNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15077`; `coolCalc.ResultNeededEnergySavings (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15078`; `coolCalc.ResultSourceEnergy2ESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15072`; `coolCalc.ResultSourceEnergy2ESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15075`; `coolCalc.ResultSourceEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15065`; `coolCalc.ResultSourceEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15068`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15064`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15071`; `resultEnergyForCoolingESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15063`; `resultEnergyForCoolingESM2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15070`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15064` `double num = resultEnergyForCoolingESM * coolCalc.Part1ESM / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15065` `coolCalc.ResultSourceEnergyESM = num / (coolCalc.TransmitTempEfficiencyESM / 100.0 * (coolCalc.SupplyNetEfficiencyESM / 100.0) * (coolCalc.AutomaticESM / 100.0) * (coolCalc.EnergyManagementESM / 100.0) * (coolCalc.GeneratorColdEfficiency1ESM / 100.0));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15071` `double num2 = resultEnergyForCoolingESM2 * coolCalc.Part2ESM / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15072` `coolCalc.ResultSourceEnergy2ESM = num2 / (coolCalc.TransmitTempEfficiency2ESM / 100.0 * (coolCalc.SupplyNetEfficiency2ESM / 100.0) * (coolCalc.Automatic2ESM / 100.0) * (coolCalc.EnergyManagement2ESM / 100.0) * (coolCalc.GeneratorColdEfficiency2ESM / 100.0));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15077` `coolCalc.ResultNeededEnergyESM = coolCalc.ResultSourceEnergyESM + coolCalc.ResultSourceEnergy2ESM;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15078` `coolCalc.ResultNeededEnergySavings = (coolCalc.ResultNeededEnergyBaseLine - coolCalc.ResultNeededEnergyESM).ToString("F3");`

### HeatingAndCoolingResultCalc.CalculateCoolingInputsRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15082`
- Inputs: `Section section`, `CalculationData calcData`, `CalculationData ventCool`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15087`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15083`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15086`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15089`; `num4 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15094`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15090`; `num5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15093`; `num6 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15096`; `num7 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15101`; `num7 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15097`; ... (+2)
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15086` `double num2 = ((i >= section.CoolingSeasons.Cooling.WorkCurrentStart && i < section.CoolingSeasons.Cooling.WorkCurrentEnd) ? calcData.ProjectTemperatureRef1 : calcData.NonProjectTemperatureRef1);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15087` `num += ventCool.DebitRef1 * 0.34 * (num2 - ventCool.FlowTemperatureRef1) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15089` `double num3 = num * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15093` `double num5 = ((j >= section.CoolingSeasons.Cooling.SatCurrentStart && j <= section.CoolingSeasons.Cooling.SatCurrentEnd) ? calcData.ProjectTemperatureRef1 : calcData.NonProjectTemperatureRef1);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15094` `num4 += ventCool.DebitRef1 * 0.34 * (num5 - ventCool.FlowTemperatureRef1) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15096` `double num6 = num4 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15100` `double num8 = ((k >= section.CoolingSeasons.Cooling.SunCurrentStart && k <= section.CoolingSeasons.Cooling.SunCurrentEnd) ? calcData.ProjectTemperatureRef1 : calcData.NonProjectTemperatureRef1);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15101` `num7 += ventCool.DebitRef1 * 0.34 * (num8 - ventCool.FlowTemperatureRef1) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15103` `double num9 = num7 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15104` `return num3 + num6 + num9;`

### HeatingAndCoolingResultCalc.CalculateCoolingInputsRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15108`
- Inputs: `Section section`, `CalculationData calcData`, `CalculationData ventCool`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15113`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15109`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15112`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15115`; `num4 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15120`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15116`; `num5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15119`; `num6 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15122`; `num7 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15127`; `num7 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15123`; ... (+2)
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15112` `double num2 = ((i >= section.CoolingSeasons.Cooling.WorkCurrentStart && i < section.CoolingSeasons.Cooling.WorkCurrentEnd) ? calcData.ProjectTemperatureRef2 : calcData.NonProjectTemperatureRef2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15113` `num += ventCool.DebitRef2 * 0.34 * (num2 - ventCool.FlowTemperatureRef2) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15115` `double num3 = num * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15119` `double num5 = ((j >= section.CoolingSeasons.Cooling.SatCurrentStart && j <= section.CoolingSeasons.Cooling.SatCurrentEnd) ? calcData.ProjectTemperatureRef2 : calcData.NonProjectTemperatureRef2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15120` `num4 += ventCool.DebitRef2 * 0.34 * (num5 - ventCool.FlowTemperatureRef2) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15122` `double num6 = num4 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15126` `double num8 = ((k >= section.CoolingSeasons.Cooling.SunCurrentStart && k <= section.CoolingSeasons.Cooling.SunCurrentEnd) ? calcData.ProjectTemperatureRef2 : calcData.NonProjectTemperatureRef2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15127` `num7 += ventCool.DebitRef2 * 0.34 * (num8 - ventCool.FlowTemperatureRef2) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15129` `double num9 = num7 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15130` `return num3 + num6 + num9;`

### HeatingAndCoolingResultCalc.CalculateCoolingInputs
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15134`
- Inputs: `Section section`, `CalculationData calcData`, `CalculationData ventCool`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15139`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15135`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15138`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15141`; `num4 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15146`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15142`; `num5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15145`; `num6 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15148`; `num7 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15153`; `num7 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15149`; ... (+2)
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15138` `double num2 = ((i >= section.CoolingSeasons.Cooling.WorkCurrentStart && i < section.CoolingSeasons.Cooling.WorkCurrentEnd) ? calcData.ProjectTemperatureActual : calcData.NonProjectTemperatureActual);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15139` `num += ventCool.DebitActual * 0.34 * (num2 - ventCool.FlowTemperatureActual) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15141` `double num3 = num * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15145` `double num5 = ((j >= section.CoolingSeasons.Cooling.SatCurrentStart && j <= section.CoolingSeasons.Cooling.SatCurrentEnd) ? calcData.ProjectTemperatureActual : calcData.NonProjectTemperatureActual);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15146` `num4 += ventCool.DebitActual * 0.34 * (num5 - ventCool.FlowTemperatureActual) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15148` `double num6 = num4 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15152` `double num8 = ((k >= section.CoolingSeasons.Cooling.SunCurrentStart && k <= section.CoolingSeasons.Cooling.SunCurrentEnd) ? calcData.ProjectTemperatureActual : calcData.NonProjectTemperatureActual);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15153` `num7 += ventCool.DebitActual * 0.34 * (num8 - ventCool.FlowTemperatureActual) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15155` `double num9 = num7 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15156` `return num3 + num6 + num9;`

### HeatingAndCoolingResultCalc.CalculateCoolingInputsBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15160`
- Inputs: `Section section`, `CalculationData calcData`, `CalculationData ventCool`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15165`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15161`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15164`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15167`; `num4 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15172`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15168`; `num5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15171`; `num6 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15174`; `num7 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15179`; `num7 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15175`; ... (+2)
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15164` `double num2 = ((i >= section.CoolingSeasons.Cooling.WorkBaseStart && i < section.CoolingSeasons.Cooling.WorkBaseEnd) ? calcData.ProjectTemperatureBaseLine : calcData.NonProjectTemperatureBaseLine);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15165` `num += ventCool.DebitBaseLine * 0.34 * (num2 - ventCool.FlowTemperatureBaseLine) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15167` `double num3 = num * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15171` `double num5 = ((j >= section.CoolingSeasons.Cooling.SatBaseStart && j <= section.CoolingSeasons.Cooling.SatBaseEnd) ? calcData.ProjectTemperatureBaseLine : calcData.NonProjectTemperatureBaseLine);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15172` `num4 += ventCool.DebitBaseLine * 0.34 * (num5 - ventCool.FlowTemperatureBaseLine) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15174` `double num6 = num4 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15178` `double num8 = ((k >= section.CoolingSeasons.Cooling.SunBaseStart && k <= section.CoolingSeasons.Cooling.SunBaseEnd) ? calcData.ProjectTemperatureBaseLine : calcData.NonProjectTemperatureBaseLine);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15179` `num7 += ventCool.DebitBaseLine * 0.34 * (num8 - ventCool.FlowTemperatureBaseLine) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15181` `double num9 = num7 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15182` `return num3 + num6 + num9;`

### HeatingAndCoolingResultCalc.CalculateCoolingInputsESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15186`
- Inputs: `Section section`, `CalculationData calcData`, `CalculationData ventCool`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15191`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15187`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15190`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15193`; `num4 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15198`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15194`; `num5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15197`; `num6 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15200`; `num7 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15205`; `num7 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15201`; ... (+2)
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15190` `double num2 = ((i >= section.CoolingSeasons.Cooling.WorkEsmStart && i < section.CoolingSeasons.Cooling.WorkEsmEnd) ? calcData.ProjectTemperatureESM : calcData.NonProjectTemperatureESM);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15191` `num += ventCool.DebitESM * 0.34 * (num2 - ventCool.FlowTemperatureESM) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15193` `double num3 = num * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15197` `double num5 = ((j >= section.CoolingSeasons.Cooling.SatEsmStart && j <= section.CoolingSeasons.Cooling.SatEsmEnd) ? calcData.ProjectTemperatureESM : calcData.NonProjectTemperatureESM);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15198` `num4 += ventCool.DebitESM * 0.34 * (num5 - ventCool.FlowTemperatureESM) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15200` `double num6 = num4 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15204` `double num8 = ((k >= section.CoolingSeasons.Cooling.SunEsmStart && k <= section.CoolingSeasons.Cooling.SunEsmEnd) ? calcData.ProjectTemperatureESM : calcData.NonProjectTemperatureESM);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15205` `num7 += ventCool.DebitESM * 0.34 * (num8 - ventCool.FlowTemperatureESM) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15207` `double num9 = num7 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15208` `return num3 + num6 + num9;`

### HeatingAndCoolingResultCalc.GetDaysHours
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15212`
- Inputs: `ClimateZones climateZone`, `int month`
- Outputs: returns `List<TempHumidityPerDay>`; writes `item (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15213`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15214`
- Internal calls: `PreferencesManager.GetClimateZoneParams`
- Formulas: _None extracted_

### HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15220`
- Inputs: `Section section`, `CalculationInput calcInput`, `CoolingCalculations ventCoolCalculations`, `MonthlyDays month`, `out double powHeating`, `out double powCooling`
- Outputs: writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15223`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15233`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15221`; `num10 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15253`; `num11 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15265`; `num11 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15254`; `num12 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15261`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15255`; `num13 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15258`; `num14 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15268`; ... (+14)
- Internal calls: `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.CalculateEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15226` `double num3 = ventCoolCalculations.VentilationCooling.DebitRef1 * (CalcRoW(daysHours[i].Temp) * CalculateEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef1) * CalculateEntalpia(ventCoolCa...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15229` `num2 += Math.Abs(num3);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15233` `num += num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15236` `double num4 = num2 / 3600.0 * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15237` `double num5 = num / 3600.0 * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15242` `double num8 = ventCoolCalculations.VentilationCooling.DebitRef1 * (CalcRoW(daysHours[j].Temp) * CalculateEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef1) * CalculateEntalpia(ventCoolCa...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15245` `num7 += Math.Abs(num8);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15249` `num6 += num8;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15252` `double num9 = num7 / 3600.0 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15253` `double num10 = num6 / 3600.0 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15258` `double num13 = ventCoolCalculations.VentilationCooling.DebitRef1 * (CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalculateEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef1) * Calcu...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15261` `num12 += Math.Abs(num13);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15265` `num11 += num13;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15268` `double num14 = num12 / 3600.0 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15269` `double num15 = num11 / 3600.0 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15270` `powHeating = num14 + num9 + num4;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15271` `powCooling = num15 + num10 + num5;`

### HeatingAndCoolingResultCalc.CalculateWitheringEnergyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15275`
- Inputs: `Section section`, `CalculationInput calcInput`, `CoolingCalculations ventCoolCalculations`, `MonthlyDays month`
- Outputs: returns `double`; writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15277`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15281`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15276`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15280`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15283`; `num4 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15288`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15284`; `num5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15287`; `num6 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15290`; `num7 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15295`; ... (+3)
- Internal calls: `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.CalculateWitheringEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15280` `double num2 = ventCoolCalculations.VentilationCooling.DebitRef1 * (CalcRoW(daysHours[i].Temp) * CalculateWitheringEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef1) * CalculateWitheringE...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15281` `num += num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15283` `double num3 = num / 3600.0 * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15287` `double num5 = ventCoolCalculations.VentilationCooling.DebitRef1 * (CalcRoW(daysHours[j].Temp) * CalculateWitheringEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef1) * CalculateWitheringE...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15288` `num4 += num5;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15290` `double num6 = num4 / 3600.0 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15294` `double num8 = ventCoolCalculations.VentilationCooling.DebitRef1 * (CalcRoW(daysHours[k].Temp) * CalculateWitheringEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef1) * CalculateWitheringE...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15295` `num7 += num8;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15297` `double num9 = num7 / 3600.0 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15298` `return num3 + num6 + num9;`

### HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15302`
- Inputs: `Section section`, `CalculationInput calcInput`, `CoolingCalculations ventCoolCalculations`, `MonthlyDays month`, `out double powHeating`, `out double powCooling`
- Outputs: writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15305`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15315`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15303`; `num10 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15335`; `num11 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15347`; `num11 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15336`; `num12 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15343`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15337`; `num13 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15340`; `num14 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15350`; ... (+14)
- Internal calls: `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.CalculateEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15308` `double num3 = ventCoolCalculations.VentilationCooling.DebitRef2 * (CalcRoW(daysHours[i].Temp) * CalculateEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef2) * CalculateEntalpia(ventCoolCa...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15311` `num2 += Math.Abs(num3);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15315` `num += num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15318` `double num4 = num2 / 3600.0 * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15319` `double num5 = num / 3600.0 * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15324` `double num8 = ventCoolCalculations.VentilationCooling.DebitRef2 * (CalcRoW(daysHours[j].Temp) * CalculateEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef2) * CalculateEntalpia(ventCoolCa...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15327` `num7 += Math.Abs(num8);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15331` `num6 += num8;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15334` `double num9 = num7 / 3600.0 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15335` `double num10 = num6 / 3600.0 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15340` `double num13 = ventCoolCalculations.VentilationCooling.DebitRef2 * (CalcRoW(daysHours[k].Temp) * CalculateEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef2) * CalculateEntalpia(ventCoolC...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15343` `num12 += Math.Abs(num13);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15347` `num11 += num13;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15350` `double num14 = num12 / 3600.0 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15351` `double num15 = num11 / 3600.0 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15352` `powHeating = num14 + num9 + num4;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15353` `powCooling = num15 + num10 + num5;`

### HeatingAndCoolingResultCalc.CalculateWitheringEnergyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15357`
- Inputs: `Section section`, `CalculationInput calcInput`, `CoolingCalculations ventCoolCalculations`, `MonthlyDays month`
- Outputs: returns `double`; writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15359`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15363`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15358`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15362`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15365`; `num4 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15370`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15366`; `num5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15369`; `num6 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15372`; `num7 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15377`; ... (+3)
- Internal calls: `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.CalculateWitheringEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15362` `double num2 = ventCoolCalculations.VentilationCooling.DebitRef2 * (CalcRoW(daysHours[i].Temp) * CalculateWitheringEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef2) * CalculateWitheringE...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15363` `num += num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15365` `double num3 = num / 3600.0 * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15369` `double num5 = ventCoolCalculations.VentilationCooling.DebitRef2 * (CalcRoW(daysHours[j].Temp) * CalculateWitheringEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef2) * CalculateWitheringE...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15370` `num4 += num5;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15372` `double num6 = num4 / 3600.0 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15376` `double num8 = ventCoolCalculations.VentilationCooling.DebitRef2 * (CalcRoW(daysHours[k].Temp) * CalculateWitheringEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef2) * CalculateWitheringE...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15377` `num7 += num8;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15379` `double num9 = num7 / 3600.0 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15380` `return num3 + num6 + num9;`

### HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15384`
- Inputs: `Section section`, `CalculationInput calcInput`, `CoolingCalculations ventCoolCalculations`, `MonthlyDays month`, `out double powHeating`, `out double powCooling`
- Outputs: writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15387`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15397`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15385`; `num10 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15417`; `num11 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15429`; `num11 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15418`; `num12 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15425`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15419`; `num13 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15422`; `num14 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15432`; ... (+14)
- Internal calls: `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.CalculateEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15390` `double num3 = ventCoolCalculations.VentilationCooling.DebitActual * (CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalculateEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(ventCoolCalculations.VentilationCooling.FlowTemperatureActual, ventC...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15393` `num2 += Math.Abs(num3);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15397` `num += num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15400` `double num4 = num2 / 3600.0 * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15401` `double num5 = num / 3600.0 * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15406` `double num8 = ventCoolCalculations.VentilationCooling.DebitActual * (CalcRo(daysHours[j].Temp, daysHours[j].Humidity) * CalculateEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRo(ventCoolCalculations.VentilationCooling.FlowTemperatureActual, ventC...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15409` `num7 += Math.Abs(num8);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15413` `num6 += num8;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15416` `double num9 = num7 / 3600.0 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15417` `double num10 = num6 / 3600.0 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15422` `double num13 = ventCoolCalculations.VentilationCooling.DebitActual * (CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalculateEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(ventCoolCalculations.VentilationCooling.FlowTemperatureActual, vent...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15425` `num12 += Math.Abs(num13);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15429` `num11 += num13;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15432` `double num14 = num12 / 3600.0 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15433` `double num15 = num11 / 3600.0 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15434` `powHeating = num14 + num9 + num4;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15435` `powCooling = num15 + num10 + num5;`

### HeatingAndCoolingResultCalc.CalculateWitheringEnergyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15439`
- Inputs: `Section section`, `CalculationInput calcInput`, `CoolingCalculations ventCoolCalculations`, `MonthlyDays month`
- Outputs: returns `double`; writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15441`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15445`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15440`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15444`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15447`; `num4 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15452`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15448`; `num5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15451`; `num6 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15454`; `num7 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15459`; ... (+3)
- Internal calls: `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.CalculateWitheringEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15444` `double num2 = ventCoolCalculations.VentilationCooling.DebitActual * (CalcRoW(daysHours[i].Temp) * CalculateWitheringEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureActual) * CalculateWither...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15445` `num += num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15447` `double num3 = num / 3600.0 * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15451` `double num5 = ventCoolCalculations.VentilationCooling.DebitActual * (CalcRoW(daysHours[j].Temp) * CalculateWitheringEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureActual) * CalculateWither...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15452` `num4 += num5;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15454` `double num6 = num4 / 3600.0 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15458` `double num8 = ventCoolCalculations.VentilationCooling.DebitActual * (CalcRoW(daysHours[k].Temp) * CalculateWitheringEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureActual) * CalculateWither...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15459` `num7 += num8;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15461` `double num9 = num7 / 3600.0 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15462` `return num3 + num6 + num9;`

### HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15466`
- Inputs: `Section section`, `CalculationInput calcInput`, `CalculationData calcData`, `MonthlyDays month`, `out double powHeating`, `out double powCooling`
- Outputs: writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15469`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15479`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15467`; `num10 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15499`; `num11 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15511`; `num11 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15500`; `num12 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15507`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15501`; `num13 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15504`; `num14 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15514`; ... (+14)
- Internal calls: `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.CalculateEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15472` `double num3 = calcData.DebitBaseLine * (CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalculateEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.FlowTemperatureBaseLine, calcData.RelativeHumidityBaseLine) * CalculateEntalpia(calcData...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15475` `num2 += Math.Abs(num3);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15479` `num += num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15482` `double num4 = num2 / 3600.0 * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15483` `double num5 = num / 3600.0 * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15488` `double num8 = calcData.DebitBaseLine * (CalcRo(daysHours[j].Temp, daysHours[j].Humidity) * CalculateEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRo(calcData.FlowTemperatureBaseLine, calcData.RelativeHumidityBaseLine) * CalculateEntalpia(calcData...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15491` `num7 += Math.Abs(num8);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15495` `num6 += num8;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15498` `double num9 = num7 / 3600.0 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15499` `double num10 = num6 / 3600.0 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15504` `double num13 = calcData.DebitBaseLine * (CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalculateEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.FlowTemperatureBaseLine, calcData.RelativeHumidityBaseLine) * CalculateEntalpia(calcDat...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15507` `num12 += Math.Abs(num13);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15511` `num11 += num13;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15514` `double num14 = num12 / 3600.0 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15515` `double num15 = num11 / 3600.0 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15516` `powHeating = num14 + num9 + num4;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15517` `powCooling = num15 + num10 + num5;`

### HeatingAndCoolingResultCalc.CalculateWitheringEnergyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15521`
- Inputs: `Section section`, `CalculationInput calcInput`, `CalculationData calcData`, `MonthlyDays month`
- Outputs: returns `double`; writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15523`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15527`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15522`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15526`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15529`; `num4 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15534`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15530`; `num5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15533`; `num6 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15536`; `num7 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15541`; ... (+3)
- Internal calls: `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.CalculateWitheringEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15526` `double num2 = calcData.DebitBaseLine * (CalcRoW(daysHours[i].Temp) * CalculateWitheringEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRoW(calcData.FlowTemperatureBaseLine) * CalculateWitheringEntalpia(calcData.FlowTemperatureBaseLine, calcData.Rel...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15527` `num += num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15529` `double num3 = num / 3600.0 * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15533` `double num5 = calcData.DebitBaseLine * (CalcRoW(daysHours[j].Temp) * CalculateWitheringEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRoW(calcData.FlowTemperatureBaseLine) * CalculateWitheringEntalpia(calcData.FlowTemperatureBaseLine, calcData.Rel...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15534` `num4 += num5;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15536` `double num6 = num4 / 3600.0 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15540` `double num8 = calcData.DebitBaseLine * (CalcRoW(daysHours[k].Temp) * CalculateWitheringEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRoW(calcData.FlowTemperatureBaseLine) * CalculateWitheringEntalpia(calcData.FlowTemperatureBaseLine, calcData.Rel...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15541` `num7 += num8;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15543` `double num9 = num7 / 3600.0 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15544` `return num3 + num6 + num9;`

### HeatingAndCoolingResultCalc.CalculateMontlyCoolEnergyESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15548`
- Inputs: `Section section`, `CalculationInput calcInput`, `CoolingCalculations ventCoolCalculations`, `MonthlyDays month`, `out double powHeating`, `out double powCooling`
- Outputs: writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15551`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15561`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15549`; `num10 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15581`; `num11 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15593`; `num11 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15582`; `num12 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15589`; `num12 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15583`; `num13 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15586`; `num14 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15596`; ... (+14)
- Internal calls: `HeatingAndCoolingResultCalc.CalcRo`, `HeatingAndCoolingResultCalc.CalculateEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15554` `double num3 = ventCoolCalculations.VentilationCooling.DebitESM * (CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalculateEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(ventCoolCalculations.VentilationCooling.FlowTemperatureESM, ventCoolCal...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15557` `num2 += Math.Abs(num3);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15561` `num += num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15564` `double num4 = num2 / 3600.0 * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15565` `double num5 = num / 3600.0 * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15570` `double num8 = ventCoolCalculations.VentilationCooling.DebitESM * (CalcRo(daysHours[j].Temp, daysHours[j].Humidity) * CalculateEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRo(ventCoolCalculations.VentilationCooling.FlowTemperatureESM, ventCoolCal...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15573` `num7 += Math.Abs(num8);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15577` `num6 += num8;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15580` `double num9 = num7 / 3600.0 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15581` `double num10 = num6 / 3600.0 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15586` `double num13 = ventCoolCalculations.VentilationCooling.DebitESM * (CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalculateEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(ventCoolCalculations.VentilationCooling.FlowTemperatureESM, ventCoolCa...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15589` `num12 += Math.Abs(num13);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15593` `num11 += num13;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15596` `double num14 = num12 / 3600.0 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15597` `double num15 = num11 / 3600.0 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15598` `powHeating = num14 + num9 + num4;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15599` `powCooling = num15 + num10 + num5;`

### HeatingAndCoolingResultCalc.CalculateWitheringEnergyESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15603`
- Inputs: `Section section`, `CalculationInput calcInput`, `CoolingCalculations ventCoolCalculations`, `MonthlyDays month`
- Outputs: returns `double`; writes `daysHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15605`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15609`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15604`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15608`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15611`; `num4 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15616`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15612`; `num5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15615`; `num6 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15618`; `num7 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15623`; ... (+3)
- Internal calls: `HeatingAndCoolingResultCalc.CalcRoW`, `HeatingAndCoolingResultCalc.CalculateWitheringEntalpia`, `HeatingAndCoolingResultCalc.GetDaysHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15608` `double num2 = ventCoolCalculations.VentilationCooling.DebitESM * (CalcRoW(daysHours[i].Temp) * CalculateWitheringEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureESM) * CalculateWitheringEnt...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15609` `num += num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15611` `double num3 = num / 3600.0 * (double)month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15615` `double num5 = ventCoolCalculations.VentilationCooling.DebitESM * (CalcRoW(daysHours[j].Temp) * CalculateWitheringEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureESM) * CalculateWitheringEnt...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15616` `num4 += num5;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15618` `double num6 = num4 / 3600.0 * (double)month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15622` `double num8 = ventCoolCalculations.VentilationCooling.DebitESM * (CalcRoW(daysHours[k].Temp) * CalculateWitheringEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureESM) * CalculateWitheringEnt...`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15623` `num7 += num8;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15625` `double num9 = num7 / 3600.0 * (double)month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15626` `return num3 + num6 + num9;`

### HeatingAndCoolingResultCalc.CalculateEntalpia
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15630`
- Inputs: `double temp`, `double humidity`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15631`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15632` `return 1.006 * temp + num * (2501.0 + 1.805 * temp);`

### HeatingAndCoolingResultCalc.CalculateWitheringEntalpia
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15636`
- Inputs: `double temp`, `double humidity`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15637`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAirX`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15638` `return num * (2501.0 + 1.805 * temp);`

### HeatingAndCoolingResultCalc.CalcEntalpia
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16454`
- Inputs: `double temp`, `double humidity`, `double pb`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16455`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16456`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16457`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16458`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16455` `double num = 273.15 + temp;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16456` `double num2 = Math.Pow(2.718281828459, 77.345 + 0.0057 * num - 7235.0 / num) / Math.Pow(num, 8.2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16457` `double num3 = humidity * num2 / 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16458` `double num4 = 0.62198 * (num3 / (pb - num3));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16459` `return 1.006 * temp + num4 * (1.805 * temp + 2501.0);`
