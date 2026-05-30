# 05 - Qve / ventilation engine

Тук има две различни логики:

- инфилтрационно `Qve` в отоплителния баланс;
- отделен engine за механична вентилационна топлина.

## Инфилтрационно Qve в отоплителния баланс

`CalculateParameterQve(...)` (`HeatingAndCoolingResultCalc.cs:3663`) връща:

```text
Qve_m = Hve * (DeltaProject_m + DeltaNonProject_m) / 1000
```

Източник: `HeatingAndCoolingResultCalc.cs:3665`.

`CalcParameterHve(...)` (`HeatingAndCoolingResultCalc.cs:3687`) връща:

```text
Hve = HeatedVolume * InfiltracionActual * 0.34
```

Източник: `HeatingAndCoolingResultCalc.cs:3689`.

`DeltaProject_m` и `DeltaNonProject_m` са същите като при `Qtr`:

- `CalcAvgProjectTemp(...)` (`HeatingAndCoolingResultCalc.cs:3668-3675`);
- `CalcAvgNonProjectTemp(...)` (`HeatingAndCoolingResultCalc.cs:3677-3685`).

Следователно:

```text
Qve_m =
  0.34 * HeatedVolume * InfiltracionActual
  * (DeltaProject_m + DeltaNonProject_m)
  / 1000
```

Това е формула от `CalculateParameterQve(...)` + `CalcParameterHve(...)`.

## Вентилационни седмични часове

`GetWeekHoursResultActual(...)` (`HeatingAndCoolingResultCalc.cs:3419`) използва `section.CalcHours(...)`:

```text
weekHours =
  5 * CalcHours(WorkCurrentStart, WorkCurrentEnd)
  + CalcHours(SunCurrentStart, SunCurrentEnd), ако > 0
  + CalcHours(SatCurrentStart, SatCurrentEnd), ако > 0
```

Източник: `HeatingAndCoolingResultCalc.cs:3421-3432`.

За месечните отоплителни `Qve` формули обаче кодът използва директно `End - Start`, не `CalcHours`.

## Механична вентилационна топлина

Входни точки:

- `VentilationHeatEnergyRef1(...)` (`HeatingAndCoolingResultCalc.cs:15642`);
- `VentilationHeatEnergyRef2(...)` (`HeatingAndCoolingResultCalc.cs:15676`);
- `VentilationHeatEnergyActual(...)` (`HeatingAndCoolingResultCalc.cs:15710`);
- `VentilationHeatEnergyBaseLine(...)` (`HeatingAndCoolingResultCalc.cs:15753`);
- `VentilationHeatEnergyESM(...)` (`HeatingAndCoolingResultCalc.cs:15796`).

Actual поток:

```text
VentilationHeatEnergyActual(...)
  -> section.CalcPeriod(...)
  -> for each month:
       CalculateMontlyHeatEnergyActual(..., out thermoPumpEnergy)
       list.Add(num + thermoPumpEnergy)
       ResulHeatingInputsActual += DebitActual * 0.34 *
           (FlowTemperatureActual - ProjectTemperatureActual) *
           monthHours / 1000
  -> ResultEnergyForHeatingActual = sum(list)
```

Източник: `HeatingAndCoolingResultCalc.cs:15715-15750`.

## Месечна механична вентилация Actual

`CalculateMontlyHeatEnergyActual(...)` (`HeatingAndCoolingResultCalc.cs:16141`) прави:

```text
monthHours = GetMonthHoursActual(month, section)
innerTemp = CalculateAverageVentHeatTempActual(section, heatCalculations, month)
avgTemp = Climate[month].AvgTemp
humidity = average(Climate.TempHumidity[month].Hours.Humidity)

t_after_first_rec = innerTemp - FirstRecEfficiencyActual / 100 * (innerTemp - avgTemp)
t_after_recovery_mix = innerTemp - t_after_first_rec + avgTemp
```

Източник: `HeatingAndCoolingResultCalc.cs:16143-16150`.

Ако `SecondRecEfficiencyActual > 0` и `HeatingAirDifferenceActual` е между 3 и 8 включително:

```text
h1 = CalcEntalpia(t_after_first_rec, humidity, Pb)
h2 = CalcEntalpia(MinimumEndTemperatureActual, humidity, Pb)
raw = DebitActual * 1.2 * (h1 - h2) * monthHours / 3600
thermoPumpEnergy = raw / (1 - 100 / SecondRecEfficiencyActual)
deltaT = thermoPumpEnergy * 1000 / (DebitActual * 0.34 * monthHours)
```

Източник: `HeatingAndCoolingResultCalc.cs:16151-16159`.

Ограничения:

- ако `deltaT >= HeatingAirDifferenceActual`, `thermoPumpEnergy = DebitActual * 0.34 * HeatingAirDifferenceActual * monthHours / 1000`;
- ако `deltaT < FlowTemperatureActual - t_after_recovery_mix`, връща `DebitActual * 0.34 * (FlowTemperatureActual - adjustedT) * monthHours / 1000`;
- иначе `thermoPumpEnergy = DebitActual * 0.34 * (FlowTemperatureActual - t_after_recovery_mix) * monthHours / 1000` и методът връща `0`.

Източник: `HeatingAndCoolingResultCalc.cs:16160-16170`.

Ако няма вторична рекуперация:

```text
energy = DebitActual * 0.34 * (FlowTemperatureActual - t_after_recovery_mix) * monthHours / 1000
```

Източник: `HeatingAndCoolingResultCalc.cs:16171-16176`.

## Потребна енергия

`CalculateVentNeededEnergyActual(...)` (`HeatingAndCoolingResultCalc.cs:15903`) дели енергията по два генератора:

```text
source1 = ResultEnergyForHeatingActual * Part1Actual / 100
          / (TransmitTempEfficiencyActual/100
             * SupplyNetEfficiencyActual/100
             * AutomaticActual/100
             * EnergyManagementActual/100
             * GeneratorHeatEfficiency1Actual/100)

source2 = ResultEnergyForHeatingActual * Part2Actual / 100
          / (TransmitTempEfficiency2Actual/100
             * SupplyNetEfficiency2Actual/100
             * Automatic2Actual/100
             * EnergyManagement2Actual/100
             * GeneratorHeatEfficiency2Actual/100)

ResultNeededEnergyActual = source1 + source2
```

Източник: `HeatingAndCoolingResultCalc.cs:15920-15932`.
