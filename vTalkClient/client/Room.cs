using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vTalkClient.gui.room;
using vTalkClient.tools;

namespace vTalkClient.client
{
    public class Room
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public bool HasPassword { get; set; }

        public RoomWindow Window {get; set;}

        public Room()
        {
        }

        public void Decode(PacketReader pr)
        {
            RoomId = pr.ReadInt();
            Name = pr.ReadString();
            HasPassword = pr.ReadBool();
        }
    }
}
