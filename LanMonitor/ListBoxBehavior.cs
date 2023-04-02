using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LanMonitor
{
    public class ListBoxBehavior
    {
        public bool ScrollOnNewItem { get; set; }

        public static bool GetScrollOnNewItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ScrollOnNewItemProperty);
        }

        public static void SetScrollOnNewItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ScrollOnNewItemProperty, value);
        }

        public static readonly DependencyProperty ScrollOnNewItemProperty =
            DependencyProperty.RegisterAttached(
            "ScrollOnNewItem",
            typeof(bool),
            typeof(ListBoxBehavior),
            new UIPropertyMetadata(false, OnScrollOnNewItemChanged));

        public static void OnScrollOnNewItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender == null)
            {
                return;
            }
            bool newValue = (bool)e.NewValue;
            if (newValue)
            {
                ((ListBox)sender).Loaded += ListBox_Loaded;
            }
        }

        private static void ListBox_Loaded(object sender, RoutedEventArgs e)
        {
            new ListBoxScroll(sender as ListBox);
        }

        private class ListBoxScroll
        {
            private readonly ListBox listBox;
            private ScrollViewer scrollViewer;

            public ListBoxScroll(ListBox listBox)
            {
                if (listBox != null && listBox.ItemsSource != null)
                {
                    this.listBox = listBox;
                    if (listBox.ItemsSource is INotifyCollectionChanged changed)
                    {
                        changed.CollectionChanged += CollectionChanged;
                    }
                }
            }

            private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Add && !listBox.IsMouseOver)
                {
                    if (scrollViewer == null)
                    {
                        if (VisualTreeHelper.GetChildrenCount(listBox) > 0)
                        {
                            DependencyObject obj = VisualTreeHelper.GetChild(listBox, 0);
                            scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(obj, 0);
                        }
                    }
                    scrollViewer?.ScrollToBottom();
                }
            }
        }
    }

}
