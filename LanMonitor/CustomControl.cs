using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LanMonitor
{
    internal class CustomControl
    {
        public static bool GetFadeInOnLoad(DependencyObject obj)
        {
            return (bool)obj.GetValue(FadeInOnLoadProperty);
        }

        public static void SetFadeInOnLoad(DependencyObject obj, bool value)
        {
            obj.SetValue(FadeInOnLoadProperty, value);
        }

        public static readonly DependencyProperty FadeInOnLoadProperty = DependencyProperty.RegisterAttached(
                "FadeInOnLoad",
                typeof(bool),
                typeof(CustomControl),
                new UIPropertyMetadata(false, FadeInOnLoadChanged));

        public static void FadeInOnLoadChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender == null)
            {
                return;
            }
            bool newValue = (bool)e.NewValue;
            if (newValue)
            {
                FrameworkElement element = sender as FrameworkElement;
                element.Opacity = 0;

                ((FrameworkElement)sender).Loaded += (object _sender, RoutedEventArgs _e) =>
                {
                    Random random = new();
                    double beginTime = 500 + 100 * Grid.GetRow(element) + 100 * random.NextDouble();
                    element.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(300))) { BeginTime = TimeSpan.FromMilliseconds(beginTime) });

                    TranslateTransform transform = new(0, 0);
                    transform.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(12, 0, new Duration(TimeSpan.FromMilliseconds(300))) { BeginTime = TimeSpan.FromMilliseconds(beginTime) });
                    element.RenderTransform = transform;
                };
            }
        }
    }

    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format("{0:00}:{1:00}", (int)value / 60, (int)value % 60);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString().Split(':').Take(2).Select(int.Parse).Aggregate(0, (acc, cur) => acc * 60 + cur);
        }
    }

    public partial class TimeValueFieldValidation : ValidationRule
    {
        [GeneratedRegex(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):([0-5][0-9]|[0-9])$")]
        private static partial Regex TimeRegex();
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (TimeRegex().IsMatch((string)value))
                return ValidationResult.ValidResult;
            else
                return new ValidationResult(false, "123");
        }
    }
}
