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
    public class MessagePacket : Packet
    {
        private Profile from;
        private Profile to;
        private string content;
        private bool read;
        [XmlIgnore]
        public DateTime Date { get; set; }

        [XmlElement]
        public Profile From { get => from; set => from = value; }

        [XmlElement]
        public Profile To { get => to; set => to = value; }

        [XmlElement]
        public string Content { get => content; set => content = value; }

        [XmlElement]
        public bool Read { get => read; set => read = value; }

        [XmlElement("Date")]
        public string DateString { get => Date.ToString("yyyy-MM-dd HH:mm:ss"); set => Date = DateTime.Parse(value); }


        internal MessagePacket() : base () { }

        internal MessagePacket(PacketTypes type, string alea, Profile from, Profile to, bool read) : base(type, alea)
        {
            From = from;
            To = to;
            Read = read;
            Date = DateTime.Now;
        }

        public override string ToString()
        {
            return base.ToString() + ", From=" + from.Username + " ,To=" + to.Username + ", Content=" + content + "]";
        }
    }
}
