using System;
using System.Windows.Forms;

namespace EEDoklad
{
    public class TestForm : Form
    {
        private SectionManager sectionManager;
        private ListBox lstSections;
        private Button btnInsertSection;
        private Button btnAddSection;
        private Button btnRemoveSection;

        public TestForm()
        {
            InitializeComponent();
            sectionManager = new SectionManager();
            
            // Добавяме начални секции за тест
            sectionManager.AddSection("Въведение");
            sectionManager.AddSection("Методология");
            sectionManager.AddSection("Резултати");
            sectionManager.AddSection("Заключение");
            
            RefreshSectionsList();
        }

        private void InitializeComponent()
        {
            this.Text = "Тест на секции - EE Доклад";
            this.Size = new System.Drawing.Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            lstSections = new ListBox 
            { 
                Left = 10, 
                Top = 10, 
                Width = 460, 
                Height = 250 
            };

            btnInsertSection = new Button 
            { 
                Text = "Вмъкни секция на позиция...", 
                Left = 10, 
                Top = 270, 
                Width = 220 
            };
            btnInsertSection.Click += BtnInsertSection_Click;

            btnAddSection = new Button 
            { 
                Text = "Добави секция накрая", 
                Left = 240, 
                Top = 270, 
                Width = 230 
            };
            btnAddSection.Click += BtnAddSection_Click;

            btnRemoveSection = new Button 
            { 
                Text = "Изтрий избрана секция", 
                Left = 10, 
                Top = 310, 
                Width = 220 
            };
            btnRemoveSection.Click += BtnRemoveSection_Click;

            this.Controls.AddRange(new Control[] { lstSections, btnInsertSection, btnAddSection, btnRemoveSection });
        }

        private void BtnInsertSection_Click(object sender, EventArgs e)
        {
            var dialog = new InsertSectionDialog(sectionManager.Count);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    sectionManager.InsertSectionAt(dialog.Position, dialog.SectionName);
                    RefreshSectionsList();
                    MessageBox.Show($"Секция '{dialog.SectionName}' беше вмъкната на позиция {dialog.Position}.\nВсички секции след нея са преномерирани.", 
                        "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Грешка: {ex.Message}", "Грешка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnAddSection_Click(object sender, EventArgs e)
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox("Въведете име на секцията:", "Добави секция", "");
            if (!string.IsNullOrWhiteSpace(name))
            {
                sectionManager.AddSection(name);
                RefreshSectionsList();
            }
        }

        private void BtnRemoveSection_Click(object sender, EventArgs e)
        {
            if (lstSections.SelectedIndex >= 0)
            {
                int position = lstSections.SelectedIndex + 1;
                var result = MessageBox.Show($"Сигурни ли сте, че искате да изтриете секция {position}?", 
                    "Потвърждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    sectionManager.RemoveSectionAt(position);
                    RefreshSectionsList();
                    MessageBox.Show("Секцията беше изтрита. Останалите секции са преномерирани.", 
                        "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Моля, изберете секция за изтриване.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RefreshSectionsList()
        {
            lstSections.Items.Clear();
            foreach (var section in sectionManager.GetSections())
            {
                lstSections.Items.Add(section.ToString());
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TestForm());
        }
    }
}
