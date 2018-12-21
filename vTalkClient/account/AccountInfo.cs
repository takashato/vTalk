using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vTalkClient.tools;

namespace vTalkClient.account
{
    public class AccountInfo
    {
        public String Account { get; set; }

        public AccountInfo()
        {

        }

        public void Decode(PacketReader pr)
        {
            Account = pr.ReadString();
        }
    }
}
