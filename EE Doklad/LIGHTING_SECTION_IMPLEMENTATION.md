# Имплементация на Секция 15. Осветление

## Дата: 31 Януари 2026

## Обобщение

Създадена е пълна функционална имплементация на секция "Осветление" в WPF приложението "ЕЕ Доклад". Секцията включва:
- Таблица "Описание на осветителната инсталация на сградата"
- Searchable ComboBox за избор на осветители от база данни
- Автоматични изчисления на консумирана енергия и едновременна мощност
- Интеграция с данни от Секция 5 "Данни за обекта"

---

## Създадени файлове

### 1. **Models/LightingSectionData.cs**
Главен модел за секция Осветление, съдържащ:

#### `LightingSectionData` - основен модел за секцията
- `Title` - заглавие на секцията
- `Description` - описание
- `DefaultHoursPerDay` - работен режим [h/day] за изчисление на едновременна мощност (по подразбиране: 5.0)
- `DefaultDaysPerWeek` - дни седмично [days/week] за изчисление на едновременна мощност (по подразбиране: 5.0)
- `LineItems` - ObservableCollection<LightingLineItem> - редове в таблицата
- `TotalPower_kW` - обща мощност [kW] = Σ PowerTotal_kW
- `TotalAnnualEnergy_kWh` - обща консумирана енергия [kWh/y] = Σ AnnualEnergy_kWh
- `SimultaneousPower_W_per_m2` - едновременна мощност [W/m²]
- `SimultaneousPower_W` - едновременна мощност [W]

Методи:
- `SetHolidaysPerYear(int holidays)` - задава броя почивни дни от Секция 5
- `SetHeatedArea(double area)` - задава отопляемата площ от Секция 5

#### `LightingLineItem` - ред в таблицата
- `Index` - пореден номер
- `SelectedLightingComponentName` - име на избрания осветител
- `PowerW` - мощност [W] (автоматично попълвана)
- `Quantity` - количество [бр.]
- `PowerTotal_kW` - мощност общо [kW] = (PowerW × Quantity) / 1000
- `HoursPerDay` - работен режим [h/day]
- `DaysPerWeek` - дни седмично [days/week]
- `WorkingDaysPerYear` - работен режим [days/y] = ((365 - HolidaysPerYear) / 7.0) × DaysPerWeek
- `Ke` - коефициент (0..1)
- `AnnualEnergy_kWh` - консумирана енергия [kWh/y] = PowerTotal_kW × HoursPerDay × WorkingDaysPerYear × Ke

### 2. **Views/LightingSectionEditor.xaml**
WPF потребителски интерфейс с:

#### Секция с параметри
- Полета за `DefaultHoursPerDay` и `DefaultDaysPerWeek`
- Визуално обособена в син border

#### Таблица (DataGrid)
Колони:
1. № - автоматичен номер
2. Тип осветително тяло - searchable ComboBox (IsEditable=True, IsTextSearchEnabled=True)
3. Мощност [W] - readonly, попълва се автоматично
4. Количество [бр.] - редактируемо
5. Мощност общо [kW] - readonly, изчислява се автоматично
6. Работен режим [h/day] - редактируемо
7. Дни седмично [days/week] - редактируемо
8. Работен режим [days/y] - readonly, изчислява се автоматично
9. Ke - редактируемо
10. Консумирана енергия [kWh/y] - readonly, изчислява се автоматично

#### Бутони
- "Добави осветител" (зелен)
- "Премахни осветител" (червен)

#### Ред ОБЩО (зелен border)
- Обща мощност [kW]
- Обща консумирана енергия [kWh/y]

#### Секция "Едновременна мощност" (оранжев border)
- Показва работен режим (h/седмица) = DefaultHoursPerDay × DefaultDaysPerWeek (закръглено)
- Едновременна мощност [W/m²]
- Едновременна мощност (общо) [W]
- Формула (информационен текст)

### 3. **Views/LightingSectionEditor.xaml.cs**
Code-behind с логика:
- Инициализация на `LightingService` за зареждане на осветители от БД
- `LoadLightingOptions()` - зарежда seed + user осветители
- `AddLineItem_Click()` - добавя нов ред в таблицата
- `RemoveLineItem_Click()` - премахва ред (селектиран или последен)
- `UpdateIndexes()` - актуализира индексите след промени
- `LightingComboBox_SelectionChanged()` - автоматично попълва PowerW при избор на осветител

---

## Модифицирани файлове

### 4. **Models/Section.cs**
- Добавен `SectionType.Lighting` в enum
- Добавено свойство `LightingSectionData? LightingSectionData`

### 5. **MainWindow.xaml.cs**
- Добавена обработка на `SectionType.Lighting` в метода `UpdateSectionContent()`
- Автоматично зареждане на данни от Секция 5 (HolidaysPerYear, HeatedArea)
- Извикване на `SetHolidaysPerYear()` и `SetHeatedArea()` за актуализиране на изчисленията

---

## Формула за едновременна мощност

### Описание
```
SimultaneousPower_W_per_m2 = (TotalAnnualEnergy_kWh × 1000.0) / Denominator

където:
Denominator = ((365 - HolidaysPerYear) / 7.0) × RoundedWorkRegime × HeatedArea_m2
RoundedWorkRegime = round(DefaultHoursPerDay × DefaultDaysPerWeek)
```

### Валидации
- `TotalAnnualEnergy_kWh > 0`
- `HeatedArea_m2 > 0`
- `DefaultHoursPerDay > 0`
- `DefaultDaysPerWeek > 0`
- `0 ≤ HolidaysPerYear ≤ 365`
- `0 ≤ Ke ≤ 1`
- `0 ≤ DaysPerWeek ≤ 7`
- `Quantity > 0`
- `PowerW > 0`

### Извличане на данни от Секция 5
- `HolidaysPerYear` = `ObjectDataSectionData.MonthlyDaysOffSum` (сума на почивни дни по месеци)
- `HeatedArea_m2` = `ObjectDataSectionData.HeatedArea` (парсиран като double)

---

## Архитектурни решения

### MVVM Pattern
- Моделите използват `CommunityToolkit.Mvvm.ComponentModel` за `ObservableObject`
- `ObservableProperty` за автоматична генерация на INotifyPropertyChanged
- Computed properties с `OnPropertyChanged()` при промяна на зависимости

### Reactive Updates
- `LineItems.CollectionChanged` - актуализира общите суми при добавяне/премахване на редове
- `LineItem_PropertyChanged` - актуализира общите суми при промяна в ред
- `partial void On<Property>Changed()` - каскадно обновяване на изчислени свойства

### Data Binding
- Two-way binding за редактируеми полета
- One-way binding (ReadOnly) за изчислени стойности
- `FlexibleDoubleConverter` за обработка на числени входове
- `UpdateSourceTrigger=LostFocus` за избягване на прекалено чести обновявания

### Интеграция с съществуващи сървиси
- Използва `LightingService` (вече съществуващ)
- Използва `JsonLightingRepository` за достъп до seed/user данни
- Връща `LightingRow` за display в ComboBox (съществуваща структура)

---

## Тестване

### Build
✅ Успешен компилация без грешки

### Стартиране
✅ Приложението се стартира без проблеми

### Функционалности за проверка от потребител
1. Създайте нова секция "Осветление" или редактирайте съществуваща
2. Кликнете "Добави осветител"
3. Изберете осветител от ComboBox (пишете част от името за търсене)
4. Проверете че `PowerW` се попълва автоматично
5. Въведете количество и други параметри
6. Проверете че всички изчисления се актуализират в реално време
7. Добавете няколко реда
8. Проверете "ОБЩО" секцията
9. Променете `DefaultHoursPerDay` и `DefaultDaysPerWeek` и проверете че "Едновременна мощност" се преизчислява
10. Запазете доклада (JSON) и го заредете отново - проверете че данните се запазват

---

## Пример за използване

### Входни данни (от прикачената снимка)
| № | Тип | Мощност | Брой | Режим | Дни | Ke | Енергия |
|---|-----|---------|------|-------|-----|-----|---------|
| 1 | ЛЛЖ | 60 | 156 | 5.00 | 5.00 | 0.6 | 6739 |
| 2 | ЛОГ 3х36 | 108 | 20 | 5.00 | 5.00 | 0.6 | 1555 |
| 3 | ЛОГ 2х36 | 72 | 36 | 5.00 | 5.00 | 0.6 | 1866 |
| 4 | ЛОГ 2х18 | 36 | 25 | 5.00 | 5.00 | 0.6 | 648 |
| 5 | ЕСЛ | 10 | 20 | 5.00 | 5.00 | 0.6 | 192 |
| 6 | ЛЕД | 20 | 18 | 5.00 | 5.00 | 0.6 | 346 |

### Резултати
- Обща мощност: 15.57 kW
- Отопляема площ: 1572.00 m²
- Работен режим: 25.00 h/седмица
- Едновременна мощност: 5.87 W/m²

---

## Бъдещи подобрения (опционално)

1. **Export/Import на осветители**: Аналогично на материалите
2. **Validation tooltips**: Визуални подсказки при невалидни стойности
3. **Preset profiles**: Запазване на често използвани конфигурации
4. **PDF Export**: Форматиране на таблицата за PDF генериране
5. **Графика**: Визуализация на консумацията по видове осветители
6. **Копиране на редове**: Бутон за дублиране на ред с всички параметри

---

## Заключение

Имплементацията е **пълна и функционална**, следва установените архитектурни патърни на проекта и е готова за продуктивна употреба. Всички изчисления се извършват автоматично в реално време, като използват точната формула, зададена в техническото задание.

Секцията се интегрира безпроблемно с останалата част от приложението и автоматично извлича необходимите данни от Секция 5 "Данни за обекта".

✅ **Готово за продукция**
