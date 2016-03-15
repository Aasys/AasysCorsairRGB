using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using CUE.NET.Devices.Generic;
using Timer = System.Timers.Timer;

namespace AasysCorsairRGB.Effects
{
    class DotaFX:IRunnableFx
    {
        private static string[] HP_LEDS_ID =
        {
            "Escape", "F1", "F2", "F3", "F4", "F5", "F6",
            "F7", "F8", "F9", "F10", "F11", "F12"
        };

        private static string[] MP_LEDS_ID =
        {
            "GraveAccentAndTilde", "D1", "D2", "D3", "D4", "D5", "D6","D7", "D8",
            "D9", "D0", "MinusAndUnderscore", "EqualsAndPlus", "Backspace"
        };

        private CorsairRgb _corsairRgb;
        private CorsairLed[] _hpLeds;
        private CorsairLed[] _mpLeds;

        private Timer _timer;

        private Bitmap _bitmap;
        private Graphics _bitMapGraphics;
        private Rectangle _screenBounds;

        public bool Running { get; private set; } = false;

        public int HpXStart { get; set; } = 1353;
        public int HpXEnd { get; set; } = 2672;
        public int HpY { get; set; } = 1829;
        public int MpXStart { get; set; } = 1353;
        public int MpXEnd { get; set; } = 2672;
        public int MpY { get; set; } = 1890;

        public DotaFX()
        {
            _corsairRgb = CorsairRgb.INSTANCE;
            _hpLeds = _corsairRgb.GetKeyboardLeds(HP_LEDS_ID);
            _mpLeds = _corsairRgb.GetKeyboardLeds(MP_LEDS_ID);

            _timer = new Timer(500);
            _timer.Elapsed += new ElapsedEventHandler(Update);

            _screenBounds = Screen.GetBounds(Point.Empty);
            _bitmap = new Bitmap(_screenBounds.Width, _screenBounds.Height);
            _bitMapGraphics = Graphics.FromImage(_bitmap);
        }

        private void Update(object sender, ElapsedEventArgs e)
        {
            _bitMapGraphics.CopyFromScreen(Point.Empty, Point.Empty, _screenBounds.Size);
            //_bitmap.Save("d:/test.jpg", ImageFormat.Bmp);
            //HP bar
            int hpStep = (HpXEnd - HpXStart) / _hpLeds.Length;
            bool dead = false;
            for (int i = 0; i < _hpLeds.Length; i++)
            {
                Color color = _bitmap.GetPixel(HpXStart + i * hpStep, HpY);
                //Console.WriteLine(color);
                //dead
                if (i == 0 && color.G < 60)
                {
                    dead = true;
                    _corsairRgb.ColorAll(_hpLeds, Color.White);
                    break;
                }
                _hpLeds[i].Color = (color.G > 80) ? Color.LawnGreen : Color.Black;
            }
            if (!dead)
            {
                int mpStep = (MpXEnd - MpXStart)/_mpLeds.Length;
                for (int i = 0; i < _mpLeds.Length; i++)
                {
                    Color color = _bitmap.GetPixel(MpXStart + i*mpStep, MpY);
                    //Console.WriteLine(color);
                    _mpLeds[i].Color = (color.B > 80) ? Color.Blue : Color.Black;
                }
            }
            else
            {

                _corsairRgb.ColorAll(_mpLeds, Color.Black);
            }
            _corsairRgb.Update();
        }
        
        public void Start()
        {
            if (Running)
                return;
            _timer.Start();
            Running = true;
        }


        public void Stop()
        {
            if (!Running)
                return;
            _timer.Stop();
            Running = false;
        }
    }
}
