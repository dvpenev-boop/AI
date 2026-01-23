# Секция 9: Прозорци и врати - Имплементация

## Общ преглед

Секция 9 "Прозорци и врати" е напълно преработена за енергийно изчисление на слънчеви печалби по методиката (формули 3.41/3.42 + таблици за щори). Секцията използва модел с "партиди" (batches) и обобщена таблица за визуализация.

## Архитектура

### Модели (Models/)

#### WindowsSectionData
- **WindowBatches**: ObservableCollection от WindowBatch - източник на истина
- **Description**: Описателен текст

#### WindowBatch (партида прозорци/врати)
Съдържа:
- **Основни данни**: Kind (Window/Door), Orientation, Count
- **Геометрия**: Width, Height, AreaGross
- **Топлотехнически**: UValue, GN (g perpendicular), OpticalType
- **Рамка**: FrameFraction → изчислява AreaGlass = AreaGross × (1 - FrameFraction)
- **Слънцезащита**: ShadingTypeId, ShadingReductionFactor
- **Препятствия**: ObstacleProfileId, MonthlyObstacleFactors[12]

#### Derived Properties:
- **AreaGlass**: Автоматично изчислена площ на стъклото
- **GEffBase**: Базова ефективна пропускливост
  - Ако OpticalType == Clear и няма щора: `g_eff_base = 0.90 × g_n` (формула 3.41)
  - Иначе: `g_eff_base = 0.75 × g_alt + 0.25 × g_dif` (формула 3.42)
- **GEff**: Финална ефективна пропускливост след shading: `g_eff = g_eff_base × ShadingReductionFactor`

#### WindowSummaryRow (обобщен ред за визуализация)
Групира партиди по:
- **Orientation** (фасада)
- **TypeSignature** (тип прозорец/врата)

Изчислява:
- **TotalCount**: Σ Count
- **ATotalGross**: Σ (Count × AreaGross)
- **ATotalGlass**: Σ (Count × AreaGlass)
- **UAvg**: Средно претеглен U спрямо площта = Σ(U × Count × AreaGross) / ATotalGross
- **GAvg**: Средно претеглен g спрямо площта на стъклото = Σ(g_eff × Count × AreaGlass) / ATotalGlass

#### ShadingOption (Таблица 4)
Опции за слънцезащита с коефициенти:
- Бели венециански щори (α=0.10, 3 варианта по τ)
- Бели завеси (α=0.10, 3 варианта по τ)
- Цветен текстил (α=0.30, 3 варианта по τ)
- Текстил с алуминиево покритие (α=0.20, τ=0.05)

Всеки вариант има:
- **FShadeInt**: Коефициент за вътрешна щора
- **FShadeExt**: Коефициент за външна щора

#### ObstacleProfile
Предефинирани профили за засенчване:
- **None**: Без препятствия (всички коефициенти = 1.0)
- **Balcony**: Балкон (типични месечни коефициенти)
- **AdjacentBuilding**: Съседна сграда
- **Trees**: Дървета (по-силно засенчване когато има листа)
- **Custom**: Потребителят задава сам 12 месечни коефициента

### Services

#### WindowCalculator
Статичен service с методи:
- **GroupBatches()**: Групира партиди по фасада и тип
- **CalculateUAvg()**: Изчислява средно претеглен U
- **CalculateGAvg()**: Изчислява средно претеглен g
- **CalculateAreaGlass()**: A_gl = A_gross × (1 - F_fr)
- **CalculateGEffBase()**: Прилага формули 3.41/3.42
- **GetShadingOptions()**: Връща каталог с опции от Таблица 4
- **GetObstacleProfiles()**: Връща предефинирани профили

### ViewModels

#### WindowsSectionViewModel
Управлява секция 9:
- **SummaryRows**: ObservableCollection от WindowSummaryRow (за визуализация)
- **SelectedSummaryRow**: Избрана група
- **Commands**:
  - AddWindowCommand: Отваря AddWindowWizardDialog
  - EditSelectedCommand: Отваря Details за избраната група
  - DeleteSelectedCommand: Изтрива всички партиди от групата
  - OpenDetailsCommand: Показва WindowBatchDetailsDialog

- **RefreshSummary()**: Презарежда обобщената таблица от партидите

### Views

#### WindowsSectionView (основен UI)
UserControl със:
- **Header**: Заглавие "9. Прозорци и врати"
- **Description**: Кратко описание
- **Toolbar**: Бутони за Add/Edit/Delete/Import/Export
- **Summary Table**: DataGrid с обобщени редове:
  - Фасада | Тип | Брой | A брутна | A стъкло | Ū | ḡ | [Details >]
- **Info Panel**: Информационен текст

#### AddWindowWizardDialog (6-стъпков wizard)

**Стъпка 1: Основни данни**
- Вид: Прозорец/Врата
- Фасада (ориентация)
- Брой (Count)

**Стъпка 2: Геометрия**
- Radio: Ширина × Височина ИЛИ Директно площ
- Автоматично изчисляване и показване

**Стъпка 3: Топлотехнически/оптични данни**
- Radio: Каталог ИЛИ Ръчно
- U-стойност (W/m²K)
- g_n (перпендикуляр, 0-1)
- Оптичен тип: Clear/Diffusing
- Info: Clear = 4 сезона, Diffusing = матирано/със щора

**Стъпка 4: Рамка**
- F_fr процент площ на рамката (типично 10-20%)
- Показва изчислено: A_стъкло = A_брутна × (1 - F_fr)

**Стъпка 5: Слънцезащита** ⭐ НОВО!
- Radio: Без / Вътрешна / Външна
- Dropdown: Вид слънцезащита (4 категории)
- **Таблица с коефициенти (Табл. 4)**:
  - Вид | α | τ | F_вътр | F_външ
  - При избор автоматично попълва ShadingReductionFactor
- Preview поле: "Избран коефициент: X.XX"
- Info: Коефициентът е множител към g_base

**Стъпка 6: Препятствия**
- Dropdown: Профил (None/Balcony/AdjacentBuilding/Trees/Custom)
- Info: Месечни коефициенти F_sh,obst[m]

**Навигация**:
- Бутони: < Назад | Напред > | Отказ
- Валидация на всяка стъпка
- При Finish: Създава/актуализира WindowBatch

#### WindowBatchDetailsDialog
Modal прозорец за детайли на група:
- **Header**: Име на групата
- **Summary Panel**: Общо партиди | Брой | A | Ū | ḡ
- **Toolbar**: + Добави партида към тази група
- **Batches Table**: DataGrid с партидите:
  - № | Щора | Препятствия | F_fr | A_gross | A_gl | U | g_eff | [Edit] [Delete]
- **Close Button**

### Converters

#### OrientationConverter
Конвертира Orientation enum → текстов етикет (И, СИ, С, СЗ, З, ЮЗ, Ю, ЮИ)

#### ShadingTypeConverter
Конвертира ShadingTypeId → кратък текст ("Бели венециански", "Цветен текстил", ...)

#### ObstacleProfileConverter
Конвертира ObstacleProfileId → текст ("Без", "Балкон", "Съседна сграда", ...)

## Интеграция

### MainWindow.xaml.cs
Добавен case за SectionType.Windows:
```csharp
else if (section.Type == ModelSectionType.Windows ||
         (!string.IsNullOrEmpty(section.Title) && section.Title.Contains("Прозорци")))
{
    if (section.WindowsSectionData == null)
        section.WindowsSectionData = new Models.WindowsSectionData();

    section.Type = ModelSectionType.Windows;

    var windowsSectionView = new WindowsSectionView
    {
        DataContext = new ViewModels.WindowsSectionViewModel(section.WindowsSectionData)
    };
    ContentScrollViewer.Content = windowsSectionView;
}
```

### Section.cs
Разширен SectionType enum:
```csharp
public enum SectionType
{
    ...
    Windows  // Прозорци и врати (секция №9)
}
```

Добавено поле:
```csharp
public WindowsSectionData? WindowsSectionData { get; set; }
```

## Използване

### Добавяне на прозорец/врата:
1. Натискане на "+ Добави прозорец/врата"
2. Wizard с 6 стъпки
3. След завършване: автоматично групиране и обобщаване

### Преглед на детайли:
1. Избор на група от обобщената таблица
2. Натискане на "Details >"
3. Показва се списък с партидите

### Редакция на партида:
1. Details dialog → [Edit] бутон на партида
2. Отваря се wizard с попълнени данни
3. След Save: презареждане на обобщената таблица

### Изтриване:
- Ниво партида: Delete бутон в Details dialog
- Ниво група: Delete избраното от основния екран (изтрива всички партиди)

## Формули и изчисления

### g_eff изчисление

**Формула 3.41** (Clear, без щора):
```
g_eff = 0.90 × g_n
```

**Формула 3.42** (Diffusing или със щора):
```
g_eff_base = 0.75 × g_alt + 0.25 × g_dif
```
*(В първа версия: g_alt = g_dif = g_n като placeholder)*

**С щора**:
```
g_eff = g_eff_base × ShadingReductionFactor
```

### Групиране и осредняване

**Групиране** по:
```
GroupKey = (Orientation, TypeSignature)
```

**U_avg** (претеглен по площ):
```
U_avg = Σ(U × Count × A_gross) / Σ(Count × A_gross)
```

**g_avg** (претеглен по площ на стъклото):
```
g_avg = Σ(g_eff × Count × A_glass) / Σ(Count × A_glass)
```

## Таблица 4: Коефициенти за намаление (точни стойности)

### Бели венециански щори (α=0.10)
| τ    | F_внутр | F_външ |
|------|---------|--------|
| 0.05 | 0.25    | 0.10   |
| 0.10 | 0.30    | 0.15   |
| 0.30 | 0.45    | 0.35   |

### Бели завеси (α=0.10)
| τ    | F_внутр | F_външ |
|------|---------|--------|
| 0.50 | 0.65    | 0.55   |
| 0.70 | 0.80    | 0.75   |
| 0.90 | 0.95    | 0.95   |

### Цветен текстил (α=0.30)
| τ    | F_внутр | F_външ |
|------|---------|--------|
| 0.10 | 0.42    | 0.17   |
| 0.30 | 0.57    | 0.37   |
| 0.50 | 0.77    | 0.57   |

### Текстил с алуминиево покритие (α=0.20)
| τ    | F_внутр | F_външ |
|------|---------|--------|
| 0.05 | 0.20    | 0.08   |

## Важни забележки

### ❌ НЕ смесвай с:
- Прозрачни към неотопляемо (секция 8 - под към неотопляем сутерен)
- Изчисления по 13370
- Други "прозрачни" елементи

### ✅ Фокус:
- Външни фасади (Orientation задължително)
- Слънчеви печалби
- Методиката с формули 3.41/3.42
- Таблица 4 за щори

### TODO за бъдещи версии:
- [ ] Каталог типове прозорци (предефинирани U, g_n)
- [ ] Отделни полета за g_alt и g_dif (вместо placeholder)
- [ ] Custom месечни коефициенти за препятствия (редактор)
- [ ] Експорт/импорт от Excel
- [ ] Визуализация на групите по месеци
- [ ] 3D симулация на засенчване (advanced)

## Файлове

### Нови файлове:
```
EE.Doklad/
├── Models/
│   └── WindowsSectionData.cs          (WindowsSectionData, WindowBatch, WindowSummaryRow, енуми)
├── Services/
│   └── WindowCalculator.cs            (GroupBatches, изчисления, каталози)
├── ViewModels/
│   └── WindowsSectionViewModel.cs     (команди, refresh логика)
├── Views/
│   ├── WindowsSectionView.xaml        (основен UI)
│   ├── WindowsSectionView.xaml.cs
│   ├── AddWindowWizardDialog.xaml     (6-стъпков wizard)
│   ├── AddWindowWizardDialog.xaml.cs
│   ├── WindowBatchDetailsDialog.xaml  (детайли на група)
│   └── WindowBatchDetailsDialog.xaml.cs
└── Converters/
    ├── OrientationConverter.cs
    └── WindowConverters.cs            (ShadingTypeConverter, ObstacleProfileConverter)
```

### Модифицирани файлове:
```
EE.Doklad/
├── Models/
│   └── Section.cs                     (+ Windows enum, + WindowsSectionData поле)
└── MainWindow.xaml.cs                 (+ case за Windows секция)
```

## Тестване

Проектът НЕ използва unit testing framework. За ръчно тестване:

1. **Добавяне на прозорец**:
   - Тествай всички 6 стъпки
   - Проверка на валидация
   - Проверка на изчисления (A_glass, g_eff)

2. **Групиране**:
   - Добави 2+ партиди със същи характеристики → трябва да се групират
   - Добави партиди с различни фасади → отделни групи

3. **Изчисления**:
   - U_avg: Различни U в една група → среднопретеглено
   - g_avg: Различни g/щори в група → среднопретеглено по A_glass

4. **Щори (Стъпка 5)**:
   - None → g_eff = g_eff_base
   - Вътрешна Бели венециански τ=0.05 → g_eff = g_eff_base × 0.25
   - Външна Цветен текстил τ=0.10 → g_eff = g_eff_base × 0.17

5. **Details Dialog**:
   - Редакция на партида
   - Изтриване на партида
   - Добавяне на нова партида в група

## Край на документация
