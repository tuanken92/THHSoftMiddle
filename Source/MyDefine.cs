using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace THHSoftMiddle.Source
{
    public enum type_of_test_format
    {
        test_leading = 0,
        test_data,
        test_terminating,
        test_all,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        private int _Left;
        private int _Top;
        private int _Right;
        private int _Bottom;

        public RECT(RECT Rectangle) : this(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
        {
        }
        public RECT(int Left, int Top, int Right, int Bottom)
        {
            _Left = Left;
            _Top = Top;
            _Right = Right;
            _Bottom = Bottom;
        }

        public int X
        {
            get { return _Left; }
            set { _Left = value; }
        }
        public int Y
        {
            get { return _Top; }
            set { _Top = value; }
        }
        public int Left
        {
            get { return _Left; }
            set { _Left = value; }
        }
        public int Top
        {
            get { return _Top; }
            set { _Top = value; }
        }
        public int Right
        {
            get { return _Right; }
            set { _Right = value; }
        }
        public int Bottom
        {
            get { return _Bottom; }
            set { _Bottom = value; }
        }
        public int Height
        {
            get { return _Bottom - _Top; }
            set { _Bottom = value + _Top; }
        }
        public int Width
        {
            get { return _Right - _Left; }
            set { _Right = value + _Left; }
        }
        public Point Location
        {
            get { return new Point(Left, Top); }
            set
            {
                _Left = value.X;
                _Top = value.Y;
            }
        }
        public Size Size
        {
            get { return new Size(Width, Height); }
            set
            {
                _Right = value.Width + _Left;
                _Bottom = value.Height + _Top;
            }
        }

        public static implicit operator Rectangle(RECT Rectangle)
        {
            return new Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height);
        }
        public static implicit operator RECT(Rectangle Rectangle)
        {
            return new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
        }
        public static bool operator ==(RECT Rectangle1, RECT Rectangle2)
        {
            return Rectangle1.Equals(Rectangle2);
        }
        public static bool operator !=(RECT Rectangle1, RECT Rectangle2)
        {
            return !Rectangle1.Equals(Rectangle2);
        }

        public override string ToString()
        {
            return "{Left: " + _Left + "; " + "Top: " + _Top + "; Right: " + _Right + "; Bottom: " + _Bottom + "}";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public bool Equals(RECT Rectangle)
        {
            return Rectangle.Left == _Left && Rectangle.Top == _Top && Rectangle.Right == _Right && Rectangle.Bottom == _Bottom;
        }

        public override bool Equals(object Object)
        {
            if (Object is RECT)
            {
                return Equals((RECT)Object);
            }
            else if (Object is Rectangle)
            {
                return Equals(new RECT((Rectangle)Object));
            }

            return false;
        }
    }

    public class ClickOnPointTool
    {

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

#pragma warning disable 649
        internal struct INPUT
        {
            public UInt32 Type;
            public MOUSEKEYBDHARDWAREINPUT Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;
        }

        internal struct MOUSEINPUT
        {
            public Int32 X;
            public Int32 Y;
            public UInt32 MouseData;
            public UInt32 Flags;
            public UInt32 Time;
            public IntPtr ExtraInfo;
        }

#pragma warning restore 649


        public static void ClickOnPoint(IntPtr wndHandle, Point clientPoint)
        {
            var oldPos = Cursor.Position;

            /// get screen coordinates
            ClientToScreen(wndHandle, ref clientPoint);

            /// set cursor on coords, and press mouse
            Cursor.Position = new Point(clientPoint.X, clientPoint.Y);

            var inputMouseDown = new INPUT();
            inputMouseDown.Type = 0; /// input type mouse
            inputMouseDown.Data.Mouse.Flags = 0x0002; /// left button down

            var inputMouseUp = new INPUT();
            inputMouseUp.Type = 0; /// input type mouse
            inputMouseUp.Data.Mouse.Flags = 0x0004; /// left button up

            var inputs = new INPUT[] { inputMouseDown, inputMouseUp };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));

            /// return mouse 
            Cursor.Position = oldPos;
        }

        public static Point Get_Position_Hwnd(IntPtr hwnd)
        {
            RECT rc;
            MyDefine.GetWindowRect(hwnd, out rc);
            Console.WriteLine(rc.ToString());
            return rc.Location;
        }


    }

    public struct Comport
    {
        string comport;
        int baudrate;

        public Comport(string comport, int baudrate)
        {
            this.comport = comport;
            this.baudrate = baudrate;
        }
    }

    public class LeadingText
    {
        bool is_using;
        string leading_text;
        string input_text;

        public LeadingText() { }

        public LeadingText(string input)
        {
            is_using = true;
            input_text = input;
            leading_text = null;
        }

        public LeadingText(bool is_using, string leading_text, string input_text)
        {
            this.is_using = is_using;
            this.leading_text = leading_text;
            this.input_text = input_text;
        }

        public bool Is_using { get => is_using; set => is_using = value; }
        public string Leading_text { get => leading_text; set => leading_text = value; }
        public string Input_text { get => input_text; set => input_text = value; }

        public string Process()
        {
            string data = input_text;
            if (is_using)
            {
                data = string.Format($"{leading_text}{input_text}");
            }

            Console.WriteLine("LeadingText: {0}", data);
            return data;
        }

    }

    public enum type_of_terminating
    {
        terminating_none = 0,
        terminating_tab,
        terminating_crlf
    }

    public class TerminatingText
    {
        bool is_using;
        string input_text;
        string terminal_text;
        type_of_terminating type_terminating;

        public TerminatingText() { }
        public TerminatingText(string input)
        {
            this.is_using = true;
            this.input_text = input;
            this.type_terminating = type_of_terminating.terminating_none;
        }
        public TerminatingText(bool is_using, string input_text, string terminal_text, type_of_terminating type_terminating)
        {
            this.is_using = is_using;
            this.input_text = input_text;
            this.terminal_text = terminal_text;
            this.type_terminating = type_terminating;
        }

        public bool Is_using { get => is_using; set => is_using = value; }
        public string Input_text { get => input_text; set => input_text = value; }
        public string Terminal_text { get => terminal_text; set => terminal_text = value; }
        internal type_of_terminating Type_terminating { get => type_terminating; set => type_terminating = value; }

        public string Process()
        {
            string data = input_text;
            if (is_using)
            {
                switch (type_terminating)
                {
                    case type_of_terminating.terminating_crlf:
                        data = string.Format($"{input_text}{terminal_text}\r\n");
                        break;
                    case type_of_terminating.terminating_tab:
                        data = string.Format($"{input_text}{terminal_text}\t");
                        break;
                    case type_of_terminating.terminating_none:
                    default:
                        data = string.Format($"{input_text}{terminal_text}");
                        break;
                }

            }

            Console.WriteLine("TerminatingText: {0}", data);
            return data;
        }
    }

    public enum type_of_customer
    {
        custom_none = 0,
        custom_upper,
        custom_lower,
        custom_trim,
        custom_cut
    }

    public class CustomerData
    {
        bool is_using;
        bool is_upper;
        bool is_lower;
        bool is_cut;
        bool is_trim;
        string input_text;
        int pos_begin, pos_end;

        public CustomerData(string input)
        {
            is_using = true;
            is_upper = false;
            is_lower = false;
            is_cut = false;
            is_trim = false;
            input_text = input;
            pos_begin = 0;
            pos_end = 0;
        }
        public CustomerData()
        {
            is_using = false;
            is_upper = false;
            is_lower = false;
            is_cut = false;
            is_trim = false;
            input_text = null;
            pos_begin = 0;
            pos_end = 0;
        }

        public bool Is_using { get => is_using; set => is_using = value; }
        public bool Is_upper { get => is_upper; set => is_upper = value; }
        public bool Is_lower { get => is_lower; set => is_lower = value; }
        public bool Is_cut { get => is_cut; set => is_cut = value; }
        public bool Is_trim { get => is_trim; set => is_trim = value; }
        public string Input_text { get => input_text; set => input_text = value; }
        public int Pos_begin { get => pos_begin; set => pos_begin = value; }
        public int Pos_end { get => pos_end; set => pos_end = value; }

        public string Process()
        {
            string data = Input_text;
            if (!is_using || string.IsNullOrEmpty(data))
                return data;

            if (is_trim)
                data = data.Replace(" ", "");
            if (is_upper)
                data = data.ToUpper();
            if (is_lower)
                data = data.ToLower();
            if (is_cut)
                data = data.Substring(pos_begin, Math.Min(pos_end - pos_begin, Input_text.Length - 1));

            Console.WriteLine("CustomerData: {0}", data);
            return data;
        }

    }

    public class Data_Barcode
    {
        public String data = null;
        public bool found = false;
        public Data_Barcode(String data)
        {
            this.data = data;
            this.found = false;
        }

    }
    public class PushData
    {
        List<String> list_data;

        public PushData() {
            list_data = new List<string>();
        }


        public List<string> List_data { get => list_data; set => list_data = value; }

        public bool Push()
        {
            foreach(var data in list_data)
            {
                //puttext
                SendKeys.SendWait(data);
                SendKeys.SendWait("{ENTER}");
            }

            return true;
        }
    }

    public class DataFormat
    {
        public string input_data;
        public LeadingText leading_text;
        public CustomerData custom_text;
        public TerminatingText terminating_text;

        public DataFormat() { }
        public DataFormat(string input)
        {
            this.input_data = input;
            leading_text = new LeadingText(input);
            custom_text = new CustomerData(input);
            terminating_text = new TerminatingText(input);
        }

        public string Process()
        {
            var customed_text = custom_text.Process();
            customed_text = leading_text.Process();
            customed_text = terminating_text.Process();
            Console.WriteLine("text output: {0}", customed_text);
            return customed_text;
        }


    }

    public enum output_soft
    { 
        method_none = 0,
        method_click,
        method_com
    }

    public enum input_soft
    {
        method_none = 0,
        method_com,
        method_tcp
    }

    public enum check_forward
    {
        direct = 0,
        compare_text
    }

    public class Ouput_Text
    {
        bool is_compare;
        output_soft write_method;
        string input_text;
        string compare_text;
        List<Point> list_click;
        IntPtr programHandle;

        public Ouput_Text() 
        {
            is_compare = false;
            write_method = output_soft.method_none;
            input_text = null;
            compare_text = null;
            list_click = new List<Point>();
            programHandle = IntPtr.Zero;
        }

        public bool Is_compare { get => is_compare; set => is_compare = value; }
        public output_soft Write_method { get => write_method; set => write_method = value; }
        public string Input_text { get => input_text; set => input_text = value; }
        public string Compare_text { get => compare_text; set => compare_text = value; }
        public List<Point> List_click { get => list_click; set => list_click = value; }
        public IntPtr ProgramHandle { get => programHandle; set => programHandle = value; }

        public void Run_Sequence()
        {
            if (string.IsNullOrEmpty(input_text))
                return;
            if(is_compare)
            {
                if(input_text.Contains(compare_text))
                {
                    //do somthing
                    Run_Click();
                }
                else
                {
                    return;
                }
            }
            else
            {
                //do something
                Run_Click();
            }
        }

        public void Run_Click()
        {
            Thread th = new Thread(() =>
            {
                foreach (var click_item in list_click)
                {
                    ClickOnPointTool.ClickOnPoint(ProgramHandle, click_item);
                    Thread.Sleep(100);
                }

                SendKeys.SendWait(input_text);
            });
            th.Start();
        }
    }

    public enum eListBox
    {
        LB_BARCODE = 0,
        LB_HEARTBEAT
    }
    public class ColoredItem
    {
        public string Text;
        public Color Color;
    };

    public class SaveLoad_Parameter
    {
        public static void Save_Parameter(object param)
        {
            // serialize JSON directly to a file
            Console.WriteLine(MyDefine.file_config);
            using (StreamWriter file = File.CreateText(MyDefine.file_config))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, param);
            }
        }

        public static object Load_Parameter(object param)
        {
            using (StreamReader file = File.OpenText(MyDefine.file_config))
            {
                JsonSerializer serializer = new JsonSerializer();
                param = serializer.Deserialize(file, param.GetType());
            }
            Console.WriteLine(MyDefine.file_config.ToString());
            return param;
        }

        public static void Save_Parameter(object param, string file_name)
        {
            // serialize JSON directly to a file
            Console.WriteLine(MyDefine.file_config);
            using (StreamWriter file = File.CreateText(file_name))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, param);
            }
        }

        public static object Load_Parameter(object param, string file_name)
        {
            using (StreamReader file = File.OpenText(file_name))
            {
                JsonSerializer serializer = new JsonSerializer();
                param = serializer.Deserialize(file, param.GetType());
            }
            Console.WriteLine(MyDefine.file_config.ToString());
            return param;
        }
    }

    public class Param_COM
    {
        string comport;
        int baudrate;

        public Param_COM(string comport, int baudrate=9600)
        {
            Comport = comport;
            Baudrate = baudrate;
        }
        public Param_COM()
        {
            Comport = null;
            Baudrate = 9600;
        }

        public string Comport { get => comport; set => comport = value; }
        public int Baudrate { get => baudrate; set => baudrate = value; }
    }

    public class Param_TCP
    {
        string ip;
        int port;
        public Param_TCP(string ip, int port=8888)
        {
            Ip = ip;
            Port = port;
        }
        public Param_TCP()
        {
            Ip = "127.0.0.1";
            Port = 8888;
        }

        public string Ip { get => ip; set => ip = value; }
        public int Port { get => port; set => port = value; }
    }

    public class Thread_Manager 
    {
        public string name_thread;
        public bool enable_thread;
        public Thread thread;

        public Thread_Manager()
        {
            
        }
    }
    public class Config_Out_Param
    {
        bool is_send = false;
        bool is_transmit_data = false;
        Param_COM out_com;
        List<Point> list_out_click;
        string key_check_fw;

        public Config_Out_Param()
        {
            Out_com = new Param_COM();
            List_out_click = new List<Point>();
            Key_check_fw = null;
            Is_send = false;
            Is_transmit_data = false;

        }

        public void Print_Infor()
        {
            Console.WriteLine("---------------------------");
            Console.WriteLine($"comport = {out_com.Comport}, baud = {out_com.Baudrate}");
            Console.WriteLine($"key_check_fw = {key_check_fw}");
            Console.WriteLine($"list_out_click:");
            Console.WriteLine($"is_send = {Is_send}");
            Console.WriteLine($"is_transmit_data = {Is_transmit_data}");
            foreach (var p in list_out_click)
            {
                Console.WriteLine(p);
            }
            Console.WriteLine("---------------------------");
        }
        public Param_COM Out_com { get => out_com; set => out_com = value; }
        public List<Point> List_out_click { get => list_out_click; set => list_out_click = value; }
        public string Key_check_fw { get => key_check_fw; set => key_check_fw = value; }
        public bool Is_send { get => is_send; set => is_send = value; }
        public bool Is_transmit_data { get => is_transmit_data; set => is_transmit_data = value; }
    }
    public class Config_Common_Param
    {
        string char_split;
        string target_name;
        int target_hwnd;
        int offset_x;
        int offset_y;
        int time_delay;
        int number_barcode;
        input_soft in_soft;
        output_soft out_soft;
        check_forward check_to_forward;

        Param_COM in_com;
        Param_TCP in_tcp;

        Dictionary<int, Config_Out_Param> dic_barcode;

        public Config_Common_Param()
        {
            Char_split = null;
            Target_name = null;
            Target_hwnd = 0;
            Offset_x = 0;
            Offset_y = 30;
            Time_delay = 100;
            Number_barcode = 1;
            In_soft = input_soft.method_none;
            Out_soft = output_soft.method_none;
            Check_to_forward = check_forward.direct;
            In_com = new Param_COM("COM888");
            In_tcp = new Param_TCP("192.168.100.111");

            dic_barcode = new Dictionary<int, Config_Out_Param>();
            
        }

        public string Target_name { get => target_name; set => target_name = value; }
        public int Target_hwnd { get => target_hwnd; set => target_hwnd = value; }
        public int Offset_x { get => offset_x; set => offset_x = value; }
        public int Offset_y { get => offset_y; set => offset_y = value; }
        public int Time_delay { get => time_delay; set => time_delay = value; }
        public int Number_barcode { get => number_barcode; set => number_barcode = value; }
        public input_soft In_soft { get => in_soft; set => in_soft = value; }
        public output_soft Out_soft { get => out_soft; set => out_soft = value; }
        public check_forward Check_to_forward { get => check_to_forward; set => check_to_forward = value; }
        public Param_COM In_com { get => in_com; set => in_com = value; }
        public Param_TCP In_tcp { get => in_tcp; set => in_tcp = value; }
        public Dictionary<int, Config_Out_Param> Dic_barcode { get => dic_barcode; set => dic_barcode = value; }
        public string Char_split { get => char_split; set => char_split = value; }
    }
    public class Config_Format_String
    {
        //leading
        bool using_leading;
        string leading;

        //data
        bool using_data_format;
        bool using_upper;
        bool using_lower;
        bool using_removespace;
        bool using_cut;
        int pos_begin;
        int pos_end;

        //terminating
        bool using_terminating;
        string terminating;
        bool using_crlf;
        bool using_tab;

        public bool Using_leading { get => using_leading; set => using_leading = value; }
        public string Leading { get => leading; set => leading = value; }
        public bool Using_data_format { get => using_data_format; set => using_data_format = value; }
        public bool Using_upper { get => using_upper; set => using_upper = value; }
        public bool Using_lower { get => using_lower; set => using_lower = value; }
        public bool Using_removespace { get => using_removespace; set => using_removespace = value; }
        public bool Using_cut { get => using_cut; set => using_cut = value; }
        public int Pos_begin { get => pos_begin; set => pos_begin = value; }
        public int Pos_end { get => pos_end; set => pos_end = value; }
        public bool Using_terminating { get => using_terminating; set => using_terminating = value; }
        public bool Using_crlf { get => using_crlf; set => using_crlf = value; }
        public bool Using_tab { get => using_tab; set => using_tab = value; }
        public string Terminating { get => terminating; set => terminating = value; }

        public Config_Format_String()
        {
            //leading
            using_leading = false;
            leading = null;

            //data
            using_data_format = false;
            using_upper = false;
            using_lower = false;
            using_removespace = false;
            using_cut = false;
            pos_begin = 0;
            pos_end = 0;

            //terminating
            using_terminating = false;
            terminating = null;
            using_crlf = false;
            using_tab = false;
        }
    }
    public class Job_Step
    {

        //input string from barcode reader


        //compare string to next step


        //format output


        //put to other system by some method as comport virtua or set click control
    }

    public class Windown_Infor
    {
        string name;
        int hwnd;

        public Windown_Infor(string name, int hwnd)
        {
            Name = name;
            Hwnd = hwnd;
        }

        public string Name { get => name; set => name = value; }
        public int Hwnd { get => hwnd; set => hwnd = value; }
    }

    public class MyDefine
    {

        public static uint WM_LBUTTONDOWN = 0x201;
        public static uint WM_LBUTTONUP = 0x202;

        public static readonly string workingDirectory = Environment.CurrentDirectory;
        public static readonly string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
        public static readonly string workspaceDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

        public static readonly string file_config = String.Format($"{workingDirectory}\\Data\\configs\\config_param.json");
        public static readonly string file_config_format_data = String.Format($"{workingDirectory}\\Data\\configs\\format_data.json");
        public static readonly string file_config_common_param = String.Format($"{workingDirectory}\\Data\\configs\\common_param.json");
        public static readonly string file_config_filter_window = String.Format($"{workingDirectory}\\Data\\configs\\filter_window.json");
        public static readonly string path_load_img_database = @"C:\Program Files\Cognex\VisionPro\Images";
        public static readonly string path_load_vpp_file = @"C:\Users\Admin\Desktop\Vpp_file";
        public static readonly string path_save_images = String.Format("{0}\\Images", projectDirectory);

        public static readonly List<int> list_baudrate = new List<int> { 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000 };

        // Get a handle to an application window.
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(int hWnd, int msg, int wParam, IntPtr lParam);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetWindowText",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

        // Define the callback delegate's type.
        private delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        private static Dictionary<string, IntPtr> dic_programs = null;
        private static List<Windown_Infor> list_window_active = new List<Windown_Infor>();



        public static IntPtr GetControl(String name_wd)
        {
            IntPtr hWnd = IntPtr.Zero;
            var list_process = Process.GetProcesses();
            foreach (Process pList in list_process)
            {
                if (pList.MainWindowTitle.Contains(name_wd))
                {
                    hWnd = pList.MainWindowHandle;
                    break;
                }
            }
            return hWnd;
        }

        // Return a list of the desktop windows' handles and titles.
        public static void GetDesktopWindowHandlesAndTitles(out Dictionary<string, IntPtr> dic)
        {
            dic_programs = new Dictionary<string, IntPtr>();
            list_window_active = new List<Windown_Infor>();
            if (!EnumDesktopWindows(IntPtr.Zero, FilterCallback, IntPtr.Zero))
            {
                dic = null;
            }
            else
            {
                dic = dic_programs;
            }
        }

         
        public static void GetDesktopWindowHandlesAndTitles(out List<Windown_Infor> list_window)
        {
            list_window= new List<Windown_Infor>();
            list_window_active = new List<Windown_Infor>();
            dic_programs = new Dictionary<string, IntPtr>();
            if (!EnumDesktopWindows(IntPtr.Zero, FilterCallback, IntPtr.Zero))
            {
                list_window = null;
            }
            else
            {
                list_window = list_window_active;
            }


            
        }


        // We use this function to filter windows.
        // This version selects visible windows that have titles.
        private static bool FilterCallback(IntPtr hWnd, int lParam)
        {
            List<string> list_filter = new List<string> { "THHSoftMiddle", "Window Search", "Internet Security Warning",
            "e-Manual Viewer Database Service", "Microsoft Store", "e-Manual Viewer Service Server", "Microsoft Text Input Application",
            "Movies & TV", "Program Manager"};

            // Get the window's title.
            StringBuilder sb_title = new StringBuilder(1024);
            int length = GetWindowText(hWnd, sb_title, sb_title.Capacity);
            string title = sb_title.ToString();
            foreach(var str_filter in list_filter)
            {
                if (title.Contains(str_filter))
                    return true;
            }
            // If the window is visible and has a title, save it.
            if (IsWindowVisible(hWnd) && string.IsNullOrEmpty(title) == false)
            {
                if (!dic_programs.ContainsKey(title))
                {
                    dic_programs.Add(title, hWnd);
                    //Console.WriteLine($"add {title} - windown {hWnd}");
                }
                else
                {
                    //Console.WriteLine($"keep {title} - windown {hWnd}");
                }

                list_window_active.Add(new Windown_Infor(title, (int)hWnd));

            }

            // Return true to indicate that we
            // should continue enumerating windows.
            return true;
        }

        public static void Get_All_Process()
        {
            Process[] processlist = Process.GetProcesses();

            foreach (Process process in processlist)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    Console.WriteLine("Process: {0}, ID: {1}, Window title: {2}", process.ProcessName, process.Id, process.MainWindowTitle);
                }
            }
        }


        public static string GetLocalIPAddress()
        {
            String str_ip = null;
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    str_ip =  ip.ToString();
                    break;
                }
            }
            return str_ip;
        }

        public static List<string> Scan_Comport()
        {
            string[] ArrayComPortsNames = null;
            int index = -1;

            ArrayComPortsNames = SerialPort.GetPortNames();
            do
            {
                index += 1;
            }
            while (!(index == ArrayComPortsNames.GetUpperBound(0)));
            Array.Sort(ArrayComPortsNames);


            /*foreach (var com_name in ArrayComPortsNames)
                Console.WriteLine(com_name);*/

            return (new List<string>(ArrayComPortsNames));
        }

        #region Capture and Save Window's Screen
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        public static Bitmap PrintWindow(IntPtr hwnd)
        {
            RECT rc;
            GetWindowRect(hwnd, out rc);
            rc.ToString();

            Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            IntPtr hdcBitmap = gfxBmp.GetHdc();

            PrintWindow(hwnd, hdcBitmap, 0);

            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();

            return bmp;
        }

        public static bool File_Is_Exist(string file_name)
        {
            return File.Exists(file_name);
        }

        public static bool CreateFolder(string path_folder)
        {
            bool result = Directory.Exists(path_folder);
            if (!result)
            {
                Directory.CreateDirectory(path_folder);
                result = Directory.Exists(path_folder);
            }
            return result;
        }
        public static string GenerateNameImage()
        {
            CreateFolder(path_save_images);
            return String.Format("{0}\\{1}.jpg", path_save_images, DateTime.Now.ToString("yyyyMMdd_hhmmss"));
        }

        public static string GenerateNameImage(int index)
        {
            CreateFolder(path_save_images);
            return String.Format("{0}\\{1}_{2}.jpg", path_save_images, DateTime.Now.ToString("yyyyMMdd_hhmmss"), index);
        }


        public static void Save_BitMap(Bitmap bm)
        {
            string file_name = GenerateNameImage();
            bm.Save(file_name, ImageFormat.Jpeg);
            Console.WriteLine("Saved file {0}", file_name);
        }
        #endregion
    }
}
