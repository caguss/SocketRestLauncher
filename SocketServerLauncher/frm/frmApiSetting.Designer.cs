namespace SocketServerLauncher.frm
{
    partial class frmApiSetting
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._lblPort = new DevExpress.XtraEditors.LabelControl();
            this._lblIp = new DevExpress.XtraEditors.LabelControl();
            this._tbIp = new DevExpress.XtraEditors.TextEdit();
            this._tbPort = new DevExpress.XtraEditors.TextEdit();
            this._btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._tbIp.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._tbPort.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 7;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.Controls.Add(this._lblPort, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this._lblIp, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this._tbIp, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this._tbPort, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this._btnSave, 5, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(284, 261);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // _lblPort
            // 
            this._lblPort.Appearance.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblPort.Appearance.Options.UseFont = true;
            this._lblPort.Appearance.Options.UseTextOptions = true;
            this._lblPort.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.tableLayoutPanel1.SetColumnSpan(this._lblPort, 2);
            this._lblPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lblPort.Location = new System.Drawing.Point(23, 132);
            this._lblPort.Name = "_lblPort";
            this._lblPort.Size = new System.Drawing.Size(78, 41);
            this._lblPort.TabIndex = 1;
            this._lblPort.Text = "PORT";
            // 
            // _lblIp
            // 
            this._lblIp.Appearance.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblIp.Appearance.Options.UseFont = true;
            this._lblIp.Appearance.Options.UseTextOptions = true;
            this._lblIp.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.tableLayoutPanel1.SetColumnSpan(this._lblIp, 2);
            this._lblIp.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lblIp.Location = new System.Drawing.Point(23, 85);
            this._lblIp.Name = "_lblIp";
            this._lblIp.Size = new System.Drawing.Size(78, 41);
            this._lblIp.TabIndex = 0;
            this._lblIp.Text = "IP";
            // 
            // _tbIp
            // 
            this._tbIp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this._tbIp, 3);
            this._tbIp.EditValue = "192.168.0.1";
            this._tbIp.Location = new System.Drawing.Point(107, 85);
            this._tbIp.Name = "_tbIp";
            this._tbIp.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._tbIp.Properties.Appearance.Options.UseFont = true;
            this._tbIp.Size = new System.Drawing.Size(150, 36);
            this._tbIp.TabIndex = 2;
            // 
            // _tbPort
            // 
            this._tbPort.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this._tbPort, 3);
            this._tbPort.EditValue = "8080";
            this._tbPort.Location = new System.Drawing.Point(107, 132);
            this._tbPort.Name = "_tbPort";
            this._tbPort.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._tbPort.Properties.Appearance.Options.UseFont = true;
            this._tbPort.Size = new System.Drawing.Size(150, 36);
            this._tbPort.TabIndex = 3;
            // 
            // _btnSave
            // 
            this._btnSave.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnSave.Appearance.Options.UseFont = true;
            this._btnSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this._btnSave.Location = new System.Drawing.Point(203, 179);
            this._btnSave.Name = "_btnSave";
            this._btnSave.Size = new System.Drawing.Size(54, 41);
            this._btnSave.TabIndex = 4;
            this._btnSave.Text = "저장";
            this._btnSave.Click += new System.EventHandler(this._btnSave_Click);
            // 
            // frmApiSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "frmApiSetting";
            this.Text = "frmApiSetting";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._tbIp.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._tbPort.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraEditors.LabelControl _lblIp;
        private DevExpress.XtraEditors.LabelControl _lblPort;
        private DevExpress.XtraEditors.TextEdit _tbIp;
        private DevExpress.XtraEditors.TextEdit _tbPort;
        private DevExpress.XtraEditors.SimpleButton _btnSave;
    }
}