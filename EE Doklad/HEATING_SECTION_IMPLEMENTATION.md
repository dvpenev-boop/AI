# Имплементация на Секция 10: Отопление

## Обзор

Реализирана е нова секция "10. Отопление" в EE Доклад приложението с пълна поддръжка на:
- Ръчни входове за параметри на отоплението
- Автоматично изчисление на топлина от обитатели
- **Линейна интерполация** по температура (без закръгляване)
- Реактивно преизчисление при промяна на входовете
- Валидация на входните данни

## Ключови Особености

### 1. Линейна Интерполация (БЕЗ Закръгляване)

Стойностите за явна (sensible) и скрита (latent) топлина се изчисляват чрез **линейна интерполация** между най-близките две температурни колони в таблицата:

```
alpha = (T - T_lower) / (T_upper - T_lower)
sensible(T) = sensible(T_lower) + alpha * (sensible(T_upper) - sensible(T_lower))
latent(T) = latent(T_lower) + alpha * (latent(T_upper) - latent(T_lower))
```

**Пример:** При T=21°C и активност "Кино, театър, училище":
- T=20°C: sensible=79W, latent=21W
- T=22°C: sensible=72W, latent=28W
- alpha = 0.5
- **sensible(21°C) = 75.50W** (НЕ 79W или 72W)
- **latent(21°C) = 24.50W** (НЕ 21W или 28W)

### 2. Clamping при Крайни Стойности

- **Под минимума:** Ако T < 20°C → използва се стойността при 20°C
- **Над максимума:** Ако T > 28°C → използва се стойността при 28°C

### 3. Числови Входове с Локализация

Полетата за десетични стойности приемат:
- И **запетая** (`,`) и **точка** (`.`) като десетичен разделител
- Автоматично нормализиране към invariant format (с точка)
- Форматиране до **2 знака след десетичната запетая**
- Валидация в реално време с error messages

**Пример въвеждане:**
- `21` → нормализира се до `21.00`
- `21.5` → приема се като `21.50`
- `21,50` → нормализира се до `21.50`

### 4. Реактивни Изчисления

Автоматично преизчисление при промяна на:
- **Проектна температура** → преизчислява sensible/latent per person
- **Степен на активност** → преизчислява sensible/latent per person
- **Брой обитатели** (от Секция 5) → преизчислява общата топлина

## Създадени Файлове

### Models
- `EE.Doklad/Models/HeatingModels.cs` - Таблица с активности и служба за интерполация
- `EE.Doklad/Models/HeatingSectionData.cs` - Модел на данните за секция 10

### ViewModels
- `EE.Doklad/ViewModels/HeatingSectionViewModel.cs` - Бизнес логика и валидация

### Views
- `EE.Doklad/Views/HeatingSectionView.xaml` - UI дизайн
- `EE.Doklad/Views/HeatingSectionView.xaml.cs` - Code-behind

### Тестове
- `InterpolationTest/` - Конзолен проект с 8 unit теста за валидиране на интерполацията

## Модификирани Файлове

- `EE.Doklad/Models/Section.cs` - Добавен `SectionType.Heating` и `HeatingSectionData` свойство
- `EE.Doklad/ViewModels/MainViewModel.cs` - Автоматично създаване на секция 10
- `EE.Doklad/MainWindow.xaml.cs` - Routing към HeatingSectionView

## Таблица с Активности

Реализирани са **10 типа активности** с данни за 6 температурни колони (20, 22, 24, 26, 27, 28°C):

1. **Cinema** - Кино, театър, училище (100 W/m²)
2. **Office** - Работа на компютър (120 W/m²)
3. **HotelReceptionKasier** - Офисна работа, столова, магазини (130 W/m²)
4. **StandingLightWork** - Стоящ, правомагазин, ходещ, баня (130 W/m²)
5. **WalkingSeated** - Ходещ, седнал (150 W/m²)
6. **ModerateWork** - Средна работа, слуга, фризьор (160 W/m²)
7. **LightWorkSeated** - Лека работа седнал, механична продукция (220 W/m²)
8. **Dancing** - Танцуване, лека партийна работа (250 W/m²)
9. **FastWalking** - Бързо ходене, планинско ходене (300 W/m²)
10. **HeavyWork** - Тежка работа, атлети, спортуване (430 W/m²)

## Ръчни Входове

### Параметри на Отоплението
- **Инфилтрация [1/ч]** - decimal, ≥0, 2 знака
- **Проектна температура [°C]** - decimal, 2 знака (влияе на интерполацията)
- **Температура на понижение [°C]** - decimal, 2 знака
- **Ефективност на отдаване [%]** - 0-100
- **Ефективност на разпределителна мрежа [%]** - 0-100
- **Автоматично управление [%]** - 0-100
- **Енергиен мениджмънт [%]** - 0-100
- **КПД на топлоснабдяване [%]** - 0-100

### Обитатели
- **Брой обитатели** (read-only) - взима се от Секция 5
- **Степен на активност** (dropdown) - избор от 10 типа
- **Температура в помещението** (read-only) - равна на проектната температура

## Изчислени Полета (Read-Only)

- **Явна топлина от един човек [W]** - от таблица + интерполация
- **Скрита топлина от един човек [W]** - от таблица + интерполация
- **Топлина от обитатели [W]** = Брой × Явна топлина
- **Латентна топлина от обитатели [W]** = Брой × Скрита топлина

## Unit Тестове - Резултати

Всички 8 теста **ПРЕМИНАХА УСПЕШНО ✓**:

1. ✓ Точно съвпадение при T=20°C
2. ✓ **Линейна интерполация при T=21°C** (КЛЮЧОВ ACCEPTANCE TEST)
3. ✓ Линейна интерполация при T=25°C
4. ✓ Температура под минимума - Clamp към 20°C
5. ✓ Температура над максимума - Clamp към 28°C
6. ✓ Десетична температура T=20.25°C
7. ✓ Обща топлина за 20 обитатели при T=21°C
8. ✓ Проверка на различни активности при T=24°C

### Acceptance Test - TEST 2 (T=21°C)

```
При T=21°C за Cinema:
  Очаквано: sensible=75.50W, latent=24.50W
  Резултат:  sensible=75.50W, latent=24.50W
  Статус: ✓ УСПЕХ
```

## Използване

1. **Стартиране на приложението:**
   ```powershell
   cd "e:\AI\EE Doklad"
   dotnet run --project EE.Doklad
   ```

2. **Избиране на секция 10:**
   - В лявата панел изберете "10. Отопление"
   - UI автоматично зарежда HeatingSectionView

3. **Попълване на данни:**
   - Въведете ръчните параметри (инфилтрация, температури, ефективности)
   - Изберете степен на активност от dropdown
   - Изчислените стойности се актуализират автоматично

4. **Изпълнение на тестове:**
   ```powershell
   cd "e:\AI\EE Doklad\InterpolationTest"
   dotnet run
   ```

## API Reference

### `ActivityDataService`

```csharp
// Връща всички налични активности
IReadOnlyList<ActivityRow> GetAllActivities()

// Връща активност по ключ
ActivityRow? GetActivity(ActivityLevel level)

// Изчислява sensible и latent heat за дадена температура
(double SensibleHeat, double LatentHeat) CalculateHeatForTemperature(
    ActivityLevel level, 
    double temperature)
```

### `HeatingSectionViewModel`

```csharp
// Ръчни входове (с валидация)
string InfiltrationText { get; set; }
string DesignTemperatureText { get; set; }
string ReductionTemperatureText { get; set; }
double EmissionEfficiency { get; set; }
double DistributionEfficiency { get; set; }
double AutomaticControl { get; set; }
double EnergyManagement { get; set; }
double HeatingEfficiency { get; set; }

// Обитатели
ActivityLevel SelectedActivityLevel { get; set; }
int NumberOfOccupants { get; } // read-only, от ObjectDataSectionData

// Изчислени стойности (read-only)
double SensibleHeatPerPerson { get; }
double LatentHeatPerPerson { get; }
double TotalOccupantHeat { get; }
double TotalLatentHeat { get; }
string RoomTemperatureDisplay { get; }

// Validation errors
string? InfiltrationError { get; }
string? DesignTemperatureError { get; }
string? ReductionTemperatureError { get; }
```

## Архитектура

```
┌─────────────────────────────────────────┐
│          MainWindow.xaml.cs             │
│  (Routing към HeatingSectionView)       │
└─────────────┬───────────────────────────┘
              │
              ▼
┌─────────────────────────────────────────┐
│      HeatingSectionView.xaml            │
│  (UI с ръчни входове + read-only поле)  │
└─────────────┬───────────────────────────┘
              │ DataBinding
              ▼
┌─────────────────────────────────────────┐
│    HeatingSectionViewModel.cs           │
│  • Валидация на входове                 │
│  • Реактивно преизчисление              │
│  • Формат нормализация                  │
└─────────────┬───────────────────────────┘
              │ използва
              ▼
┌─────────────────────────────────────────┐
│    ActivityDataService                  │
│  • Таблица с активности                 │
│  • Линейна интерполация                 │
│  • Clamping                             │
└─────────────────────────────────────────┘
```

## Спазени Изисквания

✅ **Линейна интерполация** - без закръгляване до най-близка колона  
✅ **Acceptance test** - T=21°C дава 75.50W (не 79W или 72W)  
✅ **Локализация** - приема и `,` и `.` като десетичен разделител  
✅ **Форматиране** - до 2 знака след десетичната запетая  
✅ **Валидация** - error messages при невалиден вход  
✅ **Реактивност** - автоматично преизчисление при промени  
✅ **Брой обитатели** - взима се от Секция 5 (read-only)  
✅ **Clamping** - под/над крайни температури  
✅ **Persistence** - данните се запазват в HeatingSectionData модела  

## Известни Ограничения

- Тестовете са в отделен конзолен проект (няма xUnit в основния проект)
- Не е имплементирана PDF export функционалност за секция 10
- JSON сериализация/десериализация не е тествана

## Следващи Стъпки (Опционални)

1. Добавяне на PDF export за секция 10
2. Интеграция с реални данни от климатични зони
3. Добавяне на графики за визуализация на топлинните потоци
4. Имплементация на advanced validation rules
5. Добавяне на unit тестове в отделен test проект с xUnit

---

**Дата на имплементация:** 31 януари 2026  
**Версия:** 1.0  
**Автор:** GitHub Copilot
