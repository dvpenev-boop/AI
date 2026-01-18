using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;

namespace EE.Doklad.Views
{
    public partial class ColdRoofDetailControl : UserControl
    {
        public ColdRoofDetailControl()
        {
            InitializeComponent();
        }

        private void AddU1Layer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoofType roofType && roofType.ColdDetail != null)
            {
                roofType.ColdDetail.U1.Layers.Add(new RoofLayer());
            }
        }

        private void RemoveU1Layer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoofType roofType && roofType.ColdDetail != null && roofType.ColdDetail.U1.Layers.Any())
            {
                roofType.ColdDetail.U1.Layers.RemoveAt(roofType.ColdDetail.U1.Layers.Count - 1);
            }
        }

        private void AddU2Layer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoofType roofType && roofType.ColdDetail != null)
            {
                roofType.ColdDetail.U2.Layers.Add(new RoofLayer());
            }
        }

        private void RemoveU2Layer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoofType roofType && roofType.ColdDetail != null && roofType.ColdDetail.U2.Layers.Any())
            {
                roofType.ColdDetail.U2.Layers.RemoveAt(roofType.ColdDetail.U2.Layers.Count - 1);
            }
        }

        private void AddUwLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoofType roofType && roofType.ColdDetail != null)
            {
                roofType.ColdDetail.Uw.Layers.Add(new RoofLayer());
            }
        }

        private void RemoveUwLayer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoofType roofType && roofType.ColdDetail != null && roofType.ColdDetail.Uw.Layers.Any())
            {
                roofType.ColdDetail.Uw.Layers.RemoveAt(roofType.ColdDetail.Uw.Layers.Count - 1);
            }
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoofType roofType && roofType.ColdDetail != null)
            {
                roofType.ColdDetail.CalculateAll();
            }
        }

        private void RemoveDetail_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoofType roofType)
            {
                // Find the parent view and call its removal logic
                var parent = FindParentView();
                if (parent != null && parent.DataContext is ViewModels.RoofSectionViewModel vm)
                {
                    vm.RemoveRoofTypeCommand.Execute(roofType);
                }
            }
        }

        private RoofSectionView? FindParentView()
        {
            DependencyObject current = this;
            while (current != null)
            {
                if (current is RoofSectionView view)
                {
                    return view;
                }
                current = System.Windows.Media.VisualTreeHelper.GetParent(current);
            }
            return null;
        }
    }
}
