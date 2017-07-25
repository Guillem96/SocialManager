using System;
using System.Xml.Serialization;

namespace SocialManager_Client
{
    [Serializable]
    [XmlRoot("SocialNetwork")]
    public class SocialNetwork
    {
        private string name;
        private string username;
        private string password;

        public string Name { get => name; set => name = value; }

        [XmlElement]
        public string Username { get => username; set => username = value; }

        [XmlElement]
        public string Password { get => password; set => password = value; }

        public SocialNetwork()
        {

        }
    }
}
