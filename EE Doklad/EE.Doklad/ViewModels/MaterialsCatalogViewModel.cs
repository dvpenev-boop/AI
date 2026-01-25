using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.ViewModels
{
    public partial class MaterialsCatalogViewModel : ObservableObject
    {
        public MaterialsCatalogViewModel() : this(new MaterialsService(new JsonMaterialsRepository()))
        {
        }

        public MaterialsCatalogViewModel(MaterialsService service)
        {
            Service = service;
            Refresh();
        }

        public MaterialsService Service { get; }

        public ObservableCollection<BuildingMaterialRow> Materials { get; } = new();

        [ObservableProperty]
        private BuildingMaterialRow? _selected;

        [RelayCommand]
        private void Refresh()
        {
            Materials.Clear();
            foreach (var row in Service.GetCombinedRows())
                Materials.Add(row);
        }

        [RelayCommand(CanExecute = nameof(CanEditOrDeleteSelected))]
        private void DeleteSelected()
        {
            if (Selected == null) return;
            if (Selected.IsSeed) return;

            Service.DeleteUserMaterial(Selected.Id);
            Refresh();
        }

        private bool CanEditOrDeleteSelected() => Selected != null && !Selected.IsSeed;

        partial void OnSelectedChanged(BuildingMaterialRow? value)
        {
            DeleteSelectedCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void AddUserMaterial()
        {
            // Minimal add: create a new user material with one variant.
            var mat = new BuildingMaterialUser
            {
                Id = "",
                NameBg = "Нов материал",
                Variants =
                {
                    new BuildingMaterialVariantUser
                    {
                        Id = "",
                        RhoKgM3 = null,
                        CJKgK = null,
                        LambdaWMK = null,
                        Mu = null
                    }
                }
            };

            Service.AddUserMaterial(mat);
            Refresh();
            Selected = Materials.FirstOrDefault(x => x.Id == mat.Id);
        }
    }
}
