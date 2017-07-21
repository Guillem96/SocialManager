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
    public class AgendaEventPacket : Packet
    {
        private string username;
        private string eventName;
        private string eventInfo;

        [XmlIgnore]
        public DateTime Date { get; set; }

        [XmlElement]
        public string Username { get => username; set => username = value; }

        [XmlElement]
        public string EventName { get => eventName; set => eventName = value; }

        [XmlElement]
        public string EventInfo { get => eventInfo; set => eventInfo = value; }

        [XmlElement("Date")]
        public string DateString { get => Date.ToString("yyyy-MM-dd HH:mm:ss"); set => Date = DateTime.Parse(value); }


        internal AgendaEventPacket(PacketTypes type, string alea, string username, string eventName, string eventInfo, DateTime date) : base(type, alea)
        {
            Username = username;
            EventName = eventName;
            EventInfo = eventInfo;
            Date = date;
        }

        internal AgendaEventPacket() : base() { }

        public override string ToString()
        {
            return base.ToString() + ", Client=" + Username + ", EventName=" + EventName + ", Date=" + Date + "]";
        }
    }
}
