# 04 Heating Engine

Heating-related methods and formulas extracted from the decompiled source. This includes the monthly envelope balance, utilization factor, heating ventilation, baseline, ESM, and reference variants.

- Covered methods: `142`

## Method Flow

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

### HeatingAndCoolingResultCalc.CalculateHveRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1751`
- Inputs: `CalculationData ventCool`
- Outputs: returns `double`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1752` `return ventCool.DebitRef1 * 0.34;`

### HeatingAndCoolingResultCalc.CalculateHveRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1756`
- Inputs: `CalculationData ventCool`
- Outputs: returns `double`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1757` `return ventCool.DebitRef2 * 0.34;`

### HeatingAndCoolingResultCalc.CalculateHve
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1761`
- Inputs: `CalculationData ventCool`
- Outputs: returns `double`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1762` `return ventCool.DebitActual * 0.34;`

### HeatingAndCoolingResultCalc.CalculateHveBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1766`
- Inputs: `CalculationData ventCool`
- Outputs: returns `double`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1767` `return ventCool.DebitBaseLine * 0.34;`

### HeatingAndCoolingResultCalc.CalculateHveESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1771`
- Inputs: `CalculationData ventCool`
- Outputs: returns `double`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:1772` `return ventCool.DebitESM * 0.34;`

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

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2255`
- Inputs: `this HeatingCalculations calc`, `Section section`
- Outputs: writes `calc.FansAndPumps.PumpNeededEnergyRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2267`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2257`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2260`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2261`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2259`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2262`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2265`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2256`; `weekHeatingVentilationHoursBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2258`
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursBaseLine`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2257` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2259` `double num2 = calc.FansAndPumps.VentilatorsHeatRef1 * weekHeatingVentilationHoursBaseLine * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2260` `num2 += calc.FansAndPumps.PumpVentilationRef1 * weekHeatingVentilationHoursBaseLine * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2261` `num2 += calc.FansAndPumps.PumpHeatingRef1 * 24.0 * 7.0 * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2262` `num2 = num2 / calc.FansAndPumps.EnergyManagementRef1 * 100.0;`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2271`
- Inputs: `this HeatingCalculations calc`, `Section section`
- Outputs: writes `calc.FansAndPumps.PumpNeededEnergyRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2283`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2273`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2276`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2277`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2275`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2278`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2281`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2272`; `weekHeatingVentilationHoursBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2274`
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursBaseLine`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2273` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2275` `double num2 = calc.FansAndPumps.VentilatorsHeatRef2 * weekHeatingVentilationHoursBaseLine * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2276` `num2 += calc.FansAndPumps.PumpVentilationRef2 * weekHeatingVentilationHoursBaseLine * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2277` `num2 += calc.FansAndPumps.PumpHeatingRef2 * 24.0 * 7.0 * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2278` `num2 = num2 / calc.FansAndPumps.EnergyManagementRef2 * 100.0;`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2287`
- Inputs: `this HeatingCalculations calc`, `Section section`
- Outputs: writes `calc.FansAndPumps.PumpNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2299`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2289`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2292`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2293`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2291`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2294`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2297`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2288`; `weekHeatingVentilationHoursActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2290`
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursActual`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2289` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2291` `double num2 = calc.FansAndPumps.VentilatorsHeatActual * weekHeatingVentilationHoursActual * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2292` `num2 += calc.FansAndPumps.PumpVentilationActual * weekHeatingVentilationHoursActual * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2293` `num2 += calc.FansAndPumps.PumpHeatingActual * 24.0 * 7.0 * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2294` `num2 = num2 / calc.FansAndPumps.EnergyManagementActual * 100.0;`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2303`
- Inputs: `this HeatingCalculations calc`, `Section section`
- Outputs: writes `calc.FansAndPumps.PumpNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2316`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2305`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2308`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2310`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2307`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2311`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2314`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2304`; `weekHeatingVentilationHoursBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2306`; `weekHeatingVentilationHoursBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2309`
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursBaseLine`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2305` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2307` `double num2 = calc.FansAndPumps.VentilatorsHeatBaseLine * weekHeatingVentilationHoursBaseLine * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2308` `num2 += calc.FansAndPumps.PumpVentilationBaseLine * weekHeatingVentilationHoursBaseLine * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2310` `num2 += calc.FansAndPumps.PumpHeatingBaseLine * 24.0 * 7.0 * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2311` `num2 = num2 / calc.FansAndPumps.EnergyManagementBaseLine * 100.0;`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2320`
- Inputs: `this HeatingCalculations calc`, `Section section`
- Outputs: writes `calc.FansAndPumps.PumpNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2333`; `calc.FansAndPumps.PumpNeededEnergySavings (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2334`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2322`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2325`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2327`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2324`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2328`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2331`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2321`; `weekHeatingVentilationHoursEsm (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2323`; ... (+1)
- Internal calls: `HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursEsm`, `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursEsm`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2322` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2324` `double num2 = calc.FansAndPumps.VentilatorsHeatESM * weekHeatingVentilationHoursEsm * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2325` `num2 += calc.FansAndPumps.PumpVentilationESM * weekHeatingVentilationHoursEsm * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2327` `num2 += calc.FansAndPumps.PumpHeatingESM * 24.0 * 7.0 * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2328` `num2 = num2 / calc.FansAndPumps.EnergyManagementESM * 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2334` `calc.FansAndPumps.PumpNeededEnergySavings = (calc.FansAndPumps.PumpNeededEnergyBaseLine - calc.FansAndPumps.PumpNeededEnergyESM).ToString("F3");`

### HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2470`
- Inputs: `Section section`
- Outputs: returns `double`; writes `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2472`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2471`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2471` `double num = 5.0 * section.CalcHours(section.HeatingSeasons.Ventilation.WorkCurrentStart, section.HeatingSeasons.Ventilation.WorkCurrentEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2472` `num += section.CalcHours(section.HeatingSeasons.Ventilation.SunCurrentStart, section.HeatingSeasons.Ventilation.SunCurrentEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2473` `return num + section.CalcHours(section.HeatingSeasons.Ventilation.SatCurrentStart, section.HeatingSeasons.Ventilation.SatCurrentEnd);`

### HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2477`
- Inputs: `Section section`
- Outputs: returns `double`; writes `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2479`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2478`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2478` `double num = 5.0 * section.CalcHours(section.HeatingSeasons.Ventilation.WorkBaseStart, section.HeatingSeasons.Ventilation.WorkBaseEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2479` `num += section.CalcHours(section.HeatingSeasons.Ventilation.SunBaseStart, section.HeatingSeasons.Ventilation.SunBaseEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2480` `return num + section.CalcHours(section.HeatingSeasons.Ventilation.SatBaseStart, section.HeatingSeasons.Ventilation.SatBaseEnd);`

### HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2484`
- Inputs: `Section section`
- Outputs: returns `double`; writes `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2486`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2485`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2485` `double num = 5.0 * section.CalcHours(section.HeatingSeasons.Ventilation.WorkEsmStart, section.HeatingSeasons.Ventilation.WorkEsmEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2486` `num += section.CalcHours(section.HeatingSeasons.Ventilation.SunEsmStart, section.HeatingSeasons.Ventilation.SunEsmEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2487` `return num + section.CalcHours(section.HeatingSeasons.Ventilation.SatEsmStart, section.HeatingSeasons.Ventilation.SatEsmEnd);`

### HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2491`
- Inputs: `Section section`
- Outputs: returns `double`; writes `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2493`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2492`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2492` `double num = 5.0 * section.CalcHours(section.HeatingSeasons.Heating.WorkCurrentStart, section.HeatingSeasons.Heating.WorkCurrentEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2493` `num += section.CalcHours(section.HeatingSeasons.Heating.SunCurrentStart, section.HeatingSeasons.Heating.SunCurrentEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2494` `return num + section.CalcHours(section.HeatingSeasons.Heating.SatCurrentStart, section.HeatingSeasons.Heating.SatCurrentEnd);`

### HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2498`
- Inputs: `Section section`
- Outputs: returns `double`; writes `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2500`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2499`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2499` `double num = 5.0 * section.CalcHours(section.HeatingSeasons.Heating.WorkBaseStart, section.HeatingSeasons.Heating.WorkBaseEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2500` `num += section.CalcHours(section.HeatingSeasons.Heating.SunBaseStart, section.HeatingSeasons.Heating.SunBaseEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2501` `return num + section.CalcHours(section.HeatingSeasons.Heating.SatBaseStart, section.HeatingSeasons.Heating.SatBaseEnd);`

### HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2505`
- Inputs: `Section section`
- Outputs: returns `double`; writes `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2507`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2506`
- Internal calls: `InputDataCalc.CalcHours`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2506` `double num = 5.0 * section.CalcHours(section.HeatingSeasons.Heating.WorkEsmStart, section.HeatingSeasons.Heating.WorkEsmEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2507` `num += section.CalcHours(section.HeatingSeasons.Heating.SunEsmStart, section.HeatingSeasons.Heating.SunEsmEnd);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2508` `return num + section.CalcHours(section.HeatingSeasons.Heating.SatEsmStart, section.HeatingSeasons.Heating.SatEsmEnd);`

### HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2917`
- Inputs: `this CalculationData heatgCalc`
- Outputs: writes `heatgCalc.HeatEfficiencyGeneratingRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2920`; `heatgCalc.HeatEfficiencyGeneratingRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2923`; `heatgCalc.HeatEfficiencyGeneratingRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2928`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2920` `heatgCalc.HeatEfficiencyGeneratingRef1 = (heatgCalc.ResultSourceEnergyRef1 * heatgCalc.GeneratorHeatEfficiency1Ref1 + heatgCalc.ResultSourceEnergy2Ref1 * heatgCalc.GeneratorHeatEfficiency2Ref1) / (heatgCalc.ResultSourceEnergyRef1 + heatgCalc.ResultSourceEne...`

### HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2933`
- Inputs: `this CalculationData heatgCalc`
- Outputs: writes `heatgCalc.HeatEfficiencyGeneratingRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2936`; `heatgCalc.HeatEfficiencyGeneratingRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2939`; `heatgCalc.HeatEfficiencyGeneratingRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2944`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2936` `heatgCalc.HeatEfficiencyGeneratingRef2 = (heatgCalc.ResultSourceEnergyRef2 * heatgCalc.GeneratorHeatEfficiency1Ref2 + heatgCalc.ResultSourceEnergy2Ref2 * heatgCalc.GeneratorHeatEfficiency2Ref2) / (heatgCalc.ResultSourceEnergyRef2 + heatgCalc.ResultSourceEne...`

### HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2949`
- Inputs: `this CalculationData heatgCalc`
- Outputs: writes `heatgCalc.HeatEfficiencyGeneratingActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2952`; `heatgCalc.HeatEfficiencyGeneratingActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2955`; `heatgCalc.HeatEfficiencyGeneratingActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2960`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2952` `heatgCalc.HeatEfficiencyGeneratingActual = (heatgCalc.ResultSourceEnergyActual * heatgCalc.GeneratorHeatEfficiency1Actual + heatgCalc.ResultSourceEnergy2Actual * heatgCalc.GeneratorHeatEfficiency2Actual) / (heatgCalc.ResultSourceEnergyActual + heatgCalc.Res...`

### HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2965`
- Inputs: `this CalculationData heatgCalc`
- Outputs: writes `heatgCalc.HeatEfficiencyGeneratingBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2968`; `heatgCalc.HeatEfficiencyGeneratingBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2971`; `heatgCalc.HeatEfficiencyGeneratingBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2976`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2968` `heatgCalc.HeatEfficiencyGeneratingBaseLine = (heatgCalc.ResultSourceEnergyBaseLine * heatgCalc.GeneratorHeatEfficiency1BaseLine + heatgCalc.ResultSourceEnergy2BaseLine * heatgCalc.GeneratorHeatEfficiency2BaseLine) / (heatgCalc.ResultSourceEnergyBaseLine + h...`

### HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2981`
- Inputs: `this CalculationData heatgCalc`
- Outputs: writes `heatgCalc.HeatEfficiencyGeneratingESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2984`; `heatgCalc.HeatEfficiencyGeneratingESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2987`; `heatgCalc.HeatEfficiencyGeneratingESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2992`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:2984` `heatgCalc.HeatEfficiencyGeneratingESM = (heatgCalc.ResultSourceEnergyESM * heatgCalc.GeneratorHeatEfficiency1ESM + heatgCalc.ResultSourceEnergy2ESM * heatgCalc.GeneratorHeatEfficiency2ESM) / (heatgCalc.ResultSourceEnergyESM + heatgCalc.ResultSourceEnergy2ESM);`

### HeatingAndCoolingResultCalc.OccupantHours
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3467`
- Inputs: `Section section`, `MonthlyDays month`
- Outputs: returns `int`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3468` `return month.WorkDays * (section.HeatingSeasons.Occupants.WorkCurrentEnd - section.HeatingSeasons.Occupants.WorkCurrentStart) + month.Sundays * (section.HeatingSeasons.Occupants.SunCurrentEnd - section.HeatingSeasons.Occupants.SunCurrentStart) + month.Satur...`

### HeatingAndCoolingResultCalc.CalculateActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3483`
- Inputs: `CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `MonthData monthData`, `MonthlyDays month`, `double latentHeatPerMonth`
- Outputs: writes `monthData.NetEnergyQnd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3490`; `monthData.ParameterGama (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3488`; `monthData.ParameterNi (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3489`; `monthData.ParameterQgn (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3487`; `monthData.ParameterQht (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3486`; `monthData.ParameterQtr (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3484`; `monthData.ParameterQve (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3485`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateParameterNign`, `HeatingAndCoolingResultCalc.CalculateParameterQgn`, `HeatingAndCoolingResultCalc.CalculateParameterQtr`, `HeatingAndCoolingResultCalc.CalculateParameterQve`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3486` `monthData.ParameterQht = monthData.ParameterQtr + monthData.ParameterQve;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3487` `monthData.ParameterQgn = CalculateParameterQgn(section, calcInput.General.ClimateZone, month) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3488` `monthData.ParameterGama = (monthData.ParameterQgn + latentHeatPerMonth * section.Area.HeatedArea) / monthData.ParameterQht;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3490` `monthData.NetEnergyQnd = monthData.ParameterQht - monthData.ParameterNi * monthData.ParameterQgn;`

### HeatingAndCoolingResultCalc.CalculateParameterNign
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3632`
- Inputs: `CalculationData calculationdata`, `ClimateZones climateZone`, `MonthlyDays month`, `double gamma`, `Section section`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3633`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateaH`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3636` `return (1.0 - Math.Pow(gamma, num)) / (1.0 - Math.Pow(gamma, num + 1.0));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3644` `return num / (num + 1.0);`

### HeatingAndCoolingResultCalc.CalculateaH
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3650`
- Inputs: `CalculationData calculationdata`, `Section section`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `averageInnerHeatTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3652`; `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3651`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3653`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3654`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3655`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3656`; `num5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3657`; `num6 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3658`
- Internal calls: `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3`, `HeatingAndCoolingResultCalc.CalcParameterHve`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHgCurrent`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3656` `double num4 = CalculateParameterHdCurrent(section) + CalculateParameterHgCurrent(section) + (num + num2 + num3);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3658` `double num6 = section.Area.HeatedArea * section.Area.HeatCapacity / (num4 + num5);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3659` `return 1.0 + num6 / 15.0;`

### HeatingAndCoolingResultCalc.CalculateParameterQve
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3663`
- Inputs: `Section section`, `ClimateZones climateZone`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTemp`, `HeatingAndCoolingResultCalc.CalcAvgProjectTemp`, `HeatingAndCoolingResultCalc.CalcParameterHve`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3664` `return CalcParameterHve(section, calculationData) * (CalcAvgProjectTemp(section, climateZone, calculationData, month) + CalcAvgNonProjectTemp(section, climateZone, calculationData, month)) / 1000.0;`

### HeatingAndCoolingResultCalc.CalcAvgProjectTemp
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3668`
- Inputs: `Section section`, `ClimateZones climateZone`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3669`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3670`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3671`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3672`
- Internal calls: `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3670` `int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3671` `int num2 = month.Sundays * (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3672` `int num3 = month.Saturdays * (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3673` `return (calculationData.ProjectTemperatureActual - avgTemp) * (double)(num + num3 + num2);`

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTemp
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3677`
- Inputs: `Section section`, `ClimateZones climateZone`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3678`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3679`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3680`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3681`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3682`
- Internal calls: `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3679` `int num = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3680` `int num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3681` `int num3 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3682` `int num4 = month.Holydays * 24;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3683` `return (calculationData.NonProjectTemperatureActual - avgTemp) * (double)(num + num2 + num3 + num4);`

### HeatingAndCoolingResultCalc.CalcParameterHve
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3687`
- Inputs: `Section section`, `CalculationData calculationData`
- Outputs: returns `double`; writes `section.Test.ParameterHve (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3688`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3688` `section.Test.ParameterHve = section.Area.HeatedVolume * calculationData.InfiltracionActual * 0.34;`

### HeatingAndCoolingResultCalc.CalculateParameterQtr
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3693`
- Inputs: `CalculationData calculationData`, `Section section`, `ClimateZones climateZone`, `MonthlyDays month`, `out double parameterHtr`
- Outputs: returns `double`; writes `averageInnerHeatTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3695`; `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3694`; `parameterHtr (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3696`; `section.Test.ParameterHtr (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3697`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTemp`, `HeatingAndCoolingResultCalc.CalcAvgProjectTemp`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHtr`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3698` `return section.Test.ParameterHtr * (CalcAvgProjectTemp(section, climateZone, calculationData, month) + CalcAvgNonProjectTemp(section, climateZone, calculationData, month)) / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateParameterHtr
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3702`
- Inputs: `Section section`, `double averageMontlyTemp`, `double averageInnerHeatTemp`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3703`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3704`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3705`; `section.Test.ParameterHd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3707`; `section.Test.ParameterHg (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3708`; `section.Test.ParameterHu (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3706`
- Internal calls: `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3`, `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHgCurrent`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3706` `section.Test.ParameterHu = num + num2 + num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3709` `return section.Test.ParameterHd + section.Test.ParameterHg + section.Test.ParameterHu;`

### HeatingAndCoolingResultCalc.CalculateAverageHeatTempCurrent
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3824`
- Inputs: `Section section`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `nonProjectTemperatureActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3833`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3825`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3826`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3827`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3829`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3830`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3831`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3832`; `projectTemperatureActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3828`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3825` `int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3826` `num = ((month.Sundays > 0) ? (month.Sundays * (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3827` `num = ((month.Saturdays > 0) ? (month.Saturdays * (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3829` `int num2 = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3830` `num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3831` `num2 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3832` `num2 = month.Holydays * 24 + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3834` `return ((double)num * projectTemperatureActual + (double)num2 * nonProjectTemperatureActual) / (double)(num + num2);`

### HeatingAndCoolingResultCalc.CalculateParameterQgn
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3941`
- Inputs: `Section section`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3942`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3943`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3944`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3945`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3946`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3947`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3948`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsol`, `HeatingAndCoolingResultCalc.CalculateTrasparentFsol`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3942` `int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3943` `num = ((month.Sundays > 0) ? (month.Sundays * (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3944` `num = ((month.Saturdays > 0) ? (month.Saturdays * (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3945` `int num2 = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3946` `num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3947` `num2 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3948` `num2 = month.Holydays * 24 + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3949` `return (CalculateNonTrasparentFsol(section, climateZone, month) + CalculateTrasparentFsol(section, climateZone, month)) * (double)(num + num2);`

### HeatingAndCoolingResultCalc.CalculateEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4039`
- Inputs: `this CalculationData heatingAndCoolingCalculations`, `Section section`, `CalculationInput calcInput`, `MonthlyDays month`, `double latentHeatPerMonth`
- Outputs: returns `double`; writes `gamma (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4044`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4040`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4041`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4042`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4043`; `parameterNiESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4045`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateParameterNiEsm`, `HeatingAndCoolingResultCalc.CalculateParameterQgnEsm`, `HeatingAndCoolingResultCalc.CalculateParameterQtrEsm`, `HeatingAndCoolingResultCalc.CalculateParameterQveEsm`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4042` `double num3 = num + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4043` `double num4 = CalculateParameterQgnEsm(section, calcInput.General.ClimateZone, month) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4044` `double gamma = (num4 + latentHeatPerMonth * section.Area.HeatedArea) / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4046` `return num3 - parameterNiESM * num4;`

### HeatingAndCoolingResultCalc.CalculateaHesm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4068`
- Inputs: `CalculationData calculationdata`, `Section section`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `averageInnerHeatTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4070`; `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4069`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4071`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4072`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4073`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4074`; `num5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4075`; `num6 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4076`
- Internal calls: `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3`, `HeatingAndCoolingResultCalc.CalcParameterHveEsm`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempEsm`, `HeatingAndCoolingResultCalc.CalculateParameterHdEsm`, `HeatingAndCoolingResultCalc.CalculateParameterHgEsm`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1Esm`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4074` `double num4 = CalculateParameterHdEsm(section) + CalculateParameterHgEsm(section) + (num + num2 + num3);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4076` `double num6 = section.Area.HeatedArea * section.Area.HeatCapacity / (num4 + num5);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4077` `return 1.0 + num6 / 15.0;`

### HeatingAndCoolingResultCalc.CalculateParameterQveEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4081`
- Inputs: `Section section`, `ClimateZones climateZone`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4082`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempEsm`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempEsm`, `HeatingAndCoolingResultCalc.CalcParameterHveEsm`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4083` `return CalcParameterHveEsm(section, calculationData) * (CalcAvgProjectTempEsm(section, avgTemp, calculationData, month) + CalcAvgNonProjectTempEsm(section, avgTemp, calculationData, month)) / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateParameterHtrEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4087`
- Inputs: `Section section`, `double averageMontlyTemp`, `double averageInnerHeatTemp`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4088`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4089`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4090`; `section.Test.ParameterHd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4092`; `section.Test.ParameterHg (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4093`; `section.Test.ParameterHu (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4091`
- Internal calls: `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3`, `HeatingAndCoolingResultCalc.CalculateParameterHdEsm`, `HeatingAndCoolingResultCalc.CalculateParameterHgEsm`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1Esm`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4091` `section.Test.ParameterHu = num + num2 + num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4094` `return section.Test.ParameterHd + section.Test.ParameterHg + section.Test.ParameterHu;`

### HeatingAndCoolingResultCalc.CalcParameterHveEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4098`
- Inputs: `Section section`, `CalculationData heatingAndCoolingCalculations`
- Outputs: returns `double`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4099` `return section.Area.HeatedVolume * heatingAndCoolingCalculations.InfiltracionESM * 0.34;`

### HeatingAndCoolingResultCalc.CalculateParameterQtrEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4103`
- Inputs: `CalculationData claculationdata`, `Section section`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `averageInnerHeatTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4105`; `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4104`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4106`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4107`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4108`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4109`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempEsm`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempEsm`, `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempEsm`, `HeatingAndCoolingResultCalc.CalculateParameterHdEsm`, `HeatingAndCoolingResultCalc.CalculateParameterHgEsm`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1Esm`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4109` `double num4 = CalculateParameterHdEsm(section) + CalculateParameterHgEsm(section) + (num + num2 + num3);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4110` `return num4 * (CalcAvgProjectTempEsm(section, avgTemp, claculationdata, month) + CalcAvgNonProjectTempEsm(section, avgTemp, claculationdata, month)) / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateAverageHeatTempEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4114`
- Inputs: `Section section`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `nonProjectTemperatureESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4123`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4115`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4116`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4117`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4119`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4120`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4121`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4122`; `projectTemperatureESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4118`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4115` `int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkEsmEnd - section.HeatingSeasons.Heating.WorkEsmStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4116` `num = ((month.Sundays > 0) ? (month.Sundays * (section.HeatingSeasons.Heating.SunEsmEnd - section.HeatingSeasons.Heating.SunEsmStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4117` `num = ((month.Saturdays > 0) ? (month.Saturdays * (section.HeatingSeasons.Heating.SatEsmEnd - section.HeatingSeasons.Heating.SatEsmStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4119` `int num2 = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkEsmEnd - section.HeatingSeasons.Heating.WorkEsmStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4120` `num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatEsmEnd - section.HeatingSeasons.Heating.SatEsmStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4121` `num2 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunEsmEnd - section.HeatingSeasons.Heating.SunEsmStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4122` `num2 = month.Holydays * 24 + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4124` `return ((double)num * projectTemperatureESM + (double)num2 * nonProjectTemperatureESM) / (double)(num + num2);`

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTempEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4128`
- Inputs: `Section section`, `double averageMontlyTemp`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4129`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4130`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4131`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4132`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4129` `int num = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkEsmEnd - section.HeatingSeasons.Heating.WorkEsmStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4130` `int num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatEsmEnd - section.HeatingSeasons.Heating.SatEsmStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4131` `int num3 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunEsmEnd - section.HeatingSeasons.Heating.SunEsmStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4132` `int num4 = month.Holydays * 24;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4133` `return (calculationData.NonProjectTemperatureESM - averageMontlyTemp) * (double)(num + num2 + num3 + num4);`

### HeatingAndCoolingResultCalc.CalcAvgProjectTempEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4137`
- Inputs: `Section section`, `double averageMontlyTemp`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4138`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4139`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4140`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4138` `int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkEsmEnd - section.HeatingSeasons.Heating.WorkEsmStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4139` `int num2 = month.Sundays * (section.HeatingSeasons.Heating.SunEsmEnd - section.HeatingSeasons.Heating.SunEsmStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4140` `int num3 = month.Saturdays * (section.HeatingSeasons.Heating.SatEsmEnd - section.HeatingSeasons.Heating.SatEsmStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4141` `return (calculationData.ProjectTemperatureESM - averageMontlyTemp) * (double)(num + num3 + num2);`

### HeatingAndCoolingResultCalc.CalculateParameterQgnEsm
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4194`
- Inputs: `Section section`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4195`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4196`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4197`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4198`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4199`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4200`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4201`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsolEsm`, `HeatingAndCoolingResultCalc.CalculateTrasparentFsolEsm`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4195` `int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkEsmEnd - section.HeatingSeasons.Heating.WorkEsmStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4196` `num = ((month.Sundays > 0) ? (month.Sundays * (section.HeatingSeasons.Heating.SunEsmEnd - section.HeatingSeasons.Heating.SunEsmStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4197` `num = ((month.Saturdays > 0) ? (month.Saturdays * (section.HeatingSeasons.Heating.SatEsmEnd - section.HeatingSeasons.Heating.SatEsmStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4198` `int num2 = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkEsmEnd - section.HeatingSeasons.Heating.WorkEsmStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4199` `num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatEsmEnd - section.HeatingSeasons.Heating.SatEsmStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4200` `num2 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunEsmEnd - section.HeatingSeasons.Heating.SunEsmStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4201` `num2 = month.Holydays * 24 + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4202` `return (CalculateNonTrasparentFsolEsm(section, climateZone, month) + CalculateTrasparentFsolEsm(section, climateZone, month)) * (double)(num + num2);`

### HeatingAndCoolingResultCalc.CalculateBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4275`
- Inputs: `this CalculationData heatingAndCoolingCalculations`, `Section section`, `CalculationInput calcInput`, `MonthlyDays month`, `double latentHeatPerMonth`
- Outputs: returns `double`; writes `gamma (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4280`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4276`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4277`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4278`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4279`; `parameterNiBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4281`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateParameterNignBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterQgnBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterQtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterQveBaseLIne`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4278` `double num3 = num + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4279` `double num4 = CalculateParameterQgnBaseLine(section, calcInput.General.ClimateZone, month) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4280` `double gamma = (num4 + latentHeatPerMonth * section.Area.HeatedArea) / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4282` `return num3 - parameterNiBaseLine * num4;`

### HeatingAndCoolingResultCalc.CalculateaHbaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4286`
- Inputs: `CalculationData calculationdata`, `Section section`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `averageInnerHeatTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4288`; `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4287`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4289`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4290`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4291`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4292`; `num5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4293`; `num6 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4294`
- Internal calls: `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3`, `HeatingAndCoolingResultCalc.CalcParameterHveBaseLine`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHgCurrent`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4292` `double num4 = CalculateParameterHdCurrent(section) + CalculateParameterHgCurrent(section) + (num + num2 + num3);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4294` `double num6 = section.Area.HeatedArea * section.Area.HeatCapacity / (num4 + num5);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4295` `return 1.0 + num6 / 15.0;`

### HeatingAndCoolingResultCalc.CalculateParameterNignBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4299`
- Inputs: `CalculationData calculationdata`, `ClimateZones climateZone`, `MonthlyDays month`, `double gamma`, `Section section`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4300`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateaHbaseLine`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4303` `return (1.0 - Math.Pow(gamma, num)) / (1.0 - Math.Pow(gamma, num + 1.0));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4311` `return num / (num + 1.0);`

### HeatingAndCoolingResultCalc.CalculateParameterQtrBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4317`
- Inputs: `CalculationData claculationdata`, `Section section`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `averageInnerHeatTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4319`; `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4318`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4320`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4321`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4322`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4323`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempBaseLine`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempBaseLine`, `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHgCurrent`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4323` `double num4 = CalculateParameterHdCurrent(section) + CalculateParameterHgCurrent(section) + (num + num2 + num3);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4324` `return num4 * (CalcAvgProjectTempBaseLine(section, avgTemp, claculationdata, month) + CalcAvgNonProjectTempBaseLine(section, avgTemp, claculationdata, month)) / 1000.0;`

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTempBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4328`
- Inputs: `Section section`, `double averageMontlyTemp`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4329`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4330`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4331`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4332`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4329` `int num = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4330` `int num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4331` `int num3 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4332` `int num4 = month.Holydays * 24;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4333` `return (calculationData.NonProjectTemperatureBaseLine - averageMontlyTemp) * (double)(num + num2 + num3 + num4);`

### HeatingAndCoolingResultCalc.CalcAvgProjectTempBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4337`
- Inputs: `Section section`, `double averageMontlyTemp`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4338`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4339`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4340`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4338` `int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4339` `int num2 = month.Sundays * (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4340` `int num3 = month.Saturdays * (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4341` `return (calculationData.ProjectTemperatureBaseLine - averageMontlyTemp) * (double)(num + num3 + num2);`

### HeatingAndCoolingResultCalc.CalculateAverageHeatTempBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4345`
- Inputs: `Section section`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `nonProjectTemperatureBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4354`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4346`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4347`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4348`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4350`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4351`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4352`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4353`; `projectTemperatureBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4349`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4346` `int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4347` `num = ((month.Sundays > 0) ? (month.Sundays * (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4348` `num = ((month.Saturdays > 0) ? (month.Saturdays * (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4350` `int num2 = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4351` `num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4352` `num2 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4353` `num2 = month.Holydays * 24 + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4355` `return ((double)num * projectTemperatureBaseLine + (double)num2 * nonProjectTemperatureBaseLine) / (double)(num + num2);`

### HeatingAndCoolingResultCalc.CalculateParameterQveBaseLIne
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4359`
- Inputs: `Section section`, `CalculationData calculationData`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4360`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempBaseLine`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempBaseLine`, `HeatingAndCoolingResultCalc.CalcParameterHveBaseLine`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4361` `return CalcParameterHveBaseLine(section, calculationData) * (CalcAvgProjectTempBaseLine(section, avgTemp, calculationData, month) + CalcAvgNonProjectTempBaseLine(section, avgTemp, calculationData, month)) / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateParameterHtrBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4365`
- Inputs: `Section section`, `double averageMontlyTemp`, `double averageInnerHeatTemp`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4366`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4367`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4368`; `section.Test.ParameterHd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4370`; `section.Test.ParameterHg (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4371`; `section.Test.ParameterHu (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4369`
- Internal calls: `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3`, `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHgCurrent`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4369` `section.Test.ParameterHu = num + num2 + num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4372` `return section.Test.ParameterHd + section.Test.ParameterHg + section.Test.ParameterHu;`

### HeatingAndCoolingResultCalc.CalcParameterHveBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4376`
- Inputs: `Section section`, `CalculationData heatingAndCoolingCalculations`
- Outputs: returns `double`; writes `section.Test.ParameterHve (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4377`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4377` `section.Test.ParameterHve = section.Area.HeatedVolume * heatingAndCoolingCalculations.InfiltracionBaseLine * 0.34;`

### HeatingAndCoolingResultCalc.CalculateParameterQgnBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4382`
- Inputs: `Section section`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4383`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4384`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4385`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4386`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4387`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4388`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4389`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateNonTrasparentFsol`, `HeatingAndCoolingResultCalc.CalculateTrasparentFsol`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4383` `int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4384` `num = ((month.Sundays > 0) ? (month.Sundays * (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4385` `num = ((month.Saturdays > 0) ? (month.Saturdays * (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4386` `int num2 = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4387` `num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4388` `num2 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4389` `num2 = month.Holydays * 24 + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4390` `return (CalculateNonTrasparentFsol(section, climateZone, month) + CalculateTrasparentFsol(section, climateZone, month)) * (double)(num + num2);`

### HeatingAndCoolingResultCalc.CalculateParameterQveRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4421`
- Inputs: `Section section`, `ClimateZones climateZone`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempRef1`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempRef1`, `HeatingAndCoolingResultCalc.CalcParameterHveRef1`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4422` `return CalcParameterHveRef1(section, calculationData) * (CalcAvgProjectTempRef1(section, climateZone, calculationData, month) + CalcAvgNonProjectTempRef1(section, climateZone, calculationData, month)) / 1000.0;`

### HeatingAndCoolingResultCalc.CalcParameterHveRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4426`
- Inputs: `Section section`, `CalculationData heatingAndCoolingCalculations`
- Outputs: returns `double`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4427` `return section.Area.HeatedVolume * heatingAndCoolingCalculations.InfiltracionRef1 * 0.34;`

### HeatingAndCoolingResultCalc.CalculateParameterQtrRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4431`
- Inputs: `CalculationData calculationdata`, `Section tempSection`, `ClimateZones climateZone`, `MonthlyDays month`, `out double parameterHtr`
- Outputs: returns `double`; writes `averageInnerHeatTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4433`; `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4432`; `parameterHtr (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4434`; `tempSection.Test.ParameterHtr (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4435`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempRef1`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempRef1`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempRef1`, `HeatingAndCoolingResultCalc.CalculateParameterHtr`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4436` `return tempSection.Test.ParameterHtr * (CalcAvgProjectTempRef1(tempSection, climateZone, calculationdata, month) + CalcAvgNonProjectTempRef1(tempSection, climateZone, calculationdata, month)) / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateParameterHtrRef
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4440`
- Inputs: `Section tempSection`, `double averageMontlyTemp`, `double averageInnerHeatTemp`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4441`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4442`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4443`; `tempSection.Test.ParameterHd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4445`; `tempSection.Test.ParameterHg (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4446`; `tempSection.Test.ParameterHu (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4444`
- Internal calls: `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2`, `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3`, `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent`, `HeatingAndCoolingResultCalc.CalculateParameterHgCurrent`, `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4444` `tempSection.Test.ParameterHu = num + num2 + num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4447` `return tempSection.Test.ParameterHd + tempSection.Test.ParameterHg + tempSection.Test.ParameterHu;`

### HeatingAndCoolingResultCalc.CalculateAverageHeatTempRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4451`
- Inputs: `Section section`, `CalculationData calculationdata`, `MonthlyDays month`
- Outputs: returns `double`; writes `nonProjectTemperatureRef (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4460`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4452`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4453`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4454`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4456`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4457`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4458`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4459`; `projectTemperatureRef (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4455`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4452` `int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4453` `num = ((month.Sundays > 0) ? (month.Sundays * (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunCurrentStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4454` `num = ((month.Saturdays > 0) ? (month.Saturdays * (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SunCurrentStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4456` `int num2 = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4457` `num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4458` `num2 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4459` `num2 = month.Holydays * 24 + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4461` `return ((double)num * projectTemperatureRef + (double)num2 * nonProjectTemperatureRef) / (double)(num + num2);`

### HeatingAndCoolingResultCalc.CalcAvgProjectTempRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4465`
- Inputs: `Section section`, `ClimateZones climateZone`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4466`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4467`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4468`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4469`
- Internal calls: `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4467` `int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4468` `int num2 = month.Sundays * (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4469` `int num3 = month.Saturdays * (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4470` `return (calculationData.ProjectTemperatureRef1 - avgTemp) * (double)(num + num3 + num2);`

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTempRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4474`
- Inputs: `Section section`, `ClimateZones climateZone`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4475`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4476`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4477`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4478`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4479`
- Internal calls: `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4476` `int num = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4477` `int num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4478` `int num3 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4479` `int num4 = month.Holydays * 24;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4480` `return (calculationData.NonProjectTemperatureRef1 - avgTemp) * (double)(num + num2 + num3 + num4);`

### HeatingAndCoolingResultCalc.CalculateaHref1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4484`
- Inputs: `CalculationData calculationdata`, `Section tempSection`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `averageInnerHeatTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4486`; `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4485`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4487`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4488`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4489`
- Internal calls: `HeatingAndCoolingResultCalc.CalcParameterHveRef1`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempRef1`, `HeatingAndCoolingResultCalc.CalculateParameterHtr`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4489` `double num3 = tempSection.Area.HeatedArea * tempSection.Area.HeatCapacity / (num + num2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4490` `return 1.0 + num3 / 15.0;`

### HeatingAndCoolingResultCalc.CalculateParameterNignRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4494`
- Inputs: `CalculationData calculationdata`, `ClimateZones climateZone`, `MonthlyDays month`, `double gamma`, `Section section`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4495`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateaHref1`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4498` `return (1.0 - Math.Pow(gamma, num)) / (1.0 - Math.Pow(gamma, num + 1.0));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4506` `return num / (num + 1.0);`

### HeatingAndCoolingResultCalc.CalculateParameterQveRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4679`
- Inputs: `Section section`, `CalculationData calculationData`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4680`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempRef2`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempRef2`, `HeatingAndCoolingResultCalc.CalcParameterHveRef2`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4681` `return CalcParameterHveRef2(section, calculationData) * (CalcAvgProjectTempRef2(section, climateZone, calculationData, month) + CalcAvgNonProjectTempRef2(section, climateZone, calculationData, month)) / 1000.0;`

### HeatingAndCoolingResultCalc.CalcParameterHveRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4685`
- Inputs: `Section section`, `CalculationData heatingAndCoolingCalculations`
- Outputs: returns `double`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4686` `return section.Area.HeatedVolume * heatingAndCoolingCalculations.InfiltracionRef2 * 0.34;`

### HeatingAndCoolingResultCalc.CalculateParameterQtrRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4690`
- Inputs: `CalculationData calculationdata`, `Section tempSection`, `ClimateZones climateZone`, `MonthlyDays month`, `out double parameterHtr`
- Outputs: returns `double`; writes `averageInnerHeatTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4692`; `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4691`; `parameterHtr (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4693`; `tempSection.Test.ParameterHtr (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4694`
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgNonProjectTempRef2`, `HeatingAndCoolingResultCalc.CalcAvgProjectTempRef2`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempRef2`, `HeatingAndCoolingResultCalc.CalculateParameterHtr`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4695` `return tempSection.Test.ParameterHtr * (CalcAvgProjectTempRef2(tempSection, climateZone, calculationdata, month) + CalcAvgNonProjectTempRef2(tempSection, climateZone, calculationdata, month)) / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateAverageHeatTempRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4699`
- Inputs: `Section section`, `CalculationData calculationdata`, `MonthlyDays month`
- Outputs: returns `double`; writes `nonProjectTemperatureRef (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4708`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4700`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4701`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4702`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4704`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4705`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4706`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4707`; `projectTemperatureRef (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4703`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4700` `int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4701` `num = ((month.Sundays > 0) ? (month.Sundays * (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4702` `num = ((month.Saturdays > 0) ? (month.Saturdays * (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart) + num) : num);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4704` `int num2 = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4705` `num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4706` `num2 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart)) + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4707` `num2 = month.Holydays * 24 + num2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4709` `return ((double)num * projectTemperatureRef + (double)num2 * nonProjectTemperatureRef) / (double)(num + num2);`

### HeatingAndCoolingResultCalc.CalcAvgProjectTempRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4713`
- Inputs: `Section section`, `ClimateZones climateZone`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4714`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4715`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4716`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4717`
- Internal calls: `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4715` `int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4716` `int num2 = month.Sundays * (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4717` `int num3 = month.Saturdays * (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4718` `return (calculationData.ProjectTemperatureRef2 - avgTemp) * (double)(num + num3 + num2);`

### HeatingAndCoolingResultCalc.CalcAvgNonProjectTempRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4722`
- Inputs: `Section section`, `ClimateZones climateZone`, `CalculationData calculationData`, `MonthlyDays month`
- Outputs: returns `double`; writes `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4723`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4724`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4725`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4726`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4727`
- Internal calls: `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4724` `int num = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4725` `int num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4726` `int num3 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4727` `int num4 = month.Holydays * 24;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4728` `return (calculationData.NonProjectTemperatureRef2 - avgTemp) * (double)(num + num2 + num3 + num4);`

### HeatingAndCoolingResultCalc.CalculateaHref2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4732`
- Inputs: `CalculationData calculationdata`, `Section tempSection`, `ClimateZones climateZone`, `MonthlyDays month`
- Outputs: returns `double`; writes `averageInnerHeatTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4734`; `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4733`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4735`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4736`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4737`
- Internal calls: `HeatingAndCoolingResultCalc.CalcParameterHveRef2`, `HeatingAndCoolingResultCalc.CalculateAverageHeatTempRef2`, `HeatingAndCoolingResultCalc.CalculateParameterHtrRef`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4737` `double num3 = tempSection.Area.HeatedArea * tempSection.Area.HeatCapacity / (num + num2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4738` `return 1.0 + num3 / 15.0;`

### HeatingAndCoolingResultCalc.CalculateParameterNignRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4742`
- Inputs: `CalculationData calculationdata`, `ClimateZones climateZone`, `MonthlyDays month`, `double gamma`, `Section section`
- Outputs: returns `double`; writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4743`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateaHref2`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4746` `return (1.0 - Math.Pow(gamma, num)) / (1.0 - Math.Pow(gamma, num + 1.0));`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:4754` `return num / (num + 1.0);`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5092`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.Lights.Heating.DevicesNeededEnergyRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5095`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5094`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5093`
- Internal calls: `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5094` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5095` `calcData.Lights.Heating.DevicesNeededEnergyRef1 = calcData.Lights.Heating.WorkScheduleRef1 * calcData.Lights.Heating.PowerRef1 * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5099`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.Lights.Heating.DevicesNeededEnergyRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5102`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5101`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5100`
- Internal calls: `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5101` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5102` `calcData.Lights.Heating.DevicesNeededEnergyRef2 = calcData.Lights.Heating.WorkScheduleRef2 * calcData.Lights.Heating.PowerRef2 * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5134`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.Lights.Heating.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5157`; `calcData.Lights.Heating.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5168`; `calcData.Lights.Heating.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5173`; `calcData.Lights.Heating.PowerActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5160`; `calcData.Lights.Heating.WorkScheduleActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5164`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5135`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5136`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5138`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5144`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5137`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5143` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5144` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5152` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5153` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5155` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5156` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5157` `calcData.Lights.Heating.DevicesNeededEnergyActual = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5164` `calcData.Lights.Heating.WorkScheduleActual = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5173` `calcData.Lights.Heating.DevicesNeededEnergyActual = calcData.Lights.Heating.WorkScheduleActual * calcData.Lights.Heating.PowerActual * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5266`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.Lights.Heating.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5289`; `calcData.Lights.Heating.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5300`; `calcData.Lights.Heating.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5305`; `calcData.Lights.Heating.PowerBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5292`; `calcData.Lights.Heating.WorkScheduleBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5296`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5267`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5268`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5270`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5276`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5269`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5275` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5276` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5284` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5285` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5287` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5288` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5289` `calcData.Lights.Heating.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5296` `calcData.Lights.Heating.WorkScheduleBaseLine = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5305` `calcData.Lights.Heating.DevicesNeededEnergyBaseLine = calcData.Lights.Heating.WorkScheduleBaseLine * calcData.Lights.Heating.PowerBaseLine * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5398`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.Lights.Heating.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5421`; `calcData.Lights.Heating.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5432`; `calcData.Lights.Heating.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5437`; `calcData.Lights.Heating.PowerESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5424`; `calcData.Lights.Heating.WorkScheduleESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5428`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5399`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5400`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5402`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5408`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5401`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5407` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5408` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5416` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5417` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5419` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5420` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5421` `calcData.Lights.Heating.DevicesNeededEnergyESM = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5428` `calcData.Lights.Heating.WorkScheduleESM = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5437` `calcData.Lights.Heating.DevicesNeededEnergyESM = calcData.Lights.Heating.WorkScheduleESM * calcData.Lights.Heating.PowerESM * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1Balanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5530`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.BalancedDevices.Heating.DevicesNeededEnergyRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5533`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5532`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5531`
- Internal calls: `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5532` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5533` `calcData.BalancedDevices.Heating.DevicesNeededEnergyRef1 = calcData.BalancedDevices.Heating.WorkScheduleRef1 * calcData.BalancedDevices.Heating.PowerRef1 * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2Balanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5551`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.BalancedDevices.Heating.DevicesNeededEnergyRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5554`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5553`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5552`
- Internal calls: `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5553` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5554` `calcData.BalancedDevices.Heating.DevicesNeededEnergyRef2 = calcData.BalancedDevices.Heating.WorkScheduleRef2 * calcData.BalancedDevices.Heating.PowerRef2 * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5572`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.BalancedDevices.Heating.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5595`; `calcData.BalancedDevices.Heating.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5606`; `calcData.BalancedDevices.Heating.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5611`; `calcData.BalancedDevices.Heating.PowerActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5598`; `calcData.BalancedDevices.Heating.WorkScheduleActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5602`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5573`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5574`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5576`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5582`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5575`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5581` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5582` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5590` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5591` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5593` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5594` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5595` `calcData.BalancedDevices.Heating.DevicesNeededEnergyActual = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5602` `calcData.BalancedDevices.Heating.WorkScheduleActual = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5611` `calcData.BalancedDevices.Heating.DevicesNeededEnergyActual = calcData.BalancedDevices.Heating.WorkScheduleActual * calcData.BalancedDevices.Heating.PowerActual * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5704`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.BalancedDevices.Heating.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5727`; `calcData.BalancedDevices.Heating.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5738`; `calcData.BalancedDevices.Heating.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5743`; `calcData.BalancedDevices.Heating.PowerBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5730`; `calcData.BalancedDevices.Heating.WorkScheduleBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5734`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5705`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5706`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5708`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5714`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5707`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5713` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5714` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5722` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5723` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5725` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5726` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5727` `calcData.BalancedDevices.Heating.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5734` `calcData.BalancedDevices.Heating.WorkScheduleBaseLine = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5743` `calcData.BalancedDevices.Heating.DevicesNeededEnergyBaseLine = calcData.BalancedDevices.Heating.WorkScheduleBaseLine * calcData.BalancedDevices.Heating.PowerBaseLine * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5836`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.BalancedDevices.Heating.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5859`; `calcData.BalancedDevices.Heating.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5870`; `calcData.BalancedDevices.Heating.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5875`; `calcData.BalancedDevices.Heating.PowerESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5862`; `calcData.BalancedDevices.Heating.WorkScheduleESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5866`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5837`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5838`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5840`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5846`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5839`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5845` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5846` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5854` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5855` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5857` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5858` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5859` `calcData.BalancedDevices.Heating.DevicesNeededEnergyESM = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5866` `calcData.BalancedDevices.Heating.WorkScheduleESM = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5875` `calcData.BalancedDevices.Heating.DevicesNeededEnergyESM = calcData.BalancedDevices.Heating.WorkScheduleESM * calcData.BalancedDevices.Heating.PowerESM * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1NonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5968`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.NonBalancedDevices.Heating.DevicesNeededEnergyRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5971`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5970`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5969`
- Internal calls: `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5970` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5971` `calcData.NonBalancedDevices.Heating.DevicesNeededEnergyRef1 = calcData.NonBalancedDevices.Heating.WorkScheduleRef1 * calcData.NonBalancedDevices.Heating.PowerRef1 * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2NonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5975`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.NonBalancedDevices.Heating.DevicesNeededEnergyRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5978`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5977`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5976`
- Internal calls: `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5977` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:5978` `calcData.NonBalancedDevices.Heating.DevicesNeededEnergyRef2 = calcData.NonBalancedDevices.Heating.WorkScheduleRef2 * calcData.NonBalancedDevices.Heating.PowerRef2 * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualNonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6010`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.NonBalancedDevices.Heating.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6033`; `calcData.NonBalancedDevices.Heating.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6036`; `calcData.NonBalancedDevices.Heating.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6041`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6011`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6012`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6014`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6020`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6013`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6028`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6024`; ... (+5)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6019` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6020` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6028` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6029` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6031` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6032` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6033` `calcData.NonBalancedDevices.Heating.DevicesNeededEnergyActual = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6041` `calcData.NonBalancedDevices.Heating.DevicesNeededEnergyActual = calcData.NonBalancedDevices.Heating.WorkScheduleActual * calcData.NonBalancedDevices.Heating.PowerActual * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineNonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6118`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.NonBalancedDevices.Heating.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6141`; `calcData.NonBalancedDevices.Heating.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6152`; `calcData.NonBalancedDevices.Heating.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6157`; `calcData.NonBalancedDevices.Heating.PowerBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6144`; `calcData.NonBalancedDevices.Heating.WorkScheduleBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6148`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6119`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6120`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6122`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6128`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6121`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6127` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6128` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6136` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6137` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6139` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6140` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6141` `calcData.NonBalancedDevices.Heating.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6148` `calcData.NonBalancedDevices.Heating.WorkScheduleBaseLine = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6157` `calcData.NonBalancedDevices.Heating.DevicesNeededEnergyBaseLine = calcData.NonBalancedDevices.Heating.WorkScheduleBaseLine * calcData.NonBalancedDevices.Heating.PowerBaseLine * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMNonBalanced
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6250`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.NonBalancedDevices.Heating.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6273`; `calcData.NonBalancedDevices.Heating.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6284`; `calcData.NonBalancedDevices.Heating.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6289`; `calcData.NonBalancedDevices.Heating.PowerESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6276`; `calcData.NonBalancedDevices.Heating.WorkScheduleESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6280`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6251`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6252`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6254`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6260`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6253`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6259` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6260` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6268` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6269` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6271` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6272` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6273` `calcData.NonBalancedDevices.Heating.DevicesNeededEnergyESM = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6280` `calcData.NonBalancedDevices.Heating.WorkScheduleESM = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6289` `calcData.NonBalancedDevices.Heating.DevicesNeededEnergyESM = calcData.NonBalancedDevices.Heating.WorkScheduleESM * calcData.NonBalancedDevices.Heating.PowerESM * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef1HotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6382`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.HotWaterPumps.Heating.DevicesNeededEnergyRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6385`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6384`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6383`
- Internal calls: `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6384` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6385` `calcData.HotWaterPumps.Heating.DevicesNeededEnergyRef1 = calcData.HotWaterPumps.Heating.WorkScheduleRef1 * calcData.HotWaterPumps.Heating.PowerRef1 * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodRef2HotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6403`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.HotWaterPumps.Heating.DevicesNeededEnergyRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6406`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6405`; `source (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6404`
- Internal calls: `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6405` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6406` `calcData.HotWaterPumps.Heating.DevicesNeededEnergyRef2 = calcData.HotWaterPumps.Heating.WorkScheduleRef2 * calcData.HotWaterPumps.Heating.PowerRef2 * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodActualHotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6424`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.HotWaterPumps.Heating.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6447`; `calcData.HotWaterPumps.Heating.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6450`; `calcData.HotWaterPumps.Heating.DevicesNeededEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6455`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6425`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6426`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6428`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6434`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6427`; `num2 (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6442`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6438`; ... (+5)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6433` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6434` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6442` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6443` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6445` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6446` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6447` `calcData.HotWaterPumps.Heating.DevicesNeededEnergyActual = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6455` `calcData.HotWaterPumps.Heating.DevicesNeededEnergyActual = calcData.HotWaterPumps.Heating.WorkScheduleActual * calcData.HotWaterPumps.Heating.PowerActual * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodBaseLineHotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6532`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.HotWaterPumps.Heating.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6555`; `calcData.HotWaterPumps.Heating.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6566`; `calcData.HotWaterPumps.Heating.DevicesNeededEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6571`; `calcData.HotWaterPumps.Heating.PowerBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6558`; `calcData.HotWaterPumps.Heating.WorkScheduleBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6562`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6533`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6534`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6536`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6542`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6535`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6541` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6542` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6550` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6551` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6553` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6554` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6555` `calcData.HotWaterPumps.Heating.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6562` `calcData.HotWaterPumps.Heating.WorkScheduleBaseLine = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6571` `calcData.HotWaterPumps.Heating.DevicesNeededEnergyBaseLine = calcData.HotWaterPumps.Heating.WorkScheduleBaseLine * calcData.HotWaterPumps.Heating.PowerBaseLine * num / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateHeatingPeriodESMHotWaterPumps
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6664`
- Inputs: `this CalculationData calcData`, `Section section`
- Outputs: writes `calcData.HotWaterPumps.Heating.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6687`; `calcData.HotWaterPumps.Heating.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6698`; `calcData.HotWaterPumps.Heating.DevicesNeededEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6703`; `calcData.HotWaterPumps.Heating.DevicesNeededEnergySavings (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6704`; `calcData.HotWaterPumps.Heating.PowerESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6690`; `calcData.HotWaterPumps.Heating.WorkScheduleESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6694`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6665`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6666`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6668`; `num (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6674`; ... (+8)
- Internal calls: `HeatingAndCoolingResultCalc.CalcAvgMonthPower`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6673` `list2.Add(weekRegime * weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6674` `num += weeks;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6682` `num2 += list[i] * list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6683` `num3 += list2[i];`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6685` `double num4 = num2 / num3;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6686` `double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6687` `calcData.HotWaterPumps.Heating.DevicesNeededEnergyESM = num4 * num5 / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6694` `calcData.HotWaterPumps.Heating.WorkScheduleESM = num5 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6703` `calcData.HotWaterPumps.Heating.DevicesNeededEnergyESM = calcData.HotWaterPumps.Heating.WorkScheduleESM * calcData.HotWaterPumps.Heating.PowerESM * num / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6704` `calcData.HotWaterPumps.Heating.DevicesNeededEnergySavings = (calcData.HotWaterPumps.Heating.DevicesNeededEnergyBaseLine - calcData.HotWaterPumps.Heating.DevicesNeededEnergyESM).ToString("F3");`

### HeatingAndCoolingResultCalc.CalculateBuildingHeatingPower
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6864`
- Inputs: `CalculationInput calcInput`, `Results buildingBalanceResult`
- Outputs: writes `buildingBalanceResult.PowerBudgetTable.Heating.Actual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6868`; `buildingBalanceResult.PowerBudgetTable.Heating.ActualArea (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6869`; `buildingBalanceResult.PowerBudgetTable.Heating.BaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6870`; `buildingBalanceResult.PowerBudgetTable.Heating.BaseLineArea (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6871`; `buildingBalanceResult.PowerBudgetTable.Heating.ESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6872`; `buildingBalanceResult.PowerBudgetTable.Heating.ESMArea (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6873`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6867`
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6867` `double num = calcInput.BuildingZones.Sum((BuildingZone buildingZone) => buildingZone.Heating.Area.HeatedArea);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6868` `buildingBalanceResult.PowerBudgetTable.Heating.Actual = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Heating.Actual);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6869` `buildingBalanceResult.PowerBudgetTable.Heating.ActualArea = buildingBalanceResult.PowerBudgetTable.Heating.Actual * 1000.0 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6870` `buildingBalanceResult.PowerBudgetTable.Heating.BaseLine = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Heating.BaseLine);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6871` `buildingBalanceResult.PowerBudgetTable.Heating.BaseLineArea = buildingBalanceResult.PowerBudgetTable.Heating.BaseLine * 1000.0 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6872` `buildingBalanceResult.PowerBudgetTable.Heating.ESM = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Heating.ESM);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6873` `buildingBalanceResult.PowerBudgetTable.Heating.ESMArea = buildingBalanceResult.PowerBudgetTable.Heating.ESM * 1000.0 / num;`

### HeatingAndCoolingResultCalc.CalculateHeatingPower
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6915`
- Inputs: `this CalculationData calcData`, `Section section`, `BuildingZone zone`, `Results results`
- Outputs: writes `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6926`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6927`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6930`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6931`; `num5 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6934`; `num6 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6935`; `results.PowerBudgetTable.Heating.Actual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6918`; `results.PowerBudgetTable.Heating.Actual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6928`; `results.PowerBudgetTable.Heating.ActualArea (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6919`; `results.PowerBudgetTable.Heating.ActualArea (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6929`; ... (+8)
- Internal calls: `HeatingAndCoolingResultCalc.CalcParameterHve`, `HeatingAndCoolingResultCalc.CalcParameterHveBaseLine`, `HeatingAndCoolingResultCalc.CalcParameterHveEsm`, `HeatingAndCoolingResultCalc.CalculateParameterHtr`, `HeatingAndCoolingResultCalc.CalculateParameterHtrBaseLine`, `HeatingAndCoolingResultCalc.CalculateParameterHtrEsm`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6928` `results.PowerBudgetTable.Heating.Actual = (num + num2) * (calcData.ProjectTemperatureActual - results.PowerBudgetTable.Heating.CalculateTemperature) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6929` `results.PowerBudgetTable.Heating.ActualArea = results.PowerBudgetTable.Heating.Actual * 1000.0 / section.Area.HeatedArea;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6932` `results.PowerBudgetTable.Heating.BaseLine = (num3 + num4) * (calcData.ProjectTemperatureBaseLine - results.PowerBudgetTable.Heating.CalculateTemperature) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6933` `results.PowerBudgetTable.Heating.BaseLineArea = results.PowerBudgetTable.Heating.BaseLine * 1000.0 / section.Area.HeatedArea;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6936` `results.PowerBudgetTable.Heating.ESM = (num5 + num6) * (calcData.ProjectTemperatureESM - results.PowerBudgetTable.Heating.CalculateTemperature) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:6937` `results.PowerBudgetTable.Heating.ESMArea = results.PowerBudgetTable.Heating.ESM * 1000.0 / section.Area.HeatedArea;`

### HeatingAndCoolingResultCalc.GetVeiHeating
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8871`
- Inputs: `Results zoneBalanceResult`, `Fuel fuel`, `double efficiency`, `double quantity`, `double area`
- Outputs: writes `zoneBalanceResult.NeededEnergyTable.Heating.GeneralVEI (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8875`; `zoneBalanceResult.NeededEnergyTable.Heating.GeneralVEI (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8879`; `zoneBalanceResult.NeededEnergyTable.Heating.GeneralVEI (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8883`; `zoneBalanceResult.NeededEnergyTable.Heating.VEI (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8878`; `zoneBalanceResult.NeededEnergyTable.Heating.VEI (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8882`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateElectricityVEI`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8875` `zoneBalanceResult.NeededEnergyTable.Heating.GeneralVEI += CalculateElectricityVEI(efficiency, quantity) * area;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8878` `zoneBalanceResult.NeededEnergyTable.Heating.VEI += quantity * area;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8879` `zoneBalanceResult.NeededEnergyTable.Heating.GeneralVEI += quantity * area;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8882` `zoneBalanceResult.NeededEnergyTable.Heating.VEI += quantity * area;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8883` `zoneBalanceResult.NeededEnergyTable.Heating.GeneralVEI += quantity * area;`

### HeatingAndCoolingResultCalc.GetVeiHeatVentilation
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8889`
- Inputs: `Results zoneBalanceResult`, `Fuel fuel`, `double efficiency`, `double quantity`, `double heatedArea`
- Outputs: writes `zoneBalanceResult.NeededEnergyTable.HeatingVentilation.GeneralVEI (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8893`; `zoneBalanceResult.NeededEnergyTable.HeatingVentilation.GeneralVEI (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8897`; `zoneBalanceResult.NeededEnergyTable.HeatingVentilation.GeneralVEI (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8901`; `zoneBalanceResult.NeededEnergyTable.HeatingVentilation.VEI (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8896`; `zoneBalanceResult.NeededEnergyTable.HeatingVentilation.VEI (+=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8900`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateElectricityVEI`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8893` `zoneBalanceResult.NeededEnergyTable.HeatingVentilation.GeneralVEI += CalculateElectricityVEI(efficiency, quantity) * heatedArea;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8896` `zoneBalanceResult.NeededEnergyTable.HeatingVentilation.VEI += quantity * heatedArea;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8897` `zoneBalanceResult.NeededEnergyTable.HeatingVentilation.GeneralVEI += quantity * heatedArea;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8900` `zoneBalanceResult.NeededEnergyTable.HeatingVentilation.VEI += quantity * heatedArea;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:8901` `zoneBalanceResult.NeededEnergyTable.HeatingVentilation.GeneralVEI += quantity * heatedArea;`

### HeatingAndCoolingResultCalc.CalculateHeatingSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11208`
- Inputs: `this CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `BuildingZone zone`, `CalculationData lightsAndDevicesCalculationData`
- Outputs: writes `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11219`; `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11232`; `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11252`; `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11259`; `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11279`; `dataRow (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11236`; `dataRow (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11263`; `dataRow.Value (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11249`; `dataRow.Value (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11276`; `dataRow2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11256`; ... (+25)
- Internal calls: `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CalculateEnergy`, `HeatingAndCoolingResultCalc.CalculateEnergyESM`, `HeatingAndCoolingResultCalc.CalculateUsavingType`, `HeatingAndCoolingResultCalc.CalculateUsavingTypeESM`, `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSources`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSourcesESM`, `HeatingAndCoolingResultCalc.CheckForFuelSavings`, `HeatingAndCoolingResultCalc.CheckForSavings`, `HeatingAndCoolingResultCalc.CopyHeatingWorkingSchedule`, `HeatingAndCoolingResultCalc.CopyHeatingWorkingScheduleESM`, `HeatingAndCoolingResultCalc.CreateHeatingVirtualBaseLine`, `HeatingAndCoolingResultCalc.CreateHeatingVirtualESM`, `HeatingAndCoolingResultCalc.GetBaseLine`, `HeatingAndCoolingResultCalc.GetESM`, `HeatingAndCoolingResultCalc.SetBaseLine`, `HeatingAndCoolingResultCalc.SetESM`, `HeatingAndCoolingResultCalc.SetSavingsValues`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11258` `saving.Saving = virtualBaseLineNetEnergy - saving.NetEnergy;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11285` `saving.SavingNMinusOne = saving.NetEnergyNMinusOne - virtualESMNetEnergy;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11287` `double num = list.Sum((SavingsData o) => o.Saving);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11290` `item.Part = item.Saving / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11292` `double num2 = virtualBaseLineNetEnergy - virtualESMNetEnergy;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11293` `double num3 = num2 / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11296` `item2.ActualSaving = num2 * (item2.Saving / num2 * num3 + item2.SavingNMinusOne / num2) / 2.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11298` `double num4 = list.Sum((SavingsData o) => o.ActualSaving);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11299` `double num5 = (virtualBaseLineNetEnergy - virtualESMNetEnergy) / num4;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11302` `item3.ActualSaving *= num5;`

### HeatingAndCoolingResultCalc.CreateHeatingVirtualBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11410`
- Inputs: `CalculationData tempCalculationdata`, `Section section`, `CalculationInput calcInput`, `BuildingZone zone`, `CalculationData lightsAndDevicesCalculationData`
- Outputs: returns `List<DataRow>`; writes `baseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11411`; `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11427`; `dataRow (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11412`; `dataRow.Value (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11415`; `dataRow2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11417`; `dataRow2.Value (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11420`; `dataRow3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11422`; `dataRow3.Value (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11425`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateEnergy`, `HeatingAndCoolingResultCalc.GetBaseLine`, `HeatingAndCoolingResultCalc.SetBaseLine`
- Formulas: _None extracted_

### HeatingAndCoolingResultCalc.CreateHeatingVirtualESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11433`
- Inputs: `CalculationData tempCalculationdata`, `Section section`, `CalculationInput calcInput`, `BuildingZone zone`, `CalculationData lightsAndDevicesCalculationData`
- Outputs: returns `List<DataRow>`; writes `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11450`; `dataRow (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11435`; `dataRow.Value (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11438`; `dataRow2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11440`; `dataRow2.Value (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11443`; `dataRow3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11445`; `dataRow3.Value (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11448`; `eSM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11434`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateEnergy`, `HeatingAndCoolingResultCalc.GetESM`, `HeatingAndCoolingResultCalc.SetESM`
- Formulas: _None extracted_

### HeatingAndCoolingResultCalc.CalculateVentilationHeatingSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11596`
- Inputs: `this CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `BuildingZone zone`, `HeatingCalculations heatCalculations`
- Outputs: writes `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11607`; `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11618`; `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11634`; `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11649`; `calculationData (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11668`; `dataRow (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11622`; `dataRow.Value (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11631`; `dataRow2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11640`; `dataRow3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11655`; `dataRow3.Value (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11664`; ... (+18)
- Internal calls: `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CalculateGeneratorHeatEfficiencyBaseLine`, `HeatingAndCoolingResultCalc.CalculateVentNeededEnergyBaseLine`, `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings`, `HeatingAndCoolingResultCalc.CheckForDifferentFuelSources`, `HeatingAndCoolingResultCalc.CheckForFuelSavings`, `HeatingAndCoolingResultCalc.CheckForVentilationSavings`, `HeatingAndCoolingResultCalc.CopyVentilationHeatingWorkingSchedule`, `HeatingAndCoolingResultCalc.GetVentilationBaseLine`, `HeatingAndCoolingResultCalc.SetVentilationBaseLine`, `HeatingAndCoolingResultCalc.SetVentilationSavingsValues`, `HeatingAndCoolingResultCalc.VentilationHeatEnergyBaseLine`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11608` `IList<SavingsData> list = CheckForVentilationSavings("Вентилация - Отопление");`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11642` `saving.Saving = virtualBaseLineNetEnergy - saving.NetEnergy;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11644` `double num = list.Sum((SavingsData o) => o.Saving);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11647` `item.Part = item.Saving / num;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11678` `double num2 = resultNeededEnergyBaseLine - dataRow4.Value;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11681` `item2.ActualSaving = num2 * item2.Part;`

### HeatingAndCoolingResultCalc.CalculateFansAndPumpsHeatingSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11792`
- Inputs: `this HeatingCalculations calc`, `Section section`, `BuildingZone zone`
- Outputs: writes `fansAndPumnps (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11793`; `item.ActualSaving (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11808`; `item.ActualSaving (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11815`; `item.ActualSaving (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11822`; `item.ActualSaving (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11829`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11794`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11799`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11827`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11828`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11820`; ... (+6)
- Internal calls: `HeatingAndCoolingResultCalc.AddSavingsToZone`, `HeatingAndCoolingResultCalc.CheckAndCalculateNegativeSavings`, `HeatingAndCoolingResultCalc.CheckHeatingForFansAndPumpsSavings`, `HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekHeatingSeasonHoursEsm`, `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursBaseLine`, `HeatingAndCoolingResultCalc.GetWeekHeatingVentilationHoursEsm`, `HeatingAndCoolingResultCalc.SetHeatingFansAndPumpsSavingsValues`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11794` `IList<SavingsData> list = CheckHeatingForFansAndPumpsSavings("Помпи и вентилатори - Отопление");`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11799` `double num = source.Sum((MonthlyDays month) => month.Weeks);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11806` `double num8 = calc.FansAndPumps.VentilatorsHeatBaseLine * GetWeekHeatingVentilationHoursBaseLine(section) * num / (calc.FansAndPumps.EnergyManagementBaseLine / 100.0) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11807` `double num9 = calc.FansAndPumps.VentilatorsHeatESM * GetWeekHeatingVentilationHoursEsm(section) * num / (calc.FansAndPumps.EnergyManagementESM / 100.0) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11808` `item.ActualSaving = num8 - num9;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11813` `double num6 = calc.FansAndPumps.PumpVentilationBaseLine * GetWeekHeatingVentilationHoursBaseLine(section) * num / (calc.FansAndPumps.EnergyManagementBaseLine / 100.0) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11814` `double num7 = calc.FansAndPumps.PumpVentilationESM * GetWeekHeatingVentilationHoursEsm(section) * num / (calc.FansAndPumps.EnergyManagementESM / 100.0) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11815` `item.ActualSaving = num6 - num7;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11820` `double num4 = calc.FansAndPumps.PumpHeatingBaseLine * 24.0 * 7.0 * num / (calc.FansAndPumps.EnergyManagementBaseLine / 100.0) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11821` `double num5 = calc.FansAndPumps.PumpHeatingESM * 24.0 * 7.0 * num / (calc.FansAndPumps.EnergyManagementESM / 100.0) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11822` `item.ActualSaving = num4 - num5;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11827` `double num2 = calc.FansAndPumps.PumpHeatingBaseLine * GetWeekHeatingSeasonHoursBaseLine(section) * num / (calc.FansAndPumps.EnergyManagementBaseLine / 100.0) / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11828` `double num3 = calc.FansAndPumps.PumpHeatingESM * GetWeekHeatingSeasonHoursEsm(section) * num / (calc.FansAndPumps.EnergyManagementESM / 100.0) / 1000.0;`

### HeatingAndCoolingResultCalc.SetHeatingFansAndPumpsSavingsValues
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11844`
- Inputs: `IList<SavingsData> savings`
- Outputs: writes `fansAndPumnps.EnergyManagementSavings (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11848`; `fansAndPumnps.PumpHeatingSavings (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11847`; `fansAndPumnps.PumpVentilationSavings (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11846`; `fansAndPumnps.VentilatorsHeatSavings (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11845`
- Internal calls: `HeatingAndCoolingResultCalc.GetSaving`
- Formulas: _None extracted_

### HeatingAndCoolingResultCalc.CheckHeatingForFansAndPumpsSavings
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11852`
- Inputs: `string technology`
- Outputs: returns `IList<SavingsData>`; writes `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:11853`
- Internal calls: _None_
- Formulas: _None extracted_

### HeatingAndCoolingResultCalc.CopyHeatingWorkingSchedule
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13972`
- Inputs: `Section tempSection`, `Section section`
- Outputs: writes `tempSection.HeatingSeasons.Heating.SatBaseEnd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13976`; `tempSection.HeatingSeasons.Heating.SatBaseStart (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13975`; `tempSection.HeatingSeasons.Heating.SunBaseEnd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13978`; `tempSection.HeatingSeasons.Heating.SunBaseStart (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13977`; `tempSection.HeatingSeasons.Heating.WorkBaseEnd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13974`; `tempSection.HeatingSeasons.Heating.WorkBaseStart (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13973`
- Internal calls: _None_
- Formulas: _None extracted_

### HeatingAndCoolingResultCalc.CopyHeatingWorkingScheduleESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13982`
- Inputs: `Section tempSection`, `Section section`
- Outputs: writes `tempSection.HeatingSeasons.Heating.SatEsmEnd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13986`; `tempSection.HeatingSeasons.Heating.SatEsmStart (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13985`; `tempSection.HeatingSeasons.Heating.SunEsmEnd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13988`; `tempSection.HeatingSeasons.Heating.SunEsmStart (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13987`; `tempSection.HeatingSeasons.Heating.WorkEsmEnd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13984`; `tempSection.HeatingSeasons.Heating.WorkEsmStart (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13983`
- Internal calls: _None_
- Formulas: _None extracted_

### HeatingAndCoolingResultCalc.CopyVentilationHeatingWorkingSchedule
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13992`
- Inputs: `Section tempSection`, `Section section`
- Outputs: writes `tempSection.HeatingSeasons.Ventilation.SatBaseEnd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13996`; `tempSection.HeatingSeasons.Ventilation.SatBaseStart (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13995`; `tempSection.HeatingSeasons.Ventilation.SunBaseEnd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13998`; `tempSection.HeatingSeasons.Ventilation.SunBaseStart (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13997`; `tempSection.HeatingSeasons.Ventilation.WorkBaseEnd (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13994`; `tempSection.HeatingSeasons.Ventilation.WorkBaseStart (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:13993`
- Internal calls: _None_
- Formulas: _None extracted_

### HeatingAndCoolingResultCalc.VentilationHeatEnergyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15642`
- Inputs: `this CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `HeatingCalculations heatCalculations`
- Outputs: writes `calcData.Part1Ref1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15668`; `calcData.ResulHeatingInputsRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15670`; `calcData.ResultEnergyForHeatingRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15672`; `calcData.ResultSourceEnergy2Ref1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15662`; `calcData.ResultSourceEnergyRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15660`; `heatCalculations.HeatingResult.ResulVentilationInputsRef1 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15671`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15643`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15644`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15645`; `list4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15646`; ... (+4)
- Internal calls: `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef1`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15654` `list.Add(num + thermoPumpEnergy);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15656` `list2.Add(calcData.DebitRef1 * 0.34 * (calcData.FlowTemperatureRef1 - heatCalculations.HeatingResult.ProjectTemperatureRef1) * monthHours / 1000.0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15660` `calcData.ResultSourceEnergyRef1 = list3.Aggregate(0.0, (double num4, double item) => num4 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15661` `double num2 = list.Aggregate(0.0, (double num4, double item) => num4 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15662` `calcData.ResultSourceEnergy2Ref1 = num2 - calcData.ResultSourceEnergyRef1;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15663` `double num3 = calcData.ResultSourceEnergyRef1 / num2 * 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15670` `calcData.ResulHeatingInputsRef1 = list2.Aggregate(0.0, (double num4, double item) => num4 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15672` `calcData.ResultEnergyForHeatingRef1 = ((list.Count == list4.Count) ? list.Aggregate(0.0, (double num4, double item) => num4 + item) : 0.0);`

### HeatingAndCoolingResultCalc.VentilationHeatEnergyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15676`
- Inputs: `this CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `HeatingCalculations heatCalculations`
- Outputs: writes `calcData.Part1Ref2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15702`; `calcData.ResulHeatingInputsRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15704`; `calcData.ResultEnergyForHeatingRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15706`; `calcData.ResultSourceEnergy2Ref2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15696`; `calcData.ResultSourceEnergyRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15694`; `heatCalculations.HeatingResult.ResulVentilationInputsRef2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15705`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15677`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15678`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15679`; `list4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15680`; ... (+4)
- Internal calls: `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef2`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15688` `list.Add(num + thermoPumpEnergy);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15690` `list2.Add(calcData.DebitRef2 * 0.34 * (calcData.FlowTemperatureRef2 - heatCalculations.HeatingResult.ProjectTemperatureRef2) * monthHours / 1000.0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15694` `calcData.ResultSourceEnergyRef2 = list3.Aggregate(0.0, (double num4, double item) => num4 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15695` `double num2 = list.Aggregate(0.0, (double num4, double item) => num4 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15696` `calcData.ResultSourceEnergy2Ref2 = num2 - calcData.ResultSourceEnergyRef2;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15697` `double num3 = calcData.ResultSourceEnergyRef2 / num2 * 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15704` `calcData.ResulHeatingInputsRef2 = list2.Aggregate(0.0, (double num4, double item) => num4 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15706` `calcData.ResultEnergyForHeatingRef2 = ((list.Count == list4.Count) ? list.Aggregate(0.0, (double num4, double item) => num4 + item) : 0.0);`

### HeatingAndCoolingResultCalc.VentilationHeatEnergyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15710`
- Inputs: `this CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `HeatingCalculations heatCalculations`
- Outputs: writes `calcData.Part1Actual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15745`; `calcData.ResulHeatingInputsActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15747`; `calcData.ResultEnergyForHeatingActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15749`; `calcData.ResultSourceEnergy2Actual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15739`; `calcData.ResultSourceEnergyActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15737`; `heatCalculations.HeatingResult.ResulVentilationInputsActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15748`; `innerTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15732`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15711`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15712`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15713`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempActual`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyActual`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15722` `list.Add(num + thermoPumpEnergy);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15726` `section.Area.ETlineData.MonthJanuaryVentilationHeatingEnergy.Actual = num * section.Area.HeatedArea;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15730` `section.Area.ETlineData.MonthMarchVentilationHeatingEnergy.Actual = num * section.Area.HeatedArea;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15733` `list2.Add(calcData.DebitActual * 0.34 * (calcData.FlowTemperatureActual - heatCalculations.HeatingResult.ProjectTemperatureActual) * monthHours / 1000.0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15737` `calcData.ResultSourceEnergyActual = list3.Aggregate(0.0, (double num4, double item) => num4 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15738` `double num2 = list.Aggregate(0.0, (double num4, double item) => num4 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15739` `calcData.ResultSourceEnergy2Actual = num2 - calcData.ResultSourceEnergyActual;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15740` `double num3 = calcData.ResultSourceEnergyActual / num2 * 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15747` `calcData.ResulHeatingInputsActual = list2.Aggregate(0.0, (double num4, double item) => num4 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15749` `calcData.ResultEnergyForHeatingActual = ((list.Count == list4.Count) ? list.Aggregate(0.0, (double num4, double item) => num4 + item) : 0.0);`

### HeatingAndCoolingResultCalc.VentilationHeatEnergyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15753`
- Inputs: `this CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `HeatingCalculations heatCalculations`
- Outputs: writes `calcData.Part1BaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15788`; `calcData.ResulHeatingInputsBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15790`; `calcData.ResultEnergyForHeatingBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15792`; `calcData.ResultSourceEnergy2BaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15782`; `calcData.ResultSourceEnergyBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15780`; `heatCalculations.HeatingResult.ResulVentilationInputsBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15791`; `innerTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15775`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15754`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15755`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15756`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempBaseLine`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyBaseLine`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15765` `list.Add(num + thermoPumpEnergy);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15769` `section.Area.ETlineData.MonthJanuaryVentilationHeatingEnergy.BaseLine = num * section.Area.HeatedArea;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15773` `section.Area.ETlineData.MonthMarchVentilationHeatingEnergy.BaseLine = num * section.Area.HeatedArea;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15776` `list2.Add(calcData.DebitBaseLine * 0.34 * (calcData.FlowTemperatureBaseLine - heatCalculations.HeatingResult.ProjectTemperatureBaseLine) * monthHours / 1000.0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15780` `calcData.ResultSourceEnergyBaseLine = list3.Aggregate(0.0, (double num4, double item) => num4 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15781` `double num2 = list.Aggregate(0.0, (double num4, double item) => num4 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15782` `calcData.ResultSourceEnergy2BaseLine = num2 - calcData.ResultSourceEnergyBaseLine;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15783` `double num3 = calcData.ResultSourceEnergyBaseLine / num2 * 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15790` `calcData.ResulHeatingInputsBaseLine = list2.Aggregate(0.0, (double num4, double item) => num4 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15792` `calcData.ResultEnergyForHeatingBaseLine = ((list.Count == list4.Count) ? list.Aggregate(0.0, (double num4, double item) => num4 + item) : 0.0);`

### HeatingAndCoolingResultCalc.VentilationHeatEnergyESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15796`
- Inputs: `this CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `HeatingCalculations heatCalculations`
- Outputs: writes `calcData.Part1ESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15831`; `calcData.ResulHeatingInputsESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15833`; `calcData.ResultEnergyForHeatingESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15835`; `calcData.ResultSourceEnergy2ESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15825`; `calcData.ResultSourceEnergyESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15823`; `heatCalculations.HeatingResult.ResulVentilationInputsESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15834`; `innerTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15818`; `list (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15797`; `list2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15798`; `list3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15799`; ... (+7)
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempESM`, `HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyESM`, `InputDataCalc.CalcPeriod`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15808` `list.Add(num + thermoPumpEnergy);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15812` `section.Area.ETlineData.MonthJanuaryVentilationHeatingEnergy.ESM = num * section.Area.HeatedArea;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15816` `section.Area.ETlineData.MonthMarchVentilationHeatingEnergy.ESM = num * section.Area.HeatedArea;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15819` `list2.Add(calcData.DebitESM * 0.34 * (calcData.FlowTemperatureESM - heatCalculations.HeatingResult.ProjectTemperatureESM) * monthHours / 1000.0);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15823` `calcData.ResultSourceEnergyESM = list3.Aggregate(0.0, (double num4, double item) => num4 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15824` `double num2 = list.Aggregate(0.0, (double num4, double item) => num4 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15825` `calcData.ResultSourceEnergy2ESM = num2 - calcData.ResultSourceEnergyESM;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15826` `double num3 = calcData.ResultSourceEnergyESM / num2 * 100.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15833` `calcData.ResulHeatingInputsESM = list2.Aggregate(0.0, (double num4, double item) => num4 + item);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:15835` `calcData.ResultEnergyForHeatingESM = ((list.Count == list4.Count) ? list.Aggregate(0.0, (double num4, double item) => num4 + item) : 0.0);`

### HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16065`
- Inputs: `CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `HeatingCalculations ventHeatCalculations`, `MonthlyDays month`, `out double thermoPumpEnergy`
- Outputs: returns `double`; writes `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16068`; `hours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16069`; `humidity (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16070`; `innerTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16067`; `monthHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16066`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16071`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16072`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16073`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16089`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16078`; ... (+8)
- Internal calls: `HeatingAndCoolingResultCalc.CalcEntalpia`, `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempRef1`, `HeatingAndCoolingResultCalc.GetMonthHoursBaseLine`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16070` `double humidity = hours.Select((TempHumidityPerDay w) => w.Humidity).Aggregate(0.0, (double current, double item) => current + item) / (double)hours.Count;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16071` `double num = innerTemp - calcData.FirstRecEfficiencyRef1 / 100.0 * (innerTemp - avgTemp);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16072` `double num2 = innerTemp - num + avgTemp;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16080` `double num6 = calcData.DebitRef1 * 1.2 * (num4 - num5) * monthHours / 3600.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16081` `thermoPumpEnergy = num6 / (1.0 - 100.0 / calcData.SecondRecEfficiencyRef1);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16082` `double num7 = thermoPumpEnergy * 1000.0 / (calcData.DebitRef1 * 0.34 * monthHours);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16085` `thermoPumpEnergy = calcData.DebitRef1 * 0.34 * calcData.HeatingAirDifferenceRef1 * monthHours / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16089` `num3 = calcData.FlowTemperatureRef1 - num2 - num7;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16090` `return calcData.DebitRef1 * 0.34 * (calcData.FlowTemperatureRef1 - num3) * monthHours / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16092` `thermoPumpEnergy = calcData.DebitRef1 * 0.34 * (calcData.FlowTemperatureRef1 - num2) * monthHours / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16099` `return calcData.DebitRef1 * 0.34 * (calcData.FlowTemperatureRef1 - num3) * monthHours / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16103`
- Inputs: `CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `HeatingCalculations ventHeatCalculations`, `MonthlyDays month`, `out double thermoPumpEnergy`
- Outputs: returns `double`; writes `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16106`; `hours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16107`; `humidity (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16108`; `innerTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16105`; `monthHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16104`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16109`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16110`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16111`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16127`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16116`; ... (+8)
- Internal calls: `HeatingAndCoolingResultCalc.CalcEntalpia`, `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempRef2`, `HeatingAndCoolingResultCalc.GetMonthHoursBaseLine`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16108` `double humidity = hours.Select((TempHumidityPerDay w) => w.Humidity).Aggregate(0.0, (double current, double item) => current + item) / (double)hours.Count;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16109` `double num = innerTemp - calcData.FirstRecEfficiencyRef2 / 100.0 * (innerTemp - avgTemp);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16110` `double num2 = innerTemp - num + avgTemp;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16118` `double num6 = calcData.DebitRef2 * 1.2 * (num4 - num5) * monthHours / 3600.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16119` `thermoPumpEnergy = num6 / (1.0 - 100.0 / calcData.SecondRecEfficiencyRef2);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16120` `double num7 = thermoPumpEnergy * 1000.0 / (calcData.DebitRef2 * 0.34 * monthHours);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16123` `thermoPumpEnergy = calcData.DebitRef2 * 0.34 * calcData.HeatingAirDifferenceRef2 * monthHours / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16127` `num3 = calcData.FlowTemperatureRef2 - num2 - num7;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16128` `return calcData.DebitRef2 * 0.34 * (calcData.FlowTemperatureRef2 - num3) * monthHours / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16130` `thermoPumpEnergy = calcData.DebitRef2 * 0.34 * (calcData.FlowTemperatureRef2 - num2) * monthHours / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16137` `return calcData.DebitRef2 * 0.34 * (calcData.FlowTemperatureRef2 - num3) * monthHours / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16141`
- Inputs: `CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `HeatingCalculations heatCalculations`, `MonthlyDays month`, `out double thermoPumpEnergy`
- Outputs: returns `double`; writes `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16144`; `hours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16145`; `humidity (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16146`; `innerTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16143`; `monthHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16142`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16147`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16148`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16149`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16165`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16154`; ... (+8)
- Internal calls: `HeatingAndCoolingResultCalc.CalcEntalpia`, `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempActual`, `HeatingAndCoolingResultCalc.GetMonthHoursActual`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16146` `double humidity = hours.Select((TempHumidityPerDay w) => w.Humidity).Aggregate(0.0, (double current, double item) => current + item) / (double)hours.Count;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16147` `double num = innerTemp - calcData.FirstRecEfficiencyActual / 100.0 * (innerTemp - avgTemp);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16148` `double num2 = innerTemp - num + avgTemp;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16156` `double num6 = calcData.DebitActual * 1.2 * (num4 - num5) * monthHours / 3600.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16157` `thermoPumpEnergy = num6 / (1.0 - 100.0 / calcData.SecondRecEfficiencyActual);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16158` `double num7 = thermoPumpEnergy * 1000.0 / (calcData.DebitActual * 0.34 * monthHours);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16161` `thermoPumpEnergy = calcData.DebitActual * 0.34 * calcData.HeatingAirDifferenceActual * monthHours / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16165` `num3 = calcData.FlowTemperatureActual - num2 - num7;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16166` `return calcData.DebitActual * 0.34 * (calcData.FlowTemperatureActual - num3) * monthHours / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16168` `thermoPumpEnergy = calcData.DebitActual * 0.34 * (calcData.FlowTemperatureActual - num2) * monthHours / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16175` `return calcData.DebitActual * 0.34 * (calcData.FlowTemperatureActual - num3) * monthHours / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16179`
- Inputs: `CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `HeatingCalculations heatCalculations`, `MonthlyDays month`, `out double thermoPumpEnergy`
- Outputs: returns `double`; writes `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16182`; `hours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16183`; `humidity (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16184`; `innerTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16181`; `monthHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16180`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16185`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16186`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16187`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16203`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16192`; ... (+8)
- Internal calls: `HeatingAndCoolingResultCalc.CalcEntalpia`, `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempBaseLine`, `HeatingAndCoolingResultCalc.GetMonthHoursBaseLine`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16184` `double humidity = hours.Select((TempHumidityPerDay w) => w.Humidity).Aggregate(0.0, (double current, double item) => current + item) / (double)hours.Count;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16185` `double num = innerTemp - calcData.FirstRecEfficiencyBaseLine / 100.0 * (innerTemp - avgTemp);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16186` `double num2 = innerTemp - num + avgTemp;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16194` `double num6 = calcData.DebitBaseLine * 1.2 * (num4 - num5) * monthHours / 3600.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16195` `thermoPumpEnergy = num6 / (1.0 - 100.0 / calcData.SecondRecEfficiencyBaseLine);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16196` `double num7 = thermoPumpEnergy * 1000.0 / (calcData.DebitBaseLine * 0.34 * monthHours);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16199` `thermoPumpEnergy = calcData.DebitBaseLine * 0.34 * calcData.HeatingAirDifferenceBaseLine * monthHours / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16203` `num3 = calcData.FlowTemperatureBaseLine - num2 - num7;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16204` `return calcData.DebitBaseLine * 0.34 * (calcData.FlowTemperatureBaseLine - num3) * monthHours / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16206` `thermoPumpEnergy = calcData.DebitBaseLine * 0.34 * (calcData.FlowTemperatureBaseLine - num2) * monthHours / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16213` `return calcData.DebitBaseLine * 0.34 * (calcData.FlowTemperatureBaseLine - num3) * monthHours / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateMontlyHeatEnergyESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16217`
- Inputs: `CalculationData calcData`, `Section section`, `CalculationInput calcInput`, `HeatingCalculations ventHeatCalculations`, `MonthlyDays month`, `out double thermoPumpEnergy`
- Outputs: returns `double`; writes `avgTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16220`; `hours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16221`; `humidity (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16222`; `innerTemp (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16219`; `monthHours (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16218`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16223`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16224`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16225`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16241`; `num4 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16230`; ... (+8)
- Internal calls: `HeatingAndCoolingResultCalc.CalcEntalpia`, `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempESM`, `HeatingAndCoolingResultCalc.GetMonthHoursESM`, `PreferencesManager.GetClimateZoneParams`
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16222` `double humidity = hours.Select((TempHumidityPerDay w) => w.Humidity).Aggregate(0.0, (double current, double item) => current + item) / (double)hours.Count;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16223` `double num = innerTemp - calcData.FirstRecEfficiencyESM / 100.0 * (innerTemp - avgTemp);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16224` `double num2 = innerTemp - num + avgTemp;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16232` `double num6 = calcData.DebitESM * 1.2 * (num4 - num5) * monthHours / 3600.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16233` `thermoPumpEnergy = num6 / (1.0 - 100.0 / calcData.SecondRecEfficiencyESM);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16234` `double num7 = thermoPumpEnergy * 1000.0 / (calcData.DebitESM * 0.34 * monthHours);`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16237` `thermoPumpEnergy = calcData.DebitESM * 0.34 * calcData.HeatingAirDifferenceESM * monthHours / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16241` `num3 = calcData.FlowTemperatureESM - num2 - num7;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16242` `return calcData.DebitESM * 0.34 * (calcData.FlowTemperatureESM - num3) * monthHours / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16244` `thermoPumpEnergy = calcData.DebitESM * 0.34 * (calcData.FlowTemperatureESM - num2) * monthHours / 1000.0;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16251` `return calcData.DebitESM * 0.34 * (calcData.FlowTemperatureESM - num3) * monthHours / 1000.0;`

### HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempRef1
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16255`
- Inputs: `Section section`, `HeatingCalculations heatCalcData`, `MonthlyDays month`
- Outputs: returns `double`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempBaseLine`
- Formulas: _None extracted_

### HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempRef2
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16260`
- Inputs: `Section section`, `HeatingCalculations heatCalcData`, `MonthlyDays month`
- Outputs: returns `double`
- Internal calls: `HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempBaseLine`
- Formulas: _None extracted_

### HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempActual
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16265`
- Inputs: `Section section`, `HeatingCalculations heatCalcData`, `MonthlyDays month`
- Outputs: returns `double`; writes `nonProjectTemperatureActual (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16315`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16266`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16282`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16298`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16267`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16283`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16299`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16268`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16284`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16300`; ... (+7)
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16280` `int num4 = num2 * month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16281` `int num5 = (num + num3) * month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16296` `num4 += num2 * month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16297` `num5 += (num + num3) * month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16312` `num4 += num2 * month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16313` `num5 += (num + num3) * month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16316` `return ((double)num4 * projectTemperatureActual + (double)num5 * nonProjectTemperatureActual) / (double)(num4 + num5);`

### HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempBaseLine
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16320`
- Inputs: `Section section`, `HeatingCalculations heatCalcData`, `MonthlyDays month`
- Outputs: returns `double`; writes `nonProjectTemperatureBaseLine (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16370`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16321`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16337`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16353`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16322`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16338`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16354`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16323`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16339`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16355`; ... (+7)
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16335` `int num4 = num2 * month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16336` `int num5 = (num + num3) * month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16351` `num4 += num2 * month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16352` `num5 += (num + num3) * month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16367` `num4 += num2 * month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16368` `num5 += (num + num3) * month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16371` `return ((double)num4 * projectTemperatureBaseLine + (double)num5 * nonProjectTemperatureBaseLine) / (double)(num4 + num5);`

### HeatingAndCoolingResultCalc.CalculateAverageVentHeatTempESM
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16375`
- Inputs: `Section section`, `HeatingCalculations heatCalcData`, `MonthlyDays month`
- Outputs: returns `double`; writes `nonProjectTemperatureESM (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16425`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16376`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16392`; `num (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16408`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16377`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16393`; `num2 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16409`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16378`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16394`; `num3 (=) at reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16410`; ... (+7)
- Internal calls: _None_
- Formulas:
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16390` `int num4 = num2 * month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16391` `int num5 = (num + num3) * month.WorkDays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16406` `num4 += num2 * month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16407` `num5 += (num + num3) * month.Saturdays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16422` `num4 += num2 * month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16423` `num5 += (num + num3) * month.Sundays;`
  - `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:16426` `return ((double)num4 * projectTemperatureESM + (double)num5 * nonProjectTemperatureESM) / (double)(num4 + num5);`
