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
                                    // Заглавие на секцията
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
    }
}
