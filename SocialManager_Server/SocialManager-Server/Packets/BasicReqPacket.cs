using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SocialManager_Server.Packets
{
    /// <summary>
    /// Basic petition always contains the username of the user who is making the requests
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Packet")]
    public class BasicReqPacket : Packet
    {
        private string username;

        [XmlElement(ElementName = "Username")]
        public string Username { get => username; set => username = value; }

        public BasicReqPacket() : base() { }

        internal BasicReqPacket(PacketTypes type, string alea, string username) : base(type, alea)
        {
            Username = username;
        }

        public override string ToString()
        {
            return base.ToString() + ", Username=" + Username + "]";
        }
    }
}
