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
    public class AfterburnerOverlay
    {
        public bool hasRun = false;
        private Form f = null;

        private ScreenLocation location = ScreenLocation.Top_Left;
        public ScreenLocation ScreenLocation
        {
            get { return location; }
            set { location = value; UpdateLocation(); }
        }

        public int Width
        {
            get { return f.Width; }
            set { f.Width = value; UpdateLocation(); }
        }

        public int Height
        {
            get { return f.Height; }
            set { f.Height = value; UpdateLocation(); }
        }

        public bool Enabled { get; set; }

        public AfterburnerOverlay() : this(80, 80) { }

        public AfterburnerOverlay(int width, int height)
        {
            f = new Form();
            f.MinimumSize = new Size(1, 1);
            f.BackColor = Color.Blue;
            f.FormBorderStyle = FormBorderStyle.None;
            f.Size = new Size(width, height);
            f.TopMost = true;
            f.StartPosition = FormStartPosition.Manual;
            UpdateLocation();
        }

        public static AfterburnerOverlay LoadOrDefault()
        {
            if (File.Exists("AfterburnerOverlay.xml"))
            {
                var deserializer = new System.Xml.Serialization.XmlSerializer(typeof(AfterburnerOverlay));
                using (var sr = new System.IO.StreamReader("AfterburnerOverlay.xml", new System.Text.UTF8Encoding(false)))
                    return (AfterburnerOverlay)deserializer.Deserialize(sr);
            }
            else
            {
                return new AfterburnerOverlay();
            }
        }

        public void Save()
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(AfterburnerOverlay));
            using (var sw = new StreamWriter("AfterburnerOverlay.xml", false, new System.Text.UTF8Encoding(false)))
                serializer.Serialize(sw, this);
        }

        public void Entering3D()
        {
            if (Enabled) f.Show();
        }

        public void Leaving3D()
        {
            if (Enabled) f.Hide();
        }

        public void BurnersOn()
        {
            f.BackColor = Color.Red;
        }

        public void BurnersOff()
        {
            f.BackColor = Color.Blue;
        }

        public String getScreenLocation()
        {
            switch (ScreenLocation)
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

        private void UpdateLocation()
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            Point p = new Point(0, 0);
            switch (ScreenLocation)
            {
                case ScreenLocation.Top_Left:
                    break;
                case ScreenLocation.Top_Right:
                    p = new Point(screenWidth - f.Width, 0);
                    break;
                case ScreenLocation.Bottom_Left:
                    p = new Point(0, screenHeight - f.Height);
                    break;
                case ScreenLocation.Bottom_Right:
                    p = new Point(screenWidth - f.Width, screenHeight - f.Height);
                    break;
            }
            f.Location = p;
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
