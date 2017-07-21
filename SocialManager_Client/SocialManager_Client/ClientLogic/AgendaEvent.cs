using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SocialManager_Client
{
    [Serializable]
    [XmlRoot("AgendaEvent")]
    public class AgendaEvent
    {
        private string eventName;
        private string eventInfo;

        [XmlIgnore]
        public DateTime Date { get; set; }

        [XmlElement("Date")]
        public string DateString { get => Date.ToString("yyyy-MM-dd HH:mm:ss"); set => Date = DateTime.Parse(value); }

        [XmlElement]
        public string EventName { get => eventName; set => eventName = value; }

        [XmlElement]
        public string EventInfo { get => eventInfo; set => eventInfo = value; }

        internal AgendaEvent() { }
    }
}
