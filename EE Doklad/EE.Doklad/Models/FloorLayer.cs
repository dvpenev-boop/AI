using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace EE.Doklad.Models
{
    public partial class FloorLayer : ObservableObject
    {
        [ObservableProperty]
        private string material = string.Empty;

        [ObservableProperty]
        private string? selectedMaterialId;

        [ObservableProperty]
        private double thickness;

        [ObservableProperty]
        private double lambda;

        public double R => Lambda > 0 ? Thickness / Lambda : 0;

        partial void OnSelectedMaterialIdChanged(string? value)
        {
            // Auto-fill Lambda when material is selected
            if (!string.IsNullOrEmpty(value) && MaterialOptions != null)
            {
                var option = MaterialOptions.FirstOrDefault(o => o.Id == value);
                if (option != null)
                {
                    Lambda = option.LambdaWmk;
                    Material = option.NameBg;
                }
            }
        }

        partial void OnThicknessChanged(double value) => OnPropertyChanged(nameof(R));
        partial void OnLambdaChanged(double value) => OnPropertyChanged(nameof(R));

        // Reference to material options (set from ViewModel)
        public IReadOnlyList<MaterialOption>? MaterialOptions { get; set; }
    }
}
