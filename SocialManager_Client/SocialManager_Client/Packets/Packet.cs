using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SocialManager_Client.Packets
{
    [Serializable]
    public class Packet
    {
        internal byte type;
        protected string alea;

        [XmlElement(ElementName = "Type")]
        public byte Type { get => type; set => type = value; }
        [XmlElement(ElementName = "Alea")]
        public string Alea { get => alea; set => alea = value; }

        public Packet()
        {

        }

        internal Packet(PacketTypes type, string alea)
        {
            this.Alea = alea;
            this.Type = (byte)type;
        }

        public virtual byte[] Pack()
        {
            return Encoding.ASCII.GetBytes(this.XmlSerializeToString());
        }

        public static T Unpack<T>(byte[] bytes)
        {
            return XmlUtilities.XmlDeserializeFromString<T>
                                                    (Encoding.ASCII.GetString(bytes));
        }

        public override string ToString()
        {
            return "Type: " + ((PacketTypes)Type).ToString() + Environment.NewLine +
                       "Alea: " + Alea;
        }
    }
}
