using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.WiFi;
using Windows.Security.Credentials;

namespace BrewLib.WiFi
{
    public class WiFiConnector : INotifyPropertyChanged
    {
        #region Singleton
        private static WiFiConnector _instance;
        public static WiFiConnector Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new WiFiConnector();
                return _instance;
            }
        } 
        #endregion

        #region Private variables
        private WiFiAdapter _firstAdapter;
        private DeviceInformationCollection _devices;
        private ObservableCollection<WiFiNetworkDisplay> _networkCollection = new ObservableCollection<WiFiNetworkDisplay>(); 
        #endregion

        #region Properties
        public ObservableCollection<WiFiNetworkDisplay> NetworkCollection
        {
            get { return _networkCollection; }
            set { _networkCollection = value; OnPropertyChanged("NetworkCollection"); }
        }

        public string DeviceId { get; private set; }
        public WiFiReconnectionKind ReconnectionKind { get; private set; } 
        #endregion

        #region Public functions
        public async Task ScanAsync()
        {
            if (_firstAdapter != null)
            {
                await _firstAdapter.ScanAsync();
                DisplayNetworkReport(_firstAdapter.NetworkReport);
            }
        }

        public async Task<WiFiAvailableNetwork> GetSpecificNetwork(WiFiAdapter adapter, string bssid, string ssid)
        {
            await adapter.ScanAsync();
            foreach (var network in adapter.NetworkReport.AvailableNetworks)
            {
                if (network.Bssid.Equals(bssid) && network.Ssid.Equals(ssid))
                    return network;
            }
            return null;
        }

        public async Task<WiFiAccessStatus> RequestAccessAsync()
        {
            return await WiFiAdapter.RequestAccessAsync();
        }

        public async Task<DeviceInformationCollection> GetDevices()
        {
            return await DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());
        }

        public async Task<bool> Init(WiFiAdapter overrideAdapter = null)
        {
            _firstAdapter = overrideAdapter ?? await GetFirstDevice();
            return _firstAdapter != null;
        }

        public async Task<WiFiAdapter> GetFirstDevice()
        {
            if (_devices == null)
                _devices = await GetDevices();

            return _devices.Count == 0 ? null : await GetAdapter(_devices[0].Id);
        }

        public async Task<WiFiAdapter> GetAdapter(string id)
        {
            if (id == null)
            {
                return null;
            }

            DeviceId = id;
            return await WiFiAdapter.FromIdAsync(id);
        }

        public async Task<WiFiConnectionResult> ConnectAsync(WiFiNetworkDisplay network, WiFiReconnectionKind reconnectionKind, string password)
        {
            WiFiConnectionResult result;
            ReconnectionKind = reconnectionKind;
            if (network.AvailableNetwork.SecuritySettings.NetworkAuthenticationType == Windows.Networking.Connectivity.NetworkAuthenticationType.Open80211)
            {
                result = await _firstAdapter.ConnectAsync(network.AvailableNetwork, reconnectionKind);
            }
            else
            {
                // Only the password potion of the credential need to be supplied
                var credential = new PasswordCredential { Password = password };
                result = await _firstAdapter.ConnectAsync(network.AvailableNetwork, reconnectionKind, credential);
            }

            // Since a connection attempt was made, update the connectivity level displayed for each
            foreach (var net in NetworkCollection)
            {
                net.UpdateConnectivityLevel();
            }

            return result;
        }
        #endregion

        #region Private functions
        private void DisplayNetworkReport(WiFiNetworkReport report)
        {
            NetworkCollection.Clear();
            foreach (var network in report.AvailableNetworks)
            {
                NetworkCollection.Add(new WiFiNetworkDisplay(network, _firstAdapter));
            }
        } 
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
