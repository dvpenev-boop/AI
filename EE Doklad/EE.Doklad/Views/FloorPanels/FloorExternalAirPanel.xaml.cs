using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;

namespace EE.Doklad.Views.FloorPanels
{
    public partial class FloorExternalAirPanel : UserControl
    {
        public FloorExternalAirPanel()
        {
            InitializeComponent();
        }

        private void AddLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorExternalAirInput input)
            {
                input.Layers.Add(new RoofLayer());
            }
        }

        private void RemoveLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorExternalAirInput input && input.Layers.Count > 0)
            {
                input.Layers.RemoveAt(input.Layers.Count - 1);
            }
        }

        public FloorExternalAirInput GetInput()
        {
            return DataContext as FloorExternalAirInput ?? new FloorExternalAirInput();
        }
    }
}
