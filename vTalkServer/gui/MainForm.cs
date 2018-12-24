using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vTalkServer.constants;
using vTalkServer.gui;
using vTalkServer.server;
using vTalkServer.tools;

namespace vTalkServer
{
    public partial class MainForm : Form
    {
        private Server server;

        public MainForm()
        {
            InitializeComponent();
            server = new Server(ServerConstants.MAIN_PORT);
            server.OnClientConnected += ClientConnect;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Console.SetOut(new LoggerWriter(logger));
        }


        private void SwitchButton_Click(object sender, EventArgs e)
        {
            server.Start();
        }

        private void ClientConnect(Socket socket)
        {
            Client client = new Client(socket);
            Server.Instance.Clients.Add(client);
            Console.WriteLine("{0}:{1} đã kết nối!", client.IPEndPoint.Address, client.IPEndPoint.Port);
        }
    }
}
