# 04 - Qtr / transmission engine

## Основна формула

`CalculateParameterQtr(...)` (`HeatingAndCoolingResultCalc.cs:3693`) връща:

```text
Qtr_m = Htr_m * (DeltaProject_m + DeltaNonProject_m) / 1000
```

Източник: `HeatingAndCoolingResultCalc.cs:3695-3699`.

`DeltaProject_m` идва от `CalcAvgProjectTemp(...)` (`HeatingAndCoolingResultCalc.cs:3668`):

```text
projectHours = WorkDays * (WorkCurrentEnd - WorkCurrentStart)
             + Sundays * (SunCurrentEnd - SunCurrentStart)
             + Saturdays * (SatCurrentEnd - SatCurrentStart)

DeltaProject = (ProjectTemperatureActual - AvgTemp) * projectHours
```

Източник: `HeatingAndCoolingResultCalc.cs:3670-3674`.

`DeltaNonProject_m` идва от `CalcAvgNonProjectTemp(...)` (`HeatingAndCoolingResultCalc.cs:3677`):

```text
nonProjectHours =
    WorkDays * (24 - (WorkCurrentEnd - WorkCurrentStart))
  + Saturdays * (24 - (SatCurrentEnd - SatCurrentStart))
  + Sundays * (24 - (SunCurrentEnd - SunCurrentStart))
  + Holydays * 24

DeltaNonProject = (NonProjectTemperatureActual - AvgTemp) * nonProjectHours
```

Източник: `HeatingAndCoolingResultCalc.cs:3679-3684`.

## Htr

`CalculateParameterHtr(...)` (`HeatingAndCoolingResultCalc.cs:3702`) връща:

```text
Htr = Hd + Hg + Hu
```

Източник: `HeatingAndCoolingResultCalc.cs:3704-3710`.

### Hd

`CalculateParameterHdCurrent(...)` (`HeatingAndCoolingResultCalc.cs:3713`):

```text
Hd = SumAllDirectionsWallsCurrent(section)
   + SumAllDirectionWindowsCurrent(section)
   + SumNonTrasparentRoof(section.Roof.Current)
   + SumTrasparentRoof(section.Roof.Current)
```

Източник: `HeatingAndCoolingResultCalc.cs:3715`.

Външните стени за всяка посока използват `CalculateItemsWalls(...)`:

```text
WallsContribution =
    sum(OuterA_i * OuterU_i, i=1..6)
  + sum(Outer_i.SumL, i=1..6)
  + sum(Outer_i.SumX, i=1..6)
```

Източник: `HeatingAndCoolingResultCalc.cs:3736-3759`.

Прозорци:

```text
WindowsContribution_direction = AccumulateWindowU * AccumulateWindowA
```

Източник: `HeatingAndCoolingResultCalc.cs:3762-3772`.

Непрозрачен покрив:

```text
NonTransparentRoof =
    sum(NonTransparentA_i * NonTransparentU_i, i=1..9)
  + sum(NonTransparent_i.SumL, i=1..9)
  + sum(NonTransparent_i.SumX, i=1..9)
```

Източник: `HeatingAndCoolingResultCalc.cs:3775-3807`.

Прозрачен покрив:

```text
TransparentRoof = sum(TransparentA_i * TransparentU_i, i=1..9)
```

Източник: `HeatingAndCoolingResultCalc.cs:3810-3821`.

### Hg

`CalculateParameterHgCurrent(...)`:

```text
Hg = Floor.Current.AccumulateFloorA * Floor.Current.AccumulateFloorU
```

Източник: `HeatingAndCoolingResultCalc.cs:3718-3721`.

### Hu

`Hu` е сума от вътрешни стени, тавани и други подове:

```text
Hu = SumWallDirecrionsHu1(...) + CalcCeilingsParameterHu2(...) + CalcFloorsParameterHu3(...)
```

Източник: `HeatingAndCoolingResultCalc.cs:3704-3707`.

Вътрешен елемент:

```text
Hu_i = A_i * U_i * (averageInnerHeatTemp - W_i) / (averageInnerHeatTemp - averageMontlyTemp)
```

Източници:

- стени: `CalcWallDirectionParameterHu1(...)` (`HeatingAndCoolingResultCalc.cs:3851-3878`);
- тавани: `CalcCeilingsParameterHu2(...)` (`HeatingAndCoolingResultCalc.cs:3881-3908`);
- подове: `CalcFloorsParameterHu3(...)` (`HeatingAndCoolingResultCalc.cs:3911-3938`).

Наблюдение от C#: `SumWallDirecrionsHu1(...)` извиква осем пъти `section.NorthWalls.Current`, не различните посоки (`HeatingAndCoolingResultCalc.cs:3838-3848`). Това е поведение на декомпилирания код и не е коригирано тук.

## Средна вътрешна отоплителна температура

`CalculateAverageHeatTempCurrent(...)` (`HeatingAndCoolingResultCalc.cs:3824`) връща:

```text
AvgInner =
  (projectHours * ProjectTemperatureActual
   + nonProjectHours * NonProjectTemperatureActual)
  / (projectHours + nonProjectHours)
```

Източник: `HeatingAndCoolingResultCalc.cs:3826-3835`.

## Предварителни таблици

`WallsTableCalc`, `RoofTableCalc`, `FloorTableCalc` и `TempBridgeCalc` подготвят агрегирани площи и U стойности. Например:

- `WallsTableCalc.AccumulateOuterU(...)` използва `Calculator.AcumulateWeight(...)`;
- `RoofTableCalc.CalculateNonTranspU(...)` използва претеглено U по площ;
- `FloorTableCalc.CalculateFloorU(...)` прави същото за под към земя;
- `TempBridgeCalc.CalculateSums(...)` изчислява `L * Fi` за мостове.

Тези методи не са самият месечен баланс, но подават `Accumulate*` стойностите, които после влизат в `Htr`.
