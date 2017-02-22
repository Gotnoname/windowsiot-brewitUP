using BrewLib.WiFi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.WiFi;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BrewitUP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WiFi : INotifyPropertyChanged
    {
        #region Private variables
        private bool _isAutomaticReconnection = false;
        #endregion

        #region Properties
        public bool IsAutomaticReconnection
        {
            get { return _isAutomaticReconnection; }
            set { _isAutomaticReconnection = value; OnPropertyChanged("IsAutomaticReconnection"); }
        }
        #endregion

        #region Ctor
        public WiFi()
        {
            this.InitializeComponent();
            this.DataContext = this;
            //UIMessager.Instance.Init(UiMessageViewer);
        }
        #endregion

        #region UI events
        private void NetworkKeyBox_GotFocus(object sender, RoutedEventArgs e)
        {
            VirtualKeyboard.ReferenceTextBox = (TextBox)sender;
            VirtualKeyboard.Visibility = Visibility.Visible;
        }

        private async void SearchNetworks_ButtonClick(object sender, RoutedEventArgs e)
        {
            VirtualKeyboard.Visibility = Visibility.Collapsed;
            ResultsListView.IsEnabled = false;
            SearchProgressLoader.IsActive = true;

            await WiFiConnector.Instance.ScanAsync();

            ResultsListView.IsEnabled = true;
            SearchProgressLoader.IsActive = false;
            Debug.WriteLine("Wireless search completed");
        }

        private async void ConnectToNetwork_ButtonClick(object sender, RoutedEventArgs e)
        {
            VirtualKeyboard.Visibility = Visibility.Collapsed;
            var selectedNetwork = ResultsListView.SelectedItem as WiFiNetworkDisplay;
            if (selectedNetwork == null)
            {
                //await UIMessager.Instance.ShowMessageAndWaitForFeedback("Network empty", "Network has not been selected!", UIMessageButtons.OK, UIMessageType.Information);
                return;
            }

            if (string.IsNullOrEmpty(NetworkPassword.Text))
            {

                //await UIMessager.Instance.ShowMessageAndWaitForFeedback("Password empty", "Password cannot be empty!", UIMessageButtons.OK, UIMessageType.Information);
                return;
            }

            WiFiReconnectionKind reconnectionKind = WiFiReconnectionKind.Manual;
            //if (IsAutomaticReconnection) //always reconnect automatically
            {
                reconnectionKind = WiFiReconnectionKind.Automatic;
            }
            WiFiConnectionResult result = await WiFiConnector.Instance.ConnectAsync(selectedNetwork, reconnectionKind, NetworkPassword.Text);
            if (result.ConnectionStatus == WiFiConnectionStatus.Success)
            {
                WiFiSettings.Instance.Save(reconnectionKind, selectedNetwork, WiFiConnector.Instance.DeviceId, NetworkPassword.Text);
                //await UIMessager.Instance.ShowMessageAndWaitForFeedback("WiFi connected successfully!", string.Format("Successfully connected to {0}.", selectedNetwork.Ssid), UIMessageButtons.OK, UIMessageType.Information);
            }
            else
            {
                //await UIMessager.Instance.ShowMessageAndWaitForFeedback("WiFi connection error!",
                //    string.Format("Could not connect to {0}. Error: {1}", selectedNetwork.Ssid, result.ConnectionStatus),
                //    UIMessageButtons.OK, UIMessageType.Error);
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VirtualKeyboard.Visibility = Visibility.Collapsed;
            var selectedNetwork = ResultsListView.SelectedItem as WiFiNetworkDisplay;
            if (selectedNetwork == null)
            {
                return;
            }

            if (selectedNetwork.IsOpenNetwork)
            {
                NetworkPassword.IsEnabled = false;
            }
            else
            {
                NetworkPassword.IsEnabled = true;
            }
            ConnectNetworkButton.IsEnabled = true;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // RequestAccessAsync must have been called at least once by the app before using the API
            // Calling it multiple times is fine but not necessary
            // RequestAccessAsync must be called from the UI thread
            WiFiAccessStatus access = await WiFiConnector.Instance.RequestAccessAsync();
            if (access != WiFiAccessStatus.Allowed)
            {
                //await UIMessager.Instance.ShowMessageAndWaitForFeedback("WiFi Config", "WiFi access is disallowed for this device!", UIMessageButtons.OK, UIMessageType.Information);
            }
            else
            {
                if (!await WiFiConnector.Instance.Init())
                {
                    //await UIMessager.Instance.ShowMessageAndWaitForFeedback("WiFi Config", "No WiFi Adapters detected on this device!", UIMessageButtons.OK, UIMessageType.Information);
                }
            }
        }
        #endregion

        #region Private functions
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
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
