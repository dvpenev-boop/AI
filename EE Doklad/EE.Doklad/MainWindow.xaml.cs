using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
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

            var externalWallsEditor = new ExternalWallsSectionEditor
            {
                DataContext = section.ExternalWallsSectionData
            };
            ContentScrollViewer.Content = externalWallsEditor;
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
                dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = $"Кол. {i + 1}",
                    Binding = new Binding($"Cells[{i}].Value"),
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