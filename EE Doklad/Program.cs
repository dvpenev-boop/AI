using System;
using System.Windows.Forms;

namespace EEDoklad
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Тест на диалога
            var dialog = new InsertSectionDialog(5);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show($"Позиция: {dialog.Position}\nИме: {dialog.SectionName}", 
                    "Резултат", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
