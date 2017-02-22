using BrewLib.Interfaces;
using BrewLib.Interfaces.Implementations;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BrewitUP.Controls
{
    public sealed partial class StepInfoControl : UserControl
    {
        public static DependencyProperty BrewStepProperty =
           DependencyProperty.RegisterAttached("BrewStep", typeof(IStep), typeof(StepInfoControl), new PropertyMetadata(default(IStep)));

        public IStep BrewStep
        {
            get
            {
                return (IStep)base.GetValue(BrewStepProperty);
            }

            set
            {
                base.SetValue(BrewStepProperty, value);
            }
        }

        public StepInfoControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
    }
}
