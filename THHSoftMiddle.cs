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
        public THHSoftMiddle()
        {
            InitializeComponent();
            Initial();
            test_calculor();
        }

        

        void Initial()
        {
            
        }
       
        private void timerDateTime_Tick(object sender, EventArgs e)
        {
            this.lbDateTime.Text = DateTime.Now.ToString("ddd MM/dd/yyyy\nhh:mm::ss tt");
        }


        private static IntPtr CreateLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }

        IntPtr programHandle = IntPtr.Zero;

        private void btn_Click_Event(object sender, EventArgs e)
        {
            var curButton = sender as Button;
            Console.WriteLine($"btn_Click_Event name = {curButton.Name}");
            switch (curButton.Name)
            {
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
                        //IntPtr button_ = MyDefine.FindWindowEx(programHandle, IntPtr.Zero, null, "Serial");
                        ////MyDefine.SendMessage((int)programHandle, WM_LBUTTONUP, 0x00000000, CreateLParam(xpos, ypos));
                        //MyDefine.SendMessage((int)button_, 0x00F5, 0, IntPtr.Zero);


                        ClickOnPointTool.ClickOnPoint(programHandle, new Point(xpos, ypos));

                    }
                    break;
            }
        }


        //This is a replacement for Cursor.Position in WinForms
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        //This simulates a left mouse click
        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        private const int WM_CLOSE = 16;
        private const int BN_CLICKED = 245;
        private void test_calculor()
        {
            int hwnd = 0;
            IntPtr hwndChild = IntPtr.Zero;

            //Get a handle for the Calculator Application main window
            hwnd = (int)MyDefine.FindWindow("ApplicationFrameWindow", "Calculator");
            if (hwnd == 0)
            {
                if (MessageBox.Show("Couldn't find the calculator application. Do you want to start it?", "TestWinAPI", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("Calc");
                }
            }
            else
            {

                //Get a handle for the "1" button
                hwndChild = MyDefine.FindWindowEx((IntPtr)hwnd, IntPtr.Zero, "Button", "1");

                //send BN_CLICKED message
                MyDefine.SendMessage((int)hwndChild, BN_CLICKED, 0, IntPtr.Zero);

                //Get a handle for the "+" button
                hwndChild = MyDefine.FindWindowEx((IntPtr)hwnd, IntPtr.Zero, "Button", "+");

                //send BN_CLICKED message
                MyDefine.SendMessage((int)hwndChild, BN_CLICKED, 0, IntPtr.Zero);

                //Get a handle for the "2" button
                hwndChild = MyDefine.FindWindowEx((IntPtr)hwnd, IntPtr.Zero, "Button", "2");

                //send BN_CLICKED message
                MyDefine.SendMessage((int)hwndChild, BN_CLICKED, 0, IntPtr.Zero);

                //Get a handle for the "=" button
                hwndChild = MyDefine.FindWindowEx((IntPtr)hwnd, IntPtr.Zero, "Button", "=");

                //send BN_CLICKED message
                MyDefine.SendMessage((int)hwndChild, BN_CLICKED, 0, IntPtr.Zero);

            }

        }

        private void Checkbox_Click(object sender, EventArgs e)
        {
            var curCheckbox = sender as CheckBox;
            Console.WriteLine($"Checkbox_Click_Event name = {curCheckbox.Name}");

            switch(curCheckbox.Name)
            {
                case "chbxEnter":
                    break;
                case "chbxTab":
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
