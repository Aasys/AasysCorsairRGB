using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AasysCorsairRGB
{
    static class CommonUtil
    {
        private static Random _random;

        public static Random Random => _random ?? (_random = new Random());

        public static T randomPick<T>(T[] t)
        {
            return t[_random.Next(0, t.Length)];
        }

        public static T randomPick<T>(List<T> t)
        {
            return t.ElementAt(_random.Next(0, t.Count));
        }

        public static int GetRandomInt(int min, int max)
        {
            return Random.Next(min, max);
        }
    }
}
