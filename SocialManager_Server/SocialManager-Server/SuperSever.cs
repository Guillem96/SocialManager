using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SocialManager_Server
{
    abstract class SuperSever
    {
        /// <summary>
        /// Client status. Contains his alea and his current status
        /// </summary>
        protected class ClientStatus
        {
            internal enum Status { Disconnected, Alive, RegisterReq, LoginReq }

            private Models.Client client;
            private Status stat;
            private string alea;

            public Models.Client Client { get => client; set => client = value; }
            public Status Stat { get => stat; set => stat = value; }
            public string Alea { get => alea; set => alea = value; }

            ClientStatus(Models.Client cli, Status stat = Status.Disconnected)
            {
                Client = cli;
                Alea = new Random(DateTime.Now.Millisecond).Next(0, 9999999).ToString();
                Stat = stat;
            }
        }

        protected string name;
        protected Connections.UDPConnection udp;
        protected List<ClientStatus> clients;

        protected SuperSever(string name)
        {
            this.name = name;
            udp = new Connections.UDPConnection(11000);
        }

        public void MainLoop()
        {
            DebugInfo("Server is running...");
            Thread u = new Thread(() => UDP());
            Thread t = new Thread(() => TCP());

            u.Start();
            t.Start();

            u.Join();
            t.Join();
        }

        protected abstract void UDP();
        protected abstract void TCP();

        protected void DebugInfo(string message)
        {
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + message);
        }
    }
}
