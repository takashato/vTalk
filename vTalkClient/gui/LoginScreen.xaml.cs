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
using vTalkClient.tools;

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
                if (imgTogglePassword.Tag.ToString() == "Hide")
                    return tbPassword.Text;
                else
                    return pwbPassword.Password;
            }
            set
            {
                pwbPassword.Password = tbPassword.Text = value;
            }
        }

        public LoginScreen()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            btnLogin.IsEnabled = false;
            btnLogin.Content = "Đang kết nối...";

            LoginStatus status = await ClientWindow.Instance.Connect(ServerIP);

            if(status == LoginStatus.CantConnectToServer)
            {
                MessageBox.Show("Không thể kết nối tới server. Vui lòng thử lại.", "Đăng nhập");
                ResetLoginButton();
                return;
            }

            btnLogin.Content = "Đang đăng nhập...";

            LoginStatus loginStatus = await ClientWindow.Instance.Login(Account, Password); 

            if(loginStatus == LoginStatus.EmptyAccount)
            {
                MessageBox.Show("Tài khoản / mật khẩu không được trống.", "Đăng nhập");
                ResetLoginButton();
                return;
            }
            else
            {
                this.Close();
                ClientWindow.Instance.Show();
            }
        }

        private void ResetLoginButton()
        {
            btnLogin.IsEnabled = true;
            btnLogin.Content = "ĐĂNG NHẬP"; 
        }

        private void ImgTogglePassword_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (imgTogglePassword.Tag.ToString() == "Hide")
            {
                imgTogglePassword.Tag = "Show";
                imgTogglePassword.Source = new BitmapImage(new Uri("../resource/Show.png", UriKind.Relative));
                tbPassword.Focus();
                tbPassword.Visibility = Visibility.Hidden;
                pwbPassword.Visibility = Visibility.Visible;
                pwbPassword.Password = tbPassword.Text;
            }
            else
            {
                imgTogglePassword.Tag = "Hide";
                imgTogglePassword.Source = new BitmapImage(new Uri("../resource/Hide.png", UriKind.Relative));
                pwbPassword.Focus();
                pwbPassword.Visibility = Visibility.Hidden;
                tbPassword.Visibility = Visibility.Visible;
                tbPassword.Text = pwbPassword.Password;
            }
        }
    }
}
