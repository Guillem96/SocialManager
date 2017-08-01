using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SocialManager_Server.Models;

namespace SocialManager_Server.Packets
{
    /// <summary>
    /// Packet for sending the contact requests
    /// </summary>
    [Serializable]
    [XmlRoot("Packet")]
    public class ContactReqListPacket : Packet
    {
        private List<ContactRequest> sent;      //< Waiting for other accept or declines
        private List<ContactRequest> recieved;  //< Your requests

        [XmlArray("SentContactsRequests")]
        [XmlArrayItem("Request")]
        public List<ContactRequest> Sent { get => sent; set => sent = value; }

        [XmlArray("RecievedContactsRequests")]
        [XmlArrayItem("Request")]
        public List<ContactRequest> Recieved { get => recieved; set => recieved = value; }

        internal ContactReqListPacket(PacketTypes type, 
                                        string alea, 
                                        List<ContactRequest> sent,
                                        List<ContactRequest> recieved) : base(type, alea)
        {
            Recieved = recieved;
            Sent = sent;
        }

        internal ContactReqListPacket() : base() { }
    }
}
