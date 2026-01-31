using CommunityToolkit.Mvvm.ComponentModel;
using EE.Doklad.Models;

namespace EE.Doklad.ViewModels
{
    /// <summary>
    /// ViewModel за секция "Заключение"
    /// </summary>
    public partial class ConclusionSectionViewModel : ObservableObject
    {
        [ObservableProperty]
        private ConclusionSectionData _data;

        public ConclusionSectionViewModel(ConclusionSectionData data)
        {
            _data = data;
        }
    }
}
