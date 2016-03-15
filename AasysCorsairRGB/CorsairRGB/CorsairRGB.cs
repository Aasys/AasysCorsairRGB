using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CUE.NET;
using CUE.NET.Devices.Generic;
using CUE.NET.Devices.Generic.Enums;
using CUE.NET.Devices.Headset;
using CUE.NET.Devices.Keyboard;
using CUE.NET.Devices.Mouse;

namespace AasysCorsairRGB
{
    public class CorsairRgb
    {
        private static CorsairRgb _INSTANCE = null;
        public static CorsairRgb INSTANCE => _INSTANCE ?? (_INSTANCE = new CorsairRgb());

        private static readonly Dictionary<string, string> MODEL_MAPS = new Dictionary<string, string>()
        {
            {"K95 RGB", Properties.Resources.K95KeyMap}
        };

        private CorsairKeyboard _keyboard;
        private CorsairMouse _mouse;
        private CorsairHeadset _headset;

        private Dictionary<String, CorsairLed> _keyboardLedMapString;
        private Dictionary<int, CorsairLed> _keyboardLedMapInt; 
        private Dictionary<int, CorsairLed> _mouseLedMap;
        private Dictionary<int, CorsairLed> _headsetLedMap;

        private CorsairLed[,] _keyBoardLedMap;
        
        private CorsairRgb()
        {
            CueSDK.Initialize();
            Intialize();
        }

        private void Intialize()
        {
            _keyboard = CueSDK.KeyboardSDK;
            _mouse = CueSDK.MouseSDK;
            _headset = CueSDK.HeadsetSDK;
            _keyboardLedMapString = _keyboard?.Keys.ToDictionary(key => key.KeyId.ToString(), key => key.Led);
            _keyboardLedMapInt = _keyboard?.Keys.ToDictionary(key => key.KeyId.GetHashCode(), key => key.Led);

            //map mouse keys
            _mouseLedMap = new Dictionary<int, CorsairLed>();

            int mouseIndex = 0;
            if (_mouse != null)
                foreach (var corsairLed in _mouse.Leds)
                {
                    _mouseLedMap.Add(++mouseIndex, corsairLed);
                }

            int headsetIndex = 0;
            if (_headset != null)
                foreach (var corsairLed in _headset.Leds)
                {
                    _headsetLedMap.Add(++headsetIndex, corsairLed);
                }
        }

        public void Reinitalize()
        {
            Intialize();
        }

        public void ReinitializeSdk()
        {
            CueSDK.Reinitialize();
        }

        public void Update()
        {
            _keyboard?.Update();
            _mouse?.Update();
        }

        public CorsairLed GetKeyboardLed(int keyHash)
        {
            return _keyboardLedMapInt[keyHash];
        }

        public CorsairLed GetKeyboardLed(string key)
        {
            return _keyboardLedMapString[key];
        }

        public CorsairLed[] GetKeyboardLeds(int[] keyHashes)
        {
            var leds = new CorsairLed[keyHashes.Length];
            for (int i = 0; i < keyHashes.Length; i++)
            {
                leds[i] = GetKeyboardLed(keyHashes[i]);
            }
            return leds;
        }

        public CorsairLed[] GetKeyboardLeds()
        {
            return _keyboardLedMapInt?.Values?.ToArray();
        }

        public CorsairLed[] GetKeyboardLeds(string[] keyHashes)
        {
            var leds = new CorsairLed[keyHashes.Length];
            for (int i = 0; i < keyHashes.Length; i++)
            {
                leds[i] = GetKeyboardLed(keyHashes[i]);
            }
            return leds;
        }

        public CorsairLed[,] GetkeyboardLedMap()
        {
            if (_keyBoardLedMap != null)
            {
                return _keyBoardLedMap;
            }

            var mapText = MODEL_MAPS[_keyboard.KeyboardDeviceInfo.Model];
            var lines = mapText.Trim().Split('\n');
            var y = lines.Length;
            var x = lines[0].Trim().Split(',').Length;

            _keyBoardLedMap = new CorsairLed[y, x];

            var splitLines = new List<String[]>();
            foreach (var line in lines)
            {
                splitLines.Add(line.Trim().Split(','));
            }

            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    try
                    {
                        var cell = splitLines.ElementAt(i)[j];
                        if (!string.IsNullOrEmpty(cell))
                        {
                            _keyBoardLedMap[i, j] = GetKeyboardLed(cell.Trim());
                        }
                    }
                    catch (Exception)
                    {
                    }
                    
                }
            }
            return _keyBoardLedMap;
        }

        public CorsairLed GetMouseLed(int keyHash)
        {
            return _mouseLedMap[keyHash];
        }

        public CorsairLed[] GetMouseLeds(int[] keyHashes)
        {
            var leds = new CorsairLed[keyHashes.Length];
            for (int i = 0; i < keyHashes.Length; i++)
            {
                leds[i] = GetMouseLed(keyHashes[i]);
            }
            return leds;
        }

        public CorsairLed[] GetMouseLeds()
        {
            return _mouseLedMap?.Values?.ToArray();
        }

        public CorsairLed GetHeadsetLed(int keyHash)
        {
            return _headsetLedMap[keyHash];
        }

        public CorsairLed[] GetHeadsetLeds(int[] keyHashes)
        {
            var leds = new CorsairLed[keyHashes.Length];
            for (int i = 0; i < keyHashes.Length; i++)
            {
                leds[i] = GetHeadsetLed(keyHashes[i]);
            }
            return leds;
        }

        public CorsairLed[] GetHeadsetLeds()
        {
            return _headsetLedMap?.Values?.ToArray();
        }

        public CorsairLed[] GetAllLeds()
        {
            CorsairLed[] allLeds = GetKeyboardLeds();
            if (allLeds != null)
            { 
                CorsairLed[] mouseLeds = GetMouseLeds();
                if (mouseLeds != null)
                {
                    allLeds = allLeds.Concat(mouseLeds).ToArray();
                }
            }
            else
            {
                return null;
            }
            return allLeds;
        }

        public void ColorAll(CorsairLed[] leds, Color color)
        {
            Parallel.ForEach(leds, new Action<CorsairLed>(led => led.Color = color));
        }
    }
}
