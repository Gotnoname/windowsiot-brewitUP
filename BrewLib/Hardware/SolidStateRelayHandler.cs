using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace BrewLib.SSR
{
    public enum SSRState
    {
        On,
        Off
    }

    public class SolidStateRelayHandler : IDisposable
    {
        #region Private variables
        private readonly GpioPin _gpioPin;
        private Task _highTimeTask = null;
        private SSRState _state = SSRState.Off;

        #endregion

        #region Properties
        public double TotalHighTimeInSeconds { get; private set; } = 0.0;

        public SSRState State
        {
            private set
            {
                _state = value;

                _highTimeTask?.Wait();
                if (value == SSRState.On)
                {
                    _highTimeTask = Task.Factory.StartNew(async () =>
                    {
                        var sw = new Stopwatch();
                        sw.Start();
                        double totalHighTimeInSecondsTemp = TotalHighTimeInSeconds;
                        while (_state == SSRState.On)
                        {
                            var seconds = (sw.ElapsedMilliseconds / 1000.0);
                            TotalHighTimeInSeconds = totalHighTimeInSecondsTemp + seconds;
                            await Task.Delay(100);
                        }
                        
                        sw.Stop();
                        sw = null;
                        _highTimeTask = null;
                    });
                }
            }
            get { return _state; }
        }

        #endregion

        public SolidStateRelayHandler(int gpioPin)
        {
            if (!Utilities.IsDesktopComputer())
            {
                var gpio = GpioController.GetDefault();
                _gpioPin = gpio.OpenPin(gpioPin);
                _gpioPin.Write(GpioPinValue.Low);
                _gpioPin.SetDriveMode(GpioPinDriveMode.Output);
                Debug.WriteLine("Opening GPIO pin: {0}", gpio);
            }
        }

        public void Switch(SSRState state)
        {
            switch (state)
            {
                case SSRState.On:
                    _gpioPin?.Write(GpioPinValue.High);
                    break;
                case SSRState.Off:
                    _gpioPin?.Write(GpioPinValue.Low);
                    break;
            }
            State = state;
        }

        public void Switch()
        {
            if(State == SSRState.On)
            {
                _gpioPin?.Write(GpioPinValue.Low);
                State = SSRState.Off;
            }
            else
            {
                _gpioPin?.Write(GpioPinValue.High);
                State = SSRState.On;
            }
        }

        public void Dispose()
        {
            _gpioPin?.Write(GpioPinValue.Low);
            _gpioPin?.Dispose();
        }
    }
}
