using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
        public THHSoftMiddle()
        {
            InitializeComponent();
            Initial();
            Init_GUI();
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
        }
        void Initial()
        {
            //comport
            string com_name = txtComName.Text;
            int com_baud = int.Parse(txtComBaud.Text);
            rs232 = new RS232(com_name, com_baud);

            //comport
            string ip = txtTcpIP.Text;
            int port = int.Parse(txtTcpPort.Text);
            tcp_client = new TcpIPClient(ip, port);


            //data format
            text_demo = txtDataFormatDemo.Text;
            leadingText = new LeadingText();
            custom_text = new CustomerData();
            terminating_text = new TerminatingText();
            custom_text.Input_text = txtDataFormatDemo.Text;

            //config format string
            config_format_string = new Config_Format_String();
            if(MyDefine.File_Is_Exist(MyDefine.file_config))
            {
                config_format_string = SaveLoad_Parameter.Load_Parameter(config_format_string) as Config_Format_String;
            }
            else
            {
                MessageBox.Show($"Not found {MyDefine.file_config}");
            }
            

        }

        private void timerDateTime_Tick(object sender, EventArgs e)
        {
            this.lbDateTime.Text = DateTime.Now.ToString("ddd MM/dd/yyyy\nhh:mm::ss tt");
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
                    programHandle = MyDefine.FindWindow(class_name, window_name);

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
                    SaveLoad_Parameter.Save_Parameter(config_format_string);
                    break;

                case "btnLoadDataFormat":
                    config_format_string  = SaveLoad_Parameter.Load_Parameter(config_format_string) as Config_Format_String;
                    break;

                case "btnTestFormatOutput":
                    switch (cbxModeTest.SelectedIndex)
                    {
                        case 0:
                            leadingText.Input_text = txtInputStringToTest.Text;
                            leadingText.Leading_text = txtLeadingText.Text;
                            lbOutputData.Text = leadingText.Process();
                            break;
                        case 1:
                            custom_text.Input_text = txtInputStringToTest.Text;
                            lbOutputData.Text = custom_text.Process();
                            break;
                        case 2:
                            terminating_text.Input_text = txtInputStringToTest.Text;
                            terminating_text.Terminal_text = txtTerminatingText.Text;
                            lbOutputData.Text = terminating_text.Process();
                            break;
                        case 3:
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

            }
        }





        private void THHSoftMiddle_Load(object sender, EventArgs e)
        {
            timerDateTime.Start();
        }

        private void THHSoftMiddle_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerDateTime.Stop();
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

                    config_format_string.Using_leading = chbxLeadingText.Checked;
                    config_format_string.Leading = txtLeadingText.Text;
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

                    config_format_string.Using_data_format = chbxUseCustomData.Checked;
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

                    config_format_string.Using_upper = chbxUpperText.Checked;
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

                    config_format_string.Using_lower = chbxLowerText.Checked;
                    break;
                case "chbxTrimText":
                    custom_text.Is_trim = chbxTrimText.Checked;
                    lblCustomOutDemo.Text = custom_text.Process();

                    config_format_string.Using_removespace = chbxTrimText.Checked;
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

                    config_format_string.Using_cut = chbxCutText.Checked;
                    config_format_string.Pos_begin = (int)nbUpdownBegin.Value;
                    config_format_string.Pos_end = (int)nbUpdownEnd.Value;

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

                    config_format_string.Using_terminating = chbxTerminatingText.Checked;
                    config_format_string.Terminating = txtTerminatingText.Text;
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
                    config_format_string.Using_crlf = chbxCRLF.Checked;
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
                    config_format_string.Using_tab = chbxTab.Checked;
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
                    config_format_string.Leading = txtLeadingText.Text;
                    break;
                case "txtTerminatingText":
                    terminating_text.Terminal_text = txtTerminatingText.Text;
                    config_format_string.Terminating = txtLeadingText.Text;
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

                    config_format_string.Pos_begin = (int)nbUpdownBegin.Value;
                    config_format_string.Pos_end =  (int)nbUpdownEnd.Value;
                    break;
            }
        }
    }
}
