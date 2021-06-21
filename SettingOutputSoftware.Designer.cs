
namespace THHSoftMiddle
{
    partial class SettingOutputSoftware
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxForward = new System.Windows.Forms.GroupBox();
            this.txtCheckDataForward = new System.Windows.Forms.TextBox();
            this.chbxDirect = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.nbUpDownBarcode = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxCom = new System.Windows.Forms.GroupBox();
            this.txtBaudrate = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtComport = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBoxClick = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtHWND = new System.Windows.Forms.TextBox();
            this.cbbListWindow = new System.Windows.Forms.ComboBox();
            this.lbStatus = new System.Windows.Forms.Label();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnTestClick = new System.Windows.Forms.Button();
            this.listBoxClick = new System.Windows.Forms.ListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnApply = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnDelAll = new System.Windows.Forms.Button();
            this.groupBoxForward.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nbUpDownBarcode)).BeginInit();
            this.groupBoxCom.SuspendLayout();
            this.groupBoxClick.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxForward
            // 
            this.groupBoxForward.Controls.Add(this.txtCheckDataForward);
            this.groupBoxForward.Controls.Add(this.chbxDirect);
            this.groupBoxForward.Location = new System.Drawing.Point(12, 86);
            this.groupBoxForward.Name = "groupBoxForward";
            this.groupBoxForward.Size = new System.Drawing.Size(248, 60);
            this.groupBoxForward.TabIndex = 0;
            this.groupBoxForward.TabStop = false;
            this.groupBoxForward.Text = "Check Forward";
            // 
            // txtCheckDataForward
            // 
            this.txtCheckDataForward.Location = new System.Drawing.Point(116, 26);
            this.txtCheckDataForward.Name = "txtCheckDataForward";
            this.txtCheckDataForward.Size = new System.Drawing.Size(115, 20);
            this.txtCheckDataForward.TabIndex = 1;
            // 
            // chbxDirect
            // 
            this.chbxDirect.AutoSize = true;
            this.chbxDirect.Location = new System.Drawing.Point(16, 28);
            this.chbxDirect.Name = "chbxDirect";
            this.chbxDirect.Size = new System.Drawing.Size(77, 17);
            this.chbxDirect.TabIndex = 0;
            this.chbxDirect.Text = "chbxDirect";
            this.chbxDirect.UseVisualStyleBackColor = true;
            this.chbxDirect.CheckedChanged += new System.EventHandler(this.chbxDirect_CheckedChanged);
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(215)))), ((int)(((byte)(255)))));
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Navy;
            this.label5.Location = new System.Drawing.Point(-1, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(522, 23);
            this.label5.TabIndex = 6;
            this.label5.Text = "Setting Output Software";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // nbUpDownBarcode
            // 
            this.nbUpDownBarcode.Location = new System.Drawing.Point(112, 23);
            this.nbUpDownBarcode.Name = "nbUpDownBarcode";
            this.nbUpDownBarcode.Size = new System.Drawing.Size(115, 20);
            this.nbUpDownBarcode.TabIndex = 1;
            this.nbUpDownBarcode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nbUpDownBarcode.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nbUpDownBarcode.ValueChanged += new System.EventHandler(this.nbUpDownBarcode_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "BarCode";
            // 
            // groupBoxCom
            // 
            this.groupBoxCom.Controls.Add(this.txtBaudrate);
            this.groupBoxCom.Controls.Add(this.label4);
            this.groupBoxCom.Controls.Add(this.txtComport);
            this.groupBoxCom.Controls.Add(this.label3);
            this.groupBoxCom.Location = new System.Drawing.Point(12, 153);
            this.groupBoxCom.Name = "groupBoxCom";
            this.groupBoxCom.Size = new System.Drawing.Size(248, 89);
            this.groupBoxCom.TabIndex = 8;
            this.groupBoxCom.TabStop = false;
            this.groupBoxCom.Text = "COM";
            // 
            // txtBaudrate
            // 
            this.txtBaudrate.Location = new System.Drawing.Point(113, 53);
            this.txtBaudrate.Name = "txtBaudrate";
            this.txtBaudrate.Size = new System.Drawing.Size(121, 20);
            this.txtBaudrate.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Comport";
            // 
            // txtComport
            // 
            this.txtComport.Location = new System.Drawing.Point(113, 23);
            this.txtComport.Name = "txtComport";
            this.txtComport.Size = new System.Drawing.Size(121, 20);
            this.txtComport.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Baudrate";
            // 
            // groupBoxClick
            // 
            this.groupBoxClick.Controls.Add(this.label2);
            this.groupBoxClick.Controls.Add(this.txtHWND);
            this.groupBoxClick.Controls.Add(this.cbbListWindow);
            this.groupBoxClick.Controls.Add(this.lbStatus);
            this.groupBoxClick.Controls.Add(this.btnDelAll);
            this.groupBoxClick.Controls.Add(this.btnDel);
            this.groupBoxClick.Controls.Add(this.btnTestClick);
            this.groupBoxClick.Controls.Add(this.listBoxClick);
            this.groupBoxClick.Location = new System.Drawing.Point(266, 23);
            this.groupBoxClick.Name = "groupBoxClick";
            this.groupBoxClick.Size = new System.Drawing.Size(248, 156);
            this.groupBoxClick.TabIndex = 8;
            this.groupBoxClick.TabStop = false;
            this.groupBoxClick.Text = "Click";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(187, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Window";
            // 
            // txtHWND
            // 
            this.txtHWND.Location = new System.Drawing.Point(189, 46);
            this.txtHWND.Name = "txtHWND";
            this.txtHWND.Size = new System.Drawing.Size(52, 20);
            this.txtHWND.TabIndex = 9;
            this.txtHWND.Text = "hwnd";
            // 
            // cbbListWindow
            // 
            this.cbbListWindow.FormattingEnabled = true;
            this.cbbListWindow.Location = new System.Drawing.Point(10, 17);
            this.cbbListWindow.Name = "cbbListWindow";
            this.cbbListWindow.Size = new System.Drawing.Size(173, 21);
            this.cbbListWindow.TabIndex = 8;
            this.cbbListWindow.SelectedIndexChanged += new System.EventHandler(this.cbbListWindow_SelectedIndexChanged);
            // 
            // lbStatus
            // 
            this.lbStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStatus.Location = new System.Drawing.Point(10, 124);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(175, 20);
            this.lbStatus.TabIndex = 6;
            this.lbStatus.Text = "lbStatus";
            this.lbStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDel
            // 
            this.btnDel.Location = new System.Drawing.Point(189, 70);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(52, 21);
            this.btnDel.TabIndex = 3;
            this.btnDel.Text = "Del";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnClick_Event);
            // 
            // btnTestClick
            // 
            this.btnTestClick.Location = new System.Drawing.Point(189, 123);
            this.btnTestClick.Name = "btnTestClick";
            this.btnTestClick.Size = new System.Drawing.Size(52, 21);
            this.btnTestClick.TabIndex = 3;
            this.btnTestClick.Text = "Test";
            this.btnTestClick.UseVisualStyleBackColor = true;
            this.btnTestClick.Click += new System.EventHandler(this.btnClick_Event);
            // 
            // listBoxClick
            // 
            this.listBoxClick.FormattingEnabled = true;
            this.listBoxClick.HorizontalScrollbar = true;
            this.listBoxClick.Location = new System.Drawing.Point(10, 46);
            this.listBoxClick.Name = "listBoxClick";
            this.listBoxClick.Size = new System.Drawing.Size(173, 69);
            this.listBoxClick.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(91, 16);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(69, 30);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnClick_Event);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(172, 16);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(69, 30);
            this.btnOk.TabIndex = 10;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnClick_Event);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnApply);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(266, 180);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(248, 62);
            this.panel1.TabIndex = 11;
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(10, 16);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(69, 30);
            this.btnApply.TabIndex = 9;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnClick_Event);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.nbUpDownBarcode);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(248, 60);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Barcode";
            // 
            // btnDelAll
            // 
            this.btnDelAll.Location = new System.Drawing.Point(189, 94);
            this.btnDelAll.Name = "btnDelAll";
            this.btnDelAll.Size = new System.Drawing.Size(52, 21);
            this.btnDelAll.TabIndex = 3;
            this.btnDelAll.Text = "Del All";
            this.btnDelAll.UseVisualStyleBackColor = true;
            this.btnDelAll.Click += new System.EventHandler(this.btnClick_Event);
            // 
            // SettingOutputSoftware
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 249);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBoxClick);
            this.Controls.Add(this.groupBoxCom);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBoxForward);
            this.Name = "SettingOutputSoftware";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Setting Output Software";
            this.groupBoxForward.ResumeLayout(false);
            this.groupBoxForward.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nbUpDownBarcode)).EndInit();
            this.groupBoxCom.ResumeLayout(false);
            this.groupBoxCom.PerformLayout();
            this.groupBoxClick.ResumeLayout(false);
            this.groupBoxClick.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxForward;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chbxDirect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCheckDataForward;
        private System.Windows.Forms.NumericUpDown nbUpDownBarcode;
        private System.Windows.Forms.GroupBox groupBoxCom;
        private System.Windows.Forms.TextBox txtBaudrate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtComport;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBoxClick;
        private System.Windows.Forms.ListBox listBoxClick;
        private System.Windows.Forms.Button btnTestClick;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.ComboBox cbbListWindow;
        private System.Windows.Forms.TextBox txtHWND;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDelAll;
    }
}