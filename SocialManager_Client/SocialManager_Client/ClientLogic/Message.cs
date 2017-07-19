using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SocialManager_Client
{
    [Serializable]
    [XmlRoot("Message")]
    public class Message
    {
        private bool read;
        private Profile from;
        private Profile to;
        private string content;
        [XmlIgnore]
        public DateTime Date { get; set; }


        [XmlElement("From")]
        public Profile From { get => from; set => from = value; }
 
        [XmlElement("To")]
        public Profile To { get => to; set => to = value; }

        [XmlElement("Date")]
        public string DateString { get => Date.ToString("yyyy-MM-dd HH:mm:ss"); set => Date = DateTime.Parse(value); }

        [XmlElement("Read")]
        public bool Read { get => read; set => read = value; }

        [XmlElement("Content")]
        public string Content { get => content; set => content = value; }

        internal Message() { }
    }
}
