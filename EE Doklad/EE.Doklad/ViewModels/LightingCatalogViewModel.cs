using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.ViewModels
{
    public partial class LightingCatalogViewModel : ObservableObject
    {
        public LightingCatalogViewModel() : this(new LightingService(new JsonLightingRepository()))
        {
        }

        public LightingCatalogViewModel(LightingService service)
        {
            Service = service;
            Refresh();
        }

        public LightingService Service { get; }

        public ObservableCollection<LightingRow> Items { get; } = new();

        [ObservableProperty]
        private bool _showSeed = true;

        [ObservableProperty]
        private bool _showUser = true;

        [ObservableProperty]
        private LightingRow? _selected;

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

        partial void OnSelectedChanged(LightingRow? value)
        {
            DeleteSelectedCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void AddUserItem()
        {
            var item = new LightingUser
            {
                Id = "",
                Name = "Ново осветително тяло",
                PowerW = 0
            };

            Service.AddUserItem(item);
            Refresh();
            Selected = Items.FirstOrDefault(x => x.Id == item.Id);
        }
    }
}
