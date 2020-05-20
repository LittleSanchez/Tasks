using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using Newtonsoft.Json;

namespace PhoneList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public const int SIZE = 4096;


        ObservableCollection<Dictionary<string, string>> Phones = new ObservableCollection<Dictionary<string, string>>();

        public Socket Socket { get; set; }
        public IPEndPoint EndPoint { get; set; }

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            new ConnectionWindow(SetConnection).ShowDialog();
            listPhones.ItemsSource = Phones;
        }

        public void SetConnection(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
        }

        public void Request(string message)
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect(EndPoint);
            Socket.Send(Encoding.UTF8.GetBytes(message));
            byte[] buff = new byte[SIZE];
            Socket.BeginReceive(buff, 0, SIZE, 0, EndReceiveCallback, new ReceiveData { Socket = Socket, Buffer = buff });
        }

        public void SetNewPhonesCollection(List<Dictionary<string, string>> list)
        {
            Dispatcher.Invoke(() =>
            {
                Phones.Clear();
                foreach (var item in list)
                {
                    Phones.Add(item);
                }
            });
        }

        private void EndReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (ar.AsyncState as ReceiveData).Socket;
            var buff = (ar.AsyncState as ReceiveData).Buffer;
            socket.EndReceive(ar);
            var sr = new StringReader(Encoding.UTF8.GetString(buff));
            var list = (List<Dictionary<string, string>>)new JsonSerializer().Deserialize(sr, typeof(List<Dictionary<string, string>>));
            SetNewPhonesCollection(list);
            socket.Close();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string message = tbSearch.Text;
            Request(message);
        }

        private void listPhones_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText((listPhones.SelectedItem as Dictionary<string, string>)["Phone"]);
        }
    }


    class ReceiveData
    {
        public byte[] Buffer { get; set; }
        public Socket Socket { get; set; }
    }
}
