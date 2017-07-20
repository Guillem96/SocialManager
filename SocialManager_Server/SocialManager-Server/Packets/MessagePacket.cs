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
    public class MessagePacket : Packet
    {
        private Models.Client from;
        private Models.Client to;
        private string content;
        private bool read;
        [XmlIgnore]
        public DateTime Date { get; set; }

        [XmlElement]
        public Models.Client From { get => from; set => from = value; }

        [XmlElement]
        public Models.Client To { get => to; set => to = value; }

        [XmlElement]
        public string Content { get => content; set => content = value; }

        [XmlElement]
        public bool Read { get => read; set => read = value; }

        [XmlElement("Date")]
        public string DateString { get => Date.ToString("yyyy-MM-dd HH:mm:ss"); set => Date = DateTime.Parse(value); }


        internal MessagePacket() : base () { }

        internal MessagePacket(PacketTypes type, string alea, Models.Client from, Models.Client to,string content, bool read, DateTime date) : base(type, alea)
        {
            From = from;
            To = to;
            Read = read;
            Date = date;
            Content = content;
        }

        public override string ToString()
        {
            return base.ToString() + ", From=" + from.Username + " ,To=" + to.Username + ", Content=" + content + "]";
        }
    }
}
