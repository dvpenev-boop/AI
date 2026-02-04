using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;
using EE.Doklad.Tests;
using EE.Doklad.ViewModels;
using Microsoft.Win32;

namespace EE.Doklad.Views
{
    /// <summary>
    /// Interaction logic for UnconditionedZonesSectionView.xaml
    /// </summary>
    public partial class UnconditionedZonesSectionView : UserControl
    {
        public UnconditionedZonesSectionView()
        {
            InitializeComponent();
        }

        private void RunTest_Click(object sender, RoutedEventArgs e)
        {
            // Стартираме теста и показваме резултатите в конзолата
            System.Diagnostics.Debug.WriteLine("========================================");
            System.Diagnostics.Debug.WriteLine("Стартиране на тест за неклиматизирани зони");
            System.Diagnostics.Debug.WriteLine("========================================");

            try
            {
                UnconditionedZonesCalculatorTest.RunTest();
                
                MessageBox.Show(
                    "Тестът завърши успешно!\n\nПроверете Output прозореца (Debug) за детайлни резултати.",
                    "Тест на изчисления",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    $"Грешка при изпълнение на теста:\n\n{ex.Message}",
                    "Грешка в теста",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void AddLayer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not ZtuElement element) 
                return;

            if (DataContext is not UnconditionedZonesSectionViewModel vm) 
                return;

            var layer = new ZtuLayer 
            { 
                MaterialName = "Избери материал",
                Thickness = 100.0,
                Lambda = 1.0,
                MaterialOptions = vm.MaterialOptions.ToList()
            };

            element.Layers.Add(layer);
        }

        private void RemoveLayer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not ZtuElement element) 
                return;

            if (element.Layers.Count > 0)
            {
                element.Layers.RemoveAt(element.Layers.Count - 1);
            }
        }

        private void UploadScheme_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not ZtuElement element) 
                return;

            var dialog = new OpenFileDialog
            {
                Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp;*.gif",
                Title = "Изберете изображение на схемата"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var attachment = new AttachmentData
                    {
                        FileName = Path.GetFileName(dialog.FileName),
                        Data = File.ReadAllBytes(dialog.FileName)
                    };

                    element.SchemeAttachment = attachment;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Грешка при зареждане на изображението:\n\n{ex.Message}",
                        "Грешка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void RemoveScheme_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not ZtuElement element) 
                return;

            var result = MessageBox.Show(
                "Сигурни ли сте, че искате да премахнете схемата?",
                "Потвърждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                element.SchemeAttachment = null;
            }
        }
    }
}
