using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AasysCorsairRGB
{
    public interface IRunnableFx
    {
        bool Running { get;}
        void Start();
        void Stop();
    }
}
