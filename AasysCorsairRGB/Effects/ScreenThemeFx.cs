using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CUE.NET.Devices.Generic;

namespace AasysCorsairRGB.Effects
{
    class ScreenThemeFx : ThreadedFx
    {
        private CorsairRgb _corsairRgb;
        private CorsairLed[] _corsairLeds;

        private Bitmap _bitmap;
        private Graphics _bitMapGraphics;
        private Rectangle _screenBounds;
        private int _width;
        private int _height;

        public int Samples { get; set; } = 1000;

        public ScreenThemeFx() 
        {
            _corsairRgb = CorsairRgb.INSTANCE;
            _corsairLeds = _corsairRgb.GetAllLeds();

            _screenBounds = Screen.GetBounds(Point.Empty);

            _width = _screenBounds.Width;
            _height = _screenBounds.Height;

            _bitmap = new Bitmap(_width, _height);
            _bitMapGraphics = Graphics.FromImage(_bitmap);
        }

        protected override void Update()
        {
            int maxSample = 10;
            long startTime = 0;
            int currentSample = 1;

            Color color;
            int r, g, b;
            while (true)
            {

                if (currentSample == 1)
                {
                    startTime = DateTime.Now.Ticks;
                    //Console.WriteLine(startTime);
                }
                else if (currentSample >= maxSample)
                {
                    double diff = (DateTime.Now.Ticks - startTime) / 10000000.0;

                    double fpm = (maxSample / diff);

                    Console.WriteLine(fpm);

                    currentSample = 0;
                }
                currentSample++;

                _bitMapGraphics.CopyFromScreen(Point.Empty, Point.Empty, _screenBounds.Size);

                r = 0;
                g = 0;
                b = 0;

                for (int i = 0; i < Samples; i++)
                {
                    color = _bitmap.GetPixel(CommonUtil.Random.Next(0, _width), CommonUtil.Random.Next(0, _height));

                    r += color.R;
                    g += color.G;
                    b += color.B;
                }

                r /= Samples;
                g /= Samples;
                b /= Samples;

                double exg = 0.5;

                _corsairRgb.ColorAll(_corsairLeds, Color.FromArgb(255, (int) (r/1.5), (int) (g/1.5), (int) (b/1.5)));
                _corsairRgb.Update();
            }
        }
    }
}
