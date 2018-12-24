using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using System.Windows.Threading;
using vTalkClient.account;
using vTalkClient.client;
using vTalkClient.gui.main;
using vTalkClient.tools;

namespace vTalkClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        public static ClientWindow Instance { get; set; }
        private LoadingScreen loadingScreen;
        private LoginScreen loginScreen;

        public MainUserInfo MainUserInfo {
            get
            {
                return userInfo;
            }
        }

        public RoomList RoomList
        {
            get
            {
                return roomList;
            }
        }

        public ClientConnection Client { get; set; }

        public AccountInfo AccountInfo { get; set; }

        public List<Room> Rooms { get; set; } = new List<Room>();

        public ClientWindow()
        {
            Instance = this;
            InitializeComponent();
            loadingScreen = new LoadingScreen();
            loginScreen = new LoginScreen();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadingScreen.Owner = this;
            loginScreen.Owner = this;
            this.Hide();
            loginScreen.ShowDialog();
        }
        
        public Task<LoginStatus> Connect(string serverIP)
        {
            return Task.Run(() =>
            {
                if (Client != null && Client.Connected)
                {
                    return LoginStatus.Connected;
                }

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socket.Connect(serverIP, 8449);
                }
                catch (SocketException ex)
                {
                    return LoginStatus.CantConnectToServer;
                }

                Client = new ClientConnection(socket);

                return LoginStatus.Connected;
            });
        }

        public Task<LoginStatus> Login(string account, string password)
        {
            return Task.Run(() =>
            {
                PacketWriter pw = new PacketWriter();
                if("".Equals(account) || "".Equals(password))
                {
                    return LoginStatus.EmptyAccount;
                }
                pw.WriteString(account);
                pw.WriteString(password);
                Client.SendData(SendHeader.Login, pw.ToArray());
                return LoginStatus.Success;
            });
        }

        public void CloseLogin()
        {
            if (AccountInfo == null) return;
            Dispatcher.Invoke(() => 
            {
                loginScreen.Close();
                this.Show();
                this.IsEnabled = true;
            }, DispatcherPriority.Normal);
        }

        public void WriteLog(string message)
        {
            eventLog.WriteLine(message);
        }
    }
}
