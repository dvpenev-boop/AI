using System;
using System.IO;
using System.Linq;
using EE.Doklad.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EE.Doklad.Services
{
    /// <summary>
    /// Сервиз за генериране на PDF доклади с фиксиран layout
    /// </summary>
    public class PdfGeneratorService
    {
        public PdfGeneratorService()
        {
            // QuestPDF лиценз (Community за некомерсиална употреба)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public void GeneratePdf(Report report, string outputPath)
        {
            try
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                        page.Header()
                            .Text(report.Title)
                            .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(column =>
                            {
                                foreach (var section in report.Sections.OrderBy(s => s.Order))
                                {
                                    // Ако е Челна страница, рендерираме специален layout
                                    if (section.Type == SectionType.CoverPage && section.CoverPageData != null)
                                    {
                                        GenerateCoverPage(column, section.CoverPageData);
                                        column.Item().PageBreak(); // Нова страница след челната
                                        continue;
                                    }

                                    // Заглавие на секцията (само за Normal секции)
                                    column.Item().Text(section.Title).Bold().FontSize(14);
                                    column.Item().PaddingBottom(5);

                                    // Статичен текст
                                    if (!string.IsNullOrWhiteSpace(section.StaticText))
                                    {
                                        column.Item().Text(section.StaticText);
                                        column.Item().PaddingBottom(10);
                                    }

                                    // Таблици
                                    foreach (var table in section.Tables)
                                    {
                                        column.Item().Text(table.Title).SemiBold().FontSize(12);
                                        column.Item().PaddingBottom(3);

                                        column.Item().Table(tbl =>
                                        {
                                            // Дефиниране на колони
                                            tbl.ColumnsDefinition(columns =>
                                            {
                                                foreach (var _ in table.ColumnHeaders)
                                                {
                                                    columns.RelativeColumn();
                                                }
                                            });

                                            // Header
                                            foreach (var header in table.ColumnHeaders)
                                            {
                                                tbl.Cell().Border(1).Background(Colors.Grey.Lighten3)
                                                    .Padding(5).Text(header).Bold();
                                            }

                                            // Редове
                                            foreach (var row in table.Rows)
                                            {
                                                foreach (var cell in row.Cells)
                                                {
                                                    tbl.Cell().Border(1).Padding(5).Text(cell.Value);
                                                }
                                            }
                                        });

                                        column.Item().PaddingBottom(15);
                                    }
                                }
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Страница ");
                                x.CurrentPageNumber();
                                x.Span(" от ");
                                x.TotalPages();
                            });
                    });
                });

                document.GeneratePdf(outputPath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Грешка при генериране на PDF: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Генерира челна страница на доклада
        /// </summary>
        private void GenerateCoverPage(ColumnDescriptor column, CoverPageData data)
        {
            // Заглавие
            column.Item()
                .AlignCenter()
                .PaddingBottom(20)
                .Text("ЕНЕРГИЕН ДОКЛАД")
                .Bold()
                .FontSize(24);

            // Лого (ако има)
            if (!string.IsNullOrEmpty(data.LogoPath) && File.Exists(data.LogoPath))
            {
                try
                {
                    column.Item()
                        .AlignCenter()
                        .PaddingBottom(20)
                        .Width(150)
                        .Image(data.LogoPath);
                }
                catch
                {
                    // Ако грешка при зареждане на лого, показваме placeholder
                    column.Item()
                        .AlignCenter()
                        .PaddingBottom(20)
                        .Width(150)
                        .Height(100)
                        .Border(2)
                        .BorderColor(Colors.Grey.Medium)
                        .AlignMiddle()
                        .AlignCenter()
                        .Text("(Лого)")
                        .FontColor(Colors.Grey.Medium);
                }
            }
            else
            {
                // Placeholder за лого
                column.Item()
                    .AlignCenter()
                    .PaddingBottom(20)
                    .Width(150)
                    .Height(100)
                    .Border(2)
                    .BorderColor(Colors.Grey.Medium)
                    .AlignMiddle()
                    .AlignCenter()
                    .Text("(Лого)")
                    .FontColor(Colors.Grey.Medium);
            }

            // Данни за фирмата
            column.Item().PaddingBottom(10).Column(col =>
            {
                col.Item().Text("ФИРМА").Bold().FontSize(12);
                col.Item().Text(data.CompanyName).FontSize(14);
                col.Item().Text(data.LicenseNumber).FontSize(12).FontColor(Colors.Grey.Darken1);
            });

            // Данни за обекта
            column.Item().PaddingBottom(10).Column(col =>
            {
                col.Item().Text("ОБЕКТ").Bold().FontSize(12);
                col.Item().Text(data.ObjectName).FontSize(14);
                col.Item().Text(data.ObjectAddress).FontSize(12);
            });

            // Фаза на проектиране
            column.Item().PaddingBottom(10).Row(row =>
            {
                row.RelativeItem().Text("ФАЗА: ").Bold().FontSize(12);
                row.RelativeItem().Text(GetPhaseText(data.Phase)).FontSize(12);
            });

            // Управител
            if (!string.IsNullOrEmpty(data.ManagerName))
            {
                column.Item().PaddingTop(20).PaddingBottom(10).Column(col =>
                {
                    col.Item().Text("УПРАВИТЕЛ").Bold().FontSize(12);
                    col.Item().Text(data.ManagerName).FontSize(12);
                    col.Item().PaddingTop(10).BorderTop(1).Width(200).Text("(подпис и печат)").FontSize(9).FontColor(Colors.Grey.Medium);
                });
            }

            // Разработили
            if (data.Developers.Any())
            {
                column.Item().PaddingTop(20).Column(col =>
                {
                    col.Item().Text("РАЗРАБОТИЛ ЕКИП").Bold().FontSize(12);
                    col.Item().PaddingTop(5).Table(tbl =>
                    {
                        tbl.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        // Header
                        tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Име").Bold();
                        tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Длъжност").Bold();
                        tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Подпис").Bold();

                        // Редове
                        foreach (var dev in data.Developers)
                        {
                            tbl.Cell().Border(1).Padding(5).Text(dev.Name);
                            tbl.Cell().Border(1).Padding(5).Text(dev.Position);
                            tbl.Cell().Border(1).Padding(5).Text(""); // Празно поле за подпис
                        }
                    });
                });
            }
        }

        private string GetPhaseText(ProjectPhase phase)
        {
            return phase switch
            {
                ProjectPhase.Ideynyi => "Идеен проект",
                ProjectPhase.Tehnicheski => "Технически проект",
                ProjectPhase.Raboten => "Работен проект",
                _ => "Технически проект"
            };
        }
    }
}
