using BrewLib;
using BrewitUP.Controls;
using System;
using System.Collections.Generic;
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
    public sealed partial class Settings : Page
    {
        #region Privates
        private Flyout _flyout;
        private SimpleVirtualKeyboard _keyboard;
        #endregion

        public Settings()
        {
            this.InitializeComponent();

            _flyout = new Flyout();
            _flyout.Placement = FlyoutPlacementMode.Bottom;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (BrewProfileSettings.Instance.Verify())
            {
                BrewProfileSettings.Instance.Save();                
            }

            base.OnNavigatingFrom(e);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
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
    }
}
