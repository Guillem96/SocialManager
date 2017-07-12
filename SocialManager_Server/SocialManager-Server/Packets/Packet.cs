using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SocialManager_Server.Packets
{
    [SerializableAttribute]
    public class Packet
    { 
        internal PacketTypes type;
        protected string alea;

        [XmlElement(ElementName = "Type")]
        internal PacketTypes Type { get => type; set => type = value; }
        [XmlElement(ElementName = "Alea")]
        public string Alea { get => alea; set => alea = value; }

        public Packet()
        {

        }

        internal Packet(PacketTypes type, string alea)
        {
            this.Alea = alea;
            this.Type = type;
        }

        public virtual byte[] Pack()
        {
            return Encoding.ASCII.GetBytes(this.XmlSerializeToString());
        }

        public static Packet Unpack(byte[] bytes)
        {
            return XmlUtilities.XmlDeserializeFromString<Packet>
                                                            (Encoding.ASCII.GetString(bytes));
        }

        public override string ToString()
        {
            return "Type: " + Type.ToString() + Environment.NewLine +
                        "Alea: " + Alea;
        }
    }
}
