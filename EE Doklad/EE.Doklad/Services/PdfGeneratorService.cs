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
                            .Justify()
                            .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(column =>
                            {
                                var sections = report.Sections.OrderBy(s => s.Order).ToList();
                                for (int i = 0; i < sections.Count; i++)
                                {
                                    var section = sections[i];
                                    // Ако е Челна страница, рендерираме специален layout
                                    if (section.Type == SectionType.CoverPage && section.CoverPageData != null)
                                    {
                                        GenerateCoverPage(column, section.CoverPageData);
                                        column.Item().PageBreak(); // Нова страница след челната
                                        continue;
                                    }

                                    // Ако е Certificates секция, рендерираме удостоверенията
                                    if (section.Type == SectionType.Certificates && section.CertificatesData != null)
                                    {
                                        GenerateCertificates(column, section.CertificatesData);
                                        column.Item().PageBreak();
                                        continue;
                                    }

                                    // Ако е ObjectData секция, рендерираме данните за обекта
                                    if (section.Type == SectionType.ObjectData && section.ObjectDataSectionData != null)
                                    {
                                        GenerateObjectData(column, section.ObjectDataSectionData);
                                        column.Item().PageBreak();
                                        continue;
                                    }

                                    // Заглавие на секцията (само за Normal секции)
                                    column.Item().Text(section.Title)
                                        .Justify()
                                        .Bold().FontSize(14);
                                    column.Item().PaddingBottom(5);

                                    // Статичен текст
                                    if (!string.IsNullOrWhiteSpace(section.StaticText))
                                    {
                                        column.Item().Text(section.StaticText)
                                            .Justify();
                                        column.Item().PaddingBottom(10);
                                    }

                                    // Таблици
                                    foreach (var table in section.Tables)
                                    {
                                        column.Item().Text(table.Title)
                                            .Justify()
                                            .SemiBold().FontSize(12);
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

                                    // Добавяме page break след ВСЯКА секция, освен последната
                                    if (i < sections.Count - 1)
                                    {
                                        column.Item().PageBreak();
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
                .Justify()
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
                        .Justify()
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
                    .Justify()
                    .FontColor(Colors.Grey.Medium);
            }

            // Данни за фирмата
            column.Item().PaddingBottom(10).Column(col =>
            {
                col.Item().Text("ФИРМА").Justify().Bold().FontSize(12);
                col.Item().Text(data.CompanyName).Justify().FontSize(14);
                col.Item().Text(data.LicenseNumber).Justify().FontSize(12).FontColor(Colors.Grey.Darken1);
            });

            // Данни за обекта
            column.Item().PaddingBottom(10).Column(col =>
            {
                col.Item().Text("ОБЕКТ").Justify().Bold().FontSize(12);
                col.Item().Text(data.ObjectName).Justify().FontSize(14);
                col.Item().Text(data.ObjectAddress).Justify().FontSize(12);
            });

            // Фаза на проектиране
            column.Item().PaddingBottom(10).Row(row =>
            {
                row.RelativeItem().Text("ФАЗА: ").Justify().Bold().FontSize(12);
                row.RelativeItem().Text(GetPhaseText(data.Phase)).Justify().FontSize(12);
            });

            // Управител
            if (!string.IsNullOrEmpty(data.ManagerName))
            {
                column.Item().PaddingTop(20).PaddingBottom(10).Column(col =>
                {
                    col.Item().Text("УПРАВИТЕЛ").Justify().Bold().FontSize(12);
                    col.Item().Text(data.ManagerName).Justify().FontSize(12);
                    col.Item().PaddingTop(10).BorderTop(1).Width(200)
                        .Text("(подпис и печат)")
                        .Justify()
                        .FontSize(9).FontColor(Colors.Grey.Medium);
                });
            }

            // Разработили
            if (data.Developers.Any())
            {
                column.Item().PaddingTop(20).Column(col =>
                {
                    col.Item().Text("РАЗРАБОТИЛ ЕКИП").Justify().Bold().FontSize(12);
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

        /// <summary>
        /// Генерира страниците с удостоверения (Certificate и Insurance)
        /// </summary>
        private void GenerateCertificates(ColumnDescriptor column, CertificatesSectionData data)
        {
            // Certificate attachment
            if (data.CertificateAttachment?.HasAttachment == true)
            {
                RenderAttachment(column, "Удостоверение за регистрация", data.CertificateAttachment);
            }

            // Insurance attachment
            if (data.InsuranceAttachment?.HasAttachment == true)
            {
                RenderAttachment(column, "Застрахователна полица", data.InsuranceAttachment);
            }
        }

        /// <summary>
        /// Рендерира един attachment като отделна страница
        /// </summary>
        private void RenderAttachment(ColumnDescriptor column, string title, AttachmentData attachment)
        {
            // Заглавие
            column.Item()
                .PaddingBottom(10)
                .Text(title)
                .Justify()
                .Bold()
                .FontSize(16);

            // Предупреждение за multi-page PDF
            if (!string.IsNullOrEmpty(attachment.MultiPageWarning))
            {
                column.Item()
                    .PaddingBottom(10)
                    .Text(attachment.MultiPageWarning)
                    .Justify()
                    .FontSize(9)
                    .FontColor(Colors.Orange.Darken2);
            }

            // Рендериране на съдържанието
            if (attachment.Bytes != null && attachment.Bytes.Length > 0)
            {
                try
                {
                    // За изображения - директно рендериране
                    if (attachment.ContentType?.StartsWith("image/") == true)
                    {
                        column.Item()
                            .PaddingBottom(20)
                            .AlignCenter()
                            .MaxWidth(500)
                            .Image(attachment.Bytes);
                    }
                    // За PDF - показваме placeholder (TODO: PDF rendering)
                    else if (attachment.ContentType == "application/pdf")
                    {
                        column.Item()
                            .PaddingBottom(20)
                            .AlignCenter()
                            .Width(400)
                            .Height(300)
                            .Border(2)
                            .BorderColor(Colors.Blue.Lighten2)
                            .Background(Colors.Blue.Lighten5)
                            .AlignMiddle()
                            .AlignCenter()
                            .Column(col =>
                            {
                                col.Item().Text("📄 PDF документ")
                                    .Justify()
                                    .FontSize(18).FontColor(Colors.Blue.Darken2);
                                col.Item().PaddingTop(10)
                                    .Text(attachment.FileName)
                                    .Justify()
                                    .FontSize(12);
                                col.Item().PaddingTop(5)
                                    .Text("(Прегледът на PDF не е наличен в експорта)")
                                    .Justify()
                                    .FontSize(9).FontColor(Colors.Grey.Medium);
                            });
                    }
                }
                catch (Exception ex)
                {
                    // Ако има грешка, показваме error placeholder
                    column.Item()
                        .PaddingBottom(20)
                        .AlignCenter()
                        .Width(400)
                        .Height(200)
                        .Border(2)
                        .BorderColor(Colors.Red.Lighten2)
                        .Background(Colors.Red.Lighten5)
                        .AlignMiddle()
                        .AlignCenter()
                        .Column(col =>
                        {
                            col.Item().Text("⚠️ Грешка при рендериране")
                                .Justify()
                                .FontSize(14).FontColor(Colors.Red.Darken2);
                            col.Item().PaddingTop(5)
                                .Text(ex.Message)
                                .Justify()
                                .FontSize(9).FontColor(Colors.Grey.Medium);
                        });
                }
            }

            // Page break след всяко удостоверение
            column.Item().PageBreak();
        }

        /// <summary>
        /// Генерира страницата с данни за обекта
        /// </summary>
        private void GenerateObjectData(ColumnDescriptor column, ObjectDataSectionData data)
        {
            // Заглавие
            column.Item()
                .PaddingBottom(10)
                .Text(data.Title)
                .Justify()
                .Bold()
                .FontSize(16);

            // Описание (ако има)
            if (!string.IsNullOrWhiteSpace(data.Description))
            {
                column.Item()
                    .PaddingBottom(10)
                    .Text(data.Description)
                    .Justify()
                    .FontSize(10);
            }

            // ТАБЛИЦА 1: Данни за обекта
            column.Item()
                .PaddingBottom(5)
                .Text("Данни за обекта:")
                .Justify()
                .SemiBold()
                .FontSize(12);

            column.Item().Table(tbl =>
            {
                tbl.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                });

                // Редове с данни
                var rows = new[]
                {
                    ("Наименование на сграда", data.BuildingName ?? "-"),
                    ("Адрес", data.Address ?? "-"),
                    ("Тип сграда", data.BuildingType ?? "-"),
                    ("Собственост", data.Ownership ?? "-"),
                    ("Година на построяване", data.YearOfConstruction ?? "-"),
                    ("Брой обитатели", data.NumberOfOccupants ?? "-")
                };

                foreach (var (label, value) in rows)
                {
                    tbl.Cell()
                        .Border(1)
                        .Background(Colors.Grey.Lighten4)
                        .Padding(5)
                        .Text(label)
                        .SemiBold();

                    tbl.Cell()
                        .Border(1)
                        .Padding(5)
                        .Text(value);
                }
            });

            column.Item().PaddingBottom(15);

            // ТАБЛИЦА 2: Графици на сградата
            column.Item()
                .PaddingBottom(5)
                .Text("Графици на сградата:")
                .Justify()
                .SemiBold()
                .FontSize(12);

            column.Item().Table(tbl =>
            {
                tbl.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1.5f);
                    columns.RelativeColumn(1f);
                    columns.RelativeColumn(1f);
                    columns.RelativeColumn(1f);
                });

                // Заголовък - График на обитаване
                tbl.Cell()
                    .Border(1)
                    .Background(Colors.Grey.Lighten3)
                    .Padding(5)
                    .Text("График на обитаване (ч./ден)")
                    .SemiBold();

                tbl.Cell()
                    .Border(1)
                    .Background(Colors.Grey.Lighten3)
                    .Padding(5)
                    .Text("Работни дни")
                    .SemiBold()
                    .AlignCenter();

                tbl.Cell()
                    .Border(1)
                    .Background(Colors.Grey.Lighten3)
                    .Padding(5)
                    .Text("Събота")
                    .SemiBold()
                    .AlignCenter();

                tbl.Cell()
                    .Border(1)
                    .Background(Colors.Grey.Lighten3)
                    .Padding(5)
                    .Text("Неделя")
                    .SemiBold()
                    .AlignCenter();

                // Данни - График на обитаване
                tbl.Cell()
                    .Border(1)
                    .Padding(5)
                    .Text("");

                tbl.Cell()
                    .Border(1)
                    .Padding(5)
                    .Text("24")
                    .AlignCenter();

                tbl.Cell()
                    .Border(1)
                    .Padding(5)
                    .Text("24")
                    .AlignCenter();

                tbl.Cell()
                    .Border(1)
                    .Padding(5)
                    .Text("24")
                    .AlignCenter();

                // Заголовък - График на отопление
                tbl.Cell()
                    .Border(1)
                    .Background(Colors.Grey.Lighten3)
                    .Padding(5)
                    .Text("График на отопление (ч./ден)")
                    .SemiBold();

                tbl.Cell()
                    .Border(1)
                    .Background(Colors.Grey.Lighten3)
                    .Padding(5)
                    .Text("Работни дни")
                    .SemiBold()
                    .AlignCenter();

                tbl.Cell()
                    .Border(1)
                    .Background(Colors.Grey.Lighten3)
                    .Padding(5)
                    .Text("Събота")
                    .SemiBold()
                    .AlignCenter();

                tbl.Cell()
                    .Border(1)
                    .Background(Colors.Grey.Lighten3)
                    .Padding(5)
                    .Text("Неделя")
                    .SemiBold()
                    .AlignCenter();

                // Данни - График на отопление
                tbl.Cell()
                    .Border(1)
                    .Padding(5)
                    .Text("");

                tbl.Cell()
                    .Border(1)
                    .Padding(5)
                    .Text("12")
                    .AlignCenter();

                tbl.Cell()
                    .Border(1)
                    .Padding(5)
                    .Text("24")
                    .AlignCenter();

                tbl.Cell()
                    .Border(1)
                    .Padding(5)
                    .Text("24")
                    .AlignCenter();
            });

            column.Item().PaddingBottom(15);
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
