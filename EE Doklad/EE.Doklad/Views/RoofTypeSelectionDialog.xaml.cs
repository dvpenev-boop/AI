using System.Windows;
using EE.Doklad.Models;

namespace EE.Doklad.Views
{
    public partial class RoofTypeSelectionDialog : Window
    {
        public RoofMode? SelectedMode { get; private set; }

        public RoofTypeSelectionDialog()
        {
            InitializeComponent();
        }

        private void WarmRoof_Click(object sender, RoutedEventArgs e)
        {
            SelectedMode = RoofMode.Warm;
            DialogResult = true;
        }

        private void ColdRoof_Click(object sender, RoutedEventArgs e)
        {
            SelectedMode = RoofMode.Cold;
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
