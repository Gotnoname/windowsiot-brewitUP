using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using BrewLib;
using System.ComponentModel;
using BrewLib.Interfaces;
using BrewLib.Interfaces.Implementations;
using BrewLib.Profile;
using BrewLib.Objects;
using BrewLib.PID;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NewBrewPi.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Brew : Page
    {
        public BrewLogic Logic { get; set; }
        public IInformation Info { get; set; }

        public Brew()
        {
            this.InitializeComponent();            
            Loaded += OnLoadedF;
        }

        private void OnLoadedF(object sender, RoutedEventArgs e)
        {
            UIMessager.Instance.Init(UserNotifier);

            //Task.Run(Pie.RunTest);
            //Task.Run(Graph.RunTest);
            //Task.Run(PieStep.RunTest);

            //var profile = BrewProfile.GetTestProfile();

            //Logic = new BrewLogic(profile);
            //Logic.Start();
            //StepsPanel.DataContext = Logic;
        }

        private void CircularEnable_Tapped(object sender, RoutedEventArgs e)
        {
            Logic.Pump();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var profile = e.Parameter as BrewProfile;
            if (profile != null)
            {
                Logic = new BrewLogic(profile);                
                StepsPanel.DataContext = Logic;
                Info = new Information();
                InfoGrid.DataContext = Info;
                PieStep.DataContext = Info;
                Pie.DataContext = Info;

                if(profile.DelayedStart)
                {
                    StartBrewing_Click(null, null);
                }
            }
        }

        //protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        //{
        //    e.Cancel = true;

        //    var res = await UIMessager.Instance.ShowMessageAndWaitForFeedback("Cancel brewing?", 
        //        "Are you sure you want to cancel the brewing process?", 
        //        UIMessageButtons.YesNo, 
        //        UIMessageType.Question);

        //    if(res == UIMessageResults.Yes)
        //    {
        //        e.Cancel = false;                
        //    }
        //}

        private void StartBrewing_Click(object sender, RoutedEventArgs e)
        {
            StartBrewButton.IsEnabled = false;
            StopBrewButton.IsEnabled = true;

            Logic?.Start();

            Task.Run(async () =>
            {
                var avgTemp = new List<double>();
                int seconds = 0;
                IStep prevStep = null;
                while (Logic.IsRunning)
                {
                    try
                    {
                        var temperature = TemperatureController.Instance.Controller.Temperature;
                        Graph.Add(temperature, seconds++);

                        #region Average temperature
                        if (prevStep != Logic.CurrentStep)
                        {
                            //Step has changed, clear out average temp
                            avgTemp.Clear();
                        }
                        prevStep = Logic.CurrentStep;

                        if (avgTemp.Count > Constants.TEMPERATE_GRAPH_LIMIT)
                        {
                            avgTemp.RemoveAt(0);
                        } 
                        #endregion
                        avgTemp.Add(temperature);

                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () =>
                        {
                            Info.AverageTemperature = Math.Round(avgTemp.Average(), 1);

                            //The amount of energy used for this session
                            Info.EnergyUsed = Math.Round((BrewProfileSettings.Instance.HeaterElementWatt * Logic.HeaterOnInHours) / 1000, 2);

                            //Time spent brewing
                            Info.ElapsedMinutes = Logic.TimeBrewingInMinutes;

                            //Total progress of the brewing process
                            Info.StepProgress = Logic.PercentageCompleted;

                            Info.Temperature = Math.Round(temperature, 1);
                        });
                    }
                    catch (Exception)
                    {

                    }

                    await Task.Delay(1000);
                }
            });
        }

        private void StopBrewing_Click(object sender, RoutedEventArgs e)
        {
            Graph.Reset();
            Info?.Reset();
            Logic?.Stop();
            StartBrewButton.IsEnabled = true;
            StopBrewButton.IsEnabled = false;
        }
    }
}
