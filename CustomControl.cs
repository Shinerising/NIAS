using System;
using System.Windows;
using System.Windows.Controls;
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
                    Random random = new Random();
                    double beginTime = 500 + 100 * Grid.GetRow(element) + 100 * random.NextDouble();
                    element.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(300))) { BeginTime = TimeSpan.FromMilliseconds(beginTime) });

                    TranslateTransform transform = new TranslateTransform(0, 0);
                    transform.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(12, 0, new Duration(TimeSpan.FromMilliseconds(300))) { BeginTime = TimeSpan.FromMilliseconds(beginTime) });
                    element.RenderTransform = transform;
                };
            }
        }
    }
}
