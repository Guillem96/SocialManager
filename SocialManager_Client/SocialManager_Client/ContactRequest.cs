using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SocialManager_Client
{
    [Serializable]
    [XmlRoot("ContactRequest")]
    public class ContactRequest
    {
        private Profile from;
        private Profile to;

        [XmlElement]
        public Profile From { get => from; set => from = value; }

        [XmlElement]
        public Profile To { get => to; set => to = value; }

        internal ContactRequest() { }

        public override string ToString()
        {
            return String.Format("From: {0} To: {1}", From.Username, To.Username);
        }
    }
}
