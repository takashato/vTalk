using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vTalkClient.tools
{
    public enum LoginStatus : byte
    {
		Success,
		LoginFailed,
		AlreadyLoggedin,
		EmptyAccount,
		CantConnectToServer,
		Connected,
	}
}
