using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vTalkClient.tools
{
    public enum SendHeader : ushort
    {
        Login = 0x0,
        RoomListRequest = 0x1,
        CreateRoom = 0x2,
        JoinRoomRequest = 0x3,
        TextChat = 0x4,
        UserListRequest = 0x5,
        UserListUpdateRequest = 0x6
    }
}
