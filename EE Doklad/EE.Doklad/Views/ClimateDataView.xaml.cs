using System.Windows.Controls;
using EE.Doklad.ViewModels;

namespace EE.Doklad.Views
{
    public partial class ClimateDataView : UserControl
    {
        public ClimateDataView()
        {
            InitializeComponent();
            DataContext = new ClimateDataViewModel();
        }
    }
}
