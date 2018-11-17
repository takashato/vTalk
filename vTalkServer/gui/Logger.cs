using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vTalkServer.constants;

namespace vTalkServer.gui
{
    public partial class Logger : UserControl
    {
        public Logger()
        {
            InitializeComponent();
        }

        private void Write(String text)
        {
            if (output.Items.Count >= ServerConstants.MAX_LOGGER_LINES)
            {
                output.Items.RemoveAt(0);
            }
            output.Items.Add(text);
            output.SelectedIndex = output.Items.Count - 1;
        }

        public void CrossthreadWrite(String text)
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action)(() =>
                {
                    Write(text);
                }));
            } else
            {
                Write(text);
            }
        }

        public void WriteLine(String text)
        {
            CrossthreadWrite(DateTime.Now + " || " + text + Environment.NewLine);
        }

        public void WriteLine(String type, String text)
        {
            WriteLine("["+ type +"] " + text);
        }
    }
}
