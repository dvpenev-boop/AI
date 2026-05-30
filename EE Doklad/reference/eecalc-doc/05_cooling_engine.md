# 05 — Cooling engine

## 1. Основен flow

В `HeatingAndCoolingResultCalc.cs` има паралелен cooling engine:

```text
CoolingCalculations(...)
CalculateCoolingEnergyRef1/Ref2/Actual/BaseLine/ESM(...)
CalculateCoolingQtr...
CalculateQinf...
CalculateQve...
CalculateQgain...
CalculateETA...
CalculateAc...
```

Cooling работи с `section.CoolingSeasons` вместо `section.HeatingSeasons`.

## 2. Cooling Qtr

Подобно на отоплението:

```text
Qtr_cool_m = Htr_cool_m * cooling_temperature_hours / 1000
```

Използва:

```text
CalculateAverageCoolingTempCurrent(...)
CalculateCoolingHtr(...)
```

Средната вътрешна температура при охлаждане е:

```text
θavg_cool = (Hproj*θproject + Hnonproj*θnonproject) / (Hproj + Hnonproj)
```

## 3. Cooling infiltration Qinf

Метод: `CalculateQinf(...)`.

```text
Qinf_m = Hinf * (CalcAvgProjectTempCooling + CalcAvgNonProjectTempCooling) / 1000
```

където:

```text
Hinf = 0.34 * n * V
```

с cooling режим и cooling температури.

## 4. Cooling ventilation Qve

Метод: `CalculateQve(...)` за cooling ventilation е по-детайлен от heating infiltration, защото обхожда час по час в рамките на деня и сравнява графика на вентиляция с графика на occupants.

За всеки час:

```text
θhour = ProjectTemperatureActual ако occupants режимът е активен
        иначе NonProjectTemperatureActual
Qhour = Hve * (θhour - FlowTemperatureActual) / 1000
```

После:

```text
Q_workday = Σhours * WorkDays
Q_saturday = Σhours * Saturdays
Q_sunday = Σhours * Sundays
Q_holiday = 24h при NonProjectTemperature * Holidays
Qve = Q_workday + Q_saturday + Q_sunday + Q_holiday
```

## 5. Cooling gains / losses balance

Метод: `CalculateETA(parameterAc, loses, gainings, section)`.

```text
γ = gainings / loses
if γ > 0 and |γ-1| > 0.01:
    η = (1 - γ^(-Ac)) / (1 - γ^(-(Ac+1)))
if |γ-1| < 0.01:
    η = Ac / (Ac + 1)
if γ < 0:
    η = 1
else:
    η = 0
```

`CalculateAc(...)`:

```text
Ac = 1 + τ / 15
τ = HeatedArea * HeatCapacity / (Htr + Hinf)
```

## 6. Latent loads

Има отделни методи за latent heat:

```text
CalculateLatentHeatsInf...
CalculateLatentHeatsVent...
CalculateQLatentOccupants...
CalcAirX(...)
CalcRoW(...)
CalcRo(...)
CalculateEntalpia(...)
CalculateWitheringEntalpia(...)
```

Това показва, че cooling results може да се различават сериозно, ако не са възстановени влажностите и енталпиите.

## 7. Свободно охлаждане

Има блок:

```text
ClaculateQfreecooling...
GetNightWorkingHours(...)
```

Той трябва да се разгледа отделно при проект с free cooling / нощна вентилация.

## 8. Рискове за сравнение

1. Cooling path работи с много повече графици: cooling, ventilation, occupants.
2. Някои методи са с typo `Claculate...`, важно е да се копира името при търсене.
3. При cooling има latent loads; чисто sensible сравнение няма да съвпада, ако в UI са активни влажности или вентилация.
