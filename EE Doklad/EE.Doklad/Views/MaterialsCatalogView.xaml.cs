using System.Linq;
using System.Windows.Controls;
using EE.Doklad.ViewModels;
using EE.Doklad.Models;
using EE.Doklad.Views;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace EE.Doklad.Views
{
    public partial class MaterialsCatalogView : UserControl
    {
        public MaterialsCatalogView()
        {
            InitializeComponent();
            Loaded += MaterialsCatalogView_Loaded;
        }

        private void MaterialsCatalogView_Loaded(object? sender, System.Windows.RoutedEventArgs e)
        {
            var editBtn = this.FindName("EditButton") as System.Windows.Controls.Button;
            if (editBtn != null)
                editBtn.Click += EditButton_Click;

            var importBtn = this.FindName("ImportButton") as System.Windows.Controls.Button;
            if (importBtn != null)
                importBtn.Click += ImportButton_Click;

            var exportBtn = this.FindName("ExportButton") as System.Windows.Controls.Button;
            if (exportBtn != null)
                exportBtn.Click += ExportButton_Click;
        }

        private void ExportButton_Click(object? sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is not MaterialsCatalogViewModel vm) return;

            var dlg = new SaveFileDialog
            {
                Title = "Експортиране на потребителски материали",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = "json",
                FileName = "materials.user.export.json"
            };

            if (dlg.ShowDialog() != true) return;

            var users = vm.Service.GetUser();
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() },
                Formatting = Formatting.Indented
            };

            var json = JsonConvert.SerializeObject(users, settings);
            File.WriteAllText(dlg.FileName, json);
        }

        private void ImportButton_Click(object? sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is not MaterialsCatalogViewModel vm) return;

            var dlg = new OpenFileDialog
            {
                Title = "Импортиране на потребителски материали",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = "json"
            };

            if (dlg.ShowDialog() != true) return;

            var json = File.ReadAllText(dlg.FileName);
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() }
            };

            try
            {
                var imported = JsonConvert.DeserializeObject<System.Collections.Generic.List<BuildingMaterialUser>>(json, settings);
                if (imported == null || imported.Count == 0) return;

                // Ask user for merge strategy
                var opt = new ImportOptionsWindow { Owner = System.Windows.Window.GetWindow(this) };
                var res = opt.ShowDialog();
                if (res != true) return;

                var strategy = opt.SelectedStrategy;

                var existing = vm.Service.GetUser().ToList();

                int added = 0, skipped = 0, overwritten = 0, replaced = 0;

                if (strategy == EE.Doklad.Services.ImportMergeStrategy.ReplaceAll)
                {
                    // remove all existing user materials
                    foreach (var exm in existing.ToList())
                        vm.Service.DeleteUserMaterial(exm.Id);

                    // add all imported
                    foreach (var im in imported)
                    {
                        im.Id = string.Empty;
                        if (im.Variants != null)
                            foreach (var v in im.Variants) v.Id = string.Empty;
                        vm.Service.AddUserMaterial(im);
                        added++;
                    }
                    replaced = added;
                }
                else
                {
                    // Build lookup by name (case-insensitive). Use grouping to avoid exceptions when
                    // there are multiple existing user records with the same name.
                    var map = existing.GroupBy(x => (x.NameBg ?? string.Empty).Trim().ToLowerInvariant())
                                      .ToDictionary(g => g.Key, g => g.ToList());

                    foreach (var im in imported)
                    {
                        var nameKey = (im.NameBg ?? string.Empty).Trim().ToLowerInvariant();
                        if (map.TryGetValue(nameKey, out var existList) && existList.Count > 0)
                        {
                            // Determine if any existing entry is technically identical to the imported one.
                            bool hasIdentical = existList.Any(e => VariantsEqual(e.Variants, im.Variants));

                            if (strategy == EE.Doklad.Services.ImportMergeStrategy.MergeSkipDuplicates)
                            {
                                // Skip only when an identical technical record already exists. If the name
                                // is the same but technical values differ, treat as a new record and add it.
                                if (hasIdentical)
                                {
                                    skipped++;
                                    continue;
                                }
                                else
                                {
                                    im.Id = string.Empty;
                                    if (im.Variants != null)
                                        foreach (var v in im.Variants) v.Id = string.Empty;
                                    vm.Service.AddUserMaterial(im);
                                    added++;
                                    continue;
                                }
                            }
                            else if (strategy == EE.Doklad.Services.ImportMergeStrategy.MergeOverwriteDuplicates)
                            {
                                // Overwrite the first existing entry with the same name.
                                var exist = existList.First();
                                im.Id = exist.Id;
                                if (im.Variants != null)
                                    foreach (var v in im.Variants) v.Id = string.Empty;
                                vm.Service.UpdateUserMaterial(im);
                                overwritten++;
                                continue;
                            }
                        }

                        // No existing with same name -> add as new
                        im.Id = string.Empty;
                        if (im.Variants != null)
                            foreach (var v in im.Variants) v.Id = string.Empty;
                        vm.Service.AddUserMaterial(im);
                        added++;
                    }
                }

                vm.RefreshCommand.Execute(null);

                System.Windows.MessageBox.Show($"Импорт приключи. Добавени: {added}, Повтарящи се записи (пропуснати): {skipped}, Повтарящи се записи (презаписани): {overwritten}, Заменени (replace all): {replaced}", "Импорт", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show("Грешка при импортиране: " + ex.Message, "Грешка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private static bool VariantsEqual(System.Collections.Generic.List<EE.Doklad.Models.BuildingMaterialVariantUser>? a, System.Collections.Generic.List<EE.Doklad.Models.BuildingMaterialVariantUser>? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (a.Count != b.Count) return false;

            const double eps = 1e-6;
            for (int i = 0; i < a.Count; i++)
            {
                var va = a[i];
                var vb = b[i];

                if (!NullableDoubleEqual(va.RhoKgM3, vb.RhoKgM3, eps)) return false;
                if (!NullableDoubleEqual(va.CJKgK, vb.CJKgK, eps)) return false;
                if (!NullableDoubleEqual(va.LambdaWMK, vb.LambdaWMK, eps)) return false;
                if (!NullableDoubleEqual(va.Mu, vb.Mu, eps)) return false;
            }

            return true;
        }

        private static bool NullableDoubleEqual(double? x, double? y, double eps)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return System.Math.Abs(x.Value - y.Value) <= eps;
        }

        private void EditButton_Click(object? sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is not MaterialsCatalogViewModel vm) return;
            var selected = vm.Selected;
            if (selected == null) return;
            if (selected.IsSeed) return; // only user materials

            // Find the underlying user model
            var user = vm.Service.GetUser().FirstOrDefault(x => x.Id == selected.Id);
            if (user == null) return;

            var win = new MaterialsEditWindow(vm.Service, user);
            var res = win.ShowDialog();
            if (res == true)
            {
                vm.RefreshCommand.Execute(null);
            }
        }

        private void DataGrid_RowEditEnding(object sender, System.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != System.Windows.Controls.DataGridEditAction.Commit) return;

            if (e.Row.Item is not EE.Doklad.Models.BuildingMaterialRow row) return;
            if (row.IsSeed) return; // only persist user edits

            if (DataContext is not MaterialsCatalogViewModel vm) return;

            var user = vm.Service.GetUser().FirstOrDefault(x => x.Id == row.Id);
            if (user == null) return;

            // Ensure at least one variant
            if (user.Variants == null || user.Variants.Count == 0)
            {
                user.Variants = new System.Collections.Generic.List<EE.Doklad.Models.BuildingMaterialVariantUser>
                {
                    new EE.Doklad.Models.BuildingMaterialVariantUser()
                };
            }

            // Copy edited preview values to the user's first variant
            user.NameBg = row.NameBg;
            var v0 = user.Variants[0];
            v0.RhoKgM3 = row.RhoKgM3;
            v0.CJKgK = row.CJKgK;
            v0.LambdaWMK = row.LambdaWMK;
            v0.Mu = row.Mu;

            vm.Service.UpdateUserMaterial(user);
            // refresh to make sure UI reflects any normalization
            vm.RefreshCommand.Execute(null);
        }
    }
}
