    // TODO (Етап 2):
    // Тук да се добави engine за изчисление на Ur за студен покрив по методиката (λекв, Rse1=Rsi2, итерация за температури и крайно Ur)
    // След имплементация, U в обобщаващата таблица за студен покрив да се попълва автоматично.
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public record TocItem(string Title, int Page);

        private const string AreaFormat = "0.000";
        private const string ThicknessFormat = "0.000";
        private const string LambdaFormat = "0.000";
        private const string UValueFormat = "0.000";
        private const string RValueFormat = "0.000";
    private const string GenericNumberFormat = "0.000";

        private static string FormatNumber(double value, string format)
        {
            return value.ToString(format, CultureInfo.InvariantCulture);
        }

        private static string FormatCellValue(Cell cell)
        {
            if (cell.Type != CellType.Number)
            {
                return cell.Value ?? string.Empty;
            }

            var raw = cell.Value?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(raw))
            {
                return string.Empty;
            }

            var normalized = raw.Replace(" ", string.Empty).Replace(',', '.');
            if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            {
                return parsed.ToString(GenericNumberFormat, CultureInfo.InvariantCulture);
            }

            return raw;
        }

        private readonly Dictionary<string, int> _pageCountCache = new(StringComparer.Ordinal);

        private int GetPageCount(IDocument document)
        {
            return document.GenerateImages().Count();
        }

        private int GetTocPageCount(Report report, IReadOnlyList<TocItem> tocItems)
        {
            var cacheKey = CreateTocCacheKey(report, tocItems);
            if (_pageCountCache.TryGetValue(cacheKey, out var cached))
            {
                return cached;
            }

            var count = GetPageCount(CreateTocDocument(report, tocItems));
            _pageCountCache[cacheKey] = count;
            return count;
        }

        private int GetSectionPageCount(Report report, Section section)
        {
            var cacheKey = CreateSectionCacheKey(report, section);
            if (_pageCountCache.TryGetValue(cacheKey, out var cached))
            {
                return cached;
            }

            var count = GetPageCount(CreateSectionDocument(report, section));
            _pageCountCache[cacheKey] = count;
            return count;
        }

        private IDocument CreateTocDocument(Report report, IReadOnlyList<TocItem> tocItems)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    ConfigurePage(page, report, column => ComposeToc(column, tocItems));
                });
            });
        }

        private IDocument CreateSectionDocument(Report report, Section section)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    ConfigurePage(page, report, column => ComposeSectionContent(column, section));
                });
            });
        }

        private static string CreateTocCacheKey(Report report, IReadOnlyList<TocItem> tocItems)
        {
            var hash = new HashCode();
            hash.Add(report.Title ?? string.Empty);
            foreach (var item in tocItems)
            {
                hash.Add(item.Title ?? string.Empty);
            }

            return $"toc:{hash.ToHashCode()}";
        }

        private static string CreateSectionCacheKey(Report report, Section section)
        {
            var hash = new HashCode();
            hash.Add(report.Title ?? string.Empty);
            hash.Add(section.Type);
            hash.Add(section.Title ?? string.Empty);
            hash.Add(section.StaticText ?? string.Empty);
            hash.Add(section.Order);

            hash.Add(section.Tables.Count);
            foreach (var table in section.Tables)
            {
                hash.Add(table.Title ?? string.Empty);
                hash.Add(table.ColumnHeaders.Count);
                foreach (var header in table.ColumnHeaders)
                {
                    hash.Add(header ?? string.Empty);
                }
                hash.Add(table.Rows.Count);
                foreach (var row in table.Rows)
                {
                    hash.Add(row.Cells.Count);
                    foreach (var cell in row.Cells)
                    {
                        hash.Add(cell.Value ?? string.Empty);
                    }
                }
            }

            if (section.CoverPageData != null)
            {
                hash.Add(section.CoverPageData.CompanyName ?? string.Empty);
                hash.Add(section.CoverPageData.ObjectName ?? string.Empty);
                hash.Add(section.CoverPageData.ObjectAddress ?? string.Empty);
                hash.Add(section.CoverPageData.LicenseNumber ?? string.Empty);
                hash.Add(section.CoverPageData.ManagerName ?? string.Empty);
                hash.Add(section.CoverPageData.LogoPath ?? string.Empty);
                hash.Add(section.CoverPageData.Phase);
                hash.Add(section.CoverPageData.Developers.Count);
                foreach (var developer in section.CoverPageData.Developers)
                {
                    hash.Add(developer.Name ?? string.Empty);
                    hash.Add(developer.Position ?? string.Empty);
                }
            }

            if (section.CertificatesData != null)
            {
                AddAttachmentToHash(hash, section.CertificatesData.CertificateAttachment);
                AddAttachmentToHash(hash, section.CertificatesData.InsuranceAttachment);
            }

            if (section.ExternalWallsSectionData is { } externalWallsData)
            {
                hash.Add(externalWallsData.Title ?? string.Empty);
                hash.Add(externalWallsData.Description ?? string.Empty);
                hash.Add(externalWallsData.ShowFacadeDistribution);
                hash.Add(externalWallsData.WallTypes.Count);

                foreach (var wallType in externalWallsData.WallTypes)
                {
                    hash.Add(wallType.Index);
                    hash.Add(wallType.Name ?? string.Empty);
                    hash.Add(wallType.Area);
                    hash.Add(wallType.FacadeEast);
                    hash.Add(wallType.FacadeNorth);
                    hash.Add(wallType.FacadeWest);
                    hash.Add(wallType.FacadeSouth);
                    hash.Add(wallType.Rsi);
                    hash.Add(wallType.Rse);

                    hash.Add(wallType.Layers.Count);
                    foreach (var layer in wallType.Layers)
                    {
                        hash.Add(layer.Material ?? string.Empty);
                        hash.Add(layer.Thickness);
                        hash.Add(layer.Lambda);
                    }

                    AddAttachmentToHash(hash, wallType.SchemeAttachment);
                }
            }

            if (section.ObjectDataSectionData != null)
            {
                hash.Add(section.ObjectDataSectionData.Title ?? string.Empty);
                hash.Add(section.ObjectDataSectionData.Description ?? string.Empty);
                hash.Add(section.ObjectDataSectionData.BuildingName ?? string.Empty);
                hash.Add(section.ObjectDataSectionData.Address ?? string.Empty);
                hash.Add(section.ObjectDataSectionData.BuildingType ?? string.Empty);
                hash.Add(section.ObjectDataSectionData.Ownership ?? string.Empty);
                hash.Add(section.ObjectDataSectionData.YearOfConstruction ?? string.Empty);
                hash.Add(section.ObjectDataSectionData.NumberOfOccupants ?? string.Empty);
                hash.Add(section.ObjectDataSectionData.OccupancySchedule ?? string.Empty);
                hash.Add(section.ObjectDataSectionData.HeatingSchedule ?? string.Empty);
            }

            return $"section:{hash.ToHashCode()}";
        }

        private static void AddAttachmentToHash(HashCode hash, AttachmentData? attachment)
        {
            if (attachment == null)
            {
                return;
            }

            hash.Add(attachment.FileName ?? string.Empty);
            hash.Add(attachment.ContentType ?? string.Empty);
            hash.Add(attachment.Bytes?.Length ?? 0);
            hash.Add(attachment.SourcePageCount);
        }

        public PdfGeneratorService()
        {
            // QuestPDF лиценз (Community за некомерсиална употреба)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public void GeneratePdf(Report report, string outputPath)
        {
            try
            {
                var sections = report.Sections.OrderBy(s => s.Order).ToList();
                int tocIndex = sections.FindIndex(s => s.Title?.Trim() == "Съдържание" || s.Title?.ToLower().Contains("съдържание") == true);

                // 1. Render TOC placeholder to get its page count
                var tocItemsPlaceholder = sections
                    .Select(section => new TocItem(section.Title ?? string.Empty, 0))
                    .ToList();

                int tocPageCount = tocIndex >= 0
                    ? GetTocPageCount(report, tocItemsPlaceholder)
                    : 0;

                var pageNumbers = new int[sections.Count];
                int currentPage = 1;
                for (int i = 0; i < sections.Count; i++)
                {
                    pageNumbers[i] = currentPage;
                    if (i == tocIndex)
                    {
                        currentPage += tocPageCount;
                        continue;
                    }

                    currentPage += GetSectionPageCount(report, sections[i]);
                }

                var tocItems = sections
                    .Select((section, idx) => new TocItem(section.Title ?? string.Empty, pageNumbers[idx]))
                    .ToList();

                // 3. Render final document with real TOC
                var docWithToc = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        ConfigurePage(page, report, column =>
                        {
                            for (int i = 0; i < sections.Count; i++)
                            {
                                var section = sections[i];
                                if (i == tocIndex)
                                {
                                    ComposeToc(column, tocItems);
                                    column.Item().PageBreak();
                                    continue;
                                }
                                ComposeSectionContent(column, section);
                                if (i < sections.Count - 1)
                                {
                                    column.Item().PageBreak();
                                }
                            }
                        });
                    });
                });
                docWithToc.GeneratePdf(outputPath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Грешка при генериране на PDF: {ex.Message}", ex);
            }
        }

        private void ConfigurePage(PageDescriptor page, Report report, Action<ColumnDescriptor> contentComposer)
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
                .Column(column => contentComposer(column));

            page.Footer()
                .AlignCenter()
                .Text(x =>
                {
                    x.Span("Страница ");
                    x.CurrentPageNumber();
                    x.Span(" от ");
                    x.TotalPages();
                });
        }

        private void ComposeToc(ColumnDescriptor column, IReadOnlyList<TocItem> tocItems)
        {
            column.Item().Text("Съдържание").FontSize(16).Bold().AlignCenter();
            column.Item().PaddingTop(10);
            foreach (var item in tocItems)
            {
                if (string.IsNullOrWhiteSpace(item.Title))
                {
                    continue;
                }

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(item.Title).FontSize(12);
                    row.ConstantItem(40).AlignRight().Text(item.Page.ToString()).FontSize(12);
                });
            }
        }

    private void ComposeSectionContent(ColumnDescriptor column, Section section)
        {

            if (section.Type == SectionType.CoverPage && section.CoverPageData != null)
            {
                GenerateCoverPage(column, section.CoverPageData);
                return;
            }

            if (section.Type == SectionType.Certificates && section.CertificatesData != null)
            {
                GenerateCertificates(column, section.CertificatesData);
                return;
            }

            if (section.Type == SectionType.ObjectData && section.ObjectDataSectionData != null)
            {
                GenerateObjectData(column, section.ObjectDataSectionData);
                return;
            }

            if (section.Type == SectionType.ExternalWalls && section.ExternalWallsSectionData is { } externalWallsData)
            {
                GenerateExternalWalls(column, externalWallsData);
                return;
            }

            if (section.Type == SectionType.Roof && section.RoofSectionData is { } roofData)
            {
                GenerateRoofSection(column, roofData);
                return;
            }
        }
        // --- Roof Section PDF ---
    private void GenerateRoofSection(ColumnDescriptor column, RoofSectionData data)
    {
        column.Item().Text("Покрив").Justify().Bold().FontSize(14);
        column.Item().PaddingBottom(5);

        if (!string.IsNullOrWhiteSpace(data.Description))
        {
            column.Item().Text(data.Description).Justify();
            column.Item().PaddingBottom(10);
        }

        column.Item().Text("Покрив по типове").SemiBold().FontSize(12);
        column.Item().PaddingBottom(3);

        // Summary table
        column.Item().Table(tbl =>
        {
            tbl.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(25);
                columns.RelativeColumn(3);
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });
            tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("№").Bold();
            tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Тип покрив").Bold();
            tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Режим").Bold();
            tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("U (W/m²K)").Bold();
            tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("A (m²)").Bold();

            foreach (var roofType in data.RoofTypes)
            {
                tbl.Cell().Border(1).Padding(5).Text(roofType.Number.ToString());
                tbl.Cell().Border(1).Padding(5).Text(roofType.Name);
                tbl.Cell().Border(1).Padding(5).Text(roofType.Mode == RoofMode.Warm ? "Топъл" : "Студен");
                tbl.Cell().Border(1).Padding(5).Text(
                    roofType.Mode == RoofMode.Warm
                        ? (roofType.U.HasValue ? FormatNumber((double)roofType.U.Value, UValueFormat) : "")
                        : "Неизчислено (Етап 2)");
                tbl.Cell().Border(1).Padding(5).Text(FormatNumber((double)roofType.A, AreaFormat));
            }
        });

        foreach (var roofType in data.RoofTypes)
        {
            column.Item().PaddingTop(10).Text($"{roofType.Name}").Justify().Bold().FontSize(12);
            if (roofType.Mode == RoofMode.Warm && roofType.WarmDetail != null)
            {
                // Warm roof detail
                column.Item().Text("Слоеве").SemiBold().FontSize(11);
                column.Item().Table(tbl =>
                {
                    tbl.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });
                    tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Материал").Bold();
                    tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("δ (m)").Bold();
                    tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("λ (W/mK)").Bold();
                    tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("R=δ/λ").Bold();
                    foreach (var layer in roofType.WarmDetail.Layers)
                    {
                        tbl.Cell().Border(1).Padding(5).Text(layer.Material);
                        tbl.Cell().Border(1).Padding(5).Text(FormatNumber((double)layer.Thickness, ThicknessFormat));
                        tbl.Cell().Border(1).Padding(5).Text(FormatNumber((double)layer.Lambda, LambdaFormat));
                        tbl.Cell().Border(1).Padding(5).Text(FormatNumber((double)layer.R, RValueFormat));
                    }
                });
                column.Item().Text($"Rsi = {FormatNumber((double)roofType.WarmDetail.Rsi, RValueFormat)} m²K/W, Rse = {FormatNumber((double)roofType.WarmDetail.Rse, RValueFormat)} m²K/W").FontSize(10);
            }
            else if (roofType.Mode == RoofMode.Cold && roofType.ColdDetail != null)
            {
                // Cold roof detail (Stage 1: all inputs, no Ur)
                var cold = roofType.ColdDetail;
                column.Item().Text("Геометрия на подпокривното пространство").SemiBold().FontSize(11);
                column.Item().Text($"V′ = {FormatNumber((double)cold.Vp, AreaFormat)} m³, A′ = {FormatNumber((double)cold.Ap, AreaFormat)} m², δвс = {(cold.Deltavc.HasValue ? FormatNumber((double)cold.Deltavc.Value, ThicknessFormat) : "-")} m").FontSize(10);
                column.Item().Text("Площи на огражденията").SemiBold().FontSize(11);
                column.Item().Text($"A1 = {FormatNumber((double)cold.A1, AreaFormat)} m², A2 = {FormatNumber((double)cold.A2, AreaFormat)} m², Aw = {FormatNumber((double)cold.Aw, AreaFormat)} m²").FontSize(10);
                column.Item().Text("Вентилация").SemiBold().FontSize(11);
                column.Item().Text($"Тип: {(cold.SpaceType == ColdRoofSpaceType.Sealed ? "уплътнено" : "неуплътнено")}, n = {FormatNumber((double)cold.n, GenericNumberFormat)} h⁻¹, V = {FormatNumber((double)cold.V, AreaFormat)} m³").FontSize(10);
                column.Item().Text("Температури").SemiBold().FontSize(11);
                column.Item().Text($"θi = {FormatNumber((double)cold.Ti, "0.0")} B0C, θe = {FormatNumber((double)cold.Te, "0.0")} B0C").FontSize(10);
                // Layer tables for U1, U2, Uw
                column.Item().Text("Таванска плоча (U1)").SemiBold().FontSize(11);
                ComposeRoofLayerTable(column, cold.U1, "Rsi1", cold.U1.Rsi, "Rse1", cold.U1.Rse, cold.U1.RsiEditable, cold.U1.RseEditable);
                column.Item().Text("Покривна плоча (U2)").SemiBold().FontSize(11);
                ComposeRoofLayerTable(column, cold.U2, "Rsi2", cold.U2.Rsi, "Rse2", cold.U2.Rse, cold.U2.RsiEditable, cold.U2.RseEditable);
                column.Item().Text("Вертикални ограждения (Uw)").SemiBold().FontSize(11);
                ComposeRoofLayerTable(column, cold.Uw, "Rsiw", cold.Uw.Rsi, "Rsew", cold.Uw.Rse, cold.Uw.RsiEditable, cold.Uw.RseEditable);
                column.Item().Text("U = Неизчислено (Етап 2)").FontColor(Colors.Grey.Darken1).FontSize(10);
            }
        }
    }

    private void ComposeRoofLayerTable(ColumnDescriptor column, RoofLayerTable table, string rsiLabel, decimal rsi, string rseLabel, decimal rse, bool rsiEditable, bool rseEditable)
    {
        column.Item().Table(tbl =>
        {
            tbl.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3);
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });
            tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Материал").Bold();
            tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("δ (m)").Bold();
            tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("λ (W/mK)").Bold();
            tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("R=δ/λ").Bold();
            foreach (var layer in table.Layers)
            {
                tbl.Cell().Border(1).Padding(5).Text(layer.Material);
                tbl.Cell().Border(1).Padding(5).Text(FormatNumber((double)layer.Thickness, ThicknessFormat));
                tbl.Cell().Border(1).Padding(5).Text(FormatNumber((double)layer.Lambda, LambdaFormat));
                tbl.Cell().Border(1).Padding(5).Text(FormatNumber((double)layer.R, RValueFormat));
            }
        });
        column.Item().Text($"{rsiLabel} = {FormatNumber((double)rsi, RValueFormat)} m²K/W, {rseLabel} = {FormatNumber((double)rse, RValueFormat)} m²K/W").FontSize(10);
    }

        // ...existing code...

        private void GenerateExternalWalls(ColumnDescriptor column, ExternalWallsSectionData data)
        {
            column.Item().Text(data.Title)
                .Justify()
                .Bold().FontSize(14);
            column.Item().PaddingBottom(5);

            if (!string.IsNullOrWhiteSpace(data.Description))
            {
                column.Item().Text(data.Description)
                    .Justify();
                column.Item().PaddingBottom(10);
            }

            var showFacade = data.ShowFacadeDistribution;

            column.Item().Text("Фасади / Външни стени")
                .Justify()
                .SemiBold().FontSize(12);
            column.Item().PaddingBottom(3);

            column.Item().Table(tbl =>
            {
                int facadeColumns = showFacade ? 4 : 0;
                int columnCount = 4 + facadeColumns;
                tbl.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(25);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    for (int i = 0; i < facadeColumns; i++)
                    {
                        columns.RelativeColumn();
                    }
                });

                tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("№").Bold();
                tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Тип стена").Bold();
                tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("A (m²)").Bold();
                tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("U (W/m²K)").Bold();

                if (showFacade)
                {
                    tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("И").Bold();
                    tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("С").Bold();
                    tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("З").Bold();
                    tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Ю").Bold();
                }

                foreach (var wallType in data.WallTypes.Take(8))
                {
                    var index = wallType.Index > 0 ? wallType.Index : data.WallTypes.IndexOf(wallType) + 1;
                    tbl.Cell().Border(1).Padding(5).Text(index.ToString());
                    tbl.Cell().Border(1).Padding(5).Text(wallType.Name);
                    tbl.Cell().Border(1).Padding(5).Text(FormatNumber(wallType.Area, AreaFormat));
                    tbl.Cell().Border(1).Padding(5).Text(FormatNumber(wallType.Uw, UValueFormat));

                    if (showFacade)
                    {
                        tbl.Cell().Border(1).Padding(5).Text(FormatNumber(wallType.FacadeEast, AreaFormat));
                        tbl.Cell().Border(1).Padding(5).Text(FormatNumber(wallType.FacadeNorth, AreaFormat));
                        tbl.Cell().Border(1).Padding(5).Text(FormatNumber(wallType.FacadeWest, AreaFormat));
                        tbl.Cell().Border(1).Padding(5).Text(FormatNumber(wallType.FacadeSouth, AreaFormat));
                    }
                }
            });

            foreach (var wallType in data.WallTypes.Take(8))
            {
                var index = wallType.Index > 0 ? wallType.Index : data.WallTypes.IndexOf(wallType) + 1;
                column.Item().PaddingTop(10).Text($"СТЕНА ТИП {index}")
                    .Justify()
                    .Bold().FontSize(12);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Table(tbl =>
                        {
                            tbl.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Материал").Bold();
                            tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("δ (m)").Bold();
                            tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("λ (W/mK)").Bold();
                            tbl.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("R=δ/λ").Bold();

                            foreach (var layer in wallType.Layers)
                            {
                                tbl.Cell().Border(1).Padding(5).Text(layer.Material);
                                tbl.Cell().Border(1).Padding(5).Text(FormatNumber(layer.Thickness, ThicknessFormat));
                                tbl.Cell().Border(1).Padding(5).Text(FormatNumber(layer.Lambda, LambdaFormat));
                                tbl.Cell().Border(1).Padding(5).Text(FormatNumber(layer.R, RValueFormat));
                            }
                        });

                        col.Item().PaddingTop(5).Text($"Rsi={FormatNumber(wallType.Rsi, RValueFormat)} | Rse={FormatNumber(wallType.Rse, RValueFormat)} | Rw={FormatNumber(wallType.Rw, RValueFormat)} | Rtotal={FormatNumber(wallType.Rtotal, RValueFormat)} | Uw={FormatNumber(wallType.Uw, UValueFormat)}")
                            .FontSize(10).SemiBold();
                    });

                    row.ConstantItem(180).Border(1).Padding(5).AlignMiddle().AlignCenter()
                        .Element(container => RenderWallScheme(container, wallType.SchemeAttachment));
                });
            }
        }

        private void RenderWallScheme(IContainer container, AttachmentData? attachment)
        {
            if (attachment?.Bytes != null && attachment.Bytes.Length > 0 && attachment.ContentType != "application/pdf")
            {
                container.Image(attachment.Bytes);
                return;
            }

            container.AlignCenter().AlignMiddle().Text("Схема").FontSize(9).FontColor(Colors.Grey.Medium);
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
