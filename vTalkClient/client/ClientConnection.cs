using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
                    Console.WriteLine("Bạn đã bị ngắt kết nối với server.");
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
                                    room.Window.Log.WriteUserMessage(user, DateTime.Now.ToString("H:m:s dd/MM/yyyy"), pr.ReadString());
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
            }
        }
    }
}
