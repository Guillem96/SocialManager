using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Xml.Serialization;

namespace SocialManager_Server.Models
{
    [Serializable]
    [XmlRoot("LinkedSocialNetwork")]
    [Table(Name = "LinkedSocialNetworks")]
    public class LinkedSocialNetwork
    {
        [Column(Name = "ClientID", CanBeNull = false)]
        private int clientID;

        [Column(Name = "SocialNetworkID", CanBeNull = false)]
        private int socialNetworkID;

        private int linkedID;

        private EntityRef<Client> client;
        private EntityRef<SocialNetwork> socialNetwork;

        private string username;
        private string password;

        [Column(CanBeNull = false)]
        [XmlElement("Username")]
        public string Username { get => username; set => username = value; }

        [Column(CanBeNull = false)]
        [XmlElement("Password")]
        public string Password { get => password; set => password = value; }

        [XmlIgnore]
        [Association(Name = "FK_LinkedSN_Client",
                        Storage = "client",
                        IsForeignKey = true,
                        ThisKey = "clientID",
                        OtherKey = "ClientID")]
        public Client Client { get => client.Entity; set => client.Entity = value; }

        [XmlIgnore]
        [Association(Name = "FK_LinkedSN_SN",
                        Storage = "socialNetwork",
                        IsForeignKey = true,
                        ThisKey = "socialNetworkID",
                        OtherKey = "SocialNetworkID")]
        public SocialNetwork SocialNetwork { get => socialNetwork.Entity; set => socialNetwork.Entity = value; }

        // Avoid serialize all Social network class
        [XmlElement]
        public string Name { get => socialNetwork.Entity.Name; set { } }

        [XmlIgnore]
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int LinkedID { get => linkedID; set => linkedID = value; }

        public LinkedSocialNetwork() { }
    }
}
