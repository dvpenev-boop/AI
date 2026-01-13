using System;
using System.Collections.Generic;
using System.Windows;
using EE.Doklad.Models;

namespace EE.Doklad
{
    // NOTE: This is a WPF application, not Windows Forms
    // This file appears to be a scratch file with Windows Forms code
    // Converted to WPF Window for compatibility
    
    public class MainForm : Window
    {
        private List<Section> sections = new List<Section>();
        // Note: SectionManager is in root folder with namespace EEDoklad
        // Uncomment if needed: private EEDoklad.SectionManager sectionManager = new EEDoklad.SectionManager();

        public MainForm()
        {
            // InitializeComponent is not needed without XAML
        }

        private void btnInsertSection_Click(object sender, RoutedEventArgs e)
        {
            // Example WPF dialog usage
            // var dialog = new InsertSectionDialog(sections.Count);
            // if (dialog.ShowDialog() == true)
            // {
            //     sectionManager.InsertSectionAt(dialog.Position, dialog.SectionName);
            //     RefreshSectionsList();
            // }
        }

        private void RefreshSectionsList()
        {
            // Update the sections list in UI
        }
    }
}