using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using SocialManager_Server.ClientLogic;
using System.Threading;
using SocialManager_Server.Connections;
using System.Net.Sockets;

namespace SocialManager_Server.ServerLogic
{
    abstract class SuperServer
    {
        protected string name;                                      //< Server name
        private UDPConnection udp;                                  //< Udp connection 
        private TCPConnection tcp;                                  //< Tcp connection 
        private List<ClientStatus> clients;                         //< Store the clients status
        private Dictionary<string, TcpClient> clientsOnChat;        //< Clients ready for chat
        private List<Thread> tasks;                                 //< Store a reference to all server tasks

        // Alive timer
        System.Timers.Timer timer;

        // True if server is up
        protected bool serverUp;

        public UDPConnection Udp { get => udp; set => udp = value; }
        public TCPConnection Tcp { get => tcp; set => tcp = value; }
        public List<ClientStatus> Clients { get => clients; set => clients = value; }
        public Dictionary<string, TcpClient> ClientsOnChat { get => clientsOnChat; set => clientsOnChat = value; }

        protected SuperServer(string name)
        {
            this.name = name;
            Udp = new UDPConnection();
            Clients = new List<ClientStatus>();
            ClientsOnChat = new Dictionary<string, TcpClient>();

            serverUp = true;

            using (Models.ServerDatabase db = new Models.ServerDatabase())
            {
                Clients.AddRange(db.Clients.ToArray().Select(c => new ClientStatus(c)));
            }

            // Establish timer to check alives every 3 seconds. 
            // When a user has't sent an AliveInf Packet in 12 seconds its status changes to disconnected.
            timer = new System.Timers.Timer()
            {
                Interval = 3000,
                Enabled = true
            };
            timer.Elapsed += (sender, e) => UdpUtilities.CheckAlives((Server)this);
        }

        protected abstract void UDP();
        protected abstract void TCP();

        /// <summary>
        /// Start all server tasks in an infinite loop
        /// </summary>
        public void MainLoop()
        {
            DebugInfo("Server is running...");
            tasks = new List<Thread>
            {
                new Thread(new ThreadStart(() => UDP())),
                new Thread(new ThreadStart(() => TCP())),
                new Thread(new ThreadStart(() => Commands())),
            };
            
            // Start all tasks
            foreach (Thread t in tasks)
                t.Start();

            // Wait all tasks
            foreach (Thread t in tasks)
                t.Join();

            Console.Write("Press a key to close the window.");
            Console.ReadKey();
        }

        /// <summary>
        /// Read commands from stdin
        /// </summary>
        private void Commands()
        {
            DebugInfo("STDIN listening to commands.");
            while (serverUp)
            {
                switch (Console.ReadLine())
                {
                    case "help":
                    case "HELP":
                    case "Help":
                        DebugInfo("Available commands:" + Environment.NewLine +
                                    "\t- List: List status of all clients in database." + Environment.NewLine +
                                    "\t- ListChat: List clients on chat." + Environment.NewLine +
                                    "\t- Exit: End all server tasks." + Environment.NewLine +
                                    "\t- Help: Show this text.");
                        break;

                    case "List":
                    case "list":
                    case "LIST":
                        ListClientStatus();
                        break;
                    case "ListChat":
                    case "listchat":
                    case "LISTCHAT":
                        ListClientChat();
                        break;
                    case "exit":
                        serverUp = false;
                        timer.Enabled = false;
                        timer.Dispose();
                        DebugInfo("Alive timer is disabled.");
                        DebugInfo("Closing server.");
                        return;
                    case "":
                        break;
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
            // List all clients at the moment
            Console.WriteLine(String.Format("|{0,15}|{1,15}|{2,15}|", "Username", "Alea", "State"));
            foreach (ClientStatus cs in Clients)
            {
                Console.WriteLine(cs.ToString());
            }
        }

        /// <summary>
        /// List all clients on chat.
        /// </summary>
        private void ListClientChat()
        {
            // List all clients at the moment
            Console.WriteLine(String.Format("|{0,15}|", "Username"));
            foreach (string username in ClientsOnChat.Keys)
            {
                Console.WriteLine(String.Format("|{0,15}|", username));
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
        public void SetStatus(string username, ClientStatus.Status status, string alea, DateTime lastAlive)
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

        /// <summary>
        /// Removes the client with username "username" from database
        /// </summary>
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
        /// Returns the contacts of a user
        /// </summary>
        public List<ClientStatus> GetContacts(ClientStatus current)
        {
            using (var db = new Models.ServerDatabase())
            {
                List<string> contactsUsername =
                                   db.Contacts
                                       .Where(c => c.Client1.Username == current.Client.Username || c.Client2.Username == current.Client.Username)
                                       .Select(c => c.Client1.Username == current.Client.Username ? c.Client2.Username : c.Client1.Username).ToList();

                return contactsUsername.Select(c => GetClient(c)).ToList();
            }   
        }

        /// <summary>
        /// Return a list of non read messages of a user
        /// </summary>
        public List<Models.Message> GetUnreadMessages(ClientStatus current)
        {
            var db = new Models.ServerDatabase();
            
            return db.Messages
                    .Where(c => c.To.Username == current.Client.Username && c.Read == false)
                    .ToList();
        }

        /// <summary>
        /// Change unread messages to read if they had been read by the indicated user
        /// </summary>
        public void MarkReadMessages(ClientStatus current)
        {
            // Update the messages
            using (var db = new Models.ServerDatabase())
            {
                var dbMessages =
                        db.Messages
                                .Where(m => m.To.Username == current.Client.Username && m.Read == false)
                                .ToList();

                foreach(var message in dbMessages)
                {
                    message.Read = true;
                }

                db.SubmitChanges();
            }
        }

        /// <summary>
        /// Return the list of eventsof user with username "username"
        /// </summary>
        public List<Models.AgendaEvent> GetAgendaEvents(string username)
        {
            using (var db = new Models.ServerDatabase())

                return db.AgendaEvents
                        .Where(c => c.Client.Username == username)
                        .ToList();
        
        }

        /// <summary>
        /// Shows info from stdout
        /// </summary>
        public void DebugInfo(string message)
        {
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + message);
        }

        /// <summary>
        /// Return the list of linked social nets of user with username "username"
        /// </summary>
        public List<Models.LinkedSocialNetwork> GetSocialNetworks(string username)
        {
            var db = new Models.ServerDatabase();

            return db.LinkedSocialNetworks.Where(l => l.Client.Username.Equals(username)).ToList();
        }

    }
}
