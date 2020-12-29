using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using SharpDX.DirectInput;
using Microsoft.Win32;
using Serilog;
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
            Log.Information("Initializing main window");

            InitializeComponent();

            bmsPoll = new Timer
            {
                Interval = 1000
            };
            bmsPoll.Tick += bmsPoll_Tick;

            player.StartInfo.UseShellExecute = false;
            player.StartInfo.FileName = "mpv.exe";

            overlay = AfterburnerOverlay.LoadOrDefault();
            chkOverlayEnabled.Checked = overlay.Enabled;

            // Try to populate the BMS location from its registry entry.
            const String BMS_REG_PATH = "HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Benchmark Sims\\Falcon BMS 4.35";
            this.txtBMSLocation.Text = (String)Registry.GetValue(BMS_REG_PATH, "baseDir", "");
            Log.Information("Attempted to load BMS path from registry, got {Text}", this.txtBMSLocation.Text);
            loadBMSConfigAndRun();
        }

        private void btnBMSLocationBrowse_Click(object sender, EventArgs e) 
        {
            if (dlgBMSLocation.ShowDialog() != DialogResult.OK) return;
            txtBMSLocation.Text = Path.GetDirectoryName(dlgBMSLocation.FileName);
            Log.Debug("User browsed to BMS directory {0}", txtBMSLocation.Text);
            loadBMSConfigAndRun();
        }

        private void loadBMSConfigAndRun()
        {
            // Axe past stuff and stop our timers while we load something new.
            throttle?.Dispose();
            throttle = null;
            bmsConfig = null;
            bmsPoll.Stop();
            throttlePoll.Stop();

            if (!txtBMSLocation.Text.Any())
            {
                Log.Warning("BMS location not set; not loading configuration.");
                return;
            }

            var configPath = txtBMSLocation.Text + "/User/Config";
            Log.Information("Loading BMS configuration from {0}", configPath);

            try
            {
                bmsConfig = BMSConfig.ConfigFromBMS(configPath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Couldn't load BMS configuration from {0}", configPath);
                MessageBox.Show("Couldn't load BMS configuration from " + configPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblIdle.Text = "Idle detent: " + ValueAndPercentage(bmsConfig.IdleDetent);
            lblAfterburner.Text = "Afterburner detent: " + ValueAndPercentage(bmsConfig.AfterburnerDetent);

            try
            {
                throttle = new Joystick(input, bmsConfig.ThrottleDeviceGUID);
                throttle.Acquire();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Couldn't find and acquire throttle {0}", bmsConfig.ThrottleDeviceGUID);
                MessageBox.Show("Couldn't find throttle", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throttle?.Dispose();
                throttle = null;
                bmsConfig = null;
                return;
            }

            bmsPoll.Start();
            throttlePoll.Start();
        }

        private void throttlePoll_tick(object sender, EventArgs e)
        {
            if (throttle == null) return;

            Trace.Assert(bmsConfig != null);

            throttle.GetCurrentState(ref throttleState);

            // It's inverted, for some odd reason.
            var current = BMSConfig.MAXIN - bmsConfig.AxisDelegate(throttleState);

            lblThrottleReading.Text = "Throttle value: " + ValueAndPercentage(current);

            bool wasWet = isWet;
            isWet = current >= bmsConfig.AfterburnerDetent;

            if (!wasWet && isWet)
            {
                Log.Debug("Going from mil to afterburner...");

                // Set the image only on transitions, since setting it each tick
                // generates a crapton of garbage.
                // (Does it reload the file each time?)
                picBurner.Image = Properties.Resources.Burner;
                if (in3D)
                {
                    Log.Debug("...and we're in 3D. Playing burners-on, changing overlay");
                    player.StartInfo.Arguments = "--force-window=no ./burners-on.ogg";
                    player.Start();
                    overlay.BurnersOn();
                }
                else
                {
                    Log.Debug("...but we're not in 3D. Do nothing.");
                }
            }
            else if (wasWet && !isWet)
            {
                Log.Debug("Going from afterburner to mil...");

                picBurner.Image = Properties.Resources.Engine;
                if (in3D)
                {
                    Log.Debug("...and we're in 3D. Playing burners-off, changing overlay");
                    player.StartInfo.Arguments = "--force-window=no ./burners-off.ogg";
                    player.Start();
                    overlay.BurnersOff();
                }
                else
                {
                    Log.Debug("...but we're not in 3D. Do nothing.");
                }
            }
        }

        private void bmsPoll_Tick(object sender, EventArgs e)
        {
            bool wasIn3D = in3D;

            UpdateBMSMapping();
            in3D = (bmsData.hsiBits & 0x80000000) != 0;

            if (!wasIn3D && in3D)
            {
                Log.Information("Entering 3D");
                overlay.Entering3D();
            }
            else if (wasIn3D && !in3D)
            {
                Log.Information("Leaving 3D");
                overlay.Leaving3D();
            }
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

                if (bmsMapping != IntPtr.Zero)
                {
                    Log.Information("Mapped FalconSharedMemoryArea");
                }
            }
            // If BMS stopped running, unmap the shared memory.
            else if (!Process.GetProcessesByName("Falcon BMS").Any())
            {
                Log.Information("BMS closed. Unmapping FalconSharedMemoryArea.");

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
