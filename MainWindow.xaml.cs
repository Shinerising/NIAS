using System.Windows;
using System.Windows.Input;

namespace LanMonitor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private NetworkManager networkManager;

        public MainWindow()
        {
            networkManager = new NetworkManager();

            DataContext = networkManager;

            InitializeComponent();

            networkManager.Start();
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
