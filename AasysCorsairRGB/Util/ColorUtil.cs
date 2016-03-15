using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AasysCorsairRGB
{
    static class ColorUtil
    {
        private static readonly int MAX_COLOR = 255;
        private static readonly char COLORMAP_DELIMITER = '\n';

        private static Dictionary<string, Color[]> _loadedColorMaps = new Dictionary<string, Color[]>(); 

        private static readonly Dictionary<String, Byte[]> _colorMapsBytes = new Dictionary<string, Byte[]>()
        {
            {"autumn", Properties.Resources.autumn},
            {"bone", Properties.Resources.bone},
            {"colorcube", Properties.Resources.colorcube},
            {"cool", Properties.Resources.cool},
            {"copper", Properties.Resources.copper},
            {"flag", Properties.Resources.flag},
            {"gray", Properties.Resources.gray},
            {"hot", Properties.Resources.hot},
            {"hsv", Properties.Resources.hsv},
            {"jet", Properties.Resources.jet},
            {"lines", Properties.Resources.lines},
            {"parula", Properties.Resources.parula},
            {"pink", Properties.Resources.pink},
            {"prism", Properties.Resources.prism},
            {"spring", Properties.Resources.spring},
            {"summer", Properties.Resources.summer},
            {"winter", Properties.Resources.winter}
        };

        public static Dictionary<string, byte[]>.KeyCollection GetKnownColorMaps()
        {
            return _colorMapsBytes.Keys;
        }

        public static Color[] GetColorMap(string colorMapName)
        {
            if (_loadedColorMaps.ContainsKey(colorMapName))
                return _loadedColorMaps[colorMapName];
            else if (_colorMapsBytes.ContainsKey(colorMapName))
                return LoadColorMap(colorMapName);
            else
                return null;
        }

        private static Color[] LoadColorMap(string colorMapName)
        {
            if (_colorMapsBytes.ContainsKey(colorMapName))
            {
                Color[] cmap = ParseColors(System.Text.Encoding.UTF8.GetString(_colorMapsBytes[colorMapName]));
                _loadedColorMaps.Add(colorMapName, cmap);
                return cmap;
            }
            return null;
        }

        public static Color[] LoadColorMap(string colorMapName, string chexFilePath)
        {
            string allText = File.ReadAllText(chexFilePath);
            Color[] cmap = ParseColors(allText);
            _loadedColorMaps.Add(colorMapName, cmap);
            return cmap;
        }

        private static Color[] ParseColors(String hexColors)
        {
            hexColors = hexColors.Trim();
            String[] hexStrings = hexColors.Split(COLORMAP_DELIMITER);
            Color[] colormap = new Color[hexStrings.Length];

            for (int i = 0; i < hexStrings.Length; i++)
            {
                var colorFromHex = GetColorFromHex(hexStrings[i]);
                if (colorFromHex != null) colormap[i] = colorFromHex.Value;
            }
            return colormap;
        }

        public static Color GetRandomColor(String cMapName)
        {
            return GetRandomColor(GetColorMap(cMapName));
        }

        public static Color GetRandomColor(Color[] colors)
        {
            return colors[CommonUtil.Random.Next(colors.Length)];
        }

        public static Color GetRandomColor()
        {
            return GetRandomColor(false);
        }

        public static Color GetRandomColor(bool randomAlpha)
        {
            return Color.FromArgb((randomAlpha)?  MAX_COLOR : CommonUtil.Random.Next(MAX_COLOR),
                CommonUtil.Random.Next(MAX_COLOR),
                CommonUtil.Random.Next(MAX_COLOR),
                CommonUtil.Random.Next(MAX_COLOR));
        }

        /// <summary>
        /// Get random color while setting some parameters to fixed value</summary>
        /// <remarks>
        /// set input to null to randomize parameter or set 8-bit value for fixing it to a value</remarks>
        public static Color GetRandomColor(int? alpha, int? red, int? green, int? blue)
        {
            return Color.FromArgb(alpha ?? CommonUtil.Random.Next(MAX_COLOR),
                red ?? CommonUtil.Random.Next(MAX_COLOR),
                green ?? CommonUtil.Random.Next(MAX_COLOR),
                blue ?? CommonUtil.Random.Next(MAX_COLOR));
        }

        public static Color? GetColorFromHex(string hexcolor)
        {
            hexcolor = hexcolor.Trim();
            try
            {
                if (hexcolor.Length == 7)
                {
                    return Color.FromArgb(MAX_COLOR,
                        Convert.ToInt32(hexcolor.Substring(1, 2), 16),
                        Convert.ToInt32(hexcolor.Substring(3, 2), 16),
                        Convert.ToInt32(hexcolor.Substring(5, 2), 16));
                }
                else if (hexcolor.Length == 9)
                {
                    return Color.FromArgb(Convert.ToInt32(hexcolor.Substring(1, 2), 16),
                        Convert.ToInt32(hexcolor.Substring(3, 2), 16),
                        Convert.ToInt32(hexcolor.Substring(5, 2), 16),
                        Convert.ToInt32(hexcolor.Substring(7, 2), 16));
                }
            }
            catch (Exception)
            {
                //couldn't parse or create color
                return null;
            }
            return null;
        }


    }
}
