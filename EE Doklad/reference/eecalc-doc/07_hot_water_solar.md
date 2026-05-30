# 07 — Domestic hot water and solar calculations

## 1. Ключов entry point

Метод:

```text
CalculateHotWaterNeededPower(this SunEnergyCalculationData sunEnergyCalculationData, Section section, CalculationInput calcInput)
```

В същия блок има:

```text
CalculateParameterF
CalculateParameterX
CalculateParameterY
CalculateTOAeffect
HotWaterNeededPower
HotWaterNeededPowerTotal
DefuseradiationHd
SunDeclination
SunsetHour
SunsetHourPrim
CalculateMonthlyHorizontalRadiation
CalculateProjectionCoeficient
CalculateParameterHtMonthly
```

## 2. Соларен f-chart модел

Метод: `CalculateParameterF(x, y)`.

По имената и нормативния модел това е f-chart формулата:

```text
f = 1.029*Y - 0.065*X - 0.245*Y² + 0.0018*X² + 0.0215*Y³
```

`CalculateXwithCorrection(x)` подсказва корекция на X при различен обем на акумулатора.

## 3. X и Y параметри

Методите:

```text
CalculateParameterX(calcInput, month, neededHotWaterEnergyforMonth)
CalculateParameterY(calcInput, month, neededHotWaterEnergyforMonth)
```

са вход към f-chart. Очаквани зависимости:

```text
X ∝ Ac * FRUL * (Tref - Te) * Δt / Qw
Y ∝ Ac * FR(τα) * Ht * N / Qw
```

Точните полета трябва да се извадят от метод body при следващ pass, ако БГВ е приоритет.

## 4. Гореща вода

Методи:

```text
HotWaterNeededPower(...)
HotWaterNeededPowerTotal(...)
```

Очаквана структура:

```text
Qw = Vw * 1.161 * (θhot - θcold)
```

но точните UI fields трябва да се потвърдят с класовете `SunEnergyCalculationData`, `SunParameters`, `SunPreferences`.

## 5. Слънчева геометрия

Методите:

```text
SunDeclination(month)
SunsetHour(month)
SunsetHourPrim(month)
CalculateProjectionCoeficient(...)
CalculateParameterHtMonthly(...)
```

използват monthly solar radiation и проекционен коефициент за наклонена повърхност.

## 6. Статус

Този блок е идентифициран, но не е напълно формализиран, защото качените `EECalcCore.SunPreferences.cs` и `EECalcCore.Preferences.cs` са практически празни decompiled stubs. За пълна реконструкция на БГВ ще трябват моделните класове:

```text
SunEnergyCalculationData
SunEnergyResMonth
SunMonth
SunParameters
HotWaterCalculations
```
