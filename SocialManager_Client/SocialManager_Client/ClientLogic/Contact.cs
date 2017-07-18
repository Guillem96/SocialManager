using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SocialManager_Client
{
    [Serializable]
    [XmlRoot("Contact")]
    public class Contact
    {
        public enum Status { Disconnected, Logged }

        private Profile profile;
        private Status stat;

        [XmlElement("Profile")]
        public Profile Profile { get => profile; set => profile = value; }
        [XmlElement("Status")]
        public Status Stat { get => stat; set => stat = value; }

        internal Contact() { }
    }
}
