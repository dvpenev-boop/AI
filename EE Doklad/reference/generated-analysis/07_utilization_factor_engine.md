# 07 - Utilization factor engine

## Actual Ni

`CalculateParameterNign(...)` (`HeatingAndCoolingResultCalc.cs:3632`) изчислява коефициента на използване `Ni` от `gamma` и `aH`.

Формула:

```text
if gamma > 0 and abs(gamma - 1) > 0.01:
    Ni = (1 - gamma^aH) / (1 - gamma^(aH + 1))

if gamma < 0:
    Ni = 1

if abs(gamma - 1) < 0.01:
    Ni = aH / (aH + 1)

else:
    Ni = 0
```

Източник: `HeatingAndCoolingResultCalc.cs:3634-3647`.

Бележка: това е точно control-flow редът в C#. Вторият и третият `if` са последователни след първия `return`, не `else if`, но за практически случаи горното описание е еквивалентно.

## Gamma

`CalculateActual(...)` (`HeatingAndCoolingResultCalc.cs:3483`) задава:

```text
gamma = (Qgn + latentHeatPerMonth * HeatedArea) / Qht
```

Източник: `HeatingAndCoolingResultCalc.cs:3488-3490`.

Където:

```text
Qht = Qtr + Qve
```

Източник: `HeatingAndCoolingResultCalc.cs:3485-3487`.

## aH

`CalculateaH(...)` (`HeatingAndCoolingResultCalc.cs:3650`) изчислява:

```text
avgTemp = Climate[month].AvgTemp
averageInnerHeatTemp = CalculateAverageHeatTempCurrent(...)
Hu = SumWallDirecrionsHu1(...) + CalcCeilingsParameterHu2(...) + CalcFloorsParameterHu3(...)
Htr_like = CalculateParameterHdCurrent(section) + CalculateParameterHgCurrent(section) + Hu
Hve = CalcParameterHve(section, calculationdata)
tau = HeatedArea * HeatCapacity / (Htr_like + Hve)
aH = 1 + tau / 15
```

Източник: `HeatingAndCoolingResultCalc.cs:3652-3660`.

## Температурна константа

Кодът не умножава по 3600 и не прави друга конверсия в `CalculateaH(...)`. Единствената видима формула е:

```text
tau = HeatedArea * HeatCapacity / (Htr + Hve)
```

Източник: `HeatingAndCoolingResultCalc.cs:3657-3659`.

## Паралелни реализации

Същата структура се повтаря:

- `CalculateParameterNiEsm(...)` + `CalculateaHesm(...)` (`HeatingAndCoolingResultCalc.cs:4050-4075`);
- `CalculateParameterNignBaseLine(...)` + `CalculateaHbaseLine(...)` (`HeatingAndCoolingResultCalc.cs:4286-4301`);
- `CalculateParameterNignRef1(...)` + `CalculateaHref1(...)` (`HeatingAndCoolingResultCalc.cs:4484-4496`);
- `CalculateParameterNignRef2(...)` + `CalculateaHref2(...)` (`HeatingAndCoolingResultCalc.cs:4732-4744`).

Разликата е само кои свойства се четат: `Actual`, `BaseLine`, `ESM`, `Ref1`, `Ref2`.

## Влияние върху резултатите

`Ni` се използва в три места:

- в месечния баланс: `NetEnergyQnd = Qht - Ni * Qgn` (`HeatingAndCoolingResultCalc.cs:3491`);
- при запис в списъка за латентни печалби: `list9.Add(monthData.ParameterNi * latentHeatPerMonth)` (`HeatingAndCoolingResultCalc.cs:3278`);
- при финално нормализиране: `(Qht - Ni * Qgn) / HeatedArea - Ni * latentHeatPerMonth` (`HeatingAndCoolingResultCalc.cs:3377-3378`).
