using BrewLib.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NewBrewPi.Controls 
{
    
    public sealed partial class Graph : UserControl, INotifyPropertyChanged
    {
        #region Private
        private ObservableCollection<GraphValue> _items = new ObservableCollection<GraphValue>();
        #endregion

        #region Properties
        public ObservableCollection<GraphValue> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged("Items"); }
        }
        #endregion

        public Graph()
        {
            this.InitializeComponent();
            this.DataContext = this;
            SetupSeries();
        }

        public async void Add(double value, int second)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                    {
                        lock (Items)
                        {
                            if (Items.Count > Constants.TEMPERATE_GRAPH_LIMIT)
                            {
                                Items.RemoveAt(0);
                            }

                            Items.Add(new GraphValue { Value = value, Name = second.ToString() }); 
                        }
                    }); 
        }

        public async Task RunTest()
        {
            int seconds = 0;
            while (true)
            {
                var v = new Random().NextDouble();

                Add(v * 100.0, seconds++);
                await Task.Delay(1000);
            }
        }

        public void Reset()
        {
            Items.Clear();
            (lineChart.Series[0] as AreaSeries).ItemsSource = null;
        }

        private void SetupSeries()
        {
            var series = (AreaSeries)this.lineChart.Series[0];
            series.ItemsSource = Items;

            series.DependentRangeAxis =
                            new LinearAxis
                            {
                                Orientation = AxisOrientation.Y,
                                Foreground = new SolidColorBrush(Colors.White),
                                ShowGridLines = true,
                                Interval = 10,
                                Maximum = 100,
                                Minimum = 0
                            };
            series.IndependentAxis =
                new CategoryAxis
                {
                    Orientation = AxisOrientation.X,
                    Height = 0,
                };
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

    public class GraphValue : INotifyPropertyChanged
    {
        private string _name;
        private double _value;

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }
        public double Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged("Value"); }
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
