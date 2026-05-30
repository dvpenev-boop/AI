# 08 — Results, net/needed/primary energy and fuels

## 1. Общ result flow

В `BuildingCalculations(this Results buildingBalanceResult, CalculationInput calcInput, Results zoneBalanceResult)` беше намерен flow:

```text
GetBuildingData
GetConditionedArea
ClearNeededVEIenergy
UpdateRefsState
UpdateActualState
UpdateBaseLineState
UpdateEsmState
CalculateTotalsNeededEnergyTable
ClearFuelCells
ClearNetEnergy
ClearNetEnergyWithoutInputs
ClearPrimaryEnergy
ClearPrimaryEnergyFuelTableValues
foreach BuildingZone:
    CalculateNetEnergyByTechnologiesBuilding
    CalculateNetWithoutInputsEnergyByTechnologies
    CalculatePrimaryEnergyByTechnologies
    GetPrimaryFuelTypeAndValues
    GetFuelTypeAndValues
SetFuelValue
CalculateNetEnergyPerArea
CalculateNetWithoutInputsEnergyByTechnologiesPerArea
CalculatePrimaryEnergyPerArea
CalculatePrimaryFuelTypeAndValuesPerArea
BuildingCO2Calculations
CalculateTotalFuelEnergy
CalculatePrimaryEnergyFuelTotal
CalculatePrimaryTotalEnergy
CalculateBuildingPowerEnergy
CalculateTotalVei
SetScaleValues
```

## 2. Net energy

Метод:

```text
CalculateNetEnergy(this CalculationData heatingCalculations)
```

Формула:

```text
ResulNetEnergy = ResulNoInputsNetEnergy
               - (ResulVentilationInputs + ResulLightInputs + ResulAppliancesInputs)
```

За всеки режим има отделни полета:

```text
Ref1, Ref2, Actual, BaseLine, ESM
```

## 3. Needed energy / source energy

В кода има групи методи:

```text
CalculateNeededEnergy...
CalculateGeneratorHeatEfficiency...
CalculateTotalPrimary...
GetPrimaryFuelType...
CalculateTotalPrimaryFuel...
GetFuelType...
```

Типичният pattern е:

```text
NeededEnergy = NetEnergy / product_of_efficiencies * 100^n
PrimaryEnergy = NeededEnergy * primary_factor
FuelEnergy = split_by_fuel_parts
```

Точните формули трябва да се извадят от body на тези методи, но имената и полетата показват следните участници:

```text
Part1 / Part2
TransmitTempEfficiency1/2
SupplyNetEfficiency1/2
Automatic1/2
EnergyManagement1/2
GeneratorHeatEfficiency1/2
ResultSourceEnergy1/2
Fuel1 / Fuel2
```

## 4. BaseLineData структура

`BaseLineData.cs` дефинира таблицата за отопление/охлаждане/резултати:

```text
WorkingSchedule
UouterWalls
Uwindows
Unontransparent
Ufloor
G
UinnerWalls
Uceiling
UfloorOther
Infiltracion
ProjectTemperature
NonProjectTemperature
ResulNoInputsNetEnergy
ResulVentilationInputs
ResulLightInputs
ResulAppliancesInputs
ResulNetEnergy
Fuel1/Fuel2
Part1/Part2
TransmitTempEfficiency1/2
SupplyNetEfficiency1/2
Automatic1/2
EnergyManagement1/2
GeneratorHeatEfficiency1/2
ResultSourceEnergy/ResultSourceEnergy2
HeatEfficiencyGenerating
ResultNeededEnergy
```

## 5. Режими

Почти всички таблици имат пет режима:

```text
Ref1
Ref2
Actual / Current
BaseLine
ESM
```

Архитектурата е copy-paste със смяна на property suffix. За имплементация е по-добре да се направи generic `Scenario` модел, но при 1:1 сравнение трябва да се проверят конкретните имена и fallback-и за `NaN/Infinity`.

## 6. Контролни точки за comparison

```text
NoInputsNetEnergy by service
VentilationInputs
LightInputs
AppliancesInputs
NetEnergy
NeededEnergy
PrimaryEnergy
FuelEnergy by fuel
CO2
Scale class
```

Най-често крайната разлика идва не от `Qnd`, а от грешно приложени:

```text
efficiency percentages
fuel shares
primary energy factors
area normalization
rounding/NaN guards
```
