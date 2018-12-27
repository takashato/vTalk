using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using vTalkServer.constants;
using vTalkServer.server.packet;
using vTalkServer.tools;

namespace vTalkServer.server
{
    class Client
    {
        public ClientConnection Connection { get; set; }

        public bool Connected { get; set; }

        public IPEndPoint IPEndPoint { get; set; }

        public AccountInfo AccountInfo { get; set; }

        public Client(Socket session)
        {
            this.Connection = new ClientConnection(this, session);
            IPEndPoint = Connection.IPEndPoint;
            Connected = true;
        }

        public void Disconnect(string reason, params object[] values)
        {
            Console.WriteLine("Ngắt kết nối client: " + string.Format(reason, values));
            if (Connection != null)
                Connection.Disconnect();
        }

        internal void Disconnected()
        {
            try
            {
                if (Connected)
                {
                    Console.WriteLine(string.Format("{0}:{1} đã ngắt kết nối.", Connection.Host, Connection.Port));
                }
                Connected = false;

                Server.Instance.Clients.Remove(this);
                foreach(var pair in Server.Instance.Rooms)
                {
                    pair.Value.RemoveIfJoined(this);
                }
                Connection.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: Gặp lỗi khi ngắt kết nối client. " + e.ToString());
            }
        }

        internal void RecvPacket(byte[] rawData)
        {
            PacketReader pr_raw = new PacketReader(rawData);
            int packetLength = pr_raw.ReadInt();
            int dataType = pr_raw.ReadByte();
            byte[] data = new byte[packetLength];
            Buffer.BlockCopy(rawData, PacketProcessor.HeaderSize, data, 0, packetLength);

            PacketWriter pw;
            PacketReader pr;

            switch ((RecvHeader)dataType) // PACKET PROCESS HERE!!!
            {
                case RecvHeader.Login:
                    pr = new PacketReader(data);
                    string account = pr.ReadString();
                    string password = pr.ReadString();
                    // Verify here
                    AccountInfo = new AccountInfo(account);
                    // Reply
                    pw = new PacketWriter();
                    pw.WriteByte((byte)LoginStatus.Success);
                    AccountInfo.Encode(pw); // Encode AccountInfo object
                    Connection.SendData(SendHeader.LoginResult, pw.ToArray());
                    Console.WriteLine("{0} đã đăng nhập ({0} / {1})", account, password);
                    Server.Instance.SendMessage(account + " vừa đăng nhập.");
                    break;
                case RecvHeader.RoomListRequest:
                    if (AccountInfo == null) return; // Not logged in
                    pw = new PacketWriter();
                    pw.WriteInt(Server.Instance.Rooms.Count);
                    foreach(var room in Server.Instance.Rooms)
                    {
                        room.Value.Encode(pw);
                    }
                    Connection.SendData(SendHeader.RoomList, pw.ToArray());
                    break;
                case RecvHeader.CreateRoom:
                    pr = new PacketReader(data);
                    string name = pr.ReadString();
                    string pass = null;
                    if (pr.ReadBool()) pass = pr.ReadString();
                    Room newRoom = new Room(Server.Instance.GenerateRoomId(), name, pass, this);
                    Server.Instance.Rooms.Add(newRoom.RoomId, newRoom);
                    // Create Result
                    pw = new PacketWriter();
                    pw.WriteByte((byte)RoomOperation.Success);
                    Connection.SendData(SendHeader.CreateRoomResult, pw.ToArray());
                    // Update Room!
                    pw = new PacketWriter();
                    pw.WriteByte((byte)RoomOperation.New);
                    newRoom.Encode(pw);
                    Server.Instance.Broadcast(SendHeader.RoomListUpdate, pw.ToArray());
                    break;
                case RecvHeader.JoinRoomRequest:
                    pr = new PacketReader(data);
                    int roomId = pr.ReadInt();
                    bool hasPassword = pr.ReadBool();
                    Room rRoom = null;
                    foreach(var room in Server.Instance.Rooms)
                    {
                        if(room.Value.RoomId == roomId)
                        {
                            rRoom = room.Value;
                            break;
                        }
                    }

                    bool isSuccess = false;

                    if(rRoom != null)
                    {
                        if(hasPassword == (rRoom.Password != null))
                        {
                            string sPass = null;
                            if(hasPassword)
                            {
                                sPass = pr.ReadString();
                            }

                            if((sPass == null && rRoom.Password == null) || sPass.Equals(rRoom.Password))
                            {
                                isSuccess = true;
                            }
                        }
                    }

                    pw = new PacketWriter();
                    pw.WriteBool(isSuccess);
                    if (isSuccess)
                    {
                        pw.WriteInt(roomId);
                        pw.WriteString(rRoom.Notice);
                    }
                    Connection.SendData(SendHeader.JoinRoomResult, pw.ToArray());

                    if (isSuccess)
                    {
                        // Broadcast to Room
                        pw = RoomPacket.ServerMessage(roomId, AccountInfo.Account + " vừa tham gia phòng chat.");
                        rRoom.Broadcast(SendHeader.RoomMessage, pw.ToArray());

                        //user list update
                        pw = new PacketWriter();
                        pw.WriteByte((byte)UserOperation.New);
                        pw.WriteInt(roomId);
                        //pw.WriteLong(Server.Instance.Clients.Count);
                        pw.WriteString(this.AccountInfo.Account);
                        rRoom.Broadcast(SendHeader.UserListUpdate, pw.ToArray());

                        // Add user to this room
                        rRoom.Clients.Add(this);
                    }
                    break;
                case RecvHeader.TextChat:
                    pr = new PacketReader(data);
                    int desRoomId = pr.ReadInt();
                    string message = pr.ReadString();
                    if(Server.Instance.Rooms.ContainsKey(desRoomId))
                    {
                        Room cRoom = Server.Instance.Rooms[desRoomId];
                        if(cRoom.Clients.Contains(this)) // Joined this room
                        {
                            // Broadcast chat
                            pw = new PacketWriter();
                            pw.WriteInt(cRoom.RoomId);
                            pw.WriteByte((byte)ChatType.User);
                            pw.WriteString(AccountInfo.Account);
                            pw.WriteString(message);
                            cRoom.Broadcast(this, SendHeader.RoomMessage, pw.ToArray());
                            // Response success
                            pw = new PacketWriter();
                            pw.WriteInt(cRoom.RoomId);
                            pw.WriteByte((byte)ChatType.Success);
                            Connection.SendData(SendHeader.RoomMessage, pw.ToArray());
                        }
                    }
                    break;
                case RecvHeader.UserListRequest:
                    pr = new PacketReader(data);
                    int roomID = pr.ReadInt();
                    pw = new PacketWriter();
                    pw.WriteLong(Server.Instance.Clients.Count);
                    pw.WriteInt(roomID);
                    foreach (var client in Server.Instance.Clients)
                    {
                        pw.WriteString(client.AccountInfo.Account);
                    }
                    Connection.SendData(SendHeader.UserList, pw.ToArray());
                    break;
                case RecvHeader.LeaveRoomRequest:
                    pr = new PacketReader(data);
                    int roomIDToLeave = pr.ReadInt();
                    if (Server.Instance.Rooms.ContainsKey(roomIDToLeave))
                    {
                        Room cRoom = Server.Instance.Rooms[roomIDToLeave];
                        if (cRoom.Clients.Contains(this)) // Joined this room
                        {
                            cRoom.RemoveIfJoined(this);
                            pw = new PacketWriter();
                            pw.WriteInt(roomIDToLeave);
                            pw.WriteBool(true);
                            Connection.SendData(SendHeader.LeaveRoomResult, pw.ToArray());
                        }
                    }
                    break;
               
            }
        }
    }
}
