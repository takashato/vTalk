using System;
using System.Collections.Generic;
using System.Linq;
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

namespace vTalkClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LoadingScreen loadingScreen;
        private LoginScreen loginScreen;

        public MainWindow()
        {
            InitializeComponent();
            loadingScreen = new LoadingScreen();
            loginScreen = new LoginScreen();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadingScreen.Owner = this;
            loginScreen.Owner = this;
            loginScreen.ShowDialog();
        }

    }
}
