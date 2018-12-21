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
            output.Items.Add("");
        }

        public void Write(String text)
        {
            if (output.Items.Count >= ServerConstants.MAX_LOGGER_LINES)
            {
                output.Items.RemoveAt(0);
            }

            if (text.Equals("\n"))
            {
                output.Items.Add("");
            }

            output.Items[output.Items.Count - 1] = (output.Items[output.Items.Count - 1] as String) + text;
            output.SelectedIndex = output.Items.Count - 2;
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
