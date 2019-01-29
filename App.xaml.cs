using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace LanMonitor
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Process[] processList = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            if (processList.Length > 1)
            {
                foreach (Process process in processList)
                {
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        NativeMethods.SetForegroundWindow(process.MainWindowHandle);
                    }
                }
                Environment.Exit(0);
                return;
            }
        }
    }
}
