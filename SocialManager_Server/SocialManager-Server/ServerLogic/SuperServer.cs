using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using SocialManager_Server.ClientLogic;
using System.Threading;
using SocialManager_Server.Connections;

namespace SocialManager_Server.ServerLogic
{
    abstract class SuperServer
    {
        protected string name;              //< Server name
        private UDPConnection udp;          //< Udp socket 
        private List<ClientStatus> clients; //< Store the clients status
        private List<Thread> tasks;         //< Store a reference to all server tasks

        public UDPConnection Udp { get => udp; set => udp = value; }
        public List<ClientStatus> Clients { get => clients; set => clients = value; }

        protected SuperServer(string name)
        {
            this.name = name;
            Udp = new UDPConnection(11000);
            Clients = new List<ClientStatus>();

            using (Models.ServerDatabase db = new Models.ServerDatabase())
            {
                Clients.AddRange(db.Clients.ToArray().Select(c => new ClientStatus(c)));
            }

            // Establish timer to check alives every 3 seconds. 
            // When a user has't sent an AliveInf Packet in 12 seconds its status changes to disconnected.
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 3000;
            timer.Elapsed += (sender, e) => UdpUtilities.CheckAlives((Server)this);
            timer.Enabled = true;
        }

        protected abstract void UDP();
        protected abstract void TCP();

        /// <summary>
        /// Start all server tasks in an infinite loop
        /// </summary>
        public void MainLoop()
        {
            DebugInfo("Server is running...");
            tasks = new List<Thread>();
            tasks.Add(new Thread(() => UDP()));
            tasks.Add(new Thread(() => TCP()));
            tasks.Add(new Thread(() => Commands()));

            foreach (Thread t in tasks)
                t.Start();
        }

        /// <summary>
        /// Read commands from stdin
        /// </summary>
        private void Commands()
        {
            DebugInfo("STDIN listening to commands.");
            while (true)
            {
                switch (Console.ReadLine())
                {
                    case "help":
                    case "HELP":
                    case "Help":
                        DebugInfo("Available commands:" + Environment.NewLine +
                                    "\t- List: List status of all clients in database." + Environment.NewLine +
                                    "\t- Exit: End all server tasks." + Environment.NewLine +
                                    "\t- Help: Show this text.");
                        break;

                    case "List":
                    case "list":
                    case "LIST":
                        ListClientStatus();
                        break;
                    case "exit":
                        return;
                    default:
                        DebugInfo("This command does not exist. Type help to know more about commands.");
                        break;

                }
            }
            
        }

        /// <summary>
        /// List all registerd clients.
        /// </summary>
        private void ListClientStatus()
        {
            foreach(ClientStatus cs in Clients)
            {
                Console.WriteLine(cs.ToString());
            }
        }

        /// <summary>
        /// Return the client's status with username "username"
        /// </summary>
        public ClientStatus GetClient(string username)
        {
            return Clients
                        .Any(c => c.Client.Username == username)
                        ? clients.Single(c => c.Client.Username == username)
                        : null;
        }

        /// <summary>
        /// Modifies the status of a client.
        /// </summary>
        public void ChangeStatus(string username, ClientStatus.Status status, string alea, DateTime lastAlive)
        {
            ClientStatus cs = GetClient(username);
            cs.Stat = status;
            cs.Alea = alea;
            cs.LastAlive = lastAlive;
        }

        /// <summary>
        /// Generates a string with 7 numbers
        /// </summary>
        public static string GenerateAlea()
        {
            string alea = "";
            Random gen = new Random();

            for (int i = 0; i < 7; i++)
            {
                alea += gen.Next(0, 9).ToString();
            }

            return alea;
        }

        public void DeleteCLientFromDataBase(string username)
        {
            using (var db = new Models.ServerDatabase())
            {
                // Delete its contacts
                db.Contacts.DeleteAllOnSubmit(
                    db.Contacts.Where(c => c.Client1.Username == username ||
                                            c.Client2.Username == username)
                );

                // Delete the client
                db.Clients.DeleteOnSubmit(
                        db.Clients.Single(c => c.Username == username)
                    );

                db.SubmitChanges();

                // Delete from registered list
                clients.Remove(clients.Single(c => c.Client.Username == username));
            }
        }

        /// <summary>
        /// Shows info from stdout
        /// </summary>
        public void DebugInfo(string message)
        {
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + message);
        }
    }
}
