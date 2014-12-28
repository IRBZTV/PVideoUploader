using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PVU.MyDBTableAdapters;

namespace PVU
{
    public partial class Category : Form
    {
        int _Pid = 0;
        int _Id = 0;
        public Category(int Pid, int Id)
        {
            _Id = Id;
            _Pid = Pid;
            InitializeComponent();
        }

        private void Category_Load(object sender, EventArgs e)
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
                textBox1.Text = Arch_Ta.Categories_Select_ById(_Id)[0]["Title"].ToString().Trim();
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ARCHIVETableAdapter Arch_Ta = new ARCHIVETableAdapter();
            if (_Id == 0)
            {
                Arch_Ta.Categories_Insert(textBox1.Text.Trim(), _Pid);
                MessageBox.Show("مورد با موفقیت اضافه شد", "ثبت مورد", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Arch_Ta.Categories_UpdateTitle(textBox1.Text.Trim(), _Id);
            }
            if (System.Windows.Forms.Application.OpenForms["Form1"] != null)
            {
                (System.Windows.Forms.Application.OpenForms["Form1"] as Form1).FillCategories();
            }

            this.Close();
        }
    }
}
