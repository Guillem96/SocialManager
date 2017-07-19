using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SocialManager_Server.Models
{
    [Table(Name="Messages")]
    [Serializable]
    [XmlRoot("Message")]
    public class Message
    {
        private int messageID;
        private bool read = false;
        private string content;

        [Column(Name = "From")]
        private int fromID;     //< FK refering to the client who send the message

        [Column(Name = "To")]
        private int toID;       //< FK refering to the client who recieve the message

        [Column(CanBeNull = false)]
        [XmlIgnore]
        public DateTime Date { get; set; }

        private EntityRef<Client> from = new EntityRef<Client>();
        private EntityRef<Client> to = new EntityRef<Client>();

        [XmlIgnore]
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int MessageID { get => messageID; set => messageID = value; }

        [XmlElement("From")]
        [Association(Name = "FK_From_Message",
                        Storage = "from",
                        IsForeignKey = true,
                        ThisKey = "fromID",
                        OtherKey = "ClientID")]
        public Client From { get => from.Entity; set => from.Entity = value; }

        [XmlElement("To")]
        [Association(Name = "FK_To_Message",
                        Storage = "to",
                        IsForeignKey = true,
                        ThisKey = "toID",
                        OtherKey = "ClientID")]
        public Client To { get => to.Entity; set => to.Entity = value; }

        [XmlElement("Date")]
        public string DateString { get => Date.ToString("yyyy-MM-dd HH:mm:ss"); set => Date = DateTime.Parse(value); }

        [Column]
        [XmlElement("Content")]
        public string Content { get => content; set => content = value; }

        [XmlElement]
        [Column(Name = "Readed")]
        public bool Read { get => read; set => read = value; }

        public static bool operator==(Message l, Message r)
        {
            return l.To.Username.Equals(r.To.Username) &&
                    l.From.Username.Equals(r.From.Username) &&
                    l.Content.Equals(r.Content) &&
                    l.Date.Equals(r.Date);
        }

        public static bool operator !=(Message l, Message r)
        {
            return !(l == r);
        }
    }
}
