﻿using Microsoft.Win32;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace LanMonitor
{
    public partial class ThemeHelper
    {
        [LibraryImport("dwmapi.dll")]
        public static partial int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwAttribute, ref int pvAttribute, int cbAttribute);

        [LibraryImport("UXTheme.dll", EntryPoint = "#138", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool ShouldSystemUseDarkMode();

        [LibraryImport("UXTheme.dll", EntryPoint = "#132", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool ShouldAppsUseDarkMode();

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
        [SupportedOSPlatform("windows")]
        public static void ApplyTheme(Window window)
        {
            window.ContentRendered += Window_ContentRendered;
        }
        [SupportedOSPlatform("windows")]
        public static void ApplySimpleTheme(Window window)
        {
            if (MainWindow.IsMicaEnabled)
            {
                window.Background = Brushes.Transparent;
            }
            window.ContentRendered += Window_SimpleContentRendered;
        }
        [SupportedOSPlatform("windows")]
        private static void Window_ContentRendered(object sender, EventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(sender as Window).Handle;
            UpdateStyleAttributes(handle);
            ApplyThemeDetector(handle, WndProc);
        }
        [SupportedOSPlatform("windows")]
        private static void Window_SimpleContentRendered(object sender, EventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(sender as Window).Handle;
            UpdateSimpleStyleAttributes(handle);
            ApplyThemeDetector(handle, WndProc);
        }
        private static void ApplyThemeDetector(IntPtr handle, HwndSourceHook handler)
        {
            HwndSource source = HwndSource.FromHwnd(handle);
            source.AddHook(handler);
        }
        [SupportedOSPlatform("windows")]
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

        [SupportedOSPlatform("windows")]
        private static void UpdateDarkMode(IntPtr hwnd)
        {
            bool useLightTheme = true;
            try
            {
                using RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                useLightTheme = (int)key?.GetValue("AppsUseLightTheme") == 1;
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
        [SupportedOSPlatform("windows")]
        private static void UpdateStyleAttributes(IntPtr hwnd)
        {
            int trueValue = 0x01;
            int dwmType = (int)DwmWindowType.DWMSBT_MAINWINDOW;

            UpdateDarkMode(hwnd);

            DwmSetWindowAttribute(hwnd, DwmWindowAttribute.DWMWA_MICA_EFFECT, ref trueValue, Marshal.SizeOf(typeof(int)));
            DwmSetWindowAttribute(hwnd, DwmWindowAttribute.DWMWA_SYSTEMBACKDROP_TYPE, ref dwmType, Marshal.SizeOf(typeof(int)));
        }
        [SupportedOSPlatform("windows")]
        private static void UpdateSimpleStyleAttributes(IntPtr hwnd)
        {
            int trueValue = 0x01;
            int dwmType = (int)DwmWindowType.DWMSBT_MAINWINDOW;

            UpdateDarkMode(hwnd);

            DwmSetWindowAttribute(hwnd, DwmWindowAttribute.DWMWA_MICA_EFFECT, ref trueValue, Marshal.SizeOf(typeof(int)));
            DwmSetWindowAttribute(hwnd, DwmWindowAttribute.DWMWA_SYSTEMBACKDROP_TYPE, ref dwmType, Marshal.SizeOf(typeof(int)));
        }
    }
}
