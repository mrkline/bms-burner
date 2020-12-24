using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using SharpDX.DirectInput;

namespace bms_burner
{
    class BMSConfig
    {
        public int IdleDetent { get; set; }
        public int AfterburnerDetent { get; set; }
        public Guid ThrottleDeviceGUID { get; set; }
        public Func<JoystickState, int> AxisDelegate { get; set; }

        /// <summary>
        /// TODO: Reading straight from axismapping.dat would be nice...
        /// </summary>
        public static BMSConfig FromAltLauncherConfig(string configDir)
        {
            var files = Directory.GetFiles(configDir, "Setup.v100.*.xml");
            var throttlePosits = files.Where(p => p.EndsWith("ThrottlePosition.xml")).ToArray();
            switch (throttlePosits.Length)
            {
                case 0: throw new FileNotFoundException("Couldn't find the alt launcher's ThrottlePosition.xml");
                case 1: break;
                default: throw new FileNotFoundException("Found several ThrottlePosition.xml files");
            }
            var throttlePosit = throttlePosits[0];

            AltLauncher.ThrottlePosition detents;

            var deserializer = new System.Xml.Serialization.XmlSerializer(typeof(AltLauncher.ThrottlePosition));
            using (var sr = new System.IO.StreamReader(throttlePosit, new System.Text.UTF8Encoding(false)))
                detents = (AltLauncher.ThrottlePosition)deserializer.Deserialize(sr);

            int index = -1;
            Guid? uid = null;

            deserializer = new System.Xml.Serialization.XmlSerializer(typeof(AltLauncher.JoyAssgn));
            foreach (var deviceFile in files.Where(p => !p.EndsWith("ThrottlePosition.xml") && !p.EndsWith("MouseWheel.xml")))
            {
                var assignments = LoadJoyAssgn(deserializer, deviceFile);
                for (int i = 0; i < assignments.axis.Length; ++i)
                {
                    var axis = assignments.axis[i];
                    if (axis.AxisName == "Throttle")
                    {
                        index = i;
                        // Lol pulling the GUID of the controller out of the file name
                        var re = Regex.Match(deviceFile, "{(.+)}");
                        uid = new Guid(re.Groups[1].Value);
                        break;
                    }
                }
            }

            if (index < 0)
            {
                throw new Exception("Throttle axis not found!");
            }

            Func<JoystickState, int> axisMapper = delegate (JoystickState js)
            {
                // From the Alt launcher:
                // [0]=X
                // [1]=Y
                // [2]=Z
                // [3]=Rx
                // [4]=Ry
                // [5]=Rz
                // [6]=Slider0
                // [7]=Slider1
                switch (index)
                {
                    case 0: return js.X;
                    case 1: return js.Y;
                    case 2: return js.Z;
                    case 3: return js.RotationX;
                    case 4: return js.RotationY;
                    case 5: return js.RotationZ;
                    case 6: return js.Sliders[0];
                    case 7: return js.Sliders[1];
                    default: throw new IndexOutOfRangeException("Unknown axis index from Alt Launcher settings");
                }
            };

            var config = new BMSConfig
            {
                IdleDetent = detents.IDLE,
                AfterburnerDetent = detents.AB,
                ThrottleDeviceGUID = uid.Value,
                AxisDelegate = axisMapper
            };
            return config;
        }

        private static AltLauncher.JoyAssgn LoadJoyAssgn(System.Xml.Serialization.XmlSerializer deserializer, string file)
        {
            using (var sr = new System.IO.StreamReader(file, new System.Text.UTF8Encoding(false)))
                return (AltLauncher.JoyAssgn)deserializer.Deserialize(sr);
        }
    }
}