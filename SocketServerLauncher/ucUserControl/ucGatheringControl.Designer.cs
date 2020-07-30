namespace SocketServerLauncher
{
    partial class ucGatheringControl
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.ceLog = new DevExpress.XtraEditors.CheckEdit();
            this.lblName = new DevExpress.XtraEditors.LabelControl();
            this.btndel = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSetting = new DevExpress.XtraEditors.SimpleButton();
            this.pbStatus = new System.Windows.Forms.PictureBox();
            this.lblUsertext = new DevExpress.XtraEditors.LabelControl();
            this.lblcolon = new DevExpress.XtraEditors.LabelControl();
            this.lblUserCount = new DevExpress.XtraEditors.LabelControl();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.lboxLog = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.txtIP = new DevExpress.XtraEditors.LabelControl();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ceLog.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btndel)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbStatus)).BeginInit();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.LightSkyBlue;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lboxLog, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 1, 7);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 9;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(295, 310);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.Controls.Add(this.ceLog, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblName, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btndel, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(10, 10);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(275, 30);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // ceLog
            // 
            this.ceLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ceLog.Location = new System.Drawing.Point(188, 3);
            this.ceLog.Name = "ceLog";
            this.ceLog.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.ceLog.Properties.Appearance.Options.UseFont = true;
            this.ceLog.Properties.Caption = "log";
            this.ceLog.Size = new System.Drawing.Size(54, 24);
            this.ceLog.TabIndex = 1;
            this.ceLog.CheckedChanged += new System.EventHandler(this.ceLog_CheckedChanged);
            // 
            // lblName
            // 
            this.lblName.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Appearance.Options.UseFont = true;
            this.lblName.Appearance.Options.UseTextOptions = true;
            this.lblName.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblName.Location = new System.Drawing.Point(3, 3);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(179, 24);
            this.lblName.TabIndex = 3;
            this.lblName.Text = "labelControl1";
            // 
            // btndel
            // 
            this.btndel.BackgroundImage = global::SocketServerLauncher.Properties.Resources.garbage_symbol_png_8_original;
            this.btndel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btndel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btndel.Location = new System.Drawing.Point(246, 1);
            this.btndel.Margin = new System.Windows.Forms.Padding(1);
            this.btndel.Name = "btndel";
            this.btndel.Size = new System.Drawing.Size(28, 28);
            this.btndel.TabIndex = 2;
            this.btndel.TabStop = false;
            this.btndel.Click += new System.EventHandler(this.btndel_Click);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 7;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel3.Controls.Add(this.btnSetting, 6, 0);
            this.tableLayoutPanel3.Controls.Add(this.pbStatus, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.lblUsertext, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.lblcolon, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.lblUserCount, 4, 0);
            this.tableLayoutPanel3.Controls.Add(this.simpleButton2, 5, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(10, 50);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(275, 30);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // btnSetting
            // 
            this.btnSetting.BackgroundImage = global::SocketServerLauncher.Properties.Resources.setting;
            this.btnSetting.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSetting.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.btnSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSetting.Location = new System.Drawing.Point(244, 2);
            this.btnSetting.Margin = new System.Windows.Forms.Padding(2);
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.Size = new System.Drawing.Size(29, 26);
            this.btnSetting.TabIndex = 0;
            this.btnSetting.Click += new System.EventHandler(this.btnSetting_Click);
            // 
            // pbStatus
            // 
            this.pbStatus.BackgroundImage = global::SocketServerLauncher.Properties.Resources.RED;
            this.pbStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbStatus.Location = new System.Drawing.Point(0, 0);
            this.pbStatus.Margin = new System.Windows.Forms.Padding(0);
            this.pbStatus.Name = "pbStatus";
            this.pbStatus.Size = new System.Drawing.Size(30, 29);
            this.pbStatus.TabIndex = 2;
            this.pbStatus.TabStop = false;
            // 
            // lblUsertext
            // 
            this.lblUsertext.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblUsertext.Appearance.Options.UseFont = true;
            this.lblUsertext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblUsertext.Location = new System.Drawing.Point(45, 3);
            this.lblUsertext.Name = "lblUsertext";
            this.lblUsertext.Size = new System.Drawing.Size(61, 24);
            this.lblUsertext.TabIndex = 3;
            this.lblUsertext.Text = "접속자 수";
            this.lblUsertext.Visible = false;
            // 
            // lblcolon
            // 
            this.lblcolon.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblcolon.Appearance.Options.UseFont = true;
            this.lblcolon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblcolon.Location = new System.Drawing.Point(112, 3);
            this.lblcolon.Name = "lblcolon";
            this.lblcolon.Size = new System.Drawing.Size(4, 24);
            this.lblcolon.TabIndex = 4;
            this.lblcolon.Text = ":";
            this.lblcolon.Visible = false;
            // 
            // lblUserCount
            // 
            this.lblUserCount.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblUserCount.Appearance.Options.UseFont = true;
            this.lblUserCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblUserCount.Location = new System.Drawing.Point(122, 3);
            this.lblUserCount.Name = "lblUserCount";
            this.lblUserCount.Size = new System.Drawing.Size(76, 24);
            this.lblUserCount.TabIndex = 5;
            this.lblUserCount.Text = "0";
            this.lblUserCount.Visible = false;
            // 
            // simpleButton2
            // 
            this.simpleButton2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.simpleButton2.Location = new System.Drawing.Point(204, 3);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(35, 24);
            this.simpleButton2.TabIndex = 6;
            this.simpleButton2.Text = "test";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // lboxLog
            // 
            this.lboxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lboxLog.FormattingEnabled = true;
            this.lboxLog.ItemHeight = 12;
            this.lboxLog.Location = new System.Drawing.Point(10, 90);
            this.lboxLog.Margin = new System.Windows.Forms.Padding(0);
            this.lboxLog.Name = "lboxLog";
            this.lboxLog.Size = new System.Drawing.Size(275, 165);
            this.lboxLog.TabIndex = 2;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 170F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Controls.Add(this.simpleButton1, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.txtIP, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(10, 265);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(275, 35);
            this.tableLayoutPanel4.TabIndex = 3;
            // 
            // simpleButton1
            // 
            this.simpleButton1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.simpleButton1.Location = new System.Drawing.Point(173, 3);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(99, 29);
            this.simpleButton1.TabIndex = 0;
            this.simpleButton1.Text = "connect";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // txtIP
            // 
            this.txtIP.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtIP.Appearance.Options.UseFont = true;
            this.txtIP.Appearance.Options.UseTextOptions = true;
            this.txtIP.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.txtIP.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.txtIP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtIP.Location = new System.Drawing.Point(3, 3);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(164, 29);
            this.txtIP.TabIndex = 1;
            this.txtIP.Text = "123.123.123.123 / 5555";
            // 
            // timer1
            // 
            this.timer1.Interval = 3000;
            // 
            // ucGatheringControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ucGatheringControl";
            this.Size = new System.Drawing.Size(295, 310);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ceLog.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btndel)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbStatus)).EndInit();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.ListBox lboxLog;
        private DevExpress.XtraEditors.SimpleButton btnSetting;
        private System.Windows.Forms.PictureBox pbStatus;
        private DevExpress.XtraEditors.LabelControl lblUsertext;
        private DevExpress.XtraEditors.LabelControl lblcolon;
        private DevExpress.XtraEditors.LabelControl lblUserCount;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.LabelControl txtIP;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private DevExpress.XtraEditors.CheckEdit ceLog;
        private System.Windows.Forms.PictureBox btndel;
        private DevExpress.XtraEditors.LabelControl lblName;
    }
}
