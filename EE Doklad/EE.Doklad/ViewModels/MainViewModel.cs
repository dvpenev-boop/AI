using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EE.Doklad.Models;
using EE.Doklad.Services;
using Microsoft.Win32;

namespace EE.Doklad.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ReportStorageService _storageService;
        private readonly PdfGeneratorService _pdfService;

        [ObservableProperty]
        private int _currentClimateZone = 1;

        [ObservableProperty]
        private Report _currentReport;

        [ObservableProperty]
        private Section? _selectedSection;

        public MainViewModel()
        {
            _storageService = new ReportStorageService();
            _pdfService = new PdfGeneratorService();

            // Създаваме примерен доклад при старт
            CurrentReport = CreateSampleReport();

            // Закачаме обработчик за промяна на климатичната зона
            AttachClimateZoneHandler();

            // Initial sync
            TrySyncCurrentClimateZone();
        }

        private void TrySyncCurrentClimateZone()
        {
            var objectSection = CurrentReport?.Sections?.FirstOrDefault(s => s.Type == SectionType.ObjectData);
            if (objectSection?.ObjectDataSectionData != null)
            {
                CurrentClimateZone = objectSection.ObjectDataSectionData.ClimateZone;
            }
        }

        private void AttachClimateZoneHandler()
        {
            // Намираме секцията "Данни за обекта"
            var objectSection = CurrentReport?.Sections?.FirstOrDefault(s => s.Type == SectionType.ObjectData);
            if (objectSection?.ObjectDataSectionData != null)
            {
                objectSection.ObjectDataSectionData.PropertyChanged += ObjectDataSectionData_PropertyChanged;
            }
        }

        private void ObjectDataSectionData_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EE.Doklad.Models.ObjectDataSectionData.ClimateZone))
            {
                if (CurrentReport?.Sections == null)
                    return;

                var objectSection = CurrentReport.Sections.FirstOrDefault(s => s.Type == SectionType.ObjectData);
                int climateZone = 1;
                if (objectSection?.ObjectDataSectionData != null)
                {
                    climateZone = objectSection.ObjectDataSectionData.ClimateZone;
                }

                CurrentClimateZone = climateZone;
                // Обновяваме Te за всички студени покриви във всички RoofSectionData секции
                foreach (var roofSection in CurrentReport.Sections)
                {
                    var data = roofSection.RoofSectionData;
                    if (data?.RoofTypes == null) continue;
                    foreach (var roofType in data.RoofTypes)
                    {
                        if (roofType.Mode == EE.Doklad.Models.RoofMode.Cold && roofType.ColdDetail != null)
                        {
                            if (!roofType.ColdDetail.ManualTeInput)
                            {
                                roofType.ColdDetail.Te = EE.Doklad.ViewModels.RoofSectionViewModel.GetTeForClimateZone(climateZone);
                            }
                        }
                    }
                }
            }
        }

        [RelayCommand]
        private async Task NewReport()
        {
            if (CurrentReport?.IsDirty == true)
            {
                var result = MessageBox.Show("Имате незапазени промени. Желаете ли да продължите?", 
                    "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes) return;
            }

            CurrentReport = CreateSampleReport();
            SelectedSection = CurrentReport.Sections.FirstOrDefault();
        }

        [RelayCommand]
        private async Task OpenReport()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "JSON файлове (*.json)|*.json|Всички файлове (*.*)|*.*",
                DefaultExt = ".json"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var report = await _storageService.LoadFromFileAsync(dialog.FileName);
                    if (report != null)
                    {
                        CurrentReport = report;
                        SelectedSection = CurrentReport.Sections.FirstOrDefault();
                        MessageBox.Show("Докладът е зареден успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Грешка при зареждане: {ex.Message}", "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private async Task SaveReport()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "JSON файлове (*.json)|*.json",
                DefaultExt = ".json",
                FileName = $"{CurrentReport.Title}.json"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    await _storageService.SaveToFileAsync(CurrentReport, dialog.FileName);
                    MessageBox.Show("Докладът е запазен успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Грешка при запазване: {ex.Message}", "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void ExportToPdf()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "PDF файлове (*.pdf)|*.pdf",
                DefaultExt = ".pdf",
                FileName = $"{CurrentReport.Title}.pdf"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _pdfService.GeneratePdf(CurrentReport, dialog.FileName);
                    MessageBox.Show("PDF файлът е генериран успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Отваряме PDF-а
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = dialog.FileName,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Грешка при генериране на PDF: {ex.Message}", "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void AddSection()
        {
            int nextNumber = CurrentReport.Sections.Count + 1;
            int maxNumber = CurrentReport.Sections.Count;
            
            var dialog = new Views.InsertSectionDialog(nextNumber, maxNumber);
            
            if (dialog.ShowDialog() != true)
                return;

            // Вземаме данните от диалога
            int desiredNumber = dialog.SectionNumber;
            string title = dialog.SectionName;

            // Защита: не позволяваме вмъкване преди системните секции (1-5)
            if (desiredNumber < 6)
            {
                MessageBox.Show(
                    "Не може да вмъквате секции преди системните секции (Челна страница, Удостоверения, Съдържание, Въведение, Данни за обекта).\nМоля изберете номер 6 или по-голям.",
                    "Невалидна позиция",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Проверяваме дали има избрана секция с таблици
            if (SelectedSection != null && SelectedSection.Tables.Any())
            {
                var result = MessageBox.Show(
                    $"Искате ли да копирате структурата (таблици и колони) от текущата секция?\n\n" +
                    $"Текуща секция: {SelectedSection.Title}\n" +
                    $"Нова секция: {desiredNumber}. {title}",
                    "Копиране на структура",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Копираме структурата на текущата секция
                    var newSection = CopySectionFormat(SelectedSection);
                    InsertSection(desiredNumber, title, newSection);
                }
                else
                {
                    // Създаваме празна секция
                    var newSection = new Section { Type = SectionType.Normal };
                    InsertSection(desiredNumber, title, newSection);
                }
            }
            else
            {
                // Няма избрана секция или няма таблици - създаваме празна секция
                var newSection = new Section { Type = SectionType.Normal };
                InsertSection(desiredNumber, title, newSection);
            }

            CurrentReport.IsDirty = true;
        }

        [RelayCommand]
        private void DeleteSection()
        {
            if (SelectedSection == null)
            {
                MessageBox.Show("Моля, изберете секция за изтриване.", 
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Защита: системните секции не могат да се изтрият
            if (SelectedSection.IsSystemSection)
            {
                MessageBox.Show(
                    "Системните секции (Челна страница и Удостоверения) не могат да се изтриват.\nТе са задължителна част от документа.",
                    "Защитена секция",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Потвърждение за изтриване
            var result = MessageBox.Show(
                $"Сигурни ли сте, че искате да изтриете секцията?\n\n{SelectedSection.Title}",
                "Потвърждение за изтриване",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            // Запазваме индекса на секцията, която ще изтрием
            int deletedIndex = CurrentReport.Sections.IndexOf(SelectedSection);

            // Премахваме секцията
            CurrentReport.Sections.Remove(SelectedSection);

            // Преномерираме всички останали секции
            RenumberSections();

            // Селектираме следващата секция (или предишната, ако сме изтрили последната)
            if (CurrentReport.Sections.Any())
            {
                if (deletedIndex < CurrentReport.Sections.Count)
                {
                    SelectedSection = CurrentReport.Sections[deletedIndex];
                }
                else
                {
                    SelectedSection = CurrentReport.Sections.Last();
                }
            }
            else
            {
                SelectedSection = null;
            }

            CurrentReport.IsDirty = true;
        }

        /// <summary>
        /// Преномерира всички секции последователно от 1 нагоре
        /// </summary>
        private void RenumberSections()
        {
            for (int i = 0; i < CurrentReport.Sections.Count; i++)
            {
                var section = CurrentReport.Sections[i];
                section.Order = i; // Order е 0-based

                // Извличаме заглавието без номер
                var titleWithoutNumber = System.Text.RegularExpressions.Regex.Replace(
                    section.Title, @"^\d+\.\s*", "");
                
                // Обновяваме Title с новия номер (1-based)
                section.Title = $"{i + 1}. {titleWithoutNumber}";
            }
        }

        /// <summary>
        /// Вмъква нова секция на конкретна позиция (номер) и преномерира всички секции след нея
        /// </summary>
        /// <param name="number">Желан номер на новата секция (1-based)</param>
        /// <param name="title">Заглавие на секцията (без номер)</param>
        /// <param name="section">Секцията за вмъкване (може да е с копирана структура)</param>
        private void InsertSection(int number, string title, Section section)
        {
            // Валидация
            if (number < 1 || number > CurrentReport.Sections.Count + 1)
            {
                MessageBox.Show($"Невалиден номер на секция: {number}", 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Задаваме заглавието и Order
            section.Title = $"{number}. {title}";
            section.Order = number - 1; // Order е 0-based индекс

            // Преномерираме всички секции от тази позиция нататък (изместваме с +1)
            for (int i = 0; i < CurrentReport.Sections.Count; i++)
            {
                if (CurrentReport.Sections[i].Order >= section.Order)
                {
                    CurrentReport.Sections[i].Order++;
                    
                    // Обновяваме Title с новия номер
                    var oldTitle = CurrentReport.Sections[i].Title;
                    var titleWithoutNumber = System.Text.RegularExpressions.Regex.Replace(
                        oldTitle, @"^\d+\.\s*", "");
                    CurrentReport.Sections[i].Title = $"{CurrentReport.Sections[i].Order + 1}. {titleWithoutNumber}";
                }
            }

            // Вмъкваме новата секция на правилната позиция в ObservableCollection
            CurrentReport.Sections.Insert(section.Order, section);

            // Сортираме колекцията по Order (за да сме сигурни че е правилно подредена)
            var sortedSections = CurrentReport.Sections.OrderBy(s => s.Order).ToList();
            CurrentReport.Sections.Clear();
            foreach (var s in sortedSections)
            {
                CurrentReport.Sections.Add(s);
            }

            // Селектираме новата секция автоматично
            SelectedSection = section;
            CurrentReport.IsDirty = true;
        }

        /// <summary>
        /// Копира формата на секция (таблици, колони, структура) без да копира данните
        /// </summary>
        private Section CopySectionFormat(Section sourceSection)
        {
            var newSection = new Section
            {
                StaticText = sourceSection.StaticText
            };

            // Копираме всички таблици от източника
            foreach (var sourceTable in sourceSection.Tables)
            {
                Table newTable;
                
                // Създаваме същия тип таблица (Fixed или Dynamic)
                if (sourceTable.IsDynamic)
                {
                    newTable = new DynamicTable();
                }
                else
                {
                    newTable = new FixedTable();
                }

                // Копираме заглавието и колоните
                newTable.Title = sourceTable.Title;
                newTable.ColumnHeaders = new List<string>(sourceTable.ColumnHeaders);

                // Копираме структурата на редовете (без данни)
                foreach (var sourceRow in sourceTable.Rows)
                {
                    var newRow = new Row();
                    foreach (var sourceCell in sourceRow.Cells)
                    {
                        newRow.Cells.Add(new Cell
                        {
                            // Копираме типа на клетката и етикета, но не и стойността
                            Type = sourceCell.Type,
                            Value = sourceCell.Type == CellType.Text && !string.IsNullOrEmpty(sourceCell.Value) && IsLabel(sourceCell.Value) 
                                ? sourceCell.Value  // Запазваме етикети (напр. "Показател", "Мерна единица")
                                : "" // Изчистваме данни
                        });
                    }
                    newTable.Rows.Add(newRow);
                }

                newSection.Tables.Add(newTable);
            }

            return newSection;
        }

        /// <summary>
        /// Определя дали стойността е етикет (неизменяемо име) или данни (които да се изчистят)
        /// </summary>
        private bool IsLabel(string value)
        {
            // Етикетите обикновено са в първата колона или са измерни единици
            var labels = new[] { "Показател", "Стойност", "Мерна единица", "Обект", 
                "Отопляема площ", "Отопляем обем", "кв.м", "куб.м", "м", "бр.", "%", "W", "kW" };
            
            return labels.Any(label => value.Contains(label, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Създава статичния текст за секция 4 "Въведение"
        /// </summary>
        private string CreateIntroductionStaticText()
        {
            return @"УСТАНОВЯВАНЕ НА СЪОТВЕТСТВИЕТО НА ИНВЕСТИЦИОННИЯТ ПРОЕКТ С ИЗИСКВАНИЯТА ЗА ЕНЕРГИЙНА ЕФЕКТИВНОСТ, ИЗЧИСЛЯВАНЕ НА ПОКАЗАТЕЛИТЕ ЗА РАЗХОД НА ЕНЕРГИЯ СЪГЛАСНО Наредба № РД-02-20-3 от 9 ноември 2022г. за техническите изисквания към енергийните характеристики на сгради

За целта е извършено моделното изследване на енергопотреблението в сградата на основата на метода, разработен по EN ISO 52000-1; 52003-1;
52010-1;52016-1; 52018-1 , реализирано програмно като софтуерен продукт за изчисляване на енергийни характеристики на сгради, предоставен от
Агенцията по устойчиво енергийно развитие.
Целта е получаване на необходимата енергия за поддържане на микроклимата в сградата и сравнението и с еталонния разход на енергия за сградата
по проектни данни.
За целите на определянето на енергийните им характеристики сградите се разглеждат като интегрирани системи, в които разходът на енергия е
резултат на съвместното влияние на основните компоненти:
• Сградните ограждащи конструкции и елементи
• Системите за поддържане на микроклимата
• Вътрешните източници на топлина
• Обитателите
• Климатичните условия
Създаването на модел на такава интегрирана система изисква зониране и специфично описание на параметрите на извършващите се в зоната
топлообменни процеси. В случая е подходящо разглеждане на сградата като една топлинна зона.
Националната методология за изчисляване на интегрираната енергийна характеристика включва задължително:
• Ориентация, размерите , формата на сградата
• Топлинните и оптически характеристики, въздухопропускливостта, влагоустойчивостта, водонепропускливостта на сградните ограждащи
конструкции, елементи и вътрешни пространства
• Системи за отопление и гореща вода за битови нужди;
• Системи за климатизация
• Системи за вентилация
• Естествената вентилация
• Външни и вътрешни климатични условия";
        }

        /// <summary>
        /// Създава редовете на таблицата за секция 4 "Въведение" с данни от снимка 2
        /// </summary>
        private List<Row> CreateIntroductionTableRows()
        {
            var rows = new List<Row>();

            // Данни от новото изображение, включително липсващия ред
            var tableData = new[]
            {
                new[] { "Обяснителна записка", "Съдържа описание на основното за изготвяне на част \"Енергийна ефективност\"" },
                new[] { "Определение на коефициентите на топлопроводимост на ограждащите елементи и конструкции", "Стойностите не надвишават максимално допустимите, определени в таблица №1, чл. 23, ал. 1 и таблица №4, чл. 10, ал. 1 от Наредба № РД-02-20-3 от 9 ноември 2022г. за техническите изисквания към енергийните характеристики на сградите" },
                new[] { "Определение на коефициентите на топлинни загуби от топлопреминаване, Ht", "Изчислени съгласно Наредба № РД-02-20-3 от 9 ноември 2022г. за техническите изисквания към енергийните характеристики на сградите" },
                new[] { "Определение на коефициентите на топлинни загуби от вентилация", "Изчислени съгласно Наредба № РД-02-20-3 от 9 ноември 2022г. за техническите изисквания към енергийните характеристики на сградите" },
                new[] { "Определение на топлинните печалби от вътрешни топлинни източници", "Изчислени съгласно Наредба № РД-02-20-3 от 9 ноември 2022г. за техническите изисквания към енергийните характеристики на сградите" },
                new[] { "Определение на топлинните печалби от слънчево греене", "Изчислени съгласно Наредба № РД-02-20-3 от 9 ноември 2022г. за техническите изисквания към енергийните характеристики на сградите" },
                new[] { "Определение на годишната потребна топлина за отопление на сградата Qh", "Изчислени съгласно Наредба № РД-02-20-3 от 9 ноември 2022г. за техническите изисквания към енергийните характеристики на сградите" },
                new[] { "Определение на специфичната стойност на годишната потребна топлина за отопление на единица квадратен метър отопляема площ", "Изчислени съгласно Наредба № РД-02-20-3 от 9 ноември 2022г. за техническите изисквания към енергийните характеристики на сградите" },
                new[] { "Сравнение на специфичната стойност на годишната потребна топлина за отопление на единица квадратен метър отопляема площ", "Стойността на специфичната потребна топлина за сградата не надвишава максималната нормативна стойност" }
            };

            foreach (var rowData in tableData)
            {
                var row = new Row();
                foreach (var cellValue in rowData)
                {
                    row.Cells.Add(new Cell
                    {
                        Type = CellType.Text,
                        Value = cellValue
                    });
                }
                rows.Add(row);
            }

            return rows;
        }

        private Report CreateSampleReport()
        {
            var report = new Report
            {
                Title = "Енергиен доклад 2026"
            };

            // Първа секция винаги е Челна страница (CoverPage)
            var coverPage = new Section
            {
                Type = SectionType.CoverPage,
                Title = "1. Челна страница",
                Order = 0,
                CoverPageData = new CoverPageData
                {
                    CompanyName = "Примерна фирма ООД",
                    LicenseNumber = "№ 1234/2025",
                    ObjectName = "Жилищна сграда",
                    ObjectAddress = "гр. София, ул. Примерна 1",
                    Phase = ProjectPhase.Tehnicheski,
                    ManagerName = "Иван Иванов"
                }
            };
            report.Sections.Add(coverPage);

            // Втора секция: Удостоверения (системна секция №2)
            var certificates = new Section
            {
                Type = SectionType.Certificates,
                Title = "2. Удостоверения",
                Order = 1,
                CertificatesData = new CertificatesSectionData()
            };
            report.Sections.Add(certificates);

            // Секция 3: Съдържание
            var contentsSection = new Section
            {
                Type = SectionType.Normal,
                Title = "3. Съдържание",
                StaticText = "Попълнете данните за Съдържание",
                Order = 2  // +2 защото CoverPage=0, Certificates=1
            };
            report.Sections.Add(contentsSection);

            // Секция 4: Въведение с предварително попълнена таблица и текст
            var introSection = new Section
            {
                Type = SectionType.Normal,
                Title = "4. Въведение",
                Order = 3,
                StaticText = CreateIntroductionStaticText()
            };

            // Добавяме таблицата с проверени елементи
            var introTable = new FixedTable
            {
                Title = "Проверени елементи в проекта",
                ColumnHeaders = new List<string> { "Проверени елементи в проекта", "Констатации" }
            };

            // Добавяме редовете с предварително попълнени данни от снимка 2
            var introRows = CreateIntroductionTableRows();
            foreach (var row in introRows)
            {
                introTable.Rows.Add(row);
            }

            introSection.Tables.Add(introTable);
            report.Sections.Add(introSection);

            // Пета секция: Данни за обекта (системна секция №5)
            var objectData = new Section
            {
                Type = SectionType.ObjectData,
                Title = "5. Данни за обекта",
                Order = 4,
                ObjectDataSectionData = new ObjectDataSectionData
                {
                    Title = "Данни за обекта",
                    BuildingName = "Жилищна сграда",
                    Address = "гр. София, ул. Примерна 1",
                    BuildingType = "Жилищна сграда",
                    Ownership = "Частна собственост",
                    YearOfConstruction = "–",
                    NumberOfOccupants = null,
                    // Leave schedules and area/volume fields empty by default per user request
                    OccupancySchedule = null,
                    HeatingSchedule = null,
                    BuiltUpArea = null,
                    TotalFloorArea = null,
                    HeatedArea = null,
                    NetHeatedVolume = null,
                    GrossHeatedVolume = null,
                    CooledArea = null,
                    NetCooledVolume = null,
                    GrossCooledVolume = null
                }
            };
            report.Sections.Add(objectData);

            // Останалите секции (нормални секции от №6 нагоре)
            var laterTitles = new[]
            {
                "6. Външни стени",
                "7. Покрив",
                "8. Под",
                "9. Прозорци и врати",
                "10. Неклиматизирани зони (ztu)",
                "11. Отопление",
                "12. Охлаждане",
                "13. Вентилация Отопление",
                "14. Вентилация Охлаждане",
                "15. Помпи",
                "16. Топла вода за битови нужди (БГВ)",
                "17. Осветление",
                "18. Други разходи влияещи",
                "19. Други разходи не влияещи",
                "20. Резултати сграда",
                "21. Клас на енергопотребление",
                "22. Заключение"
            };

            for (int i = 0; i < laterTitles.Length; i++)
            {
                var section = new Section
                {
                    Type = SectionType.Normal,
                    Title = laterTitles[i],
                    StaticText = $"Попълнете данните за секция: {laterTitles[i]}",
                    Order = i + 5  // +5 защото CoverPage=0, Certificates=1, ..., ObjectData=4
                };

                if (laterTitles[i].Contains("Външни стени"))
                {
                    var externalWallsData = new ExternalWallsSectionData
                    {
                        Title = "Външни стени",
                        Description = "Попълнете данните за външните стени."
                    };

                    section.Type = SectionType.ExternalWalls;
                    section.ExternalWallsSectionData = externalWallsData;
                    section.StaticText = string.Empty;
                }
                else if (laterTitles[i].Contains("Покрив"))
                {
                    section.Type = SectionType.Roof;
                    section.RoofSectionData = new RoofSectionData();
                    section.StaticText = string.Empty;
                }
                else if (laterTitles[i].Contains("Под"))
                {
                    section.Type = SectionType.Floor;
                    section.FloorSectionData = new FloorSectionData();
                    section.StaticText = string.Empty;
                }
                else if (laterTitles[i].Contains("10. Неклиматизирани зони"))
                {
                    section.Type = SectionType.UnconditionedZones;
                    section.UnconditionedZoneSectionData = new UnconditionedZoneSectionData
                    {
                        Title = "Неклиматизирани зони (ztu)",
                        Description = "Попълнете данните за неклиматизирани зони."
                    };
                    section.StaticText = string.Empty;
                }
                else if (laterTitles[i].Contains("11. Отопление"))
                {
                    section.Type = SectionType.Heating;
                    section.HeatingSectionData = new HeatingSectionData
                    {
                        Title = "Отопление",
                        Description = "Попълнете данните за отопление."
                    };
                    section.StaticText = string.Empty;
                }
                else if (laterTitles[i].Contains("13. Вентилация Отопление"))
                {
                    section.Type = SectionType.Ventilation;
                    section.VentilationSectionData = new VentilationSectionData
                    {
                        Title = "Вентилация Отопление",
                        Description = "Попълнете данните за секция: 13. Вентилация Отопление"
                    };
                    section.StaticText = string.Empty;
                }
                else if (laterTitles[i].Contains("14. Вентилация Охлаждане"))
                {
                    // New section: Ventilation (cooling flavour) - same UI as Ventilation Отопление but initially standalone
                    section.Type = SectionType.Ventilation;
                    section.VentilationSectionData = new VentilationSectionData
                    {
                        Title = "Вентилация Охлаждане",
                        Description = "Попълнете данните за секция: 14. Вентилация Охлаждане"
                    };
                    section.StaticText = string.Empty;
                }
                // Помпи: няма специален SectionType, използваме нормална секция
                else if (laterTitles[i].Contains("16. Топла вода за битови нужди"))
                {
                    section.Type = SectionType.HotWater;
                    section.HotWaterSectionData = new HotWaterSectionData
                    {
                        Title = "Топла вода за битови нужди (БГВ)",
                        Description = "Попълнете данните за секция: 16. Топла вода за битови нужди (БГВ)"
                    };
                    section.StaticText = string.Empty;
                }
                else if (laterTitles[i].Contains("17. Осветление"))
                {
                    section.Type = SectionType.Lighting;
                    section.LightingSectionData = new LightingSectionData
                    {
                        Title = "Осветление",
                        Description = "Попълнете данните за осветление."
                    };
                    section.StaticText = string.Empty;
                }
                else if (laterTitles[i].Contains("18. Други разходи влияещи"))
                {
                    section.Type = SectionType.AppliancesAffecting;
                    section.AppliancesAffectingSectionData = new AppliancesSectionData
                    {
                        Title = "Други разходи влияещи",
                        Description = "Попълнете данните за уреди, които влияят на енергопотреблението."
                    };
                    section.StaticText = string.Empty;
                }
                else if (laterTitles[i].Contains("19. Други разходи не влияещи"))
                {
                    section.Type = SectionType.AppliancesNotAffecting;
                    section.AppliancesNotAffectingSectionData = new AppliancesSectionData
                    {
                        Title = "Други разходи не влияещи",
                        Description = "Попълнете данните за уреди, които не влияят на енергопотреблението."
                    };
                    section.StaticText = string.Empty;
                }
                else if (laterTitles[i].Contains("20. Резултати сграда"))
                {
                    section.Type = SectionType.Results;
                    section.ResultsSectionData = new ResultsSectionData
                    {
                        Title = "Резултати сграда",
                        Description = "Таблица с изчисления на потребена енергия по енергоносители."
                    };
                    section.StaticText = string.Empty;
                }
                else if (laterTitles[i].Contains("21. Клас на енергопотребление"))
                {
                    section.Type = SectionType.EnergyClass;
                    section.EnergyClassSectionData = new EnergyClassSectionData
                    {
                        Title = "Клас на енергопотребление",
                        Description = "Автоматично определяне на енергиен клас според типа сграда и EP.",
                        EnergyPerformance = null // Потребителят ще попълни
                    };
                    section.StaticText = string.Empty;
                }
                else if (laterTitles[i].Contains("22. Заключение"))
                {
                    section.Type = SectionType.Conclusion;
                    section.ConclusionSectionData = new ConclusionSectionData
                    {
                        Title = "Заключение",
                        Description = "Заключителна част на доклада с предварително попълнен текст, който може да се редактира."
                    };
                    section.StaticText = string.Empty;
                }

                report.Sections.Add(section);
            }

            return report;
        }
    }
}
