using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vTalkServer.constants;

namespace vTalkServer.server
{
    class Server
    {
        private TcpListener listener;

        public int Port { get; set; }
        public bool IsStarted { get; set; }

        public delegate void ClientConnectedHandler(Socket client);
        public event ClientConnectedHandler OnClientConnected;

        public Server(int port)
        {
            IsStarted = false;
            Port = port;
            listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            if (IsStarted) return;
            listener.Start();
            Console.WriteLine("Main Server đang lắng nghe trên port "+Port+"...");
            IsStarted = true;
            listener.BeginAcceptSocket(OnClientConnect, null);
        }

        public void OnClientConnect(IAsyncResult iar)
        {
            Socket clientSocket;
            try
            {
                clientSocket = listener.EndAcceptSocket(iar);
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("EndAccept Server: {0}", e));
                return;
            }

            try
            {
                listener.BeginAcceptSocket(OnClientConnect, null);
                if (OnClientConnected != null)
                {
                    OnClientConnected(clientSocket);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Server Error: {0}", ex));
            }
        }
    }
}
