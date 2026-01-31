using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.ViewModels
{
    public partial class ApplianceCatalogViewModel : ObservableObject
    {
        public ApplianceCatalogViewModel() : this(new ApplianceService(new JsonApplianceRepository()))
        {
        }

        public ApplianceCatalogViewModel(ApplianceService service)
        {
            Service = service;
            Refresh();
        }

        public ApplianceService Service { get; }

        public ObservableCollection<ApplianceRow> Items { get; } = new();

        [ObservableProperty]
        private bool _showSeed = true;

        [ObservableProperty]
        private bool _showUser = true;

        [ObservableProperty]
        private ApplianceRow? _selected;

        [RelayCommand]
        private void Refresh()
        {
            Items.Clear();
            foreach (var row in Service.GetCombinedRows(ShowSeed, ShowUser))
                Items.Add(row);
        }

        partial void OnShowSeedChanged(bool value)
        {
            RefreshCommand.Execute(null);
        }

        partial void OnShowUserChanged(bool value)
        {
            RefreshCommand.Execute(null);
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDeleteSelected))]
        private void DeleteSelected()
        {
            if (Selected == null) return;
            if (Selected.IsSeed) return;

            Service.DeleteUserItem(Selected.Id);
            Refresh();
        }

        private bool CanEditOrDeleteSelected() => Selected != null && !Selected.IsSeed;

        partial void OnSelectedChanged(ApplianceRow? value)
        {
            DeleteSelectedCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void AddUserItem()
        {
            var item = new ApplianceUser
            {
                Id = "",
                Name = "Нов уред",
                PowerW = 0
            };

            Service.AddUserItem(item);
            Refresh();
            Selected = Items.FirstOrDefault(x => x.Id == item.Id);
        }
    }
}
