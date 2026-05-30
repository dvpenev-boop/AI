# 06 - Qgn / gains engine

В C# има две различни употреби на gains:

- отоплителният баланс `CalculateActual(...)` използва само `CalculateParameterQgn(...)`, което включва соларни печалби;
- cooling/gains блокът има отделни `CalculateQgain*`, които събират соларни, осветление/уреди и обитатели.

## Qgn в отоплителния баланс

`CalculateParameterQgn(...)` (`HeatingAndCoolingResultCalc.cs:3941`) изчислява:

```text
projectHours =
    WorkDays * (WorkCurrentEnd - WorkCurrentStart)
  + Sundays * (SunCurrentEnd - SunCurrentStart)
  + Saturdays * (SatCurrentEnd - SatCurrentStart)

nonProjectHours =
    WorkDays * (24 - (WorkCurrentEnd - WorkCurrentStart))
  + Saturdays * (24 - (SatCurrentEnd - SatCurrentStart))
  + Sundays * (24 - (SunCurrentEnd - SunCurrentStart))
  + Holydays * 24

Qgn_raw = (Fsol_nontransparent + Fsol_transparent)
          * (projectHours + nonProjectHours)
```

Източник: `HeatingAndCoolingResultCalc.cs:3943-3950`.

`CalculateActual(...)` дели резултата на 1000:

```text
Qgn = Qgn_raw / 1000
```

Източник: `HeatingAndCoolingResultCalc.cs:3488`.

## Прозрачни соларни печалби

Единичен прозрачен елемент: `CalculateTransparentFsol(...)` (`HeatingAndCoolingResultCalc.cs:3953`):

```text
radiative = 4 * epsilon * 0.0000000567 * 283^3
loss = 0.04 * g * A * 11 * radiative
factor = 1.0 за horizontal, иначе 0.5
Fsol_transparent = A * g * I - factor * loss
```

Източник: `HeatingAndCoolingResultCalc.cs:3955-3962`.

`CalculateTrasparentFsol(...)` (`HeatingAndCoolingResultCalc.cs:3965`) сумира:

- прозорци по 8 посоки: N, NE, E, SE, S, SW, W, NW;
- прозрачен покрив по 9 позиции;
- за междинни посоки използва средно от две радиации, например `(N + E) / 2`;
- за хоризонтален покрив използва `solarRadiationPerMonth.H` с `horizontal: true`.

Източник: `HeatingAndCoolingResultCalc.cs:3967-3994`.

## Непрозрачни соларни печалби

Единичен непрозрачен елемент: `CalculateNonTransparentFsol(...)` (`HeatingAndCoolingResultCalc.cs:3997`):

```text
absorbed = alfa * 0.04 * U * A
radiative = 4 * epsilon * 0.0000000567 * 283^3
loss = 0.04 * U * A * 11 * radiative
factor = 1.0 за horizontal, иначе 0.5
Fsol_nontransparent = absorbed * I - factor * loss
```

Източник: `HeatingAndCoolingResultCalc.cs:3999-4007`.

`CalculateNonTrasparentFsol(...)` (`HeatingAndCoolingResultCalc.cs:4010`) сумира:

- външни стени по 8 посоки;
- непрозрачен покрив като хоризонтален елемент с `solarRadiationPerMonth.H`.

Източник: `HeatingAndCoolingResultCalc.cs:4012-4031`.

## Cooling/gains блок

`CalculateQgain(...)` (`HeatingAndCoolingResultCalc.cs:1293`) връща:

```text
Qgain = Qsol + Qint + Qoccupants
```

Източник: `HeatingAndCoolingResultCalc.cs:1295-1298`.

Ref1/Ref2/BaseLine/ESM имат същата структура:

- `CalculateQgainRef1(...)` (`HeatingAndCoolingResultCalc.cs:1277`);
- `CalculateQgainRef2(...)` (`HeatingAndCoolingResultCalc.cs:1285`);
- `CalculateQgainBaseLine(...)` (`HeatingAndCoolingResultCalc.cs:1301`);
- `CalculateQgainESM(...)` (`HeatingAndCoolingResultCalc.cs:1309`).

## Вътрешни печалби от осветление и уреди

Actual `CalculateQint(...)` (`HeatingAndCoolingResultCalc.cs:1331`):

```text
Lights =
  if Lights.ByMonths:
      CalcAvgMonthPower(Lights.Actual, month) * (weekRegime * month.Weeks) / 1000
  else:
      Lights.Cooling.PowerActual * (Lights.Cooling.WorkScheduleActual * month.Weeks) / 1000

Devices =
  if BalancedDevices.ByMonths:
      CalcAvgMonthPower(BalancedDevices.Actual, month) * (weekRegime * month.Weeks) / 1000
  else:
      BalancedDevices.Cooling.PowerActual * (BalancedDevices.Cooling.WorkScheduleActual * month.Weeks) / 1000

Qint = Lights * area + Devices * area
```

Източник: `HeatingAndCoolingResultCalc.cs:1331-1335`.

## Печалби от обитатели

Actual `CalculateQoccupants(...)` (`HeatingAndCoolingResultCalc.cs:1352`):

```text
hours = CalculateOccupantshours(section, month)
Qoccupants = MetabolicHeat * hours / 1000 * HeatedArea
```

Източник: `HeatingAndCoolingResultCalc.cs:1354-1356`.

За отоплителния engine директната латентна стойност се изчислява в `Calculations(...)`:

```text
latentHeatPerMonth = MetabolicHeat * OccupantHours(section, month) / 1000
```

Източник: `HeatingAndCoolingResultCalc.cs:3275-3277`.
