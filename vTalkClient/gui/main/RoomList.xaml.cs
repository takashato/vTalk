using System;
using System.Collections.Generic;
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
using vTalkClient.client;

namespace vTalkClient.gui.main
{
    /// <summary>
    /// Interaction logic for RoomList.xaml
    /// </summary>
    public partial class RoomList : UserControl
    {
        public ListView Rows
        {
            get
            {
                return rows;
            }
        }

        public CreateRoomDialog _CreateRoomDialog { get; set; }

        public RoomList()
        {
            InitializeComponent();
            _CreateRoomDialog = new CreateRoomDialog();
        }

        public void Update()
        {
            Dispatcher.Invoke(() =>
            {
                Rows.Items.Clear();
                foreach (var room in ClientWindow.Instance.Rooms)
                {
                    var lvi = new ListViewItem();
                    lvi.Tag = room.Value;
                    lvi.MouseDoubleClick += new MouseButtonEventHandler(JoinRoom);
                    lvi.Content = room.Value.Name;
                    Rows.Items.Add(lvi);
                }
            });
        }

        private void btnCreateRoom_Click(object sender, RoutedEventArgs e)
        {
            _CreateRoomDialog.Show();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _CreateRoomDialog.Owner = ClientWindow.Instance;
        }

        public void JoinRoom(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ListViewItem)) return;
            var lvi = sender as ListViewItem;
            var room = lvi.Tag as Room;
            if(room.Window != null)
            {
                room.Window.Show();
                room.Window.Activate();
                return;
            }
            string pass = null;
            if (room.HasPassword)
            {
                PasswordDialog passwordDialog = new PasswordDialog();
                passwordDialog.Owner = ClientWindow.Instance;
                if(passwordDialog.ShowDialog() == true)
                {
                    pass = passwordDialog.Password;
                }
                else
                {
                    return;
                }
            }
            ClientWindow.Instance.Join(room, pass);
        }
    }
}
