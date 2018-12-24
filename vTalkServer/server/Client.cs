using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using vTalkServer.constants;
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

                /*
                Program.Users.Remove(IPEndPoint.ToString());
                ServerForm.Instance.RemoveClient(this);
                ServerForm.Instance.UpdateUserList();*/
                Server.Instance.Clients.Remove(this);
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
                    Console.WriteLine("{0} gửi yêu cầu đăng nhập ({0} / {1})", account, password);
                    // Verify here
                    AccountInfo = new AccountInfo(account);
                    // Reply
                    pw = new PacketWriter();
                    pw.WriteByte((byte)LoginStatus.Success);
                    AccountInfo.Encode(pw); // Encode AccountInfo object
                    Connection.SendData(SendHeader.LoginResult, pw.ToArray());
                    break;
                case RecvHeader.RoomListRequest:
                    if (AccountInfo == null) return; // Not logged in
                    pw = new PacketWriter();
                    pw.WriteInt(Server.Instance.Rooms.Count);
                    foreach(var room in Server.Instance.Rooms)
                    {
                        room.Encode(pw);
                    }
                    Connection.SendData(SendHeader.RoomList, pw.ToArray());
                    break;
                case RecvHeader.CreateRoom:
                    pr = new PacketReader(data);
                    string name = pr.ReadString();
                    string pass = "";
                    if (pr.ReadBool()) pass = pr.ReadString();
                    Room newRoom = new Room(Server.Instance.GenerateRoomId(), name, pass, this);
                    Server.Instance.Rooms.Add(newRoom);
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
            }
        }
    }
}
