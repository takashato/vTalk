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

namespace vTalkClient.gui.room
{
    /// <summary>
    /// Interaction logic for ImgurUpload.xaml
    /// </summary>
    public partial class ImgurUpload : Window
    {
        public ImgurUpload()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog() { Filter = "All Graphics Types|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|" + "BMP|*.bmp|GIF|*.gif|JPG|*.jpg;*.jpeg|PNG|*.png|TIFF|*.tif;*.tiff" };
            var result = ofd.ShowDialog();
            if (result == false) return;
            tbPath.Text = ofd.FileName;
        }

        private async void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            if(!"".Equals(tbPath.Text))
            {
                lbStatus.Dispatcher.Invoke(() => lbStatus.Content = "Đang upload...");
                string link = await ImgurUtils.UploadImage(tbPath.Text);
                var owner = (Owner as RoomWindow);
                owner.Dispatcher.Invoke(() => owner.SetTextBox(link));
                Dispatcher.Invoke(() => this.Close());
            }
        }
    }
}
