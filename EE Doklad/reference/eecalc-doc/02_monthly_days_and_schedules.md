# 02 — MonthlyDays, отоплителен сезон и графици

## 1. Обект MonthlyDays

От декомпилирания код и предишния extraction са идентифицирани полетата:

```csharp
Month Month;
int WorkDays;
int Saturdays;
int Sundays;
int Holydays;
int TotalDays;
double Weeks;
```

Този обект е критичен, защото всички месечни изчисления за частично отопление/охлаждане работят през него.

## 2. Източник на start/end ден

`CalculateMonthlyDays(...)` не избира сам началния и крайния ден. Той ги получава като аргументи:

```csharp
CalculateMonthlyDays(this Section section, List<Month> period, int firstDay, int lastDay)
```

Тези стойности идват от `section.HeatingSeason` или `section.CoolingSeason`, например в качения код има множество извиквания от вида:

```csharp
section.CalcPeriod(
    (int)section.HeatingSeason.FirstMonthHeat,
    (int)section.HeatingSeason.LastMonthHeat,
    section.HeatingSeason.FirstDayHeat,
    section.HeatingSeason.LastDayHeat)
```

Следователно mapping към UI:

```text
FirstMonthHeat / FirstDayHeat -> начален месец/ден отопление
LastMonthHeat  / LastDayHeat  -> краен месец/ден отопление
```

## 3. Фиксиран календар 2006

В `CalculateMonthlyDays(...)` е намерено:

```csharp
DateTime(2006, month, day)
```

Това означава, че EECalc използва фиксирана референтна година 2006 за разпределяне на работни дни/съботи/недели. Не използва реалната календарна година на проекта.

## 4. Логика за пълни месеци

За междинните месеци се брои ден по ден:

```text
for day = 1..DaysInMonth(2006, month):
    if Saturday -> Saturdays++
    else if Sunday -> Sundays++
    else -> WorkDays++
WorkDays = max(WorkDays - Holidays, 0)
Weeks = (DaysInMonth - Holidays) / 7.0
```

## 5. Логика за първи частичен месец

Кодът има shortcut правила:

```text
if firstDay > 21:
    Saturdays = 0
    Sundays = 0
    WorkDays = daysRemaining - holidays
elif firstDay > 14:
    Saturdays = 1
    Sundays = 1
    WorkDays = daysRemaining - 2 - holidays
elif firstDay > 7:
    Saturdays = 2
    Sundays = 2
    WorkDays = daysRemaining - 4 - holidays
else:
    брои ден по ден от firstDay до края на месеца
```

където:

```text
daysRemaining = DaysInMonth - firstDay + 1
```

## 6. Логика за последен частичен месец

```text
if lastDay < 7:
    Saturdays = 0
    Sundays = 0
    WorkDays = lastDay - holidays
elif lastDay < 14:
    Saturdays = 1
    Sundays = 1
    WorkDays = lastDay - 2 - holidays
elif lastDay < 21:
    Saturdays = 2
    Sundays = 2
    WorkDays = lastDay - 4 - holidays
else:
    брои ден по ден от 1 до lastDay
```

## 7. График за отопление

За Current/Actual отопление кодът използва:

```text
section.HeatingSeasons.Heating.WorkCurrentStart / End
section.HeatingSeasons.Heating.SatCurrentStart  / End
section.HeatingSeasons.Heating.SunCurrentStart  / End
```

Часовете се смятат като:

```text
hW   = WorkEnd - WorkStart
hSat = SatEnd  - SatStart
hSun = SunEnd  - SunStart
```

В `InputDataCalc.CalcHours(...)` има обработка и за периоди през полунощ:

```text
if endHour >= startHour:
    h = endHour - startHour
else:
    h = 24 - startHour + endHour
```

В heating formulas, които видяхме, директно се използва `End - Start`. За сигурност при твоя reimplementation трябва да използваш общата `CalcHours` логика, за да няма разлики при графици през полунощ.

## 8. Проектни и непроектни часове

```text
Hproj = WorkDays*hW + Saturdays*hSat + Sundays*hSun
Hnonproj = WorkDays*(24-hW) + Saturdays*(24-hSat) + Sundays*(24-hSun) + Holydays*24
```

Това разделяне е основата на EECalc periodic heating/cooling logic.
