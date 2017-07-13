using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
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
            internal enum Status { Disconnected, Logged }

            private Models.Client client;
            private Status stat;
            private string alea;

            public Models.Client Client { get => client; set => client = value; }
            public Status Stat { get => stat; set => stat = value; }
            public string Alea { get => alea; set => alea = value; }

            public ClientStatus(Models.Client cli, Status stat = Status.Disconnected)
            {
                Client = cli;
                Alea = new Random(DateTime.Now.Millisecond).Next(0, 9999999).ToString();
                Stat = stat;
            }

            public override string ToString()
            {
                return Client.Username + " - " + Client.Password +" - "+stat.ToString() + " - " + alea;
            }
        }

        protected string name;
        protected Connections.UDPConnection udp;
        protected List<ClientStatus> clients;

        protected SuperSever(string name)
        {
            this.name = name;
            udp = new Connections.UDPConnection(11000);
            clients = new List<ClientStatus>();

            using (Models.ServerDatabase db = new Models.ServerDatabase())
            {
                clients.AddRange(db.Clients.ToArray().Select(c => new ClientStatus(c)));
            }
        }

        public void MainLoop()
        {
            DebugInfo("Server is running...");
            List<Thread> threads = new List<Thread>();
            threads.Add(new Thread(() => UDP()));
            threads.Add(new Thread(() => TCP()));
            threads.Add(new Thread(() => Commands(threads)));


            foreach (Thread t in threads)
                t.Start();   
        }

        protected abstract void UDP();
        protected abstract void TCP();

        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        private void Commands(List<Thread> threads)
        {
            DebugInfo("STDIN listening to commands.");
            while (true)
            {
                switch (Console.ReadLine())
                {
                    case "help":
                    case "HELP":
                    case "Help":
                        break;

                    case "List":
                    case "list":
                    case "LIST":
                        ListClientStatus();
                        break;
                    case "exit":
                        break;
                }
            }
            
        }

        private void ListClientStatus()
        {
            foreach(ClientStatus cs in clients)
            {
                Console.WriteLine(cs.ToString());
            }
        }

        protected void ChangeStatus(string username, ClientStatus.Status status, string alea)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Client.Username.Equals(username))
                {
                    clients[i].Stat = status;
                    clients[i].Alea = alea;
                }
            }
        }

        protected string GenerateAlea()
        {
            string alea = "";
            Random gen = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < 7; i++)
            {
                alea += gen.Next(0, 9).ToString();
            }

            return alea;
        }

        protected void DebugInfo(string message)
        {
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + message);
        }
    }
}
