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
            // Testing register
            var client = new UdpClient();
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000); // endpoint where server is listening
            client.Connect(ep);

            Packets.RegisterReqPacket reg = 
                        new Packets.RegisterReqPacket("0000000", "Guillem", "Orellana", 21, "600886706", Profile.Sex.Male, "Guillem96", "1234", "guillem.orellana@gmail.com");
            //Console.WriteLine(Packets.RegisterReqPacket.Unpack(reg.Pack()).ToString());
            //// send data
            byte[] data = Encoding.ASCII.GetBytes(reg.XmlSerializeToString());
            client.Send(data, data.Length);

            //// then receive data
            var receivedData = client.Receive(ref ep);

            Console.Write(Encoding.ASCII.GetString(receivedData));

            Console.Read();
        }
    }
}
