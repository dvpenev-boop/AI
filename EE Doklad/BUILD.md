# Build и Deployment инструкции

## Вариант 1: Debug (за разработка)

```powershell
cd "E:\AI\EE Doklad\EE.Doklad"
dotnet build -c Debug
dotnet run
```

## Вариант 2: Release build

```powershell
cd "E:\AI\EE Doklad\EE.Doklad"
dotnet build -c Release
```

Изпълним файл: `bin\Release\net8.0-windows\EE.Doklad.exe`

## Вариант 3: Single-file executable (препоръчително за дистрибуция)

### Framework-dependent (изисква .NET 8 Runtime)
```powershell
cd "E:\AI\EE Doklad\EE.Doklad"
dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true
```

Файл: `bin\Release\net8.0-windows\win-x64\publish\EE.Doklad.exe` (~15 MB)

**Изисква:** .NET 8 Desktop Runtime на целевата машина

### Self-contained (не изисква .NET Runtime)
```powershell
cd "E:\AI\EE Doklad\EE.Doklad"
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
```

Файл: `bin\Release\net8.0-windows\win-x64\publish\EE.Doklad.exe` (~80-100 MB)

**Предимства:**
- Не изисква инсталиран .NET Runtime
- Работи на всяка Windows 10/11 машина (x64)
- Всички dependencies са вградени

## Вариант 4: Trimmed single-file (по-малък размер)

```powershell
cd "E:\AI\EE Doklad\EE.Doklad"
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true /p:IncludeNativeLibrariesForSelfExtract=true
```

Файл: ~40-60 MB (trimmed unused code)

⚠️ **Внимание:** Trimming може да премахне необходим reflection code. Тествайте задълбочено!

## Вариант 5: ReadyToRun (по-бърз старт)

```powershell
cd "E:\AI\EE Doklad\EE.Doklad"
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishReadyToRun=true /p:IncludeNativeLibrariesForSelfExtract=true
```

**Предимства:** Приложението стартира по-бързо (pre-compiled)

## Инсталация на целева машина

### Framework-dependent версия
1. Инсталирайте [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Копирайте `EE.Doklad.exe` на машината
3. Двоен клик за стартиране

### Self-contained версия
1. Копирайте `EE.Doklad.exe` на машината
2. Двоен клик за стартиране (не изисква нищо друго!)

## Разпространение

### Zip архив
```powershell
# След publish команда
cd "E:\AI\EE Doklad\EE.Doklad\bin\Release\net8.0-windows\win-x64\publish"
Compress-Archive -Path "EE.Doklad.exe" -DestinationPath "EE-Doklad-v1.0-win-x64.zip"
```

### Installer (по желание)
- Използвайте WiX Toolset или Inno Setup
- Създава Start Menu shortcuts
- Добавя Uninstaller в Control Panel

## Тестване на различни машини

След publish, тествайте на:
- [ ] Windows 10 (чиста инсталация без .NET)
- [ ] Windows 11
- [ ] Машина с различни PDF viewer-и (Adobe, Edge, Chrome)
- [ ] Машина без Arial шрифт (QuestPDF трябва да използва вградени шрифтове)

## Embedding на ресурси (бъдещо)

Ако искате да вградите PDF шаблон или custom шрифтове:

1. Добавете в .csproj:
```xml
<ItemGroup>
  <EmbeddedResource Include="Resources\**\*" />
</ItemGroup>
```

2. Зареждайте от Assembly:
```csharp
using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EE.Doklad.Resources.template.pdf"))
{
    // use stream
}
```

---

**Препоръчвам Вариант 3 (self-contained) за дистрибуция към потребители.**
