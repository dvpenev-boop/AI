# 01 — Heating engine: месечен баланс за отопление

## Основен клас

```text
EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc
```

Ключови методи:

- `CalculateActual(...)` — ред 3482
- `CalculateParameterQtr(...)` — ред 3692
- `CalculateParameterQve(...)` — ред 3662
- `CalculateParameterQgn(...)` — ред 3940
- `CalculateParameterNign(...)` — ред 3631
- `CalculateaH(...)` — ред 3649

## 1. Monthly balance

Декомпилираната логика в `CalculateActual(...)` е:

```csharp
ParameterQtr = CalculateParameterQtr(...);
ParameterQve = CalculateParameterQve(...);
ParameterQht = ParameterQtr + ParameterQve;
ParameterQgn = CalculateParameterQgn(...) / 1000.0;
ParameterGama = (ParameterQgn + latentHeatPerMonth * HeatedArea) / ParameterQht;
ParameterNi = CalculateParameterNign(..., ParameterGama, ...);
NetEnergyQnd = ParameterQht - ParameterNi * ParameterQgn;
```

Формално:

```text
Qht_m = Qtr_m + Qve_m
γ_m = (Qgn_m + Qlatent_m) / Qht_m
η_m = f(γ_m, aH_m)
Qnd_m = Qht_m - η_m * Qgn_m
```

Където:

```text
Qlatent_m = latentHeatPerMonth * HeatedArea
```

Важно: `latentHeatPerMonth` участва в `γ`, но в `NetEnergyQnd` кодът изважда само `η * Qgn`, не `η * (Qgn + latent)`.

## 2. Вентилационен коефициент Hve

Метод: `CalcParameterHve(...)`.

```csharp
ParameterHve = HeatedVolume * InfiltracionActual * 0.34;
```

Формула:

```text
Hve = 0.34 * n * V
```

Единици:

```text
0.34 Wh/(m3*K)
n    1/h
V    m3
Hve  W/K
```

## 3. Qve при отопление

Метод: `CalculateParameterQve(...)`.

```csharp
Qve_m = Hve * (CalcAvgProjectTemp(...) + CalcAvgNonProjectTemp(...)) / 1000.0
```

Формула:

```text
Qve_m = Hve * (Δproj_m + Δnonproj_m) / 1000
```

## 4. Проектна температурна част

Метод: `CalcAvgProjectTemp(...)`.

```text
Hproj_m = W_m * hW + Sat_m * hSat + Sun_m * hSun
Δproj_m = (θset - Te_m) * Hproj_m
```

където:

```text
hW   = WorkCurrentEnd - WorkCurrentStart
hSat = SatCurrentEnd  - SatCurrentStart
hSun = SunCurrentEnd  - SunCurrentStart
```

## 5. Непроектна температурна част

Метод: `CalcAvgNonProjectTemp(...)`.

```text
Hnonproj_m = W_m*(24-hW) + Sat_m*(24-hSat) + Sun_m*(24-hSun) + Holidays_m*24
Δnonproj_m = (θlow - Te_m) * Hnonproj_m
```

Това е реалното място, където влиза `NonProjectTemperatureActual`, т.е. температурата с понижение.

## 6. Qtr при отопление

Метод: `CalculateParameterQtr(...)`.

```csharp
avgTemp = Climate.Month[m].AvgTemp;
avgInner = CalculateAverageHeatTempCurrent(...);
Htr = CalculateParameterHtr(section, avgTemp, avgInner);
Qtr = Htr * (CalcAvgProjectTemp(...) + CalcAvgNonProjectTemp(...)) / 1000.0;
```

Формула:

```text
Qtr_m = Htr_m * (Δproj_m + Δnonproj_m) / 1000
```

Важно: `Htr_m` се пресмята с вътрешна средна температура, за да коригира вътрешни/некондиционирани зони:

```text
θavg_int_m = (Hproj_m*θset + Hnonproj_m*θlow) / (Hproj_m + Hnonproj_m)
```

## 7. Htr

Метод: `CalculateParameterHtr(...)`.

```text
Htr_m = Hd + Hg + Hu
```

където:

```text
Hd = външни стени + прозорци + непрозрачен покрив + прозрачен покрив
Hg = под към земя
Hu = вътрешни елементи към друга зона/неотопляеми пространства
```

## 8. Qgn — топлинни печалби

Метод: `CalculateParameterQgn(...)`.

```csharp
Qgn_raw = (CalculateNonTrasparentFsol(...) + CalculateTrasparentFsol(...)) * (projectHours + nonProjectHours)
ParameterQgn = Qgn_raw / 1000.0
```

Формула:

```text
Fsol_total_m = Fsol_nontransparent_m + Fsol_transparent_m
HoursTotal_m = Hproj_m + Hnonproj_m
Qgn_m = Fsol_total_m * HoursTotal_m / 1000
```

Забележка: от този метод се виждат само соларните печалби. Други вътрешни печалби са отделни блокове (`CalculateQint`, `CalculateQoccupants`) и при cooling/inputs flows.

## 9. η / Ni utilization factor

Метод: `CalculateParameterNign(...)`.

```text
if γ > 0 and |γ-1| > 0.01:
    η = (1 - γ^aH) / (1 - γ^(aH+1))
if γ < 0:
    η = 1
if |γ-1| < 0.01:
    η = aH / (aH + 1)
else:
    η = 0
```

Метод: `CalculateaH(...)`.

```text
aH_m = 1 + τ_m / 15
τ_m = HeatedArea * HeatCapacity / (Htr_m + Hve_m)
```

В кода няма `*3600` в тази конкретна форма; използва се `section.Area.HeatCapacity` в Wh/(m²K) по UI контекст.

## 10. Специален случай: само инфилтрация / без U / без печалби

Ако:

```text
Htr = 0
Qgn = 0
```

тогава:

```text
Qnd_m = Qve_m
qnd = ΣQve_m / A
```

Това беше валидирано срещу примерния случай за зона 9, където се получи около `22.60626 kWh/m²`, т.е. `22.61` при форматиране.
