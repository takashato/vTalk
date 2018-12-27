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
using WMPLib;

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

        private System.Windows.Forms.NotifyIcon notifyIcon;

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

        public Dictionary<int, Room> Rooms { get; set; } = new Dictionary<int, Room>();

        public WindowsMediaPlayer NotifySound { get; set; } = new WindowsMediaPlayer();

        public ClientWindow()
        {
            Instance = this;
            InitializeComponent();
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Click += new EventHandler(notifyIcon_Click);
            loadingScreen = new LoadingScreen();
            loginScreen = new LoginScreen();
            // Sound
            NotifySound.URL = "resource/notification.mp3";
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            this.Show();
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

                Dispatcher.InvokeAsync(() =>
                {
                   this.userInfo.serverIp.Content = loginScreen.ServerIP;
                   this.userInfo.username.Content = loginScreen.Account;
                   this.userInfo.status.Content = "Activated";
                });

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

        public void Join(Room room, string pass)
        {
            if(room.Window != null) // Joined!
            {
                if (!room.Window.IsVisible)
                {
                    room.Window.Show();
                    room.Window.Activate();
                }
            }
            else
            {
                PacketWriter pw = new PacketWriter();
                pw.WriteInt(room.RoomId);
                pw.WriteBool(pass != null);
                if (pass != null) pw.WriteString(pass);
                Client.SendData(SendHeader.JoinRoomRequest, pw.ToArray());
            }
        }

        public Task PlayNotificationSound()
        {
            return Task.Run(() =>
            {
                NotifySound.controls.currentPosition = 0.0D;
                NotifySound.controls.play();
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var res = MessageBox.Show("Bạn có muốn thực sự thoát không?", "Thông báo", MessageBoxButton.YesNo);
            if(res == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}
