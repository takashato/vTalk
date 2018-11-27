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
            if (IsStarted) return;
            IsStarted = true;
            listener.Start();
            Program.mainForm.logger.WriteLine("Main Server is listening on "+Port+"...");
        }

        private void StartAccept()
        {
            listener.BeginAcceptTcpClient(AcceptConnectionAsync, listener);
        }

        private async void AcceptConnectionAsync(IAsyncResult res)
        {
            StartAccept(); // Wait accept new connection
            var client = await listener.AcceptTcpClientAsync();
            Program.mainForm.logger.WriteLine(client.Client.RemoteEndPoint + " connected!");
        }

        public int Port { get => port; set => port = value; }
        public bool IsStarted { get => isStarted; set => isStarted = value; }
    }
}
