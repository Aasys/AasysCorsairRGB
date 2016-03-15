using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CUE.NET.Devices.Generic;

namespace AasysCorsairRGB
{
    public class FunctionClockRgb:IRunnableFx
    {
        ///static
        private static string AM_PM_LED_ID = "Escape";

        private static double DAY_IN_SECONDS = 24*60*60D;

        private static string[] HOUR_LEDS_ID = {"F1", "F2", "F3", "F4"}; 
        private static string[] MINUTE_LEDS_ID = {"F5", "F6", "F7", "F8"}; 
        private static string[] SECOND_LEDS_ID = {"F9", "F10", "F11", "F12"};

        private static Color[] COLORS = {Color.Red, Color.LawnGreen, Color.Yellow, Color.Cyan};

        private static int VALUE_SEPRATOR = 15;

        ///readonly property
        private readonly CorsairRgb _corsairRgb ;

        private readonly bool[,] _binaries = new bool[16, 4];

        ///property
        private CorsairLed _amPmLed; 
      
        private CorsairLed[] _hourLeds;
        private CorsairLed[] _minuteLeds;
        private CorsairLed[] _secondLeds;

        private Timer _timer;

        public Color BlankColor { get; set; } = Color.Black;
        public Color AmColor { get; set; } = Color.OrangeRed;
        public Color PmColor { get; set; } = Color.Blue;

        public bool Running { get; private set; } = false;

        public FunctionClockRgb()
        {
            _corsairRgb = CorsairRgb.INSTANCE;

            _amPmLed = _corsairRgb.GetKeyboardLed(AM_PM_LED_ID);

            _hourLeds = _corsairRgb.GetKeyboardLeds(HOUR_LEDS_ID);
            _minuteLeds = _corsairRgb.GetKeyboardLeds(MINUTE_LEDS_ID);
            _secondLeds = _corsairRgb.GetKeyboardLeds(SECOND_LEDS_ID);

            //initialize bools
            for (int i = 0; i < 16; i++)
            {
                var bits = Convert.ToString(i, 2).ToCharArray();

                for (int j = 0; j < 4 - bits.Length; j++)
                    _binaries[i, j] = false;

                for (int j = 0; j < bits.Length; j++)
                    _binaries[i, 4 - bits.Length + j] = (bits[j] == '1') ? true : false;
            }

            _timer = new Timer(1000);
            _timer.Elapsed += new ElapsedEventHandler(Update);
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

        private void Update(object sender, ElapsedEventArgs e)
        {
            var hour24 = DateTime.Now.Hour;
            var isAm = hour24 == 0 || hour24 < 12;

            var hour12 = hour24;
            if (hour24 > 12)
                hour12 = hour12 - 12;
            else if (hour12 == 0)
                hour12 = 12;

            var minute = DateTime.Now.Minute;
            var second = DateTime.Now.Second;

            UpdateTime(hour12, _hourLeds);
            UpdateTime(minute, _minuteLeds);
            UpdateTime(second, _secondLeds);

            //_amPmLed.Color = (isAm) ? AmColor : PmColor;

            var timeInSecond = hour24*60*60 + minute*60 + second;
            var timeColorIndex = Convert.ToInt32(Math.Round((timeInSecond/DAY_IN_SECONDS)*(COLORS.Length - 1)));

            _amPmLed.Color = COLORS[timeColorIndex];

            _corsairRgb.Update();
        }

        private void UpdateTime(int value, CorsairLed[] leds)
        {
            var colorIndex = (value == 0) ? 0 : Convert.ToInt32( Math.Floor((value-1.0D)/VALUE_SEPRATOR) );
            var valueIndex = value % VALUE_SEPRATOR;
            if (value != 0 && valueIndex == 0)
                valueIndex = VALUE_SEPRATOR;

            for (int i = 0; i < 4; i++)
            {
                leds[i].Color = (_binaries[valueIndex, i]) ? COLORS[colorIndex] : BlankColor;
            }
        }

    }
}
