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
        // Machinery for polling BMS shared memory and determining if we're in 3D
        private Timer bmsPoll;
        private IntPtr bmsMapHandle = IntPtr.Zero;
        private IntPtr bmsMapping = IntPtr.Zero;
        private BMSData bmsData;
        private bool in3D = false;

        // Configuration loaded from BMS (or the Alternative Launcher)
        // and DirectInput machinery for actually reading the throttle.
        private BMSConfig bmsConfig = null;
        private DirectInput input = new SharpDX.DirectInput.DirectInput();
        private Joystick throttle = null;
        private JoystickState throttleState = new JoystickState();
        private bool isWet = false;

        private Process player = new Process();
        private AfterburnerOverlay overlay;

        public MainWindow()
        {
            InitializeComponent();

            bmsPoll = new Timer();
            bmsPoll.Interval = 1000;
            bmsPoll.Tick += bmsPoll_Tick;

            const String BMS_REG_PATH = "HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Benchmark Sims\\Falcon BMS 4.35";
            this.txtBMSLocation.Text = (String)Registry.GetValue(BMS_REG_PATH, "baseDir", "");
            loadBMSConfig();

            player.StartInfo.UseShellExecute = false;
            player.StartInfo.FileName = "mpv.exe";
            player.StartInfo.Arguments = "--force-window=no ./blowers.ogg";

            overlay = AfterburnerOverlay.LoadOrDefault();
            chkOverlayEnabled.Checked = overlay.Enabled;
        }

        private void btnBMSLocationBrowse_Click(object sender, EventArgs e) 
        {
            if (dlgBMSLocation.ShowDialog() != DialogResult.OK) return;
            txtBMSLocation.Text = Path.GetDirectoryName(dlgBMSLocation.FileName);
            loadBMSConfig();
        }

        private void loadBMSConfig()
        {
            try
            {
                bmsConfig = BMSConfig.ConfigFromBMS(txtBMSLocation.Text + "/User/Config");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error reading BMS Config from " + 
                                txtBMSLocation.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (bmsConfig == null)
            {
                MessageBox.Show("No input devices found, or no throttle bound. " +
                                "Please make sure your throttle is bound in BMS and plugged in");
                return;
            }

            lblIdle.Text = "Idle detent: " + ValueAndPercentage(bmsConfig.IdleDetent);
            lblAfterburner.Text = "AfterburnerDetent: " + ValueAndPercentage(bmsConfig.AfterburnerDetent);

            try
            {
                throttle = new Joystick(input, bmsConfig.ThrottleDeviceGUID);
                throttle.Acquire();
                StreamWriter sw = new StreamWriter("out.txt", append: true);
                sw.WriteLine("Acquired throttle name is: " + throttle.Properties.ProductName);
                sw.Close();
                bmsPoll.Start();
                throttlePoll.Start();

            }
            catch (Exception)
            {
                MessageBox.Show("No throttle found matching your BMS throttle binding", "WARNING", MessageBoxButtons.OK);
            }
        }

        private void throttlePoll_tick(object sender, EventArgs e)
        {
            if (throttle == null) return;
            throttle.GetCurrentState(ref throttleState);

            // It's inverted, for some odd reason.
            var current = BMSConfig.MAXIN - bmsConfig.AxisDelegate(throttleState);

            lblThrottleReading.Text = "Throttle value: " + ValueAndPercentage(current);

            bool wasWet = isWet;
            isWet = current >= bmsConfig.AfterburnerDetent;

            // Set the image only on transitions, since setting it each tick generates a crapton of garbage.
            if (!wasWet && isWet)
            {
                picBurner.Image = Properties.Resources.Burner;
                if (in3D)
                {
                    player.Start();
                    overlay.BurnersOn();
                }
            }
            else if (wasWet && !isWet)
            {
                picBurner.Image = Properties.Resources.Engine;
                if (in3D)
                {
                    overlay.BurnersOff();
                }
            }

            wasWet = isWet;
        }

        private void bmsPoll_Tick(object sender, EventArgs e)
        {
            bool wasIn3D = in3D;

            UpdateBMSMapping();
            in3D = (bmsData.hsiBits & 0x80000000) != 0;

            if (!wasIn3D && in3D) overlay.Entering3D();
            else if (wasIn3D && !in3D) overlay.Leaving3D();
        }

        private void UpdateBMSMapping()
        {
            if (!Process.GetProcessesByName("Falcon BMS").Any() && bmsMapping != IntPtr.Zero)
            {
                UnmapViewOfFile(bmsMapping);
                Trace.Assert(bmsMapHandle != IntPtr.Zero);
                CloseHandle(bmsMapHandle);

                bmsMapping = IntPtr.Zero;
                bmsMapHandle = IntPtr.Zero;
            }
            else if (bmsMapping == IntPtr.Zero)
            {
                Trace.Assert(bmsMapHandle == IntPtr.Zero);
                bmsMapHandle = OpenFileMapping(0x0004, false, "FalconSharedMemoryArea");
                bmsMapping = MapViewOfFile(bmsMapHandle, 0x0004, 0, 0, 0);
            }
            if (bmsMapping == IntPtr.Zero) bmsData = new BMSData();
            else bmsData = (BMSData)Marshal.PtrToStructure(bmsMapping, typeof(BMSData));
        }

        private static String ValueAndPercentage(int val)
        {
            var percent = ((double)val / (double)BMSConfig.MAXIN) * 100;
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

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        private void ABConfig_Click(object sender, EventArgs e)
        {
            overlay = wndAfterburner.ShowAfterburnerWindow(overlay);
        }

        private void chkOverlayEnabled_CheckedChanged(object sender, EventArgs e)
        {
            btnConfigureOverlay.Visible = chkOverlayEnabled.Checked;
            overlay.Enabled = chkOverlayEnabled.Checked;
        }
    }
}
