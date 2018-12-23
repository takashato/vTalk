using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vTalkServer.tools;

namespace vTalkServer.server
{
    class Room
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

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

        public void Encode(PacketWriter pw)
        {
            pw.WriteInt(RoomId);
            pw.WriteString(Name);
            pw.WriteBool(Password != null);
        }
    }
}
