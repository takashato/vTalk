using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vTalkServer.tools
{
    public enum RecvHeader : ushort
    {
        Login = 0,
        RoomListRequest = 1,
        CreateRoom = 2
    }
}
