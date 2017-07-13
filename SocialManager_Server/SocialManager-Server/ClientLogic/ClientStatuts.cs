using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Server.ClientLogic
{
    /// <summary>
    /// Client status. Contains his alea and his current status
    /// </summary>
    class ClientStatus
    {
        internal enum Status { Disconnected, Logged }

        private Models.Client client;
        private Status stat;
        private string alea;
        private DateTime lastAlive;

        public Models.Client Client { get => client; set => client = value; }
        public Status Stat { get => stat; set => stat = value; }
        public string Alea { get => alea; set => alea = value; }
        public DateTime LastAlive { get => lastAlive; set => lastAlive = value; }

        public ClientStatus(Models.Client cli, Status stat = Status.Disconnected)
        {
            Client = cli;
            Alea = "0000000";
            Stat = stat;
        }

        // Disconnects a user from server
        public void Disconnect()
        {
            Alea = "0000000";
            Stat = Status.Disconnected;
        }

        public override string ToString()
        {
            return Client.Username + " - " + Client.Password + " - " + stat.ToString() + " - " + alea;
        }
    }
}
