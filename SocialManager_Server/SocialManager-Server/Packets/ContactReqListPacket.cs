using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SocialManager_Server.Models;

namespace SocialManager_Server.Packets
{
    [Serializable]
    [XmlRoot("Packet")]
    public class ContactReqListPacket : Packet
    {
        private List<ContactRequest> requests;

        [XmlArray("ContactsRequests")]
        [XmlArrayItem("Request")]
        public List<ContactRequest> Requests { get => requests; set => requests = value; }

        internal ContactReqListPacket(PacketTypes type, 
                                        string alea, 
                                        List<ContactRequest> requests) : base(type, alea)
        {
            Requests = requests;
        }

        internal ContactReqListPacket() : base() { }

        public override string ToString()
        {
            return base.ToString() + 
                                String.Join(" | ", Requests.Select( r => "From=" + r.From.Username + ", To=" + r.To.Username)) + "]";
        }
    }
}
