using BrewLib;
using BrewLib.Interfaces;
using BrewLib.Interfaces.Implementations;
using BrewitUP.Controls;
using BrewitUP.Views.Profile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class NewProfile : INotifyPropertyChanged
    {
        #region Privates
        private Flyout _flyout;
        private SimpleVirtualKeyboard _keyboard;
        private double _temperature;
        private int _boilMinutes;
        private string _profileName;
        private string _stepName;
        private int _minutes;
        private ObservableCollection<IStep> _items = new ObservableCollection<IStep>(); 
        #endregion

        #region Properties
        public ObservableCollection<IStep> Items
        {
            get
            {
                return _items;
            }

            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        public string ProfileName
        {
            get { return _profileName; }
            set
            {
                _profileName = value;
                OnPropertyChanged("ProfileName");
            }
        }

        public string StepName
        {
            get { return _stepName; }
            set
            {
                _stepName = value;
                OnPropertyChanged("StepName");
            }
        }

        public int BoilMinutes
        {
            get { return _boilMinutes; }
            set
            {
                _boilMinutes = value;
                OnPropertyChanged("BoilMinutes");
            }
        }

        public double Temperature
        {
            get { return _temperature; }
            set
            {
                _temperature = value;
                OnPropertyChanged("Temperature");
            }
        }

        public int Minutes
        {
            get { return _minutes; }
            set
            {
                _minutes = value;
                OnPropertyChanged("Minutes");
            }
        }
        #endregion

        #region Ctor
        public NewProfile()
        {
            this.InitializeComponent();
            this.DataContext = this;

            _flyout = new Flyout();
            _flyout.Placement = FlyoutPlacementMode.Bottom;
        }
        #endregion

        private void TxtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            var txtbox = (TextBox)senderElement;
            if (txtbox != null)
            {
                SetFlytoutContent(false);
                _keyboard.ReferenceTextBox = txtbox;
                _flyout.ShowAt(senderElement);
            }
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

        private void AddStep_Click(object sender, RoutedEventArgs e)
        {
            IStep step = new MashingStep();
            step.LengthMinutes = Minutes;
            step.Temperature = Temperature;
            Items.Add(step);
        }

        private void AddProfile_Click(object sender, RoutedEventArgs e)
        {
            IStep step = new BoilStep
            {
                LengthMinutes = BoilMinutes,
                Temperature = BrewProfileSettings.Instance.MinimumBoilingTemperature,
            };
            Items.Add(step);

            var profile = new BrewProfile(ProfileName, BoilMinutes, Items.ToList());
            this.Frame.Navigate(typeof(AddIngredients), profile);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var profile = e.Parameter as BrewProfile;
            if(profile == null)
            {
                return;
            }
            ProfileName = profile.Name;
            BoilMinutes = profile.BoilTime;
            
            foreach(var step in profile.Steps)
            {
                Items.Add(step);
            }

            base.OnNavigatedTo(e);
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

        #region Listview Flyout menu
        private void ListViewItemHolding_Tapped(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;

            // If you need the clicked element:
            // Item whichOne = senderElement.DataContext as Item;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }

        private void ListViewItem_Tapped(object sender, RoutedEventArgs e)
        {
            //This event is used for testing on PCs when debugging
            if (Utilities.IsDesktopComputer())
            {
                FrameworkElement senderElement = sender as FrameworkElement;

                // If you need the clicked element:
                // Item whichOne = senderElement.DataContext as Item;
                FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
                flyoutBase.ShowAt(senderElement);
            }
        }

        private void MoveUpFlyoutItem_Tapped(object sender, RoutedEventArgs e)
        {
            var item = (sender as MenuFlyoutItem);
            if (item != null)
            {
                var step = (item.DataContext as IStep);
                if (step != null)
                {
                    var i = Items.IndexOf(step);

                    if (i > 0)
                    {
                        Items.RemoveAt(i);
                        Items.Insert(i - 1, step);
                    }
                }
            }
        }

        private void MoveDownFlyoutItem_Tapped(object sender, RoutedEventArgs e)
        {
            var item = (sender as MenuFlyoutItem);
            if (item != null)
            {
                var step = (item.DataContext as IStep);
                if (step != null)
                {
                    var i = Items.IndexOf(step);

                    if (i < Items.Count - 1)
                    {
                        Items.RemoveAt(i);
                        Items.Insert(i + 1, step);
                    }
                }
            }
        }

        private void DeleteFlyoutItem_Tapped(object sender, RoutedEventArgs e)
        {
            var item = (sender as MenuFlyoutItem);
            if (item != null)
            {
                var step = (item.DataContext as IStep);
                if (step != null)
                {
                    //Delete the profile
                    Items.Remove(step);
                }
            }
        }
        #endregion

        private void CancelProfile_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SelectProfile));
        }
    }
}
