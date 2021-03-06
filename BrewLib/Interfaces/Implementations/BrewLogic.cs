﻿using BrewLib.Hardware.Mockups;
using BrewLib.Interfaces;
using BrewLib.Interfaces.Implementations;
using BrewLib.Objects;
using BrewLib.PID;
using BrewLib.Profile;
using BrewLib.SSR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace BrewLib
{
    //Contains the logic of the brewing process.
    //Controls everything based on a brewing profile
    //created by the user.
    public class BrewLogic : BrewBase, IBrewLogic
    {
        #region Privates
        private IStep _currentStep;
        private IStep _nextStep;
        private readonly BrewProfile _profile;
        private CancellationToken _cancelToken;
        private CancellationTokenSource _tokenSource;
        SolidStateRelayHandler _ssrPumpHandler;
        SolidStateRelayHandler _ssrHeaterHandler;
        #endregion

        #region Properties
        public IInformation Info { get; set; } = new Information();

        public bool IsRunning { get; private set; }
        
        public IStep CurrentStep
        {
            get { return _currentStep; }
            set
            {
                _currentStep = value;
                OnPropertyChanged("CurrentStep");
            }
        }

        public IStep NextStep
        {
            get { return _nextStep; }
            set
            {
                _nextStep = value;
                OnPropertyChanged("NextStep");
            }
        }
        #endregion

        #region Ctor
        public BrewLogic(BrewProfile profile)
        {
            _profile = profile;
        }
        #endregion

        #region Public functions
        public void Start()
        {
            //Create the cancellation token to be used for tasks
            _tokenSource = new CancellationTokenSource();
            _cancelToken = _tokenSource.Token;

            _ssrHeaterHandler = new SolidStateRelayHandler(BrewProfileSettings.Instance.SSRHeaterGPIO);

            TemperatureController.Instance.Init();

            IsRunning = true;

            Task.Run(PIDTask);
            Task.Run(StateSaverTask);
            Task.Run(StepTask);
            Task.Run(InformationTask);
        }

        public void Stop()
        {
            IsRunning = false;
            Info?.Reset();
            _tokenSource?.Cancel();
            _ssrPumpHandler?.Dispose();
            _ssrHeaterHandler?.Dispose();
            TemperatureController.Instance.Dispose();
        }

        public void Pump()
        {
            if (_ssrPumpHandler == null)
            {
                _ssrPumpHandler = new SolidStateRelayHandler(BrewProfileSettings.Instance.SSRPumpGPIO);
            }

            _ssrPumpHandler?.Switch();
        }

        public void Pause()
        {
            IsRunning = false;
        }

        public void Dispose()
        {
            Stop();
        }
        #endregion

        #region Private tasks
        private async Task StepTask()
        {
            for (int i = 0; i < _profile.Steps.Count; i++)
            {
                var step = _profile.Steps[i];

                if (step.Finished)
                {
                    //This could occurr if we had an unsuccessful
                    //brew and we are trying to continue the process.
                    continue;
                }

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    CurrentStep = step;

                        //Check if theres a step after this
                        if (_profile.Steps.Count > i + 1)
                    {
                        NextStep = _profile.Steps[i + 1];
                    }
                    else
                    {
                        NextStep = null;
                    }
                });

                await CurrentStep.RunTaskAsync(_cancelToken);

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    OnPropertyChanged("PercentageCompleted");
                });
            }

            Stop();
        }

        private async Task InformationTask()
        {
            var time = new BrewTimer();
            time.Start();

            var avgTemp = new List<double>();
            IStep prevStep = null;

            while (IsRunning)
            {
                try
                {
                    var temperature = TemperatureController.Instance.Controller.Temperature;

                    #region Average temperature
                    if (prevStep != CurrentStep)
                    {
                        //Step has changed, clear out average temp
                        avgTemp.Clear();
                    }
                    prevStep = CurrentStep;

                    if (avgTemp.Count > Constants.TEMPERATE_GRAPH_LIMIT)
                    {
                        avgTemp.RemoveAt(0);
                    }
                    avgTemp.Add(temperature);

                    #endregion

                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () =>
                        {
                            Info.AverageTemperature = Math.Round(avgTemp.Average(), 1);

                            //The amount of energy used for this session
                            Info.EnergyUsed = Math.Round((BrewProfileSettings.Instance.HeaterElementWatt * (_ssrHeaterHandler.TotalHighTimeInSeconds / (60 * 60))) / 1000, 2);

                            //Time spent brewing
                            Info.ElapsedMinutes = time.ElapsedMinutes;

                            //Total progress of the brewing process
                            int total = _profile.Steps.Count;
                            int completed = _profile.Steps.Count(s => s.Finished == true);
                            Info.StepProgress = (int)(((double)completed / (double)total) * 100.0);

                            Info.Temperature = Math.Round(temperature, 1);
                        });
                }
                catch (Exception)
                {
                }
                await Task.Delay(1000);
            }
        }

        private async Task PIDTask()
        {
            var pid = new PIDController(
                (float)BrewProfileSettings.Instance.PIDKp, 
                (float)BrewProfileSettings.Instance.PIDKi, 
                (float)BrewProfileSettings.Instance.PIDKd, 
                100f, 
                0f);

            long previousMillisec = StartupTimer.Instance.Millis();
            while(IsRunning)
            {
                if(CurrentStep == null)
                {
                    await Task.Delay(10);
                    continue;
                }

                bool isBoiling = CurrentStep.Type == Objects.StepType.Boil;

                pid.SetPoint = (float)CurrentStep.Temperature;
                pid.ProcessVariable = (float)TemperatureController.Instance.Controller.Temperature;
                float output = pid.ControlVariable(StartupTimer.Instance.Millis() - previousMillisec);
                previousMillisec = StartupTimer.Instance.Millis();

                //Check the output and see if we need to trigger the heater on or off
                if (output >= 100 || isBoiling)
                {
                    _ssrHeaterHandler.Switch(SSRState.On);
                }
                else
                {
                    _ssrHeaterHandler.Switch(SSRState.Off);                    
                }

                if (Utilities.IsDesktopComputer())
                {
                    (TemperatureController.Instance.Controller as TemperatureSensorMock)?
                                .MockMode(_ssrHeaterHandler.State == SSRState.On ? SensorMockMode.Increase : SensorMockMode.Decrease);
                }

                //Debug.WriteLine($"PID output: {output}, temperature: {pid.SetPoint}");

                await Task.Delay(500);
            }
        }

        private async Task StateSaverTask()
        {
            while (IsRunning)
            {
                BrewState.Instance.WriteState(_profile);
                await Task.Delay(1000);
            }
            BrewState.Instance.Dispose();
        }
        #endregion
    }
}
