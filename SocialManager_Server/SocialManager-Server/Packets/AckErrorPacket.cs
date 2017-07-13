using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SocialManager_Server.Packets
{
    /// <summary>
    /// Type of packet that is send in order to accept or refuse a package
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Packet")]
    public class AckErrorPacket : Packet
    {
        private string message;

        [XmlElement(ElementName = "Message")]
        public string Message { get => message; set => message = value; }

        public AckErrorPacket() : base() { }

        internal AckErrorPacket(Packets.PacketTypes type, string message) : base (type, "0000000")
        {
            Message = message;
        }

        public override string ToString()
        {
            return base.ToString() + ", Message=" + Message + "]";
        }
    }
}
