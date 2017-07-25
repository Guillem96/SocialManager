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
    public class LinkSocialNetworkPacket : Packet
    {
        private string socialNetName;
        private string socialNetUsername;
        private string socialNetPassword;
        private string clientUsername;

        [XmlElement]
        public string SocialNetName { get => socialNetName; set => socialNetName = value; }

        [XmlElement]
        public string SocialNetUsername { get => socialNetUsername; set => socialNetUsername = value; }

        [XmlElement]
        public string SocialNetPassword { get => socialNetPassword; set => socialNetPassword = value; }

        [XmlElement]
        public string ClientUsername { get => clientUsername; set => clientUsername = value; }

        public LinkSocialNetworkPacket() : base() { }

        internal LinkSocialNetworkPacket(PacketTypes type, string alea, string clientUsername, string socialNetworkName, string username, string password) : base(type, alea)
        {
            ClientUsername = clientUsername;
            SocialNetName = socialNetworkName;
            SocialNetPassword = password;
            SocialNetUsername = username;
        }

        public override string ToString()
        {
            return base.ToString() + "SocialNet Name=" + SocialNetName + ", Client Username=" + ClientUsername + ", SocialNet Username=" + SocialNetUsername + ", SocialNet Password=" + SocialNetPassword + "]";
        }
    }
}
