# Имплементация на Секция 10: Неклиматизирани зони (ztu)

## Обобщение

Успешно добавена нова секция "10. Неклиматизирани зони (ztu)" между секция "9. Прозорци и врати" и "11. Отопление" (предишна 10).

## Направени промени

### 1. Модели (Models)

#### **Models/Section.cs**
- Добавен `UnconditionedZones` към `SectionType` enum между `Windows` и `Heating`
- Добавено property `UnconditionedZoneSectionData?` към класа `Section`
- Актуализирани коментари с новата номерация (след преномериране)

#### **Models/UnconditionedZoneModels.cs** (нов файл)
Създадени модели за поддръжка на неклиматизирани зони:

- **`ZtuType` enum**: External (ztue), Internal (ztui)
- **`ElementKind` enum**: Wall, Roof, Floor (определя Rsi)
- **`ZtuLayer`**: Слой в многослойна конструкция (материал, дебелина, λ, R)
- **`ZtuElement`**: Ограждащ елемент (име, вид, площ, слоеве, U-value)
- **`ZtuZone`**: Неклиматизирана зона (име, тип, бележки, елементи към външна среда и разделящи)
- **`UnconditionedZoneSectionData`**: Данни за секцията (заглавие, описание, списък със зони)

### 2. ViewModels

#### **ViewModels/MainViewModel.cs**
- Актуализиран `CreateSampleReport()`:
  - Добавена секция "10. Неклиматизирани зони (ztu)"
  - Преномерирани визуално всички следващи секции (10→11, 11→12, ..., 20→21)
  - Добавена логика за инициализиране на `UnconditionedZoneSectionData`

### 3. Views (UI)

#### **Views/UnconditionedZonesSectionView.xaml** (нов файл)
- Placeholder UI с информация за функционалността
- Показва заглавие, описание и статус на имплементацията
- DataContext: `UnconditionedZoneSectionData`

#### **Views/UnconditionedZonesSectionView.xaml.cs** (нов файл)
- Code-behind за View (празен конструктор)

#### **MainWindow.xaml.cs**
- Добавен case в `UpdateSectionContent()` за `ModelSectionType.UnconditionedZones`
- При избор на секцията се зарежда `UnconditionedZonesSectionView`

## Ключови характеристики на решението

### ✅ Минимално инвазивно
- **Stable IDs**: `SectionType` enum стойностите остават стабилни keys
- **Display Numbers**: Визуалното преномериране е само в `Title` property
- **Backwards compatibility**: Стари проекти без `UnconditionedZoneSectionData` ще се зареждат нормално (nullable property)

### ✅ Разширяемост
Моделите поддържат:
- Многослойни конструкции (както External Walls)
- Различни видове детайли (стена/покрив/под) с коректни Rsi
- Елементи към външна среда и разделящи елементи
- Месечни изчисления (готови за бъдеща имплементация)

### ✅ Архитектурна консистентност
- Следва същата структура като съществуващите секции (ExternalWalls, Roof, Floor)
- Използва `ObservableObject` и `ObservableCollection` за data binding
- Поддържа MVVM pattern

## Следващи стъпки (за пълна имплементация)

### Фаза 2: UI и Функционалност
1. **ViewModel**: `UnconditionedZonesSectionViewModel.cs`
   - Команди за добавяне/изтриване на зони
   - Команди за добавяне/изтриване на елементи
   - Изчисление на U-value за елементи
   
2. **Dialog**: `AddZtuElementDialog.xaml/.cs`
   - Избор на ElementKind (Wall/Roof/Floor)
   - Редактор на многослойни конструкции
   - Изчисляване на U според Rsi (boundary conditions)

3. **Пълен UI**: Актуализация на `UnconditionedZonesSectionView.xaml`
   - ListBox със зони
   - TabControl за "Към външна среда" / "Към климатизирани помещения"
   - DataGrid таблици без колони за ориентация (копие от ExternalWalls)
   - Бутони за управление

### Фаза 3: Изчисления
1. **Calculator**: `Services/UnconditionedZonesCalculator.cs`
   - Месечни изчисления: `H_ztu,e,m`, `H_ztc-ztu,m`, `H_ztu,tot,m`
   - Редукционен фактор: `b_ztu,m = H_ztu,e,m / H_ztu,tot,m`
   - Температура в ztu: `θ_ztu,m = θ_e,a,m + b_ztu,m * (θ_int - θ_e,a,m)`
   - Влияние върху Htr: `H_el,k,m = b_ztu,m * U_k,m * A_k` (ztue) или `(1-b_ztu,m) * ...` (ztui)

2. **Интеграция**: Актуализация на съществуващи calculation services
   - Включване на ZTU елементи в общите heat transfer calculations
   - Агрегиране в `HH;tr(excl.gf)`

### Фаза 4: Тестване
1. **Unit Tests**: `Tests/UnconditionedZonesCalculatorTests.cs`
   - Тест за `b_ztu` в граници [0..1]
   - Тест за `θ_ztu` между `θ_e` и `θ_int`
   - Edge cases (празна зона, няма H_ztu,tot)

2. **Integration Tests**
   - Зареждане на стар проект (без ZTU данни)
   - Добавяне на ZTU и проверка на изчисления
   - Export към PDF

## Технически детайли

### Rsi стойности според ElementKind
- **Wall (вертикална)**: Rsi = 0.13 m²K/W
- **Roof (топлина нагоре)**: Rsi = 0.10 m²K/W
- **Floor (топлина надолу)**: Rsi = 0.17 m²K/W

### Boundary conditions
- **Към ZTU**: Rsi от двете страни (side A и side B)
- **Към външен въздух**: Rsi вътре + Rse вън (като External Walls)

### Месечни формули
```
Hztu,e,m = Σ(Uk,m * Ak) за елементи към външен въздух
Hztc,j-ztu,m = Σ(Uk,m * Ak) за разделящи елементи
Hztu,tot,m = Hztu,e,m + Σ_j(Hztc,j-ztu,m)
bztu,m = Hztu,e,m / Hztu,tot,m (guard: if Hztu,tot == 0 → bztu = 0)

θztu,m = θe,a,m + bztu,m * (θint,weighted - θe,a,m)

где θint,weighted = Σ_j(Fztc,j;ztu,m * θint,calc,ztc,j,m)
и Fztc,j;ztu,m = Hztc,j-ztu,m / Σ_j(Hztc,j-ztu,m)
```

## Статус

### ✅ Завършено (Фаза 1)
- [x] Добавяне на SectionType.UnconditionedZones
- [x] Създаване на domain models
- [x] Добавяне на UnconditionedZoneSectionData към Section
- [x] Актуализация на CreateSampleReport с преномериране
- [x] Placeholder UI view
- [x] Интеграция в MainWindow
- [x] Успешна компилация и стартиране

### ⏳ В процес (Фаза 2-4)
- [ ] Пълен ViewModel с команди
- [ ] Dialog за добавяне на елементи
- [ ] Пълен UI с таблици и табове
- [ ] Calculation engine
- [ ] Unit tests
- [ ] Integration tests

## Файлове

### Нови файлове (3)
1. `Models/UnconditionedZoneModels.cs` (157 реда)
2. `Views/UnconditionedZonesSectionView.xaml` (47 реда)
3. `Views/UnconditionedZonesSectionView.xaml.cs` (15 реда)

### Модифицирани файлове (3)
1. `Models/Section.cs` - Добавен SectionType + property
2. `ViewModels/MainViewModel.cs` - Актуализиран CreateSampleReport
3. `MainWindow.xaml.cs` - Добавен case за нова секция

**Общо нови редове**: ~220
**Общо модифицирани редове**: ~50

## Проверка

Приложението стартира успешно:
- ✅ Компилация без грешки
- ✅ Стартиране без crash
- ✅ Новата секция "10. Неклиматизирани зони (ztu)" се показва в менюто
- ✅ Останалите секции са преномерирани визуално (10→11, ..., 20→21)
- ✅ При избор на секция 10 се зарежда placeholder UI

## Бележки

- **Преномерирането е САМО visual**: `SectionType` enum стойностите остават стабилни
- **Backwards compatibility**: Стари проекти ще се зареждат без проблем (nullable property)
- **Разширяемост**: Архитектурата позволява лесно добавяне на функционалност
- **Consistency**: Следва установените patterns в съществуващия код

## Следваща стъпка

За пълна функционалност препоръчвам:
1. Имплементиране на `UnconditionedZonesSectionViewModel`
2. Копиране и адаптиране на UI от `ExternalWallsSectionEditor.xaml` (без ориентации)
3. Създаване на `AddZtuElementDialog` с избор на ElementKind
4. Имплементиране на calculation engine

---

**Автор**: GitHub Copilot  
**Дата**: 2026-02-04  
**Версия**: 1.0 (Фаза 1 - Базова интеграция)
