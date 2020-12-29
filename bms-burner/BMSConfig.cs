using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Serilog;
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

        /// <summary>
        /// Load throttle settings from BMS's axismapping.dat, DeviceSorting.txt, and joystick.cal
        /// </summary>
        /// <param name="configDirectory">BMS/User/Config</param>
        public static BMSConfig ConfigFromBMS(string configDirectory)
        {
            byte[] axisMappingFile = File.ReadAllBytes(Path.Combine(configDirectory, "axismapping.dat"));
            byte[] joystickConfigFile = File.ReadAllBytes(Path.Combine(configDirectory, "joystick.cal"));
            Guid[] devices = File.ReadAllLines(Path.Combine(configDirectory, "devicesorting.txt"))
                .Select(line => Regex.Match(line, "{(.+)}").Groups[1].Value)
                .Select(str => Guid.Parse(str))
                .ToArray();

            Log.Debug("Successfully loaded axismapping.dat, joystick.cal, & devicesorting.txt");
            Log.Verbose("Device GUIDs from devicesorting.txt: {0}", devices);

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
            Log.Verbose("Raw throttle device number (index + 2): {0}", throttleBytes[0]);
            int deviceNum = throttleBytes[0] - 2;

            int axisIndex = throttleBytes[4];
            Log.Debug("Throttle axis index: {0}", axisIndex);

            if (deviceNum < 0 || deviceNum >= devices.Length)
            {
                throw new IndexOutOfRangeException(
                    String.Format("Device index {0} found in axismapipng.dat, but {1} devices found in devicesorting.txt",
                        deviceNum, devices.Length
                    ));
            }
            if (axisIndex < 0 || axisIndex > 7)
            {
                throw new IndexOutOfRangeException(
                    String.Format("Axis index from axismapping.dat ({0}) is outside [0, 7]", axisIndex));
            }

            int axisMapper(JoystickState js)
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
                switch (axisIndex)
                {
                    case 0: return js.X;
                    case 1: return js.Y;
                    case 2: return js.Z;
                    case 3: return js.RotationX;
                    case 4: return js.RotationY;
                    case 5: return js.RotationZ;
                    case 6: return js.Sliders[0];
                    case 7: return js.Sliders[1];
                    default: throw new IndexOutOfRangeException(
                        String.Format("Axis index ({0}) is outside [0, 7]", axisIndex));
                }
            }

            Guid uid = devices[deviceNum];
            Log.Debug("Throttle GUID: {0}", uid);

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
            Log.Debug("AB detent: {0} ({1} raw)", abCutoff, rawABVal);


            int rawIdleVal = joystickConfigFile[IDLE_LOC_INDEX];
            int idleCutoff = 256 * rawIdleVal * MAXIN / MAXOUT;
            Log.Debug("Idle detent: {0} ({1} raw)", idleCutoff, rawIdleVal);

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
