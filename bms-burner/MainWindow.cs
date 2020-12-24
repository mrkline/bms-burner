using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SharpDX.DirectInput;

namespace bms_burner
{
    public partial class MainWindow : Form
    {
        private BMSConfig bmsConfig = null;
        private DirectInput input = new SharpDX.DirectInput.DirectInput();
        private Joystick throttle = null;
        private JoystickState throttleState = new JoystickState();
        private const int MAXIN = 65536;
        private bool wasWet = false;

        private Process player = new Process();

        public MainWindow()
        {
            InitializeComponent();
            player.StartInfo.UseShellExecute = false;
            player.StartInfo.FileName = "mpv.exe";
            player.StartInfo.Arguments = "--force-window=no ./blowers.ogg";
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

            lblIdle.Text = "Idle detent: " + ValueAndPercentage(bmsConfig.IdleDetent);
            lblAfterburner.Text = "AfterburnerDetent: " + ValueAndPercentage(bmsConfig.AfterburnerDetent);

            throttle = new Joystick(input, bmsConfig.ThrottleDeviceGUID);
            throttle.Acquire();
            timer.Start();
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            if (throttle == null) return;
            throttle.GetCurrentState(ref throttleState);

            // It's inverted, for some odd reason.
            var current = MAXIN - throttleState.RotationZ;

            lblThrottleReading.Text = "Throttle value: " + ValueAndPercentage(current);

            var isWet = current >= bmsConfig.AfterburnerDetent;

            picBurner.Image = isWet ? Properties.Resources.Burner : Properties.Resources.Engine;

            if (!wasWet && isWet)
            {
                player.Start();
            }

            wasWet = isWet;
        }

        private static String ValueAndPercentage(int val)
        {
            var percent = ((double)val / (double)MAXIN) * 100;
            return String.Format("{0} ({1:D2}%)", val, (int)percent);
        }
    }
}
