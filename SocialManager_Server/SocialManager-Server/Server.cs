using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SocialManager_Server.Models;

namespace SocialManager_Server
{
    class Server
    {
        /// <summary>
        /// Client status. Contains his alea and his current status
        /// </summary>
        private class ClientStatus
        {
            internal enum Status { Disconnected, Alive, RegisterReq, LoginReq }

            private Client client;
            private Status stat;
            private string alea;

            public Client Client { get => client; set => client = value; }
            public Status Stat { get => stat; set => stat = value; }
            public string Alea { get => alea; set => alea = value; }

            ClientStatus(Client cli, Status stat = Status.Disconnected)
            {
                Client = cli;
                Alea = new Random(DateTime.Now.Millisecond).Next(0, 9999999).ToString();
                Stat = stat; 
            }
        }

        private string name;
        private Connections.UDPConnection udp;
        private List<ClientStatus> clients;

        public Server(string name)
        {
            this.name = name;
            udp = new Connections.UDPConnection(11000);
        }


        public void MainLoop()
        {
            Console.WriteLine("Server is running...");
            UDP();
        }

        /// <summary>
        /// Used for accepting connections and registering clients
        /// </summary>
        private void UDP()
        {
            while (true)
            {
                // Recieve the packet
                IPEndPoint tmp = new IPEndPoint(IPAddress.Any, 11000);
                var data = udp.RecieveMessage(ref tmp);

                // Read the type of the packet
                var packet = Packets.Packet.Unpack(data);

                // Depending on the type extract the remaning data
                switch (packet.Type)
                {
                    case Packets.PacketTypes.RegisterReq:
                        packet = Packets.RegisterReqPacket.Unpack(data);
                        Console.WriteLine(packet.ToString());
                        if (ClientsManagement.RegisterClient((Packets.RegisterReqPacket)packet))                       
                            udp.SendMessage(Encoding.ASCII.GetBytes("Has sido registrado."), tmp);
                        else
                            udp.SendMessage(Encoding.ASCII.GetBytes("Error. No registrado."), tmp);

                        
                        break;
                }
            }
        }

        /// <summary>
        /// Used for chat.
        /// </summary>
        private void TCP()
        {

        }


    }
}
