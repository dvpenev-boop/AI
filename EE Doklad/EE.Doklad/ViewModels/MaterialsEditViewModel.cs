using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.ViewModels
{
    public partial class MaterialsEditViewModel : ObservableObject
    {
        private readonly MaterialsService _service;

        public MaterialsEditViewModel(MaterialsService service, BuildingMaterialUser material)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            Material = material ?? new BuildingMaterialUser { Id = string.Empty };
            Variants = new ObservableCollection<BuildingMaterialVariantUser>(Material.Variants ?? Enumerable.Empty<BuildingMaterialVariantUser>());
        }

        public BuildingMaterialUser Material { get; }

        public ObservableCollection<BuildingMaterialVariantUser> Variants { get; }

        [ObservableProperty]
        private BuildingMaterialVariantUser? _selectedVariant;

        [RelayCommand]
        private void AddVariant()
        {
            var v = new BuildingMaterialVariantUser { Id = string.Empty, RhoKgM3 = null, CJKgK = null, LambdaWMK = null, Mu = null };
            Variants.Add(v);
        }

        [RelayCommand(CanExecute = nameof(CanRemoveVariant))]
        private void RemoveVariant()
        {
            if (SelectedVariant != null)
                Variants.Remove(SelectedVariant);
        }

        private bool CanRemoveVariant() => SelectedVariant != null;

        public bool Validate(out string? error)
        {
            if (string.IsNullOrWhiteSpace(Material.NameBg))
            {
                error = "Името е задължително.";
                return false;
            }

            if (Variants.Count == 0)
            {
                error = "Трябва да има поне един вариант.";
                return false;
            }

            // commit variants into model
            Material.Variants = Variants.ToList();
            error = null;
            return true;
        }

        public void Save()
        {
            // If it's a new material (no id) call Add, otherwise Update.
            if (string.IsNullOrWhiteSpace(Material.Id))
                _service.AddUserMaterial(Material);
            else
                _service.UpdateUserMaterial(Material);
        }
    }
}
