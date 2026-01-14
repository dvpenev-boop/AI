using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;
using Microsoft.Win32;

namespace EE.Doklad.Views
{
    public partial class CoverPageEditor : UserControl
    {
        private CoverPageData? _coverPageData;

        public CoverPageEditor()
        {
            InitializeComponent();
            DataContextChanged += CoverPageEditor_DataContextChanged;
        }

        private void CoverPageEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _coverPageData = DataContext as CoverPageData;
            
            if (_coverPageData != null)
            {
                // Зареждане на лого ако има
                LoadLogo();
                
                // Синхронизиране на ComboBox с моделa
                PhaseComboBox.SelectedIndex = (int)_coverPageData.Phase;
            }
        }

        private void LoadLogo()
        {
            if (_coverPageData == null) return;

            if (!string.IsNullOrEmpty(_coverPageData.LogoPath) && System.IO.File.Exists(_coverPageData.LogoPath))
            {
                try
                {
                    var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new System.Uri(_coverPageData.LogoPath, System.UriKind.Absolute);
                    bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    
                    LogoImage.Source = bitmap;
                    LogoPlaceholder.Visibility = Visibility.Collapsed;
                }
                catch
                {
                    // Ако файлът е изтрит или има грешка
                    LogoImage.Source = null;
                    LogoPlaceholder.Visibility = Visibility.Visible;
                }
            }
            else
            {
                LogoImage.Source = null;
                LogoPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void SelectLogo_Click(object sender, RoutedEventArgs e)
        {
            _coverPageData = DataContext as CoverPageData;
            
            if (_coverPageData == null)
            {
                MessageBox.Show("Данните за челната страница не са заредени.", "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dialog = new OpenFileDialog
            {
                Filter = "Изображения (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|Всички файлове (*.*)|*.*",
                Title = "Изберете лого"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _coverPageData.LogoPath = dialog.FileName;
                    LoadLogo();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Грешка при зареждане на изображение: {ex.Message}", "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RemoveLogo_Click(object sender, RoutedEventArgs e)
        {
            _coverPageData = DataContext as CoverPageData;
            
            if (_coverPageData == null) return;

            _coverPageData.LogoPath = null;
            LoadLogo();
        }

        private void PhaseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _coverPageData = DataContext as CoverPageData;
            
            if (_coverPageData == null || PhaseComboBox.SelectedItem == null) return;

            var selectedItem = PhaseComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem?.Tag is string tag)
            {
                _coverPageData.Phase = tag switch
                {
                    "Ideynyi" => ProjectPhase.Ideynyi,
                    "Tehnicheski" => ProjectPhase.Tehnicheski,
                    "Raboten" => ProjectPhase.Raboten,
                    _ => ProjectPhase.Tehnicheski
                };
            }
        }

        private void AddDeveloper_Click(object sender, RoutedEventArgs e)
        {
            _coverPageData = DataContext as CoverPageData;
            
            if (_coverPageData == null)
            {
                MessageBox.Show("Данните за челната страница не са заредени.", "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _coverPageData.Developers.Add(new Developer 
                { 
                    Name = "", 
                    Position = "Енергиен експерт" 
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Грешка при добавяне на разработил: {ex.Message}", "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveDeveloper_Click(object sender, RoutedEventArgs e)
        {
            _coverPageData = DataContext as CoverPageData;
            
            if (_coverPageData == null) return;

            var button = sender as Button;
            var developer = button?.Tag as Developer;
            
            if (developer != null)
            {
                // Не позволяваме изтриване, ако има само 1 разработил
                if (_coverPageData.Developers.Count <= 1)
                {
                    MessageBox.Show(
                        "Трябва да има поне един разработил в екипа.",
                        "Внимание",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                _coverPageData.Developers.Remove(developer);
            }
        }
    }
}
