using BrewLib;
using BrewLib.Databse;
using BrewLib.Profile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BrewitUP.Views.Profile
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectProfile : INotifyPropertyChanged
    {
        private ObservableCollection<BrewProfile> _profiles = new ObservableCollection<BrewProfile>();

        public ObservableCollection<BrewProfile> Profiles
        {
            get { return _profiles; }
            set { _profiles = value; OnPropertyChanged("Profiles"); }
        }

        public SelectProfile()
        {
            this.InitializeComponent();
            this.DataContext = this;

#if DEBUG
            DebugTest();
#endif
        }

        private void DebugTest()
        {
            var prof = BrewProfile.GetTestProfile();
            Profiles.Add(prof);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var profiles = BrewDatabase.Instance.GetProfiles();
            foreach (var profile in profiles)
            {
                Profiles.Add(profile);
            }

            base.OnNavigatedTo(e);
        }

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

        private void Grid_Tapped(object sender, RoutedEventArgs e)
        {
            //This event is used for testing on PCs when debugging
            //if (Utilities.IsDesktopComputer())
            {
                FrameworkElement senderElement = sender as FrameworkElement;

                // If you need the clicked element:
                // Item whichOne = senderElement.DataContext as Item;
                FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
                flyoutBase.ShowAt(senderElement);
            }
        }

        private void StartBrewingFlyoutItem_Tapped(object sender, RoutedEventArgs e)
        {
            var item = (sender as MenuFlyoutItem);
            if(item != null)
            {
                var profile = (item.DataContext as BrewProfile);
                if(profile != null)
                {
                    //Naviagte to the brew frame and start brewing :)
                    this.Frame.Navigate(typeof(Brew), profile);
                }
            }
        }

        private void DeleteProfileFlyoutItem_Tapped(object sender, RoutedEventArgs e)
        {
            var item = (sender as MenuFlyoutItem);
            if (item != null)
            {
                var profile = (item.DataContext as BrewProfile);
                if (profile != null)
                {
                    //Delete the profile
                    Profiles.Remove(profile);
                }
            }
        }

        private void EditProfileFlyoutItem_Tapped(object sender, RoutedEventArgs e)
        {
            var item = (sender as MenuFlyoutItem);
            if (item != null)
            {
                var profile = (item.DataContext as BrewProfile);
                if (profile != null)
                {
                    this.Frame.Navigate(typeof(NewProfile), profile);
                }
            }
        }

        private void DelayedStartBrewingFlyoutItem_Tapped(object sender, RoutedEventArgs e)
        {
            var item = (sender as MenuFlyoutItem);
            if (item != null)
            {
                var profile = (item.DataContext as BrewProfile);
                if (profile != null)
                {
                    this.Frame.Navigate(typeof(DelayedStart), profile);
                }
            }
        }
    }
}
