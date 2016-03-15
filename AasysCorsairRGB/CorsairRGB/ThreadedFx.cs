using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AasysCorsairRGB.Effects
{
    abstract class ThreadedFx:IRunnableFx
    {
        private Thread _thread;

        public bool Running { get; private set; } = false;
        
        public void Start()
        {
            if (Running)
                return;
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

        protected abstract void Update();

        public void Stop()
        {
            if (!Running)
                return;
            _thread.Suspend();
            Running = false;
        }
    }
}
