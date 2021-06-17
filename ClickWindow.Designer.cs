
namespace THHSoftMiddle
{
    partial class ClickWindow
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbbListWindow = new System.Windows.Forms.ComboBox();
            this.txtHWND = new System.Windows.Forms.TextBox();
            this.txtMouseX = new System.Windows.Forms.TextBox();
            this.txtOffsetY = new System.Windows.Forms.TextBox();
            this.txtMouseY = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnTestClick = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.lbStatus = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lbStatus);
            this.groupBox3.Controls.Add(this.cbbListWindow);
            this.groupBox3.Controls.Add(this.txtHWND);
            this.groupBox3.Controls.Add(this.txtMouseX);
            this.groupBox3.Controls.Add(this.txtOffsetY);
            this.groupBox3.Controls.Add(this.txtMouseY);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.btnCancel);
            this.groupBox3.Controls.Add(this.btnTestClick);
            this.groupBox3.Controls.Add(this.btnOk);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(262, 203);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Position Click";
            // 
            // cbbListWindow
            // 
            this.cbbListWindow.FormattingEnabled = true;
            this.cbbListWindow.Location = new System.Drawing.Point(65, 25);
            this.cbbListWindow.Name = "cbbListWindow";
            this.cbbListWindow.Size = new System.Drawing.Size(179, 21);
            this.cbbListWindow.TabIndex = 4;
            this.cbbListWindow.SelectedIndexChanged += new System.EventHandler(this.cbbListWindow_SelectedIndexChanged);
            // 
            // txtHWND
            // 
            this.txtHWND.Location = new System.Drawing.Point(145, 56);
            this.txtHWND.Name = "txtHWND";
            this.txtHWND.Size = new System.Drawing.Size(100, 20);
            this.txtHWND.TabIndex = 3;
            this.txtHWND.Text = "hwnd";
            // 
            // txtMouseX
            // 
            this.txtMouseX.Location = new System.Drawing.Point(65, 56);
            this.txtMouseX.Name = "txtMouseX";
            this.txtMouseX.Size = new System.Drawing.Size(70, 20);
            this.txtMouseX.TabIndex = 3;
            this.txtMouseX.Text = "100";
            // 
            // txtOffsetY
            // 
            this.txtOffsetY.Location = new System.Drawing.Point(145, 84);
            this.txtOffsetY.Name = "txtOffsetY";
            this.txtOffsetY.Size = new System.Drawing.Size(43, 20);
            this.txtOffsetY.TabIndex = 3;
            this.txtOffsetY.Text = "30";
            // 
            // txtMouseY
            // 
            this.txtMouseY.Location = new System.Drawing.Point(65, 84);
            this.txtMouseY.Name = "txtMouseY";
            this.txtMouseY.Size = new System.Drawing.Size(70, 20);
            this.txtMouseY.TabIndex = 3;
            this.txtMouseY.Text = "100";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Window";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "X";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 88);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Y";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(18, 130);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 30);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btn_click);
            // 
            // btnTestClick
            // 
            this.btnTestClick.Location = new System.Drawing.Point(194, 84);
            this.btnTestClick.Name = "btnTestClick";
            this.btnTestClick.Size = new System.Drawing.Size(52, 21);
            this.btnTestClick.TabIndex = 2;
            this.btnTestClick.Text = "Test";
            this.btnTestClick.UseVisualStyleBackColor = true;
            this.btnTestClick.Click += new System.EventHandler(this.btn_click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(156, 130);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(88, 30);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btn_click);
            // 
            // lbStatus
            // 
            this.lbStatus.Location = new System.Drawing.Point(4, 180);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(258, 20);
            this.lbStatus.TabIndex = 5;
            this.lbStatus.Text = "status";
            // 
            // ClickWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.ClientSize = new System.Drawing.Size(289, 227);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ClickWindow";
            this.Text = "ClickWindow";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cbbListWindow;
        private System.Windows.Forms.TextBox txtMouseX;
        private System.Windows.Forms.TextBox txtMouseY;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnTestClick;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TextBox txtHWND;
        private System.Windows.Forms.TextBox txtOffsetY;
        private System.Windows.Forms.Label lbStatus;
    }
}