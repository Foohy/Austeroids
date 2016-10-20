using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Austeroids
{
    class Interop
    {
        //Retrieve the current press-state of a specified key
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vKey);

        //Get if the selected key is pressed
        public static bool IsKeyDown(Keys key)
        {
            return (GetAsyncKeyState(key) & 0x8000) != 0;
        }
    }
}
