using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vTalkServer.gui;

namespace vTalkServer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void SwitchButton_Click(object sender, EventArgs e)
        {
            Program.StartServer();
        }
    }
}
