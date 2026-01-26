using System.Windows;
using EE.Doklad.Services;

namespace EE.Doklad.Views
{
    public partial class ImportOptionsWindow : Window
    {
        public ImportOptionsWindow()
        {
            InitializeComponent();
            RbMergeSkip.IsChecked = true; // default
        }

        public ImportMergeStrategy SelectedStrategy
        {
            get
            {
                if (RbReplaceAll.IsChecked == true) return ImportMergeStrategy.ReplaceAll;
                if (RbMergeOverwrite.IsChecked == true) return ImportMergeStrategy.MergeOverwriteDuplicates;
                return ImportMergeStrategy.MergeSkipDuplicates;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
