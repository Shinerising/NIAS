using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// OptionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OptionWindow : Window
    {
        public OptionWindow(Window owner, Options options)
        {
            Owner = owner;

            DataContext = options;

            InitializeComponent();

            ThemeHelper.ApplySimpleTheme(this);
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
            collection.Add(new Options.Pair("", ""));
        }

        private void Button_Submit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
