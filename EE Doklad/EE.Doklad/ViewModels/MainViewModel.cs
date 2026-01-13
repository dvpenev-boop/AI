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
        private Report _currentReport;

        [ObservableProperty]
        private Section? _selectedSection;

        public MainViewModel()
        {
            _storageService = new ReportStorageService();
            _pdfService = new PdfGeneratorService();
            
            // Създаваме примерен доклад при старт
            CurrentReport = CreateSampleReport();
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
            // Показваме диалог за вмъкване на секция с избор на позиция
            var dialog = new Views.InsertSectionDialog(CurrentReport.Sections.Count);
            
            if (dialog.ShowDialog() != true)
                return;

            Section newSection;

            // Ако има избрана секция с таблици, питаме дали да копираме формата
            if (SelectedSection != null && SelectedSection.Tables.Any())
            {
                var result = MessageBox.Show(
                    $"Желаете ли да копирате формата (таблици и структура) от секция '{SelectedSection.Title}'?",
                    "Копиране на формат",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Копираме формата на избраната секция
                    newSection = CopySectionFormat(SelectedSection);
                }
                else
                {
                    // Създаваме празна секция
                    newSection = new Section();
                }
            }
            else
            {
                // Създаваме празна секция
                newSection = new Section();
            }

            // Вмъкваме секцията на желаната позиция с автоматично преномериране
            InsertSection(dialog.SectionNumber, dialog.SectionName, newSection);
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

        private Report CreateSampleReport()
        {
            var report = new Report
            {
                Title = "Енергиен доклад 2026"
            };

            // Предефинирани 20 секции за енергиен доклад
            var sectionTitles = new[]
            {
                "1. Челна страница",
                "2. Удостоверения",
                "3. Съдържание",
                "4. Въведение",
                "5. Общи данни",
                "6. Външни стени",
                "7. Покрив",
                "8. Под",
                "9. Прозорци и врати",
                "10. Отопление",
                "11. Охлаждане",
                "12. Вентилация",
                "13. Помпи",
                "14. Топла вода за битови нужди (БГВ)",
                "15. Осветление",
                "16. Други разходи влияещи",
                "17. Други разходи не влияещи",
                "18. Резултати сграда",
                "19. Клас на енергопотребление",
                "20. Заключение"
            };

            for (int i = 0; i < sectionTitles.Length; i++)
            {
                var section = new Section
                {
                    Title = sectionTitles[i],
                    StaticText = $"Попълнете данните за секция: {sectionTitles[i]}",
                    Order = i
                };

                // Добавяме примерна таблица само за секция 5 (Общи данни)
                if (i == 4) // индекс 4 = "5. Общи данни"
                {
                    var table = new FixedTable
                    {
                        Title = "Основни данни за сградата",
                        ColumnHeaders = new() { "Показател", "Стойност", "Мерна единица" }
                    };

                    table.Rows.Add(new Row
                    {
                        Cells = new()
                        {
                            new Cell { Value = "Обект" },
                            new Cell { Value = "" },
                            new Cell { Value = "-" }
                        }
                    });
                    table.Rows.Add(new Row
                    {
                        Cells = new()
                        {
                            new Cell { Value = "Отопляема площ" },
                            new Cell { Value = "", Type = CellType.Number },
                            new Cell { Value = "кв.м" }
                        }
                    });
                    table.Rows.Add(new Row
                    {
                        Cells = new()
                        {
                            new Cell { Value = "Отопляем обем" },
                            new Cell { Value = "", Type = CellType.Number },
                            new Cell { Value = "куб.м" }
                        }
                    });

                    section.Tables.Add(table);
                }

                report.Sections.Add(section);
            }

            return report;
        }
    }
}
