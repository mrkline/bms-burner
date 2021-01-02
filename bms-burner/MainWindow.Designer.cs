
namespace bms_burner
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.lblBMSLocation = new System.Windows.Forms.Label();
            this.txtBMSLocation = new System.Windows.Forms.TextBox();
            this.btnBMSLocationBrowse = new System.Windows.Forms.Button();
            this.dlgBMSLocation = new System.Windows.Forms.OpenFileDialog();
            this.throttlePoll = new System.Windows.Forms.Timer(this.components);
            this.picBurner = new System.Windows.Forms.PictureBox();
            this.btnConfigureOverlay = new System.Windows.Forms.Button();
            this.chkOverlayEnabled = new System.Windows.Forms.CheckBox();
            this.grpThrottleValues = new System.Windows.Forms.GroupBox();
            this.lblThrottleReading = new System.Windows.Forms.Label();
            this.lblAfterburner = new System.Windows.Forms.Label();
            this.stripBmsStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            ((System.ComponentModel.ISupportInitialize)(this.picBurner)).BeginInit();
            this.grpThrottleValues.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblBMSLocation
            // 
            this.lblBMSLocation.AutoSize = true;
            this.lblBMSLocation.Location = new System.Drawing.Point(13, 15);
            this.lblBMSLocation.Name = "lblBMSLocation";
            this.lblBMSLocation.Size = new System.Drawing.Size(77, 13);
            this.lblBMSLocation.TabIndex = 0;
            this.lblBMSLocation.Text = "BMS Location:";
            // 
            // txtBMSLocation
            // 
            this.txtBMSLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBMSLocation.Location = new System.Drawing.Point(96, 12);
            this.txtBMSLocation.Name = "txtBMSLocation";
            this.txtBMSLocation.ReadOnly = true;
            this.txtBMSLocation.Size = new System.Drawing.Size(170, 20);
            this.txtBMSLocation.TabIndex = 1;
            // 
            // btnBMSLocationBrowse
            // 
            this.btnBMSLocationBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBMSLocationBrowse.Location = new System.Drawing.Point(272, 10);
            this.btnBMSLocationBrowse.Name = "btnBMSLocationBrowse";
            this.btnBMSLocationBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBMSLocationBrowse.TabIndex = 2;
            this.btnBMSLocationBrowse.Text = "Browse...";
            this.btnBMSLocationBrowse.UseVisualStyleBackColor = true;
            this.btnBMSLocationBrowse.Click += new System.EventHandler(this.btnBMSLocationBrowse_Click);
            // 
            // dlgBMSLocation
            // 
            this.dlgBMSLocation.CheckFileExists = false;
            this.dlgBMSLocation.CheckPathExists = false;
            this.dlgBMSLocation.FileName = "BMS Directory";
            this.dlgBMSLocation.ValidateNames = false;
            // 
            // throttlePoll
            // 
            this.throttlePoll.Interval = 16;
            this.throttlePoll.Tick += new System.EventHandler(this.throttlePoll_tick);
            // 
            // picBurner
            // 
            this.picBurner.ErrorImage = null;
            this.picBurner.Image = global::bms_burner.Properties.Resources.Engine;
            this.picBurner.Location = new System.Drawing.Point(219, 39);
            this.picBurner.Name = "picBurner";
            this.picBurner.Size = new System.Drawing.Size(128, 128);
            this.picBurner.TabIndex = 6;
            this.picBurner.TabStop = false;
            // 
            // btnConfigureOverlay
            // 
            this.btnConfigureOverlay.Location = new System.Drawing.Point(80, 39);
            this.btnConfigureOverlay.Name = "btnConfigureOverlay";
            this.btnConfigureOverlay.Size = new System.Drawing.Size(116, 23);
            this.btnConfigureOverlay.TabIndex = 7;
            this.btnConfigureOverlay.Text = "Configure Overlay...";
            this.btnConfigureOverlay.UseVisualStyleBackColor = true;
            this.btnConfigureOverlay.Visible = false;
            this.btnConfigureOverlay.Click += new System.EventHandler(this.ABConfig_Click);
            // 
            // chkOverlayEnabled
            // 
            this.chkOverlayEnabled.AutoSize = true;
            this.chkOverlayEnabled.Location = new System.Drawing.Point(12, 43);
            this.chkOverlayEnabled.Name = "chkOverlayEnabled";
            this.chkOverlayEnabled.Size = new System.Drawing.Size(62, 17);
            this.chkOverlayEnabled.TabIndex = 8;
            this.chkOverlayEnabled.Text = "Overlay";
            this.chkOverlayEnabled.UseVisualStyleBackColor = true;
            this.chkOverlayEnabled.CheckedChanged += new System.EventHandler(this.chkOverlayEnabled_CheckedChanged);
            // 
            // grpThrottleValues
            // 
            this.grpThrottleValues.Controls.Add(this.lblAfterburner);
            this.grpThrottleValues.Controls.Add(this.lblThrottleReading);
            this.grpThrottleValues.Location = new System.Drawing.Point(12, 69);
            this.grpThrottleValues.Name = "grpThrottleValues";
            this.grpThrottleValues.Size = new System.Drawing.Size(200, 98);
            this.grpThrottleValues.TabIndex = 5;
            this.grpThrottleValues.TabStop = false;
            this.grpThrottleValues.Text = "Throttle Values";
            // 
            // lblThrottleReading
            // 
            this.lblThrottleReading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblThrottleReading.AutoSize = true;
            this.lblThrottleReading.Location = new System.Drawing.Point(6, 59);
            this.lblThrottleReading.Name = "lblThrottleReading";
            this.lblThrottleReading.Size = new System.Drawing.Size(117, 13);
            this.lblThrottleReading.TabIndex = 3;
            this.lblThrottleReading.Text = "<Throttle reading here>";
            // 
            // lblAfterburner
            // 
            this.lblAfterburner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAfterburner.AutoSize = true;
            this.lblAfterburner.Location = new System.Drawing.Point(6, 26);
            this.lblAfterburner.Name = "lblAfterburner";
            this.lblAfterburner.Size = new System.Drawing.Size(132, 13);
            this.lblAfterburner.TabIndex = 5;
            this.lblAfterburner.Text = "Afterburner detent: <TBD>";
            // 
            // stripBmsStatus
            // 
            this.stripBmsStatus.Name = "stripBmsStatus";
            this.stripBmsStatus.Size = new System.Drawing.Size(89, 15);
            this.stripBmsStatus.Text = "BMS Status: Off";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stripBmsStatus});
            this.statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.statusStrip.Location = new System.Drawing.Point(0, 173);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(359, 20);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 9;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 193);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.chkOverlayEnabled);
            this.Controls.Add(this.btnConfigureOverlay);
            this.Controls.Add(this.picBurner);
            this.Controls.Add(this.btnBMSLocationBrowse);
            this.Controls.Add(this.txtBMSLocation);
            this.Controls.Add(this.lblBMSLocation);
            this.Controls.Add(this.grpThrottleValues);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "BMS Burner Sounds";
            ((System.ComponentModel.ISupportInitialize)(this.picBurner)).EndInit();
            this.grpThrottleValues.ResumeLayout(false);
            this.grpThrottleValues.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblBMSLocation;
        private System.Windows.Forms.TextBox txtBMSLocation;
        private System.Windows.Forms.Button btnBMSLocationBrowse;
        private System.Windows.Forms.OpenFileDialog dlgBMSLocation;
        private System.Windows.Forms.Timer throttlePoll;
        private System.Windows.Forms.PictureBox picBurner;
        private System.Windows.Forms.Button btnConfigureOverlay;
        private System.Windows.Forms.CheckBox chkOverlayEnabled;
        private System.Windows.Forms.GroupBox grpThrottleValues;
        private System.Windows.Forms.Label lblAfterburner;
        private System.Windows.Forms.Label lblThrottleReading;
        private System.Windows.Forms.ToolStripStatusLabel stripBmsStatus;
        private System.Windows.Forms.StatusStrip statusStrip;
    }
}

