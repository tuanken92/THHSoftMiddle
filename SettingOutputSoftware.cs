using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using THHSoftMiddle.Source;

namespace THHSoftMiddle
{
    public partial class SettingOutputSoftware : Form
    {

        int max_barcode;


        int old_index = 1;
        Dictionary<int, Config_Out_Param> dic_barcode;
        Config_Out_Param cur_config;


        //click
        List<Point> list_click;
        Dictionary<string, IntPtr> dic_programssss;
        IntPtr thhHwnd = IntPtr.Zero;
        IntPtr programHandle = IntPtr.Zero;
        string programName;
        Point programLocation;
        Point thhLocation;
        int offsetY = 30;

        public SettingOutputSoftware()
        {
            InitializeComponent();
        }

        public SettingOutputSoftware(Dictionary<int, Config_Out_Param> dic_barcode, int max_barcode)
        {
            InitializeComponent();
            this.max_barcode = max_barcode;


            Init_Variables();
            Init_UI();

            this.dic_barcode = dic_barcode;
            cur_config = new Config_Out_Param();
            if (dic_barcode.ContainsKey(0))
            {
                cur_config = this.dic_barcode[0];
            }
            Init_GUI();
            Update_GUI(0);


        }

        void Init_Variables()
        {

            thhHwnd = Process.GetCurrentProcess().Handle;
            thhLocation = ClickOnPointTool.Get_Position_Hwnd(thhHwnd);
            Console.WriteLine($"thh Location = {thhLocation.ToString()}");
            lbStatus.Text = thhLocation.ToString();

            //list_click
            list_click = new List<Point>();

        }

        private void cbbListWindow_SelectedIndexChanged(object sender, EventArgs e)
        {
            programName = ((KeyValuePair<string, IntPtr>)cbbListWindow.SelectedItem).Key;
            programHandle = dic_programssss[programName];
            txtHWND.Text = string.Format("#{0:X}", programHandle);
            MyDefine.SetForegroundWindow(programHandle);



            programLocation = ClickOnPointTool.Get_Position_Hwnd(programHandle);
            Console.WriteLine($"Position of Program: {programLocation.ToString()}");
            lbStatus.Text = programLocation.ToString();
            //MyDefine.SetForegroundWindow(thhHwnd);
            //this.Focus();

        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.X))
            {
                Point clientPoint = new Point(Math.Abs(Cursor.Position.X - programLocation.X), Math.Abs(Cursor.Position.Y - programLocation.Y - offsetY));
                /*txtMouseX.Text = clientPoint.X.ToString();
                txtMouseY.Text = clientPoint.Y.ToString();*/
                listBoxClick.Items.Add(clientPoint);
                Console.WriteLine($"CurPos: {Cursor.Position.ToString()}, offset pos: {clientPoint.ToString()}");

                list_click.Add(clientPoint);
                return true;
            }
            else if (keyData == (Keys.Control | Keys.R))
            {
                //run thread
                Thread th = new Thread(Run_Click_Trip);
                th.Start();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        void Run_Click_Trip()
        {
            //click
            foreach (var click_item in list_click)
            {
                ClickOnPointTool.ClickOnPoint(programHandle, click_item);
                Thread.Sleep(1000);
            }

            //puttext
            SendKeys.SendWait("hello");
        }

        void Init_UI()
        {

            //combobox
            MyDefine.GetDesktopWindowHandlesAndTitles(out dic_programssss);
            Console.WriteLine("------------begin---------------");
            foreach (KeyValuePair<string, IntPtr> item in dic_programssss)
            {
                Console.WriteLine(item.Key + "\t" + item.Value);
            }
            Console.WriteLine("------------end---------------");
            cbbListWindow.DataSource = new BindingSource(dic_programssss, null);
            cbbListWindow.DisplayMember = "Key";
            cbbListWindow.ValueMember = "Key";
        }

        void Init_GUI()
        {
            nbUpDownBarcode.Minimum = 1;
            nbUpDownBarcode.Maximum = this.max_barcode;

            foreach(var x in cur_config.List_out_click)
            {
                listBoxClick.Items.Add(x);
            }

        }
        void Update_GUI(int key)
        {
            if (dic_barcode.ContainsKey(key))
            {
                cur_config = dic_barcode[key];
                cur_config.Print_Infor();
                //check condition forward
                if (string.IsNullOrEmpty(cur_config.Key_check_fw))
                {
                    chbxDirect.Checked = true;
                }
                else
                {
                    chbxDirect.Checked = false;
                    txtCheckDataForward.Text = cur_config.Key_check_fw;
                }

                //Comport + baudrate
                txtComport.Text = cur_config.Out_com.Comport;
                txtBaudrate.Text = cur_config.Out_com.Baudrate.ToString();


            }
            else
            {
                Console.WriteLine("-------xxx------");
                //check condition forward
                chbxDirect.Checked = true;

                //Comport + baudrate
                txtComport.Text = "COM1";
                txtBaudrate.Text = "9600";

                //List click
                //listBoxClick.ResetText();
                listBoxClick.Items.Clear();
            }
        }

        void Get_Param()
        {
            //check condition forward
            if (chbxDirect.Checked)
            {
                cur_config.Key_check_fw = null;
            }
            else
            {
                cur_config.Key_check_fw = txtCheckDataForward.Text;
            }


            //Comport + baudrate
            cur_config.Out_com.Comport = txtComport.Text;
            cur_config.Out_com.Baudrate = int.Parse(txtBaudrate.Text);

            //list click
            cur_config.List_out_click.Clear();
            foreach(var p in listBoxClick.Items)
            {
                cur_config.List_out_click.Add((Point)p);
            }

        }

        

        private void btnClick_Event(object sender, EventArgs e)
        {
            var cur_button = sender as Button;
            switch(cur_button.Name)
            {
                case "btnOk":
                    Close();
                    DialogResult = DialogResult.OK;
                    break;
                case "btnCancel":
                    Close();
                    break;
                case "btnApply":
                    Apply_Param();
                    break;
                case "btnDel":
                    cur_config.List_out_click.RemoveAt(listBoxClick.SelectedIndex);
                    listBoxClick.Items.RemoveAt(listBoxClick.SelectedIndex);
                    break;

            }
        }

        private void Apply_Param()
        {
            int cur_code = (int)nbUpDownBarcode.Value;
            Get_Param();
            dic_barcode[cur_code] = cur_config;
            cur_config.Print_Infor();
        }

        private void chbxDirect_CheckedChanged(object sender, EventArgs e)
        {
            if(chbxDirect.Checked)
            {
                txtCheckDataForward.Enabled = false;
            }
            else
            {
                txtCheckDataForward.Enabled = true;
            }
        }

        private void nbUpDownBarcode_ValueChanged(object sender, EventArgs e)
        {
            var cur_barcode = (int)nbUpDownBarcode.Value;
            if (!dic_barcode.ContainsKey(old_index))
            {
                MessageBox.Show($"Barcode {old_index} not yet setting");
                nbUpDownBarcode.Value = old_index;
                return;
            }
            old_index = cur_barcode;
            Update_GUI(cur_barcode);
            list_click.Clear();
            
        }

       
    }
}
