using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AasysCorsairRGB.Effects
{
    abstract class TimedFx:IRunnableFx
    {
        private const int DEFAULT_INTERVAL = 1000;

        private Timer _timer;
        public bool Running { get; private set; } = false;

        public int Interval
        {
            get { return Interval; }
            set
            {
                _timer?.Stop();
                _timer?.Close();
                _timer = null;
                Interval = value;
            }
        }

        public void Start()
        {
            if (Running)
                return;

            if (_timer == null)
            {
                _timer = new Timer(Interval == null ? DEFAULT_INTERVAL:Interval);
                _timer.Elapsed += new ElapsedEventHandler(Update);
            }
            _timer.Start();
            Running = true;
        }

        internal abstract void Update(object sender, ElapsedEventArgs e);
        

        public void Stop()
        {
            if (!Running)
                return;
            _timer.Stop();
            Running = false;
        }
    }
}
