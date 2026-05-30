# 00 - Поток на изпълнение

Източник на истината: декомпилираният C# в `reference/eecalc-decompiled`. Документацията в `reference/eecalc-doc` е използвана само като контекст; формулите по-долу са описани само когато са видими в C#.

## Основни входни точки

- `HeatingAndCoolingResultCalc.Calculations(...)` (`HeatingAndCoolingResultCalc.cs:3243`) е основният месечен engine за отопление на зона.
- `HeatingAndCoolingResultCalc.CoolingCalculations(...)` (`HeatingAndCoolingResultCalc.cs:123`) е аналогичният engine за охлаждане; този пакет доклади покрива само зависимите части, които влияят върху `Qgn`.
- `InputDataCalc.CalcPeriod(...)` (`InputDataCalc.cs:13`) избира списъка от месеци и извиква `CalculateMonthlyDays(...)`.
- `VentilationHeatEnergyActual/BaseLine/ESM/Ref1/Ref2(...)` (`HeatingAndCoolingResultCalc.cs:15642`, `15676`, `15710`, `15753`, `15796`) са отделен engine за механична вентилационна топлина.

## Отоплителен баланс

`Calculations(...)` (`HeatingAndCoolingResultCalc.cs:3243`) прави:

1. Запазва текущата зона в статично поле `currentZone`.
2. При референтни стойности клонира `Section` чрез `EntityBase<Section>.Deserialize(section.Serialize())` и прилага `ApplyValuesToTempSectionRef1/Ref2(...)` (`HeatingAndCoolingResultCalc.cs:3257-3265`).
3. Генерира отоплителния период:
   `section.CalcPeriod(FirstMonthHeat, LastMonthHeat, FirstDayHeat, LastDayHeat)` (`HeatingAndCoolingResultCalc.cs:3266`).
4. За всеки `MonthlyDays`:
   - изчислява Ref1/Ref2, ако зоната има референтни стойности (`HeatingAndCoolingResultCalc.cs:3269-3273`);
   - изчислява часове на обитатели и латентна топлина (`HeatingAndCoolingResultCalc.cs:3275-3277`);
   - извиква `CalculateActual(...)` (`HeatingAndCoolingResultCalc.cs:3277`);
   - изчислява BaseLine и ESM (`HeatingAndCoolingResultCalc.cs:3280-3288`);
   - подава `Ni` към входовете от осветление и уреди (`HeatingAndCoolingResultCalc.cs:3290`);
   - попълва отделни ETline стойности за януари и март (`HeatingAndCoolingResultCalc.cs:3292-3375`).
5. След цикъла агрегира месечните списъци към `calcData.ResulNoInputsNetEnergy*` (`HeatingAndCoolingResultCalc.cs:3391-3399`).

## Месечна формула в Actual

`CalculateActual(...)` (`HeatingAndCoolingResultCalc.cs:3483`) изпълнява:

```text
Qtr = CalculateParameterQtr(...)
Qve = CalculateParameterQve(...)
Qht = Qtr + Qve
Qgn = CalculateParameterQgn(...) / 1000
gamma = (Qgn + latentHeatPerMonth * HeatedArea) / Qht
Ni = CalculateParameterNign(...)
NetEnergyQnd = Qht - Ni * Qgn
```

Важно: в `gamma` участва `latentHeatPerMonth * HeatedArea`, но в `NetEnergyQnd` се изважда само `Ni * Qgn` (`HeatingAndCoolingResultCalc.cs:3488-3491`). След връщане в `Calculations(...)` стойността се нормализира по площ и се изважда латентният принос:

```text
NetEnergyQnd_area = (Qht - Ni * Qgn) / HeatedArea - Ni * latentHeatPerMonth
```

Източник: `HeatingAndCoolingResultCalc.cs:3377-3378`.

## Отделен engine за вентилационна топлина

`VentilationHeatEnergyActual(...)` (`HeatingAndCoolingResultCalc.cs:15710`) минава през същия отоплителен период, но работи с дебит, рекуперация, температура на подаване и отоплителни графици. За всеки месец извиква `CalculateMontlyHeatEnergyActual(...)` (`HeatingAndCoolingResultCalc.cs:16141`), събира `num + thermoPumpEnergy`, записва входове от вентилация и накрая попълва:

- `calcData.ResulHeatingInputsActual`;
- `heatCalculations.HeatingResult.ResulVentilationInputsActual`;
- `calcData.ResultEnergyForHeatingActual`.

След това `CalculateVentNeededEnergyActual(...)` (`HeatingAndCoolingResultCalc.cs:15903`) преобразува `ResultEnergyForHeatingActual` към потребна енергия през ефективности.
