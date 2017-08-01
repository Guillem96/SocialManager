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
    public class ContactReqPacket : Packet
    {
        private string from;
        private string to;

        [XmlElement]
        public string From { get => from; set => from = value; }

        [XmlElement]
        public string To { get => to; set => to = value; }


        internal ContactReqPacket(PacketTypes type, string alea, string from, string to) : base(type, alea)
        {
            From = from;
            To = to;
        }

        internal ContactReqPacket() : base() { }   
    }
}
