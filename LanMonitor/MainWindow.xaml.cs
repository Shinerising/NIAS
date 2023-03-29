using NIASReport;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Web;
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
        #region Mouse Tilt Support

        private const int WM_SYSCOMMAND = 0x112;
        private const int WM_MOUSEHWHEEL = 0x020E;

        [SupportedOSPlatform("windows")]
        private IntPtr Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_MOUSEHWHEEL:
                    int tilt = (short)HIWORD(wParam);
                    OnMouseTilt(tilt);
                    return 1;
                case WM_SYSCOMMAND:
                    HandleMenuCommand(SystemMenu.HandleMenuCommand(wParam.ToInt32()));
                    break;
            }
            return IntPtr.Zero;
        }

        private static int HIWORD(IntPtr ptr)
        {
            int val32 = (int)ptr.ToInt64();
            return (val32 >> 16) & 0xFFFF;
        }

        private static void OnMouseTilt(int tilt)
        {
            if (Mouse.DirectlyOver is not UIElement element) return;

            ScrollViewer scrollViewer = element is ScrollViewer viewer ? viewer : FindParent<ScrollViewer>(element);

            if (scrollViewer == null)
                return;

            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + tilt * 0.25);
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            if (parentObject is T parent)
                return parent;
            else
                return FindParent<T>(parentObject);
        }
        #endregion

        private readonly NetworkManager networkManager;

        private readonly ReportManager reportManager;
        [SupportedOSPlatform("windows")]
        public static bool IsMicaEnabled => Environment.OSVersion.Version.Build >= 22000;

        [SupportedOSPlatform("windows")]
        public MainWindow()
        {
            Options options = LoadOptions();

            networkManager = new NetworkManager(options);

            DataContext = networkManager;

            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(60000));

            InitializeComponent();

            ThemeHelper.ApplyTheme(this);

            networkManager.Start();

            reportManager = new ReportManager(options.ReportFolder, "template.html", options.ReportLocation, options.ReportUser, options.ReportTime);
            networkManager.SetManager(reportManager);
            RawDataHelper.SetManager(reportManager);
            _ = reportManager.Initialize();
        }

        private static Options LoadOptions()
        {
            try
            {
                return JsonSerializer.Deserialize<Options>(File.ReadAllText("config.json"));
            }
            catch
            {
                return new Options();
            }
        }

        private static void SaveOptions(Options options)
        {
            try
            {
                string json = JsonSerializer.Serialize(options);
                File.WriteAllText("config.json", json);
            }
            catch
            {
            }
        }

        [SupportedOSPlatform("windows")]
        private void ShowOptionWindow()
        {
            Options options = LoadOptions();
            var result = new OptionWindow(this, options).ShowDialog();
            if (result == true)
            {
                SaveOptions(options);
            }
        }

        [SupportedOSPlatform("windows")]
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source?.AddHook(Hook);

            SystemMenu.ApplyCustomMenuItems(source.Handle);

            CheckIfWindowInScreen();
        }

        private void CheckIfWindowInScreen()
        {
            bool outOfBounds =
                (Left <= SystemParameters.VirtualScreenLeft - ActualWidth) ||
                (Top <= SystemParameters.VirtualScreenTop - ActualHeight) ||
                (SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth <= Left) ||
                (SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight <= Top);

            if (outOfBounds)
            {
                Left = 100;
                Top = 100;
                WindowState = WindowState.Normal;
            }
        }

        [SupportedOSPlatform("windows")]
        private void HandleMenuCommand(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return;
            }
            switch(tag)
            {
                case "Menu_Option":
                    ShowOptionWindow();
                    break;
                case "Menu_Help":
                    break;
                case "Menu_About":
                    {
                        bool? result = new AboutWindow(this).ShowDialog();

                        if (result == true)
                        {
                            FC_Control.Initialize(FC_Area);

                            FC_Control.Start();
                        }
                    }
                    break;
            }
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                networkManager?.Dispose();
            }
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Network_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void ReportIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                reportManager.GenerateReport();
            }
        }

        private void CloseToast_Click(object sender, RoutedEventArgs e)
        {
            networkManager.RemoveToast(((FrameworkElement)sender).DataContext as ToastMessage);
        }

        private void Element_MouseEnter(object sender, MouseEventArgs e)
        {
            (((FrameworkElement)sender).DataContext as IHoverable)?.SetHover(true);
        }

        private void Element_MouseLeave(object sender, MouseEventArgs e)
        {
            (((FrameworkElement)sender).DataContext as IHoverable)?.SetHover(false);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void Network_View_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (button.Tag?.ToString())
            {
                case "View_List":
                    networkManager.SetNetworkView(true, false);
                    break;
                case "View_Graph":
                    networkManager.SetNetworkView(false, true);
                    break;
            }
        }

        private void Button_ReportView_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            ReportFileInfo info = element.DataContext as ReportFileInfo;
            StartProcess("msedge", string.Format("\"{0}\"", info.FullName));
        }
        private void Button_ReportFolder_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            ReportFileInfo info = element.DataContext as ReportFileInfo;
            StartProcess("explorer", string.Format("/select,\"{0}\"", info.FullName));
        }
        private void Button_ReportPrint_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            ReportFileInfo info = element.DataContext as ReportFileInfo;
            StartProcess("msedge", string.Format("--kiosk-printing --no-first-run --disable-print-preview \"file:///{0}?print=true\"", info.FullName));
        }
        private static void StartProcess(string name, string parameters)
        {
            try
            {
                using var process = new Process();
                process.StartInfo = new ProcessStartInfo(name, parameters)
                {
                    UseShellExecute = true
                };
                process.Start();
            }
            catch
            {

            }
        }
    }
}
