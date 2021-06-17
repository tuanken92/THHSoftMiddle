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

        public THHSoftMiddle()
        {
            InitializeComponent();
            Initial();
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

        }
       
        private void timerDateTime_Tick(object sender, EventArgs e)
        {
            this.lbDateTime.Text = DateTime.Now.ToString("ddd MM/dd/yyyy\nhh:mm::ss tt");
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
                    if(!com_state)
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
                    bool x_parse = int.TryParse(txtMouseX.Text,out xpos);
                    bool y_parse = int.TryParse(txtMouseY.Text,out ypos);
                    if(x_parse && y_parse)
                    {
                        ClickOnPointTool.ClickOnPoint(programHandle, new Point(xpos, ypos));
                    }
                    break;
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

    }
}
