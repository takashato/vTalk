using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace vTalkServer.server
{
    class MainServerAcceptor
    {
        private int port;
        private bool isStarted;
        private TcpListener listener;

        public MainServerAcceptor(int port)
        {
            IsStarted = false;
            Port = port;
            listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            IsStarted = true;
            listener.Start();
            Program.mainForm.logger.WriteLine("Main Server is listening on "+Port+"...");
        }

        private void StartAccept()
        {
            listener.BeginAcceptTcpClient(HandleAsyncConnection, listener);
        }

        private void HandleAsyncConnection(IAsyncResult res)
        {
            StartAccept(); // Wait accept new connection
            TcpClient client = listener.EndAcceptTcpClient(res);
        }

        public int Port { get => port; set => port = value; }
        public bool IsStarted { get => isStarted; set => isStarted = value; }
    }
}
