using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;

namespace EE.Doklad.Views.FloorPanels
{
    public partial class FloorUnheatedSpacePanel : UserControl
    {
        public FloorUnheatedSpacePanel()
        {
            InitializeComponent();
        }

        private void AddLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedSpaceInput input)
            {
                input.Layers.Add(new RoofLayer());
            }
        }

        private void RemoveLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedSpaceInput input && input.Layers.Count > 0)
            {
                input.Layers.RemoveAt(input.Layers.Count - 1);
            }
        }

        public FloorUnheatedSpaceInput GetInput()
        {
            return DataContext as FloorUnheatedSpaceInput ?? new FloorUnheatedSpaceInput();
        }
    }
}
