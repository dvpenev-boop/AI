using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;

namespace EE.Doklad.Views.FloorPanels
{
    public partial class FloorUnheatedBasementPanel : UserControl
    {
        public FloorUnheatedBasementPanel()
        {
            InitializeComponent();
        }

        private void AddFloorToBasementLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail)
            {
                detail.FloorToBasementLayers.Add(new FloorLayer { Material = "Нов слой", Thickness = 0.1, Lambda = 1.0 });
            }
        }

        private void RemoveFloorToBasementLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail && detail.FloorToBasementLayers.Count > 0)
            {
                detail.FloorToBasementLayers.RemoveAt(detail.FloorToBasementLayers.Count - 1);
            }
        }

        private void AddBasementFloorLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail)
            {
                detail.BasementFloorLayers.Add(new FloorLayer { Material = "Нов слой", Thickness = 0.1, Lambda = 1.0 });
            }
        }

        private void RemoveBasementFloorLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail && detail.BasementFloorLayers.Count > 0)
            {
                detail.BasementFloorLayers.RemoveAt(detail.BasementFloorLayers.Count - 1);
            }
        }

        private void AddBasementWallLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail)
            {
                detail.BasementWallLayers.Add(new FloorLayer { Material = "Нов слой", Thickness = 0.1, Lambda = 1.0 });
            }
        }

        private void RemoveBasementWallLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail && detail.BasementWallLayers.Count > 0)
            {
                detail.BasementWallLayers.RemoveAt(detail.BasementWallLayers.Count - 1);
            }
        }

        private void AddWallAboveGradeLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail)
            {
                detail.WallAboveGradeLayers.Add(new FloorLayer { Material = "Нов слой", Thickness = 0.1, Lambda = 1.0 });
            }
        }

        private void RemoveWallAboveGradeLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorUnheatedBasementDetail detail && detail.WallAboveGradeLayers.Count > 0)
            {
                detail.WallAboveGradeLayers.RemoveAt(detail.WallAboveGradeLayers.Count - 1);
            }
        }
    }
}
