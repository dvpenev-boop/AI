using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.Views.FloorPanels
{
    public partial class FloorExternalAirPanel : UserControl
    {
        public FloorExternalAirPanel()
        {
            InitializeComponent();
        }

        private void AreaTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var binding = ((TextBox)sender).GetBindingExpression(System.Windows.Controls.TextBox.TextProperty);
            binding?.UpdateSource();
            if (DataContext is FloorExternalAirDetail detail)
            {
                System.Diagnostics.Debug.WriteLine($"[AreaTextBox_LostFocus] Area={detail.Area}");
            }
        }

        private void AreaTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Tab)
            {
                var binding = ((TextBox)sender).GetBindingExpression(System.Windows.Controls.TextBox.TextProperty);
                binding?.UpdateSource();
                if (DataContext is FloorExternalAirDetail detail)
                {
                    System.Diagnostics.Debug.WriteLine($"[AreaTextBox_KeyDown] Area={detail.Area}");
                }
            }
        }

        // DependencyProperty to receive the parent FloorItem from the containing view
        public static readonly DependencyProperty ParentFloorItemProperty = DependencyProperty.Register(
            nameof(ParentFloorItem), typeof(EE.Doklad.Models.FloorItem), typeof(FloorExternalAirPanel), new PropertyMetadata(null));

        public EE.Doklad.Models.FloorItem? ParentFloorItem
        {
            get => (EE.Doklad.Models.FloorItem?)GetValue(ParentFloorItemProperty);
            set => SetValue(ParentFloorItemProperty, value);
        }

        public static string? SelectImageFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Изображения|*.png;*.jpg;*.jpeg",
                Title = "Изберете изображение"
            };
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public static void LoadImage(AttachmentData attachment, string filePath)
        {
            try
            {
                ((EE.Doklad.Models.AttachmentData)attachment).AttachmentFileName = System.IO.Path.GetFileName(filePath);
                ((EE.Doklad.Models.AttachmentData)attachment).Data = System.IO.File.ReadAllBytes(filePath);
                var ext = System.IO.Path.GetExtension(filePath).ToLowerInvariant();
                ((EE.Doklad.Models.AttachmentData)attachment).MimeType = ext switch
                {
                    ".png" => "image/png",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    _ => "application/octet-stream"
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Грешка при зареждане: {ex.Message}",
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UploadScheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not FloorExternalAirDetail detail)
                return;
            var filePath = SelectImageFile();
            if (filePath == null)
                return;
            var newAttachment = new AttachmentData();
            LoadImage(newAttachment, filePath);
            detail.SchemeAttachment = newAttachment;
        }

        public void RemoveScheme_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not FloorExternalAirDetail detail)
                return;
            detail.SchemeAttachment = new AttachmentData();
        }


        public void AddLayer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensure Area textbox binding is committed before modifying layers
                var tb = this.FindName("AreaTextBox") as System.Windows.Controls.TextBox;
                var be = tb?.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty);
                be?.UpdateSource();

                if (DataContext is EE.Doklad.Models.FloorExternalAirDetail detail)
                {
                    detail.Layers.Add(new FloorLayer());
                    System.Diagnostics.Debug.WriteLine($"[AddLayer_Click] Layer added. Total layers: {detail.Layers.Count}");
                    // ViewModel automatically recalculates via subscription
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AddLayer_Click] Exception: {ex}");
                MessageBox.Show($"Грешка при добавяне на слой: {ex.Message}", "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RemoveLayer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensure Area textbox binding is committed before modifying layers
                var tb = this.FindName("AreaTextBox") as System.Windows.Controls.TextBox;
                var be = tb?.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty);
                be?.UpdateSource();

                if (DataContext is EE.Doklad.Models.FloorExternalAirDetail detail && detail.Layers.Count > 0)
                {
                    detail.Layers.RemoveAt(detail.Layers.Count - 1);
                    System.Diagnostics.Debug.WriteLine($"[RemoveLayer_Click] Layer removed. Total layers: {detail.Layers.Count}");
                    // ViewModel automatically recalculates via subscription
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RemoveLayer_Click] Exception: {ex}");
                MessageBox.Show($"Грешка при премахване на слой: {ex.Message}", "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LayersGrid_BeginningEdit(object sender, System.Windows.Controls.DataGridBeginningEditEventArgs e)
        {
            try
            {
                // When user starts editing a layer cell (tabbed into grid), commit the Area textbox first
                var tb = this.FindName("AreaTextBox") as System.Windows.Controls.TextBox;
                var be = tb?.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty);
                be?.UpdateSource();
                if (DataContext is FloorExternalAirDetail detail)
                {
                    System.Diagnostics.Debug.WriteLine($"[LayersGrid_BeginningEdit] Area={detail.Area}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LayersGrid_BeginningEdit] Exception: {ex}");
            }
        }

        private void LayersGrid_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[LayersGrid_CellEditEnding] Cell edit ending for column: {e.Column.Header}");
                // ViewModel automatically recalculates via layer property changed subscriptions
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LayersGrid_CellEditEnding] Exception: {ex}");
            }
        }

        public FloorExternalAirInput GetInput()
        {
            return DataContext as FloorExternalAirInput ?? new FloorExternalAirInput();
        }
    }
}
