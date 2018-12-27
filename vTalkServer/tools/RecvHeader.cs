using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vTalkServer.tools
{
    public enum RecvHeader : byte
    {
        Login = 0,
        RoomListRequest = 1,
        CreateRoom = 2,
        JoinRoomRequest = 3,
        TextChat = 4,
        UserListRequest = 5,
        UserListUpdateRequest = 6
    }
}
