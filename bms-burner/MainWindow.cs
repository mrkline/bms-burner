using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SharpDX.DirectInput;

namespace bms_burner
{
    public partial class MainWindow : Form
    {
        private BMSConfig bmsConfig = null;
        private DirectInput input = null;
        private Joystick throttle = null;
        private JoystickState throttleState = new JoystickState();
        private const int MAXIN = 65536;

        public MainWindow()
        {
            InitializeComponent();
            input = new SharpDX.DirectInput.DirectInput();
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

            lblIdle.Text = "Idle detent: " + bmsConfig.IdleDetent.ToString();
            lblAfterburner.Text = "AfterburnerDetent " + bmsConfig.AfterburnerDetent.ToString();

            throttle = new Joystick(input, bmsConfig.ThrottleDeviceGUID);
            throttle.Acquire();
            timer.Start();
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            if (throttle == null) return;
            throttle.GetCurrentState(ref throttleState);
            lblThrottleReading.Text = (MAXIN - throttleState.RotationZ).ToString();
        }
    }
}
