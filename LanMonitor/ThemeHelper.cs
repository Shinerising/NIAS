using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace LanMonitor
{
    public class ThemeHelper
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwAttribute, ref int pvAttribute, int cbAttribute);

        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        public static extern bool ShouldSystemUseDarkMode();

        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#132")]
        public static extern bool ShouldAppsUseDarkMode();

        [Flags]
        public enum DwmWindowAttribute : uint
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_MICA_EFFECT = 1029,
            DWMWA_SYSTEMBACKDROP_TYPE = 38
        }

        public enum DwmWindowType : int
        {
            DWMSBT_AUTO = 0,
            DWMSBT_DISABLE = 1, // None
            DWMSBT_MAINWINDOW = 2, // Mica
            DWMSBT_TRANSIENTWINDOW = 3, // Acrylic
            DWMSBT_TABBEDWINDOW = 4 // Tabbed
        }

        public static void ApplyTheme(MainWindow window)
        {
            window.ContentRendered += Window_ContentRendered;
        }

        private static void Window_ContentRendered(object sender, EventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(sender as Window).Handle;
            UpdateStyleAttributes(handle);
            ApplyThemeDetector(handle, WndProc);

        }
        private static void ApplyThemeDetector(IntPtr handle, HwndSourceHook handler)
        {
            HwndSource source = HwndSource.FromHwnd(handle);
            source.AddHook(handler);
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WN_SETTINGCHANGE = 0x001A;
            switch (msg)
            {
                case WN_SETTINGCHANGE:
                    UpdateDarkMode(hwnd);
                    break;
                default:
                    break;
            }

            return IntPtr.Zero;
        }

        private static void UpdateDarkMode(IntPtr hwnd)
        {
            bool useLightTheme = true;
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        useLightTheme = (int)key.GetValue("AppsUseLightTheme") == 1;
                    }
                }
            }
            catch
            {

            }

            UpdateWindowColor(useLightTheme);

            int trueValue = 0x01;
            int falseValue = 0x00;

            if (useLightTheme)
            {
                DwmSetWindowAttribute(hwnd, DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE, ref falseValue, Marshal.SizeOf(typeof(int)));
            }
            else
            {
                DwmSetWindowAttribute(hwnd, DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE, ref trueValue, Marshal.SizeOf(typeof(int)));
            }
        }

        private static void UpdateWindowColor(bool useLightTheme)
        {
            string name = "LanMonitor;component/DarkMode.xaml";
            var resources = Application.Current.Resources.MergedDictionaries;
            bool isDarkModeUsed = resources.FirstOrDefault(item => item.Source.OriginalString.Equals(name)) != null;
            if (useLightTheme && isDarkModeUsed)
            {
                resources.Remove(resources.FirstOrDefault(item => item.Source.OriginalString.Equals(name)));
            }
            else if (!useLightTheme && !isDarkModeUsed)
            {
                resources.Add(new ResourceDictionary() {  Source = new Uri(name, UriKind.Relative) });
            }
        }

        private static void UpdateStyleAttributes(IntPtr hwnd)
        {
            int trueValue = 0x01;
            int dwmType = (int)DwmWindowType.DWMSBT_MAINWINDOW;

            UpdateDarkMode(hwnd);

            DwmSetWindowAttribute(hwnd, DwmWindowAttribute.DWMWA_MICA_EFFECT, ref trueValue, Marshal.SizeOf(typeof(int)));
            DwmSetWindowAttribute(hwnd, DwmWindowAttribute.DWMWA_SYSTEMBACKDROP_TYPE, ref dwmType, Marshal.SizeOf(typeof(int)));
        }
    }
}
