using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SocialManager_Client.Packets
{
    [Serializable]
    [XmlRoot("Packet")]
    public class ClientQueryPacket : Packet
    {
        private string username;
        private string query;
        private List<string> usernames;

        [XmlElement]
        public string Username { get => username; set => username = value; }

        [XmlElement]
        public string Query { get => query; set => query = value; }

        [XmlArray("QueryResult")]
        [XmlArrayItem("User")]
        public List<string> Usernames { get => usernames; set => usernames = value; }

        internal ClientQueryPacket() : base()
        {

        }

        internal ClientQueryPacket(PacketTypes type, string alea, string username, string query) : base(type, alea)
        {
            this.username = username;
            this.query = query;
        }

        
    }
}
