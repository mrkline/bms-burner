
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
            this.lblBMSLocation = new System.Windows.Forms.Label();
            this.txtBMSLocation = new System.Windows.Forms.TextBox();
            this.btnBMSLocationBrowse = new System.Windows.Forms.Button();
            this.dlgBMSLocation = new System.Windows.Forms.OpenFileDialog();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.lblThrottleReading = new System.Windows.Forms.Label();
            this.lblIdle = new System.Windows.Forms.Label();
            this.grpThrottleValues = new System.Windows.Forms.GroupBox();
            this.lblAfterburner = new System.Windows.Forms.Label();
            this.grpThrottleValues.SuspendLayout();
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
            this.txtBMSLocation.Location = new System.Drawing.Point(96, 12);
            this.txtBMSLocation.Name = "txtBMSLocation";
            this.txtBMSLocation.Size = new System.Drawing.Size(295, 20);
            this.txtBMSLocation.TabIndex = 1;
            // 
            // btnBMSLocationBrowse
            // 
            this.btnBMSLocationBrowse.Location = new System.Drawing.Point(397, 10);
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
            this.dlgBMSLocation.FileName = "BMS Directory";
            this.dlgBMSLocation.ValidateNames = false;
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // lblThrottleReading
            // 
            this.lblThrottleReading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblThrottleReading.AutoSize = true;
            this.lblThrottleReading.Location = new System.Drawing.Point(6, 84);
            this.lblThrottleReading.Name = "lblThrottleReading";
            this.lblThrottleReading.Size = new System.Drawing.Size(117, 13);
            this.lblThrottleReading.TabIndex = 3;
            this.lblThrottleReading.Text = "<Throttle reading here>";
            // 
            // lblIdle
            // 
            this.lblIdle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblIdle.AutoSize = true;
            this.lblIdle.Location = new System.Drawing.Point(6, 28);
            this.lblIdle.Name = "lblIdle";
            this.lblIdle.Size = new System.Drawing.Size(97, 13);
            this.lblIdle.TabIndex = 4;
            this.lblIdle.Text = "Idle detent: <TBD>";
            // 
            // grpThrottleValues
            // 
            this.grpThrottleValues.Controls.Add(this.lblAfterburner);
            this.grpThrottleValues.Controls.Add(this.lblThrottleReading);
            this.grpThrottleValues.Controls.Add(this.lblIdle);
            this.grpThrottleValues.Location = new System.Drawing.Point(12, 39);
            this.grpThrottleValues.Name = "grpThrottleValues";
            this.grpThrottleValues.Size = new System.Drawing.Size(200, 100);
            this.grpThrottleValues.TabIndex = 5;
            this.grpThrottleValues.TabStop = false;
            this.grpThrottleValues.Text = "Throttle Values";
            // 
            // lblAfterburner
            // 
            this.lblAfterburner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAfterburner.AutoSize = true;
            this.lblAfterburner.Location = new System.Drawing.Point(6, 56);
            this.lblAfterburner.Name = "lblAfterburner";
            this.lblAfterburner.Size = new System.Drawing.Size(132, 13);
            this.lblAfterburner.TabIndex = 5;
            this.lblAfterburner.Text = "Afterburner detent: <TBD>";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 450);
            this.Controls.Add(this.btnBMSLocationBrowse);
            this.Controls.Add(this.txtBMSLocation);
            this.Controls.Add(this.lblBMSLocation);
            this.Controls.Add(this.grpThrottleValues);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainWindow";
            this.Text = "BMS Burner";
            this.grpThrottleValues.ResumeLayout(false);
            this.grpThrottleValues.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblBMSLocation;
        private System.Windows.Forms.TextBox txtBMSLocation;
        private System.Windows.Forms.Button btnBMSLocationBrowse;
        private System.Windows.Forms.OpenFileDialog dlgBMSLocation;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label lblThrottleReading;
        private System.Windows.Forms.Label lblIdle;
        private System.Windows.Forms.GroupBox grpThrottleValues;
        private System.Windows.Forms.Label lblAfterburner;
    }
}

