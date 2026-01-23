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
            if (DataContext is FloorHeatedBasementDetail detail)
            {
                detail.FloorLayers.Add(new FloorLayer { Material = "Бетон", Thickness = 0.2, Lambda = 1.7 });
            }
        }

        private void RemoveFloorLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorHeatedBasementDetail detail && detail.FloorLayers.Count > 0)
            {
                detail.FloorLayers.RemoveAt(detail.FloorLayers.Count - 1);
            }
        }

        private void AddWallLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorHeatedBasementDetail detail)
            {
                detail.WallLayers.Add(new FloorLayer { Material = "Бетон", Thickness = 0.25, Lambda = 1.7 });
            }
        }

        private void RemoveWallLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorHeatedBasementDetail detail && detail.WallLayers.Count > 0)
            {
                detail.WallLayers.RemoveAt(detail.WallLayers.Count - 1);
            }
        }
    }
}
