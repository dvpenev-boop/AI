# 08 - Агрегиране на резултатите

## Месечни отоплителни списъци

`Calculations(...)` (`HeatingAndCoolingResultCalc.cs:3243`) събира отделни списъци:

- `list4`: actual `monthData.NetEnergyQnd`;
- `list5`: BaseLine `CalculateBaseLine(...)`;
- `list6`: ESM `CalculateEsm(...)`;
- `list7/list8`: Ref1/Ref2 латентни или входни корекции;
- `list9/list10/list11`: `Ni * latentHeatPerMonth` за Actual/BaseLine/ESM.

Източник: `HeatingAndCoolingResultCalc.cs:3246-3256`, `3277-3290`.

## Финални no-input резултати

След месечния цикъл:

```text
ResulNoInputsNetEnergyActual =
    CheckForNaN(sum(list4) / HeatedArea - sum(list9))

ResulNoInputsNetEnergyBaseLine =
    CheckForNaN(sum(list5) / HeatedArea - sum(list10))

ResulNoInputsNetEnergyESM =
    CheckForNaN(sum(list6) / HeatedArea - sum(list11))
```

Източник: `HeatingAndCoolingResultCalc.cs:3391-3399`.

Ref1/Ref2:

```text
ResulNoInputsNetEnergyRef1 =
    CheckForNaN(sum(list2) / HeatedArea - sum(list7))

ResulNoInputsNetEnergyRef2 =
    CheckForNaN(sum(list3) / HeatedArea - sum(list8))
```

Източник: `HeatingAndCoolingResultCalc.cs:3382-3390`.

## Месечно ETline попълване

За януари и март `Calculations(...)` записва отделни месечни стойности към `section.Area.ETlineData`. Формулната форма за Actual е:

```text
monthlyNoInput = monthData.NetEnergyQnd - Ni * latentHeatPerMonth * HeatedArea

source1 =
  monthlyNoInput * Part1Actual / 100
  / (TransmitTempEfficiencyActual/100
     * SupplyNetEfficiencyActual/100
     * AutomaticActual/100
     * EnergyManagementActual/100
     * GeneratorHeatEfficiency1Actual/100)

source2 =
  monthlyNoInput * Part2Actual / 100
  / (TransmitTempEfficiency2Actual/100
     * SupplyNetEfficiency2Actual/100
     * Automatic2Actual/100
     * EnergyManagement2Actual/100
     * GeneratorHeatEfficiency2Actual/100)

MonthHeatingEnergy.Actual = source1 + source2
```

Източник за януари: `HeatingAndCoolingResultCalc.cs:3292-3305`.  
Източник за март: `HeatingAndCoolingResultCalc.cs:3334-3348`.

Същият шаблон се повтаря за BaseLine и ESM (`HeatingAndCoolingResultCalc.cs:3307-3332`, `3349-3375`).

## Потребна енергия за вентилационно отопление

`CalculateVentNeededEnergyActual(...)` (`HeatingAndCoolingResultCalc.cs:15903`) изчислява:

```text
ResultSourceEnergyActual =
  ResultEnergyForHeatingActual * Part1Actual / 100 / product(efficiencies1)

ResultSourceEnergy2Actual =
  ResultEnergyForHeatingActual * Part2Actual / 100 / product(efficiencies2)

ResultNeededEnergyActual =
  ResultSourceEnergyActual + ResultSourceEnergy2Actual
```

Източник: `HeatingAndCoolingResultCalc.cs:15920-15932`.

Ако `SecondRecEfficiencyActual > 100`, методът използва вече разделени `ResultSourceEnergyActual` и `ResultSourceEnergy2Actual` и само ги дели на ефективностите (`HeatingAndCoolingResultCalc.cs:15905-15918`).

## Защита от NaN/Infinity

В множество резултатни формули C# проверява:

```text
if double.IsInfinity(value) || double.IsNaN(value):
    value = 0
```

Примери:

- месечни ETline източникови енергии (`HeatingAndCoolingResultCalc.cs:3295-3304`);
- `CalculateVentNeededEnergyActual(...)` (`HeatingAndCoolingResultCalc.cs:15907-15916`, `15921-15930`).

`CheckForNaN(...)` е извикан при финалните агрегати, но самият метод не е разгледан тук, защото не беше нужен за извеждане на формулната карта.

## Резултатни контейнери

Директно налични файлове:

- `MonthData` и `MonthDataCooling` пазят месечните параметри.
- `BaseLineData` описва UI/result редове за сравнение между reference/base/actual/ESM.
- `DataRow` е единичен резултатен ред с `Value`, `Tag`, `Fuel`.

Основните големи резултатни типове `Results`, `HeatingCalculations`, `CoolingCalculations`, `CalculationData` липсват като `.cs` и са описани в `09_missing_types_and_files.md`.
