using System;
using System.Text;
using System.Xml.Serialization;

namespace SocialManager_Server.Packets
{
    /// <summary>
    /// Base class of all type of packages
    /// </summary>
    [Serializable]
    public class Packet
    { 
        internal byte type;
        protected string alea;

        [XmlElement(ElementName = "Type")]
        public byte Type { get => type; set => type = (byte)value; }
        [XmlElement(ElementName = "Alea")]
        public string Alea { get => alea; set => alea = value; }

        public Packet() { }

        internal Packet(PacketTypes type, string alea)
        {
            Alea = alea;
            Type = (byte)type;
        }

        /// <summary>
        /// Pack a Packet object into bytes
        /// </summary>
        /// <returns>Packet object in byte array</returns>
        public virtual byte[] Pack()
        {
            return Encoding.ASCII.GetBytes(this.XmlSerializeToString());
        }

        /// <summary>
        /// Unpacks a byte array into packet object
        /// </summary>
        /// <typeparam name="T">Type to unpack</typeparam>
        /// <param name="bytes">Packet in bytes</param>
        /// <returns>Instance of packet T</returns>
        public static T Unpack<T>(byte[] bytes)
        {
            return XmlUtilities.XmlDeserializeFromString<T>
                                                        (Encoding.ASCII.GetString(bytes));
        }

        public override string ToString()
        {
            return "[Type=" + ((Packets.PacketTypes)type).ToString() + ", Alea=" + Alea;
        }
    }
}
