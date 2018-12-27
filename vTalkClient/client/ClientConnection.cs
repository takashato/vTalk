using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using vTalkClient.client;
using vTalkClient.tools;

namespace vTalkClient
{
    public class ClientConnection : IDisposable
    {
        private readonly Socket socket;
        private readonly byte[] socketBuffer;
        private readonly string host;
        private readonly int port;
        private readonly object dispose_Sync;
        private bool disposed;
        public PacketProcessor PProcessor { get; private set; }

        public bool Connected
        {
            get
            {
                return disposed == false;
            }
        }

        public string Host
        {
            get
            {
                return host;
            }
        }

        public int Port
        {
            get
            {
                return port;
            }
        }

        public ClientConnection(Socket socket)
        {
            this.socket = socket;
            socketBuffer = new byte[1024];

            host = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
            port = ((IPEndPoint)socket.RemoteEndPoint).Port;

            dispose_Sync = new object();


            PProcessor = new PacketProcessor();
            PProcessor.PacketFinished += (data) =>
            {
                RecvPacket(data);
            };

            WaitForData();
        }

        private void WaitForData()
        {
            if (!disposed)
            {
                SocketError error = SocketError.Success;
                socket.BeginReceive(socketBuffer, 0, socketBuffer.Length, SocketFlags.None, out error, OnPacketReceived, null);

                if (error != SocketError.Success)
                {
                    Disconnect();
                }
            }
        }

        private void OnPacketReceived(IAsyncResult iar)
        {
            if (!disposed)
            {
                SocketError error = SocketError.Success;
                int size = socket.EndReceive(iar, out error);

                if (size == 0 || error != SocketError.Success)
                {
                    Disconnect();
                }
                else
                {
                    PProcessor.AddData(socketBuffer, 0, size);
                    WaitForData();
                }
            }
        }

        public void Disconnect()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (dispose_Sync)
            {
                if (disposed == true)
                    return;

                disposed = true;

                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                finally
                {
                    //ClientForm.Instance.EnableChat(false);

                    //ClientForm.Instance.UserList = new Dictionary<string, string>();
                    MessageBox.Show("Bạn đã bị ngắt kết nối với server.", "Thông báo");
                    Environment.Exit(0);
                    /*try
                    {
                        Console.WriteLine(string.Format("{0}:{1} Disconnected", SClient.Host, SClient.Port));
                        Program.Users.Remove(IPEndPoint.ToString());
                        MainForm.Instance.RemoveClient(this);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: Error when disconneting client. " + e.ToString());
                    }*/
                }
            }
        }

        ~ClientConnection()
        {
            Dispose();
        }

        public void SendData(SendHeader dataType, byte[] pData)
        {
            byte[] data = new byte[pData.Length + PacketProcessor.HeaderSize];
            PacketWriter pw = new PacketWriter();
            pw.WriteInt(pData.Length);
            pw.WriteByte((byte)dataType);
            Buffer.BlockCopy(pw.ToArray(), 0, data, 0, pw.Length);
            Buffer.BlockCopy(pData, 0, data, PacketProcessor.HeaderSize, pData.Length);
            pData = data;
            try
            {
                SendPacket(pData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Gửi thất bại: {0}", ex.ToString());
            }
        }

        private void SendPacket(byte[] data)
        {
            if (!disposed)
            {
                int offset = 0;

                while (offset < data.Length)
                {
                    SocketError outError = SocketError.Success;
                    int sent = socket.Send(data, offset, data.Length - offset, SocketFlags.None, out outError);

                    if (sent == 0 || outError != SocketError.Success)
                    {
                        Disconnect();
                        return;
                    }
                    offset += sent;
                }
            }
        }

        internal void RecvPacket(byte[] packet)
        {
            PacketReader pr_raw = new PacketReader(packet);
            int packetLength = pr_raw.ReadInt();
            int dataType = pr_raw.ReadByte();
            byte[] data = new byte[packetLength];
            Buffer.BlockCopy(packet, PacketProcessor.HeaderSize, data, 0, packetLength);
            PacketReader pr;
            PacketWriter pw;
            switch ((RecvHeader)dataType)
            {
                case RecvHeader.LoginResult:
                    pr = new PacketReader(data);
                    LoginStatus status = (LoginStatus)pr.ReadByte();
                    if(status == LoginStatus.Success)
                    {
                        ClientWindow.Instance.AccountInfo = new account.AccountInfo();
                        ClientWindow.Instance.AccountInfo.Decode(pr);
                        ClientWindow.Instance.CloseLogin();
                        ClientWindow.Instance.MainUserInfo.Update();
                        // Request for Room list
                        pw = new PacketWriter();
                        SendData(SendHeader.RoomListRequest, pw.ToArray());
                    }
                    break;
                case RecvHeader.RoomList:
                    pr = new PacketReader(data);
                    int n = pr.ReadInt();
                    for (int i = 0; i<n; i++)
                    {
                        Room room = new Room();
                        room.Decode(pr);
                        ClientWindow.Instance.Rooms.Add(room.RoomId, room);
                        ClientWindow.Instance.RoomList.Update();
                    }
                    break;
                case RecvHeader.CreateRoomResult:
                    pr = new PacketReader(data);
                    if((RoomOperation)pr.ReadByte() != RoomOperation.Success)
                    {
                        MessageBox.Show("Tạo phòng thất bại!", "Thông báo");
                    }
                    ClientWindow.Instance.Dispatcher.Invoke(() => ClientWindow.Instance.RoomList._CreateRoomDialog.Hide());
                    break;
                case RecvHeader.RoomListUpdate:
                    pr = new PacketReader(data);
                    RoomOperation operation = (RoomOperation) pr.ReadByte();
                    if(operation == RoomOperation.New)
                    {
                        Room newRoom = new Room();
                        newRoom.Decode(pr);
                        ClientWindow.Instance.Rooms.Add(newRoom.RoomId, newRoom);
                        ClientWindow.Instance.RoomList.Update();
                    }
                    break;
                case RecvHeader.ServerMessage:
                    pr = new PacketReader(data);
                    ClientWindow.Instance.WriteLog(pr.ReadString());
                    break;
                case RecvHeader.JoinRoomResult:
                    pr = new PacketReader(data);
                    if(pr.ReadBool())
                    {
                        int roomId = pr.ReadInt();
                        if(ClientWindow.Instance.Rooms.ContainsKey(roomId))
                        {
                            var room = ClientWindow.Instance.Rooms[roomId];
                            ClientWindow.Instance.Dispatcher.Invoke(() =>
                            {
                                room.Window = new gui.room.RoomWindow(room);
                                room.Window.NoticeText = pr.ReadString();
                                room.Window.SetTitle("Phòng chat <" + room.Name + "> | vTalk");
                                room.Window.Show();
                                room.Window.Activate();
                            });
                        }
                        // Request for User list
                        pw = new PacketWriter();
                        pw.WriteInt(roomId);
                        SendData(SendHeader.UserListRequest, pw.ToArray());
                    }
                    else
                    {
                        MessageBox.Show("Bạn không thể tham gia phòng chat này. Vui lòng kiểm tra lại.", "Lỗi");
                    }
                    break;
                case RecvHeader.RoomMessage:
                    pr = new PacketReader(data);
                    int desRoomId = pr.ReadInt();
                    if(ClientWindow.Instance.Rooms.ContainsKey(desRoomId))
                    {
                        Room room = ClientWindow.Instance.Rooms[desRoomId];
                        if (room.Window != null)
                        {
                            switch ((ChatType)pr.ReadByte())
                            {
                                case ChatType.Message:
                                    room.Window.Log.WriteUserMessage("", "", pr.ReadString());
                                    break;
                                case ChatType.User:
                                    string user = pr.ReadString();
                                    string processedString = MessageProcessor.Process(pr.ReadString());
                                    room.Window.Log.WriteUserMessage(user, DateTime.Now.ToString("H:m:s dd/MM/yyyy"), processedString);
                                    ClientWindow.Instance.PlayNotificationSound();
                                    break;
                                case ChatType.Success:
                                    room.Window.Dispatcher.Invoke(() =>
                                    {
                                        room.Window.tbMessage.IsEnabled = true;
                                        room.Window.tbMessage.Text = "";
                                    });
                                    break;
                            }
                        }
                    }
                    break;
                case RecvHeader.UserList:
                    pr = new PacketReader(data);
                    long m = pr.ReadLong();
                    int roomID = pr.ReadInt();
                    Room uroom = ClientWindow.Instance.Rooms[roomID];
                    for (int i = 0; i < m; i++)
                    {
                        User usr = new User();
                        usr.Name = pr.ReadString();
                        int check = 0;
                        foreach (User user in uroom.Clients)
                        {
                            if (user.Name == usr.Name)
                                check = 1;
                        }
                        if (check == 0)
                            uroom.Clients.Add(usr);
                        uroom.Window.Dispatcher.Invoke(() =>
                        {
                            uroom.Window.userList.Items.Clear();
                            foreach (User client in uroom.Clients)
                            {
                                ListViewItem lvi = new ListViewItem
                                {
                                    Content = client.Name
                                };
                                uroom.Window.userList.Items.Add(lvi);
                            }
                        });
                    }
                    break;
                case RecvHeader.UserListUpdate:
                    pr = new PacketReader(data);
                    UserOperation userOperation = (UserOperation)pr.ReadByte();
                    int usroomId = pr.ReadInt();
                    //long o = pr.ReadLong();
                    Room usroom = ClientWindow.Instance.Rooms[usroomId];
                    if (userOperation == UserOperation.New)
                    {
                        User usr = new User();
                        usr.Name = pr.ReadString();
                        int check = 0;
                        foreach(User user in usroom.Clients)
                        {
                            if (user.Name == usr.Name)
                                check = 1;
                        }
                        if (check == 0)
                            usroom.Clients.Add(usr);
                        usroom.Window.Dispatcher.Invoke(() =>
                        {
                            usroom.Window.userList.Items.Clear();
                            foreach (User client in usroom.Clients)
                            {
                                ListViewItem lvi = new ListViewItem
                                {
                                    Content = client.Name
                                };
                                usroom.Window.userList.Items.Add(lvi);
                            }
                        });
                    }
                    else if(userOperation == UserOperation.Leave)
                    {
                        string username = pr.ReadString();
                        foreach (User user in usroom.Clients)
                        {
                            if (user.Name.Equals(username))
                            {
                                usroom.Clients.Remove(user);
                                usroom.Window.userList.Dispatcher.Invoke(() =>
                                {
                                    foreach (object item in usroom.Window.userList.Items)
                                    {
                                        ListViewItem u = (ListViewItem)item; 
                                        if (u.Content.Equals(username))
                                        {
                                            usroom.Window.userList.Items.Remove(item);
                                            break;
                                        }
                                    }
                                });
                                break;
                            }
                        }
                    }
                    break;
                case RecvHeader.LeaveRoomResult:
                    pr = new PacketReader(data);
                    int roomIDToLeave = pr.ReadInt();
                    if(pr.ReadBool()) // Leave successful
                    {
                        if (ClientWindow.Instance.Rooms.ContainsKey(roomIDToLeave))
                        {
                            if(ClientWindow.Instance.Rooms[roomIDToLeave].Window != null)
                            {
                                ClientWindow.Instance.Rooms[roomIDToLeave].Window.Dispatcher.Invoke(() =>
                                {
                                    ClientWindow.Instance.Rooms[roomIDToLeave].Window.Close();
                                    ClientWindow.Instance.Rooms[roomIDToLeave].Window = null;
                                });
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không thể rời phòng chat.", "Lỗi");
                    }
                    break;
            }
        }
    }
}
