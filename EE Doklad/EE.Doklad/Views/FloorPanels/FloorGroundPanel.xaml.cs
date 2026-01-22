using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;

namespace EE.Doklad.Views.FloorPanels
{
    public partial class FloorGroundPanel : UserControl
    {
        public FloorGroundPanel()
        {
            InitializeComponent();
        }

        private void AddLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorGroundDetail detail)
            {
                detail.Layers.Add(new FloorLayer());
            }
        }

        private void RemoveLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FloorGroundDetail detail && detail.Layers.Count > 0)
            {
                detail.Layers.RemoveAt(detail.Layers.Count - 1);
            }
        }

        public FloorGroundInput GetInput()
        {
            return DataContext as FloorGroundInput ?? new FloorGroundInput(); // Not used, kept for compatibility
        }
    }
}
