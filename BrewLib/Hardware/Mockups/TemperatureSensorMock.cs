using BrewLib.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace BrewLib.Hardware.Mockups
{
    public enum SensorMockMode
    {
        Increase,
        Decrease,
    }

    public class TemperatureSensorMock : BrewBase, ITemperature
    {
        public const int INCREASE_TIME_INTERVAL = 1000;
        public const int DECREASE_TIME_INTERVAL = 1000 * 3;
        public const int START_TEMPERATURE = 20;
        public const double INCREASE_RATE_TEMPERATURE = 0.5;
        public const double DECREASE_RATE_TEMPERATURE = 0.1;

        private double _currentTemperature = START_TEMPERATURE;
        private SensorMockMode _mode = SensorMockMode.Increase;

        //1: Pretend to be a temp sensor
        //2: Allow temperature to increase in a customized matter (time)
        //3: Allow decreasing of temperature in a way that simulates normal sea level water decreasing

        public double Temperature
        {
            get { return _currentTemperature; }
            set { _currentTemperature = value; OnPropertyChanged("CurrentTemperature"); }
        }

        public void MockMode(SensorMockMode mode)
        {
            _mode = mode;
        }

        public bool IsRunning { get; set; }

        public void Init()
        {
            IsRunning = true;

            Task.Run(async () =>
            {
                while (IsRunning)
                {
                    int delay = 100;
                    if (_mode == SensorMockMode.Increase)
                    {
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () =>
                        {
                            if (Temperature < 100)
                            {
                                Temperature += INCREASE_RATE_TEMPERATURE;
                            }
                        });
                        delay = INCREASE_TIME_INTERVAL;
                    }
                    else
                    {
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () =>
                        {
                            Temperature -= DECREASE_RATE_TEMPERATURE;
                        });
                        delay = DECREASE_TIME_INTERVAL;
                    }

                    await Task.Delay(delay);
                }

            });
        }

        public void Dispose()
        {
            IsRunning = false;
        }
    }
}

