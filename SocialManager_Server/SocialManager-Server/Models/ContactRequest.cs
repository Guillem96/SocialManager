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
    [Serializable]
    [XmlRoot("ContactRequest")]
    [Table(Name="ContactRequests")]
    public class ContactRequest
    {
        private int contactRequestID;

        [Column(Name ="From")]
        private int fromID;

        [Column(Name = "To")]
        private int toID;

        private EntityRef<Client> from = new EntityRef<Client>();
        private EntityRef<Client> to = new EntityRef<Client>();

        [XmlIgnore]
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int ContactRequestID { get => contactRequestID; set => contactRequestID = value; }

        [XmlElement("From")]
        [Association(Name = "FK_From", 
                        Storage = "from", 
                        IsForeignKey = true, 
                        ThisKey = "fromID", 
                        OtherKey = "ClientID")]
        public Client From { get => from.Entity; set => from.Entity = value; }

        [XmlElement("To")]
        [Association(Name = "FK_To", 
                        Storage = "to", 
                        IsForeignKey = true, 
                        ThisKey = "toID", 
                        OtherKey = "ClientID")]
        public Client To { get => to.Entity; set => to.Entity = value; }

        public ContactRequest() { }

        public override string ToString()
        {
            return String.Format("From: {0} To: {1}", From.Username, To.Username);
        }
    }
}
