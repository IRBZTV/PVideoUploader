using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PVU.MyDBTableAdapters;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using MyList;
using System.Runtime.InteropServices;
using MediaInfoNET;

namespace PVU
{

    public partial class Form1 : Form
    {
        string _FileName = "";
        int _Id = 0;
        int _Index = -1;
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
           out long lpFreeBytesAvailable,
           out long lpTotalNumberOfBytes,
           out long lpTotalNumberOfFreeBytes);

        long FreeBytesAvailable;
        long TotalNumberOfBytes;
        long TotalNumberOfFreeBytes;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

            textBox1.Text = openFileDialog1.FileName;
            axVLCPlugin22.playlist.items.clear();
            axVLCPlugin22.playlist.add("file:///" + openFileDialog1.FileName, "dfccdcdcd", null);
            axVLCPlugin22.playlist.playItem(0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                InputLanguage.CurrentInputLanguage = InputLanguage.InstalledInputLanguages[1];
            }
            catch
            {

                MessageBox.Show("زبان فارسی به عنوان زبان دوم در سیستم وحود ندارد");
            }

            for (int i = 1; i < 32; i++)
            {
                MyList.MyListItem Lst = new MyList.MyListItem(string.Format("{0:00}", i), i);
                CmbDay.Items.Add(Lst);
                comboBox4.Items.Add(Lst);
            }
            CmbDay.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;


            for (int i = 1; i < 13; i++)
            {
                MyList.MyListItem Lst = new MyList.MyListItem(string.Format("{0:00}", i), i);
                CmbMonth.Items.Add(Lst);
                comboBox5.Items.Add(Lst);
            }
            CmbMonth.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;


            for (int i = 1380; i < 1400; i++)
            {
                MyList.MyListItem Lst = new MyList.MyListItem(i.ToString(), i);
                CmbYear.Items.Add(Lst);
                comboBox6.Items.Add(Lst);
            }
            CmbYear.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
            FillCategories();
            FillKinds();
            FillProds();
            comboBox2.SelectedIndex = 0;
            FreeSpace();
        }
        public void FillCategories()
        {
            MyDBTableAdapters.ARCHIVETableAdapter Ta = new ARCHIVETableAdapter();
            MyDB.ARCHIVEDataTable Dt = Ta.Cats_SelectByPid(0);
            CmbCat.Items.Clear();

            for (int i = 0; i < Dt.Rows.Count; i++)
            {
                MyList.MyListItem LstTitle = new MyList.MyListItem(Dt[i]["TITLE"].ToString(), int.Parse(Dt[i]["ID"].ToString()));
                CmbCat.Items.Add(LstTitle);
            }
            if (CmbCat.Items.Count > 0)
            {
                CmbCat.SelectedIndex = 0;
            }
        }
        public void FillKinds()
        {
            MyDBTableAdapters.ARCHIVETableAdapter Ta = new ARCHIVETableAdapter();
            MyDB.ARCHIVEDataTable Dt = Ta.Kinds_SelectAll();
            comboBox1.Items.Clear();

            for (int i = 0; i < Dt.Rows.Count; i++)
            {
                MyList.MyListItem LstTitle = new MyList.MyListItem(Dt[i]["TITLE"].ToString(), int.Parse(Dt[i]["ID"].ToString()));
                comboBox1.Items.Add(LstTitle);
            }
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }
        public void FillProds()
        {
            MyDBTableAdapters.ARCHIVETableAdapter Ta = new ARCHIVETableAdapter();
            MyDB.ARCHIVEDataTable Dt = Ta.Prod_SelectAll();
            comboBox3.Items.Clear();

            for (int i = 0; i < Dt.Rows.Count; i++)
            {
                MyList.MyListItem LstTitle = new MyList.MyListItem(Dt[i]["TITLE"].ToString(), int.Parse(Dt[i]["ID"].ToString()));
                comboBox3.Items.Add(LstTitle);
            }
            if (comboBox3.Items.Count > 0)
            {
                comboBox3.SelectedIndex = 0;
            }
        }
        protected void FreeSpace()
        {
            try
            {
                bool success = GetDiskFreeSpaceEx(System.Configuration.ConfigurationSettings.AppSettings["SavePath"].ToString(),
                                           out FreeBytesAvailable,
                                           out TotalNumberOfBytes,
                                           out TotalNumberOfFreeBytes);
                if (!success)
                    throw new System.ComponentModel.Win32Exception();
                label22.Text = FormatBytes(FreeBytesAvailable);

            }
            catch 
            {

                label22.Text = "Unknown";
            }
         
        }
        private static string FormatBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i = 0;
            double dblSByte = bytes;
            if (bytes > 1024)
                for (i = 0; (bytes / 1024) > 0; i++, bytes /= 1024)
                    dblSByte = bytes / 1024.0;
            return String.Format("{0:0.##}{1}", dblSByte, Suffix[i]);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Start")
            {
                button1.Text = "Started";
                button1.BackColor = Color.Red;
            }
            _Index = QeueProcess();
            if (_Index >= 0)
            {
                Upload();
            }

        }
        protected void Upload()
        {
            //try
            //{
                axVLCPlugin22.playlist.items.clear();
                axVLCPlugin22.playlist.add("file:///" + dataGridView1.Rows[_Index].Cells[2].Value.ToString(), "dfccdcdcd", null);
                axVLCPlugin22.playlist.playItem(0);

                Thread.Sleep(5000);
                if (axVLCPlugin22.playlist.items.count > 0)
                {



                    dataGridView1.Rows[_Index].Cells[9].Value = "In Progress";
                    //axVLCPlugin22.playlist.togglePause();

                    DateTime Dt = DateTime.Parse(dataGridView1.Rows[_Index].Cells[4].Value.ToString());

                    DateTime DtDelivery = DateTime.Parse(dataGridView1.Rows[_Index].Cells[5].Value.ToString());



                    MediaFile VideoFilesss = new MediaFile(dataGridView1.Rows[_Index].Cells[2].Value.ToString());
                     
                    
                    ARCHIVETableAdapter Arch_Ta = new ARCHIVETableAdapter();
                    string FileName = Arch_Ta.Media_Insert(
                        dataGridView1.Rows[_Index].Cells[0].Value.ToString(),
                        dataGridView1.Rows[_Index].Cells[1].Value.ToString(),
                         int.Parse(dataGridView1.Rows[_Index].Cells[3].Value.ToString()),
                                  int.Parse(dataGridView1.Rows[_Index].Cells[11].Value.ToString()),
                                           int.Parse(dataGridView1.Rows[_Index].Cells[10].Value.ToString()),
                                           Dt,
                                            int.Parse(dataGridView1.Rows[_Index].Cells[6].Value.ToString()),
                                            VideoFilesss.Video[0].DurationMillis,
                                            dataGridView1.Rows[_Index].Cells[7].Value.ToString(),
                                            dataGridView1.Rows[_Index].Cells[8].Value.ToString(),
                                            DtDelivery).ToString();


                    _FileName = dataGridView1.Rows[_Index].Cells[0].Value.ToString();
                    _Id = int.Parse(FileName);

                    ////////////////////////////////////////

                    axVLCPlugin22.playlist.items.clear();
                    axVLCPlugin22.playlist.stop();


                    string DestFolder = System.Configuration.ConfigurationSettings.AppSettings["SavePath"].ToString() +
                        Arch_Ta.GetDate()[0]["YYYY-MM-DD"].ToString() + "\\";
                    DirectoryInfo DestDir = new DirectoryInfo(DestFolder);
                    if (!DestDir.Exists)
                    {
                        DestDir.Create();
                    }

                    string DestFilePath = DestFolder + FileName;

                    progressBar1.Value = 0;
                    label12.Text = "0%";
                    if (Path.GetExtension(dataGridView1.Rows[_Index].Cells[2].Value.ToString()).ToLower() == ".mp4")
                    {
                        List<String> TempFiles = new List<String>();
                        TempFiles.Add(dataGridView1.Rows[_Index].Cells[2].Value.ToString());
                        CopyFiles.CopyFiles Temp = new CopyFiles.CopyFiles(TempFiles, DestFilePath + ".mp4");
                        //Temp.EV_copyCanceled += Temp_EV_copyCanceled;
                        //Temp.EV_copyComplete += Temp_EV_copyComplete;
                        CopyFiles.DIA_CopyFiles TempDiag = new CopyFiles.DIA_CopyFiles();
                        TempDiag.SynchronizationObject = this;
                        Temp.CopyAsync(TempDiag);
                    }
                    else
                    {
                        Process proc = new Process();
                        if (Environment.Is64BitOperatingSystem)
                        {
                            proc.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg64";
                        }
                        else
                        {
                            proc.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg32";
                        }
                        string InterLaced = "-flags +ildct+ilme";



                        proc.StartInfo.Arguments = "-i " + "\"" + dataGridView1.Rows[_Index].Cells[2].Value.ToString() + "\"" + "  -r 25 -b 10000k  -ar 48000 -ab 192k -async 1 " + InterLaced + "   -y  " + "\"" + DestFilePath + ".mp4" + "\"";
                        proc.StartInfo.RedirectStandardError = true;
                        proc.StartInfo.UseShellExecute = false;
                        proc.StartInfo.CreateNoWindow = true;
                        proc.EnableRaisingEvents = true;
                        // proc.Exited += new EventHandler(myProcess_Exited);
                        if (!proc.Start())
                        {
                            MessageBox.Show("Error Coverting File, Check FileName ,No Persian And Special Character");
                            return;
                        }

                        proc.PriorityClass = ProcessPriorityClass.RealTime;
                        StreamReader reader = proc.StandardError;
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (richTextBox1.Lines.Length > 5)
                            {
                                richTextBox1.Text = "";
                            }
                            FindDuration(line, "1");
                            richTextBox1.Text += (line) + " \n";
                            richTextBox1.SelectionStart = richTextBox1.Text.Length;
                            richTextBox1.ScrollToCaret();
                            Application.DoEvents();
                        }
                        proc.Close();
                    }

                    Arch_Ta.Media_Active(_Id);
                    double SelectedTime = 10;
                    SelectedTime = Math.Round((SelectedTime * 25));
                    Process proc2 = new Process();
                    if (Environment.Is64BitOperatingSystem)
                    {
                        proc2.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg64";
                    }
                    else
                    {
                        proc2.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg32";
                    }
                    proc2.StartInfo.Arguments = "-i " + "\"" + dataGridView1.Rows[_Index].Cells[2].Value.ToString() + "\"" + " -filter:v select=\"eq(n\\," + SelectedTime.ToString() + ")\",scale=320:240,crop=iw:240 -vframes 1  -y    \"" + DestFilePath + ".png\"";
                    proc2.StartInfo.RedirectStandardError = true;
                    proc2.StartInfo.UseShellExecute = false;
                    proc2.StartInfo.CreateNoWindow = true;
                    proc2.Exited += new EventHandler(myProcess_Exited);
                    if (!proc2.Start())
                    {
                        richTextBox1.Text += " \n" + "Error starting";
                        return;
                    }
                    StreamReader reader2 = proc2.StandardError;
                    string line2;
                    richTextBox1.Text += "Start create Image: " + _Id + ".png\n";
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                    Application.DoEvents();
                    while ((line2 = reader2.ReadLine()) != null)
                    {
                        if (richTextBox1.Lines.Length > 5)
                        {
                            richTextBox1.Text = "";
                        }
                        richTextBox1.Text += (line2) + " \n";
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        richTextBox1.ScrollToCaret();
                        Application.DoEvents();
                    }
                    proc2.Close();
                    richTextBox1.Text += "End Create Image: " + _Id + ".png\n";
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                    Application.DoEvents();


                    axVLCPlugin22.playlist.items.clear();
                    axVLCPlugin22.playlist.stop();


                    try
                    {
                       System.IO.File.Delete(_FileName);
                    }
                    catch (Exception Exp)
                    {
                        MessageBox.Show(Exp.Message);
                        throw;
                    }

                    dataGridView1.Rows[_Index].Cells[9].Value = "Done";
                    _Index = QeueProcess();
                    if (_Index >= 0)
                    {
                        Upload();
                    }
                }
            //}
            //catch (Exception)
            //{

            //    throw;
            //}





        }

        private void myProcess_Exited(object sender, System.EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate()
            {
                axVLCPlugin22.playlist.items.clear();
                axVLCPlugin22.playlist.stop();
                try
                {
                    System.IO.File.Delete(_FileName);
                }
                catch (Exception Exp)
                {
                    MessageBox.Show(Exp.Message);
                    throw;
                }

            }));
        }

        protected void FindDuration(string Str, string ProgressControl)
        {
            string TimeCode = "";
            if (Str.Contains("Duration:"))
            {
                TimeCode = Str.Substring(Str.IndexOf("Duration: "), 21).Replace("Duration: ", "").Trim();
                string[] Times = TimeCode.Split('.')[0].Split(':');
                double Frames = double.Parse(Times[0].ToString()) * (3600) * (25) +
                    double.Parse(Times[1].ToString()) * (60) * (25) +
                    double.Parse(Times[2].ToString()) * (25);
                if (ProgressControl == "1")
                {
                    progressBar1.Maximum = int.Parse(Frames.ToString());
                }
                else
                {

                }
                // label2.Text = Frames.ToString();

            }
            if (Str.Contains("time="))
            {
                try
                {
                    string CurTime = "";
                    CurTime = Str.Substring(Str.IndexOf("time="), 16).Replace("time=", "").Trim();
                    string[] CTimes = CurTime.Split('.')[0].Split(':');
                    double CurFrame = double.Parse(CTimes[0].ToString()) * (3600) * (25) +
                        double.Parse(CTimes[1].ToString()) * (60) * (25) +
                        double.Parse(CTimes[2].ToString()) * (25);

                    if (ProgressControl == "1")
                    {
                        progressBar1.Value = int.Parse(CurFrame.ToString());

                        label12.Text = ((progressBar1.Value * 100) / progressBar1.Maximum).ToString() + "%";
                    }
                    else
                    {

                    }

                    //label3.Text = CurFrame.ToString();
                    Application.DoEvents();
                }
                catch
                {


                }

            }
            if (Str.Contains("fps="))
            {

                string Speed = "";

                Speed = Str.Substring(Str.IndexOf("fps="), 8).Replace("fps=", "").Trim();

                label9.Text = "Speed: " + (float.Parse(Speed) / 25).ToString() + " X ";
                Application.DoEvents();


            }




        }
        void Temp_EV_copyComplete()
        {
            axVLCPlugin22.playlist.items.clear();
            axVLCPlugin22.playlist.stop();
            System.IO.File.Delete(_FileName);
        }
        void Temp_EV_copyCanceled(List<CopyFiles.CopyFiles.ST_CopyFileDetails> filescopied)
        {
            //throw new NotImplementedException();
            MessageBox.Show("عملیات کپی متوقف شد");
            //ARCHIVETableAdapter Arch_Ta = new ARCHIVETableAdapter();
            //Arch_Ta.Delete_Master(_Id);

            // button2.Enabled = true;
        }
        protected int QeueProcess()
        {
            int Index = -1;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[9].Value.ToString() == "Waiting")
                {
                    Index = i;
                    return Index;
                }
            }
            return Index;
        }



        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                //Check Code is Exist Or Not:
                MyDBTableAdapters.ARCHIVETableAdapter Ta = new ARCHIVETableAdapter();
                MyDB.ARCHIVEDataTable Dt = Ta.Media_Select_ByCode(textBox4.Text.Trim());
                if (Dt.Rows.Count == 0)
                {
                    DateTime DtTolid = DateConversion.JD2GD(((MyList.MyListItem)(CmbYear.SelectedItem)).Name + "/" +
                               ((MyList.MyListItem)(CmbMonth.SelectedItem)).Name + "/" +
                              ((MyList.MyListItem)(CmbDay.SelectedItem)).Name);


                    DateTime DtPakhsh = DateConversion.JD2GD(((MyList.MyListItem)(comboBox6.SelectedItem)).Name + "/" +
                             ((MyList.MyListItem)(comboBox5.SelectedItem)).Name + "/" +
                             ((MyList.MyListItem)(comboBox4.SelectedItem)).Name);

                    if (DtTolid <= DtPakhsh)
                    {
                        this.dataGridView1.Rows.Add(textBox4.Text.Trim(),//code
                            textBox2.Text.Trim(),//Title
                            textBox1.Text.Trim(),//FileName
                            ((MyListItem)(CmbSubCat.SelectedItem)).value.ToString(),//SubCatId                   
                            DtTolid,//DtTolid    
                            DtPakhsh,//DtPakhsh    
                            comboBox2.SelectedItem.ToString(),//Savemonth
                            textBox3.Text.Trim(),//Reviewer
                            textBox5.Text.Trim(),//Desc              
                            "Waiting",
                              ((MyListItem)(comboBox3.SelectedItem)).value.ToString(),//SubCatId  
                                 ((MyListItem)(comboBox1.SelectedItem)).value.ToString()//SubCatId  
                                 );
                    }
                    else
                    {
                        MessageBox.Show("تاریخ تولید باید قبل از تاریخ پخش باشد", "تاریخ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("کد وارد شده تکراری است", "کد برنامه", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception Exp)
            {

                MessageBox.Show(Exp.Message);
                MessageBox.Show("لطفا اطلاعات ورودی را چک نمایید");
            }

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                if (dataGridView1.SelectedRows[0].Cells[9].Value.ToString() == "Waiting")
                {
                    this.dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                }
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            axVLCPlugin22.playlist.items.clear();
            axVLCPlugin22.playlist.stop();
        }

        private void CmbCat_SelectedIndexChanged(object sender, EventArgs e)
        {
            MyListItem Selected = (MyListItem)CmbCat.SelectedItem;
            MyDBTableAdapters.ARCHIVETableAdapter Ta = new ARCHIVETableAdapter();
            MyDB.ARCHIVEDataTable Dt = Ta.Cats_SelectByPid(int.Parse(Selected.value.ToString()));
            CmbSubCat.Items.Clear();

            for (int i = 0; i < Dt.Rows.Count; i++)
            {
                MyList.MyListItem LstTitle = new MyList.MyListItem(Dt[i]["TITLE"].ToString(), int.Parse(Dt[i]["ID"].ToString()));
                CmbSubCat.Items.Add(LstTitle);
            }
            if (CmbSubCat.Items.Count > 0)
            {
                CmbSubCat.SelectedIndex = 0;
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            //Categories Add
            Category Frm = new Category(0, 0);
            Frm.Show();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (CmbCat.Items.Count > 0)
            {
                Category Frm = new Category(0, ((MyListItem)(CmbCat.SelectedItem)).value);
                Frm.Show();
            }
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Category Frm = new Category(((MyListItem)(CmbCat.SelectedItem)).value, 0);
            Frm.Show();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            if (CmbSubCat.Items.Count > 0)
            {
                Category Frm = new Category(((MyListItem)(CmbCat.SelectedItem)).value, ((MyListItem)(CmbSubCat.SelectedItem)).value);
                Frm.Show();
            }
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            Kinds Frm = new Kinds(0);
            Frm.Show();
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count > 0)
            {
                Kinds Frm = new Kinds(((MyListItem)(comboBox1.SelectedItem)).value);
                Frm.Show();
            }

        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            Prods Frm = new Prods(0);
            Frm.Show();
        }
    }
}
