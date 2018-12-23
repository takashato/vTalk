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

namespace vTalkClient.gui.main
{
    /// <summary>
    /// Interaction logic for MainUserInfo.xaml
    /// </summary>
    public partial class MainUserInfo : UserControl
    {
        public MainUserInfo()
        {
            InitializeComponent();
        }

        public void Update()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                serverIp.Content = ClientWindow.Instance.Client.Host + ":" + ClientWindow.Instance.Client.Port;
                username.Content = ClientWindow.Instance.AccountInfo.Account;
            }));
        }
    }
}
