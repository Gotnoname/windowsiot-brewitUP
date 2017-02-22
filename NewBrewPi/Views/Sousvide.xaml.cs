using BrewLib;
using BrewLib.Interfaces.Implementations;
using BrewLib.Profile;
using BrewitUP.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BrewitUP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Sousvide : INotifyPropertyChanged
    {
        private Flyout _flyout;
        private SimpleVirtualKeyboard _keyboard;
        private BrewLogic _logic;

        private int _minutes;
        private double _temperature;
        private string _currentTemperature;
        private TimeSpan _timeRemaining;

        public string CurrentTemperature { get { return _currentTemperature; } set { _currentTemperature = value; OnPropertyChanged("CurrentTemperature"); } }
        public double Temperature { get { return _temperature; } set { _temperature = value; OnPropertyChanged("Temperature"); } }
        public int Minutes { get { return _minutes; } set { _minutes = value; OnPropertyChanged("Minutes"); } }
        public TimeSpan TimeRemaining { get { return _timeRemaining; } set { _timeRemaining = value; OnPropertyChanged("TimeRemaining"); } }

        public Sousvide()
        {
            this.InitializeComponent();
            this.DataContext = this;

            _flyout = new Flyout();
            _flyout.Placement = FlyoutPlacementMode.Bottom;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopButton.IsEnabled = false;
            StartButton.IsEnabled = true;

            _logic?.Stop();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StopButton.IsEnabled = true;
            StartButton.IsEnabled = false;

            StartSousVide();
        }

        private void NumBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            var txtbox = (TextBox)senderElement;
            if (txtbox != null)
            {
                SetFlytoutContent(true);
                _keyboard.ReferenceTextBox = txtbox;
                _flyout.ShowAt(senderElement);
            }
        }

        private void StartSousVide()
        {
            var profile = new BrewProfile();
            profile.Steps.Add(new MashingStep
                {
                    Temperature = Temperature,
                    LengthMinutes = Minutes,
                }
            );

            _logic = new BrewLogic(profile);
            _logic.Start();

            Task.Run(async () =>
            {
                int count = Minutes * 60;
                while(_logic.IsRunning)
                {
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        TimeRemaining = new TimeSpan(0, 0, count--);
                        CurrentTemperature = TemperatureController.Instance.Controller.Temperature.ToString();
                    });

                    await Task.Delay(1000);
                }
            });
        }

        private void SetFlytoutContent(bool numeric)
        {
            _keyboard = new SimpleVirtualKeyboard();
            _keyboard.IsNumericOnly = numeric;
            _flyout.Content = _keyboard;

            Style style = new Style { TargetType = typeof(FlyoutPresenter) };
            style.Setters.Add(new Setter(MinWidthProperty, numeric ? 300 : 750));
            style.Setters.Add(new Setter(BackgroundProperty, new SolidColorBrush(Colors.Black) { Opacity = 0.95 }));

            _flyout.FlyoutPresenterStyle = style;
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
