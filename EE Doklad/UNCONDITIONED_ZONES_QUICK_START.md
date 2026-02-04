# Секция 10: Неклиматизирани зони (ztu) - Quick Start

## Статус: ✅ Базова интеграция завършена

Новата секция е успешно добавена между "9. Прозорци и врати" и "11. Отопление".

## Какво е направено (Фаза 1)

✅ **Models**: Създадени domain models за ztu зони, елементи и слоеве  
✅ **Integration**: Добавено `SectionType.UnconditionedZones` и property в `Section`  
✅ **UI**: Placeholder view с информация за функционалността  
✅ **Renumbering**: Всички следващи секции преномерирани визуално (10→11, ..., 20→21)  
✅ **Build**: Успешна компилация и стартиране на приложението  

## Как да използвате

1. Стартирайте приложението
2. В лявото меню изберете "**10. Неклиматизирани зони (ztu)**"
3. Вижте placeholder UI с описание на функционалността

## Следващи стъпки (Фаза 2-4)

🔨 **UI & UX** - Пълен потребителски интерфейс с таблици и диалози  
🔨 **ViewModel** - Команди за добавяне/редактиране на зони и елементи  
🔨 **Calculator** - Месечни изчисления (Hztu,e, bztu, θztu)  
🔨 **Integration** - Включване в heat transfer calculations на сградата  
🔨 **Tests** - Unit tests за изчисления и интеграция  

## Документация

📄 **Техническа**: [`UNCONDITIONED_ZONES_IMPLEMENTATION.md`](UNCONDITIONED_ZONES_IMPLEMENTATION.md)  
📘 **Потребителска**: [`USER_GUIDE_UNCONDITIONED_ZONES.md`](USER_GUIDE_UNCONDITIONED_ZONES.md)  

## Ключови файлове

### Нови (3)
- `Models/UnconditionedZoneModels.cs`
- `Views/UnconditionedZonesSectionView.xaml`
- `Views/UnconditionedZonesSectionView.xaml.cs`

### Модифицирани (3)
- `Models/Section.cs`
- `ViewModels/MainViewModel.cs`
- `MainWindow.xaml.cs`

## Архитектура

```
UnconditionedZoneSectionData
  └─ Zones: ObservableCollection<ZtuZone>
       ├─ Name: string
       ├─ Type: ZtuType (External/Internal)
       ├─ ElementsToExternal: ObservableCollection<ZtuElement>
       └─ ElementsToBoundary: ObservableCollection<ZtuElement>
            ├─ Name: string
            ├─ Kind: ElementKind (Wall/Roof/Floor)
            ├─ Area: double
            ├─ Layers: ObservableCollection<ZtuLayer>
            └─ UValue: double
```

## Формули (за имплементация)

```
Hztu,e,m = Σ(Uk,m * Ak)                    // Към външна среда
Hztc-ztu,m = Σ(Uk,m * Ak)                  // Разделящи елементи
bztu,m = Hztu,e,m / (Hztu,e,m + Hztc-ztu,m) // Редукционен фактор
θztu,m = θe,a,m + bztu,m * (θint - θe,a,m)  // Температура в ztu

Влияние върху Htr:
  - ztue: Hel = bztu * U * A
  - ztui: Hel = (1 - bztu) * U * A
```

## Тестване

```powershell
cd "e:\AI\EE Doklad"
dotnet build EE.Doklad/EE.Doklad.csproj
dotnet run --project EE.Doklad
```

## Backwards Compatibility

✅ Стари проекти се зареждат без проблем (nullable property)  
✅ Секциите запазват стабилни SectionType keys  
✅ Визуалното преномериране е само в Title  

---

**Автор**: GitHub Copilot  
**Дата**: 2026-02-04  
**Версия**: 1.0 (Фаза 1)
