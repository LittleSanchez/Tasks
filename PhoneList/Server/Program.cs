using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {

        const int PORT = 2020;
        const int SIZE = 1024;
        static void Main(string[] args)
        {
            List<Phones> list = null;
            using (PhoneListModel context = new PhoneListModel())
            {
                list =  context.Phones.ToList();
            }

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            socket.Listen(10);
            while (true)
            {
                try
                {
                    var client = socket.Accept();
                    byte[] buff = new byte[SIZE];
                    int len = client.Receive(buff);
                    string line = Encoding.UTF8.GetString(buff, 0, len);
                    Phones[] names = list.Where(x => x.Name.ToLower().StartsWith(line.ToLower())).ToArray();
                    List<Dictionary<string, string>> sendData = new List<Dictionary<string, string>>();
                    foreach (var item in names)
                    {
                        sendData.Add(new Dictionary<string, string>());
                        sendData.Last().Add(nameof(item.Name), item.Name);
                        sendData.Last().Add(nameof(item.Phone), item.Phone);
                    }
                    var writer = new StringWriter();
                    new JsonSerializer().Serialize(writer, sendData);
                    client.Send(Encoding.UTF8.GetBytes(writer.ToString()));

                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
                catch(SocketException se)
                {
                    Console.WriteLine(se.Message);
                }
            }
        }
    }
}
