# Секция 19: Клас на енергопотребление - Имплементация

## Обзор

Секция 19 "Клас на енергопотребление" е преработена да работи като **автоматична display-only секция**, която получава данни от:
- **Секция 5** "Данни за обекта" → Тип сграда (`BuildingTypeCode`)
- **Секция 18** "Резултати сграда" → EP (годишна специфична енергия) от ред "ОБЩО", колона "kWh/m²"

Без ръчно въвеждане. Автоматично обновяване при промени.

---

## Архитектура

### 1. **Модели**

#### `EnergyClassSectionData.cs`
Основен модел за секция 19:

**Свойства:**
- `BuildingType` (BuildingTypeCode?) - от секция 5, readonly
- `EnergyPerformance` (double?) - EP от секция 18, readonly
- `ThresholdRows` (ObservableCollection<EnergyClassThresholdRow>) - таблица с прагове A-G
- `CalculatedClass` (EnergyClass?) - computed, изчислен клас A-G
- `MarkerValueRounded` (int?) - EP закръглен до цяло число за маркера
- `IsDataAvailable` (bool) - дали има налични данни за изчисление
- `DataUnavailableMessage` (string) - съобщение при липса на данни

**Методи:**
- `RefreshThresholds()` - обновява таблицата с прагове от `EnergyClassCalculator`
- `GetNormalizedMarkerPosition()` - изчислява позицията на маркера в скалата (0.0-1.0)

#### `EnergyClassThresholdRow.cs`
Ред в таблицата с прагове:
- `Class` (string) - "A", "B", ..., "G"
- `MinValue` (double?) - минимална стойност
- `MaxValue` (double?) - максимална стойност
- `RuleText` (string) - "EP < 83", "83 ≤ EP < 166", и т.н.
- `ColorHex` (string) - цвят на класа (#00C853, #FFD600, и т.н.)

#### `ResultsSectionData.cs` - Разширение
Добавено **computed property**:
```csharp
public double? TotalSpecificConsumption
{
    get
    {
        var totalRow = Rows.FirstOrDefault(r => r.RowName == "Общо");
        return double.TryParse(totalRow?.SpecificConsumption, out double value) ? value : null;
    }
}
```

Това свойство се използва от секция 19 за получаване на EP.

---

### 2. **ViewModel**

#### `EnergyClassViewModel.cs`
Свързва секция 19 с останалата част от доклада:

**Отговорности:**
- Намира `ObjectDataSectionData` (секция 5) и `ResultsSectionData` (секция 18)
- Абонира се за промени в `BuildingTypeCode` и `TotalSpecificConsumption`
- Автоматично обновява `EnergyClassSectionData.BuildingType` и `EnergyPerformance`

**Методи:**
- `Initialize(Report, EnergyClassSectionData)` - инициализира връзките
- `Cleanup()` - премахва слушателите при смяна на секция

**Lifecycle:**
- Създава се когато се отваря секция 19
- Cleanup се вика при смяна на секция (в `MainWindow.UpdateSectionContent()`)

---

### 3. **Визуален контрол**

#### `EnergyClassScale.cs`
Custom WPF Control за визуална скала A-G:

**Dependency Properties:**
- `BandHeight` (double) - височина на всяка лента (default 60px)
- `MarkerValue` (int?) - EP стойност за показване
- `NormalizedMarkerPosition` (double?) - позиция на маркера 0.0-1.0
- `CurrentClass` (string) - текущ клас за highlight

**Визуални елементи:**
- 7 цветни ленти (A-G) с градиентни цветове от зелено (#00C853) до червено (#DD2C00)
- Маркер - черна линия с етикет "XXX kWh/m²"
- Стрелка (триъгълник) показваща точната позиция
- Highlight на текущия клас (дебела черна рамка)

**Rendering:**
- Рисува се динамично в Canvas елементи
- Автоматично преизчислява при промяна на размера
- Интерполира позицията на маркера вътре в лентата

---

### 4. **UI (XAML)**

#### `EnergyClassSectionEditor.xaml`
Структура на UI:

**Блок A: Входни данни (Read-Only)**
- Тип сграда - `BuildingTypeDisplay` (от секция 5)
- EP - `EnergyPerformanceDisplay` (от секция 18)

**Блок B: Резултат: Енергиен клас**
- **Лява страна:** Карта с голяма буква на класа (A-G)
- **Дясна страна:** `EnergyClassScale` контрол с визуална скала

**Блок C: Таблица с прагове**
- DataGrid с 4 колони:
  - Клас (цветна лента)
  - От [kWh/m²]
  - До [kWh/m²]
  - Условие (RuleText)

**Conditional Visibility:**
- Ако `IsDataAvailable == false` → показва се предупреждение
- Ако `IsDataAvailable == true` → показват се резултат + таблица

---

## Интеграция с други секции

### Секция 5 "Данни за обекта"
**Източник:** `ObjectDataSectionData.BuildingTypeCode`

**Промяна:**
```csharp
ObjectDataSection_PropertyChanged(sender, e)
{
    if (e.PropertyName == nameof(BuildingTypeCode))
    {
        RefreshBuildingType(); // обновява EnergyClassSectionData.BuildingType
    }
}
```

### Секция 18 "Резултати сграда"
**Източник:** `ResultsSectionData.TotalSpecificConsumption`

**Промяна:**
```csharp
ResultsSection_PropertyChanged(sender, e)
{
    if (e.PropertyName == nameof(TotalSpecificConsumption))
    {
        RefreshEnergyPerformance(); // обновява EnergyClassSectionData.EnergyPerformance
    }
}
```

**Trigger:**
- При промяна на `ConsumedEnergy` в ред "Общо"
- При промяна на `HeatedArea` (влияе на `SpecificConsumption`)

---

## Изчисление на енергиен клас

### Използван сервис
`EnergyClassCalculator.CalculateClass(BuildingTypeCode, double ep)`

**Прагове по тип сграда (Приложение №2):**
```csharp
SingleFamilyResidential:  A:83,  B:166, C:203, D:240, E:300, F:360
MultiFamilyResidential:   A:90,  B:180, C:235, D:290, E:363, F:435
Administrative:           A:134, B:268, C:329, D:390, E:488, F:585
Schools:                  A:35,  B:70,  C:110, D:150, E:188, F:225
...
```

**Логика:**
```csharp
if (ep < A) return EnergyClass.A;
if (ep < B) return EnergyClass.B;
...
if (ep < F) return EnergyClass.F;
return EnergyClass.G;
```

### Позициониране на маркера

**Интерполация вътре в лента:**
- За класове B-F (затворени интервали):
  ```
  t = (ep - min) / (max - min)  // 0.0 - 1.0
  position = bandStart + t * bandHeight
  ```
- За клас A (отворен отгоре): `t = ep / A` (clamped 0-1)
- За клас G (отворен отдолу): `t = (ep - F) / (F * 0.5)` (50% допълнителна скала)

**Normalized position:**
- bandIndex = 0..6 (A=0, G=6)
- bandStart = bandIndex / 7
- bandHeight = 1 / 7
- finalPosition = bandStart + t * bandHeight

---

## Файлове

### Нови файлове
```
EE.Doklad/
├── Models/
│   └── EnergyClassSectionData.cs (UPDATED)
│       - Добавени: ThresholdRows, MarkerValueRounded, IsDataAvailable,
│         GetNormalizedMarkerPosition(), RefreshThresholds()
│       - Премахнати: ThresholdsInfo (заменен с ThresholdRows)
│
├── ViewModels/
│   └── EnergyClassViewModel.cs (NEW)
│       - Автоматична синхронизация с секция 5 и 18
│
├── Views/
│   ├── EnergyClassSectionEditor.xaml (UPDATED)
│   │   - 3 блока (Input, Result+Scale, Thresholds Table)
│   │   - Премахнато: ръчно въвеждане на EP
│   │
│   └── Controls/
│       └── EnergyClassScale.cs (NEW)
│           - Custom WPF Control за визуална скала A-G
│
└── MainWindow.xaml.cs (UPDATED)
    - Инициализация на EnergyClassViewModel
    - Cleanup при смяна на секция
```

### Модифицирани файлове
```
EE.Doklad/Models/ResultsModels.cs
    - Добавено: TotalSpecificConsumption computed property
    - Добавено: NotifyPropertyChanged при промяна на ред "Общо"
```

---

## Backward Compatibility

### Стари доклади
- Ако липсва `BuildingTypeCode` → показва "Не е избран"
- Ако липсва EP от секция 18 → показва "—"
- Ако липсват и двете → показва инструктивен текст
- Не чупи serialization/deserialization

### Миграция
Няма нужда от миграция. Старите доклади ще работят със секция 19:
- Ако имат попълнен тип сграда и данни в секция 18 → автоматично показва клас
- Ако нямат → показва съобщение за липса на данни

---

## Тестове

### Ръчни тестове (smoke tests)

**1. Промяна на тип сграда:**
- Отворете секция 5, променете "Тип сграда"
- Отворете секция 19 → трябва да се обнови таблицата с прагове

**2. Промяна на EP:**
- Отворете секция 18, въведете/променете данни в ред "Отопление"
- Ред "ОБЩО" се обновява автоматично
- Отворете секция 19 → класът и маркерът трябва да се обновят

**3. Гранични стойности:**
- Въведете EP = 83.0 (SingleFamily) → трябва да е клас B
- Въведете EP = 82.9 → трябва да е клас A
- Въведете EP = 500 → трябва да е клас G

**4. Визуална скала:**
- Проверете че маркерът се появява на правилната позиция
- Проверете че текущият клас е highlighted с черна рамка

### Unit тестове (концептуални)
Тъй като проектът няма test framework, тестовете са документирани в:
- `ENERGY_CLASS_IMPLEMENTATION.md` (този файл)

**Покрити сценарии:**
- `CalculateClass()` с различни EP стойности
- Гранични стойности (точно на прага между класове)
- `GetThresholds()` за всички типове сгради
- `BuildThresholdRows()` - 7 реда A-G с правилни RuleText
- `MarkerValueRounded` - закръгляване до цяло число
- `GetNormalizedMarkerPosition()` - позиция в диапазон 0-1
- `IsDataAvailable` - true/false при различни комбинации от данни

---

## Performance

### Automatic Updates
- Използва `INotifyPropertyChanged` за ефективно обновяване
- Само засегнатите UI елементи се преизчертават
- Няма polling - само event-based updates

### Memory
- `EnergyClassViewModel` се cleanup-ва при смяна на секция
- Няма memory leaks от event subscriptions

---

## Бъдещи подобрения

### Възможни разширения:
1. **Анимация на маркера** при промяна на EP
2. **Export на скалата като PNG** за включване в PDF доклади
3. **Comparison mode** - показване на няколко сценария едновременно
4. **Historical tracking** - показване на промените в класа във времето

### Оптимизации:
1. Виртуализация на таблицата при много редове (сега - само 7)
2. Кеширане на изчислените позиции на маркера

---

## Заключение

Секция 19 "Клас на енергопотребление" е напълно автоматична, display-only секция, която:
- ✅ Получава тип сграда от секция 5
- ✅ Получава EP от секция 18 (ред "ОБЩО")
- ✅ Изчислява енергиен клас A-G автоматично
- ✅ Показва визуална скала с маркер
- ✅ Показва таблица с прагове
- ✅ Обновява се при промени в секция 5 или 18
- ✅ Backward compatible със стари доклади
- ✅ Без ръчно въвеждане на данни

**Потребителят вижда:** автоматично изчислен клас на база попълнените данни в останалата част на доклада.

**Разработчикът вижда:** чист separation of concerns със ViewModel, reactive updates, и reusable visual control.
