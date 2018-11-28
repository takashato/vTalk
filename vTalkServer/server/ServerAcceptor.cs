using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace vTalkServer.server
{
    class MainServerAcceptor
    {
        private int port;
        private bool isStarted;
        private TcpListener listener;
        private CancellationTokenSource cts;

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
            cts = new CancellationTokenSource();
            HandleConnectionAsync(listener, cts.Token);
        }

        private async Task HandleConnectionAsync(TcpListener listener, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                //TcpClient client = await listener.AcceptTcpClientAsync();
                TcpClient client = await listener.AcceptTcpClientAsync();
                Program.mainForm.logger.WriteLine(client.Client.RemoteEndPoint + " Connected.");
            }

        }

        public int Port { get => port; set => port = value; }
        public bool IsStarted { get => isStarted; set => isStarted = value; }
    }
}
