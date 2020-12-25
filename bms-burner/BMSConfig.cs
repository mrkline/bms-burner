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
        public const int MAXIN = 65536;

        public static Dictionary<int, JoystickAxis> AXIS_DICT = new Dictionary<int, JoystickAxis>()
        {
            {0, JoystickAxis.X},
            {0, JoystickAxis.Y},
            {0, JoystickAxis.Z},
            {0, JoystickAxis.Rx},
            {0, JoystickAxis.Ry},
            {0, JoystickAxis.Rz},
            {0, JoystickAxis.Slider0},
            {0, JoystickAxis.Slider1},
        };
        const int THR_LOC = 0x48 ; //indeex where the 16-byte throttle axis information starts at in axismapping.dat
        const int AXIS_INFO_LENGTH = 0x10; // each axis is stored using 16 bytes of data
        const int THR_BOUND_INDEX = 0x54; // byte which determines if throttle is bound in joystick.cal
        const int AB_LOC_INDEX = 0x49; // byte which determines the afterburner detent position in joystick.cal
        const int IDLE_LOC_INDEX = 0x53; //byte which determines the idle position in joystick.cal

        public int IdleDetent { get; set; } = 0;
        public int AfterburnerDetent { get; set; } = MAXIN;
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
            foreach (var deviceFile in files.Where(p => !p.EndsWith("ThrottlePosition.xml") 
                                                     && !p.EndsWith("MouseWheel.xml")
                                                     && !p.EndsWith("AfterburnerIndicator.xml")))
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

        //TODO: look at devicesorting.txt, it might have more information on which joystick to grab

        public BMSConfig ConfigFromBMS(byte[] axisMappingFile, byte[] joystickConfigFile)
        {
            byte[] throttleBytes = new byte[16];
            Array.Copy(axisMappingFile, 72, throttleBytes, 0, 16);
            int deviceNum = throttleBytes[0];
            if (deviceNum < 2)
                return null;
            int index = throttleBytes[4];

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

            var din = new DirectInput();
            var lst = din.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices);
            if (!lst.Any())
                return null;
            Guid uid = lst[deviceNum - 2].InstanceGuid;

            const int MAXIN = 65536;
            const int MAXOUT = 14848;

            int rawABVal = joystickConfigFile[AB_LOC_INDEX];
            int ABCutoff = MAXIN - 256 * rawABVal * MAXIN / MAXOUT;

            int rawIdleVal = joystickConfigFile[IDLE_LOC_INDEX];
            int IdleCutoff = MAXIN - 256 * rawIdleVal * MAXIN / MAXOUT;

            return new BMSConfig
            {
                IdleDetent = IdleCutoff,
                AfterburnerDetent = ABCutoff,
                ThrottleDeviceGUID = uid,
                AxisDelegate = axisMapper
            };
        }

        private static AltLauncher.JoyAssgn LoadJoyAssgn(System.Xml.Serialization.XmlSerializer deserializer, string file)
        {
            using (var sr = new System.IO.StreamReader(file, new System.Text.UTF8Encoding(false)))
                return (AltLauncher.JoyAssgn)deserializer.Deserialize(sr);
        }
    }
}
