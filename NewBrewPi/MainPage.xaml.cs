using BrewLib;
using BrewLib.Databse;
using BrewitUP.Views;
using BrewitUP.Views.Profile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BrewitUP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        #region UI Events
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainPage_Loaded;

            if(BrewState.Instance.HasStateFile)
            {
                contentFrame.Navigate(typeof(IncompleteBrewView));
            }
            else
            {
                contentFrame.Navigate(typeof(SelectProfile));
                //contentFrame.Navigate(typeof(Brew), BrewProfile.GetTestProfile());
            }

            StartupTimer.Instance.Init();

            if (!Utilities.IsDesktopComputer())
            {
                //Hide the cursor for the application
                Window.Current.CoreWindow.PointerCursor = null;
            }

            Task.Run(async () =>
            {
                await Utilities.TryConnectToWireless();
            });

            Task.Run(SetSystemTime);
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenuSplitView.IsPaneOpen = !MainMenuSplitView.IsPaneOpen;
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(typeof(SelectProfile));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(typeof(Settings));
        }

        private void AddNewProfile_Click(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(typeof(NewProfile));
        }

        private void WiFiSettings_Click(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(typeof(WiFi));
        }

        private void SouseVide_Click(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(typeof(Sousvide));
        }
        #endregion

        #region Private functions
        private static async Task SetSystemTime()
        {
            DateTime t = DateTime.UtcNow;
            try
            {
                t = await NtpClient.Instance.GetNetworkTimeAsync(5000);
            }
            catch
            {
            }
            finally
            {
                TimeManager.UpdateOffset(t);
            }
            Debug.WriteLine("NTP time received: " + t);
        }
        #endregion        
    }
}
