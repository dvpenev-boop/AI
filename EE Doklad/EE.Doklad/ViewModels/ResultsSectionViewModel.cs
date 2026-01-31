using CommunityToolkit.Mvvm.ComponentModel;
using EE.Doklad.Models;

namespace EE.Doklad.ViewModels
{
    /// <summary>
    /// ViewModel за секция "Резултати сграда"
    /// </summary>
    public partial class ResultsSectionViewModel : ObservableObject
    {
        [ObservableProperty]
        private ResultsSectionData _data;

        public ResultsSectionViewModel(ResultsSectionData data)
        {
            _data = data;
        }

        /// <summary>
        /// Задава отопляема площ от ObjectData
        /// </summary>
        public void SetHeatedArea(double? heatedArea)
        {
            Data.HeatedArea = heatedArea;
        }
    }
}
