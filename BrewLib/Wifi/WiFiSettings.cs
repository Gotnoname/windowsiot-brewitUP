using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.WiFi;

namespace BrewLib.WiFi
{
    public class WiFiSettings
    {
        #region Singleton
        private static WiFiSettings _instance = null;
        public static WiFiSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new WiFiSettings();
                return _instance;
            }
        }
        #endregion

        private bool _hasLoaded = false;
        private readonly string _settingsFilePath;
        private const string SETTINGS_FILE_NAME = "brewWiFiSettings.dat";
        
        public string Password { get; private set; }
        public string DeviceId { get; private set; }
        public string Bssid { get; private set; }
        public string Ssid { get; private set; }
        public WiFiReconnectionKind ReconnectionKind { get; private set; }

        public WiFiSettings()
        {
            _settingsFilePath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, SETTINGS_FILE_NAME);
        }

        public void Save(WiFiReconnectionKind reconnectionKind, WiFiNetworkDisplay network, string deviceId, string password)
        {
            try
            {
                using (var writer = new BinaryWriter(new FileStream(_settingsFilePath, FileMode.Create)))
                {
                    writer.Write((byte)reconnectionKind);
                    writer.Write(password);
                    writer.Write(deviceId);
                    writer.Write(network.AvailableNetwork.Bssid);
                    writer.Write(network.AvailableNetwork.Ssid);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not save WiFiSettings");
            }
        }

        public async Task<WiFiAdapter> Load()
        {
            if (!_hasLoaded)
            {
                _hasLoaded = true;
                if (!File.Exists(_settingsFilePath))
                    return null;

                try
                {
                    using (var reader = new BinaryReader(new FileStream(_settingsFilePath, FileMode.Open)))
                    {
                        ReconnectionKind = (WiFiReconnectionKind)reader.ReadByte();
                        Password = reader.ReadString();
                        DeviceId = reader.ReadString();
                        Bssid = reader.ReadString();
                        Ssid = reader.ReadString();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Could not load WiFiSettings");
                    return null;
                }
            }
            return await WiFiConnector.Instance.GetAdapter(DeviceId); ;
        }
    }
}
