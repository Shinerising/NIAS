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
    public class MicaMaterialHelper
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

        public static void Window_ContentRendered(object sender, EventArgs e)
        {
            // Apply Mica brush
            UpdateStyleAttributes(new WindowInteropHelper(sender as Window).Handle);
        }

        public static void UpdateStyleAttributes(IntPtr hwnd)
        {
            bool useLightTheme = false;
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
            {
                if (key != null)
                {
                    useLightTheme = (int)(key.GetValue("AppsUseLightTheme")) == 1;
                }
            }

            int trueValue = 0x01;
            DwmWindowAttribute flag = useLightTheme ? DwmWindowAttribute.DWMWA_MICA_EFFECT : DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE;
            DwmSetWindowAttribute(hwnd, flag, ref trueValue, Marshal.SizeOf(typeof(int)));
        }
    }
}
