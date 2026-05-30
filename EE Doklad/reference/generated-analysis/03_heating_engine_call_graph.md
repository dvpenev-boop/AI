# 03 - Call graph на отоплителния engine

## Главен поток

```text
CalculationData.Calculations(section, calcInput, zone, lightsAndDevicesCalculationData)
  -> section.CalcPeriod(...)
     -> section.CalculateMonthlyDays(...)
  -> CalculateRef1(...) / CalculateRef2(...) [само при zone.HasRefenceValues]
  -> OccupantHours(...)
  -> CalculateActual(...)
       -> CalculateParameterQtr(...)
            -> CalculateAverageHeatTempCurrent(...)
            -> CalculateParameterHtr(...)
                 -> SumWallDirecrionsHu1(...)
                 -> CalcCeilingsParameterHu2(...)
                 -> CalcFloorsParameterHu3(...)
                 -> CalculateParameterHdCurrent(...)
                 -> CalculateParameterHgCurrent(...)
            -> CalcAvgProjectTemp(...)
            -> CalcAvgNonProjectTemp(...)
       -> CalculateParameterQve(...)
            -> CalcParameterHve(...)
            -> CalcAvgProjectTemp(...)
            -> CalcAvgNonProjectTemp(...)
       -> CalculateParameterQgn(...)
            -> CalculateNonTrasparentFsol(...)
                 -> CalculateNonTransparentFsol(...)
            -> CalculateTrasparentFsol(...)
                 -> CalculateTransparentFsol(...)
       -> CalculateParameterNign(...)
            -> CalculateaH(...)
                 -> CalculateAverageHeatTempCurrent(...)
                 -> SumWallDirecrionsHu1(...)
                 -> CalcCeilingsParameterHu2(...)
                 -> CalcFloorsParameterHu3(...)
                 -> CalculateParameterHdCurrent(...)
                 -> CalculateParameterHgCurrent(...)
                 -> CalcParameterHve(...)
  -> CalculateBaseLine(...)
  -> CalculateEsm(...)
  -> CalculateLightsAndDevicesInputs(...)
  -> aggregate result lists
```

Главна входна точка: `HeatingAndCoolingResultCalc.Calculations(...)` (`HeatingAndCoolingResultCalc.cs:3243`).

## Actual месечен баланс

`CalculateActual(...)` (`HeatingAndCoolingResultCalc.cs:3483`) е най-компактният израз на отоплителния engine:

```text
Qht = Qtr + Qve
gamma = (Qgn + latentHeatPerMonth * HeatedArea) / Qht
Qnd = Qht - Ni * Qgn
```

Източник: `HeatingAndCoolingResultCalc.cs:3485-3491`.

## Qtr клон

`CalculateParameterQtr(...)` (`HeatingAndCoolingResultCalc.cs:3693`) прави:

```text
avgTemp = Climate[month].AvgTemp
averageInnerHeatTemp = CalculateAverageHeatTempCurrent(...)
Htr = CalculateParameterHtr(section, avgTemp, averageInnerHeatTemp)
Qtr = Htr * (DeltaProject + DeltaNonProject) / 1000
```

Източник: `HeatingAndCoolingResultCalc.cs:3695-3699`.

## Qve клон

`CalculateParameterQve(...)` (`HeatingAndCoolingResultCalc.cs:3663`):

```text
Qve = Hve * (DeltaProject + DeltaNonProject) / 1000
```

Източник: `HeatingAndCoolingResultCalc.cs:3665`.

`CalcParameterHve(...)`:

```text
Hve = HeatedVolume * InfiltracionActual * 0.34
```

Източник: `HeatingAndCoolingResultCalc.cs:3687-3690`.

## Qgn клон

`CalculateParameterQgn(...)` (`HeatingAndCoolingResultCalc.cs:3941`) изчислява общи проектни и непроектни часове и умножава по сумарния соларен поток:

```text
Qgn_raw = (Fsol_nontransparent + Fsol_transparent) * (projectHours + nonProjectHours)
Qgn = Qgn_raw / 1000
```

Източник: `HeatingAndCoolingResultCalc.cs:3943-3950`, делението е в `CalculateActual(...)` (`HeatingAndCoolingResultCalc.cs:3488`).

## Ni клон

`CalculateParameterNign(...)` (`HeatingAndCoolingResultCalc.cs:3632`) използва `aH` от `CalculateaH(...)`:

```text
if gamma > 0 and abs(gamma - 1) > 0.01:
    Ni = (1 - gamma^aH) / (1 - gamma^(aH + 1))
elif gamma < 0:
    Ni = 1
elif abs(gamma - 1) < 0.01:
    Ni = aH / (aH + 1)
else:
    Ni = 0
```

Източник: `HeatingAndCoolingResultCalc.cs:3634-3647`.

## BaseLine, ESM, Ref1, Ref2

Има паралелни методи със същата структура:

- ESM: `CalculateEsm(...)`, `CalculateParameterNiEsm(...)`, `CalculateaHesm(...)`, `CalculateParameterQveEsm(...)`, `CalculateParameterQtrEsm(...)`, `CalculateParameterQgnEsm(...)` (`HeatingAndCoolingResultCalc.cs:4039-4194`).
- BaseLine: `CalculateaHbaseLine(...)`, `CalculateParameterNignBaseLine(...)`, `CalculateParameterQtrBaseLine(...)`, `CalculateParameterQveBaseLIne(...)`, `CalculateParameterQgnBaseLine(...)` (`HeatingAndCoolingResultCalc.cs:4286-4391`).
- Ref1/Ref2: `CalculateParameterQtrRef1/Ref2`, `CalculateParameterQveRef1/Ref2`, `CalculateParameterNignRef1/Ref2` (`HeatingAndCoolingResultCalc.cs:4421-4744`).

Формулната форма е същата, но полетата идват от `Ref1`, `Ref2`, `BaseLine` или `ESM` свойства.
