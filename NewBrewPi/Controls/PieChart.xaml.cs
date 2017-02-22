using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NewBrewPi.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PieChart : UserControl, INotifyPropertyChanged
    {
        public static DependencyProperty PietextProperty =
           DependencyProperty.RegisterAttached("Pietext", typeof(string), typeof(PieChart), new PropertyMetadata(null));
        public static DependencyProperty MaxValueProperty =
           DependencyProperty.RegisterAttached("MaxValue", typeof(int), typeof(PieChart), new PropertyMetadata(100));
        public static DependencyProperty MinValueProperty =
           DependencyProperty.RegisterAttached("MinValue", typeof(int), typeof(PieChart), new PropertyMetadata(0));
        public static DependencyProperty ChartColorProperty =
           DependencyProperty.RegisterAttached("ChartColor", typeof(SolidColorBrush), typeof(PieChart), new PropertyMetadata(Colors.CornflowerBlue));
        public static DependencyProperty ValueProperty =
           DependencyProperty.RegisterAttached("Value", typeof(double), typeof(PieChart), new PropertyMetadata(0.0));

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

        public int MaxValue
        {
            get
            {
                return (int)base.GetValue(MaxValueProperty);
            }

            set
            {
                base.SetValue(MaxValueProperty, value);
            }
        }

        public int MinValue
        {
            get
            {
                return (int)base.GetValue(MinValueProperty);
            }

            set
            {
                base.SetValue(MinValueProperty, value);
            }
        }

        public SolidColorBrush ChartColor
        {
            get
            {
                return (SolidColorBrush)base.GetValue(ChartColorProperty);
            }

            set
            {
                base.SetValue(ChartColorProperty, value);
            }
        }

        public double Value
        {
            get
            {
                return (double)base.GetValue(ValueProperty);
            }

            set
            {
                base.SetValue(ValueProperty, value);
            }
        }


        public PieChart()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }       

        public async Task RunTest()
        {
            //while(true)
            //{
            //    var v = new Random().NextDouble() * 100.0;
            //    SetValue(v);
            //    await Task.Delay(1000);
            //}
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
    }
}
