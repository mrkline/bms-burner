using SharpDX.DirectInput;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace bms_burner
{
    public class AfterburnerIndicator
    {
        public bool hasRun = false;
        protected Joystick throttle;
        private JoystickState thrState = new JoystickState();
        protected int aB;
        protected Point loc;
        public ScreenLocation scrLoc;
        public int width;
        public int height;
        public bool isEnabled;
        const int JOYSTICK_POLL_PERIOD = 16;
        const int BMS_POLL_PERIOD = 1000;
        Form f;
        Timer throttlePoller;

        private Func<JoystickState, int> AxisDelegate;

        public AfterburnerIndicator()
        {
            scrLoc = ScreenLocation.Top_Left;
            width = 80;
            height = 80;
            isEnabled = false;
        }

        public void Execute(Func<JoystickState, int> refFunc, int aBLoc, Joystick throttle)
        {
            this.throttle = throttle;
            this.AxisDelegate = refFunc;
            this.aB = aBLoc;
            f = new Form();
            f.BackColor = Color.Blue;
            f.FormBorderStyle = FormBorderStyle.None;
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            f.Size = new Size(width, height);
            f.TopMost = true;
            f.StartPosition = FormStartPosition.Manual;
            Point p = new Point(0, 0);
            switch (scrLoc)
            {
                case ScreenLocation.Top_Left:
                    break;
                case ScreenLocation.Top_Right:
                    p = new Point(screenWidth - width, 0);
                    break;
                case ScreenLocation.Bottom_Left:
                    p = new Point(0, screenHeight - height);
                    break;
                case ScreenLocation.Bottom_Right:
                    p = new Point(screenWidth - width, screenHeight - height);
                    break;
            }
            f.Location = p;
            f.Opacity = 0;

            throttlePoller = new Timer();
            throttlePoller.Interval = JOYSTICK_POLL_PERIOD;
            throttlePoller.Tick += new EventHandler(onThrottlePoll);

            f.ShowDialog();
        }
        public void onBMSPoll(bool isBMS3d)
        {
            if (isBMS3d && !hasRun)
            {
                f.Opacity = 1;
                throttlePoller.Start();
                hasRun = true;
            }
            if (!isBMS3d && hasRun)
            {
                f.Close();
                throttlePoller.Stop();
            }
        }
        protected void onThrottlePoll(object sender, EventArgs e)
        {
            throttle.GetCurrentState(ref thrState);
            int input = AxisDelegate(thrState);
            //reverse the input
            input = MainWindow.MAXIN - input;
            if (input > aB)
                f.BackColor = Color.Red;
            else
                f.BackColor = Color.Blue;
        }

        public String getScreenLocation()
        {
            switch (scrLoc)
            {
                case ScreenLocation.Top_Left:
                    return "Top Left";
                case ScreenLocation.Top_Right:
                    return "Top Right";
                case ScreenLocation.Bottom_Left:
                    return "Bottom Left";
                case ScreenLocation.Bottom_Right:
                    return "Bottom Right";
            }
            return "Top Left";
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenFileMapping(uint dwDesiredAccess,
            bool bInheritHandle,
           string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint
           dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow,
           uint dwNumberOfBytesToMap);
    }
}
