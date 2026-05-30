# 02 - Engine за месечни дни

Източник: `InputDataCalc.cs`.

## Избор на период

`CalcPeriod(this Section section, int firstMonth, int lastMonth, int firstDay, int lastDay)` (`InputDataCalc.cs:13`) изгражда списък от `Month`:

- ако `firstMonth == lastMonth`, добавя само този месец;
- ако `firstMonth < lastMonth`, добавя последователно от първия до последния месец;
- ако периодът пресича края на годината, добавя от `firstMonth` до декември и после от януари до `lastMonth`.

След това извиква:

```text
section.CalculateMonthlyDays(period, firstDay, lastDay)
```

Източник: `InputDataCalc.cs:29`, `InputDataCalc.cs:40`.

## Часове през полунощ

`CalcHours(...)` (`InputDataCalc.cs:43`) е общият helper за часови интервали:

```text
if endHour >= startHour:
    hours = endHour - startHour
else:
    hours = 24 - startHour + endHour
```

Важно: част от отоплителните формули не използват `CalcHours`, а директно `End - Start`. Това е поведение от C# и трябва да се възпроизведе, ако целта е byte-for-byte съвместимост.

## Календар

`CalculateMonthlyDays(...)` (`InputDataCalc.cs:48`) използва фиксирана година 2006:

```text
daysInMonth = DateTime.DaysInMonth(2006, month + 1)
date = new DateTime(2006, month + 1, day)
```

Източник: `InputDataCalc.cs:53`, `InputDataCalc.cs:69`, `146`, `224`, `252`.

Празниците идват от `section.Holidays` чрез `GetHollydays(...)` (`InputDataCalc.cs:281`).

## Едномесечен период

Ако периодът съдържа само един месец, методът брои ден по ден от `firstDay` до `lastDay`:

- събота -> `Saturdays++`;
- неделя -> `Sundays++`;
- друго -> `WorkDays++`;
- после `WorkDays = max(WorkDays - hollydays, 0)`.

Източник: `InputDataCalc.cs:57-84`.

`Weeks` при едномесечен период е integer деление:

```text
Weeks = (daysInMonth - hollydays) / 7
```

Източник: `InputDataCalc.cs:59-65`.

## Първи частичен месец

За първия месец в многомесечен период `Weeks` идва от `GetWeeksInMonth(...)`:

```text
Weeks = max(daysInMonth - firstDay + 1 - hollydays, 0) / 7
```

Източник: `InputDataCalc.cs:87-90`, `272-277`.

Shortcut правила:

- `firstDay > 21`: `Saturdays = 0`, `Sundays = 0`, `WorkDays = daysRemaining - holidays`.
- `firstDay > 14`: `Saturdays = 1`, `Sundays = 1`, `WorkDays = daysRemaining - 2 - holidays`.
- `firstDay > 7`: `Saturdays = 2`, `Sundays = 2`, `WorkDays = daysRemaining - 4 - holidays`.
- иначе брои ден по ден от `firstDay` до края на месеца.

Източник: `InputDataCalc.cs:92-160`.

## Последен частичен месец

Ако `lastDay > daysInMonth`, той се реже до последния ден (`InputDataCalc.cs:166-169`).

`Weeks`:

```text
Weeks = max(lastDay - hollydays, 0) / 7
```

Източник: `InputDataCalc.cs:170`, `278`.

Shortcut правила:

- `lastDay < 7`: `Saturdays = 0`, `Sundays = 0`, `WorkDays = lastDay - holidays`.
- `lastDay < 14`: `Saturdays = 1`, `Sundays = 1`, `WorkDays = lastDay - 2 - holidays`.
- `lastDay < 21`: `Saturdays = 2`, `Sundays = 2`, `WorkDays = lastDay - 4 - holidays`.
- иначе брои ден по ден от 1 до `lastDay`.

Източник: `InputDataCalc.cs:171-239`.

## Пълни междинни месеци

За месеци, които не са първи или последен:

```text
Weeks = (daysInMonth - hollydays) / 7.0
WorkDays/Saturdays/Sundays се броят ден по ден
WorkDays = max(WorkDays - hollydays, 0)
```

Източник: `InputDataCalc.cs:242-267`.
