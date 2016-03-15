using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using CUE.NET.Devices.Generic;
using Timer = System.Timers.Timer;

namespace AasysCorsairRGB.Effects
{
    class ScreenMappingFx:IRunnableFx
    {
        private CorsairRgb _corsairRgb;

        private Thread _thread;

        private Bitmap _bitmap;
        private Graphics _bitMapGraphics;
        private Rectangle _screenBounds;

        public bool Running { get; private set; } = false;

        public List<PointLed> _pointLeds = new List<PointLed>();

        internal class PointLed
        {
            internal PointLed(int x, int y, CorsairLed led)
            {
                X = x;
                Y = y;
                Led = led;
            }

            public CorsairLed Led { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }

        public ScreenMappingFx()
        {
            _corsairRgb = CorsairRgb.INSTANCE;

            _screenBounds = Screen.GetBounds(Point.Empty);
            _bitmap = new Bitmap(_screenBounds.Width, _screenBounds.Height);
            _bitMapGraphics = Graphics.FromImage(_bitmap);

            InitializePointLed(_corsairRgb.GetkeyboardLedMap());
        }

        private void InitializePointLed(CorsairLed[,] keyboardMap)
        {

            int colMax = keyboardMap.GetLength(1);
            int rowMax = keyboardMap.GetLength(0);

            for (int i = 0; i < rowMax; i++)
            {

                int row, col = 0;
                row = Convert.ToInt32(((double)i / rowMax) * _screenBounds.Height);
                for (int j = 0; j < colMax; j++)
                {
                    if (keyboardMap[i, j] != null)
                    {
                        col = Convert.ToInt32(((double)j / colMax) * _screenBounds.Width);
                        _pointLeds.Add(new PointLed(col, row, keyboardMap[i,j]));
                    }
                }
            }
        }

        private void Update()
        {
            int maxSample = 10;

            long startTime=0;

            int currentSample = 1;

            while (true)
            {
                if (currentSample == 1)
                {
                    startTime = DateTime.Now.Ticks;
                    //Console.WriteLine(startTime);
                }
                else if (currentSample >= maxSample)
                {
                    double diff = (DateTime.Now.Ticks - startTime)/10000000.0;

                    double fpm = (maxSample/diff);

                    Console.WriteLine(fpm);

                    currentSample = 0;
                }
                currentSample++;

                _bitMapGraphics.CopyFromScreen(Point.Empty, Point.Empty, _screenBounds.Size);

                foreach (var pointLed in _pointLeds)
                {
                    pointLed.Led.Color = _bitmap.GetPixel(pointLed.X, pointLed.Y);
                }
               
                _corsairRgb.Update();
            }
        }

        public void Start()
        {
            if (_thread == null)
            {
                _thread = new Thread(new ThreadStart(Update));
                _thread.Start();
                Running = true;
                return;
            }
            if (Running)
                return;
            _thread.Resume();
            Running = true;
        }


        public void Stop()
        {
            if (!Running)
                return;
            _thread.Suspend();
            Running = false;
        }
    }
}
