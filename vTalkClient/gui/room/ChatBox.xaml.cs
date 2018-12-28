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
        private bool isLoaded = false;
        private Queue<string[]> messageQueue = new Queue<string[]>();

        public ChatBox()
        {
            InitializeComponent();
            UseTemplate();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public void UseTemplate()
        {
            try
            {
                string template = File.ReadAllText("resource/template.html");
                browser.NavigateToString(template);
            } catch (Exception ex)
            {
            }
        }

        public void WriteUserMessage(string user, string time, string text)
        {
            string[] message = new string[] { user, time, text };
            if (!isLoaded) messageQueue.Enqueue(message);
            else
            {
                Dispatcher.Invoke(() =>
                {
                    browser.InvokeScript("newMessage", message);
                });
            }
        }

        private void browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            isLoaded = true;
            while(messageQueue.Count>0)
            {
                string[] message = messageQueue.Dequeue();
                Dispatcher.Invoke(() =>
                {
                    browser.InvokeScript("newMessage", message);
                });
            }
        }
    }
}
