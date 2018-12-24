using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vTalkServer.tools
{
    public enum SendHeader : byte
    {
        LoginResult = 0x0,
        RoomList = 0x1,
        CreateRoomResult = 0x2,
        RoomListUpdate = 0x3,
        ServerMessage = 0xFF
    }
}
