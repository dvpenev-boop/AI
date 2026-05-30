# EECalc reverse engineering — обща картина

Този пакет е първи завършен extraction pass върху декомпилираните файлове на `EECalcCore`. Целта е да се получи **алгоритмична база за сравнение** срещу новия C# софтуер.

## Използвани файлове

- `EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs` — основен engine; 689 извлечени метода.
- `WallsTableCalc.cs`, `RoofTableCalc.cs`, `FloorTableCalc.cs`, `TempBridgeCalc.cs` — агрегиране на U/A/топлинни мостове.
- `BaseLineData.cs` — структура на резултатните редове за ref/base/actual/ESM.
- `Izdaki softuer za probata.pdf` — UI screenshots от примерен проект; ползва се само като контекст за mapping.

## Основни изводи

1. Основният monthly heating balance е в `HeatingAndCoolingResultCalc.CalculateActual(...)`.
2. Нетната енергия за отопление е:

```text
Qnd_m = Qht_m - ηgn_m * Qgn_m
Qht_m = Qtr_m + Qve_m
```

3. При случая без U-загуби и без печалби:

```text
Qnd_m = Qve_m
```

4. EECalc не използва директно седмично усредняване за частично отопление, а разделя месечните часове на проектни и непроектни:

```text
Qve_m = Hve * (Δproj_m + Δnonproj_m) / 1000
```

5. Дните се генерират в `InputDataCalc.CalculateMonthlyDays(...)`, с фиксиран календар `2006`. Това е критичен източник на разлики.

## Документи в пакета

- `01_heating_engine.md` — отопление, Qtr/Qve/Qgn/Ni.
- `02_monthly_days_and_schedules.md` — календар, частични месеци, графици.
- `03_transmission_geometry.md` — стени, прозорци, покрив, под, топлинни мостове.
- `04_gains_solar_internal.md` — слънчеви и вътрешни печалби.
- `05_cooling_engine.md` — охлаждане и latent loads.
- `06_ventilation_systems_fans_pumps.md` — вентилационна енергия, вентилатори и помпи.
- `07_hot_water_solar.md` — БГВ и соларни изчисления.
- `08_results_primary_fuels.md` — нетна/потребна/първична енергия и горива.
- `09_risk_register_and_validation_plan.md` — контролни точки за сравнение с твоя софтуер.
- `99_method_index.md` — автоматично извлечен списък на методите.

## Статус

Това е инженерна реконструкция от наличните файлове. Някои типове не са качени (`CalculationData`, `Section`, `CalculationInput`, `MonthlyDays` и др.), затова част от mapping-а към UI/модела е отбелязан като зависимост. Формулите, които са директно видими в качения код, са извадени като deterministic rules.
