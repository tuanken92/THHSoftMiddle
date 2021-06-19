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
    public partial class SettingOutputSoftware : Form
    {

        Dictionary<int, Config_Out_Param> dic_barcode;

        public SettingOutputSoftware()
        {
            InitializeComponent();
        }

        public SettingOutputSoftware(Dictionary<int, Config_Out_Param> dic_barcode)
        {
            InitializeComponent();
            this.dic_barcode = dic_barcode;
        }

        void Update_GUI(int key)
        {

        }

        void Get_Param(int key)
        {

        }

        

        private void btnClick_Event(object sender, EventArgs e)
        {
            var cur_button = sender as Button;
            switch(cur_button.Name)
            {
                case "btnOK":
                    Close();
                    DialogResult = DialogResult.OK;
                    break;
                case "btnCancel":
                    Close();
                    break;
                case "btnApply":
                    Apply_Param();
                    break;
            }
        }

        private void Apply_Param()
        {
            throw new NotImplementedException();
        }
    }
}
