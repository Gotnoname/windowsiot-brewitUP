using BrewitUP.Keyboard;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BrewitUP.Controls
{
    public sealed partial class SimpleVirtualKeyboard : UserControl
    {
        private List<VirtualKey> _keys = new List<VirtualKey>();

        public static DependencyProperty ReferenceTextBoxProperty =
           DependencyProperty.RegisterAttached("ReferenceTextBox", typeof(TextBox), typeof(SimpleVirtualKeyboard), new PropertyMetadata(null));
        public static DependencyProperty IsNumericOnlyProperty =
           DependencyProperty.RegisterAttached("IsNumericOnly", typeof(bool), typeof(SimpleVirtualKeyboard), new PropertyMetadata(false));

        public TextBox ReferenceTextBox
        {
            get
            {
                return (TextBox)base.GetValue(ReferenceTextBoxProperty);
            }

            set
            {
                base.SetValue(ReferenceTextBoxProperty, value);
            }
        }

        public bool IsNumericOnly
        {
            get
            {
                return (bool)base.GetValue(IsNumericOnlyProperty);
            }

            set
            {
                base.SetValue(IsNumericOnlyProperty, value);
            }
        }

        public List<VirtualKey> Keys
        {
            get
            {
                return _keys;
            }

            set
            {
                _keys = value;
            }
        }

        public SimpleVirtualKeyboard()
        {
            this.InitializeComponent();
            this.DataContext = this;

            Loaded += SimpleVirtualKeyboard_Loaded;
        }

        private void SimpleVirtualKeyboard_Loaded(object sender, RoutedEventArgs e)
        {
            List<VirtualKey> row1 = new List<VirtualKey>();
            List<VirtualKey> row2 = new List<VirtualKey>();
            List<VirtualKey> row3 = new List<VirtualKey>();
            List<VirtualKey> row4 = new List<VirtualKey>();
            List<VirtualKey> row5 = new List<VirtualKey>();

            StackPanel panel1 = new StackPanel();            
            panel1.SetValue(Grid.RowProperty, 0);
            panel1.Margin = new Thickness(28, 0, 0, 0);

            StackPanel panel2 = new StackPanel();
            panel2.SetValue(Grid.RowProperty, 1);
            panel2.Margin = new Thickness(28 * 2, 0, 0, 0);

            StackPanel panel3 = new StackPanel();
            panel3.SetValue(Grid.RowProperty, 2);
            panel3.Margin = new Thickness(28 * 3, 0, 0, 0);

            StackPanel panel4 = new StackPanel();
            panel4.SetValue(Grid.RowProperty, 3);
            //panel4.Margin = new Thickness(28 * 4, 0, 0, 0);

            StackPanel panel5 = new StackPanel();
            panel5.SetValue(Grid.RowProperty, 4);
            panel5.HorizontalAlignment = HorizontalAlignment.Center;

            if (IsNumericOnly)
            {
                panel1.Margin = new Thickness(28, 0, 0, 0);
                panel2.Margin = new Thickness(28, 0, 0, 0);
                panel3.Margin = new Thickness(28, 0, 0, 0);
                panel4.Margin = new Thickness(28, 0, 0, 0);

                row1.Add(new VirtualKey(0, 0, Windows.System.VirtualKey.Number1, new List<string> { "1" }));
                row1.Add(new VirtualKey(0, 1, Windows.System.VirtualKey.Number2, new List<string> { "2" }));
                row1.Add(new VirtualKey(0, 2, Windows.System.VirtualKey.Number3, new List<string> { "3" }));

                row2.Add(new VirtualKey(0, 0, Windows.System.VirtualKey.Number4, new List<string> {"4" }));
                row2.Add(new VirtualKey(0, 1, Windows.System.VirtualKey.Number5, new List<string> { "5" }));
                row2.Add(new VirtualKey(0, 2, Windows.System.VirtualKey.Number6, new List<string> { "6" }));

                row3.Add(new VirtualKey(0, 0, Windows.System.VirtualKey.Number7, new List<string> { "7" }));
                row3.Add(new VirtualKey(0, 1, Windows.System.VirtualKey.Number8, new List<string> {  "8" }));
                row3.Add(new VirtualKey(0, 2, Windows.System.VirtualKey.Number9, new List<string> { "9" }));

                row4.Add(new VirtualKey(0, 0, Windows.System.VirtualKey.Number0, new List<string> { "0" }));
                row4.Add(new VirtualKey(0, 1, Windows.System.VirtualKey.Stop, new List<string> { "." }));
                row4.Add(new VirtualKey(0, 2, Windows.System.VirtualKey.Back, new List<string> { "BACKSPACE" }) { SpecialWidth = 2.4 });

                ResizeGrid(row1, panel1);
                ResizeGrid(row2, panel2);
                ResizeGrid(row3, panel3);
                ResizeGrid(row4, panel4);

                KeyGrid.Children.Add(panel1);
                KeyGrid.Children.Add(panel2);
                KeyGrid.Children.Add(panel3);
                KeyGrid.Children.Add(panel4);
            }
            else
            {

                row1.Add(new VirtualKey(0, 0, Windows.System.VirtualKey.Number1, new List<string> { "!", "1" }));
                row1.Add(new VirtualKey(0, 1, Windows.System.VirtualKey.Number2, new List<string> { "\"", "2" }));
                row1.Add(new VirtualKey(0, 2, Windows.System.VirtualKey.Number3, new List<string> { "#", "3" }));
                row1.Add(new VirtualKey(0, 3, Windows.System.VirtualKey.Number4, new List<string> { "¤", "4" }));
                row1.Add(new VirtualKey(0, 4, Windows.System.VirtualKey.Number5, new List<string> { "%", "5" }));
                row1.Add(new VirtualKey(0, 5, Windows.System.VirtualKey.Number6, new List<string> { "&", "6" }));
                row1.Add(new VirtualKey(0, 6, Windows.System.VirtualKey.Number7, new List<string> { "/", "7" }));
                row1.Add(new VirtualKey(0, 7, Windows.System.VirtualKey.Number8, new List<string> { "(", "8" }));
                row1.Add(new VirtualKey(0, 8, Windows.System.VirtualKey.Number9, new List<string> { ")", "9" }));
                row1.Add(new VirtualKey(0, 9, Windows.System.VirtualKey.Number0, new List<string> { "=", "0" }));
                row1.Add(new VirtualKey(0, 10, Windows.System.VirtualKey.Back, new List<string> { "BACKSPACE" }) { SpecialWidth = 2.4 });
                ResizeGrid(row1, panel1);

                row2.Add(new VirtualKey(1, 0, Windows.System.VirtualKey.Q, new List<string> { "Q" }));
                row2.Add(new VirtualKey(1, 1, Windows.System.VirtualKey.W, new List<string> { "W" }));
                row2.Add(new VirtualKey(1, 2, Windows.System.VirtualKey.E, new List<string> { "E" }));
                row2.Add(new VirtualKey(1, 3, Windows.System.VirtualKey.R, new List<string> { "R" }));
                row2.Add(new VirtualKey(1, 4, Windows.System.VirtualKey.T, new List<string> { "T" }));
                row2.Add(new VirtualKey(1, 5, Windows.System.VirtualKey.Y, new List<string> { "Y" }));
                row2.Add(new VirtualKey(1, 6, Windows.System.VirtualKey.U, new List<string> { "U" }));
                row2.Add(new VirtualKey(1, 7, Windows.System.VirtualKey.I, new List<string> { "I" }));
                row2.Add(new VirtualKey(1, 8, Windows.System.VirtualKey.O, new List<string> { "O" }));
                row2.Add(new VirtualKey(1, 9, Windows.System.VirtualKey.P, new List<string> { "P" }));
                ResizeGrid(row2, panel2);

                row3.Add(new VirtualKey(2, 0, Windows.System.VirtualKey.A, new List<string> { "A" }));
                row3.Add(new VirtualKey(2, 1, Windows.System.VirtualKey.S, new List<string> { "S" }));
                row3.Add(new VirtualKey(2, 2, Windows.System.VirtualKey.D, new List<string> { "D" }));
                row3.Add(new VirtualKey(2, 3, Windows.System.VirtualKey.F, new List<string> { "F" }));
                row3.Add(new VirtualKey(2, 4, Windows.System.VirtualKey.G, new List<string> { "G" }));
                row3.Add(new VirtualKey(2, 5, Windows.System.VirtualKey.H, new List<string> { "H" }));
                row3.Add(new VirtualKey(2, 6, Windows.System.VirtualKey.J, new List<string> { "J" }));
                row3.Add(new VirtualKey(2, 7, Windows.System.VirtualKey.K, new List<string> { "K" }));
                row3.Add(new VirtualKey(2, 8, Windows.System.VirtualKey.L, new List<string> { "L" }));
                ResizeGrid(row3, panel3);

                row4.Add(new VirtualKey(3, 0, Windows.System.VirtualKey.LeftShift, new List<string> { "SHIFT" }) { Width = 50 * 2.4 });
                row4.Add(new VirtualKey(3, 1, Windows.System.VirtualKey.Z, new List<string> { "Z" }));
                row4.Add(new VirtualKey(3, 2, Windows.System.VirtualKey.X, new List<string> { "X" }));
                row4.Add(new VirtualKey(3, 3, Windows.System.VirtualKey.C, new List<string> { "C" }));
                row4.Add(new VirtualKey(3, 4, Windows.System.VirtualKey.V, new List<string> { "V" }));
                row4.Add(new VirtualKey(3, 5, Windows.System.VirtualKey.B, new List<string> { "B" }));
                row4.Add(new VirtualKey(3, 6, Windows.System.VirtualKey.N, new List<string> { "N" }));
                row4.Add(new VirtualKey(3, 7, Windows.System.VirtualKey.M, new List<string> { "M" }));
                row4.Add(new VirtualKey(3, 8, Windows.System.VirtualKey.Decimal, new List<string> { ";", "," }));
                row4.Add(new VirtualKey(3, 9, Windows.System.VirtualKey.Stop, new List<string> { ":", "." }));
                row4.Add(new VirtualKey(3, 10, Windows.System.VirtualKey.Subtract, new List<string> { "_", "-" }));
                ResizeGrid(row4, panel4);

                row5.Add(new VirtualKey(4, 0, Windows.System.VirtualKey.Space, new List<string> { "SPACE" }) { SpecialWidth = 5 });
                ResizeGrid(row5, panel5);

                KeyGrid.Children.Add(panel1);
                KeyGrid.Children.Add(panel2);
                KeyGrid.Children.Add(panel3);
                KeyGrid.Children.Add(panel4);
                KeyGrid.Children.Add(panel5);
            }


            foreach (Button button in FindVisualChildren<Button>(this))
            {
                button.Click += LetterButtonClick;
            }
        }

        private void ResizeGrid(List<VirtualKey> keys, StackPanel panel)
        {
            if (keys == null)
            {
                //Reset();
                return;
            }

            Grid grid = new Grid();
            foreach (var key in keys)
                grid.Children.Add(key);


            // Make sure there's the right number of rows
            var rowCount = keys.Max(x => x.GridRow) + 1;
            for (var rowsToAdd = rowCount - grid.RowDefinitions.Count; rowsToAdd > 0; rowsToAdd--)
            {
                // Add the extra Row
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            }
            for (var rowsToRemove = grid.RowDefinitions.Count - rowCount; rowsToRemove > 0; rowsToRemove--)
            {
                // Remove the extra Row
                grid.RowDefinitions.RemoveAt(0);
            }

            // Make sure there's the right number of cols
            var colCount = keys.Max(x => x.GridColumn) + 1;
            for (var colsToAdd = colCount - grid.ColumnDefinitions.Count; colsToAdd > 0; colsToAdd--)
            {
                // Add the extra Column
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            }
            for (var colsToRemove = grid.ColumnDefinitions.Count - colCount; colsToRemove > 0; colsToRemove--)
            {
                // Remove the extra Column
                grid.ColumnDefinitions.RemoveAt(0);
            }

            panel.Children.Add(grid);
        }


        private bool _leftShiftEnabled = false;
        private VirtualKey _leftShuftKey;
        private Brush _leftShiftPreviousColor;

        private void LetterButtonClick(object sender, RoutedEventArgs e)
        {
            VirtualKey key = sender as VirtualKey;
            if(key == null || ReferenceTextBox == null)
            {
                return;
            }

            string letter = key.GetKey();
            int length = ReferenceTextBox.Text.Length;

            //Special case
            if (key.Key == Windows.System.VirtualKey.Back)
            {
                if (length > 0)
                {
                    ReferenceTextBox.Text = ReferenceTextBox.Text.Remove(length - 1, 1);
                }
            }
            else if(key.Key == Windows.System.VirtualKey.Space)
            {
                ReferenceTextBox.Text += " ";
            }
            else if (key.Key == Windows.System.VirtualKey.LeftShift)
            {
                if(!_leftShiftEnabled)
                {
                    _leftShuftKey = key;
                    _leftShiftPreviousColor = _leftShuftKey.Background;
                    _leftShuftKey.Background = new SolidColorBrush(Colors.LightBlue);
                    _leftShiftEnabled = true;
                }
                else
                {
                    _leftShuftKey.Background = _leftShiftPreviousColor;
                    _leftShiftEnabled = false;
                }
                return;
            }
            else
            {
                //Always start as lower
                letter = letter.ToLower();

                if(_leftShiftEnabled)
                {
                    if(key.HasShiftKey)
                    {
                        letter = key.GetShiftKey();
                    }
                    else
                    {
                        letter = letter.ToUpper();
                    }
                }

                ReferenceTextBox.Text += letter;
            }

            if(_leftShuftKey != null && 
               _leftShiftPreviousColor != null)
            {
                _leftShuftKey.Background = _leftShiftPreviousColor;
            }
            _leftShiftEnabled = false;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

    }
}
