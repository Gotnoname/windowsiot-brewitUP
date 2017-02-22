using BrewLib;
using BrewitUP.Views.Profile;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BrewitUP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IncompleteBrewView : Page
    {
        public IncompleteBrewView()
        {
            this.InitializeComponent();
        }

        private void UnSuccessfulBrewContinueClick(object sender, RoutedEventArgs e)
        {
            BrewProfile profile = BrewState.Instance.GetState();
            if (profile != null)
            {
                //Use the delayedStart property to force
                //an automatic start of the brew process
                profile.DelayedStart = true;

                this.Frame.Navigate(typeof(Brew), profile);
            }
            BrewState.Instance.Dispose();
        }

        private void UnSuccessfulBrewAbortClick(object sender, RoutedEventArgs e)
        {
            BrewState.Instance.Dispose();
            this.Frame.Navigate(typeof(SelectProfile));
        }
    }
}
