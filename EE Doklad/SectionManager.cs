using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace EEDoklad
{
    public class SectionManager
    {
        private List<Section> sections = new List<Section>();

        public SectionManager()
        {
            sections = new List<Section>();
        }

        public void AddSection(string name)
        {
            sections.Add(new Section 
            { 
                Number = sections.Count + 1, 
                Name = name 
            });
        }

        public void InsertSectionAt(int position, string sectionName)
        {
            if (position < 1 || position > sections.Count + 1)
                throw new ArgumentOutOfRangeException(nameof(position));

            var newSection = new Section 
            { 
                Number = position, 
                Name = sectionName 
            };
            
            sections.Insert(position - 1, newSection);
            RenumberSections();
        }

        public void RemoveSectionAt(int position)
        {
            if (position < 1 || position > sections.Count)
                throw new ArgumentOutOfRangeException(nameof(position));

            sections.RemoveAt(position - 1);
            RenumberSections();
        }

        private void RenumberSections()
        {
            for (int i = 0; i < sections.Count; i++)
            {
                sections[i].Number = i + 1;
            }
        }

        public List<Section> GetSections()
        {
            return sections.ToList();
        }

        public int Count => sections.Count;
    }

    public class Section
    {
        public int Number { get; set; }
        public string Name { get; set; }
        
        public override string ToString()
        {
            return $"{Number}. {Name}";
        }
    }

    public partial class MainForm : Form
    {
        private SectionManager sectionManager = new SectionManager();

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnAddSection_Click(object sender, EventArgs e)
        {
            var dialog = new InsertSectionDialog(sectionManager.Count);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                sectionManager.InsertSectionAt(dialog.Position, dialog.SectionName);
                RefreshSectionsList();
            }
        }

        private void RefreshSectionsList()
        {
            listBoxSections.Items.Clear();
            foreach (var section in sectionManager.GetSections())
            {
                listBoxSections.Items.Add(section.ToString());
            }
        }
    }
}
