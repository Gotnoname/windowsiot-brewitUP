using BrewLib.WiFi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;

namespace BrewLib
{
    public class Utilities
    {
        public static async Task<string> TryConnectToWireless()
        {
            try
            {
                WiFiAdapter adapter = await WiFiSettings.Instance.Load();
                await WiFiConnector.Instance.Init(adapter);
                if (adapter != null)
                {
                    //We have an adapter saved, try to connect
                    WiFiAvailableNetwork availableNetwork = await WiFiConnector.Instance.GetSpecificNetwork(
                        adapter,
                        WiFiSettings.Instance.Bssid,
                        WiFiSettings.Instance.Ssid);

                    var network = new WiFiNetworkDisplay(availableNetwork, adapter);

                    WiFiConnectionResult result = await WiFiConnector.Instance.ConnectAsync(
                        network,
                        WiFiSettings.Instance.ReconnectionKind,
                        WiFiSettings.Instance.Password);

                    if (result.ConnectionStatus == WiFiConnectionStatus.Success)
                    {
                        Debug.WriteLine("Connect on startup to wireless network: " + network.Ssid);
                        return GetAddresses();
                    }
                    else
                    {
                        Debug.WriteLine("Could not connect to wireless network on startup: " + network.Ssid);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not connect to wireless network on startup: " + e);
            }
            return "Unknown";
        }

        public static string GetAddresses()
        {
            StringBuilder sb = new StringBuilder();
            var hostnames = NetworkInformation.GetHostNames();
            foreach (var hn in hostnames)
            {
                //IanaInterfaceType == 71 => Wifi
                //IanaInterfaceType == 6 => Ethernet (Emulator)
                if (hn.IPInformation != null &&
                (hn.IPInformation.NetworkAdapter.IanaInterfaceType == 71
                || hn.IPInformation.NetworkAdapter.IanaInterfaceType == 6) && 
                   hn.Type == Windows.Networking.HostNameType.Ipv4)
                {
                    string ipAddress = hn.DisplayName;
                    sb.AppendLine(ipAddress);
                }
            }
            return sb.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }

        public static string GetDeviceName()
        {
            var hostNames = NetworkInformation.GetHostNames();
            var localName = hostNames.FirstOrDefault(name => name.DisplayName.Contains(".local"));
            return localName.DisplayName.Replace(".local", "");
        }

        public static string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        public static bool IsDesktopComputer()
        {
            return Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop";
        }
    }
}
