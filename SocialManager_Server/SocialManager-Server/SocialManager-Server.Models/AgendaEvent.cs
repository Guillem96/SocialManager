using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Xml.Serialization;
using System.Data.Linq;

namespace SocialManager_Server.Models
{
    [Serializable]
    [XmlRoot("AgendaEvent")]
    [Table(Name = "AgendaEvents")]
    public class AgendaEvent
    {

        [Column(Name = "ClientID", CanBeNull = false)]
        private int clientID;
        private int agendaEventID;

        private EntityRef<Client> owner;
        private string eventName;
        private string eventInfo;

        [XmlIgnore]
        [Column(CanBeNull = false)]
        public DateTime Date { get; set; }

        [XmlElement("Date")]
        public string DateString { get => Date.ToString("yyyy-MM-dd HH:mm:ss"); set => Date = DateTime.Parse(value); }

        [XmlIgnore]
        [Association(Name = "FK_Event",
                        Storage = "owner",
                        IsForeignKey = true,
                        ThisKey = "clientID",
                        OtherKey = "ClientID")]
        public Client Client { get => owner.Entity; set => owner.Entity = value; }

        [Column(CanBeNull = false)]
        [XmlElement]
        public string EventName { get => eventName; set => eventName = value; }

        [Column(CanBeNull = false)]
        [XmlElement]
        public string EventInfo { get => eventInfo; set => eventInfo = value; }

        [XmlIgnore]
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int AgendaEventID { get => agendaEventID; set => agendaEventID = value; }

        public AgendaEvent() { }
    }
}
