using PVU.MyDBTableAdapters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PVU
{
    public partial class Kinds : Form
    {
        int _Id = 0;
        public Kinds(int Id)
        {
            _Id = Id;
            InitializeComponent();
        }

        private void Kinds_Load(object sender, EventArgs e)
        {
            ARCHIVETableAdapter Arch_Ta = new ARCHIVETableAdapter();
            if (_Id == 0)
            {
                label1.Text = "ثبت مورد جدید";
                textBox1.Text = "";

            }
            else
            {
                label1.Text = "ویرایش مورد انتخاب شده";
                textBox1.Text = Arch_Ta.Kinds_Select_ById(_Id)[0]["Title"].ToString().Trim();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ARCHIVETableAdapter Arch_Ta = new ARCHIVETableAdapter();
            if (_Id == 0)
            {
                Arch_Ta.Kinds_Insert(textBox1.Text.Trim());
                MessageBox.Show("مورد با موفقیت اضافه شد", "ثبت مورد", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Arch_Ta.Kinds_Update(textBox1.Text.Trim(), _Id);
            }
            if (System.Windows.Forms.Application.OpenForms["Form1"] != null)
            {
                (System.Windows.Forms.Application.OpenForms["Form1"] as Form1).FillKinds();
            }
            this.Close();
        }
    }
}
