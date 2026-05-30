# 09 - Липсващи типове и файлове

Декомпилираният пакет съдържа 20 C# файла, но основните домейн модели не са налични като отделни файлове. Това ограничава пълна типова карта, но формулите в докладите са извлечени от наличния C#.

## Налични C# файлове

- `EECalcCore.Calculations.BuildingTypesManager.cs`
- `EECalcCore.Calculations.Calculator.cs`
- `EECalcCore.Calculations.DataRow.cs`
- `EECalcCore.Calculations.InputDataCalc.cs`
- `EECalcCore.Calculations.MonthData.cs`
- `EECalcCore.Calculations.MonthDataCooling.cs`
- `EECalcCore.Calculations.MonthlyDays.cs`
- `EECalcCore.Calculations.PreferencesManager.cs`
- `EECalcCore.Calculations.SavingsData.cs`
- `EECalcCore.Calculations.SunEnergyPreferencesManager.cs`
- `EECalcCore.Calculations.SunMonth.cs`
- `EECalcCore.Calculations.TableCalculations.BaseLineData.cs`
- `EECalcCore.Calculations.TableCalculations.FloorTableCalc.cs`
- `EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs`
- `EECalcCore.Calculations.TableCalculations.RoofTableCalc.cs`
- `EECalcCore.Calculations.TableCalculations.TempBridgeCalc.cs`
- `EECalcCore.Calculations.TableCalculations.WallsTableCalc.cs`
- `EECalcCore.cs`
- `EECalcCore.Preferences.cs`
- `EECalcCore.SunPreferences.cs`

## Липсващи основни домейн типове

Следните типове се използват от наличния C#, но няма техни дефиниции в `reference/eecalc-decompiled`:

- `Section`
- `CalculationData`
- `CalculationInput`
- `BuildingZone`
- `BuildingZones`
- `Results`
- `HeatingCalculations`
- `CoolingCalculations`
- `Walls`, `WallsStates`
- `Roof`, `RoofStates`
- `Floor`, `FloorStates`
- `TempBridge`
- `Fuel`
- `Month`
- `ClimateZones`
- `SolarRadiationPerMonth`
- `ClimateZoneTempHumidityMonth`
- `TempHumidityPerDay`
- `ScheduleMonth`
- `SunEnergyCalculationData`
- `SunEnergyResMonth`
- `BuildingCategories`
- `BuildingScaleTypes`
- `Scale`

## Липсващи/външни членове, от които зависят формулите

`Section` трябва да съдържа поне:

- `Area.HeatedArea`, `Area.HeatedVolume`, `Area.HeatCapacity`, `Area.MetabolicHeat`;
- `Area.ETlineData.*`;
- `HeatingSeason.FirstMonthHeat`, `LastMonthHeat`, `FirstDayHeat`, `LastDayHeat`;
- `CoolingSeason.FirstMonthCool`, `LastMonthCool`, `FirstDayCool`, `LastDayCool`;
- `HeatingSeasons.Heating.*Start/*End`;
- `HeatingSeasons.Occupants.*Start/*End`;
- `HeatingSeasons.Ventilation.*Start/*End`;
- `Holidays.January` ... `Holidays.December`;
- `NorthWalls`, `NorthEastWalls`, `EastWalls`, `SouthEastWalls`, `SouthWalls`, `SouthWestWalls`, `WestWalls`, `NorthWestWalls`;
- `Roof.Current/BaseLine/Esm`, `Floor.Current/BaseLine/Esm`;
- `Test.ParameterHtr`, `ParameterHve`, `ParameterHd`, `ParameterHg`, `ParameterHu`.

`CalculationData` трябва да съдържа поне:

- `ProjectTemperatureActual/BaseLine/ESM/Ref1/Ref2`;
- `NonProjectTemperatureActual/BaseLine/ESM/Ref1/Ref2`;
- `InfiltracionActual/BaseLine/ESM/Ref1/Ref2`;
- `DebitActual/BaseLine/ESM/Ref1/Ref2`;
- `FlowTemperatureActual/BaseLine/ESM/Ref1/Ref2`;
- `FirstRecEfficiency*`, `SecondRecEfficiency*`;
- `HeatingAirDifference*`, `MinimumEndTemperature*`;
- `Part1*`, `Part2*`;
- `TransmitTempEfficiency*`, `SupplyNetEfficiency*`, `Automatic*`, `EnergyManagement*`;
- `GeneratorHeatEfficiency1*`, `GeneratorHeatEfficiency2*`;
- `Lights`, `BalancedDevices`;
- `ResultEnergyForHeating*`, `ResultSourceEnergy*`, `ResultSourceEnergy2*`, `ResultNeededEnergy*`;
- `ResulNoInputsNetEnergy*`, `ResulHeatingInputs*`.

Климатичните типове трябва да предоставят:

- `PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[index].AvgTemp`;
- посочни радиации `N`, `E`, `S`, `W`, `H`;
- `TempHumidity.Months[index].Hours` със `Temp` и `Humidity`;
- барометрично налягане `Pb`.

## Липсващи методи или методи извън фокуса

Някои методи са използвани, но не са разгърнати в този пакет доклади, защото са извън деветте заявени engine карти или изискват липсващи модели:

- `ApplyValuesToTempSectionRef1(...)`, `ApplyValuesToTempSectionRef2(...)`
- `CalculateRef1(...)`, `CalculateRef2(...)`
- `CalculateBaseLine(...)`, `CalculateEsm(...)` са частично разгледани чрез видимите им Qtr/Qve/Qgn/Ni клони.
- `CalculateQsol*`, `CalculateQLatentOccupants*`, `CalculateOccupantshours*`
- `GetLightsAndDevicesInputs(...)`
- `CheckForNaN(...)`
- `CalcAirX(...)`
- `CalculateHotWaterNeededPower(...)` и свързаните БГВ/слънчеви методи
- `BuildingCalculations(...)`, `ZoneCalculations(...)`, `CO2EnergyZoneCalculations(...)`

## Декомпилационни рискове

Наличните файлове започват с предупреждение, че някои assembly references не са разрешени автоматично. Наблюдавани рискови места:

- `SumWallDirecrionsHu1(...)` използва `section.NorthWalls.Current` осем пъти вместо осем различни посоки (`HeatingAndCoolingResultCalc.cs:3838-3848`).
- В `CalcWallDirectionParameterHu1(...)` за пети вътрешен елемент `innerU = wall.IneerA5`, а не видимо `InnerU5` (`HeatingAndCoolingResultCalc.cs:3870-3872`).
- В `CalcCeilingsParameterHu2(...)` за пети таван `ceilingU = roof.CeilingA5`, а не видимо `CeilingU5` (`HeatingAndCoolingResultCalc.cs:3900-3902`).
- Има имена с кирилска буква в идентификатор, например `NonTransparentРђ6` и `TransparentРђ6`, което може да е декомпилационен или encoding артефакт.

Тези места не са поправяни в анализа; те са документирани като source-of-truth поведение на наличния C#.
