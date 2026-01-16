using System.Windows;
using System.Windows.Controls;
using EE.Doklad.Models;
using EE.Doklad.ViewModels;

namespace EE.Doklad.Views
{
    public class RoofDetailTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? WarmRoofTemplate { get; set; }
        public DataTemplate? ColdRoofTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is RoofType roofType)
            {
                var fe = container as FrameworkElement;
                if (roofType.Mode == RoofMode.Warm)
                    return WarmRoofTemplate ?? fe?.FindResource("WarmRoofDetailTemplate") as DataTemplate;
                if (roofType.Mode == RoofMode.Cold)
                    return ColdRoofTemplate ?? fe?.FindResource("ColdRoofDetailTemplate") as DataTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
