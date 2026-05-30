# 06 — Ventilation systems, fans and pumps

## 1. Heating ventilation energy

В кода има отделен системен блок:

```text
VentilationHeatEnergyRef1/Ref2/Actual/BaseLine/ESM
CalculateMontlyHeatEnergy...
CalculateAverageVentHeatTemp...
CalculateVentNeededEnergy...
```

Това е различно от `Qve` на building heat balance. `Qve` е топлинна загуба/печалба в зоната, докато `VentilationHeatEnergy...` е потребна енергия на вентилационна/отоплителна система.

## 2. Week hours за heating ventilation

Методите:

```text
GetWeekHoursReferences
GetWeekHoursActual
GetWeekHoursBaseLine
GetWeekHoursESM
```

и helper-и от вида:

```text
GetWeekHeatingVentilationHoursActual(section)
GetWeekHeatingSeasonHoursBaseLine(section)
```

използват седмични часове за системи, различни от месечните `MonthlyDays` energy balance.

## 3. Fans and pumps — отопление

Метод: `CalculateFansAndPumpsHeatingActual(...)`.

Логика:

```text
months = section.CalcPeriod(HeatingSeason...)
weeks = Σ month.Weeks
ventHoursWeek = GetWeekHeatingVentilationHoursActual(section)

PumpNeededEnergyActual =
    VentilatorsHeatActual * ventHoursWeek * weeks / 1000
  + PumpVentilationActual * ventHoursWeek * weeks / 1000
  + PumpHeatingActual * 24*7 * weeks / 1000

PumpNeededEnergyActual = PumpNeededEnergyActual / EnergyManagementActual * 100
```

За Ref/Base/ESM се сменят съответните полета.

## 4. Fans and pumps — охлаждане

Аналогично:

```text
CalculateFansAndPumpsCooling...
```

Използва cooling periods и cooling ventilation hours.

## 5. Vent needed energy

Методи:

```text
CalculateVentNeededEnergyRef1/Ref2/Actual/BaseLine/Esm
CalculateVentCoolNeededEnergy...
```

Тези методи акумулират резултата от monthly ventilation system calculations към резултатни полета.

## 6. Контролни точки

За сравнение с твоя софтуер трябва да се логват:

```text
weeks = Σ MonthlyDays.Weeks
weekHeatingVentilationHours
weekHeatingSeasonHours
VentilatorsHeat power
PumpVentilation power
PumpHeating power
EnergyManagement
PumpNeededEnergy
```

Честа причина за разлика: `weeks` е `(days - holidays)/7`, не непременно цяло число.
