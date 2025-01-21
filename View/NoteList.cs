using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoteApp.View
{
    public partial class NoteList : Form
    {
        public NoteList()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(new Model.NoteModel("/base.sql").getCategories());
        }

        private void NoteList_Load(object sender, EventArgs e)
        {

        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void создатьЗаметкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form curForm = Form.ActiveForm;
            Form nextForm = new View.EditForm();
            nextForm.Show();
            curForm.Hide();
        }
    }
}
