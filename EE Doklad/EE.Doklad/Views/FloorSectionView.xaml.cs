using System.Windows;
using System.Windows.Controls;
using EE.Doklad.ViewModels;
using System.Diagnostics;

namespace EE.Doklad.Views
{
    public partial class FloorSectionView : UserControl
    {
        public FloorSectionView()
        {
            Debug.WriteLine("[FloorSectionView] Constructor called");
            InitializeComponent();
            Debug.WriteLine("[FloorSectionView] InitializeComponent completed");
        }

        private void AddFloor_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("[FloorSectionView] AddFloor_Click called");
            
            try
            {
                var viewModel = DataContext as FloorSectionViewModel;
                if (viewModel == null)
                {
                    Debug.WriteLine("[FloorSectionView] ERROR: ViewModel is null!");
                    MessageBox.Show("Грешка: ViewModel не е инициализиран.", "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Debug.WriteLine("[FloorSectionView] ViewModel found, creating dialog");
                var dialog = new FloorTypeSelectionDialog
                {
                    Owner = Window.GetWindow(this)
                };

                Debug.WriteLine("[FloorSectionView] Showing dialog");
                var result = dialog.ShowDialog();
                Debug.WriteLine($"[FloorSectionView] Dialog result: {result}");

                if (result == true)
                {
                    Debug.WriteLine($"[FloorSectionView] User selected floor type: {dialog.SelectedFloorType}");
                    
                    Debug.WriteLine("[FloorSectionView] Calling TryAddFloor");
                    if (viewModel.TryAddFloor(dialog.SelectedFloorType, out string? error))
                    {
                        Debug.WriteLine("[FloorSectionView] Floor added successfully");
                    }
                    else
                    {
                        Debug.WriteLine($"[FloorSectionView] Failed to add floor: {error}");
                        MessageBox.Show(error ?? "Грешка при добавяне на под.", "Грешка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    Debug.WriteLine("[FloorSectionView] User cancelled dialog");
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"[FloorSectionView] CRITICAL ERROR in AddFloor_Click: {ex.Message}");
                Debug.WriteLine($"[FloorSectionView] Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"[FloorSectionView] Inner exception: {ex.InnerException.Message}");
                    Debug.WriteLine($"[FloorSectionView] Inner stack trace: {ex.InnerException.StackTrace}");
                }
                MessageBox.Show($"Критична грешка: {ex.Message}\n\nПроверете Output прозореца за детайли.", "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
