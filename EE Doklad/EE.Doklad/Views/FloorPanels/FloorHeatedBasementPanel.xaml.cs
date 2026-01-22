using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;

namespace EE.Doklad.Views.FloorPanels
{
    public partial class FloorHeatedBasementPanel : UserControl
    {
        public FloorHeatedBasementPanel()
        {
            InitializeComponent();
        }

        private void AddFloorLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorHeatedBasementInput input)
            {
                input.FloorLayers.Add(new FloorLayer());
            }
        }

        private void RemoveFloorLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorHeatedBasementInput input && input.FloorLayers.Count > 0)
            {
                input.FloorLayers.RemoveAt(input.FloorLayers.Count - 1);
            }
        }

        private void AddWallLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorHeatedBasementInput input)
            {
                input.WallLayers.Add(new FloorLayer());
            }
        }

        private void RemoveWallLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorHeatedBasementInput input && input.WallLayers.Count > 0)
            {
                input.WallLayers.RemoveAt(input.WallLayers.Count - 1);
            }
        }

        public FloorHeatedBasementInput GetInput()
        {
            return DataContext as FloorHeatedBasementInput ?? new FloorHeatedBasementInput();
        }
    }
}
