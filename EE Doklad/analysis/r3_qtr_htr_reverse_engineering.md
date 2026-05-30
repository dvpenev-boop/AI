# R3 reverse engineering: Htr/Qtr EECalc oracle

Източник на истина: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs`.

Цел: test-only oracle за `Htr` и `Qtr`, без промени в production code. Всички формули по-долу са преписани от decompiled C# и трябва да се имплементират като EECalc-compatible поведение, включително декомпилационните особености.

## Обобщен call graph

```text
CalculateParameterQtr
  -> PreferencesManager.GetClimateZoneParams(...).SolarRadiation.Months[month].AvgTemp
  -> CalculateAverageHeatTempCurrent
  -> CalculateParameterHtr
       -> SumWallDirecrionsHu1
            -> CalcWallDirectionParameterHu1
       -> CalcCeilingsParameterHu2
       -> CalcFloorsParameterHu3
       -> CalculateParameterHdCurrent
            -> SumAllDirectionsWallsCurrent
                 -> CalculateItemsWalls
            -> SumAllDirectionWindowsCurrent
            -> SumNonTrasparentRoof
            -> SumTrasparentRoof
       -> CalculateParameterHgCurrent
  -> CalcAvgProjectTemp
  -> CalcAvgNonProjectTemp
```

## CalculateParameterQtr

Exact EECalc source method:

- `HeatingAndCoolingResultCalc.CalculateParameterQtr`
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3693`
- Signature: `private static double CalculateParameterQtr(CalculationData calculationData, Section section, ClimateZones climateZone, MonthlyDays month, out double parameterHtr)`

Inputs:

- `calculationData`: `ProjectTemperatureActual`, `NonProjectTemperatureActual`, and other calculation values used by dependent methods.
- `section`: heating schedule, geometry, envelope data, test output fields.
- `climateZone`: used to read monthly outdoor average temperature.
- `month`: `MonthlyDays` row with `Month`, `WorkDays`, `Saturdays`, `Sundays`, `Holydays`.
- `parameterHtr`: out parameter.

Outputs:

- Returns monthly transmission heat transfer energy `Qtr` in kWh.
- Writes `parameterHtr`.
- Writes `section.Test.ParameterHtr`.

Formula:

```text
avgTemp =
  PreferencesManager.GetClimateZoneParams(climateZone)
    .SolarRadiation.Months[(int)month.Month].AvgTemp

averageInnerHeatTemp =
  CalculateAverageHeatTempCurrent(section, calculationData, month)

parameterHtr =
  CalculateParameterHtr(section, avgTemp, averageInnerHeatTemp)

section.Test.ParameterHtr = parameterHtr

Qtr =
  section.Test.ParameterHtr
  * (CalcAvgProjectTemp(section, climateZone, calculationData, month)
     + CalcAvgNonProjectTemp(section, climateZone, calculationData, month))
  / 1000.0
```

C# formula line:

- `HeatingAndCoolingResultCalc.cs:3698`: `return section.Test.ParameterHtr * (CalcAvgProjectTemp(section, climateZone, calculationData, month) + CalcAvgNonProjectTemp(section, climateZone, calculationData, month)) / 1000.0;`

Dependencies:

- `PreferencesManager.GetClimateZoneParams`
- `CalculateAverageHeatTempCurrent`
- `CalculateParameterHtr`
- `CalcAvgProjectTemp`
- `CalcAvgNonProjectTemp`

Oracle note:

- `CalcAvgProjectTemp` and `CalcAvgNonProjectTemp` are already part of R2 degree-hour logic and must be reused for Qtr. Do not recompute monthly hours differently for Qtr.

## CalculateParameterHtr

Exact EECalc source method:

- `HeatingAndCoolingResultCalc.CalculateParameterHtr`
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3702`
- Signature: `private static double CalculateParameterHtr(Section section, double averageMontlyTemp, double averageInnerHeatTemp)`

Inputs:

- `section`: wall, window, roof, floor and adjacent-zone data.
- `averageMontlyTemp`: monthly outdoor average temperature.
- `averageInnerHeatTemp`: weighted indoor heating temperature for the month.

Outputs:

- Returns `Htr`.
- Writes:
  - `section.Test.ParameterHu`
  - `section.Test.ParameterHd`
  - `section.Test.ParameterHg`

Formula:

```text
HuWalls = SumWallDirecrionsHu1(section, averageMontlyTemp, averageInnerHeatTemp)
HuCeilings = CalcCeilingsParameterHu2(section.Roof.Current, averageMontlyTemp, averageInnerHeatTemp)
HuFloors = CalcFloorsParameterHu3(section.Floor.Current, averageMontlyTemp, averageInnerHeatTemp)

section.Test.ParameterHu = HuWalls + HuCeilings + HuFloors
section.Test.ParameterHd = CalculateParameterHdCurrent(section)
section.Test.ParameterHg = CalculateParameterHgCurrent(section)

Htr =
  section.Test.ParameterHd
  + section.Test.ParameterHg
  + section.Test.ParameterHu
```

C# formula lines:

- `HeatingAndCoolingResultCalc.cs:3706`: `section.Test.ParameterHu = num + num2 + num3;`
- `HeatingAndCoolingResultCalc.cs:3709`: `return section.Test.ParameterHd + section.Test.ParameterHg + section.Test.ParameterHu;`

Dependencies:

- `SumWallDirecrionsHu1`
- `CalcCeilingsParameterHu2`
- `CalcFloorsParameterHu3`
- `CalculateParameterHdCurrent`
- `CalculateParameterHgCurrent`

## CalculateParameterHdCurrent

Exact EECalc source method:

- `HeatingAndCoolingResultCalc.CalculateParameterHdCurrent`
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3713`
- Signature: `private static double CalculateParameterHdCurrent(Section section)`

Inputs:

- `section.NorthWalls.Current`, `NorthEastWalls.Current`, `EastWalls.Current`, `SouthEastWalls.Current`, `SouthWalls.Current`, `SouthWestWalls.Current`, `WestWalls.Current`, `NorthWestWalls.Current`
- `section.Roof.Current`

Outputs:

- Returns direct transmission coefficient `Hd`.

Formula:

```text
Hd =
  SumAllDirectionsWallsCurrent(section)
  + SumAllDirectionWindowsCurrent(section)
  + SumNonTrasparentRoof(section.Roof.Current)
  + SumTrasparentRoof(section.Roof.Current)
```

C# formula line:

- `HeatingAndCoolingResultCalc.cs:3714`: `return SumAllDirectionsWallsCurrent(section) + SumAllDirectionWindowsCurrent(section) + SumNonTrasparentRoof(section.Roof.Current) + SumTrasparentRoof(section.Roof.Current);`

Dependencies:

- `SumAllDirectionsWallsCurrent`
- `SumAllDirectionWindowsCurrent`
- `SumNonTrasparentRoof`
- `SumTrasparentRoof`

Oracle note:

- Although the user-requested R3 list starts at `CalculateParameterHdCurrent`, an oracle cannot be complete without these four sub-dependencies.

## CalculateParameterHgCurrent

Exact EECalc source method:

- `HeatingAndCoolingResultCalc.CalculateParameterHgCurrent`
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3718`
- Signature: `private static double CalculateParameterHgCurrent(Section section)`

Inputs:

- `section.Floor.Current.AccumulateFloorA`
- `section.Floor.Current.AccumulateFloorU`

Outputs:

- Returns ground transmission coefficient `Hg`.

Formula:

```text
Hg = section.Floor.Current.AccumulateFloorA * section.Floor.Current.AccumulateFloorU
```

C# formula line:

- `HeatingAndCoolingResultCalc.cs:3719`: `return section.Floor.Current.AccumulateFloorA * section.Floor.Current.AccumulateFloorU;`

Dependencies:

- None.

## SumWallDirecrionsHu1

Exact EECalc source method:

- `HeatingAndCoolingResultCalc.SumWallDirecrionsHu1`
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3838`
- Signature: `private static double SumWallDirecrionsHu1(Section section, double averageMontlyTemp, double averageInnerHeatTemp)`

Inputs:

- `section`
- `averageMontlyTemp`
- `averageInnerHeatTemp`

Outputs:

- Returns wall-adjacent unconditioned/interior contribution for `Hu`.

Formula from decompiled source:

```text
num  = CalcWallDirectionParameterHu1(section.NorthWalls.Current, averageMontlyTemp, averageInnerHeatTemp)
num2 = CalcWallDirectionParameterHu1(section.NorthWalls.Current, averageMontlyTemp, averageInnerHeatTemp)
num3 = CalcWallDirectionParameterHu1(section.NorthWalls.Current, averageMontlyTemp, averageInnerHeatTemp)
num4 = CalcWallDirectionParameterHu1(section.NorthWalls.Current, averageMontlyTemp, averageInnerHeatTemp)
num5 = CalcWallDirectionParameterHu1(section.NorthWalls.Current, averageMontlyTemp, averageInnerHeatTemp)
num6 = CalcWallDirectionParameterHu1(section.NorthWalls.Current, averageMontlyTemp, averageInnerHeatTemp)
num7 = CalcWallDirectionParameterHu1(section.NorthWalls.Current, averageMontlyTemp, averageInnerHeatTemp)
num8 = CalcWallDirectionParameterHu1(section.NorthWalls.Current, averageMontlyTemp, averageInnerHeatTemp)

HuWalls = num + num2 + num3 + num4 + num5 + num6 + num7 + num8
```

C# source lines:

- `HeatingAndCoolingResultCalc.cs:3840-3847`: each call uses `section.NorthWalls.Current`.
- `HeatingAndCoolingResultCalc.cs:3848`: `return num + num2 + num3 + num4 + num5 + num6 + num7 + num8;`

Dependencies:

- `CalcWallDirectionParameterHu1`

Important forensic note:

- The method name is misspelled in EECalc as `SumWallDirecrionsHu1`.
- Decompiled source calls `CalcWallDirectionParameterHu1(section.NorthWalls.Current, ...)` eight times. It does not call NE/E/SE/S/SW/W/NW wall groups in this method. Treat this as source-of-truth behavior for an EECalc-compatible oracle unless later binary execution proves the decompiler wrong.
- KD-004 is confirmed: `SumWallDirecrionsHu1` uses `NorthWalls.Current` eight times for internal wall `Hu`.

Mode decision:

- `LegacyEECalc`: preserve behavior as `CalcWallDirectionParameterHu1(NorthWalls.Current) * 8`.
- `CurrentCorrect`: use corrected directional sum:
  `North + NorthEast + East + SouthEast + South + SouthWest + West + NorthWest`.
- Validation reports must label the delta between these modes as `KD-004`, not as an unknown Htr formula mismatch.

## CalcWallDirectionParameterHu1

Exact EECalc source method:

- `HeatingAndCoolingResultCalc.CalcWallDirectionParameterHu1`
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3851`
- Signature: `private static double CalcWallDirectionParameterHu1(Walls wall, double averageMontlyTemp, double averageInnerHeatTemp)`

Inputs:

- `wall.InnerA1..InnerA6`
- `wall.InnerU1..InnerU6`
- `wall.InnerW1..InnerW6`
- `averageMontlyTemp`
- `averageInnerHeatTemp`

Outputs:

- Returns one wall-direction `Hu1` value.

Formula:

```text
denominator = averageInnerHeatTemp - averageMontlyTemp

Hu1 =
  InnerA1 * InnerU1 * (averageInnerHeatTemp - InnerW1) / denominator
  + InnerA2 * InnerU2 * (averageInnerHeatTemp - InnerW2) / denominator
  + InnerA3 * InnerU3 * (averageInnerHeatTemp - InnerW3) / denominator
  + InnerA4 * InnerU4 * (averageInnerHeatTemp - InnerW4) / denominator
  + IneerA5 * IneerA5 * (averageInnerHeatTemp - InnerW5) / denominator
  + InnerA6 * InnerU6 * (averageInnerHeatTemp - InnerW6) / denominator
```

C# source lines:

- `HeatingAndCoolingResultCalc.cs:3852`: `double num = averageInnerHeatTemp - averageMontlyTemp;`
- `HeatingAndCoolingResultCalc.cs:3853-3878`: per-row calculations.
- `HeatingAndCoolingResultCalc.cs:3879`: `return num3 + num4 + num5 + num6 + num7 + num8;`

Important forensic note:

- For the fifth component, decompiled C# assigns:
  - `innerA = wall.IneerA5;`
  - `innerU = wall.IneerA5;`
- This means the EECalc-compatible formula uses `IneerA5 * IneerA5`, not `InnerA5 * InnerU5`, if we follow decompiled source exactly.
- The property is also misspelled as `IneerA5` in the decompiled source.

Dependencies:

- None.

## CalcCeilingsParameterHu2

Exact EECalc source method:

- `HeatingAndCoolingResultCalc.CalcCeilingsParameterHu2`
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3881`
- Signature: `private static double CalcCeilingsParameterHu2(Roof roof, double averageMontlyTemp, double averageInnerHeatTemp)`

Inputs:

- `roof.CeilingA1..CeilingA6`
- `roof.CeilingU1..CeilingU6`
- `roof.CeilingW1..CeilingW6`
- `averageMontlyTemp`
- `averageInnerHeatTemp`

Outputs:

- Returns ceiling adjacent-zone contribution `Hu2`.

Formula:

```text
denominator = averageInnerHeatTemp - averageMontlyTemp

Hu2 =
  CeilingA1 * CeilingU1 * (averageInnerHeatTemp - CeilingW1) / denominator
  + CeilingA2 * CeilingU2 * (averageInnerHeatTemp - CeilingW2) / denominator
  + CeilingA3 * CeilingU3 * (averageInnerHeatTemp - CeilingW3) / denominator
  + CeilingA4 * CeilingU4 * (averageInnerHeatTemp - CeilingW4) / denominator
  + CeilingA5 * CeilingA5 * (averageInnerHeatTemp - CeilingW5) / denominator
  + CeilingA6 * CeilingU6 * (averageInnerHeatTemp - CeilingW6) / denominator
```

C# source lines:

- `HeatingAndCoolingResultCalc.cs:3882`: `double num = averageInnerHeatTemp - averageMontlyTemp;`
- `HeatingAndCoolingResultCalc.cs:3883-3908`: per-row calculations.
- `HeatingAndCoolingResultCalc.cs:3909`: `return num3 + num4 + num5 + num6 + num7 + num8;`

Important forensic note:

- For the fifth component, decompiled C# assigns:
  - `ceilingA = roof.CeilingA5;`
  - `ceilingU = roof.CeilingA5;`
- EECalc-compatible oracle should therefore use `CeilingA5 * CeilingA5` for this component.

Dependencies:

- None.

## CalcFloorsParameterHu3

Exact EECalc source method:

- `HeatingAndCoolingResultCalc.CalcFloorsParameterHu3`
- Source: `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs:3911`
- Signature: `private static double CalcFloorsParameterHu3(Floor floor, double averageMontlyTemp, double averageInnerHeatTemp)`

Inputs:

- `floor.OtherFloorA1..OtherFloorA6`
- `floor.OtherFloorU1..OtherFloorU6`
- `floor.OtherFloorW1..OtherFloorW6`
- `averageMontlyTemp`
- `averageInnerHeatTemp`

Outputs:

- Returns other-floor adjacent-zone contribution `Hu3`.

Formula:

```text
denominator = averageInnerHeatTemp - averageMontlyTemp

Hu3 =
  OtherFloorA1 * OtherFloorU1 * (averageInnerHeatTemp - OtherFloorW1) / denominator
  + OtherFloorA2 * OtherFloorU2 * (averageInnerHeatTemp - OtherFloorW2) / denominator
  + OtherFloorA3 * OtherFloorU3 * (averageInnerHeatTemp - OtherFloorW3) / denominator
  + OtherFloorA4 * OtherFloorU4 * (averageInnerHeatTemp - OtherFloorW4) / denominator
  + OtherFloorA5 * OtherFloorU5 * (averageInnerHeatTemp - OtherFloorW5) / denominator
  + OtherFloorA6 * OtherFloorU6 * (averageInnerHeatTemp - OtherFloorW6) / denominator
```

C# source lines:

- `HeatingAndCoolingResultCalc.cs:3912`: `double num = averageInnerHeatTemp - averageMontlyTemp;`
- `HeatingAndCoolingResultCalc.cs:3913-3938`: per-row calculations.
- `HeatingAndCoolingResultCalc.cs:3939`: `return num3 + num4 + num5 + num6 + num7 + num8;`

Dependencies:

- None.

## Required Hd sub-dependencies

These are required to implement `CalculateParameterHdCurrent`.

### SumAllDirectionsWallsCurrent

Exact EECalc source method:

- `HeatingAndCoolingResultCalc.SumAllDirectionsWallsCurrent`
- Source: `HeatingAndCoolingResultCalc.cs:3723`

Inputs:

- Eight wall direction groups: N, NE, E, SE, S, SW, W, NW.

Formula:

```text
WallsCurrent =
  CalculateItemsWalls(NorthWalls.Current)
  + CalculateItemsWalls(NorthEastWalls.Current)
  + CalculateItemsWalls(EastWalls.Current)
  + CalculateItemsWalls(SouthEastWalls.Current)
  + CalculateItemsWalls(SouthWalls.Current)
  + CalculateItemsWalls(SouthWestWalls.Current)
  + CalculateItemsWalls(WestWalls.Current)
  + CalculateItemsWalls(NorthWestWalls.Current)
```

C# formula line:

- `HeatingAndCoolingResultCalc.cs:3732`: `return north + northEast + east + southEast + south + southWest + west + northWest;`

### CalculateItemsWalls

Exact EECalc source method:

- `HeatingAndCoolingResultCalc.CalculateItemsWalls`
- Source: `HeatingAndCoolingResultCalc.cs:3736`

Formula:

```text
WallOpaqueUA =
  OuterA1*OuterU1 + OuterA2*OuterU2 + OuterA3*OuterU3
  + OuterA4*OuterU4 + OuterA5*OuterU5 + OuterA6*OuterU6

WallSumL =
  Outer1.SumL + Outer2.SumL + Outer3.SumL
  + Outer4.SumL + Outer5.SumL + Outer6.SumL

WallSumX =
  Outer1.SumX + Outer2.SumX + Outer3.SumX
  + Outer4.SumX + Outer5.SumX + Outer6.SumX

CalculateItemsWalls = WallOpaqueUA + WallSumL + WallSumX
```

C# formula lines:

- `HeatingAndCoolingResultCalc.cs:3737-3743`: area times U sum.
- `HeatingAndCoolingResultCalc.cs:3744-3750`: `SumL` sum.
- `HeatingAndCoolingResultCalc.cs:3751-3758`: `SumX` sum and return.

### SumAllDirectionWindowsCurrent

Exact EECalc source method:

- `HeatingAndCoolingResultCalc.SumAllDirectionWindowsCurrent`
- Source: `HeatingAndCoolingResultCalc.cs:3762`

Formula:

```text
WindowsCurrent =
  North.AccumulateWindowU * North.AccumulateWindowA
  + NorthEast.AccumulateWindowU * NorthEast.AccumulateWindowA
  + East.AccumulateWindowU * East.AccumulateWindowA
  + SouthEast.AccumulateWindowU * SouthEast.AccumulateWindowA
  + South.AccumulateWindowU * South.AccumulateWindowA
  + SouthWest.AccumulateWindowU * SouthWest.AccumulateWindowA
  + West.AccumulateWindowU * West.AccumulateWindowA
  + NorthWest.AccumulateWindowU * NorthWest.AccumulateWindowA
```

C# formula lines:

- `HeatingAndCoolingResultCalc.cs:3763-3771`

### SumNonTrasparentRoof

Exact EECalc source method:

- `HeatingAndCoolingResultCalc.SumNonTrasparentRoof`
- Source: `HeatingAndCoolingResultCalc.cs:3775`

Formula:

```text
NonTransparentRoofUA =
  NonTransparentA1*NonTransparentU1 + ... + NonTransparentA9*NonTransparentU9

NonTransparentRoofSumL =
  NonTransparent1.SumL + ... + NonTransparent9.SumL

NonTransparentRoofSumX =
  NonTransparent1.SumX + ... + NonTransparent9.SumX

SumNonTrasparentRoof =
  NonTransparentRoofUA + NonTransparentRoofSumL + NonTransparentRoofSumX
```

C# formula lines:

- `HeatingAndCoolingResultCalc.cs:3776-3785`: area times U sum.
- `HeatingAndCoolingResultCalc.cs:3786-3795`: `SumL`.
- `HeatingAndCoolingResultCalc.cs:3796-3806`: `SumX` and return.

For element 6 the decompiled property is `NonTransparentРђ6` with Cyrillic-looking `Рђ`, not plain ASCII `A`.

### SumTrasparentRoof

Exact EECalc source method:

- `HeatingAndCoolingResultCalc.SumTrasparentRoof`
- Source: `HeatingAndCoolingResultCalc.cs:3810`

Formula:

```text
TransparentRoof =
  TransparentA1*TransparentU1 + TransparentA2*TransparentU2
  + TransparentA3*TransparentU3 + TransparentA4*TransparentU4
  + TransparentA5*TransparentU5 + TransparentРђ6*TransparentU6
  + TransparentA7*TransparentU7 + TransparentA8*TransparentU8
  + TransparentA9*TransparentU9
```

C# formula lines:

- `HeatingAndCoolingResultCalc.cs:3811-3820`

For element 6 the decompiled property is `TransparentРђ6`.

## CalculateAverageHeatTempCurrent dependency

`CalculateParameterQtr` uses this method to compute `averageInnerHeatTemp` before calling `CalculateParameterHtr`.

Exact EECalc source method:

- `HeatingAndCoolingResultCalc.CalculateAverageHeatTempCurrent`
- Source: `HeatingAndCoolingResultCalc.cs:3824`

Formula:

```text
projectHours =
  WorkDays * (WorkCurrentEnd - WorkCurrentStart)
  + Sundays * (SunCurrentEnd - SunCurrentStart)
  + Saturdays * (SatCurrentEnd - SatCurrentStart)

nonProjectHours =
  WorkDays * (24 - (WorkCurrentEnd - WorkCurrentStart))
  + Saturdays * (24 - (SatCurrentEnd - SatCurrentStart))
  + Sundays * (24 - (SunCurrentEnd - SunCurrentStart))
  + Holydays * 24

averageInnerHeatTemp =
  (projectHours * ProjectTemperatureActual
   + nonProjectHours * NonProjectTemperatureActual)
  / (projectHours + nonProjectHours)
```

C# formula lines:

- `HeatingAndCoolingResultCalc.cs:3825-3834`

Oracle note:

- This method uses direct `End - Start` schedule durations, not `CalcHours`.
- If a schedule crosses midnight, EECalc-compatible behavior should follow the decompiled direct subtraction unless a later binary validation proves otherwise.

## Qtr degree-hour dependencies from R2

`CalculateParameterQtr` reuses the same degree-hour structure as Qve:

```text
Qtr = Htr * (CalcAvgProjectTemp + CalcAvgNonProjectTemp) / 1000
```

The R3 oracle should call the same R2-compatible implementations for:

- `CalcAvgProjectTemp`
- `CalcAvgNonProjectTemp`

This avoids divergence between Qve and Qtr hours.

## Implementation checklist for test-only oracle

1. Add test-only model for envelope inputs, not production model mutation.
2. Implement `CalculateParameterHgCurrent` first; it is isolated.
3. Implement `CalculateItemsWalls`, `SumAllDirectionsWallsCurrent`, `SumAllDirectionWindowsCurrent`, `SumNonTrasparentRoof`, `SumTrasparentRoof`.
4. Implement `CalculateParameterHdCurrent`.
5. Implement `CalcWallDirectionParameterHu1`, preserving the fifth-component `IneerA5 * IneerA5` behavior.
6. Implement `SumWallDirecrionsHu1` with explicit mode:
   - `LegacyEECalc`: preserve the eight repeated `NorthWalls.Current` calls.
   - `CurrentCorrect`: sum all eight directions.
   - Mark any mode delta as confirmed known difference `KD-004`.
7. Implement `CalcCeilingsParameterHu2`, preserving `CeilingA5 * CeilingA5`.
8. Implement `CalcFloorsParameterHu3`.
9. Implement `CalculateAverageHeatTempCurrent`.
10. Implement `CalculateParameterHtr`.
11. Implement `CalculateParameterQtr` by reusing R2 degree-hour functions.

## Known EECalc-compatible quirks to preserve

- `SumWallDirecrionsHu1` spelling and behavior.
- `KD-004` confirmed: `SumWallDirecrionsHu1` calls `section.NorthWalls.Current` eight times. `LegacyEECalc` preserves this; `CurrentCorrect` sums all eight wall directions.
- `CalcWallDirectionParameterHu1` uses `wall.IneerA5` as both area and U for component 5.
- `CalcCeilingsParameterHu2` uses `roof.CeilingA5` as both area and U for component 5.
- `SumNonTrasparentRoof` and `SumTrasparentRoof` use decompiled element-6 properties with Cyrillic-looking `Рђ`.
- No division-by-zero guard is present in `CalcWallDirectionParameterHu1`, `CalcCeilingsParameterHu2`, or `CalcFloorsParameterHu3`; oracle should surface the same `double` behavior unless the test harness intentionally wraps diagnostics around it.
