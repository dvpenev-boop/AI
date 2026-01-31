using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using EE.Doklad.Models;
using EE.Doklad.ViewModels;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace EE.Doklad.Views
{
    public partial class AppliancesCatalogView : UserControl
    {
        public AppliancesCatalogView()
        {
            InitializeComponent();
            Loaded += AppliancesCatalogView_Loaded;
        }

        private void AppliancesCatalogView_Loaded(object? sender, System.Windows.RoutedEventArgs e)
        {
            var importBtn = this.FindName("ImportButton") as Button;
            if (importBtn != null)
                importBtn.Click += ImportButton_Click;

            var exportBtn = this.FindName("ExportButton") as Button;
            if (exportBtn != null)
                exportBtn.Click += ExportButton_Click;
        }

        private void ExportButton_Click(object? sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is not ApplianceCatalogViewModel vm) return;

            var dlg = new SaveFileDialog
            {
                Title = "Експортиране на потребителски електрически уреди",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = "json",
                FileName = "appliances.user.export.json"
            };

            if (dlg.ShowDialog() != true) return;

            var users = vm.Service.GetUser();
            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(dlg.FileName, json);
        }

        private void ImportButton_Click(object? sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is not ApplianceCatalogViewModel vm) return;

            var dlg = new OpenFileDialog
            {
                Title = "Импортиране на потребителски електрически уреди",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = "json"
            };

            if (dlg.ShowDialog() != true) return;

            var json = File.ReadAllText(dlg.FileName);

            try
            {
                var imported = JsonConvert.DeserializeObject<List<ApplianceUser>>(json);
                if (imported == null || imported.Count == 0) return;

                var existing = vm.Service.GetUser().ToList();
                int added = 0;

                foreach (var item in imported)
                {
                    item.Id = string.Empty;
                    vm.Service.AddUserItem(item);
                    added++;
                }

                vm.RefreshCommand.Execute(null);

                System.Windows.MessageBox.Show($"Импорт приключи. Добавени: {added}", 
                    "Импорт", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show("Грешка при импортиране: " + ex.Message, 
                    "Грешка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit) return;
            if (e.Row.Item is not ApplianceRow row) return;
            if (row.IsSeed) return;

            if (DataContext is not ApplianceCatalogViewModel vm) return;

            var user = vm.Service.GetUser().FirstOrDefault(x => x.Id == row.Id);
            if (user == null) return;

            user.Name = row.Name;
            user.PowerW = row.PowerW;

            vm.Service.UpdateUserItem(user);
            vm.RefreshCommand.Execute(null);
        }
    }
}
