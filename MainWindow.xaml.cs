using System;
using System.Windows;
using System.Windows.Input;

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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
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
