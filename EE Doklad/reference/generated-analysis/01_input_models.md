# 01 - Входни модели

Тук са описани само моделите, които присъстват като C# файлове или са директно използвани от тях. Много домейн типове се реферират, но не са налични като декомпилирани `.cs` файлове; те са описани в `09_missing_types_and_files.md`.

## Налични локални модели

### `MonthlyDays`

Файл: `EECalcCore.Calculations.MonthlyDays.cs`.

Полета:

- `Month Month`
- `int WorkDays`
- `int Saturdays`
- `int Sundays`
- `int Holydays`
- `int TotalDays`
- `double Weeks`

`MonthlyDays` е централният календарен обект. Той се подава към `CalculateActual(...)`, `CalculateParameterQtr(...)`, `CalculateParameterQve(...)`, `CalculateParameterQgn(...)`, `CalculateParameterNign(...)`, `CalculateQint*`, `CalculateQoccupants*` и вентилационните методи.

### `MonthData`

Файл: `EECalcCore.Calculations.MonthData.cs`.

Полета:

- `MonthlyDays Month`
- `AvgTemp`
- `ParameterQtr`, `ParameterHtr`, `ParamHd`, `ParamHg`, `ParamHu`
- `ParameterHve`, `ParameterQve`
- `ParameterQgn`, `ParameterGama`, `ParameterNi`
- `ParameterQht`
- `NetEnergyQnd`

Това е месечният резултат на отоплителния engine. `CalculateActual(...)` попълва `Qtr`, `Qve`, `Qht`, `Qgn`, `gamma`, `Ni`, `NetEnergyQnd` (`HeatingAndCoolingResultCalc.cs:3483-3491`).

### `MonthDataCooling`

Файл: `EECalcCore.Calculations.MonthDataCooling.cs`.

Съдържа същия набор като `MonthData`, плюс `ParameterQsol`. Използва се от cooling engine-а (`HeatingAndCoolingResultCalc.cs:123` и следващите методи).

### `DataRow` и `BaseLineData`

`DataRow` (`DataRow.cs`) е прост UI/result ред: `Value`, `Tag`, `Fuel`, с `INotifyPropertyChanged`.

`BaseLineData` (`BaseLineData.cs`) групира редове за:

- график;
- U стойности на стени, прозорци, покрив, под и вътрешни елементи;
- инфилтрация и температури;
- нетна, входна, потребна и източникова енергия;
- гориво и ефективности за два генератора.

## Налични helper-и за таблици

- `WallsTableCalc` агрегира площи, U, g, epsilon, alfa за стени и прозорци.
- `RoofTableCalc` агрегира непрозрачен/прозрачен покрив и тавани.
- `FloorTableCalc` агрегира под към земя и други подове.
- `TempBridgeCalc.CalculateSums(...)` изчислява `TypeN_L * TypeN_Fi` и сумарен мост.
- `Calculator.SumFields(...)` и `Calculator.AcumulateWeight(...)` са общи помощници; `Calculator.cs` е наличен.

## Ключови външни входни типове

Следните типове не са налични като `.cs`, но са задължителни за engine-а:

- `Section`: съдържа `Area`, `HeatingSeason`, `HeatingSeasons`, `CoolingSeason`, `Holidays`, всички геометрични направления, `Roof`, `Floor`, `Test`.
- `CalculationData`: съдържа температури, инфилтрация, дебити, ефективности, резултати и входове от осветление/уреди.
- `CalculationInput`: съдържа `General.ClimateZone`.
- `BuildingZone`: съдържа `HasRefenceValues`.
- `ClimateZones`, `SolarRadiationPerMonth`, `ClimateZoneTempHumidityMonth`, `TempHumidityPerDay`: климатични данни.
- `HeatingCalculations` и `CoolingCalculations`: резултатни контейнери за отопление/охлаждане.
- `Walls`, `Roof`, `Floor`, `TempBridge`: домейн геометрия.

Тези липси ограничават пълната типова карта, но не пречат на извеждането на формулите, защото методите използват видими свойства.
