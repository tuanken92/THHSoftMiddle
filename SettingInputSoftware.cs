using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using THHSoftMiddle.Source;

namespace THHSoftMiddle
{
    public partial class SettingInputSoftware : Form
    {

        public Param_COM in_com;
        public Param_TCP in_tcp;

        public SettingInputSoftware()
        {
            InitializeComponent();
            Init_GUI();
        }

        public SettingInputSoftware(Param_COM in_com, Param_TCP in_tcp)
        {
            InitializeComponent();
            this.in_com = in_com;
            this.in_tcp = in_tcp;
            Init_GUI();
        }

        void Init_GUI()
        {
            //tcp
            txtIP.Text = in_tcp.Ip;
            txtPort.Text = in_tcp.Port.ToString();

            //comport
            List<string> list_comport = MyDefine.Scan_Comport();
            cbxComport.DataSource = list_comport;
            cbxComport.Text = in_com.Comport;

            var bindingBaudrate = new BindingSource();
            bindingBaudrate.DataSource = MyDefine.list_baudrate;
            cbxBaudrate.DataSource = bindingBaudrate.DataSource;
            cbxBaudrate.Text = in_com.Baudrate.ToString();
        }

        void Get_Param()
        {
            //tcp
            in_tcp.Ip = txtIP.Text;
            in_tcp.Port = int.Parse(txtPort.Text);

            //comport
            in_com.Comport = cbxComport.Text;
            in_com.Baudrate = int.Parse(cbxBaudrate.Text);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Get_Param();
            this.Close();
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGetIP_Click(object sender, EventArgs e)
        {
            txtIP.Text = MyDefine.GetLocalIPAddress();
        }
    }
}
