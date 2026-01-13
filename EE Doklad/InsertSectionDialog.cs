using System;
using System.Windows.Forms;

namespace EEDoklad
{
    public class InsertSectionDialog : Form
    {
        public int Position { get; private set; }
        public string SectionName { get; private set; }

        private NumericUpDown numPosition;
        private TextBox txtSectionName;
        private Button btnOK;
        private Button btnCancel;

        public InsertSectionDialog(int maxSections)
        {
            InitializeComponent();
            SetupControls(maxSections);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ResumeLayout(false);
        }

        private void SetupControls(int maxSections)
        {
            this.Text = "Вмъкване на секция";
            this.Size = new System.Drawing.Size(350, 160);
            
            Label lblPosition = new Label { Text = "Вмъкни преди номер:", Left = 10, Top = 10, Width = 150 };
            numPosition = new NumericUpDown { Left = 170, Top = 10, Width = 150, Minimum = 1, Maximum = maxSections + 1, Value = maxSections + 1 };
            
            Label lblName = new Label { Text = "Име на секцията:", Left = 10, Top = 40, Width = 150 };
            txtSectionName = new TextBox { Left = 170, Top = 40, Width = 150 };
            
            btnOK = new Button { Text = "OK", Left = 170, Top = 80, Width = 75, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Отказ", Left = 250, Top = 80, Width = 75, DialogResult = DialogResult.Cancel };
            
            btnOK.Click += BtnOK_Click;
            
            this.Controls.AddRange(new Control[] { lblPosition, numPosition, lblName, txtSectionName, btnOK, btnCancel });
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSectionName.Text))
            {
                MessageBox.Show("Моля, въведете име на секцията.", "Грешка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            Position = (int)numPosition.Value;
            SectionName = txtSectionName.Text.Trim();
        }
    }
}
