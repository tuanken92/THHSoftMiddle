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
        public Dictionary<int, Config_Out_Param> dic_barcode;
        Config_Out_Param cur_config;


        //click
        List<Point> list_click;
        //Dictionary<string, IntPtr> dic_programssss;
        List<Windown_Infor> list_program;
        IntPtr thhHwnd = IntPtr.Zero;
        public IntPtr programHandle = IntPtr.Zero;
        public string programName;
        Point programLocation;
        Point thhLocation;

        int offsetX = 0;
        int offsetY = 30;
        int time_delay = 100;


        public SettingOutputSoftware()
        {
            InitializeComponent();
        }

        public SettingOutputSoftware(Config_Common_Param config_common_param)
        {
            this.max_barcode = config_common_param.Number_barcode;
            this.offsetY = config_common_param.Offset_x;
            this.offsetY = config_common_param.Offset_y;
            this.time_delay = config_common_param.Time_delay;
            this.programName = config_common_param.Target_name;
            
            InitializeComponent();

            Init_Variables();
            Init_UI(config_common_param.Target_name);

            this.dic_barcode = config_common_param.Dic_barcode;
            cur_config = new Config_Out_Param();
            if (dic_barcode.ContainsKey(1))
            {
                cur_config = this.dic_barcode[1];
            }
            Init_GUI();
            Update_GUI(1);


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
            Windown_Infor window_infor = (Windown_Infor)cbbListWindow.SelectedItem;
            programName = window_infor.Name;
            programHandle = (IntPtr)window_infor.Hwnd;
            txtHWND.Text = string.Format("#{0:X}", programHandle);
            MyDefine.SetForegroundWindow(programHandle);



            programLocation = ClickOnPointTool.Get_Position_Hwnd(programHandle);
            Console.WriteLine($"Position of Program: {programLocation.ToString()}");
            lbStatus.Text = programLocation.ToString();
            

        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.X))
            {
                Point clientPoint = new Point(Math.Abs(Cursor.Position.X - programLocation.X - offsetX), 
                                              Math.Abs(Cursor.Position.Y - programLocation.Y - offsetY));
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

        void Run_Click_Barcode(int barcode_index)
        {
            //if (cur_config.List_out_click.Count <= 0)
            //{
            //    MessageBox.Show($"barcode {barcode_index} click is empty");
            //    return;
            //}
            //click
            foreach (var click_item in cur_config.List_out_click)
            {
                Console.WriteLine(click_item);
                ClickOnPointTool.ClickOnPoint(programHandle, click_item);
                Thread.Sleep(time_delay);
            }

            //puttext
            SendKeys.SendWait("{END}");
            SendKeys.SendWait($"hello barcode {barcode_index}");
            Console.WriteLine("~~~~~~~~~~~~~");
        }
        void Run_Click_Trip()
        {
            foreach (var click_item in list_click)
            {
                ClickOnPointTool.ClickOnPoint(programHandle, click_item);
                Thread.Sleep(time_delay);
            }

            //puttext
            SendKeys.SendWait("{END}");
            SendKeys.SendWait("hello");
        }

        void Init_UI(string target_name)
        {

            /*//combobox
            MyDefine.GetDesktopWindowHandlesAndTitles(out list_program);
            //Console.WriteLine("------------begin---------------");
            bool is_target_name_exist_in_list = false;
            foreach (var x in list_program)
            {

                //Console.WriteLine(x.Name + "\t" + x.Hwnd);
                if(x.Name == target_name)
                {
                    is_target_name_exist_in_list = true;
                }
            }

            //Console.WriteLine("------------end---------------");
            //cbbListWindow.DataSource = new BindingSource(dic_programssss, null);
            cbbListWindow.DataSource = new BindingSource(list_program, null);
            cbbListWindow.DisplayMember = "Name";
            cbbListWindow.ValueMember = "Name";*/

            /*if(dic_programssss.ContainsKey(target_name))
                cbbListWindow.Text = target_name;*/

            /*if (is_target_name_exist_in_list)*/
            cbbListWindow.Text = target_name;
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
                Console.WriteLine("key = " + key);
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


                //list click
                listBoxClick.Items.Clear();
                foreach (var p in cur_config.List_out_click)
                {
                    listBoxClick.Items.Add(p);
                }
            }
            else
            {
                Console.WriteLine("-------xxx------");
                //check condition forward
                chbxDirect.Checked = true;

                txtCheckDataForward.Text = "***";

                //Comport + baudrate
                txtComport.Text = "COM1";
                txtBaudrate.Text = "9600";

                //List click
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
            Console.WriteLine(cur_button.Name);
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
                case "btnTestClick":
                    Run_Click_Barcode((int)nbUpDownBarcode.Value);
                    break;
                case "btnDelAll":
                    cur_config.List_out_click.Clear();
                    listBoxClick.Items.Clear();
                    break;



            }
        }

        private void Apply_Param()
        {
            int cur_code = (int)nbUpDownBarcode.Value;
            Get_Param();
            dic_barcode[cur_code] = cur_config;
            Console.WriteLine("cur_code = " + cur_code);
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
