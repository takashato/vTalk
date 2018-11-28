using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace vTalkClient
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        public String ServerIP
        {
            get
            {
                return tbIP.Text;
            }

            set
            {
                tbIP.Text = value;
            }
        }
        public String Account
        {
            get
            {
                return tbAccount.Text;
            }

            set
            {
                tbAccount.Text = value;
            }
        }
        public String Password
        {
            get
            {
                return tbPassword.Text;
            }

            set
            {
                tbPassword.Text = value;
            }
        }

        public LoginScreen()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            btnLogin.IsEnabled = false;
            btnLogin.Content = "Connecting...";

            IPAddress address;
            if(!IPAddress.TryParse(ServerIP, out address))
            {
                MessageBox.Show("Server IP is not valid.", "Login");
                ResetLoginButton();
                return;
            }

            if(Account == "")
            {
                MessageBox.Show("Account can not be empty.", "Login");
                ResetLoginButton();
                return;
            }

            Client.Instance.Address = address;
            if(await Client.Instance.Connect())
            {
                btnLogin.Content = "Login...";
            }
            else
            {
                MessageBox.Show("Can't connect to server. Please try again.", "Login");
                ResetLoginButton();
            }
        }

        private void ResetLoginButton()
        {
            btnLogin.IsEnabled = true;
            btnLogin.Content = "LOGIN"; 
        }
    }
}
