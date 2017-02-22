using BrewLib;
using NewBrewPi.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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

namespace NewBrewPi.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DelayedStart : INotifyPropertyChanged
    {
        private BrewProfile _profile;
        private Flyout _flyout;
        private SimpleVirtualKeyboard _keyboard;
        private CancellationToken _cancelToken;
        private CancellationTokenSource _tokenSource;
        private int _hoursDelayed;
        private int _minutesDelayed;
        private string _delayCountdown;

        public string DelayCountdown
        {
            get { return _delayCountdown; }
            set { _delayCountdown = value; OnPropertyChanged("DelayCountdown"); }
        }

        public int MinutesDelayed
        {
            get { return _minutesDelayed; }
            set { _minutesDelayed = value; OnPropertyChanged("MinutesDelayed"); }
        }

        public int HoursDelayed
        {
            get { return _hoursDelayed; }
            set { _hoursDelayed = value; OnPropertyChanged("HoursDelayed"); }
        }

        public DelayedStart()
        {
            this.InitializeComponent();
            this.DataContext = this;

            _flyout = new Flyout();
            _flyout.Placement = FlyoutPlacementMode.Bottom;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _profile = e.Parameter as BrewProfile;
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

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopButton.IsEnabled = false;
            StartButton.IsEnabled = true;

            _tokenSource.Cancel();
            DelayCountdown = "00:00:00";
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StopButton.IsEnabled = true;
            StartButton.IsEnabled = false;

            _tokenSource = new CancellationTokenSource();
            _cancelToken = _tokenSource.Token;

            Task.Run(StartTask);
        }

        private async Task StartTask()
        {
            var zeroTime = new TimeSpan(0, 0, 0);

            try
            {
                long start = StartupTimer.Instance.Millis() / 1000;
                while (true)
                {
                    _cancelToken.ThrowIfCancellationRequested();

                    long now = StartupTimer.Instance.Millis() / 1000;
                    int left = ((HoursDelayed * 60 * 60) + (MinutesDelayed * 60)) - (int)(now - start);

                    var ts = new TimeSpan(0, 0, 0, left);

                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        DelayCountdown = ts.ToString();
                    });

                    if (ts <= zeroTime)
                    {
                        _profile.DelayedStart = true;
                        break;
                    }

                    await Task.Delay(1000);
                }

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        this.Frame.Navigate(typeof(Brew), _profile);
                    });
            }
            catch (OperationCanceledException)
            {
               
            }
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
