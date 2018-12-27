using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vTalkServer.server.packet;
using vTalkServer.tools;

namespace vTalkServer.server
{
    class Room
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Notice { get; set; } = "Chào mừng các bạn đã tham gia phòng chat!";

        public List<Client> Clients { get; set; } = new List<Client>();
        
        public Client Owner { get; set; }

        public Room(int roomId, string name, string password, Client owner)
        {
            RoomId = roomId;
            Name = name;
            Password = password;
            Owner = owner;
        }

        public void Broadcast(tools.SendHeader dataType, byte[] data)
        {
            foreach(var client in Clients)
            {
                client.Connection.SendData(dataType, data);
            }
        }

        public void Broadcast(Client c, tools.SendHeader dataType, byte[] data)
        {
            foreach (var client in Clients)
            {
                if (client == c) continue;
                client.Connection.SendData(dataType, data);
            }
        }

        public void Encode(PacketWriter pw)
        {
            pw.WriteInt(RoomId);
            pw.WriteString(Name);
            pw.WriteBool(Password != null);
        }

        public void RemoveIfJoined(Client client)
        {
            if (Clients.Contains(client))
            {
                Clients.Remove(client);
                PacketWriter pw = RoomPacket.ServerMessage(RoomId, client.AccountInfo.Account + " vừa rời khỏi phòng chat.");
                Broadcast(SendHeader.RoomMessage, pw.ToArray());

                //user list update
                pw = new PacketWriter();
                pw.WriteByte((byte)UserOperation.Leave);
                pw.WriteInt(RoomId);
                pw.WriteString(client.AccountInfo.Account);
                Server.Instance.Broadcast(SendHeader.UserListUpdate, pw.ToArray());
            }
        }
    }
}
