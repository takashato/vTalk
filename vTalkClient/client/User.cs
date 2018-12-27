using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vTalkClient.gui.room;
using vTalkClient.tools;

namespace vTalkClient.client
{
    public class User
    {
        //public int RoomId { get; set; }
        public string Name { get; set; }
        //public RoomWindow Window {get; set;}

        public User()
        {
        }

       /* public void Decode(PacketReader pr)
        {
            Name = pr.ReadString();
           // RoomId = pr.ReadInt();
        }*/
    }
}
