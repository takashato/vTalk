﻿using System;
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
        public static RoomWindow Instance { get; set; }
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

        private bool canClose = false;

        private Room Room { get; set; }      

        public ListView Users
        {
            get
            {
                return userList;
            }
        }

        public ChatBox Log
        {
            get
            {
                return chatBox;
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

        public void SetLeaveRoom()
        {
            canClose = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (canClose) return;

            MessageBoxResult res = MessageBox.Show("Bạn có muốn thoát phòng chat không?", "Thông báo", MessageBoxButton.YesNo);
            if(res == MessageBoxResult.Yes)
            {
                PacketWriter pw = new PacketWriter();
                pw.WriteInt(Room.RoomId);
                ClientWindow.Instance.Client.SendData(SendHeader.LeaveRoomRequest, pw.ToArray());
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
            Log.WriteUserMessage(ClientWindow.Instance.AccountInfo.Account, DateTime.Now.ToString("H:m:s dd/MM/yyyy"), MessageProcessor.Process(tbMessage.Text));
        }

        private void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                btnSend_Click(sender, new RoutedEventArgs());
            }
        }

        public void SetTextBox(string s)
        {
            tbMessage.Text = s;
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ImgurUpload imgurUpload = new ImgurUpload();
            imgurUpload.Owner = this;
            imgurUpload.Show();
        }
    }
}
