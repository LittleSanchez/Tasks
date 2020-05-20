using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
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

namespace PhoneList
{
    /// <summary>
    /// Interaction logic for ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        public Action<IPEndPoint> SetConnection { get; set; }
        public ConnectionWindow(Action<IPEndPoint> action)
        {
            InitializeComponent();
            SetConnection = action;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetConnection(new IPEndPoint(IPAddress.Parse(tbAddress.Text), int.Parse(tbPort.Text)));
                Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error); 
            }
        }
    }
}
