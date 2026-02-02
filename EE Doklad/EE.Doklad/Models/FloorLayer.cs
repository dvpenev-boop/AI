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

        partial void OnMaterialChanged(string value)
        {
            // When the user types a material name manually, try to find a matching option
            // and auto-fill Lambda and SelectedMaterialId similar to the SelectedMaterialId handler.
            if (string.IsNullOrWhiteSpace(value) || MaterialOptions == null)
                return;

            // Try exact name match first, then try display/text contains for partial matches.
            var match = MaterialOptions.FirstOrDefault(o => string.Equals(o.NameBg, value, System.StringComparison.CurrentCultureIgnoreCase))
                        ?? MaterialOptions.FirstOrDefault(o => (!string.IsNullOrEmpty(o.Display) && o.Display.IndexOf(value, System.StringComparison.CurrentCultureIgnoreCase) >= 0))
                        ?? MaterialOptions.FirstOrDefault(o => (!string.IsNullOrEmpty(o.NameBg) && o.NameBg.IndexOf(value, System.StringComparison.CurrentCultureIgnoreCase) >= 0));

            if (match != null)
            {
                // Avoid overwriting if already selected
                if (SelectedMaterialId != match.Id)
                {
                    SelectedMaterialId = match.Id;
                }

                // Fill lambda if available
                if (match.LambdaWmk > 0)
                    Lambda = match.LambdaWmk;
            }
        }
    }
}
