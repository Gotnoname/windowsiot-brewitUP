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

namespace BrewitUP.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CircularEnable : UserControl
    {
        public static DependencyProperty PietextProperty =
            DependencyProperty.RegisterAttached("Pietext", typeof(string), typeof(CircularEnable), new PropertyMetadata(null));

        public static DependencyProperty ControlColorProperty =
            DependencyProperty.RegisterAttached("ControlColor", typeof(SolidColorBrush), typeof(CircularEnable), new PropertyMetadata(Colors.CornflowerBlue));

        public static DependencyProperty EnableProperty =
            DependencyProperty.RegisterAttached("Enable", typeof(SolidColorBrush), typeof(CircularEnable), new PropertyMetadata(false));

        public string Pietext
        {
            get
            {
                return (string)base.GetValue(PietextProperty);
            }

            set
            {
                base.SetValue(PietextProperty, value);
            }
        }

        public bool Enable
        {
            get
            {
                return (bool)base.GetValue(EnableProperty);
            }

            set
            {
                base.SetValue(EnableProperty, value);
            }
        }

        public SolidColorBrush ControlColor
        {
            get
            {
                return (SolidColorBrush)base.GetValue(ControlColorProperty);
            }

            set
            {
                base.SetValue(ControlColorProperty, value);
            }
        }

        public CircularEnable()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        private void Ellipse_Tapped(object sender, PointerRoutedEventArgs e)
        {
            if(Enable)
            {
                opacityOFFStoryBoard.Begin();
            }
            else
            {
                opacityONStoryBoard.Begin();
            }

            Enable = !Enable;
            EnableText.Text = Enable == true ? "ON" : "OFF";
        }        
    }
}
