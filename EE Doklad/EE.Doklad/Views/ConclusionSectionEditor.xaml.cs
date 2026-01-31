using System.Windows.Controls;
using EE.Doklad.ViewModels;

namespace EE.Doklad.Views
{
    /// <summary>
    /// Interaction logic for ConclusionSectionEditor.xaml
    /// </summary>
    public partial class ConclusionSectionEditor : UserControl
    {
        public ConclusionSectionEditor()
        {
            InitializeComponent();
        }

        public ConclusionSectionEditor(ConclusionSectionViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
