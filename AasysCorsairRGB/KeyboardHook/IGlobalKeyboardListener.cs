using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AasysCorsairRGB.KeyboardHook;

namespace AasysCorsairRGB
{
    interface IGlobalKeyboardListener
    {
        void OnKeyPressed(KeyCombo keyCombo);
    }
}
