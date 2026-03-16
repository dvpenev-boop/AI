using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using EE.Doklad.Models;
using EE.Doklad.Services;
using EE.Doklad.ViewModels;
using EE.Doklad.Views;
using ModelSection = EE.Doklad.Models.Section;
using ModelSectionType = EE.Doklad.Models.SectionType;

namespace EE.Doklad;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private static int CalculateWorkingDaysPerYearFromDaysOff(ObjectDataSectionData objectData)
    {
        int result = 365 - objectData.MonthlyDaysOffSum;
        return result > 0 ? result : 0;
    }

    public MainWindow()
    {
        InitializeComponent();
        
        // Subscribe към промяна на SelectedSection
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
        
        Loaded += MainWindow_Loaded;
    }

    private void OpenClimateData_Click(object sender, RoutedEventArgs e)
    {
        // Show read-only methodology data viewer.
        ContentScrollViewer.Content = new ClimateDataView();
    }

    private void OpenMaterials_Click(object sender, RoutedEventArgs e)
    {
        ContentScrollViewer.Content = new MaterialsCatalogView();
    }

    private void OpenLighting_Click(object sender, RoutedEventArgs e)
    {
        ContentScrollViewer.Content = new LightingCatalogView();
    }

    private void OpenAppliances_Click(object sender, RoutedEventArgs e)
    {
        ContentScrollViewer.Content = new AppliancesCatalogView();
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Актуализираме UI при първоначално зареждане
        UpdateSectionContent();

        // Enable mouse wheel scrolling on ContentScrollViewer
        ContentScrollViewer.PreviewMouseWheel += (s, args) =>
        {
            var scrollViewer = (ScrollViewer)s;
            if (scrollViewer.ScrollableHeight > 0)
            {
                double newOffset = scrollViewer.VerticalOffset - (args.Delta / 3.0);
                newOffset = System.Math.Max(0, System.Math.Min(newOffset, scrollViewer.ScrollableHeight));
                scrollViewer.ScrollToVerticalOffset(newOffset);
                args.Handled = true;
            }
        };
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.SelectedSection))
        {
            UpdateSectionContent();
        }
    }

    private void UpdateSectionContent()
    {
        // Cleanup previous ViewModel if it was EnergyClassViewModel
        if (ContentScrollViewer.Content is FrameworkElement oldContent && 
            oldContent.Tag is ViewModels.EnergyClassViewModel oldViewModel)
        {
            oldViewModel.Cleanup();
        }

        if (DataContext is not MainViewModel viewModel || viewModel.SelectedSection == null)
        {
            ContentScrollViewer.Content = null;
            return;
        }

        var section = viewModel.SelectedSection;

        // Проверка за Секция 3 "Съдържание" (може да е по Type или Title)
        // Ако имате специален SectionType за съдържание, използвайте него. Тук ще проверим по Title и/или номер.
        // Пример: ако секцията е трета и/или има заглавие "Съдържание"
        if ((section.Title?.Trim() == "Съдържание" || section.Title?.ToLower().Contains("съдържание") == true) || (viewModel.CurrentReport?.Sections?.IndexOf(section) == 2))
        {
            // Показваме само информационно съобщение
            var infoPanel = new StackPanel { Margin = new Thickness(40, 80, 40, 0), HorizontalAlignment = HorizontalAlignment.Center };
            infoPanel.Children.Add(new TextBlock
            {
                Text = "Секция 3 \"Съдържание\"",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20),
                TextAlignment = TextAlignment.Center
            });
            infoPanel.Children.Add(new TextBlock
            {
                Text = "Генерира се автоматично.",
                FontSize = 15,
                Foreground = Brushes.DimGray,
                TextAlignment = TextAlignment.Center
            });
            ContentScrollViewer.Content = infoPanel;
            return;
        }

        if (section.Type == ModelSectionType.CoverPage && section.CoverPageData != null)
        {
            // Показваме CoverPage Editor
            var coverPageEditor = new CoverPageEditor
            {
                DataContext = section.CoverPageData
            };
            ContentScrollViewer.Content = coverPageEditor;
        }
        else if (section.Type == ModelSectionType.Certificates)
        {
            // Показваме Certificates Editor
            if (section.CertificatesData == null)
                section.CertificatesData = new Models.CertificatesSectionData
                {
                    Title = "Удостоверения",
                    CertificateAttachment = new Models.AttachmentData(),
                    InsuranceAttachment = new Models.AttachmentData()
                };
            
            var certificatesEditor = new CertificatesSectionEditor
            {
                DataContext = section.CertificatesData
            };
            ContentScrollViewer.Content = certificatesEditor;
        }
        else if (section.Type == ModelSectionType.ObjectData)
        {
            // Показваме ObjectData Editor
            if (section.ObjectDataSectionData == null)
                section.ObjectDataSectionData = new Models.ObjectDataSectionData();
            
            var objectDataEditor = new ObjectDataSectionEditor
            {
                DataContext = section.ObjectDataSectionData
            };
            ContentScrollViewer.Content = objectDataEditor;
        }
        else if (section.Type == ModelSectionType.ExternalWalls ||
                 (!string.IsNullOrEmpty(section.Title) && section.Title.Contains("Външни стени")))
        {
            if (section.ExternalWallsSectionData == null)
                section.ExternalWallsSectionData = new Models.ExternalWallsSectionData();

            section.Type = ModelSectionType.ExternalWalls;

            try
            {
                var externalWallsEditor = new ExternalWallsSectionEditor
                {
                    DataContext = section.ExternalWallsSectionData
                };
                ContentScrollViewer.Content = externalWallsEditor;
            }
            catch (System.Exception ex)
            {
                // Surface the error so the user can see what went wrong when selecting the section
                var errorPanel = new StackPanel { Margin = new Thickness(40, 40, 40, 0) };
                errorPanel.Children.Add(new TextBlock
                {
                    Text = "Грешка при зареждане на секция 'Външни стени':",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 10)
                });
                var tb = new TextBox
                {
                    Text = ex.ToString(),
                    TextWrapping = TextWrapping.Wrap,
                    IsReadOnly = true,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Height = 300
                };
                errorPanel.Children.Add(tb);
                ContentScrollViewer.Content = errorPanel;
            }
        }
        else if (section.Type == ModelSectionType.Roof ||
                 (!string.IsNullOrEmpty(section.Title) && section.Title.Contains("Покрив")))
        {
            if (section.RoofSectionData == null)
                section.RoofSectionData = new Models.RoofSectionData();

            section.Type = ModelSectionType.Roof;
            try
            {
                var roofSectionView = new RoofSectionView
                {
                    DataContext = new ViewModels.RoofSectionViewModel(section.RoofSectionData)
                };
                ContentScrollViewer.Content = roofSectionView;
            }
            catch (System.Exception ex)
            {
                var errorPanel = new StackPanel { Margin = new Thickness(40, 40, 40, 0) };
                errorPanel.Children.Add(new TextBlock
                {
                    Text = "Грешка при зареждане на секция 'Покрив':",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 10)
                });
                var tb = new TextBox
                {
                    Text = ex.ToString(),
                    TextWrapping = TextWrapping.Wrap,
                    IsReadOnly = true,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Height = 300
                };
                errorPanel.Children.Add(tb);
                ContentScrollViewer.Content = errorPanel;
            }
        }
        else if (section.Type == ModelSectionType.Floor ||
                 (!string.IsNullOrEmpty(section.Title) && section.Title.Contains("Под")))
        {
            if (section.FloorSectionData == null)
                section.FloorSectionData = new Models.FloorSectionData();

            section.Type = ModelSectionType.Floor;

            var floorSectionView = new FloorSectionView
            {
                DataContext = new ViewModels.FloorSectionViewModel(section.FloorSectionData)
            };
            ContentScrollViewer.Content = floorSectionView;
        }
        else if (section.Type == ModelSectionType.Windows ||
                 (!string.IsNullOrEmpty(section.Title) && section.Title.Contains("Прозорци")))
        {
            if (section.WindowsSectionData == null)
                section.WindowsSectionData = new Models.WindowsSectionData();

            section.Type = ModelSectionType.Windows;

            var windowsSectionView = new WindowsSectionView
            {
                DataContext = new ViewModels.WindowsSectionViewModel(section.WindowsSectionData, viewModel.CurrentReport)
            };
            ContentScrollViewer.Content = windowsSectionView;
        }
        else if (section.Type == ModelSectionType.UnconditionedZones ||
                 (!string.IsNullOrEmpty(section.Title) && section.Title.Contains("Неклиматизирани зони")))
        {
            if (section.UnconditionedZoneSectionData == null)
                section.UnconditionedZoneSectionData = new Models.UnconditionedZoneSectionData();

            section.Type = ModelSectionType.UnconditionedZones;

            // Provide object data and heating data to the viewmodel so section 10 can compute seasonal temps
            var objectDataSection = viewModel.CurrentReport?.Sections?.FirstOrDefault(s => s.Type == ModelSectionType.ObjectData);
            var objectData = objectDataSection?.ObjectDataSectionData;
            var heatingSection = viewModel.CurrentReport?.Sections?.FirstOrDefault(s => s.Type == ModelSectionType.Heating);
            var heatingData = heatingSection?.HeatingSectionData;
            var coolingSection = viewModel.CurrentReport?.Sections?.FirstOrDefault(s => s.Type == ModelSectionType.Normal && !string.IsNullOrEmpty(s.Title) && s.Title.Contains("Охлаждане"));
            var coolingData = coolingSection?.CoolingSectionData;

            var unconditionedZonesView = new UnconditionedZonesSectionView
            {
                DataContext = new ViewModels.UnconditionedZonesSectionViewModel(section.UnconditionedZoneSectionData, objectData, heatingData, coolingData)
            };
            ContentScrollViewer.Content = unconditionedZonesView;
        }
    else if (section.Type == ModelSectionType.Heating ||
         (section.Type == ModelSectionType.Normal && !string.IsNullOrEmpty(section.Title) && section.Title.Contains("Отопление")))
        {
            if (section.HeatingSectionData == null)
                section.HeatingSectionData = new Models.HeatingSectionData();

            section.Type = ModelSectionType.Heating;

            // Намираме секция 5 (ObjectData) за да извлечем броя обитатели
            var objectDataSection = viewModel.CurrentReport?.Sections?.FirstOrDefault(s => s.Type == ModelSectionType.ObjectData);
            var objectData = objectDataSection?.ObjectDataSectionData;

            var heatingSectionView = new HeatingSectionView
            {
                DataContext = new ViewModels.HeatingSectionViewModel(section.HeatingSectionData, objectData, viewModel.CurrentReport)
            };
            ContentScrollViewer.Content = heatingSectionView;
        }
        else if (section.Type == ModelSectionType.Normal &&
                 !string.IsNullOrEmpty(section.Title) && section.Title.Contains("12. Охлаждане"))
        {
            if (section.CoolingSectionData == null)
                section.CoolingSectionData = new Models.CoolingSectionData();

            // Намираме секция 5 (ObjectData) за да извлечем броя обитатели
            var objectDataSection = viewModel.CurrentReport?.Sections?.FirstOrDefault(s => s.Type == ModelSectionType.ObjectData);
            var objectData = objectDataSection?.ObjectDataSectionData;

            var coolingSectionView = new Views.CoolingSectionView
            {
                DataContext = new ViewModels.CoolingSectionViewModel(section.CoolingSectionData, objectData)
            };
            ContentScrollViewer.Content = coolingSectionView;
        }
        else if (!string.IsNullOrEmpty(section.Title) && section.Title.Contains("Вентилация Охлаждане"))
        {
            if (section.VentilationSectionData == null)
                section.VentilationSectionData = new Models.VentilationSectionData();

            section.Type = ModelSectionType.Ventilation;

            var objectDataSection = viewModel.CurrentReport?.Sections?.FirstOrDefault(s => s.Type == ModelSectionType.ObjectData);
            var objectData = objectDataSection?.ObjectDataSectionData;

            var coolingSection = viewModel.CurrentReport?.Sections?.FirstOrDefault(s => s.Type == ModelSectionType.Normal && !string.IsNullOrEmpty(s.Title) && s.Title.Contains("Охлаждане"));
            var coolingData = coolingSection?.CoolingSectionData;

            try
            {
                var ventCoolingView = new Views.VentilationCoolingSectionView
                {
                    DataContext = new ViewModels.VentilationCoolingSectionViewModel(section.VentilationSectionData, objectData, coolingData, viewModel.CurrentReport)
                };
                ContentScrollViewer.Content = ventCoolingView;
            }
            catch (System.Exception ex)
            {
                try
                {
                    System.IO.File.WriteAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ventilation_view_error.txt"), ex.ToString());
                }
                catch { }

                System.Windows.MessageBox.Show("Възникна грешка при отваряне на секция Вентилация Охлаждане. Проверете ventilation_view_error.txt за подробности.", "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else if (section.Type == ModelSectionType.Ventilation ||
                 (section.Type == ModelSectionType.Normal && !string.IsNullOrEmpty(section.Title) && section.Title.Contains("Вентилация")))
        {
            if (section.VentilationSectionData == null)
                section.VentilationSectionData = new Models.VentilationSectionData();

            section.Type = ModelSectionType.Ventilation;

            // Намираме секция 5 (ObjectData) за отопляемата площ и климатичната зона
            var objectDataSection = viewModel.CurrentReport?.Sections?.FirstOrDefault(s => s.Type == ModelSectionType.ObjectData);
            var objectData = objectDataSection?.ObjectDataSectionData;

            // Намираме секция 10 (Heating) за вътрешната температура
            var heatingSection = viewModel.CurrentReport?.Sections?.FirstOrDefault(s => s.Type == ModelSectionType.Heating);
            var heatingData = heatingSection?.HeatingSectionData;

            // Получаваме климатични данни от климатичната зона
            Models.ClimateZoneData? climateData = null;
            if (objectData != null)
            {
                var climateService = new ClimateService(new JsonClimateRepository());
                if (climateService.TryGetZone(objectData.ClimateZone, out var zone))
                {
                    climateData = zone;
                }
            }

            // Актуализираме отопляемата площ
            if (objectData != null && double.TryParse(objectData.HeatedArea, out double heatedArea))
            {
                section.VentilationSectionData.HeatedArea_m2 = heatedArea;
            }

            // Актуализираме вътрешната температура от секция Отопление
            if (heatingData != null)
            {
                section.VentilationSectionData.IndoorTemperature_C = heatingData.DesignTemperature;
            }

            try
            {
                var ventilationView = new Views.VentilationSectionView
                {
                    DataContext = new ViewModels.VentilationSectionViewModel(
                        section.VentilationSectionData,
                        objectData,
                        climateData)
                };
                ContentScrollViewer.Content = ventilationView;
            }
            catch (System.Exception ex)
            {
                // Log the exception to disk for debugging and show a simple message so the user knows
                try
                {
                    System.IO.File.WriteAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ventilation_view_error.txt"), ex.ToString());
                }
                catch { }

                System.Windows.MessageBox.Show("Възникна грешка при отваряне на секция Вентилация. Проверете ventilation_view_error.txt за подробности.", "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else if (section.Type == ModelSectionType.HotWater ||
                 (!string.IsNullOrEmpty(section.Title) && section.Title.Contains("Топла вода за битови нужди")))
        {
            if (section.HotWaterSectionData == null)
                section.HotWaterSectionData = new Models.HotWaterSectionData();

            section.Type = ModelSectionType.HotWater;

            // Намираме секция 5 (ObjectData) за да извлечем необходимите данни
            var objectDataSection = viewModel.CurrentReport?.Sections?.FirstOrDefault(s => s.Type == ModelSectionType.ObjectData);
            var objectData = objectDataSection?.ObjectDataSectionData;

            if (objectData != null)
            {
                // Брой хора
                if (int.TryParse(objectData.NumberOfOccupants, out int numberOfPeople))
                {
                    section.HotWaterSectionData.SetNumberOfPeople(numberOfPeople);
                }
                else
                {
                    section.HotWaterSectionData.SetNumberOfPeople(0);
                }

                // Отопляема площ
                if (double.TryParse(objectData.HeatedArea, out double heatedArea))
                {
                    section.HotWaterSectionData.SetHeatedArea(heatedArea);
                }
                else
                {
                    section.HotWaterSectionData.SetHeatedArea(0);
                }

                // Брой дни (почивни дни)
                var holidaysSum = objectData.MonthlyDaysOffSum;
                section.HotWaterSectionData.SetHolidaysPerYear(holidaysSum);

                // Days-per-week: compute days (5/6/7) from occupancy schedule in section 5
                static bool ParsePositive(string? s)
                {
                    if (string.IsNullOrWhiteSpace(s)) return false;
                    if (double.TryParse(s.Trim(), out double v)) return v > 0.0;
                    return false;
                }

                bool work = ParsePositive(objectData.OccupancyWorkdaysHours);
                bool sat = ParsePositive(objectData.OccupancySaturdayHours);
                bool sun = ParsePositive(objectData.OccupancySundayHours);
                int days = work ? 5 + (sat ? 1 : 0) + (sun ? 1 : 0) : 5;
                section.HotWaterSectionData.SetDaysPerWeek(days);

                // Keep UI reactive: when occupancy fields change, update days in section
                objectData.PropertyChanged += (s, e) =>
                {
                    bool w = ParsePositive(objectData.OccupancyWorkdaysHours);
                    bool sa = ParsePositive(objectData.OccupancySaturdayHours);
                    bool su = ParsePositive(objectData.OccupancySundayHours);
                    int ndays = w ? 5 + (sa ? 1 : 0) + (su ? 1 : 0) : 5;
                    section.HotWaterSectionData.SetDaysPerWeek(ndays);
                    if (e.PropertyName != null && e.PropertyName.StartsWith("DaysOff"))
                    {
                        section.HotWaterSectionData.SetHolidaysPerYear(objectData.MonthlyDaysOffSum);
                    }
                };
            }

            var hotWaterEditor = new HotWaterSectionEditor
            {
                DataContext = section.HotWaterSectionData
            };
            ContentScrollViewer.Content = hotWaterEditor;
        }
        else if (section.Type == ModelSectionType.PumpsAndFans ||
                 (!string.IsNullOrEmpty(section.Title) && section.Title.Contains("Помпи и вентилатори")))
        {
            try
            {
                if (section.PumpsAndFansSectionData == null)
                    section.PumpsAndFansSectionData = new Models.PumpsAndFansSectionData();

                section.Type = ModelSectionType.PumpsAndFans;

                // Намираме секция 5 (ObjectData) за автоматично изчисление на часове
                var objectDataSection = viewModel.CurrentReport?.Sections?.FirstOrDefault(s => s.Type == ModelSectionType.ObjectData);
                var objectData = objectDataSection?.ObjectDataSectionData;

                var pumpsAndFansView = new PumpsAndFansSectionView
                {
                    DataContext = new ViewModels.PumpsAndFansSectionViewModel(section.PumpsAndFansSectionData, objectData)
                };
                ContentScrollViewer.Content = pumpsAndFansView;
            }
            catch (System.Exception ex)
            {
                // Log the exception to disk for debugging
                try
                {
                    System.IO.File.WriteAllText(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "pumps_fans_view_error.txt"), ex.ToString());
                }
                catch { }

                System.Windows.MessageBox.Show($"Грешка при отваряне на секция Помпи и вентилатори:\n{ex.Message}", "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else if (section.Type == ModelSectionType.Lighting ||
                 (!string.IsNullOrEmpty(section.Title) && section.Title.Contains("Осветление")))
        {
            if (section.LightingSectionData == null)
                section.LightingSectionData = new Models.LightingSectionData();

            section.Type = ModelSectionType.Lighting;

            // Намираме секция 5 (ObjectData) за да извлечем HolidaysPerYear и HeatedArea
            var objectDataSection = viewModel.CurrentReport?.Sections?.FirstOrDefault(s => s.Type == ModelSectionType.ObjectData);
            var objectData = objectDataSection?.ObjectDataSectionData;

            if (objectData != null)
            {
                var holidaysSum = objectData.MonthlyDaysOffSum;
                System.Diagnostics.Debug.WriteLine($"[MainWindow] Loading Lighting section - MonthlyDaysOffSum: {holidaysSum}");
                
                section.LightingSectionData.SetHolidaysPerYear(holidaysSum);
                
                // Извличане на отопляемата площ от ObjectData
                if (double.TryParse(objectData.HeatedArea, out double heatedArea))
                {
                    System.Diagnostics.Debug.WriteLine($"[MainWindow] Loading Lighting section - HeatedArea: {heatedArea}");
                    section.LightingSectionData.SetHeatedArea(heatedArea);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[MainWindow] Loading Lighting section - HeatedArea parse failed: {objectData.HeatedArea}");
                }
                section.LightingSectionData.SetOccupancyWorkingDaysPerYear(
                    CalculateWorkingDaysPerYearFromDaysOff(objectData));

                // React to changes in ObjectData occupancy schedule
                objectData.PropertyChanged += (s, e) =>
                {
                    section.LightingSectionData.SetOccupancyWorkingDaysPerYear(
                        CalculateWorkingDaysPerYearFromDaysOff(objectData));
                    // Also update holidays and heated area if DaysOff changed
                    if (e.PropertyName != null && e.PropertyName.StartsWith("DaysOff"))
                    {
                        section.LightingSectionData.SetHolidaysPerYear(objectData.MonthlyDaysOffSum);
                    }
                };
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[MainWindow] Loading Lighting section - ObjectData is NULL");
            }

            var lightingEditor = new LightingSectionEditor
            {
                DataContext = section.LightingSectionData
            };
            ContentScrollViewer.Content = lightingEditor;
        }
        else if (section.Type == ModelSectionType.AppliancesAffecting ||
                 (!string.IsNullOrEmpty(section.Title) && section.Title.Contains("Други разходи влияещи")))
        {
            if (section.AppliancesAffectingSectionData == null)
            {
                section.AppliancesAffectingSectionData = new Models.AppliancesSectionData
                {
                    Title = "16. Други разходи влияещи"
                };
            }

            section.Type = ModelSectionType.AppliancesAffecting;

            // Намираме секция 5 (ObjectData) за да извлечем HolidaysPerYear и HeatedArea
            var objectDataSection2 = viewModel.CurrentReport?.Sections?.FirstOrDefault(s => s.Type == ModelSectionType.ObjectData);
            var objectData2 = objectDataSection2?.ObjectDataSectionData;

            if (objectData2 != null)
            {
                var holidaysSum = objectData2.MonthlyDaysOffSum;
                section.AppliancesAffectingSectionData.SetHolidaysPerYear(holidaysSum);
                
                if (double.TryParse(objectData2.HeatedArea, out double heatedArea))
                {
                    section.AppliancesAffectingSectionData.SetHeatedArea(heatedArea);
                }
                section.AppliancesAffectingSectionData.SetOccupancyWorkingDaysPerYear(
                    CalculateWorkingDaysPerYearFromDaysOff(objectData2));

                objectData2.PropertyChanged += (s, e) =>
                {
                    section.AppliancesAffectingSectionData.SetOccupancyWorkingDaysPerYear(
                        CalculateWorkingDaysPerYearFromDaysOff(objectData2));
                    if (e.PropertyName != null && e.PropertyName.StartsWith("DaysOff"))
                    {
                        section.AppliancesAffectingSectionData.SetHolidaysPerYear(objectData2.MonthlyDaysOffSum);
                    }
                };
            }

            var appliancesAffectingEditor = new AppliancesSectionEditor
            {
                DataContext = section.AppliancesAffectingSectionData
            };
            ContentScrollViewer.Content = appliancesAffectingEditor;
        }
        else if (section.Type == ModelSectionType.AppliancesNotAffecting ||
                 (!string.IsNullOrEmpty(section.Title) && section.Title.Contains("Други разходи невлияещи")))
        {
            if (section.AppliancesNotAffectingSectionData == null)
            {
                section.AppliancesNotAffectingSectionData = new Models.AppliancesSectionData
                {
                    Title = "17. Други разходи невлияещи"
                };
            }

            section.Type = ModelSectionType.AppliancesNotAffecting;

            // Намираме секция 5 (ObjectData) за да извлечем HolidaysPerYear и HeatedArea
            var objectDataSection3 = viewModel.CurrentReport?.Sections?.FirstOrDefault(s => s.Type == ModelSectionType.ObjectData);
            var objectData3 = objectDataSection3?.ObjectDataSectionData;

            if (objectData3 != null)
            {
                var holidaysSum = objectData3.MonthlyDaysOffSum;
                section.AppliancesNotAffectingSectionData.SetHolidaysPerYear(holidaysSum);
                
                if (double.TryParse(objectData3.HeatedArea, out double heatedArea))
                {
                    section.AppliancesNotAffectingSectionData.SetHeatedArea(heatedArea);
                }
                section.AppliancesNotAffectingSectionData.SetOccupancyWorkingDaysPerYear(
                    CalculateWorkingDaysPerYearFromDaysOff(objectData3));

                objectData3.PropertyChanged += (s, e) =>
                {
                    section.AppliancesNotAffectingSectionData.SetOccupancyWorkingDaysPerYear(
                        CalculateWorkingDaysPerYearFromDaysOff(objectData3));
                    if (e.PropertyName != null && e.PropertyName.StartsWith("DaysOff"))
                    {
                        section.AppliancesNotAffectingSectionData.SetHolidaysPerYear(objectData3.MonthlyDaysOffSum);
                    }
                };
            }

            var appliancesNotAffectingEditor = new AppliancesSectionEditor
            {
                DataContext = section.AppliancesNotAffectingSectionData
            };
            ContentScrollViewer.Content = appliancesNotAffectingEditor;
        }
        else if (section.Type == ModelSectionType.Results ||
                 (!string.IsNullOrEmpty(section.Title) && section.Title.Contains("Резултати сграда")))
        {
            if (section.ResultsSectionData == null)
            {
                section.ResultsSectionData = new Models.ResultsSectionData
                {
                    Title = "Резултати сграда"
                };
            }

            section.Type = ModelSectionType.Results;

            // Синхронизираме отопляема площ от ObjectData
            var objectDataSection = viewModel.CurrentReport?.Sections?.FirstOrDefault(s => s.Type == ModelSectionType.ObjectData);
            if (objectDataSection?.ObjectDataSectionData != null)
            {
                if (double.TryParse(objectDataSection.ObjectDataSectionData.HeatedArea, out double heatedArea))
                {
                    section.ResultsSectionData.HeatedArea = heatedArea;
                }
            }

            var resultsViewModel = new ViewModels.ResultsSectionViewModel(section.ResultsSectionData);
            var resultsEditor = new ResultsSectionEditor
            {
                DataContext = resultsViewModel
            };
            ContentScrollViewer.Content = resultsEditor;
        }
        else if (section.Type == ModelSectionType.EnergyClass ||
                 (!string.IsNullOrEmpty(section.Title) && section.Title.Contains("Клас на енергопотребление")))
        {
            if (section.EnergyClassSectionData == null)
            {
                section.EnergyClassSectionData = new Models.EnergyClassSectionData
                {
                    Title = "Клас на енергопотребление"
                };
            }

            section.Type = ModelSectionType.EnergyClass;

            // Създаваме и инициализираме ViewModel за автоматична синхронизация
            var energyClassViewModel = new ViewModels.EnergyClassViewModel();
            if (viewModel.CurrentReport != null)
            {
                energyClassViewModel.Initialize(viewModel.CurrentReport, section.EnergyClassSectionData);
            }

            var energyClassEditor = new EnergyClassSectionEditor
            {
                DataContext = section.EnergyClassSectionData
            };
            
            // Запазваме референция към ViewModel за cleanup при смяна на секция
            energyClassEditor.Tag = energyClassViewModel;
            
            ContentScrollViewer.Content = energyClassEditor;
        }
        else if (section.Type == ModelSectionType.Conclusion ||
                 (!string.IsNullOrEmpty(section.Title) && section.Title.Contains("Заключение")))
        {
            if (section.ConclusionSectionData == null)
            {
                section.ConclusionSectionData = new Models.ConclusionSectionData
                {
                    Title = "Заключение"
                };
            }

            section.Type = ModelSectionType.Conclusion;

            var conclusionViewModel = new ViewModels.ConclusionSectionViewModel(section.ConclusionSectionData);
            var conclusionEditor = new Views.ConclusionSectionEditor(conclusionViewModel);
            ContentScrollViewer.Content = conclusionEditor;
        }
        else if (section.Type == ModelSectionType.InternalGainsDebug ||
                 (!string.IsNullOrEmpty(section.Title) && section.Title.Contains("Дебъг вътрешни")))
        {
            if (section.InternalGainsDebugInput == null)
                section.InternalGainsDebugInput = new Models.InternalGainsDebugInput { ZoneId = 1 };

            section.Type = ModelSectionType.InternalGainsDebug;

            // Взимаме ObjectDataSectionData от Секция 5
            var objectDataSection = viewModel.CurrentReport?.Sections
                ?.FirstOrDefault(s => s.Type == ModelSectionType.ObjectData);
            var objectData = objectDataSection?.ObjectDataSectionData;

            var debugView = new Views.InternalGainsDebugView(
                section.InternalGainsDebugInput,
                objectData,
                viewModel.CurrentReport);
            ContentScrollViewer.Content = debugView;
        }
        else if (section.Type == ModelSectionType.SolarGains ||
                 (!string.IsNullOrEmpty(section.Title) && section.Title.Contains("слънчево греене")))
        {
            if (section.SolarGainsData == null)
                section.SolarGainsData = new EE.Doklad.Sections.Section24SolarGains.Models.Section24SolarGainsData();

            section.Type = ModelSectionType.SolarGains;

            var solarGainsVm = new EE.Doklad.Sections.Section24SolarGains.ViewModels.Section24ViewModel(
                section.SolarGainsData,
                viewModel.CurrentReport,
                autoSyncEnabled: true);
            var solarGainsView = new EE.Doklad.Sections.Section24SolarGains.Views.Section24View
            {
                DataContext = solarGainsVm
            };
            ContentScrollViewer.Content = solarGainsView;
        }
        else
        {
            // Ако е секция 4. Въведение, показваме IntroSectionEditor
            if (!string.IsNullOrEmpty(section.Title) && section.Title.Contains("Въведение"))
            {
                var introEditor = new Views.IntroSectionEditor
                {
                    DataContext = section
                };
                ContentScrollViewer.Content = introEditor;
            }
            else
            {
                // Показваме Normal Section Editor
                var normalEditor = CreateNormalSectionEditor(section);
                ContentScrollViewer.Content = normalEditor;
            }
        }
    }

    private FrameworkElement CreateNormalSectionEditor(ModelSection section)
    {
        var stackPanel = new StackPanel { Margin = new Thickness(20) };

        // Заглавие
        stackPanel.Children.Add(new TextBlock
        {
            Text = "Избрана секция",
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 0, 0, 10)
        });

        // Заглавие на секция
        stackPanel.Children.Add(new TextBlock { Text = "Заглавие:" });
        var titleTextBox = new TextBox { Margin = new Thickness(0, 5, 0, 15) };
        titleTextBox.SetBinding(TextBox.TextProperty, new Binding("Title")
        {
            Source = section,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });
        stackPanel.Children.Add(titleTextBox);

        // Статичен текст
        stackPanel.Children.Add(new TextBlock { Text = "Статичен текст:" });
        var staticTextBox = new TextBox
        {
            Height = 80,
            TextWrapping = TextWrapping.Wrap,
            AcceptsReturn = true,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Margin = new Thickness(0, 5, 0, 15)
        };
        staticTextBox.SetBinding(TextBox.TextProperty, new Binding("StaticText")
        {
            Source = section,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });
        stackPanel.Children.Add(staticTextBox);

        // Таблици заглавие
        stackPanel.Children.Add(new TextBlock
        {
            Text = "Таблици:",
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 10, 0, 10)
        });

        // Таблици ItemsControl
        foreach (var table in section.Tables)
        {
            var border = new Border
            {
                BorderBrush = new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 15)
            };

            var tableStack = new StackPanel();
            tableStack.Children.Add(new TextBlock
            {
                Text = table.Title,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 5)
            });

            var dataGrid = new DataGrid
            {
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                HeadersVisibility = DataGridHeadersVisibility.Column
            };
            dataGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding("Rows") { Source = table });

            // Динамично добавяме колони
            for (int i = 0; i < table.ColumnHeaders.Count && i < 3; i++)
            {
                bool isNumericColumn = table.Rows.Any(row => row.Cells.Count > i && row.Cells[i].Type == CellType.Number);

                var binding = new Binding($"Cells[{i}].Value")
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
                };

                if (isNumericColumn)
                {
                    binding.Converter = (IValueConverter?)Application.Current.TryFindResource("FlexibleDoubleConverter");
                    binding.StringFormat = "0.000";
                }

                dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = $"Кол. {i + 1}",
                    Binding = binding,
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star)
                });
            }

            tableStack.Children.Add(dataGrid);
            border.Child = tableStack;
            stackPanel.Children.Add(border);
        }

        return stackPanel;
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("EE Доклад v1.0\n\nПриложение за изготвяне на енергийни доклади.\n\n© 2026", 
            "За програмата", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
