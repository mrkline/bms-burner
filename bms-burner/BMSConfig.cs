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

        public int IdleDetent { get; set; } = 0;
        public int AfterburnerDetent { get; set; } = MAXIN;
        public Guid ThrottleDeviceGUID { get; set; }
        public Func<JoystickState, int> AxisDelegate { get; set; }

        //TODO: look at devicesorting.txt, it might have more information on which joystick to grab

        public static BMSConfig ConfigFromBMS(string configDirectory)
        {
            byte[] axisMappingFile = File.ReadAllBytes(Path.Combine(configDirectory, "axisMapping.dat"));
            byte[] joystickConfigFile = File.ReadAllBytes(Path.Combine(configDirectory, "joystick.cal"));
            String[] devLst = File.ReadAllLines(Path.Combine(configDirectory, "DeviceSorting.txt"));

            if (!devLst.Any())
                return null;

            StreamWriter sw = new StreamWriter("out.txt");

            // Most of this is reverse-engineered from undocumented
            // byte slinging in the Alternative Launcher. Better docs welcome.

            // Axismapping.dat starts with a 24-byte header of:
            // - The primary (pitch) device's index + 2(!?) as a 4-byte LE int
            // - The primary (pitch) device's GUID
            // - The number of devices as a 4-byte LE int.
            //
            // Following that, each bound axis has a 16-byte entry containing:
            // - The axis's device index + 2(!?) as a 4-byte LE int
            // - The axis index (see axismapper below) as a 4-byte LE int
            // - Deadzone info (format unknown) as a 4-byte field
            // - Saturation info (format unknown) as a 4-byte field
            //
            // In BMS 4.35, the axis order is Pitch, Roll, Yaw, Throttle...
            byte[] throttleBytes = new byte[16];
            Array.Copy(axisMappingFile, 72, throttleBytes, 0, 16);
            int deviceNum = throttleBytes[0];
            sw.WriteLine("Device number: " + (deviceNum - 2));
            if (deviceNum < 2)
                return null;
            int index = throttleBytes[4];
            sw.WriteLine("Index number: " + index);

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
                    default: throw new IndexOutOfRangeException("Unknown axis index from axismapping.dat");
                }
            };

            Regex r = new Regex("{(.+)}");

            // Again, the weird +2 (mouse & keyboard?)
            Guid uid = new Guid(r.Match(devLst[deviceNum - 2]).Value);
            sw.WriteLine("The GUID is: " + uid.ToString() + 
                         " and name is: " + devLst[deviceNum - 2].Substring(40));

            // joystick.cal has 24-byte entries for each axis.
            // In BMS 4.35, the axis order is Pitch, Roll, Yaw, Throttle...
            // If the axis is the throttle axis, byte 1 is the afterburner detent
            // and byte 5 is the idle detent.
            const int AB_LOC_INDEX = 0x49; // byte which determines the afterburner detent position in joystick.cal
            const int IDLE_LOC_INDEX = 0x53; //byte which determines the idle position in joystick.cal

            const int MAXIN = 65536;
            const int MAXOUT = 14848;

            // Weird scaling...
            int rawABVal = joystickConfigFile[AB_LOC_INDEX];
            int abCutoff = MAXIN - 256 * rawABVal * MAXIN / MAXOUT;

            int rawIdleVal = joystickConfigFile[IDLE_LOC_INDEX];
            int idleCutoff = 256 * rawIdleVal * MAXIN / MAXOUT;

            sw.Close();

            return new BMSConfig
            {
                IdleDetent = idleCutoff,
                AfterburnerDetent = abCutoff,
                ThrottleDeviceGUID = uid,
                AxisDelegate = axisMapper
            };
            }
    }
}
