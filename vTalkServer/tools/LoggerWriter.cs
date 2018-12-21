using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vTalkServer.gui;

namespace vTalkServer.tools
{
    class LoggerWriter: TextWriter
    {
        Logger _output = null;

        public LoggerWriter(Logger output)
        {
            _output = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            _output.CrossthreadWrite(value.ToString());
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
