# 04 — Gains: слънчеви и вътрешни печалби

## 1. Qgn в heating engine

В `CalculateActual(...)`:

```text
ParameterQgn = CalculateParameterQgn(...) / 1000.0
```

`CalculateParameterQgn(...)` връща W*h преди делене на 1000.

## 2. CalculateParameterQgn

Основната формула:

```text
Qgn_m = (Fsol_nontransparent_m + Fsol_transparent_m) * HoursTotal_m / 1000
```

където:

```text
HoursTotal_m = Hproj_m + Hnonproj_m
```

Методът използва heating schedule, но накрая сумира проектните и непроектните часове, т.е. реално получава всички часове в отоплителния период за месеца.

## 3. Прозрачни соларни печалби

Метод: `CalculateTrasparentFsol(...)`.

За всяка ориентация се взима климатичната радиация:

```text
N, NE=(N+E)/2, E, SE=(S+E)/2, S, SW=(S+W)/2, W, NW=(N+W)/2
```

За покрив има 9 прозрачни позиции, включително хоризонтална с `H`.

Ниско ниво:

```text
Fsol_transparent = A_window * G_window * E_window * I_orientation
```

При horizontal параметърът вика overload с различен sky/geometry correction, но в качения fragment основната структура е тази.

## 4. Непрозрачни соларни печалби

Метод: `CalculateNonTrasparentFsol(...)`.

За стени и покрив:

```text
Fsol_opaque = α * U * ε * A * I_orientation * correction
```

където:

```text
α = absorption coefficient / outer alfa
U = U-value
ε = emissivity или correction field E
A = area
I = solar radiation intensity for orientation/month
```

За покрив се използва хоризонтална радиация `H`.

## 5. Вътрешни печалби — cooling/internal blocks

В качения код вътрешните печалби са по-ясно видими в cooling path:

### Осветление и уреди

Метод: `CalculateQint(...)`.

```text
if Lights.ByMonths:
    Q_lights = CalcAvgMonthPower(Lights.Actual, month) * (weekRegime * month.Weeks) / 1000
else:
    Q_lights = Lights.Cooling.PowerActual * (Lights.Cooling.WorkScheduleActual * month.Weeks) / 1000

if BalancedDevices.ByMonths:
    Q_devices = CalcAvgMonthPower(BalancedDevices.Actual, month) * (weekRegime * month.Weeks) / 1000
else:
    Q_devices = BalancedDevices.Cooling.PowerActual * (BalancedDevices.Cooling.WorkScheduleActual * month.Weeks) / 1000

Qint = (Q_lights + Q_devices) * area
```

### Обитатели

Метод: `CalculateQoccupants(...)`.

```text
hours_occ = CalculateOccupantshours(section, month)
Qoccupants = MetabolicHeat * hours_occ / 1000 * HeatedArea
```

Има и латентна част за occupants:

```text
CalculateQLatentOccupants...
```

## 6. Проверки при comparison

Контролни междинни стойности:

```text
Fsol_transparent_m
Fsol_nontransparent_m
HoursTotal_m
Qgn_m
Qint_m
Qoccupants_m
```

Ако крайният резултат се различава, първо се сравняват:

1. климатична радиация по ориентация;
2. агрегирани A/G/U/E/α;
3. часове от MonthlyDays;
4. дали вътрешните печалби са включени в съответния result path.
