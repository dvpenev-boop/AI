using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.Views
{
    public partial class LightingSectionEditor : UserControl
    {
        private readonly LightingService _lightingService;

        public ObservableCollection<LightingRow> LightingOptions { get; } = new();

        public LightingSectionEditor()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;

            // Initialize lighting service
            _lightingService = new LightingService(new JsonLightingRepository());
            LoadLightingOptions();
        }

        private void LoadLightingOptions()
        {
            LightingOptions.Clear();
            var options = _lightingService.GetCombinedRows(includeSeed: true, includeUser: true);
            foreach (var option in options)
            {
                LightingOptions.Add(option);
            }
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Ако има нужда от допълнителна инициализация при смяна на DataContext
        }

        private void AddLineItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not LightingSectionData data)
            {
                return;
            }

            var newItem = new LightingLineItem
            {
                Index = data.LineItems.Count + 1,
                SelectedLightingComponentName = null,
                PowerW = 0,
                Quantity = 1,
                HoursPerDay = 5.0,
                DaysPerWeek = 5.0,
                Ke = 0.6
            };

            data.LineItems.Add(newItem);
            UpdateIndexes(data);
        }

        private void RemoveLineItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not LightingSectionData data)
            {
                return;
            }

            if (LightingGrid.SelectedItem is LightingLineItem selectedItem)
            {
                data.LineItems.Remove(selectedItem);
                UpdateIndexes(data);
                return;
            }

            // Ако няма селектиран ред, премахваме последния
            if (data.LineItems.Any())
            {
                data.LineItems.RemoveAt(data.LineItems.Count - 1);
                UpdateIndexes(data);
            }
        }

        private void UpdateIndexes(LightingSectionData data)
        {
            for (int i = 0; i < data.LineItems.Count; i++)
            {
                data.LineItems[i].Index = i + 1;
            }
        }

        private void LightingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ComboBox comboBox)
                return;

            if (comboBox.Tag is not LightingLineItem lineItem)
                return;

            // Когато се избере осветител, автоматично попълваме PowerW
            if (comboBox.SelectedItem is LightingRow selectedRow)
            {
                lineItem.PowerW = selectedRow.PowerW;
            }
        }
    }
}
