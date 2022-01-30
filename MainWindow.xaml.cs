using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace LanMonitor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private readonly NetworkManager networkManager;

        public MainWindow()
        {
            networkManager = new NetworkManager();

            DataContext = networkManager;

            InitializeComponent();

            networkManager.Start();

            SnmpHelper.Initialize();
            SnmpHelper.FetchData();
        }

        private void WindowMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void WindowMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }
        private void WindowClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
        }

        private void Window_Activated(object sender, EventArgs e)
        {
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            var source = PresentationSource.FromVisual(this);
            ((HwndSource)source)?.AddHook(Hook);
        }

        const int WM_MOUSEHWHEEL = 0x020E;

        private IntPtr Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_MOUSEHWHEEL:
                    int tilt = (short)HIWORD(wParam);
                    OnMouseTilt(tilt);
                    return (IntPtr)1;
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Gets high bits values of the pointer.
        /// </summary>
        private static int HIWORD(IntPtr ptr)
        {
            int val32 = (int)ptr.ToInt64();
            return ((val32 >> 16) & 0xFFFF);
        }

        /// <summary>
        /// Gets low bits values of the pointer.
        /// </summary>
        private static int LOWORD(IntPtr ptr)
        {
            int val32 = (int)ptr.ToInt64();
            return (val32 & 0xFFFF);
        }

        private void OnMouseTilt(int tilt)
        {
            UIElement element = Mouse.DirectlyOver as UIElement;

            if (element == null) return;

            ScrollViewer scrollViewer = element is ScrollViewer viewer ? viewer : FindParent<ScrollViewer>(element);

            if (scrollViewer == null)
                return;

            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + tilt * 0.25);
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (networkManager != null)
                {
                    networkManager.Dispose();
                }
            }
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Network_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
