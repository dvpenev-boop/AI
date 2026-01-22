using System;
using System.Windows;
using EE.Doklad.Models;
using System.Diagnostics;

namespace EE.Doklad.Views
{
    public partial class FloorTypeSelectionDialog : Window
    {
        public FloorType SelectedFloorType { get; private set; }

        public FloorTypeSelectionDialog()
        {
            Debug.WriteLine("[FloorTypeSelectionDialog] Constructor called");
            InitializeComponent();
            Debug.WriteLine("[FloorTypeSelectionDialog] InitializeComponent completed");
        }

        private void ExternalAir_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("[FloorTypeSelectionDialog] ExternalAir button clicked");
            try
            {
                SelectedFloorType = FloorType.ExternalAir;
                Debug.WriteLine($"[FloorTypeSelectionDialog] SelectedFloorType set to: {SelectedFloorType}");
                DialogResult = true;
                Debug.WriteLine("[FloorTypeSelectionDialog] DialogResult set to true");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FloorTypeSelectionDialog] ERROR in ExternalAir_Click: {ex.Message}");
                Debug.WriteLine($"[FloorTypeSelectionDialog] Stack trace: {ex.StackTrace}");
                MessageBox.Show($"Грешка: {ex.Message}", "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Ground_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("[FloorTypeSelectionDialog] Ground button clicked");
            try
            {
                SelectedFloorType = FloorType.Ground;
                Debug.WriteLine($"[FloorTypeSelectionDialog] SelectedFloorType set to: {SelectedFloorType}");
                DialogResult = true;
                Debug.WriteLine("[FloorTypeSelectionDialog] DialogResult set to true");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FloorTypeSelectionDialog] ERROR in Ground_Click: {ex.Message}");
                Debug.WriteLine($"[FloorTypeSelectionDialog] Stack trace: {ex.StackTrace}");
                MessageBox.Show($"Грешка: {ex.Message}", "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UnheatedBasement_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("[FloorTypeSelectionDialog] UnheatedBasement button clicked");
            try
            {
                SelectedFloorType = FloorType.UnheatedBasement;
                Debug.WriteLine($"[FloorTypeSelectionDialog] SelectedFloorType set to: {SelectedFloorType}");
                DialogResult = true;
                Debug.WriteLine("[FloorTypeSelectionDialog] DialogResult set to true");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FloorTypeSelectionDialog] ERROR in UnheatedBasement_Click: {ex.Message}");
                Debug.WriteLine($"[FloorTypeSelectionDialog] Stack trace: {ex.StackTrace}");
                MessageBox.Show($"Грешка: {ex.Message}", "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HeatedBasement_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("[FloorTypeSelectionDialog] HeatedBasement button clicked");
            try
            {
                SelectedFloorType = FloorType.HeatedBasement;
                Debug.WriteLine($"[FloorTypeSelectionDialog] SelectedFloorType set to: {SelectedFloorType}");
                DialogResult = true;
                Debug.WriteLine("[FloorTypeSelectionDialog] DialogResult set to true");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FloorTypeSelectionDialog] ERROR in HeatedBasement_Click: {ex.Message}");
                Debug.WriteLine($"[FloorTypeSelectionDialog] Stack trace: {ex.StackTrace}");
                MessageBox.Show($"Грешка: {ex.Message}", "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("[FloorTypeSelectionDialog] Cancel button clicked");
            DialogResult = false;
            Debug.WriteLine("[FloorTypeSelectionDialog] DialogResult set to false");
        }
    }
}
