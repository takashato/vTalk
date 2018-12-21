using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vTalkServer.tools;

namespace vTalkServer
{
    class AccountInfo
    {
        public string Account { get; set; }

        public AccountInfo(string account)
        {
            Account = account;
        }

        public void Encode(PacketWriter packetWriter)
        {
            packetWriter.WriteString(Account);
        }
    }
}
