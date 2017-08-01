using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SocialManager_Server.Packets
{
    [Serializable]
    [XmlRoot(ElementName = "Packet")]
    public class LoginReqPacket : Packet
    {
        private string username;
        private string password;

        [XmlElement(ElementName = "Username")]
        public string Username { get => username; set => username = value; }
        [XmlElement(ElementName = "Password")]
        public string Password { get => password; set => password = value; }

        public LoginReqPacket() { }

        internal LoginReqPacket(PacketTypes type, string alea, string username, string password) : base(type, alea)
        {
            Username = username;
            Password = password;
        }

        public override string ToString()
        {
            return base.ToString() + ", Username=" + Username + ", Password=" + Password + "]";
        }
    }
}
