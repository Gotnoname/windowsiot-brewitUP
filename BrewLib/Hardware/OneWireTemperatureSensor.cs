using BrewLib.Profile;
using Rinsen.IoT.OneWire;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrewLib.Hardware
{
    class OneWireTemperatureSensor : BrewBase, ITemperature
    {
        #region Private variables
        private double _currentTemperature;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _cancelToken;
        private bool _hasInitialized = false;
        private bool _hasDetectedDevices = false;
        private bool _hasShownError = false;
        private const int TEMPERATURE_SANITY = 2;
        #endregion

        #region Constants
        public const int TEMPERATURE_PULL_INTERVAL = 500;
        #endregion

        public double Temperature
        {
            get { return _currentTemperature; }
            set { _currentTemperature = value; OnPropertyChanged("CurrentTemperature"); }
        }

        public bool IsRunning { get; set; }

        public void Init()
        {
            if (_hasInitialized)
            {
                return;
            }
            IsRunning = true;
            _tokenSource = new CancellationTokenSource();
            _cancelToken = _tokenSource.Token;
            Task.Factory.StartNew(GetTemperature);
            _hasInitialized = true;
        }

        public void Dispose()
        {
            IsRunning = false;
            _tokenSource?.Cancel();
            Temperature = 0.0;
            _hasInitialized = false;
        }

        private async void GetTemperature()
        {
            if (Utilities.IsDesktopComputer())
            {
                return;
            }

            bool firstTime = true;
            while (true)
            {
                try
                {
                    if (_cancelToken.IsCancellationRequested)
                    {
                        _cancelToken.ThrowIfCancellationRequested();
                    }

                    using (var oneWireDeviceHandler = new OneWireDeviceHandler(false, false))
                    {
                        var devices = oneWireDeviceHandler.OneWireDevices.GetDevices<DS18B20>().ToList();
                        if (!devices.Any())
                        {
                            if (_hasDetectedDevices)
                            {
                                //We have had devices detected, so something must have gone wrong!
                                //Device might have been unplugged!
                                Temperature = -1;
                                if (!_hasShownError)
                                {
                                    //await UIMessager.Instance.ShowMessageAndWaitForFeedback(
                                    //    "Temperature device error!",
                                    //    "Temperature sensor may have been plugged out!",
                                    //    UIMessageButtons.OK,
                                    //    UIMessageType.Error);
                                    _hasShownError = true;
                                }
                            }
                        }
                        else
                        {
                            foreach (var device in devices)
                            {
                                var result = device.GetTemperature();

                                if (firstTime)
                                {
                                    if (result > 0)
                                    {
                                        Temperature = result;
                                        firstTime = false;
                                    }
                                    await Task.Delay(TEMPERATURE_PULL_INTERVAL);
                                    continue;
                                }

                                //We should have some  sanity check of the temperature
                                //in case some noise gets in the way. 
                                //If we for example had a temperature of 25, and
                                //then noise ruins the next result and returns 85, then
                                //we should not trust the result, it should instead be discarded.
                                //Temperature cant increase by 2 within 500ms???
                                if ((Math.Abs(result - Temperature) <= TEMPERATURE_SANITY))
                                {
                                    Temperature = result;
                                }
                            }

                            _hasDetectedDevices = true;
                            _hasShownError = false;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    // Insert code to log all exceptions!
                    Debug.WriteLine("Could not get temperature: " + e);
                }

                await Task.Delay(TEMPERATURE_PULL_INTERVAL);
            }
            Temperature = 0;
            _tokenSource.Dispose();
            _tokenSource = null;
        }
    }
}
