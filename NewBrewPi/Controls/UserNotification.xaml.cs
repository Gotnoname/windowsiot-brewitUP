using BrewLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BrewitUP.Controls
{
    public sealed partial class UserNotification : UIMessagerUIControl
    {
        private UIMessageButtons _buttons;

        protected override UIMessageButtons Buttons
        {
            get { return _buttons; }
            set
            {
                _buttons = value;

                Reset();
                switch (value)
                {
                    case UIMessageButtons.OK:
                        ButtonPanel.Children.Add(GetButton("OK"));
                        break;
                    case UIMessageButtons.Cancel:
                        ButtonPanel.Children.Add(GetButton("Cancel"));
                        break;
                    case UIMessageButtons.Close:
                        ButtonPanel.Children.Add(GetButton("Close"));
                        break;
                    case UIMessageButtons.OKCancel:
                        ButtonPanel.Children.Add(GetButton("OK"));
                        ButtonPanel.Children.Add(GetButton("Cancel"));
                        break;
                    case UIMessageButtons.Yes:
                        ButtonPanel.Children.Add(GetButton("Yes"));
                        break;
                    case UIMessageButtons.No:
                        ButtonPanel.Children.Add(GetButton("No"));
                        break;
                    case UIMessageButtons.YesNo:
                        ButtonPanel.Children.Add(GetButton("Yes"));
                        ButtonPanel.Children.Add(GetButton("No"));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        public UserNotification()
        {
            this.InitializeComponent();
            DataContext = this; 
        }

        #region UIEvents
        private void AnswerClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn.Content.ToString() == "OK")
            {
                ClickResult = UIMessageResults.OK;
            }
            else if (btn.Content.ToString() == "Cancel")
            {
                ClickResult = UIMessageResults.Cancel;
            }
            else if (btn.Content.ToString() == "Close")
            {
                ClickResult = UIMessageResults.Close;
            }
            else if (btn.Content.ToString() == "Yes")
            {
                ClickResult = UIMessageResults.Yes;
            }
            else if (btn.Content.ToString() == "No")
            {
                ClickResult = UIMessageResults.No;
            }

            CancelMessage();
            Debug.WriteLine("Clicked button: " + ClickResult);
            //Reset();
        }
        #endregion

        private Button GetButton(string content)
        {
            var btn = new Button
            {
                Width = 60,
                Height = 30,
                Content = content,
                Margin = new Thickness(6),
                Background = new SolidColorBrush(Colors.Transparent),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.White),
                Foreground = new SolidColorBrush(Colors.White),
            };
            btn.Click += AnswerClick;
            return btn;
        }

        protected override void Reset()
        {
            base.Reset();
            foreach (var child in ButtonPanel.Children)
            {
                (child as Button).Click -= AnswerClick;
            }
            ButtonPanel.Children.Clear();
        }
    }
}
