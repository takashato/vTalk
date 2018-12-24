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
using System.Windows.Shapes;
using vTalkClient.client;
using vTalkClient.tools;

namespace vTalkClient.gui.room
{
    /// <summary>
    /// Interaction logic for RoomWindow.xaml
    /// </summary>
    public partial class RoomWindow : Window
    {
        public string NoticeText
        {
            get
            {
                return noticeText.Text;
            }

            set
            {
                noticeText.Text = value;
            }
        }

        private Room Room { get; set; }

        public ChatLog Log
        {
            get
            {
                return chatLog;
            }
        }

        public RoomWindow()
        {
            InitializeComponent();
        }

        public RoomWindow(Room room) : this()
        {
            Room = room;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Bạn có muốn thoát phòng chat không?", "Thông báo", MessageBoxButton.YesNo);
            if(res == MessageBoxResult.Yes)
            {

            }
            else
            {
                this.Hide();
            }
            e.Cancel = true;
        }

        public void SetTitle(string title)
        {
            Title = title;
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if ("".Equals(tbMessage.Text)) return;
            PacketWriter pw = new PacketWriter();
            pw.WriteInt(Room.RoomId);
            pw.WriteString(tbMessage.Text);
            ClientWindow.Instance.Client.SendData(SendHeader.TextChat, pw.ToArray());
            tbMessage.IsEnabled = false;
        }

        private void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                btnSend_Click(sender, new RoutedEventArgs());
            }
        }
    }
}
