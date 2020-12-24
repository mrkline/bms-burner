using System;
using System.Linq;
using System.Text;

namespace AltLauncher
{
    public class JoyAssgn
    {
        // Member
        public string productName = "";
        public Guid productGUID;
        public Guid instanceGUID;

        /// <summary>
        /// [0]=X
        /// [1]=Y
        /// [2]=Z
        /// [3]=Rx
        /// [4]=Ry
        /// [5]=Rz
        /// [6]=Slider0
        /// [7]=Slider1
        /// </summary>
        public AxAssgn[] axis = new AxAssgn[8];
    }
}
