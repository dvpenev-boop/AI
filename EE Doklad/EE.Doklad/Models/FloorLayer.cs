using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    public partial class FloorLayer : ObservableObject
    {
        [ObservableProperty]
        private string material = string.Empty;

        [ObservableProperty]
        private double thickness;

        [ObservableProperty]
        private double lambda;

        public double R => Lambda > 0 ? Thickness / Lambda : 0;

        partial void OnThicknessChanged(double value) => OnPropertyChanged(nameof(R));
        partial void OnLambdaChanged(double value) => OnPropertyChanged(nameof(R));
    }
}
