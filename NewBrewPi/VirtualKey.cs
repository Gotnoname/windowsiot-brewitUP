using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace NewBrewPi.Keyboard
{
    public class VirtualKey : Button
    {
        public int GridRow
        {
            get { return (int)GetValue(Grid.RowProperty); }
            set { SetValue(Grid.RowProperty, value); }
        }

        public int GridColumn
        {
            get { return (int)GetValue(Grid.ColumnProperty); }
            set { SetValue(Grid.ColumnProperty, value); }
        }

        public int GridColumnSpan
        {
            get { return (int)GetValue(Grid.ColumnSpanProperty); }
            set { SetValue(Grid.ColumnSpanProperty, value); }
        }

        public double SpecialWidth
        {
            set
            {
                var v = Width* value;
                var f = Math.Ceiling(value);
                Width = v;
                GridColumnSpan = (int)f;
            }
        }

        public Windows.System.VirtualKey Key { get; set; }
        public Windows.System.VirtualKey ShiftKey { get; set; }
        public bool HasShiftKey
        {
            get
            {
                return Characters.Count > 0;
            }
        }

        public List<string> Characters { get; set; }

        public VirtualKey(int gridrow, int gridcolumn, Windows.System.VirtualKey key, List<string> characters)
        {
            Margin = new Thickness(3);
            BorderBrush = new SolidColorBrush(Colors.Gray);
            Background = new SolidColorBrush(Colors.Black) { Opacity = 0.9};
            Foreground = new SolidColorBrush(Colors.White);
            Width = 50;
            Height = 40;
            GridRow = gridrow;
            GridColumn = gridcolumn;
            Key = key;
            Characters = characters;
            SetContent();
        }

        public string GetShiftKey()
        {
            return Characters.First();
        }

        public string GetKey()
        {
            return Characters.Last();
        }

        private void SetContent()
        {
            Grid g = new Grid();
            g.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            g.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            for (int i = 0; i < Characters.Count; i++)
            {
                var txt = new TextBlock();
                txt.Text = Characters[i];
                txt.SetValue(Grid.RowProperty, i);
                if(Characters.Count > 1 && i == 0)
                {
                    txt.FontSize = 10;
                    txt.Margin = new Thickness(0, -5, 0, 0);
                }
                
                g.Children.Add(txt);
            }
            Content = g;
        }
    }
}
