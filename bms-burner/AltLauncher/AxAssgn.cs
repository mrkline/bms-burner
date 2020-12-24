using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltLauncher
{
    /// <summary>
    /// Means each physical axis on a joystick.
    /// </summary>
    public class AxAssgn
    {
        // Property for Falcon Alt Launcher XML
        public string AxisName { get; set; }
        public DateTime AssgnDate { get; set; }
        public bool Invert { get; set; }
        public AxCurve Saturation { get; set; }
        public AxCurve Deadzone { get; set; }
    }

    public enum AxCurve
    {
        None,
        Small,
        Medium,
        Large
    }
}
