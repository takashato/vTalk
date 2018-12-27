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
using System.Windows.Shapes;
using vTalkClient.tools;

namespace vTalkClient.gui.main
{
    /// <summary>
    /// Interaction logic for CreateRoomDialog.xaml
    /// </summary>
    public partial class CreateRoomDialog : Window
    {
        public CreateRoomDialog()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            pbPassword.Password = "";
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            PacketWriter pw = new PacketWriter();
            pw.WriteString(tbName.Text);
            bool hasPassword = !"".Equals(pbPassword.Password);
            pw.WriteBool(hasPassword);
            if (hasPassword) pw.WriteString(pbPassword.Password);
            ClientWindow.Instance.Client.SendData(SendHeader.CreateRoom, pw.ToArray());


        }

        #region Process Placehoder "Tên phòng"
        private void TbName_ProcessFocus(object sender, RoutedEventArgs e)
        {
            if (tbName.Text == "Tên phòng")
            {
                tbName.Text = "";
            }
            else if (tbName.Text == "")
            {
                tbName.Text = "Tên phòng";
                tbName.Foreground = new BrushConverter().ConvertFrom("#666") as SolidColorBrush;
            }
        }

        private void TbName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbName.Text != "")
            {
                tbName.Foreground = new BrushConverter().ConvertFrom("#000") as SolidColorBrush;
            }
        }
        #endregion

    }
}
