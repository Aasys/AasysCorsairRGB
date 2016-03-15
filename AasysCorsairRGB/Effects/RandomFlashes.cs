using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using CUE.NET.Devices.Generic;

namespace AasysCorsairRGB
{
    class RandomFlashes:IRunnableFx
    {
        private const int MIN_TIME = 500;
        private const int MAX_TIME = 2000;
        private const int MIN_GROUP = 5;
        private const int MAX_GROUP = 80;
        private const String COLOR_MAP = "hsv";

        private readonly CorsairRgb _corsairRgb;

        private Thread _thread;

        private Color[] _colorMap;
        private CorsairLed[] _corsairLeds;


        public bool Running { get; private set; } = false;

        public RandomFlashes()
        {
            _corsairRgb = CorsairRgb.INSTANCE;
            _colorMap = ColorUtil.GetColorMap(COLOR_MAP);
            _corsairLeds = _corsairRgb.GetAllLeds();
        }

        public void Start()
        {
            if (_thread == null)
            {
                _thread = new Thread(new ThreadStart(Update));
                _thread.Start();
                Running = true;
            }
            else if(!Running)
            {
                _thread.Resume();
                Running = true;
            }
        }

        public void Stop()
        {
            if (_thread == null || !Running)
            {
                return;
            }
            else if (Running)
            {
                _thread.Suspend();
                Running = false;
            }
        }

        private void Update()
        {
            while (true)
            {
                _corsairRgb.ColorAll(GetLedGroup(), CommonUtil.randomPick(_colorMap));
                _corsairRgb.Update();
                Thread.Sleep(CommonUtil.GetRandomInt(MIN_TIME, MAX_TIME));
                _corsairRgb.ColorAll(_corsairLeds, Color.Black);
                /*_corsairRgb.Update();
                Thread.Sleep(CommonUtil.GetRandomInt(MIN_TIME, MAX_TIME));*/
            }
        }

        private CorsairLed[] GetLedGroup()
        {
            int size = CommonUtil.GetRandomInt(MIN_GROUP, MAX_GROUP);
            CorsairLed[] corsairLeds = new CorsairLed[size];
            for (int i = 0; i < size; i++)
            {
                corsairLeds[i] = CommonUtil.randomPick(_corsairLeds);
            }
            return corsairLeds;
        }
    }
}
