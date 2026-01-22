using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    public partial class FloorLayer : ObservableObject
    {
        [ObservableProperty]
        private string material = string.Empty;

        [ObservableProperty]
        private double thickness = 0.1; // Default 10 cm

        [ObservableProperty]
        private double lambda = 0.04; // Default insulation thermal conductivity

        public double R => Lambda > 0 ? Thickness / Lambda : 0;

        partial void OnThicknessChanged(double value) => OnPropertyChanged(nameof(R));
        partial void OnLambdaChanged(double value) => OnPropertyChanged(nameof(R));
    }
}
