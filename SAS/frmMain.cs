﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using SAS.Forms;
using ICSharpCode.SharpZipLib.Zip;
using SAS.ClassSet.FunctionTools;
using SAS.ClassSet.Common;
using SAS.ClassSet.ListViewShow;
using SAS.ClassSet.MemberInfo;
using DevComponents.WinForms;
namespace SAS
{
    public partial class frmMain :Form
    {
       
        public frmMain()
        {
            InitializeComponent();
            fm = this;
        }
        public static frmMain fm;
        public int pagesize = 19;
        public int currentpage = 1;
        public int totalpage;
        SqlHelper pageshow;
        public int expertflag=0;//导出标志
        private void frmMain_Load(object sender, EventArgs e)
        {
            SetStatusText("欢迎使用~", 0);
            Common.xmlRead();
            if (Common.load())
            {
            }
            else
            {
                MessageBox.Show("找不到数据库");
            }

            flashListview();
            Control.CheckForIllegalCrossThreadCalls = false;
            label1.Text = "临时数据";
            delete.Enabled = false;

        }
       
       
        public void flashListview()
        {
            //---------------------------------------------
            listView1.Items.Clear();
          
            pageshow = new SqlHelper();
            totalpage = pageshow.totalpage("select * from Placement_Data", pagesize, "Placement_Data");
            labPageAll.Text = totalpage + "";
            textBoxNow.Text = currentpage.ToString();
            DataTable dt = pageshow.ListviewShow("select * from Placement_Data order by Class_week", currentpage, pagesize, "Placement_Data");
            UIShow show = new UIShow();
            show.placement_listview_write(dt, listView1, currentpage, pagesize);
           

        }
        public void flashListview_check()
        {
            //---------------------------------------------
            listView1.Items.Clear();

            pageshow = new SqlHelper();
            totalpage = pageshow.totalpage("select * from CheckPlacement_Data", pagesize, "CheckPlacement_Data");
            labPageAll.Text = totalpage + "";
            textBoxNow.Text = currentpage.ToString();
            DataTable dt = pageshow.ListviewShow("select * from CheckPlacement_Data order by Class_week", currentpage, pagesize, "CheckPlacement_Data");
            UIShow show = new UIShow();
            show.placement_listview_write(dt, listView1, currentpage, pagesize);


        }
        #region 委托

        delegate void SetTextCallback(string text, int color);
        public void SetStatusText(string text, int color)
        {
            if (this.toolStrip1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetStatusText);
                toolStrip1.Invoke(d, new object[] { text, color });
            }
            else
            {
                tsslStatus.Text = text;
                if (color == 0) tsslStatus.ForeColor = Color.Black;
                else tsslStatus.ForeColor = Color.Red;
            }
        }
        #endregion

        private void tsbSet_Click(object sender, EventArgs e)
        {
            fm = this;
            frmSetting fs = new frmSetting();
            fs.ShowDialog();
        }

        private void tsbLog_Click(object sender, EventArgs e)
        {
            frmLog fl = new frmLog();
            fl.Show();
        }

        private void tsbTeacher_Click(object sender, EventArgs e)
        {
          
        }

        private void tsbJdb_Click(object sender, EventArgs e)
        {
            frmPlan fp = new frmPlan();
            fp.Show();
        }

        private void tssbSend_ButtonClick(object sender, EventArgs e)
        {
            tssbSend.ShowDropDown();
        }

        private void tsbCreate_ButtonClick(object sender, EventArgs e)
        {
            tsbCreate.ShowDropDown();
        }

        private void tsmiAutoCreate_Click(object sender, EventArgs e)
        {
            //SetStatusText("正在工作中，请耐心等待~~", 1);
            //NewMathup creatplacement = new NewMathup();
            //creatplacement.select_superviosr();
            frmPlacement frmp = new frmPlacement();
            frmp.Show();
            //---------------------------------------------
        }
      
        private void tsmiCreate_Click(object sender, EventArgs e)
        {
            frmAnPai fa = new frmAnPai();
            fa.Show();
        }
        private void tsmiChangeAnPai_Click(object sender, EventArgs e)
        {
            if (listView1.CheckedItems.Count == 1)
            {
                frmAnPai fa = new frmAnPai(listView1.CheckedItems[0].Text);
                fa.ShowDialog();
            }
        }
       
       
    
        private void tsmiAllot_Click(object sender, EventArgs e)
        {
            if (listView1.CheckedItems.Count == 1)
            {
                //listView1.Items[0].BackColor = Color.Green;
                EmailPlacement sent = new EmailPlacement(listView1);
                sent.SentPlacement();
                //SentPlacement();
            }
        }

        private void tsmiResult_Click(object sender, EventArgs e)
        {
            if (listView1.CheckedItems.Count == 1)
            {
                frmResult fr = new frmResult(listView1);
                fr.Show();
            }
        }

        public  void Changelist()
        {
            listView1.SelectedItems[0].BackColor = Color.Green;
        }

        private void tsbHelp_Click(object sender, EventArgs e)
        {
            string helpPath = Common.TempPath + "\\SAS_help.htm";
            if (File.Exists(helpPath)) { System.Diagnostics.Process.Start(helpPath); }
            else
            {
                //FileStream fs = new FileStream(help, FileMode.OpenOrCreate, FileAccess.Write);
                ////创建byte数组，装资源 
                //Byte[] b = DMS.Properties.Resources.help;
                //fs.Write(b, 0, b.Length);
                //if (fs != null)
                //    fs.Close();
                try
                {
                    Stream S = new MemoryStream(SAS.Properties.Resources.SAS_Help);
                    //using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
                    string rootFile = "";
                    string fileDir = Common.TempPath;
                    //读取压缩文件(zip文件)，准备解压缩
                    ZipInputStream s = new ZipInputStream(S);

                    ZipEntry theEntry;
                    string path = fileDir;  //解压出来的文件保存的路径

                    string rootDir = ""; //根目录下的第一个子文件夹的名称
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        rootDir = Path.GetDirectoryName(theEntry.Name); //得到根目录下的第一级子文件夹的名称
                        if (rootDir.IndexOf("\\") >= 0)
                        {
                            rootDir = rootDir.Substring(0, rootDir.IndexOf("\\") + 1);
                        }
                        string dir = Path.GetDirectoryName(theEntry.Name); //根目录下的第一级子文件夹的下的文件夹的名称
                        string fileName = Path.GetFileName(theEntry.Name); //根目录下的文件名称
                        if (dir != "" && fileName == "") //创建根目录下的子文件夹,不限制级别
                        {
                            if (!Directory.Exists(fileDir + "\\" + dir))
                            {
                                path = fileDir + "\\" + dir; //在指定的路径创建文件夹
                                Directory.CreateDirectory(path);
                            }
                        }
                        else if (dir == "" && fileName != "") //根目录下的文件
                        {
                            path = fileDir;
                            rootFile = fileName;
                        }
                        else if (dir != "" && fileName != "") //根目录下的第一级子文件夹下的文件
                        {
                            if (dir.IndexOf("\\") > 0) //指定文件保存的路径
                            {
                                path = fileDir + "\\" + dir;
                            }
                        }

                        if (dir == rootDir) //判断是不是需要保存在根目录下的文件
                        {
                            path = fileDir + "\\" + rootDir;
                        }
                        //以下为解压缩zip文件的基本步骤
                        //基本思路就是遍历压缩文件里的所有文件，创建一个相同的文件。
                        if (fileName != String.Empty)
                        {
                            FileStream streamWriter = File.Create(path + "\\" + fileName);

                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            streamWriter.Close();
                            //streamWriter.Dispose();
                        }
                    }

                    s.Close();
           
                    System.Diagnostics.Process.Start(helpPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                GC.Collect();
            }
        }



        private void btnPageUp_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            if (currentpage > 1)
            {
                currentpage--;
               
            }
            if (currentpage == 1)
            {
                currentpage = 1;
             

            }
           textBoxNow.Text = currentpage.ToString();
            if (expertflag==0)
            {
                DataTable dt = pageshow.ListviewShow("select * from Placement_Data order by Class_week", currentpage, pagesize, "Placement_Data");
                UIShow show = new UIShow();
                show.placement_listview_write(dt, listView1, currentpage, pagesize);
            } 
            else
            {
                DataTable dt = pageshow.ListviewShow("select * from CheckPlacement_Data order by Class_week", currentpage, pagesize, "CheckPlacement_Data");
                UIShow show = new UIShow();
                show.placement_listview_write(dt, listView1, currentpage, pagesize);
            }
          
        }

        private void btnPageDown_Click(object sender, EventArgs e)
        {
       listView1.Items.Clear();
            if (currentpage < totalpage)
            {
                currentpage++;
            }
            if (currentpage == totalpage)
            {
                currentpage = totalpage;
            }
            textBoxNow.Text = currentpage.ToString();
            if (expertflag == 0)
            {
                DataTable dt = pageshow.ListviewShow("select * from Placement_Data order by Class_week", currentpage, pagesize, "Placement_Data");
                UIShow show = new UIShow();
                show.placement_listview_write(dt, listView1, currentpage, pagesize);
            }
            else
            {
                DataTable dt = pageshow.ListviewShow("select * from CheckPlacement_Data order by Class_week", currentpage, pagesize, "CheckPlacement_Data");
                UIShow show = new UIShow();
                show.placement_listview_write(dt, listView1, currentpage, pagesize);
            }
           
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {   if (expertflag==0)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
            }// flashListview();
            else
            {
                listView1.Items.Clear();
                string fe = "Teacher like '%" + txtSearch.Text.Trim() + "%' or Supervisor_Name like '%" + txtSearch.Text.Trim() + "%'";
                string cmdText = "SELECT * FROM Placement_Data WHERE " + fe;
                pageshow = new SqlHelper();
                totalpage = pageshow.totalpage(cmdText, pagesize, "Placement_Data");
                labPageAll.Text = totalpage + "";
                textBoxNow.Text = currentpage.ToString();
                DataTable dt = pageshow.ListviewShow(cmdText, currentpage, pagesize, "Placement_Data");
                UIShow show = new UIShow();
                show.placement_listview_write(dt, listView1, currentpage, pagesize);
            }
        } 
        else
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
            }// flashListview();
            else
            {
                listView1.Items.Clear();
                string fe = "Teacher like '%" + txtSearch.Text.Trim() + "%' or Supervisor_Name like '%" + txtSearch.Text.Trim() + "%'";
                string cmdText = "SELECT * FROM CheckPlacement_Data WHERE " + fe;
                pageshow = new SqlHelper();
                totalpage = pageshow.totalpage(cmdText, pagesize, "CheckPlacement_Data");
                labPageAll.Text = totalpage + "";
                textBoxNow.Text = currentpage.ToString();
                DataTable dt = pageshow.ListviewShow(cmdText, currentpage, pagesize, "CheckPlacement_Data");
                UIShow show = new UIShow();
                show.placement_listview_write(dt, listView1, currentpage, pagesize);
            }
        }
           
        }



        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (listView1.CheckedItems.Count == 1)
            {
                ToolStripMenuItem.Enabled = true;
                tsmiAllot.Enabled = true;
                tsmiResult.Enabled = true;
            }
            else
            {
                ToolStripMenuItem.Enabled = false;
                tsmiAllot.Enabled = false;
                tsmiResult.Enabled = false;
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {

            Point curPos = this.listView1.PointToClient(Control.MousePosition);
            ListViewItem lvwItem = this.listView1.GetItemAt(curPos.X, curPos.Y);
            foreach (ListViewItem s in listView1.Items)
            {
                s.Checked = false;
                s.Selected = false;
            }
            if (lvwItem != null)
            {
                lvwItem.Selected = true;
                if (e.X > 16) lvwItem.Checked = true;
            }
            else {  }
        }

        private void tsbAbout_Click(object sender, EventArgs e)
        {
            frmAbout fa = new frmAbout();
            fa.ShowDialog();
        }

        private void ExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {   if(listView1.Items.Count>0){
            if (expertflag==0)
            {
                //ExcelTools output = new ExcelTools();
                //output.ExportToExecl("select * from Placement_Data", "Placement_Data", listView1);
                new NPOIHelper() .ExportToExecl("select * from Placement_Data", "Placement_Data");
               
            } 
            else
            {
                //ExcelTools output = new ExcelTools();
                //output.ExportToExecl("select * from CheckPlacement_Data", "Placement_Data", listView1);
                new NPOIHelper().ExportToExecl("select * from CheckPlacement_Data", "CheckPlacement_Data");
               
            }
           
            }else{
                MessageBox.Show("当前没有数据");
            }
           
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmSearch frm = new frmSearch();
            frm.Show();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            frmTeacher ft = new frmTeacher();
            ft.Show();
        }

        private void tsbTeacher_ButtonClick(object sender, EventArgs e)
        {
            tsbTeacher.ShowDropDown();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            frmSpareTime fs = new frmSpareTime();
            fs.Show();
        }

        private void ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
             frmAnPai frm = new frmAnPai("1");
            frm.Show();
        }

        private void 保存记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.CheckedItems.Count == 1)
            {   SqlHelper help=new SqlHelper();
                DataTable dt =  help.getDs("select * from Placement_Data", "Placement_Data").Tables[0];
                DataRow[] dr = dt.Select("Teacher like '%" + listView1.CheckedItems[0].SubItems[6].Text + "%'" + " and " + "Class_week= '" + listView1.CheckedItems[0].SubItems[7].Text + "'");//+ " and " + "Class_week= '" + listView1.CheckedItems[0].SubItems[7].Text + "'" + 
                DataTable dtcheck =  help.getDs("select * from CheckPlacement_Data", "CheckPlacement_Data").Tables[0];
                DataRow drc = dtcheck.NewRow();
                for (int i = 0; i < dt.Columns.Count;i++ )
                {
                    drc[i] = dr[0][i];
                }
                dtcheck.Rows.Add(drc);
                OleDbDataAdapter da = help.adapter("select * from CheckPlacement_Data");
                try
                {
                    da.Update(dtcheck);
                    MessageBox.Show("保存成功");
                }
                catch (OleDbException)
                {
                    MessageBox.Show("记录已存在，请删除后再保存");
                }
                finally
                {

                   
                }
                }
             
                
                
        }

        private void 查看保存记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flashListview_check();
            saverecord.Enabled = false;
            tsmiAutoCreate.Enabled = false;
            tsmiCreate.Enabled = false;
            label1.Text = "保存数据";
            delete.Enabled = true;
            search.Enabled = false;
            expertflag = 1;
        }

        private void 查看临时数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flashListview();
            saverecord.Enabled = true;
            tsmiAutoCreate.Enabled = true;
            tsmiCreate.Enabled = true;
            label1.Text = "临时数据";
            delete.Enabled = false;
            search.Enabled = true;
            expertflag = 0;
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listView1.CheckedItems.Count==1){
                string deletecommand = string.Format("delete * from CheckPlacement_Data where Teacher like  '%{0}%' and Class_week={1}", listView1.CheckedItems[0].SubItems[6].Text, listView1.CheckedItems[0].SubItems[7].Text);
                SqlHelper help = new SqlHelper();
               if (help.Oledbcommand(deletecommand)!=0)
               {
                   flashListview_check();
                   MessageBox.Show("删除成功");
               } 
               else
               {
                   MessageBox.Show("删除失败");
               }
              

            }
        }

      
       

       
     

    }
}
