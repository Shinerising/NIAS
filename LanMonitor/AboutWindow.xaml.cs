﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LanMonitor
{
    /// <summary>
    /// AboutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AboutWindow : Window
    {
        public Dictionary<string, string> InfoList { get; set; }
        public string Copyright { get; set; }
        [SupportedOSPlatform("windows")]
        public AboutWindow(Window owner)
        {
            Owner = owner;
            InitializeInformation();

            DataContext = this;

            InitializeComponent();

            ThemeHelper.ApplySimpleTheme(this);
        }
        private void InitializeInformation()
        {
            Assembly currentAssem = Assembly.GetExecutingAssembly();
            string version = currentAssem.GetCustomAttributes<AssemblyFileVersionAttribute>().FirstOrDefault()?.Version;
            string company = currentAssem.GetCustomAttributes<AssemblyCompanyAttribute>().FirstOrDefault()?.Company;
            string author = "Apollo Wayne";
            Copyright = string.Format("© {0} {1} {2}", DateTime.Now.Year, company, AppResource.GetString("About_Copyright"));

            InfoList = new Dictionary<string, string>()
            {
                { AppResource.GetString("About_Version"), "V" + version },
                { AppResource.GetString("About_Author"), author },
                { AppResource.GetString("About_Cert"), "2022SR1382319" }
            };
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            using (Process.Start(e.Uri.ToString()))
            {

            }
        }

        private void Run_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = true;
        }
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
