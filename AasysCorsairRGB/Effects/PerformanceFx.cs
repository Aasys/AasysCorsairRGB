using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CUE.NET.Devices.Generic;

namespace AasysCorsairRGB
{
    class PerformanceFx : IRunnableFx
    {
        private static readonly float MAX_NETWORK_BYTES = 8000000f;
        private static readonly string JET_COLOR_MAP = "jet";
        private static readonly string[] GROUP_1_LEDS_STRS = new string[] { "G16", "G13", "G10", "G7", "G4", "G1" };
        private static readonly string[] GROUP_2_LEDS_STRS = new string[] { "G17", "G14", "G11", "G8", "G5", "G2" };
        private static readonly string[] GROUP_3_LEDS_STRS = new string[] { "G18", "G15", "G12", "G9", "G6", "G3" };
        private static readonly string[] GROUP_4_LEDS_STRS = new string[] { "Delete", "End", "PageDown",
            "Insert", "Home", "PageUp",
            "PrintScreen", "ScrollLock", "PauseBreak"};
        private static readonly string[] GROUP_5_LEDS_STRS = new string[] { "Stop", "ScanPreviousTrack", "PlayPause", "ScanNextTrack" };
        private static readonly string[] GROUP_6_LEDS_STRS = new string[] { "Escape", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8" };



        private CorsairRgb _corsairRgb;

        private CorsairLed[] _LedsGroup1;
        private CorsairLed[] _LedsGroup2;
        private CorsairLed[] _LedsGroup3;
        private CorsairLed[] _LedsGroup4;
        private CorsairLed[] _LedsGroup5;
        private CorsairLed[] _LedsGroup6;

        private Color[] _colors;

        private Timer _timer;

        public bool Running { get; private set; } = false;

        private PerformanceCounter[] _cpuCounters = new PerformanceCounter[]
        {

            new PerformanceCounter("Processor", "% Processor Time", "_Total"),
            new PerformanceCounter("Processor", "% Processor Time", "0"),
            new PerformanceCounter("Processor", "% Processor Time", "1"),
            new PerformanceCounter("Processor", "% Processor Time", "2"),
            new PerformanceCounter("Processor", "% Processor Time", "3"),
            new PerformanceCounter("Processor", "% Processor Time", "4"),
            new PerformanceCounter("Processor", "% Processor Time", "5"),
            new PerformanceCounter("Processor", "% Processor Time", "6"),
            new PerformanceCounter("Processor", "% Processor Time", "7")
        };

        private PerformanceCounter _cpuCounter = new PerformanceCounter()
        {
            CategoryName = "Processor",
            CounterName = "% Processor Time",
            InstanceName = "_Total"
        };

        private PerformanceCounter _ramCounter = new PerformanceCounter()
        {
            CategoryName = "Memory",
            CounterName = "% Committed Bytes In Use"
        };

        private PerformanceCounter _diskCounter = new PerformanceCounter()
        {
            CategoryName = "PhysicalDisk",
            CounterName = "% Idle Time",
            InstanceName = "0 C: D:"
        };

        private PerformanceCounter _netCounter = new PerformanceCounter()
        {
            CategoryName = "Network Interface",
            CounterName = "Bytes Total/sec",
            InstanceName = "Intel[R] Ethernet Connection [2] I219-V"
        };


        public PerformanceFx()
        {
            _corsairRgb = CorsairRgb.INSTANCE;
            _LedsGroup1 = _corsairRgb.GetKeyboardLeds(GROUP_1_LEDS_STRS);
            _LedsGroup2 = _corsairRgb.GetKeyboardLeds(GROUP_2_LEDS_STRS);
            _LedsGroup3 = _corsairRgb.GetKeyboardLeds(GROUP_3_LEDS_STRS);
            _LedsGroup4 = _corsairRgb.GetKeyboardLeds(GROUP_4_LEDS_STRS);
            _LedsGroup5 = _corsairRgb.GetKeyboardLeds(GROUP_5_LEDS_STRS);
            _LedsGroup6 = _corsairRgb.GetKeyboardLeds(GROUP_6_LEDS_STRS);

            _colors = ColorUtil.GetColorMap(JET_COLOR_MAP);

        }

        public void Start()
        {
            if (Running)
                return;

            if (_timer == null)
            {
                _timer = new Timer(500);
                _timer.Elapsed += new ElapsedEventHandler(Update);
            }
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
            UpdateLeds(_cpuCounter.NextValue()/100.0f, _LedsGroup1);
            //UpdateLeds(_ramCounter.NextValue() / 100.0f, _LedsGroup2);
            UpdateLeds(_netCounter.NextValue() / MAX_NETWORK_BYTES, _LedsGroup3);
            UpdateLeds(1.0f - _diskCounter.NextValue() / 100.0f, _LedsGroup2);

            _corsairRgb.Update();
        }

        private void UpdateCpuLeds()
        {
            for (int i = 0; i < _cpuCounters.Length; i++)
            {
                int perColour = Convert.ToInt32(Math.Floor(Convert.ToDouble((_cpuCounters[i].NextValue() / 100.0f)) * _colors.Length));
                _LedsGroup6[i].Color = _colors[perColour];

            }
        }

        private void UpdateLeds(float perUnit, CorsairLed[] leds)
        {
            if (perUnit > 1.0f)
            {
                perUnit = 1.0f;
            }
            else if (perUnit < 0.0f)
            {
                perUnit = 0.0f;
            }
            int numKeys = leds.Length*53;
            int normalized = Convert.ToInt32(Math.Round(Convert.ToDouble(perUnit * numKeys)));
            Color color = _colors[((_colors.Length - 1) * normalized) / numKeys];
            _corsairRgb.ColorAll(leds, Color.Black);
            normalized = (normalized%(leds.Length -1)) + 1;
            for (int i = 0; i < normalized; i++)
            {
                leds[i].Color = color;
            }
        }
    }
}
