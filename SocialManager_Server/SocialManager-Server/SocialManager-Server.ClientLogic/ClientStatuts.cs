using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SocialManager_Server.ClientLogic
{
    /// <summary>
    /// Client status. Contains his alea and his current state
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Contact")]
    public class ClientStatus
    {
        public enum Status { Disconnected, Logged }

        private Models.Client client;   //< Reference to client information
        private Status stat;            //< Client state (online or disconnected)      
        private string alea;            //< Alea number
        private DateTime lastAlive;     //< Last alive recieved

        [XmlElement("Profile")]
        public Models.Client Client { get => client; set => client = value; }
        [XmlElement("Status")]
        public Status Stat { get => stat; set => stat = value; }
        [XmlIgnore]
        public string Alea { get => alea; set => alea = value; }
        [XmlIgnore]
        public DateTime LastAlive { get => lastAlive; set => lastAlive = value; }

        internal ClientStatus() { }

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
            return String.Format("|{0,15}|{1,15}|{2,15}|", Client.Username, Alea, Stat.ToString());
        }
    }
}
