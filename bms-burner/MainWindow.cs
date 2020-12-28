using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using SharpDX.DirectInput;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace bms_burner
{
    public partial class MainWindow : Form
    {
        // Machinery for polling BMS shared memory and determining if we're in 3D
        private readonly Timer bmsPoll;
        private BMSData bmsData;
        private bool in3D = false;
        // Related native WinAPI nonsense
        private IntPtr bmsMapHandle = IntPtr.Zero;
        private IntPtr bmsMapping = IntPtr.Zero;

        // Configuration loaded from BMS
        // and DirectInput machinery for actually reading the throttle.
        private BMSConfig bmsConfig = null;
        private readonly DirectInput input = new SharpDX.DirectInput.DirectInput();
        private Joystick throttle = null;
        private JoystickState throttleState = new JoystickState();
        private bool isWet = false;

        // Apparently .NET doesn't provide any media support out of the box
        // except for playing .wav files.
        // To play a sound, we bundle MPV and run it.
        // TODO: Do something less hokey and dumb to play sound.
        private readonly Process player = new Process();

        private AfterburnerOverlay overlay;

        public MainWindow()
        {
            InitializeComponent();

            bmsPoll = new Timer
            {
                Interval = 1000
            };
            bmsPoll.Tick += bmsPoll_Tick;

            // Try to populate the BMS location from its registry entry.
            const String BMS_REG_PATH = "HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Benchmark Sims\\Falcon BMS 4.35";
            this.txtBMSLocation.Text = (String)Registry.GetValue(BMS_REG_PATH, "baseDir", "");
            loadBMSConfig();

            player.StartInfo.UseShellExecute = false;
            player.StartInfo.FileName = "mpv.exe";

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
                MessageBox.Show(ex.Message, "Error reading BMS Config from " + txtBMSLocation.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblIdle.Text = "Idle detent: " + ValueAndPercentage(bmsConfig.IdleDetent);
            lblAfterburner.Text = "AfterburnerDetent: " + ValueAndPercentage(bmsConfig.AfterburnerDetent);

            try
            {
                throttle = new Joystick(input, bmsConfig.ThrottleDeviceGUID);
                throttle.Acquire();
                bmsPoll.Start();
                throttlePoll.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No throttle found", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (!wasWet && isWet)
            {
                // Set the image only on transitions, since setting it each tick
                // generates a crapton of garbage.
                // (Does it reload the file each time?)
                picBurner.Image = Properties.Resources.Burner;
                if (in3D)
                {
                    player.StartInfo.Arguments = "--force-window=no ./burners-on.ogg";
                    player.Start();
                    overlay.BurnersOn();
                }
            }
            else if (wasWet && !isWet)
            {
                picBurner.Image = Properties.Resources.Engine;
                if (in3D)
                {
                    player.StartInfo.Arguments = "--force-window=no ./burners-off.ogg";
                    player.Start();
                    overlay.BurnersOff();
                }
            }
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
            // If we havn't yet, try opening the BMS shared memory.
            if (bmsMapping == IntPtr.Zero)
            {
                Trace.Assert(bmsMapHandle == IntPtr.Zero);
                bmsMapHandle = OpenFileMapping(0x0004, false, "FalconSharedMemoryArea");
                // Gracefully returns IntPtr.Zero if bmsMapHandle is zero.
                bmsMapping = MapViewOfFile(bmsMapHandle, 0x0004, 0, 0, 0);
            }
            // If BMS stopped running, unmap the shared memory.
            else if (!Process.GetProcessesByName("Falcon BMS").Any())
            {
                UnmapViewOfFile(bmsMapping);
                Trace.Assert(bmsMapHandle != IntPtr.Zero);
                CloseHandle(bmsMapHandle);

                bmsMapping = IntPtr.Zero;
                bmsMapHandle = IntPtr.Zero;
            }

            // If the shared memory isn't mapped, zero out bmsData.
            if (bmsMapping == IntPtr.Zero) bmsData = new BMSData();
            // If it is mapped, grab a copy of the data!
            else bmsData = (BMSData)Marshal.PtrToStructure(bmsMapping, typeof(BMSData));
        }

        /// <summary>
        /// Convert the given joystick value into a string
        /// of itself and a percentage of the max value.
        /// </summary>
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
