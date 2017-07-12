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
        private static IPEndPoint DefaultUdpEndPoint = new IPEndPoint(IPAddress.Any, 11000);

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
