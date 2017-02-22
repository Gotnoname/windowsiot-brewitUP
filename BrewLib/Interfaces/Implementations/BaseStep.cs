using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BrewLib.Objects;
using BrewLib.Profile;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Diagnostics;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace BrewLib.Interfaces.Implementations
{
    public abstract class BaseStep : BrewBase, IStep
    {
        private int _elapsedMinutes;
        private int _elapsedSeconds;
        private bool _finished;
        private int _lengthMinutes;
        private int _progressPercent;
        private double _temperature;
        private StepType _type;
        private string _title;
        private double _amount;
        private ObservableCollection<IStep> _subSteps = new ObservableCollection<IStep>();

        public ObservableCollection<IStep> SubSteps
        {
            get
            {
                return _subSteps;
            }

            set
            {
                _subSteps = value;
                OnPropertyChanged("SubSteps");
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public int ElapsedMinutes
        {
            get
            {
                return _elapsedMinutes;
            }

            set
            {
                _elapsedMinutes = value;
                OnPropertyChanged("ElapsedMinutes");
            }
        }

        public int ElapsedSeconds
        {
            get
            {
                return _elapsedSeconds;
            }

            set
            {
                _elapsedSeconds = value;
                OnPropertyChanged("ElapsedSeconds");
            }
        }

        public bool Finished
        {
            get
            {
                return _finished;
            }

            set
            {
                _finished = value;
                OnPropertyChanged("Finished");
            }
        }

        public bool HasStarted
        {
            get
            {
                return ElapsedSeconds > 0;
            }
        }

        public int LengthMinutes
        {
            get
            {
                return _lengthMinutes;
            }

            set
            {
                _lengthMinutes = value;
                OnPropertyChanged("LengthMinutes");
            }
        }

        public int ProgressPercent
        {
            get
            {
                return _progressPercent;
            }

            set
            {
                _progressPercent = value;
                OnPropertyChanged("ProgressPercent");
            }
        }

        public double Temperature
        {
            get
            {
                return _temperature;
            }

            set
            {
                _temperature = value;
                OnPropertyChanged("Temperature");
            }
        }

        public StepType Type
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }

        public double Amount
        {
            get
            {
                return _amount;
            }

            set
            {
                _amount = value; OnPropertyChanged("Amount");
            }
        }

        public virtual async Task RunTaskAsync(CancellationToken cancelToken)
        {            
            var timer = new BrewTimer();
            timer.Start(ElapsedSeconds);

            while (!Finished)
            {
                if (SubSteps.Any())
                {
                    var step = SubSteps.FirstOrDefault(s => ElapsedSeconds >= (s.LengthMinutes * 60) && !s.Finished);
                    if (step != null)
                    {
                        step.RunTaskAsync(cancelToken);
                        Debug.WriteLine($"Sub step completed at {step.LengthMinutes} minutes");
                    }
                }

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        //Gather information on how long the step takes
                        ElapsedSeconds = timer.ElapsedSeconds;
                        ElapsedMinutes = timer.ElapsedMinutes;
                    });

                await Task.Delay(100);
            }
        }

        public virtual void StopTask()
        {
            
        }

        public virtual void WriteJson(JsonWriter writer)
        {
            writer.WritePropertyName("Title");
            writer.WriteValue(Title);
            writer.WritePropertyName("LengthMinutes");
            writer.WriteValue(LengthMinutes);
            writer.WritePropertyName("ElapsedMinutes");
            writer.WriteValue(ElapsedMinutes);
            writer.WritePropertyName("Temperature");
            writer.WriteValue(Temperature);
            writer.WritePropertyName("Type");
            writer.WriteValue(Type);

            foreach(var step in SubSteps)
            {
                step.WriteJson(writer);
            }
        }
    }
}
