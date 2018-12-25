using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace vTalkClient.gui.room
{
    /// <summary>
    /// Interaction logic for ChatBox.xaml
    /// </summary>
    public partial class ChatBox : UserControl
    {
        public ChatBox()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UseTemplate();
        }

        public void UseTemplate()
        {
            string curDir = Directory.GetCurrentDirectory();
            browser.Navigate(String.Format("file:///{0}/resource/template.html", curDir));
        }

        public void WriteUserMessage(string user, string time, string text)
        {
            Dispatcher.Invoke(() =>
            {
                browser.InvokeScript("newMessage", new string[] { user, time, text });
            });
        }
    }
}
