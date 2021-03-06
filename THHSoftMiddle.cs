using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using THHSoftMiddle.Source;

namespace THHSoftMiddle
{
    public partial class THHSoftMiddle : Form
    {
        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int BN_CLICKED = 245;

        RS232 rs232;
        TcpIPClient tcp_client;

        //format text
        LeadingText leadingText;
        CustomerData custom_text;
        TerminatingText terminating_text;
        string text_demo = null;

        //format string
        Config_Format_String config_format_string;

        //common param
        Config_Common_Param config_common_param;

        //operator vairable

        int number_barcode_processed = 0;
        bool is_start_program = false;
        string data_barcode_input;

        //thread heartbeat
        Thread th_heartbeat = null;
        bool en_th_heartbeat;


        //thread comport
        Thread th_read_input = null;
        bool en_th_read_input;


        bool b_rs232 = false;
        bool b_tcp = false;

        #region main_process
        void Init_Thread()
        {
            if (th_heartbeat == null)
            {
                th_heartbeat = new Thread(new ThreadStart(Thread_HeartBeat));
                th_heartbeat.Name = "Thread heartbeat";
                th_heartbeat.IsBackground = true;
                en_th_heartbeat = true;

            }

            if (th_read_input == null)
            {
                th_read_input = new Thread(new ThreadStart(Run_Thread_Input));
                th_read_input.Name = "Thread read input";
                th_read_input.IsBackground = true;
                en_th_read_input = true;
            }

        }
        private void btnClick_MainProcess(object sender, EventArgs e)
        {
            var cur_btn = sender as Button;
            switch (cur_btn.Name)
            {
                case "btnRun":
                    Start_Program();
                    break;

                case "btnStop":
                    Stop_Program();
                    break;

                case "btnReset":
                    Reset_Program();
                    break;
                case "btnHide":
                    this.Hide();
                    notifyIcon.Visible = true;
                    break;
            }
        }


        void Start_Program()
        {
            if(!is_start_program)
            {


                //handler program 
                Add_Log(eListBox.LB_HEARTBEAT, String.Format("Try to find window {0}", config_common_param.Target_name), Color.Orange);
                programHandle = MyDefine.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, config_common_param.Target_name);
                if(programHandle == IntPtr.Zero)
                {
                    Add_Log(eListBox.LB_HEARTBEAT, String.Format("Not found window {0}", config_common_param.Target_name), Color.Red);
                    Add_Log(eListBox.LB_HEARTBEAT, String.Format("Can not Start, check window {0}", config_common_param.Target_name), Color.DarkRed);
                    return;
                }
                Add_Log(eListBox.LB_HEARTBEAT, String.Format("Found window {0}, set focus", config_common_param.Target_name));
                MyDefine.SetForegroundWindow(programHandle);
                //open connect to input
                if (config_common_param.In_soft == input_soft.method_com)
                {
                    rs232 = new RS232(config_common_param.In_com.Comport, config_common_param.In_com.Baudrate);
                    b_rs232 = rs232.Open();
                    Add_Log(eListBox.LB_HEARTBEAT, String.Format("Open Serial port {0} = {1}", config_common_param.In_com.Comport, b_rs232), Color.Orange);
                    if(!b_rs232)
                    {
                        Add_Log(eListBox.LB_HEARTBEAT, String.Format("Can not Start, check Comport {0}", config_common_param.In_com.Comport), Color.DarkRed);
                        return;
                    }
                }
                else if(config_common_param.In_soft == input_soft.method_tcp)
                {
                    tcp_client = new TcpIPClient(config_common_param.In_tcp.Ip, config_common_param.In_tcp.Port);
                    b_tcp = tcp_client.Connect();
                    Add_Log(eListBox.LB_HEARTBEAT, String.Format("Connect {0}:{1} = {2}", config_common_param.In_tcp.Ip, config_common_param.In_tcp.Port, b_tcp), Color.Orange);
                    if(!b_tcp)
                    {
                        Add_Log(eListBox.LB_HEARTBEAT, String.Format("Can not Start, check Server {0}:{1}", config_common_param.In_tcp.Ip, config_common_param.In_tcp.Port), Color.DarkRed);
                        return;
                    }
                }

                Init_Thread();

                th_heartbeat.Start();
                th_read_input.Start();

                //run thread heartbeat
                en_th_heartbeat = true;


                //run thread read input
                en_th_read_input = true;


                is_start_program = true;
                Add_Log(eListBox.LB_HEARTBEAT, String.Format("Program started, waiting data ..."));
            }

        }

        void Stop_Program()
        {
            if (is_start_program)
            {

                //close connect to input
                if (config_common_param.In_soft == input_soft.method_com)
                {
                    if(b_rs232)
                        rs232.Close();
                }
                else if (config_common_param.In_soft == input_soft.method_tcp)
                {
                    if(b_tcp)
                        tcp_client.Disconnect();
                }

                en_th_heartbeat = false;
                en_th_read_input = false;

                th_heartbeat.Abort();
                th_read_input.Abort();
                th_heartbeat = null;
                th_read_input = null;

                is_start_program = false;
                Add_Log(eListBox.LB_HEARTBEAT, String.Format("Program stoped"), Color.DarkRed);
            }
        }

        void Reset_Program()
        {
            is_start_program = false;
            number_barcode_processed = 0;

            //reset all barcode
            for (int i = 0; i < config_common_param.Dic_barcode.Count; i++)
            {
                config_common_param.Dic_barcode.ElementAt(i).Value.Is_send = false;
            }

        }
        /// <summary>
        /// Input: data from serial port or TCP -> str_data_input
        /// </summary>
        void Run_Thread_Input2()
        {
            //splip dat by \r\n or somthing else
            while(en_th_read_input)
            {
                Thread.Sleep(100);

                if (config_common_param.In_soft == input_soft.method_com)
                {
                    if (!string.IsNullOrEmpty(rs232.Data_receive))
                    {
                        data_barcode_input = rs232.Data_receive;
                        Console.WriteLine("comport = {0}", rs232.Data_receive);
                        rs232.Data_receive = null;
                    }
                }
                else if (config_common_param.In_soft == input_soft.method_tcp)
                {
                    if(!tcp_client.Get_State())
                    {
                        en_th_read_input = false;
                        break;
                    }
                    if (!string.IsNullOrEmpty(tcp_client.Data_receive))
                    {
                        data_barcode_input = tcp_client.Data_receive;
                        Console.WriteLine("tcp = {0}", tcp_client.Data_receive);
                        tcp_client.Data_receive = null;
                    }
                }

                if(!string.IsNullOrEmpty(data_barcode_input))
                {
                    Console.WriteLine("comport: " + data_barcode_input);
                    Process_Barcode_Data(data_barcode_input);

                    /*Print_BarcodeData(data_barcode_input);
                    data_barcode_input = Run_Process_Format_Data(data_barcode_input);
                    var data = data_barcode_input;
                    data_barcode_input = null;
                    Thread th = new Thread(() => Run_Process_Ouput_System(data));
                    th.Start();*/
                    
                    data_barcode_input = null;


                }
            }
        }


        void Run_Thread_Input()
        {
            if (config_common_param.In_soft == input_soft.method_com)
            {
                while (en_th_read_input && b_rs232)
                {
                    Thread.Sleep(100);

                    if (!string.IsNullOrEmpty(rs232.Data_receive))
                    {
                        Console.WriteLine("comport: " + rs232.Data_receive);
                        Add_Log(eListBox.LB_BARCODE, rs232.Data_receive, Color.Blue);
                        Process_Barcode_Data(rs232.Data_receive);
                        rs232.Data_receive = null;
                    }
                }
            } 
            else if(config_common_param.In_soft == input_soft.method_tcp)
            {
                while (en_th_read_input && b_tcp)
                {
                    Thread.Sleep(100);

                    if (!string.IsNullOrEmpty(tcp_client.Data_receive))
                    {
                        Console.WriteLine("tcp: " + tcp_client.Data_receive);
                        Add_Log(eListBox.LB_BARCODE, tcp_client.Data_receive, Color.Blue);
                        Process_Barcode_Data(tcp_client.Data_receive);
                        tcp_client.Data_receive = null;
                    }
                }
            }
     
        }


        void Process_Barcode_Data(String data_barcode)
        {
            //Split data
            var list_barcode = data_barcode.Split(config_common_param.Char_split[0]);
            List<Data_Barcode> list_barcode_IS = new List<Data_Barcode>();

            foreach(var str_barcode in list_barcode)
            {
                Console.WriteLine(str_barcode);
                list_barcode_IS.Add(new Data_Barcode(str_barcode));
                Add_Log(eListBox.LB_BARCODE, str_barcode);
            }

            PushData pushData = new PushData();

            for (int i = 0; i < config_common_param.Dic_barcode.Count; i++)
            {
                var cur_barcode_infor = config_common_param.Dic_barcode.ElementAt(i).Value;
                //Console.WriteLine(cur_barcode_infor.Key_check_fw);

                for(int j  = 0; j < list_barcode_IS.Count; j++)
                {
                    if (list_barcode_IS[j].found)
                        continue;
                    if(list_barcode_IS[j].data.Contains(cur_barcode_infor.Key_check_fw))
                    {
                        pushData.List_data.Add(list_barcode_IS[j].data);
                        list_barcode_IS[j].found = true;
                        //Add_Log(eListBox.LB_BARCODE, list_barcode_IS[j].data);
                        break;
                    }
                }
 
            }

            if(pushData.List_data.Count == config_common_param.Dic_barcode.Count)
            {
                //check window
                programHandle = MyDefine.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, config_common_param.Target_name);
                if (programHandle == IntPtr.Zero)
                {
                    Add_Log(eListBox.LB_HEARTBEAT, String.Format("Not found window {0}", config_common_param.Target_name), Color.Red);
                    Add_Log(eListBox.LB_BARCODE, String.Format("Stopped Program! Please open \"{0}\"", config_common_param.Target_name), Color.Red);
                    Stop_Program();
                    return;
                }

                MyDefine.SetForegroundWindow(programHandle);
                bool b = pushData.Push();
                Console.WriteLine("Push data = " + b);
                Add_Log(eListBox.LB_BARCODE, String.Format("Push to Bartector = {0}",b), b?Color.Green:Color.DarkRed);
            }
            else
            {
                Console.WriteLine("Error!");
                Add_Log(eListBox.LB_BARCODE, "Not enough barcode data", Color.DarkRed);
            }

            //release
            list_barcode_IS.Clear();
            Add_Log(eListBox.LB_BARCODE, "----------------------------------");
        }
       

        /// <summary>
        /// input: str_data_input -> output with formated
        /// </summary>
        string Run_Process_Format_Data(string str_data_input)
        {
            //1.custom
            custom_text.Input_text = str_data_input;
            var custom_out = custom_text.Process();

            //2.leading
            leadingText.Input_text = custom_out;
            leadingText.Leading_text = config_format_string.Leading;
            var leading_out = leadingText.Process();

            //3.terminating
            terminating_text.Input_text = leading_out;
            terminating_text.Terminal_text = config_format_string.Terminating;
            var output_string = terminating_text.Process();
            Console.WriteLine($"String output_process = {output_string}");

            return output_string;
        }

        
        /// <summary>
        /// Input: str_data_barcode is data receive from input software
        /// Output: consider this data is satisfy the condition -> send data out
        /// </summary>
        /// <param name="str_data_barcode"></param>
        void Run_Process_Ouput_System(string str_data_barcode)
        {
            //consider input data
            if (string.IsNullOrEmpty(str_data_barcode))
            {
                Console.WriteLine("return");
                return;
            }

            //FIX-ME: check key = null

            //consider cast data
            for (int i = 0; i < config_common_param.Dic_barcode.Count; i++)
            {
                var cur_barcode_infor = config_common_param.Dic_barcode.ElementAt(i).Value;
                
                //if this string is send -> continue
                if (cur_barcode_infor.Is_send || cur_barcode_infor.Is_transmit_data)
                {
                    Console.WriteLine("continue 1");
                    continue;
                }

                //if this string is contain check_fw
                if(!string.IsNullOrEmpty(cur_barcode_infor.Key_check_fw))
                {
                    if (!str_data_barcode.Contains(cur_barcode_infor.Key_check_fw))
                    {
                        Console.WriteLine("continue 2");
                        continue;
                    }
                }

                //forward & set flag send = true
                Transmit_Data(config_common_param.Dic_barcode.ElementAt(i).Key, str_data_barcode);

            }
            
        }

        /// <summary>
        /// consider to transmit barcode & set flag send = true
        /// </summary>
        /// <param name="key_barcode"></param>
        void Transmit_Data(int key_barcode, string barcode)
        {
            config_common_param.Dic_barcode[key_barcode].Is_transmit_data = true;

            bool send_data = false;
            if (config_common_param.Out_soft == output_soft.method_click)
            {
                //click and send
                send_data = Send_Data_By_Click(key_barcode, barcode);
            }
            else if (config_common_param.Out_soft == output_soft.method_com)
            {
                //open comport and send
                send_data = Send_Data_By_Comport(key_barcode, barcode);
            }

            if(send_data)
            {
                config_common_param.Dic_barcode[key_barcode].Is_send = true;
                config_common_param.Dic_barcode[key_barcode].Is_transmit_data = false;
                number_barcode_processed++;
            }
        }

        bool Send_Data_By_Click(int key_barcode, string data)
        {
            bool send_result = true;
            MyDefine.SetForegroundWindow(programHandle);
            //click
            foreach (var click_item in config_common_param.Dic_barcode[key_barcode].List_out_click)
            {
                ClickOnPointTool.ClickOnPoint(programHandle, click_item);
                Thread.Sleep(config_common_param.Time_delay);
            }

            //puttext
            SendKeys.SendWait("{END}");
            SendKeys.SendWait($"{data}");
            return send_result;
        }

        bool Send_Data_By_Comport(int key_barcode, string data)
        {
            bool send_result = false;
            //open com
            RS232 rs232_temp  = new RS232(config_common_param.Dic_barcode[key_barcode].Out_com);
            send_result = rs232_temp.Open();
            //send data
            send_result = rs232_temp.SendData(data);
            //close com
            send_result = rs232_temp.Close();
            return send_result;
        }




        /// <summary>
        /// Consider: 1. all thread is run
        ///           2. quanti barcode is enough with target  
        ///           3. if done process -> reset all param
        /// </summary>
        void Thread_HeartBeat()
        {
            while(en_th_heartbeat)
            {
                Thread.Sleep(1500);

                
                if (config_common_param.In_soft == input_soft.method_com)
                {
                    //check com
                    b_rs232 = rs232.Get_State();
                }
                else if (config_common_param.In_soft == input_soft.method_tcp)
                {
                    //check tcp
                    b_tcp = tcp_client.Get_State();
                }





                

                //check thread 2





                //print infor
                Print_HeartBeat();
            }
        }

        private delegate void SafeCallDelegate();
        void Print_HeartBeat()
        {
            if (listBoxHeartBeat.InvokeRequired)
            {
                var d = new SafeCallDelegate(Print_HeartBeat);
                listBoxHeartBeat.Invoke(d);
            }
            else
            {
                Add_Log(eListBox.LB_HEARTBEAT, "---------------------", Color.DarkOrange);
                if (config_common_param.In_soft == input_soft.method_com)
                {
                    b_rs232 = rs232.Get_State();
                    Add_Log(eListBox.LB_HEARTBEAT, $"Serial status: {b_rs232}", Color.DarkOrange);
                }
                else if (config_common_param.In_soft == input_soft.method_tcp)
                {
                    b_tcp = tcp_client.Get_State();
                    Add_Log(eListBox.LB_HEARTBEAT, $"Socket status: {b_tcp}", Color.DarkOrange);
                }

                Add_Log(eListBox.LB_HEARTBEAT, $"Thread Read_Input status: {th_read_input.IsAlive}", Color.DarkOrange);
                Add_Log(eListBox.LB_HEARTBEAT, "---------------------", Color.DarkOrange);

                //stop program
                if (config_common_param.In_soft == input_soft.method_com)
                {
                    if(!b_rs232)
                    {
                        Add_Log(eListBox.LB_BARCODE, $"Check Serial Port: {config_common_param.In_com.Comport}, lost connect", Color.DarkRed);
                        Stop_Program();
                    }
                }
                else if (config_common_param.In_soft == input_soft.method_tcp)
                {
                    if(!b_tcp)
                    {
                        Add_Log(eListBox.LB_BARCODE, $"Check Server: {config_common_param.In_tcp.Ip}:{config_common_param.In_tcp.Port}, lost connect", Color.DarkRed);
                        Stop_Program();
                    }
                    
                }

            }

            
        }

        private delegate void SafeCallDelegate2(string text);
        void Print_BarcodeData(string data)
        {
            if (listBoxBarcodeState.InvokeRequired)
            {
                var d = new SafeCallDelegate2(Print_BarcodeData);
                listBoxBarcodeState.Invoke(d, new object[] { data });
            }
            else
            {
                listBoxBarcodeState.Items.Insert(0, data);
            }


        }
        #endregion

        public THHSoftMiddle()
        {
            InitializeComponent();
            Initial();
            Init_GUI();
            Init_Thread();

        }


        void Init_GUI()
        {
            //format data string
            chbxLeadingText.Checked = config_format_string.Using_leading;
            txtLeadingText.Text = config_format_string.Leading;

            chbxTerminatingText.Checked = config_format_string.Using_terminating;
            txtTerminatingText.Text = config_format_string.Terminating;
            chbxCRLF.Checked = config_format_string.Using_crlf;
            chbxTab.Checked = config_format_string.Using_tab;

            chbxUseCustomData.Checked = config_format_string.Using_data_format;
            chbxUpperText.Checked = config_format_string.Using_upper;
            chbxLowerText.Checked = config_format_string.Using_lower;
            chbxTrimText.Checked = config_format_string.Using_removespace;
            chbxCutText.Checked = config_format_string.Using_cut;
            nbUpdownBegin.Value = (decimal)config_format_string.Pos_begin;
            nbUpdownEnd.Value = (decimal)config_format_string.Pos_end;

            //common param
            cbxInputSoft.DataSource = Enum.GetValues(typeof(input_soft));
            cbxOutputSoft.DataSource = Enum.GetValues(typeof(output_soft));
            

            txtTargetWindow.Text = config_common_param.Target_name;
            txtTargetHwnd.Text = config_common_param.Target_hwnd.ToString();
            txtOffsetX.Text = config_common_param.Offset_x.ToString();
            txtOffsetY.Text = config_common_param.Offset_y.ToString();
            txtTimeDelay.Text = config_common_param.Time_delay.ToString();
            nbUpdownNumberCode.Value = config_common_param.Number_barcode;
            cbxInputSoft.SelectedIndex = (int)config_common_param.In_soft;
            cbxOutputSoft.SelectedIndex = (int)config_common_param.Out_soft;
            txtSplitChar.Text = config_common_param.Char_split;


            //list box
            //listBoxHeartBeat.BackColor = Color.Beige;
            listBoxHeartBeat.DrawMode = DrawMode.OwnerDrawFixed;
            listBoxHeartBeat.DrawItem += ListBoxHeartBeat_DrawItem;

            //listBoxBarcodeState.BackColor = Color.Beige;
            listBoxBarcodeState.DrawMode = DrawMode.OwnerDrawFixed;
            listBoxBarcodeState.DrawItem += ListBoxHeartBeat_DrawItem;


            notifyIcon.ShowBalloonTip(500);

        }

        private void ListBoxHeartBeat_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;
            var item = ((ListBox)sender).Items[e.Index] as ColoredItem;

            if (item != null)
            {
                e.Graphics.DrawString(
                    item.Text,
                    e.Font,
                    new SolidBrush(item.Color),
                    e.Bounds);
            }

            /*try
            {
                e.DrawBackground();
                Brush myBrush = Brushes.Green;

                
                e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(),
                e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);

                e.DrawFocusRectangle();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }*/
        }

        void Initial()
        {
            /*//comport
            string com_name = txtComName.Text;
            int com_baud = int.Parse(txtComBaud.Text);
            rs232 = new RS232(com_name, com_baud);

            //comport
            string ip = txtTcpIP.Text;
            int port = int.Parse(txtTcpPort.Text);
            tcp_client = new TcpIPClient(ip, port);*/


            //data format
            text_demo = txtDataFormatDemo.Text;
            leadingText = new LeadingText();
            custom_text = new CustomerData();
            terminating_text = new TerminatingText();
            custom_text.Input_text = txtDataFormatDemo.Text;

            //config format string
            config_format_string = new Config_Format_String();
            if (MyDefine.File_Is_Exist(MyDefine.file_config_format_data))
            {
                config_format_string = SaveLoad_Parameter.Load_Parameter(config_format_string, MyDefine.file_config_format_data) as Config_Format_String;
            }
            else
            {
                MessageBox.Show($"Not found {MyDefine.file_config_format_data}");
            }


            //common param
            config_common_param = new Config_Common_Param();
            if (MyDefine.File_Is_Exist(MyDefine.file_config_common_param))
            {
                config_common_param = SaveLoad_Parameter.Load_Parameter(config_common_param, MyDefine.file_config_common_param) as Config_Common_Param;
            }
            else
            {
                MessageBox.Show($"Not found {MyDefine.file_config_common_param}");
            }



        }

        private void timerDateTime_Tick(object sender, EventArgs e)
        {
            this.lbDateTime.Text = DateTime.Now.ToString("ddd MM/dd/yyyy\nhh:mm:ss tt");
        }

        void Get_Common_Param()
        {
            var number_barcode = (int)nbUpdownNumberCode.Value;
            config_common_param.Target_name = txtTargetWindow.Text;
            config_common_param.Target_hwnd = int.Parse(txtTargetHwnd.Text);
            config_common_param.Offset_x = int.Parse(txtOffsetX.Text);
            config_common_param.Offset_y = int.Parse(txtOffsetY.Text);
            config_common_param.Time_delay = int.Parse(txtTimeDelay.Text);
            config_common_param.Number_barcode = number_barcode;
            config_common_param.In_soft = (input_soft)cbxInputSoft.SelectedIndex;
            config_common_param.Out_soft = (output_soft)cbxOutputSoft.SelectedIndex;
            config_common_param.Char_split = txtSplitChar.Text;

            
            for (int i = 1; i < number_barcode + 1; i++)
            {
                Console.WriteLine(i);
                if(!config_common_param.Dic_barcode.ContainsKey(i))
                {
                    config_common_param.Dic_barcode[i] = new Config_Out_Param();
                }
            }

            for(int i = number_barcode+1; i < config_common_param.Dic_barcode.Count+1; i++)
            {
                Console.WriteLine("need clear" + i);
                if (config_common_param.Dic_barcode.ContainsKey(i))
                {
                    config_common_param.Dic_barcode.Remove(i);
                    Console.WriteLine("Clear " + i);
                }
            }
            
        }
        void Get_Config_Format()
        {
            config_format_string.Using_leading = chbxLeadingText.Checked;
            config_format_string.Leading = txtLeadingText.Text;

            config_format_string.Using_terminating = chbxTerminatingText.Checked;
            config_format_string.Terminating = txtTerminatingText.Text;

            config_format_string.Using_crlf = chbxCRLF.Checked;
            config_format_string.Using_tab = chbxTab.Checked;
            config_format_string.Using_data_format = chbxUseCustomData.Checked;
            config_format_string.Using_upper = chbxUpperText.Checked;
            config_format_string.Using_lower = chbxLowerText.Checked;
            config_format_string.Using_removespace = chbxTrimText.Checked;
            config_format_string.Using_cut = chbxCutText.Checked;

            config_format_string.Pos_begin = (int)nbUpdownBegin.Value;
            config_format_string.Pos_end = (int)nbUpdownEnd.Value;

        }

        IntPtr programHandle = IntPtr.Zero;

        private void btn_Click_Event(object sender, EventArgs e)
        {
            var curButton = sender as Button;
            Console.WriteLine($"btn_Click_Event name = {curButton.Name}");
            switch (curButton.Name)
            {
                #region rs232_button
                case "btnComConnect":
                    if (rs232 == null)
                        return;
                    bool com_state = rs232.Get_State();
                    if (!com_state)
                    {
                        if (rs232.Open())
                            btnComConnect.Text = "Connected";
                    }
                    else
                    {
                        if (rs232.Close())
                            btnComConnect.Text = "Disconnected";

                    }
                    break;
                case "btnComClearData":
                    lbxComDataReceive.Items.Clear();
                    break;
                case "btnComSend":
                    if (rs232 == null)
                        return;
                    rs232.SendData(txtComDataToSend.Text);
                    break;
                #endregion

                #region tcpip_client_button
                case "btnTcpOpen":
                    bool tcp_client_state = tcp_client.Get_State();
                    if (!tcp_client_state)
                    {
                        if (tcp_client.Connect())
                            btnTcpOpen.Text = "Connected";
                    }
                    else
                    {
                        if (tcp_client.Disconnect())
                            btnTcpOpen.Text = "Disconnected";

                    }
                    break;
                case "btnTcpClearData":
                    lbxTcpDataReceive.Items.Clear();
                    break;
                case "btnTcpSend":
                    tcp_client.SendData(txtTcpDataToSend.Text);
                    break;
                #endregion

                #region development
                case "btnCapture":
                    var screen_image = MyDefine.PrintWindow(programHandle);
                    MyDefine.Save_BitMap(screen_image);
                    break;
                case "btnTest":
                    //Get a handle for the "+" button
                    var hwndChild = MyDefine.FindWindowEx(programHandle, IntPtr.Zero, "TPageControl", null);

                    //send BN_CLICKED message
                    //SendMessage((int)hwndChild, BN_CLICKED, 0x00050b0c, 0x02020001);


                    var udp_setup_tab = MyDefine.FindWindowEx(hwndChild, IntPtr.Zero, "TTabSheet", "UDP Setup");
                    var find_devices_btn = MyDefine.FindWindowEx(udp_setup_tab, IntPtr.Zero, "TBitBtn", "&Find devices");
                    MyDefine.SendMessage((int)find_devices_btn, BN_CLICKED, 0, IntPtr.Zero);

                    var serial_setup_tab = MyDefine.FindWindowEx(hwndChild, IntPtr.Zero, "TTabSheet", "Serial");
                    var tcp_setup_tab = MyDefine.FindWindowEx(hwndChild, IntPtr.Zero, "TTabSheet", "TCP Server");
                    var about_setup_tab = MyDefine.FindWindowEx(hwndChild, IntPtr.Zero, "TTabSheet", "About");


                    hwndChild = MyDefine.FindWindowEx(programHandle, IntPtr.Zero, "TPageControl", "UDP Setup");

                    break;
                case "btnWrite":
                    // Get a handle to the Calculator application. The window class
                    // and window name were obtained using the Spy++ tool.


                    // Verify that Calculator is a running process.
                    if (programHandle == IntPtr.Zero)
                    {
                        MessageBox.Show("Program is not running.");
                        return;
                    }

                    // Make Calculator the foreground application and send it
                    // a set of calculations.
                    MyDefine.SetForegroundWindow(programHandle);


                    if(chbxEnter.Checked)
                        SendKeys.SendWait(txtMessage.Text + "\r");
                    else
                        SendKeys.SendWait(txtMessage.Text);
                    Console.WriteLine($"Wrote {txtMessage.Text}");

                    break;

                case "btnWrite2":
                    // Get a handle to the Calculator application. The window class
                    // and window name were obtained using the Spy++ tool.


                    // Verify that Calculator is a running process.
                    if (programHandle == IntPtr.Zero)
                    {
                        MessageBox.Show("Program is not running.");
                        return;
                    }

                    // Make Calculator the foreground application and send it
                    // a set of calculations.
                    MyDefine.SetForegroundWindow(programHandle);


                    if (chbxEnter.Checked)
                    {
                        SendKeys.SendWait("tuan1");
                        SendKeys.SendWait(txtMessage.Text + "\r");
                        SendKeys.SendWait("tuan2");
                        SendKeys.SendWait(txtMessage.Text + "\r");
                        SendKeys.SendWait("tuan3");
                        SendKeys.SendWait(txtMessage.Text + "\r");
                        SendKeys.SendWait("tuan4");
                        SendKeys.SendWait(txtMessage.Text + "\r");
                        SendKeys.SendWait("tuan5");
                        SendKeys.SendWait(txtMessage.Text + "\r");
                    }
                    else
                        SendKeys.SendWait(txtMessage.Text);
                    Console.WriteLine($"Wrote {txtMessage.Text}");

                    break;
                case "btnMove":

                    break;
                case "btnText":
                    IntPtr button_handel = MyDefine.FindWindowEx(programHandle, IntPtr.Zero, null, "Open"); // Lấy handle của nút "Thông tin" theo text
                    MyDefine.SendMessage((int)button_handel, 0x00F5, (int)IntPtr.Zero, IntPtr.Zero); // Gửi tin nhắn BM_CLICK=0x005F để click
                    break;

                case "btnFind":
                    // Get a handle to the Calculator application. The window class
                    // and window name were obtained using the Spy++ tool.
                    var class_name = txtClassName.Text;
                    var window_name = txtWindowName.Text;
                    if (string.IsNullOrWhiteSpace(class_name))
                        class_name = null;
                    if (string.IsNullOrWhiteSpace(window_name))
                        window_name = null;

                    Console.WriteLine($"class name = {class_name}, window name = {window_name}");
                    /*programHandle = MyDefine.FindWindow(class_name, window_name);*/

                    programHandle = MyDefine.GetControl(window_name);

                    // Verify that Calculator is a running process.
                    if (programHandle == IntPtr.Zero)
                    {
                        MessageBox.Show($"{window_name} is not running");
                    }
                    else
                    {
                        Console.WriteLine($"{window_name} is running");
                        MyDefine.SetForegroundWindow(programHandle);
                    }

                    break;
                case "btnFindChild":
                    
                    var wd_bartector = txtWindowName.Text;
                    var wd_child = txtChildWindow.Text;
                    
                    IntPtr child_window = MyDefine.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, wd_child);

                    Console.WriteLine($"window: {programHandle} & {child_window}");
                    // Verify that Calculator is a running process.
                    if (child_window == IntPtr.Zero)
                    {
                        MessageBox.Show($"{wd_child} is not running");
                    }
                    else
                    {
                        programHandle = child_window;
                        Console.WriteLine($"{wd_child} is running");
                        MyDefine.SetForegroundWindow(child_window);
                    }
                    break;
                case "btnClear":
                    txtClassName.Text = "";
                    txtWindowName.Text = "";
                    break;
                case "btnClearMouse":
                    txtMouseX.Text = "";
                    txtMouseY.Text = "";
                    break;
                case "btnGoHome":
                    SendKeys.Send("{HOME}");
                    break;
                case "btnGoEnd":
                    SendKeys.Send("{END}");
                    break;
                case "btnClick":
                    int xpos;
                    int ypos;
                    bool x_parse = int.TryParse(txtMouseX.Text, out xpos);
                    bool y_parse = int.TryParse(txtMouseY.Text, out ypos);
                    if (x_parse && y_parse)
                    {
                        ClickOnPointTool.ClickOnPoint(programHandle, new Point(xpos, ypos));
                    }
                    break;
                #endregion

                #region format_data_ouput
                case "btnSaveDataFormat":
                    Get_Config_Format();
                    SaveLoad_Parameter.Save_Parameter(config_format_string, MyDefine.file_config_format_data);
                    break;

                case "btnLoadDataFormat":
                    config_format_string = SaveLoad_Parameter.Load_Parameter(config_format_string, MyDefine.file_config_format_data) as Config_Format_String;
                    break;

                case "btnTestFormatOutput":
                    switch (cbxModeTest.SelectedIndex)
                    {
                        case (int)type_of_test_format.test_leading:
                            leadingText.Input_text = txtInputStringToTest.Text;
                            leadingText.Leading_text = txtLeadingText.Text;
                            lbOutputData.Text = leadingText.Process();
                            break;
                        case (int)type_of_test_format.test_data:
                            custom_text.Input_text = txtInputStringToTest.Text;
                            lbOutputData.Text = custom_text.Process();
                            break;
                        case (int)type_of_test_format.test_terminating:
                            terminating_text.Input_text = txtInputStringToTest.Text;
                            terminating_text.Terminal_text = txtTerminatingText.Text;
                            lbOutputData.Text = terminating_text.Process();
                            break;
                        case (int)type_of_test_format.test_all:
                            //1.custom
                            custom_text.Input_text = txtInputStringToTest.Text;
                            var custom_out = custom_text.Process();

                            //2.leading
                            leadingText.Input_text = custom_out;
                            leadingText.Leading_text = txtLeadingText.Text;
                            var leading_out = leadingText.Process();

                            //3.terminating
                            terminating_text.Input_text = leading_out;
                            terminating_text.Terminal_text = txtTerminatingText.Text;
                            lbOutputData.Text = terminating_text.Process();
                            break;

                    }
                    break;

                #endregion

                #region setting_common_param
                case "btnSaveCommonParam":
                    Get_Common_Param();
                    SaveLoad_Parameter.Save_Parameter(config_common_param, MyDefine.file_config_common_param);
                    break;

                case "btnSettingInputSoft":
                    Get_Common_Param();
                    SettingInputSoftware setting_input_dlg = new SettingInputSoftware(
                        config_common_param.In_com,
                        config_common_param.In_tcp
                        );
                    if(setting_input_dlg.ShowDialog() == DialogResult.OK)
                    {
                        config_common_param.In_com = setting_input_dlg.in_com;
                        config_common_param.In_tcp = setting_input_dlg.in_tcp;
                        Console.WriteLine("Update input param");

                    }
                    setting_input_dlg.Dispose();
                    break;

                case "btnSettingOutput":
                    Get_Common_Param();
                    SettingOutputSoftware setting_output_dlg = new SettingOutputSoftware(config_common_param);
                    if (setting_output_dlg.ShowDialog() == DialogResult.OK)
                    {
                        config_common_param.Dic_barcode = setting_output_dlg.dic_barcode;
                        //config_common_param.Target_name = setting_output_dlg.programName;
                        //config_common_param.Target_hwnd = (int)setting_output_dlg.programHandle;
                        txtTargetWindow.Text = config_common_param.Target_name;
                        txtTargetHwnd.Text = config_common_param.Target_hwnd.ToString();
                        Console.WriteLine("Update output param");
                    }
                    setting_output_dlg.Dispose();
                    break;
                #endregion
            }
        }





        private void THHSoftMiddle_Load(object sender, EventArgs e)
        {
            timerDateTime.Start();
        }

        private void THHSoftMiddle_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            // Display a MsgBox asking the user to save changes or abort.
            if (MessageBox.Show("Do you want to Close this Application", "THHSoftware",
               MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                timerDateTime.Stop();
                Stop_Program();
            }
        }


        private void Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            var cur_checkbox = sender as CheckBox;
            Console.WriteLine($"Checkbox {cur_checkbox.Name} = {cur_checkbox.Checked}");

            switch (cur_checkbox.Name)
            {
                #region Format data
                case "chbxLeadingText":
                    if (chbxLeadingText.Checked)
                    {
                        txtLeadingText.Enabled = true;
                        leadingText.Leading_text = txtLeadingText.Text;

                    }
                    else
                    {
                        txtLeadingText.Enabled = false;
                        leadingText.Leading_text = null;
                    }
                    leadingText.Is_using = chbxLeadingText.Checked;

                    /*config_format_string.Using_leading = chbxLeadingText.Checked;
                    config_format_string.Leading = txtLeadingText.Text;*/
                    break;
                case "chbxUseCustomData":
                    if (chbxUseCustomData.Checked)
                    {
                        chbxUpperText.Enabled = true;
                        chbxLowerText.Enabled = true;
                        chbxTrimText.Enabled = true;
                        chbxCutText.Enabled = true;
                        txtDataFormatDemo.Enabled = true;
                    }
                    else
                    {
                        chbxUpperText.Enabled = false;
                        chbxLowerText.Enabled = false;
                        chbxTrimText.Enabled = false;
                        chbxCutText.Enabled = false;
                        txtDataFormatDemo.Enabled = false;
                    }
                    custom_text.Is_using = chbxUseCustomData.Checked;
                    lblCustomOutDemo.Text = custom_text.Process();

                    /*config_format_string.Using_data_format = chbxUseCustomData.Checked;*/
                    break;
                case "chbxUpperText":
                    if (chbxUpperText.Checked)
                    {
                        chbxLowerText.Checked = false;
                    }
                    else
                    {

                    }
                    custom_text.Is_upper = chbxUpperText.Checked;
                    lblCustomOutDemo.Text = custom_text.Process();

                    /*config_format_string.Using_upper = chbxUpperText.Checked;*/
                    break;
                case "chbxLowerText":
                    if (chbxLowerText.Checked)
                    {
                        chbxUpperText.Checked = false;
                        lblCustomOutDemo.Text = text_demo.ToLower();
                    }
                    else
                    {

                    }
                    custom_text.Is_lower = chbxLowerText.Checked;
                    lblCustomOutDemo.Text = custom_text.Process();

                    /*config_format_string.Using_lower = chbxLowerText.Checked;*/
                    break;
                case "chbxTrimText":
                    custom_text.Is_trim = chbxTrimText.Checked;
                    lblCustomOutDemo.Text = custom_text.Process();

                    /*config_format_string.Using_removespace = chbxTrimText.Checked;*/
                    break;
                case "chbxCutText":
                    if (chbxCutText.Checked)
                    {
                        nbUpdownBegin.Enabled = true;
                        nbUpdownEnd.Enabled = true;
                        nbUpdownEnd.Maximum = txtDataFormatDemo.Text.Length - 1;
                    }
                    else
                    {
                        nbUpdownBegin.Enabled = false;
                        nbUpdownEnd.Enabled = false;
                    }
                    custom_text.Is_cut = chbxCutText.Checked;
                    custom_text.Pos_begin = (int)nbUpdownBegin.Value;
                    custom_text.Pos_end = (int)nbUpdownEnd.Value;
                    lblCustomOutDemo.Text = custom_text.Process();

                    /*config_format_string.Using_cut = chbxCutText.Checked;
                    config_format_string.Pos_begin = (int)nbUpdownBegin.Value;
                    config_format_string.Pos_end = (int)nbUpdownEnd.Value;*/
                    break;
                case "chbxTerminatingText":
                    if (chbxTerminatingText.Checked)
                    {
                        txtTerminatingText.Enabled = true;
                        chbxCRLF.Enabled = true;
                        chbxTab.Enabled = true;
                    }
                    else
                    {
                        txtTerminatingText.Enabled = false;
                        chbxCRLF.Enabled = false;
                        chbxTab.Enabled = false;
                    }
                    terminating_text.Is_using = chbxTerminatingText.Checked;

                    /*config_format_string.Using_terminating = chbxTerminatingText.Checked;
                    config_format_string.Terminating = txtTerminatingText.Text;*/
                    break;
                case "chbxCRLF":
                    if (chbxCRLF.Checked)
                    {
                        chbxTab.Checked = false;
                        terminating_text.Type_terminating = type_of_terminating.terminating_crlf;
                    }
                    else
                    {

                    }
                    /*config_format_string.Using_crlf = chbxCRLF.Checked;*/
                    break;
                case "chbxTab":
                    if (chbxTab.Checked)
                    {
                        chbxCRLF.Checked = false;
                        terminating_text.Type_terminating = type_of_terminating.terminating_tab;
                    }
                    else
                    {

                    }
                    /*config_format_string.Using_tab = chbxTab.Checked;*/
                    break;
                    #endregion
            }
        }

        private void textbox_TextChanged(object sender, EventArgs e)
        {
            var cur_textbox = sender as TextBox;
            switch (cur_textbox.Name)
            {
                case "txtDataFormatDemo":
                    custom_text.Input_text = txtDataFormatDemo.Text;
                    nbUpdownEnd.Maximum = txtDataFormatDemo.Text.Length - 1;
                    break;
                case "txtLeadingText":
                    leadingText.Leading_text = txtLeadingText.Text;
                    /*config_format_string.Leading = txtLeadingText.Text;*/
                    break;
                case "txtTerminatingText":
                    terminating_text.Terminal_text = txtTerminatingText.Text;
                    /*config_format_string.Terminating = txtLeadingText.Text;*/
                    break;
            }
        }

        private void nbUpdown_ValueChanged(object sender, EventArgs e)
        {
            var updown = sender as NumericUpDown;
            switch (updown.Name)
            {
                case "nbUpdownBegin":
                case "nbUpdownEnd":
                    custom_text.Pos_begin = (int)nbUpdownBegin.Value;
                    custom_text.Pos_end = (int)nbUpdownEnd.Value;
                    lblCustomOutDemo.Text = custom_text.Process();

                    /*config_format_string.Pos_begin = (int)nbUpdownBegin.Value;
                    config_format_string.Pos_end =  (int)nbUpdownEnd.Value;*/
                    break;
            }
        }


        private delegate void SafeCallDelegate3(eListBox lb, String text, Color? color = null);
        void Add_Log(eListBox lb, String text, Color? color = null)
        {
            if (listBoxBarcodeState.InvokeRequired)
            {
                var d = new SafeCallDelegate3(Add_Log);
                listBoxBarcodeState.Invoke(d, new object[] { lb, text, color });
            }
            else
            {

                text = DateTime.Now.ToString("hh:mm:ss") + ": " + text;
                //text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + ":\t" + text;
                ColoredItem coloredItem = new ColoredItem { Color = color ?? Color.Green, Text = text };
                if (lb == eListBox.LB_BARCODE)
                    listBoxBarcodeState.Items.Insert(0, coloredItem);
                else if (lb == eListBox.LB_HEARTBEAT)
                    listBoxHeartBeat.Items.Insert(0, coloredItem);
            }

        }

        private void btnClear_log(object sender, EventArgs e)
        {

        }

        private void THHSoftMiddle_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
            }
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void btnClearLogBarcode_Click(object sender, EventArgs e)
        {
            listBoxBarcodeState.Items.Clear();
        }


        private void btnClearLogHeartBeat_Click(object sender, EventArgs e)
        {
            listBoxHeartBeat.Items.Clear();
        }

        private void pictureBoxLogo_Click(object sender, EventArgs e)
        {
            Process.Start(MyDefine.workingDirectory);
        }
    }
}
