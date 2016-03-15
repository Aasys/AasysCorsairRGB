using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CUE.NET.Devices.Generic;

namespace AasysCorsairRGB.Effects
{
    class BinaryGkeyColock : IRunnableFx
    {
        private static readonly string[] G_KEYS_ID =
        {
            "G1", "G2", "G3", "G4", "G5", "G6",
            "G7", "G8", "G9", "G10", "G11", "G12",
            "G13", "G14", "G15", "G16", "G17", "G18"
        };

        private static readonly int TOTAL_TIME = 24 * 60;

        private readonly CorsairRgb _corsairRgb;
        private readonly CorsairLed[] _gKeys;
        private Timer _timer;

        public bool Hour24 { get; set; } = false;
        public bool AmPmIndicatior { get; set; } = true;

        public BinaryGkeyColock()
        {
            _corsairRgb = CorsairRgb.INSTANCE;
            _gKeys = _corsairRgb.GetKeyboardLeds(G_KEYS_ID);
        }

        public bool Running { get; private set; } = false;

        public void Start()
        {
            if (_timer == null)
            {
                _timer = new Timer(1000);
                _timer.Elapsed += new ElapsedEventHandler(Update);
            }
            _timer.Start();
            Running = true;
        }

        public void Stop()
        {
            _timer.Stop();
            Running = false;
        }

        public bool IsRunning()
        {
            return Running;
        }

        private void Update(object sender, EventArgs e)
        {
            var hour24 = DateTime.Now.Hour;
            var isAm = hour24 == 0 || hour24 < 12;
            var hour12 = hour24;
            if (hour24 > 12)
                hour12 = hour12 - 12;
            else if (hour12 == 0)
                hour12 = 12;

            var min = DateTime.Now.Minute;

            var hour = Convert.ToString((Hour24) ? hour24 : hour12, 2);
            var minute = Convert.ToString(min, 2);
            var second = Convert.ToString(DateTime.Now.Second, 2);

            _corsairRgb.ColorAll(_gKeys, System.Drawing.Color.Black);

            if (AmPmIndicatior)
                Color(1, '1', (isAm) ? System.Drawing.Color.Yellow : System.Drawing.Color.Blue);



            //hour
            for (var i = hour.Length; i > 0; i--)
            {
                Color(6 + i - hour.Length, hour[i - 1], System.Drawing.Color.Red);
            }

            //minute
            for (var i = minute.Length; i > 0; i--)
            {
                Color(12 + i - minute.Length, minute[i - 1], System.Drawing.Color.GreenYellow);
            }

            //second
            for (var i = second.Length; i > 0; i--)
            {
                Color(18 + i - second.Length, second[i - 1], System.Drawing.Color.Cyan);
            }

            _corsairRgb.Update();

            //Mouse Update
            var colorMap = ColorUtil.GetColorMap("jet");
            var curValue = (hour24) * 60 + (min + 1);
            var clIndx = Convert.ToInt32((Convert.ToDouble(curValue) / TOTAL_TIME) * (colorMap.Length - 1));
        }

        private void Color(int gKey, char bit, Color color)
        {
            _gKeys[gKey - 1].Color = bit.Equals('1') ? color : System.Drawing.Color.Black;
        }
    }
}
