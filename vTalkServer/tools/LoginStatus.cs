using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vTalkServer.tools
{
    public enum LoginStatus : byte
    {
        EmptyAccount,
        CantConnectToServer,
        LoginFailed = 1,
        Connected,
        AlreadyLoggedin = 2,
        Success = 0
    }
}
