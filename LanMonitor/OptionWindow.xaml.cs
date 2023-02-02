using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Navigation;

namespace LanMonitor
{
    /// <summary>
    /// OptionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OptionWindow : Window
    {
        [SupportedOSPlatform("windows")]
        public OptionWindow(Window owner, Options options)
        {
            Owner = owner;

            DataContext = options;

            InitializeComponent();

            ThemeHelper.ApplySimpleTheme(this);
        }

        public static bool IsNmapInstalled => ExistsOnPath("nmap.exe");
        public static bool IsNpcapInstalled => ExistsOnPath("C:\\Program Files\\Npcap\\NPFInstall.exe");

        public static bool ExistsOnPath(string fileName)
        {
            return GetFullPath(fileName) != null;
        }

        public static string GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
            {
                return Path.GetFullPath(fileName);
            }

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
            return null;
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            Options.Pair pair = element.DataContext as Options.Pair;
            ObservableCollection<Options.Pair> collection = element.Tag as ObservableCollection<Options.Pair>;
            collection.Remove(pair);
        }
        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            ObservableCollection<Options.Pair> collection = element.DataContext as ObservableCollection<Options.Pair>;
            if (collection.Count > 32)
            {
                return;
            }
            collection.Add(new Options.Pair("Name", ""));
        }

        private void Button_Submit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true});
            e.Handled = true;
        }
    }
}
