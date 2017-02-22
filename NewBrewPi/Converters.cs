using BrewLib;
using BrewLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace BrewitUP
{
    public class TimeRemainingStepConverter : DependencyObject, IValueConverter
    {
        public IStep Step
        {
            get { return (IStep)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step",
                                        typeof(IStep),
                                        typeof(TimeRemainingStepConverter),
                                        new PropertyMetadata(null));

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || Step == null)
            {
                return 0;
            }

            var elapsedSec = (int)value;
            int v = (Step.LengthMinutes * 60) - elapsedSec;
            return new TimeSpan(0, 0, v).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SubStepAnyVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value != null && value is IStep)
            {
                if(((IStep)value).SubSteps.Any())
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class StepToNextSubStepSecondsConverter : DependencyObject, IValueConverter
    {
        public IStep Step
        {
            get { return (IStep)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step",
                                        typeof(IStep),
                                        typeof(StepToNextSubStepSecondsConverter),
                                        new PropertyMetadata(null));

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value == null || Step == null)
            {
                return 0;
            }

            var elapsedSec = (int)value;

            var sorted = Step.SubSteps.OrderBy(i => i.LengthMinutes);
            var s = sorted.FirstOrDefault(i => !i.Finished);

            if(s == null || sorted.All(i => i.Finished))
            {
                return 0;
            }

            int v =  (s.LengthMinutes * 60) - elapsedSec;
            return new TimeSpan(0,0,v);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SolidBrushToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var brush = (SolidColorBrush)value;

            return brush.Color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class NumericToTemperatureStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return string.Format("{0}°", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class StepsToIngredientAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var list = (List<IStep>)value;
            if(list != null)
            {
                int count = 0;
                foreach(var i in list)
                {
                    count += i.SubSteps.Count;
                }
                return count;
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class StepsToMashingStepsAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var list = (List<IStep>)value;
            if (list != null)
            {
                int count = 0;
                foreach (var i in list)
                {
                    if (i.Type == BrewLib.Objects.StepType.Mash)
                    {
                        count++;
                    }
                }
                return count;
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class StepsToBoilingTimeAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var list = (List<IStep>)value;
            if (list != null)
            {
                var step = list.FirstOrDefault(s => s.Type == BrewLib.Objects.StepType.Boil);
                if(step != null)
                {
                    return step.LengthMinutes;
                }
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class UIMessageTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var type = (UIMessageType)value;
            BitmapImage img = null;

            switch (type)
            {
                case UIMessageType.Error:
                    img = new BitmapImage(new Uri("ms-appx://BrewitUP/Assets/delete.png"));
                    break;
                case UIMessageType.Information:
                    img = new BitmapImage(new Uri("ms-appx://BrewitUP/Assets/ExclamationMark.png"));
                    break;
                case UIMessageType.Question:
                    img = new BitmapImage(new Uri("ms-appx://BrewitUP/Assets/QuestionMark.png"));
                    break;
                case UIMessageType.Warning:
                    img = new BitmapImage(new Uri("ms-appx://BrewitUP/Assets/Warning.png"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
