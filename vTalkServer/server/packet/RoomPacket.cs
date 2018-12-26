using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vTalkServer.tools;

namespace vTalkServer.server.packet
{
    public class RoomPacket
    {
        public static PacketWriter ServerMessage(int roomId, string message)
        {
            PacketWriter pw = new PacketWriter();
            pw.WriteInt(roomId);
            pw.WriteByte((byte)ChatType.Message);
            pw.WriteString(message);
            return pw;
        }


    }
}
