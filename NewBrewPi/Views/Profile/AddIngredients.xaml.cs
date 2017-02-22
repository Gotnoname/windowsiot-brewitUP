using BrewLib;
using BrewLib.Databse;
using BrewLib.Interfaces;
using BrewLib.Interfaces.Implementations;
using NewBrewPi.Controls;
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

namespace NewBrewPi.Views.Profile
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddIngredients : INotifyPropertyChanged
    {
        private ObservableCollection<IStep> _items = new ObservableCollection<IStep>();
        private IStep _selectedStep;
        private string _stepName;
        private int _minutes;
        private double _amount;
        private Flyout _flyout;
        private SimpleVirtualKeyboard _keyboard;
        private BrewProfile _profile;

        public double Amount
        {
            get { return _amount; }
            set { _amount = value; OnPropertyChanged("Amount"); }
        }

        public IStep SelectedStep
        {
            get
            {
                return _selectedStep;
            }

            set
            {
                _selectedStep = value;
                OnPropertyChanged("SelectedStep");
            }
        }

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

        public string StepName
        {
            get { return _stepName; }
            set
            {
                _stepName = value;
                OnPropertyChanged("StepName");
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

        public AddIngredients()
        {
            this.InitializeComponent();
            this.DataContext = this;

            _flyout = new Flyout();
            _flyout.Placement = FlyoutPlacementMode.Bottom;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _profile = (e.Parameter as BrewProfile);
            
            foreach(var step in _profile.Steps)
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

        private void BackProfile_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NewProfile), _profile); //Use navigate instead with parameter? Doesnt seem to remember
        }

        private void FinishProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BrewDatabase.Instance.AddProfile(_profile);                
            }
            catch (Exception ex)
            {
                //await UIMessager.Instance.ShowMessageAndWaitForFeedback("Profile error", "Profile was NOT created. " + ex.Message,
                //    UIMessageButtons.OK, UIMessageType.Information);
            }

            this.Frame.Navigate(typeof(SelectProfile));
        }

        private void AddStep_Click(object sender, RoutedEventArgs e)
        {
            if(SelectedStep != null)
            {
                IStep step = new IngredientStep
                {
                    //Boil length minus "last x min" results in when after boil starts we add ingredient
                    LengthMinutes = _profile.BoilTime - Minutes, 
                    Title = StepName,
                    Amount = Amount,
                };
                SelectedStep.SubSteps.Add(step);
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

        #region ListView flyout
        private void DeleteIngredientFlyoutItem_Tapped(object sender, RoutedEventArgs e)
        {
            var item = (sender as MenuFlyoutItem);
            if (item != null)
            {
                var step = (item.DataContext as IStep);
                if (step != null)
                {
                    //Delete the profile
                    SelectedStep.SubSteps.Remove(step);
                }
            }
        }

        private void ListViewSubItemHolding_Tapped(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;

            // If you need the clicked element:
            // Item whichOne = senderElement.DataContext as Item;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }

        private void ListViewSubItem_Tapped(object sender, RoutedEventArgs e)
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
    }

}
