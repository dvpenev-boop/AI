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
            var newSection = new Section
            {
                Title = $"Секция {CurrentReport.Sections.Count + 1}",
                Order = CurrentReport.Sections.Count
            };
            CurrentReport.Sections.Add(newSection);
            SelectedSection = newSection;
            CurrentReport.IsDirty = true;
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
