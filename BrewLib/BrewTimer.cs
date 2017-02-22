using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewLib
{
    public class BrewTimer
    {
        private Stopwatch _sw = new Stopwatch();
        private int _offsetSeconds;


        public int ElapsedSeconds
        {
            get
            {
                return (int)(_sw.Elapsed.TotalSeconds + _offsetSeconds);
            }
        }

        public int ElapsedMinutes
        {
            get
            {
                return ElapsedSeconds / 60;
            }
        }

        public void Start(int offsetSeconds = 0)
        {
            _offsetSeconds = offsetSeconds;
            _sw.Start();
        }

        public void EndReset()
        {
            _sw.Reset();
        }
    }
}
