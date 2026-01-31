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
    public partial class AppliancesSectionEditor : UserControl
    {
        private readonly ApplianceService _applianceService;

        public ObservableCollection<ApplianceRow> AppliancesOptions { get; } = new();

        public AppliancesSectionEditor()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;

            // Initialize appliance service
            _applianceService = new ApplianceService(new JsonApplianceRepository());
            LoadApplianceOptions();
        }

        private void LoadApplianceOptions()
        {
            AppliancesOptions.Clear();
            var options = _applianceService.GetCombinedRows(includeSeed: true, includeUser: true);
            foreach (var option in options)
            {
                AppliancesOptions.Add(option);
            }
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Ако има нужда от допълнителна инициализация при смяна на DataContext
        }

        private void AddLineItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not AppliancesSectionData data)
            {
                return;
            }

            var newItem = new AppliancesLineItem
            {
                Index = data.LineItems.Count + 1,
                SelectedApplianceName = null,
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
            if (DataContext is not AppliancesSectionData data)
            {
                return;
            }

            if (AppliancesGrid.SelectedItem is AppliancesLineItem selectedItem)
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

        private void UpdateIndexes(AppliancesSectionData data)
        {
            for (int i = 0; i < data.LineItems.Count; i++)
            {
                data.LineItems[i].Index = i + 1;
            }
        }

        private void ApplianceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ComboBox comboBox)
                return;

            if (comboBox.Tag is not AppliancesLineItem lineItem)
                return;

            // Когато се избере уред, автоматично попълваме PowerW
            if (comboBox.SelectedItem is ApplianceRow selectedRow)
            {
                lineItem.PowerW = selectedRow.PowerW;
            }
        }
    }
}
