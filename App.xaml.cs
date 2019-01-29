using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

            List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
            foreach (ResourceDictionary dictionary in Current.Resources.MergedDictionaries)
            {
                dictionaryList.Add(dictionary);
            }
            string requestedCulture = string.Format(@"StringResource.{0}.xaml", CultureInfo.CurrentCulture.ToString());
            ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedCulture));
            if (resourceDictionary == null)
            {
                requestedCulture = @"StringResource.xaml";
                resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedCulture));
            }
            if (resourceDictionary != null)
            {
                Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
            
            base.OnStartup(e);
        }
    }
}
