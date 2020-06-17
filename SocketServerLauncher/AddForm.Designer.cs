namespace SocketServerLauncher
{
    partial class AddForm
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
            this.components = new System.ComponentModel.Container();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtName = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.txtIP = new DevExpress.XtraEditors.TextEdit();
            this.txtPort = new DevExpress.XtraEditors.TextEdit();
            this.radioGroup1 = new DevExpress.XtraEditors.RadioGroup();
            this.radioGroup2 = new DevExpress.XtraEditors.RadioGroup();
            this.EP1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.btnServer = new DevExpress.XtraEditors.SimpleButton();
            this.btnClient = new DevExpress.XtraEditors.SimpleButton();
            this.btnRest = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIP.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPort.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioGroup2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EP1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 16F);
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(37, 30);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(51, 27);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "서버명";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(170, 35);
            this.txtName.Name = "txtName";
            this.txtName.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.txtName.Properties.Appearance.Options.UseFont = true;
            this.txtName.Properties.Appearance.Options.UseTextOptions = true;
            this.txtName.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.txtName.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            this.txtName.Size = new System.Drawing.Size(144, 26);
            this.txtName.TabIndex = 1;
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 16F);
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(37, 78);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(150, 27);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "종류를 선택하세요.";
            // 
            // labelControl8
            // 
            this.labelControl8.Appearance.Font = new System.Drawing.Font("Tahoma", 16F);
            this.labelControl8.Appearance.Options.UseFont = true;
            this.labelControl8.Location = new System.Drawing.Point(37, 302);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(101, 25);
            this.labelControl8.TabIndex = 8;
            this.labelControl8.Text = "IP Address";
            // 
            // labelControl9
            // 
            this.labelControl9.Appearance.Font = new System.Drawing.Font("Tahoma", 16F);
            this.labelControl9.Appearance.Options.UseFont = true;
            this.labelControl9.Location = new System.Drawing.Point(37, 345);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Size = new System.Drawing.Size(38, 25);
            this.labelControl9.TabIndex = 9;
            this.labelControl9.Text = "Port";
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnCancel.Location = new System.Drawing.Point(112, 422);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 31);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "취소";
            this.btnCancel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCancel_MouseDown);
            // 
            // btnOK
            // 
            this.btnOK.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnOK.Appearance.Options.UseFont = true;
            this.btnOK.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnOK.Location = new System.Drawing.Point(215, 422);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(85, 31);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "저장";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtIP
            // 
            this.txtIP.EditValue = "";
            this.EP1.SetIconAlignment(this.txtIP, System.Windows.Forms.ErrorIconAlignment.MiddleRight);
            this.txtIP.Location = new System.Drawing.Point(170, 306);
            this.txtIP.Name = "txtIP";
            this.txtIP.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.txtIP.Properties.Appearance.Options.UseFont = true;
            this.txtIP.Properties.Appearance.Options.UseTextOptions = true;
            this.txtIP.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.txtIP.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            this.txtIP.Properties.EditFormat.FormatString = "000.000.000.000";
            this.txtIP.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.txtIP.Size = new System.Drawing.Size(158, 26);
            this.txtIP.TabIndex = 19;
            this.txtIP.Leave += new System.EventHandler(this.txtIP_Leave);
            // 
            // txtPort
            // 
            this.txtPort.EditValue = "";
            this.txtPort.Location = new System.Drawing.Point(170, 347);
            this.txtPort.Name = "txtPort";
            this.txtPort.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.txtPort.Properties.Appearance.Options.UseFont = true;
            this.txtPort.Properties.Appearance.Options.UseTextOptions = true;
            this.txtPort.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.txtPort.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            this.txtPort.Properties.EditFormat.FormatString = "0000";
            this.txtPort.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtPort.Properties.MaxLength = 4;
            this.txtPort.Size = new System.Drawing.Size(82, 26);
            this.txtPort.TabIndex = 20;
            // 
            // radioGroup1
            // 
            this.radioGroup1.Location = new System.Drawing.Point(77, 205);
            this.radioGroup1.Name = "radioGroup1";
            this.radioGroup1.Properties.Columns = 2;
            this.radioGroup1.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem("sync", "동기식", true, "sync"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem("async", "비동기식", true, "async")});
            this.radioGroup1.Size = new System.Drawing.Size(270, 32);
            this.radioGroup1.TabIndex = 24;
            this.radioGroup1.SelectedIndexChanged += new System.EventHandler(this.radioGroup1_SelectedIndexChanged);
            // 
            // radioGroup2
            // 
            this.radioGroup2.Location = new System.Drawing.Point(77, 252);
            this.radioGroup2.Name = "radioGroup2";
            this.radioGroup2.Properties.Columns = 3;
            this.radioGroup2.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem("MSSQL", "MSSQL", true, "MSSQL"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem("MYSQL", "MYSQL", true, "MYSQL")});
            this.radioGroup2.Size = new System.Drawing.Size(270, 32);
            this.radioGroup2.TabIndex = 25;
            this.radioGroup2.SelectedIndexChanged += new System.EventHandler(this.radioGroup2_SelectedIndexChanged);
            // 
            // EP1
            // 
            this.EP1.ContainerControl = this;
            // 
            // btnServer
            // 
            this.btnServer.Appearance.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnServer.Appearance.Font = new System.Drawing.Font("Tahoma", 18F);
            this.btnServer.Appearance.Options.UseBackColor = true;
            this.btnServer.Appearance.Options.UseFont = true;
            this.btnServer.Location = new System.Drawing.Point(77, 122);
            this.btnServer.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.btnServer.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnServer.Name = "btnServer";
            this.btnServer.Size = new System.Drawing.Size(55, 55);
            this.btnServer.TabIndex = 26;
            this.btnServer.Tag = "server";
            this.btnServer.Text = "S";
            this.btnServer.Click += new System.EventHandler(this.btnServer_Click);
            // 
            // btnClient
            // 
            this.btnClient.Appearance.BackColor = System.Drawing.Color.Gold;
            this.btnClient.Appearance.Font = new System.Drawing.Font("Tahoma", 18F);
            this.btnClient.Appearance.Options.UseBackColor = true;
            this.btnClient.Appearance.Options.UseFont = true;
            this.btnClient.Location = new System.Drawing.Point(186, 122);
            this.btnClient.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.btnClient.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnClient.Name = "btnClient";
            this.btnClient.Size = new System.Drawing.Size(55, 55);
            this.btnClient.TabIndex = 27;
            this.btnClient.Tag = "client";
            this.btnClient.Text = "C";
            this.btnClient.Click += new System.EventHandler(this.btnServer_Click);
            // 
            // btnRest
            // 
            this.btnRest.Appearance.BackColor = System.Drawing.Color.LightGreen;
            this.btnRest.Appearance.Font = new System.Drawing.Font("Tahoma", 18F);
            this.btnRest.Appearance.Options.UseBackColor = true;
            this.btnRest.Appearance.Options.UseFont = true;
            this.btnRest.Location = new System.Drawing.Point(292, 122);
            this.btnRest.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.btnRest.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnRest.Name = "btnRest";
            this.btnRest.Size = new System.Drawing.Size(55, 55);
            this.btnRest.TabIndex = 28;
            this.btnRest.Tag = "rest";
            this.btnRest.Text = "R";
            this.btnRest.Click += new System.EventHandler(this.btnServer_Click);
            // 
            // AddForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(416, 473);
            this.Controls.Add(this.btnRest);
            this.Controls.Add(this.btnClient);
            this.Controls.Add(this.btnServer);
            this.Controls.Add(this.radioGroup2);
            this.Controls.Add(this.radioGroup1);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.labelControl9);
            this.Controls.Add(this.labelControl8);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.labelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AddForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "추가";
            this.Load += new System.EventHandler(this.AddForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIP.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPort.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioGroup2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EP1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtName;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraEditors.LabelControl labelControl9;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.TextEdit txtIP;
        private DevExpress.XtraEditors.TextEdit txtPort;
        private DevExpress.XtraEditors.RadioGroup radioGroup1;
        private DevExpress.XtraEditors.RadioGroup radioGroup2;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider EP1;
        private DevExpress.XtraEditors.SimpleButton btnServer;
        private DevExpress.XtraEditors.SimpleButton btnRest;
        private DevExpress.XtraEditors.SimpleButton btnClient;
    }
}