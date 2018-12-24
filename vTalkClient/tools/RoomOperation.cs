using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vTalkClient.tools
{
    public enum RoomOperation : byte
    {
        New = 0,
        Remove = 1,
        Update = 2,
        Success = 3,
    }
}
