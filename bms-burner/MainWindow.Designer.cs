
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
            this.lblBMSLocation = new System.Windows.Forms.Label();
            this.txtBMSLocation = new System.Windows.Forms.TextBox();
            this.btnBMSLocationBrowse = new System.Windows.Forms.Button();
            this.dlgBMSLocation = new System.Windows.Forms.OpenFileDialog();
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
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 450);
            this.Controls.Add(this.btnBMSLocationBrowse);
            this.Controls.Add(this.txtBMSLocation);
            this.Controls.Add(this.lblBMSLocation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainWindow";
            this.Text = "BMS Burner";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblBMSLocation;
        private System.Windows.Forms.TextBox txtBMSLocation;
        private System.Windows.Forms.Button btnBMSLocationBrowse;
        private System.Windows.Forms.OpenFileDialog dlgBMSLocation;
    }
}

