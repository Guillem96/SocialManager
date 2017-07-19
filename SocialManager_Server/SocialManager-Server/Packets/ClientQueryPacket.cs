using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SocialManager_Server.Packets
{
    [Serializable]
    [XmlRoot("Packet")]
    public class ClientQueryPacket : Packet
    {
        private string username;
        private string query;
        private List<Models.Client> profiles;

        [XmlElement]
        public string Username { get => username; set => username = value; }

        [XmlElement]
        public string Query { get => query; set => query = value; }

        [XmlArray("QueryResult")]
        [XmlArrayItem("User")]
        public List<Models.Client> Profiles { get => profiles; set => profiles = value; }

        internal ClientQueryPacket() : base()
        {

        }

        internal ClientQueryPacket(PacketTypes type, string alea, List<Models.Client> q) : base(type, alea)
        {
            Profiles = q;
        }

        
    }
}
