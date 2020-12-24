using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;

namespace bms_burner
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnBMSLocationBrowse_Click(object sender, EventArgs e)
        {
            if (dlgBMSLocation.ShowDialog() != DialogResult.OK) return;

            try
            {
                var dirPath = Path.GetDirectoryName(dlgBMSLocation.FileName);
                txtBMSLocation.Text = dirPath;

                bmsConfig = BMSConfig.FromAltLauncherConfig(dirPath + "/User/Config");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error reading BMS Config", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private BMSConfig bmsConfig;
    }
}
