using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;
using Rinsen.IoT.OneWire;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using BrewLib.Hardware;
using BrewLib.Hardware.Mockups;

namespace BrewLib.Profile
{
    public class TemperatureController : BrewBase, IDisposable
    {
        #region Singleton
        private static TemperatureController _instance;
        public static TemperatureController Instance => _instance ?? (_instance = new TemperatureController());
        #endregion

        #region Private variables
        
        #endregion

        #region Constants
        public const int TEMPERATURE_PULL_INTERVAL = 500;
        #endregion

        private ITemperature _controller;

        public ITemperature Controller
        {
            get { return _controller; }
            set { _controller = value; OnPropertyChanged("Controller"); }
        }

        public void Init()
        {
            if (Utilities.IsDesktopComputer())
            {
                Controller = new TemperatureSensorMock();
            }
            else
            {
                Controller = new OneWireTemperatureSensor();
            }

            Controller.Init();
        }

        public void Dispose()
        {
            Controller.Dispose();
        }
    }
}
