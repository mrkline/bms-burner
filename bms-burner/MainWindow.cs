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
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace bms_burner
{
    public partial class MainWindow : Form
    {
        private Timer bmsPoller;
        private bool isBMSRunning = false;
        private const String BMS_REG_PATH = "HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Benchmark Sims\\Falcon BMS 4.35";
        private BMSConfig bmsConfig = null;
        private DirectInput input = new SharpDX.DirectInput.DirectInput();
        private Joystick throttle = null;
        private JoystickState throttleState = new JoystickState();
        public const int MAXIN = 65536;
        private const int BMS_POLLER_INTERVAL = 1000;
        private bool wasWet = false;
        public String BMSPath;

        private Process player = new Process();
        private AfterburnerIndicator ABIndicator;

        public MainWindow()
        {
            InitializeComponent();

            bmsPoller = new Timer();
            bmsPoller.Interval = BMS_POLLER_INTERVAL;
            bmsPoller.Tick += bmsPoller_Tick;

            BMSPath = (String)Registry.GetValue(BMS_REG_PATH, "baseDir", "");
            this.txtBMSLocation.Text = BMSPath;
            loadBMSConfig();
            player.StartInfo.UseShellExecute = false;
            player.StartInfo.FileName = "mpv.exe";
            player.StartInfo.Arguments = "--force-window=no ./blowers.ogg";

            if (File.Exists("ABWindow.xml"))
            {
                var deserializer = new System.Xml.Serialization.XmlSerializer(typeof(AfterburnerIndicator));
                var sr = new System.IO.StreamReader("ABWindow.xml", new System.Text.UTF8Encoding(false));
                ABIndicator = (AfterburnerIndicator)deserializer.Deserialize(sr);
            }
            else
            {
                ABIndicator = new AfterburnerIndicator();
            }

        }

        private void btnBMSLocationBrowse_Click(object sender, EventArgs e) 
        {
            if (dlgBMSLocation.ShowDialog() != DialogResult.OK) return;
            BMSPath = Path.GetDirectoryName(dlgBMSLocation.FileName);
            txtBMSLocation.Text = BMSPath;
            loadBMSConfig();
        }

        private void loadBMSConfig()
        {
            try
            {
                txtBMSLocation.Text = BMSPath;

                bmsConfig = BMSConfig.FromAltLauncherConfig(BMSPath + "/User/Config");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error reading BMS Config from " + BMSPath, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblIdle.Text = "Idle detent: " + ValueAndPercentage(bmsConfig.IdleDetent);
            lblAfterburner.Text = "AfterburnerDetent: " + ValueAndPercentage(bmsConfig.AfterburnerDetent);

            try
            {
                throttle = new Joystick(input, bmsConfig.ThrottleDeviceGUID);
                throttle.Acquire();
                bmsPoller.Start();
                timer.Start();

            }
            catch (Exception ex)
            {
                MessageBox.Show("No throttle found", "WARNING", MessageBoxButtons.OK);
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (throttle == null) return;
            throttle.GetCurrentState(ref throttleState);

            // It's inverted, for some odd reason.
            var current = MAXIN - bmsConfig.AxisDelegate(throttleState);

            lblThrottleReading.Text = "Throttle value: " + ValueAndPercentage(current);

            var isWet = current >= bmsConfig.AfterburnerDetent;

            // Set the image only on transitions, since setting it each tick generates a crapton of garbage.
            if (!wasWet && isWet)
            {
                picBurner.Image = Properties.Resources.Burner;
                //player.Start();
            }
            else if (wasWet && !isWet)
            {
                picBurner.Image = Properties.Resources.Engine;
            }

            wasWet = isWet;
        }

        private void bmsPoller_Tick(object sender, EventArgs e)
        {
            if (throttle == null) return;
            if (!isBMSRunning && Process.GetProcessesByName("Falcon BMS").Length > 0)
            {
                isBMSRunning = true;
                ABIndicator.Execute(bmsConfig.AxisDelegate, bmsConfig.AfterburnerDetent, throttle);
            }

            if (isBMSRunning)
            {
                IntPtr tmpPointer = OpenFileMapping(0x0004, false, "FalconSharedMemoryArea");
                IntPtr mmfPointer = MapViewOfFile(tmpPointer, 0x0004, 0, 0, 0);
                BMSData data = (BMSData)Convert.ChangeType(Marshal.PtrToStructure(mmfPointer, typeof(BMSData)), typeof(BMSData));
                bool isBMS3d = (data.hsiBits & 0x80000000) == 0x80000000;
                ABIndicator.onBMSPoll(isBMS3d);
            }

        }

        private static String ValueAndPercentage(int val)
        {
            var percent = ((double)val / (double)MAXIN) * 100;
            return String.Format("{0} ({1:D2}%)", val, (int)percent);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenFileMapping(uint dwDesiredAccess,
            bool bInheritHandle,
           string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint
           dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow,
           uint dwNumberOfBytesToMap);

        private void ABConfig_Click(object sender, EventArgs e)
        {
            ABIndicator = AfterburnerWindow.ShowAfterburnerWindow(ABIndicator);
        }
    }
}
