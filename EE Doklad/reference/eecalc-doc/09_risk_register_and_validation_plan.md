# 09 — Risk register and validation plan

## 1. Основни рискове за несъвпадение

### R1 — MonthlyDays

Най-критично. Трябва да се повтори:

```text
- фиксирана година 2006
- shortcut правила за частични месеци
- holidays се изваждат от WorkDays
- Weeks = (days - holidays)/7
```

### R2 — Schedule hours

В някои места кодът прави `End - Start`; helper `CalcHours` обработва crossing midnight. Нужно е да се установи къде UI допуска график през полунощ.

### R3 — Scenario suffixes

Ref1/Ref2/Actual/BaseLine/ESM често ползват различни property имена:

```text
Current vs Actual
Base vs BaseLine
ESM vs Esm
```

### R4 — Decompiled artifacts

Подозрителни редове:

```text
SumWallDirecrionsHu1 повтаря NorthWalls
InnerU = wall.IneerA5
CeilingU = roof.CeilingA5
```

Тези трябва да се проверят срещу IL или runtime тест.

### R5 — Units

Кодът смесва:

```text
W/K
Wh
kWh
kWh/m²
percent efficiencies
```

Най-често `/1000` и `/A` са източник на грешки.

### R6 — NaN/Infinity guards

Много методи нулират резултат при `NaN` или `Infinity`. Новият софтуер трябва да копира това поведение.

## 2. Препоръчана архитектура за новия engine

```text
Core
 ├── CalendarEngine / MonthlyDays
 ├── ScheduleEngine
 ├── ClimateProvider
 ├── TransmissionEngine
 ├── VentilationHeatEngine
 ├── GainsEngine
 ├── UtilizationEngine
 ├── HeatingBalanceEngine
 ├── CoolingBalanceEngine
 ├── SystemsEngine
 └── ResultsAggregator
```

## 3. Минимален validation output

За всеки месец log:

```text
Month
Te
WorkDays/Saturdays/Sundays/Holidays/Weeks
Hproj
Hnonproj
θavg
Hve
Htr/Hd/Hg/Hu
Qtr
Qve
Qht
FsolTransparent
FsolNonTransparent
Qgn
Gamma
Ni
Qnd
```

За системи:

```text
VentilationInputs
LightInputs
AppliancesInputs
FansAndPumps
NeededEnergy
PrimaryEnergy
FuelEnergy
CO2
```

## 4. Стъпки за сравнение с твоя софтуер

1. Пусни примерен проект в EECalc и експортирай/заснеми всички междинни резултати.
2. Пусни същия input в твоя софтуер.
3. Сравни първо `MonthlyDays`. Ако не съвпада — спри; всичко надолу ще е различно.
4. Сравни `Hproj/Hnonproj`.
5. Сравни `Hve/Htr`.
6. Сравни `Qtr/Qve/Qgn`.
7. Сравни `Gamma/Ni`.
8. Сравни `Qnd`.
9. Едва после сравнявай системи, fuels, primary, CO2.

## 5. Следващи файлове, които са нужни за пълна реконструкция

```text
CalculationData.cs
CalculationInput.cs
Section.cs
BuildingZone.cs
MonthData.cs
MonthlyDays.cs
InputDataCalc.cs
HeatingCalculations.cs
CoolingCalculations.cs
Results.cs
Fuel.cs
DataRow.cs
SunEnergyCalculationData.cs
```

Без тях формулите могат да се възстановят, но mapping-ът към UI и дефолтните стойности остава частично несигурен.
