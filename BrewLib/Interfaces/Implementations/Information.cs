using System;
using BrewLib.Profile;

namespace BrewLib.Interfaces.Implementations
{
    public class Information : BrewBase, IInformation
    {
        private double _averageTemperature;
        private int _elapsedSeconds;
        private double _energyUsed;
        private string _ipAddress;
        private int _stepProgress;
        private double _temperature;

        public double Temperature
        {
            get { return _temperature; }
            set { _temperature = value; OnPropertyChanged("Temperature"); }
        }

        public double AverageTemperature
        {
            get
            {
                return _averageTemperature;
            }

            set
            {
                _averageTemperature = value;
                OnPropertyChanged("AverageTemperature");
            }
        }

        public int ElapsedMinutes
        {
            get
            {
                return _elapsedSeconds;
            }

            set
            {
                _elapsedSeconds = value;
                OnPropertyChanged("ElapsedMinutes");
                OnPropertyChanged("TimeElapsed");
            }
        }

        public double EnergyUsed
        {
            get
            {
                return _energyUsed;
            }

            set
            {
                _energyUsed = value;
                OnPropertyChanged("EnergyUsed");
            }
        }

        public string IpAddress
        {
            get
            {
                return _ipAddress;
            }

            set
            {
                _ipAddress = value;
                OnPropertyChanged("IpAddress");
            }
        }

        public int StepProgress
        {
            get
            {
                return _stepProgress;
            }

            set
            {
                _stepProgress = value;
                OnPropertyChanged("StepProgress");
            }
        }

        public string TimeElapsed
        {
            get { return new TimeSpan(0, ElapsedMinutes, 0).ToString(@"hh\:mm"); }
        }

        public Information()
        {
            IpAddress = Utilities.GetAddresses();
        }

        public void Reset()
        {
            StepProgress = 0;
            ElapsedMinutes = 0;
            AverageTemperature = 0;
            EnergyUsed = 0;
            Temperature = 0;       
        }
    }
}
