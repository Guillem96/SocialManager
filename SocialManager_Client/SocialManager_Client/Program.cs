using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new UdpClient();
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000); // endpoint where server is listening
            client.Connect(ep);

            // send data
            client.Send(new byte[] { 1, 2, 3, 4, 5 }, 5);

            // then receive data
            var receivedData = client.Receive(ref ep);

            Console.Write(Encoding.ASCII.GetString(receivedData));

            Console.Read();
        }
    }
}
