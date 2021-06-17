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
    public partial class ClickWindow : Form
    {
        List<Point> list_click;
        Dictionary<string, IntPtr> dic_programssss;
        IntPtr thhHwnd = IntPtr.Zero;
        IntPtr programHandle = IntPtr.Zero;
        string programName;
        Point programLocation, thhLocation;
        int xpos;
        int ypos;
        int offsetX, offsetY;

        public ClickWindow()
        {
            InitializeComponent();
            Init_Variables();
            Init_UI();
        }

        void Init_Variables()
        {
            //get offset
            bool offset_parse = int.TryParse(txtOffsetY.Text, out offsetY);
            Console.WriteLine($"Offset y = {offsetY}");

            thhHwnd = Process.GetCurrentProcess().Handle;
            thhLocation = ClickOnPointTool.Get_Position_Hwnd(thhHwnd);
            Console.WriteLine($"thh Location = {thhLocation.ToString()}");
            lbStatus.Text = thhLocation.ToString();

            //list_click
            list_click = new List<Point>();
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

        private void btn_click(object sender, EventArgs e)
        {
            var cur_button = sender as Button;
            Console.WriteLine($"clicked button {cur_button.Text}");
            switch (cur_button.Name)
            {
                case "btnTestClick":
                    Test_Click();
                    break;
                case "btnOk":
                    this.Close();
                    break;
                case "btnCancel":
                    this.Close();
                    break;
            }
        }

        void Test_Click()
        {
            
            bool x_parse = int.TryParse(txtMouseX.Text, out xpos);
            bool y_parse = int.TryParse(txtMouseY.Text, out ypos);
            Console.WriteLine($"Click x = {xpos}, y = {ypos} on the {programName}");
            if (x_parse && y_parse)
            {
                //MyDefine.SetForegroundWindow(programHandle);
                ClickOnPointTool.ClickOnPoint(programHandle, new Point(xpos, ypos));
                
            }
            else
            {
                Console.WriteLine("Parse data error");
            }

            //MyDefine.SetForegroundWindow(programHandle);
            //MyDefine.SetForegroundWindow(thhHwnd);
            //this.Focus();
            //ClickOnPointTool.ClickOnPoint(thhHwnd, thhLocation);
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
                txtMouseX.Text = clientPoint.X.ToString();
                txtMouseY.Text = clientPoint.Y.ToString();
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
            foreach(var click_item in list_click)
            {
                ClickOnPointTool.ClickOnPoint(programHandle, click_item);
                Thread.Sleep(1000);
            }
        }

       
    }
}
