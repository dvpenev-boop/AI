# План за валидационен harness: EE.Doklad срещу EECalc

Цел: да се изгради контролируем validation harness, който сравнява междинни и крайни стойности от проекта `EE.Doklad` срещу очаквани стойности от EECalc, без промени в production code. Source of truth са `reference/eecalc-decompiled`, `reference/eecalc-doc` и вече генерираните `analysis/docs`.

## 0. Принципи

- Не се променя production code. Ако са нужни диагностични точки, те да са през test-only adapters, conditional test hooks, reflection, wrapper-и или отделен debug exporter.
- Сравняването да е на стъпки, не само по крайна енергия. Първо календар, после `Qve`, `Qtr/Htr`, `Qgn`, `Gamma/Ni`, `NetEnergy`, `NeededEnergy`, `Building aggregation`.
- Всяка очаквана стойност трябва да носи provenance: EECalc метод, source ред и име на входен fixture.
- Да има толеранси по ниво: календарът е exact, междинните double стойности с малък абсолютен/относителен tolerance, крайни таблици с tolerance според форматиране.

## 1. EECalc методи за първа валидация

Редът е по риск и blast radius.

### 1. MonthlyDays / CalcPeriod

Първо се валидират:

- `InputDataCalc.CalcPeriod(...)`: избира месеците и извиква `CalculateMonthlyDays`; анализът го цитира като source `reference/eecalc-decompiled/EECalcCore.Calculations.InputDataCalc.cs:13` и вътрешно извикване към `InputDataCalc.CalculateMonthlyDays` (`analysis/docs/02_method_index.md:11465-11473`).
- `InputDataCalc.CalculateMonthlyDays(...)`: генерира `WorkDays`, `Saturdays`, `Sundays`, `Holydays`, `TotalDays`, `Weeks`; source `InputDataCalc.cs:48` (`analysis/docs/02_method_index.md:11490-11498`).
- `InputDataCalc.CalcHours(...)`: обработва интервали през полунощ чрез `(endHour >= startHour) ? end-start : 24-start+end`; formula catalog source `InputDataCalc.cs:43-44` (`analysis/docs/03_formula_catalog.md:7505-7511`).

Причина: всички месечни енергии използват `MonthlyDays`. Грешка тук се умножава във всеки следващ engine.

### 2. Qve

След календар:

- `HeatingAndCoolingResultCalc.CalculateParameterQve(...)`: `Qve = Hve * (CalcAvgProjectTemp + CalcAvgNonProjectTemp) / 1000`; source `HeatingAndCoolingResultCalc.cs:3663`, формула на `3664` (`analysis/docs/02_method_index.md:3140-3148`, `analysis/docs/03_formula_catalog.md:2247-2253`).
- `HeatingAndCoolingResultCalc.CalcParameterHve(...)`: `Hve = HeatedVolume * InfiltracionActual * 0.34`; source `HeatingAndCoolingResultCalc.cs:3687`, output `section.Test.ParameterHve` (`analysis/docs/02_method_index.md:3186-3194`).
- `CalcAvgProjectTemp(...)` и `CalcAvgNonProjectTemp(...)`, защото са shared degree-hour компоненти за `Qve` и `Qtr`.

Причина: `Qve` е най-малко зависимият енергиен клон след календара и изолира проблеми в обем, инфилтрация, график и температури.

### 3. Qtr / Htr

После:

- `HeatingAndCoolingResultCalc.CalculateParameterQtr(...)`: връща `section.Test.ParameterHtr * (CalcAvgProjectTemp + CalcAvgNonProjectTemp) / 1000`; source `HeatingAndCoolingResultCalc.cs:3693`, формула на `3698` (`analysis/docs/02_method_index.md:3199-3207`, `analysis/docs/03_formula_catalog.md:2286-2292`).
- `HeatingAndCoolingResultCalc.CalculateParameterHtr(...)`: `Htr = Hd + Hg + Hu`; source `HeatingAndCoolingResultCalc.cs:3702`, outputs `ParameterHd`, `ParameterHg`, `ParameterHu` (`analysis/docs/02_method_index.md:3212-3220`).
- `CalculateParameterHdCurrent(...)`, `CalculateParameterHgCurrent(...)`, `SumWallDirecrionsHu1(...)`, `CalcCeilingsParameterHu2(...)`, `CalcFloorsParameterHu3(...)`.

Причина: `Htr` събира геометрия, U стойности, мостове и некондиционирани зони. Тук рискът от mapping грешки е висок.

### 4. Qgn

След envelope:

- `HeatingAndCoolingResultCalc.CalculateParameterQgn(...)`: source `HeatingAndCoolingResultCalc.cs:3941`, outputs project/non-project часове и извиква `CalculateNonTrasparentFsol`, `CalculateTrasparentFsol` (`analysis/docs/02_method_index.md:3463-3471`).
- Формулните редове за часовете са на `HeatingAndCoolingResultCalc.cs:3942-3948` (`analysis/docs/03_formula_catalog.md:2483-2495`).
- `CalculateTransparentFsol(...)`, `CalculateTrasparentFsol(...)`, `CalculateNonTransparentFsol(...)`, `CalculateNonTrasparentFsol(...)` за соларните компоненти.

Причина: `Qgn` комбинира графици, климатична радиация, ориентации, стъклопакети, непрозрачни елементи и roof horizontal logic.

### 5. Gamma / Ni

После:

- `HeatingAndCoolingResultCalc.CalculateActual(...)`: задава `ParameterQht`, `ParameterQgn`, `ParameterGama`, `ParameterNi`, `NetEnergyQnd`; source `HeatingAndCoolingResultCalc.cs:3483`, outputs в анализа (`analysis/docs/02_method_index.md:3040-3048`).
- Формули:
  - `Qht = Qtr + Qve`;
  - `Qgn = CalculateParameterQgn(...) / 1000`;
  - `Gamma = (Qgn + latentHeatPerMonth * HeatedArea) / Qht`;
  - `NetEnergyQnd = Qht - Ni * Qgn`;
  цитирани от `HeatingAndCoolingResultCalc.cs:3486-3490` (`analysis/docs/03_formula_catalog.md:2150-2159`).
- `HeatingAndCoolingResultCalc.CalculateParameterNign(...)`: source `HeatingAndCoolingResultCalc.cs:3632`, формули за `Ni` на `3636` и `3644` (`analysis/docs/02_method_index.md:3111-3119`, `analysis/docs/03_formula_catalog.md:2228-2235`).

Причина: `Gamma/Ni` са чувствителни към деление на малки стойности, edge cases около `gamma ~= 1`, и латентна топлина.

### 6. NetEnergy

Проверява се след валидирани `Qtr/Qve/Qgn/Ni`:

- `CalculateActual(...)` за месечния raw `NetEnergyQnd`;
- `HeatingAndCoolingResultCalc.Calculations(...)`: source `HeatingAndCoolingResultCalc.cs:3243`, извиква `CalculateActual`, `CalculateBaseLine`, `CalculateEsm`, `CalculateRef1/Ref2`, `CheckForNaN`, `GetLightsAndDevicesInputs`, `CalcPeriod` (`analysis/docs/01_call_graph.md:1112-1115`).
- Крайната нормализация по площ и латентна корекция трябва да се сравни отделно, защото `CalculateActual` първо пази raw kWh, а `Calculations` по-късно прави area result.

### 7. NeededEnergy

След net energy:

- `HeatingAndCoolingResultCalc.CalculateVentNeededEnergyActual(...)`: source `HeatingAndCoolingResultCalc.cs:15903`, outputs към `ResultSourceEnergyActual`, `ResultSourceEnergy2Actual`, `ResultNeededEnergyActual` (`analysis/docs/02_method_index.md:11086-11094`).
- Формули:
  - `ResultNeededEnergyActual = ResultSourceEnergyActual + ResultSourceEnergy2Actual`;
  - `num = ResultEnergyForHeatingActual * Part1Actual / 100`;
  - `num2 = ResultEnergyForHeatingActual * Part2Actual / 100`;
  - деление през `TransmitTempEfficiency`, `SupplyNetEfficiency`, `Automatic`, `EnergyManagement`, `GeneratorHeatEfficiency`;
  цитирани от `HeatingAndCoolingResultCalc.cs:15906-15926` (`analysis/docs/03_formula_catalog.md:7245-7257`).

Причина: тук често има разлики от проценти, деление на 100, втори генератор и NaN/Infinity fallback.

### 8. Building aggregation

Последно:

- `HeatingAndCoolingResultCalc.BuildingCalculations(...)`: source `HeatingAndCoolingResultCalc.cs:8524`, inputs `Results buildingBalanceResult`, `CalculationInput calcInput`, `Results zoneBalanceResult`; анализът показва извиквания към building rollups, primary energy, total fuel, scale и CO2 (`analysis/docs/02_method_index.md:7035-7043`).
- `ZoneCalculations(...)`, `BuildingCO2Calculations(...)`, `ZoneCO2Calculations(...)`, `CO2EnergyZoneCalculations(...)`, `SetScaleType(...)`.

Причина: агрегира много вече изчислени таблици; грешките тук са най-трудни за локализиране, ако предходните нива не са заключени.

## 2. Междинни стойности за логване

Логването трябва да е структурирано и test-only. Препоръчителен формат: `CSV` за таблична инспекция и `JSONL` за snapshot сравнение.

### За MonthlyDays / CalcPeriod

Лог за всеки период:

- `fixture_id`
- `firstMonth`, `lastMonth`, `firstDay`, `lastDay`
- `month`
- `TotalDays`
- `WorkDays`
- `Saturdays`
- `Sundays`
- `Holydays`
- `Weeks`
- `CalcHours` cases за work/sat/sun графици

Очакван EECalc provenance: `InputDataCalc.CalcPeriod` и `CalculateMonthlyDays` (`analysis/docs/02_method_index.md:11465-11498`).

### За Qve

Лог за всеки месец и scenario `Actual/BaseLine/ESM/Ref1/Ref2`:

- `HeatedVolume`
- `Infiltration`
- `Hve`
- `ProjectTemperature`
- `NonProjectTemperature`
- `AvgOutdoorTemp`
- `projectHours`
- `nonProjectHours`
- `DeltaProject`
- `DeltaNonProject`
- `Qve`

Очакван EECalc provenance: `CalculateParameterQve`, `CalcParameterHve`, `CalcAvgProjectTemp`, `CalcAvgNonProjectTemp` (`analysis/docs/03_formula_catalog.md:2247-2253`).

### За Qtr / Htr

Лог:

- `AvgOutdoorTemp`
- `AverageInnerHeatTemp`
- `Hd`
- `Hg`
- `Hu`
- `Htr`
- `Qtr`
- за `Hd`: walls, windows, non-transparent roof, transparent roof
- за `Hg`: floor area, floor U
- за `Hu`: walls Hu, ceiling Hu, other floors Hu
- за thermal bridges: `SumL`, `SumX` или техните еквиваленти в `EE.Doklad`

Очакван EECalc provenance: `CalculateParameterQtr` (`analysis/docs/03_formula_catalog.md:2286-2292`) и `CalculateParameterHtr` (`analysis/docs/02_method_index.md:3212-3220`).

### За Qgn

Лог:

- `projectHours`, `nonProjectHours`, `totalHours`
- `FsolTransparentWallsByDirection`
- `FsolTransparentRoofByPosition`
- `FsolNonTransparentWallsByDirection`
- `FsolNonTransparentRoof`
- `SolarRadiation.N/E/S/W/H`
- `Qgn_raw`
- `Qgn_kWh = Qgn_raw / 1000`

Очакван EECalc provenance: `CalculateParameterQgn` и редове `3942-3948` (`analysis/docs/03_formula_catalog.md:2483-2495`).

### За Gamma / Ni

Лог:

- `Qtr`
- `Qve`
- `Qht`
- `Qgn`
- `latentHeatPerMonth`
- `HeatedArea`
- `gamma`
- `aH`
- `tau`
- `Ni`
- branch: `gamma > 0 && abs(gamma-1)>0.01`, `gamma < 0`, `abs(gamma-1)<0.01`, fallback

Очакван EECalc provenance: `CalculateActual` (`analysis/docs/03_formula_catalog.md:2150-2159`) и `CalculateParameterNign` (`analysis/docs/03_formula_catalog.md:2228-2235`).

### За NetEnergy

Лог:

- `NetEnergyQnd_raw = Qht - Ni * Qgn`
- `NetEnergyQnd_perArea_beforeLatent`
- `latentCorrection = Ni * latentHeatPerMonth`
- `ResulNoInputsNetEnergyActual`
- `ResulNoInputsNetEnergyBaseLine`
- `ResulNoInputsNetEnergyESM`
- `CheckForNaN` result

### За NeededEnergy

Лог:

- `ResultEnergyForHeating*`
- `Part1*`, `Part2*`
- всички efficiency множители за генератор 1 и 2
- `ResultSourceEnergy*`
- `ResultSourceEnergy2*`
- `ResultNeededEnergy*`
- `IsNaN/IsInfinity fallback applied`

Очакван EECalc provenance: `CalculateVentNeededEnergyActual` (`analysis/docs/03_formula_catalog.md:7245-7257`).

### За Building aggregation

Лог:

- zone id / building id
- `ConditionedArea`, `HeatedArea`
- zone-level net/needed/source/primary/fuel tables
- building accumulated net/needed/source/primary/fuel tables
- `Fuel1/Fuel2`, VEI values, CO2 values
- scale classification inputs and result

Очакван EECalc provenance: `BuildingCalculations` (`analysis/docs/02_method_index.md:7035-7043`).

## 3. Unit tests за създаване

### MonthlyDays / CalcPeriod tests

- Full month без празници: очаквани `WorkDays/Saturdays/Sundays/Weeks`.
- Single-month partial period: `firstDay..lastDay` в един месец.
- First partial month branches: `firstDay > 21`, `>14`, `>7`, `<=7`.
- Last partial month branches: `lastDay < 7`, `<14`, `<21`, `>=21`.
- Cross-year period: например ноември-март.
- Holidays greater than work days: `WorkDays` трябва да стане `0`, не отрицателно.
- `CalcHours` през полунощ: `22 -> 6 = 8`; normal: `8 -> 17 = 9`.

### Qve tests

- Minimal fixture с `Htr=0`, `Qgn=0`, само инфилтрация: очакване `Qnd == Qve`.
- Actual/BaseLine/ESM variants с различна инфилтрация.
- Нулев heated volume.
- Графици с `Work/Sat/Sun` различни часове.
- Проверка, че heating `CalcAvgProjectTemp` използва директно `End - Start`, не `CalcHours`, ако EECalc source го прави така.

### Qtr / Htr tests

- Само външни стени: `Hd = sum(A*U + SumL + SumX)`.
- Само прозорци: `Hd_windows = AccumulateWindowU * AccumulateWindowA`.
- Само под: `Hg = AccumulateFloorA * AccumulateFloorU`.
- Само вътрешна зона: `Hu_i = A*U*(avgInner-W)/(avgInner-avgOutdoor)`.
- Комбиниран fixture: `Htr = Hd + Hg + Hu`.
- Regression test за известните декомпилационни особености, ако `EE.Doklad` цели съвместимост с EECalc: например `SumWallDirecrionsHu1` поведение да се сравни внимателно срещу EECalc output.

### Qgn tests

- Нулеви площи: `Qgn=0` или само радиационна загуба според компонентите.
- Само прозорец на юг.
- Само хоризонтален roof transparent.
- Само непрозрачен roof.
- Проверка на diagonal directions: `NE=(N+E)/2`, `SE=(S+E)/2`, `SW=(S+W)/2`, `NW=(N+W)/2`.
- Проверка на делението `/1000` в `CalculateActual`, а не вътре в `CalculateParameterQgn`.

### Gamma / Ni tests

- `gamma > 0` и далеч от `1`: `(1 - gamma^aH)/(1 - gamma^(aH+1))`.
- `gamma < 0`: `Ni=1`.
- `abs(gamma - 1) < 0.01`: `Ni=aH/(aH+1)`.
- fallback branch: очакване `0`.
- `Qht` близо до нула: очаквано поведение с NaN/Infinity downstream.

### NetEnergy tests

- `Qht`, `Qgn`, `Ni` фиксирани чрез fixture или test double: `NetEnergyQnd_raw = Qht - Ni*Qgn`.
- Проверка на per-area transform и латентна корекция.
- Проверка Actual/BaseLine/ESM списъчно агрегиране.
- Проверка `CheckForNaN` behavior на крайни резултати.

### NeededEnergy tests

- Един генератор `Part1=100`, `Part2=0`.
- Два генератора `Part1+Part2=100`.
- Ефективности 100%: source energy == split energy.
- Ефективности различни от 100%.
- NaN/Infinity fallback при нулева ефективност.
- Branch `SecondRecEfficiency > 100` срещу normal branch.

### Building aggregation tests

- Една зона: building резултатът трябва да е равен на zone резултата.
- Две зони с различна площ: проверка на area weighted/per-area резултати.
- Зони с различни горива: fuel table accumulation.
- Primary energy и CO2 rollup.
- Scale classification boundaries.

## 4. Snapshot/debug таблици

Да се генерират следните test artifacts за всеки fixture:

### `debug_monthly_days.csv`

Колони:

```text
fixture, scenario, month, total_days, work_days, saturdays, sundays, holidays, weeks
```

### `debug_heating_monthly_balance.csv`

Колони:

```text
fixture, scenario, month,
avg_outdoor_temp, project_temp, non_project_temp,
project_hours, non_project_hours,
hve, qve,
hd, hg, hu, htr, qtr,
fsol_transparent, fsol_nontransparent, qgn_raw, qgn,
latent_heat_per_month, gamma, ah, ni,
qht, net_energy_raw, net_energy_per_area
```

### `debug_transmission_breakdown.csv`

Колони:

```text
fixture, scenario, month,
walls_outer, windows, roof_nontransparent, roof_transparent,
floor_ground, hu_walls, hu_ceiling, hu_floor_other,
hd, hg, hu, htr
```

### `debug_solar_gains_breakdown.csv`

Колони:

```text
fixture, scenario, month,
direction, element_type, area, u, g, epsilon, alfa,
radiation, horizontal_flag, fsol
```

### `debug_needed_energy.csv`

Колони:

```text
fixture, scenario, subsystem,
result_energy, part1, part2,
eta_transmit1, eta_supply1, eta_auto1, eta_management1, eta_generator1,
eta_transmit2, eta_supply2, eta_auto2, eta_management2, eta_generator2,
source1, source2, needed, fallback
```

### `debug_building_aggregation.csv`

Колони:

```text
fixture, building, zone,
conditioned_area, heated_area,
net_energy, no_inputs_net_energy, needed_energy, source_energy,
primary_energy, fuel1, fuel2, co2, scale
```

### `snapshot_expected_eecalc.json`

Един canonical JSON snapshot:

```json
{
  "fixture": "name",
  "source": "EECalc",
  "methods": {
    "monthlyDays": "InputDataCalc.CalculateMonthlyDays:48",
    "qve": "HeatingAndCoolingResultCalc.CalculateParameterQve:3663",
    "qtr": "HeatingAndCoolingResultCalc.CalculateParameterQtr:3693",
    "qgn": "HeatingAndCoolingResultCalc.CalculateParameterQgn:3941",
    "ni": "HeatingAndCoolingResultCalc.CalculateParameterNign:3632"
  },
  "months": []
}
```

## 5. Сравнение EECalc expected срещу EE.Doklad actual

### Fixture protocol

1. Дефинирай fixture в неутрален формат: `validation/fixtures/*.json`.
2. Направи loader към `EE.Doklad` test model.
3. Направи expected snapshot от EECalc анализа. Ако няма runnable EECalc DLL, expected стойностите да се попълнят от декомпилираните формули в отделен test oracle, маркиран като `EecalcOracle`.
4. Изпълни `EE.Doklad` calculation върху същия fixture.
5. Експортирай actual debug tables.
6. Сравни expected vs actual на всяко ниво от pipeline-а.

### Comparison keys

Минимален ключ:

```text
fixture_id + scenario + month + metric_name
```

За breakdown таблици:

```text
fixture_id + scenario + month + component + direction + element_index
```

### Tolerances

- Calendar fields: exact equality.
- Hours: exact equality, освен ако `EE.Doklad` intentionally нормализира през `CalcHours`; тогава mismatch да се маркира като behavioral difference.
- `Hve`, `Htr`, `Qve`, `Qtr`, `Qgn`, `gamma`, `Ni`: `abs <= 1e-6` или `rel <= 1e-6`.
- Monthly kWh: `abs <= 1e-4`.
- Per-area kWh/m2: `abs <= 1e-4`.
- UI-rounded values: отделен comparison mode с `F2`/`F3` според production formatting.

### Diff format

Всеки mismatch да показва:

```text
fixture=scenario/month/metric
expected_value
actual_value
abs_diff
rel_diff
eecalc_method
eecalc_source_line
upstream_dependencies
```

Пример:

```text
fixture=zone9_minimal, scenario=Actual, month=January, metric=Qve
expected=123.456789
actual=123.450001
abs_diff=0.006788
source=HeatingAndCoolingResultCalc.CalculateParameterQve:3663
dependencies=MonthlyDays, Hve, CalcAvgProjectTemp, CalcAvgNonProjectTemp
```

## 6. Задачи по ред на риск

### R1. MonthlyDays / CalcPeriod

Deliverables:

- Unit tests за всички branch-ове на `CalculateMonthlyDays`.
- Snapshot `debug_monthly_days.csv`.
- Проверка на fixed year `2006`.
- Проверка на holidays subtraction.

Exit criteria:

- 100% exact match за `Month`, `WorkDays`, `Saturdays`, `Sundays`, `Holydays`, `TotalDays`, `Weeks`.

### R2. Qve

Deliverables:

- Unit tests за `Hve`, degree-hour components и `Qve`.
- `debug_heating_monthly_balance.csv` с Qve columns.
- Separate comparison за Actual/BaseLine/ESM/Ref.

Exit criteria:

- `Hve`, `DeltaProject`, `DeltaNonProject`, `Qve` match в tolerance.

### R3. Qtr / Htr

Deliverables:

- Component tests за `Hd`, `Hg`, `Hu`.
- Snapshot `debug_transmission_breakdown.csv`.
- Fixture-и за walls/windows/roof/floor/thermal bridges.

Exit criteria:

- `Hd/Hg/Hu/Htr/Qtr` match; ако има intentional correction спрямо декомпилиран bug, тя да е документирана като known divergence.

### R4. Qgn

Deliverables:

- Directional solar fixture-и.
- Snapshot `debug_solar_gains_breakdown.csv`.
- Tests за transparent/non-transparent и horizontal factor.

Exit criteria:

- `Fsol` по компонент и `Qgn_raw/Qgn` match.

### R5. Gamma / Ni

Deliverables:

- Branch tests за `CalculateParameterNign`.
- Monthly balance fixture-и с контролирани `gamma`.
- Log на `aH`, `tau`, branch id.

Exit criteria:

- `gamma`, `aH`, `Ni` match; edge cases около `gamma ~= 1` са explicit.

### R6. NetEnergy

Deliverables:

- Tests за `Qht`, raw `NetEnergyQnd`, per-area result и latent correction.
- Actual/BaseLine/ESM aggregate snapshots.

Exit criteria:

- Месечен и сезонен net energy match.

### R7. NeededEnergy

Deliverables:

- Tests за generator split и efficiencies.
- Snapshot `debug_needed_energy.csv`.
- NaN/Infinity fallback tests.

Exit criteria:

- `ResultSourceEnergy1/2` и `ResultNeededEnergy` match.

### R8. Building aggregation

Deliverables:

- One-zone identity test.
- Multi-zone weighted aggregation test.
- Fuel/primary/CO2/scale snapshots.
- `debug_building_aggregation.csv`.

Exit criteria:

- Building totals equal sum/weighted rules from EECalc; per-area values and fuel totals match.

## 7. Предложена структура на тестовете

```text
tests/
  validation/
    fixtures/
      001_monthly_days_full_month.json
      002_monthly_days_cross_year.json
      010_qve_minimal.json
      020_qtr_walls_only.json
      030_qgn_south_window.json
      040_gamma_ni_edges.json
      050_net_energy_zone.json
      060_needed_energy_two_generators.json
      070_building_two_zones.json
    expected/
      eecalc_expected_*.json
    snapshots/
      debug_monthly_days.csv
      debug_heating_monthly_balance.csv
      debug_transmission_breakdown.csv
      debug_solar_gains_breakdown.csv
      debug_needed_energy.csv
      debug_building_aggregation.csv
```

Test naming:

```text
EecalcParity_MonthlyDays_*
EecalcParity_Qve_*
EecalcParity_QtrHtr_*
EecalcParity_Qgn_*
EecalcParity_GammaNi_*
EecalcParity_NetEnergy_*
EecalcParity_NeededEnergy_*
EecalcParity_BuildingAggregation_*
```

## 8. Definition of done

- Всички R1-R8 fixtures имат expected EECalc snapshot.
- Всеки mismatch сочи най-близката upstream междинна стойност, не само крайния резултат.
- Има machine-readable diff output за CI.
- Има human-readable CSV таблици за инженерна проверка.
- Всички known divergences са отделени в `analysis/validation_known_differences.md`, ако такива се появят.
