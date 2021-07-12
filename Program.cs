using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace THHSoftMiddle
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            String thisprocessname = Process.GetCurrentProcess().ProcessName;

            if (Process.GetProcesses().Count(p => p.ProcessName == thisprocessname) > 1)
            {
                MessageBox.Show("This program is running", "Warning", MessageBoxButtons.OK);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new THHSoftMiddle());
            //Application.Run(new ClickWindow());
        }
    }
}
