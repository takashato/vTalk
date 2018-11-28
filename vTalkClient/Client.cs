using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace vTalkClient
{
    class Client
    {
        public static Client Instance
        {
            get;
            set;
        } = new Client();

        public TcpClient Client_ { get; set; }
        public IPAddress Address { get; set; }

        public Client()
        {
            Client_ = new TcpClient();
        }

        public async Task<bool> Connect()
        {
            if (Address == null) return false;
            try
            {
                await Client_.ConnectAsync(Address, 8449);
                return true;
            } 
            catch(SocketException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
