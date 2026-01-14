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

        if (section.Type == ModelSectionType.CoverPage && section.CoverPageData != null)
        {
            // Показваме CoverPage Editor
            var coverPageEditor = new CoverPageEditor
            {
                DataContext = section.CoverPageData
            };
            ContentScrollViewer.Content = coverPageEditor;
        }
        else
        {
            // Показваме Normal Section Editor
            var normalEditor = CreateNormalSectionEditor(section);
            ContentScrollViewer.Content = normalEditor;
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