using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using vTalkServer.constants;
using vTalkServer.server;

namespace vTalkServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new mainForm());
        }

        public static ServerAcceptor acceptor = new ServerAcceptor(ServerConstants.MAIN_PORT);

        public static void StartServer()
        {
            acceptor.Start();
        }
    }
}
