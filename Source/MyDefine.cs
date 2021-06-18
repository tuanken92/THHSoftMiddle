﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace THHSoftMiddle.Source
{
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

    public enum method_write
    { 
        method_none = 0,
        method_click,
        method_com
    }

    public class Ouput_Text
    {
        bool is_compare;
        method_write write_method;
        string input_text;
        string compare_text;
        List<Point> list_click;
        IntPtr programHandle;

        public Ouput_Text() 
        {
            is_compare = false;
            write_method = method_write.method_none;
            input_text = null;
            compare_text = null;
            list_click = new List<Point>();
            programHandle = IntPtr.Zero;
        }

        public bool Is_compare { get => is_compare; set => is_compare = value; }
        public method_write Write_method { get => write_method; set => write_method = value; }
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
    public class MyDefine
    {

        public static uint WM_LBUTTONDOWN = 0x201;
        public static uint WM_LBUTTONUP = 0x202;

        public static readonly string workingDirectory = Environment.CurrentDirectory;
        public static readonly string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
        public static readonly string workspaceDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

        public static readonly string file_config = String.Format($"{workspaceDirectory}\\Data\\configs\\config_param.json");
        public static readonly string path_load_img_database = @"C:\Program Files\Cognex\VisionPro\Images";
        public static readonly string path_load_vpp_file = @"C:\Users\Admin\Desktop\Vpp_file";
        public static readonly string path_save_images = String.Format("{0}\\Images", projectDirectory);

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

        // Return a list of the desktop windows' handles and titles.
        public static void GetDesktopWindowHandlesAndTitles(out Dictionary<string, IntPtr> dic)
        {
            dic_programs = new Dictionary<string, IntPtr>();
            if (!EnumDesktopWindows(IntPtr.Zero, FilterCallback, IntPtr.Zero))
            {
                dic = null;
            }
            else
            {
                dic = dic_programs;
            }
        }

        // We use this function to filter windows.
        // This version selects visible windows that have titles.
        private static bool FilterCallback(IntPtr hWnd, int lParam)
        {
            // Get the window's title.
            StringBuilder sb_title = new StringBuilder(1024);
            int length = GetWindowText(hWnd, sb_title, sb_title.Capacity);
            string title = sb_title.ToString();

            // If the window is visible and has a title, save it.
            if (IsWindowVisible(hWnd) &&
                string.IsNullOrEmpty(title) == false)
            {
                if (!dic_programs.ContainsKey(title))
                    dic_programs.Add(title, hWnd);
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
