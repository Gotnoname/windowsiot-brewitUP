using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewLib
{
    public class StartupTimer
    {
        #region Singleton
        private static StartupTimer _instance;
        public static StartupTimer Instance => _instance ?? (_instance = new StartupTimer());
        #endregion

        #region Private variables
        private Stopwatch _timer;
        #endregion

        #region Ctor
        public void Init()
        {
            _timer = new Stopwatch();
            _timer.Start();
        }

        /// <summary>
        /// Returns the number of milliseconds since the program started running.
        /// </summary>
        /// <returns></returns>
        public long Millis()
        {
            return _timer.ElapsedMilliseconds;
        }
        #endregion
    }
}
