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
        public static MainForm mainForm;
        private static MainServerAcceptor acceptor = new MainServerAcceptor(ServerConstants.MAIN_PORT);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mainForm = new MainForm();
            Application.Run(mainForm);
            
        }


        public static void StartServer()
        {
            acceptor.Start();
        }
    }
}
